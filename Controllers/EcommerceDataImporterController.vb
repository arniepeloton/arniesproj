Public Class EcommerceDataImporterController
    Inherits System.Web.Mvc.AsyncController

    Private Shared mEcommerceDataSync As New EcommerceDataSync
    Dim mFolder As String

    Public Function OnlineStatus() As String

        Return "online"

    End Function

    Public Function IsBusy() As String

        Return mEcommerceDataSync.IsBusy

    End Function

    Public Function Upload() As String

        Dim pOrgID As String
        Dim pError As String
        Dim pPrivateDatabaseName As String

        pOrgID = Common.WebRequest.Data("OrgID")
        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return JsonDictionary.Serialize("ErrorMessage", pError)
        End If
        If Request.Files.Count = 0 Then
            Return JsonDictionary.Serialize("ErrorMessage", "At least 1 file is expected.")
        End If
        pPrivateDatabaseName = WebRequest.Instance.ProData.GetCurrentDatabase()
        Dim pKey As String = GenerateString(30)
        Dim pFilename = Common.ServerPath & "temp\EcommerceDataImporter\" & pOrgID & "\" & pKey & "\data.zip"
        CreateFolder(pFilename)
        Request.Files(0).SaveAs(pFilename)
        TimedThread( _
            Sub()
                ExtractZipFile(
                    vOrgID:=pOrgID,
                    vPrivateDatabaseName:=pPrivateDatabaseName,
                    vKey:=pKey)
            End Sub)

        Return JsonDictionary.ToString("FollowUpKey", pKey)

    End Function

    Public Function GetResult(vOrgID As String, vKey As String)

        Dim pFilename = Common.ServerPath & "temp\EcommerceDataImporter\" & vOrgID & "\" & vKey & "\Result.txt"
        If IO.File.Exists(pFilename) Then
            Return FileText(pFilename)
        End If

        Return JsonDictionary.ToString("Result", "processing")

    End Function

    Public Function DownloadResult(vOrgID As String, vKey As String) As ActionResult

        Dim pFilename = Common.ServerPath & "temp\EcommerceDataImporter\" & vOrgID & "\" & vKey & "\Result.zip"

        Response.TransmitFile(pFilename)

    End Function

    Private Sub ExtractZipFile(vOrgID As String, vPrivateDatabaseName As String, vKey As String)

        Dim pFolder = Common.ServerPath & "temp\EcommerceDataImporter\" & vOrgID & "\" & vKey & "\"
        mFolder = pFolder
        Dim pFilename = pFolder & "data.zip"
        Dim pZip As New ProZip(pFilename)
        Dim pError As String
        Dim pJson As New JsonDictionary()

        pZip.Unzip(pFolder, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            pJson.Add("ErrorMessage", "An error occured while extracting " & pFilename & ". Error: " & pError)
            FileText(pFolder & "Result.txt") = pJson.ToString()
            Return
        End If

        ImportData(pFolder & "private.db", vPrivateDatabaseName)
        ZipResult(pFolder)

    End Sub

    Private Sub AddLog(vText As String)

        AppendToLog(vText, mFolder, "Log")

    End Sub

    Private Sub ZipResult(vFolder As String)
        
        Dim pFilename = vFolder & "Result.zip"
        Dim pZip As New ProZip(pFilename)
        pZip.Open(pFilename)

        pFilename = vFolder & "Result.private.db"
        If IO.File.Exists(pFilename) Then
            pZip.AddFile(pFilename)
        End If
        pZip.Close()

        Dim pJson As New JsonDictionary()
        pJson.Add("Result", "done")
        FileText(vFolder & "Result.txt") = pJson.ToString()


    End Sub

    Private Sub ImportData(vSourceSqliteFilename As String, vSqlServerDatabaseName As String)

        Dim pSqlite = New ProSqlite()
        pSqlite.Open(vSourceSqliteFilename)

        Dim pResultSqlite = New ProSqlite
        Dim pFilename = AppendBackslash(IO.Path.GetDirectoryName(vSourceSqliteFilename)) & "Result." & IO.Path.GetFileName(vSourceSqliteFilename)
        AddLog("Opening sqlite file, " & pFilename & ". ref: vbKACS93H22Z!")
        pResultSqlite.Open(pFilename)

        AddLog("Opening sql server connection. ref: vbHSRRFYNB5M!")
        Using pProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=vSqlServerDatabaseName,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

            ImportSalesOrder(pSqlite, pProData, pResultSqlite)
        End Using

    End Sub

    Private Sub ImportSalesOrder(vSourceSqlite As ProSqlite, vDestinationProData As ProData, vDestinationSqlite As ProSqlite)

        AddLog("ImportSalesOrder")
        If vSourceSqlite.TableExists("TransInvoice") = False Then
            AddLog("Table, TransInvoice, does not exist. ref: vb43FHZQACKPv!")
            Exit Sub
        End If
        AddLog("Table, TransInvoice, exists. ref: vb778JND4X87!")

        Dim pDtResult As New DataTable
        pDtResult.Columns.Add("ID")

        Dim pSql = "SELECT * FROM TransInvoice"
        Dim pDT = vSourceSqlite.Execute(pSql)
        AddLog("TransInvoice has " & pDT.Rows.Count & " rows. ref: vbBH5A2EWQ5E!")
        For Each pDR As DataRow In pDT.Rows
            Try
                If ImportRow(pDR, vDestinationProData, "TransInvoice") = False Then
                    Continue For
                End If

                vDestinationProData.BeginTransaction()

                pSql = "DELETE FROM TransInvoiceItems" & vbCrLf &
                    " WHERE InvoiceID = " & ReadyString(pDR("ID"))
                vDestinationProData.ExecuteSQL(pSql)

                pSql = "SELECT * FROM TransInvoiceItems" & vbCrLf &
                    " WHERE InvoiceID = " & ReadyString(pDR("ID"))
                Dim pDtItems = vSourceSqlite.Execute(pSql)
                For Each pDrItem In pDtItems.Rows
                    If ImportRow(pDrItem, vDestinationProData, "TransInvoiceItems") = False Then
                        vDestinationProData.RollbackTransaction(True, True)
                        Continue For
                    End If
                Next
                pDtResult.Rows.Add({CStr2(pDR("ID"))})
                vDestinationProData.CommitTransaction()
            Catch ex As Exception
                AddLog("Error on ImportSalesOrder loop. ref: vbXVE4U9P6Y2!" & vbCrLf &
                    ex.Message & vbCrLf &
                    "Inner exception: " & GetStackTrace(, ex))
            End Try
        Next pDR
        AddLog("Creating results table for TransInvoice. ref: vbUKJ7YLQD4M!")
        vDestinationSqlite.CreateTable("TransInvoice", pDtResult)
        AddLog("Done creating results table for TransInvoice. ref: vbRFAUPH5FEE!")

    End Sub

    Private Function ImportRow(vSourceDr As DataRow, vDestinationProData As ProData, vTableName As String) As Boolean

        Dim pIU = New InsertUpdate(vTableName)
        Dim pSql As String

        Try
            For Each pDC As DataColumn In vSourceDr.Table.Columns
                If ImportFromSqlite.ExcludeField(vTableName, pDC.ColumnName) Then
                    Continue For
                End If
                If IsDBNull(vSourceDr(pDC.ColumnName)) Then
                    pIU(pDC.ColumnName) = "NULL"
                Else
                    Select Case MajorDataType(pDC.DataType)
                        Case eMajorDataType.Numeric
                            pIU(pDC.ColumnName) = CDec2(vSourceDr(pDC.ColumnName))

                        Case Else
                            pIU(pDC.ColumnName) = ReadyString(vSourceDr(pDC.ColumnName))

                    End Select
                End If
            Next pDC
            pSql = pIU.GetUpdate("ID = " & ReadyString(vSourceDr("ID")))

            Dim pCount = vDestinationProData.ExecuteSQL(pSql, True)
            If pCount = 0 Then
                pSql = pIU.GetInsert()
                vDestinationProData.ExecuteSQL(pSql)
            End If
            Return True
        Catch ex As Exception
            AddLog("Error on ImportRow. ref: vbSEYNCMKW44!" & vbCrLf &
                   ex.Message & vbCrLf &
                   "Original SQL: " & pSql & vbCrLf &
                   "UPDATE SQL: " & pIU.GetUpdate("INSERT"))
            Return False
        End Try

    End Function
End Class
