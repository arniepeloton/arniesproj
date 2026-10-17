Namespace MobizApplication
    Public Class ConfirmedController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Confirmed

        Function Index() As ActionResult
            'CreateCsv()
            'DBProvisioningSuccess()
            'GetDBProvisioningResults()
            'SaveConfirmationDetails()
            Return View()
        End Function

        <HttpPost()>
        Function CheckConfirmedEmail() As String
            Dim clsSubscribe As New Subscribe
            Dim strReturn As String

            clsSubscribe.EmailActivationLinkID = Common.WebRequest.Data("ActivationLinkID")

            strReturn = JsonDictionary.Serialize("EmailData", clsSubscribe.CheckConfirmedEmail())

            Return strReturn

        End Function

        Function GetSubscriberName(ByVal id As String, ByVal type As String) As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim pSql2 As String
            Dim pDT2 As DataTable

            Try
                If type = "org" Then
                    pSql = "SELECT SuperAdminID FROM TransSubscription WHERE OrganizationID = " & ReadyString(id)

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If (pDT.Rows.Count > 0) Then
                        pSql2 = "SELECT FirstName FROM LibraryPeople WHERE ID = " & ReadyString(pDT.Rows(0).Item("SuperAdminID"))

                        pDT2 = Common.WebRequest.ProData.Execute(pSql2)


                        If (pDT2.Rows.Count > 0) Then
                            Return pDT2.Rows(0).Item("FirstName")
                        Else
                            Return "No data found"
                        End If
                    Else
                        Return "No data found"
                    End If
                Else
                    pSql = "SELECT PeopleID FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(id)

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If (pDT.Rows.Count > 0) Then
                        pSql = "SELECT FirstName, MiddleName, Surname FROM LibraryPeople WHERE ID = " & ReadyString(pDT.Rows(0).Item("PeopleID"))

                        pDT = Common.WebRequest.ProData.Execute(pSql)


                        If (pDT.Rows.Count > 0) Then
                            Return pDT.Rows(0).Item("Surname") & ", " & pDT.Rows(0).Item("FirstName") & " " & pDT.Rows(0).Item("MiddleName")
                        Else
                            Return "No data found"
                        End If
                    Else
                        Return "No data found"
                    End If
                End If


            Catch ex As Exception
                Return "No data found"
            End Try
        End Function

        Function GetSubscriberEmail(ByVal id As String, ByVal type As String) As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim pSql2 As String
            Dim pDT2 As DataTable

            Try
                If type = "org" Then
                    pSql = "SELECT SuperAdminID FROM TransSubscription WHERE OrganizationID = " & ReadyString(id)

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If (pDT.Rows.Count > 0) Then
                        pSql2 = "SELECT EmailAddress FROM LibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(pDT.Rows(0).Item("SuperAdminID"))

                        pDT2 = Common.WebRequest.ProData.Execute(pSql2)

                        If (pDT2.Rows.Count > 0) Then

                            Return pDT2.Rows(0).Item("EmailAddress")

                        Else
                            Return "No data found"
                        End If
               
                    Else
                        Return "No data found"
                    End If



                Else
                    pSql = "SELECT EmailAddress FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(id)

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If (pDT.Rows.Count > 0) Then

                        Return pDT.Rows(0).Item("EmailAddress")

                    Else
                        Return "No data found"
                    End If
                End If


            Catch ex As Exception
                Return "No data found"
            End Try
        End Function

        Function GetSubscriberCompany(ByVal id As String, ByVal type As String) As String
            Dim pSql As String
            Dim pDT As DataTable

            Try
                If type = "org" Then
                    pSql = "SELECT CompanyName FROM LibraryOrganization WHERE ID = " & ReadyString(id)

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If (pDT.Rows.Count > 0) Then
                        Return pDT.Rows(0).Item("CompanyName")
                    Else
                        Return "No data found"
                    End If


                Else
                    pSql = "SELECT PeopleID FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(id)

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If (pDT.Rows.Count > 0) Then
                        pSql = "SELECT TOP 1 OrganizationID FROM LibraryPeopleOrganization WHERE PeopleID = " & ReadyString(pDT.Rows(0).Item("PeopleID")) & " ORDER BY DateTimeInvited desc"

                        pDT = Common.WebRequest.ProData.Execute(pSql)
                        If (pDT.Rows.Count > 0) Then
                            pSql = "SELECT CompanyName FROM LibraryOrganization WHERE ID = " & ReadyString(pDT.Rows(0).Item("OrganizationID"))

                            pDT = Common.WebRequest.ProData.Execute(pSql)


                            If (pDT.Rows.Count > 0) Then
                                Return pDT.Rows(0).Item("CompanyName")
                            Else
                                Return "No data found"
                            End If

                        Else
                            Return "No data found"
                        End If
                    Else
                        Return "No data found"
                    End If

                End If
                
            Catch ex As Exception
                Return "No data found"
            End Try

        End Function

        'for the free version of mobiz complete retail, this is the trigger that the details should be sent to our server
        'if not free, save only if the user has paid the subscription fee
        <HttpPost()>
        Function SaveConfirmationDetails() As String

            Dim strReturn As String

            If (Common.WebRequest.Data("ConfirmationCode") <> "") Then

                If EmailConfirmationSuccess(Common.WebRequest.Data("ConfirmationCode")) = "True" Then
                    '''''''
                    'add CODE to contact mobiz server
                    If CreateCsv() = "ok" Then
                        'trigger mobiz server code to get my zip files
                        strReturn = JsonDictionary.Serialize("ConfirmData", "Ok")
                    Else
                        strReturn = JsonDictionary.Serialize("Error", "No Data")
                    End If
                    '''''''''''''''

                Else
                    strReturn = JsonDictionary.Serialize("Error", "No Data")
                End If
            Else
                strReturn = JsonDictionary.Serialize("Error", "No Data")
            End If

            Return strReturn

        End Function


        Function EmailConfirmationSuccess(ByVal code As String) As String
            Dim name As String
            Dim org As String
            Dim email As String
            Dim clsSubscribe As New Subscribe
            Dim strReturn As String

            Try
                AppendToLog("ContactEmailID")
                clsSubscribe.ContactEmailID = Common.WebRequest.Data("ContactEmailID")

                clsSubscribe.ConfirmationCode = code

                strReturn = clsSubscribe.SaveConfirmationDetails()
                Return True
                'AppendToLog("ContactEmailID " & strReturn)
                'name = GetSubscriberName(clsSubscribe.ContactEmailID, "")
                'AppendToLog("ContactEmailID " & name)
                'org = GetSubscriberCompany(clsSubscribe.ContactEmailID, "")
                'AppendToLog("ContactEmailID " & org)
                'email = GetSubscriberEmail(clsSubscribe.ContactEmailID, "")
                'AppendToLog("SaveConfirmationDetails " & email)

                'send email to admin for notification
                'Dim strBody As String = EmailContent(Common.WebRequest.Data("ContactEmailID"), True)
                
                'ERROR MESSAGE: smtp server requires a secure connection or the client was not authenticated 5.5.1
                'commented by arnie 7-24-19
                'dapat ang mobiz na mag-activate sa new customer para magcreate ug DB
                '#If DEBUG Then
                '            SendMail("mobizcompleteretail@outlook.com",
                '                 "arniepeloton17@gmail.com",
                '                 "Mobiz",
                '                 "New Mobiz Customer",
                '                 strBody,
                '                 "smtp.live.com",
                '                 587,
                '                 "mobizcompleteretail@outlook.com",
                '                 "m0b!zp30pl30nly!", , , , , True)


                '#Else
                '                SendMail("mobizcompleteretail@outlook.com",
                '                    "arniepeloton17@gmail.com, robert.chong@mobiz-apps.com",
                '                    "Mobiz",
                '                    "New Mobiz Customer",
                '                    strBody,
                '                    "smtp.live.com",
                '                    587,
                '                    "mobizcompleteretail@outlook.com",
                '                    "m0b!zp30pl30nly!", , , , , True)


                '#End If
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try
            

            Return strReturn
        End Function

        Function EmailContent(ByVal emailid As String, ByVal justBody As Boolean)
            Dim clsPeople As New Subscribe
            Dim strBody As String

            'strBody = "We have a new customer! <br/> Name: " & name & " <br/> Email Address: " & email & "<br/> Company: " & org & "<br/><br/> Please transfer data to Mobiz server."

            Dim pSql As String = "SELECT * FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(emailid)
            Dim pDTEmail As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)
            Dim peopleID As String = pDTEmail("PeopleID").ToString()

            pSql = "SELECT * FROM LibraryPeople WHERE ID = " & ReadyString(peopleID)
            Dim pDTPerson As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

            pSql = "SELECT mobile.*, telco.Prefix FROM LibraryPeopleContactMobile as mobile" & vbCrLf &
                    " INNER JOIN LibraryTelcoPrefixes as telco ON mobile.TelcoPrefixID = telco.ID" & vbCrLf &
                    " WHERE PeopleID = " & ReadyString(peopleID)
            Dim pDTMobile As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

            pSql = "SELECT TOP 1 OrganizationID FROM LibraryPeopleOrganization WHERE PeopleID = " & ReadyString(peopleID) & " ORDER BY DateTimeInvited desc"
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            pSql = "SELECT org.*, offcity.Name as offcity, offcity.Zip as offcityzip, offcountry.Name as OffCountry, buscity.Name as buscity, buscity.Zip as buscityZip, buscountry.Name as BusCountry FROM LibraryOrganization as org " & vbCrLf &
                    " INNER JOIN LibraryCities as offcity on org.OfficeAddressCityID = offcity.ID " & vbCrLf &
                    " INNER JOIN LibraryCountries as offcountry on org.OfficeAddressCountryID = offcountry.ID " & vbCrLf &
                    " INNER JOIN LibraryCities as buscity on org.BusinessAddressCityID = buscity.ID " & vbCrLf &
                    " INNER JOIN LibraryCountries as buscountry on org.BusinessAddressCountryID = buscountry.ID " & vbCrLf &
                    " WHERE org.ID = " & ReadyString(pDT.Rows(0).Item("OrganizationID"))
            Dim pDTCompany As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

            pSql = "SELECT ind.Class FROM LibraryOrganizationIndustryClass as orgind INNER JOIN LibraryIndustryClass as ind ON orgind.IndustryClassID = ind.ID WHERE OrganizationID = " & ReadyString(pDT.Rows(0).Item("OrganizationID"))
            Dim pDTBusInd As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

            pSql = "SELECT * FROM LibraryOrganizationTradeName WHERE OrganizationID = " & ReadyString(pDT.Rows(0).Item("OrganizationID"))
            Dim pDTBusTrade As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

            Dim IDType As String = ""
            Dim IDNo As String = ""

            If CStr2(pDTPerson("SSSNo")) <> "" Then
                IDType = "SSS"
                IDNo = pDTPerson("SSSNo")
            ElseIf CStr2(pDTPerson("PhilHealthNo")) <> "" Then
                IDType = "PhilHealth"
                IDNo = pDTPerson("PhilHealthNo")
            ElseIf CStr2(pDTPerson("TIN")) <> "" Then
                IDType = "TIN"
                IDNo = pDTPerson("TIN")
            ElseIf CStr2(pDTPerson("HDMFNo")) <> "" Then
                IDType = "HDMF"
                IDNo = pDTPerson("HDMFNo")
            ElseIf CStr2(pDTPerson("NBINo")) <> "" Then
                IDType = "NBI"
                IDNo = pDTPerson("NBINo")
            ElseIf CStr2(pDTPerson("PassportNo")) <> "" Then
                IDType = "Passport"
                IDNo = pDTPerson("PassportNo")
            ElseIf CStr2(pDTPerson("DriversLicenseNo")) <> "" Then
                IDType = "Driver's License"
                IDNo = pDTPerson("DriversLicenseNo")
            End If

            If justBody Then
                strBody = "<table border='0' cellpadding='0' cellspacing='0' style='font-family: monospace; max-width:600px;'>" +
                        "<tbody>" +
                            "<tr><td colspan='2' style='color:#fff; background-color:#4caf50; font-size:20px; padding:20px;'>We have a new customer!</td></tr>" +
                            "<tr><td colspan='2' style='color:#fff; font-size:20px; background-color:#28435d; padding:20px;'>Company Profile</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Corporate Name:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("CompanyName") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Corporate Name Initials:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("CompanyNameInitial") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business or Trade Name:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTBusTrade("BusinessTradeName") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business or Trade Name Initials:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTBusTrade("BusinessTradeNameInitial") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Ownership Type:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("OwnershipType") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Are you a franchisor?</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + IIf(CBool2(pDTCompany("IsFranchisor")) = False, "No", "Yes") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Registration Type:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("RegistrationType") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Registration Number:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("RegistrationNo") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Registration Date:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + Format(pDTCompany("RegistrationDate"), "MMMM d, yyyy") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Corporate TIN:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("TinNumber") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business Industry:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTBusInd("Class") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Office Address:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("OfficeAddressBuildingNo") & ", " & pDTCompany("OfficeAddressStreet") & ", " & pDTCompany("offcity") & " " & pDTCompany("offcityzip") & ", " & pDTCompany("OffCountry") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Business Address:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTCompany("BusinessAddressBuildingNo") & ", " & pDTCompany("BusinessAddressStreet") & ", " & pDTCompany("buscity") & " " & pDTCompany("buscityzip") & ", " & pDTCompany("BusCountry") + "</td></tr>" +
                            "<tr><td colspan='2' style='color:#fff; font-size:20px; background-color:#2f608e; padding:20px;'>Administrator Profile</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Fullname:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTPerson("FirstName") & " " & pDTPerson("MiddleName") & " " & pDTPerson("Surname") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Generational Suffix:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTPerson("PersonalSuffixName") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Nickname:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTPerson("NickName") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Gender:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTPerson("Gender") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Date of Birth:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + Format(pDTPerson("DateOfBirth"), "MMMM d, yyyy") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Mother's Maiden Name:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTPerson("MotherMaidenFirstName") & " " & pDTPerson("MotherMaidenMiddleName") & " " & pDTPerson("MotherMaidenSurname") + "</td></tr>" +
                            "<tr><td colspan='2' style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c; text-align:center;'>Contact Information</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Mobile Number:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>+63." + pDTMobile("Prefix") & "." & pDTMobile("ContactNo") + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Email Address:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + pDTEmail("EmailAddress") + "</td></tr>" +
                            "<tr><td colspan='2' style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c; text-align:center;'>Primary ID Reference</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>Primary ID Type:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + IDType + "</td></tr>" +
                            "<tr><td style='font-size:16px; padding:5px;border-bottom: 1px solid #ccc; color:#8c8c8c;'>" + IDType + " ID:</td><td style='font-size:18px; border-bottom: 1px solid #ccc; padding:10px;'>" + IDNo + "</td></tr>" +
                        "</tbody>" +
                    "</table>"

                Return strBody
            Else
                Dim pOut As New JsonDictionary

                pOut.Add("CorporateName", pDTCompany("CompanyName"))
                pOut.Add("CorporateNameInitial", pDTCompany("CompanyNameInitial"))
                pOut.Add("BusinessName", pDTBusTrade("BusinessTradeName"))
                pOut.Add("BusinessTradeNameInitial", pDTBusTrade("BusinessTradeNameInitial"))
                pOut.Add("Ownership", pDTCompany("OwnershipType"))
                pOut.Add("IsFranchisor", IIf(CBool2(pDTCompany("IsFranchisor")) = False, "No", "Yes"))
                pOut.Add("RegistrationType", pDTCompany("RegistrationType"))
                pOut.Add("RegistrationNo", pDTCompany("RegistrationNo"))
                pOut.Add("RegistrationDate", Format(pDTCompany("RegistrationDate"), "MMMM d, yyyy"))
                pOut.Add("TinNumber", pDTCompany("TinNumber"))
                pOut.Add("BusinessIndustry", pDTBusInd("Class"))
                pOut.Add("OfficeAddress", pDTCompany("OfficeAddressBuildingNo") & ", " & pDTCompany("OfficeAddressStreet") & ", " & pDTCompany("offcity") & " " & pDTCompany("offcityzip") & ", " & pDTCompany("OffCountry"))
                pOut.Add("BusinessAddress", pDTCompany("BusinessAddressBuildingNo") & ", " & pDTCompany("BusinessAddressStreet") & ", " & pDTCompany("buscity") & " " & pDTCompany("buscityzip") & ", " & pDTCompany("BusCountry"))
                pOut.Add("AdministratorName", pDTPerson("FirstName") & " " & pDTPerson("MiddleName") & " " & pDTPerson("Surname"))
                pOut.Add("PersonalSuffixName", pDTPerson("PersonalSuffixName"))
                pOut.Add("Nickname", pDTPerson("NickName"))
                pOut.Add("Gender", pDTPerson("Gender"))
                pOut.Add("DateOfBirth", Format(pDTPerson("DateOfBirth"), "MMMM d, yyyy"))
                pOut.Add("MothersName", pDTPerson("MotherMaidenFirstName") & " " & pDTPerson("MotherMaidenMiddleName") & " " & pDTPerson("MotherMaidenSurname"))
                pOut.Add("MobileNumber", pDTMobile("Prefix") & "." & pDTMobile("ContactNo"))
                pOut.Add("EmailAddress", pDTEmail("EmailAddress"))
                pOut.Add("IDType", IDType)
                pOut.Add("IDNo", IDNo)
                Return pOut.Serialize()
            End If
        End Function

        'this function creates the csv files . condition: if status is confirmed
        Public Function CreateCsv() As String

            Dim pDT As DataTable
            Dim subscribe As New Subscribe
            Dim pZip As ProZip
            Dim pZipFilename As String

            pZipFilename = Common.ServerPath & "temp\DBaseProvisioning\DBaseProvisioning.zip"
            If IO.File.Exists(pZipFilename) Then
                IO.File.Delete(pZipFilename)
            End If
            CreateFolder(pZipFilename)
            pZip = New ProZip(pZipFilename)
            pZip.CsvEncoderMaxLength = 10000 ' example lang ni

            pDT = subscribe.GetOrganizationInfo()
            pZip.AddTableCsv(pDT, "LibraryOrganization")

            pDT = subscribe.GetOrgTradeNameInfo()
            pZip.AddTableCsv(pDT, "LibraryOrganizationTradeName")

            pDT = subscribe.GetOrgIndustryClassInfo()
            pZip.AddTableCsv(pDT, "LibraryOrganizationIndustryClass")

            pDT = subscribe.GetPeopleInfo()
            pZip.AddTableCsv(pDT, "LibraryPeople")

            pDT = subscribe.GetPeopleUserAccountInfo()
            pZip.AddTableCsv(pDT, "LibraryPeopleUserAccount")

            pDT = subscribe.GetPeopleSecurityQuestionInfo()
            pZip.AddTableCsv(pDT, "LibraryPeopleSecurityQuestion")

            pDT = subscribe.GetPeopleContactEmailInfo()
            pZip.AddTableCsv(pDT, "LibraryPeopleContactEmail")

            pDT = subscribe.GetPeopleContactMobileInfo()
            pZip.AddTableCsv(pDT, "LibraryPeopleContactMobile")

            pDT = subscribe.GetPeopleOrganizationInfo()
            pZip.AddTableCsv(pDT, "LibraryPeopleOrganization")

            pDT = subscribe.GetSubscriberList
            pZip.AddTableCsv(pDT, "TransSubscription")

            pDT = subscribe.GetSubscriptionCartList()
            pZip.AddTableCsv(pDT, "TransSubscriptionCart")


            Return "ok"

        End Function
        'this is the function that the mobiz server will call to get my zip files
        Public Function GetSubscribers() As ActionResult

            Dim pFile As IO.FileInfo
            Dim pFilename As String

            If CreateCsv() = "ok" Then
                pFilename = Common.ServerPath & "\temp\DBaseProvisioning\DBaseProvisioning.zip"
                pFile = New IO.FileInfo(pFilename)
                Try
                    If pFile.Exists Then
                        Response.AddHeader("FileDate", pFile.LastWriteTime.ToString("s", Globalization.CultureInfo.InvariantCulture))
                        Response.AddHeader("FileSize", pFile.Length)
                        Response.ContentType = "application/" & IO.Path.GetExtension(pFilename)
                        Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(pFilename, True))
                        Response.TransmitFile(pFilename)
                    Else
                        Throw New IO.FileNotFoundException
                    End If
                Catch ex As Exception
                    Stop2()
                    Throw
                End Try
            End If


        End Function

        ' this is the function that gets the result of db provisioning from the mobiz server
        Public Function DBProvisioningResult() As ActionResult

            Dim clsSubscribe As New Subscribe

            Dim file As HttpPostedFileBase
            Try
                'deal with the uploaded file
                If (Request.Files.Count > 0) Then
                    file = Request.Files(0)

                    If (Not file Is Nothing And file.ContentLength > 0) Then
                        file.SaveAs(Common.ServerPath & "temp\DBaseProvisioning\DBaseProvisioningResult.zip")

                        GetDBProvisioningResults()

                    End If
                End If
            Catch ex As Exception
                Stop2()
                Throw
            End Try

        End Function

        'get org id and status from mobiz server. then update database to success or fail status
        Sub GetDBProvisioningResults()

            Dim pFilename As String
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pValues As String
            Dim pRowIndex As Integer
            Dim clsSubscribe As New Subscribe

            pFilename = Common.ServerPath & "temp\DBaseProvisioning\DBaseProvisioningResult.zip"
            pCsvReader = New CsvReader(pFilename, "DBaseProvisioningResult")
            pValues = ""
            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do

                If (clsSubscribe.UpdateTransferStatus(pLine("OrganizationID"), pLine("Status")) = "ok") Then
                    EmailCustomer(pLine("OrganizationID"), pLine("Status"))
                End If

                pRowIndex += 1
            Loop

        End Sub

        'email customer dpending on status
        Sub EmailCustomer(ByVal orgid As String, ByVal status As String)
            Dim name As String
            Dim org As String
            Dim email As String
            Dim clsSubscribe As New Subscribe


            name = GetSubscriberName(orgid, "org")
            org = GetSubscriberCompany(orgid, "org")
            email = GetSubscriberEmail(orgid, "org")
            'name = "[SAMPLE ONLY]"

            Dim strBody As String
            Dim clsPeople As New Subscribe
            Dim subject As String

            If status = "Success" Then

                strBody = "<div style='font-size: 16; font-family: Arial;'><p>Hello " & name & ",</p>" &
                   "<p>Congratulations! Your organization’s own private database has been created successfully. <br/>" &
                   "Let’s take a few simple steps before we start using Mobiz for your business: </p>" &
                   "<h3>Step 1: SETTING UP YOUR DATA</h3>" &
                   "<p>We'll walk you through the data setup process using the setup wizard.</p>" &
                   "<p>1. Using your internet browser, go to www.mobiz-apps.com.  Please click on the 'Sign In' link located at the upper right-hand corner of the Mobiz webpage.</p>" &
                   "<p>2. You will be directed to the Sign-In Page. Please sign in using the username and password that you have provided during the subscription process.</p>" &
                   "<p>3. You will be directed to the main page.  Scroll to the bottom of the main page and click on 'System Administration' and then click again on 'Setup Checklist'.</p>" &
                   "<p>4. You will see a list of the data libraries that need to be set up.  Please click on each of item on the list and fill the up the respective forms. Note that we have attached an Excel file of the Product Information template for your to fill up.  Please email this Product Information to us when you’re done filling up.</p>" &
                   "<p>5. Once done, you are now ready to sync your Android device to the Mobiz Cloud and start using your Mobiz Complete Retail for your business.</p><br/>" &
                   "<p>Please note that our setup servers are available from 8:30 AM until 9:35 PM everyday including Sundays.</p><br/>" &
                   "<h3>Step 2: DOWNLOAD THE MOBIZ APP ON YOUR ANDROID DEVICE</h3>" &
                   "<p>After completing the setup wizard on your browser, please do the following:</p>" &
                   "<p>1. On your mobile device's browser, open this link: <a href=""http://122.52.123.251/androidjstest/downloadapk"">Mobiz APK</a> and wait for the Mobiz APK file to download successfully.</p>" &
                   "<p>2. On your device's file explorer, click on the Mobiz APK file to install Mobiz Complete Retail on your device.</p><br/>" &
                   "<p>Note that your device must be able to download from non-Google Play sources. You can do this by clicking on the check box labelled 'Unknown sources' located on the security settings of your device.</p><br/>" &
                   "<h3>Step 3: SYNC YOUR ANDROID DEVICE TO THE MOBIZ CLOUD TO START TRANSACTING</h3>" &
                   "<p>Sign in to Mobiz Complete Retail app to synchronize your device(s).</p>" &
                   "<p>1. On your device, open the Mobiz Complete Retail app and sign in.</p>" &
                   "<p>2. You will be directed to the main page.</p>" &
                   "<p>3. On the modules list, click on 'Troubleshooting Tools' and then click on 'Synchronize Data'. Your device will pull data references from the Mobiz Cloud. Please make sure that you have an active and stable internet connection during this step.</p>" &
                   "<p>4. After your device pulls in the database, it will then perform a battery of automated diagnostics to see if your database and device are working properly.</p>" &
                   "<p>5. Once diagnostics have completed, you will need to sign in again. Please do so.</p>" &
                   "<p>You're now ready to use Mobiz Complete Retail for your business!<br/>" &
                   "Any questions? Please contact <a href=""http://mobiz-apps.com/support/contactus"">Mobiz Support</a>. </p><br/>" &
                   "<p>All the best, <br/>" &
                   "The Mobiz Team<br/>" &
                   "Business-Grade Software in Your Pocket<br/>" &
                   "<a href=""http://mobiz-apps.com"">www.mobiz-apps.com</a> </p></div>"

                ' ' old message
                ' strBody = "<div style='font-size: 16; font-family: Arial;'><p>Hello " & name & ",</p>" &
                '"<p>Congratulations! Your organization’s own private database has been created successfully. <br/>" &
                '"Let’s take a few simple steps before we start using Mobiz for your business: </p>" &
                '"<h3>Step 1:</h3>" &
                '"<p>Go to <a href=""http://mobiz-apps.com/"">www.mobiz-apps.com</a> to sign in. We’ll walk you through the settings and data library wizard so that you can set up your organization’s important information.</p>" &
                '"<h3>Step 2: </h3>" &
                '"<p>After completing the setup wizard, download Mobiz Complete Retail app from the <a href=""https://play.google.com/store/apps/details?id=mobiz.mobiz"">Google Play Store</a>.</p>" &
                '"<h3>Step 3:</h3>" &
                '"<p>Sign in to Mobiz Complete Retail app to synchronize your device(s).</p><br/>" &
                '"<p>After completing these 3 simple steps, you are now good to go!<br/>" &
                '"Any questions? Please contact <a href=""http://mobiz-apps.com/support/contactus"">Mobiz Support</a>. </p><br/>" &
                '"<p>All the best, <br/>" &
                '"The Mobiz Team<br/>" &
                '"Business-Grade Software in Your Pocket<br/>" &
                '"<a href=""http://mobiz-apps.com"">www.mobiz-apps.com</a> </p></div>"

                subject = "Setting Up Your Mobiz Account"
            Else

                strBody = "<div style='font-size: 16; font-family: Arial;'><p>Hello " & name & ",</p>" &
               "<p>Upon checking your organization details, we found out that the company " & org & " has an existing account with Mobiz. <br/>" &
               "We will contact your super administrator as soon as possible to send you a request for employment so you can start using Mobiz. </p><br/>" &
               "Any questions? Please contact <a href=""http://mobiz-apps.com/support/contactus"">Mobiz Support</a>. </p><br/>" &
               "<p>All the best, <br/>" &
               "The Mobiz Team<br/>" &
               "Business-Grade Software in Your Pocket<br/>" &
               "<a href=""http://mobiz-apps.com"">www.mobiz-apps.com</a> </p></div>"

                subject = "Existing Organization"
            End If



            '"janine.son@mobiz-apps.com, robert.chong@mobiz-apps.com, arvin.meralpis@mobiz-apps.com, francis.hernandez@mobiz-apps.com, francis.adan@mobiz-apps.com"


            SendMail("mobizcompleteretail@outlook.com",
                 email,
                 "Mobiz",
                 subject,
                 strBody,
                 "smtp.live.com",
                 587,
                 "mobizcompleteretail@outlook.com",
                 "m0b!zp30pl30nly!", , , , , True)



        End Sub


    End Class
End Namespace