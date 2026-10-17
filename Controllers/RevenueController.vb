Public Class RevenueController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    Function Quotation()
        Return View()
    End Function

    Function SalesReportByCustomerAndSupplierProducts() As String
        Dim revenue As New Revenue

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Return revenue.SalesReportByCustomerAndSupplierProducts()

    End Function

    Function SalesReportByProductMeasurementQuantity() As String
        Dim revenue As New Revenue

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Return revenue.SalesReportByProductMeasurementQuantity()

    End Function

    Function SalesReportByProductQuantityPerMonth() As String
        Dim revenue As New Revenue

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Return revenue.SalesReportByProductQuantityPerMonth()

    End Function

    Function SalesReportByProductBaseQuantity() As String
        Dim revenue As New Revenue

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Return revenue.SalesReportByProductBaseQuantity()

    End Function

    Function InventoryMovementByFOWST() As String
        Dim revenue As New Revenue

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Return revenue.InventoryMovementByFOWST()

    End Function

    Public Function DownloadReport() As ActionResult
        Dim pKey = Common.WebRequest.QueryString("Key")

        If WebRequest.Instance.HandleOrganization(True) = False Then Return View()

        If String.IsNullOrWhiteSpace(pKey) Then
            Return Content(JsonDictionary.Serialize("Error", "Key not found. ref: vbTD8XBG5NFY!"))
        End If

        'Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp\PrintablePdf\CrossTabReport_" & pKey & ".xls")
        Dim pFileName = Common.ServerPath & "temp/RevenueReports/" & WebRequest.Instance.OrgID & "/" & pKey & ".xls"

        Return File(pFileName, "application/xls", pKey & ".xls")

    End Function

    Function GetPriceBulletinDashboard()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String = "SELECT ROW_NUMBER() OVER (ORDER BY bulletin.Name ASC) AS RowNum, bulletin.*, ppol.FirstName + ' ' + ppol.Surname as UpdatedByName " & vbCrLf &
                                    " FROM LibraryPriceBulletin as bulletin " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = bulletin.UpdatedBy "

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Function GetDataPriceBulletin()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim bulletinID As String = Common.WebRequest.Data("BulletinID")
        Dim pOut As New JsonDictionary
        Dim pSql As String = "SELECT ID, Name FROM LibraryPriceBulletin WHERE ID = " & ReadyString(bulletinID)

        pOut.Add("Info", jss.DeserializeObject(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        pSql = "SELECT pim.ID, prod.ID as DbID, prod.DisplayName, pim.ProductName as ProductNameOnly, pim.ProductNo, pim.Brand, 'edit' as State " & vbCrLf &
                ", Retail.Price AS RetailPrice " & vbCrLf &
                ", Retail.LastUpdated AS RetailPriceLastUpdated " & vbCrLf &
                ", WholesalePrice.Price AS WholesalePrice " & vbCrLf &
                ", WholesalePrice.LastUpdated AS WholesalePriceLastUpdated " & vbCrLf &
                " FROM LibraryPriceBulletinProducts as prod " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = prod.ProductID " & vbCrLf &
                " OUTER APPLY (SELECT TOP 1 Price" & vbCrLf &
                "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                " 	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(modClient.RetailPriceID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID = pim.ID" & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                " ) AS Retail    " & vbCrLf &
                " OUTER APPLY (SELECT TOP 1 Price" & vbCrLf &
                "     ,MobizLibraryPIMProductPriceLevel.LastUpdated" & vbCrLf &
                " 	FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
                "     WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.PriceLevelID = " & ReadyString(modClient.WholesalePriceID) & vbCrLf &
                "     AND MobizLibraryPIMProductPriceLevel.ProductID = pim.ID" & vbCrLf &
                "     ORDER BY MobizLibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
                " ) AS WholesalePrice    " & vbCrLf &
                " WHERE prod.BulletinID = " & ReadyString(bulletinID)

        pOut.Add("Products", jss.DeserializeObject(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

        Return pOut.Serialize()
    End Function

    Function GetPriceBulletinGeneralSettings()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pOut As New JsonDictionary

        pOut.Add("ItemsPerPage", WebRequest.Instance.Business.SystemDefaults.PriceBulletinItemsPerPage)
        pOut.Add("PageChangeDelay", WebRequest.Instance.Business.SystemDefaults.PriceBulletinPageChangeDelay)

        Return pOut.Serialize()
    End Function

    Function SavePriceBulletinGeneralSettings()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        WebRequest.Instance.Business.SystemDefaults.PriceBulletinItemsPerPage = Common.WebRequest.Data("ItemsPerPage")
        WebRequest.Instance.Business.SystemDefaults.PriceBulletinPageChangeDelay = Common.WebRequest.Data("PageChangeDelay")

        Return JsonDictionary.Serialize("Result", "saved")
    End Function

    Function GetPriceBulletinList()
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim pSql As String
            Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
            Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
            Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
            Dim whereSearchContent As String = ""
            Dim operators As String = ""
            Dim arr As Object = Split(keyword, "+")

            Dim mainquery As String = "SELECT ROW_NUMBER() OVER (ORDER BY bulletin.Name ASC) AS RowNum, bulletin.*, ppol.FirstName + ' ' + ppol.Surname as UpdatedByName " & vbCrLf &
                                    " FROM LibraryPriceBulletin as bulletin " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = bulletin.UpdatedBy "

            If CStr2(keyword) <> "" Then
                mainquery += " WHERE ("
                For i As Integer = 0 To arr.length - 1
                    If whereSearchContent <> "" Then
                        operators = " AND "
                    End If

                    whereSearchContent = whereSearchContent & operators & vbCrLf &
                         " ISNULL(bulletin.Name, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                Next

                mainquery += whereSearchContent & ") "
            End If

            pSql = "SELECT * FROM (" & vbCrLf &
                    mainquery & vbCrLf &
                    " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                    " ORDER BY Name ASC"

            Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function SavePriceBulletin()
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            Dim BulletinID As String = CStr2(Common.WebRequest.Data("BulletinID"))
            Dim BulletinName As String = CStr2(Common.WebRequest.Data("BulletinName"))
            Dim Products As Object = Common.WebRequest.AllData("Products")
            Dim p As New InsertUpdate("LibraryPriceBulletin")
            Dim pSql As String

            p("Name") = ReadyString(BulletinName)
            p("LastUpdated") = "getdate()"
            p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("IsActive") = 1

            If String.IsNullOrWhiteSpace(BulletinID) Then
                BulletinID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(BulletinID)
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" ID = " & ReadyString(BulletinID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.Execute(pSql)

            For Each prod As Object In Products
                p = New InsertUpdate("LibraryPriceBulletinProducts")

                p("BulletinID") = ReadyString(BulletinID)
                p("ProductID") = ReadyString(prod("ID"))
                p("DisplayName") = ReadyString(prod("DisplayName"))

                If prod("State") = "new" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                ElseIf prod("State") = "edit" Then
                    pSql = p.GetUpdate(" ID = " & ReadyString(prod("DbID")))
                Else
                    pSql = "DELETE FROM LibraryPriceBulletinProducts WHERE ProductID = " & ReadyString(prod("ID")) & " AND BulletinID = " & ReadyString(BulletinID)
                End If

                Common.WebRequest.ProData.Execute(pSql)
            Next prod

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "saved")
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try
    End Function
End Class
