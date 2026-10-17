Namespace WebProj
    Public Class DirectoryController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function SearchSupplier()
            Try
                Dim pSql As String
                Dim clsorgshort As New OrganizationShort
                Dim filter As String = Common.WebRequest.Data("filter")

                pSql = " SELECT DISTINCT OrganizationID, org.CompanyName as SupplierName FROM ( " & vbCrLf

                If filter = "all" Or filter = "categories" Then
                    pSql &= "   SELECT libcat.CategoryName as Description, transcat.OrganizationID " & vbCrLf &
                        "   FROM Mobiz.dbo.TransOrganizationCategoriesOfProductOrServicesOffer as transcat " & vbCrLf &
                        "   INNER JOIN Mobiz.dbo.LibraryCategoriesOfProductOrServices as libcat ON libcat.ID = transcat.CategoryID " & vbCrLf
                End If

                If filter = "all" Then
                    pSql &= " UNION ALL "
                End If

                If filter = "all" Or filter = "brands" Then
                    pSql &= "   SELECT libbrand.BrandName as Description, transbrand.OrganizationID " & vbCrLf &
                        "   FROM Mobiz.dbo.TransOrganizationBrandsOffer as transbrand " & vbCrLf &
                        "   INNER JOIN Mobiz.dbo.LibraryBrandNames as libbrand ON libbrand.ID = transbrand.BrandNameID " & vbCrLf
                End If

                pSql &= ") as T INNER JOIN MobizLibraryOrganization as org ON org.ID = t.OrganizationID WHERE Description LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords")) & vbCrLf

                Dim pdtSupplier As Object = jss.Deserialize(Of Object)(clsorgshort.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each supplier As Object In pdtSupplier
                    pSql = " (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + libcat.CategoryName " & vbCrLf &
                            "       FROM LibraryCategoriesOfProductOrServices as libcat " & vbCrLf &
                            "           INNER JOIN TransOrganizationCategoriesOfProductOrServicesOffer as transcat " & vbCrLf &
                            "               ON libcat.ID = transcat.CategoryID " & vbCrLf &
                            "           WHERE transcat.OrganizationID = " & ReadyString(supplier("OrganizationID")) & vbCrLf &
                            "       FOR XML PATH('') " & vbCrLf &
                            "   ), 1, 1,'') as Names FROM LibraryCategoriesOfProductOrServices) " & vbCrLf

                    supplier.Add("Categories", jss.Deserialize(Of Object)(clsorgshort.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                    pSql = " (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + libbrand.BrandName " & vbCrLf &
                            "       FROM LibraryBrandNames as libbrand " & vbCrLf &
                            "           INNER JOIN TransOrganizationBrandsOffer as transbrand " & vbCrLf &
                            "               ON libbrand.ID = transbrand.BrandNameID " & vbCrLf &
                            "           WHERE transbrand.OrganizationID = " & ReadyString(supplier("OrganizationID")) & vbCrLf &
                            "       FOR XML PATH('') " & vbCrLf &
                            "   ), 1, 1,'') as Names FROM LibraryBrandNames) " & vbCrLf

                    supplier.Add("Brands", jss.Deserialize(Of Object)(clsorgshort.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next supplier

                Return Json(New With {Key .data = pdtSupplier}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                Return Json(New With {Key .data = ex.Message}, JsonRequestBehavior.AllowGet)
            End Try
        End Function

        Function GetSupplierInfo()
            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort
            Dim pSql As String
            Dim pOrgSelect As String
            Dim pOrgFrom As String
            Dim OrgID As String = Common.WebRequest.Data("ID")
            Dim pOut As New JsonDictionary

            clsorgshort.OrgID = OrgID

            'strReturn = clsorgshort.GetProspectDetails()
            BrowserController.ComposeCustomer(pOrgSelect, pOrgFrom, "org")

            pSql = "SELECT org.ID, " & pOrgSelect & " AS CompanyName" & vbCrLf &
            ",OfficeAddressBuildingNo, TinNumber, OfficeAddressCityID, OfficeAddressCountryID, OfficeAddressStreet" & vbCrLf &
            " , ISNULL(OfficeAddressBuildingNo + ', ', '') + ' ' + ISNULL(OfficeAddressStreet + ', ', '') + ' ' + city.Name + ' ' + country.Name as Address " & vbCrLf &
            " , Background, Mission, Vision, FoundingDate, Region, Island, ind.Class as IndustryName, ind.ID as IndustryID " & vbCrLf &
            " , ISNULL(CONVERT(varchar, NoOfEmployees), '') as NoOfEmployees, ISNULL(CONVERT(varchar, RevenuePerYear), '') as RevenuePerYear, ISNULL(CONVERT(varchar, ScopeOfOperations), '') as ScopeOfOperations " & vbCrLf &
            " , ISNULL(CONVERT(varchar, NoOfFactory), '') as NoOfFactory, ISNULL(CONVERT(varchar, NoOfOffice), '') as NoOfOffice, ISNULL(CONVERT(varchar, NoOfWarehouse), '') as NoOfWarehouse, ISNULL(CONVERT(varchar, NoOfStore), '') as NoOfStore, ISNULL(CONVERT(varchar, NoOfTruck), '') as NoOfTruck " & vbCrLf &
            " ,org.BusinessIndustryText " & vbCrLf &
            " ,org.TinNumber as TIN " & vbCrLf &
            " FROM MobizLibraryOrganization as org LEFT JOIN MobizLibraryIndustryClass as ind ON org.IndustryClassID = ind.ID " & vbCrLf &
            pOrgFrom & vbCrLf &
            " LEFT JOIN MobizLibraryCities as city ON City.ID = OfficeAddressCityID " & vbCrLf &
            " LEFT JOIN MobizLibraryCountries as country ON country.ID = OfficeAddressCountryID " & vbCrLf &
            " WHERE org.ID = " & ReadyString(OrgID)

            pOut.Add("CompanyDetails", jss.Deserialize(Of Object)(clsorgshort.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = " SELECT libcat.CategoryName as Name, transcat.BusinessActivities " & vbCrLf &
                    "   FROM MobizLibraryCategoriesOfProductOrServices as libcat " & vbCrLf &
                    "   INNER JOIN MobizTransOrganizationCategoriesOfProductOrServicesOffer as transcat " & vbCrLf &
                    "   ON libcat.ID = transcat.CategoryID " & vbCrLf &
                    "   WHERE transcat.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                    "   ORDER BY libcat.CategoryName ASC "

            pOut.Add("Categories", jss.Deserialize(Of Object)(clsorgshort.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = " SELECT libbrand.BrandName as Name, transbrand.BusinessActivities " & vbCrLf &
                    "   FROM MobizLibraryBrandNames as libbrand " & vbCrLf &
                    "   INNER JOIN MobizTransOrganizationBrandsOffer as transbrand " & vbCrLf &
                    "   ON libbrand.ID = transbrand.BrandNameID " & vbCrLf &
                    "   WHERE transbrand.OrganizationID = " & ReadyString(OrgID) & vbCrLf &
                    "   ORDER BY libbrand.BrandName ASC "

            pOut.Add("Brands", jss.Deserialize(Of Object)(clsorgshort.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            Dim objTelephone As Object = jss.Deserialize(Of Object)(clsorgshort.GetJson(clsorgshort.GetProspectOrgTel(OrgID)))
            Dim objMobile As Object = jss.Deserialize(Of Object)(clsorgshort.GetJson(clsorgshort.GetProspectOrgMobile(OrgID)))
            Dim objEmail As Object = jss.Deserialize(Of Object)(clsorgshort.GetJson(clsorgshort.GetProspectOrgEmail(OrgID)))
            Dim objContactPerson As Object = jss.Deserialize(Of Object)(clsorgshort.GetJson(clsorgshort.GetProspectOrgContactPerson(OrgID)))
            Dim objFiles As Object = jss.DeserializeObject(New OrganizationController().GetOrganizationFiles())("d")("Results")("Rows")

            pOut.Add("CompanyTelephone", objTelephone)
            pOut.Add("CompanyMobile", objMobile)
            pOut.Add("CompanyEmail", objEmail)
            pOut.Add("CompanyPictures", objFiles)

            Return pOut.Serialize()
        End Function
    End Class
End Namespace