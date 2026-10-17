Public Class ArchiveController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Return View()

    End Function

    Public Function ArchiveNow() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pAllowedOrgs = New List(Of String)
        pAllowedOrgs.Add(ReadyGuid(modClient.AbcOrgID))
#If DEBUG Then
        pAllowedOrgs.Add(ReadyGuid(modClient.DemoCorpOrgID))
#End If
        If pAllowedOrgs.Contains(ReadyGuid(WebRequest.Instance.OrgID)) = False Then
            Return JsonDictionary.Serialize("Error", "Data Archiving feature is not enabled for your organization. ref: vb6N8VT4SN95!")
        End If

        Dim pAffectedRecords As Integer
        Dim pError As String
        Dim pProData = WebRequest.Instance.ProData
        Dim pNewDatabaseName = WebRequest.Instance.Database & "Archive"
        Dim pDatabases = pProData.GetDatabases(True, True)
        If pDatabases.Contains(pNewDatabaseName.ToLower()) = False Then
            If CreateArchiveDatabase(pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If
        End If
        Dim pDate = Common.WebRequest.Data("Date2")
        If IsDate(pDate) = False Then
            Return JsonDictionary.Serialize("Error", "Date2 must be a valid date.")
        End If
        pDate = Format(ParseDate(pDate), cGlobal.FormatDateTimeSql)

        Using pNewProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            .Server = Common.ConnectionInfo.Server,
            .Database = pNewDatabaseName,
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

            WebRequest.Instance.ProData.BeginTransaction()
            Dim pSql As String
            Dim pSqlWhere = " WHERE CONVERT(date, TransInvoice.LastUpdated) < " & ReadyString(pDate) & vbCrLf &
                " AND TransInvoice.TransType <> 'AccountsPayable'" & vbCrLf &
                " AND TransInvoice.TransType <> 'PurchaseOrder'" & vbCrLf &
                " AND TransInvoice.TransType <> 'ExpenseTicket'" & vbCrLf &
                " AND ISNULL(TransInvoice.PurchaseOrderNo, 0) = 0" & vbCrLf

            pSql = "SELECT TransInvoiceItemSalesReps.* " & vbCrLf &
                " FROM TransInvoiceItemSalesReps" & vbCrLf &
                " JOIN TransInvoiceItems ON TransInvoiceItems.ID = TransInvoiceItemSalesReps.InvoiceItemID " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItemSalesReps", pSql)

            pSql = "SELECT TransInvoiceItemsModifier.* " & vbCrLf &
                " FROM TransInvoiceItemsModifier" & vbCrLf &
                " JOIN TransInvoiceItems ON TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItemsModifier", pSql)

            pSql = "SELECT TransInvoiceItems.* " & vbCrLf &
                " FROM TransInvoiceItems " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItems", pSql,
                Sub()
                    WebRequest.Instance.ProData.AlterTrigger("TransInvoice_tru", "TransInvoice", False)
                    Dim pSql2 = "UPDATE TransInvoice" & vbCrLf &
                        " SET TransInvoice.InvoiceNumber = 0" & vbCrLf &
                        " WHERE TransInvoice.ID IN (" & vbCrLf &
                        " SELECT InvoiceID FROM (" & pSql & ") t" & vbCrLf &
                        ")"
                    WebRequest.Instance.ProData.ExecuteSQL(pSql2)
                    WebRequest.Instance.ProData.AlterTrigger("TransInvoice_tru", "TransInvoice", True)
                End Sub)

            pSql = "SELECT TransInvoiceItemSerials.* " & vbCrLf &
                " FROM TransInvoiceItemSerials " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItemSerials.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItemSerials", pSql)

            pSql = "SELECT TransInvoiceItemsPickingFowsts.* " & vbCrLf &
                " FROM TransInvoiceItemsPickingFowsts " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItemsPickingFowsts.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItemsPickingFowsts", pSql)

            pSql = "SELECT TransInvoiceItemsVoided.* " & vbCrLf &
                " FROM TransInvoiceItemsVoided " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItemsVoided.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItemsVoided", pSql)

            pSql = "SELECT TransInvoiceItemWarehouseSections.* " & vbCrLf &
                " FROM TransInvoiceItemWarehouseSections " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItemWarehouseSections.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItemWarehouseSections", pSql)

            pSql = "SELECT TransInvoiceItemWarehouseWithdrawal.* " & vbCrLf &
                " FROM TransInvoiceItemWarehouseWithdrawal " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceItemWarehouseWithdrawal.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceItemWarehouseWithdrawal", pSql)

            pSql = "SELECT TransInvoicePrints.* " & vbCrLf &
                " FROM TransInvoicePrints " & vbCrLf &
                " JOIN TransInvoice ON TransInvoicePrints.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoicePrints", pSql)

            pSql = "SELECT TransInvoiceSeniorCitizenPWDCustomer.* " & vbCrLf &
                " FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceSeniorCitizenPWDCustomer", pSql)

            pSql = "SELECT TransInvoiceAdjustmentReturnedChecks.* " & vbCrLf &
                " FROM TransInvoiceAdjustmentReturnedChecks" & vbCrLf &
                " JOIN TransInvoiceAdjustments ON TransInvoiceAdjustments.id = TransInvoiceAdjustmentReturnedChecks.InvoiceAdjustmentID" & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceAdjustments.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceAdjustmentReturnedChecks", pSql)

            pSql = "SELECT TransInvoiceAdjustments.* " & vbCrLf &
                " FROM TransInvoiceAdjustments " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceAdjustments.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceAdjustments", pSql)

            pSql = "SELECT TransInvoiceCharges.* " & vbCrLf &
                " FROM TransInvoiceCharges " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceCharges.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceCharges", pSql)

            pSql = "SELECT TransInvoiceDiscount.* " & vbCrLf &
                " FROM TransInvoiceDiscount " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceDiscount.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceDiscount", pSql)

            pSql = "SELECT TransInvoiceFiles.* " & vbCrLf &
                " FROM TransInvoiceFiles " & vbCrLf &
                " JOIN TransInvoice ON TransInvoiceFiles.InvoiceID = TransInvoice.ID" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoiceFiles", pSql)

            pAffectedRecords += CopyMemo(pNewProData, pSqlWhere)
            CopyReceiving(pNewProData, pSqlWhere)

            pSql = "SELECT TransInvoice.* " & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                pSqlWhere
            pAffectedRecords += CopyToArchiveDatabase(pNewProData, "TransInvoice", pSql)

            WebRequest.Instance.ProData.CommitTransaction() ' this is the proper line of code
            'WebRequest.Instance.ProData.RollbackTransaction() ' this is the debugging line of code

        End Using

        Dim pOut = New JsonDictionary()
        pOut.Add("Result", "ok")
        pOut.Add("AffectedRecords", pAffectedRecords)
        Return pOut.Serialize()

    End Function

    Private Function CopyMemo(vNewProData As ProData, vWhere As String) As Integer

        Dim pAffectedRecords As Integer
        Dim pSql = "SELECT TransMemoSalesReturn.* " & vbCrLf &
            " FROM TransMemoSalesReturn " & vbCrLf &
            " JOIN TransInvoice ON TransMemoSalesReturn.InvoiceID = TransInvoice.ID" & vbCrLf &
            vWhere
        Dim pDT = WebRequest.Instance.ProData.Execute(pSql)
        Dim pDtMemoIds = ExtractFieldValues(pDT, "MemoID")
        pAffectedRecords += CopyToArchiveDatabase(vNewProData, "TransMemoSalesReturn", pSql, , pDT)

        For Each pDrMemoId As DataRow In pDtMemoIds.Rows
            pSql = "SELECT TransMemo.*" & vbCrLf &
                " FROM TransMemo" & vbCrLf &
                " WHERE TransMemo.ID IN (" & pDrMemoId("MemoID") & ")"
            pAffectedRecords += CopyToArchiveDatabase(vNewProData, "TransMemo", pSql)
        Next

        Return pAffectedRecords

    End Function

    Private Sub CopyReceiving(vNewProData As ProData, vDate2 As String)

    End Sub

    Private Function CopyToArchiveDatabase(vDestinationProData As ProData, vTableName As String, vSql As String, Optional vAfterCopy As [Delegate] = Nothing,
        Optional vSourceDT As DataTable = Nothing) As Integer

        Dim pDT As DataTable
        If vSourceDT Is Nothing Then
            pDT = WebRequest.Instance.ProData.Execute(vSql)
        Else
            pDT = vSourceDT
        End If
        Dim pDestinationSchema = vDestinationProData.ColumnsInformation(vTableName)
        Using pBulkCopy As New SqlBulkCopy(vDestinationProData.Connection, SqlBulkCopyOptions.KeepIdentity + SqlBulkCopyOptions.KeepNulls, Nothing)
            'Using pBulkCopy As New SqlBulkCopy(pProData.Connection)
            For Each pSchemaRow As DataRow In pDestinationSchema.Rows
                If CBool2(pSchemaRow("iscomputed")) = False Then
                    pBulkCopy.ColumnMappings.Add(pSchemaRow("name"), pSchemaRow("name"))
                End If
            Next

            pBulkCopy.DestinationTableName = vTableName
            pBulkCopy.BulkCopyTimeout = 0
            pBulkCopy.WriteToServer(pDT)
        End Using

        If vAfterCopy IsNot Nothing Then
            vAfterCopy.DynamicInvoke()
        End If
        Dim pSql = "DELETE FROM " & vTableName & vbCrLf &
            " WHERE ID IN (" &
            "SELECT ID FROM (" & vSql & ") t" & vbCrLf &
            ")"
        Dim pAffectedRecords = WebRequest.Instance.ProData.ExecuteSQL(pSql)

        Return pAffectedRecords

    End Function

    Private Function CreateArchiveDatabase(ByRef oError) As Boolean

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        Dim pProData = WebRequest.Instance.ProData
        Dim pNewDatabaseName = WebRequest.Instance.Database & "Archive"
        Dim pDatabases = pProData.GetDatabases(True, True)
        If pDatabases.Contains(pNewDatabaseName.ToLower()) Then
            oError = ("Database, " & pNewDatabaseName & ", already exists. vbJX8PLNEQHQ!")
            Return False
        End If
        Dim pDatabaseFilename = pProData.GetFirstDataFilePath(WebRequest.Instance.Database)
        Dim pDatabaseFolder = AppendBackslash(IO.Path.GetDirectoryName(pDatabaseFilename))
        Dim pNewDatabaseFolder = Common.ServerPath & "temp\ArchiveDatabaseTemp\"
        Dim pNewDatabaseNameTemp = "temp_" & pNewDatabaseName
        If IO.Directory.Exists(pNewDatabaseFolder) Then
            Dim pNewFilename = pNewDatabaseFolder & pNewDatabaseNameTemp & ".mdf"
            If IO.File.Exists(pNewFilename) Then
                IO.File.Delete(pNewFilename)
            End If

            pNewFilename = pNewDatabaseFolder & pNewDatabaseNameTemp & ".ldf"
            If IO.File.Exists(pNewFilename) Then
                IO.File.Delete(pNewFilename)
            End If
        Else
            IO.Directory.CreateDirectory(pNewDatabaseFolder)
        End If
        ProPack.Utility.AddDirectoryUser(pNewDatabaseFolder)

        WebRequest.Instance.ProData.CreateDatabase(pNewDatabaseNameTemp, pNewDatabaseFolder)

        Using pSourceProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            .Server = Common.ConnectionInfo.Server,
            .Database = WebRequest.Instance.ProData.GetCurrentDatabase(),
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

            Using pNewProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                .Server = Common.ConnectionInfo.Server,
                .Database = pNewDatabaseNameTemp,
                .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                .Username = Common.ConnectionInfo.Username,
                .Password = Common.ConnectionInfo.Password})

                Dim pDatabase As New DBCompare.Database(pSourceProData)
                pDatabase.LoadFromSql(True, False, False, True)
                pDatabase.SetCompareStatusToUnique()
                DBCompare.Common.SqlDelimiter = vbCrLf & vbCrLf & "-- delimeter " & GenerateString(20) & vbCrLf & vbCrLf
                Dim pSql = pDatabase.GetScript()
                For Each pLine In pSql.Split({DBCompare.Common.SqlDelimiter}, StringSplitOptions.RemoveEmptyEntries)
                    Try
                        pNewProData.ExecuteSQL(pLine)
                    Catch ex As Exception
                        AppendToLog("vb9VZDHTFYDG! CreateArchiveDatabase " & ex.Message & vbCrLf & pLine)
                        Throw
                    End Try
                Next

                With New Synonyms()
                    .Prefix = "Mobiz"
                    .SourceDatabaseName = "Mobiz"
                    .SourceDatabase = pNewDatabaseNameTemp
                    .ProData = pNewProData
                    .Generate()
                End With

                Dim pDtFilenames = pNewProData.GetDatabaseFiles(pNewDatabaseNameTemp)
                Dim pFilenameToAttach As String
                pNewProData.DetachDatabase(pNewDatabaseNameTemp)
                For Each pDrFilename As DataRow In pDtFilenames.Rows
                    Dim pDestinationFilename = AppendBackslash(pDatabaseFolder) & IO.Path.GetFileName(pDrFilename("physical_name"))
                    If pFilenameToAttach Is Nothing Then
                        If CStr2(pDrFilename("type_desc")).ToUpper() = "ROWS" Then
                            pFilenameToAttach = pDestinationFilename
                        End If
                    End If
                    MoveFile(pDrFilename("physical_name"), pDestinationFilename)
                Next
                pNewProData.AttachDatabase(pNewDatabaseName, pFilenameToAttach)
            End Using
        End Using

        Return True
    End Function

End Class
