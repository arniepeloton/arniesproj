Public Class Receiving

    <HttpPost()>
    Public Function GetAllReceiving() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .SearchString = WebRequest.Instance.Data("data")

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.TransReceiving.ID"
                End With

                .Add("ReceivingNo", SqlDbType.Int)
                .Add("IsReceiving", SqlDbType.Bit)

                With .Add("ReceivingPlaceID", SqlDbType.VarChar)
                    .Searchable = False
                End With

                With .Add("ReceivingPlace", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("SourceOfSKU1ID", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "MobizLibraryOrganizationFowst_1.ID"
                End With

                With .Add("SourceOfSKU1", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFowst_1.Name"
                End With

                With .Add("SourceOfSKU2ID", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "MobizLibraryOrganization_1.ID"
                End With

                With .Add("SourceOfSKU2", SqlDbType.VarChar)
                    .Expression = " MobizLibraryOrganization_1.CompanyName"
                End With

                .Add("DateReceived", SqlDbType.DateTime)

                .Add("DocumentDate", SqlDbType.DateTime)
                .Add("DocumentType", SqlDbType.VarChar)
                .Add("DocumentNo", SqlDbType.VarChar)
                .Add("Is3PL", SqlDbType.Bit)

                With .Add("ShipperID", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryOrganization.ID"
                End With

                With .Add("Shipper", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganization.CompanyName"
                End With

                .Add("VesselID", SqlDbType.VarChar)
                .Add("DatePrepared", SqlDbType.DateTime)
                .Add("VoyageNumber", SqlDbType.VarChar)
                .Add("WaybillNumber", SqlDbType.VarChar)

                .Add("InvoiceDate", SqlDbType.DateTime)
                .Add("InvoiceNumber", SqlDbType.VarChar)

                .Add("FreightAndHandling", SqlDbType.Money)
                .Add("Arrastre", SqlDbType.Money)
                .Add("Total", SqlDbType.Money)
                .Add("TotalCC", SqlDbType.Float)
                .Add("TotalCBM", SqlDbType.Float)

                .Add("TotalWeightInKg", SqlDbType.Float)
                .Add("TotalCostOfBundle", SqlDbType.Money)
                .Add("Bundles", SqlDbType.VarChar)


                .Add("IsResaleable", SqlDbType.VarChar)
                .Add("ResaleableReason", SqlDbType.VarChar)
                .Add("ResaleableRemarks", SqlDbType.VarChar)

                With .Add("ReceivedBy", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPeople.Surname + ', ' + dbo.MobizLibraryPeople.FirstName"
                End With

                With .Add("ReceiveFromOutletName", SqlDbType.VarChar)
                    .Expression = "ISNULL(tblReceiveFromOutlet.Name, '')"
                End With

                With .Add("ReceiveFromOutlet", SqlDbType.VarChar)
                    .Expression = "ISNULL(CONVERT(varchar(MAX), TransReceiving.ReceiveFromOutlet), '')"
                End With

                .Add("TransportedBy", SqlDbType.VarChar)

                .Add("TransferOutToReceivingID", SqlDbType.VarChar)

            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                      "dbo.TransReceiving ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransReceiving.ReceivingPlaceID LEFT JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganization ON dbo.TransReceiving.ShipperID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPeople ON dbo.TransReceiving.UserID = dbo.MobizLibraryPeople.ID LEFT OUTER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganization AS MobizLibraryOrganization_1 ON dbo.TransReceiving.SupplierID = MobizLibraryOrganization_1.ID LEFT OUTER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST AS MobizLibraryOrganizationFowst_1 ON dbo.TransReceiving.SupplierID = MobizLibraryOrganizationFowst_1.ID " & vbCrLf &
                      " LEFT JOIN MobizLibraryOrganizationFOWST tblReceiveFromOutlet ON tblReceiveFromOutlet.ID = TransReceiving.ReceiveFromOutlet "


            If Not Common.WebRequest.Data("ID") Is Nothing Then
                .SqlWhere = "WHERE TransReceiving.ID = " & ReadyString(Common.WebRequest.Data("ID"))
            End If

            .SqlOrder = "ORDER BY ReceivingNo desc"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetAllReceivingSKU() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "dbo.TransReceivingStock.ProductID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductNo"
                End With
                .Add("UnitMeasureGood", SqlDbType.UniqueIdentifier)
                .Add("ActualQuantityGood", SqlDbType.Int)
                .Add("ProductName", SqlDbType.VarChar)
                .Add("QuantityGood", SqlDbType.Int)
                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "QuantityGood"
                End With
                .Add("Label", SqlDbType.VarChar)
                With .Add("UnitMeasure", SqlDbType.Int)
                    .Expression = "Label"
                End With
                With .Add("UnitMeasureID", SqlDbType.Int)
                    .Expression = "UnitMeasureGood"
                End With
            End With
            .SqlFrom = "FROM TransReceivingStock " & vbCrLf &
                        "INNER JOIN mobizlibrarypimproduct on mobizlibrarypimproduct.id = TransReceivingStock.ProductID " & vbCrLf &
                        "INNER JOIN dbo.MobizLibraryPIMProductMeasurement on MobizLibraryPIMProductMeasurement.ProductID = TransReceivingStock.ProductID " & vbCrLf &
                        "INNER JOIN dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID  " & vbCrLf &
                        "INNER JOIN dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID"

            .SqlWhere = "Where dbo.TransReceivingStock.ReceivingID = " & ReadyString(WebRequest.Instance.Data("ID"))

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetStockUnitMeasures() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                    .Expression = "dbo.MobizLibraryPIMMeasure.ID"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With
                With .Add("Quantity", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProductMeasurement.Quantity"
                End With
                With .Add("Price", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMproductPriceLevel.Price"
                End With

                If Not WebRequest.Instance.Data("ReceivingID") = "" Then
                    With .Add("QuantityGood", SqlDbType.VarChar)
                        .Expression = "dbo.TransReceivingStock.QuantityGood"
                    End With
                    With .Add("QuantityDamaged", SqlDbType.VarChar)
                        .Expression = "dbo.TransReceivingStock.QuantityDamaged"
                    End With
                    .Add("BadOrderReason", SqlDbType.VarChar)
                    .Add("AdditionalInfo", SqlDbType.VarChar)
                    .Add("IsResaleable", SqlDbType.Bit)
                End If

            End With

            If Not WebRequest.Instance.Data("ReceivingID") = "" Then
                .SqlFrom = "FROM dbo.TransReceivingStock INNER JOIN " & _
                      "dbo.MobizLibraryStockMeasurements ON dbo.TransReceivingStock.StockID = dbo.MobizLibraryStockMeasurements.StockID INNER JOIN " & _
                      "dbo.MobizLibraryUnitMeasures ON dbo.MobizLibraryStockMeasurements.UnitMeasureID = dbo.MobizLibraryUnitMeasures.ID " &
                      "AND dbo.TransReceivingStock.UnitMeasureGood = dbo.MobizLibraryUnitMeasures.UnitMeasure"

                .SqlWhere = "WHERE dbo.MobizLibraryStockMeasurements.StockID = " & ReadyString(WebRequest.Instance.Data("StockID")) & _
                            " AND dbo.TransReceivingStock.ReceivingID = " & ReadyString(WebRequest.Instance.Data("ReceivingID"))
            Else
                .SqlFrom = "FROM dbo.MobizLibraryPIMProductMeasurement INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID " & vbCrLf &
                      "inner join MobizLibraryPIMproductPriceLevel on MobizLibraryPIMproductPriceLevel.ProductID = MobizLibraryPIMProductMeasurement.ProductID " & vbCrLf &
                      "inner join MobizLibraryPIMPriceLevel on MobizLibraryPIMPriceLevel.ID = MobizLibraryPIMproductPriceLevel.PriceLevelID"

                .SqlWhere = "WHERE dbo.MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & " and MobizLibraryPIMPriceLevel.PriceLevel='Retail Price' AND MobizLibraryPIMproductPriceLevel.OrganizationID =" & ReadyString(WebRequest.Instance.OrgID)
            End If

            .SqlOrder = "Order by Quantity"
            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SaveReceiving(ByVal ID As String, ByVal IsNew As Boolean) As String
        Dim p As New InsertUpdate("TransReceiving")
        Dim pSql As String
        Dim Quantity As Double
        Dim QuantityBad As Double
        Dim StockID As String
        Dim Total As Decimal ' from Integer to Decimal [4/16/2014 9:07 AM]

        If IsNew Then
            ID = Guid.NewGuid.ToString
            p("ID") = ReadyString(ID)
            If IsGuid(WebRequest.Instance.Data("DeviceID")) Then
                pSql = " AND DeviceID = " & ReadyString(WebRequest.Instance.Data("DeviceID"))
            Else
                pSql = " AND DeviceID IS NULL"
            End If
            pSql = "SELECT MAX(ReceivingNo)" &
                " FROM TransReceiving" &
                " WHERE IsReceiving = " & If(CBool2(WebRequest.Instance.Data("IsReceiving")), 1, 0) &
                pSql
            p("ReceivingNo") = "ISNULL((" & pSql & "), 0) + 1"
        End If

        If IsGuid(WebRequest.Instance.Data("DeviceID")) Then
            p("DeviceID") = ReadyString(WebRequest.Instance.Data("DeviceID"))
        End If
        p("IsReceiving") = WebRequest.Instance.Data("IsReceiving")
        p("ReceivingPlaceID") = ReadyString(WebRequest.Instance.Data("ReceivingPlace"))
        p("SupplierID") = ReadyString(WebRequest.Instance.Data("SourceOfSKU"))

        p("DocumentDate") = ReadyString(CDate(WebRequest.Instance.Data("DocumentDate")))
        p("DocumentType") = ReadyString(WebRequest.Instance.Data("DocumentType"))
        p("DocumentNo") = ReadyString(WebRequest.Instance.Data("DocumentNo"))

        p("Is3PL") = WebRequest.Instance.Data("Is3PL")

        If Not WebRequest.Instance.Data("ShipperCompany") = vbNullString Then
            p("ShipperID") = ReadyString(WebRequest.Instance.Data("ShipperCompany"))
            p("WaybillNumber") = ReadyString(WebRequest.Instance.Data("BLNo"))
            p("DatePrepared") = ReadyString(CDate(WebRequest.Instance.Data("DatePrepared")))

            p("VesselID") = ReadyString(WebRequest.Instance.Data("Vessel"))
            p("VoyageNumber") = ReadyString(WebRequest.Instance.Data("VoyageNumber"))
            p("InvoiceDate") = ReadyString(WebRequest.Instance.Data("InvoiceDate"))
            p("InvoiceNumber") = ReadyString(WebRequest.Instance.Data("InvoiceNo"))
            p("FreightAndHandling") = CInt2(WebRequest.Instance.Data("FAndH"))
            p("Arrastre") = CInt2(WebRequest.Instance.Data("Arrastre"))
            p("Total") = CInt2(WebRequest.Instance.Data("Total"))
            p("TotalCC") = CInt2(WebRequest.Instance.Data("TotalCC"))
            p("TotalCBM") = CInt2(WebRequest.Instance.Data("TotalCBM"))
            p("TotalWeightInKg") = CInt2(WebRequest.Instance.Data("TotalWeightKg"))
            p("TotalCostOfBundle") = CInt2(WebRequest.Instance.Data("TotalCostBundle"))
            p("Bundles") = ReadyString(WebRequest.Instance.Data("Bundles"))
        End If

        'transfer out
        If Not CBool(WebRequest.Instance.Data("IsReceiving")) Then
            p("IsResaleable") = WebRequest.Instance.Data("IsResaleable")
            p("ResaleableReason") = ReadyString(WebRequest.Instance.Data("IsResaleableAnswer"))
            p("ResaleableRemarks") = ReadyString(WebRequest.Instance.Data("IsResaleableRemarks"))
            p("HasIssues") = 1
            p("IssueRemarks") = ReadyString("No stocks received in destination")
        End If

        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        p("TransportedBy") = ReadyString(WebRequest.Instance.Data("TransportedBy"))

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If IsNew Then
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate("ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If Not IsNew Then
                'AddToInventoryLedger(ID, StockID, Total)
                pSql = "Delete from TransReceivingStock where ReceivingID=" & ReadyString(ID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            For Each pObj As Object In Common.WebRequest.AllData("Items")
                p = New InsertUpdate("TransReceivingStock")

                Quantity = IIf(pObj("Quantity") = "", 0, pObj("Quantity"))
                QuantityBad = IIf(pObj("QuantityBad") = "", 0, pObj("QuantityBad"))

                If StockID = vbNullString Then
                    StockID = pObj("ID")
                ElseIf Not StockID = pObj("ID") Then
                    AddToInventoryLedger(ID, StockID, Total)
                    StockID = pObj("ID")
                    Total = 0
                End If

                If Quantity > 0 OrElse QuantityBad > 0 Then
                    p("ReceivingID") = ReadyString(ID)
                    p("ProductID") = ReadyString(pObj("ID"))
                    p("QuantityGood") = Quantity
                    p("ActualQuantityGood") = Quantity '* pObj("ActualQuantity")
                    p("UnitMeasureGood") = ReadyString(pObj("UnitMeasureID"))
                    p("QuantityDamaged") = QuantityBad
                    p("ActualQuantityDamaged") = QuantityBad '* pObj("ActualQuantity")
                    p("UnitMeasureDamaged") = ReadyString(pObj("UnitMeasureID"))
                    p("BadOrderReason") = ReadyString(pObj("BadOrderReason"))
                    p("AdditionalInfo") = ReadyString(pObj("AdditionalInfo"))
                    p("IsResaleable") = pObj("IsResaleable")
                    pSql = p.GetInsert()

                    Total += Quantity '* pObj("ActualQuantity")
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next
            WebRequest.Instance.Business.TransReceiving.UpdatePoReceivedPercent(ID)

            Common.WebRequest.ProData.CommitTransaction()

            AddToInventoryLedger(ID, StockID, Total)

            Dim pFinder As Finder
            Dim pDT As DataTable

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = 1

                With .Fields
                    .Add("ID", SqlDbType.UniqueIdentifier)
                    .Add("ReceivingNo", SqlDbType.Int)
                End With

                .SqlFrom = "FROM dbo.TransReceiving "
                .SqlOrder = "Order by LastUpdated desc"

                If Not IsNew Then
                    .SqlWhere = "Where id = " & ReadyString(ID)
                End If

                pDT = .Requery

            End With

            If CInt2(Common.WebRequest.Data("IsFromTransferOut")) Then
                pSql = "Update TransReceiving set TransferOutToReceivingID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Return pDT.Rows(0).Item("ReceivingNo").ToString & "|" & pDT.Rows(0).Item("ID").ToString
            'Return pDT.Rows(0).Item("ID").ToString

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return ex.Message.ToString
        End Try
    End Function

    Function AddToInventoryLedger(ByVal TransactionID As String, ByVal StockID As String, ByVal Total As Decimal)
        Dim EDate As Date?
        ' Total As Decimal: from Integer to Decimal [4/16/2014 9:06 AM]

        'If Not ExpDate = "-" Then
        '    EDate = CDate(ExpDate)
        'End If

        WebRequest.Instance.Business.TransInventoryLedger.Add( _
                        CStr2(StockID), _
                        CStr2(WebRequest.Instance.Data("ReceivingPlace")), _
                        TransactionID, _
                        IIf(WebRequest.Instance.Data("IsReceiving"), TransInventoryLedger.eTransType.Receiving, TransInventoryLedger.eTransType.TransferOut), _
                        WebSession.Instance.PeopleUserID, _
                        IIf(WebRequest.Instance.Data("IsReceiving"), Total, -Total)
                    )
    End Function

    Function GetReportHeaderInfo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetbr.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CorporateName", SqlDbType.VarChar)
                    .Expression = "rsetorg.CompanyName"
                End With
                With .Add("OrganizationBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetplace.FowstID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "rsettrdname.BusinessTradeName"
                End With
                With .Add("CompleteAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.StreetNumber)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.Street)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                End With
                With .Add("TelNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationFOWSTTelNos(rsetplace.FowstID)"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryReceiptSettingsFowst rsetplace" & vbCrLf & _
                " INNER JOIN dbo.LibraryReceiptSettings rset ON (rsetplace.ReceiptSettingsID = rset.ID)" & vbCrLf & _
                " INNER JOIN dbo.MobizLibraryOrganizationFOWST rsetbr ON (rsetplace.FowstID = rsetbr.ID)" & vbCrLf & _
                " INNER JOIN dbo.MobizLibraryOrganization rsetorg ON (rsetbr.OrganizationID = rsetorg.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCities cty ON (rsetbr.FowstCityID = cty.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCountries ctry ON (rsetbr.FowstCountryID = ctry.ID)"

            .SqlWhere = " WHERE rsetplace.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID"))

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetReceivingAndTransfersReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
              
                With .Add("TransactionFROM", SqlDbType.VarChar)
                    .Expression = "mFOWST.Name"
                End With
                
                With .Add("TransactionTo", SqlDbType.VarChar)
                    .Expression = "mCompany.tName"
                End With
                With .Add("TransactionType", SqlDbType.VarChar)
                    .Expression = "TransactionType"
                End With

                With .Add("TransCount", SqlDbType.VarChar)
                    .Expression = "COUNT(DISTINCT TransReceiving.ID)"
                End With
                With .Add("SKUCount", SqlDbType.UniqueIdentifier)
                    .Expression = "COUNT(ProductID)"
                End With
                
                With .Add("COGS", SqlDbType.VarChar)
                    .Expression = "SUM(TransInventoryLedger.Value)"
                End With
            End With

            .SqlFrom = " FROM TransInventoryLedger " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMProduct on MobizLibraryPIMProduct.ID = TransInventoryLedger.ProductID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST mFOWST on mFOWST.ID = TransInventoryLedger.FowstID " & vbCrLf &
                        "INNER JOIN TransReceiving on TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "OUTER APPLY( " & vbCrLf &
                        "SELECT MobizLibraryOrganizationFOWST.ID tID, MobizLibraryOrganizationFOWST.Name tName  " & vbCrLf &
                        "FROM MobizLibraryOrganizationFOWST  " & vbCrLf &
                        "WHERE MobizLibraryOrganizationFOWST.ID = TransReceiving.SupplierID " & vbCrLf &
                        "            UNION " & vbCrLf &
                        "SELECT MobizLibraryOrganization.ID tID, MobizLibraryOrganization.CompanyName tName  " & vbCrLf &
                        "FROM MobizLibraryOrganization " & vbCrLf &
                        "WHERE MobizLibraryOrganization.ID = TransReceiving.SupplierID " & vbCrLf &
                        ")mCompany"

            .SqlWhere = "WHERE CONVERT(date, TransReceiving.DateReceived) BETWEEN " & vbCrLf &
                        ReadyString(CDate(Common.WebRequest.Data("DateFrom"))) & vbCrLf &
                        " AND " & ReadyString(CDate(Common.WebRequest.Data("DateTo")))

            If (Common.WebRequest.Data("TransactionType") = "Stock Receiving") Then

                .SqlWhere += " AND TransReceiving.IsReceiving = 1"

            ElseIf (Common.WebRequest.Data("TransactionType") = "Stock Transfers") Then

                .SqlWhere += " AND TransReceiving.IsReceiving = 0"

            End If

            Dim FOWSTFrom As String
            Dim FOWSTOrOrganizationTo As String

            FOWSTFROM = CStr2(Common.WebRequest.Data("Fowst1ID"))
            FOWSTOrOrganizationTo = CStr2(Common.WebRequest.Data("FowstVendorID"))


            If Not FOWSTOrOrganizationTo = vbNullString Then

                .SqlWhere += " AND ((FowstID = " & ReadyString(FOWSTFrom) & vbCrLf &
                             " AND mCompany.tID = " & ReadyString(FOWSTOrOrganizationTo) & vbCrLf &
                             " AND TransactionType = 'Receiving' " & ")" & vbCrLf &
                             " OR " & vbCrLf &
                             " (FowstID = " & ReadyString(FOWSTOrOrganizationTo) & vbCrLf &
                             " AND mCompany.tID = " & ReadyString(FOWSTFrom) & vbCrLf &
                             " AND TransactionType = 'TransferOut'))" & vbCrLf
            Else

                .SqlWhere += " AND FowstID = " & ReadyString(FOWSTFrom) & vbCrLf

            End If
           
            .SqlGroup = "GROUP BY mFOWST.Name, mCompany.tName, TransactionType"

            .SqlOrder = "ORDER BY mFOWST.Name"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetReceivingAndTransfersSubReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "tr1.ID"
                End With
                With .Add("TransactionFROM", SqlDbType.VarChar)
                    .Expression = "mFOWST1.Name"
                End With

                With .Add("TransactionTo", SqlDbType.VarChar)
                    .Expression = "mfowst2.Name"
                End With

                With .Add("TransDate", SqlDbType.VarChar)
                    .Expression = "tr1.DateReceived"
                End With

                With .Add("ReceivingNo", SqlDbType.VarChar)
                    .Expression = "tr1.ReceivingNo"
                End With

                With .Add("TransType", SqlDbType.VarChar)
                    .Expression = "TransInventoryLedger.TransactionType"
                End With

                With .Add("ProductCount1", SqlDbType.Float)
                    .Expression = "COUNT(trs1.productid)"
                End With

                With .Add("COGS1", SqlDbType.Int)
                    .Expression = "SUM(TransInventoryLedger.Value)"
                End With

                With .Add("ProductCount2", SqlDbType.Float)
                    .Expression = "t.t2"
                End With

                With .Add("COGS2", SqlDbType.Int)
                    .Expression = "t.COGS"
                End With

            End With

            .SqlFrom = " FROM TransReceiving tr1 " & vbCrLf &
                        "INNER JOIN TransReceivingStock trs1 on trs1.ReceivingID = tr1.ID " & vbCrLf &
                        "INNER JOIN TransInventoryLedger on TransInventoryLedger.TransactionID = trS1.ReceivingID " & vbCrLf &
                        "AND trS1.ProductID = TransInventoryLedger.ProductID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST mFOWST1 on mFOWST1.ID = tr1.ReceivingPlaceID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST mFOWST2 on mFOWST2.ID = tr1.SupplierID " & vbCrLf &
                        "OUTER APPLY(  " & vbCrLf &
                        "SELECT COUNT(trs2.productID) t2, SUM(TransInventoryLedger.Value) COGS from TransReceiving tr2 " & vbCrLf &
                        "INNER JOIN TransReceivingStock trs2 on trs2.ReceivingID = tr2.ID " & vbCrLf &
                        "INNER JOIN TransInventoryLedger on TransInventoryLedger.TransactionID = trs2.ReceivingID " & vbCrLf &
                        "AND TRS2.ProductID = TransInventoryLedger.ProductID " & vbCrLf &
                        "where tr2.ID=tr1.TransferOutToReceivingID " & vbCrLf &
                        ") t"

            .SqlWhere = "WHERE CONVERT(date, tr1.DateReceived) BETWEEN " & vbCrLf &
                        ReadyString(CDate(Common.WebRequest.Data("DateFrom"))) & vbCrLf &
                        " AND " & ReadyString(CDate(Common.WebRequest.Data("DateTo")))

            If (Common.WebRequest.Data("TransactionType") = "Stock Receiving") Then

                .SqlWhere += " AND tr1.IsReceiving = 1"

            ElseIf (Common.WebRequest.Data("TransactionType") = "Stock Transfers") Then

                .SqlWhere += " AND tr1.IsReceiving = 0"

            End If

            Dim FOWSTFrom As String
            Dim FOWSTOrOrganizationTo As String

            FOWSTFrom = CStr2(Common.WebRequest.Data("Fowst1ID"))
            FOWSTOrOrganizationTo = CStr2(Common.WebRequest.Data("FowstVendorID"))

            If Not FOWSTOrOrganizationTo = vbNullString Then

                .SqlWhere += " AND (tr1.ReceivingPlaceID = " & ReadyString(FOWSTFrom) & vbCrLf &
                             " AND tr1.SupplierID = " & ReadyString(FOWSTOrOrganizationTo) & ")" & vbCrLf

                '.SqlWhere += " AND (FowstID = " & ReadyString(FOWSTFrom) & vbCrLf &
                '             " AND mCompany.tID = " & ReadyString(FOWSTOrOrganizationTo) & ")" & vbCrLf

            End If

            .SqlGroup = "GROUP BY  tr1.ID, t.t2, t.COGS, mFOWST1.Name, mFOWST2.Name, tr1.DateReceived, tr1.ReceivingNo, TransInventoryLedger.TransactionType"
            '"GROUP BY mFOWST.Name, mCompany.tName, TransReceiving.ReceivingNo, TransReceiving.DateReceived, TransactionType, TransferOutToReceivingID"

            .SqlOrder = "ORDER BY tr1.DateReceived"

            pDT = .Requery

        End With

        'With pFinder
        '    .ProData = Common.WebRequest.ProData

        '    With .Fields
        '        With .Add("TransactionFROM", SqlDbType.VarChar)
        '            .Expression = "mFOWST.Name"
        '        End With

        '        With .Add("TransactionTo", SqlDbType.VarChar)
        '            .Expression = "mCompany.tName"
        '        End With

        '        With .Add("TransDate", SqlDbType.UniqueIdentifier)
        '            .Expression = "TransReceiving.DateReceived"
        '        End With

        '        With .Add("ReceivingNo", SqlDbType.UniqueIdentifier)
        '            .Expression = "TransReceiving.ReceivingNo"
        '        End With

        '        With .Add("TransactionType", SqlDbType.VarChar)
        '            .Expression = "TransactionType"
        '        End With

        '        With .Add("SKUCount", SqlDbType.UniqueIdentifier)
        '            .Expression = "COUNT(ProductID)"
        '        End With

        '        With .Add("COGS", SqlDbType.VarChar)
        '            .Expression = "SUM(TransInventoryLedger.Value)"
        '        End With

        '        If (Common.WebRequest.Data("TransactionType") = "Stock Receiving") Then

        '            With .Add("Status", SqlDbType.VarChar)
        '                .Expression = "CASE TransferOutToReceivingID " & vbCrLf &
        '                "WHEN NULL THEN 'No Stock Transfer Document'" & vbCrLf &
        '                "ELSE 'Received With Stock Transfer Document'" & vbCrLf &
        '                "END"

        '                '.Expression = "ISNULL(TransferOutToReceivingID,'No Stock Transfer Document')"
        '            End With

        '        ElseIf (Common.WebRequest.Data("TransactionType") = "Stock Transfers") Then

        '            With .Add("Status", SqlDbType.VarChar)
        '                .Expression = "CASE TransferOutToReceivingID " & vbCrLf &
        '                "WHEN NULL THEN 'Was Not Received'" & vbCrLf &
        '                "ELSE 'Received'" & vbCrLf &
        '                "END"

        '                '.Expression = "ISNULL(TransferOutToReceivingID,'No Stock Transfer Document')"
        '            End With

        '            'With .Add("Status", SqlDbType.VarChar)
        '            '    .Expression = "ISNULL(convert(varchar, TransferOutToReceivingID),'Was Not Received')"
        '            'End With

        '        End If

        '    End With

        '    .SqlFrom = " FROM TransInventoryLedger " & vbCrLf &
        '                "INNER JOIN MobizLibraryPIMProduct on MobizLibraryPIMProduct.ID = TransInventoryLedger.ProductID " & vbCrLf &
        '                "INNER JOIN MobizLibraryOrganizationFOWST mFOWST on mFOWST.ID = TransInventoryLedger.FowstID " & vbCrLf &
        '                "INNER JOIN TransReceiving on TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
        '                "OUTER APPLY( " & vbCrLf &
        '                "SELECT MobizLibraryOrganizationFOWST.ID tID, MobizLibraryOrganizationFOWST.Name tName  " & vbCrLf &
        '                "FROM MobizLibraryOrganizationFOWST  " & vbCrLf &
        '                "WHERE MobizLibraryOrganizationFOWST.ID = TransReceiving.SupplierID " & vbCrLf &
        '                "            UNION " & vbCrLf &
        '                "SELECT MobizLibraryOrganization.ID tID, MobizLibraryOrganization.CompanyName tName  " & vbCrLf &
        '                "FROM MobizLibraryOrganization " & vbCrLf &
        '                "WHERE MobizLibraryOrganization.ID = TransReceiving.SupplierID " & vbCrLf &
        '                ")mCompany"

        '    .SqlWhere = "WHERE CONVERT(date, TransReceiving.DateReceived) BETWEEN " & vbCrLf &
        '                ReadyString(CDate(Common.WebRequest.Data("DateFrom"))) & vbCrLf &
        '                " AND " & ReadyString(CDate(Common.WebRequest.Data("DateTo")))

        '    If (Common.WebRequest.Data("TransactionType") = "Stock Receiving") Then

        '        .SqlWhere += " AND TransReceiving.IsReceiving = 1"

        '    ElseIf (Common.WebRequest.Data("TransactionType") = "Stock Transfers") Then

        '        .SqlWhere += " AND TransReceiving.IsReceiving = 0"

        '    End If

        '    Dim FOWSTFrom As String
        '    Dim FOWSTOrOrganizationTo As String

        '    FOWSTFrom = CStr2(Common.WebRequest.Data("Fowst1ID"))
        '    FOWSTOrOrganizationTo = CStr2(Common.WebRequest.Data("FowstVendorID"))

        '    If Not FOWSTOrOrganizationTo = vbNullString Then

        '        .SqlWhere += " AND (FowstID = " & ReadyString(FOWSTFrom) & vbCrLf &
        '                     " AND mCompany.tID = " & ReadyString(FOWSTOrOrganizationTo) & ")" & vbCrLf

        '    End If

        '    .SqlGroup = "GROUP BY mFOWST.Name, mCompany.tName, TransReceiving.ReceivingNo, TransReceiving.DateReceived, TransactionType, TransferOutToReceivingID"

        '    .SqlOrder = "ORDER BY TransReceiving.DateReceived"

        '    pDT = .Requery
        'End With

        Return pDT
    End Function

    Function GetReceivingAndTransfersDiscrepancyReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSql As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields

                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "TransReceiving.ID"
                End With

                With .Add("TransactionFROM", SqlDbType.VarChar)
                    .Expression = "mFOWST.Name"
                End With

                With .Add("TransactionTo", SqlDbType.VarChar)
                    .Expression = "mCompany.tName"
                End With

                With .Add("ReceivingNo", SqlDbType.VarChar)
                    .Expression = "TransReceiving.ReceivingNo"
                End With

                With .Add("TransactionType", SqlDbType.VarChar)
                    .Expression = "IsReceiving"
                End With

                With .Add("DateReceived", SqlDbType.Date)
                    .Expression = "DateReceived"
                End With

                With .Add("IssueRemarks", SqlDbType.VarChar)
                    .Expression = "IssueRemarks"
                End With

                With .Add("NetSuiteNum", SqlDbType.VarChar)
                    .Expression = "TransReceiving.NetSuiteNum"
                End With

                With .Add("NetSuiteErrorReponse", SqlDbType.VarChar)
                    .Expression = "ISNULL(TransReceiving.NetSuiteError, '') + ' ' + ISNULL(TransReceiving.NetSuiteReponse, '')"
                End With

                With .Add("ProcessedForNetSuite", SqlDbType.Date)
                    .Expression = "TransReceiving.ProcessedForNetSuite"
                End With
            End With

            .SqlFrom = " FROM TransReceiving " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST mFOWST on mFOWST.ID = TransReceiving.ReceivingPlaceID  " & vbCrLf &
                        "OUTER APPLY(  " & vbCrLf &
                        "SELECT MobizLibraryOrganizationFOWST.ID tID, MobizLibraryOrganizationFOWST.Name tName   " & vbCrLf &
                        "FROM MobizLibraryOrganizationFOWST   " & vbCrLf &
                        "WHERE MobizLibraryOrganizationFOWST.ID = TransReceiving.SupplierID  " & vbCrLf &
                        "UNION " & vbCrLf &
                        "SELECT MobizLibraryOrganization.ID tID, MobizLibraryOrganization.CompanyName tName   " & vbCrLf &
                        "FROM MobizLibraryOrganization  " & vbCrLf &
                        "WHERE MobizLibraryOrganization.ID = TransReceiving.SupplierID  " & vbCrLf &
                        ")mCompany "

            .SqlWhere = "WHERE CONVERT(date, TransReceiving.DateReceived) BETWEEN " & vbCrLf &
                        ReadyString(CDate(Common.WebRequest.Data("DateFrom"))) & vbCrLf &
                        " AND " & ReadyString(CDate(Common.WebRequest.Data("DateTo")))

            Dim FOWSTFrom As String
            Dim FOWSTOrOrganizationTo As String

            FOWSTFrom = CStr2(Common.WebRequest.Data("Fowst1ID"))
            FOWSTOrOrganizationTo = CStr2(Common.WebRequest.Data("FowstVendorID"))


            If Not FOWSTOrOrganizationTo = vbNullString Then

                .SqlWhere += " AND ((ReceivingPlaceID = " & ReadyString(FOWSTFrom) & vbCrLf &
                             " AND mCompany.tID = " & ReadyString(FOWSTOrOrganizationTo) & ")" & vbCrLf &
                             " OR (ReceivingPlaceID = " & ReadyString(FOWSTOrOrganizationTo) & vbCrLf &
                             " AND mCompany.tID = " & ReadyString(FOWSTFrom) & "))" & vbCrLf
            Else
                If (Common.WebRequest.Data("TransactionType") = "Stock Receiving") Then

                    .SqlWhere += " AND TransReceiving.IsReceiving = 1"

                ElseIf (Common.WebRequest.Data("TransactionType") = "Stock Transfers") Then

                    .SqlWhere += " AND TransReceiving.IsReceiving = 0"

                End If

                .SqlWhere += " AND ReceivingPlaceID = " & ReadyString(FOWSTFrom) & vbCrLf

            End If

            '.SqlWhere += " AND HasIssues = 1"

            .SqlOrder = "ORDER BY TransactionType desc"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveProductBarcode() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim ProductMeasurement As DataTable

        pSql = "Select ID from MobizLibraryPIMProductMeasurement WHERE ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))

        ProductMeasurement = Common.WebRequest.ProData.Execute(pSql)

        If ProductMeasurement.Rows.Count > 0 Then
            For i As Integer = 0 To ProductMeasurement.Rows.Count - 1

                pSql = "Select * from MobizLibraryPIMProductMeasurementBarcode where StockMeasurementsID = " & ReadyString(Common.WebRequest.Data("ProductID"))

                pDT = Common.WebRequest.ProData.Execute(pSql)
                Try

                    Dim p As New InsertUpdate("MobizLibraryPIMProductMeasurementBarcode")
                    Dim ID As String

                    p("StockMeasurementsID") = ReadyString(ProductMeasurement.Rows(i).Item("ID"))
                    p("Barcode") = ReadyString(Common.WebRequest.Data("Barcode"))

                    If pDT.Rows.Count = 0 Then
                        ID = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(ID)
                        pSql = p.GetInsert()

                    Else
                        pSql = p.GetUpdate("ID = " & ReadyString(pDT.Rows(0).Item("ID")))
                    End If

                    Common.WebRequest.ProData.Execute(pSql)

                Catch ex As Exception
                Return False
            End Try
            Next

            Return True
        End If
    End Function

    Function GetSystemBehaviorSettings() As Object
        Dim pSql As String
        Dim pDT As DataTable
        Dim pOut As JsonDictionary
        pOut = New JsonDictionary

        pSql = "Select DefaultValue from SystemDefaults where Description = 'Automatically Received Stocks'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("IsAutomaticReceived", pDT.Rows(0)("DefaultValue"))

        pSql = "Select DefaultValue from SystemDefaults where Description = 'Show Stock Quantities'"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("IsShowQuantityOfStocks", pDT.Rows(0)("DefaultValue"))
        Return pOut.Serialize()
    End Function

    Function GetStockTransferReport() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "Select DISTINCT " & vbCrLf &
                "MobizLibraryPIMProduct.ProductNo " & vbCrLf &
                ",MobizLibraryPIMProduct.ProductName " & vbCrLf &
                ", ISNULL(TotalTransfer.TotalTransfer,0) GrossStockTransfer " & vbCrLf &
                ", ISNULL(TotalReceive.TotalReceived,0) GrossStockReceive " & vbCrLf &
                ", ISNULL(TotalTransfer.TotalTransfer,0)- ISNULL(TotalReceive.TotalReceived,0) NetStockTransfer " & vbCrLf &
                ", prc.Price " & vbCrLf &
                ", (ISNULL(TotalTransfer.TotalTransfer,0)- ISNULL(TotalReceive.TotalReceived,0)) * prc.Price LineTotal " & vbCrLf &
                " from TransReceiving trMain " & vbCrLf &
                "inner join TransReceivingStock trsMain on trMain.ID = trsMain.ReceivingID " & vbCrLf &
                "inner join MobizLibraryPIMProduct on MobizLibraryPIMProduct.ID = trsMain.ProductID " & vbCrLf &
                "LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = MobizLibraryPIMProduct.id " & vbCrLf &
                "outer apply( " & vbCrLf &
                "Select SUM(QuantityGood) TotalReceived " & vbCrLf &
                "from TransReceivingStock trsReceiving inner join TransReceiving trReceiving on trReceiving.ID = trsReceiving.ReceivingID " & vbCrLf &
                "WHERE trsMain.ProductID = trsReceiving.ProductID AND trReceiving.IsReceiving=1) TotalReceive " & vbCrLf &
                "outer apply( " & vbCrLf &
                "Select SUM(QuantityGood) TotalTransfer " & vbCrLf &
                "from TransReceivingStock trsTransfer inner join TransReceiving trTransfer on trTransfer.ID = trsTransfer.ReceivingID " & vbCrLf &
                "WHERE trsMain.ProductID = trsTransfer.ProductID AND trTransfer.IsReceiving=0) TotalTransfer " & vbCrLf &
                "WHERE  prc.PriceLevelID = '01000012-0005-0000-0000-000000000000'  " & vbCrLf &
                "order by MobizLibraryPIMProduct.ProductName"

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function
End Class
