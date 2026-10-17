Public Class AlertsController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function TagReminders()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim ids As String = Common.WebRequest.Data("RemindersID")
            Dim p As New InsertUpdate("LibraryComplianceReminders")

            p("MarkAsUnread") = 0
            p("SeenOn") = "getdate()"

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID IN (" & Trim(ids) & ")"))

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function GetDashboardReminders()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String
            Dim p As New JsonDictionary

            pSql = " SELECT ComplianceOrg, COUNT(ComplianceOrg) as Cnt " & vbCrLf &
                "   ,(SELECT DISTINCT STUFF((SELECT ', ''' + CONVERT(varchar(max), ID) + '''' " & vbCrLf &
                "       FROM LibraryComplianceReminders " & vbCrLf &
                "       WHERE (Frequency = 'Annually' AND CAST((DueDate + ' ' + CONVERT(varchar, DatePart(YEAR, GETDATE()))) as Date) <= CONVERT(date, GETDATE()) AND MarkAsUnread = 1) " & vbCrLf &
                "       FOR XML PATH('')), 1, 1,'')FROM LibraryComplianceReminders) as IDs " & vbCrLf &
                "   FROM LibraryComplianceReminders " & vbCrLf &
                "   WHERE (Frequency = 'Annually' AND CAST((DueDate + ' ' + CONVERT(varchar, DatePart(YEAR, GETDATE()))) as Date) <= CONVERT(date, GETDATE()) AND MarkAsUnread = 1) " & vbCrLf &
                "   GROUP BY ComplianceOrg " & vbCrLf &
                " UNION ALL " & vbCrLf &
                " SELECT ComplianceOrg, COUNT(ComplianceOrg) as Cnt " & vbCrLf &
                "   ,(SELECT DISTINCT STUFF((SELECT ', ''' + CONVERT(varchar(max), ID) + '''' " & vbCrLf &
                "       FROM LibraryComplianceReminders " & vbCrLf &
                "       WHERE (Frequency = 'Monthly' AND (CAST(convert(char(7), CONVERT(date, GETDATE()), 23) + '-' + DueDate as Date) <= CONVERT(date, GETDATE()) AND MarkAsUnread = 1)) " & vbCrLf &
                "       FOR XML PATH('')), 1, 1,'')FROM LibraryComplianceReminders) as IDs " & vbCrLf &
                "   FROM LibraryComplianceReminders " & vbCrLf &
                "   WHERE (Frequency = 'Monthly' AND (CAST(convert(char(7), CONVERT(date, GETDATE()), 23) + '-' + DueDate as Date) <= CONVERT(date, GETDATE()) AND MarkAsUnread = 1)) " & vbCrLf &
                "   GROUP BY ComplianceOrg " & vbCrLf &
                " UNION ALL " & vbCrLf &
                " SELECT ComplianceOrg, COUNT(ComplianceOrg) as Cnt " & vbCrLf &
                "   ,(SELECT DISTINCT STUFF((SELECT ', ''' + CONVERT(varchar(max), ID) + '''' " & vbCrLf &
                "       FROM LibraryComplianceReminders " & vbCrLf &
                "       WHERE (Frequency = 'Weekly' AND (CAST(DueDate as int) = DATEPART(w, GETDATE()) AND CONVERT(date, GETDATE()) > DateCreated AND MarkAsUnread = 1)) " & vbCrLf &
                "       FOR XML PATH('')), 1, 1,'')FROM LibraryComplianceReminders) as IDs " & vbCrLf &
                "   FROM LibraryComplianceReminders " & vbCrLf &
                "   WHERE (Frequency = 'Weekly' AND (CAST(DueDate as int) = DATEPART(w, GETDATE()) AND CONVERT(date, GETDATE()) > DateCreated AND MarkAsUnread = 1)) " & vbCrLf &
                "   GROUP BY ComplianceOrg " & vbCrLf

            p.Add("Compliance", Common.WebRequest.ProData.Execute(pSql))

            pSql = " SELECT COUNT(ID) as Cnt " & vbCrLf &
                    " FROM TransCRM " & vbCrLf &
                    " WHERE ActionName = 'To-Do' AND IsDelete = 0 AND ToDoFinish = 0 " & vbCrLf &
                    " AND CONVERT(date, ToDoDueDate) <= CONVERT(date, GETDATE()) " & vbCrLf

            p.Add("ToDo", Common.WebRequest.ProData.Execute(pSql))

            Return p.Serialize()
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function NoPrice() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function NoPriceData() As String

        Dim pDT As DataTable
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pSql = "SELECT TOP 50 TransPIMAlerts.ProductID" & vbCrLf & _
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + " & vbCrLf & _
            "  ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') +" & vbCrLf & _
            "  ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '') AS EmployeeName" & vbCrLf & _
            " ,TransPIMAlerts.LastUpdated" & vbCrLf & _
            " FROM TransPIMAlerts" & vbCrLf & _
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransPIMAlerts.ProductID" & vbCrLf & _
            " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransPIMAlerts.UserID" & vbCrLf & _
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf & _
            " ORDER BY TransPIMAlerts.LastUpdated" & vbCrLf
        pDT = WebRequest.Instance.ProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return JsonDictionary.Serialize("Warning", "No record")
        Else
            Return JsonDictionary.Serialize("data", pDT)
        End If

    End Function

    <HttpPost()>
    Public Function NoPriceDelete() As String

        Dim pIDs As String = ""
        Dim pSql As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        For Each pID As String In Common.WebRequest.AllData("ProductIDs")
            If pIDs.Length > 0 Then pIDs &= ", "
            pIDs &= ReadyString(pID)
        Next pID
        If pIDs.Length = 0 Then
            Return JsonDictionary.Serialize("Error", "Nothing to delete")
        End If

        pSql = "DELETE FROM TransPIMAlerts" &
            " WHERE ProductID IN (" & pIDs & ")"
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    Function GetComplianceReminders()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String
            Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
            Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
            Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
            Dim ids As String = CStr2(Trim(Request.Params.Get("ids")))
            Dim whereSearch As String = ""
            Dim whereSearchContent As String = ""
            Dim operators As String = ""
            Dim arr As Object = Split(keyword, "+")

            Dim mainquery As String = " SELECT cr.*, ROW_NUMBER() OVER (ORDER BY cr.LastUpdated ASC) AS RowNum, " & vbCrLf &
                    " users.FirstName + ' ' + users.Surname as UpdatedByName " & vbCrLf &
                    " FROM LibraryComplianceReminders as cr " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as users ON users.ID = cr.UpdatedBy " & vbCrLf

            If Not String.IsNullOrWhiteSpace(ids) Then
                whereSearch = " WHERE cr.ID IN (" & ids & ") "
            End If

            If CStr2(keyword) <> "" Then
                If Not String.IsNullOrWhiteSpace(ids) Then
                    whereSearch += " AND ("
                Else
                    whereSearch += " WHERE ("
                End If

                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " AND "
                    End If

                    whereSearchContent = whereSearchContent & operators & vbCrLf &
                         " ISNULL(cr.ComplianceOrg, '') + ' ' + " & vbCrLf &
                         " ISNULL(cr.OtherComplianceOrg, '') + ' ' + " & vbCrLf &
                         " ISNULL(cr.TaxType, '') + ' ' + " & vbCrLf &
                         " ISNULL(cr.FormType, '') + ' ' + " & vbCrLf &
                         " ISNULL(cr.Description, '') + ' ' + " & vbCrLf &
                         " ISNULL(cr.ComplianceTaskTile, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next

                whereSearch += whereSearchContent & ") "
            End If

            pSql = "SELECT * FROM (" & vbCrLf &
                            mainquery & whereSearch & vbCrLf &
                            " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                            " ORDER BY LastUpdated ASC"

            Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery & whereSearch)

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
            'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function

    Function SaveComplianceReminders()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            Dim data As Object = Common.WebRequest.AllData("data")
            Dim p As New InsertUpdate("LibraryComplianceReminders")

            p("ComplianceOrg") = ReadyString(data("ComplianceOrg"))
            p("OtherComplianceOrg") = IIf(String.IsNullOrWhiteSpace(data("OtherComplianceOrg")), "NULL", ReadyString(data("OtherComplianceOrg")))
            p("TaxType") = IIf(data("ComplianceOrg") = "BIR", ReadyString(data("TaxType")), "NULL")
            p("FormType") = IIf(data("ComplianceOrg") = "BIR", ReadyString(data("FormType")), "NULL")
            p("FilingStartDate") = IIf(data("ComplianceOrg") = "BIR", ReadyString(data("FilingStartDate")), "NULL")
            p("Frequency") = IIf(data("ComplianceOrg") = "BIR", ReadyString(data("FilingFrequency")), ReadyString(data("ComplianceFrequency")))
            p("Description") = IIf(data("ComplianceOrg") = "BIR", ReadyString(data("FilingDueDateDescription")), ReadyString(data("ComplianceDueDateDescription")))
            p("DueDate") = IIf(data("ComplianceOrg") = "BIR", ReadyString(data("FilingDueDate")), ReadyString(data("ComplianceDueDate")))
            p("ComplianceTaskTile") = IIf(data("ComplianceOrg") = "BIR", "NULL", ReadyString(data("ComplianceTaskTile")))
            p("ComplianceDescription") = IIf(data("ComplianceOrg") = "BIR", "NULL", ReadyString(data("ComplianceDescription")))
            p("LastUpdated") = "getdate()"
            p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)

            If String.IsNullOrWhiteSpace(data("ID")) Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("MarkAsUnread") = "1"
                p("DateCreated") = "getdate()"
                Common.WebRequest.ProData.Execute(p.GetInsert())
            Else
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))
            End If

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function
End Class
