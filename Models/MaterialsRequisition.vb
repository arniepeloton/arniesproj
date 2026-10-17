Public Class MaterialsRequisition
    Public Property MaxReturnRow As Integer = 50
    Public Property OrganizationID As String
    Public Property ProjectID As String
    Public Property ProductID As String
    Public Property ParentID As String
    Public Property RequisitionID As String
    Public Property IsApprove As String
    Public Property ApprovedBy As String
    Public Property IsSearch As String
    Public Property SearchText As String
    Public Property ApprovedMatReq As Object
    Public Property CreatedBy As String
    Public Property RequestedBy As String
    Public Property InvoiceType As String
    Public Property DeliveryDate As String
    Public Property BillingAddress As String
    Public Property BillingAddressID As String
    Public Property DeliveryAddress As String
    Public Property DeliveryAddressID As String
    Public Property Remarks As String
    Public Property ObjVendor As Object
    Public Property VendorID As String
    Public Property ObjRequestedMaterials As Object
    Public Property TotalRequestedMaterials As String
    Public Property RequestedMaterialsNo As String

    Function SaveMaterialsRequisition() As String
        Dim pSql As String
        Dim p As New InsertUpdate("LibraryProjectRequisition")
        Dim reqID As String = Guid.NewGuid.ToString()

        Try
            p("ID") = ReadyString(reqID)
            p("ProjectID") = ReadyString(ProjectID)
            p("RequisitionNumber") = ReadyString(RequestedMaterialsNo)
            p("RequestedDate") = "getdate()"
            p("RequestedBy") = ReadyString(RequestedBy)
            p("RequisitionValue") = ReadyString(TotalRequestedMaterials)
            p("Status") = ReadyString("New")
            pSql = p.GetInsert()

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For i As Integer = 0 To ObjRequestedMaterials.Length - 1
                p = New InsertUpdate("LibraryProjectMaterialsRequisition")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("OrgID") = ReadyString(OrganizationID)
                p("ProjectID") = ReadyString(ObjRequestedMaterials(i)("ProjectID").ToString())
                p("ParentID") = ReadyString(ObjRequestedMaterials(i)("ParentID").ToString())
                p("ProductID") = ReadyString(ObjRequestedMaterials(i)("DescriptionID").ToString())
                p("VendorID") = ReadyString(ObjRequestedMaterials(i)("VendorID").ToString())
                p("RequisitionID") = ReadyString(reqID)
                p("RequestedValue") = ReadyString(ObjRequestedMaterials(i)("RequestedValue").ToString())
                p("ApprovedRequestedQty") = ReadyString(0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Function GetItemPerVendor() As Object
        Dim pOut As JsonDictionary
        Dim jss = New JavaScriptSerializer()
        Dim arrayx(ObjVendor.Length - 1) As Object

        pOut = New JsonDictionary

        For a As Integer = 0 To ObjVendor.Length - 1
            VendorID = ObjVendor(a)("ID").ToString()
            RequisitionID = ObjVendor(a)("ReqID").ToString()
            arrayx(a) = jss.Deserialize(Of Object)(GetJson(GetItemsNow()))
        Next

        pOut.Add("Results", arrayx)

        Return pOut.Serialize()
    End Function

    Function GetJson(ByVal dt As DataTable) As String
        Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
    End Function

    Function GetItemsNow() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ItemCode", SqlDbType.UniqueIdentifier)
                    '.Expression = "pimprod.ProductNo"
                    .Expression = "projbud.ItemNumber"
                End With
                With .Add("ItemDescription", SqlDbType.VarChar)
                    .Expression = "projbud.LineDescription"
                End With
                With .Add("Qty", SqlDbType.UniqueIdentifier)
                    .Expression = "matreq.ApprovedRequestedQty"
                End With
                With .Add("UoM", SqlDbType.VarChar)
                    .Expression = "projbud.DMUnitOfMeasure"
                End With
                With .Add("Price", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 pimsup.Cost " & vbCrLf &
                                    " FROM MobizLibraryPIMSupplier as pimsup " & vbCrLf &
                                    " WHERE projbud.ID = pimsup.ProductID " & vbCrLf &
                                    " AND pimsup.SupplierID = " & ReadyString(VendorID) & vbCrLf &
                                    ")"
                End With
                With .Add("VendorName", SqlDbType.VarChar)
                    .Expression = "ISNULL(org.CompanyName, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname)"
                End With
                With .Add("VendorID", SqlDbType.VarChar)
                    .Expression = "ISNULL(org.ID, ppol.ID)"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectBudget as projbud " & vbCrLf &
            " INNER JOIN LibraryProjectMaterialsRequisition as matreq " & vbCrLf &
            "   ON projbud.ID = matreq.ProductID " & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct as pimprod " & vbCrLf &
            "   ON pimprod.ID = projbud.AssociateSKUID " & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization as org " & vbCrLf &
            "   ON org.ID = matreq.VendorID " & vbCrLf &
            " LEFT JOIN MobizLibraryPeople as ppol " & vbCrLf &
            "   ON ppol.ID = matreq.VendorID " & vbCrLf

            .SqlWhere = " WHERE matreq.VendorID = " & ReadyString(VendorID) & vbCrLf

            If Common.WebRequest.Data("IsPO") <> Nothing Then
                If Common.WebRequest.Data("IsPO") = True Then
                    .SqlFrom += " LEFT JOIN LibraryProjectPO as po " & vbCrLf &
                                "   ON po.RequisitionID = matreq.RequisitionID "

                    .SqlWhere += " AND po.ID = " & ReadyString(Common.WebRequest.Data("POID"))
                End If
            Else
                If Common.WebRequest.Data("IsPO") = Nothing Then
                    .SqlWhere += " AND matreq.RequisitionID = " & ReadyString(RequisitionID)
                End If
            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SavePORequiredDetails() As String
        Dim pSql As String
        Dim p As New InsertUpdate("LibraryProjectRequisition")
        Dim pDT As DataTable

        Try
            p("IsPOCreated") = "1"
            pSql = p.GetUpdate(" ID = " & ReadyString(RequisitionID))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For i As Integer = 0 To ObjVendor.Length - 1
                p = New InsertUpdate("LibraryProjectPO")

                p("ProjectID") = ReadyString(ObjVendor(i)("ProjectID").ToString())
                p("SupplierID") = ReadyString(ObjVendor(i)("VendorID").ToString())
                p("InvoiceType") = ReadyString(InvoiceType)
                p("POCreatedBy") = ReadyString(CreatedBy)
                p("DeliveryDate") = ReadyString(DeliveryDate)
                p("BillingAddress") = ReadyString(BillingAddress)
                p("BillingAddressID") = ReadyString(BillingAddressID)
                p("DeliveryAddress") = ReadyString(DeliveryAddress)
                p("DeliveryAddressID") = ReadyString(DeliveryAddressID)
                p("Remarks") = ReadyString(Remarks)
                p("RequisitionID") = ReadyString(RequisitionID)
                p("PONumber") = ReadyString(ObjVendor(i)("PONum").ToString())
                p("DateCreated") = "getdate()"

                pSql = "SELECT * FROM LibraryProjectPO WHERE ProjectID = " & ReadyString(ObjVendor(i)("ProjectID").ToString()) & vbCrLf &
                        " AND SupplierID = " & ReadyString(ObjVendor(i)("VendorID").ToString()) & vbCrLf &
                        " AND RequisitionID = " & ReadyString(RequisitionID)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If pDT.Rows.Count = 0 Then
                    p("ID") = ReadyString(ObjVendor(i)("POID").ToString())
                    pSql = p.GetInsert()
                Else
                    p("PONumber") = ReadyString(pDT.Rows(0)("PONumber"))
                    pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0)("ID")))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            Common.WebRequest.ProData.CommitTransaction()
            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function GetEachVendor() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                End With
                With .Add("SupplierOrgName", SqlDbType.VarChar)
                    .Expression = " org.CompanyName "
                End With
                With .Add("SupplierOrgAddress", SqlDbType.UniqueIdentifier)
                    .Expression = "org.OfficeAddressStreet"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(org.OfficeAddressStreet, '') " &
                                    "+ ', ' + ISNULL(city.Name, '') + ', ' + ISNULL(country.Name, ''))"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectRequisition as req " & vbCrLf &
            " INNER JOIN LibraryProjectMaterialsRequisition as matreq " & vbCrLf &
            "   ON req.ID = matreq.RequisitionID " & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization as org " & vbCrLf &
            "   ON org.ID = matreq.VendorID " & vbCrLf &
            " LEFT JOIN MobizLibraryCities as city " & vbCrLf &
            "   ON city.ID = org.OfficeAddressCityID " & vbCrLf &
            " LEFT JOIN MobizLibraryCountries as country " & vbCrLf &
            "   ON country.ID = org.OfficeAddressCountryID " & vbCrLf &
            " LEFT JOIN MobizLibraryPeople as ppl " & vbCrLf &
            "   ON ppl.ID = matreq.VendorID " & vbCrLf &
            " LEFT JOIN MobizLibraryPeopleContactTel as ppltel " & vbCrLf &
            "   ON ppltel.PeopleID = ppl.ID AND ppltel.CountryID = country.ID " & vbCrLf &
            " LEFT JOIN MobizLibraryAreaCodes as areacodes " & vbCrLf &
            "   ON areacodes.ID = ppltel.AreaCodeID "

            .SqlWhere = " WHERE req.ID = " & ReadyString(RequisitionID) & vbCrLf
            .SqlGroup = " GROUP BY org.ID, org.CompanyName, org.OfficeAddressStreet, city.Name, country.Name "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetOrgDetails() As String
        Dim clsOrg As New Organization
        Dim pOut As JsonDictionary

        pOut = New JsonDictionary

        pOut.Add("OrgDetails", clsOrg.getOrganizationInfo(OrganizationID))
        pOut.Add("OrgTelDetails", clsOrg.getOrgContactTelList(OrganizationID))

        Return pOut.Serialize()
    End Function

    Function GetProjectDetails() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstOrgName", SqlDbType.VarChar)
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
                With .Add("ContactNo", SqlDbType.VarChar)
                    .Expression = " (SELECT STUFF(" & vbCrLf &
                        " 	(SELECT '/ ' + ISNULL(NULLIF('+' + LTRIM(RTRIM(ctr.CallingCode)) + '.', ' '), ' ') + ISNULL(NULLIF(LTRIM(RTRIM(area.CallingCode)) + '.','.'), '') + ISNULL(NULLIF(LTRIM(RTRIM(fowsttel.ContactNo)) + '',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(fowsttel.ExtensionNo)) + '',' '), '') " & vbCrLf &
                        "     FROM MobizLibraryOrganizationFOWSTTel as fowsttel " & vbCrLf &
                        " 	INNER JOIN MobizLibraryCountries as ctr " & vbCrLf &
                        " 	    ON fowsttel.CountryID = ctr.ID " & vbCrLf &
                        "   INNER JOIN MobizLibraryAreaCodes as area" & vbCrLf &
                        " 	    ON fowsttel.AreaCodeID = area.ID " & vbCrLf &
                        "   WHERE fowsttel.FOWSTID = " & ReadyString(ProjectID) & vbCrLf &
                        " 	FOR XML PATH('')" & vbCrLf &
                        "     ), 1, 2, '')" & vbCrLf &
                        " )"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
            " INNER JOIN MobizLibraryOrganization org ON (fowst.OrganizationID = org.ID) " &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID) " &
            " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
            " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) "

            If CInt(WebRequest.Instance.Data("IsSubscriber")) = 1 Then
                .SqlFrom &= " INNER JOIN dbo.MobizLibrarySubscription sub ON (org.ID = sub.OrganizationID) "
            End If

            .SqlWhere = " WHERE fowst.ID = " & ReadyString(ProjectID)
            .SqlOrder = " ORDER BY Address"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetApprovedMaterials() As String
        Dim pOut As JsonDictionary

        pOut = New JsonDictionary

        pOut.Add("HeaderDetails", GetRequestMaterialsHeader())
        pOut.Add("ApprovedMaterials", GetApprovedMaterialsList())

        Return pOut.Serialize()
    End Function

    Function GetApprovedMaterialsList() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("MaterialsRequestedID", SqlDbType.UniqueIdentifier)
                    .Expression = "matreq.ID"
                End With

                With .Add("ProjectGrouping", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 LineDescription FROM LibraryProjectBudget as projbud2 WHERE matreq.ParentID = projbud2.ItemNumber)"
                End With

                With .Add("RequestedValue", SqlDbType.VarChar)
                    .Expression = "matreq.RequestedValue"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "projbud.LineDescription"
                End With

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "projbud.DMUnitOfMeasure"
                End With

                With .Add("BudgetedQty", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedDMQty"
                End With

                With .Add("DateRequested", SqlDbType.VarChar)
                    .Expression = "matreq.LastUpdated"
                End With

                With .Add("VendorPeople", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol2.FirstName, '') + ' ' + ISNULL(ppol2.MiddleName, '') + ' ' + ISNULL(ppol2.Surname, '')"
                End With

                With .Add("VendorOrg", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With

                With .Add("ApprovedRequestedQty", SqlDbType.VarChar)
                    .Expression = "matreq.ApprovedRequestedQty"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectRequisition AS req " & vbCrLf &
                        " INNER JOIN LibraryProjectMaterialsRequisition AS matreq " & vbCrLf &
                        "   ON req.ID = matreq.RequisitionID " & vbCrLf &
                        " INNER JOIN LibraryProjectBudget AS projbud " & vbCrLf &
                        "   ON projbud.ID = matreq.ProductID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople AS ppol " & vbCrLf &
                        "   ON ppol.ID = matreq.VendorID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization AS org " & vbCrLf &
                        "   ON org.ID = matreq.VendorID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol2 " & vbCrLf &
                        "   ON matreq.VendorID = ppol2.ID " & vbCrLf

            .SqlWhere = "WHERE req.ID = " & ReadyString(RequisitionID) & vbCrLf &
                        " AND req.ProjectID = " & ReadyString(ProjectID) & vbCrLf &
                        " AND req.Status = 'Approved' "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SaveApprovedMaterialsRequisition() As String
        Dim pSql As String
        Dim p As New InsertUpdate("LibraryProjectRequisition")

        Try
            p("Status") = ReadyString(IsApprove)
            p("ApprovedBy") = ReadyString(ApprovedBy)
            p("DateDisApproved") = "getDate()"
            pSql = p.GetUpdate(" ID = " & ReadyString(RequisitionID))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each objApprovedMatReq As Object In ApprovedMatReq
                Dim matreqid As String
                p = New InsertUpdate("LibraryProjectMaterialsRequisition")

                p("ApprovedRequestedQty") = ReadyString(objApprovedMatReq("ApprovedRequestedQty").ToString())
                matreqid = objApprovedMatReq("MatReqID").ToString()
                pSql = p.GetUpdate(" ID = " & ReadyString(matreqid))

                Common.WebRequest.ProData.ExecuteSQL(pSql)

            Next objApprovedMatReq

            Common.WebRequest.ProData.CommitTransaction()
            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function GetRequestDetails() As DataTable
        Return GetRequestMaterialsHeader()
    End Function

    Function GetRequestMaterialsHeader() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "req.ID"
                End With

                With .Add("ProjectID", SqlDbType.VarChar)
                    .Expression = "orgfowst.ID"
                End With

                With .Add("ProjectName", SqlDbType.VarChar)
                    .Expression = "orgfowst.Name"
                End With

                With .Add("RequisitionNumber", SqlDbType.VarChar)
                    .Expression = "req.RequisitionNumber"
                End With

                With .Add("RequestedDate", SqlDbType.VarChar)
                    .Expression = "req.RequestedDate"
                End With

                With .Add("RequestedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.FirstName, '') + ' ' + ISNULL(ppol.MiddleName, '') + ' ' + ISNULL(ppol.Surname, '')"
                End With

                With .Add("RequisitionValue", SqlDbType.UniqueIdentifier)
                    .Expression = "req.RequisitionValue"
                End With

                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "req.Status"
                End With

                With .Add("IsPOCreated", SqlDbType.VarChar)
                    .Expression = "req.IsPOCreated"
                End With

                With .Add("ApprovedDate", SqlDbType.VarChar)
                    .Expression = "req.DateDisApproved"
                End With

                With .Add("RowNumber", SqlDbType.Int)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY req.RequestedDate DESC)"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectRequisition AS req " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFOWST AS orgfowst " & vbCrLf &
                        "   ON req.ProjectID = orgfowst.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol " & vbCrLf &
                        "   ON req.RequestedBy = ppol.ID " & vbCrLf

            If ProjectID <> "" Then
                .SqlWhere = "WHERE ProjectID = " & ReadyString(ProjectID) & vbCrLf

                If IsSearch <> Nothing Then
                    If IsSearch = True Then
                        .SqlWhere += " AND (orgfowst.Name LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    " OR req.RequisitionNumber LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    " OR ppol.FirstName LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    " OR ppol.MiddleName LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    " OR ppol.Surname LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    " OR req.RequisitionValue LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    " OR req.Status LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    " OR convert(datetime, req.RequestedDate, 9) LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                    ")"
                    End If
                End If

            ElseIf RequisitionID <> "" Then
                .SqlWhere = "WHERE req.ID = " & ReadyString(RequisitionID) & vbCrLf

            ElseIf IsSearch = True Then
                .SqlWhere = " WHERE (orgfowst.Name LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                " OR req.RequisitionNumber LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                " OR ppol.FirstName LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                " OR ppol.MiddleName LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                " OR ppol.Surname LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                " OR req.RequisitionValue LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                " OR req.Status LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                " OR convert(datetime, req.RequestedDate, 9) LIKE " & ReadyStringWild(SearchText) & vbCrLf &
                                ")"
            End If

            .SqlOrder = "ORDER BY req.RequestedDate DESC"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetRequestMaterials() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("MaterialsRequestedID", SqlDbType.UniqueIdentifier)
                    .Expression = "matreq.ID"
                End With

                With .Add("ProjectGrouping", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 LineDescription FROM LibraryProjectBudget as projbud2 WHERE matreq.ParentID = projbud2.ItemNumber)"
                End With

                With .Add("RequestedValue", SqlDbType.VarChar)
                    .Expression = "matreq.RequestedValue"
                End With

                With .Add("ApprovedQty", SqlDbType.VarChar)
                    .Expression = "matreq.ApprovedRequestedQty"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "projbud.LineDescription"
                End With

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "projbud.DMUnitOfMeasure"
                End With

                With .Add("BudgetedQty", SqlDbType.VarChar)
                    .Expression = "projbud.ProjectedDMQty"
                End With

                With .Add("DateRequested", SqlDbType.VarChar)
                    .Expression = "matreq.LastUpdated"
                End With

                With .Add("VendorPeople", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol2.FirstName, '') + ' ' + ISNULL(ppol2.MiddleName, '') + ' ' + ISNULL(ppol2.Surname, '')"
                End With

                With .Add("VendorOrg", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With

                With .Add("VendorID", SqlDbType.VarChar)
                    .Expression = "ISNULL(org.ID, ppol2.ID)"
                End With

                With .Add("Accumulated", SqlDbType.VarChar)
                    .Expression = "(SELECT SUM(matreq2.ApprovedRequestedQty) " & vbCrLf &
                                    " FROM LibraryProjectMaterialsRequisition as matreq2 " & vbCrLf &
                                    " WHERE matreq2.ProductID = matreq.ProductID " & vbCrLf &
                                    " AND matreq2.ProjectID = matreq.ProjectID )"
                    '.Expression = "(SELECT ISNULL(SUM(ApprovedRequestedQty), 0) FROM LibraryProjectMaterialsRequisition  as matreq2 " & vbCrLf &
                    '                " INNER JOIN LibraryProjectRequisition as req2 " & vbCrLf &
                    '                "   ON req2.ID = matreq2.RequisitionID " & vbCrLf &
                    '                " WHERE matreq.ProductID = matreq2.ProductID " & vbCrLf &
                    '                " AND req2.ID = " & ReadyString(RequisitionID) & vbCrLf &
                    '                " AND req2.Status = 'Approved')" & vbCrLf
                End With

                With .Add("AccumulatedRequestedQty", SqlDbType.VarChar)
                    .Expression = "(SELECT SUM(accmatreq.RequestedValue) FROM LibraryProjectMaterialsRequisition accmatreq WHERE accmatreq.ProductID = matreq.ProductID)"
                End With

                With .Add("PendingRequestedQty", SqlDbType.VarChar)
                    .Expression = "(SELECT COUNT(pendingmatreq.RequestedValue) FROM LibraryProjectMaterialsRequisition pendingmatreq WHERE pendingmatreq.ProductID = matreq.ProductID AND pendingmatreq.ApprovedRequestedQty = 0)"
                End With

                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "matreq.ProductID"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectMaterialsRequisition as matreq " & vbCrLf &
                        " INNER JOIN LibraryProjectBudget as projbud " & vbCrLf &
                        "   ON matreq.ProductID = projbud.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol2 " & vbCrLf &
                        "   ON matreq.VendorID = ppol2.ID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org " & vbCrLf &
                        "   ON matreq.VendorID = org.ID" & vbCrLf

            .SqlWhere = "WHERE matreq.RequisitionID = " & ReadyString(RequisitionID) & vbCrLf &
                        " AND matreq.ProjectID = " & ReadyString(ProjectID) & vbCrLf

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProductRequestLogs() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("RequisitionNumber", SqlDbType.UniqueIdentifier)
                    .Expression = "RequisitionNumber"
                End With

                With .Add("RequestedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(ppol.FirstName, '') + ' ' + ISNULL(ppol.MiddleName, '') + ' ' + ISNULL(ppol.Surname, '')"
                End With

                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "req.Status"
                End With

                With .Add("RequestedValue", SqlDbType.VarChar)
                    .Expression = "matreq.RequestedValue"
                End With

                With .Add("RequestedDate", SqlDbType.VarChar)
                    .Expression = "req.RequestedDate"
                End With
            End With

            .SqlFrom = "FROM LibraryProjectRequisition as req " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON req.RequestedBy = ppol.ID " & vbCrLf &
                        " INNER JOIN LibraryProjectMaterialsRequisition as matreq ON matreq.RequisitionID = req.ID " & vbCrLf

            .SqlWhere = "WHERE matreq.ProductID = " & ReadyString(ProductID) & vbCrLf &
                        " AND req.Status = 'New'"
            .SqlOrder = "ORDER BY req.RequestedDate desc"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProjectBudgetByProject() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT * FROM LibraryProjectBudget WHERE OrgID = " & ReadyString(Common.WebRequest.Data("OrgID")) & vbCrLf &
                " AND FOWSTID = " & ReadyString(Common.WebRequest.Data("ProjectID")) & vbCrLf &
                " AND ProjectOwnerOrgPersonID IS NULL AND ProjectOwnerFowstID IS NULL"

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetMaterialsVendor() As DataTable
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
                        " AND projbud.ParentID = " & ReadyString(ParentID) &
                        " ORDER BY projbud.RowNumber ASC"

            pDT = .Requery

        End With

        Return pDT
    End Function

End Class