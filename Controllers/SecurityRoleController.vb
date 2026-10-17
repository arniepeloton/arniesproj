Public Class SecurityRoleController
    Inherits System.Web.Mvc.Controller

    Public Const RequestApproveModule As String = "[Approve Request]"

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
            If (Not SecurityRole.Instance.HandleRoleAccess("Security Roles")) Then Exit Function
        End If

        Return View()

    End Function

    Function SecurityRoleSummary() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function SecurityRoleInfo() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function SecurityRoleUsers() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    <HttpPost()>
    Function setCreateNew() As String
        Dim blnIsNew As Boolean
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        blnIsNew = Common.WebRequest.Data("IsNew")
        WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
        WebSession.Instance.SecurityRoleID = Common.WebRequest.Data("ID")

        strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

        Return strReturn

    End Function

    <HttpPost()>
    Function IsNewEntry() As String
        Dim pOut As New JsonDictionary

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
        pOut.Add("SecurityRoleID", WebSession.Instance.SecurityRoleID)

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Function CheckUniqueSecurityRoleCode() As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSecurityRole.ID = Common.WebRequest.Data("SecurityRoleID")
        clsSecurityRole.Code = Common.WebRequest.Data("Code")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.CheckUniqueSecurityRoleCode())

        Return strReturn

    End Function

    <HttpPost()>
    Function CheckUniqueSecurityRole() As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSecurityRole.ID = Common.WebRequest.Data("SecurityRoleID")
        clsSecurityRole.Role = Common.WebRequest.Data("Role")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.CheckUniqueSecurityRole())

        Return strReturn

    End Function

    <HttpPost()>
    Function getSecurityRoleList() As String
        Dim strKeyWord As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strKeyWord = Common.WebRequest.Data("SearchString")
        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.getSecurityRoleList(strKeyWord))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSecurityRoleModules() As String
        Dim strID As String
        Dim strModuleMainID As String = ""
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strModuleMainID = Common.WebRequest.Data("MainModuleID")
        If IsGuid(strModuleMainID) Then
            ' just do nothing here
        ElseIf (Common.WebRequest.Data("MainModule") = "POS") Then
            strModuleMainID = "ED33178B-1C61-4404-A28F-73288B97B9E4"
        ElseIf (Common.WebRequest.Data("MainModule") = "Receiving and Transfers") Then
            strModuleMainID = "3b9310f0-b9fd-4b2a-889e-3bdfc725882f"
        ElseIf (Common.WebRequest.Data("MainModule") = "Inventory Count") Then
            strModuleMainID = "75ca0131-70ee-40ef-994b-7a58b962b1b6"
        ElseIf (Common.WebRequest.Data("MainModule") = "SFA") Then
            strModuleMainID = "8D25405D-C74B-4863-AE36-A2D0B70F544A"
        ElseIf (Common.WebRequest.Data("MainModule") = "CRM") Then
            strModuleMainID = "73C8E6C9-58E3-4067-8839-065C7E228E5E"
        ElseIf (Common.WebRequest.Data("MainModule") = "Dashboard and Reports") Then
            strModuleMainID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7"
        ElseIf (Common.WebRequest.Data("MainModule") = "Settings") Then
            strModuleMainID = "BCB686E3-185F-45CE-A3CF-FE787A86D532"
        ElseIf (Common.WebRequest.Data("MainModule") = "MDM") Then
            strModuleMainID = "df45e0e2-31e8-4c93-8d15-d106192f6387"
        ElseIf (Common.WebRequest.Data("MainModule") = "Requests and Approvals") Then
            strModuleMainID = "A02F86C9-8CE6-4977-96D8-0203211C9F48"
        ElseIf (Common.WebRequest.Data("MainModule") = "Sales Returns") Then
            strModuleMainID = "81BA8822-203E-4DE0-9B0D-A6B2AAB8ABA3"
        ElseIf (Common.WebRequest.Data("MainModule") = "PO") Then
            strModuleMainID = "81ba8822-203e-4de0-9b0d-a6b2aab8aba4"
        ElseIf (Common.WebRequest.Data("MainModule") = "CRM Dashboard") Then
            strModuleMainID = "2055faf3-d090-4848-8081-f93ab93762d7"
        ElseIf (Common.WebRequest.Data("MainModule") = "CRM Tab") Then
            strModuleMainID = "2055faf3-d090-4848-8081-f93ab93762d8"
        ElseIf (Common.WebRequest.Data("MainModule") = "Opportunities Tab") Then
            strModuleMainID = "2055faf3-d090-4848-8081-f93ab93762d9"
        ElseIf (Common.WebRequest.Data("MainModule") = "Issues Tab") Then
            strModuleMainID = "2055faf3-d090-4848-8081-f93ab93762d6"
        ElseIf (Common.WebRequest.Data("MainModule") = "CRM Settings Tab") Then
            strModuleMainID = "2055faf3-d090-4848-8081-f93ab93762d5"
        ElseIf (Common.WebRequest.Data("MainModule") = "Troubleshooting and Maintenance Tools") Then
            strModuleMainID = "48EA33E8-9991-42A8-8C83-98B71EFB4C05"
        ElseIf (Common.WebRequest.Data("MainModule") = "ERP") Then
            strModuleMainID = "BAA58006-EFBF-45E8-8160-D899EE5DF718"
        ElseIf (Common.WebRequest.Data("MainModule") = "DTR") Then
            strModuleMainID = "F4910DDE-0674-44CA-B5D6-86B6439B0FA2"
        ElseIf (Common.WebRequest.Data("MainModule") = "AssetsManagement") Then
            strModuleMainID = "e1e68f63-e681-4889-9a00-dfc34a5eabf2"
        ElseIf (Common.WebRequest.Data("MainModule") = "VendorPriceCanvassing") Then
            strModuleMainID = "50e3c106-cefe-4a62-afb0-a8dbb64cb5ae"
        End If

        strID = Common.WebRequest.Data("SecurityRoleID")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.getSecurityRoleModules(strID, strModuleMainID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSecurityRoleModulesChild() As String
        Dim strID As String
        Dim strHeaderID As String = ""
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("SecurityRoleID")
        strHeaderID = Common.WebRequest.Data("SecurityModuleHeaderID")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.getSecurityRoleModulesChild(strID, strHeaderID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSecurityRoleModulesWOHeader() As String
        Dim strID As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("SecurityRoleID")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.getSecurityRoleModulesWOHeader(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSecurityRoleInfo() As String
        Dim strID As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As New JsonDictionary()

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        Call New SecurityAccess().AddSecurityModules(Common.WebRequest.ProData)

        strID = Common.WebRequest.Data("SecurityRoleID")
        strReturn.Add("SecurityRoleData", clsSecurityRole.getSecurityRoleInfo(strID))

        strReturn.Add("IsSuperAdmin", IIf(Common.WebRequest.ProData.Execute("SELECT ID FROM LibrarySecurityRoleUser WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & " AND RoleID = '474f9f49-2876-4391-a005-107eb18f2762' --Role ID for Super Admin").Rows.Count > 0, True, False))

        Dim pSql As String

        If Not String.IsNullOrWhiteSpace(strID) Then
            pSql = "SELECT MainModuleName FROM LibrarySecurityModuleMain " & vbCrLf &
                    " INNER JOIN LibrarySecurityModuleMainPerRole ON LibrarySecurityModuleMainPerRole.ModuleMainID = LibrarySecurityModuleMain.ID " & vbCrLf &
                    " WHERE Status = 'Hide' AND RoleID = " & ReadyString(strID) & vbCrLf &
                    " GROUP BY MainModuleName "

            strReturn.Add("HiddenMenu", Common.WebRequest.ProData.Execute(pSql))
        End If

        Return strReturn.Serialize()

    End Function

    <HttpPost()>
    Function SaveSecurityRole() As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSecurityRole.ID = Common.WebRequest.Data("SecurityRoleID")
        clsSecurityRole.Code = Common.WebRequest.Data("Code")
        clsSecurityRole.Role = Common.WebRequest.Data("Role")
        clsSecurityRole.Description = Common.WebRequest.Data("Description")
        clsSecurityRole.IsActive = Common.WebRequest.Data("IsActive")
        clsSecurityRole.SecurityRoleModuleList = Common.WebRequest.AllData("SecurityRoleModule")
        clsSecurityRole.AccessMenu = Common.WebRequest.AllData("AccessMenu")

        WebSession.Instance.SecurityRoleID = clsSecurityRole.SaveSecurityRole()
        strReturn = JsonDictionary.Serialize("SecurityRoleData", WebSession.Instance.SecurityRoleID)

        Return strReturn

    End Function

    <HttpPost()>
    Function CheckUniqueSecurityRoleUser() As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSecurityRole.ID = ""
        clsSecurityRole.SecurityRoleID = WebSession.Instance.SecurityRoleID
        clsSecurityRole.PeopleID = Common.WebRequest.Data("PeopleID")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.CheckUniqueSecurityRoleUser())

        Return strReturn

    End Function

    <HttpPost()>
    Function getSecurityRoleUserList() As String
        Dim strID As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = WebSession.Instance.SecurityRoleID
        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.getSecurityRoleUserList(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function SaveSecurityRoleUser() As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSecurityRole.ID = ""
        clsSecurityRole.SecurityRoleID = WebSession.Instance.SecurityRoleID
        clsSecurityRole.PeopleID = Common.WebRequest.Data("PeopleID")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.SaveSecurityRoleUser())

        Return strReturn

    End Function

    <HttpPost()>
    Function RemoveSecurityRoleUser() As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSecurityRole.ID = Common.WebRequest.Data("SecRoleUserID")

        strReturn = JsonDictionary.Serialize("SecurityRoleData", IIf(clsSecurityRole.RemoveSecurityRoleUser(), "TRUE", "FALSE"))

        Return strReturn

    End Function

    <HttpPost()>
    Function CheckSecurityCurrentUserAcccess() As String
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String
        Dim strModuleName As String
        Dim strUserID As String
        Dim strSubscriberOrgID As String
        Dim pOut As New JsonDictionary

        WebSession.Instance.SecurityHasAccess = Nothing

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strSubscriberOrgID = WebRequest.Instance.OrgID
        '''            strUserID = IIf(Common.WebRequest.Data("UserID") = Nothing, IIf(WebSession.Instance.PeopleUserID = Nothing, "", WebSession.Instance.PeopleUserID), Common.WebRequest.Data("UserID"))
        strUserID = IIf(WebSession.Instance.PeopleUserID = Nothing, "", WebSession.Instance.PeopleUserID)
        strModuleName = Common.WebRequest.Data("ModuleName")
        If strModuleName = RequestApproveModule Then
            strModuleName = GetRequestApproveModuleName(Common.WebRequest.Data("FowstID"), strUserID)
        End If

        Dim pDT = clsSecurityRole.CheckSecurityCurrentUserAcccess(strModuleName, strSubscriberOrgID, strUserID)
        pOut.Add("SecurityRoleData", pDT)
        If pDT.Rows.Count = 0 Then
            pOut.Add("Label", clsSecurityRole.GetLabel(strModuleName))
        End If
        If CBool2(WebSession.Instance.SecurityHasAccess) Then
            pOut.Add("AllowedUserID", strUserID)
        End If

        Return pOut.Serialize()

    End Function

    <HttpPost()>
    Function CheckSecurityUserAcccess() As String
        Dim pOut As New JsonDictionary
        Dim clsSecurityRole As New SecurityRole
        Dim strReturn As String
        Dim clsPeople As New People
        Dim dtaTable As New DataTable
        Dim intCntr As Integer
        Dim strModuleName As String
        Dim strSubscriberOrgID As String
        Dim strUserID As String = ""

        WebSession.Instance.SecurityHasAccess = Nothing

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strModuleName = Common.WebRequest.Data("ModuleName")
        strSubscriberOrgID = WebRequest.Instance.OrgID
        clsPeople.UserName = Common.WebRequest.Data("Username")
        clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")
        dtaTable = clsPeople.CheckPeopleUserAccount()

        If (dtaTable.Rows.Count > 0) Then
            intCntr = 0
            Do Until (intCntr >= dtaTable.Rows.Count)

                If (ProPack.Security.Hash.Verify(clsPeople.UserPassword, dtaTable.Rows(intCntr).Item("UserPassword").ToString())) Then

                    strUserID = dtaTable.Rows(0).Item("PeopleID").ToString()
                    Exit Do

                End If

                intCntr = intCntr + 1
            Loop
        End If
        If strModuleName = RequestApproveModule Then
            strModuleName = GetRequestApproveModuleName(Common.WebRequest.Data("FowstID"), strUserID)
        End If

        pOut.Add("SecurityRoleData", clsSecurityRole.CheckSecurityCurrentUserAcccess(strModuleName, strSubscriberOrgID, strUserID))

        If CBool2(WebSession.Instance.SecurityHasAccess) Then
            pOut.Add("AllowedUserID", strUserID)
        End If

        Return pOut.Serialize()

    End Function

    <NonAction>
    Public Shared Function GetRequestApproveModuleName(vFowstID As String, vUserID As String) As String

        Dim pViewAll As Boolean
        Dim pViewGroup As Boolean
        Dim pViewBranch As Boolean
        Dim pDT As DataTable

        pViewAll = SecurityRole.Instance.HandleRoleAccess("View and Approve all requests", vUserID, True)
        pViewGroup = SecurityRole.Instance.HandleRoleAccess("View and Approve group requests", vUserID, True)
        pViewBranch = SecurityRole.Instance.HandleRoleAccess("View and Approve branch requests", vUserID, True)

        If pViewAll = False _
            AndAlso pViewGroup = False _
            AndAlso pViewBranch = False Then

            Return RequestApproveModule
        End If

        Common.WebRequest.ProData.BeginTransaction()
        WebRequest.Instance.Business.LibraryPeople.CreateTempTableAllowedViewRequestFowsts( _
            vKeywords:="",
            vAllowOrgsFowsts:=pViewAll,
            vAllowGroupFowsts:=pViewGroup,
            vAllowBranchFowsts:=pViewBranch,
            vUserID:=vUserID)
        pDT = Common.WebRequest.ProData.Execute("SELECT * FROM #tmpAllowedFowsts")
        Common.WebRequest.ProData.RollbackTransaction()

        If New DataView(pDT, "ID = " & ReadyString(vFowstID), "", DataViewRowState.CurrentRows).Count = 0 Then
            Return RequestApproveModule
        End If

        If pViewBranch Then Return "View and Approve branch requests"
        If pViewGroup Then Return "View and Approve group requests"
        If pViewAll Then Return "View and Approve all requests"

    End Function

    Function GetModulesWithNoAccess()
        Dim strReturn As String
        Dim clsSecurityRole As New SecurityRole
        Dim strID As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSecurityRole.PeopleID = WebSession.Instance.PeopleUserID

        strReturn = JsonDictionary.Serialize("Results", clsSecurityRole.GetModulesWithNoAccess())

        Return strReturn
    End Function

End Class
