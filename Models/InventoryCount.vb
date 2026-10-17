Public Class InventoryCount

    <HttpPost()>
    Function GetAllInventoryCount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .SearchString = WebRequest.Instance.Data("data")

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.TransInventoryProductCount.ID"
                End With

                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.TransInventoryProductCount.FowstID"
                End With

                With .Add("Fowst", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.TransInventoryProductCount.ProductID"
                End With

                With .Add("InventoryCountDate", SqlDbType.Date)
                    .Expression = "dbo.TransInventoryProductCount.InventoryCountDate"
                End With

                With .Add("UserID", SqlDbType.Int)
                    .Expression = "dbo.TransInventoryProductCount.UserID"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                End With
            End With

            .SqlFrom = "FROM dbo.LibraryOrganizationPIM INNER JOIN " &
                      "dbo.MobizLibraryPIMProduct ON dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " &
                      "dbo.TransInventoryProductCount INNER JOIN " &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInventoryProductCount.FowstID = dbo.MobizLibraryOrganizationFOWST.ID ON " &
                      "dbo.LibraryOrganizationPIM.ProductID = dbo.TransInventoryProductCount.ProductID"

            .SqlOrder = "ORDER BY TransInventoryProductCount.LastUpdated desc"

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetInventoryCountStocks() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .SearchString = WebRequest.Instance.Data("data")

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.TransInventoryStockCountStock.ID"
                End With

                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryCustomersBin.Name"
                End With

                With .Add("IsGoodStock", SqlDbType.Bit)
                    .Expression = "dbo.TransInventoryStockCountStock.IsGoodStock"
                End With

                With .Add("ExpirationDate", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.TransInventoryStockCountStock.ExpirationDate"
                End With

                With .Add("UnitMeasure", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.LibraryUnitMeasures.UnitMeasure"
                End With

                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "dbo.TransInventoryStockCountStock.Quantity"
                End With
            End With

            .SqlFrom = "FROM dbo.TransInventoryStockCountStock INNER JOIN " &
                      "dbo.LibraryCustomersBin ON dbo.TransInventoryStockCountStock.BinID = dbo.LibraryCustomersBin.ID INNER JOIN " &
                      "dbo.LibraryUnitMeasures ON dbo.TransInventoryStockCountStock.UnitMeasureID = dbo.LibraryUnitMeasures.ID"

            .SqlWhere = "WHERE dbo.TransInventoryStockCountStock.InventoryStockCountID = " & ReadyString(WebRequest.Instance.Data("InventoryCountID"))


            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetAllBinStock() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryFowstBinProduct.ID"
                End With

                With .Add("ProductID", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "dbo.LibraryFowstBinProduct.ProductID"
                End With

                With .Add("StockNumber", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                End With

                With .Add("MeasurementID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProductMeasurement.ID"
                End With

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With

            End With

            .SqlFrom = "FROM dbo.LibraryFowstBinProduct INNER JOIN " &
                        "dbo.MobizLibraryPIMProduct ON dbo.LibraryFowstBinProduct.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryPIMProductMeasurement ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductMeasurement.ProductID  " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID  " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID "

            .SqlWhere = "Where BinID = " & ReadyString(WebRequest.Instance.Data("BinID")) & " AND " &
                        " FOWSTID=" & ReadyString(WebRequest.Instance.Data("FOWSTID")) & " AND LibraryFowstBinProduct.IsActive=1 and MobizLibraryPIMProductMeasurement.MeasurementLevel=1"

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetAllBrands() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("Brand", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.Brand"
                End With

            End With

            .SqlFrom = "FROM dbo.LibraryOrganizationPIM INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID"

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetAllProducts() As DataTable
        Select Case Common.WebRequest.Data("GroupType")
            Case "ByBin"
                Return GetAllProductsByBin()
            Case "ByBrand"
                Return GetAllProductsByBrand()
            Case "ByNegativeBalances"
                Return GetAllNegativeBalances()
            Case "ByRecentlyReceived"
                Return GetAllProductsByRecentlyReceived()
        End Select
    End Function

    Function GetAllProductsByBin() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductNo", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("AllowDecimal", SqlDbType.Bit)
                    .Searchable = True
                    .Expression = "ISNULL((SELECT AllowDecimal" & vbCrLf &
                        " FROM LibraryOrganizationPIM" & vbCrLf &
                        " WHERE LibraryOrganizationPIM.ProductID = TransInventoryLedger_1.ProductID" & vbCrLf &
                        "), 0)"
                End With
            End With

            .SqlFrom = "FROM dbo.TransInventoryLedger AS TransInventoryLedger_1 INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON TransInventoryLedger_1.ProductID = dbo.MobizLibraryPIMProduct.ID"

            .SqlWhere = "WHERE TransInventoryLedger_1.BinID = " & ReadyString(WebRequest.Instance.Data("Filter"))

            .SqlOrder = "Order by dbo.MobizLibraryPIMProduct.ProductNo"
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetAllProductsByRecentlyReceived() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductNo", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("AllowDecimal", SqlDbType.Bit)
                    .Searchable = True
                    .Expression = "ISNULL((SELECT AllowDecimal" & vbCrLf &
                        " FROM LibraryOrganizationPIM" & vbCrLf &
                        " WHERE LibraryOrganizationPIM.ProductID = TransInventoryLedger_1.ProductID" & vbCrLf &
                        "), 0)"
                End With
            End With

            .SqlFrom = "FROM dbo.TransInventoryLedger AS TransInventoryLedger_1 INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON TransInventoryLedger_1.ProductID = dbo.MobizLibraryPIMProduct.ID"

            .SqlWhere = "WHERE ((TransInventoryLedger_1.TransactionType = 'Receiving') OR (TransInventoryLedger_1.TransactionType = 'TransferOut')) " & vbCrLf &
                        "AND (CONVERT(date, TransInventoryLedger_1.LastUpdated) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & ")"

            .SqlOrder = "Order by dbo.MobizLibraryPIMProduct.ProductNo"
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetAllProductsByBrand() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductNo", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.Date)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("AllowDecimal", SqlDbType.Bit)
                    .Searchable = True
                    .Expression = "ISNULL((SELECT AllowDecimal" & vbCrLf &
                        " FROM LibraryOrganizationPIM" & vbCrLf &
                        " WHERE LibraryOrganizationPIM.ProductID = TransInventoryLedger_1.ProductID" & vbCrLf &
                        "), 0)"
                End With
            End With

            .SqlFrom = "FROM dbo.TransInventoryLedger AS TransInventoryLedger_1 INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON TransInventoryLedger_1.ProductID = dbo.MobizLibraryPIMProduct.ID"

            .SqlWhere = "WHERE dbo.MobizLibraryPIMProduct.Brand = " & ReadyString(WebRequest.Instance.Data("Filter"))

            .SqlOrder = "Order by dbo.MobizLibraryPIMProduct.ProductNo"
            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetAllNegativeBalances() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "t.ID"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "t.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "t.ProductName"
                End With

                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "t.ProductGroupNo"
                End With

                'With .Add("Inv_Balance_Qty", SqlDbType.Float)
                '    .Expression = "(SELECT TOP (100) PERCENT SUM(Qty) AS Expr1 " &
                '                "FROM dbo.TransInventoryLedger  " &
                '                "WHERE ProductID = dbo.MobizLibraryPIMProduct.ID " &
                '                "GROUP BY ProductID) "
                'End With
                With .Add("AllowDecimal", SqlDbType.Bit)
                    .Searchable = True
                End With
            End With

            .SqlFrom = "FROM (select distinct MobizLibraryPIMProduct.ID, dbo.MobizLibraryPIMProduct.ProductNo, " &
                "dbo.MobizLibraryPIMProduct.ProductName, dbo.MobizLibraryPIMProduct.ProductGroupNo, bal1.Balance " &
                ",ISNULL((SELECT top 1 AllowDecimal" & vbCrLf &
                    " FROM LibraryOrganizationPIM" & vbCrLf &
                    " WHERE LibraryOrganizationPIM.ProductID = t0.ProductID" & vbCrLf &
                    "), 0) AS AllowDecimal" & vbCrLf &
                "from TransInventoryLedger t0 inner join MobizLibraryPIMProduct on MobizLibraryPIMProduct.ID = t0.ProductID  " &
                "outer apply( " &
                "select top 1 Balance from TransInventoryLedger as t1 " &
                "where t1.ProductID = t0.ProductID And t1.FowstID = t0.FowstID And t1.BinID = t0.BinID " &
                "order by LastUpdated desc " &
                ")  bal1 " &
                ") t "

            .SqlGroup = "group by ID, ProductNo, ProductName, ProductGroupNo, AllowDecimal"
            .SqlHaving = "having SUM(t.balance)<0"
            .SqlOrder = "Order by ProductGroupNo"
            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function SaveInventoryCount(ByVal ID As String, ByVal IsNew As Boolean) As String
        Dim p As New InsertUpdate("TransInventoryProductCount")
        Dim pSql As String
        Dim Quantity As Double
        Dim QuantityBad As Double
        Dim BinID As String
        Dim ExpirationDate As String
        Dim Total As Double
        Dim DefaultBinID As String

        Dim FOWSTID As String
        Dim ProductID As String

        FOWSTID = WebRequest.Instance.Data("FowstID")
        ProductID = WebRequest.Instance.Data("ProductID")
        Try
            Common.WebRequest.ProData.BeginTransaction()

            p("FowstID") = ReadyString(FOWSTID)
            p("ProductID") = ReadyString(ProductID)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

            If IsNew Then
                ID = Guid.NewGuid.ToString
                p("ID") = ReadyString(ID)

                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate("ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If Not IsNew Then
                pSql = "Delete from TransInventoryProductCountStock where InventoryProductCountID=" & ReadyString(ID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            For Each pObj As Object In Common.WebRequest.AllData("Count")
                p = New InsertUpdate("TransInventoryProductCountStock")

                Quantity = IIf(CStr2(pObj("Quantity")) = "", 0, CDbl(pObj("Quantity")))

                If BinID = vbNullString Then
                    ExpirationDate = pObj("ExpirationDate")
                    BinID = pObj("BinID")
                ElseIf Not BinID = pObj("BinID") Then
                    AddToInventoryLedger(ID, BinID, Total, ExpirationDate)

                    AddOrSetNewDefaultBin(ProductID, FOWSTID, BinID)

                    ExpirationDate = pObj("ExpirationDate")
                    BinID = pObj("BinID")
                    Total = 0
                End If

                If Quantity > 0 Then
                    ' [4/16/2014 10:57 AM]
                    If Quantity <> Math.Floor(Quantity) Then
                        If WebRequest.Instance.Business.LibraryStock.GetAllowDecimal(ProductID) = False Then
                            Return "Product, " & ProductID & ", does not allow decimals in quantity. You entered " & Quantity & "."
                        End If
                    End If

                    p("InventoryProductCountID") = ReadyString(ID)
                    p("BinID") = ReadyString(pObj("BinID"))

                    If Not pObj("ExpirationDate") = "N/A" Then
                        p("ExpirationDate") = ReadyString(pObj("ExpirationDate"))
                    End If

                    p("UnitMeasureID") = ReadyString(pObj("UnitMeasureID"))
                    p("Quantity") = Quantity
                    p("ActualQuantity") = pObj("ActualQuantity")
                    p("IsGoodStock") = pObj("IsGoodStock")
                    pSql = p.GetInsert()

                    Total += Quantity * pObj("ActualQuantity")

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next

            ComputeOffTake(ID, FOWSTID, BinID, ProductID, Total, Today)

            AddToInventoryLedger(ID, BinID, Total, ExpirationDate)

            AddOrSetNewDefaultBin(ProductID, FOWSTID, BinID)

            Common.WebRequest.ProData.CommitTransaction()

            Return "True"

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return ex.Message.ToString
        End Try
    End Function

    Function AddOrSetNewDefaultBin(ByVal ProductID As String, ByVal FowstID As String, ByVal BinID As String)
        Dim pSql As String
        Dim dt As New DataTable
        Dim pOut As String

        'check if the current bin is already assigned to this product in LibraryFowstBinProduct

        pSql = "SELECT dbo.LibraryFowstBin.ID, dbo.LibraryFowstBin.Name " &
                "FROM dbo.LibraryFowstBin INNER JOIN " &
                "dbo.LibraryFowstBinProduct ON dbo.LibraryFowstBin.FowstID = dbo.LibraryFowstBinProduct.FowstID AND  " &
                "dbo.LibraryFowstBin.ID = dbo.LibraryFowstBinProduct.BinID " &
                "WHERE dbo.LibraryFowstBinProduct.ProductID = " & ReadyString(ProductID) &
                " AND dbo.LibraryFowstBinProduct.IsDefault = 1 " &
                " AND  dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID) &
                " AND dbo.LibraryFowstBin.Name = 'Unnamed Bin'"

        dt = Common.WebRequest.ProData.Execute(pSql)

        'is this bin = UnNamedBin and default bin? if >0 then true;

        If dt.Rows.Count > 0 Then
            
            'dt.Rows(0).Item("ID") = 'UnNamed Bin'
            If CStr2(dt.Rows(0).Item("ID")) <> BinID Then

                'set default bin balance to 0
                AddToInventoryLedger(ProductID, dt.Rows(0).Item("ID").ToString, 0, "N/A")

                'remove isdefault to "Default Bin"
                pSql = "Update LibraryFowstBinProduct set IsDefault = 0 where ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & " AND BinID = " & ReadyString(dt.Rows(0).Item("ID").ToString) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID"))
                Common.WebRequest.ProData.ExecuteSQL(pSql)


                pSql = "Select getdate() FROM LibraryFowstBinProduct"
                pOut = Common.WebRequest.ProData.ExecuteScalar(pSql)

                'AddOrSetNewDefaultBin()
                pSql = "SELECT BinID" &
                 " FROM LibraryFowstBinProduct" &
                 " WHERE FowstID = " & ReadyString(FowstID) &
                 " AND ProductID = " & ReadyString(ProductID) &
                 " AND BinID = " & ReadyString(BinID)
                pOut = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

                If CStr2(pOut) <> "" Then
                    pSql = "Update LibraryFowstBinProduct set IsDefault = 1 where ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & " AND BinID = " & ReadyString(BinID) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID"))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    Dim pIU As InsertUpdate

                    pIU = New InsertUpdate("LibraryFowstBinProduct")
                    pIU("FowstID") = ReadyString(FowstID)
                    pIU("BinID") = ReadyString(BinID)
                    pIU("ProductID") = ReadyString(ProductID)
                    pIU("IsDefault") = 1
                    pSql = pIU.GetInsert
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            End If
        Else

            'so this bin is not unnamed bin and default, is there existing default bin?

            Dim dtTempBin As DataTable

            pSql = "SELECT BinID" &
                " FROM LibraryFowstBinProduct" &
                " WHERE FowstID = " & ReadyString(FowstID) &
                " AND ProductID = " & ReadyString(ProductID) &
                " AND dbo.LibraryFowstBinProduct.IsDefault = 1 "

            dtTempBin = Common.WebRequest.ProData.Execute(pSql)

            If dtTempBin.Rows.Count = 0 Then
                'set this bin as default bin

                pSql = "Update LibraryFowstBinProduct set IsDefault = 1 where ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & " AND BinID = " & ReadyString(BinID) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID"))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

        End If

        
        'pSql = "Select getdate() FROM LibraryFowstBinProduct"
        'pOut = Common.WebRequest.ProData.ExecuteScalar(pSql)

        ''AddOrSetNewDefaultBin()
        'pSql = "SELECT BinID" &
        ' " FROM LibraryFowstBinProduct" &
        ' " WHERE FowstID = " & ReadyString(FowstID) &
        ' " AND ProductID = " & ReadyString(ProductID) &
        ' " AND BinID = " & ReadyString(BinID)
        'pOut = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))



        'If CStr2(pOut) <> "" Then
        '    pSql = "Update LibraryFowstBinProduct set IsDefault = 1 where ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & " AND BinID = " & ReadyString(BinID) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID"))
        '    Common.WebRequest.ProData.ExecuteSQL(pSql)
        'Else
        '    Dim pIU As InsertUpdate

        '    pIU = New InsertUpdate("LibraryFowstBinProduct")
        '    pIU("FowstID") = ReadyString(FowstID)
        '    pIU("BinID") = ReadyString(BinID)
        '    pIU("ProductID") = ReadyString(ProductID)
        '    pIU("IsDefault") = 1
        '    pSql = pIU.GetInsert
        '    Common.WebRequest.ProData.ExecuteSQL(pSql)
        'End If

    End Function


    <HttpPost()>
    Function SaveBinName() As String
        Dim p As New InsertUpdate("LibraryFowstBin")
        Dim pSql As String

        p("Name") = ReadyString(WebRequest.Instance.Data("Name"))

        pSql = p.GetUpdate("ID = " & ReadyString(WebRequest.Instance.Data("FowstBinID")))

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return "True"
    End Function

    <HttpPost()>
    Function AddToInventoryLedger(ByVal TransactionID As String, ByVal BinID As String, ByVal Total As Double, ByVal ExpDate As String)
        Dim EDate As Date?

        If Not ExpDate = "N/A" Then
            EDate = CDate(ExpDate)
        End If

        WebRequest.Instance.Business.TransInventoryLedger.Add( _
                        CStr2(WebRequest.Instance.Data("ProductID")), _
                        CStr2(WebRequest.Instance.Data("FowstID")), _
                        TransactionID, _
                        TransInventoryLedger.eTransType.InventoryCount, _
                        WebSession.Instance.PeopleUserID, , _
                        Total, _
                        BinID,
                        EDate
                    )
    End Function

    Function GetAllFowstBinProduct() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields

                With .Add("ID", SqlDbType.Int)
                    .Expression = "LibraryFowstBinProduct.ID"
                End With

                With .Add("ProductID", SqlDbType.Int)
                    .Expression = "MobizLibraryPIMProduct.ID"
                End With

                With .Add("ProductName", SqlDbType.Int)
                    .Expression = "MobizLibraryPIMProduct.ProductNo + '-' + MobizLibraryPIMProduct.ProductName"
                End With

                With .Add("BinName", SqlDbType.Bit)
                    .Expression = "LibraryFowstBin.Name"
                End With

                With .Add("UnitMeasure", SqlDbType.Bit)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With

                With .Add("Balance", SqlDbType.Int)
                    .Expression = "LibraryFowstBinProduct.Balance"
                End With

                With .Add("MaxPerProduct", SqlDbType.Int)
                    .Expression = "LibraryFowstBinProduct.MaxPerProduct"
                End With

                With .Add("MinPerProduct", SqlDbType.Int)
                    .Expression = "LibraryFowstBinProduct.MinPerProduct"
                End With

                With .Add("ReorderQty", SqlDbType.Int)
                    .Expression = "LibraryFowstBinProduct.ReorderQty"
                End With

                With .Add("MaxMinDiff", SqlDbType.Int)
                    .Expression = "ISNULL(LibraryFowstBinProduct.Balance,0) - ISNULL(LibraryFowstBinProduct.MinPerProduct,0)"
                End With

            End With

            .SqlFrom = "FROM LibraryFowstBinProduct INNER JOIN " & vbCrLf &
                        "LibraryFowstBin on LibraryFowstBinProduct.BinID = LibraryFowstBin.ID INNER JOIN " & vbCrLf &
                        "MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = LibraryFowstBinProduct.ProductID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProductMeasurement ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductMeasurement.ProductID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID "

            .SqlWhere = "WHERE LibraryFowstBinProduct.FowstID = " & CStr2(ReadyString(Common.WebRequest.Data("FowstID")))

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetInventoryCountReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        Dim mCtrWeeks As Integer
        Dim mThisSunday = DateAdd(DateInterval.Day, -Today.DayOfWeek, Today)
        Dim mThisSaturday = DateAdd(DateInterval.Day, 6, mThisSunday)

        Dim mReportFrom As Date
        Dim mReportTo As Date

        Dim SqlWhere As String

        'Select Case WebRequest.Instance.Data("Within")
        '    Case "optThisWeek"
        '        mCtrWeeks = 0
        '    Case "optLastWeek"
        '        mCtrWeeks = 1
        '    Case "optLastTwoWeeks"
        '        mCtrWeeks = 2
        '    Case "optLastThreeWeeks"
        '        mCtrWeeks = 3
        '    Case "optLastMonth"
        '        Dim mStartOfMonth As New Date(Today.Year, Today.Month, 1)
        '        mReportFrom = DateAdd(DateInterval.Month, -1, mStartOfMonth)
        '        mReportTo = DateAdd(DateInterval.Day, -1, mStartOfMonth)
        '        mCtrWeeks = 4
        'End Select

        'If mCtrWeeks <= 3 Then
        '    mReportFrom = DateAdd(DateInterval.Day, -(mCtrWeeks * 7), mThisSunday)
        '    mReportTo = DateAdd(DateInterval.Day, -(mCtrWeeks * 7), mThisSaturday)
        'End If

        mReportFrom = CStr2(Common.WebRequest.Data("DateFrom"))
        mReportTo = CStr2(Common.WebRequest.Data("DateTo"))

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields

                With .Add("BINID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryFowstBin.ID"
                End With

                With .Add("BIN", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryFowstBin.Name"
                End With

                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.MobizLibraryPIMProduct.ID"
                End With

                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With

                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                End With

                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProductVariableFeatures001.C0020"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                End With

                With .Add("UnitMeasure", SqlDbType.Bit)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With

                With .Add("CountDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(VARCHAR(8), dbo.TransInventoryLedger.LastUpdated, 1)"
                End With

                With .Add("CountOnRecord", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.TransInventoryLedger.Balance - dbo.TransInventoryLedger.Qty"
                End With

                With .Add("PhysicalCount", SqlDbType.Int)
                    .Expression = "dbo.TransInventoryLedger.Balance"
                End With

                'With .Add("Direction", SqlDbType.Int)
                '    .Expression = "CASE WHEN dbo.TransInventoryLedger.Qty < 0 THEN 'NEG (-)' WHEN dbo.TransInventoryLedger.Qty > 0 THEN 'POS (+)' ELSE '0' END"
                'End With

                With .Add("Absolute", SqlDbType.Int)
                    .Expression = "dbo.TransInventoryLedger.Qty"
                End With

                With .Add("Percentage", SqlDbType.Int)
                    .Expression = "CASE WHEN CONVERT(decimal, dbo.TransInventoryLedger.Balance - dbo.TransInventoryLedger.Qty) * 100 = 0.00 THEN 0 ELSE CONVERT(decimal(18, 3), (dbo.TransInventoryLedger.Qty) / CONVERT(decimal, dbo.TransInventoryLedger.Balance - dbo.TransInventoryLedger.Qty) * 100) END"
                End With

                With .Add("Value", SqlDbType.Int)
                    .Expression = "ABS(dbo.TransInventoryLedger.Qty) * dbo.TransInventoryLedger.Value"
                End With

                With .Add("CountedBy", SqlDbType.Int)
                    .Expression = "ppl1.Surname + ' ' + ppl1.FirstName"
                End With

                With .Add("TurnedOverTo", SqlDbType.Int)
                    .Expression = "ppl2.Surname + ' ' + ppl2.FirstName"
                End With

                With .Add("TurnOverDate", SqlDbType.Int)
                    .Expression = "TransInventoryProductCountStock.TurnOverDate"
                End With

                With .Add("Remarks", SqlDbType.Int)
                    .Expression = "Remarks"
                End With

            End With

            .SqlFrom = "FROM dbo.TransInventoryLedger INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.TransInventoryLedger.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInventoryLedger.FowstID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPeople ppl1 ON dbo.TransInventoryLedger.UserID = ppl1.ID INNER JOIN " & vbCrLf &
                      "TransInventoryProductCountStock on TransInventoryProductCountStock.InventoryProductCountID = TransInventoryLedger.TransactionID LEFT JOIN " & vbCrLf &
                      "dbo.MobizLibraryPeople ppl2 ON dbo.TransInventoryProductCountStock.TurnOverToID = ppl2.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProductMeasurement ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductMeasurement.ProductID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID INNER JOIN " & vbCrLf &
                      "LibraryFowstBin on LibraryFowstBin.ID = TransInventoryLedger.BinID " & vbCrLf &
                      "INNER JOIN  MobizLibraryPIMProductVariableFeatures001 " & vbCrLf &
                      "ON dbo.TransInventoryLedger.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID "


            .SqlWhere = "WHERE dbo.TransInventoryLedger.TransactionType = 'InventoryCount' AND convert(date, dbo.TransInventoryLedger.LastUpdated) Between " & ReadyString(mReportFrom) & " AND " & ReadyString(mReportTo)


            If Not WebRequest.Instance.Data("FowstID").ToString = vbNullString Then
                .SqlWhere += " AND dbo.TransInventoryLedger.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID"))
            End If

            .SqlOrder = "ORDER BY CountDate"

            pDT = .Requery
        End With

        'If pDT.Rows.Count > 0 Then
        '    Dim dr As DataRow

        '    dr = pDT.NewRow
        '    dr(3) = "Total Value of All Variances"
        '    dr(4) = pDT.Compute("Sum(Value)", "")

        '    pDT.Rows.Add(dr)

        '    dr = pDT.NewRow
        '    dr(3) = "Total Value of Negative Variances"
        '    dr(4) = pDT.Compute("Sum(Value)", "Absolute<0")

        '    pDT.Rows.Add(dr)

        '    dr = pDT.NewRow
        '    dr(3) = "Total Value of Positive Variances"
        '    dr(4) = pDT.Compute("Sum(Value)", "Absolute>0'")

        '    pDT.Rows.Add(dr)

        'End If
        Return pDT
    End Function

    Function GetInventoryCountReportByProduct() As DataTable
        Dim pDT As DataTable
        Dim pSql As String

        Dim mCtrWeeks As Integer
        Dim mThisSunday = DateAdd(DateInterval.Day, -Today.DayOfWeek, Today)
        Dim mThisSaturday = DateAdd(DateInterval.Day, 6, mThisSunday)

        Dim mReportFrom As Date
        Dim mReportTo As Date

        Select Case WebRequest.Instance.Data("Within")
            Case "optThisWeek"
                mCtrWeeks = 0
            Case "optLastWeek"
                mCtrWeeks = 1
            Case "optLastTwoWeeks"
                mCtrWeeks = 2
            Case "optLastThreeWeeks"
                mCtrWeeks = 3
            Case "optLastMonth"
                Dim mStartOfMonth As New Date(Today.Year, Today.Month, 1)
                mReportFrom = DateAdd(DateInterval.Month, -1, mStartOfMonth)
                mReportTo = DateAdd(DateInterval.Day, -1, mStartOfMonth)
                mCtrWeeks = 4
        End Select

        If mCtrWeeks <= 3 Then
            mReportFrom = DateAdd(DateInterval.Day, -(mCtrWeeks * 7), mThisSunday)
            mReportTo = DateAdd(DateInterval.Day, -(mCtrWeeks * 7), mThisSaturday)
        End If

        pSql = "SELECT dbo.MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
            " ,dbo.MobizLibraryPIMProduct.ProductName" & vbCrLf & _
            " ,TransInventoryLedger.Qty Quantity" & vbCrLf & _
            " ,TransInventoryLedger.Balance" & vbCrLf & _
            " ,MobizLibraryOrganizationFowst.Name AS Fowst" & vbCrLf & _
            " ,TransInventoryLedger.LastUpdated" & vbCrLf & _
            " ,LibraryFowstBin.Name AS BinName" & vbCrLf & _
            ",ISNULL(tSource.TransName, TransInventoryLedger.TransactionType) AS Source " & vbCrLf & _
            " " & vbCrLf & _
            " FROM TransInventoryLedger" & vbCrLf & _
            " JOIN MobizLibraryPIMProduct ON TransInventoryLedger.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf & _
            " JOIN MobizLibraryOrganizationFowst ON MobizLibraryOrganizationFowst.ID = TransInventoryLedger.FowstID" & vbCrLf & _
            " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInventoryLedger.BinID" & vbCrLf & _
            " OUTER APPLY (" & vbCrLf & _
            " 	SELECT 'Sales Invoice ' + CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName" & vbCrLf & _
            "     FROM TransInvoice" & vbCrLf & _
            "     WHERE TransInvoice.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Receiving ' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf & _
            "     FROM TransReceiving" & vbCrLf & _
            "     WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Receiving'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Transfer Out ' + CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName" & vbCrLf & _
            "     FROM TransReceiving" & vbCrLf & _
            "     WHERE TransReceiving.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'TransferOut'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Sales Return ' + CONVERT(varchar, TransMemo.MemoNumber) AS TransName" & vbCrLf & _
            "     FROM TransMemo" & vbCrLf & _
            "     JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID" & vbCrLf & _
            "     WHERE TransMemo.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'Sales Returns'" & vbCrLf & _
            "     " & vbCrLf & _
            "     UNION" & vbCrLf & _
            "     " & vbCrLf & _
            "     SELECT 'Inventory Count ' + CONVERT(varchar, TransInventoryProductCount.InventoryCountDate) AS TransName" & vbCrLf & _
            "     FROM TransInventoryProductCount" & vbCrLf & _
            "     WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID" & vbCrLf & _
            "     AND TransInventoryLedger.TransactionType = 'InventoryCount'" & vbCrLf & _
            " ) tSource" & vbCrLf &
            " Where dbo.MobizLibraryPIMProduct.ID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & vbCrLf &
            " AND dbo.TransInventoryLedger.FowstID = " & ReadyString(WebRequest.Instance.Data("Fowst")) & vbCrLf &
            " AND dbo.TransInventoryLedger.BinID = " & ReadyString(WebRequest.Instance.Data("BinID")) & vbCrLf &
            " AND TransInventoryLedger.LastUpdated Between " & ReadyString(mReportFrom) & " AND " & ReadyString(mReportTo) & vbCrLf &
            " ORDER BY TransInventoryLedger.LastUpdated" & vbCrLf

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetCountPIMProductGroupNo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dbo.MobizLibraryPIMProduct.ID"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo + ' ' + dbo.MobizLibraryPIMProduct.ProductName"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProductVariableFeatures001.C0017"
                End With
                With .Add("Price", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMproductPriceLevel.Price"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPIMProduct " & vbCrLf &
                        "inner join dbo.MobizLibraryPIMProductVariableFeatures001 on dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                        "inner join MobizLibraryPIMproductPriceLevel on MobizLibraryPIMproductPriceLevel.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                        "inner join LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                      "inner join MobizLibraryPIMPriceLevel on MobizLibraryPIMPriceLevel.ID = MobizLibraryPIMproductPriceLevel.PriceLevelID"

            .SqlWhere = "Where ProductGroupNo = " & ReadyString(WebRequest.Instance.Data("ProductGroupNo")) & "  AND MobizLibraryPIMPriceLevel.PriceLevel='Retail Price'"

            .SqlOrder = "Order By dbo.MobizLibraryPIMProductVariableFeatures001.C0017"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function ComputeOffTake(ByVal InventoryCountID As String, ByVal FOWSTID As String, ByVal BinID As String, ByVal ProductID As String, ByVal TotalCount As Double, ByVal CountDate As Date)
        Dim pSql As String
        Dim dtPreviousBalance As DataTable
        Dim OffTake As Double

        pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                "WHERE ProductID = " & ReadyString(ProductID) & vbCrLf &
                " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                " AND BINID = " & ReadyString(BinID) & vbCrLf &
                " order by LastUpdated desc"

        dtPreviousBalance = Common.WebRequest.ProData.Execute(pSql)

        If dtPreviousBalance.Rows.Count > 0 Then
            OffTake = CDbl(dtPreviousBalance.Rows(0).Item("Balance")) - TotalCount

            Dim p As New InsertUpdate("TransOffTake")

            p("OfftakeDate") = ReadyString(CountDate)
            p("InventoryCountID") = ReadyString(InventoryCountID)
            p("ProductID") = ReadyString(ProductID)
            p("FowstID") = ReadyString(FOWSTID)
            p("BinID") = ReadyString(BinID)
            p("OffTake") = ReadyString(OffTake)

            pSql = p.GetInsert

            Common.WebRequest.ProData.ExecuteSQL(pSql)
        End If
    End Function

    Function CheckExcelColumns(ByVal vDTable As DataTable, ByVal vOrgID As String) As DataTable
        Dim pFinder As Finder
        Dim pDTReturn As DataTable
        Dim pDTemplate As DataTable
        Dim pDT As DataTable
        Dim blnRet As Boolean = False
        Dim intTemplateColCntr As Int16
        Dim intExcelColCntr As Int16
        Dim strTemplateID As String = ""
        Dim blnIsColumnExist As Boolean = False

        ''Get Org Templates 
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.ID"
                    .Searchable = False
                End With
                With .Add("PIMTemplateID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMTemplateID"
                    .Searchable = False
                End With
                With .Add("ColumnName", SqlDbType.VarChar)
                    .Expression = "tmpltecol.ColumnName"
                End With
                With .Add("PIMFeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "tmpltecol.PIMFeatureID"
                    .Searchable = False
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "tmpltecol.FeatureType"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMTemplateColumns tmpltecol " &
                " INNER JOIN MobizLibraryPIMTemplate tmplte ON (tmpltecol.PIMTemplateID = tmplte.ID)"

            .SqlWhere = " WHERE tmplte.OrganizationID = " & ReadyString(vOrgID)

            .SqlOrder = " ORDER BY tmpltecol.ColumnName ASC"

            pDT = New DataTable()
            pDT = .Requery

        End With

        pDTReturn = vDTable.Clone
        If (pDT.Rows.Count > 0) Then
            intExcelColCntr = 0
            While (intExcelColCntr < vDTable.Rows.Count)

                intTemplateColCntr = 0
                blnIsColumnExist = False
                While (intTemplateColCntr < pDT.Rows.Count)
                    If (vDTable.Rows(intExcelColCntr).Item("ExcelColumn").ToString().Trim() = pDT.Rows(intTemplateColCntr).Item("ColumnName").ToString().Trim()) Then
                        blnIsColumnExist = True
                        Exit While
                    End If

                    intTemplateColCntr = intTemplateColCntr + 1
                End While

                If (Not blnIsColumnExist) Then
                    pDTReturn.Rows.Add(vDTable.Rows(intExcelColCntr).ItemArray())
                End If

                intExcelColCntr = intExcelColCntr + 1
            End While
        Else
            pDTReturn = vDTable
        End If

        blnRet = blnIsColumnExist

        Return pDTReturn

    End Function

    Function GetBinBalances() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "t1.BinID"
                End With

                With .Add("BinName", SqlDbType.VarChar)
                    .Expression = "LibraryFowstBin.Name"
                End With

                With .Add("Qty", SqlDbType.Decimal)
                    .Expression = "t1.Qty"
                End With

                With .Add("Balance", SqlDbType.Decimal)
                    .Expression = "t1.Balance"
                End With

                With .Add("UnitMeasureID", SqlDbType.Decimal)
                    .Expression = "MobizLibraryPIMProductMeasurement.UnitMeasureID"
                End With

                With .Add("MaxPerProduct", SqlDbType.Decimal)
                    .Expression = "LibraryFowstBinProduct.MaxPerProduct"
                End With

                With .Add("MinPerProduct", SqlDbType.Decimal)
                    .Expression = "LibraryFowstBinProduct.MinPerProduct"
                End With

                With .Add("ReorderQty", SqlDbType.Decimal)
                    .Expression = "LibraryFowstBinProduct.ReorderQty"
                End With

            End With

            .SqlFrom = "FROM TransInventoryLedger TransInventoryLedger1 " & vbCrLf &
                        "inner join LibraryFowstBin on LibraryFowstBin.ID = TransInventoryLedger1.BinID " & vbCrLf &
                        "inner join LibraryFowstBinProduct on LibraryFowstBin.ID = LibraryFowstBinProduct.BinID " & vbCrLf &
                        "AND LibraryFowstBinProduct.ProductID =  TransInventoryLedger1.ProductID " & vbCrLf &
                        "AND LibraryFowstBinProduct.FowstID =  TransInventoryLedger1.FowstID " & vbCrLf &
                        "inner join MobizLibraryPIMProductMeasurement on MobizLibraryPIMProductMeasurement.ProductID = TransInventoryLedger1.ProductID " & vbCrLf &
                        "OUTER APPLY(  " & vbCrLf &
                        "Select top 1 TransInventoryLedger2.BinID, Qty, Balance " & vbCrLf &
                        "from TransInventoryLedger  TransInventoryLedger2 " & vbCrLf &
                        "where " & vbCrLf &
                        "FowstID = TransInventoryLedger1.FowstID " & vbCrLf &
                        "and BinID = TransInventoryLedger1.BinID " & vbCrLf &
                        "and ProductID = TransInventoryLedger1.ProductID " & vbCrLf &
                        "order by LastUpdated desc " & vbCrLf &
                        ") t1 "

            .SqlWhere = "Where TransInventoryLedger1.ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) &
                        " AND TransInventoryLedger1.FOWSTID = " & ReadyString(WebRequest.Instance.Data("FOWSTID")) &
                        " AND MobizLibraryPIMProductMeasurement.Quantity = 1"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetInventoryCountForApproval() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInventoryProductCountStock.ID"
                End With

                With .Add("CountDate", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInventoryProductCount.InventoryCountDate"
                End With

                With .Add("BinID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInventoryProductCountStock.BinID"
                End With

                With .Add("Quantity", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCountStock.Quantity"
                End With

                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.name"
                End With

                With .Add("Bin", SqlDbType.VarChar)
                    .Expression = "LibraryFowstBin.Name"
                End With

                With .Add("Variance", SqlDbType.Decimal)
                    .Expression = "Variance"
                End With

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With
            End With

            .SqlFrom = "FROM TransInventoryProductCountStock " & vbCrLf &
                        " INNER JOIN TransInventoryProductCount on TransInventoryProductCountStock.InventoryProductCountID=TransInventoryProductCount.ID " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryPIMProductMeasurement ON TransInventoryProductCount.ProductID = dbo.MobizLibraryPIMProductMeasurement.ProductID  " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID  " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID " & vbCrLf &
                        " INNER JOIN LibraryFowstBin on LibraryFowstBin.ID = TransInventoryProductCountStock.BinID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransInventoryProductCount.FowstID"

            .SqlWhere = "Where TransInventoryProductCount.ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & " AND Variance <> 0 AND IsActionTaken = 0 "

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetInventoryCountForApprovalHistory() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInventoryProductCountStock.ID"
                End With

                With .Add("CountDate", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInventoryProductCount.InventoryCountDate"
                End With

                With .Add("ProductName", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPIMProduct.ProductName"
                End With

                With .Add("BinID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInventoryProductCountStock.BinID"
                End With

                With .Add("Quantity", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCountStock.Quantity"
                End With

                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.name"
                End With

                With .Add("Bin", SqlDbType.VarChar)
                    .Expression = "LibraryFowstBin.Name"
                End With

                With .Add("Variance", SqlDbType.Decimal)
                    .Expression = "Variance"
                End With

                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With

                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.MiddleName + ' ' + MobizLibraryPeople.Surname"
                End With

                With .Add("ModifiedDate", SqlDbType.VarChar)
                    .Expression = "TransInventoryProductCountStock.VarianceApprovedDate"
                End With

                With .Add("Status", SqlDbType.VarChar)
                    .Expression = "CASE WHEN TransInventoryProductCountStock.IsVarianceApproved = 1 THEN 'Approved' ELSE 'Disapproved' END"
                End With
            End With

            .SqlFrom = "FROM TransInventoryProductCountStock " & vbCrLf &
                        " INNER JOIN TransInventoryProductCount on TransInventoryProductCountStock.InventoryProductCountID=TransInventoryProductCount.ID " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryPIMProductMeasurement ON TransInventoryProductCount.ProductID = dbo.MobizLibraryPIMProductMeasurement.ProductID  " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryPIMMeasure ON dbo.MobizLibraryPIMProductMeasurement.UnitMeasureID = dbo.MobizLibraryPIMMeasure.ID  " & vbCrLf &
                        " INNER JOIN dbo.MobizLibraryCountriesLanguageLabel ON dbo.MobizLibraryPIMMeasure.MeasureNameID = dbo.MobizLibraryCountriesLanguageLabel.ID " & vbCrLf &
                        " INNER JOIN LibraryFowstBin on LibraryFowstBin.ID = TransInventoryProductCountStock.BinID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransInventoryProductCount.FowstID" & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = TransInventoryProductCountStock.VarianceApprovedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInventoryProductCount.ProductID "

            If WebRequest.Instance.Data("IsAllItem") <> Nothing Then
                'Only for Inventory Levels > Inventory Count Approved/Disapproved > History
                .SqlWhere = "Where Variance <> 0 AND IsActionTaken = 1 " & vbCrLf &
                            " AND MobizLibraryOrganizationFOWST.ID = " & ReadyString(WebRequest.Instance.Data("FowstID")) & vbCrLf
            Else
                .SqlWhere = "Where TransInventoryProductCount.ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & " AND Variance <> 0 AND IsActionTaken = 1 " & vbCrLf &
                            " AND MobizLibraryOrganizationFOWST.ID = " & ReadyString(WebRequest.Instance.Data("FowstID")) & vbCrLf
            End If

            .SqlOrder = "ORDER BY TransInventoryProductCountStock.VarianceApprovedDate DESC"

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function ApprovedInventoryCount() As String
        Dim p As New InsertUpdate("TransInventoryProductCountStock")
        Dim pSql As String
        Dim dtInventoryProductCountInfo As DataTable
        Dim dtTransactionAfterCount As DataTable

        Try
            Common.WebRequest.ProData.BeginTransaction()

            p("IsActionTaken") = 1
            p("VarianceApprovedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("VarianceApprovedDate") = "getdate()"

            If CBool2(Common.WebRequest.Data("IsVarianceDisApproved")) Then
                p("IsVarianceDisApproved") = 1
            Else
                p("IsVarianceApproved") = 1
            End If

            pSql = p.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("ID")))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            If Not CBool2(Common.WebRequest.Data("IsVarianceDisApproved")) Then

                WebRequest.Instance.Business.TransInventoryLedger.Add( _
                vStockID:=CStr2(Common.WebRequest.Data("ProductID")),
                vFowstID:=CStr2(Common.WebRequest.Data("FOWSTID")),
                vTransactionID:=Common.WebRequest.Data("ID"),
                vTransactionType:=TransInventoryLedger.eTransType.InventoryCount,
                vUserID:=CStr2(WebSession.Instance.PeopleUserID),
                vQty:=CDec2(Common.WebRequest.Data("Variance")),
                vBalance:=CDec2(Common.WebRequest.Data("Count")),
                vBinID:=CStr2(Common.WebRequest.Data("BinID")),
                vInventoryCountDate:=CDate(Common.WebRequest.Data("CountDate"))
            )


                'get all products with last updated later than the count
                pSql = "Select * from TransInventoryLedger " & vbCrLf &
                                "where LastUpdated > " & ReadyString(CDate(Common.WebRequest.Data("CountDate"))) & vbCrLf &
                                "AND TransInventoryLedger.FowstID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) & vbCrLf &
                                "AND TransInventoryLedger.BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
                                "AND TransInventoryLedger.ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & vbCrLf &
                                " ORDER BY LastUpdated asc"

                dtTransactionAfterCount = Common.WebRequest.ProData.Execute(pSql)

                If dtTransactionAfterCount.Rows.Count > 0 Then
                    For i As Integer = 0 To dtTransactionAfterCount.Rows.Count - 1
                        Dim mTransType As TransInventoryLedger.eTransType

                        Select Case CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionType"))
                            Case "Invoice"
                                mTransType = TransInventoryLedger.eTransType.Invoice
                            Case "InventoryCount"
                                mTransType = TransInventoryLedger.eTransType.InventoryCount
                            Case "Receiving"
                                mTransType = TransInventoryLedger.eTransType.Receiving
                            Case "TransferOut"
                                mTransType = TransInventoryLedger.eTransType.TransferOut
                            Case "Sales Returns"
                                mTransType = TransInventoryLedger.eTransType.SalesReturn
                            Case "Product Maker"
                                mTransType = TransInventoryLedger.eTransType.SalesReturn
                        End Select

                        If mTransType <> TransInventoryLedger.eTransType.InventoryCount Then
                            '    WebRequest.Instance.Business.TransInventoryLedger.Add( _
                            '    vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                            '    vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                            '    vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                            '    vTransactionType:=mTransType,
                            '    vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                            '    vQty:=CDec2(dtTransactionAfterCount.Rows(i).Item("Qty"))
                            ')
                            Dim m As New InsertUpdate("TransInventoryLedger")

                            m("Balance") = CDec2(dtTransactionAfterCount.Rows(i).Item("Balance")) + Math.Abs(CDec2(Common.WebRequest.Data("Variance")))

                            pSql = m.GetUpdate("ID=" & ReadyString(dtTransactionAfterCount.Rows(i).Item("ID")))

                            Common.WebRequest.ProData.Execute(pSql)

                            Dim n As New InsertUpdate("TransInventoryLedgerOldBalances")

                            n("InventoryLedgerID") = ReadyString(dtTransactionAfterCount.Rows(i).Item("ID"))
                            n("Balance") = CDec2(dtTransactionAfterCount.Rows(i).Item("Balance"))
                            n("BalanceDate") = ReadyString(dtTransactionAfterCount.Rows(i).Item("LastUpdated"))

                            pSql = n.GetInsert()

                            Common.WebRequest.ProData.Execute(pSql)

                            'Else
                            '    WebRequest.Instance.Business.TransInventoryLedger.Add( _
                            '    vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                            '    vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                            '    vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                            '    vTransactionType:=mTransType,
                            '    vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                            '    vQty:=Nothing,
                            '    vBalance:=CDec2(dtTransactionAfterCount.Rows(i).Item("Balance")) + CDec2(Common.WebRequest.Data("Variance"))
                            ')
                        End If

                    Next
                End If
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return "True"

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return ex.Message.ToString
        End Try
    End Function

End Class
