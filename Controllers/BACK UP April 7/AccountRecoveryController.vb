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

    End Class
End Namespace