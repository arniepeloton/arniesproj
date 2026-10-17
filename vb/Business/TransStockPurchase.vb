Public Class TransStockPurchase

    Dim mProData As ProData
    Dim mParent As Business

    Dim mCustomerID As String
    Dim mDrMostCommonBillingAddress As DataRow
    Dim mDrMostCommonDeliveryAddress As DataRow

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public ReadOnly Property CustomerID() As String
        Get

            Return mCustomerID

        End Get
    End Property

    Public Function GetUnapproved() As DataTable

        Dim pSql As String

        pSql = "SELECT TransStockPurchaseItemsUnapproved.ID" & _
            ",TransStockPurchaseItemsUnapproved.StockID" & _
            ",TransStockPurchaseItemsUnapproved.Quantity" & _
            ",TransStockPurchaseItemsUnapproved.UnitQuantity" & _
            ",LibrarySuppliers.SupplierName" & _
            " FROM TransStockPurchaseItemsUnapproved" & _
            " LEFT JOIN LibrarySuppliers" & _
            " ON LibrarySuppliers.ID = TransStockPurchaseItemsUnapproved.SupplierID"
        Return mProData.Execute(pSql)

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vBranchID"></param>
    ''' <param name="vDateNeeded">vSupplierID must also be set if this is set</param>
    ''' <param name="vSupplierID">Time component will be removed internally. vDateNeeded must also be set if this is set.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPendingProposal(ByVal vBranchID As Integer, _
        ByVal vDateNeeded As Date, _
        ByVal vSupplierID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransProposedPurchaseOrders.ID" & _
            ",TransProposedPurchaseOrders.Num"
        pSql &= ",TransProposedPurchaseOrders.SupplierAddressID" & _
            ",TransProposedPurchaseOrders.ShipperID" & _
            ",TransProposedPurchaseOrders.DeliverAddressID" & _
            ",TransProposedPurchaseOrders.BillingAddressID" & _
            ",TransProposedPurchaseOrders.CancellationDate" & _
            ",TransProposedPurchaseOrders.Remarks"
        pSql &= " FROM TransProposedPurchaseOrders"
        pSql &= " WHERE TransProposedPurchaseOrders.PoId IS NULL" &
            " AND TransProposedPurchaseOrders.IsCancelled = 0" &
            " AND BranchID = " & vBranchID
        pSql &= " AND TransProposedPurchaseOrders.SupplierID = " & ReadyString(vSupplierID) & _
            " AND CONVERT(datetime, CONVERT(float, TransProposedPurchaseOrders.DateNeeded)) = " & ReadyString(vDateNeeded)

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Private Function AddPendingProposal(ByVal vBranchID As Integer) As DataRow

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pId As String

        pIU = New InsertUpdate("TransProposedPurchaseOrders")
        pIU("ID") = "'newid()'"
        pIU("Num") = mProData.MaxNum("TransProposedPurchaseOrders", vBranchID) + 1
        pIU("IsCancelled") = 0
        pIU("BranchID") = vBranchID
        pSql = pIU.GetInsert
        pId = mProData.InsertRow(pSql)

        pSql = "SELECT ID" & _
            ",Num" & _
            " FROM TransProposedPurchaseOrders" & _
            " WHERE ID = " & ReadyString(pId)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        Else
            Stop2() ' this is impossible
        End If

    End Function

#Region " generate proposed purchase order "
    Public Function CacheInfo(ByRef oError As String) As Boolean

        Dim pDR As DataRow

        If mCustomerID Is Nothing Then
            pDR = mParent.LibraryBranches.CurrentBranch
            If pDR Is Nothing Then
                oError = "Current branch could not be determined. Please set the current branch first"
                Return False
            Else
                If pDR("CustomerID") Is DBNull.Value Then
                    oError = "Current branch has no current account. Please set the current branch's customer account first. Please go to Library -> Branches -> Edit the current branch -> assign customer"
                    Return False
                Else
                    mCustomerID = CStr2(pDR("CustomerID"))
                End If
            End If
        End If

        If mDrMostCommonBillingAddress Is Nothing Then
            mDrMostCommonBillingAddress = MostCommonDestinationAddressID(eDestinationMode.Billing)
            If mDrMostCommonBillingAddress Is Nothing Then
                oError = "Unable to obtain the default delivery address"
                Return False
            End If
        End If
        If mDrMostCommonDeliveryAddress Is Nothing Then
            mDrMostCommonDeliveryAddress = MostCommonDestinationAddressID(eDestinationMode.Delivery)
            If mDrMostCommonDeliveryAddress Is Nothing Then
                oError = "Unable to obtain the default billing address"
                Return False
            End If
        End If

        Return True

    End Function

    Public Enum eDestinationMode
        Delivery
        Billing
    End Enum

    Private Function MostCommonDestinationAddressID(ByVal vDestinationMode As eDestinationMode) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        Select Case vDestinationMode
            Case eDestinationMode.Delivery
                pSql = "DeliverAddressID"

            Case eDestinationMode.Billing
                pSql = "BillingAddressID"

        End Select

        pSql = "SELECT TOP 1 LibraryCustomersAddresses.ID" & _
            ",ISNULL(LibraryCustomersAddresses.StreetNumber, '') + ', ' + ISNULL(LibraryStreetNames.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNamesA.StreetName, '') + ', ' + ISNULL(LibraryDistricts.Name + ', ', LibraryCustomersAddresses.District + ', ') + ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name + ', ', '') + ISNULL(LibraryCountries.Name, '') AS Address" & _
            " FROM TransStockPurchaseSupplier" & _
            " LEFT JOIN LibraryCustomersAddresses ON LibraryCustomersAddresses.ID = TransStockPurchaseSupplier." & pSql & _
            " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID " & _
            " LEFT JOIN LibraryStreetNames LibraryStreetNamesA ON LibraryStreetNamesA.ID = LibraryCustomersAddresses.StreetID2" & _
            " LEFT JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
            " LEFT JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
            " LEFT JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID" & _
            " LEFT JOIN LibraryCountries ON LibraryCountries.ID = LibraryProvinces.CountryID" & _
            " GROUP BY LibraryCustomersAddresses.ID" & _
            ",ISNULL(LibraryCustomersAddresses.StreetNumber, '') + ', ' + ISNULL(LibraryStreetNames.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNamesA.StreetName, '') + ', ' + ISNULL(LibraryDistricts.Name + ', ', LibraryCustomersAddresses.District + ', ') + ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name + ', ', '') + ISNULL(LibraryCountries.Name, '')" & _
            " ORDER BY COUNT(*) DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public ReadOnly Property MostCommonBillingAddress() As DataRow
        Get

            Return mDrMostCommonBillingAddress

        End Get
    End Property

    Public ReadOnly Property MostCommonDeliveryAddress() As DataRow
        Get

            Return mDrMostCommonDeliveryAddress

        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vSoDateFrom"></param>
    ''' <param name="vSoDateTo"></param>
    ''' <param name="vMultiplier">PO qty will be SO qty multiplied by this</param>
    ''' <param name="vBillingAddressId"></param>
    ''' <param name="vDeliveryAddressId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function GenerateProposedPurchaseOrders( _
    '    ByVal vSoDateFrom As Date, _
    '    ByVal vSoDateTo As Date, _
    '    ByVal vMultiplier As Single, _
    '    ByVal vBillingAddressId As String, _
    '    ByVal vDeliveryAddressId As String, _
    '    ByVal vDateNeeded As Date) As Integer

    '    Dim pDtStocks As DataTable
    '    Dim pDtSuppliers, pDtSuppliersAll As DataTable
    '    Dim pDrSupplier As DataRow
    '    Dim pBranchId As Integer
    '    Dim pMaxLeadTime As Integer

    '    pMaxLeadTime = (vDateNeeded - mProData.GetDate.Date).TotalDays
    '    pDtStocks = mParent.TransSalesOrder.StockSummary(vSoDateFrom, vSoDateTo, pMaxLeadTime)
    '    If pDtStocks.Rows.Count = 0 Then Return 0

    '    pDtSuppliersAll = New DataTable
    '    pDtSuppliersAll.Columns.Add("SupplierID", GetType(Guid))
    '    pDtSuppliersAll.Columns.Add("SupplierAddressID")
    '    pDtSuppliersAll.Columns.Add("ShipmentMode")

    '    pBranchId = mParent.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 0)
    '    mParent.LibraryShippers.CacheRoutes()

    '    pDtSuppliers = pDtStocks.DefaultView.ToTable(True, "SupplierID", "ShipmentMode")    ' get distinct suppliers from stocks
    '    pDtSuppliers.DefaultView.RowFilter = "SupplierID IS NOT NULL"
    '    pDtSuppliers = pDtSuppliers.DefaultView.ToTable
    '    For Each pDrSupplier In pDtSuppliers.Rows
    '        pDtSuppliersAll.DefaultView.RowFilter = "SupplierID = " & ReadyString(pDrSupplier("SupplierID"))
    '        If pDtSuppliersAll.DefaultView.Count = 0 Then
    '            pDtSuppliersAll.ImportRow(pDrSupplier)
    '            FillSupplierInfo(pDtSuppliersAll.Rows(pDtSuppliersAll.Rows.Count - 1))
    '        End If

    '        GenerateProposedPurchaseOrder(pDtSuppliersAll.DefaultView(0).Row, _
    '            New DataView(pDtStocks, "SupplierID = " & ReadyString(pDrSupplier("SupplierID")), "", DataViewRowState.CurrentRows), _
    '            pBranchId, _
    '            vBillingAddressId, _
    '            vDeliveryAddressId, _
    '            vDateNeeded, _
    '            vMultiplier)
    '    Next pDrSupplier

    'End Function

    'Public Function GenerateProposedPurchaseOrders(ByVal vDt As DataTable,
    '    ByVal vBillingAddressId As String, _
    '    ByVal vDeliveryAddressId As String, _
    '    ByVal vDateNeeded As Date,
    '    Optional vRemarks As String = Nothing,
    '    Optional ByRef oError As String = Nothing) As Integer

    '    Dim pDtSuppliers, pDtSuppliersAll As DataTable
    '    Dim pDrSupplier, pDrSupplierAll As DataRow
    '    Dim pBranchId As Integer
    '    Dim pMaxLeadTime As Integer

    '    pMaxLeadTime = (vDateNeeded - mProData.GetDate.Date).TotalDays
    '    If vDt.Rows.Count = 0 Then Return 0
    '    'vDt.Columns("BestPriceSupplierId").ColumnName = "SupplierID"

    '    pDtSuppliersAll = New DataTable
    '    pDtSuppliersAll.Columns.Add("SupplierID", GetType(Guid))
    '    pDtSuppliersAll.Columns.Add("SupplierAddressID")
    '    pDtSuppliersAll.Columns.Add("ShipmentMode")

    '    pBranchId = mParent.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 0)
    '    mParent.LibraryShippers.CacheRoutes()

    '    pDtSuppliers = vDt.DefaultView.ToTable(True, "SupplierID", "ShipmentMode", "SupplierName")    ' get distinct suppliers from stocks
    '    pDtSuppliers.DefaultView.RowFilter = "SupplierID IS NOT NULL"
    '    pDtSuppliers = pDtSuppliers.DefaultView.ToTable
    '    For Each pDrSupplier In pDtSuppliers.Rows
    '        pDtSuppliersAll.DefaultView.RowFilter = "SupplierID = " & ReadyString(pDrSupplier("SupplierID"))
    '        If pDtSuppliersAll.DefaultView.Count = 0 Then
    '            pDtSuppliersAll.ImportRow(pDrSupplier)
    '            pDrSupplierAll = pDtSuppliersAll.Rows(pDtSuppliersAll.Rows.Count - 1)
    '            FillSupplierInfo(pDrSupplierAll)
    '            If pDrSupplierAll("SupplierAddressID") Is DBNull.Value Then
    '                oError = pDrSupplier("SupplierName") & " has no address"
    '                Return 0
    '            End If
    '        End If

    '        GenerateProposedPurchaseOrder(pDtSuppliersAll.DefaultView(0).Row, _
    '            New DataView(vDt, "SupplierID = " & ReadyString(pDrSupplier("SupplierID")), "", DataViewRowState.CurrentRows), _
    '            pBranchId, _
    '            vBillingAddressId, _
    '            vDeliveryAddressId, _
    '            vDateNeeded, _
    '            1,
    '            vRemarks)
    '    Next pDrSupplier

    '    Return pDtSuppliers.Rows.Count

    'End Function

    'Private Function GenerateProposedPurchaseOrder(ByVal vDrSupplier As DataRow, ByVal vDvStocks As DataView, _
    '    ByVal vBranchID As Integer, _
    '    ByVal vBillingAddressId As String, _
    '    ByVal vDeliveryAddressId As String, _
    '    ByVal vDateNeeded As Date, _
    '    ByVal vMultiplier As Single,
    '    Optional vRemarks As String = Nothing) As Integer

    '    Dim pSql As String
    '    Dim pIU As InsertUpdate
    '    Dim pDR As DataRow
    '    Dim pDRV As DataRowView
    '    Dim pDtPrices As DataTable
    '    Dim pDrPrice As DataRow
    '    Dim pQtyToPo As Integer ' in base quantity
    '    Dim p As Integer
    '    Dim pID As String  ' TransProposedPurchaseOrders.ID
    '    Dim pShipperID As String

    '    Try
    '        vDateNeeded = vDateNeeded.Date
    '        mProData.BeginTransaction()
    '        pDR = GetPendingProposal(vBranchID, vDateNeeded, CStr2(vDrSupplier("SupplierID")))
    '        pIU = New InsertUpdate("TransProposedPurchaseOrders")
    '        pIU("BranchID") = vBranchID
    '        pIU("SupplierID") = ReadyString(vDrSupplier("SupplierID"))
    '        pIU("LastUpdated") = "GETDATE()"
    '        pIU("UserID") = mParent.MainSecurity.UserAccess.EmployeeID
    '        pIU("SupplierAddressID") = ReadyString(vDrSupplier("SupplierAddressID"))
    '        pShipperID = mParent.LibraryShippers.GetShipper(mCustomerID, CStr2(vDrSupplier("SupplierID")))
    '        If IsGuid(pShipperID) Then pIU("ShipperID") = ReadyString(pShipperID)
    '        If CStr2(vDrSupplier("ShipmentMode")).Length = 0 Then
    '            pIU("ShipmentMode") = ReadyString(mParent.SystemDefaults.GetShipmentMode)
    '        Else
    '            pIU("ShipmentMode") = ReadyString(vDrSupplier("ShipmentMode"))
    '        End If
    '        pIU("DeliverAddressID") = ReadyString(vDeliveryAddressId)
    '        pIU("BillingAddressID") = ReadyString(vBillingAddressId)
    '        pIU("DateNeeded") = ReadyString(vDateNeeded)
    '        pIU("CancellationDate") = ReadyString(vDateNeeded.AddDays(21))
    '        If vRemarks Is Nothing Then
    '            pIU("Remarks") = "'auto generated'"
    '        Else
    '            pIU("Remarks") = ReadyString(Truncate(vRemarks, 500))
    '        End If
    '        If pDR Is Nothing Then
    '            pID = Guid.NewGuid.ToString
    '            pIU("ID") = ReadyString(pID)
    '            pIU("Num") = mProData.MaxNum("TransProposedPurchaseOrders") + 1
    '            pSql = pIU.GetInsert
    '        Else
    '            pID = CStr2(pDR("ID"))
    '            pSql = pIU.GetUpdate("ID = " & ReadyString(pID))

    '            '// delete all items of this supplier from this proposed PO
    '            pSql = "DELETE FROM TransProposedPurchaseOrderItems" & _
    '                " WHERE ProposedPoId = " & ReadyString(pID)
    '            mProData.ExecuteSQL(pSql)
    '        End If
    '        mProData.ExecuteSQL(pSql)

    '        '// save stocks
    '        pIU = New InsertUpdate("TransProposedPurchaseOrderItems")
    '        pIU("ID") = "NEWID()"
    '        pIU("ProposedPoId") = ReadyString(pID)

    '        For Each pDRV In vDvStocks
    '            '// delete this item from proposed PO that have the same DateNeeded as this PO's
    '            'pSql = "SELECT TransProposedPurchaseOrderItems.ID" & _
    '            '    " FROM TransProposedPurchaseOrderSuppliers" & _
    '            '    " JOIN TransProposedPurchaseOrderItems ON TransProposedPurchaseOrderItems.ProposedPoId = TransProposedPurchaseOrderSuppliers.ProposedPoId" & _
    '            '    " WHERE CONVERT(datetime, FLOOR(CONVERT(float, TransProposedPurchaseOrderSuppliers.DateNeeded))) = " & ReadyString(vDateNeeded) & _
    '            '    " AND TransProposedPurchaseOrderItems.StockID = " & ReadyString(pDRV("StockID"))
    '            'pSql = "DELETE FROM TransProposedPurchaseOrderItems" & _
    '            '    " WHERE ID IN (" & pSql & ")"
    '            'mProData.ExecuteSQL(pSql)

    '            pQtyToPo = pDRV("Qty") * vMultiplier
    '            pIU("StockID") = ReadyString(pDRV("StockID"))

    '            pDtPrices = mParent.LibrarySuppliers.StockPrice(CStr2(vDrSupplier("SupplierID")), CStr2(pDRV("StockID")))
    '            For p = 0 To pDtPrices.Rows.Count - 1
    '                pDrPrice = pDtPrices.Rows(p)
    '                pIU("UnitMeasure") = ReadyString(pDrPrice("UnitMeasure"))
    '                pIU("UnitQuantity") = pDrPrice("ActualQuantity")
    '                pIU("UnitPrice") = pDrPrice("Price")

    '                If p = pDtPrices.Rows.Count - 1 Then
    '                    pIU("Quantity") = Math.Ceiling(pQtyToPo / pDrPrice("ActualQuantity"))
    '                Else
    '                    pIU("Quantity") = Math.Floor(pQtyToPo / pDrPrice("ActualQuantity"))
    '                End If
    '                If CDec(pIU("Quantity")) = 0 Then Continue For ' dont include zero quantities
    '                pSql = pIU.GetInsert
    '                mProData.ExecuteSQL(pSql)
    '                pQtyToPo -= pIU("Quantity") * pDrPrice("ActualQuantity")

    '                If pQtyToPo <= 0 Then Exit For
    '            Next p
    '        Next pDRV

    '        mProData.CommitTransaction()
    '    Catch ex As Exception
    '        Stop2()
    '        mProData.RollbackTransaction()
    '        Throw
    '    End Try

    'End Function

    Private Sub FillSupplierInfo(ByRef rDR As DataRow)

        Dim pDR As DataRow

        pDR = mParent.LibrarySuppliers.MostUsedAddress(CStr2(rDR("SupplierID")))
        If pDR IsNot Nothing Then
            rDR("SupplierAddressID") = CStr2(pDR("ID"))
        End If

    End Sub

    ''' <summary>
    ''' add/update TransProposedPurchaseOrderSuppliers
    ''' </summary>
    ''' <param name="rDR">a row from TransProposedPurchaseOrderSuppliers</param>
    ''' <remarks></remarks>
    Public Function FillProposedInfo(ByRef rDR As DataRow) As Boolean

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pIsExist As Boolean
        Dim pDR2 As DataRow

        If rDR("ProposedPoId") Is DBNull.Value Then Return False
        If rDR("OrderSupplierID") Is DBNull.Value Then
            ' double-check for existence of TransProposedPurchaseOrderSuppliers row
            pSql = "SELECT ID" & _
                ",ShipperID" & _
                ",SupplierAddressID" & _
                ",DeliverAddressID" & _
                ",BillingAddressID" & _
                " FROM TransProposedPurchaseOrderSuppliers" & _
                " WHERE ProposedPoId = " & ReadyString(rDR("ProposedPoId")) & _
                " AND SupplierID = " & ReadyString(rDR("SupplierID"))
            pDT = mProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                pIsExist = True
                pDR = pDT.Rows(0)
            Else
                pDR = rDR
            End If
        Else
            pIsExist = True
            pDR = rDR
        End If

        pIU = New InsertUpdate("TransProposedPurchaseOrderSuppliers")
        If pDR("ShipperID") Is DBNull.Value Then
            pIU("ShipperID") = ReadyString(mParent.LibraryShippers.GetShipper(mCustomerID, CStr2(rDR("SupplierID"))))
        End If
        If pDR("SupplierAddressID") Is DBNull.Value Then
            pDR2 = mParent.LibrarySuppliers.MostUsedAddress(CStr2(pDR("SupplierID")))
            If pDR2 IsNot Nothing Then
                pIU("SupplierAddressID") = ReadyString(pDR2("ID"))
            End If
        End If
        If pDR("DeliverAddressID") Is DBNull.Value Then
            pIU("DeliverAddressID") = ReadyString(mDrMostCommonDeliveryAddress("ID"))
        Else
            pIU("DeliverAddressID") = ReadyString(pDR("DeliverAddressID"))
        End If
        If pDR("BillingAddressID") Is DBNull.Value Then
            pIU("BillingAddressID") = ReadyString(mDrMostCommonBillingAddress("ID"))
        Else
            pIU("BillingAddressID") = ReadyString(pDR("BillingAddressID"))
        End If

        If pIU.Count > 0 Then
            If pIsExist Then
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("OrderSupplierID")))
            Else
                pIU("ID") = ReadyString(Guid.NewGuid)
                pIU("ProposedPoId") = ReadyString(pDR("ProposedPoId"))
                pIU("SupplierID") = ReadyString(pDR("SupplierID"))
                pSql = pIU.GetInsert
            End If
            mProData.ExecuteSQL(pSql)
            Return True
        End If

    End Function
#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vStockID"></param>
    ''' <param name="vProposedPoId">If blank, returns data from all unserved proposed PO</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProposed(ByVal vStockID As String, Optional ByVal vProposedPoId As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransProposedPurchaseOrderItems.ID" & _
            ",TransProposedPurchaseOrderItems.SupplierID" & _
            ",LibrarySuppliers.SupplierName" & _
            ",TransProposedPurchaseOrderItems.Quantity" & _
            ",TransProposedPurchaseOrderItems.UnitMeasure" & _
            ",TransProposedPurchaseOrderItems.UnitQuantity" & _
            ",TransProposedPurchaseOrderItems.LastUpdated" & _
            ",TransProposedPurchaseOrderItems.UnitPrice" & _
            ",TransProposedPurchaseOrderItems.ProposedPoId" & _
            ",TransProposedPurchaseOrderSuppliers.DateNeeded" & _
            ",TransProposedPurchaseOrderSuppliers.CancellationDate" & _
            ",TransProposedPurchaseOrders.Num" & _
            " FROM TransProposedPurchaseOrderItems" & _
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = TransProposedPurchaseOrderItems.SupplierID" & _
            " JOIN TransProposedPurchaseOrders ON TransProposedPurchaseOrders.ID = TransProposedPurchaseOrderItems.ProposedPoId" & _
            " LEFT JOIN TransProposedPurchaseOrderSuppliers ON TransProposedPurchaseOrderSuppliers.ProposedPoId = TransProposedPurchaseOrderItems.ProposedPoId" & _
            " AND TransProposedPurchaseOrderSuppliers.SupplierID = TransProposedPurchaseOrderItems.SupplierID" & _
            " WHERE TransProposedPurchaseOrderItems.StockID = " & ReadyString(vStockID) & _
            " AND TransProposedPurchaseOrderItems.PoId IS NULL"
        If CStr2(vProposedPoId).Length > 0 Then
            pSql &= " AND ProposedPoId = " & ReadyString(vProposedPoId)
        End If
        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("SubTotal", GetType(Decimal), "Quantity * UnitPrice")

        Return pDT

    End Function

    Public Function GetProposedBySupplierId(ByVal vProposedPoId As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TransProposedPurchaseOrderItems.ID" & _
            ",TransProposedPurchaseOrderItems.Quantity" & _
            ",TransProposedPurchaseOrderItems.UnitMeasure" & _
            ",TransProposedPurchaseOrderItems.UnitQuantity" & _
            ",TransProposedPurchaseOrderItems.UnitMeasure" & _
            ",TransProposedPurchaseOrderItems.UnitPrice" & _
            ",TransProposedPurchaseOrderItems.StockID" & _
            ",LibraryStockInformation.StockNumber" & _
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" &
            ",TransProposedPurchaseOrderItems.Quantity * TransProposedPurchaseOrderItems.UnitPrice AS SubTotal"

        pSql &= " FROM TransProposedPurchaseOrderItems"
        pSql &= " JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransProposedPurchaseOrderItems.StockID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmployees ON LibraryStockInformation.UserID = tblEmployees.ID "

        pSql &= " WHERE TransProposedPurchaseOrderItems.ProposedPoId = " & ReadyString(vProposedPoId) & _
            " AND TransProposedPurchaseOrderItems.ProposedPoId = " & ReadyString(vProposedPoId) 

        pSql &= " ORDER BY ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)), ', ') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )), ', ') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )), ', ') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ', ') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )), ', ')"
        pSql &= ",TransProposedPurchaseOrderItems.UnitQuantity" & _
            ",TransProposedPurchaseOrderItems.UnitMeasure"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

#If ProjName = "ERP" Or ProjName = "WebErp" Then
    'Public Function GeneratePoFromProposed(ByVal vProposedPoId As String,
    '    ByRef oMsg As String) As Boolean

    '    Dim pDtProposed As DataTable    ' TransProposedPurchaseOrderSuppliers
    '    Dim pDrProposed As DataRow
    '    Dim pDtStocks As DataTable      ' TransProposedPurchaseOrderItems
    '    Dim pDrStock As DataRow
    '    Dim pSql As String
    '    Dim pIU As InsertUpdate
    '    Dim pPoId As String             ' TransStockPurchase.ID
    '    Dim pPoSupplierId As String     ' TransStockPurchaseSupplier.ID

    '    pSql = "SELECT ID" & _
    '        ",StockID" & _
    '        ",Quantity" & _
    '        ",UnitMeasure" & _
    '        ",UnitQuantity" & _
    '        ",UnitPrice" & _
    '        " FROM TransProposedPurchaseOrderItems" & _
    '        " WHERE ProposedPoId = " & ReadyString(vProposedPoId)
    '    pDtStocks = mProData.Execute(pSql)
    '    If pDtStocks.Rows.Count = 0 Then
    '        oMsg = "There are no proposed PO stocks."
    '        Return False
    '    End If

    '    pSql = "SELECT ID" & _
    '        ",SupplierId" &
    '        ",SupplierAddressID" & _
    '        ",Terms" & _
    '        ",ShipperID" & _
    '        ",DeliverAddressID" & _
    '        ",BillingAddressID" & _
    '        ",DateNeeded" & _
    '        ",CancellationDate" & _
    '        ",ActualShipped" & _
    '        ",WayBillNo" & _
    '        ",Remarks" & _
    '        ",ShipmentMode" & _
    '        " FROM TransProposedPurchaseOrders" & _
    '        " WHERE ID = " & ReadyString(vProposedPoId) & _
    '        " AND PoId IS NULL"
    '    pDtProposed = mProData.Execute(pSql)
    '    If pDtProposed.Rows.Count > 0 Then
    '        pDrProposed = pDtProposed.Rows(0)
    '    Else
    '        oMsg = "There are no proposed PO supplier and shipping info"
    '        Return False
    '    End If

    '    Try
    '        mProData.BeginTransaction()

    '        '// save to TransStockPurchase
    '        pIU = New InsertUpdate("TransStockPurchase")
    '        pPoId = CStr2(Guid.NewGuid)
    '        pIU("ID") = ReadyString(pPoId)
    '        pIU("UserID") = mParent.MainSecurity.UserAccess.EmployeeID
    '        pSql = pIU.GetInsert
    '        mProData.ExecuteSQL(pSql)

    '        '// save to TransStockPurchaseSupplier
    '        pIU = New InsertUpdate("TransStockPurchaseSupplier")
    '        pPoSupplierId = CStr2(Guid.NewGuid)
    '        pIU("ID") = ReadyString(pPoSupplierId)
    '        pIU("StockPurchaseID") = ReadyString(pPoId)
    '        pIU("SupplierID") = ReadyString(pDrProposed("SupplierId"))
    '        pIU("SupplierAddressID") = ReadyString(pDrProposed("SupplierAddressID"))
    '        pIU("ShipperID") = ReadyString(pDrProposed("ShipperID"))
    '        pIU("DeliverAddressID") = ReadyString(pDrProposed("DeliverAddressID"))
    '        pIU("BillingAddressID") = ReadyString(pDrProposed("BillingAddressID"))
    '        pIU("DateNeeded") = ReadyString(pDrProposed("DateNeeded"))
    '        pIU("CancellationDate") = ReadyString(pDrProposed("CancellationDate"))
    '        pIU("Remarks") = ReadyString(pDrProposed("Remarks"))
    '        pIU("ShipmentMode") = ReadyString(pDrProposed("ShipmentMode"))
    '        pIU("IsApproved") = 1
    '        pSql = pIU.GetInsert
    '        mProData.ExecuteSQL(pSql)

    '        '// save to TransStockPurchaseItems
    '        pIU = New InsertUpdate("TransStockPurchaseItems")
    '        pIU("StockPurchaseID") = ReadyString(pPoId)
    '        pIU("StockPurchaseSupplierID") = ReadyString(pPoSupplierId)
    '        For Each pDrStock In pDtStocks.Rows
    '            pIU("StockID") = ReadyString(pDrStock("StockID"))
    '            pIU("Quantity") = pDrStock("Quantity")
    '            pIU("ActualQuantity") = pDrStock("UnitQuantity")
    '            pIU("UnitMeasure") = ReadyString(pDrStock("UnitMeasure"))
    '            pIU("UnitPrice") = pDrStock("UnitPrice")
    '            pSql = pIU.GetInsert
    '            mProData.ExecuteSQL(pSql)
    '        Next pDrStock

    '        '// update TransProposedPurchaseOrders
    '        pIU = New InsertUpdate("TransProposedPurchaseOrders")
    '        pIU("PoId") = ReadyString(pPoId)
    '        pSql = pIU.GetUpdate("ID = " & ReadyString(vProposedPoId))
    '        mProData.ExecuteSQL(pSql)

    '        mProData.CommitTransaction()

    '        Return True
    '    Catch ex As Exception
    '        Try
    '            mProData.RollbackTransaction()
    '        Catch ex2 As Exception
    '            Stop2()
    '        End Try
    '        Throw
    '    End Try

    'End Function
#End If

    Public Function ById(vID As String) As DataSet

        Dim pDS As DataSet
        Dim pDT As DataTable
        Dim pSql As String

        pDS = New DataSet
        pSql = "SELECT TransStockPurchase.ID" & vbCrLf &
            " ,RIGHT ('0000000' + CONVERT (varchar, TransStockPurchase.PONo), 7) AS PONo" & vbCrLf & _
            " ,TransStockPurchase.DatePO" & vbCrLf & _
            " ,LibrarySuppliers.SupplierName" & vbCrLf & _
            " ,LibrarySuppliersAddress.Street + ', ' + LibrarySuppliersAddress.District + ', ' + LibraryLocations.LocationName AS SupplierAddress" & vbCrLf & _
            " ,LibrarySuppliers.ContactNumber" & vbCrLf & _
            " ,LibrarySuppliers.EmailAddress" & vbCrLf & _
            " ,TransStockPurchaseSupplier.Terms" & vbCrLf & _
            " ,TransStockPurchaseSupplier.IsApproved" & vbCrLf & _
            " ,TransStockPurchaseSupplier.IsCancelled" & vbCrLf & _
            " ,TransStockPurchaseSupplier.IsComputerGenerated" & vbCrLf & _
            " ,RIGHT ('0000000' + CONVERT (varchar, TransStockCanvass.CanvassNo), 7) AS CanvassNo" & vbCrLf & _
            " ,LibraryShipper.ShipperName" & vbCrLf & _
            " ,ISNULL(LibraryCustomersAddresses.StreetNumber, '') + ', ' + ISNULL(" & vbCrLf & _
            "  LibraryStreetNames.StreetName, '') + ', ' +" & vbCrLf & _
            "  LibraryCustomersAddresses.District + ', ' +" & vbCrLf & _
            "  LibraryLocations_1.LocationName AS DeliveryAddress" & vbCrLf & _
            " ,ISNULL(LibraryCustomersAddresses_1.StreetNumber, '') + ', ' + ISNULL(" & vbCrLf & _
            "  LibraryStreetNames_1.StreetName, '') + ', ' +" & vbCrLf & _
            "  LibraryCustomersAddresses_1.District + ', ' +" & vbCrLf & _
            "  LibraryLocations_2.LocationName AS BillingAddress" & vbCrLf & _
            " ,TransStockPurchaseSupplier.DateNeeded" & vbCrLf & _
            " ,TransStockPurchaseSupplier.CancellationDate" & vbCrLf & _
            " ,TransStockPurchaseSupplier.Remarks" & vbCrLf & _
            " ,TransStockPurchase.PermanentRemark" & vbCrLf & _
            " ,TransStockPurchase.Status" & vbCrLf
        pSql &= " FROM TransStockPurchase" & vbCrLf & _
            " JOIN TransStockPurchaseSupplier ON TransStockPurchase.ID = TransStockPurchaseSupplier.StockPurchaseID" & vbCrLf & _
            " JOIN LibrarySuppliers ON TransStockPurchaseSupplier.SupplierID = LibrarySuppliers.ID" & vbCrLf & _
            " JOIN LibrarySuppliersAddress ON TransStockPurchaseSupplier.SupplierAddressID = LibrarySuppliersAddress.ID" & vbCrLf & _
            " JOIN LibraryLocations ON LibrarySuppliersAddress.LocationID = LibraryLocations.ID" & vbCrLf & _
            " JOIN LibraryShipper ON TransStockPurchaseSupplier.ShipperID = LibraryShipper.ID" & vbCrLf & _
            " JOIN LibraryCustomersAddresses ON TransStockPurchaseSupplier.DeliverAddressID = LibraryCustomersAddresses.ID" & vbCrLf & _
            " JOIN LibraryLocations LibraryLocations_1 ON LibraryCustomersAddresses.LocationID = LibraryLocations_1.ID" & vbCrLf & _
            " JOIN LibraryCustomersAddresses LibraryCustomersAddresses_1 ON TransStockPurchaseSupplier.BillingAddressID = LibraryCustomersAddresses_1.ID" & vbCrLf & _
            " JOIN LibraryLocations LibraryLocations_2 ON LibraryCustomersAddresses_1.LocationID = LibraryLocations_2.ID" & vbCrLf & _
            " LEFT JOIN TransStockCanvass ON TransStockPurchaseSupplier.CanvassID = TransStockCanvass.ID" & vbCrLf & _
            " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID" & vbCrLf & _
            " LEFT JOIN LibraryStreetNames LibraryStreetNames_1 ON LibraryStreetNames_1.ID = LibraryCustomersAddresses_1.StreetID" & vbCrLf & _
            " WHERE TransStockPurchase.ID = " & ReadyString(vID) & vbCrLf & _
            " " & vbCrLf
        pDT = mProData.Execute(pSql)
        pDT.TableName = "Master"
        pDS.Tables.Add(pDT)

        pSql = "SELECT TransStockPurchaseItems.StockID" & vbCrLf & _
            " ,LibraryStockInformation.StockNumber" & vbCrLf & _
            " ,ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') +" & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') +" & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & vbCrLf & _
            " ,TransStockPurchaseItems.Quantity" & vbCrLf & _
            " ,TransStockPurchaseItems.UnitMeasure" & vbCrLf & _
            " ,TransStockPurchaseItems.Term" & vbCrLf & _
            " ,TransStockPurchaseItems.UnitPrice" & vbCrLf & _
            " ,TransStockPurchaseItems.SubTotal" & vbCrLf & _
            " ,TransStockPurchaseItems.ActualQuantity" & vbCrLf & _
            ",TransStockPurchaseItems.Quantity * TransStockPurchaseItems.ActualQuantity AS OrderedQuantity" & vbCrLf &
            " ,ISNULL(" & vbCrLf & _
            " 	(SELECT ISNULL(SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood), 0)" & vbCrLf & _
            " 	 FROM TransReceivingStock " & vbCrLf & _
            "      JOIN TransReceiving ON TransReceivingStock.ReceivingID = TransReceiving.ID" & vbCrLf & _
            " 	 WHERE TransReceivingStock.POID = TransStockPurchaseItems.StockPurchaseID" & vbCrLf & _
            " 	 AND TransReceivingStock.IsNonInventoryItem = 0" & vbCrLf & _
            " 	 AND TransReceiving.IsCancelled = 0" & vbCrLf & _
            " 	 AND TransReceivingStock.StockID = TransStockPurchaseItems.StockID" & vbCrLf & _
            " 	)" & vbCrLf & _
            " ,0) AS ReceivedQuantityGood" & vbCrLf & _
            " ,ISNULL(" & vbCrLf & _
            " 	(SELECT ISNULL(SUM(TransReceivingStock.QuantityDamaged * TransReceivingStock.ActualQuantityDamaged), 0)" & vbCrLf & _
            " 	 FROM TransReceivingStock " & vbCrLf & _
            "      JOIN TransReceiving ON TransReceivingStock.ReceivingID = TransReceiving.ID" & vbCrLf & _
            " 	 WHERE TransReceivingStock.POID = TransStockPurchaseItems.StockPurchaseID" & vbCrLf & _
            " 	 AND TransReceivingStock.IsNonInventoryItem = 0" & vbCrLf & _
            " 	 AND TransReceiving.IsCancelled = 0" & vbCrLf & _
            " 	 AND TransReceivingStock.StockID = TransStockPurchaseItems.StockID" & vbCrLf & _
            " 	)" & vbCrLf & _
            " ,0) AS ReceivedQuantityDamaged " & vbCrLf &
            " FROM TransStockPurchaseItems" & vbCrLf & _
            " JOIN LibraryStockInformation ON TransStockPurchaseItems.StockID = LibraryStockInformation.ID" & vbCrLf & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & vbCrLf & _
            " WHERE TransStockPurchaseItems.StockPurchaseID = " & ReadyString(vID) & vbCrLf & _
            " AND TransStockPurchaseItems.Quantity > 0" & vbCrLf & _
            " ORDER BY ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') +" & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') +" & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & vbCrLf & _
            "  ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '')" & vbCrLf
        pDT = mProData.Execute(pSql)
        pDT.TableName = "Details"
        pDS.Tables.Add(pDT)

        Return pDS

    End Function

    Public Function GetFollowUpHistory(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DrpTransPoFollowUp.Remarks" &
            ",tblEmp.LastName + ', ' + tblEmp.FirstName + ' ' + tblEmp.MiddleName AS FullName" &
            ",DrpTransPoFollowUp.LastUpdated" &
            " FROM DrpTransPoFollowUp" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmp ON tblEmp.ID = DrpTransPoFollowUp.UserId" &
            " WHERE DrpTransPoFollowUp.PoId = " & ReadyString(vID) &
            " ORDER BY DrpTransPoFollowUp.LastUpdated DESC"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function DrpWeek(vWeekNum As Integer) As Date?

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT Week" & Format(If(vWeekNum = 0, 1, vWeekNum), "00") & "Start" &
         " FROM DrpCacheDrpSettings"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            If vWeekNum = 0 Then
                Return CDate(pDT.Rows(0)(0)).AddDays(-7)
            Else
                Return pDT.Rows(0)(0)
            End If
        End If

    End Function

    Public Function DrpCacheEndDate() As Date?

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT EndUpdate" &
         " FROM DrpCacheDrpSettings"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            If IsDate(pDT.Rows(0)("EndUpdate")) Then
                Return pDT.Rows(0)("EndUpdate")
            End If
        End If

    End Function

    Public Class PoCreator
        Dim mBusiness As Business
        Dim mProData As ProData

        Public SupplierID As String
        Public SupplierAddressID As String
        Public ShipperID As String
        Public DeliverAddressID As String
        Public BillingAddressID As String
        Public DateNeeded As Date
        Public CancellationDate As Date
        Public Remarks As String
        Public ShipmentMode As String
        Public DtStocks As DataTable

        Public Sub New(vBusiness As Business)

            mBusiness = vBusiness
            mProData = vBusiness.ProData

        End Sub

        'Public Function Create() As Boolean

        '    Dim pPoSupplierId As String
        '    Dim pPoId As String             ' TransStockPurchase.ID
        '    Dim pSql As String
        '    Dim pIU As InsertUpdate

        '    Try
        '        mProData.BeginTransaction()

        '        '// save to TransStockPurchase
        '        pIU = New InsertUpdate("TransStockPurchase")
        '        pPoId = CStr2(Guid.NewGuid)
        '        pIU("ID") = ReadyString(pPoId)
        '        pIU("UserID") = mBusiness.MainSecurity.UserAccess.EmployeeID
        '        pIU("PermanentRemark") = ReadyString(mBusiness.SystemDefaults.POPermanentRemark)
        '        pSql = pIU.GetInsert
        '        mProData.ExecuteSQL(pSql)

        '        '// save to TransStockPurchaseSupplier
        '        pIU = New InsertUpdate("TransStockPurchaseSupplier")
        '        pPoSupplierId = CStr2(Guid.NewGuid)
        '        pIU("ID") = ReadyString(pPoSupplierId)
        '        pIU("StockPurchaseID") = ReadyString(pPoId)
        '        pIU("SupplierID") = ReadyString(SupplierID)
        '        pIU("SupplierAddressID") = ReadyString(SupplierAddressID)
        '        pIU("ShipperID") = ReadyString(ShipperID)
        '        pIU("DeliverAddressID") = ReadyString(DeliverAddressID)
        '        pIU("BillingAddressID") = ReadyString(BillingAddressID)
        '        pIU("DateNeeded") = ReadyString(DateNeeded)
        '        pIU("CancellationDate") = ReadyString(CancellationDate)
        '        pIU("Remarks") = ReadyString(Remarks)
        '        pIU("ShipmentMode") = ReadyString(ShipmentMode)
        '        pIU("IsApproved") = 1
        '        pSql = pIU.GetInsert
        '        mProData.ExecuteSQL(pSql)

        '        '// save to TransStockPurchaseItems
        '        pIU = New InsertUpdate("TransStockPurchaseItems")
        '        pIU("StockPurchaseID") = ReadyString(pPoId)
        '        pIU("StockPurchaseSupplierID") = ReadyString(pPoSupplierId)
        '        For Each pDrStock As DataRow In DtStocks.Rows
        '            pIU("StockID") = ReadyString(pDrStock("StockID"))
        '            pIU("Quantity") = pDrStock("Qty")
        '            pIU("ActualQuantity") = pDrStock("UnitQty")
        '            pIU("UnitMeasure") = ReadyString(pDrStock("Unit"))
        '            pIU("UnitPrice") = pDrStock("Price")
        '            pSql = pIU.GetInsert
        '            mProData.ExecuteSQL(pSql)
        '        Next pDrStock

        '        mProData.CommitTransaction()

        '        Return True
        '    Catch ex As Exception
        '        Try
        '            mProData.RollbackTransaction()
        '        Catch ex2 As Exception
        '            Stop2()
        '        End Try
        '        Throw
        '    End Try

        'End Function
    End Class
End Class
