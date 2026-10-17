Namespace MobizApplication
    Public Class SignUpController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /SignUp

        Function Index() As ActionResult
            Return View()
        End Function

        Function Success() As ActionResult
            Return View()
        End Function
        Function ImportantReminders() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function getCallingCodeList() As String
            Dim strReturn As String
            Dim dataTblCallingCodeList As DataTable

            dataTblCallingCodeList = Common.WebRequest.ProData.Execute("SELECT ID, Name, CallingCode FROM LibraryCountries ORDER BY CallingCode, Name")


            strReturn = JsonDictionary.Serialize("CallingCodeData", dataTblCallingCodeList)

            Return strReturn

        End Function

        <HttpPost()>
        Function getTelcoPrefixList() As String
            Dim strReturn As String
            Dim dataTblTelcoPrefixList As DataTable

            dataTblTelcoPrefixList = Common.WebRequest.ProData.Execute("SELECT a.ID, a.Prefix, b.Name FROM LibraryTelcoPrefixes a, LibraryTelcos b WHERE a.TelcoId = b.ID ORDER BY a.Prefix, b.Name")


            strReturn = JsonDictionary.Serialize("TelcoPrefixData", dataTblTelcoPrefixList)

            Return strReturn

        End Function

        <HttpPost()>
        Function getAreaCodeList() As String
            Dim strReturn As String
            Dim dataTblAreaCodeList As DataTable

            dataTblAreaCodeList = Common.WebRequest.ProData.Execute("SELECT ID, Area, CallingCode FROM LibraryAreaCodes ORDER BY CallingCode, Area")


            strReturn = JsonDictionary.Serialize("AreaCodeData", dataTblAreaCodeList)

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleInfoNameInitialForSystemAdministrationOnly() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String
            Dim PeopleID As String

            If (Not IsDate(Common.WebRequest.Data("DateOfBirth"))) Then
                strReturn = JsonDictionary.Serialize("PeopleNameData", "DateOfBirthError")
            Else

                clsPeople.ID = ""
                clsPeople.TitleID = Common.WebRequest.Data("Title")
                clsPeople.Surname = Common.WebRequest.Data("Surname")
                clsPeople.FirstName = Common.WebRequest.Data("FirstName")
                clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
                clsPeople.PersonalSuffixName = Common.WebRequest.Data("PersonalSuffixName")
                clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
                clsPeople.NickName = Common.WebRequest.Data("Nickname")
                clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
                clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
                clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")

                clsPeople.PersonContactCountryIDTel = Common.WebRequest.Data("PersonContactCountryIDTel")
                clsPeople.PersonContactAreaCodeID = Common.WebRequest.Data("PersonContactAreaCodeID")
                clsPeople.PersonContactNoTel = Common.WebRequest.Data("PersonContactNoTel")
                clsPeople.PersonContactTypeTel = Common.WebRequest.Data("PersonContactTypeTel")

                clsPeople.PersonContactCountryIDMobile = Common.WebRequest.Data("PersonContactCountryIDMobile")
                clsPeople.PersonContactTelCoPrefixID = Common.WebRequest.Data("PersonContactTelCoPrefixID")
                clsPeople.PersonContactNoMobile = Common.WebRequest.Data("PersonContactNoMobile")
                clsPeople.PersonContactTypeMobile = Common.WebRequest.Data("PersonContactTypeMobile")

                clsPeople.PersonContactEmail = Common.WebRequest.Data("PersonContactEmail")
                clsPeople.PersonContactEmailType = Common.WebRequest.Data("PersonContactEmailType")

                clsPeople.UserName = Common.WebRequest.Data("UserName")
                clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")

                clsPeople.IsTermAgreed = Common.WebRequest.Data("IsTermAgreed")
                clsPeople.IsCertifiedCorrect = Common.WebRequest.Data("IsCertifiedCorrect")
                clsPeople.IsFirstPartySignUp = Common.WebRequest.Data("IsFirstPartySignUp")

                If (clsPeople.IsFirstPartySignUp = "1") Then
                    clsPeople.SecurityQuestion1 = Common.WebRequest.Data("Question1")
                    clsPeople.SecurityQuestion1Answer = Common.WebRequest.Data("Question1Answer")
                    clsPeople.SecurityQuestion2 = Common.WebRequest.Data("Question2")
                    clsPeople.SecurityQuestion2Answer = Common.WebRequest.Data("Question2Answer")
                End If

                'WebSession.Instance.PeopleID = clsPeople.SavePeopleInfoNameInitial()
                'WebSession.Instance.PeopleUserID = WebSession.Instance.PeopleID
                PeopleID = clsPeople.SavePeopleInfoNameInitial()

                If (PeopleID = "Error") Then
                    strReturn = JsonDictionary.Serialize("PeopleNameData", "Error")
                Else
                    strReturn = JsonDictionary.Serialize("PeopleNameData", PeopleID)

                    'Set cookie to authorize the user to see the register success page
                    FormsAuthentication.SetAuthCookie("User", False)
                End If

            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePeopleInfoNameInitial() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String

            If (Not IsDate(Common.WebRequest.Data("DateOfBirth"))) Then
                strReturn = JsonDictionary.Serialize("PeopleNameData", "DateOfBirthError")
            Else

                clsPeople.ID = ""
                clsPeople.TitleID = Common.WebRequest.Data("Title")
                clsPeople.Surname = Common.WebRequest.Data("Surname")
                clsPeople.FirstName = Common.WebRequest.Data("FirstName")
                clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
                clsPeople.PersonalSuffixName = Common.WebRequest.Data("PersonalSuffixName")
                clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
                clsPeople.NickName = Common.WebRequest.Data("Nickname")
                clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
                clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
                clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")

                clsPeople.PersonContactCountryIDTel = Common.WebRequest.Data("PersonContactCountryIDTel")
                clsPeople.PersonContactAreaCodeID = Common.WebRequest.Data("PersonContactAreaCodeID")
                clsPeople.PersonContactNoTel = Common.WebRequest.Data("PersonContactNoTel")
                clsPeople.PersonContactTypeTel = Common.WebRequest.Data("PersonContactTypeTel")

                clsPeople.PersonContactCountryIDMobile = Common.WebRequest.Data("PersonContactCountryIDMobile")
                clsPeople.PersonContactTelCoPrefixID = Common.WebRequest.Data("PersonContactTelCoPrefixID")
                clsPeople.PersonContactNoMobile = Common.WebRequest.Data("PersonContactNoMobile")
                clsPeople.PersonContactTypeMobile = Common.WebRequest.Data("PersonContactTypeMobile")

                clsPeople.PersonContactEmail = Common.WebRequest.Data("PersonContactEmail")
                clsPeople.PersonContactEmailType = Common.WebRequest.Data("PersonContactEmailType")

                clsPeople.UserName = Common.WebRequest.Data("UserName")
                clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")

                clsPeople.IsTermAgreed = Common.WebRequest.Data("IsTermAgreed")
                clsPeople.IsCertifiedCorrect = Common.WebRequest.Data("IsCertifiedCorrect")
                clsPeople.IsFirstPartySignUp = Common.WebRequest.Data("IsFirstPartySignUp")

                If (clsPeople.IsFirstPartySignUp = "1") Then
                    clsPeople.SecurityQuestion1 = Common.WebRequest.Data("Question1")
                    clsPeople.SecurityQuestion1Answer = Common.WebRequest.Data("Question1Answer")
                    clsPeople.SecurityQuestion2 = Common.WebRequest.Data("Question2")
                    clsPeople.SecurityQuestion2Answer = Common.WebRequest.Data("Question2Answer")
                End If

                WebSession.Instance.PeopleID = clsPeople.SavePeopleInfoNameInitial()
                WebSession.Instance.PeopleUserID = WebSession.Instance.PeopleID

                If (WebSession.Instance.PeopleID = "Error") Then
                    strReturn = JsonDictionary.Serialize("PeopleNameData", "Error")
                Else
                    strReturn = JsonDictionary.Serialize("PeopleNameData", WebSession.Instance.PeopleID)

                    'Set cookie to authorize the user to see the register success page
                    FormsAuthentication.SetAuthCookie("User", False)
                End If

            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleUniqueName() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String
            Dim blnFullCheckName As Boolean

            clsPeople.ID = Common.WebRequest.Data("PeopleID")
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
            Dim clsPeople As New SignUp
            Dim strReturn As String

            clsPeople.UserName = Common.WebRequest.Data("Username")

            strReturn = JsonDictionary.Serialize("PeopleUserNameData", clsPeople.CheckPeopleUniqueUserName())

            Return strReturn

        End Function


        <HttpPost()>
        Function SaveIsFirstPartySignUp() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If

            clsPeople.PeopleID = Common.WebRequest.Data("ID")
            clsPeople.IsFirstPartySignUp = "1"

            strReturn = JsonDictionary.Serialize("PeopleIsFirstPartySignUpData", clsPeople.SaveIsFirstPartySignUp())

            'Set cookie to authorize the user to see the register success page
            FormsAuthentication.SetAuthCookie("User", False)

            Return strReturn

        End Function

    End Class
End Namespace