Public Class BackOrder
    Dim pSql As String = vbNullString
    Dim pDT As DataTable

    Public Function getOriginalSKUInformation() As String
        Dim dt As New DataTable

        pSql = "SELECT dbo.TransSalesOrderItems.ID AS TransSalesOrderItemsID, dbo.TransSalesOrderItems.StockID, dbo.LibraryStockInformation.StockNumber, " &
                "ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '') " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '') AS StockDescription, dbo.TransSalesOrderItems.Quantity,  " &
                "dbo.TransSalesOrderItems.UnitMeasure, dbo.TransSalesOrderItems.UnitPrice, dbo.TransSalesOrderItems.SubTotal,  " &
                "dbo.TransSalesBackOrderItems.ID AS TransSalesBackOrderItemsID FROM dbo.TransSalesOrder INNER JOIN " &
                "dbo.TransSalesOrderItems ON dbo.TransSalesOrder.ID = dbo.TransSalesOrderItems.SalesOrderID INNER JOIN " &
                "dbo.LibraryStockInformation ON dbo.TransSalesOrderItems.StockID = dbo.LibraryStockInformation.ID INNER JOIN " &
                "dbo.LibraryBrands ON dbo.LibraryStockInformation.BrandID = dbo.LibraryBrands.ID INNER JOIN " &
                "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID INNER JOIN " &
                "dbo.TransSalesBackOrderItems ON dbo.TransSalesOrder.ID = dbo.TransSalesBackOrderItems.SalesOrderID AND  " &
                "dbo.TransSalesOrderItems.StockID = dbo.TransSalesBackOrderItems.StockID " &
                "WHERE dbo.TransSalesOrderItems.SalesOrderID = " & ReadyString(WebSession.Instance.OrderID) &
                " and dbo.TransSalesOrderItems.StockID = " & ReadyString(WebSession.Instance.BackOrderSKUID)

        'pSql = "SELECT DISTINCT dbo.LibraryStockInformation.StockNumber, ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') " &
        '        "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '') " &
        '        "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), " &
        '        "'') AS StockDescription, dbo.TransSalesOrderItems.Quantity, dbo.TransSalesOrderItems.UnitMeasure, dbo.TransSalesOrderItems.UnitPrice, " &
        '        "dbo.TransSalesOrderItems.SubTotal FROM dbo.TransSalesOrder INNER JOIN " &
        '        "dbo.TransSalesOrderItems ON dbo.TransSalesOrder.ID = dbo.TransSalesOrderItems.SalesOrderID INNER JOIN " &
        '        "dbo.LibraryStockInformation INNER JOIN " &
        '        "dbo.TransStockPurchaseItems ON dbo.TransStockPurchaseItems.StockID = dbo.LibraryStockInformation.ID INNER JOIN " &
        '        "dbo.LibraryBrands ON dbo.LibraryStockInformation.BrandID = dbo.LibraryBrands.ID INNER JOIN " &
        '        "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID ON " &
        '        "dbo.TransSalesOrderItems.StockID = dbo.LibraryStockInformation.ID " &
        '        "where dbo.TransSalesOrderItems.ID = " & ReadyString(sdt("m_orderID ")) & " AND dbo.LibraryStockInformation.ID = " & ReadyString(sdt("m_skuID"))

        'ReadyString(sdt("BackOrderItemsID"))
        With Common.WebRequest
            dt = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(dt)
        Return JsonDictionary.Serialize(vDt)

    End Function

    Public Function getSuggestedSKUInformation(ByVal sdt As Dictionary(Of String, Object)) As String
        Dim dt As New DataTable

        pSql = "SELECT DISTINCT  " &
                "TOP (1) dbo.TransSalesOrderItems.ID AS TransSalesOrderItemsID, dbo.TransSalesOrderItems.StockID, dbo.LibraryStockInformation.StockNumber, " &
                "ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '') AS StockDescription, dbo.TransSalesOrderItems.Quantity,  " &
                "dbo.TransSalesOrderItems.UnitMeasure, dbo.TransSalesOrderItems.UnitPrice, dbo.TransSalesOrderItems.SubTotal,  " &
                "dbo.TransSalesBackOrderItems.ID AS TransSalesBackOrderItemsID " &
                "FROM   dbo.TransSalesBackOrderItems INNER JOIN dbo.TransSalesOrder INNER JOIN " &
                "dbo.TransSalesOrderItems ON dbo.TransSalesOrder.ID = dbo.TransSalesOrderItems.SalesOrderID ON  " &
                "dbo.TransSalesBackOrderItems.RefStockID = dbo.TransSalesOrderItems.StockID INNER JOIN " &
                "dbo.LibraryBrands INNER JOIN " &
                "dbo.LibraryStockInformation ON dbo.LibraryBrands.ID = dbo.LibraryStockInformation.BrandID INNER JOIN " &
                "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID ON  " &
                "dbo.TransSalesBackOrderItems.StockID = dbo.LibraryStockInformation.ID " &
                "WHERE dbo.TransSalesOrderItems.SalesOrderID = " & ReadyString(WebSession.Instance.OrderID) &
                " and dbo.TransSalesBackOrderItems.RefStockID = " & ReadyString(WebSession.Instance.BackOrderSKUID) &
                " and dbo.TransSalesBackOrderItems.IsReplacement = 1"

        With Common.WebRequest
            dt = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(dt)
        Return JsonDictionary.Serialize(vDt)

    End Function

    Public Function GetBackOrder() As DataTable

        'CONVERT(VARCHAR(13), dbo.TransSalesOrder.OrderDate, 101) 

        pSql = "SELECT DISTINCT " &
                "dbo.TransSalesBackOrderItems.SalesBackOrderID, dbo.TransSalesOrderItems.SalesOrderID, dbo.LibraryCustomers.CompanyName, dbo.TransSalesOrder.OrderNo, " &
                "dbo.LibraryCustomers.ContactNumber, dbo.TransSalesOrder.OrderDate AS OrderDate, dbo.TransSalesOrder.LastUpdated " &
                "FROM dbo.TransSalesBackOrderItems INNER JOIN " &
                "dbo.TransSalesOrder INNER JOIN " &
                "dbo.TransSalesOrderItems ON dbo.TransSalesOrder.ID = dbo.TransSalesOrderItems.SalesOrderID ON  " &
                "dbo.TransSalesBackOrderItems.SalesOrderID = dbo.TransSalesOrderItems.SalesOrderID INNER JOIN " &
                "dbo.LibraryCustomers ON dbo.TransSalesOrder.CustomerID = dbo.LibraryCustomers.ID INNER JOIN " &
                "dbo.TransSalesBackOrder ON dbo.TransSalesBackOrderItems.SalesBackOrderID = dbo.TransSalesBackOrder.ID " &
                "where dbo.TransSalesOrderItems.SalesOrderID = " & ReadyString(WebSession.Instance.OrderID) &
                " and dbo.TransSalesBackOrder.IsWebUpdated = 0"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                WebSession.Instance.SalesBackOrderID = pDT.Rows(0).Item("SalesBackOrderID").ToString
            End If

        End With

        Return pDT
    End Function

    Public Function GetBackOrderDetails() As String

        pSql = "SELECT DISTINCT dbo.LibraryStockInformation.ID as StockID, " &
                "dbo.TransSalesBackOrderItems.SalesBackOrderID, dbo.TransSalesBackOrderItems.SalesOrderID, dbo.LibraryStockInformation.StockNumber, " &
                "ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '') AS StockDescription, dbo.TransStockSortingListItems.IsHasReplacement,  " &
                "dbo.TransSalesBackOrderItems.ActionTaken " &
                "FROM dbo.LibraryBrands INNER JOIN " &
                "dbo.LibraryStockInformation ON dbo.LibraryBrands.ID = dbo.LibraryStockInformation.BrandID INNER JOIN " &
                "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID INNER JOIN " &
                "dbo.TransSalesBackOrderItems ON dbo.LibraryStockInformation.ID = dbo.TransSalesBackOrderItems.StockID LEFT OUTER JOIN " &
                "dbo.TransStockSortingListItems ON dbo.TransSalesBackOrderItems.SalesOrderID = dbo.TransStockSortingListItems.SalesOrderID AND  " &
                "dbo.TransSalesBackOrderItems.StockID = dbo.TransStockSortingListItems.StockID " &
                "where dbo.TransSalesBackOrderItems.SalesOrderID = " & ReadyString(WebSession.Instance.OrderID) & " and dbo.TransSalesBackOrderItems.IsReplacement = 0 " &
                " and dbo.TransSalesBackOrderItems.IsCancelled = 0"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        If pDT.Rows.Count = 0 Then
            Transfer("http://localhost:3002/backorder/Err/")
            'Exit Function
        End If

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Function getReviewOrderDetails() As String

        pSql = "Select DISTINCT " &
                "dbo.LibraryStockInformation.ID AS StockID, dbo.TransSalesBackOrderItems.SalesBackOrderID, dbo.TransSalesBackOrderItems.SalesOrderID, " &
                "dbo.LibraryStockInformation.StockNumber, ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ', '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ', '), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.UnitSize)), ', '), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '),  " &
                "'') AS StockDescription, dbo.TransStockSortingListItems.IsHasReplacement, dbo.TransSalesBackOrderItems.ActionTaken, dbo.TransSalesBackOrderItems.RefStockID,  " &
                "dbo.TransSalesBackOrderItems.CancellationPeriod, dbo.TransSalesBackOrderItems.IsCancelled " &
                "FROM dbo.LibraryBrands INNER JOIN " &
                "dbo.LibraryStockInformation ON dbo.LibraryBrands.ID = dbo.LibraryStockInformation.BrandID INNER JOIN " &
                "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID INNER JOIN " &
                "dbo.TransSalesBackOrderItems ON dbo.LibraryStockInformation.ID = dbo.TransSalesBackOrderItems.StockID LEFT OUTER JOIN " &
                "dbo.TransStockSortingListItems ON dbo.TransSalesBackOrderItems.SalesOrderID = dbo.TransStockSortingListItems.SalesOrderID AND  " &
                "dbo.TransSalesBackOrderItems.StockID = dbo.TransStockSortingListItems.StockID " &
                "where dbo.TransSalesBackOrderItems.SalesOrderID = " & ReadyString(WebSession.Instance.OrderID) & " and dbo.TransSalesBackOrderItems.IsReplacement = 0 "

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)
    End Function
    Public Function SaveBackOrderSKUAction(ByVal sdt As Dictionary(Of String, Object)) As String
        Dim sql As String
        Dim myRating As Integer

        Select Case sdt("action")
            Case "wait"
                '" & ReadyString(sdt("val")) & " 
                sql = "Update TransSalesBackOrderItems set ActionTaken = " & ReadyString(sdt("action")) & ", CancellationPeriod = " & DateDiff(DateInterval.Day, Today, sdt("value")) & " where TransSalesBackOrderItems.ID = " & ReadyString(sdt("m_BackorderID"))
            Case "cancel"
                sql = "Update TransSalesBackOrderItems set ActionTaken = 'Cancelled', IsCancelled = 1 where TransSalesBackOrderItems.ID = " & ReadyString(sdt("m_BackorderID"))
            Case "replace"
                sql = "Update TransSalesBackOrderItems set ActionTaken = 'Replaced' where TransSalesBackOrderItems.ID =  " & ReadyString(sdt("m_BackorderID"))
        End Select

        With Common.WebRequest
            Return .ProData.ExecuteSQL(sql)
        End With

        Return ""
    End Function

    Public Function CreateNewOrder()
        Dim sql As String
        Dim ID As String
        Dim str As String
        Dim ctr As Integer
        Dim dtTemp As New DataTable

        sql = getReplacementDetails()

        If pDT.Rows.Count > 0 Then
            ID = Guid.NewGuid.ToString

            sql = "INSERT INTO TransStockPickingList(ID,IsRushPrint,UserID) VALUES (" & ReadyString(ID) & ",1,554)"

            With Common.WebRequest
                str = .ProData.ExecuteSQL(sql)
            End With

            sql = "Update TransStockPickingListItems SET IsRePrinted=1,RePrintPickingListID=" & ReadyString(ID) & " WHERE ID IS NULL "

            With Common.WebRequest
                str = .ProData.ExecuteSQL(sql)
            End With


            While ctr < pDT.Rows.Count

                sql = "Insert into TransStockPickingListItems( " &
                "StockPickingListID, " &
                "SalesOrderID, " &
                "StockID, " &
                "Quantity, " &
                "ActualQuantity, " &
                "UnitMeasure, " &
                "TermDiscount, " &
                "DiscountIfPaidWithIn, " &
                "NetDue, " &
                "UnitPrice, " &
                "SubTotal, " &
                "PriceLevelID, " &
                "IsBackOrder, " &
                "SalesBackOrderID, " &
                "CancellationPeriod)  " &
                "Values(" & ReadyString(ID) & "," &
                ReadyString(WebSession.Instance.OrderID) & "," &
                ReadyString(pDT.Rows(ctr).Item("StockID")) & "," &
                pDT.Rows(ctr).Item("Quantity") & "," &
                pDT.Rows(ctr).Item("ActualQuantity") & "," &
                ReadyString(pDT.Rows(ctr).Item("UnitMeasure")) & "," &
                ReadyString(pDT.Rows(ctr).Item("TermDiscount")) & "," &
                ReadyString(pDT.Rows(ctr).Item("DiscountIfPaidWithIn")) & "," &
                ReadyString(pDT.Rows(ctr).Item("NetDue")) & "," &
                pDT.Rows(ctr).Item("UnitPrice") & "," &
                pDT.Rows(ctr).Item("SubTotal") & "," &
                ReadyString(pDT.Rows(ctr).Item("PriceLevelID")) & "," &
                1 & "," &
                ReadyString(WebSession.Instance.SalesBackOrderID) & "," &
                 "0)"

                With Common.WebRequest
                    str = .ProData.ExecuteSQL(sql)
                End With

                Dim baseUnitMeasure As String

                sql = "SELECT LibraryStockMeasurements.MeasurementLevel, " &
                        "LibraryStockMeasurements.Quantity, " &
                        "LibraryUnitMeasures.UnitMeasure  " &
                        "FROM dbo.LibraryStockMeasurements INNER JOIN " &
                        "dbo.LibraryUnitMeasures ON dbo.LibraryStockMeasurements.UnitMeasureID = dbo.LibraryUnitMeasures.ID " &
                        " WHERE (LibraryStockMeasurements.StockID =" & ReadyString(pDT.Rows(ctr).Item("StockID")) & ")  "

                With Common.WebRequest
                    dtTemp = .ProData.Execute(sql)
                    baseUnitMeasure = dtTemp.Rows(0).Item("UnitMeasure")
                End With

                sql = "SELECT LibraryStockMeasurements.MeasurementLevel, " &
                        "LibraryStockMeasurements.Quantity, " &
                        "LibraryUnitMeasures.UnitMeasure  " &
                        "FROM dbo.LibraryStockMeasurements INNER JOIN " &
                        "dbo.LibraryUnitMeasures ON dbo.LibraryStockMeasurements.UnitMeasureID = dbo.LibraryUnitMeasures.ID " &
                        " WHERE (LibraryStockMeasurements.StockID =" & ReadyString(pDT.Rows(ctr).Item("StockID")) & ")  " &
                        " and dbo.LibraryUnitMeasures.UnitMeasure =" & ReadyString(pDT.Rows(ctr).Item("UnitMeasure"))

                With Common.WebRequest
                    dtTemp = .ProData.Execute(sql)
                End With

                sql = "INSERT INTO TransStockPickingListMeasurements(" &
                        "StockPickingListID, " &
                        "MeasurementLevel," &
                        "StockID," &
                        "Quantity," &
                        "ActualQuantity," &
                        "UnitMeasure," &
                        "BasedUnitMeasure," &
                        "WarehousePersonnel) " &
                        "VALUES( " &
                        ReadyString(ID) & "," &
                        ReadyString(dtTemp.Rows(0).Item("MeasurementLevel")) & "," &
                        ReadyString(pDT.Rows(ctr).Item("StockID")) & "," &
                        pDT.Rows(ctr).Item("Quantity") & "," &
                        pDT.Rows(ctr).Item("ActualQuantity") & "," &
                        ReadyString(pDT.Rows(ctr).Item("UnitMeasure")) & "," &
                        ReadyString(baseUnitMeasure) & "," &
                         "554)"

                With Common.WebRequest
                    str = .ProData.ExecuteSQL(sql)
                End With

                ctr += 1
            End While

            sql = "UPDATE TransSalesBackOrderItems SET  IsServe=1 ,LastUpdated = GetDate() WHERE (SalesBackOrderID  = " & ReadyString(WebSession.Instance.SalesBackOrderID) & " )  AND (SalesOrderID  =" & ReadyString(WebSession.Instance.OrderID) & " )  AND (IsCancelled = 0)  AND (IsServe = 0) "

            With Common.WebRequest
                str = .ProData.ExecuteSQL(sql)
            End With

            sql = "UPDATE TransSalesOrder SET DateSetRushOrder = GetDate() WHERE ID = " & ReadyString(WebSession.Instance.OrderID)

            With Common.WebRequest
                str = .ProData.ExecuteSQL(sql)
            End With

            sql = "UPDATE TransSalesBackOrder SET IsWebUpdated = 1 WHERE ID = " & ReadyString(WebSession.Instance.SalesBackOrderID)

            With Common.WebRequest
                str = .ProData.ExecuteSQL(sql)
            End With
        End If
    End Function

    Public Function getReplacementDetails()
        Dim sql As String

        sql = "SELECT DISTINCT " &
            "StockID " &
            "FROM dbo.TransSalesBackOrderItems " &
            "WHERE SalesOrderID = " & ReadyString(WebSession.Instance.OrderID) &
            " and ActionTaken = 'Replaced'"

        With Common.WebRequest
            pDT = .ProData.Execute(sql)
        End With

        sql = "SELECT DISTINCT " &
            "ID, SalesBackOrderID, SalesOrderID, StockID, Quantity, ActualQuantity, UnitMeasure, TermDiscount, DiscountIfPaidWithIn, NetDue, UnitPrice, SubTotal, PriceLevelID, " &
            "CancellationPeriod, IsCancelled, IsReadyToServe, ServeDateTime, IsInvoiced, UserID, LastUpdated, EncodedBy, RegisteredDate, UnitGP, UnitGPM, LineGP, " &
            "LineGPM, LineCost, Cost, IsServe, IsServe_Value, ActionTaken, IsReplacement, RefStockID, IsHasReplacement " &
            "FROM dbo.TransSalesBackOrderItems " &
            "WHERE SalesOrderID = " & ReadyString(WebSession.Instance.OrderID) & " and (RefStockID = " & ReadyString(pDT.Rows(0).Item("StockID"))

        Dim ctr = 1

        While ctr < pDT.Rows.Count
            sql += " or RefStockID = " & ReadyString(pDT.Rows(0).Item("StockID"))
            ctr += 1
        End While

        sql += ")"

        With Common.WebRequest
            pDT = .ProData.Execute(sql)
        End With
    End Function

    Function getBackOrderCancellationDate() As String
        Dim dt As New DataTable

        dt = GetBackOrder()

        pSql = "Select DefaultValue from SystemDefaults where Description = 'MBORTResponseTime'"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Function getWaitingTime(ByVal sdt As Dictionary(Of String, Object)) As String
        pSql = "Select CancellationPeriod from TransSalesBackOrderItems where ID = " & ReadyString(sdt("m_BackorderID"))

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Function getBackOrderCancellationAction() As String
        Dim dt As New DataTable
        Dim nDT As New DataTable
        nDT.Columns.Add("DefaultValue")

        dt = GetBackOrder()

        pSql = "Select DefaultValue from SystemDefaults where Description = 'MBORTUnAnsweredBOA'"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim dr As DataRow

        dr = nDT.NewRow

        dr(0) = DateAdd(DateInterval.Hour, pDT.Rows(0).Item("DefaultValue"), dt.Rows(0).Item("LastUpdated"))

        nDT.Rows.Add(dr)

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(nDT)
        Return JsonDictionary.Serialize(vDt)
    End Function
End Class
