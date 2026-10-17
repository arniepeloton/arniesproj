Imports Newtonsoft.Json
Imports System.Collections.Specialized
Imports System.Threading

Namespace WebProj

    Public Class SMSController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Triggers

        Dim mBusiness As Business
        Private sFtp As Renci.SshNet.SftpClient
        Private myFileStream As System.IO.FileStream
        Private mProData As ProData
        Dim mZip As ProZip

        Function Index() As ActionResult
            StartTrigger()
            AppendToLog("Reports and Subscription has been triggered successfully")
            Return View()
        End Function

        Function StartTrigger()
            Dim smsMessage As New SMS()
            Dim t As Boolean
            Dim MyLocation As String()

            MyLocation = Split(Common.WebRequest.QueryString("Message"), "-")

            t = smsMessage.SendSMS("Amen", MyLocation(1))
        End Function

        Function SaveSMSMessage() As String
            Dim smsMessage As New SMS()
            Dim mMessage As String()
            Dim pSql As String
            Dim dbName As String
            Dim mOrgID As String
            Dim mDeviceID As String
            Dim mDeviceUUID As String
            Dim ID As String
            Dim dtMessage As DataTable
            Dim rawMessage As String

            Dim mJson As Object
            Dim mMaster As Object
            Dim mStocks As Object
            Dim dtHeader As New DataTable
            Dim dtStocks As New DataTable
            Dim IsSalesOrder As Boolean
            Dim dtTemp As DataTable
            Dim pDataReply As String

            Try

                mProData = Common.WebRequest.ProData

                rawMessage = Common.WebRequest.QueryString("Message")

                rawMessage = HttpUtility.UrlDecode(rawMessage)

                mMessage = Split(rawMessage, "|")

                'AppendToLog(rawMessage)

                If mMessage(0) = "CanISendSMSMessages" Then

                    'AppendToLog(mMessage(1))

                    pSql = "Select Distinct SMSID from MobizTransSMSTransactionMessages WHERE SenderNo = " & ReadyString(mMessage(1))

                    dtTemp = mProData.Execute(pSql)

                    pDataReply = GetMissingSMS(dtTemp)

                    'AppendToLog(pDataReply)

                    'If Not pDataReply = vbNullString Then
                    Return pDataReply
                    'End If
                End If

                If mMessage.Length > 0 Then

                    'Deviceno, randomNo, MaxMessageParts, TransactionType
                    ID = mMessage(0) & "|" & mMessage(1) & "|" & mMessage(2) & "|" & mMessage(3)

                    pSql = "Select * from MobizTransSMSTransactionMessages " & vbCrLf &
                            "where SMSID = " & ReadyString(ID) & vbCrLf &
                            " AND SMSMessagePosition = " & CInt2(mMessage(4))

                    dtTemp = mProData.Execute(pSql)

                    If dtTemp.Rows.Count > 0 Then
                        Exit Function
                    End If

                    pSql = "Insert into MobizTransSMSTransactionMessages(SMSID, SMSMessagePosition, SMSMessage, SenderNo) values(" &
                                ReadyString(ID) & "," &
                                CInt2(mMessage(4)) & "," &
                                ReadyString(mMessage(5)) & "," &
                                ReadyString(mMessage(6)) & ")"

                    mProData.ExecuteSQL(pSql)

                    'check if the number of expected sms count is equal to actual sms received in the server
                    pSql = "Select SMSMessage from MobizTransSMSTransactionMessages where SMSID = " &
                            ReadyString(ID)

                    dtMessage = mProData.Execute(pSql)

                    'mMessage(4) = sms count

                    If dtMessage.Rows.Count = mMessage(2) Then

                        pSql = "Select SMSMessage from MobizTransSMSTransactionMessages where SMSID = " &
                                ReadyString(ID) &
                                " AND SMSMessagePosition = 0"

                        Dim mOrgInfo As String = CStr2(mProData.ExecuteScalar(pSql))

                        mOrgID = mOrgInfo.Split(";")(0)
                        mDeviceID = mOrgInfo.Split(";")(1)
                        mDeviceUUID = mOrgInfo.Split(";")(2)

                        If ID.Split("|")(3) = "SO" Then
                            IsSalesOrder = True
                        Else
                            IsSalesOrder = False
                        End If

                        If mOrgID.Length > 0 Then

                            'pSql = "Select [database] from LibrarySubscription where OrganizationID = " & ReadyString(mOrgID)
                            'dbName = CStr2(mProData.ExecuteScalar(pSql))

                            'mProData.Execute("USE " & EscapeSqlName(dbName))

                            pSql = "Select SMSMessage from MobizTransSMSTransactionMessages where SMSID = " &
                                ReadyString(ID) & " order by SMSMessagePosition "
                            dtMessage = mProData.Execute(pSql)

                            'Compile messages and save to database

                            Dim pJson As String
                            Dim j As New JavaScriptSerializer()

                            For i As Integer = 1 To dtMessage.Rows.Count - 1
                                pJson &= CStr2(dtMessage.Rows(i).Item("SMSMessage"))
                            Next

                            'Creating from text file fo json

                            mJson = j.DeserializeObject(pJson)
                            mMaster = mJson("Master")
                            mStocks = mJson("Units")

                            'Creating table for invoice header

                            If Not mMaster Is Nothing Then

                                dtHeader.Columns.Add("ID")
                                dtHeader.Columns.Add("FowstID")
                                dtHeader.Columns.Add("CustomerID")
                                dtHeader.Columns.Add("InvoiceNumber")
                                dtHeader.Columns.Add("TotalAmount")
                                dtHeader.Columns.Add("DeliveryAddressID")
                                dtHeader.Columns.Add("Remarks")
                                dtHeader.Columns.Add("UserID")
                                dtHeader.Columns.Add("InvoiceDate")
                                dtHeader.Columns.Add("BillingAddressID")
                                dtHeader.Columns.Add("DeliveryDate")
                                dtHeader.Columns.Add("LastUpdated")
                                dtHeader.Columns.Add("DeviceID")

                                If IsSalesOrder Then
                                    dtHeader.Columns.Add("SalesOrderNo")
                                Else
                                    dtHeader.Columns.Add("PurchaseOrderNo")
                                End If

                                dtHeader.Columns.Add("SupplierID")
                                dtHeader.Columns.Add("SupplierFowstID")
                                dtHeader.Columns.Add("SupplierBinID")
                                dtHeader.Columns.Add("CustomerFowstID")
                                dtHeader.Columns.Add("CustomerBinID")
                                dtHeader.Columns.Add("SalesOrderDate")

                                Dim dr As DataRow

                                dr = dtHeader.NewRow
                                dr(0) = mMaster("ID")
                                dr(1) = mMaster("FowstID")
                                dr(2) = mMaster("CustomerID")
                                dr(3) = mMaster("InvoiceNumber")
                                dr(4) = mMaster("TotalAmount")
                                dr(5) = mMaster("DeliveryAddressID")
                                dr(6) = mMaster("Remarks")
                                dr(7) = mMaster("UserID")
                                dr(8) = mMaster("InvoiceDate")
                                dr(9) = mMaster("BillingAddressID")
                                dr(10) = mMaster("DeliveryDate")
                                dr(11) = mMaster("LastUpdated")
                                dr(12) = mDeviceID

                                If IsSalesOrder Then
                                    dr(13) = mMaster("SalesOrderNo")
                                Else
                                    dr(13) = mMaster("PurchaseOrderNo")
                                End If

                                If mMaster.ContainsKey("SupplierID") Then
                                    dr(14) = mMaster("SupplierID")
                                End If
                                If mMaster.ContainsKey("SupplierFowstID") Then
                                    dr(15) = mMaster("SupplierFowstID")
                                End If
                                If mMaster.ContainsKey("SupplierBinID") Then
                                    dr(16) = mMaster("SupplierBinID")
                                End If
                                If mMaster.ContainsKey("CustomerFowstID") Then
                                    dr(17) = mMaster("CustomerFowstID")
                                End If
                                If mMaster.ContainsKey("CustomerBinID") Then
                                    dr(18) = mMaster("CustomerBinID")
                                End If
                                If mMaster.ContainsKey("SalesOrderDate") Then
                                    dr(19) = mMaster("SalesOrderDate")
                                End If

                                dtHeader.Rows.Add(dr)

                                'Next pRow
                            End If

                            'Creating table for invoice items

                            If Not mStocks Is Nothing Then

                                dtStocks.Columns.Add("ID")
                                dtStocks.Columns.Add("ItemID")
                                dtStocks.Columns.Add("Quantity")
                                dtStocks.Columns.Add("InvoiceID")
                                dtStocks.Columns.Add("IsOrderSlipPrinted")
                                dtStocks.Columns.Add("StockID")
                                dtStocks.Columns.Add("ActualQuantity")
                                dtStocks.Columns.Add("UnitPrice")
                                dtStocks.Columns.Add("UnitMeasure")
                                dtStocks.Columns.Add("PriceLevelID")
                                dtStocks.Columns.Add("OrderedQuantity")
                                dtStocks.Columns.Add("SubTotal")
                                dtStocks.Columns.Add("LineNumber")
                                dtStocks.Columns.Add("ProductName")
                                dtStocks.Columns.Add("ProductNo")

                                For Each pRow As Object In mStocks("Rows")

                                    Dim dr As DataRow

                                    dr = dtStocks.NewRow
                                    dr(0) = pRow("ItemID")
                                    dr(1) = pRow("ID")
                                    dr(2) = pRow("Quantity")
                                    dr(3) = pRow("InvoiceID")
                                    dr(4) = pRow("IsOrderSlipPrinted")
                                    dr(5) = pRow("ProductID")
                                    dr(6) = pRow("ActualQuantity")
                                    dr(7) = pRow("UnitPrice")
                                    dr(8) = pRow("UnitMeasure")
                                    dr(9) = pRow("PriceLevelID")
                                    dr(10) = pRow("OrderedQuantity")
                                    dr(11) = pRow("SubTotal")
                                    dr(12) = pRow("LineNumber")

                                    dtStocks.Rows.Add(dr)

                                Next pRow
                            End If

                            'Creating info.json file

                            Dim infoPath As String
                            Dim pZipFilename As String
                            Dim pFileName As String

                            infoPath = Common.ServerPath & "temp\SMSFlushing\info.json"

                            If System.IO.File.Exists(infoPath) Then
                                System.IO.File.Delete(infoPath)
                            End If

                            Using sw As System.IO.StreamWriter = System.IO.File.CreateText(infoPath)

                                sw.WriteLine("{")
                                sw.WriteLine("""UUID"": """ & mDeviceUUID & """,")
                                sw.WriteLine("""AppBuildDate"": ""Jul 14, 2016 5:05 PM"",")
                                sw.WriteLine("""AppBuildDate"": ""Jul 14, 2016 5:05 PM"",")
                                sw.WriteLine("""AppVersion"": ""318"",")
                                sw.WriteLine("""OrgID"": """ & mOrgID & """")
                                sw.WriteLine("}")

                            End Using

                            'adding files to zip
                            pFileName = DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss") & ".zip"
                            pZipFilename = Common.ServerPath & "temp\SMSFlushing\" & pFileName

                            CreateZip(pZipFilename)
                            AddFileToZip(dtHeader, "TransInvoice") 'adding invoice header to zip
                            AddFileToZip(dtStocks, "TransInvoiceItems") 'adding invoice items to zip
                            mZip.AddFile(infoPath, "") 'adding .info file to zip

                            'sending zip file to mobiz server

                            Dim pValues As NameValueCollection
                            Dim pJson2 As JsonDictionary
                            Dim pResult As String

                            pJson2 = New JsonDictionary
                            pJson2.Add("Filename", IO.Path.GetFileName(pZipFilename))
                            pJson2.Add("OrgID", mOrgID)
                            pJson2.Add("DeviceID", mDeviceID)

                            pValues = New NameValueCollection
                            pValues.Add("json", pJson2.ToString())

                            pSql = "Update MobizTransSMSTransactionMessages set FileName = " & ReadyString(IO.Path.GetFileName(pZipFilename)) &
                            "where SMSID = " & ReadyString(ID) & " AND SMSMessagePosition = 0"

                            mProData.Execute(pSql)

#If DEBUG Then
                            pResult = UploadFile(pZipFilename, "http://localhost:8080/OfflineData/Dump", pValues)
#Else
                            'pResult = UploadFile(pZipFilename, "http://122.52.123.251/OfflineData/Dump", pValues)
                            pResult = UploadFile(pZipFilename, "http://localhost/OfflineData/Dump", pValues)
#End If

                            Dim mThread As Threading.Thread
                            mThread = New Threading.Thread(CType(
                                Sub()
                                    RequestFollowUp(ID, mOrgID, mDeviceID, mProData)
                                End Sub, ThreadStart))

                            mThread.Start()

                            Select Case mMessage(3)
                                Case "SO"
                            End Select
                        End If

                    End If
                End If
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

        End Function

        Private Sub RequestFollowUp(TransID As String, mOrgID As String, mDeviceID As String, mProData As ProData)
            Dim webClient As New System.Net.WebClient

            Dim pValues As NameValueCollection
            Dim pJson2 As JsonDictionary
            Dim pSql As String

            pJson2 = New JsonDictionary
            pJson2.Add("OrgID", mOrgID)
            pJson2.Add("DeviceID", mDeviceID)

            Dim mResult As String

#If DEBUG Then
            mResult = webClient.UploadString("http://localhost:8080/OfflineData/FollowUpImportData", "POST", pJson2.ToString)
#Else
            mResult = webClient.UploadString("http://122.52.123.251/OfflineData/FollowUpImportData", "POST",pJson2.ToString)
#End If



            If mResult.Contains("Error") Then
                Thread.Sleep(2000)
                RequestFollowUp(TransID, mOrgID, mDeviceID, mProData)
            ElseIf mResult.Contains("ok") Then
                pSql = "Update MobizTransSMSTransactionMessages set DateImported = GETDATE() " &
                        "where SMSID = " & ReadyString(TransID) & " AND SMSMessagePosition = 0"

                mProData.Execute(pSql)
            End If

        End Sub

        Private Sub CreateZip(mFilename As String)
            mZip = New ProZip(mFilename)
            mZip.Open(mFilename)
        End Sub

        Private Sub AddFileToZip(ByVal pDT As DataTable, ByVal TableName As String)

            Dim pFile As IO.FileInfo

            mZip.AddTableCsv(pDT, TableName)

        End Sub

        Public Function GetMissingSMS(dtSMSTransaction As DataTable) As String
            Dim currentTransaction As String
            Dim pSql As String
            Dim dtTemp As DataTable
            Dim pMaxRows As Integer
            Dim pStr() As String
            Dim pMissingMessageParts As String
            Dim IsFound As Boolean

            For i As Integer = 0 To dtSMSTransaction.Rows.Count - 1

                pSql = "Select * from TransSMSTransactionMessages WHERE SMSID = " & ReadyString(CStr2(dtSMSTransaction.Rows(i).Item("SMSID"))) & " ORDER BY SMSMessagePosition"
                dtTemp = mProData.Execute(pSql)

                If dtTemp.Rows.Count > 0 Then

                    'Deviceno, randomNo, MaxMessageParts, TransactionType, PartxOfMax
                    pStr = Split(CStr2(dtTemp.Rows(0).Item("SMSID")), "|")

                    For j As Integer = 1 To CInt2(pStr(2)) - 1
                        IsFound = False
                        For k As Integer = 0 To dtTemp.Rows.Count - 1
                            If j = CInt2(dtTemp.Rows(k).Item("SMSMessagePosition")) Then
                                IsFound = True
                                Exit For
                            End If
                        Next
                        If Not IsFound Then
                            pMissingMessageParts &= CStr2(dtTemp.Rows(0).Item("SMSID")) & "|" & j & ","
                        End If
                    Next
                End If
            Next

            If pMissingMessageParts = vbNullString Then
                'para hindi empty ang return
                pMissingMessageParts = "-"
            End If
            Return pMissingMessageParts
        End Function
    End Class

    Class Frown
        Public Rows As List(Of RowsClass)

        Public Class RowsClass

            Public ID As String
            Public TotalAmount As String
            Public DeliveryAddressID As String
            Public FowstID As String
            Public CustomerID As String
            Public OrgFowst As String
            Public CustomerName As String
            Public InvoiceNumber As String
            Public OrgName As String
            Public TinNumber As String
            Public Remarks As String
            Public UserID As String
            Public DeliveryAddress As String
            Public InvoiceDate As String
            Public BillingAddressID As String
            Public DeliveryDate As String
            Public LastUpdated As String

        End Class
    End Class

End Namespace