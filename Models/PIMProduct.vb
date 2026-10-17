Public Class PIMProduct
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property ProductID As String

    Public Property OrganizationID As String
    Public Property ProductNo As String
    Public Property ProductName As String

    Public Property IsAsset As Boolean
    Public Property IsEcommerceVendorAllowedToSell As Boolean
    Public Property Brand As String
    Public Property ModelNo As String
    Public Property GroupNo As String
    Public Property Division As String
    Public Property Category As String
    Public Property MarketingText As String
    Public Property Warranty As String

    Public Property ProductFamilyID As String
    Public Property ProductUNSPSCCommIDOrig As String
    Public Property ProductUNSPSCCommID As String

    Public Property PublishedByID As String
    Public Property UploadedByID As String

    Public Property ProductFeatureList As Object()

    Public Property DateAdded As String

    Public Property IsActive As String
    Public Property UserID As String

    Function CheckUniqueProduct() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ID"
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.OrganizationID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ProductFamilyID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ProductFamilyID"
                End With
                With .Add("UNSPSCCommodityID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.UNSPSCCommodityID"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMProduct prd "

            .SqlWhere = "WHERE prd.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND prd.OrganizationID " & IIf(OrganizationID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(OrganizationID)) &
                        " AND prd.ProductNo = " & ReadyString(ProductNo) &
                        " AND prd.ProductName = " & ReadyString(ProductName) &
                        " AND prd.ProductFamilyID " & IIf(OrganizationID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(OrganizationID)) &
                        " AND prd.UNSPSCCommodityID " & IIf(ProductUNSPSCCommID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(ProductUNSPSCCommID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getProductList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ID"
                    .Searchable = False
                End With
                With .Add("PublishedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.PublishedByID"
                    .Searchable = False
                End With
                With .Add("PublishedBy", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "pub.CompanyName"
                End With
                With .Add("UploadedBy", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "upl.CompanyName"
                End With
                With .Add("IsBrandOwner", SqlDbType.Bit)
                    .Searchable = False
                    .Expression = "prd.IsBrandOwner"
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductGroupNo"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                '' ''With .Add("ProductFamilyID", SqlDbType.UniqueIdentifier)
                '' ''    .Expression = "prd.ProductFamilyID"
                '' ''    .Searchable = False
                '' ''End With
                '' ''With .Add("ProductFamily", SqlDbType.VarChar)
                '' ''    .Expression = "prdflylbl.Label"
                '' ''End With
                '' ''With .Add("UNSPSCCommodityID", SqlDbType.UniqueIdentifier)
                '' ''    .Expression = "prd.UNSPSCCommodityID"
                '' ''    .Searchable = False
                '' ''End With
                '' ''With .Add("Commodity", SqlDbType.VarChar)
                '' ''    .Expression = "comm.Commodity"
                '' ''End With
                '' ''With .Add("ClassID", SqlDbType.UniqueIdentifier)
                '' ''    .Expression = "prd.UNSPSCClassID"
                '' ''End With
                '' ''With .Add("ClassCode", SqlDbType.VarChar)
                '' ''    .Expression = "uncls.ClassCode"
                '' ''End With
                '' ''With .Add("Class", SqlDbType.VarChar)
                '' ''    .Expression = "uncls.Class"
                '' ''End With
                '' ''With .Add("FamilyID", SqlDbType.UniqueIdentifier)
                '' ''    .Expression = "prd.UNSPSCFamilyID"
                '' ''End With
                '' ''With .Add("FamilyCode", SqlDbType.VarChar)
                '' ''    .Expression = "unfmly.FamilyCode"
                '' ''End With
                '' ''With .Add("Family", SqlDbType.VarChar)
                '' ''    .Expression = "unfmly.Family"
                '' ''End With
                '' ''With .Add("SegmentID", SqlDbType.UniqueIdentifier)
                '' ''    .Expression = "prd.UNSPSCSegmentID"
                '' ''End With
                '' ''With .Add("SegmentCode", SqlDbType.VarChar)
                '' ''    .Expression = "unseg.SegmentCode"
                '' ''End With
                '' ''With .Add("Segment", SqlDbType.VarChar)
                '' ''    .Expression = "unseg.Segment"
                '' ''End With

                With .Add("DateAdded", SqlDbType.Date)
                    .Expression = "prd.DateAdded"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Expression = " ISNULL(prd.ProductSearchString,'') "
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMProduct prd " &
                    " LEFT JOIN MobizLibraryOrganization pub ON (prd.PublishedByID = pub.ID) " &
                    " LEFT JOIN MobizLibraryOrganization upl ON (prd.UploadedByID = upl.ID) "

            '.SqlFrom = " FROM dbo.MobizLibraryPIMProduct prd " &
            '        " LEFT JOIN MobizLibraryPIMProductFamily prdfly ON (prd.ProductFamilyID = prdfly.ID) " &
            '        " LEFT JOIN MobizLibraryCountriesLanguageLabel prdflylbl ON (prdfly.ProductFamilyVocLabelID = prdflylbl.ID) " &
            '        " LEFT JOIN MobizLibraryUNSPSCCommodity comm ON (prd.UNSPSCCommodityID = comm.ID) " &
            '        " LEFT JOIN MobizLibraryUNSPSCClass uncls ON (prd.UNSPSCClassID = uncls.ID) " &
            '        " LEFT JOIN MobizLibraryUNSPSCFamily unfmly ON (prd.UNSPSCFamilyID = unfmly.ID) " &
            '        " LEFT JOIN MobizLibraryUNSPSCSegment unseg ON (prd.UNSPSCSegmentID = unseg.ID) " &
            '        " LEFT JOIN MobizLibraryOrganization pub ON (prd.PublishedByID = pub.ID) " &
            '        " LEFT JOIN MobizLibraryOrganization upl ON (prd.UploadedByID = upl.ID) "

            .SearchString = strKeyWord

            .SqlOrder = " ORDER BY prd.IsBrandOwner DESC, prd.ProductName ASC "

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getProductFeatureList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "commftr.ID"
                    .Searchable = False
                End With
                With .Add("CommodityID", SqlDbType.UniqueIdentifier)
                    .Expression = "commftr.CommodityID"
                    .Searchable = False
                End With
                With .Add("FeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "commftr.FeatureID"
                    .Searchable = False
                End With
                With .Add("FeatureLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureLabelID"
                    .Searchable = False
                End With
                With .Add("FeatureLabel", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("FeatureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureNameID"
                    .Searchable = False
                End With
                With .Add("Feature", SqlDbType.VarChar)
                    .Expression = "voc.Label"
                End With
                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "commftr.SortOrder"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProductFeature prdftr  " &
                    " INNER JOIN MobizLibraryUNSPSCCommodityFeature commftr ON (commftr.FeatureID = prdftr.CommodityFeatureID) " &
                    " INNER JOIN MobizLibraryPIMFeature ftr ON (prdftr.CommodityFeatureID = ftr.ID)  " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (ftr.FeatureLabelID = lbl.ID)  " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel voc ON (ftr.FeatureNameID = voc.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE commftr.CommodityID IS NULL", " WHERE commftr.CommodityID = " + ReadyString(strID))

            .SqlOrder = "ORDER BY commftr.SortOrder ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getProductFeatureValueList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdftr.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdftr.ProductID"
                End With
                With .Add("CommodityFeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdftr.CommodityFeatureID"
                End With
                With .Add("Value", SqlDbType.VarChar)
                    .Expression = "prdftr.Value"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMProductFeature prdftr "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prdftr.ProductID IS NULL", " WHERE prdftr.ProductID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getProductInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.OrganizationID"
                    .Searchable = False
                End With
                With .Add("Organization", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("ProductFamilyID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ProductFamilyID"
                    .Searchable = False
                End With
                With .Add("ProductFamily", SqlDbType.VarChar)
                    .Expression = "prdflylbl.Label"
                End With
                With .Add("UNSPSCCommodityID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.UNSPSCCommodityID"
                    .Searchable = False
                End With
                With .Add("Commodity", SqlDbType.VarChar)
                    .Expression = "comm.Commodity"
                End With
                With .Add("ClassID", SqlDbType.UniqueIdentifier)
                    .Expression = "comm.ClassID"
                End With
                With .Add("ClassCode", SqlDbType.VarChar)
                    .Expression = "uncls.ClassCode"
                End With
                With .Add("Class", SqlDbType.VarChar)
                    .Expression = "uncls.Class"
                End With
                With .Add("FamilyID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncls.FamilyID"
                End With
                With .Add("FamilyCode", SqlDbType.VarChar)
                    .Expression = "unfmly.FamilyCode"
                End With
                With .Add("Family", SqlDbType.VarChar)
                    .Expression = "unfmly.Family"
                End With
                With .Add("SegmentID", SqlDbType.UniqueIdentifier)
                    .Expression = "unfmly.SegmentID"
                End With
                With .Add("SegmentCode", SqlDbType.VarChar)
                    .Expression = "unseg.SegmentCode"
                End With
                With .Add("Segment", SqlDbType.VarChar)
                    .Expression = "unseg.Segment"
                End With
                With .Add("DateAdded", SqlDbType.Date)
                    .Expression = "prd.DateAdded"
                End With
                With .Add("IsActive", SqlDbType.Date)
                    .Expression = "prd.IsActive"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMProduct prd " &
                    " INNER JOIN MobizLibraryOrganization org ON (prd.OrganizationID = org.ID) " &
                    " INNER JOIN MobizLibraryPIMProductFamily prdfly ON (prd.ProductFamilyID = prdfly.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel prdflylbl ON (prdfly.ProductFamilyVocLabelID = prdflylbl.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCCommodity comm ON (prd.UNSPSCCommodityID = comm.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCClass uncls ON (comm.ClassID = uncls.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCFamily unfmly ON (uncls.FamilyID = unfmly.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCSegment unseg ON (unfmly.SegmentID = unseg.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE prd.ID IS NULL", " WHERE prd.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveProductInfo() As String
        Dim p As New InsertUpdate("MobizLibraryPIMProduct")
        Dim pSql As String
        Dim strIDReturn As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable

        p("ProductNo") = ReadyString(ProductNo)
        p("ProductName") = ReadyString(ProductName)
        p("IsAsset") = ReadyString(IsAsset)
        p("IsEcommerceVendorAllowedToSell") = ReadyString(IsEcommerceVendorAllowedToSell)
        p("Brand") = ReadyString(Brand)
        p("ModelNo") = ReadyString(ModelNo)
        p("ProductGroupNo") = ReadyString(GroupNo)
        p("DivisionDescription") = ReadyString(Division)
        p("MarketingText") = ReadyString(MarketingText)
        p("Warranty") = ReadyString(Warranty)

        If (ProductFamilyID <> Nothing) Then
            p("ProductFamilyID") = IIf(ProductFamilyID.Trim().Length <= 0, "NULL", ReadyString(ProductFamilyID))
        End If
        If (ProductUNSPSCCommID <> Nothing) Then
            p("UNSPSCCommodityID") = IIf(ProductUNSPSCCommID.Trim().Length <= 0, "NULL", ReadyString(ProductUNSPSCCommID))
        End If

        p("IsActive") = 1
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("DateAdded") = "getdate()"
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Product Feature
            ''' Category for now
            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "ID"
                        .Searchable = False
                    End With
                End With
                .SqlFrom = " FROM MobizLibraryPIMProductVariableFeatures001 "
                .SqlWhere = IIf(strIDReturn.Trim().Length = 0, "WHERE ProductID IS NULL", " WHERE ProductID = " + ReadyString(strIDReturn))

                pDT = .Requery

            End With

            p = New InsertUpdate("MobizLibraryPIMProductVariableFeatures001")
            p("ProductID") = ReadyString(strIDReturn)
            p("C0020") = ReadyString(Category)

            If (pDT.Rows.Count <= 0) Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0).Item("ID").ToString()))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            ProductID = strIDReturn

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function


End Class