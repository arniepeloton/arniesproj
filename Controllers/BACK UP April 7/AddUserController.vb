Public Class AddUserController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Return View()

    End Function

    Public Function Add() As ActionResult

        If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.SecurityAccess_SecurityAccess, ErpAccess.eAccessMode.IsNew, True) = False Then Exit Function

        Return View()

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pDR As DataRow
        Dim pKey As String
        Dim pOut As JsonDictionary

        pKey = Common.WebRequest.Data("k")
        If CStr2(pKey).Length = 30 Then
            pDR = WebRequest.Instance.Business.MainSecurity.GetByLink(pKey)
            If pDR Is Nothing Then
                Return JsonDictionary.Serialize("Error", "Invalid Link")
            Else
                pOut = New JsonDictionary
                pOut.Add("Result", "ok")
                pOut.Add("FullName", pDR("FullName"))
                pOut.Add("LevelType", pDR("LevelType"))
                Return pOut.Serialize
            End If
        Else
            Return JsonDictionary.Serialize("Error", "Invalid Link")
        End If

    End Function
    
    <HttpPost()>
    Public Function SetupUser() As String

        Dim pEmployeeID As String
        Dim pSecurityLevelID As String
        Dim pEmail As String
        Dim pLink As String
        Dim pBody As String
        Dim pOut As JsonDictionary
        Dim pErrorEmails As List(Of String)
        Dim pDrEmployee As DataRow

        Try
            If WebSession.Instance.UserAccess Is Nothing _
               OrElse (WebSession.Instance.UserAccess.SecurityAccess_SecurityAccess.IsNew) = False Then

                Return JsonNode.Serialize("Error", "Access denied")
            End If

            pEmployeeID = Common.WebRequest.Data("EmployeeID")
            If Integer.TryParse(pEmployeeID, Nothing) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid EmployeeID")
            End If
            pEmployeeID = CInt(pEmployeeID)

            pSecurityLevelID = Common.WebRequest.Data("SecurityLevelID")
            If IsGuid(pSecurityLevelID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Security Level ID")
            End If
            pSecurityLevelID = ReadyGuid(pSecurityLevelID)

            pEmail = Common.WebRequest.Data("Email")
            If IsEmails(pEmail, , pErrorEmails) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid emails: " & String.Join(", ", pErrorEmails))
            End If

            pLink = WebRequest.Instance.Business.MainSecurity.GetSetupUser(pEmployeeID, pSecurityLevelID)
            If pLink.Length = 0 Then
                pLink = WebRequest.Instance.Business.MainSecurity.SetupUser(pEmployeeID, pSecurityLevelID, pEmail, WebSession.Instance.EmployeeID)
            End If

            pDrEmployee = WebRequest.Instance.Business.EmployeeProfiles.ById(pEmployeeID)
            If pDrEmployee Is Nothing Then
                Return JsonDictionary.Serialize("Error", "Employee does not exist")
            End If
            ' todo: verify inactive or resigned here
            ' todo: verify inactive or existing security level here
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", "An error occured while creating user account: " & ex.Message)
        End Try

        Try
            pBody = "Hi " & pDrEmployee("FirstName") & "," & vbCrLf
            pBody &= vbCrLf
            pBody &= "Please click the link below to create your login account" & vbCrLf
            pBody &= Common.VirtualPath & "/AddUser?k=" & pLink
            SendMail("MobizAccess@GMail.com", "", "Mobiz Access", "Welcome to Mobiz", pBody, "smtp.gmail.com", 587, "mobizaccess@gmail.com", "123mobizaccess456", , , , , , pEmail)
            pOut = New JsonDictionary
            pOut.Add("Result", "ok")
            pOut.Add("Message", "Link was successfully emailed")
            Return pOut.Serialize
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", "An error occured while sending email: " & ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function Apply() As String

        Dim pLink As String
        Dim pUsername As String
        Dim pPassword As String
        Dim pError As String

        Try
            pLink = Common.WebRequest.Data("Link")
            If CStr2(pLink).Length <> 30 Then
                Return JsonDictionary.Serialize("Error", "Invalid Link")
            End If

            pUsername = Common.WebRequest.Data("Username")
            If CStr2(pUsername).Length = 0 _
                OrElse pUsername.Length > 20 Then

                Return JsonDictionary.Serialize("Error", "Invalid Username")
            End If

            pPassword = Common.WebRequest.Data("Password")
            If CStr2(pPassword).Length = 0 _
                OrElse pPassword.Length > 20 Then

                Return JsonDictionary.Serialize("Error", "Invalid Password")
            End If

            If WebRequest.Instance.Business.MainSecurity.Apply(pLink, pUsername, pPassword, pError) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If
        Catch ex As System.Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function
End Class
