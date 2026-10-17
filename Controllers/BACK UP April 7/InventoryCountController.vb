Namespace WebProj
    Public Class InventoryCountController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /InventoryCount

        

        Function Index() As ActionResult
            'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()
            'If (Not SecurityRole.Instance.HandleRoleAccess("Inventory Count")) Then Exit Function
            'Return View()

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

            'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

            'If (Not SecurityRole.Instance.HandleRoleAccess("Inventory Count")) Then Exit Function

        End Function

        <HttpPost()>
        Function GetAllBrands() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("InventoryCount", s.GetAllBrands())
        End Function

        <HttpPost()>
        Function GetAllProducts() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("InventoryCount", s.GetAllProducts())
        End Function

        <HttpPost()>
        Function GetAllBinStock() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("BinStock", s.GetAllBinStock())
        End Function

        <HttpPost()>
        Function SaveBinName() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Save", s.SaveBinName())
        End Function

        <HttpPost()>
        Function SaveInventoryCount() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Save", s.SaveInventoryCount(WebRequest.Instance.Data("ID"), CBool(WebRequest.Instance.Data("IsNew"))))

        End Function

        <HttpPost()>
        Function GetOrganizationID()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("OrganizationID", WebSession.Instance.SubscriberOrganizationID)
        End Function

        <HttpPost()>
        Function SetFowstID()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            WebSession.Instance.FowstID = WebRequest.Instance.Data("FowstID")
            Return JsonDictionary.Serialize("Fowst", "True")
        End Function

        <HttpPost()>
        Function SetProductID()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            WebSession.Instance.PIMProductID = WebRequest.Instance.Data("ProductID")
            Return JsonDictionary.Serialize("Fowst", "True")
        End Function

        <HttpPost()>
        Function GetAllInventoryCount() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("InventoryCount", s.GetAllInventoryCount())
        End Function

        <HttpPost()>
        Function GetInventoryCountStocks() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("InventoryCount", s.GetInventoryCountStocks())
        End Function

        <HttpPost()>
        Function AddBin() As ActionResult
            Return View()
        End Function

        Function Report() As ActionResult
            Return View()
        End Function

        Function ReportLedger() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function GetInventoryCountReport() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("InventoryCountReport", s.GetInventoryCountReport())
        End Function

        <HttpPost()>
        Function GetInventoryCountReportByProduct() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("InventoryCountReport", s.GetInventoryCountReportByProduct())
        End Function

        <HttpPost()>
        Function GetCountPIMProductGroupNo() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("InventoryCount", s.GetCountPIMProductGroupNo())
        End Function

        <HttpPost()>
        Function EmptyDefaultBin() As String


            Dim s As New InventoryCount
            Dim pSql As String
            Dim dt As New DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT dbo.LibraryFowstBin.ID " &
                "FROM dbo.LibraryFowstBin INNER JOIN " &
                "dbo.LibraryFowstBinProduct ON dbo.LibraryFowstBin.FowstID = dbo.LibraryFowstBinProduct.FowstID AND  " &
                "dbo.LibraryFowstBin.ID = dbo.LibraryFowstBinProduct.BinID " &
                "WHERE dbo.LibraryFowstBinProduct.ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) &
                " AND  dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) &
                " AND dbo.LibraryFowstBin.Name = 'Default Bin'"

            dt = Common.WebRequest.ProData.Execute(pSql)

            s.AddToInventoryLedger(WebRequest.Instance.Data("ProductID"), dt.Rows(0).Item("ID").ToString, 0, "N/A")
        End Function


        Function UpdateFowstBinProductMaxMinCap() As String
            Dim pSql As String
            Dim dt As DataTable
            Dim pIU As InsertUpdate
            Dim IsUpdate As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select ID from LibraryFowstBinProduct " & vbCrLf &
                    "Where FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                    " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
                    " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))

            dt = Common.WebRequest.ProData.Execute(pSql)

            If dt.Rows.Count > 0 Then
                IsUpdate = True
            Else
                IsUpdate = False
            End If

            pIU = New InsertUpdate("LibraryFowstBinProduct")
            pIU("FowstID") = ReadyString(Common.WebRequest.Data("FowstID"))
            pIU("BinID") = ReadyString(Common.WebRequest.Data("BinID"))
            pIU("ProductID") = ReadyString(Common.WebRequest.Data("ProductID"))
            pIU("MaxPerProduct") = Common.WebRequest.Data("MaxCap")
            pIU("MinPerProduct") = Common.WebRequest.Data("MinCap")

            If IsUpdate Then
                pSql = pIU.GetUpdate("ID = " & ReadyString(CStr2(dt.Rows(0).Item("ID"))))
            Else
                pSql = pIU.GetInsert()
            End If

            'mProData.ExecuteSQL(pSql)

            'If dt.Rows.Count > 0 Then
            '    pSql = "Update LibraryFowstBinProduct set " & vbCrLf &
            '        "MaxPerProduct = " & Common.WebRequest.Data("MaxCap") & vbCrLf &
            '        ", MinPerProduct = " & Common.WebRequest.Data("MinCap") & vbCrLf &
            '        "Where FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
            '        " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
            '        " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))
            'Else
            '    pSql = "Insert into LibraryFowstBinProduct (FowstID) set " & vbCrLf &
            '        "MaxPerProduct = " & Common.WebRequest.Data("MaxCap") & vbCrLf &
            '        ", MinPerProduct = " & Common.WebRequest.Data("MinCap") & vbCrLf &
            '        "Where  = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
            '        " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
            '        " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))
            'End If
            

            Try
                Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "Error")
            End Try

            Return JsonDictionary.Serialize("Results", "Success")

        End Function
    End Class
End Namespace