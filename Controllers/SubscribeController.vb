Namespace MobizApplication
    Public Class SubscribeController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Subscribe

        Function Index() As ActionResult
            Return View()
        End Function

        Function StoreKeeper() As ActionResult
            Return View()
        End Function

        Function FullSizedStoreAndRestaurant() As ActionResult
            Return View()
        End Function

        Function KioskAndFoodTruck() As ActionResult
            Return View()
        End Function

        Function Free() As ActionResult
            Return View()
        End Function

        Function CompanyInfo() As ActionResult
            Return View()
        End Function

        Function AdminInfo() As ActionResult
            Return View()
        End Function

        Function Features() As ActionResult
            Return View()
        End Function

        Function SubscriptionAgreement() As ActionResult
            Return View()
        End Function

        Function GetCode() As ActionResult
            Return View()
        End Function

        Function DealMaker() As ActionResult
            Return View()
        End Function

        Function KiosksAndFoodTrucks() As ActionResult
            Return View()
        End Function

        Function DTRStandardPackage() As ActionResult
            Return View()
        End Function

        Function DTRWithFacialRecognition() As ActionResult
            Return View()
        End Function
        '<HttpPost()>
        'Sub SaveIsAgreed()

        '    'Set cookie to authorize the user to see the sign up page
        '    FormsAuthentication.SetAuthCookie("User", False)

        'End Sub

        Function AddOn() As ActionResult

            'Set cookie to authorize the user to see the thank you page
            FormsAuthentication.SetAuthCookie("User", False)

            Return View()
            ' If the user successfully registered (user is authorized to view this page)
            'If User.Identity.IsAuthenticated Then
            '    FormsAuthentication.SignOut() 'Clear the form
            '    Return View()

            'Else ' The user is not authorized. Therefore, redirect to home page
            '    Return RedirectToAction("Index", "Subscribe")
            'End If
        End Function

        Function ThankYou() As ActionResult
            ' If the user successfully registered (user is authorized to view this page)
            If User.Identity.IsAuthenticated Then
                FormsAuthentication.SignOut() 'Clear the form
                Return View()

            Else ' The user is not authorized. Therefore, redirect to home page
                Return RedirectToAction("KioskAndFoodTruck", "Subscribe")
            End If
        End Function

        Function InforMobizForNewCustomer()
            Try
                Dim emailid As String = Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID"))).Rows(0)("ID").ToString()

                'Set cookie to authorize the user to see the received request page
                FormsAuthentication.SetAuthCookie("User", False)

                Return New ConfirmedController().EmailContent(emailid, False)
            Catch ex As Exception

            End Try
        End Function

        <HttpPost()>
        Function SaveOrganizationInfo() As String
            Dim clsOrg As New Subscribe
            Dim strReturn As String

            clsOrg.OrganizationID = ""
            clsOrg.CompanyName = Common.WebRequest.Data("CompanyName")
            clsOrg.CompanyNameInitial = Common.WebRequest.Data("CompanyNameInitial")

            clsOrg.OwnershipType = Common.WebRequest.Data("OwnershipType")
            clsOrg.IsFranchisor = Common.WebRequest.Data("IsFranchisor")
            clsOrg.RegistrationType = Common.WebRequest.Data("RegistrationType")
            clsOrg.RegistrationNo = Common.WebRequest.Data("RegistrationNo")
            clsOrg.RegistrationDate = Common.WebRequest.Data("RegistrationDate")

            clsOrg.CorporateTIN = Common.WebRequest.Data("CorporateTIN")
            clsOrg.CategoryTypeID = Common.WebRequest.Data("CategoryTypeID")

            clsOrg.OfficeAddressBuildingNo = Common.WebRequest.Data("OfficeAddressBuildingNo")
            clsOrg.OfficeAddressStreet = Common.WebRequest.Data("OfficeAddressStreet")
            clsOrg.OfficeAddressCityID = Common.WebRequest.Data("OfficeAddressCityID")
            clsOrg.OfficeAddressCountryID = Common.WebRequest.Data("OfficeAddressCountryID")

            clsOrg.BusinessAddressBuildingNo = Common.WebRequest.Data("BusinessAddressBuildingNo")
            clsOrg.BusinessAddressStreet = Common.WebRequest.Data("BusinessAddressStreet")
            clsOrg.BusinessAddressCityID = Common.WebRequest.Data("BusinessAddressCityID")
            clsOrg.BusinessAddressCountryID = Common.WebRequest.Data("BusinessAddressCountryID")

            clsOrg.BusinessTradeName = Common.WebRequest.Data("BusinessTradeName")
            clsOrg.BusinessTradeNameInitial = Common.WebRequest.Data("BusinessTradeNameInitial")

            clsOrg.IsOrganization = Common.WebRequest.Data("IsOrganization")

            clsOrg.ReferenceNo = CStr2(Common.WebRequest.Data("ReferenceNo"))

            WebSession.Instance.OrganizationID = clsOrg.SaveOrganizationInfo()

            If (WebSession.Instance.OrganizationID = "Error") Then
                strReturn = JsonDictionary.Serialize("OrgData", "Error")
            Else
                strReturn = JsonDictionary.Serialize("OrgData", WebSession.Instance.OrganizationID)

                'Set cookie to authorize the user to see the register success page
                FormsAuthentication.SetAuthCookie("User", False)
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleInfo() As String
            Dim clsPeople As New Subscribe
            Dim strReturn As String
            Try

                If (Not IsDate(Common.WebRequest.Data("DateOfBirth"))) Then
                    strReturn = JsonDictionary.Serialize("PeopleNameData", "DateOfBirthError")
                Else

                    clsPeople.PeopleID = ""
                    clsPeople.Surname = Common.WebRequest.Data("Surname")
                    clsPeople.FirstName = Common.WebRequest.Data("FirstName")
                    clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
                    clsPeople.PersonalSuffixName = Common.WebRequest.Data("PersonalSuffixName")
                    clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
                    clsPeople.NickName = Common.WebRequest.Data("Nickname")
                    clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
                    clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
                    clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")

                    clsPeople.Gender = Common.WebRequest.Data("Gender")

                    clsPeople.PersonContactCountryIDMobile = Common.WebRequest.Data("PersonContactCountryIDMobile")
                    clsPeople.PersonContactTelCoPrefixID = Common.WebRequest.Data("PersonContactTelCoPrefixID")
                    clsPeople.PersonContactNoMobile = Common.WebRequest.Data("PersonContactNoMobile")
                    clsPeople.ContactType = Common.WebRequest.Data("ContactType")

                    clsPeople.PersonContactEmail = Common.WebRequest.Data("PersonContactEmail")
                    clsPeople.EmailType = Common.WebRequest.Data("EmailType")

                    clsPeople.UserName = Common.WebRequest.Data("UserName")
                    clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")

                    clsPeople.SecurityQuestion1 = Common.WebRequest.Data("Question1")
                    clsPeople.SecurityQuestion1Answer = Common.WebRequest.Data("Question1Answer")
                    clsPeople.SecurityQuestion2 = Common.WebRequest.Data("Question2")
                    clsPeople.SecurityQuestion2Answer = Common.WebRequest.Data("Question2Answer")

                    clsPeople.IsCertifiedCorrect = Common.WebRequest.Data("IsCertifiedCorrect")

                    clsPeople.PrimaryIDType = Common.WebRequest.Data("PrimaryIDReference")

                    clsPeople.DTRPin = Common.WebRequest.Data("DTRPin")

                    If (clsPeople.PrimaryIDType = "SSS") Then
                        clsPeople.SSSNo = Common.WebRequest.Data("PrimaryID")
                    End If

                    If (clsPeople.PrimaryIDType = "Phil. Health") Then
                        clsPeople.PhilHealthNo = Common.WebRequest.Data("PrimaryID")
                    End If

                    If (clsPeople.PrimaryIDType = "TIN") Then
                        clsPeople.TIN = Common.WebRequest.Data("PrimaryID")
                    End If

                    If (clsPeople.PrimaryIDType = "HDMF") Then
                        clsPeople.HDMFNo = Common.WebRequest.Data("PrimaryID")
                    End If

                    If (clsPeople.PrimaryIDType = "NBI") Then
                        clsPeople.NBINo = Common.WebRequest.Data("PrimaryID")
                    End If

                    If (clsPeople.PrimaryIDType = "Passport") Then
                        clsPeople.PassportNo = Common.WebRequest.Data("PrimaryID")
                    End If

                    If (clsPeople.PrimaryIDType = "Driver's License") Then
                        clsPeople.DriversLicenseNo = Common.WebRequest.Data("PrimaryID")
                    End If

                    clsPeople.OrganizationID = WebSession.Instance.OrganizationID

                    'check if org id exists
                    If (CheckOrganizationIDExists()) Then

                        WebSession.Instance.PeopleUserID = clsPeople.SavePeopleInfo()

                        clsPeople.PeopleID = WebSession.Instance.PeopleUserID
                        strReturn = JsonDictionary.Serialize("SubscriptionData", WebSession.Instance.PeopleUserID)

                    Else
                        strReturn = JsonDictionary.Serialize("SubscriptionData", "NoOrganization")
                    End If

                End If

            Catch ex As Exception
                strReturn = JsonDictionary.Serialize("Error", "An error occured: " & ex.Message)
            End Try

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveSubscriptionInfo() As String
            Dim clsSubscribe As New Subscribe
            Dim strReturn As String

            clsSubscribe.OrganizationID = WebSession.Instance.OrganizationID
            clsSubscribe.PeopleID = WebSession.Instance.PeopleUserID
            clsSubscribe.PromoCode = Common.WebRequest.Data("PromoCode")
            clsSubscribe.AgentName = Common.WebRequest.Data("AgentName")
            clsSubscribe.SourceOfInfo = Common.WebRequest.Data("SourceOfInfo")

            If (Common.WebRequest.Data("OtherSource") <> "Other source/s of information" And Common.WebRequest.Data("OtherSource") <> "") Then
                clsSubscribe.SourceOfInfo = Common.WebRequest.Data("SourceOfInfo") & ", " & Common.WebRequest.Data("OtherSource")
            End If

            strReturn = JsonDictionary.Serialize("InvoiceData", clsSubscribe.SaveSubscriptionInfo())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveSubscriptionCartInfo() As String
            Dim clsSubscribe As New Subscribe
            Dim strReturn As String

            clsSubscribe.InvoiceID = Common.WebRequest.Data("InvoiceID")
            clsSubscribe.ProductID = Common.WebRequest.Data("ProductID")
            clsSubscribe.Qty = Common.WebRequest.Data("Qty")
            clsSubscribe.Price = Common.WebRequest.Data("Price")

            strReturn = JsonDictionary.Serialize("SubscriptionCartData", clsSubscribe.SaveSubscriptionCartInfo())

            'Set cookie to authorize the user to see the thank you page
            FormsAuthentication.SetAuthCookie("User", False)

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleUniqueName() As String
            Dim clsPeople As New Subscribe
            Dim strReturn As String
            Dim blnFullCheckName As Boolean

            clsPeople.PeopleID = Common.WebRequest.Data("PeopleID")
            clsPeople.Surname = Common.WebRequest.Data("Surname")
            clsPeople.FirstName = Common.WebRequest.Data("FirstName")
            clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
            clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
            clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
            clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
            clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")
            clsPeople.PersonalSuffixName = Common.WebRequest.Data("PersonalSuffixName")
            blnFullCheckName = Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("FullCheckName")))

            strReturn = JsonDictionary.Serialize("PeopleNameData", clsPeople.CheckPeopleUniqueName(blnFullCheckName))

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleUniqueUserName() As String
            Dim clsPeople As New Subscribe
            Dim strReturn As String

            clsPeople.UserName = Common.WebRequest.Data("Username")

            strReturn = JsonDictionary.Serialize("PeopleUserNameData", clsPeople.CheckPeopleUniqueUserName())

            Return strReturn

        End Function


        Function CheckOrganizationIDExists() As Boolean
            Dim clsOrg As New Subscribe
            Dim blnReturn As Boolean

            clsOrg.OrganizationID = WebSession.Instance.OrganizationID

            blnReturn = clsOrg.CheckOrganizationIDExists()

            Return blnReturn

        End Function

        <HttpPost()>
        Function CheckOrganizationUnique() As String
            Dim clsOrg As New Subscribe
            Dim strReturn As String

            clsOrg.CompanyName = Common.WebRequest.Data("CompanyName")

            strReturn = JsonDictionary.Serialize("UniqueOrgData", clsOrg.CheckOrganizationUnique())

            Return strReturn

        End Function



        <HttpPost()>
        Function SendPromoCode() As String
            Dim pOut As JsonDictionary
            Dim strBody As String
            Dim email As String
            Dim clsSubscribe As New Subscribe
            Dim agentCode As String
            Try
                'Get mobiz sales agent code
                agentCode = clsSubscribe.GetAgentCode()

                'Construct the message
                email = Common.WebRequest.Data("EmailAddress").Trim()

                strBody = "<p>Hello " & email & ", </p>" &
                        "<p>Thank you for your interest in Mobiz. Please enter the code below when you sign up for Mobiz Complete Retail Free Trial.</p>" &
                        "<h3>" & agentCode & "</h3>" &
                        "<p>This code entitles you to a 30-day free trial. This no commitment trial period is perfect for entrepreneurs and business owners out there who want to take their business to the next level. If you want to learn more about Mobiz, visit our website at <a href=""http://mobiz-apps.com"">www.mobiz-apps.com</a>.</p>" &
                        "<p>All the best, <br/>" &
                        "The Mobiz Team<br/>" &
                        "Business-Grade Software in Your Pocket<br/>" &
                        "<a href=""http://mobiz-apps.com"">www.mobiz-apps.com</a> </p>"


                SendMail("mobizcompleteretail@outlook.com",
                     email,
                     "Mobiz",
                     "Mobiz Promo Code",
                     strBody,
                     "smtp.live.com",
                     587,
                     "mobizcompleteretail@outlook.com",
                     "m0b!zp30pl30nly!", , , , , True)

                'Set cookie to authorize the user to see the received request page
                FormsAuthentication.SetAuthCookie("User", False)

                pOut = New JsonDictionary
                pOut.Add("Result", "True")

                Return pOut.Serialize

            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", "An error occured while sending email: " & ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function SendEmailConfirmation() As String
            Dim pOut As JsonDictionary
            Dim strBody As String
            Dim linkID As String
            Dim confirmCode As String
            Dim email As String
            Dim clsPeople As New Subscribe
            Dim completeLink As String

            Try
                clsPeople.PeopleID = WebSession.Instance.PeopleUserID

                linkID = clsPeople.GetEmailActivationLinkID()
                confirmCode = GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper
                completeLink = "http://mobiz-apps.com/confirmed?id=" & linkID & "&c=" & confirmCode
                'Construct the message

                'strBody = "<p>Hello " & Common.WebRequest.Data("FirstName").Trim() & ", </p>" &
                '        "<p>Thank you for subscribing. Your Mobiz account has been successfully registered, along with your company details. Please verify your email address by clicking the link below: " &
                '        "<br/><a href=" & completeLink & ">" & completeLink & "</a><br/></p>" &
                '        "<p>After verifying your email address, we will send you an email to instruct you on how to pay for and activate your subscription (and receive your add-on hardware purchases if applicable).  </p>" &
                '        "<p>From the Mobiz Team, thank you very much for giving us the opportunity to help you better run your business.</p>" &
                '        "<br/></p>" &
                '        "<p>All the best, <br/>" &
                '        "The Mobiz Team<br/>" &
                '        "Business-Grade Software in Your Pocket<br/>" &
                '        "<a href=""http://mobiz-apps.com"">www.mobiz-apps.com</a> </p>"

                'FREE TRIAL
                strBody = "<p>Hello " & Common.WebRequest.Data("FirstName").Trim() & ", </p>" &
                        "<p>Thank you for your interest in Mobiz. Your account has been successfully registered, along with your company details. Please verify your email address by clicking the link below: " &
                        "<br/><a href=" & completeLink & ">" & completeLink & "</a><br/></p>" &
                        "<p>After verifying your email address, we will activate your Mobiz Complete Retail " & Common.WebRequest.Data("TrialDuration").Trim() & "-Day Free Trial . If you have questions or feedback, call us at (082) 286-9579 or go to <a href=""http://mobiz-apps.com/support/contactus"">www.mobiz-apps.com/support/contactus</a>. </p>" &
                        "<p>From the Mobiz Team, thank you very much for giving us the opportunity to help you better run your business.</p>" &
                        "<br/></p>" &
                        "<p>All the best, <br/>" &
                        "The Mobiz Team<br/>" &
                        "Business-Grade Software in Your Pocket<br/>" &
                        "<a href=""http://mobiz-apps.com"">www.mobiz-apps.com</a> </p>"


                email = Common.WebRequest.Data("EmailAddress").Trim()


                SendMail("mobizcompleteretail@outlook.com",
                     email,
                     "Mobiz",
                     "Welcome to Mobiz!",
                     strBody,
                     "smtp.live.com",
                     587,
                     "mobizcompleteretail@outlook.com",
                     "m0b!zp30pl30nly!", , , , , True)

                'Set cookie to authorize the user to see the received request page
                FormsAuthentication.SetAuthCookie("User", False)

                pOut = New JsonDictionary
                pOut.Add("Result", "True")

                Return pOut.Serialize

            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", "An error occured while sending email: " & ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function getProductList() As String
            Dim clsSubscribe As New Subscribe
            Dim strReturn As String

            strReturn = JsonDictionary.Serialize("ProductData", clsSubscribe.getProductList())

            Return strReturn

        End Function

        <HttpPost()>
        Function getSubscriptionDuration() As String
            Dim clsSubscribe As New Subscribe
            Dim strReturn As String

            clsSubscribe.ProductID = Common.WebRequest.Data("ProductID")

            strReturn = JsonDictionary.Serialize("DurationData", clsSubscribe.getSubscriptionDuration())

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPromoCodeExists() As String
            Dim pDT As DataTable
            Dim strReturn As String

            pDT = Common.WebRequest.ProData.Execute("SELECT SalesAgentCode as PromoCode FROM LibraryPeople WHERE LibraryPeople.SalesAgentCode LIKE " + ReadyString(Common.WebRequest.Data("PromoCode")) + " UNION SELECT SalesAgentCode as PromoCode FROM LibraryOrganization WHERE LibraryOrganization.SalesAgentCode = " + ReadyString(Common.WebRequest.Data("PromoCode")))
            strReturn = JsonDictionary.Serialize("AgentData", pDT)

            Return strReturn

        End Function





    End Class
End Namespace