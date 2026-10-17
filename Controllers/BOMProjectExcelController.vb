Imports System.IO

Namespace WebProj
    Public Class BOMProjectExcelController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function EmploymentConfirmation() As ActionResult
            Return View()
        End Function

        Function GetSKUsUnitMeasures() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim browser As New BrowserController
            Dim sku As Object = Common.WebRequest.AllData("sku")

            For Each s As Object In sku
                s.Add("Uom", jss.DeserializeObject(browser.LibraryPIMProductMeasure(s("ID").ToString()))("d")("Results")("Rows"))
            Next s

            Return Json(New With {Key .data = sku})
        End Function

        Function GetPIMUoM() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pJson As New JsonDictionary()
            Dim pRow As DataRow = WebRequest.Instance.Business.LibraryStock.GetFirstUnit(Common.WebRequest.Data("ProductID"))
            pJson.Add("Unit", pRow)
            Return pJson.Serialize()

        End Function

        <HttpPost()>
        Function GetAllowedPermissionsPerJobRole() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            strReturn = JsonDictionary.Serialize("Results", clsBOMProjectExcel.GetAllowedPermissionsPerJobRole())

            Return strReturn
        End Function

        Function GetAccessRights() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("id")
            strReturn = JsonDictionary.Serialize("Results", clsBOMProjectExcel.GetAccessRights())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetJobRole() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            strReturn = JsonDictionary.Serialize("Results", clsBOMProjectExcel.GetJobRole())

            Return strReturn
        End Function

        Function GetOrgFowst() As String
            Dim strOrgID As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim strReturn As String

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True
                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "fowst.ID"
                        .Searchable = False
                    End With
                    With .Add("FowstName", SqlDbType.UniqueIdentifier)
                        .Expression = "fowst.Name"
                    End With
                    With .Add("Address", SqlDbType.VarChar)
                        .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                        "+ ', ' + ISNULL(fowst.Street, '') " &
                                        "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                    End With
                    With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                        .Expression = "fowst.IsFOWSTInsideAMall"
                    End With
                    With .Add("FOWSTMallName", SqlDbType.VarChar)
                        .Expression = "fowst.FOWSTMallName"
                    End With
                End With

                If CInt(WebRequest.Instance.Data("IsThirdPartyOrganization")) = 1 Then
                    strOrgID = WebRequest.Instance.Data("ThirdPartyOrganizationID")
                Else
                    strOrgID = WebRequest.Instance.OrgID
                    If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
                        If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                            strOrgID = WebRequest.Instance.Data("OrganizationID").Trim()
                        End If
                    End If
                End If


                If CInt(WebRequest.Instance.Data("FilterFowst")) = 1 Then
                    .SqlFrom = "FROM dbo.LibraryOrganizationEmployee INNER JOIN " &
                          "dbo.LibraryOrganizationEmployeeFOWST ON dbo.LibraryOrganizationEmployee.PeopleID = dbo.LibraryOrganizationEmployeeFOWST.PeopleID INNER JOIN " &
                          "dbo.MobizLibraryOrganizationFOWST AS fowst ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID" & vbCrLf &
                          " JOIN LibraryDeviceFowst ON LibraryDeviceFowst.FowstID = fowst.ID" & vbCrLf &
                          " JOIN LibraryDevice ON LibraryDevice.ID = LibraryDeviceFowst.DeviceID" & vbCrLf &
                          " AND LibraryDevice.UUID = " & ReadyString(WebSession.Instance.DeviceSerial) & vbCrLf &
                          " LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " &
                          "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID"
                    .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) & " AND dbo.LibraryOrganizationEmployee.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & " AND dbo.LibraryOrganizationEmployee.Deactivated = 0" & " AND fowst.FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'"
                Else
                    .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
                    " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                    " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)"

                    .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) &
                                " AND fowst.FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'"
                End If

                .SqlOrder = "ORDER BY Address"

                pDT = .Requery
            End With
            strReturn = JsonDictionary.Serialize("Results", pDT)
            Return strReturn
        End Function

        <HttpPost()>
        Function GetProjectBudget() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String
            Dim pSql As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrgID")

            Dim budget As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(clsBOMProjectExcel.GetProjectBudget()))

            For Each item As Object In budget
                'pSql = "SELECT sku.*, pim.ProductName as SkuName, lbl.Label as UomName, 'edit' as State FROM LibraryProjectBudgetAssociatedSku as sku " & vbCrLf &
                '                    " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = sku.SkuID " & vbCrLf &
                '                    " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = sku.UomID " & vbCrLf &
                '                    " INNER JOIN MobizLibraryCountriesLanguageLabel as lbl ON lbl.ID = msr.MeasureNameID " & vbCrLf &
                '                    " WHERE ProjectBudgetID = " & ReadyString(item("ID"))
                'item.Add("AssociatedSKUs", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                item.Add("Remarks", GetLogs(item("ID"), "remarks"))
                item.Add("BudgetedDuration", GetLogs(item("ID"), "budgetedduration"))
                item.Add("BudgetedQty", GetLogs(item("ID"), "budgetedqty"))
                'item.Add("AccReqQty", GetLogs(item("ID"), "accreqqty"))
                item.Add("AccUsedUpQty", GetLogs(item("ID"), "accusedupqty"))
                item.Add("LaborCost", GetLogs(item("ID"), "laborcost"))
                item.Add("RequestedAdvances", GetLogs(item("ID"), "RequestedAdvances"))

                pSql = "SELECT req.ID, req.RequestedValue as Qty, Status as State, req.RequestedByID, convert(varchar, req.LastUpdated, 22) as LastUpdated " & vbCrLf &
                        " , convert(varchar, req.DateNeeded, 22) as DateNeeded, STUFF(RIGHT(CONVERT(VarChar(19), req.TimeNeededFrom, 0), 7), 6, 0, ' ') as TimeNeededFrom, STUFF(RIGHT(CONVERT(VarChar(19), req.TimeNeededTo, 0), 7), 6, 0, ' ') as TimeNeededTo, req.Remarks " & vbCrLf &
                        " , reqby.FirstName + ' ' + reqby.Surname as RequestedByName, req.RequestedByID as ModifiedByID, req.HasSkuID, req.ProjectBudgetID " & vbCrLf &
                        " FROM LibraryProjectMaterialsRequisition as req " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as reqby ON reqby.ID = req.RequestedByID " & vbCrLf &
                        " WHERE req.ProjectBudgetID = " & ReadyString(item("ID"))
                item.Add("AccReqQty", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
            Next item
            'strReturn = JsonDictionary.Serialize("ProjectBudget", clsBOMProjectExcel.GetProjectBudget())

            Return Json(New With {Key .data = budget})

        End Function

        Function GetLogs(ByVal ProjectBudgetId As String, ByVal logtype As String) As Object
            Dim pSql As String

            pSql = "SELECT logs.ID, logs.ModifiedByID, convert(varchar, logs.LastUpdated, 22) as LastUpdated " & vbCrLf

            If logtype = "remarks" Then
                pSql += ", logs.Remarks, 'edit' as State "
            ElseIf logtype = "budgetedduration" Then
                pSql += ", logs.Duration, logs.StateIn, 'edit' as State "
            ElseIf logtype = "budgetedqty" Then
                pSql += ", logs.Qty --, logs.UomID, lbl.Label as UomName, 'edit' as State " & vbCrLf
            ElseIf logtype = "accreqqty" Or logtype = "accusedupqty" Then
                pSql += ", logs.Qty, logs.Remarks, ISNULL(logs.Status, 'requested') as State, convert(varchar, logs.DateNeeded, 107) as DateNeeded, STUFF(RIGHT(CONVERT(VarChar(19), logs.TimeNeededFrom, 0), 7), 6, 0, ' ') as TimeNeededFrom, STUFF(RIGHT(CONVERT(VarChar(19), logs.TimeNeededTo, 0), 7), 6, 0, ' ') as TimeNeededTo " & vbCrLf &
                        ", (SELECT ISNULL(ID, '') as ID FROM LibraryProjectMaterialsRequisition WHERE LogID = logs.ID) as MaterialRequisitionID "
            ElseIf logtype = "laborcost" Then
                pSql += ", logs.Amount, 'edit' as State "
            ElseIf logtype = "RequestedAdvances" Then
                pSql += ", logs.Amount, logs.Remarks, logs.PaymentBalance, ISNULL(logs.Status, 'requested') as State"
            End If

            pSql += " , ppol.FirstName + ' ' + ppol.Surname as ModifiedByName " & vbCrLf

            If logtype = "accreqqty" Or logtype = "RequestedAdvances" Then
                pSql += " , reqby.FirstName + ' ' + reqby.Surname as RequestedByName, logs.RequestedByID " & vbCrLf
            End If

            pSql += " FROM LibraryProjectLogs as logs " & vbCrLf

            'If logtype = "budgetedqty" Then
            '    pSql += " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = logs.UomID " & vbCrLf &
            '            " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf
            'End If

            If logtype = "accreqqty" Or logtype = "RequestedAdvances" Then
                pSql += " INNER JOIN MobizLibraryPeople as reqby ON reqby.ID = logs.RequestedByID " & vbCrLf
            End If

            pSql += " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = logs.ModifiedByID " & vbCrLf &
                    " WHERE LogType = " & ReadyString(logtype) & " AND ProjectBudgetID = " & ReadyString(ProjectBudgetId) & vbCrLf &
                    " ORDER BY logs.LastUpdated ASC "

            Return jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        End Function

        Function GetOrgDetailsToPrint() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrgID")

            strReturn = JsonDictionary.Serialize("OrgDetails", clsBOMProjectExcel.GetOrgDetailsToPrint())

            Return strReturn
        End Function

        Function GetProjectPermissions() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT * FROM LibraryProjectPermission ORDER BY Description ASC"

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetProjectBudgetTimeline() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrgID")

            strReturn = JsonDictionary.Serialize("Timeline", clsBOMProjectExcel.GetProjectBudgetTimeline())

            Return strReturn

        End Function

        Function GetProjectBudgetCanvassMaterials() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrgID")

            If Common.WebRequest.Data("CanvassForm") = "yes" Then
                strReturn = JsonDictionary.Serialize("CanvassMaterials", clsBOMProjectExcel.GetProjectBudgetCanvassMaterialsForm())
            Else
                strReturn = JsonDictionary.Serialize("CanvassMaterials", clsBOMProjectExcel.GetProjectBudgetCanvassMaterials())
            End If

            Return strReturn

        End Function

        Function GetProjectBudgetCanvassLabor() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrgID")

            If Common.WebRequest.Data("CanvassForm") = "yes" Then
                strReturn = JsonDictionary.Serialize("CanvassLabor", clsBOMProjectExcel.GetProjectBudgetCanvassLaborForm())
            Else
                strReturn = JsonDictionary.Serialize("CanvassLabor", clsBOMProjectExcel.GetProjectBudgetCanvassLabor())
            End If

            Return strReturn

        End Function

        Function GetProjectOwnerOrgPerson() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsBOMProjectExcel.FowstID = Common.WebRequest.Data("FowstID")
            strReturn = JsonDictionary.Serialize("ProjectOwnerOrgPerson", clsBOMProjectExcel.GetProjectOwnerOrgPerson())

            Return strReturn

        End Function

        Function GetCanvassMaterials() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Common.WebRequest.Data("ProjectID")
            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrgID")

            strReturn = JsonDictionary.Serialize("CanvassLabor", clsBOMProjectExcel.GetProjectBudgetCanvassLabor())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProjectHeader() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String
            Dim pDt As DataTable

            clsBOMProjectExcel.OrganizationID = Request.QueryString.GetValues(0)(0).ToString() 'parameter OrgID
            clsBOMProjectExcel.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsBOMProjectExcel.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsBOMProjectExcel.PageSearchText = CStr2(Request.Params.Get("keywords"))
            clsBOMProjectExcel.IsCountPage = False
            clsBOMProjectExcel.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsBOMProjectExcel.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()

            clsBOMProjectExcel.OrganizationID = Request.Params.Get("OrgID")
            clsBOMProjectExcel.FowstID = Request.Params.Get("ProjectFowstID")

            pDt = clsBOMProjectExcel.GetProjectHeader()

            clsBOMProjectExcel.IsCountPage = True
            Dim pDTCountPage As DataTable = clsBOMProjectExcel.GetProjectHeader()

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDt)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)

        End Function

        Function GetProjectInfo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrganizationID")
            clsBOMProjectExcel.FowstID = Common.WebRequest.Data("ProjectFowstID")

            strReturn = JsonDictionary.Serialize("ProjectHeader", clsBOMProjectExcel.GetProjectInfo())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProjectOwnerManagerAssistant() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.FowstID = Common.WebRequest.Data("FowstID")
            strReturn = JsonDictionary.Serialize("ProjectPeopleDetails", clsBOMProjectExcel.GetProjectOwnerManagerAssistant())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProjectTelephoneInfo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.FowstID = Common.WebRequest.Data("FowstID")
            strReturn = JsonDictionary.Serialize("ProjectTelephoneDetails", clsBOMProjectExcel.GetProjectTelephoneInfo())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProjectMobileInfo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.FowstID = Common.WebRequest.Data("FowstID")
            strReturn = JsonDictionary.Serialize("ProjectMobileDetails", clsBOMProjectExcel.GetProjectMobileInfo())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProjectEmailInfo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.FowstID = Common.WebRequest.Data("FowstID")
            strReturn = JsonDictionary.Serialize("ProjectEmailDetails", clsBOMProjectExcel.GetProjectEmailInfo())

            Return strReturn

        End Function

        Function CheckProjectName() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectName = Common.WebRequest.Data("ProjectName")

            strReturn = JsonDictionary.Serialize("ProjectName", clsBOMProjectExcel.CheckProjectName)
            Return strReturn
        End Function

        Function CheckProjectCode() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectCode = Common.WebRequest.Data("ProjectCode")

            strReturn = JsonDictionary.Serialize("ProjectCode", clsBOMProjectExcel.CheckProjectCode)
            Return strReturn
        End Function

        <HttpPost()>
        Function SaveProjectHeader() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.OrganizationID = Common.WebRequest.Data("OrgID")
            clsBOMProjectExcel.FowstID = Common.WebRequest.Data("FowstID") 'aka ProjectID
            clsBOMProjectExcel.ProjectTypeID = Common.WebRequest.Data("ProjectTypeID")
            clsBOMProjectExcel.PreTypeFowstName = Common.WebRequest.Data("PreTypeFowstName")
            clsBOMProjectExcel.PreTypeFowstNameID = Common.WebRequest.Data("PreTypeFowstNameID")
            clsBOMProjectExcel.ProjectName = Common.WebRequest.Data("ProjectName")
            clsBOMProjectExcel.ProjectCode = Common.WebRequest.Data("ProjectCode")
            clsBOMProjectExcel.ProjectDescription = Common.WebRequest.Data("ProjectDescription")
            clsBOMProjectExcel.ProjectBusinessNameID = Common.WebRequest.Data("ProjectBusinessNameID")
            clsBOMProjectExcel.ProjectOwnerOrgPersonID = Common.WebRequest.Data("ProjectOwnerOrgPersonID")
            clsBOMProjectExcel.ClientID = Common.WebRequest.Data("ClientID")
            clsBOMProjectExcel.BranchFowstID = Common.WebRequest.Data("BranchFowstID")

            clsBOMProjectExcel.ProjectHouseNo = Common.WebRequest.Data("ProjectHouseNo")
            clsBOMProjectExcel.ProjectStreet = Common.WebRequest.Data("ProjectStreet")
            clsBOMProjectExcel.ProjectCityID = Common.WebRequest.Data("ProjectCityID")
            clsBOMProjectExcel.ProjectCountryID = Common.WebRequest.Data("ProjectCountryID")

            clsBOMProjectExcel.JobRolePermission = Common.WebRequest.AllData("JobRolePermission")
            clsBOMProjectExcel.JobRole = Common.WebRequest.AllData("JobRole")
            clsBOMProjectExcel.ProjectMember = Common.WebRequest.AllData("ProjectMember")

            clsBOMProjectExcel.ProjectOwner = Common.WebRequest.AllData("ProjectOwner")
            clsBOMProjectExcel.ProjectManager = Common.WebRequest.AllData("ProjectManager")
            clsBOMProjectExcel.ProjectAssistantManager = Common.WebRequest.AllData("ProjectAssistantManager")

            clsBOMProjectExcel.ProjectedStartDate = Common.WebRequest.Data("ProjectedStartDate")
            clsBOMProjectExcel.ActualStartDate = Common.WebRequest.Data("ActualStartDate")
            clsBOMProjectExcel.ProjectedCompletionDate = Common.WebRequest.Data("ProjectedCompletionDate")
            clsBOMProjectExcel.ActualCompletionDate = Common.WebRequest.Data("ActualCompletionDate")

            clsBOMProjectExcel.ProjectTelephoneNumber = Common.WebRequest.AllData("ProjectTelephoneNumber")
            clsBOMProjectExcel.ProjectMobileNumber = Common.WebRequest.AllData("ProjectMobileNumber")
            clsBOMProjectExcel.ProjectEmailAddress = Common.WebRequest.AllData("ProjectEmailAddress")

            strReturn = JsonDictionary.Serialize("ProjectHeader", clsBOMProjectExcel.SaveProjectHeader)

            Return strReturn

        End Function

        Function SaveProjectBudget() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            Dim files As HttpFileCollectionBase = Request.Files
            Dim data As Object = jss.Deserialize(Of Object)(Request.Form("data"))

            clsBOMProjectExcel.Projectphotos = Request.Files
            clsBOMProjectExcel.ProjectID = data("ProjectID")
            clsBOMProjectExcel.ProjectBudgetData = data("ProjectBudget")
            clsBOMProjectExcel.ProjectBudgetCanvassMaterials = data("ProjectCanvassMaterials")
            clsBOMProjectExcel.ProjectBudgetCanvassLabor = data("ProjectCanvassLabor")
            clsBOMProjectExcel.ProjectBudgetTimeline = data("ProjectTimeline")

            strReturn = JsonDictionary.Serialize("ProjectBudget", clsBOMProjectExcel.SaveProjectBudget)

            Return strReturn
        End Function

        Function UploadProjectFiles() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsBOMProjectExcel As New BOMProjectExcel
            Dim strReturn As String

            clsBOMProjectExcel.ProjectID = Request.Form("ProjectID")
            clsBOMProjectExcel.ProjectFiles = Request.Files
            clsBOMProjectExcel.ProjectFilesDetails = jss.Deserialize(Of Object)(Request.Form("data"))

            Return JsonDictionary.Serialize("Results", clsBOMProjectExcel.UploadProjectFiles())
        End Function

        Function GetRemarksPhotos() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim ProjectBudgetID As String = Common.WebRequest.Data("ProjectBudgetID")
            Dim RemarksID As String = Common.WebRequest.Data("RemarksID")
            Dim pSql As String = "SELECT ID, FileName as filename, 'edit' as status FROM LibraryProjectPhotos WHERE RemarksID = " & ReadyString(RemarksID)
            Dim rows As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            For Each r As Object In rows
                Dim folderName As String = Common.ServerPath & "ProjectPhotos\" & WebRequest.Instance.OrgID & "\" & ProjectBudgetID & "\" & r("filename")
                Dim src As String = "/ProjectPhotos/" & WebRequest.Instance.OrgID & "/" & ProjectBudgetID & "/" & r("filename")
                r.Add("src", src) 'Convert.ToBase64String(System.IO.File.ReadAllBytes(folderName))
                r.Add("size", New FileInfo(folderName).Length)
                r.Add("isnew", False)
            Next r

            Return Json(New With {Key .data = rows})
        End Function

        Function GetProjectFiles() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim ProjectID As String = Common.WebRequest.Data("ProjectID")
            Dim pSql As String = "SELECT ph.ID, FileName as filename, FileDescription as filedescription, 'edit' as status, " & vbCrLf &
                " convert(varchar, ph.DateUploaded, 120) as DateUploaded, ppol.FirstName + ' ' + ppol.Surname as UploadedBy " & vbCrLf &
                " FROM LibraryProjectPhotos as ph" & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = ph.UploadedBy " & vbCrLf &
                " WHERE ProjectID = " & ReadyString(ProjectID) & vbCrLf &
                " ORDER BY ph.DateUploaded DESC "
            Dim rows As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            For Each r As Object In rows
                Dim folderName As String = Common.ServerPath & "ProjectFiles\" & WebRequest.Instance.OrgID & "\" & ProjectID & "\" & r("filename")
                Dim src As String = "/ProjectFiles/" & WebRequest.Instance.OrgID & "/" & ProjectID & "/" & r("filename")
                r.Add("src", src) 'Convert.ToBase64String(System.IO.File.ReadAllBytes(folderName))
                r.Add("type", System.IO.Path.GetExtension(folderName))
                r.Add("size", New FileInfo(folderName).Length)
                r.Add("isnew", False)
                r.Add("IsImage", IsValidImage(folderName))
            Next r

            Return Json(New With {Key .data = rows})
        End Function

        Function DownloadProjectFile() As String
            'Dim webClient As New WebClient
            Dim source As String = "ProjectFiles/" & WebRequest.Instance.OrgID & "/" & Request.Params.Get("ProjectID") & "/" & Request.Params.Get("FileName")
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

            'webClient.DownloadFile(source & Request.Params.Get("FileName"), Common.ServerPath & "SurveyFiles/" & Request.Params.Get("ClientID") & "/" & Request.Params.Get("DownloadFileName"))
            'webClient.Dispose()
        End Function

        Function IsValidImage(filename As String) As Boolean
            Try
                Dim img As System.Drawing.Image = System.Drawing.Image.FromFile(filename)
            Catch generatedExceptionName As OutOfMemoryException
                Return False
            End Try
            Return True
        End Function

        Function SaveNewClient() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsbom As New BOMProjectExcel
            clsbom.ClientData = Common.WebRequest.AllData("data")

            Return JsonDictionary.Serialize("Results", clsbom.SaveNewClient())
        End Function
    End Class
End Namespace