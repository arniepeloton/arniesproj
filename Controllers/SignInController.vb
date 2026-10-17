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
#If DEBUG Then
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.Cache.SetNoStore()
            'Response.Redirect("/Signin/KB")
            'Response.Redirect("/Signin/CRM")
            Return View()
#Else
            Response.Redirect("/Signin/CRM")
            'Response.Redirect("/Signin/KB")
#End If

        End Function

        Function Project() As ActionResult
            Return View()
        End Function

        Function CRM() As ActionResult
            Return View()
        End Function

        Function KB() As ActionResult
            Return View()
        End Function

        Function ERP() As ActionResult
            If CBool2(FileText(Common.ServerPath & "settings\AllowEas.txt")) = False Then
                Response.Redirect("/Signin/" & Request.Url.Query)
            End If
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

        Function CustomerDisplay() As ActionResult
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

        Function CheckTwoFA()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim isTwoFA As Boolean = CBool2(WebRequest.Instance.Business.SystemDefaults.Enable2FA)

                pOut.Add("TwoFA", isTwoFA)

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SendTwoFA()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim peopleid As String = Common.WebRequest.Data("PeopleID")

                pSql = " SELECT EmailAddress = STUFF(( " & vbCrLf &
                                "   SELECT ', ' + EmailAddress " & vbCrLf &
                                "   FROM MobizLibraryPeopleContactEmail " &
                                "   WHERE PeopleID = " & ReadyString(peopleid) & vbCrLf &
                                "   FOR XML PATH('') ), 1, 1, '') " & vbCrLf &
                                " FROM MobizLibraryPeopleContactEmail " & vbCrLf &
                                "WHERE PeopleID = " & ReadyString(peopleid)

                Dim pRows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If pRows.Count > 0 Then
                    Dim code As Integer = New System.Random().Next(100000, 999999)
                    Dim body As String = "<table style='width:100%; border:none; background-color: #f5f5f5;'>" +
                                "<tr><td style='padding:20px; font-size: 20px;'>Hello " + CStr2(WebSession.Instance.FirstName) + ",<br /><br />Here is your verification code.<br /></td></tr>" +
                                "<tr><td style='padding:20px; font-size: 25px;'>" + CStr2(code) + "<br /></td></tr>" +
                                "<tr><td style='padding:20px; font-size: 20px;'>Please enter the code to verify your identity and sign in. This code is only valid for 5 minutes.<br /></td></tr>" +
                            "</table>"

                    SendMail("customercare@mobiz-apps.com",
                        pRows(0)("EmailAddress").ToString(),
                        "Mobiz",
                        "Two-Factor Authentication",
                        body,
                        "smtp-mail.outlook.com",
                         587,
                         "customercare@mobiz-apps.com",
                         "MobizCustCare2221", , , , , True)

                    Dim p As New InsertUpdate("TransTwoFactorAuthentication")
                    Dim expirationdate As String = Date.Now().AddMinutes(5)
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PeopleID") = ReadyString(peopleid)
                    p("Code") = ReadyString(code)
                    p("Sent") = "getdate()"
                    p("ExpirationDate") = ReadyString(expirationdate)
                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    pSql = " SELECT EmailAddress " & vbCrLf &
                            " FROM MobizLibraryPeopleContactEmail " & vbCrLf &
                            "WHERE PeopleID = " & ReadyString(peopleid)

                    pRows = Common.WebRequest.ProData.Execute(pSql).Rows

                    Dim arrEmailAdd As New List(Of String)

                    For Each email As Object In pRows
                        Dim ea As String = email("EmailAddress").ToString()
                        Dim obfuscated = String.Join("@", ea.Split("@"c).Select(Function(x, n) _
                            If(n = 0, _
                                x.Substring(0, System.Math.Min(x.Length / 2, x.Length)).PadRight(x.Length, "*"c), _
                                x)))

                        arrEmailAdd.Add(obfuscated)
                    Next

                    pOut.Add("EmailAddress", String.Join(", ", arrEmailAdd))
                Else
                    pOut.Add("NoEmailAdd", True)
                End If

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
            
        End Function

        Function VerifyTwoFACode()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim peopleid As String = Common.WebRequest.Data("PeopleID")
                Dim code As String = Common.WebRequest.Data("Code")
                Dim pSql As String

                pSql = "SELECT * FROM TransTwoFactorAuthentication WHERE PeopleID = " & ReadyString(peopleid) & " AND Code = " & ReadyString(code)
                Dim pRows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If pRows.Count > 0 Then
                    If CDate(Date.Now()) >= CDate(pRows(0)("Sent")) And CDate(Date.Now()) <= CDate(pRows(0)("ExpirationDate")) Then
                        'delete all user's 2fa code
                        pSql = "DELETE FROM TransTwoFactorAuthentication WHERE PeopleID = " & ReadyString(peopleid)
                        Common.WebRequest.ProData.Execute(pSql)
                        Return JsonDictionary.Serialize("Valid", True)
                    End If
                End If

                Return JsonDictionary.Serialize("Invalid", True)
            Catch ex As Exception
                AppendToDebug("Verifying 2FA: " & ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function
    End Class
End Namespace