Namespace WebProj
    Public Class AccountManagementController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /AccountManagement

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
                If WebSession.Instance.IsFirstPartySignUp Then Return RedirectToAction("Index", "SignIn")
            End If

            Return View()
        End Function


        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            'If WebSession.Instance.PeopleUserID Is Nothing Then WebSession.Instance.PeopleUserID = ""
            ' Return JsonDictionary.Serialize("Error", "Access denied")

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PeopleID", WebSession.Instance.PeopleID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckPeopleUserAccountUnchanged() As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dtaTable As New DataTable
            Dim intCntr As Integer
            Dim pOut As JsonDictionary

            clsPeople.UserName = Common.WebRequest.Data("Username")
            clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")
            dtaTable = clsPeople.CheckPeopleUserAccount()
            pOut = New JsonDictionary

            strReturn = "False"

            If (dtaTable.Rows.Count > 0) Then
                intCntr = 0
                Do Until (intCntr >= dtaTable.Rows.Count)

                    If (ProPack.Security.Hash.Verify(clsPeople.UserPassword, dtaTable.Rows(intCntr).Item("UserPassword").ToString())) Then
                        strReturn = "True"
                        Exit Do
                    End If

                    intCntr = intCntr + 1
                Loop

            End If

            pOut.Add("PeopleUserAccountData", strReturn)

            Return pOut.Serialize
        End Function

        <HttpPost()>
        Function SavePeopleUserAccountManage() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If

            clsPeople.PeopleID = Common.WebRequest.Data("PeopleID")
            clsPeople.UserName = Common.WebRequest.Data("Username")
            clsPeople.UserPassword = ProPack.Security.Hash.SaltedHashedString(Common.WebRequest.Data("UserPassword"))

            clsPeople.SecurityQuestion1 = Common.WebRequest.Data("Question1")
            clsPeople.SecurityQuestion1Answer = Common.WebRequest.Data("Question1Answer")
            clsPeople.SecurityQuestion2 = Common.WebRequest.Data("Question2")
            clsPeople.SecurityQuestion2Answer = Common.WebRequest.Data("Question1Answer2")

            strReturn = JsonDictionary.Serialize("PeopleUserAccountManageData", clsPeople.SavePeopleUserAccountManage())

            'Set cookie to authorize the user to see the register success page
            FormsAuthentication.SetAuthCookie("User", False)

            Return strReturn

        End Function
    End Class
End Namespace