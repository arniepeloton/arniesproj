Imports Aspose
Imports Aspose.Cells
Imports Newtonsoft.Json

Namespace WebProj
    Public Class ExpensesController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function ExpenseTools() As ActionResult
            Return View()
        End Function

        Function GetEmployeeRouteSchedules()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pOut As New JsonDictionary
                Dim employeeID As String = Common.WebRequest.Data("EmployeeID")
                Dim pSql As String = "SELECT route.*, ISNULL(location.CompanyName, location.PersonFirstName + ' ' + location.PersonSurname) as LocationName, fowst.Name as BranchName" & vbCrLf &
                                    " FROM LibraryExpensesRouteSchedules as route " & vbCrLf &
                                    " INNER JOIN MobizLibraryOrganization as location ON location.ID = route.LocationID " & vbCrLf &
                                    " INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = route.FowstID " & vbCrLf &
                                    " WHERE route.EmployeeID = " & ReadyString(employeeID) & vbCrLf &
                                    " ORDER BY Day, Week, StartTime ASC "

                pOut.Add("Schedules", Common.WebRequest.ProData.Execute(pSql))

                pSql = "SELECT rsemp.*, emp.FirstName + ' ' + emp.Surname as EmployeeName FROM LibraryExpensesRouteSchedulesEmployees as rsemp " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as emp ON emp.ID = rsemp.EmployeeID " & vbCrLf &
                        " WHERE EmployeeID = " & ReadyString(employeeID)

                pOut.Add("Info", Common.WebRequest.ProData.Execute(pSql))

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetRouteSchedules()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim whereSearchContent As String = ""
                Dim operators As String = ""
                Dim arr As Object = Split(keyword, "+")

                Dim mainquery As String = " SELECT route.EmployeeID, " & vbCrLf &
                        " ppol.Surname + ', ' + ppol.FirstName as EmployeeName, " & vbCrLf &
                        " modified.FirstName + ' ' + modified.Surname as LastUpdatedBy, " & vbCrLf &
                        " STUFF((SELECT ', ' + ISNULL(DATEName(WEEKDAY, CONVERT(int, Day)), 'Any Day') " & vbCrLf &
                        "       FROM LibraryExpensesRouteSchedules " & vbCrLf &
                        "       WHERE EmployeeID = route.EmployeeID " & vbCrLf &
                        "       GROUP BY Day ORDER BY Day " & vbCrLf &
                        "   FOR XML PATH('') " & vbCrLf &
                        " ), 1, 1,'') as Day, " & vbCrLf &
                        " (SELECT TOP 1 LastUpdated FROM LibraryExpensesRouteSchedules WHERE EmployeeID = route.EmployeeID ORDER BY LastUpdated DESC) as LastUpdated, " & vbCrLf &
                        " ROW_NUMBER() OVER (ORDER BY ppol.Surname ASC) AS RowNum," & vbCrLf &
                        " rsemp.ID as ParentID, " & vbCrLf &
                        " rsemp.StrictlyFollowSched, rsemp.Frequency " & vbCrLf &
                        " FROM LibraryExpensesRouteSchedules as route " & vbCrLf &
                        " LEFT JOIN LibraryExpensesRouteSchedulesEmployees as rsemp ON rsemp.EmployeeID = route.EmployeeID " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = route.EmployeeID " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as modified ON modified.ID = route.LastUpdatedBy " & vbCrLf

                If CStr2(keyword) <> "" Then
                    mainquery += " WHERE ("
                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " ISNULL(ppol.FirstName, '') + ' ' + " & vbCrLf &
                             " ISNULL(ppol.Surname, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next

                    mainquery += whereSearchContent & ") "
                End If

                mainquery += " GROUP BY rsemp.ID, rsemp.StrictlyFollowSched, rsemp.Frequency, route.EmployeeID, ppol.FirstName, ppol.Surname, modified.FirstName, modified.Surname " & vbCrLf

                pSql = "SELECT * FROM (" & vbCrLf &
                                mainquery & vbCrLf &
                                " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                                " ORDER BY RowNum ASC "

                Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
                'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveRouteSchedules()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")

                Common.WebRequest.ProData.BeginTransaction()

                Dim p As New InsertUpdate("LibraryExpensesRouteSchedulesEmployees")
                Dim pSql As String
                Dim parentID As String
                p("EmployeeID") = ReadyString(data("EmployeeID"))
                p("RouteName") = ReadyString(data("RouteName"))
                p("RouteDescription") = IIf(String.IsNullOrWhiteSpace(data("RouteDescription")), "NULL", ReadyString(data("RouteDescription")))
                p("StrictlyFollowSched") = IIf(Not data.ContainsKey("StrictlyFollowSched"), 0, CInt2(data("StrictlyFollowSched")))
                p("Frequency") = ReadyString(data("Frequency"))
                p("LastUpdatedBy") = ReadyString(data("ModifiedBy"))
                p("LastUpdated") = "getdate()"

                If String.IsNullOrWhiteSpace(data("ParentID")) Then
                    parentID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(parentID)
                    pSql = p.GetInsert()
                Else
                    parentID = data("ParentID")
                    pSql = p.GetUpdate(" ID = " & ReadyString(parentID))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                For Each sched As Object In data("Schedules")
                    p = New InsertUpdate("LibraryExpensesRouteSchedules")

                    p("ParentID") = ReadyString(parentID)
                    p("EmployeeID") = ReadyString(sched("EmployeeID"))
                    p("LocationID") = ReadyString(sched("LocationID")) 'Customer or Supplier ID
                    p("FowstID") = ReadyString(sched("FowstID"))

                    If sched.ContainsKey("Day") Then
                        p("Day") = IIf(sched("Day") = "any", "NULL", ReadyString(sched("Day")))
                    End If

                    If sched.ContainsKey("StartTime") Then
                        p("StartTime") = IIf(String.IsNullOrWhiteSpace(sched("StartTime")), "NULL", ReadyString(sched("StartTime")))
                    End If

                    If sched.ContainsKey("EndTime") Then
                        p("EndTime") = IIf(String.IsNullOrWhiteSpace(sched("EndTime")), "NULL", ReadyString(sched("EndTime")))
                    End If

                    p("LastUpdatedBy") = ReadyString(sched("ModifiedBy"))
                    p("LastUpdated") = "getdate()"

                    If sched.ContainsKey("Frequency") Then
                        p("Frequency") = ReadyString(sched("Frequency"))
                    End If

                    If sched.ContainsKey("Week") Then
                        p("Week") = IIf(String.IsNullOrWhiteSpace(sched("Week")), "NULL", ReadyString(sched("Week")))
                    End If

                    If sched.ContainsKey("Tag") Then
                        p("Tag") = IIf(CStr2(sched("Tag")) = "", "NULL", ReadyString(sched("Tag")))
                    End If

                    If CBool2(sched("IsDeleted")) Then
                        pSql = "DELETE FROM LibraryExpensesRouteSchedules WHERE ID = " & ReadyString(sched("ID"))
                    Else
                        If String.IsNullOrWhiteSpace(sched("ID")) Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                        Else
                            pSql = p.GetUpdate(" ID = " & ReadyString(sched("ID")))
                        End If
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Next

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryExpensesRouteSchedules")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetOptionalSettings()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim pOut As New JsonDictionary
            Dim pDT As DataTable

            pSql = "SELECT DefaultValue FROM SystemDefaults WHERE Description = 'LogExpenseRemarksField'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("LogExpenseRemarksField", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pOut.Add("AllowCustomLocationName", WebRequest.Instance.Business.SystemDefaults.AllowCustomLocationName)

            Return pOut.Serialize()
        End Function

        Function SaveOptionalSettings()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim data As Object = Common.WebRequest.AllData("data")
            Dim p As New InsertUpdate("SystemDefaults")

            p("Description") = ReadyString("LogExpenseRemarksField")
            p("DefaultValue") = ReadyString(data("LogExpenseRemarksField"))
            p("LastUpdated") = "getdate()"

            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate("Description = " & ReadyString("LogExpenseRemarksField"))) = 0 Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
                Common.WebRequest.ProData.Execute(pSql)
            End If

            WebRequest.Instance.Business.SystemDefaults.AllowCustomLocationName = data("AllowCustomLocationName")

            WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("SystemDefaults")

            Return JsonDictionary.Serialize("Results", "saved")
        End Function

        Function CheckTicketUserIfHasAccess()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT ID FROM LibraryExpensesTicketUsers WHERE UserID = " & ReadyString(Common.WebRequest.Data("UserID")) & vbCrLf &
                                        " AND Type = " & ReadyString(Common.WebRequest.Data("type"))
                Dim hasAccess As Boolean = False

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                    hasAccess = True
                End If

                Return JsonDictionary.Serialize("Results", hasAccess)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function RemoveExpenseTicket()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "DELETE FROM LibraryExpensesTicketUsers WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "deleted")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetExpenseTicket()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.QueryString("type")
                Dim mainSql As String = "SELECT maker.*, makerppol.FirstName + ' ' + makerppol.Surname as MakerName, ppol.FirstName + ' ' + ppol.Surname as AddedByName " & vbCrLf &
                                    " FROM LibraryExpensesTicketUsers as maker " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = maker.AddedBy " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as makerppol ON makerppol.ID = maker.UserID "
                Dim PageStart = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
                Dim PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
                Dim pSql As String
                Dim pDT As DataTable
                Dim clsOrganization As New Organization
                Dim whereSearch As String = " WHERE Type = " & ReadyString(Common.WebRequest.QueryString("type"))
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.MakerName " & PageColumnDirection
                ElseIf PageColumnNo = 2 Then
                    whereOrder = " ORDER BY CONVERT(datetime, mainsql.DateAdded) " & PageColumnDirection
                ElseIf PageColumnNo = 3 Then
                    whereOrder = " ORDER BY CONVERT(datetime, mainsql.AddedByName) " & PageColumnDirection
                End If

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        mainSql & vbCrLf &
                        " " & whereSearch & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim count = Common.WebRequest.ProData.Execute(mainSql).Rows.Count

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsOrganization.GetJson(pDT)), .recordsTotal = count, .recordsFiltered = count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveExpenseTicket()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For Each maker As Object In Common.WebRequest.AllData("data")
                    Dim p As New InsertUpdate("LibraryExpensesTicketUsers")

                    p("UserID") = ReadyString(maker("ID").ToString())

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" UserID = " & ReadyString(maker("ID")) & " AND Type = " & ReadyString(Common.WebRequest.Data("Type"))), True) = 0 Then
                        p("DateAdded") = "getdate()"
                        p("AddedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("Type") = ReadyString(Common.WebRequest.Data("Type"))

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next maker

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function CountUnfinishedExpenseReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT COUNT(ID) as Count FROM TransExpensesLogDetails WHERE ISNULL(Amount, '') = '' AND IsUnfinishedExpense = 1 "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function CountUnapprovedExpenseTicket()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT COUNT(ID) as Count FROM TransExpensesLogDetails WHERE ISNULL(IsApproved, 1) = 0 AND ISNULL(IsRejected, 0) = 0 "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetLatestData()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim month As Integer = CInt2(Format(Date.Now, "MM"))
                Dim year As Integer = CInt2(Format(Date.Now, "yyyy"))
                Dim carryovermonth As Integer = month
                Dim carryoveryear As Integer = year

                If carryovermonth = 1 Then
                    carryovermonth = 12
                    carryoveryear = carryoveryear - 1
                Else
                    carryovermonth = carryovermonth - 1
                End If

                pSql = " SELECT * FROM TransExpensesLogDetails " & vbCrLf &
                        " WHERE PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " And MONTH(ISNULL(ExpenseDate, DateCreated)) = " & month & vbCrLf &
                        " AND YEAR(ISNULL(ExpenseDate, DateCreated)) = " & year & vbCrLf &
                        " ORDER BY ISNULL(ExpenseDate, DateCreated) ASC "

                pOut.Add("Expenses", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT * FROM TransExpensesCheckInOutDetails " & vbCrLf &
                        " WHERE PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " And MONTH(CheckIn) = " & month & vbCrLf &
                        " AND YEAR(CheckIn) = " & year & vbCrLf &
                        " ORDER BY CheckIn ASC "

                pOut.Add("CheckInOut", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT * FROM TransExpensesReceiveFunds " & vbCrLf &
                        " WHERE PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " And MONTH(ReceiveDate) = " & month & vbCrLf &
                        " AND YEAR(ReceiveDate) = " & year & vbCrLf &
                        " ORDER BY ReceiveDate ASC "

                pOut.Add("ReceiveFunds", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT * FROM TransNetFundBalance " & vbCrLf &
                        " WHERE PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " And (MonthCovered = " & month & vbCrLf &
                        " Or MonthCovered = " & carryovermonth & vbCrLf &
                        " ) AND (YearCovered = " & year & vbCrLf &
                        " Or YearCovered = " & carryoveryear & vbCrLf &
                        " ) "

                pOut.Add("NetFundBalances", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT * FROM TransLogExpensesCache " & vbCrLf &
                        " WHERE PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " And MonthCovered = " & month & vbCrLf &
                        " AND YearCovered = " & year & vbCrLf

                pOut.Add("ExpensesCache", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT * FROM TransReceiveFundsCache " & vbCrLf &
                        " WHERE PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " And MonthCovered = " & month & vbCrLf &
                        " AND YearCovered = " & year & vbCrLf

                pOut.Add("ReceiveFundsCache", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT * FROM LibraryExpensesRouteSchedules " & vbCrLf &
                        " WHERE EmployeeID = " & ReadyString(data("PeopleID")) & vbCrLf

                pOut.Add("RouteSchedules", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT * FROM LibraryExpensesRouteSchedulesEmployees " & vbCrLf &
                        " WHERE EmployeeID = " & ReadyString(data("PeopleID")) & vbCrLf

                pOut.Add("RouteSchedulesEmployees", Common.WebRequest.ProData.Execute(pSql))

                Return pOut.Serialize()

            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveCarryOverFund()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String
                Dim p As New InsertUpdate("TransNetFundBalance")

                p("OriginalAmount") = data("OriginalFund")
                p("AdjustedAmount") = data("AdjustedAmount")
                p("Remarks") = ReadyString(data("Remarks"))
                p("FowstID") = ReadyString(data("FowstID"))
                p("LastUpdated") = "getdate()"
                p("PeopleID") = ReadyString(data("EmployeeID"))
                p("MonthCovered") = ReadyString(data("Month"))
                p("YearCovered") = ReadyString(data("Year"))
                p("Status") = ReadyString("Pending")
                p("IsAdjustment") = 1
                p("IsCancelRequest") = 0
                p("RequestedDate") = "getdate()"
                p("ID") = ReadyString(Guid.NewGuid.ToString())

                Common.WebRequest.ProData.Execute(p.GetInsert())

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("TransNetFundBalance")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetAdjustedFundHistory()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String = " SELECT net.*, " & vbCrLf &
                        " updatedby.FirstName + ' ' + updatedby.Surname as UpdatedBy, " & vbCrLf &
                        " requestedby.FirstName + ' ' + requestedby.Surname as RequestedBy, " & vbCrLf &
                        " fowst.Name as FowstName " & vbCrLf &
                        " FROM TransNetFundBalance as net " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as requestedby ON requestedby.ID = net.PeopleID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as updatedby ON updatedby.ID = net.RequestUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = net.FowstID " & vbCrLf &
                        " WHERE (MonthCovered = " & data("Month") & vbCrLf &
                        " AND YearCovered = " & data("Year") & ") " & vbCrLf &
                        " AND PeopleID = " & ReadyString(data("EmployeeID")) & vbCrLf &
                        " AND FowstID = " & ReadyString(data("FowstID")) & vbCrLf &
                        " AND IsAdjustment = 1 " & vbCrLf &
                        " ORDER BY RequestedDate DESC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function RecomputeNetFundBalance()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim empid As String = Common.WebRequest.Data("EmployeeID")
                Dim pSql As String
                Dim pRow As Object
                Dim pOut As New JsonDictionary
                Dim adminpassword As String = Common.WebRequest.Data("Password")

                If CStr2(adminpassword) <> "mobizpeopleonly!" Then
                    Return JsonDictionary.Serialize("Error", "Invalid admin password")
                End If

                Common.WebRequest.ProData.BeginTransaction()

                Dim pFowst As Object = New Organization().GetEmployeeFowst().Rows

                If pFowst.Count > 0 Then
                    For Each fowst As Object In pFowst
                        pSql = "SELECT TOP 1 ISNULL(ExpenseDate, DateCreated) as StartMonth FROM TransExpensesLogDetails WHERE PeopleID = " & ReadyString(empid) & vbCrLf &
                                " AND FowstID = " & ReadyString(fowst("ID").ToString())
                        pRow = Common.WebRequest.ProData.Execute(pSql).Rows

                        If pRow.Count > 0 Then
                            Dim startmonth As Date = CDate(pRow(0)("StartMonth")).ToString("MM-1-yyyy")
                            Dim endmonth As Date = CDate(CDate(CDate(DateTime.Now).AddMonths(1)).ToString("MM-1-yyyy"))

                            Do While startmonth <> endmonth
                                Dim month As Integer = CInt2(CDate(startmonth).ToString("MM"))
                                Dim year As Integer = CInt2(CDate(startmonth).ToString("yyyy"))
                                Dim carryovermonth As Integer = month
                                Dim carryoveryear As Integer = year

                                pSql = " SELECT ISNULL(SUM(Amount), 0) as TotalAmountReceived FROM TransExpensesReceiveFunds " & vbCrLf &
                                        " WHERE MONTH(ReceiveDate) = " & month & " AND YEAR(ReceiveDate) = " & year & vbCrLf &
                                        " AND PeopleID = " & ReadyString(empid) & vbCrLf &
                                        " AND ISNULL(IsDelete, 'False') = 'False' "

                                pRow = Common.WebRequest.ProData.Execute(pSql).Rows

                                Dim totalreceivedfund As Decimal = 0

                                If pRow.Count > 0 Then
                                    totalreceivedfund = CDec2(pRow(0)("TotalAmountReceived"))
                                End If

                                If carryovermonth = 1 Then
                                    carryovermonth = 12
                                    carryoveryear = carryoveryear - 1
                                Else
                                    carryovermonth = carryovermonth - 1
                                End If

                                pSql = " SELECT TOP 1 * " & vbCrLf &
                                        " FROM TransNetFundBalance " & vbCrLf &
                                        " WHERE (MonthCovered = " & carryovermonth & vbCrLf &
                                        " AND YearCovered = " & carryoveryear & ") " & vbCrLf &
                                        " AND PeopleID = " & ReadyString(empid) & vbCrLf &
                                        " AND (ISNULL(Status, '') = 'Approved' OR ISNULL(IsAdjustment, 0) = 0) " & vbCrLf &
                                        " AND ISNULL(IsCancelRequest, 0) = 0 " & vbCrLf &
                                        " ORDER BY IsAdjustment DESC, LastUpdated DESC, Status ASC "
                                pRow = Common.WebRequest.ProData.Execute(pSql).Rows

                                Dim carryoverfund As Decimal = 0

                                If pRow.Count > 0 Then
                                    carryoverfund = CDec2(pRow(0)("NetFundBalance"))
                                End If

                                pSql = " SELECT SUM(Amount) as TotalExpense " & vbCrLf &
                                        " FROM TransExpensesLogDetails " & vbCrLf &
                                        " WHERE MONTH(ISNULL(ExpenseDate, DateCreated)) = " & month & vbCrLf &
                                        " AND YEAR(ISNULL(ExpenseDate, DateCreated)) = " & year & vbCrLf &
                                        " AND PeopleID = " & ReadyString(empid) & vbCrLf &
                                        " AND ISNULL(IsDelete, 'False') = 'False' " & vbCrLf &
                                        " AND FowstID = " & ReadyString(fowst("ID").ToString())
                                pRow = Common.WebRequest.ProData.Execute(pSql).Rows

                                Dim totalexpense As Decimal = 0

                                If pRow.Count > 0 Then
                                    totalexpense = CDec2(pRow(0)("TotalExpense"))
                                End If

                                Dim netfund As Decimal = (totalreceivedfund + carryoverfund) - totalexpense
                                Dim p As New InsertUpdate("TransNetFundBalance")

                                p("PeopleID") = ReadyString(empid)
                                p("NetFundBalance") = netfund
                                p("MonthCovered") = month
                                p("YearCovered") = year
                                p("FowstID") = ReadyString(fowst("ID").ToString())

                                pSql = p.GetUpdate("ISNULL(IsAdjustment, 0) = 0 AND MonthCovered = " & month & " AND YearCovered = " & year & " AND PeopleID = " & ReadyString(empid) & " AND FowstID = " & ReadyString(fowst("ID").ToString()))

                                If Common.WebRequest.ProData.ExecuteSQL(pSql, True) = 0 Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("LastUpdated") = "getdate()"
                                    pSql = p.GetInsert
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                                pSql = " SELECT DISTINCT ExpenseType, SUM(Amount) as Amount " & vbCrLf &
                                        " FROM TransExpensesLogDetails " & vbCrLf &
                                        " WHERE MONTH(ISNULL(ExpenseDate, DateCreated)) = " & month & vbCrLf &
                                        " AND YEAR(ISNULL(ExpenseDate, DateCreated)) = " & year & vbCrLf &
                                        " AND PeopleID = " & ReadyString(empid) & vbCrLf &
                                        " AND ISNULL(IsDelete, 'False') = 'False' " & vbCrLf &
                                        " AND FowstID = " & ReadyString(fowst("ID").ToString()) & vbCrLf &
                                        " AND ExpenseType IN ( SELECT ExpenseType " & vbCrLf &
                                        "   FROM TransExpensesLogDetails " & vbCrLf &
                                        "   WHERE MONTH(ISNULL(ExpenseDate, DateCreated)) = " & month & vbCrLf &
                                        "   AND YEAR(ISNULL(ExpenseDate, DateCreated)) = " & year & vbCrLf &
                                        "   AND PeopleID = " & ReadyString(empid) & vbCrLf &
                                        "   AND ISNULL(IsDelete, 'False') = 'False' " & vbCrLf &
                                        "   AND FowstID = " & ReadyString(fowst("ID").ToString()) & vbCrLf &
                                        "   GROUP BY ExpenseType)" & vbCrLf &
                                        " GROUP BY ExpenseType " & vbCrLf
                                pRow = Common.WebRequest.ProData.Execute(pSql).Rows

                                For Each expensecache As Object In pRow
                                    p = New InsertUpdate("TransLogExpensesCache")

                                    p("Amount") = CDec2(expensecache("Amount"))
                                    pSql = p.GetUpdate(" ExpenseType = " & ReadyString(expensecache("ExpenseType")) & " AND MonthCovered = " & month & " AND YearCovered = " & year & " AND PeopleID =  " & ReadyString(empid) & " AND FowstID = " & ReadyString(fowst("ID").ToString()))

                                    If Common.WebRequest.ProData.ExecuteSQL(pSql, True) = 0 Then
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("MonthCovered") = month
                                        p("YearCovered") = year
                                        p("LastUpdated") = "getdate()"
                                        p("PeopleID") = ReadyString(empid)
                                        p("FowstID") = ReadyString(fowst("ID").ToString())
                                        p("ExpenseType") = ReadyString(expensecache("ExpenseType"))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If
                                Next expensecache

                                pOut.Add(fowst("FowstName") & " (" & month & " " & year & ") = ", netfund)

                                startmonth = CDate(CDate(startmonth).AddMonths(1)).ToString("MM-1-yyyy")
                            Loop

                            Common.WebRequest.ProData.CommitTransaction()
                        Else
                            pOut.Add(fowst("FowstName"), "No expenses found")
                        End If
                    Next
                Else
                    pOut.Add("Fowst:", "Nothing")
                End If

                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function ApplySelectedFowstToExpenses()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim fowstid As String = Common.WebRequest.Data("FowstID")
                Dim employeeid As String = Common.WebRequest.Data("EmployeeID")
                Dim adminpassword As String = Common.WebRequest.Data("Password")

                If CStr2(adminpassword) <> "mobizpeopleonly!" Then
                    Return JsonDictionary.Serialize("Error", "Invalid admin password")
                End If

                Dim p As New InsertUpdate("TransExpensesReceiveFunds")

                Common.WebRequest.ProData.BeginTransaction()

                p("FowstID") = ReadyString(fowstid)
                Common.WebRequest.ProData.Execute(p.GetUpdate(" PeopleID = " & ReadyString(employeeid)))

                p = New InsertUpdate("TransReceiveFundsCache")

                p("FowstID") = ReadyString(fowstid)
                Common.WebRequest.ProData.Execute(p.GetUpdate(" PeopleID = " & ReadyString(employeeid)))

                p = New InsertUpdate("TransNetFundBalance")

                p("FowstID") = ReadyString(fowstid)
                Common.WebRequest.ProData.Execute(p.GetUpdate(" PeopleID = " & ReadyString(employeeid)))

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Result", "Updated")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Result", ex.Message)
            End Try
        End Function

        Function GetLogExpenseHistory()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT ROW_NUMBER() OVER (ORDER BY log.LastUpdated DESC) as RowNum, log.*, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName, " & vbCrLf &
                        " associated.CompanyName + ' ' + associated.PersonFirstName + ' ' + associated.PersonSurname as CompanyName, " & vbCrLf &
                        " fowst.Name as FowstName, cen.Name as CostCenterName " & vbCrLf &
                        " FROM TransExpensesLogDetailsHistory as log " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = log.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = log.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as associated ON associated.ID = log.AssociatedTo " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = log.FowstID " & vbCrLf &
                        " LEFT JOIN LibraryExpensesCenters as cen ON cen.ID = log.CostCenter " & vbCrLf &
                        " WHERE LogExpenseID = " & ReadyString(Common.WebRequest.Data("ExpenseID")) & vbCrLf &
                        " ORDER BY log.LastUpdated DESC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetReceiveFundHistory()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT ROW_NUMBER() OVER (ORDER BY rcv.LastUpdated DESC) as RowNum, rcv.*, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName, " & vbCrLf &
                        " rcvfrom.FirstName + ' ' + rcvfrom.Surname as ReceiveFromName " & vbCrLf &
                        " FROM TransExpensesReceiveFundsHistory as rcv " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = rcv.PeopleID" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = rcv.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = rcv.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as rcvfrom ON rcvfrom.ID = rcv.LastUpdatedBy " & vbCrLf &
                        " WHERE ReceiveFundID = " & ReadyString(Common.WebRequest.Data("ReceiveFundID")) & vbCrLf &
                        " ORDER BY rcv.LastUpdated DESC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetNetFundBalanceOfEmployee()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim clsorgshort As New OrganizationShort
                Dim employeeID As String = Common.WebRequest.Data("EmployeeID")
                Dim month As String = CInt2(Common.WebRequest.Data("Month"))
                Dim year As String = CInt2(Common.WebRequest.Data("Year"))
                Dim carryovermonth = month
                Dim carryoveryear = year
                Dim pSql As String
                Dim pOut As New JsonDictionary

                pSql = " SELECT ROW_NUMBER() OVER (ORDER BY ReceiveDate ASC) as RowNum, rcv.*, rcvfrom.FirstName + ' ' + rcvfrom.Surname as ReceiveFromName " & vbCrLf &
                        " , emp.FirstName + ' ' + emp.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName " & vbCrLf &
                        " FROM TransExpensesReceiveFunds as rcv " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as emp ON emp.ID = rcv.PeopleID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as rcvfrom ON rcvfrom.ID = rcv.ReceiveFrom " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = rcv.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = rcv.EncodedBy " & vbCrLf &
                        " WHERE MONTH(ReceiveDate) = " & month & " AND YEAR(ReceiveDate) = " & year & vbCrLf &
                        " AND ISNULL(IsDelete, 'False') = 'False' " & vbCrLf &
                        " AND PeopleID = " & ReadyString(employeeID)

                pOut.Add("ReceivedFunds", Common.WebRequest.ProData.Execute(pSql))

                If carryovermonth = 1 Then
                    carryovermonth = 12
                    carryoveryear = carryoveryear - 1
                Else
                    carryovermonth = carryovermonth - 1
                End If

                pSql = " SELECT TOP 1 * " & vbCrLf &
                        " FROM TransNetFundBalance " & vbCrLf &
                        " WHERE (MonthCovered = " & carryovermonth & vbCrLf &
                        " AND YearCovered = " & carryoveryear & ") " & vbCrLf &
                        " AND PeopleID = " & ReadyString(employeeID) & vbCrLf &
                        " AND (ISNULL(Status, '') = 'Approved' OR ISNULL(IsAdjustment, 0) = 0) " & vbCrLf &
                        " AND ISNULL(IsCancelRequest, 0) = 0 " & vbCrLf &
                        " ORDER BY IsAdjustment DESC, LastUpdated DESC, Status ASC  "

                pOut.Add("CarryOverFund", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT ROW_NUMBER() OVER (ORDER BY ISNULL(ExpenseDate, DateCreated) ASC) as RowNum, log.*, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = log.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = log.LastUpdatedBy " & vbCrLf &
                        " WHERE MONTH(ISNULL(ExpenseDate, DateCreated)) = " & month & " AND YEAR(ISNULL(ExpenseDate, DateCreated)) = " & year & vbCrLf &
                        " AND ISNULL(IsDelete, 'False') = 'False' " & vbCrLf &
                        " AND PeopleID = " & ReadyString(employeeID) & vbCrLf &
                        " ORDER BY ISNULL(ExpenseDate, DateCreated) ASC "

                pOut.Add("Expenses", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT TOP 1 * " & vbCrLf &
                        " FROM TransNetFundBalance " & vbCrLf &
                        " WHERE (MonthCovered = " & month & vbCrLf &
                        " AND YearCovered = " & year & ") " & vbCrLf &
                        " AND PeopleID = " & ReadyString(employeeID) & vbCrLf &
                        " AND (ISNULL(Status, '') = 'Approved' AND ISNULL(IsAdjustment, 0) = 1) " & vbCrLf &
                        " AND ISNULL(IsCancelRequest, 0) = 0 " & vbCrLf &
                        " ORDER BY LastUpdated DESC "

                pOut.Add("CurrentMonthAdjustedNetFundBalance", Common.WebRequest.ProData.Execute(pSql))

                pSql = " SELECT TOP 1 * " & vbCrLf &
                        " FROM TransNetFundBalance " & vbCrLf &
                        " WHERE (MonthCovered = " & carryovermonth & vbCrLf &
                        " AND YearCovered = " & carryoveryear & ") " & vbCrLf &
                        " AND PeopleID = " & ReadyString(employeeID) & vbCrLf &
                        " AND (ISNULL(Status, '') = 'Approved' AND ISNULL(IsAdjustment, 0) = 1) " & vbCrLf &
                        " AND ISNULL(IsCancelRequest, 0) = 0 " & vbCrLf &
                        " ORDER BY LastUpdated DESC "

                pOut.Add("CarryOverNetFundBalance", Common.WebRequest.ProData.Execute(pSql))

                Return pOut.Serialize()
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        Function GetAllLogExpenses()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim whereSearchContent As String = ""
                Dim operators As String = ""
                Dim arr As Object = Split(keyword, "+")
                Dim showtickets As String = Request.Params.Get("ShowTickets")

                Dim mainquery As String = " SELECT log.*, ROW_NUMBER() OVER (ORDER BY log.LastUpdated DESC) AS RowNum, ISNULL(cen.Name, 'General Expense') as CostCenterName, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " CASE WHEN ISNULL(org.CompanyName, '') = '' THEN org.PersonFirstName + ' ' + org.PersonMiddleName + ' ' + org.PersonSurname ELSE org.CompanyName END as AssociatedToName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName, " & vbCrLf &
                        " approvedorrejectedby.FirstName + ' ' + approvedorrejectedby.Surname as ApprovedOrRejectedByName, " & vbCrLf &
                        " reimburseto.FirstName + ' ' + reimburseto.Surname as ReimburseToEmployeeName, " & vbCrLf &
                        " cancelledby.FirstName + ' ' + cancelledby.Surname as CancelledByName, " & vbCrLf &
                        " CONVERT(varchar, fowst.FowstNO) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, log.ExpenseReferenceNo) as ExpenseTicketNo, " & vbCrLf &
                        " disburse.CompanyName as DisburseToVendorName, " & vbCrLf &
                        " terms.Days as CreditTermsInDays, " & vbCrLf &
                        " (SELECT CASE WHEN COUNT(ID) > 0 THEN 1 ELSE 0 END FROM LibraryOrganizationCustomer WHERE OrganizationID = log.AssociatedTo) as IsAssociatedACustomer, " & vbCrLf &
                        " (SELECT CASE WHEN COUNT(ID) > 0 THEN 1 ELSE 0 END FROM LibraryOrganizationSupplier WHERE SupplierID = log.AssociatedTo) as IsAssociatedASupplier " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = log.AssociatedTo " & vbCrLf &
                        " LEFT JOIN LibraryExpensesCenters as cen ON cen.ID = log.CostCenter " & vbCrLf &
                        " LEFT JOIN LibraryDevice as dev ON dev.ID = log.DeviceID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = log.FowstID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = log.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = log.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as approvedorrejectedby ON approvedorrejectedby.ID = log.ApprovedOrRejectedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as reimburseto ON reimburseto.ID = log.ReimburseToEmployee " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as cancelledby ON cancelledby.ID = log.CancelledBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as disburse ON disburse.ID = log.DisburseToVendor " & vbCrLf &
                        " LEFT JOIN LibraryOrganizationSupplier as vendor ON vendor.SupplierID = log.DisburseToVendor " & vbCrLf &
                        " LEFT JOIN LibraryPaymentTerms as terms ON terms.ID = vendor.CreditTermsID " & vbCrLf &
                        " WHERE ISNULL(log.IsDelete, 'False') = 'False' " & vbCrLf

                If showtickets = "unapproved" Then
                    mainquery &= " AND IsApproved = 0 AND IsRejected = 0 "
                ElseIf showtickets = "approved" Then
                    mainquery &= " AND IsApproved = 1 AND IsRejected = 0 "
                ElseIf showtickets = "rejected" Then
                    mainquery &= " AND IsApproved = 0 AND IsRejected = 1 "
                ElseIf showtickets = "unfinished" Then
                    mainquery &= " AND IsUnfinishedExpense = 1 "
                ElseIf showtickets = "cancelled" Then
                    mainquery &= " AND IsCancelled = 1 "
                End If

                If CStr2(keyword) <> "" Then
                    mainquery += " AND ("
                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " (ISNULL(CONVERT(varchar(max), fowst.FowstNo), '') + '-' + ISNULL(CONVERT(varchar(max), dev.DeviceNo), '') + '-' + ISNULL(CONVERT(varchar, log.ExpenseReferenceNo), '')) + ' ' + " & vbCrLf &
                             " ISNULL(log.ExpenseType, '') + ' ' + " & vbCrLf &
                             " ISNULL(ppol.FirstName, '') + ' ' + " & vbCrLf &
                             " ISNULL(ppol.Surname, '') + ' ' + " & vbCrLf &
                             " SUBSTRING(ISNULL(CONVERT(varchar(max), ExpenseDate), ''), 1, 11) + ' ' + " & vbCrLf &
                             " ISNULL(CONVERT(varchar(max), Amount), '') + ' ' + " & vbCrLf &
                             " ISNULL(ISNULL(org.CompanyName, org.PersonFirstName + ' ' + org.PersonMiddleName + ' ' + org.PersonSurname), '') + ' ' + " & vbCrLf &
                             " ISNULL(log.Remarks, '') + ' ' + " & vbCrLf &
                             " ISNULL(log.Particulars, '') + ' ' + " & vbCrLf &
                             " ISNULL(disburse.CompanyName, '') + ' ' + " & vbCrLf &
                             " ISNULL(cen.Name, 'General Expense') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next

                    mainquery += whereSearchContent & ") "
                End If

                pSql = "SELECT * FROM (" & vbCrLf &
                                mainquery & vbCrLf &
                                " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                                " ORDER BY ExpenseDate DESC"

                Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
                'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Private Class SaveLogExpenseInput

            Public data As New dataClass

            Public Class dataClass
                Public ID As String ' 
                Public StatusExpenseTicket As String ' approved
                Public IsApproved As String ' bool
            End Class
            
        End Class

        Function SaveLogExpense()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim data As Object = Common.WebRequest.AllData("data")
            Dim pOut As New JsonDictionary
            Dim expenseID As String
            Try
                Dim pInput = JsonConvert.DeserializeObject(Of SaveLogExpenseInput)(Common.WebRequest.InputStreamText)
                If String.IsNullOrWhiteSpace(pInput.data.ID) = False _
                    AndAlso IsApproved(pInput.data.ID) AndAlso data("Amount").ToString() = "" Then

                    Return JsonDictionary.Serialize("Results", "This expense ticket was already approved.")
                End If
                Common.WebRequest.ProData.BeginTransaction()

                Dim p As New InsertUpdate("TransExpensesLogDetails")
                Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.GetOrCreateDesktop()
                Dim pDeviceID = CStr2(pDrDevice("ID"))
                Dim pSql As String
                Dim accountpayableerror As String = ""

                p("ExpenseDate") = ReadyString(data("ExpenseDate"))
                p("ExpenseType") = ReadyString(data("ExpenseType"))
                p("FromLocation") = ReadyString(data("FromLocation"))
                p("ToLocation") = ReadyString(data("ToLocation"))
                p("Amount") = IIf(data("Amount").ToString() = "", "NULL", CDec2(data("Amount")))
                p("FowstID") = IIf(data("FowstID") = "", "NULL", ReadyString(data("FowstID")))
                p("CostCenter") = IIf(data("CostCenter") = "", "NULL", ReadyString(data("CostCenter")))
                p("Remarks") = ReadyString(data("Remarks"))
                p("AssociatedTo") = IIf(data("AssociatedTo") = "", "NULL", ReadyString(data("AssociatedTo")))
                p("LastUpdated") = "getdate()"

                If data("Amount").ToString() = "" Then
                    p("IsUnfinishedExpense") = 1
                Else
                    p("IsUnfinishedExpense") = 0
                End If

                If data.ContainsKey("IsApproved") Then
                    p("IsApproved") = data("IsApproved")
                End If

                If data.ContainsKey("IsRejected") Then
                    p("IsRejected") = data("IsRejected")
                End If

                If data.ContainsKey("StatusExpenseTicket") Then
                    If data("StatusExpenseTicket") = "approved" Then
                        p("IsApproved") = 1
                    Else
                        p("IsRejected") = 1
                    End If

                    If CBool2(data("IsFromUnfinishedExpense")) = False Then ' if the value of IsFromUnfinishedExpense is true, it means the AMOUNT just encoded today, no need to change the original approver
                        p("ApprovedOrRejectedOn") = "getdate()"
                        p("ApprovedOrRejectedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                End If

                If data.ContainsKey("Particulars") Then
                    p("Particulars") = ReadyString(data("Particulars"))
                End If

                If data.ContainsKey("PaymentMethod") Then
                    p("PaymentMethod") = ReadyString(data("PaymentMethod"))
                    p("ReimburseToEmployee") = IIf(data("ReimburseToEmployee") = "", "NULL", ReadyString(data("ReimburseToEmployee")))
                    p("DisburseToVendor") = IIf(data("DisburseToVendor") = "", "NULL", ReadyString(data("DisburseToVendor")))
                End If

                If data.ContainsKey("RequestedBy") Then
                    p("RequestedBy") = ReadyString(data("RequestedBy"))
                End If

                If CStr2(data("ID")) = "" Then
                    expenseID = Guid.NewGuid.ToString()
                    p("DateCreated") = "getdate()"
                    p("ID") = ReadyString(expenseID)
                    p("PeopleID") = IIf(data("PeopleID") = "", "NULL", ReadyString(data("PeopleID")))
                    p("EncodedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("IsCreatedInWebApp") = 1
                    p("DeviceID") = ReadyString(pDeviceID)
                    p("ExpenseReferenceNo") = GetExpenseTicketNo(pDeviceID, data("FowstID"))
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    expenseID = data("ID")

                    'GET ORIGINAL DATA
                    pSql = "SELECT * FROM TransExpensesLogDetails WHERE ID = " & ReadyString(data("ID"))
                    Dim origdata As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                    'SAVE ORIGINAL DATA TO HISTORY TABLE FOR FUTURE AUDITABILITY - July 28, 2020 by Arnie
                    SaveLogExpenseHistory(origdata, Common.WebRequest.ProData)

                    p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))
                End If

                If data("PeopleID") <> "" Then
                    RecacheLogExpense(data, Common.WebRequest.ProData)

                    Dim datecovered As DateTime = Convert.ToDateTime(data("ExpenseDate"))
                    UpdateNetFundBalance(datecovered.Month & " " & datecovered.Year & " " & data("PeopleID"), Common.WebRequest.ProData, data("FowstID"))
                End If

                p = New InsertUpdate("MobizLibraryOrganization")
                'Create DUMMY Customer ORGANIZATION for Expense Ticket when Payment Method is for REIMBURSEMENT
                p("CompanyName") = ReadyString("Employee Expense Reimbursement")

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = '2edb5011-a759-4140-8c81-788bfc27be5a' ")) = 0 Then
                    p("ID") = ReadyString("2edb5011-a759-4140-8c81-788bfc27be5a")
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryOrganizationFOWST")

                p("Name") = ReadyString("Employee Expense Reimbursement Address")

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = '59d37322-f641-453d-a85d-6708057f72dd' ")) = 0 Then
                    p("ID") = ReadyString("59d37322-f641-453d-a85d-6708057f72dd")
                    p("OrganizationID") = ReadyString("2edb5011-a759-4140-8c81-788bfc27be5a")
                    p("StreetNumber") = "''"
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If


                If CBool2(pInput.data.IsApproved) _
                    OrElse pInput.data.StatusExpenseTicket = "approved" Then

                    Dim res = SaveExpenseToAccountPayable(expenseID, accountpayableerror)
                    If IsGuid(res) Then
                        p = New InsertUpdate("TransExpensesLogDetails")

                        p("InvoiceID") = ReadyString(res)

                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(expenseID)))
                    End If
                End If

                If accountpayableerror <> "" Then
                    pOut.Add("Results", accountpayableerror)
                    Return pOut.Serialize()
                End If

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("TransExpensesLogDetails")

                Common.WebRequest.ProData.CommitTransaction()

                pOut.Add("Results", "saved")
                pOut.Add("ID", expenseID)

            Catch ex As Exception
                pOut.Add("Results", ex.Message)
            End Try

            Return pOut.Serialize()
        End Function

        Private Function IsApproved(vID As String) As Boolean

            Dim pSql = "SELECT TransExpensesLogDetails.IsApproved" & vbCrLf &
                " FROM TransExpensesLogDetails" & vbCrLf &
                " WHERE TransExpensesLogDetails.ID = " & ReadyString(vID)
            Dim pOut = CBool2(WebRequest.Instance.ProData.ExecuteScalar(pSql))

            Return pout

        End Function

        Function SaveExpenseToAccountPayable(ByVal expenseID As String, ByRef oError As String)
            Dim pSql = "SELECT log.*, employee.FirstName + ' ' + employee.Surname as ReimburseToEmployeeName" & vbCrLf &
                ",CONVERT(varchar, fowst.FowstNO) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, log.ExpenseReferenceNo) AS ExpenseTicketNo" & vbCrLf &
                ",(SELECT ISNULL(STUFF(" & vbCrLf &
                    " (SELECT TOP 100 PERCENT" & vbCrLf &
                    " ', ' + LibraryCRMAttachments.InvoiceRefNo" & vbCrLf &
                    "  FROM TransReceivingInvoiceRefImages" & vbCrLf &
                    "  JOIN LibraryCRMAttachments ON LibraryCRMAttachments.ID = TransReceivingInvoiceRefImages.CRMAttachmentID" & vbCrLf &
                    "  WHERE TransReceivingInvoiceRefImages.ReceivingID = log.ID" & vbCrLf &
                    "  FOR XML PATH (''))" & vbCrLf &
                    "  , 1, 1, ''), '')) AS InvoiceNumbers" & vbCrLf &
                " FROM TransExpensesLogDetails as log " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = log.FowstID" & vbCrLf &
                " left JOIN LibraryDevice as dev ON dev.ID = log.DeviceID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as employee ON employee.ID = log.ReimburseToEmployee" & vbCrLf &
                " WHERE log.ID = " & ReadyString(expenseID)

            Dim pRow As DataRow = Common.WebRequest.ProData.ExecuteRow(psql)
            '2edb5011-a759-4140-8c81-788bfc27be5a use when Payment method is For Reimbursement, Org profile for Employee, since OrganizationID ang need sa CustomerID sa transinvoice
            Dim supplierid As String = IIf(CStr2(pRow("DisburseToVendor").ToString()) = "", "2edb5011-a759-4140-8c81-788bfc27be5a", pRow("DisburseToVendor").ToString())

            Dim pJson As New TransReceiving.SaveJson
            pJson.Receiving.SaveAndFinalize = True
            'pJson.Receiving.ID = CStr2(pRow("ID").ToString())
            pJson.Receiving.SupplierID = CStr2(supplierid)
            pJson.Receiving.BillingAddressID = CStr2("59d37322-f641-453d-a85d-6708057f72dd")
            pJson.Receiving.DeliveryAddressID = CStr2("59d37322-f641-453d-a85d-6708057f72dd")
            pJson.Receiving.DeviceID = CStr2(pRow("DeviceID").ToString())
            pJson.Receiving.UserID = CStr2(pRow("EncodedBy").ToString())
            pJson.Receiving.Total = CDec2(pRow("Amount"))
            pJson.Receiving.ExpenseReferenceNo = CInt2(pRow("ExpenseReferenceNo"))
            pJson.Receiving.ReimburseToEmployeeName = IIf(CStr2(pRow("ReimburseToEmployeeName")) = "", "NULL", CStr2(pRow("ReimburseToEmployeeName")))
            pJson.Receiving.ReceivingPlaceID = CStr2(pRow("FowstID").ToString())
            pJson.Receiving.TransType = TransInvoice.SaveSalesOrderJson.eTransType.ExpenseTicket.ToString()
            pJson.Receiving.Remarks = IIf(CStr2(pRow("ReimburseToEmployeeName")) <> "", "Reimburse To: " & CStr2(pRow("ReimburseToEmployeeName")) & " ", "") & "Particulars: " & CStr2(pRow("Particulars")) & IIf(CStr2(pRow("Remarks")) = "", "", " Remarks:" & pRow("Remarks"))
            pJson.CreatedPo.Remarks = "Created from Expense Ticket " & CStr2(pRow("ExpenseTicketNo")) & vbCrLf & CStr2(pRow("Particulars"))
            pJson.Receiving.InvoiceNumber = "Expense " & CStr2(pRow("ExpenseTicketNo"))
            If String.IsNullOrWhiteSpace(CStr2(pRow("InvoiceNumbers"))) = False Then
                pJson.Receiving.InvoiceNumber &= " - " & CStr2(pRow("InvoiceNumbers")).Trim()
            End If

            Dim pProductItem = New TransReceiving.SaveJson.ProductClass.Item
            pProductItem.GoodQty = 1
            pProductItem.UnitMeasure = WebRequest.Instance.Business.LibraryStockMeasurements.GetOrCreateUnitMeasureID("None")
            pProductItem.UnitQty = 1
            pProductItem.PoUnitPrice = CDec2(pRow("Amount"))
            pJson.Products.ProductID = WebRequest.Instance.Business.LibraryStock.GetOrCreatePimByName("For Payment Disbursement")
            Dim pUnitMeasureID = WebRequest.Instance.Business.LibraryStockMeasurements.GetOrCreateUnitMeasureID("PC")
            WebRequest.Instance.Business.LibraryStockMeasurements.GetOrCreatePimProductMeasurement(pJson.Products.ProductID, pUnitMeasureID)
            WebRequest.Instance.Business.LibraryStock.GetOrCreateRetailPrice(WebRequest.Instance.OrgID, pJson.Products.ProductID, 1)

            pJson.Products.Items.Add(pProductItem)

            If supplierid <> "2edb5011-a759-4140-8c81-788bfc27be5a" Then
                'pJson.Receiving.PaymentTerm = CInt2(Common.WebRequest.ProData.ExecuteRow("SELECT term.Days FROM LibraryPaymentTerms as term " & vbCrLf &
                '                            " INNER JOIN LibraryOrganizationSupplier as supplier ON supplier.CreditTermsID = term.ID" & vbCrLf &
                '                            " WHERE supplier.SupplierID = " & ReadyString(supplierid))("Days"))
            End If

            WebRequest.Instance.ProData.BeginTransaction()
            Dim pResult = WebRequest.Instance.Business.TransReceiving.Save(pJson, WebRequest.Instance.OrgID)
            If pResult.Result = "ok" Then
                WebRequest.Instance.ProData.CommitTransaction()
                Return pResult.Receiving.ID
            Else
                WebRequest.Instance.ProData.RollbackTransaction()
            End If

            Return pResult.Result

        End Function

        Function GetExpenseTicketNo(ByVal deviceid As String, ByVal fowstid As String)
            Dim LastExpenseReferenceNo As String = CInt2(Common.WebRequest.ProData.Execute("SELECT ISNULL(MAX(ExpenseReferenceNo), 0) AS LastExpenseReferenceNo FROM TransExpensesLogDetails WHERE DeviceID = " & ReadyString(deviceid) & " AND FowstID = " & ReadyString(fowstid)).Rows(0)("LastExpenseReferenceNo"))

            Return LastExpenseReferenceNo + 1
        End Function

        Public Function RecacheLogExpense(ByVal data As Object, ByVal mProdata As ProData)
            Try
                Dim month As Integer = CInt2(CDate(data("ExpenseDate")).ToString("MM"))
                Dim year As Integer = CInt2(CDate(data("ExpenseDate")).ToString("yyyy"))
                Dim pSql As String = " SELECT SUM(Amount) as TotalExpense " & vbCrLf &
                                        " FROM TransExpensesLogDetails " & vbCrLf &
                                        " WHERE MONTH(ISNULL(ExpenseDate, DateCreated)) = " & month & vbCrLf &
                                        " AND YEAR(ISNULL(ExpenseDate, DateCreated)) = " & year & vbCrLf &
                                        " AND PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                                        " AND ISNULL(IsDelete, 'False') = 'False' " & vbCrLf &
                                        " AND FowstID = " & ReadyString(data("FowstID").ToString()) & vbCrLf &
                                        " AND ExpenseType = " & ReadyString(data("ExpenseType").ToString())
                Dim pRow As Object = mProdata.Execute(pSql).Rows
                Dim totalamount As Decimal = 0

                If pRow.Count > 0 Then
                    totalamount = pRow(0)("TotalExpense")
                End If

                Dim pTable As New InsertUpdate("TransLogExpensesCache")

                pTable("Amount") = totalamount
                pTable("LastUpdated") = "getdate()"

                If mProdata.ExecuteSQL(pTable.GetUpdate("ExpenseType = " & ReadyString(data("ExpenseType")) & " AND PeopleID = " & ReadyString(data("PeopleID").ToString()) & " AND MonthCovered = " & month & " AND YearCovered = " & year), True) = 0 Then
                    pTable("ID") = ReadyString(Guid.NewGuid.ToString())
                    pTable("ExpenseType") = ReadyString(data("ExpenseType"))
                    pTable("PeopleID") = ReadyString(data("PeopleID"))
                    pTable("MonthCovered") = ReadyString(month)
                    pTable("YearCovered") = ReadyString(year)
                    pTable("FowstID") = ReadyString(data("FowstID"))
                    mProdata.Execute(pTable.GetInsert())
                End If
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try
        End Function

        Function CancelET()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim p As New InsertUpdate("TransExpensesLogDetails")

                p("IsCancelled") = 1
                p("CancelledBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("CancelledOn") = "getdate()"

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID"))))

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Results", "error")
            End Try
        End Function

        Public Function SaveLogExpenseHistory(ByVal origdata As Object, ByVal mProdata As ProData)
            Dim pHistory As New InsertUpdate("TransExpensesLogDetailsHistory")
            pHistory("LogExpenseID") = ReadyString(origdata("ID"))
            pHistory("DateCreated") = ReadyString(origdata("DateCreated"))
            pHistory("PeopleID") = IIf(origdata("PeopleID").ToString() = "", "NULL", ReadyString(origdata("PeopleID")))
            pHistory("EncodedBy") = IIf(origdata("EncodedBy").ToString() = "", "NULL", ReadyString(origdata("EncodedBy")))
            pHistory("ExpenseDate") = IIf(origdata("ExpenseDate").ToString() = "", "NULL", ReadyString(Convert.ToDateTime(origdata("ExpenseDate")).ToString("yyyy-MM-dd")))
            pHistory("ExpenseType") = ReadyString(origdata("ExpenseType"))
            pHistory("FromLocation") = IIf(origdata("FromLocation").ToString() = "", "NULL", ReadyString(origdata("FromLocation")))
            pHistory("ToLocation") = IIf(origdata("ToLocation").ToString() = "", "NULL", ReadyString(origdata("ToLocation")))
            pHistory("Amount") = ReadyString(CDec2(origdata("Amount")))
            pHistory("FowstID") = IIf(origdata("FowstID").ToString() = "", "NULL", ReadyString(origdata("FowstID")))
            pHistory("CostCenter") = IIf(origdata("CostCenter").ToString() = "", "NULL", ReadyString(origdata("CostCenter")))
            pHistory("Remarks") = IIf(origdata("Remarks").ToString() = "", "NULL", ReadyString(origdata("Remarks")))
            pHistory("GPSLocation") = IIf(origdata("GPSLocation").ToString() = "", "NULL", ReadyString(origdata("GPSLocation")))
            pHistory("AssociatedTo") = IIf(origdata("AssociatedTo").ToString() = "", "NULL", ReadyString(origdata("AssociatedTo")))
            pHistory("LastUpdatedBy") = IIf(origdata("LastUpdatedBy").ToString() = "", "NULL", ReadyString(origdata("LastUpdatedBy")))
            pHistory("IsUnfinishedExpense") = IIf(origdata("IsUnfinishedExpense").ToString() = "", "NULL", ReadyString(origdata("IsUnfinishedExpense")))
            pHistory("IsApproved") = IIf(origdata("IsApproved").ToString() = "", "NULL", ReadyString(origdata("IsApproved")))
            pHistory("IsRejected") = IIf(origdata("IsRejected").ToString() = "", "NULL", ReadyString(origdata("IsRejected")))
            pHistory("Particulars") = IIf(origdata("Particulars").ToString() = "", "NULL", ReadyString(origdata("Particulars")))
            pHistory("ApprovedOrRejectedBy") = IIf(origdata("ApprovedOrRejectedBy").ToString() = "", "NULL", ReadyString(origdata("ApprovedOrRejectedBy")))
            pHistory("IsCreatedInWebApp") = IIf(origdata("IsCreatedInWebApp").ToString() = "", 0, ReadyString(origdata("IsCreatedInWebApp")))
            pHistory("LastUpdated") = ReadyString(origdata("LastUpdated").ToString())

            If mProdata.ExecuteSQL(pHistory.GetUpdate("LogExpenseID = " & ReadyString(origdata("ID")) & " AND LastUpdated = " & ReadyString(origdata("LastUpdated").ToString())), True) = 0 Then
                pHistory("ID") = ReadyString(Guid.NewGuid.ToString())
                mProdata.Execute(pHistory.GetInsert())
            End If
        End Function

        Function SaveReceiveFund()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim p As New InsertUpdate("TransExpensesReceiveFunds")
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String

                p("ReceiveDate") = ReadyString(data("ReceiveDate"))
                p("FowstID") = IIf(data("FowstID") = "", "NULL", ReadyString(data("FowstID")))
                p("ReceiveFrom") = IIf(data("ReceiveFrom") = "", "NULL", ReadyString(data("ReceiveFrom")))
                p("OtherPerson") = IIf(data("OtherPerson") = "", "NULL", ReadyString(data("OtherPerson")))
                p("Amount") = ReadyString(CDec2(data("Amount")))
                p("Comment") = ReadyString(data("Comment"))
                p("LastUpdated") = "getdate()"

                If CStr2(data("ID")) = "" Then
                    p("DateCreated") = "getdate()"
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PeopleID") = IIf(data("PeopleID") = "", "NULL", ReadyString(data("PeopleID")))
                    p("EncodedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    'GET ORIGINAL DATA
                    pSql = "SELECT * FROM TransExpensesReceiveFunds WHERE ID = " & ReadyString(data("ID"))
                    Dim origdata As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                    'SAVE ORIGINAL DATA TO HISTORY TABLE FOR FUTURE AUDITABILITY - July 31, 2020 by Arnie
                    SaveReceiveFundHistory(origdata, Common.WebRequest.ProData)

                    p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))
                End If

                If data("PeopleID") <> "" Then
                    Dim datecovered As DateTime = Convert.ToDateTime(data("ReceiveDate"))
                    UpdateNetFundBalance(datecovered.Month & " " & datecovered.Year & " " & data("PeopleID"), Common.WebRequest.ProData, data("FowstID"))
                End If

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("TransExpensesReceiveFunds")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Public Function SaveReceiveFundHistory(ByVal origdata As Object, ByVal mProdata As ProData)
            Try
                Dim p As New InsertUpdate("TransExpensesReceiveFundsHistory")
                p("ReceiveFundID") = ReadyString(origdata("ID").ToString())
                p("ReceiveDate") = ReadyString(origdata("ReceiveDate"))
                p("ReceiveFrom") = IIf(origdata("ReceiveFrom").ToString() = "", "NULL", ReadyString(origdata("ReceiveFrom").ToString()))
                p("FowstID") = IIf(origdata("FowstID").ToString() = "", "NULL", ReadyString(origdata("FowstID").ToString()))
                p("OtherPerson") = IIf(origdata("OtherPerson").ToString() = "", "NULL", ReadyString(origdata("OtherPerson").ToString()))
                p("Amount") = ReadyString(CDec2(origdata("Amount")))
                p("Comment") = IIf(origdata("Comment").ToString() = "", "NULL", ReadyString(origdata("Comment").ToString()))
                p("LastUpdated") = ReadyString(origdata("LastUpdated"))
                p("DateCreated") = ReadyString(origdata("DateCreated"))
                p("PeopleID") = IIf(origdata("PeopleID").ToString() = "", "NULL", ReadyString(origdata("PeopleID").ToString()))
                p("EncodedBy") = IIf(origdata("EncodedBy").ToString() = "", "NULL", ReadyString(origdata("EncodedBy").ToString()))
                p("LastUpdatedBy") = IIf(origdata("LastUpdatedBy").ToString() = "", "NULL", ReadyString(origdata("LastUpdatedBy").ToString()))

                If mProdata.ExecuteSQL(p.GetUpdate("ReceiveFundID = " & ReadyString(origdata("ID").ToString()) & " AND LastUpdated = " & ReadyString(origdata("LastUpdated").ToString())), True) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    mProdata.Execute(p.GetInsert())
                End If
            Catch ex As Exception
                AppendToLog("Error when saving receivefundhistory")
                AppendToLog(ex.Message)
            End Try
        End Function

        Function GenerateDefaultCostCenter()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("LibraryExpensesCenters")

                p("Name") = ReadyString("General Expense")
                p("Description") = ReadyString("General or miscellaneous expenses")
                p("Type") = ReadyString("Cost Centers")
                p("ModifiedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                Common.WebRequest.ProData.BeginTransaction()

                If Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryExpensesCenters WHERE ID = '817b4c2a-af4b-4f58-ba21-c3907320aa32' AND ISNULL(ModifiedDate, '') = ''").Rows.Count = 1 Then
                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString("817b4c2a-af4b-4f58-ba21-c3907320aa32"))) = 0 Then
                        p("ID") = ReadyString("817b4c2a-af4b-4f58-ba21-c3907320aa32")
                        p("DateCreated") = "getdate()"
                        p("IsActive") = 1
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                ElseIf Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryExpensesCenters WHERE ID = '817b4c2a-af4b-4f58-ba21-c3907320aa32'").Rows.Count = 0 Then
                    p("IsActive") = 1
                    p("ID") = ReadyString("817b4c2a-af4b-4f58-ba21-c3907320aa32")
                    p("DateCreated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetCostCenters()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT * FROM LibraryExpensesCenters WHERE Type = 'Cost Centers' AND ISNULL(IsActive, 1) = 1 ORDER BY Name ASC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Public Shared Function UpdateNetFundBalance(ByVal dc As String, ByVal mProdata As ProData, ByVal fowstid As String)
            Dim fund As Decimal = 0
            Dim carryover As Decimal = 0
            Dim expense As Decimal = 0
            Dim month As String = dc.Split(" ")(0)
            Dim year As String = dc.Split(" ")(1)
            Dim peopleid As String = dc.Split(" ")(2)
            Dim monthcovered As String = IIf(month = "", "DatePart(MM, getdate())", month)
            Dim yearcovered As String = IIf(year = "", "DatePart(YY, getdate())", year)
            Dim carryovermonth = monthcovered
            Dim carryoveryear = yearcovered
            Dim startmonthtobeupdated As Date = CDate(monthcovered & "-1-" & yearcovered)
            Dim endmonthtobeupdated As Date = CDate(CDate(CDate(DateTime.Now).AddMonths(1)).ToString("MM-1-yyyy"))
            Dim pRow As Object

            Try
                Do While startmonthtobeupdated <> endmonthtobeupdated
                    Dim pSql As String = "SELECT SUM(Total) as Total FROM ( " & vbCrLf &
                                    "   SELECT ISNULL(SUM(Amount), 0) as Total FROM TransExpensesReceiveFunds " & vbCrLf &
                                    "   WHERE DatePart(MM, ReceiveDate) = " & monthcovered & vbCrLf &
                                    "   AND DatePart(YY, ReceiveDate) = " & yearcovered & vbCrLf &
                                    "   AND PeopleID = " & ReadyString(peopleid) & vbCrLf &
                                    "   AND IsDelete = 0 " & vbCrLf &
                                    "   AND FowstID =  " & ReadyString(fowstid) & vbCrLf &
                                    " UNION ALL " & vbCrLf &
                                    "   SELECT SUM(NetFundBalance) As Total FROM TransNetFundBalance " & vbCrLf &
                                    "   WHERE PeopleID = " & ReadyString(peopleid) & vbCrLf &
                                    "   AND MonthCovered + ' ' + YearCovered <> convert(varchar, " & monthcovered & ") + ' ' + convert(varchar, " & yearcovered & ") " & vbCrLf &
                                    ") as T " & vbCrLf

                    pSql = "   SELECT ISNULL(SUM(Amount), 0) as Total FROM TransExpensesReceiveFunds " & vbCrLf &
                            "   WHERE DatePart(MM, ReceiveDate) = " & monthcovered & vbCrLf &
                            "   AND DatePart(YY, ReceiveDate) = " & yearcovered & vbCrLf &
                            "   AND PeopleID = " & ReadyString(peopleid) & vbCrLf &
                            "   AND IsDelete = 0 " & vbCrLf &
                            "   AND FowstID =  " & ReadyString(fowstid) & vbCrLf

                    pRow = mProdata.Execute(pSql).Rows

                    fund = CDec2(pRow(0)("Total"))

                    If carryovermonth = 1 Then
                        carryovermonth = 12
                        carryoveryear = carryoveryear - 1
                    Else
                        carryovermonth = carryovermonth - 1
                    End If

                    'lahi ang result diri ug sa sql server
                    pSql = " SELECT TOP 1 * " & vbCrLf &
                            " FROM TransNetFundBalance " & vbCrLf &
                            " WHERE (MonthCovered = " & carryovermonth & vbCrLf &
                            " AND YearCovered = " & carryoveryear & ") " & vbCrLf &
                            " AND PeopleID = " & ReadyString(peopleid) & vbCrLf &
                            " AND (ISNULL(Status, '') = 'Approved' OR ISNULL(IsAdjustment, 0) = 0) " & vbCrLf &
                            " AND ISNULL(IsCancelRequest, 0) = 0 " & vbCrLf &
                            " ORDER BY IsAdjustment DESC, LastUpdated DESC, Status ASC "

                    pRow = mProdata.Execute(pSql).Rows

                    If pRow.Count > 0 Then
                        carryover = CDec2(pRow(0)("NetFundBalance"))
                    End If

                    'pSql = "SELECT ISNULL(SUM(Amount), 0) as Total FROM TransExpensesLogDetails " & vbCrLf &
                    '        " WHERE DatePart(MM, DateCreated) = DatePart(MM, getdate()) " & vbCrLf &
                    '        " AND DatePart(YY, DateCreated) = DatePart(YY, getdate()) " & vbCrLf &
                    '        " AND PeopleID = " & ReadyString(peopleid) & vbCrLf &
                    '        " AND IsDelete = 0 "

                    pSql = "SELECT ISNULL(SUM(Amount), 0) as Total FROM TransExpensesLogDetails " & vbCrLf &
                            " WHERE convert(varchar, DATEPART(m, ISNULL(ExpenseDate, DateCreated))) + ' ' + convert(varchar, DATEPART(yyyy, ISNULL(ExpenseDate, DateCreated))) = convert(varchar, " & monthcovered & ") + ' ' + convert(varchar, " & yearcovered & ") " & vbCrLf &
                            " AND PeopleID = " & ReadyString(peopleid) & vbCrLf &
                            " AND FowstID =  " & ReadyString(fowstid) & vbCrLf &
                            " AND IsDelete = 0 "

                    pRow = mProdata.Execute(pSql).Rows

                    If pRow.Count > 0 Then
                        expense = CDec2(pRow(0)("Total"))
                    End If

                    Dim begbal As Decimal = (fund + carryover) - expense

                    'If begbal < 0 Then
                    '    begbal = 0
                    'End If

                    Dim p As New InsertUpdate("TransNetFundBalance")

                    p("FowstID") = ReadyString(fowstid)
                    p("PeopleID") = ReadyString(peopleid)
                    p("NetFundBalance") = begbal
                    p("MonthCovered") = monthcovered '"DatePart(MM, getdate())"
                    p("YearCovered") = yearcovered ' "DatePart(YY, getdate())"
                    p("LastUpdated") = "getdate()"

                    pSql = p.GetUpdate("ISNULL(IsAdjustment, 0) = 0 AND MonthCovered = " & monthcovered & " AND YearCovered = " & yearcovered & " AND PeopleID = " & ReadyString(peopleid) &
                                       " AND FowstID =  " & ReadyString(fowstid) & vbCrLf)

                    If mProdata.ExecuteSQL(pSql, True) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert
                        mProdata.ExecuteSQL(pSql)
                    End If

                    If monthcovered = 12 Then
                        monthcovered = 1
                        yearcovered += 1
                    Else
                        monthcovered += 1
                    End If

                    startmonthtobeupdated = CDate(CDate(startmonthtobeupdated).AddMonths(1)).ToString("MM-1-yyyy")
                    carryovermonth = CInt2(CDate(CDate(startmonthtobeupdated)).ToString("MM"))
                    carryoveryear = CInt2(CDate(CDate(startmonthtobeupdated)).ToString("yyyy"))
                Loop
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try
        End Function

        Function CheckAccess()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDT As DataTable = Common.WebRequest.ProData.Execute("SELECT PeopleID FROM LibraryExpensesFundAdjustmentsAdmin " & vbCrLf &
                                                                     " WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID")))

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetEmployeesToApproveFA()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT admin.PeopleID as ID, ppol.FirstName + ' ' + ppol.Surname as PeopleName  " & vbCrLf &
                                " FROM LibraryExpensesFundAdjustmentsAdmin as admin " & vbCrLf &
                                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = admin.PeopleID "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))

        End Function

        Function SaveEmployeesToApproveFA()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim data As Object = Common.WebRequest.AllData("data")

                Common.WebRequest.ProData.BeginTransaction()

                For Each emp As Object In data
                    Dim p As New InsertUpdate("LibraryExpensesFundAdjustmentsAdmin")

                    p("PeopleID") = ReadyString(emp("ID"))

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" PeopleID = " & ReadyString(emp("ID"))), True) = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid().ToString())
                        p("UserID") = ReadyString(WebSession.Instance.PeopleID)
                        p("LastUpdated") = "getdate()"
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next emp

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function UpdateAdjustment()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("TransNetFundBalance")
                Dim data As Object = Common.WebRequest.AllData("data")

                p("RequestUpdatedBy") = ReadyString(WebSession.Instance.PeopleID)
                p("LastUpdated") = "getdate()"
                p("Status") = ReadyString(data("Status"))

                If data("Status") = "Approved" Then
                    p("NetFundBalance") = CDec2(data("Amount"))
                End If

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))

                'UpdateNetFundBalance(data("MonthCovered") & " " & data("YearCovered") & " " & data("PeopleID"), Common.WebRequest.ProData, data("FowstID"))

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetUpdatesRAF()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")

                Dim pSql As String = "SELECT net.*, ppol.FirstName + ' ' + ppol.Surname as RequestUpdatedByName FROM TransNetFundBalance as net " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = net.RequestUpdatedBy " & vbCrLf &
                                    " WHERE net.ID = " & ReadyString(data("ReqID").ToString())

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DeleteLogExpense() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim expenseID As String = Common.WebRequest.Data("ExpenseID")
                Dim employeeID As String = Common.WebRequest.Data("EmployeeID")
                Dim fowstid As String = Common.WebRequest.Data("FowstID")
                Dim month As String = Common.WebRequest.Data("MonthCovered")
                Dim year As String = Common.WebRequest.Data("YearCovered")
                Dim IsFromCheckInOut As String = Common.WebRequest.Data("IsFromCheckInOut")
                Dim p As New InsertUpdate("TransExpensesCheckInOutDetails")
                Dim pSql As String
                Dim pDt As DataTable

                Common.WebRequest.ProData.BeginTransaction()

                If CBool2(IsFromCheckInOut) Then
                    pSql = "SELECT ID, CheckInLogExpenseID, CheckOutLogExpenseID FROM TransExpensesCheckInOutDetails " & vbCrLf &
                            " WHERE CheckInLogExpenseID = " & ReadyString(expenseID) & " AND CheckOutLogExpenseID = " & ReadyString(expenseID)
                    pDt = Common.WebRequest.ProData.Execute(pSql)

                    If pDt.Rows.Count > 0 Then
                        If CStr2(pDt.Rows(0)("CheckInLogExpenseID")) <> "" Then
                            p("CheckInLogExpenseID") = "NULL"
                        End If

                        If CStr2(pDt.Rows(0)("CheckOutLogExpenseID")) <> "" Then
                            p("CheckOutLogExpenseID") = "NULL"
                        End If

                        'Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(pDt.Rows(0)("ID"))))
                    End If
                End If

                p = New InsertUpdate("TransExpensesLogDetails")

                p("IsDelete") = 1
                p("LastUpdated") = "getdate()"

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(expenseID)))

                UpdateNetFundBalance(month & " " & year & " " & employeeID, Common.WebRequest.ProData, fowstid)

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetMoreDetailsOfExpenseType() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String
                Dim dayOfWeek = CInt(DateTime.Today.DayOfWeek)
                Dim startOfWeek = DateTime.Today.AddDays(-1 * dayOfWeek)

                pSql = " SELECT * FROM TransExpensesLogDetails " & vbCrLf &
                        " WHERE PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " AND ExpenseType = " & ReadyString(data("ExpenseType")) & vbCrLf &
                        " AND ISNULL(IsDelete, 'False') = 'False'" & vbCrLf

                If data("Period") = "today" Then
                    pSql += " AND ISNULL(ExpenseDate, DateCreated) = " & ReadyString(Format(Date.Now, "yyyy-MM-dd")) & vbCrLf
                ElseIf data("Period") = "week" Then
                    pSql += " AND ISNULL(ExpenseDate, DateCreated) BETWEEN " & ReadyString(Format(startOfWeek, "yyyy-MM-dd")) & " AND " & ReadyString(Format(Date.Now, "yyyy-MM-dd")) & vbCrLf
                ElseIf data("Period") = "month" Then
                    pSql += " AND ISNULL(ExpenseDate, DateCreated) BETWEEN " & ReadyString(Format(Date.Now, "yyyy-MM-01")) & " AND " & ReadyString(Format(Date.Now, "yyyy-MM-dd")) & vbCrLf
                End If

                pSql += " ORDER BY ISNULL(ExpenseDate, DateCreated) " & data("Sort")

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetMoreDetailsOfReceivedFunds() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String

                pSql = " SELECT fund.*, CASE WHEN ISNULL(CONVERT(varchar(max), fund.ReceiveFrom), '') = '' THEN fund.OtherPerson ELSE ppol.FirstName + ' ' + ppol.Surname END as PeopleName " & vbCrLf &
                        " FROM TransExpensesReceiveFunds as fund " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = fund.ReceiveFrom " & vbCrLf &
                        " WHERE fund.PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " AND DatePart(MM, fund.ReceiveDate) = DatePart(MM, getdate()) " & vbCrLf &
                        " AND DatePart(YY, fund.ReceiveDate) = DatePart(YY, getdate()) " & vbCrLf &
                        " AND ISNULL(fund.IsDelete, 'False') = 'False'" & vbCrLf &
                        " ORDER BY fund.ReceiveDate DESC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetMoreDetailsOfCheckInOut() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pSql As String

                pSql = " SELECT checks.*, " & vbCrLf &
                        " logcheckin.Amount as CheckinAmount, logcheckin.FromLocation as CheckinFromLocation, logcheckin.ToLocation as CheckinToLocation, " & vbCrLf &
                        " logcheckout.Amount as CheckoutAmount, logcheckout.FromLocation as CheckoutFromLocation, logcheckout.ToLocation as CheckoutToLocation " & vbCrLf &
                        " FROM TransExpensesCheckInOutDetails as checks " & vbCrLf &
                        " LEFT JOIN TransExpensesLogDetails as logcheckin ON logcheckin.ID = checks.CheckInLogExpenseID " & vbCrLf &
                        " LEFT JOIN TransExpensesLogDetails as logcheckout ON logcheckout.ID = checks.CheckOutLogExpenseID " & vbCrLf &
                        " WHERE checks.PeopleID = " & ReadyString(data("PeopleID")) & vbCrLf &
                        " AND DatePart(MM, checks.CheckIn) = DatePart(MM, getdate()) " & vbCrLf &
                        " AND DatePart(YY, checks.CheckIn) = DatePart(YY, getdate()) " & vbCrLf &
                        " ORDER BY checks.CheckIn " & data("Order")

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetLogExpenseByEmployee() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
                Dim IsCountPage As Boolean
                Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()

                Dim whereSearch As String = ""
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " OR "
                        End If

                        whereSearchContent = whereSearchContent & operators & " ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') " & vbCrLf &
                                        " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next
                    whereSearch = " WHERE (" & whereSearchContent & ")"
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.EmployeeName asc, ISNULL(mainsql.ExpenseDate, mainsql.DateCreated), mainsql.DateCreated " & PageColumnDirection
                    'ElseIf PageColumnNo = 2 Then
                    '    whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
                    'ElseIf PageColumnNo = 5 Then
                    '    whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
                End If

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        GetLogExpenseByEmployeeQuery() & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(GetLogExpenseByEmployeeQuery())

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetGeneralExpenses() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
                Dim IsCountPage As Boolean
                Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()

                Dim whereSearch As String = ""
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " OR "
                        End If

                        whereSearchContent = whereSearchContent & operators & " ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') " & vbCrLf &
                                        " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next
                    whereSearch = " WHERE (" & whereSearchContent & ")"
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.EmployeeName asc, ISNULL(mainsql.ExpenseDate, mainsql.DateCreated), mainsql.DateCreated " & PageColumnDirection
                    'ElseIf PageColumnNo = 2 Then
                    '    whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
                    'ElseIf PageColumnNo = 5 Then
                    '    whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
                End If

                Dim q As String = " SELECT log.*, org.CompanyName, fowst.Name as FowstName, cen.Name as CostCenterName, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID" & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = log.AssociatedTo " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = log.FowstID " & vbCrLf &
                        " LEFT JOIN LibraryExpensesCenters as cen ON cen.ID = log.CostCenter " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = log.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = log.LastUpdatedBy " & vbCrLf &
                        " WHERE ISNULL(log.Amount, '') = '' " & vbCrLf &
                        " AND ISNULL(log.IsDelete, 'False') = 'False'" & vbCrLf &
                        " AND IsUnfinishedExpense = 1 "

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        q & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(q)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetUnapprovedExpenseTickets() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
                Dim IsCountPage As Boolean
                Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()
                Dim showtickets As String = Request.Params.Get("ShowTickets")
                Dim keyword As Object = Split(CStr2(Common.WebRequest.QueryString("keywords")), "+")

                Dim whereSearch As String = ""
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " (ISNULL(CONVERT(varchar(max), fowst.FowstNo), '') + '-' + ISNULL(CONVERT(varchar(max), dev.DeviceNo), '') + '-' + ISNULL(CONVERT(varchar, log.ExpenseReferenceNo), '')) + ' ' + " & vbCrLf &
                             " ISNULL(log.ExpenseType, '') + ' ' + " & vbCrLf &
                             " ISNULL(ppol.FirstName, '') + ' ' + " & vbCrLf &
                             " ISNULL(ppol.Surname, '') + ' ' + " & vbCrLf &
                             " SUBSTRING(ISNULL(CONVERT(varchar(max), ExpenseDate), ''), 1, 11) + ' ' + " & vbCrLf &
                             " ISNULL(CONVERT(varchar(max), Amount), '') + ' ' + " & vbCrLf &
                             " ISNULL(org.CompanyName, '') + ' ' + " & vbCrLf &
                             " ISNULL(log.Remarks, '') + ' ' + " & vbCrLf &
                             " ISNULL(cen.Name, 'General Expense') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next
                    whereSearch = " WHERE (" & whereSearchContent & ")"
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.EmployeeName asc, ISNULL(mainsql.ExpenseDate, mainsql.DateCreated), mainsql.DateCreated " & PageColumnDirection
                    'ElseIf PageColumnNo = 2 Then
                    '    whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
                    'ElseIf PageColumnNo = 5 Then
                    '    whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
                End If

                Dim q As String = " SELECT log.*, org.CompanyName, fowst.Name as FowstName, cen.Name as CostCenterName, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName, " & vbCrLf &
                        " approvedorrejectedby.FirstName + ' ' + approvedorrejectedby.Surname as ApprovedOrRejectedByName " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID" & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = log.AssociatedTo " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = log.FowstID " & vbCrLf &
                        " LEFT JOIN LibraryExpensesCenters as cen ON cen.ID = log.CostCenter " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = log.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = log.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as approvedorrejectedby ON approvedorrejectedby.ID = log.ApprovedOrRejectedBy " & vbCrLf &
                        " WHERE ISNULL(log.IsDelete, 'False') = 'False'" & vbCrLf

                If showtickets = "unapproved" Then
                    q &= " AND IsApproved = 0 AND IsRejected = 0 "
                ElseIf showtickets = "approved" Then
                    q &= " AND IsApproved = 1 AND IsRejected = 0 "
                ElseIf showtickets = "rejected" Then
                    q &= " AND IsApproved = 0 AND IsRejected = 1 "
                End If

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        q & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(q)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DownloadExpenseTicketReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pReport As ProReport
                Dim pOrgShort As New OrganizationShort
                Dim file As System.IO.FileInfo
                Dim pFilename = "ExpenseTicketReport"
                Dim pSql As String = " SELECT log.*, ISNULL(org.CompanyName, 'N/A') as AssociatedCompanyName, fowst.Name as FowstName, cen.Name as CostCenterName, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName, " & vbCrLf &
                        " approvedorrejectedby.FirstName + ' ' + approvedorrejectedby.Surname as ApprovedOrRejectedByName, " & vbCrLf &
                        " reimburseto.FirstName + ' ' + reimburseto.Surname as ReimburseToEmployeeName, " & vbCrLf &
                        " requestedby.FirstName + ' ' + requestedby.Surname as RequestedByName, " & vbCrLf &
                        " CONVERT(varchar, fowst.FowstNO) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, log.ExpenseReferenceNo) as ExpenseTicketNo, " & vbCrLf &
                        " disburse.CompanyName as DisburseToVendorName, " & vbCrLf &
                        " terms.Days as CreditTermsInDays, CONVERT(varchar, CAST(Amount AS money), 1) as ConvertedAmount " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID" & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = log.AssociatedTo " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = log.FowstID " & vbCrLf &
                        " LEFT JOIN LibraryExpensesCenters as cen ON cen.ID = log.CostCenter " & vbCrLf &
                        " LEFT JOIN LibraryDevice as dev ON dev.ID = log.DeviceID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = log.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = log.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as approvedorrejectedby ON approvedorrejectedby.ID = log.ApprovedOrRejectedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as reimburseto ON reimburseto.ID = log.ReimburseToEmployee " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as requestedby ON requestedby.ID = log.RequestedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as disburse ON disburse.ID = log.DisburseToVendor " & vbCrLf &
                        " LEFT JOIN LibraryOrganizationSupplier as vendor ON vendor.SupplierID = log.DisburseToVendor " & vbCrLf &
                        " LEFT JOIN LibraryPaymentTerms as terms ON terms.ID = vendor.CreditTermsID " & vbCrLf &
                        " WHERE log.ID = " & ReadyString(Request.Params.Get("ID"))
                Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

                pReport = New ProReport()
                pReport.AutoFillHeader = False
                pReport.SourceFilename = pFilename

                pReport.AddTable("Info", pDT)
                pReport.AddTable("CompanyInfo", pOrgShort.GetProspectOrgDetails(WebRequest.Instance.OrgID))

                ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"

                pFilename = Common.ServerPath & "FileTemplates\ET-" & pDT.Rows(0)("ExpenseTicketNo") & ".pdf"

                pReport.ExportXmlFile()
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

                file = New System.IO.FileInfo(pFilename)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=ET-" & pDT.Rows(0)("ExpenseTicketNo") & ".pdf")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetFundByEmployee() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
                Dim IsCountPage As Boolean
                Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()

                Dim whereSearch As String = ""
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " OR "
                        End If

                        whereSearchContent = whereSearchContent & operators & " ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') " & vbCrLf &
                                        " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next
                    whereSearch = " WHERE (" & whereSearchContent & ")"
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.EmployeeName asc, mainsql.ReceiveDate " & PageColumnDirection
                    'ElseIf PageColumnNo = 2 Then
                    '    whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
                    'ElseIf PageColumnNo = 5 Then
                    '    whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
                End If

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        GetFundByEmployeeQuery() & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(GetFundByEmployeeQuery())

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetFundAdjustmentByEmployee() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
                Dim IsCountPage As Boolean
                Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()

                Dim whereSearch As String = ""
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " OR "
                        End If

                        whereSearchContent = whereSearchContent & operators & " ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + ' ' + " & vbCrLf &
                                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') " & vbCrLf &
                                        " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next
                    whereSearch = " WHERE (" & whereSearchContent & ")"
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.LastUpdated DESC "
                    'ElseIf PageColumnNo = 2 Then
                    '    whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
                    'ElseIf PageColumnNo = 5 Then
                    '    whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
                End If

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        GetFundAdjustmentByEmployeeQuery() & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(GetFundAdjustmentByEmployeeQuery())

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetLogExpenseByEmployeeQuery() As String
            Dim pSql As String = " SELECT log.*, org.CompanyName, fowst.Name as FowstName, cen.Name as CostCenterName, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID" & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = log.AssociatedTo " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = log.FowstID " & vbCrLf &
                        " LEFT JOIN LibraryExpensesCenters as cen ON cen.ID = log.CostCenter " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = log.EncodedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = log.LastUpdatedBy " & vbCrLf &
                        " WHERE ISNULL(log.ExpenseDate, log.DateCreated) BETWEEN " & ReadyString(Request.Params.Get("FromDate")) & vbCrLf &
                        " AND " & ReadyString(Request.Params.Get("ToDate")) & vbCrLf &
                        " AND ISNULL(log.IsDelete, 'False') = 'False'" & vbCrLf &
                        " AND ISNULL(log.IsApproved, 1) = 1 " & vbCrLf &
                        IIf(Request.Params.Get("ExpenseType") = "all", "", " AND log.ExpenseType = " & ReadyString(Request.Params.Get("ExpenseType"))) & vbCrLf

            If Request.Params.Get("PeopleID") = "all" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), PeopleID), '') <> '' " '817B4C2A-AF4B-4F58-BA21-C3907320AA32 = General Expense
            ElseIf IsGuid(Request.Params.Get("PeopleID")) Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), PeopleID), '') = " & ReadyString(Request.Params.Get("PeopleID"))
            ElseIf Request.Params.Get("PeopleID") = "na" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), PeopleID), '') = '' "
            End If

            If Request.Params.Get("CostCenter") = "all" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), CostCenter), '817B4C2A-AF4B-4F58-BA21-C3907320AA32') <> '' " '817B4C2A-AF4B-4F58-BA21-C3907320AA32 = General Expense
            ElseIf IsGuid(Request.Params.Get("CostCenter")) Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), CostCenter), '817B4C2A-AF4B-4F58-BA21-C3907320AA32') = " & ReadyString(Request.Params.Get("CostCenter"))
            End If

            If Request.Params.Get("CostCenterLocation") = "all" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), FowstID), '') <> '' "
            Else
                pSql &= " AND ISNULL(CONVERT(varchar(max), FowstID), '') = " & ReadyString(Request.Params.Get("CostCenterLocation"))
            End If

            Return pSql
        End Function

        Function GetFundByEmployeeQuery() As String
            Dim pSql As String = " SELECT fund.*, " & vbCrLf &
                    " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                    " rcvfrom.FirstName + ' ' + rcvfrom.Surname as ReceiveFromName, " & vbCrLf &
                    " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName " & vbCrLf &
                    " FROM TransExpensesReceiveFunds as fund " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = fund.PeopleID" & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as rcvfrom ON rcvfrom.ID = fund.ReceiveFrom" & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = fund.LastUpdatedBy " & vbCrLf &
                    " WHERE fund.ReceiveDate BETWEEN " & ReadyString(Request.Params.Get("FromDate")) & vbCrLf &
                    " AND " & ReadyString(Request.Params.Get("ToDate")) & vbCrLf &
                    " AND ISNULL(fund.IsDelete, 'False') = 'False'" & vbCrLf &
                    " AND fund.FowstID = " & ReadyString(Request.Params.Get("CostCenterLocation")) & vbCrLf

            If Request.Params.Get("PeopleID") = "all" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), fund.PeopleID), '') <> '' " '817B4C2A-AF4B-4F58-BA21-C3907320AA32 = General Expense
            ElseIf IsGuid(Request.Params.Get("PeopleID")) Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), fund.PeopleID), '') = " & ReadyString(Request.Params.Get("PeopleID"))
            ElseIf Request.Params.Get("PeopleID") = "na" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), fund.PeopleID), '') = '' "
            End If

            Return pSql
        End Function

        Function GetFundAdjustmentByEmployeeQuery() As String
            Dim pSql As String = "SELECT net.*, " & vbCrLf &
                    "   updatedby.FirstName + ' ' + updatedby.Surname as UpdatedBy, " & vbCrLf &
                    "   requestedby.FirstName + ' ' + requestedby.Surname as RequestedBy, " & vbCrLf &
                    "   fowst.Name as FowstName " & vbCrLf &
                    "   FROM TransNetFundBalance as net " & vbCrLf &
                    "   INNER JOIN MobizLibraryPeople as requestedby ON requestedby.ID = net.PeopleID " & vbCrLf &
                    "   LEFT JOIN MobizLibraryPeople as updatedby ON updatedby.ID = net.RequestUpdatedBy " & vbCrLf &
                    "   LEFT JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = net.FowstID " & vbCrLf &
                    "   WHERE IsAdjustment = 1 " & vbCrLf &
                    "   AND ISNULL(IsCancelRequest, 0) = 0 " & vbCrLf &
                    "   AND MonthCovered + ' ' + YearCovered = '" & System.DateTime.Now.AddMonths(-1).Month() & " " & System.DateTime.Now.AddMonths(-1).Year() & "' " & vbCrLf

            If Request.Params.Get("PeopleID") = "all" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), net.PeopleID), '') <> '' " '817B4C2A-AF4B-4F58-BA21-C3907320AA32 = General Expense
            ElseIf IsGuid(Request.Params.Get("PeopleID")) Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), net.PeopleID), '') = " & ReadyString(Request.Params.Get("PeopleID"))
            ElseIf Request.Params.Get("PeopleID") = "na" Then
                pSql &= " AND ISNULL(CONVERT(varchar(max), net.PeopleID), '') = '' "
            End If

            Return pSql
        End Function

        'Function GetRouteSchedulesReportByEmployee()
        '    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        '    Try
        '        Dim fromdate As String = Request.Params.Get("FromDate")
        '        Dim todate As String = Request.Params.Get("ToDate")
        '        Dim employeeID As String = Request.Params.Get("PeopleID")
        '        Dim pOut As New JsonDictionary
        '        Dim pSql As String = "SELECT route.*, emp.FirstName + ' ' + emp.Surname as EmployeeName, ISNULL(location.CompanyName, location.PersonFirstName + ' ' + location.PersonSurname) as LocationName, fowst.Name as BranchName" & vbCrLf &
        '                            " , rsemp.ID as ParentID, rsemp.RouteName, rsemp.RouteDescription " & vbCrLf &
        '                            " FROM LibraryExpensesRouteSchedules as route " & vbCrLf &
        '                            " LEFT JOIN LibraryExpensesRouteSchedulesEmployees as rsemp ON rsemp.EmployeeID = route.EmployeeID " & vbCrLf &
        '                            " INNER JOIN MobizLibraryPeople as emp ON emp.ID = route.EmployeeID " & vbCrLf &
        '                            " INNER JOIN MobizLibraryOrganization as location ON location.ID = route.LocationID " & vbCrLf &
        '                            " INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = route.FowstID " & vbCrLf &
        '                            " WHERE route.EmployeeID = " & ReadyString(employeeID) & vbCrLf &
        '                            " ORDER BY Day, StartTime ASC "
        '        Dim scheds As DataTable = Common.WebRequest.ProData.Execute(pSql)
        '        scheds.Columns.Add("Actual")
        '        For Each sched As DataRow In scheds.Rows
        '            pSql = GetCheckInOutDetailsByEmployeeQuery()
        '            pSql += " AND DATENAME(WEEKDAY, checked.CheckIn) = CASE " & CInt2(sched("Day")) & " WHEN 0 THEN 'Monday' " & vbCrLf &
        '                    " WHEN 1 THEN 'Tuesday' " & vbCrLf &
        '                    " WHEN 2 THEN 'Wednesday' " & vbCrLf &
        '                    " WHEN 3 THEN 'Thursday' " & vbCrLf &
        '                    " WHEN 4 THEN 'Friday' " & vbCrLf &
        '                    " WHEN 5 THEN 'Saturday' " & vbCrLf &
        '                    " WHEN 6 THEN 'Sunday' " & vbCrLf &
        '                    " ELSE 'Any Day' " & vbCrLf &
        '                    " END " & vbCrLf &
        '                    " AND checked.CheckInCustomerOrgID = " & ReadyString(sched("LocationID")) & vbCrLf &
        '                    " AND checked.CheckInCustomerOrgOutletID = " & ReadyString(sched("FowstID")) & vbCrLf &
        '                    " ORDER BY checked.CheckIn ASC "

        '            sched("Actual") = DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))
        '        Next

        '        pOut.Add("Results", scheds)

        '        Return pOut.Serialize()
        '    Catch ex As Exception
        '        AppendToDebug(ex.Message)
        '        Return JsonDictionary.Serialize("Error", ex.Message)
        '    End Try
        'End Function

        Function GetRouteSchedulesReportByEmployeeV2()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim fromdate As String = Request.Params.Get("FromDate")
                Dim todate As String = Request.Params.Get("ToDate")
                Dim employeeID As String = Request.Params.Get("PeopleID")
                Dim pOut As New JsonDictionary
                Dim pSql As String = " -- kwaon ang Route Planned Schedule " & vbCrLf &
                                    " SELECT ISNULL(location.CompanyName, location.PersonFirstName + ' ' + location.PersonSurname)  + ' - ' + fowst.Name as CheckInLocationName" & vbCrLf &
                                    " , route.LocationID, route.FowstID " & vbCrLf &
                                    " FROM LibraryExpensesRouteSchedules as route " & vbCrLf &
                                    " INNER JOIN MobizLibraryOrganization as location ON location.ID = route.LocationID " & vbCrLf &
                                    " INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = route.FowstID " & vbCrLf &
                                    " WHERE route.EmployeeID = " & ReadyString(employeeID) & vbCrLf &
                                    " GROUP BY location.CompanyName, location.PersonFirstName, location.PersonSurname, fowst.Name, route.LocationID, route.FowstID " & vbCrLf &
                                    " -- kwaon ang wala naka sched na client " & vbCrLf &
                                    " UNION ALL " & vbCrLf &
                                    " SELECT ISNULL(location.CompanyName, location.PersonFirstName + ' ' + location.PersonSurname)  + ' - ' + fowst.Name as CheckInLocationName" & vbCrLf &
                                    " , checkin.CheckInCustomerOrgID, checkin.CheckInCustomerOrgOutletID " & vbCrLf &
                                    " FROM TransExpensesCheckInOutDetails as checkin " & vbCrLf &
                                    " INNER JOIN MobizLibraryOrganization as location ON location.ID = checkin.CheckInCustomerOrgID " & vbCrLf &
                                    " INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = checkin.CheckInCustomerOrgOutletID " & vbCrLf &
                                    " WHERE checkin.PeopleID = " & ReadyString(employeeID) & vbCrLf &
                                    " AND CAST(CheckIn AS DATE) BETWEEN " & ReadyString(fromdate) & " AND " & ReadyString(todate) & vbCrLf &
                                    " AND checkin.CheckInCustomerOrgID NOT IN (SELECT LocationID FROM LibraryExpensesRouteSchedules WHERE EmployeeID = " & ReadyString(employeeID) & ") " & vbCrLf &
                                    " AND checkin.CheckInCustomerOrgOutletID NOT IN (SELECT FowstID FROM LibraryExpensesRouteSchedules WHERE EmployeeID = " & ReadyString(employeeID) & ") " & vbCrLf &
                                    " GROUP BY location.CompanyName, location.PersonFirstName, location.PersonSurname, fowst.Name, checkin.CheckInCustomerOrgID, checkin.CheckInCustomerOrgOutletID " & vbCrLf &
                                    " -- kwaon ang free type exact location " & vbCrLf &
                                    " UNION ALL " & vbCrLf &
                                    " SELECT CheckInLocationName, NULL as LocationID, NULL as FowstID " & vbCrLf &
                                    " FROM TransExpensesCheckInOutDetails " & vbCrLf &
                                    " WHERE CheckInCustomerOrgID IS NULL AND CheckInCustomerOrgOutletID IS NULL " & vbCrLf &
                                    " AND PeopleID = " & ReadyString(employeeID) & vbCrLf &
                                    " AND CAST(CheckIn AS DATE) BETWEEN " & ReadyString(fromdate) & " AND " & ReadyString(todate) & vbCrLf &
                                    " GROUP BY CheckInLocationName, CheckInCustomerOrgID, CheckInCustomerOrgOutletID " & vbCrLf
                Dim locations As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each locs As Object In locations
                    Dim visitedclientloc As Boolean = False

                    If Not String.IsNullOrWhiteSpace(locs("LocationID")) And Not String.IsNullOrWhiteSpace(locs("FowstID")) Then
                        visitedclientloc = True

                        pSql = "SELECT CONVERT(varchar(15), StartTime, 100) as StartTime, CONVERT(varchar(15), EndTime, 100) as EndTime, Day, CASE WHEN Day = 0 THEN 'Mon' " & vbCrLf &
                            " WHEN Day = 1 THEN 'Tue' " & vbCrLf &
                            " WHEN Day = 2 THEN 'Wed' " & vbCrLf &
                            " WHEN Day = 3 THEN 'Thu' " & vbCrLf &
                            " WHEN Day = 4 THEN 'Fri' " & vbCrLf &
                            " WHEN Day = 5 THEN 'Sat' " & vbCrLf &
                            " WHEN Day = 6 THEN 'Sun' " & vbCrLf &
                            " WHEN Day IS NULL THEN 'Any day' " & vbCrLf &
                            " END as DayName, Week " & vbCrLf &
                            " FROM LibraryExpensesRouteSchedules " & vbCrLf &
                            " WHERE LocationID = " & ReadyString(locs("LocationID")) & vbCrLf &
                            " AND FowstID = " & ReadyString(locs("FowstID")) & vbCrLf &
                            " AND EmployeeID = " & ReadyString(employeeID) & vbCrLf &
                            " ORDER BY Week, Day ASC "

                        locs.Add("Schedule", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                    End If

                    pSql = GetCheckInOutDetailsByEmployeeQuery()

                    If visitedclientloc Then
                        pSql += " AND CheckInCustomerOrgID = " & ReadyString(locs("LocationID")) & vbCrLf &
                            " AND CheckInCustomerOrgOutletID = " & ReadyString(locs("FowstID")) & vbCrLf
                    Else
                        pSql += " AND CheckInLocationName = " & ReadyString(locs("CheckInLocationName"))
                    End If

                    locs.Add("VisitedLocations", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next

                Return Json(New With {Key .Details = locations})
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetCheckInOutDetailsByEmployee() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim IsCountPage As Boolean
                Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()

                Dim whereSearch As String = "" & vbCrLf
                '& " AND (" & vbCrLf &
                '"   (checked.CheckInCustomerOrgID IS NULL " & vbCrLf &
                '"   AND checked.CheckInCustomerOrgOutletID IS NULL) " & vbCrLf &
                '" OR " & vbCrLf &
                '"   (checked.CheckInCustomerOrgID NOT IN (SELECT LocationID FROM LibraryExpensesRouteSchedules WHERE EmployeeID = " & ReadyString(Request.Params.Get("PeopleID")) & ") " & vbCrLf &
                '"   AND " & vbCrLf &
                '"   checked.CheckInCustomerOrgOutletID NOT IN (SELECT LocationID FROM LibraryExpensesRouteSchedules WHERE EmployeeID = " & ReadyString(Request.Params.Get("PeopleID")) & ") " & vbCrLf &
                '" ))"
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " OR "
                        End If

                        whereSearchContent = ""
                    Next
                    whereSearch = " AND (" & whereSearchContent & ")"
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.LastUpdated " & PageColumnDirection & ", mainsql.EmployeeName " & PageColumnDirection
                    'ElseIf PageColumnNo = 2 Then
                    '    whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
                    'ElseIf PageColumnNo = 5 Then
                    '    whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
                End If

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        GetCheckInOutDetailsByEmployeeQuery() & vbCrLf &
                        whereSearch & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(GetCheckInOutDetailsByEmployeeQuery() & vbCrLf &
                        whereSearch & vbCrLf)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetCheckInOutDetailsByEmployeeQuery() As String
            Return " SELECT checked.*, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as EmployeeName, " & vbCrLf &
                        " logcheckin.ExpenseType as CheckInExpenseType, logcheckin.FromLocation as CheckInFromLocation, logcheckin.ToLocation as CheckInToLocation, logcheckin.Amount CheckInAmount, logcheckin.Remarks as CheckInExpenseRemarks, " & vbCrLf &
                        " logcheckout.ExpenseType as CheckOutExpenseType, logcheckout.FromLocation as CheckOutFromLocation, logcheckout.ToLocation as CheckOutToLocation, logcheckout.Amount CheckOutAmount, logcheckout.Remarks as CheckOutExpenseRemarks, " & vbCrLf &
                        " CASE WHEN ISNULL(CONVERT(varchar(max), checked.CheckInCustomerOrgID), '') = '' " & vbCrLf &
                        "   THEN checked.CheckInLocationName " & vbCrLf &
                        "   ELSE (SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = checked.CheckInCustomerOrgID) + ' - ' + (SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = checked.CheckInCustomerOrgOutletID) " & vbCrLf &
                        "  END as ExactCheckInLocation, " & vbCrLf &
                        " CASE WHEN ISNULL(CONVERT(varchar(max), checked.CheckOutCustomerOrgID), '') = '' " & vbCrLf &
                        "   THEN checked.CheckOutLocationName " & vbCrLf &
                        "   ELSE (SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = checked.CheckOutCustomerOrgID) + ' - ' + (SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = checked.CheckOutCustomerOrgOutletID) " & vbCrLf &
                        "  END as ExactCheckOutLocation, " & vbCrLf &
                        " CASE WHEN DATENAME(WEEKDAY, checked.CheckIn) = 'Monday' THEN 0 " & vbCrLf &
                        "   WHEN DATENAME(WEEKDAY, checked.CheckIn) = 'Tuesday' THEN 1 " & vbCrLf &
                        "   WHEN DATENAME(WEEKDAY, checked.CheckIn) = 'Wednesday' THEN 2 " & vbCrLf &
                        "   WHEN DATENAME(WEEKDAY, checked.CheckIn) = 'Thursday' THEN 3 " & vbCrLf &
                        "   WHEN DATENAME(WEEKDAY, checked.CheckIn) = 'Friday' THEN 4 " & vbCrLf &
                        "   WHEN DATENAME(WEEKDAY, checked.CheckIn) = 'Saturday' THEN 5 " & vbCrLf &
                        "   WHEN DATENAME(WEEKDAY, checked.CheckIn) = 'Sunday' THEN 6 " & vbCrLf &
                        "   ELSE 'Any Day' " & vbCrLf &
                        " END as Day " & vbCrLf &
                        " FROM TransExpensesCheckInOutDetails as checked " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = checked.PeopleID " & vbCrLf &
                        " LEFT JOIN TransExpensesLogDetails as logcheckin ON logcheckin.ID = checked.CheckInLogExpenseID AND ISNULL(logcheckin.IsDelete, 'False') = 'False'" & vbCrLf &
                        " LEFT JOIN TransExpensesLogDetails as logcheckout ON logcheckout.ID = checked.CheckOutLogExpenseID AND ISNULL(logcheckout.IsDelete, 'False') = 'False' " & vbCrLf &
                        " WHERE CAST(checked.CheckIn AS DATE) BETWEEN " & ReadyString(Request.Params.Get("FromDate")) & vbCrLf &
                        " AND " & ReadyString(Request.Params.Get("ToDate")) & vbCrLf &
                        IIf(Request.Params.Get("PeopleID") = "", "", " AND checked.PeopleID = " & ReadyString(Request.Params.Get("PeopleID"))) & vbCrLf
        End Function

        Function GetLogExpenseByMonth() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String

                pSql = " SELECT ISNULL(SUM(log.Amount), 0) as Total " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " WHERE DatePart(MM, ISNULL(log.ExpenseDate, log.DateCreated)) = DatePart(MM, getdate()) " & vbCrLf &
                        " AND DatePart(YY, ISNULL(log.ExpenseDate, log.DateCreated)) = DatePart(YY, getdate()) " & vbCrLf &
                        " AND ISNULL(log.IsDelete, 'False') = 'False' " & vbCrLf

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetFundByMonth() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String

                pSql = " SELECT ISNULL(SUM(Amount), 0) as Total " & vbCrLf &
                        " FROM TransExpensesReceiveFunds as log " & vbCrLf &
                        " WHERE DatePart(MM, ReceiveDate) = DatePart(MM, getdate()) " & vbCrLf &
                        " AND DatePart(YY, ReceiveDate) = DatePart(YY, getdate()) " & vbCrLf &
                        " AND ISNULL(IsDelete, 'False') = 'False' " & vbCrLf

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetExpenseDetailsForDownload() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pOut As New JsonDictionary

                pOut.Add("Header", Common.WebRequest.ProData.Execute(ExpenseHeaderQuery(Common.WebRequest.Data("PeopleID"), Common.WebRequest.Data("MonthCovered"), Common.WebRequest.Data("YearCovered"))))
                pOut.Add("Body", Common.WebRequest.ProData.Execute(ExpenseBodyQuery(Common.WebRequest.Data("PeopleID"), Common.WebRequest.Data("MonthCovered"), Common.WebRequest.Data("YearCovered"))))

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DownloadExpenseReports() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim pDtExcel As New DataTable
                Dim pHeader As New JsonDictionary
                Dim totalPerExpense As New JsonDictionary
                Dim totalExpense As Decimal = 0
                Dim ctrHeader = 7
                Dim fullname As String = "AllEmployees"
                Dim prevEmployee As String = ""

                If IsGuid(Request.Params.Get("PeopleID")) Then
                    pSql = "SELECT FirstName + '' + Surname as Fullname FROM MobizLibraryPeople WHERE ID = " & ReadyString(Request.Params.Get("PeopleID"))
                    fullname = "ExpenseReportOf_" & Common.WebRequest.ProData.Execute(pSql).Rows(0)("Fullname")
                Else
                    If Request.Params.Get("PeopleID") = "na" Then
                        fullname = "GeneralExpenses"
                    Else
                        fullname = "ExpenseReportOfAllEmployees"
                    End If
                End If

                pSql = ExpenseHeaderQuery(Request.Params.Get("PeopleID"), Request.Params.Get("FromDate"), Request.Params.Get("ToDate"))

                Dim header As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If header.Count = 0 Then
                    Return JsonDictionary.Serialize("Results", "No data to be downloaded")
                End If

                pDtExcel.Columns.Add("Date")
                pDtExcel.Columns.Add("Employee Name")
                pDtExcel.Columns.Add("GPS Location When Expense Was Logged")
                pDtExcel.Columns.Add("Related GPS Check-in Location")
                pDtExcel.Columns.Add("Related GPS Check-out Location")
                pDtExcel.Columns.Add("From")
                pDtExcel.Columns.Add("To")

                If Request.Params.Get("Format") = "separate" Then
                    For Each head As Object In header
                        pDtExcel.Columns.Add(head("ExpenseType"))
                        pHeader.Add(head("ExpenseType"), ctrHeader)
                        ctrHeader += 1
                    Next head
                Else
                    pDtExcel.Columns.Add("Expense Type")
                    pDtExcel.Columns.Add("Amount")
                    ctrHeader += 2
                End If

                pDtExcel.Columns.Add("Remarks")

                pSql = ExpenseBodyQuery(Request.Params.Get("PeopleID"), Request.Params.Get("FromDate"), Request.Params.Get("ToDate"))

                Dim body As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                Dim datarow As DataRow

                For Each b As Object In body
                    datarow = pDtExcel.NewRow

                    If CStr2(Request.Params.Get("PeopleID")) = "" Then
                        If prevEmployee <> "" And b("EmployeeName") <> prevEmployee Then
                            datarow(1) = "TOTAL EXPENSE"
                            If Request.Params.Get("Format") = "separate" Then
                                For Each head As Object In header
                                    If totalPerExpense.ContainsKey(head("ExpenseType")) Then
                                        datarow(pHeader(head("ExpenseType"))) = totalPerExpense(head("ExpenseType"))
                                    End If
                                Next head

                                totalPerExpense = New JsonDictionary()
                            Else
                                datarow(8) = CDec2(totalExpense)
                            End If

                            pDtExcel.Rows.Add(datarow)

                            datarow = pDtExcel.NewRow
                            totalExpense = 0
                        End If
                    End If

                    datarow(0) = Format(IIf(CStr2(b("ExpenseDate")) = "", b("DateCreated"), b("ExpenseDate")), "MM-dd-yyyy")
                    datarow(1) = b("EmployeeName")
                    datarow(2) = b("GPSLocation")
                    datarow(3) = b("CheckInGPSLocation")
                    datarow(4) = b("CheckOutGPSLocation")
                    datarow(5) = b("FromLocation")
                    datarow(6) = b("ToLocation")

                    If Request.Params.Get("Format") = "separate" Then
                        datarow(pHeader(b("ExpenseType"))) = b("Amount")

                        If totalPerExpense.ContainsKey(b("ExpenseType")) Then
                            totalPerExpense(b("ExpenseType")) = CDec2(totalPerExpense(b("ExpenseType"))) + CDec2(b("Amount"))
                        Else
                            totalPerExpense.Add(b("ExpenseType"), CDec2(b("Amount")))
                        End If
                    Else
                        datarow(7) = b("ExpenseType")
                        datarow(8) = b("Amount")
                        totalExpense += CDec2(b("Amount"))
                    End If

                    datarow(ctrHeader) = b("Remarks")
                    prevEmployee = b("EmployeeName").ToString()

                    pDtExcel.Rows.Add(datarow)
                Next b

                datarow = pDtExcel.NewRow
                datarow(1) = "TOTAL EXPENSE"

                If Request.Params.Get("Format") = "separate" Then
                    For Each head As Object In header
                        datarow(pHeader(head("ExpenseType"))) = totalPerExpense(head("ExpenseType"))
                    Next head
                Else
                    datarow(8) = CDec2(totalExpense)
                End If

                pDtExcel.Rows.Add(datarow)

                Dim mFilenameFolder = Common.ServerPath & "FileTemplates"

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                mFilenameFolder = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID)

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                Dim strFileName = "ExpenseReport_" & Request.Params.Get("PeopleID") & ".xlsx"

                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID)

                TableToExcelFile(pDtExcel, "", "", pSourceURL & "\" & strFileName)

                Dim file As New System.IO.FileInfo(pSourceURL & "\" & strFileName)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & fullname & "_from_" & Format(CDate(Request.Params.Get("FromDate")), "MM-dd-yyyy") & "_to_" & Format(CDate(Request.Params.Get("ToDate")), "MM-dd-yyyy") & ".xlsx")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DownloadFundReports() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim pDtExcel As New DataTable
                Dim pHeader As New JsonDictionary
                Dim totalPerExpense As New JsonDictionary
                Dim totalExpense As Decimal = 0
                Dim fullname As String = "AllEmployees"
                Dim prevEmployee As String = ""

                If Request.Params.Get("PeopleID") <> Nothing Then
                    pSql = "SELECT FirstName + '' + Surname as Fullname FROM MobizLibraryPeople WHERE ID = " & ReadyString(Request.Params.Get("PeopleID"))
                    fullname = Common.WebRequest.ProData.Execute(pSql).Rows(0)("Fullname")
                End If

                pSql = "SELECT fund.*, " & vbCrLf &
                        " CASE WHEN ISNULL(convert(varchar(MAX), fund.ReceiveFrom), '') = '' THEN fund.OtherPerson ELSE rcvfrom.FirstName + ' ' + rcvfrom.Surname END as ReceivedFromName, " & vbCrLf &
                        " ppol.Surname + ', ' + ppol.FirstName  as EmployeeName " & vbCrLf &
                        " FROM TransExpensesReceiveFunds as fund" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as rcvfrom ON rcvfrom.ID = fund.ReceiveFrom " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = fund.PeopleID " & vbCrLf &
                        " WHERE fund.ReceiveDate BETWEEN " & ReadyString(Request.Params.Get("FromDate")) & vbCrLf &
                        " AND " & ReadyString(Request.Params.Get("ToDate")) & vbCrLf &
                        " AND ISNULL(fund.IsDelete, 'False') = 'False'" & vbCrLf &
                        IIf(CStr2(Request.Params.Get("PeopleID")) = "", "", " AND fund.PeopleID = " & ReadyString(Request.Params.Get("PeopleID"))) & vbCrLf &
                        " ORDER BY fund.ReceiveDate, ppol.Surname ASC "

                Dim header As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If header.Count = 0 Then
                    Return JsonDictionary.Serialize("Results", "No data to be downloaded")
                End If

                pDtExcel.Columns.Add("Received Date")
                pDtExcel.Columns.Add("Employee Name")
                pDtExcel.Columns.Add("Received From")
                pDtExcel.Columns.Add("Amount")
                pDtExcel.Columns.Add("Comment")

                Dim datarow As DataRow

                For Each b As Object In header
                    datarow = pDtExcel.NewRow

                    If CStr2(Request.Params.Get("PeopleID")) = "" Then
                        If prevEmployee <> "" And b("EmployeeName") <> prevEmployee Then
                            datarow(1) = "TOTAL RECEIVED FUND"
                            datarow(3) = CDec2(totalExpense)

                            pDtExcel.Rows.Add(datarow)

                            datarow = pDtExcel.NewRow
                            totalExpense = 0
                        End If
                    End If

                    datarow(0) = Format(b("ReceiveDate"), "MM-dd-yyyy")
                    datarow(1) = b("EmployeeName")
                    datarow(2) = b("ReceivedFromName")
                    datarow(3) = Format(CDec2(b("Amount")), "0,0.00")
                    datarow(4) = b("Comment")

                    totalExpense += CDec2(b("Amount"))
                    prevEmployee = b("EmployeeName")

                    pDtExcel.Rows.Add(datarow)
                Next b

                datarow = pDtExcel.NewRow
                datarow(1) = "TOTAL RECEIVED FUND"
                datarow(3) = Format(CDec2(totalExpense), "0,0.00")

                pDtExcel.Rows.Add(datarow)

                Dim mFilenameFolder = Common.ServerPath & "FileTemplates"

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                mFilenameFolder = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID)

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                Dim strFileName = "ReceivedFundReport_" & Request.Params.Get("PeopleID") & ".xlsx"

                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID)

                TableToExcelFile(pDtExcel, "", "", pSourceURL & "\" & strFileName)

                Dim file As New System.IO.FileInfo(pSourceURL & "\" & strFileName)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=ReceivedFundReportOf_" & fullname & "_from_" & Format(CDate(Request.Params.Get("FromDate")), "MM-dd-yyyy") & "_to_" & Format(CDate(Request.Params.Get("ToDate")), "MM-dd-yyyy") & ".xlsx")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DownloadCheckinReports() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim pDtExcel As New DataTable
                Dim pHeader As New JsonDictionary
                Dim totalPerExpense As New JsonDictionary
                Dim totalExpense As Decimal = 0
                Dim fullname As String = "AllEmployees"
                Dim prevEmployee As String = ""
                Dim pWB As Aspose.Cells.Workbook
                Dim pWS As Aspose.Cells.Worksheet

                pWB = New Aspose.Cells.Workbook
                pWS = pWB.Worksheets(0)

                If Request.Params.Get("PeopleID") <> Nothing Then
                    pSql = "SELECT FirstName + '' + Surname as Fullname FROM MobizLibraryPeople WHERE ID = " & ReadyString(Request.Params.Get("PeopleID"))
                    fullname = Common.WebRequest.ProData.Execute(pSql).Rows(0)("Fullname")
                End If

                pSql = "SELECT checks.*, ppol.Surname + ', ' + ppol.FirstName as EmployeeName, " & vbCrLf &
                        " logcheckin.Amount as CheckinAmount, logcheckin.FromLocation as CheckinFromLocation, logcheckin.ToLocation as CheckinToLocation, " & vbCrLf &
                        " logcheckout.Amount as CheckoutAmount, logcheckout.FromLocation as CheckoutFromLocation, logcheckout.ToLocation as CheckoutToLocation, " & vbCrLf &
                        " CASE WHEN ISNULL(CONVERT(varchar(max), checks.CheckInCustomerOrgID), '') = '' " & vbCrLf &
                        "   THEN checks.CheckInLocationName " & vbCrLf &
                        "   ELSE (SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = checks.CheckInCustomerOrgID) + ' - ' + (SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = checks.CheckInCustomerOrgOutletID) " & vbCrLf &
                        "  END as ExactCheckInLocation, " & vbCrLf &
                        " CASE WHEN ISNULL(CONVERT(varchar(max), checks.CheckOutCustomerOrgID), '') = '' " & vbCrLf &
                        "   THEN checks.CheckOutLocationName " & vbCrLf &
                        "   ELSE (SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = checks.CheckOutCustomerOrgID) + ' - ' + (SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = checks.CheckOutCustomerOrgOutletID) " & vbCrLf &
                        "  END as ExactCheckOutLocation " & vbCrLf &
                        " FROM TransExpensesCheckInOutDetails as checks " & vbCrLf &
                        " LEFT JOIN TransExpensesLogDetails as logcheckin ON logcheckin.ID = checks.CheckInLogExpenseID " & vbCrLf &
                        " LEFT JOIN TransExpensesLogDetails as logcheckout ON logcheckout.ID = checks.CheckOutLogExpenseID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = checks.PeopleID " & vbCrLf &
                        " WHERE CAST(checks.CheckIn AS DATE) BETWEEN " & ReadyString(Request.Params.Get("FromDate")) & vbCrLf &
                        " AND " & ReadyString(Request.Params.Get("ToDate")) & vbCrLf &
                        IIf(CStr2(Request.Params.Get("PeopleID")) = "", "", " AND checks.PeopleID = " & ReadyString(Request.Params.Get("PeopleID"))) & vbCrLf &
                        " ORDER BY checks.CheckIn, ppol.Surname ASC "

                Dim header As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If header.Count = 0 Then
                    Return JsonDictionary.Serialize("Results", "No data to be downloaded")
                End If

                pWS.Cells(0, 0).PutValue("Employee Name")
                pWS.Cells(0, 0).Style.Font.Size = 11
                pWS.Cells(0, 0).Style.Font.IsBold = True
                pWS.Cells(0, 1).PutValue("Checked-in Date")
                pWS.Cells(0, 1).Style.Font.Size = 11
                pWS.Cells(0, 1).Style.Font.IsBold = True
                pWS.Cells(0, 2).PutValue("Checked-in Location")
                pWS.Cells(0, 2).Style.Font.Size = 11
                pWS.Cells(0, 2).Style.Font.IsBold = True
                pWS.Cells(0, 3).PutValue("Checked-in GPS")
                pWS.Cells(0, 3).Style.Font.Size = 11
                pWS.Cells(0, 3).Style.Font.IsBold = True
                pWS.Cells(0, 4).PutValue("Checked-out Date")
                pWS.Cells(0, 4).Style.Font.Size = 11
                pWS.Cells(0, 4).Style.Font.IsBold = True
                pWS.Cells(0, 5).PutValue("Checked-out Location")
                pWS.Cells(0, 5).Style.Font.Size = 11
                pWS.Cells(0, 5).Style.Font.IsBold = True
                pWS.Cells(0, 6).PutValue("Checked-out GPS")
                pWS.Cells(0, 6).Style.Font.Size = 11
                pWS.Cells(0, 6).Style.Font.IsBold = True
                pWS.Cells(0, 7).PutValue("Duration")
                pWS.Cells(0, 7).Style.Font.Size = 11
                pWS.Cells(0, 7).Style.Font.IsBold = True
                pWS.Cells(0, 8).PutValue("Remarks")
                pWS.Cells(0, 8).Style.Font.Size = 11
                pWS.Cells(0, 8).Style.Font.IsBold = True

                pWS.Cells.Rows(0).Style.Font.Size = 18
                pWS.Cells.Rows(0).Style.Font.IsBold = True

                Dim datarow As DataRow
                Dim ctrRow As Integer = 1
                For Each b As Object In header
                    datarow = pDtExcel.NewRow

                    If CStr2(Request.Params.Get("PeopleID")) = "" Then
                        If prevEmployee <> "" And b("EmployeeName") <> prevEmployee Then
                            datarow(1) = ""

                            pDtExcel.Rows.Add(datarow)

                            datarow = pDtExcel.NewRow
                        End If
                    End If

                    pWS.Cells(ctrRow, 0).PutValue(b("EmployeeName"))
                    'datarow(0) = b("EmployeeName")

                    If CStr2(b("CheckIn")) <> "" Then
                        pWS.Cells(ctrRow, 1).PutValue(Format(b("CheckIn"), "MM-dd-yyyy h:mm tt"))
                        pWS.Cells(ctrRow, 2).PutValue(b("ExactCheckInLocation"))
                        pWS.Cells(ctrRow, 3).PutValue(b("CheckInGPSLocation"))
                        'datarow(1) = Format(b("CheckIn"), "MM-dd-yyyy h:mm tt")
                        'datarow(2) = b("ExactCheckInLocation")
                        'datarow(3) = b("CheckInGPSLocation")
                    End If

                    If CStr2(b("CheckOut")) <> "" Then
                        pWS.Cells(ctrRow, 4).PutValue(Format(b("CheckOut"), "MM-dd-yyyy h:mm tt"))
                        pWS.Cells(ctrRow, 5).PutValue(b("ExactCheckOutLocation"))
                        pWS.Cells(ctrRow, 6).PutValue(b("CheckOutGPSLocation"))
                        'datarow(4) = Format(b("CheckOut"), "MM-dd-yyyy h:mm tt")
                        'datarow(5) = b("ExactCheckOutLocation")
                        'datarow(6) = b("CheckOutGPSLocation")
                    End If

                    If CStr2(b("CheckIn")) <> "" And CStr2(b("CheckOut")) <> "" Then
                        Dim diff As TimeSpan = Convert.ToDateTime(b("CheckOut")) - Convert.ToDateTime(b("CheckIn"))
                        Dim duration As String = ""

                        If diff.Days > 0 Then
                            duration += diff.Days & " day(s) & "
                        End If

                        If diff.Hours > 0 Then
                            duration += diff.Hours & " hour(s) & "
                        End If

                        If diff.Minutes > 0 Then
                            duration += diff.Minutes & " min(s) "
                        End If

                        pWS.Cells(ctrRow, 7).PutValue(duration)
                        'datarow(7) = duration
                    End If

                    pWS.Cells(ctrRow, 7).PutValue(b("Remarks"))
                    'datarow(8) = b("Remarks")
                    prevEmployee = b("EmployeeName")

                    'pDtExcel.Rows.Add(datarow)
                    ctrRow += 1
                Next b

                pWS.AutoFitColumns()
                pWS.AutoFitRows()

                Dim filename As String = "CheckedInReportOf_" & fullname & "_from_" & Format(CDate(Request.Params.Get("FromDate")), "MM-dd-yyyy") & "_to_" & Format(CDate(Request.Params.Get("ToDate")), "MM-dd-yyyy") & ".xlsx"
                Dim filesource = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\" & filename

                pWB.Save(filesource)

                Dim file As New System.IO.FileInfo(filesource)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & filename)
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                Return JsonDictionary.Serialize("success", filename)

                Dim mFilenameFolder = Common.ServerPath & "FileTemplates"

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                mFilenameFolder = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID)

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                Dim strFileName = "CheckedInReport_" & Request.Params.Get("PeopleID") & ".xlsx"

                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID)

                TableToExcelFile(pDtExcel, "", "", pSourceURL & "\" & strFileName)

                'Dim file As New System.IO.FileInfo(pSourceURL & "\" & strFileName)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=CheckedInReportOf_" & fullname & "_from_" & Format(CDate(Request.Params.Get("FromDate")), "MM-dd-yyyy") & "_to_" & Format(CDate(Request.Params.Get("ToDate")), "MM-dd-yyyy") & ".xlsx")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function ExpenseBodyQuery(ByVal PeopleID As String, ByVal FromDate As String, ByVal ToDate As String) As String
            Return " SELECT log.*, ppol.Surname + ', ' + ppol.FirstName as EmployeeName, " & vbCrLf &
                    " (SELECT CheckInGPSLocation FROM TransExpensesCheckInOutDetails WHERE CheckInLogExpenseID = log.ID) as CheckInGPSLocation, " & vbCrLf &
                    " (SELECT CheckOutGPSLocation FROM TransExpensesCheckInOutDetails WHERE CheckOutLogExpenseID = log.ID) as CheckOutGPSLocation " & vbCrLf &
                    " FROM TransExpensesLogDetails as log " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = log.PeopleID " & vbCrLf &
                    " WHERE ISNULL(log.ExpenseDate, log.DateCreated) BETWEEN " & ReadyString(FromDate) & vbCrLf &
                    " AND " & ReadyString(ToDate) & vbCrLf &
                    " AND ISNULL(log.IsDelete, 'False') = 'False'" & vbCrLf &
                    IIf(Not IsGuid(PeopleID), IIf(PeopleID = "all", " AND ISNULL(CONVERT(varchar(max), log.PeopleID), '') <> '' ", " AND ISNULL(CONVERT(varchar(max), log.PeopleID), '') = '' "), " AND log.PeopleID = " & ReadyString(PeopleID)) & vbCrLf &
                    " ORDER BY ppol.Surname ASC, ISNULL(log.ExpenseDate, log.DateCreated) ASC "
        End Function

        Function ExpenseHeaderQuery(ByVal PeopleID As String, ByVal FromDate As String, ByVal ToDate As String) As String
            Return " SELECT ExpenseType " & vbCrLf &
                        " FROM TransExpensesLogDetails as log " & vbCrLf &
                        " WHERE ISNULL(log.ExpenseDate, log.DateCreated) BETWEEN " & ReadyString(FromDate) & vbCrLf &
                        " AND " & ReadyString(ToDate) & vbCrLf &
                        " AND ISNULL(log.IsDelete, 'False') = 'False'" & vbCrLf &
                        IIf(Not IsGuid(PeopleID), IIf(PeopleID = "all", " AND ISNULL(CONVERT(varchar(max), log.PeopleID), '') <> '' ", " AND ISNULL(CONVERT(varchar(max), log.PeopleID), '') = '' "), " AND log.PeopleID = " & ReadyString(PeopleID)) & vbCrLf &
                        " GROUP BY ExpenseType " & vbCrLf &
                        " ORDER BY ExpenseType ASC "
        End Function

        Function DownloadStoreVisitedReports()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim startdate As Date = CDate(data("Start"))
                Dim enddate As Date = CDate(data("End"))
                Dim pSql As String
                Dim pWB As Aspose.Cells.Workbook
                Dim pWS As Aspose.Cells.Worksheet

                pWB = New Aspose.Cells.Workbook
                pWS = pWB.Worksheets(0)

                Dim rowctr As Integer = 0

                For Each emp As String In data("Employees")
                    pSql = "SELECT Surname + ', ' + FirstName + ' ' + MiddleName as EmployeeName " & vbCrLf &
                            " FROM MobizLibraryPeople " & vbCrLf &
                            " WHERE ID = " & ReadyString(emp)
                    Dim empname As String = Trim(Common.WebRequest.ProData.Execute(pSql).Rows(0)("EmployeeName"))

                    pWS.Cells(rowctr, 0).PutValue(empname)
                    pWS.Cells(rowctr, 0).Style.ForegroundColor = Color.FromArgb(155, 187, 89)
                    pWS.Cells(rowctr, 0).Style.Pattern = BackgroundType.Solid
                    pWS.Cells(rowctr, 0).Style.Font.Size = 11
                    pWS.Cells(rowctr, 0).Style.Font.IsBold = True

                    Dim monthctr As Integer = 1
                    While startdate <= enddate
                        Dim monthlabel As String

                        If CDate(data("Start")).Year = CDate(data("End")).Year Then
                            monthlabel = Format(startdate, "MMMM")
                        Else
                            monthlabel = Format(startdate, "MMMM yyyy")
                        End If

                        pWS.Cells(rowctr, monthctr).PutValue(monthlabel)
                        pWS.Cells(rowctr, monthctr).Style.HorizontalAlignment = TextAlignmentType.Center
                        pWS.Cells(rowctr, monthctr).Style.ForegroundColor = Color.FromArgb(155, 187, 89)
                        pWS.Cells(rowctr, monthctr).Style.Pattern = BackgroundType.Solid
                        pWS.Cells(rowctr, monthctr).Style.Font.Size = 11
                        pWS.Cells(rowctr, monthctr).Style.Font.IsBold = True
                        startdate = startdate.AddMonths(1)
                        monthctr += 1
                    End While

                    rowctr += 1
                    startdate = CDate(data("Start"))

                    pSql = " SELECT DISTINCT " & vbCrLf &
                            " CheckInCustomerOrgID, " & vbCrLf &
                            " CheckInCustomerOrgOutletID, " & vbCrLf &
                            " CASE WHEN ISNULL(CONVERT(varchar(max), CheckInCustomerOrgID), '') = '' " & vbCrLf &
                            "   THEN CheckInLocationName " & vbCrLf &
                            "   ELSE (SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = CheckInCustomerOrgID) + ' - ' + (SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = CheckInCustomerOrgOutletID)" & vbCrLf &
                            "  END as CompanyName, " & vbCrLf &
                            " CASE WHEN ISNULL(CONVERT(varchar(max), CheckInCustomerOrgID), '') = '' THEN 0 ELSE 1 END IsUserDefinedLocation " & vbCrLf &
                            " FROM TransExpensesCheckInOutDetails" & vbCrLf &
                            " WHERE PeopleID = " & ReadyString(emp) & vbCrLf &
                            " AND convert(varchar(7), CheckIn, 126) BETWEEN " & ReadyString(Format(startdate, "yyyy-MM")) & " AND " & ReadyString(Format(enddate, "yyyy-MM")) & vbCrLf &
                            " ORDER BY CompanyName " & vbCrLf

                    Dim empvisitedloc As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    If empvisitedloc.Count > 0 Then
                        For Each loc As Object In empvisitedloc
                            pWS.Cells(rowctr, 0).PutValue(loc("CompanyName"))

                            Dim visitctr As Integer = 0

                            monthctr = 1
                            While startdate <= enddate
                                If CBool(loc("IsUserDefinedLocation")) Then
                                    pSql = "SELECT COUNT(ID) as CNT FROM TransExpensesCheckInOutDetails " & vbCrLf &
                                         " WHERE CheckInCustomerOrgID = " & ReadyString(loc("CheckInCustomerOrgID").ToString()) & vbCrLf &
                                         " AND CheckInCustomerOrgOutletID = " & ReadyString(loc("CheckInCustomerOrgOutletID").ToString()) &
                                         " AND PeopleID = " & ReadyString(emp) & vbCrLf &
                                         " AND convert(varchar(7), CheckIn, 126) = " & ReadyString(Format(startdate, "yyyy-MM"))
                                Else
                                    pSql = "SELECT COUNT(ID) as CNT FROM TransExpensesCheckInOutDetails " & vbCrLf &
                                         " WHERE CheckInLocationName = " & ReadyString(loc("CompanyName").ToString()) & vbCrLf &
                                         " AND PeopleID = " & ReadyString(emp) & vbCrLf &
                                         " AND convert(varchar(7), CheckIn, 126) = " & ReadyString(Format(startdate, "yyyy-MM"))
                                End If

                                visitctr = Common.WebRequest.ProData.Execute(pSql).Rows(0)("CNT")

                                If visitctr > 0 Then
                                    pWS.Cells(rowctr, monthctr).PutValue(visitctr)
                                    pWS.Cells(rowctr, monthctr).Style.HorizontalAlignment = TextAlignmentType.Center
                                End If

                                startdate = startdate.AddMonths(1)
                                monthctr += 1
                            End While
                            startdate = CDate(data("Start"))
                            rowctr += 1
                        Next
                    Else
                        pWS.Cells(rowctr, 0).PutValue("No visited places")
                        rowctr += 1
                    End If

                    startdate = CDate(data("Start"))
                Next emp

                pWS.AutoFitColumns()
                pWS.AutoFitRows()

                Dim filename As String = "storevistedreport_" & Format(startdate, "MMM-yyyy") & "-to-" & Format(enddate, "MMM-yyyy") & ".xlsx"
                Dim filesource = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\" & filename

                pWB.Save(filesource)

                Return JsonDictionary.Serialize("success", filename)
            Catch ex As Exception
                Return JsonDictionary.Serialize("error", ex.Message)
            End Try
        End Function

        Function DownloadSVRNow()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim filename As String = Request.Params.Get("name")
            Dim file As New System.IO.FileInfo(Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\" & filename)

            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" & filename)
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
            Response.ContentType = "application/octet-stream"
            Response.WriteFile(file.FullName)
            Response.End()
        End Function
    End Class
End Namespace