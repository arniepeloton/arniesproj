Imports System.Linq

Public Class TransReceivingPayment

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

        Try
            If IsGuid(vJson.DeviceID) = False Then
                oError = "DeviceID must be a valid GUID. ref: vbFL25HYPDT6!"
                Return Nothing
            End If
            If IsGuid(vJson.FowstID) = False Then
                oError = "FowstID must be a valid GUID. ref: vb9C2WHJRSLX!"
                Return Nothing
            End If
            If VerifyPayment(vJson, oError) = False Then
                Return Nothing
            End If

            If VerifySrvPayments(vJson, oError) = False Then
                Return Nothing
            End If
            If VerifyInvoices(vJson, oError) = False Then
                Return Nothing
            End If
            vJson.ConsumedAmount = (From pInvoice In vJson.Receivings
                                Select pInvoice.AmountPaid).Sum()
            vJson.CheckAmount = (From pCheque In vJson.Cheques
                                Select pCheque.CheckAmount).Sum()
            vJson.CreditCardAmount = (From pCreditCard In vJson.CreditCards
                                Select pCreditCard.CreditCardAmount).Sum()
            vJson.DebitCardAmount = (From pDebitCard In vJson.DebitCards
                                Select pDebitCard.DebitCardAmount).Sum()
            vJson.OtherTypeAmount = (From pOtherPayment In vJson.OtherPayments
                                Select pOtherPayment.Amount).Sum()
            If vJson.UnappliedCreditToUse IsNot Nothing Then
                vJson.UsedUnappliedCredit =
                    (From pUnappliedCreditToUse In vJson.UnappliedCreditToUse
                    Select CDec2(pUnappliedCreditToUse.AmountToUse)).Sum()
            End If
            
            'vJson.SrvPaymentAmount = (From pSrvPayment In vJson.SrvPayments
            '                    Select pSrvPayment.ConsumedAmount).Sum()
            pTotalPaid = vJson.Cash +
                vJson.CheckAmount +
                vJson.CreditCardAmount +
                vJson.DebitCardAmount +
                vJson.OtherTypeAmount +
                vJson.SrvPaymentAmount +
                vJson.ConsumedUnappliedPaymentProper

            pIU = New InsertUpdate("TransReceivingPayment")
            pID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pID)
            pIU("UserID") = ReadyString(vJson.UserID)
            pIU("SupplierID") = ReadyString(vJson.CustomerID)
            pIU("CashAmount") = vJson.Cash
            pIU("ChecksAmount") = vJson.CheckAmount
            pIU("CreditCardAmount") = vJson.CreditCardAmount
            pIU("DebitCardAmount") = vJson.DebitCardAmount
            pIU("OtherPaymentAmount") = vJson.OtherTypeAmount
            pIU("UsedUnappliedCredit") = vJson.UsedUnappliedCredit
            mProData.BeginTransaction()
            pOrNum = GetNextNumber(vJson.FowstID, vJson.DeviceID)
            pIU("PaymentNum") = pOrNum
            pIU("IsActive") = 1
            pIU("FowstID") = ReadyString(vJson.FowstID)
            pIU("DeviceID") = ReadyString(vJson.DeviceID)
            pIU("PaymentDate") = ReadyString(vJson.PaymentDateProper)
            pIU("LastUpdated") = "GETDATE()"
            If String.IsNullOrWhiteSpace(vJson.ManualReceiptNumber) = False Then
                pIU("ManualReceiptNumber") = ReadyString(vJson.ManualReceiptNumber.Trim())
            End If
            pIU("UnappliedPayment") = pTotalPaid - vJson.ConsumedAmount
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            If SaveReceivings(pID, vJson) = False Then
                mProData.RollbackTransaction()
                Return Nothing
            End If
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
            SaveChecksAmount(pID)
            mParent.TransReceiving.CacheReceivingsWithCredit()
            mProData.CommitTransaction()

            Return pID
        Catch ex As Exception
            AppendToDebug("vbCVUGCGS8UZ! " & ex.Message, True, ex, True)
            oError = ex.Message
        End Try

    End Function

    Private Function GetNextNumber(vFowstID As String, vDeviceID As String) As Integer

        Dim pSql As String
        Dim pOut As Integer

        pSql = "SELECT MAX(PaymentNum)" & vbCrLf &
            " FROM TransReceivingPayment" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID)
        pOut = CInt2(mProData.ExecuteScalar(pSql)) + 1

        Return pOut

    End Function

    Private Function VerifyPayment(vJson As SavePaymentJson, ByRef oError As String) As Boolean

        Dim pAvailable As Decimal

        oError = ""
        If vJson.ConsumedUnappliedPaymentProper = 0 Then
            Return True
        End If
        If vJson.ConsumedUnappliedPaymentProper < 0 Then
            oError = "Consumed unapplied payment must be equal to or greater than zero."
            Return False
        End If

        pAvailable = GetAvailableUnappliedCredits(vJson.CustomerID)
        If pAvailable < vJson.ConsumedUnappliedPaymentProper Then
            oError = "The Accumulated Unapplied Payment(s) And Credit(s) for this customer is " & Format(pAvailable, cGlobal.FormatCurrency) & ". You tried to consume " & Format(vJson.ConsumedUnappliedPaymentProper, cGlobal.FormatCurrency) & "."
            Return False
        End If

        Return True

    End Function

    Private Function SaveReceivings(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pReceiving As SavePaymentJson.ReceivingClass In vJson.Receivings
            pIU = New InsertUpdate("TransReceivingPaymentReceivings")
            pIU("ID") = "NEWID()"
            pIU("PaymentID") = ReadyString(vPaymentID)
            pIU("ReceivingID") = ReadyString(pReceiving.ID)
            pIU("AmountPaid") = pReceiving.AmountPaid
            pIU("WithHoldingTaxAmount") = CDec2(pReceiving.WithHoldingTaxAmount)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            UpdateProductPrices(pReceiving.ID, vJson.OrgID, False)
        Next pReceiving

        Return True

    End Function

    Public Function UpdateProductPrices(vReceivingID As String, vOrgID As String, vDontUpdateNonZeroPrices As Boolean) As Integer

        Dim pProductVendorCost As Decimal
        Dim pFreightAndHandling As Decimal

        Dim pSql = " SELECT TransReceiving.ID" & vbCrLf &
            " ,TransReceiving.FreightAndHandling" & vbCrLf &
            " ,TransReceiving.LastUpdated" & vbCrLf &
            " ,TransReceiving.AccountsPayableID" & vbCrLf &
            " ,TransInvoiceItems.UnitPrice" & vbCrLf &
            " ,TransInvoiceItems.ProductID" & vbCrLf &
            " ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            " ,TransInvoiceItems.Quantity" & vbCrLf &
            " ,TransReceivingStock.QuantityGood" & vbCrLf &
            " ,TransReceivingStock.QuantityDamaged" & vbCrLf &
            " ,TransReceivingStock.ActualQuantityGood" & vbCrLf &
            " ,TransReceivingStock.UnitFreightCost" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransReceiving.AccountsPayableID" & vbCrLf &
            " LEFT JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID" & vbCrLf &
            " AND TransInvoiceItems.ProductID = TransReceivingStock.ProductID" & vbCrLf &
            " AND TransInvoiceItems.ActualQuantity = TransReceivingStock.ActualQuantityGood" & vbCrLf &
            " WHERE TransReceiving.ID = " & ReadyString(vReceivingID) & vbCrLf &
            " ORDER BY ActualQuantityGood"
        Dim pDT = mProData.Execute(pSql)

        pFreightAndHandling = CDec2(pDT.Rows(0)("FreightAndHandling"))
        If pDT.Rows.Count = 0 Then
            Return 0
        End If
        For Each pDR In pDT.Rows
            pProductVendorCost += (CDec2(pDR("Quantity")) + CDec2(pDR("QuantityDamaged"))) * CDec2(pDR("UnitPrice"))
        Next
        For Each pDR In pDT.Rows
            Dim pLineFreightCost As Decimal
            If pProductVendorCost * pFreightAndHandling = 0 Then
                pLineFreightCost = 0
            Else
                pLineFreightCost = (CDec2(pDR("Quantity")) + CDec2(pDR("QuantityDamaged"))) * CDec2(pDR("UnitPrice")) / pProductVendorCost * pFreightAndHandling
            End If
            If (CDec2(pDR("Quantity")) + CDec2(pDR("QuantityDamaged"))) = 0 Then
                pDR("UnitFreightCost") = 0
            Else
                pDR("UnitFreightCost") = pLineFreightCost / (CDec2(pDR("Quantity")) + CDec2(pDR("QuantityDamaged")))
            End If
        Next

        Dim pDtProducts = pDT.DefaultView.ToTable(True, "ProductID")
        For Each pDrProduct As DataRow In pDtProducts.Rows
            Dim pDV = New DataView(pDT, "ProductID = " & ReadyString(pDrProduct("ProductID")), "", DataViewRowState.CurrentRows)
            Dim pTotalGoodQty As Decimal = 0 ' in base unit (1)
            Dim pTotalGoodBadQty As Decimal = 0 ' in base unit (1)
            Dim pTotalFreightCost As Decimal = 0 ' per base unit
            Dim pTotalVendorCost As Decimal = 0 ' per base unit
            For Each pDRV As DataRowView In pDV
                pTotalGoodQty += CDec2(pDRV("Quantity")) * CDec2(pDRV("ActualQuantity"))
                pTotalGoodBadQty += (CDec2(pDRV("Quantity")) + CDec2(pDRV("QuantityDamaged"))) * CDec2(pDRV("ActualQuantity"))
                pTotalFreightCost += (CDec2(pDRV("Quantity")) + CDec2(pDRV("QuantityDamaged"))) * CDec2(pDRV("UnitFreightCost"))
                pTotalVendorCost += CDec2(pDRV("Quantity")) * CDec2(pDRV("UnitPrice"))
            Next pDRV
            Dim pFreightCost As Decimal = 0
            Dim pVendorPrice As Decimal = 0
            If pTotalGoodQty <> 0 Then
                pVendorPrice = pTotalVendorCost / pTotalGoodQty
            End If
            If pTotalGoodBadQty <> 0 Then
                pFreightCost = pTotalFreightCost / pTotalGoodBadQty
            End If

            Dim pUpdateVendorPrice As Boolean = False
            Dim pUpdateFreightCost As Boolean = False
            Dim pUpdateLandedCost As Boolean = False

            If vDontUpdateNonZeroPrices Then
                If mParent.LibraryStock.GetFactoryPrice(CStr2(pDrProduct("ProductID")), vOrgID) = 0 Then
                    pUpdateVendorPrice = True
                End If
                If mParent.LibraryStock.GetFreightCost(CStr2(pDrProduct("ProductID")), vOrgID) = 0 Then
                    pUpdateFreightCost = True
                End If
                If mParent.LibraryStock.GetLandedCost(CStr2(pDrProduct("ProductID")), vOrgID) = 0 Then
                    pUpdateLandedCost = True
                End If
            Else
                pUpdateVendorPrice = True
                pUpdateFreightCost = True
                pUpdateLandedCost = True
            End If

            If pUpdateVendorPrice Then
                mParent.LibraryStock.SetPrice(CStr2(pDrProduct("ProductID")), "01000012-0002-0000-0000-000000000000", pVendorPrice, vOrgID) ' Factory Price ID
            End If
            If pUpdateFreightCost Then
                mParent.LibraryStock.SetPrice(CStr2(pDrProduct("ProductID")), "01000012-0001-0000-0000-000000000000", pFreightCost, vOrgID) ' Freight Cost ID
            End If
            If pUpdateLandedCost Then
                mParent.LibraryStock.SetPrice(CStr2(pDrProduct("ProductID")), "01000012-0003-0000-0000-000000000000", pVendorPrice + pFreightCost, vOrgID) ' Landed Cost ID
            End If
        Next pDrProduct

    End Function

    Private Function VerifyInvoices(vJson As SavePaymentJson, ByRef oError As String) As Boolean

        For Each pReceiving As SavePaymentJson.ReceivingClass In vJson.Receivings
            Dim pDT = mParent.TransReceiving.GetReceivingsWithPayableFromCache(vJson.CustomerID, , pReceiving.ID)
            For Each pDR As DataRow In pDT.Rows
                If pDR Is Nothing Then
                    oError = "Receiving, " & CStr2(pDR("InvoiceNumber")) & "(todo) was already paid. Please reload the page."
                    Return False
                ElseIf CDec2(pDR("NetInvoiceBalance")) < pReceiving.AmountPaid Then
                    oError = CStr2(pDR("ApNum")) & " " & CStr2(pDR("InvoiceNumber")) & " amount paid (" & Format(pReceiving.AmountPaid, cGlobal.FormatCurrency) & ") is greater than invoice net balance (" & Format(CDec2(pDR("NetInvoiceBalance")), cGlobal.FormatCurrency) & ")"
                    Return False
                End If
            Next
        Next

        Return True

    End Function

    Private Function SaveCheques(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pCheque As SavePaymentJson.ChequeClass In vJson.Cheques
            pIU = New InsertUpdate("TransReceivingPaymentChecks")
            pIU("ID") = "NEWID()"
            pIU("PaymentID") = ReadyString(vPaymentID)
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

    Public Function ChequeDetail(vBankID As String, vCheckNumber As String) As DataRow

        Dim pOrgSelect As String
        Dim pOrgFrom As String

        BrowserController.ComposeCustomer(pOrgSelect, pOrgFrom, vPeoplePreNominalSuffix:="MobizLibraryPeoplePreNominalSuffix_Payor")

        vCheckNumber = TransCustomerPayment.TrimLeadingZero(vCheckNumber)
        vCheckNumber = vCheckNumber.ToLower()
        Dim pSql = "  SELECT TransReceivingPaymentChecks.CheckNumber" & vbCrLf &
            "  ,TransReceivingPaymentChecks.CheckDate" & vbCrLf &
            "  ,TransReceivingPaymentChecks.CheckAmount" & vbCrLf &
            "  ,TransReceivingPaymentChecks.BankName" & vbCrLf &
            "  ,TransReceivingPaymentChecks.BankBranchName" & vbCrLf &
            "  ,TransReceivingPayment.PaymentDate" & vbCrLf &
            "," & BrowserController.ComposePeopleName() & " AS Encoder" &
            "," & pOrgSelect & " AS Payee" &
            "  FROM TransReceivingPaymentChecks" & vbCrLf &
            "  JOIN TransReceivingPayment ON TransReceivingPayment.ID = TransReceivingPaymentChecks.PaymentID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransReceivingPayment.SupplierID" & vbCrLf &
            pOrgFrom &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransReceivingPayment.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " WHERE BankID = " & ReadyString(vBankID) & vbCrLf &
            " AND LTRIM(RTRIM(CheckNumber)) like " & ReadyStringWild(vCheckNumber)
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pCheckNumber = CStr2(pDR("CheckNumber"))
            pCheckNumber = TransCustomerPayment.TrimLeadingZero(pCheckNumber)
            pCheckNumber = pCheckNumber.ToLower()
            If pCheckNumber = vCheckNumber Then
                pDT.Columns.Add("Payor")
                Dim pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
                pDR("Payor") = pHeaderInfo.CompanyName
                Return pDR
            End If
        Next

    End Function

    Private Function SaveCreditCards(vPaymentID As String, vJson As SavePaymentJson) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        For Each pCreditCard As SavePaymentJson.CreditCardClass In vJson.CreditCards
            pIU = New InsertUpdate("TransReceivingPaymentCreditCard")
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

            If pCreditCard.CreditCardIssuanceDate IsNot Nothing _
                AndAlso String.IsNullOrWhiteSpace(pCreditCard.CreditCardIssuanceDate) = False Then

                pIU("CreditCardIssuanceDate") = ReadyString(pCreditCard.CreditCardIssuanceDate)
            End If

            If pCreditCard.CreditCardExpirationDate IsNot Nothing _
                AndAlso String.IsNullOrWhiteSpace(pCreditCard.CreditCardExpirationDate) = False Then

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
            pIU = New InsertUpdate("TransReceivingPaymentDebitCard")
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

            If pDebitCard.DebitCardIssuanceDate IsNot Nothing _
                AndAlso String.IsNullOrWhiteSpace(pDebitCard.DebitCardIssuanceDate) = False Then

                pIU("DebitCardIssuanceDate") = ReadyString(pDebitCard.DebitCardIssuanceDate)
            End If

            If pDebitCard.DebitCardExpirationDate IsNot Nothing _
                AndAlso String.IsNullOrWhiteSpace(pDebitCard.DebitCardExpirationDate) = False Then

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
            pIU = New InsertUpdate("TransReceivingPaymentOther")
            pIU("ID") = "NEWID()"
            pIU("PaymentID") = ReadyString(vPaymentID)
            If String.IsNullOrWhiteSpace(pOtherPayments.PaymentType) Then
                Throw New ApplicationException("PaymentType must not be blank. vb784LTZCABC!")
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
            pIU = New InsertUpdate("TransReceivingPaymentAppliedCredits")
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
        Public UsedUnappliedCredit As Decimal ' set in server side
        Public SrvPaymentAmount As Decimal ' set in server side
        ''' <summary>
        ''' total amount paid to invoices
        ''' </summary>
        ''' <remarks></remarks>
        Public ConsumedAmount As Decimal ' set in server side
        Public CurrentPaymentReceived As Decimal?
        Public UserID As String
        Public CustomerID As String ' SupplierID
        Public Receivings As List(Of ReceivingClass)
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
        Public PaymentDate As String
        Public OrgID As String ' set in server side

        Public Class ReceivingClass
            Public ID As String
            Public AmountPaid As Decimal?
            Public WithHoldingTaxAmount As String ' decimal
        End Class

        Public ReadOnly Property ConsumedUnappliedPaymentProper As Decimal
            Get

                Return CDec2(ConsumedUnappliedPayment)

            End Get
        End Property

        Public ReadOnly Property PaymentDateProper As String
            Get

                Dim pOut As String = PaymentDate

                If String.IsNullOrWhiteSpace(PaymentDate) Then
                    pOut = Now
                End If

                If IsDate(pOut) Then
                    pOut = Format(ParseDate(pOut), cGlobal.FormatDateTimeSql2)
                Else
                    Throw New ApplicationException("PaymentDate must be a valid date. ref: vbYVWC6TRY54!")
                End If

                Return pOut

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

    Public Function SaveChecksAmount(vID As String) As Integer

        Dim pDrPayment As DataRow
        Dim pDtChecks As DataTable
        Dim pDtReceivings As DataTable
        Dim pTotalCheckAmount As Decimal
        Dim pTotalAmountPaid As Decimal
        Dim pUnappliedPayment As Decimal
        Dim pDatas As List(Of SaveChecksAmountData)
        Dim pData As SaveChecksAmountData

        pDrPayment = GetByID(vID)
        pDtChecks = GetChecks(vID)
        pDtReceivings = GetReceivings(vID)

        pTotalCheckAmount = CDec2(pDtChecks.Compute("SUM(CheckAmount)", ""))
        pDatas = New List(Of SaveChecksAmountData)

        If pTotalCheckAmount <> 0 Then
            pTotalAmountPaid = CDec2(pDtReceivings.Compute("SUM(AmountPaid)", ""))
            pUnappliedPayment = CDec2(pDrPayment("UnappliedPayment"))

            For Each pDrReceivings As DataRow In pDtReceivings.Rows
                For Each pDrCheck As DataRow In pDtChecks.Rows
                    pData = New SaveChecksAmountData
                    pData.CheckID = CStr2(pDrCheck("ID"))
                    pData.ReceivingID = CStr2(pDrReceivings("ReceivingID"))
                    pData.AmountPaid = CDec2(pDrCheck("CheckAmount")) * pDrReceivings("AmountPaid") / (pTotalAmountPaid + pUnappliedPayment)
                    pDatas.Add(pData)
                Next pDrCheck
            Next pDrReceivings

            If pUnappliedPayment > 0 Then
                For Each pDrCheck As DataRow In pDtChecks.Rows
                    pData = New SaveChecksAmountData
                    pData.CheckID = CStr2(pDrCheck("ID"))
                    pData.Overpayment = CDec2(pDrCheck("CheckAmount")) * pUnappliedPayment / (pTotalAmountPaid + pUnappliedPayment)
                    pDatas.Add(pData)
                Next pDrCheck
            End If
        End If

        SaveChecksAmountActual(vID, pDatas)

        Return pDatas.Count

    End Function

    Private Function SaveChecksAmountActual(vID As String, vData As List(Of SaveChecksAmountData))

        Dim pIU As InsertUpdate
        Dim pSql As String

        mProData.BeginTransaction()
        pSql = "DELETE FROM TransReceivingPaymentCheckAmount" & vbCrLf &
            " WHERE PaymentID = " & ReadyString(vID)
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        For Each pData As SaveChecksAmountData In vData
            pIU = New InsertUpdate("TransReceivingPaymentCheckAmount")
            pIU("ID") = "NEWID()"
            pIU("PaymentID") = ReadyString(vID)
            pIU("PaymentCheckID") = ReadyString(pData.CheckID)
            If pData.ReceivingID Is Nothing Then
                pIU("Overpayment") = pData.Overpayment
            Else
                pIU("ReceivingID") = ReadyString(pData.ReceivingID)
                pIU("AmountPaid") = pData.AmountPaid
            End If
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pData
        mProData.CommitTransaction()

    End Function

    Private Class SaveChecksAmountData
        Public CheckID As String
        Public ReceivingID As String
        Public AmountPaid As Decimal
        Public Overpayment As Decimal
    End Class

    Private Function GetChecks(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransReceivingPaymentChecks.ID" & vbCrLf &
            ",TransReceivingPaymentChecks.CheckAmount" & vbCrLf &
            ",TransReceivingPaymentChecks.CheckNumber" & vbCrLf &
            ",TransReceivingPaymentChecks.CheckDate" & vbCrLf &
            ",TransReceivingPaymentChecks.BankName" & vbCrLf &
            ",TransReceivingPaymentChecks.BankBranchName" & vbCrLf &
            ",TransReceivingPaymentChecks.BankName2" & vbCrLf &
            ",TransReceivingPaymentChecks.BankBranchName2" & vbCrLf &
            ",TransReceivingPaymentChecks.AccountName" & vbCrLf &
            ",TransReceivingPaymentChecks.AccountName2" & vbCrLf &
            ",TransReceivingPaymentChecks.AccountNumber" & vbCrLf &
            ",TransReceivingPaymentChecks.AccountNumber2" & vbCrLf &
            ",ISNULL(TransReceivingPaymentChecks.IsOnlinePayment, 0) AS IsOnlinePayment" & vbCrLf &
            " FROM TransReceivingPaymentChecks" & vbCrLf &
            " WHERE TransReceivingPaymentChecks.PaymentID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetReceivings(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        ' PrintedInvoiceNumber = for payment voucher print template
        pSql = " SELECT TransReceivingPaymentReceivings.ReceivingID" & vbCrLf &
            " ,CacheReceivings.NetInvoiceAmount" & vbCrLf &
            " ,TransReceivingPaymentReceivings.AmountPaid" & vbCrLf &
            " ,CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.AccountsPayableNum) AS ApNum" & vbCrLf &
            " ,TransReceiving.InvoiceNumber" & vbCrLf &
            " FROM TransReceivingPaymentReceivings" & vbCrLf &
            " LEFT JOIN CacheReceivings ON CacheReceivings.ID = TransReceivingPaymentReceivings.ReceivingID" & vbCrLf &
            "  LEFT JOIN TransReceiving ON TransReceiving.ID = TransReceivingPaymentReceivings.ReceivingID" & vbCrLf &
            "  LEFT JOIN TransInvoice ON TransInvoice.ID = TransReceiving.AccountsPayableID" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "  LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " WHERE TransReceivingPaymentReceivings.PaymentID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("PrintedInvoiceNumber")

        For Each pDR As DataRow In pDT.Rows
            If CStr2(pDR("ApNum")) <> "" Then
                If CStr2(pDR("PrintedInvoiceNumber")) <> "" Then
                    pDR("PrintedInvoiceNumber") &= "; "
                End If
                pDR("PrintedInvoiceNumber") = CStr2(pDR("PrintedInvoiceNumber")) & "AP " & pDR("ApNum")
            End If
            If CStr2(pDR("InvoiceNumber")).Trim() <> "" Then
                If CStr2(pDR("PrintedInvoiceNumber")) <> "" Then
                    pDR("PrintedInvoiceNumber") &= "; "
                End If
                pDR("PrintedInvoiceNumber") = CStr2(pDR("PrintedInvoiceNumber")) & CStr2(pDR("InvoiceNumber")).Trim()
            End If
        Next

        Return pDT

    End Function

    ''' <summary>
    ''' TransReceivingPayment.ID
    ''' </summary>
    ''' <param name="vPaymentID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetOtherPayments(vPaymentID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TransReceivingPaymentOther.PaymentType" & vbCrLf &
            " ,TransReceivingPaymentOther.Amount" & vbCrLf &
            " FROM TransReceivingPaymentOther" & vbCrLf &
            " WHERE TransReceivingPaymentOther.PaymentID = " & ReadyString(vPaymentID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetByID(vID As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT UnappliedPayment" & vbCrLf &
            " FROM TransReceivingPayment" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetChequeDistribution(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " -- payment distribution" & vbCrLf &
            "  SELECT TransCustomerPaymentChecks.CustomerPaymentID" & vbCrLf &
            "  , TransCustomerPaymentInvoices.ID" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
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
            " ,TransCustomerPaymentCheckAmount.AmountPaid" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount.Overpayment" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount.CustomerPaymentCheckID" & vbCrLf &
            " ,TransCustomerPaymentChecks.CheckAmount" & vbCrLf &
            " ,TransCustomerPaymentChecks.CheckNumber" & vbCrLf &
            " ,TransCustomerPaymentChecks.CheckDate" & vbCrLf &
            " FROM TransCustomerPaymentCheckAmount" & vbCrLf &
            " JOIN TransCustomerPaymentChecks ON TransCustomerPaymentChecks.ID = TransCustomerPaymentCheckAmount.CustomerPaymentCheckID" & vbCrLf &
            " LEFT JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.InvoiceID = TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
            " AND TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentChecks.CustomerPaymentID" & vbCrLf &
            " LEFT JOIN TransInvoice ON TransInvoice.ID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " WHERE TransCustomerPaymentCheckAmount.CustomerPaymentID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetCheckInvoices(vCustomerPaymentCheckID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TransCustomerPaymentCheckAmount.AmountPaid" & vbCrLf &
            " ,TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
            " FROM TransCustomerPaymentCheckAmount" & vbCrLf &
            " WHERE TransCustomerPaymentCheckAmount.CustomerPaymentCheckID = " & ReadyString(vCustomerPaymentCheckID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

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

    Public Function GetPaymentIdsByAccountPayableID(vAccountPayableID As String) As DataTable

        Dim pSql = " SELECT TransReceivingPaymentReceivings.PaymentID" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " JOIN TransReceivingPaymentReceivings ON TransReceivingPaymentReceivings.ReceivingID = TransReceiving.ID" & vbCrLf &
            " WHERE TransReceiving.AccountsPayableID = " & ReadyString(vAccountPayableID) & vbCrLf &
            " GROUP BY TransReceivingPaymentReceivings.PaymentID"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetPaymentData(vID As String) As DataSet

        Dim pOut As New DataSet
        Dim pOrgSelect As String
        Dim pOrgFrom As String

        BrowserController.ComposeCustomer(pOrgSelect, pOrgFrom)

        Dim pSql = "SELECT TransReceivingPayment.ID" & vbCrLf &
            " ,TransReceivingPayment.SupplierID" & vbCrLf &
            " ," & pOrgSelect & " AS CustomerName" & vbCrLf &
            " ,TransReceivingPayment.CashAmount + TransReceivingPayment.ChecksAmount + TransReceivingPayment.CreditCardAmount + TransReceivingPayment.DebitCardAmount + ISNULL(TransReceivingPayment.OtherPaymentAmount, 0) + ISNULL(TransReceivingPayment.UsedUnappliedCredit, 0) AS TotalPayment" & vbCrLf &
            " ,TransReceivingPayment.CreditCardAmount + TransReceivingPayment.DebitCardAmount AS OtherAmount" & vbCrLf &
            " ,TransReceivingPayment.ChecksAmount" & vbCrLf &
            " ,TransReceivingPayment.CashAmount" & vbCrLf &
            " ,TransReceivingPayment.PaymentDate" & vbCrLf &
            " ,TransReceivingPayment.UnappliedPayment" & vbCrLf &
            " ,ISNULL(TransReceivingPayment.ManualReceiptNumber, TransReceivingPayment.PaymentNum) AS ReceiptNumber" & vbCrLf &
            " ,MobizLibraryOrganization.TinNumber" & vbCrLf & _
            ",COALESCE(MobizLibraryOrganization.BusinessIndustryText, MobizLibraryIndustryClass.Class) AS BusinessIndustryDisplay" & vbCrLf &
            ",ISNULL(MobizLibraryOrganization.OfficeAddressBuildingNo, '')" & vbCrLf &
            " + ',' + ISNULL(MobizLibraryOrganization.OfficeAddressStreet, '')" & vbCrLf &
            " + ',' + ISNULL(tblCustomerCity.Name, '')" & vbCrLf &
            " + ',' + ISNULL(tblCustomerCountry.Name, '') AS CustomerAddress" & vbCrLf &
            ",MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName AS EmployeeFullName" & vbCrLf &
            " FROM TransReceivingPayment" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransReceivingPayment.SupplierID" & vbCrLf &
            pOrgFrom & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblCustomerCity ON tblCustomerCity.ID = MobizLibraryOrganization.OfficeAddressCityID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountries tblCustomerCountry ON tblCustomerCountry.ID = MobizLibraryOrganization.OfficeAddressCountryID" & vbCrLf &
            " LEFT JOIN MobizLibraryIndustryClass ON MobizLibraryIndustryClass.ID = MobizLibraryOrganization.IndustryClassID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransReceivingPayment.UserID" & vbCrLf &
            " WHERE TransReceivingPayment.ID = " & ReadyString(vID) & vbCrLf
        Dim pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return Nothing
        End If
        pDT.Columns.Add("TotalDue", GetType(Decimal))
        pDT.Columns.Add("PaidAmountInWords")
        'pDT.Columns.Add("Invoices")
        pDT.TableName = "Master"
        pOut.Tables.Add(pDT)

        Dim pDtInvoices = GetReceivings(vID)
        pDtInvoices.TableName = "Invoices"
        pOut.Tables.Add(pDtInvoices)

        'For Each pDR As DataRow In pDT.Rows
        '    pDR("TotalDue") = pDtInvoices.Compute("SUM(NetInvoiceAmount)", "")
        '    For Each pDrInvoice As DataRow In pDtInvoices.Rows
        '        If CStr2(pDR("Invoices")) <> "" Then
        '            pDR("Invoices") &= ", "
        '        End If
        '        pDR("Invoices") = CStr2(pDR("Invoices")) & pDrInvoice("InvoiceNumber")
        '    Next pDrInvoice
        'Next pDR

        Dim pDtChecks = GetChecks(vID)
        pDtChecks.TableName = "Checks"
        pOut.Tables.Add(pDtChecks)

        Dim pDtOtherPayments = GetOtherPayments(vID)
        pDtOtherPayments.TableName = "OtherPayments"
        pOut.Tables.Add(pDtOtherPayments)

        Return pOut

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vPaymentID"></param>
    ''' <param name="vMode">AP or AR (default)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateForm2307(vPaymentID As String, vMode As String) As String

        Dim pFilename = cGlobal.AppPath & "reports\Form-2307-Jan-2018-ENCS-v3.xlsx"
        If IO.File.Exists(pFilename) = False Then
            Return JsonDictionary.Serialize("Error", "File, " & pFilename & ", does not exist. ref: vbG6FAJSZLU7!")
        End If

        Dim pBaseFilename = DateTimeStamp()
        Dim pFilename2 = cGlobal.AppPath & "temp\Form2307\Form2307_" & pBaseFilename & ".xlsx"
        CopyFile(pFilename, pFilename2)
        Dim pZip = New ProZip(pFilename2)

        Const pPeriodFromMonthDate As String = "2JFLFXK95E"
        Const pPeriodFromYear As String = "YB6C9ZS4T6"
        Const pPeriodToMonthDate As String = "NKLX2JRDD6"
        Const pPeriodToYear As String = "7BC2A5JYDN"

        Const pPayeeTin1 As String = "SPKNLZB8YE"
        Const pPayeeTin2 As String = "BZEYYGLSP9"
        Const pPayeeTin3 As String = "D5DE8HHHM2"
        Const pPayeeTin4 As String = "Z56L4MJ5E7"
        Const pPayeeName As String = "NBBGK5X7JK"
        Const pPayeeAddress As String = "XK2WWZ56YB"
        Const pPayeeZipCode As String = "R22REBQ36L"

        Const pPayorTin1 As String = "8Y49E7QGME"
        Const pPayorTin2 As String = "ESKT88XCLN"
        Const pPayorTin3 As String = "26FCQBULKU"
        Const pPayorTin4 As String = "8KDNXYXU28"
        Const pPayorName As String = "9CPJB49BSD"
        Const pPayorAddress As String = "EAQ2GTQBGW"
        Const pPayorZipCode As String = "MARNRB7EUX"

        Dim pContent = pZip.GetString("xl\drawings\drawing1.xml")
        pContent = pContent.Replace(pPeriodFromMonthDate, Now.ToString("MMdd"))
        pContent = pContent.Replace(pPeriodFromYear, Now.ToString("yyy"))
        pContent = pContent.Replace(pPeriodToMonthDate, Now.ToString("MMdd"))
        pContent = pContent.Replace(pPeriodToYear, Now.ToString("yyy"))

        Dim pArray As String()

        If vMode = "AP" Then
            Dim pDsPayment = WebRequest.Instance.Business.TransReceivingPayment.GetPaymentData(vPaymentID)
            Dim pDtMaster = pDsPayment.Tables("Master")

            Dim pOrgShort = New OrganizationShort
            Dim pDtOrgShort = pOrgShort.GetProspectOrgDetails(CStr2(pDtMaster.Rows(0)("SupplierID")))
            If pDtOrgShort.Rows.Count > 0 Then
                pContent = pContent.Replace(pPayeeName, CStr2(pDtOrgShort.Rows(0)("CompanyName")))
                pContent = pContent.Replace(pPayeeAddress, CStr2(pDtOrgShort.Rows(0)("Address")))
                pArray = Client.StringToArray(CStr2(pDtOrgShort.Rows(0)("TIN")).PadRight(12), 3, 4)
                pContent = pContent.Replace(pPayeeTin1, pArray(0))
                pContent = pContent.Replace(pPayeeTin2, pArray(1))
                pContent = pContent.Replace(pPayeeTin3, pArray(2))
                pContent = pContent.Replace(pPayeeTin4, pArray(3))
                pContent = pContent.Replace(pPayeeZipCode, CStr2(pDtOrgShort.Rows(0)("Zip")))
            End If

            pDtOrgShort = pOrgShort.GetProspectOrgDetails(WebRequest.Instance.OrgID)
            If pDtOrgShort.Rows.Count > 0 Then
                pContent = pContent.Replace(pPayorName, CStr2(pDtOrgShort.Rows(0)("CompanyName")))
                pContent = pContent.Replace(pPayorAddress, CStr2(pDtOrgShort.Rows(0)("Address")))
                pArray = Client.StringToArray(CStr2(pDtOrgShort.Rows(0)("TIN")).PadRight(12), 3, 4)
                pContent = pContent.Replace(pPayorTin1, pArray(0))
                pContent = pContent.Replace(pPayorTin2, pArray(1))
                pContent = pContent.Replace(pPayorTin3, pArray(2))
                pContent = pContent.Replace(pPayorTin4, pArray(3))
                pContent = pContent.Replace(pPayorZipCode, CStr2(pDtOrgShort.Rows(0)("Zip")))
            End If
        End If
        pZip.AddString(pContent, "xl\drawings\drawing1.xml")

        Const pIncomePayments As String = "356MJCG33J"
        Const pAtc As String = "VDXAVBDYDH"
        Const pCellAmount1 As String = "PHHRUFFPBP"
        Const pCellAmount2 As String = "DA5TFUPGWT"
        Const pCellAmount3 As String = "T7BAXVNN9K"
        pContent = pZip.GetString("xl\sharedStrings.xml")

        pContent = pContent.Replace(pIncomePayments, "Purchases")
        pContent = pContent.Replace(pAtc, "WP. WC 158")
        Dim pSql = "SELECT SUM(TaxableAmount) AS TaxableAmount" & vbCrLf &
                " FROM TransReceivingPaymentOther" & vbCrLf &
                " WHERE PaymentType = 'Witholding Tax'" & vbCrLf &
                " AND PaymentID = " & ReadyString(vPaymentID)
        Dim pAmount = CDec2(mProData.ExecuteScalar(pSql))
        pContent = pContent.Replace(pAtc, "WP. WC 158")
        pContent = pContent.Replace(pCellAmount1, Format(pAmount, cGlobal.FormatCurrencyGrid))
        pContent = pContent.Replace(pCellAmount2, Format(pAmount, cGlobal.FormatCurrencyGrid))
        pContent = pContent.Replace(pCellAmount3, Format(pAmount, cGlobal.FormatCurrencyGrid))

        pZip.AddString(pContent, "xl\sharedStrings.xml")

        Return pBaseFilename
        'Return File(pFilename, "application/xlsx")

    End Function

    Public Function GetAvailableUnappliedCreditsDetails(vVendorID As String, vFilter As String) As DataTable

        Dim pWhere As String = ""
        If vFilter = "all" Then
            pWhere &= "AND ISNULL(TransReceivingPayment.UnappliedPayment, 0) > 0" & vbCrLf
        Else
            pWhere &= "AND ISNULL(TransReceivingPayment.UnappliedPayment, 0) - ISNULL(tblAppliedCredits.Amount, 0) > 0" & vbCrLf
        End If
        Dim pSql = " -- GetAvailableUnappliedCredits receiving payment available unapplied credits" & vbCrLf &
            " SELECT TransReceivingPayment.LastUpdated" & vbCrLf &
            " ,TransReceivingPayment.ID AS SourcePaymentID" & vbCrLf &
            " ,TransReceivingPayment.UnappliedPayment" & vbCrLf &
            " ,ISNULL(tblAppliedCredits.Amount, 0) AS ConsumedAmount" & vbCrLf &
            " ,ISNULL(TransReceivingPayment.UnappliedPayment, 0) - ISNULL(tblAppliedCredits.Amount , 0) AS AvailableAmount" & vbCrLf &
            " ,TransReceivingPayment.ManualReceiptNumber" & vbCrLf &
            " ,TransReceivingPayment.PaymentNum" & vbCrLf &
            " ,TransReceivingPayment.PaymentDate" & vbCrLf &
            " FROM TransReceivingPayment" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT SUM(TransReceivingPaymentAppliedCredits.Amount) AS Amount" & vbCrLf &
            "     FROM TransReceivingPaymentAppliedCredits" & vbCrLf &
            "     WHERE TransReceivingPaymentAppliedCredits.SourcePaymentID = TransReceivingPayment.ID" & vbCrLf &
            " ) tblAppliedCredits" & vbCrLf &
            " WHERE TransReceivingPayment.SupplierID = " & ReadyString(vVendorID) & vbCrLf &
            pWhere &
            " ORDER BY TransReceivingPayment.PaymentDate "
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetDestinationUnappliedCreditsDetails(vSourcePaymentID As String) As DataTable

        Dim pSql = "  -- GetDestinationUnappliedCreditsDetails" & vbCrLf &
            "  SELECT TransReceivingPayment.LastUpdated" & vbCrLf &
            "  ,TransReceivingPayment.ID AS SourcePaymentID" & vbCrLf &
            "  ,TransReceivingPayment.UnappliedPayment" & vbCrLf &
            "  ,TransReceivingPaymentAppliedCredits.Amount AS ConsumedAmount" & vbCrLf &
            "  ,TransReceivingPayment.ManualReceiptNumber" & vbCrLf &
            "  ,TransReceivingPayment.PaymentNum" & vbCrLf &
            "  ,ISNULL(TransReceivingPayment.ManualReceiptNumber, TransReceivingPayment.PaymentNum) AS PaymentNumText" & vbCrLf &
            "  ,TransReceivingPayment.PaymentDate" & vbCrLf &
            "  FROM TransReceivingPayment" & vbCrLf &
            "  JOIN TransReceivingPaymentAppliedCredits ON TransReceivingPaymentAppliedCredits.DestinationPaymentID = TransReceivingPayment.ID" & vbCrLf &
            "  WHERE TransReceivingPaymentAppliedCredits.SourcePaymentID = " & ReadyString(vSourcePaymentID) & vbCrLf &
            "  ORDER BY TransReceivingPayment.PaymentDate "
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSourceUnappliedCreditsDetails(vSourcePaymentID As String) As DataTable

        Dim pSql = "  -- GetDestinationUnappliedCreditsDetails" & vbCrLf &
            "  SELECT TransReceivingPayment.LastUpdated" & vbCrLf &
            "  ,TransReceivingPayment.ID AS SourcePaymentID" & vbCrLf &
            "  ,TransReceivingPayment.UnappliedPayment" & vbCrLf &
            "  ,TransReceivingPaymentAppliedCredits.Amount AS ConsumedAmount" & vbCrLf &
            "  ,TransReceivingPayment.ManualReceiptNumber" & vbCrLf &
            "  ,TransReceivingPayment.PaymentNum" & vbCrLf &
            "  ,ISNULL(TransReceivingPayment.ManualReceiptNumber, TransReceivingPayment.PaymentNum) AS PaymentNumText" & vbCrLf &
            "  ,TransReceivingPayment.PaymentDate" & vbCrLf &
            "  FROM TransReceivingPayment" & vbCrLf &
            "  JOIN TransReceivingPaymentAppliedCredits ON TransReceivingPaymentAppliedCredits.SourcePaymentID = TransReceivingPayment.ID" & vbCrLf &
            "  WHERE TransReceivingPaymentAppliedCredits.DestinationPaymentID = " & ReadyString(vSourcePaymentID) & vbCrLf &
            "  ORDER BY TransReceivingPayment.PaymentDate "
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetAvailableUnappliedCredits(vVendorID As String) As Decimal

        Dim pSql = " -- GetAvailableUnappliedCredits receiving payment available unapplied credits" & vbCrLf &
            " SELECT SUM(TransReceivingPayment.UnappliedPayment - ISNULL(tblAppliedCredits.Amount , 0)) AS AvailableAmount" & vbCrLf &
            " FROM TransReceivingPayment" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT SUM(TransReceivingPaymentAppliedCredits.Amount) AS Amount" & vbCrLf &
            "     FROM TransReceivingPaymentAppliedCredits" & vbCrLf &
            "     WHERE TransReceivingPaymentAppliedCredits.SourcePaymentID = TransReceivingPayment.ID" & vbCrLf &
            " ) tblAppliedCredits" & vbCrLf &
            " WHERE TransReceivingPayment.UnappliedPayment > 0" & vbCrLf &
            " AND TransReceivingPayment.SupplierID = " & ReadyString(vVendorID) & vbCrLf

        Dim pOut = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function
End Class
