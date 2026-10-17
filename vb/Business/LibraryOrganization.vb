Imports System.Threading
Imports Aspose.Cells
Imports Newtonsoft.Json
'Imports PdfSharp.Pdf.Annotations
Imports WebProj.WebProj

Public Class LibraryOrganization
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ByID(vID As String,
        Optional vOnlineStoreName As String = Nothing,
        Optional vVendorPortalName As String = Nothing) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = " SELECT TOP 50 org.ID AS [ID]," & vbCrLf &
            " org.IsOrganization," & vbCrLf &
            "        (CASE org.IsOrganization" & vbCrLf &
            "           WHEN 1 THEN org.CompanyName" & vbCrLf &
            "           ELSE ISNULL(NULLIF (LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonSurname)) + ', ', ', '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonFirstName)) + ' ', ' '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonMiddleName)) + ' ', ' '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(org.PersonSuffixName)), ', '), '')" & vbCrLf &
            "         END) AS [CompanyName]" & vbCrLf &
            "         ,MobizLibrarySubscription.[Database]" & vbCrLf &
            ",ISNULL(MobizLibrarySubscription.FreeUser, 0) AS FreeUser" & vbCrLf &
            " ,MobizLibrarySubscription.LastChangeUserPeopleID" & vbCrLf &
            " ,MobizLibrarySubscription.LastChangeUserTime" & vbCrLf &
            " ,MobizLibrarySubscription.OnlineStoreName" & vbCrLf &
            " ,MobizLibrarySubscription.VendorPortalName" & vbCrLf &
            " FROM MobizLibraryOrganization org" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)" & vbCrLf &
            " LEFT JOIN MobizLibrarySubscription ON MobizLibrarySubscription.OrganizationID = org.ID" & vbCrLf
        If String.IsNullOrWhiteSpace(vVendorPortalName) = False Then
            pSql &= " WHERE MobizLibrarySubscription.VendorPortalName = " & ReadyString(vVendorPortalName)
        ElseIf String.IsNullOrWhiteSpace(vOnlineStoreName) = False Then
            pSql &= " WHERE MobizLibrarySubscription.OnlineStoreName = " & ReadyString(vOnlineStoreName)
        Else
            pSql &= " WHERE org.ID = " & ReadyString(vID)
        End If
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function CheckIfAutoApproveFranchisorOrFranchiseeRequest(vCompanyID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        'vCompanyID must have own database
        'vCompanyID either a Franchisor or Franchisee

        mProData.Execute("USE [" & RemoveSymbols(mProData.Execute("SELECT * FROM MobizLibrarySubscription WHERE OrganizationID = " & ReadyString(vCompanyID)).Rows(0)("Database")) & "]")

        pSql = "SELECT DefaultValue " &
            " FROM SystemDefaults" &
            " WHERE Description = " & ReadyString("AutoApproveFranchiseeRequests")
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)("DefaultValue") & ""
        Else
            Return ""
        End If

    End Function

    Public Sub SetOnlineStoreName(vID As String, vOnlineStoreName As String)

        Dim pIU As New InsertUpdate("MobizLibrarySubscription")
        pIU("OnlineStoreName") = ReadyString(vOnlineStoreName)
        Dim pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub SetVendorPortalName(vID As String, vOnlineStoreName As String)

        Dim pIU As New InsertUpdate("MobizLibrarySubscription")
        pIU("VendorPortalName") = ReadyString(vOnlineStoreName)
        Dim pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(vID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function ByFranchiseGroupID(vFranchiseGroupID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT org.ID AS [ID]," & vbCrLf &
           " org.IsOrganization," & vbCrLf &
           " 	(CASE org.IsOrganization" & vbCrLf &
           "     WHEN 1 THEN org.CompanyName" & vbCrLf &
           "     ELSE ISNULL(NULLIF (LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonSurname)) + ', ', ', '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonFirstName)) + ' ', ' '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonMiddleName)) + ' ', ' '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(org.PersonSuffixName)), ', '), '')" & vbCrLf &
           "     END) AS [CompanyName]" & vbCrLf &
           " ,MobizLibrarySubscription.[Database]" & vbCrLf &
           " ,ISNULL(MobizLibrarySubscription.FreeUser, 0) AS FreeUser" & vbCrLf &
           " ,MobizLibrarySubscription.LastChangeUserPeopleID" & vbCrLf &
           " ,MobizLibrarySubscription.LastChangeUserTime" & vbCrLf &
           " FROM MobizLibraryOrganization org" & vbCrLf &
           " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)" & vbCrLf &
           " LEFT JOIN MobizLibrarySubscription ON MobizLibrarySubscription.OrganizationID = org.ID" & vbCrLf &
           " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.OrganizationID = org.ID" & vbCrLf &
           " JOIN MobizLibraryOrganizationFranchisee ON MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
           " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(vFranchiseGroupID) & vbCrLf &
           " ORDER BY [Database], CompanyName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function IsValidCustomer(vID As String) As Boolean

        Dim pSql As String

        pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM LibraryOrganizationCustomer" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
            " WHERE LibraryOrganizationCustomer.OrganizationID = " & ReadyString(vID)

        Return mProData.ExecuteScalar(pSql) > 0

    End Function

    Public Sub DeleteDuplicateLibraryOrganizationEmployeeFOWST()

        Dim pDtDb As DataTable
        Dim pDT As DataTable
        Dim pSql As String
        Dim pProData As ProData
        Dim pCount As Integer ' pCount is for debugging only

        Const pSqlMain As String = " SELECT COUNT(*) AS Count" & vbCrLf &
            " ,LibraryOrganizationEmployeeFOWST.PeopleID" & vbCrLf &
            " ,LibraryOrganizationEmployeeFOWST.FowstID" & vbCrLf &
            " FROM LibraryOrganizationEmployeeFOWST" & vbCrLf &
            " GROUP BY LibraryOrganizationEmployeeFOWST.PeopleID" & vbCrLf &
            " ,LibraryOrganizationEmployeeFOWST.FowstID" & vbCrLf &
            " HAVING COUNT(*) > 1"

        Try
            pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
            pSql = "SELECT [Database]" & vbCrLf &
                   " FROM LibrarySubscription"
            pDtDb = pProData.Execute(pSql)
            For Each pDrDb As DataRow In pDtDb.Rows
                pSql = "USE " & EscapeSqlName(pDrDb("Database"))
                Try
                    pProData.ExecuteSQL(pSql)
                    Try
                        pCount = 0
                        pProData.BeginTransaction()
                        pDT = pProData.Execute(pSqlMain)
                        For Each pDR As DataRow In pDT.Rows
                            pSql = " SELECT TOP " & (pDR("Count") - 1) & " LibraryOrganizationEmployeeFOWST.ID" & vbCrLf &
                                " FROM LibraryOrganizationEmployeeFOWST" & vbCrLf &
                                " WHERE PeopleID = " & ReadyString(pDR("PeopleID")) & vbCrLf &
                                " AND FowstID = " & ReadyString(pDR("FowstID"))
                            pSql = "DELETE FROM LibraryOrganizationEmployeeFOWST" & vbCrLf &
                                " WHERE ID IN (" & vbCrLf &
                                pSql & ")"
                            pCount += pProData.ExecuteSQL(pSql) ' pCount is for debugging only
                        Next pDR
                        pProData.CommitTransaction()
                        If pCount > 0 Then
                            AppendToLog(pCount & " records deleted from " & pDrDb("Database") & ".LibraryOrganizationEmployeeFOWST")
                        End If
                    Catch ex As System.Exception
                        Stop2() ' tell Arvin
                        pProData.RollbackTransaction()
                    End Try
                Catch ex As System.Exception
                    ' maybe the database does not exist
                    Stop2()
                End Try
            Next pDrDb
        Catch ex As System.Exception
            Stop2() ' tell Arvin
        End Try

    End Sub

    Public Function GetAll(Optional vWithDatabaseOnly As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1000 org.ID AS [ID]," & vbCrLf &
            " org.IsOrganization," & vbCrLf &
            "        (CASE org.IsOrganization" & vbCrLf &
            "           WHEN 1 THEN org.CompanyName" & vbCrLf &
            "           ELSE ISNULL(NULLIF (LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonSurname)) + ', ', ', '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonFirstName)) + ' ', ' '), '') + ISNULL(NULLIF (LTRIM(RTRIM(org.PersonMiddleName)) + ' ', ' '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(org.PersonSuffixName)), ', '), '')" & vbCrLf &
            "         END) AS [CompanyName]" & vbCrLf &
            ",sys.databases.name AS [Database]" & vbCrLf &
            " FROM MobizLibraryOrganization org" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)" & vbCrLf &
            " JOIN MobizLibrarySubscription ON MobizLibrarySubscription.OrganizationID = org.ID" & vbCrLf &
            " LEFT JOIN sys.databases ON sys.databases.name COLLATE DATABASE_DEFAULT = MobizLibrarySubscription.[Database] COLLATE DATABASE_DEFAULT -- prevent this error: Cannot resolve the collation conflict between" & vbCrLf
        If vWithDatabaseOnly Then
            pSql &= " WHERE sys.databases.name IS NOT NULL" & vbCrLf
        End If

        pSql &= " ORDER BY CompanyName"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetByDatabase(vDatabase As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganization.ID" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            " JOIN MobizLibrarySubscription ON MobizLibrarySubscription.OrganizationID = MobizLibraryOrganization.ID" & vbCrLf &
            " WHERE MobizLibrarySubscription.[Database] = " & ReadyString(vDatabase)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetAllHaveFowst(Optional vOrgID As String = "") As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibrarySubscription.OrganizationID" & vbCrLf &
            " ,sys.databases.name AS DatabaseName" & vbCrLf &
            " FROM MobizLibrarySubscription" & vbCrLf &
            " JOIN sys.databases ON sys.databases.name COLLATE DATABASE_DEFAULT = MobizLibrarySubscription.[Database] COLLATE DATABASE_DEFAULT -- prevent this error: Cannot resolve the collation conflict between" & vbCrLf &
            " WHERE EXISTS (" & vbCrLf &
            " 	SELECT MobizLibraryOrganizationFOWST.*" & vbCrLf &
            "     FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            "     WHERE MobizLibraryOrganizationFOWST.OrganizationID = MobizLibrarySubscription.OrganizationID" & vbCrLf &
            " )" & vbCrLf
        If CStr2(vOrgID) <> "" Then
            pSql &= " AND MobizLibrarySubscription.OrganizationID = " & ReadyString(vOrgID)
        End If

        pSql &= " ORDER BY DatabaseName"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function AddFranchisor(vFranchisorOrgID As String,
        vFranchisorDatabase As String,
        vFranchiseeOrgFowstID As String)

        Dim pIU As InsertUpdate
        Dim pSql = "SELECT OrganizationID" & vbCrLf &
            " FROM MobizLibrarySubscription" & vbCrLf &
            " WHERE [Database] = " & ReadyString(vFranchisorDatabase)
        If mProData.ExecuteRow(pSql) Is Nothing Then
            pIU = New InsertUpdate("MobizLibrarySubscription")
            pIU("OrganizationID") = ReadyString(vFranchisorOrgID)
            pIU("[Database]") = ReadyString(vFranchisorDatabase)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        mProData.BeginTransaction()
        pSql = "SELECT MobizLibraryOrganizationFranchisee.ID" & vbCrLf &
            ",MobizLibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
            ",MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed" & vbCrLf &
            ",MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed" & vbCrLf &
            ",MobizLibraryOrganizationFranchisee.IsActive" & vbCrLf &
            " FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            " WHERE FranchiseeOrgFowstID = " & ReadyString(vFranchiseeOrgFowstID) & vbCrLf &
            " AND FranchisorOrgID = " & ReadyString(vFranchisorOrgID)
        Dim pDR = mProData.ExecuteRow(pSql)
        pIU = New InsertUpdate("MobizLibraryOrganizationFranchisee")
        Dim pFranchiseGroupID As String
        If pDR Is Nothing Then
            pIU("ID") = "NEWID()"
            pFranchiseGroupID = Guid.NewGuid().ToString()
            pIU("FranchiseGroupID") = ReadyString(pFranchiseGroupID)
            pIU("FranchiseeOrgFowstID") = ReadyString(vFranchiseeOrgFowstID)
            pIU("FranchisorOrgID") = ReadyString(vFranchisorOrgID)
            pIU("IsFranchisorConfirmed") = 1
            pIU("IsFranchiseeConfirmed") = 1
            pIU("IsActive") = 1
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pSql = pIU.GetInsert()
        Else
            pIU("IsFranchisorConfirmed") = 1
            pIU("IsFranchiseeConfirmed") = 1
            pIU("IsActive") = 1
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pFranchiseGroupID = CStr2(pDR("FranchiseGroupID"))
        End If
        mProData.ExecuteSQL(pSql)

        pDR = GetFranchiseGroup(vFranchisorDatabase, pFranchiseGroupID)
        If pDR Is Nothing Then
            pIU = New InsertUpdate(vFranchisorDatabase & ".dbo.LibraryFranchiseGroup")
            pIU("ID") = ReadyString(pFranchiseGroupID)
            pIU("GroupName") = ReadyString(DateTimeStamp(False))
            pIU("Description") = "'vbCUKCG44TPD!'"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        mProData.CommitTransaction()

    End Function

    Public Function GetFranchisors(vFranchiseeOrgID As String, Optional vFranchiseeOrgFowstID As String = Nothing) As DataTable

        Dim pOut As DataTable
        Dim pSql As String
        Dim pDrFranchiseGroup As DataRow

        pSql = "  SELECT " & vbCrLf &
            " MobizLibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
            " ,MobizLibraryOrganizationFranchisee.IsFranchisorConfirmed" & vbCrLf &
            " ,MobizLibraryOrganizationFranchisee.IsFranchiseeConfirmed" & vbCrLf &
            " ,MobizLibraryOrganizationFranchisee.IsActive" & vbCrLf &
            vbCrLf &
            " ,MobizLibraryOrganizationFranchisee.FranchisorOrgID " & vbCrLf &
            " ,MobizLibrarySubscription.[Database] AS FranchisorDatabase" & vbCrLf &
            " ,sys.databases.state AS FranchisorDatabaseState -- 0 = ONLINE; if null, db does not exist" & vbCrLf &
            "  " & vbCrLf &
            " ,tblFranchiseeOrg.ID AS FranchiseeOrgID" & vbCrLf &
            " ,tblFranchiseeSubcription.[Database] AS FranchiseeDatabase" & vbCrLf &
            " ,tblFranchiseeDatabases.state AS FranchiseeDatabaseState -- 0 = ONLINE; if null, db does not exist" & vbCrLf &
            " ,MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID" & vbCrLf &
            " FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = MobizLibraryOrganizationFranchisee.FranchisorOrgID" & vbCrLf &
            " LEFT JOIN MobizLibrarySubscription ON MobizLibrarySubscription.OrganizationID = MobizLibraryOrganizationFranchisee.FranchisorOrgID" & vbCrLf &
            " LEFT JOIN sys.databases ON sys.databases.name COLLATE DATABASE_DEFAULT = MobizLibrarySubscription.[Database] COLLATE DATABASE_DEFAULT -- prevent this error: Cannot resolve the collation conflict between" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization tblFranchiseeOrg ON tblFranchiseeOrg.ID = MobizLibraryOrganizationFOWST.OrganizationID" & vbCrLf &
            " LEFT JOIN MobizLibrarySubscription tblFranchiseeSubcription ON tblFranchiseeSubcription.OrganizationID = tblFranchiseeOrg.ID" & vbCrLf &
            " LEFT JOIN sys.databases tblFranchiseeDatabases ON tblFranchiseeDatabases.name COLLATE DATABASE_DEFAULT = tblFranchiseeSubcription.[Database] COLLATE DATABASE_DEFAULT -- prevent this error: Cannot resolve the collation conflict between" & vbCrLf &
            " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID IS NOT NULL" & vbCrLf &
            " AND MobizLibraryOrganizationFOWST.OrganizationID <> MobizLibraryOrganizationFranchisee.FranchisorOrgID " & vbCrLf &
            " AND tblFranchiseeOrg.ID = " & ReadyString(vFranchiseeOrgID) & vbCrLf
        If String.IsNullOrWhiteSpace(vFranchiseeOrgFowstID) = False Then
            pSql &= " AND MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = " & ReadyString(vFranchiseeOrgFowstID) & vbCrLf
        End If
        pSql &= " ORDER BY MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID"
        ' additional by janine condition not to include in copy invoice if franchisee is org's own fowst
        pOut = mProData.Execute(pSql)
            pOut.Columns.Add("IsValid", GetType(Boolean))
            pOut.Columns.Add("InvalidReason")
            'pOut.Columns.Add("FranchiseGroup", GetType(DataRow))
            pOut.Columns.Add("FranchisorFowstID")
            pOut.Columns.Add("FranchisorGroupName")
            pOut.Columns.Add("RegionalGroupID")
            pOut.Columns.Add("RegionName")

            For Each pDR As DataRow In pOut.Rows
                pDR("IsValid") = False
                If CBool2(pDR("IsFranchisorConfirmed")) = False Then
                    pDR("InvalidReason") = "IsFranchisorConfirmed must be true"
                    Continue For
                End If

                If CBool2(pDR("IsFranchiseeConfirmed")) = False Then
                    pDR("InvalidReason") = "IsFranchiseeConfirmed must be true"
                    Continue For
                End If

                If CBool2(pDR("IsActive")) = False Then
                    pDR("InvalidReason") = "IsActive must be true"
                    Continue For
                End If

                If CStr2(pDR("FranchisorDatabase")).Trim() = "" Then
                    pDR("InvalidReason") = "FranchisorDatabase must not be blank"
                    Continue For
                End If

                If pDR("FranchisorDatabase") Is DBNull.Value Then
                    pDR("InvalidReason") = "FranchisorDatabase must exist"
                    Continue For
                End If

                If pDR("FranchisorDatabaseState") Is DBNull.Value Then
                    pDR("InvalidReason") = "FranchisorDatabaseState must not be null"
                    Continue For
                End If

                If pDR("FranchisorDatabaseState") <> 0 Then
                    pDR("InvalidReason") = "FranchisorDatabaseState must be online"
                    Continue For
                End If

                If CStr2(pDR("FranchiseeDatabase")).Trim() = "" Then
                    pDR("InvalidReason") = "FranchiseeDatabase must not be blank"
                    Continue For
                End If

                If pDR("FranchiseeDatabase") Is DBNull.Value Then
                    pDR("InvalidReason") = "FranchiseeDatabase must exist"
                    Continue For
                End If

                If pDR("FranchiseeDatabaseState") Is DBNull.Value Then
                    pDR("InvalidReason") = "FranchiseeDatabaseState must not be null."
                    Continue For
                End If

                If pDR("FranchiseeDatabaseState") <> 0 Then
                    pDR("InvalidReason") = "FranchiseeDatabaseState must be online"
                    Continue For
                End If

                pDrFranchiseGroup = GetFranchiseGroup(pDR("FranchisorDatabase"), CStr2(pDR("FranchiseGroupID")))
                If pDrFranchiseGroup Is Nothing Then
                    pDR("InvalidReason") = "FranchiseGroup does not exist"
                    Continue For
                End If
                pDR("FranchisorFowstID") = CStr2(pDrFranchiseGroup("FranchisorFowstID"))
                pDR("FranchisorGroupName") = CStr2(pDrFranchiseGroup("GroupName"))
                pDR("RegionalGroupID") = CStr2(pDrFranchiseGroup("RegionalGroupID"))
                pDR("RegionName") = CStr2(pDrFranchiseGroup("RegionName"))

                pDR("IsValid") = True
            Next pDR

            Return pOut

    End Function

    Private Function GetFranchiseGroup(vDatabase As String, vGroupID As String) As DataRow
        ' PARAMS:
        '   vGroupID = MobizLibraryOrganizationFranchisee.FranchiseGroupID

        Dim pSql As String

        pSql = "SELECT LibraryFranchiseGroup.FranchisorFowstID" & vbCrLf &
            ",LibraryFranchiseGroup.GroupName" & vbCrLf &
            ",LibraryRegionalGroupSettingsDetails.RegionalGroupID" & vbCrLf &
            ",LibraryRegionalGroupSettings.RegionName" & vbCrLf &
            " FROM " & EscapeSqlName(vDatabase) & ".dbo.LibraryFranchiseGroup LibraryFranchiseGroup" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vDatabase) & ".dbo.LibraryRegionalGroupSettingsDetails LibraryRegionalGroupSettingsDetails ON LibraryRegionalGroupSettingsDetails.GroupID = LibraryFranchiseGroup.ID" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vDatabase) & ".dbo.LibraryRegionalGroupSettings LibraryRegionalGroupSettings ON LibraryRegionalGroupSettings.ID = LibraryRegionalGroupSettingsDetails.RegionalGroupID" & vbCrLf &
            " WHERE LibraryFranchiseGroup.ID = " & ReadyString(vGroupID)
        Try
            Dim pOut = mProData.ExecuteRow(pSql)
            If Now.Date = New Date(2022, 10, 27) Then
                AppendToLog("pSql=" & pSql, , "GetFranchiseGroup vbRH6WJM2MNW")
            End If
            Return pOut
        Catch ex As Exception
            AppendToLog("Error=" & ex.Message & vbCrLf &
                "pSql=" & pSql, , "GetFranchiseGroup vbRH6WJM2MNW")
            Throw
        End Try

    End Function

    Public Function Categories() As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = " SELECT DISTINCT MobizLibraryPIMProductVariableFeatures001.C0020 AS [Category]" & vbCrLf &
            " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            " JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
            " ORDER BY Category"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' get Darlington NetSuite Excel import
    ''' </summary>
    ''' <param name="vCompanyName"></param>
    ''' <param name="vNetSuiteID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOrAddByCompanyName(vCompanyName As String, vNetSuiteID As String,
        vAddAsCustomer As Boolean,
        vAddAsSupplier As Boolean,
        Optional vThirdPartyID As String = Nothing) As String

        Dim pSql As String
        Dim pDR As DataRow
        Dim pIU As InsertUpdate
        Dim pID As String

        ' --- NO NEED TO CREATE CUSTOMER PROFILE FOR ECOMMERCE USER, IT WILL AUTOMATICALLY TAG AS CUSTOMER ONCE HE/SHE VISIT THE CONFIRMATION LINK
        '--- COMMENTED BY ARNIE, 4-23-2020 10:00 AM
        ' uncommented 2022_05_13__14_03_49_9336
        pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            " WHERE CompanyName = " & ReadyString(vCompanyName)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryOrganization")
            pID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pID)
            pIU("CompanyName") = ReadyString(vCompanyName)
            pIU("NetSuiteID") = ReadyString(vNetSuiteID)
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Else
            pID = CStr2(pDR("ID"))
        End If

        If vAddAsCustomer Then
            AddAsCustomer(pID, vThirdPartyID)
        End If

        If vAddAsSupplier Then
            AddAsSupplier(pID)
        End If

        Return pID

    End Function

    Public Sub AddAsSupplier(vSupplierOrgID As String)

        Dim pSql As String
        Dim pDR As DataRow
        Dim pIU As InsertUpdate

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryOrganizationSupplier" & vbCrLf &
            " WHERE SupplierID = " & ReadyString(vSupplierOrgID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            pIU = New InsertUpdate("LibraryOrganizationSupplier")
            pIU("ID") = "NEWID()"
            pIU("SupplierID") = ReadyString(vSupplierOrgID)
            pIU("ActivationDate") = "getdate()"
            pIU("LastUpdated") = "getdate()"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Public Sub AddAsCustomer(vCustomerOrgID As String, Optional vThirdPartyID As String = Nothing)

        Dim pSql As String
        Dim pDR As DataRow
        Dim pIU As InsertUpdate

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryOrganizationCustomer" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vCustomerOrgID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            pIU = New InsertUpdate("LibraryOrganizationCustomer")
            pIU("OrganizationID") = ReadyString(vCustomerOrgID)
            If String.IsNullOrWhiteSpace(vThirdPartyID) = False Then
                pIU("ThirdPartyID") = ReadyString(vThirdPartyID)
            End If
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        AddRetailPriceToCustomer(vCustomerOrgID)
        AddSrpToCustomer(vCustomerOrgID)

    End Sub

    Public Sub AddRetailPriceToCustomer(vCustomerOrgID As String)

        Dim pPriceLevelID As String

        pPriceLevelID = mParent.SystemDefaults.GetRetailPriceID()
        AddPriceToCustomer(vCustomerOrgID, pPriceLevelID)

    End Sub

    Public Sub AddSrpToCustomer(vCustomerOrgID As String)

        Dim pPriceLevelID As String

        pPriceLevelID = mParent.SystemDefaults.GetSrpID()
        AddPriceToCustomer(vCustomerOrgID, pPriceLevelID)

    End Sub

    Public Sub AddPriceToCustomer(vCustomerOrgID As String, vPriceLevelID As String)

        Dim pSql As String
        Dim pDR As DataRow
        Dim pIU As InsertUpdate

        pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryOrganizationPriceLevel" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vCustomerOrgID) & vbCrLf &
            " AND PriceLevelID = " & ReadyString(vPriceLevelID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            pIU = New InsertUpdate("LibraryOrganizationPriceLevel")
            pIU("OrganizationID") = ReadyString(vCustomerOrgID)
            pIU("PriceLevelID") = ReadyString(vPriceLevelID)
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Public Function GetCustomerByName(vCompanyName As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " FROM LibraryOrganizationCustomer" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
            " WHERE MobizLibraryOrganization.CompanyName = " & ReadyString(vCompanyName)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetHeaderInfo(vID As String) As HeaderInfo

        Dim pOrgShort As OrganizationShort
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pContact As String
        Dim pContacts As String
        Dim pOut As HeaderInfo

        pOut = New HeaderInfo()
        pOrgShort = New OrganizationShort
        pDT = pOrgShort.GetProspectOrgDetails(vID)
        If pDT.Rows.Count > 0 Then
            pDR = pDT.Rows(0)
            pOut.CompanyName = CStr2(pDR("CompanyName"))
            pOut.CompanyAddress = CStr2(pDR("Address"))
            pOut.VatType = CStr2(pDR("VatType"))
        End If

        pDT = pOrgShort.GetProspectOrgTel(vID)
        For Each pDR In pDT.Rows
            pContact = CStr2(pDR("TelephoneNo"))
            If String.IsNullOrWhiteSpace(pContact) Then
                Continue For
            End If
            If String.IsNullOrWhiteSpace(pContacts) = False Then
                pContacts &= "; "
            End If
            pContacts &= pContact
        Next pDR

        pDT = pOrgShort.GetProspectOrgMobile(vID)
        For Each pDR In pDT.Rows
            pContact = CStr2(pDR("CallingCode")) & CStr2(pDR("Prefix")) & CStr2(pDR("MobileNo"))
            If String.IsNullOrWhiteSpace(pContact) Then
                Continue For
            End If
            If String.IsNullOrWhiteSpace(pContacts) = False Then
                pContacts &= "; "
            End If
            pContacts &= pContact
        Next pDR

        pDT = pOrgShort.GetProspectOrgEmail(vID)
        For Each pDR In pDT.Rows
            pContact = CStr2(pDR("EmailAdd"))
            If String.IsNullOrWhiteSpace(pContact) Then
                Continue For
            End If
            If String.IsNullOrWhiteSpace(pContacts) = False Then
                pContacts &= "; "
            End If
            pContacts &= pContact
        Next pDR

        pOut.ContactInfos = pContacts

        Return pOut

    End Function

    Public Class HeaderInfo
        Public CompanyName As String
        Public CompanyAddress As String
        Public ContactInfos As String
        Public VatType As String
    End Class

    Public Class RankedName
        Public Rank As Integer
        Public Name As String
    End Class

    Public Function AddBanksIfNeeded(vRankedNames As List(Of RankedName)) As Integer

        Dim pOut As Integer

        Dim pSql = "UPDATE MobizLibraryOrganization" & vbCrLf &
            " SET IsABank = 0"
        mProData.ExecuteSQL(pSql)

        For Each pRankedName In vRankedNames
            If AddBankIfNeeded(pRankedName.Name, pRankedName.Rank) Then
                pOut += 1
            End If
        Next pRankedName

        Return pOut

    End Function

    Private Function AddBankIfNeeded(vName As String, vRank As Integer) As Boolean

        Dim pIU As InsertUpdate
        Dim pDR As DataRow
        Dim pSql As String

        vName = vName.Trim()
        pIU = New InsertUpdate("MobizLibraryOrganization")
        pIU("CompanyName") = ReadyString(vName)
        pIU("IsABank") = 1
        pIU("BankRank") = vRank
        pDR = GetBankByName(vName)

        If pDR Is Nothing Then
            pIU("ID") = "NEWID()"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            Return True
        ElseIf CBool2(pDR("IsABank")) = False _
            OrElse CInt2(pDR("BankRank")) <> vRank Then

            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
            Return True
        End If

    End Function

    Private Function GetBankByName(vName As String) As DataRow

        Dim pSql = " SELECT ID" & vbCrLf &
            " ,IsABank" & vbCrLf &
            " ,BankRank" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            " WHERE LTRIM(RTRIM(CompanyName)) = " & ReadyString(vName.Trim())
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetAllCustomers() As DataTable

        Dim pSelect As String
        Dim pFrom As String

        BrowserController.ComposeCustomer(pSelect, pFrom)
        Dim pSql = "SELECT DISTINCT MobizLibraryOrganization.ID" & vbCrLf &
            "," & pSelect & " AS CompanyName" & vbCrLf &
            ",MobizLibraryOrganization.IsActive" & vbCrLf &
            ",LibraryOrganizationCustomer.LastUpdated" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            pFrom & vbCrLf &
            " JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = MobizLibraryOrganization.ID" & vbCrLf &
            " WHERE ISNULL(LibraryOrganizationCustomer.Deactivated, 0) = 0" & vbCrLf &
            " ORDER BY CompanyName"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetAllSuppliers() As DataTable

        Dim pSelect As String
        Dim pFrom As String

        BrowserController.ComposeCustomer(pSelect, pFrom)
        Dim pSql = "SELECT DISTINCT MobizLibraryOrganization.ID" & vbCrLf &
            "," & pSelect & " AS CompanyName" & vbCrLf &
            ",MobizLibraryOrganization.IsActive" & vbCrLf &
            ",LibraryOrganizationSupplier.LastUpdated" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            pFrom & vbCrLf &
            " JOIN LibraryOrganizationSupplier ON LibraryOrganizationSupplier.SupplierID = MobizLibraryOrganization.ID" & vbCrLf &
            " WHERE ISNULL(LibraryOrganizationSupplier.Deactivated, 0) = 0" & vbCrLf &
            " ORDER BY CompanyName"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Class SimilarOrganizationsResult

        Public DateStarted As DateTime
        Public DateUpdated As DateTime
        Public DateEnded As DateTime?
        Public Busy As Boolean
        Public Items As New ItemsClass()

        Public Class ItemsClass
            Inherits List(Of ItemClass)

            Public Function Add(vOrgID As String, vCompanyName As String, vLastUpdated As String) As ItemClass

                Dim pOut = New ItemClass
                pOut.OrgID = vOrgID
                pOut.CompanyName = vCompanyName
                pOut.LastUpdated = vLastUpdated
                MyBase.Add(pOut)

                Return pOut

            End Function
        End Class

        Public Class ItemClass
            Public OrgID As String
            Public CompanyName As String
            Public LastUpdated As String

            Public Duplicates As New ItemsClass

            Public Sub New()

            End Sub

            Public Sub New(vOrgID As String, vCompanyName As String)

                OrgID = vOrgID
                CompanyName = vCompanyName

            End Sub
        End Class
    End Class

    Private Shared ReadOnly mSemaphoreSimilarOrganizations As New SemaphoreSlim(1)
    Private Shared mCancelSimilarOrganizations As Boolean

    Private Shared mSimilarOrganizationsFilename As String
    Private Shared Function SimilarOrganizationsFilename() As String

        If mSimilarOrganizationsFilename Is Nothing Then
            mSimilarOrganizationsFilename = Common.ServerPath & "temp\SimilarOrganizations"
        End If

        Return mSimilarOrganizationsFilename

    End Function

    Private Shared ReadOnly mSimilarOrganizationsFileLock = New Object()

    Private Shared Function LoadSimilarOrganizationsFile() As SimilarOrganizationsResult

        SyncLock mSimilarOrganizationsFileLock
            Try
                If IO.File.Exists(SimilarOrganizationsFilename()) Then
                    Dim pText = FileText(SimilarOrganizationsFilename())
                    Dim pOut = JsonConvert.DeserializeObject(Of SimilarOrganizationsResult)(pText)
                    Return pOut
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End SyncLock

    End Function

    Private Shared Sub SaveSimilarOrganizationsFile(vInput As SimilarOrganizationsResult)

        SyncLock mSimilarOrganizationsFileLock
            vInput.DateUpdated = Now
            Dim pText = JsonConvert.SerializeObject(vInput, Formatting.Indented)
            FileText(SimilarOrganizationsFilename()) = pText
        End SyncLock

    End Sub

    Public Shared Sub DeleteSimilarOrganizationsFile()

        SyncLock mSimilarOrganizationsFileLock
            Try
                If IO.File.Exists(SimilarOrganizationsFilename()) Then
                    mCancelSimilarOrganizations = True
                    mSemaphoreSimilarOrganizations.Wait()
                    mSemaphoreSimilarOrganizations.Release()
                    IO.File.Delete(SimilarOrganizationsFilename())
                End If
            Catch ex As Exception
                Console.WriteLine(ex.Message) ' just ignore any errors here
            Finally
                mCancelSimilarOrganizations = False
            End Try
        End SyncLock

    End Sub

    Public Shared Sub CancelSimilarOrganizations()

        Try
            mCancelSimilarOrganizations = True
            mSemaphoreSimilarOrganizations.Wait()
            mSemaphoreSimilarOrganizations.Release()
        Catch ex As Exception
        Finally
            mCancelSimilarOrganizations = False
        End Try

    End Sub

    Public Shared Function GetSimilarOrganizations(vDT As DataTable) As SimilarOrganizationsResult

        Dim pFromFile = LoadSimilarOrganizationsFile()
        If pFromFile IsNot Nothing Then
            If mSemaphoreSimilarOrganizations.CurrentCount = 0 Then
                pFromFile.Busy = True
            End If
            Return pFromFile
        End If

        If mSemaphoreSimilarOrganizations.Wait(2000) = False Then
            Throw New ApplicationException("Another thread is processing GetSimilarOrganizations")
        End If

        Dim pOut = New SimilarOrganizationsResult()
        pOut.DateStarted = Now
        pOut.DateUpdated = Now
        Dim pCurrentResult As SimilarOrganizationsResult.ItemClass
        Dim pAllIDs As New List(Of String)

        Dim pSwSave = Stopwatch.StartNew()
        Const pSaveInterval As Integer = 5000 ' in milliseconds
        Try
            For pIndex1 = 0 To vDT.Rows.Count - 2
                If mCancelSimilarOrganizations Then
                    Exit For
                End If
                If pSwSave.ElapsedMilliseconds > pSaveInterval Then
                    SaveSimilarOrganizationsFile(pOut)
                    pSwSave.Restart()
                End If
                If pAllIDs.Contains(CStr2(vDT.Rows(pIndex1)("ID"))) Then
                    Continue For
                End If
                pCurrentResult = Nothing
                For pIndex2 = pIndex1 + 1 To vDT.Rows.Count - 1
                    If mCancelSimilarOrganizations Then
                        Exit For
                    End If
                    Dim pCompanyName1 = CStr2(vDT.Rows(pIndex1)("CompanyName"))
                    Dim pCompanyName2 = CStr2(vDT.Rows(pIndex2)("CompanyName"))
                    Dim pDistancePercent = LevenshteinDistancePercent(pCompanyName1, pCompanyName2)
                    If pDistancePercent <= 10 Then
                        If pCurrentResult Is Nothing Then
                            pCurrentResult = pOut.Items.Add(CStr2(vDT.Rows(pIndex1)("ID")), pCompanyName1, CStr2(vDT.Rows(pIndex1)("LastUpdated")))
                            pAllIDs.Add(CStr2(vDT.Rows(pIndex1)("ID")))
                        End If
                        pCurrentResult.Duplicates.Add(CStr2(vDT.Rows(pIndex2)("ID")), pCompanyName2, CStr2(vDT.Rows(pIndex2)("LastUpdated")))
                        pAllIDs.Add(CStr2(vDT.Rows(pIndex2)("ID")))
                    End If
                    If pSwSave.ElapsedMilliseconds > pSaveInterval Then
                        SaveSimilarOrganizationsFile(pOut)
                        pSwSave.Restart()
                    End If
                Next pIndex2
                If mCancelSimilarOrganizations Then
                    Exit For
                End If
            Next pIndex1

            pOut.DateEnded = Now
            SaveSimilarOrganizationsFile(pOut)

            Return pOut
        Catch ex As Exception

        Finally
            mSemaphoreSimilarOrganizations.Release()
        End Try

    End Function

    '    Public Function GetSimilarOrganizationsToExcel() As String

    '        Dim pTemplateFilename = Common.ServerPath & "FileTemplates\SimilarOrganizations.xlsx"
    '        Dim pWS = GetFirstExcelWorksheet(pTemplateFilename)
    '        Dim pDT = GetAllOrganizations()

    '#If DEBUG Then
    '        Dim pDV = ProPack.Utility.GetTopRecords(pDT.DefaultView, 10000)
    '        pDT = pDV.ToTable()
    '#End If

    '        Dim pSimilar = GetSimilarOrganizations(pDT)
    '        Dim pRowIndex = 0

    '        For Each pItem In pSimilar.Items.Values
    '            pRowIndex += 2
    '            pWS.Cells(pRowIndex, 0).PutValue("To be retained:")
    '            pWS.Cells(pRowIndex, 1).PutValue(pItem.OrgID)
    '            pWS.Cells(pRowIndex, 2).PutValue(pItem.CompanyName)

    '            For Each pDuplicate In pItem.Duplicates.Values
    '                pRowIndex += 1
    '                pWS.Cells(pRowIndex, 0).PutValue("Duplicates to be deactivated:")
    '                pWS.Cells(pRowIndex, 1).PutValue(pDuplicate.OrgID)
    '                pWS.Cells(pRowIndex, 2).PutValue(pDuplicate.CompanyName)
    '            Next pDuplicate
    '        Next pItem

    '        Dim pDestinationFilename = Common.ServerPath & "temp\SimilarOrganizations\SimilarOrganizations " & DateTimeStamp(False) & ".xlsx"
    '        CreateFolder(pDestinationFilename)
    '        pWS.Workbook.Save(pDestinationFilename)

    '        Return pDestinationFilename

    '    End Function

    ''' <summary>
    ''' Compute LevenshteinDistance.
    ''' </summary>
    Public Shared Function LevenshteinDistance(ByVal s As String,
                                        ByVal t As String) As Integer
        ' https://www.dotnetperls.com/levenshtein-vbnet
        Dim n As Integer = s.Length
        Dim m As Integer = t.Length
        Dim d(n + 1, m + 1) As Integer

        If n = 0 Then
            Return m
        End If

        If m = 0 Then
            Return n
        End If

        Dim i As Integer
        Dim j As Integer

        For i = 0 To n
            d(i, 0) = i
        Next

        For j = 0 To m
            d(0, j) = j
        Next

        For i = 1 To n
            For j = 1 To m

                Dim cost As Integer
                If t(j - 1) = s(i - 1) Then
                    cost = 0
                Else
                    cost = 1
                End If

                d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1),
                                   d(i - 1, j - 1) + cost)
            Next
        Next

        Return d(n, m)
    End Function

    Public Shared Function LevenshteinDistancePercent(
        ByVal s As String,
        ByVal t As String) As Integer

        Dim pLength = Math.Max(s.Length, t.Length)
        If pLength = 0 Then
            Return 0
        End If
        Dim pDistance = LevenshteinDistance(s, t)
        Dim pOut = Math.Round(pDistance / pLength * 100, 0)

        Return pOut

    End Function

    Public Sub DeleteTransImportsData(Optional ByRef oLogs As String = Nothing)

        Dim pProData As ProData
        Dim pDtDb As DataTable
        Dim pSql As String
        Dim pErrorOnTruncate As String = ""
        Dim pErrorOnConnect As String = ""
        Dim pSuccess As String = ""

        pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
        pSql = "SELECT [Database]" & vbCrLf &
               " FROM LibrarySubscription"
        pDtDb = pProData.Execute(pSql)
        For Each pDrDb As DataRow In pDtDb.Rows
            pSql = "USE " & EscapeSqlName(pDrDb("Database"))
            Try
                pProData.ExecuteSQL(pSql)
                Try
                    pSql = "TRUNCATE TABLE TransImports"
                    pProData.ExecuteSQL(pSql)
                    pSuccess &= "TransImports was successfully truncated in " & EscapeSqlName(pDrDb("Database")) & vbCrLf
                Catch ex As Exception
                    pErrorOnTruncate &= "An error occured while trying to truncate TransImports in " & EscapeSqlName(pDrDb("Database")) & ": " & ex.Message & vbCrLf
                End Try
            Catch ex As System.Exception
                ' maybe the database does not exist
                pErrorOnConnect &= "An error occured while trying to connect to " & EscapeSqlName(pDrDb("Database")) & ": " & ex.Message & vbCrLf
            End Try
        Next pDrDb

        oLogs = pErrorOnTruncate & vbCrLf &
            pErrorOnConnect & vbCrLf &
            pSuccess

    End Sub

    Public Class SaveLogInput
        Public DeviceTime As String
        Public DeviceUuid As String
        Public LogText As String
    End Class

    Public Sub SaveDeviceLog(vInput As SaveLogInput)

        Dim pIU = New InsertUpdate("TransDeviceLogs")
        Dim pDeviceDate = ParseDate(vInput.DeviceTime)
        If pDeviceDate IsNot Nothing Then
            pIU("DeviceTime") = ReadyString(Format(pDeviceDate, cGlobal.FormatDateTimeSql))
        End If
        pIU("DeviceUuid") = ReadyString(Truncate(CStr2(vInput.DeviceUuid).Trim(), 100))
        pIU("LogText") = ReadyString(Truncate(CStr2(vInput.LogText).Trim(), 4000))
        pIU("ServerTime") = "GETDATE()"
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function RestoreOrganizationTin() As Integer

        Dim pUpdated As Integer

        Dim pSql = "SELECT MobizLibraryOrganization.ID" & vbCrLf &
            " ,MobizLibraryOrganization.TinNumber" & vbCrLf &
            " ,MobizLibraryOrganization.TinNumberBackup" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf &
            " WHERE ISNULL(MobizLibraryOrganization.TinNumber, '') <> ISNULL(MobizLibraryOrganization.TinNumberBackup, '')" & vbCrLf &
            " AND LEN(ISNULL(MobizLibraryOrganization.TinNumberBackup, '')) >= 9"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            If CStr2(pDR("TinNumberBackup")).Length >= 9 Then
                If pDR("TinNumberBackup") <> CStr2(pDR("TinNumber")) Then
                    Dim pIU = New InsertUpdate("MobizLibraryOrganization")
                    pIU("TinNumber") = ReadyString(pDR("TinNumberBackup"))
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                    mProData.ExecuteSQL(pSql)
                End If
                pUpdated = 1
            End If
        Next

        Return pUpdated

    End Function

    Public Sub ExportToXmlZip(vOrgID As String)

        Dim pProZip As New ProZip
        Dim pOrgIds As String
        Dim pSqlWhere = " WHERE MobizLibraryOrganization.ID = " & ReadyString(vOrgID) & vbCrLf
        Dim pDT = mProData.Execute("SELECT * FROM MobizLibraryOrganization" & vbCrLf &
            pSqlWhere &
            " ORDER BY MobizLibraryOrganization.LastUpdated")
        If pDT.Rows.Count > 0 Then
            pOrgIds = CStr2(ExtractFieldValues(pDT, "ID", Integer.MaxValue)(0)("ID"))
            pDT.TableName = "MobizLibraryOrganization"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT LibraryOrganizationCustomer.* " +
                " FROM LibraryOrganizationCustomer" +
                " WHERE LibraryOrganizationCustomer.OrganizationID IN (" + pOrgIds + ")" +
                " ORDER BY LibraryOrganizationCustomer.ID")
            pDT.TableName = "LibraryOrganizationCustomer"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT LibraryOrganizationPriceLevel.* " +
                " FROM LibraryOrganizationPriceLevel" +
                " WHERE LibraryOrganizationPriceLevel.OrganizationID IN (" + pOrgIds + ")" +
                " ORDER BY LibraryOrganizationPriceLevel.ID")
            pDT.TableName = "LibraryOrganizationPriceLevel"
            pProZip.AddTable(pDT)

            pDT = mProData.Execute("SELECT MobizLibraryOrganizationFOWST.* " +
                " FROM MobizLibraryOrganizationFOWST" +
                " WHERE MobizLibraryOrganizationFOWST.OrganizationID IN (" + pOrgIds + ")" +
                " ORDER BY MobizLibraryOrganizationFOWST.ID")
            pDT.TableName = "MobizLibraryOrganizationFOWST"
            pProZip.AddTable(pDT)
        End If

        Dim pFilename As String = Common.ServerPath & "temp\ExportedOrganization\ExportedOrganization_" & ReadyGuid(vOrgID) & ".zip"
        CreateFolder(pFilename)
        If IO.File.Exists(pFilename) Then
            IO.File.Delete(pFilename)
        End If
        pProZip.Save(pFilename)

    End Sub

    Public Function CustomerCreditTerms(vIncludeApproved As Boolean, vIncludeUnapproved As Boolean) As DataTable

        Dim pCustomerSelectSql As String
        Dim pCustomerFromSql As String

        BrowserController.ComposeCustomer(pCustomerSelectSql, pCustomerFromSql)

        Dim pSql = "SELECT --ISNULL(convert(varchar(MAX), lvl.ID), '') as PriceLevelID, ISNULL(lvl.PriceLevel, '') as PriceLevelName, " & vbCrLf &
                "ISNULL(convert(varchar(MAX), ppol.ID), '') as SalesRepID, " & vbCrLf &
                "ISNULL(ppol.Surname + ', ', '') + ISNULL(ppol.FirstName + ' ', '') + ISNULL(ppol.MiddleName + ' ', '') as SalesRepName, " & vbCrLf &
                "ISNULL(convert(varchar(MAX), term.ID), '') as TermID, " & vbCrLf &
                "ISNULL(term.Name, '') as TermName, " & vbCrLf &
                "ISNULL(cus.CreditLimit,'') as CreditLimit, " & vbCrLf &
                "ISNULL(cus.CreditBegBalance,'') as CreditBegBalance, " & vbCrLf &
                "ISNULL(CONVERT(varchar(MAX), cus.CreditBalanceAsOf), '') as CreditAsOf " & vbCrLf &
                " ,cus.ThirdPartyBegBalance" & vbCrLf &
                ",ISNULL(CONVERT(varchar(MAX), cus.ThirdPartyBegBalanceAsOf, 126), '') as ThirdPartyBegBalanceAsOf " & vbCrLf &
                ",ISNULL(cus.UnlimitedCredit, 0) AS UnlimitedCredit" & vbCrLf &
                ",ISNULL(cus.RequireCnCApproval, 0) AS RequireCnCApproval" & vbCrLf &
                ",ISNULL(cus.CustomerRequirePoNum, 0) AS CustomerRequirePoNum" & vbCrLf &
                ",cus.ID, cus.FowstID, fowst.Name as FowstName " & vbCrLf &
                ",ISNULL(enc.Surname + ', ', '') + ISNULL(enc.FirstName + ' ', '') + ISNULL(enc.MiddleName + ' ', '') as EncodedByName " & vbCrLf &
                ",ISNULL(app.Surname + ', ', '') + ISNULL(app.FirstName + ' ', '') + ISNULL(app.MiddleName + ' ', '') as ApprovedByName " & vbCrLf &
                ", " & pCustomerSelectSql & " AS CustomerName" & vbCrLf &
                ",cus.OrganizationID AS CustomerID" & vbCrLf

        pSql &= " FROM LibraryOrganizationCustomerCreditTerms cus" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = cus.OrganizationID" & vbCrLf &
            pCustomerFromSql

        pSql &= " --LEFT JOIN MobizLibraryPIMPriceLevel as lvl ON lvl.ID = cus.PriceLevelID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = cus.SalesRepID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as enc ON enc.ID = cus.EncodedBy " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as app ON app.ID = cus.ApprovedBy " & vbCrLf &
                " LEFT JOIN LibraryPaymentTerms as term ON term.ID = cus.CreditTermsID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = cus.FowstID " & vbCrLf &
                " WHERE 1=1" & vbCrLf
        If vIncludeApproved Then
            pSql &= " AND cus.ApprovedBy IS NOT NULL" & vbCrLf
        End If
        If vIncludeUnapproved Then
            pSql &= " AND cus.ApprovedBy IS NULL" & vbCrLf
        End If
        Dim pDT = mProData.Execute(pSql)
        Return pDT

    End Function
End Class