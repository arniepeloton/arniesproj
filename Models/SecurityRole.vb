Public Class SecurityRole
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property SecurityRoleID As String

    Public Property Code As String
    Public Property Role As String
    Public Property Description As String
    Public Property IsActive As String
    Public Property PeopleID As String

    Public Property SecurityRoleModuleList As Object()
    Public Property AccessMenu As Object()

    Public Shared ReadOnly Property Instance As SecurityRole
        Get
            Dim pSecurityRole As New SecurityRole
            Return pSecurityRole

        End Get
    End Property

    Public Function HandleRoleAccess(ByVal vModuleName As String,
        Optional vUserID As String = Nothing,
        Optional vDontRedirect As Boolean = False) As Boolean

        Dim strModuleName As String
        Dim strUserID As String = ""
        Dim strSubscriberOrgID As String

        'If WebSession.Instance.SecurityHasAccess <> "1" Then ' commented by Arvin.
        If WebRequest.Instance.HandleOrganization(True) = False Then Return False
        strSubscriberOrgID = WebRequest.Instance.OrgID
        If vUserID Is Nothing Then
            strUserID = WebSession.Instance.PeopleUserID
        Else
            strUserID = vUserID
        End If
        strModuleName = vModuleName

        If (CheckSecurityCurrentUserAcccess(strModuleName, strSubscriberOrgID, strUserID).Rows.Count <= 0) Then
            WebSession.Instance.SecurityHasAccess = Nothing
            If vDontRedirect = False Then
                Transfer("/SignIn")
            End If
            Return False
        End If
        'End If ' commented by Arvin.

        WebSession.Instance.SecurityHasAccess = Nothing

        Return True

    End Function

    Function CheckUniqueSecurityRoleCode() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrole.ID"
                End With
            End With

            .SqlFrom = " FROM dbo.LibrarySecurityRole secrole "

            .SqlWhere = "WHERE secrole.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND secrole.Code = " & ReadyString(Code)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function CheckUniqueSecurityRole() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrole.ID"
                End With
            End With

            .SqlFrom = " FROM dbo.LibrarySecurityRole secrole "

            .SqlWhere = "WHERE secrole.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND secrole.Role = " & ReadyString(Role)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getSecurityRoleList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrole.ID"
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "secrole.Code"
                End With
                With .Add("Role", SqlDbType.VarChar)
                    .Expression = "secrole.Role"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "secrole.Description"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "secrole.IsActive"
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "secrole.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = "FROM LibrarySecurityRole secrole " &
                        " LEFT JOIN dbo.MobizLibraryPeople ppol ON (secrole.UserID = ppol.ID)"

            .SqlWhere = "WHERE (secrole.IsCompleteRetail = 'True' OR secrole.IsCompleteRetail IS NULL)"

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY secrole.Role ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSecurityRoleModules(ByVal strID As String, ByVal strMainModuleID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 0

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrolemod.ID"
                End With
                With .Add("RoleID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrolemod.RoleID"
                End With
                With .Add("ModuleID", SqlDbType.UniqueIdentifier)
                    .Expression = "secmod.ID"
                End With
                With .Add("ModuleName", SqlDbType.VarChar)
                    .Expression = "secmod.ModuleName"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "secmod.Label"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "ISNULL(secmod.ModuleDescription, '')"
                End With
                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "secmod.SortOrder"
                End With
                With .Add("IsHeader", SqlDbType.Bit)
                    .Expression = "secmod.IsHeader"
                End With
                With .Add("IsHasAccess", SqlDbType.Bit)
                    .Expression = "ISNULL(secrolemod.IsHasAccess,0)"
                End With
            End With

            .SqlFrom = "FROM LibrarySecurityModule secmod " &
                " LEFT JOIN LibrarySecurityRoleModules secrolemod ON (secmod.ID = secrolemod.ModuleID " & IIf(strID.Trim().Length = 0, " AND secrolemod.RoleID IS NULL", " AND secrolemod.RoleID = " + ReadyString(strID)) & " ) "

            .SqlWhere = "WHERE secmod.HeaderID IS NULL " &
            " AND secmod.MainModuleID " & IIf(strMainModuleID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(strMainModuleID))

            .SqlOrder = "ORDER BY secmod.ModuleName ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSecurityRoleModulesChild(ByVal strID As String, ByVal strHeaderID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 0

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrolemod.ID"
                End With
                With .Add("RoleID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrolemod.RoleID"
                End With
                With .Add("ModuleID", SqlDbType.UniqueIdentifier)
                    .Expression = "secmod.ID"
                End With
                With .Add("ModuleName", SqlDbType.VarChar)
                    .Expression = "secmod.ModuleName"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "secmod.Label"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "ISNULL(secmod.ModuleDescription, '')"
                End With
                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "secmod.SortOrder"
                End With
                With .Add("IsHeader", SqlDbType.Bit)
                    .Expression = "secmod.IsHeader"
                End With
                With .Add("IsHasAccess", SqlDbType.Bit)
                    .Expression = "ISNULL(secrolemod.IsHasAccess,0)"
                End With
            End With

            .SqlFrom = "FROM LibrarySecurityModule secmod " &
                " LEFT JOIN LibrarySecurityRoleModules secrolemod ON (secmod.ID = secrolemod.ModuleID " & IIf(strID.Trim().Length = 0, " AND secrolemod.RoleID IS NULL", " AND secrolemod.RoleID = " + ReadyString(strID)) & " ) "

            .SqlWhere = "WHERE secmod.HeaderID " & IIf(strHeaderID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(strHeaderID))

            .SqlOrder = "ORDER BY secmod.ModuleName ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSecurityRoleModulesWOHeader(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 0 ' 0 = unlimited

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrolemod.ID"
                End With
                With .Add("RoleID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrolemod.RoleID"
                End With
                With .Add("ModuleID", SqlDbType.UniqueIdentifier)
                    .Expression = "secmod.ID"
                End With
                With .Add("ModuleName", SqlDbType.VarChar)
                    .Expression = "secmod.ModuleName"
                End With
                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "secmod.SortOrder"
                End With
                With .Add("IsHeader", SqlDbType.Bit)
                    .Expression = "secmod.IsHeader"
                End With
                With .Add("IsHasAccess", SqlDbType.Bit)
                    .Expression = "ISNULL(secrolemod.IsHasAccess,0)"
                End With
            End With

            .SqlFrom = "FROM LibrarySecurityModule secmod " &
                " LEFT JOIN LibrarySecurityRoleModules secrolemod ON (secmod.ID = secrolemod.ModuleID " & IIf(strID.Trim().Length = 0, " AND secrolemod.RoleID IS NULL", " AND secrolemod.RoleID = " + ReadyString(strID)) & " ) "

            .SqlWhere = "WHERE secmod.IsHeader = 0 "

            If CBool(Common.WebRequest.Data("IsCrm")) Then
                'Get modules for crm
                .SqlWhere += " AND (secmod.MainModuleID = '2055faf3-d090-4848-8081-f93ab93762d7' " & vbCrLf &
                                " OR secmod.MainModuleID = '2055faf3-d090-4848-8081-f93ab93762d8' " & vbCrLf &
                                " OR secmod.MainModuleID = '2055faf3-d090-4848-8081-f93ab93762d9' " & vbCrLf &
                                " OR secmod.MainModuleID = '2055faf3-d090-4848-8081-f93ab93762d6' " & vbCrLf &
                                " OR secmod.MainModuleID = '2055faf3-d090-4848-8081-f93ab93762d5') " & vbCrLf
            Else
                'Exclude crm modules
                .SqlWhere += " AND (secmod.MainModuleID <> '2055faf3-d090-4848-8081-f93ab93762d7' " & vbCrLf &
                                " AND secmod.MainModuleID <> '2055faf3-d090-4848-8081-f93ab93762d8' " & vbCrLf &
                                " AND secmod.MainModuleID <> '2055faf3-d090-4848-8081-f93ab93762d9' " & vbCrLf &
                                " AND secmod.MainModuleID <> '2055faf3-d090-4848-8081-f93ab93762d6' " & vbCrLf &
                                " AND secmod.MainModuleID <> '2055faf3-d090-4848-8081-f93ab93762d5') " & vbCrLf
            End If

            .SqlOrder = "ORDER BY secmod.ID ASC, secmod.SortOrder ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSecurityRoleInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secrole.ID"
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "secrole.Code"
                End With
                With .Add("Role", SqlDbType.VarChar)
                    .Expression = "secrole.Role"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "secrole.Description"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "secrole.IsActive"
                End With
            End With

            .SqlFrom = "FROM LibrarySecurityRole secrole "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE secrole.ID IS NULL", " WHERE secrole.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveSecurityRole() As String
        Dim p As New InsertUpdate("LibrarySecurityRole")
        Dim pSql As String
        Dim strIDReturn As String = ""
        Dim strIDRec As String = ""

        p("Code") = ReadyString(Code)
        p("Role") = ReadyString(Role)
        p("Description") = ReadyString(Description)
        p("IsActive") = IsActive
        p("IsDTR") = ReadyString("False")
        p("IsCompleteRetail") = ReadyString("True")
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            SecurityRoleID = strIDReturn

            '''Save Security Module List
            If (SecurityRoleModuleList.Length > 0) Then

                For Each objRow As Object In SecurityRoleModuleList

                    If (objRow("RecState").ToString().Trim().Length > 0) Then

                        If (objRow("RecState").ToString().Trim() <> "Remove") Then
                            p = New InsertUpdate("LibrarySecurityRoleModules")

                            p("RoleID") = ReadyString(strIDReturn)
                            p("ModuleID") = ReadyString(objRow("ModuleID").ToString())
                            p("IsHasAccess") = objRow("IsHasAccess").ToString()

                            If (objRow("ID").ToString().Trim().Length <= 0) Then
                                strIDRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDRec)
                                pSql = p.GetInsert()
                            Else
                                strIDRec = objRow("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else

                            pSql = "DELETE " &
                                   " FROM dbo.LibrarySecurityRoleModules " &
                                   " WHERE ID " & IIf(objRow("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objRow("ID").ToString().Trim()))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If

                    End If

                Next objRow

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibrarySecurityModuleMain'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibrarySecurityRole'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)

            Else
                pSql = "DELETE " &
                       " FROM dbo.LibrarySecurityRoleModules " &
                       " WHERE RoleID " & IIf(strIDReturn.Trim().Length = 0, "IS NULL", " = " & ReadyString(strIDReturn))

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            For Each menu As Object In AccessMenu
                p = New InsertUpdate("LibrarySecurityModuleMainPerRole")
                Dim mainmoduleid As String = Common.WebRequest.ProData.ExecuteScalar("SELECT ID FROM LibrarySecurityModuleMain WHERE MainModuleName = " & ReadyString(menu("Menu"))).ToString()
                p("ModuleMainID") = ReadyString(mainmoduleid)
                p("RoleID") = ReadyString(strIDReturn)
                p("Status") = ReadyString(menu("Status"))
                p("LastUpdated") = "getdate()"

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ModuleMainID = " & ReadyString(mainmoduleid) & " AND RoleID = " & ReadyString(strIDReturn))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If
            Next

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            AppendToDebug("Security Role error: " & ex.StackTrace.ToString)
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn
    End Function

    Function CheckUniqueSecurityRoleUser() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secroleusr.ID"
                End With
            End With

            .SqlFrom = " FROM dbo.LibrarySecurityRoleUser secroleusr "

            .SqlWhere = "WHERE secroleusr.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND secroleusr.RoleID = " & ReadyString(SecurityRoleID) &
                        " AND secroleusr.PeopleID = " & ReadyString(PeopleID)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getSecurityRoleUserList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "secuser.ID"
                    .Searchable = False
                End With
                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "secuser.PeopleID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolpresuffix.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
            End With
            .SqlFrom = "FROM LibrarySecurityRoleUser secuser " &
                    " INNER JOIN MobizLibraryPeople ppol ON (secuser.PeopleID = ppol.ID) " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolpresuffix ON (ppol.TitleID = ppolpresuffix.ID)"

            .SqlWhere = "WHERE secuser.RoleID = " & ReadyString(strID)

            .SqlOrder = "ORDER BY (ISNULL(ppol.SurName,'') + ', ' + ISNULL(ppol.FirstName,'') + ' ' + ISNULL(ppol.MiddleName,'')) ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveSecurityRoleUser() As String
        Dim p As New InsertUpdate("LibrarySecurityRoleUser")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String

        Try

            p("RoleID") = ReadyString(SecurityRoleID)
            p("PeopleID") = ReadyString(PeopleID)

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function RemoveSecurityRoleUser() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
               " FROM dbo.LibrarySecurityRoleUser " &
               " WHERE ID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function

    Function GetLabel(vModuleName As String) As String

        Dim pSql = "SELECT Label" & vbCrLf &
            " FROM LibrarySecurityModule" & vbCrLf &
            " WHERE ModuleName = " & ReadyString(vModuleName)
        Dim pOut = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
        If String.IsNullOrWhiteSpace(pOut) Then
            Return vModuleName
        Else
            Return pOut
        End If

    End Function

    Function CheckSecurityCurrentUserAcccess(ByVal strModule As String, ByVal strSubscriberOrgID As String, ByVal strUserID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("IsModule", SqlDbType.Bit)
                    .Expression = "md.IsModule"
                End With
                With .Add("IsFeature", SqlDbType.Bit)
                    .Expression = "md.IsFeature"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "md.Label"
                End With
            End With

            .SqlFrom = " FROM dbo.LibrarySecurityRoleUser usr " &
                " INNER JOIN dbo.LibrarySecurityRole r ON (usr.RoleID = r.ID) " &
                " INNER JOIN dbo.LibrarySecurityRoleModules rmd ON (rmd.RoleID = r.ID) " &
                " INNER JOIN dbo.LibrarySecurityModule md ON (rmd.ModuleID = md.ID) "

            .SqlWhere = " WHERE rmd.IshasAccess = 1 " &
                        " AND md.ModuleName = " & ReadyString(strModule) &
                        " AND usr.PeopleID = " & IIf(strUserID.Trim().Length > 0, ReadyString(strUserID), " NULL")

            .SqlWhere = " WHERE usr.PeopleID = " & IIf(strUserID.Trim().Length > 0, ReadyString(strUserID), " NULL") & vbCrLf &
                " AND" & vbCrLf &
                " (" & vbCrLf &
                "   (" & vbCrLf &
                "       rmd.IshasAccess = 1" & vbCrLf &
                "       AND md.ModuleName = " & ReadyString(strModule) & vbCrLf &
                "   )" & vbCrLf &
                "   OR " & vbCrLf &
                "   (" & vbCrLf &
                "     " & ReadyString(strModule) & " = 'System Settings'" & vbCrLf &
                "     AND r.Role = 'Super Admin'" & vbCrLf &
                "   )" & vbCrLf &
                " )" & vbCrLf

            pDT = .Requery
        End With

        WebSession.Instance.SecurityHasAccess = IIf(pDT.Rows.Count > 0, "1", "0")

        Return pDT

    End Function

    Public Function HasSuperAdminRole(vPeopleID As String) As Boolean

        Dim pSql = " SELECT DISTINCT LibrarySecurityRole.Role" & vbCrLf &
            " FROM LibrarySecurityRoleUser" & vbCrLf &
            " JOIN LibrarySecurityRole ON LibrarySecurityRole.ID = LibrarySecurityRoleUser.RoleID" & vbCrLf &
            " WHERE LibrarySecurityRole.Role = 'Super Admin'" & vbCrLf &
            " AND LibrarySecurityRoleUser.PeopleID = newid()" & vbCrLf &
            " ORDER BY LibrarySecurityRole.Role"

    End Function

    Public Function GetModulesWithNoAccess() As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT md.ModuleName " & vbCrLf &
                  "FROM dbo.LibrarySecurityRoleUser usr  INNER JOIN dbo.LibrarySecurityRole r  " & vbCrLf &
                  "ON (usr.RoleID = r.ID)  INNER JOIN dbo.LibrarySecurityRoleModules rmd  " & vbCrLf &
                  "ON (rmd.RoleID = r.ID)  INNER JOIN dbo.LibrarySecurityModule md ON (rmd.ModuleID = md.ID)    " & vbCrLf &
                  "WHERE usr.PeopleID = " & ReadyString(PeopleID) & vbCrLf &
                  " and rmd.IshasAccess = 0  " & vbCrLf

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function
End Class
