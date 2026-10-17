Public Class WholesaleCheckPriceRequest

    Dim mProData As ProData
    Dim mBusiness As Business
    Dim mSalesOrderID As String
    Dim mDR As DataRow
    Dim mDtItems As DataTable
    Dim mLandedCostID As String
    Dim mMessage As String
    Dim mResult As Boolean
    Dim mRequestedPriceCount As Integer

    Public Sub New(vBusiness As Business, vSalesOrderID As String)

        mBusiness = vBusiness
        mProData = mBusiness.ProData
        mSalesOrderID = vSalesOrderID
        mLandedCostID = mBusiness.SystemDefaults.GetLandedCostID
        GetData()
        mResult = HandleRequestedPrices(False)

    End Sub

    Public ReadOnly Property Message As String
        Get

            Return mMessage

        End Get
    End Property

    Public ReadOnly Property Result As Boolean
        Get

            Return mResult

        End Get
    End Property

    Public ReadOnly Property RequestedPriceCount As Integer
        Get

            Return mRequestedPriceCount

        End Get
    End Property

    'Private Function HandleRequestedPrices(ByVal vMode As Byte) As Boolean
    Private Function HandleRequestedPrices(ByVal vUserAdmin As Boolean) As Boolean
        Dim pMinimumLineGPM As Double
        Dim pMinimumTotalGpm As Double
        Dim pOnlyHigherPrices

        Dim pLineGPM As Decimal
        Dim pTotalGP As Double
        Dim pTotalGPM As Double

        Dim pBelowMinimumGPM As Boolean
        Dim pYearPurchase As Decimal
        Dim pCurrentPurchase As Decimal
        Dim pModeSupplierPrice As Decimal
        Dim pRemarkRequestPrice As String

        Dim pForApproval As Boolean

        '   vMode: 0 = view only
        '        : 1 = save comment
        '        : 2 = approve requested price

        Try
            pOnlyHigherPrices = True
            pTotalGP = GetTotalLineGP(True)
            pCurrentPurchase = GetTotalAmount(True)
            pTotalGPM = pTotalGP / pCurrentPurchase * 100

            If Not mBusiness.LibraryCustomers.GetMinimumProfit(CStr2(mDR("CustomerID")), pMinimumLineGPM, pMinimumTotalGpm) Then
                pMinimumLineGPM = mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Line GPM", 8)
                pMinimumTotalGpm = mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Total GPM", 20)
            End If

            'MsgBox("pMinimumLineGPM :" & pMinimumLineGPM)
            'MsgBox("pMinimumTotalGpm :" & pMinimumTotalGpm)

            mMessage = mMessage & "Checking 1st chance, 1st condition" & vbCrLf

