Namespace WebProj
    Public Class KnowledgeBaseController
        Inherits System.Web.Mvc.Controller

        Dim clsKb As New KnowledgeBase
        Dim pSourceURL As String = Common.ServerPath & "\KBPictures"
        Dim pFilename As String
        '
        ' GET: /Inventory Form

        Function Index() As ActionResult
            Return View()
        End Function

        Function ByTopic() As ActionResult
            Return View()
        End Function

        Function AllTopics() As ActionResult
            Return View()
        End Function

        Function GeneratePDFKb() As ActionResult
            Return View()
        End Function

        Function CountLinkedProblem()
            ValidateOrg()

            Try
                Dim type As String = Common.WebRequest.Data("Type")
                Dim pID As String = Common.WebRequest.Data("ID")
                Dim pSql As String

                If type = "topic" Then
                    pSql = "SELECT ProblemID FROM LibraryKnowledgeManagementArticleTopics WHERE TopicID = " & ReadyString(pID) & " GROUP BY ProblemID "
                Else
                    pSql = "SELECT ProblemID FROM LibraryKnowledgeManagementArticleTopics WHERE ProductID = " & ReadyString(pID) & " GROUP BY ProblemID "
                End If

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql).Rows.Count)
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function Remove()
            ValidateOrg()

            Try
                Dim type As String = Common.WebRequest.Data("Type")
                Dim pID As String = Common.WebRequest.Data("ID")
                Dim pSql As String

                pSql = "DELETE FROM LibraryKnowledgeManagement WHERE ID = " & ReadyString(pID)

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                If type = "topic" Then
                    pSql = "DELETE FROM LibraryKnowledgeManagementArticleTopics WHERE TopicID = " & ReadyString(pID)
                    Common.WebRequest.ProData.Execute(pSql)

                    'remove also the subtopics
                    pSql = "DELETE FROM LibraryKnowledgeManagement WHERE TopicID = " & ReadyString(pID)
                    Common.WebRequest.ProData.Execute(pSql)
                ElseIf type = "product" Then
                    pSql = "DELETE FROM LibraryKnowledgeManagementArticleTopics WHERE ProductID = " & ReadyString(pID)
                    Common.WebRequest.ProData.Execute(pSql)
                ElseIf type = "subtopic" Then
                    pSql = "DELETE FROM LibraryKnowledgeManagementArticleTopics WHERE SubTopicID = " & ReadyString(pID)
                    Common.WebRequest.ProData.Execute(pSql)
                End If
                
                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "removed")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveArticle() As String
            ValidateOrg()

            clsKb.arrArticle = Common.WebRequest.AllData("Articles")
            clsKb.ShortLink = GenerateShortLink()
            Return JsonDictionary.Serialize("Results", clsKb.SaveArticle())
        End Function

        Function GetQuestions() As String
            ValidateOrg()

            Dim pSql As String = "SELECT ID, Problem FROM TransKnowledgeManagement WHERE Topic = " & ReadyString(Common.WebRequest.Data("TopicID")) & vbCrLf &
                                " AND SubTopic = " & ReadyString(Common.WebRequest.Data("SubTopicID"))

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function CheckShortLink() As Object
            ValidateOrg()

            Dim pSql As String
            Dim tlink As String = Common.WebRequest.Data("tlink")
            Dim alink As String = Common.WebRequest.Data("alink")
            Dim pDTTopic As DataTable
            Dim pDTAnswer As DataTable

            pSql = "SELECT ID FROM TransKnowledgeManagement WHERE ShortLink = " & ReadyString(tlink) & vbCrLf &
                    " COLLATE Latin1_General_CS_AS "
            pDTTopic = Common.WebRequest.ProData.Execute(pSql)

            pSql = "SELECT ID FROM TransKnowledgeManagement WHERE ShortLink = " & ReadyString(alink) & vbCrLf &
                    " COLLATE Latin1_General_CS_AS "
            pDTAnswer = Common.WebRequest.ProData.Execute(pSql)

            If pDTTopic.Rows.Count > 0 And pDTAnswer.Rows.Count > 0 Then
                Return Json(New With {Key .Result = "ok",
                                            .t = pDTTopic.Rows(0)("ID").ToString(),
                                            .a = pDTAnswer.Rows(0)("ID").ToString()})
            End If

            If pDTTopic.Rows.Count > 0 Then
                Return Json(New With {Key .Result = "ok",
                                            .t = pDTTopic.Rows(0)("ID").ToString()})
            End If

            If pDTTopic.Rows.Count > 0 And alink <> "" Then
                Return Json(New With {Key .Result = "ok",
                                            .t = pDTTopic.Rows(0)("ID").ToString(),
                                            .a = alink.ToString()})
            End If

            Return Json(New With {Key .Result = "Sorry! No matching keywords found."})
        End Function

        Function SaveArrangement() As String
            ValidateOrg()

            clsKb.ServerUrl = Common.WebRequest.Data("ServerUrl")
            clsKb.arrReadingList = Common.WebRequest.AllData("data")

            Return JsonDictionary.Serialize("Results", clsKb.SaveArrangement())
        End Function

        Function GeneralSettings()
            ValidateOrg()

            Dim pOut As New JsonDictionary

            pOut.Add("ServerUrl", WebRequest.Instance.Business.SystemDefaults.KBServerUrl)

            Return pOut.Serialize()
        End Function

        Function ArrangeList() As Object
            ValidateOrg()

            clsKb.IsArrangeReadingList = True

            Return clsKb.GetStartupData()
        End Function

        Function GetTopicCases() As Object
            ValidateOrg()

            clsKb.TopicID = Common.WebRequest.Data("TopicID")
            clsKb.ProductID = Common.WebRequest.Data("ProductID")

            Return clsKb.GetTopicCases()
        End Function

        Function GetStartupData() As Object
            ValidateOrg()

            Return clsKb.GetStartupData()
        End Function

        Function UploadPicture(ByVal fileStepPic As HttpPostedFileBase, ByVal OrgID As String, ByVal caretpos As Integer, ByVal imgid As String) As Object
            ValidateOrg()


            Dim p As New InsertUpdate("LibraryKnowledgeManagementPictures")

            Try
                Dim filetype As Object = Split(fileStepPic.ContentType, "/")

                'pSourceURL = pSourceURL & "\" & OrgID

                If Not System.IO.File.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                pFilename = pSourceURL & "\" & imgid & "." & filetype(1)

                'If Not System.IO.Directory.Exists(pSourceURL) Then
                '    IO.Directory.CreateDirectory(pSourceURL)
                'End If

                fileStepPic.SaveAs(pFilename)

                'p("ID") = ReadyString(imgid)
                'p("ParentID") = ReadyString(stepid)
                'p("CaretPos") = CInt2(caretpos)
                'Common.WebRequest.ProData.ExecuteSQL(p.GetInsert())

                Return Json(New With {Key .Location = pFilename,
                                .CaretPos = caretpos,
                                .ImgID = imgid,
                                .Status = "add"
                            })
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

        Function GetLinkedDetails() As Object
            ValidateOrg()

            clsKb.TopicID = Common.WebRequest.Data("t")
            clsKb.AnswerID = Common.WebRequest.Data("a")

            Return clsKb.GetLinkedDetails()
        End Function

        Function ContentCreator() As ActionResult
            Return View()
        End Function

        Function RemoveCase() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable

            Try
                pSql = "SELECT * FROM TransKnowledgeManagement WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ID"))
                pDt = Common.WebRequest.ProData.Execute(pSql)

                If pDt.Rows.Count > 0 Then
                    For Each sol As Object In pDt.Rows
                        pSql = "DELETE FROM TransKnowledgeManagement WHERE SolutionID = " & ReadyString(sol("ID"))
                        Common.WebRequest.ProData.Execute(pSql)
                    Next
                End If

                pSql = "DELETE FROM TransKnowledgeManagement WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "DELETE FROM TransKnowledgeManagement WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "removed")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function GetSearchResult() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable

            clsKb.PageSearchText = Common.WebRequest.Data("SearchKey")
            clsKb.Filter = Common.WebRequest.Data("Filter")
            clsKb.PageColumnNo = "2" 'default to topic
            clsKb.IsCountPage = True

            'pSql = "SELECT * FROM (Select t2.* , ROW_NUMBER() OVER (ORDER BY t2.ProblemID ASC) AS RowNum From(SELECT * FROM (Select DISTINCT " & vbCrLf &
            '        "mkm.ID as ProblemID, mkm.CaseNo, ISNULL(mkm.SubTopic, '') as SubTopicID, mkm.Problem,  " & vbCrLf &
            '        "lkm1.Topic, lkm1.ID as TopicID, lkm2.Product, lkm2.ID as ProductID, lkm3.Language, lkm3.ID as LanguageID, mkm.PeopleID, " & vbCrLf &
            '        "mkm.Tags, CONVERT(VARCHAR(10),mkm.Lastupdated,110) LastUpdated,  " & vbCrLf &
            '        "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName EditedBy,  " & vbCrLf &
            '        "(SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ km2.Solution " & vbCrLf &
            '        "       FROM TransKnowledgeManagement as km2 " & vbCrLf &
            '        "       WHERE km2.ProblemID = mkm.ID " & vbCrLf &
            '        "   FOR XML PATH('')" & vbCrLf &
            '        "), 1, 1,'') FROM TransKnowledgeManagement) as Solution " & vbCrLf &
            '        "from TransKnowledgeManagement as mkm " & vbCrLf &
            '        "inner join MobizLibraryPeople on MobizLibraryPeople.ID = mkm.PeopleID  " & vbCrLf &
            '        "inner join LibraryKnowledgeManagement lkm1 on mkm.Topic = lkm1.ID AND lkm1.Topic IS NOT NULL " & vbCrLf &
            '        "inner join LibraryKnowledgeManagement lkm2 on mkm.Product = lkm2.ID AND lkm2.Product IS NOT NULL " & vbCrLf &
            '        "inner join LibraryKnowledgeManagement lkm3 on mkm.Language = lkm3.ID AND lkm3.Language IS NOT NULL) " & vbCrLf &
            '        "as t1 " & vbCrLf &
            '        "WHERE Problem LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Topic LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Product LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " " & vbCrLf &
            '        "OR Language LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Tags LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Solution LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & ") " & vbCrLf &
            '        "as t2) as t2 WHERE RowNum >= 1 AND RowNum <= 10 ORDER BY Problem ASC"

            'Try
            '    pDt = Common.WebRequest.ProData.Execute(pSql)
            'Catch ex As Exception
            '    AppendToLog(ex.Message)
            'End Try

            pDt = clsKb.GetAllKnowledgeManagementAndQuestionaire()

            Return JsonDictionary.Serialize("Results", pDt)

        End Function

        <HttpPost()>
        Function GetAllKnowledgeManagementAndQuestionaire() As Object
            ValidateOrg()

            Dim strReturn As String
            Dim pDT As DataTable
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsKb.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsKb.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsKb.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsKb.IsCountPage = False
            clsKb.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsKb.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            clsKb.Status = Request.Form.GetValues("columns[0][search][value]")(0).ToString()
            clsKb.Filter = Request.Form.GetValues("columns[1][search][value]")(0).ToString()
            pDT = clsKb.GetAllKnowledgeManagementAndQuestionaire()

            clsKb.IsCountPage = True
            Dim pDTCountPage As DataTable = clsKb.GetAllKnowledgeManagementAndQuestionaire() 'Common.WebRequest.ProData.Execute(pSql)

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsKb.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)

        End Function

        <HttpPost()>
        Function GetProblemSolution() As Object
            ValidateOrg()

            Dim pSql As String
            Dim pDtSolution As DataTable
            Dim pDtStep As DataTable
            Dim pDtImages As DataTable
            Dim pDtAllowedTeam As DataTable

            pSql = "SELECT ID as SolutionID, Solution, IsPublic, OrderNo, ShortLink FROM TransKnowledgeManagement " & vbCrLf &
                    "WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ProblemID")) & vbCrLf &
                    "  ORDER BY OrderNo Asc "

            Try
                pDtSolution = Common.WebRequest.ProData.Execute(pSql)

                pSql = "SELECT TopicID, SubTopicID FROM LibraryKnowledgeManagementArticleTopics " & vbCrLf &
                        " WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ProblemID"))
                Dim pDTArticle As DataTable = Common.WebRequest.ProData.Execute(pSql)

                pSql = "SELECT DISTINCT ProductID as ID FROM LibraryKnowledgeManagementArticleTopics " & vbCrLf &
                        " WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ProblemID"))
                Dim pDTProduct As DataTable = Common.WebRequest.ProData.Execute(pSql)

                If pDtSolution.Rows.Count > 0 Then
                    Dim solObj(pDtSolution.Rows.Count - 1) As Object

                    For i As Integer = 0 To pDtSolution.Rows.Count - 1
                        pSql = "SELECT ID as StepID, Steps as Step, OrderNo, ShortLink FROM TransKnowledgeManagement " & vbCrLf &
                                "WHERE SolutionID = " & ReadyString(pDtSolution.Rows(i)("SolutionID").ToString()) & vbCrLf &
                                " AND Steps <> '' ORDER BY OrderNo ASC"
                        pDtStep = Common.WebRequest.ProData.Execute(pSql)

                        Dim stepObj(pDtStep.Rows.Count - 1) As Object

                        If pDtStep.Rows.Count > 0 Then
                            For m As Integer = 0 To pDtStep.Rows.Count - 1
                                pSql = "SELECT * FROM LibraryKnowledgeManagementPictures " & vbCrLf &
                                "WHERE ParentID = " & ReadyString(pDtStep.Rows(m)("StepID").ToString())
                                pDtImages = Common.WebRequest.ProData.Execute(pSql)

                                Dim imgObj(pDtImages.Rows.Count - 1) As Object

                                If pDtImages.Rows.Count > 0 Then
                                    For n As Integer = 0 To pDtImages.Rows.Count - 1
                                        pFilename = pDtImages.Rows(n)("ID").ToString() & "." & pDtImages.Rows(n)("Type").ToString()
                                        imgObj(n) = New With {Key .ImgID = pDtImages.Rows(n)("ID").ToString(), .CaretPos = pDtImages.Rows(n)("CaretPos").ToString(), .StepID = pDtImages.Rows(n)("ParentID").ToString(), .Status = "old", .Location = pFilename}
                                    Next
                                End If
                                stepObj(m) = New With {Key .StepID = pDtStep.Rows(m)("StepID").ToString(),
                                                       .No = pDtStep.Rows(m)("OrderNo").ToString(),
                                                       .StepShortLink = pDtStep.Rows(m)("ShortLink").ToString(),
                                                       .Step = pDtStep.Rows(m)("Step").ToString(),
                                                       .State = "Edit",
                                                       .Images = imgObj}
                            Next
                        End If

                        pSql = "SELECT TransKnowledgeManagement.ID, LibraryKnowledgeManagement.TeamName, LibraryKnowledgeManagement.ID as TeamID FROM TransKnowledgeManagement " & vbCrLf &
                                " INNER JOIN LibraryKnowledgeManagement " & vbCrLf &
                                "   ON LibraryKnowledgeManagement.ID = TransKnowledgeManagement.TeamID " & vbCrLf &
                                "WHERE SolutionID = " & ReadyString(pDtSolution.Rows(i)("SolutionID").ToString()) & vbCrLf &
                                " AND convert(varchar(MAX), TeamID) <> ''"
                        pDtAllowedTeam = Common.WebRequest.ProData.Execute(pSql)

                        Dim stepAllowedTeam(pDtAllowedTeam.Rows.Count - 1) As Object

                        If pDtAllowedTeam.Rows.Count > 0 Then
                            For m As Integer = 0 To pDtAllowedTeam.Rows.Count - 1
                                stepAllowedTeam(m) = New With {Key .ID = pDtAllowedTeam.Rows(m)("ID").ToString(), .TeamID = pDtAllowedTeam.Rows(m)("TeamID").ToString(), .No = i + 1, .Name = pDtAllowedTeam.Rows(m)("TeamName").ToString(), .State = "Edit"}
                            Next
                        End If

                        solObj(i) = New With {Key .SolutionID = pDtSolution.Rows(i)("SolutionID").ToString(),
                                              .IsPublic = If(pDtSolution.Rows(i)("IsPublic").ToString() = "False", "False", "True"),
                                              .No = pDtSolution.Rows(i)("OrderNo"),
                                              .Solution = pDtSolution.Rows(i)("Solution").ToString(),
                                              .SolutionShortLink = pDtSolution.Rows(i)("ShortLink").ToString(),
                                              .State = "Edit", .Step = stepObj,
                                              .AllowedTeam = stepAllowedTeam,
                                              .OrgID = WebRequest.Instance.Data("OrgID"),
                                              .Topics = jss.DeserializeObject(DataTableToJSON.GetJson(pDTArticle)),
                                              .Products = jss.DeserializeObject(DataTableToJSON.GetJson(pDTProduct))
                                             }
                    Next

                    Dim serializer As New JavaScriptSerializer()
                    Return Json(New With {Key .Topics = jss.DeserializeObject(DataTableToJSON.GetJson(pDTArticle)),
                                        .Products = jss.DeserializeObject(DataTableToJSON.GetJson(pDTProduct)),
                                        .Rows = solObj
                            })
                End If


            Catch ex As Exception
                AppendToLog(ex.Message)
                Return ex.Message
            End Try

        End Function

        <HttpPost()>
        Function GetLastKMNumber() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable

            pSql = "Select ISNULL(MAX(CaseNo),0) + 1  LastNumber from TransKnowledgeManagement "

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)
        End Function

        <HttpPost()>
        Function SaveCase() As String

            ValidateOrg()

            Dim p As New InsertUpdate("TransKnowledgeManagement")
            Dim problemID As String = ""
            Dim pSql As String = "[query here]"
            Dim pDt As DataTable
            Dim Solution() As String

            Try
                p("CaseNo") = CInt2(Common.WebRequest.Data("Case"))
                p("Problem") = ReadyString(CStr2(Common.WebRequest.Data("Case")))
                p("Topic") = CStr2(Common.WebRequest.Data("Topic"))
                p("SubTopic") = CStr2(Common.WebRequest.Data("SubTopic"))
                p("Product") = CStr2(Common.WebRequest.Data("Product"))
                p("Language") = ReadyString(CStr2(Common.WebRequest.Data("Language")))
                p("PeopleID") = ReadyString(CStr2(Common.WebRequest.Data("ModifiedBy")))
                p("Tags") = ReadyString(CStr2(Common.WebRequest.Data("Tags")))
                p("LastUpdated") = ReadyString(CDate(Common.WebRequest.Data("Date")))

                If CStr2(Common.WebRequest.Data("ProblemID")) = "" Then
                    problemID = ReadyString(Guid.NewGuid.ToString())
                    p("ID") = problemID
                    p("ShortLink") = ReadyString(GenerateShortLink())
                    pSql = p.GetInsert()
                Else
                    If CStr2(Common.WebRequest.Data("ProblemShortLink")) = "" Then
                        p("ShortLink") = ReadyString(GenerateShortLink())
                    End If

                    problemID = ReadyString(Common.WebRequest.Data("ProblemID"))
                    pSql = p.GetUpdate(" ID = " & problemID)

                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                For Each product As Object In Common.WebRequest.AllData("Products")
                    Dim excludedidstobedeleted As String 'dili iapil ang mga naka checked na topic and subtopic sa pagdelete, ang mga idelete na data kay katong mga gipang unchecked sa user na topic or subtopic
                    For Each top As Object In Common.WebRequest.AllData("ArticleTopic")
                        p = New InsertUpdate("LibraryKnowledgeManagementArticleTopics")

                        For Each subtop As Object In top("SubTopic")
                            'CHECK IF THERE'S EXISTING RECORD
                            pSql = "SELECT ID FROM LibraryKnowledgeManagementArticleTopics " & vbCrLf &
                                " WHERE ProblemID = " & problemID & " AND TopicID = " & ReadyString(top("TopicID")) & vbCrLf &
                                " AND SubTopicID = " & ReadyString(subtop("ID")) & vbCrLf &
                                " AND ProductID = " & ReadyString(product("ID")) & vbCrLf
                            Dim len As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                            If len.Count = 0 Then
                                Dim id As String = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(id)
                                p("ProblemID") = problemID
                                p("TopicID") = ReadyString(top("TopicID"))
                                p("SubTopicID") = ReadyString(subtop("ID"))
                                p("ProductID") = ReadyString(product("ID"))
                                p("ModifiedByID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("LastUpdated") = "getdate()"
                                Common.WebRequest.ProData.Execute(p.GetInsert())

                                excludedidstobedeleted += ReadyString(id) & ","
                            Else
                                excludedidstobedeleted += ReadyString(len(0)("ID")) & ","
                            End If
                        Next subtop
                    Next top

                    pSql = "DELETE FROM LibraryKnowledgeManagementArticleTopics WHERE ProblemID = " & problemID & vbCrLf &
                        " AND ID NOT IN (" & excludedidstobedeleted.Trim().Substring(0, excludedidstobedeleted.Length - 1) & ")"
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Next product

                If CStr2(Common.WebRequest.AllData("Solution")) = "System.Object[]" Then
                    Dim solutionOrderNo As Integer = 1

                    For Each solutions As Object In Common.WebRequest.AllData("Solution")
                        Dim solID As String
                        Dim stepOrderNo As Integer = 1
                        p = New InsertUpdate("TransKnowledgeManagement")
                        p("Solution") = ReadyString(CStr2(solutions("Solution")))
                        p("ProblemID") = problemID
                        p("IsPublic") = ReadyString(CBool(solutions("IsPublic")))
                        p("OrderNo") = CInt2(solutions("No"))

                        If CStr2(solutions("State")) = "Add" Then
                            p("ShortLink") = ReadyString(GenerateShortLink())
                            solID = ReadyString(Guid.NewGuid.ToString())
                            p("ID") = solID
                            pSql = p.GetInsert()
                        ElseIf CStr2(solutions("State")) = "Edit" Then
                            If CStr2(solutions("SolutionShortLink")) = "" Then
                                p("ShortLink") = ReadyString(GenerateShortLink())
                            End If
                            solID = ReadyString(solutions("SolutionID"))
                            pSql = p.GetUpdate(" ID = " & solID)
                        Else
                            pSql = "DELETE FROM TransKnowledgeManagement WHERE ID = " & ReadyString(solutions("SolutionID"))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        If CStr2(solutions("State")) = "Remove" Then
                            pSql = "DELETE FROM TransKnowledgeManagement WHERE SolutionID = " & ReadyString(solutions("SolutionID"))
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Else
                            For Each steps As Object In solutions("Step")
                                Dim stepID As String
                                p = New InsertUpdate("TransKnowledgeManagement")
                                p("SolutionID") = solID
                                p("Steps") = ReadyString(CStr2(steps("Step")))
                                p("OrderNo") = ReadyString(CInt(steps("No")))

                                If CStr2(steps("State")) = "Add" Then
                                    stepID = ReadyString(CStr2(steps("StepID")))
                                    p("ID") = stepID
                                    pSql = p.GetInsert()
                                ElseIf CStr2(steps("State")) = "Edit" Then
                                    stepID = ReadyString(steps("StepID"))
                                    pSql = p.GetUpdate(" ID = " & stepID)
                                Else
                                    pSql = "DELETE FROM TransKnowledgeManagement WHERE ID = " & ReadyString(steps("StepID"))
                                End If

                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                Try
                                    If steps.ContainsKey("Images") = True Then
                                        p = New InsertUpdate("LibraryKnowledgeManagementPictures")

                                        For Each image As Object In steps("Images")
                                            Dim filetype As Object = Split(image("Location"), ".")
                                            p("ID") = ReadyString(image("ImgID"))
                                            p("ParentID") = stepID
                                            p("CaretPos") = CInt2(image("CaretPos"))
                                            p("Location") = ReadyString(image("Location"))
                                            p("Type") = ReadyString(filetype(1))

                                            If image("Status") = "new" Then
                                                pSql = p.GetInsert()
                                            ElseIf image("Status") = "delete" Then
                                                pSql = "DELETE FROM LibraryKnowledgeManagementPictures WHERE ID = " & ReadyString(image("ImgID"))
                                            End If

                                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                                        Next
                                    End If
                                Catch ex As Exception
                                    AppendToLog(ex.Message)
                                End Try

                                stepOrderNo += 1
                            Next steps

                            If CStr2(solutions("IsPublic")) = "True" Then
                                If solutions("SolutionID") <> Nothing Then
                                    pSql = "DELETE FROM TransKnowledgeManagement WHERE SolutionID = " & ReadyString(solutions("SolutionID")) & " AND CONVERT(varchar(MAX), TeamID) <> ''"
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If
                            Else
                                For Each team As Object In solutions("AllowedTeam")
                                    Dim stepID As String
                                    p = New InsertUpdate("TransKnowledgeManagement")
                                    p("SolutionID") = solID
                                    p("TeamID") = ReadyString(CStr2(team("TeamID")))

                                    If CStr2(team("State")) = "Add" Then
                                        stepID = ReadyString(Guid.NewGuid.ToString())
                                        p("ID") = stepID
                                        pSql = p.GetInsert()
                                    ElseIf CStr2(team("State")) = "Edit" Then
                                        stepID = ReadyString(team("ID"))
                                        pSql = p.GetUpdate(" ID = " & stepID)
                                    Else
                                        pSql = "DELETE FROM TransKnowledgeManagement WHERE ID = " & ReadyString(team("ID"))
                                    End If

                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                Next team
                            End If
                        End If

                        solutionOrderNo += 1
                    Next solutions
                End If

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "True")
            Catch ex As Exception
                AppendToLog(ex.Message)
                AppendToLog(pSql)
                Return ex.Message
            End Try

            'If (CStr2(Common.WebRequest.Data("Steps")).Length > 0) Then
            '    Solution = CStr2(Common.WebRequest.Data("Steps")).Split(New String() {vbLf, vbCr, vbCrLf},
            '                           StringSplitOptions.None)
            'End If

            'Try

            '    If Not Solution Is Nothing Then
            '        For i As Integer = 0 To UBound(Solution)
            '            pSql = "Insert into TransKnowledgeManagement(CaseNo, CaseDescription, Solution, Steps, Topic, Product, Language, PeopleID, Tags) values(" & vbCrLf &
            '            CInt2(Common.WebRequest.Data("CaseID")) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Case"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Solution"))) & "," & vbCrLf &
            '            ReadyString(Solution(i)) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Topic"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Product"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Language"))) & "," & vbCrLf &
            '            ReadyString(CStr2(WebSession.Instance.PeopleUserID)) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Tags"))) & ")"

            '            Common.WebRequest.ProData.Execute(pSql)
            '        Next
            '    Else
            '        pSql = "Insert into TransKnowledgeManagement(CaseNo, CaseDescription, Solution, Steps, Topic, Product, Language, PeopleID, Tags) values(" & vbCrLf &
            '            CInt2(Common.WebRequest.Data("CaseID")) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Case"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Solution"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Steps"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Topic"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Product"))) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Language"))) & "," & vbCrLf &
            '            ReadyString(CStr2(WebSession.Instance.PeopleUserID)) & "," & vbCrLf &
            '            ReadyString(CStr2(Common.WebRequest.Data("Tags"))) & ")"

            '        Common.WebRequest.ProData.Execute(pSql)
            '    End If

            'Catch ex As Exception
            '    Return JsonDictionary.Serialize("Results", "Error")
            '    AppendToLog(ex.Message)
            'End Try

            'Return JsonDictionary.Serialize("Results", "True")
        End Function

        Public Function GenerateShortLink() As String
            Dim rand As New Random()
            Dim allowableChars() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
            Dim link As String

            For i As Integer = 0 To 10 - 1
                link += allowableChars(rand.Next(allowableChars.Length - 1))
            Next

            'CHECK IF GENERATED LINK EXIST
            Dim pSql As String = "SELECT ShortLink FROM TransKnowledgeManagement WHERE ShortLink = " & ReadyString(link)
            Dim transKB As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

            'pSql = "SELECT ShortLink FROM LibraryKnowledgeManagementArticle WHERE ShortLink = " & ReadyString(link)
            'Dim libArticle As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

            If transKB > 0 Then 'Or libArticle > 0
                link = GenerateShortLink() 'replace the old link with the new generated link
            End If

            Return link
        End Function

        Function GetAllTopics() As Object
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable
            Dim arrTopic As New List(Of Dictionary(Of String, Object))

            Try
                pSql = "Select ID, Topic from LibraryKnowledgeManagement where Topic IS NOT NULL ORDER BY Topic ASC"

                If Common.WebRequest.Data("keywords") <> "" Then
                    pSql += " AND Topic LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))
                    pDt = Common.WebRequest.ProData.Execute(pSql)
                Else
                    pDt = Common.WebRequest.ProData.Execute(pSql)

                    For i As Integer = 0 To pDt.Rows.Count - 1
                        Dim topic As Object = pDt.Rows(i)
                        Dim pTopic As New Dictionary(Of String, Object)

                        pSql = "Select ID, SubTopic from LibraryKnowledgeManagement where SubTopic IS NOT NULL AND TopicID = " & ReadyString(topic("ID")) & vbCrLf &
                                " ORDER BY SubTopic ASC "
                        Dim pDtSubTopic As DataTable = Common.WebRequest.ProData.Execute(pSql)

                        pTopic.Add("ID", topic("ID"))
                        pTopic.Add("Topic", topic("Topic"))
                        pTopic.Add("SubTopic", jss.DeserializeObject(DataTableToJSON.GetJson(pDtSubTopic)))
                        arrTopic.Add(pTopic)

                    Next
                End If
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return Json(New With {Key .Results = arrTopic})
        End Function

        <HttpPost()>
        Function GetAllSubTopics(Optional ByVal TopicID As String = "")
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable
            Dim arrTopic As New List(Of Dictionary(Of String, Object))
            TopicID = IIf(TopicID = "", CStr2(Common.WebRequest.Data("TopicID")), TopicID)

            pSql = "Select LibraryKnowledgeManagement.ID, SubTopic from LibraryKnowledgeManagement " & vbCrLf &
                    " LEFT JOIN LibraryKnowledgeManagementOrdering as lkmo ON lkmo.SubTopicID = LibraryKnowledgeManagement.ID " & vbCrLf &
                    " where SubTopic IS NOT NULL " & IIf(TopicID = "", "", " AND LibraryKnowledgeManagement.TopicID = " & ReadyString(TopicID)) & vbCrLf &
                    " ORDER BY lkmo.OrderNo ASC "

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)

                For i As Integer = 0 To pDt.Rows.Count - 1
                    Dim topic As Object = pDt.Rows(i)
                    Dim pTopic As New Dictionary(Of String, Object)

                    pTopic.Add("ID", topic("ID"))
                    pTopic.Add("SubTopic", topic("SubTopic"))

                    If CBool2(Common.WebRequest.Data("IncludeRelatedQuestions")) Then
                        pSql = "Select trans.ID as ProblemID, Problem from TransKnowledgeManagement as trans " & vbCrLf &
                                " inner join LibraryKnowledgeManagementArticleTopics as lib ON lib.ProblemID = trans.ID" & vbCrLf &
                                " where lib.SubTopicID = " & ReadyString(topic("ID")) & vbCrLf &
                                " ORDER BY trans.OrderNo ASC "
                        Dim pDtQuestions As DataTable = Common.WebRequest.ProData.Execute(pSql)
                        pTopic.Add("Questions", jss.DeserializeObject(DataTableToJSON.GetJson(pDtQuestions)))
                    End If

                    arrTopic.Add(pTopic)
                Next
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return Json(New With {Key .Results = arrTopic})
        End Function

        <HttpPost()>
        Function GetAllProducts() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable

            pSql = "Select ID, Product from LibraryKnowledgeManagement where Product IS NOT NULL ORDER BY Product ASC"

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)
        End Function

        <HttpPost()>
        Function GetAllLanguages() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable

            pSql = "Select ID, Language from LibraryKnowledgeManagement where Language IS NOT NULL"

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)
        End Function

        <HttpPost()>
        Function SaveTopic() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            Try
                Dim p As New InsertUpdate("LibraryKnowledgeManagement")

                p("Topic") = ReadyString(CStr2(Common.WebRequest.Data("Description")))

                If CBool2(Common.WebRequest.Data("IsEdit")) Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(CStr2(Common.WebRequest.Data("ID"))))
                Else
                    p("ID") = ReadyString(CStr2(Common.WebRequest.Data("ID")))
                    pSql = p.GetInsert()
                End If

                Common.WebRequest.ProData.Execute(pSql)

            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", "Error")
            End Try

            Return JsonDictionary.Serialize("Results", "True")
        End Function

        <HttpPost()>
        Function SaveSubTopic() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String
            Dim p As New InsertUpdate("LibraryKnowledgeManagement")
            Try
                p("SubTopic") = ReadyString(CStr2(Common.WebRequest.Data("Description")))
                p("TopicID") = ReadyString(CStr2(Common.WebRequest.Data("TopicID")))

                If CBool2(Common.WebRequest.Data("IsEdit")) Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(CStr2(Common.WebRequest.Data("ID"))))
                Else
                    p("ID") = ReadyString(CStr2(Common.WebRequest.Data("ID")))
                    pSql = p.GetInsert()
                End If

                Common.WebRequest.ProData.Execute(pSql)

            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "Error")
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", "True")
        End Function

        <HttpPost()>
        Function SaveProduct() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            Try

                pSql = "Insert into LibraryKnowledgeManagement(ID, Product) values(" & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("ID"))) & "," & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("Description"))) & ")"

                Common.WebRequest.ProData.Execute(pSql)

            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "Error")
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", "True")
        End Function

        <HttpPost()>
        Function SaveLanguage() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            Try

                pSql = "Insert into LibraryKnowledgeManagement(ID, Language) values(" & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("ID"))) & "," & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("Description"))) & ")"

                Common.WebRequest.ProData.Execute(pSql)

            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "Error")
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", "True")
        End Function

        <HttpPost()>
        Function SaveNewTeam() As String
            ValidateOrg()

            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            Try

                pSql = "Insert into LibraryKnowledgeManagement(ID, TeamName, LastUpdated) values(" & vbCrLf &
                ReadyString(Guid.NewGuid.ToString) & "," & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("TeamName"))) & "," & vbCrLf &
                ReadyString(CDate(Common.WebRequest.Data("DateNow"))) & ")"

                Common.WebRequest.ProData.Execute(pSql)

            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", "True")
        End Function

        Function GetCompanyName()
            ValidateOrg()

            Return JsonDictionary.Serialize("Results", WebRequest.Instance.Business.LibraryOrganization.ByID(Common.WebRequest.Data("OrgID"))("CompanyName"))
        End Function

        Function ValidateOrg() As String
            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End Function
    End Class
End Namespace