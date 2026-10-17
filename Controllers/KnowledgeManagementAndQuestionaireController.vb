Namespace WebProj
    Public Class KnowledgeManagementAndQuestionaireController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Inventory Form

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost>
        Function GetAllKnowledgeManagementAndQuestionaire() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'pSql = "Select DISTINCT CaseNo, CaseDescription, Solution, Topic, Product, Language, PeopleID, Tags, CONVERT(VARCHAR(10),TransKnowledgeManagement.Lastupdated,110) LastUpdated, MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName EditedBy from TransKnowledgeManagement inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransKnowledgeManagement.PeopleID "
            pSql = "Select DISTINCT " & vbCrLf &
                    "mkm.ID as ProblemID, mkm.CaseNo, ISNULL(mkm.SubTopic, '') as SubTopicID, mkm.Problem,  " & vbCrLf &
                    "lkm1.Topic, lkm1.ID as TopicID, lkm2.Product, lkm2.ID as ProductID, lkm3.Language, lkm3.ID as LanguageID, mkm.PeopleID, " & vbCrLf &
                    "mkm.Tags, CONVERT(VARCHAR(10),mkm.Lastupdated,110) LastUpdated,  " & vbCrLf &
                    "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName EditedBy,  " & vbCrLf &
                    "(SELECT DISTINCT STUFF((SELECT DISTINCT Top 1 ', '+ km2.Solution " & vbCrLf &
                    "       FROM TransKnowledgeManagement as km2 " & vbCrLf &
                    "       WHERE km2.ProblemID = mkm.ID " & vbCrLf &
                    "   FOR XML PATH('')" & vbCrLf &
                    "), 1, 1,'') FROM TransKnowledgeManagement) as Solution " & vbCrLf &
                    "from TransKnowledgeManagement as mkm " & vbCrLf &
                    "inner join MobizLibraryPeople on MobizLibraryPeople.ID = mkm.PeopleID  " & vbCrLf &
                    "inner join LibraryKnowledgeManagement lkm1 on mkm.Topic = lkm1.ID AND lkm1.Topic IS NOT NULL " & vbCrLf &
                    "inner join LibraryKnowledgeManagement lkm2 on mkm.Product = lkm2.ID AND lkm2.Product IS NOT NULL " & vbCrLf &
                    "inner join LibraryKnowledgeManagement lkm3 on mkm.Language = lkm3.ID AND lkm3.Language IS NOT NULL "

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)

        End Function

        <HttpPost()>
        Function GetProblemSolution() As String
            Dim pSql As String
            Dim pDtSolution As DataTable
            Dim pDtStep As DataTable
            Dim pDtAllowedTeam As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT ID as SolutionID, Solution, IsPublic FROM TransKnowledgeManagement " & vbCrLf &
                    "WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ProblemID")) & vbCrLf

            Try
                pDtSolution = Common.WebRequest.ProData.Execute(pSql)

                If pDtSolution.Rows.Count > 0 Then
                    Dim solObj(pDtSolution.Rows.Count - 1) As Object

                    For i As Integer = 0 To pDtSolution.Rows.Count - 1
                        pSql = "SELECT ID as StepID, Steps as Step FROM TransKnowledgeManagement " & vbCrLf &
                                "WHERE SolutionID = " & ReadyString(pDtSolution.Rows(i)("SolutionID").ToString()) & vbCrLf &
                                " AND Steps <> ''"
                        pDtStep = Common.WebRequest.ProData.Execute(pSql)

                        Dim stepObj(pDtStep.Rows.Count - 1) As Object

                        If pDtStep.Rows.Count > 0 Then
                            For m As Integer = 0 To pDtStep.Rows.Count - 1
                                stepObj(m) = New With {Key .StepID = pDtStep.Rows(m)("StepID").ToString(), .No = i + 1, .Step = pDtStep.Rows(m)("Step").ToString(), .State = "Edit"}
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

                        solObj(i) = New With {Key .SolutionID = pDtSolution.Rows(i)("SolutionID").ToString(), .IsPublic = If(pDtSolution.Rows(i)("IsPublic").ToString() = "False", "False", "True"), .No = i + 1, .Solution = pDtSolution.Rows(i)("Solution").ToString(), .State = "Edit", .Step = stepObj, .AllowedTeam = stepAllowedTeam}
                    Next

                    Dim serializer As New JavaScriptSerializer()
                    Return serializer.Serialize(solObj)
                End If


            Catch ex As Exception
                AppendToLog(ex.Message)
                Return ex.Message
            End Try

        End Function

        <HttpPost()>
        Function GetLastKMNumber() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim p As New InsertUpdate("TransKnowledgeManagement")
            Dim problemID As String = ""
            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            Try
                p("CaseNo") = CInt2(Common.WebRequest.Data("Case"))
                p("Problem") = ReadyString(CStr2(Common.WebRequest.Data("Case")))
                p("Topic") = ReadyString(CStr2(Common.WebRequest.Data("Topic")))
                p("SubTopic") = ReadyString(CStr2(Common.WebRequest.Data("SubTopic")))
                p("Product") = ReadyString(CStr2(Common.WebRequest.Data("Product")))
                p("Language") = ReadyString(CStr2(Common.WebRequest.Data("Language")))
                p("PeopleID") = ReadyString(CStr2(WebSession.Instance.PeopleUserID))
                p("Tags") = ReadyString(CStr2(Common.WebRequest.Data("Tags")))

                If CStr2(Common.WebRequest.Data("ProblemID")) = "" Then
                    problemID = ReadyString(Guid.NewGuid.ToString())
                    p("ID") = problemID
                    pSql = p.GetInsert()
                Else
                    problemID = ReadyString(Common.WebRequest.Data("ProblemID"))
                    pSql = p.GetUpdate(" ID = " & problemID)
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                For Each solutions As Object In Common.WebRequest.AllData("Solution")
                    Dim solID As String
                    p = New InsertUpdate("TransKnowledgeManagement")
                    p("Solution") = ReadyString(CStr2(solutions("Solution")))
                    p("ProblemID") = problemID
                    p("IsPublic") = ReadyString(CBool(solutions("IsPublic")))

                    If CStr2(solutions("State")) = "Add" Then
                        solID = ReadyString(Guid.NewGuid.ToString())
                        p("ID") = solID
                        pSql = p.GetInsert()
                    ElseIf CStr2(solutions("State")) = "Edit" Then
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

                            If CStr2(steps("State")) = "Add" Then
                                stepID = ReadyString(Guid.NewGuid.ToString())
                                p("ID") = stepID
                                pSql = p.GetInsert()
                            ElseIf CStr2(steps("State")) = "Edit" Then
                                stepID = ReadyString(steps("StepID"))
                                pSql = p.GetUpdate(" ID = " & stepID)
                            Else
                                pSql = "DELETE FROM TransKnowledgeManagement WHERE ID = " & ReadyString(steps("StepID"))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Next steps

                        If CStr2(solutions("IsPublic")) = "True" Then
                            pSql = "DELETE FROM TransKnowledgeManagement WHERE SolutionID = " & ReadyString(solutions("SolutionID")) & " AND CONVERT(varchar(MAX), TeamID) <> ''"
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
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

                Next solutions

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "True")
            Catch ex As Exception
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

        <HttpPost()>
        Function GetAllTopics() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select ID, Topic from LibraryKnowledgeManagement where Topic IS NOT NULL"

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)
        End Function

        <HttpPost()>
        Function GetAllSubTopics() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select ID, SubTopic from LibraryKnowledgeManagement where SubTopic IS NOT NULL AND TopicID = " & ReadyString(CStr2(Common.WebRequest.Data("TopicID")))

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)
        End Function

        <HttpPost()>
        Function GetAllProducts() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select ID, Product from LibraryKnowledgeManagement where Product IS NOT NULL"

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)
        End Function

        <HttpPost()>
        Function GetAllLanguages() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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
            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try

                pSql = "Insert into LibraryKnowledgeManagement(ID, Topic) values(" & vbCrLf &
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
        Function SaveSubTopic() As String
            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try

                pSql = "Insert into LibraryKnowledgeManagement(ID, SubTopic, TopicID) values(" & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("ID"))) & "," & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("Description"))) & "," & vbCrLf &
                ReadyString(CStr2(Common.WebRequest.Data("TopicID"))) & ")"

                Common.WebRequest.ProData.Execute(pSql)

            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "Error")
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", "True")
        End Function

        <HttpPost()>
        Function SaveProduct() As String
            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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
            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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
            Dim pSql As String
            Dim pDt As DataTable
            Dim Solution() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

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
    End Class
End Namespace