Public Class SupplierStockController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        WebRequest.Instance.HandleWebKey()
        If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PurchaseOrder_PurchaseOrder, ErpAccess.eAccessMode.IsEnable) = False Then Exit Function

        Return View()

    End Function

    <HttpPost()>
    Public Function GetData() As String

        Dim pSupplierID As String
        Dim pStockID As String
        Dim pOut As JsonNode
        Dim pDT As DataTable
        Dim pDR As DataRow

        Try
            If WebSession.Instance.UserAccess Is Nothing _
            OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView = False Then

                Return JsonNode.Serialize("Error", "Access denied")
            End If

            pOut = New JsonNode

            pSupplierID = Request.QueryString("supplierid")
            If IsGuid(pSupplierID) = False Then Return JsonNode.Serialize("Error", "Not a valid supplier")
            pSupplierID = ReadyGuid(pSupplierID)

            pStockID = Request.QueryString("stockid")
            If IsGuid(pStockID) = False Then Return JsonNode.Serialize("Error", "Not a valid stock")
            pStockID = ReadyGuid(pStockID)

            pDR = WebRequest.Instance.Business.LibrarySuppliers.ById(pSupplierID)
            If pDR Is Nothing Then
                Return JsonNode.Serialize("Error", "Supplier does not exist")
            End If
            pOut.Add("SupplierName", pDR("SupplierName"))

            pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pStockID))
            If pDT.Rows.Count > 0 Then
                pDR = pDT.Rows(0)
                pOut.Add("StockNumber", pDR("StockNumber"))
                pOut.Add("StockDescription", pDR("StockDescription"))
                pOut.Add("Units", WebRequest.Instance.Business.LibraryStock.GetUnits(ReadyString(pStockID)))

                pDT = WebRequest.Instance.Business.LibrarySuppliers.GetStockParams(pSupplierID, pStockID)
                If pDT.Rows.Count > 0 Then
                    pDR = pDT.Rows(0)
                    pOut.Add("LeadTime", CStr2(pDR("LeadTime")))
                    pOut.Add("MinimumOrderQty", CStr2(pDR("MinimumOrderQty")))
                    pOut.Add("LotSize", CStr2(pDR("LotSize")))
                    pOut.Add("MinimumOrderUnit", CStr2(pDR("MinimumOrderUnit")))
                    pOut.Add("LotSizeUnit", CStr2(pDR("LotSizeUnit")))
                End If
            Else
                Return JsonNode.Serialize("Error", "Stock does not exist")
            End If

            Return pOut.Serialize
        Catch ex As System.Exception
            Return JsonNode.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Public Function Save() As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSupplierID As String
        Dim pStockID As String

        Try
            If WebSession.Instance.UserAccess Is Nothing _
            OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView = False Then

                Return JsonNode.Serialize("Error", "Access denied")
            End If

            pSupplierID = ReadyGuid(Common.WebRequest.Data("SupplierID"))
            pStockID = ReadyGuid(Common.WebRequest.Data("StockID"))

            pIU = New InsertUpdate("LibrarySupplierStocks")
            pIU("LastUpdated") = "GETDATE()"
            pIU("UserID") = WebSession.Instance.EmployeeID
            pIU("LeadTime") = Common.WebRequest.Data("txtLeadTime")
            pIU("LotSize") = Common.WebRequest.Data("txtLotSize")
            pIU("LotSizeUnit") = ReadyString(Common.WebRequest.Data("cbLotSize"))
            pIU("MinimumOrderQty") = Common.WebRequest.Data("txtMoq")
            pIU("MinimumOrderUnit") = ReadyString(Common.WebRequest.Data("cbMoq"))
            pSql = pIU.GetUpdate("SupplierID = " & ReadyString(pSupplierID) & " AND StockID = " & ReadyString(pStockID))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonNode.SerializeText("Ok")
        Catch ex As System.Exception
            Return JsonNode.Serialize("Error", ex.Message)
        End Try

    End Function
End Class
