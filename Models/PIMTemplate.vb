Public Class PIMTemplate
    Public Property MaxReturnRow As Integer = 50
    Public Property ProductIDList As New ArrayList
    Public Property ProductList As ArrayList

    Function UploadToDBase(ByVal dtaTable As DataTable, ByVal OrgID As String) As ArrayList
        Dim pFinder As Finder
        Dim blnRet As New ArrayList
        Dim intCntr As Integer
        Dim pSql As String
        Dim pProductID As String
        Dim pMeasurementID As String
        Dim pPriceID As String
        Dim pID As String
        Dim pDT As DataTable
        Dim pFeature As DataTable
        Dim p As New InsertUpdate("MobizLibraryPIMProduct")

        If (dtaTable.Rows.Count > 0) Then

            Try

                Common.WebRequest.ProData.BeginTransaction()

                intCntr = 0
                While (intCntr < dtaTable.Rows.Count)

                    Dim dtaRow As DataRow
                    dtaRow = dtaTable.Rows(intCntr)

                    '''Insert into PIMProduct
                    If (dtaRow("Short Product Name *").ToString().Trim().Length > 0) Then
                        If (dtaRow("Internal Product Code *").ToString().Trim().Length > 0) Then
                            p = New InsertUpdate("MobizLibraryPIMProduct")

                            p("PublishedByID") = ReadyString(OrgID)
                            p("UploadedByID") = ReadyString(OrgID)
                            p("IsBrandOwner") = "0"

                            p("ProductName") = ReadyString(dtaRow("Short Product Name *").ToString())
                            p("DivisionDescription") = ReadyString(dtaRow("Your Custom Classification").ToString()) 'NIKE's term is Division Description. so this is actually custom
                            p("Brand") = ReadyString(dtaRow("Brand").ToString())
                            p("ModelNo") = ReadyString(dtaRow("Model Number").ToString())
                            'p("InternalProductCode") = ReadyString(dtaRow("Internal Product Code").ToString())
                            p("ProductGroupNo") = ReadyString(dtaRow("Product Grouping Number").ToString())
                            If (IsNumeric(dtaRow("Suggested Retail Price (SRP)").ToString().Trim())) Then
                                p("SRP") = Convert.ToDouble(dtaRow("Suggested Retail Price (SRP)").ToString().Trim())
                            Else
                                p("SRP") = 0
                            End If

                            p("MarketingText") = ReadyString(dtaRow("Marketing Text").ToString())
                            p("Warranty") = ReadyString(dtaRow("Warranty Policy").ToString())
                            p("ProductNo") = ReadyString(dtaRow("Internal Product Code *").ToString())

                            p("ProductFamilyID") = "NULL"

                            p("UNSPSCSegmentID") = "NULL"
                            p("UNSPSCFamilyID") = "NULL"

                            ''' Get UNSPSC Class ID
                            If (dtaRow("UNSPSC Classification by Class").ToString().Trim().Length > 0) Then
                                '''Find UNSPSCClassID
                                pFinder = New Finder
                                With pFinder
                                    .ProData = Common.WebRequest.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "ID"
                                            .Searchable = False
                                        End With
                                        With .Add("Class", SqlDbType.VarChar)
                                            .Expression = "Class"
                                        End With
                                    End With

                                    .SqlFrom = "FROM MobizLibraryUNSPSCClass "

                                    .SqlWhere = " WHERE Class = " & ReadyString(dtaRow("UNSPSC Classification by Class").ToString().Trim())

                                    pDT = New DataTable()
                                    pDT = .Requery

                                End With

                                If (pDT.Rows.Count > 0) Then '''If it exists in the record, get id
                                    p("UNSPSCClassID") = ReadyString(pDT.Rows(0).Item("ID").ToString())
                                End If


                            End If


                            ''' Get UNSPSC Commodity ID
                            If (dtaRow("UNSPSC Commodity Type").ToString().Trim().Length > 0) Then
                                '''Find UNSPSCCommodityID
                                pFinder = New Finder
                                With pFinder
                                    .ProData = Common.WebRequest.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "ID"
                                            .Searchable = False
                                        End With
                                        With .Add("Commodity", SqlDbType.VarChar)
                                            .Expression = "Commodity"
                                        End With
                                    End With

                                    .SqlFrom = "FROM MobizLibraryUNSPSCCommodity "

                                    .SqlWhere = " WHERE Commodity = " & ReadyString(dtaRow("UNSPSC Commodity Type").ToString().Trim())

                                    pDT = New DataTable()
                                    pDT = .Requery

                                End With

                                If (pDT.Rows.Count > 0) Then '''If it exists in the record, get id
                                    p("UNSPSCCommodityID") = ReadyString(pDT.Rows(0).Item("ID").ToString())
                                End If


                            End If

                            p("DateAdded") = "getdate()"
                            p("IsActive") = "1"
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            p("LastUpdated") = "getdate()"

                            'pSql = p.GetInsert()
                            'Common.WebRequest.ProData.ExecuteSQL(pSql)


                            pID = CheckProductInfoExist(OrgID, dtaRow("Internal Product Code *").ToString().Trim())

                            If (pID.Trim().Length <= 0) Then
                                pProductID = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(pProductID)

                                pSql = p.GetInsert()
                            Else
                                pProductID = pID
                                pSql = p.GetUpdate(" ID = " & ReadyString(pID))

                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)


                            '''Insert into LibraryPIMProductFeatureStructure - Brand
                            If (dtaRow("Brand").ToString().Trim().Length > 0) Then

                                '''Insert into LibraryPIMProductFeatureStructure - Brand Structure '''Fixed ID for brand feature
                                pFinder = New Finder
                                With pFinder
                                    .ProData = Common.WebRequest.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "feastr.ID"
                                            .Searchable = False
                                        End With
                                        With .Add("StructuredValue", SqlDbType.VarChar)
                                            .Expression = "feastr.StructuredValue"
                                        End With
                                    End With

                                    .SqlFrom = "FROM MobizLibraryPIMFeatureStructure feastr "

                                    .SqlWhere = " WHERE feastr.FeatureID = " & ReadyString("7B3524D2-EFFC-4921-B752-3CF7D84434D6") &
                                               " AND feastr.StructuredValue = " & ReadyString(dtaRow("Brand").ToString())
                                    pDT = New DataTable()
                                    pDT = .Requery

                                End With

                                If (pDT.Rows.Count <= 0) Then '''If not exist in the structured option, insert
                                    p = New InsertUpdate("MobizLibraryPIMFeatureStructure")

                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("FeatureID") = ReadyString("7B3524D2-EFFC-4921-B752-3CF7D84434D6") '''Fixed value for commodity feature with no unspscclassification
                                    p("StructuredValue") = ReadyString(dtaRow("Brand").ToString())

                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                            End If
                            '''End Insert into Brand Structure ----------------
                            ''' 
                            '''Insert into LibraryPIMProductFeatureStructure - Product Type
                            If (dtaRow("Product Type").ToString().Trim().Length > 0) Then

                                '''Insert into LibraryPIMProductFeatureStructure - Product Type Structure '''Fixed ID for product type feature
                                pFinder = New Finder
                                With pFinder
                                    .ProData = Common.WebRequest.ProData
                                    .MaxRecords = MaxReturnRow

                                    With .Fields
                                        With .Add("ID", SqlDbType.UniqueIdentifier)
                                            .Expression = "feastr.ID"
                                            .Searchable = False
                                        End With
                                        With .Add("StructuredValue", SqlDbType.VarChar)
                                            .Expression = "feastr.StructuredValue"
                                        End With
                                    End With

                                    .SqlFrom = "FROM MobizLibraryPIMFeatureStructure feastr "

                                    .SqlWhere = " WHERE feastr.FeatureID = " & ReadyString("32D49B10-F1D9-4D5B-888B-EAC65F35473D") &
                                               " AND feastr.StructuredValue = " & ReadyString(dtaRow("Product Type").ToString())
                                    pDT = New DataTable()
                                    pDT = .Requery

                                End With

                                If (pDT.Rows.Count <= 0) Then '''If not exist in the structured option, insert
                                    p = New InsertUpdate("MobizLibraryPIMFeatureStructure")

                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("FeatureID") = ReadyString("32D49B10-F1D9-4D5B-888B-EAC65F35473D") '''Fixed value for product type feature id 
                                    p("StructuredValue") = ReadyString(dtaRow("Product Type").ToString())

                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                            End If
                            '''End Insert into Product Type Structure ----------------
                        End If

                    End If
                    '''End Insert into PIM Product If Product Name condition (if product name, measurements issue) ----------------

                    '''Insert into MobizLibraryPIMProductMeasurement
                    If (dtaRow("Packing Size Unit *").ToString().Trim().Length > 0) Then
                        p = New InsertUpdate("MobizLibraryPIMProductMeasurement")

                        pMeasurementID = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(pMeasurementID)
                        p("ProductID") = ReadyString(pProductID)
                        p("MeasurementLevel") = 1
                        p("UnitMeasureID") = "(SELECT TOP 1 mea.[ID] FROM MobizLibraryPIMMeasure mea " &
                                            " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (mea.[MeasureNameID] = lbl.ID) " &
                                            " INNER JOIN MobizLibraryPIMMeasureSign sgn ON (mea.[SignID] = sgn.ID) " &
                                            " WHERE ((CASE " &
                                            "       WHEN mea.[IsStandardLength] = 1 THEN 'Length' " &
                                            "       WHEN mea.[IsStandardTime] = 1 THEN 'Time' " &
                                            "       WHEN mea.[IsStandardArea] = 1 THEN 'Area' " &
                                            "       WHEN mea.[IsStandardVolCapacity] = 1 THEN 'Vol (Capacity)' " &
                                            "       WHEN mea.[IsStandardVolDry] = 1 THEN 'Vol (Dry)' " &
                                            "       WHEN mea.[IsStandardWeight] = 1 THEN 'Weight/Mass' " &
                                            "       WHEN mea.[IsStandardCooking] = 1 THEN 'Cooking' " &
                                            "       WHEN mea.[IsStandardElectricity] = 1 THEN 'Electrical' " &
                                            "       ELSE 'Non-Standard' " &
                                            "END) + ' - ' + lbl.Label + ' (' + sgn.Sign + ')') = " & ReadyString(dtaRow("Packing Size Unit *").ToString().Trim()) &
                                            " OR (lbl.Label + ' (' + sgn.Sign + ')') = " & ReadyString(dtaRow("Packing Size Unit *").ToString().Trim()) & ")"

                        If (IsNumeric(dtaRow("Packing Size Qty *").ToString().Trim())) Then
                            p("Quantity") = Convert.ToDouble(dtaRow("Packing Size Qty *").ToString().Trim())
                        Else
                            p("Quantity") = 0
                        End If

                        If (IsNumeric(dtaRow("Content Qty").ToString().Trim())) Then
                            p("ContentQty") = Convert.ToDouble(dtaRow("Content Qty").ToString().Trim())
                        Else
                            p("ContentQty") = 0
                        End If

                        p("ContentUnit") = ReadyString(dtaRow("Content Unit").ToString().Trim())

                        If (IsNumeric(dtaRow("Length").ToString().Trim())) Then
                            p("ProdDimensionLength") = Convert.ToDouble(dtaRow("Length").ToString().Trim())
                        Else
                            p("ProdDimensionLength") = 0
                        End If

                        p("ProdDimensionLengthUnit") = ReadyString(dtaRow("Length Unit").ToString().Trim())

                        If (IsNumeric(dtaRow("Width").ToString().Trim())) Then
                            p("ProdDimensionWidth") = Convert.ToDouble(dtaRow("Width").ToString().Trim())
                        Else
                            p("ProdDimensionWidth") = 0
                        End If

                        p("ProdDimensionWidthUnit") = ReadyString(dtaRow("Width Unit").ToString().Trim())

                        If (IsNumeric(dtaRow("Height").ToString().Trim())) Then
                            p("ProdDimensionHeight") = Convert.ToDouble(dtaRow("Height").ToString().Trim())
                        Else
                            p("ProdDimensionHeight") = 0
                        End If

                        p("ProdDimensionHeightUnit") = ReadyString(dtaRow("Height Unit").ToString().Trim())

                        If (IsNumeric(dtaRow("Weight").ToString().Trim())) Then
                            p("ProductWeight") = Convert.ToDouble(dtaRow("Weight").ToString().Trim())
                        Else
                            p("ProductWeight") = 0
                        End If
                        p("ProductWeightUnit") = ReadyString(dtaRow("Weight Unit").ToString().Trim())

                        p("Discount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.ExecuteSQL(pSql)


                        '''Insert into MobizLibraryPIMProductMeasurementBarcode
                        If (dtaRow("UPC Code (Barcode #)").ToString().Trim().Length > 0) Then

                            p = New InsertUpdate("MobizLibraryPIMProductMeasurementBarcode")

                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("StockMeasurementsID") = ReadyString(pMeasurementID)

                            If (dtaRow("UPC Code (Barcode #)").ToString().Trim().Length > 0) Then
                                p("Barcode") = ReadyString(dtaRow("UPC Code (Barcode #)").ToString().Trim())
                                p("ReadyBarcode") = ReadyString(dtaRow("UPC Code (Barcode #)").ToString().Trim().TrimStart("0"c))
                            End If

                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If
                        ''' End Insert into MobizLibraryPIMProductMeasurementBarcode
                        ''' 

                    End If
                    '''End Insert into Product Dimension Length----------------


                    '''Insert into MobizLibraryPIMProductPriceLevel
                    ''' Cost From Vendor
                    If (dtaRow("Cost From Vendor").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)

                        p("PriceLevelID") = ReadyString("01000012-0002-0000-0000-000000000000") '''Fixed value for COST FROM VENDOR price level


                        If (IsNumeric(dtaRow("Cost From Vendor").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Cost From Vendor").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckCostFromVendorPriceExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    ''' End Insert into Cost From Vendor
                    ''' 
                    ''' Freight Cost
                    If (dtaRow("Freight Cost").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)

                        p("PriceLevelID") = ReadyString("01000012-0001-0000-0000-000000000000") '''Fixed value for FREIGHT COST price level


                        If (IsNumeric(dtaRow("Freight Cost").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Freight Cost").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckFreightCostExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    '''End Insert into Freight Cost

                    ''' Landed Cost
                    If (dtaRow("Landed Cost *").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)

                        p("PriceLevelID") = ReadyString("01000012-0003-0000-0000-000000000000") '''Fixed value for LANDED COST price level


                        If (IsNumeric(dtaRow("Landed Cost *").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Landed Cost *").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckLandedCostExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    '''End Insert into Landed Cost

                    ''' Wholesale Price
                    If (dtaRow("Wholesale Price").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)

                        p("PriceLevelID") = ReadyString("01000012-0004-0000-0000-000000000000") '''Fixed value for WHOLESALE PRICE price level


                        If (IsNumeric(dtaRow("Wholesale Price").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Wholesale Price").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckWholesalePriceExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    '''End Insert into Wholesale Price

                    '''Retail Price
                    If (dtaRow("Retail Price").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)
                        p("PriceLevelID") = ReadyString("01000012-0005-0000-0000-000000000000") '''Fixed value for RETAIL PRICE price level


                        If (IsNumeric(dtaRow("Retail Price").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Retail Price").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckRetailPriceExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    '''End Insert into Retail Price----------------

                    '''Employee Price
                    If (dtaRow("Employee Price").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)
                        p("PriceLevelID") = ReadyString("B458EA5D-A6B6-460A-9BFA-25CCD98E3CB3") '''Fixed value for EMPLOYEE PRICE price level


                        If (IsNumeric(dtaRow("Employee Price").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Employee Price").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckEmployeePriceExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    '''End Insert into Employee Price----------------
                    ''' 
                    '''Corporate Price
                    If (dtaRow("Corporate Price").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)
                        p("PriceLevelID") = ReadyString("6C5450DA-6CB8-4704-A9BA-DBD4CD1C1BE1") '''Fixed value for CORPORATE OFFICE PRICE price level


                        If (IsNumeric(dtaRow("Corporate Price").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Corporate Price").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckCorporatePriceExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    '''End Insert into Corporate Price----------------

                    '''Special Price
                    If (dtaRow("Special Price").ToString().Trim().Length > 0) Then

                        p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                        p("OrganizationID") = ReadyString(OrgID)
                        p("ProductID") = ReadyString(pProductID)
                        p("PriceLevelID") = ReadyString("A1EE4163-0B6F-4A68-ADBD-05957B113777") '''Fixed value for SPECIAL PRICE price level


                        If (IsNumeric(dtaRow("Special Price").ToString().Trim())) Then
                            p("Price") = Convert.ToDouble(dtaRow("Special Price").ToString().Trim())
                        Else
                            p("Price") = 0
                        End If

                        p("DeactivateDiscount") = 0
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "getdate()"

                        pPriceID = CheckRetailPriceExist(OrgID, pProductID)

                        If (pPriceID.Trim().Length <= 0) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'Else
                            '    pSql = p.GetUpdate(" ID = " & ReadyString(pPriceID))
                        End If



                    End If
                    '''End Insert into Special Price----------------
                    ''' 
                    intCntr = intCntr + 1

                    ProductIDList.Add(pProductID)
                End While

                Common.WebRequest.ProData.CommitTransaction()

            Catch ex As Exception

                Common.WebRequest.ProData.RollbackTransaction()
            End Try

        End If

        blnRet = ProductIDList

        Return blnRet

    End Function


    Function CheckRetailPriceExist(ByVal vOrgID As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprice"

            .SqlWhere = " WHERE prdprice.OrganizationID = " & ReadyString(vOrgID) &
                " AND prdprice.ProductID = " & ReadyString(vProductID) &
                " AND prdprice.PriceLevelID = " & ReadyString("01000012-0005-0000-0000-000000000000") '''Fixed Price Level ID for Retail PRice

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function


    Function CheckWholesalePriceExist(ByVal vOrgID As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprice"

            .SqlWhere = " WHERE prdprice.OrganizationID = " & ReadyString(vOrgID) &
                " AND prdprice.ProductID = " & ReadyString(vProductID) &
                " AND prdprice.PriceLevelID = " & ReadyString("01000012-0004-0000-0000-000000000000") '''Fixed Price Level ID for Wholesale PRice

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function

    Function CheckCostFromVendorPriceExist(ByVal vOrgID As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprice"

            .SqlWhere = " WHERE prdprice.OrganizationID = " & ReadyString(vOrgID) &
                " AND prdprice.ProductID = " & ReadyString(vProductID) &
                " AND prdprice.PriceLevelID = " & ReadyString("01000012-0002-0000-0000-000000000000") '''Fixed Price Level ID for Cost From Vendor PRice

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function


    Function CheckFreightCostExist(ByVal vOrgID As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprice"

            .SqlWhere = " WHERE prdprice.OrganizationID = " & ReadyString(vOrgID) &
                " AND prdprice.ProductID = " & ReadyString(vProductID) &
                " AND prdprice.PriceLevelID = " & ReadyString("01000012-0001-0000-0000-000000000000") '''Fixed Price Level ID for Freight Cost

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function


    Function CheckLandedCostExist(ByVal vOrgID As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprice"

            .SqlWhere = " WHERE prdprice.OrganizationID = " & ReadyString(vOrgID) &
                " AND prdprice.ProductID = " & ReadyString(vProductID) &
                " AND prdprice.PriceLevelID = " & ReadyString("01000012-0003-0000-0000-000000000000") '''Fixed Price Level ID for Landed Cost

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function

    Function CheckEmployeePriceExist(ByVal vOrgID As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprice"

            .SqlWhere = " WHERE prdprice.OrganizationID = " & ReadyString(vOrgID) &
                " AND prdprice.ProductID = " & ReadyString(vProductID) &
                " AND prdprice.PriceLevelID = " & ReadyString("B458EA5D-A6B6-460A-9BFA-25CCD98E3CB3") '''Fixed Price Level ID for Employee Price

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function

    Function CheckCorporatePriceExist(ByVal vOrgID As String, ByVal vProductID As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductPriceLevel prdprice"

            .SqlWhere = " WHERE prdprice.OrganizationID = " & ReadyString(vOrgID) &
                " AND prdprice.ProductID = " & ReadyString(vProductID) &
                " AND prdprice.PriceLevelID = " & ReadyString("6C5450DA-6CB8-4704-A9BA-DBD4CD1C1BE1") '''Fixed Price Level ID for Corporate Price

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function


    Function CheckProductInfoExist(ByVal vOrgID As String, ByVal productNo As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strRet As String = ""

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProduct "

            .SqlWhere = " WHERE PublishedByID = " & ReadyString(vOrgID) &
                " AND UploadedByID = " & ReadyString(vOrgID) &
                " AND ProductNo = " & ReadyString(productNo)

            pDT = New DataTable()
            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then '''If not exist in the structured option, insert
            strRet = pDT.Rows(0).Item("ID").ToString()
        End If

        Return strRet

    End Function


    Function SaveToOrganizationPIM() As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim blnRet As Boolean
        Dim pSql As String
        Dim p As New InsertUpdate("LibraryOrganizationPIM")
        Dim intCtr = 0


        If (ProductList.Count > 0) Then

            While (intCtr < ProductList.Count)
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData

                    With .Fields
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "ID"
                            .Searchable = False
                        End With
                    End With

                    .SqlFrom = "FROM LibraryOrganizationPIM"

                    .SqlWhere = " WHERE ProductID = " & ReadyString(ProductList.Item(intCtr))

                    pDT = New DataTable()
                    pDT = .Requery

                End With

                p("ActivationDate") = "getdate()"
                p("ProductID") = ReadyString(ProductList.Item(intCtr))

                If (pDT.Rows.Count > 0) Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                Else '''If not exist, insert
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                intCtr = intCtr + 1
            End While

            blnRet = True
        End If
    End Function
    Function GetUnitMeasureList(ByVal strMeasurementType As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim intCntr As Int16 = 0

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "mea.ID"
                    .Searchable = False
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "mea.MeasureNameID"
                    .Searchable = False
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "mea.SignID"
                    .Searchable = False
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "sgn.Sign"
                End With
                With .Add("MeasureType", SqlDbType.VarChar)
                    .Expression = "(CASE " &
                                  "       WHEN mea.[IsStandardLength] = 1 THEN 'Length' " &
                                  "       WHEN mea.[IsStandardTime] = 1 THEN 'Time' " &
                                  "       WHEN mea.[IsStandardArea] = 1 THEN 'Area' " &
                                  "       WHEN mea.[IsStandardVolCapacity] = 1 THEN 'Vol (Capacity)' " &
                                  "       WHEN mea.[IsStandardVolDry] = 1 THEN 'Vol (Dry)' " &
                                  "       WHEN mea.[IsStandardWeight] = 1 THEN 'Weight/Mass' " &
                                  "       WHEN mea.[IsStandardCooking] = 1 THEN 'Cooking' " &
                                  "       WHEN mea.[IsStandardElectricity] = 1 THEN 'Electrical' " &
                                  "       ELSE 'Non-Standard' " &
                                  "END)"
                End With

            End With

            .SqlFrom = "FROM MobizLibraryPIMMeasure mea " &
                " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (mea.[MeasureNameID] = lbl.ID) " &
                " INNER JOIN MobizLibraryPIMMeasureSign sgn ON (mea.[SignID] = sgn.ID) "

            If (strMeasurementType.Trim().Length > 0) Then
                If (strMeasurementType = "Content") Then
                    .SqlWhere = "WHERE mea.[IsStandardLength] = 1 OR " &
                                "    mea.[IsStandardTime] = 1 OR " &
                                "    mea.[IsStandardArea] = 1 OR " &
                                "    mea.[IsStandardVolCapacity] = 1 OR " &
                                "    mea.[IsStandardVolDry] = 1 OR " &
                                "    mea.[IsStandardWeight] = 1 OR " &
                                "    mea.[IsStandardCooking] = 1 OR " &
                                "    mea.[IsStandardElectricity] = 1"
                Else
                    .SqlWhere = "WHERE (CASE " &
                                "       WHEN mea.[IsStandardLength] = 1 THEN 'Length' " &
                                "       WHEN mea.[IsStandardTime] = 1 THEN 'Time' " &
                                "       WHEN mea.[IsStandardArea] = 1 THEN 'Area' " &
                                "       WHEN mea.[IsStandardVolCapacity] = 1 THEN 'Vol (Capacity)' " &
                                "       WHEN mea.[IsStandardVolDry] = 1 THEN 'Vol (Dry)' " &
                                "       WHEN mea.[IsStandardWeight] = 1 THEN 'Weight/Mass' " &
                                "       WHEN mea.[IsStandardCooking] = 1 THEN 'Cooking' " &
                                "       WHEN mea.[IsStandardElectricity] = 1 THEN 'Electrical' " &
                                "       ELSE 'Non-Standard' " &
                                "END) = " & ReadyString(strMeasurementType)

                End If

                
            End If

            .SqlOrder = " ORDER BY lbl.Label ASC"

            pDT = New DataTable()
            pDT = .Requery

        End With

        Return pDT

    End Function


End Class
