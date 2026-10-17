Imports System.Linq

Public Class TransCustomerPayment

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function SavePayments(vJson As SavePaymentJson, ByRef oError As String) As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String
        Dim pTotalPaid As Decimal
        Dim pOrNum As Integer
        Dim pUpdateFileAction As Action =
            Sub()
                Console.WriteLine("Just a dummy to function to make not null. ref: vbQUA7NPYCXJ!")
            End Sub

        Try
            mParent.WebRequest.StopWatch("SavePayments vbTC6N9YV3E5!")
            If IsGuid(vJson.DeviceID) = False Then
                oError = "DeviceID must be a valid GUID. ref: vbSJ9CRYU4YW!"
                Return Nothing
            End If
            If IsGuid(vJson.FowstID) = False Then
                oError = "FowstID must be a valid GUID. ref: vbFPFJMTSGYG!"
                Return Nothing
            End If
            If VerifyPayment(vJson, oError) = False Then
                Return Nothing
            End If

            If VerifySrvPayments(vJson, oError) = False Then
                Return Nothing
            End If
            mProData.BeginTransaction()
            If CBool2(vJson.IsPos) _
                AndAlso vJson.Invoices IsNot Nothing _
                AndAlso vJson.Invoices.Count = 1 Then

                mParent.TransInvoice.FinalizeWebPos(vJson.Invoices(0).ID, vJson.OrgID, pUpdateFileAction)
            End If

            mParent.WebRequest.StopWatch("before VerifyInvoices vbEV285BUTJC!")
            If VerifyInvoices(vJson, oError) = False Then
                Return Nothing
            End If
            mParent.WebRequest.StopWatch("after VerifyInvoices vb8Q3XXZZ8LZ!")

            vJson.ConsumedAmount = (From pInvoice In vJson.Invoices
                                    Select pInvoice.AmountPaid).Sum()
            vJson.CheckAmount = (From pCheque In vJson.Cheques
                                 Select pCheque.CheckAmount).Sum()
            vJson.CreditCardAmount = (From pCreditCard In vJson.CreditCards
                                      Select pCreditCard.CreditCardAmount).Sum()
            vJson.DebitCardAmount = (From pDebitCard In vJson.DebitCards
                                     Select pDebitCard.DebitCardAmount).Sum()
            vJson.OtherTypeAmount = (From pOtherPayment In vJson.OtherPayments
                                     Select pOtherPayment.Amount).Sum()
            vJson.SrvPaymentAmount = (From pSrvPayment In vJson.SrvPayments
                                      Select pSrvPayment.ConsumedAmount).Sum()
            pTotalPaid = vJson.Cash +
                vJson.CheckAmount +
                vJson.CreditCardAmount +
                vJson.DebitCardAmount +
                vJson.OtherTypeAmount +
                vJson.SrvPaymentAmount +
                vJson.ConsumedUnappliedPaymentProper

            If Now.Date = #7/19/2023# Then
                AppendToLog("vbLPNVUEAT9G! vJson.AllowOverMaxOverPayment=" & vJson.AllowOverMaxOverPayment & vbCrLf &
                    "pTotalPaid=" & pTotalPaid & vbCrLf &
                    "vJson.ConsumedAmount=" & vJson.ConsumedAmount & vbCrLf &
                    "vJson.IsPos=" & vJson.IsPos,, "SavePayments vb6ZBPVVSP6W")
            End If

            If CBool2(vJson.AllowOverMaxOverPayment) = False _
                AndAlso CBool2(vJson.IsPos) _
                AndAlso vJson.ConsumedAmount > 0 Then

                Dim pOverPaymentPercent = (pTotalPaid - vJson.ConsumedAmount) / vJson.ConsumedAmount
                pOverPaymentPercent *= 100
                If pOverPaymentPercent > 500123456 Then
                    oError = "Overpayment must be less than 5%. Your overpayment is " & pOverPaymentPercent.ToString(cGlobal.FormatCurrency) & "%. " &
                    "Total payment = " & pTotalPaid.ToString(cGlobal.FormatCurrency) & ". " & vbCrLf &
                    "Total payable = " & vJson.ConsumedAmount.ToString(cGlobal.FormatCurrency) & "."

                    Return Nothing
                End If
            End If

            pIU = New InsertUpdate("TransCustomerPayment" & If(vJson.SaveAsDraftProper, "Draft", ""))
            pID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pID)
            pIU("UserID") = ReadyString(If(vJson.UserID Is Nothing, Guid.Empty.ToString, vJson.UserID))
            pIU("CustomerID") = ReadyString(vJson.CustomerID)
            pIU("CashAmount") = vJson.Cash
            pIU("ConsumedUnappliedPayment") = vJson.ConsumedUnappliedPaymentProper
            pIU("CheckPaymentAmount") = vJson.CheckAmount
            pIU("CreditCardAmount") = vJson.CreditCardAmount
            pIU("DebitCardAmount") = CDec2(vJson.DebitCardAmount)
            pIU("OtherTypeAmount") = vJson.OtherTypeAmount
            pIU("SRVAmount") = vJson.SrvPaymentAmount
            pIU("CustomerEmailAddresses") = ReadyString(vJson.CustomerEmailAddresses)

            If vJson.SaveAsDraftProper Then
                pIU("ReceiptNumber") = 0
                pIU("ReceiptType") = "'OR'"
            Else
                mParent.WebRequest.StopWatch("before GetLastNumber(POSLastORNo) vbKFUE8CHCAM!")
                pOrNum = mParent.LibraryDeviceFowst.GetLastNumber("POSLastORNo", vJson.FowstID, vJson.DeviceID)
                mParent.WebRequest.StopWatch("after GetLastNumber(POSLastORNo) vbLH6RYWRAD4!")
                pIU("ReceiptNumber") = pOrNum
                pIU("ReceiptType") = "'OR'"
            End If
            pIU("FowstID") = ReadyString(vJson.FowstID)
            pIU("DeviceID") = ReadyString(vJson.DeviceID)
            If CBool2(vJson.IsPos) Then
                Dim pUnappliedPayment = pTotalPaid - vJson.ConsumedAmount
                Dim pAmountChange = Math.Min(CDec2(vJson.Cash), pUnappliedPayment)
                pIU("CustomerChangeCash") = pAmountChange
                pIU("UnappliedPayment") = pUnappliedPayment - pAmountChange
            Else
                pIU("UnappliedPayment") = pTotalPaid - vJson.ConsumedAmount
            End If
            If String.IsNullOrWhiteSpace(vJson.ManualReceiptNumber) = False Then
                pIU("ManualReceiptNumber") = ReadyString(vJson.ManualReceiptNumber.Trim())
            End If
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            mParent.WebRequest.StopWatch("before SaveInvoices vb9R66GTPLNP!")
            If SaveInvoices(pID, vJson) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
            mParent.WebRequest.StopWatch("after SaveInvoices vbDWFA78AMJF!")
            mParent.WebRequest.StopWatch("before SaveCheques to SaveChecksAmount vbJUHRFQELVX!")
            If SaveCheques(pID, vJson) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
            If SaveCreditCards(pID, vJson) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
            If SaveDebitCards(pID, vJson) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
            If SaveOtherPayments(pID, vJson) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
            If SaveUnappliedCreditToUse(pID, vJson) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
            If SaveSrvPayments(pID, vJson, oError) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
            SaveChecksAmount(pID, vJson.SaveAsDraftProper)
            mParent.WebRequest.StopWatch("after SaveCheques to SaveChecksAmount vbMUA3DA8CPS!")
            If vJson.SaveAsDraftProper = False Then
                mParent.WebRequest.StopWatch("before CacheInvoicesWithCredit vbYPVEUJVMB8!")
                If vJson.Invoices IsNot Nothing _
                    And vJson.Invoices.Count = 1 Then

                    mParent.TransInvoice.CacheInvoicesWithCredit(vJson.Invoices(0).ID)
                Else
                    mParent.TransInvoice.CacheInvoicesWithCredit(, vJson.CustomerID)
                End If
                mParent.WebRequest.StopWatch("after CacheInvoicesWithCredit vbYPVEUJVMB8!")
                mParent.WebRequest.StopWatch("before UpdateLastNumber vbNRUZGL4K7L!")
                'mParent.LibraryDeviceFowst.UpdateLastNumber("POSLastORNo", vJson.FowstID, vJson.DeviceID, pOrNum)
                mParent.WebRequest.StopWatch("after UpdateLastNumber vbTLHNEP8PG4!")
            End If
            If CBool2(vJson.IsPos) Then
                Dim pImportDataClass As New ImportDataClass(mParent)
                mParent.WebRequest.StopWatch("before UpdateZReadCache vbGM4EJLJZXC!")
                For Each pInvoice In vJson.Invoices
                    pImportDataClass.UpdateZReadCache(pInvoice.ID)
                    UpdateZReadCache(pInvoice.ID)
                Next
                mParent.WebRequest.StopWatch("after UpdateZReadCache vbAKYK7NZ54S!")
            End If
            mProData.CommitTransaction()
            If pUpdateFileAction IsNot Nothing Then
                pUpdateFileAction.Invoke()
            End If
            mParent.WebRequest.StopWatch("end vbQF9LNJE8RB!")

            If String.IsNullOrWhiteSpace(vJson.CustomerEmailAddresses) = False Then
                mParent.TransInvoice.EmailReceiptHtml(vJson.Invoices(0).ID, True)
            End If
            Return pID
        Catch ex As ProSqlException
            AppendToDebug("vbW44WBL8F4P " & ex.Sql, True, ex, True)
            oError = ex.Message
        Catch ex As Exception
            AppendToDebug("vbX44KAA9EZU", True, ex, True)
            oError = ex.Message
        End Try

    End Function

    Private Function VerifyPayment(vJson As SavePaymentJson, ByRef oError As String) As Boolean

        Dim pAvailable As Decimal

        oError = ""
        If vJson.ConsumedUnappliedPaymentProper <> 0 Then
            If vJson.ConsumedUnappliedPaymentProper < 0 Then
                oError = "Consumed unapplied payment must be equal to or greater than zero."
                Return False
            End If

            pAvailable = mParent.LibraryCustomers.GetUnappliedPayments(vJson.CustomerID)
            If pAvailable < vJson.ConsumedUnappliedPaymentProper Then
                oError = "The Accumulated Unapplied Payment(s) And Credit(s) for this customer is " & Format(pAvailable, cGlobal.FormatCurrency) & ". You tried to consume " & Format(vJson.ConsumedUnappliedPaymentProper, cGlobal.FormatCurrency) & "."
                Return False
            End If
        End If

        If String.IsNullOrWhiteSpace(vJson.ManualReceiptNumber) = False Then
            Dim pDR = GetByManualReceiptNumber(vJson.ManualReceiptNumber)
            If pDR IsNot Nothing Then
                oError = "Manual receipt number must be unique. ref: vbWUWTEMEXWR!"
                Return False
            End If
        End If

        Return True

    End Function

    Private Function SaveInvoices(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pInvoice As SavePaymentJson.InvoicesClass In vJson.Invoices
            pIU = New InsertUpdate("TransCustomerPaymentInvoices" & If(vJson.SaveAsDraftProper, "Draft", ""))
            pIU("CustomerPaymentID") = ReadyString(vPaymentID)
            pIU("InvoiceID") = ReadyString(pInvoice.ID)
            pIU("AmountPaid") = pInvoice.AmountPaid
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            If vJson.IsPos Then
                Dim pRsdDateTime = ParseDate(vJson.RsdDateTime)
                mParent.TransInvoice.UpdateInvoiceDate(pInvoice.ID, pRsdDateTime)
            End If
        Next pInvoice

        Return True

    End Function

    Private Function VerifyInvoices(vJson As SavePaymentJson, ByRef oError As String) As Boolean

        Dim pDR As DataRow

        For Each pInvoice As SavePaymentJson.InvoicesClass In vJson.Invoices
            pDR = mParent.TransInvoice.FromCache(pInvoice.ID)
            If pDR Is Nothing Then
                oError = "Invoice " & pInvoice.ID & "(todo) was already paid. Please reload the page."
                Return False
            ElseIf CDec2(pDR("NetInvoiceBalance")) < pInvoice.AmountPaid Then
                oError = CStr2(pDR("InvoiceNumber")) & " amount paid (" & Format(pInvoice.AmountPaid, cGlobal.FormatCurrency) & ") is greater than invoice net balance (" & Format(CDec2(pDR("NetInvoiceBalance")), cGlobal.FormatCurrency) & ")"
                Return False
            ElseIf CBool2(vJson.IsPos) _
                AndAlso CDec2(pDR("NetInvoiceBalance")) > pInvoice.AmountPaid Then

                oError = "Amount paid (" & Format(pInvoice.AmountPaid, cGlobal.FormatCurrency) & ") is must be equal to or greater than the invoice net balance (" & Format(CDec2(pDR("NetInvoiceBalance")), cGlobal.FormatCurrency) & ")"
                Return False
            End If
            pDR = mParent.TransInvoice.ById(pInvoice.ID)
            Dim pTotalAmount = CDec(pDR("TotalAmount"))
            Dim pItemsAmount = mParent.TransInvoice.GetItemsTotal(pInvoice.ID)
            Dim pDiff = pTotalAmount - pItemsAmount
            pDiff = Math.Abs(pDiff)
            If pDiff > 0.01 Then
                oError = "Invoice amount is " & Format(pTotalAmount, cGlobal.FormatCurrency) & ". Items total is " & Format(pItemsAmount, cGlobal.FormatCurrency) & ". Please save your invoice."
                Return False
            End If
        Next pInvoice

        Return True

    End Function

    Private Function SaveCheques(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pCheque As SavePaymentJson.ChequeClass In vJson.Cheques
            pIU = New InsertUpdate("TransCustomerPaymentChecks" & If(vJson.SaveAsDraftProper, "Draft", ""))
            pIU("CustomerPaymentID") = ReadyString(vPaymentID)
            If String.IsNullOrWhiteSpace(pCheque.BankID) = False Then
                pIU("BankID") = ReadyString(pCheque.BankID)
            End If
            If String.IsNullOrWhiteSpace(pCheque.BankID2) = False Then
                pIU("BankID2") = ReadyString(pCheque.BankID2)
            End If
            If String.IsNullOrWhiteSpace(pCheque.BankName) = False Then
                pIU("BankName") = ReadyString(pCheque.BankName)
            End If
            If String.IsNullOrWhiteSpace(pCheque.BankName2) = False Then
                pIU("BankName2") = ReadyString(pCheque.BankName2)
            End If

            If String.IsNullOrWhiteSpace(pCheque.BankBranchID) = False Then
                pIU("BankBranchID") = ReadyString(pCheque.BankBranchID)
            End If

            If String.IsNullOrWhiteSpace(pCheque.BankBranchName) = False Then
                pIU("BankBranchName") = ReadyString(pCheque.BankBranchName)
            End If
            If String.IsNullOrWhiteSpace(pCheque.BankBranchName2) = False Then
                pIU("BankBranchName2") = ReadyString(pCheque.BankBranchName2)
            End If

            If String.IsNullOrWhiteSpace(pCheque.AccountName) = False Then
                pIU("AccountName") = ReadyString(pCheque.AccountName)
            End If
            If String.IsNullOrWhiteSpace(pCheque.AccountName2) = False Then
                pIU("AccountName2") = ReadyString(pCheque.AccountName2)
            End If

            If String.IsNullOrWhiteSpace(pCheque.AccountNumber) = False Then
                pIU("AccountNumber") = ReadyString(pCheque.AccountNumber)
            End If
            If String.IsNullOrWhiteSpace(pCheque.AccountNumber2) = False Then
                pIU("AccountNumber2") = ReadyString(pCheque.AccountNumber2)
            End If

            If String.IsNullOrWhiteSpace(pCheque.CheckNumber) = False Then
                pIU("CheckNumber") = ReadyString(pCheque.CheckNumber)
            End If

            If String.IsNullOrWhiteSpace(pCheque.CheckDate) = False Then
                pIU("CheckDate") = ReadyString(pCheque.CheckDate)
            End If

            If IsNumeric(pCheque.CheckAmount) = False Then
                Throw New ApplicationException("CheckAmount must be numeric.")
            End If
            pIU("CheckAmount") = CDec2(pCheque.CheckAmount)
            Select Case CStr2(pCheque.PaymentType).Trim().ToLower()
                Case "gift-cert"
                    pIU("IsGiftCertificate") = 1

                Case "online-payment"
                    pIU("IsOnlinePayment") = 1

            End Select
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pCheque

        Return True

    End Function

    Private Function SaveCreditCards(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pCreditCard As SavePaymentJson.CreditCardClass In vJson.CreditCards
            pIU = New InsertUpdate("TransCustomerPaymentCreditCard" & If(vJson.SaveAsDraftProper, "Draft", ""))
            pIU("CustomerPaymentID") = ReadyString(vPaymentID)
            pIU("CreditCardType") = ReadyString(pCreditCard.CreditCardType)
            If String.IsNullOrWhiteSpace(pCreditCard.CreditCardBankID) = False Then
                pIU("CreditCardBankID") = ReadyString(pCreditCard.CreditCardBankID)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.CreditCardBankName) = False Then
                pIU("CreditCardBankName") = ReadyString(pCreditCard.CreditCardBankName)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.BankCountryID) = False Then
                pIU("BankCountryID") = ReadyString(pCreditCard.BankCountryID)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.HolderLastName) = False Then
                pIU("HolderLastName") = ReadyString(pCreditCard.HolderLastName)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.HolderFirstName) = False Then
                pIU("HolderFirstName") = ReadyString(pCreditCard.HolderFirstName)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.HolderMiddleName) = False Then
                pIU("HolderMiddleName") = ReadyString(pCreditCard.HolderMiddleName)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.HolderGenerationalSuffix) = False Then
                pIU("HolderGenerationalSuffix") = ReadyString(pCreditCard.HolderGenerationalSuffix)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.CreditCardNumber) = False Then
                pIU("CreditCardNumber") = ReadyString(pCreditCard.CreditCardNumber)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.CreditCardIssuanceDate) = False Then
                pIU("CreditCardIssuanceDate") = ReadyString(pCreditCard.CreditCardIssuanceDate)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.CreditCardExpirationDate) = False Then
                pIU("CreditCardExpirationDate") = ReadyString(pCreditCard.CreditCardExpirationDate)
            End If

            If String.IsNullOrWhiteSpace(pCreditCard.ApprovalNo) = False Then
                pIU("ApprovalNo") = ReadyString(pCreditCard.ApprovalNo)
            End If

            'If String.IsNullOrWhiteSpace(pCreditCard.) = False Then
            '    pIU("") = ReadyString(pCreditCard.)
            'End If

            pIU("NoOfInstallmentPayments") = CDec2(pCreditCard.NoOfInstallmentPayments)

            If IsNumeric(pCreditCard.CreditCardAmount) = False Then
                Throw New ApplicationException("CreditCardAmount must be numeric.")
            End If
            pIU("CreditCardAmount ") = CDec2(pCreditCard.CreditCardAmount)

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pCreditCard

        Return True

    End Function

    Private Function SaveDebitCards(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pDebitCard As SavePaymentJson.DebitCardClass In vJson.DebitCards
            pIU = New InsertUpdate("TransCustomerPaymentDebitCard" & If(vJson.SaveAsDraftProper, "Draft", ""))
            pIU("ID") = "NEWID()"
            pIU("CustomerPaymentID") = ReadyString(vPaymentID)
            pIU("DebitCardType") = ReadyString(pDebitCard.DebitCardType)
            If String.IsNullOrWhiteSpace(pDebitCard.DebitCardBankID) = False Then
                pIU("DebitCardBankID") = ReadyString(pDebitCard.DebitCardBankID)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.DebitCardBankName) = False Then
                pIU("DebitCardBankName") = ReadyString(pDebitCard.DebitCardBankName)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.BankCountryID) = False Then
                pIU("BankCountryID") = ReadyString(pDebitCard.BankCountryID)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.HolderLastName) = False Then
                pIU("HolderLastName") = ReadyString(pDebitCard.HolderLastName)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.HolderFirstName) = False Then
                pIU("HolderFirstName") = ReadyString(pDebitCard.HolderFirstName)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.HolderMiddleName) = False Then
                pIU("HolderMiddleName") = ReadyString(pDebitCard.HolderMiddleName)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.HolderGenerationalSuffix) = False Then
                pIU("HolderGenerationalSuffix") = ReadyString(pDebitCard.HolderGenerationalSuffix)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.DebitCardNumber) = False Then
                pIU("DebitCardNumber") = ReadyString(pDebitCard.DebitCardNumber)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.DebitCardIssuanceDate) = False Then
                pIU("DebitCardIssuanceDate") = ReadyString(pDebitCard.DebitCardIssuanceDate)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.DebitCardExpirationDate) = False Then
                pIU("DebitCardExpirationDate") = ReadyString(pDebitCard.DebitCardExpirationDate)
            End If

            If String.IsNullOrWhiteSpace(pDebitCard.ApprovalNo) = False Then
                pIU("ApprovalNo") = ReadyString(pDebitCard.ApprovalNo)
            End If

            'If String.IsNullOrWhiteSpace(pCreditCard.) = False Then
            '    pIU("") = ReadyString(pCreditCard.)
            'End If

            pIU("NoOfInstallmentPayments") = CDec2(pDebitCard.NoOfInstallmentPayments)

            If IsNumeric(pDebitCard.DebitCardAmount) = False Then
                Throw New ApplicationException("DebitCardAmount must be numeric.")
            End If
            pIU("DebitCardAmount") = CDec2(pDebitCard.DebitCardAmount)

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pDebitCard

        Return True

    End Function

    Private Function SaveOtherPayments(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pOtherPayments As SavePaymentJson.OtherPaymentClass In vJson.OtherPayments
            pIU = New InsertUpdate("TransCustomerPaymentOther" & If(vJson.SaveAsDraftProper, "Draft", ""))
            pIU("ID") = "NEWID()"
            pIU("CustomerPaymentID") = ReadyString(vPaymentID)
            If String.IsNullOrWhiteSpace(pOtherPayments.PaymentType) Then
                Throw New ApplicationException("PaymentType must not be blank. vbADQZFMCWD7!")
            End If
            pIU("PaymentType") = ReadyString(pOtherPayments.PaymentType)
            If String.IsNullOrWhiteSpace(pOtherPayments.IssuingOrganization) = False Then
                pIU("IssuingOrganization") = ReadyString(pOtherPayments.IssuingOrganization)
            End If

            If String.IsNullOrWhiteSpace(pOtherPayments.CardNumber) = False Then
                pIU("CardNumber") = ReadyString(pOtherPayments.CardNumber)
            End If

            If String.IsNullOrWhiteSpace(pOtherPayments.CardHolderName) = False Then
                pIU("CardHolderName") = ReadyString(pOtherPayments.CardHolderName)
            End If

            If pOtherPayments.CardIssuanceDate IsNot Nothing Then
                pIU("CardIssuanceDate") = ReadyString(pOtherPayments.CardIssuanceDate)
            End If

            If pOtherPayments.CardExpirationDate IsNot Nothing Then
                pIU("CardExpirationDate") = ReadyString(pOtherPayments.CardExpirationDate)
            End If

            If String.IsNullOrWhiteSpace(pOtherPayments.ApprovalNo) = False Then
                pIU("ApprovalNo") = ReadyString(pOtherPayments.ApprovalNo)
            End If

            If String.IsNullOrWhiteSpace(pOtherPayments.GrossTaxableAmount) = False Then
                pIU("GrossTaxableAmount") = CDec2(pOtherPayments.GrossTaxableAmount)
            End If

            If String.IsNullOrWhiteSpace(pOtherPayments.VatRate) = False Then
                pIU("VatRate") = CDec2(pOtherPayments.VatRate)
            End If

            If String.IsNullOrWhiteSpace(pOtherPayments.TaxableAmount) = False Then
                pIU("TaxableAmount") = CDec2(pOtherPayments.TaxableAmount)
            End If

            If String.IsNullOrWhiteSpace(pOtherPayments.WitholdingTaxRate) = False Then
                pIU("WitholdingTaxRate") = CDec2(pOtherPayments.WitholdingTaxRate)
            End If

            If IsNumeric(pOtherPayments.Amount) = False Then
                Throw New ApplicationException("Other Payment Amount must be numeric.")
            End If
            pIU("Amount") = CDec2(pOtherPayments.Amount)

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pOtherPayments

        Return True

    End Function

    Private Function SaveUnappliedCreditToUse(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        If vJson.UnappliedCreditToUse Is Nothing Then
            Return True
        End If

        For Each pUseUnappliedCredit As SavePaymentJson.UseUnappliedCreditClass In vJson.UnappliedCreditToUse
            pIU = New InsertUpdate("TransCustomerPaymentAppliedCredits")
            pIU("ID") = "NEWID()"
            pIU("SourcePaymentID") = ReadyString(pUseUnappliedCredit.SourcePaymentID)
            pIU("DestinationPaymentID") = ReadyString(vPaymentID)
            pIU("Amount") = CDec2(pUseUnappliedCredit.AmountToUse)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pUseUnappliedCredit

        Return True

    End Function

    Function VerifySrvPayments(vJson As SavePaymentJson, ByRef oError As String) As Boolean

        Dim pRemainingValue As Decimal

        oError = Nothing
        For Each pSrvPayment As SavePaymentJson.SrvPaymentClass In vJson.SrvPayments
            pRemainingValue = mParent.TransSalesReturnLedger.GetRemainingValue(pSrvPayment.SrvNumber, pSrvPayment.Pin, pSrvPayment.MemoID)
            If pSrvPayment.MemoID Is Nothing Then
                oError = "SRV# " & pSrvPayment.SrvNumber & " does not exist or wrong PIN."
                Return False
            End If
            If pSrvPayment.ConsumedAmount > pRemainingValue Then
                oError = "The remaining value (" & Format(pRemainingValue, cGlobal.FormatCurrency) & ") of SRV# " & pSrvPayment.SrvNumber & " is lesser than the value you are trying to pay with (" & Format(pSrvPayment.ConsumedAmount, cGlobal.FormatCurrency) & ")."
                Return False
            End If
        Next pSrvPayment

        Return True

    End Function

    Function SaveSrvPayments(vPaymentID As String, vJson As SavePaymentJson, ByRef oError As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pSrvPayment As SavePaymentJson.SrvPaymentClass In vJson.SrvPayments
            pIU = New InsertUpdate("TransCustomerPaymentSRV")
            pIU("CustomerPaymentID") = ReadyString(vPaymentID)
            pIU("MemoID") = ReadyString(pSrvPayment.MemoID)
            pIU("ConsumedAmount") = pSrvPayment.ConsumedAmount
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            If mParent.TransSalesReturnLedger.Consume(pSrvPayment.MemoID, vPaymentID, pSrvPayment.ConsumedAmount, "todo (not needed now) vbCSQCZ4MHLV!", oError) = False Then
                Return False
            End If
        Next pSrvPayment

        Return True

    End Function

    Public Class SavePaymentJson
        Public Cash As Decimal
        Public CheckAmount As Decimal ' set in server side
        Public CreditCardAmount As Decimal ' set in server side
        Public DebitCardAmount As Decimal ' set in server side
        Public OtherTypeAmount As Decimal ' set in server side
        Public SrvPaymentAmount As Decimal ' set in server side
        ''' <summary>
        ''' total amount paid to invoices
        ''' </summary>
        ''' <remarks></remarks>
        Public ConsumedAmount As Decimal ' set in server side
        Public CurrentPaymentReceived As Decimal?
        Public UserID As String
        Public FromEcommerce As String ' boolean
        Public CustomerID As String
        Public Invoices As List(Of InvoicesClass)
        Public Cheques As List(Of ChequeClass)
        Public CreditCards As List(Of CreditCardClass)
        Public DebitCards As List(Of DebitCardClass)
        Public OtherPayments As List(Of OtherPaymentClass)
        Public SrvPayments As List(Of SrvPaymentClass)
        Public UnappliedCreditToUse As List(Of UseUnappliedCreditClass)
        Public FowstID As String
        Public DeviceID As String
        Public ManualReceiptNumber As String
        Public ConsumedUnappliedPayment As String
        Public SaveAsDraft As String ' boolean
        Public OrgID As String
        Public CustomerEmailAddresses As String
        Public IsPos As String ' boolean
        Public AllowOverMaxOverPayment As String ' boolean
        Public RsdDateTime As String ' date

        Public Class InvoicesClass
            Public ID As String
            Public AmountPaid As Decimal?
        End Class

        Public ReadOnly Property ConsumedUnappliedPaymentProper As Decimal
            Get

                Return CDec2(ConsumedUnappliedPayment)

            End Get
        End Property

        Public ReadOnly Property SaveAsDraftProper As Boolean
            Get

                Return CBool2(SaveAsDraft) OrElse CBool2(FromEcommerce)

            End Get
        End Property

        Public Class ChequeClass
            Public BankID As String
            Public BankName As String
            Public BankBranchID As String
            Public BankBranchName As String
            Public AccountName As String
            Public AccountNumber As String

            Public BankID2 As String
            Public BankName2 As String
            Public BankBranchName2 As String
            Public AccountName2 As String
            Public AccountNumber2 As String

            Public CheckNumber As String
            Public CheckDate As Date
            Public CheckAmount As Decimal
            Public PaymentType As String
        End Class

        Public Class CreditCardClass
            Public CreditCardType As String
            Public CreditCardBankID As String
            Public CreditCardBankName As String
            Public BankCountryID As String
            Public HolderLastName As String
            Public HolderFirstName As String
            Public HolderMiddleName As String
            Public HolderGenerationalSuffix As String
            Public CreditCardNumber As String
            Public CreditCardIssuanceDate As Date?
            Public CreditCardExpirationDate As Date?
            Public ApprovalNo As String
            Public NoOfInstallmentPayments As Integer?
            Public CreditCardAmount As Decimal
        End Class

        Public Class DebitCardClass
            Public DebitCardType As String
            Public DebitCardBankID As String
            Public DebitCardBankName As String
            Public BankCountryID As String
            Public HolderLastName As String
            Public HolderFirstName As String
            Public HolderMiddleName As String
            Public HolderGenerationalSuffix As String
            Public DebitCardNumber As String
            Public DebitCardIssuanceDate As Date?
            Public DebitCardExpirationDate As Date?
            Public ApprovalNo As String
            Public NoOfInstallmentPayments As String
            Public DebitCardSurcharge As String
            Public DebitCardAmount As Decimal
        End Class

        Public Class OtherPaymentClass
            Public PaymentType As String
            Public Amount As Decimal
            Public IssuingOrganization As String
            Public CardNumber As String
            Public CardHolderName As String
            Public CardIssuanceDate As Date?
            Public CardExpirationDate As Date?
            Public ApprovalNo As String

            ' witholding tax
            Public GrossTaxableAmount As String
            Public VatRate As String
            Public TaxableAmount As String
            Public WitholdingTaxRate As String
        End Class

        Public Class SrvPaymentClass
            Public SrvNumber As Integer
            Public Pin As String
            Public ConsumedAmount As Decimal
            Public MemoID As String ' set in server side
        End Class

        Public Class UseUnappliedCreditClass
            Public AmountToUse As String ' decimal
            Public SourcePaymentID As String ' guid
        End Class
    End Class ' END SavePaymentJson

    Public Function SaveChecksAmount(vID As String, vSaveAsDraft As Boolean) As Integer

        Dim pDrPayment As DataRow
        Dim pDtChecks As DataTable
        Dim pDtInvoices As DataTable
        Dim pTotalCheckAmount As Decimal
        Dim pTotalAmountPaid As Decimal
        Dim pUnappliedPayment As Decimal
        Dim pMultiplier As Decimal
        Dim pDatas As List(Of SaveChecksAmountData)
        Dim pData As SaveChecksAmountData

        pDrPayment = GetByID(vID, vSaveAsDraft)
        pDtChecks = GetChecks(vID, vSaveAsDraft)
        pDtInvoices = GetInvoices(vID, vSaveAsDraft)

        pTotalCheckAmount = CDec2(pDtChecks.Compute("SUM(CheckAmount)", ""))
        pDatas = New List(Of SaveChecksAmountData)

        If pTotalCheckAmount <> 0 Then
            pTotalAmountPaid = CDec2(pDtInvoices.Compute("SUM(AmountPaid)", ""))
            pUnappliedPayment = CDec2(pDrPayment("UnappliedPayment"))

            pMultiplier = pTotalCheckAmount / (pTotalAmountPaid + pUnappliedPayment)

            For Each pDrInvoice As DataRow In pDtInvoices.Rows
                For Each pDrCheck As DataRow In pDtChecks.Rows
                    pData = New SaveChecksAmountData
                    pData.CheckID = CStr2(pDrCheck("ID"))
                    pData.InvoiceID = CStr2(pDrInvoice("InvoiceID"))
                    pData.AmountPaid = CDec2(pDrCheck("CheckAmount")) * pDrInvoice("AmountPaid") / (pTotalAmountPaid + pUnappliedPayment)
                    pDatas.Add(pData)
                Next pDrCheck
            Next pDrInvoice

            If pUnappliedPayment > 0 Then
                For Each pDrCheck As DataRow In pDtChecks.Rows
                    pData = New SaveChecksAmountData
                    pData.CheckID = CStr2(pDrCheck("ID"))
                    pData.Overpayment = CDec2(pDrCheck("CheckAmount")) * pUnappliedPayment / (pTotalAmountPaid + pUnappliedPayment)
                    pDatas.Add(pData)
                Next pDrCheck
            End If
        End If

        SaveChecksAmountActual(vID, pDatas, vSaveAsDraft)

        Return pDatas.Count

    End Function

    Private Function SaveChecksAmountActual(vID As String, vData As List(Of SaveChecksAmountData), vSaveAsDraft As Boolean)

        Dim pIU As InsertUpdate
        Dim pSql As String

        mProData.BeginTransaction()
        pSql = "DELETE FROM TransCustomerPaymentCheckAmount" & If(vSaveAsDraft, "Draft", "") & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vID)
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        For Each pData As SaveChecksAmountData In vData
            pIU = New InsertUpdate("TransCustomerPaymentCheckAmount" & If(vSaveAsDraft, "Draft", ""))
            pIU("ID") = "NEWID()"
            pIU("CustomerPaymentID") = ReadyString(vID)
            pIU("CustomerPaymentCheckID") = ReadyString(pData.CheckID)
            If pData.InvoiceID Is Nothing Then
                pIU("Overpayment") = pData.Overpayment
            Else
                pIU("InvoiceID") = ReadyString(pData.InvoiceID)
                pIU("AmountPaid") = pData.AmountPaid
            End If
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pData
        mProData.CommitTransaction()

    End Function

    Private Class SaveChecksAmountData
        Public CheckID As String
        Public InvoiceID As String
        Public AmountPaid As Decimal
        Public Overpayment As Decimal
    End Class

    Private Function GetChecks(vID As String, vFromDraft As Boolean) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransCustomerPaymentChecks.ID" & vbCrLf &
            ",TransCustomerPaymentChecks.CheckAmount" & vbCrLf &
            ",TransCustomerPaymentChecks.CheckNumber" & vbCrLf &
            ",TransCustomerPaymentChecks.BankID" & vbCrLf &
            ",TransCustomerPaymentChecks.BankName" & vbCrLf &
            ",TransCustomerPaymentChecks.BankBranchID" & vbCrLf &
            ",TransCustomerPaymentChecks.BankBranchName" & vbCrLf &
            ",TransCustomerPaymentChecks.AccountName" & vbCrLf &
            ",TransCustomerPaymentChecks.AccountNumber" & vbCrLf &
            ",TransCustomerPaymentChecks.CheckDate" & vbCrLf &
            " FROM TransCustomerPaymentChecks" & If(vFromDraft, "Draft AS TransCustomerPaymentChecks", "") & vbCrLf &
            " WHERE TransCustomerPaymentChecks.CustomerPaymentID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetInvoices(vID As String, vFromDraft As Boolean) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            " ,CacheInvoices.InvoiceNumber" & vbCrLf &
            " ,CacheInvoices.NetInvoiceAmount" & vbCrLf &
            " ,TransCustomerPaymentInvoices.AmountPaid" & vbCrLf &
            " FROM TransCustomerPaymentInvoices" & If(vFromDraft, "Draft AS TransCustomerPaymentInvoices", "") & vbCrLf &
            " LEFT JOIN CacheInvoices ON CacheInvoices.ID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            " WHERE TransCustomerPaymentInvoices.CustomerPaymentID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetByID(vID As String, vFromDraft As Boolean) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT UnappliedPayment" & vbCrLf &
            ",TransCustomerPayment" & If(vFromDraft, "Draft", "") & ".CustomerID" & vbCrLf &
            " FROM TransCustomerPayment" & If(vFromDraft, "Draft", "") & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetChequeDistribution(Optional vID As String = Nothing,
        Optional vCustomerPaymentCheckID As String = Nothing,
        Optional vCustomerPaymentCheckAmountID As String = Nothing,
        Optional vFromDraft As Boolean = False
        ) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " -- payment distribution" & vbCrLf &
            "  SELECT TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".CustomerPaymentID" & vbCrLf &
            ",TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".BankName" & vbCrLf &
            "  , TransCustomerPaymentInvoices" & If(vFromDraft, "Draft", "") & ".ID" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".InvoiceID" & vbCrLf &
            " ,TransInvoice.CustomerID" & vbCrLf &
            " ,TransInvoice.TotalAmount AS GrossInvoiceAmount" & vbCrLf &
            " ,TransInvoice.InvoiceDate" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            "       ,CASE" & vbCrLf &
            "       	WHEN ISNULL(TransInvoice.InvoiceNumber, 0) > 0 THEN 'SI'" & vbCrLf &
            "          WHEN ISNULL(TransInvoice.SalesOrderNo, 0) > 0 THEN 'SO'" & vbCrLf &
            "          WHEN ISNULL(TransInvoice.PurchaseOrderNo, 0) > 0 THEN 'PO'" & vbCrLf &
            "          ELSE ''" & vbCrLf &
            "          END + '-' +" & vbCrLf &
            "       	+ CONVERT(varchar, MobizLibraryOrganizationFOWST.FowstNo) + '-'" & vbCrLf &
            "          + CONVERT(varchar, LibraryDevice.DeviceNo) + '-'" & vbCrLf &
            "          + CONVERT(varchar," & vbCrLf &
            "  CASE" & vbCrLf &
            "        	 WHEN ISNULL(TransInvoice.InvoiceNumber, 0) > 0 THEN TransInvoice.InvoiceNumber" & vbCrLf &
            "           WHEN ISNULL(TransInvoice.SalesOrderNo, 0) > 0 THEN TransInvoice.SalesOrderNo" & vbCrLf &
            "           WHEN ISNULL(TransInvoice.PurchaseOrderNo, 0) > 0 THEN TransInvoice.PurchaseOrderNo" & vbCrLf &
            "           ELSE 0" & vbCrLf &
            "           END" & vbCrLf &
            " ) AS InvoiceNumber" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".ID AS CustomerPaymentCheckAmountID" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".AmountPaid" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".Overpayment" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".CustomerPaymentCheckID" & vbCrLf &
            " ,TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".CheckAmount" & vbCrLf &
            " ,TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".CheckNumber" & vbCrLf &
            " ,TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".CheckDate" & vbCrLf &
            " FROM TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & "" & vbCrLf &
            " JOIN TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & " ON TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".ID = TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".CustomerPaymentCheckID" & vbCrLf &
            " LEFT JOIN TransCustomerPaymentInvoices" & If(vFromDraft, "Draft", "") & " ON TransCustomerPaymentInvoices" & If(vFromDraft, "Draft", "") & ".InvoiceID = TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".InvoiceID" & vbCrLf &
            " AND TransCustomerPaymentInvoices" & If(vFromDraft, "Draft", "") & ".CustomerPaymentID = TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".CustomerPaymentID" & vbCrLf &
            " LEFT JOIN TransInvoice ON TransInvoice.ID = TransCustomerPaymentInvoices" & If(vFromDraft, "Draft", "") & ".InvoiceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf
        If String.IsNullOrWhiteSpace(vCustomerPaymentCheckID) = False Then
            pSql &= " WHERE TransCustomerPaymentChecks" & If(vFromDraft, "Draft", "") & ".ID = " & ReadyString(vCustomerPaymentCheckID)
        ElseIf String.IsNullOrWhiteSpace(vCustomerPaymentCheckAmountID) = False Then
            pSql &= " WHERE TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".ID = " & ReadyString(vCustomerPaymentCheckAmountID)
        Else
            pSql &= " WHERE TransCustomerPaymentCheckAmount" & If(vFromDraft, "Draft", "") & ".CustomerPaymentID = " & ReadyString(vID)
        End If
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ' commented on 2021_06_24__08_17_08_3462
    'Public Function GetCheckInvoices(vCustomerPaymentCheckID As String) As DataTable

    '    Dim pDT As DataTable
    '    Dim pSql As String

    '    pSql = " SELECT TransCustomerPaymentCheckAmount.AmountPaid" & vbCrLf &
    '        " ,TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
    '        " ,TransCustomerPaymentCheckAmount.CustomerPaymentCheckID" & vbCrLf &
    '        " FROM TransCustomerPaymentCheckAmount" & vbCrLf &
    '        " WHERE TransCustomerPaymentCheckAmount.CustomerPaymentCheckID = " & ReadyString(vCustomerPaymentCheckID)
    '    pDT = mProData.Execute(pSql)

    '    Return pDT

    'End Function

    Public Function FixNumbers() As Integer

        Dim pSql As String
        Dim pDT As DataTable
        Dim pFowstID As String
        Dim pDeviceID As String
        Dim pIU As InsertUpdate
        Dim pNum As Integer

        pFowstID = mParent.TransInvoice.MostCommonFowstID()
        pDeviceID = mParent.TransInvoice.MostCommonDeviceID()

        pSql = "SELECT ID" & vbCrLf &
            ",ReceiptType" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " WHERE ReceiptNumber = 1" & vbCrLf &
            " AND FowstID IS NULL" & vbCrLf &
            " AND DeviceID IS NULL" & vbCrLf &
            " ORDER BY LastUpdated"
        pDT = mProData.Execute(pSql)

        For Each pDR As DataRow In pDT.Rows
            mProData.BeginTransaction()
            pIU = New InsertUpdate("TransCustomerPayment")
            pIU("FowstID") = ReadyString(pFowstID)
            pIU("DeviceID") = ReadyString(pDeviceID)
            pNum = mParent.LibraryDeviceFowst.GetLastNumber("POSLastORNo", pFowstID, pDeviceID) + 1
            pIU("ReceiptNumber") = pNum
            If CStr2(String.IsNullOrWhiteSpace(pDR("ReceiptType"))) Then
                pIU("ReceiptType") = "'OR'"
            End If
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
            mParent.LibraryDeviceFowst.UpdateLastNumber("POSLastORNo", pFowstID, pDeviceID, pNum, Nothing)
            mProData.CommitTransaction()
        Next pDR

    End Function

    Public Function SaveDeposits(vDeposits As DepositsClass, ByRef oError As String) As String

        Dim pTableName As String
        Dim pFieldName As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pRecordsAffected As Integer
        Dim pValue As Byte
        Dim pDepositGroupIDFieldName As String
        Dim pDepositedByFieldName As String
        Dim pDepositGroupID As String
        Dim pDepositDateFieldName As String

        Try
            oError = Nothing

            Try
                If vDeposits.Verify(oError) = False Then
                    Return Nothing
                End If

                pDepositGroupID = Guid.NewGuid().ToString()
                pValue = vDeposits.Value
                If pValue < 0 _
                    OrElse pValue > 2 Then

                    oError = "Value of " & vDeposits.Value & " is not a valid deposit status. ref: vbRMSKGN3CGN!"
                    Return Nothing
                End If
            Catch ex As Exception
                oError = "Value of " & vDeposits.Value & " is not a valid byte. ref: vbNSCCRWQZ2G!"
                Return Nothing
            End Try

            mProData.BeginTransaction()
            For Each pDeposit As DepositsClass.DepositClass In vDeposits.Data
                Select Case pDeposit.PaymentType
                    Case "Cash"
                        If vDeposits.Mode = "disbursement" Then
                            Throw New ApplicationException("vbJJ487JULPY! Cash deposit in disbursement mode is not yet implemented.")
                        Else
                            pTableName = "TransCustomerPayment"
                            pFieldName = "CashDepositStatus"
                            pDepositGroupIDFieldName = "CashDepositGroupID"
                            pDepositDateFieldName = "CashDepositDate"
                            pDepositedByFieldName = "CashDepositedBy"
                        End If

                    Case "Check"
                        If vDeposits.Mode = "disbursement" Then
                            pTableName = "TransReceivingPaymentChecks"
                            pFieldName = "DepositStatus"
                            pDepositGroupIDFieldName = "DepositGroupID"
                            pDepositDateFieldName = "DepositsDepositDate"
                        Else
                            pTableName = "TransCustomerPaymentChecks"
                            pFieldName = "DepositStatus"
                            pDepositGroupIDFieldName = "DepositGroupID"
                            pDepositDateFieldName = "DepositsDepositDate"
                        End If
                        pDepositedByFieldName = "DepositedBy"

                    Case "Credit Card"
                        pTableName = "TransCustomerPaymentCreditCard"
                        pFieldName = "DepositStatus"
                        pDepositGroupIDFieldName = "DepositGroupID"
                        pDepositDateFieldName = "DepositsDepositDate"
                        pDepositedByFieldName = "DepositedBy"

                    Case "Debit Card"
                        pTableName = "TransCustomerPaymentDebitCard"
                        pFieldName = "DepositStatus"
                        pDepositGroupIDFieldName = "DepositGroupID"
                        pDepositDateFieldName = "DepositsDepositDate"
                        pDepositedByFieldName = "DepositedBy"

                End Select

                pIU = New InsertUpdate(pTableName)
                pIU(pFieldName) = pValue
                pIU(pDepositedByFieldName) = ReadyString(vDeposits.DepositedBy)
                pIU(pDepositGroupIDFieldName) = ReadyString(pDepositGroupID)
                pIU("DepositoryBankID") = ReadyString(vDeposits.DepositoryBankID)
                pIU("DepositoryBankBranch") = ReadyString(vDeposits.DepositoryBankBranch)
                pIU("DepositoryAccountName") = ReadyString(vDeposits.AccountName)
                pIU("DepositoryAccountNumber") = ReadyString(vDeposits.AccountNumber)
                pIU("TimeChecked") = ReadyString(pDeposit.TimeChecked)
                pIU(pDepositDateFieldName) = ReadyString(vDeposits.DepositDateProper)
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDeposit.ID))
                pRecordsAffected = mProData.ExecuteSQL(pSql)
                If pRecordsAffected = 0 Then
                    oError = "No affected record: " & pSql & ". ref: vb3JPBYAE6VQ!"
                    mProData.RollbackTransaction()
                    Return Nothing
                End If
            Next pDeposit
            mProData.CommitTransaction()

            Return pDepositGroupID
        Catch ex As Exception
            oError = ex.Message & "ref: vb2A78BPVALT!"
            mProData.RollbackTransaction(, True)
        End Try

    End Function

    Public Class DepositsClass
        Public Data As List(Of DepositClass)
        Public Value As String
        Public DepositoryBankID As String
        Public DepositoryBankBranch As String
        Public DepositedBy As String
        Public AccountName As String
        Public AccountNumber As String
        Public DepositDate As String
        Public Mode As String ' revenue (default) or disbursement

        Public Function Verify(ByRef oError As String) As Boolean

            oError = Nothing

            If String.IsNullOrWhiteSpace(DepositoryBankID) Then
                oError = "DepositoryBankID is required. ref: vbUUJCEZDPNU!"
                Return False
            End If

            If IsGuid(DepositoryBankID) = False Then
                oError = "DepositoryBankID must be a GUID. ref: vbZ6YWM4DA7A!"
                Return False
            End If

            If String.IsNullOrWhiteSpace(DepositoryBankBranch) Then
                oError = "DepositoryBankBranch is required. ref: vbLE532NSXGG!"
                Return False
            End If

            If String.IsNullOrWhiteSpace(AccountName) Then
                oError = "AccountName is required. ref: vb35KYVFJZXL!"
                Return False
            End If

            If String.IsNullOrWhiteSpace(AccountNumber) Then
                oError = "AccountNumber is required. ref: vbXN757W7NBE!"
                Return False
            End If

            If IsDate(DepositDate) = False Then
                oError = "DepositDate must be a valid date. ref: vbTNZA5Z4D7E!"
                Return False
            End If

            Return True

        End Function

        Public ReadOnly Property DepositDateProper As Date
            Get

                Return CDate(DepositDate)

            End Get
        End Property

        Public Class DepositClass
            Public ID As String
            Public PaymentType As String
            Public TimeChecked As String
        End Class
    End Class

    Public Function GetByManualReceiptNumber(vManualReceiptNumber As String) As DataRow

        Dim pSql = "SELECT TransCustomerPayment.ID" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " WHERE LTRIM(RTRIM(TransCustomerPayment.ManualReceiptNumber)) = " & ReadyString(vManualReceiptNumber.Trim())
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetPaymentIdsByInvoiceID(vInvoiceID As String) As DataTable

        Dim pSql = " SELECT TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            " ,TransCustomerPayment.LastUpdated" & vbCrLf &
            " FROM TransCustomerPaymentInvoices" & vbCrLf &
            " JOIN TransCustomerPayment ON TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            " WHERE TransCustomerPaymentInvoices.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND ISNULL(TransCustomerPayment.IsCancelled, 0) = 0" & vbCrLf &
            " GROUP BY TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            " ,TransCustomerPayment.LastUpdated" & vbCrLf &
            " ORDER BY TransCustomerPayment.LastUpdated DESC"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetPaymentData(vID As String) As DataSet

        Dim pOut As New DataSet
        Dim pOrgSelect As String
        Dim pOrgFrom As String

        BrowserController.ComposeCustomer(pOrgSelect, pOrgFrom)

        Dim pSql = "SELECT TransCustomerPayment.ID" & vbCrLf &
            " ,TransCustomerPayment.CustomerID" & vbCrLf &
            " ," & pOrgSelect & " AS CustomerName" & vbCrLf &
            " ,TransCustomerPayment.CashAmount - TransCustomerPayment.ChargeAmount + TransCustomerPayment.SRVAmount + TransCustomerPayment.CheckPaymentAmount + TransCustomerPayment.CreditCardAmount + TransCustomerPayment.DebitCardAmount + TransCustomerPayment.ChargeAmount + TransCustomerPayment.OtherTypeAmount AS TotalPayment" & vbCrLf &
            " ,TransCustomerPayment.PaymentDate" & vbCrLf &
            " ,TransCustomerPayment.CashAmount" & vbCrLf &
            " ,TransCustomerPayment.CheckPaymentAmount AS ChecksAmount" & vbCrLf &
            " ,TransCustomerPayment.OtherTypeAmount AS OtherAmount" & vbCrLf &
            " ,TransCustomerPayment.UnappliedPayment" & vbCrLf &
            " ,ISNULL(TransCustomerPayment.ManualReceiptNumber, TransCustomerPayment.ReceiptNumber) AS ReceiptNumber" & vbCrLf &
            " ,MobizLibraryOrganization.TinNumber" & vbCrLf &
            ",COALESCE(MobizLibraryOrganization.BusinessIndustryText, MobizLibraryIndustryClass.Class) AS BusinessIndustryDisplay" & vbCrLf &
            ",ISNULL(MobizLibraryOrganization.OfficeAddressBuildingNo, '')" & vbCrLf &
            " + ',' + ISNULL(MobizLibraryOrganization.OfficeAddressStreet, '')" & vbCrLf &
            " + ',' + ISNULL(tblCustomerCity.Name, '')" & vbCrLf &
            " + ',' + ISNULL(tblCustomerCountry.Name, '') AS CustomerAddress" & vbCrLf &
            ",MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName AS EmployeeFullName" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransCustomerPayment.CustomerID" & vbCrLf &
            pOrgFrom & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblCustomerCity ON tblCustomerCity.ID = MobizLibraryOrganization.OfficeAddressCityID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountries tblCustomerCountry ON tblCustomerCountry.ID = MobizLibraryOrganization.OfficeAddressCountryID" & vbCrLf &
            " LEFT JOIN MobizLibraryIndustryClass ON MobizLibraryIndustryClass.ID = MobizLibraryOrganization.IndustryClassID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransCustomerPayment.UserID" & vbCrLf &
            " WHERE TransCustomerPayment.ID = " & ReadyString(vID) & vbCrLf
        Dim pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return Nothing
        End If
        pDT.Columns.Add("TotalDue", GetType(Decimal))
        pDT.Columns.Add("PaidAmountInWords")
        pDT.Columns.Add("Invoices")
        pDT.TableName = "Master"
        pOut.Tables.Add(pDT)

        Dim pDtInvoices = GetPaymentInvoices(vID)
        pDtInvoices.TableName = "Invoices"
        pOut.Tables.Add(pDtInvoices)

        For Each pDR As DataRow In pDT.Rows ' only 1 row is expected
            pDR("TotalDue") = pDtInvoices.Compute("SUM(TotalAmount)", "")
            For Each pDrInvoice As DataRow In pDtInvoices.Rows
                If CStr2(pDR("Invoices")) <> "" Then
                    pDR("Invoices") &= ", "
                End If
                pDR("Invoices") = CStr2(pDR("Invoices")) & pDrInvoice("PrintedInvoiceNumber")
            Next pDrInvoice
        Next pDR

        Dim pDtChecks = GetPaymentChecks(vID)
        pDtChecks.TableName = "Checks"
        pOut.Tables.Add(pDtChecks)

        pDT = GetOtherPayments(vID)
        pDT.TableName = "OtherPayments"
        pOut.Tables.Add(pDT)

        Return pOut

    End Function

    Private Function GetOtherPayments(vPaymentID As String) As DataTable

        Dim pSql = " SELECT TransCustomerPaymentOther.PaymentType" & vbCrLf &
            " ,TransCustomerPaymentOther.Amount" & vbCrLf &
            " ,CustomerPaymentID" & vbCrLf &
            " FROM TransCustomerPaymentOther" & vbCrLf &
            " WHERE TransCustomerPaymentOther.Amount <> 0" & vbCrLf &
            " AND TransCustomerPaymentOther.CustomerPaymentID = " & ReadyString(vPaymentID) & vbCrLf &
            " ORDER BY TransCustomerPaymentOther.PaymentType"
        Dim pDT = mProData.Execute(pSql)

        Return pDT


    End Function

    Public Function OtherPaymentsSummary(vDateFrom As Date, vDateTo As Date, Optional vDeviceID As String = Nothing) As DataTable

        Dim pSql = " SELECT PaymentType" & vbCrLf &
            " ,SUM(Amount) AS Amount" & vbCrLf &
            " FROM (" & vbCrLf &
            "   SELECT TransCustomerPaymentOther.PaymentType" & vbCrLf &
            "   ,TransCustomerPaymentOther.Amount" & vbCrLf &
            "   ,CustomerPaymentID" & vbCrLf &
            "   FROM TransCustomerPaymentOther" & vbCrLf &
            "   WHERE TransCustomerPaymentOther.Amount <> 0" & vbCrLf &
            "   AND TransCustomerPaymentOther.CustomerPaymentID IN (" & vbCrLf &
            "       SELECT TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            "       FROM TransCustomerPaymentInvoices" & vbCrLf &
            "       JOIN TransInvoice ON TransInvoice.ID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            "       AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Format(vDateFrom, cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(vDateTo, cGlobal.FormatDateTimeSql)) & vbCrLf
        If String.IsNullOrWhiteSpace(vDeviceID) = False Then
            pSql &= " AND TransInvoice.DeviceID = " & ReadyString(vDeviceID) & vbCrLf
        End If
        pSql &= "   )" & vbCrLf &
            " ) t" & vbCrLf &
            " GROUP BY PaymentType" & vbCrLf &
            " ORDER BY PaymentType"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetPaymentInvoices(vPaymentID As String) As DataTable

        Dim pSql As String

        pSql = " SELECT TransInvoice.ID" & vbCrLf
        pSql &= ",TransInvoice.TotalAmount" & vbCrLf
        pSql &= ",TransCustomerPaymentInvoices.AmountPaid" & vbCrLf
        pSql &= ",ISNULL(LibraryDocumentControl.DocumentName, 'SI') + '-' + COALESCE(CONVERT(varchar, TransInvoice.FowstWideInvoiceNumber), CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber)) AS PrintedInvoiceNumber" & vbCrLf
        pSql &= " FROM TransInvoice " & vbCrLf
        pSql &= " JOIN MobizLibraryOrganizationFOWST InvFowst ON TransInvoice.FowstID = InvFowst.ID" & vbCrLf
        pSql &= " JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf
        pSql &= " JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID" & vbCrLf
        pSql &= " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf
        pSql &= " WHERE TransCustomerPaymentInvoices.CustomerPaymentID = " & ReadyString(vPaymentID) & vbCrLf
        pSql &= " ORDER BY PrintedInvoiceNumber" & vbCrLf

        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetPaymentChecks(vPaymentID As String) As DataTable

        Dim pSql As String

        pSql = " SELECT TOP 100 PERCENT TransCustomerPaymentChecks.ID" & vbCrLf
        pSql &= ",TransCustomerPaymentChecks.CheckNumber" & vbCrLf
        pSql &= ",ISNULL(MobizLibraryOrganization.CompanyName, TransCustomerPaymentChecks.BankName) AS BankName" & vbCrLf
        pSql &= ",TransCustomerPaymentChecks.CheckAmount" & vbCrLf
        pSql &= ",TransCustomerPaymentChecks.CheckDate" & vbCrLf
        pSql &= " FROM TransCustomerPaymentChecks " & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransCustomerPaymentChecks.BankID" & vbCrLf
        pSql &= " WHERE TransCustomerPaymentChecks.CustomerPaymentID = " & ReadyString(vPaymentID) & vbCrLf
        pSql &= " ORDER BY TransCustomerPaymentChecks.CheckNumber" & vbCrLf

        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Shared mCancelPaymentLock As New Object

    Public Function CancelPayment(vPaymentID As String, ByRef oError As String, vCancelledByID As String) As Boolean

        SyncLock mCancelPaymentLock
            Dim pSql = "SELECT TransCustomerPayment.IsCancelled" & vbCrLf &
                " ,TransCustomerPayment.DateCancelled" & vbCrLf &
                " ,TransCustomerPayment.CancelledByID" & vbCrLf &
                " FROM TransCustomerPayment" & vbCrLf &
                " WHERE ID = " & ReadyString(vPaymentID)
            Dim pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                oError = "Payment record with ID, " & vPaymentID & ", does not exist."
                Return False
            End If

            If CBool2(pDR("IsCancelled")) Then
                oError = "This payment was already cancelled on " & Format(pDR("DateCancelled"), cGlobal.FormatDateTime)
                Return False
            End If

            mProData.BeginTransaction()
            Dim pDsPayment = GetPaymentData(vPaymentID)
            Dim pDT = pDsPayment.Tables("Master")
            If pDT.Rows.Count = 0 Then
                oError = "pDsPayment.Tables(""Master"") has no row. ref: vb235U8N8AS7!"
                Return False
            End If

            '// tag payment as cancelled
            Dim pIU = New InsertUpdate("TransCustomerPayment")
            pIU("IsCancelled") = 1
            pIU("DateCancelled") = "GETDATE()"
            pIU("CancelledByID") = ReadyString(vCancelledByID)
            pSql = pIU.GetUpdate("ID = " & ReadyString(vPaymentID))
            mProData.ExecuteSQL(pSql)

            '// create invoice adjustment for each invoice paid by this payment
            pDT = pDsPayment.Tables("Invoices")
            For Each pDrInvoice As DataRow In pDT.Rows
                Dim pAdjustment As New TransInvoice.SaveAdjustmentJson()
                pAdjustment.InvoiceID = CStr2(pDrInvoice("ID"))
                pAdjustment.Amount = pDrInvoice("AmountPaid")
                pAdjustment.UserID = vCancelledByID
                pAdjustment.IsNew = True
                pAdjustment.ReasonID = mParent.LibraryAccountAdjustments.AddInvalidPaymentDebitIfNeeded()
                mParent.TransInvoice.SaveAdjustment(pAdjustment, oError)
                If String.IsNullOrWhiteSpace(oError) = False Then
                    mProData.RollbackTransaction()
                    Return False
                End If
                mParent.TransInvoice.CacheInvoicesWithCredit(pAdjustment.InvoiceID)
            Next

            '// tag cheques as returned
            pDT = pDsPayment.Tables("Checks")
            For Each pDrCheque In pDT.Rows
                pIU = New InsertUpdate("TransCustomerPaymentChecks")
                pIU("DepositStatus") = 2
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDrCheque("ID")))
                mProData.ExecuteSQL(pSql)
            Next

            mProData.CommitTransaction()

            Return True
        End SyncLock

    End Function

    Public Function GetDraftPaymentDataSet(vPaymentID As String) As DataSet

        Dim pOut As New DataSet

        Dim pSql = "SELECT TransCustomerPaymentDraft.UserID" & vbCrLf &
            ",TransCustomerPaymentDraft.CustomerID" & vbCrLf &
            ",TransCustomerPaymentDraft.CashAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.ConsumedUnappliedPayment" & vbCrLf &
            ",TransCustomerPaymentDraft.CheckPaymentAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.CreditCardAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.DebitCardAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.OtherTypeAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.UnappliedPayment" & vbCrLf &
            ",TransCustomerPaymentDraft.PaymentDate" & vbCrLf &
            " FROM TransCustomerPaymentDraft" & vbCrLf &
            " WHERE TransCustomerPaymentDraft.ID = " & ReadyString(vPaymentID) & vbCrLf
        Dim pDT = mProData.Execute(pSql)

        If pDT.Rows.Count = 0 Then
            Return Nothing
        End If
        pDT.TableName = "Payment"
        pOut.Tables.Add(pDT)

        pDT = GetInvoices(vPaymentID, True)
        pDT.TableName = "Invoices"
        pOut.Tables.Add(pDT)

        pDT = GetChecks(vPaymentID, True)
        pDT.TableName = "Cheques"
        pOut.Tables.Add(pDT)

        Return pOut

    End Function

    Public Function GetDraftPayment(vPaymentID As String) As SavePaymentJson

        Dim pSql = "SELECT TransCustomerPaymentDraft.UserID" & vbCrLf &
            ",TransCustomerPaymentDraft.CustomerID" & vbCrLf &
            ",TransCustomerPaymentDraft.CashAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.ConsumedUnappliedPayment" & vbCrLf &
            ",TransCustomerPaymentDraft.CheckPaymentAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.CreditCardAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.DebitCardAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.OtherTypeAmount" & vbCrLf &
            ",TransCustomerPaymentDraft.UnappliedPayment" & vbCrLf &
            " FROM TransCustomerPaymentDraft" & vbCrLf &
            " WHERE TransCustomerPaymentDraft.ID = " & ReadyString(vPaymentID) & vbCrLf
        Dim pDR = mProData.ExecuteRow(pSql)

        If pDR Is Nothing Then
            Return Nothing
        End If

        Dim pOut As New SavePaymentJson
        pOut.UserID = CStr2(pDR("UserID"))
        pOut.CustomerID = CStr2(pDR("CustomerID"))
        pOut.Cash = CDec2(pDR("CashAmount"))
        pOut.ConsumedUnappliedPayment = CDec2(pDR("ConsumedUnappliedPayment"))
        pOut.CheckAmount = CDec2(pDR("CheckPaymentAmount"))
        pOut.CreditCardAmount = CDec2(pDR("CreditCardAmount"))
        pOut.DebitCardAmount = CDec2(pDR("DebitCardAmount"))

        Dim pDtInvoices = GetInvoices(vPaymentID, True)
        pOut.Invoices = New List(Of SavePaymentJson.InvoicesClass)
        For Each pDrInvoice As DataRow In pDtInvoices.Rows
            Dim pInvoice As New SavePaymentJson.InvoicesClass
            pInvoice.ID = ReadyGuid(pDrInvoice("InvoiceID"))
            pInvoice.AmountPaid = CDec2(pDrInvoice("AmountPaid"))
            pOut.Invoices.Add(pInvoice)
        Next

        Dim pDtCheques = GetChecks(vPaymentID, True)
        pOut.Cheques = New List(Of SavePaymentJson.ChequeClass)
        For Each pDrCheque In pDtCheques.Rows
            Dim pCheque = New SavePaymentJson.ChequeClass
            pCheque.BankID = CStr2(pDrCheque("BankID"))
            pCheque.BankName = CStr2(pDrCheque("BankName"))
            pCheque.BankBranchID = CStr2(pDrCheque("BankBranchID"))
            pCheque.BankBranchName = CStr2(pDrCheque("BankBranchName"))
            pCheque.AccountName = CStr2(pDrCheque("AccountName"))
            pCheque.AccountNumber = CStr2(pDrCheque("AccountNumber"))
            pCheque.CheckNumber = CStr2(pDrCheque("CheckNumber"))
            pCheque.CheckDate = CStr2(pDrCheque("CheckDate"))
            pCheque.CheckAmount = CDec2(pDrCheque("CheckAmount"))
            pOut.Cheques.Add(pCheque)
        Next

        Return pOut

    End Function

    Public Function CreatePaymentPdf(vPaymentID As String, vFromDraft As Boolean) As String

        Dim pReport As ProReport
        Dim pDtInvoices As DataTable
        Dim pFilename As String
        Dim pBaseFilename As String
        Dim pHeaderInfo As LibraryOrganization.HeaderInfo

        pReport = New ProReport()
        pReport.AutoFillHeader = False
        pReport.SourceFilename = "AcknowledgementReceipt"
        Dim pDT = GetChequeDistribution(vPaymentID, , , vFromDraft)
        pReport.AddTable("ChequesDistribution", pDT)

        Dim pData = GetDraftPaymentDataSet(vPaymentID)
        pReport.AddTable("Payment", pData.Tables("Payment"))
        pReport.AddTable("Cheques", pData.Tables("Cheques"))

        pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
        pReport.Variables("CompanyName") = pHeaderInfo.CompanyName
        pReport.Variables("CompanyAddress") = pHeaderInfo.CompanyAddress
        pReport.Variables("CompanyContactInfos") = pHeaderInfo.ContactInfos

        Dim pDR = GetByID(vPaymentID, True)
        If pDR Is Nothing Then
            Throw New ApplicationException("Draft payment, " & vPaymentID & ", does not exist")
        End If
        pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(CStr2(pDR("CustomerID")))
        pReport.Variables("CustomerName") = pHeaderInfo.CompanyName
        pReport.Variables("CustomerAddress") = pHeaderInfo.CompanyAddress
        pReport.Variables("CustomerContactInfos") = pHeaderInfo.ContactInfos

