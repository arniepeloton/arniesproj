Public Class LibraryAccountAdjustments

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Public Const ReturnedChequesID As String = "265F42A0-80F3-4DE0-8FE3-C5ECA158BF67"

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function Add(vID As String,
        vReason As String,
        vAdjustmentType As String,
        vEffect As String,
        vAccountType As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("LibraryAccountAdjustments")
        pIU("ID") = ReadyString(vID)
        pIU("Reason") = ReadyString(vReason)
        pIU("AdjustmentType") = ReadyString(vAdjustmentType)
        pIU("Effect") = ReadyString(vEffect)
        pIU("AccountType") = ReadyString(vAccountType)
        pIU("IsActive") = 1
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return vID

    End Function

    Public Sub AddTypesIfNeeded()

        AddWrongDataDebitIfNeeded()
        AddInvalidPaymentDebitIfNeeded()
        AddWrongDataCreditIfNeeded()
        AddReturnedCheckIfNeeded()
        AddWithholdingTaxIfNeeded()
        AddCashAdvanceIfNeeded()
        AddOtherCreditAdjustmentsIfNeeded()
        AddOtherDebitAdjustmentsIfNeeded()
        AddSalesDiscountIfNeeded()
        AddSalesReturnsIfNeeded()
        AddDeliveryChargeIfNeeded()
        AddLatePaymentChargesIfNeeded()
        AddOtherChargeIfNeeded()
        AddServiceChargeIfNeeded()

        AddIfNeeded("Adjustment Due to Wrong Data Input", "4C95E860-7772-4B6E-8186-D8B18A005C3E", False, True, False)
        AddIfNeeded("Cash Advance by Vendor or Their  Representative", "4C262E04-07A8-4B30-8A01-CD8700F0E37C", False, True, False)
        AddIfNeeded("Freight Receivable from Vendor", "4DC7DE88-CC68-42EC-B267-2A386737A550", False, True, False)
        AddIfNeeded("Other Debit Adjustments to AP", "F61EFCCE-C42B-4645-ADB5-00C06FD0EAEA", False, True, False)
        AddIfNeeded("Purchase Discount", "7EB20629-D675-4CC9-9703-977095290AA3", False, True, False)
        AddIfNeeded("Purchase Returns", "502FB020-4CEE-4610-BDBA-BE33AE0BFCB4", False, True, False)
        AddIfNeeded("Withholding Tax", "8A3027A9-9BE9-4E52-A5CC-674A23897D77", False, True, False)
        AddIfNeeded("Adjustment Due to Wrong Data Input", "9AE23566-C152-4D75-8A85-47AE8018D772", True, False, False)
        AddIfNeeded("Late Payment Charges In Favor of Vendor", "7CD318B6-2415-475A-8E43-948B9EE6F782", True, False, False)
        AddIfNeeded("Other Charges by Vendor", "5EC4B14D-4BFB-43B8-A29F-9F588CA39513", True, False, False)
        AddIfNeeded("Other Credit Adjustments to AP", "6B098D29-0527-43A3-8174-269FDC6336F5", True, False, False)
        AddIfNeeded("Returned Cheques/ Stop Payment", "639B43B8-EB0C-4AC8-A06A-EF1ACD9627CF", True, False, False)

    End Sub

    Private Sub AddOtherCreditAdjustmentsIfNeeded()

        Const pReason As String = "Other Credit Adjustments to AR"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="56CA4E72-6514-47EF-BED8-107D8D7F247B",
                vReason:=pReason,
                vAdjustmentType:="Credit Memo",
                vEffect:="Deductive (-)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddOtherDebitAdjustmentsIfNeeded()

        Const pReason As String = "Other Debit Adjustments to AR"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="BEF6B08E-2557-4CAF-8483-F2939FC3AD5D",
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (-)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Public Function AddInvalidPaymentDebitIfNeeded() As String

        Const pReason As String = "Invalid Payment"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason) & vbCrLf &
            " AND AdjustmentType = 'Debit Memo'"
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return Add(vID:="846811C7-03B6-45CD-A539-5B2D89BF290E",
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (+)",
                vAccountType:="ar"
            )
        Else
            Return CStr2(pDR("ID"))
        End If

    End Function

    Public Function AddWrongDataDebitIfNeeded() As String

        Const pReason As String = "Adjustment Due to Wrong Data Input"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason) & vbCrLf &
            " AND AdjustmentType = 'Debit Memo'"
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return Add(vID:="0DC4249A-DFEB-4614-9715-CFDD09FD92B3",
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (+)",
                vAccountType:="ar"
            )
        Else
            Return CStr2(pDR("ID"))
        End If

    End Function

    Private Sub AddWrongDataCreditIfNeeded()

        Const pReason As String = "Adjustment Due to Wrong Data Input"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason) & vbCrLf &
            " AND AdjustmentType = 'Credit Memo'"
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="010A8087-4369-4833-AA0D-98B63CBBC30B",
                vReason:=pReason,
                vAdjustmentType:="Credit Memo",
                vEffect:="Deductive (-)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddSalesReturnsIfNeeded()

        Const pReason As String = "Sales Returns"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason) & vbCrLf &
            " AND AdjustmentType = 'Credit Memo'"
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="{DA0FB89C-D5F4-46AB-B57B-AFFAD0BD4DC1}",
                vReason:=pReason,
                vAdjustmentType:="Credit Memo",
                vEffect:="Deductive (-)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddSalesDiscountIfNeeded()

        Const pReason As String = "Sales Discount"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason) & vbCrLf &
            " AND AdjustmentType = 'Credit Memo'"
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="{F476B0BF-044F-449E-A391-F24E7C6FDB25}",
                vReason:=pReason,
                vAdjustmentType:="Credit Memo",
                vEffect:="Deductive (-)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddReturnedCheckIfNeeded()

        Const pReason As String = "Returned Checks"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:=ReturnedChequesID,
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (+)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddDeliveryChargeIfNeeded()

        Const pReason As String = "Delivery Charge"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="09080D4B-8447-4DCC-960C-8F741F1F1CE0",
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (+)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddLatePaymentChargesIfNeeded()

        Const pReason As String = "Late Payment Charges"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="{57CB9B75-2518-4061-89D4-B7FEB6B4ABBD}",
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (+)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddOtherChargeIfNeeded()

        Const pReason As String = "Other Charge"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="7BBB6B15-CA0D-4D08-AB14-F1A760B712E0",
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (+)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddServiceChargeIfNeeded()

        Const pReason As String = "Service Charge"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="07A7B21F-3E90-4745-B886-3E38C863C27D",
                vReason:=pReason,
                vAdjustmentType:="Debit Memo",
                vEffect:="Additive (+)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddWithholdingTaxIfNeeded()

        Const pReason As String = "Withholding Tax"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="789C66A6-649C-4521-A1B4-6B26425C5B22",
                vReason:=pReason,
                vAdjustmentType:="Credit Memo",
                vEffect:="Deductive (-)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddCashAdvanceIfNeeded()

        Const pReason As String = "Cash Advance"
        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:="905AFA22-1DA3-4EAB-9D49-34A2B85A49AB",
                vReason:=pReason,
                vAdjustmentType:="Credit Memo",
                vEffect:="Deductive (-)",
                vAccountType:="ar"
            )
        End If

    End Sub

    Private Sub AddIfNeeded(vReason As String, vID As String, vIsCreditMemo As Boolean, vIsDeductive As Boolean, vIsAr As Boolean)

        Dim pReason As String = vReason
        Dim pSql As String
        Dim pDR As DataRow
        Dim pAdjustmentType As String
        Dim pEffect As String
        Dim pAccountType As String

        If vIsCreditMemo Then
            pAdjustmentType = "Credit Memo"
        Else
            pAdjustmentType = "Debit Memo"
        End If

        If vIsDeductive Then
            pEffect = "Deductive (-)"
        Else
            pEffect = "Additive (-)"
        End If

        If vIsAr Then
            pAccountType = "ar"
        Else
            pAccountType = "ap"
        End If

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(pReason)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Add(vID:=vID,
                vReason:=pReason,
                vAdjustmentType:=pAdjustmentType,
                vEffect:=pEffect,
                vAccountType:=pAccountType
            )
        End If

    End Sub

    Public Function IsAdjustmentDeductive(vID As String) As Boolean

        Dim pDR As DataRow
        pDR = GetLibraryAdjustment(vID)
        If pDR Is Nothing Then
            Throw New ApplicationException("The value passed to IsAdjustmentDeductive does not exist in LibraryAccountAdjustments.")
        End If
        Return CStr2(pDR("Effect")).Contains("Deductive")

    End Function

    Public Function GetLibraryAdjustment(vID As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT Effect" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetByID(vID As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT Reason" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetByReason(vReason) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryAccountAdjustments" & vbCrLf &
            " WHERE Reason = " & ReadyString(vReason)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class

