Imports System.Security.Cryptography
Imports System.Net

Public Class MallLessorPITX

    ReadOnly mProData As ProData

    Sub ConnectToDatabase(dbName As String)
        'mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
        mProData.Execute("USE " & EscapeSqlName(dbName))
    End Sub

    Sub New(Optional ByRef rProData As ProData = Nothing)
        mProData = rProData
    End Sub

    Private Function IsRunPITXDataSender() As Boolean

        Dim pFilename = Common.ServerPath & "settings\IsRunPITXDataSender.txt"
        Dim pText = CStr2(FileText(pFilename))
        Dim pOut = CBool2(pText)
        FileText(pFilename) = pOut

        Return pOut

    End Function

    Function StartSendingTransactionToLessor() As String

        Dim pSql As String
        Dim pDT As DataTable
        Dim pJsonString As String

        Dim PDTMainTable As New DataTable
        Dim submission_uuid As String
        Dim tenant_id As Integer = 95
        Dim terminal_id As Integer = 102
        Dim submission_timestamp As String
        Dim transaction_count As Integer = 1
        Dim payload_checksum As String
        Dim pBytes() As Byte

        If IsRunPITXDataSender() = False Then
            Exit Function
        End If

        PDTMainTable.Columns.Add("submission_uuid")
        PDTMainTable.Columns.Add("tenant_id")
        PDTMainTable.Columns.Add("terminal_id")
        PDTMainTable.Columns.Add("submission_timestamp")
        PDTMainTable.Columns.Add("transaction_count")
        PDTMainTable.Columns.Add("payload_checksum")

        'temporary, dapat guid ni sya sa transaction
        PDTMainTable.Columns.Add("transaction_id")

        While True

            '------------ Transaction-----------------
            PDTMainTable.Rows.Clear()

#If DEBUG Then
            pSql = "Select top 1 * from CacheInvoices "
#Else
        'pSql = "Select top 1 * from CacheInvoices where FOWSTID = 'b96ca9a9-b1af-4fbc-aa56-e6e2e58f7729' AND IsSentToLessor = 0 AND convert(date,invoicedate) >= convert(date,'2026-05-24') AND SCDiscount>0 ORDER BY invoicedate "
        pSql = "Select top 1 * from CacheInvoices where FOWSTID = 'b96ca9a9-b1af-4fbc-aa56-e6e2e58f7729' AND IsSentToLessor = 0 AND convert(date,invoicedate) >= convert(date,'2026-09-29') ORDER BY invoicedate "
