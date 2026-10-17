Namespace MobizApplication
    Public Class SignInController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /SignIn

        Function Index() As ActionResult

            If CStr2(Common.WebRequest.QueryString("mode")).ToLower = "logout" Then
                Common.WebSession.UserID = ""
            End If

            FormsAuthentication.SignOut()

            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.Cache.SetNoStore()

            Return View()

        End Function

        Function DownloadDatabase() As ActionResult
            Return View()
        End Function

        Function TraxInfo() As ActionResult
            Return View()
        End Function

        Function Notice() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function CheckNeedEmailVerification() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String
            Dim dt As DataTable

            clsPeople.PeopleID = Common.WebRequest.Data("ID")

            dt = clsPeople.CheckNeedEmailVerification()

            strReturn = JsonDictionary.Serialize("EmailData", dt)

            If (dt.Rows.Count > 0) Then
                WebSession.Instance.IsEmailVerified = False
            Else
                WebSession.Instance.IsEmailVerified = True
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleIsFirstPartySignUp() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String

            clsPeople.PeopleID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("PeopleFirstPartySignUpData", clsPeople.CheckPeopleIsFirstPartySignUp())

            Return strReturn

        End Function
    End Class
End Namespace