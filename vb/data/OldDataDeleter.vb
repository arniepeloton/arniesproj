Imports Aspose.Cells
Imports Stimulsoft.Base.Drawing.Win32

Public Class OldDataDeleter
    Dim mProData As ProData
    Dim mResult As Dictionary(Of String, Long) ' key is trimmed lowecase
    Dim mHeaderSql As String
    Dim mDtTables As DataTable
    Dim mFolder As String
    Dim mRecordCount As Long
    Public Delete As Boolean
    Public DateFrom As Date
    Public DateTo As Date
    Public Fresh As Boolean
    Public Shared DateStarted As Date?
    Public Shared LastLogDate As Date?
    Public Shared LastLog As String

    Public Shared Sub GetLatest(ByRef oPending As Boolean?, vFolder As String, ByRef oDtTables As DataTable, vDate As Date)

        Dim pExcelFilename As String
        pExcelFilename = IO.Path.Combine(vFolder, "Tables " & vDate.ToString("yyyy-MM") & " pending.xlsx")
        If IO.File.Exists(pExcelFilename) Then
            Dim pWB = New Workbook()
            pWB.Open(pExcelFilename)
            Dim pExcelHeaders = GetExcelHeaders(pWB.Worksheets(0))
            Dim pError As String
            oDtTables = ExcelToTable(pExcelFilename, pExcelHeaders.ToArray(), pError)
            oPending = True
            Return
        End If

        pExcelFilename = IO.Path.Combine(vFolder, "Tables " & vDate.ToString("yyyy-MM") & ".xlsx")
        If IO.File.Exists(pExcelFilename) Then
            Dim pWB = New Workbook()
            pWB.Open(pExcelFilename)
            Dim pExcelHeaders = GetExcelHeaders(pWB.Worksheets(0))
            Dim pError As String
            oDtTables = ExcelToTable(pExcelFilename, pExcelHeaders.ToArray(), pError)
            oPending = False
            Return
        End If

        oPending = Nothing

    End Sub

    Private Sub ExecuteInternal(vConnectionInfo As ConnectionInfo)

        DateFrom = DateFrom.Date
        DateTo = DateTo.Date
        DateStarted = Now

        Dim pPending As Boolean?
        GetLatest(oPending:=pPending,
            vFolder:=mFolder, oDtTables:=mDtTables, vDate:=DateFrom)
        If pPending = False Then
            AppendToLocalLog("Data from " & DateFrom.ToString(cGlobal.FormatDate) & " to " & DateTo.ToString(cGlobal.FormatDate) & " was already processed by previous sessions.")
            For Each pDrTable As DataRow In mDtTables.Rows
                mRecordCount += CInt2(pDrTable("DeletableCount"))
            Next
            Return
        End If

        Try
            If Delete _
                AndAlso Now.Date <> #2024/12/29# Then

                AppendToLocalLog("OldDataDeleter is not allowed to execute today. vbWVYK5EJ8RD!")
                Throw New ApplicationException("OldDataDeleter is not allowed to execute today. vbWVYK5EJ8RD!")
            End If

            mProData = ProData.CreateFromConnectionInfo(vConnectionInfo)
            If String.IsNullOrWhiteSpace(mProData.ConnectError) = False Then
                AppendToLocalLog(mProData.ConnectError)
                Throw New ApplicationException(mProData.ConnectError)
            End If

            If Delete Then
                Dim pSql = "SELECT @@servicename"
                Dim pInstanceName = CStr2(mProData.ExecuteScalar(pSql))
                If pInstanceName.Trim().ToLower() = "MSSQLSERVER".ToLower() Then
                    Throw New ApplicationException("OldDataDeleter cannot be executed on the default SQL Server instance. vb8F9B78DU47!")
                End If
                AppendToLocalLog("Deleting data from " & DateFrom.ToString(cGlobal.FormatDate) & " to " & DateTo.ToString(cGlobal.FormatDate) & ".")
            Else
                AppendToLocalLog("Counting deletable data from " & DateFrom.ToString(cGlobal.FormatDate) & " to " & DateTo.ToString(cGlobal.FormatDate) & ".")
            End If

            mProData.CommandTimeout = 48 * 60 * 60

            Dim pDataDeleter = New DataDeleter()
            pDataDeleter.Database = vConnectionInfo.Database
            pDataDeleter.ProData = mProData
            If mDtTables Is Nothing Then
                AppendToLocalLog("populating tables")
                mDtTables = pDataDeleter.PopTables()
                mDtTables.Columns.Add("DeletableCount", GetType(Long))
                mDtTables.Columns.Add("TotalAmount", GetType(Decimal))
                mDtTables.Columns.Add("MinDate", GetType(DateTime))
                mDtTables.Columns.Add("MaxDate", GetType(DateTime))
                mDtTables.Columns.Remove("include")
                mDtTables.Columns.Remove("deletecondition")
                mDtTables.Columns.Remove("conditionalcolumn")
                mDtTables.Columns.Remove("conditionalrowcount")
                AppendToLocalLog("savings tables to excel")
                TableToExcelFile(mDtTables, "", "", mFolder & "Tables " & DateFrom.ToString("yyyy-MM") & " pending.xlsx")
            End If
            AppendToLocalLog("counting")
            CountRecords()
            MoveFile(mFolder & "Tables " & DateFrom.ToString("yyyy-MM") & " pending.xlsx", mFolder & "Tables " & DateFrom.ToString("yyyy-MM") & ".xlsx")
            AppendToLocalLog("done - Tables " & DateFrom.ToString("yyyy-MM") & ".xlsx")
        Catch ex As ProSqlException
            Dim pMsg = ex.Message & vbCrLf &
                ex.Sql & vbCrLf &
                GetStackTrace(, ex)
            Dim pFilename = mFolder & "Error.txt"
            CreateFolder(pFilename)
            FileText(pFilename) = pMsg
            AppendToLocalLog("done. Error: " & pMsg)
        Catch ex As Exception
            Dim pMsg = ex.Message & vbCrLf &
                GetStackTrace(, ex)
            Dim pFilename = mFolder & "Error.txt"
            CreateFolder(pFilename)
            FileText(pFilename) = pMsg
            AppendToLocalLog("done. Error: " & pMsg)
        End Try

    End Sub

    Public Function Execute(vConnectionInfo As ConnectionInfo)

        mFolder = Common.ServerPath & "temp\OldDataDeleter\" & DateFrom.ToString(cGlobal.FormatDateSql) & " to " & DateTo.ToString(cGlobal.FormatDateSql) & "-" & CreateValidFilename(vConnectionInfo.Database) & "\"
        If IO.Directory.Exists(mFolder) Then
            IO.Directory.CreateDirectory(mFolder)
        End If

        Try
            Dim pDateTo = DateTo
            Do
                Dim pDateFrom = New DateTime(pDateTo.Year, pDateTo.Month, 1)
                If pDateFrom <= DateFrom Then
                    pDateFrom = DateFrom
                End If
                Dim pOldDataDeleter = New OldDataDeleter()
                pOldDataDeleter.Delete = Delete
                pOldDataDeleter.DateFrom = pDateFrom
                pOldDataDeleter.DateTo = pDateTo
                pOldDataDeleter.mFolder = mFolder
                pOldDataDeleter.ExecuteInternal(vConnectionInfo)

                If pOldDataDeleter.RecordCount = 0 Then
                    Exit Do
                End If
                If pDateFrom = DateFrom Then
                    Exit Do
                Else
                    pDateTo = pDateFrom.AddDays(-1)
                End If
            Loop
            AppendToLocalLog("done")
        Catch ex As Exception
            Dim pMsg = ex.Message & vbCrLf &
                GetStackTrace(, ex)
            AppendToLocalLog("error - " & pMsg)
        End Try

    End Function

    Function CreateFinderFields(ParamArray vParams() As String) As FinderFields

        Dim pOut = New FinderFields()
        Dim pIndex As Integer
        Do
            If pIndex = vParams.Length Then
                Return pOut
            End If
            Dim pItem As New FinderField()
            pItem.FieldName = vParams(pIndex)
            pIndex += 1
            pItem.Expression = vParams(pIndex)
            pOut.Add(pItem)
            pIndex += 1
        Loop

        Return pOut

    End Function

    Function CountRecords()

        mResult = New Dictionary(Of String, Long)

        mHeaderSql = " DECLARE @TransDateFrom DATETIME = " & ReadyString(Format(DateFrom, cGlobal.FormatDateSql)) & ";" & vbCrLf &
            " DECLARE @TransDateTo DATETIME = " & ReadyString(Format(DateTo, cGlobal.FormatDateSql)) & ";" & vbCrLf

        'mProData.BeginTransaction()

        SelectOrDelete("TransReceivingPaymentAppliedCredits",
            "TransReceivingPaymentAppliedCredits.DestinationPaymentID IN" & vbCrLf &
            " 	(SELECT TransReceivingPayment.ID" & vbCrLf &
            "     	FROM TransReceivingPayment" & vbCrLf &
            "     	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            "     )" & vbCrLf &
            " AND TransReceivingPaymentAppliedCredits.SourcePaymentID IN" & vbCrLf &
            " 	(SELECT TransReceivingPayment.ID" & vbCrLf &
            "     	FROM TransReceivingPayment" & vbCrLf &
            "     	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            "     )", CreateFinderFields("TotalAmount", "SUM(Amount)"))

        SelectOrDelete("TransReceivingPaymentCheckAmount",
            " TransReceivingPaymentCheckAmount.PaymentID IN (" & vbCrLf &
            " 	SELECT TransReceivingPayment.ID" & vbCrLf &
            "     FROM TransReceivingPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )", CreateFinderFields("TotalAmount", "SUM(AmountPaid)"))

        SelectOrDelete("TransReceivingPaymentChecks",
            " TransReceivingPaymentChecks.PaymentID IN (" & vbCrLf &
            " 	SELECT TransReceivingPayment.ID" & vbCrLf &
            "     FROM TransReceivingPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransReceivingPaymentCreditCard",
            " TransReceivingPaymentCreditCard.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransReceivingPayment.ID" & vbCrLf &
            "     FROM TransReceivingPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransReceivingPaymentDebitCard",
            " TransReceivingPaymentDebitCard.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransReceivingPayment.ID" & vbCrLf &
            "     FROM TransReceivingPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransReceivingPaymentOther",
            " TransReceivingPaymentOther.PaymentID IN (" & vbCrLf &
            " 	SELECT TransReceivingPayment.ID" & vbCrLf &
            "     FROM TransReceivingPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransReceivingPaymentReceivings",
            " TransReceivingPaymentReceivings.PaymentID IN (" & vbCrLf &
            " 	SELECT TransReceivingPayment.ID" & vbCrLf &
            "     FROM TransReceivingPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransReceivingPayment",
            "CONVERT(date, TransReceivingPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo")

        SelectOrDelete("TransCustomerPaymentAppliedCredits",
            "TransCustomerPaymentAppliedCredits.DestinationPaymentID IN" & vbCrLf &
            " 	(SELECT TransCustomerPayment.ID" & vbCrLf &
            "     	FROM TransCustomerPayment" & vbCrLf &
            "     	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            "     )" & vbCrLf &
            " AND TransCustomerPaymentAppliedCredits.SourcePaymentID IN" & vbCrLf &
            " 	(SELECT TransCustomerPayment.ID" & vbCrLf &
            "     	FROM TransCustomerPayment" & vbCrLf &
            "     	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            "     )")

        SelectOrDelete("TransCustomerPaymentUnappliedCredits",
            " TransCustomerPaymentUnappliedCredits.DestinationPaymentID IN" & vbCrLf &
            " 	(SELECT TransCustomerPayment.ID" & vbCrLf &
            "     	FROM TransCustomerPayment" & vbCrLf &
            "     	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            "     )" & vbCrLf &
            " AND TransCustomerPaymentUnappliedCredits.SourcePaymentID IN" & vbCrLf &
            " 	(SELECT TransCustomerPayment.ID" & vbCrLf &
            "     	FROM TransCustomerPayment" & vbCrLf &
            "     	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            "     )")

        SelectOrDelete("TransCustomerPaymentInvoices",
            " TransCustomerPaymentInvoices.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransCustomerPayment.ID" & vbCrLf &
            "     FROM TransCustomerPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )", CreateFinderFields("TotalAmount", "SUM(AmountPaid)"))

        SelectOrDelete("TransCustomerPaymentOther",
            " TransCustomerPaymentOther.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransCustomerPayment.ID" & vbCrLf &
            "     FROM TransCustomerPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )", CreateFinderFields("TotalAmount", "SUM(Amount)"))

        SelectOrDelete("TransCustomerPaymentCheckAmount",
            " TransCustomerPaymentCheckAmount.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransCustomerPayment.ID" & vbCrLf &
            "     FROM TransCustomerPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransCustomerPaymentChecks",
            " TransCustomerPaymentChecks.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransCustomerPayment.ID" & vbCrLf &
            "     FROM TransCustomerPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransCustomerPaymentCreditCard",
            " TransCustomerPaymentCreditCard.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransCustomerPayment.ID" & vbCrLf &
            "     FROM TransCustomerPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )", CreateFinderFields("TotalAmount", "SUM(CreditCardAmount)", "MinDate", "MIN(CreditCardExpirationDate)", "MaxDate", "MAX(CreditCardExpirationDate)"))

        SelectOrDelete("TransCustomerPaymentDebitCard",
            " TransCustomerPaymentDebitCard.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransCustomerPayment.ID" & vbCrLf &
            "     FROM TransCustomerPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransCustomerPaymentSRV",
            " TransCustomerPaymentSRV.CustomerPaymentID IN (" & vbCrLf &
            " 	SELECT TransCustomerPayment.ID" & vbCrLf &
            "     FROM TransCustomerPayment" & vbCrLf &
            " 	WHERE CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransCustomerPayment",
            " CONVERT(date, TransCustomerPayment.PaymentDate) BETWEEN @TransDateFrom AND @TransDateTo")

        SelectOrDelete("TransInvoiceItemsModifier",
            " InvoiceItemID IN (" & vbCrLf &
            " 	SELECT TransInvoiceItems.ID" & vbCrLf &
            "     FROM TransInvoiceItems" & vbCrLf &
            "     JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceSeniorCitizenPWDCustomer",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceItemSalesReps",
            " TransInvoiceItemSalesReps.InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceItemSerials",
            " TransInvoiceItemSerials.InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceItems",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )", CreateFinderFields("TotalAmount", "SUM(SubTotal)"))

        SelectOrDelete("TransInvoiceItemsVoided",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceItemWarehouseSections",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceItemWarehouseWithdrawal",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoicePrints",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceItemsPickingFowsts",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceItemsRebates",
            " ModifierGroupID IN (" & vbCrLf &
            " 	SELECT ModifierGroupID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom AND @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceAdjustmentReturnedChecks",
            " TransInvoiceAdjustmentReturnedChecks.InvoiceAdjustmentID IN (" & vbCrLf &
            "   SELECT TransInvoiceAdjustments.ID" & vbCrLf &
            "   FROM TransInvoiceAdjustments" & vbCrLf &
            "   WHERE InvoiceID IN (" & vbCrLf &
            " 	    SELECT ID" & vbCrLf &
            "       FROM TransInvoice" & vbCrLf &
            " 	    WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " 	)" & vbCrLf &
            ")")

        SelectOrDelete("TransInvoiceAdjustments",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceDiscount",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoiceFiles",
            " InvoiceID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInvoice" & vbCrLf &
            " 	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInvoice",
            " CONVERT(date, TransInvoice.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(TotalAmount)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("TransInventoryLedgerAdjustment",
            " TransInventoryLedgerID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInventoryLedger" & vbCrLf &
            " 	WHERE CONVERT(date, TransInventoryLedger.TransactionDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransInventoryLedgerOldBalances",
            "CONVERT(date, TransInventoryLedgerOldBalances.BalanceDate) BETWEEN @TransDateFrom And @TransDateTo")

        SelectOrDelete("TransInventoryLedger",
            " CONVERT(date, TransactionDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Value)", "MinDate", "MIN(TransactionDate)", "MaxDate", "MAX(TransactionDate)"))

        SelectOrDelete("CacheInventoryMovement",
            " CONVERT(date, TransactionDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(LandedCost)", "MinDate", "MIN(TransactionDate)", "MaxDate", "MAX(TransactionDate)"))

        SelectOrDelete("CacheInvoices",
            " CONVERT(date, InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(GrossInvoiceAmount)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("TransImportBenchmark2",
            " CONVERT(date, LastUpdated) BETWEEN @TransDateFrom And @TransDateTo")

        SelectOrDelete("TransInventoryProductCountStock",
            " TransInventoryProductCountStock.InventoryProductCountID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "     FROM TransInventoryProductCount" & vbCrLf &
            "     WHERE CONVERT(date, TransInventoryProductCount.InventoryCountDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )",
            CreateFinderFields("TotalAmount", "SUM(Variance)", "MinDate", "MIN(TurnOverDate)", "MaxDate", "MAX(TurnOverDate)"))

        SelectOrDelete("TransInventoryProductCount",
            "CONVERT(date, InventoryCountDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(InventoryCountDate)", "MaxDate", "MAX(InventoryCountDate)"))

        SelectOrDelete("CacheInvoiceModifiers",
            "CONVERT(date, CacheInvoiceModifiers.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(ModifierAmount)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("TransOfftake",
            "CONVERT(date, TransOfftake.OffTakeDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(OffTake)", "MinDate", "MIN(OffTakeDate)", "MaxDate", "MAX(OffTakeDate)"))

        SelectOrDelete("TransSalesOrderStatus",
            "CONVERT(date, TransSalesOrderStatus.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransReceivingStock",
            " TransReceivingStock.ReceivingID IN (" & vbCrLf &
            " 	SELECT TransReceiving.ID" & vbCrLf &
            "     FROM TransReceiving" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceiving.DateReceived) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )",
            CreateFinderFields("TotalAmount", "SUM(LineLandedCost)"))

        SelectOrDelete("TransReceivingInvoiceRefImages",
            " TransReceivingInvoiceRefImages.ReceivingID IN (" & vbCrLf &
            " 	SELECT TransReceiving.ID" & vbCrLf &
            "     FROM TransReceiving" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceiving.DateReceived) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransReceivingStockSerials",
            " TransReceivingStockSerials.ReceivingID IN (" & vbCrLf &
            " 	SELECT TransReceiving.ID" & vbCrLf &
            "     FROM TransReceiving" & vbCrLf &
            " 	WHERE CONVERT(date, TransReceiving.DateReceived) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " )")

        SelectOrDelete("TransReceiving",
            "CONVERT(date, TransReceiving.DateReceived) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Total)", "MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransDeviceLogs",
            "CONVERT(date, TransDeviceLogs.ServerTime) BETWEEN @TransDateFrom And @TransDateTo")

        SelectOrDelete("TransDeviceAvailability",
            "CONVERT(date, TransDeviceAvailability.TransactionDate) BETWEEN @TransDateFrom And @TransDateTo")

        SelectOrDelete("TransZReadCache",
            "CONVERT(date, TransZReadCache.SalesDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(CurGrossSales)", "MinDate", "MIN(SalesDate)", "MaxDate", "MAX(SalesDate)"))

        SelectOrDelete("TransActionLogs",
            "CONVERT(date, TransActionLogs.ActionDate) BETWEEN @TransDateFrom And @TransDateTo")

        SelectOrDelete("TransCashHandling",
            " CONVERT(date, TransCashHandling.TransactionDateAndTimeFrom) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " And CONVERT(date, TransCashHandling.TransactionDateAndTimeTo) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(TotalAmount)", "MinDate", "MIN(DepositDate)", "MaxDate", "MAX(DepositDate)"))

        SelectOrDelete("TransCashHandlingCashCount",
            "CONVERT(date, TransCashHandlingCashCount.CashCountDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(TotalAmount)", "MinDate", "MIN(CashCountDate)", "MaxDate", "MAX(CashCountDate)"))

        SelectOrDelete("TransPimSalesCaching",
            " CONVERT(date, TransPimSalesCaching.DateIntervalStart) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " And CONVERT(date, TransPimSalesCaching.DateIntervalEnd) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(SalesQty)", "MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransSalesInvoiceAndRebatesCache",
            "CONVERT(date, TransSalesInvoiceAndRebatesCache.TransactionDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(SubTotal)", "MinDate", "MIN(TransactionDate)", "MaxDate", "MAX(TransactionDate)"))

        SelectOrDelete("TransImports",
            "CONVERT(date, TransImports.DateReceived) BETWEEN @TransDateFrom And @TransDateTo")

        SelectOrDelete("CacheInvoicesDaily",
            "CONVERT(date, CacheInvoicesDaily.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(GrossInvoiceAmount)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("CacheInvoiceFinishedProducts",
            "CONVERT(date, CacheInvoiceFinishedProducts.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(SalesValue)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("TransMemoSalesReturn",
            "CONVERT(date, TransMemoSalesReturn.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(TotalAmount)", "MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransMemo",
            "CONVERT(date, TransMemo.MemoDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(MemoDate)", "MaxDate", "MAX(MemoDate)"))

        SelectOrDelete("TransSalesReturnLedger",
            "CONVERT(date, TransSalesReturnLedger.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("CacheInvoicesHourly",
            "CONVERT(date, CacheInvoicesHourly.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(GrossInvoiceAmount)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("TenantMonitoringLogs",
            "CONVERT(date, TenantMonitoringLogs.SalesDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(SalesDate)", "MaxDate", "MAX(SalesDate)"))

        SelectOrDelete("TransSalesInvoiceAndRebatesHeader",
            "CONVERT(date, TransSalesInvoiceAndRebatesHeader.TransactionMonthYear) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(OriginalValue)", "MinDate", "MIN(TransactionMonthYear)", "MaxDate", "MAX(TransactionMonthYear)"))

        SelectOrDelete("TransInvoiceDeliveryDetails",
            "CONVERT(date, TransInvoiceDeliveryDetails.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("CacheReceivings",
            "CONVERT(date, CacheReceivings.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(GrossInvoiceAmount)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("AuditTrailAndDashboardLog",
            " CONVERT(date, AuditTrailAndDashboardLog.DateFrom) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            " And CONVERT(date, AuditTrailAndDashboardLog.DateTo) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(POSTransGrossSales)", "MinDate", "MIN(GeneratedDateTime)", "MaxDate", "MAX(GeneratedDateTime)"))

        SelectOrDelete("TransProfitability",
            "CONVERT(date, TransProfitability.TransDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(SellingPrice)", "MinDate", "MIN(TransDate)", "MaxDate", "MAX(TransDate)"))

        SelectOrDelete("CacheFc",
            "CONVERT(date, CacheFc.SalesOrderDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Total)", "MinDate", "MIN(SalesOrderDate)", "MaxDate", "MAX(SalesOrderDate)"))

        SelectOrDelete("CacheDailyCustomerInvoice",
            "CONVERT(date, CacheDailyCustomerInvoice.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("CacheInvoiceFlavors",
            "CONVERT(date, CacheInvoiceFlavors.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(SalesValue)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("CacheInvoiceProductSizes",
            "CONVERT(date, CacheInvoiceProductSizes.InvoiceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(SalesValue)", "MinDate", "MIN(InvoiceDate)", "MaxDate", "MAX(InvoiceDate)"))

        SelectOrDelete("CacheMonthlyCustomerInvoice",
            "CacheMonthlyCustomerInvoice.InvoiceYear * 12 + CacheMonthlyCustomerInvoice.InvoiceMonth" & vbCrLf &
            " BETWEEN YEAR(@TransDateFrom) * 12 + MONTH(@TransDateFrom)" & vbCrLf &
            " And YEAR(@TransDateTo) * 12 + MONTH(@TransDateTo)",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(cast(cast(InvoiceYear*10000 + InvoiceMonth*100 + 1 as varchar(255)) as date))", "MaxDate", "MAX(cast(cast(InvoiceYear*10000 + InvoiceMonth*100 + 1 as varchar(255)) as date))"))

        SelectOrDelete("TransBackOrderProducts",
            " BackOrderID IN (" & vbCrLf &
            " 	SELECT ID" & vbCrLf &
            "   FROM TransBackOrders" & vbCrLf &
            " 	WHERE TransBackOrders.OriginalSalesOrderID IN (" & vbCrLf &
            "       SELECT ID" & vbCrLf &
            "       FROM TransInvoice" & vbCrLf &
            "       WHERE CONVERT(date, TransInvoice.SalesOrderDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            "   )" & vbCrLf &
            ")",
            CreateFinderFields("TotalAmount", "SUM(Qty * UnitPrice)"))

        SelectOrDelete("TransBackOrders",
            "TransBackOrders.OriginalSalesOrderID IN (" & vbCrLf &
            "   SELECT ID" & vbCrLf &
            "   FROM TransInvoice" & vbCrLf &
            "   WHERE CONVERT(date, TransInvoice.SalesOrderDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")")

        SelectOrDelete("TransBillingStatementsInvoices",
            "TransBillingStatementsInvoices.StatementID IN (" & vbCrLf &
            "   SELECT ID" & vbCrLf &
            "   FROM TransBillingStatements" & vbCrLf &
            "   WHERE CONVERT(date, TransBillingStatements.StatementDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")",
            CreateFinderFields("TotalAmount", "SUM(Balance)", "MinDate", "MIN(DueDate)", "MaxDate", "MAX(DueDate)"))

        SelectOrDelete("TransBillingStatements",
            "CONVERT(date, TransBillingStatements.StatementDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(TotalAmount)", "MinDate", "MIN(StatementDate)", "MaxDate", "MAX(StatementDate)"))

        SelectOrDelete("TransBomMakerContents",
            " TransBomMakerContents.ParentID IN (" & vbCrLf &
            " 	SELECT TransBomMakerProducts.ID" & vbCrLf &
            "   FROM TransBomMakerProducts" & vbCrLf &
            " 	WHERE TransBomMakerProducts.ParentID IN (" & vbCrLf &
            "       SELECT TransBomMaker.ID" & vbCrLf &
            "       FROM TransBomMaker" & vbCrLf &
            "       WHERE CONVERT(date, TransBomMaker.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            "   )" & vbCrLf &
            ")",
            CreateFinderFields("TotalAmount", "SUM(QtyInBaseUnit)"))

        SelectOrDelete("TransBomMakerProducts",
            "TransBomMakerProducts.ParentID IN (" & vbCrLf &
            "   SELECT TransBomMaker.ID" & vbCrLf &
            "   FROM TransBomMaker" & vbCrLf &
            "   WHERE CONVERT(date, TransBomMaker.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")",
            CreateFinderFields("TotalAmount", "SUM(Quantity)"))

        SelectOrDelete("TransBomMaker",
            "CONVERT(date, TransBomMaker.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransCustomerTransactionSummaryCache",
            "CONVERT(date, TransCustomerTransactionSummaryCache.SalesDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(NetSales)", "MinDate", "MIN(SalesDate)", "MaxDate", "MAX(SalesDate)"))

        SelectOrDelete("TransDailyBeginningBalance",
            "CONVERT(date, TransDailyBeginningBalance.BeginningBalanceDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Sales)", "MinDate", "MIN(BeginningBalanceDate)", "MaxDate", "MAX(BeginningBalanceDate)"))

        SelectOrDelete("TransDateImported",
            "CONVERT(date, TransDateImported.RowLastUpdated) BETWEEN @TransDateFrom And @TransDateTo")

        SelectOrDelete("TransDeliveryLoggerInvoices",
            "TransDeliveryLoggerInvoices.DeliveryLoggerID IN (" & vbCrLf &
            "   SELECT TransDeliveryLogger.ID" & vbCrLf &
            "   FROM TransDeliveryLogger" & vbCrLf &
            "   WHERE CONVERT(date, TransDeliveryLogger.DeliveryEncodingDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")",
            CreateFinderFields("MinDate", "MIN(DeliveryEncodingDate)", "MaxDate", "MAX(DeliveryEncodingDate)"))

        SelectOrDelete("TransDeliveryLogger",
            "CONVERT(date, TransDeliveryLogger.DeliveryEncodingDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(DeliveryEncodingDate)", "MaxDate", "MAX(DeliveryEncodingDate)"))

        SelectOrDelete("TransDeliveryLogs",
            "CONVERT(date, TransDeliveryLogs.DeliveryEncodingDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(DeliveryEncodingDate)", "MaxDate", "MAX(DeliveryEncodingDate)"))

        SelectOrDelete("TransRentableAssetCustomer",
            "TransRentableAssetCustomer.TransRentableAssetID IN (" & vbCrLf &
            "   SELECT TransRentableAssets.ID" & vbCrLf &
            "   FROM TransRentableAssets" & vbCrLf &
            "   WHERE CONVERT(date, TransRentableAssets.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")")

        SelectOrDelete("TransRentableAssetCustomerRequest",
            "TransRentableAssetCustomerRequest.TransRentableAssetID IN (" & vbCrLf &
            "   SELECT TransRentableAssets.ID" & vbCrLf &
            "   FROM TransRentableAssets" & vbCrLf &
            "   WHERE CONVERT(date, TransRentableAssets.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")",
            CreateFinderFields("MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransRentableAssetOtherCharges",
            "TransRentableAssetOtherCharges.TransRentableAssetID IN (" & vbCrLf &
            "   SELECT TransRentableAssets.ID" & vbCrLf &
            "   FROM TransRentableAssets" & vbCrLf &
            "   WHERE CONVERT(date, TransRentableAssets.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")",
            CreateFinderFields("TotalAmount", "SUM(Qty)"))

        SelectOrDelete("TransRentableAssets",
            "CONVERT(date, TransRentableAssets.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransExpensesLogDetailsHistory",
            "TransExpensesLogDetailsHistory.LogExpenseID IN (" & vbCrLf &
            "   SELECT TransExpensesLogDetails.ID" & vbCrLf &
            "   FROM TransExpensesLogDetails" & vbCrLf &
            "   WHERE CONVERT(date, TransExpensesLogDetails.ExpenseDate) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransExpensesLogDetails",
            "CONVERT(date, TransExpensesLogDetails.ExpenseDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(ExpenseDate)", "MaxDate", "MAX(ExpenseDate)"))

        SelectOrDelete("TransExpensesReceiveFundsHistory",
            "TransExpensesReceiveFundsHistory.ReceiveFundID IN (" & vbCrLf &
            "   SELECT TransExpensesReceiveFunds.ID" & vbCrLf &
            "   FROM TransExpensesReceiveFunds" & vbCrLf &
            "   WHERE CONVERT(date, TransExpensesReceiveFunds.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransExpensesReceiveFunds",
            "CONVERT(date, TransExpensesReceiveFunds.ReceiveDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(ReceiveDate)", "MaxDate", "MAX(ReceiveDate)"))

        SelectOrDelete("TransCashAdvanceEmployeesPayment",
            "CONVERT(date, TransCashAdvanceEmployeesPayment.DatePaid) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(AmountPaid)", "MinDate", "MIN(DatePaid)", "MaxDate", "MAX(DatePaid)"))

        SelectOrDelete("TransItemRequisitionBackOrder",
            "TransItemRequisitionBackOrder.ItemRequisitionID IN (" & vbCrLf &
            "   SELECT TransItemRequisitions.ID" & vbCrLf &
            "   FROM TransItemRequisitions" & vbCrLf &
            "   WHERE CONVERT(date, TransItemRequisitions.DateRequested) BETWEEN @TransDateFrom And @TransDateTo" & vbCrLf &
            ")")

        SelectOrDelete("TransItemRequisitions",
            "CONVERT(date, TransItemRequisitions.DateRequested) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(RequestedQtyTotal)", "MinDate", "MIN(DateRequested)", "MaxDate", "MAX(DateRequested)"))

        SelectOrDelete("TransReceiveFundsCache",
            "CONVERT(date, TransReceiveFundsCache.LastUpdated) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(Amount)", "MinDate", "MIN(LastUpdated)", "MaxDate", "MAX(LastUpdated)"))

        SelectOrDelete("TransVendorPriceHistory",
            "CONVERT(date, TransVendorPriceHistory.TransDate) BETWEEN @TransDateFrom And @TransDateTo",
            CreateFinderFields("TotalAmount", "SUM(CurrentPrice)", "MinDate", "MIN(TransDate)", "MaxDate", "MAX(TransDate)"))

        'mProData.RollbackTransaction()

        ' exclude these tables
        ' TransCustomerPaymentType - no records
    End Function

    Sub SelectOrDelete(vTableName As String, vWhere As String, Optional vAdditionalFields As FinderFields = Nothing)

        If mResult.ContainsKey(vTableName.Trim().ToLower()) Then
            Throw New ApplicationException("SelectOrDelete: Table, " + vTableName + ", already exist. vb67VFNP89WK!")
        End If

        Dim pOut As Long
        Dim pDR As DataRow
        Dim pSql As String

        Dim pRows = mDtTables.Select("Name = " & ReadyString(vTableName))
        If pRows.Length > 0 Then
            If IsNumeric(pRows(0)("DeletableCount")) Then
                AppendToLocalLog("SelectOrDelete: skipping " & vTableName)
                Exit Sub
            End If
        End If

        If mProData.TableExist(vTableName) = False Then
            AppendToLocalLog("SelectOrDelete: table, " & vTableName & ", does not exist")
            Exit Sub
        End If

        Debug.WriteLine("SelectOrDelete: " & vTableName)
        AppendToLocalLog("SelectOrDelete: " & vTableName & " from " & DateFrom.ToString(cGlobal.FormatDate) & " to " & DateTo.ToString(cGlobal.FormatDate))
        If Delete Then
            pSql = "WITH CTE AS (" & vbCrLf &
                " SELECT TOP 10000 * FROM " & EscapeSqlName(vTableName) & vbCrLf &
                " WHERE " & vWhere & vbCrLf &
                ")" & vbCrLf &
                " -- DELETE FROM CTE" & vbCrLf &
                " SELECT ID FROM CTE"
        Else
            Dim pAdditionalFields As String = ""
            If vAdditionalFields IsNot Nothing Then
                For Each pField In vAdditionalFields
                    pAdditionalFields &= "," & pField.Value.Expression & " AS " & pField.Key & vbCrLf
                Next
            End If

            pSql = "SELECT COUNT(*) AS Count" & vbCrLf &
                pAdditionalFields &
                " FROM " & EscapeSqlName(vTableName) & vbCrLf &
                " WHERE " & vWhere
        End If
        pSql = mHeaderSql & pSql
        If Delete Then
            Do
                Dim pSW = Stopwatch.StartNew()
                Dim pDtIds = mProData.Execute(pSql)
                AppendToLocalLog("IDs for deletion from " & vTableName & " : " & pDtIds.Rows.Count.ToString(cGlobal.FormatInteger) &
                    ". Duration: " & pSW.ElapsedMilliseconds().ToString(cGlobal.FormatInteger) & " ms.")
                Dim pRecordsDeleted As Integer
                If pDtIds.Rows.Count > 0 Then
                    Dim pDtJoined = ExtractFieldValues(pDtIds, "ID", 1000)
                    For Each pDrJoined As DataRow In pDtJoined.Rows
                        Dim pSql2 = "DELETE FROM " & EscapeSqlName(vTableName) & vbCrLf &
                            " WHERE ID  IN (" & pDrJoined("ID") & ")"
                        pSW.Restart()
                        pRecordsDeleted = mProData.ExecuteSQL(pSql2)
                        AppendToLocalLog("Records deleted from " & vTableName & " : " & pRecordsDeleted.ToString(cGlobal.FormatInteger) &
                            ". Duration: " & pSW.ElapsedMilliseconds().ToString(cGlobal.FormatInteger) & " ms." &
                            " Method: from pre-SELECT-ed IDs")
                    Next
                End If

                If pRecordsDeleted = 0 Then
                    Exit Do
                End If
                pOut += pRecordsDeleted
            Loop
        Else
            pDR = mProData.ExecuteRow(pSql)
            pOut = pDR("Count")
        End If
        mResult.Add(vTableName.Trim().ToLower(), pOut)
        Dim pDrs = mDtTables.Select("Name = " & ReadyString(vTableName))
        If pDrs.Length > 0 Then
            pDrs(0)("DeletableCount") = pOut
            If pDR IsNot Nothing Then
                If vAdditionalFields?.ContainsKey("TotalAmount") Then
                    pDrs(0)("TotalAmount") = pDR("TotalAmount")
                End If
                If vAdditionalFields?.ContainsKey("MinDate") Then
                    pDrs(0)("MinDate") = pDR("MinDate")
                End If
                If vAdditionalFields?.ContainsKey("MaxDate") Then
                    pDrs(0)("MaxDate") = pDR("MaxDate")
                End If
            End If
        End If
        TableToExcelFile(mDtTables, "", "", mFolder & "Tables " & DateFrom.ToString("yyyy-MM") & " pending.xlsx")
        mRecordCount += pOut

    End Sub

    Private Sub AppendToLocalLog(vText As String)

        LastLogDate = Now
        LastLog = vText
        If IO.Path.IsPathRooted(mFolder) Then
            AppendToLog(vText, mFolder)
        Else
            AppendToLog(vText, IO.Path.Combine(Common.ServerPath & "temp\OldDataDeleter", mFolder))
        End If

    End Sub

    Public Shared Function GetLastLog() As String

        Dim pOut As String = "Date Started: "
        If DateStarted.HasValue Then
            pOut &= Format(DateStarted.Value, cGlobal.FormatDateTime)
        End If
        pOut &= vbCrLf
        pOut &= "Last Log Date: "
        If LastLogDate.HasValue Then
            pOut &= Format(LastLogDate.Value, cGlobal.FormatDateTime)
        End If
        pOut &= vbCrLf
        pOut &= "Last Log: " & LastLog

        Return pOut

    End Function

    Public ReadOnly Property RecordCount As Long
        Get
            Return mRecordCount
        End Get
    End Property
End Class
