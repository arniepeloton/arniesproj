Public Class Konstruksyon
    Inherits System.Web.Mvc.Controller

    Public Property ClientID As String
    Public Property PersonDetails As Object
    Public Property Url As String
    Public Property EmailLinkID As String
    Public Property ConfirmationCode As String
    Public Property OID As String
    Public Property QuestionaireID As String

    Function AddUserToKonstruksyon(ByVal PeopleID As String)
        Try
            Dim pSql As String
            Dim employeeID As String = Guid.NewGuid.ToString()
            Dim p As New InsertUpdate("LibraryOrganizationEmployee")
            Dim row As Object

            p("ID") = ReadyString(employeeID)
            p("PeopleID") = ReadyString(PeopleID)
            p("DateHired") = "getdate()"
            p("DeActivated") = ReadyString("0")
            p("IsBan") = ReadyString("0")
            p("LastUpdated") = "GetDate()"

            pSql = p.GetInsert()
            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("MobizLibraryEmployeeOrganizationRelationship")

            p("IsDirectHired") = ReadyString("1")
            p("IsSelfEmployed") = ReadyString("0")
            p("EmployeeID") = ReadyString(employeeID)
            p("LastUpdated") = "getdate()"

            p("DirectEmployerID") = ReadyString(WebRequest.Instance.OrgID)
            p("ManpowerAgencyClientID") = "NULL"
            p("AssignedThirdPartyClientID") = "NULL"

            p("DirectEmployerIDDescription") = ReadyString("dbid")
            p("ManpowerAgencyClientIDDescription") = ReadyString("")
            p("AssignedThirdPartyClientIDDescription") = ReadyString("")

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "SELECT TOP 1 ID FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)

            p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

            p("PeopleID") = ReadyString(PeopleID)
            p("FowstID") = ReadyString(Common.WebRequest.ProData.Execute(pSql).Rows(0)("ID").ToString())
            p("LastUpdated") = "GetDate()"
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("LibrarySecurityRoleUser")

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("PeopleID") = ReadyString(PeopleID)
            p("RoleID") = ReadyString("183b056c-c917-4510-88eb-8012bd4854f3")
            p("LastUpdated") = "GetDate()"
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("MobizLibraryPeopleOrganization")

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("PeopleID") = ReadyString(PeopleID)
            p("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
            p("IsTerminated") = ReadyString("0")
            p("DateTimeTerminated") = "NULL"
            p("IsConfirmed") = CInt(1)
            p("DateTimeConfirmed") = "getDate()"
            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationEmployee'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("LibraryOrganizationCustomer")

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("ActivationDate") = "getdate()"
            p("LastUpdated") = "getdate()"
            p("IsBan") = ReadyString(0)

            pSql = "SELECT * FROM MobizLibraryPeople WHERE ID = " & ReadyString(PeopleID)
            row = Common.WebRequest.ProData.Execute(pSql).Rows(0)

            If CBool2(row("IsFromOrg")) Then
                pSql = "SELECT OrganizationID FROM MobizLibraryOrganizationContactPerson WHERE PeopleID = " & ReadyString(PeopleID)

                p("OrganizationID") = ReadyString(Common.WebRequest.ProData.Execute(pSql).Rows(0)("OrganizationID").ToString())
            Else
                p("OrganizationID") = ReadyString(PeopleID)
            End If

            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            p = New InsertUpdate("LibraryPeople") 'For CRM

            p("ID") = ReadyString(PeopleID)
            p("FirstName") = ReadyString(row("FirstName"))
            p("MiddleName") = ReadyString(row("MiddleName"))
            p("Surname") = ReadyString(row("Surname"))
            p("DateOfBirth") = ReadyString(row("DateOfBirth"))
            p("LastUpdated") = "getDate()"

            pSql = p.GetInsert()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "SELECT * FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(PeopleID)
            row = Common.WebRequest.ProData.Execute(pSql).Rows

            If row.Count > 0 Then
                p = New InsertUpdate("LibraryPeopleContactEmail")

                p("ID") = ReadyString(row(0)("ID"))
                p("PeopleID") = ReadyString(PeopleID)
                p("EmailAddress") = ReadyString(row(0)("EmailAddress"))
                p("EmailType") = ReadyString("Personal Email")
                p("LastUpdated") = "getDate()"
                p("EmailActivationLinkID") = ReadyString(row(0)("EmailActivationLinkID"))
                p("ConfirmationCode") = ReadyString(row(0)("ConfirmationCode"))
                p("IsEmailConfirmed") = ReadyString(row(0)("IsEmailConfirmed"))
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return "confirmed"

        Catch ex As Exception
            AppendToLog(ex.Message)
            Return "error"
        End Try
    End Function

    Function MyProfileDetails() As String
        Dim pOut As New JsonDictionary

        Dim pSql As String = "SELECT FirstName, MiddleName, Surname, CONVERT(Varchar, DateOfBirth) as DateOfBirth, ISNULL(PersonalSuffixName, '') as PersonalSuffixName, IsActive FROM MobizLibraryPeople WHERE ID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("Details", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        pSql = "SELECT ID, EmailAddress FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("Email", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        pSql = "SELECT ID, Username FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("Credentials", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        pSql = "SELECT ID, SecurityQuestion FROM MobizLibraryPeopleSecurityQuestion WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
        pOut.Add("SecQ", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        Return pOut.Serialize()
    End Function

    Function CreateAccount() As String
        Dim p As New InsertUpdate("MobizLibraryPeople")
        Dim ppolID As String = Guid.NewGuid().ToString()
        Dim emaillinkid As String = Guid.NewGuid.ToString()
        Dim signup As New SignUp

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
            p("IsFromOrg") = ReadyString(CBool2(PersonDetails("isPartOfOrganization")))

            Common.WebRequest.ProData.Execute(p.GetInsert())

            p = New InsertUpdate("MobizLibraryPeopleContactEmail")

            p("PeopleID") = ReadyString(ppolID)
            p("EmailAddress") = ReadyString(PersonDetails("EmailAddress"))
            p("EmailType") = ReadyString("Work Email")
            p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))
            p("IsEmailConfirmed") = 0
            p("EmailActivationLinkID") = ReadyString(emaillinkid)

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

            '''' NOTE: do not save the new user as customer, he/she must confirm the link first before he/she will be consider as customer

            If CBool2(PersonDetails("isPartOfOrganization")) Then
                Dim orgid As String = Guid.NewGuid.ToString()

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
                p("IsUseForPersonOrgAffiliates") = ReadyString(0)

                Common.WebRequest.ProData.Execute(p.GetInsert())
            End If

            If Not SendConfirmationLink(PersonDetails("EmailAddress"), emaillinkid, PersonDetails("FirstName")) Then
                Return "There was an error occured during sending confirmation link to your email. Please contact the mobiz technical team."
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SendConfirmationLink(ByVal email As String, ByVal emaillinkid As String, ByVal personname As String) As Boolean
        Try
            Dim link As String = "http://" & Url & "/Konstruksyon/Confirm?id=" & emaillinkid &
                        "&c=" & GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper &
                        "&OrgID=3e630d60-1be9-4673-9c60-405855efdf23"

            Dim msg As String = "<table style='width:100%; border:none; background-color: #f5f5f5;'>" +
                                    "<tr><td style='height:100px; text-align:center; padding:20px; vertical-align:middle; background-color: #0083c1; font-size: 40px; color: #FFF;'>WELCOME</td></tr>" +
                                    "<tr><td style='padding:20px; font-size: 20px;'>Hello " + personname.ToUpper() + ",<br /><br />Thanks for registering! Before you can login, please click the button below to activate your account.<br /></td></tr>" +
                                    "<tr><td style='padding:20px; font-size: 20px; vertical-align:middle; text-align:center;'><a href='" & link & "' style='background-color: #5cb85c; border: none; border-radius: 5px; padding: 15px; color: #fff; text-decoration:none;'>ACTIVATE MY ACCOUNT</a><br /><br /></td></tr>" +
                                    "<tr><td style='padding:20px; vertical-align:middle; background-color: #dadada; font-size: 16px;'>To know more about Mobiz, please <a href='http://mobiz-apps.com' target='_blank' style='text-decoration:none;'>click here</a>.</td></tr>" +
                                "</table>"

            SendMail("mobizcompleteretail@outlook.com",
                     email,
                     "Mobiz",
                     "Welcome to Mobiz!",
                     msg,
                     "smtp.live.com",
                     587,
                     "mobizcompleteretail@outlook.com",
                     "m0b!zp30pl30nly!", , , , , True)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function ConfirmedLink() As String
        Dim pSql = "SELECT * FROM MobizLibraryPeopleContactEmail WHERE EmailActivationLinkID = " & ReadyString(EmailLinkID)
        Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

        If rows.Count = 0 Then
#If DEBUG Then
            Return "Confirmation link not found. Please <a href='http://localhost:8080/Konstruksyon'>click here</a> to sign up."
#Else
            Return "Confirmation link not found. Please <a href='"& Request.Url.Authority &"/Konstruksyon'>click here</a> to sign up."
#End If

        Else
            If CBool(rows(0)("IsEmailConfirmed")) Then
#If DEBUG Then
                Return "Link already expired. Please <a href='http://localhost:8080/Konstruksyon'>click here</a> to go to login page."
#Else
                Return "Link already expired. Please <a href='"& Request.Url.Authority &"/Konstruksyon'>click here</a> to go to login page."
#End If
            End If

            Common.WebRequest.ProData.BeginTransaction()

            Dim p As New InsertUpdate("MobizLibraryPeopleContactEmail")

            p("ConfirmationCode") = ReadyString(ConfirmationCode)
            p("IsEmailConfirmed") = 1

            pSql = p.GetUpdate(" EmailActivationLinkID = " & ReadyString(EmailLinkID))
            Common.WebRequest.ProData.Execute(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return rows(0)("PeopleID").ToString()
        End If
    End Function

    Function SaveMyAccount() As String
        Try
            Common.WebRequest.ProData.BeginTransaction()

            Dim p As New InsertUpdate("MobizLibraryPeople")
            Dim pSql As String
            Dim pDT As Object

            If CBool2(PersonDetails("IsEditName")) Then
                p("FirstName") = ReadyString(PersonDetails("FirstName"))
                p("MiddleName") = ReadyString(PersonDetails("MiddleName"))
                p("Surname") = ReadyString(PersonDetails("LastName"))
                p("PersonalSuffixName") = ReadyString(PersonDetails("GenSuffix"))
            End If

            p("DateOfBirth") = ReadyString(CDate(PersonDetails("DateOfBirth")))
            p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(WebSession.Instance.PeopleUserID)))

            p = New InsertUpdate("MobizLibraryOrganization")

            If CBool2(PersonDetails("IsEditName")) Then
                p("PersonFirstName") = ReadyString(PersonDetails("FirstName"))
                p("PersonMiddleName") = ReadyString(PersonDetails("MiddleName"))
                p("PersonSurname") = ReadyString(PersonDetails("LastName"))
                p("PersonSuffixName") = ReadyString(PersonDetails("GenSuffix"))
            End If

            p("PersonDateOfBirth") = ReadyString(CDate(PersonDetails("DateOfBirth")))
            p("LastUpdated") = ReadyString(CDate(PersonDetails("DateNow")))

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(WebSession.Instance.PeopleUserID)))

            pSql = "SELECT TOP 1 ID FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
            pDT = Common.WebRequest.ProData.Execute(pSql).Rows

            p = New InsertUpdate("MobizLibraryPeopleContactEmail")
            p("EmailAddress") = ReadyString(PersonDetails("EmailAddress"))

            If pDT.Count > 0 Then
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(pDT(0)("ID").ToString())))
            Else
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("PeopleID") = ReadyString(WebSession.Instance.PeopleUserID)
                Common.WebRequest.ProData.Execute(p.GetInsert())
            End If

            p = New InsertUpdate("MobizLibraryPeopleUserAccount")
            p("Username") = ReadyString(PersonDetails("Username"))

            If CBool2(PersonDetails("IsChangePassword")) Then
                p("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("Password")))
            End If

            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

            p("SecurityQuestion") = ReadyString(PersonDetails("SecQ1"))
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("SecQ1Ans")))

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("SecQ1ID"))))

            p = New InsertUpdate("MobizLibraryPeopleSecurityQuestion")

            p("SecurityQuestion") = ReadyString(PersonDetails("SecQ2"))
            p("SecurityAnswer") = ReadyString(ProPack.Security.Hash.SaltedHashedString(PersonDetails("SecQ2Ans")))

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(PersonDetails("SecQ2ID"))))

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return "error"
        End Try
    End Function

    Function CheckIfUserAlreadyTakeSurvey()
        Dim pSql = "SELECT * FROM LibrarySurveyRespondent WHERE ClientID = " & ReadyString(ClientID)

        If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
            Return "done"
        Else
            Return "notyet"
        End If
    End Function

    Function GetResponse() As Object
        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String = "SELECT res.*, field.Label, q.Type, q.IsUseCheckbox, field.ControlType as Control, resd.ClientID, field.DataType " & vbCrLf &
                            " FROM LibrarySurveyResponse as res " & vbCrLf &
                            " LEFT JOIN LibrarySurveyQuestionFields as field ON field.ID = res.FieldID " & vbCrLf &
                            " LEFT JOIN LibrarySurveyQuestions as q ON q.ID = res.QuestionID " & vbCrLf &
                            " LEFT JOIN LibrarySurveyRespondent as resd ON resd.ID = res.RespondentID " & vbCrLf &
                            " WHERE res.RespondentID = " & ReadyString(Common.WebRequest.Data("RID")) & vbCrLf
        Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

        Dim rows As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        For Each f As Object In rows
            If f("Type") <> "Single Input" And f("Type") <> "file" Then
                pSql = "SELECT TunneledTo FROM LibrarySurveyQuestionFieldsTunnel WHERE FieldID = " & ReadyString(f("FieldID").ToString())
                Dim tunnel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If tunnel.Count > 0 Then
                    f.Add("Tunnel", tunnel(0)("TunneledTo").ToString())
                Else
                    f.Add("Tunnel", "")
                End If
            End If
        Next f

        Return Json(New With {Key .data = rows})
    End Function

    Function GetCustomerBuildingPermit()
        Dim pSql As String

        pSql = "SELECT res.ID as RespondentID, qre.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as CreatedByName, " & vbCrLf &
                " (SELECT Link FROM MobizLibrarySurveyQuestionaireLink WHERE QuestionaireID = qre.ID) as Link, " & vbCrLf &
                " --(SELECT COUNT(ID) FROM LibrarySurveyRespondent WHERE QuestionaireID = qre.ID) as Respondents, " & vbCrLf &
                " --(SELECT ID FROM LibrarySurveyRespondent WHERE QuestionaireID = qre.ID AND ClientID = " & ReadyString(ClientID) & ") as RespondentID, " & vbCrLf &
                " res.DateResponded, " & vbCrLf &
                " (SELECT Answer FROM LibrarySurveyResponse WHERE RespondentID = res.ID AND QuestionID = 'c86d5652-5f14-4957-a7db-75304ffbd0ac') as ProjectName " & vbCrLf &
                " --(SELECT ISNULL(SUM(Score), 0) as Score FROM librarysurveyrespondent as respondent" & vbCrLf &
                " --  INNER JOIN librarysurveyresponse as response ON response.RespondentID = respondent.ID " & vbCrLf &
                " --  INNER JOIN librarysurveyquestionfields as field ON field.ID = response.FieldID " & vbCrLf &
                " --  WHERE respondent.ClientID = " & ReadyString(CStr2(ClientID)) & ") as Score " & vbCrLf &
                " FROM LibrarySurveyRespondent as res " & vbCrLf &
                " LEFT JOIN LibrarySurveyQuestionaire as qre ON res.QuestionaireID = qre.ID " & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = qre.CreatedBy " & vbCrLf &
                " WHERE res.ClientID = " & ReadyString(CStr2(ClientID)) & vbCrLf

        Dim pDTQuestionaire As DataTable = Common.WebRequest.ProData.Execute(pSql)
        Dim arrSurvey As New List(Of Dictionary(Of String, Object))

        If pDTQuestionaire.Rows.Count > 0 Then
            If pDTQuestionaire.Rows(0).Table.Columns("Error") IsNot Nothing Then
                Return pDTQuestionaire.Rows(0)("ERROR")
            End If

            For i As Integer = 0 To pDTQuestionaire.Rows.Count - 1
                Dim survey As Object = pDTQuestionaire.Rows(i)

                arrSurvey.Add(GetSurveyDetails(survey, i + 1, True))
            Next
        End If

        Return Json(New With {Key .data = arrSurvey}, JsonRequestBehavior.AllowGet)
    End Function

    Function GetSurveyDetails(ByVal survey As Object, ByVal RowNum As Integer, Optional ByVal byClient As Boolean = False) As Object
        Dim pItemSurvey As New Dictionary(Of String, Object)
        Dim pSql As String = "SELECT * FROM LibrarySurveyPage WHERE QuestionaireID = " & ReadyString(survey("ID").ToString()) & " ORDER BY PageNo ASC"
        Dim pDTPage As DataTable = Common.WebRequest.ProData.Execute(pSql)
        Dim arrPage As New List(Of Dictionary(Of String, Object))

        If pDTPage.Rows.Count > 0 Then
            For p As Integer = 0 To pDTPage.Rows.Count - 1
                Dim page As Object = pDTPage.Rows(p)
                pSql = "SELECT q.*, COUNT(tunnel.TunneledTo) as TunnelCount FROM LibrarySurveyQuestions as q " & vbCrLf &
                        " LEFT JOIN LibrarySurveyQuestionFieldsTunnel as tunnel ON q.ID = tunnel.TunneledTo " & vbCrLf &
                        " WHERE PageID = " & ReadyString(page("ID")) & vbCrLf &
                        " AND ISNULL(q.IsDelete, 0) = 0 " & vbCrLf &
                        " GROUP by q.VariableName, q.IsDelete, q.ID, q.QuestionaireID, q.Question, q.Type, q.IsRequired, q.HasOther, q.LastUpdated, q.IsUseCheckbox, q.PageID, q.OrderNo, q.IsRadioButtonPresentAsSelectMenu " & vbCrLf &
                        " ORDER BY q.OrderNo ASC "

                Dim pDTQuestions As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim arrQuestions As New List(Of Dictionary(Of String, Object))
                Dim pItemPage As New Dictionary(Of String, Object)

                If pDTQuestions.Rows.Count > 0 Then
                    For m As Integer = 0 To pDTQuestions.Rows.Count - 1
                        Dim question As Object = pDTQuestions.Rows(m)
                        Dim pItemQuestion As New Dictionary(Of String, Object)

                        pItemQuestion.Add("ID", question("ID"))
                        pItemQuestion.Add("PageID", page("ID"))
                        pItemQuestion.Add("QuestionType", question("Type"))
                        pItemQuestion.Add("Question", HttpUtility.HtmlEncode(question("Question")))
                        pItemQuestion.Add("Required", question("IsRequired"))
                        pItemQuestion.Add("VariableName", CStr2(question("VariableName")))
                        pItemQuestion.Add("TunnelCount", question("TunnelCount"))
                        pItemQuestion.Add("IsUseCheckbox", CBool2(question("IsUseCheckbox")))
                        pItemQuestion.Add("IsRadioButtonPresentAsSelectMenu", CInt2(question("IsRadioButtonPresentAsSelectMenu")))
                        pItemQuestion.Add("OrderNo", CInt2(question("OrderNo")))
                        pItemQuestion.Add("State", "edit")
                        pItemQuestion.Add("Index", m)

                        If question("Type") = "radioinput" Or question("Type") = "checkboxinput" Then
                            pItemQuestion.Add("IsWithField", True)
                        End If

                        If question("Type") <> "Single Input" Then
                            pSql = "SELECT field.*, tunnel.TunneledTo FROM LibrarySurveyQuestionFields as field " & vbCrLf &
                                " LEFT JOIN LibrarySurveyQuestionFieldsTunnel as tunnel ON tunnel.FieldID = field.ID " & vbCrLf &
                                " WHERE QuestionID = " & ReadyString(question("ID")) & vbCrLf &
                                " AND ISNULL(field.IsDelete, 0) = 0 ORDER BY field.OrderNo ASC"
                            Dim pDTFields As DataTable = Common.WebRequest.ProData.Execute(pSql)
                            Dim arrFields As New List(Of Dictionary(Of String, Object))
                            Dim arrRows As New List(Of Dictionary(Of String, Object))
                            Dim arrColumns As New List(Of Dictionary(Of String, Object))

                            If pDTFields.Rows.Count > 0 Then
                                For n As Integer = 0 To pDTFields.Rows.Count - 1
                                    Dim fields As Object = pDTFields.Rows(n)
                                    Dim pItemFields As New Dictionary(Of String, Object)
                                    Dim arrReq As New List(Of String)

                                    pSql = "SELECT * FROM LibrarySurveyQuestionFieldRequirements WHERE FieldID = " & ReadyString(fields("ID").ToString())
                                    Dim rowsReq As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                                    For Each req As Object In rowsReq
                                        arrReq.Add(req("RequirementID").ToString())
                                    Next req

                                    If question("Type") <> "Matrix" Then
                                        If CBool2(fields("IsOther")) Then
                                            Dim arrOther As Object
                                            arrOther = New With {Key .AsType = fields("AsType"),
                                                                    .Label = fields("Label"),
                                                                    .Score = fields("Score"),
                                                                    .ID = fields("ID").ToString(),
                                                                    .VariableName = fields("VariableName").ToString()}
                                            pItemQuestion.Add("Other", arrOther)
                                        Else
                                            pItemFields.Add("ID", fields("ID").ToString())
                                            pItemFields.Add("Label", fields("Label"))
                                            pItemFields.Add("Score", fields("Score"))
                                            pItemFields.Add("Value", CStr2(fields("Value")))
                                            pItemFields.Add("TunneledTo", CStr2(fields("TunneledTo")))
                                            pItemFields.Add("VariableName", CStr2(fields("VariableName")))
                                            pItemFields.Add("State", "edit")
                                            pItemFields.Add("IsRequired", CBool2(fields("IsRequired")))
                                            pItemFields.Add("DataType", CStr2(fields("DataType")))
                                            pItemFields.Add("CharMax", CInt2(fields("CharMax")))
                                            pItemFields.Add("CharMin", CInt2(fields("CharMin")))
                                            pItemFields.Add("OrderNo", CInt2(fields("OrderNo")))
                                            pItemFields.Add("Requirements", arrReq)
                                            arrFields.Add(pItemFields)
                                        End If
                                    Else
                                        pItemFields.Add("ID", fields("ID").ToString())
                                        pItemFields.Add("Label", fields("Label"))
                                        pItemFields.Add("Score", fields("Score"))
                                        pItemFields.Add("Control", fields("ControlType"))
                                        pItemFields.Add("Type", fields("AsType"))
                                        pItemFields.Add("State", "edit")
                                        pItemFields.Add("IsRequired", CBool2(fields("IsRequired")))
                                        pItemFields.Add("DataType", CStr2(fields("DataType")))
                                        pItemFields.Add("CharMax", CInt2(fields("CharMax")))
                                        pItemFields.Add("CharMin", CInt2(fields("CharMin")))
                                        pItemFields.Add("OrderNo", CInt2(fields("OrderNo")))
                                        pItemFields.Add("VariableName", CStr2(fields("VariableName")))

                                        If fields("AsType") = "Row" Then
                                            arrRows.Add(pItemFields)
                                        Else
                                            arrColumns.Add(pItemFields)
                                        End If
                                    End If

                                Next
                                If question("Type") = "Matrix" Then
                                    Dim matrixFields As New Dictionary(Of String, Object)
                                    matrixFields.Add("Rows", arrRows)
                                    matrixFields.Add("Columns", arrColumns)
                                    pItemQuestion.Add("Fields", matrixFields)
                                Else
                                    pItemQuestion.Add("Fields", arrFields)
                                End If
                            End If
                        End If

                        arrQuestions.Add(pItemQuestion)
                    Next
                End If

                pItemPage.Add("ID", page("ID").ToString())
                pItemPage.Add("Title", page("Title").ToString())
                pItemPage.Add("PageNo", page("PageNo").ToString())
                pItemPage.Add("State", "edit")
                pItemPage.Add("Index", p)
                pItemPage.Add("Questions", arrQuestions)
                pItemPage.Add("PrevLabel", page("PrevLabel").ToString())
                pItemPage.Add("PrevAction", page("PrevAction").ToString())
                pItemPage.Add("PrevGoto", page("PrevGoto").ToString())
                pItemPage.Add("NextLabel", page("NextLabel").ToString())
                pItemPage.Add("NextAction", page("NextAction").ToString())
                pItemPage.Add("NextGoto", page("NextGoto").ToString())
                pItemPage.Add("NextGotoLink", page("NextGotoLink").ToString())
                pItemPage.Add("IsShowButton", page("IsShowButton").ToString())
                arrPage.Add(pItemPage)
            Next
        End If

        pSql = "SELECT req.*, mrt.FileName, 'edit' as State, g.ID as ReqGroupID, g.Name as RequiredBy, CONVERT(VARCHAR(MAX),req.Lastupdated,120) as ReqLastUpdated FROM LibrarySurveyRequirements as req " & vbCrLf &
                " LEFT JOIN LibrarySurveyFiles as mrt ON mrt.ID = req.MrtFileID " & vbCrLf &
                " LEFT JOIN LibrarySurveyGroupRequirements as g ON g.ID = req.GroupRequirementID " & vbCrLf &
                " WHERE req.QuestionaireID = " & ReadyString(survey("ID").ToString())

        pItemSurvey.Add("Requirements", jss.Deserialize(Of Object)(DataTableToJSON.GetJson((Common.WebRequest.ProData.Execute(pSql)))))
        pItemSurvey.Add("ID", survey("ID"))
        pItemSurvey.Add("Title", survey("Title"))
        pItemSurvey.Add("CreatedBy", survey("CreatedBy"))
        pItemSurvey.Add("IsActive", survey("IsActive"))
        pItemSurvey.Add("Link", survey("Link"))
        pItemSurvey.Add("ProjectName", survey("ProjectName"))
        'pItemSurvey.Add("Respondents", survey("Respondents"))
        pItemSurvey.Add("LastUpdated", Format(IIf(CStr2(survey("LastUpdated")) = "", survey("DateCreated"), survey("LastUpdated")), "MMM dd, yyyy hh:mm:ss tt"))
        'pItemSurvey.Add("CreatedByName", survey("CreatedByName"))
        pItemSurvey.Add("Pages", arrPage)

        pSql = "SELECT *, 'edit' as State FROM LibrarySurveyGroupRequirements WHERE QuestionaireID = " & ReadyString(survey("ID").ToString()) & vbCrLf &
                " ORDER BY OrderNo ASC"

        pItemSurvey.Add("ReqGroups", jss.Deserialize(Of Object)(DataTableToJSON.GetJson((Common.WebRequest.ProData.Execute(pSql)))))

        If survey.Table.Columns.Contains("Score") Then
            pItemSurvey.Add("Score", survey("Score"))
        End If
        pItemSurvey.Add("RowNum", RowNum)

        If byClient Then
            pItemSurvey.Add("DateResponded", Format(survey("DateResponded"), "MMM dd, yyyy hh:mm:ss tt"))
            pItemSurvey.Add("RespondentID", survey("RespondentID"))
        End If

        Return pItemSurvey
    End Function

    Function GetFiles(ByVal RespondentID As String) As DataTable
        Dim pSql As String = "SELECT ID, FileName, LastUpdated FROM LibrarySurveyUsersFiles " & vbCrLf &
                            " WHERE RespondentID = " & ReadyString(RespondentID) & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function
End Class