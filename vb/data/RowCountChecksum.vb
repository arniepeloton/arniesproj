Public Class RowCountChecksum
    Implements IDisposable

    Public OrgID As String
    Public DeviceID As String
    Public ZipFilename As String

    Dim mProData As ProData
    Dim mBusiness As Business

    Public Sub New(vOrgID As String, vDeviceID As String, vZipFilename As String)

        OrgID = vOrgID
        DeviceID = vDeviceID
        ZipFilename = vZipFilename

    End Sub

    Public Function Process() As Boolean

        If ConnectToDatabase() = False Then
            Return False
        End If

        If IO.File.Exists(ZipFilename) = False Then
            AddError("Zipfile, " & ZipFilename & ", does not exist vbMRBD9MJBC3!")
            Return False
        End If

        If C1.C1Zip.C1ZipFile.IsZipFile(ZipFilename) = False Then
            AddError("Zipfile, " & ZipFilename & ", is not a valid zip file vbV3PXFH8YD6!")
            Return False
        End If

        CheckTables()

        Return True

    End Function

    Private Function CheckTables() As Boolean

        Dim pCsv As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pCount As Integer
        Dim pDiffTables As String = ""

        pCsv = New CsvReader(ZipFilename, "RowCount")
        Do
            pLine = pCsv.ReadLine
            If pLine Is Nothing Then Exit Do

            If pLine("Table").ToLower() <> "TotalRowCount".ToLower() _
                AndAlso pLine("Table").ToLower() <> "Signatures".ToLower() Then

                pCount = CheckTable(pCsv, pLine("Table"))
                AddCount(pLine("Table"), CDec2(pLine("Count")), pCount)
                If pCount <> pLine("Count") Then
                    If pDiffTables <> "" Then pDiffTables &= ", "
                    pDiffTables &= pLine("Table")
                End If
            End If
        Loop

        If pDiffTables <> "" Then
            mBusiness.TransDevAlerts.AddEntry("Device row count does not equal SQL Server database row count on: " & pDiffTables, "RowCountChecksum", OrgID, DeviceID)
        End If

    End Function

    Private Function CheckTable(vCsv As CsvReader, vTableName As String) As Integer

        Dim pTableName As String
        Dim pCount As Integer
        Dim pLine As Dictionary(Of String, String)
        Dim pCsv As CsvReader
        Dim pSql As String
        Dim pDR As DataRow

        pTableName = vTableName
        If IsTableExist(pTableName) = False Then
            pTableName = "Mobiz" & vTableName
            If IsTableExist(pTableName) = False Then
                Return -1
            End If
        End If

        pCsv = New CsvReader(ZipFilename, vTableName)
        Do
            pLine = pCsv.ReadLine
            If pLine Is Nothing Then Exit Do

            pSql = "SELECT ID FROM " & EscapeSqlName(pTableName) &
                " WHERE ID = " & ReadyString(pLine("ID"))
            pDR = mProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                pCount += 1
            End If
        Loop

        Return pCount

    End Function

    Private Function IsTableExist(vTableName As String) As Boolean
        ' cant use mProData.TableExist
        ' mProData.TableExist ignores synonyms

        Try
            mProData.Execute("SELECT TOP 0 * FROM " & EscapeSqlName(vTableName))
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Function ConnectToDatabase() As Boolean

        Dim pBusiness As Business
        Dim pDR As DataRow

        mProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=Common.ConnectionInfo.Database,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
        pBusiness = New Business(mProData)
        pDR = pBusiness.LibraryOrganization.ByID(OrgID)
        If pDR Is Nothing Then
            AddError("OrgID, " & OrgID & ", does not exist in the database vbCCQBT2KB3Y!")
            Return False
        End If

        If CStr2(pDR("Database")).Trim() = "" Then
            AddError("OrgID, " & OrgID & ", is not a subscriber vb3F2YAUEHPC!")
            Return False
        End If

        Try
            mProData.ExecuteSQL("USE " + EscapeSqlName(pDR("Database")))
        Catch ex As Exception
            AddError("An error occured while connecting to database, " & pDR("Database") & ", for OrgID, " & OrgID & " vb28U753PNMT!")
            Return False
        End Try

        mProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=pDR("Database"),
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
        mBusiness = New Business(mProData)

        Return True

    End Function

    Private Sub AddError(vMessage As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            pIU = New InsertUpdate("MobizTransRowCount")
            pIU("ID") = "newid()"
            pIU("OrgID") = ReadyString(OrgID)
            pIU("DeviceID") = ReadyString(DeviceID)
            pIU("DateLogged") = "GETDATE()"
            pIU("ErrorMessage") = ReadyString(vMessage)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Stop2()
        End Try

    End Sub

    Private Sub AddCount(vTableName As String, vDeviceRowCount As Integer, vServerRowCount As Integer)

        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            If vDeviceRowCount = 0 _
                AndAlso vServerRowCount = 0 Then

                Exit Sub
            End If

            pIU = New InsertUpdate("MobizTransRowCount")
            pIU("ID") = "newid()"
            pIU("OrgID") = ReadyString(OrgID)
            pIU("DeviceID") = ReadyString(DeviceID)
            pIU("DateLogged") = "GETDATE()"
            pIU("DeviceRowCount") = vDeviceRowCount
            pIU("ServerRowCount") = vServerRowCount
            pIU("TableName") = ReadyString(vTableName)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Stop2()
        End Try

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Try
                    mProData.Dispose()
                Catch ex As Exception
                    Stop2()
                End Try
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
