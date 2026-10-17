Imports Newtonsoft.Json

Public Class BrowserController
    Inherits System.Web.Mvc.Controller

    Dim mFinder As Finder

    Function Index() As ActionResult
        Return View()
    End Function

    Function LibraryVDMMedia()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pOut As New JsonDictionary
        Dim pSql As String
        Dim keywords As String = Common.WebRequest.Data("keywords")

        pSql = " SELECT * FROM LibraryVDMMedia " & vbCrLf

        If Not String.IsNullOrEmpty(keywords) Then
            pSql += " WHERE Name LIKE " & ReadyStringWild(keywords)
        End If

        pOut.Add("Results", Common.WebRequest.ProData.Execute(pSql))

        Return pOut.Serialize()
    End Function

    Function LibraryVendingMachineBoards()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String

        pSql = " SELECT * " & vbCrLf &
                " FROM LibraryVendingMachineBoards " & vbCrLf &
                " ORDER BY Name " & vbCrLf

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Function LibraryFOWSTRealCashier()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String

        pSql = " SELECT ID, ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '') as CashierName " & vbCrLf &
                " FROM MobizLibraryPeople " & vbCrLf &
                " WHERE ID IN (SELECT DISTINCT UserID FROM TransInvoice" & vbCrLf &
                "   WHERE FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                "   AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & vbCrLf &
                " ) "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Public Function LibraryPosVendors()
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .MaxRecords = IIf(CBool2(Common.WebRequest.Data("IsAll")), 0, 50)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPosVendors.ID"
                    .Searchable = False
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPosVendors.Name"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPosVendors"

            .SqlWhere = " WHERE IsActive = 1"

            .SqlOrder = "ORDER BY Name ASC"
        End With

        Return SubmitFinder()
    End Function
    Public Function LibraryOrganizationFranchiseeFowst()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .MaxRecords = IIf(CBool2(Common.WebRequest.Data("IsAll")), 0, 50)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstCode", SqlDbType.VarChar)
                    .Expression = "fowst.Code"
                End With
                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "fowst.LastUpdated"
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("CompanyID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                    .Searchable = False
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = " ISNULL(" & vbCrLf &
                        "   NULLIF(" & vbCrLf &
                        "       dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') + ', ' + ISNULL(" & vbCrLf &
                        "       fowst.Street, '') + ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(" & vbCrLf &
                        "       ctry.Name, '')) " & vbCrLf &
                        "   , '')" & vbCrLf &
                        "   , fowst.Name)"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFOWSTFranchiseeRequests as franchisee " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.OrganizationID = franchisee.CompanyID AND fowst.IsActive = 1 " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganization as org ON org.ID = franchisee.CompanyID " & vbCrLf &
                        " LEFT OUTER JOIN MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID " & vbCrLf &
                        " LEFT OUTER JOIN MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID " & vbCrLf

            .SqlWhere = " WHERE DBID = " & ReadyString(WebRequest.Instance.OrgID)

            .SqlOrder = "ORDER BY fowst.Name ASC"
        End With

        Return SubmitFinder()
    End Function

    Public Function LibraryDiscountSettings()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String = " SELECT * FROM (SELECT ID, ISNULL(DiscountType,'') as DiscountType, ISNULL(PercentDiscount,0) as PercentDiscount " & vbCrLf &
                            "   FROM LibraryDiscountSettings " & vbCrLf &
                            "   WHERE PercentDiscount <> 0 " & vbCrLf &
                            " UNION ALL " & vbCrLf &
                            "   SELECT ID, Name as DiscountType, DiscountPercentage as PercentDiscount FROM LibraryCustomDiscounts " & vbCrLf &
                            " ) as t " & vbCrLf &
                            " ORDER BY DiscountType ASC "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Public Function LibraryCRMTodoCategories()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim modules As String = Common.WebRequest.Data("module")
        Dim pSql As String = " SELECT * " & vbCrLf &
                            " FROM LibraryCRMTodoCategories" & vbCrLf

        If Not String.IsNullOrWhiteSpace(modules) Then
            pSql += " WHERE Module = " & ReadyString(modules)
        End If

        pSql += " ORDER BY CategoryName ASC "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Public Function LibraryCategoriesOfProductOrServices()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        'ginagamit sa Online Directory, dili ni category sa PIM, category ni sa gina offer sa Vendor
        Dim pSql As String = " SELECT ID, CategoryName " & vbCrLf &
                            " FROM MobizLibraryCategoriesOfProductOrServices" & vbCrLf & vbCrLf &
                            IIf(CStr2(Common.WebRequest.Data("keywords")) = "", "", " WHERE CategoryName LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))) & vbCrLf &
                            " ORDER BY CategoryName ASC "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Public Function LibraryBrandNames()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        'ginagamit sa Online Directory, dili ni brand sa PIM, brand ni sa gina offer sa Vendor
        Dim pSql As String = " SELECT ID, BrandName " & vbCrLf &
                            " FROM MobizLibraryBrandNames" & vbCrLf & vbCrLf &
                            IIf(CStr2(Common.WebRequest.Data("keywords")) = "", "", " WHERE BrandName LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))) & vbCrLf &
                            " ORDER BY BrandName ASC "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Public Function LibraryCanvassForm()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String = " SELECT ID, CanvassName, LastUpdated " & vbCrLf &
                            " FROM LibraryPriceCanvass" & vbCrLf

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Public Function LibraryContractDesignerTemplate()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        Dim pSql As String = "SELECT ID, TemplateName FROM LibraryContractDesignerTemplate WHERE Module = " & ReadyString(Common.WebRequest.Data("Module"))
        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function
    Public Function LibraryHotButtonTemplate()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pim.ID"
                    .Searchable = False
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "pim.Name"
                End With
                With .Add("LastUpdated", SqlDbType.UniqueIdentifier)
                    .Expression = "pim.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM LibraryPimMenu as pim INNER JOIN MobizLibraryPeople as ppl " & vbCrLf &
                        " ON pim.UserID = ppl.ID WHERE pim.IsActive = 1 " & vbCrLf

            If Not CBool2(WebRequest.Instance.Business.SystemDefaults.EnableVDMFeatures) Then
                .SqlFrom &= " AND VDMModelID IS NULL "
            End If

            .SqlOrder = "ORDER BY pim.Name ASC"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryKBCase() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "kb.ID"
                    .Searchable = False
                End With
                With .Add("Case", SqlDbType.VarChar)
                    .Expression = "kb.CaseDescription"
                End With
                With .Add("ShortLink", SqlDbType.UniqueIdentifier)
                    .Expression = "kb.ShortLink"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM TransKnowledgeManagement kb "

            .SqlOrder = "ORDER BY kb.CaseDescription ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPaymentTerms() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "terms.ID"
                    .Searchable = False
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "terms.Name"
                End With
                With .Add("Days", SqlDbType.Int)
                    .Expression = "terms.Days"
                End With
            End With

            .SqlFrom = "FROM LibraryPaymentTerms terms "

            .SqlWhere = " WHERE IsActive  = 1 "

            .SqlOrder = "ORDER BY terms.Name ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCRMTeam() As String
        'Used for KB (Private LibraryPeople)
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "TeamName"
                End With
            End With

            .SqlFrom = "FROM LibraryKnowledgeManagement WHERE TeamName <> '' "

            .SqlOrder = "ORDER BY TeamName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryTeam() As String
        'Used for CRM
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "TeamName"
                End With
            End With

            .SqlFrom = "FROM LibraryCRMTeams "
            .SqlWhere = " WHERE ISNULL(Type, 'team') = 'team' "
            .SqlOrder = "ORDER BY TeamName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDepartment() As String
        'Used for CRM
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "TeamName"
                End With
            End With

            .SqlFrom = "FROM LibraryCRMTeams "
            .SqlWhere = " WHERE Type = 'department' "
            .SqlOrder = "ORDER BY TeamName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryManPowerAgency() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryOrganization.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganization.CompanyName"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationSupplier " & vbCrLf &
                        "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = LibraryOrganizationSupplier.SupplierID "
            .SqlWhere = " WHERE LibraryOrganizationSupplier.TaggedAs = 'manpower' "
            .SqlOrder = "ORDER BY MobizLibraryOrganization.CompanyName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryVendor() As String
        'Used for CRM
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryOrganization.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganization.CompanyName"
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationSupplier " & vbCrLf &
                        "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = LibraryOrganizationSupplier.SupplierID " & vbCrLf
            .SqlWhere = " WHERE LibraryOrganizationSupplier.TaggedAs IS NULL "
            .SqlOrder = "ORDER BY MobizLibraryOrganization.CompanyName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryVendorPrice() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pProductID As String = Common.WebRequest.Data("ProductID")
        Dim pOptionalProductID As String = Common.WebRequest.Data("OptionalProductID")
        Dim pDT As DataTable

        With CreateFinder()
            With .Fields
                With .Add("VendorID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryOrganizationSupplier.SupplierID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganization.CompanyName"
                End With
                With .Add("Price", SqlDbType.Decimal)
                    .Expression = "LibraryVendorPrices.Price"
                End With
                With .Add("UnitPrice", SqlDbType.Decimal)
                    .Expression = "LibraryVendorPrices.UnitPrice"
                End With
                With .Add("PaymentTerm", SqlDbType.VarChar)
                    .Expression = "LibraryPaymentTerms.Name"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "LibraryVendorPrices.LastUpdated"
                End With
                With .Add("UpdatedByName", SqlDbType.VarChar)
                    .Expression = ComposePeopleName()
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationSupplier " & vbCrLf &
                "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = LibraryOrganizationSupplier.SupplierID " & vbCrLf
            .SqlFrom &= " LEFT JOIN LibraryVendorPrices ON LibraryOrganizationSupplier.SupplierID = LibraryVendorPrices.VendorID" & vbCrLf
            If String.IsNullOrWhiteSpace(pOptionalProductID) = False Then
                .SqlFrom &= " AND LibraryVendorPrices.ProductID = " & ReadyString(pOptionalProductID) & vbCrLf
            End If
            .SqlFrom &= " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryVendorPrices.UpdatedBy" & vbCrLf &
                 " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
            .SqlFrom &= " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf

            .SqlWhere = " WHERE 1=1" & vbCrLf
            If String.IsNullOrWhiteSpace(pProductID) = False Then
                .SqlWhere &= " AND LibraryVendorPrices.ProductID = " & ReadyString(pProductID) & vbCrLf &
                            " AND ISNULL(LibraryVendorPrices.IsActive, 1) = 1 " & vbCrLf
            End If
            .SqlWhere &= " AND ISNULL(LibraryOrganizationSupplier.Deactivated, 0) = 0" & vbCrLf
            .SqlOrder = "ORDER BY MobizLibraryOrganization.CompanyName ASC" & vbCrLf

            pDT = mFinder.Requery
            If pDT.Rows.Count = 0 Then
                ' todo (spaghetti code) same code as above but with different parameters
                pOptionalProductID = pProductID
                pProductID = Nothing
                With CreateFinder()
                    With .Fields
                        With .Add("VendorID", SqlDbType.UniqueIdentifier)
                            .Expression = "LibraryOrganizationSupplier.SupplierID"
                        End With
                        With .Add("CompanyName", SqlDbType.VarChar)
                            .Expression = "MobizLibraryOrganization.CompanyName"
                        End With
                        With .Add("Price", SqlDbType.Decimal)
                            .Expression = "LibraryVendorPrices.Price"
                        End With
                        With .Add("PaymentTerm", SqlDbType.VarChar)
                            .Expression = "LibraryPaymentTerms.Name"
                        End With
                        With .Add("LastUpdated", SqlDbType.DateTime)
                            .Expression = "LibraryVendorPrices.LastUpdated"
                        End With
                        With .Add("UpdatedByName", SqlDbType.VarChar)
                            .Expression = ComposePeopleName()
                        End With
                    End With

                    .SqlFrom = "FROM LibraryOrganizationSupplier " & vbCrLf &
                        "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = LibraryOrganizationSupplier.SupplierID " & vbCrLf
                    .SqlFrom &= " LEFT JOIN LibraryVendorPrices ON LibraryOrganizationSupplier.SupplierID = LibraryVendorPrices.VendorID" & vbCrLf
                    If String.IsNullOrWhiteSpace(pOptionalProductID) = False Then
                        .SqlFrom &= " AND LibraryVendorPrices.ProductID = " & ReadyString(pOptionalProductID) & vbCrLf
                    End If
                    .SqlFrom &= " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryVendorPrices.UpdatedBy" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
                    .SqlFrom &= " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf

                    .SqlWhere = " WHERE 1=1" & vbCrLf
                    If String.IsNullOrWhiteSpace(pProductID) = False Then
                        .SqlWhere &= " AND LibraryVendorPrices.ProductID = " & ReadyString(pProductID) & vbCrLf
                    End If
                    .SqlWhere &= " AND ISNULL(LibraryOrganizationSupplier.Deactivated, 0) = 0" & vbCrLf
                    .SqlOrder = "ORDER BY MobizLibraryOrganization.CompanyName ASC" & vbCrLf

                    pDT = mFinder.Requery
                    If pDT.Rows.Count = 0 Then
                    End If
                    Return JsonDictionary.Serialize("Results", pDT)
                End With
            End If
            Return JsonDictionary.Serialize("Results", pDT)
        End With

    End Function

    <HttpPost()>
    Public Function LibraryProspectPeople() As String
        'Used for CRM (Private LibraryPeople)
        Dim strOrgID As String

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(FirstName + ' ', '') + ISNULL(MiddleName + ' ', '') + ISNULL(Surname + ' ', '')"
                End With
            End With

            If Common.WebRequest.Data("IsMcr") <> Nothing Then
                .SqlFrom = "FROM MobizLibraryPeople "
                .SqlWhere = "WHERE Surname <> '' "
            Else
                .SqlFrom = "FROM LibraryPeople "
            End If

            .SqlOrder = "ORDER BY Surname"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryProspectOrg() As String
        'Used for CRM (Private LibraryOrganization)
        Dim strOrgID As String

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("OrgID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "CompanyName"
                End With
            End With

            If Common.WebRequest.Data("IsMcr") <> Nothing Then
                .SqlFrom = "FROM MobizLibraryOrganization "
                .SqlWhere = "WHERE Companyname <> '' AND ISNULL(CONVERT(varchar(MAX), OriginalKeyID), '') = '' "
            Else
                .SqlFrom = "FROM LibraryOrganization "
            End If


            .SqlOrder = "ORDER BY CompanyName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryProject() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstName", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.Name"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
                With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                    .Expression = "fowst.IsFOWSTInsideAMall"
                End With
                With .Add("FOWSTMallName", SqlDbType.VarChar)
                    .Expression = "fowst.FOWSTMallName"
                End With
            End With

            If CInt(WebRequest.Instance.Data("IsThirdPartyOrganization")) = 1 Then
                strOrgID = WebRequest.Instance.Data("ThirdPartyOrganizationID")
            Else
                strOrgID = WebRequest.Instance.OrgID
                If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
                    If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                        strOrgID = WebRequest.Instance.Data("OrganizationID").Trim()
                    End If
                End If
            End If

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
                " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)"

            .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) &
                        " AND fowst.FOWSTTypeID = 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'"

            .SqlOrder = "ORDER BY Address"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationRepresentative() As String

        Dim pOrganizationID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        pOrganizationID = WebRequest.Instance.OrgID

        If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
            If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                pOrganizationID = WebRequest.Instance.Data("OrganizationID").Trim()
            End If
        End If

        'todo: add security
        With CreateFinder()
            .MaxRecords = If(CDec2(WebRequest.Instance.Data("Top1")) > 0, 1, 0)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgrep.ID"
                    .Searchable = False
                End With
                With .Add("RepresentativeName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.Title)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.LastName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(orgrep.GenerationalSuffix)),', '), '') "
                End With
                With .Add("TelNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationRepTel(orgrep.ID)"
                End With
                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationRepMob(orgrep.ID)"
                End With
                With .Add("Email", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationRepEmail(orgrep.ID)"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "orgrep.IsActive"
                End With
            End With
            .SqlFrom = "FROM dbo.MobizLibraryOrganizationRepresentatives orgrep " &
                    " INNER JOIN dbo.MobizLibraryOrganization org ON (orgrep.OrganizationID = org.ID)"
            .SqlWhere = " WHERE " & IIf(pOrganizationID.Trim().Length = 0, " orgrep.OrganizationID IS NULL ", " orgrep.OrganizationID = " & ReadyString(pOrganizationID))
            .SqlOrder = "ORDER BY orgrep.LastName ASC, " &
                " orgrep.FirstName ASC," &
                " orgrep.MiddleName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationEmployee() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .MaxRecords = If(CDec2(WebRequest.Instance.Data("Top1")) > 0, 1, 0)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPeople.ID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') + " &
                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '')"
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 EmailAddress FROM MobizLibraryPeopleContactEmail WHERE PeopleID = MobizLibraryPeople.ID ORDER BY LastUpdated DESC)"
                End With
            End With
            .SqlFrom = "FROM LibraryOrganizationEmployee" &
                    " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationEmployee.PeopleID" & vbCrLf
            .SqlFrom &= " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

            .SqlWhere = " WHERE LibraryOrganizationEmployee.Deactivated = 0"

            .SqlOrder = "ORDER BY MobizLibraryPeople.Surname ASC, " &
                " MobizLibraryPeople.FirstName ASC," &
                " MobizLibraryPeople.MiddleName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationEmployeeFOWST() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .MaxRecords = If(CDec2(WebRequest.Instance.Data("Top1")) > 0, 1, 0)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPeople.ID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') + " &
                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '')"
                End With
            End With
            .SqlFrom = "FROM LibraryOrganizationEmployeeFOWST" &
                    " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationEmployeeFOWST.PeopleID" & vbCrLf
            .SqlFrom &= " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

            .SqlWhere = " WHERE LibraryOrganizationEmployeeFOWST.FowstID = " & ReadyString(WebRequest.Instance.FowstID) & vbCrLf
            .SqlOrder = "ORDER BY MobizLibraryPeople.Surname ASC, " &
                " MobizLibraryPeople.FirstName ASC," &
                " MobizLibraryPeople.MiddleName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowst() As String
        Try
            Dim strOrgID As String
            Dim pDeviceSerial As String
            Dim pAddFowstsToDeviceIfDeviceHasNoFowst As Boolean = CBool2(Common.WebRequest.Data("AddFowstsToDeviceIfDeviceHasNoFowst"))
            Dim pRequireLogIn = CBool2(Common.WebRequest.Data("RequireLogIn"))
            Dim pUuid As String
            Dim pDeviceID As String
            Dim pFowstCount As Integer
            Dim pFowstTypeID = Common.WebRequest.Data("FowstTypeID")
            Dim pIncludeAllFowstOptions = CBool2(WebRequest.Instance.Data("IncludeAllFowstOptions"))

            If pRequireLogIn Then
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If pAddFowstsToDeviceIfDeviceHasNoFowst Then
                pUuid = Common.WebRequest.Data("Uuid")
                If String.IsNullOrWhiteSpace(pUuid) = False Then
                    pDeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
                    pFowstCount = WebRequest.Instance.Business.LibraryDevice.FowstCount(pDeviceID)
                    If pFowstCount = 0 Then
                        WebRequest.Instance.Business.LibraryDevice.AddOrgFowsts(pDeviceID, WebRequest.Instance.OrgID)
                    End If
                End If
            End If

            With CreateFinder()
                .Distinct = True
                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "fowst.ID"
                        .Searchable = False
                    End With
                    With .Add("FowstName", SqlDbType.VarChar)
                        .Expression = "fowst.Name"
                    End With
                    With .Add("Code", SqlDbType.VarChar)
                        .Expression = "ISNULL(fowst.Code, '')"
                    End With
                    With .Add("Address", SqlDbType.VarChar)
                        .Expression = "               ISNULL(" & vbCrLf &
                            "                 NULLIF(" & vbCrLf &
                            "                   dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') + ', ' + ISNULL(" & vbCrLf &
                            "                     fowst.Street, '') + ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(" & vbCrLf &
                            "                     ctry.Name, '')) " & vbCrLf &
                            "                 , '')" & vbCrLf &
                            "               , fowst.Name)"
                    End With
                    With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                        .Expression = "fowst.IsFOWSTInsideAMall"
                    End With
                    With .Add("FOWSTMallName", SqlDbType.VarChar)
                        .Expression = "fowst.FOWSTMallName"
                    End With
                    With .Add("DefaultPosView", SqlDbType.VarChar)
                        .Expression = "fowst.DefaultPosView"
                    End With

                    If CBool2(WebRequest.Instance.Data("ShowHBTemplate")) Then
                        With .Add("HBTemplateID", SqlDbType.VarChar)
                            .Expression = "ISNULL(convert(varchar(max), fowst.DefaultHotButtonTemplateID), '')"
                        End With
                    End If
                End With

                If CInt(WebRequest.Instance.Data("IsThirdPartyOrganization")) = 1 Then
                    strOrgID = WebRequest.Instance.Data("ThirdPartyOrganizationID")
                    Dim mainloc As Business = New Business(Common.WebRequest.ProData)
                    mainloc.LibraryOrganizationFOWST.AddOrUpdateFromMainAddress(strOrgID)
                Else
                    strOrgID = WebRequest.Instance.OrgID
                    If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
                        If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                            strOrgID = WebRequest.Instance.Data("OrganizationID").Trim()
                        End If
                    End If
                End If


                If CInt(WebRequest.Instance.Data("FilterFowst")) = 1 Then
                    pDeviceSerial = Common.WebRequest.Data("DeviceSerial")
                    If CStr2(pDeviceSerial).Trim() = "" Then
                        pDeviceSerial = WebSession.Instance.DeviceSerial
                        If String.IsNullOrWhiteSpace(pDeviceSerial) Then
                            pDeviceSerial = Common.WebRequest.Data("UUID")
                        End If
                    End If
                    .SqlFrom = "FROM dbo.LibraryOrganizationEmployee INNER JOIN " &
                          "dbo.LibraryOrganizationEmployeeFOWST ON dbo.LibraryOrganizationEmployee.PeopleID = dbo.LibraryOrganizationEmployeeFOWST.PeopleID INNER JOIN " &
                          "dbo.MobizLibraryOrganizationFOWST AS fowst ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID" & vbCrLf
                    If CStr2(pDeviceSerial).Trim() <> "" Then
                        .SqlFrom &= " JOIN LibraryDeviceFowst ON LibraryDeviceFowst.FowstID = fowst.ID" & vbCrLf &
                            " JOIN LibraryDevice ON LibraryDevice.ID = LibraryDeviceFowst.DeviceID" & vbCrLf &
                            " AND LibraryDevice.UUID = " & ReadyString(pDeviceSerial) & vbCrLf
                    End If
                    .SqlFrom &= " LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " &
                        "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID"
                    .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) & " AND dbo.LibraryOrganizationEmployee.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & " AND dbo.LibraryOrganizationEmployee.Deactivated = 0" & vbCrLf &
                                " AND (fowst.FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'" & vbCrLf &
                                "   OR fowst.FOWSTTypeID IS NULL)"
                    If CStr2(pDeviceSerial).Trim() <> "" Then
                        .SqlWhere &= "    AND LibraryDeviceFowst.IsActive = 1" & vbCrLf
                    End If
                Else
                    .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
                        " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                        " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)"

                    If CBool2(WebRequest.Instance.Data("AllowFranchiseeFOWST")) Then
                        .SqlWhere = " WHERE ISNULL(fowst.IsActive, 1) = 1" & vbCrLf &
                                " AND (fowst.FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'" & vbCrLf &
                                "   OR fowst.FOWSTTypeID IS NULL)"
                    Else
                        .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) & vbCrLf &
                                " AND ISNULL(fowst.IsActive, 1) = 1" & vbCrLf &
                                " AND (fowst.FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'" & vbCrLf &
                                "   OR fowst.FOWSTTypeID IS NULL)"
                    End If

                End If

                If String.IsNullOrWhiteSpace(pFowstTypeID) = False Then
                    .SqlWhere &= " AND fowst.FOWSTTypeID = " & ReadyString(pFowstTypeID) & vbCrLf
                End If

                If CBool2(WebRequest.Instance.Data("IsAll")) Then
                    .MaxRecords = 0
                End If

                If CBool2(WebRequest.Instance.Data("IncludeInactiveStore")) Then
                    .SqlWhere &= " OR ISNULL(fowst.IsActive, 1) = 0 " & vbCrLf
                End If

                If CBool2(WebRequest.Instance.Data("IsByEmployeeFOWST")) Then
                    .SqlWhere &= " AND fowst.ID IN(Select DISTINCT FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & " )"
                End If

                If Not String.IsNullOrWhiteSpace(CStr2(WebRequest.Instance.Data("Mall"))) Then
                    .SqlWhere &= " AND fowst.FOWSTMallName = " & ReadyString(WebRequest.Instance.Data("Mall"))
                End If

                .SqlOrder = "ORDER BY FowstName, Address"
            End With

            Dim pDT As DataTable
            Dim pSql As String

            pSql = mFinder.CreateSql()
            If CBool2(Common.WebRequest.Data("HideNoAddress")) Then
                pSql = "SELECT * FROM (" & pSql & ") t" & vbCrLf &
                    " WHERE LEN(LTRIM(RTRIM(ISNULL(Address, '')))) > 0"
            End If
            pDT = mFinder.ProData.Execute(pSql)

            If pIncludeAllFowstOptions Then
                Dim pDR = pDT.NewRow()
                pDR("FowstName") = "[All FOWSTs]"
                pDT.Rows.InsertAt(pDR, 0)
            End If

            Return JsonDictionary.Serialize("Results", pDT)

            Return JsonDictionary.Serialize("Results", pDT)
        Catch ex As ProSqlException
            AppendToDebug("ref: vbBPKQGPH2NR!" & vbCrLf & ex.Message & vbCrLf & ex.Sql, , ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        Catch ex As Exception
            AppendToDebug("ref: vbZGFM6L95ZE!" & vbCrLf & ex.Message, , ex, True)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function LibraryFowstAll() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .Distinct = True
            .MaxRecords = IIf(CBool2(Common.WebRequest.Data("IsAll")), 0, 50)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.OrganizationID"
                    .Searchable = False
                End With
                With .Add("FowstOrgName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
                With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                    .Expression = "fowst.IsFOWSTInsideAMall"
                End With
                With .Add("FOWSTMallName", SqlDbType.VarChar)
                    .Expression = "fowst.FOWSTMallName"
                End With
                With .Add("IsVat", SqlDbType.Bit)
                    .Expression = "ISNULL(fowst.IsVat, 1)"
                End With
                With .Add("FowstOrgOwner", SqlDbType.VarChar)
                    .Expression = " (SELECT STUFF(" & vbCrLf &
                        " 	(SELECT '; ' + " & BrowserController.ComposePeopleName() & vbCrLf &
                        "     FROM MobizLibraryOrganizationOwner" & vbCrLf &
                        " 	JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = MobizLibraryOrganizationOwner.PeopleID" & vbCrLf &
                        " 	LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                        "     WHERE MobizLibraryOrganizationOwner.OrganizationID = fowst.OrganizationID" & vbCrLf &
                        " 	FOR XML PATH('')" & vbCrLf &
                        "     ), 1, 2, '')" & vbCrLf &
                        " )"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
            " INNER JOIN MobizLibraryOrganization org ON (fowst.OrganizationID = org.ID) " &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID) " &
            " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
            " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) "

            If CInt(WebRequest.Instance.Data("IsSubscriber")) = 1 Then
                .SqlFrom &= " INNER JOIN dbo.MobizLibrarySubscription sub ON (org.ID = sub.OrganizationID) "
            End If

            If IsGuid(Common.WebRequest.Data("FowstID")) Then
                .SqlWhere = " WHERE fowst.ID = " & ReadyString(Common.WebRequest.Data("FowstID"))
            End If

            .SqlOrder = " ORDER BY fowst.Name ASC "

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowstPrinter() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fwostprt.ID"
                    .Searchable = False
                End With
                With .Add("PrinterIPAddress", SqlDbType.VarChar)
                    .Expression = "fwostprt.PrinterIPAddress"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWSTPrinter fwostprt "

            .SqlWhere = " WHERE fwostprt.FowstID = " & ReadyString(WebRequest.Instance.FowstID)

            .SqlOrder = "ORDER BY fwostprt.PrinterIPAddress ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryBanks() As String

        'todo: add security
        With CreateFinder()
            If CInt(WebRequest.Instance.Data("IsAll")) = 1 Then
                .MaxRecords = 0
            End If

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("BankCode", SqlDbType.VarChar)
                .Add("BankName", SqlDbType.VarChar)
            End With
            .SqlFrom = " FROM MobizLibraryBanks "
            .SqlWhere = " WHERE IsActive = 1 "
            .SqlOrder = " ORDER BY BankName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryBankBranches() As String

        Dim pBankID As String

        pBankID = Common.WebRequest.Data("BankID")

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "bnkbr.ID"
                    .Searchable = False
                End With
                With .Add("BranchName", SqlDbType.VarChar)
                    .Expression = "bnkbr.BranchName"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "bnkbr.Address"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryBankBranch bnkbr "
            .SqlWhere = " WHERE " & IIf(pBankID.Trim().Length = 0, " bnkbr.BankID IS NULL ", " bnkbr.BankID = " & ReadyString(pBankID))
            .SqlOrder = "ORDER BY bnkbr.BranchName ASC "

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCountries(Optional ByVal IsAll As Boolean = False) As String

        'todo: add security
        With CreateFinder()
            If CInt(WebRequest.Instance.Data("IsAll")) = 1 Or IsAll Then
                .MaxRecords = 0
            End If

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Code", SqlDbType.VarChar)
                .Add("Name", SqlDbType.VarChar)
                .Add("CallingCode", SqlDbType.VarChar)
                With .Add("NoOfDigits", SqlDbType.VarChar)
                    .Searchable = False
                End With
            End With
            .SqlFrom = "FROM MobizLibraryCountries"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY Code"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCountryLanguage() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylng.ID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylng.CountryID"
                    .Searchable = False
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylng.Language"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryCountriesLanguage ctrylng " &
                    " INNER JOIN MobizLibraryCountries ctry ON (ctrylng.CountryID = ctry.ID)"
            .SqlOrder = "ORDER BY ctrylng.Language ASC," &
                    " ctry.[Name] ASC"
        End With

        Return SubmitFinder()

    End Function

    Public Function LibraryCountryLanguageVocabularyLabel() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "voc.ID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylng.CountryID"
                    .Searchable = False
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("LanguageID", SqlDbType.UniqueIdentifier)
                    .Expression = "voc.LanguageID"
                    .Searchable = False
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylng.Language"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "voc.Label"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryCountriesLanguageLabel voc " &
                    " INNER JOIN MobizLibraryCountriesLanguage ctrylng ON (voc.LanguageID = ctrylng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (ctrylng.CountryID = ctry.ID)"

            .SqlOrder = "ORDER BY voc.Label ASC"

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMPriceLevel() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prclvl.ID"
                    .Searchable = False
                End With
                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "prclvl.PriceLevel"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMPriceLevel prclvl "

            .SqlWhere = " WHERE (prclvl.IsActive = 1 AND prclvl.OrgID IS NULL OR prclvl.OrgID = " & ReadyString(WebRequest.Instance.OrgID) & ") "

            .SqlOrder = "ORDER BY prclvl.PriceLevel ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTPriceLevel() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prclvl.ID"
                    .Searchable = False
                End With
                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "prclvl.PriceLevel"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWSTPriceLevel as fowst" & vbCrLf &
                        "INNER JOIN MobizLibraryPIMPriceLevel as prclvl ON fowst.PriceLevelID = prclvl.ID"

            .SqlWhere = " WHERE (prclvl.IsActive = 1 AND prclvl.OrgID IS NULL OR prclvl.OrgID = " & ReadyString(WebRequest.Instance.OrgID) & ") " & vbCrLf &
                        " AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID"))

            .SqlOrder = "ORDER BY prclvl.PriceLevel ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMFeature() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim keywords As Object = Split(Common.WebRequest.Data("keywords"), " ")
        Dim whereSearch As String = ""

        WebRequest.Instance.Business.LibraryPIMFeature.AddRequiredFeatures()
        If Trim(Common.WebRequest.Data("keywords")) <> "" Then
            For i As Integer = 0 To keywords.length - 1
                If i <> 0 Then
                    whereSearch = whereSearch & " AND " & vbCrLf
                End If

                whereSearch = whereSearch & "(ISNULL(FeatureLabel, '') + ' ' + ISNULL(Feature, '') + ' ' + ISNULL(FeatureMeasure, '') + ' ' + ISNULL(FeatureMeasureSign, '') + ' ' + ISNULL(FeatureType, '') + ' ' + ISNULL(DisplayFormat, ''))" & vbCrLf &
                                " LIKE " & ReadyStringWild(keywords(i))
            Next
        End If

        pSql = "SELECT * FROM (" & vbCrLf &
                    "SELECT ftr.ID AS [ID] " & vbCrLf &
                    ", convert(varchar(max), ftr.FeatureLabelID) AS [FeatureLabelID] " & vbCrLf &
                    ", lbl.Label AS [FeatureLabel] " & vbCrLf &
                    ", convert(varchar(max), ftr.FeatureNameID) AS [FeatureNameID] " & vbCrLf &
                    ", ftrname.Label AS [Feature] " & vbCrLf &
                    ", convert(varchar(max), ftr.FeatureMeasureID) AS [FeatureMeasureID] " & vbCrLf &
                    ", msrname.Label AS [FeatureMeasure] " & vbCrLf &
                    ", msrsgn.Sign AS [FeatureMeasureSign] " & vbCrLf &
                    ", ftr.FeatureType AS [FeatureType] " & vbCrLf &
                    ", ftr.DisplayFormat AS [DisplayFormat] " & vbCrLf &
                    "   FROM MobizLibraryPIMFeature ftr " & vbCrLf &
                    "       INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (ftr.FeatureLabelID = lbl.ID) " & vbCrLf &
                    "       INNER JOIN MobizLibraryCountriesLanguageLabel ftrname ON (ftr.FeatureNameID = ftrname.ID) " & vbCrLf &
                    "       LEFT JOIN MobizLibraryPIMMeasure msr ON (ftr.FeatureMeasureID = msr.ID) " & vbCrLf &
                    "       LEFT JOIN MobizLibraryCountriesLanguageLabel msrname ON (msr.MeasureNameID = msrname.ID) " & vbCrLf &
                    "       LEFT JOIN MobizLibraryPIMMeasureSign msrsgn ON (msr.SignID = msrsgn.ID) " & vbCrLf &
                    "UNION " & vbCrLf &
                    "SELECT lvl.ID as ID, " & vbCrLf &
                    "NULL as FeatureLabelID, " & vbCrLf &
                    "PriceLevel as FeatureLable, " & vbCrLf &
                    "NULL as FeatureNameID, " & vbCrLf &
                    "PriceLevel as Feature, " & vbCrLf &
                    "NULL as FeatureMeasureID, " & vbCrLf &
                    "NULL as FeatureMeasure, " & vbCrLf &
                    "NULL as FeatureMeasureSign, " & vbCrLf &
                    "'Textbox (Decimal)' as FeatureType, " & vbCrLf &
                    "NULL as DisplayFormat " & vbCrLf &
                    "   FROM MobizLibraryPIMPriceLevel as lvl " & vbCrLf &
                    "       LEFT JOIN MobizLibraryPeople as ppol ON lvl.UserID = ppol.ID " & vbCrLf &
                    "       WHERE (lvl.IsActive = 1 AND lvl.OrgID IS NULL OR lvl.OrgID = " & ReadyString(WebRequest.Instance.OrgID) & ")" & vbCrLf &
                    "           AND lvl.ID NOT IN ('01000012-0001-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000', '01000012-0003-0000-0000-000000000000', '01000012-0004-0000-0000-000000000000', '01000012-0005-0000-0000-000000000000', 'eef716c6-b290-4b28-b2a6-0a4f52cf4989') " & vbCrLf &
                ") as lvl " & vbCrLf &
                If(whereSearch = "", "", "WHERE " & whereSearch) & vbCrLf &
                "ORDER BY FeatureLabel" & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)
        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.ID"
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
                    .Expression = "ftrname.Label"
                End With
                With .Add("FeatureMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureMeasureID"
                    .Searchable = False
                End With
                With .Add("FeatureMeasure", SqlDbType.VarChar)
                    .Expression = "msrname.Label"
                End With
                With .Add("FeatureMeasureSign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "ftr.FeatureType"
                End With
                With .Add("DisplayFormat", SqlDbType.VarChar)
                    .Expression = "ftr.DisplayFormat"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMFeature ftr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (ftr.FeatureLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel ftrname ON (ftr.FeatureNameID = ftrname.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasure msr ON (ftr.FeatureMeasureID = msr.ID) " &
                    " LEFT JOIN MobizLibraryCountriesLanguageLabel msrname ON (msr.MeasureNameID = msrname.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign msrsgn ON (msr.SignID = msrsgn.ID) "

            .SqlOrder = "ORDER BY ftrname.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMProductFamily() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ID"
                End With
                With .Add("ProductFamilyVocLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ProductFamilyVocLabelID"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "lng.Language"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMProductFamily pfam " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (pfam.ProductFamilyVocLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguage lng ON (lbl.LanguageID = lng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (lng.CountryID = ctry.ID) "

            .SqlOrder = "ORDER BY lbl.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryUNSPSCCommodity() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncom.ID"
                End With
                With .Add("CommodityCode", SqlDbType.VarChar)
                    .Expression = "uncom.CommodityCode"
                End With
                With .Add("Commodity", SqlDbType.VarChar)
                    .Expression = "uncom.Commodity"
                End With
                With .Add("ClassID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncom.ClassID"
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
            End With

            .SqlFrom = "FROM MobizLibraryUNSPSCCommodity uncom " &
                    " INNER JOIN MobizLibraryUNSPSCClass uncls ON (uncom.ClassID = uncls.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCFamily unfmly ON (uncls.FamilyID = unfmly.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCSegment unseg ON (unfmly.SegmentID = unseg.ID)"

            .SqlOrder = "ORDER BY uncom.Commodity ASC, " &
                    "uncls.Class ASC," &
                    "unfmly.Family ASC," &
                    "unseg.Segment ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasureSign() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msrsgn.ID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMMeasureSign msrsgn "
            .SqlOrder = "ORDER BY msrsgn.Sign ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasure() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"
            .SqlOrder = "ORDER BY lbl.Label ASC"

            If CBool2(WebRequest.Instance.Data("IsAll")) Then
                .MaxRecords = 0
            End If
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasureWeight() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

            .SqlWhere = " WHERE msr.IsStandardWeight = 1"

            .SqlOrder = "ORDER BY lbl.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasureLength() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

            .SqlWhere = " WHERE msr.IsStandardLength = 1"

            .SqlOrder = "ORDER BY lbl.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMProductMeasure(Optional ByVal ID As String = "") As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.ID"
                End With
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.UnitMeasureID"
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')"
                End With
                With .Add("Quantity", SqlDbType.Decimal)
                    .Expression = "prdmea.Quantity"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProductMeasurement prdmea " &
                    " INNER JOIN MobizLibraryPIMMeasure msr ON (prdmea.UnitMeasureID = msr.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

            .SqlWhere = " WHERE prdmea.ProductID = " & ReadyString(IIf(CStr2(ID) = "", WebRequest.Instance.Data("ProductID"), ID))

            .SqlOrder = " ORDER BY prdmea.Quantity ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryAreaCodes() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Area", SqlDbType.VarChar)
                .Add("CallingCode", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryAreaCodes"
            .SqlOrder = "ORDER BY Area"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryTelcoPrefixes(Optional ByVal IsAll As Boolean = False) As String

        'todo: add security
        With CreateFinder()
            If IsAll Then
                .MaxRecords = 0
            End If
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prf.ID"
                    .Searchable = False
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "prf.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryTelcoPrefixes prf " &
                    " INNER JOIN MobizLibraryTelcos telco ON (prf.TelcoId = telco.Id)"
            .SqlOrder = "ORDER BY prf.Prefix"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryLocation() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Location", SqlDbType.VarChar)
                    .Expression = "LocationName"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryLocations "
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY LocationName "
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibrarySecurityRole() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'Temporary for BIR demo only, dili ipakita ang Super Admin Access if lain user ang naka login
        'Only Mobiz people lang ang pwede maka add og "Super Admin" access role
        'by Arnie 3112026
        Dim isadmin As Integer = Common.WebRequest.ProData.Execute("SELECT * FROM LibrarySecurityRoleUser WHERE RoleID = '474f9f49-2876-4391-a005-107eb18f2762' AND PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)).Rows.Count

        If WebRequest.Instance.OrgID.ToLower() <> "565c260c-3844-4069-ac41-d26f0598e562" Then
            'walay labot ang dili BIR DB OrgID
            isadmin = 0
        End If
        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "Code"
                End With
                With .Add("Role", SqlDbType.VarChar)
                    .Expression = "Role"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "Description"
                End With
            End With
            .SqlFrom = "FROM LibrarySecurityRole "
            .SqlWhere = "WHERE IsActive = 1" & IIf(isadmin > 0, "", " AND ID <> '474f9f49-2876-4391-a005-107eb18f2762' ")
            .SqlOrder = "ORDER BY Role "
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCities(Optional ByVal IsAll As Boolean = False) As String

        'todo: add security
        With CreateFinder()
            If (Common.WebRequest.Data("IsAll") <> Nothing And CBool2(Common.WebRequest.Data("IsAll")) Or IsAll) Then
                .MaxRecords = 0
            End If
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryCities"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY Name"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCitizenship() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Citizenship", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryCitizenship"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY Citizenship"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost>
    Public Function LibraryOrganizationAndPerson() As String
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ISNULL(CompanyName, '') + ' ' + ISNULL(PersonFullName, '') + ' ' + ISNULL(PersonFirstName, '') + ' ' + ISNULL(PersonMiddleName, '') + ' ' + ISNULL(PersonSurname, '')"
                End With
                .Add("IsOrganization", SqlDbType.Bit)
                With .Add("CreatedByOrgID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
            End With
            .SqlFrom = "FROM MobizLibraryOrganization"
            .SqlWhere = "WHERE ISNULL(convert(varchar(max), OriginalKeyID), '') = '' " & vbCrLf &
                        " AND ISNULL(CompanyName, '') + ' ' + ISNULL(PersonFullName, '') + ' ' + ISNULL(PersonFirstName, '') + ' ' + ISNULL(PersonMiddleName, '') + ' ' + ISNULL(PersonSurname, '') <> '' "

            If Common.WebRequest.Data("FilterBy") = "Org" Then
                .SqlWhere += " AND IsOrganization = 1"
            ElseIf Common.WebRequest.Data("FilterBy") = "Person" Then
                .SqlWhere += " AND IsOrganization = 0"
            End If

            .SqlOrder = "ORDER BY CompanyName"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryOrganization() As String

        Dim pFilterCustomers As Boolean
        Dim pFilterTypePerson As Boolean
        Dim pFilterFranchisor As Boolean
        Dim pFilterSupplier As Boolean

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pFilterCustomers = CDec2(Common.WebRequest.Data("FilterCustomers"))
        If pFilterCustomers Then
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End If
        pFilterTypePerson = CDec2(Common.WebRequest.Data("FilterTypePerson"))
        pFilterFranchisor = CDec2(Common.WebRequest.Data("FilterFranchisor"))
        pFilterSupplier = CBool2(Common.WebRequest.Data("FilterSupplier"))
        If WebRequest.Instance.HandleOrganization(True) = False Then
            If pFilterSupplier Then
                Return WebRequest.InvalidOrganization
            End If
        End If

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "org.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("TinNumber", SqlDbType.VarChar)
                    .Expression = "org.TinNumber"
                End With
                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "org.PersonFirstName"
                    .Searchable = False
                End With
                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "org.PersonMiddleName"
                    .Searchable = False
                End With
                With .Add("Surname", SqlDbType.VarChar)
                    .Expression = "org.PersonSurname"
                    .Searchable = False
                End With
                With .Add("IsOrganization", SqlDbType.VarChar)
                    .Expression = "org.IsOrganization"
                    .Searchable = False
                End With
                With .Add("PersonSeniorCitizensIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonSeniorCitizensIDNo"
                End With
                With .Add("PersonPWDIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPWDIDNo"
                End With
                With .Add("PersonPassportNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPassportNo"
                End With
                With .Add("PersonDriversLicenseNo", SqlDbType.VarChar)
                    .Expression = "org.PersonDriversLicenseNo"
                End With
                With .Add("PersonSSSNo", SqlDbType.VarChar)
                    .Expression = "org.PersonSSSNo"
                End With
                With .Add("PersonGSISIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonGSISIDNo"
                End With
                With .Add("PersonPRCIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPRCIDNo"
                End With
                With .Add("PersonPostalIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPostalIDNo"
                End With
                With .Add("PersonVotersIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonVotersIDNo"
                End With
                With .Add("PersonOtherIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonOtherIDNo"
                End With
                With .Add("CustomerRequirePoNum", SqlDbType.Bit)
                    .Searchable = False
                    .Expression = "LibraryOrganizationCustomer.CustomerRequirePoNum"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization org " &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)"
            If pFilterCustomers = False Then
                .SqlFrom &= " LEFT "
            End If
            .SqlFrom &= " JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = org.ID" & vbCrLf
            If pFilterCustomers Then
                With .Fields.Add("CreditTerms", SqlDbType.Int)
                    .Expression = "LibraryPaymentTerms.Days"
                End With
                .SqlFrom &= " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationCustomer.CreditTermsID" & vbCrLf

                With .Fields.Add("SalesRepID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryOrganizationCustomer.SalesRepID"
                End With
                With .Fields.Add("SalesRepName", SqlDbType.VarChar)
                    .Expression = "ISNULL(peopleSalesRep.Surname + ', ', '') + ISNULL(peopleSalesRep.FirstName + ' ', '') + ISNULL(peopleSalesRep.MiddleName + ' ', '')"
                End With
                .SqlFrom &= " LEFT JOIN MobizLibraryPeople peopleSalesRep ON peopleSalesRep.ID = LibraryOrganizationCustomer.SalesRepID" & vbCrLf
            End If

            If CDec2(Common.WebRequest.Data("FilterPeople")) <> 0 Then
                .SqlFrom &= " JOIN MobizLibraryPeopleOrganization ON MobizLibraryPeopleOrganization.OrganizationID = org.ID" & vbCrLf &
                    " AND MobizLibraryPeopleOrganization.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                    " AND MobizLibraryPeopleOrganization.IsConfirmed = 1" & vbCrLf &
                    " AND MobizLibraryPeopleOrganization.IsTerminated = 0" & vbCrLf

            End If
            If pFilterSupplier Then
                With .Fields
                    With .Add("CreditTermsID", SqlDbType.UniqueIdentifier)
                        .Expression = "LibraryOrganizationSupplier.CreditTermsID"
                    End With
                    With .Add("CreditTerms", SqlDbType.Int)
                        .Expression = "LibraryPaymentTerms.Days"
                    End With
                    With .Add("CreditTermsName", SqlDbType.VarChar)
                        .Expression = "LibraryPaymentTerms.Name"
                    End With
                    With .Add("CreditLimit", SqlDbType.Money)
                        .Expression = "LibraryOrganizationSupplier.CreditLimit"
                    End With
                    With .Add("CreditBegBalance", SqlDbType.Money)
                        .Expression = "LibraryOrganizationSupplier.CreditBegBalance"
                    End With
                    With .Add("CreditBalanceAsOf", SqlDbType.Date)
                        .Expression = "LibraryOrganizationSupplier.CreditBalanceAsOf"
                    End With
                End With
                .SqlFrom &= " JOIN LibraryOrganizationSupplier ON LibraryOrganizationSupplier.SupplierID = org.ID" & vbCrLf &
                    " AND ISNULL(LibraryOrganizationSupplier.IsBan, 0) = 0" & vbCrLf &
                    " AND ISNULL(LibraryOrganizationSupplier.Deactivated, 0) = 0" & vbCrLf &
                    " LEFT JOIN LibraryPaymentTerms  ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf
            End If
            'additional by janine sept 12 2014 for franchisor
            If pFilterFranchisor Then
                .SqlFrom &= " JOIN MobizLibrarySubscription ON MobizLibrarySubscription.OrganizationID = org.ID "
            End If

            'pag ang OriginalKeyID kay dili empty it means copy data from existing record, so dapat dili nato iapil ang mga copied data -- arnie 060718
            .SqlWhere = " WHERE org.IsActive = 1 " & vbCrLf &
                " AND LEN(RTRIM(LTRIM(ISNULL(" & .Fields("CompanyName").Expression & ", '')))) > 0" & vbCrLf
            '.SqlWhere = " WHERE org.IsActive = 1" & vbCrLf

            'additional by janine july 26 for senior citizen customers
            If pFilterTypePerson Then
                .SqlWhere &= " AND ISNULL(convert(nvarchar(50), OriginalKeyID), '') = '' AND org.IsOrganization = 0 " & vbCrLf
            End If
            'additional by janine sept 12 2014 for franchisor
            If pFilterFranchisor Then
                .SqlWhere &= " AND ISNULL(convert(nvarchar(50), OriginalKeyID), '') = '' AND org.IsFranchisor = 1 " & vbCrLf
            End If

            If pFilterSupplier Then
                .SqlWhere &= " AND ISNULL(TaggedAs, '') = '' " 'TaggedAs Column use for dtr (manpower, co-user modules), pag null ang value kay supplier
            End If
            .SqlOrder = "ORDER BY (CASE org.IsOrganization " &
                        "     WHEN 1 THEN  org.CompanyName " &
                        "     ELSE  " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                        " END)"
        End With

        Dim pDT As DataTable
        Dim pOut As JsonDictionary
        Dim pDefaultCustomerOrganizationID As String

        pOut = New JsonDictionary
        Dim pSql As String
        pSql = mFinder.CreateSql()
        'pSql = "SELECT * FROM (" & pSql & ")t WHERE LEN(RTRIM(LTRIM(ISNULL(t.Companyname, '')))) > 0"
        pDT = mFinder.ProData.Execute(pSql)
        pOut.Add("Results", pDT)
        If pFilterCustomers Then
            pDefaultCustomerOrganizationID = WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID
            If pDefaultCustomerOrganizationID Is Nothing Then
                pOut.Add("OfferAsDefault", 1)
            End If
        End If
        Return pOut.Serialize

    End Function

    <HttpPost>
    Public Function LibraryOrganizationCustomer() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .MaxRecords = 0
            With .Fields
                With .Add("DbID", SqlDbType.UniqueIdentifier)
                    .Expression = "cus.ID"
                    .Searchable = False
                End With
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                    .Searchable = False
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = "CASE WHEN org.CompanyName = '' THEN ISNULL(org.PersonFullName + ' ', '') + ISNULL(org.PersonSurname + ', ', '')  + ISNULL(org.PersonFirstName + ' ', '') + ISNULL(org.PersonMiddleName, '') ELSE org.CompanyName END"
                End With
                With .Add("IsOrganization", SqlDbType.VarChar)
                    .Expression = "org.IsOrganization"
                    .Searchable = False
                End With

                If Common.WebRequest.Data("FilterBy") = "Person" Then
                    .Add("PersonFullName", SqlDbType.VarChar)
                    .Add("PersonFirstName", SqlDbType.VarChar)
                    .Add("PersonMiddleName", SqlDbType.VarChar)
                    .Add("PersonSurname", SqlDbType.VarChar)
                    With .Add("PersonDateOfBirth", SqlDbType.Date)
                        .Expression = "org.PersonDateOfBirth"
                        .Searchable = False
                    End With
                End If

                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "cus.LastUpdated"
                    .Searchable = False
                End With
            End With
            .SqlFrom = "FROM LibraryOrganizationCustomer as cus " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganization as org ON org.ID = cus.OrganizationID  " & vbCrLf

            .SqlWhere = " WHERE cus.Deactivated = 0 "

            If Common.WebRequest.Data("FilterBy") = "Org" Then
                .SqlWhere += " AND org.IsOrganization = 1 "
            ElseIf Common.WebRequest.Data("FilterBy") = "Person" Then
                .SqlWhere += " AND org.IsOrganization = 0 "
            End If

            .SqlOrder = "ORDER BY org.CompanyName, org.PersonFullName, org.PersonSurname ASC"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost>
    Public Function LibraryOrganizationSupplier() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .MaxRecords = 0
            With .Fields
                With .Add("DbID", SqlDbType.UniqueIdentifier)
                    .Expression = "sup.ID"
                    .Searchable = False
                End With
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                    .Searchable = False
                End With
                With .Add("SupplierName", SqlDbType.VarChar)
                    .Expression = "CASE WHEN org.CompanyName = '' THEN org.PersonSurname + ', ' + org.PersonFirstName + ' ' + org.PersonMiddleName ELSE org.CompanyName END"
                End With
                With .Add("IsOrganization", SqlDbType.VarChar)
                    .Expression = "org.IsOrganization"
                    .Searchable = False
                End With

                If Common.WebRequest.Data("FilterBy") = "Person" Then
                    .Add("PersonFirstName", SqlDbType.VarChar)
                    .Add("PersonMiddleName", SqlDbType.VarChar)
                    .Add("PersonSurname", SqlDbType.VarChar)
                    With .Add("PersonDateOfBirth", SqlDbType.Date)
                        .Expression = "org.PersonDateOfBirth"
                        .Searchable = False
                    End With
                End If

                With .Add("CreditTermsInDays", SqlDbType.VarChar)
                    .Expression = "(SELECT Days FROM LibraryPaymentTerms WHERE ID = sup.CreditTermsID)"
                    .Searchable = False
                End With
            End With
            .SqlFrom = "FROM LibraryOrganizationSupplier as sup " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganization as org ON org.ID = sup.SupplierID  " & vbCrLf

            .SqlWhere = " WHERE sup.Deactivated = 0 AND org.IsOrganization = 1 " & vbCrLf &
                        "AND ISNULL(sup.TaggedAs, '') = '' -- dapat blank ni kay para lang ni sa DTR " & vbCrLf

            .SqlOrder = "ORDER BY org.CompanyName, org.PersonSurname ASC"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost>
    Public Function LibraryOrganizationCustomerAndSupplier()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String

        pSql = " SELECT DISTINCT ID, Name as CompanyName, IsOrganization FROM ( " & vbCrLf &
                "   SELECT org.ID, " & vbCrLf &
                "   CASE WHEN org.CompanyName = '' THEN org.PersonFirstName + ' ' + org.PersonMiddleName + ' ' + org.PersonSurname ELSE org.CompanyName END as Name, " & vbCrLf &
                "   org.IsOrganization, " & vbCrLf &
                "   'Customer' as Type " & vbCrLf &
                "   FROM LibraryOrganizationCustomer as cusorg " & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganization as org ON cusorg.OrganizationID = org.ID " & vbCrLf &
                "   WHERE cusorg.Deactivated = '0' AND ISNULL(convert(varchar(max), org.ID), '') <> '' " & vbCrLf &
                "   UNION ALL " & vbCrLf &
                "   SELECT org.ID, " & vbCrLf &
                "   CASE WHEN org.CompanyName = '' THEN org.PersonFirstName + ' ' + org.PersonMiddleName + ' ' + org.PersonSurname ELSE org.CompanyName END as Name, " & vbCrLf &
                "   org.IsOrganization, " & vbCrLf &
                "   'Supplier' as Type " & vbCrLf &
                "   FROM LibraryOrganizationSupplier as suporg " & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganization as org ON suporg.SupplierID = org.ID " & vbCrLf &
                "   WHERE suporg.Deactivated = '0' AND ISNULL(convert(varchar(max), org.ID), '') <> '' " & vbCrLf &
                ") as T WHERE Name IS NOT NULL" & vbCrLf

        If Common.WebRequest.Data("keywords") <> Nothing Then
            pSql += " AND Name LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords")) & vbCrLf
        End If

        pSql += " ORDER BY Name, ID asc "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    <HttpPost()>
    Public Function LibraryOrganizationBanks() As String

        Dim pFilterCustomers As Boolean

        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pFilterCustomers = CDec2(Common.WebRequest.Data("FilterCustomers"))
        If pFilterCustomers Then
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End If

        With CreateFinder()
            .MaxRecords = 1000
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "org.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization org " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)"

            .SqlWhere = "WHERE IsABank = 1"
            .SqlOrder = "ORDER BY "
            If CBool2(Common.WebRequest.Data("RankIfEmptySeach")) _
                AndAlso String.IsNullOrWhiteSpace(.SearchString) Then

                .SqlOrder &= " CASE ISNULL(org.BankRank,0)" & vbCrLf &
                    " WHEN 0 THEN 9999999" & vbCrLf &
                    " ELSE org.BankRank" & vbCrLf &
                    " END," & vbCrLf
            End If
            .SqlOrder &= " (CASE org.IsOrganization " &
                        "     WHEN 1 THEN  org.CompanyName " &
                        "     ELSE  " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                        " END)"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationTradeName() As String
        Dim pOrganizationID As String

        pOrganizationID = Common.WebRequest.Data("OrganizationID")

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("BusinessTradeName", SqlDbType.VarChar)
                .Add("BusinessTradeNameInitial", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryOrganizationTradeName "
            .SqlWhere = " WHERE " & IIf(pOrganizationID.Trim().Length = 0, "OrganizationID IS NULL ", "OrganizationID = " & ReadyString(pOrganizationID))
            .SqlOrder = "ORDER BY BusinessTradeName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationCategory() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cattyp.ID"
                    .Searchable = False
                End With
                With .Add("CategoryID", SqlDbType.UniqueIdentifier)
                    .Expression = "cattyp.CategoryID"
                    .Searchable = False
                End With
                With .Add("CategoryType", SqlDbType.VarChar)
                    .Expression = "cattyp.CategoryType"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "cat.Category"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrgType cattyp " &
                    " INNER JOIN MobizLibraryOrgCategory cat ON (cattyp.CategoryID = cat.ID)"

            .SqlOrder = "ORDER BY cat.Category ASC, cattyp.CategoryType ASC"

        End With

        Return SubmitFinder()

    End Function

    Public Function LibraryOrganizationReceiptSettings() As String
        Dim pOrganizationID As String

        pOrganizationID = Common.WebRequest.Data("OrganizationID")

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ret.ID"
                    .Searchable = False
                End With
                With .Add("OrgTradeNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "ret.OrgTradeNameID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "trade.BusinessTradeName"
                End With
            End With

            .SqlFrom = " FROM LibraryReceiptSettings ret " &
                " INNER JOIN MobizLibraryOrganizationTradeName trade ON (ret.OrgTradeNameID = trade.ID)"

            .SqlWhere = " WHERE " & IIf(pOrganizationID.Trim().Length = 0, " ret.OrganizationID IS NULL ", " ret.OrganizationID = " & ReadyString(pOrganizationID))
            .SqlOrder = "ORDER BY trade.BusinessTradeName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryJobOccupation() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Code", SqlDbType.VarChar)
                .Add("JobOccupation", SqlDbType.VarChar)
                .Add("Tags", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryJobOccupation"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY JobOccupation"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPeople() As String

        Dim pEmployeesOnly As Boolean

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        pEmployeesOnly = CDec2(Common.WebRequest.Data("EmployeesOnly"))
        If pEmployeesOnly Then
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End If

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                    .Searchable = False
                End With
                With .Add("PeopleFirstName", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.FirstName"
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With

                .Add("DateOfBirth", SqlDbType.Date)

                With .Add("MotherMaidenName", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.MotherMaidenSurname + ', ' + ppol.MotherMaidenFirstName"
                    .Searchable = False
                End With

            End With
            .SqlFrom = "FROM MobizLibraryPeople ppol " & vbCrLf
            If pEmployeesOnly Then
                .SqlFrom &= " JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = ppol.ID" & vbCrLf
            End If
            .SqlFrom &= " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)" & vbCrLf

            .SqlOrder = "ORDER BY (ISNULL(ppol.Surname,'') + ', ' + ISNULL(ppol.FirstName,'') + ' ' + ISNULL(ppol.MiddleName,'')) ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationContactPerson() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPeople.ID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = ComposePeopleName()
                End With
                With .Add("Position", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryPeople.Position, '')"
                End With

                With .Add("PositionDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryPeople.PositionDescription, '')"
                End With


            End With
            .SqlFrom = "FROM MobizLibraryOrganizationContactPerson  " & vbCrLf &
                " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = MobizLibraryOrganizationContactPerson.PeopleID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
            .SqlOrder = "ORDER BY " & ComposePeopleName()
            .SqlWhere = " WHERE MobizLibraryOrganizationContactPerson.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & vbCrLf &
                " AND ISNULL(MobizLibraryOrganizationContactPerson.IsUseForPersonOrgAffiliates, 0) = 0" & vbCrLf
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPeoplePreSuffix() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolpresuffix.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "ppolpresuffix.Code"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "ppolpresuffix.Description"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "ppolpresuffix.Category"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPeoplePreNominalSuffix ppolpresuffix "
            .SqlOrder = "ORDER BY ppolpresuffix.SortOrder ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPeoplePostSuffix() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolpostsuffix.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Code"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Description"
                End With
                With .Add("Type", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Type"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Country"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPeoplePostNominalSuffix ppolpostsuffix "
            .SqlOrder = "ORDER BY ppolpostsuffix.Code ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryEducation() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "educ.ID"
                    .Searchable = False
                End With
                With .Add("CourseCode", SqlDbType.VarChar)
                    .Expression = "educ.CourseCode"
                End With
                With .Add("CourseTitle", SqlDbType.VarChar)
                    .Expression = "educ.CourseTitle"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryEducationCourse educ "
            .SqlOrder = "ORDER BY educ.CourseTitle ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTType() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryFOWSTType"
            .SqlWhere = "WHERE IsActive = 1 AND ID <> 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B'" 'excluding Type PROJECT
            .SqlOrder = " ORDER BY SortOrder ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTWarehouseType() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryFOWSTWarehouseType"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = " ORDER BY Name ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTRetailStoreType() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryFOWSTRetailStoreType"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = " ORDER BY Name ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTStoreLocationType() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("LocationType", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryFOWSTStoreLocationType"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = " ORDER BY LocationType ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryIndustryClassification(Optional ByVal IsAll As Boolean = False) As String

        'todo: add security
        With CreateFinder()
            If IsAll Then
                .MaxRecords = 0
            End If

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "indstryclass.Code"
                End With
                With .Add("Class", SqlDbType.VarChar)
                    .Expression = "indstryclass.Class"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "indstryclass.Description"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryIndustryClass indstryclass" & vbCrLf

            If Common.WebRequest.Data("keywords") <> Nothing Then
                .SqlWhere = " WHERE indstryclass.Class LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords")) & vbCrLf
            End If
            .SqlOrder = "ORDER BY indstryclass.Class ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibrarySalesChannels() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Name", SqlDbType.VarChar)
                End With
            End With
            .SqlFrom = "FROM MobizLibrarySalesChannels" & vbCrLf
            .SearchString = Common.WebRequest.Data("keywords")

            .SqlOrder = "ORDER BY Name ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDevice() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("IsActive", SqlDbType.Int)
                    .Expression = "dev.IsActive"
                End With
                With .Add("LastUpdated", SqlDbType.Int)
                    .Expression = "dev.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With
            .SqlFrom = "FROM LibraryDevice dev " &
                        " LEFT JOIN dbo.MobizLibraryPeople ppol ON (dev.PeopleID = ppol.ID)"
            .SqlWhere = " WHERE dev.IsActive = 1 "

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"

            If CBool2(WebRequest.Instance.Data("IsAll")) Then
                .MaxRecords = 0
            End If
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDevicePrinter() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.ID"
                    .Searchable = False
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterName"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterType"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.FowstID"
                    .Searchable = False
                End With
                With .Add("Fowst", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("IPAddress", SqlDbType.VarChar)
                    .Expression = "prntr.IPAddress"
                End With
                With .Add("IsActive", SqlDbType.VarChar)
                    .Expression = "prntr.IsActive"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinter prntr " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = prntr.FowstID) "

            .SearchString = strKeyWord

            .SqlOrder = "ORDER BY prntr.PrinterName ASC"

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDeviceFowst() As String
        Dim strKeyWord As String
        Dim pFowstID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        pFowstID = Common.WebRequest.Data("FOWSTID")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.ID"
                    .Searchable = False
                End With
                With .Add("DeviceID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.DeviceID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.FowstID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("POSRegistrationNo", SqlDbType.VarChar)
                    .Expression = "devfowst.POSRegistrationNo"
                End With
                With .Add("TerminalNo", SqlDbType.VarChar)
                    .Expression = "devfowst.TerminalNo"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "CASE WHEN ISNULL(dev.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END"
                End With
            End With
            .SqlFrom = "FROM LibraryDeviceFowst devfowst " &
                " INNER JOIN LibraryDevice dev ON (devfowst.DeviceID = dev.ID) " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (devfowst.FowstID = fowst.ID) "

            .SqlWhere = " Where devfowst.FowstID = " & ReadyString(pFowstID) & vbCrLf

            If Not CBool2(Common.WebRequest.Data("IsAll")) Then
                .SqlWhere &= " AND ISNULL(dev.IsActive, 0) = 1"
            End If

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY ISNULL(dev.IsActive, 0) DESC, dev.PhoneName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDevicePolicyProfile() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dev.DevicePolicyProfileID"
                End With
                With .Add("DevicePolicyProfileName", SqlDbType.VarChar)
                    .Expression = "dev.DevicePolicyProfileName"
                End With
                With .Add("DevicePolicyDescription", SqlDbType.VarChar)
                    .Expression = "dev.DevicePolicyDescription"
                End With
            End With
            .SqlFrom = "FROM LibraryDevicePolicyProfile dev"
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.DevicePolicyProfileName ASC"
        End With

        Return SubmitFinder()

    End Function



    <HttpPost()>
    Public Function LibraryDeviceApproval() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
            End With
            .SqlFrom = "FROM LibraryDevice dev"
            .SqlWhere = "Where IsActive=0 "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
        End With

        Return SubmitFinder()

    End Function



    <HttpPost()>
    Public Function LibraryDevicePolicyAppoint() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
            End With
            .SqlFrom = "FROM LibraryDevice dev"
            .SqlWhere = "Where IsActive=1 "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
        End With
        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibrarySalesRep() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "Name"
                End With
            End With
            .SqlFrom = "FROM LibrarySalesRep"
            .SqlWhere = " WHERE ISNULL(LibrarySalesRep.IsActive, 1) = 1"
            .SqlOrder = "ORDER BY Name"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransItemRequisitions() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pFowstID = Common.WebRequest.Data("SelectedFowstID")
        Dim pProductID = Common.WebRequest.Data("ProductID")
        Dim pDateRequested As Date? = ParseDate(Common.WebRequest.Data("DateRequested"))
        Dim pDeactivated = CBool2(Common.WebRequest.Data("Deactivated"))
        Dim pPoFilter = CStr2(Common.WebRequest.Data("PoFilter")) ' po or all or else
        Dim pExpanded = CBool2(Common.WebRequest.Data("Expanded"))
        Dim pGroupBy As String
        Dim pCustomerSelectSql As String
        Dim pCustomerFromSql As String

        If pExpanded Then
            ComposeCustomer(pCustomerSelectSql, pCustomerFromSql, "tblCustomer")
        End If

        With CreateFinder()
            Dim pMaxRecords As Integer
            Dim pMaxRecordsText = Common.WebRequest.Data("MaxRecords")
            If pMaxRecordsText = "all" Then
                pMaxRecords = 0
            Else
                pMaxRecords = CInt2(pMaxRecordsText)
            End If

            .MaxRecords = pMaxRecords
            .ProData.AddColumn("TransItemRequisitions", "FowstID", "uniqueidentifier")
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransItemRequisitions.ID"
                    pGroupBy = .Expression
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransItemRequisitions.ProductID"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("VendorID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransItemRequisitions.VendorID"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("RequestedByID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransItemRequisitions.RequestedByID"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("CreatedPurchaseOrderID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransItemRequisitions.CreatedPurchaseOrderID"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransItemRequisitions.UserID"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPimProduct.ProductNo"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryPimProduct.Brand + ' ', '') + MobizLibraryPimProduct.ProductName"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "ISNULL(TransItemRequisitions.UnitMeasure, '')"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("Vendor", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganization.CompanyName"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("Remarks", SqlDbType.VarChar)
                    .Expression = "TransItemRequisitions.Remarks"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("RequesterName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName("tblRequester", "tblRequesterPeoplePreNominalSuffix")
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("EncoderName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName("tblEncoder", "tblEncoderPeoplePreNominalSuffix")
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("UnitQty", SqlDbType.Decimal)
                    .Expression = "ISNULL(TransItemRequisitions.UnitQty, 1)"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("RequestedQty", SqlDbType.Decimal)
                    .Expression = "TransItemRequisitions.RequestedQty"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("BackorderQty", SqlDbType.Decimal)
                    .Expression = "TransItemRequisitions.BackorderQty"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("RequestedQtyTotal", SqlDbType.Decimal)
                    .Expression = "TransItemRequisitions.RequestedQtyTotal"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("DateRequested", SqlDbType.DateTime2)
                    .Expression = "TransItemRequisitions.DateRequested"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("FullPurchaseOrderNumber", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + ISNULL(CONVERT(varchar, TransInvoice.PurchaseOrderNo), 0)"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("PurchaseOrderDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.PurchaseOrderDate"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("ComputedRequestedYearWeek", SqlDbType.VarChar)
                    .Expression = "TransItemRequisitions.ComputedRequestedYearWeek"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "tblRequestingFowst.Name"
                    pGroupBy &= ", " & .Expression & vbCrLf
                End With
                If pExpanded Then
                    With .Add("CustomerName", SqlDbType.VarChar)
                        .Expression = pCustomerSelectSql
                        pGroupBy &= ", " & .Expression & vbCrLf
                    End With
                    With .Add("BackOrderCustomerID", SqlDbType.UniqueIdentifier)
                        .Expression = "tblBackOrder.CustomerID"
                        pGroupBy &= ", " & .Expression & vbCrLf
                    End With
                    With .Add("BackOrderQty", SqlDbType.Decimal)
                        .Expression = "SUM(tblBackOrderItems.ActualQuantity * tblBackOrderItems.OrderedQuantity)"
                    End With
                End If
            End With
            .SqlFrom = " FROM TransItemRequisitions" & vbCrLf &
                " JOIN MobizLibraryPimProduct ON MobizLibraryPimProduct.ID = TransItemRequisitions.ProductID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransItemRequisitions.VendorID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeople tblRequester ON tblRequester.ID = TransItemRequisitions.RequestedByID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblRequesterPeoplePreNominalSuffix ON tblRequester.TitleID = tblRequesterPeoplePreNominalSuffix.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeople tblEncoder ON tblEncoder.ID = TransItemRequisitions.UserID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblEncoderPeoplePreNominalSuffix ON tblEncoder.TitleID = tblEncoderPeoplePreNominalSuffix.ID" & vbCrLf &
                "  LEFT JOIN TransInvoice ON TransInvoice.ID = TransItemRequisitions.CreatedPurchaseOrderID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganizationFOWST InvFowst ON TransInvoice.FowstID = InvFowst.ID" & vbCrLf &
                "  LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganizationFOWST tblRequestingFowst ON TransItemRequisitions.FowstID = tblRequestingFowst.ID" & vbCrLf
            If pExpanded Then
                .SqlFrom &= " LEFT JOIN TransItemRequisitionBackOrder ON TransItemRequisitionBackOrder.ItemRequisitionID = TransItemRequisitions.ID" & vbCrLf &
                    "   LEFT JOIN TransInvoice tblBackOrder ON tblBackOrder.ID = TransItemRequisitionBackOrder.BackOrderSourceID" & vbCrLf &
                    "   LEFT JOIN TransInvoiceItems tblBackOrderItems ON tblBackOrderItems.InvoiceID = tblBackOrder.ID" & vbCrLf &
                    "   AND tblBackOrderItems.ProductID = TransItemRequisitions.ProductID" & vbCrLf &
                    "   LEFT JOIN MobizLibraryOrganization tblCustomer ON tblCustomer.ID = tblBackOrder.CustomerID" & vbCrLf &
                    pCustomerFromSql
            End If
            .SqlWhere = " WHERE 1=1" & vbCrLf

            If String.IsNullOrWhiteSpace(pProductID) = False Then
                .SqlWhere &= " AND TransItemRequisitions.ProductID = " & ReadyString(pProductID) & vbCrLf
            End If

            .SqlWhere &= " AND ISNULL(TransItemRequisitions.IsDeactivated, 0) = " & If(pDeactivated, 1, 0) & vbCrLf

            If String.IsNullOrWhiteSpace(pFowstID) = False Then
                .SqlWhere &= " AND (TransItemRequisitions.FowstID = " & ReadyString(pFowstID) & vbCrLf &
                    " OR TransItemRequisitions.FowstID IS NULL)" & vbCrLf
            End If

            '.SqlWhere &= " AND CONVERT(date, TransItemRequisitions.DateRequested) = " & ReadyString(Format(pDateRequested, cGlobal.FormatDateTimeSql)) & vbCrLf
            Dim pWeeksAgoRequested = CInt2(Common.WebRequest.Data("WeeksAgoRequested"))
            Dim pYearWeekFrom = DateRange.GetYearWeek(Now, -pWeeksAgoRequested)

            .SqlWhere &= " AND TransItemRequisitions.ComputedRequestedYearWeek >= " & ReadyString(pYearWeekFrom) & vbCrLf

            If pPoFilter = "po" Then
                .SqlWhere &= " AND TransInvoice.ID IS NOT NULL" & vbCrLf
            ElseIf pPoFilter = "all" Then
                ' just dont filter here
            Else
                .SqlWhere &= " AND TransInvoice.ID IS NULL" & vbCrLf
            End If
            .SqlGroup = " GROUP BY " & pGroupBy
            .SqlOrder = "ORDER BY TransItemRequisitions.DateRequested"

            Dim pDT As DataTable = .Requery()

            If pExpanded Then
                Dim pParentIDs As New List(Of String)
                Dim pNewRows As New List(Of DataRow)
                Dim pSortOrder As Integer
                pDT.Columns.Add("SortOrder", GetType(Integer))
                For Each pDR As DataRow In pDT.Rows
                    If CDec2(pDR("BackOrderQty")) = 0 Then
                        pSortOrder += 1
                        pDR("SortOrder") = pSortOrder
                        Continue For
                    End If ' END If CDec2(pDR("BackOrderQty")) > 0 Then

                    If pParentIDs.Contains(ReadyGuid(pDR("ID"))) = False Then
                        pParentIDs.Add(ReadyGuid(pDR("ID")))
                        Dim pNewRow As DataRow = pDT.NewRow()
                        pNewRow.ItemArray = pDR.ItemArray.Clone()
                        pSortOrder += 1
                        pNewRow("SortOrder") = pSortOrder
                        pNewRow("BackOrderCustomerID") = DBNull.Value
                        pNewRow("CustomerName") = DBNull.Value
                        pNewRow("BackOrderQty") = 0
                        pNewRow("RequestedQtyTotal") = pNewRow("RequestedQty")
                        pNewRows.Add(pNewRow) ' cant add rows directly to the table when in a loop
                    End If

                    pSortOrder += 1
                    pDR("SortOrder") = pSortOrder
                    pDR("RequestedQty") = 0
                    pDR("RequestedQtyTotal") = pDR("BackOrderQty")
                Next ' END For Each pDR As DataRow In pDT.Rows

                For Each pDR In pNewRows
                    pDT.Rows.Add(pDR)
                Next
                pDT.DefaultView.Sort = "SortOrder"
                pDT = pDT.DefaultView.ToTable()
            End If ' END If pExpanded Then

            Return JsonDictionary.Serialize("Results", pDT)
        End With


    End Function

    <HttpPost()>
    Public Function TransLastestOrderedStockByCustomer() As String

        Dim pCustomerID As String
        Dim pDateRangeType As String
        Dim pWhere As String
        Dim pDateRange As DateRange
        Dim pDT As DataTable
        Dim pJson As JsonDictionary

        pCustomerID = Common.WebRequest.Data("CustomerID")
        If IsGuid(pCustomerID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid customer ID")
        End If

        pDateRangeType = Common.WebRequest.Data("DateRangeType")
        Select Case pDateRangeType
            Case "Within last week"
                Client.GetWithinLastWeek(pDateRange)

            Case "Within the last 2 weeks"
                Client.GetWithinLast2Weeks(pDateRange)

            Case "Within the last 4 weeks"
                Client.GetWithinLast4Weeks(pDateRange)

            Case "Within the last 3 months"
                Client.GetWithinLast3Months(pDateRange)

            Case "Within the last 6 months"
                Client.GetWithinLast6Months(pDateRange)

            Case "Within last year"
                Client.GetWithinLastYear(pDateRange)

            Case "Week to date"
                Client.GetWtdDates(pDateRange, Nothing)

            Case "Month to date"
                Client.GetMtdDates(pDateRange, Nothing)

            Case "Quarter to date"
                Client.GetQtdDates(pDateRange, Nothing)

            Case "Year to date"
                Client.GetYtdDates(pDateRange, Nothing)

            Case Else
                Return JsonDictionary.Serialize("Error", "Invalid DateRangeType: " & pDateRangeType)

        End Select
        pWhere = " AND DrpCacheStockSalesDailyByCustomer.TransDate BETWEEN " & ReadyString(pDateRange.DateFrom) & " AND " & ReadyString(pDateRange.DateTo)

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "LibraryStockInformation.ID"
                End With
                With .Add("StockNumber", SqlDbType.VarChar)
                    .Expression = "LibraryStockInformation.StockNumber"
                End With
                With .Add("StockDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)), '') +" & vbCrLf &
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,') +" & vbCrLf &
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,') +" & vbCrLf &
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,') +" & vbCrLf &
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', ')" & vbCrLf
                End With
                With .Add("TransDate", SqlDbType.DateTime2)
                    .Expression = "MAX(DrpCacheStockSalesDailyByCustomer.TransDate)"
                    .Searchable = False
                End With
                With .Add("Subtotal", SqlDbType.Money)
                    .Expression = "SUM(DrpCacheStockSalesDailyByCustomer.Subtotal)"
                End With
            End With
            .SqlFrom = "FROM DrpCacheStockSalesDailyByCustomer" & vbCrLf &
                " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockSalesDailyByCustomer.StockID" & vbCrLf &
                " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf &
                " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & vbCrLf
            .SqlGroup = "GROUP BY " & .Fields("ID").Expression & vbCrLf &
                "," & .Fields("StockNumber").Expression & vbCrLf &
                "," & .Fields("StockDescription").Expression
            .SqlWhere = "WHERE DrpCacheStockSalesDailyByCustomer.CustomerID = " & ReadyString(pCustomerID) & vbCrLf &
                pWhere
            .SqlOrder = "ORDER BY " & .Fields("TransDate").Expression & " DESC" & vbCrLf &
                "," & .Fields("StockDescription").Expression
        End With
        pDT = mFinder.Requery

        pJson = New JsonDictionary
        pJson.Add("Results", pDT)
        pJson.Add("DateRange", DisplayableDateRange(pDateRange))

        Return pJson.Serialize

    End Function

    <HttpPost()>
    Public Function LibraryPIMProduct(Optional vDontUseCacheProductSearch As Boolean = False, Optional vSearchBarcode As Boolean = False) As String

        Dim pSearchPhrase As String
        Dim pSql As String
        Dim pWhere As String
        Dim pSearchMore As Boolean
        Dim pVendorID = Common.WebRequest.Data("VendorID")
        Dim pUseCacheProductSearch = CBool2(Common.WebRequest.Data("UseCacheProductSearch"))
        If vDontUseCacheProductSearch Then
            pUseCacheProductSearch = False
        End If
        Dim pCacheProductSearch = CBool2(Common.WebRequest.Data("CacheProductSearch"))
        Dim pFilterSupplierID = Common.WebRequest.Data("FilterSupplierID")
        Dim pDontIncludeRetailPrice = CBool2(Common.WebRequest.Data("DontIncludeRetailPrice"))
        Dim pShowArea = CBool2(Common.WebRequest.Data("ShowArea"))
        Dim pShowLastInvCount = CBool2(Common.WebRequest.Data("LastInvCount"))
        Dim pInput = Newtonsoft.Json.JsonConvert.DeserializeObject(Of LibraryPIMProductInput)(Common.WebRequest.InputStreamText)
        Dim pFowstID As String = CStr2(WebRequest.Instance.Data("FowstID"))
        Dim pShowInventoryColumns As String = CBool2(WebRequest.Instance.Data("ShowInventoryColumns"))
        Dim pDrFowstGroupPrice As DataRow

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSearchMore = CInt2(WebRequest.Instance.Data("IsSearchMore")) <> 0
        Dim pInventoryFowstID = Common.WebRequest.Data("InventoryFowstID")

        If String.IsNullOrWhiteSpace(pInventoryFowstID) = False Then
            'If WebRequest.Instance.Business.SystemDefaults.ShowQtyAvailableToCommit = False Then ' old code 2023_09_16__12_57_57_2431
            If pShowInventoryColumns = False Then
                pInventoryFowstID = Nothing
            End If
        End If

        If String.IsNullOrWhiteSpace(pFowstID) = False Then
            pDrFowstGroupPrice = WebRequest.Instance.Business.LibraryOrganizationFOWST.FowstGroupPrice(pFowstID)
        End If

        With CreateFinder()
            pSearchPhrase = CStr2(mFinder.SearchString)
            pSearchPhrase = OptimizeKeywords(pSearchPhrase)
            pSearchPhrase = LeanString(pSearchPhrase)

            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "pimpro.ID"
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductGroupNo"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(pimpro.Brand)), '') + ' ','') + pimpro.ProductName"
                End With
                With .Add("ProductNameOnly", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductName"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = " ISNULL(pimpro.ProductSearchString,'') "
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
                    .Expression = "pimpro.IsBrandOwner"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = " (SELECT TOP 1 C0020 FROM MobizLibraryPIMProductVariableFeatures001 WHERE ProductID = pimpro.ID) "
                End With
                With .Add("Brand", SqlDbType.VarChar)
                    .Expression = "ISNULL(pimpro.Brand, '')"
                End With
                With .Add("Image", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "(SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE Size = 'medium' AND ProductID = pimpro.ID AND IsDefault = 1 AND IsFromVendor = 0)"
                End With
                If pShowArea Then
                    .Add("Area", SqlDbType.VarChar, "tblArea.Area")
                End If
                If pShowLastInvCount Then
                    With .Add("LastInventoryCountDate", SqlDbType.VarChar)
                        .Searchable = False
                        .Expression = " tblLastInvCount.LastInventoryCountDate "
                    End With
                    With .Add("LastInventoryCountBy", SqlDbType.VarChar)
                        .Searchable = False
                        .Expression = " ISNULL(tblLastInvCount.LastInventoryCountBy, '') "
                    End With
                End If
                If CBool2(Common.WebRequest.Data("IncludeBaseUnit")) Then
                    With .Add("UnitMeasureID", SqlDbType.VarChar, "tblBaseUnit.UnitMeasureID")
                        .Searchable = False
                    End With
                    With .Add("UnitMeasure", SqlDbType.VarChar, "tblBaseUnit.UnitMeasure")
                        .Searchable = False
                    End With
                    With .Add("UnitQty", SqlDbType.VarChar, "tblBaseUnit.UnitQty")
                        .Searchable = False
                    End With

                End If
                If CBool2(Common.WebRequest.Data("IncludeMaxUnit")) Then
                    With .Add("MaxUnitMeasureID", SqlDbType.VarChar, "tblMaxUnit.UnitMeasureID")
                        .Searchable = False
                    End With
                    With .Add("MaxUnitMeasure", SqlDbType.VarChar, "tblMaxUnit.UnitMeasure")
                        .Searchable = False
                    End With
                    With .Add("MaxUnitQty", SqlDbType.VarChar, "tblMaxUnit.UnitQty")
                        .Searchable = False
                    End With
                End If

                'If CInt2(WebRequest.Instance.Data("Barcode")) = 1 Then
                '    With .Add("UoMBarcode", SqlDbType.VarChar)
                '        .Expression = " ISNULL((SELECT DISTINCT STUFF((SELECT DISTINCT ', <br> ' + " & vbCrLf &
                '                        " label.Label + ' - ' + barcode.Barcode " & vbCrLf &
                '                        "FROM MobizLibraryPIMProductMeasurement as promeasure " & vbCrLf &
                '                        "   LEFT JOIN MobizLibraryPIMMeasure as measure " & vbCrLf &
                '                        "       ON promeasure.UnitMeasureID = measure.ID " & vbCrLf &
                '                        "   LEFT JOIN MobizLibraryPIMProductMeasurementBarcode as barcode " & vbCrLf &
                '                        "       ON barcode.StockMeasurementsID = promeasure.ID " & vbCrLf &
                '                        "   LEFT JOIN MobizLibraryCountriesLanguageLabel as label " & vbCrLf &
                '                        "       ON label.ID = measure.MeasurenameID " & vbCrLf &
                '                        "   WHERE promeasure.ProductID = pimpro.ID " & vbCrLf &
                '                        "FOR XML PATH('') " & vbCrLf &
                '                        "), 1, 1,'') FROM MobizLibraryPIMProductMeasurement), '') " & vbCrLf
                '    End With
                'End If

                If pSearchMore Then
                    With .Add("AllowDecimal", SqlDbType.Bit)
                        .Searchable = False
                        .Expression = "0"
                    End With
                Else
                    With .Add("AllowDecimal", SqlDbType.Bit)
                        .Searchable = False
                        .Expression = "ISNULL(orgpim.AllowDecimal, 0)"
                    End With
                    If pDontIncludeRetailPrice = False Then
                        With .Add("RetailPrice", SqlDbType.Money)
                            .Searchable = False
                            .Expression = "pimprolvl.Price"
                        End With
                        With .Add("RetailPriceLastUpdated", SqlDbType.DateTime)
                            .Searchable = False
                            .Expression = "pimprolvl.LastUpdated"
                        End With
                        With .Add("WholesalePrice", SqlDbType.Money)
                            .Searchable = False
                            .Expression = "pimprolvl2.Price"
                        End With
                        With .Add("WholesalePriceLastUpdated", SqlDbType.DateTime)
                            .Searchable = False
                            .Expression = "pimprolvl2.LastUpdated"
                        End With
                    End If
                    If pDrFowstGroupPrice IsNot Nothing Then
                        With .Add("FowstGroupPrice", SqlDbType.Money)
                            .Searchable = False
                            .Expression = "tblFowstGroupPrice.Price"
                        End With
                        With .Add("FowstGroupPriceLastUpdated", SqlDbType.DateTime)
                            .Searchable = False
                            .Expression = "tblFowstGroupPrice.LastUpdated"
                        End With
                    End If
                    With .Add("IsDeactivated", SqlDbType.Bit)
                        .Searchable = False
                        .Expression = "orgpim.IsDeactivated"
                    End With
                End If
                If String.IsNullOrWhiteSpace(pVendorID) = False Then
                    With .Add("VendorPrice", SqlDbType.Money)
                        .Searchable = False
                        .Expression = "tblVendorPrice.Price"
                    End With
                    With .Add("VendorPriceLastUpdated", SqlDbType.DateTime)
                        .Searchable = False
                        .Expression = "tblVendorPrice.LastUpdated"
                    End With
                End If
                If IsGuid(pInventoryFowstID) Then
                    With .Add("QtyOnHand", SqlDbType.Decimal)
                        .Searchable = False
                        .Expression = "ISNULL(tblInventory.QtyOnHand, 0)"
                    End With
                    With .Add("CommittedQty", SqlDbType.Decimal)
                        .Searchable = False
                        .Expression = "ISNULL(tblCommitted.Quantity, 0)"
                    End With
                    With .Add("AvailableQtyToCommit", SqlDbType.Decimal)
                        .Searchable = False
                        .Expression = "ISNULL(tblInventory.QtyOnHand, 0) - ISNULL(tblCommitted.Quantity, 0)"
                    End With
                End If

                If CBool2(Common.WebRequest.Data("InventoryCountItemOnly")) Then
                    With .Add("MaxQtyToBeRcvTfer", SqlDbType.Decimal)
                        .Searchable = False
                        .Expression = "ISNULL(orgpim.MaxQtyToBeRcvTfer, 0)"
                    End With
                End If
            End With

            If pSearchMore Then
                .SqlFrom = " FROM MobizLibraryPIMProduct pimpro " &
                    " LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " &
                    " LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) "
            Else
                .SqlFrom = " FROM MobizLibraryPIMProduct pimpro " &
                    " INNER JOIN LibraryOrganizationPIM orgpim ON (pimpro.ID = orgpim.ProductID) " &
                    " LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " &
                    " LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) " & vbCrLf
                If pDontIncludeRetailPrice = False Then
                    .SqlFrom &= "   OUTER APPLY (" & vbCrLf &
                    "     SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                    "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                    "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = orgpim.ProductID" & vbCrLf &
                    "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0005-0000-0000-000000000000'" & vbCrLf &
                    "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                    "   ) pimprolvl" & vbCrLf

                    .SqlFrom &= "   OUTER APPLY (" & vbCrLf &
                    "     SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                    "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                    "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = orgpim.ProductID" & vbCrLf &
                    "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0004-0000-0000-000000000000'" & vbCrLf &
                    "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                    "   ) pimprolvl2" & vbCrLf
                End If

                If pDrFowstGroupPrice IsNot Nothing Then
                    .SqlFrom &= "   OUTER APPLY (" & vbCrLf &
                    "     SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                    "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                    "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = orgpim.ProductID" & vbCrLf &
                    "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(pDrFowstGroupPrice("ID")) & vbCrLf &
                    "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                    "   ) tblFowstGroupPrice" & vbCrLf
                End If
            End If
            If (CBool2(WebRequest.Instance.Data("WithContentsOnly"))) Then
                .SqlFrom &= " JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = pimpro.ID " &
                " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            End If
            If String.IsNullOrWhiteSpace(pVendorID) = False Then
                .SqlFrom &= "    OUTER APPLY (" & vbCrLf &
                    "      SELECT TOP 1 LibraryVendorPrices.Price" & vbCrLf &
                    "      ,LibraryVendorPrices.LastUpdated" & vbCrLf &
                    "      FROM LibraryVendorPrices" & vbCrLf &
                    "      WHERE LibraryVendorPrices.ProductID = orgpim.ProductID" & vbCrLf &
                    "      AND LibraryVendorPrices.VendorID = " & ReadyString(pVendorID) & vbCrLf &
                    "      AND ISNULL(LibraryVendorPrices.IsActive, 1) = 1 " & vbCrLf &
                    "    ) tblVendorPrice" & vbCrLf
            End If

            If pUseCacheProductSearch Then
                .SqlFrom &= " JOIN CacheProductSearch" & vbCrLf &
                    " ON CacheProductSearch.ProductID = pimpro.ID" & vbCrLf &
                    " AND CacheProductSearch.SearchPhrase = " & ReadyString(pSearchPhrase) & vbCrLf
            End If

            If IsGuid(pInventoryFowstID) Then
                .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                    "   SELECT SUM(LibraryFowstBinProduct.Balance) AS QtyOnHand" & vbCrLf &
                    "   FROM LibraryFowstBinProduct" & vbCrLf &
                    "   WHERE LibraryFowstBinProduct.FowstID = " & ReadyString(pInventoryFowstID) & vbCrLf &
                    "   AND LibraryFowstBinProduct.ProductID = pimpro.ID" & vbCrLf &
                    ") tblInventory" & vbCrLf
                '.SqlFrom &= " OUTER APPLY (" & vbCrLf &
                '    "   SELECT SUM(#t2.Balance) AS AvailableQtyToCommit" & vbCrLf &
                '    "   FROM LibraryFowstBinProduct #t2" & vbCrLf &
                '    "   WHERE #t2.FowstID = " & ReadyString(pInventoryFowstID) & vbCrLf &
                '    "   AND #t2.ProductID = pimpro.ID" & vbCrLf &
                '    ") tblInventory" & vbCrLf
                .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                    " SELECT t1.FOWSTID, SUM(OrderedQuantity) Quantity FROM TransInvoice t1 " & vbCrLf &
                    " inner join TransInvoiceItems on t1.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
                    " WHERE t1.TransType = 'SalesOrder' " & vbCrLf &
                    " AND ISNULL(t1.IsCancelled, 0) = 0" & vbCrLf &
                    " AND TransInvoiceItems.ProductID = pimpro.ID" & vbCrLf &
                    " AND t1.FOWSTID = " & ReadyString(pInventoryFowstID) & vbCrLf &
                    " AND NOT EXISTS (SELECT * FROM TransInvoice t2 " & vbCrLf &
                    " WHERE invoicenumber > 0 " & vbCrLf &
                    " AND t2.SalesOrderID = t1.ID)  " & vbCrLf &
                    " GROUP BY t1.FOWSTID " & vbCrLf &
                    " ) tblCommitted" & vbCrLf
            End If
            If IsGuid(pFilterSupplierID) Then
                .SqlFrom &= " JOIN LibraryVendorPrices ON LibraryVendorPrices.ProductID = pimpro.ID" & vbCrLf &
                    " AND LibraryVendorPrices.VendorID = " & ReadyString(pFilterSupplierID) & vbCrLf &
                    " AND ISNULL(LibraryVendorPrices.IsActive, 1) = 1 "
            End If
            If pShowArea Then
                .SqlFrom &= "  OUTER APPLY (" & vbCrLf &
                    "  	SELECT TOP 1 LibraryFowstBin.Area" & vbCrLf &
                    "     FROM LibraryFowstBinProduct" & vbCrLf &
                    "     LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = LibraryFowstBinProduct.BinID" & vbCrLf &
                    "     AND LibraryFowstBin.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) & vbCrLf &
                    "     WHERE LibraryFowstBinProduct.ProductID = pimpro.ID" & vbCrLf &
                    "     AND RTRIM(LTRIM(LibraryFowstBin.Area)) <> ''" & vbCrLf &
                    "  ) tblArea" & vbCrLf
            End If
            If pShowLastInvCount Then
                .SqlFrom &= "  OUTER APPLY (" & vbCrLf &
                    "  	SELECT TOP 1 LibraryFOWSTBinProduct.LastInventoryCountDate as LastInventoryCountDate" & vbCrLf &
                    "     , ISNULL(LibraryFOWSTBinProduct.LastInventoryCountBy, '') as LastInventoryCountBy" & vbCrLf &
                    "     FROM LibraryFowstBinProduct" & vbCrLf &
                    "     LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = LibraryFowstBinProduct.BinID" & vbCrLf &
                    "     AND LibraryFowstBin.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) & vbCrLf &
                    "     WHERE LibraryFowstBinProduct.ProductID = pimpro.ID" & vbCrLf &
                    "     ORDER BY LibraryFowstBinProduct.LastUpdated DESC " & vbCrLf &
                    "  ) tblLastInvCount " & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("IncludeBaseUnit")) Then
                .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                    " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                    " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                    " 	,MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                    "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                    "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                    "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductMeasurement.ProductID = pimpro.ID" & vbCrLf &
                    "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                    " ) tblBaseUnit" & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("IncludeMaxUnit")) Then
                .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                    " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                    " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                    " 	,MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                    "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                    "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                    "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductMeasurement.ProductID = pimpro.ID" & vbCrLf &
                    "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity DESC" & vbCrLf &
                    " ) tblMaxUnit" & vbCrLf
            End If
            .SqlOrder = " ORDER BY pimpro.IsBrandOwner DESC, ProductName ASC "

            pWhere = " WHERE pimpro.IsActive = 1 " & vbCrLf &
                " AND ISNULL(pimpro.ProductName, '') <> ''" & vbCrLf

            If CBool2(Common.WebRequest.Data("InventoryCountItemOnly")) Then
                pWhere &= " AND orgpim.IsInventoryCountItem = 1 " & vbCrLf
            End If

            If Not pSearchMore Then
                pWhere &= " AND orgpim.IsDeactivated = " & ReadyString(CDec2(WebRequest.Instance.Data("IsDeactivated")))
            End If

            If CStr2(mFinder.SearchString).Trim <> "" Then
                If vSearchBarcode Then
                    pWhere &= " AND (" & .CreateWhere & " OR " & vbCrLf &
                    " pimpro.ID IN" & vbCrLf &
                    "   (SELECT MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
                    "   FROM MobizLibraryPIMProductMeasurementBarcode" & vbCrLf &
                    "   JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurement.ID = MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID" & vbCrLf &
                    "   WHERE MobizLibraryPIMProductMeasurementBarcode.ReadyBarcode = " & ReadyString(ReadyBarcode(mFinder.SearchString)) & vbCrLf &
                    "))"
                End If
            End If

            If InStr(1, pSearchPhrase, "BLT--") Then
                .SqlWhereExact = pWhere & " AND pimpro.ID IN (select ProductID from LibraryFowstBin " & vbCrLf &
                    " inner join LibraryFowstBinProduct " & vbCrLf &
                    " on LibraryFowstBin.ID = LibraryFowstBinProduct.BinID and LibraryFowstBin.FowstID = LibraryFowstBinProduct.FowstID " & vbCrLf &
                    " where LibraryFowstBin.ID = " & ReadyString(Common.WebRequest.Data("Base64ToGuid")) & ") " & vbCrLf
            End If
            If CBool2(pInput.FilterByBrandAndCategory) _
                OrElse CBool2(pInput.FilterByBrandOrCategory) Then

                Dim pBrands = ""
                Dim pCategories = ""

                If pInput.Brands IsNot Nothing _
                    AndAlso pInput.Brands.Count > 0 Then

                    For Each pBrand In pInput.Brands
                        If pBrands <> "" Then
                            pBrands &= ", "
                        End If
                        pBrands &= ReadyString(pBrand)
                    Next
                End If

                If pInput.Categories IsNot Nothing _
                    AndAlso pInput.Categories.Count > 0 Then

                    For Each pCategory In pInput.Categories
                        If pCategories <> "" Then
                            pCategories &= ", "
                        End If
                        pCategories &= ReadyString(pCategory)
                    Next
                End If

                pWhere &= " AND (" & vbCrLf
                If pBrands <> "" Then
                    pWhere &= " ISNULL(pimpro.Brand, '') IN (" & pBrands & ")" & vbCrLf
                Else
                    pWhere &= " 1=1 -- dont filter by brand " & vbCrLf
                End If

                If pBrands <> "" _
                    AndAlso pCategories <> "" _
                    AndAlso CBool2(pInput.FilterByBrandOrCategory) Then

                    pWhere &= " OR " & vbCrLf
                Else
                    pWhere &= " AND " & vbCrLf
                End If

                If pCategories <> "" Then
                    pWhere &= " EXISTS(SELECT TOP 1 C0020 FROM MobizLibraryPIMProductVariableFeatures001 WHERE ProductID = pimpro.ID AND C0020 IN (" & pCategories & "))" & vbCrLf
                Else
                    pWhere &= " 1=1 -- dont filter by category " & vbCrLf
                End If

                pWhere &= ")" & vbCrLf
            End If

            If CBool2(WebRequest.Instance.Data("IsAll")) Then
                .MaxRecords = 0 ' release
                '.MaxRecords = 100
            End If
            .SqlWhere = pWhere
            pSql = .CreateSql()
        End With

        'WebRequest.Instance.ProData.BeginTransaction()
        If IsGuid(pInventoryFowstID) Then
            WebRequest.Instance.Business.TransInventoryLedger.UpdateLibraryFowstBinProductDetails(pInventoryFowstID)
        End If
#If DEBUG Then
        Common.WebRequest.ProData.CommandTimeout = 10 * 60 '
#Else
        Common.WebRequest.ProData.CommandTimeout = 60 ' 60 seconds
#End If
        Dim pDT As DataTable
        Try
            pDT = Common.WebRequest.ProData.Execute(pSql)
        Catch ex As ProSqlException
            AppendToLog("vbQWR727TSK4! " & ex.Message & vbCrLf & ex.Sql)
            Throw
        End Try
        'WebRequest.Instance.ProData.RollbackTransaction()
        If pUseCacheProductSearch _
            AndAlso pDT.Rows.Count = 0 Then

            Return LibraryPIMProduct(True)
        End If
        If vSearchBarcode = False _
            AndAlso pDT.Rows.Count = 0 Then

            Return LibraryPIMProduct(True, True)
        End If

        If pCacheProductSearch Then
            Dim pSqlDelete = "DELETE FROM CacheProductSearch" & vbCrLf &
                " WHERE SearchPhrase = " & ReadyString(pSearchPhrase)
            pSqlDelete = Common.WebRequest.ProData.ExecuteSQL(pSqlDelete) ' output is used for debugging only

            Dim pIU = New InsertUpdate("CacheProductSearch")
            pIU("ProductID") = "ID"
            pIU("SearchPhrase") = ReadyString(pSearchPhrase)
            pIU.FromSql = " FROM (" & pSql & ") t"
            pSql = pIU.GetInsert(True)
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Dim pOut = New JsonDictionary
            pOut.Add("Results", pDT)
            If pDrFowstGroupPrice IsNot Nothing Then
                pOut.Add("FowstGroupPriceName", CStr2(pDrFowstGroupPrice("PriceLevel")))
            End If
            Return pOut.Serialize()
        End If

    End Function

    <HttpPost()>
    Public Function LibraryPimProductGroupNo(Optional ByVal IsAll As Boolean = False) As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            '.MaxRecords = 0
            With .Fields
                .Add("ProductGroupNo", SqlDbType.VarChar, "ProductGroupNo")
            End With

            .SqlFrom = " FROM MobizLibraryPimProduct" & vbCrLf
            .SqlWhere = "WHERE ISNULL(ProductGroupNo, '') <> '' "

            If CStr2(Common.WebRequest.Data("keywords")) <> "" Then
                .SqlWhere &= " AND ProductGroupNo LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))
            End If

            .SqlOrder = " ORDER BY ProductGroupNo ASC "
            .SqlGroup = " GROUP BY ProductGroupNo "
        End With

        Return SubmitFinder()

    End Function

    Public Class LibraryPIMProductInput
        Public FilterByBrandAndCategory As String ' boolean
        Public FilterByBrandOrCategory As String ' boolean
        Public Brands As List(Of String)
        Public Categories As List(Of String)
    End Class

    <HttpPost()>
    Public Function LibraryPIMProductPublic() As String

        Dim pSql As String
        Dim pWhere As String
        Dim pSearchMore As Boolean

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        pSearchMore = CInt2(WebRequest.Instance.Data("IsSearchMore")) <> 0

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "pimpro.ID"
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductGroupNo"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductName"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Expression = " ISNULL(pimpro.ProductSearchString,'') "
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
                    .Expression = "pimpro.IsBrandOwner"
                End With
                With .Add("AllowDecimal", SqlDbType.Bit)
                    .Searchable = False
                    .Expression = "0"
                End With

            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct pimpro " &
            " LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " &
            " LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) "

            pWhere = " WHERE pimpro.IsActive = 1 " & vbCrLf &
                " AND pimpro.ID NOT IN (SELECT ProductID FROM LibraryOrganizationPIM) " & vbCrLf &
                " AND ISNULL(pimpro.ProductName, '') <> ''" & vbCrLf

            If CStr2(mFinder.SearchString).Trim <> "" Then
                pWhere &= " AND (" & .CreateWhere & " OR " & vbCrLf &
                    " pimpro.ID IN" & vbCrLf &
                    "   (SELECT MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
                    "   FROM MobizLibraryPIMProductMeasurementBarcode" & vbCrLf &
                    "   JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurement.ID = MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID" & vbCrLf &
                    "   WHERE MobizLibraryPIMProductMeasurementBarcode.ReadyBarcode = " & ReadyString(ReadyBarcode(mFinder.SearchString)) & vbCrLf &
                    "))"
            End If

            .SqlOrder = " ORDER BY pimpro.IsBrandOwner DESC, pimpro.ProductName ASC "

            pSql = .CreateSql(pWhere)
        End With

        Return SubmitFinder(pSql)

    End Function

    Public Class ProductsByCategoriesJson
        Public PriceLevelToAdjustID As String
        Public PriceLevelBasisID As String
        Public Brands As List(Of String)
        Public Categories As List(Of String)
        Public Rate As String
        Public Method As String
        Public RoundMethod As String
        Public RoundIncrement As String
        Public ExcludeDateFrom As String
        Public ExcludeDateTo As String

        Public ReadOnly Property RateProper As Decimal
            Get
                Return CDec2(Rate)
            End Get
        End Property

    End Class

    <HttpPost()>
    Public Function ProductsByCategories() As String

        Dim pSql As String
        Dim pWhere As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInput = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ProductsByCategoriesJson)(Common.WebRequest.InputStreamText)
        If pInput.Categories Is Nothing _
            OrElse pInput.Categories.Count = 0 Then

            Return JsonDictionary.Serialize("Error", "Please select at least 1 category.")
        End If
        Dim pCategories = ""
        For Each pCategory In pInput.Categories
            If pCategories <> "" Then
                pCategories &= ","
            End If
            pCategories &= ReadyString(pCategory)
        Next pCategory
        If pInput.Method = "gross margin" Then
            pInput.PriceLevelBasisID = WebRequest.Instance.Business.SystemDefaults.GetLandedCostID()
        ElseIf pInput.Method = "nominal price" Then
            pInput.PriceLevelBasisID = pInput.PriceLevelToAdjustID
        End If
        With CreateFinder()
            .Distinct = True
            .MaxRecords = 1000
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Expression = " ISNULL(MobizLibraryPIMProduct.ProductSearchString,'') "
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = " MobizLibraryPIMProductVariableFeatures001.C0020"
                End With
                With .Add("PriceToAdjust", SqlDbType.Decimal)
                    .Expression = " tblPriceToAdjust.Price"
                End With
                With .Add("BasisPrice", SqlDbType.Decimal)
                    .Expression = " tblBasisPrice.Price"
                End With
                Select Case pInput.Method
                    Case "mark up"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = 1 + pInput.RateProper / 100 & " * tblBasisPrice.Price"
                        End With

                    Case "mark down"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = 1 - pInput.RateProper / 100 & " * tblBasisPrice.Price"
                        End With

                    Case "gross margin"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = "tblBasisPrice.Price / " & (1 - pInput.RateProper / 100)
                        End With

                    Case "nominal price"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = pInput.RateProper
                        End With

                    Case Else
                        Return JsonDictionary.Serialize("Error", "Invalid price adjustment method: " & pInput.Method)

                End Select
            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct" & vbCrLf &
                " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID"
            .SqlFrom &= "  OUTER APPLY (" & vbCrLf &
                "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(pInput.PriceLevelToAdjustID) & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                "  ) tblPriceToAdjust" & vbCrLf &
                "  OUTER APPLY (" & vbCrLf &
                "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(pInput.PriceLevelBasisID) & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                "  ) tblBasisPrice" & vbCrLf
            .SqlWhere = " WHERE MobizLibraryPIMProduct.IsActive = 1 " & vbCrLf
            If String.IsNullOrWhiteSpace(pCategories) = False Then
                .SqlWhere &= " AND MobizLibraryPIMProductVariableFeatures001.C0020 IN (" & pCategories & ")" & vbCrLf
            End If
            .SqlOrder = " ORDER BY MobizLibraryPIMProduct.ProductName"

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function ProductsByBrandsOrCategories() As String

        Dim pSql As String
        Dim pWhere As String
        Dim pDT As DataTable

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInput = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ProductsByCategoriesJson)(Common.WebRequest.InputStreamText)

        If String.IsNullOrWhiteSpace(pInput.ExcludeDateFrom) = False Then
            If IsDate(pInput.ExcludeDateFrom) = False Then
                Return JsonDictionary.Serialize("Error", "If specified, Exclude Date From must be a valid date.")
            End If
        End If

        If String.IsNullOrWhiteSpace(pInput.ExcludeDateTo) = False Then
            If IsDate(pInput.ExcludeDateTo) = False Then
                Return JsonDictionary.Serialize("Error", "If specified, Exclude To Date must be a valid date.")
            End If
        End If

        If (pInput.Categories Is Nothing _
            OrElse pInput.Categories.Count = 0) _
            And (pInput.Brands Is Nothing _
            OrElse pInput.Brands.Count = 0) Then

            Return JsonDictionary.Serialize("Error", "Please select at least 1 brand or category.")
        End If
        Dim pBrands = ""
        If pInput.Brands IsNot Nothing Then
            For Each pBrand In pInput.Brands
                If pBrands <> "" Then
                    pBrands &= ","
                End If
                pBrands &= ReadyString(pBrand)
            Next pBrand
        End If

        Dim pCategories = ""
        If pInput.Categories IsNot Nothing Then
            For Each pCategory In pInput.Categories
                If pCategories <> "" Then
                    pCategories &= ","
                End If
                pCategories &= ReadyString(pCategory)
            Next pCategory
        End If

        If pInput.Method = "gross margin" Then
            pInput.PriceLevelBasisID = WebRequest.Instance.Business.SystemDefaults.GetLandedCostID()
        ElseIf pInput.Method = "nominal price" Then
            pInput.PriceLevelBasisID = pInput.PriceLevelToAdjustID
        End If
        With CreateFinder()
            .Distinct = True
            .MaxRecords = 1000
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Expression = " ISNULL(MobizLibraryPIMProduct.ProductSearchString,'') "
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = " MobizLibraryPIMProductVariableFeatures001.C0020"
                End With
                With .Add("PriceToAdjust", SqlDbType.Decimal)
                    .Expression = " tblPriceToAdjust.Price"
                End With
                With .Add("BasisPrice", SqlDbType.Decimal)
                    .Expression = " tblBasisPrice.Price"
                End With
                Select Case pInput.Method
                    Case "mark up"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = 1 + pInput.RateProper / 100 & " * tblBasisPrice.Price"
                        End With

                    Case "mark down"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = 1 - pInput.RateProper / 100 & " * tblBasisPrice.Price"
                        End With

                    Case "gross margin"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = "tblBasisPrice.Price / " & (1 - pInput.RateProper / 100)
                        End With

                    Case "nominal price"
                        With .Add("ResultingPrice", SqlDbType.Decimal)
                            .Expression = pInput.RateProper
                        End With

                    Case Else
                        Return JsonDictionary.Serialize("Error", "Invalid price adjustment method: " & pInput.Method)

                End Select
            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct" & vbCrLf &
                " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryOrganizationPIM.ProductID" & vbCrLf
            .SqlFrom &= "  OUTER APPLY (" & vbCrLf &
                "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(pInput.PriceLevelToAdjustID) & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                "  ) tblPriceToAdjust" & vbCrLf &
                "  OUTER APPLY (" & vbCrLf &
                "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(pInput.PriceLevelBasisID) & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                "  ) tblBasisPrice" & vbCrLf
            .SqlWhere = " WHERE MobizLibraryPIMProduct.IsActive = 1 " & vbCrLf
            If String.IsNullOrWhiteSpace(pInput.ExcludeDateFrom) = False Then
                .SqlWhere &= " AND (tblPriceToAdjust.LastUpdated IS NULL" & vbCrLf
                .SqlWhere &= " OR NOT (" & vbCrLf &
                    " CONVERT(date, tblPriceToAdjust.LastUpdated) >= " & ReadyString(Format(ParseDate(pInput.ExcludeDateFrom).Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf &
                    " AND CONVERT(date, tblPriceToAdjust.LastUpdated) <= " & ReadyString(Format(ParseDate(pInput.ExcludeDateTo).Value.Date, cGlobal.FormatDateTimeSql)) & vbCrLf &
                    ")" & vbCrLf &
                    ")"
            End If
            .SqlWhere &= " AND (" & vbCrLf
            If String.IsNullOrWhiteSpace(pCategories) Then
                .SqlWhere &= " 1=0 " & vbCrLf
            Else
                .SqlWhere &= " MobizLibraryPIMProductVariableFeatures001.C0020 IN (" & pCategories & ")" & vbCrLf
            End If
            .SqlWhere &= " OR " & vbCrLf
            If String.IsNullOrWhiteSpace(pBrands) Then
                .SqlWhere &= " 1=0 " & vbCrLf
            Else
                .SqlWhere &= " MobizLibraryPIMProduct.Brand IN (" & pBrands & ")" & vbCrLf
            End If
            .SqlWhere &= " )" & vbCrLf
            .SqlOrder = " ORDER BY MobizLibraryPIMProduct.ProductName"
            pDT = .Requery()
        End With

        Dim pRoundMethod = pInput.RoundMethod ' Common.WebRequest.Data("RoundMethod")
        Dim pRoundIncrement = pInput.RoundIncrement ' CDec2(Common.WebRequest.Data("RoundIncrement"))
        pRoundIncrement /= 100

        If String.IsNullOrWhiteSpace(pRoundMethod) Then
            'Return JsonDictionary.Serialize("Results", pDT)
            'just do nothing
        ElseIf pRoundMethod = "RoundOff" _
            OrElse pRoundMethod = "RoundUp" _
            OrElse pRoundMethod = "RoundDown" Then

            Dim pRoundingMethod As eRoundingMethod

            If pRoundMethod = "RoundOff" Then
                pRoundingMethod = eRoundingMethod.Off
            ElseIf pRoundMethod = "RoundUp" Then
                pRoundingMethod = eRoundingMethod.Up
            ElseIf pRoundMethod = "RoundDown" Then
                pRoundingMethod = eRoundingMethod.Down
            End If

            For Each pDR As DataRow In pDT.Rows
                pDR("ResultingPrice") = RoundDecimal(CDec2(pDR("ResultingPrice")), pRoundIncrement, pRoundingMethod)
            Next
        End If

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    <HttpPost()>
    Public Function LibraryFowstBin() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            If CInt(WebRequest.Instance.Data("NoFilter")) Then
                mFinder.MaxRecords = 0
            End If
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                    .Expression = "ISNULL(LibraryFowstBin.ID,'')"
                End With

                With .Add("Area", SqlDbType.VarChar)
                    .Expression = "ISNULL(LibraryFowstArea.AreaName,'')"
                End With

                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "Replace(ISNULL(LibraryFowstBin.Name,''),'.','')"
                End With

                With .Add("BinLongName", SqlDbType.VarChar)
                    .Expression = "ISNULL(LibraryFowstBin.BinLongName,'')"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "ISNULL(Description,'')"
                End With

                .Add("IsActive", SqlDbType.Bit)

            End With
            .SqlFrom = "FROM LibraryFowstBin left join LibraryFowstArea on LibraryFowstArea.ID = LibraryFowstBin.AreaID"

            .SqlWhere = "WHERE LibraryFowstBin.IsActive = 1 AND Name<>'Unnamed Bin' AND LibraryFowstBin.FowstID = " & ReadyString(IIf(Common.WebRequest.Data("FowstID") = vbNullString, WebSession.Instance.FowstID, Common.WebRequest.Data("FowstID")))

            .SqlOrder = " Order BY cast( replace(STUFF(BinLongName, PATINDEX('%[^0-9]%', BinLongName), 2, ''),'.','') as int) ASC"
            If Not CStr2(Common.WebRequest.Data("AreaID")) = vbNullString Then
                If Not CStr2(Common.WebRequest.Data("AreaID")) = "All" Then
                    .SqlWhere += " AND LibraryFowstArea.ID = " & ReadyString(CStr2(Common.WebRequest.Data("AreaID")))
                End If
            End If
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowstBinProduct() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                    .Expression = "bin.ID"
                End With

                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "bin.Name"
                End With

                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "bin.Description"
                End With

                With .Add("MinPerProduct", SqlDbType.VarChar)
                    .Expression = "binpro.MinPerProduct"
                End With

                With .Add("MaxPerProduct", SqlDbType.VarChar)
                    .Expression = "binpro.MaxPerProduct"
                End With
            End With
            .SqlFrom = "FROM dbo.LibraryFowstBin bin " &
                " INNER JOIN dbo.LibraryFowstBinProduct binpro ON (bin.ID = binpro.BinID)"

            .SqlWhere = "WHERE binpro.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) &
                        " AND binpro.BinID = " & ReadyString(Common.WebRequest.Data("BinID")) &
                        " AND binpro.ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransSalesOrder() As String

        Dim pCustomerID As String

        pCustomerID = Common.WebRequest.Data("CustomerID")
        If IsGuid(pCustomerID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Customer ID")
        End If

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransSalesOrder.ID"
                End With
                With .Add("OrderNo", SqlDbType.VarChar)
                    .Expression = "TransSalesOrder.OrderNo"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "TransSalesOrder.LastUpdated"
                End With
                With .Add("Total", SqlDbType.Money)
                    .Expression = "tblItems.Total"
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "tblItems.SkuCount"
                End With
            End With
            .SqlFrom = " FROM TransSalesOrder" & vbCrLf
            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransSalesOrderItems.SubTotal) AS Total" & vbCrLf &
                "   ,COUNT(DISTINCT TransSalesOrderItems.StockID) AS SkuCount" & vbCrLf &
                " 	FROM TransSalesOrderItems" & vbCrLf &
                "   WHERE TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & vbCrLf &
                "   AND TransSalesOrderItems.Quantity > 0" & vbCrLf &
                " ) tblItems    " & vbCrLf
            .SqlWhere = " WHERE TransSalesOrder.CustomerID = " & ReadyString(pCustomerID) & vbCrLf &
                " AND TransSalesOrder.IsHold = 1" & vbCrLf &
                " AND TransSalesOrder.IsCancelled = 0" & vbCrLf
            .SqlOrder = "ORDER BY TransSalesOrder.LastUpdated DESC" & vbCrLf
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransInvoices() As String

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEdit _
        '            OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable _
        '            OrElse WebSession.Instance.UserAccess.PointOfSales_PointOfSales.IsEnable _
        '    ) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pCustomerID As String
        Dim pNonZero As Boolean
        Dim pForPickingList As Boolean
        Dim pSectionID As String
        Dim pForInvoicePrinting As Boolean
        Dim pPendingReprintRequest As Boolean
        Dim pPoMode As Boolean
        Dim pTransType As String
        Dim pTransTypes As String
        Dim pAnyZero As Boolean
        Dim pWithCnCUnapprovedReason As Boolean
        Dim pWithApprovedCnC As Boolean
        Dim pWithCnCUnapprovedReasonSummary As Boolean
        Dim pForInvoicing As Boolean
        Dim pAllowNoItems As Boolean
        Dim pUserID As String = Common.WebRequest.Data("UserID") ' if set, result will be filtered by the specified UserID
        Dim pExcludeDelivered As Boolean = CBool2(Common.WebRequest.Data("ExcludeDelivered"))
        Dim pExcludeUndelivered As Boolean = CBool2(Common.WebRequest.Data("ExcludeUndelivered"))
        Dim pDisplayUnservedCount As Boolean = CBool2(Common.WebRequest.Data("DisplayUnservedCount"))
        Dim pInvoiceFowstID As String = Common.WebRequest.Data("InvoiceFowstID")
        Dim pDeliveryCityID As String = Common.WebRequest.Data("DeliveryCityID")
        Dim pFilterPoAndInvoice As String = Common.WebRequest.Data("FilterPoAndInvoice")
        Dim pLastUpdatedDateFrom As Date? = CDateNull(Common.WebRequest.Data("LastUpdatedDateFrom"))
        Dim pLastUpdatedDateTo As Date? = CDateNull(Common.WebRequest.Data("LastUpdatedDateTo"))
        Dim pBackordersOnly As Boolean = CBool2(Common.WebRequest.Data("BackordersOnly"))
        Dim pExcludeBackorders As Boolean = CBool2(Common.WebRequest.Data("ExcludeBackorders"))
        Dim pInvoiceSource As String = CStr2(Common.WebRequest.Data("InvoiceSource")).ToLower()
        Dim pDraftStatus = CStr2(Common.WebRequest.Data("DraftStatus")).Trim().ToLower()
        Dim pApprovedStatus = CStr2(Common.WebRequest.Data("ApprovedStatus")).Trim().ToLower()
        Dim pRoutingSheetNumber = CStr2(Common.WebRequest.Data("RoutingSheetNumber"))
        Dim pHasUnapprovedReprintRequest = CBool2(Common.WebRequest.Data("HasUnapprovedReprintRequest"))
        Dim pLimitSearchFields = CBool2(Common.WebRequest.Data("LimitSearchFields"))
        Dim pArchive = Common.WebRequest.Data("Archive") = "archive"
        Dim pDeliveryTruckFowstID As String = CStr2(Common.WebRequest.Data("DeliveryTruckFowstID"))
        Dim pDelivery3PlCompanyID As String = CStr2(Common.WebRequest.Data("Delivery3PlCompanyID"))
        Dim pDeliveryCustomerRepresentative As String = CStr2(Common.WebRequest.Data("DeliveryCustomerRepresentative"))
        Dim pDeliveryStatus As String = CStr2(Common.WebRequest.Data("DeliveryStatus"))
        Dim pExcludeCashPaymentCustomer As Boolean = CBool2(Common.WebRequest.Data("ExcludeCashPaymentCustomer"))
        Dim pNonZeroServedSkuCount As Boolean = CBool2(Common.WebRequest.Data("NonZeroServedSkuCount"))

        If pArchive Then
            Dim pNewDatabaseName = WebRequest.Instance.Database & "Archive"
            Dim pDatabases = WebRequest.Instance.ProData.GetDatabases(True, True)
            If pDatabases.Contains(pNewDatabaseName.ToLower()) = False Then
                Return JsonDictionary.Serialize("Error", "Archive database does not exist for the current company. Go to System Settings > Auto Data Archiving to archive some records")
            End If
        End If

        Dim pDT As DataTable

        If pLastUpdatedDateFrom IsNot Nothing Then
            pLastUpdatedDateFrom = pLastUpdatedDateFrom.Value.Date
        End If
        If pLastUpdatedDateTo IsNot Nothing Then
            pLastUpdatedDateTo = pLastUpdatedDateTo.Value.Date
        End If
        Dim pXAxisValue = Common.WebRequest.Data("XAxisValue")
        Dim pYAxisValue = Common.WebRequest.Data("YAxisValue")
        Dim pXAxisName = Common.WebRequest.Data("XAxisName")
        Dim pYAxisName = Common.WebRequest.Data("YAxisName")
        pForInvoicing = CBool2(Common.WebRequest.Data("ForInvoicing"))
        pWithCnCUnapprovedReasonSummary = CBool2(Common.WebRequest.Data("WithCnCUnapprovedReasonSummary"))
        Dim pWithCnCDisapproved = CBool2(Common.WebRequest.Data("WithCnCDisapproved"))
        If pWithCnCUnapprovedReasonSummary Then
            pWithCnCUnapprovedReason = True
        Else
            pWithCnCUnapprovedReason = CBool2(Common.WebRequest.Data("WithCnCUnapprovedReason"))
            pWithApprovedCnC = CBool2(Common.WebRequest.Data("WithApprovedCnC"))
        End If
        pAllowNoItems = CBool2(Common.WebRequest.Data("AllowNoItems"))
        pTransType = Common.WebRequest.Data("TransType")
        pTransTypes = Common.WebRequest.Data("TransTypes")
        pPoMode = CBool2(Common.WebRequest.Data("PoMode"))
        pForPickingList = CBool2(Common.WebRequest.Data("ForPickingList"))
        pPendingReprintRequest = CBool2(Common.WebRequest.Data("PendingReprintRequest"))
        If pForPickingList Then
            pSectionID = Common.WebRequest.Data("SectionID")
            If IsGuid(pSectionID) = False Then
                If pSectionID <> "-" Then
                    Return JsonDictionary.Serialize("Error", "SectionID must be a valid GUID if ForPickingList is set")
                End If
            Else
                pSectionID = ReadyGuid(pSectionID)
            End If
        End If
        pCustomerID = Common.WebRequest.Data("CustomerID")
        pNonZero = CDec2(Common.WebRequest.Data("NonZero"))
        pAnyZero = CBool2(Common.WebRequest.Data("AnyZero"))
        pForInvoicePrinting = CBool2(Common.WebRequest.Data("ForInvoicePrinting"))

        If CBool2(Common.WebRequest.QueryString("DownloadReport")) Then
            'para ni sa pag download og Delivery Report, by Arnie 3-14-2023
            pExcludeUndelivered = CBool2(Common.WebRequest.QueryString("ExcludeUndelivered"))
            pDeliveryTruckFowstID = CStr2(Common.WebRequest.QueryString("DeliveryTruckFowstID"))
            pDelivery3PlCompanyID = CStr2(Common.WebRequest.QueryString("Delivery3PlCompanyID"))
            pDeliveryCustomerRepresentative = CStr2(Common.WebRequest.QueryString("DeliveryCustomerRepresentative"))
            pRoutingSheetNumber = CStr2(Common.WebRequest.QueryString("RoutingSheetNumber"))
            pNonZero = CDec2(Common.WebRequest.QueryString("NonZero"))
        End If

        Dim pDateNeededFrom As String = Common.WebRequest.Data("DateNeededFrom")
        If IsDate(pDateNeededFrom) = False Then
            pDateNeededFrom = Nothing
        End If
        Dim pDateNeededTo As String = Common.WebRequest.Data("DateNeededTo")
        If IsDate(pDateNeededTo) = False Then
            pDateNeededTo = Nothing
        End If

        Dim pSalesOrderDateFrom As String = Common.WebRequest.Data("SalesOrderDateFrom")
        If IsDate(pSalesOrderDateFrom) = False Then
            pSalesOrderDateFrom = Nothing
        End If
        Dim pSalesOrderDateTo As String = Common.WebRequest.Data("SalesOrderDateTo")
        If IsDate(pSalesOrderDateTo) = False Then
            pSalesOrderDateTo = Nothing
        End If

        Dim pMaxRecords As Integer = 50
        Dim pMaxRecordsText = WebRequest.Instance.Data("MaxRecords")
        If String.IsNullOrWhiteSpace(pMaxRecordsText) = False Then
            pMaxRecords = CInt2(pMaxRecordsText)
        End If
        With CreateFinder()
            .MaxRecords = pMaxRecords
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.ID"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.CustomerID"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("InvoiceNumber", SqlDbType.Int)
                    .Expression = "TransInvoice.InvoiceNumber"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("FOWSTNo", SqlDbType.Int)
                    .Expression = "ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("SearchFowstWideInvoiceNumberNoDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)) + CONVERT(varchar, TransInvoice.FowstWideInvoiceNumber)"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("SearchFowstWideInvoiceNumber", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)) + '-' + CONVERT(varchar, TransInvoice.FowstWideInvoiceNumber)"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("SearchSInoDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)) + CONVERT(varchar, ISNULL(LibraryDevice.DeviceNo, 0)) +  CONVERT(varchar, ISNULL(TransInvoice.InvoiceNumber,0)) "
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "invoice")
                End With
                With .Add("SearchSIDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)) + '-' + CONVERT(varchar, ISNULL(LibraryDevice.DeviceNo, 0)) + '-' + CONVERT(varchar, ISNULL(TransInvoice.InvoiceNumber,0)) "
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "invoice")
                End With
                With .Add("InvoiceNum", SqlDbType.VarChar)
                    .Expression = "RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7)"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("RefInvoiceNum", SqlDbType.VarChar)
                    .Expression = "TransInvoice.RefInvoiceNum"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("PurchaseOrderText", SqlDbType.VarChar)
                    .Expression = "TransInvoice.PurchaseOrderText"
                End With
                With .Add("FowstWideInvoiceNumber", SqlDbType.Int)
                    .Expression = "TransInvoice.FowstWideInvoiceNumber"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("SalesOrderNoRemoveDash", SqlDbType.VarChar) 'additional by janine search sales order no remove dash
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) + CONVERT(varchar, TransInvoice.SalesOrderNo)"
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "salesorder")
                End With
                With .Add("SalesOrderNoDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.SalesOrderNo)"
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "salesorder")
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE MobizLibraryOrganization.IsOrganization " &
                                  "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.InvoiceDate"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("DateNeeded", SqlDbType.DateTime)
                    .Expression = "TransInvoice.DateNeeded"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("Remarks", SqlDbType.VarChar)
                    .Expression = "TransInvoice.Remarks"
                    .Searchable = False
                End With
                With .Add("InternalRemarks", SqlDbType.VarChar)
                    .Expression = "TransInvoice.InternalRemarks"
                    .Searchable = False
                End With
                With .Add("PurchaseOrderDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.PurchaseOrderDate"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("DateCreated", SqlDbType.DateTime)
                    .Expression = "TransInvoice.DateCreated"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("SalesOrderDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.SalesOrderDate"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("Total", SqlDbType.Money)
                    .Expression = "tblItems.Total"
                    .Searchable = False
                End With
                With .Add("OrderedAmount", SqlDbType.Money)
                    .Expression = "tblItems.OrderedAmount"
                    .Searchable = False
                End With
                With .Add("ServedAmount", SqlDbType.Money)
                    .Expression = "tblItems.ServedAmount"
                    .Searchable = False
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "tblItems.SkuCount"
                    .Searchable = False
                End With
                With .Add("TotalOrder", SqlDbType.Money)
                    .Expression = "tblItemsOrder.TotalOrder"
                    .Searchable = False
                End With
                With .Add("SkuCountOrder", SqlDbType.Int)
                    .Expression = "tblItemsOrder.SkuCountOrder"
                    .Searchable = False
                End With
                With .Add("SalesOrderNo", SqlDbType.Int)
                    .Expression = "ISNULL(TransInvoice.SalesOrderNo,0)"
                    .Searchable = False
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("EncodingFOWSTName", SqlDbType.VarChar)
                    .Expression = "tblEncodingFowst.Name"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = "LibraryDevice.PhoneName"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.UserID"
                    .Searchable = False
                End With
                With .Add("EmployeeFullName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName"
                    .Searchable = True
                End With
                With .Add("CurrentUserID", SqlDbType.UniqueIdentifier)
                    .Expression = ReadyString(WebSession.Instance.PeopleUserID)
                    .Searchable = False
                End With
                With .Add("MIN", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("TableNo", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerTableNo"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("ApprovedDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.ApprovedDate"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CncDisapprovedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.CncDisapprovedByID"
                End With
                With .Add("SearchSO", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' +  CONVERT(varchar, ISNULL(TransInvoice.SalesOrderNo,0)) "
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("SearchSOnoDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) +  CONVERT(varchar, ISNULL(TransInvoice.SalesOrderNo,0)) "
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("DeliveryMethod", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryMethod"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("OrderMethod", SqlDbType.VarChar)
                    .Expression = "TransInvoice.OrderMethod"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CustomerDemographicGender", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerDemographicGender"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CustomerDemographicAge", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerDemographicAge"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("PurchaserName", SqlDbType.VarChar)
                    .Expression = "TransInvoice.PurchaserName"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("PurchaseOrderNo", SqlDbType.Int)
                    .Expression = "TransInvoice.PurchaseOrderNo"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("SearchPO", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' +  CONVERT(varchar, ISNULL(TransInvoice.PurchaseOrderNo,0)) "
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "purchaseorder")
                End With
                With .Add("SearchPOnoDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) +  CONVERT(varchar, ISNULL(TransInvoice.PurchaseOrderNo,0)) "
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "purchaseorder")
                End With
                With .Add("QuotationNo", SqlDbType.Int)
                    .Expression = "ISNULL(TransInvoice.QuotationNo, 0)"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("FullQuotationNum", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' +  CONVERT(varchar, ISNULL(TransInvoice.QuotationNo,0)) "
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "quote")
                End With
                With .Add("FullQuotationNumNoDash", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) +  CONVERT(varchar, ISNULL(TransInvoice.QuotationNo,0)) "
                    .Searchable = Not (pLimitSearchFields AndAlso CStr2(pTransType).Trim().ToLower() <> "quote")
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "TransInvoice.LastUpdated"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CnCUnapprovedReason", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CnCUnapprovedReason"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CncApprovedByID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.CncApprovedByID"
                    .Searchable = False
                End With
                With .Add("FinalizePickListByID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.FinalizePickListByID"
                    .Searchable = False
                End With
                With .Add("FinalizePickListTime", SqlDbType.DateTime2)
                    .Expression = "TransInvoice.FinalizePickListTime"
                    .Searchable = False
                End With
                With .Add("FulfillmentStatus", SqlDbType.VarChar)
                    .Expression = "CASE WHEN TransInvoice.FinalizePickListByID IS NULL THEN 'Draft'" & vbCrLf &
                        "   ELSE 'Finalized'" & vbCrLf &
                        "   END"
                    .Searchable = False
                End With
                With .Add("DocumentNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.DocumentNameID"
                End With
                With .Add("DocumentName", SqlDbType.VarChar)
                    .Expression = "LibraryDocumentControl.DocumentName"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CustomerFowst", SqlDbType.VarChar)
                    .Expression = "tblFowstCustomer.Name"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("CustomerBin", SqlDbType.VarChar)
                    .Expression = "tblBinCustomer.Name"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("BackorderSourceMinSalesOrderNo", SqlDbType.VarChar)
                    .Expression = "tblBackorderInvoice.BackorderSourceMinSalesOrderNo"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("BackorderSourceSalesOrderDate", SqlDbType.DateTime)
                    .Expression = "tblBackorderInvoice.BackorderSourceSalesOrderDate"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("IsCancelled", SqlDbType.Bit)
                    .Expression = "TransInvoice.IsCancelled"
                    .Searchable = False
                End With
                With .Add("TransType", SqlDbType.VarChar)
                    .Expression = "TransInvoice.TransType"
                    .Searchable = False
                End With
                With .Add("DeliveryAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
                        " + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf &
                        " + ',' + ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
                        " + ',' + ISNULL(tblDeliveryCountry.Name, '')" & vbCrLf
                    .Searchable = False
                End With
                With .Add("DeliveryStreet", SqlDbType.VarChar)
                    .Expression = "ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
                        " + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf
                    .Searchable = False
                End With
                With .Add("DeliveryCity", SqlDbType.VarChar)
                    .Expression = "ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
                        " + ',' + ISNULL(tblDeliveryCountry.Name, '')" & vbCrLf
                    .Searchable = False
                End With
                With .Add("DeliveryCityID", SqlDbType.VarChar)
                    .Expression = "tblDeliveryCity.ID" & vbCrLf
                    .Searchable = False
                End With
                With .Add("DeliveryEncodingDate", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryEncodingDate" & vbCrLf
                    .Searchable = False
                End With
                With .Add("DeliveryStatus", SqlDbType.VarChar)
                    .Expression = "ISNULL(TransInvoice.DeliveryStatus, CASE WHEN ISNULL(TransInvoice.IsDelivered, 0) = 0 THEN 'Undelivered' ELSE 'Delivered' END)" & vbCrLf
                    .Searchable = False
                End With
                With .Add("RoutingSheetNumber", SqlDbType.VarChar)
                    .Expression = "ISNULL(TransInvoice.RoutingSheetNumber, (SELECT TOP 1 RoutingSheetNumber FROM TransDeliveryLogger INNER JOIN TransDeliveryLoggerInvoices ON TransDeliveryLoggerInvoices.DeliveryLoggerID = TransDeliveryLogger.ID WHERE TransDeliveryLoggerInvoices.InvoiceID = TransInvoice.ID))" & vbCrLf
                End With
                With .Add("StrNum", SqlDbType.Int)
                    .Expression = "TransInvoice.StrNum"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("IsDelivered", SqlDbType.Bit)
                    .Expression = "TransInvoice.IsDelivered"
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("ParentSalesOrderNo", SqlDbType.VarChar, "tblParentOrder.ParentSalesOrderNo")
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("InvoiceSalesOrderNo", SqlDbType.VarChar, "tblInvoiceOrder.InvoiceSalesOrderNo")
                    .Searchable = Not pLimitSearchFields
                End With
                With .Add("ReceivedInStockReceiving", SqlDbType.Bit)
                    .Expression = "CASE WHEN EXISTS(SELECT TransReceiving.ID" & vbCrLf &
                        "   FROM TransReceiving" & vbCrLf &
                        "   WHERE TransReceiving.PurchaseOrderID = TransInvoice.ID)" & vbCrLf &
                        " THEN 1 ELSE 0 END"
                    .Searchable = False
                End With
                If pDisplayUnservedCount Then
                    With .Add("VendorPoSummaryFinalized", SqlDbType.Int, "tblVendorPoSummary.IsFinalized")
                        .Searchable = False
                    End With
                    With .Add("VendorPoSummaryNotFinalized", SqlDbType.Int, "tblVendorPoSummary.NotFinalized")
                        .Searchable = False
                    End With
                End If

                If CBool2(Common.WebRequest.Data("ShowCustomerContactInfo")) Then
                    With .Add("CustomerMobileNumbers", SqlDbType.VarChar)
                        .Expression = "(SELECT ISNULL(STUFF(" & vbCrLf &
                            " (SELECT TOP 100 PERCENT" & vbCrLf &
                            " ', ' + CASE WHEN LEN(orgmobile.ContactNo) <= 7 THEN CONVERT(varchar, prefix.Prefix) ELSE '' END + ISNULL(orgmobile.ContactNo, '')" & vbCrLf &
                            "  FROM MobizLibraryOrganizationContactMobile as orgmobile" & vbCrLf &
                            "  LEFT JOIN MobizLibraryCountries as ctry" & vbCrLf &
                            "    ON ctry.ID = orgmobile.CountryID" & vbCrLf &
                            "  LEFT JOIN MobizLibraryTelcoPrefixes as prefix" & vbCrLf &
                            "    ON prefix.ID = orgmobile.TelCoPrefixID" & vbCrLf &
                            "  WHERE orgmobile.OrganizationID = TransInvoice.CustomerID" & vbCrLf &
                            "  FOR XML PATH (''))" & vbCrLf &
                            "  , 1, 1, ''), ''))" & vbCrLf
                        .Searchable = False
                    End With
                    With .Add("CustomerTelephoneNumbers", SqlDbType.VarChar)
                        .Expression = "(SELECT ISNULL(STUFF(" & vbCrLf &
                            " (SELECT TOP 100 PERCENT" & vbCrLf &
                            " ', ' + ctry.CallingCode + areacode.CallingCode + orgtel.ContactNo" & vbCrLf &
                            "  FROM MobizLibraryOrganizationContactTel as orgtel" & vbCrLf &
                            "  LEFT JOIN MobizLibraryCountries as ctry" & vbCrLf &
                            "    ON ctry.ID = orgtel.CountryID" & vbCrLf &
                            "  LEFT JOIN MobizLibraryAreaCodes as areacode" & vbCrLf &
                            "    ON areacode.ID = orgtel.AreaCodeID" & vbCrLf &
                            "  WHERE orgtel.OrganizationID = TransInvoice.CustomerID" & vbCrLf &
                            "  FOR XML PATH (''))" & vbCrLf &
                            "  , 1, 1, ''), ''))" & vbCrLf
                        .Searchable = False
                    End With
                    With .Add("CustomerEmails", SqlDbType.VarChar)
                        .Expression = "(SELECT ISNULL(STUFF(" & vbCrLf &
                            " (SELECT TOP 100 PERCENT" & vbCrLf &
                            " ', ' + orgemail.EmailAddress" & vbCrLf &
                            "  FROM MobizLibraryOrganizationContactEmail as orgemail" & vbCrLf &
                            "  WHERE orgemail.OrganizationID = TransInvoice.CustomerID" & vbCrLf &
                            "  FOR XML PATH (''))" & vbCrLf &
                            "  , 1, 1, ''), ''))" & vbCrLf
                        .Searchable = False
                    End With
                End If
                If pHasUnapprovedReprintRequest Then
                    With .Add("RequestReprintID", SqlDbType.UniqueIdentifier)
                        .Expression = "tblReprintRequest.ID"
                        .Searchable = Not pLimitSearchFields
                    End With
                    With .Add("RequestReprintReason", SqlDbType.VarChar)
                        .Expression = "tblReprintRequest.Remarks"
                        .Searchable = Not pLimitSearchFields
                    End With
                    With .Add("RequestReprintDateRequested", SqlDbType.DateTime)
                        .Expression = "tblReprintRequest.DateRequested"
                        .Searchable = Not pLimitSearchFields
                    End With
                End If
                With .Add("PoReceivedPercent", SqlDbType.Decimal)
                    .Expression = "TransInvoice.PoReceivedPercent"
                    .Searchable = False
                End With
                With .Add("PrintCount", SqlDbType.Int)
                    .Expression = "TransInvoice.PrintCount"
                    .Searchable = Not pLimitSearchFields
                End With
            End With
            '.SqlFrom = " FROM TransInvoice" & vbCrLf & _
            '    " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf
            Dim pInvoiceTable = "TransInvoice"
            If pArchive Then
                pInvoiceTable = WebRequest.Instance.Database & "Archive.dbo." & pInvoiceTable
            End If
            .SqlFrom = " FROM " & pInvoiceTable & " TransInvoice LEFT JOIN" & vbCrLf &
                      " MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID" & vbCrLf &
                      " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN" & vbCrLf &
                      " MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                      " LEFT JOIN MobizLibraryOrganizationFOWST tblEncodingFowst ON TransInvoice.EncodingFowstID = tblEncodingFowst.ID" & vbCrLf &
                      " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
                      " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf
            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
                "   ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS OrderedAmount" & vbCrLf &
                "   ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS ServedAmount" & vbCrLf &
                "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
                " 	FROM TransInvoiceItems" & vbCrLf &
                "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                "   AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
                "   OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                "   AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
                " ) tblItems    " & vbCrLf
            .SqlFrom &= "  LEFT JOIN MobizLibraryOrganizationFOWST tblFowstCustomer ON tblFowstCustomer.ID = TransInvoice.CustomerFowstID" & vbCrLf &
                " LEFT JOIN LibraryFowstBin tblBinCustomer ON tblBinCustomer.ID = TransInvoice.CustomerBinID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransInvoice.DeliveryAddressID" & vbCrLf &
                " LEFT JOIN MobizLibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID" & vbCrLf &
                " LEFT JOIN MobizLibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID" & vbCrLf
            'additional by janine sept 5 2014 for sales order taker
            If pForPickingList Then
                .SqlFrom &= " CROSS "
            Else
                .SqlFrom &= " OUTER "
            End If
            .SqlFrom &= " APPLY (" & vbCrLf &
               " 	SELECT SUM(TransInvoiceItems.SubTotal) AS TotalOrder" & vbCrLf &
               "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCountOrder" & vbCrLf &
               " 	FROM TransInvoiceItems" & vbCrLf &
               "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
               "   AND TransInvoiceItems.OrderedQuantity > 0" & vbCrLf
            If pForPickingList Then
                .SqlFrom &= "    AND TransInvoiceItems.ProductID not IN (" & vbCrLf &
                    "    		SELECT TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
                    "         FROM TransInvoiceItemWarehouseWithdrawal" & vbCrLf &
                    "        JOIN TransInvoiceItemWarehouseSections ON TransInvoiceItemWarehouseSections.InvoiceID = TransInvoiceItemWarehouseWithdrawal.InvoiceID" & vbCrLf &
                    "        AND TransInvoiceItemWarehouseSections.ProductID = TransInvoiceItemWarehouseWithdrawal.ProductID" & vbCrLf &
                    "        AND TransInvoiceItemWarehouseSections.IsDone = 1" & vbCrLf &
                    "        WHERE TransInvoiceItemWarehouseWithdrawal.InvoiceID = TransInvoiceItems.InvoiceID" & vbCrLf
                If pSectionID <> "-" Then
                    .SqlFrom &= "        AND TransInvoiceItemWarehouseWithdrawal.SectionID = " & ReadyString(pSectionID) & vbCrLf
                End If
                .SqlFrom &= "    )" & vbCrLf
            End If
            .SqlFrom &= " ) tblItemsOrder    " & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                "  	SELECT 'SO-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblBackorderSalesOrder.SalesOrderNo) AS BackorderSourceMinSalesOrderNo" & vbCrLf &
                "     ,tblBackorderSalesOrder.SalesOrderDate AS BackorderSourceSalesOrderDate" & vbCrLf &
                "     FROM TransInvoice tblBackorderInvoice" & vbCrLf &
                "     JOIN TransInvoice tblBackorderSalesOrder ON tblBackorderSalesOrder.ID = tblBackorderInvoice.SalesOrderID" & vbCrLf &
                "     LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.id = tblBackorderSalesOrder.FowstID" & vbCrLf &
                "     LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
                "     WHERE tblBackOrderInvoice.ID = TransInvoice.BackorderSourceInvoiceID" & vbCrLf &
                "  ) tblBackorderInvoice"

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                "  	SELECT 'SO-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblParent.SalesOrderNo) AS ParentSalesOrderNo" & vbCrLf &
                "     FROM TransInvoice tblParent" & vbCrLf &
                "     LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.id = tblParent.FowstID" & vbCrLf &
                "     LEFT JOIN LibraryDevice ON tblParent.DeviceID = LibraryDevice.ID" & vbCrLf &
                "     WHERE tblParent.ID = TransInvoice.ParentOrderID" & vbCrLf &
                "  ) tblParentOrder" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                "  	SELECT 'SO-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblParent.SalesOrderNo) AS InvoiceSalesOrderNo" & vbCrLf &
                "     FROM TransInvoice tblParent" & vbCrLf &
                "     LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.id = tblParent.FowstID" & vbCrLf &
                "     LEFT JOIN LibraryDevice ON tblParent.DeviceID = LibraryDevice.ID" & vbCrLf &
                "     WHERE tblParent.ID = TransInvoice.SalesOrderID" & vbCrLf &
                "  ) tblInvoiceOrder" & vbCrLf
            If pHasUnapprovedReprintRequest Then
                .SqlFrom &= " CROSS APPLY (SELECT TransInvoicePrints.Remarks " & vbCrLf &
                    ",TransInvoicePrints.ID" & vbCrLf &
                    ",TransInvoicePrints.DateRequested" & vbCrLf &
                    " FROM TransInvoicePrints" & vbCrLf &
                    " WHERE TransInvoicePrints.ApprovedByID IS NULL" & vbCrLf &
                    " AND TransInvoicePrints.DeniedByID IS NULL" & vbCrLf &
                    " AND InvoiceID = TransInvoice.ID" & vbCrLf &
                    ") tblReprintRequest"
            End If

            If pPendingReprintRequest Then
                .SqlFrom &= "-- PendingReprintRequest" & vbCrLf
                .SqlFrom &= " JOIN TransInvoicePrints ON TransInvoicePrints.InvoiceID = TransInvoice.ID" & vbCrLf &
                    " AND TransInvoicePrints.DeniedByID IS NULL" & vbCrLf &
                    " AND TransInvoicePrints.ApprovedByID IS NULL" & vbCrLf
            End If
            If pDisplayUnservedCount Then
                .SqlFrom &= " CROSS APPLY (" & vbCrLf &
                    " 	SELECT SUM(crossPo.IsFinalized) AS IsFinalized" & vbCrLf &
                    "     ,SUM(crossPo.NotFinalized) AS NotFinalized" & vbCrLf &
                    "     FROM TransInvoiceItems" & vbCrLf &
                    "     CROSS APPLY (" & vbCrLf &
                    "     	SELECT tblPo.CustomerID" & vbCrLf &
                    "         ,CASE WHEN tblPo.FinalizePickListByID IS NOT NULL THEN 1" & vbCrLf &
                    "         ELSE 0" & vbCrLf &
                    "         END AS IsFinalized" & vbCrLf &
                    "         ,CASE WHEN tblPo.FinalizePickListByID IS NOT NULL THEN 0" & vbCrLf &
                    "         ELSE 1" & vbCrLf &
                    "         END AS NotFinalized" & vbCrLf &
                    "         FROM TransInvoice tblPo" & vbCrLf &
                    "         WHERE tblPo.ID = TransInvoiceItems.PurchaseOrderID" & vbCrLf &
                    "     ) crossPo" & vbCrLf &
                    "     WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                    " ) tblVendorPoSummary"
            End If

            .SqlWhere = " WHERE 1=1" & vbCrLf
            If CBool2(Common.WebRequest.Data("IsCancelled")) Then
                .SqlWhere &= " AND TransInvoice.IsCancelled = 1" & vbCrLf
            Else
                .SqlWhere &= " AND TransInvoice.IsCancelled = 0" & vbCrLf
            End If
            If pAnyZero = False Then
                .SqlWhere &= " AND TransInvoice.InvoiceNumber " & If(pNonZero, "<>", "=") & " 0" & vbCrLf
            End If
            If pAllowNoItems = False Then
                .SqlWhere &= " AND EXISTS(SELECT TransInvoiceItems.ID" & vbCrLf &
                    "   FROM TransInvoiceItems" & vbCrLf &
                    "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)" & vbCrLf
            End If
            If IsGuid(pCustomerID) Then
                .SqlWhere &= " AND TransInvoice.CustomerID = " & ReadyString(ReadyGuid(pCustomerID)) & vbCrLf
            End If
            If IsGuid(pUserID) Then
                .SqlWhere &= " AND TransInvoice.UserID = " & ReadyString(ReadyGuid(pUserID)) & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("FinalizedPoOnly")) Then
                .SqlWhere &= " AND (ISNULL(TransInvoice.PurchaseOrderNo, 0) " & IIf(CBool2(Common.WebRequest.Data("IsForFollowUp")), "=", "<>") & " 0 " & vbCrLf
                If CBool2(Common.WebRequest.Data("IncludeFinalizedInvoices")) Then
                    .SqlWhere &= " OR (ISNULL(TransInvoice.InvoiceNumber, 0) <> 0 " & vbCrLf &
                        " AND TransInvoice.CustomerID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        ")" & vbCrLf
                End If
                .SqlWhere &= " ) " & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("NoReceiving")) Then
                '.SqlWhere &= " AND NOT EXISTS(SELECT TransReceiving.ID" & vbCrLf & _
                '    "   FROM TransReceiving" & vbCrLf & _
                '    "   WHERE TransReceiving.PurchaseOrderID = TransInvoice.ID)" & vbCrLf
                .SqlWhere &= " AND ISNULL(TransInvoice.PoReceivedPercent, 0) < 100"
            End If
            If CBool2(Common.WebRequest.Data("HasReceiving")) Then
                .SqlWhere &= " AND EXISTS(SELECT TransReceiving.ID" & vbCrLf &
                    "   FROM TransReceiving" & vbCrLf &
                    "   WHERE TransReceiving.PurchaseOrderID = TransInvoice.ID)" & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("SalesOrder")) Then
                .SqlWhere &= " AND TransInvoice.SalesOrderNo != 0" & vbCrLf
                If pForPickingList Then
                    .SqlWhere &= " AND TransInvoice.ApprovedDate IS NOT NULL" & vbCrLf &
                        " AND tblItemsOrder.TotalOrder > 0" & vbCrLf & vbCrLf &
                        " AND TransInvoice.SopaApprovedByID IS NOT NULL" & vbCrLf

                    ' show only if current section's fowst is included for picking (choose via MobizCR -> Dashboard -> Sales Order Processing & Analytics)
                    .SqlWhere &= "   AND " & vbCrLf &
                        "   	(" & vbCrLf &
                        "     	  EXISTS " & vbCrLf &
                        "         (" & vbCrLf &
                        "             SELECT *" & vbCrLf &
                        "           	FROM TransInvoiceItemsPickingFowsts" & vbCrLf &
                        "           	JOIN MobizLibraryOrganizationFOWSTSection ON MobizLibraryOrganizationFOWSTSection.FowstID = TransInvoiceItemsPickingFowsts.FowstID" & vbCrLf &
                        "           	WHERE TransInvoiceItemsPickingFowsts.InvoiceID = TransInvoice.ID" & vbCrLf
                    If pSectionID <> "-" Then
                        .SqlWhere &= "               AND MobizLibraryOrganizationFOWSTSection.ID = " & ReadyString(pSectionID) & vbCrLf
                    End If
                    .SqlWhere &= "   		)  " & vbCrLf &
                        "         " & vbCrLf &
                        "         OR " & vbCrLf &
                        "         -- there more invoice items than there are item picking fowst" & vbCrLf &
                        "         (" & vbCrLf &
                        "         	(" & vbCrLf &
                        "               SELECT COUNT(*)" & vbCrLf &
                        "               FROM TransInvoiceItemsPickingFowsts" & vbCrLf &
                        "               WHERE TransInvoiceItemsPickingFowsts.InvoiceID = TransInvoice.ID" & vbCrLf &
                        " 			) " & vbCrLf &
                        "             < " & vbCrLf &
                        " 			(" & vbCrLf &
                        "               SELECT COUNT(*)" & vbCrLf &
                        "               FROM TransInvoiceItems" & vbCrLf &
                        "               WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                        "             )" & vbCrLf &
                        "         )" & vbCrLf &
                        "   	)  "
                End If
            End If
            If pForInvoicePrinting Then
                .SqlWhere &= " AND TransInvoice.DucInvoiceNum > 0" & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("FilterQuotations")) Then
                .SqlWhere &= " AND TransInvoice.QuotationNo <> 0" & vbCrLf
            End If

            If CBool2(Common.WebRequest.Data("FilterSalesOrders")) _
                AndAlso CStr2(pTransType).Trim().ToLower() = "salesorder" _
                AndAlso CBool2(Common.WebRequest.Data("ExcludeInvoiced")) Then

                .SqlWhere &= " AND ( " & vbCrLf &
                    " (TransInvoice.SalesOrderNo <> 0" & vbCrLf &
                    " AND TransInvoice.TransType = 'SalesOrder'" & vbCrLf &
                    " ) OR (TransInvoice.PurchaseOrderNo <> 0" & vbCrLf &
                    " AND (TransInvoice.TransType = 'PurchaseOrder' OR TransInvoice.TransType IS NULL)" & vbCrLf &
                    " AND TransInvoice.CustomerID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                    " ) OR TransInvoice.TransType = 'StockTransfer'" & vbCrLf &
                    ")" & vbCrLf

                .SqlWhere &= " AND TransInvoice.InvoiceID IS NULL" & vbCrLf &
                    " AND TransInvoice.ID NOT IN (" & vbCrLf &
                    " SELECT tblInvoice.SalesOrderID" & vbCrLf &
                    " FROM TransInvoice tblInvoice" & vbCrLf &
                    " WHERE tblInvoice.SalesOrderID IS NOT NULL" & vbCrLf &
                    " AND tblInvoice.TransType = 'Invoice'" & vbCrLf &
                    ")" & vbCrLf
            Else
                If CBool2(Common.WebRequest.Data("FilterSalesOrders")) Then
                    .SqlWhere &= " AND TransInvoice.SalesOrderNo <> 0" & vbCrLf
                End If
                If pTransTypes = "PurchaseOrderAndInvoice" Then
                    If String.IsNullOrWhiteSpace(pFilterPoAndInvoice) Then
                        .SqlWhere &= "  AND (TransInvoice.TransType = 'PurchaseOrder' or TransInvoice.TransType = 'Invoice')" & vbCrLf
                    ElseIf pFilterPoAndInvoice = "po" Then
                        .SqlWhere &= "  AND (TransInvoice.TransType = 'PurchaseOrder')" & vbCrLf
                    ElseIf pFilterPoAndInvoice = "invoice" Then
                        .SqlWhere &= "  AND (TransInvoice.TransType = 'Invoice')" & vbCrLf
                    End If
                End If
                If String.IsNullOrWhiteSpace(pTransType) = False Then
                    If pInvoiceSource = "pos" Then
                        .SqlWhere &= " AND ISNULL(TransInvoice.TransType, '') = ''" & vbCrLf
                    Else
                        .SqlWhere &= " AND TransInvoice.TransType = " & ReadyString(pTransType) & vbCrLf
                    End If

                    If CBool2(Common.WebRequest.Data("IsForFollowUp")) Then
                        If pTransType = "PurchaseOrder" Then
                            .SqlWhere &= " AND ISNULL(TransInvoice.PurchaseOrderNo, 0) = 0  AND TransInvoice.IsForFollowUp = 1" & vbCrLf
                        End If
                    End If
                End If
                If CBool2(Common.WebRequest.Data("ExcludeInvoiced")) Then
                    .SqlWhere &= " AND TransInvoice.InvoiceID IS NULL" & vbCrLf &
                        " AND TransInvoice.ID NOT IN (" & vbCrLf &
                        " SELECT tblSalesOrders.SalesOrderID" & vbCrLf &
                        " FROM TransInvoice tblSalesOrders" & vbCrLf &
                        " WHERE tblSalesOrders.SalesOrderID IS NOT NULL" & vbCrLf &
                        " AND tblSalesOrders.TransType = 'Invoice'" & vbCrLf &
                        ")" & vbCrLf
                End If
            End If

            Dim pTransactionType = Common.WebRequest.Data("TransactionType")
            If pWithCnCUnapprovedReason Then
                .SqlWhere &= " AND TransInvoice.CncApprovedByID IS NULL" & vbCrLf
            ElseIf pWithApprovedCnC Then
                .SqlWhere &= " AND TransInvoice.CncApprovedByID IS NOT NULL" & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("IsCancelled")) = False Then
                If pWithCnCDisapproved Then
                    .SqlWhere &= " AND CncDisapprovedByID IS NOT NULL" & vbCrLf
                Else
                    .SqlWhere &= " AND CncDisapprovedByID IS NULL" & vbCrLf
                End If
            End If
            If pWithCnCUnapprovedReason OrElse pWithApprovedCnC Then
                Dim pUnapprovedWhere As String
                .SqlWhere &= " AND ISNULL(TransInvoice.CnCUnapprovedReason, '') <> ''" & vbCrLf

                If String.IsNullOrWhiteSpace(pTransactionType) OrElse pTransactionType = "so" Then
                    pUnapprovedWhere &= " OR (" & vbCrLf &
                    "       TransInvoice.TransType = 'SalesOrder'" & vbCrLf &
                    "       AND TransInvoice.SalesOrderNo > 0" & vbCrLf &
                    "   )" & vbCrLf
                End If

                If String.IsNullOrWhiteSpace(pTransactionType) OrElse pTransactionType = "po" Then
                    pUnapprovedWhere &= " OR -- commented on oct 29, 2020 " & vbCrLf &
                        "   (" & vbCrLf &
                        "       --TransInvoice.TransType = 'PurchaseOrder' -- dont require TransType" & vbCrLf &
                        "       TransInvoice.PurchaseOrderNo > 0" & vbCrLf &
                        "   ) " & vbCrLf
                End If

                If String.IsNullOrWhiteSpace(pTransactionType) OrElse pTransactionType = "invoices" Then
                    pUnapprovedWhere &= " OR " & vbCrLf &
                        "   (" & vbCrLf &
                        "       TransInvoice.TransType = 'Invoice'" & vbCrLf &
                        "       AND ISNULL(TransInvoice.FinalizedForApproval, 0) = 1" & vbCrLf &
                        "       AND TransInvoice.InvoiceNumber = 0" & vbCrLf &
                        "   )" & vbCrLf
                End If

                If String.IsNullOrWhiteSpace(pUnapprovedWhere) = False Then
                    .SqlWhere &= " AND (1=0" & vbCrLf & pUnapprovedWhere
                    .SqlWhere &= ")" & vbCrLf
                End If
            End If

            If CBool2(Common.WebRequest.Data("ShowChildSos")) = False Then
                .SqlWhere &= " AND TransInvoice.ParentOrderID IS NULL" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(pDeliveryCityID) = False Then
                .SqlWhere &= " AND tblDeliveryCity.ID = " & ReadyString(pDeliveryCityID) & vbCrLf
            End If
            If pForInvoicing Then
                .SqlWhere &= " AND (TransInvoice.CnCUnapprovedReason IS NULL" & vbCrLf &
                    " OR TransInvoice.CncApprovedByID IS NOT NULL)" & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("UneditedBackorder")) Then
                .SqlWhere &= " AND ISNULL(TransInvoice.UneditedBackorder, 0) = 1" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(pInvoiceFowstID) = False Then
                '.SqlWhere &= " AND ISNULL(TransInvoice.InvoiceFowstID, TransInvoice.FowstID) = " & ReadyString(pInvoiceFowstID) & vbCrLf ' old code 2024_01_05__11_31_49_3444
                .SqlWhere &= " AND ISNULL(TransInvoice.FowstID, TransInvoice.InvoiceFowstID) = " & ReadyString(pInvoiceFowstID) & vbCrLf
            End If
            Dim pDateFrom As String = Common.WebRequest.Data("DateFrom")
            If IsDate(pDateFrom) = False Then
                pDateFrom = Nothing
            End If
            If pDateFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.InvoiceDate) >= " & ReadyString(Format(ParseDate(pDateFrom), "yyyy-MM-dd")) & vbCrLf
            End If
            Dim pDateTo As String = Common.WebRequest.Data("DateTo")
            If IsDate(pDateTo) = False Then
                pDateTo = Nothing
            End If
            If pDateTo IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.InvoiceDate) <= " & ReadyString(Format(ParseDate(pDateTo), "yyyy-MM-dd")) & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("FinalizedFcOnly")) Then
                .SqlWhere &= " AND TransInvoice.FinalizePickListByID IS NOT NULL" & vbCrLf
            End If
            If CBool2(Common.WebRequest.Data("ExcludeFinalizedFc")) Then
                .SqlWhere &= " AND TransInvoice.FinalizePickListByID IS NULL" & vbCrLf
            End If

            If pDraftStatus = "draft" Then
                .SqlWhere &= " AND ISNULL(TransInvoice.PurchaseOrderNo, 0) = 0" & vbCrLf
            ElseIf pDraftStatus = "finalized" Then
                .SqlWhere &= " AND TransInvoice.PurchaseOrderNo <> 0" & vbCrLf
            End If

            If pApprovedStatus = "unapproved" Then
                .SqlWhere &= " AND TransInvoice.ApprovedDate IS NULL" & vbCrLf
            ElseIf pApprovedStatus = "approved" Then
                .SqlWhere &= " AND TransInvoice.ApprovedDate IS NOT NULL" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(pXAxisName) = False _
                OrElse String.IsNullOrWhiteSpace(pXAxisValue) = False _
                OrElse String.IsNullOrWhiteSpace(pYAxisName) = False _
                OrElse String.IsNullOrWhiteSpace(pYAxisValue) = False Then

                .MaxRecords = 0

                Dim pDateEncodedFrom As String = Common.WebRequest.Data("DateEncodedFrom")
                If IsDate(pDateEncodedFrom) = False Then
                    pDateEncodedFrom = Nothing
                End If
                Dim pDateEncodedTo As String = Common.WebRequest.Data("DateEncodedTo")
                If IsDate(pDateEncodedTo) = False Then
                    pDateTo = Nothing
                End If
                Dim pXAxis As TransInvoice.eCrossTabAxis
                Dim pYAxis As TransInvoice.eCrossTabAxis
                If [Enum].TryParse(Of TransInvoice.eCrossTabAxis)(pXAxisName, True, pXAxis) = False Then
                    Return JsonDictionary.Serialize("Error", "XAxis must be out of the string in TransInvoice.eCrossTabAxis. ref: vb4EPPT2SGLK!")
                End If
                If [Enum].TryParse(Of TransInvoice.eCrossTabAxis)(pYAxisName, True, pYAxis) = False Then
                    Return JsonDictionary.Serialize("Error", "YAxis must be out of the string in TransInvoice.eCrossTabAxis. ref: vbMLP84JLXV3!")
                End If
                Dim pCrossTabWhere = TransInvoice.CrossTabReportSql(
                    vXAxis:=pXAxis,
                    vYAxis:=pYAxis,
                    vXAxisValue:=pXAxisValue,
                    vYAxisValue:=pYAxisValue,
                    vInvoiceDateFrom:=CDateNull(pDateFrom),
                    vInvoiceDateTo:=CDateNull(pDateTo),
                    vDateNeededFrom:=CDateNull(pDateNeededFrom),
                    vDateNeededTo:=CDateNull(pDateNeededTo),
                    vDateEncodedFrom:=CDateNull(pDateEncodedFrom),
                    vDateEncodedTo:=CDateNull(pDateEncodedTo))
                .SqlWhere &= " AND TransInvoice.ID IN (" & pCrossTabWhere & ")" & vbCrLf
            End If
            If pExcludeDelivered Then
                .SqlWhere &= " AND ISNULL(TransInvoice.IsDelivered, 0) = 0" & vbCrLf
            ElseIf pExcludeUndelivered Then
                .SqlWhere &= " AND ISNULL(TransInvoice.IsDelivered, 0) = 1" & vbCrLf
            End If
            If Not String.IsNullOrWhiteSpace(pDeliveryStatus) Then
                .SqlWhere &= " AND ISNULL(TransInvoice.DeliveryStatus, CASE WHEN ISNULL(TransInvoice.IsDelivered, 0) = 0 THEN 'Undelivered' ELSE 'Delivered' END) = " & ReadyString(pDeliveryStatus) & vbCrLf
            End If
            If pLastUpdatedDateFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.LastUpdated) >= " & ReadyString(Format(pLastUpdatedDateFrom, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If pLastUpdatedDateTo IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.LastUpdated) <= " & ReadyString(Format(pLastUpdatedDateTo, cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If pDateNeededFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.DateNeeded) >= " & ReadyString(Format(CDate(pDateNeededFrom), cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If pDateNeededTo IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.DateNeeded) <= " & ReadyString(Format(CDate(pDateNeededTo), cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If pSalesOrderDateFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.SalesOrderDate) >= " & ReadyString(Format(CDate(pSalesOrderDateFrom), cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If pSalesOrderDateTo IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.SalesOrderDate) <= " & ReadyString(Format(CDate(pSalesOrderDateTo), cGlobal.FormatDateTimeSql)) & vbCrLf
            End If
            If pBackordersOnly Then
                .SqlWhere &= " AND ISNULL(TransInvoice.SalesOrderNo, 0) = 0 " & vbCrLf &
                    " AND TransInvoice.BackorderSourceInvoiceID IS NOT NULL" & vbCrLf
            End If
            If pExcludeBackorders Then
                .SqlWhere &= " AND (ISNULL(TransInvoice.SalesOrderNo, 0) > 0 " & vbCrLf &
                    " OR TransInvoice.BackorderSourceInvoiceID IS NULL)" & vbCrLf
            End If
            'If pDisplayUnservedCount Then
            '    .SqlWhere &= " AND tblVendorPoSummary.IsFinalized + tblVendorPoSummary.NotFinalized > 0" & vbCrLf
            'End If
            If Not String.IsNullOrWhiteSpace(pDeliveryTruckFowstID) Then
                .SqlWhere &= " AND TransInvoice.DeliveryTruckFowstID = " & ReadyString(pDeliveryTruckFowstID) & vbCrLf
            End If
            If Not String.IsNullOrWhiteSpace(pDelivery3PlCompanyID) Then
                .SqlWhere &= " AND TransInvoice.Delivery3PlCompanyID = " & ReadyString(pDelivery3PlCompanyID) & vbCrLf
            End If
            If Not String.IsNullOrWhiteSpace(pDeliveryCustomerRepresentative) Then
                .SqlWhere &= " AND TransInvoice.DeliveryCustomerRepresentative LIKE " & ReadyStringWild(pDeliveryCustomerRepresentative) & vbCrLf
            End If
            If Not String.IsNullOrWhiteSpace(CStr2(Common.WebRequest.Data("DeliveryEncodingDate"))) Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.DeliveryEncodingDate) = " & ReadyString(Format(ParseDate(Common.WebRequest.Data("DeliveryEncodingDate")), "yyyy-MM-dd")) & vbCrLf
            End If
            If Not String.IsNullOrWhiteSpace(CStr2(Common.WebRequest.Data("DeliveryEncodingDateFrom"))) And Not String.IsNullOrWhiteSpace(CStr2(Common.WebRequest.Data("DeliveryEncodingDateTo"))) Then
                .SqlWhere &= " AND CONVERT(date, TransInvoice.DeliveryEncodingDate) BETWEEN " & ReadyString(Format(ParseDate(Common.WebRequest.Data("DeliveryEncodingDateFrom")), "yyyy-MM-dd")) & " AND " & ReadyString(Format(ParseDate(Common.WebRequest.Data("DeliveryEncodingDateTo")), "yyyy-MM-dd")) & vbCrLf
            End If
            If Not String.IsNullOrWhiteSpace(pRoutingSheetNumber) Then
                .SqlWhere &= " AND TransInvoice.RoutingSheetNumber = " & ReadyString(pRoutingSheetNumber) & vbCrLf
            End If
            If pExcludeCashPaymentCustomer Then
                'Customer: [Cash Payment]
                .SqlWhere &= " AND TransInvoice.CustomerID <> " & ReadyString("cd71a0f7-08a9-4601-876e-5f13c064f873") & vbCrLf
            End If
            If pNonZeroServedSkuCount Then
                .SqlWhere &= " AND tblItems.SkuCount > 0" & vbCrLf
            End If
            .SqlOrder = " ORDER BY TransInvoice.LastUpdated DESC" & vbCrLf &
                ",TransInvoice.InvoiceDate DESC"

        End With

        mFinder.ProData.CommandTimeout = 3 * 60 * 1000
        If pWithCnCUnapprovedReasonSummary Then
            Dim pJsonDictionary = New JsonDictionary()
            mFinder.MaxRecords = Integer.MaxValue
            Dim pSql = mFinder.CreateSql
            pSql = "SELECT SUM(TotalOrder) AS TotalOrder" & vbCrLf & vbCrLf &
                ",COUNT(*) AS Count	" & vbCrLf &
                " FROM(" & pSql & ") t"
            Dim pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
            pJsonDictionary.Add("Result", pDR)
            Return pJsonDictionary.Serialize()
        ElseIf CBool2(Common.WebRequest.Data("CountOnly")) Then
            mFinder.MaxRecords = Integer.MaxValue
            Dim pSql = mFinder.CreateSql
            pSql = "SELECT COUNT(*)" & vbCrLf &
                " FROM (" & pSql & ") t"
            Return JsonDictionary.Serialize("Result", CInt2(WebRequest.Instance.ProData.ExecuteScalar(pSql)))
        Else
            Return SubmitFinder()
        End If

    End Function

    <HttpPost>
    Public Function TransInvoiceFromCache() As String

        Dim pInvoiceID As String ' optional
        Dim pCustomerID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        pCustomerID = Common.WebRequest.Data("CustomerID")

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "CacheInvoices.ID")
                .Add("InvoiceNumber", SqlDbType.VarChar, "CacheInvoices.InvoiceNumber")
                .Add("InvoiceDate", SqlDbType.DateTime, "CacheInvoices.InvoiceDate")
                .Add("CompanyName", SqlDbType.DateTime, "MobizLibraryOrganization.CompanyName")
            End With
            .SqlFrom = " FROM CacheInvoices" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = CacheInvoices.CustomerID" & vbCrLf
            .SqlWhere = " WHERE 1=1" & vbCrLf
            If IsGuid(pInvoiceID) Then
                .SqlWhere &= " AND CacheInvoices.ID = " & ReadyString(pInvoiceID) & vbCrLf
            End If
            If IsGuid(pCustomerID) Then
                .SqlWhere &= " AND CacheInvoices.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
            End If
            .SqlOrder = " ORDER BY InvoiceDate"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransInvoicesForReprint() As String
        Dim strFOWSTID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        strFOWSTID = Common.WebRequest.Data("FOWSTID")

        With CreateFinder()
            .MaxRecords = 5
            .SearchString = ""
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.ID"
                    .Searchable = True
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.CustomerID"
                    .Searchable = True
                End With
                With .Add("InvoiceNum", SqlDbType.VarChar)
                    .Expression = "RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7)"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE MobizLibraryOrganization.IsOrganization " &
                                  "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.InvoiceDate"
                End With
                With .Add("Total", SqlDbType.Money)
                    .Expression = "(SELECT SUM(SubTotal) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)"
                    .Searchable = False
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "(SELECT COUNT(ID) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)"
                    .Searchable = False
                End With
                With .Add("SalesOrderNo", SqlDbType.Int)
                    .Expression = "ISNULL(SalesOrderNo,0)"
                    .Searchable = False
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = "LibraryDevice.PhoneName"
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.UserID"
                    .Searchable = False
                End With
                With .Add("EmployeeFullName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName"
                    .Searchable = False
                End With
                With .Add("CurrentUserID", SqlDbType.UniqueIdentifier)
                    .Expression = ReadyString(WebSession.Instance.PeopleUserID)
                    .Searchable = False
                End With
                With .Add("MIN", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)"
                    .Searchable = False
                End With
                With .Add("TableNo", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerTableNo"
                End With
            End With
            .SqlFrom = " FROM TransInvoice INNER JOIN" & vbCrLf &
                      " MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID INNER JOIN" & vbCrLf &
                      " LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN" & vbCrLf &
                      " MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                      " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID "
            .SqlWhere = " WHERE TransInvoice.IsCancelled = 0 " & vbCrLf &
                " AND TransInvoice.InvoiceNumber > 0 " & vbCrLf &
                " AND TransInvoice.UserID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                " AND TransInvoice.FowstID = " & ReadyString(strFOWSTID)

            .SqlOrder = " ORDER BY TransInvoice.InvoiceDate DESC" & vbCrLf &
                ",TransInvoice.LastUpdated DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransInvoicesReturnables(Optional ByVal vIsAllDeviceInFowst As Boolean = False) As String
        Dim pDT As DataTable

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pCustomerID As String
        Dim pExcludeMemoID As String
        Dim pWhereMemo As String
        Dim pFastMode As Boolean = CBool2(Common.WebRequest.Data("FastMode")) ' dont join memo tables
        Dim pRestrictedTransactionLookupForSalesReturn As Boolean = CBool2(Common.WebRequest.Data("RestrictedTransactionLookupForSalesReturn"))
        Dim pInvoiceAmount = CDec2(Common.WebRequest.Data("InvoiceAmount"))
        Dim pFowstID As String
        Dim pDeviceID As String
        Dim pAllowOfflineSrvPayments As Boolean = WebRequest.Instance.Business.SystemDefaults.AllowOfflineSrvPayments

        pCustomerID = Common.WebRequest.Data("CustomerID")
        pExcludeMemoID = Common.WebRequest.Data("MemoID")
        If IsGuid(pExcludeMemoID) Then
            pExcludeMemoID = ReadyGuid(pExcludeMemoID)
            pWhereMemo = " AND TransMemo.ID <> " & ReadyString(pExcludeMemoID) & vbCrLf
        End If

        If pAllowOfflineSrvPayments Then
            pFowstID = WebRequest.Instance.Data("FowstID")
            If CStr2(pFowstID) = "" Then
                Return JsonDictionary.Serialize("Error", "FowstID is required if Allow offline SRV payments is set. This fix this, please update to the latest version of Mobiz Complete Retail.")
            End If
            pDeviceID = WebRequest.Instance.Data("DeviceID")
            If CStr2(pDeviceID) = "" Then
                Return JsonDictionary.Serialize("Error", "DeviceID is required if Allow offline SRV payments is set. This fix this, please update to the latest version of Mobiz Complete Retail.")
            End If
        End If

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.ID"
                    .Searchable = False
                End With
                With .Add("InvoiceNum", SqlDbType.VarChar)
                    ' 12 chars max kay 12 chars max pud sa POS print
                    .Expression = "RIGHT('000000000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 12)"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE MobizLibraryOrganization.IsOrganization " &
                                  "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.InvoiceDate"
                End With
                With .Add("Total", SqlDbType.Money)
                    If pFastMode Then
                        .Expression = "SUM(TransInvoice.TotalAmount)"
                    Else
                        .Expression = "SUM(tblItems.Total)"
                    End If
                    .Searchable = False
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    If pFastMode Then
                        .Expression = "NULL"
                    Else
                        .Expression = "SUM(tblItems.SkuCount)"
                    End If
                    .Searchable = False
                End With
                With .Add("Returnables", SqlDbType.Int)
                    If pFastMode Then
                        .Expression = "NULL"
                    Else
                        .Expression = "SUM(tblItems.Returnables)"
                    End If
                    .Searchable = False
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = "LibraryDevice.PhoneName"
                End With
                With .Add("OrderByFowst", SqlDbType.Int)
                    .Expression = "	CASE TransInvoice.FOWSTID " &
                        " WHEN " & ReadyString(WebRequest.Instance.FowstID) & " THEN 0 " &
                        " ELSE " &
                        "   1 " &
                        " END "
                End With
                'commented by janine nov 24 2014 . do not check device.
                'With .Add("OrderByDevice", SqlDbType.Int)
                '    .Expression = "	CASE TransInvoice.DeviceID " &
                '        " WHEN " & ReadyString(WebRequest.Instance.DeviceID) & " THEN 0 " &
                '        " ELSE " &
                '        "   1 " &
                '        " END "
                'End With
            End With
            .SqlFrom = " FROM TransInvoice" & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                " INNER JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
                " " & vbCrLf
            If pFastMode = False Then
                .SqlFrom &= " CROSS APPLY" & vbCrLf &
                    " 	(SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
                    " 	,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
                    "   ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) - ISNULL(tblReturns.QuantityReturned, 0)  - ISNULL(tblReturns.QuantityBad, 0) AS Returnables" & vbCrLf &
                    "   FROM TransInvoiceItems" & vbCrLf &
                    "   " & vbCrLf &
                    " 	OUTER APPLY " & vbCrLf &
                    "     	(SELECT SUM(TransMemoSalesReturn.QuantityReturned * TransMemoSalesReturn.ActualQuantity) AS QuantityReturned" & vbCrLf &
                    "     	,SUM(TransMemoSalesReturn.QuantityBad * TransMemoSalesReturn.ActualQuantity) AS QuantityBad" & vbCrLf &
                    "     	FROM TransMemoSalesReturn" & vbCrLf &
                    "       JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf &
                    " 		WHERE TransMemo.IsCancelled = 0" & vbCrLf &
                    "       AND TransMemoSalesReturn.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
                    "       AND TransMemoSalesReturn.InvoiceID = TransInvoiceItems.InvoiceID" & vbCrLf &
                    "       AND TransMemo.ApprovedBy IS NOT NULL" & vbCrLf &
                    pWhereMemo &
                    " 	) AS tblReturns" & vbCrLf &
                    "   " & vbCrLf &
                    " 	WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                    "   AND TransInvoiceItems.Quantity > 0" & vbCrLf &
                    " 	GROUP BY tblReturns.QuantityReturned, tblReturns.QuantityBad" & vbCrLf &
                    "   HAVING SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) > ISNULL(tblReturns.QuantityReturned, 0)" & vbCrLf &
                    " ) AS tblItems" & vbCrLf &
                    " " & vbCrLf
            End If

            .SqlWhere = " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
                " AND TransInvoice.InvoiceNumber <> 0 " & vbCrLf
            If pFastMode = False Then
                .SqlWhere &= " AND tblItems.Returnables > 0" & vbCrLf
            End If
            .SqlWhere &= " AND TransInvoice.FOWSTID = " & ReadyString(WebRequest.Instance.FowstID) & vbCrLf
            If pRestrictedTransactionLookupForSalesReturn Then
                .SqlWhere &= " AND TransInvoice.InvoiceNumber = " & ReadyString(Common.WebRequest.Data("keywords")) & vbCrLf &
                    " AND TransInvoice.AmountDueNetofCharges - TransInvoice.SCDiscount - TransInvoice.TotalNonSCDiscount >= " & Math.Floor(pInvoiceAmount) & vbCrLf &
                    " AND TransInvoice.AmountDueNetofCharges - TransInvoice.SCDiscount - TransInvoice.TotalNonSCDiscount < " & Math.Floor(pInvoiceAmount) + 1 & vbCrLf
            Else
                If IsDate(Common.WebRequest.Data("DateFrom")) Then
                    .SqlWhere &= " AND CONVERT(date, TransInvoice.InvoiceDate) >= " & ReadyString(CDate(Common.WebRequest.Data("DateFrom")).Date) & vbCrLf
                End If
                If IsDate(Common.WebRequest.Data("DateTo")) Then
                    .SqlWhere &= " AND CONVERT(date, TransInvoice.InvoiceDate) <= " & ReadyString(CDate(Common.WebRequest.Data("DateTo")).Date) & vbCrLf
                End If
            End If

            'commented by janine nov 24 2014 . do not check device.
            ''   IIf(Not vIsAllDeviceInFowst, " AND TransInvoice.DeviceID = " & ReadyString(WebRequest.Instance.DeviceID), "")

            'If (CStr2(Common.WebRequest.Data("keywords")) <> "") Then
            '    .SqlWhere &= " AND TransInvoice.InvoiceNumber = " & ReadyString(Common.WebRequest.Data("keywords"))
            'End If

            If IsGuid(pCustomerID) Then
                .SqlWhere &= " AND TransInvoice.CustomerID = " & ReadyString(ReadyGuid(pCustomerID)) & vbCrLf
            End If
            If IsGuid(pFowstID) Then
                .SqlWhere &= " AND TransInvoice.FowstID = " & ReadyString(ReadyGuid(pFowstID)) & vbCrLf
            End If
            If IsGuid(pDeviceID) Then
                .SqlWhere &= " AND TransInvoice.DeviceID = " & ReadyString(ReadyGuid(pDeviceID)) & vbCrLf
            End If
            .SqlGroup = " GROUP BY TransInvoice.ID " & vbCrLf &
                " ," & .Fields("InvoiceNum").Expression & vbCrLf &
                " , (CASE MobizLibraryOrganization.IsOrganization " &
                "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                "     ELSE  " &
                "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                " END)" & vbCrLf &
                " , TransInvoice.FowstID" & vbCrLf &
                " , TransInvoice.DeviceID" & vbCrLf &
                " , TransInvoice.InvoiceDate" & vbCrLf &
                " , MobizLibraryOrganizationFOWST.Name" & vbCrLf &
                " , LibraryDevice.PhoneName" & vbCrLf &
                " ,TransInvoice.LastUpdated " & vbCrLf

            .SqlOrder = " ORDER BY " & vbCrLf &
                " (CASE TransInvoice.FOWSTID " &
                " WHEN " & ReadyString(WebRequest.Instance.FowstID) & " THEN 0 " &
                " ELSE " &
                "   1 " &
                " END) ASC, " & vbCrLf &
                " TransInvoice.InvoiceDate DESC" & vbCrLf &
                " ,TransInvoice.LastUpdated DESC" & vbCrLf
            'commented by janine nov 24 2014 . do not check device.
            '" (CASE TransInvoice.DeviceID " &
            '   " WHEN " & ReadyString(WebRequest.Instance.DeviceID) & " THEN 0 " &
            '   " ELSE " &
            '   "   1 " &
            '   " END) ASC, " & vbCrLf & _
        End With

        Try
            mFinder.ProData.CommandTimeout = 3 * 60 * 1000
            Return SubmitFinder()
        Catch ex As Exception
            Dim pSql As String

            pSql = GetStackTrace(, ex) & vbCrLf
            pSql &= mFinder.CreateSql
            AppendToLog(pSql, , "BrowserController_TransInvoicesReturnables")
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

        ' '' '''Change by Francis Adan
        ' '' ''' At first query, it is looking only on the current device, if no result then call again the function but this time all device of the fowst
        ' ''pDT = mFinder.Requery
        ' ''If (pDT.Rows.Count > 0) Then
        ' ''    Return JsonDictionary.Serialize("Results", pDT)
        ' ''Else
        ' ''    If (Not vIsAllDeviceInFowst) Then
        ' ''        TransInvoicesReturnables(True)
        ' ''    Else
        ' ''        Return JsonDictionary.Serialize("Results", pDT)
        ' ''    End If
        ' ''End If


    End Function

    <HttpPost()>
    Public Function MainSecurityLevel() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                With .Add("LevelType", SqlDbType.VarChar)
                    .Expression = "MainSecurityLevel.LevelType"
                End With
            End With
            .SqlFrom = "FROM MainSecurityLevel"
            .SqlWhere = "WHERE IsCancelled = 0"
            .SqlOrder = "ORDER BY MainSecurityLevel.LevelType"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryBranch() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                With .Add("BranchName", SqlDbType.VarChar)
                    .Expression = "tblBranch.Address"
                End With
            End With
            .SqlFrom = "FROM DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch"
            .SqlOrder = "ORDER BY BranchName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Function LibraryDefaultPIMFeature() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.ID"
                    .Searchable = False
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With
            End With
            .SqlFrom = "FROM dbo.MobizLibraryCountriesLanguageLabel INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMFeature ON dbo.MobizLibraryCountriesLanguageLabel.ID = dbo.MobizLibraryPIMFeature.FeatureLabelID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProductFeature INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryUNSPSCCommodityFeature ON dbo.MobizLibraryPIMProductFeature.CommodityFeatureID = dbo.MobizLibraryUNSPSCCommodityFeature.ID ON  " & vbCrLf &
                      "dbo.MobizLibraryPIMFeature.ID = dbo.MobizLibraryUNSPSCCommodityFeature.FeatureID INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProductFeature.ProductID = dbo.LibraryOrganizationPIM.ProductID "
            .SqlWhere = "WHERE MobizLibraryPIMFeature.IsDefaultFeature = 1"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryCustomerFowst() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstName", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.Name"
                    .Searchable = False
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
            End With

            strOrgID = WebRequest.Instance.OrgID
            If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
                If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                    strOrgID = WebRequest.Instance.Data("OrganizationID").Trim()
                End If
            End If

            If CInt(WebRequest.Instance.Data("FilterFowst")) = 1 Then
                .SqlFrom = "FROM dbo.LibraryOrganizationEmployee INNER JOIN " &
                      "dbo.LibraryOrganizationEmployeeFOWST ON dbo.LibraryOrganizationEmployee.PeopleID = dbo.LibraryOrganizationEmployeeFOWST.PeopleID INNER JOIN " &
                      "dbo.MobizLibraryOrganizationFOWST AS fowst ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID"
                .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID)
            Else
                .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
                " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)"

                .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID)
            End If

            .SqlOrder = "ORDER BY Address"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryCustomerFowstBin() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            If CInt(WebRequest.Instance.Data("NoFilter")) Then
                mFinder.MaxRecords = 0
            End If
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
                .Add("Description", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryOrganizationCustomerFowstBin"
            .SqlWhere = "WHERE FowstID = " & ReadyString(IIf(Common.WebRequest.Data("CustomerID") = vbNullString, WebSession.Instance.FowstID, Common.WebRequest.Data("CustomerID")))
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryDTRSchedule() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                .Add("ScheduleName", SqlDbType.VarChar)
                .Add("ScheduleDescription", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryDTRSchedule"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryDeminimis() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryDeminimis.ID"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "LibraryDeminimis.AllowanceName"
                End With
                .Add("MaxAmount", SqlDbType.VarChar)
                .Add("MaxTaxExemptAmount", SqlDbType.VarChar)
                .Add("FrequencyOfAllowance", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryDeminimis"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibrarySalaryGrade() As String
        'Used for CRM
        Dim strOrgID As String


        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibrarySalaryGrade.ID"
                End With
                With .Add("SalaryGradeName", SqlDbType.VarChar)
                    .Expression = "LibrarySalaryGrade.SalaryGradeName"
                End With
            End With

            .SqlFrom = "FROM LibrarySalaryGrade " & vbCrLf
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFranchisee() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With

                With .Add("FowstName", SqlDbType.Int)
                    .Expression = "fowst.Name"
                End With

                With .Add("Address", SqlDbType.Int)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                     "+ ', ' + ISNULL(fowst.Street, '') " &
                                     "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With

            End With
            .SqlFrom = "FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
                        "inner join MobizLibraryOrganizationFOWST fowst on fowst.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                        "LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " & vbCrLf &
                        "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID "

            .SqlWhere = "where MobizLibraryOrganizationFranchisee.IsActive = 1 AND FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryOverallPeople() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Expression = "ISNULL(pub.ID, prv.ID)"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(ISNULL(NULLIF(pub.PersonSurname + ',' + pub.PersonFirstName + ',' + pub.PersonMiddleName + ' ' + pub.PersonSuffixName, ',, '), pub.CompanyName), ''), prv.Surname + ',' + prv.FirstName + ',' + prv.MiddleName + ' ' + prv.PersonalSuffixName)"
                End With

            End With
            .SqlFrom = "FROM MobizLibraryPeople AS prv" & vbCrLf &
                        "RIGHT JOIN LibraryOrganizationEmployee ON prv.ID = LibraryOrganizationEmployee.PeopleID" & vbCrLf &
                        "FULL JOIN MobizLibraryOrganization AS pub ON prv.ID = pub.ID "

        End With

        Return SubmitFinder()
    End Function

    Public Function LibraryDocument() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransDocumentTracker.ID"
                    .Searchable = False
                End With
                With .Add("ReleaseDoc", SqlDbType.VarChar)
                    .Expression = "TransDocumentTracker.ReleaseDoc"
                End With
            End With
            .SqlFrom = "FROM TransDocumentTracker"
            .SqlWhere = " WHERE TransDocumentTracker.ReleaseDoc LIKE '%" & WebRequest.Instance.Data("docNum") & "%'"
            .SqlOrder = "ORDER BY TransDocumentTracker.ReleaseDoc"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationProductPublisher() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("NoOfProducts", SqlDbType.BigInt)
                    .Expression = "(SELECT COUNT(ID) FROM MobizLibraryPIMProduct WHERE PublishedByID = org.ID)"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "(SELECT TOP 1 LastUpdated FROM MobizLibraryPIMProduct WHERE PublishedByID = org.ID ORDER BY LastUpdated DESC)"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMProduct pim INNER JOIN MobizLibraryOrganization org ON pim.PublishedByID = org.ID "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY org.CompanyName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransReceivingAndTransfers() As String
        'Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pIsReceiving As Boolean = CBool2(Common.WebRequest.Data("IsReceiving"))

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'pFinder = New Finder

        With CreateFinder()
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .SearchString = WebRequest.Instance.Data("keywords")

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.TransReceiving.ID"
                End With

                .Add("ReceivingNo", SqlDbType.Int)
                .Add("IsReceiving", SqlDbType.Bit)
                With .Add("InvoiceNumber", SqlDbType.VarChar)
                    .Expression = " TransReceiving.InvoiceNumber"
                End With
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

                With .Add("TransportedBy", SqlDbType.VarChar)
                    .Expression = " TransReceiving.TransportedBy"
                End With

                With .Add("SKUCount", SqlDbType.VarChar)
                    .Expression = "(SELECT COUNT(*) FROM TransReceivingStock WHERE ReceivingID = TransReceiving.ID" & vbCrLf &
                        " AND (TransReceivingStock.QuantityGood > 0 OR TransReceivingStock.QuantityDamaged > 0))"
                    .Searchable = False
                End With

                .Add("DateReceived", SqlDbType.DateTime)

                .Add("DocumentDate", SqlDbType.DateTime)
                .Add("DocumentType", SqlDbType.VarChar)
                .Add("DocumentNo", SqlDbType.VarChar)
                With .Add("NetSuiteNum", SqlDbType.VarChar)
                    .Expression = "TransReceiving.NetSuiteNum"
                End With
                With .Add("MIN", SqlDbType.VarChar)
                    .Expression = "ISNULL(CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo), 'X') + '-' + ISNULL(CONVERT(varchar, LibraryDevice.DeviceNo), 'X') + '-' + CONVERT(varchar, ReceivingNo)"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime2)
                    .Expression = "TransReceiving.LastUpdated"
                End With
                With .Add("FullPurchaseOrderNum", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, tblPoFowst.FOWSTNo) + '-' + CONVERT(varchar, tblPoDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.PurchaseOrderNo)"
                End With
                With .Add("PoLastUpdated", SqlDbType.DateTime2)
                    .Expression = "TransInvoice.LastUpdated"
                End With
                With .Add("UpdatedBy", SqlDbType.VarChar)
                    .Expression = ComposePeopleName()
                End With

                With .Add("ReceiveFromOutletName", SqlDbType.VarChar)
                    .Expression = "ISNULL(tblReceiveFromOutlet.Name, '')"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                      "dbo.TransReceiving ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransReceiving.ReceivingPlaceID LEFT JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganization ON dbo.TransReceiving.ShipperID = dbo.MobizLibraryOrganization.ID" & vbCrLf &
                      " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransReceiving.UserID" & vbCrLf &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                      " LEFT OUTER JOIN dbo.MobizLibraryOrganization AS MobizLibraryOrganization_1 ON dbo.TransReceiving.SupplierID = MobizLibraryOrganization_1.ID LEFT OUTER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST AS MobizLibraryOrganizationFowst_1 ON dbo.TransReceiving.SupplierID = MobizLibraryOrganizationFowst_1.ID " & vbCrLf &
                      " LEFT JOIN LibraryDevice ON TransReceiving.DeviceID = LibraryDevice.ID" & vbCrLf &
                " LEFT JOIN TransInvoice ON TransInvoice.ID = TransReceiving.PurchaseOrderID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST tblPoFowst ON tblPoFowst.ID = TransInvoice.FowstID" & vbCrLf &
                " LEFT JOIN LibraryDevice tblPoDevice ON TransInvoice.DeviceID = tblPoDevice.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST tblReceiveFromOutlet ON tblReceiveFromOutlet.ID = TransReceiving.ReceiveFromOutlet "

            'If Not Common.WebRequest.Data("ID") Is Nothing Then
            '    .SqlWhere = "WHERE TransReceiving.ID = " & ReadyString(Common.WebRequest.Data("ID"))
            'End If

            If pIsReceiving Then
                .SqlWhere = "WHERE TransReceiving.IsReceiving = 1" & vbCrLf
                .SqlWhere &= " AND ISNULL(TransReceiving.IsCancelled, 0) = 0" & vbCrLf
                .SqlOrder = "ORDER BY TransReceiving.LastUpdated desc"
            Else
                .SqlWhere = "WHERE TransReceiving.IsReceiving = 0 AND TransferOutToReceivingID IS NULL " ' AND ReceivingPlaceID = " & Common.WebRequest.Data("FOWSTID")
                .SqlWhere &= " AND ISNULL(TransReceiving.IsCancelled, 0) = 0" & vbCrLf
                .SqlOrder = "ORDER BY TransReceiving.LastUpdated desc" & vbCrLf &
                    ", DocumentDate desc"
            End If

            'pDT = .Requery
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function RequestAndApprovals()

        Dim pSql As String
        Dim pSqlInner As String = ""
        Dim pFowst As String
        Dim pStatus As String
        Dim pType As String
        Dim pIncludeReturns As Boolean
        Dim pDT As DataTable
        Dim pViewAll As Boolean
        Dim pViewGroup As Boolean
        Dim pViewBranch As Boolean

        pFowst = Common.WebRequest.Data("Fowst")
        If IsGuid(pFowst) = False _
            AndAlso pFowst <> "all" Then

            Return JsonDictionary.Serialize("CantViewRequests", 1)
        End If

        pStatus = Common.WebRequest.Data("Status")
        pType = Common.WebRequest.Data("Type")
        pIncludeReturns = pType = "returns" OrElse pType = "all"

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pViewAll = SecurityRole.Instance.HandleRoleAccess("View all requests", , True)
        If pViewAll = False Then
            pViewAll = SecurityRole.Instance.HandleRoleAccess("View and Approve all requests", , True)
        End If

        pViewGroup = SecurityRole.Instance.HandleRoleAccess("View group requests", , True)
        If pViewGroup = False Then
            pViewGroup = SecurityRole.Instance.HandleRoleAccess("View and Approve group requests", , True)
        End If

        pViewBranch = SecurityRole.Instance.HandleRoleAccess("View branch requests only", , True)
        If pViewBranch = False Then
            pViewBranch = SecurityRole.Instance.HandleRoleAccess("View and Approve branch requests", , True)
        End If

        If pViewAll = False _
            AndAlso pViewGroup = False _
            AndAlso pViewBranch = False Then

            Return JsonDictionary.Serialize("CantViewRequests", 1)
        End If

        Common.WebRequest.ProData.BeginTransaction()
        WebRequest.Instance.Business.LibraryPeople.CreateTempTableAllowedViewRequestFowsts(
            vKeywords:=Nothing,
            vAllowOrgsFowsts:=pViewAll,
            vAllowGroupFowsts:=pViewGroup,
            vAllowBranchFowsts:=pViewBranch)
        pDT = Common.WebRequest.ProData.Execute("SELECT * FROM #tmpAllowedFowsts")

        If pIncludeReturns Then
            With CreateFinder()
                .ProData = Common.WebRequest.ProData
                .MaxRecords = 50
                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier, "TransMemo.ID")
                        .Searchable = False
                    End With
                    .Add("RequestType", SqlDbType.VarChar, "'Sales Returns'")
                    .Add("Reference", SqlDbType.VarChar, "'SR ' + CONVERT(varchar, TransMemo.MemoNumber)")
                    .Add("FowstName", SqlDbType.VarChar, "MobizLibraryOrganizationFOWST.Name")
                    With .Add("RequestedBy", SqlDbType.VarChar)
                        .Expression = ComposePeopleName()
                    End With
                    .Add("RequestedDateTime", SqlDbType.DateTime2, "TransMemo.DateRequested")
                    .Add("ApprovedBy", SqlDbType.VarChar, ComposePeopleName("tblPeopleApprover", "tblNominalApprover"))
                    .Add("ApprovedDateTime", SqlDbType.DateTime2, "TransMemo.DateApproved")
                End With
                .SqlFrom = " FROM TransMemo" & vbCrLf &
                    " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransMemo.FowstID" & vbCrLf &
                    " JOIN #tmpAllowedFowsts ON #tmpAllowedFowsts.ID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                    " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransMemo.RequestedByPeopleID" & vbCrLf &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople tblPeopleApprover ON tblPeopleApprover.ID = TransMemo.ApprovedBy" & vbCrLf &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblNominalApprover ON tblPeopleApprover.TitleID = tblNominalApprover.ID" & vbCrLf

                .SqlWhere = " WHERE 1 = 1"
                Select Case CStr2(pStatus).Trim().ToLower()
                    Case "all"
                    Case Else ' pending
                        .SqlWhere &= " AND TransMemo.ApprovedBy IS NULL" & vbCrLf &
                            " AND TransMemo.DeniedBy IS NULL"
                End Select
                If IsGuid(pFowst) Then
                    .SqlWhere &= " AND TransMemo.FowstID = " & ReadyString(pFowst)
                End If

                If pSqlInner <> "" Then
                    pSqlInner &= vbCrLf & " UNION " & vbCrLf
                End If
                pSqlInner = .CreateSql()
            End With
        End If ' END If pIncludeReturns Then

        If pSqlInner = "" Then
            Return JsonDictionary.Serialize("NoResult", 1)
        Else
            pSql = "SELECT *" & vbCrLf &
                " FROM (" & vbCrLf &
                pSqlInner & vbCrLf &
                ") t" & vbCrLf &
                " ORDER BY RequestedDateTime"
            pDT = Common.WebRequest.ProData.Execute(pSql)
        End If
        Common.WebRequest.ProData.RollbackTransaction()

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    <HttpPost()>
    Public Function RequestAndApproveFowsts() As String

        Dim pViewAll As Boolean
        Dim pViewGroup As Boolean
        Dim pViewBranch As Boolean
        Dim pDT As DataTable

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pViewAll = SecurityRole.Instance.HandleRoleAccess("View all requests", , True)
        If pViewAll = False Then
            pViewAll = SecurityRole.Instance.HandleRoleAccess("View and Approve all requests", , True)
        End If

        pViewGroup = SecurityRole.Instance.HandleRoleAccess("View group requests", , True)
        If pViewGroup = False Then
            pViewGroup = SecurityRole.Instance.HandleRoleAccess("View and Approve group requests", , True)
        End If

        pViewBranch = SecurityRole.Instance.HandleRoleAccess("View branch requests only", , True)
        If pViewBranch = False Then
            pViewBranch = SecurityRole.Instance.HandleRoleAccess("View and Approve branch requests", , True)
        End If

        If pViewAll = False _
            AndAlso pViewGroup = False _
            AndAlso pViewBranch = False Then

            Return JsonDictionary.Serialize("CantViewRequests", 1)
        End If

        Common.WebRequest.ProData.BeginTransaction()
        WebRequest.Instance.Business.LibraryPeople.CreateTempTableAllowedViewRequestFowsts(
            vKeywords:=Common.WebRequest.Data("keywords"),
            vAllowOrgsFowsts:=pViewAll,
            vAllowGroupFowsts:=pViewGroup,
            vAllowBranchFowsts:=pViewBranch)
        pDT = Common.WebRequest.ProData.Execute("SELECT * FROM #tmpAllowedFowsts")
        Common.WebRequest.ProData.RollbackTransaction()

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    <HttpPost()>
    Public Function ItemModifiers() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "LibraryItemModifiers.ID")
                .Add("Name", SqlDbType.VarChar, "LibraryItemModifiers.Name")
                .Add("Type", SqlDbType.VarChar, "LibraryItemModifiers.Type")
                .Add("LastUpdated", SqlDbType.DateTime2, "LibraryItemModifiers.LastUpdated")
                .Add("UpdatedByName", SqlDbType.VarChar, ComposePeopleName())
            End With
            .SqlFrom = " FROM LibraryItemModifiers" & vbCrLf &
                " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryItemModifiers.UpdatedBy" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
            .SqlOrder = " ORDER BY LibraryItemModifiers.LastUpdated DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransBomMaker() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransBomMaker.ID")
                .Add("AssignedToID", SqlDbType.UniqueIdentifier, "TransBomMaker.AssignedToID")
                .Add("Num", SqlDbType.Int, "TransBomMaker.Num")
                .Add("LastUpdated", SqlDbType.DateTime, "TransBomMaker.LastUpdated")
                .Add("DateNeeded", SqlDbType.DateTime, "TransBomMaker.DateNeeded")
                .Add("UpdatedByName", SqlDbType.VarChar, ComposePeopleName())
                .Add("AssignedToName", SqlDbType.VarChar, ComposePeopleName("tblAssignedToMobizLibraryPeople", "tblAssignedToMobizLibraryPeoplePreNominalSuffix"))
                .Add("FowstDeviceNum", SqlDbType.VarChar, "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)")
            End With
            .SqlFrom = " FROM TransBomMaker" & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransBomMaker.UserID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeople tblAssignedToMobizLibraryPeople ON tblAssignedToMobizLibraryPeople.ID = TransBomMaker.AssignedToID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblAssignedToMobizLibraryPeoplePreNominalSuffix ON tblAssignedToMobizLibraryPeople.TitleID = tblAssignedToMobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST ON TransBomMaker.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransBomMaker.DeviceID" & vbCrLf
            .SqlOrder = " ORDER BY TransBomMaker.LastUpdated DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowstProductBarcode() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("NumLabels", SqlDbType.VarChar)
                    .Expression = "(SELECT SUM(NumLabels) FROM LibraryFowstProductBarcode WHERE TransactionID = sku.TransactionID)"
                End With
                With .Add("CreatedBy", SqlDbType.UniqueIdentifier)
                    .Expression = "sku.CreatedBy"
                End With
                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "convert(varchar(16), sku.LastUpdated, 120)"
                End With
                With .Add("TransactionNum", SqlDbType.Int)
                    .Expression = "sku.TransactionNum"
                End With
            End With
            .SqlFrom = " FROM LibraryFowstProductBarcode sku " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProduct AS prod ON sku.SkuDesc = prod.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProductPriceLevel AS price ON sku.Price = price.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMPriceLevel AS pricelevel ON price.PriceLevelID = pricelevel.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProductMeasurement AS uom ON sku.UoM = uom.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMMeasure AS uom2 ON uom.UnitMeasureID = uom2.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryCountriesLanguageLabel AS uom3 ON uom2.MeasurenameID = uom3.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryPIMProductMeasurementBarcode AS barcode ON sku.Barcode = barcode.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryOrganization AS org ON sku.OrgID = org.ID " & vbCrLf
            .SqlWhere = " WHERE CONVERT(varchar(50), TransactionID) <> '' " & vbCrLf
            .SqlGroup = " GROUP BY sku.TransactionID , sku.CreatedBy, convert(varchar(16), sku.LastUpdated, 108), sku.LastUpdated, sku.TransactionNum " & vbCrLf
            .SqlOrder = " ORDER BY sku.TransactionNum DESC"

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function PimCategories(Optional ByVal IsAll As Boolean = False) As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("Name", SqlDbType.VarChar, "MobizLibraryPIMProductVariableFeatures001.C0020")

                If CBool2(Common.WebRequest.Data("IsInventoryItem")) Then
                    .Add("SortOrder", SqlDbType.Int, "(SELECT TOP 1 SortOrder FROM LibraryPimCategories WHERE Category = MobizLibraryPIMProductVariableFeatures001.C0020)")
                End If
            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct" & vbCrLf &
                " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID"

            If CBool2(Common.WebRequest.Data("IsInventoryItem")) Then
                .SqlOrder = " ORDER BY SortOrder"
            Else
                .SqlOrder = " ORDER BY MobizLibraryPIMProductVariableFeatures001.C0020"
            End If

            If CBool2(Common.WebRequest.Data("IsInventoryItem")) Then
                .SqlWhere = " WHERE LibraryOrganizationPIM.IsInventoryCountItem = 1 "
            End If

            If CBool2(WebRequest.Instance.Data("IsAll")) Or IsAll Then
                .MaxRecords = 0
            End If
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPimBrands(Optional ByVal IsAll As Boolean = False) As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'only unique brands ang nakasave diri, no duplication

        With CreateFinder()
            '.MaxRecords = 0
            With .Fields
                With .Add("ID", SqlDbType.VarChar, "ID")
                    .Searchable = False
                End With
                .Add("Brand", SqlDbType.VarChar, "Brand")
            End With

            .SqlFrom = " FROM LibraryPimBrands" & vbCrLf
            .SqlWhere = "WHERE IsActive = 1 "

            If CStr2(Common.WebRequest.Data("keywords")) <> "" Then
                .SqlWhere &= " AND Brand LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))
            End If

            .SqlOrder = " ORDER BY Brand ASC "

            If CBool2(WebRequest.Instance.Data("IsAll")) Or IsAll Then
                .MaxRecords = 0
            End If
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPimCategories(Optional ByVal IsAll As Boolean = False) As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'only unique categories ang nakasave diri, no duplication

        With CreateFinder()
            If CBool2(Common.WebRequest.Data("IsAll")) Then
                .MaxRecords = 0
            End If

            With .Fields
                With .Add("ID", SqlDbType.VarChar, "ID")
                    .Searchable = False
                End With
                .Add("Category", SqlDbType.VarChar, "Category")
            End With

            .SqlFrom = " FROM LibraryPimCategories" & vbCrLf
            .SqlWhere = "WHERE IsActive = 1 "

            If CStr2(Common.WebRequest.Data("keywords")) <> "" Then
                .SqlWhere &= " AND Category LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))
            End If

            .SqlOrder = " ORDER BY Category ASC "

            If CBool2(WebRequest.Instance.Data("IsAll")) Or IsAll Then
                .MaxRecords = 0
            End If
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPimCategoryGpm() As String

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("Category", SqlDbType.VarChar, "LibraryPimCategoryGpm.Category")
                .Add("Gpm", SqlDbType.Money, "LibraryPimCategoryGpm.Gpm")
            End With

            .SqlFrom = " FROM LibraryPimCategoryGpm" & vbCrLf

            .SqlOrder = " ORDER BY Category"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationFOWSTSection() As String

        Dim pFowstID As String

        pFowstID = Common.WebRequest.Data("FowstID")
        If IsGuid(pFowstID) = False Then
            Return JsonDictionary.Serialize("Error", "FowstID must be a valid GUID")
        End If
        pFowstID = ReadyGuid(pFowstID)

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "MobizLibraryOrganizationFOWSTSection.ID")
                .Add("Code", SqlDbType.VarChar, "MobizLibraryOrganizationFOWSTSection.Code")
                .Add("Name", SqlDbType.VarChar, "MobizLibraryOrganizationFOWSTSection.Name")
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFOWSTSection" & vbCrLf
            .SqlWhere = " WHERE MobizLibraryOrganizationFOWSTSection.FOWSTID = " & ReadyString(pFowstID)
            .SqlOrder = " ORDER BY Code, Name"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function ReadyToInvoice() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("InvoiceID", SqlDbType.UniqueIdentifier, "TransInvoiceItemWarehouseSections.InvoiceID")
                .Add("InvoiceNumber", SqlDbType.Int, "TransInvoice.InvoiceNumber")
                .Add("SalesOrderNo", SqlDbType.Int, "TransInvoice.SalesOrderNo")
                .Add("InvoiceTypeCode", SqlDbType.VarChar, "TransInvoice.InvoiceTypeCode")
                .Add("CustomerID", SqlDbType.UniqueIdentifier, "TransInvoice.CustomerID")
                .Add("LastUpdated", SqlDbType.DateTime2, "TransInvoice.LastUpdated")
                .Add("CompanyName", SqlDbType.VarChar, "MobizLibraryOrganization.CompanyName")
                With .Add("MIN", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)"
                End With
            End With
            .SqlFrom = " FROM TransInvoiceItemWarehouseSections" & vbCrLf &
                " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItemWarehouseSections.InvoiceID" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
                " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf
            .SqlWhere = " WHERE TransInvoiceItemWarehouseSections.IsDone = 1" & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.ReadyForInvoicingDate IS NULL" & vbCrLf &
                " AND TransInvoiceItemWarehouseSections.SortlingListID IS NULL"
            .SqlOrder = " ORDER BY LastUpdated DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryAccountAdjustments() As String

        Dim pAccountType As String
        pAccountType = Common.WebRequest.Data("AccountType")
        If String.IsNullOrWhiteSpace(pAccountType) Then
            Return JsonDictionary.Serialize("Error", "AccountType must not be blank when requesting reasons.")
        End If
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        WebRequest.Instance.Business.LibraryAccountAdjustments.AddTypesIfNeeded()

        Dim pDebitSign As String
        Dim pCreditSign As String

        If pAccountType.ToLower() = "ar" Then
            pDebitSign = "+"
            pCreditSign = "-"
        Else
            pDebitSign = "-"
            pCreditSign = "+"
        End If

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "LibraryAccountAdjustments.ID")
                .Add("Reason", SqlDbType.VarChar, "LibraryAccountAdjustments.Reason")
                .Add("AdjustmentType", SqlDbType.VarChar, "LibraryAccountAdjustments.AdjustmentType")
                .Add("Effect", SqlDbType.VarChar, "LibraryAccountAdjustments.Effect")
                With .Add("Display", SqlDbType.VarChar)
                    .Expression = "CASE LibraryAccountAdjustments.AdjustmentType" & vbCrLf &
                        " WHEN 'Debit Memo' THEN 'Debit Memo (" & pDebitSign & ") -- ' + LibraryAccountAdjustments.Reason" & vbCrLf &
                        " WHEN 'Credit Memo' THEN 'Credit Memo (" & pCreditSign & ") -- ' + LibraryAccountAdjustments.Reason" & vbCrLf &
                        " END" & vbCrLf
                End With
            End With
            .SqlFrom = " FROM LibraryAccountAdjustments" & vbCrLf
            .SqlWhere = " WHERE LibraryAccountAdjustments.IsActive = 1" & vbCrLf &
                " AND AccountType = " & ReadyString(pAccountType)
            .SqlOrder = " ORDER BY Display, Effect, Reason"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDocumentControl() As String

        Dim pModuleSource As String

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pModuleSource = Common.WebRequest.Data("ModuleSource")
        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "LibraryDocumentControl.ID")
                .Add("DocumentName", SqlDbType.VarChar, "LibraryDocumentControl.DocumentName")
                .Add("ModuleSource", SqlDbType.VarChar, "LibraryDocumentControl.ModuleSource")
                .Add("UnitPriceSurchargePercent", SqlDbType.Decimal, "LibraryDocumentControl.UnitPriceSurchargePercent")
            End With
            .SqlFrom = " FROM LibraryDocumentControl" & vbCrLf
            .SqlWhere = " WHERE LibraryDocumentControl.IsActive = 1"
            If String.IsNullOrWhiteSpace(pModuleSource) = False Then
                .SqlWhere &= " AND LibraryDocumentControl.ModuleSource = " & ReadyString(pModuleSource) & vbCrLf
            End If
            .SqlOrder = " ORDER BY LibraryDocumentControl.DocumentName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function PurchaseOrderablePriceCanvass() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "LibraryPriceCanvass.ID")
                .Add("VendorID", SqlDbType.UniqueIdentifier, "LibraryPriceCanvassVendor.VendorID")
                .Add("CanvassNumber", SqlDbType.Int, "LibraryPriceCanvass.CanvassNumber")
                .Add("CompanyName", SqlDbType.VarChar, "MobizLibraryOrganization.CompanyName")
                .Add("LastUpdated", SqlDbType.DateTime, "LibraryPriceCanvass.LastUpdated")
            End With
            .SqlFrom = " FROM LibraryPriceCanvassVendorPrice" & vbCrLf &
                " JOIN LibraryPriceCanvassVendor ON LibraryPriceCanvassVendor.ID = LibraryPriceCanvassVendorPrice.CanvassVendorID" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryPriceCanvassVendor.VendorID" & vbCrLf &
                " JOIN LibraryPriceCanvassItemSku ON LibraryPriceCanvassItemSku.ID = LibraryPriceCanvassVendorPrice.CanvassItemSkuID" & vbCrLf &
                " JOIN LibraryPriceCanvassItems ON LibraryPriceCanvassItems.ID = LibraryPriceCanvassItemSku.CanvassItemID" & vbCrLf &
                " JOIN LibraryPriceCanvass ON LibraryPriceCanvass.ID = LibraryPriceCanvassVendor.CanvassID" & vbCrLf
            .SqlWhere = " WHERE LibraryPriceCanvassVendorPrice.Price > 0" & vbCrLf &
                " AND LibraryPriceCanvassItems.Qty > 0"
            .SqlOrder = " ORDER BY LibraryPriceCanvass.LastUpdated" & vbCrLf &
                ",MobizLibraryOrganization.CompanyName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function GetCanvassItems() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pCanvassID As String = Common.WebRequest.Data("CanvassID")
        Dim pVendorID As String = Common.WebRequest.Data("VendorID")

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ProductID", SqlDbType.UniqueIdentifier, "LibraryPriceCanvassItemSku.SkuID")
                .Add("Qty", SqlDbType.Decimal, "LibraryPriceCanvassItems.Qty")
                .Add("Price", SqlDbType.Decimal, "LibraryPriceCanvassVendorPrice.Price")
                .Add("Unit", SqlDbType.VarChar, "MobizLibraryCountriesLanguageLabel.Label")
                .Add("UnitID", SqlDbType.UniqueIdentifier, "LibraryPriceCanvassItems.UnitOfMeasure")
                .Add("ProductNo", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductNo")
                .Add("ProductName", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductName")
            End With
            .SqlFrom = " FROM LibraryPriceCanvass" & vbCrLf &
                " JOIN LibraryPriceCanvassItems ON LibraryPriceCanvassItems.CanvassID = LibraryPriceCanvass.ID" & vbCrLf &
                " JOIN LibraryPriceCanvassItemSku ON LibraryPriceCanvassItemSku.CanvassItemID = LibraryPriceCanvassItems.ID" & vbCrLf &
                " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryPriceCanvassItemSku.SkuID" & vbCrLf &
                " JOIN LibraryPriceCanvassVendorPrice ON LibraryPriceCanvassVendorPrice.CanvassItemSkuID = LibraryPriceCanvassItemSku.ID" & vbCrLf &
                " JOIN LibraryPriceCanvassVendor ON LibraryPriceCanvassVendor.CanvassID = LibraryPriceCanvass.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = LibraryPriceCanvassItems.UnitOfMeasure" & vbCrLf &
                " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf
            .SqlWhere = " WHERE LibraryPriceCanvass.ID = " & ReadyString(pCanvassID) & vbCrLf &
                " AND LibraryPriceCanvassVendor.VendorID = " & ReadyString(pVendorID)
            .SqlOrder = " ORDER BY MobizLibraryPIMProduct.ProductName" & vbCrLf
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransCustomerPaymentChecks() As String

        Dim pInvoiceID As String = Common.WebRequest.Data("InvoiceID")
        Dim pCustomerID As String = Common.WebRequest.Data("CustomerID")

        ' commented 2021_06_21__10_06_49_0476
        'If String.IsNullOrWhiteSpace(pInvoiceID) Then
        '    Return JsonDictionary.Serialize("Error", "InvoiceID is required. ref: vbZNCDBYQR9U!")
        'End If

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransCustomerPaymentChecks.ID")
                .Add("InvoiceID", SqlDbType.UniqueIdentifier, "TransCustomerPaymentCheckAmount.InvoiceID")
                .Add("BankName", SqlDbType.VarChar, "TransCustomerPaymentChecks.BankName")
                .Add("CheckNumber", SqlDbType.VarChar, "TransCustomerPaymentChecks.CheckNumber")
                .Add("CheckAmount", SqlDbType.Money, "TransCustomerPaymentChecks.CheckAmount")
                .Add("CheckDate", SqlDbType.DateTime, "TransCustomerPaymentChecks.CheckDate")
                .Add("InvoiceNumber", SqlDbType.DateTime, "TransInvoice.InvoiceNumber")
                .Add("InvoiceDate", SqlDbType.Int, "TransInvoice.InvoiceDate")
                .Add("InvoiceAmount", SqlDbType.Money, "TransInvoice.TotalAmount")
                .Add("ChequeAmountPaid", SqlDbType.Money, "TransCustomerPaymentCheckAmount.AmountPaid")
                .Add("InvoiceMin", SqlDbType.VarChar, "CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)")
            End With
            .SqlFrom = " FROM TransCustomerPaymentChecks" & vbCrLf &
                " JOIN TransCustomerPaymentCheckAmount ON TransCustomerPaymentCheckAmount.CustomerPaymentCheckID = TransCustomerPaymentChecks.ID" & vbCrLf &
                " JOIN TransInvoice ON TransInvoice.ID = TransCustomerPaymentCheckAmount.InvoiceID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST InvFowst ON TransInvoice.FowstID = InvFowst.ID" & vbCrLf &
                " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf
            .SqlWhere = " WHERE TransCustomerPaymentChecks.DepositStatus <> 2 " & vbCrLf
            If String.IsNullOrWhiteSpace(pInvoiceID) = False Then
                .SqlWhere &= " AND TransCustomerPaymentInvoices.InvoiceID = " & ReadyString(pInvoiceID) & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(pCustomerID) = False Then
                .SqlWhere &= " AND TransInvoice.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
            End If
            .SqlOrder = " ORDER BY TransCustomerPaymentChecks.BankName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransCustomerPaymentDraft() As String

        Dim pCustomerID As String = Common.WebRequest.Data("CustomerID")

        If String.IsNullOrWhiteSpace(pCustomerID) Then
            Return JsonDictionary.Serialize("Error", "pCustomerID is required. ref: vbVA6CPEY9J2!")
        End If

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransCustomerPaymentDraft.ID")
                .Add("PaymentDate", SqlDbType.DateTime, "TransCustomerPaymentDraft.PaymentDate")
                .Add("CashAmount", SqlDbType.Money, "TransCustomerPaymentDraft.CashAmount")
            End With
            .SqlFrom = " FROM TransCustomerPaymentDraft" & vbCrLf
            .SqlWhere = " WHERE TransCustomerPaymentDraft.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
            .SqlOrder = " ORDER BY TransCustomerPaymentDraft.PaymentDate"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransCustomerPayment() As String

        Dim pCustomerID As String = Common.WebRequest.Data("CustomerID")

        If String.IsNullOrWhiteSpace(pCustomerID) Then
            Return JsonDictionary.Serialize("Error", "pCustomerID is required. ref: vbVA6CPEY9J2!")
        End If

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransCustomerPayment.ID")
                .Add("PaymentDate", SqlDbType.DateTime, "TransCustomerPayment.PaymentDate")
                .Add("CashAmount", SqlDbType.Money, "TransCustomerPayment.CashAmount")
                .Add("TotalAmountDue", SqlDbType.Money, "TransCustomerPayment.TotalAmountDue")
                .Add("CheckPaymentAmount", SqlDbType.Money, "TransCustomerPayment.CheckPaymentAmount")
                .Add("CreditCardAmount", SqlDbType.Money, "TransCustomerPayment.CreditCardAmount")
                .Add("DebitCardAmount", SqlDbType.Money, "TransCustomerPayment.DebitCardAmount")
                With .Add("TotalPayment", SqlDbType.Money)
                    .Expression = "TransCustomerPayment.CashAmount - TransCustomerPayment.ChargeAmount + TransCustomerPayment.SRVAmount + TransCustomerPayment.CheckPaymentAmount + TransCustomerPayment.CreditCardAmount + TransCustomerPayment.DebitCardAmount + TransCustomerPayment.ChargeAmount"
                End With
            End With
            .SqlFrom = " FROM TransCustomerPayment" & vbCrLf
            .SqlWhere = " WHERE TransCustomerPayment.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
            .SqlOrder = " ORDER BY TransCustomerPayment.PaymentDate DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransReceivingPayment() As String

        Dim pCustomerID As String = Common.WebRequest.Data("SupplierID")
        Dim pCustomerSelectSql As String
        Dim pCustomerFromSql As String

        'If String.IsNullOrWhiteSpace(pCustomerID) Then
        '    Return JsonDictionary.Serialize("Error", "SupplierID is required. ref: vbVA6CPEY9J2!")
        'End If

        'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        ComposeCustomer(pCustomerSelectSql, pCustomerFromSql)

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransReceivingPayment.ID")
                .Add("PaymentDate", SqlDbType.DateTime, "TransReceivingPayment.PaymentDate")
                .Add("CashAmount", SqlDbType.Money, "TransReceivingPayment.CashAmount")
                .Add("CheckPaymentAmount", SqlDbType.Money, "TransReceivingPayment.ChecksAmount")
                .Add("CreditCardAmount", SqlDbType.Money, "TransReceivingPayment.CreditCardAmount")
                .Add("DebitCardAmount", SqlDbType.Money, "TransReceivingPayment.DebitCardAmount")
                .Add("SupplierName", SqlDbType.VarChar, pCustomerSelectSql)
                With .Add("TotalPayment", SqlDbType.Money)
                    .Expression = "TransReceivingPayment.CashAmount + TransReceivingPayment.ChecksAmount + TransReceivingPayment.CreditCardAmount + TransReceivingPayment.DebitCardAmount" & vbCrLf &
                        " + ISNULL((SELECT SUM(TransReceivingPaymentOther.Amount)" & vbCrLf &
                        " 	FROM TransReceivingPaymentOther" & vbCrLf &
                        "   WHERE TransReceivingPaymentOther.PaymentID = TransReceivingPayment.ID" & vbCrLf &
                        " ), 0)"
                End With
            End With
            .SqlFrom = " FROM TransReceivingPayment" & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransReceivingPayment.SupplierID" & vbCrLf &
                pCustomerFromSql & vbCrLf
            If CStr2(pCustomerID) <> "" Then
                .SqlWhere = " WHERE TransReceivingPayment.SupplierID = " & ReadyString(pCustomerID) & vbCrLf
            End If
            .SqlOrder = " ORDER BY TransReceivingPayment.PaymentDate DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function GetCollectionReport() As String
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT As DataTable
        Dim pCustomerSelect, pCustomerFrom As String
        ComposeCustomer(pCustomerSelect, pCustomerFrom, "cus", "cussuf")
        Dim pFowstID As String = Common.WebRequest.Data("FowstID")

        With CreateFinder()
            .Distinct = True
            .MaxRecords = 0
            With .Fields
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = pCustomerSelect
                End With
                With .Add("InvoiceNumber", SqlDbType.VarChar)
                    .Expression = "ISNULL(doccon.DocumentName, CASE" & vbCrLf &
                                    "      	WHEN ISNULL(inv.InvoiceNumber, 0) > 0 THEN 'SI'" & vbCrLf &
                                    "         WHEN ISNULL(inv.SalesOrderNo, 0) > 0 THEN 'SO'" & vbCrLf &
                                    "         WHEN ISNULL(inv.PurchaseOrderNo, 0) > 0 THEN 'PO'" & vbCrLf &
                                    "         ELSE ''" & vbCrLf &
                                    "         END) + '-' +" & vbCrLf &
                                    "      	+ ISNULL(CONVERT(varchar, fowst.FowstNo), '') + '-'" & vbCrLf &
                                    "         + ISNULL(CONVERT(varchar, device.DeviceNo), '') + '-'" & vbCrLf &
                                    "         + CONVERT(varchar, " & vbCrLf &
                                    " CASE" & vbCrLf &
                                    "       	 WHEN ISNULL(inv.InvoiceNumber, 0) > 0 THEN inv.InvoiceNumber" & vbCrLf &
                                    "          WHEN ISNULL(inv.SalesOrderNo, 0) > 0 THEN inv.SalesOrderNo" & vbCrLf &
                                    "          WHEN ISNULL(inv.PurchaseOrderNo, 0) > 0 THEN inv.PurchaseOrderNo" & vbCrLf &
                                    "          ELSE 0" & vbCrLf &
                                    "          END" & vbCrLf &
                                    ")" & vbCrLf
                End With
                With .Add("RefInvoiceNum", SqlDbType.VarChar)
                    .Expression = "inv.RefInvoiceNum"
                End With
                .Add("ManualReceiptNumber", SqlDbType.VarChar, "ISNULL(cuspay.ManualReceiptNumber, '')")
                .Add("AccountName", SqlDbType.VarChar, "ISNULL(chk.AccountName, '')")
                .Add("BankName", SqlDbType.VarChar, "ISNULL(chk.BankName, '')")
                .Add("BankBranch", SqlDbType.VarChar, "ISNULL(chk.BankBranchName, '')")
                .Add("CheckNumber", SqlDbType.VarChar, "ISNULL(chk.CheckNumber, '')")
                .Add("CheckAmount", SqlDbType.Money, "ISNULL(chk.CheckAmount, '')")
                .Add("CheckDate", SqlDbType.DateTime, "ISNULL(CONVERT(varchar, chk.CheckDate, 126), '')") ' 126 = ISO format. http://stackoverflow.com/a/16434602/334045
                .Add("CheckTotal", SqlDbType.Money, "ISNULL(chk.CheckAmount, '')")

                .Add("AccountNameOnline", SqlDbType.VarChar, "ISNULL(chk.AccountNameOnline, '')")
                .Add("BankNameOnline", SqlDbType.VarChar, "ISNULL(chk.BankNameOnline, '')")
                .Add("BankBranchOnline", SqlDbType.VarChar, "ISNULL(chk.BankBranchNameOnline, '')")
                .Add("CheckNumberOnline", SqlDbType.VarChar, "ISNULL(chk.CheckNumberOnline, '')")
                .Add("CheckAmountOnline", SqlDbType.Money, "ISNULL(chk.CheckAmountOnline, '')")
                .Add("CheckDateOnline", SqlDbType.DateTime, "ISNULL(CONVERT(varchar, chk.CheckDateOnline, 126), '')") ' 126 = ISO format. http://stackoverflow.com/a/16434602/334045
                .Add("CheckTotalOnline", SqlDbType.Money, "ISNULL(chk.CheckAmountOnline, '')")

                .Add("AccountNameOnline2", SqlDbType.VarChar, "ISNULL(chk.AccountNameOnline2, '')")
                .Add("BankNameOnline2", SqlDbType.VarChar, "ISNULL(chk.BankNameOnline2, '')")
                .Add("BankBranchOnline2", SqlDbType.VarChar, "ISNULL(chk.BankBranchNameOnline2, '')")

                .Add("CashAmount", SqlDbType.Money, "ISNULL(cuspay.CashAmount, '')")
                .Add("PaymentDate", SqlDbType.DateTime, "cuspay.PaymentDate")
                .Add("LastUpdated", SqlDbType.DateTime, "cuspay.LastUpdated")
                .Add("ReceiptNumber", SqlDbType.VarChar, "cuspay.ReceiptType + '-'" & vbCrLf &
                    " + ISNULL(CONVERT(varchar, fowst.FowstNo), '') + '-'" & vbCrLf &
                    " + ISNULL(CONVERT(varchar, device.DeviceNo), '') + '-'" & vbCrLf &
                    " + CONVERT(varchar, cuspay.ReceiptNumber)")
                .Add("FowstName", SqlDbType.VarChar, "ISNULL(fowst.Name, '')")
                .Add("Total", SqlDbType.Money, "ISNULL(chk.CheckAmount, 0) + ISNULL(chk.CheckAmountOnline, 0) + cuspay.CashAmount")
                .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier, "cuspay.ID")
                .Add("InvoiceID", SqlDbType.UniqueIdentifier, "TransCustomerPaymentInvoices.InvoiceID")
                .Add("CustomerID", SqlDbType.UniqueIdentifier, "inv.CustomerID")
                .Add("CheckID", SqlDbType.UniqueIdentifier, "chk.ID")
                .Add("GrossInvoiceAmount", SqlDbType.Decimal, "CacheInvoices.GrossInvoiceAmount")
                .Add("NetInvoiceAmount", SqlDbType.Decimal, "CacheInvoices.NetInvoiceAmount")
                .Add("TotalPayment", SqlDbType.Decimal, "CacheInvoices.TotalPayment")
                .Add("CheckAmountPaid", SqlDbType.Decimal, "chkamt.AmountPaid")
                .Add("InvoiceDueDate", SqlDbType.DateTime2, "CacheInvoices.InvoiceDueDate")
                '.Add("AccountName", SqlDbType.VarChar, "tblLibraryOrganizationFinancialDetails.AccountName") ' commented 2021_11_18__11_02_54_1448
            End With
            .SqlFrom = " FROM TransCustomerPayment as cuspay" & vbCrLf &
                "  LEFT JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = cuspay.ID" & vbCrLf

            .SqlFrom &= "   LEFT JOIN (" & vbCrLf &
                "   	SELECT TransCustomerPaymentChecks.ID" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CustomerPaymentID" & vbCrLf &
                " 	,TransCustomerPaymentChecks.IsOnlinePayment" & vbCrLf &
                "     ,TransCustomerPaymentChecks.AccountName" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankID" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankName" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankBranchName" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CheckNumber" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CheckDate" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CheckAmount" & vbCrLf &
                " 	  ,NULL AS AccountNameOnline" & vbCrLf &
                "     ,NULL AS BankIDOnline" & vbCrLf &
                "     ,NULL AS BankNameOnline" & vbCrLf &
                "     ,NULL AS BankBranchNameOnline" & vbCrLf &
                "     ,NULL AS CheckNumberOnline" & vbCrLf &
                "     ,NULL AS CheckDateOnline" & vbCrLf &
                "     ,NULL AS CheckAmountOnline" & vbCrLf &
                "     ,NULL AS AccountNameOnline2" & vbCrLf &
                "     ,NULL AS BankIDOnline2" & vbCrLf &
                "     ,NULL AS BankNameOnline2" & vbCrLf &
                "     ,NULL AS BankBranchNameOnline2" & vbCrLf &
                "     FROM TransCustomerPaymentChecks" & vbCrLf &
                "     WHERE ISNULL(TransCustomerPaymentChecks.IsOnlinePayment, 0) = 0" & vbCrLf &
                "     UNION" & vbCrLf &
                "     SELECT TransCustomerPaymentChecks.ID" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CustomerPaymentID" & vbCrLf &
                " 	  ,TransCustomerPaymentChecks.IsOnlinePayment" & vbCrLf &
                "     ,NULL AS AccountName" & vbCrLf &
                "     ,NULL AS BankID" & vbCrLf &
                "     ,NULL AS BankName" & vbCrLf &
                "     ,NULL AS BankBranchName" & vbCrLf &
                "     ,NULL AS CheckNumber" & vbCrLf &
                "     ,NULL AS CheckDate" & vbCrLf &
                "     ,NULL AS CheckAmount" & vbCrLf &
                "     ,TransCustomerPaymentChecks.AccountName AS AccountNameOnline" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankID AS BankIDOnline" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankName AS BankNameOnline" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankBranchName AS BankBranchNameOnline" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CheckNumber AS CheckNumberOnline" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CheckDate AS CheckDateOnline" & vbCrLf &
                "     ,TransCustomerPaymentChecks.CheckAmount AS CheckAmountOnline" & vbCrLf &
                "     ,TransCustomerPaymentChecks.AccountName2 AS AccountNameOnline2" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankID2 AS BankIDOnline2" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankName2 AS BankNameOnline2" & vbCrLf &
                "     ,TransCustomerPaymentChecks.BankBranchName2 AS BankBranchNameOnline2" & vbCrLf &
                "     FROM TransCustomerPaymentChecks" & vbCrLf &
                "     WHERE ISNULL(TransCustomerPaymentChecks.IsOnlinePayment, 0) = 1" & vbCrLf &
                "   ) as chk ON chk.CustomerPaymentID = cuspay.ID" & vbCrLf

            .SqlFrom &= "  LEFT JOIN TransCustomerPaymentCheckAmount as chkamt ON chk.ID = chkamt.CustomerPaymentCheckID" & vbCrLf &
                "  LEFT JOIN TransInvoice as inv ON inv.ID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
                "  -- 2020_10_20__08_27_11_8431 delete this line AND chkamt.InvoiceID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
                "  LEFT JOIN CacheInvoices ON CacheInvoices.ID = inv.ID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganization as cus ON cus.ID = cuspay.CustomerID" & vbCrLf &
                "  LEFT JOIN MobizLibraryPeoplePreNominalSuffix as cussuf ON cus.PersonTitleID = cussuf.ID" & vbCrLf &
                "  LEFT JOIN LibraryDocumentControl as doccon ON doccon.ID = inv.DocumentNameID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = inv.FowstID" & vbCrLf &
                "  LEFT JOIN LibraryDevice as device ON device.ID = inv.DeviceID" & vbCrLf

            ' commented 2021_11_18__11_02_54_1448
            '.SqlFrom &= "   OUTER APPLY (" & vbCrLf &
            '    "   	SELECT TOP 1 LibraryOrganizationFinancialDetails.AccountName" & vbCrLf &
            '    "     FROM LibraryOrganizationFinancialDetails" & vbCrLf &
            '    "     WHERE LibraryOrganizationFinancialDetails.CustomerOrgID = cuspay.CustomerID" & vbCrLf &
            '    "     AND LibraryOrganizationFinancialDetails.BankOrgID = chk.BankID" & vbCrLf &
            '    "   ) tblLibraryOrganizationFinancialDetails" & vbCrLf

            .SqlWhere = " WHERE cuspay.LastUpdated >= " & ReadyString(Common.WebRequest.Data("From") + " 00:00:00 ") & " AND cuspay.LastUpdated <= " & ReadyString(Common.WebRequest.Data("To") + " 23:59:59 ") & vbCrLf &
                " AND (" & vbCrLf &
                "  	    chkamt.id is null" & vbCrLf &
                "   	OR chkamt.InvoiceID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
                " )" & vbCrLf
            If IsGuid(pFowstID) Then
                .SqlWhere &= " AND (" & vbCrLf &
                    "   cuspay.FowstID = " & ReadyString(pFowstID) &
                    "   OR (cuspay.FowstID IS NULL AND inv.FowstID = " & ReadyString(pFowstID) & ")" & vbCrLf &
                    ")" & vbCrLf
            End If
            .SqlOrder = " ORDER BY cuspay.LastUpdated ASC"
            pDT = .Requery
            'pDT = ResortCollectionReport(pDT) ' commented on 2018_02_07__13_55_07_6191
        End With

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

#Region " ResortCollectionReport "
    Private Function ResortCollectionReport(vDT As DataTable) As DataTable

        Dim pOut As DataTable
        Dim pDrOut As DataRow
        Dim pDV As DataView
        Dim pOrdinal As Integer
        Dim pValid As Boolean
        Dim pBlankInvoice As Boolean
        Dim pBlankCheck As Boolean
        Dim pDrExisting As DataRow

        pOut = vDT.Clone
        pOut.Columns.Add("Ordinal", GetType(Integer))
        pDV = New DataView(vDT, "", "CustomerName, PaymentDate, InvoiceDate, CustomerID, CustomerPaymentID", DataViewRowState.CurrentRows)
        For Each pDRV As DataRowView In pDV
            pValid = False
            pBlankInvoice = False
            pBlankCheck = False
            pDrExisting = Nothing
            pOrdinal += 1
            pDrOut = pOut.NewRow
            pDrOut.ItemArray = pDRV.Row.ItemArray
            pDrOut("Ordinal") = pOrdinal
            If ResortCollectionReportHasCustomer(pOut, CStr2(pDrOut("CustomerID"))) Then
                'pDrOut("CustomerID") = DBNull.Value
                pDrOut("CustomerName") = DBNull.Value
            Else
                pValid = True
            End If
            If ResortCollectionReportHasCustomerPaymentID(pOut, CStr2(pDrOut("CustomerPaymentID"))) Then
                pDrOut("PaymentDate") = DBNull.Value
                pDrOut("ReceiptNumber") = DBNull.Value
                pDrOut("ManualReceiptNumber") = DBNull.Value
                pDrOut("CashAmount") = DBNull.Value
                pDrOut("CheckTotal") = DBNull.Value
                pDrOut("Total") = DBNull.Value
            Else
                pValid = True
            End If
            If ResortCollectionReportHasInvoiceID(pOut, CStr2(pDrOut("InvoiceID"))) Then
                pDrOut("InvoiceID") = DBNull.Value
                pDrOut("InvoiceNumber") = DBNull.Value
                pDrOut("InvoiceDate") = DBNull.Value
                pBlankInvoice = True
            Else
                pValid = True
            End If
            If ResortCollectionReportHasCheckID(pOut, CStr2(pDrOut("CheckID"))) Then
                pDrOut("CheckID") = DBNull.Value
                pDrOut("CheckNumber") = DBNull.Value
                pDrOut("CheckAmount") = DBNull.Value
                pDrOut("BankName") = DBNull.Value
                pDrOut("BankBranch") = DBNull.Value
                pBlankCheck = True
            Else
                pValid = True
            End If
            If pValid Then
                If pBlankCheck AndAlso pBlankInvoice = False Then
                    pDrExisting = ResortCollectionReportGetWithBlankInvoice(pOut, CStr2(pDrOut("CustomerPaymentID")))
                    If pDrExisting IsNot Nothing Then
                        pDrExisting("InvoiceID") = pDrOut("InvoiceID")
                        pDrExisting("InvoiceNumber") = pDrOut("InvoiceNumber")
                        pDrExisting("InvoiceDate") = pDrOut("InvoiceDate")
                    End If
                ElseIf pBlankCheck = False AndAlso pBlankInvoice Then
                    pDrExisting = ResortCollectionReportGetWithBlankCheck(pOut, CStr2(pDrOut("CustomerPaymentID")))
                    If pDrExisting IsNot Nothing Then
                        pDrExisting("CheckID") = pDrOut("CheckID")
                        pDrExisting("CheckNumber") = pDrOut("CheckNumber")
                        pDrExisting("CheckAmount") = pDrOut("CheckAmount")
                        pDrExisting("BankName") = pDrOut("BankName")
                        pDrExisting("BankBranch") = pDrOut("BankBranch")
                    End If
                End If

                If pDrExisting Is Nothing Then
                    pOut.Rows.Add(pDrOut)
                End If
            Else
                pValid = True ' does nothing. breakpoit here
            End If
        Next pDRV

        ResortCollectionReportFillCheckTotal(pOut)

        Return pOut

    End Function

    Private Function ResortCollectionReportHasCustomer(vDT As DataTable, vCustomerID As String) As Boolean

        Return vDT.Select("CustomerID = " & ReadyString(vCustomerID)).Count > 0

    End Function

    Private Function ResortCollectionReportHasCustomerPaymentID(vDT As DataTable, vCustomerPaymentID As String) As Boolean

        Return vDT.Select("CustomerPaymentID = " & ReadyString(vCustomerPaymentID)).Count > 0

    End Function

    Private Function ResortCollectionReportHasInvoiceID(vDT As DataTable, vInvoiceID As String) As Boolean

        Return vDT.Select("InvoiceID = " & ReadyString(vInvoiceID)).Count > 0

    End Function

    Private Function ResortCollectionReportHasCheckID(vDT As DataTable, vCheckID As String) As Boolean

        Return vDT.Select("CheckID = " & ReadyString(vCheckID)).Count > 0

    End Function

    Private Function ResortCollectionReportGetWithBlankInvoice(vDT As DataTable, vCustomerPaymentID As String) As DataRow

        Dim pRows() As DataRow

        pRows = vDT.Select("InvoiceID IS NULL AND CustomerPaymentID = " & ReadyString(vCustomerPaymentID))
        If pRows.Count > 0 Then
            Return pRows(0)
        End If

    End Function

    Private Function ResortCollectionReportGetWithBlankCheck(vDT As DataTable, vCustomerPaymentID As String) As DataRow

        Dim pRows() As DataRow

        pRows = vDT.Select("CheckID IS NULL AND CustomerPaymentID = " & ReadyString(vCustomerPaymentID))
        If pRows.Count > 0 Then
            Return pRows(0)
        End If

    End Function

    Private Function ResortCollectionReportGetCheckTotal(vDT As DataTable, vCustomerPaymentID As String) As Decimal

        Return CDec2(vDT.Compute("SUM(CheckAmount)", "CustomerPaymentID = " & ReadyString(vCustomerPaymentID)))

    End Function

    Private Sub ResortCollectionReportFillCheckTotal(ByRef rDT As DataTable)

        Dim pDV As DataView

        pDV = New DataView(rDT, "PaymentDate IS NOT NULL", "", DataViewRowState.CurrentRows)
        For Each pDRV As DataRowView In pDV
            pDRV("CheckTotal") = ResortCollectionReportGetCheckTotal(rDT, CStr2(pDRV("CustomerPaymentID")))
            pDRV("Total") = CDec2(pDRV("CashAmount")) + pDRV("CheckTotal")
        Next pDRV

    End Sub
#End Region

    <HttpPost>
    Public Function CustomerPaymentReport() As String

        Dim pStrArr As String()

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pCustomerID As String = Common.WebRequest.Data("CustomerID")
        Dim pEncodingDateRange As String = Common.WebRequest.Data("EncodingDateRange")
        Dim pEncodingDateFrom, pEncodingDateTo As Date?
        Dim pDoNotFilterEncodingDate As Boolean = CBool2(Common.WebRequest.Data("DoNotFilterEncodingDate"))
        Dim pMaturityDateRange As String = Common.WebRequest.Data("MaturityDateRange")
        Dim pMaturityDateFrom, pMaturityDateTo As Date?
        Dim pDoNotFilterMaturityDate As Boolean = CBool2(Common.WebRequest.Data("DoNotFilterMaturityDate"))
        Dim pDepositDateRange As String = Common.WebRequest.Data("DepositDateRange")
        Dim pDepositDateFrom, pDepositDateTo As Date?
        Dim pDoNotFilterDepositDate As Boolean = CBool2(Common.WebRequest.Data("DoNotFilterDepositDate"))
        Dim pIncludeCash As Boolean = CBool2(Common.WebRequest.Data("IncludeCash"))
        Dim pIncludeChecks As Boolean = CBool2(Common.WebRequest.Data("IncludeChecks"))
        Dim pIncludeCreditCards As Boolean = CBool2(Common.WebRequest.Data("IncludeCreditCards"))
        Dim pIncludeDebitCards As Boolean = CBool2(Common.WebRequest.Data("IncludeDebitCards"))
        Dim pIncludeSrv As Boolean = CBool2(Common.WebRequest.Data("IncludeSrv"))
        Dim pIncludeDeposited As Boolean = CBool2(Common.WebRequest.Data("IncludeDeposited"))
        Dim pIncludeReturned As Boolean = CBool2(Common.WebRequest.Data("IncludeReturned"))
        Dim pIncludeUndeposited As Boolean = CBool2(Common.WebRequest.Data("IncludeUndeposited"))
        Dim pDownloadAsCustomerPaymentPdf As Boolean = CBool2(Common.WebRequest.Data("DownloadAsCustomerPaymentPdf"))
        Dim pDownloadAsDepositsPdf As Boolean = CBool2(Common.WebRequest.Data("DownloadAsDepositsPdf"))
        Dim pDepositGroupID As String = Common.WebRequest.Data("DepositGroupID")
        Dim pUserID As String = CStr2(Common.WebRequest.Data("UserID"))
        Dim pSalesRepID As String = CStr2(Common.WebRequest.Data("SalesRepID"))
        Dim pSalesChannelID As String = CStr2(Common.WebRequest.Data("SalesChannelID"))
        Dim pRowsPerPage As String = CInt2(Common.WebRequest.Data("RowsPerPage"))
        Dim pSearchKeywords As String = Common.WebRequest.Data("keywords")
        Dim pGroupByHeader As String = CBool2(Common.WebRequest.Data("GroupByHeader"))
        Dim pPaymentFowstID As String = Common.WebRequest.Data("PaymentFowstID") ' optional

        Dim pDT As DataTable
        Dim pUuid As String

        pUuid = Common.WebRequest.Data("UUID")
        If String.IsNullOrWhiteSpace(pUuid) Then
            pUuid = "no_uuid"
        End If

        If IsGuid(pDepositGroupID) = False Then
            If pDoNotFilterEncodingDate = False Then
                If Client.IsTodayOrEalier(pEncodingDateRange) Then
                    pEncodingDateTo = Now.Date
                Else
                    pStrArr = pEncodingDateRange.Split({" to "}, StringSplitOptions.RemoveEmptyEntries)
                    If pStrArr.Count <> 2 Then
                        Return JsonDictionary.Serialize("Error", "vb6V5SXH82R3")
                    End If
                    pEncodingDateFrom = ParseDate(pStrArr(0))
                    pEncodingDateTo = ParseDate(pStrArr(1))
                End If
            End If

            If pDoNotFilterMaturityDate = False Then
                If Client.IsTodayOrEalier(pMaturityDateRange) Then
                    pMaturityDateTo = Now.Date
                Else
                    pStrArr = pMaturityDateRange.Split({" to "}, StringSplitOptions.RemoveEmptyEntries)
                    If pStrArr.Count <> 2 Then
                        Return JsonDictionary.Serialize("Error", "vbF9KCJKWGV9")
                    End If
                    pMaturityDateFrom = ParseDate(pStrArr(0))
                    pMaturityDateTo = ParseDate(pStrArr(1))
                End If
            End If

            If pDoNotFilterDepositDate = False Then
                If Client.IsTodayOrEalier(pDepositDateRange) Then
                    pDepositDateTo = Now.Date
                Else
                    pStrArr = pDepositDateRange.Split({" to "}, StringSplitOptions.RemoveEmptyEntries)
                    If pStrArr.Count <> 2 Then
                        Return JsonDictionary.Serialize("Error", "vbU4S296M4MQ")
                    End If
                    pDepositDateFrom = ParseDate(pStrArr(0))
                    pDepositDateTo = ParseDate(pStrArr(1))
                End If
            End If
        End If

        pDT = WebRequest.Instance.Business.TransInvoice.CustomerPaymentReport(
            vPaymentDateFrom:=pEncodingDateFrom,
            vPaymentDateTo:=pEncodingDateTo,
            vMaturityDateFrom:=pMaturityDateFrom,
            vMaturityDateTo:=pMaturityDateTo,
            vDepositDateFrom:=pDepositDateFrom,
            vDepositDateTo:=pDepositDateTo,
            vExcludeCash:=Not pIncludeCash,
            vExcludeChecks:=Not pIncludeChecks,
            vExcludeCreditCard:=Not pIncludeCreditCards,
            vExcludeDebitCard:=Not pIncludeDebitCards,
            vExcludeSrv:=Not pIncludeSrv,
            vCustomerID:=pCustomerID,
            vExcludeDeposited:=Not pIncludeDeposited,
            vExcludeReturned:=Not pIncludeReturned,
            vExcludeUndeposited:=Not pIncludeUndeposited,
            vDepositGroupID:=pDepositGroupID,
            vUserID:=pUserID,
            vSalesRepID:=pSalesRepID,
            vSalesChannelID:=pSalesChannelID,
            vSearchKeywords:=pSearchKeywords,
            vGroupByHeader:=pGroupByHeader,
            vPaymentFowstID:=pPaymentFowstID
        )

        If pDownloadAsCustomerPaymentPdf OrElse pDownloadAsDepositsPdf Then
            Dim pReport As ProReport
            Dim pFilename As String
            Dim pBaseFilename As String

            pReport = New ProReport
            pReport.AutoFillHeader = False
            If pDownloadAsCustomerPaymentPdf Then
                pReport.SourceFilename = "CustomerPaymentReport"
                pReport.Variables("RowsPerPage") = pRowsPerPage
            Else
                pReport.SourceFilename = "Deposits"
            End If
            pReport.AddTable("Data", pDT)
            pReport.ExportXmlFile()
            pBaseFilename = DateTimeStamp() & ".pdf" ' remove the CreateValidFilename(pUuid) & "-" & to prevent printing to WPA
            pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename
            CreateFolder(pFilename)
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

            Return JsonDictionary.Serialize("Filename", pBaseFilename)
        Else
            Return JsonDictionary.Serialize("Results", pDT)
        End If

    End Function

#Region " for DTR project "
    '<HttpPost>
    Public Function PayrollPeriods() As String

        With CreateFinder()
            .ProData.ExecuteSQL("USE DavaoUnifiedPayroll")
            With .Fields
                .Add("ID", SqlDbType.Int, "PayrollPeriod.ID")
                .Add("Period", SqlDbType.VarChar, "PayrollPeriod.Period")
                .Add("PeriodFrom", SqlDbType.DateTime, "PayrollPeriod.PeriodFrom")
                .Add("PeriodTo", SqlDbType.DateTime, "PayrollPeriod.PeriodTo")
            End With
            .SqlFrom = " FROM PayrollPeriod"
            .SqlOrder = " ORDER BY PayrollPeriod.PeriodFrom, PayrollPeriod.PeriodTo"
        End With

        Return SubmitFinder()

    End Function

    '<HttpPost>
    Public Function DtrBranches() As String

        With CreateFinder()
            .ProData.ExecuteSQL("USE DavaoUnifiedPayroll")
            With .Fields
                .Add("ID", SqlDbType.Int, "LibraryBranch.ID")
                .Add("Name", SqlDbType.VarChar, "LibraryBranch.Name")
            End With
            .SqlFrom = " FROM LibraryBranch"
            .SqlOrder = " ORDER BY LibraryBranch.Name"
        End With

        Return SubmitFinder()

    End Function
#End Region ' " for DTR project "

    <HttpPost>
    Public Function LibraryDepositAccounts() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "LibraryDepositAccounts.ID")
                .Add("AccountName", SqlDbType.VarChar, "LibraryDepositAccounts.AccountName")
                .Add("AccountNumber", SqlDbType.VarChar, "LibraryDepositAccounts.AccountNumber")
            End With
            .SqlFrom = " FROM LibraryDepositAccounts" & vbCrLf
            .SqlOrder = " ORDER BY LibraryDepositAccounts.AccountName, LibraryDepositAccounts.AccountNumber"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost>
    Public Function Areas() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pIncludeAllAreas = CBool2(Common.WebRequest.Data("IncludeAllAreas"))
        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("Area", SqlDbType.VarChar)
            End With
            .SqlFrom = " FROM LibraryFowstBin" & vbCrLf
            .SqlWhere = " WHERE FowstID = " & ReadyString(WebRequest.Instance.FowstID) & vbCrLf &
                " AND LTRIM(RTRIM(ISNULL(Area, ''))) <> ''" & vbCrLf
            .SqlOrder = " ORDER BY Area"
        End With

        Dim pSql = mFinder.CreateSql()
        Dim pDT = mFinder.ProData.Execute(pSql)
        If pIncludeAllAreas Then
            Dim pDR = pDT.NewRow()
            pDR("Area") = "[All Areas]"
            pDT.Rows.InsertAt(pDR, 0)
        End If

        Return JsonDictionary.Serialize("Results", pDT)


    End Function

    <HttpPost>
    Public Function TransFlushingReport() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        WebRequest.Instance.Business.TransFlushingReport.ExtractLogs()
        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("CompanyName", SqlDbType.VarChar)
                .Add("DeviceName", SqlDbType.VarChar)
                .Add("ProcessName", SqlDbType.VarChar)
                .Add("ProcessDuration", SqlDbType.Int)
                .Add("ProcessStart", SqlDbType.DateTime2)
                .Add("ProcessEnd", SqlDbType.DateTime2)
            End With
            .SqlFrom = " FROM MobizTransFlushingReport" & vbCrLf
            .SqlWhere = " WHERE ProcessDuration >= " & CInt2(Common.WebRequest.Data("MinimumDuration"))
            .SqlOrder = " ORDER BY ProcessDuration DESC"
        End With

        Return SubmitFinder()

        Dim pSql = mFinder.CreateSql()
        Dim pDT = mFinder.ProData.Execute(pSql)
        Dim pDR = pDT.NewRow()
        pDR("Area") = "[All Areas]"
        pDT.Rows.InsertAt(pDR, 0)
        Return JsonDictionary.Serialize("Results", pDT)


    End Function

    <HttpPost>
    Public Function TransProfitability() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDateFrom = ParseDate(Common.WebRequest.Data("LastModifiedFrom"))
        Dim pDateTo = ParseDate(Common.WebRequest.Data("LastModifiedTo"))
        Dim pApprovedStatus = CStr2(Common.WebRequest.Data("ApprovedStatus")).Trim().ToLower()

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransProfitability.ID")
                .Add("CustomerID", SqlDbType.UniqueIdentifier, "TransInvoice.CustomerID")
                .Add("TransType", SqlDbType.VarChar, "TransInvoice.TransType")
                .Add("InvoiceID", SqlDbType.UniqueIdentifier, "TransProfitability.InvoiceID")
                .Add("ItemID", SqlDbType.UniqueIdentifier, "TransProfitability.ItemID")
                .Add("TransMin", SqlDbType.VarChar, "TransProfitability.TransMin")
                .Add("TransDate", SqlDbType.DateTime2, "TransProfitability.TransDate")
                .Add("CreatedByName", SqlDbType.VarChar, "TransProfitability.CreatedByName")
                .Add("ProductName", SqlDbType.VarChar, "TransProfitability.ProductName")
                .Add("OrderQuantity", SqlDbType.Decimal, "TransProfitability.OrderQuantity")
                .Add("UnitMeasure", SqlDbType.VarChar, "TransProfitability.UnitMeasure")
                .Add("COGS", SqlDbType.Decimal, "TransProfitability.COGS")
                .Add("CogsLastUpdated", SqlDbType.DateTime, "TransProfitability.CogsLastUpdated")
                .Add("SellingPrice", SqlDbType.Decimal, "TransProfitability.SellingPrice")
                .Add("GpmThreshold", SqlDbType.Decimal, "TransProfitability.GpmThreshold")
                .Add("GP", SqlDbType.Decimal, "TransProfitability.GP")
                .Add("GPM", SqlDbType.Decimal, "TransProfitability.GPM")
                .Add("LineGp", SqlDbType.Decimal, "TransProfitability.LineGp")
                .Add("UnapprovedTransTotalGp", SqlDbType.Decimal, "TransProfitability.UnapprovedTransTotalGp")
                .Add("UnapprovedTransTotalGpm", SqlDbType.Decimal, "TransProfitability.UnapprovedTransTotalGpm")
                .Add("ApprovedSellingPrice", SqlDbType.Decimal, "TransProfitability.ApprovedSellingPrice")
                .Add("ApprovedGp", SqlDbType.Decimal, "TransProfitability.ApprovedGp")
                .Add("ApprovedGpm", SqlDbType.Decimal, "TransProfitability.ApprovedGpm")
                .Add("ApprovedLineGp", SqlDbType.Decimal, "TransProfitability.ApprovedLineGp")
                .Add("ApprovedTransTotalGp", SqlDbType.Decimal, "TransProfitability.ApprovedTransTotalGp")
                .Add("ApprovedTransTotalGpm", SqlDbType.Decimal, "TransProfitability.ApprovedTransTotalGpm")
                .Add("TransItemCount", SqlDbType.Decimal, "TransProfitability.TransItemCount")
                .Add("ApprovedByID", SqlDbType.UniqueIdentifier, "TransProfitability.ApprovedByID")
                .Add("ApprovedByName", SqlDbType.VarChar, "TransProfitability.ApprovedByName")
                .Add("ApprovedDate", SqlDbType.DateTime, "TransProfitability.ApprovedDate")
                .Add("ProductID", SqlDbType.DateTime, "TransProfitability.ProductID")
                With .Add("IsPriceFreeTyped", SqlDbType.Bit, "TransProfitability.IsPriceFreeTyped")
                    .Searchable = False
                End With
                .Add("SellingPriceLevel", SqlDbType.VarChar, "TransProfitability.SellingPriceLevel")
                .Add("SellingPriceLastUpdated", SqlDbType.DateTime, "TransProfitability.SellingPriceLastUpdated")
                .Add("CompanyName", SqlDbType.VarChar, "MobizLibraryOrganization.CompanyName")
            End With
            .SqlFrom = " FROM TransProfitability" & vbCrLf &
                " JOIN TransInvoice ON TransInvoice.ID = TransProfitability.InvoiceID" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoice.CustomerID" & vbCrLf
            .SqlWhere = " WHERE 1=1" & vbCrLf
            If pDateFrom IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransProfitability.TransDate) >= " & ReadyString(pDateFrom.Value.Date) & vbCrLf
            End If
            If pDateTo IsNot Nothing Then
                .SqlWhere &= " AND CONVERT(date, TransProfitability.TransDate) <= " & ReadyString(pDateTo.Value.Date) & vbCrLf
            End If
            If pApprovedStatus = "unapproved" Then
                .SqlWhere &= " AND TransProfitability.ApprovedByID IS NULL" & vbCrLf
            ElseIf pApprovedStatus = "approved" Then
                .SqlWhere &= " AND TransProfitability.ApprovedByID IS NOT NULL" & vbCrLf
            End If
            .SqlOrder = " ORDER BY TransProfitability.TransDate DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost>
    Public Function TransVendorPriceHistory() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDateFrom = ParseDate(Common.WebRequest.Data("LastModifiedFrom"))
        Dim pDateTo = ParseDate(Common.WebRequest.Data("LastModifiedTo"))
        Dim pApprovedStatus = CStr2(Common.WebRequest.Data("ApprovedStatus")).Trim().ToLower()

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier, "TransVendorPriceHistory.ID")
                .Add("ProductNo", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductNo")
                .Add("ProductName", SqlDbType.VarChar, "MobizLibraryPIMProduct.ProductName")
                .Add("CompanyName", SqlDbType.VarChar, "MobizLibraryOrganization.CompanyName")
                .Add("PreviousPrice", SqlDbType.Money, "TransVendorPriceHistory.PreviousPrice")
                .Add("CurrentPrice", SqlDbType.Money, "TransVendorPriceHistory.CurrentPrice")
                .Add("HistoryCreationDate", SqlDbType.DateTime, "TransVendorPriceHistory.HistoryCreationDate")
                .Add("FullTransNum", SqlDbType.VarChar,
                    "CASE TransVendorPriceHistory.TransType" & vbCrLf &
                    "WHEN 'PurchaseOrder' THEN 'PO ' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, ISNULL(TransInvoice.PurchaseOrderNo, 0))" & vbCrLf &
                    "WHEN 'AccountsPayable' THEN 'AP ' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, ISNULL(TransInvoice.AccountsPayableNum, 0))" & vbCrLf &
                    "ELSE TransVendorPriceHistory.TransType" & vbCrLf &
                    "END")
                With .Add("Tagged", SqlDbType.Bit)
                    .Expression = "CASE WHEN TransVendorPriceHistory.TagByID IS NULL THEN 0" & vbCrLf &
                         "ELSE 1" & vbCrLf &
                         "END"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "  FROM TransVendorPriceHistory" & vbCrLf &
                "  LEFT JOIN TransInvoice ON TransInvoice.ID = TransVendorPriceHistory.TransID" & vbCrLf &
                "  JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransVendorPriceHistory.ProductID" & vbCrLf &
                "  JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransVendorPriceHistory.VendorID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
                "  LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf
            .SqlWhere = " WHERE 1=1" & vbCrLf

            .SqlOrder = " ORDER BY TransVendorPriceHistory.HistoryCreationDate DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost>
    Public Function ProfitDetails() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pInvoiceYear = CInt2(Common.WebRequest.Data("InvoiceYear"))
        Dim pCustomerID = Common.WebRequest.Data("CustomerID")
        Dim pProductID = Common.WebRequest.Data("ProductID")

        With CreateFinder()
            .Distinct = True
            .MaxRecords = 0
            With .Fields
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = "(CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END)"
                End With
                With .Add("FullInvoiceNumber", SqlDbType.VarChar)
                    .Expression = "ISNULL(LibraryDocumentControl.DocumentName, 'SI') + ' ' + CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber)"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.InvoiceDate"
                End With
                With .Add("InvoiceID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoiceItems.InvoiceID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("Quantity", SqlDbType.Decimal)
                    .Expression = "TransInvoiceItems.Quantity"
                End With
                With .Add("OrderedQuantity", SqlDbType.Decimal)
                    .Expression = "TransInvoiceItems.OrderedQuantity"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "TransInvoiceItems.UnitMeasure"
                End With
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPIMMeasure.ID"
                End With
                With .Add("UnitPrice", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.UnitPrice"
                End With
                With .Add("SubTotal", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.SubTotal"
                End With
                With .Add("OrderedAmount", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity"
                End With
                With .Add("ServedAmount", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity"
                End With
                With .Add("ActualQuantity", SqlDbType.Decimal)
                    .Expression = "TransInvoiceItems.ActualQuantity"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoiceItems.ProductID"
                End With
                With .Add("PriceLevelID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoiceItems.PriceLevelID"
                End With
                With .Add("Cost", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.Cost"
                End With
                With .Add("LineCost", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.LineCost"
                End With
                With .Add("UnitGP", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.UnitGP"
                End With
                With .Add("UnitGPM", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.UnitGPM"
                End With
                With .Add("LineGP", SqlDbType.Money)
                    .Expression = "TransInvoiceItems.LineGP"
                End With
            End With
            .SqlFrom = "  FROM TransInvoiceItems" & vbCrLf &
                "  JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
                "  LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "  LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.Label = TransInvoiceItems.UnitMeasure" & vbCrLf &
                "  LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID" & vbCrLf &
                "  OUTER APPLY (" & vbCrLf &
                "  SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                "  	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty" & vbCrLf &
                "      FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "      JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                "      JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "      WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "      ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                "  ) tblBaseUnit" & vbCrLf &
                " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
                " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST InvFowst ON TransInvoice.FowstID = InvFowst.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix MobizLibraryPeoplePreNominalSuffix ON MobizLibraryOrganization.PersonTitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

            .SqlWhere = " WHERE TransInvoiceItems.Quantity > 0" & vbCrLf &
                " AND TransInvoice.InvoiceNumber > 0" & vbCrLf &
                " AND TransInvoice.TransType = 'Invoice'" & vbCrLf
            If pInvoiceYear <> 0 Then
                .SqlWhere &= " AND YEAR(TransInvoice.InvoiceDate) = " & pInvoiceYear & vbCrLf
            End If
            If String.IsNullOrEmpty(pCustomerID) = False Then
                .SqlWhere &= " AND TransInvoice.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
            End If
            If String.IsNullOrEmpty(pProductID) = False Then
                .SqlWhere &= " AND TransInvoiceItems.ProductID = " & ReadyString(pProductID) & vbCrLf
            End If
            .SqlOrder = " ORDER BY TransInvoice.InvoiceDate"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost>
    Public Function CrossTabReport() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pXAxis, pYAxis As TransInvoice.eCrossTabAxis

        If [Enum].TryParse(Of TransInvoice.eCrossTabAxis)(Common.WebRequest.Data("XAxis"), True, pXAxis) = False Then
            Return JsonDictionary.Serialize("Error", "XAxis must be out of the string in TransInvoice.eCrossTabAxis. ref: vb4EPPT2SGLK!")
        End If

        If [Enum].TryParse(Of TransInvoice.eCrossTabAxis)(Common.WebRequest.Data("YAxis"), True, pYAxis) = False Then
            Return JsonDictionary.Serialize("Error", "YAxis must be out of the string in TransInvoice.eCrossTabAxis. ref: vbMLP84JLXV3!")
        End If

        If pXAxis = pYAxis Then
            Return JsonDictionary.Serialize("Error", "XAxis and YAxis must not be the same.")
        End If

        Dim pDateFrom As String = Common.WebRequest.Data("DateFrom")
        If IsDate(pDateFrom) = False Then
            pDateFrom = Nothing
        End If
        Dim pDateTo As String = Common.WebRequest.Data("DateTo")
        If IsDate(pDateTo) = False Then
            pDateTo = Nothing
        End If
        Dim pDateNeededFrom As String = Common.WebRequest.Data("DateNeededFrom")
        If IsDate(pDateNeededFrom) = False Then
            pDateNeededFrom = Nothing
        End If
        Dim pDateNeededTo As String = Common.WebRequest.Data("DateNeededTo")
        If IsDate(pDateNeededTo) = False Then
            pDateNeededTo = Nothing
        End If
        Dim pDateEncodedFrom As String = Common.WebRequest.Data("DateEncodedFrom")
        If IsDate(pDateEncodedFrom) = False Then
            pDateEncodedFrom = Nothing
        End If
        Dim pDateEncodedTo As String = Common.WebRequest.Data("DateEncodedTo")
        If IsDate(pDateEncodedTo) = False Then
            pDateEncodedTo = Nothing
        End If
        Dim pSalesFigure = Common.WebRequest.Data("SalesFigure")
        If pSalesFigure = "salesamount" Then
            pSalesFigure = "TransInvoiceItems.SubTotal"
        ElseIf pSalesFigure = "salesvolume" Then
            pSalesFigure = "TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity"
        Else
            Return JsonDictionary.Serialize("Error", "Invalid sales figure. ref: vbZXP84JBXV8!")
        End If
        Dim pDT = WebRequest.Instance.Business.TransInvoice.CrossTabReport(
            CreateFinder(),
            pXAxis, pYAxis,
            CDateNull(pDateFrom), CDateNull(pDateTo),
            CDateNull(pDateNeededFrom), CDateNull(pDateNeededTo),
            pSalesFigure,
            CDateNull(pDateEncodedFrom), CDateNull(pDateEncodedTo),
            vIncludeProducts:=CBool2(Common.WebRequest.Data("ShowProducts"))
        )

        If CBool2(Common.WebRequest.Data("Download")) Then
            Dim pKey = DateTimeStamp()
            Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp\PrintablePdf\CrossTabReport_" & pKey & ".xls")
            CreateFolder(pFilename)
            Dim pWB As Aspose.Cells.Workbook
            If CBool2(Common.WebRequest.Data("ShowProducts")) Then
                Dim pXAxisOutput = TransInvoice.GetCrossTabAxis(pXAxis)
                Dim pYAxisOutput = TransInvoice.GetCrossTabAxis(pYAxis)
                pWB = TransInvoice.ProductSalesReportByGroup(vDT:=pDT, vXAxisName:=pXAxisOutput.ColumnText, vYAxisName:=pYAxisOutput.ColumnText)
            Else
                pWB = TableToExcel(pDT, "", "", True,
                vPreserveDataTypes:=True)
            End If
            pWB.Save(pFilename)
            Return JsonDictionary.Serialize("Key", pKey)
        Else
            Return JsonDictionary.Serialize(pDT)
        End If
    End Function

    Public Function DownloadCrossTabReport() As ActionResult
        Dim pKey = Common.WebRequest.QueryString("Key")
        If String.IsNullOrWhiteSpace(pKey) Then
            Return Content(JsonDictionary.Serialize("Error", "Key not found. ref: vbTD8XBG5NFY!"))
        End If
        Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp\PrintablePdf\CrossTabReport_" & pKey & ".xls")
        Return File(pFilename, "application/xls", "CrossTabReport_" & pKey & ".xls")

    End Function

    <HttpPost>
    Public Function LibraryFranchiseGroup() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pGroupType = CStr2(Common.WebRequest.Data("pGroupType"))

        With CreateFinder()
            .Distinct = True
            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
                .Add("GroupName", SqlDbType.VarChar)
            End With
            .SqlFrom = " FROM LibraryFranchiseGroup" & vbCrLf

            If Not pGroupType = vbNullString Then
                .SqlWhere = " WHERE GroupName like " & ReadyString(pGroupType) & vbCrLf
            End If

            If CBool2(Common.WebRequest.Data("FilterByUserGroup")) Then
                If Not pGroupType = vbNullString Then
                    .SqlWhere &= " AND " & vbCrLf
                Else
                    .SqlWhere = " WHERE "
                End If

                .SqlWhere &= " ID IN ( " & vbCrLf &
                            "   SELECT " & vbCrLf &
                            "   FranchiseGroupID " & vbCrLf &
                            "   FROM LibraryFranchiseGroupHeads " & vbCrLf &
                            "   WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                            ") " & vbCrLf
            End If

            .SqlOrder = " ORDER BY GroupName"
        End With

        Dim pSql = mFinder.CreateSql()
        Dim pDT = mFinder.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    <HttpPost>
    Public Function LibraryGroupsOfRegion() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "LibraryFranchiseGroup.ID"
                End With
                .Add("GroupName", SqlDbType.VarChar)
            End With
            .SqlFrom = " FROM LibraryFranchiseGroup " & vbCrLf & vbCrLf &
                        " INNER JOIN LibraryRegionalGroupSettingsDetails ON LibraryRegionalGroupSettingsDetails.GroupID = LibraryFranchiseGroup.ID " & vbCrLf &
                        " INNER JOIN LibraryRegionalGroupSettings ON LibraryRegionalGroupSettings.ID = LibraryRegionalGroupSettingsDetails.RegionalGroupID "

            .SqlWhere = " WHERE LibraryRegionalGroupSettings.ID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf

            .SqlOrder = " ORDER BY GroupName"
        End With

        Dim pSql = mFinder.CreateSql()
        Dim pDT = mFinder.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)

    End Function


    <HttpPost>
    Public Function LibraryRegion() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "LibraryRegionalGroupSettings.ID"
                End With
                .Add("RegionName", SqlDbType.VarChar)
            End With
            .SqlFrom = " FROM LibraryRegionalGroupSettings " & vbCrLf & vbCrLf &
                        " INNER JOIN LibraryRegionalGroupSettingsDetails ON LibraryRegionalGroupSettingsDetails.RegionalGroupID = LibraryRegionalGroupSettings.ID " & vbCrLf &
                        " INNER JOIN LibraryFranchiseGroup ON LibraryRegionalGroupSettingsDetails.GroupID = LibraryFranchiseGroup.ID " & vbCrLf

            .SqlWhere = " WHERE IsActive = 1 " & vbCrLf

            .SqlWhere &= " AND LibraryFranchiseGroup.ID IN ( " & vbCrLf &
                           "   SELECT " & vbCrLf &
                           "   franchisee.FranchiseGroupID " & vbCrLf &
                           "   FROM LibraryOrganizationEmployeeFOWST as empfowst " & vbCrLf &
                           "   INNER JOIN MobizLibraryOrganizationFranchisee as franchisee ON franchisee.FranchiseeOrgFowstID = empfowst.FowstID " & vbCrLf &
                           "   WHERE franchisee.FranchiseGroupID  IS NOT NULL AND empfowst.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                           "   AND franchisee.FranchisorOrgID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                           ") " & vbCrLf


            .SqlOrder = " ORDER BY RegionName"
        End With

        Dim pSql = mFinder.CreateSql()
        Dim pDT = mFinder.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    <HttpPost()>
    Public Function RoutingSheetNumbers() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDateTextFrom = Common.WebRequest.Data("DeliveryEncodingDateFrom")
        Dim pDateTextTo = Common.WebRequest.Data("DeliveryEncodingDateTo")
        Dim pDate = ParseDate(pDateTextFrom)
        Dim pSql As String
        If pDate IsNot Nothing Then
            pDate = pDate.Value.Date
            pDateTextFrom = Format(pDate, cGlobal.FormatDateTimeSql)
            pDateTextFrom = ReadyString(pDateTextFrom)
        End If

        pDate = ParseDate(pDateTextTo)
        If pDate IsNot Nothing Then
            pDate = pDate.Value.Date
            pDateTextTo = Format(pDate, cGlobal.FormatDateTimeSql)
            pDateTextTo = ReadyString(pDateTextTo)
        End If

        pSql = " SELECT TOP 50 * FROM (SELECT DISTINCT '' as ID, TransInvoice.RoutingSheetNumber AS [RoutingSheetNumber] " & vbCrLf &
                ", TransInvoice.DeliveryEncodingDate AS [DeliveryEncodingDate] " & vbCrLf &
                ", 1 as IsOldProgram " & vbCrLf &
                ", truck.Name as TruckName " & vbCrLf &
                ", threepl.CompanyName as Delivery3PlCompanyName " & vbCrLf &
                ", TransInvoice.DeliveryCustomerRepresentative " & vbCrLf &
                ", 0 IsDraft " & vbCrLf &
                " FROM TransInvoice " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST as truck ON truck.ID = TransInvoice.DeliveryTruckFowstID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization as threepl ON threepl.ID = TransInvoice.Delivery3PlCompanyID " & vbCrLf &
                " WHERE ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
                " --AND TransInvoice.DeliveryTruckFowstID = " & ReadyString(Common.WebRequest.Data("DeliveryTruckFowstID")) & vbCrLf &
                " --AND CONVERT(date, TransInvoice.DeliveryEncodingDate) BETWEEN " & pDateTextFrom & " AND " & pDateTextTo & vbCrLf &
                " AND TransInvoice.RoutingSheetNumber IS NOT NULL" & vbCrLf &
                " UNION ALL " & vbCrLf &
                " SELECT DISTINCT CONVERT(varchar(max), TransDeliveryLogger.ID) as ID, RoutingSheetNumber, DeliveryEncodingDate " & vbCrLf &
                ", 0 as IsOldProgram " & vbCrLf &
                ", truck.Name as TruckName " & vbCrLf &
                ", threepl.CompanyName as Delivery3PlCompanyName " & vbCrLf &
                ", TransDeliveryLogger.DeliveryCustomerRepresentative " & vbCrLf &
                ", TransDeliveryLogger.IsDraft " & vbCrLf &
                " FROM TransDeliveryLogger " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST as truck ON truck.ID = TransDeliveryLogger.DeliveryTruckFowstID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganization as threepl ON threepl.ID = TransDeliveryLogger.Delivery3PlCompanyID " & vbCrLf &
                " --WHERE CONVERT(date, DeliveryEncodingDate) BETWEEN " & pDateTextFrom & " AND " & pDateTextTo & vbCrLf &
                " --AND DeliveryTruckFowstID = " & ReadyString(Common.WebRequest.Data("DeliveryTruckFowstID")) & vbCrLf &
                " ) as T " & vbCrLf

        If Trim(Common.WebRequest.Data("keywords")) <> "" Or String.IsNullOrWhiteSpace(Common.WebRequest.Data("Status")) = False Then
            pSql += " WHERE " & vbCrLf
        End If

        If Trim(Common.WebRequest.Data("keywords")) <> "" Then
            pSql += " ISNULL(RoutingSheetNumber, '') + ' ' + ISNULL(TruckName, '') " & vbCrLf &
                    "  + ' ' + ISNULL(Delivery3PlCompanyName, '') + ' ' + ISNULL(DeliveryCustomerRepresentative, '') " & vbCrLf &
                    " LIKE " & ReadyStringWild(Trim(Common.WebRequest.Data("keywords")))
        End If

        If String.IsNullOrWhiteSpace(Common.WebRequest.Data("Status")) = False Then
            If Trim(Common.WebRequest.Data("keywords")) <> "" Then
                pSql += " AND "
            End If
            pSql += " IsDraft = " & CDec2(Common.WebRequest.Data("Status"))
        End If

        pSql += "ORDER BY DeliveryEncodingDate DESC"

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))

    End Function

    <HttpPost()>
    Public Function TransPriceUpdateRequest() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductGroupNo"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    '.Expression = " ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ','') + MobizLibraryPIMProduct.ProductName"
                    .Expression = "MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("ProductNameOnly", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductName"
                End With

                With .Add("Brand", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryPIMProduct.Brand, '')"
                End With
                With .Add("DateRequested", SqlDbType.DateTime)
                    .Searchable = False
                    .Expression = "TransPriceUpdateRequest.DateRequested"
                End With
                With .Add("RequestedByName", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = ComposePeopleName()
                End With
                With .Add("Remarks", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "TransPriceUpdateRequest.Remarks"
                End With
                With .Add("RetailPrice", SqlDbType.Money)
                    .Searchable = False
                    .Expression = "tblRetailPrice.Price"
                End With
                With .Add("RetailPriceLastUpdated", SqlDbType.DateTime)
                    .Searchable = False
                    .Expression = "tblRetailPrice.LastUpdated"
                End With
                With .Add("WholesalePrice", SqlDbType.Money)
                    .Searchable = False
                    .Expression = "tblWholesalePrice.Price"
                End With
                With .Add("WholesalePriceLastUpdated", SqlDbType.DateTime)
                    .Searchable = False
                    .Expression = "tblWholesalePrice.LastUpdated"
                End With
            End With

            .SqlFrom = " FROM TransPriceUpdateRequest" & vbCrLf &
                " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransPriceUpdateRequest.ProductID" & vbCrLf &
                " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransPriceUpdateRequest.RequestedByID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
            .SqlFrom &= "   OUTER APPLY (" & vbCrLf &
                    "     SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                    "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                    "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                    "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = TransPriceUpdateRequest.ProductID" & vbCrLf &
                    "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0005-0000-0000-000000000000'" & vbCrLf &
                    "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                    "   ) tblRetailPrice" & vbCrLf

            .SqlFrom &= "   OUTER APPLY (" & vbCrLf &
                "     SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
                "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = TransPriceUpdateRequest.ProductID" & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0004-0000-0000-000000000000'" & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                "   ) tblWholesalePrice" & vbCrLf

            .SqlOrder = " ORDER BY TransPriceUpdateRequest.DateRequested DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function PimSalesCaching() As String

        Dim pSql As String
        Dim pDT As DataTable

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            .MaxRecords = 1000
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Expression = " ISNULL(MobizLibraryPIMProduct.ProductSearchString,'') "
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = " MobizLibraryPIMProductVariableFeatures001.C0020"
                End With
                With .Add("Interval", SqlDbType.Int)
                    .Expression = " LibraryPimSalesCaching.Interval"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct" & vbCrLf &
                " JOIN LibraryPimSalesCaching ON LibraryPimSalesCaching.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf
            .SqlWhere = " WHERE MobizLibraryPIMProduct.IsActive = 1 " & vbCrLf
            .SqlOrder = " ORDER BY MobizLibraryPIMProduct.ProductName"
            pDT = .Requery()
        End With

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    <HttpPost()>
    Public Function TransESales() As String

        Dim pDT As DataTable

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pYear = WebRequest.Instance.Data("ZReadYear")
        Dim pMachineNumber = WebRequest.Instance.Data("MachineNumber")

        With CreateFinder()
            .Distinct = True
            .MaxRecords = 1000
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransESalesYearly.ID"
                End With
                With .Add("ZReadYear", SqlDbType.Int)
                    .Expression = "TransESalesYearly.ZReadYear"
                End With
                With .Add("MachineNumber", SqlDbType.VarChar)
                    .Expression = "TransESalesYearly.MachineNumber"
                End With
                With .Add("VatRate", SqlDbType.Money)
                    .Expression = "TransESalesYearly.VatRate"
                End With
                With .Add("OutputTaxRate", SqlDbType.Money)
                    .Expression = "TransESalesYearly.OutputTaxRate"
                End With
                With .Add("PreviousGrossSales", SqlDbType.Money)
                    .Expression = "TransESalesYearly.PreviousGrossSales"
                End With
                With .Add("PreviousVatSales", SqlDbType.Money)
                    .Expression = "TransESalesYearly.PreviousVatSales"
                End With
                With .Add("TotalGrossSales", SqlDbType.Money)
                    .Expression = "TransESalesYearly.TotalGrossSales"
                End With
                With .Add("TotalVatSales", SqlDbType.Money)
                    .Expression = "TransESalesYearly.TotalVatSales"
                End With
                For pMonth As Integer = 1 To 12
                    .Add("InvoiceNumber" & Format(pMonth, "00"), SqlDbType.Int)
                    .Add("GrossSales" & Format(pMonth, "00"), SqlDbType.Money)
                    .Add("AccumulatedGrossSales" & Format(pMonth, "00"), SqlDbType.Money)
                    .Add("VatSales" & Format(pMonth, "00"), SqlDbType.Money)
                    .Add("AccumulatedVatSales" & Format(pMonth, "00"), SqlDbType.Money)
                    .Add("OutputTax" & Format(pMonth, "00"), SqlDbType.Money)
                Next
            End With

            .SqlFrom = " FROM TransESalesYearly"
            .SqlOrder = " ORDER BY TransESalesYearly.ZReadYear, TransESalesYearly.MachineNumber"
            .SqlWhere = " WHERE 1=1" & vbCrLf
            If IsNumeric(pYear) Then
                .SqlWhere &= " AND ZReadYear = " & ReadyString(pYear) & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(pMachineNumber) = False Then
                .SqlWhere &= " AND MachineNumber = " & ReadyString(pMachineNumber) & vbCrLf
            End If
            pDT = .Requery()

            If pDT.Rows.Count <= 1 _
                AndAlso IsNumeric(pYear) _
                AndAlso String.IsNullOrWhiteSpace(pMachineNumber) = False Then
                Dim pDR = WebRequest.Instance.Business.TransESales.GetPastTotals(pYear, pMachineNumber)
                If pDR IsNot Nothing Then
                    pDT.Columns.Add("PreviousYear", GetType(Integer))
                    Dim pNewRow As DataRow
                    If pDT.Rows.Count = 0 Then
                        pNewRow = pDT.NewRow
                    Else
                        pNewRow = pDT.Rows(0)
                    End If
                    pNewRow("PreviousYear") = pDR("ZReadYear")
                    pNewRow("PreviousGrossSales") = pDR("GrossSales")
                    pNewRow("PreviousVatSales") = pDR("VatSales")
                    If pDT.Rows.Count = 0 Then
                        pDT.Rows.Add(pNewRow)
                    End If
                    Return JsonDictionary.Serialize("Results", pDT)
                End If
                End If
        End With

        Return SubmitFinder()

    End Function

#Region " common "
    Private Function CreateFinder() As Finder

        'todo: add common security

        mFinder = New Finder
        mFinder.MaxRecords = 50
        mFinder.SearchString = Common.WebRequest.Data("keywords")
        mFinder.ProData = Common.WebRequest.ProData

        Return mFinder

    End Function

    Private Function SubmitFinder(Optional ByVal vSql As String = "") As String

        Dim pDT As DataTable

        If CStr2(vSql) <> "" Then
            pDT = mFinder.ProData.Execute(vSql)
        Else
            pDT = mFinder.Requery
        End If

        Return JsonDictionary.Serialize("Results", pDT)

    End Function

    Public Shared Function ComposePeopleName(
        Optional ByVal vPeopleTableAlias As String = "MobizLibraryPeople",
        Optional ByVal vPreNominalSuffixAlias As String = "MobizLibraryPeoplePreNominalSuffix") As String

        ' note:
        '" JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransMemo.RequestedByPeopleID" & vbCrLf &
        '" LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

        Return "ISNULL(NULLIF(LTRIM(RTRIM(" & vPreNominalSuffixAlias & ".Code)) + ' ', ' '), '') + " &
            "ISNULL(NULLIF(LTRIM(RTRIM(" & vPeopleTableAlias & ".Surname)) + ', ',', '), '') + " &
            "ISNULL(NULLIF(LTRIM(RTRIM(" & vPeopleTableAlias & ".FirstName)) + ' ',' '), '') + " &
            "ISNULL(NULLIF(LTRIM(RTRIM(" & vPeopleTableAlias & ".MiddleName)) + ' ',' '), '') + " &
            "ISNULL(NULLIF(', ' + LTRIM(RTRIM(" & vPeopleTableAlias & ".PersonalSuffixName)),', '), '') + " &
            "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(" & vPeopleTableAlias & ".ID))),' '), '')"

    End Function

    Public Shared Sub ComposeCustomer(ByRef oSelect As String, ByRef oFrom As String,
        Optional vOrgAlias As String = "MobizLibraryOrganization",
        Optional vPeoplePreNominalSuffix As String = "MobizLibraryPeoplePreNominalSuffix")

        oSelect = " (CASE " & vOrgAlias & ".IsOrganization " &
            "     WHEN 1 THEN  " & vOrgAlias & ".CompanyName " &
            "     ELSE  " &
            "         ISNULL(NULLIF(LTRIM(RTRIM(" & vPeoplePreNominalSuffix & ".Code)) + ' ', ' '), '') + " &
            "         ISNULL(NULLIF(LTRIM(RTRIM(" & vOrgAlias & ".PersonSurname)) + ', ',', '), '') + " &
            "         ISNULL(NULLIF(LTRIM(RTRIM(" & vOrgAlias & ".PersonFirstName)) + ' ',' '), '') + " &
            "         ISNULL(NULLIF(LTRIM(RTRIM(" & vOrgAlias & ".PersonMiddleName)) + ' ',' '), '') + " &
            "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(" & vOrgAlias & ".PersonSuffixName)),', '), '') " &
            " END)"
        oFrom = " LEFT JOIN MobizLibraryPeoplePreNominalSuffix " & vPeoplePreNominalSuffix & " ON " & vOrgAlias & ".PersonTitleID = " & vPeoplePreNominalSuffix & ".ID" & vbCrLf
    End Sub
#End Region
End Class