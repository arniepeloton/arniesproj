Public Class CacheInventoryMovement

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Public LogToFile As Boolean

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Enum eTransType
        Invoice
        InventoryCount
        Receiving
        CancelReceiving
        TransferOut
        SalesReturn
        CancelSalesReturn
        ProductMaker
        CarryOver
        InventoryCountAdjustment
        BeginningBalance
        CancelInvoice
        BinRelocation
    End Enum

    Public Sub CacheDataByTransaction(vProductID As String, vFowstID As String, vTransactionDate As Date, vTransactionType As eTransType, vTransactionID As String, pQty As Decimal _
                                        , Optional vLandedcost As Decimal = 0, Optional vInventoryCountDate As Date = Nothing, Optional vCountedBy As String = "")

        Dim pSourceSql As String
        Dim pFinder As Finder
        Dim pSql As String
        'Dim pQty As Double
        Dim dtCacheInventoryMovement As DataTable

        mProData.BeginTransaction()

        pSql = "Select * FROM CacheInventoryMovement" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND convert(date,TransactionDate) = " & ProSqlite.ReadyDate(vTransactionDate)

        dtCacheInventoryMovement = mProData.Execute(pSql)

        Dim pIU As New InsertUpdate("CacheInventoryMovement")

        Select Case vTransactionType
            Case eTransType.Invoice

                'pSql = " SELECT SUM(TransInventoryLedger.Qty) AS Qty" & vbCrLf &
                '        " FROM TransInventoryLedger " & vbCrLf &
                '        " WHERE TransactionID = " & ReadyString(vTransactionID) & vbCrLf &
                '        " AND TransInventoryLedger.ProductID = " & ReadyString(vProductID) & vbCrLf &
                '        " AND TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf
                ''" AND CONVERT(date, TransInventoryLedger.TransactionDate) BETWEEN " & ProSqlite.ReadyDate(vTransactionDate) & " AND " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &


                'pQty = CDec2(mProData.ExecuteScalar(pSql))

                If dtCacheInventoryMovement.Rows.Count = 0 Then
                    pIU("Usage") = pQty
                    pIU("TransOut") = 0
                    pIU("TransIn") = 0
                    pIU("Delivery") = 0
                    pIU("Shortlanded") = 0
                    pIU("Overlanded") = 0
                    pIU("ProductDefect") = 0
                Else
                    pIU("Usage") = CDec2(dtCacheInventoryMovement.Rows(0).Item("Usage")) + pQty
                End If

            Case eTransType.InventoryCount
                Dim NewDate As Date

                NewDate = vTransactionDate.AddDays(1)

                'recompute current balance
                CacheData(vProductID, vFowstID, NewDate)

                'pIU("InventoryCount") = pQty
                'pIU("InventoryCountdateAndtime") = ReadyString(vInventoryCountDate)
                'pIU("CountedBy") = ReadyString(vCountedBy)

            Case eTransType.Receiving

                Dim dtIsDelivery As DataTable
                Dim dtIsShortLanded As DataTable
                Dim dtIsOverLanded As DataTable
                Dim dtProductDefect As DataTable

                pSql = "SELECT * " & vbCrLf &
                        "FROM TransReceiving " & vbCrLf &
                        "WHERE TransReceiving.SupplierID in (select SupplierID from LibraryOrganizationSupplier) " & vbCrLf &
                        "AND TransReceiving.IsReceiving = 1 " & vbCrLf &
                        "AND TransReceiving.ID = " & ReadyString(vTransactionID) & vbCrLf &
                        "AND TransReceiving.ReceivingPlaceID = " & ReadyString(vFowstID) & vbCrLf

                dtIsDelivery = mProData.Execute(pSql)

                pSql = "SELECT * " & vbCrLf &
                     "FROM TransReceiving " & vbCrLf &
                     "INNER JOIN TransReceivingStock on TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
                     "WHERE ISNULL(TransReceivingStock.IsShortLanded,0) = 1 " & vbCrLf &
                     "AND TransReceiving.IsReceiving = 1 " & vbCrLf &
                     "AND TransReceiving.ID = " & ReadyString(vTransactionID) & vbCrLf &
                     "AND TransReceiving.ReceivingPlaceID = " & ReadyString(vFowstID) & vbCrLf

                dtIsShortLanded = mProData.Execute(pSql)

                pSql = "SELECT * " & vbCrLf &
                        "FROM TransReceiving " & vbCrLf &
                        "INNER JOIN TransReceivingStock on TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
                        "WHERE ISNULL(TransReceivingStock.IsOverLanded,0) = 1 " & vbCrLf &
                        "AND TransReceiving.IsReceiving = 1 " & vbCrLf &
                        "AND TransReceiving.ID = " & ReadyString(vTransactionID) & vbCrLf &
                        "AND TransReceiving.ReceivingPlaceID = " & ReadyString(vFowstID) & vbCrLf

                dtIsOverLanded = mProData.Execute(pSql)

                pSql = "SELECT MobizLibraryOrganizationFOWST.Name " & vbCrLf &
                       "FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                       "INNER JOIN TransReceiving ON MobizLibraryOrganizationFOWST.ID = TransReceiving.SupplierID " & vbCrLf &
                       "WHERE MobizLibraryOrganizationFOWST.Name = 'Product Defect' " & vbCrLf &
                       "AND TransReceiving.IsReceiving = 1 " & vbCrLf &
                       "AND TransReceiving.ID = " & ReadyString(vTransactionID) & vbCrLf &
                       "AND TransReceiving.ReceivingPlaceID = " & ReadyString(vFowstID) & vbCrLf

                dtProductDefect = mProData.Execute(pSql)

                If dtIsShortLanded.Rows.Count > 0 Then
                    'shortlanded
                    If dtCacheInventoryMovement.Rows.Count = 0 Then
                        pIU("Shortlanded") = pQty
                        pIU("Delivery") = 0
                        pIU("Usage") = 0
                        pIU("TransIn") = 0
                        pIU("TransOut") = 0
                        pIU("Overlanded") = 0
                        pIU("ProductDefect") = 0
                    Else
                        pIU("Shortlanded") = CDec2(dtCacheInventoryMovement.Rows(0).Item("Shortlanded")) + pQty
                    End If
                ElseIf dtIsOverLanded.Rows.Count > 0 Then
                    'overlanded
                    If dtCacheInventoryMovement.Rows.Count = 0 Then
                        pIU("Overlanded") = pQty
                        pIU("Delivery") = 0
                        pIU("Usage") = 0
                        pIU("TransIn") = 0
                        pIU("TransOut") = 0
                        pIU("Shortlanded") = 0
                        pIU("ProductDefect") = 0
                    Else
                        pIU("Overlanded") = CDec2(dtCacheInventoryMovement.Rows(0).Item("Overlanded")) + pQty
                    End If
                ElseIf dtProductDefect.Rows.Count > 0 Then
                    'product defect
                    If dtCacheInventoryMovement.Rows.Count = 0 Then
                        pIU("ProductDefect") = pQty
                        pIU("Delivery") = 0
                        pIU("Usage") = 0
                        pIU("TransIn") = 0
                        pIU("TransOut") = 0
                        pIU("Shortlanded") = 0
                        pIU("Overlanded") = 0
                    Else
                        pIU("ProductDefect") = CDec2(dtCacheInventoryMovement.Rows(0).Item("ProductDefect")) + pQty
                    End If
                ElseIf dtIsDelivery.Rows.Count > 0 Then
                    'delivery
                    If dtCacheInventoryMovement.Rows.Count = 0 Then
                        pIU("Delivery") = pQty
                        pIU("Usage") = 0
                        pIU("TransIn") = 0
                        pIU("TransOut") = 0
                        pIU("Shortlanded") = 0
                        pIU("Overlanded") = 0
                        pIU("ProductDefect") = 0
                    Else
                        pIU("Delivery") = CDec2(dtCacheInventoryMovement.Rows(0).Item("Delivery")) + pQty
                    End If
                Else
                    'trans in
                    If dtCacheInventoryMovement.Rows.Count = 0 Then
                        pIU("TransIn") = pQty
                        pIU("Delivery") = 0
                        pIU("Usage") = 0
                        pIU("Delivery") = 0
                        pIU("TransOut") = 0
                        pIU("Shortlanded") = 0
                        pIU("Overlanded") = 0
                    Else
                        pIU("TransIn") = CDec2(dtCacheInventoryMovement.Rows(0).Item("TransIn")) + pQty
                    End If
                End If


            Case eTransType.CancelReceiving

            Case eTransType.TransferOut

                'pSql = "SELECT SUM(TransReceivingStock.QuantityGood) * SUM(TransReceivingStock.ActualQuantityGood) AS QuantityGood " & vbCrLf &
                '        "FROM TransReceiving " & vbCrLf &
                '        "JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
                '        "WHERE ISNULL(TransReceiving.IsReceiving, 0) = 0 " & vbCrLf &
                '        "AND TransReceivingStock.ProductID = " & ReadyString(vProductID) & vbCrLf &
                '        "AND TransReceiving.ID = " & ReadyString(vTransactionID) & vbCrLf &
                '        "AND TransReceiving.ReceivingPlaceID = " & ReadyString(vFowstID) & vbCrLf

                'pQty = CDec2(mProData.ExecuteScalar(pSql))


                pQty = pQty * -1

                If dtCacheInventoryMovement.Rows.Count = 0 Then
                    pIU("TransOut") = pQty
                    pIU("Usage") = 0
                    pIU("TransIn") = 0
                    pIU("Delivery") = 0
                    pIU("Shortlanded") = 0
                    pIU("Overlanded") = 0
                Else
                    pIU("TransOut") = CDec2(dtCacheInventoryMovement.Rows(0).Item("TransOut")) + pQty
                End If

            Case eTransType.SalesReturn

            Case eTransType.ProductMaker

            Case eTransType.CarryOver

            Case eTransType.InventoryCountAdjustment

            Case eTransType.BeginningBalance

            Case eTransType.CancelInvoice

            Case eTransType.CancelSalesReturn

            Case eTransType.BinRelocation

            Case Else
                'mParent.ExtraLog("vb9XYX86CQHC! vTransactionID:" & vTransactionID)
                Throw New ArgumentException("vTransactionType = " & vTransactionType)

        End Select

        'Ending Balance

        pSql = "SELECT TOP 1 TransInventoryLedger.Balance " & vbCrLf &
               "FROM TransInventoryLedger " & vbCrLf &
               "WHERE TransInventoryLedger.FowstID = " & ReadyString(vFowstID) & vbCrLf &
               "AND TransInventoryLedger.ProductID = " & ReadyString(vProductID) & vbCrLf &
               "AND CONVERT(date, TransInventoryLedger.TransactionDate) <= " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &
               "ORDER BY TransInventoryLedger.TransactionDate DESC " & vbCrLf

        pQty = CDec2(mProData.ExecuteScalar(pSql))
        pIU("EndingBalance") = pQty

        If dtCacheInventoryMovement.Rows.Count = 0 Then
            'set beginning balance

            pSql = "SELECT TOP 1 TransInventoryLedger.Balance " & vbCrLf &
                   "FROM TransInventoryLedger " & vbCrLf &
                   "WHERE TransInventoryLedger.FowstID = " & ReadyString(vFowstID) & vbCrLf &
                   "AND TransInventoryLedger.ProductID = " & ReadyString(vProductID) & vbCrLf &
                   "AND TransInventoryLedger.TransactionDate < " & ProSqlite.ReadyDate(vTransactionDate.Date) & vbCrLf &
                   "ORDER BY TransInventoryLedger.TransactionDate DESC " & vbCrLf

            pQty = CDec2(mProData.ExecuteScalar(pSql))
            pIU("BeginningBalance") = pQty

            'set Category
            Dim pCategory As String
            Dim pUnitMeasure As String
            Dim pProductName As String
            Dim pProductNo As String
            Dim pFOWSTInformation As DataTable

            pSql = "Select ISNULL(MobizLibraryPIMProductVariableFeatures001.C0020, '') " & vbCrLf &
                    " FROM MobizLibraryPimProduct " & vbCrLf &
                    "JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                    "WHERE MobizLibraryPimProduct.ID = " & ReadyString(vProductID) & vbCrLf

            pCategory = CStr2(mProData.ExecuteScalar(pSql))

            pIU("Category") = ReadyString(pCategory)
            pIU("ProductID") = ReadyString(vProductID)
            pIU("FowstID") = ReadyString(vFowstID)
            pIU("TransactionDate") = ProSqlite.ReadyDate(vTransactionDate.Date)

            pIU("LandedCost") = CDec2(vLandedcost)


            pSql = "Select lbl.Label UnitMeasure FROM " & vbCrLf &
                    " MobizLibraryPIMProductMeasurement as pimmsr  " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID   " & vbCrLf &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                    " WHERE MeasurementLevel = 1 AND pimmsr.ProductID = " & ReadyString(vProductID)

            pUnitMeasure = CStr2(mProData.ExecuteScalar(pSql))

            pIU("UnitMeasure") = ReadyString(pUnitMeasure)

            pSql = "Select MobizLibraryPimProduct.ProductName " & vbCrLf &
                    " FROM MobizLibraryPimProduct " & vbCrLf &
                    "WHERE MobizLibraryPimProduct.ID = " & ReadyString(vProductID) & vbCrLf

            pProductName = CStr2(mProData.ExecuteScalar(pSql))

            pIU("Productname") = ReadyString(pProductName)

            pSql = "Select MobizLibraryPimProduct.ProductNo " & vbCrLf &
                    " FROM MobizLibraryPimProduct " & vbCrLf &
                    "WHERE MobizLibraryPimProduct.ID = " & ReadyString(vProductID) & vbCrLf

            pProductNo = CStr2(mProData.ExecuteScalar(pSql))

            pIU("ProductNo") = ReadyString(pProductNo)

            pSql = "Select Code, Name, LibraryFranchiseGroup.ID, LibraryFranchiseGroup.GroupName from MobizLibraryOrganizationFOWST " & vbCrLf &
                    "LEFT JOIN MobizLibraryOrganizationFranchisee on FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    "LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID " & vbCrLf &
                    "WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(vFowstID) & " AND MobizLibraryOrganizationFranchisee.FranchisorOrgID = " & ReadyString(mParent.OrgID)

            pFOWSTInformation = mProData.Execute(pSql)

            If pFOWSTInformation.Rows.Count > 0 Then
                pIU("FOWSTCode") = ReadyString(pFOWSTInformation.Rows(0).Item("Code"))
                pIU("FOWSTName") = ReadyString(pFOWSTInformation.Rows(0).Item("Name"))
                pIU("GroupName") = ReadyString(pFOWSTInformation.Rows(0).Item("GroupName"))
                If CStr2(pFOWSTInformation.Rows(0).Item("ID")) <> "" Then
                    pIU("FranchiseGroupID") = ReadyString(pFOWSTInformation.Rows(0).Item("ID"))
                End If
            Else
                pSql = "Select Code, Name from MobizLibraryOrganizationFOWST " & vbCrLf &
                    " WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(vFowstID)

                pFOWSTInformation = mProData.Execute(pSql)

                pIU("FOWSTCode") = ReadyString(pFOWSTInformation.Rows(0).Item("Code"))
                pIU("FOWSTName") = ReadyString(pFOWSTInformation.Rows(0).Item("Name"))

            End If

            'insert 

            pSql = pIU.GetInsert()
        Else

            pSql = pIU.GetUpdate(" ProductID = " & ReadyString(vProductID) & vbCrLf &
                                 " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
                                 " AND TransactionDate = " & ProSqlite.ReadyDate(vTransactionDate))
        End If

        Try
            Dim pInt = mProData.ExecuteSQL(pSql) ' output is for debugging purposes only
        Catch ex As ProSqlException
            Throw New ApplicationException(ex.Message & vbCrLf & ex.Sql, ex)
        Catch ex As Exception
            Throw
        End Try

        mProData.CommitTransaction()

    End Sub

    Public Function CacheData(vProductID As String, vFowstID As String, vTransactionDate As Date) As Integer

        Dim pSourceSql As String
        Dim pFinder As Finder

        pFinder = New Finder
        With pFinder
            With .Fields
                With .Add("Category", SqlDbType.Bit)
                    .Expression = "ISNULL(MobizLibraryPIMProductVariableFeatures001.C0020, '')"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPimProduct.ID"
                End With
                With .Add("ProductNo", SqlDbType.Bit)
                    .Expression = "MobizLibraryPimProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.Bit)
                    .Expression = "MobizLibraryPimProduct.ProductName"
                End With
                With .Add("BeginningBalance", SqlDbType.Bit)
                    .Expression = "ISNULL(tblBeginningBalance.Balance,0)"
                End With
                With .Add("Delivery", SqlDbType.Int)
                    .Expression = "ISNULL(tblDelivery.QuantityGood,0)"
                End With
                With .Add("TransIn", SqlDbType.Bit)
                    .Expression = "ISNULL(tblTransIn.QuantityGood,0)"
                End With
                With .Add("TransOut", SqlDbType.Bit)
                    .Expression = "ISNULL(tblTransOut.QuantityGood,0)"
                End With
                With .Add("EndingBalance", SqlDbType.Bit)
                    .Expression = "ISNULL(tblEndingBalance.Balance,0)"
                End With
                With .Add("Usage", SqlDbType.Bit)
                    .Expression = "ISNULL(tblUsage.Qty,0)"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPimProduct " & vbCrLf &
                        "JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPimProduct.ID " & vbCrLf

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT TOP 1 TransInventoryLedger.Balance " & vbCrLf &
                        "FROM TransInventoryLedger " & vbCrLf &
                        "WHERE TransInventoryLedger.FowstID = " & ReadyString(vFowstID) & vbCrLf &
                        "AND TransInventoryLedger.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionDate < " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &
                        "ORDER BY TransInventoryLedger.TransactionDate DESC " & vbCrLf &
                        ") tblBeginningBalance " & vbCrLf


            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS QuantityGood " & vbCrLf &
                        "FROM TransReceiving " & vbCrLf &
                        "JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
                        "WHERE TransReceiving.SupplierID <> " & ReadyString(mParent.OrgID) & " " & vbCrLf &
                        "AND TransReceiving.IsReceiving = 1 " & vbCrLf &
                        "AND TransReceivingStock.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        "AND CONVERT(date, TransReceiving.LastUpdated) BETWEEN " & ProSqlite.ReadyDate(vTransactionDate) & " AND " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &
                        "AND TransReceiving.ReceiveInOutlet = " & ReadyString(vFowstID) & vbCrLf &
                        ") tblDelivery "

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS QuantityGood " & vbCrLf &
                        "FROM TransReceiving " & vbCrLf &
                        "JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
                        "WHERE TransReceiving.IsReceiving = 1 " & vbCrLf &
                        "AND TransReceivingStock.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        "AND CONVERT(date, TransReceiving.LastUpdated) BETWEEN " & ProSqlite.ReadyDate(vTransactionDate) & " AND " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &
                        "AND TransReceiving.ReceivingPlaceID = " & ReadyString(vFowstID) & vbCrLf &
                        ") tblTransIn "

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS QuantityGood " & vbCrLf &
                        "FROM TransReceiving " & vbCrLf &
                        "JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
                        "WHERE ISNULL(TransReceiving.IsReceiving, 0) = 0 " & vbCrLf &
                        "AND TransReceivingStock.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        "AND CONVERT(date, TransReceiving.LastUpdated) BETWEEN " & ProSqlite.ReadyDate(vTransactionDate) & " AND " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &
                        "AND TransReceiving.ReceivingPlaceID = " & ReadyString(vFowstID) & vbCrLf &
                        ") tblTransOut " & vbCrLf

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT TOP 1 TransInventoryLedger.Balance " & vbCrLf &
                        "FROM TransInventoryLedger " & vbCrLf &
                        "WHERE TransInventoryLedger.FowstID = " & ReadyString(vFowstID) & vbCrLf &
                        "AND TransInventoryLedger.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        "AND CONVERT(date, TransInventoryLedger.TransactionDate) <= " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &
                        "ORDER BY TransInventoryLedger.TransactionDate DESC " & vbCrLf &
                        ") tblEndingBalance " & vbCrLf

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        " SELECT SUM(TransInventoryLedger.Qty) AS Qty" & vbCrLf &
                        " FROM TransInventoryLedger " & vbCrLf &
                        " WHERE TransInventoryLedger.FowstID = " & ReadyString(vFowstID) & vbCrLf &
                        " AND TransInventoryLedger.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        " AND CONVERT(date, TransInventoryLedger.TransactionDate) BETWEEN " & ProSqlite.ReadyDate(vTransactionDate) & " AND " & ProSqlite.ReadyDate(vTransactionDate) & vbCrLf &
                        " AND TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf &
                        ") tblUsage " & vbCrLf

            .SqlWhere = " WHERE MobizLibraryPimProduct.ID = " & ReadyString(vProductID) & vbCrLf &
                " AND (ISNULL(tblBeginningBalance.Balance, 0) <> ISNULL(tblEndingBalance.Balance,0) " & vbCrLf &
                " OR ISNULL(tblDelivery.QuantityGood,0) <> 0" & vbCrLf &
                " OR ISNULL(tblTransIn.QuantityGood,0) <> 0" & vbCrLf &
                " OR ISNULL(tblTransOut.QuantityGood,0) <> 0" & vbCrLf &
                " OR ISNULL(tblUsage.Qty,0) <> 0" & vbCrLf &
                ")" & vbCrLf

            .SqlOrder = " ORDER BY Category,ProductName"
            pSourceSql = .CreateSql()
            If LogToFile Then
                AppendToLog("vbZGR6B45D6J! CacheData" &
                    "vProductID: " & vProductID & vbCrLf &
                    "vFowstID: " & vFowstID & vbCrLf &
                    "vTransactionDate: " & vTransactionDate & vbCrLf &
                    pSourceSql)
            End If
        End With

        mProData.BeginTransaction()
        Dim pSql = "DELETE FROM CacheInventoryMovement" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vProductID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND TransactionDate = " & ProSqlite.ReadyDate(vTransactionDate)
        mProData.ExecuteSQL(pSql)

        Dim pIU As New InsertUpdate("CacheInventoryMovement")
        pIU("Category") = "Category"
        pIU("ProductID") = "ProductID"
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("Delivery") = "Delivery"
        pIU("TransIn") = "TransIn"
        pIU("TransOut") = "TransOut"
        pIU("BeginningBalance") = "BeginningBalance"
        pIU("EndingBalance") = "EndingBalance"
        pIU("Usage") = "Usage"
        pIU("TransactionDate") = ProSqlite.ReadyDate(vTransactionDate.Date)
        pIU.FromSql = "FROM (" & pSourceSql & ") t"
        pSql = pIU.GetInsert(True)
        Try
            Dim pInt = mProData.ExecuteSQL(pSql) ' output is for debugging purposes only
            mProData.CommitTransaction()
            Return pInt
        Catch ex As Exception
            AppendToLog("vbTD5KU3FCXW! CacheData" &
                "vProductID: " & vProductID & vbCrLf &
                "vFowstID: " & vFowstID & vbCrLf &
                "vTransactionDate: " & vTransactionDate & vbCrLf &
                "Error: " & ex.Message & vbCrLf &
                pSourceSql)
            Throw
        End Try

    End Function

    Public Function CacheData(vTransactionDate As Date) As Integer

        Dim pOut As Integer
        Dim pDtProductFowst = UniqueProductFowst(vTransactionDate)
        For Each pDR As DataRow In pDtProductFowst.Rows
            pOut += CacheData(
                vProductID:=CStr2(pDR("ProductID")),
                vFowstID:=CStr2(pDR("FowstID")),
                vTransactionDate:=vTransactionDate
            )
        Next
        Return pOut

    End Function

    Function UniqueProductFowst(vTransactionDate As Date) As DataTable

        Dim pSql = "SELECT DISTINCT CacheInventoryMovement.ProductID" & vbCrLf &
            ",CacheInventoryMovement.FowstID" & vbCrLf &
            " FROM CacheInventoryMovement" & vbCrLf &
            " WHERE CONVERT(Date, CacheInventoryMovement.TransactionDate) = " & ProSqlite.ReadyDate(vTransactionDate.Date)
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function
End Class