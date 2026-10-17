Public Class LibrarySuppliers

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetDefaultAddress(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 2 LibrarySuppliersAddress.ID" & _
            ",LibrarySuppliersAddress.Street + ', ' + LibrarySuppliersAddress.District + ', ' + ISNULL(LibraryLocations.LocationName, '') AS SupplierAddress" & _
            " FROM LibrarySuppliersAddress" & _
            " LEFT JOIN LibraryLocations ON LibrarySuppliersAddress.LocationID = LibraryLocations.ID" & _
            " WHERE LibrarySuppliersAddress.SupplierID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 1 Then
            ' must return only 1 row
            Return pDT.Rows(0)
        End If

    End Function

    Public Function MostUsedAddress(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 LibrarySuppliersAddress.ID" & _
            ",LibrarySuppliersAddress.Street + ', ' + LibrarySuppliersAddress.District + ', ' + ISNULL(LibraryLocations.LocationName, '') AS SupplierAddress" & _
            " FROM LibrarySuppliersAddress" & _
            " LEFT JOIN LibraryLocations ON LibrarySuppliersAddress.LocationID = LibraryLocations.ID" & _
            " LEFT JOIN TransStockPurchaseSupplier ON TransStockPurchaseSupplier.SupplierAddressID = LibrarySuppliersAddress.ID" & _
            " WHERE LibrarySuppliersAddress.SupplierID = " & ReadyString(vID) & _
            " GROUP BY LibrarySuppliersAddress.ID" & _
            ",LibrarySuppliersAddress.Street + ', ' + LibrarySuppliersAddress.District + ', ' + ISNULL(LibraryLocations.LocationName, '')" & _
            " ORDER BY COUNT(*) DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function StockPrice(ByVal vID As String, ByVal vStockId As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibrarySupplierStockPrices.ID," & _
            "LibrarySupplierStocks.SupplierID," & _
            "LibrarySupplierStockPrices.SupplierStockID," & _
            "LibrarySupplierStockPrices.ActualQuantity," & _
            "LibrarySupplierStockPrices.Terms," & _
            "LibrarySupplierStockPrices.UnitMeasure," & _
            "LibrarySupplierStockPrices.Quantity," & _
            "LibrarySupplierStockPrices.Price," & _
            "LibrarySupplierStockPrices.UserID," & _
            "LibrarySupplierStockPrices.LastUpdated " & _
            ",tblEmp.LastName + ', ' + tblEmp.FirstName + ' ' + tblEmp.MiddleName AS UpdatedByName" &
            " FROM LibrarySupplierStockPrices" & _
            " JOIN LibrarySupplierStocks ON LibrarySupplierStockPrices.SupplierStockID = LibrarySupplierStocks.ID" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmp ON tblEmp.ID = LibrarySupplierStockPrices.UserID" &
            " WHERE LibrarySupplierStocks.StockID = " & ReadyString(vStockId) & _
            " AND LibrarySupplierStocks.SupplierID = " & ReadyString(vID) & _
            " AND LibrarySupplierStockPrices.Price > 0" & _
            " ORDER BY LibrarySupplierStockPrices.LastUpdated DESC" & _
            ",LibrarySupplierStockPrices.ActualQuantity DESC" & _
            ",LibrarySupplierStockPrices.Price"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function


    ''' <summary>
    ''' for the "address" grid of "Supplier" library
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAddresses(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibrarySuppliersAddress.ID" & _
            ",LibrarySuppliersAddress.StreetNumber" & _
            ",LibrarySuppliersAddress.StreetID" & _
            ",LibraryStreetNames.StreetName" & _
            ",LibrarySuppliersAddress.StreetID2" & _
            ",LibraryStreetNamesA.StreetName AS StreetName2" & _
            ",LibraryStreetNames.DistrictID" & _
            ",LibraryDistricts.CityID" & _
            ",LibraryCities.ProvinceID" & _
            ",LibraryProvinces.CountryID" & _
            ",ISNULL(LibrarySuppliersAddress.StreetNumber, '') + ', ' + ISNULL(LibraryStreetNames.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNamesA.StreetName, '') + ', ' + ISNULL(LibraryDistricts.Name + ', ', LibrarySuppliersAddress.District + ', ') + ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name + ', ', '') + ISNULL(LibraryCountries.Name, '') AS Address"

        pSql &= " FROM LibrarySuppliersAddress" & _
            " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibrarySuppliersAddress.StreetID " & _
            " LEFT JOIN LibraryStreetNames LibraryStreetNamesA ON LibraryStreetNamesA.ID = LibrarySuppliersAddress.StreetID2" & _
            " LEFT JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
            " LEFT JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
            " LEFT JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID" & _
            " LEFT JOIN LibraryCountries ON LibraryCountries.ID = LibraryProvinces.CountryID"
        pSql &= " WHERE CONVERT(varchar(36), LibrarySuppliersAddress.SupplierID) = " & ReadyString(vID)

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ById(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT SupplierName" &
            " FROM LibrarySuppliers" &
            " WHERE ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

#If DEBUG Then
    ''' <summary>
    ''' add random street to supplier addresses if they dont have 1
    ''' </summary>
    ''' <returns>The number of added/updated records</returns>
    ''' <remarks>for debugging purposes only</remarks>
    Public Function AddRandomStreet() As Integer

        Dim pDT, pDtStreets As DataTable
        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT LibraryStreetNames.ID" & _
            " FROM LibraryStreetNames" & _
            " JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
            " JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
            " WHERE LEN(LibraryCities.Name) > 3"
        pDtStreets = mProData.Execute(pSql)
        If pDtStreets.Rows.Count = 0 Then Return 0

        pSql = "SELECT ID" & _
            " FROM LibrarySuppliersAddress" & _
            " WHERE StreetID IS NULL"
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pSql = "UPDATE LibrarySuppliersAddress" & _
                " SET StreetID = " & ReadyString(pDtStreets.Rows(RandomInteger(0, pDtStreets.Rows.Count - 1))("ID")) & _
                " WHERE ID = " & ReadyString(pDR("ID"))
            mProData.ExecuteSQL(pSql)
        Next pDR

        Return pDT.Rows.Count

    End Function
#End If

    Public Function GetParams(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ID AS SupplierID" &
            ",LeadTime" &
            ",SupplierName" &
            ",MinimumOrderQty" &
            ",LotSize" &
            ",MinimumOrderUnit" &
            ",LotSizeUnit" &
            " FROM LibrarySuppliers" &
            " WHERE ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetStockParams(vID As String, vStockID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LeadTime" &
            ",MinimumOrderQty" &
            ",LotSize" &
            ",MinimumOrderUnit" &
            ",LotSizeUnit" &
            " FROM LibrarySupplierStocks" &
            " WHERE SupplierID = " & ReadyString(vID) &
            " AND StockID = " & ReadyString(vStockID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub GetStockParams(vID As String, vStockID As String,
        ByRef oLotSize As Integer,
        ByRef oLotSizeUnit As String,
        ByRef oLotSizeUnitQty As Integer,
        ByRef oMoq As Integer,
        ByRef oMoqUnit As String,
        ByRef oMoqUnitQty As Integer,
        ByRef oDtSupplierPrices As DataTable)

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pDtUnits As DataTable
        Dim pIndex As Integer

        oLotSize = 0
        oMoq = 0
        oDtSupplierPrices = StockPrice(vID, vStockID) ' not needed anymore in this function but needed outside
        pDtUnits = mParent.LibraryStock.GetUnits(ReadyString(vStockID))

        pDT = GetStockParams(vID, vStockID)
        If pDT.Rows.Count > 0 Then
            pDR = pDT.Rows(0)
            If pDR("LotSize") IsNot DBNull.Value _
                AndAlso pDR("LotSizeUnit") IsNot DBNull.Value Then

                oLotSize = Math.Max(1, pDR("LotSize"))
                oLotSizeUnit = pDR("LotSizeUnit")
            End If

            If pDR("MinimumOrderQty") IsNot DBNull.Value _
                AndAlso pDR("MinimumOrderUnit") IsNot DBNull.Value Then

                oMoq = Math.Max(1, pDR("MinimumOrderQty"))
                oMoqUnit = pDR("MinimumOrderUnit")
            End If
        End If

        If oLotSize > 0 Then
            pDtUnits.DefaultView.RowFilter = "UnitMeasure = " & ReadyString(oLotSizeUnit)
            If pDtUnits.DefaultView.Count > 0 Then
                oLotSizeUnitQty = pDtUnits.DefaultView(0)("ActualQuantity")
            Else
                ' issue warning message
            End If
        End If

        If oMoq > 0 Then
            pDtUnits.DefaultView.RowFilter = "UnitMeasure = " & ReadyString(oMoqUnit)
            If pDtUnits.DefaultView.Count > 0 Then
                oMoqUnitQty = pDtUnits.DefaultView(0)("ActualQuantity")
            Else
                ' issue warning message
            End If
        End If

        If oLotSize > 0 AndAlso oMoq > 0 Then Exit Sub ' exit coz there is no params for this supplier-stock

        '// supplier specific params
        pDT = GetParams(vID)
        If pDT.Rows.Count > 0 Then
            pDR = pDT.Rows(0)
            If oLotSize = 0 _
                AndAlso pDR("LotSize") IsNot DBNull.Value _
                AndAlso pDR("LotSizeUnit") IsNot DBNull.Value Then

                oLotSize = Math.Max(1, pDR("LotSize"))
                oLotSizeUnit = pDR("LotSizeUnit") ' integer
                If pDtUnits.Rows.Count > oLotSizeUnit - 1 Then
                    pIndex = oLotSizeUnit - 1
                Else
                    pIndex = pDtUnits.Rows.Count - 1
                End If
                oLotSizeUnit = pDtUnits.Rows(pIndex)("UnitMeasure") ' string
                oLotSizeUnitQty = pDtUnits.Rows(pIndex)("UnitQty")
            End If

            If oMoq = 0 _
                AndAlso pDR("MinimumOrderQty") IsNot DBNull.Value _
                AndAlso pDR("MinimumOrderUnit") IsNot DBNull.Value Then

                oMoq = Math.Max(1, pDR("MinimumOrderQty"))
                oMoqUnit = pDR("MinimumOrderUnit") ' integer
                If pDtUnits.Rows.Count > oMoqUnit - 1 Then
                    pIndex = oMoqUnit - 1
                Else
                    pIndex = pDtUnits.Rows.Count - 1
                End If
                oMoqUnit = pDtUnits.Rows(pIndex)("UnitMeasure") ' string
                oMoqUnitQty = pDtUnits.Rows(pIndex)("UnitQty")
            End If
        End If

        If oLotSize > 0 AndAlso oMoq > 0 Then Exit Sub ' exit coz there is no params for this supplier-stock

        '// "system defaults" 
        '   last resort
        oLotSize = mParent.SystemDefaults.LotSize
        oLotSizeUnit = mParent.SystemDefaults.LotSizeUnit

        If pDtUnits.Rows.Count > oLotSizeUnit - 1 Then
            pIndex = oLotSizeUnit - 1
        Else
            pIndex = pDtUnits.Rows.Count - 1
        End If
        oLotSizeUnit = pDtUnits.Rows(pIndex)("UnitMeasure") ' string
        oLotSizeUnitQty = pDtUnits.Rows(pIndex)("UnitQty")


        oMoq = mParent.SystemDefaults.MinumumOrderQuantity
        oMoqUnit = mParent.SystemDefaults.MinumumOrderUnit
        If pDtUnits.Rows.Count > oMoqUnit - 1 Then
            pIndex = oMoqUnit - 1
        Else
            pIndex = pDtUnits.Rows.Count - 1
        End If
        oMoqUnit = pDtUnits.Rows(pIndex)("UnitMeasure") ' string
        oMoqUnitQty = pDtUnits.Rows(pIndex)("UnitQty")

    End Sub

    Public Sub TransferReceivings(vOldSupplierID As String, vNewSupplierID As String)

        Dim pIU = New InsertUpdate("TransReceiving")
        pIU("SupplierID") = ReadyString(vNewSupplierID)
        Dim pSql = pIU.GetUpdate("SupplierID = " & ReadyString(vOldSupplierID))
        pSql = mProData.ExecuteSQL(pSql) ' output is used to debugging only

    End Sub

    Public Sub Deactivate(vOrgID As String, vReason As String)

        Dim pIU = New InsertUpdate("LibraryOrganizationSupplier")
        pIU("DeactivationReason") = ReadyString(vReason)
        pIU("Deactivated") = 1
        pIU("DeactivationDate") = "GETDATE()"
        Dim pSql = pIU.GetUpdate("SupplierID = " & ReadyString(vOrgID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function GetData(vOrgID As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT LibraryOrganizationSupplier.CreditBegBalance" & vbCrLf &
            " ,LibraryOrganizationSupplier.CreditBalanceAsOf" & vbCrLf &
            " ,LibraryOrganizationSupplier.CreditLimit" & vbCrLf &
            " ,LibraryPaymentTerms.Days AS CreditTerm" & vbCrLf &
            " FROM LibraryOrganizationSupplier" & vbCrLf &
            " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf &
            " WHERE LibraryOrganizationSupplier.SupplierID = " & vbCrLf & ReadyString(vOrgID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class
