Imports Newtonsoft.Json

Namespace WebProj
    Public Class PickProductsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /PickProducts

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            End If

            Return View()

        End Function
        Function SalesOrder() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            End If

            Return View()

        End Function

        Function SalesOrderStockViewer() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            End If

            Return View()

        End Function

        <HttpPost()>
        Public Function GetPendingSalesOrders() As String
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDT As DataTable
            Dim pSql As String

            pSql = "SELECT ID, CustomerID, FowstID" & vbCrLf &
            " FROM TransInvoice WHERE InvoiceNumber = 0 AND SalesOrderNo IS NOT NULL"

            pSql &= " ORDER BY InvoiceDate"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("Data", pDT)

            Return strReturn

        End Function

        <HttpPost()>
        Public Function getProductBinLocation() As String
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDT As DataTable
            Dim pSql As String
            Dim pSectionID As String
            Dim pProductID As String
            Dim pInvoiceID As String
            Dim pOut As JsonDictionary

            pSectionID = Common.WebRequest.Data("SectionID")
            If IsGuid(pSectionID) = False Then
                Return JsonDictionary.Serialize("Error", "SectionID must be a valid guid.")
            End If
            pSectionID = ReadyGuid(pSectionID)

            pProductID = Common.WebRequest.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "ProductID must be a valid guid.")
            End If
            pProductID = ReadyGuid(pProductID)

            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceID must be a valid guid.")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            pSql = "SELECT DISTINCT ISNULL(transBinProduct.PreparedQuantity, 0) as PreparedQuantity, binProd.BinID, bin.Name, binProd.FowstID, binProd.ProductID " &
                ",transBinProduct.UnitQty" & vbCrLf &
                ",transBinProduct.SectionID" & vbCrLf &
                " FROM LibraryFowstBinProduct binProd " &
                " INNER JOIN LibraryFowstBin bin ON binProd.BinID = bin.ID" &
                " OUTER APPLY (" & vbCrLf &
                "   SELECT PreparedQuantity" & vbCrLf &
                "   ,UnitQty" & vbCrLf &
                "   ,SectionID" & vbCrLf &
                "   FROM TransInvoiceItemWarehouseWithdrawal " &
                " WHERE InvoiceID = " & ReadyString(Common.WebRequest.Data("InvoiceID")) &
                " AND TransInvoiceItemWarehouseWithdrawal.SectionID = " & ReadyString(pSectionID) & vbCrLf &
                " AND ProductID = binProd.ProductID AND FowstBinID = binProd.BinID) transBinProduct " &
                " WHERE binProd.ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & " AND binProd.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) &
                " AND bin.Name <> 'Unnamed Bin' AND binProd.IsActive = 1 AND bin.IsActive = 1"

            pSql &= " ORDER BY bin.Name"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            Dim pOrderedAndWithdrawnQty As TransInvoice.OrderedAndWithdrawnQtyClass
            Common.WebRequest.ProData.BeginTransaction()

            pOrderedAndWithdrawnQty = WebRequest.Instance.Business.TransInvoice.OrderedAndWithdrawnQty(pInvoiceID, pProductID)
            pOut = New JsonDictionary
            pOut.Add("Data", pDT)
            pOut.Add("OrderedQty", pOrderedAndWithdrawnQty.OrderedQty)
            pOut.Add("WithdrawnQty", pOrderedAndWithdrawnQty.WithdrawnQty)
            strReturn = pOut.Serialize

            Return strReturn

        End Function

        <HttpPost()>
        Public Function saveFowstBinProduct() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDT As DataTable
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pProductID As String
            Dim pFowstID As String
            Dim pBinID As String


            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Stock ID")
            End If
            pProductID = ReadyGuid(pProductID)

            pFowstID = WebRequest.Instance.Data("FowstID")
            If IsGuid(pFowstID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Invoice ID")
            End If
            pFowstID = ReadyGuid(pFowstID)

            pBinID = WebRequest.Instance.Data("BinID")
            If IsGuid(pBinID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Bin ID")
            End If
            pBinID = ReadyGuid(pBinID)

            Try
                pSql = "SELECT ID FROM LibraryFowstBinProduct " &
                   " WHERE ProductID = " & ReadyString(pProductID) &
                   " AND FowstID = " & ReadyString(pFowstID) &
                   " AND BinID = " & ReadyString(pBinID)
                pDT = Common.WebRequest.ProData.Execute(pSql)


                If (pDT.Rows.Count < 1) Then
                    pIU = New InsertUpdate("LibraryFowstBinProduct")
                    pIU("FowstID") = ReadyString(pFowstID)
                    pIU("BinID") = ReadyString(pBinID)
                    pIU("ProductID") = ReadyString(pProductID)
                    pIU("IsDefault") = 0
                    pIU("MinPerProduct") = 0
                    pIU("MaxPerProduct") = 0
                    pIU("LastUpdated") = "GETDATE()"
                    pIU("IsActive") = 1


                    pSql = pIU.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)



                Else
                    pIU = New InsertUpdate("LibraryFowstBinProduct")

                    pIU("LastUpdated") = "GETDATE()"
                    pIU("IsActive") = 1

                    pSql = pIU.GetUpdate("ID =" & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                End If

                saveProductBinLocation()
                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Public Function removeFowstBinProduct() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDT As DataTable
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pProductID As String
            Dim pFowstID As String
            Dim pBinID As String
            Dim pInvoiceID As String

            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Stock ID")
            End If
            pProductID = ReadyGuid(pProductID)

            pFowstID = WebRequest.Instance.Data("FowstID")
            If IsGuid(pFowstID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Fowst ID")
            End If
            pFowstID = ReadyGuid(pFowstID)

            pBinID = WebRequest.Instance.Data("BinID")
            If IsGuid(pBinID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Bin ID")
            End If
            pBinID = ReadyGuid(pBinID)

            pInvoiceID = CStr2(WebRequest.Instance.Data("InvoiceID"))
            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid InvoiceID")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            Common.WebRequest.ProData.BeginTransaction()

            Try
                'pSql = "SELECT ID FROM LibraryFowstBinProduct " &
                '   " WHERE ProductID = " & ReadyString(pProductID) &
                '   " AND FowstID = " & ReadyString(pFowstID) &
                '   " AND BinID = " & ReadyString(pBinID)
                'pDT = Common.WebRequest.ProData.Execute(pSql)

                'If (pDT.Rows.Count > 0) Then
                '    pIU = New InsertUpdate("LibraryFowstBinProduct")
                '    pIU("LastUpdated") = "GETDATE()"
                '    pIU("IsActive") = 0

                '    pSql = pIU.GetUpdate("ID=" & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                '    Common.WebRequest.ProData.ExecuteSQL(pSql)
                'End If

                pSql = "DELETE FROM TransInvoiceItemWarehouseWithdrawal " &
              " WHERE InvoiceID = " & ReadyString(pInvoiceID) &
              " AND ProductID = " & ReadyString(pProductID) &
              " AND FowstBinID = " & ReadyString(pBinID)

                Common.WebRequest.ProData.Execute(pSql)

                'update transinvoice qty
                pSql = "SELECT SUM(PreparedQuantity) as qty FROM TransInvoiceItemWarehouseWithdrawal " &
                         " WHERE InvoiceID = " & ReadyString(pInvoiceID) &
                         " AND ProductID = " & ReadyString(pProductID)
                pDT = Common.WebRequest.ProData.Execute(pSql)


                If (pDT.Rows.Count > 0) Then
                    pIU = New InsertUpdate("TransInvoiceItems")
                    pIU("Quantity") = CDec2(pDT.Rows(0).Item("qty").ToString())

                    pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(pInvoiceID) & " AND ProductID = " & ReadyString(pProductID))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                End If

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Public Function saveProductBinLocation() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDT As DataTable
            Dim pIU As InsertUpdate
            Dim pSql As String
            'Dim pProductID As String
            'Dim pInvoiceID As String
            'Dim pBinID As String
            'Dim pUnitQty As Decimal
            'Dim pSectionID As String
            'Dim pUnitMeasure As String
            'Dim pQuantity As Decimal
            'Dim pStr As String

            'pProductID = WebRequest.Instance.Data("ProductID")
            'If IsGuid(pProductID) = False Then
            '    Return JsonDictionary.Serialize("Error", "Invalid Stock ID")
            'End If
            'pProductID = ReadyGuid(pProductID)

            'pInvoiceID = WebRequest.Instance.Data("InvoiceID")
            'If IsGuid(pInvoiceID) = False Then
            '    Return JsonDictionary.Serialize("Error", "Invalid Invoice ID")
            'End If
            'pInvoiceID = ReadyGuid(pInvoiceID)

            'pBinID = WebRequest.Instance.Data("BinID")
            'If IsGuid(pBinID) = False Then
            '    Return JsonDictionary.Serialize("Error", "Invalid Bin ID")
            'End If
            'pBinID = ReadyGuid(pBinID)

            'pStr = Common.WebRequest.Data("UnitQty")
            'If IsNumeric(pStr) = False Then
            '    Return JsonDictionary.Serialize("Error", "UnitQty must be numeric")
            'End If
            'pUnitQty = CDec2(pStr)

            'pSectionID = WebRequest.Instance.Data("SectionID")
            'If IsGuid(pSectionID) = False Then
            '    Return JsonDictionary.Serialize("Error", "Invalid section ID")
            'End If
            'pSectionID = ReadyGuid(pSectionID)

            'pUnitMeasure = Common.WebRequest.Data("UnitMeasure")
            'If CStr2(pUnitMeasure).Trim() = "" Then
            '    Return JsonDictionary.Serialize("Error", "No unit measure")
            'End If

            'pStr = WebRequest.Instance.Data("Quantity")
            'If IsNumeric(pStr) = False Then
            '    Return JsonDictionary.Serialize("Error", "Quantity must be numeric")
            'End If
            'pQuantity = CDec2(pStr)

            Dim pInput As saveProductBinLocationClass
            Dim pError As String
            Dim pInputWithdrawQty As Decimal

            Try
                pInput = JsonConvert.DeserializeObject(Of saveProductBinLocationClass)(Common.WebRequest.InputStreamText)
                If pInput.Verify(pError) = False Then
                    Return JsonDictionary.Serialize("Error", pError)
                End If

                Dim pOrderedAndWithdrawnQty As TransInvoice.OrderedAndWithdrawnQtyClass
                Common.WebRequest.ProData.BeginTransaction()

                pOrderedAndWithdrawnQty = WebRequest.Instance.Business.TransInvoice.OrderedAndWithdrawnQty(pInput.InvoiceID, pInput.ProductID, pInput.SectionID)
                pInputWithdrawQty = pInput.TotalQuantity()
                If pOrderedAndWithdrawnQty.OrderedQty < pOrderedAndWithdrawnQty.WithdrawnQty + pInputWithdrawQty Then
                    Return JsonDictionary.Serialize("Error", "Withdrawn quantity will exceed ordered quantity. Ordered quantity = " & pOrderedAndWithdrawnQty.OrderedQty & "; Quantity withdrawn from other section = " & pOrderedAndWithdrawnQty.WithdrawnQty & "; Quantity you are attempting to withdraw: " & pInputWithdrawQty)
                End If

                For Each pStock As saveProductBinLocationClass.StockClass In pInput.Stocks
                    

                    pSql = "SELECT ID FROM TransInvoiceItemWarehouseWithdrawal " &
                        " WHERE ProductID = " & ReadyString(pInput.ProductID) &
                        " AND InvoiceID = " & ReadyString(pInput.InvoiceID) &
                        " AND FowstBinID = " & ReadyString(pInput.BinID) & vbCrLf &
                        " AND UnitQty = " & pStock.UnitQty & vbCrLf &
                        " AND SectionID = " & ReadyString(pInput.SectionID) '& vbCrLf &
                    '" AND UnitMeasure = " & ReadyString(pUnitMeasure)
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    pIU = New InsertUpdate("TransInvoiceItemWarehouseWithdrawal")
                    pIU("PreparedQuantity") = pStock.QtyResolved
                    pIU("UnitMeasure") = ReadyString(pStock.UnitMeasure)
                    If (pDT.Rows.Count = 0) Then
                        pIU("InvoiceID") = ReadyString(pInput.InvoiceID)
                        pIU("ProductID") = ReadyString(pInput.ProductID)
                        pIU("FowstBinID") = ReadyString(pInput.BinID)
                        pIU("UnitQty") = pStock.UnitQtyResolved
                        pIU("SectionID") = ReadyString(pInput.SectionID)
                        pSql = pIU.GetInsert()
                    Else
                        pSql = pIU.GetUpdate("ID =" & ReadyString(pDT.Rows(0).Item("ID").ToString()))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Next pStock
                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            Finally
                Common.WebRequest.ProData.RollbackTransaction(, True)
            End Try

        End Function

        Private Class saveProductBinLocationClass
            Public BinID As String
            Public FowstID As String
            Public InvoiceID As String
            Public ProductID As String
            Public SectionID As String

            Public Stocks As List(Of StockClass)

            Public Function Verify(ByRef oError As String) As Boolean

                oError = Nothing

                If Stocks Is Nothing Then
                    oError = "No stock data"
                    Return False
                End If

                For Each pStock As StockClass In Stocks
                    If pStock.Verify(oError) = False Then
                        Return False
                    End If
                Next pStock

                Return True

            End Function

            Public Function TotalQuantity() As Decimal

                Dim pOut As Decimal

                If Stocks Is Nothing Then
                    Return 0
                End If

                pOut = (From pStock As StockClass In Stocks
                       Select pStock.QtyResolved * pStock.UnitQtyResolved).Sum()

                'pOut = Stocks.Sum(
                '    Function(vItem As StockClass) As Decimal
                '        Return vItem.QtyResolved * vItem.UnitQtyResolved
                '    End Function)

                Return pOut

            End Function

            Public Class StockClass
                Public PriceLevelID As String
                Public Qty As String
                Public UnitMeasure As String
                Public UnitQty As String

                Public ReadOnly Property QtyResolved As Decimal
                    Get
                        Return Qty
                    End Get
                End Property

                Public ReadOnly Property UnitQtyResolved As Decimal
                    Get
                        Return UnitQty
                    End Get
                End Property

                Public Function Verify(ByRef oError As String) As Boolean

                    oError = Nothing

                    If IsNumeric(Qty) = False Then
                        oError = "Qty must be numeric"
                        Return False
                    End If
                    If QtyResolved < 0 Then
                        oError = "Qty must be equal or greater than zero."
                        Return False
                    End If

                    If IsNumeric(UnitQty) = False Then
                        oError = "UnitQty must be numeric"
                        Return False
                    End If
                    If UnitQtyResolved <= 0 Then
                        oError = "UnitQty must be greater than zero"
                        Return False
                    End If

                    Return True
                End Function
            End Class
        End Class

        <HttpPost()>
        Public Function SaveAsReadyForInvoicing() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pInvoiceID As String

            pInvoiceID = WebRequest.Instance.Data("InvoiceID")
            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Invoice ID")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            Try

                pIU = New InsertUpdate("TransInvoice")
                pIU("IsSOReadyForInvoice") = 1

                pSql = pIU.GetUpdate("ID = " & ReadyString(pInvoiceID))
                Common.WebRequest.ProData.ExecuteSQL(pSql)


                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Public Function CheckIsQtyConfirmed() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pInvoiceID As String
            Dim pProductID As String
            Dim pIsConfirmed As Boolean
            Dim pSortingListCreator As SortingListCreator
            Dim pSectionID As String

            pInvoiceID = WebRequest.Instance.Data("InvoiceID")
            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Invoice ID")
            End If

            pProductID = WebRequest.Instance.Data("ProductID")
            If IsGuid(pProductID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid Product ID")
            End If
            pProductID = ReadyGuid(pProductID)

            pSectionID = WebRequest.Instance.Data("SectionID")
            If IsGuid(pSectionID) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid section ID")
            End If
            pSectionID = ReadyGuid(pSectionID)

            pIsConfirmed = CBool2(WebRequest.Instance.Data("IsConfirmed"))
            Try
                WebRequest.Instance.Business.TransInvoice.TagDoneWithdrawal(
                    vID:=pInvoiceID,
                    vSectionID:=pSectionID,
                    vProductID:=pProductID,
                    vIsDone:=pIsConfirmed
                )
                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Public Function getAlternativeItems() As String
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pDT As DataTable
            Dim pSql As String

            pSql = "SELECT ISNULL(ProductNo, '') + ' ' + ProductName as ProductName  FROM " &
                "  MobizLibraryPIMProduct pim INNER JOIN TransInvoiceItems invitem " &
                " ON pim.ID = invitem.ProductID WHERE RefStockID in (SELECT ID FROM TransInvoiceItems " &
                " WHERE InvoiceID = " & ReadyString(WebRequest.Instance.Data("InvoiceID")) & " AND ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) & ")"

            pDT = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("Data", pDT)

            Return strReturn

        End Function

    End Class
End Namespace