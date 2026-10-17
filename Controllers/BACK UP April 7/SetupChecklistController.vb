Namespace WebProj

    Public Class SetupChecklistController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function SystemAdministration() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        <HttpPost()>
        Function DeviceSearch() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDT As DataTable
            Dim strReturn As String
            Dim pSql As String
            Dim DeviceSearchName = Common.WebRequest.Data("SearchName")

            pSql = "SELECT * FROM LibraryDevice WHERE PhoneName LIKE '%" & DeviceSearchName & "%' OR IMEI LIKE '%" & DeviceSearchName & "%'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("AuthorizedDevice", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Function FowstSearch() As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim strID As String
            Dim strReturn As String
            Dim FowstSearchName = Common.WebRequest.Data("SearchName")

            strID = WebSession.Instance.OrganizationID
            If (Common.WebRequest.Data("OrganizationID") <> Nothing) Then
                strID = Common.WebRequest.Data("OrganizationID")
            End If

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = 50

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "orgadd.ID"
                        .Searchable = False
                    End With
                    With .Add("FOWSTTypeID", SqlDbType.UniqueIdentifier)
                        .Expression = "orgadd.FOWSTTypeID"
                        .Searchable = False
                    End With
                    With .Add("Code", SqlDbType.VarChar)
                        .Expression = "orgadd.Code"
                    End With
                    With .Add("Name", SqlDbType.VarChar)
                        .Expression = "orgadd.Name"
                    End With
                    With .Add("Description", SqlDbType.VarChar)
                        .Expression = "orgadd.Description"
                    End With
                    With .Add("BusinessTradeName", SqlDbType.VarChar)
                        .Expression = "custtrade.BusinessTradeName"
                    End With
                    With .Add("CompleteFowstAddress", SqlDbType.VarChar)
                        .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgadd.StreetNumber)) + ' ', ' '), '') + " &
                                      "ISNULL(NULLIF(LTRIM(RTRIM(orgadd.Street)) + ', ',', '), '') + " &
                                      "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                      "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                    End With
                End With

                .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST orgadd " &
                           " LEFT JOIN dbo.MobizLibraryOrganizationTradeName custtrade ON (orgadd.BusinessTradeNameID = custtrade.ID) " &
                           " LEFT JOIN dbo.MobizLibraryCities cty ON (orgadd.FowstCityID = cty.ID) " &
                           " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgadd.FowstCountryID = ctry.ID) " &
                           " LEFT JOIN dbo.MobizLibraryFOWSTType rtype ON (orgadd.SubFOWSTTypeID = rtype.ID) " &
                           " LEFT JOIN dbo.MobizLibraryFOWSTWarehouseType wtype ON (orgadd.SubFOWSTTypeID = wtype.ID) " &
                           " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTPOSSettings posset ON (orgadd.ID = posset.FOWSTID) "

                .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE orgadd.OrganizationID IS NULL", " WHERE orgadd.OrganizationID = " + ReadyString(strID)) + " AND orgadd.Name LIKE '%" + FowstSearchName + "%'"

                .SqlOrder = " ORDER BY orgadd.Name ASC"

                pDT = .Requery
            End With

            strReturn = JsonDictionary.Serialize("FOWST", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Function getModuleList() As String
            Dim pDT As DataTable
            Dim strReturn As String
            Dim pSql As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT 'Organization' as ModuleType " &
                " ,NULL AS Cnt " &
                " ,LastUpdated " &
                " ,1  as SortOrder " &
                " FROM MobizLibraryOrganization " &
                " WHERE ID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) & " ORDER BY LastUpdated DESC) as LastUpdated " &
                ",2  as SortOrder " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Authorized Device' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDevice ORDER BY LastUpdated DESC) as LastUpdated " &
                ",3  as SortOrder " &
                " FROM LibraryDevice "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Receipt Settings' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryReceiptSettings ORDER BY LastUpdated DESC) as LastUpdated " &
                ",4  as SortOrder " &
                " FROM LibraryReceiptSettings "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Security Access' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibrarySecurityRoleUser ORDER BY LastUpdated DESC) as LastUpdated " &
                ",5  as SortOrder " &
                " FROM LibrarySecurityRoleUser "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Employee' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationEmployee ORDER BY LastUpdated DESC) as LastUpdated " &
                ",6  as SortOrder " &
                " FROM LibraryOrganizationEmployee "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Products' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationPIM ORDER BY LastUpdated DESC) as LastUpdated " &
                ",7  as SortOrder " &
                " FROM LibraryOrganizationPIM "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Customers/Clients' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationCustomer ORDER BY LastUpdated DESC) as LastUpdated " &
                ",8  as SortOrder " &
                " FROM LibraryOrganizationCustomer "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Supplier/Vendor' as ModuleType " &
                ",COUNT(*) AS Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryOrganizationSupplier ORDER BY LastUpdated DESC) as LastUpdated " &
                ",9  as SortOrder " &
                " FROM LibraryOrganizationSupplier "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Discount Settings' as ModultType " &
                ",NULL As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDiscountSettings ORDER BY LastUpdated Desc) as LastUpdated " &
                ",10  as SortOrder " &
                "FROM LibraryDiscountSettings "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Device Printer' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryDevicePrinter ORDER BY LastUpdated Desc) as LastUpdated " &
                ",11 as SortOrder " &
                "FROM LibraryDevicePrinter"
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Hot Button' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryPimMenu ORDER BY LastUpdated Desc) as LastUpdated " &
                ",12 as SortOrder " &
                "FROM LibraryPimMenu "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Group Setting' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryFranchiseGroup ORDER BY LastUpdated Desc) as LastUpdated " &
                ",13 as SortOrder " &
                "FROM LibraryFranchiseGroup "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'CRM' as ModultType " &
                ",COUNT(*) As Cnt " &
                ",(SELECT TOP 1 LastUpdated FROM LibraryCRMTeams ORDER BY LastUpdated Desc) as LastUpdated " &
                ",14 as SortOrder " &
                "FROM LibraryCRMTeams " &
                " ORDER BY SortOrder"


            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("PIMChecklistData", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Function getModuleRequiredDataList() As String
            Dim pDT As DataTable
            Dim strReturn As String
            Dim pSql As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT ModuleType ,RequiredData ,IsOk FROM ("
            pSql = pSql & "SELECT " &
                "'Organization' as ModuleType " &
                ",'Trade Name' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(OrganizationID) > 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationTradeName " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Name' as RequiredData " &
                ",(CASE " &
                "   WHEN RTRIM(LTRIM([Name])) <> '' THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Trade Name' as RequiredData " &
                ",(CASE " &
                "   WHEN BusinessTradeNameID IS NOT NULL THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Address' as RequiredData " &
                ",(CASE " &
                "   WHEN RTRIM(LTRIM(Street)) <> '' THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID)
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Telephone No.' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(fowsttel.FOWSTID) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST fowst" &
                " LEFT JOIN MobizLibraryOrganizationFOWSTTel fowsttel ON (fowsttel.FOWSTID = fowst.ID) " &
                " WHERE OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) &
                " GROUP BY fowst.ID "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'FOWST' as ModuleType " &
                ",'FOWST Printer' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(fowstptr.FOWSTID) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST fowst " &
                " LEFT JOIN LibraryDevicePrinterFowst fowstptr ON (fowstptr.FOWSTID = fowst.ID) " &
                " WHERE fowst.OrganizationID = " & ReadyString(WebRequest.Instance().OrgID) &
                " GROUP BY fowst.ID "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Authorized Device' as ModuleType " &
                ",'Device' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(*) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM LibraryDevice dev"
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Authorized Device' as ModuleType " &
                ",'Device FOWST' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(devfowst.ID) <> 0 THEN 1 " &
                "	ELSE 0 " &
                " END) as IsOk " &
                " FROM LibraryDevice dev " &
                " LEFT JOIN LibraryDeviceFowst devfowst ON (dev.ID = devfowst.DeviceID) " &
                " GROUP BY dev.ID "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Receipt Settings' as ModuleType " &
                ",'Receipt Settings' as RequiredData " &
                ",(CASE " &
                "   WHEN COUNT(rset.ID) <> 0 THEN 1 " &
                "   ELSE 0 " &
                " END) as IsOk " &
                " FROM LibraryReceiptSettings rset "
            pSql = pSql & " UNION "
            pSql = pSql & "SELECT " &
                "'Receipt Settings' as ModuleType " &
                ",'Receipt Settings FOWST' as RequiredData " &
                ",(CASE " &
             "   WHEN COUNT(fowst.ID) <> 0 THEN 1 " &
             "   ELSE 0 " &
                " END) as IsOk " &
                " FROM MobizLibraryOrganizationFOWST fowst " &
                " LEFT JOIN LibraryReceiptSettingsFowst rsetfowst ON (fowst.ID = rsetfowst.FowstID) " &
                " LEFT JOIN LibraryReceiptSettings rset ON (rset.ID = rsetfowst.ReceiptSettingsID) " &
                " GROUP BY fowst.ID"
            pSql = pSql & ") test "
            pSql = pSql & " WHERE IsOk = 0 "
            pSql = pSql & " ORDER BY ModuleType "

            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("PIMChecklistData", pDT)

            Return strReturn

        End Function


    End Class

End Namespace