Namespace WebProj
    Public Class AccountRecoveryController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /AccountRecovery

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function getPeopleSecurityQuestionList() As String
            Dim strID As String
            Dim clsPeople As New SignUp
            Dim strReturn As String
            Dim dtaTable As DataTable
            Dim dtaRow As DataRow

            strID = ""
            clsPeople.ID = ""
            clsPeople.Surname = Common.WebRequest.Data("Surname")
            clsPeople.FirstName = Common.WebRequest.Data("FirstName")
            clsPeople.MiddleName = Common.WebRequest.Data("MiddleName")
            clsPeople.DateOfBirth = Common.WebRequest.Data("DateOfBirth")
            clsPeople.MotherMaidenSurname = Common.WebRequest.Data("MotherMaidenSurname")
            clsPeople.MotherMaidenFirstName = Common.WebRequest.Data("MotherMaidenFirstName")
            clsPeople.MotherMaidenMiddleName = Common.WebRequest.Data("MotherMaidenMiddleName")
            clsPeople.PersonalSuffixName = Common.WebRequest.Data("PersonalSuffixName")

            dtaTable = New DataTable()
            dtaTable = clsPeople.CheckPeopleUniqueName(True)
            If (dtaTable.Rows.Count > 0) Then
                dtaRow = dtaTable.Rows(0)
                strID = dtaRow("ID").ToString()
            End If

            strReturn = JsonDictionary.Serialize("PeopleSecurityData", clsPeople.getPeopleSecurityQuestionList(strID))

            Return strReturn

        End Function


        <HttpPost()>
        Function SavePeopleUserAccountRecovery() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If

            clsPeople.PeopleID = Common.WebRequest.Data("PeopleID")
            clsPeople.UserName = Common.WebRequest.Data("Username")
            clsPeople.UserPassword = ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("UserPassword"))

            strReturn = JsonDictionary.Serialize("PeopleUserAccountData", clsPeople.SavePeopleUserAccountRecovery())

            'Set cookie to authorize the user to see the register success page
            FormsAuthentication.SetAuthCookie("User", False)

            Return strReturn

        End Function

        <HttpPost>
        Public Function SendAccountRecovery() As String

            Dim pEmail As String
            Dim pDT As DataTable
            Dim pMessage As String

            pEmail = Common.WebRequest.Data("Email")
            If IsEmail(pEmail) = False Then
                Return JsonDictionary.Serialize("Error", pEmail & " is not a valid email address.")
            End If
            WebRequest.Instance.Business.LibraryPeople.InsertResetCodes(pEmail)
            pDT = WebRequest.Instance.Business.LibraryPeople.GetResetCodes(pEmail)
            pMessage = "Please click the following link to change your password." & vbCrLf
            If pDT.Rows.Count > 0 Then
                For Each pDR As DataRow In pDT.Rows
                    pMessage &= "Username: " & pDR("Username") & vbCrLf &
                        "http://122.52.123.251/AccountRecovery?ResetCode=" & HttpUtility.UrlEncode(pDR("ResetCode")) & vbCrLf & vbCrLf
                Next pDR
                SendMail("mobizcompleteretail@outlook.com",
                    pEmail,
                    "Mobiz",
                    "Mobiz Account Recovery",
                    pMessage,
                    "smtp.live.com",
                    587,
                    "mobizcompleteretail@outlook.com",
                    "m0b!zp30pl30nly!")
            End If

            Return JsonDictionary.Serialize("Count", pDT.Rows.Count)

        End Function

        <HttpPost>
        Public Function ChangePassword() As String

            Dim pPassword As String
            Dim pResetCode As String
            Dim pDT As DataTable
            Dim pIU As InsertUpdate
            Dim pSql As String

            pPassword = Common.WebRequest.Data("Password")
            If CStr2(pPassword).Length < 8 Then
                Return JsonDictionary.Serialize("Error", "Password must be at least 8 characters.")
            End If

            pResetCode = Common.WebRequest.Data("ResetCode")
            If CStr2(pResetCode).Length <> 30 Then
                Return JsonDictionary.Serialize("Error", "ResetCode is required.")
            End If

            pDT = WebRequest.Instance.Business.LibraryPeople.GetAccountByResetCode(pResetCode)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Reset code was already used.")
            End If

            pIU = New InsertUpdate("MobizLibraryPeopleUserAccount")
            pIU("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(pPassword))
            pIU("ResetCode") = "NULL"
            pSql = pIU.GetUpdate("ResetCode = " & ReadyString(pResetCode))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function VerifyResetCode() As String

            Dim pResetCode As String
            Dim pDT As DataTable
          
            pResetCode = Common.WebRequest.Data("ResetCode")
            If CStr2(pResetCode).Length <> 30 Then
                Return JsonDictionary.Serialize("Error", "ResetCode is required.")
            End If

            pDT = WebRequest.Instance.Business.LibraryPeople.GetAccountByResetCode(pResetCode)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Result", "used")
            End If

            Return JsonDictionary.Serialize("Result", "ok")

        End Function
    End Class
End Namespace