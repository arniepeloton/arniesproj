Imports System.IO
Imports C1.C1Zip
Imports System.Reflection
Imports WebProj.MobizApplication

Public Class MobizPortalController
    Inherits System.Web.Mvc.Controller

    Public Function Index() As ActionResult
        WebSession.Instance.PeopleUserID = Nothing
        FormsAuthentication.SignOut() 'Clear the form
        Return View()
    End Function

    Public Function Dashboard() As ActionResult
        Return View()
    End Function

    Public Function Logout() As ActionResult
        WebSession.Instance.PeopleUserID = Nothing

        Return RedirectToAction("", "MobizPortal")
    End Function

    Public Function Login() As String
        Try
            Dim uname As String = Common.WebRequest.Data("Username")
            Dim pword As String = Common.WebRequest.Data("Password")

            WebSession.Instance.PeopleUserID = Nothing

            If uname <> "mobizadmin" Or pword <> "mobizpeopleonly" Then
                Return JsonDictionary.Serialize("Results", "invalid")
            Else
                'Set cookie to authorize the user to see the received request page
                FormsAuthentication.SetAuthCookie("User", False)
                WebSession.Instance.PeopleUserID = "adminid"
                Return JsonDictionary.Serialize("Results", "success")
            End If
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function GetNewCompany() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim pSql As String
            Dim search As String = Common.WebRequest.Data("Search")

            pSql = "SELECT org.ID as CompanyID, " & vbCrLf &
                    " org.CompanyName, " & vbCrLf &
                    " ppol.ID as SuperAdminID, " & vbCrLf &
                    " ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as Administrator, " & vbCrLf &
                    " sub.SubscriptionDate, " & vbCrLf &
                    " sub.TransferStatus, " & vbCrLf &
                    " ppolemail.EmailAddress,  " & vbCrLf &
                    " '0' + prefix.Prefix + '' + ppolmobile.ContactNo as MobileNo " & vbCrLf &
                    " FROM TransSubscription as sub " & vbCrLf &
                    " INNER JOIN LibraryOrganization as org ON org.ID = sub.OrganizationID " & vbCrLf &
                    " INNER JOIN LibraryPeople as ppol ON ppol.ID = sub.SuperAdminID " & vbCrLf &
                    " LEFT JOIN LibraryPeopleContactEmail as ppolemail ON ppolemail.PeopleID = sub.SuperAdminID " & vbCrLf &
                    " LEFT JOIN LibraryPeopleContactMobile as ppolmobile ON ppolmobile.PeopleID = sub.SuperAdminID " & vbCrLf &
                    " LEFT JOIN LibraryTelcoPrefixes as prefix ON prefix.ID = ppolmobile.TelCoPrefixID " & vbCrLf &
                    IIf(search = "", "", " WHERE org.CompanyName + ' ' + ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname LIKE " & ReadyStringWild(search)) & vbCrLf &
                    " ORDER BY sub.SubscriptionDate DESC, CompanyName ASC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function Activate() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim p As New InsertUpdate("LibraryPeopleContactEmail")
            Dim clsSubscribe As New Subscribe
            Dim clsConfirmed As New ConfirmedController
            Dim pSql As String

            pSql = "SELECT ID FROM LibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID"))
            clsSubscribe.ContactEmailID = Common.WebRequest.ProData.Execute(pSql).Rows(0)("ID").ToString()
            clsSubscribe.ConfirmationCode = GenerateString(6, "ABCDEFGHJKLMNPQRSTUVWXYZ23456789").ToUpper

            Dim strResult As String = clsSubscribe.SaveConfirmationDetails()

            If CBool2(strResult) Then
                If clsConfirmed.CreateCsv() = "ok" Then
                    Return JsonDictionary.Serialize("Results", "confirmed")
                End If
            Else
                Return JsonDictionary.Serialize("Results", strResult)
            End If
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function
End Class
