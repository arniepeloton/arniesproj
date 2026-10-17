Imports Newtonsoft.Json

Namespace WebProj
    Public Class ReceivingController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Receiving

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Receiving and Transfers")) Then Exit Function
            End If

            ' ''If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function
            Return PartialView("~/Views/Receiving/BsIndex.vbhtml")
            'Return View()
        End Function

        Function BsIndex() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Receiving and Transfers")) Then Exit Function
            End If

            ' ''If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

            Return View()
        End Function

        'Function InventoryReceivingAndTransfersReport() As ActionResult

        '    If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
        '        If WebRequest.Instance.HandleAccess = False Then Exit Function
        '    End If

        '    Return View()

        'End Function

        Function StockViewer() As ActionResult
            'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

            'If (Not SecurityRole.Instance.HandleRoleAccess("Receiving and Transfers")) Then Exit Function

            Return View()
        End Function

        Function GeneratePDF() As ActionResult

            Return View()
        End Function

        <HttpPost()>
        Function GetAllReceiving() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetAllReceiving())
        End Function

        <HttpPost()>
        Function GetAllReceivingSKU() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetAllReceivingSKU())
        End Function

        <HttpPost()>
        Function GetStockUnitMeasures() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("UnitMeasure", s.GetStockUnitMeasures())
        End Function

        <HttpPost()>
        Function SaveReceiving() As String
            Dim s As New Receiving
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Save", s.SaveReceiving(WebRequest.Instance.Data("ID"), CBool(WebRequest.Instance.Data("IsNew"))))

        End Function

        <HttpPost()>
        Function GetOrganizationID()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("OrganizationID", WebSession.Instance.SubscriberOrganizationID)
        End Function

        Function GetReportHeaderInfo() As String
            Dim Receiving As New Receiving

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", Receiving.GetReportHeaderInfo)

        End Function

        Function ReceivingAndTransfersReport() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Receiving and Transfers")) Then Exit Function
            End If

            ' ''If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function

            Return View()
        End Function

        Function GetReceivingAndTransfersReport() As String
            Dim Receiving As New Receiving

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", Receiving.GetReceivingAndTransfersReport)
        End Function

        Function GetReceivingAndTransfersSubReport() As String
            Dim Receiving As New Receiving

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", Receiving.GetReceivingAndTransfersSubReport)
        End Function

        Function GetReceivingAndTransfersDiscrepancyReport() As String
            Dim Receiving As New Receiving

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", Receiving.GetReceivingAndTransfersDiscrepancyReport)
        End Function

        Function SaveProductBarcode() As String
            Dim Receiving As New Receiving

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", Receiving.SaveProductBarcode)
        End Function

        Function GetSystemBehaviorSettings() As String
            Dim Receiving As New Receiving

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", Receiving.GetSystemBehaviorSettings)
        End Function

        <HttpPost>
        Public Function GetStockInfo() As String

            Dim pStockInfo As TransReceiving.StockInfo
            Dim pProductID As String = Common.WebRequest.Data("ProductID")
            Dim pTransID As String = Common.WebRequest.Data("TransID")
            Dim pReceivingStockID As String = Common.WebRequest.Data("ReceivingStockID")

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pStockInfo = WebRequest.Instance.Business.TransReceiving.GetStockInfo(pProductID, pTransID, pReceivingStockID)

            Return JsonConvert.SerializeObject(pStockInfo)

        End Function

        <HttpPost>
        Public Function Save() As String

            Dim pSaveJson As TransReceiving.SaveJson
            Dim pOut As TransReceiving.SaveOutput
            Dim pError As String
            Dim pNonZeroItemCount As Integer

            Try
                pSaveJson = JsonConvert.DeserializeObject(Of TransReceiving.SaveJson)(WebRequest.Instance.InputStreamText)
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
                pSaveJson.Receiving.UserID = WebSession.Instance.PeopleUserID
                If pSaveJson.Receiving IsNot Nothing Then
                    If pSaveJson.Receiving.Verify(pError) = False Then
                        Return JsonDictionary.Serialize("Error", pError)
                    End If
                    If pSaveJson.Receiving.SaveAndFinalizeProper Then
                        pNonZeroItemCount = WebRequest.Instance.Business.TransReceiving.NonZeroItemCount(pSaveJson.Receiving.ID)
                        If pNonZeroItemCount = 0 Then
                            Return JsonDictionary.Serialize("Error", "Finalized transactions must have at least 1 item with a quantity that is greater than zero.")
                        End If
                    End If
                End If
                pOut = WebRequest.Instance.Business.TransReceiving.Save(pSaveJson, WebRequest.Instance.OrgID)
                Return JsonConvert.SerializeObject(pOut)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function GetData() As String

            Dim pID As String
            Dim pDT As DataTable
            Dim pOut As JsonDictionary
            Dim pCreateFromPoID As String = Common.WebRequest.Data("CreateFromPoID")
            Dim pUuid As String = Common.WebRequest.Data("UUID")
            Dim pDeviceID As String = Common.WebRequest.Data("DeviceID")
            Dim pFowstID As String = Common.WebRequest.Data("FowstID")
            Dim pError As String
            Dim pDR As DataRow

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If String.IsNullOrWhiteSpace(pCreateFromPoID) Then
                pID = Common.WebRequest.Data("ID")
            Else
                pDeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
                If IsGuid(pDeviceID) = False Then
                    Return JsonDictionary.Serialize("Error", "DeviceID must be a GUID. ref: jsHZ4FHKDFN9!")
                End If
                If IsGuid(pFowstID) = False Then
                    Return JsonDictionary.Serialize("Error", "FowstID must be a GUID. ref: jsH9QE7SDSSK!")
                End If
                pID = WebRequest.Instance.Business.TransReceiving.CreateFromPo( _
                    vInvoiceID:=pCreateFromPoID,
                    vFowstID:=pFowstID,
                    vDeviceID:=pDeviceID,
                    vUserID:=WebSession.Instance.PeopleUserID,
                    oError:=pError
                )
                If String.IsNullOrWhiteSpace(pError) = False Then
                    Return JsonDictionary.Serialize("Error", pError)
                End If
            End If
            If IsGuid(pID) = False Then
                Return JsonDictionary.Serialize("Error", "ID must be GUID. ref: vbE3ET3AFUYR!")
            End If

            pDT = WebRequest.Instance.Business.TransReceiving.GetByID(pID)
            pOut = New JsonDictionary()
            If pDT.Rows.Count > 0 Then
                pOut.Add("Master", pDT.Rows(0))
                pDT = WebRequest.Instance.Business.TransReceiving.GetStocks(pID)
                pOut.Add("Products", pDT)
            End If

            Return pOut.Serialize()

        End Function

        Public Function CreatePdf()

            Dim pReceivingID As String
            Dim pReport As ProReport
            Dim pFilename As String
            Dim pHeaderInfo As LibraryOrganization.HeaderInfo
            'Dim pUuid As String ' delete this code 2021_12_21__15_32_32_9912
            Dim pDT As DataTable
            Dim pAccountsPayableID As String
            Dim pOut As New JsonDictionary()

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ' BEGIN delete this code 2021_12_21__15_32_32_9912
            'pUuid = Common.WebRequest.Data("UUID")
            'If String.IsNullOrWhiteSpace(pUuid) Then
            '    Return JsonDictionary.Serialize("Error", "UUID is required.")
            'End If
            'If pUuid.Length < 1 Then
            '    Return JsonDictionary.Serialize("Error", "UUID must be at least 1 character in length.")
            'End If
            ' END delete this code 2021_12_21__15_32_32_9912

            pAccountsPayableID = Common.WebRequest.Data("AccountsPayableID")
            pReceivingID = Common.WebRequest.Data("ReceivingID")
            If String.IsNullOrWhiteSpace(pReceivingID) Then
                Dim pDR = WebRequest.Instance.Business.TransReceiving.GetByAccountsPayableID(pAccountsPayableID)
                If pDR Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "Cant find receiving with AccountsPayableID" & pAccountsPayableID & ".")
                End If
                pReceivingID = CStr2(pDR("ID"))
            End If
            pDT = WebRequest.Instance.Business.TransReceiving.GetByID(pReceivingID)
            If pDT.Rows.Count = 0 Then
                Return JsonDictionary.Serialize("Error", "Transaction does not exist.")
            End If

            'If WebRequest.Instance.Business.TransInvoice.IsSavedAsFinal(pDR) = False Then
            '    Return JsonDictionary.Serialize("Error", "Only transactions that were saved as final can be printed.")
            'End If
         
            pReport = New ProReport
            pReport.AutoFillHeader = False
            If Common.WebRequest.Data("Template") = "StockReceivingAndPO" Then
                pReport.SourceFilename = "StockReceivingAndPO"
            ElseIf Common.WebRequest.Data("TransType") = "StockTransfer" Then
                pReport.SourceFilename = "StockTransfer"
            Else
                pReport.SourceFilename = "StockReceiving"
            End If
            pReport.AddTable("Master", pDT)
            If pDT.Rows.Count > 0 Then
                pDT = WebRequest.Instance.Business.TransReceiving.GetStocks(pReceivingID, pAccountsPayableID)
                pReport.AddTable("Stocks", pDT)
            End If

            pHeaderInfo = WebRequest.Instance.Business.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
            pReport.Variables("CompanyName") = pHeaderInfo.CompanyName
            pReport.Variables("CompanyAddress") = pHeaderInfo.CompanyAddress
            pReport.Variables("CompanyContactInfos") = pHeaderInfo.ContactInfos

            pReport.ExportXmlFile()

            pFilename = DateTimeStamp() & "-" & GenerateString(10) & ".pdf"
            pOut.Add("BaseFilename", pFilename)
            pFilename = Common.ServerPath & "temp\PrintablePdf\" & pFilename
            CreateFolder(pFilename)
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function DeleteItem() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pID = Common.WebRequest.Data("ID")
            If String.IsNullOrWhiteSpace(pID) Then
                Return JsonDictionary.Serialize("Error", "ID must not be blank. ref: vbJE9272WAJQ!")
            End If

            Dim pReceivingID = Common.WebRequest.Data("ReceivingID")
            If String.IsNullOrWhiteSpace(pReceivingID) Then
                Return JsonDictionary.Serialize("Error", "ID must not be blank. ref: vbE7YWF6XZFM!")
            End If

            Dim pProductID = Common.WebRequest.Data("ProductID")
            If String.IsNullOrWhiteSpace(pProductID) Then
                Return JsonDictionary.Serialize("Error", "ProductID must not be blank. ref: vbVNT2ZPYP53!")
            End If

            Dim pUnitMeasureID = Common.WebRequest.Data("UnitMeasureID")
            If String.IsNullOrWhiteSpace(pUnitMeasureID) Then
                Return JsonDictionary.Serialize("Error", "UnitMeasureID must not be blank. ref: vbSHTWX4UZ5V!")
            End If

            Dim pDrReceiving = WebRequest.Instance.Business.TransReceiving.ByID(pReceivingID)
            If pDrReceiving Is Nothing Then
                Return JsonDictionary.Serialize("Error", "The specified receiving does not exist. ref: vb7M6SBVKQQJ!")
            End If

            If CBool2(pDrReceiving("IsCancelled")) Then
                Return JsonDictionary.Serialize("Error", "This stock receiving is already cancelled.")
            End If

            If CInt2(pDrReceiving("ReceivingNo")) <> 0 Then
                Return JsonDictionary.Serialize("Error", "You may not remove items from a finalized transaction.")
            End If


            Dim pCount = WebRequest.Instance.Business.TransReceiving.DeleteItem(pReceivingID, pID, pProductID, pUnitMeasureID)
            If pCount = 0 Then
                Return JsonDictionary.Serialize("Error", "Nothing was delete. Please reload the transaction.")
            End If

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function CancelReceiving() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pError As String
            Dim pReceivingID As String = Common.WebRequest.Data("ReceivingID")

            If CStr2(pReceivingID).Trim() = "" Then
                Return JsonDictionary.Serialize("Error", "ReceivingID must not be blank. ref: vbGYKF747YDN!")
            End If

            If WebRequest.Instance.Business.TransReceiving.CancelReceiving(pReceivingID, WebSession.Instance.PeopleUserID, WebRequest.Instance.OrgID, pError) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

        End Function

        <HttpPost>
        Public Function GetCacheInvoice() As String

            Dim pInvoiceID As String
            Dim pDrInvoice As DataRow
            Dim pOut As JsonDictionary
            Dim pSql As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInvoiceID = Common.WebRequest.Data("AccountsPayableID")

            pOut = New JsonDictionary

            pSql = "SELECT CacheReceivings.* " & vbCrLf &
                " ,TransReceiving.PurchaseOrderID" & vbCrLf &
                " FROM CacheReceivings" & vbCrLf &
                " JOIN TransReceiving ON TransReceiving.ID = CacheReceivings.ID" & vbCrLf &
                " WHERE TransReceiving.AccountsPayableID = " & ReadyString(pInvoiceID)
            pDrInvoice = Common.WebRequest.ProData.ExecuteRow(pSql)
            pOut.Add("Invoice", pDrInvoice)
            pOut.Add("Adjustments", WebRequest.Instance.Business.TransInvoice.GetAdjustments(pInvoiceID))

            Return pOut.Serialize

        End Function

        Function GetStockTransferReport() As String
            Dim receiving As New Receiving
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary

            pOut.Add("Results", receiving.GetStockTransferReport())

            Return pOut.Serialize

        End Function

        Function GetReceivingInvoiceRefImages()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim id As String = CStr2(Common.WebRequest.Data("ReceivingID"))
            Dim transtype As String = Common.WebRequest.Data("TransType")
            Dim pSql As String

            If transtype = "AccountsPayable" Then
                pSql = "SELECT ID FROM TransReceiving WHERE AccountsPayableID = " & ReadyString(id)
                id = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            ElseIf transtype = "Expense" Then
                pSql = "SELECT ID FROM TransReceiving WHERE InvoiceNumber = " & ReadyString("Expense #" & Common.WebRequest.Data("ExpenseNumber"))
                id = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

                If String.IsNullOrEmpty(id) Then
                    'pwede mag attach og Invoice ref kahit draft or wala pay nacreate na invoice sa AP mod.
                    id = CStr2(Common.WebRequest.Data("ReceivingID"))
                Else
                    'kung naa na - ioverwrite lang ang ReceivingID (id sa ET) to ID sa AP, para makuha ang inv img kahit sa ET or AP mod
                    Dim p As New InsertUpdate("TransReceivingInvoiceRefImages")
                    p("ReceivingID") = ReadyString(id)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ReceivingID = " & ReadyString(CStr2(Common.WebRequest.Data("ReceivingID")))))
                End If
            End If

            If String.IsNullOrEmpty(id) Then
                Return JsonDictionary.Serialize("Results", "")
            End If

            pSql = "SELECT files.*, ppol.FirstName + ' ' + ppol.Surname as AddedByName " & vbCrLf &
                    " FROM TransReceivingInvoiceRefImages as trans " & vbCrLf &
                    " INNER JOIN LibraryCRMAttachments as files ON files.ID = trans.CRMAttachmentID " & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = trans.AddedBy " & vbCrLf &
                    " WHERE trans.ReceivingID = " & ReadyString(id)

            Dim pOut As New JsonDictionary
            pOut.Add("ReceivingID", id)
            pOut.Add("Results", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function

        Function AttachReceivingInvoiceRefImages()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim files As Object = Common.WebRequest.AllData("Files")
                Dim recid As String = Common.WebRequest.Data("ReceivingID")
                Dim supplierid As String = Common.WebRequest.Data("SupplierID")
                Dim p As New InsertUpdate("TransReceivingInvoiceRefImages")

                If recid = "" Then
                    Return JsonDictionary.Serialize("Results", "Must be a draft of finalized transaction")
                End If

                'commented by Arnie 7-14-2023, since pwede na DAW mag-attach og reference gamit ang other payment method
                'If supplierid = "" Then
                '    Return JsonDictionary.Serialize("Results", "Please select a Vendor first")
                'End If

                If files.Length = 0 Then
                    Return JsonDictionary.Serialize("Results", "Please browse a Vendor's Invoice Image")
                End If

                Common.WebRequest.ProData.BeginTransaction()

                Dim pSql As String

                If Common.WebRequest.Data("TransType") = "AccountsPayable" Then
                    pSql = "SELECT ID FROM TransReceiving WHERE AccountsPayableID = " & ReadyString(recid)
                    recid = Common.WebRequest.ProData.Execute(pSql).Rows(0)("ID").ToString()
                End If

                For Each f In files
                    p = New InsertUpdate("TransReceivingInvoiceRefImages")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("ReceivingID") = ReadyString(recid)
                    p("SupplierID") = IIf(String.IsNullOrWhiteSpace(supplierid), "NULL", ReadyString(supplierid))

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" CRMAttachmentID = " & ReadyString(f("ID").ToString()))) = 0 Then
                        p("LastUpdated") = "getdate()"
                        p("AddedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("CRMAttachmentID") = ReadyString(f("ID").ToString())
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

    End Class
End Namespace