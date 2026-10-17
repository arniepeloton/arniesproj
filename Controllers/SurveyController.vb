Imports System.Net
Imports System.IO
Imports Ionic.Zip
Imports System.Collections.Generic
Namespace WebProj
    Public Class SurveyController
        Inherits System.Web.Mvc.Controller

        Public Property PageStart As Integer
        Public Property PageNoOfRows As Integer
        Public Property PageSearchText As String
        Public Property PageColumnNo As String
        Public Property PageColumnDirection As String
        Public Property IsCountPage As String
        Dim totalScore As Decimal = 0

        '
        ' GET: /Inventory Form

        Function Ecommerce() As ActionResult
            Return View()
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

        Function ZipFiles() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim zip As New ProZip
                Dim qid As String = Request.Params.Get("QID")
                Dim resid As String = Request.Params.Get("RespondentID")
                Dim zipfile As String = Server.MapPath("~/SurveyRequirements/Requirements.zip")
                Dim pSql As String
                Dim d As New WebClient()
                Dim pReport As ProReport
                Dim pBaseFilename As String
                Dim pFilename As String
                Dim clientID As String
                Dim noReq As Boolean = True
                Dim errmsg As String = ""

                pSql = "SELECT ClientID FROM LibrarySurveyRespondent WHERE ID = " & ReadyString(resid)
                clientID = Common.WebRequest.ProData.Execute(pSql).Rows(0)("ClientID").ToString()

                Dim folderName As String = Common.ServerPath & "SurveyFiles\BuildingPermit\" & clientID

                If Not System.IO.Directory.Exists(folderName) Then
                    System.IO.Directory.CreateDirectory(folderName)
                End If

                pSql = "SELECT f.FileName, f.FilePath FROM LibrarySurveyRequirements as r " & vbCrLf &
                        " INNER JOIN LibrarySurveyFiles as f ON f.ID = r.MrtFileID " & vbCrLf &
                        " WHERE r.QuestionaireID = " & ReadyString(qid) & vbCrLf &
                        " AND r.IsSysGen = 1 "
                Dim rowSysGen As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If rowSysGen.Count > 0 Then
                    noReq = False
                    For Each sg As Object In rowSysGen
                        Dim sourcefilename As String = Split(Split(sg("FilePath"), "\").Last(), ".").First()
                        Dim filename As String = String.Join(" ", Split(sg("FileName"), ".").Take(Split(sg("FileName"), ".").Length() - 1))
                        GenerateSysForms(qid, resid, filename, sourcefilename, clientID, errmsg)

                        If errmsg <> "" Then
                            Return JsonDictionary.Serialize("Results", "error downloading file")
                        End If

                        zip.AddFile(Server.MapPath("~/SurveyFiles/BuildingPermit/" & clientID & "/" & filename & ".pdf"), "System Generated Files")
                    Next sg
                Else
                    noReq = True
                End If
                

                Dim nonsgname As String = GenerateNonSysForms(clientID)

                If nonsgname <> "empty" Then
                    noReq = False
                    zip.AddFile(Server.MapPath("~/SurveyFiles/BuildingPermit/" & clientID & "/" & nonsgname & ".pdf"), "Non System Generated Files")
                End If

                If noReq Then
                    Return JsonDictionary.Serialize("Results", "empty")
                End If

                Dim file As System.IO.FileInfo = New System.IO.FileInfo(zipfile)

                Response.Clear()
                Response.ContentType = "application/octet-stream"
                Response.AddHeader("content-disposition", "attachment; filename=Requirements.zip")
                zip.Save(zipfile)
                Response.WriteFile(file.FullName)
                Response.End()
                Return JsonDictionary.Serialize("Results", "success")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DownloadZipFiles(file) As String
            Dim pFilename As String

            pFilename = Common.ServerPath & "temp\Billing\" & Common.WebRequest.QueryString("Filename") & ".pdf"

            Return file(pFilename, "application/pdf")
        End Function

        Function Download() As String
            'Dim webClient As New WebClient
            Dim source As String = "SurveyFiles/" & Request.Params.Get("ClientID") & "/" & Request.Params.Get("FileName")
            Dim path As String = Server.MapPath("~/" & source)
            Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)

            If file.Exists Then 'set appropriate headers
                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & Request.Params.Get("DownloadFileName"))
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
                Return JsonDictionary.Serialize("Results", "Exist")
            Else
                Return JsonDictionary.Serialize("Results", "Not Exist")
            End If

            'webClient.DownloadFile(source & Request.Params.Get("FileName"), Common.ServerPath & "SurveyFiles/" & Request.Params.Get("ClientID") & "/" & Request.Params.Get("DownloadFileName"))
            'webClient.Dispose()
        End Function

        Function UploadRequirements() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim files As HttpFileCollectionBase = Request.Files
                Dim QID As String = Request.Params.Get("QID")
                Dim ID As String = Request.Params.Get("ID")
                Dim IsSysGen As String = Request.Params.Get("IsSysGen")
                Dim Name As String = Request.Params.Get("Name")
                Dim NoOfCopies As String = Request.Params.Get("NoOfCopies")
                Dim State As String = Request.Params.Get("State")
                Dim DateNow As String = Request.Params.Get("Date")
                Dim FileID As String = Guid.NewGuid.ToString()
                Dim MrtFileID As String = Request.Params.Get("MrtFileID")
                Dim IsUniversal As String = Request.Params.Get("IsUniversal")
                Dim ReqGroupID As String = Request.Params.Get("ReqGroupID")
                Dim UpdateMRTFile = False
                Dim p As New InsertUpdate("LibrarySurveyFiles")

                Common.WebRequest.ProData.BeginTransaction()

                If CBool2(IsSysGen) And State <> "remove" And files.Count > 0 Then
                    Dim f As HttpPostedFileBase = files(0)
                    Dim folderName As String = Common.ServerPath & "reports"
                    Dim filename As String = IIf(CStr2(MrtFileID) = "", FileID, MrtFileID) & "_" & f.FileName
                    Dim pSourceURL As String = folderName & "\" & filename
                    UpdateMRTFile = True

                    p("FileName") = ReadyString(f.FileName)
                    p("FilePath") = ReadyString(pSourceURL)

                    If State = "new" Then
                        p("ID") = ReadyString(FileID)
                        p("UploadedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("DateUploaded") = ReadyString(DateNow)
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    ElseIf State = "edit" Then
                        FileID = MrtFileID
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(FileID)))
                    End If

                    If Not System.IO.Directory.Exists(folderName) Then
                        System.IO.Directory.CreateDirectory(folderName)
                    End If

                    f.SaveAs(pSourceURL)
                End If

                If MrtFileID <> Nothing Then
                    FileID = MrtFileID
                End If

                p = New InsertUpdate("LibrarySurveyRequirements")

                p("QuestionaireID") = ReadyString(QID)
                p("Name") = ReadyString(Name)
                p("NoOfCopies") = ReadyString(NoOfCopies)
                p("IsSysGen") = ReadyString(IsSysGen)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("IsUniversal") = ReadyString(IsUniversal)
                p("GroupRequirementID") = IIf(ReqGroupID = Nothing, "NULL", ReadyString(ReqGroupID))

                If CBool2(IsSysGen) Then
                    p("MrtFileID") = ReadyString(FileID)
                End If

                If State = "new" Then
                    p("LastUpdated") = ReadyString(DateNow)
                    p("ID") = ReadyString(ID)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                ElseIf State = "edit" Then
                    If UpdateMRTFile Then
                        p("LastUpdated") = ReadyString(DateNow)
                    End If
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(ID)))
                Else
                    Common.WebRequest.ProData.Execute("DELETE FROM LibrarySurveyRequirements WHERE ID = " & ReadyString(ID))
                End If

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function Upload() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim files As HttpFileCollectionBase = Request.Files
            Dim qID As String = Request.Params.Get("questionID")
            Dim cID As String = Request.Params.Get("clientID")
            Dim rID As String = Request.Params.Get("respondentID")
            Dim pID As String = Request.Params.Get("pageID")
            Dim folderName As String = Common.ServerPath & "SurveyFiles\" & cID

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim p As New InsertUpdate("LibrarySurveyResponse")
                    Dim filename As String = qID & "_" & file.FileName

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("QuestionID") = ReadyString(qID)
                    p("RespondentID") = ReadyString(rID)
                    p("Answer") = ReadyString(file.FileName)
                    p("Description") = ReadyString(filename)
                    p("pageID") = ReadyString(pID)
                    p("IsOther") = "0"

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    Dim pSourceURL As String = folderName & "\" & filename
                    file.SaveAs(pSourceURL)
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function Index() As ActionResult
            Return View()
        End Function

        Function Creator() As ActionResult
            Return View()
        End Function

        Function Questionaire() As ActionResult
            Return View()
        End Function

        Function GenerateNonSysForms(ByVal clientid As String) As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim qID As String = Request.Params.Get("QID") 'Common.WebRequest.Data("QID")
                Dim pSql As String = "SELECT * FROM LibrarySurveyGroupRequirements WHERE QuestionaireID = " & ReadyString(qID)
                Dim group As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                Dim dt As New DataTable
                Dim pReport As ProReport
                Dim pBaseFilename As String
                Dim pFilename As String

                dt.Columns.Add("GroupName")
                dt.Columns.Add("Requirements")

                For g As Integer = 0 To group.Count - 1
                    pSql = "SELECT * FROM LibrarySurveyRequirements WHERE GroupRequirementID = " & ReadyString(group(g)("ID").ToString()) & vbCrLf &
                            " AND IsSysGen = 0"
                    Dim grrep As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                    Dim reqdesc As New List(Of String)
                    Dim newRow As DataRow
                    newRow = dt.NewRow

                    For r As Integer = 0 To grrep.Count - 1
                        Dim copies As String = "(" & grrep(r)("NoOfCopies") & IIf(CDec2(grrep(r)("NoOfCopies")) > 1, " copies)", " copy)")
                        reqdesc.Add(copies & " of " & grrep(r)("Name"))
                    Next

                    If reqdesc.Count > 0 Then ' do not add group without requirement(s)
                        newRow(0) = group(g)("Name").ToString()
                        newRow(1) = String.Join(", ", reqdesc)
                        dt.Rows.Add(newRow)
                    End If
                Next

                If dt.Rows.Count = 0 Then
                    Return "empty"
                End If

                pReport = New ProReport()
                pReport.AutoFillHeader = False
                pReport.SourceFilename = "NonSystemForm"

                'pReport.Variables("FullName") = Common.WebRequest.Data("FullName")
                'pReport.Variables("Address") = Common.WebRequest.Data("Address")

                pBaseFilename = GenerateString(20)
                pFilename = Common.ServerPath & "SurveyFiles\BuildingPermit\" & clientid & "\Non_System_Generated_Forms.pdf"
                pReport.AddTable("Info", dt)
