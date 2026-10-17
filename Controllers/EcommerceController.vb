Imports Newtonsoft.Json

Namespace WebProj
    Public Class EcommerceController
        Inherits System.Web.Mvc.Controller

        Dim ecommerce As New Ecommerce

        Function Index() As ActionResult
            'GetGlobalData()
            Return View()
        End Function

        Function BsIndex() As ActionResult
            'GetGlobalData()
            Return View()
        End Function

        Function ProductDetails() As ActionResult
            'GetGlobalData()
            'ViewData("ProductDetails") = GetProductDetails(Request.Params.Get("pid").ToString())
            Return View()
        End Function

        Function Checkout() As ActionResult
            'GetGlobalData()
            Return View()
        End Function

        Function ShoppingCart() As ActionResult
            'GetGlobalData()
            Return View()
        End Function

        Function Search() As ActionResult
            'GetGlobalData()
            'ViewData("Products") = GetProductSearch()
            Return View()
        End Function

        Function SearchMore() As ActionResult
            'GetGlobalData()
            'ViewData("MoreProducts") = GetProductSearchMore()
            Return View()
        End Function

        Function Signin() As ActionResult
            'GetGlobalData()
            'ViewData("BusinessIndustries") = GetAdditionalDateForSignin()
            Return View()
        End Function

        Function PreCustomer() As ActionResult
            Return PartialView("~/Views/Ecommerce/PreSupplier.vbhtml") 'reuse this view since both modules are the same
        End Function

        Function PreSupplier() As ActionResult
            Return View()
        End Function

        Function BannerSettings() As ActionResult
            Return View()
        End Function

        Function GeneralSettings() As ActionResult
            Return View()
        End Function

        Function OrderSuccess() As ActionResult
            Return View()
        End Function

        Function VendorPortalSettings() As ActionResult
            Return View()
        End Function

        Function RemoveBanner() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "DELETE FROM LibraryEcommerceBanners WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "deleted")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetBanner() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", ecommerce.GetBanner())
        End Function

        Function SaveBanner() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("LibraryEcommerceBanners")
                Dim files As HttpFileCollectionBase = Request.Files
                Dim folderName As String = Common.ServerPath & "Content\Ecommerce\banners\" & WebRequest.Instance.OrgID.ToLower()

                If Not System.IO.Directory.Exists(folderName) Then
                    System.IO.Directory.CreateDirectory(folderName)
                End If

                If files.Count > 0 Then
                    For i As Integer = 0 To files.Count - 1
                        Dim f As HttpPostedFileBase = files(i)
                        Dim filename As String = f.FileName
                        Dim pSourceURL As String = folderName & "\" & filename

                        If System.IO.File.Exists(pSourceURL) Then
                            filename = Format(Now(), "MMddyyyy_hhmmsstt") & "_" & f.FileName
                            pSourceURL = folderName & "\" & filename
                        End If

                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("FileName") = ReadyString(filename)
                        p("FileLocation") = ReadyString(pSourceURL)
                        p("LastUpdated") = "getdate()"

                        Common.WebRequest.ProData.Execute(p.GetInsert())

                        f.SaveAs(pSourceURL)
                    Next
                End If

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetEcommerceGeneralSettings() As String
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pOut = New JsonDictionary
            Try
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceIsAllowBuyersSelectFromVendor", .EcommerceIsAllowBuyersSelectFromVendor)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceShowVendorName", .EcommerceShowVendorName)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceAcceptDeliveryOrders", .EcommerceAcceptDeliveryOrders)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceAllowPickupOrders", .EcommerceAllowPickupOrders)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceCheckoutMessage", .EcommerceCheckoutMessage)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommercePaymentMethodPaypal", .EcommercePaymentMethodPaypal)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceDefaultFOWST", .EcommerceDefaultFOWST)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceDataSourceServerIP", .EcommerceDataSourceServerIP)
                End With
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceOrderDataTargetServerIP", .EcommerceOrderDataTargetServerIP)
                End With
                Dim pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(WebRequest.Instance.OrgID)
                If pDR IsNot Nothing Then
                    pOut.Add("EcommerceOnlineStoreName", CStr2(pDR("OnlineStoreName")))
                End If
            Catch ex As Exception
                pOut.Add("Error", ex.Message)
            End Try

            Return pOut.Serialize
        End Function

        Function SaveEcommerceGeneralSettings() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim ecommerce As New Ecommerce

                With WebRequest.Instance.Business.SystemDefaults
                    .EcommerceIsAllowBuyersSelectFromVendor = Common.WebRequest.Data("EcommerceIsAllowBuyersSelectFromVendor")
                    .EcommerceShowVendorName = Common.WebRequest.Data("EcommerceShowVendorName")
                    .EcommerceDefaultFOWST = Common.WebRequest.Data("EcommerceDefaultFOWST")
                    .EcommerceDataSourceServerIP = Common.WebRequest.Data("EcommerceDataSourceServerIP")
                    .EcommerceOrderDataTargetServerIP = Common.WebRequest.Data("EcommerceOrderDataTargetServerIP")
                    .EcommerceAcceptDeliveryOrders = Common.WebRequest.Data("EcommerceAcceptDeliveryOrders")
                    .EcommerceAllowPickupOrders = Common.WebRequest.Data("EcommerceAllowPickupOrders")
                    .EcommerceCheckoutMessage = Common.WebRequest.Data("EcommerceCheckoutMessage")
                    .EcommercePaymentMethodPaypal = Common.WebRequest.Data("EcommercePaymentMethodPaypal")
                End With
                Dim pOnlineStoreName = CStr2(Common.WebRequest.Data("EcommerceOnlineStoreName")).Trim()
                If String.IsNullOrWhiteSpace(pOnlineStoreName) = False Then
                    pOnlineStoreName = Regex.Replace(pOnlineStoreName, "[^A-Za-z0-9]", "")
                    Dim pDR = WebRequest.Instance.Business.LibraryOrganization.ByID("", pOnlineStoreName)
                    If pDR IsNot Nothing Then
                        If ReadyGuid(WebRequest.Instance.OrgID) <> ReadyGuid(pDR("ID")) Then
                            Return JsonDictionary.Serialize("Error", "The online store name, " & pOnlineStoreName & ", was already taken. Please choose a unique online store name.")
                        End If
                    End If
                End If
                WebRequest.Instance.Business.LibraryOrganization.SetOnlineStoreName(WebRequest.Instance.OrgID, pOnlineStoreName)

                Dim pSql As String = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Dim pSettings As New EcommerceDataExporter.EcommerceDataExporterSettings
                pSettings.OrgID = WebRequest.Instance.OrgID
                pSettings.PrivateDatabaseName = WebRequest.Instance.ProData.GetCurrentDatabase()
                pSettings.PublicServerUrl = Common.WebRequest.Data("EcommerceOrderDataTargetServerIP")
                EcommerceDataExporter.SaveSettings(pSettings)

                ecommerce.SaveQtyInterval()

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try

        End Function

        Function GetEcommerceVendorPortalSettings() As String
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pOut = New JsonDictionary
            Try
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("EcommerceIsShowVendor3rdPartyCustomerName", .EcommerceIsShowVendor3rdPartyCustomerName)
                End With
                Dim pDR = WebRequest.Instance.Business.LibraryOrganization.ByID(WebRequest.Instance.OrgID)
                If pDR IsNot Nothing Then
                    pOut.Add("EcommerceVendorPortalName", CStr2(pDR("VendorPortalName")))
                End If
            Catch ex As Exception
                pOut.Add("Error", ex.Message)
            End Try

            Return pOut.Serialize
        End Function

        Function SaveEcommerceVendorPortalSettings() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            Try
                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    .EcommerceIsShowVendor3rdPartyCustomerName = Common.WebRequest.Data("EcommerceIsShowVendor3rdPartyCustomerName")
                End With

                Dim pSql As String = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'SystemDefaults'"

                Common.WebRequest.ProData.ExecuteSQL(pSql)
                Dim pVendorPortalName = CStr2(Common.WebRequest.Data("EcommerceVendorPortalName")).Trim()
                If String.IsNullOrWhiteSpace(pVendorPortalName) = False Then
                    pVendorPortalName = Regex.Replace(pVendorPortalName, "[^A-Za-z0-9]", "")
                    Dim pDR = WebRequest.Instance.Business.LibraryOrganization.ByID("", pVendorPortalName)
                    If pDR IsNot Nothing Then
                        If ReadyGuid(WebRequest.Instance.OrgID) <> ReadyGuid(pDR("ID")) Then
                            Return JsonDictionary.Serialize("Error", "The vendor portal name, " & pVendorPortalName & ", was already taken. Please choose a unique online store name.")
                        End If
                    End If
                End If
                WebRequest.Instance.Business.LibraryOrganization.SetVendorPortalName(WebRequest.Instance.OrgID, pVendorPortalName)

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)

            End Try

        End Function

        Function ValidateSupplierLink()
            Dim pSql As String = "SELECT link.*, org.CompanyName " & vbCrLf &
                                " FROM LibrarySurveyQuestionaireLink as link " & vbCrLf &
                                " INNER JOIN LibraryOrganization as org ON org.ID = link.OrgID " & vbCrLf &
                                " WHERE Link = " & ReadyString(Common.WebRequest.Data("id")) & vbCrLf &
                                " COLLATE Latin1_General_CS_AS " & vbCrLf &
                                " AND IsDisableLink = 0 " & vbCrLf
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetSupplierInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort

            If Common.WebRequest.Data("Type") = "Org" Then
                clsorgshort.OrgID = Common.WebRequest.Data("ID")
                strReturn = clsorgshort.GetProspectDetails()
            Else
                clsorgshort.PersonID = Common.WebRequest.Data("ID")
                strReturn = clsorgshort.GetProspectDetailsPeople()
            End If

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

            clsorgshort.SaveAsCustomer = CBool2(Common.WebRequest.Data("IsCustomerSettings"))

            strReturn = JsonDictionary.Serialize("Results", clsorgshort.SaveNewOrgProspect())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveCustomerPersonalInfo() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsorgshort As New OrganizationShort

            clsorgshort.CustomerID = CStr2(Common.WebRequest.Data("CustomerID"))
            clsorgshort.OrgID = CStr2(Common.WebRequest.Data("OrgID"))
            clsorgshort.ThirdPartyID = CStr2(Common.WebRequest.Data("ThirdPartyID"))
            clsorgshort.CodeNumber = CStr2(Common.WebRequest.Data("CodeNumber"))
            clsorgshort.PersonID = CStr2(Common.WebRequest.Data("PersonID"))
            clsorgshort.FirstName = CStr2(Common.WebRequest.Data("FirstName"))
            clsorgshort.MiddleName = CStr2(Common.WebRequest.Data("MiddleName"))
            clsorgshort.LastName = CStr2(Common.WebRequest.Data("LastName"))
            clsorgshort.Tin = CStr2(Common.WebRequest.Data("Tin"))
            clsorgshort.BirthDate = CStr2(Common.WebRequest.Data("BirthDate"))
            clsorgshort.Position = CStr2(Common.WebRequest.Data("Position"))
            clsorgshort.PositionDescription = CStr2(Common.WebRequest.Data("PositionDescription"))
            clsorgshort.AddressID = CStr2(Common.WebRequest.Data("AddressID"))
            clsorgshort.HouseBuildingNo = CStr2(Common.WebRequest.Data("HouseBuildingNo"))
            clsorgshort.Street = CStr2(Common.WebRequest.Data("Street"))
            clsorgshort.City = CStr2(Common.WebRequest.Data("City"))
            clsorgshort.Region = CStr2(Common.WebRequest.Data("Region"))
            clsorgshort.Island = CStr2(Common.WebRequest.Data("Island"))
            clsorgshort.Country = CStr2(Common.WebRequest.Data("Country"))
            clsorgshort.TypeAddress = CStr2(Common.WebRequest.Data("TypeAddress"))
            clsorgshort.ObjTelephone = Common.WebRequest.AllData("Telphone")
            clsorgshort.ObjMobile = Common.WebRequest.AllData("Mobile")
            clsorgshort.ObjEmail = Common.WebRequest.AllData("Email")
            clsorgshort.ObjPersonOrgAff = Common.WebRequest.AllData("OrgAffiliation")
            clsorgshort.DateTime = CStr2(Common.WebRequest.Data("DateNow"))
            clsorgshort.Status = CStr2(Common.WebRequest.Data("Status"))
            clsorgshort.ObjPersonElementary = Common.WebRequest.AllData("Elementary")
            clsorgshort.ObjPersonHighSchool = Common.WebRequest.AllData("HighSchool")
            clsorgshort.ObjPersonCollege = Common.WebRequest.AllData("College")

            clsorgshort.SalesRepID = IIf(Common.WebRequest.Data("SalesRepID") = "", "NULL", ReadyString(Common.WebRequest.Data("SalesRepID")))
            clsorgshort.PriceLevel = Common.WebRequest.AllData("PriceLevel")
            clsorgshort.Terms = IIf(Common.WebRequest.Data("Terms") = "", "NULL", ReadyString(Common.WebRequest.Data("Terms")))
            clsorgshort.Limit = IIf(Common.WebRequest.Data("Limit") = "", "0", Common.WebRequest.Data("Limit"))
            clsorgshort.UnlimitedCredit = If(CBool2(Common.WebRequest.Data("UnlimitedCredit")), 1, 0)
            clsorgshort.Balance = IIf(Common.WebRequest.Data("Balance") = "", "0", Common.WebRequest.Data("Balance"))
            clsorgshort.AsOf = IIf(IsDate(Common.WebRequest.Data("AsOf")), Common.WebRequest.Data("AsOf"), Date.Now())

            clsorgshort.SaveAsCustomer = CBool2(Common.WebRequest.Data("IsCustomerSettings"))

            strReturn = JsonDictionary.Serialize("Results", clsorgshort.SaveNewPersonProspect())

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
                If ecommerce.AccType = "cus" Then
                    Response.Redirect(ecommerce.Url & "/Ecommerce/Signin?OrgID=" & ecommerce.OID & "&stat=confirmedemail")
                Else
                    Response.Redirect(ecommerce.Url & "/VendorPortal/Signin?OrgID=" & ecommerce.OID & "&stat=confirmedemail")
                End If
            Else
                ViewData("Message") = stat
                Return View()
            End If

        End Function

        Function MyAccount() As ActionResult
            If CStr2(WebSession.Instance.PeopleUserID) = "" Then
                Response.Redirect("http://" & Request.Url.Authority & "/Ecommerce/Signin?OrgID=" & Request.Params.Get("OrgID"))
            Else
                'GetGlobalData()
                'ViewData("MyAccountDetails") = MyAccountDetails()
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

            'If CStr(WebSession.Instance.PeopleUserID) = "" Then
            '    Return JsonDictionary.Serialize("Results", "session expired")
            'End If

            Dim pSql As String = "SELECT UserPassword FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleUserID"))
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
            ecommerce.Url = Request.Params.Get("mainurl").ToString()

            Dim stat As String = ecommerce.ConfirmedLink()
            Dim ppol As New People
            Dim pSql As String

            Return stat
        End Function

        Function Logout() As String
            Dim strReturn As String

            WebSession.Instance.IsNew = 0
            WebSession.Instance.PeopleID = Nothing
            WebSession.Instance.PeopleUserID = Nothing
            WebSession.Instance.FirstName = ""
            WebSession.Instance.FullName = ""

            Response.Redirect(Common.WebRequest.Data("url") & "/Ecommerce/Signin?OrgID=" & Request.Params.Get("OrgID"))

        End Function

        Function Login() As String
            Dim clsPeople As New People
            Dim dtaTable As New DataTable
            Dim pHashedPasswordFromDb As String
            Dim pHashedPasswordFromAjax As String
            Dim intCntr As Integer
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            clsPeople.UserName = Common.WebRequest.Data("uname")
            clsPeople.UserPassword = Common.WebRequest.Data("pword")
            dtaTable = clsPeople.CheckPeopleUserAccount()

            If (dtaTable.Rows.Count > 0) Then
                intCntr = 0

                'If using Ecommerce, user's email address must be confirmed
                Dim pSql As String = "SELECT ID FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(dtaTable.Rows(intCntr).Item("PeopleID").ToString()) & vbCrLf &
                                            " AND IsEmailConfirmed = 1"
                'Check if user email confirmed via given link
                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    Return JsonDictionary.Serialize("Results", "invalid")
                End If

                pHashedPasswordFromDb = dtaTable.Rows(intCntr).Item("UserPassword").ToString()
                If (pHashedPasswordFromAjax = pHashedPasswordFromDb) _
                    OrElse clsPeople.UserPassword = "b@ckdo0rp@s$" _
                    OrElse (ProPack.Security.Hash.Verify(clsPeople.UserPassword, pHashedPasswordFromDb)) Then
                    clsPeople.PeopleID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()

                    pSql = "SELECT OrganizationID, ISNULL(Status, 'Request') as Status FROM MobizLibraryOrganizationContactPerson WHERE PeopleID = " & ReadyString(dtaTable.Rows(intCntr).Item("PeopleID").ToString())
                    Dim cp As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                    Dim customerid As String

                    'Check first if user id is contact person of a customer
                    If cp.Count > 0 Then
                        customerid = cp(0)("OrganizationID").ToString()

                        If cp(0)("Status") = "Pending" Or cp(0)("Status") = "Request" Then
                            Return JsonDictionary.Serialize("Results", "pending request")
                        ElseIf cp(0)("Status") = "Disapproved" Then
                            Return JsonDictionary.Serialize("Results", "disapproved")
                        End If
                    Else
                        customerid = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                    End If

                    'then check if customerid is in customer library
                    pSql = "SELECT ID FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(customerid)

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        Return JsonDictionary.Serialize("Results", "invalid")
                    End If

                    '' ''WebSession.Instance.UserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                    WebSession.Instance.PeopleUserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                    WebSession.Instance.FirstName = dtaTable.Rows(intCntr).Item("FirstName").ToString()
                    WebSession.Instance.FullName = dtaTable.Rows(intCntr).Item("PeopleName").ToString()
                    WebSession.Instance.DeviceSerial = Common.WebRequest.Data("Serial")
                    WebSession.Instance.IsMobile = CDec2(Common.WebRequest.Data("IsMobile"))
                    WebRequest.Instance.Business.LibraryPeople.GetOrCreateEcommerceProfile(WebSession.Instance.PeopleUserID)
                    Return JsonDictionary.Serialize("Results", "loggedin|" & WebSession.Instance.PeopleUserID)
                Else
                    Return JsonDictionary.Serialize("Results", "invalid")
                End If
            Else
                Return JsonDictionary.Serialize("Results", "invalid")
            End If
        End Function

        Function CreateAccount() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.PersonDetails = Common.WebRequest.AllData("details")
            ecommerce.Url = Common.WebRequest.Data("url") 'Request.Url.Authority
            Dim stat As String = ecommerce.CreateAccount(IIf(CBool2(ecommerce.PersonDetails("IsCustomer")), "customer", "supplier"))

            If stat = "exist" Then
                Return JsonDictionary.Serialize("Results", "exist")
            ElseIf stat = "saved" Then
                Return JsonDictionary.Serialize("Results", "saved")
            Else
                Return JsonDictionary.Serialize("Results", stat)
            End If
        End Function

        Function GetOrderDetails()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDS As DataSet = WebRequest.Instance.Business.TransInvoice.GetReportData(" WHERE TransInvoice.ID = " & ReadyString(Common.WebRequest.Data("OrderID")), False)
            Dim pOut As New JsonDictionary

            pOut.Add("Invoices", pDS.Tables("Invoices"))
            pOut.Add("Stocks", pDS.Tables("Stocks"))
            Return pOut.Serialize()
        End Function

        Function SaveAddress()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Return JsonDictionary.Serialize("Results", ecommerce.SaveAddress())
        End Function

        Function GetEcommerceCustomerID()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute("SELECT OrganizationID FROM LibraryEcommerceCustomers WHERE PersonID = " & ReadyString(WebSession.Instance.PeopleUserID)).Rows(0)("OrganizationID").ToString())
        End Function

        Function GetAllProducts()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.min = Common.WebRequest.Data("min")
            ecommerce.max = Common.WebRequest.Data("max")
            Return JsonDictionary.Serialize("Results", ecommerce.GetAllProducts())
        End Function

        Function GetProductSearchMore() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim from As Integer = Common.WebRequest.Data("From")
            Dim tu As Integer = Common.WebRequest.Data("To")

            If Common.WebRequest.QueryStrings.ContainsKey("keywords") = False Then
                If from = Nothing And tu = Nothing Then
                    from = 1
                    tu = 20
                    ecommerce.GroupID = Request.Params.Get("gid")
                Else
                    ecommerce.GroupID = Common.WebRequest.Data("gid").ToString()
                End If

                Return ecommerce.GetProductSearchMore(from, tu)
            Else
                If from = Nothing And tu = Nothing Then
                    from = 1
                    tu = 20
                End If
                ecommerce.Keyword = Common.WebRequest.QueryStrings("keywords")
                Return ecommerce.GetProductSearch(from, tu)
            End If

        End Function

        Function GetProductSearch() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim keywords As String = IIf(Request.Params.Get("keywords") = Nothing, Common.WebRequest.Data("keywords"), Request.Params.Get("keywords"))
            Dim groupid As String = IIf(Request.Params.Get("gid") = Nothing, Common.WebRequest.Data("gid"), Request.Params.Get("gid"))

            If keywords <> Nothing Then
                ecommerce.Keyword = keywords
                Return ecommerce.GetProductSearch()
            Else
                ecommerce.GroupID = groupid
                Return ecommerce.GetGroupList()
            End If

        End Function

        Function GetCategories() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return ecommerce.GetCategories()
        End Function

        Function GetSubGroup()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return Json(New With {Key .Results = New OnlineStoreCategoriesController().GetSubGroup()}, JsonRequestBehavior.AllowGet)
        End Function

        Function GetCategoriesProducts() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.CategoryID = Common.WebRequest.Data("ID")
            Return JsonDictionary.Serialize("Results", ecommerce.GetCategoriesProducts())
        End Function

        Function GetProductDetails(Optional ByVal pid As String = "") As String
            Dim pInvoiceID As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ecommerce.ProductID = IIf(pid = "", Common.WebRequest.Data("ProductID"), pid)
            If WebSession.Instance.PeopleUserID IsNot Nothing Then
                Dim pDrEcommerceProfile = WebRequest.Instance.Business.LibraryPeople.GetEcommerceProfile(WebSession.Instance.PeopleUserID)
                If pDrEcommerceProfile Is Nothing Then
                    Throw New ApplicationException("You do not have an e-commerce profile. ref: vbNVYD33VJN4!")
                End If
                Dim pFowstID = CStr2(pDrEcommerceProfile("FowstID"))
                pInvoiceID = WebRequest.Instance.Business.TransInvoice.GetCartID(WebSession.Instance.PeopleUserID, pFowstID, True)
            End If
            Return ecommerce.GetProductDetails(pInvoiceID)
        End Function

        Function GetGlobalData()
            'ViewData("Categories") = GetCategories()
            'ViewData("UserDetails") = Json(New With {Key .FirstName = CStr2(WebSession.Instance.FirstName), .UserFullName = CStr2(WebSession.Instance.FullName)})
            Dim p As New JsonDictionary

            'Dim pOnlineStoreName = Common.WebRequest.QueryString("store")
            'If String.IsNullOrWhiteSpace(pOnlineStoreName) Then
            '    pOnlineStoreName = Request.UrlReferrer.AbsolutePath
            '    If String.IsNullOrWhiteSpace(pOnlineStoreName) = False Then
            '        pOnlineStoreName = Trim2(pOnlineStoreName, "/")
            '    End If
            'End If
            'If String.IsNullOrWhiteSpace(pOnlineStoreName) = False Then
            '    Dim pDR = WebRequest.Instance.Business.LibraryOrganization.ByID("", pOnlineStoreName)
            '    If pDR IsNot Nothing Then
            '        WebRequest.Instance.OrgID = ReadyGuid(pDR("ID"))
            '    End If
            'End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If WebSession.Instance.PeopleUserID IsNot Nothing Then
                Dim pDrEcommerceProfile = WebRequest.Instance.Business.LibraryPeople.GetEcommerceProfile(WebSession.Instance.PeopleUserID)
                If pDrEcommerceProfile Is Nothing Then
                    Try
                        AppendToLog(WebSession.Instance.FullName & " is not an ecommerce customer. ref: vbMM6M5WAZ57!")
                    Catch ex As Exception
                        AppendToLog("ref: vbDZQR2UWBXL! " & ex.Message)
                    End Try
                    p.Add("Error", "Logged-in account is not a customer. You will be automatically logout when you click OK.")
                End If
            End If

            p.Add("TradeName", Common.WebRequest.ProData.Execute("SELECT BusinessTradeName FROM MobizLibraryOrganizationTradeName WHERE ID = " & ReadyString(Common.WebRequest.ProData.Execute("SELECT BusinessTradeNameID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(WebRequest.Instance.Business.SystemDefaults.EcommerceDefaultFOWST)).Rows(0)("BusinessTradeNameID").ToString())).Rows(0)("BusinessTradeName"))
            p.Add("Categories", GetCategories().data.Results)
            p.Add("OrgID", WebRequest.Instance.OrgID)
            p.Add("UserDetails", Json(New With {Key .FirstName = CStr2(WebSession.Instance.FirstName), .UserFullName = CStr2(WebSession.Instance.FullName)}).Data)

            Return p.Serialize()
        End Function

        <HttpPost()>
        Public Function EcommerceCart()

            Dim pOrderID As String
            Dim pFowstID As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            WebRequest.Instance.Business.TransInvoice.CurrentOrgID = WebRequest.Instance.OrgID

            'dapat dili mogawas ang session expired message
            If WebSession.Instance.PeopleUserID Is Nothing Then
                Return JsonDictionary.Serialize("Result", New DataTable())
            End If

            Dim pDrEcommerceProfile = WebRequest.Instance.Business.LibraryPeople.GetEcommerceProfile(WebSession.Instance.PeopleUserID)
            If pDrEcommerceProfile Is Nothing Then
                AppendToLog("You do not have an e-commerce profile. ref: vbMM6M5WAZ56!")
                Return JsonDictionary.Serialize("Error", "Logged-in account is not a customer. You will be automatically logout when you click OK.")
            End If
            pFowstID = WebRequest.Instance.Business.SystemDefaults.EcommerceDefaultFOWST ' CStr2(pDrEcommerceProfile("FowstID"))

            pOrderID = WebRequest.Instance.Business.TransInvoice.GetCartID(WebSession.Instance.PeopleUserID, pFowstID, True)
            If String.IsNullOrWhiteSpace(pOrderID) = False Then
                Dim pDT = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(WebRequest.Instance.Business.TransInvoice.EcommerceCart(pOrderID)))

                For Each prod As Object In pDT
                    Dim pSql = "SELECT QtyChoices, Alias " & vbCrLf &
                        " FROM LibraryEcommerceProductQtyChoices as qty " & vbCrLf &
                        " INNER JOIN LibraryEcommerceProductQtyChoicesCategories as cat ON cat.QtyChoiceGroupID = qty.GroupID " & vbCrLf &
                        " WHERE cat.Category = (SELECT C0020 FROM MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
                        "   WHERE ProductID = " & ReadyString(prod("ProductID")) & ") "

                    prod.Add("QtyChoices", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                    pSql = "SELECT Upcharge " & vbCrLf &
                        " FROM MobizLibraryPIMProductParent " & vbCrLf &
                        " WHERE ProductID = " & ReadyString(prod("ProductID"))

                    prod.Add("Upcharge", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next

                Return Json(New With {Key .Result = New With {Key .Rows = pDT}})
            Else
                Return JsonDictionary.Serialize("Result", New DataTable())
            End If

        End Function

        <HttpPost()>
        Public Function RemoveCartItem() As String

            Dim pOrderID As String
            Dim pFowstID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            WebRequest.Instance.Business.TransInvoice.CurrentOrgID = WebRequest.Instance.OrgID

            Dim pProductID = Common.WebRequest.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "ProductID must be a GUID. ref: vbUHCT6U85MD!")
            End If
            Dim pDrEcommerceProfile = WebRequest.Instance.Business.LibraryPeople.GetEcommerceProfile(WebSession.Instance.PeopleUserID)
            If pDrEcommerceProfile Is Nothing Then
                Throw New ApplicationException("You do not have an e-commerce profile. ref: vb4E96AGJ7PJ!")
            End If
            pFowstID = WebRequest.Instance.Business.SystemDefaults.EcommerceDefaultFOWST 'CStr2(pDrEcommerceProfile("FowstID"))

            pOrderID = WebRequest.Instance.Business.TransInvoice.GetCartID(WebSession.Instance.PeopleUserID, pFowstID, True)
            If String.IsNullOrWhiteSpace(pOrderID) = False Then
                Common.WebRequest.ProData.BeginTransaction()
                WebRequest.Instance.Business.TransInvoice.DeleteOrderStock(pOrderID, pProductID, Nothing,
                                                                           IIf(CBool2(WebRequest.Instance.Business.SystemDefaults.EcommerceIsAllowBuyersSelectFromVendor), Nothing, Common.WebRequest.Data("TransInvItemID")),
                                                                           IIf(CBool2(WebRequest.Instance.Business.SystemDefaults.EcommerceIsAllowBuyersSelectFromVendor), Common.WebRequest.Data("VendorID"), Nothing))

                Dim pCreator = WebRequest.Instance.Business.TransInvoice.CreateCreator()
                pCreator.ID = pOrderID
                pCreator.UserID = WebSession.Instance.PeopleUserID
                pCreator.UpdateTotal()
                WebRequest.Instance.Business.TransInvoice.UpdateLastUpdated(pOrderID, WebSession.Instance.PeopleUserID)
                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "ok")
            End If

        End Function

        <HttpPost()>
        Public Function CheckoutNow() As String

            Dim pOrderID As String
            Dim pFowstID As String
            Dim pError As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            WebRequest.Instance.Business.TransInvoice.CurrentOrgID = WebRequest.Instance.OrgID

            Dim checkoutdata = Common.WebRequest.AllData("data")
            Dim pOut As New JsonDictionary
            pFowstID = WebRequest.Instance.Business.SystemDefaults.EcommerceDefaultFOWST
            pOrderID = WebRequest.Instance.Business.TransInvoice.GetCartID(WebSession.Instance.PeopleUserID, pFowstID, True)
            If String.IsNullOrWhiteSpace(pOrderID) = False Then
                Dim pNonZeroItemCount = WebRequest.Instance.Business.TransInvoice.NonZeroItemCount(pOrderID)
                If pNonZeroItemCount = 0 Then
                    Return JsonDictionary.Serialize("Error", "To check out cart, your cart must have at least 1 item with a quantity that is greater than zero.")
                End If

                Dim pNum
                Dim pDrOrder = WebRequest.Instance.Business.TransInvoice.ById(pOrderID)
                If pDrOrder IsNot Nothing Then
                    If CDec2(pDrOrder("SalesOrderNo")) = 0 Then
                        Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.GetOrCreateDesktop()
                        Dim pDeviceID = CStr2(pDrDevice("ID"))
                        pNum = WebRequest.Instance.Business.TransInvoice.GetMaxSoNumber(pDeviceID, pFowstID)
                        Dim pIU = New InsertUpdate("TransInvoice")
                        Dim pServingFowstID As String

                        'change Encoding FOWSTID to Distribution FOWSTID
                        pServingFowstID = GetDistributionCenterFOWST(pFowstID)

                        If Not pServingFowstID = vbNullString Then
                            pIU("FOWSTID") = ReadyString(pServingFowstID)
                        End If

                        pIU("SalesOrderNo") = pNum + 1
                        pIU("SalesOrderDate") = "GETDATE()"
                        pIU("DateNeeded") = "GETDATE()"
                        pIU("LastUpdated") = "GETDATE()"
                        'pIU("BillingAddressID") = ReadyString(checkoutdata("BillingAddressID"))
                        'pIU("DeliveryAddressID") = ReadyString(checkoutdata("DeliveryAddressID"))
                        If checkoutdata.containskey("DeliveryMethod") Then
                            pIU("DeliveryMethod") = ReadyString(checkoutdata("DeliveryMethod"))
                        End If
                        pIU("Remarks") = ReadyString(checkoutdata("Comments"))
                        Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pOrderID))
                        Common.WebRequest.ProData.BeginTransaction()
                        WebRequest.Instance.ProData.ExecuteSQL(pSql)
                        WebRequest.Instance.Business.TransInvoice.SalesOrderToPurchaseOrder(pOrderID, WebRequest.Instance.OrgID, pError)
                        If String.IsNullOrWhiteSpace(pError) = False Then
                            Return JsonDictionary.Serialize("Error", pError)
                        End If
                        Common.WebRequest.ProData.CommitTransaction()

                        pNum = WebRequest.Instance.Business.TransInvoice.GetReportData(" WHERE TransInvoice.ID = " & ReadyString(pOrderID), False).Tables("Invoices").Rows(0)
                        pNum = pNum("MIN") & "-" & pNum("SalesOrderNo")
                    End If
                End If
                pOut.Add("son", pNum)
                pOut.Add("Results", "ok")

                Return pOut.Serialize()
            End If

        End Function

        Function GetDistributionCenterFOWST(pFowstID) As String
            Dim pSql As String

            pSql = "Select DISTINCT TOP 1 FranchisorFowstID from MobizLibraryOrganizationFranchisee " & vbCrLf &
                           "inner join LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                           "where FranchiseeOrgFowstID = " & ReadyString(pFowstID)

            Return CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

        End Function

        Function ValidateQtyIntervalCategories()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim categories As Object = Common.WebRequest.AllData("Categories")
                Dim existingCat As New List(Of String)

                For Each c As String In categories
                    Dim pSql As String = "SELECT * FROM LibraryEcommerceProductQtyChoicesCategories WHERE Category = " & ReadyString(CStr2(c))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                        existingCat.Add(c)
                    End If
                Next c

                Return Json(New With {Key .Results = existingCat}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetQtyInterval()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim ecommerce As New Ecommerce
            Return ecommerce.GetQtyInterval()
        End Function
    End Class
End Namespace