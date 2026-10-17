Namespace WebProj
    Public Class TicketController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function ReportAnIssue() As ActionResult
            Return View()
        End Function

        Function TagAsNotified()
            Dim p As New InsertUpdate("TransMobizIssues")

            p("Notify") = 0

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ID IN (" & Common.WebRequest.Data("IDs") & ")"))

            Return JsonDictionary.Serialize("Results", "ok")
        End Function

        Function UpdateRAI()
            Try
                Dim transid As String = Common.WebRequest.Data("TransID")
                Dim status As String = Common.WebRequest.Data("Status")
                Dim updatedby As String = Common.WebRequest.Data("UpdatedBy")
                Dim dborgid, dbname As String

                If status <> "Unresolved" Then
                    Dim p As New InsertUpdate("TransMobizIssues")
                    p("Status") = ReadyString(status)
                    p("LastUpdated") = "getdate()"
                    p("UpdatedBy") = ReadyString(updatedby)

                    Common.WebRequest.ProData.BeginTransaction()
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(transid)))

#If DEBUG Then
                    'MobizABC dev db
                    dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
                    dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
                    '1Mobiz prod db 
                    dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
                    dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If
                    Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(dbname))

                    p = New InsertUpdate("TransCRM")
                    p("Status") = ReadyString(status)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(transid)))

                    Common.WebRequest.ProData.CommitTransaction()
                End If

                Return JsonDictionary.Serialize("Results", "updated")

            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetRAI()
            Try
                Dim pSql As String
                Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim whereSearchContent As String = ""
                Dim operators As String = ""
                Dim arr As Object = Split(keyword, "+")
                Dim status As String = Request.Params.Get("Status")

                Dim mainquery As String = " SELECT iss.*, ROW_NUMBER() OVER (ORDER BY iss.DateReported DESC) AS RowNum, " & vbCrLf &
                        " fowst.Name as FowstName, " & vbCrLf &
                        " org.CompanyName as CustomerName, " & vbCrLf &
                        " reportedby.FirstName + ' ' + reportedby.Surname as ReportedByName, " & vbCrLf &
                        " updatedby.FirstName + ' ' + updatedby.Surname as UpdatedByName " & vbCrLf &
                        " FROM TransMobizIssues as iss " & vbCrLf &
                        " INNER JOIN LibraryOrganization as org ON org.ID = iss.OrgID " & vbCrLf &
                        " INNER JOIN LibraryOrganizationFOWST as fowst ON fowst.ID = iss.FowstID " & vbCrLf &
                        " INNER JOIN LibraryPeople as reportedby ON reportedby.ID = iss.ReportedBy " & vbCrLf &
                        " LEFT JOIN LibraryPeople as updatedby ON updatedby.ID = iss.UpdatedBy " & vbCrLf

                If status = "Unresolved" Then
                    mainquery &= " WHERE Status = 'Unresolved' "
                ElseIf status = "Resolved" Then
                    mainquery &= " WHERE Status = 'Resolved' "
                ElseIf status = "Cancelled" Then
                    mainquery &= " WHERE Status = 'Cancelled' "
                End If

                If CStr2(keyword) <> "" Then
                    mainquery += " AND ("
                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " iss.TicketNo + ' ' + " & vbCrLf &
                             " ISNULL(org.CompanyName, '') + ' ' + " & vbCrLf &
                             " ISNULL(reportedby.FirstName, '') + ' ' + " & vbCrLf &
                             " ISNULL(reportedby.Surname, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next

                    mainquery += whereSearchContent & ") "
                End If

                pSql = "SELECT * FROM (" & vbCrLf &
                                mainquery & vbCrLf &
                                " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                                " ORDER BY DateReported DESC"

                Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
                'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function CountNewReports()
            Try
                Dim pSql As String = "SELECT COUNT(ID) as Cnt, (SELECT DISTINCT STUFF((SELECT ', ''' + CONVERT(varchar(max), ID) + '''' FROM TransMobizIssues WHERE Notify = 1 AND Status = 'Unresolved' FOR XML PATH('')), 1, 1,'')FROM TransMobizIssues) as IDs FROM TransMobizIssues WHERE Notify = 1 AND Status = 'Unresolved'"
                Dim dr As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)
                Dim pOut As New JsonDictionary

                pOut.Add("Count", dr("Cnt"))
                pOut.Add("IDs", dr("IDs"))
                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveRAI()
            Try
                Dim p As New InsertUpdate("TransMobizIssues")
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim ticketno As String = Format(GenerateRAINo(data("OrgID")), "000000")
                Dim dborgid, dbname As String
                Dim ticketid As String = Guid.NewGuid.ToString()

                p("ID") = ReadyString(ticketid)
                p("OrgID") = ReadyString(data("OrgID"))
                p("FowstID") = ReadyString(data("FowstID")) 'if NULL meaning, sa EAS/MCR gihimo ang ticket
                p("TicketNo") = ReadyString(ticketno)
                p("ReportedBy") = ReadyString(data("ReportedBy"))
                p("Product") = ReadyString(data("Product"))
                p("Subject") = ReadyString(data("Subject"))
                p("Urgency") = ReadyString(data("Urgency"))
                p("Importance") = ReadyString(data("Importance"))
                p("Description") = ReadyString(data("Description"))
                p("DateReported") = "getdate()"
                p("Status") = ReadyString("Unresolved")
                p("Notify") = 1

                If data("OrgID").ToString().ToLower() = "d362ffdb-15d1-451f-97dc-c567b94fb410" Or data("OrgID").ToString().ToLower() = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" Then
                    'FOR WOW CO and FO only
                    p("DelegateToClient") = 1
                Else
                    p("DelegateToClient") = 0 'ang Mobiz mismo mag update ani, meaning iallow ni Mobiz maka edit si client sa ticket na which is si Mobiz lang pwd mag-edit
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(p.GetInsert())

