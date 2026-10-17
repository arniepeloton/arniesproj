Imports System.Threading
Imports Newtonsoft.Json

Namespace WebProj
    Public Class CreditAndCollectionController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function Payments() As ActionResult
            Return View()
        End Function

        Function OnlineSOA() As ActionResult
            Return View()
        End Function

        Function InvalidSOALink() As ActionResult
            Dim pDT As Object = Common.WebRequest.ProData.Execute("SELECT CompanyName FROM MobizLibraryOrganization WHERE ID = " & ReadyString(Request.Params.Get("o"))).Rows

            If pDT.Count > 0 Then
                ViewData("Name") = pDT(0)("CompanyName").ToString()
            Else
                ViewData("Name") = ""
            End If

            Return View()
        End Function

        Function TagAsAttachedOnEarlierSOA()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("TransInvoice")

                p("IsAttachedOnEarlierSOA") = 1

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("InvoiceID"))))

                Return JsonDictionary.Serialize("Results", "ok")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function UploadInvoiceAttachments() As String
            If Request.UrlReferrer.AbsolutePath.ToLower <> "/creditandcollection/onlinesoa" Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function

            End If
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim files As HttpFileCollectionBase = Request.Files
            Dim InvoiceID As String = Request.Params.Get("InvoiceID").ToString()
            Dim folderName As String = Common.ServerPath & "InvoicesFiles\" & WebRequest.Instance.OrgID & "\" & InvoiceID

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim p As New InsertUpdate("TransInvoiceFiles")
                    Dim id As String = Guid.NewGuid.ToString()
                    Dim filename As String = id & "_" & file.FileName

                    p("ID") = ReadyString(id)
                    p("InvoiceID") = ReadyString(InvoiceID)
                    p("FileName") = ReadyString(filename)
                    p("DateUploaded") = "getdate()"
                    p("UploadedBy") = ReadyString(If(WebSession.Instance.PeopleUserID Is Nothing, Guid.Empty.ToString(), WebSession.Instance.PeopleUserID))

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    Dim pSourceURL As String = folderName & "\" & filename
                    file.SaveAs(pSourceURL)
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function UploadInvoiceImageWebCam()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim imageData As String = Common.WebRequest.Data("imageData")
                Dim InvoiceID As String = Common.WebRequest.Data("InvID")
                Dim folderName As String = Common.ServerPath & "InvoicesFiles\" & WebRequest.Instance.OrgID & "\" & InvoiceID

                If Not System.IO.Directory.Exists(folderName) Then
                    System.IO.Directory.CreateDirectory(folderName)
                End If

                Dim p As New InsertUpdate("TransInvoiceFiles")
                Dim id As String = Guid.NewGuid.ToString()
                Dim filename As String = id & "_" & Guid.NewGuid.ToString() & ".jpeg"

                p("ID") = ReadyString(id)
                p("InvoiceID") = ReadyString(InvoiceID)
                p("FileName") = ReadyString(filename)
                p("DateUploaded") = "getdate()"
                p("UploadedBy") = ReadyString(If(WebSession.Instance.PeopleUserID Is Nothing, Guid.Empty.ToString(), WebSession.Instance.PeopleUserID))

                Common.WebRequest.ProData.Execute(p.GetInsert())

                Using imagePath As IO.FileStream = New IO.FileStream(folderName & "\" & filename, IO.FileMode.Create)
                    Using binaryWriting As IO.BinaryWriter = New IO.BinaryWriter(imagePath)
                        binaryWriting.Write(Convert.FromBase64String(imageData))
                        binaryWriting.Close()
                    End Using
                End Using

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetInvoiceFiles() As String
            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim id As String = Common.WebRequest.Data("ID").ToString() 'invoiceid or receivingid
            Dim pOut As New JsonDictionary
            Dim pRow As DataTable
            Dim illegalstring As String = "[~`!@#$%^&*()+=|{}':;,<>\/?]"
            Dim folderName As String = Common.ServerPath & "InvoicesFiles\" & WebRequest.Instance.OrgID & "\" & id
            Dim pSql As String = "SELECT * FROM TransInvoiceFiles WHERE InvoiceID = " & ReadyString(id)

            Common.WebRequest.ProData.BeginTransaction()

            pRow = Common.WebRequest.ProData.Execute(pSql)

            For Each f As DataRow In pRow.Rows
                If f("FileName").IndexOfAny(illegalstring.ToCharArray) <> -1 Then
                    Dim newname As String = RemoveSymbols(f("FileName"), " .-")
                    Dim p As New InsertUpdate("TransInvoiceFiles")
                    p("FileName") = ReadyString(newname)
                    Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(f("ID").ToString())))

                    My.Computer.FileSystem.RenameFile(Common.ServerPath & "InvoicesFiles\" & WebRequest.Instance.OrgID & "\" & id & "\" & f("Filename"), newname)
                    f("FileName") = newname
                End If
            Next

            pOut.Add("Invoice", pRow)

            pSql = "SELECT files.ID, files.FileName, trans.SupplierID " & vbCrLf &
                    " FROM TransReceivingInvoiceRefImages as trans " & vbCrLf &
                    " INNER JOIN LibraryCRMAttachments as files ON files.ID = trans.CRMAttachmentID " & vbCrLf &
                    " WHERE trans.ReceivingID = " & ReadyString(id)
            pRow = Common.WebRequest.ProData.Execute(pSql)

            For Each f As DataRow In pRow.Rows
                If f("FileName").IndexOfAny(illegalstring.ToCharArray) <> -1 Then
                    Dim newname As String = RemoveSymbols(f("FileName"), " .-")
                    Dim p As New InsertUpdate("LibraryCRMAttachments")
                    p("FileName") = ReadyString(newname)
                    Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(f("ID").ToString())))

                    My.Computer.FileSystem.RenameFile(Common.ServerPath & "FileTemplates\" & WebRequest.Instance.OrgID & "\CustomerSupplierFiles\" & f("SupplierID").ToString() & "\" & f("Filename"), newname)
                    f("FileName") = newname
                End If
            Next
            pOut.Add("Receiving", pRow)

            Common.WebRequest.ProData.CommitTransaction()

            Return pOut.Serialize()
        End Function

        Function DeleteInvoiceFiles() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "DELETE FROM TransInvoiceFiles WHERE ID = " & ReadyString(Common.WebRequest.Data("ID").ToString())
            Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", "deleted")
        End Function

        Function GetCustomerWithCredit() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If Now.Date < #5/16/2018# Then
                WebRequest.Instance.Business.TransCustomerPayment.FixNumbers()
            End If

            If CBool2(Common.WebRequest.Data("ComputeWeightedAverageDaysOverdue")) Then
                WebRequest.Instance.Business.TransInvoice.CacheWeightedAverageDaysOverdue()
            End If
            Dim pOut As New JsonDictionary
            pOut.Add("Results", WebRequest.Instance.Business.TransInvoice.CustomersWithCredit(Common.WebRequest.Data("Filter")))
            Dim pWeightedAverageDaysOverdueLastUpdate = WebRequest.Instance.Business.SystemDefaults.WeightedAverageDaysOverdueLastUpdate
            If pWeightedAverageDaysOverdueLastUpdate Is Nothing Then
                pOut.Add("OutdatedWeightedAverageDaysOverdue", 1)
            Else
                If pWeightedAverageDaysOverdueLastUpdate.Value.Date <> Now.Date Then
                    pOut.Add("OutdatedWeightedAverageDaysOverdue", 1)
                End If
            End If

            Return pOut.Serialize()

        End Function

        Function GetCustomerContactInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim org As New OrganizationShort
            Dim customerid As String = Common.WebRequest.Data("CustomerID")

            pOut.Add("Telephone", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(org.GetProspectOrgTel(customerid))))
            pOut.Add("Mobile", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(org.GetProspectOrgMobile(customerid))))
            pOut.Add("Email", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(org.GetProspectOrgEmail(customerid))))
            pOut.Add("Website", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(org.GetProspectOrgWebsite(customerid))))

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function GetCustomerNetBalance() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pCustomerOrganizationID = Common.WebRequest.Data("CustomerOrganizationID")

            If IsGuid(pCustomerOrganizationID) = False Then
                Return JsonDictionary.Serialize("Error", "CustomerOrganizationID must be a GUID. You passed " & pCustomerOrganizationID & ". ref: vb2932W8MM8N!")
            End If

            Dim pOut = New JsonDictionary()
            pOut.Add("CreditLimit", WebRequest.Instance.Business.LibraryCustomers.GetCreditLimit(pCustomerOrganizationID))

            Dim pDT = WebRequest.Instance.Business.TransInvoice.CustomersWithCredit("custom", "WHERE MobizLibraryOrganization.ID = " & ReadyString(pCustomerOrganizationID))
            If pDT.Rows.Count = 0 Then
                pOut("NetInvoiceBalance") = 0
            Else
                pOut("NetInvoiceBalance") = CDec2(pDT.Rows(0)("NetInvoiceBalance"))
            End If

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function GetCustomerWithCreditSummary() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pJson As New JsonDictionary()
            pJson.Add("Results", WebRequest.Instance.Business.TransInvoice.CustomersWithCreditSummary())

            Return pJson.Serialize()

        End Function
        Function GetCustomerCreditDetails() As String

            Dim pOut As JsonDictionary
            Dim pFilter As String
            Dim pIncludeHasPayable As Boolean
            Dim pIncludeFullyPaid As Boolean
            Dim pStr As String
            Dim pDR As DataRow
            Dim pDT As DataTable
            Dim pAgingOfAr As Decimal
            Dim pNetInvoiceBalance As Decimal
            Dim pCheckPaymentAmountPdc As Decimal
            Dim pEsoaMode As Boolean

            Try
                If Request.UrlReferrer.AbsolutePath.ToLower <> "/creditandcollection/onlinesoa" Then
                    If WebSession.Instance.PeopleUserID Is Nothing Then
                        Return JsonDictionary.Serialize("Error", "Access denied")
                    End If
                Else
                    pEsoaMode = True
                End If

                If WebRequest.Instance.HandleOrganization(True) = False Then
                    Return WebRequest.InvalidOrganization
                End If

                pFilter = Common.WebRequest.Data("Filter")
                If pFilter = "all" Then
                    pIncludeHasPayable = True
                    pIncludeFullyPaid = True
                ElseIf pFilter = "has payable" Then
                    pIncludeHasPayable = True
                ElseIf pFilter = "fully paid" Then
                    pIncludeFullyPaid = True
                End If
            Catch ex As Exception
                pStr = "GetCustomerCreditDetails vb5EPLQM867E!"
                AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), , pStr)
                Return JsonDictionary.Serialize("Error", "Error has been logged to " & pStr)
            End Try

            Try
                Dim pEarliestDate As Date?
                pOut = New JsonDictionary
                If pEsoaMode Then
                    pEarliestDate = WebRequest.Instance.Business.SystemDefaults.EsoaEarliestDate
                Else
                    pEarliestDate = WebRequest.Instance.Business.SystemDefaults.SoaEarliestDate
                End If
                pDT = WebRequest.Instance.Business.TransInvoice.GetInvoicesWithCreditFromCache(Common.WebRequest.Data("CustomerID"), , , pIncludeHasPayable, pIncludeFullyPaid, , pEarliestDate)
                pAgingOfAr = TransInvoice.GetAgingOfAr(pDT)
                pOut.Add("AgingOfAr", pAgingOfAr)
                pNetInvoiceBalance = CDec2(pDT.Compute("SUM(NetInvoiceBalance)", ""))
                pOut.Add("NetInvoiceBalance", pNetInvoiceBalance)
                pCheckPaymentAmountPdc = CDec2(pDT.Compute("SUM(CheckPaymentAmountPdc)", ""))
                pOut.Add("CheckPaymentAmountPdc", pCheckPaymentAmountPdc)
                pOut.Add("AvailableCreditLimitComputation", WebRequest.Instance.Business.SystemDefaults.AvailableCreditLimitComputation)
                pOut.Add("Results", pDT)
                pDR = WebRequest.Instance.Business.LibraryCustomers.GetData(Common.WebRequest.Data("CustomerID"))
                If pDR IsNot Nothing Then
                    pOut.Add("Data", pDR)
                End If
                pOut.Add("Overpayment", WebRequest.Instance.Business.LibraryCustomers.GetUnappliedPayments(Common.WebRequest.Data("CustomerID")))
                pOut.Add("ExcessPayments", WebRequest.Instance.Business.LibraryCustomers.GetExcessPayments(Common.WebRequest.Data("CustomerID")))

                Return pOut.Serialize()
            Catch ex As Exception
                pStr = "GetCustomerCreditDetails vbD6B594CK49!"
                AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), , pStr)
                Return JsonDictionary.Serialize("Error", "Error has been logged to " & pStr)
            End Try

        End Function

        <HttpPost>
        Public Function GetCustomerInvoicesWithCredit() As String

            Dim pOut As JsonDictionary
            Dim pInvoiceID As String ' optional
            Dim pEsoaMode As Boolean
            Dim pIncludeHasPayable As Boolean
            Dim pIncludeFullyPaid As Boolean

            If Request.UrlReferrer.AbsolutePath.ToLower <> "/creditandcollection/onlinesoa" Then
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            Else
                pEsoaMode = True
            End If

            Dim pCustomerID = Common.WebRequest.Data("CustomerID")
            If String.IsNullOrWhiteSpace(pCustomerID) Then
                Return JsonDictionary.Serialize("Error", "CustomerID must not be blank. vbTHT9B98MEV!")
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim pEarliestDate As Date?

            If pEsoaMode Then
                pEarliestDate = WebRequest.Instance.Business.SystemDefaults.EsoaEarliestDate
            Else
                pEarliestDate = WebRequest.Instance.Business.SystemDefaults.SoaEarliestDate
            End If

            Dim pFilter = Common.WebRequest.Data("Filter")
            If pFilter = "all" Then
                pIncludeHasPayable = True
                pIncludeFullyPaid = True
            ElseIf pFilter = "has payable" Then
                pIncludeHasPayable = True
            ElseIf pFilter = "fully paid" Then
                pIncludeFullyPaid = True
            End If

            pOut = New JsonDictionary
            pOut.Add("Results", WebRequest.Instance.Business.TransInvoice.GetInvoicesWithCreditFromCache(pCustomerID, , pInvoiceID, pIncludeHasPayable, pIncludeFullyPaid, , pEarliestDate))
            pOut.Add("Overpayment", WebRequest.Instance.Business.TransCustomerPayment.GetAvailableUnappliedCredits(pCustomerID))

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function SavePayment() As String

            Dim pJsonAr As TransCustomerPayment.SavePaymentJson
            Dim pJsonAp As TransReceivingPayment.SavePaymentJson
            Dim pError As String
            Dim pOut As JsonDictionary
            Dim pDT As DataTable
            Dim pID As String
            Dim pUuid As String
            Dim pMode As String = Common.WebRequest.Data("Mode")
            Dim pJsonSettings As JsonSerializerSettings
            Dim pInvoiceMin As String
            Dim pInvoiceNum As String

            If Request.UrlReferrer.AbsolutePath.ToLower <> "/creditandcollection/onlinesoa" Then
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pUuid = Common.WebRequest.Data("UUID")
            pJsonSettings = New JsonSerializerSettings()
            pJsonSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            WebRequest.Instance.Business.WebRequest = WebRequest.Instance
            If pMode = "AP" Then
                pJsonAp = JsonConvert.DeserializeObject(Of TransReceivingPayment.SavePaymentJson)(Common.WebRequest.InputStreamText, pJsonSettings)
                pJsonAp.UserID = WebSession.Instance.PeopleUserID
                pJsonAp.DeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
                pJsonAp.OrgID = WebRequest.Instance.OrgID
                pID = WebRequest.Instance.Business.TransReceivingPayment.SavePayments(pJsonAp, pError)
            Else
                pJsonAr = JsonConvert.DeserializeObject(Of TransCustomerPayment.SavePaymentJson)(Common.WebRequest.InputStreamText, pJsonSettings)
                If CBool2(pJsonAr.IsPos) Then
                    If String.IsNullOrWhiteSpace(pUuid) Then
                        Return JsonDictionary.Serialize("Error", "A blank UUID is not allowed. Please login using Mobiz Controller.")
                    End If
                End If
                pJsonAr.UserID = WebSession.Instance.PeopleUserID
                pJsonAr.DeviceID = WebRequest.Instance.Business.LibraryDevice.GetOrAddByHddSerial(pUuid)
                pJsonAr.OrgID = WebRequest.Instance.OrgID
                If IsGuid(pJsonAr.FowstID) = False _
                    AndAlso CBool2(pJsonAr.FromEcommerce) Then

                    pJsonAr.FowstID = WebRequest.Instance.Business.SystemDefaults.EcommerceDefaultFOWST
                End If

                If CBool2(pJsonAr.IsPos) Then
                    If WebRequest.Instance.Business.LibraryDeviceFowst.IsAllowed(pJsonAr.DeviceID, pJsonAr.FowstID, pJsonAr.UserID, pError) = False Then
                        Return JsonDictionary.Serialize("Error", pError)
                    End If
                End If

                pID = WebRequest.Instance.Business.TransCustomerPayment.SavePayments(pJsonAr, pError)
                If pID IsNot Nothing _
                    AndAlso CBool2(pJsonAr.IsPos) _
                    AndAlso pJsonAr.Invoices IsNot Nothing _
                    AndAlso pJsonAr.Invoices.Count > 0 Then

                    Dim pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById2(pJsonAr.Invoices.First.ID)
                    If pDrInvoice IsNot Nothing Then
                        pInvoiceMin = CStr2(pDrInvoice("MIN")) & "-" & CStr2(pDrInvoice("InvoiceNumber"))
                        pInvoiceNum = CStr2(pDrInvoice("InvoiceNumber"))
                    End If
                End If
            End If
            If pID IsNot Nothing Then
                pOut = New JsonDictionary
                pOut.Add("Result", "ok")
                pOut.Add("ID", pID)
                If String.IsNullOrWhiteSpace(pInvoiceMin) = False Then
                    pOut.Add("InvoiceMin", pInvoiceMin)
                    pOut.Add("InvoiceNum", pInvoiceNum)
                End If
                pDT = WebRequest.Instance.Business.TransCustomerPayment.GetChequeDistribution(pID)
                pOut.Add("Distribution", pDT)
                Return pOut.Serialize()
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

        End Function

        <HttpPost>
        Public Function ChequeDetail() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pMode As String = Common.WebRequest.Data("Mode")

            Dim pOut = New JsonDictionary()
            Dim pDrChequeDetail As DataRow
            If pMode = "AP" Then
                pDrChequeDetail = WebRequest.Instance.Business.TransReceivingPayment.ChequeDetail(Common.WebRequest.Data("BankID"), Common.WebRequest.Data("CheckNumber"))
            Else
                pDrChequeDetail = WebRequest.Instance.Business.TransCustomerPayment.ChequeDetail(Common.WebRequest.Data("BankID"), Common.WebRequest.Data("CheckNumber"))
            End If
            If pDrChequeDetail IsNot Nothing Then
                pOut.Add("ChequeDetail", pDrChequeDetail)
            End If

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function GetChequeDistribution() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = WebRequest.Instance.Business.TransCustomerPayment.GetChequeDistribution(Nothing, Common.WebRequest.Data("CustomerPaymentChequeID"))

            Return JsonConvert.SerializeObject(pOut)

        End Function

        <HttpPost>
        Public Function GetDraftPayment() As String

            Dim pDraftPaymentID As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pDraftPaymentID = Common.WebRequest.Data("ID")
            Dim pOut = WebRequest.Instance.Business.TransCustomerPayment.GetDraftPayment(pDraftPaymentID)

