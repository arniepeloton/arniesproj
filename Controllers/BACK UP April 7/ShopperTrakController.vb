Namespace WebProj
    Public Class ShopperTrakController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /ShopperTrak

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function ShopperTrakSalesReport() As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim SqlWhere As String
            Dim Fowst As String()
            Dim CtrFowst As Integer = 1

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("Account_ID", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.ShopperTrakAccountID"
                    End With

                    With .Add("InvoiceDate", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "replace(CONVERT(VARCHAR(10),dbo.TransInvoice.InvoiceDate,21),'-','')"
                    End With

                    With .Add("FullInvoiceDate", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "dbo.TransInvoice.InvoiceDate"
                    End With

                    With .Add("Sales", SqlDbType.VarChar)
                        .Expression = "SUM(transinvoiceitems.SubTotal) - dbo.TransInvoice.DiscountAmount"
                    End With

                    With .Add("Category", SqlDbType.VarChar)
                        .Expression = "''"
                    End With

                    With .Add("Currency", SqlDbType.VarChar)
                        .Expression = "'PHP'"
                    End With

                End With

                '.SqlFrom = "FROM dbo.TransInvoice INNER JOIN " &
                '            "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " &
                '            "INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = TransInvoice.ProductID "
                .SqlFrom = "FROM dbo.TransInvoice INNER JOIN MobizLibraryOrganizationFOWST ON " &
                          "dbo.TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID INNER JOIN " &
                          "TransInvoiceItems on transinvoice.ID = TransInvoiceItems.InvoiceID inner join " &
                          "LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = TransInvoiceItems.ProductID"
                '.SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND " &
                '            "CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " &
                '            ReadyString(WebRequest.Instance.Data("To")) & " AND IsIncludeToShopperTrakReport = 1 AND dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0"

                .SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND " &
                            "CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " &
                            ReadyString(WebRequest.Instance.Data("To")) & " AND IsIncludeToShopperTrakReport = 1 AND dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 "

                .SqlGroup = "GROUP by MobizLibraryOrganizationFOWST.ShopperTrakAccountID, dbo.TransInvoice.InvoiceDate, dbo.TransInvoice.DiscountAmount"

                .SqlOrder = "ORDER BY dbo.TransInvoice.InvoiceDate"

                pDT = .Requery

                Dim DaysApart As Integer

                DaysApart = DateDiff(DateInterval.Day, CDate(WebRequest.Instance.Data("From")), CDate(WebRequest.Instance.Data("To")))

                Dim CurrentDate As Date = CDate(WebRequest.Instance.Data("From"))
                Dim i As Integer
                Dim sb As StringBuilder = New StringBuilder()

                While DateDiff(DateInterval.Day, CurrentDate, CDate(WebRequest.Instance.Data("To"))) >= 0
                    i = 0
                    While i < 23
                        pDT.DefaultView.RowFilter = "FullInvoiceDate >= '" & Format(CurrentDate, "yyyy-MM-dd " & i & ":00:01") & "' AND FullInvoiceDate <= '" & Format(CurrentDate, "yyyy-MM-dd " & i + 1 & ":00:00") & "'"
                        Dim dt As DataTable = pDT.DefaultView.ToTable
                        Dim j As Integer = 0
                        Dim TotalSales As Double = 0.0

                        If dt.Rows.Count > 0 Then
                            While j < dt.Rows.Count
                                TotalSales = TotalSales + dt.Rows(j).Item("Sales")
                                j += 1
                            End While
                            sb.Append(dt.Rows(0).Item("Account_ID") & "," & dt.Rows(0).Item("InvoiceDate") & "," & Format(i + 1, "##") & "0000," & Format(TotalSales, "#.00") & "," & dt.Rows.Count & ",,PHP")
                            sb.AppendLine()
                        End If
                        i += 1
                    End While

                    CurrentDate = DateAdd(DateInterval.Day, 1, CurrentDate)
                End While

                FileText(Common.ServerPath & "temp\SALES_" & Format(CDate(WebRequest.Instance.Data("To")), "yyyyMMdd") & ".txt") = sb.ToString

                Return JsonDictionary.Serialize("Report", "SALES_" & Format(CDate(WebRequest.Instance.Data("To")), "yyyyMMdd") & ".txt")
            End With
        End Function

        <HttpPost()>
        Function ShopperTrakReportLaborData() As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim SqlWhere As String
            Dim Fowst As String()
            Dim CtrFowst As Integer = 1

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("Account_ID", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.ShopperTrakAccountID"
                    End With
                    .Add("PeopleNo", SqlDbType.Int)
                    .Add("MondayFrom", SqlDbType.DateTime2)
                    .Add("MondayTo", SqlDbType.DateTime2)
                    .Add("MondayLunchBreakFrom", SqlDbType.DateTime2)
                    .Add("MondayLunchBreakTo", SqlDbType.DateTime2)
                    .Add("MondayDinnerBreakFrom", SqlDbType.DateTime2)
                    .Add("MondayDinnerBreakTo", SqlDbType.DateTime2)
                    .Add("TuesdayFrom", SqlDbType.DateTime2)
                    .Add("TuesdayTo", SqlDbType.DateTime2)
                    .Add("TuesdayLunchBreakFrom", SqlDbType.DateTime2)
                    .Add("TuesdayLunchBreakTo", SqlDbType.DateTime2)
                    .Add("TuesdayDinnerBreakFrom", SqlDbType.DateTime2)
                    .Add("TuesdayDinnerBreakTo", SqlDbType.DateTime2)
                    .Add("WednesdayFrom", SqlDbType.DateTime2)
                    .Add("WednesdayTo", SqlDbType.DateTime2)
                    .Add("WednesdayLunchBreakFrom", SqlDbType.DateTime2)
                    .Add("WednesdayLunchBreakTo", SqlDbType.DateTime2)
                    .Add("WednesdayDinnerBreakFrom", SqlDbType.DateTime2)
                    .Add("WednesdayDinnerBreakTo", SqlDbType.DateTime2)
                    .Add("ThursdayFrom", SqlDbType.DateTime2)
                    .Add("ThursdayTo", SqlDbType.DateTime2)
                    .Add("ThursdayLunchBreakFrom", SqlDbType.DateTime2)
                    .Add("ThursdayLunchBreakTo", SqlDbType.DateTime2)
                    .Add("ThursdayDinnerBreakFrom", SqlDbType.DateTime2)
                    .Add("ThursdayDinnerBreakTo", SqlDbType.DateTime2)
                    .Add("FridayFrom", SqlDbType.DateTime2)
                    .Add("FridayTo", SqlDbType.DateTime2)
                    .Add("FridayLunchBreakFrom", SqlDbType.DateTime2)
                    .Add("FridayLunchBreakTo", SqlDbType.DateTime2)
                    .Add("FridayDinnerBreakFrom", SqlDbType.DateTime2)
                    .Add("FridayDinnerBreakTo", SqlDbType.DateTime2)
                    .Add("SaturdayFrom", SqlDbType.DateTime2)
                    .Add("SaturdayTo", SqlDbType.DateTime2)
                    .Add("SaturdayLunchBreakFrom", SqlDbType.DateTime2)
                    .Add("SaturdayLunchBreakTo", SqlDbType.DateTime2)
                    .Add("SaturdayDinnerBreakFrom", SqlDbType.DateTime2)
                    .Add("SaturdayDinnerBreakTo", SqlDbType.DateTime2)
                    .Add("SundayFrom", SqlDbType.DateTime2)
                    .Add("SundayTo", SqlDbType.DateTime2)
                    .Add("SundayLunchBreakFrom", SqlDbType.DateTime2)
                    .Add("SundayLunchBreakTo", SqlDbType.DateTime2)
                    .Add("SundayDinnerBreakFrom", SqlDbType.DateTime2)
                    .Add("SundayDinnerBreakTo", SqlDbType.DateTime2)
            End With

                .SqlFrom = "FROM dbo.LibraryOrganizationWorkSchedule INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationWorkScheduleEmployee ON  " & vbCrLf &
                      "dbo.LibraryOrganizationWorkSchedule.ID = dbo.LibraryOrganizationWorkScheduleEmployee.WorkScheduleID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.LibraryOrganizationWorkScheduleEmployee.FowstID = dbo.MobizLibraryOrganizationFOWST.ID INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationEmployee ON dbo.LibraryOrganizationWorkScheduleEmployee.PeopleID = dbo.LibraryOrganizationEmployee.ID"

                .SqlWhere = "WHERE IsIncludeToShopperTrakReport = 1  AND (dbo.LibraryOrganizationWorkSchedule.IsActive = 1)"

            pDT = .Requery
            End With

            Dim CurrentDate As Date = CDate(WebRequest.Instance.Data("From"))
            Dim i As Integer
            Dim sb As StringBuilder = New StringBuilder()
            Dim m_DayOfWeek As String

            While DateDiff(DateInterval.Day, CurrentDate, CDate(WebRequest.Instance.Data("To"))) >= 0

                    Select Case CInt(DateTime.Parse(CurrentDate).DayOfWeek)
                    Case 0
                        m_DayOfWeek = "Monday"
                        Case 1
                        m_DayOfWeek = "Tuesday"
                        Case 2
                        m_DayOfWeek = "Wednesday"
                        Case 3
                        m_DayOfWeek = "Thursday"
                        Case 4
                        m_DayOfWeek = "Friday"
                        Case 5
                        m_DayOfWeek = "Saturday"
                        Case 6
                        m_DayOfWeek = "Sunday"
                    End Select

                pDT.DefaultView.RowFilter = m_DayOfWeek & "FROM IS NOT NULL "

                    Dim dt As DataTable = pDT.DefaultView.ToTable
                    Dim j As Integer = 0


                If dt.Rows.Count > 0 Then
                    For j = 0 To dt.Rows.Count - 1
                        If IsDate(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakFrom")) Then
                            sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "FROM"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakFrom"), "HHmmss"))
                            sb.AppendLine()
                            sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakTo"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "To"), "HHmmss"))
                        End If
                        If IsDate(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakFrom")) Then
                            sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "FROM"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakFrom"), "HHmmss"))
                            sb.AppendLine()
                            sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakTo"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "To"), "HHmmss"))
                        End If
                        sb.AppendLine()
                    Next
                End If
                CurrentDate = DateAdd(DateInterval.Day, 1, CurrentDate)
            End While

            FileText(Common.ServerPath & "temp\LABOR_" & Format(CDate(WebRequest.Instance.Data("To")), "yyyyMMdd") & ".txt") = sb.ToString

            Return JsonDictionary.Serialize("Report", "LABOR_" & Format(CDate(WebRequest.Instance.Data("To")), "yyyyMMdd") & ".txt")
        End Function

        Function DownloadShopperTrakReport() As ActionResult

            Dim pFileTitle = Common.WebRequest.QueryString("p")
            Dim pSourceURL As String = Common.ServerPath & "temp\" & pFileTitle
            Dim pContentType As String = "application/txt"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function
    End Class
End Namespace