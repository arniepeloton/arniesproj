Imports System.Xml.Serialization

Public Class PoXmlImport
    Dim mBusiness As Business
    Public SourceFilename As String

    Public Sub New(vBusiness As Business)

        mBusiness = vBusiness

    End Sub

    Public Function ImportSm() As String

        Using pFS = IO.File.Open(SourceFilename, IO.FileMode.Open)
            Dim pSerializer = New XmlSerializer(GetType(PoXmlSm.xml))
            Dim pXml = CType(pSerializer.Deserialize(pFS), PoXmlSm.xml)
            Dim pID = ImportHeader(pXml)
            Debug.WriteLine(pXml.document.header.DocumentType)
            Return pID
        End Using

    End Function

    Private Function ImportHeader(vXml As PoXmlSm.xml) As String

        Dim pSql As String

        If vXml.document.header.DocumentType <> "Purchase Order" Then
            Return "DocumentType must be Purchase Order"
        End If

        mBusiness.ProData.BeginTransaction()
        Dim pCustomerID = mBusiness.LibraryOrganization.GetOrAddByCompanyName(vXml.document.header.CompanyName, "", True, False)
        Dim pDrFowst = mBusiness.LibraryOrganizationFOWST.GetOrAddFowstByName(
            vOrgID:=pCustomerID,
            vFowstName:=vXml.document.header.SiteName,
            vStreet:=vXml.document.header.SiteAddress,
            vThirdPartySystemID:=vXml.document.header.SiteCode,
            vTINBranchCode:=vXml.document.header.SiteTIN)
        Dim pCreator = mBusiness.TransInvoice.CreateCreator()
        pCreator.CustomerID = pCustomerID
        pCreator.InvoiceNumber = 0
        pCreator.FowstID = CStr2(pDrFowst("ID"))
        pCreator.TransType = "Invoice"
        pCreator.UserID = WebSession.Instance.PeopleUserID
        pCreator.BillingAddressID = pCreator.FowstID
        pCreator.DeliveryAddressID = pCreator.FowstID
        pCreator.DeviceID = WebRequest.Instance.DeviceID
        pCreator.PurchaseOrderText = vXml.document.header.DocumentNumber
        pCreator.Stocks = New DataTable
        With pCreator.Stocks.Columns
            .Add("ID")
            .Add("InvoiceID")
            .Add("StockID")
            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", GetType(Decimal))
            .Add("ActualQuantity", GetType(Decimal))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("OriginalUnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            .Add("ModifierGroupID")
        End With

        For Each pItem In vXml.document.body
            Dim pDR = pCreator.Stocks.NewRow()
            pDR("StockID") = mBusiness.LibraryStock.GetOrCreatePimByName(pItem.ArticleDescription.Description)
            mBusiness.LibraryStock.AddToOrg(CStr2(pDR("StockID")))
            If String.IsNullOrWhiteSpace(pItem.BuyUM) = False Then
                Dim pUnitMeasureID = mBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID(pItem.BuyUM)
                Dim pProductMeasurementID = mBusiness.LibraryStockMeasurements.GetOrCreatePimProductMeasurement(CStr2(pDR("StockID")), pUnitMeasureID, CDec2(pItem.Package))
                If String.IsNullOrWhiteSpace(pItem.UPC) = False Then
                    mBusiness.LibraryStockMeasurements.AddBarcode(pProductMeasurementID, pItem.UPC)
                End If
            End If
            Dim pIU = New InsertUpdate("MobizLibraryPIMProduct")
            pIU("ProductNo") = ReadyString(pItem.SKUMatCode)
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("StockID")))
            pSql = mBusiness.ProData.ExecuteSQL(pSql) ' output is used for debugging only

            pDR("OrderedQuantity") = CDec2(pItem.BuyQty)
            pDR("Quantity") = CDec2(pItem.BuyQty)
            pDR("ActualQuantity") = CDec2(pItem.Package)
            pDR("UnitMeasure") = CStr2(pItem.BuyUM)
            pDR("UnitPrice") = CDec2(pItem.BuyCost)
            pDR("Subtotal") = CDec2(pDR("UnitPrice")) * CDec2(pDR("Quantity"))
            pDR("Cost") = 0 ' todo
            pDR("PriceLevelID") = CStr2(mBusiness.SystemDefaults.GetRetailPriceID)
            pCreator.Stocks.Rows.Add(pDR)
        Next pItem

        Dim pError As String
        If pCreator.Save(True, pError) = False Then
            mBusiness.ProData.RollbackTransaction()
            Return pError
        End If

        'mBusiness.ProData.RollbackTransaction() ' for debugging only
        mBusiness.ProData.CommitTransaction() ' for prod
        Return pCreator.ID

    End Function

End Class
