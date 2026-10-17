Imports System.Linq

Public Class LibraryStock

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

    Public Sub GetProfitability(ByVal vStockID As String, ByVal vQty As Long, ByVal vUnitQty As Long,
        ByVal vUnitPrice As Decimal,
        ByRef oUnitGP As Decimal, ByRef oUnitGPM As Double, ByRef oLineGP As Decimal, ByRef oLineGPM As Double,
        ByRef oLandedCost As Decimal, ByVal vHistoricCost As Decimal,
        ByVal vOrgID As String)

        Dim pCost As Decimal

        oLandedCost = GetLandedCost(vStockID, vOrgID) '* vUnitQty
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
            If vUnitPrice <> 0 Then
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

    Public Function GetRetailPrice(ByVal vStockID As String, vOrgID As String, Optional ByRef oLastUpdated As Date = Nothing) As Decimal

        Return GetPrice(vStockID, "Retail Price ID", "{01000012-0005-0000-0000-000000000000}", oLastUpdated, vOrgID)

    End Function

    Public Function GetSrp(ByVal vStockID As String, vOrgID As String, Optional ByRef oLastUpdated As Date = Nothing) As Decimal

        Return GetPrice(vStockID, "SRP (Suggested Retail Price) ID", "{EEF716C6-B290-4B28-B2A6-0A4F52CF4989}", oLastUpdated, vOrgID)

    End Function

    Public Function GetLandedCost(ByVal vStockID As String, vOrgID As String, Optional ByRef oLastUpdated As Date? = Nothing) As Decimal

        Return GetPrice(vStockID, "Landed Cost ID", "{01000012-0003-0000-0000-000000000000}", oLastUpdated, vOrgID)

    End Function

    Public Function GetFactoryPrice(ByVal vStockID As String, vOrgID As String, Optional ByRef oLastUpdated As Date = Nothing) As Decimal

        Return GetPrice(vStockID, "Factory Price ID", "{01000012-0002-0000-0000-000000000000}", oLastUpdated, vOrgID)

    End Function

    Public Function GetFreightCost(ByVal vStockID As String, vOrgID As String, Optional ByRef oLastUpdated As Date = Nothing) As Decimal

        Return GetPrice(vStockID, "Freight Cost ID", "{01000012-0001-0000-0000-000000000000}", oLastUpdated, vOrgID)

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
        ByRef oLastUpdated As Date?,
        vOrgID As String) As Decimal

        Dim pSql As String
        Dim pDT As DataTable

        pSql = mParent.SystemDefaults.GetOrAddSystemDefault(vPriceLevelText, vPriceLevelID)
        If IsGuid(pSql) = False Then pSql = vPriceLevelID

        pSql = "SELECT ISNULL(Price,0) AS Price " &
            ",LastUpdated" &
            " FROM MobizLibraryPIMProductPriceLevel " &
            " WHERE PriceLevelID = " & ReadyString(pSql) &
            " AND ProductID = " & ReadyString(vStockID) &
            " AND OrganizationID = " & ReadyString(vOrgID)
        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            If pDT.Rows.Count > 1 Then
                'Stop2() ' record count should only be 1
            End If
            oLastUpdated = pDT.Rows(0)("LastUpdated")
            Return pDT.Rows(0)("Price")
        Else
            Return -1
        End If

    End Function

    Public Function GetPrice(ByVal vID As String,
        ByVal vUnitQty As Integer,
        ByVal vPriceLevelID As String) As Decimal

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim pGuid As String
        Dim pPrice As Decimal

        pSql = "SELECT Price" &
            ",DeactivateDiscount" &
            " FROM MobizLibraryPIMProductPriceLevel" &
            " WHERE ProductID = " & ReadyString(vID) &
            " AND PriceLevelID = " & ReadyString(vPriceLevelID) &
            " AND Price > 0"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pDR = pDT.Rows(0)
            If CDec2(pDR("DeactivateDiscount")) = 0 Then
                pPrice = pDR("Price")
                pSql = "SELECT Discount" &
                    " FROM MobizLibraryPIMProductMeasurement" &
                    " WHERE ProductID = " & ReadyString(vID) &
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

        pSql = "SELECT COUNT(*) AS Count" &
            ",StockNumber" &
            " FROM LibraryStockInformation" &
            " WHERE IsActive = 1" &
            " GROUP BY StockNumber" &
            " HAVING COUNT(*) > 1"
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            If CStr(pDR("StockNumber")).Length >= 3 Then
                pNumber = 0
                For p = 2 To pDR("Count")
                    pStockNumber = NextAvailableNumber(pDR("StockNumber"), pNumber)
                    If pStockNumber.Length > 0 Then
                        pSql = "SELECT TOP 1 ID" &
                            " FROM LibraryStockInformation" &
                            " WHERE StockNumber = " & ReadyString(pDR("StockNumber")) &
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
            pSql = "SELECT ID" &
                " FROM LibraryStockInformation" &
                " WHERE StockNumber = " & ReadyString(pPrefix & Format(p, "000"))
            pDT = mProData.Execute(pSql)
            If pDT.Rows.Count = 0 Then
                oNumber = p
                Return pPrefix & Format(p, "000")
            End If
        Loop

    End Function

    'Public Function GetBranchCost(ByVal vID As String, ByRef oPriceLevelID As String, _
    '        ByRef rActualQuantity As Integer, _
    '        ByRef rUnitMeasure As String) As Decimal

    '    Dim pDT As DataTable
    '    Dim pPriceLevelID As String
    '    Dim pStr As String

    '    pDT = PriceTable(vID)

    '    '// get the available unit quantity and unit measurement
    '    pDT.DefaultView.RowFilter = "Quantity = " & rActualQuantity
    '    If pDT.DefaultView.Count > 0 Then
    '        rUnitMeasure = pDT.DefaultView(0)("UnitMeasure")
    '    Else
    '        pDT.DefaultView.RowFilter = "UnitMeasure = " & ReadyString(rUnitMeasure)
    '        If pDT.DefaultView.Count > 0 Then
    '            rActualQuantity = pDT.DefaultView(0)("Quantity")
    '        Else
    '            pDT.DefaultView.RowFilter = "Quantity = 1"
    '            If pDT.DefaultView.Count > 0 Then
    '                rActualQuantity = 1
    '                rActualQuantity = pDT.DefaultView(0)("Quantity")
    '            Else
    '                rActualQuantity = 0
    '                rUnitMeasure = ""
    '                ' no hope
    '                Return 0
    '            End If
    '        End If
    '    End If

    '    pPriceLevelID = "{01000012-0006-0000-0000-000000000000}"
    '    pDT.DefaultView.RowFilter = "PriceLevelID = " & ReadyString(ReadyGuid(pPriceLevelID)) & _
    '        " AND Quantity = " & rActualQuantity & _
    '        " AND DiscountedPrice > 0"
    '    If pDT.DefaultView.Count > 0 Then
    '        oPriceLevelID = pPriceLevelID
    '        Return pDT.DefaultView(0)("DiscountedPrice")
    '    End If

    '    pPriceLevelID = mParent.SystemDefaults.GetWholesalePriceID
    '    pDT.DefaultView.RowFilter = "PriceLevelID = " & ReadyString(ReadyGuid(pPriceLevelID)) & _
    '        " AND Quantity = " & rActualQuantity & _
    '        " AND DiscountedPrice > 0"
    '    If pDT.DefaultView.Count > 0 Then
    '        oPriceLevelID = pPriceLevelID
    '        Return pDT.DefaultView(0)("DiscountedPrice")
    '    End If

    '    pPriceLevelID = mParent.SystemDefaults.GetRetailPriceID
    '    pDT.DefaultView.RowFilter = "PriceLevelID = " & ReadyString(ReadyGuid(pPriceLevelID)) & _
    '        " AND Quantity = " & rActualQuantity & _
    '        " AND DiscountedPrice > 0"
    '    If pDT.DefaultView.Count > 0 Then
    '        oPriceLevelID = pPriceLevelID
    '        Return pDT.DefaultView(0)("DiscountedPrice")
    '    End If

    'End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vId"></param>
    ''' <param name="vCustomerID"></param>
    ''' <param name="vLowestPriceOnly">display only the lowest priced price level per unit qty</param>
    ''' <param name="vIncludeLastPriceRequest">If true, includes 2 additional columns: CustomerLastInvoicePrice, CustomerLastInvoiceDate</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PriceTable(vOrgID As String, ByVal vId As String, Optional ByVal vCustomerID As String = Nothing,
        Optional vLowestPriceOnly As Boolean = False,
        Optional vRetailPrice As Boolean = False,
        Optional vIncludeBaseUnit As Boolean = False,
        Optional vIncludeLastPriceRequest As Boolean = False,
        Optional vUseDefaultCustomerIfNoResult As Boolean = False,
        Optional vDefaultPriceLevelID As String = Nothing,
        Optional vDefaultPriceLevelIDs As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pWhere As String
        Dim pFieldBaseUnit As String
        Dim pSqlBaseUnit As String
        Dim pDefaultCustomerID As String

#If DEBUG Then
        Dim pSW As New Diagnostics.Stopwatch
        pSW.Start()
#End If

        If vRetailPrice Then
            pWhere = " AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(mParent.SystemDefaults.GetRetailPriceID) & vbCrLf
        End If
        If vIncludeBaseUnit Then
            pFieldBaseUnit = ",tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
                ",tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf
            pSqlBaseUnit = " CROSS APPLY (" & vbCrLf &
                " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                " ) tblBaseUnit" & vbCrLf
        End If

        pSql = " SELECT DISTINCT TOP 100 PERCENT " & vbCrLf &
        "  MobizLibraryPIMProductPriceLevel.PriceLevelID" & vbCrLf &
        " ,tblUnits.MeasurementLevel" & vbCrLf &
        ",tblUnits.Label AS UnitMeasure" & vbCrLf &
        " ,tblUnits.Quantity" & vbCrLf &
        " ,MobizLibraryPIMPriceLevel.PriceLevel" & vbCrLf &
        " ,~MobizLibraryPIMProductPriceLevel.DeactivateDiscount * tblUnits.Discount AS Discount" & vbCrLf &
        " ,CONVERT(Money, tblUnits.Quantity * MobizLibraryPIMProductPriceLevel.Price) AS OriginalPrice" & vbCrLf &
        " ,tblUnits.Quantity * MobizLibraryPIMProductPriceLevel.Price - (tblUnits.Quantity * MobizLibraryPIMProductPriceLevel.Price) * (tblUnits.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount AS DiscountedPrice" & vbCrLf &
        " ,MobizLibraryPIMProductPriceLevel.Price - MobizLibraryPIMProductPriceLevel.Price * (tblUnits.Discount / 100) * ~MobizLibraryPIMProductPriceLevel.DeactivateDiscount AS DiscountedPricePerUnit" & vbCrLf &
        " ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
        " ,tblUnits.Discount AS MeasurementDiscount" & vbCrLf &
        " ,MobizLibraryPIMProductPriceLevel.DeactivateDiscount, tblUnits.UnitMeasureID" & vbCrLf &
        " ,tblSrp.Price * tblUnits.Quantity as SRP" & vbCrLf

        If vIncludeLastPriceRequest Then
            pSql &= ",tblLastInvoice.UnitPrice AS CustomerLastInvoicePrice" & vbCrLf &
                ",tblLastInvoice.InvoiceDate AS CustomerLastInvoiceDate" & vbCrLf
        End If
        pSql &= pFieldBaseUnit

        pSql &= " FROM  MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "  JOIN (" & vbCrLf &
            "  	SELECT MobizLibraryPIMProductMeasurement.MeasurementLevel" & vbCrLf &
            "     ,MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            "     ,MobizLibraryCountriesLanguageLabel.Label" & vbCrLf &
            "     ,MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
            "     ,MobizLibraryPIMProductMeasurement.Discount" & vbCrLf &
            "     ,MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            "  	JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "     UNION" & vbCrLf &
            "     SELECT 99999 AS MeasurementLevel" & vbCrLf &
            "     ,LibraryEcommerceProductQtyChoices.QtyChoices" & vbCrLf &
            "     ,LibraryEcommerceProductQtyChoices.Alias AS Label" & vbCrLf &
            "     ,MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
            "     ,0 AS Discount" & vbCrLf &
            "     ,NULL AS UnitMeasureID" & vbCrLf &
            "     FROM LibraryEcommerceProductQtyChoices" & vbCrLf &
            "     JOIN LibraryEcommerceProductQtyChoicesCategories ON LibraryEcommerceProductQtyChoicesCategories.QtyChoiceGroupID = LibraryEcommerceProductQtyChoices.GroupID" & vbCrLf &
            "     JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.C0020 = LibraryEcommerceProductQtyChoicesCategories.Category" & vbCrLf &
            "  ) tblUnits ON tblUnits.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
            "  JOIN MobizLibraryPIMPriceLevel ON MobizLibraryPIMProductPriceLevel.PriceLevelID = MobizLibraryPIMPriceLevel.ID" & vbCrLf

        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = 'eef716c6-b290-4b28-b2a6-0a4f52cf4989' " & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.ProductID = " & ReadyString(vId) & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
            "  ) tblSrp" & vbCrLf

        pSql &= pSqlBaseUnit
        If CStr2(vCustomerID) <> "" Then
            pSql &= " JOIN LibraryOrganizationPriceLevel ON (LibraryOrganizationPriceLevel.PriceLevelID = MobizLibraryPIMPriceLevel.ID" & vbCrLf &
                " AND LibraryOrganizationPriceLevel.OrganizationID = " & ReadyString(vCustomerID) & ")" & vbCrLf
            If String.IsNullOrWhiteSpace(vDefaultPriceLevelID) = False Then
                pSql &= " OR MobizLibraryPIMPriceLevel.ID = " & ReadyString(vDefaultPriceLevelID) & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(vDefaultPriceLevelIDs) = False Then
                pSql &= " OR MobizLibraryPIMPriceLevel.ID IN (" & vDefaultPriceLevelIDs & ")" & vbCrLf
            End If
            ' no need to filter customer in private database
            ' " AND LibraryCustomersPriceLevel.CustomerID = " & ReadyString(vCustomerID) & vbCrLf
        End If
        If vIncludeLastPriceRequest Then
            Throw New NotImplementedException
            pSql &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT TOP 1 TransInvoice.InvoiceDate" & vbCrLf &
                "     ,TransInvoiceItems.UnitPrice" & vbCrLf &
                "     FROM TransInvoice" & vbCrLf &
                "     JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                "     WHERE TransInvoiceItems.StockID = MobizLibraryPIMProductPriceLevel.StockID" & vbCrLf &
                "     AND TransInvoice.CustomerID = " & ReadyString(vCustomerID) & vbCrLf &
                "     AND TransInvoiceItems.ActualQuantity = MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                "     AND TransInvoice.IsCancelled = 0" & vbCrLf &
                "     ORDER BY TransInvoice.InvoiceDate DESC" & vbCrLf &
                "     ,TransInvoice.LastUpdated DESC" & vbCrLf &
                " ) tblLastInvoice" & vbCrLf

        End If
        pSql &= " WHERE tblUnits.ProductID = " & ReadyString(vId) & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.PriceLevelID <> " & ReadyString(mParent.SystemDefaults.GetOrAddSystemDefault("Freight Cost ID", "{01000012-0001-0000-0000-000000000000}")) & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.PriceLevelID <> " & ReadyString(mParent.SystemDefaults.GetOrAddSystemDefault("Factory Price ID", "{01000012-0002-0000-0000-000000000000}")) & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.PriceLevelID <> " & ReadyString(mParent.SystemDefaults.GetOrAddSystemDefault("Landed Cost ID", "{01000012-0003-0000-0000-000000000000}")) & vbCrLf &
            " AND MobizLibraryPIMPriceLevel.IsActive = 1" & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            pWhere &
            " ORDER BY MobizLibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            ",tblUnits.MeasurementLevel" & vbCrLf

        If vLowestPriceOnly Then
            pSql = "WITH CTE AS (" & vbCrLf &
                pSql & vbCrLf &
                ")" & vbCrLf
            pSql = pSql & " SELECT CTE2.*" & vbCrLf &
                " FROM " & vbCrLf &
                " 	(SELECT CTE.Quantity, MIN(CTE.DiscountedPrice) AS DiscountedPrice" & vbCrLf &
                " 	FROM CTE" & vbCrLf &
                " 	GROUP BY CTE.Quantity" & vbCrLf &
                " ) CTE" & vbCrLf &
                " CROSS APPLY" & vbCrLf &
                " 	(SELECT TOP 1 * " & vbCrLf &
                "     FROM CTE CTE2" & vbCrLf &
                "     WHERE CTE2.Quantity = CTE.Quantity" & vbCrLf &
                "     AND CTE2.DiscountedPrice = CTE.DiscountedPrice    " & vbCrLf &
                "     ORDER BY CTE2.DiscountedPrice" & vbCrLf &
                "     ,CTE2.LastUpdated DESC" & vbCrLf &
                " ) CTE2" & vbCrLf
        End If

        pDT = mProData.Execute(pSql)
#If DEBUG Then
        Debug.Print("PriceTable: " & pSW.ElapsedMilliseconds & " ms")
#End If

        If vUseDefaultCustomerIfNoResult _
            AndAlso CStr2(vCustomerID) <> "" Then

            ' ScreenSteps 2015_09_03__13_59_36_8290
            ' if no price from the specified customer, use the default customer
            ' Default customer = GetSystemDefault("Customer Organization ID")
            '                  = WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID
            If pDT.Rows.Count = 0 Then
                pDefaultCustomerID = WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID
                If IsGuid(pDefaultCustomerID) Then
                    If ReadyGuid(pDefaultCustomerID) <> ReadyGuid(vCustomerID) Then
                        pDT = PriceTable(vOrgID:=vOrgID,
                            vId:=vId,
                            vCustomerID:=pDefaultCustomerID,
                            vLowestPriceOnly:=vLowestPriceOnly,
                            vRetailPrice:=vRetailPrice,
                            vIncludeBaseUnit:=vIncludeBaseUnit,
                            vIncludeLastPriceRequest:=vIncludeLastPriceRequest,
                            vUseDefaultCustomerIfNoResult:=False
                        )
                    End If
                End If
            End If
        End If

        Return pDT

    End Function

    Public Function Measurements(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

#If DEBUG Then
        Dim pSW As New Diagnostics.Stopwatch
        pSW.Start()
#End If
        pSql = "SELECT LibraryStockMeasurements.MeasurementLevel," &
            "LibraryStockMeasurements.Barcode," &
            "LibraryUnitMeasures.UnitMeasure," &
            "LibraryStockMeasurements.Quantity," &
            "LibraryStockMeasurements.Dimension," &
            "LibraryStockMeasurements.Weight " &
            ",LibraryStockMeasurements.Quantity"
        pSql &= " FROM LibraryStockMeasurements" &
            " JOIN LibraryUnitMeasures ON LibraryStockMeasurements.UnitMeasureID = LibraryUnitMeasures.ID"
        pSql &= " WHERE LibraryStockMeasurements.StockID = " & ReadyString(vID) &
            " ORDER BY LibraryStockMeasurements.Quantity"

        pDT = mProData.Execute(pSql)
#If DEBUG Then
        Debug.Print("Measurements: " & pSW.ElapsedMilliseconds & " ms")
#End If

        Return pDT

    End Function

    Public Function ByNumber(vProductNum As String, Optional vOrgID As String = Nothing) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        If vOrgID Is Nothing Then
            vOrgID = WebRequest.Instance.OrgID
        End If
        pSql = "SELECT ID" & vbCrLf &
            ",UploadedByID" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " WHERE ProductNo = " & ReadyString(vProductNum) & vbCrLf &
            " AND UploadedByID = " & ReadyString(vOrgID)

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 1 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function ByNumberAndName(vProductNum As String, vProductName As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String
        Dim pDV As DataView

        pSql = "SELECT ID" & vbCrLf &
            ",ProductNo" & vbCrLf &
            ",ProductName" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " WHERE ProductNo = " & ReadyString(vProductNum)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 1 Then
            Return pDT.Rows(0)
        End If
        If pDT.Rows.Count > 1 Then
            pDV = New DataView(pDT, "ProductName = " & ReadyString(vProductName), "", DataViewRowState.CurrentRows)
            If pDV.Count > 0 Then
                Return pDV(0).Row
            End If
        End If

    End Function

    Public Function ByNumberOrName(vProductNum As String, vProductName As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String
        Dim pDV As DataView

        pSql = "SELECT ID" & vbCrLf &
            ",ProductNo" & vbCrLf &
            ",ProductName" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " WHERE ProductNo = " & ReadyString(vProductNum) & vbCrLf &
            " OR ProductName = " & ReadyString(vProductName)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count = 1 Then
            Return pDT.Rows(0)
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vIds"></param>
    ''' <param name="vDtPriceLevels">ID = LibraryPriceLevels.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ByIds(ByVal vIds As String,
        Optional ByVal vDtPriceLevels As DataTable = Nothing,
        Optional vIncludeBaseUnit As Boolean = False,
        Optional vIncludeBom As Boolean = False) As DataTable

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT MobizLibraryPIMProduct.ID" &
            ",MobizLibraryPIMProduct.ProductNo" &
            ",MobizLibraryPIMProduct.ProductName" & vbCrLf
        If vDtPriceLevels IsNot Nothing Then
            pSql &= ",LibraryBrands.BrandName" &
                ",LibraryProducts.ProductName" &
                ",LibraryDepartments.DepartmentName"
            For Each pDR In vDtPriceLevels.Select("ISNULL(ID, '') <> ''")
                pSql &= ",(SELECT TOP 1 Price" &
                    " FROM LibraryPIMProductPriceLevel" &
                    " WHERE LibraryPIMProductPriceLevel.StockID = LibraryStockInformation.ID" &
                    " AND LibraryStockPriceLevels.PriceLevelID = " & ReadyString(pDR("ID")) &
                    " ORDER BY LastUpdated DESC, Price DESC) AS [" & CStr2(pDR("ID")) & "]"
            Next pDR
        End If
        If vIncludeBaseUnit Then
            pSql &= " ,tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
                " ,tblBaseUnit.UnitMeasureID AS BaseUnitMeasureID" & vbCrLf &
                " ,tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf &
                " ,tblBaseUnit.ContentQty" & vbCrLf &
                " ,tblBaseUnit.ContentUnit" & vbCrLf
        End If
        If vIncludeBom Then
            ' vbjs9CNX7NNK4F! do not use ISNULL. let null be null. js code depends on false, true, and null values
            pSql &= ",MobizLibraryPIMProductParent.Jit" & vbCrLf &
                ",ISNULL(MobizLibraryPIMProductParent.Instructions, MobizLibraryPIMProduct.BomInstruction) AS BomInstruction" & vbCrLf &
                ",CONVERT(bit, CASE WHEN MobizLibraryPIMProductParent.ID IS NULL THEN 1 ELSE 0 END) AS HasSubcomponent"
        End If
        pSql &= " FROM MobizLibraryPIMProduct"
        If vIncludeBaseUnit Then
            pSql &= " OUTER APPLY (" & vbCrLf &
                "  SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                "  	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                "   ,MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                "   ,MobizLibraryPIMProductMeasurement.ContentQty" & vbCrLf &
                "   ,MobizLibraryPIMProductMeasurement.ContentUnit" & vbCrLf &
                "      FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "      JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                "      JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "      WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "      ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                "  ) tblBaseUnit" & vbCrLf
        End If
        If vDtPriceLevels IsNot Nothing Then
            pSql &= " LEFT JOIN LibraryDepartments ON LibraryDepartments.ID = LibraryStockInformation.DepartmentID" & vbCrLf
        End If
        If vIncludeBom Then
            pSql &= " LEFT JOIN MobizLibraryPIMProductParent ON MobizLibraryPIMProductParent.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " AND MobizLibraryPIMProductParent.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf
        End If
        pSql &= " WHERE MobizLibraryPIMProduct.ID IN (" & vIds & ")"

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

        pSql = "SELECT Price" &
            ",UserID" &
            ",LastUpdated" &
            " FROM LibraryStockPriceLevels" &
            " WHERE StockID = " & ReadyString(vStockID) &
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

    Public Function LatestHistoricPrice(ByVal vStockID As String, ByVal vPriceLevelID As String,
        Optional ByVal vExcludePrice As Double = 0) As Double

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 2 Price" &
            " FROM LibraryStockPriceHistory" &
            " WHERE StockID = " & ReadyString(vStockID) &
            " AND PriceLevelID = " & ReadyString(vPriceLevelID) &
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

        pSql = "SELECT ISNULL(SUM(Price), 0) FROM (" &
            " SELECT " &
            "   (SELECT TOP 1 Price * LibraryStockKits.Quantity * LibraryStockKits.ActualQuatity" &
            "    FROM LibraryStockPriceLevels" &
            "    WHERE LibraryStockPriceLevels.StockID = LibraryStockKits.StockKitID" &
            "    AND LibraryStockPriceLevels.PriceLevelID = " & ReadyString(vPriceLevelID) &
            "    ORDER BY LibraryStockPriceLevels.LastUpdated DESC) AS Price" &
            " FROM LibraryStockKits" &
            " WHERE StockID = " & ReadyString(vStockID) &
            " ) t"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)(0)
        End If

    End Function

    Public Function GetParentStocks(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT StockID" &
            " FROM LibraryStockKits" &
            " WHERE StockKitID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function AddToTotalStockOut(ByVal vID As String, ByVal vQty As Integer,
        ByRef oError As String,
        Optional ByVal vBranchID As Integer? = Nothing) As Boolean

        Dim pSql As String

        oError = Nothing
        If vBranchID Is Nothing Then
            vBranchID = mParent.LibraryBranches.CurrentBranchDesignation
        End If

        pSql = "UPDATE WholesaleStockInventory" &
            " SET TotalStocksOut = TotalStocksOut + " & vQty &
            ",LastUpdated = Getdate() " &
            " WHERE StockID = " & ReadyString(vID) &
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

    Public Sub SaveIntoInventoryLedger(ByVal vStockID As String,
        ByVal vTransaction As String,
        ByVal vAmount As Double,
        ByVal vDebit As Long,
        ByVal vCredit As Long,
        ByVal vTransactionType As eLedgerTransactionType,
        ByVal vReferenceID As String,
        ByVal vFreight As Double,
        ByVal vFactory As Double,
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
            DimensionValue =
                (pWidthFeet _
                + pWidthInches / 12) _
                * (pHeightFeet _
                + pHeightInches / 12)
        End If

    End Function

    Public Function GetStockOnHand(ByVal vStockID As String) As Integer

        Dim pSql As String

        pSql = "SELECT StocksOnHand " &
            " FROM WholesaleStockInventory " &
            " WHERE StockID = " & ReadyString(vStockID) &
            " AND BranchID = " & mParent.LibraryBranches.CurrentBranchDesignation

        Return CDec2(mProData.ExecuteScalar(pSql))

    End Function

    Public Function ParseDimension(ByVal vText As String,
        ByRef oWidthFeet As Double, ByRef oWidthInches As Double,
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

    Public Function ModeSupplierID(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TOP 1" &
            " TransReceiving.SupplierID" &
            ", SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS Qty" &
            " FROM TransReceivingApproval " &
            " JOIN TransReceiving ON TransReceivingApproval.ReceivingID = TransReceiving.ID " &
            " JOIN TransReceivingStock ON TransReceiving.ID = TransReceivingStock.ReceivingID" &
            " WHERE TransReceiving.IsCancelled = 0" &
            " AND TransReceivingApproval.IsCancelled = 0" &
            " AND TransReceivingStock.StockID = " & ReadyString(vID) &
            " AND DATEdiff(day, TransReceiving.DateReceived, GETDATE()) <= 182" &
            " GROUP BY TransReceiving.SupplierID" &
            " ORDER BY SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) DESC"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            ModeSupplierID = pDT.Rows(0)("SupplierID").ToString.ToUpper
        End If
        pDT.Dispose()

    End Function

    Public Function DateLastApprovedCount(ByVal vStockID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT MAX(DateAndTimeEnded) AS DateAndTimeEnded" &
            " FROM TransCycleCountStocks" &
            " JOIN TransCycleCount" &
            " ON TransCycleCountStocks.CycleCountID = TransCycleCount.ID" &
            " WHERE TransCycleCountStocks.StockID = " & ReadyString(vStockID) &
            " AND IsCancelled = 0" &
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
        pSql = "Update WholesaleStockInventory Set " &
            "IsCritical=1 " &
            " Where (MinStockNumber > StocksOnHand) " &
            " AND (TotalStocksIn <> 0 OR TotalStocksOut <> 0) " &
            " AND (StockID='" & vStockID & "') " &
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

        pSql = "SELECT DISTINCT LibraryStockInformation.ID " &
            " FROM LibraryStockInformation" &
            " LEFT JOIN LibraryStockMeasurements" &
            " ON LibraryStockMeasurements.StockID = LibraryStockInformation.ID" &
            " LEFT JOIN LibraryStockMeasurementsBarcode" &
            " ON LibraryStockMeasurementsBarcode.StockMeasurementsID = LibraryStockMeasurements.ID " &
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

        pSql = "SELECT WholesaleInventoryLedger.ID" &
            ",WholesaleInventoryLedger.DateTimeStamp" &
            ",WholesaleInventoryLedger.InvTransaction" &
            ",CASE WholesaleInventoryLedger.IsReceiving WHEN 1 THEN 0 ELSE WholesaleInventoryLedger.Amount END AS Amount" &
            ",WholesaleInventoryLedger.Debit" &
            ",WholesaleInventoryLedger.Credit" &
            ",WholesaleInventoryLedger.Balance" &
            ",DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.MiddleName AS UpdatedBy" &
            ",WholesaleInventoryLedger.IsReceiving " &
            " FROM WholesaleInventoryLedger" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles ON WholesaleInventoryLedger.UserID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID " &
            " WHERE WholesaleInventoryLedger.StockID = " & ReadyString(vID) &
            " AND WholesaleInventoryLedger.DateTimeStamp BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            " AND WholesaleInventoryLedger.BranchID = " & mParent.LibraryBranches.CurrentBranchDesignation &
            " ORDER BY DateTimeStamp ASC"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetSales2(ByVal vID As String, ByVal vDateFrom As Date, ByVal vDateTo As Date) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT 'Sales To External Customer' AS InvTransaction" & vbCrLf &
            " ,0 AS Debit," & vbCrLf &
            " ISNULL((SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity)" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            " JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" & vbCrLf &
            " WHERE TransInvoiceItems.StockID = " & ReadyString(vID) & vbCrLf &
            " AND TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND LibraryCustomers.IsCompanyBranch = 0" & vbCrLf &
            " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & "),0) AS Credit," & vbCrLf &
            " 1 as SequenceNo" & vbCrLf &
            " FROM LibraryBranches" & vbCrLf &
            " " & vbCrLf &
            " UNION" & vbCrLf &
            " " & vbCrLf &
            " SELECT DISTINCT 'Sales Return From External Customer' AS InvTransaction" & vbCrLf &
            " ,ISNULL((Select SUM(TransMemoSalesReturn.QuantityReturned) AS Debit " & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " JOIN LibraryCustomers ON TransMemoSalesReturn.CustomerID = LibraryCustomers.ID " & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID" & vbCrLf &
            " WHERE TransMemoSalesReturn.StockID = " & ReadyString(vID) & vbCrLf &
            " AND LibraryCustomers.IsCompanyBranch = 0" & vbCrLf &
            " AND CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & "),0) AS Debit," & vbCrLf &
            " 0 AS Credit," & vbCrLf &
            " 2 as SequenceNo " & vbCrLf &
            " FROM LibraryBranches" & vbCrLf &
            " " & vbCrLf &
            " UNION" & vbCrLf &
            " " & vbCrLf &
            " SELECT 'POS Sales' AS InvTransaction" & vbCrLf &
            " ,0 AS Debit" & vbCrLf &
            " ,ISNULL(SUM(POSSalesItems.Quantity * POSSalesItems.ActualQuantity), 0) AS Credit" & vbCrLf &
            " ,3 AS SequenceNo" & vbCrLf &
            " FROM POSSalesItems" & vbCrLf &
            " JOIN POSSales ON POSSales.ID = POSSalesItems.POSSalesID" & vbCrLf &
            " WHERE POSSalesItems.StockID = " & ReadyString(vID) & vbCrLf &
            " AND CONVERT(date, POSSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " " & vbCrLf &
            " UNION" & vbCrLf &
            " " & vbCrLf &
            " SELECT 'POS Returns' AS InvTransaction" & vbCrLf &
            " ,ISNULL(SUM(POSSalesReturnItems.Quantity * POSSalesReturnItems.ActualQuantity), 0) AS Debit" & vbCrLf &
            " ,0 AS Credit" & vbCrLf &
            " ,4 AS SequenceNo" & vbCrLf &
            " FROM POSSalesReturnItems" & vbCrLf &
            " JOIN POSSalesReturn ON POSSalesReturn.ID = POSSalesReturnItems.SalesReturnID" & vbCrLf &
            " JOIN POSSales ON POSSales.ID = POSSalesReturn.ReferenceID" & vbCrLf &
            " WHERE POSSalesReturnItems.StockID = " & ReadyString(vID) & vbCrLf &
            " AND CONVERT(date, POSSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " AND POSSalesReturn.SalesType IN (1, 2)" & vbCrLf &
            " " & vbCrLf &
            " UNION" & vbCrLf &
            " " & vbCrLf &
            " SELECT 'Cash Slip Sales' AS InvTransaction" & vbCrLf &
            " ,0 AS Debit" & vbCrLf &
            " ,ISNULL(SUM(POSCashSlipSalesItems.Quantity * POSCashSlipSalesItems.ActualQuantity), 0) AS Credit" & vbCrLf &
            " ,5 AS SequenceNo" & vbCrLf &
            " FROM POSCashSlipSalesItems" & vbCrLf &
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSCashSlipSalesItems.CashSlipID" & vbCrLf &
            " WHERE POSCashSlipSalesItems.StockID = " & ReadyString(vID) & vbCrLf &
            " AND CONVERT(date, POSCashSlipSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " " & vbCrLf &
            " UNION" & vbCrLf &
            " " & vbCrLf &
            " SELECT 'Cash Slip Returns' AS InvTransaction" & vbCrLf &
            " ,ISNULL(SUM(POSSalesReturnItems.Quantity * POSSalesReturnItems.ActualQuantity), 0) AS Debit" & vbCrLf &
            " ,0 AS Credit" & vbCrLf &
            " ,6 AS SequenceNo" & vbCrLf &
            " FROM POSSalesReturnItems" & vbCrLf &
            " JOIN POSSalesReturn ON POSSalesReturn.ID = POSSalesReturnItems.SalesReturnID" & vbCrLf &
            " JOIN POSCashSlipSales ON POSCashSlipSales.ID = POSSalesReturn.ReferenceID" & vbCrLf &
            " WHERE POSSalesReturnItems.StockID = " & ReadyString(vID) & vbCrLf &
            " AND CONVERT(date, POSCashSlipSales.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " AND POSSalesReturn.SalesType = 3" & vbCrLf &
            " " & vbCrLf &
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

        pSql = "SELECT DISTINCT 'Sales To External Customer' AS InvTransaction," &
            " 0 AS Debit," &
            " ISNULL((SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity)" &
            " FROM TransInvoiceItems" &
            " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID" &
            " JOIN LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID" &
            " WHERE TransInvoiceItems.StockID = " & ReadyString(vID) &
            " AND TransInvoice.IsCancelled = 0" &
            " AND LibraryCustomers.IsCompanyBranch = 0" &
            " AND TransInvoice.InvoiceDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & "),0) AS Credit," &
            " 1 as SequenceNo" &
            " FROM LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Sales Return From External Customer' AS InvTransaction," &
            "ISNULL((Select SUM(TransMemoSalesReturn.QuantityReturned) AS Debit " &
            "        FROM TransMemoSalesReturn INNER JOIN " &
            "        LibraryCustomers ON TransMemoSalesReturn.CustomerID = LibraryCustomers.ID " &
            " WHERE TransMemoSalesReturn.StockID = " & ReadyString(vID) &
            " AND LibraryCustomers.IsCompanyBranch = 0" &
            " AND TransMemoSalesReturn.LastUpdated BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & "),0) as Debit," &
            " 0 AS Credit," &
            " 2 as SequenceNo " &
            " FROM LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT 'Sales To Branches' AS InvTransaction," &
            "0 AS Debit," &
            "ISNULL((Select SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) as Credit " &
            "        FROM TransInvoiceItems INNER JOIN " &
            "        TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID INNER JOIN " &
            "        LibraryCustomers ON TransInvoice.CustomerID = LibraryCustomers.ID " &
            "        WHERE TransInvoiceItems.StockID = " & ReadyString(vID) &
            "        AND TransInvoice.IsCancelled = 0 " &
            "        AND LibraryCustomers.IsCompanyBranch = 1" &
            "        AND TransInvoice.InvoiceDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) & "),0) as Credit," &
            "3 as SequenceNo " &
            " From LibraryBranches"

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Sales Return From Branches' AS InvTransaction," &
            "ISNULL((Select SUM(TransMemoSalesReturn.QuantityReturned) AS Debit " &
            "        FROM TransMemoSalesReturn INNER JOIN " &
            "        LibraryCustomers ON TransMemoSalesReturn.CustomerID = LibraryCustomers.ID " &
            "        WHERE TransMemoSalesReturn.StockID = " & ReadyString(vID) &
            "        AND LibraryCustomers.IsCompanyBranch = 1" &
            "        AND TransMemoSalesReturn.LastUpdated BETWEEN " & ReadyString(vDateFrom) & "  AND " & ReadyString(vDateTo) & "),0) as Debit," &
            "0 AS Credit," &
            "4 as SequenceNo " &
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

        pSql = "SELECT DISTINCT 'Backorder To External Customer' AS InvTransaction," &
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " &
            " FROM TransSalesBackOrderItems" &
            " JOIN TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID" &
            " JOIN LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " &
            " WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) &
            " AND TransSalesBackOrderItems.IsCancelled = 0 " &
            " AND TransSalesBackOrderItems.IsServe = 0" &
            " AND LibraryCustomers.IsCompanyBranch = 0" &
            " AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            " ) " &
            ",0) AS Debit, " &
            "0 as Credit," &
            "1 as SequenceNo " &
            " From LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Cancelled Backorder Of External Customer' AS InvTransaction," &
            "0 as Debit," &
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " &
            "        FROM TransSalesBackOrderItems INNER JOIN " &
            "        TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " &
            "        LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " &
            "        WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) &
            "        AND TransSalesBackOrderItems.IsCancelled = 1" &
            "        AND LibraryCustomers.IsCompanyBranch = 0" &
            "        AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            "       ) " &
            ",0) AS Credit," &
            "2 as SequenceNo " &
            " From LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT 'Backorder To Branches' AS InvTransaction," &
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " &
            "        FROM TransSalesBackOrderItems INNER JOIN " &
            "        TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " &
            "        LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " &
            "        WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) &
            "        AND TransSalesBackOrderItems.IsCancelled = 0" &
            "        AND LibraryCustomers.IsCompanyBranch = 1" &
            "        AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            "       ) " &
            ",0) AS Debit, " &
            "0 as Credit," &
            "3 as SequenceNo " &
            " From LibraryBranches "

        pSql &= " UNION "

        pSql &= "SELECT DISTINCT '     Cancelled Backorder Of Branches' AS InvTransaction," &
            "0 as Debit," &
            "ISNULL((SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Debit " &
            "        FROM TransSalesBackOrderItems INNER JOIN " &
            "        TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " &
            "        LibraryCustomers ON TransSalesOrder.CustomerID = LibraryCustomers.ID " &
            "        WHERE TransSalesBackOrderItems.StockID = " & ReadyString(vID) &
            "        AND TransSalesBackOrderItems.IsCancelled = 1" &
            "        AND LibraryCustomers.IsCompanyBranch = 1" &
            "        AND TransSalesOrder.OrderDate BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            "       ) " &
            ",0) AS Credit," &
            "4 as SequenceNo " &
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

        pSql = "SELECT RIGHT('0000000' + CONVERT(varchar, TransStockPurchase.PONo), 7) AS PONo," &
            "TransStockPurchase.DatePO AS PODate," &
            "LibrarySuppliers.SupplierName," &
            "TransStockPurchaseItems.Quantity," &
            "TransStockPurchaseItems.UnitMeasure," &
            "TransStockPurchaseItems.Term AS Terms," &
            "TransStockPurchaseItems.UnitPrice," &
            "TransStockPurchaseItems.SubTotal " &
            " FROM TransStockPurchaseItems INNER JOIN " &
            " TransStockPurchaseSupplier ON TransStockPurchaseItems.StockPurchaseSupplierID = TransStockPurchaseSupplier.ID INNER JOIN " &
            " TransStockPurchase ON TransStockPurchaseSupplier.StockPurchaseID = TransStockPurchase.ID INNER JOIN " &
            " LibrarySuppliers ON TransStockPurchaseSupplier.SupplierID = LibrarySuppliers.ID " &
            " WHERE TransStockPurchaseItems.StockID = " & ReadyString(vID) &
            " AND TransStockPurchase.DatePO BETWEEN " & ReadyString(vDateFrom) & " AND " & ReadyString(vDateTo) &
            " AND TransStockPurchaseSupplier.IsApproved = 1" &
            " AND TransStockPurchaseSupplier.IsCancelled = 0"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function FixBarcodes() As Integer

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String
        Dim pOut As Integer

        pSql = pSql & " SELECT DISTINCT Barcode" & vbCrLf &
            " FROM LibraryStockMeasurementsBarcode" & vbCrLf &
            " WHERE (Barcode NOT LIKE '%[0-9a-z]'" & vbCrLf &
            " OR Barcode NOT LIKE '[1-9a-z]%'" & vbCrLf &
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
            pSql = pSql & " SELECT LibrarySupplierStocks.StockID" & vbCrLf &
                " ,LibrarySupplierStocks.SupplierID" & vbCrLf &
                " ,LibrarySuppliers.ShipmentMode" & vbCrLf &
                " ,LibrarySuppliers.SupplierName" & vbCrLf &
                " ,LibrarySupplierStockPrices.Price AS SupplierPrice" & vbCrLf &
                " ,LibrarySupplierStockPrices.UnitMeasure AS SupplierUnitMeasure" & vbCrLf &
                " ,LibrarySupplierStockPrices.ActualQuantity AS SupplierActualQuantity" & vbCrLf &
                " ,LibrarySupplierStocks.LeadTime" & vbCrLf &
                " ,LibrarySupplierStocks.MinimumOrderQty" & vbCrLf &
                " ,LibrarySupplierStocks.MinimumOrderUnit" & vbCrLf &
                " ,LibrarySupplierStocks.LotSize" & vbCrLf &
                " ,LibrarySupplierStocks.LotSizeUnit" & vbCrLf &
                " ,LibrarySuppliers.LeadTime AS SupplierLeadTime" & vbCrLf &
                " ,LibrarySuppliers.MinimumOrderQty AS SupplierMinimumOrderQty" & vbCrLf &
                " ,LibrarySuppliers.MinimumOrderUnit AS SupplierMinimumOrderUnit" & vbCrLf &
                " ,LibrarySuppliers.LotSize AS SupplierLotSize" & vbCrLf &
                " ,LibrarySuppliers.LotSizeUnit AS SupplierLotSizeUnit" & vbCrLf &
                " ,LibrarySupplierStockPrices.LastUpdated" & vbCrLf &
                " " & vbCrLf &
                " FROM LibrarySuppliers" & vbCrLf &
                " JOIN LibrarySupplierStocks ON LibrarySupplierStocks.SupplierID = LibrarySuppliers.ID" & vbCrLf &
                " CROSS APPLY " & vbCrLf &
                " (" & vbCrLf &
                " 	SELECT TOP 1 LibrarySupplierStockPrices.ID" & vbCrLf &
                "     ,LibrarySupplierStockPrices.Price" & vbCrLf &
                "     ,LibrarySupplierStockPrices.UnitMeasure" & vbCrLf &
                "     ,LibrarySupplierStockPrices.ActualQuantity" & vbCrLf &
                "     ,LibrarySupplierStockPrices.LastUpdated" & vbCrLf &
                "     FROM LibrarySupplierStockPrices" & vbCrLf &
                "     WHERE LibrarySupplierStockPrices.SupplierStockID = LibrarySupplierStocks.ID" & vbCrLf &
                "     ORDER BY LibrarySupplierStockPrices.LastUpdated DESC" & vbCrLf &
                " ) AS LibrarySupplierStockPrices" & vbCrLf &
                " " & vbCrLf &
                " WHERE LibrarySupplierStocks.StockID IN (" & vIds & ")" & vbCrLf &
                " AND LibrarySuppliers.IsActive = 1" & vbCrLf &
                " " & vbCrLf &
                " ORDER BY LibrarySupplierStockPrices.Price / LibrarySupplierStockPrices.ActualQuantity" & vbCrLf
        Else
            pSqlStocks = "SELECT ID AS StockId" & vbCrLf &
                " FROM LibraryStockInformation" & vbCrLf &
                " WHERE ID IN (" & vIds & ")" & vbCrLf

            pSql = "(SELECT TOP 1 LibrarySupplierStockPrices.ID" & vbCrLf &
                " FROM LibrarySupplierStockPrices" & vbCrLf &
                " JOIN " & vbCrLf &
                " (" & vbCrLf &
                "   SELECT LibrarySupplierStocks.StockID" & vbCrLf &
                "   ,(SELECT TOP 1 LibrarySupplierStockPrices.ID" & vbCrLf &
                "       FROM LibrarySupplierStockPrices" & vbCrLf &
                "       WHERE LibrarySupplierStockPrices.SupplierStockID = LibrarySupplierStocks.ID" & vbCrLf &
                "       ORDER BY LibrarySupplierStockPrices.LastUpdated DESC" & vbCrLf &
                "   ) AS SupplierStockPricesID" & vbCrLf &
                "   FROM LibrarySupplierStocks" & vbCrLf &
                "   JOIN LibrarySuppliers ON LibrarySuppliers.ID = LibrarySupplierStocks.SupplierId" & vbCrLf &
                "   WHERE LibrarySupplierStocks.StockId = t.StockId" & vbCrLf &
                "   AND LibrarySuppliers.IsActive = 1" & vbCrLf &
                " ) t2 ON t2.SupplierStockPricesID = LibrarySupplierStockPrices.ID" & vbCrLf &
                " " & vbCrLf &
                " WHERE t2.StockID = t.StockId" & vbCrLf &
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
            " JOIN LibraryUnitMeasures ON LibraryStockMeasurements.UnitMeasureID = LibraryUnitMeasures.ID" &
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

        pSql = "SELECT SUM(Quantity * ActualQuantity) AS BasedQuantity" & vbCrLf &
            " FROM TransSalesBackOrderItems" & vbCrLf &
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransSalesBackOrderItems.SalesOrderID" & vbCrLf &
            " WHERE TransSalesBackOrderItems.IsCancelled = 0 " & vbCrLf &
            " AND IsServe = 0 " & vbCrLf &
            " AND StockID = " & ReadyString(vID) & vbCrLf &
            " AND IsInvoiced = 0" & vbCrLf &
            " AND DATEDIFF(day, TransSalesBackOrderItems.RegisteredDate, GETDATE()) < TransSalesBackOrderItems.CancellationPeriod" & vbCrLf &
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

        pSql = "SELECT SUM(TransSalesOrderItems.Quantity * TransSalesOrderItems.ActualQuantity) AS Total" & vbCrLf &
            " FROM TransSalesOrder" & vbCrLf &
            " JOIN TransSalesOrderItems ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & vbCrLf &
            " WHERE CONVERT(date, TransSalesOrder.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf &
            " AND TransSalesOrderItems.StockID = " & ReadyString(vID) & vbCrLf &
            " AND TransSalesOrder.IsCancelled = 0" & vbCrLf &
            " " & vbCrLf &
            " UNION " & vbCrLf &
            " " & vbCrLf &
            " SELECT SUM(TransSalesBackOrderItems.Quantity * TransSalesBackOrderItems.ActualQuantity) AS Total" & vbCrLf &
            " FROM TransSalesBackOrderItems" & vbCrLf &
            " JOIN TransSalesOrder ON TransSalesOrder.ID = TransSalesBackOrderItems.SalesOrderID" & vbCrLf &
            " WHERE CONVERT(date, TransSalesOrder.LastUpdated) BETWEEN " & ReadyString(vDateFrom.Date) & " AND " & ReadyString(vDateTo.Date) & vbCrLf &
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
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" &
            ",SUM(DrpCacheStockSalesDaily.QtyInternal + DrpCacheStockSalesDaily.QtyExternal) AS Qty" &
            ",DatePart(year, DrpCacheStockSalesDaily.TransDate) AS TransYear" &
            " FROM DrpCacheStockSalesDaily" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockSalesDaily.StockID" &
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" &
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
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '')" &
            " ORDER BY " &
            " DatePart(year, DrpCacheStockSalesDaily.TransDate) DESC" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " &
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
            pSql = "SELECT DepartureDay" &
                ",LeadTime" &
                ",DepartureHour" &
                " FROM LibraryShipperVessels " &
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

        pSqlLastApprovedCount = "SELECT MAX(DateAndTimeEnded) AS DateAndTimeEnded" &
            " FROM TransCycleCountStocks" &
            " JOIN TransCycleCount" &
            " ON TransCycleCountStocks.CycleCountID = TransCycleCount.ID" &
            " WHERE TransCycleCountStocks.StockID = DrpCacheStockSalesDaily.StockID" & vbCrLf &
            " AND IsCancelled = 0" &
            " AND ApprovedBy <> 0"

        pSqlQtyOnHand = "SELECT TOP 1 StocksOnHand " &
            " FROM WholesaleStockInventory " &
            " WHERE StockID = DrpCacheStockSalesDaily.StockID" &
            " AND BranchID = " & pBranchID &
            " ORDER BY WholesaleStockInventory.LastUpdated DESC"

        pSql = ""
        If CStr2(vIDs).Length > 0 Then
            pSql = " AND DrpCacheStockSalesDaily.StockID IN (" & vIDs & ")"
        End If
        pSql = "SELECT TOP 100 PERCENT DrpCacheStockSalesDaily.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription" &
            ",SUM(DrpCacheStockSalesDaily.QtyInternal + DrpCacheStockSalesDaily.QtyExternal) AS Qty" &
            ",SUM(DrpCacheStockSalesDaily.COGS) AS COGS" &
            ",(" & pSqlLastApprovedCount & ") AS LastApprovedCount" & vbCrLf &
            ",(" & pSqlQtyOnHand & ") AS QtyOnHand" & vbCrLf &
            " FROM DrpCacheStockSalesDaily" &
            " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockSalesDaily.StockID" &
            " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" &
            " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" &
            " WHERE DATEDIFF(day, DrpCacheStockSalesDaily.TransDate, GETDATE()) < 365" &
            pSql &
            " GROUP BY DrpCacheStockSalesDaily.StockID" &
            ",LibraryStockInformation.StockNumber" &
            ",ISNULL(NULLIF(LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,'),'') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,'), '') + " &
            "   ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,'), '') + " &
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

        pSql = "SELECT MIN(LastUpdated) AS DateStart" & vbCrLf &
            " ,MAX(LastUpdated) AS DateEnd" & vbCrLf &
            " FROM DrpCacheStockRank" & vbCrLf
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 _
            AndAlso IsDate(pDT.Rows(0)("DateStart")) Then

            Return New DateRange(pDT.Rows(0)("DateStart"), pDT.Rows(0)("DateEnd"))
        End If

    End Function

    Public Sub AddAlert(vID As String, vProblem As String, vUserID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransPIMAlerts")
        pIU("ProductID") = ReadyString(vID)
        pIU("Problem") = ReadyString(vProblem)
        pIU("UserID") = ReadyString(vUserID)

        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

    End Sub

    ''' <summary>
    ''' requires the current organization
    ''' </summary>
    ''' <param name="vProductID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAllowDecimal(vProductID As String) As Boolean

        Dim pSql As String

        pSql = "SELECT AllowDecimal" &
            " FROM LibraryOrganizationPIM" &
            " WHERE ProductID = " & ReadyString(vProductID)

        Return CDec2((mProData.ExecuteScalar(pSql)))

    End Function

    Public Function GetContents(vParentProductID As String, vChildProductID As String, vOrganizationID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " ,ISNULL(MobizLibraryCountriesLanguageLabel.Label, MobizLibraryPIMProductContents.UnitMeasure) AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductContents.Quantity" & vbCrLf &
            " FROM MobizLibraryPIMProductContents" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " WHERE MobizLibraryPIMProductContents.ParentProductID = " & ReadyString(vParentProductID) & vbCrLf &
            " AND MobizLibraryPIMProductContents.ChildProductID = " & ReadyString(vChildProductID) & vbCrLf &
            " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(vOrganizationID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub UpdateParentProductPrice(vChildProductIDs As List(Of String), vOrgID As String)

        Dim pAllParentIDs As New List(Of String)

        For Each pChildProductID In vChildProductIDs
            Dim pParentIDs = UpdateParentProductPrice(pChildProductID, vOrgID, pAllParentIDs)
            pAllParentIDs.AddRange(pParentIDs)
        Next

    End Sub

    ''' <summary>
    ''' return parent product IDs
    ''' </summary>
    ''' <param name="vChildProductID"></param>
    ''' <param name="vOrgID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateParentProductPrice(vChildProductID As String, vOrgID As String,
        Optional vExcludeParentIDs As List(Of String) = Nothing) As List(Of String)

        Dim pOut = New List(Of String)
        Dim pSql = "SELECT DISTINCT ParentProductID" & vbCrLf &
            " FROM MobizLibraryPIMProductContents" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " AND ChildProductID = " & ReadyString(vChildProductID)
        Dim pDtParents = mProData.Execute(pSql)

        For Each pDrParent As DataRow In pDtParents.Rows
            If vExcludeParentIDs IsNot Nothing _
                AndAlso vExcludeParentIDs.Contains(ReadyGuid(pDrParent("ParentProductID"))) Then

                Continue For
            End If
            pOut.Add(ReadyGuid(pDrParent("ParentProductID")))
            UpdateProductPriceFromContents(CStr2(pDrParent("ParentProductID")), vOrgID)
        Next

        Return pOut

    End Function

    Public Sub UpdateProductPriceFromContents(vParentProductID As String, vOrgID As String)

        Dim pDtContents = GetContents(vParentProductID, vOrgID)
        If pDtContents.Rows.Count = 0 Then
            Return
        End If
        Dim pFactoryPrice = CDec2(pDtContents.Compute("SUM(TotalFactoryPrice)", ""))
        Dim pFreightCost = CDec2(pDtContents.Compute("SUM(TotalFreightCost)", ""))
        Dim pLandedCost = CDec2(pDtContents.Compute("SUM(TotalLandedCost)", ""))

        SetPrice(vParentProductID, "01000012-0002-0000-0000-000000000000", pFactoryPrice, vOrgID)
        SetPrice(vParentProductID, "01000012-0001-0000-0000-000000000000", pFreightCost, vOrgID)
        SetPrice(vParentProductID, "01000012-0003-0000-0000-000000000000", pLandedCost, vOrgID)

    End Sub

    'Public Function GetRecursiveContents(vOrganization As String)

    '    Dim pSql = " DECLARE @OrgID UNIQUEIDENTIFIER" & vbCrLf &
    '        " SELECT @OrgID = " & ReadyString(vOrganization) & vbCrLf &
    '        " SELECT MobizLibraryPIMProduct.ID" & vbCrLf &
    '        " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
    '        " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
    '        " ,MobizLibraryPIMProductParent.Jit" & vbCrLf &
    '        " FROM MobizLibraryPIMProduct" & vbCrLf &
    '        " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
    '        " LEFT JOIN MobizLibraryPIMProductParent ON MobizLibraryPIMProductParent.ProductID = LibraryOrganizationPIM.ProductID" & vbCrLf &
    '        " AND MobizLibraryPIMProductParent.OrganizationID = @OrgID" & vbCrLf &
    '        " WHERE MobizLibraryPIMProduct.ID IN (" & vbCrLf &
    '        " 	SELECT MobizLibraryPIMProductContents.ParentProductID" & vbCrLf &
    '        "     FROM MobizLibraryPIMProductContents" & vbCrLf &
    '        "     WHERE MobizLibraryPIMProductContents.OrganizationID = @OrgID" & vbCrLf &
    '        " )" & vbCrLf &
    '        " AND ISNULL(LibraryOrganizationPIM.IsDeactivated, 0) = 0" & vbCrLf &
    '        " ORDER BY MobizLibraryPIMProduct.ProductName"
    '    Dim pDtParentProducts = mProData.Execute(pSql)
    '    For Each pDrParentProduct In pDtParentProducts.Rows

    '    Next

    'End Function

    'Public Function GetRecursiveContents(vProductID As String, vOrganization As String)

    '    Dim pSql = " DECLARE @OrgID UNIQUEIDENTIFIER" & vbCrLf &
    '        " SELECT @OrgID = " & ReadyString(vOrganization) & vbCrLf &
    '        " SELECT MobizLibraryPIMProduct.ID" & vbCrLf &
    '        " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
    '        " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
    '        " ,MobizLibraryPIMProductParent.Jit" & vbCrLf &
    '        " FROM MobizLibraryPIMProduct" & vbCrLf &
    '        " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
    '        " LEFT JOIN MobizLibraryPIMProductParent ON MobizLibraryPIMProductParent.ProductID = LibraryOrganizationPIM.ProductID" & vbCrLf &
    '        " AND MobizLibraryPIMProductParent.OrganizationID = @OrgID" & vbCrLf &
    '        " WHERE MobizLibraryPIMProduct.ID IN (" & vbCrLf &
    '        " 	SELECT MobizLibraryPIMProductContents.ParentProductID" & vbCrLf &
    '        "     FROM MobizLibraryPIMProductContents" & vbCrLf &
    '        "     WHERE MobizLibraryPIMProductContents.OrganizationID = @OrgID" & vbCrLf &
    '        " )" & vbCrLf &
    '        " AND ISNULL(LibraryOrganizationPIM.IsDeactivated, 0) = 0" & vbCrLf &
    '        " ORDER BY MobizLibraryPIMProduct.ProductName"
    '    Dim pDtParentProducts = mProData.Execute(pSql)
    '    For Each pDrParentProduct In pDtParentProducts.Rows

    '    Next

    'End Function

    Public Function ProductsAndContentsExcel(vOrganization As String) As String

        Dim pDT = ProductsAndContents(vOrganization)
        Dim pParentProductID As String
        For Each pDR As DataRow In pDT.Rows
            If pParentProductID = CStr2(pDR("ParentProductID")) Then
                pDR("ParentProductNo") = DBNull.Value
                pDR("ParentProductName") = DBNull.Value
            Else
                pParentProductID = CStr2(pDR("ParentProductID"))
            End If
        Next
        pDT.Columns.Remove("ParentProductID")
        pDT.Columns.Remove("ChildProductID")
        Dim pFileTitle = "BOM_" & DateTimeStamp() & ".xlsx"
        TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\documents\" & pFileTitle)

        Return pFileTitle

    End Function

    Public Function ProductsAndContents(vOrganization As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " -- Parent Products and Contents" & vbCrLf &
            " SELECT tblParent.ProductNo AS ParentProductNo" & vbCrLf &
            " ,tblParent.ProductName AS ParentProductName" & vbCrLf &
            " ,tblParent.ID AS ParentProductID" & vbCrLf &
            " ,T.*" & vbCrLf &
            " from MobizLibraryPIMProduct tblParent" & vbCrLf &
            " CROSS APPLY ("

        pSql &= " SELECT TOP 100 PERCENT MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo AS ChildProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName AS ChildProductName" & vbCrLf &
            " ,MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " ,ISNULL(MobizLibraryCountriesLanguageLabel.Label, MobizLibraryPIMProductContents.UnitMeasure) AS ChildUnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductContents.Quantity" & vbCrLf &
            "  ,tblLandedCost.Price / ISNULL(tblCustomUnit.Quantity, 1) AS LandedCost" & vbCrLf &
            "  ,tblLandedCost.Price * MobizLibraryPIMProductContents.Quantity / ISNULL(tblCustomUnit.Quantity, 1) AS TotalLandedCost" & vbCrLf &
            "  ,tblFactoryPrice.Price / ISNULL(tblCustomUnit.Quantity, 1) AS FactoryPrice" & vbCrLf &
            "  ,tblFactoryPrice.Price * MobizLibraryPIMProductContents.Quantity / ISNULL(tblCustomUnit.Quantity, 1) AS TotalFactoryPrice" & vbCrLf &
            "  ,tblFreightCost.Price / ISNULL(tblCustomUnit.Quantity, 1) AS FreightCost" & vbCrLf &
            "  ,tblFreightCost.Price * MobizLibraryPIMProductContents.Quantity / ISNULL(tblCustomUnit.Quantity, 1) AS TotalFreightCost" & vbCrLf &
            " -- ,tblCustomUnit.Quantity AS tblCustomUnit_Quantity -- used for debugging only" & vbCrLf &
            " --  ,tblCustomUnit.DestinationMeasure AS tblCustomUnit_DestinationMeasure  -- used for debugging only" & vbCrLf &
            " FROM MobizLibraryPIMProductContents" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "  	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0003-0000-0000-000000000000'" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "  ) tblLandedCost" & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "  	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0002-0000-0000-000000000000'" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "  ) tblFactoryPrice" & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "  	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0001-0000-0000-000000000000'" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "  ) tblFreightCost" & vbCrLf

        pSql &= " OUTER APPLY (" & vbCrLf &
            "   	SELECT MobizLibraryPIMMeasureDefinition.SourceMeasureID" & vbCrLf &
            "     ,MobizLibraryPIMMeasureDefinition.DestinationMeasure" & vbCrLf &
            "     ,MobizLibraryPIMMeasureDefinition.Quantity" & vbCrLf &
            "     FROM MobizLibraryPIMMeasureDefinition" & vbCrLf &
            "     WHERE MobizLibraryPIMMeasureDefinition.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductContents.UnitMeasureID IS NULL" & vbCrLf &
            "     AND MobizLibraryPIMMeasureDefinition.DestinationMeasure = ISNULL(MobizLibraryCountriesLanguageLabel.Label, MobizLibraryPIMProductContents.UnitMeasure)" & vbCrLf &
            "     AND MobizLibraryPIMMeasureDefinition.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "   ) tblCustomUnit" & vbCrLf

        pSql &= " WHERE MobizLibraryPIMProductContents.ParentProductID = tblParent.ID" & vbCrLf &
            " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.ProductName" & vbCrLf
        pSql &= " ) T" & vbCrLf &
            " ORDER BY tblParent.ProductNo" & vbCrLf &
            " ,tblParent.ProductName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetContents(vParentProductID As String, vOrganization As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " ,ISNULL(MobizLibraryCountriesLanguageLabel.Label, MobizLibraryPIMProductContents.UnitMeasure) AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductContents.Quantity" & vbCrLf &
            "  ,tblLandedCost.Price / ISNULL(tblCustomUnit.Quantity, 1) AS LandedCost" & vbCrLf &
            "  ,tblLandedCost.Price * MobizLibraryPIMProductContents.Quantity / ISNULL(tblCustomUnit.Quantity, 1) AS TotalLandedCost" & vbCrLf &
            "  ,tblFactoryPrice.Price / ISNULL(tblCustomUnit.Quantity, 1) AS FactoryPrice" & vbCrLf &
            "  ,tblFactoryPrice.Price * MobizLibraryPIMProductContents.Quantity / ISNULL(tblCustomUnit.Quantity, 1) AS TotalFactoryPrice" & vbCrLf &
            "  ,tblFreightCost.Price / ISNULL(tblCustomUnit.Quantity, 1) AS FreightCost" & vbCrLf &
            "  ,tblFreightCost.Price * MobizLibraryPIMProductContents.Quantity / ISNULL(tblCustomUnit.Quantity, 1) AS TotalFreightCost" & vbCrLf &
            "  ,tblCustomUnit.Quantity AS tblCustomUnit_Quantity -- used for debugging only" & vbCrLf &
            "  ,tblCustomUnit.DestinationMeasure AS tblCustomUnit_DestinationMeasure  -- used for debugging only" & vbCrLf &
            " FROM MobizLibraryPIMProductContents" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "  	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0003-0000-0000-000000000000'" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "  ) tblLandedCost" & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "  	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0002-0000-0000-000000000000'" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "  ) tblFactoryPrice" & vbCrLf
        pSql &= "  OUTER APPLY (" & vbCrLf &
            "  	SELECT TOP 1 MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            "  	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0001-0000-0000-000000000000'" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "  ) tblFreightCost" & vbCrLf

        pSql &= " OUTER APPLY (" & vbCrLf &
            "   	SELECT MobizLibraryPIMMeasureDefinition.SourceMeasureID" & vbCrLf &
            "     ,MobizLibraryPIMMeasureDefinition.DestinationMeasure" & vbCrLf &
            "     ,MobizLibraryPIMMeasureDefinition.Quantity" & vbCrLf &
            "     FROM MobizLibraryPIMMeasureDefinition" & vbCrLf &
            "     WHERE MobizLibraryPIMMeasureDefinition.ProductID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductContents.UnitMeasureID IS NULL" & vbCrLf &
            "     AND MobizLibraryPIMMeasureDefinition.DestinationMeasure = ISNULL(MobizLibraryCountriesLanguageLabel.Label, MobizLibraryPIMProductContents.UnitMeasure)" & vbCrLf &
            "     AND MobizLibraryPIMMeasureDefinition.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            "   ) tblCustomUnit" & vbCrLf

        pSql &= " WHERE MobizLibraryPIMProductContents.ParentProductID = " & ReadyString(vParentProductID) & vbCrLf &
            " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(vOrganization) & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.ProductName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetContents(Optional vOrgID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        If vOrgID IsNot Nothing Then
            pSql = " WHERE MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(vOrgID) & vbCrLf
        End If
        pSql = " SELECT MobizLibraryPIMProductContents.ChildProductID AS ProductID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " ,ISNULL(MobizLibraryCountriesLanguageLabel.Label, MobizLibraryPIMProductContents.UnitMeasure) AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductContents.Quantity" & vbCrLf &
            ",MobizLibraryPIMProductContents.ParentProductID" & vbCrLf &
            ",MobizLibraryPIMProductContents.OrganizationID" & vbCrLf &
            " ,tblParentProduct.ProductNo AS ParentProductNo" & vbCrLf &
            " ,tblParentProduct.ProductName AS ParentProductName" & vbCrLf &
            " FROM MobizLibraryPIMProductContents" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = MobizLibraryPIMProductContents.ChildProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductContents.UnitMeasureID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct tblParentProduct ON tblParentProduct.ID = MobizLibraryPIMProductContents.ParentProductID" & vbCrLf &
            pSql &
            " ORDER BY MobizLibraryPIMProduct.ProductName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetOrCreateUnitDefinition(vProductID As String, vOrgID As String) As DataRow

        Dim pDR As DataRow
        Dim pDT As DataTable

        pDR = GetUnitDefinition(vProductID, vOrgID)
        If pDR Is Nothing Then
            pDT = ByIds(ReadyString(vProductID), , True)
            If pDT.Rows.Count = 0 Then
                Return Nothing
            End If
            pDR = pDT.Rows(0)
            If (CDec2(pDR("BaseUnitQty"))) < 0.001 Then
                Return Nothing
            End If
            If IsGuid(pDR("BaseUnitMeasureID")) = False Then
                Return Nothing
            End If
            If (CDec2(pDR("ContentQty"))) < 0.001 Then
                Return Nothing
            End If
            If String.IsNullOrWhiteSpace(CStr2(pDR("ContentUnit"))) Then
                Return Nothing
            End If
            CreateUnitDefinition(
                vProductID:=vProductID,
                vSourceMeasureID:=CStr2(pDR("BaseUnitMeasureID")),
                vDestinationMeasure:=pDR("ContentUnit"),
                vQuantity:=pDR("ContentQty") / pDR("BaseUnitQty"),
                vOrgID:=vOrgID)
            pDR = GetUnitDefinition(vProductID, vOrgID)
        End If

        Return pDR

    End Function

    Public Function GetUnitDefinition(vProductID As String, vOrganization As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "  SELECT MobizLibraryPIMMeasureDefinition.Quantity" & vbCrLf &
            ",MobizLibraryPIMMeasureDefinition.SourceMeasureID" & vbCrLf &
            "  ,tblSourceLabel.Label AS SourceUnitMeasure" & vbCrLf &
            "  ,MobizLibraryPIMMeasureDefinition.DestinationMeasure" & vbCrLf &
            "  " & vbCrLf &
            "  FROM MobizLibraryPIMMeasureDefinition" & vbCrLf &
            "  " & vbCrLf &
            "  JOIN MobizLibraryPIMMeasure tblSourceMeasure ON tblSourceMeasure.ID = MobizLibraryPIMMeasureDefinition.SourceMeasureID" & vbCrLf &
            "  LEFT JOIN MobizLibraryCountriesLanguageLabel tblSourceLabel ON tblSourceLabel.ID = tblSourceMeasure.MeasureNameID" & vbCrLf &
            "   " & vbCrLf &
            " WHERE MobizLibraryPIMMeasureDefinition.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND MobizLibraryPIMMeasureDefinition.OrganizationID = " & ReadyString(vOrganization)

        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function CreateUnitDefinition(vProductID As String,
        vSourceMeasureID As String,
        vQuantity As String,
        vDestinationMeasure As String,
        vOrgID As String) As String

        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pID As String

        vDestinationMeasure = RemoveParenthesis(vDestinationMeasure)
        mProData.BeginTransaction()
        Try
            pSql = "DELETE FROM MobizLibraryPIMMeasureDefinition" &
                " WHERE ProductID = " & ReadyString(vProductID) &
                " AND OrganizationID = " & ReadyString(vOrgID)
            mProData.ExecuteSQL(pSql)

            pIU = New InsertUpdate("MobizLibraryPIMMeasureDefinition")
            pID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pID)
            pIU("OrganizationID") = ReadyString(vOrgID)
            pIU("ProductID") = ReadyString(vProductID)
            pIU("SourceMeasureID") = ReadyString(vSourceMeasureID)
            pIU("DestinationMeasure") = ReadyString(vDestinationMeasure)
            pIU("Quantity") = vQuantity
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            mProData.CommitTransaction()

            Return pID
        Catch ex As System.Exception
            mProData.RollbackTransaction(, True)
            Throw
        End Try

    End Function

    Private Shared Function RemoveParenthesis(vText As String) As String

        If String.IsNullOrWhiteSpace(vText) Then
            Return vText
        End If

        Dim pIndex = vText.IndexOf("(")
        If pIndex = -1 Then
            Return vText
        Else
            Return vText.Substring(0, pIndex).Trim()
        End If

    End Function

    Public Function GetBomHeader(vID As String, vOrganization As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = " SELECT ISNULL(Jit, 0) AS Jit" & vbCrLf &
            " ,Instructions" & vbCrLf &
            " FROM MobizLibraryPIMProductParent" & vbCrLf &
            " WHERE MobizLibraryPIMProductParent.ProductID = " & ReadyString(vID) &
            " AND MobizLibraryPIMProductParent.OrganizationID = " & ReadyString(vOrganization)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetBomBehaviorOnSalesReturns(vID As String, vOrganization As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = " SELECT ISNULL(IsSeperateIntoComponentDuringSalesReturns, 0) AS IsSeperateIntoComponentDuringSalesReturns" & vbCrLf &
            " ,Instructions" & vbCrLf &
            " FROM MobizLibraryPIMProductParent" & vbCrLf &
            " WHERE MobizLibraryPIMProductParent.ProductID = " & ReadyString(vID) &
            " AND MobizLibraryPIMProductParent.OrganizationID = " & ReadyString(vOrganization)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetRecursiveContents(Optional vOrgID As String = Nothing) As String

        Dim p As New RecursiveContents(mParent)

        Return p.GetIssues(vOrgID)

    End Function

    Public Function GetRecursiveContents(vOrganizationID As String, vParentProductID As String, vChildProductID As String) As String

        Dim p As New RecursiveContents(mParent)

        Return p.GetFutureIssues(vParentProductID, vChildProductID, vOrganizationID)

    End Function

    Public Sub DeleteContents(vParentProductID As String, vOrganizationID As String)

        Dim pSql As String

        pSql = "DELETE FROM MobizLibraryPIMProductContents" &
            " WHERE ParentProductID = " & ReadyString(vParentProductID) &
            " AND OrganizationID = " & ReadyString(vOrganizationID)
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function AddContent(vParentProductID As String, vChildProductID As String, vQuantity As Decimal, vUnitMeasure As String, vOrganizationID As String)

        Dim pIU As InsertUpdate
        Dim pSql As String

#If DEBUG Then
        If ReadyGuid(vParentProductID) = ReadyGuid("8e421e70-0f59-4cea-baa3-f4f72af369d4") _
            AndAlso ReadyGuid(vChildProductID) = ReadyGuid("9c8f60eb-4f8f-474b-9e97-2353208cea53") Then

            Stop2()
        End If
#End If

        '// insert/update LibraryPIMProductContents
        pIU = New InsertUpdate("MobizLibraryPIMProductContents")
        pIU("ID") = "NEWID()"
        pIU("ParentProductID") = ReadyString(vParentProductID)
        pIU("ChildProductID") = ReadyString(vChildProductID)
        pIU("Quantity") = vQuantity
        pIU("UnitMeasure") = ReadyString(vUnitMeasure)
        pIU("OrganizationID") = ReadyString(vOrganizationID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        '// update parent LibraryPIMProduct
        pIU = New InsertUpdate("MobizLibraryPIMProduct")
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetUpdate("ID = " & ReadyString(vParentProductID))
        mProData.ExecuteSQL(pSql)

    End Function

    Public Function ImportFromExcel(vDT As DataTable, vOrgID As String)

        Dim pDrProduct As DataRow
        Dim pParentProductNum As String
        Dim pParentID As String
        Dim pChildProductNum As String
        Dim pChildID As String
        Dim pQty As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pHasParent As Boolean
        Dim pUnitMeasure As String
        Dim pParentIsOk As Boolean

        For Each pDR As DataRow In vDT.Rows
            pHasParent = False

            pParentProductNum = CStr2(pDR("Finished Product Number"))
            If pParentProductNum.Trim().Length > 0 Then
                pDrProduct = ByNumberAndName(pParentProductNum, "")
                If pDrProduct Is Nothing Then
                    pDR("Import Result Column") = "Finished product with number " & pParentProductNum & " does not exist."
                    Continue For
                End If
                pParentID = CStr2(pDrProduct("ID"))
                pHasParent = True
                pParentIsOk = False
            ElseIf pParentIsOk = False Then
                pDR("Import Result Column") = "There is a problem saving the previous row with the same finished product."
                Continue For
            End If

            If pParentID Is Nothing Then
                pDR("Import Result Column") = "No finished product."
                Continue For
            End If

            pChildProductNum = CStr2(pDR("Component Product Number"))
            pDrProduct = ByNumberAndName(pChildProductNum, "")
            If pDrProduct Is Nothing Then
                pDR("Import Result Column") = "Component product with number " & pParentProductNum & " does not exist."
                Continue For
            End If
            pChildID = CStr2(pDrProduct("ID"))

            pQty = pDR("Quantity")
            If IsNumeric(pQty) = False Then
                pDR("Import Result Column") = "Quantity is not a valid number."
                Continue For
            End If

            pUnitMeasure = pDR("Unit")
            If CStr2(pUnitMeasure).Trim() = "" Then
                pDR("Import Result Column") = "No unit measure."
                Continue For
            End If

#If DEBUG Then
            If pDR("Spreadsheet Row Number") = 129 Then
                Stop2()
            End If
#End If

            If pHasParent Then
                DeleteContents(pParentID, vOrgID)
                pParentIsOk = True
            End If
            AddContent(pParentID, pChildID, CDec2(pQty), pUnitMeasure, vOrgID)
        Next pDR

    End Function

    Private Class RecursiveContents
        Dim mBusiness As Business
        Dim mDtAll As DataTable

        Public Sub New(vBusiness As Business)

            mBusiness = vBusiness

        End Sub

        ''' <summary>
        ''' use this to test issues before saving the child product.
        ''' used in StockController/SaveBomPageData
        ''' </summary>
        ''' <param name="vParentProductID"></param>
        ''' <param name="vChildParentID"></param>
        ''' <param name="vOrganizationID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFutureIssues(vParentProductID As String, vChildParentID As String,
                vOrganizationID As String) As String

            Dim pDT As DataTable
            Dim pTrails As List(Of Trail)
            Dim pProductName As String
            Dim pError As String

            pTrails = New List(Of Trail)

            pDT = mBusiness.LibraryStock.GetContents(vOrganizationID)

            AddTrail(vDtAll:=pDT,
                vParentProductID:=vChildParentID,
                rTrails:=pTrails,
                rCurrentTrail:=New Trail(CreateTrailItemFromID(pDT, vChildParentID)),
                oError:=pError
            )

#If DEBUG Then
            Dim pStr As String

            pStr = ""
            For Each pTrail As Trail In pTrails
                pStr &= pTrail.Display()
                pStr &= vbCrLf & vbCrLf
            Next pTrail
            Debug.Print("vbWP6V8JLW54! " & vbCrLf & pStr)
#End If

            Return pError

        End Function

        Private Function AddTrail(vDtAll As DataTable, vParentProductID As String,
            ByRef rTrails As List(Of Trail), ByRef rCurrentTrail As Trail,
            ByRef oError As String) As Boolean

            Dim pDV As DataView
            Dim pTrail As Trail

            oError = Nothing
            If rCurrentTrail.ContainsProductID(vParentProductID) Then
                oError = "You may not add that product as a content because it already exist in the product line. "
                oError &= rCurrentTrail.Display()
                Return False
            End If

            rCurrentTrail.Items.Add(CreateTrailItemFromID(vDtAll, vParentProductID))
            pDV = New DataView(vDtAll, "ParentProductID = " & ReadyString(vParentProductID), "", DataViewRowState.CurrentRows)
            If pDV.Count = 0 Then
                rTrails.Add(rCurrentTrail)
                Return True
            End If

            For Each pDRV As DataRowView In pDV
                pTrail = rCurrentTrail.Clone()

                AddTrail(vDtAll:=vDtAll,
                    vParentProductID:=CStr2(pDRV("ChildProductID")),
                    rTrails:=rTrails,
                    rCurrentTrail:=pTrail,
                    oError:=oError
                )
            Next pDRV

            Return True

        End Function

        Private Shared Function CreateTrailItemFromID(vDtAll As DataTable, vChildProductID As String) As TrailItem

            Dim pDV As DataView
            Dim pOut As TrailItem

            pDV = New DataView(vDtAll, "ChildProductID = " & ReadyString(vChildProductID), "", DataViewRowState.CurrentRows)
            If pDV.Count = 0 Then
                pOut = New TrailItem(vChildProductID, "")
            Else
                pOut = New TrailItem(vChildProductID, CStr2(pDV(0)("ProductName")))
            End If

            Return pOut

        End Function


        Public Class Trail

            Dim mItems As List(Of TrailItem)

            Public Sub New()

                mItems = New List(Of TrailItem)

            End Sub

            Public Sub New(vProductID As String, vProductName As String)

                mItems = New List(Of TrailItem)
                mItems.Add(New TrailItem(vProductID, vProductName))

            End Sub

            Public Sub New(vTrailItem As TrailItem)

                mItems = New List(Of TrailItem)
                mItems.Add(vTrailItem)

            End Sub

            Public ReadOnly Property Items As List(Of TrailItem)
                Get

                    Return mItems

                End Get
            End Property

            Public Function ContainsProductID(vProductID As String) As Boolean

                Return (From pItem As TrailItem In mItems
                        Where ReadyGuid(pItem.ID) = ReadyGuid(vProductID)).Any()

            End Function

            Public Function Display() As String

                Dim pOut As String = ""

                For Each pItem As TrailItem In mItems
                    If CStr2(pItem.ProductName).Trim() = "" Then
                        Continue For
                    End If
                    If pOut <> "" Then
                        pOut &= " -> "
                    End If
                    pOut &= pItem.ProductName.Trim()
                Next pItem

                Return pOut

            End Function

            Public Function Clone() As Trail

                Dim p As Trail

                p = New Trail
                For Each pItem As TrailItem In mItems
                    p.Items.Add(pItem)
                Next pItem

                Return p

            End Function
        End Class

        Public Class TrailItem

            Public ID As String
            Public ProductName As String

            Public Sub New(vID As String, vProductName As String)

                ID = ReadyGuid(vID)
                ProductName = vProductName

            End Sub
        End Class

        ''' <summary>
        ''' use this overload to find issues that already exist in the database.
        ''' this will detect issues on all organizations
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIssues(vOrgID As String) As String

            Dim pDtOrgs As DataTable
            Dim pOut As String = ""
            Dim pIssue As String
            Dim pDV As DataView

            mDtAll = mBusiness.LibraryStock.GetContents(vOrgID)
            pDtOrgs = New DataView(mDtAll, "", "", DataViewRowState.CurrentRows).ToTable(True, {"OrganizationID"})
            For Each pDrOrg As DataRow In pDtOrgs.Rows
                pDV = New DataView(mDtAll, "OrganizationID = " & ReadyString(pDrOrg("OrganizationID")), "OrganizationID", DataViewRowState.CurrentRows)
                pIssue = GetOrganizationIssues(CStr2(pDrOrg("OrganizationID")), pDV.ToTable())
                If CStr2(pIssue).Trim() <> "" Then
                    If pOut <> "" Then
                        pOut &= vbCrLf
                    End If
                    pOut &= pIssue
                End If
            Next pDrOrg

            Return pOut

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vOrgID"></param>
        ''' <param name="vDT">filtered to contain only rows from that org</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetOrganizationIssues(vOrgID As String, vDT As DataTable) As String

            Dim pDtParentProducts As DataTable
            Dim pIssue As String
            Dim pOut As String

            pDtParentProducts = New DataView(vDT, "", "", DataViewRowState.CurrentRows).ToTable(True, {"ParentProductID", "ParentProductName", "ParentProductNo"})
            pDtParentProducts.Columns("ParentProductID").ColumnName = "ProductID"
            pDtParentProducts.Columns("ParentProductName").ColumnName = "ProductName"
            pDtParentProducts.Columns("ParentProductNo").ColumnName = "ProductNo"
            For Each pDrParentProduct As DataRow In pDtParentProducts.Rows
                pIssue = GetParentProductIssues(pDrParentProduct, vDT)
                If CStr2(pIssue).Trim() <> "" Then
                    If pOut <> "" Then
                        pOut &= ";" & vbCrLf
                    End If
                    pOut &= pIssue
                End If
            Next pDrParentProduct

            Return pOut

        End Function

        Private Function GetParentProductIssues(vDrProduct As DataRow, vDtOrgProducts As DataTable,
            Optional vTrail As List(Of DataRow) = Nothing) As String

            If vTrail Is Nothing Then
                vTrail = New List(Of DataRow)
            End If
            Dim pParentProductID = ReadyGuid(vDrProduct("ProductID"))
            Dim pOut As String
            Dim pHasIssue As Boolean

            For Each pItem In vTrail
                Dim pItemParentID = ReadyGuid(pItem("ProductID"))
                If pItemParentID = pParentProductID Then
                    pHasIssue = True
                    Exit For
                End If
            Next

            If pHasIssue Then
                For Each pItem In vTrail
                    pOut &= CStr2(pItem("ProductNo")).Trim() & " - " & CStr2(pItem("ProductName")).Trim() & " > "
                Next
                pOut &= CStr2(vDrProduct("ProductNo")).Trim() & " - " & CStr2(vDrProduct("ProductName")).Trim()
                Return pOut
            End If

            vTrail.Add(vDrProduct)
            Dim pDV = New DataView(vDtOrgProducts, "ParentProductID = " & ReadyString(pParentProductID), "", DataViewRowState.CurrentRows)
            For Each pDRV As DataRowView In pDV
                Dim pTrail = vTrail.ToList()
                pOut = GetParentProductIssues(vDrProduct:=pDRV.Row,
                    vDtOrgProducts:=vDtOrgProducts,
                    vTrail:=pTrail)
                If pOut IsNot Nothing Then
                    Return pOut
                End If
            Next

            Return pOut

        End Function

        Private Function GetChildProducts(vParentProductID As String, vDtOrgProducts As DataTable,
            ByRef oDrConflictingChildProduct As DataRow) As List(Of DataRow)

            Dim pDV As DataView
            Dim pOut As List(Of DataRow)
            Dim pChildren As List(Of DataRow)

            oDrConflictingChildProduct = Nothing
            pOut = New List(Of DataRow)
            pDV = New DataView(vDtOrgProducts, "ParentProductID = " & ReadyString(vParentProductID), "", DataViewRowState.CurrentRows)
            For Each pDRV As DataRowView In pDV
                If ReadyGuid(vParentProductID) = ReadyGuid(pDRV("ChildProductID")) Then
                    oDrConflictingChildProduct = pDRV.Row
                    Return Nothing
                End If
                If pOut.Contains(pDRV.Row) Then
                    oDrConflictingChildProduct = pDRV.Row
                    Return Nothing
                End If
                pOut.Add(pDRV.Row)

                pChildren = GetChildProducts(CStr2(pDRV("ChildProductID")), vDtOrgProducts, oDrConflictingChildProduct)
                If oDrConflictingChildProduct IsNot Nothing Then
                    Return Nothing
                End If
                pOut.AddRange(pChildren)
            Next pDRV

            Return pOut

        End Function
    End Class

    ''' <summary>
    ''' proper entry point
    ''' </summary>
    ''' <param name="vPublishedByID"></param>
    ''' <param name="vProductNumber"></param>
    ''' <param name="vProductName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function AddPim(vPublishedByID As String, vProductNumber As String, vProductName As String,
        Optional vRetailPrice As Decimal = 1,
        Optional vSrp As Decimal = 1,
        Optional vLandedCost As Decimal = 1,
        Optional vUserID As String = modClient.EmployeeWebGuidID)

        Dim pID As String
        Dim pUnitMeasureID As String

        mProData.BeginTransaction()
        Try
            pID = GetOrCreatePimID(vPublishedByID, vProductNumber, vProductName, vUserID)
            AddToOrg(pID)
            pUnitMeasureID = mParent.LibraryStockMeasurements.GetOrCreateUnitMeasureID("PC")
            mParent.LibraryStockMeasurements.GetOrCreatePimProductMeasurement(pID, pUnitMeasureID)
            GetOrCreateRetailPrice(vPublishedByID, pID, vRetailPrice)
            GetOrCreateSrp(vPublishedByID, pID, vSrp)
            GetOrCreateLandedCost(vPublishedByID, pID, vLandedCost)
            mParent.SystemDefaults.UpdateTableLastUpdated("MobizLibraryPIMProduct")
        Catch ex As Exception
            mProData.RollbackTransaction(, True)
            Throw
        End Try
        mProData.CommitTransaction()

        Return pID

    End Function

    ''' <summary>
    ''' this will set IsActive = 1. update LastUpdated and UserID
    ''' </summary>
    ''' <param name="vPublishedByID"></param>
    ''' <param name="vProductNumber"></param>
    ''' <param name="vProductName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function GetOrCreatePimID(vPublishedByID As String, vProductNumber As String, vProductName As String,
        Optional vUserID As String = modClient.EmployeeWebGuidID) As String

        Dim pDT As DataTable
        Dim pID As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        pDT = GetByProductNumberAndPublishedByID(vPublishedByID, vProductNumber)
        pIU = New InsertUpdate("MobizLibraryPIMProduct")
        pIU("ProductName") = ReadyString(vProductName)
        pIU("LastUpdated") = "GETDATE()"
        pIU("UserID") = ReadyString(vUserID)
        pIU("IsActive") = 1
        If pDT.Rows.Count = 0 Then
            pID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pID)
            pIU("ProductNo") = ReadyString(vProductNumber)
            pIU("PublishedByID") = ReadyString(vPublishedByID)
            pIU("DateAdded") = "GETDATE()"
            pSql = pIU.GetInsert()
        Else
            pID = CStr2(pDT.Rows(0)("ID"))
            pSql = pIU.GetUpdate("ID = " & ReadyString(pID))
        End If
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Overridable Function GetByProductNumberAndPublishedByID(vPublishedByID As String, vProductNumber As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibraryPIMProduct.ID" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.PublishedByID = " & ReadyString(vPublishedByID) & vbCrLf &
            " AND MobizLibraryPIMProduct.ProductNo = " & ReadyString(vProductNumber)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function AddToOrg(vProductID As String) As Boolean

        Dim pSql As String
        Dim pDR As DataRow
        Dim pIU As InsertUpdate

        pIU = New InsertUpdate("LibraryOrganizationPIM")
        pSql = " SELECT ID" & vbCrLf &
            ",IsDeactivated" & vbCrLf &
            ",IsBan" & vbCrLf &
            " FROM LibraryOrganizationPIM" & vbCrLf &
            " WHERE LibraryOrganizationPIM.ProductID = " & ReadyString(vProductID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            pIU("ProductID") = ReadyString(vProductID)
            pIU("ActivationDate") = "GETDATE()"
            pSql = pIU.GetInsert()
        Else
            If CBool2(pDR("IsDeactivated")) _
                OrElse CBool2(pDR("IsBan")) Then

                pIU("IsDeactivated") = 0
                pIU("IsBan") = 0
                pIU("LastUpdated") = "GETDATE()"
                pSql = pIU.GetUpdate("ProductID = " & ReadyString(vProductID))
            Else
                Return False
            End If
        End If
        pSql = mProData.ExecuteSQL(pSql) ' return value is only used for debugging

        Return True

    End Function

    Public Function GetOrCreateRetailPrice(vOrgID As String, vProductID As String, vPrice As Decimal) As Boolean

        Dim pRetailPriceID As String
        Dim pSql As String
        Dim pPrice As Decimal
        Dim pIU As InsertUpdate

        pPrice = GetRetailPrice(vProductID, vOrgID) ' todo: create real way to determine if product has price or not
        If pPrice = -1 Then
            pIU = New InsertUpdate("MobizLibraryPIMProductPriceLevel")
            pIU("OrganizationID") = ReadyString(vOrgID)
            pIU("ProductID") = ReadyString(vProductID)
            pRetailPriceID = mParent.SystemDefaults.GetRetailPriceID()
            pIU("PriceLevelID") = ReadyString(pRetailPriceID)
            pIU("Price") = vPrice
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pIU("DeactivateDiscount") = 0
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            Return True
        End If

        Return False

    End Function

    Public Function GetOrCreateSrp(vOrgID As String, vProductID As String, vPrice As Decimal) As Boolean

        Dim pPriceID As String
        Dim pSql As String
        Dim pPrice As Decimal
        Dim pIU As InsertUpdate

        pPrice = GetSrp(vProductID, vOrgID) ' todo: create real way to determine if product has price or not
        If pPrice = -1 Then
            pIU = New InsertUpdate("MobizLibraryPIMProductPriceLevel")
            pIU("OrganizationID") = ReadyString(vOrgID)
            pIU("ProductID") = ReadyString(vProductID)
            pPriceID = mParent.SystemDefaults.GetSrpID()
            pIU("PriceLevelID") = ReadyString(pPriceID)
            pIU("Price") = vPrice
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pIU("DeactivateDiscount") = 0
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            Return True
        End If

        Return False

    End Function

    Public Function GetOrCreateLandedCost(vOrgID As String, vProductID As String, vPrice As Decimal) As Boolean

        Dim pPriceID As String
        Dim pSql As String
        Dim pPrice As Decimal
        Dim pIU As InsertUpdate
        Dim pLastUpdated As Date?

        pPrice = GetLandedCost(vProductID, vOrgID, pLastUpdated) ' todo: create real way to determine if product has price or not
        If pLastUpdated Is Nothing Then
            pIU = New InsertUpdate("MobizLibraryPIMProductPriceLevel")
            pIU("OrganizationID") = ReadyString(vOrgID)
            pIU("ProductID") = ReadyString(vProductID)
            pPriceID = mParent.SystemDefaults.GetLandedCostID()
            pIU("PriceLevelID") = ReadyString(pPriceID)
            pIU("Price") = vPrice
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pIU("DeactivateDiscount") = 0
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
            Return True
        End If

        Return False

    End Function

    Public Function SetLandedCost(vOrgID As String, vProductID As String, vPrice As Decimal) As Boolean

        Dim pPriceID As String
        Dim pSql As String
        Dim pPrice As Decimal
        Dim pIU As InsertUpdate
        Dim pAffectedRecord As Integer

        pIU = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

        pPriceID = mParent.SystemDefaults.GetSrpID()
        pIU("Price") = vPrice
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pIU("DeactivateDiscount") = 0
        pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND PriceLevelID = " & ReadyString(pPriceID))
        pAffectedRecord = mProData.ExecuteSQL(pSql)
        If pAffectedRecord = 0 Then
            pIU("OrganizationID") = ReadyString(vOrgID)
            pIU("ProductID") = ReadyString(vProductID)
            pIU("PriceLevelID") = ReadyString(pPriceID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        Return True


    End Function

    Public Function AddPriceBreak(vOrgID As String, vProductID As String, vUnitMeasureID As String,
        vQty As Decimal, vDiscount As Decimal, vDiscountType As Byte) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("LibraryOrganizationPIMPriceBreak")
        pIU("Discount") = vDiscount
        pIU("DiscountType") = vDiscountType
        pSql = pIU.GetUpdate("ProductID = " & ReadyString(vProductID) &
            " AND UnitMeasureID = " & ReadyString(vUnitMeasureID) &
            " AND Qty = " & vQty)
        If mProData.ExecuteSQL(pSql, True) = 0 Then
            pIU("ProductID") = ReadyString(vProductID)
            pIU("UnitMeasureID") = ReadyString(vUnitMeasureID)
            pIU("Qty") = vQty
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If
        mParent.SystemDefaults.UpdateTableLastUpdated("LibraryOrganizationPIM")

        Return True

    End Function

    Public Function FastQuerySource(vOrgID As String) As DataTable

        Dim pSql As String
        Dim pRetailPriceID As String
        Dim pDT As DataTable

        pRetailPriceID = mParent.SystemDefaults.GetRetailPriceID()

        pSql = " -- PIM fast query source" & vbCrLf &
            " DECLARE @PriceLevelID UNIQUEIDENTIFIER = " & ReadyString(pRetailPriceID) & ";" & vbCrLf &
            " DECLARE @OrgID UNIQUEIDENTIFIER = " & ReadyString(vOrgID) & ";" & vbCrLf &
            " SELECT DISTINCT MobizLibraryPIMProduct.ID AS [ID]" & vbCrLf &
            " , [ProductNo]" & vbCrLf &
            " , [ProductName]" & vbCrLf &
            " , [ProductGroupNo]" & vbCrLf &
            " , ISNULL(MobizLibraryPIMProduct.ProductSearchString, '') AS [ProductSearchString]" & vbCrLf &
            " , pub.CompanyName AS [PublishedBy]" & vbCrLf &
            " , upl.CompanyName AS [UploadedBy]" & vbCrLf &
            " , (SELECT TOP 1 Price" & vbCrLf &
            "  FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "  WHERE PriceLevelID = @PriceLevelID" & vbCrLf &
            "  AND ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            "  AND OrganizationID = @OrgID" & vbCrLf &
            "  ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
            " ) AS [DefaultPrice]" & vbCrLf &
            " ,MobizLibraryPIMProduct.IsBrandOwner" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurementBarcode.ReadyBarcode" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN LibraryOrganizationPIM orgpim ON MobizLibraryPIMProduct.ID = orgpim.ProductID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization pub ON MobizLibraryPIMProduct.PublishedByID = pub.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization upl ON MobizLibraryPIMProduct.UploadedByID = upl.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProductMeasurementBarcode ON MobizLibraryPIMProductMeasurement.ID = MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.IsActive = 1 AND orgpim.IsDeactivated = 0 AND orgpim.ProductAvailableToMobile = 1 " & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.IsBrandOwner DESC" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf
        pDT = mProData.Execute(pSql)

#If DEBUG Then
        'pDT.DefaultView.RowFilter = "ID = " & ReadyString("420950da-e7fa-4162-8db4-a841bbceeea2")

        'If pDT.DefaultView.Count > 0 Then
        '    For pIndex As Integer = 1 To 1000000 - pDT.Rows.Count
        '        Dim pDrNew As DataRow = pDT.NewRow
        '        For Each pDC As DataColumn In pDT.Columns
        '            pDrNew(pDC.ColumnName) = pDT.DefaultView(0)(pDC.ColumnName)
        '        Next pDC
        '        pDrNew("ProductName") = Format(pIndex, "000,000,000") & " " & pDT.DefaultView(0)("ProductName")
        '        pDT.Rows.Add(pDrNew)
        '    Next pIndex
        'End If
        'pDT.DefaultView.RowFilter = ""
#End If

        Return pDT

    End Function

    Public Function GetIDByPrice(vOrgID As String, vPrice As Decimal, vPriceLevelID As String) As String

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " -- find product with same price" & vbCrLf &
            " DECLARE @PriceLevelID UNIQUEIDENTIFIER" & vbCrLf &
            " DECLARE @OrgID UNIQUEIDENTIFIER" & vbCrLf &
            " DECLARE @Price MONEY" & vbCrLf &
            " SET @PriceLevelID = " & ReadyString(vPriceLevelID) & vbCrLf &
            " SET @OrgID = " & ReadyString(vOrgID) & vbCrLf &
            " SET @Price = " & vPrice & vbCrLf
        pSql &= " SELECT TOP 10 LibraryOrganizationPIM.ProductID" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            " FROM LibraryOrganizationPIM" & vbCrLf &
            " JOIN MobizLibraryPIMProductPriceLevel ON MobizLibraryPIMProductPriceLevel.ProductID = LibraryOrganizationPIM.ProductID" & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.OrganizationID = @OrgID" & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.PriceLevelID = @PriceLevelID" & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.Price = @Price" & vbCrLf &
            " ORDER BY MobizLibraryPIMProductPriceLevel.Price"
        pDR = mProData.ExecuteRow(pSql)

        Return CStr2(pDR("ProductID"))

    End Function

    Public Function DeactivateAllProducts(vUserID As String) As Integer

        Dim pOut As Integer
        Dim pSql As String

        mProData.BeginTransaction()
        pSql = "UPDATE LibraryOrganizationPIM" & vbCrLf &
             " SET IsDeactivated = 1" & vbCrLf &
             ",DeactivationDate = GETDATE()" & vbCrLf &
             ",DeactivationReason = 'Deactivate all products'" & vbCrLf &
             " WHERE IsDeactivated = 0"
        pOut = mProData.ExecuteSQL(pSql, True)

        pSql = "Update TablesLastUpdated set LastUpdated = GETDATE() where TableName = 'LibraryOrganizationPIM'"

        mProData.ExecuteSQL(pSql)

#If DEBUG Then
        mProData.CommitTransaction()
        'mProData.RollbackTransaction() ' rollback for debugging
#Else
        mProData.CommitTransaction()
#End If

        Return pOut

    End Function

    Public Function ReactivateAllProducts(vUserID As String) As Integer

        Dim pOut As Integer
        Dim pSql As String

        mProData.BeginTransaction()
        pSql = "UPDATE LibraryOrganizationPIM" & vbCrLf &
             " SET IsDeactivated = 0" & vbCrLf &
             ",DeactivationDate = GETDATE()" & vbCrLf &
             ",DeactivationReason = ''" & vbCrLf &
             " WHERE IsDeactivated = 1"
        pOut = mProData.ExecuteSQL(pSql, True)

        pSql = "Update TablesLastUpdated set LastUpdated = GETDATE() where TableName = 'LibraryOrganizationPIM'"

        mProData.ExecuteSQL(pSql)

#If DEBUG Then
        mProData.CommitTransaction()
        'mProData.RollbackTransaction() ' rollback for debugging
#Else
        mProData.CommitTransaction()
#End If

        Return pOut

    End Function

    Public Function GetCategory(vProductID As String) As String

        Dim pSql As String

        pSql = " SELECT DISTINCT MobizLibraryPIMProductVariableFeatures001.C0020 AS [Category]" & vbCrLf &
            " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            " WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = " & ReadyString(vProductID) & vbCrLf
        pSql = CStr2(mProData.ExecuteScalar(pSql))

        Return pSql

    End Function

    Public Function GetMinGpmByCategory(vCategory As String) As Decimal?

        Dim pSql As String
        Dim pDR As DataRow

        pSql = "SELECT Gpm" & vbCrLf &
            " FROM LibraryPimCategoryGpm" & vbCrLf &
            " WHERE Category = " & ReadyString(vCategory)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return pDR("Gpm")
        End If

    End Function

    Public Function GetFirstUnit(vID As String) As DataRow

        Dim pSql = " SELECT DISTINCT TOP 100 PERCENT" & vbCrLf &
            " MobizLibraryPIMProductMeasurement.MeasurementLevel" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            " ,MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            " JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " WHERE MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(vID) & vbCrLf &
            " ORDER BY MobizLibraryPIMProductMeasurement.Quantity, MobizLibraryPIMProductMeasurement.MeasurementLevel"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            If CDec2(pDR("Quantity")) = 1 Then
                Return pDR
            End If
        Next

        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetUnit(vID As String, vUnitMeasureID As String) As DataRow

        Dim pSql = " SELECT DISTINCT TOP 1" & vbCrLf &
            " MobizLibraryPIMProductMeasurement.MeasurementLevel" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            " ,MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            " JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " WHERE MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(vID) & vbCrLf &
            " AND MobizLibraryPIMProductMeasurement.UnitMeasureID = " & ReadyString(vUnitMeasureID) & vbCrLf &
            " ORDER BY MobizLibraryPIMProductMeasurement.MeasurementLevel"
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function GetUnitsNew(vID As String) As DataTable

        Dim pSql = " SELECT " & vbCrLf &
            " MobizLibraryPIMProductMeasurement.MeasurementLevel" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            " ,MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            " JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " WHERE MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(vID) & vbCrLf &
            " ORDER BY MobizLibraryPIMProductMeasurement.MeasurementLevel"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetBrands() As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT MobizLibraryPIMProduct.Brand" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.IsActive = 1" & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.Brand"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function GetProductCategories() As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT MobizLibraryPIMProductVariableFeatures001.C0020 AS [Category]" & vbCrLf &
            " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            " JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct on MobizLibraryPIMProduct.ID = MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.IsActive = 1" & vbCrLf &
            " ORDER BY Category"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function SetPrice(vProductID As String, vPricelevelID As String, vPrice As Decimal, vOrgID As String)

        Dim pIU = New InsertUpdate("MobizLibraryPIMProductPriceLevel")

        pIU("Price") = vPrice
        pIU("LastUpdated") = "GETDATE()"
        Dim pSql = pIU.GetUpdate("PriceLevelID = " & ReadyString(vPricelevelID) & vbCrLf &
            " AND OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID))
        Dim pAffected = mProData.ExecuteSQL(pSql)
        If pAffected = 0 Then
            pIU("ID") = "NEWID()"
            pIU("PriceLevelID") = ReadyString(vPricelevelID)
            pIU("OrganizationID") = ReadyString(vOrgID)
            pIU("ProductID") = ReadyString(vProductID)
            pIU("DeactivateDiscount") = "0"
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Function

    Public Function GetByID(vID) As DataRow

        Dim pSql = "SELECT UploadedByID" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

#Region "ParseProductDescription"
    Public Function CreateProductDescriptionParser() As ProductDescriptionParser

        Dim pOut = New ProductDescriptionParser

        Return pOut

    End Function

    Public Class ProductDescriptionParser
        Public Class Output
            Public ProductDescription As String
            Public BasicUnit As NumericUnit
            Public UnitQty As Decimal = 1
        End Class

        Public Function ParseProductDescription(vRaw As String) As Output

            Dim pOut = New Output()
            Dim pLastSlashIndex As Integer

            Try
                If String.IsNullOrWhiteSpace(vRaw) Then
                    Return pOut
                End If

                vRaw = LeanString(vRaw)
                '// find if has slash
                pLastSlashIndex = vRaw.LastIndexOf("/", StringComparison.Ordinal)
                If pLastSlashIndex = -1 Then
                    pOut.ProductDescription = vRaw
                    Return pOut
                End If

                Dim pSegment As String

                pSegment = vRaw.Substring(pLastSlashIndex + 1)
                Dim pBasicUnit = ExtractNumericUnit(pSegment)
                If pBasicUnit Is Nothing Then
                    pOut.ProductDescription = vRaw
                ElseIf pBasicUnit.Value = 0 Then
                    pOut.ProductDescription = vRaw
                    pBasicUnit = Nothing
                Else
                    pOut.ProductDescription = Trim(vRaw.Substring(0, pLastSlashIndex)) & " " & Trim(pSegment)
                    Do
                        pSegment = vRaw.Substring(0, pLastSlashIndex)
                        Dim pLastWord = LastWord(pSegment)
                        If String.IsNullOrWhiteSpace(pLastWord.Word) = False _
                            AndAlso pLastWord.Index <> 0 _
                            AndAlso IsNumeric(pLastWord.Word) Then

                            pLastSlashIndex = pLastWord.Index
                            If pOut.UnitQty = 0 Then
                                pOut.UnitQty = 1
                            End If
                            pOut.UnitQty *= CDec2(pLastWord.Word)
                            pOut.ProductDescription = vRaw.Substring(0, pLastSlashIndex).Trim() & " " & pBasicUnit.Unit
                        Else
                            Exit Do
                        End If
                    Loop
                End If
                pOut.BasicUnit = pBasicUnit

                Return pOut
            Catch ex As Exception
                Throw
            End Try

        End Function

        Public Class NumericUnit
            Public Value As Decimal
            Public Unit As String
            Public FreeValue As Decimal
        End Class

        Private Enum eFindWhat
            FirstNumericChar
            DecimalPointOrNumericChar
            NumericChar

            UnitOrPlusFreeChar
            FirstPlusFreeNumericChar
            PlusFreeNumericChar
            FreeChar
        End Enum

        Private Function ExtractNumericUnit(vRaw As String) As NumericUnit

            Dim pOut = New NumericUnit()
            Dim pNumberString As String
            Dim pFindWhat As eFindWhat
            Dim pFreeChars As String

            pOut.Unit = CStr2(vRaw).Trim()
            For pCurrentIndex As Integer = 0 To vRaw.Length - 1
                Dim pCurrentChar As String = vRaw.Substring(pCurrentIndex, 1)

                If pFindWhat = eFindWhat.FirstNumericChar Then
                    If IsNumeric(pCurrentChar) Then
                        pFindWhat = eFindWhat.DecimalPointOrNumericChar
                        pNumberString = pCurrentChar
                    ElseIf String.IsNullOrWhiteSpace(pCurrentChar) = False Then
                        Return pOut
                    End If
                ElseIf pFindWhat = eFindWhat.DecimalPointOrNumericChar Then
                    If IsNumeric(pCurrentChar) Then
                        pNumberString &= pCurrentChar
                    ElseIf pCurrentChar = "." Then
                        pNumberString &= pCurrentChar
                        pFindWhat = eFindWhat.NumericChar
                    Else
                        pFindWhat = eFindWhat.UnitOrPlusFreeChar
                        pOut.Value = pNumberString
                    End If
                ElseIf pFindWhat = eFindWhat.NumericChar Then
                    If IsNumeric(pCurrentChar) Then
                        pNumberString &= pCurrentChar
                    Else
                        pOut.Value = pNumberString
                        pFindWhat = eFindWhat.UnitOrPlusFreeChar
                    End If
                End If

                If pFindWhat = eFindWhat.UnitOrPlusFreeChar Then
                    If pCurrentChar = "+" Then
                        pFreeChars = pCurrentChar
                        pFindWhat = eFindWhat.FirstPlusFreeNumericChar
                        'ElseIf String.IsNullOrWhiteSpace(pCurrentChar) = False Then
                    End If
                ElseIf pFindWhat = eFindWhat.FirstPlusFreeNumericChar Then
                    If String.IsNullOrWhiteSpace(pCurrentChar) Then
                        pFreeChars &= pCurrentChar
                    ElseIf IsNumeric(pCurrentChar) Then
                        pFreeChars &= pCurrentChar
                        pNumberString = pCurrentChar
                        pFindWhat = eFindWhat.PlusFreeNumericChar
                    Else
                        pFindWhat = eFindWhat.UnitOrPlusFreeChar
                        pFreeChars = Nothing
                    End If
                ElseIf pFindWhat = eFindWhat.PlusFreeNumericChar Then
                    If IsNumeric(pCurrentChar) Then
                        pFreeChars &= pCurrentChar
                        pNumberString = pCurrentChar
                    Else
                        pFindWhat = eFindWhat.FreeChar
                    End If
                End If

                If pFindWhat = eFindWhat.FreeChar Then
                    If String.IsNullOrWhiteSpace(pCurrentChar) Then
                        pFreeChars &= pCurrentChar
                    ElseIf pCurrentChar.ToLower() = "f" Then
                        If (vRaw & Space(2)).Substring(pCurrentIndex, 2).Trim().ToLower() = "f" _
                            OrElse (vRaw & Space(4)).Substring(pCurrentIndex, 4).ToLower() = "free" Then

                            pOut.FreeValue = pNumberString
                            pFreeChars = Nothing
                        End If
                    End If
                End If

            Next pCurrentIndex

            Select Case pFindWhat
                Case eFindWhat.FreeChar,
                    eFindWhat.PlusFreeNumericChar

                    pOut.FreeValue = pNumberString
                    pFreeChars = Nothing

            End Select

            Return pOut

        End Function

        Public Class LastWordOutput
            Public Word As String
            Public Index As Integer
        End Class

        Public Shared Function LastWord(vText As String) As LastWordOutput

            Dim pOut As New LastWordOutput
            Dim pHasWordChar As Boolean
            Dim pEndIndex As Integer

            If String.IsNullOrWhiteSpace(vText) Then
                Return pOut
            End If
            pEndIndex = vText.Length - 1
            For pIndex = vText.Length - 1 To 0 Step -1
                Dim pChar = vText.Substring(pIndex, 1)
                If pHasWordChar Then
                    If String.IsNullOrWhiteSpace(pChar) _
                        OrElse pChar = "/" Then

                        pOut.Index = pIndex + 1
                        pOut.Word = vText.Substring(pOut.Index, pEndIndex - pIndex)
                        Return pOut
                    End If
                Else
                    If String.IsNullOrWhiteSpace(pChar) _
                        OrElse pChar = "/" Then
                        pEndIndex = pIndex - 1
                    Else
                        pEndIndex = pIndex
                        pHasWordChar = True
                    End If
                End If
            Next pIndex

            pOut.Word = vText.Substring(0, pEndIndex + 1)
            Return pOut

        End Function

        Public Shared Function LeanString(vText As String) As String

            Dim pOut As String = ""
            Dim pPreviousChar As Char
            Const pAlphaNumeric As String = "abcdefghijklmnopqrstuvwxyz1234567890"
            Dim pLastAddIsAlphaNumeric As Boolean

            vText = CStr2(vText).Trim()
            For Each pChar In vText
                If String.IsNullOrWhiteSpace(pChar) Then
                    pChar = " "
                End If
                If pAlphaNumeric.Contains(pChar.ToString().ToLower()) _
                    AndAlso String.IsNullOrWhiteSpace(pPreviousChar) Then

                    ' this char is alphanumeric and the previous char is a space
                    ' so add the space and this char
                    If pLastAddIsAlphaNumeric Then
                        pOut &= " " & pChar
                    Else
                        pOut &= pChar
                    End If
                    pLastAddIsAlphaNumeric = True
                ElseIf pAlphaNumeric.Contains(pChar.ToString().ToLower()) = False _
                    AndAlso String.IsNullOrWhiteSpace(pChar) = False Then

                    pOut &= pChar
                    pLastAddIsAlphaNumeric = False
                ElseIf pAlphaNumeric.Contains(pChar.ToString().ToLower()) Then
                    pOut &= pChar
                    pLastAddIsAlphaNumeric = True
                End If

                pPreviousChar = pChar
            Next

            Return pOut

        End Function
    End Class ' Public Class ProductDescriptionParser
#End Region ' END ParseProductDescription

    Public Sub UpdateDiscount(vProductID As String, vUnitPrice As Decimal, vUnitQty As Decimal, vOrgID As String)

        Dim pRetailPrice = GetRetailPrice(vProductID, vOrgID)
        If pRetailPrice * vUnitQty = 0 Then
            Return
        End If
        Dim pDiscountPercent = vUnitPrice / (pRetailPrice * vUnitQty)
        pDiscountPercent = (1 - pDiscountPercent) * 100
        Dim pIU = New InsertUpdate("MobizLibraryPIMProductMeasurement")
        pIU("Discount") = pDiscountPercent
        pIU("LastUpdated") = "GETDATE()"
        Dim pSql = pIU.GetUpdate("ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND Quantity = " & vUnitQty & vbCrLf &
            " AND Discount <> " & pDiscountPercent)
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

    End Sub

    Public Class UpdateSerialsInput
        Public InvoiceID As String
        Public ProductID As String
        Public Serials As List(Of SerialClass)
        Public Imeis As List(Of SerialClass)

        Public Class SerialClass
            Public Serial As String
            Public Imei As String
            Public IsDelete As String
        End Class
    End Class

    Public Sub UpdateSerials(vInput As UpdateSerialsInput)

        mProData.BeginTransaction()
        Dim pSql = "DELETE FROM TransInvoiceItemSerials" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInput.InvoiceID) & vbCrLf &
            " AND ProductID = " & ReadyString(vInput.ProductID)
        mProData.ExecuteSQL(pSql)

        For Each pSerial In vInput.Serials
            If Not CBool2(pSerial.IsDelete) Then
                Dim pIU = New InsertUpdate("TransInvoiceItemSerials")
                pIU("InvoiceID") = ReadyString(vInput.InvoiceID)
                pIU("ProductID") = ReadyString(vInput.ProductID)
                pIU("SerialNumber") = ReadyString(pSerial.Serial)
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            End If
        Next

        For Each pImei In vInput.Imeis
            If Not CBool2(pImei.IsDelete) Then
                Dim pIU = New InsertUpdate("TransInvoiceItemSerials")
                pIU("InvoiceID") = ReadyString(vInput.InvoiceID)
                pIU("ProductID") = ReadyString(vInput.ProductID)
                pIU("Imei") = ReadyString(pImei.Imei)
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            End If
        Next

        mProData.CommitTransaction()

    End Sub

    Public Function GetSerials(vInvoiceID As String, vProductID As String) As DataTable

        Dim pSql = "SELECT SerialNumber as Serial, Imei, 'False' as IsDelete " & vbCrLf &
            " FROM TransInvoiceItemSerials" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID)
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function DeleteColumnMappings(vOrgID As String) As Integer

        mProData.BeginTransaction()
        Dim pSql = "DELETE FROM MobizLibraryPIMTemplateColumns" & vbCrLf &
            " WHERE PIMTemplateID IN (" & vbCrLf &
            "   SELECT ID" & vbCrLf &
            "   FROM MobizLibraryPIMTemplate" & vbCrLf &
            "   WHERE OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            ")"
        Dim pAffectedCount = mProData.ExecuteSQL(pSql)

        pSql = "DELETE FROM MobizLibraryPIMTemplate" & vbCrLf &
            "   WHERE OrganizationID = " & ReadyString(vOrgID)
        pAffectedCount += mProData.ExecuteSQL(pSql)
        mProData.CommitTransaction()
        'mProData.RollbackTransaction() ' debug only

        Return pAffectedCount

    End Function

    Public Function CacheCommittedQty(vProductID As String, vFowstID As String) As Decimal

        Dim pSql = "SELECT SUM(OrderedQuantity) Quantity FROM TransInvoice t1 " & vbCrLf &
            "inner join TransInvoiceItems on t1.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
            "WHERE TransType = 'SalesOrder' " & vbCrLf &
            " AND ISNULL(t1.IsCancelled, 0) = 0" & vbCrLf &
            " AND NOT EXISTS (SELECT * FROM TransInvoice t2 " & vbCrLf &
            "WHERE invoicenumber > 0 " & vbCrLf &
            "AND t2.SalesOrderID = t1.ID)  " & vbCrLf &
            "AND TransInvoiceItems.ProductID = " & ReadyString(vProductID)
        pSql &= "AND t1.FOWSTID = " & ReadyString(vFowstID)
        pSql &= " GROUP BY t1.FOWSTID"
        Dim pOut As Decimal = CDec2(mProData.ExecuteScalar(pSql))

        Dim pIU = New InsertUpdate("CacheFowstProducts")
        pIU("CommittedQty") = pOut
        pSql = pIU.GetUpdate("ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID))
        Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
        If pAffectedRecords = 0 Then
            pIU("ProductID") = ReadyString(vProductID)
            pIU("FowstID") = ReadyString(vFowstID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        Return pOut

    End Function

    Public Function GetCachedCommittedQty(vProductID As String, vFowstID As String) As DataTable

        Dim pSql = "SELECT CommittedQty" & vbCrLf &
            ",FowstID" & vbCrLf &
            " FROM CacheFowstProducts" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID) & vbCrLf
        If String.IsNullOrWhiteSpace(vFowstID) = False Then
            pSql &= " AND FowstID = " & ReadyString(vFowstID)
        End If
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function PriceLevelsPimTemplate(vOrgID As String, vNotIn As Boolean) As DataTable

        Dim pSql = " -- price levels not in PIM features" & vbCrLf &
            "  SELECT MobizLibraryPIMPriceLevel.ID" & vbCrLf &
            "  ,MobizLibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            "  ,tblTemplate.ColumnName" & vbCrLf &
            "  ,tblTemplate.PIMFeatureID" & vbCrLf &
            "  FROM MobizLibraryPIMPriceLevel" & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            "  	SELECT MobizLibraryPIMTemplateColumns.PIMFeatureID" & vbCrLf &
            "     ,MobizLibraryPIMTemplateColumns.ColumnName" & vbCrLf &
            "     FROM MobizLibraryPIMTemplateColumns" & vbCrLf &
            "     JOIN MobizLibraryPIMTemplate ON MobizLibraryPIMTemplate.ID = MobizLibraryPIMTemplateColumns.PIMTemplateID" & vbCrLf &
            "     WHERE MobizLibraryPIMTemplateColumns.PIMFeatureID = MobizLibraryPIMPriceLevel.ID" & vbCrLf &
            " --    AND MobizLibraryPIMTemplateColumns.ColumnName = MobizLibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            " 	AND MobizLibraryPIMTemplate.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            "  ) tblTemplate" & vbCrLf &
            "  WHERE (MobizLibraryPIMPriceLevel.OrgID = " & ReadyString(vOrgID) & vbCrLf &
            "  OR MobizLibraryPIMPriceLevel.OrgID IS NULL)" & vbCrLf &
            "  AND MobizLibraryPIMPriceLevel.IsActive = 1" & vbCrLf &
            "  AND tblTemplate.ColumnName IS " & If(vNotIn, "", "NOT ") & " NULL" & vbCrLf &
            " UNION" & vbCrLf &
            "   SELECT CASE MobizLibraryPIMTemplateColumns.PIMFeatureID" & vbCrLf &
            "   WHEN '1eb487fc-8b91-4d0c-901f-16bf4b80a9a3' THEN '01000012-0003-0000-0000-000000000000' -- landed cost" & vbCrLf &
            "   WHEN 'C5BD7B11-369D-404A-AD18-C53566744C90' THEN '01000012-0005-0000-0000-000000000000' -- retail price" & vbCrLf &
            "   WHEN 'C5BD7B11-369D-404A-AD18-C53566744C99' THEN 'eef716c6-b290-4b28-b2a6-0a4f52cf4989' -- SRP" & vbCrLf &
            "   END AS ID" & vbCrLf &
            "   ,NULL AS PriceLevel" & vbCrLf &
            "   ,MobizLibraryPIMTemplateColumns.ColumnName" & vbCrLf &
            "   ,MobizLibraryPIMTemplateColumns.PIMFeatureID" & vbCrLf &
            "   FROM MobizLibraryPIMTemplateColumns" & vbCrLf &
            "   JOIN MobizLibraryPIMTemplate ON MobizLibraryPIMTemplate.ID = MobizLibraryPIMTemplateColumns.PIMTemplateID" & vbCrLf &
            "   WHERE MobizLibraryPIMTemplateColumns.PIMFeatureID IN (" & vbCrLf &
            "       '1eb487fc-8b91-4d0c-901f-16bf4b80a9a3'  -- landed cost" & vbCrLf &
            "       ,'C5BD7B11-369D-404A-AD18-C53566744C90' -- retail price" & vbCrLf &
            "       ,'C5BD7B11-369D-404A-AD18-C53566744C99' -- SRP" & vbCrLf &
            "   ) AND MobizLibraryPIMTemplate.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            "  ORDER BY MobizLibraryPIMPriceLevel.PriceLevel"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function AddPricelLevelsToPimTemplate(vOrgID As String, vUserID As String) As Integer

        mProData.BeginTransaction()
        Dim pDT = PriceLevelsPimTemplate(vOrgID, True)
        If pDT.Rows.Count = 0 Then
            mProData.RollbackTransaction()
            Return 0
        End If
        Dim pTemplateID = CreatePimTemplate(vOrgID, vUserID)

        Dim pIU = New InsertUpdate("MobizLibraryPIMTemplateColumns")
        pIU("ID") = "NEWID()"
        pIU("FeatureType") = ReadyString("Textbox (Decimal)")
        pIU("PIMTemplateID") = ReadyString(pTemplateID)
        For Each pDR As DataRow In pDT.Rows
            If pDR("ID") Is DBNull.Value Then
                Continue For
            End If
            If String.IsNullOrWhiteSpace(CStr2(pDR("PriceLevel"))) Then
                Continue For
            End If
            pIU("ColumnName") = ReadyString(pDR("PriceLevel"))
            pIU("PIMFeatureID") = ReadyString(pDR("ID"))
            Dim pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next
        mProData.CommitTransaction()

    End Function

    Private Function CreatePimTemplate(vOrgID As String, vUserID As String) As String

        Dim pID As String = Guid.NewGuid().ToString()
        Dim pIU As New InsertUpdate("MobizLibraryPIMTemplate")
        pIU("ID") = ReadyString(pID)
        pIU("TemplateName") = ReadyString("New Template" & GenerateString(6, "0123456789"))
        pIU("OrganizationID") = ReadyString(vOrgID)
        pIU("UserID") = ReadyString(vUserID)
        pIU("LastUpdated") = "GETDATE()"
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Private Shared mDeleteDuplicateProductCategoriesLock As New Object

    Public Function DeleteDuplicateProductCategories() As Integer

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String

        SyncLock mDeleteDuplicateProductCategoriesLock
            pSql = "SELECT MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
                " ,COUNT(*) AS Count" & vbCrLf &
                " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
                " GROUP BY MobizLibraryPIMProductVariableFeatures001.ProductID" & vbCrLf &
                " HAVING count(*) >= 2" & vbCrLf
            pDT = mProData.Execute(pSql)
            For Each pDR In pDT.Rows
                pSql = "SELECT TOP " & pDR("Count") - 1 & " ID" & vbCrLf &
                    " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
                    " WHERE ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                    " ORDER BY LEN(MobizLibraryPIMProductVariableFeatures001.C0020)" & vbCrLf
                pSql = "DELETE FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
                    " WHERE ID IN (" & pSql & ")" & vbCrLf
                Dim pAffected = mProData.ExecuteSQL(pSql) ' output is for debugging only
            Next pDR

            Return pDT.Rows.Count
        End SyncLock

    End Function

    Public Sub ChangeProductArea(vProductID As String, vFowstID As String, vArea As String, vUpdateAllFowst As Boolean)

        Dim pFowstBinID = GetOrAddFowstBin(vFowstID, vArea)
        Dim pIU As New InsertUpdate("LibraryFowstBinProduct")
        pIU("BinID") = ReadyString(pFowstBinID)
        Dim pSql = " ProductID = " & ReadyString(vProductID) & vbCrLf
        If vUpdateAllFowst = False Then
            pSql &= " AND FowstID = " & ReadyString(vFowstID) & vbCrLf
        End If
        pSql = pIU.GetUpdate(pSql)
        Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
        If pAffectedRecords = 0 Then
            pIU("FowstID") = ReadyString(vFowstID)
            pIU("ProductID") = ReadyString(vProductID)
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Private Function GetOrAddFowstBin(vFowstID As String, vArea As String) As String

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM LibraryFowstBin" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND Area = " & ReadyString(vArea)
        Dim pDR As DataRow = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return CStr2(pDR("ID"))
        End If

        Dim pID = Guid.NewGuid().ToString()
        Dim pIU = New InsertUpdate("LibraryFowstBin")
        pIU("ID") = ReadyString(pID)
        pIU("Area") = ReadyString(vArea)
        pIU("FowstID") = ReadyString(vFowstID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function GetAreas(vFowstID As String) As DataTable

        Dim pSql = "SELECT DISTINCT Area" & vbCrLf &
            " FROM LibraryFowstBin" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND LTRIM(RTRIM(ISNULL(Area, ''))) <> ''" & vbCrLf &
            " ORDER BY Area"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Shared ReadOnly mImportProductAreaFromErpLock As New Object()
    Private Shared mImportProductAreaFromErpBusy As Boolean

    Public Function ImportProductAreaFromErp() As Integer

        If mImportProductAreaFromErpBusy Then
            Return -1
        End If

        SyncLock mImportProductAreaFromErpLock
            mImportProductAreaFromErpBusy = True
            Try
                Dim pWarehouse As String
                Dim pWarehouseActiveProductCount As Integer
                Dim pStartImporting As Boolean
                Dim pSettingsFilename = Common.ServerPath & "settings\ImportProductAreaFromErp.txt"
                CreateFolder(pSettingsFilename)
                Dim pSettingsProductID = FileText(pSettingsFilename)
                If IsGuid(pSettingsProductID) Then
                    pSettingsProductID = ReadyGuid(pSettingsProductID)
                Else
                    pSettingsProductID = ""
                    pStartImporting = True
                End If
#If DEBUG Then
                Dim pErpProData = ProData.CreateFromConnectionInfo("(local)", "wholesale", "sa", "abc123", False)
#Else
        Dim pErpProData = ProData.CreateFromConnectionInfo("192.168.15.254", "wholesale", "sa", "ducmain", False)
#End If
                Dim pSql = " SELECT DISTINCT TOP 100 PERCENT LibraryStockInformation.ID" & vbCrLf &
                    " ,LibraryStockInformation.StockNumber" & vbCrLf &
                    " ,LibraryWarehouses.WarehouseName" & vbCrLf &
                    " ,LibraryWarehouses.Location" & vbCrLf &
                    " FROM LibraryStockInformation" & vbCrLf &
                    " JOIN LibraryWarehouses ON LibraryWarehouses.ID = LibraryStockInformation.WarehouseID" & vbCrLf &
                    " WHERE LibraryWarehouses.WarehouseName <> 'NONE'" & vbCrLf &
                    " AND LibraryWarehouses.Location IN (" & vbCrLf &
                    " 	'Alvarez St'," & vbCrLf &
                    "     'Office'," & vbCrLf &
                    "     'pronto'," & vbCrLf &
                    "     'Sta. Ana'," & vbCrLf &
                    "     'TPT'" & vbCrLf &
                    " )" & vbCrLf &
                    " ORDER BY LibraryWarehouses.WarehouseName" & vbCrLf &
                    " ,LibraryStockInformation.StockNumber" & vbCrLf &
                    ",LibraryStockInformation.ID"
                Dim pDtSource = pErpProData.Execute(pSql)
                pDtSource.Columns.Add("Deactivated", GetType(Boolean))
                For Each pDrSource As DataRow In pDtSource.Rows
                    If pStartImporting = False _
                        AndAlso String.IsNullOrWhiteSpace(pSettingsProductID) = False _
                        AndAlso ReadyGuid(pDrSource("ID")) = pSettingsProductID Then

                        pStartImporting = True
                    End If
                    If pStartImporting = False Then
                        Continue For
                    End If
                    If ReadyGuid(pDrSource("ID")) = ReadyGuid("CCF48794-DBAD-45C3-9BAA-0F1749F83129") _
                        OrElse ReadyGuid(pDrSource("ID")) = ReadyGuid("0A46BE14-14F2-4801-BDBA-2BAEBB5FBED1") _
                        OrElse ReadyGuid(pDrSource("ID")) = ReadyGuid("B1981A78-16C2-440C-BFF1-5E8BE9A1ADB5") _
                        OrElse ReadyGuid(pDrSource("ID")) = ReadyGuid("D0B863A3-6032-45ED-93CF-66E7441504D5") _
                        OrElse ReadyGuid(pDrSource("ID")) = ReadyGuid("AEF05E41-3EBB-4E8C-90AA-8D94789B9641") _
                        OrElse ReadyGuid(pDrSource("ID")) = ReadyGuid("B80A2B23-01AF-46EF-8E97-F4716742E759") _
                        OrElse ReadyGuid(pDrSource("ID")) = ReadyGuid("DDB59C1B-1645-405A-8881-521A17F4F89F") Then
                        Debug.WriteLine("ref: vbASND9HHK6D!")
                    End If
                    If pWarehouse Is Nothing _
                        OrElse pWarehouse <> CStr2(pDrSource("WarehouseName")) Then

                        pWarehouse = CStr2(pDrSource("WarehouseName"))
                        pWarehouseActiveProductCount = 0
                    End If
                    'If pWarehouseActiveProductCount >= 50 Then
                    '    Continue For
                    'End If
                    pSql = "SELECT ISNULL(IsDeActivated, 0) AS IsDeActivated" & vbCrLf &
                        " FROM LibraryOrganizationPIM" & vbCrLf &
                        " WHERE ProductID = " & ReadyString(pDrSource("ID"))
                    pDrSource("Deactivated") = CBool2(mProData.ExecuteScalar(pSql))
                    If pDrSource("Deactivated") Then
                        Continue For
                    End If
                    pWarehouseActiveProductCount += 1

                    ChangeProductArea(CStr2(pDrSource("ID")), "4ecf91b3-d51a-468c-b3d2-b6e979356739", CStr2(pDrSource("WarehouseName")), False) ' fowstid 4ecf91b3-d51a-468c-b3d2-b6e979356739 = DUC SALES OFFICE IN TPT STA ANA
                    FileText(pSettingsFilename) = ReadyGuid(pDrSource("ID"))
                Next

                TableToExcelFile(pDtSource, "", "", Common.ServerPath & "temp\logs\ImportProductAreaFromErp_" & DateTimeStamp() & ".xls")

                Return pDtSource.Rows.Count
            Finally
                mImportProductAreaFromErpBusy = False
            End Try

        End SyncLock

    End Function

    Public Function Brands() As DataTable

        Dim pSql = " SELECT DISTINCT MobizLibraryPIMProduct.Brand" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.Brand"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Class SimilarBrandsResult

        Public Items As New ItemsClass()

        Public Class ItemsClass
            Inherits List(Of ItemClass)

            Public Function Add(vBrand As String) As ItemClass

                Dim pOut = New ItemClass
                pOut.Brand = vBrand
                MyBase.Add(pOut)

                Return pOut

            End Function
        End Class

        Public Class ItemClass
            Public Brand As String

            Public Duplicates As New ItemsClass

            Public Sub New()

            End Sub

            Public Sub New(vBrand As String)

                Brand = vBrand

            End Sub
        End Class
    End Class

    Public Shared Function GetSimilarBrands(vDT As DataTable) As SimilarBrandsResult

        Dim pOut = New SimilarBrandsResult()
        Dim pCurrentResult As SimilarBrandsResult.ItemClass
        Dim pAllBrands As New List(Of String)

        For pIndex1 = 0 To vDT.Rows.Count - 2
            If pAllBrands.Contains(CStr2(vDT.Rows(pIndex1)("Brand"))) Then
                Continue For
            End If
            pCurrentResult = Nothing
            For pIndex2 = pIndex1 + 1 To vDT.Rows.Count - 1
                Dim pBrand1 = CStr2(vDT.Rows(pIndex1)("Brand"))
                Dim pBrand2 = CStr2(vDT.Rows(pIndex2)("Brand"))
                Dim pDistancePercent = LibraryOrganization.LevenshteinDistancePercent(pBrand1, pBrand2)
                If pDistancePercent <= 10 Then
                    If pCurrentResult Is Nothing Then
                        pCurrentResult = pOut.Items.Add(pBrand1)
                        pAllBrands.Add(pBrand1)
                    End If
                    pCurrentResult.Duplicates.Add(pBrand2)
                    pAllBrands.Add(pBrand2)
                End If
            Next pIndex2
        Next pIndex1

        Return pOut

    End Function

    Public Function MergeBrands(vInput As SimilarBrandsResult) As Integer

        Dim pAffectedRecords As Integer

        For Each pItem In vInput.Items
            Dim pDuplicates = ""
            For Each pDuplicate In pItem.Duplicates
                If pDuplicates <> "" Then
                    pDuplicates &= ", "
                End If
                pDuplicates &= ReadyString(pDuplicate.Brand)
            Next
            Dim pSql = " SELECT MobizLibraryPIMProduct.ID" & vbCrLf &
                " FROM MobizLibraryPIMProduct" & vbCrLf &
                " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " WHERE MobizLibraryPIMProduct.Brand IN (" & pDuplicates & ")" & vbCrLf
            pSql = "UPDATE MobizLibraryPIMProduct" & vbCrLf &
                " SET Brand = " & ReadyString(pItem.Brand) & vbCrLf &
                " WHERE MobizLibraryPIMProduct.ID IN (" & vbCrLf &
                pSql & ")"
            Dim pInt = mProData.ExecuteSQL(pSql)
            pAffectedRecords += pInt
        Next

        Return pAffectedRecords

    End Function

    Public Function GetOrCreatePimByName(vProductName As String, Optional vCategory As String = Nothing) As String

        Dim pID As String
        Dim pIU As InsertUpdate
        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " WHERE ProductName = " & ReadyString(vProductName)
        pID = CStr2(mProData.ExecuteScalar(pSql))
        If String.IsNullOrWhiteSpace(pID) = False Then
            Dim pCategory = GetCategory(pID)
            If String.IsNullOrWhiteSpace(vCategory) = False _
                AndAlso pCategory <> vCategory Then

                SetProductCategory(pID, vCategory)
            End If
            Return pID
        Else
            pIU = New InsertUpdate("MobizLibraryPIMProduct")
            pIU("ProductName") = ReadyString(vProductName)
            pIU("LastUpdated") = "GETDATE()"
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pID)
            pIU("DateAdded") = "GETDATE()"
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            SetProductCategory(pID, vCategory)
        End If

        ' call these after this function
        'pUnitMeasureID = mParent.LibraryStockMeasurements.GetOrCreateUnitMeasureID("PC")
        'mParent.LibraryStockMeasurements.GetOrCreatePimProductMeasurement(pID, pUnitMeasureID)

        Return pID

    End Function

    Public Function DuplicateProductNumberCount() As Integer

        Dim pSql = " -- count" & vbCrLf &
            " SELECT SUM(ItemCount) FROM (" & vbCrLf &
            " 	SELECT TOP 100 PERCENT RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, ''))) AS ProductNo" & vbCrLf &
            " 	,COUNT(*) AS ItemCount" & vbCrLf &
            " 	FROM MobizLibraryPIMProduct" & vbCrLf &
            " 	JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " 	GROUP BY RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, '')))" & vbCrLf &
            " 	HAVING COUNT(*) > 1" & vbCrLf &
            " 	ORDER BY RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, '')))" & vbCrLf &
            " ) t"
        Dim pOut = CInt2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function DuplicateProductNumbers() As DataTable

        Dim pSql = " SELECT MobizLibraryPIMProduct.ID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, ''))) AS ProductNoNormalized" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " WHERE RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, ''))) IN (" & vbCrLf &
            " SELECT TOP 100 PERCENT RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, ''))) AS ProductNo" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " GROUP BY RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, '')))" & vbCrLf &
            " HAVING COUNT(*) > 1" & vbCrLf &
            " )" & vbCrLf &
            " ORDER BY RTRIM(LTRIM(ISNULL(MobizLibraryPIMProduct.ProductNo, '')))"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDT">output from DuplicateProductNumbers</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FillDeduplicatedProductNumbers(vDT As DataTable) As DataTable

        Dim pProductNumber As String
        Dim pNumberFormat As String
        Dim pIndexWithinGroup As Integer

        vDT.Columns.Add("FinalProductNo")
        For Each pDR As DataRow In vDT.Rows
            If pProductNumber Is Nothing _
                OrElse pProductNumber <> pDR("ProductNoNormalized") Then

                pProductNumber = pDR("ProductNoNormalized")
                Dim pGroup = vDT.Select("ProductNoNormalized = " & ReadyString(pProductNumber))
                pNumberFormat = New String("0", pGroup.Count.ToString().Length)
                pIndexWithinGroup = 0
            End If

            pIndexWithinGroup += 1
            pDR("FinalProductNo") = pDR("ProductNoNormalized") & "-" & Format(pIndexWithinGroup, pNumberFormat)
        Next pDR

        Return vDT

    End Function

    Public Function DeduplicateProductNumbers() As Integer

        Dim pSql As String
        Dim pDT = DuplicateProductNumbers()
        pDT = FillDeduplicatedProductNumbers(pDT)

        Dim pFilename = Common.ServerPath & "temp\DeduplicateProductNumbers\DeduplicateProductNumbers " & DateTimeStamp(False) & ".xlsx"
        CreateFolder(pFilename)
        TableToExcelFile(pDT, "", "", pFilename)

        mProData.BeginTransaction()
        For Each pDR As DataRow In pDT.Rows
            Dim pIU As New InsertUpdate("MobizLibraryPIMProduct")
            pIU("ProductNo") = ReadyString(pDR("FinalProductNo"))
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next

        pSql = "Update TablesLastUpdated set LastUpdated = GETDATE() where TableName = 'LibraryOrganizationPIM'"
        pSql = mProData.ExecuteSQL(pSql) ' output is used for debugging only

        mProData.CommitTransaction()

        Return pDT.Rows.Count

    End Function

    Public Function PurchaseOrderInTransit(vProductID As String, vIncludeReceived As Boolean) As DataTable

        Dim pSql = " -- purchase order in transit" & vbCrLf &
            " SELECT TOP 50 TransInvoice.ID AS [ID]" & vbCrLf &
            " , TransInvoice.CustomerID AS [CustomerID]" & vbCrLf &
            " , TransInvoice.InvoiceNumber AS [InvoiceNumber]" & vbCrLf &
            " , MobizLibraryOrganizationFOWST.FOWSTNo AS [FOWSTNo]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, TransInvoice.FowstWideInvoiceNumber) AS [SearchFowstWideInvoiceNumberNoDash]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, TransInvoice.FowstWideInvoiceNumber) AS [SearchFowstWideInvoiceNumber]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) +  CONVERT(varchar, ISNULL(TransInvoice.InvoiceNumber,0))  AS [SearchSInoDash]" & vbCrLf &
            " , RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) AS [InvoiceNum]" & vbCrLf &
            " , TransInvoice.RefInvoiceNum AS [RefInvoiceNum]" & vbCrLf &
            " , TransInvoice.FowstWideInvoiceNumber AS [FowstWideInvoiceNumber]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) + CONVERT(varchar, TransInvoice.SalesOrderNo) AS [SalesOrderNoRemoveDash]" & vbCrLf &
            " ,  (CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [CompanyName]" & vbCrLf &
            " , TransInvoice.InvoiceDate AS [InvoiceDate]" & vbCrLf &
            " , TransInvoice.DateNeeded AS [DateNeeded]" & vbCrLf &
            " , TransInvoice.Remarks AS [Remarks]" & vbCrLf &
            " , TransInvoice.PurchaseOrderDate AS [PurchaseOrderDate]" & vbCrLf &
            " , TransInvoice.DateCreated AS [DateCreated]" & vbCrLf &
            " , TransInvoice.SalesOrderDate AS [SalesOrderDate]" & vbCrLf &
            " , tblItems.Total AS [Total]" & vbCrLf &
            " , tblItems.OrderedQuantity" & vbCrLf &
            " , tblItems.ServedQuantity" & vbCrLf &
            " , tblItems.ActualQuantity" & vbCrLf &
            " , tblItems.UnitMeasure" & vbCrLf &
            " , tblItems.SkuCount AS [SkuCount]" & vbCrLf &
            " , ISNULL(TransInvoice.SalesOrderNo,0) AS [SalesOrderNo]" & vbCrLf &
            " , MobizLibraryOrganizationFOWST.Name AS [FOWSTName]" & vbCrLf &
            " , tblEncodingFowst.Name AS [EncodingFOWSTName]" & vbCrLf &
            " , LibraryDevice.PhoneName AS [DeviceName]" & vbCrLf &
            " , TransInvoice.UserID AS [UserID]" & vbCrLf &
            " , MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName AS [EmployeeFullName]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS [MIN]" & vbCrLf &
            " , TransInvoice.CustomerTableNo AS [TableNo]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' +  CONVERT(varchar, ISNULL(TransInvoice.SalesOrderNo,0))  AS [SearchSO]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) +  CONVERT(varchar, ISNULL(TransInvoice.SalesOrderNo,0))  AS [SearchSOnoDash]" & vbCrLf &
            " , TransInvoice.DeliveryMethod AS [DeliveryMethod]" & vbCrLf &
            " , TransInvoice.OrderMethod AS [OrderMethod]" & vbCrLf &
            " , TransInvoice.CustomerDemographicGender AS [CustomerDemographicGender]" & vbCrLf &
            " , TransInvoice.CustomerDemographicAge AS [CustomerDemographicAge]" & vbCrLf &
            " , TransInvoice.PurchaserName AS [PurchaserName]" & vbCrLf &
            " , TransInvoice.PurchaseOrderNo AS [PurchaseOrderNo]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' +  CONVERT(varchar, ISNULL(TransInvoice.PurchaseOrderNo,0))  AS [SearchPO]" & vbCrLf &
            " , CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) +  CONVERT(varchar, ISNULL(TransInvoice.PurchaseOrderNo,0))  AS [SearchPOnoDash]" & vbCrLf &
            " , TransInvoice.QuotationNo AS [QuotationNo]" & vbCrLf &
            " , TransInvoice.LastUpdated AS [LastUpdated]" & vbCrLf &
            " , TransInvoice.CnCUnapprovedReason AS [CnCUnapprovedReason]" & vbCrLf &
            " , TransInvoice.CncApprovedByID AS [CncApprovedByID]" & vbCrLf &
            " , TransInvoice.FinalizePickListByID AS [FinalizePickListByID]" & vbCrLf &
            " , TransInvoice.FinalizePickListTime AS [FinalizePickListTime]" & vbCrLf &
            " , CASE WHEN TransInvoice.FinalizePickListByID IS NULL THEN 'Draft'" & vbCrLf &
            "    ELSE 'Finalized'" & vbCrLf &
            "    END AS [FulfillmentStatus]" & vbCrLf &
            " , TransInvoice.DocumentNameID AS [DocumentNameID]" & vbCrLf &
            " , LibraryDocumentControl.DocumentName AS [DocumentName]" & vbCrLf &
            " , tblFowstCustomer.Name AS [CustomerFowst]" & vbCrLf &
            " , tblBinCustomer.Name AS [CustomerBin]" & vbCrLf &
            " , TransInvoice.IsCancelled AS [IsCancelled]" & vbCrLf &
            " , TransInvoice.TransType AS [TransType]" & vbCrLf &
            " , ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
            "  + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf &
            "  + ',' + ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
            "  + ',' + ISNULL(tblDeliveryCountry.Name, '')" & vbCrLf &
            "  AS [DeliveryAddress]" & vbCrLf &
            " , ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
            "  + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf &
            "  AS [DeliveryStreet]" & vbCrLf &
            " , ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
            "  + ',' + ISNULL(tblDeliveryCountry.Name, '')" & vbCrLf &
            "  AS [DeliveryCity]" & vbCrLf &
            " , TransInvoice.StrNum AS [StrNum]" & vbCrLf &
            " , TransInvoice.IsDelivered AS [IsDelivered]" & vbCrLf &
            " , CASE WHEN EXISTS(SELECT TransReceiving.ID" & vbCrLf &
            "    FROM TransReceiving" & vbCrLf &
            "    WHERE TransReceiving.PurchaseOrderID = TransInvoice.ID)" & vbCrLf &
            "  THEN 1 ELSE 0 END AS [ReceivedInStockReceiving]" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID" & vbCrLf &
            " JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblEncodingFowst ON TransInvoice.EncodingFowstID = tblEncodingFowst.ID" & vbCrLf &
            " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
            " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            "  	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
            "    ,SUM(TransInvoiceItems.OrderedQuantity) AS OrderedQuantity" & vbCrLf &
            "    ,SUM(TransInvoiceItems.Quantity) AS ServedQuantity" & vbCrLf &
            "    ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            "    ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            "    ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
            "  	FROM TransInvoiceItems" & vbCrLf &
            "    WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
            "    AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            "    AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
            "    OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
            "    AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
            "    GROUP BY TransInvoiceItems.ActualQuantity" & vbCrLf &
            "    ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            "  ) tblItems" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblFowstCustomer ON tblFowstCustomer.ID = TransInvoice.CustomerFowstID" & vbCrLf &
            " LEFT JOIN LibraryFowstBin tblBinCustomer ON tblBinCustomer.ID = TransInvoice.CustomerBinID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransInvoice.DeliveryAddressID" & vbCrLf &
            " LEFT JOIN MobizLibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID" & vbCrLf &
            " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            " AND ISNULL(TransInvoice.PurchaseOrderNo, 0) <> 0" & vbCrLf
        If vIncludeReceived = False Then
            pSql &= " AND NOT EXISTS(SELECT TransReceiving.ID" & vbCrLf &
            "    FROM TransReceiving" & vbCrLf &
            "    WHERE TransReceiving.PurchaseOrderID = TransInvoice.ID)" & vbCrLf
        End If
        pSql &= " AND TransInvoice.TransType = 'PurchaseOrder'" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC" & vbCrLf &
            " ,TransInvoice.InvoiceDate DESC"
        Dim pDT = mProData.Execute(pSql)

        pDT.Columns.Add("RunningTotal", GetType(Decimal))
        Dim pRunningTotal As Decimal = 0
        For Each pDR As DataRow In pDT.Rows
            pRunningTotal += CDec2(pDR("OrderedQuantity"))
            pDR("RunningTotal") = pRunningTotal
        Next

        Return pDT

    End Function

    Private Function UpdatePriceLevelTypes() As Integer

        Dim pSql = "UPDATE MobizLibraryPIMPriceLevel" & vbCrLf &
            " SET PriceLevelType = 'price'" & vbCrLf &
            " WHERE PriceLevelType IS NULL" & vbCrLf &
            " AND MobizLibraryPIMPriceLevel.ID IN (" & vbCrLf &
            "'{01000012-0005-0000-0000-000000000000}' -- RETAIL PRICE" & vbCrLf &
            ",'{01000012-0004-0000-0000-000000000000}' -- WHOLESALE PRICE" & vbCrLf &
            ")"
        Dim pOut = mProData.ExecuteSQL(pSql)

        Return pOut
    End Function

    Public Function RoundUpSellingPrice(vOrgID As String) As Integer

        Dim pRoundUpSellingPrices = mParent.SystemDefaults.RoundUpSellingPrices

        If String.IsNullOrWhiteSpace(pRoundUpSellingPrices) Then
            Return 0
        End If

        pRoundUpSellingPrices = pRoundUpSellingPrices.Trim.ToLower()
        If pRoundUpSellingPrices <> "always" _
            AndAlso pRoundUpSellingPrices <> "once" Then

            Return 0
        End If

        Dim pRoundUpSellingPricesCentavo = mParent.SystemDefaults.RoundUpSellingPricesCentavo
        If pRoundUpSellingPricesCentavo = 0 _
            OrElse pRoundUpSellingPricesCentavo >= 100 Then

            Return 0
        End If

        UpdatePriceLevelTypes()
        Dim pDT = GetRoundableSellingPrices(vOrgID, pRoundUpSellingPricesCentavo)
        If pDT.Rows.Count = 0 Then
            Return 0
        End If
        pDT.Columns.Add("ModifiedPrice", GetType(Decimal))
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("MobizLibraryPIMProductPriceLevel")
            Dim pModifiedPrice = RoundDecimal(pDR("Price"), pRoundUpSellingPricesCentavo / 100, eRoundingMethod.Up)
            pDR("ModifiedPrice") = pModifiedPrice
            pIU("Price") = pModifiedPrice
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.ExecuteSQL(pSql)
        Next pDR

        Return pDT.Rows.Count

    End Function

    Private Function GetRoundableSellingPrices(vOrgID As String, vCentavos As Integer) As DataTable

        Dim pSql = " SELECT MobizLibraryPIMProductPriceLevel.ID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            " ,ABS(MobizLibraryPIMProductPriceLevel.Price) % 1 AS Centavos" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.PriceLevelID" & vbCrLf &
            " ,MobizLibraryPIMPriceLevel.PriceLevel" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " JOIN MobizLibraryPIMProductPriceLevel ON MobizLibraryPIMProductPriceLevel.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " JOIN MobizLibraryPIMPriceLevel ON MobizLibraryPIMPriceLevel.ID = MobizLibraryPIMProductPriceLevel.PriceLevelID" & vbCrLf &
            " WHERE MobizLibraryPIMPriceLevel.IsActive = 1" & vbCrLf &
            " AND MobizLibraryPIMPriceLevel.PriceLevelType = 'price'" & vbCrLf &
            " AND (" & vbCrLf &
            " 	MobizLibraryPIMPriceLevel.OrgID IS NULL" & vbCrLf &
            "     OR MobizLibraryPIMPriceLevel.OrgID = " & ReadyString(vOrgID) & vbCrLf &
            " )" & vbCrLf &
            " AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " AND ((ABS(MobizLibraryPIMProductPriceLevel.Price) % 1) * 100) % " & vCentavos & " <> 0" & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.ProductName"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function SaveAs(vOrgID As String, vSourceProductID As String, vProductNum As String, vProductName As String, vProductBrand As String) As String

        mProData.BeginTransaction()
        Dim pSql = "SELECT * " & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            " WHERE MobizLibraryPIMProduct.ID = " & ReadyString(vSourceProductID)
        Dim pDR = mProData.ExecuteRow(pSql)
        Dim pIU = Business.DataRowToInsertUpdate(pDR)
        pIU.TableName = "MobizLibraryPIMProduct"
        Dim pID = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pID)
        pIU("ProductNo") = ReadyString(vProductNum)
        pIU("ProductName") = ReadyString(vProductName)
        pIU("Brand") = ReadyString(vProductBrand)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT * " & vbCrLf &
            " FROM LibraryOrganizationPIM" & vbCrLf &
            " WHERE LibraryOrganizationPIM.ProductID = " & ReadyString(vSourceProductID)
        pDR = mProData.ExecuteRow(pSql)
        pIU = Business.DataRowToInsertUpdate(pDR)
        pIU.TableName = "LibraryOrganizationPIM"
        pIU("ID") = "NEWID()"
        pIU("ProductID") = ReadyString(pID)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        pSql = "SELECT *" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " WHERE MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(vSourceProductID)
        Dim pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "MobizLibraryPIMProductMeasurement"
            pIU("ID") = "NEWID()"
            pIU("ProductID") = ReadyString(pID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

        pSql = "SELECT *" &
            " FROM MobizLibraryPIMProductPriceLevel " &
            " WHERE ProductID = " & ReadyString(vSourceProductID) &
            " AND OrganizationID = " & ReadyString(vOrgID)
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "MobizLibraryPIMProductPriceLevel"
            pIU("ID") = "NEWID()"
            pIU("ProductID") = ReadyString(pID)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

        pSql = "SELECT *" &
            " FROM LibraryVendorPrices " &
            " WHERE ProductID = " & ReadyString(vSourceProductID)
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "LibraryVendorPrices"
            pIU("ID") = "NEWID()"
            pIU("ProductID") = ReadyString(pID)
            If pDR("UpdatedBy") IsNot DBNull.Value Then
                pIU("UpdatedBy") = ReadyString(pDR("UpdatedBy"))
            End If
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

        mProData.CommitTransaction()

        Return pID

    End Function

    Public Function SavePIMRelationship(vParentProductID As String, vChildProductID As String, vTagProductAs As String) As String
        mProData.BeginTransaction()

        Dim relationshipid As String = Guid.NewGuid.ToString()
        Dim pSql, parentrelationshipid As String
        Dim pRows As Object
        Dim p As New InsertUpdate("MobizLibraryPIMProductRelationship")

        parentrelationshipid = AddToRelationshipTable(vParentProductID, relationshipid, vTagProductAs)
        relationshipid = AddToRelationshipTable(vChildProductID, parentrelationshipid, vTagProductAs)

        p("RelationshipID") = ReadyString(relationshipid)
        p("LastUpdated") = "getdate()"
        p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
        mProData.Execute(p.GetUpdate(" RelationshipID = " & ReadyString(parentrelationshipid)))

        mProData.CommitTransaction()

        Return relationshipid
    End Function

    Function AddToRelationshipTable(vProductID As String, vRelationshipID As String, vTagProductAs As String)
        Dim pSql As String
        Dim pRows As Object
        Dim p As New InsertUpdate("MobizLibraryPIMProductRelationship")

        pSql = "SELECT * FROM MobizLibraryPIMProductRelationship WHERE ProductID = " & ReadyString(vProductID) & vbCrLf &
                " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " AND Tag = " & ReadyString(vTagProductAs)
        pRows = mProData.Execute(pSql).Rows

        If pRows.Count = 0 Then
            p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("OrganizationID") = ReadyString(WebRequest.Instance.OrgID)
            p("ProductID") = ReadyString(vProductID)
            p("RelationshipID") = ReadyString(vRelationshipID)
            p("Tag") = ReadyString(vTagProductAs)
            p("LastUpdated") = "getdate()"
            p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("IsActive") = 1
            mProData.Execute(p.GetInsert())
        Else
            vRelationshipID = pRows(0)("RelationshipID").ToString()
        End If

        Return vRelationshipID
    End Function

    Public Function GetRelatedProducts(vProductID As String, Optional vTagProduct As String = "") As DataTable
        Dim pSql As String

        pSql = "SELECT rel.*, pim.ProductName, pim.ProductNo, pim.Brand, " & vbCrLf &
                " ppl.FirstName + ' ' + ppl.Surname as UpdatedByName " & vbCrLf &
                " FROM MobizLibraryPIMProductRelationship as rel " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = rel.ProductID " & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppl ON ppl.ID = rel.UpdatedBy " & vbCrLf &
                " WHERE RelationshipID = (SELECT RelationshipID FROM MobizLibraryPIMProductRelationship WHERE ISNULL(IsActive, 1) = 1 AND ProductID = " & ReadyString(vProductID) & " AND Tag = " & ReadyString(vTagProduct) & ")" & vbCrLf &
                " AND ProductID <> " & ReadyString(vProductID) & vbCrLf &
                " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " AND ISNULL(rel.IsActive, 1) = 1 "

        If vTagProduct <> "" Then
            pSql &= " AND Tag = " & ReadyString(vTagProduct) & vbCrLf
        End If

        Return mProData.Execute(pSql)
    End Function

    Public Function VendorPrices(vID As String, Optional vIsActive As Integer = 1) As DataTable

        With New Finder
            .ProData = mProData
            With .Fields
                With .Add("VendorID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryOrganizationSupplier.SupplierID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganization.CompanyName"
                End With
                With .Add("Price", SqlDbType.Decimal)
                    .Expression = "LibraryVendorPrices.Price"
                End With
                With .Add("UnitPrice", SqlDbType.Decimal)
                    .Expression = "LibraryVendorPrices.UnitPrice"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "LibraryVendorPrices.UnitMeasure"
                End With
                With .Add("Url", SqlDbType.VarChar)
                    .Expression = "LibraryVendorPrices.Url"
                End With
                With .Add("Note", SqlDbType.VarChar)
                    .Expression = "LibraryVendorPrices.Note"
                End With
                With .Add("UnitQty", SqlDbType.Decimal)
                    .Expression = "LibraryVendorPrices.UnitQty"
                End With
                With .Add("IsActive", SqlDbType.Decimal)
                    .Expression = "ISNULL(LibraryVendorPrices.IsActive, 1)"
                End With
                With .Add("PaymentTerm", SqlDbType.VarChar)
                    .Expression = "LibraryPaymentTerms.Name"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "LibraryVendorPrices.LastUpdated"
                End With
                With .Add("UpdatedByName", SqlDbType.VarChar)
                    .Expression = BrowserController.ComposePeopleName()
                End With
            End With

            .SqlFrom = "FROM LibraryOrganizationSupplier " & vbCrLf &
                "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = LibraryOrganizationSupplier.SupplierID " & vbCrLf
            .SqlFrom &= " JOIN LibraryVendorPrices ON LibraryOrganizationSupplier.SupplierID = LibraryVendorPrices.VendorID" & vbCrLf
            .SqlFrom &= " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationSupplier.CreditTermsID" & vbCrLf
            .SqlFrom &= " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryVendorPrices.UpdatedBy" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
            .SqlWhere = " WHERE ISNULL(LibraryVendorPrices.IsActive, 1) = " & vIsActive & " AND LibraryVendorPrices.ProductID = " & ReadyString(vID) & vbCrLf
            .SqlOrder = "ORDER BY LibraryVendorPrices.LastUpdated DESC, MobizLibraryOrganization.CompanyName ASC" & vbCrLf
            Dim pDT = .Requery
            Return pDT
        End With

    End Function

    Public Function VendorPriceProducts(vVendorID As String, Optional vProductID As String = Nothing) As DataTable

        Dim pSql As String = "SELECT LibraryVendorPrices.ProductID" & vbCrLf &
            ",LibraryVendorPrices.Price" & vbCrLf &
            " FROM LibraryVendorPrices" & vbCrLf &
            " WHERE LibraryVendorPrices.VendorID = " & ReadyString(vVendorID) & vbCrLf
        If String.IsNullOrWhiteSpace(vProductID) = False Then
            pSql &= " AND LibraryVendorPrices.ProductID = " & ReadyString(vProductID) & vbCrLf
        End If
        pSql &= " ORDER BY LibraryVendorPrices.LastUpdated DESC"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub SetProductCategory(vProductID As String, vCategory As String)

        Dim pSql As String
        Dim p = New InsertUpdate("MobizLibraryPIMProductVariableFeatures001")
        p("ID") = "NEWID()"
        p("ProductID") = ReadyString(vProductID)
        p("C0020") = ReadyString(vCategory)

        pSql = p.GetUpdate("ProductID = " & ReadyString(vProductID))
        Dim pAffectedCount = mProData.ExecuteSQL(pSql)
        If pAffectedCount = 0 Then
            pSql = p.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

    End Sub

    Public Function GetOrCreateMallLessorProduct() As String

        Dim pID = GetOrCreatePimByName("Mall Lessor Product", "System-Created")
        Dim pUnitMeasureID = mParent.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Unit")
        mParent.LibraryStockMeasurements.GetOrCreatePimProductMeasurement(pID, pUnitMeasureID)

        Return pID

    End Function

    Public Class SavePimSalesCachingInput
        Public Pim As IEnumerable(Of PimClass)
        Public Class PimClass
            Public ProductID As String
            Public Interval As String ' integer
        End Class
    End Class

    Public Sub SavePimSalesCaching(vInput As SavePimSalesCachingInput, vUserID As String)

        mProData.BeginTransaction()
        For Each pItem In vInput.Pim
            Dim pIU = New InsertUpdate("LibraryPimSalesCaching")
            pIU("Interval") = CInt2(pItem.Interval)
            pIU("UpdatedBy") = ReadyString(vUserID)
            pIU("LastUpdated") = "GETDATE()"
            Dim pSql = pIU.GetUpdate("ProductID = " & ReadyString(pItem.ProductID))
            Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pIU("ID") = "NEWID()"
                pIU("ProductID") = ReadyString(pItem.ProductID)
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            End If
        Next
        mProData.CommitTransaction()

    End Sub

    Public Function IsValidRentableAsset(vProductID As String, ByRef oError As String) As Boolean

        Dim pSql = "   -- source for creating repeatable trans" & vbCrLf &
            "   SELECT MobizLibraryPIMProduct.ID" & vbCrLf &
            "   ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            "   ,MobizLibraryPIMProductVariableFeatures001.C0020 AS Category" & vbCrLf &
            "   ,LibraryInvoicingMethod.MethodType" & vbCrLf &
            "   FROM MobizLibraryPIMProduct" & vbCrLf &
            "   LEFT JOIN LibraryRentableAssets ON MobizLibraryPIMProduct.ID = LibraryRentableAssets.ProductID" & vbCrLf &
            "   LEFT JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            "   LEFT JOIN LibraryInvoicingMethodCategory ON LibraryInvoicingMethodCategory.Category = MobizLibraryPIMProductVariableFeatures001.C0020" & vbCrLf &
            "   LEFT JOIN LibraryInvoicingMethod ON LibraryInvoicingMethod.ID = LibraryInvoicingMethodCategory.InvoicingMethodID" & vbCrLf &
            "   WHERE MobizLibraryPIMProduct.ID = " & ReadyString(vProductID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            oError = "ProductID, " & vProductID & ", does not exist in PIM library."
            Return False
        End If
        If CStr2(pDR("ProductName")).Trim() = "" Then
            oError = "ProductID, " & vProductID & ", has no product name."
            Return False
        End If
        If CStr2(pDR("Category")).Trim() = "" Then
            oError = pDR("ProductName") & " has no category. Please assign it a category in PIM library."
            Return False
        End If
        If CStr2(pDR("MethodType")).Trim() = "" Then
            oError = pDR("ProductName") & " has no invoicing method type. Please assign the product category, " & pDR("Category") & ", an invoicing method type in Asset Management > Settings."
            Return False
        End If

        Return True

    End Function

    Public Function DetectRecursiveContent(vOrgID As String, vParentProductID As String, vChildProductID As String) As String

        Dim pSql = " -- recursive MobizLibraryPIMProductContents" & vbCrLf &
            " WITH RecursiveContent AS (" & vbCrLf &
            "     -- Anchor member: start with the given parent" & vbCrLf &
            "     SELECT ParentProductID, ChildProductID, 1 AS level" & vbCrLf &
            "     FROM MobizLibraryPIMProductContents" & vbCrLf &
            "     WHERE MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            "     AND MobizLibraryPIMProductContents.ParentProductID = " & ReadyString(vChildProductID) & vbCrLf &
            "     UNION ALL" & vbCrLf &
            "     -- Recursive member: join to get deeper levels" & vbCrLf &
            "     SELECT MobizLibraryPIMProductContents_1.ParentProductID" & vbCrLf &
            "     ,MobizLibraryPIMProductContents_1.ChildProductID" & vbCrLf &
            "     ,RecursiveContent_1.level + 1" & vbCrLf &
            "     FROM MobizLibraryPIMProductContents MobizLibraryPIMProductContents_1" & vbCrLf &
            "     JOIN RecursiveContent RecursiveContent_1 ON MobizLibraryPIMProductContents_1.ParentProductID = RecursiveContent_1.ChildProductID" & vbCrLf &
            " )" & vbCrLf &
            " SELECT RecursiveContent.*" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " FROM RecursiveContent" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = RecursiveContent.ChildProductID" & vbCrLf &
            " WHERE RecursiveContent.ChildProductID = " & ReadyString(vParentProductID) & vbCrLf &
            " ORDER BY level;"
        Dim pDT = mParent.ProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            Return "recursive"
        End If

    End Function
End Class