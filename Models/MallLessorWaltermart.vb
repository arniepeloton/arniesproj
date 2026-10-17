Imports System.Security.Cryptography
Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports System.IO
Imports WinSCP

Public Class MallLessorWaltermart

    ReadOnly mProData As ProData


    Shared mHttpClient = New HttpClient()
    'Dim mLastResponse As LogInput
    'Dim mLastFailResponse As LogInput


    Sub ConnectToDatabase(dbName As String)
        'mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
        mProData.Execute("USE " & EscapeSqlName(dbName))
    End Sub

    Sub New(Optional ByRef rProData As ProData = Nothing)
        mProData = rProData
    End Sub

    Private Function IsRunWaltermartDataSender() As Boolean

        Dim pFilename = Common.ServerPath & "settings\IsRunWaltermartDataSender.txt"
        Dim pText = CStr2(FileText(pFilename))
        Dim pOut = CBool2(pText)
        FileText(pFilename) = pOut

        Return pOut

    End Function

    Function StartSendingTransactionToLessor() As String
        Dim dtZRead As DataTable
        Dim dtPayment As DataTable
        Dim pSourceURL As String = Common.ServerPath & "temp\Waltermart"
        Dim strFileName As String
        Dim FOWSTID As String
        Dim DeviceID As String
        Dim DateFrom As String
        Dim DateTo As String
        Dim WMStoreID As String
        Dim WMDeviceSerialID As String

        Dim pFtpServer As String
        Dim pFtpUsername As String
        Dim pFtpPassword As String
        Dim pFullFilename As String
        Dim pFilename As String
        Dim pSql As String
        Dim LastTransactionDate As String
        Dim CurTransactionDate As String

#If DEBUG Then
        FOWSTID = "fe639d1f-cd83-4b3e-8b0a-586731f479cb"
        DeviceID = "8FD8C454-B932-43AC-9869-9E7C9B07F982"
        DateFrom = DateAdd(DateInterval.Day, -1, Today)
        DateTo = DateAdd(DateInterval.Day, -1, Today)
        WMStoreID = "3c60f1bb-4da2-4939-86b7-fe5147a51f9f" ' WALTERMART MUNOZ
        WMDeviceSerialID = "358679590362249" ' WALTERMART MUNOZ
#Else
        FOWSTID = "a511473b-ec9a-45a2-9525-252a6df48808"
        DeviceID = "c8dfd7b5-2303-4bf9-ab08-381242451393"
        DateFrom = DateAdd(DateInterval.Day, -1, Today)
        DateTo = DateAdd(DateInterval.Day, -1, Today)
        WMStoreID = "3c60f1bb-4da2-4939-86b7-fe5147a51f9f" ' WALTERMART MUNOZ
        WMDeviceSerialID = "358679590362249" ' WALTERMART MUNOZ     
