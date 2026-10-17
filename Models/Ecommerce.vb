Public Class Ecommerce
    Inherits System.Web.Mvc.Controller

    Public Property MaxReturnRow As Integer = 50
    Public Property CategoryID As String
    Public Property ProductID As String
    Public Property Keyword As String
    Public Property GroupID As String
    Public Property PersonDetails As Object
    Public Property Url As String

    Public Property EmailLinkID As String
    Public Property ConfirmationCode As String
    Public Property OID As String
    Public Property AccType As String

    'For Homepage
    Public Property min As Integer = 1
    Public Property max As Integer = 16

    Function GetQtyInterval()
        Dim pSql As String = "SELECT GroupID FROM LibraryEcommerceProductQtyChoices GROUP BY GroupID"
        Dim pRows As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        For Each qty As Object In pRows
            pSql = "SELECT * FROM LibraryEcommerceProductQtyChoices WHERE GroupID = " & ReadyString(qty("GroupID"))
            Dim pRowsCat As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            Dim pChoices As New List(Of Object)

            For Each choices As Object In pRowsCat
                pChoices.Add(New With {Key .ID = choices("ID"), .Status = "edit", .Qty = choices("QtyChoices"), .Alias = choices("Alias")})
            Next choices

            qty.Add("choices", pChoices)

            pSql = "SELECT * FROM LibraryEcommerceProductQtyChoicesCategories WHERE QtyChoiceGroupID = " & ReadyString(qty("GroupID"))
            Dim pCat As New List(Of String)

            For Each cat As Object In Common.WebRequest.ProData.Execute(pSql).Rows
                pCat.Add(cat("Category"))
            Next cat

            qty.Add("categories", pCat)
            qty.Add("Status", "edit")
        Next

        Return Json(New With {Key .Results = pRows })
    End Function

    Function SaveQtyInterval()
        Try
            Common.WebRequest.ProData.BeginTransaction()

            Dim qtyintervals As Object = Common.WebRequest.AllData("QtyIntervalsTemplate")
            Dim qtygroupid As String = Guid.NewGuid.ToString()

            For Each qty As Object In qtyintervals
                Dim p As New InsertUpdate("LibraryEcommerceProductQtyChoices")

                For Each interval As Object In qty("choices")
                    p = New InsertUpdate("LibraryEcommerceProductQtyChoices")

                    p("QtyChoices") = ReadyString(interval("Qty"))
                    p("Alias") = ReadyString(interval("Alias"))
                    p("LastUpdated") = "getdate()"
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                    If interval("Status") = "add" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("GroupID") = ReadyString(qtygroupid)
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    ElseIf interval("Status") = "edit" Then
                        qtygroupid = qty("GroupID")
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(interval("ID"))))
                    Else
                        qtygroupid = qty("GroupID")
                        Common.WebRequest.ProData.Execute("DELETE FROM LibraryEcommerceProductQtyChoices WHERE ID = " & ReadyString(interval("ID")))
                    End If
                Next interval

                Common.WebRequest.ProData.Execute("DELETE FROM LibraryEcommerceProductQtyChoicesCategories WHERE QtyChoiceGroupID = " & ReadyString(qtygroupid))

                If qty("Status") <> "delete" Then
                    For Each cat As Object In qty("categories")
                        p = New InsertUpdate("LibraryEcommerceProductQtyChoicesCategories")

                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("QtyChoiceGroupID") = ReadyString(qtygroupid)
                        p("Category") = ReadyString(cat)

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    Next cat
                End If
            Next qty

            Common.WebRequest.ProData.CommitTransaction()
            Return False
        Catch ex As Exception
            AppendToLog("Qty Interval: " & ex.Message)
            Throw New ApplicationException(ex.Message)
        End Try
    End Function

    Function GetBanner()
        Try
            Dim pSql As String = "SELECT ID, FileName FROM LibraryEcommerceBanners "

            Return Common.WebRequest.ProData.Execute(pSql)
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveAddress()
        Try
            Dim data As Object = Common.WebRequest.AllData("data")

            Dim pSql As String
            Dim p As New InsertUpdate("MobizLibraryOrganizationFowst")

            p("OrganizationID") = ReadyString(WebRequest.Instance.Business.LibraryPeople.GetEcommerceProfile(WebSession.Instance.PeopleUserID)("OrganizationID"))
            p("StreetNumber") = ReadyString(data("Address").Trim())
            p("IsActive") = 1

            If data("Action") = "add" Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            ElseIf data("Action") = "edit" Then
                pSql = p.GetUpdate(" ID = " & ReadyString(data("ID")))
            ElseIf data("Action") = "delete" Then
                pSql = "DELETE FROM MobizLibraryOrganizationFowst WHERE ID = " & ReadyString(data("ID"))
            End If

            Common.WebRequest.ProData.Execute(pSql)
            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function ResetCredentials() As String
        Try
            Dim pSql As String = "SELECT ID FROM MobizLibraryPeopleUserAccount WHERE Username = " & ReadyString(PersonDetails("Username")) & vbCrLf &
                                    " AND PeopleID <> " & ReadyString(WebSession.Instance.PeopleUserID)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                Return "Sorry, that username is taken."
            Else
                Dim p As New InsertUpdate("MobizLibraryPeopleUserAccount")
                Dim msg As String = "<table style='width:100%; border:none; background-color: #f5f5f5;'>" +
                                        "<tr><td style='height:100px; text-align:center; padding:20px; vertical-align:middle; background-color: #c52c2c; font-size: 40px; color: #FFF;'>Security Alert</td></tr>" +
                                        "<tr><td style='padding:20px; font-size: 20px;'>Hello " + WebSession.Instance.FirstName + ",<br /><br />Your Mobiz E-commerce account has been reset on " + PersonDetails("DateNow") + ".</td></tr>" +
                                        "<tr><td style='padding:20px; vertical-align:middle; background-color: #dadada; font-size: 16px;'>To know more about Mobiz, please <a href='http://mobiz-apps.com' target='_blank' style='text-decoration:none;'>click here</a>.</td></tr>" +
                                    "</table>"

                p("Username") = ReadyString(PersonDetails("Username"))
                p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("Password")))

                Common.WebRequest.ProData.Execute(p.GetUpdate(" PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)))

                WebSession.Instance.PeopleUserID = Nothing  'Clear this variable after save
                WebSession.Instance.FirstName = ""

                SendMail("mobizecommerce@outlook.com",
                     PersonDetails("EmailAddress"),
                     "E-commerce",
                     "Security Alert from Mobiz E-commerce",
                     msg,
                     "smtp.live.com",
                     587,
                     "mobizecommerce@outlook.com",
                     "m0b!zp30pl30nly!", , , , , True)

                Return "saved"
            End If
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return "There was an error while saving your changes to your account. Please contact your system administrator."
        End Try
    End Function

    Function CheckSecQAnswer() As String
        Dim pSql As String = "SELECT SecurityAnswer FROM MobizLibraryPeopleSecurityQuestion WHERE ID = " & ReadyString(PersonDetails(0)("ID").ToString())
        Dim sechash As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("SecurityAnswer")

        If Not ProPack.Security.Hash.Verify(PersonDetails(0)("Answer"), sechash) Then
            Return "Invalid answer for question no. 1"
        End If

        pSql = "SELECT SecurityAnswer FROM MobizLibraryPeopleSecurityQuestion WHERE ID = " & ReadyString(PersonDetails(1)("ID").ToString())
        sechash = Common.WebRequest.ProData.Execute(pSql).Rows(0)("SecurityAnswer")

        If Not ProPack.Security.Hash.Verify(PersonDetails(1)("Answer"), sechash) Then
            Return "Invalid answer for question no. 2"
        End If

        Return "valid"

    End Function

    Function CheckUserForgotPassword() As Object
        Dim pSql As String = "SELECT ID FROM MobizLibraryPeople WHERE FirstName = " & ReadyString(PersonDetails("FirstName")) & vbCrLf &
                            IIf(PersonDetails("MiddleName") = "", "", " AND MiddleName = " & ReadyString(PersonDetails("MiddleName"))) & vbCrLf &
                            " AND Surname = " & ReadyString(PersonDetails("LastName")) & vbCrLf &
                            IIf(PersonDetails("GenSuffix") = "", "", " AND PersonalSuffixName = " & ReadyString(PersonDetails("GenSuffix")))
        Dim row As Object = Common.WebRequest.ProData.Execute(pSql).Rows
        Dim pOut As New JsonDictionary

        If row.Count > 0 Then
            Dim peopleID = row(0)("ID").ToString()
            WebSession.Instance.PeopleUserID = peopleID
            WebSession.Instance.FirstName = PersonDetails("FirstName")

            pSql = "SELECT EmailAddress FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(peopleID) & vbCrLf &
                    "AND EmailAddress = " & ReadyString(PersonDetails("EmailAddress"))

            row = Common.WebRequest.ProData.Execute(pSql).Rows

            If row.Count > 0 Then
                pSql = "SELECT ID, SecurityQuestion FROM MobizLibraryPeopleSecurityQuestion WHERE PeopleID = " & ReadyString(peopleID)
                pOut.Add("Results", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
            Else
                pOut.Add("Results", "invalid")
            End If
        Else
            pOut.Add("Results", "invalid")
        End If

        Return pOut.Serialize()
    End Function

    Function SaveMyAccount() As String
        Try
            Common.WebRequest.ProData.BeginTransaction()

            Dim p As New InsertUpdate("MobizLibraryPeople")

            If Not CBool2(Common.WebRequest.Data("DonotSavePersonDetails")) Then
                If CBool2(PersonDetails("IsEditName")) Then
                    p("FirstName") = ReadyString(PersonDetails("FirstName"))
                    p("MiddleName") = ReadyString(PersonDetails("MiddleName"))
                    p("Surname") = ReadyString(PersonDetails("LastName"))
                    p("PersonalSuffixName") = ReadyString(PersonDetails("GenSuffix"))
                End If

                p("DateOfBirth") = ReadyString(CDate(PersonDetails("DateOfBirth")))
                p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))
                p("UserID") = ReadyString(PersonDetails("PeopleUserID"))

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("PeopleUserID"))))

                p = New InsertUpdate("MobizLibraryOrganization")

                If CBool2(PersonDetails("IsEditName")) Then
                    p("PersonFirstName") = ReadyString(PersonDetails("FirstName"))
                    p("PersonMiddleName") = ReadyString(PersonDetails("MiddleName"))
                    p("PersonSurname") = ReadyString(PersonDetails("LastName"))
                    p("PersonSuffixName") = ReadyString(PersonDetails("GenSuffix"))
                End If

                p("PersonDateOfBirth") = ReadyString(CDate(PersonDetails("DateOfBirth")))
                p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("PeopleUserID"))))

                p = New InsertUpdate("MobizLibraryPeopleContactEmail")
                p("EmailAddress") = ReadyString(PersonDetails("PersonEmailAdd"))

                Common.WebRequest.ProData.Execute(p.GetUpdate(" PeopleID = " & ReadyString(PersonDetails("PeopleUserID"))))

                If CBool2(PersonDetails("IsChangePassword")) Then
                    Dim signup As New SignUp

                    signup.UserName = PersonDetails("Username")

                    If signup.CheckPeopleUniqueUserName().Rows.Count > 0 Then
                        Return "exist"
                    End If

                    p = New InsertUpdate("MobizLibraryPeopleUserAccount")
                    p("Username") = ReadyString(PersonDetails("Username"))
                    p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("NewPassword")))

                    Common.WebRequest.ProData.Execute(p.GetUpdate(" PeopleID = " & ReadyString(PersonDetails("PeopleUserID"))))
                End If

                p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                p("SecurityQuestion") = ReadyString(PersonDetails("SecQ1"))
                p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("SecQ1Ans")))

                If CBool2(PersonDetails("IsEditAns1")) Then
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("SecQ1ID"))))
                End If

                p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

                p("SecurityQuestion") = ReadyString(PersonDetails("SecQ2"))
                p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("SecQ2Ans")))

                If CBool2(PersonDetails("IsEditAns2")) Then
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("SecQ2ID"))))
                End If
            End If

            If Not CBool2(Common.WebRequest.Data("DonotSaveCompanyDetails")) Then
                If PersonDetails("AccountType") = "company" Then
                    Dim orgid As String = Guid.NewGuid.ToString()

                    p = New InsertUpdate("MobizLibraryOrganization")

                    p("CompanyName") = ReadyString(PersonDetails("CompanyName"))
                    p("TinNumber") = ReadyString(PersonDetails("Tin"))
                    p("IndustryClassID") = IIf(CStr2(PersonDetails("BusinessIndustry")) = "", "NULL", ReadyString(PersonDetails("BusinessIndustry")))
                    p("OfficeAddressBuildingNo") = ReadyString(PersonDetails("CompanyBldgNo"))
                    p("OfficeAddressStreet") = ReadyString(PersonDetails("CompanyStreetName"))
                    p("OfficeAddressCityID") = ReadyString(PersonDetails("CompanyCity"))
                    p("OfficeAddressCountryID") = ReadyString(PersonDetails("CompanyCountry"))
                    p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

                    If PersonDetails.ContainsKey("CompanyBackground") Then
                        p("Background") = ReadyString(PersonDetails("CompanyBackground"))
                    End If

                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("CompanyID"))))

                    If PersonDetails.ContainsKey("CompanyEmailAddresses") Then
                        For Each email As Object In PersonDetails("CompanyEmailAddresses")
                            p = New InsertUpdate("MobizLibraryOrganizationContactEmail")

                            p("EmailAddress") = ReadyString(email("EmailAddress").ToString())
                            p("OrganizationID") = ReadyString(email("OrganizationID"))
                            p("LastUpdated") = "getdate()"

                            If CStr2(email("ID")) = "" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            ElseIf email("State") = "edit" Then
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(email("ID"))))
                            ElseIf email("State") = "remove" Then
                                Common.WebRequest.ProData.Execute("DELETE FROM MobizLibraryOrganizationContactEmail WHERE ID = " & ReadyString(email("ID")))
                            End If
                        Next email
                    Else
                        p = New InsertUpdate("MobizLibraryOrganizationContactEmail")

                        p("EmailAddress") = ReadyString(PersonDetails("CompanyEmailAdd").ToString())
                        p("OrganizationID") = ReadyString(PersonDetails("CompanyID"))
                        p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

                        If CStr2(PersonDetails("CompanyEmailID")) = "" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        Else
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("CompanyEmailID"))))
                        End If
                    End If

                    If PersonDetails.ContainsKey("CompanyMobileNos") Then
                        For Each mobile As Object In PersonDetails("CompanyMobileNos")
                            p = New InsertUpdate("MobizLibraryOrganizationContactMobile")

                            p("OrganizationID") = ReadyString(mobile("OrganizationID"))
                            p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(mobile("ContactNo"))("ID"))
                            p("ContactNo") = ReadyString(mobile("ContactNo").ToString())
                            p("LastUpdated") = "getdate()"

                            If CStr2(mobile("ID")) = "" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            ElseIf mobile("State") = "edit" Then
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(mobile("ID"))))
                            ElseIf mobile("State") = "remove" Then
                                Common.WebRequest.ProData.Execute("DELETE FROM MobizLibraryOrganizationContactMobile WHERE ID = " & ReadyString(mobile("ID")))
                            End If
                        Next mobile
                    Else
                        p = New InsertUpdate("MobizLibraryOrganizationContactMobile")

                        p("OrganizationID") = ReadyString(PersonDetails("CompanyID"))
                        p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(PersonDetails("CompanyMobileNo"))("ID"))
                        p("ContactNo") = ReadyString(PersonDetails("CompanyMobileNo").ToString())
                        p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

                        If CStr2(PersonDetails("CompanyMobileID")) = "" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        Else
                            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("CompanyMobileID"))))
                        End If
                    End If

                    If PersonDetails.ContainsKey("CompanyTelephoneNos") Then
                        For Each tel As Object In PersonDetails("CompanyTelephoneNos")
                            p = New InsertUpdate("MobizLibraryOrganizationContactTel")

                            p("OrganizationID") = ReadyString(tel("OrganizationID"))
                            p("AreaCodeID") = ReadyString(tel("AreaCodeID"))
                            p("CountryID") = ReadyString(tel("CountryID"))
                            p("ExtensionNo") = ReadyString(tel("ExtensionNo"))
                            p("ContactNo") = ReadyString(tel("ContactNo").ToString())
                            p("ContactType") = ReadyString("Work Telephone")
                            p("LastUpdated") = "getdate()"

                            If CStr2(tel("ID")) = "" Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            ElseIf tel("State") = "edit" Then
                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(tel("ID"))))
                            ElseIf tel("State") = "remove" Then
                                Common.WebRequest.ProData.Execute("DELETE FROM MobizLibraryOrganizationContactTel WHERE ID = " & ReadyString(tel("ID")))
                            End If
                        Next tel
                    End If
                End If
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return "error"
        End Try
    End Function

    Function MyAccountDetails() As String
        Dim pOut As New JsonDictionary
        Dim pSql As String
        Dim browse As New BrowserController

        pSql = "SELECT OrganizationID FROM MobizLibraryOrganizationContactPerson WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
        Dim org As Object = Common.WebRequest.ProData.Execute(pSql).Rows

        If org.Count > 0 Then
            pSql = "SELECT org.ID, CompanyName, TinNumber, Background, IndustryClassID, ind.Class as IndustryClassName, OfficeAddressBuildingNo, OfficeAddressStreet, OfficeAddressCityID, OfficeAddressCountryID " & vbCrLf &
                    " FROM MobizLibraryOrganization as org " & vbCrLf &
                    " LEFT JOIN MobizLibraryIndustryClass as ind ON ind.ID = org.IndustryClassID " & vbCrLf &
                    " WHERE org.ID = " & ReadyString(org(0)("OrganizationID").ToString())
            pOut.Add("CompanyDetails", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT *, 'edit' as State FROM MobizLibraryOrganizationContactEmail WHERE OrganizationID = " & ReadyString(org(0)("OrganizationID").ToString())
            pOut.Add("CompanyEmail", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT *, 'edit' as State FROM MobizLibraryOrganizationContactMobile WHERE OrganizationID = " & ReadyString(org(0)("OrganizationID").ToString())
            pOut.Add("CompanyMobile", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = "SELECT tel.*, 'edit' as State, ctr.Code + ' - ' + ctr.CallingCode as CountryCode, area.Area + ' - ' + area.CallingCode as AreaCode FROM MobizLibraryOrganizationContactTel as tel INNER JOIN MobizLibraryAreaCodes as area ON area.ID = tel.AreaCodeID INNER JOIN MobizLibraryCountries as ctr ON ctr.ID = tel.CountryID WHERE OrganizationID = " & ReadyString(org(0)("OrganizationID").ToString())
            pOut.Add("CompanyTelephone", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pOut.Add("AccountType", "company")
        Else
            pOut.Add("AccountType", "personal")
        End If

        pOut.Add("BusinessIndustries", jss.DeserializeObject(browse.LibraryIndustryClassification(True))("d")("Results")("Rows"))
        pOut.Add("Cities", jss.DeserializeObject(browse.LibraryCities(True))("d")("Results")("Rows"))
        pOut.Add("Countries", jss.DeserializeObject(browse.LibraryCountries(True))("d")("Results")("Rows"))

        pSql = "SELECT FirstName, MiddleName, Surname, CONVERT(Varchar, DateOfBirth) as DateOfBirth, PersonalSuffixName, IsActive FROM MobizLibraryPeople WHERE ID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("Details", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        pSql = "SELECT ID, EmailAddress FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("Email", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        pSql = "SELECT ID, Username FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("Credentials", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        pSql = "SELECT ID, SecurityQuestion FROM MobizLibraryPeopleSecurityQuestion WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("SecQ", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        Return pOut.Serialize()
    End Function

    Function ConfirmedLink() As String
        Dim pSql = "SELECT * FROM MobizLibraryPeopleContactEmail WHERE EmailActivationLinkID = " & ReadyString(EmailLinkID)
        Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
        Dim signinlink As String

        If AccType = "cus" Then
            signinlink = "<a href='" & Url & "/Ecommerce/Signin?OrgID=" & OID & "'>click here</a>"
        Else
            signinlink = "<a href='" & Url & "/VendorPortal/Signin?OrgID=" & OID & "'>click here</a>"
        End If

        If rows.Count = 0 Then
            Return "Confirmation link not found. Please " & signinlink & " to sign up."
        Else
            If CBool(rows(0)("IsEmailConfirmed")) Then
                Return "Link already expired. Please " & signinlink & " to sign in."
            End If

            Common.WebRequest.ProData.BeginTransaction()

            Dim p As New InsertUpdate("MobizLibraryPeopleContactEmail")

            p("ConfirmationCode") = ReadyString(ConfirmationCode)
            p("IsEmailConfirmed") = 1

            pSql = p.GetUpdate(" EmailActivationLinkID = " & ReadyString(EmailLinkID))
            Common.WebRequest.ProData.Execute(pSql)

            'Get PeopleID of user
            pSql = "SELECT PeopleID FROM MobizLibraryPeopleContactEmail WHERE EmailActivationLinkID = " & ReadyString(EmailLinkID)
            Dim peopleid As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("PeopleID").ToString()

            'check if user tag as contact person, if tag, the affiliated company must be tag as customer/supplier
            pSql = "SELECT * FROM MobizLibraryOrganizationContactPerson WHERE PeopleID = " & ReadyString(peopleid)
            Dim ctr As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            Dim id As String

            If ctr.Count > 0 Then 'save the company as customer/supplier
                id = ctr(0)("OrganizationID").ToString()
            Else
                id = peopleid
            End If

            Dim code As String = New OrganizationShort().GenerateCodeNumber(IIf(AccType = "cus", True, False))

            'SAVE TO CUSTOMER or SUPPLIER LIBRARY (tag new user as customer/supplier)

            If AccType = "cus" Then
                p = New InsertUpdate("LibraryEcommerceCustomers")

                p("OrganizationID") = ReadyString(id)

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" PersonID = " & ReadyString(peopleid))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PersonID") = ReadyString(peopleid)
                    Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())
                End If

                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("OrganizationID") = ReadyString(id)
                p("SalesRepID") = "NULL"
                p("CustomerCode") = ReadyString(code)
            Else
                p = New InsertUpdate("LibraryOrganizationSupplier")

                p("SupplierID") = ReadyString(id)
                p("VendorCode") = ReadyString(code)
            End If
            
            p("ActivationDate") = "GetDate()"
            p("DeActivated") = 0
            p("IsBan") = ReadyString("0")
            p("LastUpdated") = "getdate()"
            p("ThirdPartyID") = "NULL"

            If AccType = "cus" Then
                pSql = p.GetUpdate(" OrganizationID = " & ReadyString(id))
            Else
                pSql = p.GetUpdate(" SupplierID = " & ReadyString(id))
            End If

            If Common.WebRequest.ProData.ExecuteSQL(pSql) = 0 Then
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return rows(0)("PeopleID").ToString()
        End If
    End Function

    Function CreateAccount(ByVal usertype As String) As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim ppolID As String = Guid.NewGuid().ToString()
        Dim emaillinkid As String = Guid.NewGuid.ToString()
        Dim signup As New SignUp
        Dim orguserid As String
        Dim pSql As String

        Try
            Common.WebRequest.ProData.BeginTransaction()

            signup.UserName = PersonDetails("Username")

            If signup.CheckPeopleUniqueUserName().Rows.Count > 0 Then
                Return "exist"
            End If

            p("ID") = ReadyString(ppolID)
            p("FirstName") = ReadyString(PersonDetails("FirstName"))
            p("MiddleName") = ReadyString(PersonDetails("MiddleName"))
            p("Surname") = ReadyString(PersonDetails("LastName"))
            p("PersonalSuffixName") = ReadyString(PersonDetails("GenSuffix"))
            p("DateOfBirth") = ReadyString(CDate(PersonDetails("DateOfBirth")))
            p("IsActive") = 1
            p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))
            p("IsFromOrg") = ReadyString(IIf(PersonDetails("AccountType") = "company", 1, 0))

            Common.WebRequest.ProData.Execute(p.GetInsert())

            p = New InsertUpdate("MobizLibraryPeopleContactEmail")

            p("PeopleID") = ReadyString(ppolID)
            p("EmailAddress") = ReadyString(PersonDetails("PersonEmailAdd"))
            p("EmailType") = ReadyString("Work Email")
            p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))
            p("IsEmailConfirmed") = 0
            p("EmailActivationLinkID") = ReadyString(emaillinkid)

            Common.WebRequest.ProData.Execute(p.GetInsert())

            p = New InsertUpdate("MobizLibraryPeopleAddress")

            p("PeopleID") = ReadyString(ppolID)
            p("HouseBuildingNo") = ReadyString(PersonDetails("PersonBldgNo"))
            p("Street") = ReadyString(PersonDetails("PersonStreetName"))
            p("CityID") = ReadyString(PersonDetails("PersonCity"))
            p("CountryID") = ReadyString(PersonDetails("PersonCountry"))
            p("AddressType") = ReadyString("Permanent Address")
            p("LastUpdated") = "getdate()"

            p("ID") = ReadyString(Guid.NewGuid.ToString())

            Common.WebRequest.ProData.Execute(p.GetInsert())

            p = New InsertUpdate("MobizLibraryPeopleUserAccount")

            p("PeopleID") = ReadyString(ppolID)
            p("Username") = ReadyString(PersonDetails("Username"))
            p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("Password")))
            Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

            p("PeopleID") = ReadyString(ppolID)
            p("SecurityQuestion") = ReadyString(PersonDetails("SecQ1"))
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("SecQ1Ans"))) 'ReadyString(SecurityQuestion1Answer)
            p("ID") = ReadyString(Guid.NewGuid.ToString())

            Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

            p("PeopleID") = ReadyString(ppolID)
            p("SecurityQuestion") = ReadyString(PersonDetails("SecQ2"))
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("SecQ2Ans"))) 'ReadyString(SecurityQuestion1Answer)
            p("ID") = ReadyString(Guid.NewGuid.ToString())

            Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

            If PersonDetails("AccountType") = "company" Then
                Dim orgid As String = Guid.NewGuid.ToString()
                orguserid = orgid

                p = New InsertUpdate("MobizLibraryOrganization")

                p("ID") = ReadyString(orgid)
                p("CompanyName") = ReadyString(PersonDetails("CompanyName"))
                p("TinNumber") = ReadyString(PersonDetails("Tin"))
                p("IndustryClassID") = ReadyString(PersonDetails("BusinessIndustry"))
                p("OfficeAddressBuildingNo") = ReadyString(PersonDetails("CompanyBldgNo"))
                p("OfficeAddressStreet") = ReadyString(PersonDetails("CompanyStreetName"))
                p("OfficeAddressCityID") = ReadyString(PersonDetails("CompanyCity"))
                p("OfficeAddressCountryID") = ReadyString(PersonDetails("CompanyCountry"))
                p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))
                p("IsActive") = 1
                p("IsOrganization") = 1

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationFowst") 'SAVE ADDRESS

                p("OrganizationID") = ReadyString(orgid)
                p("StreetNumber") = ReadyString(Trim(PersonDetails("CompanyBldgNo") & " " & PersonDetails("CompanyStreetName") & " " & PersonDetails("CompanyCityName") & " " & PersonDetails("CompanyCountryName")))
                p("IsActive") = 1
                p("ID") = ReadyString(Guid.NewGuid.ToString())

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationContactEmail")

                p("OrganizationID") = ReadyString(orgid)
                p("EmailAddress") = ReadyString(PersonDetails("CompanyEmailAdd").ToString())
                p("EmailType") = ReadyString("Work Email")
                p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationContactMobile")

                p("OrganizationID") = ReadyString(orgid)
                p("CountryID") = ReadyString("e783ed13-aa98-4cc1-918e-d49b05c0167e") 'default to philippines
                p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(PersonDetails("CompanyMobileNo"))("ID"))
                p("ContactNo") = ReadyString(PersonDetails("CompanyMobileNo").ToString())
                p("ContactType") = ReadyString("Work Mobile")
                p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

                Common.WebRequest.ProData.Execute(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationContactPerson")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("OrganizationID") = ReadyString(orgid)
                p("PeopleID") = ReadyString(ppolID)
                p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))
                p("IsUseForEcommerce") = 1
                p("Status") = ReadyString("Pending")
                p("StatusLastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))
                p("IsUseForPersonOrgAffiliates") = ReadyString(0)

                Common.WebRequest.ProData.Execute(p.GetInsert())
            Else
                orguserid = ppolID

                p = New InsertUpdate("MobizLibraryOrganization")

                p("ID") = ReadyString(ppolID)
                p("PersonFirstName") = ReadyString(PersonDetails("FirstName"))
                p("PersonMiddleName") = ReadyString(PersonDetails("MiddleName"))
                p("PersonSurname") = ReadyString(PersonDetails("LastName"))
                p("PersonSuffixName") = ReadyString(PersonDetails("GenSuffix"))
                p("PersonDateOfBirth") = ReadyString(CDate(PersonDetails("DateOfBirth")))
                p("IsOrganization") = "0"
                p("IsActive") = 1
                Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

                p = New InsertUpdate("MobizLibraryOrganizationFowst") 'isave lang pag PERSONAL ACCOUNT

                p("OrganizationID") = ReadyString(ppolID)
                p("StreetNumber") = ReadyString(Trim(PersonDetails("PersonBldgNo") & " " & PersonDetails("PersonStreetName") & " " & PersonDetails("PersonCityName") & " " & PersonDetails("PersonCountryName")))
                p("IsActive") = 1
                p("ID") = ReadyString(Guid.NewGuid.ToString())

                Common.WebRequest.ProData.Execute(p.GetInsert())
            End If

            If Not SendConfirmationLink(PersonDetails("PersonEmailAdd"), emaillinkid, PersonDetails("FirstName"), PersonDetails("IsCustomer")) Then
                Return "There was an error occured during sending confirmation link to your email. Please contact the mobiz technical team."
            End If

            Dim code As String = New OrganizationShort().GenerateCodeNumber(IIf(usertype = "customer", True, False))

            If usertype = "customer" Then
                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("OrganizationID") = ReadyString(orguserid)
                p("SalesRepID") = "NULL"
                p("CustomerCode") = ReadyString(code)
            Else
                p = New InsertUpdate("LibraryOrganizationSupplier")

                p("SupplierID") = ReadyString(orguserid)
                p("VendorCode") = ReadyString(code)
            End If

            p("ActivationDate") = "GetDate()"
            p("DeActivated") = 0
            p("IsBan") = ReadyString("0")
            p("LastUpdated") = "getdate()"
            p("ThirdPartyID") = "NULL"

            If usertype = "customer" Then
                pSql = p.GetUpdate(" OrganizationID = " & ReadyString(orguserid))
            Else
                pSql = p.GetUpdate(" SupplierID = " & ReadyString(orguserid))
            End If

            If Common.WebRequest.ProData.ExecuteSQL(pSql) = 0 Then
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SendConfirmationLink(ByVal email As String, ByVal emaillinkid As String, ByVal personname As String, ByVal IsCustomer As String) As Boolean
        Try
            Dim link As String = Url & "/Ecommerce/Confirm?id=" & emaillinkid &
                        "&c=" & GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper &
                        "&OrgID=" & Common.WebRequest.Data("OrgID") &
                        "&acctype=" & IIf(CBool2(IsCustomer), "cus", "sup") &
                        "&mainurl=" & Url

            Dim msg As String = "<table style='width:100%; border:none; background-color: #f5f5f5;'>" +
                                    "<tr><td style='height:100px; text-align:center; padding:20px; vertical-align:middle; background-color: #0083c1; font-size: 40px; color: #FFF;'>WELCOME</td></tr>" +
                                    "<tr><td style='padding:20px; font-size: 20px;'>Hello " + personname.ToUpper() + ",<br /><br />Thanks for registering! Before you can login, please click the button below to activate your account.<br /></td></tr>" +
                                    "<tr><td style='padding:20px; font-size: 20px; vertical-align:middle; text-align:center;'><a href='" & link & "' style='background-color: #5cb85c; border: none; border-radius: 5px; padding: 15px; color: #fff; text-decoration:none;'>ACTIVATE MY ACCOUNT</a><br /><br /></td></tr>" +
                                    "<tr><td style='padding:20px; vertical-align:middle; background-color: #dadada; font-size: 16px;'>To know more about Mobiz, please <a href='http://mobiz-apps.com' target='_blank' style='text-decoration:none;'>click here</a>.</td></tr>" +
                                "</table>"

            SendMail("mobizecommerce@outlook.com",
                     email,
                     "E-commerce",
                     "Welcome to Mobiz E-commerce!",
                     msg,
                     "smtp.live.com",
                     587,
                     "mobizecommerce@outlook.com",
                     "m0b!zp30pl30nly!", , , , , True)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function GetProductSearchMore(Optional ByVal from As Integer = 1, Optional ByVal tu As Integer = 20) As String
        Dim pSql As String
        Dim pDt As DataTable
        Dim pOut As New JsonDictionary()

        pSql = "SELECT * FROM ( " & vbCrLf &
                    "SELECT TOP 100 PERCENT grppim.ID, grppim.ShortName, pim.ProductName, pim.ID as ProductID, prc.Price, " & vbCrLf &
                    " (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = grppim.ProductID AND Size = 'big' AND IsDefault = 1  AND ISNULL(IsFromVendor, 0) = 0) as FileName, " & vbCrLf &
                    " grp.Name as Category, 'big' as FileSize, " & vbCrLf &
                    " ROW_NUMBER() OVER (ORDER BY grppim.OrderNo, pim.ProductName ASC) as RowNum " & vbCrLf &
                    " , um.UnitMeasure " & vbCrLf &
                    " FROM LibraryStoreCategoryGroupPIM as grppim " & vbCrLf &
                    " INNER JOIN LibraryStoreCategoryGroup as grp ON grp.ID = grppim.GroupID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = grppim.ProductID " & vbCrLf &
                    " LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = grppim.ProductID " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1" & vbCrLf &
                    "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                    "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                    "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                    "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                    "       WHERE MobizLibraryPIMProductMeasurement.ProductID = pim.ID AND MobizLibraryPIMProductMeasurement.Quantity = 1" & vbCrLf &
                    "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity ) as um " & vbCrLf &
                    " WHERE prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' -- retail price " & vbCrLf &
                    " AND prc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID.ToString()) & vbCrLf &
                    " AND grppim.GroupID = " & ReadyString(GroupID.ToString()) & vbCrLf &
                    " AND ISNULL(UnitMeasure, '') <> '' " & vbCrLf &
                    " ORDER BY grppim.OrderNo asc, Name asc " & vbCrLf &
                ") as t WHERE RowNum >= " & CInt2(from) & " AND RowNum <= " & CInt2(tu)

        pDt = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("MoreProducts", pDt)

        Return pOut.Serialize()

    End Function

    Function GetGroupList() As String
        Dim pOut As New JsonDictionary()

        Dim pSql As String = "SELECT * FROM LibraryStoreCategoryGroup WHERE ParentID = " & ReadyString(GroupID) & vbCrLf &
                                " ORDER BY OrderNo ASC "
        Dim pRows As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        If pRows.Length > 0 Then
            For Each row As Object In pRows
                'COMMENTED BY ARNIE 5-24-2020 3:40pm                '
                'pSql = "SELECT TOP 21 grppim.ID, grppim.ShortName, pim.ProductName, pim.ID as ProductID, prc.Price, " & vbCrLf &
                '                " (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = grppim.ProductID AND Size = 'big' AND IsDefault = 1) as FileName, " & vbCrLf &
                '                " grp.Name as Category, 'big' as FileSize " & vbCrLf &
                '                " , um.UnitMeasure " & vbCrLf &
                '                " FROM LibraryStoreCategoryGroupPIM as grppim " & vbCrLf &
                '                " INNER JOIN LibraryStoreCategoryGroup as grp ON grp.ID = grppim.GroupID " & vbCrLf &
                '                " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = grppim.ProductID " & vbCrLf &
                '                " LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = grppim.ProductID " & vbCrLf &
                '                " OUTER APPLY (SELECT DISTINCT TOP 1" & vbCrLf &
                '                "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                '                "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                '                "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                '                "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                '                "       WHERE MobizLibraryPIMProductMeasurement.ProductID = pim.ID AND MobizLibraryPIMProductMeasurement.Quantity = 1) as um " & vbCrLf &
                '                " WHERE prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' -- retail price " & vbCrLf &
                '                " AND prc.OrganizationID = " & ReadyString(Common.WebRequest.QueryStrings("OrgID").ToString()) & vbCrLf &
                '                " AND grppim.GroupID = " & ReadyString(row("ID").ToString()) & vbCrLf &
                '                " AND ISNULL(UnitMeasure, '') <> '' " & vbCrLf &
                '                " ORDER BY grppim.OrderNo "

                'row.Add("Products", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                row.Add("SubGroup", New OnlineStoreCategoriesController().GetSubGroup(row("MenuID").ToString(), row("ID").ToString(), 2, True, 20))
            Next row
        End If

        pOut.Add("SubGroup", pRows)

        pSql = "SELECT TOP 20 grppim.ID, grppim.ShortName, pim.ProductName, pim.ID as ProductID, prc.Price, " & vbCrLf &
                " (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = grppim.ProductID AND Size = 'big' AND IsDefault = 1 AND ISNULL(IsFromVendor, 0) = 0) as FileName, " & vbCrLf &
                " grp.Name as Category, 'big' as FileSize " & vbCrLf &
                " , um.UnitMeasure " & vbCrLf &
                " FROM LibraryStoreCategoryGroupPIM as grppim " & vbCrLf &
                " INNER JOIN LibraryStoreCategoryGroup as grp ON grp.ID = grppim.GroupID " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = grppim.ProductID " & vbCrLf &
                " LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = grppim.ProductID " & vbCrLf &
                " OUTER APPLY (SELECT TOP 1" & vbCrLf &
                "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "       WHERE MobizLibraryPIMProductMeasurement.ProductID = pim.ID AND MobizLibraryPIMProductMeasurement.Quantity = 1" & vbCrLf &
                "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity ) as um " & vbCrLf &
                " WHERE prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' -- retail price " & vbCrLf &
                " AND prc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID.ToString()) & vbCrLf &
                " AND grppim.GroupID = " & ReadyString(GroupID) & vbCrLf &
                " AND ISNULL(UnitMeasure, '') <> '' " & vbCrLf &
                " ORDER BY grppim.OrderNo "

        pOut.Add("MoreProducts", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        Return pOut.Serialize()
    End Function

    Function GetProductSearch(Optional ByVal from As Integer = 1, Optional ByVal tu As Integer = 20) As String
        Dim pOut As New JsonDictionary()

        Dim whereSearch As String = " WHERE pimpro.IsActive = 1 AND ISNULL(pimpro.ProductName, '') <> '' " & vbCrLf &
                                    " AND ISNULL(UnitMeasure, '') <> '' AND orgpim.IsDeactivated = '0' "
        Dim whereSearchContent As String = ""
        Dim operators As String = ""

        Dim arr As Object = Split(Keyword, " ")

        For i As Integer = 0 To arr.length - 1
            If whereSearchContent <> "" Then
                operators = " AND "
            End If

            whereSearchContent = whereSearchContent & operators & " ISNULL(pimpro.ProductGroupNo, '') +  " & vbCrLf &
                    "       ISNULL(pimpro.ProductNo, '') +  " & vbCrLf &
                    "       ISNULL( ISNULL(NULLIF(LTRIM(RTRIM(pimpro.Brand)), '') + ' ','') + pimpro.ProductName, '') +  " & vbCrLf &
                    "       ISNULL( ISNULL(pimpro.ProductSearchString,'') , '') " & vbCrLf &
                            " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
        Next

        If whereSearchContent <> "" Then
            whereSearch &= " AND (" & whereSearchContent & ")"
        End If

        Dim pSql As String = "SELECT * FROM ( " & vbCrLf &
                    "SELECT DISTINCT TOP 100 PERCENT pimpro.ID AS [ProductID] " & vbCrLf &
                    " , pimpro.ProductNo AS [ProductNo] " & vbCrLf &
                    " , pimpro.ProductName AS [ProductName] " & vbCrLf &
                    " , pimprolvl.Price AS [Price] " & vbCrLf &
                    " , (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = pimpro.ID AND Size = 'big' AND IsDefault = 1 AND ISNULL(IsFromVendor, 0) = 0) as FileName" & vbCrLf &
                    " , 'big' as FileSize " & vbCrLf &
                    " , ROW_NUMBER() OVER (ORDER BY pimpro.ProductName ASC) as RowNum " & vbCrLf &
                    " , um.UnitMeasure " & vbCrLf &
                    " FROM MobizLibraryPIMProduct pimpro " & vbCrLf &
                    " INNER JOIN LibraryOrganizationPIM orgpim ON (pimpro.ID = orgpim.ProductID) " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1" & vbCrLf &
                    "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                    "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                    "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                    "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                    "       WHERE MobizLibraryPIMProductMeasurement.ProductID = orgpim.ProductID AND MobizLibraryPIMProductMeasurement.Quantity = 1" & vbCrLf &
                    "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity ) as um " & vbCrLf &
                    "   OUTER APPLY ( " & vbCrLf &
                    "       SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price " & vbCrLf &
                    "           FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                    "           WHERE MobizLibraryPIMProductPriceLevel.ProductID = orgpim.ProductID " & vbCrLf &
                    "           AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0005-0000-0000-000000000000' " & vbCrLf &
                    "           ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC " & vbCrLf &
                    "   ) pimprolvl " & vbCrLf &
                    whereSearch & vbCrLf &
                ") as t1 WHERE RowNum >= " & CInt2(from) & " AND RowNum <= " & CInt2(tu) & " ORDER BY RowNum asc "
        pOut.Add("MoreProducts", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        Return pOut.Serialize()
    End Function

    Function GetProductDetails(Optional vInvoiceID As String = Nothing) As String
        Dim pOut As New JsonDictionary()
        Dim pSql As String = "SELECT * FROM LibraryPIMPictures WHERE Size = 'big' AND ISNULL(IsFromVendor, 0) = 0 AND ProductID = " & ReadyString(ProductID) &
                                " ORDER BY IsDefault DESC "
        Dim pDt As DataTable = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("Big", pDt)

        pSql = "SELECT * FROM LibraryPIMPictures WHERE Size = 'medium' AND ISNULL(IsFromVendor, 0) = 0 AND ProductID = " & ReadyString(ProductID) &
                " ORDER BY IsDefault DESC "
        pDt = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("Medium", pDt)

        pSql = "SELECT * FROM LibraryPIMPictures WHERE Size = 'small' AND ISNULL(IsFromVendor, 0) = 0 AND ProductID = " & ReadyString(ProductID) &
                " ORDER BY IsDefault DESC "
        pDt = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("Small", pDt)

        pSql = "SELECT QtyChoices, Alias " & vbCrLf &
                " FROM LibraryEcommerceProductQtyChoices as qty " & vbCrLf &
                " INNER JOIN LibraryEcommerceProductQtyChoicesCategories as cat ON cat.QtyChoiceGroupID = qty.GroupID " & vbCrLf &
                " WHERE cat.Category = (SELECT C0020 FROM MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
                "   WHERE ProductID = " & ReadyString(ProductID) & ") "
        pDt = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("QtyChoices", pDt)

        pSql = "SELECT Upcharge FROM MobizLibraryPIMProductParent WHERE ProductID = " & ReadyString(ProductID)
        pDt = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("Upcharge", pDt)

        pSql = "SELECT pim.ProductName, pim.ID as ProductID, prc.Price " & vbCrLf &
                " , pim.MarketingText " & vbCrLf &
                " , um.UnitMeasure " & vbCrLf &
                " , ISNULL(pim.IsEcommerceVendorAllowedToSell, 1) as IsEcommerceVendorAllowedToSell " & vbCrLf &
                " FROM MobizLibraryPIMProduct as pim " & vbCrLf &
                " --INNER JOIN LibraryStoreCategoryGroupPIM as grppim ON pim.ID = grppim.ProductID " & vbCrLf &
                " LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = pim.ID " & vbCrLf &
                " OUTER APPLY (SELECT TOP 1" & vbCrLf &
                "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "       WHERE MobizLibraryPIMProductMeasurement.ProductID = pim.ID AND MobizLibraryPIMProductMeasurement.Quantity = 1 " & vbCrLf &
                "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity) as um " & vbCrLf &
                " WHERE prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' -- retail price " & vbCrLf &
                " AND prc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " AND pim.ID = " & ReadyString(ProductID) & vbCrLf &
                " AND ISNULL(UnitMeasure, '') <> '' "
        pDt = Common.WebRequest.ProData.Execute(pSql)

        pOut.Add("Details", pDt)
        Dim IsEcommerceVendorAllowedToSell As Boolean = CBool2(pDt.Rows(0)("IsEcommerceVendorAllowedToSell"))

        If vInvoiceID IsNot Nothing Then
            Dim pDtItems = WebRequest.Instance.Business.TransInvoice.GetItems(vInvoiceID, False, ProductID)
            Dim pQty = CDec2(pDtItems.Compute("SUM(OrderedQuantity)", ""))
            pOut.Add("Qty", pQty)
        End If

        If CBool2(WebRequest.Instance.Business.SystemDefaults.EcommerceIsAllowBuyersSelectFromVendor) And IsEcommerceVendorAllowedToSell Then
            pSql = " SELECT vendor.VendorID, " & vbCrLf &
                    IIf(CBool2(WebRequest.Instance.Business.SystemDefaults.EcommerceShowVendorName), " CASE WHEN vendordetails.IsOrganization = 0 THEN PersonFirstName + ' ' + PersonMiddleName + ' ' + vendordetails.PersonSurname ELSE vendordetails.CompanyName END as VendorName, ", "") & vbCrLf &
                    " (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = sku.SkuID AND Size = 'medium' AND IsDefault = 1  AND ISNULL(IsFromVendor, 0) = 1 AND VendorID = vendor.VendorID) as FileName, " & vbCrLf &
                    " sup.VendorCode, " & vbCrLf &
                    " price.Price, " & vbCrLf &
                    " um.UnitMeasure " & vbCrLf &
                    " FROM LibraryPriceCanvassItems as sku " & vbCrLf &
                    " INNER JOIN LibraryPriceCanvassVendor as vendor ON vendor.CanvassID = sku.CanvassID " & vbCrLf &
                    IIf(CBool2(WebRequest.Instance.Business.SystemDefaults.EcommerceShowVendorName), " INNER JOIN MobizLibraryOrganization as vendordetails ON vendordetails.ID = vendor.VendorID ", "") & vbCrLf &
                    " INNER JOIN LibraryOrganizationSupplier as sup ON sup.SupplierID = vendor.VendorID " & vbCrLf &
                    " INNER JOIN LibraryVendorPrices as price ON price.VendorID = vendor.VendorID AND price.ProductID = sku.SkuID " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1" & vbCrLf &
                    "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                    "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                    "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                    "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                    "       WHERE MobizLibraryPIMProductMeasurement.ProductID = sku.SkuID AND MobizLibraryPIMProductMeasurement.Quantity = 1 " & vbCrLf &
                    "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity ) as um " & vbCrLf &
                    " WHERE sku.SkuID = " & ReadyString(ProductID) & vbCrLf &
                    " AND ISNULL(price.Price, 0) <> 0 "

            pDt = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("Vendor", pDt)
        End If

        Return pOut.Serialize()
    End Function

    Function GetCategories() As Object
        Dim pSql As String = "SELECT ID FROM LibraryStoreCategoryTemplate WHERE IsDefault = 1"
        Dim pRows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

        If pRows.Count > 0 Then
            Return New OnlineStoreCategoriesController().NewGetHotButtons(pRows(0)("ID").ToString(), False)
        Else
            Return Json(New With {Key .Results = New List(Of Dictionary(Of String, Object))})
        End If
    End Function

    Function GetCategoriesProducts() As DataTable
        Dim pSql As String = "SELECT " & vbCrLf &
                            " grpim.ID, " & vbCrLf &
                            " grpim.ShortName, " & vbCrLf &
                            " prc.Price, " & vbCrLf &
                            " pic.FileName, " & vbCrLf &
                            " pic.Size as FileSize, " & vbCrLf &
                            " grpim.ProductID " & vbCrLf &
                            " FROM LibraryStoreCategoryGroupPIM as grpim " & vbCrLf &
                            " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = grpim.ProductID " & vbCrLf &
                            " LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = grpim.ProductID " & vbCrLf &
                            " LEFT JOIN LibraryPIMPictures as pic ON pic.ProductID = grpim.ProductID AND pic.Size = 'big' AND pic.IsDefault = 1  AND ISNULL(pic.IsFromVendor, 0) = 0 " & vbCrLf &
                            " WHERE grpim.GroupID = " & ReadyString(CategoryID) & vbCrLf &
                            " AND prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' --retail price " & vbCrLf &
                            " AND prc.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID").ToString()) & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetAllProducts() As DataTable
        Dim pSql As String = "SELECT * FROM (" & vbCrLf &
                                "SELECT grppim.ID, grppim.ShortName, pim.ProductName, pim.ID as ProductID, prc.Price, " & vbCrLf &
                                " (SELECT TOP 1 FileName FROM LibraryPIMPictures WHERE ProductID = grppim.ProductID AND Size = 'big' AND IsDefault = 1  AND ISNULL(IsFromVendor, 0) = 0) as FileName, " & vbCrLf &
                                " grp.Name as Category, 'big' as FileSize, " & vbCrLf &
                                " ROW_NUMBER() OVER (ORDER BY grppim.OrderNo ASC) as RowNum " & vbCrLf &
                                " , um.UnitMeasure " & vbCrLf &
                                " FROM LibraryStoreCategoryGroupPIM as grppim " & vbCrLf &
                                " LEFT JOIN LibraryStoreCategoryGroup as grp ON grp.ID = grppim.GroupID " & vbCrLf &
                                " LEFT JOIN MobizLibraryPIMProduct as pim ON pim.ID = grppim.ProductID " & vbCrLf &
                                " LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = grppim.ProductID " & vbCrLf &
                                " OUTER APPLY (SELECT TOP 1" & vbCrLf &
                                "       MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                                "       FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                                "       JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                                "       JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                                "       WHERE MobizLibraryPIMProductMeasurement.ProductID = pim.ID AND MobizLibraryPIMProductMeasurement.Quantity = 1 " & vbCrLf &
                                "       ORDER BY MobizLibraryPIMProductMeasurement.Quantity ) as um " & vbCrLf &
                                " WHERE prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' -- retail price " & vbCrLf &
                                " AND prc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                                " AND (grppim.GroupID IN (SELECT ID FROM LibraryStoreCategoryGroup WHERE MenuID = (SELECT ID FROM LibraryStoreCategoryTemplate WHERE IsDefault = 1)) " & vbCrLf &
                                " OR grppim.MenuID = (SELECT ID FROM LibraryStoreCategoryTemplate WHERE IsDefault = 1)) " & vbCrLf &
                                " AND ISNULL(UnitMeasure, '') <> '' " & vbCrLf &
                            ") as p WHERE RowNum BETWEEN " & min & " AND " & max

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

End Class