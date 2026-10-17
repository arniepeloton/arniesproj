Public Class LibraryStockLegacy

    Dim mProData As ProData
    Dim mParent As Business

    Public Enum enmDefaultPriceLevel
        enmFreightCost = 0
        enmFactoryPrice = 1
        enmLandedCost = 2
        enmWholesalePrice = 3
        enmRetailPrice = 4
        enmOtherPriceLevel = 5
    End Enum

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Sub GetProfitability(ByVal vStockID As String, ByVal vQty As Long, ByVal vUnitQty As Long, _
        ByVal vUnitPrice As Decimal, _
        ByRef oUnitGP As Decimal, ByRef oUnitGPM As Double, ByRef oLineGP As Decimal, ByRef oLineGPM As Double, _
        ByRef oLandedCost As Decimal, ByVal vHistoricCost As Decimal)

        Dim pCost As Decimal

        oLandedCost = GetLandedCost(vStockID) '* vUnitQty
        If vHistoricCost = 0 Then
            pCost = oLandedCost
        Else
            pCost = vHistoricCost
        End If

        If vQty = 0 Then
            oUnitGP = 0
            oUnitGPM = 0
            oLineGP = 0
            oLineGPM = 0
        Else
            oUnitGP = vUnitPrice - pCost * vUnitQty
            If pCost <> 0 And vUnitPrice <> 0 Then
                oUnitGPM = oUnitGP / vUnitPrice
            Else
                oUnitGPM = 0
            End If
            If vQty <> 0 Then
                oLineGP = oUnitGP * vQty
                If vUnitPrice * vQty <> 0 Then
                    oLineGPM = oLineGP / (vUnitPrice * vQty)
                Else
                    oLineGPM = 0
                End If
            End If
        End If

    End Sub

    Public Function GetLandedCost(ByVal vStockID As String, Optional ByRef oLastUpdated As Date = Nothing) As Decimal

        Return GetPrice(vStockID, "Landed Cost ID", "{01000012-0003-0000-0000-000000000000}", oLastUpdated)

    End Function

    Public Function GetFactoryPrice(ByVal vStockID As String, Optional ByRef oLastUpdated As Date = Nothing) As Decimal

        Return GetPrice(vStockID, "Factory Price ID", "{01000012-0002-0000-0000-000000000000}", oLastUpdated)

    End Function

    Public Function GetFreightCost(ByVal vStockID As String, Optional ByRef oLastUpdated As Date = Nothing) As Decimal

        Return GetPrice(vStockID, "Freight Cost ID", "{01000012-0001-0000-0000-000000000000}", oLastUpdated)
    End Function

    ''' <summary>
    ''' returns -1 if not found
    ''' </summary>
    ''' <param name="vStockID"></param>
    ''' <param name="vPriceLevelText"> from SystemDefaults
    ''' Eg. "Landed Cost ID", "Factory Price ID"</param>
    ''' <param name="vPriceLevelID">the default value from SystemDefaults </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPrice(ByVal vStockID As String,
        ByVal vPriceLevelText As String,
        ByVal vPriceLevelID As String,
        ByRef oLastUpdated As Date) As Decimal

        Dim pSql As String
        Dim pDT As DataTable

        pSql = mParent.SystemDefaults.GetOrAddSystemDefault(vPriceLevelText, vPriceLevelID)
        If IsGuid(pSql) = False Then pSql = vPriceLevelID

        pSql = "SELECT ISNULL(Price,0) as Price " & _
            ",LastUpdated" &
            " From LibraryStockPriceLevels " & _
            " WHERE PriceLevelID = " & ReadyString(pSql) & _
            " AND StockID = " & ReadyString(vStockID)
        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            oLastUpdated = pDT.Rows(0)("LastUpdated")
            Return pDT.Rows(0)("Price")
        Else
            Return -1
        End If

    End Function

    Public Function GetPrice(ByVal vID As String, _
        ByVal vUnitQty As Integer, _
        ByVal vPriceLevelID As String) As Decimal

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim pGuid As String
        Dim pPrice As Decimal

        pSql = "SELECT Price" & _
            ",DeactivateDiscount" & _
            " FROM LibraryStockPriceLevels" & _
            " WHERE StockID = " & ReadyString(vID) & _
            " AND PriceLevelID = " & ReadyString(vPriceLevelID) & _
            " AND Price > 0"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pDR = pDT.Rows(0)
            If CDec2(pDR("DeactivateDiscount")) = 0 Then
                pPrice = pDR("Price")
                pSql = "SELECT Discount" & _
                    " FROM LibraryStockMeasurements" & _
                    " WHERE StockID = " & ReadyString(vID) & _
                    " AND Quantity = " & vUnitQty
                pDT = mProData.Execute(pSql)
                If pDT.Rows.Count > 0 Then
                    pDR = pDT.Rows(0)
                    pPrice = pPrice * vUnitQty
                    GetPrice = pPrice + pPrice * CDec2(pDR("Discount")) / 100
                Else
                    GetPrice = pPrice * vUnitQty
                End If
            Else
                GetPrice = pPrice * vUnitQty
            End If
        Else
            pGuid = mParent.SystemDefaults.GetOrAddSystemDefault("Retail Price ID", "{01000012-0005-0000-0000-000000000000}")
            If pGuid = vPriceLevelID Then
                Exit Function
            Else
                GetPrice = GetPrice(vID, vUnitQty, pGuid)
            End If
        End If

    End Function

    Public Function MakeStockNumbersUnique() As Integer

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim pNumber As Integer
        Dim pStockNumber As String
        Dim p As Integer
        Dim pIU As New InsertUpdate("LibraryStockInformation")

        pSql = "SELECT COUNT(*) AS Count" & _
            ",StockNumber" & _
            " FROM LibraryStockInformation" & _
            " WHERE IsActive = 1" & _
            " GROUP BY StockNumber" & _
            " HAVING COUNT(*) > 1"
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            If CStr(pDR("StockNumber")).Length >= 3 Then
                pNumber = 0
                For p = 2 To pDR("Count")
                    pStockNumber = NextAvailableNumber(pDR("StockNumber"), pNumber)
                    If pStockNumber.Length > 0 Then
                        pSql = "SELECT TOP 1 ID" & _
                            " FROM LibraryStockInformation" & _
                            " WHERE StockNumber = " & ReadyString(pDR("StockNumber")) & _
                            " AND IsActive = 1"
                        pIU("StockNumber") = ReadyString(pStockNumber)
                        pIU("Code") = pNumber
                        pSql = pIU.GetUpdate("ID IN (" & pSql & ")")
                        mProData.ExecuteSQL(pSql)
                    End If
                Next p
            End If
        Next pDR

    End Function

    Private Function NextAvailableNumber(ByVal vStockNumber As String, ByRef oNumber As Integer) As String

        Dim pDT As DataTable
        Dim pSql As String
        Dim p As Integer
        Dim pPrefix As String
        Dim pSuffix As String

        If vStockNumber.Length < 3 Then Return ""
        pPrefix = vStockNumber.Substring(0, vStockNumber.Length - 3)
        pSuffix = vStockNumber.Substring(vStockNumber.Length - 3, 3)
        If IsNumeric(pSuffix) = False Then Return ""

        If oNumber = 0 Then
            p = CDec(pSuffix)
        Else
            p = oNumber
        End If
        Do
            p += 1
            pSql = "SELECT ID" & _
                " FROM LibraryStockInformation" & _
                " WHERE StockNumber = " & ReadyString(pPrefix & Format(p, "000"))
            pDT = mProData.Execute(pSql)
            If pDT.Rows.Count = 0 Then
                oNumber = p
                Return pPrefix & Format(p, "000")
            End If
        Loop

    End Function

    Public Function GetBranchCost(ByVal vID As String, ByRef oPriceLevelID As String, _
            ByRef rActualQuantity As Integer, _
            ByRef rUnitMeasure As String) As Decimal

        Dim pDT As DataTable
        Dim pPriceLevelID As String
        Dim pStr As String

        pDT = PriceTable(vID)

        '// get the available unit quantity and unit measurement
        pDT.DefaultView.RowFilter = "Quantity = " & rActualQuantity
        If pDT.DefaultView.Count > 0 Then
            rUnitMeasure = pDT.DefaultView(0)("UnitMeasure")
        Else
            pDT.DefaultView.RowFilter = "UnitMeasure = " & ReadyString(rUnitMeasure)
            If pDT.DefaultView.Count > 0 Then
                rActualQuantity = pDT.DefaultView(0)("Quantity")
            Else
                pDT.DefaultView.RowFilter = "Quantity = 1"
                If pDT.DefaultView.Count > 0 Then
                    rActualQuantity = 1
                    rActualQuantity = pDT.DefaultView(0)("Quantity")
                Else
                    rActualQuantity = 0
                    rUnitMeasure = ""
                    ' no hope
                    Return 0
                End If
            End If
        End If

        pPriceLevelID = "{01000012-0006-0000-0000-000000000000}"
        pDT.DefaultView.RowFilter = "PriceLevelID = " & ReadyString(ReadyGuid(pPriceLevelID)) & _
            " AND Quantity = " & rActualQuantity & _
            " AND DiscountedPrice > 0"
        If pDT.DefaultView.Count > 0 Then
            oPriceLevelID = pPriceLevelID
            Return pDT.DefaultView(0)("DiscountedPrice")
        End If

        pPriceLevelID = mParent.SystemDefaults.GetWholesalePriceID
        pDT.DefaultView.RowFilter = "PriceLevelID = " & ReadyString(ReadyGuid(pPriceLevelID)) & _
            " AND Quantity = " & rActualQuantity & _
            " AND DiscountedPrice > 0"
        If pDT.DefaultView.Count > 0 Then
            oPriceLevelID = pPriceLevelID
            Return pDT.DefaultView(0)("DiscountedPrice")
        End If

        pPriceLevelID = mParent.SystemDefaults.GetRetailPriceID
        pDT.DefaultView.RowFilter = "PriceLevelID = " & ReadyString(ReadyGuid(pPriceLevelID)) & _
            " AND Quantity = " & rActualQuantity & _
            " AND DiscountedPrice > 0"
        If pDT.DefaultView.Count > 0 Then
            oPriceLevelID = pPriceLevelID
            Return pDT.DefaultView(0)("DiscountedPrice")
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vId"></param>
    ''' <param name="vCustomerID"></param>
    ''' <param name="vLowestPriceOnly">display only the lowest priced price level per unit qty</param>
    ''' <param name="vIncludeLastPriceRequest">If true, includes 2 additional columns: CustomerLastInvoicePrice, CustomerLastInvoiceDate</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PriceTable(ByVal vId As String, Optional ByVal vCustomerID As String = Nothing,
        Optional vLowestPriceOnly As Boolean = False,
        Optional vRetailPrice As Boolean = False,
        Optional vIncludeBaseUnit As Boolean = False,
        Optional vIncludeLastPriceRequest As Boolean = False) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pWhere As String
        Dim pFieldBaseUnit As String
        Dim pSqlBaseUnit As String

#If DEBUG Then
        Dim pSW As New Diagnostics.Stopwatch
        pSW.Start()