FirstChance:
            '---Verify line GPM's

            For Each pDrItem As DataRow In mDtItems.Rows
                If pDrItem.RowState <> DataRowState.Deleted Then
                    If pDrItem("RequestedUnitPrice") IsNot DBNull.Value Then
                        mRequestedPriceCount += 1
                        'MsgBox(ItemData("StockID").ToString.ToUpper)
                        If pDrItem("RequestedUnitPrice") > 0 Then
                            '---Oct 14, 2010: obsolete. replaced by the condition below\
                            'MsgBox(ItemData("StockDescription") & " " & gBusiness.LibraryStock.GetPrice(CStr2(ItemData("StockID")), ItemData("ActualQuantity"), CStr2(ItemData("PriceLevelID"))))
                            'If ItemData("RequestedUnitPrice") < ItemData("UnitPrice") Then
                            If pDrItem("RequestedUnitPrice") < GetPrice(CStr2(pDrItem("StockID")), pDrItem("ActualQuantity"), CStr2(pDrItem("PriceLevelID"))) Then
                                pOnlyHigherPrices = False
                            End If
                            pLineGPM = (pDrItem("RequestedUnitPrice") - pDrItem("CurrentCost")) * pDrItem("Quantity") / (pDrItem("RequestedUnitPrice") * pDrItem("Quantity")) * 100

                            If pLineGPM < 0 Then
                                'MsgBox("Verify PlineGMP")
                                mMessage = "The requested unit price of item " & " (" & pDrItem("StockNumber") & ") is below cost."
                                ''ShowMessage(pMessage, "Request Unit Price", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                Exit Function
                            ElseIf pMinimumLineGPM <= pLineGPM Then
                                '---Passed
                                mMessage = mMessage & "PASSED: The requested unit price of item #" & pDrItem("StockNumber") & " (" & pDrItem("StockNumber") & ") is accepted. "
                                mMessage = mMessage & "Its line GPM"

                                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                                '    pMessage = pMessage & " (" & Format(pLineGPM, "#,##0.00;(#,##0.00);-") & "%)"
                                'End If
                                mMessage = mMessage & " passed the theshold line GPM "

                                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                                'pMessage = pMessage & " (" & Format(pMinimumLineGPM, "#,##0.00;(#,##0.00);-") & "%)"
                                'End If

                                mMessage = mMessage & "." & vbCrLf
                            Else
                                mMessage = mMessage & "FAILED: The requested unit price of item #" & " (" & pDrItem("StockNumber") & ") is too low. "
                                mMessage = mMessage & "Its line GPM"

                                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                                '    pMessage = pMessage & " (" & Format(pLineGPM, "#,##0.00;(#,##0.00);-") & "%)"
                                'End If

                                mMessage = mMessage & " is below the theshold line GPM "

                                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                                '    pMessage = pMessage & " (" & Format(pMinimumLineGPM, "#,##0.00;(#,##0.00);-") & "%)"
                                'End If
                                mMessage = mMessage & "." & vbCrLf

                                HandleRequestedPrices = False
                                pForApproval = True
                                If pOnlyHigherPrices = False Then GoTo SecondChance
                            End If
                        Else ' If pDrItem("RequestedUnitPrice") > 0 Then
                            HandleRequestedPrices = False
                            ''ShowMessage("Requested Unit Price must be greater than zero.", "Request Unit Price", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            Exit Function
                        End If ' If pDrItem("RequestedUnitPrice") > 0 Then
                    End If ' If pDrItem("RequestedUnitPrice") IsNot DBNull.Value Then

                End If ' If pDrItem.RowState <> DataRowState.Deleted Then
            Next

            If mRequestedPriceCount = 0 Then
                mMessage = Nothing
                Return True
            End If

            '---Verify total GPM's
            mMessage = mMessage & "Checking 1st chance, 2nd condition" & vbCrLf
            If pMinimumTotalGpm <= pTotalGPM Then
                mMessage = mMessage & "PASSED: The total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pTotalGPM, cGlobal.FormatCurrencyGrid) & "%)"
                'End If

                mMessage = mMessage & " passed the threshhold total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pMinimumTotalGpm, cGlobal.FormatCurrencyGrid) & "%)"
                'End If

                mMessage = mMessage & "." & vbCrLf

                GoTo ForApproval
            Else
                mMessage = mMessage & "FAILED: The total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pTotalGPM, cGlobal.FormatCurrencyGrid) & "%)"
                'End If

                mMessage = mMessage & " is below the threshhold total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pMinimumTotalGpm, cGlobal.FormatCurrencyGrid) & "%)"
                'End If

                mMessage = mMessage & "." & vbCrLf
            End If

