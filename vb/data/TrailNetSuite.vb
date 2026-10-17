Imports System.Net
Imports System.Linq
Imports Newtonsoft.Json

Public Class TrailNetSuite

    ReadOnly mProData As ProData
    ReadOnly mBusiness As Business
    Private mOrgId As String
    Const mNetSuiteErrorInvalidPim As String = "Contains invalid product code"
    Const mNetSuiteUrl As String = "https://3772489.restlets.api.netsuite.com/app/site/hosting/restlet.nl?script=270&deploy=1"

    Public Shared Function StartStatic()

        Dim p As TrailNetSuite
        Dim pProData As ProData

        pProData = ProPack.ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
        p = New TrailNetSuite(pProData)
#If DEBUG Then
        p.OrgID = modClient.AbcOrgID
#Else
                    p.OrgID = modClient.DarlingtonTrailOrgID
#End If
        p.Start()

        Return Nothing

        '        Static sSL As New Threading.SpinLock

        '        If sSL.IsHeld Then Return False ' app is already running
        '        TimedThread( _
        '            Sub()
        '                Try
        '                    If sSL.IsHeld Then Exit Sub
        '                    sSL.Enter(False)

        '                    pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
        '                    p = New TrailNetSuite(pProData)
        '#If DEBUG Then
        '                    p.OrgID = modClient.AbcOrgID
        '#Else
        '                    p.OrgID = modClient.DarlingtonTrailOrgID
        '#End If
        '                    p.Start()
        '                    Threading.Thread.Sleep(60 * 1000)
        '                Catch ex As Exception
        '                    Stop2()
        '                Finally
        '                    If sSL.IsHeldByCurrentThread Then
        '                        Try
        '                            'If mBusiness IsNot Nothing Then
        '                            '    mBusiness.ProData.Disconnect()
        '                            'End If
        '                        Catch ex As Exception
        '                            Stop2() ' just ignore this
        '                        End Try
        '                        sSL.Exit()
        '                    End If
        '                End Try
        '            End Sub)

    End Function

    Public Sub New(vProData As ProData)

        mProData = vProData
        mBusiness = New Business(mProData)

    End Sub

    Public Property OrgID As String
        Get
            Return mOrgId
        End Get
        Set(value As String)

            Dim pDR As DataRow
            Dim pDatabase As String

            pDR = mBusiness.LibraryOrganization.ByID(value)
            If pDR Is Nothing Then
                Throw New Exception("Org, " & value & ", does not exist. vb3Y5WRYVSA6!")
            End If
            pDatabase = CStr2(pDR("Database"))
            If String.IsNullOrWhiteSpace(pDatabase) Then
                Throw New Exception("Org, " & value & " does not have a database, vbKQWDGKE83A!")
            End If
            mProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))

            mOrgId = value

        End Set
    End Property

    Public ReadOnly Property ProData As ProData
        Get

            Return mProData

        End Get
    End Property

    Public Sub Start()

        Dim pTries As Integer

        Do
            Try
                GetCorrectableCustomerBins()
                'ExportTransfersToNetSuite()
                ExportPurchaseOrdersToNetSuite()
                ExportReceivingsToNetSuite()
                Return
            Catch ex As Exception
                AppendToLog(GetStackTrace(, ex), "", "TrailNetSuite")
                pTries += 1
                If pTries = 3 Then
                    Return
                End If
            End Try
        Loop

    End Sub

    Private Sub ExportTransfersToNetSuite()

        Dim pJsonStr As String
        Dim pUploadResult As String
        Dim pJson As JsonDictionary

        ' get transfers w/o NetSuiteNum
        Dim pSql As String
        Dim pDtTransfers As DataTable
        Dim pDtItems As DataTable
        Dim pIU As InsertUpdate
        Dim pHasValidProductNum As Boolean
        Dim pNetSuiteNum As Long?

        If ReadyGuid(mOrgId) <> ReadyGuid(modClient.AbcOrgID) _
            AndAlso ReadyGuid(mOrgId) <> ReadyGuid(DarlingtonTrailOrgID) Then

            Return
        End If

        pSql = "SELECT TOP 10 TransReceiving.ID" & vbCrLf &
            " ,TransReceiving.SupplierID" & vbCrLf &
            " ,TransReceiving.ReceivingPlaceID" & vbCrLf &
            ",tblFromFowst.Code AS FromCode" & vbCrLf &
            ",tblToFowst.Code AS ToCode" & vbCrLf &
            " FROM TransReceiving" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblFromFowst ON tblFromFowst.ID = TransReceiving.SupplierID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblToFowst ON tblToFowst.ID = TransReceiving.ReceivingPlaceID" & vbCrLf &
            " WHERE TransReceiving.NetSuiteNum IS NULL" & vbCrLf &
            " AND ISNULL(TransReceiving.NetSuiteError, '') <> " & ReadyString(mNetSuiteErrorInvalidPim) & vbCrLf &
            " AND TransReceiving.IsReceiving = 0" & vbCrLf &
            " ORDER BY TransReceiving.LastUpdated DESC"
        pDtTransfers = mBusiness.ProData.Execute(pSql)
        For Each pDrTransfer As DataRow In pDtTransfers.Rows
            Try
                pHasValidProductNum = False
                pJson = New JsonDictionary
                pJson.Add("recordtype", "transferorder")
                pJson.Add("location", CStr2(pDrTransfer("FromCode"))) ' example: 22
                pJson.Add("transferlocation", CStr2(pDrTransfer("ToCode"))) ' example: 20
                pDtItems = mBusiness.TransReceiving.GetItems(CStr2(pDrTransfer("ID")))
                With pJson.AddList("items")
                    For Each pDrItem As DataRow In pDtItems.Rows
                        If IsNumeric(pDrItem("ProductNo")) Then
                            pHasValidProductNum = True
                        Else
                            Continue For
                        End If
                        With .AddDictionary
                            .Add("item", CStr2(pDrItem("ProductNo")))
                            .Add("quantity", CDec2(pDrItem("QuantityGood")))
                        End With
                    Next pDrItem
                End With
                If pHasValidProductNum = False Then
                    pIU = New InsertUpdate("TransReceiving")
                    pIU("ProcessedForNetSuite") = "GETDATE()"
                    pIU("NetSuiteError") = ReadyString(mNetSuiteErrorInvalidPim)
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                    mBusiness.ProData.ExecuteSQL(pSql)
                    Continue For
                End If

                pJsonStr = pJson.ToString()
                Dim pWeb = CreateWebClient()
                pUploadResult = pWeb.UploadString(mNetSuiteUrl, "POST", pJsonStr)
                ' transfer order transaction number

                pIU = New InsertUpdate("TransReceiving")
                pIU("NetSuiteReponse") = ReadyString(pUploadResult)
                pNetSuiteNum = ParseReponse(pUploadResult)
                If pNetSuiteNum IsNot Nothing Then
                    pIU("NetSuiteNum") = ReadyString(pNetSuiteNum)
                End If
                pIU("ProcessedForNetSuite") = "GETDATE()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                mBusiness.ProData.ExecuteSQL(pSql)
            Catch ex As Exception
                Try
                    pIU = New InsertUpdate("TransReceiving")
                    pIU("ProcessedForNetSuite") = "GETDATE()"
                    pIU("NetSuiteError") = ReadyString(Truncate(ex.Message, 2000))
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                    mBusiness.ProData.ExecuteSQL(pSql)
                Catch ex2 As Exception
                    Stop2()
                End Try
            End Try
        Next pDrTransfer

    End Sub

    Private Sub ExportPurchaseOrdersToNetSuite()

        Dim pJsonStr As String
        Dim pUploadResult As String
        Dim pPoJson As PoJson

        ' get transfers w/o NetSuiteNum
        Dim pSql As String
        Dim pDtInvoices As DataTable
        Dim pDtItems As DataTable
        Dim pIU As InsertUpdate
        Dim pHasValidProductNum As Boolean
        Dim pNetSuiteNum As Long?
        Dim pItemJson As PoJson.ItemJson

        If ReadyGuid(mOrgId) <> ReadyGuid(modClient.AbcOrgID) _
            AndAlso ReadyGuid(mOrgId) <> ReadyGuid(DarlingtonTrailOrgID) Then

            Return
        End If

        pSql = "SELECT TOP 10 TransInvoice.ID" & vbCrLf &
            " ,TransInvoice.SupplierID" & vbCrLf &
            " ,TransInvoice.SupplierFowstID" & vbCrLf &
            " ,TransInvoice.Remarks" & vbCrLf &
            ",tblCustomerOrg.NetSuiteID AS CustomerOrgNetSuiteID" & vbCrLf &
            ",LibraryOrganizationCustomer.ThirdPartyID AS CustomerOrgThirdPartyID" & vbCrLf &
            ",tblToFowst.NetSuiteID AS CustomerFowstNetSuiteID" & vbCrLf &
            ",tblCustomerBin.NetSuiteID AS CustomerBinNetSuiteID" & vbCrLf &
            ",tblFromFowst.NetSuiteID AS FromFowstNetSuiteID" & vbCrLf &
            ",tblSupplierFowst.NetSuiteID AS SupplierFowstNetSuiteID" & vbCrLf &
            "      ,ISNULL(LibraryDocumentControl.DocumentName, CASE" & vbCrLf &
            "      	WHEN ISNULL(TransInvoice.InvoiceNumber, 0) > 0 THEN 'SI'" & vbCrLf &
            "         WHEN ISNULL(TransInvoice.SalesOrderNo, 0) > 0 THEN 'SO'" & vbCrLf &
            "         WHEN ISNULL(TransInvoice.PurchaseOrderNo, 0) > 0 THEN 'PO'" & vbCrLf &
            "         ELSE ''" & vbCrLf &
            "         END) + '-' +" & vbCrLf &
            "      	+ ISNULL(CONVERT(varchar, tblFromFowst.FowstNo), '') + '-'" & vbCrLf &
            "         + ISNULL(CONVERT(varchar, LibraryDevice.DeviceNo), '') + '-'" & vbCrLf &
            "         + CONVERT(varchar, " & vbCrLf &
            " CASE" & vbCrLf &
            "       	 WHEN ISNULL(TransInvoice.InvoiceNumber, 0) > 0 THEN TransInvoice.InvoiceNumber" & vbCrLf &
            "          WHEN ISNULL(TransInvoice.SalesOrderNo, 0) > 0 THEN TransInvoice.SalesOrderNo" & vbCrLf &
            "          WHEN ISNULL(TransInvoice.PurchaseOrderNo, 0) > 0 THEN TransInvoice.PurchaseOrderNo" & vbCrLf &
            "          ELSE 0" & vbCrLf &
            "          END" & vbCrLf &
            ") AS InvoiceNumber" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            "       LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblFromFowst ON tblFromFowst.ID = TransInvoice.FowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblSupplierFowst ON tblSupplierFowst.ID = TransInvoice.SupplierFowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganizationFOWST tblToFowst ON tblToFowst.ID = TransInvoice.CustomerFowstID" & vbCrLf &
            " LEFT JOIN MobizLibraryOrganization tblCustomerOrg ON tblCustomerOrg.ID = TransInvoice.CustomerID" & vbCrLf &
            " LEFT JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = TransInvoice.CustomerID" & vbCrLf &
            " LEFT JOIN LibraryFowstBin tblCustomerBin ON tblCustomerBin.ID = TransInvoice.CustomerBinID" & vbCrLf &
            "     LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID" & vbCrLf &
            " WHERE TransInvoice.NetSuiteNum IS NULL" & vbCrLf &
            " AND TransInvoice.SopaApprovedByID IS NOT NULL" & vbCrLf &
            " AND ISNULL(TransInvoice.NetSuiteError, '') <> " & ReadyString(mNetSuiteErrorInvalidPim) & vbCrLf &
            " AND ISNULL(TransInvoice.NetSuiteReponse, '') NOT LIKE '""INVALID_%'" & vbCrLf &
            " AND ISNULL(TransInvoice.NetSuiteReponse, '') NOT LIKE '""EXCEEDED%'" & vbCrLf &
            " AND ISNULL(TransInvoice.NetSuiteReponse, '') NOT LIKE '%Please enter value%'" & vbCrLf &
            " AND (ISNULL(TransInvoice.PurchaseOrderNo, '') <> ''" & vbCrLf &
            " OR ISNULL(TransInvoice.SalesOrderNo, 0) <> 0" & vbCrLf &
            " ) AND TransInvoice.ID IN (" & vbCrLf &
            "   SELECT InvoiceID " & vbCrLf &
            "   FROM TransInvoiceItems" & vbCrLf &
            "   WHERE TransInvoiceItems.Quantity > 0" & vbCrLf &
            ")" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        pDtInvoices = mBusiness.ProData.Execute(pSql)
        If Now.Date = New Date(2020, 7, 2) Then
            AppendToLog("ExportPurchaseOrdersToNetSuite vbT32L4YFQ6E" & vbCrLf &
                "count: " & pDtInvoices.Rows.Count & vbCrLf &
                pSql, , "ExportPurchaseOrdersToNetSuite vbT32L4YFQ6E")
        End If
        For Each pDrTransfer As DataRow In pDtInvoices.Rows
            Try
                pHasValidProductNum = False
                pPoJson = New PoJson()
                pPoJson.recordtype = "customrecord_custom_to"
                If CStr2(pDrTransfer("CustomerOrgThirdPartyID")).Trim().Length > 0 Then
                    pPoJson.custrecord_custom_to_customer = CStr2(pDrTransfer("CustomerOrgThirdPartyID"))
                Else
                    pPoJson.custrecord_custom_to_customer = CStr2(pDrTransfer("CustomerOrgNetSuiteID"))
                End If
                pPoJson.custrecord_custom_to_fromlocation = CStr2(pDrTransfer("FromFowstNetSuiteID"))
                pPoJson.custrecord_custom_to_tolocation = CStr2(pDrTransfer("SupplierFowstNetSuiteID"))
                pPoJson.custrecord_custom_to_brand = 25 ' todo
                pPoJson.custrecord_custom_to_storeoutlet = CStr2(pDrTransfer("CustomerFowstNetSuiteID"))
                pPoJson.custrecord_custom_to_custdept = CStr2(pDrTransfer("CustomerBinNetSuiteID"))
                pPoJson.custrecord_custom_to_memo = CStr2(pDrTransfer("Remarks"))
                pPoJson.custrecordcustrecord_custom_to_po = CStr2(pDrTransfer("InvoiceNumber"))
                pDtItems = mBusiness.TransInvoice.GetItems(CStr2(pDrTransfer("ID")), True)
                For Each pDrItem As DataRow In pDtItems.Rows
                    If IsNumeric(pDrItem("ProductNo")) Then
                        pHasValidProductNum = True
                    Else
                        Continue For
                    End If

                    pItemJson = New PoJson.ItemJson()
                    pItemJson.item = pDrItem("ProductNo")
