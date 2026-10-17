Public Class FranchisorCentral
    Public Property ID As String
    Public Property OrgID As String

    Function CheckIfHaveEmailAddress() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With
                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "EmailAddress"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPeopleContactEmail "

            .SqlWhere = "WHERE PeopleID = " & ReadyString(ID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetOtherPriceLevels() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("SID", SqlDbType.UniqueIdentifier)
                    .Expression = "othprclvl.ID"
                End With
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prclvl.ID"
                End With
                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "prclvl.PriceLevel"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "othprclvl.IsActive"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryFranchiseGroupPriceLevels othprclvl " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMPriceLevel prclvl " & vbCrLf &
                        "ON othprclvl.PriceLevelID = prclvl.ID " & vbCrLf

            .SqlWhere = "WHERE othprclvl.GroupID = " & ReadyString(ID) & vbCrLf &
                        " AND othprclvl.OrgID = " & ReadyString(OrgID) & vbCrLf

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function CheckIfOwnedFowst() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.Bit)
                    .Expression = "fowst.ID"
                End With
                With .Add("Name", SqlDbType.Bit)
                    .Expression = "fowst.Name"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWST fowst"
            .SqlWhere = "WHERE OrganizationID = " & ReadyString(OrgID) &
                        " AND ID = " & ReadyString(ID)

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetAllFranchisorGroup(ByVal SearchKeyword As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.Bit)
                    .Expression = "grp.ID"
                End With
                With .Add("GroupName", SqlDbType.Bit)
                    .Expression = "grp.GroupName"
                End With
                With .Add("Description", SqlDbType.Bit)
                    .Expression = "grp.Description"
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "grp.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = "FROM LibraryFranchiseGroup grp LEFT JOIN MobizLibraryPeople ppol ON (ppol.ID = grp.UserID)"
            .SqlWhere = "WHERE GroupName LIKE '%" & SearchKeyword & "%'"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetFranchisorGroupInfo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryFranchiseGroup.ID"
                End With

                .Add("GroupName", SqlDbType.VarChar)

                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "LibraryFranchiseGroup.Description"
                End With

                .Add("FranchisorFowstID", SqlDbType.UniqueIdentifier)

                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizlibraryOrganizationFowst.Name"
                End With

                With .Add("FullName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName"
                End With

                .Add("FranchisorGroupHead", SqlDbType.UniqueIdentifier)
                .Add("RoyaltyFee", SqlDbType.VarChar)
                .Add("MarketingFee", SqlDbType.VarChar)

                With .Add("IsFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(LibraryFranchiseGroup.IsFranchisee, 0)"
                End With

                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "ISNULL(LibraryFranchiseGroup.IsActive, 1)"
                End With
            End With

            .SqlFrom = "from LibraryFranchiseGroup " & vbCrLf &
                        "left join MobizlibraryOrganizationFowst on MobizlibraryOrganizationFowst.ID = LibraryFranchiseGroup.FranchisorFowstID " & vbCrLf &
                        "left join MobizLibraryPeople on MobizLibraryPeople.ID = LibraryFranchiseGroup.franchisorgrouphead "

            .SqlWhere = "Where LibraryFranchiseGroup.ID = " & ReadyString(ID)
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetGroupFranchiseeFOWST() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With

                With .Add("FowstCode", SqlDbType.Int)
                    .Expression = "fowst.Code"
                End With

                With .Add("FowstName", SqlDbType.Int)
                    .Expression = "fowst.Name"
                End With

                With .Add("Address", SqlDbType.Int)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                     "+ ', ' + ISNULL(fowst.Street, '') " &
                                     "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With

                With .Add("MaxInventoryValue", SqlDbType.Float)
                    .Expression = "MaxInventoryValue"
                End With

                With .Add("InventoryValuePriceLevel", SqlDbType.VarChar)
                    .Expression = "InventoryValuePriceLevel"
                End With

                With .Add("DefaultSellingPriceLevelID", SqlDbType.UniqueIdentifier)
                    .Expression = "DefaultSellingPriceLevel"
                End With

                With .Add("DefaultSellingPriceLevel", SqlDbType.UniqueIdentifier)
                    .Expression = "LPPL1.PriceLevel"
                End With

                With .Add("POPriceLevelID", SqlDbType.UniqueIdentifier)
                    .Expression = "POPriceLevel"
                End With

                With .Add("POPriceLevel", SqlDbType.UniqueIdentifier)
                    .Expression = "LPPL2.PriceLevel"
                End With

                With .Add("IsPOExclusiveToPrincipal", SqlDbType.Bit)
                    .Expression = "IsPOExclusiveToPrincipal"
                End With

                With .Add("FowstOrgName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With

                With .Add("FowstOrgOwner", SqlDbType.VarChar)
                    .Expression = " (SELECT STUFF(" & vbCrLf &
                        " 	(SELECT '; ' + " & BrowserController.ComposePeopleName() & vbCrLf &
                        "     FROM MobizLibraryOrganizationOwner" & vbCrLf &
                        " 	JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = MobizLibraryOrganizationOwner.PeopleID" & vbCrLf &
                        " 	LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                        "     WHERE MobizLibraryOrganizationOwner.OrganizationID = fowst.OrganizationID" & vbCrLf &
                        " 	FOR XML PATH('')" & vbCrLf &
                        "     ), 1, 2, '')" & vbCrLf &
                        " )"
                End With

                With .Add("OwnershipType", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryOrganizationFranchisee.OwnershipType,'')"
                End With

            End With
            .SqlFrom = "FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
                        "inner join MobizLibraryOrganizationFOWST fowst on fowst.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                        "LEFT join MobizLibraryPIMPriceLevel LPPL1 on " & vbCrLf &
                        "MobizLibraryOrganizationFranchisee.DefaultSellingPriceLevel = LPPL1.ID " & vbCrLf &
                        "LEFT join MobizLibraryPIMPriceLevel LPPL2 on  " & vbCrLf &
                        "MobizLibraryOrganizationFranchisee.POPriceLevel = LPPL2.ID " & vbCrLf &
                        "LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " & vbCrLf &
                        "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganization org ON org.ID = fowst.OrganizationID"

            .SqlWhere = "Where MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(ID)

            .SqlOrder = " ORDER BY fowst.Name ASC "

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetGroupSchedule() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With

                .Add("CutOffName", SqlDbType.Int)
                .Add("WeekDay", SqlDbType.Int)
                .Add("CutOffTime", SqlDbType.Int)
            End With

            .SqlFrom = "FROM LibraryFranchiseGroupCutOffDetails "

            .SqlWhere = "Where FranchiseGroupID = " & ReadyString(ID) & " AND IsActive=1"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetGroupHeads() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "PeopleID"
                    .Searchable = False
                End With

                With .Add("PeopleName", SqlDbType.UniqueIdentifier)
                    .Expression = "Surname + ', ' + FirstName + ' ' + MiddleName"
                End With

                With .Add("CompanyEmailAddress", SqlDbType.UniqueIdentifier)
                    .Expression = "CompanyEmailAddress"
                End With

            End With

            .SqlFrom = "from LibraryFranchiseGroupHeads inner join MobizLibraryPeople on MobizLibraryPeople.ID = LibraryFranchiseGroupHeads.PeopleID "

            .SqlWhere = "Where FranchiseGroupID = " & ReadyString(ID)

            .SqlGroup = " GROUP BY PeopleID, Surname, FirstName, MiddleName, CompanyEmailAddress "

            .SqlOrder = "Order by Surname ASC "

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveGroup(oError As String) As String
        Dim p As New InsertUpdate("LibraryFranchiseGroup")
        Dim pSql As String
        Dim strIDReturn As String
        Dim ctrOtherPriceLvl As Integer = 0

        oError = Nothing
        p("GroupName") = ReadyString(Common.WebRequest.Data("Name"))
        p("Description") = ReadyString(Common.WebRequest.Data("Description"))
        p("FranchisorFowstID") = IIf(String.IsNullOrWhiteSpace(Common.WebRequest.Data("GroupReportingFOWST")), "NULL", ReadyString(Common.WebRequest.Data("GroupReportingFOWST")))
        p("RoyaltyFee") = ReadyString(Common.WebRequest.Data("RoyaltyFee"))
        p("MarketingFee") = ReadyString(Common.WebRequest.Data("MarketingFee"))
        p("IsFranchisee") = CInt2(Common.WebRequest.Data("IsFranchisee"))
        p("IsActive") = CInt2(Common.WebRequest.Data("IsActive"))
        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
        p("LastUpdated") = "GetDate()"

        Try
            Common.WebRequest.ProData.BeginTransaction()

            If ID.Trim().Length <= 0 Then

                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update MobizLibraryOrganizationFranchisee set FranchiseGroupID = null where FranchiseGroupID = " & ReadyString(strIDReturn) & " AND FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each pObj As Object In Common.WebRequest.AllData("Franchisee")

                If Not pObj("InventoryValuePriceLevel") = "-" Then
                    pSql = "Update MobizLibraryOrganizationFranchisee set " & vbCrLf &
                        "FranchiseGroupID = " & ReadyString(strIDReturn) & vbCrLf &
                        ",MaxInventoryValue = " & CDec2(pObj("MaxInventoryValue")) & vbCrLf &
                        ",InventoryValuePriceLevel = " & ReadyString(pObj("InventoryValuePriceLevel")) & vbCrLf &
                        ",DefaultSellingPriceLevel = " & ReadyString(pObj("DefaultSellingPriceLevelID")) & vbCrLf &
                        ",IsPOExclusiveToPrincipal = " & CInt2(pObj("IsPOExclusiveToPrincipal")) & vbCrLf &
                        ",POPriceLevel = " & ReadyString(pObj("POPriceLevelID")) & vbCrLf &
                        ",OwnershipType = " & ReadyString(pObj("OwnershipType")) &
                        " WHERE FranchiseeOrgFowstID = " & ReadyString(pObj("ID")) & " AND FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
                Else
                    pSql = "Update MobizLibraryOrganizationFranchisee set " & vbCrLf &
                        "FranchiseGroupID = " & ReadyString(strIDReturn) & vbCrLf &
                        ",MaxInventoryValue = " & CDec2(pObj("MaxInventoryValue")) & vbCrLf &
                        ",InventoryValuePriceLevel = " & ReadyString(pObj("InventoryValuePriceLevel")) & vbCrLf &
                        ",DefaultSellingPriceLevel = NULL " & vbCrLf &
                        ",IsPOExclusiveToPrincipal = " & CInt2(pObj("IsPOExclusiveToPrincipal")) & vbCrLf &
                        ",POPriceLevel = NULL" & vbCrLf &
                        ",OwnershipType = " & ReadyString(pObj("OwnershipType")) &
                        " WHERE FranchiseeOrgFowstID = " & ReadyString(pObj("ID")) & " AND FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            'pSql = "Delete from LibraryFranchiseGroupCutOffDetails where FranchiseGroupID = " & ReadyString(strIDReturn)
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            Dim vFranchiseGroupCutOffID As String
            Dim dtTemp As DataTable

            pSql = "Select ID from  LibraryFranchiseGroupCutOff where FranchiseGroupID = " & ReadyString(strIDReturn)
            dtTemp = Common.WebRequest.ProData.Execute(pSql)


            If dtTemp.Rows.Count > 0 Then
                vFranchiseGroupCutOffID = CStr2(dtTemp.Rows(0).Item("ID"))
            Else
                vFranchiseGroupCutOffID = Guid.NewGuid.ToString

                pSql = "Insert into LibraryFranchiseGroupCutOff(ID, FranchiseGroupID, ScheduleType)" &
                    "Values(" &
                    ReadyString(vFranchiseGroupCutOffID) & "," &
                    ReadyString(strIDReturn) & "," &
                    ReadyString("Daily") & ")"

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            For Each pObj As Object In Common.WebRequest.AllData("Schedule")
                Dim mSchedule As New InsertUpdate("LibraryFranchiseGroupCutOffDetails")

                If Not IsGuid(CStr2(pObj("ID"))) Then
                    mSchedule("FranchiseGroupCutOffID") = ReadyString(vFranchiseGroupCutOffID)
                    mSchedule("FranchiseGroupID") = ReadyString(strIDReturn)
                    mSchedule("CutOffName") = ReadyString(pObj("ScheduleName"))
                    mSchedule("WeekDay") = WeekdayValue(pObj("ScheduleDay"))
                    mSchedule("CutOffTime") = ReadyString(pObj("ScheduleTime"))

                    pSql = mSchedule.GetInsert()
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            For Each objOtherPriceLevel As Object In Common.WebRequest.AllData("OtherPriceLevel")
                Dim mOtherPriceLevel As New InsertUpdate("MobizLibraryFranchiseGroupPriceLevels")

                mOtherPriceLevel("OrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
                mOtherPriceLevel("GroupID") = ReadyString(strIDReturn)
                mOtherPriceLevel("PriceLevelID") = ReadyString(objOtherPriceLevel("ID").ToString())
                mOtherPriceLevel("IsActive") = ReadyString(CBool(objOtherPriceLevel("IsActive").ToString()))
                mOtherPriceLevel("LastUpdated") = "getdate()"

                If objOtherPriceLevel("State").ToString() = "add" Then
                    mOtherPriceLevel("ID") = ReadyString(Guid.NewGuid.ToString)
                    pSql = mOtherPriceLevel.GetInsert()

                ElseIf objOtherPriceLevel("State").ToString() = "edit" Then
                    pSql = mOtherPriceLevel.GetUpdate(" ID = " & ReadyString(objOtherPriceLevel("SID").ToString()))

                ElseIf objOtherPriceLevel("State").ToString() = "delete" Then
                    pSql = "DELETE FROM MobizLibraryFranchiseGroupPriceLevels WHERE ID = " & ReadyString(objOtherPriceLevel("SID").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            pSql = "Delete from LibraryFranchiseGroupHeads where FranchiseGroupID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each pObj As Object In Common.WebRequest.AllData("GroupHeads")
                Dim mSchedule As New InsertUpdate("LibraryFranchiseGroupHeads")

                mSchedule("CompanyEmailAddress") = ReadyString(pObj("CompanyEmailAddress"))
                mSchedule("FranchiseGroupID") = ReadyString(strIDReturn)
                mSchedule("PeopleID") = ReadyString(pObj("ID"))

                pSql = mSchedule.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next
            Common.WebRequest.ProData.CommitTransaction()
            UpdateFranchiseeOrgs(strIDReturn)
            WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryFranchiseGroup")
            Return strIDReturn

        Catch ex As Exception
            AppendToDebug("vbGKCUFNL7RJ! " + ex.Message, True, ex, True)
            Common.WebRequest.ProData.RollbackTransaction()
            oError = ex.Message
        End Try

    End Function

    Private Function UpdateFranchiseeOrgs(vFranchiseGroupID As String) As Integer

        Dim pProData As ProData
        Dim pBusiness As Business
        Dim pDT As DataTable

        pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
        pBusiness = New Business(pProData)
        pDT = pBusiness.LibraryOrganization.ByFranchiseGroupID(vFranchiseGroupID)
        For Each pDR As DataRow In pDT.Rows
            If CStr2(pDR("Database")).Trim() <> "" Then
                pProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
                pBusiness.SystemDefaults.UpdateTableLastUpdated("MobizLibraryOrganizationFranchisee")
            End If
        Next pDR

        Return pDT.Rows.Count

    End Function
End Class