SecondChance:
            mMessage = mMessage & "Checking 2nd chance, 1st optional condition" & vbCrLf
            If pCurrentPurchase >= CDec2(mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Purchase", 20000)) Then
                mMessage = mMessage & "PASSED: This transaction's total (" & Format(pCurrentPurchase, cGlobal.FormatCurrency) & ") has passed the threshold (" & Format(mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Purchase", 20000), "Standard") & ")." & vbCrLf
            Else
                mMessage = mMessage & "FAILED: This transaction's total (" & Format(pCurrentPurchase, cGlobal.FormatCurrency) & ") is below the threshold (" & Format(mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Purchase", 20000), "Standard") & ")." & vbCrLf

                'MsgBox("Minimum Purchase : " & Format(gBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Purchase", 20000), "Standard"))
                'MsgBox(gBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Purchase", 20000))
                mMessage = mMessage & "Checking 2nd chance, 2nd optional condition" & vbCrLf
                pYearPurchase = mBusiness.LibraryCustomers.TotalAmountPurchaseLastDays(CStr2(mDR("CustomerID")), 365) + pCurrentPurchase
                If pYearPurchase >= CDec2(mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Year Purchase", 250000)) Then
                    mMessage = mMessage & "PASSED: This customer's total purchase for the last 365 day (" & Format(pYearPurchase, cGlobal.FormatCurrency) & ") has passed the threshold (" & Format(mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Year Purchase", 250000), "Standard") & ")." & vbCrLf
                Else
                    mMessage = mMessage & "FAILED: This customer's total purchase for the last 365 day (" & Format(pYearPurchase, cGlobal.FormatCurrency) & ") is below the threshold (" & Format(mBusiness.SystemDefaults.GetOrAddSystemDefault("Minimum Year Purchase", 250000), "Standard") & ")." & vbCrLf
                    pForApproval = True
                    GoTo ForApproval
                End If
            End If

            '---Verify current total GPM against minimum total GPM per business type
            pMinimumTotalGpm = mBusiness.LibraryCustomers.MinimumTotalGPM(CStr2(mDR("CustomerID")))
            mMessage = mMessage & "Checking 2nd chance, 2nd condition" & vbCrLf
            If pMinimumTotalGpm <= pTotalGPM Then
                mMessage = mMessage & "PASSED: The total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pTotalGPM, cGlobal.FormatCurrency) & "%)"
                'End If

                mMessage = mMessage & " passed the business type's threshhold total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pMinimumTotalGpm, cGlobal.FormatCurrency) & "%)"
                'End If

                mMessage = mMessage & "." & vbCrLf
            Else
                mMessage = mMessage & "FAILED: The total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pTotalGPM, cGlobal.FormatCurrencyGrid) & "%)"
                'End If
                mMessage = mMessage & " is below the business type's threshhold total GPM"

                'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                '    pMessage = pMessage & " (" & Format(pMinimumTotalGpm, cGlobal.FormatCurrencyGrid) & "%)"
                'End If

                mMessage = mMessage & "." & vbCrLf
                pForApproval = True
                GoTo ForApproval
            End If

            pMinimumLineGPM = mBusiness.LibraryCustomers.MinimumLineGPM(CStr2(mDR("CustomerID")))

            mMessage = mMessage & "Checking 2nd chance, 3rd condition" & vbCrLf

            For Each pDrItem As DataRow In mDtItems.Rows
                pLineGPM = (pDrItem("UnitPrice") - pDrItem("CurrentCost")) * pDrItem("Quantity") / (pDrItem("UnitPrice") * pDrItem("Quantity")) * 100

                'pLineGPM = (arDataArray(grdStock.Columns("UnitPrice").ColIndex, p) - arDataArray(grdStock.Columns("Current Cost").ColIndex, p)) _
                '    * arDataArray(grdStock.Columns("Quantity").ColIndex, p) / (arDataArray(grdStock.Columns("UnitPrice").ColIndex, p) _
                '    * arDataArray(grdStock.Columns("Quantity").ColIndex, p)) * 100

                pModeSupplierPrice = mBusiness.LibraryStockLegacy.GetModeSupplierPrice(CStr2(pDrItem("StockID"))) * CDec2(pDrItem("ActualQuantity"))
                'pModeSupplierPrice = gLibStock.GetModeSupplierPrice(arDataArray(0, p)) * ReplaceNull(arDataArray(7, p)) ' 7 = actial quantity
                If pModeSupplierPrice = 0 Then
                    'pMessage = pMessage & "FAILED: The mode supplier price of item #" & p + 1 & " (" & arDataArray(1, p) & ") is zero." & vbCrLf
                    mMessage = mMessage & "FAILED: The mode supplier price of item " & " (" & pDrItem("StockNumber") & ") is zero." & vbCrLf
                    pForApproval = True
                    GoTo ForApproval
                ElseIf pModeSupplierPrice <= CDec2(pDrItem("CurrentCost")) Then
                    ' passed
                    mMessage = mMessage & "PASSED: The mode supplier price "

                    'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                    '    pMessage = pMessage & "(" & Format(pModeSupplierPrice, cGlobal.FormatCurrency) & ")"
                    'End If

                    mMessage = mMessage & " of item " & " (" & pDrItem("StockNumber") & ") "
                    mMessage = mMessage & " is lower than its landed cost"

                    'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                    '    pMessage = pMessage & "(" & Format(CDec2(ItemData("CurrentCost")), cGlobal.FormatCurrency) & ")"
                    'End If
                    mMessage = mMessage & "." & vbCrLf

                    '---check line GPM against the business type
                    If pMinimumLineGPM <= CDec2(pDrItem("LineGPM")) Then
                        '---Passed
                        mMessage = mMessage & "PASSED: The requested unit price of item " & " (" & pDrItem("StockNumber") & " is accepted. "
                        mMessage = mMessage & "Its line GPM"

                        'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                        '    pMessage = pMessage & " (" & Format(pLineGPM, cGlobal.FormatCurrencyGrid) & "%)"
                        'End If
                        mMessage = mMessage & " passed the business type's theshold line GPM "

                        'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                        '    pMessage = pMessage & " (" & Format(pMinimumLineGPM, cGlobal.FormatCurrencyGrid) & "%)"
                        'End If

                        mMessage = mMessage & "." & vbCrLf
                    Else
                        mMessage = mMessage & "FAILED: The requested unit price of item " & " (" & pDrItem("StockNumber") & ") is too low. "
                        mMessage = mMessage & "Its line GPM"

                        'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                        '    pMessage = pMessage & " (" & Format(pLineGPM, cGlobal.FormatCurrencyGrid) & "%)"
                        'End If

                        mMessage = mMessage & " is below the business type's theshold line GPM "

                        'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                        '    pMessage = pMessage & " (" & Format(pMinimumLineGPM, cGlobal.FormatCurrencyGrid) & "%)"
                        'End If

                        mMessage = mMessage & "." & vbCrLf
                        pForApproval = True
                        GoTo ForApproval
                    End If
                Else
                    '---Failed
                    mMessage = mMessage & "FAILED: The mode supplier price "

                    'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                    '    pMessage = pMessage & "(" & Format(pModeSupplierPrice, cGlobal.FormatCurrencyGrid) & ")"
                    'End If

                    mMessage = mMessage & " of item " & " (" & pDrItem("StockNumber") & ") "
                    mMessage = mMessage & " is higher than its landed cost"

                    'If gBusiness.MainSecurity.UserAccess.SalesOrder_ShowProfitabilityColumns.IsEnable Then
                    '    pMessage = pMessage & "(" & Format(CDec2(ItemData("CurrentCost")), cGlobal.FormatCurrency) & ")"
                    'End If

                    mMessage = mMessage & "." & vbCrLf
                    pForApproval = True
                    GoTo ForApproval
                End If
            Next