#End If

        If vRetailPrice Then
            pWhere = " AND LibraryStockPriceLevels.PriceLevelID = " & ReadyString(mParent.SystemDefaults.GetRetailPriceID) & vbCrLf
        End If
        If vIncludeBaseUnit Then
            pFieldBaseUnit = ",tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
                ",tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf
            pSqlBaseUnit = "CROSS APPLY (" & vbCrLf &
                "SELECT TOP 1 LibraryUnitMeasures_2.UnitMeasure" &
                ",LibraryStockMeasurements_2.Quantity AS UnitQty" &
                " FROM LibraryStockMeasurements LibraryStockMeasurements_2" &
                " JOIN LibraryUnitMeasures LibraryUnitMeasures_2 ON LibraryStockMeasurements_2.UnitMeasureID = LibraryUnitMeasures_2.ID" & _
                " WHERE LibraryStockMeasurements_2.StockID = LibraryStockMeasurements.StockID" &
                " ORDER BY LibraryStockMeasurements_2.Quantity" & vbCrLf &
                ") tblBaseUnit" & vbCrLf
        End If
        pSql = "SELECT DISTINCT TOP 100 PERCENT " & vbCrLf & _
            "LibraryStockPriceLevels.PriceLevelID," & vbCrLf & _
            "LibraryStockMeasurements.MeasurementLevel," & vbCrLf & _
            "LibraryStockMeasurements.Barcode," & vbCrLf & _
            "LibraryUnitMeasures.UnitMeasure," & vbCrLf & _
            "LibraryStockMeasurements.Quantity," & vbCrLf & _
            "LibraryPriceLevels.PriceLevelName," & vbCrLf & _
            " ~LibraryStockPriceLevels.DeactivateDiscount * LibraryStockMeasurements.Discount AS Discount," & vbCrLf & _
            "CONVERT(Money, LibraryStockMeasurements.Quantity * LibraryStockPriceLevels.Price) AS OriginalPrice," & vbCrLf & _
            " LibraryStockMeasurements.Quantity * LibraryStockPriceLevels.Price - (LibraryStockMeasurements.Quantity * LibraryStockPriceLevels.Price) * (LibraryStockMeasurements.Discount / 100) * ~LibraryStockPriceLevels.DeactivateDiscount AS DiscountedPrice" & vbCrLf & _
            ",LibraryStockPriceLevels.Price - LibraryStockPriceLevels.Price * (LibraryStockMeasurements.Discount / 100) * ~LibraryStockPriceLevels.DeactivateDiscount AS DiscountedPricePerUnit," & vbCrLf & _
            "LibraryStockMeasurements.Dimension," & vbCrLf & _
            "LibraryStockMeasurements.Weight" & vbCrLf & _
            ",LibraryStockPriceLevels.LastUpdated" & vbCrLf & _
            ",tblEP.LastName + ', ' + tblEP.FirstName + ' ' + tblEP.MiddleName AS LastUpdatedBy" & vbCrLf & _
            ",LibraryStockMeasurements.Discount AS MeasurementDiscount" & vbCrLf & _
            ",LibraryStockPriceLevels.DeactivateDiscount" & vbCrLf
        If vIncludeLastPriceRequest Then
            pSql &= ",tblLastInvoice.UnitPrice AS CustomerLastInvoicePrice" & vbCrLf &
                ",tblLastInvoice.InvoiceDate AS CustomerLastInvoiceDate" & vbCrLf
        End If
        pSql &= pFieldBaseUnit
        pSql &= " FROM LibraryStockMeasurements" & vbCrLf & _
            " JOIN LibraryStockPriceLevels ON LibraryStockMeasurements.StockID = LibraryStockPriceLevels.StockID" & vbCrLf & _
            " JOIN LibraryUnitMeasures ON LibraryStockMeasurements.UnitMeasureID = LibraryUnitMeasures.ID" & vbCrLf & _
            " JOIN LibraryPriceLevels ON LibraryStockPriceLevels.PriceLevelID = LibraryPriceLevels.ID " & vbCrLf & _
            pSqlBaseUnit &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEP ON tblEP.ID = LibraryStockPriceLevels.UserID" & vbCrLf
        If CStr2(vCustomerID) <> "" Then
            pSql &= " JOIN LibraryCustomersPriceLevel" & vbCrLf & _
                " ON LibraryCustomersPriceLevel.PriceLevelID = LibraryPriceLevels.ID" & vbCrLf & _
                " AND LibraryCustomersPriceLevel.CustomerID = " & ReadyString(vCustomerID) & vbCrLf
        End If
        If vIncludeLastPriceRequest Then
            pSql &= " OUTER APPLY (" & vbCrLf & _
                " 	SELECT TOP 1 TransInvoice.InvoiceDate" & vbCrLf & _
                "     ,TransInvoiceItems.UnitPrice" & vbCrLf & _
                "     FROM TransInvoice" & vbCrLf & _
                "     JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf & _
                "     WHERE TransInvoiceItems.StockID = LibraryStockPriceLevels.StockID" & vbCrLf & _
                "     AND TransInvoice.CustomerID = " & ReadyString(vCustomerID) & vbCrLf & _
                "     AND TransInvoiceItems.ActualQuantity = LibraryStockMeasurements.Quantity" & vbCrLf & _
                "     AND TransInvoice.IsCancelled = 0" & vbCrLf & _
                "     ORDER BY TransInvoice.InvoiceDate DESC" & vbCrLf & _
                "     ,TransInvoice.LastUpdated DESC" & vbCrLf & _
                " ) tblLastInvoice" & vbCrLf

        End If
        pSql &= " WHERE (LibraryStockMeasurements.StockID = '" & vId & "') " & vbCrLf & _
            " AND LibraryStockPriceLevels.PriceLevelID <> " & ReadyString(mParent.SystemDefaults.GetOrAddSystemDefault("Freight Cost ID", "{01000012-0001-0000-0000-000000000000}")) & vbCrLf & _
            " AND LibraryStockPriceLevels.PriceLevelID <> " & ReadyString(mParent.SystemDefaults.GetOrAddSystemDefault("Factory Price ID", "{01000012-0002-0000-0000-000000000000}")) & vbCrLf & _
            " AND LibraryStockPriceLevels.PriceLevelID <> " & ReadyString(mParent.SystemDefaults.GetOrAddSystemDefault("Landed Cost ID", "{01000012-0003-0000-0000-000000000000}")) & vbCrLf & _
            " AND LibraryPriceLevels.IsActive = 1" & vbCrLf & _
            pWhere &
            " ORDER BY LibraryPriceLevels.PriceLevelName" & vbCrLf & _
            ",LibraryStockMeasurements.MeasurementLevel" & vbCrLf

        If vLowestPriceOnly Then
            pSql = "WITH CTE AS (" & vbCrLf &
                pSql & vbCrLf &
                ")" & vbCrLf
            pSql = pSql & " SELECT CTE2.*" & vbCrLf & _
                " FROM " & vbCrLf & _
                " 	(SELECT CTE.Quantity, MIN(CTE.DiscountedPrice) AS DiscountedPrice" & vbCrLf & _
                " 	FROM CTE" & vbCrLf & _
                " 	GROUP BY CTE.Quantity" & vbCrLf & _
                " ) CTE" & vbCrLf & _
                " CROSS APPLY" & vbCrLf & _
                " 	(SELECT TOP 1 * " & vbCrLf & _
                "     FROM CTE CTE2" & vbCrLf & _
                "     WHERE CTE2.Quantity = CTE.Quantity" & vbCrLf & _
                "     AND CTE2.DiscountedPrice = CTE.DiscountedPrice    " & vbCrLf & _
                "     ORDER BY CTE2.DiscountedPrice" & vbCrLf & _
                "     ,CTE2.LastUpdated DESC" & vbCrLf & _
                " ) CTE2" & vbCrLf
        End If

        pDT = mProData.Execute(pSql)
#If DEBUG Then
        Debug.Print("PriceTable: " & pSW.ElapsedMilliseconds & " ms")