#End If

            pDT = mProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then

                pDT.Columns.Add("payload_checksum")

                submission_uuid = Guid.NewGuid.ToString
                submission_timestamp = Date.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss'Z'")

                Dim pDR As DataRow

                pDR = PDTMainTable.NewRow

                pDR(0) = submission_uuid
                pDR(1) = tenant_id
                pDR(2) = terminal_id
                pDR(3) = submission_timestamp
                pDR(4) = transaction_count

                PDTMainTable.Rows.Add(pDR)

                pJsonString = CreateJSONPayload(PDTMainTable, pJsonString, pDT)

                ''------------ Transaction -----------------

                ''------------ debugging-----------------
                'AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", pJsonString)

                'Return ""

                ''------------ Sending -----------------

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                Try
                    'Dim mRequestPresignedUrl As String = "https://stagingtsms.pitx.com.ph/api/v1/transactions/official"
                    Dim mRequestPresignedUrl As String = "https://salesportal.pitx.com.ph/api/v1/transactions/official"
                    Dim pWeb = New WebClient()
                    Dim pUploadResult As String

                    pWeb.Headers.Add("Authorization", "Bearer 438|3ZQBM33wxeJNqK6DoTi613QCe85OwnvhTDqydtrmd76058b6")
                    pWeb.Headers.Add("Content-Type", "application/json")

                    pUploadResult = pWeb.UploadString(mRequestPresignedUrl, "POST", pJsonString)

                    AppendToFile(Common.ServerPath & "temp\logs\PITX.txt", Now.ToString & " -Success- " & pUploadResult & " -JSONString- " & pJsonString)

                    If pUploadResult.Contains("") Then
                        pSql = "Update CacheInvoices set IsSentToLessor = 1 WHERE ID = " & ReadyString(CStr2(pDT.Rows(0).Item("ID")))
                        mProData.Execute(pSql)
                    End If

                    Dim pDirectory As String

                    pDirectory = Common.ServerPath & "temp\PITX\"

                    If Not System.IO.Directory.Exists(pDirectory) Then
                        IO.Directory.CreateDirectory(pDirectory)
                    End If

                    pDirectory = Common.ServerPath & "temp\PITX\" & CDate(pDT.Rows(0).Item("InvoiceDate")).ToString("yyyy-MM-dd")

                    If Not System.IO.Directory.Exists(pDirectory) Then
                        IO.Directory.CreateDirectory(pDirectory)
                    End If

                    AppendToFile(pDirectory & "\" & CStr2(pDT.Rows(0).Item("InvoiceNumber")) & ".txt", pJsonString)

                Catch ex As Exception
                    AppendToLog(ex.Message)
                    AppendToFile(Common.ServerPath & "temp\logs\PITX.txt", Now.ToString & " -Error- " & ex.Message & " -JSONString- " & pJsonString)
                End Try

                'sleep for 3 seconds
                Threading.Thread.Sleep(3000)
            Else
                'no available data
                'sleep for 1 hour
                Threading.Thread.Sleep(3600000)
            End If
        End While

    End Function

    Private Function CreateJSONPayload(PDTMainTable As DataTable, pJsonStringTransaction As String, PDTTransaction As DataTable) As String
        Dim jFinalString As String
        Dim pJsonHeader As JsonDictionary
        Dim submission_uuid As String
        Dim tenant_id As Integer
        Dim terminal_id As Integer
        Dim submission_timestamp As String
        Dim transaction_count As Integer = 1
        Dim payload_checksum As String

        Dim promo_discount_amount As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim senior_discount As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim pwd_discount As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim vip_card_discount As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim service_charge_distributed_to_employees As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim service_charge_retained_by_management As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim employee_discount As String = CStr2(CDec2(0.0).ToString("######0.00"))

        Dim VAT As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim VATABLE_SALES As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim SC_VAT_EXEMPT_SALES As String = CStr2(CDec2(0.0).ToString("######0.00"))
        Dim OTHER_TAX As String = CStr2(CDec2(0.0).ToString("######0.00"))

        Dim transaction_id As String
        Dim hardware_id As String = "TA04930027"
        Dim receipt_no As String = ""
        Dim transaction_timestamp As String '= "2026-05-14T08:59:28Z" 'Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'")
        Dim gross_sales As String = "0"
        Dim net_sales As String = "0"
        Dim promo_status As String = "WITHOUT_APPROVAL"
        Dim customer_code As String = "C-S1011"
        Dim payload_transaction_checksum As String = ""
        Dim pJsonForTransactionPayload As JsonDictionary
        Dim pJsonForHeaderPayload As JsonDictionary
        Dim SCDiscount As Double = 0

        pJsonForTransactionPayload = New JsonDictionary
        pJsonForHeaderPayload = New JsonDictionary

        pJsonHeader = New JsonDictionary

        '=======================Header================================

        submission_uuid = CStr2(PDTMainTable.Rows(0).Item("submission_uuid"))
        tenant_id = CInt2(PDTMainTable.Rows(0).Item("tenant_id"))
        terminal_id = CInt2(PDTMainTable.Rows(0).Item("terminal_id"))
        submission_timestamp = CStr2(PDTMainTable.Rows(0).Item("submission_timestamp"))
        transaction_count = CInt2(PDTMainTable.Rows(0).Item("transaction_count"))
        transaction_id = CStr2(PDTTransaction.Rows(0).Item("ID")) '"f9c5dd6a-2d71-40b8-903c-df0a02b417ed" ' CStr2(PDTMainTable.Rows(0).Item("transaction_id"))
        transaction_timestamp = CStr2(CDate(PDTTransaction.Rows(0).Item("InvoiceDate")).ToUniversalTime.ToString("yyyy-MM-ddTHH:mm:ss'Z'"))

        SCDiscount = CDec2(PDTTransaction.Rows(0).Item("SCDiscount"))
        gross_sales = CDec2(PDTTransaction.Rows(0).Item("GrossSalesNetOfVATExemptions")).ToString("######0.00")
        receipt_no = CStr2(PDTTransaction.Rows(0).Item("InvoiceNumber"))

        '======================= discount ================================
        If SCDiscount > 0 Then
            Dim pSQLDiscount As String
            Dim SCExemptSales As Double = 0
            Dim SKUID As String
            Dim ItemBasedSKUID As String

            'retail price id
            'product id (Jumbo Fries)
            'Franchisee Org ID

            'Get SKU
            pSQLDiscount = "Select top 1 ProductID from TransInvoiceItems where invoiceid = " & ReadyString(CStr2(PDTTransaction.Rows(0).Item("ID")))
            SKUID = CStr2(mProData.ExecuteScalar(pSQLDiscount))

            AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", pSQLDiscount)
            AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", "SKU ID: " & SKUID)

            'Get SKU Max discount via item based

            pSQLDiscount = "select ItemBasedMaxAllowableDiscount  from LibraryOrganizationPIM where ProductID = " & ReadyString(SKUID)
            ItemBasedSKUID = CStr2(mProData.ExecuteScalar(pSQLDiscount))

            AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", pSQLDiscount)
            AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", "SKU ID: " & ItemBasedSKUID)

            If Not ItemBasedSKUID = vbNullString Then
                'Get SKU Max Discount

                pSQLDiscount = "SELECT Price " &
                " FROM MobizLibraryPIMProductPriceLevel  " &
                " WHERE PriceLevelID = " & ReadyString("01000012-0005-0000-0000-000000000000") &
                " AND ProductID = " & ReadyString(ItemBasedSKUID) &
                " AND OrganizationID = " & ReadyString("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba")

                SCExemptSales = CDec2(mProData.ExecuteScalar(pSQLDiscount))

                AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", pSQLDiscount)
                AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", "SCExemptSales : " & SCExemptSales)

            End If

            senior_discount = SCDiscount.ToString("######0.00")

            If SCExemptSales > 0 AndAlso CDec2(gross_sales) > SCExemptSales Then
                'cap discount
                Dim VatExemptSales As Double
                Dim VatableSales As Double

                VatableSales = CDec2(gross_sales) - SCExemptSales
                VatExemptSales = SCExemptSales / 1.12

                SC_VAT_EXEMPT_SALES = VatExemptSales.ToString("######0.00")

                net_sales = (VatableSales + VatExemptSales - SCDiscount).ToString("######0.00")
                VATABLE_SALES = (VatableSales / 1.12).ToString("######0.00")
                VAT = (VatableSales - (VatableSales / 1.12)).ToString("######0.00")

                'change gross sales computation
                'new gross = Discount Amount + Excess +  + Discount
                'discount amount = cap amount / 1.12 * .8

                'gross_sales = ((SCExemptSales / 1.12 * 0.8) + VatableSales + SCDiscount).ToString("######0.00")

                'AppendToFile(Common.ServerPath & "temp\logs\PITX.debug", "SCExemptSales: " & SCExemptSales.ToString & vbCrLf & " VatableSales: " & VatableSales & vbCrLf & " gross_sales: " & gross_sales)
            Else
                'no cap discount
                net_sales = ((CDec2(gross_sales) / 1.12) - SCDiscount).ToString("######0.00")
                SC_VAT_EXEMPT_SALES = (CDec2(gross_sales) / 1.12).ToString("######0.00")
            End If
            
        Else
            senior_discount = SCDiscount.ToString("######0.00")
            net_sales = (CDec2(gross_sales) / 1.12).ToString("######0.00")
            VATABLE_SALES = (CDec2(gross_sales) / 1.12).ToString("######0.00")
            VAT = (CDec2(gross_sales) - (CDec2(gross_sales) / 1.12)).ToString("######0.00")
        End If
        '======================= discount ================================

        pJsonHeader.Add("submission_uuid", "" & submission_uuid & "")
        pJsonHeader.Add("tenant_id", tenant_id)
        pJsonHeader.Add("terminal_id", terminal_id)
        pJsonHeader.Add("submission_timestamp", submission_timestamp)
        pJsonHeader.Add("transaction_count", transaction_count)

        pJsonForHeaderPayload.Add("submission_uuid", "" & submission_uuid & "")
        pJsonForHeaderPayload.Add("tenant_id", tenant_id)
        pJsonForHeaderPayload.Add("terminal_id", terminal_id)
        pJsonForHeaderPayload.Add("submission_timestamp", submission_timestamp)
        pJsonForHeaderPayload.Add("transaction_count", transaction_count)
        pJsonForHeaderPayload.Add("transaction", "")

        '=================For Checksum=================
        'tranasction checksum first kay dili ma-assign ang transaction checksum sa inner json

        pJsonForTransactionPayload.Add("hardware_id", "" & hardware_id & "")
        pJsonForTransactionPayload.Add("receipt_no", "" & receipt_no & "")
        pJsonForTransactionPayload.Add("transaction_id", "" & transaction_id & "")
        pJsonForTransactionPayload.Add("transaction_timestamp", "" & transaction_timestamp & "")
        pJsonForTransactionPayload.Add("gross_sales", "" & gross_sales & "")
        pJsonForTransactionPayload.Add("net_sales", "" & net_sales & "")
        pJsonForTransactionPayload.Add("promo_status", "" & promo_status & "")
        pJsonForTransactionPayload.Add("customer_code", "" & customer_code & "")

        With pJsonForTransactionPayload.AddList("adjustments")
            With .AddDictionary
                .Add("adjustment_type", "promo_discount")
                .Add("amount", "" & promo_discount_amount & "")
            End With
            With .AddDictionary
                .Add("adjustment_type", "senior_discount")
                .Add("amount", "" & senior_discount & "")
            End With
            With .AddDictionary
                .Add("adjustment_type", "pwd_discount")
                .Add("amount", "" & pwd_discount & "")
            End With
            With .AddDictionary
                .Add("adjustment_type", "vip_card_discount")
                .Add("amount", "" & vip_card_discount & "")
            End With
            With .AddDictionary
                .Add("adjustment_type", "service_charge_distributed_to_employees")
                .Add("amount", "" & service_charge_distributed_to_employees & "")
            End With
            With .AddDictionary
                .Add("adjustment_type", "service_charge_retained_by_management")
                .Add("amount", "" & service_charge_retained_by_management & "")
            End With
            With .AddDictionary
                .Add("adjustment_type", "employee_discount")
                .Add("amount", "" & employee_discount & "")
            End With
        End With

        With pJsonForTransactionPayload.AddList("taxes")
            With .AddDictionary
                .Add("amount", "" & VAT & "")
                .Add("tax_type", "VAT")
            End With
            With .AddDictionary
                .Add("amount", "" & VATABLE_SALES & "")
                .Add("tax_type", "VATABLE_SALES")
            End With
            With .AddDictionary
                .Add("amount", "" & SC_VAT_EXEMPT_SALES & "")
                .Add("tax_type", "SC_VAT_EXEMPT_SALES")
            End With
            With .AddDictionary
                .Add("amount", "" & OTHER_TAX & "")
                .Add("tax_type", "OTHER_TAX")
            End With
        End With


        payload_transaction_checksum = CreateJSONPayloadTransaction(pJsonForTransactionPayload)
        pJsonForTransactionPayload.Add("payload_checksum", payload_transaction_checksum)

        '=================For Checksum=================

        '=======================Transaction================================

        With pJsonHeader.AddDictionary("transaction")

            .Add("hardware_id", "" & hardware_id & "")
            .Add("receipt_no", "" & receipt_no & "")
            .Add("transaction_id", "" & transaction_id & "")
            .Add("transaction_timestamp", "" & transaction_timestamp & "")
            .Add("gross_sales", "" & gross_sales & "")
            .Add("net_sales", "" & net_sales & "")
            .Add("promo_status", "" & promo_status & "")
            .Add("customer_code", "" & customer_code & "")

            With .AddList("adjustments")
                With .AddDictionary
                    .Add("adjustment_type", "promo_discount")
                    .Add("amount", "" & promo_discount_amount & "")
                End With
                With .AddDictionary
                    .Add("adjustment_type", "senior_discount")
                    .Add("amount", "" & senior_discount & "")
                End With
                With .AddDictionary
                    .Add("adjustment_type", "pwd_discount")
                    .Add("amount", "" & pwd_discount & "")
                End With
                With .AddDictionary
                    .Add("adjustment_type", "vip_card_discount")
                    .Add("amount", "" & vip_card_discount & "")
                End With
                With .AddDictionary
                    .Add("adjustment_type", "service_charge_distributed_to_employees")
                    .Add("amount", "" & service_charge_distributed_to_employees & "")
                End With
                With .AddDictionary
                    .Add("adjustment_type", "service_charge_retained_by_management")
                    .Add("amount", "" & service_charge_retained_by_management & "")
                End With
                With .AddDictionary
                    .Add("adjustment_type", "employee_discount")
                    .Add("amount", "" & employee_discount & "")
                End With
            End With

            With .AddList("taxes")
                With .AddDictionary
                    .Add("tax_type", "VAT")
                    .Add("amount", "" & VAT & "")
                End With
                With .AddDictionary
                    .Add("tax_type", "VATABLE_SALES")
                    .Add("amount", "" & VATABLE_SALES & "")
                End With
                With .AddDictionary
                    .Add("tax_type", "SC_VAT_EXEMPT_SALES")
                    .Add("amount", "" & SC_VAT_EXEMPT_SALES & "")
                End With
                With .AddDictionary
                    .Add("tax_type", "OTHER_TAX")
                    .Add("amount", "" & OTHER_TAX & "")
                End With
            End With

            .Add("payload_checksum", "" & payload_transaction_checksum & "")

        End With


        ''=======================end of Transaction================================

        payload_checksum = CreateJSONPayloadMain(pJsonForHeaderPayload, pJsonForTransactionPayload)

        pJsonHeader.Add("payload_checksum", payload_checksum)

        jFinalString = pJsonHeader.ToString

        Return jFinalString

    End Function


    Private Function CreateJSONPayloadTransaction(json As JsonDictionary) As String
        Dim SortedJSONObject As String
        Dim pBytes() As Byte
        Dim payload_checksum As String

        SortedJSONObject = SortJSONObject(json).ToString()

        pBytes = New System.Text.ASCIIEncoding().GetBytes(SortedJSONObject)
        pBytes = New SHA256Managed().ComputeHash(pBytes)
        payload_checksum = BitConverter.ToString(pBytes).Replace("-", "").ToLower

        Return payload_checksum

    End Function

    Private Function CreateJSONPayloadMain(jsonHeader As JsonDictionary, jsonTransaction As JsonDictionary) As String
        Dim SortedJSONObject As JsonDictionary
        Dim SortedJSONObjectTransaction As JsonDictionary
        Dim pBytes() As Byte
        Dim payload_checksum As String

        SortedJSONObject = SortJSONObject(jsonHeader)

        'reinsert transaction
        Dim InsertTransaction As JsonDictionary
        Dim InsertTransactionString As String

        InsertTransaction = New JsonDictionary

        For Each Key As String In SortedJSONObject.Keys
            If Key = "transaction" Then

                With InsertTransaction.AddDictionary("transaction")

                    SortedJSONObjectTransaction = SortJSONObject(jsonTransaction)

                    For Each KeyTrans As String In SortedJSONObjectTransaction.Keys

                        .Add(KeyTrans, SortedJSONObjectTransaction(KeyTrans))

                        'If KeyTrans = "adjustments" OrElse KeyTrans = "taxes" Then
                        '    .Add(KeyTrans, SortedJSONObjectTransaction(KeyTrans))
                        'Else
                        '    .Add(KeyTrans, SortedJSONObjectTransaction(KeyTrans))
                        'End If

                    Next

                End With

            Else
                InsertTransaction.Add(Key.ToString, SortedJSONObject.Item(Key))
            End If

        Next

        InsertTransactionString = InsertTransaction.ToString

        pBytes = New System.Text.ASCIIEncoding().GetBytes(InsertTransactionString)
        pBytes = New SHA256Managed().ComputeHash(pBytes)
        payload_checksum = BitConverter.ToString(pBytes).Replace("-", "").ToLower

        Return payload_checksum

    End Function

    Function SortJSONObject(json As JsonDictionary) As JsonDictionary
        Dim dtJson As New DataTable
        Dim jsonTemp As JsonDictionary

        dtJson.Columns.Add("TransactionHeaders")

        For Each obj As Object In json.Keys

            'If obj <> "payload_checksum" Then
            Dim dr As DataRow

            dr = dtJson.NewRow
            dr(0) = obj

            dtJson.Rows.Add(dr)
            'End If
        Next

        dtJson.DefaultView.Sort = "TransactionHeaders ASC"

        Dim dtSorted As DataTable

        dtSorted = dtJson.DefaultView.ToTable

        jsonTemp = New JsonDictionary

        For i As Integer = 0 To dtSorted.Rows.Count - 1
            Dim vKey As String

            vKey = CStr2(dtSorted.Rows(i).Item("TransactionHeaders"))

            jsonTemp.Add(vKey, json(vKey))

        Next

        Return jsonTemp

    End Function

End Class
