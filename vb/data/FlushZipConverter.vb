Imports System.Collections.Specialized
Imports System.IO
Imports Newtonsoft.Json

Public Class FlushZipConverter
    Implements IDisposable
    Dim mZipFilename As String

    Dim mOrgID As String
    Const mUserID As String = modClient.EmployeeWebGuidID
    Dim mFowstID As String
    Dim mDeviceID As String
    Dim mBillingAddressID As String
    Dim mDeliveryAddressID As String
    Dim mCustomerID As String
    Public Const DeviceUuid As String = "Third Party Importation"
    Public BaseUrl As String
    Dim mPriceLevelID As String

    ' need for summary ony
    Dim mCompanyName As String
    Dim mFowstName As String
    Dim mTotalAmount As Decimal

    Dim mBusiness As Business
    Dim mDtInvoices As DataTable
    Dim mDtInvoiceItems As DataTable
    Dim mDtPayments As DataTable
    Dim mDtPaymentInvoices As DataTable
    Dim mDtPaymentsOther As DataTable
    Dim mDtItemsModifier As DataTable
    Dim mJsonInfo As JsonInfo
    Private disposedValue As Boolean
    Const mReplacementProductNo As String = "3rdParty"
    Const mReplacementModifierProductNo As String = "test_flavor"

    Public Function Process(vZipFilename As String, ByRef oError As String) As Boolean

        oError = Nothing
        mZipFilename = vZipFilename

        If String.IsNullOrWhiteSpace(BaseUrl) Then
            Throw New ApplicationException("BaseUrl is blank. vbTXK6ZLRJCP!")
        End If
        ProcessFolderInfo()
        ProcessJsonInfo(oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return False
        End If

        mBillingAddressID = mBusiness.SystemDefaults.GetSystemDefault("POS Billing Address ID")
        mDeliveryAddressID = mBusiness.SystemDefaults.GetSystemDefault("POS Delivery Address ID")
        mPriceLevelID = mBusiness.SystemDefaults.GetRetailPriceID()
        mCustomerID = mBusiness.SystemDefaults.CustomerOrganizationID

        mDtInvoices = ProcessTransInvoice(oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return False
        End If

        mDtInvoiceItems = ProcessTransInvoiceItems(oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return False
        End If

        mDtItemsModifier = ProcessTransInvoiceItemsModifier(oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return False
        End If

        mDtPayments = ProcessTransCustomerPayment(oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return False
        End If

        mDtPaymentsOther = ProcessTransCustomerPaymentOther(oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            Return False
        End If

        MakeFlushableZip()

    End Function

    Public Class FolderClass
        Public OrgID As String
    End Class

    Sub ProcessFolderInfo()

        Dim pFilename = IO.Path.Combine(Common.ServerPath, "settings\ThirdParty.txt")
        If IO.File.Exists(pFilename) = False Then
            Throw New ApplicationException("vb92DFTL694C! file does not exist. " & pFilename)
        End If

        Dim pFileText = FileText(pFilename)
        Dim pFolderJson = JsonConvert.DeserializeObject(Of FolderClass)(pFileText)
        mOrgID = pFolderJson.OrgID
        If IsGuid(mOrgID) = False Then
            Throw New ApplicationException("vbDBFMJF7N4Y! OrgID from ""ThirdParty.txt"" must be a GUID. Actual value = " & mOrgID)
        End If
        mOrgID = ReadyGuid(mOrgID)

        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
        Dim pSql = " SELECT MobizLibrarySubscription.OrganizationID" & vbCrLf &
            " ,MobizLibrarySubscription.[Database]" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " FROM MobizLibrarySubscription" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = MobizLibrarySubscription.OrganizationID" & vbCrLf &
            " WHERE MobizLibrarySubscription.OrganizationID = " & ReadyString(mOrgID) & vbCrLf
        Dim pDR = mBusiness.ProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Throw New ApplicationException("vbUTHWZH97VL! OrgID (" & mOrgID & ") from ""ThirdParty.txt"" has no record in MobizLibrarySubscription or MobizLibraryOrganization")
        End If
        mCompanyName = CStr2(pDR("CompanyName"))
        Dim pDatabase = CStr2(pDR("Database"))
        If String.IsNullOrWhiteSpace(pDatabase) Then
            Throw New ApplicationException("vb4S9ZDDBML4! OrgID (" & mOrgID & ") from ""ThirdParty.txt"" has no database in MobizLibrarySubscription")
        End If

        Dim pConnectionInfo = New ConnectionInfo()
        pConnectionInfo.Server = Common.ConnectionInfo.Server
        pConnectionInfo.Database = pDatabase
        pConnectionInfo.Username = Common.ConnectionInfo.Username
        pConnectionInfo.Password = Common.ConnectionInfo.Password
        pConnectionInfo.IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity
        mBusiness.ProData.Connection.Close()
        mBusiness = New Business(ProData.CreateFromConnectionInfo(pConnectionInfo))

    End Sub

    Function ProcessJsonInfo(ByRef oError As String) As Boolean

        If C1.C1Zip.C1ZipFile.IsZipFile(mZipFilename) = False Then
            oError = "This file is not a valid zip file."
            Return False
        End If
        Dim pInfo = New ProZip(mZipFilename).GetString("info.json")
        If CStr2(pInfo).Trim = "" Then
            oError = "Info.json does not exist or is empty." ' removed the ref: vb26CQKWA5ZY! tag
            Return False
        End If
        Try
            mJsonInfo = JsonConvert.DeserializeObject(Of JsonInfo)(pInfo)
        Catch ex As System.Exception
            oError = "Info.json does not contain a valid JSON"
            Return False
        End Try

        If String.IsNullOrWhiteSpace(mJsonInfo.StoreName) Then
            oError = "StoreName in json.info must not be blank."
            Return False
        End If

        Dim pSql = " SELECT MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE MobizLibraryOrganizationFOWST.Code = " & ReadyString(mJsonInfo.StoreName) & vbCrLf &
            " ORDER BY MobizLibraryOrganizationFOWST.IsActive DESC" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.LastUpdated DESC"
        Dim pDR = mBusiness.ProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            oError = "An error was found in Info.json: StoreName, " & mJsonInfo.StoreName & ", is not referencing a valid store."
            Return False
        End If
        mFowstID = CStr2(pDR("ID"))
        mFowstName = CStr2(pDR("Name"))
        mDeviceID = mBusiness.LibraryDevice.GetOrCreateByUuid(DeviceUuid)

        Return True

    End Function

    Function ProcessTransInvoice(ByRef oError As String) As DataTable

        Dim pBinder = New DictionaryBinder()
        With pBinder.Items
            .Add("InvoiceNumber", True, SqlDbType.Int)
            .Add("InvoiceDate", True, SqlDbType.DateTime)
            .Add("TotalAmount", True, SqlDbType.Money)

            .Add("ID", False, SqlDbType.UniqueIdentifier)
            .Add("CustomerID", False, SqlDbType.UniqueIdentifier)
            .Add("DeviceID", False, SqlDbType.UniqueIdentifier)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("FowstID", False, SqlDbType.UniqueIdentifier)
            .Add("BillingAddressID", False, SqlDbType.UniqueIdentifier)
            .Add("DeliveryAddressID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)

            .Add("ItemsCount", False, SqlDbType.Int) ' used for this class only
        End With

        Dim pDT = pBinder.CreateDataTable()

        Dim pCsv = New CsvReader(mZipFilename, "TransInvoice", True)
        Do
            Dim pLine = pCsv.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pBinder.Source = pLine
            Dim pDR = pBinder.CreateDataRow(pDT, oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                oError = "Error in TransInvoice.csv (line " & pCsv.RowIndex & "): " & oError & ". StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If
            If CDec2(pLine("TotalAmount")) <> CInt2(pLine("TotalAmount")) Then
                Debug.WriteLine("vbXAJGWJQ8R6! " & pLine("TotalAmount"))
            End If
            pDR("ID") = Guid.NewGuid().ToString()
            pDR("UserID") = mUserID
            pDR("FowstID") = mFowstID
            pDR("DeviceID") = mDeviceID
            pDR("CustomerID") = mCustomerID
            pDR("BillingAddressID") = mBillingAddressID
            pDR("DeliveryAddressID") = mDeliveryAddressID
            pDR("LastUpdated") = pDR("InvoiceDate")
            pDT.Rows.Add(pDR)
        Loop
        If pDT.Rows.Count = 0 Then
            oError = "TransInvoice.csv is required and must contain at least 1 row."
            Return Nothing
        End If

        Return pDT

    End Function

    Function ProcessTransInvoiceItems(ByRef oError As String) As DataTable

        Dim pBinder = New DictionaryBinder()
        With pBinder.Items
            .Add("InvoiceNumber", True, SqlDbType.Int)
            .Add("InvoiceItemNo", True, SqlDbType.Int)
            .Add("ProductNo", True, SqlDbType.VarChar)
            .Add("Quantity", True, SqlDbType.Int)
            .Add("UnitPrice", True, SqlDbType.Money)
            .Add("SubTotal", True, SqlDbType.Money)

            .Add("ID", False, SqlDbType.UniqueIdentifier)
            .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier) ' should be true; false to force import
            .Add("InvoiceID", False, SqlDbType.UniqueIdentifier)
            .Add("ProductID", False, SqlDbType.UniqueIdentifier) ' should be true; false to force import
            .Add("ActualQuantity", False, SqlDbType.Int)
            .Add("UnitMeasure", False, SqlDbType.VarChar) ' should be true; false to force import
        End With
        Dim pDT = pBinder.CreateDataTable()
        pDT.Columns.Add("ThirdPartyProductNumber")

        Dim pCsv = New CsvReader(mZipFilename, "TransInvoiceItems", True)
        Do
            Dim pLine = pCsv.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pBinder.Source = pLine
            Dim pDR = pBinder.CreateDataRow(pDT, oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                oError = "Error in TransInvoiceItems.csv (line " & pCsv.RowIndex & "): " & oError & ". StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If

            ProcessTransInvoiceItem(pDR, oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                oError = "Error in TransInvoiceItems.csv (line " & pCsv.RowIndex & "): " & oError & ". StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If
            mTotalAmount += CDec2(pLine("Quantity")) * CDec2(pLine("UnitPrice"))
            pDT.Rows.Add(pDR)
        Loop
        If pDT.Rows.Count = 0 Then
            oError = "TransInvoiceItems.csv is required and must contain at least 1 row."
            Return Nothing
        End If
        Dim pInvoicesWithoutItems =
            From pDrInvoice As DataRow In mDtInvoices.Rows
            Where CInt2(pDrInvoice("ItemsCount")) = 0
            Select pDrInvoice

        pInvoicesWithoutItems = pInvoicesWithoutItems.Take(5)
        If pInvoicesWithoutItems.Any() Then
            If pInvoicesWithoutItems.Count() > 5 Then
                oError = pInvoicesWithoutItems.Count() & " invoices have no items. Example: "
                Dim pIndex = -1
                For Each pInvoiceWithoutItems In pInvoicesWithoutItems
                    pIndex += 1
                    If pIndex = 4 Then
                        oError &= " & "
                    ElseIf pIndex > 4 Then
                        Exit For
                    ElseIf pIndex > 0 Then
                        oError &= ", "
                    End If
                    oError &= pInvoiceWithoutItems("InvoiceNumber")
                Next
                oError &= ". "
            ElseIf pInvoicesWithoutItems.Count() > 1 Then
                Dim pIndex = -1
                oError = pInvoicesWithoutItems.Count() & " invoices have no items. Invoice numbers: "
                For Each pInvoiceWithoutItems In pInvoicesWithoutItems
                    pIndex += 1
                    If pIndex = pInvoicesWithoutItems.Count() - 1 Then
                        oError &= " & "
                    ElseIf pIndex > 0 Then
                        oError &= ", "
                    End If
                    oError &= pInvoiceWithoutItems("InvoiceNumber")
                Next
                oError &= ". "
            Else
                oError = pInvoicesWithoutItems.Count() & " invoice have no items. Invoice number " & pInvoicesWithoutItems.First()("InvoiceNumber") & ". "
            End If
            oError &= "StoreName: " & mJsonInfo.StoreName
        End If

        Return pDT

    End Function

    Sub ProcessTransInvoiceItem(rDR As DataRow, ByRef oError As String)

        Dim pInvoices =
                From pDrInvoice As DataRow In mDtInvoices.Rows
                Where pDrInvoice("InvoiceNumber") = rDR("InvoiceNumber")
                Select pDrInvoice

        If pInvoices.Any() = False Then
            oError = "InvoiceNumber, " & rDR("InvoiceNumber") & ", does not reference to any invoice in the TransInvoices.csv."
            Return
        End If
        pInvoices.First()("ItemsCount") = CInt2(pInvoices.First()("ItemsCount")) + 1 ' used for this class only
        rDR("InvoiceID") = CStr2(pInvoices.First()("ID"))
        rDR("ID") = Guid.NewGuid().ToString()
        rDR("PriceLevelID") = mPriceLevelID

        ' delete this commented code: 2025_01_29__08_57_35_3478
        'If String.IsNullOrWhiteSpace(mReplacementProductNo) = False Then
        '    pDR("ProductNo") = mReplacementProductNo
        'End If

        Dim pProductID = mBusiness.LibraryProductMap.ThirdPartyProductNumberToProductID(mFowstID, rDR("ProductNo"))
        If String.IsNullOrWhiteSpace(pProductID) Then
            pProductID = mBusiness.LibraryStock.GetOrCreatePimID(mOrgID, mReplacementProductNo, "3rd Party Product")
            rDR("ThirdPartyProductNumber") = rDR("ProductNo")

            ' delete this commented code: 2025_01_29__08_57_35_3478
            'oError = "Error in TransInvoiceItems.csv (line " & pCsv.RowIndex & "): ProductNo, " & pDR("ProductNo") & ", does not reference to any product."
            'Return Nothing

        End If
        rDR("ProductID") = pProductID
        Dim pDtUnits = mBusiness.LibraryStockMeasurements.ByProductID(pProductID)
        Dim pUnitMeasureID As String = Nothing
        If pDtUnits.Rows.Count = 0 Then
            pUnitMeasureID = mBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("PC")
            mBusiness.LibraryStockMeasurements.GetOrCreatePimProductMeasurement(pProductID, pUnitMeasureID)
            pDtUnits = mBusiness.LibraryStockMeasurements.ByProductID(pProductID) ' not optimized, i know

            ' delete this commented code: 2025_01_29__08_57_35_3478
            'oError = "Error in TransInvoiceItems.csv (line " & pCsv.RowIndex & "): ProductNo, " & pDR("ProductNo") & ", does not have any unit measures."
            'Return Nothing

        End If

        Dim pUnitQty = CDec2(pDtUnits.Rows(0)("BaseUnitQty"))
        If pUnitQty <= 0 Then
            pUnitQty = 1
        End If
        rDR("ActualQuantity") = pUnitQty
        rDR("UnitMeasure") = CStr2(pDtUnits.Rows(0)("BaseUnit"))

    End Sub

    Function ProcessTransInvoiceItemsModifier(ByRef oError As String) As DataTable

        Dim pBinder = New DictionaryBinder()
        With pBinder.Items
            .Add("InvoiceItemNo", True, SqlDbType.Int)
            .Add("OrderedQty", True, SqlDbType.Decimal) ' sub qty
            .Add("ProductNo", True, SqlDbType.VarChar)
            .Add("ModifierPrice", True, SqlDbType.Money)
            .Add("PremiumPrice", False, SqlDbType.Money)
            .Add("InputQty", True, SqlDbType.Decimal) ' qty for 1 item

            .Add("ID", False, SqlDbType.UniqueIdentifier)
            .Add("InvoiceItemID", False, SqlDbType.UniqueIdentifier)
            .Add("ItemModifierID", False, SqlDbType.UniqueIdentifier) ' LibraryItemModifiers.ID
            .Add("ModifierID", False, SqlDbType.UniqueIdentifier) ' LibraryItemModifierInventoryAffecting.ID
            .Add("OtherDescription", False, SqlDbType.VarChar)
            .Add("SubTotal", False, SqlDbType.Money) ' (CDec2(item.OrderQty) * CDec2(item.UnitPrice)) + (CBool(item.PremiumPriceApplied) ? CDec2(item.PremiumPrice) : 0))
        End With
        Dim pDT = pBinder.CreateDataTable()

        Dim pCsv = New CsvReader(mZipFilename, "TransInvoiceItemsModifier", True)
        Do
            Dim pLine = pCsv.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pBinder.Source = pLine
            Dim pDR = pBinder.CreateDataRow(pDT, oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                oError = "Error in TransInvoiceItemsModifier.csv (line " & pCsv.RowIndex & "): " & oError & ". StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If

            Dim pDrInvoiceItems =
                From pDrInvoiceItem As DataRow In mDtInvoiceItems.Rows
                Where pDrInvoiceItem("InvoiceItemNo") = pLine("InvoiceItemNo")
                Select pDrInvoiceItem

            If pDrInvoiceItems.Any() = False Then
                oError = "Error in ProcessTransInvoiceItemsModifier: InvoiceItemNo, " & pLine("InvoiceItemNo") & ", does not reference to any invoice item in the TransInvoiceItems.csv. StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If

            Dim pParentProductID = CStr2(pDrInvoiceItems.First()("ProductID"))
            If String.IsNullOrWhiteSpace(mReplacementModifierProductNo) = False Then
                pLine("ProductNo") = mReplacementModifierProductNo
            End If
            Dim pDrModifier = mBusiness.LibraryItemModifiers.ByParentProductIdAndModifierProductNumber(pParentProductID, pLine("ProductNo"))
            If pDrModifier Is Nothing Then
                Dim pDrItem = mDtInvoiceItems.NewRow()
                pDrItem("InvoiceNumber") = pDrInvoiceItems.First()("InvoiceNumber")
                pDrItem("ProductNo") = pDR("ProductNo")
                pDrItem("Quantity") = CDec2(pDR("OrderedQty"))
                pDrItem("UnitPrice") = CDec2(pDR("ModifierPrice"))
                pDrItem("SubTotal") = CDec2(pDR("OrderedQty")) * CDec2(pDR("ModifierPrice")) + CDec2(pDR("PremiumPrice"))
                ProcessTransInvoiceItem(pDrItem, oError)
                If (String.IsNullOrWhiteSpace(oError) = False) Then
                    Return Nothing
                End If
                mDtInvoiceItems.Rows.Add(pDrItem)
                'oError = "Error in ProcessTransInvoiceItemsModifier: Item ProductNo, " & pDrInvoiceItems.First()("ProductNo") & ", and modifier ProductNo, " & pLine("ProductNo") & ", does not reference to any record in item modifier library."
                Continue Do
            End If

            pDR("ID") = Guid.NewGuid().ToString()
            pDR("InvoiceItemID") = CStr2(pDrInvoiceItems.First()("ID"))
            pDR("ItemModifierID") = CStr2(pDrModifier("ItemModifierID"))
            pDR("ModifierID") = CStr2(pDrModifier("ModifierID"))
            pDR("SubTotal") = CDec2(pDR("OrderedQty")) * CDec2(pDR("ModifierPrice")) + CDec2(pDR("PremiumPrice"))

            pDT.Rows.Add(pDR)
        Loop

        Return pDT

    End Function

    Function ProcessTransCustomerPayment(ByRef oError As String) As DataTable

        Dim pBinder = New DictionaryBinder()
        With pBinder.Items
            .Add("InvoiceNumber", True, SqlDbType.Int)
            .Add("CashAmount", True, SqlDbType.Money)
            .Add("PaymentDate", True, SqlDbType.DateTime)

            .Add("CustomerID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("ID", False, SqlDbType.UniqueIdentifier)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("CheckPaymentAmount", False, SqlDbType.Money)
            .Add("AmountChange", False, SqlDbType.Money)
            .Add("SRVAmount", False, SqlDbType.Money)
            .Add("OtherTypeAmount", False, SqlDbType.Money)
            .Add("ChargeAmount", False, SqlDbType.Money) 'new
            .Add("CreditCardAmount", False, SqlDbType.Money)
            .Add("CreditCardVat", False, SqlDbType.Money)
            .Add("DiscountAmount", False, SqlDbType.Money)
            .Add("DebitCardAmount", False, SqlDbType.Money)
            .Add("TotalAmountDue", False, SqlDbType.Money)
            .Add("CustomerChangeCash", False, SqlDbType.Money)
            .Add("ReceiptType", False, SqlDbType.VarChar)
            .Add("ReceiptNumber", False, SqlDbType.VarChar)
            .Add("OnCreditAmount", False, SqlDbType.Money)
            .Add("OnCreditRemarks", False, SqlDbType.VarChar)
            .Add("RewardsCardNum", False, SqlDbType.VarChar)
            .Add("CustomerEmailAddresses", False, SqlDbType.VarChar)
        End With
        Dim pDT = pBinder.CreateDataTable()

        Dim pCsv = New CsvReader(mZipFilename, "TransCustomerPayment", True)
        Do
            Dim pLine = pCsv.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pBinder.Source = pLine
            Dim pDR = pBinder.CreateDataRow(pDT, oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                oError = "Error in TransCustomerPayment.csv (line " & pCsv.RowIndex & "): " & oError & ". StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If

            Dim pInvoices =
                From pDrInvoice As DataRow In mDtInvoices.Rows
                Where pDrInvoice("InvoiceNumber") = pLine("InvoiceNumber")
                Select pDrInvoice

            If pInvoices.Any() = False Then
                oError = "Error in TransCustomerPayment.csv (line " & pCsv.RowIndex & "): InvoiceNumber, " & pLine("InvoiceNumber") & ", does not reference to any invoice in the TransInvoices.csv." & ". StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If
            pDR("CustomerID") = mCustomerID
            pDR("ID") = Guid.NewGuid().ToString()
            pDR("UserID") = mUserID
            pDR("CheckPaymentAmount") = 0
            pDR("AmountChange") = 0
            pDR("SRVAmount") = 0
            pDR("OtherTypeAmount") = 0
            pDR("ChargeAmount") = 0
            pDR("CreditCardAmount") = 0
            pDR("CreditCardVat") = 0
            pDR("DiscountAmount") = 0
            pDR("DebitCardAmount") = 0
            pDR("TotalAmountDue") = CDec2(pInvoices.First()("TotalAmount"))
            pDR("CustomerChangeCash") = 0
            pDR("ReceiptType") = "OR"
            pDR("ReceiptNumber") = pLine("InvoiceNumber")
            pDR("PaymentDate") = CDate(pInvoices.First()("InvoiceDate"))
            pDR("OnCreditAmount") = 0
            pDR("OnCreditRemarks") = ""
            pDR("RewardsCardNum") = ""
            pDR("CustomerEmailAddresses") = ""

            pDT.Rows.Add(pDR)

            AddCustomerPaymentInvoiceRow(mDtPaymentInvoices, pDR("ID"), pInvoices.First())
        Loop

        Return pDT

    End Function

    Sub AddCustomerPaymentInvoiceRow(ByRef oDT As DataTable, vPaymentID As String, vDrInvoice As DataRow)

        If oDT Is Nothing Then
            oDT = New DataTable
            With oDT.Columns
                .Add("CustomerPaymentID")
                .Add("ID")
                .Add("InvoiceID")
                .Add("Balance", GetType(Decimal))
                .Add("DiscountAmount", GetType(Decimal))
                .Add("InvoiceAmount", GetType(Decimal))
                .Add("AmountPaid", GetType(Decimal))
            End With
        End If

        Dim pDR = oDT.NewRow()
        pDR("ID") = Guid.NewGuid().ToString()
        pDR("CustomerPaymentID") = vPaymentID
        pDR("InvoiceID") = CStr2(vDrInvoice("ID"))
        pDR("Balance") = 0
        pDR("DiscountAmount") = 0
        pDR("InvoiceAmount") = CDec2(vDrInvoice("TotalAmount"))
        pDR("AmountPaid") = CDec2(vDrInvoice("TotalAmount"))
        oDT.Rows.Add(pDR)

    End Sub

    Function ProcessTransCustomerPaymentOther(ByRef oError As String) As DataTable

        Dim pBinder = New DictionaryBinder()
        With pBinder.Items
            .Add("InvoiceNumber", True, SqlDbType.Int)
            .Add("Amount", True, SqlDbType.Money)
            .Add("PaymentType", True, SqlDbType.VarChar,, 50)
            .Add("IssuingOrganization", False, SqlDbType.VarChar,, 255)
            .Add("CardNumber", False, SqlDbType.VarChar,, 30)
            .Add("CardHolderName", False, SqlDbType.VarChar, , 255)
            .Add("CardIssuanceDate", False, SqlDbType.Date)
            .Add("CardExpirationDate", False, SqlDbType.Date)
            .Add("ApprovalNo", False, SqlDbType.VarChar,, 30)
            .Add("GCashRefNo", False, SqlDbType.VarChar,, 50)

            .Add("ID", False, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", False, SqlDbType.UniqueIdentifier)
        End With
        Dim pDT = pBinder.CreateDataTable()

        Dim pCsv = New CsvReader(mZipFilename, "TransCustomerPaymentOther", True)
        Do
            Dim pLine = pCsv.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pBinder.Source = pLine
            Dim pDR = pBinder.CreateDataRow(pDT, oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                Return Nothing
            End If

            Dim pPayments =
                From pDrPayment As DataRow In mDtPayments.Rows
                Where pDrPayment("InvoiceNumber") = pLine("InvoiceNumber")
                Select pDrPayment

            If pPayments.Any() = False Then
                oError = "Error in TransCustomerPaymentOther.csv (line " & pCsv.RowIndex & "): InvoiceNumber, " & pLine("InvoiceNumber") & ", does not reference to any row in the TransCustomerPayment.csv." & ". StoreName: " & mJsonInfo.StoreName
                Return Nothing
            End If
            pDR("ID") = Guid.NewGuid().ToString()
            pDR("CustomerPaymentID") = CStr2(pPayments.First()("ID"))

            pDT.Rows.Add(pDR)
        Loop

        Return pDT

    End Function

    Function ProcessTransSalesReturn(ByRef oError As String) As DataTable

        Dim pBinder = New DictionaryBinder()
        With pBinder.Items
            .Add("InvoiceNumber", False, SqlDbType.Int)
            .Add("SalesReturnDate", True, SqlDbType.DateTime)
            .Add("ReturnedAmount", True, SqlDbType.Money)
            .Add("ReturnProductNo", True, SqlDbType.VarChar)
        End With

        Dim pDT = pBinder.CreateDataTable()

        Dim pCsv = New CsvReader(mZipFilename, "TransSalesReturn", True)
        Do
            Dim pLine = pCsv.ReadLine()
            If pLine Is Nothing Then
                Exit Do
            End If
            pBinder.Source = pLine
            Dim pDR = pBinder.CreateDataRow(pDT, oError)
            If String.IsNullOrWhiteSpace(oError) = False Then
                Return Nothing
            End If
            pDR("ID") = Guid.NewGuid().ToString()
            pDR("UserID") = mUserID
            pDR("FowstID") = mFowstID
            pDR("DeviceID") = mDeviceID
            pDR("CustomerID") = mCustomerID
            pDR("BillingAddressID") = mBillingAddressID
            pDR("DeliveryAddressID") = mDeliveryAddressID
            pDR("LastUpdated") = pDR("InvoiceDate")
            pDT.Rows.Add(pDR)
        Loop

        Return pDT

    End Function

    Public Class DumpResult
        Public d As dClass
        Public Class dClass
            Public TimeStamp As String
            Public Filename As String
            Public [Error] As String
        End Class
    End Class

    Sub MakeFlushableZip()

        Dim pFilename = IO.Path.GetDirectoryName(mZipFilename)
        pFilename = IO.Path.Combine(pFilename, "flushable_" & DateTimeStamp() & ".zip")
        If IO.File.Exists(pFilename) Then
            IO.File.Delete(pFilename)
        End If
        Dim pZip As New ProZip(pFilename)
        Dim pJson As New JsonDictionary()
        pJson.Add("OrgID", mOrgID)
        pJson.Add("UUID", DeviceUuid)
        pJson.Add("AppVersion", "from FlushZipConverter")
        pJson.Add("AppVersionName", "from FlushZipConverter")
        pJson.Add("FromFtp", "1")
        pJson.Add("AppBuildDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
        pJson.Add("AppInstallDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
        pJson.Add("ThirdPartyFilename", mZipFilename)
        pZip.AddString(pJson.ToString(), "info.json")

        mDtInvoices.Columns.Remove("ItemsCount")
        mDtInvoices = New DataView(mDtInvoices, "", "InvoiceNumber", DataViewRowState.CurrentRows).ToTable()
        pZip.AddTableCsv(mDtInvoices, "TransInvoice")
        mDtInvoiceItems.Columns.Remove("ProductNo")
        pZip.AddTableCsv(mDtInvoiceItems, "TransInvoiceItems")
        pZip.AddTableCsv(mDtItemsModifier, "TransInvoiceItemsModifier")
        pZip.AddTableCsv(mDtPayments, "TransCustomerPayment")
        pZip.AddTableCsv(mDtPaymentInvoices, "TransCustomerPaymentInvoices")
        pZip.AddTableCsv(mDtPaymentsOther, "TransCustomerPaymentOther")

        Dim pUrl = BaseUrl & "/OfflineData/Dump"
        Dim pValues = New NameValueCollection()
        pValues.Add("OrgID", mOrgID)
        pValues.Add("DeviceID", mDeviceID)
        pValues.Add("DontDeleteUnprocessedTransImport", True)
        Dim pResult = UploadFile(pFilename, pUrl, pValues)
        Try
            pResult = CStr2(pResult)
            Dim pDumpResult = JsonConvert.DeserializeObject(Of DumpResult)(pResult)
            If String.IsNullOrWhiteSpace(pDumpResult?.d?.Error) = False Then
                Throw New ApplicationException("vb9CH6W92MUW! Error during flushing of third party zip file: " & pResult)
            End If
            If String.IsNullOrWhiteSpace(pDumpResult?.d?.TimeStamp) Then
                Throw New ApplicationException("vbWUNYS4S2B8! Expected ""Timestamp"" was not found in third party flushing result.")
            End If
            If String.IsNullOrWhiteSpace(pDumpResult?.d?.Filename) Then
                Throw New ApplicationException("vbH2KT6UGAQZ! Expected ""Filename"" was not found in third party flushing result.")
            End If
            CreateSummary(pDumpResult.d.Filename)
        Catch ex As Exception
            AppendToLog("vbL7UG6NVUYY! unexpected error" & vbCrLf & ex.Message)
        Finally
            Try
                System.IO.File.Delete(pFilename)
            Catch ex As Exception
                AppendToLog("vbJKQ7R3MPKP! An error occured while deleteting file, " & pFilename & vbCrLf &
                    ex.Message)
            End Try
        End Try

    End Sub

    Public Class Summary
        Public OrganizationName As String
        Public FowstName As String
        Public StoreName As String
        Public MinInvoiceNumber As Integer?
        Public MaxInvoiceNumber As Integer?
        Public MinInvoiceDate As Date?
        Public MaxInvoiceDate As Date?
        Public InvoiceTotalAmount As Decimal
        Public InvoiceCount As Integer
        Public SkippedInvoiceNumbers As Integer
        Public OrganizationID As String
        Public FowstID As String
        Public DeviceID As String
        Public FlushFilename As String
    End Class

    Sub CreateSummary(vFlushFilename As String)

        Dim pSummary = New Summary()

        pSummary.OrganizationName = mCompanyName
        pSummary.OrganizationID = mOrgID
        pSummary.FowstName = mFowstName
        pSummary.FowstID = mFowstID
        pSummary.StoreName = mJsonInfo.StoreName
        pSummary.FlushFilename = vFlushFilename
        pSummary.DeviceID = mDeviceID
        If mDtInvoices.Rows.Count > 0 Then
            pSummary.MinInvoiceNumber = mDtInvoices.Rows(0)("InvoiceNumber")
            pSummary.MaxInvoiceNumber = mDtInvoices.Rows(mDtInvoices.Rows.Count - 1)("InvoiceNumber")
            pSummary.InvoiceTotalAmount = mTotalAmount ' CDec2(mDtInvoiceItems.Compute("SUM(SubTotal)", ""))
            pSummary.InvoiceCount = mDtInvoices.Rows.Count
            pSummary.SkippedInvoiceNumbers = pSummary.MaxInvoiceNumber - pSummary.MinInvoiceNumber - pSummary.InvoiceCount + 1
            pSummary.MinInvoiceDate = mDtInvoices.Compute("MIN(InvoiceDate)", "")
            pSummary.MaxInvoiceDate = mDtInvoices.Compute("MAX(InvoiceDate)", "")
        End If

        Dim pFlushFilename = Path.Combine(Path.GetDirectoryName(mZipFilename), Path.GetFileNameWithoutExtension(mZipFilename) & "_flush.txt")
        Dim pText = JsonConvert.SerializeObject(pSummary, Formatting.Indented)
        FileText(pFlushFilename) = pText

    End Sub

    Public Class JsonInfo
        Public OrgID As String
        Public StoreName As String
        ''' <summary>
        ''' applicable for cyberpos
        ''' </summary>
        Public CsvFilename As String
    End Class

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Try
                    mBusiness.ProData.Dispose()
                Catch ex As Exception
                    Debug.WriteLine(ex.Message)
                End Try
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