#End If

        Return pDT

    End Function

    Public Function Measurements(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

#If DEBUG Then
        Dim pSW As New Diagnostics.Stopwatch
        pSW.Start()
#End If
        pSql = "SELECT LibraryStockMeasurements.MeasurementLevel," & _
            "LibraryStockMeasurements.Barcode," & _
            "LibraryUnitMeasures.UnitMeasure," & _
            "LibraryStockMeasurements.Quantity," & _
            "LibraryStockMeasurements.Dimension," & _
            "LibraryStockMeasurements.Weight " & _
            ",LibraryStockMeasurements.Quantity"
        pSql &= " FROM LibraryStockMeasurements" & _
            " JOIN LibraryUnitMeasures ON LibraryStockMeasurements.UnitMeasureID = LibraryUnitMeasures.ID"
        pSql &= " WHERE LibraryStockMeasurements.StockID = " & ReadyString(vID) & _
            " ORDER BY LibraryStockMeasurements.Quantity"

        pDT = mProData.Execute(pSql)
#If DEBUG Then
        Debug.Print("Measurements: " & pSW.ElapsedMilliseconds & " ms")
#End If

        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vIds"></param>
    ''' <param name="vDtPriceLevels">ID = LibraryPriceLevels.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ByIds(ByVal vIds As String, Optional ByVal vDtPriceLevels As DataTable = Nothing) As DataTable

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT LibraryStockInformation.ID" & _
            ",LibraryStockInformation.StockNumber" & _
            ",ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName     )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make    )) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
            " ISNULL(LTRIM(RTRIM(LibraryStockInformation.Content )) + ', ', '') AS StockDescription" & _
            ",tblEmployees.LastName + ', ' + tblEmployees.FirstName + ' ' + tblEmployees.MiddleName AS UpdatedBy" & _
            ",LibraryStockInformation.IsActive"
        If vDtPriceLevels IsNot Nothing Then
            pSql &= ",LibraryBrands.BrandName" & _
                ",LibraryProducts.ProductName" & _
                ",LibraryDepartments.DepartmentName"
            For Each pDR In vDtPriceLevels.Select("ISNULL(ID, '') <> ''")
                pSql &= ",(SELECT TOP 1 Price" & _
                    " FROM LibraryStockPriceLevels" & _
                    " WHERE LibraryStockPriceLevels.StockID = LibraryStockInformation.ID" & _
                    " AND LibraryStockPriceLevels.PriceLevelID = " & ReadyString(pDR("ID")) & _
                    " ORDER BY LastUpdated DESC, Price DESC) AS [" & CStr2(pDR("ID")) & "]"
            Next pDR
        End If
        pSql &= " FROM LibraryStockInformation" & _
            " LEFT JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " LEFT JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID " & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmployees ON LibraryStockInformation.UserID = tblEmployees.ID "
        If vDtPriceLevels IsNot Nothing Then
            pSql &= " LEFT JOIN LibraryDepartments ON LibraryDepartments.ID = LibraryStockInformation.DepartmentID"

        End If
        pSql &= " WHERE LibraryStockInformation.ID IN (" & vIds & ")"

        Return mProData.Execute(pSql)

    End Function

#If ProjName = "ERP" Then
    ''' <summary>
    ''' This is will add the factory cost + freight cost as landed cost if (factory cost + freight cost) is higher than the current landed cost
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FixLandedCost(ByRef oError As String) As Integer

        Dim pFactoryPriceID As String
        Dim pFreightCostID As String
        Dim pLandedCostID As String

        Dim pFactory As Decimal
        Dim pFreight As Decimal
        Dim pLanded As Decimal

        Dim pSql As String
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pIU As InsertUpdate
        Dim pResult As Integer

        oError = Nothing
#If Debug = 0 Then
        Try
#End If
        pIU = New InsertUpdate("LibraryStockPriceLevels")
        pFactoryPriceID = mParent.SystemDefaults.GetOrAddSystemDefault("Factory Price ID", "{01000012-0002-0000-0000-000000000000}")
        pFreightCostID = mParent.SystemDefaults.GetOrAddSystemDefault("Freight Cost ID", "{01000012-0001-0000-0000-000000000000}")
        pLandedCostID = mParent.SystemDefaults.GetOrAddSystemDefault("Landed Cost ID", "{01000012-0003-0000-0000-000000000000}")

        pSql = "SELECT ID FROM LibraryStockInformation"
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pSql = "SELECT Price" & _
                " FROM LibraryStockPriceLevels" & _
                " WHERE StockID = " & ReadyString(pDR("ID")) & _
                " AND PriceLevelID = " & ReadyString(pFactoryPriceID)
            pFactory = CDec2(mProData.ExecuteScalar(pSql))

            pSql = "SELECT Price" & _
                " FROM LibraryStockPriceLevels" & _
                " WHERE StockID = " & ReadyString(pDR("ID")) & _
                " AND PriceLevelID = " & ReadyString(pFreightCostID)
            pFreight = CDec2(mProData.ExecuteScalar(pSql))

            pSql = "SELECT Price" & _
                " FROM LibraryStockPriceLevels" & _
                " WHERE StockID = " & ReadyString(pDR("ID")) & _
                " AND PriceLevelID = " & ReadyString(pLandedCostID)
            pLanded = CDec2(mProData.ExecuteScalar(pSql))

            If pLanded < pFactory + pFreight Then
                pIU.Reset()
                pIU("Price") = pFactory + pFreight
                pIU("LastUpdated") = "GETDATE()"
                pSql = pIU.GetUpdate("StockID = " & ReadyString(pDR("ID")) & _
                    " AND PriceLevelID = " & ReadyString(pLandedCostID))
                If mProData.ExecuteSQL(pSql) = 0 Then
                    pIU("StockID") = ReadyString(pDR("ID"))
                    pIU("PriceLevelID") = ReadyString(pLandedCostID)
                    pIU("AdjustmentMethod") = "'Peso Value'"
                    pIU("UserID") = gBusiness.MainSecurity.UserAccess.EmployeeID
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                End If
                pResult += 1
            End If
        Next pDR
#If Debug = 0 Then
        Catch ex As Exception
            ' just ignore any error on "Release" version
            oError = ex.Message
        End Try
#End If

        Return pResult

    End Function

    ' updates the price of all parent stocks of a particular content stock
    Public Function UpdateKitPrices(ByVal vContentStockID As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        Dim pPriceIDs As List(Of String)
        Dim pPriceID As String
        Dim p As Integer
        Dim pRecordsAffected As Long
        Dim pDT As DataTable
        Dim pDR As DataRow

        mProData.BeginTransaction()
        pDT = GetParentStocks(vContentStockID)
        If pDT.Rows.Count > 0 Then
            pPriceIDs = New List(Of String)
            pPriceIDs.Add(mParent.SystemDefaults.GetOrAddSystemDefault("Factory Price ID", "{01000012-0002-0000-0000-000000000000}"))
            pPriceIDs.Add(mParent.SystemDefaults.GetOrAddSystemDefault("Freight Cost ID", "{01000012-0001-0000-0000-000000000000}"))
            pPriceIDs.Add(mParent.SystemDefaults.GetOrAddSystemDefault("Landed Cost ID", "{01000012-0003-0000-0000-000000000000}"))

            pIU = New InsertUpdate
            pIU.TableName = "LibraryStockPriceLevels"
            For Each pDR In pDT.Rows
                For Each pPriceID In pPriceIDs
                    pIU.Reset()
                    pIU("Price") = Format(KitPrice(CStr2(pDR("StockID")), pPriceID), cGlobal.FormatCurrencySave)
                    pIU("AdjustmentMethod") = "'Peso Value'"
                    pIU("UserID") = gBusiness.MainSecurity.UserAccess.EmployeeID
                    pIU("LastUpdated") = "GETDATE()"
                    pSql = pIU.GetUpdate("StockID = " & ReadyString(pDR("StockID")) & " AND PriceLevelID = " & ReadyString(pPriceID))
                    pRecordsAffected = mProData.ExecuteSQL(pSql)
                    If pRecordsAffected = 0 Then
                        pIU("StockID") = ReadyString(pDR("StockID"))
                        pIU("PriceLevelID") = ReadyString(pPriceID)
                        pSql = pIU.GetInsert
                        mProData.ExecuteSQL(pSql)
                    End If
                Next pPriceID
                UpdateKitPrices(CStr2(pDR("StockID")))
            Next pDR
        End If
        mProData.CommitTransaction()

        Return True

    End Function
#End If

    ' save the current price to history
    Public Sub SavePriceHistory(ByVal vStockID As String, ByVal vPriceLevelID As String)

        Dim pDT As DataTable
        Dim pPrice As Double
        Dim pSql As String
        Dim pIU As InsertUpdate

        pSql = "SELECT Price" & _
            ",UserID" & _
            ",LastUpdated" & _
            " FROM LibraryStockPriceLevels" & _
            " WHERE StockID = " & ReadyString(vStockID) & _
            " AND PriceLevelID = " & ReadyString(vPriceLevelID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pPrice = LatestHistoricPrice(vStockID, vPriceLevelID)
            If pPrice >= 0 Then
                If Math.Abs(pPrice - pDT.Rows(0)("Price")) > 0.0009 Then
                    ' not the same price, so save to history
                Else
                    ' same price as history
                    Exit Sub
                End If
            Else
                ' no history yet, so save the current price as history
            End If

            pIU = New InsertUpdate
            pIU.TableName = "LibraryStockPriceHistory"
            pIU("StockID") = ReadyString(vStockID)
            pIU("PriceLevelID") = ReadyString(vPriceLevelID)
            pIU("Price") = pDT.Rows(0)("Price")
            pIU("UserID") = pDT.Rows(0)("UserID")
            pIU("LastUpdated") = ReadyString(pDT.Rows(0)("LastUpdated"))
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Public Function LatestHistoricPrice(ByVal vStockID As String, ByVal vPriceLevelID As String, _
        Optional ByVal vExcludePrice As Double = 0) As Double

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 2 Price" & _
            " FROM LibraryStockPriceHistory" & _
            " WHERE StockID = " & ReadyString(vStockID) & _
            " AND PriceLevelID = " & ReadyString(vPriceLevelID) & _
            " ORDER BY LastUpdated DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            If Math.Abs(pDT.Rows(0)("Price") - vExcludePrice) > 0.0009 Then
                Return pDT.Rows(0)("Price")
            ElseIf pDT.Rows.Count > 1 Then
                Return pDT.Rows(1)("Price")
            End If
        Else
            Return -1
        End If

    End Function

    Public Function KitPrice(ByVal vStockID As String, ByVal vPriceLevelID As String) As Decimal

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ISNULL(SUM(Price), 0) FROM (" & _
            " SELECT " & _
            "   (SELECT TOP 1 Price * LibraryStockKits.Quantity * LibraryStockKits.ActualQuatity" & _
            "    FROM LibraryStockPriceLevels" & _
            "    WHERE LibraryStockPriceLevels.StockID = LibraryStockKits.StockKitID" & _
            "    AND LibraryStockPriceLevels.PriceLevelID = " & ReadyString(vPriceLevelID) & _
            "    ORDER BY LibraryStockPriceLevels.LastUpdated DESC) AS Price" & _
            " FROM LibraryStockKits" & _
            " WHERE StockID = " & ReadyString(vStockID) & _
            " ) t"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)(0)
        End If

    End Function

    Public Function GetParentStocks(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT StockID" & _
            " FROM LibraryStockKits" & _
            " WHERE StockKitID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function AddToTotalStockOut(ByVal vID As String, ByVal vQty As Integer, _
        ByRef oError As String,
        Optional ByVal vBranchID As Integer? = Nothing) As Boolean

        Dim pSql As String

        oError = Nothing
        If vBranchID Is Nothing Then
            vBranchID = mParent.LibraryBranches.CurrentBranchDesignation
        End If

        pSql = "UPDATE WholesaleStockInventory" & _
            " SET TotalStocksOut = TotalStocksOut + " & vQty & _
            ",LastUpdated = Getdate() " & _
            " WHERE StockID = " & ReadyString(vID) & _
            " AND BranchID = " & vBranchID
        mProData.ExecuteSQL(pSql)

        SetCriticalItem(vID, vBranchID)

        Return True

    End Function

    Public Enum eLedgerTransactionType
        LedgerInvoicing
        LedgerMemo
        LedgerCustomerPayment
        LedgerReceiving
        LedgerPayables
        LedgerPOSCashSlipSales
        LedgerPOSSales
        LedgerPOSSalesReturn
        LedgerPOSReceivingStock
        LedgerUnKnown
        LedgerStockReturns
    End Enum

    Public Sub SaveIntoInventoryLedger(ByVal vStockID As String, _
        ByVal vTransaction As String, _
        ByVal vAmount As Double, _
        ByVal vDebit As Long, _
        ByVal vCredit As Long, _
        ByVal vTransactionType As eLedgerTransactionType, _
        ByVal vReferenceID As String, _
        ByVal vFreight As Double, _
        ByVal vFactory As Double, _
        Optional ByVal vIsReceiving As Boolean = False,
        Optional vBranchID As Integer? = Nothing)

        Dim pTransactionType As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        Select Case vTransactionType
            Case eLedgerTransactionType.LedgerCustomerPayment
                pTransactionType = "Customer Payment"
            Case eLedgerTransactionType.LedgerInvoicing
                pTransactionType = "Sales Invoice"
            Case eLedgerTransactionType.LedgerMemo
                pTransactionType = "Memo"
            Case eLedgerTransactionType.LedgerPayables
                pTransactionType = "Payables"
            Case eLedgerTransactionType.LedgerPOSCashSlipSales
                pTransactionType = "POS Cash Slip Sales"
            Case eLedgerTransactionType.LedgerPOSSales
                pTransactionType = "POS Sales"
            Case eLedgerTransactionType.LedgerPOSSalesReturn
                pTransactionType = "POS Sales Return"
            Case eLedgerTransactionType.LedgerPOSReceivingStock
                pTransactionType = "POS Stock Receiving"
            Case eLedgerTransactionType.LedgerReceiving
                pTransactionType = "Stock Receiving"
            Case eLedgerTransactionType.LedgerUnKnown
                pTransactionType = "Unknown"
            Case eLedgerTransactionType.LedgerStockReturns
                pTransactionType = "Stock Return"
        End Select

        pIU = New InsertUpdate("WholesaleInventoryLedger")
        pIU("StockID") = ReadyString(vStockID)
        pIU("InvTransaction") = ReadyString(vTransaction)
        pIU("Amount") = vAmount
        pIU("Debit") = vDebit
        pIU("Credit") = vCredit
        pIU("Balance") = GetStockOnHand(vStockID)
        pIU("IsReceiving") = CDec2(vIsReceiving)
        pIU("SourceModule") = ReadyString(pTransactionType)
        pIU("ReferenceID") = ReadyString(vReferenceID)
        If vBranchID Is Nothing Then vBranchID = mParent.LibraryBranches.CurrentBranchDesignation
        pIU("BranchID") = vBranchID
#If _MYTYPE = "Web" Then
        pIU("UserID") = WebSession.Instance.EmployeeID
#Else
        pIU("UserID") = User.EmployeeID
#End If
        pSql = pIU.GetInsert

        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function DimensionValue(ByVal vText As String) As Double

        Dim pWidthFeet As Double
        Dim pWidthInches As Double
        Dim pHeightFeet As Double
        Dim pHeightInches As Double

        DimensionValue = 1

        If ParseDimension(vText, pWidthFeet, pWidthInches, pHeightFeet, pHeightInches) Then
            DimensionValue = _
                (pWidthFeet _
                + pWidthInches / 12) _
                * (pHeightFeet _
                + pHeightInches / 12)
        End If

    End Function

    Public Function GetStockOnHand(ByVal vStockID As String) As Integer

        Dim pSql As String

        pSql = "SELECT StocksOnHand " & _
            " FROM WholesaleStockInventory " & _
            " WHERE StockID = " & ReadyString(vStockID) & _
            " AND BranchID = " & mParent.LibraryBranches.CurrentBranchDesignation

        Return CDec2(mProData.ExecuteScalar(pSql))

    End Function

    Public Function ParseDimension(ByVal vText As String, _
        ByRef oWidthFeet As Double, ByRef oWidthInches As Double, _
        ByRef oHeightFeet As Double, ByRef oHeightInches As Double) As Boolean

        Dim pDimensions() As String
        Dim pPos1 As Integer    ' feet (')
        Dim pPos2 As Integer    ' inches (")

        pDimensions = Split(vText, "x")
        If UBound(pDimensions) > 0 Then
            '// Width
            pPos1 = InStr(1, pDimensions(0), "'")
            pPos2 = InStr(1, pDimensions(0), """")

            ' sort
            If pPos1 = 0 Then
                oWidthInches = Mid(pDimensions(0), 1, pPos2 - 1)
            ElseIf pPos2 = 0 Then
                oWidthFeet = Mid(pDimensions(0), 1, pPos1 - 1)
            ElseIf pPos1 < pPos2 Then
                oWidthFeet = Mid(pDimensions(0), 1, pPos1 - 1)
                oWidthInches = Mid(pDimensions(0), pPos1 + 1, pPos2 - pPos1 - 1)
            ElseIf pPos2 < pPos1 Then
                oWidthInches = Mid(pDimensions(0), 1, pPos2 - 1)
                oWidthFeet = Mid(pDimensions(0), pPos2 + 1, pPos1 - pPos2 - 1)
            End If

            '// Height
            pPos1 = InStr(1, pDimensions(1), "'")
            pPos2 = InStr(1, pDimensions(1), """")

            ' sort
            If pPos1 = 0 Then
                oHeightInches = Mid(pDimensions(1), 1, pPos2 - 1)
            ElseIf pPos2 = 0 Then
                oHeightFeet = Mid(pDimensions(1), 1, pPos1 - 1)
            ElseIf pPos1 < pPos2 Then
                oHeightFeet = Mid(pDimensions(1), 1, pPos1 - 1)
                oHeightInches = Mid(pDimensions(1), pPos1 + 1, pPos2 - pPos1 - 1)
            ElseIf pPos2 < pPos1 Then
                oHeightInches = Mid(pDimensions(1), 1, pPos2 - 1)
                oHeightFeet = Mid(pDimensions(1), pPos2 + 1, pPos1 - pPos2 - 1)
            End If

            ParseDimension = True
        End If

    End Function

    Public Function GetModeSupplierPrice(ByVal vID As String) As Decimal

        Dim pSupplierID As String

        pSupplierID = ModeSupplierID(vID)
        If pSupplierID <> vbNullString Then
            GetModeSupplierPrice = CDec2(LastSupplierPrice(vID, pSupplierID))
        End If

    End Function

    Public Function ModeSupplierID(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TOP 1" & _
            " TransReceiving.SupplierID" & _
            ", SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS Qty" & _
            " FROM TransReceivingApproval " & _
            " JOIN TransReceiving ON TransReceivingApproval.ReceivingID = TransReceiving.ID " & _
            " JOIN TransReceivingStock ON TransReceiving.ID = TransReceivingStock.ReceivingID" & _
            " WHERE TransReceiving.IsCancelled = 0" & _
            " AND TransReceivingApproval.IsCancelled = 0" & _
            " AND TransReceivingStock.StockID = " & ReadyString(vID) & _
            " AND DATEdiff(day, TransReceiving.DateReceived, GETDATE()) <= 182" & _
            " GROUP BY TransReceiving.SupplierID" & _
            " ORDER BY SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) DESC"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            ModeSupplierID = pDT.Rows(0)("SupplierID").ToString.ToUpper
        End If
        pDT.Dispose()

    End Function

    Public Function LastSupplierPrice(ByVal vID As String, ByVal vSupplierID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        '// this is the supplier price base on the latest approved stock receiving with that supplier
        'pSql = " SELECT TOP 1 " & _
        '    " TransReceivingStock.LandedCost / TransReceivingStock.ActualQuantityGood AS Price" & _
        '    " FROM TransReceivingApproval " & _
        '    " JOIN TransReceiving ON TransReceivingApproval.ReceivingID = TransReceiving.ID " & _
        '    " JOIN TransReceivingStock ON TransReceiving.ID = TransReceivingStock.ReceivingID" & _
        '    " WHERE TransReceiving.IsCancelled = 0" & _
        '    " AND TransReceivingApproval.IsCancelled = 0" & _
        '    " AND TransReceivingStock.StockID = " & ReadyString(vID) & _
        '    " AND TransReceiving.SupplierID = " & ReadyString(vSupplierID) & _
        '    " ORDER BY TransReceiving.DateReceived DESC" & _
        '    ",TransReceiving.LastUpdated DESC"

        '// this is the current supplier price
        pSql = "SELECT TOP 1 Price / ActualQuantity AS Price" & _
            " FROM LibrarySupplierStockPrices" & _
            " JOIN LibrarySupplierStocks" & _
            " ON LibrarySupplierStocks.ID = LibrarySupplierStockPrices.SupplierStockID" & _
            " WHERE StockID = " & ReadyString(vID) & _
            " AND SupplierID = " & ReadyString(vSupplierID) & _
            " ORDER BY ActualQuantity"
        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            If pDT.Rows(0)("Price") IsNot DBNull.Value Then
                LastSupplierPrice = pDT.Rows(0)("Price").ToString
                If 0 < CDec2(LastSupplierPrice) Then
                    LastSupplierPrice = Trim(Str(CDec2(LastSupplierPrice) + GetFreightCost(vID)))
                End If

            End If
        Else
            LastSupplierPrice = ""
        End If

    End Function

    Public Function DateLastApprovedCount(ByVal vStockID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MAX(DateAndTimeEnded) AS DateAndTimeEnded" & _
            " FROM TransCycleCountStocks" & _
            " JOIN TransCycleCount" & _
            " ON TransCycleCountStocks.CycleCountID = TransCycleCount.ID" & _
            " WHERE TransCycleCountStocks.StockID = " & ReadyString(vStockID) & _
            " AND IsCancelled = 0" & _
            " AND ApprovedBy <> 0"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            If pDT.Rows(0)("DateAndTimeEnded") Is DBNull.Value Then
                Return "..."
            Else
                Return pDT.Rows(0)("DateAndTimeEnded")
            End If
        Else
            Return "..."
        End If

    End Function

    Public Function SetCriticalItem(ByVal vStockID As String, Optional vBranchID As Integer? = Nothing)
        Dim pSql As String

        If vBranchID Is Nothing Then vBranchID = mParent.LibraryBranches.CurrentBranchDesignation
        pSql = "Update WholesaleStockInventory Set " & _
            "IsCritical=1 " & _
            " Where (MinStockNumber > StocksOnHand) " & _
            " AND (TotalStocksIn <> 0 OR TotalStocksOut <> 0) " & _
            " AND (StockID='" & vStockID & "') " & _
            " AND (BranchID=" & vBranchID & ") "

        mProData.ExecuteSQL(pSql)

    End Function

    ''' <summary>
    ''' also search by stock number
    ''' </summary>
    ''' <param name="vBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetIDsByBarcode(ByVal vBarcode As String) As String

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT DISTINCT LibraryStockInformation.ID " & _
            " FROM LibraryStockInformation" & _
            " LEFT JOIN LibraryStockMeasurements" & _
            " ON LibraryStockMeasurements.StockID = LibraryStockInformation.ID" & _
            " LEFT JOIN LibraryStockMeasurementsBarcode" & _
            " ON LibraryStockMeasurementsBarcode.StockMeasurementsID = LibraryStockMeasurements.ID " & _
            " WHERE LTRIM(RTRIM(LibraryStockMeasurementsBarcode.Barcode)) = " & ReadyString(CStr2(vBarcode).Trim) &
            " OR LibraryStockInformation.StockNumber = " & ReadyString(vBarcode)
        pDT = mProData.Execute(pSql)

        pSql = ""
        For Each pDR In pDT.Rows
            If pSql <> "" Then pSql = pSql & ", "
            pSql = pSql & ReadyString(pDR("ID"))
        Next pDR

        Return pSql

    End Function

    Public Function GetLedger(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT WholesaleInventoryLedger.ID" & _
            ",WholesaleInventoryLedger.DateTimeStamp" & _
            ",WholesaleInventoryLedger.InvTransaction" & _
            ",CASE WholesaleInventoryLedger.IsReceiving WHEN 1 THEN 0 ELSE WholesaleInventoryLedger.Amount END AS Amount" & _
            ",WholesaleInventoryLedger.Debit" & _
            ",WholesaleInventoryLedger.Credit" & _
            ",WholesaleInventoryLedger.Balance" & _
            ",DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.MiddleName AS UpdatedBy" & _
            ",WholesaleInventoryLedger.IsReceiving " & _
            " FROM WholesaleInventoryLedger" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles ON WholesaleInventoryLedger.UserID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID " & _
            " WHERE WholesaleInventoryLedger.StockID = " & ReadyString(vID) & _
            " AND WholesaleInventoryLedger.DateTimeStamp BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
            " AND WholesaleInventoryLedger.BranchID = " & mParent.LibraryBranches.CurrentBranchDesignation &
            " ORDER BY DateTimeStamp ASC"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSales2(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT 'Sales To External Customer' AS InvTransaction" & vbCrLf & _
            " ,0 AS Debit," & vbCrLf & _
            " ISNULL((SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity)" & vbCrLf & _
            " FROM TransInvoiceItems" & vbCrLf & _
            " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf & _
            " JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" & vbCrLf & _
            " WHERE TransInvoiceItems.StockID = " & ReadyString(vID) & vbCrLf & _
            " AND TransInvoice.IsCancelled = 0" & vbCrLf & _
            " AND LibraryCustomers.IsCompanyBranch = 0" & vbCrLf & _
            " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & "),0) AS Credit," & vbCrLf & _
            " 1 as SequenceNo" & vbCrLf & _
            " FROM LibraryBranches" & vbCrLf & _
            " " & vbCrLf & _
            " UNION" & vbCrLf & _
            " " & vbCrLf & _
            " SELECT DISTINCT 'Sales Return From External Customer' AS InvTransaction" & vbCrLf & _
            " ,ISNULL((Select SUM(TransMemoSalesReturn.QuantityReturned) AS Debit " & vbCrLf & _
            " FROM TransMemoSalesReturn" & vbCrLf & _
            " JOIN LibraryCustomers ON TransMemoSalesReturn.CustomerID = LibraryCustomers.ID " & vbCrLf & _
            " JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID" & vbCrLf & _
            " WHERE TransMemoSalesReturn.StockID = " & ReadyString(vID) & vbCrLf & _
            " AND LibraryCustomers.IsCompanyBranch = 0" & vbCrLf & _
            " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & "),0) AS Debit," & vbCrLf & _
            " 0 AS Credit," & vbCrLf & _
            " 2 as SequenceNo " & vbCrLf & _
            " FROM LibraryBranches" & vbCrLf & _
            " " & vbCrLf & _
            " UNION" & vbCrLf & _
            " " & vbCrLf & _
            " SELECT 'POS Sales' AS InvTransaction" & vbCrLf & _
            " ,0 AS Debit" & vbCrLf & _
            " ,ISNULL(SUM(POSSalesItems.Quantity * POSSalesItems.ActualQuantity), 0) AS Credit" & vbCrLf & _
            " ,3 AS SequenceNo" & vbCrLf & _
            " FROM POSSalesItems" & vbCrLf & _
            " JOIN POSSales ON POSSales.ID = POSSalesItems.POSSalesID" & vbCrLf & _
            " WHERE POSSalesItems.StockID = " & ReadyString(vID) & vbCrLf & _
            " AND CONVERT(date, POSSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf & _
            " " & vbCrLf & _
            " UNION" & vbCrLf & _
            " " & vbCrLf & _
            " SELECT 'POS Returns' AS InvTransaction" & vbCrLf & _
            " ,ISNULL(SUM(POSSalesReturnItems.Quantity * POSSalesReturnItems.ActualQuantity), 0) AS Debit" & vbCrLf & _
            " ,0 AS Credit" & vbCrLf & _
            " ,4 AS SequenceNo" & vbCrLf & _
            " FROM POSSalesReturnItems" & vbCrLf & _
            " JOIN POSSalesReturn ON POSSalesReturn.ID = POSSalesReturnItems.SalesReturnID" & vbCrLf & _
            " JOIN POSSales ON POSSales.ID = POSSalesReturn.ReferenceID" & vbCrLf & _
            " WHERE POSSalesReturnItems.StockID = " & ReadyString(vID) & vbCrLf & _
            " AND CONVERT(date, POSSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf & _
            " AND POSSalesReturn.SalesType IN (1, 2)" & vbCrLf & _
            " " & vbCrLf & _
            " UNION" & vbCrLf & _
            " " & vbCrLf & _
            " SELECT 'Cash Slip Sales' AS InvTransaction" & vbCrLf & _
            " ,0 AS Debit" & vbCrLf & _
            " ,ISNULL(SUM(POSCashSlipSalesItems.Quantity * POSCashSlipSalesItems.ActualQuantity), 0) AS Credit" & vbCrLf & _
            " ,5 AS SequenceNo" & vbCrLf & _
            " FROM POSCashSlipSalesItems" & vbCrLf & _
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSCashSlipSalesItems.CashSlipID" & vbCrLf & _
            " WHERE POSCashSlipSalesItems.StockID = " & ReadyString(vID) & vbCrLf & _
            " AND CONVERT(date, POSCashSlipSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf & _
            " " & vbCrLf & _
            " UNION" & vbCrLf & _
            " " & vbCrLf & _
            " SELECT 'Cash Slip Returns' AS InvTransaction" & vbCrLf & _
            " ,ISNULL(SUM(POSSalesReturnItems.Quantity * POSSalesReturnItems.ActualQuantity), 0) AS Debit" & vbCrLf & _
            " ,0 AS Credit" & vbCrLf & _
            " ,6 AS SequenceNo" & vbCrLf & _
            " FROM POSSalesReturnItems" & vbCrLf & _
            " JOIN POSSalesReturn ON POSSalesReturn.ID = POSSalesReturnItems.SalesReturnID" & vbCrLf & _
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSSalesReturn.ReferenceID" & vbCrLf & _
            " WHERE POSSalesReturnItems.StockID = " & ReadyString(vID) & vbCrLf & _
            " AND CONVERT(date, POSCashSlipSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf & _
            " AND POSSalesReturn.SalesType = 3" & vbCrLf & _
            " " & vbCrLf & _
            " ORDER BY SequenceNo" & vbCrLf

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSales2Net(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As Integer

        Dim pDT As DataTable

        pDT = GetSales2(vID, vDateFrom, vDateTo)

        Return CDec2(pDT.Compute("SUM(Credit) - SUM(Debit)", ""))

    End Function

    ''' <summary>
    ''' summary for ucLibraryStockSales
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSales(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT 'Sales To External Customer' AS InvTransaction," & _
            " 0 AS Debit," & _
            " ISNULL((SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity)" & _
            " FROM TransInvoiceItems" &
            " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & _
            " JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" & _
            " WHERE TransInvoiceItems.StockID = " & ReadyString(vID) & _
            " AND TransInvoice.IsCancelled = 0" & _
            " AND LibraryCustomers.IsCompanyBranch = 0" & _
            " AND TransInvoice.InvoiceDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & "),0) AS Credit," & _
            " 1 as SequenceNo" & _
            " FROM LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Sales Return From External Customer' AS InvTransaction," & _
            "ISNULL((Select SUM(TransMemoSalesReturn.QuantityReturned) AS Debit " & _
            "        FROM TransMemoSalesReturn INNER JOIN " & _
            "        LibraryCustomers ON TransMemoSalesReturn.CustomerID = LibraryCustomers.ID " & _
            " WHERE TransMemoSalesReturn.StockID = " & ReadyString(vID) & _
            " AND LibraryCustomers.IsCompanyBranch = 0" & _
            " AND TransMemoSalesReturn.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & "),0) as Debit," & _
            " 0 AS Credit," & _
            " 2 as SequenceNo " & _
            " FROM LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT 'Sales To Branches' AS InvTransaction," & _
            "0 AS Debit," & _
            "ISNULL((Select SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) as Credit " & _
            "        FROM TransInvoiceItems INNER JOIN " & _
            "        TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID INNER JOIN " & _
            "        LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID " & _
            "        WHERE TransInvoiceItems.StockID = " & ReadyString(vID) & _
            "        AND TransInvoice.IsCancelled = 0 " & _
            "        AND LibraryCustomers.IsCompanyBranch = 1" & _
            "        AND TransInvoice.InvoiceDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & "),0) as Credit," & _
            "3 as SequenceNo " & _
            " From LibraryBranches"

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Sales Return From Branches' AS InvTransaction," & _
            "ISNULL((Select SUM(TransMemoSalesReturn.QuantityReturned) AS Debit " & _
            "        FROM TransMemoSalesReturn INNER JOIN " & _
            "        LibraryCustomers ON TransMemoSalesReturn.CustomerID = LibraryCustomers.ID " & _
            "        WHERE TransMemoSalesReturn.StockID = " & ReadyString(vID) & _
            "        AND LibraryCustomers.IsCompanyBranch = 1" & _
            "        AND TransMemoSalesReturn.LastUpdated BETWEEN " & ReadyString(vDateFrom) & "  AND " & ReadyString(vDateTo) & "),0) as Debit," & _
            "0 AS Credit," & _
            "4 as SequenceNo " & _
            " From LibraryBranches"

        pSql &= " Order By SequenceNo "

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' summary for ucLibraryStockSales
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSalesBackOrder(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT 'Backorder To External Customer' AS InvTransaction," & _
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " & _
            " FROM TransSalesBackOrderItems" &
            " JOIN TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID" &
            " JOIN LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " & _
            " WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) & _
            " AND TransSalesBackOrderItems.IsCancelled = 0 " & _
            " AND TransSalesBackOrderItems.IsServe = 0" & _
            " AND LibraryCustomers.IsCompanyBranch = 0" & _
            " AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
            " ) " & _
            ",0) AS Debit, " & _
            "0 as Credit," & _
            "1 as SequenceNo " & _
            " From LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Cancelled Backorder Of External Customer' AS InvTransaction," & _
            "0 as Debit," & _
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " & _
            "        FROM TransSalesBackOrderItems INNER JOIN " & _
            "        TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " & _
            "        LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " & _
            "        WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) & _
            "        AND TransSalesBackOrderItems.IsCancelled = 1" & _
            "        AND LibraryCustomers.IsCompanyBranch = 0" & _
            "        AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
            "       ) " & _
            ",0) AS Credit," & _
            "2 as SequenceNo " & _
            " From LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT 'Backorder To Branches' AS InvTransaction," & _
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " & _
            "        FROM TransSalesBackOrderItems INNER JOIN " & _
            "        TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " & _
            "        LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " & _
            "        WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) & _
            "        AND TransSalesBackOrderItems.IsCancelled = 0" & _
            "        AND LibraryCustomers.IsCompanyBranch = 1" & _
            "        AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
            "       ) " & _
            ",0) AS Debit, " & _
            "0 as Credit," & _
            "3 as SequenceNo " & _
            " From LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Cancelled Backorder Of Branches' AS InvTransaction," & _
            "0 as Debit," & _
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " & _
            "        FROM TransSalesBackOrderItems INNER JOIN " & _
            "        TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " & _
            "        LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " & _
            "        WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) & _
            "        AND TransSalesBackOrderItems.IsCancelled = 1" & _
            "        AND LibraryCustomers.IsCompanyBranch = 1" & _
            "        AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
            "       ) " & _
            ",0) AS Credit," & _
            "4 as SequenceNo " & _
            " From LibraryBranches "
        pSql &= " Order By SequenceNo "

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' summary for ucLibraryStockPurchaseLedger
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPurchaseHistory(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT RIGHT('0000000' + CONVERT(varchar, TransStockPurchase.PONo), 7) AS PONo," & _
            "TransStockPurchase.DatePO AS PODate," & _
            "LibrarySuppliers.SupplierName," & _
            "TransStockPurchaseItems.Quantity," & _
            "TransStockPurchaseItems.UnitMeasure," & _
            "TransStockPurchaseItems.Term AS Terms," & _
            "TransStockPurchaseItems.UnitPrice," & _
            "TransStockPurchaseItems.SubTotal " & _
            " FROM TransStockPurchaseItems INNER JOIN " & _
            " TransStockPurchaseSupplier ON TransStockPurchaseItems.StockPurchaseSupplierID = TransStockPurchaseSupplier.ID INNER JOIN " & _
            " TransStockPurchase ON TransStockPurchaseSupplier.StockPurchaseID = TransStockPurchase.ID INNER JOIN " & _
            " LibrarySuppliers ON TransStockPurchaseSupplier.SupplierID = LibrarySuppliers.ID " & _
            " WHERE TransStockPurchaseItems.StockID = " & ReadyString(vID) & _
            " AND TransStockPurchase.DatePO BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & _
            " AND TransStockPurchaseSupplier.IsApproved = 1" & _
            " AND TransStockPurchaseSupplier.IsCancelled = 0"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function FixBarcodes() As Integer

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim pOut As Integer

        pSql = pSql & " SELECT DISTINCT Barcode" & vbCrLf & _
            " FROM LibraryStockMeasurementsBarcode" & vbCrLf & _
            " WHERE (Barcode NOT LIKE '%[0-9a-z]'" & vbCrLf & _
            " OR Barcode NOT LIKE '[1-9a-z]%'" & vbCrLf & _
            " OR Barcode LIKE '%[^0-9a-z]%'" & vbCrLf &
            ") AND Barcode <> ''"
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pSql = "UPDATE LibraryStockMeasurementsBarcode" &
                " SET Barcode = " & ReadyString(FixBarcode(pDR("Barcode"))) &
                " WHERE Barcode = " & ReadyString(pDR("Barcode"))
            pOut += mProData.ExecuteSQL(pSql)
        Next pDR

        Return pOut

    End Function

    Public Function FixBarcode(ByVal vBarcode As String) As String

        vBarcode = Regex.Replace(vBarcode, "\W", "")
        'dotNet = \A0+
        'VB6/Javascript = ^0+
        vBarcode = Regex.Replace(vBarcode, "\A0+", "")

        Return vBarcode

    End Function

    Public Function BestSupplierPrices(vIds As String, vIncludeAllSuppliers As Boolean) As DataTable

        Dim pDT, pDtUnits As DataTable
        Dim pDR As DataRow
        Dim pSql, pSqlStocks As String
        Dim pObj As Object
        Dim pDRV As DataRowView

        If vIncludeAllSuppliers Then
            pSql = pSql & " SELECT LibrarySupplierStocks.StockID" & vbCrLf & _
                " ,LibrarySupplierStocks.SupplierID" & vbCrLf & _
                " ,LibrarySuppliers.ShipmentMode" & vbCrLf & _
                " ,LibrarySuppliers.SupplierName" & vbCrLf & _
                " ,LibrarySupplierStockPrices.Price AS SupplierPrice" & vbCrLf & _
                " ,LibrarySupplierStockPrices.UnitMeasure AS SupplierUnitMeasure" & vbCrLf & _
                " ,LibrarySupplierStockPrices.ActualQuantity AS SupplierActualQuantity" & vbCrLf & _
                " ,LibrarySupplierStocks.LeadTime" & vbCrLf & _
                " ,LibrarySupplierStocks.MinimumOrderQty" & vbCrLf & _
                " ,LibrarySupplierStocks.MinimumOrderUnit" & vbCrLf & _
                " ,LibrarySupplierStocks.LotSize" & vbCrLf & _
                " ,LibrarySupplierStocks.LotSizeUnit" & vbCrLf & _
                " ,LibrarySuppliers.LeadTime AS SupplierLeadTime" & vbCrLf & _
                " ,LibrarySuppliers.MinimumOrderQty AS SupplierMinimumOrderQty" & vbCrLf & _
                " ,LibrarySuppliers.MinimumOrderUnit AS SupplierMinimumOrderUnit" & vbCrLf & _
                " ,LibrarySuppliers.LotSize AS SupplierLotSize" & vbCrLf & _
                " ,LibrarySuppliers.LotSizeUnit AS SupplierLotSizeUnit" & vbCrLf & _
                " ,LibrarySupplierStockPrices.LastUpdated" & vbCrLf & _
                " " & vbCrLf & _
                " FROM LibrarySuppliers" & vbCrLf & _
                " JOIN LibrarySupplierStocks ON LibrarySupplierStocks.SupplierID = LibrarySuppliers.ID" & vbCrLf & _
                " CROSS APPLY " & vbCrLf & _
                " (" & vbCrLf & _
                " 	SELECT TOP 1 LibrarySupplierStockPrices.ID" & vbCrLf & _
                "     ,LibrarySupplierStockPrices.Price" & vbCrLf & _
                "     ,LibrarySupplierStockPrices.UnitMeasure" & vbCrLf & _
                "     ,LibrarySupplierStockPrices.ActualQuantity" & vbCrLf & _
                "     ,LibrarySupplierStockPrices.LastUpdated" & vbCrLf & _
                "     FROM LibrarySupplierStockPrices" & vbCrLf & _
                "     WHERE LibrarySupplierStockPrices.SupplierStockID = LibrarySupplierStocks.ID" & vbCrLf & _
                "     ORDER BY LibrarySupplierStockPrices.LastUpdated DESC" & vbCrLf & _
                " ) AS LibrarySupplierStockPrices" & vbCrLf & _
                " " & vbCrLf & _
                " WHERE LibrarySupplierStocks.StockID IN (" & vIds & ")" & vbCrLf & _
                " AND LibrarySuppliers.IsActive = 1" & vbCrLf & _
                " " & vbCrLf & _
                " ORDER BY LibrarySupplierStockPrices.Price / LibrarySupplierStockPrices.ActualQuantity" & vbCrLf
        Else
            pSqlStocks = "SELECT ID AS StockId" & vbCrLf &
                " FROM LibraryStockInformation" & vbCrLf &
                " WHERE ID IN (" & vIds & ")" & vbCrLf

            pSql = "(SELECT TOP 1 LibrarySupplierStockPrices.ID" & vbCrLf & _
                " FROM LibrarySupplierStockPrices" & vbCrLf & _
                " JOIN " & vbCrLf & _
                " (" & vbCrLf & _
                "   SELECT LibrarySupplierStocks.StockID" & vbCrLf & _
                "   ,(SELECT TOP 1 LibrarySupplierStockPrices.ID" & vbCrLf & _
                "       FROM LibrarySupplierStockPrices" & vbCrLf & _
                "       WHERE LibrarySupplierStockPrices.SupplierStockID = LibrarySupplierStocks.ID" & vbCrLf & _
                "       ORDER BY LibrarySupplierStockPrices.LastUpdated DESC" & vbCrLf & _
                "   ) AS SupplierStockPricesID" & vbCrLf & _
                "   FROM LibrarySupplierStocks" & vbCrLf & _
                "   JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierId" & vbCrLf &
                "   WHERE LibrarySupplierStocks.StockId = t.StockId" & vbCrLf &
                "   AND LibrarySuppliers.IsActive = 1" & vbCrLf &
                " ) t2 ON t2.SupplierStockPricesID = LibrarySupplierStockPrices.ID" & vbCrLf & _
                " " & vbCrLf & _
                " WHERE t2.StockID = t.StockId" & vbCrLf & _
                " ORDER BY LibrarySupplierStockPrices.Price / LibrarySupplierStockPrices.ActualQuantity" & vbCrLf &
                ") AS SupplierStockPricesId" & vbCrLf

            pSql = "SELECT *, " & pSql & vbCrLf &
                " FROM (" & pSqlStocks & ") t" & vbCrLf

            pSql = "SELECT t.*" & vbCrLf &
                ",LibrarySupplierStocks.SupplierID" & vbCrLf &
                ",LibrarySuppliers.ShipmentMode" & vbCrLf &
                ",LibrarySuppliers.SupplierName" & vbCrLf &
                ",LibrarySupplierStockPrices.Price AS SupplierPrice" & vbCrLf &
                ",LibrarySupplierStockPrices.UnitMeasure AS SupplierUnitMeasure" & vbCrLf &
                ",LibrarySupplierStockPrices.ActualQuantity AS SupplierActualQuantity" & vbCrLf &
                ",LibrarySupplierStocks.LeadTime" & vbCrLf &
                ",LibrarySupplierStocks.MinimumOrderQty" & vbCrLf &
                ",LibrarySupplierStocks.MinimumOrderUnit" & vbCrLf &
                ",LibrarySupplierStocks.LotSize" & vbCrLf &
                ",LibrarySupplierStocks.LotSizeUnit" & vbCrLf &
                ",LibrarySuppliers.LeadTime AS SupplierLeadTime" & vbCrLf &
                ",LibrarySuppliers.MinimumOrderQty AS SupplierMinimumOrderQty" & vbCrLf &
                ",LibrarySuppliers.MinimumOrderUnit AS SupplierMinimumOrderUnit" & vbCrLf &
                ",LibrarySuppliers.LotSize AS SupplierLotSize" & vbCrLf &
                ",LibrarySuppliers.LotSizeUnit AS SupplierLotSizeUnit" & vbCrLf &
                " FROM (" & pSql & ") t" & vbCrLf &
                " LEFT JOIN LibrarySupplierStockPrices ON LibrarySupplierStockPrices.ID = t.SupplierStockPricesId" & vbCrLf &
                " LEFT JOIN LibrarySupplierStocks ON LibrarySupplierStocks.ID = LibrarySupplierStockPrices.SupplierStockID" & vbCrLf &
                " LEFT JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierID" & vbCrLf
        End If

        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("SupplierUnitMeasure2", GetType(String), "SupplierUnitMeasure + ' [' + SupplierActualQuantity + ']'")
        pDT.Columns.Add("MinimumOrderUnitQty", GetType(Integer))
        pDT.Columns.Add("LotSizeUnitQty", GetType(Integer))

        If pDT.Rows.Count > 0 Then
            pDtUnits = GetUnits(pDT)
            For Each pDR In pDT.Rows
                If pDR("LeadTime") Is DBNull.Value Then
                    If pDR("SupplierLeadTime") IsNot DBNull.Value Then
                        pDR("LeadTime") = pDR("SupplierLeadTime")
                    Else
                        pObj = mParent.SystemDefaults.VendorLeadTime
                        If pObj Is Nothing Then
                            pDR("LeadTime") = DBNull.Value
                        Else
                            pDR("LeadTime") = pObj
                        End If
                    End If
                End If

                If pDR("MinimumOrderUnit") Is DBNull.Value Then
                    If pDR("SupplierMinimumOrderUnit") IsNot DBNull.Value Then
                        pDtUnits.DefaultView.RowFilter = "StockID = " & ReadyString(pDR("StockId"))
                        If pDtUnits.DefaultView.Count > 0 Then
                            pDRV = pDtUnits.DefaultView(Math.Min(pDtUnits.DefaultView.Count - 1, Math.Max(1, pDR("SupplierMinimumOrderUnit")) - 1))
                            pDR("MinimumOrderUnit") = pDRV("UnitMeasure")
                            pDR("MinimumOrderUnitQty") = pDRV("UnitQty")
                        End If
                    Else
                        pObj = mParent.SystemDefaults.MinumumOrderUnit
                        If pObj IsNot Nothing Then
                            pDtUnits.DefaultView.RowFilter = "StockID = " & ReadyString(pDR("StockId"))
                            If pDtUnits.DefaultView.Count > 0 Then
                                pDRV = pDtUnits.DefaultView(Math.Min(pDtUnits.DefaultView.Count - 1, Math.Max(1, pObj) - 1))
                                pDR("MinimumOrderUnit") = pDRV("UnitMeasure")
                                pDR("MinimumOrderUnitQty") = pDRV("UnitQty")
                            End If
                        End If
                    End If
                Else
                    pDtUnits.DefaultView.RowFilter = "StockID = " & ReadyString(pDR("StockId")) & " AND UnitMeasure = " & ReadyString(pDR("MinimumOrderUnit"))
                    If pDtUnits.DefaultView.Count > 0 Then
                        pDR("MinimumOrderUnitQty") = pDtUnits.DefaultView(0)("UnitQty")
                    End If
                End If

                If pDR("MinimumOrderQty") Is DBNull.Value Then
                    If pDR("SupplierMinimumOrderQty") IsNot DBNull.Value Then
                        pDR("MinimumOrderQty") = pDR("SupplierMinimumOrderQty")
                    Else
                        pObj = mParent.SystemDefaults.MinumumOrderQuantity
                        If pObj Is Nothing Then
                            pDR("MinimumOrderQty") = DBNull.Value
                        Else
                            pDR("MinimumOrderQty") = pObj
                        End If
                    End If
                End If

                If pDR("LotSizeUnit") Is DBNull.Value Then
                    If pDR("SupplierLotSizeUnit") IsNot DBNull.Value Then
                        pDtUnits.DefaultView.RowFilter = "StockID = " & ReadyString(pDR("StockId"))
                        If pDtUnits.DefaultView.Count > 0 Then
                            pDRV = pDtUnits.DefaultView(Math.Min(pDtUnits.DefaultView.Count - 1, Math.Max(1, pDR("SupplierLotSizeUnit")) - 1))
                            pDR("LotSizeUnit") = pDRV("UnitMeasure")
                            pDR("LotSizeUnitQty") = pDRV("UnitQty")
                        End If
                    Else
                        pObj = mParent.SystemDefaults.LotSizeUnit
                        If pObj IsNot Nothing Then
                            pDtUnits.DefaultView.RowFilter = "StockID = " & ReadyString(pDR("StockId"))
                            If pDtUnits.DefaultView.Count > 0 Then
                                pDRV = pDtUnits.DefaultView(Math.Min(pDtUnits.DefaultView.Count - 1, Math.Max(1, pObj) - 1))
                                pDR("LotSizeUnit") = pDRV("UnitMeasure")
                                pDR("LotSizeUnitQty") = pDRV("UnitQty")
                            End If
                        End If
                    End If
                Else
                    pDtUnits.DefaultView.RowFilter = "StockID = " & ReadyString(pDR("StockId")) & " AND UnitMeasure = " & ReadyString(pDR("LotSizeUnit"))
                    If pDtUnits.DefaultView.Count > 0 Then
                        pDR("LotSizeUnitQty") = pDtUnits.DefaultView(0)("UnitQty")
                    End If
                End If

                If pDR("LotSize") Is DBNull.Value Then
                    If pDR("SupplierLotSize") IsNot DBNull.Value Then
                        pDR("LotSize") = pDR("SupplierLotSize")
                    Else
                        pObj = mParent.SystemDefaults.LotSize
                        If pObj Is Nothing Then
                            pDR("LotSize") = DBNull.Value
                        Else
                            pDR("LotSize") = pObj
                        End If
                    End If
                End If
            Next pDR
        End If

        Return pDT

    End Function

    Public Function GetBestSupplierPricesCache(vIds As String, vMinDateCached As Date, vSaveToCacheIfMissing As Boolean) As DataTable

        Dim pDT, pDtFresh As DataTable
        Dim pSql As String
        Dim pIds As String()
        Dim pId As String
        Dim pPendingIds As String = ""
        Dim pDR As DataRow

        pSql = "SELECT StockId" &
            ",BestPriceSupplierId" &
            ",BestPrice" &
            ",BestPriceUnit" &
            ",BestPriceUnitQty" &
            ",LibrarySuppliers.SupplierName" &
            ",LibrarySuppliers.ShipmentMode" &
            " FROM DrpLibStockBestPrice" &
            " JOIN LibrarySuppliers ON LibrarySuppliers.ID = DrpLibStockBestPrice.BestPriceSupplierId" &
            " WHERE StockId IN (" & vIds & ")" &
            " AND DateCached >= " & ReadyString(vMinDateCached) &
            " AND LibrarySuppliers.IsActive = 1"
        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("BestPriceUnit2", GetType(String), "BestPriceUnit + ' [' + BestPriceUnitQty + ']'")
        pIds = vIds.Split({","}, StringSplitOptions.RemoveEmptyEntries)
        If vSaveToCacheIfMissing _
            AndAlso pDT.Rows.Count < pIds.Length Then

            For Each pId In pIds
                If New DataView(pDT, "StockId = " & ReadyString(ReadyGuid(pId)), "", DataViewRowState.CurrentRows).Count = 0 Then
                    If pPendingIds.Length > 0 Then pPendingIds &= ","
                    pPendingIds &= pId
                End If
            Next pId
            If pPendingIds.Length > 0 Then
                pDtFresh = BestSupplierPrices(pPendingIds, False)
                pDtFresh.Columns("SupplierID").ColumnName = "BestPriceSupplierId"
                pDtFresh.Columns("SupplierPrice").ColumnName = "BestPrice"
                pDtFresh.Columns("SupplierUnitMeasure").ColumnName = "BestPriceUnit"
                pDtFresh.Columns("SupplierActualQuantity").ColumnName = "BestPriceUnitQty"
                pDtFresh.Columns("SupplierUnitMeasure2").ColumnName = "BestPriceUnit2"
                pDtFresh = New DataView(pDtFresh, "BestPriceSupplierId IS NOT NULL", "", DataViewRowState.CurrentRows).ToTable
                For Each pDR In pDtFresh.Rows
                    pDT.ImportRow(pDR)
                Next pDR
                SaveBestSupplierPricesCache(pDtFresh)
            End If
        End If

        Return pDT

    End Function

    Private Sub SaveBestSupplierPricesCache(vDt As DataTable)

        Dim pIU As InsertUpdate
        Dim pDR As DataRow
        Dim pSql As String

        pIU = New InsertUpdate("DrpLibStockBestPrice")
        For Each pDR In vDt.Rows
            pIU("StockId") = ReadyString(pDR("StockId"))
            pIU("DateCached") = "GETDATE()"
            pIU("BestPriceSupplierId") = ReadyString(pDR("BestPriceSupplierId"))
            pIU("BestPrice") = pDR("BestPrice")
            pIU("BestPriceUnit") = ReadyString(pDR("BestPriceUnit"))
            pIU("BestPriceUnitQty") = pDR("BestPriceUnitQty")
            pSql = pIU.GetInsert
            mProData.Execute(pSql)
        Next pDR

    End Sub

    Public Function GetBestSupplierPricesCache(vDtStockId As DataTable, vMinCacheDate As Date, vSaveToCacheIfMissing As Boolean) As DataTable

        Dim pDt, pDtAll As DataTable
        Dim pDR As DataRow

        For Each pDR In ExtractFieldValues(vDtStockId, "StockId").Rows
            pDt = GetBestSupplierPricesCache(pDR("StockId"), vMinCacheDate, vSaveToCacheIfMissing)
            If pDtAll Is Nothing Then
                pDtAll = pDt
            Else
                pDtAll.Merge(pDt)
            End If
        Next pDR

        Return pDtAll

    End Function

    ''' <summary>
    ''' primary key = StockId
    ''' </summary>
    ''' <param name="vDtStockId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUnits(vDtStockId As DataTable) As DataTable

        Dim pDt, pDtAll As DataTable
        Dim pDR As DataRow

        For Each pDR In ExtractFieldValues(vDtStockId, "StockId").Rows
            pDt = GetUnits(CStr(pDR("StockId"))) ' need to convert the parameter to string
            If pDtAll Is Nothing Then
                pDtAll = pDt
            Else
                pDtAll.Merge(pDt)
            End If
        Next pDR
        pDtAll.Columns.Add("UnitMeasure2", GetType(String), "UnitMeasure + '[' + UnitQty + ']'")

        Return pDtAll

    End Function

    Public Function GetUnits(vIds As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryStockMeasurements.StockID" &
            ",LibraryUnitMeasures.UnitMeasure" &
            ",LibraryStockMeasurements.Quantity AS UnitQty" &
            " FROM LibraryStockMeasurements" &
            " JOIN LibraryUnitMeasures ON LibraryStockMeasurements.UnitMeasureID = LibraryUnitMeasures.ID" & _
            " WHERE LibraryStockMeasurements.StockID IN (" & vIds & ")" &
            " ORDER BY LibraryStockMeasurements.Quantity"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetBasicUnit(vID As String) As String

        Dim pDT As DataTable
        Dim pStr As String

        pDT = GetUnits(ReadyString(vID))
        If pDT.Rows.Count > 0 Then
            pStr = pDT.Rows(0)("UnitMeasure")
            If pDT.Rows(0)("UnitQty") > 1 Then
                pStr &= " [" & pDT.Rows(0)("UnitQty") & "]"
            End If
        End If

        Return pStr

    End Function

    Public Function GetBackOrders(vID As String, Optional vCancellationDateFrom As Date? = Nothing, Optional vCancellationDateTo As Date? = Nothing) As Integer

        Dim pSql As String
        Dim pWhere As String

        If vCancellationDateFrom IsNot Nothing Then
            pWhere = " AND DATEADD(day, TransSalesBackOrderItems.CancellationPeriod, TransSalesBackOrderItems.RegisteredDate) >= " & ReadyString(vCancellationDateFrom.Value.Date)
        End If
        If vCancellationDateTo IsNot Nothing Then
            pWhere &= " AND DATEADD(day, TransSalesBackOrderItems.CancellationPeriod, TransSalesBackOrderItems.RegisteredDate) <= " & ReadyString(vCancellationDateTo.Value.Date)
        End If

        pSql = "SELECT SUM(Quantity * ActualQuantity) AS BasedQuantity" & vbCrLf & _
            " FROM TransSalesBackOrderItems" & vbCrLf & _
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransSalesBackOrderItems.SalesOrderID" & vbCrLf & _
            " WHERE TransSalesBackOrderItems.IsCancelled = 0 " & vbCrLf & _
            " AND IsServe = 0 " & vbCrLf & _
            " AND StockID = " & ReadyString(vID) & vbCrLf & _
            " AND IsInvoiced = 0" & vbCrLf &
            " AND DATEDIFF(day, TransSalesBackOrderItems.RegisteredDate, GETDATE()) < TransSalesBackOrderItems.CancellationPeriod" & vbCrLf & _
            " AND TransSalesOrder.BranchID = " & mParent.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 0) &
            pWhere
        pSql = CDec2(mProData.ExecuteScalar(pSql))

        Return pSql

    End Function

    Public Function GetHistoricWeeklySales(vID As String, vWeekStartDate As Date, vYearsAgo As Integer, vWeeksCount As Integer, vSalesAndBackOrders As Boolean) As DataTable

        Dim pWeekNum As Integer
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pCalendar As System.Globalization.GregorianCalendar
        Dim pIncrement As Integer

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("WeeekNum", GetType(Integer))
            .Add("QtySold", GetType(Integer))
            .Add("DateFrom", GetType(Date))
            .Add("DateTo", GetType(Date))
            .Add("DateRange")
        End With

        pCalendar = New System.Globalization.GregorianCalendar
        pWeekNum = pCalendar.GetWeekOfYear(vWeekStartDate, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday)

        For pIncrement = 0 To vWeeksCount - 1
            pDrOut = pDtOut.NewRow
            pDrOut("WeeekNum") = pIncrement + 1
            pDrOut("DateFrom") = pCalendar.AddWeeks(FirstFullWeek(Now.Year - vYearsAgo), pWeekNum + pIncrement)
            pDrOut("DateTo") = pCalendar.AddWeeks(FirstFullWeek(Now.Year - vYearsAgo), pWeekNum + pIncrement + 1).AddDays(-1)
            pDrOut("DateRange") = DisplayableDateRange(pDrOut("DateFrom"), pDrOut("DateTo"))
            If vSalesAndBackOrders Then
                pDrOut("QtySold") = GetDemandTotal(vID,
                    pDrOut("DateFrom"),
                    pDrOut("DateTo"))
            Else
                pDrOut("QtySold") = GetSales2Net(vID,
                    pDrOut("DateFrom"),
                    pDrOut("DateTo"))
            End If
            pDtOut.Rows.Add(pDrOut)
        Next pIncrement

        Return pDtOut

    End Function

    ''' <summary>
    ''' orders (active) and backorders (active and cancelled)
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDemandTotal(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As Integer

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) AS Total" & vbCrLf & _
            " FROM TransSalesOrder" & vbCrLf & _
            " JOIN TransSalesOrderItems ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & vbCrLf & _
            " WHERE CONVERT(date, TransSalesOrder.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf & _
            " AND TransSalesOrderItems.StockID = " & ReadyString(vID) & vbCrLf &
            " AND TransSalesOrder.IsCancelled = 0" & vbCrLf &
            " " & vbCrLf & _
            " UNION " & vbCrLf & _
            " " & vbCrLf & _
            " SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Total" & vbCrLf & _
            " FROM TransSalesBackOrderItems" & vbCrLf & _
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransSalesBackOrderItems.SalesOrderID" & vbCrLf & _
            " WHERE CONVERT(date, TransSalesOrder.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf & _
            " AND TransSalesBackOrderItems.StockID = " & ReadyString(vID)

        pDT = mProData.Execute(pSql)

        Return CDec2(pDT.Compute("SUM(Total)", ""))

    End Function

    Public Function GetDemand(vWeekStartDate As Date, vYearsAgo As Integer, vWeeksCount As Integer, vExcludeMustCount As Boolean,
        Optional vStockID As String = Nothing) As DataSet

        Dim pWeekNum As Integer
        Dim pDt, pDtData As DataTable
        Dim pDr As DataRow
        Dim pDtDates As DataTable
        Dim pDrDates As DataRow
        Dim pCalendar As System.Globalization.GregorianCalendar
        Dim pIncrement As Integer
        Dim pDsOut As DataSet

        pDtDates = New DataTable
        pDtDates.TableName = "Dates"
        With pDtDates.Columns
            .Add("WeeekNum", GetType(Integer))
            .Add("QtySold", GetType(Integer))
            .Add("DateFrom", GetType(Date))
            .Add("DateTo", GetType(Date))
            .Add("DateRange")

            .Add("NowDateFrom", GetType(Date))
            .Add("NowDateTo", GetType(Date))
            .Add("NowDateRange")
        End With

        pCalendar = New System.Globalization.GregorianCalendar
        pWeekNum = pCalendar.GetWeekOfYear(vWeekStartDate, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday)

        For pIncrement = 0 To vWeeksCount - 1
            pDrDates = pDtDates.NewRow
            pDrDates("WeeekNum") = pIncrement + 1
            pDrDates("DateFrom") = pCalendar.AddWeeks(FirstFullWeek(Now.Year - vYearsAgo), pWeekNum + pIncrement - 1)
            pDrDates("DateTo") = pCalendar.AddWeeks(FirstFullWeek(Now.Year - vYearsAgo), pWeekNum + pIncrement).AddDays(-1)
            pDrDates("DateRange") = DisplayableDateRange(pDrDates("DateFrom"), pDrDates("DateTo"))

            pDrDates("NowDateFrom") = pCalendar.AddWeeks(FirstFullWeek(Now.Year), pWeekNum + pIncrement - 1)
            pDrDates("NowDateTo") = pCalendar.AddWeeks(FirstFullWeek(Now.Year), pWeekNum + pIncrement).AddDays(-1)
            pDrDates("NowDateRange") = DisplayableDateRange(pDrDates("NowDateFrom"), pDrDates("NowDateTo"))

            pDt = GetDemand(pDrDates("DateFrom"), pDrDates("DateTo"), vExcludeMustCount, vStockID)
            pDt.Columns.Add("DateRange")
            pDt.Columns.Add("NowDateFrom", GetType(Date))
            pDt.Columns.Add("NowDateTo", GetType(Date))
            pDt.Columns.Add("NowDateRange")
            For Each pDr In pDt.Rows
                pDr("DateRange") = pDrDates("DateRange")
                pDr("NowDateFrom") = pDrDates("NowDateFrom")
                pDr("NowDateTo") = pDrDates("NowDateTo")
                pDr("NowDateRange") = pDrDates("NowDateRange")
            Next pDr
            If pDtData Is Nothing Then
                pDtData = pDt
                pDtData.TableName = "Data"
            Else
                pDtData.Merge(pDt)
            End If
            pDtDates.Rows.Add(pDrDates)
        Next pIncrement

        pDtData = CreateCrossTab(pDtData, {
                "StockID",
                "StockNumber",
                "StockDescription"
            },
            {New CrossColumn("{NowDateRange}", "Qty", "LineTotal")})
#If DEBUG Then
        pDtData = GetTopRecords(pDtData.DefaultView, 100).ToTable
#End If
        pDtData.TableName = "Data"

        pDsOut = New DataSet
        pDsOut.Tables.Add(pDtDates)
        pDsOut.Tables.Add(pDtData)

        Return pDsOut

    End Function

    ''' <summary>
    ''' orders (active) and backorders (active and cancelled)
    ''' </summary>
    ''' <param name="vDateFrom"></param>
    ''' <param name="vDateTo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDemand(ByVal vDateFrom As Date, ByVal vDateTo As Date, vExcludeMustCount As Boolean,
        Optional vStockID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        ' DrpCacheStockSalesDaily.TransDate is TransSalesOrder.LastUpdated
        pSql = "SELECT TOP 1000 DrpCacheStockSalesDaily.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & _
            ",SUM(DrpCacheStockSalesDaily.QtyInternal + DrpCacheStockSalesDaily.QtyExternal) AS Qty" &
            ",DatePart(year, DrpCacheStockSalesDaily.TransDate) AS TransYear" &
            " FROM DrpCacheStockSalesDaily" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockSalesDaily.StockID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID"
        If vExcludeMustCount Then
            ' if a stock passes this, it is red flag
            pSql &= " JOIN DrpCacheStockRank ON DrpCacheStockRank.StockID = DrpCacheStockSalesDaily.StockID" &
                " AND LEN(ISNULL(DrpCacheStockRank.MustCountReason, '')) = 0"

        End If
        pSql &= " WHERE DrpCacheStockSalesDaily.TransDate BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date)
        If CStr2(vStockID).Length > 0 Then pSql &= " AND DrpCacheStockSalesDaily.StockID = " & ReadyString(vStockID)
        pSql &= " GROUP BY DrpCacheStockSalesDaily.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",DatePart(year, DrpCacheStockSalesDaily.TransDate)" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '')" &
            " ORDER BY " &
            " DatePart(year, DrpCacheStockSalesDaily.TransDate) DESC" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '')"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetForecast(vWeekStartDate As Date, vWeeksCount As Integer, vExcludeMustCount As Boolean) As DataSet

        Dim pDsDemand As DataSet

        pDsDemand = GetDemand(vWeekStartDate, 1, vWeeksCount, vExcludeMustCount)

        Return pDsDemand

    End Function

    ''' <summary>
    ''' Output fields: StockDescription, StocksOnHand, QtyCommited (wrong spelling),
    ''' OrderQty, BackOrderQty 
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <param name="vBranchID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStatus(vID As String, vBranchID As Integer) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "spGetSKUStatus " & ReadyString(vID) & ", " & vBranchID
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetIncomingStocks(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pDtShipper As DataTable

        pSql = "spGetIncomingStocks " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        pDT.Columns.Add("ActualDepartureDateTime", GetType(Date))
        For Each pDR As DataRow In pDT.Rows
            ' this code need optimization.
            ' get only shipper vessels once per shipper.
            pSql = "SELECT DepartureDay" & _
                ",LeadTime" & _
                ",DepartureHour" & _
                " FROM LibraryShipperVessels " & _
                " WHERE ShipperID = " & ReadyString(pDR("ShipperID"))
            pDtShipper = mProData.Execute(pSql)
            If pDtShipper.Rows.Count > 0 Then
                pDtShipper.Columns.Add("ActualDepartureDateTime", GetType(Date))
                For Each pDrShipper As DataRow In pDtShipper.Rows
                    pDrShipper("ActualDepartureDateTime") = GetNextDate(WeekdayValue(pDrShipper("DepartureDay")), pDrShipper("DepartureHour"), pDR("ActualShipped")).AddHours(pDrShipper("LeadTime"))
                Next pDrShipper
                pDtShipper.DefaultView.Sort = "ActualDepartureDateTime"
                pDR("ActualDepartureDateTime") = pDtShipper.DefaultView(0)("ActualDepartureDateTime")
            Else
                pDR("ActualDepartureDateTime") = pDR("ActualShipped")
            End If
        Next pDR

        Return pDT

    End Function

    Public Sub CacheRanksIfNeeded()

        Dim pDateRange As DateRange

        pDateRange = RankCacheDates()
        If pDateRange Is Nothing Then
            CacheRanks()
        ElseIf Now.Date <> pDateRange.DateTo.Date Then
            CacheRanks()
        Else
            ' dont cache
        End If

    End Sub

    Public Sub CacheRanks(Optional ByVal vIDs As String = Nothing)

        Dim pOut As JsonDictionary
        Dim pDT As DataTable
        Dim pSql As String
        Dim pTotalCogs, pAccumulatedPercentage As Decimal
        Dim pRank As Integer
        Dim pSqlLastApprovedCount As String
        Dim pSqlQtyOnHand As String
        Dim pIU As InsertUpdate
        Dim pDrCycleCountSettings
        Dim pDrStatus As DataRow
        Dim pBranchID As Integer

        pBranchID = mParent.LibraryBranches.CurrentBranchDesignation
        pSql = "SELECT * FROM MainCycleCount"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Throw New ApplicationException("MainCycleCount has no data")
        End If
        pDrCycleCountSettings = pDT.Rows(0)

        pSqlLastApprovedCount = "SELECT MAX(DateAndTimeEnded) AS DateAndTimeEnded" & _
            " FROM TransCycleCountStocks" & _
            " JOIN TransCycleCount" & _
            " ON TransCycleCountStocks.CycleCountID = TransCycleCount.ID" & _
            " WHERE TransCycleCountStocks.StockID = DrpCacheStockSalesDaily.StockID" & vbCrLf & _
            " AND IsCancelled = 0" & _
            " AND ApprovedBy <> 0"

        pSqlQtyOnHand = "SELECT TOP 1 StocksOnHand " & _
            " FROM WholesaleStockInventory " & _
            " WHERE StockID = DrpCacheStockSalesDaily.StockID" & _
            " AND BranchID = " & pBranchID &
            " ORDER BY WholesaleStockInventory.LastUpdated DESC"

        pSql = ""
        If CStr2(vIDs).Length > 0 Then
            pSql = " AND DrpCacheStockSalesDaily.StockID IN (" & vIDs & ")"
        End If
        pSql = "SELECT TOP 100 PERCENT DrpCacheStockSalesDaily.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" & _
            ",SUM(DrpCacheStockSalesDaily.QtyInternal + DrpCacheStockSalesDaily.QtyExternal) AS Qty" &
            ",SUM(DrpCacheStockSalesDaily.COGS) AS COGS" &
            ",(" & pSqlLastApprovedCount & ") AS LastApprovedCount" & vbCrLf &
            ",(" & pSqlQtyOnHand & ") AS QtyOnHand" & vbCrLf &
            " FROM DrpCacheStockSalesDaily" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockSalesDaily.StockID" & _
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & _
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" &
            " WHERE DATEDIFF(day, DrpCacheStockSalesDaily.TransDate, GETDATE()) < 365" &
            pSql &
            " GROUP BY DrpCacheStockSalesDaily.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " & _
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '')" &
            " ORDER BY SUM(DrpCacheStockSalesDaily.COGS) DESC"
        pDT = mProData.Execute(pSql)
        With pDT.Columns
            .Add("Rank", GetType(Integer))
            .Add("RankLetter")
            .Add("PercentageOfTotal", GetType(Decimal))
            .Add("AccumulatedPercentage", GetType(Decimal))
            .Add("Reason")
        End With

        pTotalCogs = CDec2(pDT.Compute("SUM(COGS)", ""))
        If pTotalCogs > 0 Then
            For Each pDR As DataRow In pDT.Rows
                pRank += 1
                pDR("Rank") = pRank
                pDR("PercentageOfTotal") = pDR("COGS") / pTotalCogs
                pAccumulatedPercentage += pDR("PercentageOfTotal")
                pDR("AccumulatedPercentage") = pAccumulatedPercentage

                If pDR("AccumulatedPercentage") * 100 <= pDrCycleCountSettings("AP") Then
                    pDR("RankLetter") = "A"
                ElseIf pDR("AccumulatedPercentage") * 100 <= pDrCycleCountSettings("BP") Then
                    pDR("RankLetter") = "B"
                ElseIf pDR("AccumulatedPercentage") * 100 <= pDrCycleCountSettings("CP") Then
                    pDR("RankLetter") = "C"
                Else
                    pDR("RankLetter") = "D"
                End If

                If CDec2(pDR("QtyOnHand")) < 0 Then
                    pDR("Reason") = "Negative quantity on hand"
                ElseIf pDR("LastApprovedCount") Is DBNull.Value Then
                    pDR("Reason") = "Not yet counted"
                Else
                    If 365 \ pDrCycleCountSettings(pDR("RankLetter") & "CountsPerYear") < DateDiff(DateInterval.Day, pDR("LastApprovedCount"), Now) Then
                        pDR("Reason") = "Too long ago since last counted"
                    End If
                End If
            Next pDR

            pSql = "DELETE FROM DrpCacheStockRank"
            mProData.ExecuteSQL(pSql)

            pIU = New InsertUpdate("DrpCacheStockRank")
            For Each pDR As DataRow In pDT.Rows
                pIU.Reset()
                pIU("ID") = "newid()"
                pIU("LastUpdated") = "GETDATE()"
                pIU("StockID") = ReadyString(pDR("StockID"))
                pIU("QtyOnHand") = CDec2(pDR("QtyOnHand"))
                If pDR("LastApprovedCount") IsNot DBNull.Value Then
                    pIU("LastApprovedCount") = ReadyString(pDR("LastApprovedCount"))
                End If
                If CStr2(pDR("Reason")).Length > 0 Then
                    pIU("MustCountReason") = ReadyString(pDR("Reason"))
                End If
                pIU("RankNum") = pDR("Rank")
                pIU("RankLetter") = ReadyString(pDR("RankLetter"))
                pIU("COGS") = pDR("COGS")
                pIU("PercentageOfTotal") = pDR("PercentageOfTotal")
                pIU("AccumulatedPercentage") = pDR("AccumulatedPercentage")

                pDrStatus = GetStatus(CStr2(pDR("StockID")), pBranchID)
                If pDrStatus Is Nothing Then
                    pIU("QtyCommitted") = "0"
                Else
                    pIU("QtyCommitted") = pDrStatus("QtyCommited") ' QtyCommited = wrong spelling inside spGetSKUStatus
                End If
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            Next pDR
        End If

    End Sub

    Public Function RankCacheDates() As DateRange

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MIN(LastUpdated) AS DateStart" & vbCrLf & _
            " ,MAX(LastUpdated) AS DateEnd" & vbCrLf & _
            " FROM DrpCacheStockRank" & vbCrLf
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 _
            AndAlso IsDate(pDT.Rows(0)("DateStart")) Then

            Return New DateRange(pDT.Rows(0)("DateStart"), pDT.Rows(0)("DateEnd"))
        End If

    End Function
End Class

Public Class CacheDrpTables
    Dim mProData As ProData
    Dim mBusiness As Business
    Dim mForecastHorizon As Integer
    Dim mSafetyStockDuration As Integer

    Public Sub New(vBusiness As Business)

        mBusiness = vBusiness
        mProData = mBusiness.ProData
        GetSettings()

    End Sub

    Private Sub GetSettings()

        mSafetyStockDuration = Math.Max(1, CDec2(mBusiness.SystemDefaults.SafetyStockDuration))
        mForecastHorizon = mBusiness.SystemDefaults.ForecastHorizon
        mForecastHorizon = Math.Max(mForecastHorizon, 1)
        mForecastHorizon = Math.Min(mForecastHorizon, 15) ' 15 for now

    End Sub

    Public Sub CacheDrp()

        CacheDrpSettings()
        CacheDrpStocks()
        EndCache()

    End Sub

    Private Sub DeleteDrpCache()

        Dim pSql As String

        pSql = "DELETE FROM DrpCacheDrpSettings"
        mProData.ExecuteSQL(pSql)

        pSql = "DELETE FROM DrpCacheDrpStocks"
        mProData.ExecuteSQL(pSql)

        pSql = "DELETE FROM DrpCacheDrpTable"
        mProData.ExecuteSQL(pSql)

        pSql = "DELETE FROM DrpCacheDrpStockUnits"
        mProData.ExecuteSQL(pSql)

    End Sub

    Private Sub CacheDrpSettings()

        Dim pIU As InsertUpdate
        Dim pDateRange As DateRange
        Dim pSql As String

        pIU = New InsertUpdate("DrpCacheDrpSettings")
        pIU("ForecastHorizon") = mForecastHorizon
        pIU("SafetyStockDuration") = mBusiness.SystemDefaults.GetOrAddSystemDefault("Safety stock duration", 2)
        pIU("LastUpdated") = "getdate()"
        pIU("PoProcessingLeadTime") = mBusiness.SystemDefaults.PoProcessingLeadTime

        pDateRange = WeekRange(Now)
        For p As Integer = 1 To mForecastHorizon
            pIU("Week" & Format(p, "00") & "Start") = ReadyString(pDateRange.DateFrom.AddDays(7 * (p - 1)))
            pIU("Week" & Format(p, "00") & "End") = ReadyString(pDateRange.DateTo.AddDays(7 * (p - 1)))
        Next p

        DeleteDrpCache()
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub CacheDrpStocks(Optional ByVal vIDs As String = "")

        Dim pDtStockStatus As DataTable
        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pDrShipper As DataRow
        Dim pDtSuppliers As DataTable
        Dim pPoProcessingLeadTime As Integer
        Dim pRowIndex As Integer
        Dim pSafetyStock As Integer

        pPoProcessingLeadTime = mBusiness.SystemDefaults.PoProcessingLeadTime
        pSql = "SELECT StockID" &
            ",QtyOnHand" &
            ",QtyCommitted" &
            " FROM DrpCacheStockRank" &
            " WHERE MustCountReason IS NULL"
        If CStr2(vIDs).Length > 0 Then
            pSql &= " AND StockID IN (" & vIDs & ")"
        End If
        pDtStockStatus = mProData.Execute(pSql)
        pIU = New InsertUpdate("DrpCacheDrpStocks")
        For Each pDR As DataRow In pDtStockStatus.Rows
            Try
                pIU.Reset()
                pIU("StockID") = ReadyString(pDR("StockID"))
                pIU("QtyOnHand") = pDR("QtyOnHand")
                pIU("QtyCommitted") = pDR("QtyCommitted")
                pIU("ShippingLeadTime") = CDec2(mBusiness.SystemDefaults.ShippingLeadTime)
                pDrShipper = mBusiness.LibraryShippers.GetDefaultShipper
                If pDrShipper IsNot Nothing Then
                    pIU("ShipperID") = ReadyString(pDrShipper("ID"))
                End If

                pDtSuppliers = mBusiness.LibraryStock.BestSupplierPrices(ReadyString(pDR("StockID")), True)
                If pDtSuppliers.Rows.Count > 0 Then
                    pIU("SupplierID") = ReadyString(pDtSuppliers.Rows(0)("SupplierID"))
                    pIU("VendorLeadTime") = Format(pDtSuppliers.Rows(0)("LeadTime"), cGlobal.FormatInteger)
                Else
                    Continue For
                End If
                pIU("TotalLeadTime") = CDec2(pIU("VendorLeadTime")) + CDec2(pIU("ShippingLeadTime")) + pPoProcessingLeadTime
                CacheDrpTable(CStr2(pDR("StockID")), pIU("TotalLeadTime"), CStr2(pDtSuppliers.Rows(0)("SupplierID")), pSafetyStock)
                pIU("SafetyStocks") = pSafetyStock

                pSql = pIU.GetUpdate("StockID = " & ReadyString(pDR("StockID")))
                If mProData.ExecuteSQL(pSql) = 0 Then
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                End If

            Catch ex As Exception
                AppendToDebug(ex.Message & " stack trace: " & GetStackTrace(, ex))
            End Try
            pRowIndex += 1
        Next pDR

    End Sub

    Private Sub CacheDrpTable(vStockID As String,
        ByVal vLeadTime As Integer,
        vSupplierID As String,
        ByRef oSafetyStock As Integer)

        Dim pIU As InsertUpdate
        Dim pDS As DataSet
        Dim pDtData As DataTable
        Dim pDtDates As DataTable

        Dim pSql As String
        Dim pDrStatus As DataRow
        Dim pDtIncoming As DataTable
        Dim pPastDue As Boolean = True

        Dim pDrGrossRequirements As DataRow
        Dim pDrForecast As DataRow
        Dim pDrAdvanceSalesOrders As DataRow
        Dim pDrBackOrders As DataRow
        Dim pDrScheduledReceipts As DataRow
        Dim pDrPlannedOrders As DataRow
        Dim pDrProjectedOnHand, pDrQtyAvailableToCommit2 As DataRow
        Dim pWeek1Start As Date
        Dim pDateName As String ' pDtDates.Rows(pWeekNum)("NowDateFrom") 
        '                       ' also the column name for pDtData
        Dim pDateNamePrev As String
        Dim pGrossToAdd As Integer
        Dim pPpoQty As Integer
        Dim pTotalGrossRequirements As Integer  ' get this to get the average (w/c then becomes the safety stock)
        Dim pBackorders As Dictionary(Of String, Integer) ' key NowDateRange

        pTotalGrossRequirements = 0
        pBackorders = New Dictionary(Of String, Integer)
        oSafetyStock = 0
        pDrStatus = mBusiness.LibraryStock.GetStatus(vStockID, mBusiness.LibraryBranches.CurrentBranchDesignation)
        If pDrStatus IsNot Nothing Then
            pTotalGrossRequirements = CDec2(pDrStatus("OrderQty"))
        End If

        pDS = mBusiness.LibraryStock.GetDemand(Now, 1, Math.Max(CDec2(mForecastHorizon), 1), False, vStockID)
        pDtData = pDS.Tables("Data") ' this only contains 1 row
        pDtDates = pDS.Tables("Dates")
        For Each pDrData As DataRow In pDtData.Rows ' 1 row only
            For Each pDrDate As DataRow In pDS.Tables("Dates").Rows
                If pDtData.Columns.Contains(pDrDate("NowDateRange")) Then
                    pTotalGrossRequirements += CDec2(pDrData(pDrDate("NowDateRange")))
                End If
                pBackorders.Add(pDrDate("NowDateRange"), mBusiness.LibraryStock.GetBackOrders(vStockID, pDrDate("NowDateFrom"), pDrDate("NowDateTo")))
                pTotalGrossRequirements += pBackorders(pDrDate("NowDateRange"))
            Next pDrDate
        Next pDrData
        oSafetyStock = Math.Ceiling(Math.Max(0, mSafetyStockDuration * (pTotalGrossRequirements / mForecastHorizon)))

        pWeek1Start = pDtDates.Rows(0)("NowDateFrom") ' this is guaranteed to have at least 1 row
        With pDtData.Columns
            .Add("Text")
            .Add("PastDue", GetType(Integer))
            .Add("Ordinal", GetType(Byte))
            .Remove("StockID")
            .Remove("StockNumber")
            .Remove("StockDescription")
        End With

        pDtIncoming = mBusiness.LibraryStock.GetIncomingStocks(vStockID)

        pDrGrossRequirements = pDtData.NewRow
        pDrGrossRequirements("Text") = "Gross Requirements"
        pDrGrossRequirements("PastDue") = 0
        pDrGrossRequirements("Ordinal") = 1
        pDtData.Rows.Add(pDrGrossRequirements)

        pDrForecast = pDtData.Rows(0)
        pDrForecast("Text") = "    Forecast"
        pDrForecast("PastDue") = 0
        pDrForecast("Ordinal") = 2

        pDrAdvanceSalesOrders = pDtData.NewRow
        pDrAdvanceSalesOrders("Text") = "    Advanced Sales Orders"
        pDrAdvanceSalesOrders("PastDue") = 0
        pDrAdvanceSalesOrders("Ordinal") = 3
        pDtData.Rows.Add(pDrAdvanceSalesOrders)

        pDrBackOrders = pDtData.NewRow
        pDrBackOrders("Text") = "    Backorders"
        pDrBackOrders("PastDue") = 0
        pDrBackOrders("Ordinal") = 4
        pDtData.Rows.Add(pDrBackOrders)

        pDrProjectedOnHand = pDtData.NewRow
        pDrProjectedOnHand("Text") = "Projected Qty on Hand"
        pDrProjectedOnHand("PastDue") = CDec2(pDrStatus("StocksOnHand"))
        pDrProjectedOnHand("Ordinal") = 5
        pDtData.Rows.Add(pDrProjectedOnHand)

        pDrScheduledReceipts = pDtData.NewRow
        pDrScheduledReceipts("Text") = "Scheduled receipts"
        pDrScheduledReceipts("PastDue") = CDec2(pDtIncoming.Compute("SUM(Quantity)", "ActualDepartureDateTime < " & ReadyString(pWeek1Start)))
        pDrScheduledReceipts("Ordinal") = 6
        pDtData.Rows.Add(pDrScheduledReceipts)

        pDrPlannedOrders = pDtData.NewRow
        pDrPlannedOrders("Text") = "Planned Orders"
        pDrPlannedOrders("PastDue") = 0
        pDrPlannedOrders("Ordinal") = 7
        pDtData.Rows.Add(pDrPlannedOrders)

        pDrQtyAvailableToCommit2 = pDtData.NewRow
        pDrQtyAvailableToCommit2("Text") = "Net Projected on Hand"
        pDrQtyAvailableToCommit2("PastDue") = pDrProjectedOnHand("PastDue")
        pDrQtyAvailableToCommit2("Ordinal") = 8
        pDtData.Rows.Add(pDrQtyAvailableToCommit2)

        '// add missing weeks dates (coz some SKU's dont have sales on those weeks
        For pWeekNum As Integer = 0 To pDtDates.Rows.Count - 1
            pDateName = pDtDates.Rows(pWeekNum)("NowDateRange")
            If pDtData.Columns.Contains(pDateName) = False Then
                pDtData.Columns.Add(pDateName, GetType(Integer))
            End If
        Next pWeekNum

        For pWeekNum As Integer = 0 To pDtDates.Rows.Count - 1 ' -1 = past due
            ' week 1 = current week; week 2 = next week
            pDateName = pDtDates.Rows(pWeekNum)("NowDateRange")
            If pWeekNum = 0 Then
                pDateNamePrev = "PastDue"
            Else
                pDateNamePrev = pDtDates.Rows(pWeekNum - 1)("NowDateRange")
            End If
            If pWeekNum = 0 Then
                pDrAdvanceSalesOrders(pDateName) = CDec2(pDrStatus("OrderQty"))
            End If
            If pBackorders.ContainsKey(pDateName) Then
                pDrBackOrders(pDateName) = pBackorders(pDateName)
            Else
                pDrBackOrders(pDateName) = 0
            End If
            pDrGrossRequirements(pDateName) = CDec2(pDrForecast(pDateName)) + CDec2(pDrAdvanceSalesOrders(pDateName)) + CDec2(pDrBackOrders(pDateName))

            pDrProjectedOnHand(pDateName) = CDec2(pDrQtyAvailableToCommit2(pDateNamePrev)) - CDec2(pDrGrossRequirements(pDateName))
            pDrScheduledReceipts(pDateName) = CDec2(pDtIncoming.Compute("SUM(Quantity)",
                "ActualDepartureDateTime >= " & ReadyString(pDtDates.Rows(pWeekNum)("NowDateFrom")) &
                " AND ActualDepartureDateTime <= " & ReadyString(pDtDates.Rows(pWeekNum)("NowDateTo")))) ' todo: add planned orders [t - totalLeadTime]
            If pWeekNum - Math.Ceiling(vLeadTime / 7) = -1 Then
                pDrScheduledReceipts(pDateName) += CDec2(pDrPlannedOrders("PastDue"))
            ElseIf Math.Ceiling(vLeadTime / 7) <= pWeekNum Then
                pDrScheduledReceipts(pDateName) += CDec2(pDrPlannedOrders(pDtDates.Rows(pWeekNum - Math.Ceiling(vLeadTime / 7))("NowDateRange")))
            End If
            pGrossToAdd = 0
            ' mSafetyStockDuration = change to Order Qty
            For p As Integer = Math.Min(pWeekNum + 1, pDtDates.Rows.Count - 1) To Math.Min(pWeekNum + mSafetyStockDuration, pDtDates.Rows.Count - 1)
                pGrossToAdd += CDec2(pDrGrossRequirements(pDtDates.Rows(p)("NowDateRange")))
            Next p
            pPpoQty = Math.Max(0, oSafetyStock - pDrProjectedOnHand(pDateName) - pDrScheduledReceipts(pDateName) + pGrossToAdd)
            pPpoQty = CacheStockUnits(vSupplierID, vStockID, pPpoQty, pWeekNum)
            pDrPlannedOrders(pDateNamePrev) = pPpoQty
            pDrQtyAvailableToCommit2(pDateName) = pDrProjectedOnHand(pDateName) + pDrScheduledReceipts(pDateName)
        Next pWeekNum

        ' redundant
        pSql = "DELETE FROM DrpCacheDrpTable WHERE StockID = " & ReadyString(vStockID)
        mProData.ExecuteSQL(pSql)

        pIU = New InsertUpdate("DrpCacheDrpTable")
        pIU("StockID") = ReadyString(vStockID)
        For Each pDR As DataRow In pDtData.Rows
            pIU("Text") = ReadyString(pDR("Text"))
            pIU("PastDue") = CDec2(pDR("PastDue"))
            For pWeekNum As Integer = 0 To pDtDates.Rows.Count - 1 ' -1 = past due
                pIU("Week" & Format(pWeekNum + 1, "00")) = CDec2(pDR(pDtDates.Rows(pWeekNum)("NowDateRange")))
            Next pWeekNum
            pIU("Ordinal") = pDR("Ordinal")
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Sub

    ''' <summary>
    ''' returns the qty after considering Lot Size & MOQ
    ''' </summary>
    ''' <param name="vSupplierID"></param>
    ''' <param name="vQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CacheStockUnits(vSupplierID As String, vStockID As String, vQty As Integer, vWeekNum As Integer) As Integer

        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pOut As Integer ' actual ordered qty (in base)
        Dim pQty As Integer ' qty saved so far
        Dim pDT As DataTable
        Dim pPrice As Decimal
        Dim pLotSize As Integer
        Dim pLotSizeUnit As String
        Dim pLotSizeUnitQty As Integer
        Dim pMoq As Integer
        Dim pMoqUnit As String
        Dim pMoqUnitQty
        Dim pDtSupplierPrices As DataTable
        Dim pLotMultiplier As Integer

        ' redundant
        pSql = "DELETE FROM DrpCacheDrpStockUnits" &
            " WHERE StockID = " & ReadyString(vStockID) &
            " AND SupplierID = " & ReadyString(vSupplierID) &
            " AND WeekNum = " & vWeekNum
        mProData.ExecuteSQL(pSql)
        If vQty = 0 Then Return 0

        pDT = mBusiness.LibrarySuppliers.StockPrice(vSupplierID, vStockID)
        If pDT.Rows.Count = 0 Then
            Return 0
        End If
        pPrice = CDec2(pDT.Rows(0)("Price")) / Math.Max(1, CDec2(pDT.Rows(0)("Quantity")))

        pIU = New InsertUpdate("DrpCacheDrpStockUnits")
        pIU("StockID") = ReadyString(vStockID)
        pIU("SupplierID") = ReadyString(vSupplierID)
        pIU("WeekNum") = vWeekNum

        mBusiness.LibrarySuppliers.GetStockParams( _
            vID:=vSupplierID,
            vStockID:=vStockID,
            oLotSize:=pLotSize,
            oLotSizeUnit:=pLotSizeUnit,
            oLotSizeUnitQty:=pLotSizeUnitQty,
            oMoq:=pMoq,
            oMoqUnit:=pMoqUnit, oMoqUnitQty:=pMoqUnitQty,
            oDtSupplierPrices:=pDtSupplierPrices)

        pLotMultiplier = Math.Ceiling((vQty - pMoq * pMoqUnitQty) / (pLotSize * pLotSizeUnitQty))
        pLotMultiplier = Math.Max(0, pLotMultiplier)
        If pLotMultiplier > 0 Then
            If pMoqUnitQty = pLotSizeUnitQty Then
                pDtSupplierPrices.DefaultView.Sort = "ActualQuantity DESC"
                pDtSupplierPrices.DefaultView.RowFilter = "ActualQuantity >= " & pMoqUnitQty
                If pDtSupplierPrices.DefaultView.Count = 0 Then
                    pDtSupplierPrices.DefaultView.RowFilter = ""
                End If
                pIU("Price") = pDtSupplierPrices.DefaultView(0)("Price") * (pMoqUnitQty / pDtSupplierPrices.DefaultView(0)("ActualQuantity"))
                pIU("Qty") = pMoq + pLotMultiplier * pLotSize
                pIU("UnitQty") = pMoqUnitQty
                pIU("Unit") = ReadyString(pMoqUnit)
                pIU("SupplierPriceDate") = ReadyString(pDtSupplierPrices.DefaultView(0)("LastUpdated"))
                pIU("UpdatedByName") = ReadyString(pDtSupplierPrices.DefaultView(0)("UpdatedByName"))
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            Else
                pDtSupplierPrices.DefaultView.Sort = "ActualQuantity DESC"
                pDtSupplierPrices.DefaultView.RowFilter = "ActualQuantity >= " & pMoqUnitQty
                If pDtSupplierPrices.DefaultView.Count = 0 Then
                    pDtSupplierPrices.DefaultView.RowFilter = ""
                End If
                pIU("Price") = pDtSupplierPrices.DefaultView(0)("Price") * (pMoqUnitQty / pDtSupplierPrices.DefaultView(0)("ActualQuantity"))
                pIU("Qty") = pMoq
                pIU("UnitQty") = pMoqUnitQty
                pIU("Unit") = ReadyString(pMoqUnit)
                pIU("SupplierPriceDate") = ReadyString(pDtSupplierPrices.DefaultView(0)("LastUpdated"))
                pIU("UpdatedByName") = ReadyString(pDtSupplierPrices.DefaultView(0)("UpdatedByName"))
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)

                pDtSupplierPrices.DefaultView.RowFilter = "ActualQuantity >= " & pLotSizeUnitQty
                If pDtSupplierPrices.DefaultView.Count = 0 Then
                    pDtSupplierPrices.DefaultView.RowFilter = ""
                End If
                pIU("Price") = pDtSupplierPrices.DefaultView(0)("Price") * (pLotSizeUnitQty / pDtSupplierPrices.DefaultView(0)("ActualQuantity"))
                pIU("UnitQty") = pLotSizeUnitQty
                pIU("Qty") = pLotMultiplier * pLotSize
                pIU("Unit") = ReadyString(pLotSizeUnit)
                pIU("SupplierPriceDate") = ReadyString(pDtSupplierPrices.DefaultView(0)("LastUpdated"))
                pIU("UpdatedByName") = ReadyString(pDtSupplierPrices.DefaultView(0)("UpdatedByName"))
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            End If
        Else
            pDtSupplierPrices.DefaultView.Sort = "ActualQuantity DESC"
            pDtSupplierPrices.DefaultView.RowFilter = "ActualQuantity >= " & pMoqUnitQty
            If pDtSupplierPrices.DefaultView.Count = 0 Then
                pDtSupplierPrices.DefaultView.RowFilter = ""
            End If
            pIU("Price") = pDtSupplierPrices.DefaultView(0)("Price") * (pMoqUnitQty / pDtSupplierPrices.DefaultView(0)("ActualQuantity"))
            pIU("Qty") = pMoq
            pIU("UnitQty") = pMoqUnitQty
            pIU("Unit") = ReadyString(pMoqUnit)
            pIU("SupplierPriceDate") = ReadyString(pDtSupplierPrices.DefaultView(0)("LastUpdated"))
            pIU("UpdatedByName") = ReadyString(pDtSupplierPrices.DefaultView(0)("UpdatedByName"))
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        End If
        pOut = pMoq * pMoqUnitQty + pLotMultiplier * pLotSize * pLotSizeUnitQty

        Return pOut

    End Function

    Private Sub EndCache()

        Dim pSql As String

        pSql = "UPDATE DrpCacheDrpSettings" &
            " SET EndUpdate = GETDATE()"
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Enum eCacheStatus
        Old
        Valid
        Ongoing
    End Enum

    Public Function Status() As eCacheStatus

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LastUpdated" &
            ",EndUpdate" &
            " FROM DrpCacheDrpSettings"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 0 Then
            Return eCacheStatus.Old
        Else
            If pDT.Rows(0)("EndUpdate") Is DBNull.Value AndAlso DateDiff(DateInterval.Minute, pDT.Rows(0)("LastUpdated"), Now) < 30 Then
                Return eCacheStatus.Ongoing
            End If
            If pDT.Rows(0)("EndUpdate") Is DBNull.Value OrElse CDate(pDT.Rows(0)("LastUpdated")).Date <> Now.Date Then
                Return eCacheStatus.Old
            End If
            Return eCacheStatus.Valid
        End If

    End Function
End Class
