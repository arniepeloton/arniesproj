Public Class BOMProjectExcel
    Public Property OrganizationID As String
    Public Property PeopleID As String
    Public Property FowstID As String
    Public Property ParentID As String
    Public Property ProjectID As String
    Public Property ProductID As String
    Public Property ProjectTypeID As String
    Public Property ProjectName As String
    Public Property ProjectCode As String
    Public Property ProjectDescription As String
    Public Property ProjectBusinessNameID As String
    Public Property ProjectHouseNo As String
    Public Property ProjectStreet As String
    Public Property ProjectCityID As String
    Public Property ProjectCountryID As String
    Public Property JobRolePermission As Object
    Public Property JobRole As Object
    Public Property ProjectMember As Object
    Public Property ProjectOwner As Object
    Public Property ProjectManager As Object
    Public Property ProjectAssistantManager As Object
    Public Property ProjectedStartDate As String
    Public Property ActualStartDate As String
    Public Property ProjectedCompletionDate As String
    Public Property ActualCompletionDate As String
    Public Property ProjectTelephoneNumber As Object
    Public Property ProjectMobileNumber As Object
    Public Property ProjectEmailAddress As Object
    Public Property ProjectBudgetData As Object
    Public Property ProjectBudgetCanvassMaterials As Object
    Public Property ProjectBudgetCanvassLabor As Object
    Public Property ProjectOwnerOrgPersonID As String
    Public Property ClientID As String
    Public Property BranchFowstID As String
    Public Property PreTypeFowstName As String
    Public Property PreTypeFowstNameID As String
    Public Property ProjectBudgetTimeline As Object
    Public Property Projectphotos As HttpFileCollectionBase

    Public Property PageStart As Integer
    Public Property PageNoOfRows As Integer
    Public Property PageSearchText As String
    Public Property IsCountPage As Boolean
    Public Property PageColumnNo As String
    Public Property PageColumnDirection As String
    Public Property ProjectFiles As Object
    Public Property ProjectFilesDetails As Object
    Public Property ClientData As Object

    Function GetTotalMaterialsRequisitionFromServer() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ParentID", SqlDbType.UniqueIdentifier)
                    .Expression = "matreq.ParentID"
                End With

                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "matreq.ProductID"
                End With

                With .Add("TotalNo", SqlDbType.UniqueIdentifier)
                    .Expression = "SUM(matreq.ApprovedRequestedQty)"
                End With

                With .Add("Description", SqlDbType.UniqueIdentifier)
                    .Expression = "projbud.LineDescription"
                End With

            End With

            .SqlFrom = "FROM LibraryProjectMaterialsRequisition as matreq " & vbCrLf &
                        "LEFT JOIN LibraryProjectBudget as projbud ON matreq.ProductID = projbud.ID " & vbCrLf

            .SqlWhere = "WHERE matreq.ProjectID = " & ReadyString(ProjectID) & vbCrLf &
                        " AND matreq.ParentID = " & ReadyString(ParentID) & vbCrLf

            .SqlGroup = "GROUP BY matreq.ParentID, matreq.ProductID, projbud.LineDescription"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetAccessRights() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("PermissionID", SqlDbType.UniqueIdentifier)
                    .Expression = "jobper.PermissionID"
                End With

                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "per.Description"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectJobRolePermission jobper " & vbCrLf &
                        " LEFT JOIN (SELECT * FROM LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                        "               AND FowstID = " & ReadyString(ProjectID) & vbCrLf &
                        "           ) " & vbCrLf &
                        "           AS empfowst ON empfowst.JobRoleID = jobper.JobRoleID " & vbCrLf &
                        " LEFT JOIN LibraryProjectJobRole jobrole ON jobrole.JobRoleID = jobper.JobRoleID " & vbCrLf &
                        " LEFT JOIN LibraryProjectPermission per ON jobper.PermissionID = per.ID " & vbCrLf


            .SqlWhere = " WHERE jobper.ProjectID = " & ReadyString(ProjectID) & vbCrLf &
                        " AND empfowst.JobRoleID = jobper.JobRoleID " & vbCrLf &
                        " AND jobrole.JobRoleID = jobper.JobRoleID " & vbCrLf &
                        " AND jobrole.ProjectID = " & ReadyString(ProjectID) & vbCrLf

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetAllowedPermissionsPerJobRole() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "jobrolepermission.ID"
                End With

                With .Add("RoleID", SqlDbType.VarChar)
                    .Expression = "jobrolepermission.JobRoleID"
                End With

                With .Add("PermissionID", SqlDbType.VarChar)
                    .Expression = "jobrolepermission.PermissionID"
                End With

                With .Add("UserID", SqlDbType.VarChar)
                    .Expression = "jobrolepermission.UserID"
                End With

                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "jobrolepermission.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectJobRolePermission as jobrolepermission "

            .SqlWhere = "WHERE jobrolepermission.ProjectID = " & ReadyString(ProjectID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetJobRole() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "jobrole.ID"
                End With

                With .Add("RoleID", SqlDbType.VarChar)
                    .Expression = "jobrole.JobRoleID"
                End With

                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "jobrole.JobRoleDescription"
                End With

                With .Add("UserID", SqlDbType.VarChar)
                    .Expression = "jobrole.UserID"
                End With

                With .Add("LastUpdated", SqlDbType.VarChar)
                    .Expression = "jobrole.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectJobRole as jobrole "

            .SqlWhere = "WHERE jobrole.ProjectID = " & ReadyString(ProjectID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectBudgetTimeline() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "timeline.ID"
                End With

                With .Add("DescriptionID", SqlDbType.VarChar)
                    .Expression = "timeline.ProductID"
                End With

                With .Add("Importance", SqlDbType.VarChar)
                    .Expression = "timeline.Importance"
                End With

                With .Add("Urgency", SqlDbType.VarChar)
                    .Expression = "timeline.Urgency"
                End With

                With .Add("TimeToComplete", SqlDbType.VarChar)
                    .Expression = "timeline.TimeToComplete"
                End With

                With .Add("AssignedTo", SqlDbType.VarChar)
                    .Expression = "timeline.AssignedTo"
                End With

                With .Add("AssignedToPeopleID", SqlDbType.VarChar)
                    .Expression = "timeline.AssignedToPeopleID"
                End With

                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "timeline.Status"
                End With

                With .Add("Dependency", SqlDbType.VarChar)
                    .Expression = "timeline.Dependency"
                End With

                With .Add("Tags", SqlDbType.VarChar)
                    .Expression = "timeline.Tags"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectBudgetTimeline as timeline "

            .SqlWhere = "WHERE timeline.OrgID = " & ReadyString(OrganizationID) &
                        " AND timeline.FowstID = " & ReadyString(ProjectID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetOrgDetailsToPrint() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(orgcountry.CallingCode)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgareacode.CallingCode)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgtel.ContactNo)) + ' ', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgtel.ExtensionNo)) + ' ', ' '), '') "
                End With

                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(orgcountry.CallingCode)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgtelprefix.Prefix)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgmobile.ContactNo)) + ' ', ' '), '') "
                End With

                With .Add("FaxNo", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(orgcountry.CallingCode)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgareacode.CallingCode)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgfax.ContactNo)) + ' ', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgfax.ExtensionNo)) + ' ', ' '), '') "
                End With

                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "orgemail.EmailAddress"
                End With

                With .Add("BusinessAddress", SqlDbType.VarChar)
                    .Expression = " ISNULL(NULLIF(LTRIM(RTRIM(org.BusinessAddressBuildingNo)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(org.BusinessAddressStreet)) + '.', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(city.Name)) + ' ', ' '), '') + " &
                                  " ISNULL(NULLIF(LTRIM(RTRIM(orgcountry.Name)) + ' ', ' '), '') "
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization as org " &
                        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal " &
                        " ON (org.PersonTitleID = orgprenominal.ID) " &
                        " LEFT JOIN MobizLibraryOrganizationContactEmail as orgemail " &
                        " ON orgemail.OrganizationID = org.ID " &
                        " LEFT JOIN MobizLibraryOrganizationContactFax as orgfax " &
                        " ON orgfax.OrganizationID = org.ID " &
                        " LEFT JOIN MobizLibraryCountries as orgcountry " &
                        " ON orgcountry.ID = orgfax.CountryID AND org.BusinessAddressCountryID = orgcountry.ID " &
                        " LEFT JOIN MobizLibraryAreaCodes as orgareacode " &
                        " ON orgareacode.ID = orgfax.AreaCodeID " &
                        " LEFT JOIN MobizLibraryOrganizationContactMobile as orgmobile " &
                        " ON orgmobile.OrganizationID = org.ID AND orgcountry.ID = orgmobile.CountryID " &
                        " LEFT JOIN MobizLibraryTelcoPrefixes as orgtelprefix " &
                        " ON orgtelprefix.Id = orgmobile.TelCoPrefixID " &
                        " LEFT JOIN MobizLibraryTelcos as orgtelco " &
                        " ON orgtelco.ID = orgtelprefix.TelcoId " &
                        " LEFT JOIN MobizLibraryOrganizationContactTel as orgtel " &
                        " ON orgtel.OrganizationID = org.ID AND orgcountry.ID = orgtel.CountryID AND orgareacode.Id = orgtel.AreaCodeID " &
                        " LEFT JOIN MobizLibraryCities as city " &
                        " ON city.ID = org.BusinessAddressCityID "

            .SqlWhere = " WHERE org.ID = " & ReadyString(OrganizationID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectBudget() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "projbud.ID"
                End With

                With .Add("RowNumber", SqlDbType.VarChar)
                    .Expression = "projbud.RowNumber"
                End With

                With .Add("ItemNumber", SqlDbType.VarChar)
                    .Expression = "projbud.ItemNumber"
                End With

                With .Add("IsParent", SqlDbType.VarChar)
                    .Expression = "projbud.IsParent"
                End With

                With .Add("IsMaterial", SqlDbType.VarChar)
                    .Expression = "projbud.IsMaterial"
                End With

                With .Add("ParentID", SqlDbType.VarChar)
                    .Expression = "projbud.ParentID"
                End With

                With .Add("LineDescription", SqlDbType.VarChar)
                    .Expression = "projbud.LineDescription"
                End With

                With .Add("AssociateSKUID", SqlDbType.VarChar)
                    .Expression = "projbud.AssociateSKUID"
                End With

                With .Add("Qty", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedDMQty"
                End With

                With .Add("UnitOfMeasure", SqlDbType.VarChar)
                    .Expression = "projbud.DMUnitOfMeasure"
                End With

                With .Add("MaterialUnitCost", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedDMUnitCost"
                End With

                With .Add("MaterialAmount", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedDMAmount"
                End With

                With .Add("LaborUnitCost", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedDLUnitCost"
                End With

                With .Add("LaborAmount", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedDLAmount"
                End With

                With .Add("TotalUnitCost", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedTotalUnitCost"
                End With

                With .Add("TotalAmount", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedTotalAmount"
                End With

                With .Add("MaterialVendor", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(convert(varchar(MAX), projbud.DMVendorOrgPeopleID), '')"
                End With

                With .Add("LaborOrgPeople", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(convert(varchar(MAX), projbud.DLVendorOrgPeopleID), '')"
                End With

                With .Add("SKUName", SqlDbType.VarChar)
                    .Expression = "pimprod.ProductName"
                End With

                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "projbud.Status"
                End With

                With .Add("IsMarkAsCompleted", SqlDbType.Bit)
                    .Expression = "projbud.IsMarkAsCompleted"
                End With

                With .Add("PointPersonID", SqlDbType.VarChar)
                    .Expression = "projbud.PointPersonID"
                End With

                With .Add("PointPersonName", SqlDbType.VarChar)
                    .Expression = "(SELECT FirstName + ' ' + Surname FROM MobizLibraryPeople WHERE ID = projbud.PointPersonID)"
                End With

                With .Add("Dependency", SqlDbType.VarChar)
                    .Expression = "projbud.Dependency"
                End With

                With .Add("ActualDuration", SqlDbType.VarChar)
                    .Expression = "projbud.ActualDuration"
                End With

                With .Add("DateStarted", SqlDbType.VarChar)
                    .Expression = "convert(varchar, projbud.DateStarted, 120)"
                End With

                With .Add("DateCompleted", SqlDbType.VarChar)
                    .Expression = "convert(varchar, projbud.DateCompleted, 120)"
                End With

                With .Add("Importance", SqlDbType.VarChar)
                    .Expression = "projbud.Importance"
                End With

                With .Add("Urgency", SqlDbType.VarChar)
                    .Expression = "projbud.Urgency"
                End With

                With .Add("UnitMeasureID", SqlDbType.VarChar)
                    .Expression = "projbud.UnitMeasureID"
                End With

                With .Add("UnitMeasureName", SqlDbType.VarChar)
                    .Expression = "(SELECT ISNULL(lbl.Label, '') FROM MobizLibraryPIMMeasure msr INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) WHERE msr.ID = projbud.UnitMeasureID)"
                End With

                With .Add("IndentNo", SqlDbType.VarChar)
                    .Expression = "projbud.IndentNo"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectBudget as projbud " &
                        " LEFT JOIN MobizLibraryPIMProduct as pimprod " &
                        " ON projbud.AssociateSKUID = pimprod.ID"

            .SqlWhere = "WHERE projbud.OrgID = " & ReadyString(OrganizationID) &
                        " AND projbud.FowstID = " & ReadyString(ProjectID) &
                        " AND projbud.ProjectOwnerOrgPersonID IS NULL " &
                        " AND projbud.ProjectOwnerFowstID IS NULL " &
                        " ORDER BY projbud.RowNumber ASC"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectBudgetCanvassMaterials() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pimsup.ID"
                End With

                With .Add("RowNumber", SqlDbType.VarChar)
                    .Expression = "projbud.RowNumber"
                End With

                With .Add("ItemNumber", SqlDbType.VarChar)
                    .Expression = "projbud.ItemNumber"
                End With

                With .Add("ProjectBudgetID", SqlDbType.VarChar)
                    .Expression = "projbud.ID"
                End With

                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ISNULL(liborg.CompanyName, '')"
                End With

                With .Add("CompanyID", SqlDbType.UniqueIdentifier)
                    .Expression = "liborg.ID"
                End With

                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With

                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With

                With .Add("Cost", SqlDbType.VarChar)
                    .Expression = "pimsup.Cost"
                End With

                With .Add("IsPreferredCanvass", SqlDbType.VarChar)
                    .Expression = "pimsup.IsPreferredVendor"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectBudget as projbud " &
                        " LEFT JOIN MobizLibraryPIMProduct as pimprod " &
                        " ON projbud.AssociateSKUID = pimprod.ID" &
                        " LEFT JOIN MobizLibraryPimSupplier as pimsup" &
                        " ON projbud.ID = pimsup.ProductID" &
                        " LEFT JOIN MobizLibraryOrganization as liborg" &
                        " ON pimsup.SupplierID = liborg.ID" &
                        " LEFT JOIN MobizLibraryPeople as ppol" &
                        " ON pimsup.SupplierID = ppol.ID" &
                        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal" &
                        " ON ppol.TitleID = ppolprenominal.ID"

            .SqlWhere = "WHERE projbud.OrgID = " & ReadyString(OrganizationID) &
                        " AND projbud.FowstID = " & ReadyString(ProjectID) &
                        " ORDER BY projbud.RowNumber ASC"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectBudgetCanvassMaterialsForm() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("CompanyID", SqlDbType.UniqueIdentifier)
                    .Expression = "liborg.ID"
                End With

                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ISNULL(liborg.CompanyName, '')"
                End With

                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With

                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectBudget as projbud " &
                        " LEFT JOIN MobizLibraryPimSupplier as pimsup" &
                        " ON projbud.ID = pimsup.ProductID" &
                        " LEFT JOIN MobizLibraryOrganization as liborg" &
                        " ON pimsup.SupplierID = liborg.ID" &
                        " LEFT JOIN MobizLibraryPeople as ppol" &
                        " ON pimsup.SupplierID = ppol.ID" &
                        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal" &
                        " ON ppol.TitleID = ppolprenominal.ID"

            .SqlWhere = "WHERE projbud.OrgID = " & ReadyString(OrganizationID) &
                        " AND projbud.FowstID = " & ReadyString(ProjectID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectBudgetCanvassLabor() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "labcost.ID"
                End With

                With .Add("RowNumber", SqlDbType.VarChar)
                    .Expression = "projbud.RowNumber"
                End With

                With .Add("ItemNumber", SqlDbType.VarChar)
                    .Expression = "projbud.ItemNumber"
                End With

                With .Add("ProjectBudgetID", SqlDbType.UniqueIdentifier)
                    .Expression = "projbud.ID"
                End With

                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ISNULL(liborg.CompanyName, '')"
                End With

                With .Add("CompanyID", SqlDbType.UniqueIdentifier)
                    .Expression = "liborg.ID"
                End With

                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With

                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With

                With .Add("Cost", SqlDbType.VarChar)
                    .Expression = "labcost.Cost"
                End With

                With .Add("IsPreferredCanvass", SqlDbType.VarChar)
                    .Expression = "labcost.IsPreferredLaborer"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectBudget as projbud " &
                        " LEFT JOIN MobizLibraryPIMProduct as pimprod " &
                        " ON projbud.AssociateSKUID = pimprod.ID" &
                        " LEFT JOIN LibraryProjectBudgetLaborCost as labcost" &
                        " ON projbud.ID = labcost.ProductID" &
                        " LEFT JOIN MobizLibraryOrganization as liborg" &
                        " ON labcost.LaborID = liborg.ID" &
                        " LEFT JOIN MobizLibraryPeople as ppol" &
                        " ON labcost.LaborID = ppol.ID" &
                        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal" &
                        " ON ppol.TitleID = ppolprenominal.ID"

            .SqlWhere = "WHERE projbud.OrgID = " & ReadyString(OrganizationID) &
                        " AND projbud.FowstID = " & ReadyString(ProjectID) &
                        " ORDER BY projbud.RowNumber ASC"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectBudgetCanvassLaborForm() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("CompanyID", SqlDbType.UniqueIdentifier)
                    .Expression = "liborg.ID"
                End With

                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ISNULL(liborg.CompanyName, '')"
                End With

                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With

                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectBudget as projbud " &
                        " LEFT JOIN LibraryProjectBudgetLaborCost as labcost" &
                        " ON projbud.ID = labcost.ProductID" &
                        " LEFT JOIN MobizLibraryOrganization as liborg" &
                        " ON labcost.LaborID = liborg.ID" &
                        " LEFT JOIN MobizLibraryPeople as ppol" &
                        " ON labcost.LaborID = ppol.ID" &
                        " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal" &
                        " ON ppol.TitleID = ppolprenominal.ID"

            .SqlWhere = "WHERE projbud.OrgID = " & ReadyString(OrganizationID) &
                        " AND projbud.FowstID = " & ReadyString(ProjectID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectHeader() As DataTable
        Dim pSql As String
        Dim pDT As DataTable
        Dim whereSearch As String = "WHERE orgfowst.OrganizationID = " & ReadyString(OrganizationID) & "AND orgfowst.FOWSTTypeID = 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'"
        Dim whereOrder As String = ""
        Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

        If CStr2(FowstID) <> "" Then
            whereSearch += " AND orgfowst.ID = " & ReadyString(FowstID) & vbCrLf
        End If

        If PageSearchText <> "" Then
            Dim whereSearchContent As String = ""
            Dim operators As String = ""

            Dim arr As Object = Split(PageSearchText, " ")

            For i As Integer = 0 To arr.length - 1
                If whereSearchContent <> "" Then
                    operators = " OR "
                End If

                whereSearchContent = whereSearchContent & operators & " orgfowst.Name " & vbCrLf &
                                " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
            Next
            whereSearch += " AND (" & whereSearchContent & ")"
        End If

        If IsCountPage = True Then
            whereLimit = ""
        End If

        If PageColumnNo = 0 Or PageColumnNo = 1 Then
            whereOrder = " ORDER BY mainsql.ProjectName " & PageColumnDirection
        ElseIf PageColumnNo = 2 Then
            whereOrder = " ORDER BY mainsql.ProjectCode " & PageColumnDirection
        ElseIf PageColumnNo = 3 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.ProjectedStartDate) " & PageColumnDirection
        ElseIf PageColumnNo = 4 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.ActualStartDate) " & PageColumnDirection
        ElseIf PageColumnNo = 5 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.ProjectedCompletionDate) " & PageColumnDirection
        ElseIf PageColumnNo = 6 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.ActualCompletionDate) " & PageColumnDirection
        ElseIf PageColumnNo = 7 Then
            whereOrder = " ORDER BY CONVERT(datetime, mainsql.LastUpdated) " & PageColumnDirection
        End If

        pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                " SELECT orgfowst.ID AS [ID] " & vbCrLf &
                ", city.ID AS [CityID] " & vbCrLf &
                ", country.ID AS [CountryID] " & vbCrLf &
                ", orgfowst.Name AS [ProjectName] " & vbCrLf &
                ", orgfowst.Code AS [ProjectCode] " & vbCrLf &
                ", orgfowst.Description AS [ProjectDescription] " & vbCrLf &
                ", convert(varchar, orgfowst.ProjectedStartDate, 120) AS [ProjectedStartDate] " & vbCrLf &
                ", convert(varchar, orgfowst.ActualCompletionDate, 120) AS [ActualStartDate] " & vbCrLf &
                ", convert(varchar, orgfowst.ProjectedCompletionDate, 120) AS [ProjectedCompletionDate] " & vbCrLf &
                ", convert(varchar, orgfowst.ActualCompletionDate, 120) AS [ActualCompletionDate] " & vbCrLf &
                ", orgfowst.StreetNumber AS [StreetNumber] " & vbCrLf &
                ", orgfowst.Street AS [Street] " & vbCrLf &
                ", city.Name AS [City] " & vbCrLf &
                ", country.Name AS [Country] " & vbCrLf &
                ", convert(varchar, orgfowst.LastUpdated, 120) AS [LastUpdated] " & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST as orgfowst " & vbCrLf &
                " INNER JOIN MobizLibraryCities as city ON city.ID = orgfowst.FowstCityID " & vbCrLf &
                " INNER JOIN MobizLibraryCountries as country ON country.ID = orgfowst.FowstCountryID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                ") as mainsql ) as v2 " & whereLimit

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetProjectInfo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.ID"
                End With

                With .Add("CityID", SqlDbType.UniqueIdentifier)
                    .Expression = "city.ID"
                End With

                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "country.ID"
                End With

                With .Add("ProjectName", SqlDbType.VarChar)
                    .Expression = "orgfowst.Name"
                End With

                With .Add("ProjectCode", SqlDbType.VarChar)
                    .Expression = "orgfowst.Code"
                End With

                With .Add("ProjectDescription", SqlDbType.VarChar)
                    .Expression = "orgfowst.Description"
                End With

                With .Add("ProjectedStartDate", SqlDbType.Date)
                    .Expression = "orgfowst.ProjectedStartDate"
                End With

                With .Add("ActualStartDate", SqlDbType.Date)
                    .Expression = "orgfowst.ActualCompletionDate"
                End With

                With .Add("ProjectedCompletionDate", SqlDbType.Date)
                    .Expression = "orgfowst.ProjectedCompletionDate"
                End With

                With .Add("ActualCompletionDate", SqlDbType.Date)
                    .Expression = "orgfowst.ActualCompletionDate"
                End With

                With .Add("StreetNumber", SqlDbType.Date)
                    .Expression = "orgfowst.StreetNumber"
                End With

                With .Add("Street", SqlDbType.Date)
                    .Expression = "orgfowst.Street"
                End With

                With .Add("City", SqlDbType.Date)
                    .Expression = "city.Name"
                End With

                With .Add("Country", SqlDbType.Date)
                    .Expression = "country.Name"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "orgfowst.LastUpdated"
                End With

                With .Add("RowNumber", SqlDbType.Int)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY orgfowst.LastUpdated DESC)"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWST as orgfowst " &
                        "INNER JOIN MobizLibraryCities as city ON city.ID = orgfowst.FowstCityID " &
                        "INNER JOIN MobizLibraryCountries as country ON country.ID = orgfowst.FowstCountryID"

            If CStr2(FowstID) = "" Then
                .SqlWhere = "WHERE orgfowst.OrganizationID = " & ReadyString(OrganizationID) &
                        " AND orgfowst.FOWSTTypeID = 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'" 'Default "PROJECT"
            Else
                .SqlWhere = "WHERE orgfowst.OrganizationID = " & ReadyString(OrganizationID) &
                        " AND orgfowst.ID = " & ReadyString(FowstID) &
                        " AND orgfowst.FOWSTTypeID = 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'"
            End If

            If CStr2(Common.WebRequest.Data("keywords")) <> "" Then
                .SqlWhere += " AND orgfowst.Name LIKE " & ReadyStringWild(Common.WebRequest.Data("keywords"))
            End If

            .SqlOrder = "ORDER BY orgfowst.LastUpdated DESC"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectOwnerOrgPerson() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProjectBudgetID", SqlDbType.UniqueIdentifier)
                    .Expression = "projbud.ID"
                End With

                With .Add("ProjectOwnerOrgPersonID", SqlDbType.UniqueIdentifier)
                    .Expression = "projbud.ProjectOwnerOrgPersonID"
                End With

                With .Add("ProjectOwnerFowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "projbud.ProjectOwnerFowstID"
                End With

                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "ISNULL(liborg.CompanyName, '')"
                End With

                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(liborg.PersonSurname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(liborg.PersonFirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(liborg.PersonMiddleName)) + ' ',' '), '')"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFOWST as orgfowst" &
                        " LEFT JOIN LibraryProjectBudget as projbud ON projbud.FOWSTID = orgfowst.ID" &
                        " LEFT JOIN MobizLibraryOrganization as liborg ON liborg.ID = projbud.ProjectOwnerOrgPersonID"

            .SqlWhere = "WHERE orgfowst.OrganizationID = " & ReadyString(OrganizationID) &
                    " AND orgfowst.ID = " & ReadyString(FowstID) &
                    " AND orgfowst.FOWSTTypeID = 'dff0f087-4c2b-4b67-aa1d-502f02773e2b'" &
                    " AND (projbud.ProjectOwnerOrgPersonID IS NOT NULL OR projbud.ProjectOwnerFowstID IS NOT NULL)"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetProjectOwnerManagerAssistant() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgempfowst.ID"
                End With

                With .Add("PeopleID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                End With

                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With

                With .Add("IsProjectManager", SqlDbType.Bit)
                    .Expression = "orgempfowst.IsProjectManager"
                End With

                With .Add("IsProjectOwner", SqlDbType.Bit)
                    .Expression = "orgempfowst.IsProjectOwner"
                End With

                With .Add("MemberRoleID", SqlDbType.Bit)
                    .Expression = "orgempfowst.JobRoleID"
                End With

                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "orgempfowst.IsActive"
                End With

            End With

            .SqlFrom = "FROM LibraryOrganizationEmployeeFOWST as orgempfowst " &
                        "INNER JOIN MobizLibraryPeople as ppol " &
                        "ON orgempfowst.PeopleID = ppol.ID"

            .SqlWhere = "WHERE orgempfowst.FowstID = " & ReadyString(FowstID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectTelephoneInfo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowsttel.ID"
                End With

                With .Add("AreaCodeID", SqlDbType.UniqueIdentifier)
                    .Expression = "area.ID"
                End With

                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "country.ID"
                End With

                With .Add("Area", SqlDbType.Date)
                    .Expression = "area.Area"
                End With

                With .Add("AreaCallingCode", SqlDbType.Date)
                    .Expression = "area.CallingCode"
                End With

                With .Add("Country", SqlDbType.Date)
                    .Expression = "country.Name"
                End With

                With .Add("CountryCode", SqlDbType.Date)
                    .Expression = "country.Code"
                End With

                With .Add("CountryCallingCode", SqlDbType.Date)
                    .Expression = "country.CallingCode"
                End With

                With .Add("ContactNo", SqlDbType.Date)
                    .Expression = "orgfowsttel.ContactNo"
                End With

                With .Add("ExtensionNo", SqlDbType.Date)
                    .Expression = "orgfowsttel.ExtensionNo"
                End With

                With .Add("ContactType", SqlDbType.Date)
                    .Expression = "orgfowsttel.ContactType"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "orgfowsttel.LastUpdated"
                End With

            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWSTTel as orgfowsttel " &
                        "INNER JOIN MobizLibraryOrganizationFOWST as orgfowst " &
                        "ON orgfowsttel.FowstID = orgfowst.ID " &
                        "INNER JOIN MobizLibraryAreaCodes as area ON area.ID = orgfowsttel.AreaCodeID " &
                        "INNER JOIN MobizLibraryCountries as country ON country.ID = orgfowsttel.CountryID "

            .SqlWhere = "WHERE orgfowsttel.FowstID = " & ReadyString(FowstID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectMobileInfo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstmobile.ID"
                End With

                With .Add("TelCoPrefixID", SqlDbType.UniqueIdentifier)
                    .Expression = "telcopfx.ID"
                End With

                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "country.ID"
                End With

                With .Add("TelCoPrefix", SqlDbType.Date)
                    .Expression = "telcopfx.Prefix"
                End With

                With .Add("TelCoName", SqlDbType.Date)
                    .Expression = "telco.Name"
                End With

                With .Add("Country", SqlDbType.Date)
                    .Expression = "country.Name"
                End With

                With .Add("ContactNo", SqlDbType.Date)
                    .Expression = "orgfowstmobile.ContactNo"
                End With

                With .Add("CountryCode", SqlDbType.Date)
                    .Expression = "country.Code"
                End With

                With .Add("CountryCallingCode", SqlDbType.Date)
                    .Expression = "country.CallingCode"
                End With

                With .Add("ContactType", SqlDbType.Date)
                    .Expression = "orgfowstmobile.ContactType"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "orgfowstmobile.LastUpdated"
                End With

            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWSTMobile as orgfowstmobile " &
                        "INNER JOIN MobizLibraryOrganizationFOWST as orgfowst " &
                        "ON orgfowstmobile.FowstID = orgfowst.ID " &
                        "INNER JOIN MobizLibraryTelcoPrefixes as telcopfx ON telcopfx.ID = orgfowstmobile.TelCoPrefixID " &
                        "INNER JOIN MobizLibraryCountries as country ON country.ID = orgfowstmobile.CountryID " &
                        "INNER JOIN MobizLibraryTelcos as telco ON telco.ID = telcopfx.TelcoId "

            .SqlWhere = "WHERE orgfowstmobile.FowstID = " & ReadyString(FowstID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectEmailInfo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstemail.ID"
                End With

                With .Add("EmailAddress", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstemail.EmailAddress"
                End With

                With .Add("EmailType", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowstemail.EmailType"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "orgfowstemail.LastUpdated"
                End With

            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWSTEmail as orgfowstemail " &
                        "INNER JOIN MobizLibraryOrganizationFOWST as orgfowst " &
                        "ON orgfowstemail.FowstID = orgfowst.ID "

            .SqlWhere = "WHERE orgfowstemail.FowstID = " & ReadyString(FowstID)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function CheckProjectName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.ID"
                End With

                With .Add("ProjectName", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.Name"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWST as orgfowst "

            .SqlWhere = "WHERE orgfowst.Name = " & ReadyString(ProjectName)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function CheckProjectCode() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.ID"
                End With

                With .Add("ProjectCode", SqlDbType.UniqueIdentifier)
                    .Expression = "orgfowst.Name"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWST as orgfowst "

            .SqlWhere = "WHERE orgfowst.Code = " & ReadyString(ProjectCode)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SaveProjectHeader() As String
        Dim p As New InsertUpdate("MobizLibraryOrganizationFOWST")
        Dim pSql As String
        Dim strFowstID As String

        Try

            pSql = "SELECT ISNULL(MAX(SysGenProjectCode), 0) as Code FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                    " WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                    " AND FOWSTTypeID = 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B' " 'PROJECT

            Dim sysgenprojectcode As Integer = CDec2(Common.WebRequest.ProData.ExecuteRow(pSql)(0)) + 1

            p("OrganizationID") = ReadyString(OrganizationID)
            p("FOWSTTypeID") = ReadyString(ProjectTypeID)
            p("Code") = ReadyString(ProjectCode)
            p("Name") = ReadyString(ProjectName)
            p("Description") = ReadyString(ProjectDescription)
            'p("BusinessTradeNameID") = ReadyString(ProjectBusinessNameID)
            p("StreetNumber") = ReadyString(ProjectHouseNo)
            p("Street") = ReadyString(ProjectStreet)
            p("FowstCityID") = ReadyString(ProjectCityID)
            p("FowstCountryID") = ReadyString(ProjectCountryID)
            p("ProjectedStartDate") = ReadyString(CDate(ProjectedStartDate))
            p("ActualStartDate") = If(ActualStartDate = "Set Date", "NULL", ReadyString(CDate(ActualStartDate)))
            p("ProjectedCompletionDate") = ReadyString(CDate(ProjectedCompletionDate))
            p("ActualCompletionDate") = If(ActualCompletionDate = "Set Date", "NULL", ReadyString(CDate(ActualCompletionDate)))
            p("LastUpdated") = "getdate()"

            If FowstID = Nothing Then
                strFowstID = Guid.NewGuid.ToString()
                p("SysGenProjectCode") = ReadyString(sysgenprojectcode)
                p("ID") = ReadyString(strFowstID)
                pSql = p.GetInsert()
            Else
                strFowstID = FowstID

                pSql = "SELECT SysGenProjectCode FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(strFowstID)
                Dim ProjectCode As String = CStr2(Common.WebRequest.ProData.ExecuteRow(pSql)(0))

                If ProjectCode = "" Then
                    p("SysGenProjectCode") = ReadyString(sysgenprojectcode)
                End If

                pSql = p.GetUpdate(" ID = " & ReadyString(strFowstID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'commented by arnie 12-12-2018, nakalimot para sa asa ni
            'p = New InsertUpdate("MobizLibraryOrganizationFOWST")

            'p("OrganizationID") = If(ClientID = "", "NULL", ReadyString(ClientID))
            'p("FOWSTTypeID") = ReadyString("6af9bc83-426c-4774-a051-044fc09c552b") 'default STORE
            'p("Name") = ReadyString(PreTypeFowstName)
            'p("StreetNumber") = ReadyString(".")
            'p("Street") = ReadyString(".")
            'p("LastUpdated") = "getdate()"
            'p("IsIncludeSpecialReport") = ReadyString("0")
            'p("IsIncludeToShopperTrakReport") = ReadyString("0")
            'p("IsFromDevice") = ReadyString("0")
            'p("IsForMobizConstruct") = ReadyString("1") 'for mobiz construct only

            'If ClientID <> "" Then
            '    If PreTypeFowstNameID = "" Then
            '        p("ID") = ReadyString(Guid.NewGuid.ToString())
            '        pSql = p.GetInsert()
            '    Else
            '        pSql = p.GetUpdate(" ID = " & ReadyString(PreTypeFowstNameID))
            '    End If
            '    Common.WebRequest.ProData.ExecuteSQL(pSql)
            'End If

            p = New InsertUpdate("LibraryProjectBudget")

            p("OrgID") = ReadyString(OrganizationID)
            p("FOWSTID") = ReadyString(strFowstID)
            p("ProjectOwnerOrgPersonID") = If(ClientID = "", "NULL", ReadyString(ClientID))
            p("ProjectOwnerFowstID") = If(BranchFowstID = "", "NULL", ReadyString(BranchFowstID))

            If ClientID <> "" Or BranchFowstID <> "" Then
                If ProjectOwnerOrgPersonID = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(ProjectOwnerOrgPersonID))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            '''Job Role Permission
            For Each objJobRolePermission As Object In JobRolePermission
                p = New InsertUpdate("LibraryProjectJobRolePermission")

                p("JobRoleID") = ReadyString(objJobRolePermission("RoleID").ToString())
                p("PermissionID") = ReadyString(objJobRolePermission("PermissionID").ToString())
                p("ProjectID") = ReadyString(strFowstID)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If objJobRolePermission("DbID").ToString() = "" And CBool(objJobRolePermission("IsChecked").ToString()) = True Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf objJobRolePermission("DbID").ToString() <> "" And CBool(objJobRolePermission("IsChecked").ToString()) = False Then
                    pSql = "DELETE FROM LibraryProjectJobRolePermission WHERE ID = " & ReadyString(objJobRolePermission("DbID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

            Next objJobRolePermission

            '''Job Role
            For Each objJobRole As Object In JobRole
                p = New InsertUpdate("LibraryProjectJobRole")

                p("JobRoleID") = ReadyString(objJobRole("RoleID").ToString())
                p("JobRoleDescription") = ReadyString(objJobRole("Label").ToString())
                p("ProjectID") = ReadyString(strFowstID)
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If objJobRole("ID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf objJobRole("ID").ToString() <> "" And objJobRole("State").ToString() = "delete" Then
                    pSql = "DELETE FROM LibraryProjectJobRole WHERE ID = " & ReadyString(objJobRole("ID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

            Next objJobRole

            '''Project Member
            For Each objProjectMember As Object In ProjectMember
                p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                p("PeopleID") = ReadyString(objProjectMember("MemberID").ToString())
                p("JobRoleID") = ReadyString(objProjectMember("MemberRoleID").ToString())
                p("FowstID") = ReadyString(strFowstID)
                p("IsActive") = objProjectMember("IsActive").ToString()
                p("LastUpdated") = "getdate()"

                If objProjectMember("ID").ToString() = "" And (objProjectMember("State") = "" Or objProjectMember("State") = "add") Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf objProjectMember("ID").ToString() <> "" And objProjectMember("State") = "edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(objProjectMember("ID").ToString()))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    pSql = "DELETE FROM LibraryOrganizationEmployeeFOWST WHERE ID = " & ReadyString(objProjectMember("ID").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

            Next objProjectMember

            '''Project Owner
            For Each objProjectOwner As Object In ProjectOwner
                p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                p("PeopleID") = ReadyString(objProjectOwner("PeopleID").ToString())
                p("FowstID") = ReadyString(strFowstID)
                p("IsProjectOwner") = "1"
                p("LastUpdated") = "getdate()"

                If objProjectOwner("ProjectOwnerState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                ElseIf objProjectOwner("ProjectOwnerState").ToString() = "Remove" Then
                    pSql = "DELETE FROM LibraryOrganizationEmployeeFOWST WHERE ID = " & ReadyString(objProjectOwner("id").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

            Next objProjectOwner

            '''Project Manager
            For Each objProjectManager As Object In ProjectManager
                p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                p("PeopleID") = ReadyString(objProjectManager("PeopleID").ToString())
                p("FowstID") = ReadyString(strFowstID)
                p("IsProjectManager") = ReadyString(objProjectManager("IsProjectManager").ToString())
                p("LastUpdated") = "getdate()"

                If objProjectManager("ProjectManagerState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                ElseIf objProjectManager("ProjectManagerState").ToString() = "Remove" Then
                    pSql = "DELETE FROM LibraryOrganizationEmployeeFOWST WHERE ID = " & ReadyString(objProjectManager("id").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

            Next objProjectManager

            '''Project Assistant Manager
            For Each objProjectAssistantManager As Object In ProjectAssistantManager
                p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")

                p("PeopleID") = ReadyString(objProjectAssistantManager("PeopleID").ToString())
                p("FowstID") = ReadyString(strFowstID)
                p("IsProjectManager") = ReadyString(objProjectAssistantManager("IsProjectManager").ToString())
                p("LastUpdated") = "getdate()"

                If objProjectAssistantManager("ProjectAsstManagerState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                ElseIf objProjectAssistantManager("ProjectAsstManagerState").ToString() = "Remove" Then
                    pSql = "DELETE FROM LibraryOrganizationEmployeeFOWST WHERE ID = " & ReadyString(objProjectAssistantManager("id").ToString())
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

            Next objProjectAssistantManager

            '''Telephone Info
            For Each objTelephoneNumber As Object In ProjectTelephoneNumber
                p = New InsertUpdate("MobizLibraryOrganizationFOWSTTel")

                p("FOWSTID") = ReadyString(strFowstID)
                p("CountryID") = ReadyString(objTelephoneNumber("CountryCodeID").ToString())
                p("AreaCodeID") = ReadyString(objTelephoneNumber("AreaCodeID").ToString())
                p("ContactNo") = ReadyString(objTelephoneNumber("TelephoneNo").ToString())
                p("ExtensionNo") = ReadyString(objTelephoneNumber("ExtensionNo").ToString())
                p("ContactType") = ReadyString(objTelephoneNumber("ContactType").ToString())
                p("LastUpdated") = "getdate()"

                If objTelephoneNumber("TelState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf objTelephoneNumber("TelState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(objTelephoneNumber("id").ToString()))

                ElseIf objTelephoneNumber("TelState").ToString() = "Remove" Then
                    pSql = "DELETE FROM MobizLibraryOrganizationFOWSTTel WHERE ID = " & ReadyString(objTelephoneNumber("id").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next objTelephoneNumber

            '''Mobile Info
            For Each objMobileNumber As Object In ProjectMobileNumber
                p = New InsertUpdate("MobizLibraryOrganizationFOWSTMobile")

                p("FOWSTID") = ReadyString(strFowstID)
                p("CountryID") = ReadyString(objMobileNumber("CountryCodeID").ToString())
                p("TelCoPrefixID") = ReadyString(objMobileNumber("TelCodeID").ToString())
                p("ContactNo") = ReadyString(objMobileNumber("MobileNo").ToString())
                p("ContactType") = ReadyString(objMobileNumber("ContactType").ToString())
                p("LastUpdated") = "getdate()"

                If objMobileNumber("MobileState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf objMobileNumber("MobileState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(objMobileNumber("id").ToString()))

                ElseIf objMobileNumber("MobileState").ToString() = "Remove" Then
                    pSql = "DELETE FROM MobizLibraryOrganizationFOWSTMobile WHERE ID = " & ReadyString(objMobileNumber("id").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next objMobileNumber

            'Email Info
            For Each objEmailAddress As Object In ProjectEmailAddress
                p = New InsertUpdate("MobizLibraryOrganizationFOWSTEmail")

                p("FOWSTID") = ReadyString(strFowstID)
                p("EmailAddress") = ReadyString(objEmailAddress("EmailAdd").ToString())
                p("EmailType") = ReadyString(objEmailAddress("EmailType").ToString())
                p("LastUpdated") = "getdate()"

                If objEmailAddress("EmailState").ToString() = "Add" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()

                ElseIf objEmailAddress("EmailState").ToString() = "Edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(objEmailAddress("id").ToString()))

                ElseIf objEmailAddress("EmailState").ToString() = "Remove" Then
                    pSql = "DELETE FROM MobizLibraryOrganizationFOWSTEmail WHERE ID = " & ReadyString(objEmailAddress("id").ToString())
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next objEmailAddress

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryProjectBudget'"
            Dim row As Integer = Common.WebRequest.ProData.ExecuteSQL(pSql)

            If row = 0 Then
                pSql = "INSERT INTO TablesLastUpdated(ID, TableName, FileName, LastUpdated) " & vbCrLf &
                        "VALUES(" & ReadyString(Guid.NewGuid.ToString()) & ", 'LibraryProjectBudget', NULL, GetDate())"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Dim pOut As New JsonDictionary

            pOut.Add("ID", strFowstID)
            pOut.Add("ProjectName", ProjectName)
            Return pOut.Serialize()
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Function SaveProjectBudget() As String

        Dim pSql As String
        Dim strProjectBudgetID As String
        Dim ctrCanvassMaterialsA As Integer = 0
        Dim ctrCanvassLaborA As Integer = 0
        Dim ctrTimeline As Integer = 0
        Dim errmsg As String = ""
        Dim returnID As String
        Dim arrNewReqMaterial As New List(Of Object)
        Dim arrExistingReqMaterial As New List(Of Object)
        Dim p As New InsertUpdate("LibraryProjectBudget")

        Try
            Common.WebRequest.ProData.BeginTransaction()

            For Each objProjectBudget As Object In ProjectBudgetData
                p = New InsertUpdate("LibraryProjectBudget")

                FowstID = objProjectBudget("FOWSTID").ToString()

                p("OrgID") = ReadyString(objProjectBudget("OrgID").ToString())
                p("FOWSTID") = ReadyString(objProjectBudget("FOWSTID").ToString())
                p("ItemNumber") = ReadyString(objProjectBudget("ItemNumber").ToString())
                p("RowNumber") = ReadyString(objProjectBudget("RowNumber").ToString())
                p("IsParent") = ReadyString(objProjectBudget("IsParent").ToString())
                p("IsMaterial") = ReadyString(objProjectBudget("IsMaterial").ToString())
                p("ParentID") = ReadyString(objProjectBudget("ParentID").ToString())
                p("LineDescription") = ReadyString(objProjectBudget("LineDescription").ToString())
                p("AssociateSKUID") = IIf(objProjectBudget("AssociateSKUID").ToString() = "", "NULL", ReadyString(objProjectBudget("AssociateSKUID").ToString()))
                p("DMUnitOfMeasure") = ReadyString(objProjectBudget("UnitMeasure").ToString())
                p("ProjectedDMQty") = ReadyString(objProjectBudget("Qty").ToString())
                p("ProjectedDMUnitCost") = IIf(objProjectBudget("MaterialUnitCost").ToString() = "", "0", ReadyString(objProjectBudget("MaterialUnitCost").ToString()))
                p("DMVendorOrgPeopleID") = IIf(objProjectBudget("MaterialUnitCostVendor").ToString() = "", "NULL", ReadyString(objProjectBudget("MaterialUnitCostVendor").ToString()))
                p("ProjectedDMAmount") = IIf(objProjectBudget("MaterialAmount").ToString() = "", "0", ReadyString(objProjectBudget("MaterialAmount").ToString()))
                p("ProjectedDLUnitCost") = IIf(objProjectBudget("LaborUnitCost").ToString() = "", "0", ReadyString(objProjectBudget("LaborUnitCost").ToString()))
                p("DLVendorOrgPeopleID") = IIf(objProjectBudget("LaborUnitCostVendor").ToString() = "", "NULL", ReadyString(objProjectBudget("LaborUnitCostVendor").ToString()))
                p("ProjectedDLAmount") = IIf(objProjectBudget("LaborAmount").ToString() = "", "0", ReadyString(objProjectBudget("LaborAmount").ToString()))
                p("ProjectedTotalUnitCost") = IIf(objProjectBudget("TotalUnitCost").ToString() = "", "0", ReadyString(objProjectBudget("TotalUnitCost").ToString()))
                p("ProjectedTotalAmount") = IIf(objProjectBudget("TotalAmount").ToString() = "", "0", ReadyString(objProjectBudget("TotalAmount").ToString()))
                p("Status") = ReadyString(objProjectBudget("Status").ToString())
                p("IsMarkAsCompleted") = CInt2(objProjectBudget("IsMarkAsCompleted"))
                p("LastUpdated") = "getdate()"

                p("PointPersonID") = IIf(CStr2(objProjectBudget("PointPersonID")) = "", "NULL", ReadyString(objProjectBudget("PointPersonID")))
                p("Dependency") = ReadyString(objProjectBudget("Dependency"))
                p("ActualDuration") = ReadyString(objProjectBudget("ActualDuration"))
                p("DateStarted") = IIf(CStr2(objProjectBudget("DateStarted")) = "", "NULL", ReadyString(objProjectBudget("DateStarted")))
                p("DateCompleted") = IIf(CStr2(objProjectBudget("DateCompleted")) = "", "NULL", ReadyString(objProjectBudget("DateCompleted")))
                p("Importance") = ReadyString(objProjectBudget("Importance"))
                p("Urgency") = ReadyString(objProjectBudget("Urgency"))
                p("UnitMeasureID") = IIf(CStr2(objProjectBudget("UnitMeasureID")) = "", "NULL", ReadyString(objProjectBudget("UnitMeasureID")))
                p("IndentNo") = ReadyString(objProjectBudget("IndentNo"))

                If objProjectBudget("State").ToString() = "add" Then
                    strProjectBudgetID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strProjectBudgetID)
                    pSql = p.GetInsert()
                Else
                    strProjectBudgetID = objProjectBudget("ID").ToString()
                    pSql = p.GetUpdate(" ID = " & ReadyString(strProjectBudgetID))
                End If

                For Each remark As Object In objProjectBudget("Remarks")
                    If remark("ID") = "" Then
                        returnID = remark("ID")
                        If returnID = "" Then
                            If Not SaveLogs(strProjectBudgetID, "remarks", returnID, errmsg, remark) Then
                                Return errmsg
                            End If

                            For Each file As Object In objProjectBudget("Files")
                                Dim photos As HttpPostedFileBase
                                Dim folderName As String = Common.ServerPath & "ProjectPhotos\" & WebRequest.Instance.OrgID & "\" & strProjectBudgetID
                                Dim pSourceURL As String

                                For pp As Integer = 0 To Projectphotos.Count - 1
                                    photos = Projectphotos(pp)
                                    Dim filename As String = photos.FileName
                                    Dim photoname As String = Guid.NewGuid.ToString() & "." & filename.Substring(filename.LastIndexOf(".") + 1)
                                    pSourceURL = folderName & "\" & photoname

                                    If file("filename") = photos.FileName And file("status") = "new" And file("size") = photos.ContentLength Then
                                        If Not System.IO.Directory.Exists(folderName) Then
                                            System.IO.Directory.CreateDirectory(folderName)
                                        End If

                                        photos.SaveAs(pSourceURL)

                                        Dim ph As New InsertUpdate("LibraryProjectPhotos")

                                        ph("RemarksID") = ReadyString(returnID)
                                        ph("FileName") = ReadyString(photoname)
                                        ph("DateUploaded") = "getdate()"
                                        ph("UploadedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                                        ph("ID") = ReadyString(file("ID").ToString())

                                        Common.WebRequest.ProData.Execute(ph.GetInsert())

                                        Exit For
                                    End If
                                Next

                                If file("status") = "remove" Then
                                    pSourceURL = folderName & "\" & file("filename")
                                    Common.WebRequest.ProData.Execute("DELETE FROM LibraryProjectPhotos WHERE ID = " & ReadyString(file("ID").ToString()))
                                    System.IO.File.Delete(pSourceURL)
                                End If
                            Next file
                        End If
                        Exit For
                    End If
                Next remark

                For Each buddur As Object In objProjectBudget("BudgetedDuration")
                    If buddur("ID") = "" Then
                        returnID = buddur("ID")
                        If returnID = "" Then
                            If Not SaveLogs(strProjectBudgetID, "budgetedduration", returnID, errmsg, buddur) Then
                                Return errmsg
                            End If
                        End If
                        Exit For
                    End If
                Next buddur

                For Each budqty As Object In objProjectBudget("BudgetedQty")
                    If budqty("ID") = "" Then
                        returnID = budqty("ID")
                        If returnID = "" Then
                            If Not SaveLogs(strProjectBudgetID, "budgetedqty", returnID, errmsg, budqty) Then
                                Return errmsg
                            End If
                        End If
                        Exit For
                    End If
                Next budqty

                For Each accreqqty As Object In objProjectBudget("AccReqQty") ' LATEST REQUESTED  QTY
                    'do not add to requisition table if no linked sku
                    If objProjectBudget("AssociateSKUID").ToString() <> "" And accreqqty("State") = "new" Then
                        accreqqty.Add("ProjectBudgetID", strProjectBudgetID)
                        accreqqty.Add("HasSkuID", If(objProjectBudget("AssociateSKUID").ToString() <> "", True, False))
                        arrNewReqMaterial.Add(accreqqty)
                    Else
                        arrExistingReqMaterial.Add(accreqqty)
                    End If
                Next accreqqty

                For Each accusedupqty As Object In objProjectBudget("AccUsedUpQty")
                    If accusedupqty("ID") = "" Then
                        returnID = accusedupqty("ID")
                        If returnID = "" Then
                            If Not SaveLogs(strProjectBudgetID, "accusedupqty", returnID, errmsg, accusedupqty) Then
                                Return errmsg
                            End If
                        End If
                        Exit For
                    End If
                Next accusedupqty

                For Each labor As Object In objProjectBudget("LaborCost")
                    If labor("ID") = "" Then
                        returnID = labor("ID")
                        If returnID = "" Then
                            If Not SaveLogs(strProjectBudgetID, "laborcost", returnID, errmsg, labor) Then
                                Return errmsg
                            End If
                        End If
                        Exit For
                    End If
                Next labor

                For Each disburse As Object In objProjectBudget("RequestedAdvances")
                    returnID = disburse("ID")
                    If Not SaveLogs(strProjectBudgetID, "RequestedAdvances", returnID, errmsg, disburse) Then
                        Return errmsg
                    End If
                Next disburse

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                For Each sku As Object In objProjectBudget("AssociatedSKUs")
                    p = New InsertUpdate("LibraryProjectBudgetAssociatedSku")

                    p("ProjectBudgetID") = ReadyString(strProjectBudgetID)
                    p("SkuID") = ReadyString(sku("SkuID").ToString())
                    p("UomID") = ReadyString(sku("UomID").ToString())

                    If sku("State") = "new" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                    ElseIf sku("State") = "delete" Then
                        pSql = "DELETE FROM LibraryProjectBudgetAssociatedSku WHERE ID = " & ReadyString(sku("ID").ToString())
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Next sku

                If objProjectBudget("RowType").ToString() <> "subtotal" Then
                    'p = New InsertUpdate("LibraryFowstBinProduct")

                    'p("ID") = ReadyString(Guid.NewGuid.ToString())
                    'p("FowstID") = ReadyString(objProjectBudget("FOWSTID").ToString())
                    'p("BinID") = ReadyString(strProjectBudgetID)
                    'p("ProductID") = ReadyString(objProjectBudget("AssociateSKUID").ToString())
                    'p("IsDefault") = ReadyString(1)
                    'p("MaxPerProduct") = ReadyString(objProjectBudget("Qty").ToString())
                    'p("LastUpdated") = "getdate()"

                    'pSql = p.GetInsert()
                    'Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If ProjectBudgetCanvassMaterials.Length <> 0 Then

                        p = New InsertUpdate("MobizLibraryPimSupplier")

                        For ctrCanvassMaterialsB As Integer = 0 To ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA).Length - 1

                            p("OrgID") = ReadyString(ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(3))
                            p("FowstID") = ReadyString(ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(4))
                            p("SupplierID") = ReadyString(ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(2))
                            p("ProductID") = ReadyString(strProjectBudgetID)
                            p("Cost") = ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(0)
                            p("UnitMeasure") = ReadyString(objProjectBudget("UnitMeasure").ToString())
                            p("SKUID") = IIf(objProjectBudget("AssociateSKUID").ToString() = "", "NULL", ReadyString(objProjectBudget("AssociateSKUID").ToString()))
                            p("IsPreferredVendor") = ReadyString(ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(7))
                            p("LastUpdated") = "getDate()"

                            If ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(5).ToString() = "add" Or (ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(5).ToString() = "edit" And ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(6).ToString() = "") Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()

                            ElseIf ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(5).ToString() = "edit" Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(6)))
                            ElseIf ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(5).ToString() = "delete" And ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(6).ToString() <> "" Then
                                pSql = "DELETE FROM MobizLibraryPimSupplier WHERE ID = " & ReadyString(ProjectBudgetCanvassMaterials(ctrCanvassMaterialsA)(ctrCanvassMaterialsB)(6))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Next

                        ctrCanvassMaterialsA = ctrCanvassMaterialsA + 1
                    End If

                    If ProjectBudgetCanvassLabor.Length <> 0 Then

                        p = New InsertUpdate("LibraryProjectBudgetLaborCost")

                        For ctrCanvassLaborB As Integer = 0 To ProjectBudgetCanvassLabor(ctrCanvassLaborA).Length - 1
                            p("OrgID") = ReadyString(ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(3))
                            p("FowstID") = ReadyString(ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(4))
                            p("LaborID") = ReadyString(ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(2))
                            p("ProductID") = ReadyString(strProjectBudgetID)
                            p("Cost") = ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(0)
                            p("IsPreferredLaborer") = ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(7)
                            p("LastUpdated") = "getDate()"

                            If ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(5).ToString() = "add" Or (ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(5).ToString() = "edit" And ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(6).ToString() = "") Then
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                pSql = p.GetInsert()
                            ElseIf ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(5).ToString() = "edit" Then
                                pSql = p.GetUpdate(" ID = " & ReadyString(ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(6)))
                            ElseIf ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(5).ToString() = "delete" And ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(6).ToString() <> "" Then
                                pSql = "DELETE FROM LibraryProjectBudgetLaborCost WHERE ID = " & ReadyString(ProjectBudgetCanvassLabor(ctrCanvassLaborA)(ctrCanvassLaborB)(6))
                            End If
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Next

                        ctrCanvassLaborA = ctrCanvassLaborA + 1
                    End If

                    If ProjectBudgetTimeline.Length <> 0 Then
                        p = New InsertUpdate("LibraryProjectBudgetTimeline")

                        p("FOWSTID") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("FOWSTID").ToString())
                        p("OrgID") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("OrgID").ToString())
                        p("ProductID") = ReadyString(strProjectBudgetID)
                        p("Importance") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("Importance").ToString())
                        p("Urgency") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("Urgency").ToString())
                        p("TimeToComplete") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("TimeToComplete").ToString())
                        p("AssignedTo") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("AssignedTo").ToString())
                        p("AssignedToPeopleID") = IIf(ProjectBudgetTimeline(ctrTimeline)("AssignedToPeopleID").ToString() = "", "NULL", ReadyString(ProjectBudgetTimeline(ctrTimeline)("AssignedToPeopleID").ToString()))
                        p("Status") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("Status").ToString())
                        p("Dependency") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("Dependency").ToString())
                        p("Tags") = ReadyString(ProjectBudgetTimeline(ctrTimeline)("Tags").ToString())
                        p("LastUpdated") = "getDate()"

                        If ProjectBudgetTimeline(ctrTimeline)("tlrowid") = "" Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            pSql = p.GetInsert()
                        Else
                            pSql = p.GetUpdate(" ID = " & ReadyString(ProjectBudgetTimeline(ctrTimeline)("tlrowid")))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                        ctrTimeline = ctrTimeline + 1

                    End If
                End If

            Next objProjectBudget

            If arrNewReqMaterial.Count > 0 Then
                pSql = "SELECT COUNT(ID) as Cnt FROM LibraryProjectRequisition"
                Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                Dim reqno As Integer = IIf(rows.Count > 0, CInt2(rows(0)("Cnt")) + 1, 1)

                pSql = "SELECT SysGenProjectCode FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(ProjectID)
                Dim code As String = Common.WebRequest.ProData.ExecuteRow(pSql)(0)
                Dim reqid As String = Guid.NewGuid.ToString()

                p = New InsertUpdate("LibraryProjectRequisition")

                p("ProjectID") = ReadyString(ProjectID)
                p("RequestedDate") = "getdate()"
                p("RequestedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("RequisitionNumber") = ReadyString("RQ" & code & "-" & reqno)
                p("ID") = ReadyString(reqid)

                Common.WebRequest.ProData.Execute(p.GetInsert())

                For Each req As Object In arrNewReqMaterial
                    p = New InsertUpdate("LibraryProjectMaterialsRequisition")

                    p("OrgID") = ReadyString(WebRequest.Instance.OrgID)
                    p("ProjectID") = ReadyString(ProjectID)
                    p("RequisitionID") = ReadyString(reqid)
                    p("ProjectBudgetID") = ReadyString(req("ProjectBudgetID"))
                    p("RequestedValue") = ReadyString(req("Qty"))
                    p("HasSkuID") = ReadyString(CInt2(req("HasSkuID")))
                    p("LastUpdated") = "getdate()"
                    p("Remarks") = ReadyString(req("Remarks"))
                    p("DateNeeded") = ReadyString(req("DateNeeded"))
                    p("TimeNeededFrom") = IIf(req("TimeNeededFrom") = "", "NULL", ReadyString(req("TimeNeededFrom")))
                    p("TimeNeededTo") = IIf(req("TimeNeededTo") = "", "NULL", ReadyString(req("TimeNeededTo")))
                    p("RequestedByID") = ReadyString(WebSession.Instance.PeopleUserID)

                    If CBool2(req("HasSkuID")) Then
                        p("Status") = ReadyString("requested")
                    Else
                        p("Status") = ReadyString("pending")
                    End If
                    p("ID") = ReadyString(Guid.NewGuid.ToString())

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Next
            End If

            For Each req As Object In arrExistingReqMaterial
                If CStr2(req("ID")) = "" Then
                    Continue For 'ignore if wala pay ID (ID sa requisition table)
                End If
                p = New InsertUpdate("LibraryProjectMaterialsRequisition")

                p("HasSkuID") = ReadyString(CInt2(req("HasSkuID")))
                p("Status") = ReadyString(req("State"))

                If CBool2(req("HasSkuID")) Then
                    p("LastUpdated") = "getdate()"
                    'Else
                    '    p("Status") = ReadyString("pending")
                End If

                Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(req("ID"))))
            Next

            Dim a As New InsertUpdate("MobizLibraryOrganizationFowst")
            Dim aSql As String

            a("LastUpdated") = "getDate()"
            aSql = a.GetUpdate(" ID = " & ReadyString(FowstID))
            Common.WebRequest.ProData.ExecuteSQL(aSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryProjectBudget'"
            Dim row As Integer = Common.WebRequest.ProData.ExecuteSQL(pSql)

            If row = 0 Then
                pSql = "INSERT INTO TablesLastUpdated(ID, TableName, FileName, LastUpdated) " & vbCrLf &
                        "VALUES(" & ReadyString(Guid.NewGuid.ToString()) & ", 'LibraryProjectBudget', NULL, GetDate())"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()
            Return "saved"

        Catch ex As Exception
            AppendToDebug(ex.Message & " " & ex.StackTrace)
            Return ex.Message
        End Try
    End Function

    Function SaveLogs(ByVal LineProjectBudgetID As String,
                      ByVal LogType As String,
                      ByRef rowID As String,
                      ByRef errmsg As String,
                      ByVal data As Object) As Boolean

        Try
            Dim p As New InsertUpdate("LibraryProjectLogs")

            If CStr2(LogType) = "remarks" Then
                p("Remarks") = ReadyString(data("Remarks"))
            End If

            If CStr2(LogType) = "budgetedduration" Then
                p("Duration") = ReadyString(data("Duration"))
                p("StateIn") = ReadyString(data("StateIn"))
            End If

            If CStr2(LogType) = "budgetedqty" Then
                p("Qty") = ReadyString(data("Qty"))
                'p("UomID") = ReadyString(data("UomID"))
            End If

            If CStr2(LogType) = "accreqqty" Then
                p("RequestedByID") = ReadyString(data("RequestedByID"))
                p("DateNeeded") = ReadyString(data("DateNeeded"))
                p("TimeNeededFrom") = IIf(data("TimeNeededFrom") = "", "NULL", ReadyString(data("TimeNeededFrom")))
                p("TimeNeededTo") = IIf(data("TimeNeededTo") = "", "NULL", ReadyString(data("TimeNeededTo")))
                p("Status") = ReadyString(data("State"))
            End If

            If CStr2(LogType) = "accreqqty" Or CStr2(LogType) = "accusedupqty" Then
                p("Qty") = ReadyString(data("Qty"))
                p("Remarks") = ReadyString(data("Remarks"))
            End If

            If CStr2(LogType) = "laborcost" Then
                p("Amount") = ReadyString(CDec2(data("Amount")))
            End If

            If CStr2(LogType) = "RequestedAdvances" Then
                p("Amount") = ReadyString(CDec2(data("Amount")))
                p("Remarks") = ReadyString(data("Remarks"))
                p("PaymentBalance") = ReadyString(CDec2(data("PaymentBalance")))
                p("RequestedByID") = ReadyString(data("RequestedByID"))
                p("Status") = ReadyString(data("State"))
            End If

            p("ProjectBudgetID") = ReadyString(LineProjectBudgetID)
            p("LogType") = ReadyString(LogType)

            p("ModifiedByID") = ReadyString(WebSession.Instance.PeopleUserID)

            If CStr2(rowID) = "" Then
                p("LastUpdated") = "getdate()"
                rowID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(rowID)

                Common.WebRequest.ProData.Execute(p.GetInsert())
            Else
                If data("State") = "cancelled" Then
                    p("LastUpdated") = "getdate()"
                End If
                Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(data("ID").ToString())))
            End If

            Return True
        Catch ex As Exception
            errmsg = ex.Message
            Return False
        End Try

    End Function

    Function UploadProjectFiles() As String
        Try
            Common.WebRequest.ProData.BeginTransaction()

            If CStr2(ProjectID) = "" Then
                Return "Please select Project first"
            End If

            For Each file As Object In ProjectFilesDetails
                Dim photos As HttpPostedFileBase
                Dim folderName As String = Common.ServerPath & "ProjectFiles\" & WebRequest.Instance.OrgID & "\" & ProjectID
                Dim pSourceURL As String

                For pp As Integer = 0 To ProjectFiles.Count - 1
                    photos = ProjectFiles(pp)
                    Dim filename As String = photos.FileName
                    Dim photoname As String = file("rename") & "." & filename.Substring(filename.LastIndexOf(".") + 1)
                    pSourceURL = folderName & "\" & photoname

                    If file("filename") = photos.FileName And file("status") = "new" And file("size") = photos.ContentLength Then
                        If Not System.IO.Directory.Exists(folderName) Then
                            System.IO.Directory.CreateDirectory(folderName)
                        End If

                        Try
                            photos.SaveAs(pSourceURL)
                        Catch ex As Exception
                            Return "File name <b>" & file("rename") & "</b> already exist"
                        End Try

                        Dim ph As New InsertUpdate("LibraryProjectPhotos")

                        ph("ProjectID") = ReadyString(ProjectID)
                        ph("FileName") = ReadyString(photoname)
                        ph("FileDescription") = ReadyString(file("filedescription"))
                        ph("DateUploaded") = "getdate()"
                        ph("UploadedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        ph("ID") = ReadyString(Guid.NewGuid.ToString())

                        Common.WebRequest.ProData.Execute(ph.GetInsert())

                        Exit For
                    End If
                Next

                If file("status") = "remove" Then
                    pSourceURL = folderName & "\" & file("filename")
                    Common.WebRequest.ProData.Execute("DELETE FROM LibraryProjectPhotos WHERE ID = " & ReadyString(file("ID").ToString()))
                    System.IO.File.Delete(pSourceURL)
                End If
            Next file

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function

    Function SaveNewClient() As String
        Try
            Dim p As New InsertUpdate("MobizLibraryOrganization")
            Dim clientID As String = Guid.NewGuid.ToString()

            Common.WebRequest.ProData.BeginTransaction()

            If ClientData("ClientType") = "org" Then
                p("CompanyName") = ReadyString(ClientData("OrgName"))
                p("TinNumber") = ReadyString(ClientData("TIN"))
                p("OfficeAddressBuildingNo") = ReadyString(ClientData("HomeNo"))
                p("OfficeAddressStreet") = ReadyString(ClientData("StreetName"))
                p("OfficeAddressCityID") = If(ClientData("City") = "", "NULL", ReadyString(ClientData("City")))
                p("OfficeAddressCountryID") = If(ClientData("Country") = "", "NULL", ReadyString(ClientData("Country")))
                p("IsOrganization") = "1"
            Else
                p("PersonFirstName") = ReadyString(ClientData("Firstname"))
                p("PersonMiddleName") = ReadyString(ClientData("Middlename"))
                p("PersonSurname") = ReadyString(ClientData("Lastname"))
                p("IsOrganization") = "0"
            End If

            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "getdate()"
            p("IsActive") = "1"
            p("ID") = ReadyString(clientID)

            Common.WebRequest.ProData.Execute(p.GetInsert())

            If ClientData("ClientType") = "person" Then
                If ClientData("HomeNo") <> "" Or ClientData("StreetName") <> "" Then
                    p = New InsertUpdate("MobizLibraryPeopleAddress")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PeopleID") = ReadyString(clientID)
                    p("HouseBuildingNo") = ReadyString(ClientData("HomeNo"))
                    p("Street") = ReadyString(ClientData("StreetName"))
                    p("CityID") = If(ClientData("City") = "", "NULL", ReadyString(ClientData("City")))
                    p("CountryID") = If(ClientData("Country") = "", "NULL", ReadyString(ClientData("Country")))
                    p("AddressType") = ReadyString("Non-Permanent Address")
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                If ClientData("MobileNo") <> "" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PeopleID") = ReadyString(clientID)
                    p("CountryID") = If(ClientData("Country") = "", "NULL", ReadyString(ClientData("Country")))
                    p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(ClientData("MobileNo"))("ID"))
                    p("ContactNo") = ReadyString(LibraryTelcoPrefixes.ParseNumber(ClientData("MobileNo")))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("ContactType") = ReadyString("Work Mobile")
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                If ClientData("EmailAdd") <> "" Then
                    p = New InsertUpdate("MobizLibraryPeopleContactEmail")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("PeopleID") = ReadyString(clientID)
                    p("EmailAddress") = ReadyString(ClientData("EmailAdd"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("EmailType") = ReadyString("Work Email")
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

            Else
                If ClientData("MobileNo") <> "" Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactMobile")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("OrganizationID") = ReadyString(clientID)
                    p("CountryID") = If(ClientData("Country") = "", "NULL", ReadyString(ClientData("Country")))
                    p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(ClientData("MobileNo"))("ID"))
                    p("ContactNo") = ReadyString(LibraryTelcoPrefixes.ParseNumber(ClientData("MobileNo")))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("ContactType") = ReadyString("Work Mobile")
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                If ClientData("EmailAdd") <> "" Then
                    p = New InsertUpdate("MobizLibraryOrganizationContactEmail")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("OrganizationID") = ReadyString(clientID)
                    p("EmailAddress") = ReadyString(ClientData("EmailAdd"))
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    p("EmailType") = ReadyString("Work Email")
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

            End If

            p = New InsertUpdate("LibraryOrganizationCustomer")

            p("ActivationDate") = "GetDate()"
            p("DeActivated") = "0"
            p("IsBan") = ReadyString("0")
            p("LastUpdated") = "getdate()"
            p("OrganizationID") = ReadyString(clientID)
            p("ID") = ReadyString(Guid.NewGuid.ToString())
            Common.WebRequest.ProData.Execute(p.GetInsert())

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved|" & clientID & "|" & IIf(ClientData("ClientType") = "org", ClientData("OrgName"), ClientData("Firstname") & " " & ClientData("Middlename") & " " & ClientData("Lastname"))
        Catch ex As Exception
            AppendToLog(ex.Message)
            Return ex.Message
        End Try
    End Function
End Class