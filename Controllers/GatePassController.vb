Namespace WebProj
    Public Class GatePassController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function GetHtmlPage(ByVal path As String) As ActionResult
            'Return View()
            'Return PartialView("/Scripts/sources/ERP/GatePass/main/view.html")
            Return New FilePathResult(path, "text/html")
        End Function

        Function CheckGatePassUserIfHasAccess()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT ID FROM LibraryGatePassUsers WHERE UserID = " & ReadyString(Common.WebRequest.Data("UserID")) & vbCrLf &
                                        " AND Type = 'approver' "
                Dim hasAccess As Boolean = False

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                    hasAccess = True
                End If

                Return JsonDictionary.Serialize("Results", hasAccess)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function RemoveGatePassUser()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "DELETE FROM LibraryGatePassUsers WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "deleted")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetGatePassUsers()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.QueryString("type")
                Dim mainSql As String = "SELECT maker.*, makerppol.FirstName + ' ' + makerppol.Surname as MakerName, ppol.FirstName + ' ' + ppol.Surname as AddedByName " & vbCrLf &
                                    " FROM LibraryGatePassUsers as maker " & vbCrLf &
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

        Function SaveGatePassUsers()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For Each maker As Object In Common.WebRequest.AllData("data")
                    Dim p As New InsertUpdate("LibraryGatePassUsers")

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

        Function CountGatePasses()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pOut As New JsonDictionary
                Dim pSql As String = "SELECT COUNT(ID) AS Count FROM TransGatePassDetails WHERE IsApproved = 0 AND IsCancelled = 0 AND IsTagAsReturned = 0 "

                pOut.Add("Unapproved", CInt2(Common.WebRequest.ProData.Execute(pSql).Rows(0)("Count")))

                pSql = "SELECT COUNT(ID) AS Count FROM TransGatePassDetails WHERE IsApproved = 1 AND IsCancelled = 0 AND IsTagAsReturned = 0 "

                pOut.Add("Approved", CInt2(Common.WebRequest.ProData.Execute(pSql).Rows(0)("Count")))

                pSql = "SELECT COUNT(ID) AS Count FROM TransGatePassDetails WHERE IsApproved = 0 AND IsCancelled = 1 AND IsTagAsReturned = 0 "

                pOut.Add("Cancelled", CInt2(Common.WebRequest.ProData.Execute(pSql).Rows(0)("Count")))

                pSql = "SELECT COUNT(ID) AS Count FROM TransGatePassDetails WHERE IsApproved = 1 AND IsCancelled = 0 AND IsTagAsReturned = 1 "

                pOut.Add("Returned", CInt2(Common.WebRequest.ProData.Execute(pSql).Rows(0)("Count")))

                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetAllGatePasses()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim whereSearchContent As String = ""
                Dim operators As String = ""
                Dim arr As Object = Split(keyword, "+")
                Dim showtickets As String = Request.Params.Get("ShowPass")

                Dim mainquery As String = " SELECT gate.*, ROW_NUMBER() OVER (ORDER BY gate.DateCreated DESC) AS RowNum, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as PeopleName, " & vbCrLf &
                        " org.CompanyName + '' + org.PersonFirstName + ' ' + org.PersonMiddleName + ' ' + org.PersonSurname as AssociatedToName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName, " & vbCrLf &
                        " approvedorrejectedby.FirstName + ' ' + approvedorrejectedby.Surname as ApprovedOrRejectedByName, " & vbCrLf &
                        " requestedby.FirstName + ' ' + requestedby.Surname as RequestedByName, " & vbCrLf &
                        " CONVERT(varchar, fowst.FowstNO) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, gate.GatePassNo) as ReferenceNo " & vbCrLf &
                        " FROM TransGatePassDetails as gate " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = gate.PeopleID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = gate.AssociatedTo " & vbCrLf &
                        " LEFT JOIN LibraryDevice as dev ON dev.ID = gate.DeviceID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = gate.FromLocation " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as encoded ON encoded.ID = gate.EncodedBy " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as requestedby ON requestedby.ID = gate.RequestedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = gate.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as approvedorrejectedby ON approvedorrejectedby.ID = gate.ApprovedOrRejectedOrCancelledBy " & vbCrLf

                If showtickets = "unapproved" Then
                    mainquery &= " WHERE IsApproved = 0 AND IsRejected = 0 AND IsCancelled = 0  AND IsTagAsReturned = 0 "
                ElseIf showtickets = "approved" Then
                    mainquery &= " WHERE IsApproved = 1 AND IsRejected = 0  AND IsCancelled = 0  AND IsTagAsReturned = 0 "
                ElseIf showtickets = "rejected" Then
                    mainquery &= " WHERE IsApproved = 0 AND IsRejected = 1  AND IsCancelled = 0  AND IsTagAsReturned = 0"
                ElseIf showtickets = "cancelled" Then
                    mainquery &= " WHERE IsApproved = 0 AND IsRejected = 0  AND IsCancelled = 1 AND IsTagAsReturned = 0 "
                ElseIf showtickets = "returned" Then
                    mainquery &= " WHERE IsApproved = 1 AND IsRejected = 0  AND IsCancelled = 0 AND IsTagAsReturned = 1 "
                End If

                If CStr2(keyword) <> "" Then
                    mainquery += " AND ("
                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " (ISNULL(CONVERT(varchar(max), fowst.FowstNo), '') + '-' + ISNULL(CONVERT(varchar(max), dev.DeviceNo), '') + '-' + ISNULL(CONVERT(varchar, gate.GatePassNo), '')) + ' ' + " & vbCrLf &
                             " ISNULL(ppol.FirstName, '') + ' ' + " & vbCrLf &
                             " ISNULL(ppol.Surname, '') + ' ' + " & vbCrLf &
                             " SUBSTRING(ISNULL(CONVERT(varchar(max), DateCreated), ''), 1, 11) + ' ' + " & vbCrLf &
                             " ISNULL(org.CompanyName, '') + ' ' + " & vbCrLf &
                             " ISNULL(gate.Particulars, '') + ' ' + " & vbCrLf &
                             " ISNULL(gate.Remarks, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next

                    mainquery += whereSearchContent & ") "
                End If

                pSql = "SELECT * FROM (" & vbCrLf &
                                mainquery & vbCrLf &
                                " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                                " ORDER BY DateCreated DESC"

                Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
                'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveGatePass()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim gatepassID As String

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim p As New InsertUpdate("TransGatePassDetails")
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.GetOrCreateDesktop()
                Dim pDeviceID = CStr2(pDrDevice("ID"))
                Dim accountpayableerror As String = ""

                p("PeopleID") = IIf(data("PeopleID") = "", "NULL", ReadyString(data("PeopleID")))
                p("FromLocation") = ReadyString(data("FromLocation"))
                p("ToLocation") = IIf(data("ToLocation") = "Other Location", "NULL", ReadyString(data("ToLocation")))
                p("Remarks") = IIf(data("Remarks") = "", "NULL", ReadyString(data("Remarks")))
                p("AssociatedTo") = IIf(data("AssociatedTo") = "", "NULL", ReadyString(data("AssociatedTo")))
                p("ReturnDate") = IIf(data("ReturnDate") = "", "NULL", ReadyString(data("ReturnDate")))
                p("OtherToLocation") = IIf(data("OtherToLocation") = "", "NULL", ReadyString(data("OtherToLocation")))
                p("Particulars") = ReadyString(data("Particulars"))
                p("RequestedBy") = ReadyString(data("RequestedBy"))

                If CStr2(data("ID")) = "" Then
                    gatepassID = Guid.NewGuid.ToString()
                    p("DateCreated") = "getdate()"
                    p("ID") = ReadyString(gatepassID)
                    p("DeviceID") = ReadyString(pDeviceID)
                    p("GatePassNo") = GetGatePassNo(pDeviceID, data("FromLocation"))
                    p("EncodedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("IsApproved") = 0
                    p("IsRejected") = 0
                    p("IsCancelled") = 0
                    p("IsTagAsReturned") = 0
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    If CBool2(data("IsApproved")) Then
                        p("IsApproved") = CInt2(data("IsApproved"))
                    End If

                    If CBool2(data("IsRejected")) Then
                        p("IsRejected") = CInt2(data("IsRejected"))
                    End If

                    If CBool2(data("IsCancelled")) Then
                        p("IsCancelled") = CInt2(data("IsCancelled"))
                    End If

                    If CBool2(data("IsApproved")) Or CBool2(data("IsRejected")) Or CBool2(data("IsCancelled")) Then
                        p("ApprovedOrRejectedOrCancelledBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("DateApprovedOrRejectedOrCancelled") = "getdate()"
                    End If

                    If CBool2(data("IsTagAsReturned")) Then
                        p("IsTagAsReturned") = data("IsTagAsReturned")
                        p("ItemReturnedOn") = "getdate()"
                    End If

                    gatepassID = data("ID")
                    p("LastUpdated") = "getdate()"
                    'GET ORIGINAL DATA
                    'pSql = "SELECT * FROM TransGatePassDetails WHERE ID = " & ReadyString(data("ID"))
                    'Dim origdata As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                    'SAVE ORIGINAL DATA TO HISTORY TABLE FOR FUTURE AUDITABILITY - July 28, 2020 by Arnie
                    'SaveGatePassDetailsHistory(origdata, Common.WebRequest.ProData)

                    p("LastUpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("ID"))))
                End If

                Common.WebRequest.ProData.CommitTransaction()

                pOut.Add("Results", "saved")
                pOut.Add("ID", gatepassID)

            Catch ex As Exception
                pOut.Add("Results", ex.Message)
            End Try

            Return pOut.Serialize()
        End Function

        Function GetGatePassNo(ByVal deviceid As String, ByVal fowstid As String)
            Dim LastExpenseReferenceNo As String = CInt2(Common.WebRequest.ProData.Execute("SELECT ISNULL(MAX(GatePassNo), 0) AS GatePassNo FROM TransGatePassDetails WHERE DeviceID = " & ReadyString(deviceid) & " AND FromLocation = " & ReadyString(fowstid)).Rows(0)("GatePassNo"))

            Return LastExpenseReferenceNo + 1
        End Function

        Public Function SaveGatePassDetailsHistory(ByVal origdata As Object, ByVal mProdata As ProData)
            Dim pHistory As New InsertUpdate("TransGatePassDetailsHistory")
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

        Function DownloadGatePassReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pReport As ProReport
                Dim pOrgShort As New OrganizationShort
                Dim file As System.IO.FileInfo
                Dim pFilename = "GatePass"
                Dim pSql As String = " SELECT gate.*, ROW_NUMBER() OVER (ORDER BY gate.DateCreated DESC) AS RowNum, " & vbCrLf &
                        " ppol.FirstName + ' ' + ppol.Surname as PeopleName, " & vbCrLf &
                        " org.CompanyName + '' + org.PersonFirstName + ' ' + org.PersonMiddleName + ' ' + org.PersonSurname as AssociatedToName, " & vbCrLf &
                        " encoded.FirstName + ' ' + encoded.Surname as EncodedByName, " & vbCrLf &
                        " lastupdatedby.FirstName + ' ' + lastupdatedby.Surname as LastUpdatedByName, " & vbCrLf &
                        " approvedorrejectedby.FirstName + ' ' + approvedorrejectedby.Surname as ApprovedOrRejectedByName, " & vbCrLf &
                        " requestedby.FirstName + ' ' + requestedby.Surname as RequestedByName, " & vbCrLf &
                        " CONVERT(varchar, fowstfrom.FowstNO) + '-' + CONVERT(varchar, dev.DeviceNo) + '-' + CONVERT(varchar, gate.GatePassNo) as ReferenceNo, " & vbCrLf &
                        " fowstfrom.Name as FowstLocationName, " & vbCrLf &
                        " ISNULL(fowstto.Name, gate.OtherToLocation) as ToLocationName, " & vbCrLf &
                        " ISNULL(CONVERT(varchar(max), gate.ReturnDate), 'N/A') as ReturnDateString " & vbCrLf &
                        " FROM TransGatePassDetails as gate " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = gate.PeopleID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = gate.AssociatedTo " & vbCrLf &
                        " INNER JOIN LibraryDevice as dev ON dev.ID = gate.DeviceID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST as fowstfrom ON fowstfrom.ID = gate.FromLocation " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST as fowstto ON fowstto.ID = gate.ToLocation " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as encoded ON encoded.ID = gate.EncodedBy " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as requestedby ON requestedby.ID = gate.RequestedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as lastupdatedby ON lastupdatedby.ID = gate.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as approvedorrejectedby ON approvedorrejectedby.ID = gate.ApprovedOrRejectedOrCancelledBy " & vbCrLf &
                        " WHERE gate.ID = " & ReadyString(Request.Params.Get("ID"))
                Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

                pReport = New ProReport()
                pReport.AutoFillHeader = False
                pReport.SourceFilename = pFilename

                pReport.AddTable("Info", pDT)
                pReport.AddTable("CompanyInfo", pOrgShort.GetProspectOrgDetails(WebRequest.Instance.OrgID))

                ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"

                pFilename = Common.ServerPath & "FileTemplates\GP-" & pDT.Rows(0)("ReferenceNo") & ".pdf"

                pReport.ExportXmlFile()
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

                file = New System.IO.FileInfo(pFilename)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=GP-" & pDT.Rows(0)("ReferenceNo") & ".pdf")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function
    End Class
End Namespace