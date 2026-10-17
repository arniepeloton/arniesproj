Public Class TransInventoryLedger

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

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
    End Enum

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function ByTransIdAndProductId(vTransID As String, vProductID As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM TransInventoryLedger" & vbCrLf &
            " WHERE TransactionID = " & ReadyString(vTransID) & vbCrLf &
            " AND ProductID = " & ReadyString(vProductID)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    ''' <summary>
    ''' returns the created TransInventoryLedger.ID
    ''' </summary>
    ''' <param name="vStockID"></param>
    ''' <param name="vFowstID"></param>
    ''' <param name="vTransactionID"></param>
    ''' <param name="vTransactionType"></param>
    ''' <param name="vUserID"></param>
    ''' <param name="vQty"></param>
    ''' <param name="vBalance"></param>
    ''' <param name="vBinID"></param>
    ''' <param name="vExpirationDate"></param>
    ''' <param name="vBadCount"></param>
    ''' <param name="vBadCountReason"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Add(ByVal vStockID As String,
        ByVal vFowstID As String,
        ByVal vTransactionID As String,
        ByVal vTransactionType As eTransType,
        ByVal vUserID As String,
        Optional ByVal vQty As Decimal? = Nothing,
        Optional ByVal vBalance As Decimal? = Nothing,
        Optional ByVal vBinID As String = Nothing,
        Optional ByVal vExpirationDate As Date? = Nothing,
        Optional ByVal vBadCount As Integer? = Nothing,
        Optional ByVal vBadCountReason As String = "",
        Optional vSubTransactionType As String = "",
        Optional vParentID As String = "",
        Optional vInventoryCountDate As Date? = Nothing,
        Optional vPrice As Decimal? = Nothing) As String

        Dim pTransClass As TransClass
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pQty As Decimal
        Dim pBalance As Decimal
        Dim pBalanceLog As String = ""

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pOut As String


        mParent.ExtraLog("vb8JDZ3P5JLK! vTransactionID:" & vTransactionID)

        If vQty Is Nothing _
            AndAlso vBalance Is Nothing Then

            mParent.ExtraLog("vbLBLQSSWZZM! vTransactionID:" & vTransactionID)
            Throw New ArgumentException("vQty and vBalance cannot be both nothing")
        End If

        Select Case vTransactionType
            Case eTransType.Invoice
                pTransClass = New TransInvoice
            Case eTransType.InventoryCount
                pTransClass = New TransInventoryCount
            Case eTransType.Receiving
                pTransClass = New TransReceiving
            Case eTransType.CancelReceiving
                pTransClass = New CancelReceiving
            Case eTransType.TransferOut
                pTransClass = New TransTransferOut
            Case eTransType.SalesReturn
                pTransClass = New TransMemoSalesReturn
            Case eTransType.ProductMaker
                pTransClass = New TransBomMaker
            Case eTransType.CarryOver
                pTransClass = New TransCarryOver
            Case eTransType.InventoryCountAdjustment
                pTransClass = New InventoryCountAdjustment
            Case eTransType.BeginningBalance
                pTransClass = New BeginningBalance
            Case eTransType.CancelInvoice
                pTransClass = New CancelInvoice()
            Case eTransType.CancelSalesReturn
                pTransClass = New CancelTransMemoSalesReturn
            Case Else
                mParent.ExtraLog("vb9XYX86CQHC! vTransactionID:" & vTransactionID)
                Throw New ArgumentException("vTransactionType = " & vTransactionType)

        End Select
        pTransClass.Parent = Me

        If CStr2(vBinID).Length = 0 Then
            vBinID = mParent.SystemDefaults.GetOrAddDefaultBin(vFowstID, vStockID)
        End If

        If vQty Is Nothing Then
            pQty = vBalance - GetBalance(vStockID, vFowstID, vBinID, pTransClass.TransactionType, vExpirationDate)
            pBalanceLog &= "vbUYEYDELLJ8! vBalance = " & vBalance & vbCrLf &
                "pQty = " & pQty & vbCrLf &
                "vStockID = " & vStockID & vbCrLf &
                "vFowstID = " & vFowstID & vbCrLf &
                "vBinID = " & vBinID & vbCrLf &
                "pTransClass.TransactionType = " & pTransClass.TransactionType & vbCrLf &
                "vExpirationDate = " & CStr2(vExpirationDate) & vbCrLf
        Else
            pQty = vQty
            pBalanceLog &= "vbHAQP5SM2XU! vQty = " & vQty & vbCrLf
        End If
        pBalanceLog &= "vbEXJKBLMHJH! pQty = " & pQty & vbCrLf
        If vBalance Is Nothing Then
            pBalance = GetBalance(vStockID, vFowstID, vBinID, pTransClass.TransactionType, vExpirationDate)
            pBalanceLog &= "vbSELQKMTPVM! pBalance = " & pBalance & vbCrLf &
                "vStockID = " & vStockID & vbCrLf &
                "vFowstID = " & vFowstID & vbCrLf &
                "vBinID = " & vBinID & vbCrLf &
                "pTransClass.TransactionType = " & pTransClass.TransactionType & vbCrLf &
                "vExpirationDate = " & CStr2(vExpirationDate) & vbCrLf
            pBalance += pQty
            pBalanceLog &= "vbT2Z5J3SRM4! pBalance = " & pBalance & vbCrLf &
                "pQty = " & pQty & vbCrLf
        Else
            pBalance = vBalance
            pBalanceLog &= "vbRTMAQNYPZH! pBalance = " & pBalance & vbCrLf
        End If

        If pQty = 0 And Not (vTransactionType = eTransType.InventoryCount Or vTransactionType = eTransType.BeginningBalance) Then Exit Function ' nothing to save

        If vTransactionType = eTransType.InventoryCount Then
            'if 0 then its possible that its a new bin since GetBalance() will result to 0
            If (vBalance - pQty) = 0 Then

            End If
        End If

        Dim pTransDateSql = pTransClass.GetTransactionDateSql(vTransactionID)
        Dim pTransDate As String

        If pTransDateSql IsNot Nothing Then
            Dim pDrTransDate = mProData.ExecuteRow(pTransDateSql)
            If pDrTransDate IsNot Nothing Then
                If IsDate(pDrTransDate("TransDate")) Then
                    pTransDate = ProSqlite.ReadyDate(pDrTransDate("TransDate"))
                End If
            End If
        End If


        Dim pDTPreviousTransactions As DataTable
        Dim PreviousBalance As Double
        mParent.ExtraLog("vbC2DMZAK7HB! vTransactionID:" & vTransactionID)

        If Not String.IsNullOrWhiteSpace(pTransDate) Then
            pDTPreviousTransactions = GetPreviousInventoryLedgerTransactionCount(pTransDate, vFowstID, vBinID, vStockID)

            If pDTPreviousTransactions.Rows.Count > 0 Then

                PreviousBalance = GetInventoryLedgerPreviousBalance(pTransDate, vFowstID, vBinID, vStockID)

                If vTransactionType = eTransType.InventoryCount Then

                    pQty = vBalance - PreviousBalance

                    PreviousBalance = pBalance
                Else
                    PreviousBalance += pQty

                    pBalance = PreviousBalance
                    pBalanceLog &= "vbFBRFLDDCKK! pBalance = " & pBalance & vbCrLf
                End If

            End If
        End If
        mParent.ExtraLog("vbUQBXM6RTCW! vTransactionID:" & vTransactionID)

        pSql = "SELECT ID" & vbCrLf &
            ",Qty" & vbCrLf &
            " FROM TransInventoryLedger" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vStockID) & vbCrLf &
            " And TransactionID = " & ReadyString(vTransactionID) & vbCrLf &
            " -- And BinID = " & ReadyString(vBinID) & vbCrLf &
            " And TransactionType = " & ReadyString(pTransClass.TransactionType) & vbCrLf &
            " AND ISNULL(SubTransactionType, '') = " & ReadyString(vSubTransactionType)
        Dim pDrExisting = mProData.ExecuteRow(pSql)
        If pDrExisting IsNot Nothing Then
            Dim pExistingQty = CDec2(pDrExisting("Qty"))
            If pExistingQty = pQty Then
                Return CStr2(pDrExisting("ID"))
            End If
            pQty = pQty - pExistingQty
            pBalance += pQty - pExistingQty
        End If

        pIU = New InsertUpdate("TransInventoryLedger")
        pOut = Guid.NewGuid().ToString()
        pIU("ID") = ReadyString(pOut)
        pIU("ProductID") = ReadyString(vStockID)
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("TransactionID") = ReadyString(vTransactionID)
        pIU("TransactionType") = ReadyString(pTransClass.TransactionType)
        pIU("TransactionDate") = "GETDATE()" ' default trans date

        If pTransDateSql IsNot Nothing Then
            Dim pDrTransDate = mProData.ExecuteRow(pTransDateSql)
            If pDrTransDate IsNot Nothing Then
                If IsDate(pDrTransDate("TransDate")) Then
                    pIU("TransactionDate") = ProSqlite.ReadyDate(pDrTransDate("TransDate"))
                End If
            End If
        End If

        pIU("BinID") = ReadyString(vBinID)
        If vExpirationDate IsNot Nothing Then pIU("ExpirationDate") = ReadyString(vExpirationDate)
        pIU("Qty") = pQty
        pIU("Balance") = pBalance
        pIU("UserID") = ReadyString(vUserID)
        pIU("Value") = Math.Abs(pQty) * mParent.LibraryStock.GetPrice(vStockID, 1, mParent.SystemDefaults.GetLandedCostID)
        If vBadCount IsNot Nothing Then pIU("BadCount") = vBadCount
        pIU("Reason") = ReadyString(vBadCountReason)
        If CStr2(vSubTransactionType) <> "" Then
            pIU("SubtransactionType") = ReadyString(vSubTransactionType)
        End If

        If CStr2(pTransClass.TransactionType).ToLower() = "invoice" Then
            Select Case CStr2(vSubTransactionType).ToLower()
                Case "Production".ToLower()
                    ' dont remove this case. nothing to do here

                Case "Usage Via BOM".ToLower()
                    pIU("BomQty") = pQty * -1

                Case "Usage Via Item Modifer".ToLower()
                    pIU("ItemModifierQty") = pQty * -1

                    'Case "Sale".ToLower()
                Case Else
                    pIU("SalesQty") = pQty * -1

            End Select
        End If

        If CStr2(vParentID) <> "" Then
            pIU("ParentID") = ReadyString(vParentID)
        End If

        If IsDate(vInventoryCountDate) Then
            pIU("LastUpdated") = ReadyString(vInventoryCountDate)
        End If

        If vPrice IsNot Nothing Then
            pIU("Price") = vPrice
        End If

        pSql = pIU.GetInsert()
        Try
            mProData.ExecuteSQL(pSql)
            AddToTempLogs("vbS2SZDJ5KTT! ProductID = " & vStockID & vbCrLf &
                "TransactionID = " & vTransactionID)
        Catch ex As Exception
            AppendToLog("vbB3GWY42JRS! " & ex.Message & vbCrLf & pBalanceLog & "INSERT: " & pSql & vbCrLf & "UPDATE: " & pIU.GetUpdate("INSERT"))
            Throw
        End Try
        mParent.ExtraLog("vbK3ZL9NHBSA! vTransactionID:" & vTransactionID)

        If pDTPreviousTransactions IsNot Nothing Then
            For i As Integer = 0 To pDTPreviousTransactions.Rows.Count - 1
                If pDTPreviousTransactions.Rows(i).Item("TransactionType") = "InventoryCount" Then

                    Dim InventoryCountMovement As Double

                    InventoryCountMovement = CDec2(pDTPreviousTransactions.Rows(i).Item("Balance")) - PreviousBalance
                    PreviousBalance = pDTPreviousTransactions.Rows(i).Item("Balance")

                    pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & ", Qty = " & InventoryCountMovement & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                    mProData.ExecuteSQL(pSql)
                Else
                    PreviousBalance += CDbl(pDTPreviousTransactions.Rows(i).Item("Qty"))

                    pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                    mProData.ExecuteSQL(pSql)
                End If
            Next
        End If

        mParent.ExtraLog("vbK3ZL9NHBSBB! vTransactionID:" & vTransactionID)

        '''Caching Balances
        ''' 02/14/17
        Dim dtFowstBinProduct As DataTable
        Dim dtFowstProduct As DataTable

        Dim IsNewFowstBinProduct As Boolean 'important for Fowst Product computation

        ''' LibraryFowstBinProduct Table
        dtFowstBinProduct = GetDataFromLibraryFowstBinProduct(vStockID, vFowstID, vBinID, pTransClass.TransactionType)

        pIU = New InsertUpdate("LibraryFowstBinProduct")

        pIU("Balance") = pBalance
        pIU("LastUpdated") = "GETDATE()"

        If vTransactionType = eTransType.InventoryCount Then

            Dim mSql As String = "Select Surname + ', ' + Firstname from MobizLibraryPeople where ID =  " & ReadyString(vUserID)
            Dim mName As String = CStr2(mProData.ExecuteScalar(mSql))

            pIU("LastInventoryCountDate") = "GETDATE()" ' default trans date

            If IsDate(vInventoryCountDate) Then
                pIU("LastInventoryCountDate") = ReadyString(vInventoryCountDate)
            End If

            pIU("LastInventoryCountBy") = ReadyString(mName)
        End If

        If dtFowstBinProduct.Rows.Count > 0 Then

            pSql = pIU.GetUpdate(" ID = " & ReadyString(CStr2(dtFowstBinProduct.Rows(0).Item("ID"))))

        Else

            IsNewFowstBinProduct = True

            Dim TempBinID As String

            'check if there's existing default bin for this fowst-product
            pSql = "Select BinID from LibraryFowstBinProduct WHERE ProductID = " & ReadyString(vStockID) & " AND FOWSTID = " & ReadyString(vFowstID) & " AND IsDefault=1"

            TempBinID = CStr2(mProData.ExecuteScalar(pSql))
            mParent.ExtraLog("vbK3ZL9NHBSCC! vTransactionID:" & vTransactionID)

            pOut = Guid.NewGuid().ToString()

            pIU("ID") = ReadyString(pOut)
            pIU("FowstID") = ReadyString(vFowstID)
            pIU("BinID") = ReadyString(vBinID)
            pIU("ProductID") = ReadyString(vStockID)

            If TempBinID = vbNullString Then
                pIU("IsDefault") = 1

            End If
            pSql = pIU.GetInsert()

        End If

        mProData.ExecuteSQL(pSql)
        mParent.ExtraLog("vbK3ZL9NHBSB! vTransactionID:" & vTransactionID)

        ''' LibraryFowstProduct Table

        dtFowstProduct = GetDataFromLibraryFowstProduct(vStockID, vFowstID)

        pIU = New InsertUpdate("LibraryFowstProduct")

        If dtFowstProduct.Rows.Count > 0 Then

            If IsNewFowstBinProduct Then
                pIU("Balance") = pBalance + CDec2(dtFowstProduct.Rows(0).Item("Balance"))
                pBalanceLog &= "vbG8CQJXJWDE! pBalance = " & pIU("Balance") & vbCrLf
            Else

                pBalance = CDec2(dtFowstProduct.Rows(0).Item("Balance"))

                pIU("Balance") = pBalance + pQty
                pBalanceLog &= "vbLMEMYVHFVW! pBalance = " & pIU("Balance") & vbCrLf
            End If

            pSql = pIU.GetUpdate(" ID = " & ReadyString(CStr2(dtFowstProduct.Rows(0).Item("ID"))))

        Else

            IsNewFowstBinProduct = True

            Dim pID = Guid.NewGuid().ToString()

            pIU("ID") = ReadyString(pID)
            pIU("FowstID") = ReadyString(vFowstID)
            pIU("ProductID") = ReadyString(vStockID)
            pIU("Balance") = pBalance
            pBalanceLog &= "vbC2BYUGS94C! pBalance = " & pIU("Balance") & vbCrLf

            pSql = pIU.GetInsert()

        End If

        mProData.ExecuteSQL(pSql)
        mParent.ExtraLog("vbK3ZL9NHBSC! vTransactionID:" & vTransactionID)

        'assign more information to fowstbinproduct table(for caching inventory)
        UpdateLibraryFowstBinProductDetails(vFowstID)

        ''' End of Caching

        '''Organization PIM 
        '''Added By Francis A. 
        ''' Check if Product not yet added to private LibraryOrganizationPIM, if yes, then insert new record
        ''' 08/14/2013
        pFinder = New Finder
        With pFinder
            .ProData = mProData
            With .Fields
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgpimprd.ProductID"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryOrganizationPIM orgpimprd "

            .SqlWhere = " WHERE orgpimprd.ProductID = " + ReadyString(vStockID)

            pDT = .Requery
        End With
        mParent.ExtraLog("vb2HUBQ79GWR! vTransactionID:" & vTransactionID)

        If (pDT.Rows.Count <= 0) Then
            mParent.ExtraLog("vb8248UZ4GFB! vTransactionID:" & vTransactionID)
            pIU = New InsertUpdate("LibraryOrganizationPIM")
            pIU("ProductID") = ReadyString(vStockID)
            pIU("ActivationDate") = "getdate()"
            pIU("IsDeactivated") = "0"
            pIU("IsDoNotShareTransDataToBrandOwner") = "0"
            pIU("LastUpdated") = "GetDate()"

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            mProData.ExecuteSQL(pSql)
        Else
            If Not vTransactionType = eTransType.InventoryCount And Not pQty = 0 Then

                pIU = New InsertUpdate("LibraryOrganizationPIM")

                pIU("LastTransactionDate") = "GetDate()"
                pIU("LastTransactionReference") = ReadyString(pTransClass.TransactionType)

                pSql = pIU.GetUpdate("ProductID = " & ReadyString(vStockID))
                mProData.ExecuteSQL(pSql)

                'mag set pa ta for reactivation dire na part?

                pSql = "Delete from LibraryPIMForDeactivationAndReactivation where ProductID = " & ReadyString(vStockID)
                mProData.ExecuteSQL(pSql)
            Else
                'refer to last transaction if inventory count ang transaction and 0 ang qty or bad count.
                'dili na magrefer, since maupdate man gyapon ang lastupdated kung musulod ang transaction

            End If

        End If
        '''END Organization PIM 
        mParent.ExtraLog("vb4ZEWB63H77! vTransactionID:" & vTransactionID)

        'If String.IsNullOrWhiteSpace(pTransDate) And pTransDate IsNot Nothing Then

        Try
            If String.IsNullOrWhiteSpace(pTransDate) = False Then
                pTransDate = ParseDate(Trim2(pTransDate, "'"))
            End If
        Catch ex As Exception
            Throw
        End Try

        If pTransDate Is Nothing Then
            pTransDate = Now.Date
        End If

        mParent.CacheInventoryMovement.CacheDataByTransaction(vStockID, vFowstID, pTransDate, vTransactionType, vTransactionID, pQty, mParent.LibraryStock.GetPrice(vStockID, 1, mParent.SystemDefaults.GetLandedCostID))

        'If vTransactionType = eTransType.InventoryCount Then

        '    Dim mSql As String = "Select Surname + ', ' + Firstname from MobizLibraryPeople where ID =  " & ReadyString(vUserID)
        '    Dim mName As String = CStr2(mProData.ExecuteScalar(mSql))

        '    mParent.CacheInventoryMovement.CacheDataByTransaction(vStockID, vFowstID, pTransDate, vTransactionType, vTransactionID, pQty, mParent.LibraryStock.GetPrice(vStockID, 1, mParent.SystemDefaults.GetLandedCostID), vInventoryCountDate, mName)

        'Else
        '    mParent.CacheInventoryMovement.CacheDataByTransaction(vStockID, vFowstID, pTransDate, vTransactionType, vTransactionID, pQty, mParent.LibraryStock.GetPrice(vStockID, 1, mParent.SystemDefaults.GetLandedCostID))
        'End If


        Return pOut

    End Function

    Public Function Add(ByVal vStockID As String,
        ByVal vUnitMeasure As String,
        ByVal vFowstID As String,
        ByVal vTransactionID As String,
        ByVal vTransactionType As eTransType,
        ByVal vUserID As String,
        Optional ByVal vQty As Decimal? = Nothing,
        Optional ByVal vBalance As Decimal? = Nothing,
        Optional ByVal vBinID As String = Nothing,
        Optional ByVal vExpirationDate As Date? = Nothing)

        Dim pTransClass As TransClass
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pQty As Decimal
        Dim pBalance As Decimal

        Dim pFinder As Finder
        Dim pDT As DataTable

        If vQty Is Nothing _
            AndAlso vBalance Is Nothing Then

            Throw New ArgumentException("vQty and vBalance cannot be both nothing")
        End If

        Select Case vTransactionType
            Case eTransType.Invoice
                pTransClass = New TransInvoice
            Case eTransType.InventoryCount
                pTransClass = New TransInventoryCount
            Case eTransType.Receiving
                pTransClass = New TransReceiving
            Case eTransType.TransferOut
                pTransClass = New TransTransferOut
            Case eTransType.SalesReturn
                pTransClass = New TransMemoSalesReturn
            Case Else
                Throw New ArgumentException("vTransactionType = " & vTransactionType)

        End Select
        pTransClass.Parent = Me

        '''Added By Francis A. 
        ''' Check if Product has BOM
        ''' 05/07/2014
        CheckProductBOM("", vStockID, "", vStockID, "", vFowstID, vTransactionID, vTransactionType, vUserID, vQty)

    End Function

    Public Function CheckProductBOM(ByVal vRecBOMID As String,
                           ByVal vParentProductID As String,
                           ByVal vComponentParentProductID As String,
                           ByVal vProductID As String,
                           ByVal vComponentMethod As String,
                           ByVal vFowstID As String,
                           ByVal vTransactionID As String,
                           ByVal vTransactionType As eTransType,
                           ByVal vUserID As String,
                           Optional ByVal vQty As Decimal? = Nothing,
                           Optional ByVal vBalance As Decimal? = Nothing,
                           Optional ByVal vBinID As String = Nothing,
                           Optional ByVal vExpirationDate As Date? = Nothing)

        Dim pTransClass As TransClass
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pQty As Decimal
        Dim pBalance As Decimal

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDTBOM As DataTable

        Dim intCntr As Integer
        Dim dblQty As Double
        Dim pDTRow As DataRow

        If vQty Is Nothing _
            AndAlso vBalance Is Nothing Then

            Throw New ArgumentException("vQty and vBalance cannot be both nothing")
        End If

        Select Case vTransactionType
            Case eTransType.Invoice
                pTransClass = New TransInvoice
            Case eTransType.InventoryCount
                pTransClass = New TransInventoryCount
            Case eTransType.Receiving
                pTransClass = New TransReceiving
            Case eTransType.TransferOut
                pTransClass = New TransTransferOut
            Case eTransType.SalesReturn
                pTransClass = New TransMemoSalesReturn
            Case Else
                Throw New ArgumentException("vTransactionType = " & vTransactionType)

        End Select

        pTransClass.Parent = Me

        ''' Check if Product has BOM
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("RecBOMID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdbom.ID"
                End With
                With .Add("BOMProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdbom.BOMProductID"
                End With
                With .Add("Qty", SqlDbType.Decimal)
                    .Expression = "prdbom.Qty"
                End With
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdbom.UnitMeasureID"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "mealbl.Label"
                End With
                With .Add("ComponentMethod", SqlDbType.VarChar)
                    .Expression = "prdbom.ComponentMethod"
                End With
                With .Add("ParentBOMProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdbom.ParentBOMProductID"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProductBOM prdbom " &
                " INNER JOIN MobizLibraryPIMMeasure mea ON (mea.ID = prdbom.UnitMeasureID) " &
                " INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) "

            .SqlWhere = " WHERE prdbom.OrganizationID = (SELECT TOP 1 OrganizationID FROM MobizLibraryOrganizationFOWST WHERE ID = '" & vFowstID & "') " &
                " AND prdbom.ProductID = " & ReadyString(vParentProductID) &
                " AND prdbom.ParentBOMProductID " & IIf(vRecBOMID.Trim().Length > 0, "= " & ReadyString(vRecBOMID), " IS NULL")

            pDTBOM = .Requery
        End With

        If (pDTBOM.Rows.Count > 0) Then

            ''' Check if Every Sub component has other sub component
            intCntr = 0
            While (intCntr < pDTBOM.Rows.Count)

                pDTRow = pDTBOM.Rows(intCntr)

                dblQty = GetBOMQty(pDTRow("BOMProductID").ToString(), vQty, pDTRow("Qty").ToString(), pDTRow("UnitMeasure").ToString())

                If (pDTRow("ComponentMethod") = "Made-to-Order") Then

                    CheckProductBOM(pDTRow("RecBOMID").ToString(),
                               vParentProductID,
                               vProductID,
                               pDTRow("BOMProductID").ToString(),
                               pDTRow("ComponentMethod").ToString(),
                               vFowstID,
                               vTransactionID,
                               vTransactionType,
                               vUserID,
                               dblQty)
                Else
                    AddToInventoryLedger(vParentProductID,
                               vComponentParentProductID,
                               pDTRow("BOMProductID").ToString(),
                               vFowstID,
                               vTransactionID,
                               vTransactionType,
                               vUserID,
                               dblQty)
                End If

                intCntr = intCntr + 1

            End While

        Else
            AddToInventoryLedger(vParentProductID,
                       vComponentParentProductID,
                       vProductID,
                       vFowstID,
                       vTransactionID,
                       vTransactionType,
                       vUserID,
                       vQty)
        End If

    End Function

    Public Function GetBOMQty(
                              ByVal vProductID As String,
                              ByVal vPQty As Double,
                              ByVal vBOMQty As String,
                              ByVal vBOMUnitMeasure As String) As Double
        Dim pFinder As Finder
        Dim dblQty As Double
        Dim pDTUnitMeasure As DataTable
        Dim pDTUnitMeasureFamily As DataTable
        Dim pDTUnitMeasureConversion As DataTable
        Dim pDTContentRow As DataRow

        Dim intMea As Int32
        Dim intMeaFmly As Int32
        Dim blnIsMeasureFamily As Boolean
        Dim strMeasureFamily As String
        Dim pDTRowMeasure As DataRow
        Dim pDTRowMeasureFamily As DataRow
        Dim strUnitMeasureFamilyMatch As String
        Dim pDTRowMeasureConversion As DataRow

        dblQty = 0

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "mealbl.Label"
                End With
                With .Add("Quantity", SqlDbType.Decimal)
                    .Expression = "prdmea.Quantity"
                End With
                With .Add("ContentQty", SqlDbType.Decimal)
                    .Expression = "prdmea.ContentQty"
                End With
                With .Add("ContentUnit", SqlDbType.VarChar)
                    .Expression = "prdmea.ContentUnit"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProductMeasurement prdmea " &
                        " INNER JOIN MobizLibraryPIMMeasure mea ON (prdmea.UnitMeasureID = mea.ID) " &
                        " INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) "

            .SqlWhere = " WHERE prdmea.ProductID = " & ReadyString(vProductID)

            .SqlOrder = " ORDER BY prdmea.MeasurementLevel ASC "

            pDTUnitMeasure = .Requery

        End With

        If (pDTUnitMeasure.Rows.Count > 0) Then

            '''check if BOM Measure belongs to current/available Measurement of Product
            intMea = 0
            While (intMea < pDTUnitMeasure.Rows.Count)

                pDTRowMeasure = pDTUnitMeasure.Rows(intMea)
                If (vBOMUnitMeasure = pDTRowMeasure("UnitMeasure").ToString()) Then
                    dblQty = vPQty * (vBOMQty * CDec2(pDTRowMeasure("Quantity")))
                    Return dblQty
                End If

                intMea = intMea + 1
            End While

            pDTContentRow = pDTUnitMeasure.Rows(0)
            If (vBOMUnitMeasure = pDTContentRow("ContentUnit").ToString()) Then
                dblQty = vPQty * (vBOMQty / CDec2(pDTContentRow("ContentQty")))
            Else
                '''Check if BOM Meaurement is within the same Family of Content
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData
                    With .Fields
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "mea.ID"
                        End With
                        With .Add("Measurement", SqlDbType.VarChar)
                            .Expression = "mealbl.Label"
                        End With
                    End With

                    .SqlFrom = " FROM MobizLibraryPIMMeasure mea " &
                        " INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) "

                    .SqlWhere = " WHERE (CASE " &
                                "            (SELECT " &
                                "                (CASE  " &
                                "                    WHEN IsStandardLength = 1 then 'LENGTH' " &
                                "                    WHEN IsStandardTime = 1 then 'TIME' " &
                                "                    WHEN IsStandardArea = 1 then 'AREA' " &
                                "                    WHEN IsStandardVolCapacity = 1 then 'VOLUME CAPACITY' " &
                                "                    WHEN IsStandardVolDry = 1 then 'VOLUME DRY' " &
                                "                    WHEN IsStandardWeight = 1 then 'WEIGHT' " &
                                "                    WHEN IsStandardCooking = 1 then 'COOKING' " &
                                "                    WHEN IsStandardElectricity = 1 then 'ELECTRICITY' " &
                                "                    ELSE NULL " &
                                "                 END) as MeasureFamily " &
                                "            FROM MobizLibraryPIMMeasure mea " &
                                "            INNER JOIN MobizLibraryCountriesLanguageLabel mealbl ON (mea.MeasureNameID = mealbl.ID) " &
                                "            WHERE (SELECT TOP 1 ContentUnit " &
                                "                   FROM MobizLibraryPIMProductMeasurement " &
                                "                   WHERE ProductID = " & ReadyString(vProductID) & ") LIKE '%' + mealbl.Label + '%'  " &
                                "            ) " &
                                "       WHEN 'LENGTH' THEN IsStandardLength " &
                                "       WHEN 'TIME' THEN IsStandardTime  " &
                                "       WHEN 'AREA' THEN IsStandardArea  " &
                                "       WHEN 'VOLUME CAPACITY' THEN IsStandardVolCapacity  " &
                                "       WHEN 'VOLUME DRY' THEN IsStandardVolDry  " &
                                "       WHEN 'WEIGHT' THEN IsStandardWeight  " &
                                "       WHEN 'COOKING' THEN IsStandardCooking  " &
                                "       WHEN 'ELECTRICITY' THEN IsStandardElectricity  " &
                                "       END) = 1 "

                    pDTUnitMeasureFamily = .Requery

                End With

                blnIsMeasureFamily = False
                strMeasureFamily = ""
                If (pDTUnitMeasureFamily.Rows.Count > 0) Then

                    intMeaFmly = 0
                    While (intMeaFmly < pDTUnitMeasureFamily.Rows.Count)

                        pDTRowMeasureFamily = pDTUnitMeasureFamily.Rows(intMeaFmly)
                        If (vBOMUnitMeasure = pDTRowMeasureFamily("Measurement").ToString()) Then
                            blnIsMeasureFamily = True
                            Exit While
                        End If

                        intMeaFmly = intMeaFmly + 1
                    End While

                End If '''If (pDTUnitMeasureFamily.Rows.Count > 0) Then

                If (blnIsMeasureFamily) Then

                    '''Check if BOM Meaurement is within the same Family of Content
                    pFinder = New Finder
                    With pFinder
                        .ProData = Common.WebRequest.ProData
                        .MaxRecords = 1
                        With .Fields
                            With .Add("ID", SqlDbType.UniqueIdentifier)
                                .Expression = "meaconv.ID"
                            End With
                            With .Add("BaseMeasure", SqlDbType.VarChar)
                                .Expression = "meaconv.BaseMeasure"
                            End With
                            With .Add("DestMeasure", SqlDbType.VarChar)
                                .Expression = "meaconv.DestMeasure"
                            End With
                            With .Add("MeasurementFamily", SqlDbType.VarChar)
                                .Expression = "meaconv.MeasurementFamily"
                            End With
                            With .Add("ConversionFactor", SqlDbType.Float)
                                .Expression = "meaconv.ConversionFactor"
                            End With
                        End With

                        .SqlFrom = " FROM MobizLibraryPIMMeasureConversion meaconv "

                        .SqlWhere = " WHERE BaseMeasure = " & ReadyString(vBOMUnitMeasure) &
                                    " AND DestMeasure = " & ReadyString(pDTContentRow("ContentUnit").ToString())

                        pDTUnitMeasureConversion = .Requery

                    End With

                    If (pDTUnitMeasureConversion.Rows.Count > 0) Then
                        pDTRowMeasureConversion = pDTUnitMeasureConversion.Rows(0)
                        dblQty = vPQty * (vBOMQty * CDec2(pDTRowMeasureConversion("ConversionFactor")))
                    Else
                        '''No convertion record
                        dblQty = 0
                    End If

                Else
                    '''Cross Measurement Family Conversion
                End If

            End If '''If (vBOMUnitMeasure = pDTRow("ContentUnit")) Then

        Else

        End If '''If (pDTUnitMeasure.Rows.Count > 0) Then

        Return dblQty

    End Function

    Public Function AddToInventoryLedger(ByVal vParentProductID As String,
                           ByVal vComponentParentProductID As String,
                           ByVal vProductID As String,
                           ByVal vFowstID As String,
                           ByVal vTransactionID As String,
                           ByVal vTransactionType As eTransType,
                           ByVal vUserID As String,
                           Optional ByVal vQty As Decimal? = Nothing,
                           Optional ByVal vBalance As Decimal? = Nothing,
                           Optional ByVal vBinID As String = Nothing,
                           Optional ByVal vExpirationDate As Date? = Nothing)

        Dim pTransClass As TransClass
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pQty As Decimal
        Dim pBalance As Decimal

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDTBOM As DataTable

        If vQty Is Nothing _
            AndAlso vBalance Is Nothing Then

            Throw New ArgumentException("vQty and vBalance cannot be both nothing")
        End If

        Select Case vTransactionType
            Case eTransType.Invoice
                pTransClass = New TransInvoice
            Case eTransType.InventoryCount
                pTransClass = New TransInventoryCount
            Case eTransType.Receiving
                pTransClass = New TransReceiving
            Case eTransType.TransferOut
                pTransClass = New TransTransferOut
            Case eTransType.SalesReturn
                pTransClass = New TransMemoSalesReturn
            Case Else
                Throw New ArgumentException("vTransactionType = " & vTransactionType)

        End Select
        pTransClass.Parent = Me

        If CStr2(vBinID).Length = 0 Then
            vBinID = mParent.SystemDefaults.GetOrAddDefaultBin(vFowstID, vProductID)
        End If

        If vQty Is Nothing Then
            pQty = vBalance - GetBalance(vProductID, vFowstID, vBinID, pTransClass.TransactionType, vExpirationDate)
        Else
            pQty = vQty
        End If
        If vBalance Is Nothing Then
            pBalance = GetBalance(vProductID, vFowstID, vBinID, pTransClass.TransactionType, vExpirationDate) + pQty
        Else
            pBalance = vBalance
        End If
        If pQty = 0 And Not vTransactionType = eTransType.InventoryCount Then Exit Function ' nothing to save

        Dim pTransDateSql = pTransClass.GetTransactionDateSql(vTransactionID)
        Dim pTransDate As String

        If pTransDateSql IsNot Nothing Then
            Dim pDrTransDate = mProData.ExecuteRow(pTransDateSql)
            If pDrTransDate IsNot Nothing Then
                If IsDate(pDrTransDate("TransDate")) Then
                    pTransDate = ProSqlite.ReadyDate(pDrTransDate("TransDate"))
                End If
            End If
        End If


        Dim pDTPreviousTransactions As DataTable
        Dim PreviousBalance As Double

        If String.IsNullOrWhiteSpace(pTransDate) = False Then
            pDTPreviousTransactions = GetPreviousInventoryLedgerTransactionCount(pTransDate, vFowstID, vBinID, vProductID)

            If pDTPreviousTransactions.Rows.Count > 0 Then

                PreviousBalance = GetInventoryLedgerPreviousBalance(pTransDate, vFowstID, vBinID, vProductID)

                PreviousBalance += pQty

                pBalance = PreviousBalance
            End If
        End If

        pIU = New InsertUpdate("TransInventoryLedger")

        If (vParentProductID.Trim().Length > 0) Then
            pIU("ParentProductID") = ReadyString(vParentProductID)
        End If

        If (vComponentParentProductID.Trim().Length > 0) Then
            pIU("ComponentParentProductID") = ReadyString(vComponentParentProductID)
        End If
        pIU("TransactionDate") = "GETDATE()" ' default trans date
        'pTransDateSql = pTransClass.GetTransactionDateSql(vTransactionID)
        If pTransDateSql IsNot Nothing Then
            Dim pDrTransDate = mProData.ExecuteRow(pTransDateSql)
            If pDrTransDate IsNot Nothing Then
                If IsDate(pDrTransDate("TransDate")) Then
                    pIU("TransactionDate") = ProSqlite.ReadyDate(pDrTransDate("TransDate"))
                End If
            End If
        End If
        pIU("ProductID") = ReadyString(vProductID)
        pIU("FowstID") = ReadyString(vFowstID)
        pIU("TransactionID") = ReadyString(vTransactionID)
        pIU("TransactionType") = ReadyString(pTransClass.TransactionType)
        pIU("BinID") = ReadyString(vBinID)
        If vExpirationDate IsNot Nothing Then pIU("ExpirationDate") = ReadyString(vExpirationDate)
        pIU("Qty") = pQty
        pIU("Balance") = pBalance
        pIU("UserID") = ReadyString(vUserID)
        pIU("Value") = Math.Abs(pQty) * mParent.LibraryStock.GetPrice(vProductID, 1, mParent.SystemDefaults.GetLandedCostID)

        pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        If pDTPreviousTransactions IsNot Nothing Then
            For i As Integer = 0 To pDTPreviousTransactions.Rows.Count - 1

                If pDTPreviousTransactions.Rows(i).Item("TransactionType") = "InventoryCount" Then

                    Dim InventoryCountMovement As Double

                    InventoryCountMovement = CDec2(pDTPreviousTransactions.Rows(i).Item("Balance")) - PreviousBalance
                    PreviousBalance = pDTPreviousTransactions.Rows(i).Item("Balance")

                    pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & ", Qty = " & InventoryCountMovement & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                    mProData.ExecuteSQL(pSql)
                Else
                    PreviousBalance += CDbl(pDTPreviousTransactions.Rows(i).Item("Qty"))

                    pSql = "Update TransInventoryLedger set Balance = " & PreviousBalance & " WHERE ID = " & ReadyString(CStr2(pDTPreviousTransactions.Rows(i).Item("ID")))
                    mProData.ExecuteSQL(pSql)
                End If
            Next
        End If

        '''Organization PIM 
        '''Added By Francis A. 
        ''' Check if Product not yet added to private LibraryOrganizationPIM, if yes, then insert new record
        ''' 08/14/2013
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgpimprd.ProductID"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryOrganizationPIM orgpimprd "

            .SqlWhere = " WHERE orgpimprd.ProductID = " + ReadyString(vProductID)

            pDT = .Requery
        End With

        If (pDT.Rows.Count <= 0) Then
            pIU = New InsertUpdate("LibraryOrganizationPIM")
            pIU("ProductID") = ReadyString(vProductID)
            pIU("ActivationDate") = "getdate()"
            pIU("IsDeactivated") = "0"
            pIU("IsDoNotShareTransDataToBrandOwner") = "0"
            pIU("LastUpdated") = "GetDate()"

            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)

            'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)
        End If
        '''END Organization PIM 
        ''' 
        If String.IsNullOrWhiteSpace(pTransDate) Then
            pTransDate = New Date(Trim2(pTransDate, "'")).Date
        Else
            pTransDate = Now.Date
        End If

        'mParent.CacheInventoryMovement.CacheData(vProductID, vFowstID, pTransDate)
        mParent.CacheInventoryMovement.CacheDataByTransaction(vProductID, vFowstID, pTransDate, vTransactionType, vTransactionID, pQty)

    End Function

    Private Function GetInventoryLedgerPreviousBalance(vTransDate, vFowstID, vBinID, vProductID) As Double
        Dim pSql As String

        pSql = "Select top 1 balance from TransInventoryLedger " & vbCrLf &
                "WHERE TransactionDate < " & vTransDate & vbCrLf &
                "AND BinID = " & ReadyString(vBinID) & vbCrLf &
                "AND FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
                "AND ProductID = " & ReadyString(vProductID) & vbCrLf &
                " ORDER BY TransactionDate DESC"

        Return CDbl(mProData.ExecuteScalar(pSql))

    End Function

    Private Function GetPreviousInventoryLedgerTransactionCount(vTransDate, vFowstID, vBinID, vProductID) As DataTable
        Dim pSql As String

        pSql = "Select * from TransInventoryLedger " & vbCrLf &
                "WHERE TransactionDate > " & vTransDate & vbCrLf &
                " AND BinID = " & ReadyString(vBinID) & vbCrLf &
                " AND FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
                " AND ProductID = " & ReadyString(vProductID) & vbCrLf &
                " ORDER BY TransactionDate"

        Return mProData.Execute(pSql)
    End Function

    Private Function GetBalance(ByVal vStockID As String, ByVal vFowstID As String, ByVal vBinID As String, ByVal vTransactionType As String,
        Optional ByVal vExpirationDate As Date? = Nothing) As Decimal

        Dim pSql As String

        pSql = "SELECT TOP 1 Balance" & vbCrLf &
            " FROM TransInventoryLedger" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vStockID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND BinID = " & ReadyString(vBinID) & vbCrLf
        If vExpirationDate IsNot Nothing Then
            pSql &= " AND ExpirationDate = " & ReadyString(vExpirationDate) & vbCrLf
        Else
            pSql &= " AND ExpirationDate IS NULL" & vbCrLf
        End If
        'pSql &= " ORDER BY LastUpdated DESC"
        pSql &= " ORDER BY TransactionDate DESC"

        Try
            Return CDec2(mProData.ExecuteScalar(pSql))
        Catch ex As Exception
            AppendToDebug("vbTL4U3SF9BG! SQL: " & pSql, True, ex, True)
            Throw
        End Try

    End Function

    Public Function GetEntries(vTransactionID As String, vTransactionType As eTransType) As DataTable

        Dim pTransClass As TransClass
        Dim pSql As String
        Dim pDT As DataTable

        Select Case vTransactionType
            Case eTransType.Invoice
                pTransClass = New TransInvoice
            Case eTransType.InventoryCount
                pTransClass = New TransInventoryCount
            Case eTransType.Receiving
                pTransClass = New TransReceiving
            Case eTransType.TransferOut
                pTransClass = New TransTransferOut
            Case eTransType.SalesReturn
                pTransClass = New TransMemoSalesReturn
            Case eTransType.ProductMaker
                pTransClass = New TransBomMaker
            Case Else
                Throw New ArgumentException("vTransactionType = " & vTransactionType)

        End Select

        pSql = " SELECT ProductID" & vbCrLf &
            " ,FowstID" & vbCrLf &
            " ,BinID" & vbCrLf &
            " ,Qty" & vbCrLf &
            " FROM TransInventoryLedger" &
            " WHERE TransactionType = " & ReadyString(pTransClass.TransactionType) &
            " AND TransactionID = " & ReadyString(vTransactionID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetDataFromLibraryFowstBinProduct(ByVal vStockID As String, ByVal vFowstID As String, ByVal vBinID As String, ByVal vTransactionType As String) As DataTable

        Dim pSql As String

        pSql = "SELECT TOP 1 *" & vbCrLf &
            " FROM LibraryFowstBinProduct" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vStockID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND BinID = " & ReadyString(vBinID) & vbCrLf

        Return mProData.Execute(pSql)
    End Function

    Private Function GetDataFromLibraryFowstProduct(ByVal vStockID As String, ByVal vFowstID As String)

        Dim pSql As String

        pSql = "SELECT TOP 1 *" & vbCrLf &
            " FROM LibraryFowstProduct" & vbCrLf &
            " WHERE ProductID = " & ReadyString(vStockID) & vbCrLf &
            " AND FowstID = " & ReadyString(vFowstID) & vbCrLf

        Return mProData.Execute(pSql)
    End Function

    Private Class TransInvoice
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransInvoice"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT InvoiceDate AS TransDate FROM TransInvoice WHERE ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Invoice"

            End Get
        End Property
    End Class

    Private Class TransInventoryCount
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransInventoryCount"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT InventoryCountDate AS TransDate FROM TransInventoryProductCount WHERE ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "InventoryCount"

            End Get
        End Property
    End Class

    Private Class TransReceiving
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransReceiving"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT LastUpdated AS TransDate FROM TransReceiving WHERE ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Receiving"

            End Get
        End Property
    End Class

    Private Class CancelReceiving
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransReceiving"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT DateCancelled AS TransDate FROM TransReceiving WHERE ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Cancel Receiving"

            End Get
        End Property
    End Class

    Private Class InventoryCountAdjustment
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "InventoryCountAdjustment"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return Nothing

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Clearing ending balance of unnamed bin for carrying over to new default bin"

            End Get
        End Property
    End Class

    Private Class TransCarryOver
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransCarryOver"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return Nothing

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Carryover of ending balance of unnamed bin to new default bin"

            End Get
        End Property
    End Class

    Private Class TransTransferOut
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransReceiving"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT LastUpdated AS TransDate FROM TransReceiving WHERE ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "TransferOut"

            End Get
        End Property
    End Class

    Private Class TransMemoSalesReturn
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransMemoSalesReturn"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT TransMemo.MemoDate AS TransDate" & vbCrLf &
                " FROM TransMemo" & vbCrLf &
                " WHERE TransMemo.ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Sales Returns"

            End Get
        End Property
    End Class

    Private Class CancelTransMemoSalesReturn
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransMemoSalesReturn"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT TransInvoiceAdjustments.DateDeactivated AS TransDate" & vbCrLf &
                " FROM TransInvoiceAdjustments" & vbCrLf &
                " JOIN TransMemo ON TransMemo.ID = TransInvoiceAdjustments.MemoID" & vbCrLf &
                " WHERE TransMemo.ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Cancel Sales Returns"

            End Get
        End Property
    End Class

    Private Class TransBomMaker
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransBomMaker"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT LastUpdated AS TransDate FROM TransBomMaker WHERE ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Product Maker"

            End Get
        End Property
    End Class

    ''' <summary>
    ''' this must be wrapped in transaction and error handler
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CreateTempTables(Optional vIncludeNoInventory As Boolean = False)

        Dim pSql As String
        Dim FOWSTID As String
        Dim ProductID As String
        Dim sqlWhere As String
        Dim pDR As DataRow

        FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
        ProductID = CStr2(Common.WebRequest.Data("ProductID"))

        sqlWhere = "WHERE 1=1 " & vbCrLf

        If String.IsNullOrWhiteSpace(FOWSTID) = False Then
            pSql = "SELECT OrganizationID" & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                " WHERE ID = " & ReadyString(FOWSTID)
            pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                WebRequest.Instance.OrgID = CStr2(pDR("OrganizationID"))
                WebRequest.Instance.HandleOrganization(True)
            End If

            sqlWhere &= " AND (FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                "   OR FowstID IS NULL)" & vbCrLf &
                " AND IsDeactivated = " & ReadyString(CStr2(Common.WebRequest.Data("IsDeactivated")))
        End If

        If IsGuid(ProductID) Then
            sqlWhere += " AND ProductID = " & ReadyString(ProductID)
        End If

        pSql = "SELECT DISTINCT MobizLibraryPIMProduct.ID AS ProductID" & vbCrLf &
            " ,dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,TransInventoryLedger.FowstID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS Fowst" & vbCrLf &
            " ,TransInventoryLedger.BinID" & vbCrLf &
            " ,LibraryFowstBin.Name AS BinName" & vbCrLf &
            " ,tblBaseUnit.UnitMeasure AS UnitMeasure" & vbCrLf &
            " ,tblSRP.Price AS SRP" & vbCrLf &
            " ,tblCOGS.Price AS COGS" & vbCrLf &
            " ,tblInventoryCountApproval.InventoryCountApproval" & vbCrLf &
            " ,LibraryOrganizationPIM.IsDeactivated" & vbCrLf &
            " INTO #t" & vbCrLf &
            " FROM MobizLibraryPIMProduct" & vbCrLf &
            If(vIncludeNoInventory, " LEFT", "") & " JOIN TransInventoryLedger ON TransInventoryLedger.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID" & vbCrLf &
            " JOIN MobizLibraryPIMProductVariableFeatures001 on MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInventoryLedger.BinID" & vbCrLf &
            " LEFT JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
               " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                " ) tblBaseUnit" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(Price,0) AS Price " & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = 'EEF716C6-B290-4B28-B2A6-0A4F52CF4989' " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID =  MobizLibraryPIMProduct.ID " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.OrganizationID =  " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " ) tblSRP" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(Price,0) AS Price " & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0003-0000-0000-000000000000' " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID =  MobizLibraryPIMProduct.ID " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.OrganizationID =  " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " ) tblCOGS" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(COUNT(TransInventoryProductCountStock.Variance),0) AS InventoryCountApproval " & vbCrLf &
                "     FROM TransInventoryProductCount " & vbCrLf &
                "     INNER JOIN TransInventoryProductCountStock ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                "     WHERE TransInventoryProductCountStock.IsActionTaken = 0 " & vbCrLf &
                "     AND MobizLibraryPIMProduct.ID = TransInventoryProductCount.ProductID " & vbCrLf &
                "     AND TransInventoryProductCountStock.Variance <> 0 " & vbCrLf &
                " ) tblInventoryCountApproval" & vbCrLf

        '"     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf & _
        ' display products with subcomponents
        ' + display products which are not subcomponents
        If CBool2(Common.WebRequest.Data("WithComponentsOnly")) Then
            pSql &= " WHERE EXISTS(" & vbCrLf &
                " 	SELECT *" & vbCrLf &
                "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                "     WHERE MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " 	AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " )" & vbCrLf &
                " OR NOT EXISTS(" & vbCrLf &
                " 	SELECT *" & vbCrLf &
                "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                "     WHERE MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " )"
        End If

        If CBool2(Common.WebRequest.Data("SubcomponentsOnly")) Then
            pSql &= " WHERE EXISTS(" & vbCrLf &
                " 	SELECT *" & vbCrLf &
                "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                "     WHERE MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " )"
        End If


        '' exclude product with subcomponents
        '' exclude product which are subcomponents
        'If CBool2(Common.WebRequest.Data("ExcludeSubcomponentsExcludeWithSubcomponents")) Then
        '    pSql &= " WHERE NOT EXISTS(" & vbCrLf &
        '        " 	SELECT *" & vbCrLf &
        '        "     FROM MobizLibraryPIMProductContents" & vbCrLf &
        '        "     WHERE (MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
        '        "     OR MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
        '        "     ) AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
        '        " )"
        'End If

        If Not CStr2(Common.WebRequest.Data("Category")) = vbNullString Then
            pSql &= "WHERE MobizLibraryPIMProductVariableFeatures001.C0020 = " & ReadyString(Common.WebRequest.Data("Category"))
        End If

        mProData.ExecuteSQL(pSql)

        pSql = " SELECT #t.*" & vbCrLf &
            " ,t.*" & vbCrLf &
            " INTO #t2" & vbCrLf &
            " FROM #t" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT TOP 1 TransInventoryLedger_1.LastUpdated" & vbCrLf &
            "     ,TransInventoryLedger_1.Qty AS Quantity" & vbCrLf &
            " 	,TransInventoryLedger_1.Balance" & vbCrLf &
            "     FROM TransInventoryLedger TransInventoryLedger_1" & vbCrLf &
            "     WHERE #t.FowstID = TransInventoryLedger_1.FowstID" & vbCrLf &
            "     AND #t.ProductID = TransInventoryLedger_1.ProductID" & vbCrLf &
            "     AND ISNULL(CONVERT(char(36), #t.BinID), '') = ISNULL(CONVERT(char(36), TransInventoryLedger_1.BinID), '')" & vbCrLf &
            "     ORDER BY TransInventoryLedger_1.LastUpdated DESC" & vbCrLf &
            " ) t" & vbCrLf &
            sqlWhere & vbCrLf &
            " ORDER BY t.LastUpdated" & vbCrLf
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Function GetFowstLedger() As DataTable
        Dim pSql As String
        Dim FOWSTID As String
        Dim ProductID As String
        Dim sqlWhere As String
        Dim pDR As DataRow

        FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
        ProductID = CStr2(Common.WebRequest.Data("ProductID"))

        pSql = "SELECT OrganizationID" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE ID = " & ReadyString(FOWSTID)
        pDR = Common.WebRequest.ProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            WebRequest.Instance.OrgID = CStr2(pDR("OrganizationID"))
            WebRequest.Instance.HandleOrganization(True)
        End If

        sqlWhere = "WHERE FowstID = " & ReadyString(FOWSTID)

        If IsGuid(ProductID) Then

            sqlWhere += " AND ProductID = " & ReadyString(ProductID)

        End If

        pSql = "SELECT DISTINCT LibraryFowstProduct.ProductID" & vbCrLf &
            " ,dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,dbo.LibraryFowstProduct.Balance" & vbCrLf &
            " ,tblBaseUnit.UnitMeasure AS UnitMeasure" & vbCrLf &
            " ,tblSRP.Price AS SRP" & vbCrLf &
            " ,tblSRP.Price AS COGS" & vbCrLf &
            " ,LibraryFowstProduct.LastUpdated AS LastUpdated" & vbCrLf &
            " ,tblInventoryCountApproval.InventoryCountApproval" & vbCrLf &
            " FROM LibraryFowstProduct " & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON LibraryFowstProduct.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryFowstProduct.FowstID" & vbCrLf &
            " INNER JOIN MobizLibraryPIMProductVariableFeatures001 on MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryFowstProduct.ProductID" & vbCrLf &
               " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                " ) tblBaseUnit" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(Price,0) AS Price " & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = 'EEF716C6-B290-4B28-B2A6-0A4F52CF4989' " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID =  MobizLibraryPIMProduct.ID " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.OrganizationID =  " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " ) tblSRP" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(Price,0) AS Price " & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0003-0000-0000-000000000000' " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID =  MobizLibraryPIMProduct.ID " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.OrganizationID =  " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " ) tblCOGS" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(COUNT(TransInventoryProductCountStock.Variance),0) AS InventoryCountApproval " & vbCrLf &
                "     FROM TransInventoryProductCount " & vbCrLf &
                "     INNER JOIN TransInventoryProductCountStock ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                "     WHERE TransInventoryProductCountStock.IsActionTaken = 0 " & vbCrLf &
                "     AND MobizLibraryPIMProduct.ID = TransInventoryProductCount.ProductID " & vbCrLf &
                "     AND TransInventoryProductCountStock.Variance <> 0 " & vbCrLf &
                " ) tblInventoryCountApproval" & vbCrLf

        '"     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf & _
        ' display products with subcomponents
        ' + display products which are not subcomponents
        If CBool2(Common.WebRequest.Data("WithComponentsOnly")) Then
            pSql &= " WHERE EXISTS(" & vbCrLf &
                " 	SELECT *" & vbCrLf &
                "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                "     WHERE MobizLibraryPIMProductContents.ParentProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                " 	AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " )" & vbCrLf &
                " OR NOT EXISTS(" & vbCrLf &
                " 	SELECT *" & vbCrLf &
                "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                "     WHERE MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " )"
        End If

        If CBool2(Common.WebRequest.Data("SubcomponentsOnly")) Then
            pSql &= " WHERE EXISTS(" & vbCrLf &
                " 	SELECT *" & vbCrLf &
                "     FROM MobizLibraryPIMProductContents" & vbCrLf &
                "     WHERE MobizLibraryPIMProductContents.ChildProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                "     AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                " )"
        End If

        If Not CStr2(Common.WebRequest.Data("Category")) = vbNullString Then
            pSql &= "WHERE MobizLibraryPIMProductVariableFeatures001.C0020 = " & ReadyString(Common.WebRequest.Data("Category"))
        End If

        Return mProData.Execute(pSql)
    End Function

    Public Function CacheBalanceEachFowst(Optional vOrgID As String = "") As Integer

        Dim pDtOrgs As DataTable

        pDtOrgs = mParent.LibraryOrganization.GetAllHaveFowst(vOrgID)
        For Each pDR As DataRow In pDtOrgs.Rows
            CacheFowstBalance(CStr2(pDR("OrganizationID")), pDR("DatabaseName"))
        Next pDR

    End Function

    Public Function CacheFowstBalance(vOrgID As String, vDatabaseName As String) As Integer

        Dim pSql As String
        Dim pIU As InsertUpdate

        pSql = "DELETE FROM MobizCacheInventoryBalancePerFowst" & vbCrLf &
            " WHERE FowstID IN (" & vbCrLf &
            "   SELECT ID" & vbCrLf &
            "   FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            "   WHERE OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            ")"
        pSql = mProData.ExecuteSQL(pSql) ' return value used for debugging only

        pSql = " FROM " & vbCrLf &
            " (" & vbCrLf &
            "  	SELECT DISTINCT TransInventoryLedger.ProductID" & vbCrLf &
            "     ,TransInventoryLedger.FowstID" & vbCrLf &
            "     ,TransInventoryLedger.BinID" & vbCrLf &
            "     FROM " & EscapeSqlName(vDatabaseName) & ".dbo.TransInventoryLedger" & vbCrLf &
            "  ) TransInventoryLedger" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            " SELECT TOP 1 ISNULL(Price, 0) AS Price" & vbCrLf &
            "     FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = 'EEF716C6-B290-4B28-B2A6-0A4F52CF4989'" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.ProductID =  TransInventoryLedger.ProductID" & vbCrLf &
            "     AND MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString((vOrgID)) & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
            " ) tblSRP" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            " 	SELECT TOP 1 TransInventoryLedger_1.LastUpdated" & vbCrLf &
            "     ,TransInventoryLedger_1.Qty" & vbCrLf &
            "  	  ,TransInventoryLedger_1.Balance" & vbCrLf &
            "     ,TransInventoryLedger_1.Value" & vbCrLf &
            "     FROM " & EscapeSqlName(vDatabaseName) & ".dbo.TransInventoryLedger TransInventoryLedger_1" & vbCrLf &
            "     WHERE TransInventoryLedger.FowstID = TransInventoryLedger_1.FowstID" & vbCrLf &
            "     AND TransInventoryLedger.ProductID = TransInventoryLedger_1.ProductID" & vbCrLf &
            "     AND ISNULL(CONVERT(char(36), TransInventoryLedger.BinID), '') = ISNULL(CONVERT(char(36), TransInventoryLedger_1.BinID), '')" & vbCrLf &
            "     ORDER BY TransInventoryLedger_1.LastUpdated DESC" & vbCrLf &
            " ) tblLastEntry" & vbCrLf &
            " WHERE MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " GROUP BY TransInventoryLedger.FowstID" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name"
        pIU = New InsertUpdate("MobizCacheInventoryBalancePerFowst")
        pIU.FromSql = pSql
        pIU("FowstID") = "TransInventoryLedger.FowstID"
        pIU("SrpValue") = "SUM(tblLastEntry.Balance * tblSRP.Price)"
        pIU("LastUpdated") = "GETDATE()"
        pSql = pIU.GetInsert(True)
        mProData.ExecuteSQL(pSql)

    End Function

    Public Sub UpdateLibraryFowstBinProductDetails(FOWSTID As String)
        Dim pSql As String
        Dim pDT As DataTable
        Dim pDTProductInfo As DataTable
        Dim pDR As DataRow
        Dim OrgID As String

        pSql = "SELECT OrganizationID" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE ID = " & ReadyString(FOWSTID)
        pDR = mProData.ExecuteRow(pSql)

        If pDR IsNot Nothing Then
            OrgID = CStr2(pDR("OrganizationID"))
            'WebRequest.Instance.HandleOrganization(True)
        Else
            'Throw New ApplicationException("ID, " & FOWSTID & " was not found in MobizLibraryOrganizationFOWST. Error in TransInventoryLedger.UpdateLibraryFowstBinProductDetails. vbHR62SWD6CV!")
            OrgID = FOWSTID
        End If

        pSql = "Select DISTINCT top 20 ProductID from LibraryFOWSTBinProduct where ProductNo IS NULL"
        pDT = mProData.Execute(pSql)

        For i As Integer = 0 To pDT.Rows.Count - 1

            pSql = "SELECT DISTINCT LibraryOrganizationPIM.ProductID" & vbCrLf &
            " ,dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,tblBaseUnit.UnitMeasure AS UnitMeasure" & vbCrLf &
            " ,tblSRP.Price AS SRP" & vbCrLf &
            " ,tblCOGS.Price AS COGS" & vbCrLf &
            " ,tblInventoryCountApproval.InventoryCountApproval" & vbCrLf &
            " ,LibraryOrganizationPIM.IsDeactivated" & vbCrLf &
            " FROM MobizLibraryPIMProduct " & vbCrLf &
            " JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
               " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                " ) tblBaseUnit" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(Price,0) AS Price " & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = 'EEF716C6-B290-4B28-B2A6-0A4F52CF4989' " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID =  MobizLibraryPIMProduct.ID " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.OrganizationID =  " & ReadyString(OrgID) & vbCrLf &
                " ) tblSRP" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(Price,0) AS Price " & vbCrLf &
                "     FROM MobizLibraryPIMProductPriceLevel " & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.PriceLevelID = '01000012-0003-0000-0000-000000000000' " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID =  MobizLibraryPIMProduct.ID " & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.OrganizationID =  " & ReadyString(OrgID) & vbCrLf &
                " ) tblCOGS" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " SELECT TOP 1 ISNULL(COUNT(TransInventoryProductCountStock.Variance),0) AS InventoryCountApproval " & vbCrLf &
                "     FROM TransInventoryProductCount " & vbCrLf &
                "     INNER JOIN TransInventoryProductCountStock ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                "     WHERE TransInventoryProductCountStock.IsActionTaken = 0 " & vbCrLf &
                "     AND MobizLibraryPIMProduct.ID = TransInventoryProductCount.ProductID " & vbCrLf &
                "     AND TransInventoryProductCountStock.Variance <> 0 " & vbCrLf &
                " ) tblInventoryCountApproval" & vbCrLf &
                " WHERE LibraryOrganizationPIM.ProductID = " & ReadyString(CStr2(pDT.Rows(i).Item("ProductID")))
            Try
                pDTProductInfo = mProData.Execute(pSql)
            Catch ex As Exception
                AppendToLog("vbVV8K83SM9M! " & ex.Message & vbCrLf & pSql)
                Throw
            End Try

            If pDTProductInfo.Rows.Count > 0 Then
                pSql = "Update LibraryFOWSTBinProduct SET " & vbCrLf &
                        " ProductNo = " & ReadyString(pDTProductInfo.Rows(0).Item("ProductNo")) & vbCrLf &
                        ", ProductName = " & ReadyString(pDTProductInfo.Rows(0).Item("ProductName")) & vbCrLf &
                        ", UnitMeasure = " & ReadyString(pDTProductInfo.Rows(0).Item("UnitMeasure")) & vbCrLf &
                        ", COGS = " & CDec2(pDTProductInfo.Rows(0).Item("COGS")) & vbCrLf &
                        ", SRP = " & CDec2(pDTProductInfo.Rows(0).Item("SRP")) & vbCrLf &
                        ", InventoryCountApproval = " & CInt2(pDTProductInfo.Rows(0).Item("InventoryCountApproval")) & vbCrLf &
                        ", IsDeactivated = " & CInt2(pDTProductInfo.Rows(0).Item("IsDeactivated")) & vbCrLf &
                        " WHERE ProductID = " & ReadyString(pDTProductInfo.Rows(0).Item("ProductID")) & vbCrLf

                mProData.ExecuteSQL(pSql)
            End If
        Next

    End Sub


    Private Class BeginningBalance
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "Beginning Balance"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return Nothing

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Beginning Balance"

            End Get
        End Property
    End Class

    Private Class CancelInvoice
        Inherits TransClass

        Public Overrides ReadOnly Property TableName As String
            Get

                Return "TransInvoice"

            End Get
        End Property

        Public Overrides Function GetTransactionDateSql(vID As String) As String

            Return "SELECT DateCancelled AS TransDate FROM TransInvoice WHERE ID = " & ReadyString(vID)

        End Function

        Public Overrides ReadOnly Property TransactionType As String
            Get

                Return "Cancel Invoice"

            End Get
        End Property
    End Class

    Private MustInherit Class TransClass
        Public Parent As TransInventoryLedger
        ''' <summary>
        ''' saved to TransInventoryLedger.TransactionType
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property TransactionType As String
        Public MustOverride ReadOnly Property TableName As String
        Public TransactionID As String

        Public MustOverride Function GetTransactionDateSql(vID As String) As String
    End Class

    Public Sub SaveTransPimSalesCaching()

        Dim pSourceLedgerSw = New Stopwatch
        Dim pSourceLedgerCount As Integer
        Const pDateStart As Date = #10/4/2023#

        Dim pSql = " SELECT LibraryPimSalesCaching.Interval" & vbCrLf &
            " FROM LibraryPimSalesCaching" & vbCrLf &
            " WHERE Interval > 0" & vbCrLf &
            " GROUP BY LibraryPimSalesCaching.Interval" & vbCrLf &
            " ORDER BY LibraryPimSalesCaching.Interval"
        Dim pDtIntervals = mProData.Execute(pSql)
        AppendToLog("after get interval vbX7RC9NMTZW!",, "SaveTransPimSalesCaching.partial")

        Dim pDayOffset As Integer
        Do
            Dim pDate = pDateStart.AddDays(pDayOffset)
            If pDate > Now.Date Then
                Exit Do
            End If

            mProData.BeginTransaction()
            pSql = "DELETE FROM TransPimSalesCaching" & vbCrLf &
                " WHERE CONVERT(date, DateIntervalStart) = " & ReadyString(Format(pDate, cGlobal.FormatDateTimeSql))
            mProData.ExecuteSQL(pSql)
            AppendToLog(Format(pDate, cGlobal.FormatDate) & "after delete vbVK8Z5SJ8UR!",, "SaveTransPimSalesCaching.partial")

            For Each pDrInterval As DataRow In pDtIntervals.Rows
                Dim pTimeEnd = pDate
                Do
                    Dim pTimeStart = pTimeEnd
                    If (pTimeStart.Date > pDate) Then
                        Exit Do
                    End If
                    pTimeEnd = pTimeStart.AddMinutes(pDrInterval("Interval"))
                    If pTimeEnd.Date > pDate Then
                        pTimeEnd = pDate.AddDays(1)
                    End If
                    pSql = " SELECT TransInventoryLedger.ProductID" & vbCrLf &
                        " ,TransInventoryLedger.FowstID" & vbCrLf &
                        " ,TransInventoryLedger.SubtransactionType" & vbCrLf &
                        " ,SUM(TransInventoryLedger.Qty) AS Qty" & vbCrLf &
                        " ,SUM(TransInventoryLedger.BomQty) AS BomQty" & vbCrLf &
                        " ,SUM(TransInventoryLedger.ItemModifierQty) AS ItemModifierQty" & vbCrLf &
                        " ,SUM(TransInventoryLedger.SalesQty) AS SalesQty" & vbCrLf &
                        " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
                        " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
                        " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
                        " ,tblBaseUnit.UnitMeasure AS UnitMeasure" & vbCrLf &
                        " FROM TransInventoryLedger" & vbCrLf &
                        " JOIN LibraryPimSalesCaching ON LibraryPimSalesCaching.ProductID = TransInventoryLedger.ProductID" & vbCrLf &
                        " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInventoryLedger.ProductID" & vbCrLf &
                        " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID" & vbCrLf &
                        " OUTER APPLY (" & vbCrLf &
                        " 	SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
                        " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty" & vbCrLf &
                        "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
                        "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
                        "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
                        "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                        "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
                        " ) tblBaseUnit" & vbCrLf &
                        " WHERE LibraryPimSalesCaching.Interval = " & CInt2(pDrInterval("Interval")) & vbCrLf &
                        " AND TransInventoryLedger.TransactionDate >= " & ReadyString(Format(pTimeStart, cGlobal.FormatDateTimeSql)) & vbCrLf &
                        " AND TransInventoryLedger.TransactionDate < " & ReadyString(Format(pTimeEnd, cGlobal.FormatDateTimeSql)) & vbCrLf &
                        " -- AND TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf &
                        " GROUP BY TransInventoryLedger.ProductID" & vbCrLf &
                        " ,TransInventoryLedger.FowstID" & vbCrLf &
                        " ,TransInventoryLedger.SubtransactionType" & vbCrLf &
                        " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
                        " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
                        " ,MobizLibraryOrganizationFOWST.Name" & vbCrLf &
                        " ,tblBaseUnit.UnitMeasure" & vbCrLf &
                        " ORDER BY MobizLibraryPIMProduct.ProductName"
                    AppendToLog(Format(pDate, cGlobal.FormatDate) & "before query vbBFVEXHZM44!",, "SaveTransPimSalesCaching.partial")
                    pSourceLedgerSw.Start()
                    Dim pDtInventory = mProData.Execute(pSql)
                    pSourceLedgerCount += 1
                    pSourceLedgerSw.Stop()
                    AppendToLog(Format(pDate, cGlobal.FormatDate) & " after query vbJGUN3H4EFB!",, "SaveTransPimSalesCaching.partial")
                    AppendToLog(Format(pDate, cGlobal.FormatDate) & " after query pDtInventory.Rows.Count = " & pDtInventory.Rows.Count & " vbT33XRL2RTY!",, "SaveTransPimSalesCaching.partial")
                    For Each pDrInventory In pDtInventory.Rows
                        Dim pIU = New InsertUpdate("TransPimSalesCaching")
                        pIU("ID") = "NEWID()"
                        pIU("ProductID") = ReadyString(pDrInventory("ProductID"))
                        pIU("FowstID") = ReadyString(pDrInventory("FowstID"))
                        pIU("Qty") = ReadyString(pDrInventory("Qty"))
                        'Select Case CStr2(pDrInventory("SubtransactionType")).ToLower()
                        '    Case "Production".ToLower()
                        '        ' dont remove this case. nothing to do here
                        '    Case "Usage Via BOM".ToLower()
                        '        pIU("BomQty") = CDec2(pDrInventory("Qty")) * -1
                        '    Case "Usage Via Item Modifer".ToLower()
                        '        pIU("ItemModifierQty") = CDec2(pDrInventory("Qty")) * -1
                        '    Case Else
                        '        pIU("SalesQty") = CDec2(pDrInventory("Qty")) * -1
                        'End Select

                        pIU("BomQty") = CDec2(pDrInventory("BomQty"))
                        pIU("ItemModifierQty") = CDec2(pDrInventory("ItemModifierQty"))
                        pIU("SalesQty") = CDec2(pDrInventory("SalesQty"))

                        pIU("Interval") = pDrInterval("Interval")
                        pIU("DateIntervalStart") = ReadyString(Format(pTimeStart, cGlobal.FormatDateTimeSql))
                        pIU("DateIntervalEnd") = ReadyString(Format(pTimeEnd, cGlobal.FormatDateTimeSql))
                        pIU("LastUpdated") = "GETDATE()"
                        AppendToLog(Format(pDate, cGlobal.FormatDate) & " before insert vbL7Q4XNX4KN!",, "SaveTransPimSalesCaching.partial")
                        pSql = pIU.GetInsert()
                        mProData.Execute(pSql)
                        AppendToLog(Format(pDate, cGlobal.FormatDate) & " after insert vbP2DXJYWK8M!",, "SaveTransPimSalesCaching.partial")
                    Next
                    AppendToLog(Format(pDate, cGlobal.FormatDate) & " after loop pDtInventory.Rows.Count = " & pDtInventory.Rows.Count & " vbS747YC88R3!",, "SaveTransPimSalesCaching.partial")
                Loop
            Next
            mProData.CommitTransaction()

            Dim pLog = "Running log for " & Format(pDate, cGlobal.FormatDate) & vbCrLf &
                "Running Total Inventory Ledger Read Query Count: " & Format(pSourceLedgerCount, cGlobal.FormatInteger) & "." & vbCrLf &
                "Running Total Inventory Ledger Read Query Time: " & Format(pSourceLedgerSw.Elapsed.TotalMilliseconds, cGlobal.FormatInteger) & "." & vbCrLf
            If pSourceLedgerCount > 0 Then
                pLog &= "Running Average Time Per Inventory Ledger Read Query: " & Format(pSourceLedgerSw.Elapsed.TotalMilliseconds / pSourceLedgerCount) & " ms."
            End If
            AppendToLog(pLog,, "SaveTransPimSalesCaching")

            pDayOffset += 1
        Loop

        Dim pEndLog = "End Total Inventory Ledger Read Query Count: " & Format(pSourceLedgerCount, cGlobal.FormatInteger) & "." & vbCrLf &
            "End Total Inventory Ledger Read Query Time: " & Format(pSourceLedgerSw.Elapsed.TotalMilliseconds, cGlobal.FormatInteger) & "." & vbCrLf
        If pSourceLedgerCount > 0 Then
            pEndLog &= "Running Average Time Per Inventory Ledger Read Query: " & Format(pSourceLedgerSw.Elapsed.TotalMilliseconds / pSourceLedgerCount) & " ms."
        End If
        AppendToLog(pEndLog,, "SaveTransPimSalesCaching")

    End Sub

    Public Function TransPimSalesCachingToExcel() As String

        Dim pSql = " -- GetTransPimSalesCaching" & vbCrLf &
            " SELECT TransPimSalesCaching.ProductID" & vbCrLf &
            " ,TransPimSalesCaching.FowstID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,TransPimSalesCaching.Qty" & vbCrLf &
            " ,tblBaseUnit.UnitMeasure AS UnitMeasure" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,TransPimSalesCaching.DateIntervalStart" & vbCrLf &
            " ,TransPimSalesCaching.DateIntervalEnd" & vbCrLf &
            " ,TransPimSalesCaching.Interval" & vbCrLf &
            " FROM TransPimSalesCaching" & vbCrLf &
            " JOIN LibraryPimSalesCaching ON LibraryPimSalesCaching.ProductID = TransPimSalesCaching.ProductID" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransPimSalesCaching.ProductID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransPimSalesCaching.FowstID" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            " 	SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            " 	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty" & vbCrLf &
            "     FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            "     JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            "     JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "     WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
            "     ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            " ) tblBaseUnit" & vbCrLf &
            " --WHERE LibraryPimSalesCaching.Interval = 77" & vbCrLf &
            " ORDER BY TransPimSalesCaching.DateIntervalStart" & vbCrLf &
            " ,TransPimSalesCaching.DateIntervalEnd" & vbCrLf &
            " ,TransPimSalesCaching.Interval" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf
        Dim pDT = mProData.Execute(pSql)
        Dim pBaseFilename = DateTimeStamp()
        Dim pFilename = Common.ServerPath & "temp\TransPimSalesCachingToExcel\TransPimSalesCachingToExcel_" & pBaseFilename & ".xlsx"
        CreateFolder(pFilename)
        TableToExcelFile(pDT, "", "", pFilename)

        Return pFilename

    End Function

    Public Function GetTransPimSalesCaching(vFowstID As String, vTransDate As Date) As DataTable

        vTransDate = vTransDate.Date
        Dim pSql = " -- GetTransPimSalesCaching" & vbCrLf &
            " SELECT TransPimSalesCaching.ID" & vbCrLf &
            " ,TransPimSalesCaching.ProductID" & vbCrLf &
            " ,TransPimSalesCaching.FowstID" & vbCrLf &
            " ,TransPimSalesCaching.Qty" & vbCrLf &
            " ,TransPimSalesCaching.SalesQty" & vbCrLf &
            " ,TransPimSalesCaching.ItemModifierQty" & vbCrLf &
            " ,TransPimSalesCaching.BomQty" & vbCrLf &
            " ,TransPimSalesCaching.DateIntervalStart" & vbCrLf &
            " ,TransPimSalesCaching.DateIntervalEnd" & vbCrLf &
            " ,TransPimSalesCaching.Interval" & vbCrLf &
            " FROM TransPimSalesCaching" & vbCrLf &
            " WHERE TransPimSalesCaching.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND CONVERT(date, TransPimSalesCaching.DateIntervalStart) = " & ReadyString(Format(vTransDate, cGlobal.FormatDateTimeSql)) & vbCrLf &
            " ORDER BY TransPimSalesCaching.DateIntervalStart" & vbCrLf &
            " ,TransPimSalesCaching.DateIntervalEnd" & vbCrLf &
            " ,TransPimSalesCaching.Interval" & vbCrLf
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Shared Function TableDateToString(vDT As DataTable) As DataTable

        For Each pDR As DataRow In vDT.Rows

        Next

    End Function

    Public Sub CreateDummyRecords()

        Const pCount = 120000

        If Now.Date <> #9/19/2023# Then
            Throw New ApplicationException("not today. ref: vbKJJV29AR85!")
        End If

        Dim pSql = "SELECT * FROM TransInventoryLedger" & vbCrLf &
            " WHERE ID = " & ReadyString("384EBB47-CDF6-4E32-BE61-62EC297111E3")
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Throw New ApplicationException("specified id not found. ref: vbTEHUMEQTAA!")
        End If

        Dim pIU = Business.DataRowToInsertUpdate(pDR)
        pIU.TableName = "TransInventoryLedger"
        For p = 1 To pCount
            pIU("ID") = "NEWID()"
            Select Case RandomInteger(1, 2)
                Case 1
                    pIU("ProductID") = ReadyString("0112cb10-5af2-4994-8003-b824cf2bbe77")
                Case 2
                    pIU("ProductID") = ReadyString("8d6a9f7e-3cb4-42d9-9ad1-6d2e9684f5d9")
            End Select

            Select Case RandomInteger(1, 2)
                Case 1
                    pIU("FowstID") = ReadyString("006a0ead-3fc3-4324-9480-7cc52e388954")
                Case 2
                    pIU("FowstID") = ReadyString("27c373c5-5026-4b3d-b16a-b0eda224c5ef")
            End Select

            pIU("Qty") = -RandomInteger(1, 1000)
            pIU("TransactionType") = ReadyString("Invoice")

            Select Case RandomInteger(1, 4)
                Case 1
                    pIU("SubtransactionType") = ReadyString("Production")

                Case 2
                    pIU("SubtransactionType") = ReadyString("Usage Via BOM")
                    pIU("BomQty") = pIU("Qty") * -1

                Case 3
                    pIU("SubtransactionType") = ReadyString("Usage Via Item Modifer")
                    pIU("ItemModifierQty") = pIU("Qty") * -1

                Case 4
                    pIU("SubtransactionType") = ReadyString("Sale")
                    pIU("SalesQty") = pIU("Qty") * -1

                Case Else
                    Throw New NotImplementedException("you should not be here")

            End Select
            pIU("TransactionDate") = ReadyString(Format(RandomDate(New Date(2023, 9, 16), New Date(2023, 9, 17).AddSeconds(-1), True), cGlobal.FormatDateTimeSql))
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

    End Sub
End Class