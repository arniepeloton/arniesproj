Public Class ProjectPurchaseOrder
    Public Property FilterBy As String
    Public Property Searchable As Boolean
    Public Property SearchedText As String

    Function GetPO() As Object
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "po.ID"
                End With
                With .Add("PONumber", SqlDbType.VarChar)
                    .Expression = "po.PONumber"
                End With
                With .Add("InvoiceType", SqlDbType.VarChar)
                    .Expression = "po.InvoiceType"
                End With
                With .Add("BillingAddress", SqlDbType.VarChar)
                    .Expression = "po.BillingAddress"
                End With
                With .Add("DeliveryAddress", SqlDbType.VarChar)
                    .Expression = "po.DeliveryAddress"
                End With
                With .Add("Remarks", SqlDbType.VarChar)
                    .Expression = "po.Remarks"
                End With
                With .Add("ProjectName", SqlDbType.VarChar)
                    .Expression = "orgfowst.Name"
                End With
                With .Add("SupplierName", SqlDbType.VarChar)
                    .Expression = "ISNULL(org.CompanyName, ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname)"
                End With
                With .Add("SupplierID", SqlDbType.VarChar)
                    .Expression = "ISNULL(org.ID, ppol.ID)"
                End With
                With .Add("RequestedBy", SqlDbType.VarChar)
                    .Expression = "ppol2.FirstName + ' ' + ppol2.MiddleName + ' ' + ppol2.Surname"
                End With
                With .Add("RequisitionNumber", SqlDbType.VarChar)
                    .Expression = "req.RequisitionNumber"
                End With
                With .Add("DeliveryDate", SqlDbType.DateTime)
                    .Expression = "convert(date, po.DeliveryDate)"
                End With
                'With .Add("Amount", SqlDbType.VarChar)
                '    .Expression = "(SELECT SUM(matreq2.ApprovedRequestedQty) * pimsup2.Cost " & vbCrLf &
                '                    " FROM LibraryProjectMaterialsRequisition as matreq2 " & vbCrLf &
                '                    " LEFT JOIN MobizLibraryPIMSupplier as pimsup2 " & vbCrLf &
                '                    "   ON po.SupplierID = pimsup2.SupplierID " & vbCrLf &
                '                    " WHERE matreq2.VendorID = po.SupplierID " & vbCrLf &
                '                    "   AND pimsup2.ProductID = projbud.ID " & vbCrLf &
                '                    "   AND matreq2.RequisitionID = po.RequisitionID " & vbCrLf &
                '                    " GROUP BY pimsup2.ID, pimsup2.Cost )"
                'End With
                With .Add("Amount", SqlDbType.VarChar)
                    .Expression = "(SELECT SUM(matreq.ApprovedRequestedQty * pimsup.Cost) " & vbCrLf &
                                    " FROM LibraryProjectMaterialsRequisition as matreq " & vbCrLf &
                                    " LEFT JOIN MobizLibraryPIMSupplier as pimsup " & vbCrLf &
                                    "   ON matreq.VendorID = pimsup.SupplierID AND matreq.ProductID = pimsup.ProductID " & vbCrLf &
                                    " WHERE matreq.RequisitionID = po.RequisitionID AND matreq.VendorID = po.SupplierID) " & vbCrLf
                End With
                With .Add("DateCreated", SqlDbType.DateTime)
                    .Expression = "po.DateCreated"
                End With
                With .Add("SupplierOrgAddress", SqlDbType.VarChar)
                    .Expression = "(SELECT MobizLibraryOrganization.OfficeAddressBuildingNo + ' ' + MobizLibraryOrganization.OfficeAddressStreet + ' ' + MobizLibraryCities.Name + ' ' + MobizLibraryCountries.Name " & vbCrLf &
                                    " FROM MobizLibraryOrganization " & vbCrLf &
                                    " LEFT JOIN MobizLibraryCountries ON MobizLibraryCountries.ID = MobizLibraryOrganization.OfficeAddressCountryID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryCities ON MobizLibraryCities.ID = MobizLibraryOrganization.OfficeAddressCityID " & vbCrLf &
                                    " WHERE MobizLibraryOrganization.ID = org.ID) " & vbCrLf
                End With
                With .Add("SupplierPeopleAddress", SqlDbType.VarChar)
                    .Expression = "(SELECT MobizLibraryPeopleAddress.HouseBuildingNo + ' ' + MobizLibraryPeopleAddress.Street + ' ' + MobizLibraryCities.Name + ' ' + MobizLibraryCountries.Name " & vbCrLf &
                                    " FROM MobizLibraryPeopleAddress " & vbCrLf &
                                    " LEFT JOIN MobizLibraryCountries ON MobizLibraryCountries.ID = MobizLibraryPeopleAddress.CountryID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryCities ON MobizLibraryCities.ID = MobizLibraryPeopleAddress.CityID " & vbCrLf &
                                    " WHERE MobizLibraryPeopleAddress.ID = ppol.ID)" & vbCrLf
                End With
                With .Add("BillingOrgContact", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 '+' + ctry.CallingCode + '.' + areacode.CallingCode + '.' + orgcon.ContactNo + '.' + orgcon.ExtensionNo " & vbCrLf &
                                    " FROM dbo.MobizLibraryOrganizationContactTel orgcon " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryAreaCodes areacode ON (orgcon.AreaCodeID = areacode.ID) " & vbCrLf &
                                    " WHERE orgcon.OrganizationID = po.BillingAddressID " & vbCrLf &
                                    " GROUP BY areacode.CallingCode, ctry.CallingCode, orgcon.ContactNo, orgcon.ExtensionNo )" & vbCrLf
                End With
                With .Add("BillingFowstContact", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 '+' + ctry.CallingCode + '.' + areacode.CallingCode + '.' + fowstcon.ContactNo + '.' + fowstcon.ExtensionNo " & vbCrLf &
                                    " FROM dbo.MobizLibraryOrganizationFOWSTTel fowstcon " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryCountries ctry ON (fowstcon.CountryID = ctry.ID) " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryAreaCodes areacode ON (fowstcon.AreaCodeID = areacode.ID) " & vbCrLf &
                                    " WHERE fowstcon.FOWSTID = po.BillingAddressID " & vbCrLf &
                                    " GROUP BY areacode.CallingCode, ctry.CallingCode, fowstcon.ContactNo, fowstcon.ExtensionNo )" & vbCrLf
                End With
                With .Add("DeliveryOrgContact", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 '+' + ctry.CallingCode + '.' + areacode.CallingCode + '.' + orgcon.ContactNo + '.' + orgcon.ExtensionNo " & vbCrLf &
                                    " FROM dbo.MobizLibraryOrganizationContactTel orgcon " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryCountries ctry ON (orgcon.CountryID = ctry.ID) " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryAreaCodes areacode ON (orgcon.AreaCodeID = areacode.ID) " & vbCrLf &
                                    " WHERE orgcon.OrganizationID = po.DeliveryAddressID " & vbCrLf &
                                    " GROUP BY areacode.CallingCode, ctry.CallingCode, orgcon.ContactNo, orgcon.ExtensionNo )" & vbCrLf
                End With
                With .Add("DeliveryFowstContact", SqlDbType.VarChar)
                    .Expression = "(SELECT TOP 1 '+' + ctry.CallingCode + '.' + areacode.CallingCode + '.' + fowstcon.ContactNo + '.' + fowstcon.ExtensionNo " & vbCrLf &
                                    " FROM dbo.MobizLibraryOrganizationFOWSTTel fowstcon " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryCountries ctry ON (fowstcon.CountryID = ctry.ID) " & vbCrLf &
                                    " LEFT JOIN dbo.MobizLibraryAreaCodes areacode ON (fowstcon.AreaCodeID = areacode.ID) " & vbCrLf &
                                    " WHERE fowstcon.FOWSTID = po.DeliveryAddressID " & vbCrLf &
                                    " GROUP BY areacode.CallingCode, ctry.CallingCode, fowstcon.ContactNo, fowstcon.ExtensionNo )" & vbCrLf
                End With
                'With .Add("Cost", SqlDbType.VarChar)
                '    .Expression = "(SELECT pimsup2.Cost " & vbCrLf &
                '                    " FROM MobizLibraryPIMSupplier as pimsup2 " & vbCrLf &
                '                    " WHERE po.SupplierID = pimsup2.SupplierID " & vbCrLf &
                '                    " AND pimsup2.ProductID = projbud.ID)"
                'End With
            End With

            .SqlFrom = "FROM LibraryProjectPO as po " & vbCrLf &
            " LEFT JOIN LibraryProjectRequisition as req " & vbCrLf &
            "   ON po.RequisitionID = req.ID " & vbCrLf &
            " LEFT JOIN LibraryProjectMaterialsRequisition as matreq " & vbCrLf &
            "   ON req.ID = matreq.RequisitionID " & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFowst as orgfowst " & vbCrLf &
            "   ON orgfowst.ID = po.ProjectID " & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization as org " & vbCrLf &
            "   ON org.ID = po.SupplierID " & vbCrLf &
            " LEFT JOIN MobizLibraryPeople as ppol " & vbCrLf &
            "   ON ppol.ID = po.SupplierID " & vbCrLf &
            " LEFT JOIN MobizLibraryPeople as ppol2 " & vbCrLf &
            "   ON ppol2.ID = req.RequestedBy " & vbCrLf &
            " LEFT JOIN LibraryProjectBudget as projbud " & vbCrLf &
            "   ON projbud.ID = matreq.ProductID " & vbCrLf

            If Searchable = False Then
                If FilterBy <> "all" Then
                    .SqlWhere = "WHERE orgfowst.ID = " & ReadyString(FilterBy) & vbCrLf
                End If
            ElseIf Searchable = True Then
                .SqlWhere = "WHERE po.PONumber      LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR orgfowst.Name      LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR org.CompanyName    LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR ppol.FirstName     LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR ppol.MiddleName    LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR ppol.Surname       LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR ppol2.FirstName    LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR ppol2.MiddleName   LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR ppol2.Surname      LIKE " & ReadyStringWild(SearchedText) & vbCrLf &
                            " OR DateName(month, po.DeliveryDate) LIKE " & ReadyStringWild(SearchedText) & vbCrLf
            End If

            If Common.WebRequest.Data("POID") <> Nothing Then
                .SqlWhere = "WHERE po.ID = " & ReadyString(Common.WebRequest.Data("POID")) & vbCrLf
            End If

            .SqlGroup = "GROUP BY po.ID, po.PONumber, orgfowst.Name, org.CompanyName, ppol.FirstName, " & vbCrLf &
                        " ppol.MiddleName, ppol.Surname, ppol2.FirstName, ppol2.MiddleName, ppol2.Surname, " & vbCrLf &
                        " po.DeliveryDate, po.SupplierID, org.ID, ppol.ID, projbud.ID, po.RequisitionID, " & vbCrLf &
                        " req.RequisitionNumber, po.DateCreated, po.InvoiceType, po.BillingAddress, po.DeliveryAddress, " & vbCrLf &
                        " po.Remarks, po.BillingAddressID, po.DeliveryAddressID "

            .SqlOrder = " ORDER BY po.DateCreated DESC "

            pDT = .Requery

        End With

        Return pDT
    End Function
End Class