Public Class PIMProductPrice
    Public Property MaxReturnRow As Integer = 50


    Public Property ID As String
    Public Property ProductID As String
    Public Property PriceLevelID As String
    Public Property ProductPriceList As Object()
    Public Property FilterName As String
    Public Property PriceLevelName As String
    Public Property KeywordList As Object()

    Function getProductPriceByStyleNumberColorCodeList(ByVal strID As String, ByVal strPriceLevelID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strProductGroupNo As String

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductGroupNo"
                    .Searchable = False
                End With
            End With
            .SqlFrom = " FROM MobizLibraryPIMProduct prd "
            .SqlWhere = " WHERE prd.ID = " & ReadyString(strID)
            pDT = .Requery
        End With

        strProductGroupNo = ""
        If (pDT.Rows.Count > 0) Then
            If (pDT.Rows(0).Item("ProductGroupNo").ToString().Trim().Length > 0) Then
                strProductGroupNo = pDT.Rows(0).Item("ProductGroupNo").ToString().Trim()
            End If
        End If

        pFinder = New Finder
        pDT = New DataTable()

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prd.ID"
                    .Searchable = False
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductGroupNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 C0017 " &
                          " FROM MobizLibraryPIMProductVariableFeatures001 " &
                          " WHERE ProductID = prd.ID) "
                End With
                With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.PriceLevelID"
                    .Searchable = False
                End With
                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "prclvl.PriceLevel"
                End With
                With .Add("Price", SqlDbType.Money)
                    .Expression = "ISNULL(prdprice.Price,0)"
                    .Searchable = False
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdprice.UserID"
                End With
                With .Add("UpdatedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(LTRIM(RTRIM(Surname)), '') + " &
                             " ISNULL(NULLIF(', ' + LTRIM(RTRIM(FirstName)),', '), '') +  " &
                             " ISNULL(NULLIF(' ' + LTRIM(RTRIM(MiddleName)),' '), '')"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "prdprice.LastUpdated"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct prd " &
                " LEFT JOIN MobizLibraryPIMProductPriceLevel prdprice ON (prdprice.ProductID = prd.ID AND prdprice.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & " AND prdprice.PriceLevelID " & IIf(strPriceLevelID.Trim().Length > 0, " = " & ReadyString(strPriceLevelID), " IS NULL ") & ") " &
                " LEFT JOIN dbo.MobizLibraryPIMPriceLevel prclvl ON (prdprice.PriceLevelID = prclvl.ID) " &
                " LEFT JOIN dbo.MobizLibraryPeople ppol ON (prdprice.UserID = ppol.ID) "

            .SqlWhere = " WHERE " & IIf(strProductGroupNo.Trim().Length > 0, " prd.ProductGroupNo = " & ReadyString(strProductGroupNo), " prd.ID = " & ReadyString(strID)) & " AND prdprice.OrganizationID IS NOT NULL "

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveProductPriceInfo() As Boolean
        Dim p As New InsertUpdate("MobizLibraryPIMProductPriceLevel")
        Dim pSql As String
        Dim blnRet As Boolean = False
        Dim strIDRec As String

        Try

            Common.WebRequest.ProData.BeginTransaction()

            '''Save Product Price
            For Each objValue As Object In ProductPriceList

                p = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

                p("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
                p("ProductID") = ReadyString(objValue("ProductID").ToString())
                p("PriceLevelID") = ReadyString(objValue("PriceLevelID").ToString())
                p("Price") = objValue("Price").ToString()
                p("DeactivateDiscount") = 0
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If (CStr2(objValue("ID").ToString().Trim()) = "") Then
                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()
                Else
                    strIDRec = objValue("ID").ToString()
                    pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                '''Save product if not yet exist in Customer Product List
                pSql = "INSERT INTO LibraryOrganizationPIM " &
                           "(ProductID  " &
                           ",ActivationDate  " &
                           ",IsDeactivated  " &
                           ",DeactivationDate  " &
                           ",DeactivationReason " &
                           ",Comments " &
                           ",IsBan) " &
                        "SELECT DISTINCT orgprdprclvl.ProductID " &
                              ",getdate() as ActivationDate " &
                              ",0 as IsDeactivated " &
                              ",NULL as DeactivationDate " &
                              ",NULL as DeactivationReason " &
                              ",NULL as Comments " &
                              ",0 as IsBan " &
                        " FROM MobizLibraryPIMProductPriceLevel orgprdprclvl " &
                        " LEFT JOIN LibraryOrganizationPIM orgprd ON (orgprd.ProductID = orgprdprclvl.ProductID) " &
                        " WHERE orgprdprclvl.ProductID = " & ReadyString(objValue("ProductID").ToString()) &
                        " AND orgprd.ID IS NULL"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

            Next objValue

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProductPriceLevel'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            blnRet = True

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return blnRet

    End Function

    Function GetBrand() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "ISNULL(CASE WHEN pim.Brand = '' THEN 'N/A' ELSE pim.Brand END,'N/A')"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationPIM orgpim" &
                       " LEFT JOIN MobizLibraryPIMProduct pim ON (pim.ID = orgpim.ProductID)"

            .SqlOrder = "ORDER BY Label"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetCategory() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                'With .Add("ID", SqlDbType.UniqueIdentifier)
                '    .Searchable = False
                '    .Expression = "MobizLibraryPIMProductVariableFeatures001.ID"
                'End With

                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "ISNULL(CASE WHEN dbo.MobizLibraryPIMProductVariableFeatures001.c0020 = '' THEN 'N/A' ELSE dbo.MobizLibraryPIMProductVariableFeatures001.c0020 END,'N/A')"
                End With

                .Add("AllowDecimal", SqlDbType.Bit)
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                        "LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID "

            '.SqlWhere = "WHERE MobizLibraryPIMTemplate.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))

            .SqlOrder = "Order By Label"

            pDT = .Requery


        End With

        Return pDT
    End Function

    Function GetFeatureColumnName(ByVal pLabel As String)
        Dim pSql As String
        Dim pFinder As Finder
        Dim pDt As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                .Add("PIMFeatureTable", SqlDbType.VarChar)
                .Add("PIMFeatureColumn", SqlDbType.VarChar)
                .Add("Label", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM dbo.MobizLibraryCountriesLanguageLabel INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMFeature ON dbo.MobizLibraryCountriesLanguageLabel.ID = dbo.MobizLibraryPIMFeature.FeatureLabelID "

            .SqlWhere = "where Label = " & ReadyString(pLabel)

            pDt = .Requery
        End With

        If pDt.Rows.Count > 0 Then

            If CStr(pDt.Rows(0).Item("PIMFeatureColumn")) = vbNullString Then
                Return pLabel
            End If
        End If

        Return CStr(pDt.Rows(0).Item("PIMFeatureColumn"))


    End Function


    Function GetProductList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDTFields As DataTable
        Dim pSql As String = ""
        Dim intCntr As Integer
        Dim SqlWhere As String
        Dim strProductColumns As String

        Dim count As Integer = 1
        Dim pColumn As String

        If FilterName.Trim() = "" Then
            pSql = "SELECT DISTINCT pim.ID," &
                    "pim.ProductNo, " &
                    "pim.ProductGroupNo, " &
                    "(select top 1 measbarcode.Barcode from MobizLibraryPIMProductMeasurement meas " &
                    " LEFT JOIN MobizLibraryPIMProductMeasurementBarcode measbarcode ON (meas.ID = measbarcode.StockMeasurementsID)" &
                    " WHERE meas.ProductID = pim.ID order by meas.Quantity) AS Barcode, " &
                    "pim.Brand, " &
                    "pim.ProductName, " &
                    "C0017 AS Size, " &
                    "(select top 1 meas.Quantity from MobizLibraryPIMProductMeasurement meas " &
                    " LEFT JOIN MobizLibraryPIMMeasure measID ON (measID.ID = meas.UnitMeasureID)" &
                    " LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (lbl.ID = measID.MeasureNameID)" &
                    " WHERE meas.ProductID = pim.ID order by meas.Quantity) AS Quantity, " &
                    "(select top 1 lbl.label from MobizLibraryPIMProductMeasurement meas " &
                    " LEFT JOIN MobizLibraryPIMMeasure measID ON (measID.ID = meas.UnitMeasureID)" &
                    " LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (lbl.ID = measID.MeasureNameID)" &
                    " WHERE meas.ProductID = pim.ID order by meas.Quantity) AS UnitMeasure, " &
                    "priceLvl.PriceLevel, " &
                    "prodPriceLvl.Price "
   
            pSql &= " FROM LibraryOrganizationPIM orgpim" &
                    " LEFT JOIN MobizLibraryPIMProduct pim ON (pim.ID = orgpim.ProductID)" &
                    " LEFT JOIN MobizLibraryPIMProductPriceLevel prodPriceLvl ON (prodPriceLvl.ProductID = orgpim.ProductID)" &
                    " LEFT JOIN MobizLibraryPIMPriceLevel priceLvl ON (priceLvl.ID = prodPriceLvl.PriceLevelID)" &
                    " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (orgpim.ProductID = prd001.ProductID)"

            If PriceLevelName.Trim() <> "" Then
                pSql &= " WHERE orgpim.IsDeactivated = 0 AND prodPriceLvl.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & " AND prodPriceLvl.PriceLevelID = " & ReadyString(PriceLevelName.Trim())
            Else
                pSql &= " WHERE orgpim.IsDeactivated = 0 AND prodPriceLvl.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
            End If

        Else
            For Each pKeyword As Object In KeywordList

                pSql &= "SELECT DISTINCT pim.ID, " &
                   "pim.ProductNo, " &
                   "pim.ProductGroupNo, " &
                   "(select top 1 measbarcode.Barcode from MobizLibraryPIMProductMeasurement meas " &
                   " LEFT JOIN MobizLibraryPIMProductMeasurementBarcode measbarcode ON (meas.ID = measbarcode.StockMeasurementsID)" &
                   " WHERE meas.ProductID = pim.ID order by meas.Quantity) AS Barcode, " &
                   "pim.Brand, " &
                   "pim.ProductName, " &
                   "C0017 AS Size, " &
                   "(select top 1 meas.Quantity from MobizLibraryPIMProductMeasurement meas " &
                   " LEFT JOIN MobizLibraryPIMMeasure measID ON (measID.ID = meas.UnitMeasureID)" &
                   " LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (lbl.ID = measID.MeasureNameID)" &
                   " WHERE meas.ProductID = pim.ID order by meas.Quantity) AS Quantity, " &
                   "(select top 1 lbl.label from MobizLibraryPIMProductMeasurement meas " &
                   " LEFT JOIN MobizLibraryPIMMeasure measID ON (measID.ID = meas.UnitMeasureID)" &
                   " LEFT JOIN MobizLibraryCountriesLanguageLabel lbl ON (lbl.ID = measID.MeasureNameID)" &
                   " WHERE meas.ProductID = pim.ID order by meas.Quantity) AS UnitMeasure, " &
                   "priceLvl.PriceLevel, " &
                   "prodPriceLvl.Price "

                pSql &= " FROM LibraryOrganizationPIM orgpim" &
                        " LEFT JOIN MobizLibraryPIMProduct pim ON (pim.ID = orgpim.ProductID)" &
                        " LEFT JOIN MobizLibraryPIMProductPriceLevel prodPriceLvl ON (prodPriceLvl.ProductID = orgpim.ProductID)" &
                        " LEFT JOIN MobizLibraryPIMPriceLevel priceLvl ON (priceLvl.ID = prodPriceLvl.PriceLevelID)" &
                        " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (orgpim.ProductID = prd001.ProductID)"

                If pKeyword.Trim() <> "" Then
                    If FilterName.Trim() = "Brand" Then
                        pSql &= " WHERE orgpim.IsDeactivated = 0 AND prodPriceLvl.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & " AND pim.Brand =" & ReadyString(pKeyword.Trim())
                    Else
                        pColumn = GetFeatureColumnName(FilterName.Trim())


                        pSql &= " WHERE orgpim.IsDeactivated = 0 AND prodPriceLvl.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & " AND " & pColumn & " = " & ReadyString(pKeyword.Trim())
                        If (pKeyword.Trim() = "N/A") Then
                        End If

                    End If

                    If PriceLevelName.Trim() <> "" Then
                        pSql &= " AND prodPriceLvl.PriceLevelID = " & ReadyString(PriceLevelName.Trim())

                    End If
                End If
                'if greater than 1 and not last element
                If KeywordList.Length > 1 And count < KeywordList.Length Then
                    pSql &= " UNION "
                End If


                count = count + 1
            Next pKeyword
        End If


        pSql &= " ORDER BY pim.Brand ASC, pim.ProductName ASC, priceLvl.PriceLevel ASC"



        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function


End Class
