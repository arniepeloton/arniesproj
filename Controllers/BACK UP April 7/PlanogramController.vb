Namespace WebProj
    Public Class PlanogramController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Planogram

        Function Index() As ActionResult
            If WebSession.Instance.UserAccess Is Nothing _
                OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView = False Then

                WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PurchaseOrder_PurchaseOrder, ErpAccess.eAccessMode.IsView)
            ElseIf WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView Then
                Return View()
            End If
        End Function

        <HttpPost()>
        Public Function get_Branch() As String
            Dim pOut As JsonNode
            Dim pDT As DataTable
            Dim pDR As DataRow
            Dim pUnitID As String

            Try
                If WebSession.Instance.UserAccess Is Nothing _
                OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView = False Then

                    Return JsonNode.Serialize("Error", "Access denied")
                End If

                pUnitID = Common.WebRequest.Data("ID")
                If IsGuid(pUnitID) = False Then Return JsonNode.Serialize("Error", "Not a valid branch")
                pUnitID = ReadyGuid(pUnitID)

                pOut = New JsonNode
                pDT = WebRequest.Instance.Business.TransPlanogram.GetParams(pUnitID)
                If pDT.Rows.Count > 0 Then
                    pDR = pDT.Rows(0)
                    With pOut
                        .Add("ID", CStr2(pDR("ID")))
                        .Add("NoOfAisles", CStr2(pDR("NoOfAisles")))
                        .Add("TotalOfAisles", CStr2(pDR("TotalOfAisles")))
                        .Add("Sections", CStr2(pDR("Sections")))
                        .Add("Shelves", CStr2(pDR("Shelves")))
                        .Add("EndCaps", CStr2(pDR("EndCaps")))
                        .Add("SafetyStocks", CStr2(pDR("SafetyStocks")))
                        .Add("Minimums", CStr2(pDR("Minimums")))
                        .Add("Maximums", CStr2(pDR("Maximums")))
                        .Add("NiceCaps", CStr2(pDR("NiceCaps")))
                    End With
                Else
                    Return JsonNode.Serialize("Error", "branch does not exist")
                End If

                Return pOut.Serialize
            Catch ex As System.Exception
                Return JsonNode.Serialize("Error", ex.Message)
            End Try
        End Function

        <HttpPost()>
        Public Function get_Item() As String

            Dim pOut As JsonNode
            Dim pDT As DataTable
            Dim pDR As DataRow
            Dim pUnitID As String

            Try
                If WebSession.Instance.UserAccess Is Nothing _
                OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView = False Then

                    Return JsonNode.Serialize("Error", "Access denied")
                End If

                pUnitID = Common.WebRequest.Data("ID")
                If IsGuid(pUnitID) = False Then Return JsonNode.Serialize("Error", "Not a valid item")
                pUnitID = ReadyGuid(pUnitID)

                pOut = New JsonNode
                pDT = WebRequest.Instance.Business.LibraryStockMeasurements.GetParams(pUnitID)
                If pDT.Rows.Count > 0 Then
                    pDR = pDT.Rows(0)
                    pOut.Add("UnitMeasure", pDT)
                Else
                    Return JsonNode.Serialize("Error", "item does not exist")
                End If

                Return pOut.Serialize
            Catch ex As System.Exception
                Return JsonNode.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Public Function Save() As String

            Dim pIU As InsertUpdate
            Dim str_SQL As String
            Dim str_BranchID As String

            Try
                If WebSession.Instance.UserAccess Is Nothing _
                OrElse WebSession.Instance.UserAccess.PurchaseOrder_PurchaseOrder.IsView = False Then

                    Return JsonNode.Serialize("Error", "Access denied")
                End If

                pIU = New InsertUpdate
                With pIU
                    .TableName = "TransPlanogram"
                    .Value("NoOfAisles") = Common.WebRequest.Data("cmb_Aisles")
                    .Value("TotalOfAisles") = Common.WebRequest.Data("cmb_Store")
                    .Value("Sections") = Common.WebRequest.Data("cmb_Section")
                    .Value("Shelves") = Common.WebRequest.Data("cmb_Shelf")
                    .Value("EndCaps") = Common.WebRequest.Data("cmb_EndCap")
                    .Value("SafetyStocks") = Common.WebRequest.Data("cmb_Safety")
                    .Value("Minimums") = Common.WebRequest.Data("cmb_Minimum")
                    .Value("Maximums") = Common.WebRequest.Data("cmb_Maximum")
                    .Value("NiceCaps") = Common.WebRequest.Data("cmb_NiceCap")
                    .Value("BranchID") = Common.WebRequest.Data("ID")
                    .Value("UserID") = WebSession.Instance.EmployeeID
                    str_SQL = .GetInsert()
                End With
                Common.WebRequest.ProData.ExecuteSQL(str_SQL)

                Return JsonNode.SerializeText("Ok")
            Catch ex As System.Exception
                Return JsonNode.Serialize("Error", ex.Message)
            End Try

        End Function
    End Class
End Namespace