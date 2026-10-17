Public Class LibraryPriceCanvass

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function CreateFromPurchaseOrder(vPoID As String) As String

        Dim pDrInvoice As DataRow
        Dim pDtItems As DataTable
        Dim pCanvassID As String
        Dim pItemID As String
        Dim pCanvassVendorID As String
        Dim pCanvassSkuID As String

        mProData.BeginTransaction()
        pDrInvoice = mParent.TransInvoice.ById(vPoID)
        If pDrInvoice Is Nothing Then
            Throw New ApplicationException("PO does not exist. ref: vb7HQ7AV4PLD!")
        End If
        pDtItems = mParent.TransInvoice.GetItems(vPoID, False)
        pCanvassID = CreateCanvass(CStr2(pDrInvoice("UserID")))
        For Each pDrItem As DataRow In pDtItems.Rows
            pItemID = CreateItem(pCanvassID, CDec2(pDrItem("Quantity")), mParent.LibraryStockMeasurements.GetOrCreateUnitMeasureID(pDrItem("UnitMeasure")), CStr2(pDrItem("ProductName")))
            pCanvassSkuID = CreateSku(pItemID, CStr2(pDrItem("ProductID")))
            If pCanvassVendorID Is Nothing Then
                pCanvassVendorID = CreateVendor(pCanvassID, CStr2(pDrInvoice("CustomerID")))
            End If
            Dim pUnitQty As Integer
            pUnitQty = CInt2(pDrItem("ActualQuantity"))
            If pUnitQty <= 0 Then
                pUnitQty = 1
            End If
            CreateVendorPrice(pCanvassVendorID, pCanvassSkuID, CDec2(pDrItem("UnitPrice")) / pUnitQty)
            UpdateLibraryVendorPrices(CStr2(pDrInvoice("CustomerID")), CStr2(pDrItem("ProductID")), CDec2(pDrItem("UnitPrice")), pUnitQty,
                vUserID:=CStr2(pDrInvoice("UserID")),
                vUnitMeasure:=pDrItem("UnitMeasure"))
        Next pDrItem
        mProData.CommitTransaction()

    End Function

    Private Function CreateCanvass(vUserID As String) As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String

        pID = Guid.NewGuid().ToString()
        pIU = New InsertUpdate("LibraryPriceCanvass")
        pIU("ID") = ReadyString(pID)
        pIU("CanvassName") = "'Created from PO'"
        pIU("CanvassNumber") = GetNextNumber()
        pIU("CreatedBy") = ReadyString(vUserID)
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Private Function CreateItem(vCanvassID As String, vQty As Decimal, vUnitID As String, vDescription As String) As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String

        pID = Guid.NewGuid().ToString()
        pIU = New InsertUpdate("LibraryPriceCanvassItems")
        pIU("ID") = ReadyString(pID)
        pIU("CanvassID") = ReadyString(vCanvassID)
        pIU("Description") = ReadyString(vDescription)
        pIU("Qty") = vQty
        pIU("UnitOfMeasure") = ReadyString(vUnitID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Private Function CreateSku(vCanvassItemID As String, vProductID As String) As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String

        pID = Guid.NewGuid().ToString()
        pIU = New InsertUpdate("LibraryPriceCanvassItemSku")
        pIU("ID") = ReadyString(pID)
        pIU("CanvassItemID") = ReadyString(vCanvassItemID)
        pIU("SkuID") = ReadyString(vProductID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Private Function CreateVendor(vCanvassID As String, vVendorID As String) As String

        Dim pIU As InsertUpdate
        Dim pID As String
        Dim pSql As String

        pID = Guid.NewGuid().ToString()
        pIU = New InsertUpdate("LibraryPriceCanvassVendor")
        pIU("ID") = ReadyString(pID)
        pIU("CanvassID") = ReadyString(vCanvassID)
        pIU("VendorID") = ReadyString(vVendorID)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Private Sub CreateVendorPrice(vCanvassVendorID As String, vCanvassItemSkuID As String, vPrice As Decimal)

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("LibraryPriceCanvassVendorPrice")
        pIU("ID") = "NEWID()"
        pIU("CanvassVendorID") = ReadyString(vCanvassVendorID)
        pIU("CanvassItemSkuID") = ReadyString(vCanvassItemSkuID)
        pIU("Price") = ReadyString(vPrice)
        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)
        
    End Sub

    Function GetNextNumber() As Integer

        Dim pSql As String = "SELECT MAX(CanvassNumber) as CanvassNumber FROM LibraryPriceCanvass"
        Dim pDt As DataTable = Common.WebRequest.ProData.Execute(pSql)

        If pDt.Rows.Count > 0 Then
            Return CInt2(pDt.Rows(0)("CanvassNumber")) + 1
        Else
            Return 1
        End If

    End Function

    Public Sub UpdateLibraryVendorPrices(vVendorID As String, vProductID As String, vPrice As Decimal,
        vUnitQty As Decimal, Optional vIsActive As String = Nothing, Optional vUserID As String = "",
        Optional vUnitMeasure As String = "", Optional vUrl As String = "", Optional vNote As String = "")

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pAffectedRecords As Integer

        Try
            pIU = New InsertUpdate("LibraryVendorPrices")
            If vUnitQty <= 0 Then
                vUnitQty = 1
            End If
            pIU("Price") = vPrice / vUnitQty
            pIU("UnitPrice") = vPrice
            pIU("UnitMeasure") = ReadyString(vUnitMeasure)
            pIU("UnitQty") = vUnitQty
            pIU("LastUpdated") = "GETDATE()"

            If Not String.IsNullOrWhiteSpace(vUrl) Then
                pIU("Url") = ReadyString(vUrl)
            End If

            If Not String.IsNullOrWhiteSpace(vNote) Then
                pIU("Note") = ReadyString(vNote)
            End If

            If Not vIsActive Is Nothing Then
                pIU("IsActive") = CInt2(vIsActive)
            End If

            If String.IsNullOrWhiteSpace(vUserID) Then
                vUserID = WebSession.Instance.PeopleUserID
            End If

            pIU("UpdatedBy") = IIf(vUserID = "", "NULL", ReadyString(vUserID))
            pSql = pIU.GetUpdate("VendorID = " & ReadyString(vVendorID) & vbCrLf &
                " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
                " AND ISNULL(UnitQty, 1) = " & vUnitQty)
            pAffectedRecords = mProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pIU("ID") = "NEWID()"
                pIU("VendorID") = ReadyString(vVendorID)
                pIU("ProductID") = ReadyString(vProductID)
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            End If
        Catch ex As ProSqlException
            AppendToLog("vbGSNCVKYBDU! " & ex.Message & vbCrLf &
                "SQL: " & ex.Sql)
            Throw
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Function GetVendorPrice(vVendorID As String, vProductID As String) As Decimal

        Dim pDR = GetVendorPriceInfo(vVendorID, vProductID)
        If pDR IsNot Nothing Then
            Return CDec2(pDR("Price"))
        End If

    End Function

    Function GetVendorPriceInfo(vVendorID As String, vProductID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT Price" & vbCrLf &
            ",LastUpdated" & vbCrLf &
            " FROM LibraryVendorPrices" & vbCrLf &
            " WHERE VendorID = " & ReadyString(vVendorID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function ConvertToLandedCost(vVendorID As String, vProductID As String, vDT As DataTable) As DataTable

        Dim pLandedCostID As String
        Dim pLandedCostName As String = "LANDED COST"
        Dim pVendorPrice As Decimal

        pLandedCostID = mParent.SystemDefaults.GetLandedCostID()
        If String.IsNullOrWhiteSpace(pLandedCostID) Then
            Return vDT
        End If
        pVendorPrice = GetVendorPrice(vVendorID, vProductID)
        'If pVendorPrice = 0 Then
        '    Return vDT
        'End If
        For Each pDR As DataRow In vDT.Rows
            pDR("PriceLevelID") = New Guid(pLandedCostID)
            pDR("PriceLevel") = pLandedCostName
            pDR("OriginalPrice") = pVendorPrice * CDec2(pDR("Quantity"))
            pDR("DiscountedPrice") = pVendorPrice * CDec2(pDR("Quantity"))
            pDR("DiscountedPricePerUnit") = pVendorPrice
        Next pDR

        Return vDT

    End Function

    Public Function LatestVendorProductCanvass(vVendorID As String, vProductID As String) As DataRow

        Dim pSql = " -- latest product canvass" & vbCrLf &
            " SELECT TOP 10 MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " ,LibraryPriceCanvassVendorPrice.Price AS UnitPrice" & vbCrLf &
            " ,LibraryPriceCanvass.LastUpdated" & vbCrLf &
            " ,LibraryPriceCanvass.CanvassNumber" & vbCrLf &
            " ,MobizLibraryPIMProductMeasurement.Quantity AS ActualQuantity" & vbCrLf &
            " FROM LibraryPriceCanvass" & vbCrLf &
            " JOIN LibraryPriceCanvassItems ON LibraryPriceCanvassItems.CanvassID = LibraryPriceCanvass.ID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryPriceCanvassItems.SkuID" & vbCrLf &
            " JOIN LibraryPriceCanvassVendor ON LibraryPriceCanvassVendor.CanvassID = LibraryPriceCanvass.ID" & vbCrLf &
            " JOIN LibraryPriceCanvassVendorPrice ON LibraryPriceCanvassVendorPrice.CanvassVendorID = LibraryPriceCanvassVendor.ID" & vbCrLf &
            " AND LibraryPriceCanvassVendorPrice.CanvassItemSkuID = LibraryPriceCanvassItems.ID" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryPriceCanvassVendor.VendorID" & vbCrLf &
            " JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurement.UnitMeasureID = LibraryPriceCanvassItems.UnitOfMeasure" & vbCrLf &
            " AND MobizLibraryPIMProductMeasurement.ProductID = LibraryPriceCanvassItems.SkuID" & vbCrLf &
            " WHERE LibraryPriceCanvass.IsActive = 1" & vbCrLf &
            " AND LibraryPriceCanvass.IsSaveAsFinal = 1" & vbCrLf &
            " AND LibraryPriceCanvassVendor.VendorID = " & ReadyString(vVendorID) & vbCrLf &
            " AND LibraryPriceCanvassItems.SkuID = " & ReadyString(vProductID) & vbCrLf &
            " ORDER BY LibraryPriceCanvass.LastUpdated DESC"
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function
End Class