Imports Aspose.Cells

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
            Return PartialView("~/Views/InventoryCount/BsIndex.vbhtml")
            'Return View()

            'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

            'If (Not SecurityRole.Instance.HandleRoleAccess("Inventory Count")) Then Exit Function

        End Function

        Function BsIndex() As ActionResult
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

        Function InventoryHandling()
            Return View()
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

        Function InventoryCountHistoryOffline() As ActionResult
            Return View()
        End Function

        Function ReportInventoryLevel() As ActionResult
            Return View()
        End Function

        Function GetAllFowstBinProduct() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetAllFowstBinProduct())
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

        Function EmptyAbreezaDefaultBin() As ActionResult
            Dim pSql As String
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim pFS As IO.FileStream
            Dim dtaExcelData As DataTable
            Dim dtProducts As DataTable
            Dim dtTemp As DataTable
            Dim InvCount As New InventoryCount

            If WebRequest.Instance.HandleOrganization(True) = False Then Return Nothing

            Dim strHeaders() As String = {"Style"}
            dtaExcelData = New DataTable

            dtaExcelData = ExcelToTable(Common.ServerPath & "temp\AbreezaNikeProductStyle.xlsx", strHeaders, "")

            If (Not dtaExcelData Is Nothing) Then
                Try
                    For i As Integer = 0 To dtaExcelData.Rows.Count
                        pSql = "select ID from Mobiz.dbo.LibraryPIMProduct where ProductGroupNo = " & ReadyString(dtaExcelData.Rows(i).Item("Style"))

                        dtProducts = Common.WebRequest.ProData.Execute(pSql)

                        For j As Integer = 0 To dtProducts.Rows.Count - 1
                            pSql = "SELECT dbo.LibraryFowstBin.ID " &
                                "FROM dbo.LibraryFowstBin INNER JOIN " &
                                "dbo.LibraryFowstBinProduct ON dbo.LibraryFowstBin.FowstID = dbo.LibraryFowstBinProduct.FowstID AND  " &
                                "dbo.LibraryFowstBin.ID = dbo.LibraryFowstBinProduct.BinID " &
                                "WHERE dbo.LibraryFowstBinProduct.ProductID = " & ReadyString(dtProducts.Rows(j).Item("ID").ToString) &
                                " AND  dbo.LibraryFowstBinProduct.FowstID = " & ReadyString("78534471-9B4F-4638-8D8A-970893DA332C") &
                                " AND (dbo.LibraryFowstBin.Name = 'Unnamed Bin' OR " & vbCrLf &
                                " dbo.LibraryFowstBin.Name = 'Stock Room' OR " & vbCrLf &
                                " dbo.LibraryFowstBin.Name = 'Selling Area' OR " & vbCrLf &
                                " dbo.LibraryFowstBin.Name = 'PATTY')"


                            '78534471-9B4F-4638-8D8A-970893DA332C nike abreeza fowst id

                            dtTemp = Common.WebRequest.ProData.Execute(pSql)

                            If dtTemp.Rows.Count > 0 Then
                                For k As Integer = 0 To dtTemp.Rows.Count - 1
                                    WebRequest.Instance.Business.TransInventoryLedger.Add( _
                                        CStr2(dtProducts.Rows(j).Item("ID").ToString), _
                                        CStr2("78534471-9B4F-4638-8D8A-970893DA332C"), _
                                        Guid.NewGuid.ToString, _
                                        TransInventoryLedger.eTransType.InventoryCount, _
                                        WebSession.Instance.PeopleUserID, , _
                                        0, _
                                        dtTemp.Rows(k).Item("ID").ToString,
                                        Nothing
                                    )
                                Next

                            End If

                            'InvCount.AddToInventoryLedger(WebRequest.Instance.Data("ProductID"), dtTemp.Rows(0).Item("ID").ToString, 0, "N/A")
                        Next
                    Next
                Catch ex As Exception

                End Try
            End If
        End Function

        Function DisableItems() As ActionResult
            Dim pSql As String
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim pFS As IO.FileStream
            Dim dtaExcelData As DataTable
            Dim dtProducts As DataTable
            Dim dtTemp As DataTable
            Dim InvCount As New InventoryCount

            If WebRequest.Instance.HandleOrganization(True) = False Then Return Nothing

            Dim strHeaders() As String = {"Style"}
            dtaExcelData = New DataTable

            dtaExcelData = ExcelToTable(Common.ServerPath & "temp\AthleticClubProductStyleToBeDisabled.xlsx", strHeaders, "")

            If (Not dtaExcelData Is Nothing) Then
                Try
                    For i As Integer = 0 To dtaExcelData.Rows.Count
                        pSql = "select ID from Mobiz.dbo.LibraryPIMProduct where ProductGroupNo = " & ReadyString(dtaExcelData.Rows(i).Item("Style"))

                        dtProducts = Common.WebRequest.ProData.Execute(pSql)

                        For j As Integer = 0 To dtProducts.Rows.Count - 1
                            pSql = "Update LibraryOrganizationPIM set " & vbCrLf &
                                    "IsDeactivated = 1,  " & vbCrLf &
                                    "ActivationDate = getdate(),  " & vbCrLf &
                                    "DeactivationReason = 'Old Stocks' " & vbCrLf &
                                    "where ProductID = " & ReadyString(CStr2(dtProducts.Rows(j).Item("ID")))
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Next
                    Next
                Catch ex As Exception

                End Try
            End If
        End Function

        Function EmptySMDefaultBin() As ActionResult
            Dim pSql As String
            Dim pWB As Workbook
            Dim pWS As Worksheet
            Dim pFS As IO.FileStream
            Dim dtaExcelData As DataTable
            Dim dtProducts As DataTable
            Dim dtTemp As DataTable
            Dim InvCount As New InventoryCount

            If WebRequest.Instance.HandleOrganization(True) = False Then Return Nothing

            Dim strHeaders() As String = {"Style"}
            dtaExcelData = New DataTable

            dtaExcelData = ExcelToTable(Common.ServerPath & "temp\SMNikeProductStyle.xlsx", strHeaders, "")

            If (Not dtaExcelData Is Nothing) Then
                Try
                    For i As Integer = 0 To dtaExcelData.Rows.Count
                        pSql = "select ID from Mobiz.dbo.LibraryPIMProduct where ProductGroupNo = " & ReadyString(dtaExcelData.Rows(i).Item("Style"))

                        dtProducts = Common.WebRequest.ProData.Execute(pSql)

                        For j As Integer = 0 To dtProducts.Rows.Count - 1
                            pSql = "SELECT dbo.LibraryFowstBin.ID " &
                                "FROM dbo.LibraryFowstBin INNER JOIN " &
                                "dbo.LibraryFowstBinProduct ON dbo.LibraryFowstBin.FowstID = dbo.LibraryFowstBinProduct.FowstID AND  " &
                                "dbo.LibraryFowstBin.ID = dbo.LibraryFowstBinProduct.BinID " &
                                "WHERE dbo.LibraryFowstBinProduct.ProductID = " & ReadyString(dtProducts.Rows(j).Item("ID").ToString) &
                                " AND  dbo.LibraryFowstBinProduct.FowstID = " & ReadyString("A7ACE5C3-C649-448C-8FFD-825AA19D20FF") &
                                " AND (dbo.LibraryFowstBin.Name = 'Unnamed Bin' OR " & vbCrLf &
                                " dbo.LibraryFowstBin.Name = 'Stock Room' OR " & vbCrLf &
                                " dbo.LibraryFowstBin.Name = 'Selling Area' OR " & vbCrLf &
                                " dbo.LibraryFowstBin.Name = 'PATTY')"

                            'A7ACE5C3-C649-448C-8FFD-825AA19D20FF nike abreeza fowst id

                            dtTemp = Common.WebRequest.ProData.Execute(pSql)

                            If dtTemp.Rows.Count > 0 Then
                                For k As Integer = 0 To dtTemp.Rows.Count - 1
                                    WebRequest.Instance.Business.TransInventoryLedger.Add( _
                                            CStr2(dtProducts.Rows(j).Item("ID").ToString), _
                                            CStr2("A7ACE5C3-C649-448C-8FFD-825AA19D20FF"), _
                                            Guid.NewGuid.ToString, _
                                            TransInventoryLedger.eTransType.InventoryCount, _
                                            WebSession.Instance.PeopleUserID, , _
                                            0, _
                                            dtTemp.Rows(k).Item("ID").ToString,
                                            Nothing
                                        )
                                Next
                            End If

                        Next
                    Next
                Catch ex As Exception

                End Try
            End If
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
                " AND dbo.LibraryFowstBin.Name = 'Unnamed Bin'"

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
            pIU("MaxPerProduct") = CDec2(Common.WebRequest.Data("MaxCap"))
            pIU("MinPerProduct") = CDec2(Common.WebRequest.Data("MinCap"))
            pIU("ReorderQty") = CDec2(Common.WebRequest.Data("ReorderQty"))

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

        Function GetBinBalances() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDT = s.GetBinBalances()
            If pDT.Rows.Count = 0 Then
                Dim pBinID = WebRequest.Instance.Business.SystemDefaults.GetOrAddDefaultBin(WebRequest.Instance.Data("FOWSTID"), WebRequest.Instance.Data("ProductID"))
                WebRequest.Instance.Business.TransInventoryLedger.AddToInventoryLedger("", "", WebRequest.Instance.Data("ProductID"), WebRequest.Instance.Data("FOWSTID"), Guid.NewGuid.ToString(), TransInventoryLedger.eTransType.InventoryCount, modClient.EmployeeWebGuidID, 0)
                s.AddOrSetNewDefaultBin(WebRequest.Instance.Data("ProductID"), WebRequest.Instance.Data("FOWSTID"), pBinID)
                pDT = s.GetBinBalances()
            End If
            Return JsonDictionary.Serialize("Results", pdt)
        End Function

        Function GetInventoryCountForApproval() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetInventoryCountForApproval())
        End Function

        Function GetInventoryCountForApprovalHistory() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetInventoryCountForApprovalHistory())
        End Function

        Function ApprovedInventoryCount() As String
            Dim s As New InventoryCount
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.ApprovedInventoryCount())
        End Function

        Function GetApprovedAndDisapprovedCounts() As String
            Dim pSql As String
            Dim dt As DataTable
            Dim isFirst As Boolean = True

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select ID, IsVarianceApproved, IsVarianceDisApproved from TransInventoryProductCountstock " & vbCrLf &
                   "Where IsActionTaken = 1 AND " & vbCrLf &
                   " ID IN("

            For Each pObj As Object In Common.WebRequest.AllData("ID")

                If isFirst Then
                    pSql += ReadyString(pObj("ID"))
                    isFirst = False
                Else
                    pSql += "," & ReadyString(pObj("ID"))
                End If

            Next

            pSql += ")"

            dt = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", dt)
        End Function

        Function Api()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim fowstids As Object = Common.WebRequest.AllData("LocationID")
                Dim productno As String = Common.WebRequest.Data("ProductNo")
                Dim pSql As String = "SELECT ID, Name FROM MobizLibraryOrganizationFowst WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)

                If fowstids.Length > 0 Then
                    pSql = "SELECT ID, Name FROM MobizLibraryOrganizationFowst WHERE ID IN ('" & Join(fowstids, "','") & "')"
                End If

                fowstids = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each fowst As Object In fowstids
                    pSql = "SELECT LibraryFOWSTBinProduct.ProductID " & vbCrLf &
                            " ,ISNULL(LibraryFOWSTBinProduct.ProductNo,'') AS ProductNo" & vbCrLf &
                            " ,ISNULL(LibraryFOWSTBinProduct.ProductName,'') AS ProductName" & vbCrLf &
                            " --,LibraryFOWSTBinProduct.FowstID" & vbCrLf &
                            " ,SUM(LibraryFOWSTBinProduct.Balance) AS Balance" & vbCrLf &
                            " FROM LibraryFOWSTBinProduct" & vbCrLf &
                            " WHERE ISNULL(IsDeactivated, 0) = 0 " & vbCrLf &
                            " AND FowstID = " & ReadyString(fowst("ID").ToString()) & vbCrLf &
                            " AND ProductID = (SELECT orgpim.ProductID FROM LibraryOrganizationPIM as orgpim" & vbCrLf &
                            "   INNER JOIN MobizLibraryPIMProduct as pim ON orgpim.ProductID = pim.ID" & vbCrLf &
                            "   WHERE pim.ProductNo = " & ReadyString(productno) & ") " & vbCrLf &
                            " GROUP BY LibraryFOWSTBinProduct.ProductID" & vbCrLf &
                            " ,LibraryFOWSTBinProduct.ProductNo" & vbCrLf &
                            " ,LibraryFOWSTBinProduct.ProductName" & vbCrLf &
                            " --,LibraryFOWSTBinProduct.FowstID" & vbCrLf

                    fowst.Add("ProductDetails", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next

                Return Json(New With {Key .data = fowstids})
            Catch ex As Exception
                Return Json(New With {Key .data = ex.Message})
            End Try
        End Function
    End Class
End Namespace