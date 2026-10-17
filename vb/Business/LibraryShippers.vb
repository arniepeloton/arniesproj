Public Class LibraryShippers

    Dim mProData As ProData
    Dim mParent As Business

    Dim mDtShipperRoutes As DataTable

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetVessels(ByVal vID As String) As DataTable

        Dim pSql As String

        pSql = "SELECT LibraryShipperVessels.ID" & _
            ",LibraryShipperVessels.ShipperID" & _
            ",LibraryShipperVessels.VesselName" & _
            ",LibraryShipperVessels.DepartureDay" & _
            ",LibraryShipperVessels.DepartureHour" & _
            ",LibraryShipperVessels.LeadTime" & _
            ",LibraryShipperVessels.UserID" & _
            ",EmployeeProfiles.LastName + ', ' + EmployeeProfiles.FirstName + ' ' + EmployeeProfiles.MiddleName AS UpdatedBy" & _
            ",LibraryShipperVessels.LastUpdated " & _
            ",ShipperRouteId" & _
            " FROM LibraryShipperVessels" & _
            " JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles ON LibraryShipperVessels.UserID = EmployeeProfiles.ID " & _
            " WHERE CONVERT(varchar(36), LibraryShipperVessels.ShipperID) = " & ReadyString(vID)

        Return mProData.Execute(pSql)

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID">ShipperID. If blank, get all routes</param>
    ''' <param name="vCity1ID">if this is set, vCity2ID must also be set</param>
    ''' <param name="vCity2ID">if this is set, vCity1ID must also be set</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRoutes(Optional ByVal vID As String = "") As DataTable

        Dim pSql As String

        pSql = "SELECT LibraryShipperRoutes.ID" & _
            ",LibraryShipperRoutes.OriginPortId" & _
            ",LibraryPorts1.Name AS Origin" & _
            ",LibraryShipperRoutes.DestinationPortId" & _
            ",LibraryPorts2.Name AS Destination"
        If vID.Length = 0 Then
            pSql &= ",LibraryShipper.ShipperName" & _
                ",LibraryShipperRoutes.ShipperID"
            pSql &= ",SUM(LibraryShipperRouteDetails.Cost10Ft) AS Cost10Ft" & _
                ",SUM(LibraryShipperRouteDetails.Cost20Ft) AS Cost20Ft" & _
                ",SUM(LibraryShipperRouteDetails.Cost10Ft) AS Cost10Ft" & _
                ",SUM(LibraryShipperRouteDetails.Cost40Ft) AS Cost40Ft" & _
                ",SUM(LibraryShipperRouteDetails.CostLcl) AS CostLcl"
            pSql &= ",LibraryPortAddresses1.CityId AS OriginCityID" & _
                ",LibraryPortAddresses2.CityId AS DestinationCityID"
            pSql &= ",LibraryCities1.Name AS OriginCity" & _
                ",LibraryCities2.Name AS DestinationCity"
        End If
        pSql &= " FROM LibraryShipperRoutes"
        pSql &= " LEFT JOIN LibraryPorts LibraryPorts1 ON LibraryPorts1.ID = LibraryShipperRoutes.OriginPortId"
        pSql &= " LEFT JOIN LibraryPorts LibraryPorts2 ON LibraryPorts2.ID = LibraryShipperRoutes.DestinationPortId"

        'If vPort1ID IsNot Nothing Then
        '    pSql &= " WHERE (LibraryShipperRoutes.OriginalPortId = " & ReadyString(vPort1ID) & _
        '        " AND LibraryShipperRoutes.DestinationPortId = " & ReadyString(vPort2ID) & _
        '        ") OR (LibraryShipperRoutes.OriginalPortId = " & ReadyString(vPort2ID) & _
        '        " AND LibraryShipperRoutes.DestinationPortId = " & ReadyString(vPort1ID) & _
        '        ")"
        'End If

        If vID.Length = 0 Then
            pSql &= " JOIN LibraryShipper ON LibraryShipper.ID = LibraryShipperRoutes.ShipperID"
            pSql &= " LEFT JOIN LibraryShipperRouteDetails ON LibraryShipperRouteDetails.ShipperRouteId = LibraryShipperRoutes.ID"

            pSql &= " LEFT JOIN LibraryPortAddresses LibraryPortAddresses1 ON LibraryPortAddresses1.PortID = LibraryShipperRoutes.OriginPortID" & _
                " AND LibraryPorts1.IsActive = 1"
            pSql &= " LEFT JOIN LibraryCities LibraryCities1 ON LibraryCities1.ID = LibraryPortAddresses1.CityId"

            pSql &= " LEFT JOIN LibraryPortAddresses LibraryPortAddresses2 ON LibraryPortAddresses2.PortID = LibraryShipperRoutes.DestinationPortID" & _
                " AND LibraryPorts2.IsActive = 1"
            pSql &= " LEFT JOIN LibraryCities LibraryCities2 ON LibraryCities2.ID = LibraryPortAddresses2.CityId"

            pSql &= " WHERE LibraryShipper.IsActive = 1"
            pSql &= " GROUP BY LibraryShipperRoutes.ID" & _
                ",LibraryShipperRoutes.OriginPortId" & _
                ",LibraryPorts1.Name" & _
                ",LibraryShipperRoutes.DestinationPortId" & _
                ",LibraryPorts2.Name" & _
                ",LibraryShipper.ShipperName" & _
                ",LibraryShipperRoutes.ShipperID" & _
                ",LibraryPortAddresses1.CityId" & _
                ",LibraryPortAddresses2.CityId" & _
                ",LibraryCities1.Name" & _
                ",LibraryCities2.Name"
        Else
            pSql &= " WHERE CONVERT(varchar(36), LibraryShipperRoutes.ShipperId) = " & ReadyString(vID)
        End If

        Return mProData.Execute(pSql)

    End Function

    Public Function CacheRoutes() As DataTable

        mDtShipperRoutes = GetRoutes()
        Return mDtShipperRoutes

    End Function

    Public Function GetRouteDetails(ByVal vShipperId As String, Optional ByVal vShipperRouteID As String = Nothing) As DataTable

        Dim pSql As String

        pSql = "SELECT LibraryShipperRouteDetails.ID" & _
            ",LibraryShipperRouteDetails.ShipperRouteId" & _
            ",LibraryShipperRouteDetails.GroupName" & _
            ",LibraryShipperRouteDetails.FieldName" & _
            ",LibraryShipperRouteDetails.Cost10Ft" & _
            ",LibraryShipperRouteDetails.Cost20Ft" & _
            ",LibraryShipperRouteDetails.Cost40Ft" & _
            ",LibraryShipperRouteDetails.CostLcl" & _
            " FROM LibraryShipperRouteDetails" & _
            " JOIN LibraryShipperRoutes ON LibraryShipperRoutes.ID = LibraryShipperRouteDetails.ShipperRouteId" & _
            " WHERE CONVERT(varchar(36), LibraryShipperRoutes.ShipperId) = " & ReadyString(vShipperId)
        If CStr2(vShipperRouteID).Length > 0 Then
            pSql &= " AND LibraryShipperRoutes.ID = " & ReadyString(vShipperRouteID)
        End If
        Return mProData.Execute(pSql)

    End Function

    Public Function GetCustomerSupplierRoutes(ByVal vCustomerID As String, ByVal vSupplierID As String) As DataTable

        Dim pSql, pSql1, pSql2 As String
        Dim pDT As DataTable
        Dim pPortMode1, pPortMode2 As String

        pPortMode1 = "DestinationPortId"
        pPortMode2 = "OriginPortId"

        pSql1 = GetCustomerSupplierRoutesSql(vCustomerID, vSupplierID, "DestinationPortId", "OriginPortId")
        pSql2 = GetCustomerSupplierRoutesSql(vCustomerID, vSupplierID, "OriginPortId", "DestinationPortId")
        pSql = pSql1 & " UNION " & pSql2

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetCustomerSupplierRoutesSql(ByVal vCustomerID As String, ByVal vSupplierID As String, _
        ByVal vPortMode1 As String, ByVal vPortMode2 As String) As String

        Dim pSql As String

        pSql = "SELECT LibraryShipper.ID AS ShipperID" & _
            ",LibraryCustomersAddresses.ID AS CustomerAddressID" & _
            ",LibrarySuppliersAddress.ID AS SupplierAddressID" & _
            " ,LibraryDistricts.CityID" & _
            " ,ISNULL(LibraryCustomersAddresses.StreetNumber, '') + ', ' + ISNULL(LibraryStreetNames.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNamesA.StreetName, '') + ', ' + ISNULL(LibraryDistricts.Name + ', ', LibraryCustomersAddresses.District + ', ') + ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name + ', ', '') + ISNULL(LibraryCountries.Name, '') AS CustomerAddress" & _
            " ,LibraryShipper.ShipperName" & _
            " ,LibraryPorts.Name AS CustomerPort" & _
            " ,LibraryPorts1.Name AS SupplierPort" & _
            " ,LibrarySuppliers.SupplierName" & _
            " ,ISNULL(LibrarySuppliersAddress.StreetNumber + ', ', '') + ISNULL(LibraryStreetNames1.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNames1A.StreetName, '') + ', ' + ISNULL(LibraryDistricts1.Name + ', ', '') + ISNULL(LibraryCities1.Name + ', ', '') + ISNULL(LibraryProvinces1.Name + ', ', '') + ISNULL(LibraryCountries1.Name, '') AS SupplierAddress" & _
            ",LibraryShipperRoutes.ID AS ShipperRouteID" & _
            " " & _
            " ,SUM(LibraryShipperRouteDetails.Cost10Ft) AS Cost10Ft" & _
            " ,SUM(LibraryShipperRouteDetails.Cost20Ft) AS Cost20Ft" & _
            " ,SUM(LibraryShipperRouteDetails.Cost10Ft) AS Cost10Ft" & _
            " ,SUM(LibraryShipperRouteDetails.Cost40Ft) AS Cost40Ft" & _
            " ,SUM(LibraryShipperRouteDetails.CostLcl) AS CostLcl" & _
            " ," & ReadyString(vPortMode1) & " AS PortMode" & _
            " FROM LibraryCustomersAddresses" & _
            " JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID" & _
            " LEFT JOIN LibraryStreetNames LibraryStreetNamesA ON LibraryStreetNamesA.ID = LibraryCustomersAddresses.StreetID2" & _
            " JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
            " JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
            " JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID" & _
            " JOIN LibraryCountries ON LibraryCountries.ID = LibraryProvinces.CountryID" & _
            " " & _
            " JOIN LibraryPortAddresses ON LibraryPortAddresses.CityId = LibraryCities.ID" & _
            " JOIN LibraryPorts ON LibraryPorts.ID = LibraryPortAddresses.PortId" & _
            " AND LibraryPorts.IsActive = 1" & _
            " " & _
            " JOIN LibraryShipperRoutes ON LibraryShipperRoutes." & vPortMode1 & "= LibraryPorts.ID" & _
            " JOIN LibraryShipper ON LibraryShipper.ID = LibraryShipperRoutes.ShipperID" & _
            " AND LibraryShipper.IsActive = 1" & _
            " LEFT JOIN LibraryShipperRouteDetails ON LibraryShipperRouteDetails.ShipperRouteId = LibraryShipperRoutes.ID" & _
            " " & _
            " JOIN LibraryPorts LibraryPorts1 ON LibraryPorts1.ID = LibraryShipperRoutes." & vPortMode2 & _
            " AND LibraryPorts1.IsActive = 1" & _
            " JOIN LibraryPortAddresses LibraryPortAddresses1 ON LibraryPortAddresses1.PortId = LibraryPorts1.ID" & _
            " " & _
            " JOIN LibraryCities LibraryCities1 ON LibraryCities1.ID = LibraryPortAddresses1.CityId" & _
            " JOIN LibraryDistricts LibraryDistricts1 ON LibraryDistricts1.CityID = LibraryCities1.ID" & _
            " JOIN LibraryStreetNames LibraryStreetNames1 ON LibraryStreetNames1.DistrictID = LibraryDistricts1.ID" & _
            " LEFT JOIN LibraryProvinces LibraryProvinces1 ON LibraryProvinces1.ID = LibraryCities1.ProvinceID" & _
            " LEFT JOIN LibraryCountries LibraryCountries1 ON LibraryCountries1.ID = LibraryProvinces1.CountryID" & _
            " " & _
            " JOIN LibrarySuppliersAddress ON LibrarySuppliersAddress.StreetID = LibraryStreetNames1.ID" & _
            " LEFT JOIN LibraryStreetNames LibraryStreetNames1A ON LibraryStreetNames1A.ID = LibrarySuppliersAddress.StreetID2" & _
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySuppliersAddress.SupplierID" & _
            " " & _
            " WHERE LibraryCustomersAddresses.IsDeliveryAddress = 1" & _
            " AND LibraryCustomersAddresses.CustomerID = " & ReadyString(vCustomerID) & _
            " AND LibrarySuppliers.ID = " & ReadyString(vSupplierID) & _
            " " & _
            " GROUP BY LibraryCustomersAddresses.ID" & _
            ",LibraryDistricts.CityID" & _
            ",ISNULL(LibraryCustomersAddresses.StreetNumber, '') + ', ' + ISNULL(LibraryStreetNames.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNamesA.StreetName, '') + ', ' + ISNULL(LibraryDistricts.Name + ', ', LibraryCustomersAddresses.District + ', ') + ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name + ', ', '') + ISNULL(LibraryCountries.Name, '') " & _
            ",LibraryShipper.ShipperName" & _
            ",LibraryPorts.Name " & _
            ",LibraryPorts1.Name " & _
            ",LibrarySuppliers.SupplierName" & _
            ",ISNULL(LibrarySuppliersAddress.StreetNumber + ', ', '') + ISNULL(LibraryStreetNames1.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNames1A.StreetName, '') + ', ' + ISNULL(LibraryDistricts1.Name + ', ', '') + ISNULL(LibraryCities1.Name + ', ', '') + ISNULL(LibraryProvinces1.Name + ', ', '') + ISNULL(LibraryCountries1.Name, '') " & _
            ",LibraryShipper.ID" & _
            ",LibraryShipperRoutes.ID" & _
            ",LibrarySuppliersAddress.ID"

        Return pSql

    End Function

    Public Function GetShipper(ByVal vCustomerID As String, ByVal vSupplierID As String) As String

        Dim pDT As DataTable

        pDT = GetCustomerSupplierRoutes(vCustomerID, vSupplierID)
        If pDT Is Nothing _
            OrElse pDT.Rows.Count = 0 Then

            Return mParent.SystemDefaults.GetOrAddSystemDefault("Default Shipper", "39721E43-D898-4CB0-954B-061DF72CB96B")
            Return Nothing
        Else
            pDT.DefaultView.Sort = "PortMode, Cost10Ft"
            Return CStr2(pDT.DefaultView(0)("ShipperID"))
        End If

    End Function

    Public Function GetDefaultShipper() As DataRow

        Dim pDT As DataTable
        Dim pSql As String
        Dim pId As String

        pId = mParent.SystemDefaults.GetOrAddSystemDefault("Default Shipper", "39721E43-D898-4CB0-954B-061DF72CB96B")
        If IsGuid(pId) Then
            pSql = "SELECT ID" & _
                ",ShipperName" & _
                " FROM LibraryShipper" & _
                " WHERE ID = " & ReadyString(pId)
            pDT = mProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                Return pDT.Rows(0)
            End If
        End If

    End Function
End Class