#If DEBUG Then
                ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"
#End If
                pReport.ExportXmlFile()
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

                Return "Non_System_Generated_Forms"
                'Return JsonDictionary.Serialize("Filename", pBaseFilename)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        <HttpPost>
        Function GenerateSysForms(ByVal QID As String, ByVal ResID As String, ByVal FileName As String, ByVal SourceFileName As String, ByVal clientid As String, ByRef errmsg As String) As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pReport As ProReport
                Dim pBaseFilename As String
                Dim pFilename As String

                Dim pSql As String = "SELECT ISNULL((SELECT " & vbCrLf &
                                    "   CASE WHEN q.Type = 'radioinput' THEN description WHEN q.Type = 'checkboxinput' THEN description ELSE answer END" & vbCrLf &
                                    "   FROM LibrarySurveyResponse where respondentid = " & ReadyString(ResID) & " and fieldid = fields.ID), '') as Answer, " & vbCrLf &
                                    " fields.VariableName as Label, q.Type " & vbCrLf &
                                    " FROM LibrarySurveyQuestionFields as fields " & vbCrLf &
                                    " LEFT JOIN LibrarySurveyQuestions as q ON fields.QuestionID = q.ID " & vbCrLf &
                                    " LEFT JOIN LibrarySurveyQuestionaire as qr ON qr.ID = q.QuestionaireID " & vbCrLf &
                                    " WHERE qr.ID = " & ReadyString(QID) & vbCrLf &
                                    " and q.type <> 'Matrix' " & vbCrLf &
                                    " UNION ALL " & vbCrLf &
                                    " SELECT ISNULL((select answer " & vbCrLf &
                                    "   FROM LibrarySurveyResponse where respondentid = " & ReadyString(ResID) & " and questionid = q.ID), '') as Answer, " & vbCrLf &
                                    " q.VariableName as Label, q.Type " & vbCrLf &
                                    " FROM LibrarySurveyQuestions as q " & vbCrLf &
                                    " LEFT JOIN LibrarySurveyQuestionaire as qr ON qr.ID = q.QuestionaireID " & vbCrLf &
                                    " WHERE qr.ID = " & ReadyString(QID) & vbCrLf &
                                    " and q.type = 'Single Input' " & vbCrLf
                '" UNION ALL " & vbCrLf &
                '" SELECT (SELECT CASE WHEN ControlType = 'radio' THEN 'x' ELSE r.Answer END " & vbCrLf &
                '" From LibrarySurveyQuestionFields as f2 WHERE f2.ID = r.FieldID) as Answer, " & vbCrLf &
                '" (SELECT VariableName + '_' + " & vbCrLf &
                '"       (SELECT VariableName From LibrarySurveyQuestionFields as f2 WHERE f2.ID = r.FieldID) --column name " & vbCrLf &
                '" From LibrarySurveyQuestionFields WHERE ID = r.MatrixRowID) as Label, q.Type " & vbCrLf &
                '" FROM LibrarySurveyResponse as r " & vbCrLf &
                '" LEFT JOIN LibrarySurveyQuestions as q ON q.ID = r.QuestionID " & vbCrLf &
                '" WHERE q.QuestionaireID = " & ReadyString(Common.WebRequest.Data("QID")) & vbCrLf &
                '" and q.type = 'Matrix' " & vbCrLf
                Dim pDt As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim newpDt As New DataTable
                Dim newRow As DataRow
                Dim ctrCol As Integer = 0
                Dim ProposeDateConstruction = "", ExpectedDateCompletion = ""
                Dim TotalProjectCost = 0
                newRow = newpDt.NewRow

                For i As Integer = 0 To pDt.Rows.Count - 1
                    newpDt.Columns.Add(pDt.Rows(i)("Label"))
                    newRow(i) = pDt.Rows(i)("Answer")
                    ctrCol += 1

                    If pDt.Rows(i)("Label") = "Please_indicate_area_and_timelines:__Proposed_Date_of_Construction" Then
                        ProposeDateConstruction = pDt.Rows(i)("Answer")
                    End If

                    If pDt.Rows(i)("Label") = "Please_indicate_area_and_timelines:__Expected_Date_of_Completion" Then
                        ExpectedDateCompletion = pDt.Rows(i)("Answer")
                    End If
                Next

                newpDt.Columns.Add("ProjectDurationInDays")

                If ProposeDateConstruction <> "" And ExpectedDateCompletion <> "" Then
                    Try
                        newRow(ctrCol) = Math.Abs(DateDiff(DateInterval.Day, Convert.ToDateTime(ProposeDateConstruction), Convert.ToDateTime(ExpectedDateCompletion)))
                    Catch ex As Exception
                        AppendToLog(ex.Message)
                        newRow(ctrCol) = ""
                    End Try
                Else
                    newRow(ctrCol) = ""
                End If

                ctrCol += 1

                newpDt.Columns.Add("DateToday")
                newRow(ctrCol) = Format(Date.Today(), "MMM dd, yyyy")
                ctrCol += 1

                'Matrix ROW
                pSql = "SELECT f.* " & vbCrLf &
                        " FROM LibrarySurveyQuestionFields as f " & vbCrLf &
                        " INNER JOIN LibrarySurveyQuestions as q ON q.ID = f.QuestionID and f.AsType = 'Row' " & vbCrLf &
                        " WHERE q.QuestionaireID = " & ReadyString(QID) & vbCrLf &
                        " AND q.type = 'Matrix' "
                Dim pDTRow As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                'Get response of matrix
                pSql = "SELECT r.* " & vbCrLf &
                        " FROM LibrarySurveyResponse as r " & vbCrLf &
                        " INNER JOIN LibrarySurveyQuestions as q ON q.ID = r.QuestionID " & vbCrLf &
                        " INNER JOIN LibrarySurveyQuestionaire as qr ON qr.ID = q.QuestionaireID " & vbCrLf &
                        " WHERE qr.ID = " & ReadyString(QID) & vbCrLf &
                        " AND ISNULL(convert(varchar(max), r.MatrixRowID), '') <> '' " & vbCrLf &
                        " AND r.RespondentID = " & ReadyString(ResID)
                Dim pDTRes As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                'create variable name of each column per row
                For r As Integer = 0 To pDTRow.Count - 1
                    'Matrix COLUMN
                    pSql = "SELECT f.* " & vbCrLf &
                            " FROM LibrarySurveyQuestionFields as f " & vbCrLf &
                            " INNER JOIN LibrarySurveyQuestions as q ON q.ID = f.QuestionID and f.AsType = 'Column' " & vbCrLf &
                            " WHERE q.QuestionaireID = " & ReadyString(QID) & vbCrLf &
                            " AND f.QuestionID = " & ReadyString(pDTRow(r)("QuestionID").ToString()) & vbCrLf &
                            " AND q.type = 'Matrix' "
                    Dim pDTCol As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    For c As Integer = 0 To pDTCol.Count - 1
                        Dim colname As String = pDTRow(r)("VariableName") & "__" & pDTCol(c)("VariableName")
                        newpDt.Columns.Add(colname)

                        Dim hasResponse As Boolean = False

                        For Each res As Object In pDTRes
                            If res("MatrixRowID").ToString() = pDTRow(r)("ID").ToString() And res("FieldID").ToString() = pDTCol(c)("ID").ToString() Then
                                If pDTCol(c)("ControlType") = "radio" Then
                                    newRow(ctrCol) = "x"
                                Else
                                    newRow(ctrCol) = res("Answer")

                                    If colname = "Please_indicate_Estimated_Cost(s)__Building__Please_indicate_Estimated_Cost(s)__Material_Cost" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Building__Please_indicate_Estimated_Cost(s)__Cost_of_Equipment_Installed" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Electrical__Please_indicate_Estimated_Cost(s)__Material_Cost" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Electrical__Please_indicate_Estimated_Cost(s)__Cost_of_Equipment_Installed" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Mechanical__Please_indicate_Estimated_Cost(s)__Material_Cost" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Mechanical__Please_indicate_Estimated_Cost(s)__Cost_of_Equipment_Installed" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Plumbing__Please_indicate_Estimated_Cost(s)__Material_Cost" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Plumbing__Please_indicate_Estimated_Cost(s)__Cost_of_Equipment_Installed" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Others__Please_indicate_Estimated_Cost(s)__Material_Cost" Or
                                        colname = "Please_indicate_Estimated_Cost(s)__Others__Please_indicate_Estimated_Cost(s)__Cost_of_Equipment_Installed" Then
                                        TotalProjectCost += CDec2(res("Answer"))
                                    End If
                                End If
                                hasResponse = True
                                Exit For
                            End If
                        Next res

                        If Not hasResponse Then
                            newRow(ctrCol) = ""
                        End If

                        ctrCol += 1
                    Next
                Next

                If TotalProjectCost <> 0 Then
                    newpDt.Columns.Add("TotalProjectCost")
                    newRow(ctrCol) = TotalProjectCost
                    ctrCol += 1
                End If

                newpDt.Rows.Add(newRow)

                Try
                    pReport = New ProReport()
                    pReport.AutoFillHeader = False
                    pReport.SourceFilename = SourceFileName
                    pFilename = Common.ServerPath & "SurveyFiles\BuildingPermit\" & clientid & "\" & FileName & ".pdf"
                    pReport.AddTable("Info", newpDt)

                    ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\SurveyFormXML.xml"

                    pReport.ExportXmlFile()
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
                Catch ex As Exception
                    AppendToLog(ex.Message)
                    'diri moadto pag dili equal ang nacreate na variables sa variables sa mrt file. dapat magcreate gihapon ug pdf file
                    pReport = New ProReport()
                    pReport.AutoFillHeader = False
                    pReport.SourceFilename = SourceFileName
                    pFilename = Common.ServerPath & "SurveyFiles\BuildingPermit\" & clientid & "\" & FileName & ".pdf"

                    ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\SurveyFormXML.xml"

                    pReport.ExportXmlFile()
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
                End Try

                Return FileName
            Catch ex As Exception
                AppendToLog(ex.Message)
                errmsg = ex.Message
                Return ex.Message
            End Try
        End Function

        <HttpGet>
        Public Function GetSampleMrt() As ActionResult

            Dim pFilename As String

            pFilename = Common.ServerPath & "SurveyFiles\BuildingPermit\" & Common.WebRequest.QueryString("Filename") & ".pdf"

            Return File(pFilename, "application/pdf")

        End Function

        Function GetListOfSurvey() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT qre.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as CreatedByName, " & vbCrLf &
                        " (SELECT COUNT(ID) FROM LibrarySurveyRespondent WHERE QuestionaireID = qre.ID) as Respondents, " & vbCrLf &
                        " (SELECT Link FROM MobizLibrarySurveyQuestionaireLink WHERE QuestionaireID = qre.ID) as Link " & vbCrLf &
                        " FROM LibrarySurveyQuestionaire as qre " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = qre.CreatedBy  " & vbCrLf &
                        " WHERE qre.IsActive = 1 " & vbCrLf &
                        " AND ISNULL(qre.UseInModule, 'crm') = " & ReadyString(Request.Params.Get("mod")) & vbCrLf
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetClientSurvey() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            PageStart = CInt(Request.Form.GetValues("start")(0))
            PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            IsCountPage = False
            PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            Dim pDTQuestionaire As DataTable = GetSurveyDataTable(True)
            Dim arrSurvey As New List(Of Dictionary(Of String, Object))
            Dim pItemSurvey As New Dictionary(Of String, Object)

            If pDTQuestionaire.Rows.Count > 0 Then
                If pDTQuestionaire.Rows(0).Table.Columns("Error") IsNot Nothing Then
                    Return pDTQuestionaire.Rows(0)("ERROR")
                End If

                For i As Integer = 0 To pDTQuestionaire.Rows.Count - 1
                    Dim survey As Object = pDTQuestionaire.Rows(i)

                    arrSurvey.Add(GetSurveyDetails(survey, i + 1, True))
                Next
            End If

            IsCountPage = True
            Dim pDTCountPage As DataTable = GetSurveyDataTable(True)

            Return Json(New With {Key .data = arrSurvey, .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
        End Function

        Function GetResponse() As Object
            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT res.*, field.Label, q.Type, q.IsUseCheckbox, field.ControlType as Control, resd.ClientID, field.DataType " & vbCrLf &
                                " FROM LibrarySurveyResponse as res " & vbCrLf &
                                " LEFT JOIN LibrarySurveyQuestionFields as field ON field.ID = res.FieldID " & vbCrLf &
                                " LEFT JOIN LibrarySurveyQuestions as q ON q.ID = res.QuestionID " & vbCrLf &
                                " LEFT JOIN LibrarySurveyRespondent as resd ON resd.ID = res.RespondentID " & vbCrLf &
                                " WHERE res.RespondentID = " & ReadyString(Common.WebRequest.Data("RID")) & vbCrLf
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Dim rows As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            For Each f As Object In rows
                If f("Type") <> "Single Input" And f("Type") <> "file" Then
                    pSql = "SELECT TunneledTo FROM LibrarySurveyQuestionFieldsTunnel WHERE FieldID = " & ReadyString(f("FieldID").ToString())
                    Dim tunnel As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    If tunnel.Count > 0 Then
                        f.Add("Tunnel", tunnel(0)("TunneledTo").ToString())
                    Else
                        f.Add("Tunnel", "")
                    End If
                End If
            Next f

            Return Json(New With {Key .data = rows})
        End Function

        Function GetListOfRespondents() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT res.*, " & vbCrLf &
                                "CASE WHEN ISNULL(CONVERT(varchar(MAX), org.ID), '') = '' THEN " & vbCrLf &
                                "   ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname " & vbCrLf &
                                "ELSE " & vbCrLf &
                                "   org.CompanyName" & vbCrLf &
                                "END as ClientName" & vbCrLf &
                                " FROM LibrarySurveyRespondent as res" & vbCrLf &
                                " LEFT JOIN LibraryOrganization as org ON org.ID = res.ClientID " & vbCrLf &
                                " LEFT JOIN LibraryPeople as ppol ON ppol.ID = res.ClientID " & vbCrLf &
                                " WHERE res.QuestionaireID = " & ReadyString(Common.WebRequest.Data("QID")) & vbCrLf &
                                " ORDER BY CONVERT(datetime, res.DateResponded) ASC "
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function SaveResponse() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            totalScore = 0
            Dim arrResponse As Object = Common.WebRequest.AllData("Response")
            Dim qID As String = Common.WebRequest.Data("QID")
            Dim clientID As String = Common.WebRequest.Data("ClientID")
            Dim pageID As String = Common.WebRequest.Data("PageID")
            Dim isSaveFinish As String = Common.WebRequest.Data("IsSaveFinish")
            Dim datenow As String = CDate(Common.WebRequest.Data("Date"))
            Dim pSql As String
            Dim respondentTempCompanyProfileID As String
            Dim p As New InsertUpdate("LibrarySurveyRespondent")
            Dim respondentID As String = IIf(Common.WebRequest.Data("RespondentID") = "", Guid.NewGuid.ToString(), Common.WebRequest.Data("RespondentID"))

            Common.WebRequest.ProData.BeginTransaction()

            pSql = "SELECT ID FROM LibrarySurveyRespondent WHERE ID = " & ReadyString(respondentID)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                If clientID = "" Then 'add temp company profile for unknown respondent
                    p = New InsertUpdate("LibraryOrganization")
                    Dim ctrRes As Integer = Common.WebRequest.ProData.Execute("SELECT COUNT(ID) as Count FROM LibrarySurveyRespondent").Rows(0)("Count")
                    respondentTempCompanyProfileID = Guid.NewGuid.ToString()

                    p("ID") = ReadyString(respondentTempCompanyProfileID)
                    p("CompanyName") = ReadyString("Respondent " & (CDec2(ctrRes) + 1))
                    p("LastUpdated") = ReadyString(CDate(datenow))

                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(pSql)
                End If

                p = New InsertUpdate("LibrarySurveyRespondent")
                p("ID") = ReadyString(respondentID)
                p("ClientID") = IIf(clientID = "", ReadyString(respondentTempCompanyProfileID), ReadyString(clientID.ToString()))
                p("QuestionaireID") = ReadyString(qID.ToString())
                p("DateResponded") = ReadyString(datenow)
                p("Status") = ReadyString("Pending")
                p("SaveAs") = ReadyString("draft")
                pSql = p.GetInsert()

            End If

            Try
                Common.WebRequest.ProData.Execute(pSql)

                For Each Response As Object In arrResponse
                    Dim errMsg As String = ""
                    pageID = IIf(isSaveFinish, Response("PageID"), pageID)
                    If Response("QuestionType") = "Single Input" Then
                        If SaveSingleInput(Response, pageID, respondentID, errMsg) = False Then
                            Return errMsg
                        End If
                    Else
                        For Each answer As Object In Response("Answer")
                            If SaveAnswer(answer, Response, pageID, respondentID, errMsg) = False Then
                                Return errMsg
                            End If
                        Next answer
                    End If
                Next Response

                If totalScore <> 0 Then
                    pSql = "SELECT ID FROM TransSurveyRespondentScore WHERE QuestionaireID = " & ReadyString(qID) & vbCrLf &
                            " AND RespondentID = " & ReadyString(respondentID)
                    Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    p = New InsertUpdate("TransSurveyRespondentScore")

                    p("QuestionaireID") = ReadyString(qID)
                    p("RespondentID") = ReadyString(respondentID)
                    p("TotalScore") = ReadyString(CDec2(totalScore))
                    p("LastUpdated") = ReadyString(CDate(datenow))

                    If rows.Count = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(rows(0)("ID").ToString()))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                End If

                Common.WebRequest.ProData.CommitTransaction()

                Return Json(New With {Key .Results = "saved", .RespondentID = respondentID, .ClientID = IIf(clientID = "", respondentTempCompanyProfileID, clientID)})
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return Json(New With {Key .Results = ex.Message})
            End Try
        End Function

        Function SaveSingleInput(ByVal Response As Object, ByVal pageID As String, ByVal respondentID As String, ByRef errMsg As String) As String
            Try
                Dim pSql As String

                pSql = "SELECT ID FROM LibrarySurveyResponse WHERE RespondentID = " & ReadyString(respondentID) & vbCrLf &
                                        " AND PageID = " & ReadyString(pageID) & " AND QuestionID = " & ReadyString(Response("ID")) & vbCrLf

                Dim ct As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                If Response("State") = "delete" Then 'only for unrequired question, if before has a previous answer and then change the answer (empty answer), the previous answer must be deleted
                    pSql = "DELETE FROM LibrarySurveyResponse WHERE RespondentID = " & ReadyString(respondentID) & vbCrLf &
                        " AND PageID = " & ReadyString(pageID) & " AND QuestionID = " & ReadyString(Response("ID"))
                    Common.WebRequest.ProData.Execute(pSql)
                Else
                    Dim p = New InsertUpdate("LibrarySurveyResponse")

                    p("RespondentID") = ReadyString(respondentID)
                    p("QuestionID") = ReadyString(Response("ID"))
                    p("Answer") = ReadyString(Response("Answer"))
                    p("IsOther") = "0"
                    If pageID <> "" Then
                        p("PageID") = ReadyString(pageID)
                    End If

                    If ct > 0 Then
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" RespondentID = " & ReadyString(respondentID) & " AND QuestionID = " & ReadyString(Response("ID"))))
                    Else
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If

                End If

                Return True
            Catch ex As Exception
                errMsg = ex.Message
                Return False
            End Try
        End Function

        Function SaveAnswer(ByVal answer As Object, ByVal Response As Object, ByVal pageID As String, ByVal respondentID As String, ByRef errMsg As String) As String
            Try
                Dim ct As Integer = 0
                Dim where As String
                Dim pSql As String

                If Response("QuestionType") = "Multiple Inputs" Or
                    Response("QuestionType") = "radio" Or
                    Response("QuestionType") = "checkbox" Then

                    where = " RespondentID = " & ReadyString(respondentID) & vbCrLf &
                        " AND PageID = " & ReadyString(pageID) & " AND QuestionID = " & ReadyString(Response("ID")) & vbCrLf &
                        " AND FieldID = " & ReadyString(answer("ID"))

                ElseIf Response("QuestionType") = "radioinput" Or
                    Response("QuestionType") = "checkboxinput" Then

                    where = " RespondentID = " & ReadyString(respondentID) & vbCrLf &
                        " AND PageID = " & ReadyString(pageID) & " AND QuestionID = " & ReadyString(Response("ID")) & vbCrLf &
                        " AND FieldID = " & ReadyString(answer("ID")) & vbCrLf
                Else
                    where = " RespondentID = " & ReadyString(respondentID) & vbCrLf &
                        " AND PageID = " & ReadyString(pageID) & " AND QuestionID = " & ReadyString(Response("ID")) & vbCrLf &
                        " AND FieldID = " & ReadyString(answer("ID")) & vbCrLf &
                        " AND MatrixRowID = " & ReadyString(answer("RowID").ToString())

                End If

                pSql = " SELECT ID FROM LibrarySurveyResponse WHERE " & vbCrLf &
                        where
                ct = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                If answer("State") = "delete" Then
                    pSql = " DELETE FROM LibrarySurveyResponse WHERE " & where
                    Common.WebRequest.ProData.Execute(pSql)
                Else
                    Dim p = New InsertUpdate("LibrarySurveyResponse")


                    p("RespondentID") = ReadyString(respondentID)
                    If pageID <> "" Then
                        p("PageID") = ReadyString(pageID)
                    End If
                    p("QuestionID") = ReadyString(Response("ID"))
                    p("FieldID") = ReadyString(answer("ID"))
                    p("Answer") = ReadyString(answer("Answer"))
                    If answer.ContainsKey("IsOther") Then
                        p("IsOther") = CInt2(answer("IsOther"))
                    Else
                        p("IsOther") = 0
                    End If
                    If answer.ContainsKey("RowID") Then
                        p("MatrixRowID") = ReadyString(answer("RowID").ToString())
                    End If
                    If answer.ContainsKey("Description") Then
                        p("Description") = ReadyString(answer("Description").ToString())
                    End If

                    If ct > 0 Then
                        Common.WebRequest.ProData.Execute(p.GetUpdate(where))
                    Else
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If

                    If answer.ContainsKey("Score") Then
                        totalScore += CDec2(answer("Score"))
                    End If

                End If

                If answer.ContainsKey("TunnelAnswer") Then
                    For Each TunnelResponse As Object In answer("TunnelAnswer")
                        If TunnelResponse("QuestionType") = "Single Input" Then
                            If SaveSingleInput(TunnelResponse, pageID, respondentID, errMsg) = False Then
                                Return False
                            End If
                        Else
                            For Each TunnelAnswer As Object In TunnelResponse("Answer")
                                If SaveAnswer(TunnelAnswer, TunnelResponse, pageID, respondentID, errMsg) = False Then
                                    Return False
                                End If
                            Next TunnelAnswer
                        End If
                    Next TunnelResponse
                End If

                Return True
            Catch ex As Exception
                errMsg = ex.Message
                Return False
            End Try
        End Function

        Function GetLinkDetails() As String
            Dim pSql As String = "SELECT * FROM LibrarySurveyQuestionaireLink WHERE Link = " & ReadyString(Common.WebRequest.Data("Link")) & vbCrLf &
                                " COLLATE Latin1_General_CS_AS "
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetQuestionaire() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT qre.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as CreatedByName, " & vbCrLf &
                                " (SELECT Link FROM MobizLibrarySurveyQuestionaireLink WHERE QuestionaireID = qre.ID) as Link, " & vbCrLf &
                                " (SELECT COUNT(ID) FROM LibrarySurveyRespondent WHERE QuestionaireID = qre.ID) as Respondents " & vbCrLf &
                                " FROM LibrarySurveyQuestionaire as qre " & vbCrLf &
                                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = qre.CreatedBy " & vbCrLf &
                                " WHERE qre.ID = " & ReadyString(Common.WebRequest.Data("QID"))
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                Return Json(New With {Key .data = GetSurveyDetails(pDT.Rows(0), 1)})
            Else
                Return JsonDictionary.Serialize("Results", pDT)
            End If
        End Function

        Function GetSurvey() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            PageStart = CInt(Request.Form.GetValues("start")(0))
            PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            IsCountPage = False
            PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            Dim pDTQuestionaire As DataTable = GetSurveyDataTable(False)
            Dim arrSurvey As New List(Of Dictionary(Of String, Object))
            Dim pItemSurvey As New Dictionary(Of String, Object)

            If pDTQuestionaire.Rows.Count > 0 Then
                If pDTQuestionaire.Rows(0).Table.Columns("Error") IsNot Nothing Then
                    Return pDTQuestionaire.Rows(0)("ERROR")
                End If

                For i As Integer = 0 To pDTQuestionaire.Rows.Count - 1
                    Dim survey As Object = pDTQuestionaire.Rows(i)

                    arrSurvey.Add(GetSurveyDetails(survey, survey("RowNum")))
                Next
            End If

            IsCountPage = True
            Dim pDTCountPage As DataTable = GetSurveyDataTable(False)

            CreateTempProfileForUnknownRespondent()

            Return Json(New With {Key .data = arrSurvey, .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
        End Function

        Function CreateTempProfileForUnknownRespondent() As String
            Try
                Dim pSql As String = "select * from librarysurveyrespondent where isnull(convert(varchar(max), clientid), '') = ''"
                Dim pDt As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If pDt.Count > 0 Then
                    Dim ctrRes As Integer = 1
                    For Each res As Object In pDt
                        Dim p As New InsertUpdate("LibraryOrganization")
                        Dim clientID = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(clientID)
                        p("CompanyName") = ReadyString("Respondent " & (CDec2(ctrRes)))
                        p("LastUpdated") = "getdate()"

                        pSql = p.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)

                        p = New InsertUpdate("LibrarySurveyRespondent")
                        p("ClientID") = ReadyString(clientID)

                        pSql = p.GetUpdate(" ID = " & ReadyString(res("ID").ToString()))
                        Common.WebRequest.ProData.Execute(pSql)

                        ctrRes += 1
                    Next res
                End If
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try
        End Function

        Function GetSurveyDetails(ByVal survey As Object, ByVal RowNum As Integer, Optional ByVal byClient As Boolean = False) As Object
            Dim pItemSurvey As New Dictionary(Of String, Object)
            Dim pSql As String = "SELECT * FROM LibrarySurveyPage WHERE QuestionaireID = " & ReadyString(survey("ID").ToString()) & " ORDER BY PageNo ASC"
            Dim pDTPage As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim arrPage As New List(Of Dictionary(Of String, Object))

            If pDTPage.Rows.Count > 0 Then
                For p As Integer = 0 To pDTPage.Rows.Count - 1
                    Dim page As Object = pDTPage.Rows(p)
                    pSql = "SELECT q.*, COUNT(tunnel.TunneledTo) as TunnelCount FROM LibrarySurveyQuestions as q " & vbCrLf &
                            " LEFT JOIN LibrarySurveyQuestionFieldsTunnel as tunnel ON q.ID = tunnel.TunneledTo " & vbCrLf &
                            " WHERE PageID = " & ReadyString(page("ID")) & vbCrLf &
                            " AND ISNULL(q.IsDelete, 0) = 0 " & vbCrLf &
                            " GROUP by q.VariableName, q.IsDelete, q.ID, q.QuestionaireID, q.Question, q.Type, q.IsRequired, q.HasOther, q.LastUpdated, q.IsUseCheckbox, q.PageID, q.OrderNo, q.IsRadioButtonPresentAsSelectMenu " & vbCrLf &
                            " ORDER BY q.OrderNo ASC "

                    Dim pDTQuestions As DataTable = Common.WebRequest.ProData.Execute(pSql)
                    Dim arrQuestions As New List(Of Dictionary(Of String, Object))
                    Dim pItemPage As New Dictionary(Of String, Object)

                    If pDTQuestions.Rows.Count > 0 Then
                        For m As Integer = 0 To pDTQuestions.Rows.Count - 1
                            Dim question As Object = pDTQuestions.Rows(m)
                            Dim pItemQuestion As New Dictionary(Of String, Object)

                            pItemQuestion.Add("ID", question("ID"))
                            pItemQuestion.Add("PageID", page("ID"))
                            pItemQuestion.Add("QuestionType", question("Type"))
                            pItemQuestion.Add("Question", HttpUtility.HtmlEncode(question("Question")))
                            pItemQuestion.Add("Required", question("IsRequired"))
                            pItemQuestion.Add("VariableName", CStr2(question("VariableName")))
                            pItemQuestion.Add("TunnelCount", question("TunnelCount"))
                            pItemQuestion.Add("IsUseCheckbox", CBool2(question("IsUseCheckbox")))
                            pItemQuestion.Add("IsRadioButtonPresentAsSelectMenu", CInt2(question("IsRadioButtonPresentAsSelectMenu")))
                            pItemQuestion.Add("OrderNo", CInt2(question("OrderNo")))
                            pItemQuestion.Add("State", "edit")
                            pItemQuestion.Add("Index", m)

                            If question("Type") = "radioinput" Or question("Type") = "checkboxinput" Then
                                pItemQuestion.Add("IsWithField", True)
                            End If

                            If question("Type") <> "Single Input" Then
                                pSql = "SELECT field.*, tunnel.TunneledTo FROM LibrarySurveyQuestionFields as field " & vbCrLf &
                                    " LEFT JOIN LibrarySurveyQuestionFieldsTunnel as tunnel ON tunnel.FieldID = field.ID " & vbCrLf &
                                    " WHERE QuestionID = " & ReadyString(question("ID")) & vbCrLf &
                                    " AND ISNULL(field.IsDelete, 0) = 0 ORDER BY field.OrderNo ASC"
                                Dim pDTFields As DataTable = Common.WebRequest.ProData.Execute(pSql)
                                Dim arrFields As New List(Of Dictionary(Of String, Object))
                                Dim arrRows As New List(Of Dictionary(Of String, Object))
                                Dim arrColumns As New List(Of Dictionary(Of String, Object))

                                If pDTFields.Rows.Count > 0 Then
                                    For n As Integer = 0 To pDTFields.Rows.Count - 1
                                        Dim fields As Object = pDTFields.Rows(n)
                                        Dim pItemFields As New Dictionary(Of String, Object)
                                        Dim arrReq As New List(Of String)

                                        pSql = "SELECT * FROM LibrarySurveyQuestionFieldRequirements WHERE FieldID = " & ReadyString(fields("ID").ToString())
                                        Dim rowsReq As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                                        For Each req As Object In rowsReq
                                            arrReq.Add(req("RequirementID").ToString())
                                        Next req

                                        If question("Type") <> "Matrix" Then
                                            If CBool2(fields("IsOther")) Then
                                                Dim arrOther As Object
                                                arrOther = New With {Key .AsType = fields("AsType"),
                                                                        .Label = fields("Label"),
                                                                        .Score = fields("Score"),
                                                                        .ID = fields("ID").ToString(),
                                                                        .VariableName = fields("VariableName").ToString()}
                                                pItemQuestion.Add("Other", arrOther)
                                            Else
                                                pItemFields.Add("ID", fields("ID").ToString())
                                                pItemFields.Add("Label", fields("Label"))
                                                pItemFields.Add("Score", fields("Score"))
                                                pItemFields.Add("Value", CStr2(fields("Value")))
                                                pItemFields.Add("TunneledTo", CStr2(fields("TunneledTo")))
                                                pItemFields.Add("VariableName", CStr2(fields("VariableName")))
                                                pItemFields.Add("State", "edit")
                                                pItemFields.Add("IsRequired", CBool2(fields("IsRequired")))
                                                pItemFields.Add("DataType", CStr2(fields("DataType")))
                                                pItemFields.Add("CharMax", CInt2(fields("CharMax")))
                                                pItemFields.Add("CharMin", CInt2(fields("CharMin")))
                                                pItemFields.Add("OrderNo", CInt2(fields("OrderNo")))
                                                pItemFields.Add("Requirements", arrReq)
                                                arrFields.Add(pItemFields)
                                            End If
                                        Else
                                            pItemFields.Add("ID", fields("ID").ToString())
                                            pItemFields.Add("Label", fields("Label"))
                                            pItemFields.Add("Score", fields("Score"))
                                            pItemFields.Add("Control", fields("ControlType"))
                                            pItemFields.Add("Type", fields("AsType"))
                                            pItemFields.Add("State", "edit")
                                            pItemFields.Add("IsRequired", CBool2(fields("IsRequired")))
                                            pItemFields.Add("DataType", CStr2(fields("DataType")))
                                            pItemFields.Add("CharMax", CInt2(fields("CharMax")))
                                            pItemFields.Add("CharMin", CInt2(fields("CharMin")))
                                            pItemFields.Add("OrderNo", CInt2(fields("OrderNo")))
                                            pItemFields.Add("VariableName", CStr2(fields("VariableName")))

                                            If fields("AsType") = "Row" Then
                                                arrRows.Add(pItemFields)
                                            Else
                                                arrColumns.Add(pItemFields)
                                            End If
                                        End If

                                    Next
                                    If question("Type") = "Matrix" Then
                                        Dim matrixFields As New Dictionary(Of String, Object)
                                        matrixFields.Add("Rows", arrRows)
                                        matrixFields.Add("Columns", arrColumns)
                                        pItemQuestion.Add("Fields", matrixFields)
                                    Else
                                        pItemQuestion.Add("Fields", arrFields)
                                    End If
                                End If
                            End If

                            arrQuestions.Add(pItemQuestion)
                        Next
                    End If

                    pItemPage.Add("ID", page("ID").ToString())
                    pItemPage.Add("Title", page("Title").ToString())
                    pItemPage.Add("PageNo", page("PageNo").ToString())
                    pItemPage.Add("State", "edit")
                    pItemPage.Add("Index", p)
                    pItemPage.Add("Questions", arrQuestions)
                    pItemPage.Add("PrevLabel", page("PrevLabel").ToString())
                    pItemPage.Add("PrevAction", page("PrevAction").ToString())
                    pItemPage.Add("PrevGoto", page("PrevGoto").ToString())
                    pItemPage.Add("NextLabel", page("NextLabel").ToString())
                    pItemPage.Add("NextAction", page("NextAction").ToString())
                    pItemPage.Add("NextGoto", page("NextGoto").ToString())
                    pItemPage.Add("NextGotoLink", page("NextGotoLink").ToString())
                    pItemPage.Add("IsShowButton", page("IsShowButton").ToString())
                    arrPage.Add(pItemPage)
                Next
            End If

            pSql = "SELECT req.*, mrt.FileName, 'edit' as State, g.ID as ReqGroupID, g.Name as RequiredBy, CONVERT(VARCHAR(MAX),req.Lastupdated,120) as ReqLastUpdated FROM LibrarySurveyRequirements as req " & vbCrLf &
                    " LEFT JOIN LibrarySurveyFiles as mrt ON mrt.ID = req.MrtFileID " & vbCrLf &
                    " LEFT JOIN LibrarySurveyGroupRequirements as g ON g.ID = req.GroupRequirementID " & vbCrLf &
                    " WHERE req.QuestionaireID = " & ReadyString(survey("ID").ToString())

            pItemSurvey.Add("Requirements", jss.Deserialize(Of Object)(DataTableToJSON.GetJson((Common.WebRequest.ProData.Execute(pSql)))))
            pItemSurvey.Add("ID", survey("ID"))
            pItemSurvey.Add("Title", survey("Title"))
            pItemSurvey.Add("CreatedBy", survey("CreatedBy"))
            pItemSurvey.Add("IsActive", survey("IsActive"))
            pItemSurvey.Add("Link", survey("Link"))
            pItemSurvey.Add("Respondents", survey("Respondents"))
            pItemSurvey.Add("LastUpdated", Format(IIf(CStr2(survey("LastUpdated")) = "", survey("DateCreated"), survey("LastUpdated")), "MMM dd, yyyy hh:mm:ss tt"))
            pItemSurvey.Add("CreatedByName", survey("CreatedByName"))
            pItemSurvey.Add("Pages", arrPage)

            pSql = "SELECT *, 'edit' as State FROM LibrarySurveyGroupRequirements WHERE QuestionaireID = " & ReadyString(survey("ID").ToString()) & vbCrLf &
                    " ORDER BY OrderNo ASC"

            pItemSurvey.Add("ReqGroups", jss.Deserialize(Of Object)(DataTableToJSON.GetJson((Common.WebRequest.ProData.Execute(pSql)))))

            If survey.Table.Columns.Contains("Score") Then
                pItemSurvey.Add("Score", survey("Score"))
            End If
            pItemSurvey.Add("RowNum", RowNum)

            If byClient Then
                pItemSurvey.Add("DateResponded", Format(survey("DateResponded"), "MMM dd, yyyy hh:mm:ss tt"))
                pItemSurvey.Add("RespondentID", survey("RespondentID"))
            End If

            Return pItemSurvey
        End Function

        Function GetSurveyDataTable(ByVal byClient As Boolean) As DataTable
            Dim pSql As String
            Dim pDT As DataTable
            Dim whereSearch As String = ""
            Dim whereOrder As String = ""
            Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))
            Dim status As String = Request.Form.GetValues("columns[0][search][value]")(0).ToString()

            Try
                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " OR "
                        End If

                        whereSearchContent = whereSearchContent & operators & " Title + ' ' + " & vbCrLf &
                                        "CONVERT(varchar, Respondents) + ' ' + " & vbCrLf &
                                        "CONVERT(varchar, CreatedByName) " & vbCrLf &
                                        " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next
                    whereSearch = " WHERE (" & whereSearchContent & ")"
                End If

                If IsCountPage = True Then
                    whereLimit = ""
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.Title " & PageColumnDirection
                ElseIf PageColumnNo = 2 Then
                    whereOrder = " ORDER BY mainsql.Respondents " & PageColumnDirection
                ElseIf PageColumnNo = 3 Then
                    whereOrder = " ORDER BY mainsql.CreatedByName " & PageColumnDirection
                    'ElseIf PageColumnNo = 4 Then
                    '    whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
                End If

                pSql = "SELECT * FROM  (Select mainsql.*, " & vbCrLf &
                        " ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf

                If byClient Then
                    pSql += "SELECT qre.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as CreatedByName, " & vbCrLf &
                        " (SELECT Link FROM MobizLibrarySurveyQuestionaireLink WHERE QuestionaireID = qre.ID) as Link, " & vbCrLf &
                        " (SELECT COUNT(ID) FROM LibrarySurveyRespondent WHERE QuestionaireID = qre.ID) as Respondents, " & vbCrLf &
                        " (SELECT ID FROM LibrarySurveyRespondent WHERE QuestionaireID = qre.ID AND ClientID = " & ReadyString(CStr2(Request.Params("ClientID"))) & ") as RespondentID, " & vbCrLf &
                        " res.DateResponded, " & vbCrLf &
                        " (SELECT ISNULL(SUM(Score), 0) as Score FROM librarysurveyrespondent as respondent" & vbCrLf &
                        "   INNER JOIN librarysurveyresponse as response ON response.RespondentID = respondent.ID " & vbCrLf &
                        "   INNER JOIN librarysurveyquestionfields as field ON field.ID = response.FieldID " & vbCrLf &
                        "   WHERE respondent.ClientID = " & ReadyString(CStr2(Request.Params("ClientID"))) & ") as Score " & vbCrLf &
                        " FROM LibrarySurveyRespondent as res " & vbCrLf &
                        " LEFT JOIN LibrarySurveyQuestionaire as qre ON res.QuestionaireID = qre.ID " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = qre.CreatedBy " & vbCrLf &
                        " WHERE res.ClientID = " & ReadyString(CStr2(Request.Params("ClientID"))) & vbCrLf
                Else
                    pSql += " SELECT qre.*, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname as CreatedByName, " & vbCrLf &
                        " (SELECT COUNT(ID) FROM LibrarySurveyRespondent WHERE QuestionaireID = qre.ID) as Respondents, " & vbCrLf &
                        " (SELECT Link FROM MobizLibrarySurveyQuestionaireLink WHERE QuestionaireID = qre.ID) as Link, " & vbCrLf &
                        " 0 as Score " & vbCrLf &
                        " FROM LibrarySurveyQuestionaire as qre " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = qre.CreatedBy  " & vbCrLf &
                        " WHERE ISNULL(qre.UseInModule, 'crm') = " & ReadyString(Request.Params.Get("mod")) & vbCrLf &
                        If(status = "", " AND qre.IsActive = '1' ", If(status = "All", "", " AND qre.IsActive = " & ReadyString(status))) & vbCrLf
                End If

                pSql += ") as mainsql " & whereSearch & ") as v2 " & whereLimit & vbCrLf

                pDT = Common.WebRequest.ProData.Execute(pSql)
                Return pDT

            Catch ex As Exception
                Dim err As New DataTable()
                err.Columns.Add("Error")
                err.Rows.Add(New Object() {ex.Message})
                AppendToLog(ex.Message)
                Return err
            End Try
        End Function

        Function SaveQuestionaire()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim title As String = Common.WebRequest.Data("Title")
            'Dim question As Object = Common.WebRequest.AllData("Questions")
            Dim pages As Object = Common.WebRequest.AllData("Pages")
            Dim titleID As String = Common.WebRequest.Data("TitleID")
            Dim datenow As String = Common.WebRequest.Data("Date")
            Dim titleState As String = Common.WebRequest.Data("State")
            Dim requirements As Object = Common.WebRequest.AllData("Requirements")
            Dim reqgroup As Object = Common.WebRequest.AllData("ReqGroup")
            Dim p As New InsertUpdate("LibrarySurveyQuestionaire")
            Dim pSql As String
            Dim orderno As Integer = 1
            Dim qcode As String = ""

            Try
                p("Title") = ReadyString(title)
                p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("IsActive") = ReadyString(CBool2(Common.WebRequest.Data("IsActive")))
                p("UseInModule") = ReadyString(Request.Params.Get("mod"))

                Common.WebRequest.ProData.BeginTransaction()

                If titleID = "" Then
                    qcode = GenerateShortLink()
                    titleID = Guid.NewGuid.ToString()
                    p("DateCreated") = ReadyString(CDate(datenow))
                    p("ID") = ReadyString(titleID)
                    pSql = p.GetInsert()

                    p = New InsertUpdate("MobizLibrarySurveyQuestionaireLink")
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("OrgID") = ReadyString(WebRequest.Instance.Data("OrgID"))
                    p("QuestionaireID") = ReadyString(titleID)
                    p("Link") = ReadyString(qcode)
                    Common.WebRequest.ProData.Execute(p.GetInsert())

                Else
                    If titleState = "delete" Then
                        p("IsActive") = "0"
                    End If
                    p("LastUpdated") = ReadyString(CDate(datenow))
                    pSql = p.GetUpdate(" ID = " & ReadyString(titleID))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                For Each req As Object In reqgroup
                    p = New InsertUpdate("LibrarySurveyGroupRequirements")

                    p("Name") = ReadyString(req("Name"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = ReadyString(CDate(datenow))
                    p("QuestionaireID") = ReadyString(titleID)
                    p("OrderNo") = IIf(CDec2(req("OrderNo")) = 0, orderno, req("OrderNo"))

                    If req("State") = "new" Then
                        p("ID") = ReadyString(req("ID").ToString())
                        pSql = p.GetInsert()
                    ElseIf req("State") = "edit" Then
                        pSql = p.GetUpdate("ID = " & ReadyString(req("ID").ToString()))
                    Else
                        pSql = "DELETE FROM LibrarySurveyGroupRequirements WHERE ID = " & ReadyString(req("ID").ToString())
                    End If

                    Common.WebRequest.ProData.Execute(pSql)

                    orderno += 1
                Next req

                Dim ctrPage As Integer = 1
                For Each page As Object In pages
                    p = New InsertUpdate("LibrarySurveyPage")

                    p("QuestionaireID") = ReadyString(titleID)
                    p("PageNo") = ReadyString(ctrPage)
                    p("Title") = ReadyString(page("Title").ToString())
                    p("PrevLabel") = ReadyString(page("PrevLabel").ToString())
                    p("PrevAction") = ReadyString(page("PrevAction").ToString())
                    p("PrevGoto") = ReadyString(page("PrevGoto").ToString())
                    p("NextLabel") = ReadyString(page("NextLabel").ToString())
                    p("NextAction") = ReadyString(page("NextAction").ToString())
                    p("NextGoto") = ReadyString(page("NextGoto").ToString())
                    p("NextGotoLink") = ReadyString(page("NextGotoLink").ToString())
                    p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID())
                    p("IsShowButton") = ReadyString(page("IsShowButton").ToString())
                    p("LastUpdated") = "getdate()"

                    If page("State") = "new" And page("Questions").length > 0 Then
                        p("ID") = ReadyString(page("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    ElseIf page("State") = "edit" Then
                        Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(page("ID").ToString())))
                    Else
                        Common.WebRequest.ProData.Execute("DELETE FROM LibrarySurveyPage WHERE ID = " & ReadyString(page("ID").ToString()))
                    End If

                    If page("State") <> "remove" Then
                        orderno = 1
                        For Each q As Object In page("Questions")
                            Dim questionID As String

                            If q.ContainsKey("TempID") Then
                                questionID = q("TempID").ToString()
                            Else
                                questionID = Guid.NewGuid.ToString()
                            End If

                            p = New InsertUpdate("LibrarySurveyQuestions")

                            p("PageID") = ReadyString(page("ID").ToString())
                            p("QuestionaireID") = ReadyString(titleID)
                            If q.ContainsKey("Question") Then
                                p("Question") = ReadyString(HttpUtility.HtmlDecode(q("Question").ToString()))
                            End If
                            p("Type") = ReadyString(q("QuestionType"))
                            If q.ContainsKey("Required") Then
                                p("IsRequired") = ReadyString(CBool(q("Required")))
                            End If
                            If q.ContainsKey("IsRadioButtonPresentAsSelectMenu") Then
                                p("IsRadioButtonPresentAsSelectMenu") = ReadyString(q("IsRadioButtonPresentAsSelectMenu"))
                            End If
                            p("HasOther") = IIf(q.ContainsKey("Other"), 1, 0)
                            p("LastUpdated") = ReadyString(CDate(datenow))
                            ' ERROR ??? p("IsUseCheckbox") = IIf(q.ContainsKey("IsUseCheckbox") = False, 0, CBool2(q("IsUseCheckbox")))
                            If q.ContainsKey("IsUseCheckbox") = False Then 'use for matrix
                                p("IsUseCheckbox") = 0
                            Else
                                p("IsUseCheckbox") = CInt2(q("IsUseCheckbox"))
                            End If

                            If q.ContainsKey("OrderNo") Then
                                p("OrderNo") = q("OrderNo")
                            Else
                                p("OrderNo") = orderno
                            End If

                            If q("State") = "remove" Then
                                p("IsDelete") = "1"
                            Else
                                p("IsDelete") = "0"
                            End If

                            If q("State") = "new" Then

                                If q("QuestionType") = "Single Input" Then
                                    p("VariableName") = ReadyString(CheckVariableName("LibrarySurveyQuestions", titleID, Left(q("Question").ToString().Replace(" ", "_"), 126))) 'limit character var name to 128 - wala lang
                                End If

                                p("ID") = ReadyString(questionID)
                                pSql = p.GetInsert()
                            ElseIf q("State") = "edit" Then

                                If q("QuestionType") = "Single Input" And q("VariableName") = "" Then
                                    p("VariableName") = ReadyString(CheckVariableName("LibrarySurveyQuestions", titleID, Left(q("Question").ToString().Replace(" ", "_"), 126))) 'limit character var name to 128 - wala lang
                                End If

                                questionID = q("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(questionID))
                            Else
                                questionID = q("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(questionID))
                                Common.WebRequest.ProData.Execute(pSql)
                                pSql = "DELETE FROM LibrarySurveyResponse WHERE QuestionID = " & ReadyString(questionID)
                                Common.WebRequest.ProData.Execute(pSql)
                                pSql = "DELETE FROM LibrarySurveyQuestionFields WHERE QuestionID = " & ReadyString(questionID)
                            End If

                            Common.WebRequest.ProData.Execute(pSql)

                            If q("QuestionType") <> "Single Input" And q("QuestionType") <> "file" And q("QuestionType") <> "Page Text" And q("State") <> "remove" Then
                                Dim dField As String
                                If q("QuestionType") <> "Matrix" Then
                                    dField = SaveSurveyFields(q("Fields"), questionID, q("Question"), titleID)
                                    If Not CBool2(dField) Then
                                        JsonDictionary.Serialize("Results", dField)
                                    End If
                                Else
                                    dField = SaveSurveyFields(q("Fields")("Rows"), questionID, q("Question"), titleID)
                                    If CBool2(dField) Then
                                        dField = SaveSurveyFields(q("Fields")("Columns"), questionID, q("Question"), titleID)
                                        If Not CBool2(dField) Then
                                            JsonDictionary.Serialize("Results", dField)
                                        End If
                                    Else
                                        JsonDictionary.Serialize("Results", dField)
                                    End If
                                End If

                                If q.ContainsKey("Other") Then
                                    p = New InsertUpdate("LibrarySurveyQuestionFields")
                                    p("QuestionID") = ReadyString(questionID)
                                    p("Label") = ReadyString(q("Other")("Label"))
                                    p("AsType") = ReadyString(q("Other")("AsType"))
                                    p("Score") = CDec2(q("Other")("Score"))
                                    p("IsOther") = 1

                                    If q("Other").ContainsKey("ID") Then
                                        If q("Other")("VariableName").ToString() = "" Then
                                            p("VariableName") = ReadyString(CheckVariableName("LibrarySurveyQuestionFields", titleID, "Others" & "__" & Left(q("Question").ToString().Replace(" ", "_"), 119)))
                                        End If

                                        pSql = p.GetUpdate(" ID = " & ReadyString(q("Other")("ID")))
                                    Else
                                        If Not q("Other").ContainsKey("VariableName") Then
                                            p("VariableName") = ReadyString(CheckVariableName("LibrarySurveyQuestionFields", titleID, "Others" & "__" & Left(q("Question").ToString().Replace(" ", "_"), 119)))
                                        End If

                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        pSql = p.GetInsert()
                                    End If
                                Else
                                    pSql = "DELETE FROM LibrarySurveyQuestionFields WHERE QuestionID = " & ReadyString(questionID) &
                                            " AND ISNULL(IsOther, '') = 1"
                                End If

                                Common.WebRequest.ProData.Execute(pSql)
                            End If

                            orderno += 1
                        Next q
                    End If

                    ctrPage += 1
                Next page

                'UPDATE ALSO THE SCORE OF THE PREVIOUS RESPONDENTS 
                pSql = "SELECT * FROM LibrarySurveyRespondent WHERE QuestionaireID = " & ReadyString(titleID)
                Dim respondent As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                For Each res As Object In respondent
                    pSql = "SELECT ID FROM TransSurveyRespondentScore WHERE QuestionaireID = " & ReadyString(titleID) & vbCrLf &
                            " AND RespondentID = " & ReadyString(res("ID").ToString())
                    Dim c As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    pSql = "SELECT ISNULL(SUM(Score), 0) as Score FROM LibrarySurveyResponse as res " & vbCrLf &
                            " INNER JOIN LibrarySurveyQuestionFields as fields ON res.FieldID = fields.ID " & vbCrLf &
                            " WHERE RespondentID = " & ReadyString(res("ID").ToString())
                    Dim score As Decimal = Common.WebRequest.ProData.Execute(pSql).Rows(0)("Score")

                    p = New InsertUpdate("TransSurveyRespondentScore")
                    p("TotalScore") = ReadyString(score)
                    p("LastUpdated") = ReadyString(CDate(datenow))

                    If c.Count = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("QuestionaireID") = ReadyString(titleID)
                        p("RespondentID") = ReadyString(res("ID").ToString())

                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(c(0)("ID").ToString()))
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Next res

                Common.WebRequest.ProData.CommitTransaction()

                Dim pOut As New JsonDictionary

                pOut.Add("Results", "saved")
                pOut.Add("code", qcode)

                Return pOut.Serialize() 'JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveSurveyFields(ByRef data As Object, ByVal questionID As String, ByVal question As String, ByVal questionaireID As String) As String
            Dim pSql As String
            Dim p As New InsertUpdate("LibrarySurveyQuestionFields")
            Dim orderno As Integer = 1

            Try
                For Each f As Object In data
                    Dim fieldID As String = Guid.NewGuid.ToString()
                    p = New InsertUpdate("LibrarySurveyQuestionFields")

                    p("QuestionID") = ReadyString(questionID)
                    p("Label") = ReadyString(f("Label"))

                    If f.ContainsKey("Value") Then
                        p("Value") = ReadyString(f("Value"))
                    End If

                    If f.ContainsKey("Type") Then
                        p("AsType") = ReadyString(f("Type"))
                    End If

                    If f.ContainsKey("Score") Then
                        p("Score") = CDec2(f("Score"))
                    Else
                        p("Score") = 0
                    End If

                    If f.ContainsKey("OrderNo") Then
                        p("OrderNo") = CDec2(f("OrderNo"))
                    Else
                        p("OrderNo") = orderno
                    End If

                    orderno += 1

                    If f.ContainsKey("IsRequired") Then
                        p("IsRequired") = ReadyString(f("IsRequired"))
                    End If

                    If f.ContainsKey("Control") Then 'Use only in Matrix (Column)
                        p("ControlType") = ReadyString(f("Control"))
                    End If

                    If f.ContainsKey("DataType") Then 'Use only in TEXT FIELDS
                        p("DataType") = ReadyString(f("DataType"))
                    End If

                    If f.ContainsKey("CharMax") Then 'Use only in TEXT FIELDS
                        p("CharMax") = ReadyString(f("CharMax"))
                    End If

                    If f.ContainsKey("CharMin") Then 'Use only in TEXT FIELDS
                        p("CharMin") = ReadyString(f("CharMin"))
                    End If

                    p("IsOther") = 0

                    If f("State") = "remove" Then
                        p("IsDelete") = "1"
                    Else
                        p("IsDelete") = "0"
                    End If

                    If f("State") = "new" Then

                        p("VariableName") = ReadyString(CheckVariableName("LibrarySurveyQuestionFields", questionaireID, Left(question.Replace(" ", "_"), 62) & "__" & Left(f("Label").ToString().Replace(" ", "_"), 62))) 'limit character var name to 20 - wala lang

                        If f.ContainsKey("TempID") Then
                            fieldID = f("TempID").ToString()
                        End If

                        p("ID") = ReadyString(fieldID)
                        pSql = p.GetInsert()
                    Else

                        If f("State") = "edit" And f("VariableName") = "" Then
                            p("VariableName") = ReadyString(CheckVariableName("LibrarySurveyQuestionFields", questionaireID, Left(question.Replace(" ", "_"), 62) & "__" & Left(f("Label").ToString().Replace(" ", "_"), 62))) 'limit character var name to 20 - wala lang
                        End If

                        fieldID = f("ID").ToString()
                        pSql = p.GetUpdate(" ID = " & ReadyString(fieldID))
                    End If
                    Common.WebRequest.ProData.Execute(pSql)

                    If f.Containskey("Requirements") Then
                        Dim arr As New List(Of String)
                        For i As Integer = 0 To f("Requirements").Length - 1
                            pSql = "SELECT ID FROM LibrarySurveyQuestionFieldRequirements WHERE RequirementID = " & ReadyString(f("Requirements")(i).ToString()) & vbCrLf &
                                    " AND FieldID = " & ReadyString(fieldID.ToString())
                            Dim ctr As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                            pSql = "SELECT ID FROM LibrarySurveyRequirements WHERE ID = " & ReadyString(f("Requirements")(i).ToString()) & vbCrLf
                            Dim isExist As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                            If ctr = 0 And isExist > 0 Then
                                p = New InsertUpdate("LibrarySurveyQuestionFieldRequirements")

                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("FieldID") = ReadyString(fieldID.ToString())
                                p("RequirementID") = ReadyString(f("Requirements")(i).ToString())
                                pSql = p.GetInsert()

                                Common.WebRequest.ProData.Execute(pSql)
                            End If

                            arr.Add(f("Requirements")(i).ToString())
                        Next

                        If arr.Count <> 0 Then
                            'idelete ang requirement na wala sa gipilian
                            pSql = "DELETE FROM LibrarySurveyQuestionFieldRequirements WHERE FieldID = " & ReadyString(fieldID.ToString()) & vbCrLf &
                                    " AND RequirementID NOT IN ('" & String.Join("', '", arr) & "')"
                            Common.WebRequest.ProData.Execute(pSql)
                        End If
                    End If

                    If f("State") <> "remove" Then
                        If f.ContainsKey("TunneledTo") Then
                            pSql = "SELECT * FROM LibrarySurveyQuestionFieldsTunnel WHERE FieldID = " & ReadyString(fieldID)
                            Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                            If rows.Count = 0 Then
                                If CStr2(f("TunneledTo").ToString()) <> "" Then
                                    p = New InsertUpdate("LibrarySurveyQuestionFieldsTunnel")

                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("FieldID") = ReadyString(fieldID)
                                    p("TunneledTo") = ReadyString(f("TunneledTo"))
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                    Common.WebRequest.ProData.Execute(p.GetInsert())
                                End If
                            Else
                                If CStr2(f("TunneledTo")) = "" Then
                                    pSql = "DELETE FROM LibrarySurveyQuestionFieldsTunnel WHERE ID = " & ReadyString(rows(0)("ID").ToString())
                                    Common.WebRequest.ProData.Execute(pSql)
                                ElseIf CStr2(f("TunneledTo")) <> rows(0)("TunneledTo").ToString() Then
                                    p = New InsertUpdate("LibrarySurveyQuestionFieldsTunnel")
                                    p("TunneledTo") = ReadyString(CStr2(f("TunneledTo").ToString()))
                                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(rows(0)("ID").ToString())))
                                End If
                            End If
                        End If
                    Else
                        pSql = "DELETE FROM LibrarySurveyQuestionFieldsTunnel WHERE FieldID = " & ReadyString(fieldID)
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next f

                Return True
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return ex.Message
            End Try

        End Function

        Function CheckVariableName(ByVal table As String, ByVal id As String, ByVal variablename As String, Optional ByVal c As Integer = 0) As String
            Dim pSql As String = "SELECT " & table & ".ID FROM " & table & vbCrLf &
                                    IIf(table = "LibrarySurveyQuestionFields", " INNER JOIN LibrarySurveyQuestions ON LibrarySurveyQuestions.ID = " & table & ".QuestionID ", "") & vbCrLf &
                                    " WHERE " & table & ".VariableName =  " & ReadyString(variablename & IIf(c = 0, "", "_" & c)) & vbCrLf &
                                    " AND LibrarySurveyQuestions.QuestionaireID = " & ReadyString(id) & vbCrLf

            Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            Dim ctr As Integer = 1 + c

            If rows.Count > 0 Then
                Return CheckVariableName(table, id, variablename, ctr)
            Else
                Return variablename & IIf(c = 0, "", "_" & c)
            End If
        End Function

        Public Function GenerateShortLink() As String
            Dim rand As New Random()
            Dim allowableChars() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
            Dim link As String

            For i As Integer = 0 To 10 - 1
                link += allowableChars(rand.Next(allowableChars.Length - 1))
            Next

            'CHECK IF GENERATED LINK EXIST
            Dim pSql As String = "SELECT Link FROM MobizLibrarySurveyQuestionaireLink WHERE Link = " & ReadyString(link)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                link = GenerateShortLink() 'replace the old link with the new generated link
            End If

            Return link
        End Function

        '-----------------------------------
        '------- BELOW IS FOR CRM ----------
        '-----------------------------------
        Function SurveyCreator() As ActionResult
            Return View()
        End Function

        Function RemoveCase() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim pDt As DataTable

            Try
                pSql = "SELECT * FROM TransSurvey WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ID"))
                pDt = Common.WebRequest.ProData.Execute(pSql)

                If pDt.Rows.Count > 0 Then
                    For Each sol As Object In pDt.Rows
                        pSql = "DELETE FROM TransSurvey WHERE SolutionID = " & ReadyString(sol("ID"))
                        Common.WebRequest.ProData.Execute(pSql)
                    Next
                End If

                pSql = "DELETE FROM TransSurvey WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "DELETE FROM TransSurvey WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "removed")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function GetSearchResult() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT * FROM (Select t2.* , ROW_NUMBER() OVER (ORDER BY t2.ProblemID ASC) AS RowNum From(SELECT * FROM (Select DISTINCT " & vbCrLf &
                    "mkm.ID as ProblemID, mkm.CaseNo, ISNULL(mkm.SubTopic, '') as SubTopicID, mkm.Problem,  " & vbCrLf &
                    "lkm1.Topic, lkm1.ID as TopicID, lkm2.Product, lkm2.ID as ProductID, lkm3.Language, lkm3.ID as LanguageID, mkm.PeopleID, " & vbCrLf &
                    "mkm.Tags, CONVERT(VARCHAR(10),mkm.Lastupdated,110) LastUpdated,  " & vbCrLf &
                    "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName EditedBy,  " & vbCrLf &
                    "(SELECT DISTINCT STUFF((SELECT DISTINCT ', '+ km2.Solution " & vbCrLf &
                    "       FROM TransSurvey as km2 " & vbCrLf &
                    "       WHERE km2.ProblemID = mkm.ID " & vbCrLf &
                    "   FOR XML PATH('')" & vbCrLf &
                    "), 1, 1,'') FROM TransSurvey) as Solution " & vbCrLf &
                    "from TransSurvey as mkm " & vbCrLf &
                    "inner join MobizLibraryPeople on MobizLibraryPeople.ID = mkm.PeopleID  " & vbCrLf &
                    "inner join LibrarySurvey lkm1 on mkm.Topic = lkm1.ID AND lkm1.Topic IS NOT NULL " & vbCrLf &
                    "inner join LibrarySurvey lkm2 on mkm.Product = lkm2.ID AND lkm2.Product IS NOT NULL " & vbCrLf &
                    "inner join LibrarySurvey lkm3 on mkm.Language = lkm3.ID AND lkm3.Language IS NOT NULL) " & vbCrLf &
                    "as t1 " & vbCrLf &
                    "WHERE Problem LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Topic LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Product LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " " & vbCrLf &
                    "OR Language LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Tags LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & " OR Solution LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKey")) & ") " & vbCrLf &
                    "as t2) as t2 WHERE RowNum >= 1 AND RowNum <= 10 ORDER BY Problem ASC"

            Try
                pDt = Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return JsonDictionary.Serialize("Results", pDt)

        End Function

        <HttpPost()>
        Function GetAllKnowledgeManagementAndQuestionaire() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'pSql = "Select DISTINCT CaseNo, CaseDescription, Solution, Topic, Product, Language, PeopleID, Tags, CONVERT(VARCHAR(10),TransSurvey.Lastupdated,110) LastUpdated, MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName EditedBy from TransSurvey inner join MobizLibraryPeople on MobizLibraryPeople.ID = TransSurvey.PeopleID "
            pSql = "Select DISTINCT " & vbCrLf &
                    "mkm.ID as ProblemID, mkm.CaseNo, ISNULL(mkm.SubTopic, '') as SubTopicID, mkm.Problem,  " & vbCrLf &
                    "lkm1.Topic, lkm1.ID as TopicID, lkm2.Product, lkm2.ID as ProductID, lkm3.Language, lkm3.ID as LanguageID, mkm.PeopleID, " & vbCrLf &
                    "mkm.Tags, CONVERT(VARCHAR(10),mkm.Lastupdated,110) LastUpdated,  " & vbCrLf &
                    "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName EditedBy,  " & vbCrLf &
                    "(SELECT DISTINCT STUFF((SELECT DISTINCT Top 1 ', '+ km2.Solution " & vbCrLf &
                    "       FROM TransSurvey as km2 " & vbCrLf &
                    "       WHERE km2.ProblemID = mkm.ID " & vbCrLf &
                    "   FOR XML PATH('')" & vbCrLf &
                    "), 1, 1,'') FROM TransSurvey) as Solution " & vbCrLf &
                    "from TransSurvey as mkm " & vbCrLf &
                    "left join MobizLibraryPeople on MobizLibraryPeople.ID = mkm.PeopleID  " & vbCrLf &
                    "inner join LibrarySurvey lkm1 on mkm.Topic = lkm1.ID AND lkm1.Topic IS NOT NULL " & vbCrLf &
                    "left join LibrarySurvey lkm2 on mkm.Product = lkm2.ID AND lkm2.Product IS NOT NULL " & vbCrLf &
                    "left join LibrarySurvey lkm3 on mkm.Language = lkm3.ID AND lkm3.Language IS NOT NULL "

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

            pSql = "SELECT ID as SolutionID, Solution, IsPublic FROM TransSurvey " & vbCrLf &
                    "WHERE ProblemID = " & ReadyString(Common.WebRequest.Data("ProblemID")) & vbCrLf

            Try
                pDtSolution = Common.WebRequest.ProData.Execute(pSql)

                If pDtSolution.Rows.Count > 0 Then
                    Dim solObj(pDtSolution.Rows.Count - 1) As Object

                    For i As Integer = 0 To pDtSolution.Rows.Count - 1
                        pSql = "SELECT ID as StepID, Steps as Step FROM TransSurvey " & vbCrLf &
                                "WHERE SolutionID = " & ReadyString(pDtSolution.Rows(i)("SolutionID").ToString()) & vbCrLf &
                                " AND Steps <> '' ORDER BY OrderNo ASC"
                        pDtStep = Common.WebRequest.ProData.Execute(pSql)

                        Dim stepObj(pDtStep.Rows.Count - 1) As Object

                        If pDtStep.Rows.Count > 0 Then
                            For m As Integer = 0 To pDtStep.Rows.Count - 1
                                stepObj(m) = New With {Key .StepID = pDtStep.Rows(m)("StepID").ToString(), .No = i + 1, .Step = pDtStep.Rows(m)("Step").ToString(), .State = "Edit"}
                            Next
                        End If

                        pSql = "SELECT TransSurvey.ID, LibrarySurvey.TeamName, LibrarySurvey.ID as TeamID FROM TransSurvey " & vbCrLf &
                                " INNER JOIN LibrarySurvey " & vbCrLf &
                                "   ON LibrarySurvey.ID = TransSurvey.TeamID " & vbCrLf &
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

            pSql = "Select ISNULL(MAX(CaseNo),0) + 1  LastNumber from TransSurvey "

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

            Dim p As New InsertUpdate("TransSurvey")
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

                If CStr2(Common.WebRequest.AllData("Solution")) = "System.Object[]" Then
                    Dim solutionOrderNo As Integer = 1

                    For Each solutions As Object In Common.WebRequest.AllData("Solution")
                        Dim solID As String
                        Dim stepOrderNo As Integer = 1
                        p = New InsertUpdate("TransSurvey")
                        p("Solution") = ReadyString(CStr2(solutions("Solution")))
                        p("ProblemID") = problemID
                        p("IsPublic") = ReadyString(CBool(solutions("IsPublic")))
                        p("OrderNo") = solutionOrderNo

                        If CStr2(solutions("State")) = "Add" Then
                            solID = ReadyString(Guid.NewGuid.ToString())
                            p("ID") = solID
                            pSql = p.GetInsert()
                        ElseIf CStr2(solutions("State")) = "Edit" Then
                            solID = ReadyString(solutions("SolutionID"))
                            pSql = p.GetUpdate(" ID = " & solID)
                        Else
                            pSql = "DELETE FROM TransSurvey WHERE ID = " & ReadyString(solutions("SolutionID"))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        If CStr2(solutions("State")) = "Remove" Then
                            pSql = "DELETE FROM TransSurvey WHERE SolutionID = " & ReadyString(solutions("SolutionID"))
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Else
                            For Each steps As Object In solutions("Step")
                                Dim stepID As String
                                p = New InsertUpdate("TransSurvey")
                                p("SolutionID") = solID
                                p("Steps") = ReadyString(CStr2(steps("Step")))
                                p("OrderNo") = ReadyString(CInt(steps("No")))

                                If CStr2(steps("State")) = "Add" Then
                                    stepID = ReadyString(Guid.NewGuid.ToString())
                                    p("ID") = stepID
                                    pSql = p.GetInsert()
                                ElseIf CStr2(steps("State")) = "Edit" Then
                                    stepID = ReadyString(steps("StepID"))
                                    pSql = p.GetUpdate(" ID = " & stepID)
                                Else
                                    pSql = "DELETE FROM TransSurvey WHERE ID = " & ReadyString(steps("StepID"))
                                End If

                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                stepOrderNo += 1
                            Next steps

                            If CStr2(solutions("IsPublic")) = "True" Then
                                If solutions("SolutionID") <> Nothing Then
                                    pSql = "DELETE FROM TransSurvey WHERE SolutionID = " & ReadyString(solutions("SolutionID")) & " AND CONVERT(varchar(MAX), TeamID) <> ''"
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If
                            Else
                                For Each team As Object In solutions("AllowedTeam")
                                    Dim stepID As String
                                    p = New InsertUpdate("TransSurvey")
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
                                        pSql = "DELETE FROM TransSurvey WHERE ID = " & ReadyString(team("ID"))
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
                Return ex.Message
            End Try

            'If (CStr2(Common.WebRequest.Data("Steps")).Length > 0) Then
            '    Solution = CStr2(Common.WebRequest.Data("Steps")).Split(New String() {vbLf, vbCr, vbCrLf},
            '                           StringSplitOptions.None)
            'End If

            'Try

            '    If Not Solution Is Nothing Then
            '        For i As Integer = 0 To UBound(Solution)
            '            pSql = "Insert into TransSurvey(CaseNo, CaseDescription, Solution, Steps, Topic, Product, Language, PeopleID, Tags) values(" & vbCrLf &
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
            '        pSql = "Insert into TransSurvey(CaseNo, CaseDescription, Solution, Steps, Topic, Product, Language, PeopleID, Tags) values(" & vbCrLf &
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

            pSql = "Select ID, Topic from LibrarySurvey where Topic IS NOT NULL"

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

            pSql = "Select ID, SubTopic from LibrarySurvey where SubTopic IS NOT NULL AND TopicID = " & ReadyString(CStr2(Common.WebRequest.Data("TopicID")))

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

            pSql = "Select ID, Product from LibrarySurvey where Product IS NOT NULL"

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

            pSql = "Select ID, Language from LibrarySurvey where Language IS NOT NULL"

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

                pSql = "Insert into LibrarySurvey(ID, Topic) values(" & vbCrLf &
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

                pSql = "Insert into LibrarySurvey(ID, SubTopic, TopicID) values(" & vbCrLf &
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

                pSql = "Insert into LibrarySurvey(ID, Product) values(" & vbCrLf &
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

                pSql = "Insert into LibrarySurvey(ID, Language) values(" & vbCrLf &
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

                pSql = "Insert into LibrarySurvey(ID, TeamName, LastUpdated) values(" & vbCrLf &
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