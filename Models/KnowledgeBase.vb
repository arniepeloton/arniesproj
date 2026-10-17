Imports System
Imports System.Threading
Imports System.ComponentModel
Imports System.IO.Ports
Imports Newtonsoft.Json

Public Class KnowledgeBase
    Inherits System.Web.Mvc.Controller

#Region "Variables"
    Public Property PageStart As Integer
    Public Property PageNoOfRows As Integer
    Public Property PageSearchText As String
    Public Property PageColumnNo As String
    Public Property PageColumnDirection As String
    Public Property Filter As String
    Public Property SearchContact As String
    Public Property IsCountPage As String
    Public Property Status As String

    Public Property IsEndUser As Boolean = False

    Public Property ProductID As String
    Public Property TopicID As String
    Public Property AnswerID As String

    Public Property IsArrangeReadingList As Boolean = False

    Public Property arrReadingList As Object
    Public Property arrArticle As Object
    Public Property ShortLink As String
    Public Property ServerUrl As String
#End Region

    Function GetArticle() As Object
        Dim pSql As String
        Dim pDTArticle As DataTable

        pSql = "SELECT * FROM LibraryKnowledgeManagementArticle as article " & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = article.CreatedBy"
        pDTArticle = Common.WebRequest.ProData.Execute(pSql)

        For Each article As Object In pDTArticle.Rows
            pSql = "SELECT * FROM LibraryKnowledgeManagementArticleContents WHERE ArticleID = " & ReadyString(article("ID"))
            Dim pDTContent As DataTable = Common.WebRequest.ProData.Execute(pSql)


        Next article

    End Function

    Function SaveArticle() As String
        Try
            Common.WebRequest.ProData.BeginTransaction()

            For Each article As Object In arrArticle
                Dim p As New InsertUpdate("LibraryKnowledgeManagementArticle")
                Dim articleID As String
                Dim pSql As String

                p("ArticleName") = ReadyString(article("ArticleName"))
                p("ShortLink") = ReadyString(ShortLink)
                p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If article("ID").ToString() = "" Then
                    articleID = Guid.NewGuid.ToString()
                    p("Status") = ReadyString("activate")
                    p("ID") = ReadyString(articleID)
                    pSql = p.GetInsert()
                Else
                    articleID = article("ID").ToString()
                    If article("State") = "edit" Then
                        p("Status") = ReadyString("activate")
                        pSql = p.GetUpdate(" ID = " & ReadyString(articleID))
                    Else
                        p("Status") = ReadyString("deactivate")
                        pSql = p.GetUpdate(" ID = " & ReadyString(articleID))
                    End If
                End If

                Common.WebRequest.ProData.Execute(pSql)

                For Each topic As Object In article("Topic")
                    Dim topicID As String
                    p = New InsertUpdate("LibraryKnowledgeManagementArticleContents")

                    p("ArticleID") = ReadyString(articleID)
                    p("ContentID") = ReadyString(topic("ID").ToString()) 'It can be: TopicID, SubTopicID or ProblemID
                    p("Description") = ReadyString(topic("Topic"))
                    p("Tag") = ReadyString("Topic")

                    If topic.ContainsKey("DBID") = False Then
                        topicID = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(topicID)
                        pSql = p.GetInsert()
                    Else
                        topicID = topic("DBID").ToString()
                        If topic("State") = "edit" Then
                            pSql = p.GetUpdate(" ID = " & ReadyString(topicID))
                        Else
                            'Get dbid for subtopic
                            pSql = "SELECT ID FROM LibraryKnowledgeManagementArticleContents WHERE RelatedID = " & ReadyString(topicID)
                            Dim pDTSubTopic As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                            If pDTSubTopic.Count > 0 Then
                                'Delete all question in subtopic
                                pSql = "DELETE FROM LibraryKnowledgeManagementArticleContents WHERE RelatedID = " & ReadyString(pDTSubTopic(0)("ID"))
                                Common.WebRequest.ProData.Execute(pSql)
                            End If
                            'Delete subtopic
                            pSql = "DELETE FROM LibraryKnowledgeManagementArticleContents WHERE RelatedID = " & ReadyString(topicID)
                            Common.WebRequest.ProData.Execute(pSql)
                            'Delete topic
                            pSql = "DELETE FROM LibraryKnowledgeManagementArticleContents WHERE ID = " & ReadyString(topicID)
                        End If
                    End If

                    Common.WebRequest.ProData.Execute(pSql)

                    If topic("State") <> "remove" Then
                        For Each subtopic As Object In topic("SubTopic")
                            Dim subtopicID As String
                            p = New InsertUpdate("LibraryKnowledgeManagementArticleContents")
                            p("ArticleID") = ReadyString(articleID)
                            p("ContentID") = ReadyString(subtopic("ID").ToString()) 'It can be: TopicID, SubTopicID or ProblemID
                            p("Description") = ReadyString(subtopic("SubTopic"))
                            p("Tag") = ReadyString("SubTopic")
                            p("RelatedID") = ReadyString(topicID)

                            If subtopic.ContainsKey("DBID") = False Then
                                subtopicID = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(subtopicID)
                                pSql = p.GetInsert()
                            Else
                                subtopicID = subtopic("DBID").ToString()
                                If subtopic("State") = "edit" Then
                                    pSql = p.GetUpdate(" ID = " & ReadyString(subtopicID))
                                Else
                                    'Delete question
                                    pSql = "DELETE FROM LibraryKnowledgeManagementArticleContents WHERE RelatedID = " & ReadyString(subtopicID)
                                    Common.WebRequest.ProData.Execute(pSql)
                                    'Delete subtopic
                                    pSql = "DELETE FROM LibraryKnowledgeManagementArticleContents WHERE ID = " & ReadyString(subtopicID)
                                End If
                            End If

                            Common.WebRequest.ProData.Execute(pSql)

                            If subtopic("State") <> "remove" Then
                                For Each question As Object In subtopic("Questions")
                                    Dim questionID As String
                                    p = New InsertUpdate("LibraryKnowledgeManagementArticleContents")
                                    p("ArticleID") = ReadyString(articleID)
                                    p("ContentID") = ReadyString(question("ID").ToString()) 'It can be: TopicID, SubTopicID or ProblemID
                                    p("Description") = ReadyString(question("Problem"))
                                    p("Tag") = ReadyString("Problem")
                                    p("RelatedID") = ReadyString(subtopicID)

                                    If question.ContainsKey("DBID") = False Then
                                        questionID = Guid.NewGuid.ToString()
                                        p("ID") = ReadyString(questionID)
                                        pSql = p.GetInsert()
                                    Else
                                        questionID = question("DBID").ToString()
                                        If question("State") = "edit" Then
                                            pSql = p.GetUpdate(" ID = " & ReadyString(questionID))
                                        Else
                                            'Delete question
                                            pSql = "DELETE FROM LibraryKnowledgeManagementArticleContents WHERE ID = " & ReadyString(questionID)
                                        End If
                                    End If

                                    Common.WebRequest.ProData.Execute(pSql)
                                Next question
                            End If
                        Next subtopic
                    End If
                Next topic
            Next article
            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message()
        End Try
    End Function

    Function SaveArrangement() As String

        Try
            WebRequest.Instance.Business.SystemDefaults.KBServerUrl = ServerUrl

            For i As Integer = 0 To arrReadingList.Length - 1
                Dim p As New InsertUpdate("LibraryKnowledgeManagement")
                p("OrderNo") = arrReadingList(i)("OrderNo")
                Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(arrReadingList(i)("ID"))))

                If arrReadingList(i).ContainsKey("Topic") = True Then
                    For t As Integer = 0 To arrReadingList(i)("Topic").length - 1
                        Dim topic As Object = arrReadingList(i)("Topic")(t)
                        p = New InsertUpdate("LibraryKnowledgeManagement")
                        p("OrderNo") = topic("OrderNo")
                        'Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(topic("ID"))))

                        SaveOrderNo(arrReadingList(i)("ID"), topic("ID"), "", "Topic", topic("OrderNo"))

                        p = New InsertUpdate("TransKnowledgeManagement")

                        If topic.ContainsKey("List") = True Then
                            For l As Integer = 0 To topic("List").length - 1
                                Dim list As Object = topic("List")(l)

                                If list.ContainsKey("Problem") = True Then
                                    SaveOrderNo(arrReadingList(i)("ID"), topic("ID"), list("ID"), "SubTopic", list("OrderNo"))

                                    For s As Integer = 0 To list("Problem").length - 1
                                        Dim problem As Object = list("Problem")(s)
                                        p("OrderNo") = problem("OrderNo")
                                        Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(problem("ProblemID"))))
                                    Next
                                Else
                                    p("OrderNo") = list("OrderNo")
                                    Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(list("ID"))))
                                End If
                            Next
                        End If
                    Next
                End If
            Next

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveOrderNo(ByVal ProdID As String, ByVal TopicID As String, ByVal SubTopicID As String, ByVal Type As String, ByVal OrderNo As Integer) As Integer
        Dim pSql As String
        Dim pDT As DataTable
        Dim p As New InsertUpdate("LibraryKnowledgeManagementOrdering")

        'TYPEID can be TopicID or SubTopicID

        If Type = "Topic" Then
            pSql = "SELECT * FROM LibraryKnowledgeManagementOrdering WHERE ProductID = " & ReadyString(ProdID) &
                " AND TopicID = " & ReadyString(TopicID)
        Else
            pSql = "SELECT * FROM LibraryKnowledgeManagementOrdering WHERE ProductID = " & ReadyString(ProdID) &
                " AND TopicID = " & ReadyString(TopicID) & vbCrLf &
                " AND SubTopicID = " & ReadyString(SubTopicID)
        End If
        
        pDT = Common.WebRequest.ProData.Execute(pSql)

        p("ProductID") = ReadyString(ProdID)
        p("TopicID") = ReadyString(TopicID)
        p("SubTopicID") = IIf(SubTopicID = "", "NULL", ReadyString(SubTopicID))
        p("Type") = ReadyString(Type)
        p("OrderNo") = ReadyString(OrderNo)

        If pDT.Rows.Count = 0 Then
            p("ID ") = ReadyString(Guid.NewGuid.ToString())
            Common.WebRequest.ProData.Execute(p.GetInsert())
        Else
            Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(pDT.Rows(0)("ID"))))
        End If
    End Function

    Function GetTopicCases() As Object
        Dim pSql As String
        Dim pDTTopic As DataTable
        Dim pDTSubTopic As DataTable
        Dim pDTProblem As DataTable
        Dim pDTNoSubTopic As DataTable
        Dim cases(2) As Object
        Try
            pSql = "SELECT ID, Topic, OrderNo FROM LibraryKnowledgeManagement WHERE ID = " & ReadyString(TopicID) & vbCrLf &
                    " ORDER BY OrderNo ASC"
            pDTTopic = Common.WebRequest.ProData.Execute(pSql)

            pSql = "SELECT * FROM(SELECT lkm.ID, lkm.SubTopic, lkmo.OrderNo, 1 as IsSubTopic FROM LibraryKnowledgeManagement as lkm " & vbCrLf &
                    " LEFT JOIN LibraryKnowledgeManagementOrdering as lkmo ON lkmo.SubTopicID = lkm.ID " & vbCrLf &
                    " AND lkmo.ProductID = " & ReadyString(ProductID) & vbCrLf & " AND lkmo.type='SubTopic' " & vbCrLf &
                    " WHERE lkm.TopicID = " & ReadyString(TopicID) & vbCrLf &
                    " UNION ALL " & vbCrLf &
                    "SELECT ID as ID, Problem as SubTopic, OrderNo, 0 as IsSubTopic FROM TransKnowledgeManagement WHERE Topic = " & ReadyString(TopicID) & " AND SubTopic = ''  AND Product = " & ReadyString(ProductID) & vbCrLf &
                    ") as FINAL ORDER BY OrderNo ASC"

            'pSql = "SELECT lkm.ID, lkm.SubTopic, lkmo.OrderNo FROM LibraryKnowledgeManagement as lkm " & vbCrLf &
            '        " LEFT JOIN LibraryKnowledgeManagementOrdering as lkmo ON lkmo.TopicID = " & ReadyString(TopicID) & vbCrLf &
            '        " AND lkmo.ProductID = " & ReadyString(ProductID) & vbCrLf & " AND lkmo.type='SubTopic' " & vbCrLf &
            '        "WHERE lkm.TopicID = " & ReadyString(TopicID) & vbCrLf &
            '        " ORDER BY lkmo.OrderNo ASC"
            pDTSubTopic = Common.WebRequest.ProData.Execute(pSql)

            Dim subtopic(pDTSubTopic.Rows.Count - 1) As Object

            For i As Integer = 0 To pDTSubTopic.Rows.Count - 1
                If CBool(pDTSubTopic.Rows(i)("IsSubTopic")) = True Then
                    pSql = "SELECT DISTINCT tkm.ID, tkm.Problem, tkm.ShortLink, tkm.OrderNo FROM TransKnowledgeManagement as tkm " & vbCrLf &
                        " LEFT JOIN LibraryKnowledgeManagementArticleTopics as art " & vbCrLf &
                        " ON art.ProblemID = tkm.ID " & vbCrLf &
                        " WHERE (art.SubTopicID = " & ReadyString(pDTSubTopic.Rows(i)("ID")) & vbCrLf &
                        " AND art.TopicID = " & ReadyString(TopicID) & vbCrLf &
                        " ) OR (tkm.SubTopic = " & ReadyString(pDTSubTopic.Rows(i)("ID")) & vbCrLf &
                        " AND tkm.Product = " & ReadyString(ProductID) & vbCrLf &
                        " ) GROUP BY tkm.ID, tkm.Problem, tkm.ShortLink, tkm.OrderNo ORDER BY OrderNo ASC"
                    pDTProblem = Common.WebRequest.ProData.Execute(pSql)

                    Dim problem(pDTProblem.Rows.Count - 1) As Object

                    If pDTProblem.Rows.Count > 0 Then
                        For m As Integer = 0 To pDTProblem.Rows.Count - 1
                            problem(m) = New With {Key .ProblemID = pDTProblem.Rows(m)("ID"), .Problem = pDTProblem.Rows(m)("Problem"), .ProblemShortLink = pDTProblem.Rows(m)("ShortLink"), .OrderNo = pDTProblem.Rows(m)("OrderNo")}
                        Next
                    End If

                    subtopic(i) = New With {Key .ID = pDTSubTopic.Rows(i)("ID"), .SubTopic = pDTSubTopic.Rows(i)("SubTopic"), .OrderNo = pDTSubTopic.Rows(i)("OrderNo"), .Problem = problem}
                Else
                    subtopic(i) = New With {Key .ID = pDTSubTopic.Rows(i)("ID"), .SubTopic = pDTSubTopic.Rows(i)("SubTopic"), .OrderNo = pDTSubTopic.Rows(i)("OrderNo")}
                End If
            Next

            'Query for problem that doesnt have subtopic
            pSql = "SELECT * FROM TransKnowledgeManagement WHERE Topic = " & ReadyString(TopicID) & " AND SubTopic = ''  AND Product = " & ReadyString(ProductID) & vbCrLf &
                    " ORDER BY OrderNo ASC"
            pDTNoSubTopic = Common.WebRequest.ProData.Execute(pSql)

            Dim nosubtopic(pDTNoSubTopic.Rows.Count - 1) As Object

            If pDTNoSubTopic.Rows.Count > 0 Then
                For m As Integer = 0 To pDTNoSubTopic.Rows.Count - 1
                    nosubtopic(m) = New With {Key .ProblemID = pDTNoSubTopic.Rows(m)("ID"), .Problem = pDTNoSubTopic.Rows(m)("Problem"), .OrderNo = pDTNoSubTopic.Rows(m)("OrderNo")}
                Next
            End If

            Return Json(New With {Key .TopicID = pDTTopic.Rows(0)("ID"), .Topic = pDTTopic.Rows(0)("Topic"), .OrderNo = pDTTopic.Rows(0)("OrderNo"), .SubTopic = subtopic, .NoSubTopic = nosubtopic})
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function GetStartupData() As Object
        Dim pSql As String
        Dim pDTProduct As DataTable
        Dim pDTTopic As DataTable

        Try
            pSql = "SELECT ID, Product, OrderNo FROM LibraryKnowledgeManagement WHERE Product <> ''" & vbCrLf &
                    " ORDER BY OrderNo ASC"
            pDTProduct = Common.WebRequest.ProData.Execute(pSql)

            Dim startup(pDTProduct.Rows.Count - 1) As Object

            For i As Integer = 0 To pDTProduct.Rows.Count - 1
                pSql = "SELECT * FROM (SELECT DISTINCT ISNULL(CONVERT(varchar(max), lkm.ID), lkm2.ID) as ID, ISNULL(lkm.Topic, lkm2.Topic) as Topic FROM LibraryKnowledgeManagementArticleTopics as art " & vbCrLf &
                        "LEFT JOIN TransKnowledgeManagement as tkm ON tkm.Topic = art.TopicID " & vbCrLf &
                        "LEFT JOIN LibraryKnowledgeManagement as lkm ON lkm.ID = tkm.Topic " & vbCrLf &
                        "LEFT JOIN LibraryKnowledgeManagement as lkm2 ON lkm2.ID = art.TopicID " & vbCrLf &
                        "WHERE tkm.Product = " & ReadyString(pDTProduct.Rows(i)("ID")) & vbCrLf &
                        "OR art.ProductID = " & ReadyString(pDTProduct.Rows(i)("ID")) & vbCrLf &
                        ") as t ORDER BY Topic ASC"
                pDTTopic = Common.WebRequest.ProData.Execute(pSql)

                Dim topic(pDTTopic.Rows.Count - 1) As Object

                If pDTTopic.Rows.Count > 0 Then
                    For m As Integer = 0 To pDTTopic.Rows.Count - 1
                        If IsArrangeReadingList = True Then
                            TopicID = pDTTopic.Rows(m)("ID").ToString()
                            ProductID = pDTProduct.Rows(i)("ID").ToString()
                            Dim arrtopic As Object = GetTopicCases().Data
                            topic(m) = New With {Key .ID = pDTTopic.Rows(m)("ID"), .Topic = pDTTopic.Rows(m)("Topic"), .List = arrtopic}
                        Else
                            topic(m) = New With {Key .ID = pDTTopic.Rows(m)("ID"), .Topic = pDTTopic.Rows(m)("Topic")}
                        End If
                    Next
                End If

                startup(i) = New With {Key .Name = pDTProduct.Rows(i)("Product"), .ID = pDTProduct.Rows(i)("ID"), .OrderNo = pDTProduct.Rows(i)("OrderNo"), .Topic = topic, .Type = "Product"}
            Next

            Return Json(startup)
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function GetLinkedDetails() As Object
        Dim pSqlTopic As String
        Dim pDTTopic As DataTable
        Dim pSqlAnswer As String
        Dim pDTAnswer As DataTable
        Dim pSqlStep As String
        Dim pDTStep As DataTable
        Dim pSqlImages As String
        Dim pDTImages As DataTable
        Dim arrDetails(0) As Object
        Dim serializer As New JavaScriptSerializer()

        pSqlTopic = "SELECT ID, Problem FROM TransKnowledgeManagement WHERE ID = " & ReadyString(TopicID)
        pDTTopic = Common.WebRequest.ProData.Execute(pSqlTopic)

        If pDTTopic.Rows.Count = 0 Then
            arrDetails(0) = New With {Key .Error = "No matching records found"
                                }
        Else
            pSqlAnswer = "SELECT ID, Solution FROM TransKnowledgeManagement " & If(AnswerID <> Nothing, " WHERE ID = " & ReadyString(AnswerID), " WHERE ProblemID = " & ReadyString(pDTTopic.Rows(0)("ID")) & " ORDER BY OrderNo ASC ")
            pDTAnswer = Common.WebRequest.ProData.Execute(pSqlAnswer)

            Dim arrSolution(pDTAnswer.Rows.Count - 1) As Object

            If pDTAnswer.Rows.Count > 0 Then
                For i As Integer = 0 To pDTAnswer.Rows.Count - 1
                    pSqlStep = "SELECT ID as StepID, Steps, OrderNo FROM TransKnowledgeManagement WHERE SolutionID = " & ReadyString(pDTAnswer.Rows(i)("ID")) & vbCrLf &
                        " ORDER BY OrderNo ASC"

                    pDTStep = Common.WebRequest.ProData.Execute(pSqlStep)

                    Dim stepObj(pDTStep.Rows.Count - 1) As Object

                    If pDTStep.Rows.Count > 0 Then
                        For m As Integer = 0 To pDTStep.Rows.Count - 1

                            pSqlImages = "SELECT * FROM LibraryKnowledgeManagementPictures " & vbCrLf &
                                "WHERE ParentID = " & ReadyString(pDTStep.Rows(m)("StepID").ToString())
                            pDTImages = Common.WebRequest.ProData.Execute(pSqlImages)

                            Dim imgObj(pDtImages.Rows.Count - 1) As Object

                            If pDtImages.Rows.Count > 0 Then
                                For n As Integer = 0 To pDtImages.Rows.Count - 1
                                    Dim pFilename As String = pDTImages.Rows(n)("ID").ToString() & "." & pDTImages.Rows(n)("Type").ToString()
                                    imgObj(n) = New With {Key .ImgID = pDtImages.Rows(n)("ID").ToString(), .CaretPos = pDtImages.Rows(n)("CaretPos").ToString(), .StepID = pDtImages.Rows(n)("ParentID").ToString(), .Status = "old", .Location = pFilename}
                                Next
                            End If
                            stepObj(m) = New With {Key .Step = pDTStep.Rows(m)("Steps").ToString(), .Images = imgObj}
                        Next
                    End If

                    arrSolution(i) = New With {Key .Solution = pDTAnswer.Rows(i)("Solution"),
                                    .Step = stepObj
                                }
                Next
                arrDetails(0) = New With {Key .Problem = pDTTopic.Rows(0)("Problem"),
                                   .Solution = arrSolution, .OrgID = WebRequest.Instance.Data("OrgID")
                                }

            Else
                arrDetails(0) = New With {Key .Error = "No matching records found"
                                }
            End If
        End If
        Return serializer.Serialize(arrDetails)

    End Function

    Function GetAllKnowledgeManagementAndQuestionaire() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = ""
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
        Dim whereSearchContent As String = ""
        Dim additionalwhereSearchContent = ""

        If Filter <> "" Then
            Dim arrFilter As Object = Split(Filter, "||")
            Dim txtFilter As String = ""
            Dim isFinishAppendFirstOperator As Boolean = False

            If PageSearchText <> "" Then
                additionalwhereSearchContent = " AND (" & additionalwhereSearchContent
                isFinishAppendFirstOperator = True
            End If

            For Each txtFilter In arrFilter
                Dim arrtxtFilter As Object = Split(txtFilter, "/")

                If isFinishAppendFirstOperator = False Then
                    additionalwhereSearchContent = If(additionalwhereSearchContent <> "", additionalwhereSearchContent & " AND ", "")
                End If

                If arrtxtFilter(0) = "topic" Then
                    additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(convert(varchar(max), TopicID), '') = " & ReadyString(arrtxtFilter(2))
                ElseIf arrtxtFilter(0) = "subtopic" Then
                    additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(convert(varchar(max), SubTopicID), '') = " & ReadyString(arrtxtFilter(2))
                ElseIf arrtxtFilter(0) = "product" Then
                    additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(convert(varchar(max), ProductID), '') = " & ReadyString(arrtxtFilter(2))
                ElseIf arrtxtFilter(0) = "language" Then
                    additionalwhereSearchContent = additionalwhereSearchContent & " ISNULL(convert(varchar(max), LanguageID), '') = " & ReadyString(arrtxtFilter(2))
                End If

                isFinishAppendFirstOperator = False
            Next

            If PageSearchText <> "" Then
                additionalwhereSearchContent = additionalwhereSearchContent & ") "
            End If
        Else
            additionalwhereSearchContent = ""
        End If

        If PageSearchText <> "" Then
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " AND "
                End If
                '"ISNULL(Topic, '') + ' ' + " & vbCrLf &
                '"ISNULL(Language, '') + ' ' + " & vbCrLf &
                '"ISNULL(EditedBy, '') + ' ' + " & vbCrLf &
                '"ISNULL(Solution, '') + ' ' + " & vbCrLf &
                '"ISNULL(Product, '') " & vbCrLf &
                whereSearchContent = whereSearchContent & operators & " ISNULL(Problem, '') " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            'whereSearchContent = " ISNULL(Problem, '') " & vbCrLf &
            '                        " LIKE " & ReadyStringWild(PageSearchText) & vbCrLf
            whereSearch = " WHERE (" & whereSearchContent & ") " & additionalwhereSearchContent & vbCrLf
        Else
            whereSearch = If(additionalwhereSearchContent <> "", "WHERE (" & additionalwhereSearchContent & ")", "")
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Then
            whereOrder = " ORDER BY RowNum " & PageColumnDirection
        ElseIf PageColumnNo = 1 Then
            whereOrder = " ORDER BY Problem " & PageColumnDirection
            'ElseIf PageColumnNo = 2 Then
            '    whereOrder = " ORDER BY Solution " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY Topic " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY Product " & PageColumnDirection
            'ElseIf PageColumnNo = 7 Then
            '    whereOrder = " ORDER BY Language " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY EditedBy " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY CONVERT(datetime, LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select V1.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From (Select DISTINCT " & vbCrLf &
                "   mkm.ID as ProblemID, mkm.CaseNo, ISNULL(mkm.SubTopic, '') as SubTopicID, mkm.Problem, " & vbCrLf &
                "   lkm1.Topic, -- for single topic (OLD)" & vbCrLf &
                "   lkm2.Product, -- for single product (OLD) " & vbCrLf &
                "   lkm1.ID as TopicID, lkm2.ID as ProductID, lkm3.Language, lkm3.ID as LanguageID, mkm.PeopleID, " & vbCrLf &
                "   mkm.Tags, CONVERT(VARCHAR(10),mkm.Lastupdated,110) LastUpdated, mkm.ShortLink as ProblemShortLink, " & vbCrLf &
                "   MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName EditedBy, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT Top 1 ', '+ km2.Solution " & vbCrLf &
                "          FROM TransKnowledgeManagement as km2 " & vbCrLf &
                "          WHERE km2.ProblemID = mkm.ID " & vbCrLf &
                "      FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM TransKnowledgeManagement) as Solution, " & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT Top 1 ', '+ lkm1.Topic " & vbCrLf &
                "          FROM LibraryKnowledgeManagementArticleTopics as art " & vbCrLf &
                "           INNER JOIN LibraryKnowledgeManagement lkm1 on art.TopicID = lkm1.ID AND lkm1.Topic IS NOT NULL " & vbCrLf &
                "          WHERE art.ProblemID = mkm.ID " & vbCrLf &
                "      FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM LibraryKnowledgeManagementArticleTopics) as Topics, --for multiple topics (NEW)" & vbCrLf &
                "   (SELECT DISTINCT STUFF((SELECT DISTINCT Top 1 ', '+ lkm1.Product " & vbCrLf &
                "          FROM LibraryKnowledgeManagementArticleTopics as art " & vbCrLf &
                "           INNER JOIN LibraryKnowledgeManagement lkm1 on art.ProductID = lkm1.ID " & vbCrLf &
                "          WHERE art.ProblemID = mkm.ID " & vbCrLf &
                "      FOR XML PATH('') " & vbCrLf &
                "   ), 1, 1,'') FROM LibraryKnowledgeManagementArticleTopics) as Products --for multiple products (NEW)" & vbCrLf &
                "   from TransKnowledgeManagement as mkm " & vbCrLf &
                "   left join MobizLibraryPeople on MobizLibraryPeople.ID = mkm.PeopleID " & vbCrLf &
                "   left join LibraryKnowledgeManagement lkm1 on mkm.Topic = lkm1.ID AND lkm1.Topic IS NOT NULL " & vbCrLf &
                "   left join LibraryKnowledgeManagement lkm2 on mkm.Product = lkm2.ID AND lkm2.Product IS NOT NULL " & vbCrLf &
                "   left join LibraryKnowledgeManagement lkm3 on mkm.Language = lkm3.ID AND lkm3.Language IS NOT NULL " & vbCrLf &
                "   WHERE ISNULL(mkm.Problem, '') <> '' " & vbCrLf &
                ") as V1 " & whereSearch & " ) as v2 " & whereLimit & vbCrLf
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetJson(ByVal dt As DataTable) As String
        Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
    End Function

    Private Function HexStringToBytes(ByVal hexString As String) As Byte()
        Dim bytes As New List(Of Byte)()

        ' Insert a leading 0 if there are an odd number of characters; this
        ' is simple but inelegant.
        Dim stringData As String = hexString
        If stringData.Length Mod 2 = 1 Then
            stringData = "0" & stringData
        End If

        ' Convert each pair of characters to a byte
        For i As Integer = 0 To stringData.Length - 1 Step 2
            Dim currentByte As String = stringData.Substring(i, 2)
            Dim value As Byte = Byte.Parse(currentByte, Globalization.NumberStyles.HexNumber)
            bytes.Add(value)
        Next

        Return bytes.ToArray()
    End Function

End Class