#End If

        pSql = "Select top 1 InvoiceDate from CacheInvoices where FOWSTID = 'a511473b-ec9a-45a2-9525-252a6df48808' ORDER BY invoicedate desc"
        CurTransactionDate = CStr2(mProData.ExecuteScalar(pSql))

        AppendToFile(Common.ServerPath & "temp\logs\Waltermart.txt", "CurTransactionDate " & CurTransactionDate)

        If DateDiff(DateInterval.Day, CDate(CurTransactionDate), Today) = 0 Then 'there new transaction today. you can now send last days' z-read

            pSql = "Select DefaultValue  from systemdefaults where [description] = 'LastSendOfWaltermartData'"
            LastTransactionDate = CStr2(mProData.ExecuteScalar(pSql))

            AppendToFile(Common.ServerPath & "temp\logs\Waltermart.txt", "LastTransactionDate " & LastTransactionDate)

            If DateDiff(DateInterval.Day, CDate(LastTransactionDate), Today) > 0 Then

                dtZRead = getDailySales(mProData, DateFrom, DateTo, FOWSTID, DeviceID)
                dtPayment = getOtherPaymentTypes(mProData, DateFrom, DateTo, FOWSTID, DeviceID)

                strFileName = GenerateZReadReport(DateFrom, dtZRead, dtPayment, FOWSTID, WMStoreID, WMDeviceSerialID)

                pFtpServer = "sftp.stage.kulay.io"
                pFtpUsername = "sftp-gnrtmm"
                pFtpPassword = ""
                pFullFilename = strFileName
                pFilename = strFileName.Split("\")(strFileName.Split("\").Length - 1)

                WaltermartSFtpUpload(vFtpServer:=pFtpServer,
                                  vFtpUsername:=pFtpUsername,
                                  vFtpPassword:=pFtpPassword,
                                  vSourceFilename:=pFullFilename,
                                  vDestinationFilename:=pFilename)

                mProData.Execute("Update SystemDefaults set DefaultValue = GETDATE() where [description] = 'LastSendOfWaltermartData'")

                '=====================================================
                'store id: 3c60f1bb-4da2-4939-86b7-fe5147a51f9f
                'sftp config
                '{
                '"sftpUsername": "sftp-gnrtmm",
                '"sftpHost": "sftp.stage.kulay.io",
                '"sftpPort": 2222,
                '"fingerprint": "SHA256:I+dwwG/FFgAR2kNYYjtqOv5HogBW6ntbSUY0Ad/bWUA="
                '}

                '=====================================================
            End If

        End If

    End Function

    Friend Shared Sub WaltermartSFtpUpload(vFtpServer As String, vFtpUsername As String, vFtpPassword As String,
        vSourceFilename As String, vDestinationFilename As String)

        '===================Sample code from winscp===================
        '' Set up session options
        'Dim sessionOptions As New SessionOptions
        'With sessionOptions
        '    .Protocol = Protocol.Sftp
        '    .HostName = "sftp.stage.kulay.io"
        '    .PortNumber = 2222
        '    .UserName = "sftp-gnrtmm"
        '    .SshHostKeyFingerprint = "ssh-ed25519 255 xZnkcqs/VyrfpX7PUSdRjTymWk040ViB2BEt0KPates"
        '    .SshPrivateKeyPath = "C:\DotNet\Mobiz\settings\kulay-sftp.ppk"
        'End With

        'sessionOptions.AddRawSettings("Utf", "1")
        'sessionOptions.AddRawSettings("ProxyPort", "1")

        'Using session As New Session
        '    ' Connect
        '    session.Open(sessionOptions)

        '    ' Your code
        'End Using

        '===================End of Sample code from winscp===================

        Try
            'Dim filereader As StreamReader = New StreamReader(ftpfilepath)
            'Dim filedata As Object = objJSSerializer.Deserialize(Of Object)(filereader.ReadLine)
            'filereader.Close()

            ' Setup session options
            Dim sessionOptions As New SessionOptions

#If DEBUG Then
            With sessionOptions
                .Protocol = Protocol.Sftp
                .HostName = "sftp.stage.kulay.io"
                .PortNumber = 2222
                .UserName = "sftp-gnrtmm"
                .SshHostKeyFingerprint = "ssh-ed25519 255 xZnkcqs/VyrfpX7PUSdRjTymWk040ViB2BEt0KPates"
                .SshPrivateKeyPath = "C:\DotNet\Mobiz\settings\kulay-sftp.ppk"
            End With
#Else
                    With sessionOptions
                        .Protocol = Protocol.Sftp
                        .HostName = "sftp.stage.kulay.io"
                        .PortNumber = 2222
                        .UserName = "sftp-gnrtmm"
                        '.SshHostKeyFingerprint = "SHA256:I+dwwG/FFgAR2kNYYjtqOv5HogBW6ntbSUY0Ad/bWUA="
                        .SshHostKeyFingerprint = "ssh-ed25519 255 xZnkcqs/VyrfpX7PUSdRjTymWk040ViB2BEt0KPates"
                        .SshPrivateKeyPath = Common.ServerPath & "settings\kulay-sftp.ppk"
                    End With
#End If

            sessionOptions.AddRawSettings("ConsiderDST", "0")
            sessionOptions.AddRawSettings("Utf", "1")
            sessionOptions.AddRawSettings("ProxyPort", "1")

            Using session As New Session

                session.DisableVersionCheck = True

                ' Connect
                session.Open(sessionOptions)

                ' Upload files
                Dim transferOptions As New TransferOptions
                transferOptions.TransferMode = TransferMode.Binary

                Dim folder As String = ""

#If DEBUG Then
                folder = "/inbox/zreadings"
#Else
                folder = "/inbox/zreadings"
#End If

                If Not session.FileExists(folder) Then
                    session.CreateDirectory(folder)
                End If

                Dim transferResult As TransferOperationResult
                transferResult = session.PutFiles(vSourceFilename, folder & "/*.*", False, transferOptions)
                transferResult.Check()
            End Using

            AppendToFile(Common.ServerPath & "temp\logs\Waltermart.txt", vSourceFilename)

        Catch ex As Exception
            AppendToDebug("FTP Error: " & ex.Message)
        End Try

    End Sub

    Function getDailySales(ByVal mProdata As ProData, ByVal dtDateFrom As DateTime, ByVal dtDateTo As DateTime,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        If CDate(dtDateFrom).TimeOfDay.TotalSeconds = 0 Then
            dtDateFrom = dtDateFrom & " 12:00:00 AM"
        End If

        If CDate(dtDateTo).TimeOfDay.TotalSeconds = 0 Then
            dtDateTo = dtDateTo & " 11:59:59 PM"
        End If

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = mProdata

            With .Fields
                With .Add("TransDate", SqlDbType.DateTime)
                    .Expression = "'" & IIf(dtDateFrom.ToString("MM/dd/yyyy") = dtDateTo.ToString("MM/dd/yyyy"), dtDateFrom.ToString("MM/dd/yyyy"), dtDateFrom.ToString("MM/dd/yyyy") & " - " & dtDateTo.ToString("MM/dd/yyyy")) & "'"
                    .Searchable = False
                End With
                With .Add("BegBal", SqlDbType.Money)
                    .Expression = " ISNULL(devf.BegBalance,0) "
                End With
                With .Add("PrevGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(invitem.GrossSalesNetOfVATExemptions, 2)) " & vbCrLf &
                               " FROM TransInvoiceItems invitem " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)" & vbCrLf
                End With
                'With .Add("PrevGrossSales", SqlDbType.Money)
                '    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
                '               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                '               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                '               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                '               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                '               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                '               " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString( dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                '               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                '               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                '               ",0)" & vbCrLf
                '    '      " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015

                'End With
                With .Add("PrevTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(custpay.DiscountAmount, 2)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " & vbCrLf &
                               " AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df', 'e95f0b64-1eea-47b4-bde5-b795234b462d') -- Senior, PWD, SoloParent " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND disc.NoOfSeniorCitizens IS NULL " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevSoloParentDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.SoloParentDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Solo Parent Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevPhNatTeamDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.PhNatTeamDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Philippine National Sports Team Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevTotalSCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.SCDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount' " & vbCrLf &
                               " OR LibraryDiscountSettings.DiscountType = 'Person With Disability Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(srledger.GrossRefundAmount, 2)) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE CONVERT(datetime, memo.LastUpdated) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(srledger.GrossSrvAmount, 2)) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE CONVERT(datetime, memo.LastUpdated) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevTotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(invitem.TaxAmount, 2),0)- ISNULL(ROUND(invitem.DiscountTax, 2),0)) " & vbCrLf &
                               " FROM --TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " --INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " --INNER JOIN " & vbCrLf &
                               " TransInvoice inv --ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 --AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " and ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                    '.Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)) " & vbCrLf &
                    '           " FROM TransInvoiceItems invitem " & vbCrLf &
                    '           " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                    '           " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                    '           " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                    '           " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                    '           " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                    '           ",0) " & vbCrLf &
                    '           " - " & vbCrLf &
                    '           "ISNULL((SELECT SUM(sr.TaxAmount) " & vbCrLf &
                    '           " FROM TransMemoSalesReturn sr " & vbCrLf &
                    '           " INNER JOIN TransMemo memo ON (sr.MemoID = memo.ID) " & vbCrLf &
                    '           " WHERE CONVERT(datetime, memo.LastUpdated) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                    '           " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                    '           " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                    '           ",0)"
                End With
                With .Add("PrevServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(srledger.Fees, 2), 0) + ISNULL(ROUND(srledger.RefundFees, 2), 0)) " & vbCrLf &
                                  " FROM TransSalesReturnLedger srledger " & vbCrLf &
                                  " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                                  " WHERE CONVERT(datetime, memo.LastUpdated)  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                                  " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                                  " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                                  ",0) --+" & vbCrLf
                    '"ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " & vbCrLf &
                    '" FROM TransInvoice inv " & vbCrLf &
                    '" INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " & vbCrLf &
                    '" WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                    '" AND CONVERT(datetime, inv.InvoiceDate)  < " & ReadyString( dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                    '" AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                    '" AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                    ' ",0)"
                End With
                With .Add("PrevChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(custpay.ChargeAmount, 2),0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("PrevVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.ForegoneVAT, 2)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(custpay.ChargeAmount, 2),0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
                               " FROM TransInvoiceItems invitem " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With

                .Add("CurTotalDiscount", SqlDbType.Money, "ISNULL(tblDiscount.Sum, 0)")
                .Add("CurDiscountCount", SqlDbType.Int, "ISNULL(tblDiscount.Count, 0)")


                .Add("CurSalesReturnRefund", SqlDbType.Money, "ISNULL(tblRefund.Sum, 0)")
                .Add("CurSalesReturnRefundCount", SqlDbType.Int, "ISNULL(tblRefund.Count, 0)")



                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(srledger.GrossSrvAmount, 2)) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTotalVAT", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)) " &
                    .Expression = "ISNULL((SELECT ROUND(SUM(ISNULL(invitem.TaxAmount,0)- ISNULL(invitem.DiscountTax,0)), 2) " & vbCrLf &
                               " FROM --TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " --INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " --INNER JOIN " & vbCrLf &
                               " TransInvoice inv --ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 --AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " and ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevEndInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT TOP 1 MAX(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurBegInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MIN(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurEndInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MAX(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountMemoReturnExchange", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.SrvNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTransCountMemoRefund", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTransInvItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(invitem.ProductID) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchangeItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.SrvNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesRefundItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTotalVatablePortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.Vatable, 2)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVatAmountPortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.VAT, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0 " &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSCDiscountExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.VAT) + SUM(Vatable) " &
                               " FROM TransInvoice inv " &
                               " WHERE inv.InvoiceNumber > 0 AND inv.SCDiscount > 0" &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevSCDiscountExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.VAT) + SUM(Vatable) " &
                               " FROM TransInvoice inv " &
                               " WHERE inv.InvoiceNumber > 0 AND inv.SCDiscount > 0" &
                               " AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTransCountMemo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memo.MemoNumber) " & vbCrLf &
                               " FROM TransMemo memo " & vbCrLf &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurLocalTax", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " & vbCrLf &
                                              " FROM TransInvoice inv " & vbCrLf &
                                              " INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " & vbCrLf &
                                              " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                                              " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                                              " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                                              " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                                              ",0)"
                End With
                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(srledger.Fees, 2), 0) + ISNULL(ROUND(srledger.RefundFees, 2), 0)) " & vbCrLf &
                                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                                               " WHERE CONVERT(datetime, memo.LastUpdated)  BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                                               ",0) --+" & vbCrLf
                    '"ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " & vbCrLf &
                    '" FROM TransInvoice inv " & vbCrLf &
                    '" INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " & vbCrLf &
                    '" WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                    '" AND CONVERT(datetime, inv.InvoiceDate)  BETWEEN " & ReadyString( dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                    '" AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                    '" AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                    ' ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(invitem.NonVATSale, 2)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurVATExemptSales", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - invitem.DiscountAmount) " &
                    .Expression = "ISNULL((SELECT SUM(ROUND(inv.VATExempt, 2)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' ) AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) - " & vbCrLf &
                               " ISNULL((SELECT SUM(ROUND((items.UnitPrice * memosr.QuantityReturned) / 1.12, 2)) " & vbCrLf &
                               " FROM TransMemoSalesReturn memosr " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice as inv ON inv.ID = memosr.InvoiceID " & vbCrLf &
                               " INNER JOIN TransInvoiceItems as items ON items.InvoiceID = inv.ID AND items.ProductID = memosr.ProductID " & vbCrLf &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID AND (ISNULL(inv.SCDiscount, 0) > 0 OR ISNULL(inv.SoloParentDiscount, 0) > 0)) " & vbCrLf &
                               ",0)" & vbCrLf
                    'AND invitem.TaxAmount = 0 AND ISNULL(invitem.VATableSale,0) = 0 
                End With
                With .Add("CurVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.ForegoneVAT, 2)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevCashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevDebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevCreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevCheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurPotatoCornerGC", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                            " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                            " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                            " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                            " AND inv.DeviceID = devf.DeviceID " & vbCrLf &
                            " AND other.PaymentType = 'PotatoCornerGC' " & vbCrLf &
                            "),0)"
                End With
                With .Add("PrevPotatoCornerGC", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                            " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                            " WHERE CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                            " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                            " AND inv.DeviceID = devf.DeviceID " & vbCrLf &
                            " AND other.PaymentType = 'PotatoCornerGC' " & vbCrLf &
                            "),0)"
                End With

                'With .Add("CurTransCountInv", SqlDbType.Int)
                '    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " & vbCrLf &
                '               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                '               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                '               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                '               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                '               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                '               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                '               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                '               ",0)"
                'End With

                With .Add("VoucherAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                              " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                              " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                              " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                              " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                              ",0)"
                End With
                With .Add("PrevVoucherAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                              " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                              " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                              " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                              " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                              ",0)"
                End With
                With .Add("CurOthers", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                            " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                            " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                            " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                            " AND inv.DeviceID = devf.DeviceID " & vbCrLf &
                            "),0)"
                End With
                With .Add("PrevOthers", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                            " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                            " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                            " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                            " AND inv.DeviceID = devf.DeviceID " & vbCrLf &
                            "),0)"
                End With
                With .Add("CurTermNum", SqlDbType.Money)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("DeviceID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                End With

                With .Add("TotalSoloParentDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.SoloParentDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Solo Parent Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("TotalPhNatTeamDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.PhNatTeamDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Philippine National Sports Team Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("TotalSCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.SCDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount' " & vbCrLf &
                               " OR LibraryDiscountSettings.DiscountType = 'Person With Disability Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurCustomerCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT SUM(inv.TotalNumberOfCustomer) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevSRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("OtherPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("OnCreditAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OnCreditAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSales", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)) " &
                    .Expression = "ISNULL((SELECT ROUND(SUM(ISNULL(invitem.TaxSales,0)- ISNULL(invitem.DiscountSales,0)), 2) " & vbCrLf &
                               " FROM --TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " --INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " --INNER JOIN " & vbCrLf &
                               " TransInvoice inv --ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  --AND custpay.ReceiptType <> 'RF'" & vbCrLf &
                               " and ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 " & vbCrLf &
                               " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT ROUND(SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)), 2) " & vbCrLf &
                               " FROM TransMemoSalesReturn memosr " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice as inv ON inv.ID = memosr.InvoiceID " & vbCrLf &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID AND ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0) " & vbCrLf &
                               ",0)" & vbCrLf
                End With
                With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(memosr.TaxAmount, 2)) " & vbCrLf &
                               " FROM TransMemoSalesReturn memosr " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice as inv ON inv.ID = memosr.InvoiceID " & vbCrLf &
                               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID AND ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0) " & vbCrLf &
                               ",0)" & vbCrLf
                End With
                'With .Add("CurTotalVATExemptSalesReturn", SqlDbType.Money)
                '    .Expression = "ISNULL((SELECT SUM(ROUND((items.UnitPrice * memosr.QuantityReturned) / 1.12, 4)) " & vbCrLf &
                '               " FROM TransMemoSalesReturn memosr " & vbCrLf &
                '               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                '               " INNER JOIN TransInvoice as inv ON inv.ID = memosr.InvoiceID " & vbCrLf &
                '               " INNER JOIN TransInvoiceItems as items ON items.InvoiceID = inv.ID " & vbCrLf &
                '               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                '               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                '               " AND memo.DeviceID = devf.DeviceID AND (ISNULL(inv.SCDiscount, 0) > 0 OR ISNULL(inv.SoloParentDiscount, 0) > 0)) " & vbCrLf &
                '               ",0)" & vbCrLf
                'End With
                With .Add("ZCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT ISNULL(ZReadCount, 1) FROM TransZReadCache WHERE ISNULL(ZReadCount, 0) > 0 AND FOWSTID = devf.FowstID AND DeviceID = devf.ID AND CONVERT(date, SalesDate) = CONVERT(date, " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & ")), 1)"
                End With
                With .Add("ReprintedAmount", SqlDbType.Money)
                    .Expression = "ISNULL(tblReprint.Sum, 0)"
                End With
                With .Add("ReprintedCount", SqlDbType.Int)
                    .Expression = "ISNULL(tblReprint.Count, 0)"
                End With
                With .Add("CurVoid", SqlDbType.Money)
                    .Expression = "ISNULL(tblVoid.Sum, 0)"
                End With
                With .Add("CurVoidItemQty", SqlDbType.Int)
                    .Expression = "ISNULL(tblVoid.Count, 0)"
                End With
            End With

            .SqlFrom = " FROM LibraryDeviceFowst devf " & vbCrLf &
                    " INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) " & vbCrLf

            .SqlFrom &= " OUTER APPLY(" & vbCrLf &
                "SELECT SUM(ROUND(custpay.DiscountAmount, 2)) AS Sum " & vbCrLf &
                ",COUNT(DISTINCT custpayinv.InvoiceID) AS Count " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " & vbCrLf &
                " AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df', 'e95f0b64-1eea-47b4-bde5-b795234b462d') -- Senior, PWD, SoloParent " & vbCrLf &
                " WHERE inv.InvoiceNumber > 0 And custpay.ReceiptType <> 'RF' AND disc.NoOfSeniorCitizens IS NULL " & vbCrLf &
                " AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                " AND inv.DeviceID = devf.DeviceID" & vbCrLf &
                ") tblDiscount" & vbCrLf


            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(srledger.GrossRefundAmount) AS Sum" & vbCrLf &
                "   ,COUNT(DISTINCT memo.id) AS Count" & vbCrLf &
                "  	FROM TransSalesReturnLedger srledger" & vbCrLf &
                "  	JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                "   WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                "  	AND memo.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND memo.DeviceID = devf.DeviceID" & vbCrLf &
                " 	AND srledger.GrossRefundAmount > 0" & vbCrLf &
                " ) tblRefund" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoice.TotalAmount) AS Sum" & vbCrLf &
                "   	,COUNT(DISTINCT TransInvoice.ID) AS Count" & vbCrLf &
                "  	FROM TransInvoice" & vbCrLf &
                "  	WHERE CONVERT(datetime, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                "  	AND TransInvoice.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND TransInvoice.DeviceID = devf.DeviceID" & vbCrLf &
                " 	AND TransInvoice.PrintCount > 1" & vbCrLf &
                " ) tblReprint" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoiceItemsVoided.Quantity * TransInvoiceItemsVoided.UnitPrice) AS Sum" & vbCrLf &
                "   	,CONVERT(int, SUM(TransInvoiceItemsVoided.Quantity)) AS Count" & vbCrLf &
                "  	FROM TransInvoiceItemsVoided" & vbCrLf &
                "  	WHERE CONVERT(datetime, TransInvoiceItemsVoided.DateTimeRemoved) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                "  	AND TransInvoiceItemsVoided.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND TransInvoiceItemsVoided.DeviceID = devf.DeviceID" & vbCrLf &
                " ) tblVoid" & vbCrLf


            .SqlWhere = " WHERE devf.FowstID = " & vbCrLf & ReadyString(vFowstID) & vbCrLf &
                        " AND devf.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
                        " /* vbA7XJ7CTDKZ vb92AQQE34NK */"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getOtherPaymentTypes(ByVal mProdata As ProData, ByVal dtDateFrom As DateTime, ByVal dtDateTo As DateTime,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing,
        Optional vUserID As String = Nothing) As DataTable

        If CDate(dtDateFrom).TimeOfDay.TotalSeconds = 0 Then
            dtDateFrom = dtDateFrom & " 12:00:00 AM"
        End If

        If CDate(dtDateTo).TimeOfDay.TotalSeconds = 0 Then
            dtDateTo = dtDateTo & " 11:59:59 PM"
        End If

        Dim pSql As String

        pSql = " SELECT " & vbCrLf &
            " DISTINCT other.PaymentType " & vbCrLf &
            " , SUM(other.Amount) " & vbCrLf &
            " - ISNULL(" & vbCrLf &
            "   (SELECT ISNULL(SUM(ROUND(ledger.Amount, 2)), 0) " & vbCrLf &
            "   FROM TransSalesReturnLedger as ledger " & vbCrLf &
            "   INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " & vbCrLf &
            "   INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID " & vbCrLf &
            "   INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " & vbCrLf &
            "   INNER JOIN TransCustomerPaymentOther refundother ON (refundother.CustomerPaymentID = custpay.ID) " & vbCrLf &
            "   INNER JOIN TransInvoice inv ON (cuspayinv.InvoiceID = inv.ID) " & vbCrLf &
            "   WHERE ledger.FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
            "   AND ledger.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            "   AND CONVERT(date, ledger.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
            "   AND ISNULL(RefundNum, 0) > 0 " & vbCrLf &
            "   AND refundother.PaymentType = other.PaymentType " & vbCrLf

        If Not String.IsNullOrWhiteSpace(vUserID) Then
            pSql += " AND inv.UserID = " & ReadyString(vUserID) & vbCrLf
        End If

        pSql += " ), 0) as Amount " & vbCrLf &
            " FROM TransCustomerPaymentInvoices custpayinv " &
            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
            " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
            " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
            " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
            " AND inv.FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
            " AND inv.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND other.PaymentType <> 'PotatoCornerGC' " & vbCrLf

        If Not String.IsNullOrWhiteSpace(vUserID) Then
            pSql += " AND inv.UserID = " & ReadyString(vUserID) & vbCrLf
        End If

        pSql += " GROUP BY other.PaymentType " & vbCrLf &
            " ORDER BY other.PaymentType ASC "

        Return mProdata.Execute(pSql)
    End Function

    Function GenerateZReadReport(ZReadDate As String, dtZRead As DataTable, dtPayment As DataTable, FOWSTID As String, WMStoreID As String, WMDeviceSerialID As String) As String
        Dim dtFinal As New DataTable
        Dim pSql As String
        Dim dtDailySalesInformation As DataTable
        Dim pFilename As String

        Try

            dtFinal.Columns.Add("serialNumber")
            dtFinal.Columns.Add("terminalNumber")
            dtFinal.Columns.Add("generatedBy")
            dtFinal.Columns.Add("date")
            dtFinal.Columns.Add("time")
            dtFinal.Columns.Add("startTime")
            dtFinal.Columns.Add("endTime")
            dtFinal.Columns.Add("reportGeneratedAt")
            dtFinal.Columns.Add("minSalesInvoiceNumber")
            dtFinal.Columns.Add("maxSalesInvoiceNumber")
            dtFinal.Columns.Add("minVoidNumber")
            dtFinal.Columns.Add("maxVoidNumber")
            dtFinal.Columns.Add("minReturnNumber")
            dtFinal.Columns.Add("maxReturnNumber")
            dtFinal.Columns.Add("zResetCounterValue")
            dtFinal.Columns.Add("zCounterValue")
            dtFinal.Columns.Add("presentAccumulatedSales")
            dtFinal.Columns.Add("previousAccumulatedSales")
            dtFinal.Columns.Add("totalSalesForTheDay")
            dtFinal.Columns.Add("totalVatableSales")
            dtFinal.Columns.Add("totalVat")
            dtFinal.Columns.Add("totalVatExemptSales")
            dtFinal.Columns.Add("totalZeroRatedSales")
            dtFinal.Columns.Add("totalGrossAmount")
            dtFinal.Columns.Add("totalDiscount")
            dtFinal.Columns.Add("totalSeniorCitizenDiscount")
            dtFinal.Columns.Add("totalPersonsWithDisabilitiesDiscount")
            dtFinal.Columns.Add("totalNationalAthletesAndCoachesDiscount")
            dtFinal.Columns.Add("totalMedalOfValorDiscount")
            dtFinal.Columns.Add("totalSoloParentDiscount")
            dtFinal.Columns.Add("totalOtherDiscount")
            dtFinal.Columns.Add("totalVoidedAmount")
            dtFinal.Columns.Add("totalRefundedAmount")
            dtFinal.Columns.Add("totalVatAdjustment")
            dtFinal.Columns.Add("salesAdjustmentByVoid")
            dtFinal.Columns.Add("salesAdjustmentByRefund")
            dtFinal.Columns.Add("seniorCitizenTransactionVatAdjusted")
            dtFinal.Columns.Add("personsWithDisabilitiesTransactionVatAdjusted")
            dtFinal.Columns.Add("nationalAthletesAndCoachesTransactionVatAdjusted")
            dtFinal.Columns.Add("medalOfValorTransactionVatAdjusted")
            dtFinal.Columns.Add("soloParentTransactionVatAdjusted")
            dtFinal.Columns.Add("regularDiscountTransactionVatAdjusted")
            dtFinal.Columns.Add("zeroRatedTransactionVatAdjusted")
            dtFinal.Columns.Add("otherTransactionVatAdjustment")
            dtFinal.Columns.Add("vatOnRefunds")
            dtFinal.Columns.Add("vatOnVoids")
            dtFinal.Columns.Add("totalSale")
            dtFinal.Columns.Add("openingCash")
            dtFinal.Columns.Add("totalCashInDrawer")
            dtFinal.Columns.Add("totalCashAdditions")
            dtFinal.Columns.Add("totalCashWithdrawals")
            dtFinal.Columns.Add("totalCustomerPayments")
            dtFinal.Columns.Add("totalPaymentsReceived")
            dtFinal.Columns.Add("overallTotalSalesCount")
            dtFinal.Columns.Add("taxComputationMode")
            dtFinal.Columns.Add("paymentMethods")


            pSql = "Select MAX(Invoicedate) MaxInvoiceDate, MIN(Invoicedate) MinInvoiceDate from transinvoice WHERE convert(date,Invoicedate)  =  convert(date," & ReadyString(ZReadDate) & ")"
            dtDailySalesInformation = mProData.Execute(pSql)

            Dim dr As DataRow

            dr = dtFinal.NewRow

            'Identity & Timing (8 columns)

            dr("serialNumber") = WMDeviceSerialID 'WM Munoz UUID, change to WMDeviceSerialID
            dr("terminalNumber") = 1
            dr("generatedBy") = "Francis Hernandez"
            dr("date") = CDate(ZReadDate).ToString("yyyyMMdd")
            dr("time") = CDate(ZReadDate).ToString("hhmmss")
            dr("startTime") = CDate(ZReadDate).ToString("hhmmss")
            dr("endTime") = CDate(ZReadDate).ToString("hhmmss")
            dr("reportGeneratedAt") = Today.Date.ToString("hhmmss")

            'Invoice Counters (6 columns)

            Dim scdiscvatablesales As Decimal = 0, scdiscvatableamount As Decimal = 0

            If CDec2(dtZRead.Rows(0).Item("CurSCDiscountExempt")) > 0 Then
                scdiscvatablesales = CDec2(CDec2(dtZRead.Rows(0).Item("CurSCDiscountExempt")) / 1.12)
                scdiscvatableamount = CDec2(CDec2(dtZRead.Rows(0).Item("CurSCDiscountExempt")) - scdiscvatablesales)
            End If

            dr("minSalesInvoiceNumber") = dtZRead.Rows(0).Item("CurBegInvoiceNo")
            dr("maxSalesInvoiceNumber") = dtZRead.Rows(0).Item("CurEndInvoiceNo")
            dr("minVoidNumber") = 0 ' dtZRead.Rows(0).Item("")
            dr("maxVoidNumber") = 0 'dtZRead.Rows(0).Item("")
            dr("minReturnNumber") = 0 'dtZRead.Rows(0).Item("")
            dr("maxReturnNumber") = 0 'dtZRead.Rows(0).Item("")

            dr("zResetCounterValue") = 0 'dtZRead.Rows(0).Item("")
            dr("zCounterValue") = dtZRead.Rows(0).Item("ZCount")
            dr("presentAccumulatedSales") = CDec2(dtZRead.Rows(0).Item("PrevGrossSales")) + CDec2(dtZRead.Rows(0).Item("CurGrossSales"))
            dr("previousAccumulatedSales") = dtZRead.Rows(0).Item("PrevGrossSales")
            dr("totalSalesForTheDay") = dtZRead.Rows(0).Item("CurGrossSales")
            dr("totalVatableSales") = CDec2(dtZRead.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtZRead.Rows(0).Item("CurTotalVATSales")) - CDec2(dtZRead.Rows(0).Item("CurTotalVATSalesSalesReturn"))
            dr("totalVat") = (CDec2(dtZRead.Rows(0).Item("CurTotalVAT")) + scdiscvatableamount) - CDec2(dtZRead.Rows(0).Item("CurTotalVATAmountSalesReturn"))
            dr("totalVatExemptSales") = CDec2(dtZRead.Rows(0).Item("CurVATExemptSales"))
            dr("totalZeroRatedSales") = 0
            dr("totalGrossAmount") = dtZRead.Rows(0).Item("CurGrossSales")

            'Discounts (8 columns)

            dr("totalDiscount") = dtZRead.Rows(0).Item("CurTotalDiscount")
            dr("totalSeniorCitizenDiscount") = dtZRead.Rows(0).Item("TotalSCDiscount")
            dr("totalPersonsWithDisabilitiesDiscount") = 0
            dr("totalNationalAthletesAndCoachesDiscount") = dtZRead.Rows(0).Item("TotalPhNatTeamDiscount")
            dr("totalMedalOfValorDiscount") = 0
            dr("totalSoloParentDiscount") = dtZRead.Rows(0).Item("TotalSoloParentDiscount")
            dr("totalOtherDiscount") = dtZRead.Rows(0).Item("CurTotalDiscount")
            dr("totalVoidedAmount") = 0

            'VAT Adjustments & Refunds (12 columns)

            dr("totalRefundedAmount") = 0
            dr("totalVatAdjustment") = dtZRead.Rows(0).Item("CurSalesReturnRefund") / 1.12
            dr("salesAdjustmentByVoid") = 0
            dr("salesAdjustmentByRefund") = dtZRead.Rows(0).Item("CurSalesReturnRefund")
            dr("seniorCitizenTransactionVatAdjusted") = dtZRead.Rows(0).Item("CurVATExemptSales") ' foregone vat?
            dr("personsWithDisabilitiesTransactionVatAdjusted") = 0
            dr("nationalAthletesAndCoachesTransactionVatAdjusted") = 0
            dr("medalOfValorTransactionVatAdjusted") = 0
            dr("soloParentTransactionVatAdjusted") = 0
            dr("regularDiscountTransactionVatAdjusted") = 0
            dr("zeroRatedTransactionVatAdjusted") = 0
            dr("otherTransactionVatAdjustment") = 0
            dr("vatOnRefunds") = dtZRead.Rows(0).Item("CurSalesReturnRefund") / 1.12
            dr("vatOnVoids") = 0

            ' Cash Drawer & Payments (9 columns)

            dr("totalSale") = dtZRead.Rows(0).Item("CurGrossSales")
            dr("openingCash") = 0
            dr("totalCashInDrawer") = dtZRead.Rows(0).Item("CurGrossSales")
            dr("totalCashAdditions") = 0
            dr("totalCashWithdrawals") = 0
            dr("totalCustomerPayments") = dtZRead.Rows(0).Item("CurGrossSales")
            dr("totalPaymentsReceived") = dtZRead.Rows(0).Item("CurGrossSales")
            dr("overallTotalSalesCount") = dtZRead.Rows(0).Item("CurTransCountInv")
            dr("taxComputationMode") = "VAT"

            Dim PaymentString As String = ""

            For Each op As Object In dtPayment.Rows
                PaymentString &= op("PaymentType") & ":" & CDec2(op("Amount")) & "|"
            Next

            dr("paymentMethods") = PaymentString

            'Commit

            dtFinal.Rows.Add(dr)

            'Create File
            Dim storeID As String
            Dim deviceserial As String
            Dim sendingdate As String
            Dim sequence As String = "001"

            sendingdate = CDate(ZReadDate).ToString("yyyyMMdd")

            Dim pSourceURL As String = Common.ServerPath & "temp\Waltermart\" & FOWSTID
            Dim strFileName = WMStoreID & "_" & WMDeviceSerialID & "_" & sendingdate & "_" & sequence & ".csv"

            pFilename = pSourceURL & "\" & strFileName

            If Not System.IO.Directory.Exists(pSourceURL) Then
                IO.Directory.CreateDirectory(pSourceURL)
            End If

            TableToExcelFile(dtFinal, "", "", pFilename)

            Dim pCsvWriter = New CsvWriter()

            pCsvWriter.Destination = pFilename
            pCsvWriter.MaxLength = Integer.MaxValue
            pCsvWriter.Source = dtFinal
            Dim pText = pCsvWriter.EncodeBatch()

            Return pFilename

        Catch ex As Exception
            AppendToLog(ex.Message)
        End Try
    End Function

End Class