#If DEBUG Then
                'MobizABC dev db
                dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
                '1Mobiz prod db 
                dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If

                Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(dbname))

                Dim dtRow As DataRow = Common.WebRequest.ProData.ExecuteRow("SELECT * FROM MobizLibraryOrganization WHERE ID = " & ReadyString(data("OrgID")))
                Dim crm As New CRMDashboard
                Dim errmsg As String = ""
                Dim fowstname As String = WebRequest.Instance.Business.LibraryOrganizationFOWST.ByID(data("FowstID"))("Name")
                If crm.SaveOrganizationFromCustomerVendorLibraries(dtRow, 1, 1, errmsg) = False Then
                    AppendToDebug("Error when saving ticket to 1Mobiz CRM: " & errmsg)
                End If

                p = New InsertUpdate("TransCRM")

                p("ID") = ReadyString(ticketid)
                p("ClientID") = ReadyString(data("OrgID"))
                p("TicketType") = ReadyString(data("Subject"))
                p("ProductServices") = ReadyString(data("Product"))
                p("Status") = ReadyString("Unresolved")
                p("ActionName") = ReadyString("Ticket")
                p("LastUpdated") = "getdate()"
                p("TicketNumber") = ReadyString(ticketno)
                p("Notes") = ReadyString(data("Description"))
                p("TicketOpenDate") = "getdate()"
                p("CreatedBy") = ReadyString(data("ReportedBy"))
                p("OriginalCreatedBy") = ReadyString(data("ReportedBy"))
                p("FowstID") = ReadyString(data("FowstID"))

                Common.WebRequest.ProData.Execute(p.GetInsert())

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Results", ticketno)

            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GenerateRAINo(ByVal orgid As String) As Integer
            Dim ticketno As String = New System.Random().Next(1, 999999)
            Dim pSql As String = "SELECT TicketNo FROM TransMobizIssues WHERE TicketNo = " & ReadyString(ticketno) & " AND OrgID = " & ReadyString(orgid)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                ticketno = GenerateRAINo(orgid)
            End If

            Return ticketno
        End Function
    End Class
End Namespace