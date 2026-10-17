Namespace WebProj
    Public Class SecurityRoleController
        Inherits System.Web.Mvc.Controller

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

            If (Common.WebRequest.Data("MainModule") = "POS") Then
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
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("SecurityRoleID")
            strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.getSecurityRoleInfo(strID))

            Return strReturn

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

            WebSession.Instance.SecurityHasAccess = Nothing

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strSubscriberOrgID = WebRequest.Instance.OrgID
            '''            strUserID = IIf(Common.WebRequest.Data("UserID") = Nothing, IIf(WebSession.Instance.PeopleUserID = Nothing, "", WebSession.Instance.PeopleUserID), Common.WebRequest.Data("UserID"))
            strUserID = IIf(WebSession.Instance.PeopleUserID = Nothing, "", WebSession.Instance.PeopleUserID)
            strModuleName = Common.WebRequest.Data("ModuleName")

            strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.CheckSecurityCurrentUserAcccess(strModuleName, strSubscriberOrgID, strUserID))

            Return strReturn

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

            strReturn = JsonDictionary.Serialize("SecurityRoleData", clsSecurityRole.CheckSecurityCurrentUserAcccess(strModuleName, strSubscriberOrgID, strUserID))
            Return strReturn

        End Function


    End Class

End Namespace