Public Class TransSalesReturnLedger
    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetByMemoID(vMemoID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT ID" &
            ",SrvNum" &
            ",Pin" &
            ",FowstID" &
            " FROM TransSalesReturnLedger" &
            " WHERE SourceMemoID = " & ReadyString(vMemoID) &
            " AND DestinationPaymentID IS NULL"

        Return mProData.ExecuteRow(pSql)

    End Function

    Public Function GetAllByMemoID(vMemoID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT ID" &
            ",SrvNum" &
            ",Pin" &
            ",FowstID" &
            ",DestinationPaymentID" &
            ",LastUpdated" &
            " FROM TransSalesReturnLedger" &
            " WHERE SourceMemoID = " & ReadyString(vMemoID)

        Return mProData.ExecuteRow(pSql)

    End Function

    Public Function DestinationPaymentCount(vMemoID As String) As Integer

        Dim pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " WHERE SourceMemoID = " & ReadyString(vMemoID) & vbCrLf &
            " AND DestinationPaymentID IS NOT NULL"
        Dim pOut = mProData.ExecuteScalar(pSql)

        Return pOut

    End Function

    Public Function GetByAmount(vAmount As Decimal) As DataTable

        Dim pSql As String

        pSql = "SELECT ID" &
            ",SrvNum" &
            ",Pin" &
            ",FowstID" &
            ",LastUpdated" &
            " FROM TransSalesReturnLedger" &
            " WHERE Amount = " & ReadyString(vAmount) &
            " AND DestinationPaymentID IS NULL" & vbCrLf &
            " ORDER BY LastUpdated DESC"

        Return mProData.Execute(pSql)

    End Function

    ''' <summary>
    ''' this must be inside a transaction
    ''' </summary>
    ''' <param name="vMemoID"></param>
    ''' <param name="vRefund"></param>
    ''' <param name="vSubscriberOrganizationBranchID">LibraryCustomersAddresses.ID</param>
    ''' <remarks></remarks>
    Public Sub Save(vMemoID As String, vRefund As Boolean, vSubscriberOrganizationBranchID As String,
        vDeviceID As String, Optional vFowstID As String = Nothing, Optional UUID As String = "")

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pInvoiceID As String
        Dim pDtSalesReturn As DataTable
        Dim pNum As Integer

        If vRefund Then
        Else
            pNum = CurrentSrvNumber(vSubscriberOrganizationBranchID, vDeviceID) + 1
        End If
        pDtSalesReturn = mParent.TransMemoSalesReturn.GetMemoAmount(vMemoID)
        If pDtSalesReturn.Rows.Count > 1 Then
            Throw New ApplicationException("Sales Return must not contain more than 1 invoice")
        End If
        If pDtSalesReturn.Rows.Count = 0 Then
            Throw New ApplicationException("Sales Return not found")
        End If
        pInvoiceID = CStr2(pDtSalesReturn.Rows(0)("InvoiceID"))

        pIU = New InsertUpdate("TransSalesReturnLedger")
        pIU("SourceMemoID") = ReadyString(vMemoID)
        pIU("FowstID") = ReadyString(vSubscriberOrganizationBranchID)
        If vRefund Then
            pIU("GrossRefundAmount") = CDec2(pDtSalesReturn.Rows(0)("TotalAmount"))
            pIU("GrossSrvAmount") = 0
            pIU("RefundNum") = "dbo.GetOrganizationFOWSTRefNo(" & ReadyString(vSubscriberOrganizationBranchID) & ", " & ReadyString(vDeviceID) & ") + 1"
        Else
            pIU("GrossRefundAmount") = 0
            pIU("GrossSrvAmount") = CDec2(pDtSalesReturn.Rows(0)("TotalAmount"))
            pIU("SrvNum") = pNum
        End If
        pIU("Fees") = CDec2(pIU("GrossSrvAmount")) * CDec2(mParent.SystemDefaults.SalesReturnRestockingFeePercent) / 100
        pIU("RefundFees") = CDec2(pIU("GrossRefundAmount")) * CDec2(mParent.SystemDefaults.SalesReturnRestockingFeePercent) / 100
        pIU("Amount") = CDec2(pIU("GrossSrvAmount")) - CDec2(pIU("Fees"))  ' net amount
        'If CDec2(pIU("GrossSrvAmount")) + CDec2(pIU("GrossRefundAmount")) = 0 Then
        '    Throw New ApplicationException("Memo amount must be greater than zero")
        'End If
        If vFowstID Is Nothing Then
            vFowstID = WebRequest.Instance.FowstID
        End If
#If _MYTYPE = "Web" Then
        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        pIU("FowstID") = ReadyString(vFowstID)
#Else
        pIU("UserID") = User.EmployeeID
#End If
        pIU("Pin") = ReadyString(GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper)
        pIU("SourceInvoiceID") = ReadyString(pInvoiceID)
        pIU("DeviceID") = ReadyString(vDeviceID)

        pSql = pIU.GetInsert

        mProData.ExecuteSQL(pSql)
        mParent.TransInvoice.UpdateAmounts(pInvoiceID)
        SaveSrvNumToCounterFile(vSubscriberOrganizationBranchID, vDeviceID, pNum)

        If vRefund Then
            pSql = "SELET * FROM TransCustomerPaymentInvoices " & vbCrLf &
                " WHERE InvoiceID = " & ReadyString(pInvoiceID)

        End If

        '// update counter
        'pIU = New InsertUpdate("MobizLibraryOrganizationFOWSTPOSSettings")
        'pIU("SrvNum") = "dbo.GetOrganizationFOWSTSRVNo(" & ReadyString(vSubscriberOrganizationBranchID) & ") + 1"
        'pSql = pIU.GetUpdate("FOWSTID = " & ReadyString(vSubscriberOrganizationBranchID))
        'If mProData.ExecuteSQL(pSql) = 0 Then
        '    pSql = pIU.GetInsert
        '    mProData.ExecuteSQL(pSql)
        'End If

        pIU = New InsertUpdate("LibraryDeviceFowst")
        If vRefund Then
            pIU("POSLastRefNo") = "dbo.GetOrganizationFOWSTRefNo(" & ReadyString(vSubscriberOrganizationBranchID) & ", " & ReadyString(vDeviceID) & ") + 1"
        Else
            pIU("POSLastSRVNo") = pNum
        End If
        pSql = pIU.GetUpdate("FOWSTID = " & ReadyString(vSubscriberOrganizationBranchID)) &
            " AND DeviceID = " & ReadyString(vDeviceID)
        If mProData.ExecuteSQL(pSql) = 0 Then
            pIU("FOWSTID") = ReadyString(vSubscriberOrganizationBranchID)
            pIU("DeviceID") = ReadyString(vDeviceID)
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        End If

        'ZReadCache Function
        UpdateZReadCache(vDeviceID, vSubscriberOrganizationBranchID, vMemoID)

        If Not CStr2(UUID) = vbNullString Then
            UpdateZReadCache(vDeviceID, vSubscriberOrganizationBranchID, vMemoID, True)
        End If

        UpdateEJournalCache(vMemoID, vSubscriberOrganizationBranchID, vDeviceID)

        Dim DashboardsAndReports = New DashBoardsAndReports
        Dim pCustomerID As String

        pSql = "Select CustomerID from TransInvoice where ID = " & ReadyString(pInvoiceID)

        pCustomerID = CStr2(mProData.ExecuteScalar(pSql))

        'DashboardsAndReports.CacheCustomerTransactionSummary(vFowstID, pCustomerID, Today, Today, mProData)

    End Sub

    Public Function CurrentSrvNumber(vFowstID As String, vDeviceID As String) As Integer

        Dim pSql = "SELECT ISNULL(devfowst.POSLastSRVNo,0)" & vbCrLf &
            " FROM LibraryDeviceFowst devfowst" & vbCrLf &
            " WHERE devfowst.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND devfowst.DeviceID = " & ReadyString(vDeviceID)
        Dim pOut = CInt2(mProData.ExecuteScalar(pSql))
        Dim pFileCounter = CurrentSrvNumberFromCounterFile(vFowstID, vDeviceID)
        If pOut < pFileCounter Then
            pOut = pFileCounter
        End If

        Dim pTransNum = CurrentSrvNumberFromTrans(vFowstID, vDeviceID)
        If pOut < pTransNum Then
            pOut = pTransNum
        End If

        Return pOut

    End Function

    Private Function CurrentSrvNumberFromTrans(vFowstID As String, vDeviceID As String) As Integer

        Dim pSql = " SELECT MAX(SrvNum)" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " WHERE TransSalesReturnLedger.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND TransSalesReturnLedger.DeviceID = " & ReadyString(vDeviceID)
        Dim pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Private Function CurrentSrvNumberFromCounterFile(vFowstID As String, vDeviceID As String) As Integer

        Dim pFilename = Common.ServerPath & "settings\Counters\"
        ProPack.Utility.AddDirectoryUser(pFilename)
        pFilename &= ReadyGuid(vFowstID) & "-" & ReadyGuid(vDeviceID) & "-" & "SRV" & ".txt"
        Dim pFileCounter = CInt2(FileText(pFilename))

        Return pFileCounter

    End Function

    Private Function SaveSrvNumToCounterFile(vFowstID As String, vDeviceID As String, vValue As Integer) As Integer

        Dim pFilename = Common.ServerPath & "settings\Counters\"
        ProPack.Utility.AddDirectoryUser(pFilename)
        pFilename &= ReadyGuid(vFowstID) & "-" & ReadyGuid(vDeviceID) & "-" & "SRV" & ".txt"
        Dim pFileCounter = CInt2(FileText(pFilename))
        If pFileCounter < vValue Then
            pFileCounter = vValue
            FileText(pFilename) = pFileCounter
        End If

        Return pFileCounter

    End Function

    Public Function GetRemainingValue(ByVal vSrvNum As String, vPin As String,
        ByRef oMemoID As String) As Decimal

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT SourceMemoID " & vbCrLf & _
            " FROM TransSalesReturnLedger" & vbCrLf & _
            " WHERE SrvNum = " & ReadyString(vSrvNum) & vbCrLf &
            " AND Pin = " & ReadyString(vPin)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            oMemoID = CStr2(pDR("SourceMemoID"))
            Return GetRemainingValue(oMemoID)
        End If

    End Function

    Public Function GetRemainingValue(ByVal vMemoID As String) As Decimal

        Dim pDR As DataRow
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT SUM(ISNULL(Amount, 0)) AS Amount " &
            ",SUM(ISNULL(ForfeitedAmount, 0)) AS ForfeitedAmount " &
            ",COUNT(*) AS Count" & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf & _
            " WHERE SourceMemoID = " & ReadyString(vMemoID) & vbCrLf
        pDR = mProData.ExecuteRow(pSql)
        If CDec2(pDR("Count")) = 0 Then
            pDT = mParent.TransMemoSalesReturn.GetMemoAmount(vMemoID)
            Return CDec2(pDT.Compute("SUM(TotalAmount)", ""))
        Else
            Return (pDR("Amount") - pDR("ForfeitedAmount"))
        End If

    End Function

    ''' <summary>
    ''' use this in a transaction
    ''' </summary>
    ''' <param name="vMemoID"></param>
    ''' <param name="vDestinationPaymentID"></param>
    ''' <param name="vAmount"></param>
    ''' <param name="vSubscriberOrganizationBranchID">LibraryCustomersAddresses.ID</param>
    ''' <param name="oError"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Consume(vMemoID As String,
        vDestinationPaymentID As String,
        vAmount As Decimal,
        vSubscriberOrganizationBranchID As String,
        ByRef oError As String) As Boolean

        Dim pRemaining As Decimal
        Dim pIU As InsertUpdate
        Dim pDR As DataRow
        Dim pSql As String

        oError = ""
        mProData.BeginTransaction()
        pRemaining = GetRemainingValue(vMemoID)
        pRemaining = RoundUp(pRemaining, 2)
        vAmount = RoundUp(vAmount, 2)
        If pRemaining < vAmount Then
            oError = "The amount to consume (" & Format(vAmount, cGlobal.FormatCurrency) & ") is greater than the SRV's remaining amount (" & Format(pRemaining, cGlobal.FormatCurrency) & ")"
            Return False
        End If

        pDR = GetByMemoID(vMemoID)
        If pDR Is Nothing Then
            Save(vMemoID, False, vSubscriberOrganizationBranchID, WebRequest.Instance.DeviceID)
            pDR = GetByMemoID(vMemoID)
            If pDR Is Nothing Then
                oError = "MemoID does not exist"
                Return False
            End If

            ' BEGIN OLD CODE 2021_07_17__17_03_03_5606
            'Else
            '    pIU = New InsertUpdate("TransSalesReturnLedger")
            '    pIU("LastUpdated") = "GETDATE()"

            '    ' BEGIN OLD CODE 2018_01_17__15_33_00_6266
            '    '// change PIN (sept 30, 2013)
            '    'pIU("Pin") = ReadyString(GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper)
            '    'pSql = pIU.GetUpdate("SourceMemoID = " & ReadyString(vMemoID) &
            '    '    " AND DestinationPaymentID IS NULL")
            '    ' END OLD CODE 2018_01_17__15_33_00_6266

            '    pSql = pIU.GetUpdate("SourceMemoID = " & ReadyString(vMemoID))
            '    pSql = mProData.ExecuteSQL(pSql)
            '    If pSql <> 1 Then
            '        Throw New ApplicationException("Updated record count is not 1 while trying the update PIN")
            '    End If
            ' END OLD CODE 2021_07_17__17_03_03_5606
        End If

        pIU = New InsertUpdate("TransSalesReturnLedger")
        pIU("SourceMemoID") = ReadyString(vMemoID)
        pIU("DestinationPaymentID") = ReadyString(vDestinationPaymentID)

        pIU("Amount") = -vAmount

        With WebRequest.Instance.Business.SystemDefaults
            If (.SalesReturntSRVSettings = "1-Time Use of SRV") Then
                pIU("ForfeitedAmount") = " (SELECT SUM(ISNULL(Amount,0)) FROM TransSalesReturnLedger WHERE SourceMemoID = " & ReadyString(vMemoID) & ") - " & vAmount
            Else
                pIU("ForfeitedAmount") = 0
            End If
        End With

#If _MYTYPE = "Web" Then
        pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        pIU("FowstID") = ReadyString(pDR("FowstID"))
#Else
        pIU("UserID") = User.EmployeeID
#End If
        pIU("SrvNum") = ReadyString(pDR("SrvNum"))
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

        mProData.CommitTransaction()

        Return True

    End Function

    ''' <summary>
    ''' Added by janine May 20 2015
    ''' 
    ''' This function is an addition to the sales return function, particularly, REFUND. 
    ''' This is to insert a negative or contra value to the payment made.
    ''' Example: if the client returned all of the items in an invoice (it should work like void),
    ''' a negative value of the amount paid will be inserted so that 
    ''' in the dashboard sales reports, it will be easy to sum and still get the correct
    ''' amount in the inventory of payments. so if the cashamount is 100 originally, and 
    ''' the client wants a refund, another -100 will be inserted.
    ''' 
    ''' For invoices wherein only part of the items are refunded, there is a formula applied.
    ''' the payment method average will be used as contra value of the amount (in case of multiple payment type made)
    ''' 
    ''' Also note that for payment methods such as Other Type and Credit Card, 
    ''' there is a specific contra value inserted to to their respective tables.
    ''' This is because Other Type payment methods can be SMAC, EPlus, or voucher and the credit card
    ''' could be visa, mastercard, etc. For easier summation purposes in the dashboard reports,
    ''' the inserted contra values for refund are necessary.
    ''' </summary>
    ''' <param name="vMemoID"></param>
    ''' <remarks></remarks>
    Sub SaveSalesPaymentsRefund(ByVal vMemoID As String)
        Dim p As New InsertUpdate("TransCustomerPayment")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDR As DataRow
        Dim pNow As Date
        Dim clsSalesPayments As New SalesPayments
        Dim customerPaymentID As String
        Dim pDT As DataTable

        Dim invoiceID As String
        Dim refundAmount As Decimal
        Dim originalTotalAmount As Decimal

        Dim cashAmount As Decimal
        Dim cashChangeAmount As Decimal
        Dim checkAmount As Decimal
        Dim creditCardAmount As Decimal
        Dim debitCardAmount As Decimal
        Dim srvAmount As Decimal
        Dim otherTypeAmount As Decimal
        Dim pDtPayments As DataTable

        Try
            pDT = mParent.TransMemoSalesReturn.GetMemoAmount(vMemoID)
            invoiceID = pDT.Rows(0).Item("InvoiceID").ToString()

            refundAmount = CDec2(pDT.Rows(0).Item("TotalAmount").ToString())

            pDtPayments = clsSalesPayments.IsInvoiceSaved(invoiceID)
            If pDtPayments.Rows.Count = 0 Then
                Exit Sub
            End If
            customerPaymentID = pDtPayments.Rows(0).Item("CustomerPaymentID").ToString()
            pDR = clsSalesPayments.getSalesPaymentInfo(customerPaymentID).Rows(0)


            originalTotalAmount = CDec2(pDR.Item("TotalAmountDue").ToString()) - CDec2(pDR.Item("DiscountAmount").ToString())
            If originalTotalAmount = 0 Then
                Exit Sub
            End If

            'do averaging here
            'cashamount - change
            cashAmount = CDec2(pDR.Item("CashAmount").ToString()) / originalTotalAmount
            cashChangeAmount = CDec2(pDR.Item("CustomerChangeCash").ToString()) / originalTotalAmount
            checkAmount = CDec2(pDR.Item("CheckPaymentAmount").ToString()) / originalTotalAmount
            creditCardAmount = CDec2(pDR.Item("CreditCardAmount").ToString()) / originalTotalAmount
            debitCardAmount = CDec2(pDR.Item("DebitCardAmount").ToString()) / originalTotalAmount
            srvAmount = CDec2(pDR.Item("SRVAmount").ToString()) / originalTotalAmount
            otherTypeAmount = CDec2(pDR.Item("OtherTypeAmount").ToString()) / originalTotalAmount

            p("CashAmount") = cashAmount * refundAmount * -1
            p("CustomerChangeCash") = cashChangeAmount * refundAmount * -1
            p("DebitCardAmount") = debitCardAmount * refundAmount * -1
            p("CreditCardAmount") = creditCardAmount * refundAmount * -1
            p("CheckPaymentAmount") = checkAmount * refundAmount * -1
            p("SRVAmount") = srvAmount * refundAmount * -1
            p("OtherTypeAmount") = otherTypeAmount * refundAmount * -1

            p("PaymentDate") = "getdate()"

            p("ReceiptType") = ReadyString("RF") 'make this RF (refund) to differentiate from the OR official receipt.
            p("ReceiptNumber") = ReadyString(pDR.Item("ReceiptNumber").ToString())
            p("CustomerID") = ReadyString(pDR.Item("CustomerID").ToString())

            p("DiscountAmount") = 0
            p("ChargeAmount") = 0
            p("TotalAmountDue") = 0

            p("AmountChange") = 0 'not used
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)


            otherTypeAmount = otherTypeAmount * refundAmount
            creditCardAmount = creditCardAmount * refundAmount


            '' for other type payment breakdown
            If (otherTypeAmount > 0) Then

                pDT = clsSalesPayments.getCustomerPaymentOtherDetails(customerPaymentID)
                Dim newAmount As Decimal = otherTypeAmount / pDT.Rows.Count

                For Each obj As Object In pDT.Rows

                    '''Sales Payment Other
                    p = New InsertUpdate("TransCustomerPaymentOther")

                    p("CustomerPaymentID") = ReadyString(customerPaymentID)
                    p("PaymentType") = ReadyString(obj("PaymentType").ToString())
                    p("Amount") = newAmount * -1
                    p("IssuingOrganization") = ReadyString("")
                    p("CardNumber") = ReadyString("")
                    p("CardHolderName") = ReadyString("")
                    p("CardIssuanceDate") = "NULL"
                    p("CardExpirationDate") = "NULL"
                    p("ApprovalNo") = ReadyString("")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                Next


            End If

            '' for credit card breakdown
            If (creditCardAmount > 0) Then

                pDT = clsSalesPayments.getCustomerPaymentCreditCardDetails(customerPaymentID)
                Dim newAmount As Decimal = creditCardAmount / pDT.Rows.Count

                For Each obj As Object In pDT.Rows

                    '''Sales Payment Other
                    p = New InsertUpdate("TransCustomerPaymentCreditCard")

                    p("CustomerPaymentID") = ReadyString(customerPaymentID)
                    p("CreditCardType") = ReadyString(obj("CreditCardType").ToString())
                    p("CreditCardBankID") = "NULL"
                    p("CreditCardBankName") = ReadyString("")
                    p("BankCountryID") = "NULL"
                    p("HolderLastName") = ReadyString("")
                    p("HolderFirstName") = ReadyString("")
                    p("HolderMiddleName") = ReadyString("")
                    p("HolderGenerationalSuffix") = ReadyString("")
                    p("CreditCardNumber") = ReadyString("")
                    p("CreditCardIssuanceDate") = "NULL"
                    p("CreditCardExpirationDate") = "NULL"
                    p("ApprovalNo") = ReadyString("")
                    p("NoOfInstallmentPayments") = 0
                    p("CreditCardAmount") = newAmount * -1
                    p("IsBankFromOrg") = 0

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                Next


            End If


            '''Save Invoice
            p = New InsertUpdate("TransCustomerPaymentInvoices")

            p("CustomerPaymentID") = ReadyString(strIDReturn)
            p("InvoiceID") = ReadyString(invoiceID)
            'just put zero. we only need to populate this to link the invoiceid
            p("InvoiceAmount") = 0
            p("DiscountAmount") = 0
            p("Balance") = 0
            p("AmountPaid") = 0

            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)


        Catch ex As Exception
            Throw
        End Try

    End Sub

    Function UpdateZReadCache(pDeviceID As String, pFOWSTID As String, vMemoID As String, Optional IsPOS As Boolean = False) As String
        Dim ZReadDate As String
        Dim pSql As String
        'Dim InvoiceDate As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim IsNew As Boolean = False
        Dim dtCurrZRead As DataTable
        Dim dtPrevZRead As DataTable
        Dim mCurrentDatabase As String
        'pSql = "Select InvoiceDate from TransInvoice where ID = " & ReadyString(mInvoiceID)

        'InvoiceDate = CStr2(mProData.ExecuteScalar(pSql))

        pDT = GetMemoRead(vMemoID, Today)

        pDT.DefaultView.RowFilter = "CurSalesReturnRefund>0 OR CurSalesReturnExchange>0"
        Dim pBusiness As Business
        If IsPOS Then
            mCurrentDatabase = mProData.GetCurrentDatabase()

            pBusiness = New Business(ProData.CreateFromConnectionInfo(
                       vServer:=Common.ConnectionInfo.Server,
                       vDatabase:="MobizWebPOSCache",
                       vUsername:=Common.ConnectionInfo.Username,
                       vPassword:=Common.ConnectionInfo.Password,
                       vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
                   ))
        Else
            pBusiness = mParent
        End If

        dtCurrZRead = pBusiness.TransSalesReturnLedger.GetCurrentZReadID(Today, pDeviceID, pFOWSTID)

        If dtCurrZRead.Rows.Count = 0 Then
            IsNew = True
            dtPrevZRead = pBusiness.TransSalesReturnLedger.GetPreviousZReadID(Today, pDeviceID, pFOWSTID)

            If dtPrevZRead.Rows.Count = 0 Then
                'Get ZRead Data from old query
                Dim ReportPOS As New ReportPOS

                dtPrevZRead = ReportPOS.getSalesZReportSummary("By Date", CStr2(CDate(Today).AddDays(-1)), pFOWSTID, pDeviceID, pBusiness.ProData)
            End If
        End If

        Dim p As New InsertUpdate("TransZReadCache")
        Dim dtMemoData As DataTable

        If pDT.DefaultView.Count = 0 Then
            Return Nothing
        End If
        dtMemoData = pDT.DefaultView(0).DataView.ToTable

        If IsNew Then
            p("DeviceID") = ReadyString(pDeviceID)
            p("FOWSTID") = ReadyString(pFOWSTID)

            If dtPrevZRead.Rows.Count > 0 Then
                If CDec2(dtPrevZRead.Rows(0).Item("PresentationPrevGrossSales")) = 0 Then
                    p("PresentationPrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
                Else
                    p("PresentationPrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PresentationPrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("PresentationCurrGrossSales"))
                End If

                p("PrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
                p("PrevTotalDiscount") = CDec2(dtPrevZRead.Rows(0).Item("PrevTotalDiscount")) + CDec2(dtPrevZRead.Rows(0).Item("CurTotalDiscount"))
                p("PrevSalesReturnRefund") = CDec2(dtPrevZRead.Rows(0).Item("PrevSalesReturnRefund")) + CDec2(dtPrevZRead.Rows(0).Item("CurSalesReturnRefund"))
                p("PrevSalesReturnExchange") = CDec2(dtPrevZRead.Rows(0).Item("PrevSalesReturnExchange")) + CDec2(dtPrevZRead.Rows(0).Item("CurSalesReturnExchange"))
                p("PrevTotalVAT") = CDec2(dtPrevZRead.Rows(0).Item("PrevTotalVAT")) + CDec2(dtPrevZRead.Rows(0).Item("CurTotalVAT"))
                p("PrevServiceCharge") = CDec2(dtPrevZRead.Rows(0).Item("PrevServiceCharge")) + CDec2(dtPrevZRead.Rows(0).Item("CurServiceCharge"))
                p("PrevChargeAmount") = CDec2(dtPrevZRead.Rows(0).Item("PrevChargeAmount")) + CDec2(dtPrevZRead.Rows(0).Item("ChargeAmount"))
            End If

            p("SalesDate") = ReadyString(Today)
            p("CurSalesReturnRefund") = dtMemoData.Rows(0).Item("CurSalesReturnRefund")
            p("CurSalesReturnExchange") = dtMemoData.Rows(0).Item("CurSalesReturnExchange")
            p("CurTotalVATSalesSalesReturn") = dtMemoData.Rows(0).Item("CurTotalVATSalesSalesReturn")
            p("CurTotalVATAmountSalesReturn") = dtMemoData.Rows(0).Item("CurTotalVATAmountSalesReturn")
            p("CurTransCountMemoReturnExchange") = dtMemoData.Rows(0).Item("CurTransCountMemoReturnExchange")
            p("CurSalesReturnExchangeItemCount") = dtMemoData.Rows(0).Item("CurSalesReturnExchangeItemCount")
            p("CurTransCountMemoRefund") = dtMemoData.Rows(0).Item("CurTransCountMemoRefund")
            p("CurSalesRefundItemCount") = dtMemoData.Rows(0).Item("CurSalesRefundItemCount")
            p("CurTermNum") = CInt2(dtMemoData.Rows(0).Item("CurTermNum"))
            p("FowstNo") = dtMemoData.Rows(0).Item("FowstNo")
        Else
            p("CurSalesReturnRefund") = CDec2(dtCurrZRead.Rows(0).Item("CurSalesReturnRefund")) + dtMemoData.Rows(0).Item("CurSalesReturnRefund")
            p("CurSalesReturnExchange") = CDec2(dtCurrZRead.Rows(0).Item("CurSalesReturnExchange")) + dtMemoData.Rows(0).Item("CurSalesReturnExchange")
            p("CurTotalVATSalesSalesReturn") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalVATSalesSalesReturn")) + dtMemoData.Rows(0).Item("CurTotalVATSalesSalesReturn")
            p("CurTotalVATAmountSalesReturn") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalVATAmountSalesReturn")) + dtMemoData.Rows(0).Item("CurTotalVATAmountSalesReturn")
            p("CurTransCountMemoReturnExchange") = CDec2(dtCurrZRead.Rows(0).Item("CurTransCountMemoReturnExchange")) + dtMemoData.Rows(0).Item("CurTransCountMemoReturnExchange")
            p("CurSalesReturnExchangeItemCount") = CDec2(dtCurrZRead.Rows(0).Item("CurSalesReturnExchangeItemCount")) + dtMemoData.Rows(0).Item("CurSalesReturnExchangeItemCount")
            p("CurTransCountMemoRefund") = CDec2(dtCurrZRead.Rows(0).Item("CurTransCountMemoRefund")) + dtMemoData.Rows(0).Item("CurTransCountMemoRefund")
            p("CurSalesRefundItemCount") = CDec2(dtCurrZRead.Rows(0).Item("CurSalesRefundItemCount")) + dtMemoData.Rows(0).Item("CurSalesRefundItemCount")
        End If

        If IsNew Then
            pSql = p.GetInsert()
        Else
            pSql = p.GetUpdate("ID = " & ReadyString(dtCurrZRead.Rows(0).Item("ID")))
        End If

        pBusiness.ProData.Execute(pSql)

    End Function


    Public Function UpdateEJournalCache(mMemoID As String, mFOWSTID As String, mDeviceID As String) As Boolean
        Dim pSql As String
        Dim dtMemoDetails As DataTable

        dtMemoDetails = GetInvoiceDetailsForEJournal(mMemoID)

        If dtMemoDetails.Rows.Count > 0 Then

            For i As Integer = 0 To dtMemoDetails.Rows.Count - 1
                Dim p As New InsertUpdate("TransSalesInvoiceAndRebatesCache")

                p("FOWSTID") = ReadyString(mFOWSTID)
                p("DeviceID") = ReadyString(mDeviceID)
                p("TransactionID") = ReadyString(dtMemoDetails.Rows(i).Item("ID"))

                p("TransactionType") = ReadyString(dtMemoDetails.Rows(i).Item("TransactionType"))

                p("Customer") = ReadyString(dtMemoDetails.Rows(i).Item("Customer"))
                p("CashierName") = ReadyString(dtMemoDetails.Rows(i).Item("CashierName"))
                p("ProductNo") = ReadyString(dtMemoDetails.Rows(i).Item("ProductNo"))
                p("ProductName") = ReadyString(dtMemoDetails.Rows(i).Item("ProductName"))
                p("TransactionNo") = ReadyString(dtMemoDetails.Rows(i).Item("TransactionNo"))
                p("TransactionDate") = ReadyString(dtMemoDetails.Rows(i).Item("TransactionDate"))
                p("Quantity") = CDbl(dtMemoDetails.Rows(i).Item("Quantity"))
                p("UnitMeasure") = ReadyString(dtMemoDetails.Rows(i).Item("UnitMeasure"))
                p("UnitPrice") = CDbl(dtMemoDetails.Rows(i).Item("UnitPrice"))
                p("SubTotal") = CDbl(dtMemoDetails.Rows(i).Item("SubTotal"))
                p("DiscountAmount") = CDbl(dtMemoDetails.Rows(i).Item("DiscountAmount"))
                p("DiscountedUnitPrice") = CDbl(dtMemoDetails.Rows(i).Item("DiscountedUnitPrice"))
                p("DiscountedSubTotal") = CDbl(dtMemoDetails.Rows(i).Item("DiscountedSubTotal"))
                p("CashAmount") = CDbl(dtMemoDetails.Rows(i).Item("CashAmount"))
                p("CheckPaymentAmount") = CDbl(dtMemoDetails.Rows(i).Item("CheckPaymentAmount"))
                p("DebitCardAmount") = CDbl(dtMemoDetails.Rows(i).Item("DebitCardAmount"))
                p("CreditCardAmount") = CDbl(dtMemoDetails.Rows(i).Item("CreditCardAmount"))
                p("SRVAmount") = CDbl(dtMemoDetails.Rows(i).Item("SRVAmount"))
                p("OnCreditAmount") = CDbl(dtMemoDetails.Rows(i).Item("OnCreditAmount"))
                p("OtherTypeAmount") = CDbl(dtMemoDetails.Rows(i).Item("OtherTypeAmount"))
                p("VATPercent") = CDbl(dtMemoDetails.Rows(i).Item("VATPercent"))
                p("VatableSales") = CDec2(dtMemoDetails.Rows(i).Item("VatableSales"))
                p("VATAmount") = CDec2(dtMemoDetails.Rows(i).Item("VATAmount"))
                p("CustomerChangeCash") = CDec2(dtMemoDetails.Rows(i).Item("CustomerChangeCash"))

                pSql = p.GetInsert()

                mProData.Execute(pSql)
            Next


            Return True
        End If

    End Function

    Function GetInvoiceDetailsForEJournal(mMemoID As String) As DataTable
        Dim pSql As String

        pSql = " SELECT " &
                " invlgr.TransactionID as ID " &
                " 	,fowst.Name as FOWST " &
                " 	,dev.PhoneName as DeviceName " &
                " 	,prd.ProductNo " &
                " 	,prd.ProductName " &
                " 	,fbin.[Name] as Bin " &
                " 	,invlgr.TransactionType + (CASE ISNULL(sretldr.SrvNum,0) " &
                " 		                        WHEN 0 THEN ' (Sales Refund)' " &
                " 		                        ELSE ' (SRV)' " &
                " 	                          END) as TransactionType " &
                " 	,(CASE ISNULL(sretldr.SrvNum,0) " &
                " 		WHEN 0 THEN sretldr.RefundNum " &
                " 		ELSE  sretldr.SrvNum " &
                " 	END) as TransactionNo " &
                " 	,memo.MemoDate as TransactionDate " &
                " 	,cust.CompanyName as Customer " &
                " 	,sret.QuantityReturned as Quantity " &
                " 	,sret.UnitMeasure " &
                " 	,sret.UnitPrice * -1 as UnitPrice  " &
                " 	,sret.TotalAmount * -1 as SubTotal " &
                " 	,0 as DiscountAmount " &
                " 	,0 as DiscountedUnitPrice " &
                " 	,0 as DiscountedSubTotal " &
                " 	,12 as VATPercent " &
                " 	,(sret.TotalAmount - sret.TaxAmount) * -1 as VATableSales " &
                " 	,sret.TaxAmount * -1 as VATAmount " &
                " 	,inv.ReasonForReconstructingData " &
                " 	,ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') +  " &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') +  " &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') as CashierName " &
                "   , 0 CashAmount " &
                "   , 0 CheckPaymentAmount " &
                "   , 0 DebitCardAmount " &
                "   , 0 CreditCardAmount " &
                "   , 0 SRVAmount " &
                "   , 0 OnCreditAmount " &
                "   , 0 OtherTypeAmount " &
                "   , 0 CustomerChangeCash "

        'pSql = pSql & strProductColumns

        pSql = pSql &
                " FROM TransInventoryLedger invlgr  " &
                " LEFT JOIN LibraryFowstBin fbin ON (fbin.ID = invlgr.BinID) " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = invlgr.FOWSTID) " &
                " INNER JOIN TransMemoSalesReturn sret ON (sret.MemoID = invlgr.TransactionID AND sret.ProductID = invlgr.ProductID) " &
                " INNER JOIN TransMemo memo ON (sret.MemoID = memo.ID) " &
                " INNER JOIN TransSalesReturnLedger sretldr ON (memo.ID = sretldr.SourceMemoID AND sretldr.DestinationPaymentID IS NULL) " &
                " INNER JOIN TransInvoice inv ON (inv.ID = sret.InvoiceID) " &
                " INNER JOIN MobizLibraryOrganization cust ON (cust.ID = inv.CustomerID) " &
                " LEFT JOIN MobizLibraryOrganizationFOWST custbill ON (custbill.ID = inv.BillingAddressID) " &
                " LEFT JOIN MobizLibraryCities custbillcty ON (custbill.FowstCityID = custbillcty.ID)  " &
                " LEFT JOIN MobizLibraryCountries custbillctry ON (custbill.FowstCountryID = custbillctry.ID)  " &
                " LEFT JOIN MobizLibraryOrganizationFOWST custdel ON (custdel.ID = inv.DeliveryAddressID) " &
                " LEFT JOIN MobizLibraryCities custdelcty ON (custdel.FowstCityID = custdelcty.ID)  " &
                " LEFT JOIN MobizLibraryCountries custdelctry ON (custdel.FowstCountryID = custdelctry.ID)  " &
                " INNER JOIN LibraryDevice dev ON (dev.ID = memo.DeviceID) " &
                " INNER JOIN MobizLibraryPeople cshr ON (cshr.ID = memo.UserID) " &
                " INNER JOIN MobizLibraryPIMProduct prd ON (prd.ID = invlgr.ProductID) " &
                " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (prd.ID = prd001.ProductID) " &
                " LEFT JOIN MobizLibraryPIMProductVariableFeatures002 prd002 ON (prd.ID = prd002.ProductID) " &
                " WHERE invlgr.TransactionID = " & ReadyString(mMemoID)

        Return mProData.Execute(pSql)
    End Function

    Function GetCurrentZReadID(mDate As String, pDeviceID As String, pFOWSTID As String) As DataTable
        Dim pSql As String

        pSql = "Select * FROM TransZReadCache " & vbCrLf &
                " WHERE convert(date, SalesDate) = " & ReadyString(mDate) & vbCrLf &
                " AND FOWSTID = " & ReadyString(pFOWSTID) & vbCrLf &
                " AND DeviceID = " & ReadyString(pDeviceID)

        Return mProData.Execute(pSql)
    End Function

    Function GetPreviousZReadID(mDate As String, pDeviceID As String, pFOWSTID As String) As DataTable
        Dim pSql As String

        pSql = "Select top 1 * FROM TransZReadCache " & vbCrLf &
                " WHERE convert(date, SalesDate) < " & ReadyString(mDate) & vbCrLf &
                " AND FOWSTID = " & ReadyString(pDeviceID) & vbCrLf &
                " AND DeviceID = " & ReadyString(pFOWSTID) & vbCrLf &
                " Order by SalesDate desc "

        Return mProData.Execute(pSql)
    End Function

    Function GetMemoRead(mMemoID As String, MemoDate As String) As DataTable
        Dim pDt As DataTable
        Dim pSql As String
        Dim pFinder As Finder

        pFinder = New Finder

        With pFinder

            With .Fields
                With .Add("CurSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(memosr.TaxAmount) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTransCountMemoReturnExchange", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.SrvNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchangeItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.SrvNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTransCountMemoRefund", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesRefundItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE memo.ID = " & ReadyString(mMemoID) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTermNum", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("FowstNo", SqlDbType.Int)
                    .Expression = "fowst.FOWSTNo"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf " &
                    " INNER JOIN dbo.LibraryDevice dev ON (devf.DeviceID = dev.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) "

            pSql = .CreateSql
            pDt = mProData.Execute(pSql)

        End With

        Return pDt
    End Function

End Class
