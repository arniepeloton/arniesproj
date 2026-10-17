Public Class ShelfAndBinLabelingController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
        '    If WebRequest.Instance.HandleAccess = False Then Exit Function
        'End If
        Return PartialView("~/Views/ShelfAndBinLabeling/BsIndex.vbhtml")
        'Return View()

    End Function

    Function BsIndex() As ActionResult

        'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
        '    If WebRequest.Instance.HandleAccess = False Then Exit Function
        'End If

        Return View()

    End Function

    Function MissingBarcodesSummary() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function MissingBarcodes() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function GetAllMissingBarcodes() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        clsShelf.OrgID = CStr2(Common.WebRequest.Data("OrgID"))

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetAllMissingBarcodes())

        Return strReturn
    End Function

    Function GetNumberOfMissingBarcodes() As String
        Dim pSql As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSql = "select DefaultValue from systemdefaults where Description = 'Missing Barcode Count'"

        Return Common.WebRequest.ProData.ExecuteScalar(pSql)

    End Function

    Function AddShelfAndBinLabel() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function CreateBarcodeTags() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function GeneratePDF() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function GenerateBarcodeLabels() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function GeneratePriceTagsPDF() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function CreateShelfPriceTags() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function DownloadShelfPriceTags() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function GetData() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        clsShelf.BinID = CStr2(Common.WebRequest.Data("BinID"))
        clsShelf.FowstID = CStr2(Common.WebRequest.Data("FowstID"))
        clsShelf.OrgID = CStr2(Common.WebRequest.Data("OrgID"))
        clsShelf.PriceLevelID = CStr2(Common.WebRequest.Data("PriceLevelID"))
        clsShelf.AreaID = CStr2(Common.WebRequest.Data("AreaID"))
        clsShelf.FilterDisplay = CStr2(Common.WebRequest.Data("FilterDisplay"))
        clsShelf.IsProductBarcode = CBool2(Common.WebRequest.Data("IsProductBarcode"))

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetData())

        Return strReturn

    End Function

    Function GetAllProductsWithOrWithoutBins() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        clsShelf.OrgID = CStr2(Common.WebRequest.Data("OrgID"))

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetAllProductsWithOrWithoutBins())

        Return strReturn
    End Function

    Function GetAllProductsWithPriceUpdates() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        clsShelf.BinID = CStr2(Common.WebRequest.Data("BinID"))
        clsShelf.FowstID = CStr2(Common.WebRequest.Data("FowstID"))
        clsShelf.OrgID = CStr2(Common.WebRequest.Data("OrgID"))
        clsShelf.PriceLevelID = CStr2(Common.WebRequest.Data("PriceLevelID"))
        clsShelf.AreaID = CStr2(Common.WebRequest.Data("AreaID"))
        clsShelf.FilterDisplay = CStr2(Common.WebRequest.Data("FilterDisplay"))
        clsShelf.IsProductBarcode = CBool2(Common.WebRequest.Data("IsProductBarcode"))

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetAllProductsWithPriceUpdates())

        Return strReturn

    End Function

    Function GetFowstArea() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling

        clsShelf.FowstID = CStr2(Common.WebRequest.Data("FowstID"))

        Return JsonDictionary.Serialize("Results", clsShelf.GetFowstArea())
    End Function

    Function GetShelfPriceTagsData() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        clsShelf.BinID = Common.WebRequest.Data("BinID")
        clsShelf.FowstID = Common.WebRequest.Data("FowstID")

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetShelfPriceTagsData())

        Return strReturn
    End Function

    Function GetBinLocationData() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        clsShelf.FowstID = Common.WebRequest.Data("FowstID")

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetBinLocationData())

        Return strReturn
    End Function

    Dim hasNewItem As Boolean = False
    Dim isFinishSaveNewItem As Boolean = False
    Dim prevTransNum As Integer
    Dim prevTransID As String

    Function GetProductBarcodeData() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim p As New InsertUpdate("LibraryFowstProductBarcode")
        Dim strReturn As String
        Dim StrData As String()
        Dim arrayBin(7) As Object
        Dim counted As Integer
        Dim pSql As String
        Dim pDT As DataTable
        Dim transNum As Integer
        Dim strPrice As String
        Dim strNull As String
        Dim transID As String = Guid.NewGuid.ToString()
        Dim item As Object

        pSql = "SELECT TransactionID, TransactionNum FROM LibraryFowstProductBarcode ORDER BY TransactionNum DESC "
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If pDT.Rows.Count = 0 Then
            transNum = 1
        Else
            transNum = Convert.ToInt32(CStr2(pDT.Rows(0).Item("TransactionNum"))) + 1
        End If

        If hasNewItem = False Then
            item = Common.WebRequest.AllData("Cart")
        Else
            item = Common.WebRequest.AllData("NewItem")
            isFinishSaveNewItem = True
            transNum = Convert.ToInt32(CStr2(pDT.Rows(0).Item("TransactionNum")))
            transID = CStr2(pDT.Rows(0).Item("TransactionID"))
        End If

        For Each pObj As Object In item
            counted = 0
            For Each datum As Object In pObj
                arrayBin(counted) = datum
                counted = counted + 1
            Next
            StrData = Split(arrayBin(0).ToString, ",")
            p("ID") = ReadyString(StrData(1).Replace("]", "").Trim)
            StrData = Split(arrayBin(1).ToString, ",")
            p("SkuDesc") = ReadyString(StrData(1).Replace("]", "").Trim)
            StrData = Split(arrayBin(2).ToString, ",")
            p("UoM") = ReadyString(StrData(1).Replace("]", "").Trim)
            StrData = Split(arrayBin(3).ToString, ",")
            p("NumLabels") = ReadyString(StrData(1).Replace("]", "").Trim)
            StrData = Split(arrayBin(4).ToString, ",")
            p("Barcode") = ReadyString(StrData(1).Replace("]", "").Trim)
            p("OrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
            StrData = Split(arrayBin(5).ToString, ",")
            strPrice = StrData(1).Replace("]", "").Trim
            If String.IsNullOrEmpty(strPrice) Then
                p("Price") = "NULL"
            Else
                p("Price") = ReadyString(StrData(1).Replace("]", "").Trim)
            End If
            p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "GetDate()"

            StrData = Split(arrayBin(6).ToString, ",")
            If CBool(StrData(1).Replace("]", "").Trim) Then
                If hasNewItem = False Then
                    'If lookup then just update the details
                    pSql = p.GetUpdate(" TransactionID = " & ReadyString(Split(arrayBin(7).ToString, ",")(1).Replace("]", "").Trim) & " AND SkuDesc = " & ReadyString(Split(arrayBin(1).ToString, ",")(1).Replace("]", "").Trim))
                Else
                    p("TransactionNum") = ReadyString(transNum)
                    p("TransactionID") = ReadyString(transID)
                    pSql = p.GetInsert() 'Insert new item in current trans
                End If
            Else
                p("TransactionNum") = ReadyString(transNum)
                p("TransactionID") = ReadyString(transID)
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)
        Next

        Try
            If Common.WebRequest.AllData("NewItem") IsNot Nothing And isFinishSaveNewItem = False Then
                hasNewItem = True
                isFinishSaveNewItem = True
                GetProductBarcodeData()
                transNum = transNum - 1
            End If
        Catch ex As Exception

        End Try

        clsShelf.TransNum = transNum

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetProductBarcodeData())

        hasNewItem = False
        isFinishSaveNewItem = False

        Return strReturn
    End Function

    Function LookupBarcodeData() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim p As New InsertUpdate("LibraryFowstProductBarcode")
        Dim strReturn As String

        clsShelf.TransNum = Common.WebRequest.Data("TransNum")

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetProductBarcodeData())

        Return strReturn
    End Function

    Function GetProductMeasurement() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        clsShelf.ProductID = Common.WebRequest.Data("ProductID")

        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetProductMeasurement())

        Return strReturn
    End Function

    Function GetMeasurementBarcode() As Object
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String
        Dim uomData As Object = Common.WebRequest.AllData("uomData")
        Dim mea(uomData.length - 1) As Object

        clsShelf.uomData = Common.WebRequest.AllData("uomData")

        For i As Integer = 0 To uomData.length - 1
            clsShelf.UoMID = uomData(i)("ID")
            Dim pDt As DataTable = clsShelf.GetMeasurementBarcode()
            Dim barcode(pDt.Rows.Count - 1) As Object

            For m As Integer = 0 To pDt.Rows.Count - 1
                barcode(m) = New With {Key .ID = pDt.Rows(m)("ID"),
                                 .Barcode = pDt.Rows(m)("Barcode").ToString(),
                                 .StockMeasurementsID = pDt.Rows(m)("StockMeasurementsID")
                                }
            Next
            mea(i) = barcode
        Next

        Return Json(New With {Key .Results = mea})
    End Function

    Function GetPriceLevel() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim clsShelf As New ShelfAndBinLabeling
        Dim strReturn As String

        'clsShelf.ProductID = Common.WebRequest.Data("ProductID")
        clsShelf.OrgID = Common.WebRequest.Data("OrgID")
        strReturn = JsonDictionary.Serialize("Results", clsShelf.GetPriceLevel())

        Return strReturn
    End Function

    Function GetLibraryFOWSTBin() As String
        Dim pSql As String
        Dim dtTemp As DataTable

        If WebRequest.Instance.HandleOrganization(True) = False Then Return Nothing

        pSql = "Select * from LibraryFOWSTBin " & vbCrLf &
                "WHERE FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) & vbCrLf &
                " AND Name <> 'Unnamed Bin' AND Name <> 'Default Bin'"


        dtTemp = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", dtTemp)
    End Function

    Function Lookup() As Object
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim shelf As New ShelfAndBinLabeling
        Dim clsCRMDashboard As New CRMDashboard
        Dim pDT As DataTable
        Dim ctr As Integer = 0

        shelf.PageStart = CInt(Request.Form.GetValues("start")(0))
        shelf.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
        shelf.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
        shelf.IsCountPage = False
        shelf.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
        shelf.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()

        pDT = shelf.Lookup()

        Dim a(pDT.Rows.Count - 1) As Object

        For m As Integer = 0 To pDT.Rows.Count - 1
            a(m) = New With {Key .ID = pDT.Rows(m)("TransactionID"),
                             .NumLabels = pDT.Rows(m)("NumLabels").ToString(),
                             .CreatedBy = pDT.Rows(m)("CreatedBy"),
                             .LastUpdated = pDT.Rows(m)("LastUpdated"),
                             .TransactionNum = pDT.Rows(m)("TransactionNum").ToString()}
            ctr = ctr + 1
        Next

        shelf.IsCountPage = True
        Dim pDTCountPage As DataTable = shelf.Lookup()

        Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
    End Function

    Function GetProductList() As Object
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim shelf As New ShelfAndBinLabeling
        Dim clsCRMDashboard As New CRMDashboard
        Dim pDT As DataTable
        Dim ctr As Integer = 0

        shelf.PageStart = CInt(Request.Form.GetValues("start")(0))
        shelf.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
        shelf.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
        shelf.IsCountPage = False
        shelf.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
        shelf.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()

        pDT = shelf.GetProductList()

        Dim a(pDT.Rows.Count - 1) As Object

        For m As Integer = 0 To pDT.Rows.Count - 1
            a(m) = New With {Key .ID = pDT.Rows(m)("ID"),
                             .ProductGroupNo = pDT.Rows(m)("ProductGroupNo").ToString(),
                             .ProductNo = pDT.Rows(m)("ProductNo").ToString(),
                             .ProductName = pDT.Rows(m)("ProductName"),
                             .RetailPrice = pDT.Rows(m)("RetailPrice").ToString()
                            }
            ctr = ctr + 1
        Next

        shelf.IsCountPage = True
        Dim pDTCountPage As DataTable = shelf.GetProductList()

        Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
    End Function

    Function RemoveProductFromBin() As String

        Dim pSql As String
        Dim dtTemp As DataTable

        If WebRequest.Instance.HandleOrganization(True) = False Then Return Nothing

        pSql = "Update LibraryFowstBinProduct set IsActive = 0 " &
                "where ID=" & ReadyString(Common.WebRequest.Data("ID"))


        Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", True)

    End Function

    Function UpdateItemPrintDate() As String
        Dim pSql As String
        Dim dtSPTCount As DataTable

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        For Each pObj As Object In Common.WebRequest.AllData("BarcodeTags")

            pSql = "Select ID from LibraryPIMPriceTagsLastPrintedDate " & vbCrLf &
                    " WHERE FowstID = " & ReadyString(CStr2(Common.WebRequest.Data("FowstID"))) & vbCrLf &
                    " AND ProductID = " & ReadyString(CStr2(pObj("ID"))) & vbCrLf &
                    " AND BinID = " & ReadyString(CStr2(pObj("BinID")))

            dtSPTCount = Common.WebRequest.ProData.Execute(pSql)

            If dtSPTCount.Rows.Count > 0 Then
                pSql = "Update LibraryPIMPriceTagsLastPrintedDate set LastPriceAdjustmentDate = " & ReadyString(CStr2(pObj("LastUpdated"))) & " where ID = " & ReadyString(CStr2(dtSPTCount.Rows(0).Item("ID")))
            Else
                pSql = "Insert Into LibraryPIMPriceTagsLastPrintedDate(FowstID, ProductID, BinID, LastPriceAdjustmentDate, PrintedBy) values(" & vbCrLf &
                        ReadyString(CStr2(Common.WebRequest.Data("FowstID"))) & "," & vbCrLf &
                        ReadyString(CStr2(pObj("ID"))) & "," & vbCrLf &
                        ReadyString(CStr2(pObj("BinID"))) & "," & vbCrLf &
                        ReadyString(CStr2(pObj("LastUpdated"))) & "," & vbCrLf &
                        ReadyString(CStr2(WebSession.Instance.PeopleUserID)) & ")"
            End If

            Common.WebRequest.ProData.Execute(pSql)
        Next
    End Function

    Function UpdateSPTPrintDate() As String
        Dim pSql As String
        Dim dtSPTCount As DataTable

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        For Each pObj As Object In Common.WebRequest.AllData("SPT")

            pSql = "Select ID from LibraryPriceTagsLastPrintedDate " & vbCrLf &
                    " WHERE FowstID = " & ReadyString(CStr2(Common.WebRequest.Data("FowstID"))) & vbCrLf &
                    " AND ProductID = " & ReadyString(CStr2(pObj("ID"))) & vbCrLf &
                    " AND PriceLevelID = " & ReadyString(CStr2(Common.WebRequest.Data("PriceLevelID"))) & vbCrLf &
                    " AND BinID = " & ReadyString(CStr2(pObj("BinID")))

            dtSPTCount = Common.WebRequest.ProData.Execute(pSql)

            If dtSPTCount.Rows.Count > 0 Then
                pSql = "Update LibraryPriceTagsLastPrintedDate set LastPrintedDate=GETDATE(), LastPriceAdjustmentDate = " & ReadyString(CStr2(pObj("LastUpdated"))) & " where ID = " & ReadyString(CStr2(dtSPTCount.Rows(0).Item("ID")))
            Else
                pSql = "Insert Into LibraryPriceTagsLastPrintedDate(FowstID, ProductID, BinID, PriceLevelID, LastPrintedDate, LastPriceAdjustmentDate, PrintedBy) values(" & vbCrLf &
                        ReadyString(CStr2(Common.WebRequest.Data("FowstID"))) & "," & vbCrLf &
                        ReadyString(CStr2(pObj("ID"))) & "," & vbCrLf &
                        ReadyString(CStr2(pObj("BinID"))) & "," & vbCrLf &
                        ReadyString(CStr2(Common.WebRequest.Data("PriceLevelID"))) & "," & vbCrLf &
                        "GETDATE()" & "," & vbCrLf &
                        ReadyString(CStr2(pObj("LastUpdated"))) & "," & vbCrLf &
                        ReadyString(CStr2(WebSession.Instance.PeopleUserID)) & ")"
            End If

            Common.WebRequest.ProData.Execute(pSql)
        Next

    End Function

    Function DeleteProductsFromShelf() As String
        Dim pSql As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        For Each pObj As Object In Common.WebRequest.AllData("Products")

            pSql = "Delete from LibraryFOWSTBinProduct WHERE FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("FOWSTID"))) &
                    " AND BinID = " & ReadyString(CStr2(pObj("BinID"))) & vbCrLf &
                    " AND ProductID = " & ReadyString(CStr2(pObj("ID"))) & vbCrLf

            Common.WebRequest.ProData.Execute(pSql)
        Next
    End Function
End Class