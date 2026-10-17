Public Class SystemDefaults

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetOrAddSystemDefault(ByVal vDescription As String, ByVal vDefaultValue As String) As String

        Dim pDT As DataTable
        Dim pSql As String
        Dim pInsertUpdate As InsertUpdate

        pSql = "SELECT DefaultValue " &
            " FROM SystemDefaults" &
            " WHERE Description = " & ReadyString(vDescription)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            pInsertUpdate = New InsertUpdate
            With pInsertUpdate
                .TableName = "SystemDefaults"
                .Value("Description") = ReadyString(vDescription)
                .Value("DefaultValue") = ReadyString(vDefaultValue)
                .Value("LastUpdated") = "GetDate()"
                mProData.ExecuteSQL(.GetInsert())
                Return vDefaultValue
            End With
        Else
            Return pDT.Rows(0)("DefaultValue") & ""
        End If

        Return ""

    End Function

    Public Function GetOrAddSystemDefaultPublic(ByVal vDescription As String, ByVal vDefaultValue As String) As String

        Dim pDT As DataTable
        Dim pSql As String
        Dim pInsertUpdate As InsertUpdate

        pSql = "SELECT DefaultValue " &
            " FROM MobizSystemDefaults" &
            " WHERE Description = " & ReadyString(vDescription)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            pInsertUpdate = New InsertUpdate
            With pInsertUpdate
                .TableName = "MobizSystemDefaults"
                .Value("Description") = ReadyString(vDescription)
                .Value("DefaultValue") = ReadyString(vDefaultValue)
                .Value("LastUpdated") = "GetDate()"
                mProData.ExecuteSQL(.GetInsert())
                Return vDefaultValue
            End With
        Else
            Return pDT.Rows(0)("DefaultValue") & ""
        End If

        Return ""

    End Function

    Public Function GetSystemDefault(ByVal vDescription As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DefaultValue " &
            " FROM SystemDefaults" &
            " WHERE Description = " & ReadyString(vDescription)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)("DefaultValue") & ""
        Else
            Return ""
        End If

    End Function

    Public Sub SetSystemDefault(ByVal vDescription As String, ByVal vDefaultValue As String)

        Dim pInsertUpdate As New InsertUpdate

        With pInsertUpdate
            .TableName = "SystemDefaults"
            .Value("DefaultValue") = ReadyString(vDefaultValue)
            .Value("LastUpdated") = "GetDate()"
            If mProData.ExecuteSQL(.GetUpdate("Description = " & ReadyString(vDescription))) = 0 Then
                .Value("Description") = ReadyString(vDescription)
                .Value("LastUpdated") = "GetDate()"
                mProData.ExecuteSQL(.GetInsert())
            End If
        End With

    End Sub

    Public Function GetSystemDefaultPublic(ByVal vDescription As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DefaultValue " &
            " FROM MobizSystemDefaults" &
            " WHERE Description = " & ReadyString(vDescription)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)("DefaultValue") & ""
        Else
            Return ""
        End If

    End Function

    Public Sub SetSystemDefaultPublic(ByVal vDescription As String, ByVal vDefaultValue As String)

        Dim pInsertUpdate As New InsertUpdate

        With pInsertUpdate
            .TableName = "MobizSystemDefaults"
            .Value("DefaultValue") = ReadyString(vDefaultValue)
            .Value("LastUpdated") = "GetDate()"
            If mProData.ExecuteSQL(.GetUpdate("Description = " & ReadyString(vDescription))) = 0 Then
                .Value("Description") = ReadyString(vDescription)
                .Value("LastUpdated") = "GetDate()"
                mProData.ExecuteSQL(.GetInsert())
            End If
        End With

    End Sub

    Public Function GetShipmentMode() As String

        Return GetSystemDefault("Preferred mode of shipment")

    End Function

    Public Function CapitalPriceLevels(ByVal vIncludeNames As Boolean) As DataTable

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pPriceLevelId As String
        Dim pSql As String
        Dim pDtPrice As DataTable
        Dim pItems As List(Of String())
        Dim pItem As String()

        pDT = New DataTable
        With pDT.Columns
            .Add("ID", GetType(Guid))
            .Add("Name")
        End With

        pItems = New List(Of String())
        pItems.Add(New String() {"Factory Price ID", "{01000012-0002-0000-0000-000000000000}"})
        pItems.Add(New String() {"Freight Cost ID", "{01000012-0001-0000-0000-000000000000}"})
        pItems.Add(New String() {"Landed Cost ID", "{01000012-0003-0000-0000-000000000000}"})

        For Each pItem In pItems
            pPriceLevelId = mParent.SystemDefaults.GetOrAddSystemDefault(pItem(0), pItem(1))
            If IsGuid(pPriceLevelId) _
                AndAlso vIncludeNames Then

                pSql = "SELECT PriceLevelName" &
                    " FROM LibraryPriceLevels" &
                    " WHERE ID = " & ReadyString(pPriceLevelId)
                pDtPrice = mProData.Execute(pSql)
                If pDtPrice.Rows.Count > 0 Then
                    pDR = pDT.NewRow
                    pDR("ID") = pPriceLevelId
                    pDR("Name") = pDtPrice.Rows(0)("PriceLevelName")
                    pDT.Rows.Add(pDR)
                End If
            End If
        Next pItem

        Return pDT

    End Function

    Public Function GetPurchaserValues() As DataTable

        Dim pDT, pDtOut As DataTable
        Dim pDR As DataRow
        Dim pSql As String

        pDtOut = New DataTable
        With pDtOut
            With .Columns
                .Add("ID", GetType(Guid))
                .Add("Description")
                .Add("DefaultValue")     ' value in the database
                .Add("Remarks")
                .Add("Display") ' displayed value
            End With
            With .Rows
                .Add(Nothing, "Supplier Lead Time", "30", "in days")
                .Add(Nothing, "Shipping Lead Time", "30", "in days")
                .Add(Nothing, "Purchasing Lead Time", "30", "in days")
                .Add(Nothing, "Invoice Type for Branch Auto Invoice", "", "")
                .Add(Nothing, "Autoprint Invoice PC", "", "Computer name that will autoprint BT's")
                .Add(Nothing, "DRP Tables Recomputation Frequency", 24, "hours")
                .Add(Nothing, "Length of forecast time  horizon", 1, "weeks")
                .Add(Nothing, "PO follow up interval", "", "%, separate each item by comma (,)")
            End With
        End With

        For Each pDR In pDtOut.Rows
            If pSql IsNot Nothing Then pSql &= ","
            pSql &= ReadyString(pDR("Description"))
        Next pDR

        pSql = "SELECT ID" &
            ",Description" &
            ",DefaultValue" &
            " FROM SystemDefaults" &
            " WHERE Description IN (" & pSql & ")"
        pDT = mProData.Execute(pSql)

        For Each pDR In pDT.Rows
            pDtOut.DefaultView.RowFilter = "Description = " & ReadyString(pDR("Description"))
            If pDtOut.DefaultView.Count > 0 Then
                pDtOut.DefaultView(0)("ID") = pDR("ID")
                pDtOut.DefaultView(0)("DefaultValue") = pDR("DefaultValue")
#If DEBUG Then
            Else
                Stop2() ' impossible