#If DEBUG Then
        ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "temp\Stimulsoft XML Data.xml"
        pReport.ExportXmlFile()
#End If
        pBaseFilename = "eSOA_" & GenerateString(20) & ".pdf"
        pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename
        pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

        Return pBaseFilename

    End Function

    Function UpdateZReadCache(mInvoiceID As String) As Boolean
        Dim ZReadDate As String
        Dim pSql As String
        'Dim InvoiceDate As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim IsNew As Boolean = False
        Dim dtCurrZRead As DataTable
        Dim dtPrevZRead As DataTable
        Dim dtInvoice As DataTable

        Dim pFOWSTID As String
        Dim pDeviceID As String
        Dim pTransDate As String
        Dim pTransInvoiceNo As String
        Dim dtPOSTransaction As DataTable

        Dim pBusiness As New Business(ProData.CreateFromConnectionInfo(
                        vServer:=Common.ConnectionInfo.Server,
                        vDatabase:="MobizWebPOSCache",
                        vUsername:=Common.ConnectionInfo.Username,
                        vPassword:=Common.ConnectionInfo.Password,
                        vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
                    ))

        Using mReports = pBusiness.ProData

            'mParent.ProData = mReports

            pSql = "Select * from TransInvoice where ID = " & ReadyString(mInvoiceID)

            dtInvoice = mParent.ProData.Execute(pSql)

            If dtInvoice.Rows.Count > 0 Then
                pDeviceID = CStr2(dtInvoice.Rows(0).Item("DeviceID"))
                pFOWSTID = CStr2(dtInvoice.Rows(0).Item("FowstID"))
                pTransDate = CStr2(dtInvoice.Rows(0).Item("InvoiceDate"))
                pTransInvoiceNo = CInt2(dtInvoice.Rows(0).Item("InvoiceNumber"))
            Else
                Return False
            End If

            'GetCurrentZRead(pTransDate, pDeviceID, pFOWSTID)

            pSql = "Select * FROM TransZReadCache " & vbCrLf &
                " WHERE convert(date, SalesDate) = " & ReadyString(pTransDate) & vbCrLf &
                " AND FOWSTID = " & ReadyString(pFOWSTID) & vbCrLf &
                " AND DeviceID = " & ReadyString(pDeviceID)

            dtCurrZRead = mReports.Execute(pSql)

            Dim pImportDataClass As New ImportDataClass(mParent)

            dtPOSTransaction = pImportDataClass.GetPOSTransaction(mInvoiceID, pTransDate, pDeviceID, pFOWSTID)

            If dtCurrZRead.Rows.Count = 0 Then
                IsNew = True

                'GetPreviousZRead(pTransDate, pDeviceID, pFOWSTID)

                pSql = "Select top 1 * FROM TransZReadCache " & vbCrLf &
                " WHERE convert(date, SalesDate) < " & ReadyString(pTransDate) & vbCrLf &
                " AND FOWSTID = " & ReadyString(pFOWSTID) & vbCrLf &
                " AND DeviceID = " & ReadyString(pDeviceID) & vbCrLf &
                " Order by SalesDate desc "

                dtPrevZRead = mReports.Execute(pSql)

                If dtPrevZRead.Rows.Count = 0 Then
                    'Get ZRead Data from old query
                    Dim ReportPOS As New ReportPOS
                    Dim dtTempPrevZRead As DataTable

                    dtPrevZRead = ReportPOS.getSalesZReportSummary("By Date", CStr2(CDate(pTransDate).AddDays(-1)), pFOWSTID, pDeviceID, mParent.ProData)

                    If dtPrevZRead.Rows.Count = 0 Then
                        'dtTempPrevZRead.DefaultView.RowFilter = "CurTermNum=" & CStr2(dtPOSTransaction.Rows(0).Item("CurTermNum"))

                        'dtPrevZRead = dtTempPrevZRead.DefaultView(0).DataView.ToTable
                        'dtPrevZRead = dtTempPrevZRead.DesignMode
                        'Else
                        Exit Function
                    End If

                End If
            End If

            Dim p As New InsertUpdate("TransZReadCache")

            If IsNew Then
                p("DeviceID") = ReadyString(pDeviceID)
                p("FOWSTID") = ReadyString(pFOWSTID)

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

                p("SalesDate") = ReadyString(pTransDate)
                p("PresentationCurrGrossSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
                p("CurGrossSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
                p("CurTotalDiscount") = CDec2(dtPOSTransaction.Rows(0).Item("CurTotalDiscount"))
                p("CurSCDiscount") = CDec2(dtPOSTransaction.Rows(0).Item("CurSCDiscount"))
                p("CurTotalVATSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVATSales"))
                p("CurTotalVAT") = CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVAT"))
                p("CurVATExempt") = CDec2(dtPOSTransaction.Rows(0).Item("CurVATExempt"))
                p("CurLocalTax") = CDec2(dtPOSTransaction.Rows(0).Item("CurLocalTax"))
                p("CurServiceCharge") = CDec2(dtPOSTransaction.Rows(0).Item("CurServiceCharge"))
                p("CurNoTAXSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurNoTAXSales"))
                p("CurVATExemptSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurVATExemptSales"))
                p("CurZeroRatedVATSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurZeroRatedVATSales"))
                p("CurOthers") = CDec2(dtPOSTransaction.Rows(0).Item("CurOthers"))
                p("CashAmount") = CDec2(dtPOSTransaction.Rows(0).Item("CashAmount"))
                p("DebitCardAmount") = CDec2(dtPOSTransaction.Rows(0).Item("DebitCardAmount"))
                p("CreditCardAmount") = CDec2(dtPOSTransaction.Rows(0).Item("CreditCardAmount"))
                p("SRVAmount") = CDec2(dtPOSTransaction.Rows(0).Item("SRVAmount"))
                p("SMACAmount") = CDec2(dtPOSTransaction.Rows(0).Item("SMACAmount"))
                p("CheckPaymentAmount") = CDec2(dtPOSTransaction.Rows(0).Item("CheckPaymentAmount"))
                p("OtherTypeAmount") = CDec2(dtPOSTransaction.Rows(0).Item("OtherTypeAmount"))
                p("OnCreditAmount") = CDec2(dtPOSTransaction.Rows(0).Item("OnCreditAmount"))
                p("ChargeAmount") = CDec2(dtPOSTransaction.Rows(0).Item("ChargeAmount"))
                p("EPlusAmount") = CDec2(dtPOSTransaction.Rows(0).Item("EPlusAmount"))
                p("VoucherAmount") = CDec2(dtPOSTransaction.Rows(0).Item("VoucherAmount"))
                p("CurBegInvoiceNo") = pTransInvoiceNo
                p("CurEndInvoiceNo") = pTransInvoiceNo
                p("CurTransCountInv") = 1 'CDec2(dtPrevZRead.Rows(0).Item("CurTransCountInv") + 1)
                p("CurTransInvItemCount") = CDec2(dtPOSTransaction.Rows(0).Item("CurTransInvItemCount")) '+ CDec2(dtPrevZRead.Rows(0).Item("CurTransInvItemCount"))
                p("CurTransCountMemoReturnExchange") = 0
                p("CurSalesReturnExchangeItemCount") = 0
                p("CurTransCountMemoRefund") = 0
                p("CurSalesRefundItemCount") = 0
                p("CurTermNum") = CDec2(dtPrevZRead.Rows(0).Item("CurTermNum"))
                p("FowstNo") = CDec2(dtPrevZRead.Rows(0).Item("FowstNo"))
            Else
                p("PresentationCurrGrossSales") = CDec2(dtCurrZRead.Rows(0).Item("PresentationCurrGrossSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
                p("CurGrossSales") = CDec2(dtCurrZRead.Rows(0).Item("CurGrossSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
                p("CurTotalDiscount") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalDiscount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTotalDiscount"))
                p("CurSCDiscount") = CDec2(dtCurrZRead.Rows(0).Item("CurSCDiscount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurSCDiscount"))
                p("CurTotalVATSales") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalVATSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVATSales"))
                p("CurTotalVAT") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalVAT")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVAT"))
                p("CurVATExempt") = CDec2(dtCurrZRead.Rows(0).Item("CurVATExempt")) + CDec2(dtPOSTransaction.Rows(0).Item("CurVATExempt"))
                p("CurLocalTax") = CDec2(dtCurrZRead.Rows(0).Item("CurLocalTax")) + CDec2(dtPOSTransaction.Rows(0).Item("CurLocalTax"))
                p("CurServiceCharge") = CDec2(dtCurrZRead.Rows(0).Item("CurServiceCharge")) + CDec2(dtPOSTransaction.Rows(0).Item("CurServiceCharge"))
                p("CurNoTAXSales") = CDec2(dtCurrZRead.Rows(0).Item("CurNoTAXSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurNoTAXSales"))
                p("CurVATExemptSales") = CDec2(dtCurrZRead.Rows(0).Item("CurVATExemptSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurVATExemptSales"))
                p("CurZeroRatedVATSales") = CDec2(dtCurrZRead.Rows(0).Item("CurZeroRatedVATSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurZeroRatedVATSales"))
                p("CurOthers") = CDec2(dtCurrZRead.Rows(0).Item("CurOthers")) + CDec2(dtPOSTransaction.Rows(0).Item("CurOthers"))
                p("CashAmount") = CDec2(dtCurrZRead.Rows(0).Item("CashAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("CashAmount"))
                p("DebitCardAmount") = CDec2(dtCurrZRead.Rows(0).Item("DebitCardAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("DebitCardAmount"))
                p("CreditCardAmount") = CDec2(dtCurrZRead.Rows(0).Item("CreditCardAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("CreditCardAmount"))
                p("SRVAmount") = CDec2(dtCurrZRead.Rows(0).Item("SRVAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("SRVAmount"))
                p("SMACAmount") = CDec2(dtCurrZRead.Rows(0).Item("SMACAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("SMACAmount"))
                p("CheckPaymentAmount") = CDec2(dtCurrZRead.Rows(0).Item("CheckPaymentAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("CheckPaymentAmount"))
                p("OtherTypeAmount") = CDec2(dtCurrZRead.Rows(0).Item("OtherTypeAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("OtherTypeAmount"))
                p("OnCreditAmount") = CDec2(dtCurrZRead.Rows(0).Item("OnCreditAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("OnCreditAmount"))
                p("ChargeAmount") = CDec2(dtCurrZRead.Rows(0).Item("ChargeAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("ChargeAmount"))
                p("EPlusAmount") = CDec2(dtCurrZRead.Rows(0).Item("EPlusAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("EPlusAmount"))
                p("VoucherAmount") = CDec2(dtCurrZRead.Rows(0).Item("VoucherAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("VoucherAmount"))

                'mag zero ni sya kung sales return ang first transaction of the day
                If CDec2(dtCurrZRead.Rows(0).Item("CurBegInvoiceNo")) = 0 Then
                    p("CurBegInvoiceNo") = pTransInvoiceNo
                End If

                p("CurEndInvoiceNo") = pTransInvoiceNo

                p("CurTransCountInv") = CInt2(dtCurrZRead.Rows(0).Item("CurTransCountInv")) + 1
                p("CurTransInvItemCount") = CDec2(dtCurrZRead.Rows(0).Item("CurTransInvItemCount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTransInvItemCount"))

            End If

            If IsNew Then
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate("ID = " & ReadyString(dtCurrZRead.Rows(0).Item("ID")))
            End If

            mReports.Execute(pSql)

            Return True
        End Using

    End Function

    Public Function GetAvailableUnappliedCreditsDetails(vCustomerID As String, vFilter As String) As DataTable

        Dim pWhere As String = ""
        If vFilter = "all" Then
            pWhere &= "AND ISNULL(TransCustomerPayment.UnappliedPayment, 0) > 0" & vbCrLf
        Else
            pWhere &= "AND ISNULL(TransCustomerPayment.UnappliedPayment, 0) - ISNULL(tblAppliedCredits.Amount, 0) > 0" & vbCrLf
        End If
        Dim pSql = " -- GetAvailableUnappliedCredits receiving payment available unapplied credits" & vbCrLf &
            " SELECT TransCustomerPayment.LastUpdated" & vbCrLf &
            " ,TransCustomerPayment.ID AS SourcePaymentID" & vbCrLf &
            " ,TransCustomerPayment.UnappliedPayment" & vbCrLf &
            " ,ISNULL(tblAppliedCredits.Amount, 0) AS ConsumedAmount" & vbCrLf &
            " ,ISNULL(TransCustomerPayment.UnappliedPayment, 0) - ISNULL(tblAppliedCredits.Amount , 0) AS AvailableAmount" & vbCrLf &
            " ,TransCustomerPayment.ManualReceiptNumber" & vbCrLf &
            " ,TransCustomerPayment.ReceiptNumber AS PaymentNum" & vbCrLf &
            " ,TransCustomerPayment.PaymentDate" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT SUM(TransCustomerPaymentAppliedCredits.Amount) AS Amount" & vbCrLf &
            "     FROM TransCustomerPaymentAppliedCredits" & vbCrLf &
            "     WHERE TransCustomerPaymentAppliedCredits.SourcePaymentID = TransCustomerPayment.ID" & vbCrLf &
            " ) tblAppliedCredits" & vbCrLf &
            " WHERE TransCustomerPayment.CustomerID = " & ReadyString(vCustomerID) & vbCrLf &
            pWhere &
            " ORDER BY TransCustomerPayment.PaymentDate "
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetAvailableUnappliedCredits(vCustomerID As String) As Decimal

        Dim pSql = " -- GetAvailableUnappliedCredits receiving payment available unapplied credits" & vbCrLf &
            " SELECT SUM(TransCustomerPayment.UnappliedPayment - ISNULL(tblAppliedCredits.Amount , 0)) AS AvailableAmount" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT SUM(TransCustomerPaymentAppliedCredits.Amount) AS Amount" & vbCrLf &
            "     FROM TransCustomerPaymentAppliedCredits" & vbCrLf &
            "     WHERE TransCustomerPaymentAppliedCredits.SourcePaymentID = TransCustomerPayment.ID" & vbCrLf &
            " ) tblAppliedCredits" & vbCrLf &
            " WHERE TransCustomerPayment.UnappliedPayment > 0" & vbCrLf &
            " AND TransCustomerPayment.CustomerID = " & ReadyString(vCustomerID) & vbCrLf

        Dim pOut = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function GetDestinationUnappliedCreditsDetails(vSourcePaymentID As String) As DataTable

        Dim pSql = "  -- GetDestinationUnappliedCreditsDetails" & vbCrLf &
            "  SELECT TransCustomerPayment.LastUpdated" & vbCrLf &
            "  ,TransCustomerPayment.ID AS SourcePaymentID" & vbCrLf &
            "  ,TransCustomerPayment.UnappliedPayment" & vbCrLf &
            "  ,TransCustomerPaymentAppliedCredits.Amount AS ConsumedAmount" & vbCrLf &
            "  ,TransCustomerPayment.ManualReceiptNumber" & vbCrLf &
            "  ,TransCustomerPayment.ReceiptNumber AS PaymentNum" & vbCrLf &
            "  ,ISNULL(TransCustomerPayment.ManualReceiptNumber, TransCustomerPayment.ReceiptNumber) AS PaymentNumText" & vbCrLf &
            "  ,TransCustomerPayment.PaymentDate" & vbCrLf &
            "  FROM TransCustomerPayment" & vbCrLf &
            "  JOIN TransCustomerPaymentAppliedCredits ON TransCustomerPaymentAppliedCredits.DestinationPaymentID = TransCustomerPayment.ID" & vbCrLf &
            "  WHERE TransCustomerPaymentAppliedCredits.SourcePaymentID = " & ReadyString(vSourcePaymentID) & vbCrLf &
            "  ORDER BY TransCustomerPayment.PaymentDate "
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ChequeDetail(vBankID As String, vCheckNumber As String) As DataRow

        Dim pOrgSelect As String
        Dim pOrgFrom As String

        BrowserController.ComposeCustomer(pOrgSelect, pOrgFrom, vPeoplePreNominalSuffix:="MobizLibraryPeoplePreNominalSuffix_Payor")

        vCheckNumber = TrimLeadingZero(vCheckNumber)
        vCheckNumber = vCheckNumber.ToLower()
        Dim pSql = "  SELECT TransCustomerPaymentChecks.CheckNumber" & vbCrLf &
            "  ,TransCustomerPaymentChecks.CheckDate" & vbCrLf &
            "  ,TransCustomerPaymentChecks.CheckAmount" & vbCrLf &
            "  ,TransCustomerPaymentChecks.BankName" & vbCrLf &
            "  ,TransCustomerPaymentChecks.BankBranchName" & vbCrLf &
            "  ,TransCustomerPayment.PaymentDate" & vbCrLf &
            "," & BrowserController.ComposePeopleName() & " AS Encoder" &
            "," & pOrgSelect & " AS Payor" &
            "  FROM TransCustomerPaymentChecks" & vbCrLf &
            "  JOIN TransCustomerPayment ON TransCustomerPayment.ID = TransCustomerPaymentChecks.CustomerPaymentID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransCustomerPayment.CustomerID" & vbCrLf &
            pOrgFrom &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransCustomerPayment.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " WHERE BankID = " & ReadyString(vBankID) & vbCrLf &
            " AND LTRIM(RTRIM(CheckNumber)) like " & ReadyStringWild(vCheckNumber)
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pCheckNumber = CStr2(pDR("CheckNumber"))
            pCheckNumber = TrimLeadingZero(pCheckNumber)
            pCheckNumber = pCheckNumber.ToLower()
            If pCheckNumber = vCheckNumber Then
                pDT.Columns.Add("Payee")
                Dim pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
                pDR("Payee") = pHeaderInfo.CompanyName
                Return pDR
            End If
        Next

    End Function

    Public Shared Function TrimLeadingZero(vString As String) As String

        If String.IsNullOrWhiteSpace(vString) Then
            Return ""
        End If

        vString = vString.Trim()
        Do
            If vString.StartsWith("0") Then
                vString = vString.Substring(1)
            Else
                Return vString
            End If
        Loop

    End Function

    Public Sub AdjustUnappliedPayment(vInvoiceID As String, vTransAmount As Decimal)

        Dim pNetInvoiceBalance = mParent.TransInvoice.GetNetInvoiceBalance(vInvoiceID)
        If pNetInvoiceBalance >= 0 Then
            Return
        End If

        Dim pDtPayments = GetPaymentIdsByInvoiceID(vInvoiceID)
        If pDtPayments.Rows.Count = 0 Then
            Return
        End If
        Dim pIU = New InsertUpdate("TransCustomerPayment")
        pIU("UnappliedPayment") = "UnappliedPayment + " & vTransAmount
        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDtPayments.Rows(0)("CustomerPaymentID")))
        pSql = mProData.ExecuteSQL(pSql)

    End Sub

    Public Function FillMissingPaymentData(Optional vFowstID As String = Nothing, Optional vDeviceID As String = Nothing) As DataTable

        Dim pDtInvoices = InvoicesWithoutPaymentData(vFowstID, vDeviceID)
        For Each pDrInvoice As DataRow In pDtInvoices.Rows
            mProData.BeginTransaction()
            Dim pIU = New InsertUpdate("TransCustomerPayment")
            Dim pPaymentID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pPaymentID)
            pIU("UserID") = ReadyString(pDrInvoice("UserID"))
            pIU("CustomerID") = ReadyString(pDrInvoice("CustomerID"))
            pIU("CashAmount") = CDec2(pDrInvoice("AmountDueBeforeCharges"))
            pIU("ReceiptNumber") = 0
            pIU("ReceiptType") = "'OR'"
            pIU("FowstID") = ReadyString(pDrInvoice("FowstID"))
            pIU("DeviceID") = ReadyString(pDrInvoice("DeviceID"))
            Dim pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            pIU = New InsertUpdate("TransCustomerPaymentInvoices")
            pIU("CustomerPaymentID") = ReadyString(pPaymentID)
            pIU("InvoiceID") = ReadyString(pDrInvoice("ID"))
            pIU("AmountPaid") = CDec2(pDrInvoice("AmountDueBeforeCharges"))
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            AppendToLog("InvoiceID: " & CStr2(pDrInvoice("ID")),, "FillMissingPaymentData")
            mProData.CommitTransaction()
        Next

    End Function

    Private Function InvoicesWithoutPaymentData(Optional vFowstID As String = Nothing, Optional vDeviceID As String = Nothing) As DataTable

        If mProData.CommandTimeout < 5 * 60 Then
            mProData.CommandTimeout = 5 * 60
        End If
        Dim pSql = "SELECT TransInvoice.ID" & vbCrLf &
            ",TransInvoice.AmountDueBeforeCharges" & vbCrLf &
            ",TransInvoice.UserID" & vbCrLf &
            ",TransInvoice.CustomerID" & vbCrLf &
            ",TransInvoice.FowstID" & vbCrLf &
            ",TransInvoice.DeviceID" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " LEFT JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " AND TransCustomerPaymentInvoices.ID IS NULL" & vbCrLf &
            " AND TransInvoice.AmountDueBeforeCharges > 0" & vbCrLf &
            " AND TransInvoice.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND TransInvoice.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " ORDER BY TransInvoice.InvoiceDate"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function
End Class