ForApproval:

            If pForApproval _
                And pOnlyHigherPrices = False Then

                'Select Case vMode
                '    Case 0
                '        pMessage = "This transaction has a requested price that is below acceptable values." & vbCrLf & pMessage

                '    Case 1
                '        pMessage = "You have requested a price that is below acceptable values. Please enter remarks/reasons why you want to give discount." & vbCrLf & pMessage

                '    Case 2
                '        pMessage = "Please click Approve to set all Unit Prices to their Requested Unit Prices' values." & vbCrLf & pMessage

                'End Select
                If vUserAdmin Then
                    mMessage = "Please click Approve to set all Unit Prices to their Requested Unit Prices' values." & vbCrLf & mMessage

                Else
                    mMessage = "You have requested a price that is below acceptable values. Please enter remarks/reasons why you want to give discount." & vbCrLf & mMessage
                End If

                Dim vMode As Integer = IIf(vUserAdmin, 2, 1)

                ''pRemarkRequestPrice = mRemarkRequestPrice
                ''If frmTransOrderHaggle.ShowRequestedPriceInfo(GetTotalLineGP() / GetTotalAmount() * 100, pTotalGPM, GetTotalLineGP, pTotalGP, pRemarkRequestPrice, vMode, pMessage, "Failed") Then
                ''    HandleRequestedPrices = True
                ''    Select Case vMode
                ''        Case 1
                ''            mRemarkRequestPrice = pRemarkRequestPrice
                ''        Case 2
                ''            mRemarkRequestPrice = pRemarkRequestPrice
                ''            ApproveRequestedPrices(True)
                ''            '    If gTransOrders.ApproveRequestedPrices(OrderID) Then
                ''            '        Call GetOrderInfo(True, True, False)
                ''            '    End If
                ''    End Select
                ''Else
                ''    HandleRequestedPrices = False
                ''End If
            Else
                '---If Higher Administrator and Users are authorize to change the prices
                HandleRequestedPrices = True
                If pOnlyHigherPrices Then mMessage = "Requested prices are higher than default prices. " & mMessage
                mMessage = "Requested Prices are acceptable." & vbCrLf & mMessage

                ''pRemarkRequestPrice = mRemarkRequestPrice
                ''If frmTransOrderHaggle.ShowRequestedPriceInfo(GetTotalLineGP() / GetTotalAmount() * 100, pTotalGPM, GetTotalLineGP, pTotalGP, pRemarkRequestPrice, 0, pMessage, "Passed") Then
                ''    ApproveRequestedPrices(True)
                ''End If

            End If
        Catch ex As Exception
            Throw
        End Try

    End Function

    Private Function GetTotalLineGP(Optional ByVal vIncludeRequested As Boolean = False) As Decimal

        Dim TotalGP As Decimal
        Dim aDimension As Double

        TotalGP = 0
        For Each pDtItem As DataRow In mDtItems.Rows
            If pDtItem.RowState <> DataRowState.Deleted Then
                If IsNumeric(pDtItem("Dimension")) Then
                    aDimension = CDec2(pDtItem("Dimension"))
                Else
                    aDimension = 1
                End If
                If vIncludeRequested Then
                    If IsNumeric(pDtItem("RequestedUnitPrice")) And CDec2(pDtItem("RequestedUnitPrice")) > 0 Then
                        TotalGP = TotalGP + (pDtItem("RequestedUnitPrice") - pDtItem("CurrentCost")) * pDtItem("Quantity") * aDimension
                        '(ReplaceNull(arDataArray(25, lngRowCntr)) - ReplaceNull(arDataArray(grdStock.Columns("Current Cost").ColIndex, lngRowCntr))) * ReplaceNull(arDataArray(3, lngRowCntr)) * gLibStock.DimensionValue(arDataArray(grdStock.Columns("Dimension").ColIndex, lngRowCntr) & "")
                    Else
                        TotalGP = TotalGP + (pDtItem("UnitPrice") - pDtItem("CurrentCost")) * pDtItem("Quantity") * aDimension
                        'TotalGP = TotalGP + (ReplaceNull(arDataArray(grdStock.Columns("Unit Price").ColIndex, lngRowCntr)) - _
                        '                     ReplaceNull(arDataArray(grdStock.Columns("Current Cost").ColIndex, lngRowCntr))) * ReplaceNull(arDataArray(3, lngRowCntr)) * gLibStock.DimensionValue(arDataArray(grdStock.Columns("Dimension").ColIndex, lngRowCntr) & "")
                    End If
                Else
                    TotalGP = TotalGP + (pDtItem("UnitPrice") - pDtItem("CurrentCost")) * pDtItem("Quantity") * aDimension
                    'TotalGP = TotalGP + (ReplaceNull(arDataArray(grdStock.Columns("Unit Price").ColIndex, lngRowCntr)) - ReplaceNull(arDataArray(grdStock.Columns("Current Cost").ColIndex, lngRowCntr))) * ReplaceNull(arDataArray(3, lngRowCntr)) * gLibStock.DimensionValue(arDataArray(grdStock.Columns("Dimension").ColIndex, lngRowCntr) & "")
                End If
                'MsgBox((ItemData("RequestedUnitPrice") - ItemData("CurrentCost")) * ItemData("Quantity") * aDimension)

            End If
        Next
        Return TotalGP

    End Function

    Private Function GetTotalAmount(Optional ByVal vIncludeRequested As Boolean = False) As Double
        Dim mTotalAmt As Double

        mTotalAmt = 0

        For Each pDrItem As DataRow In mDtItems.Rows
            If pDrItem.RowState <> DataRowState.Deleted Then
                If vIncludeRequested Then
                    If IsNumeric(pDrItem("RequestedUnitPrice")) And CDec2(pDrItem("RequestedUnitPrice")) > 0 Then
                        mTotalAmt = mTotalAmt + pDrItem("Quantity") * pDrItem("RequestedUnitPrice")
                    Else
                        mTotalAmt = mTotalAmt + CDbl(pDrItem("SubTotal"))
                    End If
                Else
                    mTotalAmt = mTotalAmt + CDbl(pDrItem("SubTotal"))
                End If
            End If

        Next
        Return mTotalAmt
    End Function

    'Private Function ApproveRequestedPrices(ByVal vApproved As Boolean)

    '    Dim aDimension As Double

    '    mGridBinder.Update(False)
    '    'mGridBinder.DataSource.AcceptChanges()

    '    For Each ItemData As DataRow In mGridBinder.DataSource.Rows
    '        If ItemData.RowState <> DataRowState.Deleted Then
    '            'MsgBox(CDec2(ItemData("RequestedUnitPrice")))
    '            If CDec2(ItemData("RequestedUnitPrice")) > 0 Then
    '                If IsNumeric(ItemData("Dimension")) > 0 Then
    '                    aDimension = CDec2(gBusiness.LibraryStock.DimensionValue(ItemData("Dimension")))
    '                Else
    '                    aDimension = 1
    '                End If

    '                If vApproved Then
    '                    If IsNumeric(ItemData("RequestedUnitPrice")) Then
    '                        ItemData("UnitPriceWOTax") = CDec2(ItemData("RequestedUnitPrice"))
    '                    End If
    '                Else
    '                    ItemData("UnitPriceWOTax") = CDec2(ItemData("OriginalUnitPrice"))
    '                End If
    '                ItemData("SubTotal") = ItemData("UnitPriceWOTax") * ItemData("Quantity") * aDimension
    '                'arDataArray(7, p) = ReplaceNull(arDataArray(13, p)) * ReplaceNull(arDataArray(3, p)) * ReplaceNull(gLibStock.DimensionValue(arDataArray(grdStock.Columns("Dimension").ColIndex, p) & "")) ' 3 = qty    

    '            End If
    '        End If
    '    Next

    '    If chkVat.CheckedValue Then
    '        ComputeTaxPrice()
    '    Else
    '        OriginalPrice()
    '    End If

    '    'mGridBinder.Update(False)
    '    'mGridBinder.DataSource.AcceptChanges()

    '    'SetGridProfitability()

    '    'VATOption(chkTaxable.Value, False)
    '    'grdStock_AfterUpdate()

    'End Function

    Private Sub GetData()

        Dim pSql As String

        pSql = "SELECT CustomerID" &
            " FROM TransSalesOrder" &
            " WHERE ID = " & ReadyString(mSalesOrderID)
        mDR = mProData.ExecuteRow(pSql)

        pSql = "SELECT StockID" &
            ",RequestedUnitPrice" &
            ",Quantity" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",TransSalesOrderItems.SubTotal" & vbCrLf &
            ",LibraryStockInformation.StockNumber" & vbCrLf &
            ",TransSalesOrderItems.Dimension" & vbCrLf &
            ",TransSalesOrderItems.PriceLevelID" & vbCrLf &
            " FROM TransSalesOrderItems" &
            " LEFT JOIN LibraryStockInformation ON LibraryStockInformation.ID = TransSalesOrderItems.StockID" & vbCrLf &
            " WHERE SalesOrderID = " & ReadyString(mSalesOrderID)
        mDtItems = mProData.Execute(pSql)
        mDtItems = mBusiness.TransInvoiceLegacy.AddProfitabilityColumns(mDtItems)
        'mDtItems.Columns.Add("CurrentCost", GetType(Decimal))
        'For Each pDR As DataRow In mDtItems.Rows
        '    pDR("CurrentCost") = GetPrice(CStr2(pDR("StockID")), pDR("ActualQuantity"), mLandedCostID)
        'Next pDR

    End Sub

    Private Function HasRequestedPrice() As Boolean

        Return CDec2(mDtItems.Compute("SUM(RequestedUnitPrice)", "RequestedUnitPrice > 0")) > 0

    End Function

    'Private Sub cmdTestRequestedPrice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdTestRequestedPrice.Click

    '    Dim pMode As Integer

    '    If HasRequestedPrice() Then

    '        flrSalesOrderApprovePrice = gBusiness.MainSecurity.UserAccess.SalesOrder_ApprovePrice.IsEnable
    '        Call SetGridProfitability()

    '        If HandleRequestedPrices(flrSalesOrderApprovePrice) Then

    '        End If

    '        '---Set Security Here
    '        'If Not flrSalesOrderApprovePrice Then
    '        '    pMode = 1
    '        'ElseIf flrSalesOrderApprovePrice Then
    '        '    pMode = 2
    '        '    'Else
    '        '    '    pMode = 0
    '        'End If
    '        'If HandleRequestedPrices(pMode) Then
    '        '    'MessageBox Me.Caption, "Requested Prices", "Requested Prices are acceptable.", enmOK, enmInformation
    '        'End If
    '    Else
    '        ShowMessage("No unit price change was requested.", "Request Prices", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '    End If

    'End Sub

    '----Clear Requested Prices
    'Private Sub ClearRequestedPrices()
    '    mGridBinder.Update(False)

    '    For Each ItemData As DataRow In mGridBinder.DataSource.Rows
    '        If ItemData.RowState <> DataRowState.Deleted Then
    '            If IsNumeric(ItemData("RequestedUnitPrice")) Then
    '                If (ItemData("UnitPrice") - CDec2(ItemData("OutPutTax"))) <> CDec2(ItemData("RequestedUnitPrice")) Then
    '                    ItemData("RequestedUnitPrice") = 0
    '                End If
    '            End If

    '        End If
    '    Next

    'End Sub

    Private Function GetPrice(ByVal vID As String, _
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
            pGuid = mBusiness.SystemDefaults.GetOrAddSystemDefault("Retail Price ID", "{01000012-0005-0000-0000-000000000000}")
            If pGuid = vPriceLevelID Then
                Exit Function
            Else
                GetPrice = GetPrice(vID, vUnitQty, pGuid)
            End If
        End If

    End Function

End Class ' END WholesaleCheckPriceRequest