#If DEBUG Then
            '            WebRequest.Instance.Business.TransCustomerPayment.CreatePaymentPdf(pDraftPaymentID, True)
#End If

            Return JsonConvert.SerializeObject(pOut)

        End Function

        <HttpPost>
        Public Function GenerateAcknowledgementReceipt() As String

            Dim pDraftPaymentID As String

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pDraftPaymentID = Common.WebRequest.Data("ID")
            Dim pFilename = WebRequest.Instance.Business.TransCustomerPayment.CreatePaymentPdf(pDraftPaymentID, True)

            Return JsonConvert.SerializeObject(pFilename)

        End Function

        <HttpPost>
        Public Function ReceivingUpdateProductPrices() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pReceivingID = Common.WebRequest.Data("ReceivingID")
            If String.IsNullOrEmpty(pReceivingID) Then
                Return JsonDictionary.Serialize("Error", "ReceivingID must not be blank. vbDPLB4PCLCQ!")
            End If

            If IsGuid(pReceivingID) = False Then
                Return JsonDictionary.Serialize("Error", "ReceivingID must not be a GUID. vbHPC6LA6TAM!")
            End If

            WebRequest.Instance.Business.TransReceivingPayment.UpdateProductPrices(pReceivingID, WebRequest.Instance.OrgID, True)

            Return DateTimeStamp()

        End Function

        <HttpPost>
        Public Function SaveAdjustment() As String

            Dim pJson As TransInvoice.SaveAdjustmentJson
            Dim pID As String
            Dim pError As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pJson = JsonConvert.DeserializeObject(Of TransInvoice.SaveAdjustmentJson)(Common.WebRequest.InputStreamText)
            pID = Common.WebRequest.Data("ID")
            pJson.ID = pID
            pJson.IsNew = IsGuid(pID) = False
            If IsNumeric(pJson.Amount) = False Then
                Return JsonDictionary.Serialize("Error", "Amount must be numeric.")
            End If
            WebRequest.Instance.Business.TransInvoice.SaveAdjustment(pJson, pError)
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function GetAdjustment() As String

            Dim pAdjustmentID As String
            Dim pDT As DataTable
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pAdjustmentID = Common.WebRequest.Data("AdjustmentID")
            pOut = New JsonDictionary
            pDT = WebRequest.Instance.Business.TransInvoice.GetAdjustments(Nothing, pAdjustmentID)
            pOut.Add("Master", pDT)

            pDT = WebRequest.Instance.Business.TransInvoice.GetAdjustmentReturnedChecks(pAdjustmentID)
            pOut.Add("ReturnedChecks", pDT)

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function CreateBilling() As String

            Dim pCustomerID As String
            Dim pBaseFilename As String
            Dim pEsoaMode As Boolean

            If Request.UrlReferrer.AbsolutePath.ToLower <> "/creditandcollection/onlinesoa" Then
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            Else
                pEsoaMode = True
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pCustomerID = Common.WebRequest.Data("CustomerID")
            If IsGuid(pCustomerID) = False Then
                Return JsonDictionary.Serialize("Error", "CustomerID must be a GUID.")
            End If
            Dim pEarliestDate As Date?

            If pEsoaMode Then
                pEarliestDate = WebRequest.Instance.Business.SystemDefaults.EsoaEarliestDate
            Else
                pEarliestDate = WebRequest.Instance.Business.SystemDefaults.SoaEarliestDate
            End If
            pBaseFilename = WebRequest.Instance.Business.LibraryCustomers.CreateBilling(pCustomerID, pEarliestDate)

            Return JsonDictionary.Serialize("Filename", pBaseFilename)

        End Function

        <HttpGet>
        Public Function GetBilling() As ActionResult

            Dim pFilename As String

            pFilename = Common.ServerPath & "temp\Billing\" & Common.WebRequest.QueryString("Filename") & ".pdf"

            Return File(pFilename, "application/pdf")

        End Function

        <HttpPost>
        Public Function CncApproveSalesOrder() As String

            Dim pInvoiceID As String
            Dim pMessage As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If WebRequest.Instance.Business.TransInvoice.CncApproveSalesOrder(pInvoiceID, WebSession.Instance.PeopleUserID, pMessage, WebRequest.Instance.OrgID) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pMessage)
            End If

        End Function

        <HttpPost>
        Public Function CncDisapproveSalesOrder() As String

            Dim pInvoiceID As String
            Dim pMessage As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If WebRequest.Instance.Business.TransInvoice.CncDisapproveSalesOrder(pInvoiceID, WebSession.Instance.PeopleUserID, pMessage, WebRequest.Instance.OrgID) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pMessage)
            End If

        End Function

        <HttpPost>
        Public Function IsCncApprovable() As String

            Dim pInvoiceID As String
            Dim pMessage As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If WebRequest.Instance.Business.TransInvoice.IsCncApprovable(pInvoiceID, pMessage) Then
                Return JsonDictionary.Serialize("Result", "1")
            Else
                Return JsonDictionary.Serialize("Result", "0")
            End If

        End Function

        <HttpPost>
        Public Function DeactivateMemo() As String

            Dim pMessage As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebRequest.Instance.ProData.BeginTransaction()
            pMessage = WebRequest.Instance.Business.TransInvoice.DeactivateAdjustment(Common.WebRequest.Data("ID"), WebSession.Instance.PeopleUserID)
            WebRequest.Instance.Business.TransInvoice.CacheInvoicesWithCredit()
            WebRequest.Instance.ProData.CommitTransaction()

            If String.IsNullOrWhiteSpace(pMessage) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pMessage)
            End If

        End Function

        <HttpPost>
        Public Function GetSuppliersWithPayable() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If Now.Date < #8/3/2018# Then
                WebRequest.Instance.Business.TransReceiving.CacheReceivingsWithCredit()
            End If

            ' for debugging
            Dim pArtificialDelay = CInt2(Common.WebRequest.Data("ArtificialDelay"))
            If pArtificialDelay > 0 Then
                Thread.Sleep(pArtificialDelay)
            End If

            Return JsonDictionary.Serialize("Results", WebRequest.Instance.Business.TransReceiving.SuppliersWithReceivable(Common.WebRequest.Data("Filter")))

        End Function

        <HttpPost>
        Public Function GetReceivingsWithPayable() As String

            Dim pOut As JsonDictionary
            'Dim pInvoiceID As String ' optional
            Dim pSupplierID As String = Common.WebRequest.Data("SupplierID")
            Dim pReceivingID As String = Common.WebRequest.Data("ReceivingID")
            Dim pCancelled As Boolean
            Dim pIncludeHasPayable As Boolean
            Dim pIncludeFullyPaid As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ' for debugging
            Dim pArtificialDelay = CInt2(Common.WebRequest.Data("ArtificialDelay"))
            If pArtificialDelay > 0 Then
                Thread.Sleep(pArtificialDelay)
            End If
            'pInvoiceID = Common.WebRequest.Data("InvoiceID")
            Dim pFilter = Common.WebRequest.Data("Filter")
            If pFilter = "all" Then
                pIncludeHasPayable = True
                pIncludeFullyPaid = True
            ElseIf pFilter = "has payable" Then
                pIncludeHasPayable = True
            ElseIf pFilter = "fully paid" Then
                pIncludeFullyPaid = True
            ElseIf pFilter = "cancelled" Then
                pCancelled = True
            End If

            pOut = New JsonDictionary
            pOut.Add("Results", WebRequest.Instance.Business.TransReceiving.GetReceivingsWithPayableFromCache(pSupplierID, , pReceivingID, pIncludeHasPayable, pIncludeFullyPaid, , , pCancelled))
            pOut.Add("Overpayment", WebRequest.Instance.Business.TransReceivingPayment.GetAvailableUnappliedCredits(pSupplierID))

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function GetAvailableUnappliedCreditsDetails() As String

            Dim pOut As JsonDictionary
            Dim pCustomerOrSupplierID As String = Common.WebRequest.Data("CustomerOrSupplierID")
            Dim pFilter As String = Common.WebRequest.Data("Filter")
            Dim pCacMode As String = Common.WebRequest.Data("CacMode")

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary
            If pCacMode = "AP" Then
                pOut.Add("Results", WebRequest.Instance.Business.TransReceivingPayment.GetAvailableUnappliedCreditsDetails(pCustomerOrSupplierID, pFilter))
            Else
                pOut.Add("Results", WebRequest.Instance.Business.TransCustomerPayment.GetAvailableUnappliedCreditsDetails(pCustomerOrSupplierID, pFilter))
            End If

            Return pOut.Serialize()

        End Function

        Function GetSupplierReceivablesDetails() As String

            Dim pOut As JsonDictionary
            Dim pFilter As String
            Dim pIncludeHasPayable As Boolean
            Dim pIncludeFullyPaid As Boolean
            Dim pExcludeWithOfficialReceiptRef As Boolean
            Dim pIncludeDraft As Boolean
            Dim pStr As String
            Dim pDR As DataRow

            Try
                ' for debugging
                Dim pArtificialDelay = CInt2(Common.WebRequest.Data("ArtificialDelay"))
                If pArtificialDelay > 0 Then
                    Thread.Sleep(pArtificialDelay)
                End If

                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                pFilter = Common.WebRequest.Data("Filter")
                If pFilter = "all" Then
                    pIncludeHasPayable = True
                    pIncludeFullyPaid = True
                    pIncludeDraft = True
                ElseIf pFilter = "has payable" Then
                    pIncludeHasPayable = True
                    pIncludeDraft = True
                ElseIf pFilter = "fully paid" Then
                    pIncludeFullyPaid = True
                ElseIf pFilter = "fully paid without or" Then
                    pIncludeFullyPaid = True
                    pExcludeWithOfficialReceiptRef = True
                End If
            Catch ex As Exception
                pStr = "GetSupplierReceivablesDetails vbDX76NVYDQQ!"
                AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), , pStr)
                Return JsonDictionary.Serialize("Error", "Error has been logged to " & pStr)
            End Try

            Try
                pOut = New JsonDictionary
                pOut.Add("Results", WebRequest.Instance.Business.TransReceiving.GetReceivingsWithPayableFromCache(Common.WebRequest.Data("SupplierID"), , , pIncludeHasPayable, pIncludeFullyPaid, pExcludeWithOfficialReceiptRef, pIncludeDraft))
                pDR = WebRequest.Instance.Business.LibrarySuppliers.GetData(Common.WebRequest.Data("SupplierID"))
                If pDR IsNot Nothing Then
                    pOut.Add("Data", pDR)
                End If
                pOut.Add("Overpayment", WebRequest.Instance.Business.LibraryCustomers.GetUnappliedPayments(Common.WebRequest.Data("SupplierID")))
                pOut.Add("ExcessPayments", WebRequest.Instance.Business.LibraryCustomers.GetExcessPayments(Common.WebRequest.Data("SupplierID")))

                Return pOut.Serialize()
            Catch ex As Exception
                pStr = "GetSupplierReceivablesDetails vb5V4C2HY7WL!"
                AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), , pStr)
                Return JsonDictionary.Serialize("Error", "Error has been logged to " & pStr)
            End Try

        End Function

        <HttpPost>
        Public Function SaveDeposits() As String

            Dim pOut As JsonDictionary
            Dim pDeposits As TransCustomerPayment.DepositsClass
            Dim pError As String
            Dim pDepositGroupID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pDeposits = JsonConvert.DeserializeObject(Of TransCustomerPayment.DepositsClass)(Common.WebRequest.InputStreamText)
            pDeposits.DepositedBy = WebSession.Instance.PeopleUserID

            pOut = New JsonDictionary
            pDepositGroupID = WebRequest.Instance.Business.TransCustomerPayment.SaveDeposits(pDeposits, pError)
            If pError Is Nothing Then
                pOut.Add("Results", 1)
                pOut.Add("DepositGroupID", pDepositGroupID)
            Else
                pOut.Add("Error", pError)
            End If

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function UpdateBalanceFromDuc() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


        End Function

        Function ValidateSoaLink()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim cusid As String = Common.WebRequest.Data("CustomerID")
            Dim token As String = Common.WebRequest.Data("Token")
            Dim fowstid As String = Common.WebRequest.Data("FowstID")
            Dim pSql As String = "SELECT CASE WHEN MobizLibraryOrganization.CompanyName = '' THEN MobizLibraryOrganization.PersonFirstName + ' ' + MobizLibraryOrganization.PersonSurname ELSE  MobizLibraryOrganization.CompanyName END as CustomerName, SOALinkToken as Token, SOALinkExpirationDateFrom as DateFrom, SOALinkExpirationDateTo as DateTo FROM LibraryOrganizationCustomer INNER JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryOrganizationCustomer.OrganizationID WHERE CONVERT(varchar(MAX), OrganizationID) = " & ReadyString(cusid)
            Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            Dim p As New JsonDictionary

            Dim fowstcnt As Integer = Common.WebRequest.ProData.Execute("SELECT ID FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(fowstid)).Rows.Count

            If rows.Count = 0 Then
                p.Add("Invalid", True)
                Return p.Serialize()
            End If

            If token = rows(0)("Token") And fowstcnt > 0 And CDate(Date.Now().Date) >= CDate(rows(0)("DateFrom")) And CDate(Date.Now().Date) <= CDate(rows(0)("DateTo")) Then
                p.Add("DateFrom", rows(0)("DateFrom"))
                p.Add("DateTo", rows(0)("DateTo"))
                p.Add("CompanyName", rows(0)("CustomerName"))
            Else
                p.Add("Invalid", True)
            End If

            Return p.Serialize()
        End Function

        Function GenerateSoaLink()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim cusid As String = Common.WebRequest.Data("CustomerID")
            Dim pSql As String = "SELECT SOALinkToken as Token, SOALinkExpirationDateFrom as DateFrom, SOALinkExpirationDateTo as DateTo FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(cusid)
            Dim rows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
            Dim pOut As New JsonDictionary

            If CStr2(rows(0)("DateFrom")) = "" And CStr2(rows(0)("DateTo")) = "" Then
                UpdateSOALinkDate(cusid)
            Else
                If CDate(Date.Now().Date) >= CDate(rows(0)("DateFrom")) And CDate(Date.Now().Date) <= CDate(rows(0)("DateTo")) Then
                    'do nothing
                Else
                    'reset date
                    UpdateSOALinkDate(cusid)
                End If
            End If

            pOut.Add("Results", Common.WebRequest.ProData.Execute(pSql))
            pOut.Add("soalinkdomain", If(Request.Url.Host = "localhost", "http://localhost:8080", WebRequest.Instance.Business.SystemDefaults.SOALinkDomainName))
            pOut.Add("soalinkinternalipaddress", WebRequest.Instance.Business.SystemDefaults.SOALinkDomainInternalIPAddress)
            pOut.Add("soalinkexternalipaddress", WebRequest.Instance.Business.SystemDefaults.SOALinkDomainExternalIPAddress)

            Return pOut.Serialize()
        End Function

        Function UpdateSOALinkDate(ByVal cusid As String)
            Dim allowableChars() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
            Dim link As String
            Dim rand As New Random()

            For i As Integer = 0 To 30 - 1
                link += allowableChars(rand.Next(allowableChars.Length - 1))
            Next

            Dim p As New InsertUpdate("LibraryOrganizationCustomer")

            p("SOALinkToken") = ReadyString(link)
            p("SOALinkExpirationDateFrom") = "getdate()"
            p("SOALinkExpirationDateTo") = "DATEADD(day, " & WebRequest.Instance.Business.SystemDefaults.ExpirationDaysForSOALink & ", GETDATE())"

            Common.WebRequest.ProData.Execute(p.GetUpdate(" OrganizationID = " & ReadyString(cusid)))
        End Function

        <HttpPost>
        Public Function GetSettings() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut = New JsonDictionary

            pOut.Add("GrantCreditOffline", WebRequest.Instance.Business.SystemDefaults.GrantCreditOffline)
            pOut.Add("GrantCreditAllCustomers", WebRequest.Instance.Business.SystemDefaults.GrantCreditAllCustomers)
            pOut.Add("ExpirationDaysForSOALink", WebRequest.Instance.Business.SystemDefaults.ExpirationDaysForSOALink)
            pOut.Add("SOALinkDomainName", WebRequest.Instance.Business.SystemDefaults.SOALinkDomainName)
            pOut.Add("SOALinkDomainInternalIPAddress", WebRequest.Instance.Business.SystemDefaults.SOALinkDomainInternalIPAddress)
            pOut.Add("SOALinkDomainExternalIPAddress", WebRequest.Instance.Business.SystemDefaults.SOALinkDomainExternalIPAddress)
            pOut.Add("SOAFootnote", WebRequest.Instance.Business.SystemDefaults.SOAFootnote)
            Dim pDate? = WebRequest.Instance.Business.SystemDefaults.EsoaEarliestDate
            If pDate IsNot Nothing Then
                pOut.Add("EsoaEarliestDate", Format(pDate, cGlobal.FormatDateTimeSql))
            End If
            pDate = WebRequest.Instance.Business.SystemDefaults.SoaEarliestDate
            If pDate IsNot Nothing Then
                pOut.Add("SoaEarliestDate", Format(pDate, cGlobal.FormatDateTimeSql))
            End If

            Dim pSql = "SELECT MobizLibraryOrganization.ID" & vbCrLf &
                ",MobizLibraryOrganization.CompanyName" & vbCrLf &
                " FROM LibraryOrganizationCustomer" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
                " WHERE LibraryOrganizationCustomer.UnlimitedCredit = 1" & vbCrLf &
                " ORDER BY MobizLibraryOrganization.CompanyName"
            Dim pDT = WebRequest.Instance.ProData.Execute(pSql)
            pOut.Add("Customers", pDT)

            Return pOut.Serialize()

        End Function

        Public Class SaveSettingsInput
            Public GrantCreditOffline As String ' boolean
            Public GrantCreditAllCustomers As String ' boolean
            Public EsoaEarliestDate As String ' date
            Public SoaEarliestDate As String ' date
            Public CustomerIDs As List(Of String)
            Public RemovedCustomers As List(Of String)
        End Class

        <HttpPost>
        Public Function SaveSettings() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pBaseFilename As String
            Dim pTempFilename As String
            Dim pFilename As String
            Dim pOut As New JsonDictionary

            If Request.Files.Count > 0 _
                AndAlso Request.Files(0).ContentLength > 0 Then

                pBaseFilename = "CustomerPayment_" & ReadyGuid(WebRequest.Instance.OrgID)
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

            WebRequest.Instance.ProData.BeginTransaction()
            Dim pInput = JsonConvert.DeserializeObject(Of SaveSettingsInput)(Common.WebRequest.InputStreamText)
            WebRequest.Instance.Business.SystemDefaults.GrantCreditOffline = CBool2(pInput.GrantCreditOffline)
            WebRequest.Instance.Business.SystemDefaults.GrantCreditAllCustomers = CBool2(pInput.GrantCreditAllCustomers)
            If IsDate(pInput.EsoaEarliestDate) Then
                WebRequest.Instance.Business.SystemDefaults.EsoaEarliestDate = ParseDate(pInput.EsoaEarliestDate)
            End If
            If IsDate(pInput.SoaEarliestDate) Then
                WebRequest.Instance.Business.SystemDefaults.SoaEarliestDate = ParseDate(pInput.SoaEarliestDate)
            End If
            WebRequest.Instance.Business.SystemDefaults.ExpirationDaysForSOALink = Request.Form.Get("expirationdaysforsoalink")
            WebRequest.Instance.Business.SystemDefaults.SOALinkDomainName = Request.Form.Get("soalinkdomain")
            WebRequest.Instance.Business.SystemDefaults.SOALinkDomainInternalIPAddress = Request.Form.Get("soalinkinternalipaddress")
            WebRequest.Instance.Business.SystemDefaults.SOALinkDomainExternalIPAddress = Request.Form.Get("soalinkexternalipaddress")
            WebRequest.Instance.Business.SystemDefaults.SOAFootnote = Request.Form.Get("SOAFootnote")

            If CBool2(pInput.GrantCreditAllCustomers) = False Then
                Dim pSql As String
                Dim pRemovedCustomerIDs = ""
                For Each pCustomerID In pInput.RemovedCustomers
                    If pRemovedCustomerIDs <> "" Then
                        pRemovedCustomerIDs &= ", "
                    End If
                    pRemovedCustomerIDs &= ReadyString(pCustomerID)
                Next
                If pRemovedCustomerIDs <> "" Then
                    Dim pIU = New InsertUpdate("LibraryOrganizationCustomer")
                    pIU("UnlimitedCredit") = 0
                    pIU("LastUpdated") = "GETDATE()"
                    pSql = pIU.GetUpdate("OrganizationID IN (" & pRemovedCustomerIDs & ")")
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                End If

                For Each pCustomerID In pInput.CustomerIDs
                    Dim pIU = New InsertUpdate("LibraryOrganizationCustomer")
                    pIU("UnlimitedCredit") = 1
                    pIU("LastUpdated") = "GETDATE()"
                    pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(pCustomerID))
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                Next

                If (
                        pInput.RemovedCustomers IsNot Nothing _
                        AndAlso pInput.RemovedCustomers.Count > 0
                    ) OrElse (
                        pInput.CustomerIDs IsNot Nothing _
                        AndAlso pInput.CustomerIDs.Count > 0
                    ) Then

                    pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                    WebRequest.Instance.ProData.ExecuteSQL(pSql)
                End If
            End If
            WebRequest.Instance.ProData.CommitTransaction()

            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        End Function

        Public Class SaveFinancialDetailsReadJson
            Public ID As String
            Public CustomerOrgID As String
            Public BankOrgID As String
            Public BankBranch As String
            Public AccountName As String
            Public AccountNumber As String
            Public FromEcommerce As String ' boolean
            Public PaymentType As String '// cheque (default) or online-payment
            Public Deactivated As String ' 1 or 0 or blank
        End Class

        <HttpPost>
        Public Function SaveFinancialDetails() As String

            Try
                If Request.UrlReferrer.AbsolutePath.ToLower <> "/creditandcollection/onlinesoa" Then
                    If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                End If
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                Dim pInput = JsonConvert.DeserializeObject(Of SaveFinancialDetailsReadJson)(WebRequest.Instance.InputStreamText)
                Dim pSql

                Dim pIU = New InsertUpdate("LibraryOrganizationFinancialDetails")

                pIU("CustomerOrgID") = ReadyString(pInput.CustomerOrgID)
                pIU("BankOrgID") = ReadyString(pInput.BankOrgID)
                pIU("BankBranch") = ReadyString(pInput.BankBranch)
                pIU("AccountName") = ReadyString(pInput.AccountName)
                pIU("AccountNumber") = ReadyString(pInput.AccountNumber)
                pIU("UpdatedByID") = ReadyString(If(WebSession.Instance.PeopleUserID Is Nothing, Guid.Empty.ToString(), WebSession.Instance.PeopleUserID))
                pIU("UpdatedDate") = "GETDATE()"
                If CBool2(pInput.FromEcommerce) Then
                    pIU("IsDraft") = 1
                End If
                Select Case CStr2(pInput.PaymentType).Trim().ToLower()
                    Case "online-payment"
                        pIU("OnlinePayment") = 1
                End Select

                If CStr2(Common.WebRequest.Data("ID")) = "" Then
                    pIU("ID") = "NEWID()"
                    If String.IsNullOrWhiteSpace(pInput.Deactivated) = False Then
                        If CBool(pInput.Deactivated) Then
                            pIU("DeactivatedByID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pIU("DeactivatedDate") = "GETDATE()"
                        End If
                    End If
                    pSql = pIU.GetInsert()
                Else
                    pSql = pIU.GetUpdate(" ID = " & ReadyString(pInput.ID))
                End If

                WebRequest.Instance.ProData.ExecuteSQL(pSql)

                If String.IsNullOrWhiteSpace(pInput.ID) = False _
                    AndAlso String.IsNullOrWhiteSpace(pInput.Deactivated) = False Then

                    pSql = "SELECT DeactivatedByID" & vbCrLf &
                        " FROM LibraryOrganizationFinancialDetails" & vbCrLf &
                        " WHERE ID = " & ReadyString(pInput.ID)
                    Dim pDR = WebRequest.Instance.ProData.ExecuteRow(pSql)
                    If pDR IsNot Nothing Then
                        If CBool(pInput.Deactivated) Then
                            If pDR("DeactivatedByID") Is DBNull.Value Then
                                pIU = New InsertUpdate("LibraryOrganizationFinancialDetails")
                                pIU("DeactivatedByID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pIU("DeactivatedDate") = "GETDATE()"
                                pSql = pIU.GetUpdate(" ID = " & ReadyString(pInput.ID))
                                WebRequest.Instance.ProData.ExecuteSQL(pSql)
                            End If
                        Else
                            If pDR("DeactivatedByID") IsNot DBNull.Value Then
                                pIU = New InsertUpdate("LibraryOrganizationFinancialDetails")
                                pIU("DeactivatedByID") = "NULL"
                                pIU("DeactivatedDate") = "NULL"
                                pSql = pIU.GetUpdate(" ID = " & ReadyString(pInput.ID))
                                WebRequest.Instance.ProData.ExecuteSQL(pSql)
                            End If
                        End If
                    End If
                End If

                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                If GetType(ProSqlException).IsAssignableFrom(ex.GetType()) Then
                    If ex.Message.Contains("Violation of UNIQUE KEY constraint") Then
                        Return JsonDictionary.Serialize("Error", "That account detail already exist. Please enter a unique account detail.")
                    End If
                End If
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function GetFinancialDetails() As String

            If Request.UrlReferrer.AbsolutePath.ToLower <> "/creditandcollection/onlinesoa" Then
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            End If
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pCustomerOrgID = Common.WebRequest.Data("CustomerOrgID")
            Dim pIncludeDeactivated = CBool2(Common.WebRequest.Data("IncludeDeactivated"))
            Dim pOrgID = Common.WebRequest.Data("OrgID2")
            Dim pDT = WebRequest.Instance.Business.LibraryCustomers.GetFinancialDetails(pCustomerOrgID, pOrgID, pIncludeDeactivated)

            Return JsonDictionary.Serialize(pDT)

        End Function

        <HttpPost>
        Public Function DeactivateFinancialDetails() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pInput = JsonConvert.DeserializeObject(Of SaveFinancialDetailsReadJson)(WebRequest.Instance.InputStreamText)

            Dim pIU = New InsertUpdate("LibraryOrganizationFinancialDetails")
            pIU("DeactivatedByID") = ReadyString(WebSession.Instance.PeopleUserID)
            pIU("DeactivatedDate") = "GETDATE()"
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(Common.WebRequest.Data("ID")))
            WebRequest.Instance.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Result", "ok")

        End Function

        <HttpPost>
        Public Function SupplierPaymentReport() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pPaymentType = Common.WebRequest.Data("PaymentType")
            Dim pDateFrom = Common.WebRequest.Data("From")
            Dim pDateTo = Common.WebRequest.Data("To")

            If IsDate(pDateFrom) = False _
                OrElse IsDate(pDateTo) = False Then

                Dim pDateRange = CStr2(Common.WebRequest.Data("EncodingDateRange"))
                If String.IsNullOrWhiteSpace(pDateRange) Then
                    Return JsonDictionary.Serialize("Error", "(From & To) or EncodingDateRange is required. vbNBJ2P3Y69R!")
                End If
                Dim pSplit = pDateRange.Split(New String() {"to"}, StringSplitOptions.RemoveEmptyEntries)
                If pSplit.Length <> 2 Then
                    Return JsonDictionary.Serialize("Error", "Invalid EncodingDateRange. vbMKQG9GHTBU!")
                End If
                pDateFrom = pSplit(0)
                pDateTo = pSplit(1)
                If IsDate(pDateFrom) = False _
                    OrElse IsDate(pDateTo) = False Then

                    Return JsonDictionary.Serialize("Error", "Invalid splitted EncodingDateRange. vbDS3HSB3JLG!")
                End If
            End If

            pDateFrom = Format(CDate(pDateFrom), cGlobal.FormatDateTimeSql)
            pDateTo = Format(CDate(pDateTo), cGlobal.FormatDateTimeSql)

            Dim pCustomerSelect As String
            Dim pCustomerFrom As String
            BrowserController.ComposeCustomer(pCustomerSelect, pCustomerFrom)

            Dim pSql = " -- supplier payment report" & vbCrLf &
                " SELECT DISTINCT TOP 100 PERCENT " & pCustomerSelect & " AS CompanyName" & vbCrLf &
                " ,ISNULL(chk.PaymentType, '') AS PaymentType" & vbCrLf &
                " , ISNULL(cuspay.ManualReceiptNumber, '') AS [ManualReceiptNumber]" & vbCrLf &
                " , ISNULL(chk.AccountName, '') AS [AccountName]" & vbCrLf &
                " , ISNULL(chk.BankName, '') AS [BankName]" & vbCrLf &
                " , ISNULL(chk.BankBranchName, '') AS [BankBranch]" & vbCrLf &
                " , ISNULL(chk.CheckNumber, '') AS [RefNum]" & vbCrLf &
                " , ISNULL(chk.CheckAmount, '') AS [AmountPaid]" & vbCrLf &
                " , ISNULL(CONVERT(varchar, chk.CheckDate, 126), '') AS [MaturityDate]" & vbCrLf &
                " , ISNULL(chk.CheckAmount, '') AS [CheckTotal]" & vbCrLf &
                " , ISNULL(chk.AccountNameOnline, '') AS [AccountNameOnline]" & vbCrLf &
                " , ISNULL(chk.BankNameOnline, '') AS [BankNameOnline]" & vbCrLf &
                " , ISNULL(chk.BankBranchNameOnline, '') AS [BankBranchOnline]" & vbCrLf &
                " , ISNULL(chk.CheckNumberOnline, '') AS [CheckNumberOnline]" & vbCrLf &
                " , ISNULL(chk.CheckAmountOnline, '') AS [CheckAmountOnline]" & vbCrLf &
                " , ISNULL(CONVERT(varchar, chk.CheckDateOnline, 126), '') AS [CheckDateOnline]" & vbCrLf &
                " , ISNULL(chk.CheckAmountOnline, '') AS [CheckTotalOnline]" & vbCrLf &
                " , ISNULL(chk.AccountNameOnline2, '') AS [AccountNameOnline2]" & vbCrLf &
                " , ISNULL(chk.BankNameOnline2, '') AS [BankNameOnline2]" & vbCrLf &
                " , ISNULL(chk.BankBranchNameOnline2, '') AS [BankBranchOnline2]" & vbCrLf &
                " , ISNULL(cuspay.CashAmount, '') AS [CashAmount]" & vbCrLf &
                " , cuspay.PaymentDate AS [PaymentDate]" & vbCrLf &
                " , cuspay.LastUpdated AS [LastUpdated]" & vbCrLf &
                " , ISNULL(chk.CheckAmount, 0) + ISNULL(chk.CheckAmountOnline, 0) + cuspay.CashAmount AS [Total]" & vbCrLf &
                " , cuspay.ID AS [CustomerPaymentID]" & vbCrLf &
                " , ISNULL(chk.ID, cuspay.ID) AS ID" & vbCrLf &
                " , chk.ID AS [CheckID]" & vbCrLf &
                " , chk.DepositStatus AS DepositStatus" & vbCrLf &
                "   FROM TransReceivingPayment as cuspay" & vbCrLf &
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = cuspay.SupplierID" & vbCrLf &
                pCustomerFrom & vbCrLf &
                "   LEFT JOIN (" & vbCrLf &
                "    	SELECT TransReceivingPaymentChecks.ID" & vbCrLf &
                "       ,TransReceivingPaymentChecks.PaymentID" & vbCrLf &
                "  	    ,TransReceivingPaymentChecks.IsOnlinePayment" & vbCrLf &
                "      ,TransReceivingPaymentChecks.AccountName" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankID" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankName" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankBranchName" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckNumber" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckDate" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckAmount" & vbCrLf &
                "  	  ,NULL AS AccountNameOnline" & vbCrLf &
                "      ,NULL AS BankIDOnline" & vbCrLf &
                "      ,NULL AS BankNameOnline" & vbCrLf &
                "      ,NULL AS BankBranchNameOnline" & vbCrLf &
                "      ,NULL AS CheckNumberOnline" & vbCrLf &
                "      ,NULL AS CheckDateOnline" & vbCrLf &
                "      ,NULL AS CheckAmountOnline" & vbCrLf &
                "      ,NULL AS AccountNameOnline2" & vbCrLf &
                "      ,NULL AS BankIDOnline2" & vbCrLf &
                "      ,NULL AS BankNameOnline2" & vbCrLf &
                "      ,NULL AS BankBranchNameOnline2" & vbCrLf &
                "      ,'Check' AS PaymentType" & vbCrLf &
                "      ,TransReceivingPaymentChecks.DepositStatus" & vbCrLf &
                "      FROM TransReceivingPaymentChecks" & vbCrLf &
                "      WHERE ISNULL(TransReceivingPaymentChecks.IsOnlinePayment, 0) = 0" & vbCrLf &
                "      UNION" & vbCrLf &
                "      SELECT TransReceivingPaymentChecks.ID" & vbCrLf &
                "      ,TransReceivingPaymentChecks.PaymentID" & vbCrLf &
                "  	  ,TransReceivingPaymentChecks.IsOnlinePayment" & vbCrLf &
                "      ,NULL AS AccountName" & vbCrLf &
                "      ,NULL AS BankID" & vbCrLf &
                "      ,NULL AS BankName" & vbCrLf &
                "      ,NULL AS BankBranchName" & vbCrLf &
                "      ,NULL AS CheckNumber" & vbCrLf &
                "      ,NULL AS CheckDate" & vbCrLf &
                "      ,NULL AS CheckAmount" & vbCrLf &
                "      ,TransReceivingPaymentChecks.AccountName AS AccountNameOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankID AS BankIDOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankName AS BankNameOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankBranchName AS BankBranchNameOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckNumber AS CheckNumberOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckDate AS CheckDateOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckAmount AS CheckAmountOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.AccountName2 AS AccountNameOnline2" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankID2 AS BankIDOnline2" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankName2 AS BankNameOnline2" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankBranchName2 AS BankBranchNameOnline2" & vbCrLf &
                "      ,'Online Payment' AS PaymentType" & vbCrLf &
                "      ,TransReceivingPaymentChecks.DepositStatus" & vbCrLf &
                "      FROM TransReceivingPaymentChecks" & vbCrLf &
                "      WHERE ISNULL(TransReceivingPaymentChecks.IsOnlinePayment, 0) = 1" & vbCrLf &
                "    ) as chk ON chk.PaymentID = cuspay.ID" & vbCrLf &
                "   -- 2020_10_20__08_27_11_8431 delete this line AND chkamt.InvoiceID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganization as cus ON cus.ID = cuspay.SupplierID" & vbCrLf &
                "   LEFT JOIN MobizLibraryPeoplePreNominalSuffix as cussuf ON cus.PersonTitleID = cussuf.ID" & vbCrLf &
                "   WHERE CONVERT(date, cuspay.LastUpdated) BETWEEN " & ReadyString(pDateFrom) & " AND " & ReadyString(pDateTo) & vbCrLf
            If String.IsNullOrWhiteSpace(pPaymentType) = False Then
                pSql &= " AND chk.paymenttype = 'check'" & vbCrLf
            End If
            pSql &= "     ORDER BY cuspay.LastUpdated ASC"

            Dim pIncludeDeposited As Boolean = CBool2(Common.WebRequest.Data("IncludeDeposited"))
            Dim pIncludeUndeposited As Boolean = CBool2(Common.WebRequest.Data("IncludeUndeposited"))
            pSql = "WITH CTE AS (" & vbCrLf &
                pSql & vbCrLf &
                ")" & vbCrLf &
                "SELECT *" & vbCrLf &
                "FROM CTE" & vbCrLf &
                "WHERE 1=1" & vbCrLf &
                "AND (1=0" & vbCrLf
            If pIncludeDeposited Then
                pSql &= " OR DepositStatus = 1" & vbCrLf
            End If
            If pIncludeUndeposited Then
                pSql &= " OR ISNULL(DepositStatus, 0) = 0" & vbCrLf
            End If
            pSql &= ")" & vbCrLf

            Dim pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Result", pDT)

        End Function

        <HttpPost>
        Public Function SupplierPaymentReport_old() As String
            ' preserve this old function 2022_12_22__17_05_19_0137

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql = " -- supplier payment report" & vbCrLf &
                " SELECT DISTINCT TOP 100 PERCENT  (CASE cus.IsOrganization      WHEN 1 THEN  cus.CompanyName      ELSE           ISNULL(NULLIF(LTRIM(RTRIM(cussuf.Code)) + ' ', ' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(cus.PersonSurname)) + ', ',', '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(cus.PersonFirstName)) + ' ',' '), '') +          ISNULL(NULLIF(LTRIM(RTRIM(cus.PersonMiddleName)) + ' ',' '), '') +          ISNULL(NULLIF(', ' + LTRIM(RTRIM(cus.PersonSuffixName)),', '), '')  END) AS [CustomerName]" & vbCrLf &
                " ,  ISNULL(CONVERT(varchar, fowst.FowstNo), '') + '-'" & vbCrLf &
                "          + ISNULL(CONVERT(varchar, device.DeviceNo), '') + '-'" & vbCrLf &
                "          + CONVERT(varchar, inv.AccountsPayableNum)" & vbCrLf &
                "  AS [InvoiceNumber]" & vbCrLf &
                " , inv.RefInvoiceNum AS [RefInvoiceNum]" & vbCrLf &
                " , ISNULL(cuspay.ManualReceiptNumber, '') AS [ManualReceiptNumber]" & vbCrLf &
                " , ISNULL(chk.AccountName, '') AS [AccountName]" & vbCrLf &
                " , ISNULL(chk.BankName, '') AS [BankName]" & vbCrLf &
                " , ISNULL(chk.BankBranchName, '') AS [BankBranch]" & vbCrLf &
                " , ISNULL(chk.CheckNumber, '') AS [CheckNumber]" & vbCrLf &
                " , ISNULL(chk.CheckAmount, '') AS [CheckAmount]" & vbCrLf &
                " , ISNULL(CONVERT(varchar, chk.CheckDate, 126), '') AS [CheckDate]" & vbCrLf &
                " , ISNULL(chk.CheckAmount, '') AS [CheckTotal]" & vbCrLf &
                " , ISNULL(chk.AccountNameOnline, '') AS [AccountNameOnline]" & vbCrLf &
                " , ISNULL(chk.BankNameOnline, '') AS [BankNameOnline]" & vbCrLf &
                " , ISNULL(chk.BankBranchNameOnline, '') AS [BankBranchOnline]" & vbCrLf &
                " , ISNULL(chk.CheckNumberOnline, '') AS [CheckNumberOnline]" & vbCrLf &
                " , ISNULL(chk.CheckAmountOnline, '') AS [CheckAmountOnline]" & vbCrLf &
                " , ISNULL(CONVERT(varchar, chk.CheckDateOnline, 126), '') AS [CheckDateOnline]" & vbCrLf &
                " , ISNULL(chk.CheckAmountOnline, '') AS [CheckTotalOnline]" & vbCrLf &
                " , ISNULL(chk.AccountNameOnline2, '') AS [AccountNameOnline2]" & vbCrLf &
                " , ISNULL(chk.BankNameOnline2, '') AS [BankNameOnline2]" & vbCrLf &
                " , ISNULL(chk.BankBranchNameOnline2, '') AS [BankBranchOnline2]" & vbCrLf &
                " , ISNULL(cuspay.CashAmount, '') AS [CashAmount]" & vbCrLf &
                " , cuspay.PaymentDate AS [PaymentDate]" & vbCrLf &
                " , cuspay.LastUpdated AS [LastUpdated]" & vbCrLf &
                " , ISNULL(CONVERT(varchar, fowst.FowstNo), '') + '-'" & vbCrLf &
                "  + ISNULL(CONVERT(varchar, device.DeviceNo), '') + '-'" & vbCrLf &
                "  + CONVERT(varchar, cuspay.PaymentNum) AS [ReceiptNumber]" & vbCrLf &
                " , ISNULL(fowst.Name, '') AS [FowstName]" & vbCrLf &
                " , ISNULL(chk.CheckAmount, 0) + ISNULL(chk.CheckAmountOnline, 0) + cuspay.CashAmount AS [Total]" & vbCrLf &
                " , cuspay.ID AS [CustomerPaymentID]" & vbCrLf &
                " , TransReceivingPaymentReceivings.ReceivingID AS ReceivingID" & vbCrLf &
                " --, inv.CustomerID AS [CustomerID]" & vbCrLf &
                " , chk.ID AS [CheckID]" & vbCrLf &
                " , CacheReceivings.GrossInvoiceAmount AS [GrossInvoiceAmount]" & vbCrLf &
                " , CacheReceivings.NetInvoiceAmount AS [NetInvoiceAmount]" & vbCrLf &
                " , CacheReceivings.TotalPayment AS [TotalPayment]" & vbCrLf &
                " , chkamt.AmountPaid AS [CheckAmountPaid]" & vbCrLf &
                " , CacheReceivings.InvoiceDueDate AS [InvoiceDueDate]" & vbCrLf &
                "   FROM TransReceivingPayment as cuspay" & vbCrLf &
                "   LEFT JOIN TransReceivingPaymentReceivings ON TransReceivingPaymentReceivings.PaymentID = cuspay.ID" & vbCrLf &
                "    LEFT JOIN (" & vbCrLf &
                "    	SELECT TransReceivingPaymentChecks.ID" & vbCrLf &
                "      ,TransReceivingPaymentChecks.PaymentID" & vbCrLf &
                "  	,TransReceivingPaymentChecks.IsOnlinePayment" & vbCrLf &
                "      ,TransReceivingPaymentChecks.AccountName" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankID" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankName" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankBranchName" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckNumber" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckDate" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckAmount" & vbCrLf &
                "  	  ,NULL AS AccountNameOnline" & vbCrLf &
                "      ,NULL AS BankIDOnline" & vbCrLf &
                "      ,NULL AS BankNameOnline" & vbCrLf &
                "      ,NULL AS BankBranchNameOnline" & vbCrLf &
                "      ,NULL AS CheckNumberOnline" & vbCrLf &
                "      ,NULL AS CheckDateOnline" & vbCrLf &
                "      ,NULL AS CheckAmountOnline" & vbCrLf &
                "      ,NULL AS AccountNameOnline2" & vbCrLf &
                "      ,NULL AS BankIDOnline2" & vbCrLf &
                "      ,NULL AS BankNameOnline2" & vbCrLf &
                "      ,NULL AS BankBranchNameOnline2" & vbCrLf &
                "      FROM TransReceivingPaymentChecks" & vbCrLf &
                "      WHERE ISNULL(TransReceivingPaymentChecks.IsOnlinePayment, 0) = 0" & vbCrLf &
                "      UNION" & vbCrLf &
                "      SELECT TransReceivingPaymentChecks.ID" & vbCrLf &
                "      ,TransReceivingPaymentChecks.PaymentID" & vbCrLf &
                "  	  ,TransReceivingPaymentChecks.IsOnlinePayment" & vbCrLf &
                "      ,NULL AS AccountName" & vbCrLf &
                "      ,NULL AS BankID" & vbCrLf &
                "      ,NULL AS BankName" & vbCrLf &
                "      ,NULL AS BankBranchName" & vbCrLf &
                "      ,NULL AS CheckNumber" & vbCrLf &
                "      ,NULL AS CheckDate" & vbCrLf &
                "      ,NULL AS CheckAmount" & vbCrLf &
                "      ,TransReceivingPaymentChecks.AccountName AS AccountNameOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankID AS BankIDOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankName AS BankNameOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankBranchName AS BankBranchNameOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckNumber AS CheckNumberOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckDate AS CheckDateOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.CheckAmount AS CheckAmountOnline" & vbCrLf &
                "      ,TransReceivingPaymentChecks.AccountName2 AS AccountNameOnline2" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankID2 AS BankIDOnline2" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankName2 AS BankNameOnline2" & vbCrLf &
                "      ,TransReceivingPaymentChecks.BankBranchName2 AS BankBranchNameOnline2" & vbCrLf &
                "      FROM TransReceivingPaymentChecks" & vbCrLf &
                "      WHERE ISNULL(TransReceivingPaymentChecks.IsOnlinePayment, 0) = 1" & vbCrLf &
                "    ) as chk ON chk.PaymentID = cuspay.ID" & vbCrLf &
                "   LEFT JOIN TransReceivingPaymentCheckAmount as chkamt ON chk.ID = chkamt.PaymentCheckID" & vbCrLf &
                "   LEFT JOIN TransReceiving ON TransReceiving.ID = TransReceivingPaymentReceivings.ReceivingID" & vbCrLf &
                "   LEFT JOIN TransInvoice as inv ON inv.ID = TransReceiving.AccountsPayableID" & vbCrLf &
                "   -- 2020_10_20__08_27_11_8431 delete this line AND chkamt.InvoiceID = TransCustomerPaymentInvoices.InvoiceID" & vbCrLf &
                "   LEFT JOIN CacheReceivings ON CacheReceivings.ID = inv.ID" & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganization as cus ON cus.ID = cuspay.SupplierID" & vbCrLf &
                "   LEFT JOIN MobizLibraryPeoplePreNominalSuffix as cussuf ON cus.PersonTitleID = cussuf.ID" & vbCrLf &
                "   LEFT JOIN LibraryDocumentControl as doccon ON doccon.ID = inv.DocumentNameID" & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = inv.FowstID" & vbCrLf &
                "   LEFT JOIN LibraryDevice as device ON device.ID = inv.DeviceID" & vbCrLf &
                "   WHERE CONVERT(date, cuspay.LastUpdated) BETWEEN " & ReadyString(Format(CDate(Common.WebRequest.Data("From")), cGlobal.FormatDateTimeSql)) & " AND " & ReadyString(Format(CDate(Common.WebRequest.Data("To")), cGlobal.FormatDateTimeSql)) & vbCrLf &
                "  AND (" & vbCrLf &
                "   	    chkamt.id is null" & vbCrLf &
                "    	OR chkamt.ReceivingID = TransReceivingPaymentReceivings.ReceivingID" & vbCrLf &
                "  )" & vbCrLf &
                "  /*" & vbCrLf &
                "  AND (" & vbCrLf &
                "    cuspay.FowstID = 'fe639d1f-cd83-4b3e-8b0a-586731f479cb'   OR (cuspay.FowstID IS NULL AND inv.FowstID = 'fe639d1f-cd83-4b3e-8b0a-586731f479cb')" & vbCrLf &
                " )" & vbCrLf &
                " */" & vbCrLf &
                "     ORDER BY cuspay.LastUpdated ASC"
            Dim pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        <HttpPost>
        Public Function CustomerCreditTerms() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pIncludeApproved = CBool2(Common.WebRequest.Data("IncludeApproved"))
            Dim pIncludeUnapproved = CBool2(Common.WebRequest.Data("IncludeUnapproved"))

            Dim pDT = WebRequest.Instance.Business.LibraryOrganization.CustomerCreditTerms(pIncludeApproved, pIncludeUnapproved)
            Return JsonDictionary.Serialize(pDT)

        End Function

        <HttpPost>
        Public Function CancelCustomerPayment() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Customer Payments (EAS) Cancellation", False) = False Then
                Return JsonDictionary.Serialize("Error", "You need ""Customer Payments (EAS) Cancellation"" security access.")
            End If

            Dim pCustomerPaymentID = Common.WebRequest.Data("CustomerPaymentID")
            If String.IsNullOrWhiteSpace(pCustomerPaymentID) Then
                Return JsonDictionary.Serialize("Error", "CustomerPaymentID is required. ref: vbZ8AFXELYTS!")
            End If
            Dim pError As String
            If WebRequest.Instance.Business.TransCustomerPayment.CancelPayment(pCustomerPaymentID, pError, WebSession.Instance.PeopleUserID) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

        End Function
    End Class
End Namespace