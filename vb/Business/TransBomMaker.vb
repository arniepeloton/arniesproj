Public Class TransBomMaker

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetProducts(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ID" & vbCrLf &
            ",ProductID" &
            ",Quantity" &
            ",UnitMeasureID" &
            " FROM TransBomMakerProducts" &
            " WHERE ParentID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetCart(vID As String) As DataSet

        Dim pSql As String
        Dim pDT As DataTable
        Dim pDS As New DataSet

        ' master
        pSql = "SELECT TransBomMaker.ID" & vbCrLf & _
            ",TransBomMaker.AssignedToID" & vbCrLf &
            ",TransBomMaker.LastUpdated" & vbCrLf &
            ",TransBomMaker.UserID" & vbCrLf &
            ",TransBomMaker.FowstID" & vbCrLf &
            ",TransBomMaker.DateNeeded" & vbCrLf &
            ",TransBomMaker.Num" & vbCrLf &
            ",TransBomMaker.DeviceID" & vbCrLf &
            ",CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) AS FowstDeviceNum" & vbCrLf &
            "," & BrowserController.ComposePeopleName("tblAssignedToMobizLibraryPeople", "tblAssignedToMobizLibraryPeoplePreNominalSuffix") & " AS AssignedToName" & vbCrLf &
            "," & BrowserController.ComposePeopleName("tblEncoderMobizLibraryPeople", "tblEncoderMobizLibraryPeoplePreNominalSuffix") & " AS EncoderName" & vbCrLf

        pSql &= "  FROM TransBomMaker " & vbCrLf & _
            " LEFT JOIN MobizLibraryOrganizationFOWST ON TransBomMaker.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransBomMaker.DeviceID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPeople tblAssignedToMobizLibraryPeople ON tblAssignedToMobizLibraryPeople.ID = TransBomMaker.AssignedToID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblAssignedToMobizLibraryPeoplePreNominalSuffix ON tblAssignedToMobizLibraryPeople.TitleID = tblAssignedToMobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPeople tblEncoderMobizLibraryPeople ON tblEncoderMobizLibraryPeople.ID = TransBomMaker.UserID" & vbCrLf &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix tblEncoderMobizLibraryPeoplePreNominalSuffix ON tblEncoderMobizLibraryPeople.TitleID = tblEncoderMobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

        pSql &= "  WHERE TransBomMaker.ID = " & ReadyString(vID) & vbCrLf
        pDT = mProData.Execute(pSql)
        pDT.TableName = "Master"
        pDS.Tables.Add(pDT)

        pDT = GetStocks(vID)
        pDT.TableName = "Products"

        pDS.Tables.Add(pDT)

        Return pDS

    End Function

    Private Function GetStocks(vBomMakerID As String) As DataTable

        Dim pSql As String
        Dim pDT As DataTable

        ' products
        pSql = "SELECT TransBomMakerProducts.ID" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS ProductName" & vbCrLf & _
            " ,TransBomMakerProducts.Quantity" & vbCrLf & _
            " ,TransBomMakerProducts.UnitMeasureID" & vbCrLf & _
            " ,MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf & _
            " ,TransBomMakerProducts.ProductID" & vbCrLf
        pSql &= " FROM TransBomMakerProducts " & vbCrLf & _
               " LEFT JOIN MobizLibraryPIMProduct ON TransBomMakerProducts.ProductID = MobizLibraryPIMProduct.ID  " & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = TransBomMakerProducts.UnitMeasureID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf
        'pSql &= " OUTER APPLY (" & vbCrLf & _
        '    " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf & _
        '    " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf & _
        '    "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf & _
        '    "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf & _
        '    "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf & _
        '    "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf & _
        '    "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf & _
        '    " ) tblBaseUnit" & vbCrLf
        pSql &= " WHERE TransBomMakerProducts.ParentID = " & ReadyString(vBomMakerID) & vbCrLf
        pSql &= " ORDER BY ProductName"

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function GetContents(vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TransBomMakerContents.ProductID" & vbCrLf &
            " ,TransBomMakerContents.ParentID" & vbCrLf &
            " ,TransBomMakerContents.Quantity" & vbCrLf &
            " ,TransBomMakerContents.UnitMeasureID" & vbCrLf &
            " ,TransBomMakerContents.UnitMeasure" & vbCrLf &
            " ,TransBomMakerContents.BaseUnitMeasureID" & vbCrLf &
            " ,TransBomMakerContents.QtyInBaseUnit" & vbCrLf &
            ",MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS ProductName" & vbCrLf & _
            " FROM TransBomMakerContents" & vbCrLf &
            " JOIN TransBomMakerProducts ON TransBomMakerProducts.ID = TransBomMakerContents.ParentID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct ON TransBomMakerContents.ProductID = MobizLibraryPIMProduct.ID  " & vbCrLf
        pSql &= " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = TransBomMakerContents.UnitMeasureID" & vbCrLf &
            " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            " WHERE TransBomMakerProducts.ParentID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub SaveContents(vID As String, vOrgID As String)

        Dim pDtProducts As DataTable
        Dim pSql As String

        mProData.BeginTransaction()
        pDtProducts = GetProducts(vID)
        For Each pDrProduct As DataRow In pDtProducts.Rows
            Dim pDtContentDefinition = mParent.LibraryStock.GetContents(CStr2(pDrProduct("ProductID")), vOrgID)
            For Each pDrContentDefinition As DataRow In pDtContentDefinition.Rows
                Dim pIU = New InsertUpdate("TransBomMakerContents")
                pIU("ID") = "NEWID()"
                pIU("ParentID") = ReadyString(pDrProduct("ID"))
                pIU("ProductID") = ReadyString(pDrContentDefinition("ChildProductID"))
                pIU("Quantity") = CDec2(pDrContentDefinition("Quantity")) * CDec2(pDrProduct("Quantity"))
                If String.IsNullOrWhiteSpace(CStr2(pDrContentDefinition("UnitMeasureID"))) Then
                    pIU("UnitMeasure") = ReadyString(pDrContentDefinition("UnitMeasure"))
                    Dim pConvertion = FromStandard(CStr2(pDrContentDefinition("ChildProductID")), CStr2(pDrContentDefinition("UnitMeasure")), vOrgID)
                    If pConvertion IsNot Nothing Then
                        pIU("BaseUnitMeasureID") = ReadyString(pConvertion.UnitMeasureID)
                        pIU("QtyInBaseUnit") = CDec2(pDrContentDefinition("Quantity")) * CDec2(pDrProduct("Quantity")) * pConvertion.Multiplier
                    End If
                Else
                    pIU("UnitMeasureID") = ReadyString(pDrContentDefinition("UnitMeasureID"))
                End If
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            Next
        Next
        mProData.CommitTransaction()

    End Sub

    Public Class FromStandardOutput
        Public UnitMeasureID As String
        Public Multiplier As Decimal
    End Class

    Private Function FromStandard(vProductID As String, vStandardUnitMeasure As String, vOrgID As String) As FromStandardOutput

        Dim pOut = New FromStandardOutput()
        Dim pDR = mParent.LibraryStockMeasurements.GetCustomUnit(vProductID, vOrgID)
        If pDR IsNot Nothing Then
            pOut.UnitMeasureID = CStr2(pDR("SourceMeasureID"))
            pOut.Multiplier = 1 / pDR("Quantity")
            Return pOut
        End If

        '// unit needs convertion
        Dim pSql = " SELECT LibraryPIMMeasureDefinition.SourceMeasureID" & vbCrLf &
            " ,LibraryPIMMeasureDefinition.Quantity" & vbCrLf &
            " ,LibraryPIMMeasureConversion.ConversionFactor" & vbCrLf &
            " FROM LibraryPIMMeasureDefinition" & vbCrLf &
            " JOIN LibraryPIMMeasureConversion" & vbCrLf &
            " ON LibraryPIMMeasureConversion.DestMeasure = LibraryPIMMeasureDefinition.DestinationMeasure" & vbCrLf &
            " WHERE LibraryPIMMeasureDefinition.ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND LibraryPIMMeasureConversion.BaseMeasure = " & ReadyString(vStandardUnitMeasure) & vbCrLf &
            " AND LibraryPIMMeasureDefinition.OrganizationID = " & ReadyString(vOrgID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            ' we should NOT be here
            Return Nothing
        End If
        pOut.UnitMeasureID = CStr2(pDR("SourceMeasureID"))
        pOut.Multiplier = 1 / pDR("Quantity")

        Return pOut

    End Function

    Private Function GetCombined(vID As String) As DataTable

        Dim pItems As DataTable
        Dim pDrBaseUnit As DataRow
        Dim pUnitQty As Decimal
        Dim pQty As Decimal
        Dim pDtTrans As DataTable ' data from TransBomMakerProducts & TransBomMakerContents
        Dim pDrTrans As DataRow
        Dim pDvTrans As DataView
        'Dim pBinID As String

        pDtTrans = New DataTable
        With pDtTrans.Columns
            .Add("ProductID")
            .Add("Quantity", GetType(Decimal))
            '.Add("FowstID")
            '.Add("BinID")
        End With

        ' BEGIN add products
        pItems = GetProducts(vID)
        For Each pDR As DataRow In pItems.Rows
            pDrBaseUnit = mParent.LibraryStockMeasurements.GetUnitQty(CStr2(pDR("ProductID")), CStr2(pDR("UnitMeasureID")))
            If pDrBaseUnit Is Nothing Then
                ' todo: do something about non-existing unit_measure-product combo
                pUnitQty = 1
            Else
                pUnitQty = pDrBaseUnit("Quantity")
            End If
            pQty = pUnitQty * CDec2(pDR("Quantity"))

            ' todo: BEGIN: this is a lot of redundant code
            'pBinID = mParent.SystemDefaults.GetOrAddDefaultBin(CStr2(pDR("FowstID")), CStr2(pDR("ProductID")))
            pDvTrans = New DataView(pDtTrans,
                "ProductID = " & ReadyString(pDR("ProductID")),
                "", DataViewRowState.CurrentRows)
            If pDvTrans.Count = 0 Then
                pDrTrans = pDtTrans.NewRow
                pDtTrans.Rows.Add(pDrTrans)
            Else
                pDrTrans = pDvTrans(0).Row
            End If
            pDrTrans("ProductID") = CStr2(pDR("ProductID"))
            pDrTrans("Quantity") = CDec2(pDrTrans("Quantity")) + pQty ' not redundant code: plus sign
            'pDrTrans("FowstID") = CStr2(pDR("FowstID"))
            'pDrTrans("BinID") = pBinID
            ' todo: END: this is a lot of redundant code
        Next pDR
        ' END add products

        ' BEGIN add contents
        pItems = GetContents(vID)
        For Each pDR As DataRow In pItems.Rows
            If pDR("QtyInBaseUnit") Is DBNull.Value Then
                pDrBaseUnit = mParent.LibraryStockMeasurements.GetUnitQty(CStr2(pDR("ProductID")), CStr2(pDR("UnitMeasureID")))
                If pDrBaseUnit Is Nothing Then
                    ' todo: do something about non-existing unit_measure-product combo
                    pUnitQty = 1
                Else
                    pUnitQty = pDrBaseUnit("Quantity")
                End If
                pQty = pUnitQty * CDec2(pDR("Quantity"))
            Else
                pQty = pDR("QtyInBaseUnit")
            End If

            ' todo: BEGIN: this is a lot of redundant code
            'pBinID = mParent.SystemDefaults.GetOrAddDefaultBin(CStr2(pDR("FowstID")), CStr2(pDR("ProductID")))
            pDvTrans = New DataView(pDtTrans,
                "ProductID = " & ReadyString(pDR("ProductID")),
                "", DataViewRowState.CurrentRows)
            If pDvTrans.Count = 0 Then
                pDrTrans = pDtTrans.NewRow
                pDtTrans.Rows.Add(pDrTrans)
            Else
                pDrTrans = pDvTrans(0).Row
            End If
            pDrTrans("ProductID") = CStr2(pDR("ProductID"))
            pDrTrans("Quantity") = CDec2(pDrTrans("Quantity")) - pQty ' not redundant code: minus sign
            'pDrTrans("FowstID") = CStr2(pDR("FowstID"))
            'pDrTrans("BinID") = pBinID
            ' todo: END: this is a lot of redundant code
        Next pDR
        ' END add contents

        Return pDtTrans

    End Function

    Public Function ByID(vID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT ID" &
            ",LastUpdated" &
            ",FowstID" &
            ",UserID" &
            " FROM TransBomMaker" &
            " WHERE ID = " & ReadyString(vID)

        Return mProData.ExecuteRow(pSql)

    End Function

    Public Function AddToInventoryLedger(vID As String, vOrgID As String) As Boolean

        Dim pDrBomMaker As DataRow
        Dim pItems As DataTable
        Dim pQty As Decimal
        Dim pDrBaseUnit As DataRow
        Dim pUnitQty As Decimal

        mProData.BeginTransaction()
        Try
            pDrBomMaker = ByID(vID)
            If pDrBomMaker Is Nothing Then
                Return False
            End If

            ' BEGIN add products
            pItems = GetProducts(vID)
            For Each pDR As DataRow In pItems.Rows
                pDrBaseUnit = mParent.LibraryStockMeasurements.GetUnitQty(CStr2(pDR("ProductID")), CStr2(pDR("UnitMeasureID")))
                If pDrBaseUnit Is Nothing Then
                    ' todo: do something about non-existing unit_measure-product combo
                    pUnitQty = 1
                Else
                    pUnitQty = pDrBaseUnit("Quantity")
                End If
                pQty = pUnitQty * CDec2(pDR("Quantity"))
                mParent.TransInventoryLedger.Add( _
                    vStockID:=CStr2(pDR("ProductID")),
                    vFowstID:=CStr2(pDrBomMaker("FowstID")),
                    vTransactionID:=vID,
                    vTransactionType:=TransInventoryLedger.eTransType.ProductMaker,
                    vUserID:=CStr2(pDrBomMaker("UserID")),
                    vQty:=pQty _
                )
            Next pDR
            ' END add products

            ' BEGIN add raw materials
            pItems = GetContents(vID)
            For Each pDR As DataRow In pItems.Rows
                If pDR("QtyInBaseUnit") Is DBNull.Value Then
                    pDrBaseUnit = mParent.LibraryStockMeasurements.GetUnitQty(CStr2(pDR("ProductID")), CStr2(pDR("UnitMeasureID")))
                    If pDrBaseUnit Is Nothing Then
                        ' todo: do something about non-existing unit_measure-product combo
                        pUnitQty = 1
                    Else
                        pUnitQty = pDrBaseUnit("Quantity")
                    End If
                    pQty = pUnitQty * CDec2(pDR("Quantity"))
                Else
                    pQty = pDR("QtyInBaseUnit")
                End If
                mParent.TransInvoice.AddItemToInventoryLedger( _
                    vProductID:=CStr2(pDR("ProductID")),
                    vOrgID:=vOrgID,
                    vFowstID:=CStr2(pDrBomMaker("FowstID")),
                    vTransactionID:=vID,
                    vUserID:=CStr2(pDrBomMaker("UserID")),
                    vQty:=-pQty,
                    vSubTransactionType:="Usage"
                )

                ' old and wrong:
                'mParent.TransInventoryLedger.Add( _
                '    vStockID:=CStr2(pDR("ProductID")),
                '    vFowstID:=CStr2(pDrBomMaker("FowstID")),
                '    vTransactionID:=vID,
                '    vTransactionType:=TransInventoryLedger.eTransType.ProductMaker,
                '    vUserID:=CStr2(pDrBomMaker("UserID")),
                '    vQty:=-pQty _
                ')
            Next pDR
            ' END add raw materials

            mProData.CommitTransaction()
            Return True
        Catch ex As System.Exception
            mProData.RollbackTransaction(, True)
            Throw
        End Try

    End Function

    ' ''' <summary>
    ' ''' this feels very wrong
    ' ''' </summary>
    ' ''' <param name="vID"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function AddToInventoryLedger_wrong(vID As String) As Boolean

    '    Dim pDrBomMaker As DataRow
    '    Dim pItems As DataTable
    '    Dim pQty As Decimal
    '    Dim pDrBaseUnit As DataRow
    '    Dim pUnitQty As Decimal
    '    Dim pDtLedgerable As DataTable

    '    mProData.BeginTransaction()
    '    Try
    '        pDrBomMaker = ByID(vID)
    '        If pDrBomMaker Is Nothing Then
    '            Return False
    '        End If

    '        pDtLedgerable = GetLedgerable(pDrBomMaker)
    '        For Each pDrLedgerable As DataRow In pDtLedgerable.Rows
    '            If CDec2(pDrLedgerable("Quantity")) <> 0 Then
    '                mParent.TransInventoryLedger.Add( _
    '                    vStockID:=CStr2(pDrLedgerable("ProductID")),
    '                        vFowstID:=CStr2(pDrBomMaker("FowstID")),
    '                        vTransactionID:=vID,
    '                        vTransactionType:=TransInventoryLedger.eTransType.ProductMaker,
    '                        vUserID:=CStr2(pDrBomMaker("UserID")),
    '                        vQty:=CDec2(pDrLedgerable("Quantity")) _
    '                    )
    '            End If
    '        Next pDrLedgerable

    '        ' BEGIN add products
    '        pItems = GetProducts(vID)
    '        For Each pDR As DataRow In pItems.Rows
    '            pDrBaseUnit = mParent.LibraryStockMeasurements.GetUnitQty(CStr2(pDR("ProductID")), CStr2(pDR("UnitMeasureID")))
    '            If pDrBaseUnit Is Nothing Then
    '                ' todo: do something about non-existing unit_measure-product combo
    '                pUnitQty = 1
    '            Else
    '                pUnitQty = pDrBaseUnit("Quantity")
    '            End If
    '            pQty = pUnitQty * CDec2(pDR("Quantity"))
    '            mParent.TransInventoryLedger.Add( _
    '                vStockID:=CStr2(pDR("ProductID")),
    '                vFowstID:=CStr2(pDrBomMaker("FowstID")),
    '                vTransactionID:=vID,
    '                vTransactionType:=TransInventoryLedger.eTransType.ProductMaker,
    '                vUserID:=CStr2(pDrBomMaker("UserID")),
    '                vQty:=pQty _
    '            )
    '        Next pDR
    '        ' END add products

    '        ' BEGIN add contents
    '        pItems = GetContents(vID)
    '        For Each pDR As DataRow In pItems.Rows
    '            If pDR("QtyInBaseUnit") Is DBNull.Value Then
    '                pDrBaseUnit = mParent.LibraryStockMeasurements.GetUnitQty(CStr2(pDR("ProductID")), CStr2(pDR("UnitMeasureID")))
    '                If pDrBaseUnit Is Nothing Then
    '                    ' todo: do something about non-existing unit_measure-product combo
    '                    pUnitQty = 1
    '                Else
    '                    pUnitQty = pDrBaseUnit("Quantity")
    '                End If
    '                pQty = pUnitQty * CDec2(pDR("Quantity"))
    '            Else
    '                pQty = pDR("QtyInBaseUnit")
    '            End If
    '            mParent.TransInventoryLedger.Add( _
    '                vStockID:=CStr2(pDR("ProductID")),
    '                vFowstID:=CStr2(pDrBomMaker("FowstID")),
    '                vTransactionID:=vID,
    '                vTransactionType:=TransInventoryLedger.eTransType.ProductMaker,
    '                vUserID:=CStr2(pDrBomMaker("UserID")),
    '                vQty:=-pQty _
    '            )
    '        Next pDR
    '        ' END add contents

    '        mProData.CommitTransaction()
    '        Return True
    '    Catch ex As System.Exception
    '        mProData.RollbackTransaction(, True)
    '        Throw
    '    End Try

    'End Function

    Private Function GetLedgerable(vDR As DataRow)

        Dim pDtTrans As DataTable
        Dim pDrTrans As DataRow
        Dim pDtLedger As DataTable
        Dim pDvLedger As DataView
        Dim pBinID As String

        pDtTrans = GetCombined(CStr2(vDR("ID")))
        pDtLedger = GetLedger(CStr2(vDR("ID")))

        For Each pDrTrans In pDtTrans.Rows
            pBinID = mParent.SystemDefaults.GetOrAddDefaultBin(CStr2(vDR("FowstID")), CStr2(pDrTrans("ProductID")))
            pDvLedger = New DataView(pDtLedger,
                "ProductID = " & ReadyString(pDrTrans("ProductID")) &
                " AND FowstID = " & ReadyString(vDR("FowstID")) &
                " AND BinID = " & ReadyString(pBinID),
                "", DataViewRowState.CurrentRows)
            If pDvLedger.Count > 0 Then
                pDrTrans("Quantity") = CDec2(pDrTrans("Quantity")) - CDec2(pDvLedger(0)("Qty"))
                pDvLedger(0).Delete()
            End If
        Next pDrTrans

        pDtLedger.AcceptChanges()
        For Each pDrLedger As DataRow In pDtLedger.Rows
            pDrTrans = pDtTrans.NewRow
            pDrTrans("ProductID") = CStr2(pDrLedger("ProductID"))
            pDrTrans("FowstID") = CStr2(pDrLedger("FowstID"))
            pDrTrans("BinID") = CStr2(pDrLedger("BinID"))
            pDrTrans("Quantity") = -CDec2(pDrLedger("Qty"))
            pDtTrans.Rows.Add(pDrTrans)
        Next pDrLedger

        Return pDtTrans

    End Function

    Public Function GetLedger(ByVal vID As String)

        Dim pSql As String
        Dim pDT As DataTable

        pSql = " SELECT ProductID" & vbCrLf &
            " ,FowstID" & vbCrLf &
            " ,BinID" & vbCrLf &
            " ,SUM(Qty) AS Qty" & vbCrLf &
            " FROM TransInventoryLedger" & vbCrLf &
            " WHERE TransactionID = " & ReadyString(vID) & vbCrLf &
            " AND TransactionType = 'Product Maker'" & vbCrLf &
            " GROUP BY  ProductID" & vbCrLf &
            " ,FowstID" & vbCrLf &
            " ,BinID"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Class SaveInputJson
        Public ID As String
        Public UserID As String
        Public FowstID As String
        Public AssignedToID As String
        Public DateNeeded As String
        Public DeviceID As String
        Public SaveAndFinalize As String
        Public OrgID As String
    End Class

    Public Class SaveOutJson
        Public ID As String
    End Class

    Public Function Save(vInput As SaveInputJson)

        Dim pOut = New SaveOutJson()
        Dim pSql As String
        Dim pNum As Integer

        Dim pIU = New InsertUpdate("TransBomMaker")
        pIU("LastUpdated") = "GETDATE()"
        pIU("UserID") = ReadyString(vInput.UserID)
        pIU("FowstID") = ReadyString(vInput.FowstID)
        If String.IsNullOrWhiteSpace(vInput.AssignedToID) = False Then
            pIU("AssignedToID") = ReadyString(vInput.AssignedToID)
        End If
        pIU("DeviceID") = ReadyString(vInput.DeviceID)
        If String.IsNullOrWhiteSpace(vInput.DateNeeded) = False Then
            pIU("DateNeeded") = ReadyString(Format(ParseDate(vInput.DateNeeded), cGlobal.FormatDateTimeSql))
        End If
        If String.IsNullOrWhiteSpace(vInput.ID) Then
            pOut.ID = Guid.NewGuid().ToString()
            pIU("ID") = ReadyString(pOut.ID)
            pIU("DateImported") = "GETDATE()"
            pSql = pIU.GetInsert()
        Else
            pOut.ID = vInput.ID
            pSql = pIU.GetUpdate("ID = " & ReadyString(vInput.ID))
        End If
        mProData.BeginTransaction()
        If CBool2(vInput.SaveAndFinalize) Then
            pNum = GetNextNumber(vInput.FowstID, vInput.DeviceID)
            pIU("Num") = pNum
        End If
        mProData.ExecuteSQL(pSql)
        If CBool2(vInput.SaveAndFinalize) Then
            Me.SaveContents(pOut.ID, vInput.OrgID)
            Me.AddToInventoryLedger(pOut.ID, vInput.OrgID)
        End If

        mProData.CommitTransaction()

        Return pOut

    End Function

    Public Class SaveProductInputJson

        Public ParentID As String
        Public ProductID As String
        Public Items As New List(Of ItemClass)

        Public Class ItemClass
            Public UnitMeasureID As String
            Public Qty As String
        End Class
    End Class

    Public Function SaveProduct(vInput As SaveProductInputJson)

        Dim pIU As New InsertUpdate("TransBomMakerProducts")
        Dim pSql As String

        mProData.BeginTransaction()
        pSql = "DELETE FROM TransBomMakerProducts" & vbCrLf &
            " WHERE ParentID = " & ReadyString(vInput.ParentID) &
            " AND ProductID = " & ReadyString(vInput.ProductID)
        mProData.ExecuteSQL(pSql)

        pIU("ID") = "NEWID()"
        pIU("ParentID") = ReadyString(vInput.ParentID)
        pIU("ProductID") = ReadyString(vInput.ProductID)

        For Each pItem In vInput.Items
            pIU("UnitMeasureID") = ReadyString(pItem.UnitMeasureID)
            pIU("Quantity") = ReadyString(pItem.Qty)
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next pItem
        mProData.CommitTransaction()

    End Function

    Public Class StockInfo
        Public ProductID As String
        Public ProductName As String
        Public ProductNumber As String
        Public BaseUnit As String
        Public BaseUnitQty As Decimal
        Public Units As New List(Of Unit)

        Public Class Unit
            Public MeasureID As String ' MobizLibraryPIMMeasure.ID
            Public UnitMeasure As String
            Public UnitQuantity As Decimal ' MobizLibraryPIMProductMeasurement.Quantity
            Public Qty As Decimal
        End Class
    End Class

    Public Function GetStockInfo(vProductID As String, Optional vReceivingID As String = Nothing) As StockInfo

        Dim pOut As StockInfo
        Dim pDtProduct As DataTable
        Dim pDtUnits As DataTable
        Dim pDtCart As DataTable

        vProductID = ReadyGuid(vProductID)
        pOut = New StockInfo()
        pDtProduct = mParent.LibraryStock.ByIds(ReadyString(vProductID))
        For Each pDR As DataRow In pDtProduct.Rows
            pOut.ProductID = CStr2(pDR("ID"))
            pOut.ProductName = pDR("ProductName")
            pOut.ProductNumber = CStr2(pDR("ProductNo"))
        Next pDR

        pDtUnits = mParent.LibraryStockMeasurements.ByProductID(vProductID)
        For Each pDR As DataRow In pDtUnits.Rows
            pOut.BaseUnit = CStr2(pDR("BaseUnit"))
            pOut.BaseUnitQty = CDec2(pDR("BaseUnitQty"))

            pOut.Units.Add(New StockInfo.Unit With {
                .MeasureID = ReadyGuid(pDR("ID")),
                .UnitMeasure = CStr2(pDR("UnitMeasure")),
                .UnitQuantity = CDec2(pDR("Quantity"))
            })
        Next pDR

        If String.IsNullOrWhiteSpace(vReceivingID) = False Then
            pDtCart = GetStocks(vReceivingID)
            For Each pDR As DataRow In pDtCart.Rows
                If ReadyGuid(pDR("ProductID")) <> vProductID Then
                    Continue For
                End If
                Dim pDR_1 = pDR ' AccessToForEachVariableInClosure
                Dim pUnits =
                    From pUnit In pOut.Units
                    Where pUnit.MeasureID = ReadyGuid(pDR_1("UnitMeasure"))
                    Select pUnit

                For Each pUnit In pUnits
                    pUnit.Qty += CDec2(pDR("Quantity"))
                Next pUnit
            Next pDR
        End If

        Return pOut

    End Function

    Function GetNextNumber(vFowstID As String, vDeviceID As String) As Integer

        Dim pSql = "SELECT MAX(Num)" & vbCrLf &
            " FROM TransBomMaker" & vbCrLf &
            " WHERE FowstID = " & ReadyString(vFowstID) &
            " AND DeviceID = " & ReadyString(vDeviceID)
        Dim pInt = CInt2(mProData.ExecuteScalar(pSql))

        Return pInt + 1

    End Function
End Class