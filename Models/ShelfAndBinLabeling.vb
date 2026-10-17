Public Class ShelfAndBinLabeling
    Public Property FowstID As String
    Public Property BinID As String
    Public Property AreaID As String
    Public Property OrgID As String
    Public Property ProductID As String
    Public Property FilterDisplay As String
    Public Property UoMID As String
    Public Property PriceLevelID As String
    Public Property IsProductBarcode As Boolean
    Public Property TransNum As String

    Public Property PageStart As String
    Public Property PageNoOfRows As Integer
    Public Property PageSearchText As String
    Public Property IsCountPage As Boolean
    Public Property PageColumnNo As String
    Public Property PageColumnDirection As String

    Public Property uomData As Object

    Function GetAllProductsWithOrWithoutBins() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pim.id"
                End With

                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "ISNULL(pim.ProductNo,'')"
                End With

                With .Add("ProductName", SqlDbType.UniqueIdentifier)
                    .Expression = "pim.ProductName"
                End With

                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "prdmeasure.Quantity"
                End With

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With

                With .Add("Price", SqlDbType.Decimal)
                    .Expression = "CONVERT(DECIMAL(10,2),prdpricelvl.Price)"
                End With

                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "prdpricelvl.LastUpdated"
                    '.Expression = "Replace(CONVERT(VARCHAR(10),prdpricelvl.LastUpdated,101),'-','/')"
                End With

                With .Add("Barcode", SqlDbType.VarChar)
                    .Expression = "ISNULL(prdmeasurebarcode.Barcode,'')"
                End With

            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct pim " & vbCrLf &
                        "INNER JOIN LibraryOrganizationPIM lop ON lop.ProductID = pim.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMProductMeasurement prdmeasure ON prdmeasure.ProductID = pim.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMMeasure pimmeasure ON pimmeasure.ID = prdmeasure.UnitMeasureID " & vbCrLf &
                        "INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON pimmeasure.MeasureNameID = lbl.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMproductPriceLevel prdpricelvl ON prdpricelvl.ProductID = prdmeasure.ProductID " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMPriceLevel pricelvl ON pricelvl.ID = prdpricelvl.PriceLevelID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProductMeasurementBarcode prdmeasurebarcode on prdmeasure.id = prdmeasurebarcode.StockMeasurementsID "
            
            .SqlWhere = "WHERE prdmeasure.Quantity = '1'" &
                    " AND pricelvl.ID = " & ReadyString("01000012-0005-0000-0000-000000000000") &
                    " AND prdpricelvl.OrganizationID = " & ReadyString(OrgID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pim.id"
                End With

                'If Not IsProductBarcode Then

                With .Add("FowstBinProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbin.ID"
                End With

                With .Add("BinID", SqlDbType.UniqueIdentifier)
                    .Expression = "fb.ID"
                End With

                With .Add("Bin", SqlDbType.VarChar)
                    .Expression = "fb.Name"
                End With

                'End If

                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "ISNULL(pim.ProductNo,'')"
                End With

                With .Add("ProductName", SqlDbType.UniqueIdentifier)
                    .Expression = "pim.ProductName"
                End With

                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "prdmeasure.Quantity"
                End With

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With

                'With .Add("Price", SqlDbType.Decimal)
                '    .Expression = "CONVERT(DECIMAL(10,2),prdpricelvl.Price)"
                'End With

                'With .Add("LastUpdated", SqlDbType.DateTime)
                '    .Expression = "prdpricelvl.LastUpdated"
                '    '.Expression = "Replace(CONVERT(VARCHAR(10),prdpricelvl.LastUpdated,101),'-','/')"
                'End With

                If Not IsProductBarcode Then
                    With .Add("MaxPerProduct", SqlDbType.Int)
                        .Expression = "fowstbin.MaxPerProduct"
                    End With

                    With .Add("MinPerProduct", SqlDbType.Int)
                        .Expression = "fowstbin.MinPerProduct"
                    End With
                Else
                    With .Add("Barcode", SqlDbType.VarChar)
                        .Expression = "ISNULL(prdmeasurebarcode.Barcode,'')"
                    End With
                End If
            End With

            .SqlFrom = " FROM LibraryFowstBinProduct fowstbin " & vbCrLf &
                        "INNER JOIN LibraryFowstBin fb on fb.ID = fowstbin.BinID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProduct pim ON fowstbin.ProductID = pim.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMProductMeasurement prdmeasure ON prdmeasure.ProductID = pim.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMMeasure pimmeasure ON pimmeasure.ID = prdmeasure.UnitMeasureID " & vbCrLf &
                        "LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON pimmeasure.MeasureNameID = lbl.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMproductPriceLevel prdpricelvl ON prdpricelvl.ProductID = prdmeasure.ProductID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMPriceLevel pricelvl ON pricelvl.ID = prdpricelvl.PriceLevelID " & vbCrLf &
                        "LEFT JOIN LibraryFowstArea on LibraryFowstArea.FowstID = fowstbin.FowstID" & vbCrLf

            If IsProductBarcode Then
                .SqlFrom &= " LEFT JOIN MobizLibraryPIMProductMeasurementBarcode prdmeasurebarcode on prdmeasure.id = prdmeasurebarcode.StockMeasurementsID "
            End If

            .SqlWhere = "WHERE fowstbin.FowstID = " & ReadyString(FowstID) &
                        " AND fb.Name <> 'Unnamed Bin'"

            If Not PriceLevelID = vbNullString Then
                .SqlWhere += " AND prdmeasure.Quantity = '1'" &
                    " AND pricelvl.ID = " & ReadyString(PriceLevelID) &
                    " AND prdpricelvl.OrganizationID = " & ReadyString(OrgID)
            ElseIf IsProductBarcode Then
                .SqlWhere += " AND prdmeasure.Quantity = '1'" &
                    " AND pricelvl.ID = " & ReadyString("01000012-0005-0000-0000-000000000000") &
                    " AND prdpricelvl.OrganizationID = " & ReadyString(OrgID)
            End If

            If Not AreaID = vbNullString Then
                If Not AreaID = "All" Then
                    .SqlWhere += " AND fb.AreaID = " & ReadyString(AreaID)
                End If
            End If

            If Not BinID = vbNullString Then
                If Not BinID = "All" Then
                    .SqlWhere += " AND fowstbin.BinID = " & ReadyString(BinID)
                End If
            End If

            If Not FilterDisplay = vbNullString Then
                If Not FilterDisplay = "All" Then
                    .SqlFrom &= " INNER JOIN LibraryPriceTagsLastPrintedDate on LibraryPriceTagsLastPrintedDate.FowstID = fowstbin.FowstID " & vbCrLf &
                          " AND LibraryPriceTagsLastPrintedDate.BinID = fowstbin.BinID " & vbCrLf &
                          " AND LibraryPriceTagsLastPrintedDate.PriceLevelID = pricelvl.ID " & vbCrLf &
                          " AND LibraryPriceTagsLastPrintedDate.ProductID = fowstbin.ProductID"

                    .SqlWhere &= " AND datediff(SECOND, LibraryPriceTagsLastPrintedDate.LastPriceAdjustmentDate,prdpricelvl.LastUpdated) > 5"
                End If
            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetAllProductsWithPriceUpdates() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryPIMPriceTagsLastPrintedDate.FowstID"
                End With

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryPIMPriceTagsLastPrintedDate.ProductID"
                End With

                .Add("BinID", SqlDbType.UniqueIdentifier)

                With .Add("ProductNo", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(ProductNo,'')"
                End With
                '.Add("Productno", SqlDbType.DateTime)
                .Add("ProductName", SqlDbType.DateTime)

                With .Add("BinName", SqlDbType.DateTime)
                    .Expression = "LibraryFowstBin.Name"
                End With

                With .Add("Price", SqlDbType.DateTime)
                    .Expression = "CONVERT(DECIMAL(10,2),prdpricelvl.Price) "
                End With

                With .Add("UnitMeasure", SqlDbType.DateTime)
                    .Expression = "lbl.Label"
                End With

                With .Add("Barcode", SqlDbType.VarChar)
                    .Expression = "ISNULL(prdmeasurebarcode.Barcode,'')"
                End With

                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "prdpricelvl.LastUpdated"
                End With

            End With

            .SqlFrom = "FROM LibraryPIMPriceTagsLastPrintedDate " & vbCrLf &
                        "inner join MobizLibraryPimProduct pim on pim.ID = LibraryPIMPriceTagsLastPrintedDate.ProductID " & vbCrLf &
                        "inner join LibraryFowstBin on LibraryFowstBin.ID = LibraryPIMPriceTagsLastPrintedDate.BinID " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMProductMeasurement prdmeasure ON prdmeasure.ProductID = pim.ID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMMeasure pimmeasure ON pimmeasure.ID = prdmeasure.UnitMeasureID " & vbCrLf &
                        "LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON pimmeasure.MeasureNameID = lbl.ID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMproductPriceLevel prdpricelvl ON prdpricelvl.ProductID = prdmeasure.ProductID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMPriceLevel pricelvl ON pricelvl.ID = prdpricelvl.PriceLevelID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPIMProductMeasurementBarcode prdmeasurebarcode on prdmeasure.id = prdmeasurebarcode.StockMeasurementsID "
                
            .SqlOrder = "order by LastUpdated desc "

            .SqlWhere = "WHERE LibraryFowstBin.FowstID = " & ReadyString(FowstID) & vbCrLf &
                        " AND LibraryFowstBin.Name <> 'Unnamed Bin'" & vbCrLf &
                        " AND datediff(SECOND, LibraryPIMPriceTagsLastPrintedDate.LastPriceAdjustmentDate,prdpricelvl.LastUpdated) > 5" & vbCrLf


        If Not PriceLevelID = vbNullString Then
            .SqlWhere += " AND prdmeasure.Quantity = '1'" &
                " AND pricelvl.ID = " & ReadyString(PriceLevelID) &
                " AND prdpricelvl.OrganizationID = " & ReadyString(OrgID)
        ElseIf IsProductBarcode Then
            .SqlWhere += " AND prdmeasure.Quantity = '1'" &
                " AND pricelvl.ID = " & ReadyString("01000012-0005-0000-0000-000000000000") &
                " AND prdpricelvl.OrganizationID = " & ReadyString(OrgID)
        End If

        If Not AreaID = vbNullString Then
            If Not AreaID = "All" Then
                .SqlWhere += " AND fb.AreaID = " & ReadyString(AreaID)
            End If
        End If

        If Not BinID = vbNullString Then
            If Not BinID = "All" Then
                .SqlWhere += " AND fowstbin.BinID = " & ReadyString(BinID)
            End If
        End If

        If Not FilterDisplay = vbNullString Then
            If Not FilterDisplay = "All" Then
                    .SqlFrom &= " INNER JOIN LibraryPIMPriceTagsLastPrintedDate on LibraryPIMPriceTagsLastPrintedDate.FowstID = fowstbin.FowstID " & vbCrLf &
                      " AND LibraryPIMPriceTagsLastPrintedDate.BinID = fowstbin.BinID " & vbCrLf &
                      " AND LibraryPIMPriceTagsLastPrintedDate.PriceLevelID = pricelvl.ID "

                    .SqlWhere &= " AND datediff(SECOND, LibraryPIMPriceTagsLastPrintedDate.LastPriceAdjustmentDate,prdpricelvl.LastUpdated) > 5"
            End If
        End If

        pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetFowstArea() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
                .Add("AreaName", SqlDbType.UniqueIdentifier)
            End With

        .SqlFrom = " FROM LibraryFowstArea "

        .SqlWhere = "WHERE FowstID = " & ReadyString(FowstID)

        pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetShelfPriceTagsData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbin.ID"
                End With

                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowstbin.ProductID"
                End With

                With .Add("ProductName", SqlDbType.UniqueIdentifier)
                    .Expression = "pim.ProductName"
                End With
            End With

            .SqlFrom = " FROM LibraryFowstBinProduct fowstbin INNER JOIN MobizLibraryPIMProduct pim ON fowstbin.ProductID = pim.ID "

            .SqlWhere = "WHERE fowstbin.BinID = " & ReadyString(BinID) &
                        " AND fowstbin.FowstID = " & ReadyString(FowstID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetBinLocationData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "bin.ID"
                End With

                With .Add("Name", SqlDbType.UniqueIdentifier)
                    .Expression = "bin.Name"
                End With

                With .Add("Description", SqlDbType.UniqueIdentifier)
                    .Expression = "bin.Description"
                End With
            End With

            .SqlFrom = " FROM LibraryFowstBin bin "

            .SqlWhere = " WHERE bin.FowstID = " & ReadyString(FowstID) &
                        " AND bin.Name <> 'Unnamed Bin' ORDER BY Name Asc"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProductBarcodeData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "sku.ID"
                End With

                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "sku.SkuDesc"
                End With

                With .Add("ProductNo", SqlDbType.UniqueIdentifier)
                    .Expression = "prod.ProductNo"
                End With

                With .Add("SkuDesc", SqlDbType.VarChar)
                    .Expression = "prod.ProductName"
                End With

                With .Add("UoM", SqlDbType.VarChar)
                    .Expression = "uom3.Label"
                End With

                With .Add("UoMID", SqlDbType.VarChar)
                    .Expression = "sku.UoM"
                End With

                With .Add("BarcodeID", SqlDbType.VarChar)
                    .Expression = "sku.Barcode"
                End With

                With .Add("Barcode", SqlDbType.VarChar)
                    .Expression = "barcode.barcode"
                End With

                With .Add("NumLabels", SqlDbType.Int)
                    .Expression = "sku.NumLabels"
                End With

                With .Add("TradeName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With

                With .Add("PriceID", SqlDbType.Int)
                    .Expression = "sku.Price"
                End With

                With .Add("Price", SqlDbType.Int)
                    .Expression = "price.Price"
                End With

                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "pricelevel.PriceLevel"
                End With

                With .Add("CreatedBy", SqlDbType.VarChar)
                    .Expression = "sku.CreatedBy"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "sku.LastUpdated"
                End With
            End With

            .SqlFrom = " FROM LibraryFowstProductBarcode sku " &
                        "LEFT JOIN MobizLibraryPIMProduct AS prod ON sku.SkuDesc = prod.ID " &
                        "LEFT JOIN MobizLibraryPIMProductPriceLevel AS price ON sku.Price = price.ID " &
                        "LEFT JOIN MobizLibraryPIMPriceLevel AS pricelevel ON price.PriceLevelID = pricelevel.ID " &
                        "LEFT JOIN MobizLibraryPIMProductMeasurement AS uom ON sku.UoM = uom.ID " &
                        "LEFT JOIN MobizLibraryPIMMeasure AS uom2 ON uom.UnitMeasureID = uom2.ID " &
                        "LEFT JOIN MobizLibraryCountriesLanguageLabel AS uom3 ON uom2.MeasurenameID = uom3.ID " &
                        "LEFT JOIN MobizLibraryPIMProductMeasurementBarcode AS barcode ON sku.Barcode = barcode.ID " &
                        "LEFT JOIN MobizLibraryOrganization AS org ON sku.OrgID = org.ID "

            .SqlWhere = " WHERE sku.TransactionNum = " & ReadyString(If(Common.WebRequest.Data("TransNum") = Nothing, TransNum, Common.WebRequest.Data("TransNum"))) &
                        " ORDER BY SkuDesc Asc"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProductMeasurement() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "uom.ID"
                End With

                With .Add("Label", SqlDbType.UniqueIdentifier)
                    .Expression = "uom3.Label"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProductMeasurement uom " &
                        "LEFT JOIN MobizLibraryPIMMeasure AS uom2 ON uom.UnitMeasureID = uom2.ID " &
                        "LEFT JOIN MobizLibraryCountriesLanguageLabel AS uom3 ON uom2.MeasurenameID = uom3.ID "

            .SqlWhere = " WHERE uom.productID = " & ReadyString(ProductID) &
                        " ORDER BY Label Asc"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetMeasurementBarcode() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With

                With .Add("Barcode", SqlDbType.UniqueIdentifier)
                    .Expression = "Barcode"
                End With

                With .Add("StockMeasurementsID", SqlDbType.UniqueIdentifier)
                    .Expression = "StockMeasurementsID"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProductMeasurementBarcode "

            .SqlWhere = " WHERE StockMeasurementsID = " & ReadyString(UoMID) &
                        " ORDER BY Barcode Asc"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetPriceLevel() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)

                .Add("PriceLevel", SqlDbType.UniqueIdentifier)
        End With

        .SqlFrom = " FROM MobizLibraryPIMPriceLevel"

            .SqlWhere = " WHERE (OrgID = " & ReadyString(OrgID) & " AND IsActive=1)" &
            " OR ID = " & ReadyString("01000012-0001-0000-0000-000000000000") &
            " OR ID = " & ReadyString("01000012-0002-0000-0000-000000000000") &
            " OR ID = " & ReadyString("01000012-0003-0000-0000-000000000000") &
            " OR ID = " & ReadyString("01000012-0004-0000-0000-000000000000") &
            " OR ID = " & ReadyString("01000012-0005-0000-0000-000000000000")

        pDT = .Requery

        End With

        'With pFinder
        '    .ProData = Common.WebRequest.ProData

        '    With .Fields
        '        With .Add("ID", SqlDbType.UniqueIdentifier)
        '            .Expression = "price.ID"
        '        End With

        '        With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
        '            .Expression = "price.PriceLevelID"
        '        End With

        '        With .Add("PriceLevel", SqlDbType.VarChar)
        '            .Expression = "pricelevel.PriceLevel"
        '        End With

        '        With .Add("ProductID", SqlDbType.UniqueIdentifier)
        '            .Expression = "price.ProductID"
        '        End With

        '    End With

        '    .SqlFrom = " FROM MobizLibraryPIMProductPriceLevel AS price " &
        '                "LEFT JOIN MobizLibraryPIMPriceLevel AS pricelevel ON price.PriceLevelID = pricelevel.ID "

        '    .SqlWhere = " WHERE ProductID = " & ReadyString(ProductID) &
        '                " ORDER BY PriceLevel Asc"

        '    pDT = .Requery

        'End With

        Return pDT
    End Function

    Function Lookup() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = "WHERE CONVERT(varchar(50), TransactionID) <> '' "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent = "" Then
                    whereSearchContent = "ISNULL(TransactionNum, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Else
                    whereSearchContent += "OR ISNULL(TransactionNum, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                End If
            Next
            whereSearch = "WHERE CONVERT(varchar(50), TransactionID) <> '' AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Then
            whereOrder = " ORDER BY TransactionNum " & PageColumnDirection & vbCrLf
        ElseIf PageColumnNo = 1 Then
            whereOrder = " ORDER BY NumLabel " & PageColumnDirection & vbCrLf
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY LastUpdated " & PageColumnDirection & vbCrLf
        End If

        pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From (SELECT DISTINCT (SELECT SUM(NumLabels) FROM LibraryFowstProductBarcode WHERE TransactionID = sku.TransactionID) AS [NumLabels] " & vbCrLf &
                "   , sku.CreatedBy AS [CreatedBy] " & vbCrLf &
                "   , convert(varchar(16), sku.LastUpdated, 120) AS [LastUpdated] " & vbCrLf &
                "   , sku.TransactionNum AS [TransactionNum] " & vbCrLf &
                "   , sku.TransactionID " & vbCrLf &
                "   FROM LibraryFowstProductBarcode sku " & vbCrLf &
                "       LEFT JOIN MobizLibraryPIMProduct AS prod ON sku.SkuDesc = prod.ID  " & vbCrLf &
                "       LEFT JOIN MobizLibraryPIMProductPriceLevel AS price ON sku.Price = price.ID " & vbCrLf &
                "       LEFT JOIN MobizLibraryPIMPriceLevel AS pricelevel ON price.PriceLevelID = pricelevel.ID " & vbCrLf &
                "       LEFT JOIN MobizLibraryPIMProductMeasurement AS uom ON sku.UoM = uom.ID  " & vbCrLf &
                "       LEFT JOIN MobizLibraryPIMMeasure AS uom2 ON uom.UnitMeasureID = uom2.ID " & vbCrLf &
                "       LEFT JOIN MobizLibraryCountriesLanguageLabel AS uom3 ON uom2.MeasurenameID = uom3.ID " & vbCrLf &
                "       LEFT JOIN MobizLibraryPIMProductMeasurementBarcode AS barcode ON sku.Barcode = barcode.ID " & vbCrLf &
                "       LEFT JOIN MobizLibraryOrganization AS org ON sku.OrgID = org.ID " & vbCrLf &
                "       " & whereSearch & vbCrLf &
                "       GROUP BY sku.TransactionID , sku.CreatedBy, convert(varchar(16), sku.LastUpdated, 108), sku.LastUpdated, sku.TransactionNum  " & vbCrLf &
                ") as V1 ) as v2 " & whereLimit
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProductList() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = "WHERE pimpro.IsActive = 1 AND ISNULL(pimpro.ProductName, '') <> '' "
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent = "" Then
                    whereSearchContent = "ISNULL(ProductSearchString, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Else
                    whereSearchContent += "OR ISNULL(ProductSearchString, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                End If
            Next
            whereSearch = "WHERE pimpro.IsActive = 1 AND ISNULL(pimpro.ProductName, '') <> '' AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 1 Then
            whereOrder = " ORDER BY ProductNo " & PageColumnDirection & vbCrLf
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY ProductName " & PageColumnDirection & vbCrLf
        End If

        'pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From (SELECT DISTINCT pimpro.ID AS [ID] " & vbCrLf &
        '        "   , pimpro.ProductGroupNo AS [ProductGroupNo] " & vbCrLf &
        '        "   , pimpro.ProductNo AS [ProductNo] " & vbCrLf &
        '        "   , pimpro.ProductName AS [ProductName] " & vbCrLf &
        '        "   , ISNULL(pimpro.ProductSearchString,'')  AS [ProductSearchString] " & vbCrLf &
        '        "   , pub.CompanyName AS [PublishedBy] " & vbCrLf &
        '        "   , upl.CompanyName AS [UploadedBy] " & vbCrLf &
        '        "   , pimpro.IsBrandOwner AS [IsBrandOwner] " & vbCrLf &
        '        "   , ISNULL(orgpim.AllowDecimal, 0) AS [AllowDecimal] " & vbCrLf &
        '        "   , pimprolvl.Price AS [RetailPrice] " & vbCrLf &
        '        "   FROM MobizLibraryPIMProduct pimpro " & vbCrLf &
        '        "       INNER JOIN LibraryOrganizationPIM orgpim ON (pimpro.ID = orgpim.ProductID)  " & vbCrLf &
        '        "       LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " & vbCrLf &
        '        "       LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) " & vbCrLf &
        '        "       LEFT JOIN MobizLibraryPIMProductPriceLevel pimprolvl ON (pimprolvl.ProductID = orgpim.ProductID) AND pimprolvl.PriceLevelID = '01000012-0005-0000-0000-000000000000'  " & vbCrLf &
        '        "       " & whereSearch & vbCrLf &
        '        ") as V1 ) as v2 " & whereLimit

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT " & vbCrLf &
                " orgpim.ProductID AS [ID] " & vbCrLf &
                ", pimpro.ProductNo AS [ProductNo] " & vbCrLf &
                ", pimpro.ProductName AS [ProductName] " & vbCrLf &
                ", pimpro.SRP AS [RetailPrice] " & vbCrLf &
                ", pimpro.ProductGroupNo AS [ProductGroupNo]" & vbCrLf &
                " FROM LibraryOrganizationPIM orgpim " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct pimpro ON (orgpim.ProductID = pimpro.ID) " & vbCrLf &
                "" & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetAllMissingBarcodes() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryProductsWithoutBarcode.ID"
                End With

                .Add("ProductName", SqlDbType.UniqueIdentifier)

                .Add("PriceLevel", SqlDbType.UniqueIdentifier)

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With

            End With

            .SqlFrom = " FROM LibraryProductsWithoutBarcode " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMProduct pim on pim.ID = LibraryProductsWithoutBarcode.ProductID  " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMProductMeasurement prdmeasure ON prdmeasure.ProductID = pim.ID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMMeasure pimmeasure ON pimmeasure.ID = prdmeasure.UnitMeasureID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON pimmeasure.MeasureNameID = lbl.ID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMproductPriceLevel prdpricelvl ON prdpricelvl.ProductID = prdmeasure.ProductID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMPriceLevel pricelvl ON pricelvl.ID = prdpricelvl.PriceLevelID  " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProductMeasurementBarcode prdmeasurebarcode on prdmeasure.id = prdmeasurebarcode.StockMeasurementsID    " & vbCrLf

            .SqlWhere = " WHERE (prdpricelvl.OrganizationID = " & ReadyString(OrgID) & " AND pim.IsActive=1) " & vbCrLf &
                        " AND prdpricelvl.PriceLevelID <> '01000012-0001-0000-0000-000000000000' " & vbCrLf &
                        " AND prdpricelvl.PriceLevelID <> '01000012-0002-0000-0000-000000000000' " & vbCrLf &
                        " AND prdpricelvl.PriceLevelID <> '01000012-0003-0000-0000-000000000000' " & vbCrLf
            
            pDT = .Requery

        End With

        Return pDT

    End Function
End Class
