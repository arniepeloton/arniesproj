Public Class LibraryStockMeasurements
    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    <Obsolete("2014-11-13 no known use. Arvin")>
    Public Function GetParams(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT " & _
               "    LibraryStockMeasurements.ID, " & _
               "    LibraryStockMeasurements.StockID, " & _
               "    LibraryUnitMeasures.UnitMeasure, " & _
               "    LibraryStockMeasurements.Quantity " & _
               "FROM " & _
               "    LibraryStockMeasurements INNER JOIN LibraryUnitMeasures " & _
               "    ON LibraryUnitMeasures.ID = LibraryStockMeasurements.UnitMeasureID " & _
               "WHERE " & _
               "    LibraryStockMeasurements.StockID = " & ReadyString(vID)

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ByProductID(vProductID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT TOP 100 PERCENT " & vbCrLf &
            " MobizLibraryPIMMeasure.ID" & vbCrLf &
            ", MobizLibraryPIMProductMeasurement.MeasurementLevel" & vbCrLf &
            " ,MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.Discount" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            " ,tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
            " ,tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf &
            "  FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            "  JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
            "  JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            "  SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            "  	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
            "      FROM MobizLibraryPIMProductMeasurement MobizLibraryPIMProductMeasurement_1" & vbCrLf &
            "      JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
            "      JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "      WHERE MobizLibraryPIMProductMeasurement_1.ProductID = MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf &
            "      ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            "  ) tblBaseUnit" & vbCrLf &
            "  WHERE MobizLibraryPIMProductMeasurement.ProductID = " & ReadyString(vProductID) & vbCrLf &
            "  ORDER BY MobizLibraryPIMProductMeasurement.MeasurementLevel"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetCustomUnit(vProductID As String, vOrgID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT MobizLibraryPIMMeasureDefinition.SourceMeasureID" & vbCrLf &
            ",MobizLibraryPIMMeasureDefinition.DestinationMeasure" & vbCrLf &
            ",MobizLibraryPIMMeasureDefinition.Quantity" & vbCrLf &
            " FROM MobizLibraryPIMMeasureDefinition" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND OrganizationID = " & ReadyString(vOrgID) & vbCrLf
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function MeasurementFamilies() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT MobizLibraryPIMMeasureConversion.MeasurementFamily" & vbCrLf &
            " ,MobizLibraryPIMMeasureConversion.DestMeasure" & vbCrLf &
            " FROM MobizLibraryPIMMeasureConversion" & vbCrLf &
            " GROUP BY MobizLibraryPIMMeasureConversion.MeasurementFamily" & vbCrLf &
            " ,MobizLibraryPIMMeasureConversion.DestMeasure" & vbCrLf &
            " ORDER BY MobizLibraryPIMMeasureConversion.MeasurementFamily" & vbCrLf &
            " ,MobizLibraryPIMMeasureConversion.DestMeasure"
        pDT = mProData.Execute(pSql)
        Return pDT

    End Function

    Public Function StandardToBaseUnitQty(vProductID As String, vOrgID As String, vStandardUnit As String) As Decimal

        Dim pDR As DataRow
        Dim pOut As Decimal?

        pDR = mParent.LibraryStock.GetUnitDefinition(vProductID, vOrgID)
        If pDR Is Nothing Then
            Return 1
        End If
        If CStr2(pDR("DestinationMeasure")).Trim() = "" Then
            Return 1
        End If
        If CDec2(pDR("Quantity")) <= 0 Then
            Return 1
        End If
        If CStr2(pDR("DestinationMeasure")).Trim().ToLower() = CStr2(vStandardUnit).Trim().ToLower() Then
            Return 1 / pDR("Quantity")
        End If

        pOut = GetStandardUnitConvertionFactor(CStr2(pDR("DestinationMeasure")), vStandardUnit)
        If pOut Is Nothing Then
            Return 1
        End If

        Return 1 / (pOut.Value * pDR("Quantity"))

    End Function

    Public Function GetStandardUnitConvertionFactor(vBaseUnit As String, vDestUnit As String) As Decimal?

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT MobizLibraryPIMMeasureConversion.BaseMeasure" & vbCrLf &
            " ,MobizLibraryPIMMeasureConversion.DestMeasure" & vbCrLf &
            " ,MobizLibraryPIMMeasureConversion.ConversionFactor" & vbCrLf &
            " FROM MobizLibraryPIMMeasureConversion" & vbCrLf &
            " WHERE (MobizLibraryPIMMeasureConversion.BaseMeasure =" & ReadyString(vBaseUnit) & vbCrLf &
            " AND MobizLibraryPIMMeasureConversion.DestMeasure = " & ReadyString(vDestUnit) & ")" & vbCrLf &
            " OR " & vbCrLf &
            " (MobizLibraryPIMMeasureConversion.BaseMeasure = " & ReadyString(vDestUnit) & vbCrLf &
            " AND MobizLibraryPIMMeasureConversion.DestMeasure = " & ReadyString(vBaseUnit) & ")"
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return Nothing
        End If

        If pDR("BaseMeasure") = vBaseUnit Then
            Return CDec(pDR("ConversionFactor"))
        End If

        Return 1 / CDec(pDR("ConversionFactor"))

    End Function

    Public Function GetFamilyMeasurement(vMeasure As String) As DataTable

        Dim pDT As DataTable
        Dim pDV As DataView

        pDT = MeasurementFamilies()
        pDV = New DataView(pDT, "DestMeasure = " & ReadyString(vMeasure), "", DataViewRowState.CurrentRows)
        If pDV.Count > 0 Then
            pDV = New DataView(pDT, "MeasurementFamily = " & ReadyString(pDV(0)("MeasurementFamily")), "", DataViewRowState.CurrentRows)
        End If

        Return pDV.ToTable()

    End Function

    Public Function GetUnitQty(vProductID As String, vUnitMeasureID As String) As DataRow

        Dim pDT As DataTable

        pDT = ByProductID(vProductID)
        For Each pDR As DataRow In pDT.Rows
            If ReadyGuid(pDR("ID")) = ReadyGuid(vUnitMeasureID) Then
                Return pDR
            End If
        Next pDR

    End Function

    Public Function GetUnitMeasureQty(vProductID As String, vUnitMeasure As String) As DataRow

        Dim pDT As DataTable

        pDT = ByProductID(vProductID)
        For Each pDR As DataRow In pDT.Rows
            If cstr2(pDR("UnitMeasure")).trim().toupper() = Cstr2(vUnitMeasure).trim().toupper() Then
                Return pDR
            End If
        Next pDR

    End Function

    Public Function GetUnitMeasureID(vLabel As String) As String

        Dim pSql As String
        Dim pLanguageLabelID As String
        Dim pID As String

        pLanguageLabelID = GetOrCreateLanguageLabel(vLabel)
        pSql = " SELECT MobizLibraryPIMMeasure.ID" & vbCrLf &
            " FROM MobizLibraryPIMMeasure" & vbCrLf &
            " WHERE MobizLibraryPIMMeasure.MeasureNameID = " & ReadyString(pLanguageLabelID)
        pID = CStr2(mProData.ExecuteScalar(pSql))
        If pID <> "" Then
            Return pID
        End If

    End Function

    Public Function GetOrCreateUnitMeasureID(vLabel As String) As String

        Dim pSql As String
        Dim pLanguageLabelID As String
        Dim pID As String
        Dim pIU As InsertUpdate

        pLanguageLabelID = GetOrCreateLanguageLabel(vLabel)
        pSql = " SELECT MobizLibraryPIMMeasure.ID" & vbCrLf &
            " FROM MobizLibraryPIMMeasure" & vbCrLf &
            " WHERE MobizLibraryPIMMeasure.MeasureNameID = " & ReadyString(pLanguageLabelID)
        pID = CStr2(mProData.ExecuteScalar(pSql))
        If pID <> "" Then
            Return pID
        End If

        pIU = New InsertUpdate("MobizLibraryPIMMeasure")
        pID = Guid.NewGuid.ToString()
        pIU("ID") = ReadyString(pID)
        pIU("MeasureNameID") = ReadyString(pLanguageLabelID)
        pIU("IsActive") = 1
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function GetOrCreateLanguageLabel(vLabel As String) As String

        Dim pSql As String
        Dim pID As String
        Dim pIU As InsertUpdate

        pSql = " SELECT MobizLibraryCountriesLanguageLabel.ID" & vbCrLf &
            " FROM MobizLibraryCountriesLanguageLabel" & vbCrLf &
            " WHERE MobizLibraryCountriesLanguageLabel.Label = " & ReadyString(vLabel)
        pID = CStr2(mProData.ExecuteScalar(pSql))
        If pID <> "" Then
            Return pID
        End If

        pIU = New InsertUpdate("MobizLibraryCountriesLanguageLabel")
        pID = Guid.NewGuid.ToString()
        pIU("ID") = ReadyString(pID)
        pIU("Label") = ReadyString(vLabel)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function GetOrCreatePimProductMeasurement(vProductID As String, vUnitMeasurementID As String, Optional vUnitQty As Decimal = 1) As String

        Dim pID As String
        Dim pSql As String
        Dim pIU As InsertUpdate

        pSql = "SELECT MobizLibraryPIMProductMeasurement.ID" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND UnitMeasureID = " & ReadyString(vUnitMeasurementID)
        pID = CStr2(mProData.ExecuteScalar(pSql))
        If IsGuid(pID) Then
            Return pID
        End If

        pID = Guid.NewGuid().ToString()
        pIU = New InsertUpdate("MobizLibraryPIMProductMeasurement")
        pIU("ID") = ReadyString(pID)
        pIU("ProductID") = ReadyString(vProductID)
        pIU("UnitMeasureID") = ReadyString(vUnitMeasurementID)
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pIU("MeasurementLevel") = 1
        pIU("Quantity") = vUnitQty
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function AddBarcode(vStockMeasurementsID As String, vBarcode As String) As String

        Dim pSql = "SELECT TOP 1 ID" & vbCrLf &
            " FROM MobizLibraryPIMProductMeasurementBarcode" & vbCrLf &
            " WHERE MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = " & ReadyString(vStockMeasurementsID) & vbCrLf &
            " AND MobizLibraryPIMProductMeasurementBarcode.Barcode = " & ReadyString(vBarcode) & vbCrLf
        Dim pID = CStr2(mProData.ExecuteScalar(pSql))
        If String.IsNullOrWhiteSpace(pID) = False Then
            Return pID
        End If

        pID = Guid.NewGuid.ToString()
        Dim pIU = New InsertUpdate("MobizLibraryPIMProductMeasurementBarcode")
        pIU("ID") = ReadyString(pID)
        pIU("Barcode") = ReadyString(vBarcode)
        pIU("StockMeasurementsID") = ReadyString(vStockMeasurementsID)
        pIU("ReadyBarcode") = ReadyString(CStr2(vBarcode).Trim())
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function AddConversion(
        vBaseUnit As String,
        vDestUnit As String,
        vFamily As String,
        vFactor As Decimal) As Boolean

        Dim pDR As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pID As String

        pDR = GetConversion(vBaseUnit, vDestUnit)
        If pDR IsNot Nothing Then
            Return False
        End If
        pIU = New InsertUpdate("MobizLibraryPIMMeasureConversion")
        pID = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pID)
        pIU("BaseMeasure") = ReadyString(vBaseUnit)
        pIU("DestMeasure") = ReadyString(vDestUnit)
        pIU("MeasurementFamily") = ReadyString(vFamily)
        pIU("ConversionFactor") = vFactor
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return True

    End Function

    Public Function GetConversion(vBaseUnit As String,
        vDestUnit As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            ",BaseMeasure" & vbCrLf &
            ",DestMeasure" & vbCrLf &
            ",MeasurementFamily" & vbCrLf &
            ",ConversionFactor" & vbCrLf &
            " FROM MobizLibraryPIMMeasureConversion" & vbCrLf &
            " WHERE BaseMeasure = " & ReadyString(vBaseUnit) & vbCrLf &
            " AND DestMeasure = " & ReadyString(vDestUnit)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class