#If DEBUG Then
                    pItemJson.quantity = 0
#Else
                    pItemJson.quantity = CInt2(pDrItem("Quantity"))
#End If

                    pPoJson.items.Add(pItemJson)
                Next pDrItem

                If pHasValidProductNum = False Then
                    pIU = New InsertUpdate("TransInvoice")
                    pIU("ProcessedForNetSuite") = "GETDATE()"
                    pIU("NetSuiteError") = ReadyString(mNetSuiteErrorInvalidPim)
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                    mBusiness.ProData.ExecuteSQL(pSql)
                    Continue For
                End If

                'pJsonStr = JsonConvert.SerializeObject(pPoJson, New DecimalFormatConverter())
                pJsonStr = JsonConvert.SerializeObject(pPoJson, Formatting.Indented)
                Dim pWeb = CreateWebClient()
                pUploadResult = pWeb.UploadString(mNetSuiteUrl, "POST", pJsonStr)
                ' transfer order transaction number

                pIU = New InsertUpdate("TransInvoice")
                pIU("NetSuiteReponse") = ReadyString(pUploadResult)
                pNetSuiteNum = ParseReponse(pUploadResult)
                If pNetSuiteNum IsNot Nothing Then
                    pIU("NetSuiteNum") = ReadyString(pNetSuiteNum)
                Else
                    AppendToLog("pUploadResult: " & pUploadResult & vbCrLf &
                        mBusiness.TransInvoice.GetTransactionInfo(CStr2(pDrTransfer("ID"))) &
                        "Current Date: " & Now() & vbCrLf &
                        pJsonStr, , "ExportPurchaseOrdersToNetSuite vbPJ48429WA7")
                End If
                pIU("ProcessedForNetSuite") = "GETDATE()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                mBusiness.ProData.ExecuteSQL(pSql)
            Catch ex As Exception
                Try
                    pIU = New InsertUpdate("TransInvoice")
                    pIU("ProcessedForNetSuite") = "GETDATE()"
                    pIU("NetSuiteError") = ReadyString(Truncate(ex.Message, 2000))
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                    mBusiness.ProData.ExecuteSQL(pSql)

                    AppendToLog("pUploadResult: " & pUploadResult & vbCrLf &
                        mBusiness.TransInvoice.GetTransactionInfo(CStr2(pDrTransfer("ID"))) &
                        "Current Date: " & Now() & vbCrLf &
                        pJsonStr, , "ExportPurchaseOrdersToNetSuite vbPJ48429WA7")
                Catch ex2 As Exception
                    Stop2()
                End Try
            End Try
        Next pDrTransfer

    End Sub


    Public Class DecimalFormatConverter
        ' http://stackoverflow.com/a/17872006/334045
        Inherits JsonConverter

        Public Overrides Function CanConvert(objectType As Type) As Boolean

            Return objectType.IsAssignableFrom(GetType(Decimal))

        End Function

        Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object

            Throw New NotImplementedException()

        End Function

        Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)

            writer.WriteValue(String.Format("{0:N2}", value))

        End Sub
    End Class

    Public Class PoJson
        Public recordtype As String
        Public custrecord_custom_to_customer As String
        Public custrecord_custom_to_fromlocation As String
        Public custrecord_custom_to_tolocation As String
        Public custrecord_custom_to_brand As String
        Public custrecord_custom_to_storeoutlet As String
        Public custrecord_custom_to_custdept As String
        Public items As New List(Of ItemJson)
        Public custrecord_custom_to_memo As String
        Public custrecordcustrecord_custom_to_po As String

        Public Class ItemJson
            Public item As String
            Public quantity As String
        End Class
    End Class

    Private Sub ExportReceivingsToNetSuite()

        Dim pJsonStr As String
        Dim pUploadResult As String
        Dim pJson As JsonDictionary
        Dim pTransferNetSuiteNum As String

        ' get receivings w/o NetSuiteNum
        Dim pSql As String
        Dim pDtReceivings As DataTable
        Dim pDtItems As DataTable
        Dim pIU As InsertUpdate
        Dim pHasValidProductNum As Boolean
        Dim pNetSuiteNum As Long?

        If ReadyGuid(mOrgId) <> ReadyGuid(modClient.AbcOrgID) _
            AndAlso ReadyGuid(mOrgId) <> ReadyGuid(DarlingtonTrailOrgID) Then

            Return
        End If

        pSql = " SELECT TOP 10 TransReceiving.ID" & vbCrLf &
          " ,tblTransfers.NetSuiteNum" & vbCrLf &
          " FROM TransReceiving" & vbCrLf &
          " JOIN TransReceiving tblTransfers ON tblTransfers.ID = TransReceiving.TransferOutToReceivingID" & vbCrLf &
          " WHERE TransReceiving.NetSuiteNum IS NULL" & vbCrLf &
          " AND tblTransfers.NetSuiteNum IS NOT NULL" & vbCrLf &
          " AND ISNULL(TransReceiving.NetSuiteError, '') <> " & ReadyString(mNetSuiteErrorInvalidPim) & vbCrLf &
          " AND TransReceiving.IsReceiving = 1" & vbCrLf &
          " ORDER BY TransReceiving.LastUpdated DESC"
        pDtReceivings = mBusiness.ProData.Execute(pSql)
        For Each pDrTransfer As DataRow In pDtReceivings.Rows
            Try
                pHasValidProductNum = False
                pJson = New JsonDictionary
                pJson.Add("recordtype", "itemreceipt")
                pTransferNetSuiteNum = CStr2((pDrTransfer("NetSuiteNum")))
                pTransferNetSuiteNum = Trim2(pTransferNetSuiteNum, """")
                If IsNumeric(pTransferNetSuiteNum) Then
                    pJson.Add("transferorder", CDec2(pTransferNetSuiteNum))
                End If

                ' these are not present in the spec
                'pJson.Add("location", "22")
                'pJson.Add("transferlocation", "20")

                pDtItems = mBusiness.TransReceiving.GetItems(CStr2(pDrTransfer("ID")))
                With pJson.AddList("items")
                    For Each pDrItem As DataRow In pDtItems.Rows
                        If IsNumeric(pDrItem("ProductNo")) Then
                            pHasValidProductNum = True
                        Else
                            Continue For
                        End If
                        With .AddDictionary
                            .Add("item", CStr2(pDrItem("ProductNo")))
#If DEBUG Then
                            .Add("quantity", 0)
#Else
                            .Add("quantity", CDec2(pDrItem("QuantityGood")))
#End If
                        End With
                    Next pDrItem
                End With
                If pHasValidProductNum = False Then
                    pIU = New InsertUpdate("TransReceiving")
                    pIU("ProcessedForNetSuite") = "GETDATE()"
                    pIU("NetSuiteError") = ReadyString(mNetSuiteErrorInvalidPim)
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                    mBusiness.ProData.ExecuteSQL(pSql)
                    Continue For
                End If

                pJsonStr = pJson.ToString()
                Dim pWeb = CreateWebClient()
                pUploadResult = pWeb.UploadString(mNetSuiteUrl, "POST", pJsonStr)
                pIU = New InsertUpdate("TransReceiving")
                pIU("NetSuiteReponse") = ReadyString(pUploadResult)
                pNetSuiteNum = ParseReponse(pUploadResult)
                If pNetSuiteNum IsNot Nothing Then
                    pIU("NetSuiteNum") = ReadyString(pNetSuiteNum)
                End If
                pIU("ProcessedForNetSuite") = "GETDATE()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                mBusiness.ProData.ExecuteSQL(pSql)
            Catch ex As Exception
                Try
                    pIU = New InsertUpdate("TransReceiving")
                    pIU("ProcessedForNetSuite") = "GETDATE()"
                    pIU("NetSuiteError") = ReadyString(Truncate(ex.Message, 2000))
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pDrTransfer("ID")))
                    mBusiness.ProData.ExecuteSQL(pSql)
                Catch ex2 As Exception
                    Stop2()
                End Try
            End Try

        Next pDrTransfer

    End Sub

    Public Class PatientWebClient
        ' https://stackoverflow.com/a/6994391/334045
        Inherits WebClient

        Protected Overrides Function GetWebRequest(address As Uri) As Net.WebRequest

            Dim pWR = MyBase.GetWebRequest(address)
            pWR.Timeout = 500 * 1000

            Return MyBase.GetWebRequest(address)

        End Function
    End Class

    Function CreateWebClient() As WebClient

        'Const pAccount As String = "3772489"
        'Const pEmail As String = "customercare@mobiz-apps.com"
        'Const pSignature As String = "Netsuite123"

        Const pAccount As String = "3772489"
        'Const pEmail As String = "dowi.sales@gmail.com"
        'Const pSignature As String = "MobizNetsuite123"
        Const pEmail As String = "darlington.warehouse@gmail.com"
        Const pSignature As String = "Netsuite123"
        Const pRole As String = "1049"

        Dim pHeader As String

        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls + System.Net.SecurityProtocolType.Tls11 + System.Net.SecurityProtocolType.Tls12
        'pHeader = "NLAuth nlauth_account=" + pAccount + ", nlauth_email=" + pEmail + ", nlauth_signature=" + pSignature + ", nlauth_role=" + pRole

        ' 2019_10_24__09_17_50_6789
        'pHeader = "NLAuth nlauth_account=3772489, nlauth_email=dowi.warehouse@gmail.com, nlauth_signature=Netsuite123, nlauth_role=1053, nlauth_application_id=3D9CE5A3-5C08-44BF-AE86-6E7F8A2D2857"
        pHeader = "NLAuth nlauth_account=3772489, nlauth_email=dowi.warehouse@gmail.com, nlauth_signature=Netsuite123, nlauth_role=1053"

        Dim pWeb = New WebClient()
        'Dim pWeb = New WebClient()
        pWeb.Headers.Add("Authorization", pHeader)
        pWeb.Headers.Add("Content-Type", "application/json")
        pWeb.Headers.Add("User-Agent-x", "SuiteScript Call")

        Return pWeb

    End Function

    Function ParseReponse(vResponse As String) As Long?

        Dim pOut As Long

        vResponse = Trim2(vResponse, """")
        If Long.TryParse(vResponse, pOut) Then
            Return pOut
        End If

    End Function

    Public Function ImportLibraryFromExcel()

        Dim pDtOrg As DataTable
        Dim pDtFowst As DataTable
        Dim pDtBin As DataTable
        Dim pFilename As String
        Dim pError As String
        Dim pOrgID As String
        Dim pParentCustomer As String
        ' Dim p As IQueryable(Of DataRow)
        Dim pFowstID As String
        Dim pFowstInternalID As String

        pFilename = Common.ServerPath & "data\from NetSuite Customers.xlsx"
        pDtOrg = ExcelToTable(pFilename, {"Name", "Internal ID"}, pError)

        pFilename = Common.ServerPath & "data\from NetSuite Customer Stores.xlsx"
        pDtFowst = ExcelToTable(pFilename, {"Name", "Internal ID", "Parent Customer"}, pError)

        pFilename = Common.ServerPath & "data\from NetSuite Store Departments.xlsx"
        pDtBin = ExcelToTable(pFilename, {"Name", "Internal ID", "Parent Store Internal ID"}, pError)

        mProData.BeginTransaction()
        For Each pDrFowst As DataRow In pDtFowst.Rows
            If pDrFowst("Spreadsheet Row Number") = 223 Then
                Stop2()
            End If
            If String.IsNullOrWhiteSpace(pDrFowst("Internal ID")) Then
                Continue For
            End If
            If String.IsNullOrWhiteSpace(pDrFowst("Name")) Then
                Continue For
            End If
            If String.IsNullOrWhiteSpace(pDrFowst("Parent Customer")) Then
                Continue For
            End If
            pParentCustomer = pDrFowst("Parent Customer")

            Dim p = (From pRow In pDtOrg.Rows
                Where CStr2(pRow("Name")) = pParentCustomer
                Select pRow("Internal ID")).FirstOrDefault()
            If String.IsNullOrWhiteSpace(p) Then
                Debug.Print("vbP9X8ZKGPPM! parent customer, " & pParentCustomer & ", does not exist in Customer.xlsx")
                Continue For
            End If
            pOrgID = mBusiness.LibraryOrganization.GetOrAddByCompanyName(
                pDrFowst("Parent Customer"),
                p, True, False)

            pFowstID = mBusiness.LibraryOrganizationFOWST.CreateFromNetSuite(
                pDrFowst("Name"),
                pOrgID,
                CStr2(pDrFowst("Internal ID")))

#If DEBUG Then
            If CStr2(pDrFowst("Internal ID")) = "280" OrElse CStr2(pDrFowst("Internal ID")) = "217" Then
                Debug.WriteLine(CStr2(pDrFowst("Internal ID")))
            End If
#End If

            pFowstInternalID = CStr2(pDrFowst("Internal ID"))
            Dim pBin = From pRow In pDtBin.Rows
                Where CStr2(pRow("Parent Store Internal ID")) = pFowstInternalID
                Select pRow

            For Each pDrBin As DataRow In pBin
                mBusiness.LibraryOrganizationFOWST.GetOrCreateBin(
                    CStr2(pDrBin("Name")),
                    pFowstID,
                    CStr2(pDrBin("Internal ID")))
            Next pDrBin

        Next pDrFowst
        mBusiness.SystemDefaults.UpdateTableLastUpdated("LibraryFowstBin")
        mProData.CommitTransaction()

    End Function

    Private Function GetCorrectableCustomerBins()

        Dim pSql = " -- CorrectCustomerBinID" & vbCrLf &
            " SELECT TransInvoice.ID" & vbCrLf &
            " ,TransInvoice.LastUpdated" & vbCrLf &
            " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf &
            " ,TransInvoice.CustomerFowstID" & vbCrLf &
            " ,TransInvoice.CustomerBinID" & vbCrLf &
            " ,(SELECT TOP 1 tblBin.ID" & vbCrLf &
            " 	FROM LibraryFowstBin tblBin" & vbCrLf &
            "     WHERE tblBin.Name = LibraryFowstBin.Name" & vbCrLf &
            "     AND tblBin.FowstID = TransInvoice.CustomerFowstID" & vbCrLf &
            " ) AS CorrectCustomerBinID" & vbCrLf &
            " ,(SELECT TOP 1 tblBin.NetSuiteID" & vbCrLf &
            " 	FROM LibraryFowstBin tblBin" & vbCrLf &
            "     WHERE tblBin.Name = LibraryFowstBin.Name" & vbCrLf &
            "     AND tblBin.FowstID = TransInvoice.CustomerFowstID" & vbCrLf &
            " ) AS CorrectNetSuiteID" & vbCrLf &
            " ,(SELECT TOP 1 tblBin.Name" & vbCrLf &
            " 	FROM LibraryFowstBin tblBin" & vbCrLf &
            "     WHERE tblBin.Name = LibraryFowstBin.Name" & vbCrLf &
            "     AND tblBin.FowstID = TransInvoice.CustomerFowstID" & vbCrLf &
            " ) AS CorrectBinName" & vbCrLf &
            " ,LibraryFowstBin.NetSuiteID AS WrongNetSuiteID" & vbCrLf &
            " ,LibraryFowstBin.Name AS WrongBinName" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = CustomerID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.CustomerFowstID" & vbCrLf &
            " JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInvoice.CustomerBinID" & vbCrLf &
            " WHERE TransInvoice.CustomerFowstID IS NOT NULL" & vbCrLf &
            " AND TransInvoice.CustomerBinID IS NOT NULL" & vbCrLf &
            " AND NOT EXISTS (" & vbCrLf &
            " 	SELECT *" & vbCrLf &
            "     FROM LibraryFowstBin" & vbCrLf &
            "     WHERE LibraryFowstBin.ID = TransInvoice.CustomerBinID" & vbCrLf &
            "     AND LibraryFowstBin.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            " )" & vbCrLf &
            " AND EXISTS (" & vbCrLf &
            " 	SELECT tblBin.ID" & vbCrLf &
            " 	FROM LibraryFowstBin tblBin" & vbCrLf &
            "     WHERE tblBin.Name = LibraryFowstBin.Name" & vbCrLf &
            "     AND tblBin.FowstID = TransInvoice.CustomerFowstID" & vbCrLf &
            " )" & vbCrLf &
            " AND TransInvoice.NetSuiteNum IS NULL" & vbCrLf &
            " AND (TransInvoice.NetSuiteError IS NOT NULL" & vbCrLf &
            " 	OR TransInvoice.NetSuiteReponse IS NOT NULL)" & vbCrLf &
            " AND TransInvoice.LastUpdated > '2018-03-01'" & vbCrLf &
            " ORDER BY TransInvoice.LastUpdated DESC"
        Dim pDT = mProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("TransInvoice")
            pIU("CustomerBinID") = ReadyString(pDR("CorrectCustomerBinID"))
            pIU("NetSuiteError") = "NULL"
            pIU("NetSuiteReponse") = "NULL"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            AppendToLog("vbGYLQYMW4E7!" & vbCrLf & pSql, , "GetCorrectableCustomerBins vbGYLQYMW4E7")
            Try
                pSql = mProData.ExecuteSQL(pSql) ' output is only used for debugging
                AppendToLog("vbDYLM7T7GXM! affected records: " & pSql, , "GetCorrectableCustomerBins vbGYLQYMW4E7")
            Catch ex As Exception
                AppendToLog("vbH9HLBVAN39!" & vbCrLf & ex.Message, , "GetCorrectableCustomerBins vbGYLQYMW4E7")

            End Try
        Next pDR

    End Function
End Class