#End If
            End If
        Next pDR

        pDtOut.DefaultView.RowFilter = ""
        pDtOut.AcceptChanges()
        For Each pDR In pDtOut.Rows
            Select Case pDR("Description")
                Case "Invoice Type for Branch Auto Invoice"
                    If InvoiceTypeForBranchAutoInvoice(Nothing, pSql, Nothing) Then
                        pDR("Display") = pSql
                    End If

                Case Else
                    pDR("Display") = pDR("DefaultValue")

            End Select
        Next pDR
        pDtOut.AcceptChanges()

        Return pDtOut

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oInvoiceDefaultID">MainInvoiceDefaults.ID</param>
    ''' <param name="oInvoiceType">MainInvoiceDefaults.Abbrevation</param>
    ''' <param name="oError"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InvoiceTypeForBranchAutoInvoice(ByRef oInvoiceDefaultID As String, ByRef oInvoiceType As String,
        ByRef oError As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String
        Dim pID As String

        pID = GetSystemDefault("Invoice Type for Branch Auto Invoice")
        If IsGuid(pID) = False Then
            oError = """Invoice Type for Branch Auto Invoice"" has not been set in Default Settings. Please set it first then try again"
            Return False
        End If

        pSql = "SELECT Abbrevation" &
            " FROM MainInvoiceDefaults" &
            " WHERE ID = " & ReadyString(pID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            oError = "Please re-set ""Invoice Type for Branch Auto Invoice"" in Default Settings"
            Return False
        End If

        oInvoiceDefaultID = pID
        oInvoiceType = pDT.Rows(0)(0)

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oInvoiceDefaultID">MainInvoiceDefaults.ID</param>
    ''' <param name="oInvoiceType">MainInvoiceDefaults.Abbrevation</param>
    ''' <param name="oError"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InvoiceTypeForCustomerAutoInvoice(ByRef oInvoiceDefaultID As String, ByRef oInvoiceType As String,
        ByRef oError As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String
        Dim pID As String

        pID = GetSystemDefault("Invoice Type for Customer Auto Invoice")
        If IsGuid(pID) = False Then
            oError = """Invoice Type for Customer Auto Invoice"" has not been set in Default Settings. Please set it first then try again"
            Return False
        End If

        pSql = "SELECT Abbrevation" &
            " FROM MainInvoiceDefaults" &
            " WHERE ID = " & ReadyString(pID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            oError = "Please re-set ""Invoice Type for Customer Auto Invoice"" in Default Settings"
            Return False
        End If

        oInvoiceDefaultID = pID
        oInvoiceType = pDT.Rows(0)(0)

        Return True

    End Function

    Public Function GetCompanyName() As String

        Return GetSystemDefault("Company Name")

    End Function

    Public Function GetCompanyAddress() As String

        Return GetSystemDefault("Company Address")

    End Function

    Public Function GetCompanyTin() As String

        Return GetSystemDefault("Company TIN")

    End Function

    Public Function GetCompanyContactNumbers() As String

        Return GetSystemDefault("Company Contact No")

    End Function

    ''' <summary>
    ''' This is the path used by pictures of stock viewer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPicturesPath() As String

        Return GetSystemDefault("Pictures Path")

    End Function

    Public Function GetWholesalePriceID() As String

        Return GetOrAddSystemDefault("Wholesale Price ID", "{01000012-0004-0000-0000-000000000000}")

    End Function

    Public Function GetRetailPriceID() As String

        Return GetOrAddSystemDefault("Retail Price ID", "{01000012-0005-0000-0000-000000000000}")

    End Function

    Public Function GetLandedCostID() As String

        Return GetOrAddSystemDefault("Landed Cost ID", "01000012-0003-0000-0000-000000000000")

    End Function

    Public Function GetSrpID() As String

        Return GetOrAddSystemDefault("SRP (Suggested Retail Price) ID", "EEF716C6-B290-4B28-B2A6-0A4F52CF4989")

    End Function

    Public Function GetCurrency() As Dictionary(Of String, String)

        Dim pDT As DataTable
        Dim pOut As Dictionary(Of String, String)
        Dim pSql As String
        Dim pAlphaCode As String

        pAlphaCode = GetSystemDefault("Default Currency")
        If pAlphaCode.Length > 0 Then
            pSql = "SELECT Entity, Currency, AlphaCode" &
                " FROM LibraryCurrencies" &
                " WHERE AlphaCode = " & ReadyString(pAlphaCode)
            pDT = mProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                pOut = New Dictionary(Of String, String)
                pOut.Add("Entity", pDT.Rows(0)("Entity"))
                pOut.Add("Currency", pDT.Rows(0)("Currency"))
                pOut.Add("AlphaCode", pDT.Rows(0)("AlphaCode"))
                Return pOut
            End If
        End If

    End Function

    Public Property VendorLeadTime As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Vendor Lead Time")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Vendor Lead Time", "")
            Else
                SetSystemDefault("Vendor Lead Time", value)
            End If

        End Set
    End Property

    Public Property LotSize As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Default lot size quantity")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Default lot size quantity", "")
            Else
                SetSystemDefault("Default lot size quantity", value)
            End If

        End Set
    End Property

    Public Property LotSizeUnit As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Default lot size unit")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Default lot size unit", "")
            Else
                SetSystemDefault("Default lot size unit", value)
            End If

        End Set
    End Property

    Public Property MinumumOrderQuantity As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Minumum order quantity")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Minumum order quantity", "")
            Else
                SetSystemDefault("Minumum order quantity", value)
            End If

        End Set
    End Property

    Public Property MinumumOrderUnit As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Minumum order unit")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Minumum order unit", "")
            Else
                SetSystemDefault("Minumum order unit", value)
            End If

        End Set
    End Property

    Public Property ForecastHorizon As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Forecast horizon")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Forecast horizon", "")
            Else
                SetSystemDefault("Forecast horizon", value)
            End If

        End Set
    End Property

    Public Property PoProcessingLeadTime As Integer?
        Get

            Dim pStr As String

            pStr = GetOrAddSystemDefault("Po Processing Lead Time", 4)
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Po Processing Lead Time", "")
            Else
                SetSystemDefault("Po Processing Lead Time", value)
            End If

        End Set
    End Property

    Public Property ShippingLeadTime As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Shipping Lead Time")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Shipping Lead Time", "")
            Else
                SetSystemDefault("Shipping Lead Time", value)
            End If

        End Set
    End Property

    Public Property SafetyStockDuration As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Safety stock duration")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Safety stock duration", "")
            Else
                SetSystemDefault("Safety stock duration", value)
            End If

        End Set
    End Property

    Public Property FirmOrderWeeks As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Make firm orders for planned orders in X weeks")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Make firm orders for planned orders in X weeks", "")
            Else
                SetSystemDefault("Make firm orders for planned orders in X weeks", value)
            End If

        End Set
    End Property

    Public Property POPermanentRemark As String
        Get

            Dim pStr As String

            pStr = GetSystemDefault("PO Permanent Remark")

            Return pStr

        End Get
        Set(value As String)

            If value Is Nothing Then
                SetSystemDefault("PO Permanent Remark", "")
            Else
                SetSystemDefault("PO Permanent Remark", value)
            End If

        End Set
    End Property

    '''<summary>
    ''' for in-store transactions
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPosBranch() As Dictionary(Of String, String)

        Dim pStr As String
        Dim pInt As Integer
        Dim pDT As DataTable
        Dim pOut As Dictionary(Of String, String)

        pStr = GetSystemDefault("POS Branch")
        If Integer.TryParse(pStr, pInt) Then
            pStr = "SELECT ID" &
                ",Name" &
                " FROM DavaoUnifiedPayroll.dbo.LibraryBranch" &
                " WHERE ID = " & pInt
            pDT = mProData.Execute(pStr)
            If pDT.Rows.Count > 0 Then
                pOut = New Dictionary(Of String, String)
                pOut.Add("BranchID", pDT.Rows(0)("ID"))
                pOut.Add("BranchName", pDT.Rows(0)("Name"))
                Return pOut
            End If
        End If

    End Function

    Public Property POSInvoiceDefaultID() As String
        Get

            Return GetSystemDefault("POS Invoice Default ID")

        End Get
        Set(value As String)

            SetSystemDefault("POS Invoice Default ID", value)

        End Set
    End Property

    'Public Property PosSubscriberCustomerID As String
    '    Get

    '        Return GetSystemDefault("POS Subscriber Customer ID")

    '    End Get
    '    Set(value As String)

    '        SetSystemDefault("POS Subscriber Customer ID", value)

    '    End Set
    'End Property

    Public Property SalesReturnRestockingFeePercent As Decimal
        Get

            Return 0 ' temporarily disabled
            'Return CDec2(GetSystemDefault("Sales Return Restocking Fee Percent"))

        End Get
        Set(value As Decimal)

            SetSystemDefault("Sales Return Restocking Fee Percent", value)

        End Set
    End Property

    Public Property SalesReturntMaximumReturnablePeriod As Decimal
        Get

            Return CDec2(GetSystemDefault("Days returnable"))

        End Get
        Set(value As Decimal)

            SetSystemDefault("Days returnable", value)

        End Set
    End Property

    Public Property SalesReturntReturnedPrice As String
        Get

            Return GetSystemDefault("Sales Return Returned Price")

        End Get
        Set(value As String)

            SetSystemDefault("Sales Return Returned Price", value)

        End Set
    End Property

    Public Property SalesReturntPrintRefundSignUp As Boolean
        Get

            Return CDec2(GetSystemDefault("Sales Returnt Print Refund SignUp")) <> 0

        End Get
        Set(value As Boolean)

            SetSystemDefault("Sales Returnt Print Refund SignUp", If(value, 1, 0))

        End Set
    End Property

    Public Property HideMerchandiseExchange As Boolean
        Get

            Return CDec2(GetSystemDefault("HideMerchandiseExchange")) <> 0

        End Get
        Set(value As Boolean)

            SetSystemDefault("HideMerchandiseExchange", If(value, 1, 0))

        End Set
    End Property

    Public Property RestrictedTransactionLookupForSalesReturn As Boolean
        Get

            Return CDec2(GetSystemDefault("RestrictedTransactionLookupForSalesReturn")) <> 0

        End Get
        Set(value As Boolean)

            SetSystemDefault("RestrictedTransactionLookupForSalesReturn", If(value, 1, 0))

        End Set
    End Property

    Public Property AllowedReturnDuration As Decimal
        Get

            Return CDec2(GetSystemDefault("AllowedReturnDuration"))

        End Get
        Set(value As Decimal)

            SetSystemDefault("AllowedReturnDuration", value)

        End Set
    End Property

    Public Property HideSomeSopaFields As Boolean
        Get

            Return CBool2(GetSystemDefault("HideSomeSopaFields"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("HideSomeSopaFields", If(value, 1, 0))

        End Set
    End Property

    Public Property DeliveryMethodWhenAutoPO As String
        Get

            Return GetSystemDefault("DeliveryMethodWhenAutoPO")

        End Get
        Set(value As String)

            SetSystemDefault("DeliveryMethodWhenAutoPO", value)

        End Set
    End Property

    Public Property SalesReturntSRVSettings As String
        Get

            Return GetSystemDefault("Sales Return Voucher Use Settings")

        End Get
        Set(value As String)

            SetSystemDefault("Sales Return Voucher Use Settings", value)

        End Set
    End Property

    Public Property POSMode As String
        Get

            Return GetSystemDefault("POS Mode")

        End Get
        Set(value As String)

            SetSystemDefault("POS Mode", value)

        End Set
    End Property

    Public Property IsEnableDiningMode As String
        Get

            Return CDec2(GetSystemDefault("Enable Dining Mode"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Enable Dining Mode", value)

        End Set
    End Property

    Public Property IsEnableBillingOption As String
        Get

            Return CDec2(GetSystemDefault("Enable Billing Option"))

        End Get
        Set(value As String)

            SetSystemDefault("Enable Billing Option", value)

        End Set
    End Property
    Public Property IsEnableVATExemptOption As String
        Get

            Return CDec2(GetSystemDefault("Enable VAT Exempt Option"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Enable VAT Exempt Option", value)

        End Set
    End Property
    Public Property WhenDoYouWantToPrintOrderSlip As String
        Get

            Return GetSystemDefault("When Do You Want To Print Order Slip")

        End Get
        Set(value As String)

            SetSystemDefault("When Do You Want To Print Order Slip", value)

        End Set
    End Property

    Public Property IsPrintOrderSlip As String
        Get

            Return CDec2(GetSystemDefault("Print Order Slip"))

        End Get
        Set(value As String)

            SetSystemDefault("Print Order Slip", value)

        End Set
    End Property

    Public Property IsShowTableNo As Boolean
        Get

            Return CDec2(GetSystemDefault("IsShowTableNo")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("IsShowTableNo", If(value, 1, 0))

        End Set
    End Property

    Public Property IsShowGasPumpNo As Boolean
        Get

            Return CDec2(GetSystemDefault("IsShowGasPumpNo")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("IsShowGasPumpNo", If(value, 1, 0))

        End Set
    End Property

    Public Property EcommerceIsShowVendor3rdPartyCustomerName As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Is Show Vendor 3rd Party Customer Name"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Is Show Vendor 3rd Party Customer Name", value)

        End Set
    End Property

    Public Property VendorPortalSigninURL As String
        Get

            Return CStr2(GetSystemDefault("Vendor Portal Signin URL"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Vendor Portal Signin URL", value)

        End Set
    End Property

    Public Property EcommerceShowVendorName As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Show Vendor Name"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Show Vendor Name", value)

        End Set
    End Property

    Public Property EcommerceIsAllowBuyersSelectFromVendor As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Is Allow Buyers Select From Vendor"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Is Allow Buyers Select From Vendor", value)

        End Set
    End Property

    Public Property EcommerceDefaultFOWST As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Default FOWST"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Default FOWST", value)

        End Set
    End Property

    Public Property EcommerceDataSourceServerIP As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Data Source Server IP"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Data Source Server IP", value)

        End Set
    End Property

    Public Property EcommerceOrderDataTargetServerIP As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Order Data Target Server IP"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Order Data Target Server IP", value)

        End Set
    End Property

    Public Property EcommerceAcceptDeliveryOrders As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Accept Delivery Orders"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Accept Delivery Orders", value)

        End Set
    End Property

    Public Property EcommerceAllowPickupOrders As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Allo Pick-up Orders"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Allo Pick-up Orders", value)

        End Set
    End Property

    Public Property EcommerceCheckoutMessage As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Checkout Message"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Checkout Message", value)

        End Set
    End Property

    Public Property EcommercePaymentMethodPaypal As String
        Get

            Return CStr2(GetSystemDefault("Ecommerce Payment Method Paypal"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Ecommerce Payment Method Paypal", value)

        End Set
    End Property

    Public Property DormantSKUsPeriod As String
        Get
            'in days
            Return CStr2(GetSystemDefault("DormantSKUsPeriod"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("DormantSKUsPeriod", value)

        End Set
    End Property

    Public Property IsAutoCleanUpDormantSKUs As String
        Get

            Return CStr2(GetSystemDefault("IsAutoCleanUpDormantSKUs"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("IsAutoCleanUpDormantSKUs", value)

        End Set
    End Property

    Public Property IsAutoPrintOrders As String
        Get

            Return CStr2(GetSystemDefault("Auto-Print Orders that are ready for invoicing"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Auto-Print Orders that are ready for invoicing", value)

        End Set
    End Property


    Public Property NoOfTables As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("NoOfTables")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(ByVal value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("NoOfTables", "")
            Else
                SetSystemDefault("NoOfTables", value)
            End If

        End Set
    End Property

    Public Property NoOfGasPumps As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("NoOfGasPumps")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(ByVal value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("NoOfGasPumps", "")
            Else
                SetSystemDefault("NoOfGasPumps", value)
            End If

        End Set
    End Property

    Public Property IsEnableInventoryCountMode As Boolean
        Get

            Return CDec2(GetSystemDefault("IsEnableInventoryCountMode")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("IsEnableInventoryCountMode", If(value, 1, 0))

        End Set
    End Property

    Public Property RequireSignature As Boolean
        Get

            Return CDec2(GetSystemDefault("Require Signature in Sales Order Taker")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Require Signature in Sales Order Taker", If(value, 1, 0))

        End Set
    End Property

    Public Property RequireCustomerOutlet As Boolean
        Get

            Return CBool2(GetSystemDefault("RequireCustomerOutlet"))

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("RequireCustomerOutlet", If(value, 1, 0))

        End Set
    End Property

    Public Property RequireCustomerDepartment As Boolean
        Get

            Return CBool2(GetSystemDefault("RequireCustomerDepartment"))

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("RequireCustomerDepartment", If(value, 1, 0))

        End Set
    End Property

    Public Property ShowQtyAvailableToCommit As Boolean
        Get

            Return CBool2(GetSystemDefault("ShowQtyAvailableToCommit"))

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("ShowQtyAvailableToCommit", If(value, 1, 0))

        End Set
    End Property

    Public Property IsAutomaticallyReceivedStocks As Boolean
        Get

            Return CDec2(GetSystemDefault("Automatically Received Stocks")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Automatically Received Stocks", If(value, 1, 0))

        End Set
    End Property

    Public Property IsShowQuantities As Boolean
        Get

            Return CDec2(GetSystemDefault("Show Stock Quantities")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Show Stock Quantities", If(value, 1, 0))

        End Set
    End Property

    Public Property IsExternalInReceiveIn As Boolean
        Get

            Return CDec2(GetSystemDefault("Receive From/Transfer To")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Receive From/Transfer To", If(value, 1, 0))

        End Set
    End Property

    'Public Property OrganizationReceiveFrom As Boolean
    '    Get

    '        Return CDec2(GetSystemDefault("Default Organization Receive From")) <> 0

    '    End Get
    '    Set(ByVal value As Boolean)

    '        SetSystemDefault("Default Organization Receive From", If(value, 1, 0))

    '    End Set
    'End Property


    Public Property IsThirdPartyCompanyOrOrganization As Boolean
        Get

            Return CDec2(GetSystemDefault("Receive In/Transfer From")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Receive In/Transfer From", If(value, 1, 0))

        End Set
    End Property

    Public Property IsAutomaticEffect As Boolean
        Get

            Return CDec2(GetSystemDefault("Inventory Adjustment Effect")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Inventory Adjustment Effect", If(value, 1, 0))

        End Set
    End Property

    Public Property IsSaveDailyBeginningBalance As Boolean
        Get

            Return CDec2(GetSystemDefault("Save Daily Beginning Balance")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Save Daily Beginning Balance", If(value, 1, 0))

        End Set
    End Property

    Public Property IsAllowDecimalinInventoryCount As Boolean
        Get

            Return CDec2(GetSystemDefault("Allow decimal points in Inventory Count")) <> 0

        End Get
        Set(ByVal value As Boolean)

            SetSystemDefault("Allow decimal points in Inventory Count", If(value, 1, 0))

        End Set
    End Property

    Public Property DefaultInventoryOption As String
        Get
            Return GetSystemDefault("DefaultInventoryOption")
        End Get
        Set(ByVal value As String)
            SetSystemDefault("DefaultInventoryOption", value)
        End Set
    End Property

    Public Property ShowDefaulUomInventoryCount As Boolean
        Get
            Return CDec2(GetSystemDefault("ShowDefaulUomInventoryCount")) <> 0
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowDefaulUomInventoryCount", If(value, 1, 0))
        End Set
    End Property

    Public Property AllowBasedUomInventoryCount As Boolean
        Get
            Return CDec2(GetSystemDefault("AllowBasedUomInventoryCount")) <> 0
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("AllowBasedUomInventoryCount", If(value, 1, 0))
        End Set
    End Property

    Public Property DontAllowViewZreadWhenNoInventory As Boolean
        Get
            Return CDec2(GetSystemDefault("DontAllowViewZreadWhenNoInventory"))
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("DontAllowViewZreadWhenNoInventory", If(value, 1, 0))
        End Set
    End Property

    Public Property FranchiseRedFlagHours As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("Franchise Red Flag Hours")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(ByVal value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("Franchise Red Flag Hours", "")
            Else
                SetSystemDefault("Franchise Red Flag Hours", value)
            End If

        End Set

    End Property

    Public Property EnforceZRead As Boolean
        Get
            Return CBool2(GetSystemDefault("EnforceZRead"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("EnforceZRead", If(value, 1, 0))
        End Set
    End Property

    Public Property Only1SalesRep As Boolean
        Get
            Return CBool2(GetSystemDefault("Only1SalesRep"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("Only1SalesRep", If(value, 1, 0))
        End Set
    End Property

    Public Property RequireSalesRep As Boolean
        Get
            Return CBool2(GetSystemDefault("RequireSalesRep"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("RequireSalesRep", If(value, 1, 0))
        End Set
    End Property

    Public Property DontAllowGreaterFreeTypePrices As Boolean
        Get
            Return CBool2(GetSystemDefault("DontAllowGreaterFreeTypePrices"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("DontAllowGreaterFreeTypePrices", If(value, 1, 0))
        End Set
    End Property

    Public Property AlwaysTestFreeTypeAPrice As Boolean
        Get
            Return CBool2(GetSystemDefault("AlwaysTestFreeTypeAPrice"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("AlwaysTestFreeTypeAPrice", If(value, 1, 0))
        End Set
    End Property

    Public Property SearchOnlyForBarcode As Boolean
        Get
            Return CBool2(GetSystemDefault("SearchOnlyForBarcode"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("SearchOnlyForBarcode", If(value, 1, 0))
        End Set
    End Property

    Public ReadOnly Property MParentProperty As Business
        Get
            Return mParent
        End Get
    End Property

    ''' <summary>
    ''' return LibraryCustomersBin.ID
    ''' </summary>
    ''' <param name="vCustomerAddressID">LibraryCustomersAddresses.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOrAddDefaultBin(ByVal vCustomerAddressID As String, ByVal vStockID As String) As String

        Dim pOut As String
        Dim pSql As String
        Dim pIU As InsertUpdate

        If String.IsNullOrWhiteSpace(vCustomerAddressID) Then
            Throw New ApplicationException("GetOrAddDefaultBin: FowstID must not be blank. ref: vbKECHHTNMFD!")
        End If

        If String.IsNullOrWhiteSpace(vStockID) Then
            Throw New ApplicationException("GetOrAddDefaultBin: ProductID must not be blank. ref: vb8CPAY4XRN2!")
        End If

        If IsGuid(vCustomerAddressID) = False Then
            Throw New ApplicationException("GetOrAddDefaultBin: FowstID must be a GUID. ref: vbKPN3X4YNVY!")
        End If

        If IsGuid(vStockID) = False Then
            Throw New ApplicationException("GetOrAddDefaultBin: ProductID must be a GUID. ref: vbNVBPWM43WS!")
        End If

        pSql = "SELECT BinID" &
            " FROM LibraryFowstBinProduct" &
            " WHERE FowstID = " & ReadyString(vCustomerAddressID) &
            " AND ProductID = " & ReadyString(vStockID) &
            " AND IsDefault = 1 "
        pOut = CStr2(mProData.ExecuteScalar(pSql))
        If CStr2(pOut) <> "" Then
            Return pOut
        End If

        pIU = New InsertUpdate("LibraryFowstBin")
        pOut = Guid.NewGuid.ToString
        pIU("ID") = ReadyString(pOut)
        pIU("FowstID") = ReadyString(vCustomerAddressID)
        pIU("Name") = "'Unnamed Bin'"
        pIU("Description") = "''"
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("LibraryFowstBinProduct")
        pIU("FowstID") = ReadyString(vCustomerAddressID)
        pIU("BinID") = ReadyString(pOut)
        pIU("ProductID") = ReadyString(vStockID)
        pIU("IsDefault") = 1
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

        Return pOut

    End Function

    Public Property CustomerOrganizationID() As String
        Get

            Dim pOut As String

            pOut = GetSystemDefault("Customer Organization ID")
            If IsGuid(pOut) Then
                Return ReadyGuid(pOut)
            End If

        End Get
        Set(value As String)

            SetSystemDefault("Customer Organization ID", value)

        End Set
    End Property

    Public Property AllowPriceAdjustment As Boolean
        Get

            Return CDec2(GetSystemDefault("AllowPriceAdjustment")) <> 0

        End Get
        Set(value As Boolean)

            SetSystemDefault("AllowPriceAdjustment", If(value, 1, 0))

        End Set
    End Property

    Public Property PurchaseOrderSupplierSetting As String
        Get

            Return CStr2(GetSystemDefault("PurchaseOrderSupplierSetting"))

        End Get
        Set(value As String)

            SetSystemDefault("PurchaseOrderSupplierSetting", value)

        End Set
    End Property

    Public Property PurchaseOrderSupplierID As String
        Get

            Return CStr2(GetSystemDefault("PurchaseOrderSupplierID"))

        End Get
        Set(value As String)

            SetSystemDefault("PurchaseOrderSupplierID", value)

        End Set
    End Property

    Public Property PurchaseOrderSupplierFowstID As String
        Get

            Return CStr2(GetSystemDefault("PurchaseOrderSupplierFowstID"))

        End Get
        Set(value As String)

            SetSystemDefault("PurchaseOrderSupplierFowstID", value)

        End Set
    End Property

    Public Property PurchaseOrderCustomerSetting As String
        Get

            Return CStr2(GetSystemDefault("PurchaseOrderCustomerSetting"))

        End Get
        Set(value As String)

            SetSystemDefault("PurchaseOrderCustomerSetting", value)

        End Set
    End Property

    Public Property PurchaseOrderCustomerID As String
        Get

            Return CStr2(GetSystemDefault("PurchaseOrderCustomerID"))

        End Get
        Set(value As String)

            SetSystemDefault("PurchaseOrderCustomerID", value)

        End Set
    End Property

    Public Property PurchaseOrderCustomerFowstID As String
        Get

            Return CStr2(GetSystemDefault("PurchaseOrderCustomerFowstID"))

        End Get
        Set(value As String)

            SetSystemDefault("PurchaseOrderCustomerFowstID", value)

        End Set
    End Property

    Public Property ShowCondensedProductInformationInSearchResult As Boolean
        Get

            Return CBool2(GetSystemDefault("ShowCondensedProductInformationInSearchResult"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("ShowCondensedProductInformationInSearchResult", value)

        End Set
    End Property

    Public Property ShowCondensedProductInformationInCart As Boolean
        Get

            Return CBool2(GetSystemDefault("ShowCondensedProductInformationInCart"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("ShowCondensedProductInformationInCart", value)

        End Set
    End Property

    Public Property ShowCondensedProductInformationInItemDetailsPage As Boolean
        Get

            Return CBool2(GetSystemDefault("ShowCondensedProductInformationInItemDetailsPage"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("ShowCondensedProductInformationInItemDetailsPage", value)

        End Set
    End Property

    Public Property VatPercent As Decimal
        Get
            Dim p As String

            p = GetSystemDefault("VatPercent")
            If IsNumeric(p) Then
                Return p
            Else
                Return 12
            End If

        End Get
        Set(value As Decimal)

            SetSystemDefault("VatPercent", value)

        End Set
    End Property

    Public Property Vatable As Boolean
        Get

            Dim pData = GetSystemDefault("Vatable")
            If String.IsNullOrWhiteSpace(pData) Then
                Return True
            End If
            Return CBool2(pData)

        End Get
        Set(value As Boolean)

            SetSystemDefault("Vatable", If(value, 1, 0))

        End Set
    End Property


    Public Property PictureOfIDDurationSCPWDDiscount As Boolean
        Get

            Dim pData = GetSystemDefault("PictureOfIDDurationSCPWDDiscount")
            If String.IsNullOrWhiteSpace(pData) Then
                Return True
            End If
            Return CBool2(pData)

        End Get
        Set(value As Boolean)

            SetSystemDefault("PictureOfIDDurationSCPWDDiscount", If(value, 1, 0))

        End Set
    End Property

    Public Property HideBarcodeScan As Boolean
        Get

            Return CBool2(GetSystemDefault("HideBarcodeScan"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("HideBarcodeScan", value)

        End Set
    End Property

    Public Property AllowOfflineSrvPayments As Boolean
        Get

            Return CBool2(GetSystemDefault("AllowOfflineSrvPayments"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("AllowOfflineSrvPayments", value)

        End Set
    End Property

    Public Property HideHotButtonsButton As Boolean
        Get

            Return CBool2(GetSystemDefault("HideHotButtonsButton"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("HideHotButtonsButton", value)

        End Set
    End Property

    Public Property HideSearchWithinResults As Boolean
        Get

            Return CBool2(GetSystemDefault("HideSearchWithinResults"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("HideSearchWithinResults", value)

        End Set
    End Property

    Public Property PrintDispatchSlip As Boolean
        Get

            Return CBool2(GetSystemDefault("PrintDispatchSlip"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("PrintDispatchSlip", value)

        End Set
    End Property

    Public Property FMCGDisplayCart As Boolean
        Get
            Dim val As String = CStr(GetSystemDefault("IsLineItemDisplay"))
            Return If(val = "", 1, CBool2(val))

        End Get
        Set(value As Boolean)

            SetSystemDefault("IsLineItemDisplay", value)

        End Set
    End Property

    Public Property FMCGPrintReceipt As Boolean
        Get
            Dim val As String = CStr(GetSystemDefault("RealTimePrintReceipt"))
            Return If(val = "", 1, CBool2(val))

        End Get
        Set(value As Boolean)

            SetSystemDefault("RealTimePrintReceipt", value)

        End Set
    End Property

    Public Property IsAutoServiceCharge As Boolean
        Get
            Dim val As String = CStr(GetSystemDefault("IsAutoServiceCharge"))
            Return If(val = "", 0, CBool2(val))

        End Get
        Set(value As Boolean)

            SetSystemDefault("IsAutoServiceCharge", value)

        End Set
    End Property

    Public Property CanTransactWithUnflushedData As Boolean
        Get
            Dim val As String = CStr(GetSystemDefault("CanTransactWithUnflushedData"))
            Return If(val = "", True, CBool2(val))

        End Get
        Set(value As Boolean)

            SetSystemDefault("CanTransactWithUnflushedData", value)

        End Set
    End Property

    Public Property ServiceCharge As Decimal
        Get
            Dim val As String = CStr(GetSystemDefault("ServiceCharge"))
            Return If(val = "", 0, CDec2(val))

        End Get
        Set(value As Decimal)

            SetSystemDefault("ServiceCharge", value)

        End Set
    End Property

    Public Property ServiceChargeBasis As String
        Get
            Dim pOut = GetSystemDefault("ServiceChargeBasis")
            pOut = CStr2(pOut).ToLower()
            If pOut <> "vatin" Then
                pOut = "vatex"
            End If
            Return pOut
        End Get
        Set(value As String)
            value = CStr2(value).ToLower()
            If value <> "vatin" Then
                value = "vatex"
            End If
            SetSystemDefault("ServiceChargeBasis", value)
        End Set
    End Property

    Public Property AutoImportCustomerRecordsToCRM As Decimal
        Get
            Dim val As String = CStr(GetSystemDefault("AutoImportCustomerRecordsToCRM"))
            Return If(val = "", 1, CDec2(val))

        End Get
        Set(value As Decimal)

            SetSystemDefault("AutoImportCustomerRecordsToCRM", value)

        End Set
    End Property

    Public Property AutoImportCRMRecordsToCustomer As Decimal
        Get
            Dim val As String = CStr(GetSystemDefault("AutoImportCRMRecordsToCustomer"))
            Return If(val = "", 1, CDec2(val))

        End Get
        Set(value As Decimal)

            SetSystemDefault("AutoImportCRMRecordsToCustomer", value)

        End Set
    End Property

    Public Property AutoImportCRMRecordsToSupplier As Decimal
        Get
            Dim val As String = CStr(GetSystemDefault("AutoImportCRMRecordsToSupplier"))
            Return If(val = "", 1, CDec2(val))

        End Get
        Set(value As Decimal)

            SetSystemDefault("AutoImportCRMRecordsToSupplier", value)

        End Set
    End Property

    Public Property AllItemsEligibleForSCPWDDiscount As Boolean
        Get
            Dim val As String = CStr(GetSystemDefault("AllItemsEligibleForSCPWDDiscount"))
            Return If(val = "", 1, CBool2(val))

        End Get
        Set(value As Boolean)

            If value Then
                Dim pSql As String

                pSql = "Update LibraryOrganizationPIM SET IsVatExempt = 1"
                mProData.ExecuteSQL(pSql)

            End If

            SetSystemDefault("AllItemsEligibleForSCPWDDiscount", value)

        End Set
    End Property

    ''' <summary>
    ''' Updates private.TablesLastUpdated with specified vTableName. 
    ''' If the specified vTableName does not exist, it will be created
    ''' </summary>
    ''' <param name="vTableName"></param>
    ''' <remarks></remarks>
    Public Sub UpdateTableLastUpdated(vTableName As String)
        ' ref: 2015_08_29__11_14_10_8437

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TablesLastUpdated")
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetUpdate("TableName = " & ReadyString(vTableName))
        If mProData.ExecuteSQL(pSql) = 0 Then
            pIU("TableName") = ReadyString(vTableName)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Public Property DefaultPriceLevelID As String
        Get
            Dim pOut = GetSystemDefault("DefaultPriceLevelID")
            If IsGuid(pOut) = False Then
                pOut = Me.GetRetailPriceID
                SetSystemDefault("DefaultPriceLevelID", pOut)
            End If

            Return pOut

        End Get
        Set(value As String)
            SetSystemDefault("DefaultPriceLevelID", value)
        End Set
    End Property

    Public Property DefaultPriceLevelIDs As String
        Get
            Return GetSystemDefault("DefaultPriceLevelIDs")
        End Get
        Set(value As String)
            SetSystemDefault("DefaultPriceLevelIDs", value)
        End Set
    End Property

    ''' <summary>
    ''' this function must be moved to another class
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPriceLevelName(vID As String) As String

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT Pricelevel" & vbCrLf &
            " FROM MobizLibraryPIMPriceLevel" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return pDR("PriceLevel")
        End If

    End Function

    Public Property AllowEncodingOfOrdersInInvoice As Boolean
        Get
            Return CBool2(GetSystemDefault("AllowEncodingOfOrdersInInvoice"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("AllowEncodingOfOrdersInInvoice", If(value, 1, 0))
        End Set
    End Property

    Public Property AllowEncodingOfQuantitiesInInvoice As Boolean
        Get
            Return CBool2(GetSystemDefault("AllowEncodingOfQuantitiesInInvoice"))
        End Get
        Set(value As Boolean)
            SetSystemDefault("AllowEncodingOfQuantitiesInInvoice", If(value, 1, 0))
        End Set
    End Property

    Public Property FreeTypeAPriceMinimumGpm As Decimal
        Get
            Return CDec2(GetSystemDefault("FreeTypeAPriceMinimumGpm"))
        End Get
        Set(value As Decimal)
            SetSystemDefault("FreeTypeAPriceMinimumGpm", value)
        End Set
    End Property

    Public Property ProductMakerDefaultHotButtonTemplateID As String
        Get
            Return CStr2(GetSystemDefault("ProductMakerDefaultHotButtonTemplateID"))
        End Get
        Set(value As String)
            SetSystemDefault("ProductMakerDefaultHotButtonTemplateID", value)
        End Set
    End Property

    ''' <summary>
    ''' LibraryDocumentControl.ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AutoCreateInvoiceTypeID As String
        Get
            Return CStr2(GetSystemDefault("AutoCreateInvoiceTypeID"))
        End Get
        Set(value As String)
            SetSystemDefault("AutoCreateInvoiceTypeID", value)
        End Set
    End Property

    Public Property AutoCreateInvoiceFowstID As String
        Get
            Return CStr2(GetSystemDefault("AutoCreateInvoiceFowstID"))
        End Get
        Set(value As String)
            SetSystemDefault("AutoCreateInvoiceFowstID", value)
        End Set
    End Property

    Public Property AutoCreateInvoiceDeviceID As String
        Get
            Return CStr2(GetSystemDefault("AutoCreateInvoiceDeviceID"))
        End Get
        Set(value As String)
            SetSystemDefault("AutoCreateInvoiceDeviceID", value)
        End Set
    End Property

    Public Enum eAutoCreateInvoiceCondition
        StockReceiving
        VendorPortal
        StockReceivingSelected
        VendorPortalSelected
    End Enum

    Public Function AutoCreateInvoiceConditionProper() As eAutoCreateInvoiceCondition
        Dim pValue = AutoCreateInvoiceCondition
        Dim pOut As eAutoCreateInvoiceCondition

        If [Enum].TryParse(Of eAutoCreateInvoiceCondition)(pValue, True, pOut) Then
            Return pOut
        Else
            Return eAutoCreateInvoiceCondition.StockReceiving
        End If
    End Function

    Public Property AutoCreateInvoiceCondition As String
        Get
            Return CStr2(GetSystemDefault("AutoCreateInvoiceCondition"))
        End Get
        Set(value As String)
            SetSystemDefault("AutoCreateInvoiceCondition", value)
        End Set
    End Property

    Public Property AvailableCreditLimitComputation As String
        Get
            Return CStr2(GetSystemDefault("AvailableCreditLimitComputation"))
        End Get
        Set(value As String)
            SetSystemDefault("AvailableCreditLimitComputation", value)
        End Set
    End Property

    Public Property CheckOutTimeDefault As Date
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("CheckOutTimeDefault")))
            If pOut Is Nothing Then
                Return New Date()
            Else
                Return pOut.Value
            End If
        End Get
        Set(value As Date)
            SetSystemDefault("CheckOutTimeDefault", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property CheckInTimeDefault As Date
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("CheckInTimeDefault")))
            If pOut Is Nothing Then
                Return New Date()
            Else
                Return pOut.Value
            End If
        End Get
        Set(value As Date)
            SetSystemDefault("CheckInTimeDefault", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property EarlyCheckInTime As Date
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("EarlyCheckInTime")))
            If pOut Is Nothing Then
                Return New Date()
            Else
                Return pOut.Value
            End If
        End Get
        Set(value As Date)
            SetSystemDefault("EarlyCheckInTime", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property EarlyCheckInProductID As String
        Get
            Return GetSystemDefault("EarlyCheckInProductID")
        End Get
        Set(value As String)
            SetSystemDefault("EarlyCheckInProductID", value)
        End Set
    End Property

    Public Property LateCheckOutTime As Date
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("LateCheckOutTime")))
            If pOut Is Nothing Then
                Return New Date()
            Else
                Return pOut.Value
            End If
        End Get
        Set(value As Date)
            SetSystemDefault("LateCheckOutTime", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property LateCheckOutProductID As String
        Get
            Return GetSystemDefault("LateCheckOutProductID")
        End Get
        Set(value As String)
            SetSystemDefault("LateCheckOutProductID", value)
        End Set
    End Property

    Public Property DisplayAssetPrice As String
        Get
            Dim pValue = GetSystemDefault("DisplayAssetPrice")
            If String.IsNullOrWhiteSpace(pValue) Then
                Return "exactprice"
            Else
                Return pValue
            End If
        End Get
        Set(value As String)
            SetSystemDefault("DisplayAssetPrice", value)
        End Set
    End Property

    Public Property AssetMgmtDownpaymentRequired As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("AssetMgmtDownpaymentRequired"))
            Return IIf(val = "", 0, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("AssetMgmtDownpaymentRequired", value)
        End Set
    End Property

    Public Property LastUpdateForGetBalanceFromDuc As Date?
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("LastUpdateForGetBalanceFromDuc")))
            Return pOut
        End Get
        Set(value As Date?)
            SetSystemDefault("LastUpdateForGetBalanceFromDuc", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    ' default is true
    Public Property FcSettingExcludeVendorPoInFc As Boolean
        Get
            Dim pValue = GetSystemDefault("FcSettingExcludeVendorPoInFc")
            If String.IsNullOrWhiteSpace(pValue) Then
                Return True
            End If
            If IsNumeric(pValue) = False Then
                Return True
            End If
            Return CBool2(pValue)
        End Get
        Set(value As Boolean)
            SetSystemDefault("FcSettingExcludeVendorPoInFc", If(value, 1, 0))
        End Set
    End Property

    Public Property PickListPrintCount As Integer
        Get
            Return Math.Max(CInt2(GetSystemDefault("PickListPrintCount")), 1)
        End Get
        Set(value As Integer)
            SetSystemDefault("PickListPrintCount", value)
        End Set
    End Property

    Public Property MaxMobizAppDatabaseAge As Integer
        Get
            Dim pValue = GetSystemDefault("MaxMobizAppDatabaseAge")
            If String.IsNullOrWhiteSpace(pValue) Then
                Return 30 ' the default value
            Else
                Return CInt2(pValue)
            End If
        End Get
        Set(value As Integer)
            SetSystemDefault("MaxMobizAppDatabaseAge", value)
        End Set
    End Property

    Public Property MobizDTRPort As String
        Get

            Return CDec2(GetSystemDefault("Mobiz DTR Port"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("Mobiz DTR Port", value)

        End Set
    End Property

    Public Property AutoPrintUuid As String
        Get

            Return CStr2(GetSystemDefault("AutoPrintUuid"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("AutoPrintUuid", value)

        End Set
    End Property

    Public Property RoundUpSellingPrices As String
        Get


            Return CStr2(GetSystemDefault("RoundUpSellingPrices"))

        End Get
        Set(ByVal value As String)

            SetSystemDefault("RoundUpSellingPrices", value)

        End Set
    End Property

    Public Property RoundUpSellingPricesCentavo As Integer
        Get


            Return CInt2(GetSystemDefault("RoundUpSellingPricesCentavo"))

        End Get
        Set(ByVal value As Integer)

            SetSystemDefault("RoundUpSellingPricesCentavo", value)

        End Set
    End Property

    Public Property ShowPaymentCheck As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentCheck"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentCheck", value)
        End Set
    End Property

    Public Property ShowPaymentDebit As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentDebit"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentDebit", value)
        End Set
    End Property

    Public Property ShowPaymentCredit As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentCredit"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentCredit", value)
        End Set
    End Property

    Public Property ShowPaymentSRV As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentSRV"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentSRV", value)
        End Set
    End Property

    Public Property ShowOtherPayment As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowOtherPayment"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowOtherPayment", value)
        End Set
    End Property

    Public Property ShowPaymentOnCredit As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentOnCredit"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentOnCredit", value)
        End Set
    End Property

    Public Property ShowPaymentBillingInfo As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentBillingInfo"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentBillingInfo", value)
        End Set
    End Property

    Public Property ShowPaymentDeliveryInfo As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentDeliveryInfo"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentDeliveryInfo", value)
        End Set
    End Property

    Public Property ShowPaymentDiscounts As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentDiscounts"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentDiscounts", value)
        End Set
    End Property

    Public Property ShowPaymentCharges As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentCharges"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentCharges", value)
        End Set
    End Property

    Public Property ShowPaymentRewards As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentRewards"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentRewards", value)
        End Set
    End Property

    Public Property ShowPaymentSalesReprensentative As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowPaymentSalesReprensentative"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowPaymentSalesReprensentative", value)
        End Set
    End Property

    Public Property ShowDeliveryPayment As Boolean
        Get
            Dim val = CStr2(GetSystemDefault("ShowDeliveryPayment"))
            Return IIf(val = "", 1, val)
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("ShowDeliveryPayment", value)
        End Set
    End Property

    Public Property AccountsPayableFollowVendorCost As Boolean
        Get

            Return CBool2(GetSystemDefault("AccountsPayableFollowVendorCost"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("AccountsPayableFollowVendorCost", If(value, 1, 0))

        End Set
    End Property

    Public Property GrantCreditOffline As Boolean
        Get

            Return CBool2(GetSystemDefault("GrantCreditOffline"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("GrantCreditOffline", If(value, 1, 0))

        End Set
    End Property

    Public Property GrantCreditAllCustomers As Boolean
        Get

            Return CBool2(GetSystemDefault("GrantCreditAllCustomers"))

        End Get
        Set(value As Boolean)

            SetSystemDefault("GrantCreditAllCustomers", If(value, 1, 0))

        End Set
    End Property

    Public Property ExpirationDaysForSOALink As String
        Get
            Return IIf(CStr2(GetSystemDefault("ExpirationDaysForSOALink")) = "", 7, CStr2(GetSystemDefault("ExpirationDaysForSOALink")))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ExpirationDaysForSOALink", value)
        End Set
    End Property

    Public Property SOALinkDomainName As String
        Get
            Return IIf(CStr2(GetSystemDefault("SOALinkDomainName")) = "", "konstruksyon.com", CStr2(GetSystemDefault("SOALinkDomainName")))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SOALinkDomainName", value)
        End Set
    End Property

    Public Property SOALinkDomainInternalIPAddress As String
        Get
            Return CStr2(GetSystemDefault("SOALinkDomainInternalIPAddress"))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SOALinkDomainInternalIPAddress", value)
        End Set
    End Property
    Public Property SOALinkDomainExternalIPAddress As String
        Get
            Return CStr2(GetSystemDefault("SOALinkDomainExternalIPAddress"))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SOALinkDomainExternalIPAddress", value)
        End Set
    End Property
    Public Property SOAFootnote As String
        Get
            Return CStr2(GetSystemDefault("SOAFootnote"))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SOAFootnote", value)
        End Set
    End Property
    Public Property ExpirationDaysForPOLink As String
        Get
            Return IIf(CStr2(GetSystemDefault("ExpirationDaysForPOLink")) = "", 7, CStr2(GetSystemDefault("ExpirationDaysForPOLink")))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ExpirationDaysForPOLink", value)
        End Set
    End Property
    Public Property POLinkDomainName As String
        Get
            Return IIf(CStr2(GetSystemDefault("POLinkDomainName")) = "", "konstruksyon.com", CStr2(GetSystemDefault("POLinkDomainName")))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("POLinkDomainName", value)
        End Set
    End Property

    Public Property POLinkDomainInternalIPAddress As String
        Get
            Return CStr2(GetSystemDefault("POLinkDomainInternalIPAddress"))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("POLinkDomainInternalIPAddress", value)
        End Set
    End Property
    Public Property POLinkDomainExternalIPAddress As String
        Get
            Return CStr2(GetSystemDefault("POLinkDomainExternalIPAddress"))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("POLinkDomainExternalIPAddress", value)
        End Set
    End Property
    Public Property EcommerceDataLastRequest As Date?
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("EcommerceDataLastRequest")))
            Return pOut
        End Get
        Set(value As Date?)
            SetSystemDefault("EcommerceDataLastRequest", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property EsoaEarliestDate As Date?
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("EsoaEarliestDate")))
            Return pOut
        End Get
        Set(value As Date?)
            SetSystemDefault("EsoaEarliestDate", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property SoaEarliestDate As Date?
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("SoaEarliestDate")))
            Return pOut
        End Get
        Set(value As Date?)
            SetSystemDefault("SoaEarliestDate", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property EcommerceDataLastImport As Date?
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("EcommerceDataLastImport")))
            Return pOut
        End Get
        Set(value As Date?)
            SetSystemDefault("EcommerceDataLastImport", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property WeightedAverageDaysOverdueLastUpdate As Date?
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("WeightedAverageDaysOverdueLastUpdate")))
            Return pOut
        End Get
        Set(value As Date?)
            SetSystemDefault("WeightedAverageDaysOverdueLastUpdate", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Property ItemRequisitionDateCoverage As String
        Get

            Dim value = CStr2(GetSystemDefault("ItemRequisitionDateCoverage"))
            If value <> "week_to_date" _
                AndAlso value <> "1_week_ago" _
                AndAlso value <> "2_weeks_ago" _
                AndAlso value <> "4_weeks_ago" Then

                Return "week_to_date"
            End If

            Return value

        End Get
        Set(ByVal value As String)

            If value <> "week_to_date" _
                AndAlso value <> "1_week_ago" _
                AndAlso value <> "2_weeks_ago" _
                AndAlso value <> "4_weeks_ago" Then

                Throw New ArgumentException("invalid value for ItemRequisitionDateCoverage. ref: vbWHQXC3XCBG!")
            End If

            SetSystemDefault("ItemRequisitionDateCoverage", value)

        End Set
    End Property

    Public Property FOWSTAddRecord As String
        Get
            Dim val = GetSystemDefault("FOWSTAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("FOWSTAddRecord", value)
        End Set
    End Property

    Public Property AuthorizedDevicesAddRecord As String
        Get
            Dim val = GetSystemDefault("AuthorizedDevicesAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("AuthorizedDevicesAddRecord", value)
        End Set
    End Property

    Public Property ReceiptSettingsAddRecord As String
        Get
            Dim val = GetSystemDefault("ReceiptSettingsAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ReceiptSettingsAddRecord", value)
        End Set
    End Property

    Public Property SecurityRolesAddRecord As String
        Get
            Dim val = GetSystemDefault("SecurityRolesAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SecurityRolesAddRecord", value)
        End Set
    End Property

    Public Property EmployeesAddRecord As String
        Get
            Dim val = GetSystemDefault("EmployeesAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("EmployeesAddRecord", value)
        End Set
    End Property

    Public Property PrinterRolesAddRecord As String
        Get
            Dim val = GetSystemDefault("PrinterRolesAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("PrinterRolesAddRecord", value)
        End Set
    End Property

    Public Property HotButtonSettingsAddRecord As String
        Get
            Dim val = GetSystemDefault("HotButtonSettingsAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("HotButtonSettingsAddRecord", value)
        End Set
    End Property

    Public Property GroupSettingsAddRecord As String
        Get
            Dim val = GetSystemDefault("GroupSettingsAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("GroupSettingsAddRecord", value)
        End Set
    End Property

    Public Property CustomerListAddRecord As String
        Get
            Dim val = GetSystemDefault("CustomerListAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("CustomerListAddRecord", value)
        End Set
    End Property

    Public Property SupplierListAddRecord As String
        Get
            Dim val = GetSystemDefault("SupplierListAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SupplierListAddRecord", value)
        End Set
    End Property

    Public Property SalesTeamAddRecord As String
        Get
            Dim val = GetSystemDefault("SalesTeamAddRecord")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SalesTeamAddRecord", value)
        End Set
    End Property

    Public Property Enable2FA As String
        Get
            Dim val = GetSystemDefault("Enable2FA")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("Enable2FA", value)
        End Set
    End Property

    Public Property ShowMCRModulePOS As String
        Get
            Dim val = GetSystemDefault("ShowMCRModulePOS")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModulePOS", value)
        End Set
    End Property

    Public Property ShowMCRModulePriceChecker As String
        Get
            Dim val = GetSystemDefault("ShowMCRModulePriceChecker")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModulePriceChecker", value)
        End Set
    End Property

    Public Property ShowMCRModulePO As String
        Get
            Dim val = GetSystemDefault("ShowMCRModulePO")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModulePO", value)
        End Set
    End Property

    Public Property ShowMCRModuleSalesReturn As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleSalesReturn")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleSalesReturn", value)
        End Set
    End Property

    Public Property ShowMCRModuleStockReceiving As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleStockReceiving")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleStockReceiving", value)
        End Set
    End Property

    Public Property ShowMCRModuleStockTransfer As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleStockTransfer")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleStockTransfer", value)
        End Set
    End Property

    Public Property ShowMCRModuleInventoryCount As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleInventoryCount")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleInventoryCount", value)
        End Set
    End Property

    Public Property ShowMCRModuleRouteExpenseTracking As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleRouteExpenseTracking")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleRouteExpenseTracking", value)
        End Set
    End Property

    Public Property ShowMCRModuleProductMaker As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleProductMaker")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleProductMaker", value)
        End Set
    End Property

    Public Property ShowMCRModuleShelfBinLabelling As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleShelfBinLabelling")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleShelfBinLabelling", value)
        End Set
    End Property

    Public Property ShowMCRModuleRequestApprovals As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleRequestApprovals")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleRequestApprovals", value)
        End Set
    End Property

    Public Property ShowMCRModuleCashHandling As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleCashHandling")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleCashHandling", value)
        End Set
    End Property

    Public Property ShowMCRModuleDashboard As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleDashboard")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleDashboard", value)
        End Set
    End Property

    Public Property ShowMCRModuleReportZRead As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleReportZRead")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleReportZRead", value)
        End Set
    End Property

    Public Property ShowMCRModuleReconstructSalesData As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleReconstructSalesData")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleReconstructSalesData", value)
        End Set
    End Property

    Public Property ShowMCRModuleLazyUpdate As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleLazyUpdate")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleLazyUpdate", value)
        End Set
    End Property

    Public Property ShowMCRModuleRestoreSystem As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleRestoreSystem")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleRestoreSystem", value)
        End Set
    End Property

    Public Property ShowMCRModuleCustomerDisplay As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleCustomerDisplay")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleCustomerDisplay", value)
        End Set
    End Property

    Public Property ShowMCRModuleOrderTakerSetup As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleOrderTakerSetup")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleOrderTakerSetup", value)
        End Set
    End Property

    Public Property ShowMCRModuleSMSSetup As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleSMSSetup")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleSMSSetup", value)
        End Set
    End Property

    Public Property ShowMCRModuleAdjustZReadDate As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleAdjustZReadDate")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleAdjustZReadDate", value)
        End Set
    End Property

    Public Property ShowMCRModuleUpdateCacheDB As String
        Get
            Dim val = GetSystemDefault("ShowMCRModuleUpdateCacheDB")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowMCRModuleUpdateCacheDB", value)
        End Set
    End Property

    Public Property ShowHeadCount As String 'For SC/PWD in POS during Payment
        Get
            Dim val = GetSystemDefault("ShowHeadCount")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowHeadCount", value)
        End Set
    End Property

    Public Property AutoApproveFranchiseeRequests As String
        Get
            Dim val = GetSystemDefault("AutoApproveFranchiseeRequests")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("AutoApproveFranchiseeRequests", value)
        End Set
    End Property

    Public Property ShowAnnouncements As String
        Get
            Dim val = GetSystemDefault("ShowAnnouncements")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowAnnouncements", value)
        End Set
    End Property

    Public Property RequiredCredentialsTurnoverMonies As String
        Get
            Dim val = GetSystemDefault("RequiredCredentialsTurnoverMonies")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("RequiredCredentialsTurnoverMonies", value)
        End Set
    End Property

    Public Property EnableVDMFeatures As String
        Get
            Dim val = GetSystemDefault("EnableVDMFeatures")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("EnableVDMFeatures", value)
        End Set
    End Property

    Public Property DownloadSKUPics As String
        Get
            Dim val = GetSystemDefault("DownloadSKUPicsAfterSyncingDB")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("DownloadSKUPicsAfterSyncingDB", value)
        End Set
    End Property

    Public Property ShowManagementReport As String
        Get
            Dim val = GetSystemDefault("ShowManagementReport")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("ShowManagementReport", value)
        End Set
    End Property

    Public Property SRUseThirdPartyAsSource As String
        Get
            Dim val = GetSystemDefault("SRUseThirdPartyAsSource")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SRUseThirdPartyAsSource", value)
        End Set
    End Property

    Public Property SRThirdPartySupplier As String
        Get
            Dim val = GetSystemDefault("SRThirdPartySupplier")
            Return CStr2(val)
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SRThirdPartySupplier", value)
        End Set
    End Property

    Public Property SRThirdPartySupplierOutlet As String
        Get
            Dim val = GetSystemDefault("SRThirdPartySupplierOutlet")
            Return CStr2(val)
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SRThirdPartySupplierOutlet", value)
        End Set
    End Property

    Public Property AllowSRBadCount As String
        Get
            Dim val = GetSystemDefault("AllowSRBadCount")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("AllowSRBadCount", value)
        End Set
    End Property

    Public Property RemindUserAboutTheDraftTransactionsStockRecTrans As String
        Get
            Dim val = GetSystemDefault("RemindUserAboutTheDraftTransactionsStockRecTrans")
            Return IIf(CStr2(val) = "", 0, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("RemindUserAboutTheDraftTransactionsStockRecTrans", value)
        End Set
    End Property

    Public Property RecTransPackSizeDiscprepancy As String
        Get
            Dim val = GetSystemDefault("RecTransPackSizeDiscprepancy")
            Return IIf(CStr2(val) = "", 1, CInt2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("RecTransPackSizeDiscprepancy", value)
        End Set
    End Property

    Public Property IsReceiveInInternalSetToDefault As Boolean
        Get
            Dim val = GetSystemDefault("IsReceiveInInternalSetToDefault")
            Return IIf(CStr2(val) = "", False, CBool2(val))
        End Get
        Set(ByVal value As Boolean)
            SetSystemDefault("IsReceiveInInternalSetToDefault", value)
        End Set
    End Property

    Public Property SRDefaultSourceDocType As String
        Get
            Dim val = GetSystemDefault("SRDefaultSourceDocType")
            Return CStr2(val)
        End Get
        Set(ByVal value As String)
            SetSystemDefault("SRDefaultSourceDocType", value)
        End Set
    End Property

    Public Property KBServerUrl As String
        Get
            Dim val = GetSystemDefault("KBServerUrl")
            Return IIf(CStr2(val) = "", "", CStr2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("KBServerUrl", value)
        End Set
    End Property

    Public Property PriceBulletinItemsPerPage As String
        Get
            Dim val = GetSystemDefault("PriceBulletinItemsPerPage")
            Return IIf(CStr2(val) = "", "5", CStr2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("PriceBulletinItemsPerPage", value)
        End Set
    End Property

    Public Property PriceBulletinPageChangeDelay As String
        Get
            Dim val = GetSystemDefault("PriceBulletinPageChangeDelay")
            Return IIf(CStr2(val) = "", "5", CStr2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("PriceBulletinPageChangeDelay", value)
        End Set
    End Property

    Public Property AllowCustomLocationName As String
        'for Route and Expense Tracking
        Get
            Dim val = GetSystemDefault("AllowCustomLocationName")
            Return IIf(CStr2(val) = "", 1, CStr2(val))
        End Get
        Set(ByVal value As String)
            SetSystemDefault("AllowCustomLocationName", value)
        End Set
    End Property

    Public Property LastSendOfDeviceMonitoringInfo As Date?
        Get
            Dim pOut = ParseDate(CStr2(GetSystemDefault("LastSendOfDeviceMonitoringInfo")))
            Return pOut
        End Get
        Set(value As Date?)
            SetSystemDefault("LastSendOfDeviceMonitoringInfo", Format(value, cGlobal.FormatDateTimeSql2))
        End Set
    End Property

    Public Function SaveCompanyOwnedOrFranchisee(vOrgID As String) As String

        Dim pOut As String
        'Dim pDT = mParent.LibraryOrganization.GetFranchisors(vOrgID)
        'pDT.DefaultView.RowFilter = "IsValid = 1"
        'If pDT.DefaultView.Count > 0 Then
        '    pOut = "franchisee"
        'Else
        '    pOut = "company-owned"
        'End If

        If ReadyGuid(vOrgID) = "892F84DA-9CA1-42BF-AF3D-A7DDCB9BC4BA" Then
            pOut = "franchisee"
        Else
            pOut = "company-owned"
        End If
        SetSystemDefault("CompanyOwnedOrFranchisee", pOut)

        Return pOut

    End Function

    Public Property WebPosAfterSaving As Integer?
        Get

            Dim pStr As String

            pStr = GetSystemDefault("WebPosAfterSaving")
            If IsNumeric(pStr) Then
                Return pStr
            End If

        End Get
        Set(value As Integer?)

            If value Is Nothing Then
                SetSystemDefault("WebPosAfterSaving", "")
            Else
                SetSystemDefault("WebPosAfterSaving", value)
            End If

        End Set
    End Property

    Public Property UnflushedReminderMinutes As Integer
        'for Route and Expense Tracking
        Get
            Dim val = GetSystemDefault("UnflushedReminderMinutes")
            Return CInt2(val)
        End Get
        Set(ByVal value As Integer)
            SetSystemDefault("UnflushedReminderMinutes", value)
        End Set
    End Property

    Public Property UnflushedReminderMessage As String
        'for Route and Expense Tracking
        Get
            Dim val = GetSystemDefault("UnflushedReminderMessage")
            Return val
        End Get
        Set(ByVal value As String)
            SetSystemDefault("UnflushedReminderMessage", value)
        End Set
    End Property

    Public Property UnflushedCountSendingInterval As Integer
        'for Route and Expense Tracking
        Get
            Dim val = GetSystemDefault("UnflushedCountSendingInterval")
            Return CInt2(val)
        End Get
        Set(ByVal value As Integer)
            SetSystemDefault("UnflushedCountSendingInterval", value)
        End Set
    End Property
End Class