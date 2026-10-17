Imports System.Linq

Public Class LibraryPeople
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetData(vPeopleID As String, vOrgID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT IsTerminated" & vbCrLf &
            " FROM MobizLibraryPeopleOrganization" & vbCrLf &
            " WHERE PeopleID = " & ReadyString(vPeopleID) & vbCrLf &
            " AND OrganizationID = " & ReadyString(vOrgID)

        Return mProData.ExecuteRow(pSql)

    End Function

    Public Function GetOrCreatePeople(vSurname As String, vFirstName As String, vMiddleName As String) As String

        vSurname = Truncate(vSurname, 50)
        vFirstName = Truncate(vFirstName, 50)
        vMiddleName = Truncate(vMiddleName, 50)

        Dim pSql = "SELECT ID " & vbCrLf &
            " FROM MobizLibraryPeople" & vbCrLf &
            " WHERE Surname = " & ReadyString(vSurname) & vbCrLf &
            " AND FirstName = " & ReadyString(vFirstName) & vbCrLf &
            " AND MiddleName = " & ReadyString(vMiddleName)
        Dim pID = CStr2(mProData.ExecuteScalar(pSql))
        If IsGuid(pID) Then
            Return pID
        End If

        pID = Guid.NewGuid().ToString()
        Dim pIU = New InsertUpdate("MobizLibraryPeople")
        pIU("ID") = ReadyString(pID)
        pIU("Surname") = ReadyString(vSurname)
        pIU("FirstName") = ReadyString(vFirstName)
        pIU("MiddleName") = ReadyString(vMiddleName)
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    ''' <summary>
    ''' requires HandleOrganization
    ''' </summary>
    ''' <param name="vPeopleID"></param>
    ''' <param name="vOrgID"></param>
    ''' <param name="vFowstID"></param>
    ''' <param name="vDeviceID"></param>
    ''' <param name="vDeviceUuid"></param>
    ''' <param name="oError"></param>
    ''' <param name="vDontVerifyFowst">if true, vFowstID is ignored</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Validate(ByVal vPeopleID As String, ByVal vOrgID As String, ByVal vFowstID As String, ByVal vPosMode As String, ByVal vDeviceID As String, ByVal vDeviceUuid As String, ByRef oError As String,
        ByVal vVerifyUuid As Boolean, Optional vDontVerifyPeopleFowst As Boolean = False,
        Optional vDontVerifyFowst As Boolean = False,
        Optional vDontVerifyOrgEmployee As Boolean = False) As Boolean

        Dim pDR As DataRow
        Dim pDrDevice As DataRow
        Dim pSql As String

        oError = Nothing
        pDR = GetData(vPeopleID, vOrgID)  ' MobizLibraryPeopleOrganization
        If pDR Is Nothing Then
            AppendToLog("The specified PeopleID does not exist on the specified OrgID. PeopleID = " & vPeopleID & "; OrgID = " & vOrgID & ". vbPJ859ZMZNM!", , "PeopleOrgMissing")
            'oError = ""
            'Return False
        Else
            If CDec2(pDR("IsTerminated")) <> 0 Then
                AppendToLog("The specified PeopleID was already terminated. PeopleID = " & vPeopleID & "; OrgID = " & vOrgID & ". vbPYN7MSNBJP!", , "PeopleTerminated")
            End If
        End If

        If vVerifyUuid Then
            pDrDevice = GetDeviceByUuid(vDeviceUuid) ' LibraryDevice
            If pDrDevice Is Nothing Then
                vDeviceUuid = CStr2(vDeviceUuid).Trim()
                pDrDevice = GetDeviceByUuid(vDeviceUuid) ' LibraryDevice
                If pDrDevice Is Nothing Then
                    oError = "Device with UUID " & vDeviceUuid & " does not exist on the current database."
                    Return False
                End If
            End If

            If CDec2(pDrDevice("IsActive")) = 0 Then
                Try
                    AppendToLog("vDeviceUuid: " & vDeviceUuid & vbCrLf &
                        "DeviceID: " & CStr2(pDrDevice("ID")) & vbCrLf &
                        "IsActive: " & CStr2(pDrDevice("IsActive")) & vbCrLf &
                        "Current database: " & mProData.GetCurrentDatabase())
                Catch ex As Exception
                    Stop2()
                End Try
                oError = "Device with UUID " & vDeviceUuid & " is not active."
                Return False
            End If
        End If

        If mParent.LibraryDevice.ValidDeviceFowst(vDeviceID, vFowstID, vPosMode, oError) = False Then
            Return False
        End If

        If vDontVerifyOrgEmployee = False Then
            If ValidateOrganizationEmployee(vPeopleID, oError) = False Then
                Return False
            End If
        End If

        If vDontVerifyPeopleFowst = False Then
            pSql = "SELECT ID" & vbCrLf &
                " FROM LibraryOrganizationEmployeeFOWST" &
                " WHERE PeopleID = " & ReadyString(vPeopleID) & vbCrLf &
                " AND FowstID = " & ReadyString(vFowstID)
            pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                oError = "The specified PeopleID and FowstID have no corresponding record in LibraryOrganizationEmployeeFOWST."
                Return False
            End If
        End If

        Return True

    End Function

    ''' <summary>
    ''' requires HandleOrganization
    ''' </summary>
    Public Function ValidateOrganizationEmployee(vPeopleID As String, ByRef oError As String) As Boolean

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT IsBan" & vbCrLf &
            ",Deactivated" & vbCrLf &
            " FROM LibraryOrganizationEmployee" &
            " WHERE PeopleID = " & ReadyString(vPeopleID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            oError = "The specified PeopleID has no corresponding record in LibraryOrganizationEmployee."
            Return False
        End If
        If CDec2(pDR("Deactivated")) <> 0 Then
            oError = "The specified PeopleID was deactivated."
            Return False
        End If
        If CDec2(pDR("IsBan")) <> 0 Then
            oError = "The specified PeopleID was banned."
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' creates #tmpAllowedFowsts.
    ''' requires HandleOrganization.
    ''' requires transactions.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Sub CreateTempTableAllowedViewRequestFowsts( _
        vKeywords As String,
        vAllowOrgsFowsts As Boolean,
        vAllowGroupFowsts As Boolean,
        vAllowBranchFowsts As Boolean,
        Optional vUserID As String = Nothing)

        Dim pJoin As String = ""
        Dim pFinder As Finder
        Dim pSql As String

        If CStr2(vUserID) = "" Then
            vUserID = WebSession.Instance.PeopleUserID
        End If

        pFinder = New Finder()
        With pFinder
            .ProData = mProData
            .SearchString = vKeywords
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier, "MobizLibraryOrganizationFOWST.ID")
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar, "MobizLibraryOrganizationFOWST.Name")
            End With
            If vAllowOrgsFowsts Then
                pJoin &= " SELECT tblAll.ID AS FowstID" & vbCrLf &
                    "   FROM MobizLibraryOrganizationFOWST tblAll" & vbCrLf &
                    "   WHERE tblAll.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
            End If
            If vAllowGroupFowsts Then
                If pJoin <> "" Then
                    pJoin &= vbCrLf & " UNION " & vbCrLf
                End If
                pJoin &= " SELECT LibraryFranchiseGroup.FranchisorFowstID AS FowstID" & vbCrLf &
                    "   FROM LibraryFranchiseGroupHeads" & vbCrLf &
                    "   JOIN LibraryFranchiseGroup ON LibraryFranchiseGroup.ID = LibraryFranchiseGroupHeads.FranchiseGroupID" & vbCrLf &
                    "   WHERE LibraryFranchiseGroupHeads.PeopleID = " & ReadyString(vUserID)
            End If
            If vAllowBranchFowsts Then
                If pJoin <> "" Then
                    pJoin &= vbCrLf & " UNION " & vbCrLf
                End If
                pJoin &= " SELECT LibraryOrganizationEmployeeFOWST.FowstID" & vbCrLf &
                    "   FROM LibraryOrganizationEmployeeFOWST" & vbCrLf &
                    "   WHERE LibraryOrganizationEmployeeFOWST.PeopleID = " & ReadyString(vUserID)
            End If

            .SqlFrom = " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                " JOIN (" & vbCrLf &
                pJoin &
                ") tblFowst ON tblFowst.FowstID = MobizLibraryOrganizationFOWST.ID"

            .SqlOrder = " ORDER BY MobizLibraryOrganizationFOWST.Name"
            .SqlWhere = " WHERE MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
            pSql = .CreateSql()
        End With

        pSql = "SELECT * INTO #tmpAllowedFowsts FROM (" & pSql & ")t"
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function GetByLogin(vUsername As String, vPassword As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT MobizLibraryPeopleUserAccount.PeopleID" & vbCrLf &
            ",MobizLibraryPeopleUserAccount.UserPassword" & vbCrLf &
            " FROM MobizLibraryPeopleUserAccount" &
            " WHERE MobizLibraryPeopleUserAccount.Username = " & ReadyString(vUsername)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            If ProPack.Security.Hash.Verify(vPassword, CStr2(pDR("UserPassword"))) Then
                Return pDR
            End If
        End If

    End Function

    Public Function GetGroupHeadCompanyEmails(vFowstID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT LibraryFranchiseGroupHeads.CompanyEmailAddress" & vbCrLf &
            " FROM MobizLibraryOrganizationFranchisee" & vbCrLf &
            " JOIN LibraryFranchiseGroup ON LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID" & vbCrLf &
            " JOIN LibraryFranchiseGroupHeads ON LibraryFranchiseGroupHeads.FranchiseGroupID = LibraryFranchiseGroup.ID" & vbCrLf &
            " WHERE MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = " & ReadyString(vFowstID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Class DucToMobizExporter
        Dim mBusiness As Business
        Dim mOrgID As String

        Public Class Output
            Public AddedCount As Integer
            Public UpdatedCount As Integer
        End Class

        Public Sub New(vBusiness As Business, vOrgID As String)

            mBusiness = vBusiness
            mOrgID = vOrgID

        End Sub

        ''' <summary>
        ''' this requires a private database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CopyFromHrToMobiz(Optional vEmployeeProfileID As String = Nothing) As Output

            Dim pOut = New Output

            mBusiness.ProData.BeginTransaction()
            ' UpdateDUCNumericIDFromThirdPartySystemID() ' working but commented for now
            Dim pDtEmployeeProfiles = GetFromHrToolkitButNotInMobiz(vEmployeeProfileID)
            Dim pDuplicates =
                From p In pDtEmployeeProfiles
                Group p By ID = p("ID") Into pGroup = Group
                Where pGroup.Count > 1
                Select pGroup
            If pDuplicates.Any() Then
                Dim pError = "vbDUJEEJZP7J! There are " & pDuplicates.Count() & " duplicates"
                Throw New ApplicationException(pError)
            End If

            For Each pDrEmployeeProfile As DataRow In pDtEmployeeProfiles.Rows
                If pDrEmployeeProfile("PeopleID") Is DBNull.Value Then
                    pOut.AddedCount += 1
                Else
                    pOut.UpdatedCount += 1
                End If
                Dim pPeopleID = GetOrCreatePeople(pDrEmployeeProfile)
                GetOrCreatePeopleOrganization(pDrEmployeeProfile, pPeopleID)
                GetOrCreateOrganizationEmployee(pDrEmployeeProfile, pPeopleID)
                GetOrCreatePeopleUserAccount(pDrEmployeeProfile, pPeopleID)
            Next pDrEmployeeProfile
            mBusiness.ProData.CommitTransaction()

            Return pOut

        End Function

        Private Function GetOrCreatePeople(vDR As DataRow) As String

            Dim pID As String

            If vDR("PeopleID") IsNot DBNull.Value Then
                Return CStr2(vDR("PeopleID"))
            End If

            pID = Guid.NewGuid().ToString()
            Dim pIU = New InsertUpdate("MobizLibraryPeople")
            pIU("ID") = ReadyString(pID)
            pIU("DUCNumericID") = vDR("ID")
            pIU("Surname") = ReadyString(vDR("LastName"))
            pIU("FirstName") = ReadyString(vDR("FirstName"))
            pIU("MiddleName") = ReadyString(vDR("MiddleName"))
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            Dim pSql = pIU.GetInsert()
            mBusiness.ProData.ExecuteSQL(pSql)

            Return pID

        End Function

        Private Sub GetOrCreatePeopleOrganization(vDR As DataRow, vPeopleID As String)

            If vDR("PeopleOrganizationID") IsNot DBNull.Value Then
                Return
            End If

            Dim pIU = New InsertUpdate("MobizLibraryPeopleOrganization")
            pIU("ID") = "NEWID()"
            pIU("PeopleID") = ReadyString(vPeopleID)
            pIU("OrganizationID") = ReadyString(mOrgID)
            pIU("IsConfirmed") = 1
            Dim pSql = pIU.GetInsert()
            mBusiness.ProData.ExecuteSQL(pSql)

        End Sub

        Private Sub GetOrCreateOrganizationEmployee(vDR As DataRow, vPeopleID As String)

            If vDR("OrganizationEmployeeID") IsNot DBNull.Value Then
                Return
            End If

            Dim pIU = New InsertUpdate("LibraryOrganizationEmployee")
            pIU("ID") = "NEWID()"
            pIU("PeopleID") = ReadyString(vPeopleID)
            pIU("ThirdPartySystemID") = ReadyString(vDR("EmployeeCompanyID"))
            vDR("ThirdPartySystemID") = vDR("EmployeeCompanyID")
            Dim pSql = pIU.GetInsert()
            mBusiness.ProData.ExecuteSQL(pSql)

        End Sub

        Private Sub GetOrCreatePeopleUserAccount(vDR As DataRow, vPeopleID As String)

            If vDR("PeopleUserAccountID") IsNot DBNull.Value Then
                Return
            End If

            Dim pIU = New InsertUpdate("MobizLibraryPeopleUserAccount")
            pIU("ID") = "NEWID()"
            pIU("PeopleID") = ReadyString(vPeopleID)
            pIU("Username") = ReadyString(vDR("ThirdPartySystemID"))
            pIU("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(vDR("ThirdPartySystemID")))
            pIU("IsActive") = 1
            Dim pSql = pIU.GetInsert()
            mBusiness.ProData.ExecuteSQL(pSql)

        End Sub

        ''' <summary>
        ''' this requires a private database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFromHrToolkitButNotInMobiz(Optional vEmployeeProfileID As String = Nothing,
            Optional vLastName As String = Nothing) As DataTable

            Dim pSql = " -- in EmployeeProfiles but not in LibraryPeople" & vbCrLf &
                " SELECT MobizLibraryPeople.ID AS PeopleID" & vbCrLf &
                " ,LibraryOrganizationEmployee.ID AS OrganizationEmployeeID" & vbCrLf &
                " ,MobizLibraryPeopleOrganization.ID AS PeopleOrganizationID" & vbCrLf &
                " ,MobizLibraryPeopleUserAccount.ID AS PeopleUserAccountID" & vbCrLf &
                " ,LibraryOrganizationEmployee.ThirdPartySystemID" & vbCrLf &
                " ,EmployeeProfiles.*" & vbCrLf &
                " FROM DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles" & vbCrLf &
                " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.DUCNumericID = EmployeeProfiles.ID" & vbCrLf &
                " LEFT JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeopleOrganization ON MobizLibraryPeopleOrganization.PeopleID = MobizLibraryPeople.ID" & vbCrLf &
                " AND MobizLibraryPeopleOrganization.OrganizationID = " & ReadyString(mOrgID) & vbCrLf &
                " LEFT JOIN MobizLibraryPeopleUserAccount ON MobizLibraryPeopleUserAccount.PeopleID = MobizLibraryPeople.ID" & vbCrLf &
                " WHERE EmployeeProfiles.IsActive = 1" & vbCrLf &
                " AND EmployeeProfiles.IsResigned = 0" & vbCrLf &
                " AND (MobizLibraryPeople.ID IS NULL" & vbCrLf &
                "   OR LibraryOrganizationEmployee.ID IS NULL" & vbCrLf &
                "   OR MobizLibraryPeopleOrganization.ID IS NULL" & vbCrLf &
                "   OR MobizLibraryPeopleUserAccount.ID IS NULL" & vbCrLf &
                " )" & vbCrLf
            If String.IsNullOrWhiteSpace(vEmployeeProfileID) = False Then
                pSql &= " AND EmployeeProfiles.ID = " & ReadyString(vEmployeeProfileID) & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(vLastName) = False Then
                pSql &= " AND EmployeeProfiles.LastName LIKE " & ReadyStringWild(vLastName) & vbCrLf
            End If
            pSql &= " ORDER BY EmployeeProfiles.LastName" & vbCrLf &
                " ,EmployeeProfiles.FirstName"
            Dim pDT = mBusiness.ProData.Execute(pSql)

            Return pDT

        End Function

        Private Function UpdateDUCNumericIDFromThirdPartySystemID() As Integer

            Dim pOut As Integer

            Dim pSql = "  SELECT LibraryOrganizationEmployee.PeopleID" & vbCrLf &
                "  ,EmployeeProfiles.ID" & vbCrLf &
                "  ,MobizLibraryPeople.DUCNumericID" & vbCrLf &
                "  FROM DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles" & vbCrLf &
                "  JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.ThirdPartySystemID = EmployeeProfiles.EmployeeCompanyID" & vbCrLf &
                "  LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.DUCNumericID = EmployeeProfiles.ID" & vbCrLf &
                "  WHERE EmployeeProfiles.IsActive = 1" & vbCrLf &
                "  AND EmployeeProfiles.IsResigned = 0" & vbCrLf &
                "  AND MobizLibraryPeople.ID IS NULL" & vbCrLf &
                "  ORDER BY EmployeeProfiles.LastName" & vbCrLf &
                "  ,EmployeeProfiles.FirstName"
            Dim pDT = mBusiness.ProData.Execute(pSql)
            For Each pDR As DataRow In pDT.Rows
                pSql = "UPDATE MobizLibraryPeople" & vbCrLf &
                    " SET DUCNumericID = " & pDR("ID") & vbCrLf &
                    " WHERE ID = " & ReadyString(pDR("PeopleID"))
                pOut += mBusiness.ProData.ExecuteSQL(pSql)
            Next pDR

            Return pOut

        End Function
    End Class ' END DucToMobizExporter

#Region " this should be in LibraryDevice "

    ''' <summary>
    ''' this should be in LibraryDevice
    ''' </summary>
    ''' <param name="vUuid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDeviceByUuid(vUuid As String) As DataRow

        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            ",IsActive" & vbCrLf &
            " FROM LibraryDevice" & vbCrLf &
            " WHERE UUID = " & ReadyString(vUuid)

        Return mProData.ExecuteRow(pSql)

    End Function

    Public Function InsertResetCodes(vEmail As String) As Integer

        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pDT As DataTable

        pSql = " SELECT LibraryPeopleUserAccount.ID" & vbCrLf &
            ",LibraryPeopleUserAccount.Username" & vbCrLf &
            " ,LibraryPeopleContactEmail.EmailAddress" & vbCrLf &
            " FROM LibraryPeopleContactEmail" & vbCrLf &
            " JOIN LibraryPeopleUserAccount ON LibraryPeopleUserAccount.PeopleID = LibraryPeopleContactEmail.PeopleID" & vbCrLf &
            " WHERE LibraryPeopleContactEmail.EmailAddress = " & ReadyString(vEmail) & vbCrLf &
            " AND LibraryPeopleUserAccount.ResetCode IS NULL"
        ' EmailAddress is not really needed as a SELECT field
        pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            pIU = New InsertUpdate("LibraryPeopleUserAccount")
            pIU("ResetCode") = ReadyString(GenerateString(30))
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next pDR

        Return pDT.Rows.Count

    End Function

    Public Function GetResetCodes(vEmail As String) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = " SELECT LibraryPeopleUserAccount.ID" & vbCrLf &
            ",LibraryPeopleUserAccount.Username" & vbCrLf &
            " ,LibraryPeopleContactEmail.EmailAddress" & vbCrLf &
            ",LibraryPeopleUserAccount.ResetCode" & vbCrLf &
            " FROM LibraryPeopleContactEmail" & vbCrLf &
            " JOIN LibraryPeopleUserAccount ON LibraryPeopleUserAccount.PeopleID = LibraryPeopleContactEmail.PeopleID" & vbCrLf &
            " WHERE LibraryPeopleContactEmail.EmailAddress = " & ReadyString(vEmail) & vbCrLf &
            " AND LibraryPeopleUserAccount.ResetCode IS NOT NULL"
        ' EmailAddress is not really needed as a SELECT field
        pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    Public Function GetAccountByResetCode(vResetCode As String) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        pSql = " SELECT LibraryPeopleUserAccount.ID" & vbCrLf &
            ",LibraryPeopleUserAccount.Username" & vbCrLf &
            " ,LibraryPeopleContactEmail.EmailAddress" & vbCrLf &
            " FROM LibraryPeopleContactEmail" & vbCrLf &
            " JOIN LibraryPeopleUserAccount ON LibraryPeopleUserAccount.PeopleID = LibraryPeopleContactEmail.PeopleID" & vbCrLf &
            " WHERE LibraryPeopleUserAccount.ResetCode = " & ReadyString(vResetCode)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function
#End Region

    Public Function GetByID(vID As String) As DataRow

        Dim pSql = "SELECT MobizLibraryPeople.ID" & vbCrLf &
            "," & BrowserController.ComposePeopleName & " AS FullName" & vbCrLf &
            " FROM MobizLibraryPeople" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
            " WHERE MobizLibraryPeople.ID = " & ReadyString(vID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetEcommerceProfile(vID As String) As DataRow

        Dim pSql = "SELECT LibraryEcommerceCustomers.OrganizationID" & vbCrLf &
            ",(SELECT TOP 1 ID" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE OrganizationID = LibraryEcommerceCustomers.OrganizationID" & vbCrLf &
            ") AS FowstID" & vbCrLf &
            " FROM LibraryEcommerceCustomers" & vbCrLf &
            " WHERE LibraryEcommerceCustomers.PersonID = " & ReadyString(vID) & vbCrLf
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Sub CreateEcommerceProfile(vID As String)

        Dim pDrPerson = GetByID(vID)
        If pDrPerson Is Nothing Then
            Throw New ApplicationException("Person with specified ID does not exist. ref: vb3QCFLW62AX!")
        End If
        Dim pOrgID = mParent.LibraryOrganization.GetOrAddByCompanyName(pDrPerson("FullName"), "", True, False)
        Dim pDtFowst = mParent.LibraryOrganizationFOWST.GetByOrgID(pOrgID)
        If pDtFowst.Rows.Count = 0 Then
            mParent.LibraryOrganizationFOWST.AddStreetNumber(pOrgID, "default")
        End If
        Dim pIU As New InsertUpdate("LibraryEcommerceCustomers")
        pIU("ID") = "NEWID()"
        pIU("PersonID") = ReadyString(vID)
        pIU("OrganizationID") = ReadyString(pOrgID)
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function GetOrCreateEcommerceProfile(vID As String) As DataRow

        mProData.BeginTransaction()
        Dim pDR = GetEcommerceProfile(vID)
        If pDR Is Nothing Then
            CreateEcommerceProfile(vID)
        End If
        pDR = GetEcommerceProfile(vID)
        mProData.CommitTransaction()

        Return pDR

    End Function

    Private Function GetByUsername(vUsername As String) As DataRow

        Dim pSql = "SELECT PeopleID" & vbCrLf &
            " FROM MobizLibraryPeopleUserAccount" & vbCrLf &
            " WHERE Username = " & ReadyString(vUsername)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Sub AddUserAccount(vPeopleID As String, vUsername As String, vPassword As String)

        Dim pIU = New InsertUpdate("MobizLibraryPeopleUserAccount")
        pIU("ID") = "NEWID()"
        pIU("PeopleID") = ReadyString(vPeopleID)
        pIU("Username") = ReadyString(vUsername)
        pIU("UserPassword") = ReadyString(ProPack.Security.Hash.SaltedHashedString(vPassword))
        pIU("IsActive") = 1
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Private Function AddPeopleOrganization(vOrgID As String, vPeopleID As String) As DataRow

        Dim pDR = GetPeopleOrganization(vOrgID, vPeopleID)
        If pDR IsNot Nothing Then
            If CBool2(pDR("IsConfirmed")) Then
                Return pDR
            End If
        End If

        Dim pSql As String
        Dim pIU = New InsertUpdate("MobizLibraryPeopleOrganization")
        pIU("IsConfirmed") = 1
        If pDR Is Nothing Then
            pIU("ID") = "NEWID()"
            pIU("PeopleID") = ReadyString(vPeopleID)
            pIU("OrganizationID") = ReadyString(vOrgID)
            pSql = pIU.GetInsert()
        Else
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
        End If

        mProData.ExecuteSQL(pSql)

    End Function

    Private Function GetPeopleOrganization(vOrgID As String, vPeopleID As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            ",IsConfirmed" & vbCrLf &
            " FROM MobizLibraryPeopleOrganization" & vbCrLf &
            " WHERE PeopleID = " & ReadyString(vPeopleID) & vbCrLf &
            " AND OrganizationID = " & ReadyString(vOrgID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Function AddOrganizationEmployee(vPeopleID As String) As DataRow

        Dim pDR = GetOrganizationEmployee(vPeopleID)
        If pDR IsNot Nothing Then
            Return pDR
        End If

        Dim pIU = New InsertUpdate("LibraryOrganizationEmployee")
        pIU("ID") = "NEWID()"
        pIU("PeopleID") = ReadyString(vPeopleID)
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Function

    Private Function GetOrganizationEmployee(vPeopleID As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryOrganizationEmployee" & vbCrLf &
            " WHERE PeopleID = " & ReadyString(vPeopleID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Sub AddPeopleToAdminRole(vPeopleID As String)

        If HasAdminRole(vPeopleID) Then
            Exit Sub
        End If

        Dim pIU = New InsertUpdate("LibrarySecurityRoleUser")
        pIU("ID") = "NEWID()"
        pIU("RoleID") = ReadyString(SecurityAccess.SysAdminRoleID)
        pIU("PeopleID") = ReadyString(vPeopleID)
        pIU("UserID") = ReadyString(vPeopleID)
        pIU("LastUpdated") = "GETDATE()"
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

    End Sub

    Private Function HasAdminRole(vPeopleID As String) As Boolean

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM LibrarySecurityRoleUser" & vbCrLf &
            " WHERE PeopleID = " & ReadyString(vPeopleID) & vbCrLf &
            " AND UserID = " & ReadyString(vPeopleID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR IsNot Nothing

    End Function

    Private Const mAdminUsername As String = "mobizadmin"
    Private Const mAdminPassword As String = "adminmobiz1010!"

    Public Sub AddAdmin(vOrgID As String)

        mProData.BeginTransaction()
        Dim pPeopleID As String
        Dim pDR = GetByUsername(mAdminUsername)
        If pDR Is Nothing Then
            pPeopleID = GetOrCreatePeople(mAdminUsername, mAdminUsername, mAdminUsername)
            AddUserAccount(pPeopleID, mAdminUsername, mAdminPassword)
        Else
            pPeopleID = CStr2(pDR("PeopleID"))
        End If
        AddPeopleOrganization(vOrgID, pPeopleID)
        AddOrganizationEmployee(pPeopleID)
        AddPeopleToAdminRole(pPeopleID)
        mProData.CommitTransaction()

    End Sub
End Class