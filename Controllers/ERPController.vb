Option Strict Off

Imports Newtonsoft.Json
Imports System.Linq
Imports Stimulsoft.Report.Components

Namespace WebProj
    Public Class ERPController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function BsIndex() As ActionResult
            Return View()
        End Function

        Function Quotation() As ActionResult
            Return View()
        End Function

        Function OrderProcessing() As ActionResult
            Return View()
        End Function

        Function Invoicing() As ActionResult
            Return View()
        End Function

        Function ExpenseTracking() As ActionResult
            Return View()
        End Function

        Function FulfillmentCenter() As ActionResult
            Return View()
        End Function

        Function GPSDeviceTracker() As ActionResult
            Return PartialView("~/Views/DashboardsAndReports/Device.vbhtml")
        End Function

        Function DeliveryTracking() As ActionResult
            Return View()
        End Function

        Function Deposits() As ActionResult
            Return View()
        End Function

        Function Consignee() As ActionResult
            Return View()
        End Function

        Function PriceCanvass() As ActionResult
            Return View()
        End Function

        Function StockTransfer() As ActionResult
            Return View()
        End Function

        Function StockReceiving() As ActionResult
            Return View()
        End Function

        Function Revenue() As ActionResult
            Return View()
        End Function

        Function Crm() As ActionResult
            Return View()
        End Function

        Function Disbursement() As ActionResult
            Return View()
        End Function

        Function ProjectManagement() As ActionResult
            Return View()
        End Function

        Function DTR() As ActionResult
            Return View()
        End Function

        Function GetQuickRemarks()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim whereSearchContent As String = ""
                Dim operators As String = ""
                Dim arr As Object = Split(keyword, "+")
                Dim show As String = Request.Params.Get("Show")
                Dim IsInternalRemarks As String = Request.Params.Get("IsInternalRemarks")

                Dim mainquery As String = "SELECT ROW_NUMBER() OVER (ORDER BY qr.LastUpdated ASC) AS RowNum, qr.*, ppol.FirstName + ' ' + ppol.Surname as UpdatedByName " & vbCrLf &
                        " FROM LibraryQuickRemarks as qr " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = qr.UpdatedBy " & vbCrLf

                If CStr2(show) = "" Then
                    mainquery += " WHERE Tag IS NOT NULL " & vbCrLf
                Else
                    mainquery += " WHERE Tag = " & ReadyString(show) & vbCrLf
                End If

                If CStr2(keyword) <> "" Then
                    mainquery += " AND ("
                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " ISNULL(qr.Message, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next

                    mainquery += whereSearchContent & ") "
                End If

                mainquery += " AND ISNULL(IsInternalRemarks, 0) = " & CInt2(IsInternalRemarks)

                pSql = "SELECT * FROM (" & vbCrLf &
                        mainquery & vbCrLf &
                        " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                        " ORDER BY LastUpdated ASC"

                Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
                'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveQuickRemarks()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim ID As String = Common.WebRequest.Data("ID")
                Dim msg As String = Common.WebRequest.Data("Message")
                Dim tag As String = Common.WebRequest.Data("Tag")
                Dim IsInternalRemarks As String = Common.WebRequest.Data("IsInternalRemarks")

                If CStr2(msg) = "" Then
                    Return JsonDictionary.Serialize("Error", "Please enter message")
                End If

                If CStr2(tag) = "" Then
                    Return JsonDictionary.Serialize("Error", "Please select tag")
                End If

                Dim p As New InsertUpdate("LibraryQuickRemarks")
                p("Message") = ReadyString(msg)
                p("Tag") = ReadyString(tag)
                p("IsInternalRemarks") = CInt2(IsInternalRemarks)
                p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If CStr2(ID) = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("IsActive") = 1
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(ID)))
                End If

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function DeleteQuickRemarks()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim remarksid As String = Common.WebRequest.Data("RemarksID")
                Dim pSql As String = "DELETE FROM LibraryQuickRemarks WHERE ID = " & ReadyString(remarksid)
                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "deleted")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        <HttpPost>
        Public Function SaveSalesOrder()

            Dim pJson As TransInvoice.SaveSalesOrderJson
            Dim pError As String
            Dim pID As String
            Dim pOut As JsonDictionary
            Dim pDR As DataRow
            Dim pUuid As String
            Dim pInvoiceID As String
            Dim pNonZeroItemCount As Integer
            Dim pDtOtherOrderItems As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pJson = JsonConvert.DeserializeObject(Of TransInvoice.SaveSalesOrderJson)(Common.WebRequest.InputStreamText)
            pJson.UserID = WebSession.Instance.PeopleUserID
            If WebRequest.Instance.Business.TransInvoice.Verify(pJson, WebRequest.Instance.OrgID, pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If

            If String.IsNullOrWhiteSpace(pJson.ID) = False Then
                pDR = WebRequest.Instance.Business.TransInvoice.ById(pJson.ID)
                If TransInvoice.IsSavedAsFinal(pDR) Then
                    pOut = New JsonDictionary()
                    pOut.Add("Result", "Already saved as final")
                    pOut.Add("ID", pJson.ID)
                    pOut.Add("QuotationNo", CStr2(pDR("QuotationNo")))
                    pOut.Add("SalesOrderNo", CStr2(pDR("SalesOrderNo")))
                    pOut.Add("InvoiceNumber", CStr2(pDR("InvoiceNumber")))
                    pOut.Add("PurchaseOrderNo", CStr2(pDR("PurchaseOrderNo")))
                    Return pOut.Serialize()
                End If
            End If

            pUuid = Common.WebRequest.Data("UUID")
            pJson.DeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
            If pJson.DeviceID Is Nothing Then
                Throw New Exception("No desktop device. Please go to System Administration -> Authorized Devices, then add a device with blank UUID. vb5FSJEFYBL7!")
            End If
            If String.IsNullOrWhiteSpace(pJson.CopyProductsFromInvoiceID) = False Then
                Dim pDrSource = WebRequest.Instance.Business.TransInvoice.ById(pJson.CopyProductsFromInvoiceID)
                If pDrSource Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "The source transaction does not exist. ref: vb99N4EYRUL6!")
                End If
                If CBool2(pDrSource("IsCancelled")) Then
                    Return JsonDictionary.Serialize("Error", "The source transaction was already cancelled. Please select a source transaction that is still active.")
                End If
            End If
            If String.IsNullOrWhiteSpace(pJson.CustomerFowstID) Then
                If WebRequest.Instance.Business.SystemDefaults.RequireCustomerOutlet Then
                    Return JsonDictionary.Serialize("Error", "System Behaviour Settings requires Customer Outlet to be set.")
                End If
            End If
            If String.IsNullOrWhiteSpace(pJson.CustomerBinID) Then
                If WebRequest.Instance.Business.SystemDefaults.RequireCustomerDepartment Then
                    Return JsonDictionary.Serialize("Error", "System Behaviour Settings requires Customer Department to be set.")
                End If
            End If
            If pJson.SaveAndFinalizeProper Then
                pNonZeroItemCount = WebRequest.Instance.Business.TransInvoice.NonZeroItemCount(pJson.ID)
                If pNonZeroItemCount = 0 Then
                    Return JsonDictionary.Serialize("Error", "Finalized transactions must have at least 1 item with a quantity that is greater than zero.")
                End If
            End If

            If IsDate(pJson.TransDate) Then
                Select Case pJson.TransTypeProper
                    Case TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder
                        If ParseDate(pJson.TransDate) < Now Then
                            If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Ante Date Sales Order", False) = False Then
                                Return JsonDictionary.Serialize("Error", "You need Ante Date Sales Order security access.")
                            End If
                        Else
                            If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Post Date Sales Order", False) = False Then
                                Return JsonDictionary.Serialize("Error", "You need Post Date Sales Order security access.")
                            End If
                        End If

                    Case TransInvoice.SaveSalesOrderJson.eTransType.Invoice
                        If ParseDate(pJson.TransDate) < Now Then
                            If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Ante Date Invoice", False) = False Then
                                Return JsonDictionary.Serialize("Error", "You need Ante Date Invoice security access.")
                            End If
                        Else
                            If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Post Date Invoice", False) = False Then
                                Return JsonDictionary.Serialize("Error", "You need Post Date Invoice security access.")
                            End If
                        End If

                End Select
            End If

            WebRequest.Instance.ProData.BeginTransaction()
            If pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.Invoice _
                AndAlso pJson.SaveAndFinalizeProper _
                AndAlso CBool2(pJson.IsPos) = False Then

                WebRequest.Instance.Business.TransInvoice.SaveTransProfitability(pJson.ID, WebRequest.Instance.OrgID)
            End If

            pJson.SaveSalesRep = New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Allow Edit of Sales Rep Field", False)
            pID = WebRequest.Instance.Business.TransInvoice.Save(pJson, WebRequest.Instance.OrgID, pError)
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If

            If String.IsNullOrWhiteSpace(pJson.ID) = False Then
                WebRequest.Instance.Business.TransInvoice.UpdateItemPaymentTerm(pJson.ID)
            End If
            If pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.Invoice _
                AndAlso pJson.SaveAndFinalizeProper Then

                ' BEGIN commented by Arvin 2020-12-16
                'If String.IsNullOrWhiteSpace(pJson.ConsolidateSalesOrder) Then
                '    pDtOtherOrderItems = WebRequest.Instance.Business.TransInvoice.InvoiceableChildOrders(pJson.ID)
                '    If pDtOtherOrderItems IsNot Nothing Then
                '        pDtOtherOrderItems = pDtOtherOrderItems.DefaultView.ToTable(True, {"InvoiceID"})
                '        If pDtOtherOrderItems.Rows.Count > 0 Then
                '            Return JsonDictionary.ToString("OtherOrderCount", pDtOtherOrderItems.Rows.Count)
                '        End If
                '    End If
                'ElseIf CBool2(pJson.ConsolidateSalesOrder) Then
                '    pDtOtherOrderItems = WebRequest.Instance.Business.TransInvoice.InvoiceableChildOrders(pJson.ID)
                'End If

                'If pDtOtherOrderItems IsNot Nothing Then
                '    For Each pDrOtherOrderItem As DataRow In pDtOtherOrderItems.Rows
                '        WebRequest.Instance.Business.TransInvoice.MoveProducts(CStr2(pDrOtherOrderItem("InvoiceID")), pJson.ID)
                '    Next pDrOtherOrderItem
                'End If
                ' END commented by Arvin 2020-12-16

                WebRequest.Instance.Business.TransInvoice.AddToInventoryLedger(pID, WebRequest.Instance.OrgID)
                If Not String.IsNullOrWhiteSpace(pJson.SalesOrderID) Then
                    Dim pDtLacking As DataTable = WebRequest.Instance.Business.TransInvoice.GetLackingItems(pJson.ID)
                    pDtLacking = New DataView(pDtLacking, "OrderedQuantity > 0", "", DataViewRowState.CurrentRows).ToTable()
                    If pDtLacking.Rows.Count > 0 Then
                        pInvoiceID = pJson.ID
                        pJson.ID = ""
                        pJson.TransType = "SalesOrder"
                        pJson.Remarks = "Backorder"
                        pJson.UneditedBackorder = 1
                        pJson.SaveAndFinalize = 0
                        pJson.BackorderSourceInvoiceID = pInvoiceID
                        If pJson.Verify(pError) = False Then
                            Return JsonDictionary.Serialize("Error", pError)
                        End If
                        Dim pBackOrderID As String = WebRequest.Instance.Business.TransInvoice.Save(pJson, WebRequest.Instance.OrgID, pError)
                        If String.IsNullOrWhiteSpace(pError) = False Then
                            Return JsonDictionary.Serialize("Error", pError)
                        End If
                        If pDtLacking.Rows.Count > 0 Then
                            pJson.ID = pBackOrderID
                            Dim pDtStocks As DataTable = TransInvoice.CreateStockTable()
                            For Each pDrLacking As DataRow In pDtLacking.Rows
                                If CDec2(pDrLacking("OrderedQuantity")) = 0 Then
                                    Continue For
                                End If

                                Dim pDrStock As DataRow = pDtStocks.NewRow
                                pDrStock("StockID") = CStr2(pDrLacking("ProductID"))
                                pDrStock("Quantity") = CDec2(pDrLacking("OrderedQuantity"))
                                pDrStock("OrderedQuantity") = CDec2(pDrLacking("OrderedQuantity"))
                                pDrStock("ActualQuantity") = pDrLacking("ActualQuantity")
                                pDrStock("UnitMeasure") = pDrLacking("UnitMeasure")
                                pDrStock("UnitPrice") = pDrLacking("UnitPrice")
                                pDrStock("PriceLevelID") = pDrLacking("PriceLevelID")
                                pDrStock("Subtotal") = CDec2(pDrLacking("OrderedQuantity")) * CDec2(pDrLacking("UnitPrice"))
                                pDrStock("Dimension") = ""
                                pDrStock("IsReplacement") = False
                                pDrStock("IsHasReplacement") = False
                                pDrStock("IsHasBackOrder") = False
                                pDrStock("RefStockID") = ""
                                pDrStock("Cost") = 0
                                pDtStocks.Rows.Add(pDrStock)
                            Next pDrLacking
                            WebRequest.Instance.Business.TransInvoice.SaveStocks(pBackOrderID, pDtStocks, pJson.SalesOrderID, pInvoiceID, pBackOrderID, pJson.FowstID, pJson.UserID)
                            WebRequest.Instance.Business.TransInvoice.UpdateAmounts(pBackOrderID)
                        End If
                    End If
                End If
                If CBool2(pJson.IsPos) = False Then
                    WebRequest.Instance.Business.TransInvoice.SaveTransProfitability(pID, WebRequest.Instance.OrgID)
                End If
            ElseIf pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.SalesOrder _
                AndAlso pJson.SaveAndFinalizeProper Then

                WebRequest.Instance.Business.TransInvoice.SaveTransProfitability(pID, WebRequest.Instance.OrgID)
                WebRequest.Instance.Business.TransInvoice.FinalizeChildOrders(pID)
                WebRequest.Instance.Business.TransInvoice.CacheCommittedQtys(pID)
            ElseIf pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.Invoice _
                AndAlso CBool2(pJson.IsPos) = False Then
                WebRequest.Instance.Business.TransInvoice.SaveTransProfitability(pID, WebRequest.Instance.OrgID)
            ElseIf pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder _
                OrElse pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.AccountsPayable Then

                WebRequest.Instance.Business.TransInvoice.SaveVendorPriceHistory(pID)
            End If

            'If pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.PurchaseOrder _
            '    AndAlso pJson.SaveAndFinalizeProper Then

            '    WebRequest.Instance.Business.TransReceivingPayment.ca()
            'End If

            'If pJson.SaveAndFinalizeProper Then
            If pJson.TransTypeProper = TransInvoice.SaveSalesOrderJson.eTransType.AccountsPayable Then
                WebRequest.Instance.Business.TransReceiving.CacheReceivingsWithCredit()
            End If
            'End If

            WebRequest.Instance.ProData.CommitTransaction()
            pDR = WebRequest.Instance.Business.TransInvoice.ById(pID)
            pOut = New JsonDictionary()
            pOut.Add("Result", "ok")
            pOut.Add("ID", pID)
            pOut.Add("QuotationNo", CStr2(pDR("QuotationNo")))
            pOut.Add("SalesOrderNo", CStr2(pDR("SalesOrderNo")))
            pOut.Add("InvoiceNumber", CStr2(pDR("InvoiceNumber")))
            pOut.Add("PurchaseOrderNo", CStr2(pDR("PurchaseOrderNo")))
            pOut.Add("FinalizedForApproval", pJson.FinalizedForApproval)
            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function AddAddress()

            Dim pOrgID As String
            Dim pAddress As String
            Dim pID As String
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOrgID = Common.WebRequest.Data("CustomerID")
            pAddress = Common.WebRequest.Data("Address")
            pID = WebRequest.Instance.Business.LibraryOrganizationFOWST.GetOrAddByStreetNumber(pOrgID, pAddress)
            pOut = New JsonDictionary
            pOut.Add("Result", "ok")
            pOut.Add("ID", pID)

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function AddSalesRep()

            Dim pOrgID As String
            Dim pAddress As String
            Dim pID As String
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pName = CStr2(Common.WebRequest.Data("Name")).Trim()
            If pName = "" Then
                Return JsonDictionary.Serialize("Error", "Sales rep is required.")
            End If

            Dim pSql = "SELECT ID" &
                " FROM LibrarySalesRep" & vbCrLf &
                " WHERE Name = " & ReadyString(pName)
            If Common.WebRequest.ProData.ExecuteScalar(pSql) IsNot Nothing Then
                Return JsonDictionary.Serialize("Error", "Sales rep already exist.")
            End If
            Dim pIU As New InsertUpdate("LibrarySalesRep")
            pIU("ID") = "NEWID()"
            pIU("Name") = ReadyString(pName)
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetInsert()
            WebRequest.Instance.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Function CreatePdf() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pError As String
            Dim pInvoiceID As String = Common.WebRequest.Data("InvoiceID")

            Dim pUuid As String = Common.WebRequest.Data("UUID")
            pUuid = CStr2(pUuid).Replace(";", "_")
            Dim pTransType As String = Common.WebRequest.Data("TransType")
            If CStr2(pTransType).Trim.ToLower() = "invoice" Then
                If WebRequest.Instance.Business.TransInvoice.VerifyInvoice(pInvoiceID, pError) = False Then
                    Return JsonDictionary.Serialize("Error", pError)
                End If
            End If
            Dim pTemplate = CStr2(Common.WebRequest.Data("Template"))
            Dim pAllowReprint = CBool2(WebRequest.Instance.Data("AllowReprint"))
            Dim pPrintBlindCopy = CBool2(Common.WebRequest.Data("PrintBlindCopy"))
            Dim pPrintAsDraft = CBool2(Common.WebRequest.Data("PrintAsDraft"))
            Dim pOut2 = WebRequest.Instance.Business.TransInvoice.CreatePdf(vInvoiceID:=pInvoiceID,
                vOrgID:=WebRequest.Instance.OrgID,
                vTemplate:=pTemplate,
                vAllowReprint:=pAllowReprint,
                vPrintBlindCopy:=pPrintBlindCopy,
                vPrintAsDraft:=pPrintAsDraft,
                vUuid:=pUuid,
                vTransType:=pTransType)

            Return pOut2.Serialize()

        End Function

        Public Function CreateInvoicesPdfs() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInvoiceNumbers = Common.WebRequest.Data("InvoiceNumbers")
            Dim pResult = WebRequest.Instance.Business.TransInvoice.CreateInvoicesPdfs(pInvoiceNumbers, WebRequest.Instance.OrgID)

            Return JsonConvert.SerializeObject(pResult)

        End Function

        ''' <summary>
        ''' obsolete 2023_03_21__17_24_44_0542
        ''' </summary>
        ''' <returns></returns>
        Public Function DownloadInvoicesPdfs() As ActionResult

            Dim pTimeStamp = Common.WebRequest.QueryString("TimeStamp")
            Dim pFilename = Common.ServerPath & "temp\MassInvoicePdfs\" & pTimeStamp & "\MassInvoicePdfs.zip"

            Return File(pFilename, "application/zip", "MassInvoicePdfs_" & pTimeStamp & ".zip")

        End Function

        Public Function DownloadInvoicesSinglePdf() As ActionResult

            Dim pTimeStamp = Common.WebRequest.QueryString("TimeStamp")
            Dim pFilename = Common.ServerPath & "temp\MassInvoicePdfs\" & pTimeStamp & "\Merged_" & pTimeStamp & ".pdf"

            Return File(pFilename, "application/pdf", "Merged_" & pTimeStamp & ".pdf")

        End Function

        Function TagTransAsFollowUp()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.Business.TransInvoice.TagTransAsFollowUp(Common.WebRequest.Data("InvoiceID"))

            Return JsonDictionary.Serialize("Results", "ok")
        End Function

        Function MarkTransAsFollowedUp()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pIU As InsertUpdate
                pIU = New InsertUpdate("TransInvoice")
                pIU("IsForFollowUp") = 0
                Dim pSql = pIU.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("InvoiceID")))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Return JsonDictionary.Serialize("Results", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "error")
            End Try
        End Function

        Private Shared mPrintablePdfLock As New Object

        Public Function PrintablePdf() As String

            Dim pFolder As String
            Dim pFiles() As String
            Dim pFilename As String
            Dim pUuid As String

            SyncLock mPrintablePdfLock
                pUuid = Common.WebRequest.QueryString("UUID")
                pUuid = CStr2(pUuid).Replace(";", "_")
                If String.IsNullOrWhiteSpace(pUuid) Then
                    Return "UUID is required."
                End If
                'If pPrintHash.Length <> 30 Then
                '    Return "PrintHash must be 30 characters in length."
                'End If
                'If pPrintHash.Contains("*") Then
                '    Return "PrintHash is invalid"
                'End If
                pFolder = Common.ServerPath & "temp\PrintablePdf"
                If IO.Directory.Exists(pFolder) = False Then
                    Return "no pdf"
                End If
                pFiles = IO.Directory.GetFiles(pFolder, CreateValidFilename(pUuid) & "*")
                If pFiles.Count = 0 Then
                    Return "no pdf"
                Else
                    pFilename = IO.Path.GetFileName(pFiles(0))
                    Dim pSplits() As String = pFilename.Split({";"}, StringSplitOptions.None)
                    If pSplits.Length < 3 Then
                        Return pFilename
                    End If
                    Dim pJson As New PrintablePdfJson
                    pJson.Filename = pFilename
                    Dim pOrgID As String
                    If pSplits.Length = 4 Then
                        pOrgID = pSplits(2)
                        If IsGuid(pOrgID) = False Then
                            pOrgID = Nothing
                        End If
                    End If
                    If pOrgID IsNot Nothing Then
                        WebRequest.Instance.OrgID = pOrgID
                    End If
                    If WebRequest.Instance.HandleOrganization(True) = False Then
                        AppendToLog("OrgID: " & pOrgID & ". ref: vbN8ST2PUJ9E!")
                        Return WebRequest.InvalidOrganization
                    End If
                    If IsGuid(pSplits(1)) Then
                        Dim pDT As DataTable = WebRequest.Instance.Business.LibraryDocumentControl.GetData(pSplits(1))
                        If pDT.Rows.Count > 0 Then
                            pJson.PrinterName = CStr2(pDT.Rows(0)("PrinterName"))
                        End If
                    End If
                    Dim pOut = JsonConvert.SerializeObject(pJson)
                    Try
                        pFilename = IO.Path.Combine(pFolder, pFilename)
                        Dim pDestinationFilename = IO.Path.Combine(pFolder, "Requested", IO.Path.GetFileName(pFilename))
                        MoveFile(pFilename, pDestinationFilename)
                        If IO.File.Exists(pFilename) Then
                            Return "File, " & pFilename & ", was not moved. vbJJX6ACGU6G!"
                        End If
                        Return pOut
                    Catch ex As Exception
                        Return ex.Message
                    End Try
                End If
            End SyncLock

        End Function

        Public Function GetWritableCards() As String
            Dim assetManagement As New AssetsManagementController
            Dim fileReader As String
            Dim fileReaderSplit() As String

            If Not FileIO.FileSystem.FileExists(Common.ServerPath & "temp\UUID\" & CStr2(Common.WebRequest.QueryString("UUID")) & "ORG.dat") Then
                Return ""
            End If
            fileReader = My.Computer.FileSystem.ReadAllText(Common.ServerPath & "temp\UUID\" & CStr2(Common.WebRequest.QueryString("UUID")) & "ORG.dat")

            fileReaderSplit = fileReader.Split(vbCr)
            If fileReaderSplit.Length > 0 Then
                WebRequest.Instance.OrgID = fileReaderSplit(0)
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = assetManagement.GetWritableCards(fileReaderSplit(1).ToString)
            Return pOut

        End Function

        Public Function UpdateCardCreation() As String
            Dim assetManagement As New AssetsManagementController

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return assetManagement.UpdateCardCreation()

        End Function

        Public Function UpdateReadCard() As String
            Dim assetManagement As New AssetsManagementController

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return assetManagement.UpdateReadCard()
        End Function

        Public Function CreatePaymentPdf()

            Dim pCustomerPaymentID As String
            Dim pReport As ProReport
            Dim pFilename As String
            'Dim pUuid As String
            Dim pHeaderInfo As LibraryOrganization.HeaderInfo
            Dim pBaseFilename As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'pUuid = Common.WebRequest.Data("UUID")
            'If String.IsNullOrWhiteSpace(pUuid) Then
            '    Return JsonDictionary.Serialize("Error", "UUID is required.")
            'End If
            'If pUuid.Length < 1 Then
            '    Return JsonDictionary.Serialize("Error", "UUID must be at least 1 character in length.")
            'End If
            WebRequest.Instance.Business.LibraryOrganization.RestoreOrganizationTin()
            pCustomerPaymentID = Common.WebRequest.Data("CustomerPaymentID")
            If WebRequest.Instance.Data("Mode") = "AP" Then
                pReport = New ProReport
                pReport.AutoFillHeader = False
                pReport.SourceFilename = "CustomerPayment"
                Dim pDS As DataSet = WebRequest.Instance.Business.TransReceivingPayment.GetPaymentData(pCustomerPaymentID)
                Dim pDtMaster As DataTable = pDS.Tables("Master")
                If pDtMaster.Rows.Count > 0 Then
                    Dim pSupplierID = CStr2(pDtMaster.Rows(0)("SupplierID"))
                    pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(pSupplierID)
                    pReport.Variables("PayeeName") = pHeaderInfo.CompanyName
                    pReport.Variables("PayeeContactInfo") = pHeaderInfo.ContactInfos
                    pReport.Variables("PayeeAddress") = pHeaderInfo.CompanyAddress
                End If

                'pReport.SourceFilename = "SupplierPayment_" & ReadyGuid(WebRequest.Instance.OrgID)
                pReport.AddTable("Master", pDS.Tables("Master"))
                pReport.AddTable("Invoices", pDS.Tables("Invoices"))
                Dim pDtCheques = New DataView(pDS.Tables("Checks"), "IsOnlinePayment = 0", "", DataViewRowState.CurrentRows).ToTable()
                pReport.AddTable("Checks", pDtCheques)
                Dim pDtOnlinePayments = New DataView(pDS.Tables("Checks"), "IsOnlinePayment = 1", "", DataViewRowState.CurrentRows).ToTable()
                pReport.AddTable("OnlinePayment", pDtOnlinePayments)
                pReport.AddTable("OtherPayments", pDS.Tables("OtherPayments"))
                pReport.AddTable("SourceUnappliedCredits", WebRequest.Instance.Business.TransReceivingPayment.GetSourceUnappliedCreditsDetails(pCustomerPaymentID))
                pReport.AddTable("DestinationUnappliedCredits", WebRequest.Instance.Business.TransReceivingPayment.GetDestinationUnappliedCreditsDetails(pCustomerPaymentID))
                pReport.Variables("CustomerContactInfos") = pHeaderInfo.ContactInfos
                pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
                pReport.VariablesOptional("PayorName") = pHeaderInfo.CompanyName
                pReport.VariablesOptional("PayorContactInfo") = pHeaderInfo.ContactInfos
                pReport.VariablesOptional("PayorAddress") = pHeaderInfo.CompanyAddress

                pReport.VariablesOptional("PayeeNameLabel") = "Paid To/ Payee:"
                pReport.VariablesOptional("PayeeContactInfoLabel") = "Payee's Contact No.:"
                pReport.VariablesOptional("PayeeAddressLabel") = "Payee's Address:"

                pReport.VariablesOptional("PayorNameLabel") = "Paid By/ Payor:"
                pReport.VariablesOptional("PayorContactInfoLabel") = "Payor's Contact No.:"
                pReport.VariablesOptional("PayorAddressLabel") = "Payor's Address:"

                pReport.ExportXmlFile()
                pBaseFilename = "SupplierPayment_" & DateTimeStamp() & ".pdf"
                pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename
                CreateFolder(pFilename)
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
            Else
                pFilename = IO.Path.Combine(Common.ServerPath, "Reports", "CustomerPayment_" & ReadyGuid(WebRequest.Instance.OrgID) & ".mrt")
                If IO.File.Exists(pFilename) = False Then
                    pFilename = IO.Path.Combine(Common.ServerPath, "Reports", "CustomerPayment.mrt")
                    If IO.File.Exists(pFilename) = False Then
                        Return JsonDictionary.Serialize("Error", "No print template")
                    End If
                End If
                Dim pDS = WebRequest.Instance.Business.TransCustomerPayment.GetPaymentData(pCustomerPaymentID)
                pReport = New ProReport
                pReport.AutoFillHeader = False
                pReport.SourceFilename = IO.Path.GetFileName(pFilename)

                Dim pDtMaster As DataTable = pDS.Tables("Master")
                If pDtMaster.Rows.Count > 0 Then
                    Dim pCustomerID = CStr2(pDtMaster.Rows(0)("CustomerID"))
                    pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(pCustomerID)
                    pReport.VariablesOptional("PayeeName") = pHeaderInfo.CompanyName
                    pReport.VariablesOptional("PayeeContactInfo") = pHeaderInfo.ContactInfos
                    pReport.VariablesOptional("PayeeAddress") = pHeaderInfo.CompanyAddress
                End If

                pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
                pReport.VariablesOptional("PayorName") = pHeaderInfo.CompanyName
                pReport.VariablesOptional("PayorContactInfo") = pHeaderInfo.ContactInfos
                pReport.VariablesOptional("PayorAddress") = pHeaderInfo.CompanyAddress

                pReport.VariablesOptional("PayorNameLabel") = "Paid To/ Payee:"
                pReport.VariablesOptional("PayorContactInfoLabel") = "Payee's Contact No.:"
                pReport.VariablesOptional("PayorAddressLabel") = "Payee's Address:"

                pReport.VariablesOptional("PayeeNameLabel") = "Paid By/ Payor:"
                pReport.VariablesOptional("PayeeContactInfoLabel") = "Payor's Contact No.:"
                pReport.VariablesOptional("PayeeAddressLabel") = "Payor's Address:"

                pReport.AddTable("Master", pDS.Tables("Master"))
                pReport.AddTable("Invoices", pDS.Tables("Invoices"))
                pReport.AddTable("Checks", pDS.Tables("Checks"))
                pReport.AddTable("OtherPayments", pDS.Tables("OtherPayments"))
                pReport.ExportXmlFile()
                pBaseFilename = "CustomerPayment_" & DateTimeStamp() & ".pdf"
                pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename
                CreateFolder(pFilename)
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
            End If

            Dim pOut = New JsonDictionary()
            pOut.Add("Result", "ok")
            pOut.Add("BaseFilename", pBaseFilename)
            Return pOut.Serialize()

        End Function

        Public Function GetAccountsPayablePaymentData()

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.Business.LibraryOrganization.RestoreOrganizationTin()
            Dim pAccountsPayableID = Common.WebRequest.Data("AccountsPayableID")
            Dim pDtPaymentIds = WebRequest.Instance.Business.TransReceivingPayment.GetPaymentIdsByAccountPayableID(pAccountsPayableID)
            Dim pOut = New JsonDictionary()

            Dim pList = pOut.AddList("payments")
            For Each pDrPaymentId As DataRow In pDtPaymentIds.Rows
                Dim pDS As DataSet = WebRequest.Instance.Business.TransReceivingPayment.GetPaymentData(CStr2(pDrPaymentId("PaymentID")))
                With pList.AddDictionary()
                    .Add("Master", pDS.Tables("Master"))
                    .Add("Invoices", pDS.Tables("Invoices"))
                    .Add("Checks", pDS.Tables("Checks"))
                    .Add("OtherPayments", pDS.Tables("OtherPayments"))
                End With
            Next

            Return pOut.Serialize()

        End Function

        Public Function GetPayablePaymentData()

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pPaymentID = Common.WebRequest.Data("PaymentID")
            Dim pOut = New JsonDictionary()

            Dim pDS As DataSet = WebRequest.Instance.Business.TransReceivingPayment.GetPaymentData(pPaymentID)
            With pOut
                .Add("Master", pDS.Tables("Master"))
                .Add("Invoices", pDS.Tables("Invoices"))
                .Add("Checks", pDS.Tables("Checks"))
                .Add("OtherPayments", pDS.Tables("OtherPayments"))
                .Add("DestinationUnappliedCredits", WebRequest.Instance.Business.TransReceivingPayment.GetDestinationUnappliedCreditsDetails(pPaymentID))
            End With

            Return pOut.Serialize()

        End Function

        Public Function GetAccountsReceivablePaymentData()

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary()
            WebRequest.Instance.Business.LibraryOrganization.RestoreOrganizationTin()
            Dim pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim pSalesOrderID = Common.WebRequest.Data("SalesOrderID")
            If String.IsNullOrWhiteSpace(pInvoiceID) Then
                If String.IsNullOrWhiteSpace(pSalesOrderID) Then
                    Return pOut.Serialize()
                End If
                Dim pDR = WebRequest.Instance.Business.TransInvoice.BySalesOrderID(pSalesOrderID)
                If pDR Is Nothing Then
                    Return pOut.Serialize()
                End If
                pInvoiceID = CStr2(pDR("ID"))
            End If
            Dim pDtPaymentIds = WebRequest.Instance.Business.TransCustomerPayment.GetPaymentIdsByInvoiceID(pInvoiceID)

            Dim pList = pOut.AddList("payments")
            For Each pDrPaymentId As DataRow In pDtPaymentIds.Rows
                Dim pDS As DataSet = WebRequest.Instance.Business.TransCustomerPayment.GetPaymentData(CStr2(pDrPaymentId("CustomerPaymentID")))
                With pList.AddDictionary()
                    .Add("Master", pDS.Tables("Master"))
                    .Add("Invoices", pDS.Tables("Invoices"))
                    .Add("Checks", pDS.Tables("Checks"))
                    .Add("OtherPayments", pDS.Tables("OtherPayments"))
                End With
            Next

            Return pOut.Serialize()

        End Function

        Public Function GetReceivablePaymentData()

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pPaymentID = Common.WebRequest.Data("PaymentID")
            Dim pOut = New JsonDictionary()

            Dim pDS As DataSet = WebRequest.Instance.Business.TransCustomerPayment.GetPaymentData(pPaymentID)
            With pOut
                .Add("Master", pDS.Tables("Master"))
                .Add("Invoices", pDS.Tables("Invoices"))
                .Add("Checks", pDS.Tables("Checks"))
                .Add("OtherPayments", pDS.Tables("OtherPayments"))
                .Add("DestinationUnappliedCredits", WebRequest.Instance.Business.TransCustomerPayment.GetDestinationUnappliedCreditsDetails(pPaymentID))
            End With

            Return pOut.Serialize()

        End Function

        Public Function HasWitholdingTax() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim pCount As Integer
            Dim pPaymentID = Common.WebRequest.Data("PaymentID")
            If Common.WebRequest.Data("Mode") = "AP" Then
                pSql = "SELECT COUNT(*)" & vbCrLf &
                    " FROM TransReceivingPaymentOther" & vbCrLf &
                    " WHERE PaymentType = 'Witholding Tax'" & vbCrLf &
                    " AND PaymentID = " & ReadyString(pPaymentID)
            Else
                pSql = "SELECT COUNT(*)" & vbCrLf &
                    " FROM TransCustomerPaymentOther" & vbCrLf &
                    " WHERE PaymentType = 'Witholding Tax'" & vbCrLf &
                    " AND CustomerPaymentID = " & ReadyString(pPaymentID)
            End If

            pCount = CInt2(Common.WebRequest.ProData.ExecuteScalar(pSql))

            Return JsonDictionary.Serialize("Result", pCount)

        End Function

        Public Function CreateForm2307() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pBaseFilename = WebRequest.Instance.Business.TransReceivingPayment.CreateForm2307(Common.WebRequest.Data("PaymentID"), Common.WebRequest.Data("Mode"))

            Return JsonDictionary.Serialize("BaseFilename", pBaseFilename)

        End Function

        Public Function DownloadForm2307() As ActionResult

            Dim pFilename = Common.ServerPath & "temp\Form2307\Form2307_" & WebRequest.Instance.QueryString("BaseFilename") & ".xlsx"

            Return File(pFilename, "application/xlsx", "Form2307_" & WebRequest.Instance.QueryString("BaseFilename") & ".xlsx")

        End Function

        Public Function ImportPrintablePdf() As String

            Dim pServer = WebRequest.Instance.Data("Server")
            Dim pFilename = WebRequest.Instance.Data("Filename")
            Dim pUrl = AppendIfNeeded(pServer, "/") & "Erp/DownloadPrintablePdf?Filename=" & pFilename

            Dim pFolder = Common.ServerPath & "temp\PrintablePdf"
            Dim pTempFilename = "temp_" & pFilename
            pTempFilename = IO.Path.Combine(pFolder, pTempFilename)
            CreateFolder(pTempFilename)

            Dim pWeb As New Net.WebClient
            pWeb.DownloadFile(pUrl, pTempFilename)
            ProPack.Utility.AddDirectoryUser(pFolder)
            Rename(pTempFilename, IO.Path.Combine(pFolder, pFilename))

            pUrl = AppendIfNeeded(pServer, "/") & "Erp/DeletePrintablePdf?Filename=" & pFilename
            Dim pResult = pWeb.DownloadString(pUrl) ' we dont care about the result

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Function DownloadPrintablePdf() As ActionResult

            Dim pFolder As String
            Dim pFilename As String

            pFolder = Common.ServerPath & "temp\PrintablePdf\Requested"
            pFilename = Common.WebRequest.QueryString("Filename")
            pFilename = IO.Path.Combine(pFolder, pFilename)
            If IO.File.Exists(pFilename) Then
                Return File(pFilename, "application/pdf")
            Else
                pFolder = Common.ServerPath & "temp\PrintablePdf"
                pFilename = Common.WebRequest.QueryString("Filename")
                pFilename = IO.Path.Combine(pFolder, pFilename)
                If IO.File.Exists(pFilename) Then
                    Return File(pFilename, "application/pdf")
                Else
                    Return Content("File does not exist. ref: vb2RN2DAW6PU!")
                End If
            End If

        End Function

        Public Function DeletePrintablePdf() As String

            Dim pFolder As String
            Dim pFilename As String

            TransInvoice.DeleteOldPrintablePdf()
            pFolder = Common.ServerPath & "temp\PrintablePdf"
            pFilename = Common.WebRequest.QueryString("Filename")
            pFilename = IO.Path.Combine(pFolder, pFilename)
            If IO.File.Exists(pFilename) Then
                IO.File.Delete(pFilename)
                Return "The file was successfully deleted."
            Else
                pFolder = Common.ServerPath & "temp\PrintablePdf\Requested"
                pFilename = Common.WebRequest.QueryString("Filename")
                pFilename = IO.Path.Combine(pFolder, pFilename)
                If IO.File.Exists(pFilename) Then
                    IO.File.Delete(pFilename)
                    Return "The file was successfully deleted."
                Else
                    Return "The file does not exist."
                End If
            End If

        End Function

        Public Class SaveInvoiceSettingsInput
            Public VendorIDs As List(Of String)
            Public RemovedVendors As List(Of String)
            Public AutoPrintStatus As List(Of AutoPrintStatusItemClass)

            Public Class AutoPrintStatusItemClass
                Public ID As String
                Public Status As String
            End Class
        End Class

        <HttpPost>
        Public Function SaveInvoiceSettings() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.ProData.BeginTransaction()
            WebRequest.Instance.Business.SystemDefaults.AllowEncodingOfOrdersInInvoice = CBool2(Common.WebRequest.Data("AllowEncodingOfOrdersInInvoice"))
            WebRequest.Instance.Business.SystemDefaults.AllowEncodingOfQuantitiesInInvoice = CBool2(Common.WebRequest.Data("AllowEncodingOfQuantitiesInInvoice"))
            WebRequest.Instance.Business.SystemDefaults.FreeTypeAPriceMinimumGpm = CDec2(Common.WebRequest.Data("FreeTypeAPriceMinimumGpm"))
            WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceTypeID = Common.WebRequest.Data("AutoCreateInvoiceTypeID")
            WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceFowstID = Common.WebRequest.Data("AutoCreateInvoiceFowstID")
            WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceDeviceID = Common.WebRequest.Data("AutoCreateInvoiceDeviceID")
            WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceCondition = Common.WebRequest.Data("AutoCreateInvoiceCondition")

            Dim pInput = JsonConvert.DeserializeObject(Of SaveInvoiceSettingsInput)(Common.WebRequest.InputStreamText)
            Dim pAutoCreateInvoiceCondition = WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceConditionProper
            If pAutoCreateInvoiceCondition = SystemDefaults.eAutoCreateInvoiceCondition.StockReceivingSelected _
                OrElse pAutoCreateInvoiceCondition = SystemDefaults.eAutoCreateInvoiceCondition.VendorPortalSelected Then

                Dim pSql As String
                Dim pRemovedVendorIDs = ""
                For Each pCustomerID In pInput.RemovedVendors
                    If pRemovedVendorIDs <> "" Then
                        pRemovedVendorIDs &= ", "
                    End If
                    pRemovedVendorIDs &= ReadyString(pCustomerID)
                Next
                If pRemovedVendorIDs <> "" Then
                    Dim pIU = New InsertUpdate("LibraryOrganizationSupplier")
                    pIU("AutoCreateInvoice") = 0
                    pIU("LastUpdated") = "GETDATE()"
                    pSql = pIU.GetUpdate("SupplierID IN (" & pRemovedVendorIDs & ")")
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                End If

                For Each pVendorID In pInput.VendorIDs
                    Dim pIU = New InsertUpdate("LibraryOrganizationSupplier")
                    pIU("AutoCreateInvoice") = 1
                    pIU("LastUpdated") = "GETDATE()"
                    pSql = pIU.GetUpdate("SupplierID = " & ReadyString(pVendorID))
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                Next

                If (
                        pInput.RemovedVendors IsNot Nothing _
                        AndAlso pInput.RemovedVendors.Count > 0
                    ) OrElse (
                        pInput.VendorIDs IsNot Nothing _
                        AndAlso pInput.VendorIDs.Count > 0
                    ) Then

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                End If
            End If

            For Each pAutoPrintRequest In pInput.AutoPrintStatus
                Dim pIU = New InsertUpdate("LibraryAutoPrint")
                pIU("Status") = ReadyString(pAutoPrintRequest.Status)
                Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pAutoPrintRequest.ID))
                WebRequest.Instance.ProData.ExecuteSQL(pSql)
            Next pAutoPrintRequest

            WebRequest.Instance.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "ok")
        End Function

        <HttpPost>
        Public Function GetInvoiceSettings() As String

            Dim pOut As JsonDictionary
            Dim pDR As DataRow
            Dim pID As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()

            pOut.Add("AllowEncodingOfOrdersInInvoice", WebRequest.Instance.Business.SystemDefaults.AllowEncodingOfOrdersInInvoice)
            pOut.Add("AllowEncodingOfQuantitiesInInvoice", WebRequest.Instance.Business.SystemDefaults.AllowEncodingOfQuantitiesInInvoice)
            pOut.Add("FreeTypeAPriceMinimumGpm", WebRequest.Instance.Business.SystemDefaults.FreeTypeAPriceMinimumGpm)

            Dim pDT = WebRequest.Instance.Business.LibraryDocumentControl.GetData(, " AND ModuleSource = 'Revenue > Invoicing'")
            pOut.Add("InvoiceTypes", pDT)
            pOut.Add("AutoCreateInvoiceTypeID", WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceTypeID)

            pID = WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceFowstID
            pOut.Add("AutoCreateInvoiceFowstID", pID)
            If IsGuid(pID) Then
                pDR = WebRequest.Instance.Business.LibraryOrganizationFOWST.ByID(pID)
                If pDR IsNot Nothing Then
                    pOut.Add("AutoCreateInvoiceFowstName", CStr2(pDR("Name")))
                End If
            End If

            pID = WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceDeviceID
            pOut.Add("AutoCreateInvoiceDeviceID", pID)
            If IsGuid(pID) Then
                pDR = WebRequest.Instance.Business.LibraryDevice.ById(pID)
                If pDR IsNot Nothing Then
                    pOut.Add("AutoCreateInvoiceDeviceName", CStr2(pDR("PhoneName")))
                End If
            End If

            pOut.Add("AutoCreateInvoiceCondition", WebRequest.Instance.Business.SystemDefaults.AutoCreateInvoiceCondition)

            Dim pSql = "SELECT MobizLibraryOrganization.ID" & vbCrLf &
                ",MobizLibraryOrganization.CompanyName" & vbCrLf &
                " FROM LibraryOrganizationSupplier" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryOrganizationSupplier.SupplierID" & vbCrLf &
                " WHERE LibraryOrganizationSupplier.AutoCreateInvoice = 1" & vbCrLf &
                " ORDER BY MobizLibraryOrganization.CompanyName"
            pDT = WebRequest.Instance.ProData.Execute(pSql)
            pOut.Add("Vendors", pDT)

            pSql = "SELECT LibraryAutoPrint.ID" & vbCrLf &
                "," & BrowserController.ComposePeopleName() & " AS RequestedByName" & vbCrLf &
                ",LibraryAutoPrint.Uuid" & vbCrLf &
                ",LibraryAutoPrint.DateRequested" & vbCrLf &
                " FROM LibraryAutoPrint" & vbCrLf &
                " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryAutoPrint.RequestedByPeopleID" & vbCrLf &
                " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf &
                " ORDER BY RequestedByName"
            pDT = WebRequest.Instance.ProData.Execute(pSql)
            pOut.Add("AutoPrint", pDT)

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function GetDocumentControl() As String

            Dim pOut As JsonDictionary
            Dim pID, pModuleSource As String
            Dim pWhere As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pID = Common.WebRequest.Data("ID")
            pModuleSource = CStr2(Common.WebRequest.Data("ModuleSource"))
            WebRequest.Instance.Business.LibraryDocumentControl.CreateData()
            If CBool2(Common.WebRequest.Data("NoPo")) Then
                pWhere &= " AND DocumentName NOT LIKE '%Purchase Order%'" & vbCrLf
            ElseIf CBool2(Common.WebRequest.Data("PoOnly")) Then
                pWhere &= " AND DocumentName LIKE '%Purchase Order%'" & vbCrLf
            End If

            If pModuleSource <> "" Then
                pWhere &= " AND ModuleSource = " & ReadyString(pModuleSource) & vbCrLf
            End If

            Return JsonDictionary.Serialize(WebRequest.Instance.Business.LibraryDocumentControl.GetData(pID, pWhere))

        End Function

        <HttpPost>
        Public Function SaveDocumentControl() As String

            Dim pOut As JsonDictionary
            Dim pID As String
            Dim pJson As LibraryDocumentControl.SaveJson
            Dim pError As String
            Dim pBaseFilename As String
            Dim pTempFilename As String
            Dim pFilename As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'pJson = JsonConvert.DeserializeObject(Of LibraryDocumentControl.SaveJson)(Common.WebRequest.InputStreamText)
            pJson = New LibraryDocumentControl.SaveJson()
            pJson.ID = Common.WebRequest.Data("ID")
            pJson.DocumentName = Common.WebRequest.Data("DocumentName")
            pJson.ModuleSource = Common.WebRequest.Data("ModuleSource")
            pJson.PrintLimit = Common.WebRequest.Data("PrintLimit")
            pJson.IsActive = Common.WebRequest.Data("IsActive")
            pJson.AutoPrint = Common.WebRequest.Data("AutoPrint")
            pJson.CopiesPerPrint = Common.WebRequest.Data("CopiesPerPrint")
            pJson.LabelOfCopy01 = Common.WebRequest.Data("LabelOfCopy01")
            pJson.LabelOfCopy02 = Common.WebRequest.Data("LabelOfCopy02")
            pJson.LabelOfCopy03 = Common.WebRequest.Data("LabelOfCopy03")
            pJson.LabelOfCopy04 = Common.WebRequest.Data("LabelOfCopy04")
            pJson.LabelOfCopy05 = Common.WebRequest.Data("LabelOfCopy05")
            pJson.LabelOfCopy06 = Common.WebRequest.Data("LabelOfCopy06")
            pJson.LabelOfCopy07 = Common.WebRequest.Data("LabelOfCopy07")
            pJson.LabelOfCopy08 = Common.WebRequest.Data("LabelOfCopy08")
            pJson.LabelOfCopy09 = Common.WebRequest.Data("LabelOfCopy09")
            pJson.LabelOfCopy10 = Common.WebRequest.Data("LabelOfCopy10")
            pJson.Footnote = Common.WebRequest.Data("Footnote")
            pJson.PrinterName = Common.WebRequest.Data("PrinterName")
            pJson.SeriesType = Common.WebRequest.Data("SeriesType")
            pJson.SeriesStartingNum = Common.WebRequest.Data("SeriesStartingNum")
            pJson.PermitNum = Common.WebRequest.Data("PermitNum")
            pJson.MaxLineItemsPerPage = Common.WebRequest.Data("MaxLineItemsPerPage")
            pJson.MaxNoOfSupplementalBlindCopies = Common.WebRequest.Data("MaxNoOfSupplementalBlindCopies")
            pJson.UnitPriceSurchargePercent = CDec2(Common.WebRequest.Data("UnitPriceSurchargePercent"))

            pError = pJson.Verify()
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If
            WebRequest.Instance.Business.LibraryDocumentControl.CreateData()
            pID = WebRequest.Instance.Business.LibraryDocumentControl.Save(pJson)
            If IsGuid(pID) = False Then
                Return JsonDictionary.Serialize("Error", "ID must be a GUID. ref: vbNRMWMGA9KF!")
            End If

            pOut = New JsonDictionary()
            If Request.Files.Count > 0 _
                AndAlso Request.Files(0).ContentLength > 0 Then

                pBaseFilename = "Invoice_" & ReadyGuid(WebRequest.Instance.OrgID) & "_" & ReadyGuid(pID)
                pTempFilename = Common.ServerPath & "Reports\" & pBaseFilename & "_temp.mrt"
                pFilename = Common.ServerPath & "Reports\" & pBaseFilename & ".mrt"
                Request.Files(0).SaveAs(pTempFilename)
                Try
                    Dim pReport As New ProReport
                    pReport.AutoFillHeader = False
                    pReport.SourceFilename = pBaseFilename & "_temp"
                    Try
                        IO.File.Delete(pFilename)
                    Catch ex As Exception
                    End Try
                    IO.File.Move(pTempFilename, pFilename)
                Catch ex As Exception
                    pOut.Add("Warning", "The data was successfully saved but you must reupload the template because an error occured while saving the uploaded template: " & ex.Message)
                    ' uploaded file is not a valid 
                End Try
            End If

            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function ImportMallLessorSummaryFile() As String

            Dim pOut As JsonDictionary
            Dim pError As String
            Dim pFilename As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()
            If Request.Files.Count > 0 _
                AndAlso Request.Files(0).ContentLength > 0 Then

                pFilename = Common.ServerPath & "temp\ImportMallLessorSummaryFile-" & DateTimeStamp() & ".zip"
                Request.Files(0).SaveAs(pFilename)
                Try
                    WebRequest.Instance.Business.TransInvoice.ImportMallLessor(pFilename, WebRequest.Instance.OrgID, pError)
                    If String.IsNullOrWhiteSpace(pError) Then
                        pOut.Add("Result", "ok")
                    Else
                        pOut.Add("Error", pError)
                    End If
                Catch ex As Exception
                    pOut.Add("Error", ex.Message)
                End Try
            End If

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function ImportPoXml() As String

            Dim pOut As JsonDictionary
            Dim pFilename As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()
            If Request.Files.Count > 0 _
                AndAlso Request.Files(0).ContentLength > 0 Then

                pFilename = Common.ServerPath & "temp\ImportXml\" & DateTimeStamp() & ".xml"
                CreateFolder(pFilename)
                Request.Files(0).SaveAs(pFilename)
                Try
                    Dim pPoXmlImport = New PoXmlImport(WebRequest.Instance.Business)
                    pPoXmlImport.SourceFilename = pFilename
                    Dim pID = pPoXmlImport.ImportSm()
                    If IsGuid(pID) Then
                        pOut.Add("ID", pID)
                        pOut.Add("Result", "ok")
                    Else
                        pOut.Add("Error", pID)
                    End If
                Catch ex As Exception
                    pOut.Add("Warning", "The data was successfully saved but you must reupload the template because an error occured while saving the uploaded template: " & ex.Message)
                    ' uploaded file is not a valid 
                End Try
            End If

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function UploadReportTemplate() As String

            Dim pFilename As String
            Dim pID As String

            If Request.Files.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "No file attached. ref: vb8E7VK2J2LP!")
            End If

            pID = Common.WebRequest.Data("ID")
            If IsGuid(pID) = False Then
                Return JsonDictionary.Serialize("Error", "ID must be a GUID. ref: vbNRMWMGA9KF!")
            End If

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFilename = "Invoice_" & ReadyGuid(WebRequest.Instance.OrgID) & "_" & ReadyString(pID) & ".mrt"
            pFilename = Common.ServerPath & "Reports\" & pFilename

            Request.Files(0).SaveAs(pFilename)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function CancelInvoice() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pError As String
            Dim pInvoiceID As String = Common.WebRequest.Data("InvoiceID")

            If CStr2(pInvoiceID).Trim() = "" Then
                Return JsonDictionary.Serialize("Error", "Invoice must not be blank. ref: vbUR4HHE7QY4!")
            End If

            If WebRequest.Instance.Business.TransInvoice.CancelInvoice(pInvoiceID, WebSession.Instance.PeopleUserID, WebRequest.Instance.OrgID, pError) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

        End Function

        <HttpPost>
        Public Function PriceHistory() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pCustomerID As String = Common.WebRequest.Data("CustomerID")
            Dim pProductID As String = Common.WebRequest.Data("ProductID")
            Dim pDT As DataTable

            If CStr2(pCustomerID).Trim() = "" Then
                Return JsonDictionary.Serialize("Error", "CustomerID must not be blank. ref: vbB924C4K6SV!")
            End If

            If CStr2(pProductID).Trim() = "" Then
                Return JsonDictionary.Serialize("Error", "ProductID must not be blank. ref: vb8CJKGBCXV4!")
            End If

            pDT = WebRequest.Instance.Business.LibraryCustomers.GetPriceHistory(pCustomerID, pProductID)

            Return JsonDictionary.Serialize("Result", pDT)

        End Function

        <HttpPost>
        Public Function SaveAccountNameAndNumber() As String

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pAffectedRecords As Integer

            Try
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                Dim pAccountName As String = Common.WebRequest.Data("AccountName")
                Dim pAccountNumber As String = Common.WebRequest.Data("AccountNumber")

                If String.IsNullOrWhiteSpace(pAccountName) Then
                    Return JsonDictionary.Serialize("Error", "Account Name must not be blank.")
                End If

                If String.IsNullOrWhiteSpace(pAccountNumber) Then
                    Return JsonDictionary.Serialize("Error", "Account Number must not be blank.")
                End If

                pAccountName = pAccountName.Trim()
                pAccountNumber = pAccountNumber.Trim()

                pIU = New InsertUpdate("LibraryDepositAccounts")
                pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                pIU("LastUpdated") = "GETDATE()"
                pSql = pIU.GetUpdate("AccountName = " & ReadyString(pAccountName) & vbCrLf &
                    " AND AccountNumber = " & ReadyString(pAccountNumber))
                pAffectedRecords = Common.WebRequest.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pIU("ID") = "NEWID()"
                    pIU("AccountName") = ReadyString(pAccountName)
                    pIU("AccountNumber") = ReadyString(pAccountNumber)
                    pSql = pIU.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function SaveCategoryGpm() As String

            Dim pSql As String
            Dim pAffectedRecords As Integer

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pCategory As String = Common.WebRequest.Data("Category")
            Dim pGpm As String = Common.WebRequest.Data("Gpm")
            Dim pIU As New InsertUpdate("LibraryPimCategoryGpm")

            pIU("Gpm") = CDec2(pGpm)
            pSql = pIU.GetUpdate("Category = " & ReadyString(pCategory))
            pAffectedRecords = Common.WebRequest.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pIU("Category") = ReadyString(pCategory)
                pSql = pIU.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function DeleteCategoryGpm() As String

            Dim pSql As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim p As DeleteCategoryJson

            p = JsonConvert.DeserializeObject(Of DeleteCategoryJson)(Common.WebRequest.InputStreamText)

            Common.WebRequest.ProData.BeginTransaction()
            For Each pCategory As String In p.Categories
                pSql = "DELETE FROM LibraryPimCategoryGpm WHERE Category = " & ReadyString(pCategory)
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next pCategory
            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Class DeleteCategoryJson
            Public Categories As List(Of String)
        End Class

        <HttpPost>
        Public Function ChildSos() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim pDT = WebRequest.Instance.Business.TransInvoice.ChildSos(pInvoiceID)
            Return JsonDictionary.Serialize(pDT)

        End Function

        <HttpPost>
        Public Function RelatedSalesOrders() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInvoiceID = Common.WebRequest.Data("ID")
            Dim pIncludeInvoiced = CBool2(Common.WebRequest.Data("IncludeInvoiced"))
            Dim pDT = WebRequest.Instance.Business.TransInvoice.RelatedSalesOrders(pInvoiceID, pIncludeInvoiced)
            Return JsonDictionary.Serialize(pDT)

        End Function

        <HttpPost>
        Public Function InvoicableOrdersCount() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.Business.TransInvoice.AutoFinalizePurchaseOrdersFromVendor(WebRequest.Instance.OrgID)
            Dim pOut = WebRequest.Instance.Business.TransInvoice.InvoicableOrdersCount()
            Return JsonDictionary.Serialize("Count", pOut)

        End Function

        <HttpPost>
        Public Function SaveContactPerson() As String

            Dim pSql As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Common.WebRequest.ProData.BeginTransaction()
            Dim pPersonID = WebRequest.Instance.Business.LibraryPeople.GetOrCreatePeople(Common.WebRequest.Data("LastName"), Common.WebRequest.Data("FirstName"), Common.WebRequest.Data("MiddleName"))
            Dim pIU = New InsertUpdate("MobizLibraryOrganizationContactPerson")
            pIU("ID") = "NEWID()"
            pIU("OrganizationID") = ReadyString(Common.WebRequest.Data("ClientOrgID"))
            pIU("PeopleID") = ReadyString(pPersonID)
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetInsert
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function InvoiceableChildOrdersCount() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim pDT As DataTable = WebRequest.Instance.Business.TransInvoice.InvoiceableChildOrders(pInvoiceID)
            pDT = pDT.DefaultView.ToTable(True, {"InvoiceID"})
            Dim pCount = pDT.Rows.Count

            Return JsonDictionary.Serialize("Result", pCount)

        End Function

        <HttpPost>
        Public Function GetPrinterByFowstIdAndUuid() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pFowstID = Common.WebRequest.Data("FowstID")
            Dim pUuid = Common.WebRequest.Data("Uuid")
            Dim pDR As DataRow = WebRequest.Instance.Business.LibraryPrinters.GetPrinterByFowstIdAndUuid(pFowstID, pUuid)
            If pDR IsNot Nothing Then
                Dim pOut As New JsonDictionary()
                pOut.Add("Result", pDR)
                Return pOut.Serialize()
            Else
                Return JsonDictionary.Serialize("Error", "No printer assigned to that device in that FOWST.")
            End If

        End Function

        <HttpPost>
        Public Function SaveOptions() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.Business.SystemDefaults.AccountsPayableFollowVendorCost = CBool2(Common.WebRequest.Data("AccountsPayableFollowCost"))

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function LoadOptions() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary()

            pOut.Add("AccountsPayableFollowVendorCost", If(WebRequest.Instance.Business.SystemDefaults.AccountsPayableFollowVendorCost, 1, 0))
            pOut.Add("Result", "ok")

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function SetAllCustomerUnlimitedCredit() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pValue As String = Common.WebRequest.Data("value")
            If String.IsNullOrWhiteSpace(pValue) Then
                Return JsonDictionary.Serialize("Error", "No parameter has been set.")
            End If

            WebRequest.Instance.Business.LibraryCustomers.SetAllCustomerUnlimitedCredit(CBool2(pValue))

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function IsOkForFC() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pTransID As String = Common.WebRequest.Data("ID")
            Dim pOut As New JsonDictionary()
            pOut.Add("CncNotOk", If(WebRequest.Instance.Business.TransInvoice.IsCncApprovable(pTransID, Nothing), 1, 0))
            pOut.Add("ProfitNotOk", If(WebRequest.Instance.Business.TransInvoice.IsProfitApprovable(pTransID), 1, 0))

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function ApproveProfitability() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInvoiceItemID As String = Common.WebRequest.Data("InvoiceItemID")
            WebRequest.Instance.Business.TransInvoice.ApproveProfitability(pInvoiceItemID, WebSession.Instance.PeopleUserID, True)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Class GetActivityInputJson
            Public ID As String
            Public TransType As String
            Public Serial As String
        End Class

        <HttpPost>
        Public Function GetActivity() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDT As DataTable

            Dim pInput As GetActivityInputJson = JsonConvert.DeserializeObject(Of GetActivityInputJson)(Common.WebRequest.InputStreamText)
            If String.IsNullOrWhiteSpace(pInput.ID) = False _
                AndAlso String.IsNullOrWhiteSpace(pInput.TransType) = False Then

                Select Case CStr2(pInput.TransType).Trim().ToLower()
                    Case "quote"
                        pDT = WebRequest.Instance.Business.TransInvoice.GetActivityQuotation(
                            vID:=pInput.ID,
                            vDontIncludeSalesOrder:=Nothing)

                    Case "salesorder"
                        pDT = WebRequest.Instance.Business.TransInvoice.GetActivitySalesOrder(
                            vID:=pInput.ID,
                            vDontIncludeInvoice:=False,
                            vQuotationID:=Nothing)

                    Case "invoice", ""
                        pDT = WebRequest.Instance.Business.TransInvoice.GetActivityInvoice(pInput.ID, Nothing)

                End Select
                If pDT Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "No data")
                Else
                    TransInvoice.FillElapsedTimeFromPreviousActivity(pDT)
                    Return JsonDictionary.Serialize(pDT)
                End If
            End If

            pDT = WebRequest.Instance.Business.TransInvoice.GetByItemSerialAndStockReceivingItemSerial(pInput.Serial)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "No data")
            End If
            pInput.ID = CStr2(pDT.Rows(0)("ID"))
            pInput.TransType = CStr2(pDT.Rows(0)("TransType"))
            If String.IsNullOrWhiteSpace(pInput.TransType) Then
                pInput.TransType = "invoice"
            End If

            Dim pDt1, pDtAll As DataTable
            For Each pDR As DataRow In pDT.Rows
                pInput.TransType = CStr2(pDR("TransType")).Trim().ToLower()
                pDt1 = Nothing
                Select Case pInput.TransType
                    Case "quote"
                        pDt1 = WebRequest.Instance.Business.TransInvoice.GetActivityQuotation(
                            vID:=pInput.ID,
                            vDontIncludeSalesOrder:=Nothing)

                    Case "salesorder"
                        pDt1 = WebRequest.Instance.Business.TransInvoice.GetActivitySalesOrder(
                            vID:=pInput.ID,
                            vDontIncludeInvoice:=False,
                            vQuotationID:=Nothing)

                    Case "invoice", ""
                        pDt1 = WebRequest.Instance.Business.TransInvoice.GetActivityInvoice(pInput.ID, Nothing)

                    Case "stockreceiving"
                        pDt1 = WebRequest.Instance.Business.TransReceiving.GetActivity(pInput.ID)

                End Select
                If pDt1 IsNot Nothing Then
                    If pDtAll Is Nothing Then
                        pDtAll = pDt1
                    Else
                        For Each pDr1 As DataRow In pDt1.Rows
                            pDtAll.ImportRow(pDr1)
                        Next pDr1
                    End If
                End If
            Next

            If pDtAll Is Nothing Then
                Return JsonDictionary.Serialize("Error", "No data")
            Else
                TransInvoice.FillElapsedTimeFromPreviousActivity(pDtAll)
                Return JsonDictionary.Serialize(pDtAll)
            End If

        End Function

        Public Function GetGrossProfitabilityValidationCount() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Result", WebRequest.Instance.Business.TransInvoice.GetGrossProfitabilityValidationCount())

        End Function

        Public Function TopSellingAndTopProfitableSkus() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDS = WebRequest.Instance.Business.TransInvoice.TopSellingAndTopProfitableSkus(
                vDateFrom:=Common.WebRequest.Data("DateFrom"),
                vDateTo:=Common.WebRequest.Data("DateTo"),
                vGroup:=Common.WebRequest.Data("Group"),
                vFilterField:=Common.WebRequest.Data("Filter"),
                vFilterValue:=Common.WebRequest.Data("FilterValue"))
            Dim pOut = New JsonDictionary()

            pOut.Add("Data", pDS.Tables("Data"))
            pOut.Add("Footer", pDS.Tables("Footer"))

            Return pOut.ToString()

        End Function

        Public Function SetLineItemPrice() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSourceMethod = WebRequest.Instance.Data("SourceMethod")
            If [Enum].GetNames(GetType(TransInvoice.eSetUnitPriceSource)).Contains(pSourceMethod) = False Then
                Return JsonDictionary.Serialize("Error", "Invalid SourceMethod. ref: vbVEJPY8C3HV!")
            End If
            Dim pSourceMethodProper = [Enum].Parse(GetType(TransInvoice.eSetUnitPriceSource), pSourceMethod)

            Dim pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim pVendorID = Common.WebRequest.Data("VendorID")
            Dim pItemID = Common.WebRequest.Data("ItemID")
            Dim pProductID = Common.WebRequest.Data("ProductID")
            If String.IsNullOrWhiteSpace(pItemID) Then
                WebRequest.Instance.Business.TransInvoice.SetLineItemPrice(pInvoiceID, pVendorID, pSourceMethodProper)
            Else
                WebRequest.Instance.Business.TransInvoice.SetLineItemPrice(pInvoiceID, pVendorID, pProductID, pItemID, pSourceMethodProper)
            End If

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Function GetAutoPrintUuid() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary()

            Dim pSql = "SELECT Status" & vbCrLf &
                " FROM LibraryAutoPrint" & vbCrLf &
                " WHERE Uuid = " & ReadyString(WebRequest.Instance.Data("Uuid"))
            Dim pDR = WebRequest.Instance.ProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                pOut.Add("AutoPrintApproval", "none")
            Else
                pOut.Add("AutoPrintApproval", pDR("Status"))
            End If
            pOut.Add("AutoPrintUuid", WebRequest.Instance.Business.SystemDefaults.AutoPrintUuid)

            Return pOut.Serialize()

        End Function

        Public Function SetAutoPrintUuid() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary()
            WebRequest.Instance.Business.SystemDefaults.AutoPrintUuid = WebRequest.Instance.Data("AutoPrintUuid")

            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        End Function

        Public Function RequestAutoPrint() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary()
            Dim pSql = "SELECT Status" & vbCrLf &
                " FROM LibraryAutoPrint" & vbCrLf &
                " WHERE Uuid = " & ReadyString(WebRequest.Instance.Data("Uuid"))
            Dim pDR = WebRequest.Instance.ProData.ExecuteRow(pSql)
            If pDR IsNot Nothing Then
                Return JsonDictionary.Serialize("Status", pDR("Status"))
            End If

            Dim pIU = New InsertUpdate("LibraryAutoPrint")
            pIU("ID") = "NEWID()"
            pIU("RequestedByPeopleID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("Uuid") = ReadyString(WebRequest.Instance.Data("Uuid"))
            pIU("DateRequested") = "GETDATE()"
            pIU("Status") = "'pending'"
            pSql = pIU.GetInsert()
            WebRequest.Instance.ProData.ExecuteSQL(pSql)

            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        End Function

        Public Function GetAutoPrintableInvoice() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary()
            Dim pDT = WebRequest.Instance.Business.TransInvoice.GetAutoPrintableInvoice()
            If pDT.Rows.Count > 0 Then
                pOut.Add("Data", pDT.Rows(0))
            End If
            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        End Function

        Public Function HandleWebPosDeviceFowst() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pUuid = CStr2(Common.WebRequest.Data("UUID"))
            If String.IsNullOrWhiteSpace(pUuid) Then
                Return JsonDictionary.Serialize("Error", "UUID is required. Please use Mobiz Controller to login to Mobiz")
            End If

            Dim pFowstID = CStr2(Common.WebRequest.Data("FowstID"))
            ' commented 2022_06_18__11_15_22_0851
            'If String.IsNullOrWhiteSpace(pFowstID) Then
            '    Return JsonDictionary.Serialize("Error", "FowstID is required. Please choose FOWST first.")
            'End If

            Dim pDR = WebRequest.Instance.Business.LibraryDevice.ByUuid(pUuid)
            If pDR Is Nothing Then
                Return JsonDictionary.Serialize("Error", "UUID not in device library")
            End If
            If CBool2(pDR("IsActive")) = False Then
                Return JsonDictionary.Serialize("Error", "Please request your system administrator to activate device, with UUID " & pUuid & ", before using web POS")
            End If
            Dim pDeviceID = CStr2(pDR("ID"))
            Dim pError As String
            If WebRequest.Instance.Business.LibraryDevice.ValidDeviceFowst(pDeviceID, pFowstID, "pos", pError) Then
                WebPosFlush.SaveDeviceUuid(pUuid)
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

        End Function

        Public Function GetUnapprovedPurchaseOrderCount() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql = " SELECT COUNT(*)" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE TransInvoice.TransType = 'PurchaseOrder'" & vbCrLf &
                " AND TransInvoice.PurchaseOrderNo > 0" & vbCrLf &
                " AND TransInvoice.ApprovedDate IS NULL" & vbCrLf &
                " AND TransInvoice.CancelledByID IS NULL"
            Dim pCount = CInt2(WebRequest.Instance.ProData.ExecuteScalar(pSql))

            Return JsonDictionary.Serialize("Result", pCount)

        End Function

        <HttpPost>
        Public Function ApprovePo() As String

            Dim pInvoiceID As String
            Dim pMessage As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInvoiceID = Common.WebRequest.Data("ID")
            If WebRequest.Instance.Business.TransInvoice.ApprovePo(pInvoiceID, WebSession.Instance.PeopleUserID, pMessage) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pMessage)
            End If

        End Function

        <HttpPost>
        Public Function ReprintRequest() As String

            Dim pInvoiceID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInvoiceID = Common.WebRequest.Data("ID")
            WebRequest.Instance.Business.TransInvoice.RequestReprint(pInvoiceID, Common.WebRequest.Data("Reason"), WebSession.Instance.PeopleUserID)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function ApproveReprintRequest() As String

            Dim pInvoiceID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInvoiceID = Common.WebRequest.Data("ID")
            WebRequest.Instance.Business.TransInvoice.ApproveReprintRequest(pInvoiceID, True, WebSession.Instance.PeopleUserID)
            Return JsonDictionary.Serialize("Result", "ok")


        End Function

        <HttpPost>
        Public Function MergePurchaseOrders() As String

            Dim pInvoiceID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pJson = JsonConvert.DeserializeObject(Of TransInvoice.MergePurchaseOrdersInput)(Common.WebRequest.InputStreamText)
            WebRequest.Instance.Business.TransInvoice.MergePurchaseOrders(pJson)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function TagVendorPriceHistory() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pID = CStr2(Common.WebRequest.Data("ID"))
            Dim pTagged = CBool2(Common.WebRequest.Data("Tagged"))

            Dim pIU = New InsertUpdate("TransVendorPriceHistory")
            If pTagged Then
                pIU("TagByID") = ReadyString(WebSession.Instance.PeopleUserID)
                pIU("TagDate") = "GETDATE()"
            Else
                pIU("TagByID") = "NULL"
                pIU("TagDate") = "NULL"
            End If
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pID))
            pSql = WebRequest.Instance.ProData.ExecuteSQL(pSql) ' output is used for debugging only

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function PriceUpdateRequest() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pIU = New InsertUpdate("TransPriceUpdateRequest")
            pIU("ID") = "NEWID()"
            pIU("ProductID") = ReadyString(Common.WebRequest.Data("ProductID"))
            pIU("RequestedByID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("Remarks") = ReadyString(Truncate(WebRequest.Instance.Data("Remarks"), 500))
            pIU("DateRequested") = "GETDATE()"

            Dim pSql = pIU.GetInsert()
            pSql = WebRequest.Instance.ProData.ExecuteSQL(pSql) ' output is used for debugging only

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        Public Class UpdateItemRequisitionInput
            Public IDs As List(Of String)
        End Class

        <HttpPost>
        Public Function UpdateItemRequisition() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pJson = JsonConvert.DeserializeObject(Of UpdateItemRequisitionInput)(Common.WebRequest.InputStreamText)
            If pJson.IDs Is Nothing _
                OrElse pJson.IDs.Count = 0 Then

                Return JsonDictionary.Serialize("Error", "Please select at least 1 record.")
            End If

            Dim pIDs As String
            For Each pID In pJson.IDs
                If pIDs IsNot Nothing Then
                    pIDs &= ", "
                End If
                pIDs &= ReadyString(pID)
            Next
            Dim pIU = New InsertUpdate("TransItemRequisitions")
            pIU("DateRequested") = "GETDATE()"
            Dim pSql = pIU.GetUpdate("ID IN (" &
                "SELECT TransItemRequisitionBackOrder.ItemRequisitionID" & vbCrLf &
                " FROM TransItemRequisitionBackOrder" & vbCrLf &
                " WHERE TransItemRequisitionBackOrder.BackOrderSourceID IN (" & pIDs & ")" & vbCrLf &
                ")")
            pSql = WebRequest.Instance.ProData.ExecuteSQL(pSql) ' output is used for debugging only

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function GetAverageWeeklyDemand() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pProductID = WebRequest.Instance.Data("ProductID")
            Dim pDT = WebRequest.Instance.Business.TransInvoice.GetAverageWeeklyDemand(pProductID)

            Dim pOut = New JsonDictionary()
            pOut.Add("data", pDT)
            Dim pDrUnit = WebRequest.Instance.Business.LibraryStock.GetFirstUnit(pProductID)
            If pDrUnit IsNot Nothing Then
                pOut.Add("Unit", CStr2(pDrUnit("UnitMeasure")))
            End If

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function VendorPriceHistory() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pVendorID As String = Common.WebRequest.Data("VendorID")
            Dim pProductID As String = Common.WebRequest.Data("ProductID")
            Dim pDT As DataTable

            If CStr2(pVendorID).Trim() = "" Then
                Return JsonDictionary.Serialize("Error", "VendorID must not be blank. ref: vb85Y4V86BG8!")
            End If

            If CStr2(pProductID).Trim() = "" Then
                Return JsonDictionary.Serialize("Error", "ProductID must not be blank. ref: vbDKZBEA9ZGK!")
            End If

            pDT = WebRequest.Instance.Business.TransInvoice.VendorPriceHistory(pVendorID, pProductID)

            Return JsonDictionary.Serialize("Result", pDT)

        End Function

        Public Function CreateBirExcel() As String

            Dim pPurchaseOrderMode As Boolean
            Dim pGroupByCustomer = CBool2(WebRequest.Instance.Data("GroupByCustomer"))
            Dim pAllCustomers = CBool2(WebRequest.Instance.Data("AllCustomers"))
            Dim pCustomerID = WebRequest.Instance.Data("CustomerID")
            If pAllCustomers = False Then
                If String.IsNullOrWhiteSpace(pCustomerID) Then
                    Return JsonDictionary.Serialize("Error", "CustomerID is required. ref:vbYSGDYD6HUW!")
                End If
                If IsGuid(pCustomerID) = False Then
                    Return JsonDictionary.Serialize("Error", "CustomerID must be a GUID. ref:vbULKTLSZU8G!")
                End If
            End If

            Dim pArrInvoiceTypeIDs = WebRequest.Instance.AllData("InvoiceTypeIDs")
            If pArrInvoiceTypeIDs Is Nothing _
                OrElse pArrInvoiceTypeIDs.length = 0 Then

                Return JsonDictionary.Serialize("Error", "InvoiceTypeIDs is required. ref:vbBNHU2LK3CR!")
            End If
            Dim pInvoiceTypeIDs = ""
            For Each pItem In pArrInvoiceTypeIDs
                If pItem = "po" Then
                    pPurchaseOrderMode = True
                ElseIf IsGuid(pItem) = False Then
                    Return JsonDictionary.Serialize("Error", "InvoiceTypeID must be a GUID. ref:vbXYDBD8BQJD!")
                End If

                If String.IsNullOrWhiteSpace(pInvoiceTypeIDs) = False Then
                    pInvoiceTypeIDs &= ", "
                End If

                pInvoiceTypeIDs &= ReadyString(pItem)
            Next

            Dim pInvoicDateFrom = WebRequest.Instance.Data("InvoiceDateFrom")
            If String.IsNullOrWhiteSpace(pInvoicDateFrom) Then
                Return JsonDictionary.Serialize("Error", "InvoiceDateFrom is required.")
            End If
            If IsDate(pInvoicDateFrom) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceDateFrom must be a valid date.")
            End If
            pInvoicDateFrom = ParseDate(pInvoicDateFrom).Value.Date.ToString(cGlobal.FormatDateTimeSql)

            Dim pInvoicDateTo = WebRequest.Instance.Data("InvoiceDateTo")
            If String.IsNullOrWhiteSpace(pInvoicDateTo) Then
                Return JsonDictionary.Serialize("Error", "InvoiceDateTo is required.")
            End If
            If IsDate(pInvoicDateTo) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceDateTo must be a valid date.")
            End If
            pInvoicDateTo = ParseDate(pInvoicDateTo).Value.Date.ToString(cGlobal.FormatDateTimeSql)

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String

            If pPurchaseOrderMode Then
                pSql = "  -- bir report group by vendor" & vbCrLf &
                    "  SELECT TOP 5000 (CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [Customer]" & vbCrLf &
                    "  ,MobizLibraryOrganization.TinNumber AS TIN" & vbCrLf &
                    "  ,SUM(TransInvoice.TotalAmount) AS [VAT IN Gross]" & vbCrLf &
                    "  ,SUM(TransInvoice.TotalAmount) / 1.12 AS [VAT EX Net]" & vbCrLf &
                    "  ,SUM(TransInvoice.TotalAmount) - SUM(TransInvoice.TotalAmount) / 1.12 AS VAT" & vbCrLf &
                    "  FROM TransInvoice" & vbCrLf &
                    "  JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
                    "  OUTER APPLY (" & vbCrLf &
                    "  	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
                    "      ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS OrderedAmount" & vbCrLf &
                    "      ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS ServedAmount" & vbCrLf &
                    "      ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
                    "    	FROM TransInvoiceItems" & vbCrLf &
                    "      WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                    "      AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
                    "      OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                    "      AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
                    "  ) tblItems" & vbCrLf &
                    "  WHERE ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
                    "  AND TransInvoice.PurchaseOrderNo > 0" & vbCrLf &
                    "  AND TransInvoice.TransType = 'PurchaseOrder'" & vbCrLf &
                    "  AND CONVERT(date,TransInvoice.PurchaseOrderDate) BETWEEN " & ReadyString(pInvoicDateFrom) & " AND " & ReadyString(pInvoicDateTo) & vbCrLf
                If String.IsNullOrWhiteSpace(pCustomerID) = False Then
                    pSql &= " AND TransInvoice.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
                End If
                pSql &= "  GROUP BY MobizLibraryOrganization.IsOrganization" & vbCrLf &
                    "  ,MobizLibraryOrganization.CompanyName" & vbCrLf &
                    "  ,MobizLibraryOrganization.PersonSurname" & vbCrLf &
                    "  ,MobizLibraryOrganization.PersonFirstName" & vbCrLf &
                    "  ,MobizLibraryOrganization.PersonMiddleName" & vbCrLf &
                    "  ,MobizLibraryOrganization.PersonSuffixName" & vbCrLf &
                    "  ,MobizLibraryOrganization.TinNumber" & vbCrLf &
                    "  HAVING ISNULL(SUM(TransInvoice.TotalAmount), 0) <> 0" & vbCrLf &
                    "  ORDER BY (CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END)"
            ElseIf pGroupByCustomer Then
                pSql = " -- bir report group by customer" & vbCrLf &
                " SELECT TOP 5000 (CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [Customer]" & vbCrLf &
                " ,MobizLibraryOrganization.TinNumber AS TIN" & vbCrLf &
                " ,SUM(TransInvoice.TotalAmount) AS [VAT IN Gross]" & vbCrLf &
                " ,SUM(CacheInvoices.AdjustmentAmount) AS [Adjustments]" & vbCrLf &
                " ,SUM(CacheInvoices.NetInvoiceAmount) AS [VAT IN Net]" & vbCrLf &
                " ,SUM(CacheInvoices.NetInvoiceAmount) / 1.12 AS [VAT EX Net]" & vbCrLf &
                " ,SUM(CacheInvoices.NetInvoiceAmount) - SUM(CacheInvoices.NetInvoiceAmount) / 1.12 AS VAT" & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " LEFT JOIN CacheInvoices ON CacheInvoices.ID = TransInvoice .ID" & vbCrLf &
                " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
                " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
                " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
                "     ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS OrderedAmount" & vbCrLf &
                "     ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS ServedAmount" & vbCrLf &
                "     ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
                "   	FROM TransInvoiceItems" & vbCrLf &
                "     WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                "     AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
                "     OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                "     AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
                " ) tblItems" & vbCrLf &
                " WHERE ISNULL(TransInvoice.IsCancelled, 0) = 0" & vbCrLf &
                " AND TransInvoice.TransType = 'Invoice'" & vbCrLf &
                " AND CONVERT(date,TransInvoice.InvoiceDate) BETWEEN " & ReadyString(pInvoicDateFrom) & " AND " & ReadyString(pInvoicDateTo) & vbCrLf &
                " AND TransInvoice.DocumentNameID IN (" & pInvoiceTypeIDs & ")" & vbCrLf &
                " AND TransInvoice.InvoiceNumber > 0" & vbCrLf

                If String.IsNullOrWhiteSpace(pCustomerID) = False Then
                    pSql &= "  AND TransInvoice.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
                End If

                pSql &= " GROUP BY MobizLibraryOrganization.IsOrganization" & vbCrLf &
                    " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonSurname" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonFirstName" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonMiddleName" & vbCrLf &
                    " ,MobizLibraryOrganization.PersonSuffixName" & vbCrLf &
                    " ,MobizLibraryOrganization.TinNumber" & vbCrLf &
                    " HAVING ISNULL(SUM(TransInvoice.TotalAmount), 0) <> 0" & vbCrLf &
                    " OR ISNULL(SUM(CacheInvoices.NetInvoiceAmount), 0) <> 0" & vbCrLf &
                    " ORDER BY (CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END)"
            Else
                pSql = " SELECT TOP 500 --" & vbCrLf &
                " -- , TransInvoice.RefInvoiceNum AS [RefInvoiceNum]" & vbCrLf &
                " TransInvoice.ID AS InvoiceID" & vbCrLf &
                "   ,(CASE MobizLibraryOrganization.IsOrganization      WHEN 1 THEN  MobizLibraryOrganization.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '')  END) AS [Customer]" & vbCrLf &
                "  ,MobizLibraryOrganization.TinNumber AS TIN" & vbCrLf &
                " , LibraryDocumentControl.DocumentName + '-' + CONVERT(varchar, ISNULL(MobizLibraryOrganizationFOWST.FOWSTNo, 0)) + '-' + CONVERT(varchar, ISNULL(LibraryDevice.DeviceNo, 0)) + '-' + CONVERT(varchar, ISNULL(TransInvoice.InvoiceNumber,0))  AS [InvoiceNumber]" & vbCrLf & vbCrLf &
                " , TransInvoice.RefInvoiceNum AS [ManualInvoiceNumber]" & vbCrLf &
                " , TransInvoice.InvoiceDate AS [InvoiceDate]" & vbCrLf &
                " , TransInvoice.TotalAmount AS [InvoiceAmount]" & vbCrLf &
                " ,CacheInvoices.AdjustmentAmount AS [Adjustments]" & vbCrLf &
                " ,CacheInvoices.NetInvoiceAmount AS [NetOfAdjustments]" & vbCrLf &
                " ,CacheInvoices.NetInvoiceAmount / 1.12 AS [VAT EX Net]" & vbCrLf &
                " ,CacheInvoices.NetInvoiceAmount - CacheInvoices.NetInvoiceAmount / 1.12 AS VAT" & vbCrLf &
                " --, TransInvoice.DateCreated AS [DateCreated]" & vbCrLf &
                " --, TransInvoice.TotalAmount" & vbCrLf &
                " --, TransInvoice.LastUpdated AS [LastUpdated]" & vbCrLf &
                "-- , tblFowstCustomer.Name AS [CustomerFowst]" & vbCrLf &
                "-- , tblBinCustomer.Name AS [CustomerBin]" & vbCrLf &
                "-- , tblBackorderInvoice.BackorderSourceMinSalesOrderNo AS [BackorderSourceMinSalesOrderNo]" & vbCrLf &
                "-- , tblBackorderInvoice.BackorderSourceSalesOrderDate AS [BackorderSourceSalesOrderDate]" & vbCrLf &
                " --, TransInvoice.IsCancelled AS [IsCancelled]" & vbCrLf &
                " --, TransInvoice.TransType AS [TransType]" & vbCrLf &
                " /*" & vbCrLf &
                " , ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
                "  + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf &
                "  + ',' + ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
                "  + ',' + ISNULL(tblDeliveryCountry.Name, '')" & vbCrLf &
                "  AS [DeliveryAddress]" & vbCrLf &
                " , ISNULL(tblDelivery.StreetNumber, '')" & vbCrLf &
                "  + ',' + ISNULL(tblDelivery.Street, '')" & vbCrLf &
                "  AS [DeliveryStreet]" & vbCrLf &
                " , ISNULL(tblDeliveryCity.Name, '')" & vbCrLf &
                "  + ',' + ISNULL(tblDeliveryCountry.Name, '')" & vbCrLf &
                "  AS [DeliveryCity]" & vbCrLf &
                " -- , TransInvoice.StrNum AS [StrNum]" & vbCrLf &
                " -- , TransInvoice.IsDelivered AS [IsDelivered]" & vbCrLf &
                " -- , tblParentOrder.ParentSalesOrderNo AS [ParentSalesOrderNo]" & vbCrLf &
                " -- , tblInvoiceOrder.InvoiceSalesOrderNo AS [InvoiceSalesOrderNo]" & vbCrLf &
                " -- , TransInvoice.PoReceivedPercent AS [PoReceivedPercent]" & vbCrLf &
                " */" & vbCrLf &
                "   FROM TransInvoice TransInvoice LEFT JOIN" & vbCrLf &
                "  MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID" & vbCrLf &
                " LEFT JOIN CacheInvoices ON CacheInvoices.ID = TransInvoice .ID" & vbCrLf &
                "  LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN" & vbCrLf &
                "  MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganizationFOWST tblEncodingFowst ON TransInvoice.EncodingFowstID = tblEncodingFowst.ID" & vbCrLf &
                "  JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
                "  LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
                "  OUTER APPLY (" & vbCrLf &
                "  	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf &
                "    ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity) AS OrderedAmount" & vbCrLf &
                "    ,SUM(TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity) AS ServedAmount" & vbCrLf &
                "    ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf &
                "  	FROM TransInvoiceItems" & vbCrLf &
                "    WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                "    AND (TransInvoiceItems.Quantity > 0" & vbCrLf &
                "    OR TransInvoiceItems.OrderedQuantity > 0)" & vbCrLf &
                "    AND ISNULL(TransInvoiceItems.HiddenServedQuantity, 0) = 0" & vbCrLf &
                "  ) tblItems    " & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganizationFOWST tblFowstCustomer ON tblFowstCustomer.ID = TransInvoice.CustomerFowstID" & vbCrLf &
                "  LEFT JOIN LibraryFowstBin tblBinCustomer ON tblBinCustomer.ID = TransInvoice.CustomerBinID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransInvoice.DeliveryAddressID" & vbCrLf &
                "  LEFT JOIN MobizLibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID" & vbCrLf &
                "  LEFT JOIN MobizLibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID" & vbCrLf &
                "  OUTER  APPLY (" & vbCrLf &
                "  	SELECT SUM(TransInvoiceItems.SubTotal) AS TotalOrder" & vbCrLf &
                "    ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCountOrder" & vbCrLf &
                "  	FROM TransInvoiceItems" & vbCrLf &
                "    WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf &
                "    AND TransInvoiceItems.OrderedQuantity > 0" & vbCrLf &
                "  ) tblItemsOrder    " & vbCrLf &
                "  OUTER APPLY (" & vbCrLf &
                "   	SELECT 'SO-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblBackorderSalesOrder.SalesOrderNo) AS BackorderSourceMinSalesOrderNo" & vbCrLf &
                "      ,tblBackorderSalesOrder.SalesOrderDate AS BackorderSourceSalesOrderDate" & vbCrLf &
                "      FROM TransInvoice tblBackorderInvoice" & vbCrLf &
                "      JOIN TransInvoice tblBackorderSalesOrder ON tblBackorderSalesOrder.ID = tblBackorderInvoice.SalesOrderID" & vbCrLf &
                "      LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.id = tblBackorderSalesOrder.FowstID" & vbCrLf &
                "      LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
                "      WHERE tblBackOrderInvoice.ID = TransInvoice.BackorderSourceInvoiceID" & vbCrLf &
                "   ) tblBackorderInvoice OUTER APPLY (" & vbCrLf &
                "   	SELECT 'SO-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblParent.SalesOrderNo) AS ParentSalesOrderNo" & vbCrLf &
                "      FROM TransInvoice tblParent" & vbCrLf &
                "      LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.id = tblParent.FowstID" & vbCrLf &
                "      LEFT JOIN LibraryDevice ON tblParent.DeviceID = LibraryDevice.ID" & vbCrLf &
                "      WHERE tblParent.ID = TransInvoice.ParentOrderID" & vbCrLf &
                "   ) tblParentOrder" & vbCrLf &
                "  OUTER APPLY (" & vbCrLf &
                "   	SELECT 'SO-' + CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, tblParent.SalesOrderNo) AS InvoiceSalesOrderNo" & vbCrLf &
                "      FROM TransInvoice tblParent" & vbCrLf &
                "      LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.id = tblParent.FowstID" & vbCrLf &
                "      LEFT JOIN LibraryDevice ON tblParent.DeviceID = LibraryDevice.ID" & vbCrLf &
                "      WHERE tblParent.ID = TransInvoice.SalesOrderID" & vbCrLf &
                "   ) tblInvoiceOrder" & vbCrLf &
                "   WHERE 1=1" & vbCrLf &
                "  AND TransInvoice.IsCancelled = 0" & vbCrLf &
                "  AND TransInvoice.TransType = 'Invoice'" & vbCrLf &
                " AND TransInvoice.InvoiceNumber > 0" & vbCrLf

                If String.IsNullOrWhiteSpace(pCustomerID) = False Then
                    pSql &= "  AND TransInvoice.CustomerID = " & ReadyString(pCustomerID) & vbCrLf
                End If
                pSql &= " AND CONVERT(date,TransInvoice.InvoiceDate) BETWEEN " & ReadyString(pInvoicDateFrom) & " AND " & ReadyString(pInvoicDateTo) & vbCrLf &
                    " AND TransInvoice.DocumentNameID IN (" & pInvoiceTypeIDs & ")" & vbCrLf &
                    "     ORDER BY TransInvoice.InvoiceDate" & vbCrLf &
                    " ,TransInvoice.LastUpdated"
            End If

            Dim pDT = WebRequest.Instance.ProData.Execute(pSql)
            Dim pKey = DateTimeStamp()
            Dim pFilename = Common.ServerPath & "temp\BirExcel\BirExcel_" & pKey & ".xlsx"
            CreateFolder(pFilename)
            TableToExcelFile(pDT, "", "", pFilename)

            Return JsonDictionary.Serialize("Key", pKey)

        End Function

        Public Function DownloadBirExcel() As ActionResult

            Dim pFilename = Common.ServerPath & "temp\BirExcel\BirExcel_" & WebRequest.Instance.QueryString("Key") & ".xlsx"

            Return File(pFilename, "application/xlsx", "BirExcel_" & WebRequest.Instance.QueryString("Key") & ".xlsx")

        End Function

    End Class
End Namespace