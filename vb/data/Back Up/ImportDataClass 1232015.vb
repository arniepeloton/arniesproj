Imports System.Threading
Imports C1.C1Zip

Public Class ImportDataClass

    Public InputFilename As String
    Public OrgID As String
    Public Uuid As String
    Public DataImportLog As DataImportLog
    ReadOnly mBusiness As Business
    ReadOnly mFolder As String
    Dim mFreeUser As Boolean ' MobizLibrarySubscription.FreeUser

    Dim mDone As Boolean
    Public DeviceID As String
    Dim mThread As Threading.Thread
    Public AllowReimportNonZeroInvoice As Boolean
    Public ImportInvoiceOnly As Boolean
    'Public InvoiceNumbersToImport As String
    Public InvoiceIDToImport As List(Of String)
    Public EnableLogs As Boolean
    Const mLogFile As String = "ImportDataClass"
    ''' <summary>
    ''' optional. used for debugging only
    ''' </summary>
    ''' <remarks></remarks>
    Public UploadsFolder As String
    Event Done()

    Public Sub New()

        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
        mFolder = Common.ServerPath & "temp\pending import data replies\"
        If UploadsFolder Is Nothing Then
            UploadsFolder = Common.ServerPath & "temp\Uploads\"
        End If

    End Sub

    Public ReadOnly Property TimeStamp As String
        Get

            Return IO.Path.GetFileNameWithoutExtension(InputFilename)

        End Get
    End Property

    Public Sub Start()

        Dim pDR As DataRow

        If EnableLogs Then AppendToLog("vbD957WV6CVT! start: " & InputFilename, , mLogFile)

        If IsGuid(OrgID) = False Then
            Throw New Exception("OrgID must be a valid GUID before calling Start. vsGM5UAT9R35")
        End If

        If IsGuid(DeviceID) = False Then
            Throw New Exception("DeviceID must be a valid GUID before calling Start. vbH762G76ENN")
        End If

        pDR = mBusiness.LibraryOrganization.ByID(OrgID)
        If pDR Is Nothing Then
            Throw New Exception("OrgID (" & OrgID & ") point to a non-existing record. vbJBSFSVZZR5")
        End If
        If CStr2(pDR("Database")).Trim = "" Then
            Throw New Exception("OrgID (" & OrgID & ") has no database. vbFDPU3SCMLQ")
        End If
        If mBusiness.ProData.GetDatabases(True, True).Contains(CStr(pDR("Database")).Trim().ToLower()) = False Then
            Throw New Exception("OrgID (" & OrgID & ") point to a non-existing database, " & pDR("Database") & ". vbY5HWN6HWRD")
        End If
        mFreeUser = pDR("FreeUser")

        DataImportLog = New DataImportLog
        mBusiness.ProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))

        mThread = New Threading.Thread(CType(
            Sub()
                Dim pResult As String
                Dim pOk As Boolean

                Try
                    StartTransImport(mBusiness.ProData, TimeStamp)
                    pOk = Process(pResult)
                Catch e As Exception
                    Try
                        pResult = JsonDictionary.Serialize("Error", e.Message & ";" &
                            GetStackTrace(True, e))
                        UpdateTransImport(mBusiness.ProData, TimeStamp, "error", True)
                    Catch ex As Exception
                        Stop2()
                    End Try
                End Try
                Try
                    FileText(GetFilename()) = pResult
                    If pOk Then
                        UpdateTransImport(mBusiness.ProData, TimeStamp, "ok", True)
                    Else
                        UpdateTransImport(mBusiness.ProData, TimeStamp, "error", False)
                        If EnableLogs Then AppendToLog("vbTK7BK5K3YB! not ok: " & InputFilename & "; " & pResult, , mLogFile)
                    End If
                Catch ex As Exception
                    Stop2()
                End Try
                mDone = True
                RaiseEvent Done()
            End Sub, ThreadStart))
        mThread.Start()

    End Sub

    Public ReadOnly Property IsRunning As Boolean
        Get

            Return (mThread.ThreadState = Threading.ThreadState.Running) _
                OrElse (mThread.ThreadState = Threading.ThreadState.WaitSleepJoin)

        End Get
    End Property

    Public Function GetMessage() As String

        Return "Server is still processing imported data."

    End Function

    Public Function IsDone() As Boolean

        Return mDone

    End Function

    Public Function GetFilename() As String

        Return UploadsFolder & OrgID & "\" & DeviceID & "\" & InputFilename & "_result.txt"

    End Function

    Private Function Process(ByRef oMessage As String) As Boolean

        Dim pOut As JsonDictionary
        Dim pZipOut As ProZip
        Dim pError As String
        Dim pFilenameOut As String
        Dim pFilename As String
        Dim pFranchiseeDataExporter As FranchiseeDataExporter

        Dim pInfo As String
        Dim pInfoJson As JsonDictionary

        oMessage = ""
        If False Then
            Throw New Exception("Test exception vbWDVXFXZASB")
        End If

        If False Then
            Threading.Thread.Sleep(60 * 1000)
        End If

        pFilename = UploadsFolder & OrgID & "\" & DeviceID & "\" & InputFilename & ".zip"
        ExtractSignatures(pFilename)
        If EnableLogs Then AppendToLog("vbYSCAYK3M6G! Process " & pFilename, , mLogFile)
        pInfo = New ProZip(pFilename).GetString("info.json")
        If CStr2(pInfo).Trim = "" Then
            pError = "Info.json does not exist or is empty"
            oMessage = JsonDictionary.Serialize("Error", pError)
            Return False
        End If
        Try
            pInfoJson = New JsonDictionary()
            pInfoJson.SetJson(pInfo)
        Catch ex As System.Exception
            pError = "Info.json does not contain a valid JSON"
            oMessage = JsonDictionary.Serialize("Error", pError)
            Return False
        End Try

        Uuid = pInfoJson("UUID")
        If CStr2(Uuid).Trim = "" Then
            pError = "Info.json does not contain Uuid"
            oMessage = JsonDictionary.Serialize("Error", pError)
            Return False
        End If

        pFilenameOut = AppendBackslash(IO.Path.GetDirectoryName(pFilename)) & _
            TimeStamp & "_imported.zip"
        CreateFolder(pFilenameOut)
        pZipOut = New ProZip(pFilenameOut)
        pOut = New JsonDictionary

        If ImportInvoiceOnly = False Then
            mBusiness.ProData.BeginTransaction()
            ' additional by janine
            If ImportLibraryOrganization(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.AddLog(pError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            mBusiness.ProData.CommitTransaction()
            ' additional by janine
            If ImportLibraryOrganizationFOWST(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.AddLog(pError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            ' end of additional
        End If

        If ImportInvoice(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
            DataImportLog.AddLog("vbUTND9W94VW! Filename: " & pFilename)
            DataImportLog.AddLog("vb2A9K26NW9E! pError: " & pError)
            DataImportLog.Save(DataImportLog.eResultType.Fail)
            oMessage = JsonDictionary.Serialize("Error", pError)
            Return False
        End If

        If ImportMaterialsRequisition(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
            DataImportLog.Save(DataImportLog.eResultType.Fail)
            oMessage = JsonDictionary.Serialize("Error", pError)
            Return False
        End If

        If ImportInvoiceOnly = False Then
            ' additional by francis sept 04, 2014
            If ImportLibraryFOWSTBin(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.AddLog(pError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If

            ' additional by francis sept 04, 2014
            If ImportLibraryFOWSTBinProduct(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.AddLog(pError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If

            mBusiness.ProData.BeginTransaction()
            ' additional by francis sept 03, 2014
            If ImportTransInventoryProductCount(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.AddLog(pError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            mBusiness.ProData.CommitTransaction()

            If ImportReceiving(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If

            If ImportProductBarcode(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If

            If ImportCashHandling(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            If ImportBomMaker(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If


            If ImportLibraryDevicePrinterFowst(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            If ImportLibraryDeviceFowstPrinter(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
        End If

        pFranchiseeDataExporter = New FranchiseeDataExporter(mBusiness.ProData.Clone)
        TimedThread(
            Sub()
                Try
                    pFranchiseeDataExporter.CopyRecords()
                Catch ex As Exception
                    AppendToLog("vbHAMRWRBVME! " & ex.Message & vbCrLf & GetStackTrace(, ex), , "FranchiseeDataExporter")
                End Try
            End Sub)
        Try
            Call New RowCountChecksum(OrgID, DeviceID, pFilename).Process()
        Catch ex As Exception
            AppendToLog("vbH9JDY7BW98! Error in RowCountChecksum: " & ex.Message)
        End Try
        If pZipOut.GetFileCount > 0 Then
            pOut.Add("Filename", InputFilename)
        End If
        pOut.Add("Result", "ok")
        DataImportLog.Save(DataImportLog.eResultType.Success)
        oMessage = pOut.Serialize

        Return True

    End Function

    Private Sub ExtractSignatures(vZipFilename As String)

        Dim pZip As C1ZipFile
        Dim pFolder As String
        Dim pFilename As String

        Try
            pZip = New C1ZipFile(vZipFilename)
            For Each pEntry As C1ZipEntry In pZip.Entries
                If pEntry.Attributes And IO.FileAttributes.Directory Then
                    Continue For
                End If
                pFolder = IO.Path.GetDirectoryName(pEntry.FileName)
                If pFolder.ToLower() = "signatures" Then
                    pFilename = Common.ServerPath & "temp\Uploads\SalesOrderSignature\" & OrgID & "\" & IO.Path.GetFileName(pEntry.FileName)
                    CreateFolder(pFilename)
                    pEntry.Extract(pFilename)
                End If
            Next pEntry
            pZip.Close()
        Catch ex As Exception
            AppendToLog("vbTUCNEFCYS9! Error in ExtractSignatures. Filename: " & vZipFilename & "; Error: " & ex.Message)
        End Try

    End Sub

    Private Function ImportInvoice(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut, pDtOutItems As DataTable
        Dim pDrOut As DataRow
        Dim pNum As Integer
        Dim pDtItems As DataTable
        Dim pCreator As TransInvoice.Creator
        Dim mItemInitialized As Boolean
        Dim pDrInvoice As DataRow
        Dim pDataImportLogAlreadyImported As DataImportLog
        Dim pMaxSubNumber As Integer?
        Dim pExistingInvoiceCount As Integer ' applicable only to free user

        ' customer payments
        Dim pDtCustomerPayments As DataTable
        Dim pBinderCustomerPayments As DictionaryBinder
        Dim pDtOutCustomerPayments As DataTable

        Dim pDtCustomerPaymentInvoices As DataTable
        Dim pBinderCustomerPaymentInvoices As DictionaryBinder

        Dim pDtCustomerPaymentDebitCards As DataTable
        Dim pBinderCustomerPaymentDebitCards As DictionaryBinder

        Dim pDtCustomerPaymentCreditCards As DataTable
        Dim pBinderCustomerPaymentCreditCards As DictionaryBinder

        Dim pDtCustomerPaymentChecks As DataTable
        Dim pBinderCustomerPaymentChecks As DictionaryBinder

        Dim pDtCustomerPaymentOther As DataTable
        Dim pBinderCustomerPaymentOther As DictionaryBinder


        oError = ""

        If EnableLogs Then AppendToLog("vbETEZZZNJ73 ImportInvoice " & InputFilename, , mLogFile)
        If mFreeUser Then
            ' pExistingInvoiceCount = mBusiness.TransInvoice.GetCountByMonth(ParseDate(pLine("InvoiceDate"))) ' use this if you want to determine the month by invoice date
            pExistingInvoiceCount = mBusiness.TransInvoice.GetCountByMonth()
            If TransInvoice.FreeUserMaxInvoiceCount <= pExistingInvoiceCount Then
                mBusiness.ProData.RollbackTransaction(True, True)
                oError = "As a free user, you have exceeded your maximum allowed number of invoices of " & TransInvoice.FreeUserMaxInvoiceCount & " for the month of " & Format(ParseDate(pLine("InvoiceDate")), "MMMM yyyy")
                Return False
            End If
        End If

        pDtCustomerPayments = CreateCustomerPaymentsDataTable(vInputZipFilename, pBinderCustomerPayments, oError)
        If pDtCustomerPayments Is Nothing Then
            Return False
        End If

        pDtCustomerPaymentInvoices = CreateCustomerPaymentInvoicesDataTable(vInputZipFilename, pBinderCustomerPaymentInvoices, oError)
        If pDtCustomerPaymentInvoices Is Nothing Then
            Return False
        End If

        pDtCustomerPaymentDebitCards = CreateCustomerPaymentDebitCardDataTable(vInputZipFilename, pBinderCustomerPaymentDebitCards, oError)
        If pDtCustomerPaymentDebitCards Is Nothing Then
            Return False
        End If

        pDtCustomerPaymentCreditCards = CreateCustomerPaymentCreditCardDataTable(vInputZipFilename, pBinderCustomerPaymentCreditCards, oError)
        If pDtCustomerPaymentCreditCards Is Nothing Then
            Return False
        End If

        pDtCustomerPaymentChecks = CreateCustomerPaymentCheckDataTable(vInputZipFilename, pBinderCustomerPaymentChecks, oError)
        If pDtCustomerPaymentChecks Is Nothing Then
            Return False
        End If

        pDtCustomerPaymentOther = CreateCustomerPaymentOtherDataTable(vInputZipFilename, pBinderCustomerPaymentOther, oError)
        If pDtCustomerPaymentOther Is Nothing Then
            Return False
        End If

        DataImportLog.AddLog("Import from TransInvoice")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoice")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceNumber", True, SqlDbType.Int)
            .Add("InvoiceDate", True, SqlDbType.DateTime)
            .Add("CustomerID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("BillingAddressID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("DeliveryAddressID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("DeviceID", True, SqlDbType.UniqueIdentifier)
            .Add("UserID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("RefNum", False, SqlDbType.VarChar)
            .Add("ReasonForReconstructingData", False, SqlDbType.VarChar)
            .Add("Remarks", False, SqlDbType.VarChar)
            .Add("DeliveryDate", False, SqlDbType.VarChar) ' ideally should be SqlDbType.DateTime
            .Add("IsCancelled", False, SqlDbType.Bit)
            .Add("LastUpdated", True, SqlDbType.DateTime)
            .Add("SalesOrderNo", False, SqlDbType.Int)
            .Add("SalesOrderDate", False, SqlDbType.Date)
            .Add("SalesOrderUserID", False, SqlDbType.UniqueIdentifier)
            .Add("CustomerTableNo", False, SqlDbType.VarChar)
            .Add("IsForGovReport", False, SqlDbType.Bit)
            .Add("TotalAmount", False, SqlDbType.Money)
            .Add("IsTestOnly", False, SqlDbType.Bit) ' added by janine. for the self test
            .Add("PurchaseOrderNo", False, SqlDbType.VarChar) ' added by janine. for sales order taker aug 6
            .Add("PurchaseOrderDate", False, SqlDbType.Date) ' added by janine. for sales order taker aug 6
            .Add("PurchaserName", False, SqlDbType.VarChar) ' added by janine. for sales order taker aug 6
            .Add("SalesOrderSignatory", False, SqlDbType.VarChar) ' added by janine. for sales order taker signature oct 8
            .Add("DeliveryMethod", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("OrderMethod", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("CustomerDemographicGender", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("CustomerDemographicAge", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("FranchiseeOrgID", False, SqlDbType.VarChar) ' added by francis. Nov 3
            .Add("SupplierID", False, SqlDbType.UniqueIdentifier)
            .Add("SupplierFowstID", False, SqlDbType.UniqueIdentifier) ' ref: 2015_08_31__11_05_36_0178
            .Add("InvoiceTypeCode", False, SqlDbType.VarChar)
            .Add("ReceivingID", False, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Dim dtAdjustmentItems As Date
        Dim IsHasPossibleReconstruction As Boolean

        Do
            IsHasPossibleReconstruction = False
            mBusiness.ProData.RollbackTransaction(True, True)

            If False Then
                Throw New Exception("Test exception vbWS8E53NK2C")
            End If

            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do

            'additional by janine sept 8 2014 5pm for sales order taker
            Dim posmode As String
            If pLine.ContainsKey("SalesOrderNo") AndAlso CStr2(pLine("SalesOrderNo")).Trim <> "" Then
                posmode = "so"
            Else
                posmode = "pos"
            End If
            ''''''''''''''''''''

            If pLine.ContainsKey("BillingAddressID") = False _
                OrElse IsGuid(pLine("BillingAddressID")) = False Then

                pLine("BillingAddressID") = mBusiness.SystemDefaults.GetSystemDefault("POS Billing Address ID")
            End If
            If pLine.ContainsKey("DeliveryAddressID") = False _
                OrElse IsGuid(pLine("DeliveryAddressID")) = False Then

                pLine("DeliveryAddressID") = mBusiness.SystemDefaults.GetSystemDefault("POS Delivery Address ID")
            End If

            If mBusiness.LibraryPeople.Validate(
                vPeopleID:=pLine("UserID"),
                vOrgID:=vOrgID,
                vFowstID:=pLine("FowstID"),
                vPosMode:=posmode,
                vDeviceID:=pLine("DeviceID"),
                vVerifyUuid:=True,
                vDeviceUuid:=vUuid,
                oError:=oError,
                vDontVerifyPeopleFowst:=True) = False Then

                Exit Do
            End If

            If mItemInitialized = False Then
                mItemInitialized = True
                pDtItems = CreateInvoiceItemsDataTable(vInputZipFilename, oError)
                If pDtItems Is Nothing Then
                    Return False
                End If
            End If
            If EnableLogs Then AppendToLog("vbNWBHR83QPC! pDtItems.Rows.Count:" & pDtItems.Rows.Count, , mLogFile)
            If IsGuid(pLine("ID")) Then
                pDrInvoice = mBusiness.TransInvoice.ById(pLine("ID"))
                If pDrInvoice IsNot Nothing _
                    AndAlso CDec2(pDrInvoice("InvoiceNumber")) <> 0 _
                    AndAlso AllowReimportNonZeroInvoice = False Then

                    If pDataImportLogAlreadyImported Is Nothing Then
                        pDataImportLogAlreadyImported = New DataImportLog
                        pDataImportLogAlreadyImported.AddLog("InputZipFilename: " & vInputZipFilename)
                        pDataImportLogAlreadyImported.AddLog("OrgID: " & OrgID)
                    End If
                    pDataImportLogAlreadyImported.AddLog("InvoiceID: " & pLine("ID"))
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)
                    Continue Do
                End If
                'mBusiness.TransInvoice.VerifyCounter(pLine("DeviceID"), pLine("FowstID"), pNum)
                'If CDec2(pLine("InvoiceNumber")) <= pNum Then
                '    DataImportLog.AddLog("Existing max invoice number/counter is: " & pNum & " but the invoice number to be imported is " & pLine("InvoiceNumber") & ". Imported number will become " & pNum + 1 & ". ID: " & pLine("ID") & ". Ref vbXAJ5YY4ELK")
                '    pLine("InvoiceNumber") = pNum + 1 ' TODO: apply business logic here
                '    Stop2()
                'ElseIf CDec2(pLine("InvoiceNumber")) <> pNum + 1 Then
                '    DataImportLog.AddLog("The invoice number to be imported is " & pLine("InvoiceNumber") & ". Imported number must be " & pNum + 1 & ". ID: " & pLine("ID") & ". Ref vbPA8DFTRKQX")
                '    Stop2()
                'End If
            Else
                Continue Do
            End If

            If CDec2(pLine("InvoiceNumber")) < 0 Then
                pLine("InvoiceNumber") = 0 ' prevent negative number
            End If
            If pLine.ContainsKey("DeliveryDate") Then
                If IsDate(pLine("DeliveryDate")) = False Then
                    pLine.Remove("DeliveryDate")
                End If
            End If
            '' special case ni xa
            'If pLine("LastUpdated").Trim.ToLower = "Thu Mar 20 2014 18:08:43 GMT+0800 (PHT)".ToLower Then
            '    pLine("LastUpdated") = "2014-03-20 18:08:43"
            'End If
            'If pLine("LastUpdated").Trim.ToLower = "Fri Mar 21 2014 16:56:47 GMT+0800 (PHT)".ToLower Then
            '    pLine("LastUpdated") = "2014-03-21 16:56:47"
            'End If
            'If pLine("LastUpdated").Trim.ToLower = "Sun Mar 23 2014 17:22:33 GMT+0800 (PHT)".ToLower Then
            '    pLine("LastUpdated") = "2014-03-23 17:22:33"
            'End If
            If ImportInvoiceOnly Then
                If InvoiceIDToImport Is Nothing Then
                    Throw New NullReferenceException("vbJAXZHHF5ET! InvoiceIDToImport must not be null when ImportInvoiceOnly = true")
                End If
                If InvoiceIDToImport.Contains(ReadyGuid(pLine("ID"))) = False Then
                    Continue Do
                End If
            End If

            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Exit Do
            End If
            If CDec2(pLine("InvoiceNumber")) <= 0 _
                AndAlso (
                    pLine.ContainsKey("SalesOrderNo") = False _
                    OrElse CDec2(pLine("SalesOrderNo")) <= 0 _
                ) _
                AndAlso (
                    pLine.ContainsKey("PurchaseOrderNo") = False _
                    OrElse CDec2(pLine("PurchaseOrderNo")) <= 0 _
                ) Then

                DataImportLog.AddLog("ID " & pLine("ID") & " has an invoice number of " & CDec2(pLine("InvoiceNumber")) & " and will not be imported. This row will be tagged as imported on the device. Ref VZ8KTV7LAU")
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
                Continue Do
            End If

            pCreator = New TransInvoice.Creator(mBusiness.ProData, mBusiness.TransInvoice)
            pCreator.ID = pLine("ID")
            pCreator.InvoiceNumber = pLine("InvoiceNumber")
            pCreator.InvoiceDate = ParseDate(pLine("InvoiceDate"))
            pCreator.FowstID = pLine("FowstID")
            pCreator.DeviceID = pLine("DeviceID")
            If pLine.ContainsKey("CustomerID") _
                AndAlso IsGuid(pLine("CustomerID")) Then

                pCreator.CustomerID = pLine("CustomerID")
            Else
                pCreator.CustomerID = mBusiness.SystemDefaults.CustomerOrganizationID
            End If

            If pLine.ContainsKey("TotalAmount") Then
                pCreator.TotalAmount = CDec2(pLine("TotalAmount"))
            End If
            pCreator.DeliveryAddressID = pLine("DeliveryAddressID")
            pCreator.BillingAddressID = pLine("BillingAddressID")
            pCreator.LastUpdated = ParseDate(pLine("LastUpdated"))
            pMaxSubNumber = mBusiness.TransInvoice.GetMaxSubNumber( _
                vInvoiceNumber:=pLine("InvoiceNumber"),
                vDeviceID:=pLine("DeviceID"),
                vFowstID:=pLine("FowstID"))
            If pMaxSubNumber IsNot Nothing Then
                pCreator.SubNumber = pMaxSubNumber + 1
            End If

            pLine.TryGetValue("RefNum", pCreator.RefNum)
            pLine.TryGetValue("ReasonForReconstructingData", pCreator.ReasonForReconstructingData)
            pLine.TryGetValue("InvoiceTypeCode", pCreator.InvoiceTypeCode)

            pCreator.UserID = pLine("UserID")
            If pLine.ContainsKey("Remarks") Then
                pCreator.Remarks = Truncate(pLine("Remarks"), 999)
            End If
            If pLine.ContainsKey("SalesOrderNo") AndAlso CStr2(pLine("SalesOrderNo")).Trim <> "" Then
                pCreator.SalesOrderNo = pLine("SalesOrderNo")
            End If
            If pLine.ContainsKey("SalesOrderDate") AndAlso CStr2(pLine("SalesOrderDate")).Trim <> "" Then
                pCreator.SalesOrderDate = ParseDate(pLine("SalesOrderDate"))
            End If
            If pLine.ContainsKey("SalesOrderUserID") AndAlso CStr2(pLine("SalesOrderUserID")).Trim <> "" Then
                pCreator.SalesOrderUserID = pLine("SalesOrderUserID")
            End If
            If pLine.ContainsKey("CustomerTableNo") AndAlso CStr2(pLine("CustomerTableNo")).Trim <> "" Then
                pCreator.CustomerTableNo = pLine("CustomerTableNo")
            End If
            ' additional by janine june 2
            If pLine.ContainsKey("IsForGovReport") AndAlso CStr2(pLine("IsForGovReport")).Trim <> "" Then
                pCreator.IsForGovReport = pLine("IsForGovReport")
            End If
            If pLine.ContainsKey("IsTestOnly") AndAlso CStr2(pLine("IsTestOnly")).Trim <> "" Then
                pCreator.IsTestOnly = pLine("IsTestOnly")
            End If
            'additional aug 6
            If pLine.ContainsKey("PurchaseOrderNo") AndAlso CStr2(pLine("PurchaseOrderNo")).Trim <> "" Then
                pCreator.PurchaseOrderNo = pLine("PurchaseOrderNo")
            End If
            If pLine.ContainsKey("PurchaseOrderDate") AndAlso CStr2(pLine("PurchaseOrderDate")).Trim <> "" Then
                pCreator.PurchaseOrderDate = pLine("PurchaseOrderDate")
            End If
            If pLine.ContainsKey("DeliveryDate") AndAlso CStr2(pLine("DeliveryDate")).Trim <> "" Then
                pCreator.SODeliveryDate = pLine("DeliveryDate")
            End If
            If pLine.ContainsKey("PurchaserName") AndAlso CStr2(pLine("PurchaserName")).Trim <> "" Then
                pCreator.PurchaserName = pLine("PurchaserName")
            End If
            If pLine.ContainsKey("SalesOrderSignatory") AndAlso CStr2(pLine("SalesOrderSignatory")).Trim <> "" Then
                pCreator.SalesOrderSignatory = pLine("SalesOrderSignatory")
            End If
            'oct 24
            If pLine.ContainsKey("DeliveryMethod") AndAlso CStr2(pLine("DeliveryMethod")).Trim <> "" Then
                pCreator.DeliveryMethod = pLine("DeliveryMethod")
            End If
            If pLine.ContainsKey("OrderMethod") AndAlso CStr2(pLine("OrderMethod")).Trim <> "" Then
                pCreator.OrderMethod = pLine("OrderMethod")
            End If
            If pLine.ContainsKey("CustomerDemographicGender") AndAlso CStr2(pLine("CustomerDemographicGender")).Trim <> "" Then
                pCreator.CustomerDemographicGender = pLine("CustomerDemographicGender")
            End If
            If pLine.ContainsKey("CustomerDemographicAge") AndAlso CStr2(pLine("CustomerDemographicAge")).Trim <> "" Then
                pCreator.CustomerDemographicAge = pLine("CustomerDemographicAge")
            End If
            If pLine.ContainsKey("SupplierFowstID") AndAlso CStr2(pLine("SupplierFowstID")).Trim <> "" Then
                pCreator.SupplierFowstID = pLine("SupplierFowstID")
            End If

            If CInt2(pLine("InvoiceNumber")) > 0 Then
                pCreator.FranchiseeOrgID = IIf(GetFranchiseeOrgFOWSTID(pLine("FowstID")), pLine("FowstID"), Nothing)
            End If
            If pLine.ContainsKey("SupplierID") Then
                pCreator.SupplierID = pLine("SupplierID")
            End If

            If pLine.ContainsKey("ReceivingID") Then
                pCreator.ReceivingID = pLine("ReceivingID")
            End If

            'end of additional

            'pCreator.ProductID = pLine("ProductID")
            'pCreator.SalesRepList = Common.WebRequest.AllData("SalesRep")
            'pCreator.ProductSerialList = Common.WebRequest.AllData("ProductSerial")

            pIU = pBinder.CreateInsertUpdate(oError) ' not acctually used. just used for checking (see code below)
            If pIU Is Nothing Then
                Continue Do
            End If

            dtAdjustmentItems = InventoryAdjustment(pLine("FowstID"))

            'Check for adjustment
            If (DateDiff(DateInterval.Second, CDate(pLine("InvoiceDate")), dtAdjustmentItems) > 0) Then
                IsHasPossibleReconstruction = True
            End If

            Try
                mBusiness.ProData.BeginTransaction()
                If mBusiness.ProData.TransDepth <= 0 Then
                    Throw New Exception("You must be in a transaction. vb2X7W86WQBX")
                End If

                If AllowReimportNonZeroInvoice Then
                    AppendToLog("zeroing invoice# " & pLine("InvoiceNumber"), , "Reimport")
                    ' set InvoiceNumber to zero before deleting items
                    pSql = "UPDATE TransInvoice" & vbCrLf &
                        " SET InvoiceNumber = 0" &
                        " WHERE ID = " & ReadyString(pLine("ID"))
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                pSql = "DELETE FROM TransInvoiceItemSalesReps" & vbCrLf &
                    " WHERE InvoiceID = " & ReadyString(pLine("ID"))
                mBusiness.ProData.ExecuteSQL(pSql)

                ' trigger (TransInvoiceItems_trd) will cause error for deleting items from non-zero invoices 
                pSql = "DELETE FROM TransInvoiceItems" & vbCrLf &
                    " WHERE InvoiceID = " & ReadyString(pLine("ID"))
                mBusiness.ProData.ExecuteSQL(pSql)
                If pDtItems Is Nothing Then
                    pCreator.Stocks = Nothing
                Else
                    pCreator.Stocks = New DataView(pDtItems, "InvoiceID = " & ReadyString(pLine("ID")), "", DataViewRowState.CurrentRows).ToTable
                    If pDtOutItems Is Nothing Then
                        pDtOutItems = pCreator.Stocks
                    Else
                        pDtOutItems.Merge(pCreator.Stocks)
                    End If
                End If
                If AllowReimportNonZeroInvoice Then
                    If pCreator.Stocks Is Nothing Then
                        AppendToLog("invoice# " & pLine("InvoiceNumber") & " has no items", , "Reimport")
                    Else
                        AppendToLog("invoice# " & pLine("InvoiceNumber") & " has " & pCreator.Stocks.Rows.Count & " items", , "Reimport")
                    End If
                End If
                If pCreator.Save(False, oError) = False Then
                    AppendToDebug("vbM823DUDP23 " & oError)
                    'mBusiness.ProData.RollbackTransaction(True, True)
                    mBusiness.ProData.RollbackTransaction()
                    Exit Do
                End If

                If ReadyGuid(OrgID) = ReadyGuid(ErpToMobizExporter.OrgID) Then
                    If mBusiness.Utilities.HasWholesaleDatabase Then
                        DataImportLog.AddLog("HasWholesaleDatabase = True")
                        Dim pInvoiceToOrder As InvoiceToOrder
                        pInvoiceToOrder = New InvoiceToOrder(mBusiness.ProData, CreateWholesaleProData())
                        pInvoiceToOrder.Execute(pCreator.ID)
                    Else
                        DataImportLog.AddLog("HasWholesaleDatabase = False")
                    End If
                End If

                mBusiness.TransInvoice.UpdateCounter(pLine("DeviceID"), pLine("FowstID"), pLine("InvoiceNumber"))
                If pLine.ContainsKey("SalesOrderNo") _
                    AndAlso IsNumeric(pLine("SalesOrderNo")) Then

                    mBusiness.TransInvoice.UpdateSoCounter(pLine("DeviceID"), pLine("FowstID"), pLine("SalesOrderNo"))
                End If

                If CDec2(pLine("InvoiceNumber")) > 0 Then
                    mBusiness.TransInvoice.AddToInventoryLedger(pLine("ID"), vOrgID, IsHasPossibleReconstruction)
                End If

                If ImportInvoiceItemSalesReps(vInputZipFilename, pLine("ID"), oError, pCreator.Stocks) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                If ImportInvoiceItemRebates(vInputZipFilename, pLine("ID"), pLine("FowstID"), pLine("UserID"), oError) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                If ImportInvoiceItemSerials(vInputZipFilename, pLine("ID"), oError) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                If ImportInvoiceDiscount(vInputZipFilename, pLine("ID"), oError) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                If ImportInvoiceSeniorCitizenPWDCustomer(vInputZipFilename, pLine("ID"), oError) = False Then 'additional by janine july 26
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                If ImportInvoiceCharges(vInputZipFilename, pLine("ID"), oError) = False Then 'additional by janine jan 19 2015
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                If ImportCustomerPayments(vDT:=pDtCustomerPayments,
                    vBinder:=pBinderCustomerPayments,
                    rDtOut:=pDtOutCustomerPayments,
                    rDtOutInvoices:=pDtCustomerPaymentInvoices,
                    oError:=oError,
                    vInvoiceID:=pLine("ID"),
                    vDtCustomerPaymentInvoices:=pDtCustomerPaymentInvoices,
                    vBinderCustomerPaymentInvoices:=pBinderCustomerPaymentInvoices,
                    vDtCustomerPaymentDebitCards:=pDtCustomerPaymentDebitCards,
                    vBinderCustomerPaymentDebitCards:=pBinderCustomerPaymentDebitCards,
                    vDtCustomerPaymentCreditCards:=pDtCustomerPaymentCreditCards,
                    vBinderCustomerPaymentCreditCards:=pBinderCustomerPaymentCreditCards,
                    vDtCustomerPaymentChecks:=pDtCustomerPaymentChecks,
                    vBinderCustomerPaymentChecks:=pBinderCustomerPaymentChecks,
                    vDtCustomerPaymentOther:=pDtCustomerPaymentOther,
                    vBinderCustomerPaymentOther:=pBinderCustomerPaymentOther) = False Then

                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If

                mBusiness.ProData.CommitTransaction()
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction(True, True)
                If pDtOut.Rows.Count > 0 Then
                    Exit Do
                Else
                    AppendToDebug("vbLWDB6NFYCA", , ex, True)
                    Throw
                End If
            End Try
        Loop

        If pDtOut.Rows.Count = 0 Then
            If CStr2(oError).Trim <> "" Then
                Return False
            End If
        End If

        Dim pDtVerified As DataTable

        pDtVerified = VerifiedIDs(pDtOut, "TransInvoice")
        rZipOut.AddTableCsv(pDtVerified, "TransInvoice")
        If pDtOutCustomerPayments IsNot Nothing Then
            rZipOut.AddTableCsv(pDtOutCustomerPayments, "TransCustomerPayment")
        End If

        ' TransInvoiceItems is not taggerd anymore. June 12, 2014
        'If pDtOutItems IsNot Nothing _
        '    AndAlso pDtOutItems.Rows.Count > 0 Then

        '    pDtOutItems = New DataView(pDtOutItems, "", "", DataViewRowState.CurrentRows).ToTable(True, "ID")
        '    rZipOut.AddTableCsv(pDtOutItems, "TransInvoiceItems")
        'End If

        If pDataImportLogAlreadyImported IsNot Nothing Then
            pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
        End If

        Return True

    End Function

    Private Function VerifiedIDs(ByVal vDT As DataTable, ByVal vTableName As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pOut As DataTable

        If vDT.Rows.Count = 0 Then
            Return vDT
        End If
        pDT = ExtractFieldValues(vDT, "ID", Integer.MaxValue)
        For Each pDR As DataRow In pDT.Rows
            pSql = "SELECT ID FROM " & EscapeSqlName(vTableName) &
                " WHERE ID IN (" & pDR(0) & ")"
            pOut = mBusiness.ProData.Execute(pSql)
        Next pDR

        Return pOut

    End Function

    Private Function CreateWholesaleProData() As ProData

        Return ProData.CreateFromConnectionInfo( _
            vServer:=Common.ConnectionInfo.Server, _
            vDatabase:="wholesale", _
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity, _
            vUsername:=Common.ConnectionInfo.Username, _
            vPassword:=Common.ConnectionInfo.Password, _
            vNoProgressBar:=True)

    End Function

    Private Function CreateInvoiceItemsDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)

        ' BEGIN get SKU data from database
        'Dim pDtStocks As DataTable
        'Dim pDvStocks As DataView
        'Dim pDrvStock As DataRowView
        ' END get SKU data from database

        DataImportLog.AddLog("Import from TransInvoiceItems")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItems")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            '.Add("PriceLevelID", Not ImportInvoiceOnly, SqlDbType.UniqueIdentifier) ' should be true; false to force import
            .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier) ' should be true; false to force import
            .Add("TaxSales", False, SqlDbType.Money)
            .Add("SubTotal", True, SqlDbType.Money)
            .Add("LineNumber", True, SqlDbType.Int)
            .Add("OrderedQuantity", False, SqlDbType.Decimal) ' additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("ActualQuantity", True, SqlDbType.Int)
            .Add("UnitPrice", True, SqlDbType.Money)
            .Add("OrignalUnitPrice", False, SqlDbType.Money) ' required but be optional for backwards compatibility
            .Add("TaxAmount", False, SqlDbType.Money)
            .Add("TaxPercent", False, SqlDbType.Money)
            '.Add("UnitMeasure", Not ImportInvoiceOnly, SqlDbType.VarChar) ' should be true; false to force import
            .Add("UnitMeasure", False, SqlDbType.VarChar) ' should be true; false to force import

            .Add("DiscountAmount", False, SqlDbType.Money)
            .Add("DiscountSales", False, SqlDbType.Money)
            .Add("RequestedUnitPrice", False, SqlDbType.Money)
            .Add("VATExemptSale", False, SqlDbType.Money)
            .Add("ZeroRatedSale", False, SqlDbType.Money)
            .Add("NonVATSale", False, SqlDbType.Money)
            .Add("VATableSale", False, SqlDbType.Money)
            .Add("ModifierGroupID", False, SqlDbType.UniqueIdentifier)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("InvoiceID")
            .Add("ProductID")
            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", GetType(Decimal))
            .Add("ActualQuantity", GetType(Integer))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("OriginalUnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            .Add("ModifierGroupID")
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)

            'If ImportInvoiceOnly Then
            If CStr2(pDR("PriceLevelID")).Trim() = "" Then
                AppendToLog("vbTJKFPT8A5M! No PriceLevelID;" & vbCrLf &
                    "ID: " & pLine("ID") & "; " & vbCrLf &
                    "InvoiceID: " & pLine("InvoiceID") & "; " & vbCrLf &
                    "filename: " & InputFilename, , mLogFile)
                pDR("PriceLevelID") = mBusiness.SystemDefaults.GetRetailPriceID()
                'AppendToDebug("vbPN5RXKYB62! No PriceLevelID;" & vbCrLf &
                '    "filename: " & InputFilename)
            End If
            'End If
            If CStr2(pDR("UnitMeasure")).Trim() = "" Then
                AppendToLog("vbLKWXDVTJKA! No UnitMeasure;" & vbCrLf &
                    "ID: " & pLine("ID") & "; " & vbCrLf &
                    "InvoiceID: " & pLine("InvoiceID") & "; " & vbCrLf &
                    "filename: " & InputFilename, , mLogFile)
                pDR("UnitMeasure") = "Piece"
            End If

            ' BEGIN get SKU data from database
            'pDtStocks = mBusiness.LibraryStock.PriceTable(OrgID, pDR("ProductID"))
            'pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pDR("ActualQuantity")) &
            '    " AND PriceLevelID = " & ReadyString(pDR("PriceLevelID")), "", DataViewRowState.CurrentRows)
            'If pDvStocks.Count > 0 Then
            '    pDrvStock = pDvStocks(0)
            ' END get SKU data from database

            If CDec2(pDR("Quantity")) >= 0 Then
                'pDR("ActualQuantity") = pDrvStock("Quantity")
                'pDR("UnitMeasure") = pDrvStock("UnitMeasure")
                'pDR("UnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 2)
                ' check test
                '    If Math.Abs(pDR("UnitPrice") - pLine("UnitPrice")) > 0.01 Then
                '       Throw New Exception("Conflicting values in unit price vbQV7L2A8VEY")
                '   End If
                'pDR("OriginalUnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 2)
                pDR("Cost") = mBusiness.LibraryStock.GetLandedCost(pDR("ProductID"), OrgID) * pDR("ActualQuantity")
                'If ReadyGuid(pDrvStock("PriceLevelID")) <> pRetailID Then
                '    'todo: add security
                '    'If WebSession.Instance.UserAccess.PointOfSales_PriceLevelMode.IsEnable _
                '    '    OrElse WebSession.Instance.UserAccess.PointOfSales_UnlimitedUserMode.IsEnable Then

                '    'Else
                '    '    Common.WebRequest.ProData.RollbackTransaction()
                '    '    Return JsonDictionary.Serialize("Error", "You are not authorized to set price level other than retail")
                '    'End If
                'End If
                'pDR("PriceLevelID") = CStr2(pDrvStock("PriceLevelID"))
                pDR("Dimension") = ""
                pDR("RequestedUnitPrice") = pDR("RequestedUnitPrice")
                pDR("IsReplacement") = False
                pDR("IsHasReplacement") = False
                pDR("IsHasBackOrder") = False
                pDR("RefStockID") = ""

                Dim pUnitGp As Decimal
                Dim pUnitGpm As Double
                Dim pLineGp As Decimal
                Dim pLineGpm As Decimal
                Dim pLandedCost As Decimal

                'additional by janine sept 6 
                If (CDec2(pDR("Quantity")) > 0) Then
                    mBusiness.LibraryStock.GetProfitability(
                    vStockID:=CStr2(pDR("ProductID")),
                    vQty:=pDR("Quantity"),
                    vUnitQty:=pDR("ActualQuantity"),
                    vUnitPrice:=pDR("UnitPrice"),
                    oUnitGP:=pUnitGp,
                    oUnitGPM:=pUnitGpm,
                    oLineGP:=pLineGp,
                    oLineGPM:=pLineGpm,
                    oLandedCost:=pLandedCost,
                    vHistoricCost:=0,
                    vOrgID:=OrgID)

                Else
                    mBusiness.LibraryStock.GetProfitability(
                    vStockID:=CStr2(pDR("ProductID")),
                    vQty:=pDR("OrderedQuantity"),
                    vUnitQty:=pDR("ActualQuantity"),
                    vUnitPrice:=pDR("UnitPrice"),
                    oUnitGP:=pUnitGp,
                    oUnitGPM:=pUnitGpm,
                    oLineGP:=pLineGp,
                    oLineGPM:=pLineGpm,
                    oLandedCost:=pLandedCost,
                    vHistoricCost:=0,
                    vOrgID:=OrgID)
                End If
                '''''''''''''''''''''''''''''''''''

                pDR("UnitGP") = pUnitGp
                pDR("UnitGPM") = pUnitGpm
                pDR("LineGP") = pLineGp
                pDR("LineGPM") = pLineGpm
                pDR("LineCost") = pLandedCost * CDec2(pDR("Quantity")) * CDec2(pDR("ActualQuantity"))

                'additional by janine sept 6 
                If (CDec2(pDR("Quantity")) > 0) Then
                    pDR("Subtotal") = pDR("Quantity") * pDR("UnitPrice")
                Else
                    pDR("Subtotal") = pDR("OrderedQuantity") * pDR("UnitPrice")
                End If
                '''''''''''''''''
                'pDR("Subtotal") = pDR("Quantity") * pDR("UnitPrice") 'commented by janine

                pDR("TaxPercent") = 12 ' todo

                'addtional by janine feb 4 2015
                'if SCD (vat exempt, tax sales and vat amount must be 0)
                If (CDec2(pDR("VATExemptSale")) = 0 And CDec2(pDR("ZeroRatedSale")) = 0 And CDec2(pDR("NonVATSale")) = 0) Then
                    pDR("TaxSales") = RoundDown(pDR("SubTotal") / (1 + pDR("TaxPercent") / 100), 4)
                    pDR("TaxAmount") = pDR("SubTotal") - pDR("TaxSales")
                End If

            Else
                oError = "No matching unit quantity and price level"
                Return Nothing
            End If

            ' BEGIN get SKU data from database
            'End If
            ' END get SKU data from database

            pDtOut.Rows.Add(pDR)
        Loop
        pDtOut.Columns("ProductID").ColumnName = "StockID"

        Return pDtOut

    End Function

    Private Function ImportInvoiceItemSalesReps(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String, vDtProducts As DataTable) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pDvProducts As DataView

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceItemSalesReps")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemSalesReps")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("PeopleID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceItemID", False, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceItemSalesReps" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)


        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pDvProducts = New DataView(vDtProducts, "ID = " & ReadyString(pLine("InvoiceItemID")) & " AND InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
            If pDvProducts.Count = 0 Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceItemSalesReps"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "vbSSJ2G3TNW9! An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex) & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function

    Private Function ImportInvoiceItemRebates(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        vFowstID As String, vUserID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pDtInvoiceItems As DataTable
        Dim pDvModifers As DataView

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceItemsRebates")

        pSql = "DELETE FROM TransInvoiceItemsRebates" & vbCrLf &
            " WHERE TransInvoiceItemsRebates.ModifierGroupID IN" & vbCrLf &
            " (SELECT TransInvoiceItems.ModifierGroupID" & vbCrLf &
            "   FROM TransInvoiceItems" & vbCrLf &
            "   WHERE InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            ")"
        pSql = mBusiness.ProData.ExecuteSQL(pSql) ' the output is for debugging only

        pSql = "SELECT ModifierGroupID" & vbCrLf &
            ",SUM(Quantity * ActualQuantity) AS Quantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND ModifierGroupID IS NOT NULL" & vbCrLf &
            " GROUP BY ModifierGroupID"
        pDtInvoiceItems = mBusiness.ProData.Execute(pSql)
        If pDtInvoiceItems.Rows.Count = 0 Then
            Return True
        End If

        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemsRebates")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("RebateID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("Amount", True, SqlDbType.Money)
            .Add("UnitMeasure", True, SqlDbType.VarChar)
            .Add("UnitQuantity", True, SqlDbType.Decimal)
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("ModifierGroupID", True, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            pDvModifers = New DataView(pDtInvoiceItems, "ModifierGroupID = " & ReadyString(pLine("ModifierGroupID")), "", DataViewRowState.CurrentRows)
            If pDvModifers.Count = 0 Then
                Continue Do
            End If

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceItemsRebates"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                mBusiness.TransInvoice.AddItemToInventoryLedger( _
                    vProductID:=CStr2(pLine("ProductID")),
                    vOrgID:=OrgID,
                    vFowstID:=vFowstID,
                    vTransactionID:=vInvoiceID,
                    vUserID:=vUserID,
                    vQty:=CDec2(pLine("UnitQuantity")) * CDec2(pLine("Quantity")) * CDec2(pDvModifers(0)("Quantity")),
                    vSubTransactionType:="Return"
                )
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "vbE8XZYRYXTZ! An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex) & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function

    Private Function ImportInvoiceItemSerials(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceItemSerials")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemSerials")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("SerialNumber", True, SqlDbType.VarChar)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceItemSerials" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceItemSerials"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function

    Private Function ImportInvoiceDiscount(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceDiscount")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceDiscount")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("DiscountPercentage", True, SqlDbType.Float)
            .Add("DiscountTypeID", True, SqlDbType.UniqueIdentifier)
            .Add("DiscountRemarks", False, SqlDbType.VarChar)
            .Add("NoOfPeople", False, SqlDbType.Int) ''additional by janine july 26
            .Add("NoOfSeniorCitizens", False, SqlDbType.Int) ''additional by janine july 26
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceDiscount" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceDiscount"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function

    ''additional by janine july 26
    Private Function ImportInvoiceSeniorCitizenPWDCustomer(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceSeniorCitizenPWDCustomer")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceSeniorCitizenPWDCustomer")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerID", False, SqlDbType.UniqueIdentifier) ' should be true
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceSeniorCitizenPWDCustomer" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            If pLine.ContainsKey("CustomerID") _
                AndAlso IsGuid(pLine("CustomerID")) Then

                ' just do nothing here
            Else
                pLine("CustomerID") = mBusiness.SystemDefaults.CustomerOrganizationID
            End If
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceSeniorCitizenPWDCustomer"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function
    'end of additional

    ''additional by janine jan 19 2015
    Private Function ImportInvoiceCharges(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceCharges")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceCharges")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", False, SqlDbType.UniqueIdentifier)
            .Add("ChargeType", False, SqlDbType.VarChar)
            .Add("ChargeAmount", False, SqlDbType.Money)
            .Add("TipAmount", False, SqlDbType.Money)
            .Add("TaxAmount", False, SqlDbType.Money)
            .Add("Remarks", False, SqlDbType.VarChar)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceCharges" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceCharges"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function
    'end of additional

    Private Function CreateCustomerPaymentsDataTable(ByVal vInputZipFilename As String, _
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        DataImportLog.AddLog("Import from TransCustomerPayment")
        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPayment")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("UserID", True, SqlDbType.UniqueIdentifier)
            .Add("CheckPaymentAmount", True, SqlDbType.Money)
            .Add("AmountChange", True, SqlDbType.Money)
            .Add("CashAmount", True, SqlDbType.Money)
            .Add("SRVAmount", True, SqlDbType.Money)
            .Add("OtherTypeAmount", False, SqlDbType.Money)
            .Add("ChargeAmount", False, SqlDbType.Money) 'new
            .Add("CreditCardAmount", True, SqlDbType.Money)
            .Add("DiscountAmount", True, SqlDbType.Money)
            .Add("DebitCardAmount", True, SqlDbType.Money)
            .Add("TotalAmountDue", True, SqlDbType.Money)
            .Add("CustomerChangeCash", True, SqlDbType.Money)
            .Add("ReceiptType", True, SqlDbType.VarChar)
            .Add("ReceiptNumber", True, SqlDbType.VarChar)
            .Add("PaymentDate", True, SqlDbType.DateTime)
        End With
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            If pLine.ContainsKey("CustomerID") _
                AndAlso IsGuid(pLine("CustomerID")) Then

                ' just do nothing here
            Else
                pLine("CustomerID") = mBusiness.SystemDefaults.CustomerOrganizationID
            End If
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If

            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPayments(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder, _
        ByRef rDtOut As DataTable,
        ByRef rDtOutInvoices As DataTable,
        ByRef oError As String,
        ByVal vInvoiceID As String,
        ByVal vDtCustomerPaymentInvoices As DataTable,
        ByVal vBinderCustomerPaymentInvoices As DictionaryBinder,
        ByVal vDtCustomerPaymentDebitCards As DataTable,
        ByVal vBinderCustomerPaymentDebitCards As DictionaryBinder,
        ByVal vDtCustomerPaymentCreditCards As DataTable,
        ByVal vBinderCustomerPaymentCreditCards As DictionaryBinder,
        ByVal vDtCustomerPaymentChecks As DataTable,
        ByVal vBinderCustomerPaymentChecks As DictionaryBinder,
        ByVal vDtCustomerPaymentOther As DataTable,
        ByVal vBinderCustomerPaymentOther As DictionaryBinder) As Boolean

        Dim pDvCustomerPayments As DataView
        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCustomerPaymentInvoices As DataView
        Dim pDtCustomerPaymentsUnique As DataTable
        Dim pDtFieldValues As DataTable
        Dim pIds As String

        DataImportLog.AddLog("Import from TransCustomerPayment")
        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If

        pDvCustomerPaymentInvoices = New DataView(vDtCustomerPaymentInvoices, "InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
        If pDvCustomerPaymentInvoices.Count = 0 Then
            ' this invoice has no payment
            ' valid during on order taker
            Return True
        End If
        pDtCustomerPaymentsUnique = pDvCustomerPaymentInvoices.ToTable(True, {"CustomerPaymentID"})
        pDtFieldValues = ExtractFieldValues(pDtCustomerPaymentsUnique, "CustomerPaymentID", Integer.MaxValue)
        If pDtFieldValues.Rows.Count = 0 Then
            ' no matching customer payment ID has been found.
            ' todo: trap this data error
            Stop2()
            Return True
        End If
        pIds = pDtFieldValues.Rows(0)(0)

        pDvCustomerPayments = New DataView(vDT, "ID IN (" & pIds & ")", "", DataViewRowState.CurrentRows)
        For Each pDrvIn As DataRowView In pDvCustomerPayments
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Continue For
            End If
            pDvCustomerPaymentInvoices = New DataView(vDtCustomerPaymentInvoices, "CustomerPaymentID = " & ReadyString(pDrvIn("ID")) & " AND InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
            If pDvCustomerPaymentInvoices.Count = 0 Then
                ' you should panic here
                Stop2()
            End If

            pIU.TableName = "TransCustomerPayment"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try

            If ImportCustomerPaymentInvoices(
                vDT:=pDvCustomerPaymentInvoices.ToTable(),
                vBinder:=vBinderCustomerPaymentInvoices,
                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
                oError:=oError,
                rDtOut:=rDtOutInvoices) = False Then

                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If

            If ImportCustomerPaymentDebitCard(
                vDT:=vDtCustomerPaymentDebitCards,
                vBinder:=vBinderCustomerPaymentDebitCards,
                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
                oError:=oError,
                rDtOut:=rDtOutInvoices) = False Then

                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If

            If ImportCustomerPaymentCreditCard(
                vDT:=vDtCustomerPaymentCreditCards,
                vBinder:=vBinderCustomerPaymentCreditCards,
                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
                oError:=oError,
                rDtOut:=rDtOutInvoices) = False Then

                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If

            If ImportCustomerPaymentChecks(
               vDT:=vDtCustomerPaymentChecks,
               vBinder:=vBinderCustomerPaymentChecks,
               vCustomerPaymentID:=CStr2(pDrvIn("ID")),
               oError:=oError,
               rDtOut:=rDtOutInvoices) = False Then

                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If
            If ImportCustomerPaymentOther(
               vDT:=vDtCustomerPaymentOther,
               vBinder:=vBinderCustomerPaymentOther,
               vCustomerPaymentID:=CStr2(pDrvIn("ID")),
               oError:=oError,
               rDtOut:=rDtOutInvoices) = False Then

                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If
            'If ImportCustomerPaymentCreditCard(vInputZipFilename, pLine("ID"), oError) = False Then
            '    DataImportLog.AddLog(oError)
            '    DataImportLog.Save(DataImportLog.eResultType.Fail)
            '    Return False
            'End If
            'If ImportCustomerPaymentDebitCard(vInputZipFilename, pLine("ID"), oError) = False Then
            '    DataImportLog.AddLog(oError)
            '    DataImportLog.Save(DataImportLog.eResultType.Fail)
            '    Return False
            'End If
        Next pDrvIn

        Return True

    End Function

    Private Shared Function CreateCustomerPaymentInvoicesDataTable(ByVal vInputZipFilename As String, _
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentInvoices")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("Balance", True, SqlDbType.Money)
            .Add("DiscountAmount", True, SqlDbType.Money)
            .Add("InvoiceAmount", True, SqlDbType.Money)
            .Add("AmountPaid", True, SqlDbType.Money)
        End With
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentInvoices(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCustomerPaymentInvoices As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If

        pDvCustomerPaymentInvoices = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        ' ToOptimize: delete only when needed
        pSql = "DELETE FROM TransCustomerPaymentInvoices" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvCustomerPaymentInvoices
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentInvoices"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = ReadyString(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    Private Shared Function CreateCustomerPaymentDebitCardDataTable(ByVal vInputZipFilename As String, _
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentDebitCard")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("DebitCardType", False, SqlDbType.VarChar)
            .Add("DebitCardBankID", False, SqlDbType.UniqueIdentifier)
            .Add("DebitCardBankName", False, SqlDbType.VarChar)
            .Add("IsBankFromOrg", True, SqlDbType.Bit)
            .Add("BankCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("HolderLastName", False, SqlDbType.VarChar)
            .Add("HolderFirstName", False, SqlDbType.VarChar)
            .Add("HolderMiddleName", False, SqlDbType.VarChar)
            .Add("HolderGenerationalSuffix", False, SqlDbType.VarChar)
            .Add("DebitCardNumber", True, SqlDbType.VarChar)
            .Add("DebitCardIssuanceDate", False, SqlDbType.Date)
            .Add("DebitCardExpirationDate", False, SqlDbType.DateTime)
            .Add("ApprovalNo", False, SqlDbType.VarChar)
            .Add("NoOfInstallmentPayments", False, SqlDbType.Int)
            .Add("DebitCardSurcharge", False, SqlDbType.Float)
            .Add("DebitCardAmount", False, SqlDbType.Money)
        End With
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentDebitCard(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvDebitCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvDebitCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        pSql = "DELETE FROM TransCustomerPaymentDebitCard" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvDebitCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentDebitCard"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    Private Shared Function CreateCustomerPaymentCreditCardDataTable(ByVal vInputZipFilename As String, _
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentCreditCard")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("CreditCardType", False, SqlDbType.VarChar)
            .Add("CreditCardBankID", False, SqlDbType.UniqueIdentifier)
            .Add("CreditCardBankName", False, SqlDbType.VarChar)
            .Add("IsBankFromOrg", True, SqlDbType.Bit)
            .Add("BankCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("HolderLastName", False, SqlDbType.VarChar)
            .Add("HolderFirstName", False, SqlDbType.VarChar)
            .Add("HolderMiddleName", False, SqlDbType.VarChar)
            .Add("HolderGenerationalSuffix", False, SqlDbType.VarChar)
            .Add("CreditCardNumber", True, SqlDbType.VarChar)
            .Add("CreditCardIssuanceDate", False, SqlDbType.Date)
            .Add("CreditCardExpirationDate", False, SqlDbType.DateTime)
            .Add("ApprovalNo", False, SqlDbType.VarChar)
            .Add("NoOfInstallmentPayments", False, SqlDbType.Int)
            .Add("CreditCardSurcharge", False, SqlDbType.Float)
            .Add("CreditCardAmount", False, SqlDbType.Money)
        End With
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentCreditCard(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCreditCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvCreditCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        If False Then
            Throw New Exception("Test exception vbAKKJTNVUVM")
        End If

        pSql = "DELETE FROM TransCustomerPaymentCreditCard" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvCreditCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentCreditCard"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If False Then
                    Throw New Exception("Test exception vb9F37BASVPU")
                End If
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    ' additional by janine
    Private Shared Function CreateCustomerPaymentOtherDataTable(ByVal vInputZipFilename As String, _
       ByRef oBinder As DictionaryBinder,
       ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentOther")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", False, SqlDbType.UniqueIdentifier)
            .Add("PaymentType", False, SqlDbType.VarChar)
            .Add("Amount", False, SqlDbType.Money)
            .Add("IssuingOrganization", False, SqlDbType.VarChar)
            .Add("CardNumber", False, SqlDbType.VarChar)
            .Add("CardHolderName", False, SqlDbType.VarChar)
            .Add("CardIssuanceDate", False, SqlDbType.Date)
            .Add("CardExpirationDate", False, SqlDbType.Date)
            .Add("ApprovalNo", False, SqlDbType.VarChar)
        End With
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentOther(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvDebitCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvDebitCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        pSql = "DELETE FROM TransCustomerPaymentOther" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvDebitCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentOther"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function
    Private Shared Function CreateCustomerPaymentCheckDataTable(ByVal vInputZipFilename As String, _
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentChecks")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("BankID", False, SqlDbType.UniqueIdentifier)
            .Add("BankName", False, SqlDbType.VarChar)
            .Add("BankBranchID", False, SqlDbType.UniqueIdentifier)
            .Add("BankBranchName", False, SqlDbType.VarChar)
            .Add("IsBankFromOrg", False, SqlDbType.Bit)
            .Add("AccountName", False, SqlDbType.VarChar)
            .Add("AccountNumber", False, SqlDbType.VarChar)
            .Add("CheckNumber", False, SqlDbType.VarChar)
            .Add("CheckDate", False, SqlDbType.DateTime)
            .Add("CheckAmount", False, SqlDbType.Money)
            .Add("IsDeposit", False, SqlDbType.Bit)
            .Add("ReplacementMemoID", False, SqlDbType.UniqueIdentifier)
            .Add("IsHold", False, SqlDbType.Bit)
            .Add("HoldUntil", False, SqlDbType.DateTime)
            .Add("HoldReason", False, SqlDbType.VarChar)
            .Add("IsReturned", False, SqlDbType.Bit)
            .Add("ReturnedReason", False, SqlDbType.VarChar)
            .Add("DepositDate", False, SqlDbType.DateTime)
            .Add("IsGiftCertificate", False, SqlDbType.Bit)
        End With
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function
    Private Function ImportCustomerPaymentChecks(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCreditCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvCreditCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        If False Then
            Throw New Exception("Test exception vbAKKJTNVUCV")
        End If

        pSql = "DELETE FROM TransCustomerPaymentChecks" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvCreditCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentChecks"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If False Then
                    Throw New Exception("Test exception vb9F37BASVZX")
                End If
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function
    Private Function ImportLibraryOrganization(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String

        DataImportLog.AddLog("Import from LibraryOrganization")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganization")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("IsOrganization", False, SqlDbType.Bit)
            .Add("PersonTitleID", False, SqlDbType.UniqueIdentifier)
            .Add("PersonSurname", False, SqlDbType.VarChar)
            .Add("PersonFirstName", False, SqlDbType.VarChar)
            .Add("PersonMiddleName", False, SqlDbType.VarChar)
            .Add("PersonSuffixName", False, SqlDbType.VarChar)
            .Add("PersonDateOfBirth", False, SqlDbType.Date)
            .Add("PersonMotherMaidenSurname", False, SqlDbType.VarChar)
            .Add("PersonMotherMaidenFirstName", False, SqlDbType.VarChar)
            .Add("PersonMotherMaidenMiddleName", False, SqlDbType.VarChar)
            .Add("PersonSeniorCitizensIDNo", False, SqlDbType.VarChar)
            .Add("PersonPWDIDNo", False, SqlDbType.VarChar)
            .Add("PersonPassportNo", False, SqlDbType.VarChar)
            .Add("PersonDriversLicenseNo", False, SqlDbType.VarChar)
            .Add("PersonSSSNo", False, SqlDbType.VarChar)
            .Add("PersonGSISIDNo", False, SqlDbType.VarChar)
            .Add("PersonPRCIDNo", False, SqlDbType.VarChar)
            .Add("PersonPostalIDNo", False, SqlDbType.VarChar)
            .Add("PersonVotersIDNo", False, SqlDbType.VarChar)
            .Add("PersonOtherIDType", False, SqlDbType.VarChar)
            .Add("PersonOtherIDNo", False, SqlDbType.VarChar)
            .Add("CompanyName", False, SqlDbType.VarChar)
            .Add("CompanyNameInitial", False, SqlDbType.VarChar)
            .Add("OwnershipType", False, SqlDbType.VarChar)
            .Add("IsFranchisor", False, SqlDbType.Bit)
            .Add("RegistrationType", False, SqlDbType.VarChar)
            .Add("RegistrationNo", False, SqlDbType.VarChar)
            .Add("RegistrationDate", False, SqlDbType.Date)
            .Add("TinNumber", False, SqlDbType.VarChar)
            .Add("OfficeAddressBuildingNo", False, SqlDbType.VarChar)
            .Add("OfficeAddressStreet", False, SqlDbType.VarChar)
            .Add("OfficeAddressCityID", False, SqlDbType.UniqueIdentifier)
            .Add("OfficeAddressCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("BusinessAddressBuildingNo", False, SqlDbType.VarChar)
            .Add("BusinessAddressStreet", False, SqlDbType.VarChar)
            .Add("BusinessAddressCityID", False, SqlDbType.UniqueIdentifier)
            .Add("BusinessAddressCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("Purpose", False, SqlDbType.VarChar)
            .Add("BusinessScope", False, SqlDbType.VarChar)
            .Add("NoOfRetailOutlet", False, SqlDbType.Int)
            .Add("NoOfOffice", False, SqlDbType.Int)
            .Add("NoOfWarehouse", False, SqlDbType.Int)
            .Add("NoOfEmployees", False, SqlDbType.Int)
            .Add("IsHasHoldingCompany", False, SqlDbType.Bit)
            .Add("HoldingCompanyID", False, SqlDbType.UniqueIdentifier)
            .Add("IsABank", False, SqlDbType.Bit)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
            .Add("SalesAgentCode", False, SqlDbType.VarChar)
            .Add("IsFromDevice", False, SqlDbType.Bit)
            .Add("IsMobizFranchisorRequestSent", False, SqlDbType.Bit)
            .Add("IsMobizConfirmedFranchisor", False, SqlDbType.Bit)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "MobizLibraryOrganization"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)

                pSql2 = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                mBusiness.ProData.ExecuteSQL(pSql2)

            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            If ImportLibraryOrganizationCustomer(vInputZipFilename, pLine("ID"), oError) = False Then
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If
            If ImportLibraryOrganizationPriceLevel(vInputZipFilename, pLine("ID"), oError) = False Then
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If
        Loop
        rZipOut.AddTableCsv(pDtOut, "LibraryOrganization")

        Return True

    End Function

    Private Function ImportLibraryOrganizationCustomer(ByVal vInputZipFilename As String,
        ByVal vOrganizationID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationCustomer")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("OrganizationID", True, SqlDbType.UniqueIdentifier)
            .Add("ActivationDate", False, SqlDbType.DateTime)
            .Add("Deactivated", False, SqlDbType.Bit)
            .Add("DeactivationDate", False, SqlDbType.DateTime)
            .Add("DeactivationReason", False, SqlDbType.VarChar)
            .Add("Comments", False, SqlDbType.VarChar)
            .Add("IsBan", False, SqlDbType.Bit)
            .Add("LastUpdated", False, SqlDbType.DateTime)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM LibraryOrganizationCustomer" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrganizationID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("OrganizationID")) <> ReadyGuid(vOrganizationID) Then
                Continue Do
            End If

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryOrganizationCustomer"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                mBusiness.ProData.ExecuteSQL(pSql)

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
        Loop

        Return True

    End Function

    Private Function ImportLibraryOrganizationPriceLevel(ByVal vInputZipFilename As String, ByVal vOrganizationID As String,
    ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationPriceLevel")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("OrganizationID", False, SqlDbType.UniqueIdentifier)
            .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM LibraryOrganizationPriceLevel" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrganizationID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("OrganizationID")) <> ReadyGuid(vOrganizationID) Then
                Continue Do
            End If

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryOrganizationPriceLevel"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
        Loop

        Return True

    End Function

    Private Function ImportLibraryOrganizationFOWST(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
                ByVal vOrgID As String, ByVal vUuid As String,
                ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        DataImportLog.AddLog("Import from LibraryOrganizationFOWST")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationFOWST")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("OrganizationID", False, SqlDbType.UniqueIdentifier)
            .Add("FOWSTTypeID", False, SqlDbType.UniqueIdentifier)
            .Add("Code", False, SqlDbType.VarChar)
            .Add("Name", False, SqlDbType.VarChar)
            .Add("Description", False, SqlDbType.VarChar)
            .Add("BusinessTradeNameID", False, SqlDbType.UniqueIdentifier)
            .Add("StreetNumber", False, SqlDbType.VarChar)
            .Add("Street", False, SqlDbType.VarChar)
            .Add("FowstCityID", False, SqlDbType.UniqueIdentifier)
            .Add("FowstCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("SubFOWSTTypeID", False, SqlDbType.UniqueIdentifier)
            .Add("PlateNo", False, SqlDbType.VarChar)
            .Add("NoOfWheels", False, SqlDbType.VarChar)
            .Add("BodyColor", False, SqlDbType.VarChar)
            .Add("Weight", False, SqlDbType.Float)
            .Add("Dimension", False, SqlDbType.VarChar)
            .Add("OwnershipType", False, SqlDbType.Bit)
            .Add("ContractStart", False, SqlDbType.Date)
            .Add("ContractEnd", False, SqlDbType.Date)
            .Add("CostPerMonth", False, SqlDbType.Decimal)
            .Add("IsMortgaged", False, SqlDbType.Bit)
            .Add("IsFranchisee", False, SqlDbType.Bit)
            .Add("FranchisorID", False, SqlDbType.UniqueIdentifier)
            .Add("IsFOWSTInsideAMall", False, SqlDbType.Bit)
            .Add("FOWSTMallName", False, SqlDbType.VarChar)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
            .Add("FulfillmentCenter", False, SqlDbType.UniqueIdentifier)
            .Add("FOWSTNo", False, SqlDbType.Int)
            .Add("AccountID", False, SqlDbType.VarChar)
            .Add("StoreID", False, SqlDbType.VarChar)
            .Add("IsIncludeSpecialReport", False, SqlDbType.Bit)
            .Add("ShopperTrakAccountID", False, SqlDbType.UniqueIdentifier)
            .Add("IsIncludeToShopperTrakReport", False, SqlDbType.Bit)
            .Add("IsFromDevice", False, SqlDbType.Bit)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "MobizLibraryOrganizationFOWST"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try

        Loop
        rZipOut.AddTableCsv(pDtOut, "LibraryOrganizationFOWST")

        Return True

    End Function
    ' end of additional 

    Private Function ImportTransInventoryProductCount(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pBinder2 As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        Dim pDtItems As DataTable
        Dim Total As Double
        Dim TotalBadCount As Double
        Dim TotalBadCountReason As String

        pDtItems = CreateTableForInventoryCountItems(vInputZipFilename, oError)

        DataImportLog.AddLog("Import from TransInventoryProductCount")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInventoryProductCount")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", False, SqlDbType.UniqueIdentifier)
            .Add("ProductID", False, SqlDbType.UniqueIdentifier)
            .Add("InventoryCountDate", False, SqlDbType.VarChar)
            .Add("UserID", False, SqlDbType.VarChar)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pBinder2 = New DictionaryBinder
        With pBinder2.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InventoryProductCountID", True, SqlDbType.UniqueIdentifier)
            .Add("BinID", True, SqlDbType.UniqueIdentifier)
            .Add("IsGoodStock", True, SqlDbType.Bit)
            .Add("ExpirationDate", True, SqlDbType.VarChar)
            .Add("UnitMeasureID", False, SqlDbType.UniqueIdentifier)
            .Add("Quantity", False, SqlDbType.Int)
            .Add("ActualQuantity", False, SqlDbType.Int)
            .Add("BadCountReason", True, SqlDbType.VarChar)
        End With

        Dim dtAdjustmentItems As Date
        Dim IsHasPossibleReconstruction As Boolean

        Do
            IsHasPossibleReconstruction = False
            'mBusiness.ProData.BeginTransaction()
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If

            If pDtItems Is Nothing Then
                AppendToLog("Cannot find transaction details in ID " & pLine("ID"))
                Continue Do
            End If

            pSql = "Select top 1 LastUpdated from TransInventoryLedger " & vbCrLf &
                    "where " & vbCrLf &
                    "ProductID =  " & ReadyString(pLine("ProductID")) & vbCrLf &
                    "AND FowstID =  " & ReadyString(pLine("FowstID")) & vbCrLf &
                    "AND Lastupdated > " & ReadyString(pLine("InventoryCountDate"))

            dtAdjustmentItems = CDate(mBusiness.ProData.ExecuteScalar(pSql))

            'Check for adjustment
            If (DateDiff(DateInterval.Second, CDate(pLine("InventoryCountDate")), dtAdjustmentItems) > 0) Then
                IsHasPossibleReconstruction = True
            End If

            pIU.TableName = "TransInventoryProductCount"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)

                    'TransInventoryProductCount details
                    pDtItems.DefaultView.RowFilter = " InventoryProductCountID = " & ReadyString(pLine("ID"))



                    Dim BinID As String
                    Dim ExpirationDate As String
                    Dim IsGoodCount As Boolean

                    'Tota is for Offtake computation
                    Total = 0
                    ExpirationDate = "N/A"
                    BinID = ""

                    If pDtItems.DefaultView.ToTable.Rows.Count > 0 Then
                        For i As Integer = 0 To pDtItems.DefaultView.ToTable.Rows.Count - 1

                            Dim p As New InsertUpdate("TransInventoryProductCountStock")

                            Dim pTotal As Integer = 0
                            TotalBadCount = 0
                            TotalBadCountReason = ""

                            p("ID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ID"))
                            p("InventoryProductCountID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("InventoryProductCountID"))
                            p("BinID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("BinID"))
                            BinID = pDtItems.DefaultView.ToTable.Rows(i).Item("BinID")
                            p("IsGoodStock") = CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("IsGoodStock"))

                            If Not pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate") = "N/A" Then
                                p("ExpirationDate") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate"))
                                ExpirationDate = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate"))
                            End If

                            p("UnitMeasureID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("UnitMeasureID"))
                            p("Quantity") = CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity"))
                            p("ActualQuantity") = CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))

                            p("Variance") = CInt2(pDtItems.DefaultView.ToTable.Rows(i).Item("Variance"))
                            p("ExpectedValue") = CInt2(pDtItems.DefaultView.ToTable.Rows(i).Item("ExpectedValue"))
                            p("Remarks") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("Remarks"))

                            If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")) = vbNullString Then
                                p("TurnOverToID") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")))
                            End If

                            If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")) = vbNullString Then
                                p("TurnOverDate") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")))
                            End If

                            If IsInventoryCountAutomaticEffect() Then
                                p("IsActionTaken") = 1
                            End If

                            pSql = p.GetInsert()

                            If (CBool(pDtItems.DefaultView.ToTable.Rows(i).Item("IsGoodStock"))) Then
                                Total += CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity")) '* CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))
                                pTotal = CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity")) '* CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))
                            Else
                                TotalBadCount += CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity")) * CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))
                                TotalBadCountReason = CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("BadCountReason"))
                            End If

                            mBusiness.ProData.ExecuteSQL(pSql)

                            If IsInventoryCountAutomaticEffect() Then
                                AddToInventoryLedger(pLine("ID"), BinID, Total, ExpirationDate, pLine("ProductID"), pLine("FowstID"), pLine("UserID"), TotalBadCount, TotalBadCountReason, IsHasPossibleReconstruction, CStr2(pLine("InventoryCountDate")))
                            Else
                                Dim TransactionBalanceBeforeCount As Integer

                                pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                                "where LastUpdated < " & ReadyString(CDate(pLine("InventoryCountDate"))) & vbCrLf &
                                "AND TransInventoryLedger.FowstID = " & ReadyString(pLine("FowstID")) & vbCrLf &
                                "AND TransInventoryLedger.BinID = " & ReadyString(BinID) & vbCrLf &
                                "AND TransInventoryLedger.ProductID = " & ReadyString(pLine("ProductID")) & vbCrLf &
                                " ORDER BY LastUpdated desc"

                                TransactionBalanceBeforeCount = CDec2(mBusiness.ProData.ExecuteScalar(pSql))

                                pSql = "Update TransInventoryProductCountStock set Variance = " & TransactionBalanceBeforeCount - Total & vbCrLf &
                                        " WHERE ID = " & ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ID")) & vbCrLf &
                                        " AND BinID = " & ReadyString(BinID)

                                mBusiness.ProData.ExecuteSQL(pSql)

                            End If

                        Next

                        AddOrSetNewDefaultBin(pLine("ProductID"), pLine("FowstID"), pDtItems.DefaultView.ToTable.Rows(0).Item("BinID"), pLine("UserID"))

                        ComputeOffTake(pLine("ID"), pLine("FowstID"), pDtItems.DefaultView.ToTable.Rows(0).Item("BinID"), pLine("ProductID"), Total, pLine("InventoryCountDate"))

                    End If
                Else

                    For i As Integer = 0 To pDtItems.DefaultView.ToTable.Rows.Count - 1
                        Dim p As New InsertUpdate("TransInventoryProductCountStock")

                        If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")) = vbNullString Then
                            p("TurnOverToID") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")))

                            If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")) = vbNullString Then
                                p("TurnOverDate") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")))
                            End If

                            pSql = p.GetUpdate("ID=" & ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("ID"))))

                            mBusiness.ProData.ExecuteSQL(pSql)
                        End If

                    Next

                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            'mBusiness.ProData.CommitTransaction()
        Loop

        rZipOut.AddTableCsv(pDtOut, "TransInventoryProductCount")

        Return True
    End Function

    Function ComputeOffTake(ByVal InventoryCountID As String, ByVal FOWSTID As String, ByVal BinID As String, ByVal ProductID As String, ByVal TotalCount As Double, ByVal CountDate As Date)
        Dim pSql As String
        Dim dtPreviousBalance As DataTable
        Dim OffTake As Double

        pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                "WHERE ProductID = " & ReadyString(ProductID) & vbCrLf &
                " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                " AND BINID = " & ReadyString(BinID) & vbCrLf &
                " order by LastUpdated desc"

        dtPreviousBalance = mBusiness.ProData.Execute(pSql)

        If dtPreviousBalance.Rows.Count > 0 Then
            OffTake = CDbl(dtPreviousBalance.Rows(0).Item("Balance")) - TotalCount
            Dim p As New InsertUpdate("TransOffTake")

            p("OfftakeDate") = ReadyString(CountDate)
            p("InventoryCountID") = ReadyString(InventoryCountID)
            p("ProductID") = ReadyString(ProductID)
            p("FowstID") = ReadyString(FOWSTID)
            p("BinID") = ReadyString(BinID)
            p("OffTake") = ReadyString(OffTake)

            pSql = p.GetInsert

            mBusiness.ProData.ExecuteSQL(pSql)
        End If
    End Function

    Function IsInventoryCountAutomaticEffect() As Boolean
        Return mBusiness.SystemDefaults.IsAutomaticEffect
    End Function

    Private Function ImportLibraryFOWSTBin(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
            ByVal vOrgID As String, ByVal vUuid As String,
            ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pLineNumber As Integer

        DataImportLog.AddLog("Import from LibraryFOWSTBin")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryFOWSTBin")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("Name", False, SqlDbType.VarChar)
            .Add("Description", False, SqlDbType.VarChar)
            .Add("IsActive", False, SqlDbType.Bit)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            'mBusiness.ProData.BeginTransaction()
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pLineNumber += 1
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryFOWSTBin"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf &
                    "CSV line number: " & pLineNumber & vbCrLf &
                    vbCrLf &
                    pIU.GetUpdate("for debugging only") & vbCrLf &
                    vbCrLf &
                    GetStackTrace(, ex))
                Return False
            End Try
            'mBusiness.ProData.CommitTransaction()
        Loop


        rZipOut.AddTableCsv(pDtOut, "LibraryFOWSTBin")

        Return True
    End Function

    Private Function ImportLibraryFOWSTBinProduct(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
            ByVal vOrgID As String, ByVal vUuid As String,
            ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pLineNumber As Integer
        Dim pID As String
        Dim pEnableWorkaround As Boolean

        'pEnableWorkaround = Now.Date = New Date(2015, 4, 13)
        pEnableWorkaround = Now.Date <= New Date(2015, 11, 3)
        DataImportLog.AddLog("Import from LibraryFowstBinProduct")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryFowstBinProduct")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("BinID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("MinPerProduct", False, SqlDbType.Float)
            .Add("MaxPerProduct", False, SqlDbType.Float)
            .Add("IsDefault", False, SqlDbType.Bit)
            .Add("IsActive", False, SqlDbType.Bit)
            .Add("LastUpdated", False, SqlDbType.DateTime)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pLineNumber += 1
            pBinder.Source = pLine

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                If pEnableWorkaround Then
                    pID = ""
                    If pLine.ContainsKey("ID") Then
                        pID = pLine("ID")
                        If IsGuid(pID) Then
                            pDrOut = pDtOut.NewRow
                            pDrOut("ID") = pLine("ID")
                            pDtOut.Rows.Add(pDrOut)
                        End If
                    End If
                End If ' If pEnableWorkaround
                Continue Do
            End If
            pIU.TableName = "LibraryFowstBinProduct"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf &
                    "CSV line number: " & pLineNumber & vbCrLf &
                    vbCrLf &
                    pIU.GetUpdate("for debugging only") & vbCrLf &
                    vbCrLf &
                    GetStackTrace(, ex))
                Return False
            End Try
        Loop

        rZipOut.AddTableCsv(pDtOut, "LibraryFOWSTBinProduct")

        Return True
    End Function

    Private Function CreateTableForInventoryCountItems(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable
        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim oBinder As DictionaryBinder
        Dim pIU As InsertUpdate ' used for verfication only

        DataImportLog.AddLog("Import from TransInventoryProductCountStock")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInventoryProductCountStock")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InventoryProductCountID", True, SqlDbType.UniqueIdentifier)
            .Add("BinID", True, SqlDbType.UniqueIdentifier)
            .Add("IsGoodStock", True, SqlDbType.Bit)
            .Add("ExpirationDate", True, SqlDbType.VarChar)
            .Add("UnitMeasureID", True, SqlDbType.UniqueIdentifier)
            .Add("Quantity", True, SqlDbType.Int)
            .Add("ActualQuantity", True, SqlDbType.Int)
            .Add("BadCountReason", False, SqlDbType.VarChar)
            .Add("TurnOverToID", False, SqlDbType.VarChar)
            .Add("TurnOverDate", False, SqlDbType.DateTime2)
            .Add("Variance", False, SqlDbType.Int)
            .Add("Remarks", False, SqlDbType.VarChar)
            .Add("ExpectedValue", False, SqlDbType.Int)
        End With

        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut
    End Function

    Function AddToInventoryLedger(ByVal TransactionID As String, ByVal BinID As String, ByVal Total As Double, ByVal ExpDate As String, ByVal ProductID As String, ByVal FOWSTID As String, ByVal PeopleID As String, ByVal BadCount As Integer, ByVal BadCountReason As String, Optional ByVal IsHasPossibleReconstruction As Boolean = False, Optional ByVal dtAdjustmentItems As String = "")
        Dim EDate As Date?

        If Not ExpDate = "N/A" Then
            EDate = CDate(ExpDate)
        End If

        Dim TransactionBalanceBeforeCount As String
        Dim pDataNeedsReconstruction As Boolean
        Dim pSql As String
        Dim dtTransactionAfterCount As DataTable

        If IsHasPossibleReconstruction Then

            Dim pInventoryCountDate As String

            pSql = "Select top 1 LastUpdated from TransInventoryLedger " & vbCrLf &
                    "where " & vbCrLf &
                    "ProductID =  " & ReadyString(ProductID) & vbCrLf &
                    "AND FowstID =  " & ReadyString(FOWSTID) & vbCrLf &
                    "AND BinID =  " & ReadyString(BinID) & vbCrLf &
                    "AND Lastupdated > " & ReadyString(dtAdjustmentItems)

            pInventoryCountDate = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

            If IsDate(pInventoryCountDate) Then
                If (DateDiff(DateInterval.Second, CDate(dtAdjustmentItems), CDate(pInventoryCountDate)) > 0) Then
                    pDataNeedsReconstruction = True

                    pSql = "Select * from TransInventoryLedger " & vbCrLf &
                            "where LastUpdated >= " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                            "AND TransInventoryLedger.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND TransInventoryLedger.BinID = " & ReadyString(BinID) & vbCrLf &
                            "AND TransInventoryLedger.ProductID = " & ReadyString(ProductID) & vbCrLf &
                            " ORDER BY LastUpdated asc"
                    dtTransactionAfterCount = mBusiness.ProData.Execute(pSql)

                    pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                            "where LastUpdated < " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                            "AND TransInventoryLedger.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND TransInventoryLedger.BinID = " & ReadyString(BinID) & vbCrLf &
                            "AND TransInventoryLedger.ProductID = " & ReadyString(ProductID) & vbCrLf &
                            " ORDER BY LastUpdated desc"

                    TransactionBalanceBeforeCount = mBusiness.ProData.ExecuteScalar(pSql)

                End If
            End If
        End If

        If pDataNeedsReconstruction Then
            mBusiness.TransInventoryLedger.Add( _
            vStockID:=CStr2(ProductID),
            vFowstID:=CStr2(FOWSTID),
            vTransactionID:=TransactionID,
            vTransactionType:=TransInventoryLedger.eTransType.InventoryCount,
            vUserID:=CStr2(PeopleID),
            vQty:=Total - TransactionBalanceBeforeCount,
            vBalance:=Total
        )
            If dtTransactionAfterCount.Rows.Count > 0 Then
                For i As Integer = 0 To dtTransactionAfterCount.Rows.Count - 1
                    Dim mTransType As TransInventoryLedger.eTransType

                    Select Case CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionType"))
                        Case "Invoice"
                            mTransType = TransInventoryLedger.eTransType.Invoice
                        Case "InventoryCount"
                            mTransType = TransInventoryLedger.eTransType.InventoryCount
                        Case "Receiving"
                            mTransType = TransInventoryLedger.eTransType.Receiving
                        Case "TransferOut"
                            mTransType = TransInventoryLedger.eTransType.TransferOut
                        Case "Sales Returns"
                            mTransType = TransInventoryLedger.eTransType.SalesReturn
                        Case "Product Maker"
                            mTransType = TransInventoryLedger.eTransType.SalesReturn
                    End Select

                    If mTransType <> TransInventoryLedger.eTransType.InventoryCount Then
                        mBusiness.TransInventoryLedger.Add( _
                        vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                        vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                        vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                        vTransactionType:=mTransType,
                        vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                        vQty:=CDec2(dtTransactionAfterCount.Rows(i).Item("Qty"))
                    )
                    Else
                        mBusiness.TransInventoryLedger.Add( _
                        vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                        vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                        vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                        vTransactionType:=mTransType,
                        vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                        vQty:=Nothing,
                        vBalance:=CDec2(dtTransactionAfterCount.Rows(i).Item("Balance"))
                    )
                    End If

                Next
            End If
        Else
            mBusiness.TransInventoryLedger.Add( _
            ProductID, _
            FOWSTID, _
            TransactionID, _
            TransInventoryLedger.eTransType.InventoryCount, _
            PeopleID, , _
            Total, _
            BinID,
            EDate,
            BadCount,
            BadCountReason
        )
        End If


    End Function

    Function AddOrSetNewDefaultBin(ByVal ProductID As String, ByVal FowstID As String, ByVal BinID As String, ByVal UserID As String)
        'this function should only be applied in inventory count
        Dim pSql As String
        Dim dt As New DataTable
        Dim pOut As String

        'for carry over
        pSql = "SELECT TOP 1 Balance" & vbCrLf &
                " FROM TransInventoryLedger " & vbCrLf &
                " INNER JOIN dbo.LibraryFowstBin ON " & vbCrLf &
                " dbo.LibraryFowstBin.ID = TransInventoryLedger.BinID " & vbCrLf &
                " WHERE ProductID = " & ReadyString(ProductID) & vbCrLf &
                " AND TransInventoryLedger.FowstID = " & ReadyString(FowstID) & vbCrLf &
                " AND dbo.LibraryFowstBin.Name = 'Unnamed Bin'"

        'If vExpirationDate IsNot Nothing Then
        '    pSql &= " AND ExpirationDate = " & ReadyString(vExpirationDate) & vbCrLf
        'Else
        '    pSql &= " AND ExpirationDate IS NULL" & vbCrLf
        'End If

        pSql &= " ORDER BY TransInventoryLedger.LastUpdated DESC"

        Dim Balance As Double

        Balance = CDec2(mBusiness.ProData.ExecuteScalar(pSql))

        'Query if the bin has fowstbinproduct record()
        pSql = "SELECT BinID" &
         " FROM LibraryFowstBinProduct" &
         " WHERE FowstID = " & ReadyString(FowstID) &
         " AND ProductID = " & ReadyString(ProductID) &
         " AND BinID = " & ReadyString(BinID)
        pOut = CStr2(mBusiness.ProData.ExecuteScalar(pSql))


        pSql = "SELECT dbo.LibraryFowstBin.ID " &
                "FROM dbo.LibraryFowstBin INNER JOIN " &
                "dbo.LibraryFowstBinProduct ON dbo.LibraryFowstBin.FowstID = dbo.LibraryFowstBinProduct.FowstID AND  " &
                "dbo.LibraryFowstBin.ID = dbo.LibraryFowstBinProduct.BinID " &
                "WHERE dbo.LibraryFowstBinProduct.ProductID = " & ReadyString(ProductID) &
                " AND dbo.LibraryFowstBinProduct.IsDefault = 1 " &
                " AND  dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID) &
                " AND dbo.LibraryFowstBin.Name = 'Unnamed Bin'"

        dt = mBusiness.ProData.Execute(pSql)

        If dt.Rows.Count > 0 Then
            'set default bin balance to 0
            Dim nID As String

            nID = Guid.NewGuid.ToString

            'AddToInventoryLedger(nID, dt.Rows(0).Item("ID").ToString, 0, "N/A", ProductID, FowstID, UserID, 0, "")

            mBusiness.TransInventoryLedger.Add( _
                                   ProductID, _
                                   FowstID, _
                                   nID, _
                                   TransInventoryLedger.eTransType.InventoryCountAdjustment, _
                                   CStr2(UserID),
                                   , 0, _
                                   , ,
                                   0,
                                   ""
                               )
            'mBusiness.TransInventoryLedger.Add( _
            '      vStockID:=CStr2(ProductID),
            '      vFowstID:=CStr2(FowstID),
            '      vTransactionID:=nID,
            '      vBinID:=dt.Rows(0).Item("ID").ToString,
            '      vQty:=0,
            '      vTransactionType:=TransInventoryLedger.eTransType.InventoryCountAdjustment,
            '      vUserID:=CStr2(UserID) _
            '  )

            'remove isdefault to "Default Bin"
            pSql = "Update LibraryFowstBinProduct set IsDefault = 0 where ProductID = " & ReadyString(ProductID) & " AND BinID = " & ReadyString(dt.Rows(0).Item("ID").ToString) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID)
            mBusiness.ProData.ExecuteSQL(pSql)

            'carry over

            'AddToInventoryLedger(nID, dt.Rows(0).Item("ID").ToString, Balance, "N/A", ProductID, FowstID, UserID, 0, "")

            mBusiness.TransInventoryLedger.Add( _
                   vStockID:=CStr2(ProductID),
                   vFowstID:=CStr2(FowstID),
                   vTransactionID:=nID,
                   vTransactionType:=TransInventoryLedger.eTransType.CarryOver,
                   vUserID:=CStr2(UserID),
                   vQty:=Balance,
                   vBinID:=BinID _
               )


            'add to libraryfowstbinproduct and set as default bin
            Dim pIU As InsertUpdate

            pIU = New InsertUpdate("LibraryFowstBinProduct")
            pIU("FowstID") = ReadyString(FowstID)
            pIU("BinID") = ReadyString(BinID)
            pIU("ProductID") = ReadyString(ProductID)
            pIU("IsDefault") = 1
            pSql = pIU.GetInsert
            mBusiness.ProData.ExecuteSQL(pSql)

            pOut = "Not Empty"
        End If

        'check if the current bin is already assigned to this product in LibraryFowstBinProduct

        If CStr2(pOut) = "" Then
            Dim pIU As InsertUpdate

            pIU = New InsertUpdate("LibraryFowstBinProduct")
            pIU("FowstID") = ReadyString(FowstID)
            pIU("BinID") = ReadyString(BinID)
            pIU("ProductID") = ReadyString(ProductID)
            pIU("IsDefault") = 0
            pSql = pIU.GetInsert
            mBusiness.ProData.ExecuteSQL(pSql)
        End If

    End Function

    Private Function ImportReceiving(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from TransReceiving")
            pCsvReader = New CsvReader(vInputZipFilename, "TransReceiving")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("ReceivingNo", True, SqlDbType.Int)
                .Add("IsReceiving", True, SqlDbType.Bit)
                .Add("ReceivingPlaceID", False, SqlDbType.UniqueIdentifier)
                .Add("SupplierID", False, SqlDbType.UniqueIdentifier)
                .Add("DocumentDate", True, SqlDbType.DateTime)
                .Add("DocumentType", False, SqlDbType.VarChar)
                .Add("DocumentNo", False, SqlDbType.VarChar)
                .Add("Is3PL", False, SqlDbType.Bit)
                .Add("IsResaleable", False, SqlDbType.Bit)
                .Add("ResaleableReason", False, SqlDbType.VarChar)
                .Add("UserID", True, SqlDbType.UniqueIdentifier)
                .Add("DeviceID", True, SqlDbType.UniqueIdentifier)
                .Add("TransportedBy", False, SqlDbType.VarChar)
                '.Add("TransferOutToReceivingID", False, SqlDbType.VarChar) ' should be UniqueIdentifier
                .Add("LastUpdated", False, SqlDbType.VarChar)
                .Add("HasIssues", False, SqlDbType.Bit)
                .Add("IssueRemarks", False, SqlDbType.VarChar)
                .Add("PONo", False, SqlDbType.VarChar)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Dim dtAdjustmentItems As Date
            Dim IsHasPossibleReconstruction As Boolean

            Do
                IsHasPossibleReconstruction = False
                mBusiness.ProData.RollbackTransaction(True, True)
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                If pBinder.Validate(oError) = False Then
                    Return False
                End If
                If pItemInitialized = False Then
                    pItemInitialized = True
                    pDtItems = CreateReceivingStocksDataTable(vInputZipFilename, oError)
                End If
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                If pBinder.Items.ContainsKey("SupplierID") = False Then
                    If ReadyGuid(OrgID) = ReadyGuid("9DA344F3-DADB-4675-B723-15F0C591DA3C") Then
                        pIU("SupplierID") = "'f1441fc7-fb7d-47fc-a62b-1548f066fc98'"
                    End If
                End If

                If pBinder.Items.ContainsKey("TransportedBy") = False Then
                    pIU("TransportedBy") = "'Unknown'"
                End If

                'If pLine.ContainsKey("TransferOutToReceivingID") Then
                '    If IsGuid(pLine("TransferOutToReceivingID")) = False Then
                '        ' issue with nike abreeza
                '        ' receiving id: 18DE9887-3D29-4347-9B9A-090CDCA4B8E3
                '        ' TransferOutToReceivingID = 19578262
                '        AppendToLog("vb8R95DHKEDB!" & vbCrLf &
                '            "zip filename: " & vInputZipFilename & vbCrLf &
                '            "receiving id: " & pLine("ID") & vbCrLf &
                '            "TransferOutToReceivingID: " & pLine("TransferOutToReceivingID") & vbCrLf, , "ImportReceiving")
                '        If ReadyGuid(pLine("ID")) = ReadyGuid("18DE9887-3D29-4347-9B9A-090CDCA4B8E3") Then
                '            AppendToDebug("vb9BFLE7HD3R! Tagging as imported. receiving id: 18DE9887-3D29-4347-9B9A-090CDCA4B8E3")
                '            pDrOut = pDtOut.NewRow
                '            pDrOut("ID") = pLine("ID")
                '            pDtOut.Rows.Add(pDrOut)
                '        End If
                '        Continue Do
                '    End If
                'Else
                '    pIU("TransferOutToReceivingID") = "null"
                'End If

                If (CBool2(pIU("IsReceiving")) = False) Then
                    pIU("HasIssues") = 1
                    pIU("IssueRemarks") = ReadyString("No stocks received in destination")
                End If

                pIU.TableName = "TransReceiving"

                dtAdjustmentItems = InventoryAdjustment(pLine("ReceivingPlaceID"))

                'Check for adjustment
                If (DateDiff(DateInterval.Second, CDate(pLine("LastUpdated")), dtAdjustmentItems) > 0) Then
                    IsHasPossibleReconstruction = True
                End If

                mBusiness.ProData.BeginTransaction()
                Try
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        Try
                            mBusiness.ProData.ExecuteSQL(pSql)
                        Catch ex As Exception
                            AppendToLog("vbKX97K6SVLY! " & ex.Message & vbCrLf & pIU.GetUpdate("insert statement"), , mLogFile)
                            Throw
                        End Try
                    End If

                    'Dim pDtItemsTemp As DataTable

                    'If pLine.ContainsKey("TransferOutToReceivingID") = True Then
                    '    pSql = "Update TransReceiving set TransferOutToReceivingID = " & ReadyString(pLine("ID")) & _
                    '        ", HasIssues = 0 " & _
                    '        ", IssueRemarks = '' " & _
                    '        " WHERE ID = " & ReadyString(pLine("TransferOutToReceivingID"))

                    '    mBusiness.ProData.ExecuteSQL(pSql)

                    '    pSql = "Select * from TransReceivingStock where ReceivingID = " & ReadyString(pLine("TransferOutToReceivingID"))
                    '    pDtItemsTemp = mBusiness.ProData.Execute(pSql)
                    'End If

                    pSql = "DELETE FROM TransReceivingStock WHERE ReceivingID = " & ReadyString(pLine("ID"))
                    mBusiness.ProData.ExecuteSQL(pSql)

                    ' BEGIN insert items
                    pDvItems = New DataView(pDtItems, "ReceivingID = " & ReadyString(pLine("ID")), "", DataViewRowState.CurrentRows)
                    For Each pDrvItem As DataRowView In pDvItems
                        pIU = New InsertUpdate("TransReceivingStock")
                        pIU("ID") = ReadyString(pDrvItem("ID"))
                        pIU("ReceivingID") = ReadyString(pDrvItem("ReceivingID"))
                        pIU("ProductID") = ReadyString(pDrvItem("ProductID"))
                        pIU("QuantityGood") = CDec2(pDrvItem("QuantityGood"))
                        pIU("ActualQuantityGood") = CDec2(pDrvItem("ActualQuantityGood"))
                        pIU("UnitMeasureGood") = ReadyString(pDrvItem("UnitMeasureGood"))
                        pIU("QuantityDamaged") = CDec2(pDrvItem("QuantityDamaged"))
                        pIU("ActualQuantityDamaged") = CDec2(pDrvItem("ActualQuantityDamaged"))
                        pIU("UnitMeasureDamaged") = ReadyString(pDrvItem("UnitMeasureDamaged"))
                        pIU("BadOrderReason") = ReadyString(pDrvItem("BadOrderReason"))
                        pIU("AdditionalInfo") = ReadyString(pDrvItem("AdditionalInfo"))
                        pIU("IsResaleable") = CDec2(pDrvItem("IsResaleable"))

                        pSql = pIU.GetInsert()
                        mBusiness.ProData.ExecuteSQL(pSql)

                        'If pDtItemsTemp IsNot Nothing AndAlso
                        '    pDtItemsTemp.Rows.Count > 0 Then

                        '    pDtItemsTemp.DefaultView.RowFilter = "ProductID = " & ReadyString(pDrvItem("ProductID"))

                        '    If pDtItemsTemp.DefaultView.ToTable.Rows.Count > 0 Then
                        '        If Not pDtItemsTemp.DefaultView.ToTable.Rows(0).Item("QuantityGood") = pDrvItem("QuantityGood") OrElse _
                        '            Not CStr2(pDtItemsTemp.DefaultView.ToTable.Rows(0).Item("UnitMeasureGood")) = CStr2(pDrvItem("UnitMeasureGood")) Then

                        '            'there's a problem with this sku
                        '            pSql = "Update TransReceiving set HasIssues = 1, " & _
                        '                "IssueRemarks = 'Some discrepancies found'" & _
                        '                " WHERE ID = " & ReadyString(pDrvItem("ReceivingID"))

                        '            mBusiness.ProData.ExecuteSQL(pSql)

                        '            pSql = "Update TransReceiving set HasIssues = 1, " & _
                        '                    "IssueRemarks = 'Some discrepancies found'" & _
                        '                    " WHERE ID = " & ReadyString(pLine("TransferOutToReceivingID"))

                        '            mBusiness.ProData.ExecuteSQL(pSql)

                        '        End If
                        '    Else
                        '        'received item with no stock transfer counterpart?
                        '        pSql = "Update TransReceiving set HasIssues = 1, " &
                        '                "IssueRemarks = 'Some discrepancies found'" & _
                        '                " WHERE ID = " & ReadyString(pDrvItem("ReceivingID"))

                        '        mBusiness.ProData.ExecuteSQL(pSql)

                        '        pSql = "Update TransReceiving set HasIssues = 1, " & _
                        '                "IssueRemarks = 'Some discrepancies found'" & _
                        '                " WHERE ID = " & ReadyString(pLine("TransferOutToReceivingID"))

                        '        mBusiness.ProData.ExecuteSQL(pSql)

                        '    End If
                        'Else
                        '    'received stock transfer counterpart?
                        '    pSql = "Update TransReceiving set HasIssues = 1, " &
                        '        "IssueRemarks = 'No stock transfer document from source'" & _
                        '        " WHERE ID = " & ReadyString(pDrvItem("ReceivingID"))

                        '    mBusiness.ProData.ExecuteSQL(pSql)
                        'End If
                    Next pDrvItem

                    ' END insert items
                    mBusiness.TransReceiving.AddToInventoryLedger(pLine("ID"), IsHasPossibleReconstruction)
                    mBusiness.TransReceiving.UpdateCounter(
                        vDeviceID:=pLine("DeviceID"),
                        vFowstID:=pLine("ReceivingPlaceID"),
                        vValue:=CDec2(pLine("ReceivingNo")),
                        vIsReceiving:=CBool2(pLine("IsReceiving")))
                    mBusiness.ProData.CommitTransaction()

                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)
                Catch ex As Exception
                    mBusiness.ProData.RollbackTransaction()
                    oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                    Return False
                End Try
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Loop

            If pDtOut.Rows.Count = 0 Then
                If CStr2(oError).Trim <> "" Then
                    Return False
                End If
            End If

            rZipOut.AddTableCsv(pDtOut, "TransReceiving")

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function CreateReceivingStocksDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)

        DataImportLog.AddLog("Import from TransReceivingStock")
        pCsvReader = New CsvReader(vInputZipFilename, "TransReceivingStock")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ReceivingID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("QuantityGood", True, SqlDbType.Decimal)
            .Add("ActualQuantityGood", True, SqlDbType.Decimal)
            .Add("UnitMeasureGood", True, SqlDbType.UniqueIdentifier)
            .Add("QuantityDamaged", True, SqlDbType.Decimal)
            .Add("ActualQuantityDamaged", True, SqlDbType.Decimal)
            .Add("UnitMeasureDamaged", True, SqlDbType.UniqueIdentifier)
            .Add("BadOrderReason", False, SqlDbType.VarChar)
            .Add("AdditionalInfo", False, SqlDbType.VarChar)
            .Add("IsResaleable", True, SqlDbType.Bit)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("ReceivingID")
            .Add("ProductID")
            .Add("QuantityGood", GetType(Decimal))
            .Add("ActualQuantityGood", GetType(Decimal))
            .Add("UnitMeasureGood")
            .Add("QuantityDamaged", GetType(Decimal))
            .Add("ActualQuantityDamaged", GetType(Decimal))
            .Add("UnitMeasureDamaged")
            .Add("BadOrderReason")
            .Add("AdditionalInfo")
            .Add("IsResaleable", GetType(Boolean))
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)
            pDtOut.Rows.Add(pDR)
        Loop

        Return pDtOut

    End Function

    Private Function ImportProductBarcode(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from LibraryPIMProductMeasurementBarcode")
            pCsvReader = New CsvReader(vInputZipFilename, "LibraryPIMProductMeasurementBarcode")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("StockMeasurementsID", True, SqlDbType.UniqueIdentifier)
                .Add("Barcode", True, SqlDbType.VarChar)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "MobizLibraryPIMProductMeasurementBarcode"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                    Return False
                End Try
            Loop

            rZipOut.AddTableCsv(pDtOut, "LibraryPIMProductMeasurementBarcode")

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportMaterialsRequisition(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from LibraryProjectMaterialsRequisition")
            pCsvReader = New CsvReader(vInputZipFilename, "LibraryProjectMaterialsRequisition")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("OrgID", True, SqlDbType.UniqueIdentifier)
                .Add("ProjectID", True, SqlDbType.UniqueIdentifier)
                .Add("ParentID", True, SqlDbType.VarChar)
                .Add("ProductID", False, SqlDbType.UniqueIdentifier)
                .Add("RequisitionID", False, SqlDbType.UniqueIdentifier)
                .Add("RequestedValue", False, SqlDbType.Float)
                .Add("ApprovedRequestedQty", False, SqlDbType.Float)
                .Add("VendorID", False, SqlDbType.UniqueIdentifier)
                .Add("LastUpdated", True, SqlDbType.DateTime2)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "LibraryProjectMaterialsRequisition"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                    Return False
                End Try
            Loop

            rZipOut.AddTableCsv(pDtOut, "LibraryProjectMaterialsRequisition")

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportCashHandling(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from TransCashHandling")
            pCsvReader = New CsvReader(vInputZipFilename, "TransCashHandling")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("FOWSTID", True, SqlDbType.UniqueIdentifier)
                .Add("DepositDate", True, SqlDbType.DateTime2)
                .Add("DepositedToPosition", False, SqlDbType.VarChar)
                .Add("DepositedTo", False, SqlDbType.UniqueIdentifier)
                .Add("DepositedToByName", False, SqlDbType.VarChar)
                .Add("DepositedBy", True, SqlDbType.UniqueIdentifier)
                .Add("DepositedByName", False, SqlDbType.VarChar)
                .Add("IsFromTotalAmount", False, SqlDbType.VarChar)
                .Add("TotalAmount", True, SqlDbType.Decimal)

                .Add("TotalCash", False, SqlDbType.Decimal)
                .Add("TotalCheck", False, SqlDbType.Decimal)
                .Add("TotalCreditCard", False, SqlDbType.Decimal)
                .Add("TotalDebitCard", False, SqlDbType.Decimal)
                .Add("TotalSRV", False, SqlDbType.Decimal)
                .Add("TotalPettyCash", False, SqlDbType.Decimal)

                .Add("NoOfFiveCents", False, SqlDbType.Int)
                .Add("NoOfTenCents", False, SqlDbType.Int)
                .Add("NoOfTwentyFiveCents", False, SqlDbType.Int)
                .Add("NoOfOnePeso", False, SqlDbType.Int)
                .Add("NoOfFivePesos", False, SqlDbType.Int)
                .Add("NoOfTenPesos", False, SqlDbType.Int)
                .Add("NoOfTwentyPesos", False, SqlDbType.Int)
                .Add("NoOfFiftyPesos", False, SqlDbType.Int)
                .Add("NoOfOneHundredPesos", False, SqlDbType.Int)
                .Add("NoOfTwoHundredPesos", False, SqlDbType.Int)
                .Add("NoOfFiveHundredPesos", False, SqlDbType.Int)
                .Add("NoOfOneThousandPesos", False, SqlDbType.Int)

                .Add("TotalAcceptedFromPrevTurnOver", False, SqlDbType.Decimal)
                .Add("TransactionDateAndTimeFrom", False, SqlDbType.DateTime2)
                .Add("TransactionDateAndTimeTo", False, SqlDbType.DateTime2)
                .Add("IsTurnOver", False, SqlDbType.Bit)

                .Add("ServiceCharge", False, SqlDbType.Decimal)
                .Add("BankBranch", False, SqlDbType.VarChar)
                .Add("DepositReferenceNo", False, SqlDbType.VarChar)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "TransCashHandling"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                    Return False
                End Try
            Loop

            rZipOut.AddTableCsv(pDtOut, "TransCashHandling")

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function
    Private Function ImportBomMaker(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
    ByVal vOrgID As String, ByVal vUuid As String,
    ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut, pDtOutItems As DataTable
        Dim pDrOut As DataRow
        Dim pNum As Integer
        Dim pDtItems As DataTable
        Dim pDtContents As DataTable
        Dim pItemInitialized As Boolean
        Dim pDataImportLogAlreadyImported As DataImportLog
        Dim pMaxSubNumber As Integer?
        Dim pDvItems As DataView
        Dim pDvContents As DataView

        oError = ""

        DataImportLog.AddLog("Import from TransBomMaker")
        pCsvReader = New CsvReader(vInputZipFilename, "TransBomMaker")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", True, SqlDbType.DateTime)
            .Add("UserID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("DeviceID", True, SqlDbType.UniqueIdentifier)

            .Add("AssignedToID", False, SqlDbType.UniqueIdentifier)
            .Add("DateNeeded", False, SqlDbType.DateTime)
            .Add("Num", True, SqlDbType.Int)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            mBusiness.ProData.RollbackTransaction(True, True)
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If pItemInitialized = False Then
                pItemInitialized = True
                pDtItems = CreateBomMakerProductsDataTable(vInputZipFilename, oError)
                If CStr2(oError) <> "" Then Return False

                pDtContents = CreateBomMakerContentsDataTable(vInputZipFilename, oError)
                If CStr2(oError) <> "" Then Return False
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If

            pIU.TableName = "TransBomMaker"
            mBusiness.ProData.BeginTransaction()
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                pSql = "DELETE FROM TransBomMakerContents WHERE ParentID IN (" &
                    " SELECT ID" &
                    " FROM TransBomMakerProducts" &
                    " WHERE ParentID = " & ReadyString(pLine("ID")) & ")"
                pSql = mBusiness.ProData.ExecuteSQL(pSql) ' get the number of effected records for debugging

                pSql = "DELETE FROM TransBomMakerProducts WHERE ParentID = " & ReadyString(pLine("ID"))
                pSql = mBusiness.ProData.ExecuteSQL(pSql) ' get the number of effected records for debugging

                ' BEGIN insert items
                pDvItems = New DataView(pDtItems, "ParentID = " & ReadyString(pLine("ID")), "", DataViewRowState.CurrentRows)
                For Each pDrvItem As DataRowView In pDvItems
                    pIU = New InsertUpdate("TransBomMakerProducts")
                    pIU("ID") = ReadyString(pDrvItem("ID"))
                    pIU("ParentID") = ReadyString(pLine("ID"))
                    pIU("ProductID") = ReadyString(pDrvItem("ProductID"))
                    pIU("Quantity") = CDec2(pDrvItem("Quantity"))
                    pIU("UnitMeasureID") = ReadyString(pDrvItem("UnitMeasureID"))
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)

                    ' BEGIN insert contents
                    pDvContents = New DataView(pDtContents, "ParentID = " & ReadyString(pDrvItem("ID")), "", DataViewRowState.CurrentRows)
                    For Each pDrvContent As DataRowView In pDvContents
                        pIU = New InsertUpdate("TransBomMakerContents")
                        pIU("ID") = ReadyString(pDrvContent("ID"))
                        pIU("ParentID") = ReadyString(pDrvContent("ParentID"))
                        pIU("ProductID") = ReadyString(pDrvContent("ProductID"))
                        pIU("Quantity") = CDec2(pDrvContent("Quantity"))
                        If pDrvContent("UnitMeasureID") IsNot DBNull.Value Then
                            pIU("UnitMeasureID") = ReadyString(pDrvContent("UnitMeasureID"))
                        End If
                        If pDrvContent("UnitMeasure") IsNot DBNull.Value Then
                            pIU("UnitMeasure") = ReadyString(pDrvContent("UnitMeasure"))
                        End If
                        If pDrvContent("BaseUnitMeasureID") IsNot DBNull.Value Then
                            pIU("BaseUnitMeasureID") = ReadyString(pDrvContent("BaseUnitMeasureID"))
                        End If
                        If pDrvContent("QtyInBaseUnit") IsNot DBNull.Value Then
                            pIU("QtyInBaseUnit") = CDec2(pDrvContent("QtyInBaseUnit"))
                        End If
                        pSql = pIU.GetInsert()
                        mBusiness.ProData.ExecuteSQL(pSql)
                    Next pDrvContent
                    ' END insert contents
                Next pDrvItem
                ' END insert items
                mBusiness.TransBomMaker.AddToInventoryLedger(pLine("ID"))
                mBusiness.ProData.CommitTransaction()

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        If pDtOut.Rows.Count = 0 Then
            If CStr2(oError).Trim <> "" Then
                Return False
            End If
        End If

        rZipOut.AddTableCsv(pDtOut, "TransBomMaker")

        If pDataImportLogAlreadyImported IsNot Nothing Then
            ' ReSharper disable VBWarnings::BC42104
            pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
            ' ReSharper restore VBWarnings::BC42104
        End If

        Return True

    End Function
    'additional by janine feb 3 2015
    Private Function ImportLibraryDevicePrinterFowst(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
       ByVal vOrgID As String, ByVal vUuid As String,
       ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String
        DataImportLog.AddLog("Import from LibraryDevicePrinterFowst")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryDevicePrinterFowst")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", False, SqlDbType.VarChar)
            .Add("PrinterID", False, SqlDbType.VarChar)
            .Add("PrinterIPAddress", False, SqlDbType.VarChar)
            .Add("PrinterName", False, SqlDbType.VarChar)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryDevicePrinterFowst"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)


                pSql2 = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevicePrinter'"
                mBusiness.ProData.ExecuteSQL(pSql2)

            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
        Loop
        rZipOut.AddTableCsv(pDtOut, "LibraryDevicePrinterFowst")

        Return True

    End Function

    Private Function ImportLibraryDeviceFowstPrinter(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip, _
   ByVal vOrgID As String, ByVal vUuid As String,
   ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String
        DataImportLog.AddLog("Import from LibraryDeviceFowstPrinter")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryDeviceFowstPrinter")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("DeviceID", False, SqlDbType.UniqueIdentifier)
            .Add("IsActive", False, SqlDbType.Bit)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
            .Add("PrinterFowstID", False, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryDeviceFowstPrinter"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)

                pSql2 = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
                mBusiness.ProData.ExecuteSQL(pSql2)

            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & GetStackTrace(, ex))
                Return False
            End Try
        Loop
        rZipOut.AddTableCsv(pDtOut, "LibraryDeviceFowstPrinter")

        Return True

    End Function

    ' end of additional 

    Private Function CreateBomMakerProductsDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)

        oError = Nothing
        DataImportLog.AddLog("Import from TransBomMakerProducts")
        pCsvReader = New CsvReader(vInputZipFilename, "TransBomMakerProducts")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ParentID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("UnitMeasureID", True, SqlDbType.UniqueIdentifier)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("ParentID")
            .Add("ProductID")
            .Add("Quantity", GetType(Decimal))
            .Add("UnitMeasureID")
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)
            pDtOut.Rows.Add(pDR)
        Loop

        Return pDtOut

    End Function

    Private Function CreateBomMakerContentsDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)

        oError = Nothing
        DataImportLog.AddLog("Import from TransBomMakerContents")
        pCsvReader = New CsvReader(vInputZipFilename, "TransBomMakerContents")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ParentID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("UnitMeasureID", False, SqlDbType.UniqueIdentifier)

            .Add("UnitMeasure", False, SqlDbType.VarChar)
            .Add("BaseUnitMeasureID", False, SqlDbType.UniqueIdentifier)
            .Add("QtyInBaseUnit", False, SqlDbType.Decimal)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("ParentID")
            .Add("ProductID")
            .Add("Quantity", GetType(Decimal))
            .Add("UnitMeasureID")

            .Add("UnitMeasure")
            .Add("BaseUnitMeasureID")
            .Add("QtyInBaseUnit", GetType(Decimal))
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)
            pDtOut.Rows.Add(pDR)
        Loop

        Return pDtOut

    End Function

    Function GetFranchiseeOrgFOWSTID(ByVal FOWSTID As String) As Boolean
        Dim pSql As String
        Dim dtTemp As DataTable

        pSql = "Select * from MobizLibraryOrganizationFranchisee inner join LibraryFranchiseGroup " & vbCrLf &
                " ON LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                " WHERE FranchiseeOrgFowstID = " & ReadyString(FOWSTID)

        dtTemp = mBusiness.ProData.Execute(pSql)

        If dtTemp.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Shared Function StartTransImport(ByVal vProData As ProData, ByVal vFilename As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransImports")
        pIU("ProcessStart") = "GETDATE()"
        pSql = pIU.GetUpdate("Filename = " & ReadyString(vFilename) &
            " AND ProcessStart IS NULL")
        pSql = vProData.ExecuteSQL(pSql)

        Return pSql

    End Function

    Private Shared Function UpdateTransImport(ByVal vProData As ProData, ByVal vFilename As String, ByVal vResult As String, ByVal vSuccess As Boolean) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String

        pIU = New InsertUpdate("TransImports")
        pIU("ProcessEnd") = "GETDATE()"
        pIU("Result") = ReadyString(vResult)
        pIU("Success") = If(vSuccess, 1, 0)
        pSql = pIU.GetUpdate("Filename = " & ReadyString(vFilename))
        pSql = vProData.ExecuteSQL(pSql)

        Return pSql

    End Function

    Private Function InventoryAdjustment(ByVal FOWST As String) As Date
        Dim pSql As String
        Dim dtTemp As DataTable

        'ByVal FOWSTID As String, ByVal ProductID As String, ByVal BinID As String, 
        pSql = "Select top 1 InventoryCountDate from TransInventoryProductCountStock " & vbCrLf &
                "INNER JOIN TransInventoryProductCount " & vbCrLf &
                "ON TransInventoryProductCountStock.InventoryProductCountID = TransInventoryProductCount.ID " & vbCrLf &
                "WHERE " & vbCrLf &
                "TransInventoryProductCount.FowstID = " & ReadyString(FOWST) & vbCrLf &
                "Order by InventoryCountDate desc"

        '"TransInventoryProductCountStock.BinID = " & ReadyString(BinID) & vbCrLf &
        '"AND TransInventoryProductCount.ProductID = " & ReadyString(ProductID) & vbCrLf &


        dtTemp = mBusiness.ProData.Execute(pSql)

        If dtTemp.Rows.Count > 0 Then
            Return CDate(dtTemp.Rows(0).Item("InventoryCountDate"))
        Else
            Return Nothing
        End If

        'If dtTemp.Rows.Count > 0 Then
        '    Select Case TransType
        '        Case "Receiving"
        '            dtTrans.DefaultView.RowFilter = "LastUpdated > " & ReadyString(dtTemp.Rows(0).Item("InventoryCountDate"))
        '        Case "POS"
        '            dtTrans.DefaultView.RowFilter = "InvoiceDate > " & ReadyString(dtTemp.Rows(0).Item("InventoryCountDate"))
        '        Case "Returns" 'since its online, then we dnt have to apply
        '    End Select

        '    Return dtTrans.DefaultView.ToTable
        'End If

    End Function
End Class
