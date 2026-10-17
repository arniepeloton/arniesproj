
Public Class MallLessorSM


    Function getSMTenantInfo(Optional ByVal vFowstID As String = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "sm.ID"
                    .Searchable = False
                End With
                With .Add("MallCode", SqlDbType.VarChar)
                    .Expression = "sm.MallCode"
                End With
                With .Add("TenantNo", SqlDbType.VarChar)
                    .Expression = "sm.TenantNo"
                End With
                With .Add("ClassCode", SqlDbType.VarChar)
                    .Expression = "sm.ClassCode"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTMallInfoSM sm "

            .SqlWhere = " WHERE sm.FOWSTID = " & ReadyString(vFowstID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getFOWSTOperationHR(ByVal dtDate As Date, Optional ByVal vFowstID As String = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "opthr.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "opthr.FOWSTID"
                End With
                With .Add("WeekDay", SqlDbType.Int)
                    .Expression = "opthr.WeekDay"
                End With
                With .Add("OpeningTime", SqlDbType.Time)
                    .Expression = "opthr.OpeningTime"
                End With
                With .Add("ClosingTime", SqlDbType.Time)
                    .Expression = "opthr.ClosingTime"
                End With
                With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                    .Expression = "fowst.IsFOWSTInsideAMall"
                End With
                With .Add("FOWSTMallName", SqlDbType.VarChar)
                    .Expression = "fowst.FOWSTMallName"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTOperationHRs opthr " &
                " INNER JOIN dbo.MobizLibraryOrganizationFOWST fowst ON (opthr.FOWSTID = fowst.ID) "

            .SqlWhere = " WHERE opthr.FOWSTID = " & ReadyString(vFowstID) &
                    " AND opthr.WeekDay = DATEPART(weekday,'" & dtDate.ToString("MM/dd/yyyy") & "')"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GenerateLessorSalesReport(ByVal dtDate As DateTime, ByVal IsGenerateFlg As Boolean,
        Optional ByVal vOrganizationID As String = Nothing,
        Optional ByVal vFowstID As String = Nothing) As String

        Dim strRet As String = ""
        Dim dtaHSaleTable As DataTable
        Dim dtaSchedTable As DataTable
        Dim dtDateFrom As DateTime
        Dim dtDateTo As DateTime
        Dim dtServerDateTime As DateTime

        Dim strOrgDir As String
        Dim strOrgFowstDir As String
        Dim strOrgFowstDateDir As String
        Dim strDailySalesFile As String
        Dim strHourlySalesFile As String
        Dim strDailySalesFileName As String
        Dim strHourlySalesFileName As String
        Dim strSQL As String
        Dim DBFConn As New OleDb.OleDbConnection()

        Dim strOrgID As String
        Dim strFOWSTID As String

        Try

            strOrgID = WebRequest.Instance.OrgID
            If (vOrganizationID.Trim().Length > 0) Then
                strOrgID = vOrganizationID
            End If

            strFOWSTID = WebRequest.Instance.FowstID
            If (vFowstID.Trim().Length > 0) Then
                strFOWSTID = vFowstID
            End If

            dtaSchedTable = getFOWSTOperationHR(dtDate, strFOWSTID)

            If (dtaSchedTable.Rows.Count > 0) Then

                ''' Set the Opening and closing DateTime
                ' '' ''dtDateFrom = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & dtaSchedTable.Rows(0).Item("OpeningTime").ToString())
                ' '' ''dtDateTo = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & dtaSchedTable.Rows(0).Item("ClosingTime").ToString())
                dtDateFrom = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " 00:00:00")
                dtDateTo = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " 23:59:59")
                dtDate = dtDateTo
                If (dtDateFrom > dtDateTo) Then
                    dtDateTo = dtDateTo.AddDays(1)
                End If

                dtServerDateTime = WebRequest.Instance.ProData.GetDate()

                'If (dtServerDateTime.Date = dtDate.Date) Then
                '    If (dtServerDateTime.Minute = 0) Then
                '        dtDate = dtServerDateTime.AddMinutes(-1).ToString("MM/dd/yyyy HH:") & "59:59"
                '    Else
                '        dtDate = Convert.ToDateTime(dtServerDateTime.ToString("MM/dd/yyyy") & " " & dtServerDateTime.AddHours(-1).Hour.ToString("00") & ":59:59")
                '    End If
                'Else
                '    dtDate = dtDateTo.AddMinutes(-1).ToString("MM/dd/yyyy HH:") & "59:59"
                'End If

                strHourlySalesFileName = dtDate.ToString("MMddyyyy") & ".csv"
                strOrgDir = Common.ServerPath & "temp\" & strOrgID
                strOrgFowstDir = strOrgDir & "\" & strFOWSTID
                strOrgFowstDateDir = strOrgFowstDir & "\" & dtDate.ToString("MMddyyyy")

                ''' Get the Daily Hourly Sales for the period
                dtaHSaleTable = getDailyHourlySales(dtDate, dtDateFrom, dtDateTo, strFOWSTID)

                ''' Create folder for SM OrgID folder
                If (Not IO.Directory.Exists(strOrgDir)) Then
                    IO.Directory.CreateDirectory(strOrgDir)
                End If
                ''' Create folder for SM FowstID folder
                If (Not IO.Directory.Exists(strOrgFowstDir)) Then
                    IO.Directory.CreateDirectory(strOrgFowstDir)
                End If
                ''' Create folder for SM FowstID folder
                If (Not IO.Directory.Exists(strOrgFowstDateDir)) Then
                    IO.Directory.CreateDirectory(strOrgFowstDateDir)
                End If

                '''Create Flag File
                If (IsGenerateFlg) Then
                    System.IO.File.Create(strOrgFowstDateDir & "\z" & dtDate.ToString("MMddyyyy") & ".flg").Dispose()
                End If

                TableToExcelFile(dtaHSaleTable, "", "", strOrgFowstDateDir & "\" & strHourlySalesFileName, False)

            End If

            '''generate zip file
            Dim pZip As ProZip
            Dim pZipFilename As String
            Dim pFileTitle As String

            pFileTitle = dtDate.ToString("MMddyyyy").Trim() & ".zip"
            pZipFilename = strOrgFowstDir & "\" & pFileTitle

            If (IO.File.Exists(pZipFilename)) Then
                IO.File.Delete(pZipFilename)
            End If

            pZip = New ProZip(pZipFilename)
            pZip.AddFolder(strOrgFowstDateDir)

            strRet = pFileTitle
        Catch
            If (DBFConn.State = ConnectionState.Open) Then
                DBFConn.Close()
            End If
            strRet = ""
        End Try

        Return strRet

    End Function

    Function getDailyHourlySales(ByVal dtDateTime As DateTime, _
                                 ByVal dtDateFrom As DateTime, _
                                 ByVal dtDateTo As DateTime, _
                                 Optional ByVal vFowstID As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("BR_CODE", SqlDbType.VarChar)
                    .Expression = "sm.MallCode"
                    .Searchable = False
                End With
                With .Add("N_CHECK", SqlDbType.VarChar)
                    .Expression = "sm.TenantNo"
                    .Searchable = False
                End With
                With .Add("CLAS_C", SqlDbType.VarChar)
                    .Expression = "sm.ClassCode"
                    .Searchable = False
                End With
                With .Add("CLAS_TRD_C", SqlDbType.VarChar)
                    .Expression = "sm.TradeCode"
                    .Searchable = False
                End With
                With .Add("STOR_NO", SqlDbType.Int)
                    .Expression = "sm.OutletNo"
                    .Searchable = False
                End With
                With .Add("TSL_NEW_A", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR, " &
                                    "CAST(" &
                                        "ROUND(" &
                                            "(ISNULL(devf.BegBalance,0) + " &
                                            " ISNULL((SELECT SUM(custpay.TotalAmountDue) " &
                                            " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                            " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                            " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                            " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                            " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            " ISNULL((SELECT SUM(custpay.DiscountAmount) " &
                                            " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                            " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                            " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                            " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                            " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0)+ " &
                                              "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) +" &
                                               "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0)" &
                                            " ) + " &
                                            " (ISNULL((SELECT SUM(custpay.TotalAmountDue) " &
                                            " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                            " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                            " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                            " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                            " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            " ISNULL((SELECT SUM(custpay.DiscountAmount) " &
                                            " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                            " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                            " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                            " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                            " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0) +" &
                                              "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) +" &
                                               "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0)" &
                                            " ) " &
                                        ",2) " &
                                    " as DECIMAL(12,2))" &
                                ",0) "

                    .Searchable = False
                End With
                With .Add("TSL_OLD_A", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR, " &
                                    "CAST(" &
                                        "ROUND(" &
                                            "(ISNULL(devf.BegBalance,0) + " &
                                            " ISNULL((SELECT SUM(custpay.TotalAmountDue) " &
                                            " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                            " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                            " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                            " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                            " AND inv.InvoiceDate  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            " ISNULL((SELECT SUM(custpay.DiscountAmount) " &
                                            " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                            " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                            " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                            " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                            " AND inv.InvoiceDate  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0) - " &
                                            "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                            " FROM TransSalesReturnLedger srledger " &
                                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                            " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND memo.FOWSTID = devf.FowstID " &
                                            " AND memo.DeviceID = devf.DeviceID) " &
                                            ",0) +" &
                                              "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) +" &
                                               "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0)" &
                                            " ) " &
                                        ",2) " &
                                    " as DECIMAL(12,2))" &
                                ",0) "
                    .Searchable = False
                End With
                With .Add("SALE_TYPE", SqlDbType.VarChar)
                    .Expression = "sm.SaleType"
                    .Searchable = False
                End With
                With .Add("TSL_DAY_A", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "(((ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) - " &
                                               "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0)) - " &
                                               "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) - " &
                                                "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                             " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) -" &
                                               "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                                " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) +" &
                                            "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '764EDACB-0131-4033-8E44-7B903368008C' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) +" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '7804A35A-B531-4380-945E-FC9643E99843' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) +" &
                                                 "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = 'C4199347-6A00-45C9-BE11-8934DD63ECD1' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) +" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '8E2DCA9C-433F-42FC-A644-04476DBA056A' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) -" &
                                                " ISNULL((SELECT sum(invitems.GrossSalesNetOfVATExemptions - invitems.DiscountAmount) " &
                                                " FROM dbo.TransInvoiceItems invitems " &
                                                " INNER JOIN dbo.TransInvoice inv ON (invitems.InvoiceID = inv.ID)  " &
                                                " WHERE inv.InvoiceNumber > 0 AND invitems.TaxAmount = 0 AND ISNULL(invitems.VATableSale,0) = 0 " &
                                                " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND inv.FOWSTID = devf.FowstID " &
                                                " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)) /1.12) + " &
                                               "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                                 " ISNULL((SELECT sum(invitems.GrossSalesNetOfVATExemptions - invitems.DiscountAmount) " &
                                                " FROM dbo.TransInvoiceItems invitems " &
                                                " INNER JOIN dbo.TransInvoice inv ON (invitems.InvoiceID = inv.ID)  " &
                                                " WHERE inv.InvoiceNumber > 0 AND invitems.TaxAmount = 0 AND ISNULL(invitems.VATableSale,0) = 0 " &
                                                " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND inv.FOWSTID = devf.FowstID " &
                                                " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                             " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) +" &
                                               "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                                " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_A", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '764EDACB-0131-4033-8E44-7B903368008C' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_B", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '7804A35A-B531-4380-945E-FC9643E99843' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_C", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '055A0A33-DD98-47EC-ADD3-8B2009F3944A' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_D", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '35F954A0-C67C-499C-AF60-F08E64477FDC' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_E", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(invitem.DiscountSales) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '34A4032F-A142-4C15-A868-4AC4410434DF' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_F", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                     "CAST(" &
                                         "ROUND(" &
                                            "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                            " FROM dbo.TransInvoiceItems invitem " &
                                            " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                            " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                            " WHERE inv.InvoiceNumber > 0 " &
                                            " AND invdisc.DiscountTypeID = 'C2218C86-0450-4CD3-B019-3EDD43E51741' " &
                                            " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID)" &
                                            ",0)" &
                                         ",2) " &
                                     " as DECIMAL(12,2))" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_G", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = 'C4199347-6A00-45C9-BE11-8934DD63ECD1' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_H", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '8E2DCA9C-433F-42FC-A644-04476DBA056A' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_I", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_J", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_K", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_DIS_L", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_TAX_A", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "(ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND (invdisc.DiscountTypeID = '34A4032F-A142-4C15-A868-4AC4410434DF' OR invdisc.DiscountTypeID = 'C2218C86-0450-4CD3-B019-3EDD43E51741') " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND (invdisc.DiscountTypeID <> '34A4032F-A142-4C15-A868-4AC4410434DF' AND invdisc.DiscountTypeID <> 'C2218C86-0450-4CD3-B019-3EDD43E51741') " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                                               " LEFT JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID IS NULL" &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) - " &
                                               "ISNULL((SELECT SUM(memosr.TaxAmount) " &
                                               " FROM TransMemoSalesReturn memosr " &
                                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0))" &
                                            ",2) " &
                                    " as DECIMAL(12,2))" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TAX_B", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                       "CAST(" &
                                           "ROUND(" &
                                              "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0)" &
                                           ",2) " &
                                       " as DECIMAL(12,2))" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_ADJ_A", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_ADJ_POS", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_ADJ_NEG", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_ADJ_NT_POS", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_ADJ_NT_NEG", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_NET_A", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) - " &
                                               "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_VOID", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_RFND", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TX_SAL", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "(ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) - " &
                                               "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0)) - " &
                                               "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) - " &
                                               "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) - " &
                                               "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0) +" &
                                            "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '764EDACB-0131-4033-8E44-7B903368008C' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) +" &
                                                "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '7804A35A-B531-4380-945E-FC9643E99843' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) +" &
                                                 "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = 'C4199347-6A00-45C9-BE11-8934DD63ECD1' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) +" &
                                               "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM dbo.TransInvoiceItems invitem " &
                                               " INNER JOIN dbo.TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN dbo.TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '8E2DCA9C-433F-42FC-A644-04476DBA056A' " &
                                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0) -" &
                                                " ISNULL((SELECT sum(invitems.GrossSalesNetOfVATExemptions - invitems.DiscountAmount) " &
                                                " FROM dbo.TransInvoiceItems invitems " &
                                                " INNER JOIN dbo.TransInvoice inv ON (invitems.InvoiceID = inv.ID)  " &
                                                " WHERE inv.InvoiceNumber > 0 AND invitems.TaxAmount = 0 AND ISNULL(invitems.VATableSale,0) = 0 " &
                                                " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND inv.FOWSTID = devf.FowstID " &
                                                " AND inv.DeviceID = devf.DeviceID)" &
                                                ",0) " &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_NX_SAL", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                    "CAST(" &
                                        "ROUND(" &
                                            " (ISNULL((SELECT sum(invitems.GrossSalesNetOfVATExemptions - invitems.DiscountAmount) " &
                                            " FROM dbo.TransInvoiceItems invitems " &
                                            " INNER JOIN dbo.TransInvoice inv ON (invitems.InvoiceID = inv.ID)  " &
                                            " WHERE (inv.InvoiceNumber > 0) AND invitems.TaxAmount = 0 AND ISNULL(invitems.VATableSale,0) = 0 " &
                                            " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                            " AND inv.FOWSTID = devf.FowstID " &
                                            " AND inv.DeviceID = devf.DeviceID)" &
                                           ",0))" &
                                        ",2) " &
                                " as DECIMAL(12,2))" &
                            ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_CHG", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) + " &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                                "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0) + " &
                                                 "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0) "
                    .Searchable = False
                End With
                With .Add("TSL_CSH", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0) " &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_GC", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                       "CAST(" &
                                           "ROUND(" &
                                              "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                              " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                              " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                              " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID ) " &
                                              ",0)" &
                                           ",2) " &
                                      " as DECIMAL(12,2))" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_EPS", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TND", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                      "CAST(" &
                                          "ROUND(" &
                                             "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                                             " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                             " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                             " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                             " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                             " AND inv.FOWSTID = devf.FowstID " &
                                             " AND inv.DeviceID = devf.DeviceID ) " &
                                             ",0) +" &
                                              "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                                             " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                             " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                             " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                             " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                             " AND inv.FOWSTID = devf.FowstID " &
                                             " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                          ",2) " &
                                     " as DECIMAL(12,2))" &
                                  ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_MCRD", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpaycc.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND custpaycc.CreditCardType = 'Master Card'" &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_VISA", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpaycc.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND custpaycc.CreditCardType = 'Visa'" &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_AMEX", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpaycc.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND custpaycc.CreditCardType = 'American Express'" &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_DINERS", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpaycc.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND custpaycc.CreditCardType = 'Diners'" &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_JCB", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpaycc.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND custpaycc.CreditCardType = 'JCB'" &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_OTCRD", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(custpaycc.CreditCardAmount,0)) " &
                                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                               " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                               " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND (custpaycc.CreditCardType = 'Other Card' OR custpaycc.CreditCardType = 'UnionPay')" &
                                               " AND inv.FOWSTID = devf.FowstID " &
                                               " AND inv.DeviceID = devf.DeviceID ) " &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_SV_CHG", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "CAST(" &
                                            "ROUND(" &
                                               "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) +" &
                                              "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)" &
                                            ",2) " &
                                       " as DECIMAL(12,2))" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_OT_CHG", SqlDbType.VarChar)
                    .Expression = "'0.00'"
                    .Searchable = False
                End With
                With .Add("TSL_FT", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT MIN(inv.InvoiceNumber) " &
                                        " FROM dbo.TransInvoice inv " &
                                        " WHERE inv.InvoiceNumber > 0 " &
                                        " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID)" &
                                        ",0)" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_LT", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT MAX(inv.InvoiceNumber) " &
                                        " FROM dbo.TransInvoice inv " &
                                        " WHERE inv.InvoiceNumber > 0 " &
                                        " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID)" &
                                        ",0)" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_NT", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(inv.InvoiceNumber) " &
                                        " FROM dbo.TransInvoice inv " &
                                        " WHERE inv.InvoiceNumber > 0 " &
                                        " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID)" &
                                        ",0)" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_BEG_INV", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT MIN(inv.InvoiceNumber) " &
                                        " FROM dbo.TransInvoice inv " &
                                        " WHERE inv.InvoiceNumber > 0 " &
                                        " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID)" &
                                        ",0)" &
                               ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_END_INV", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                    "ISNULL((SELECT MAX(inv.InvoiceNumber) " &
                                    " FROM dbo.TransInvoice inv " &
                                    " WHERE inv.InvoiceNumber > 0 " &
                                    " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                    " AND inv.FOWSTID = devf.FowstID " &
                                    " AND inv.DeviceID = devf.DeviceID)" &
                                    ",0)" &
                               ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_CASH", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpay.CashAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                         " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpay.CashAmount,0) > 0 " &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID) " &
                                        ",0)" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_GC", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpay.CheckPaymentAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpay.CheckPaymentAmount,0) > 0 " &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID) " &
                                        ",0)" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_EPS", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpay.DebitCardAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpay.DebitCardAmount,0) > 0 " &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID) " &
                                        ",0)" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_TND", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                      "ISNULL((SELECT COUNT(custpay.OtherTypeAmount) " &
                                      " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                      " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                      " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                      " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                      " AND ISNULL(custpay.OtherTypeAmount,0) > 0 " &
                                      " AND inv.FOWSTID = devf.FowstID " &
                                      " AND inv.DeviceID = devf.DeviceID) " &
                                      ",0) +" &
                                       "ISNULL((SELECT COUNT(custpay.SRVAmount) " &
                                      " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                      " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                      " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                      " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                      " AND ISNULL(custpay.OtherTypeAmount,0) > 0 " &
                                      " AND inv.FOWSTID = devf.FowstID " &
                                      " AND inv.DeviceID = devf.DeviceID) " &
                                      ",0)" &
                                  ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_MCD", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpaycc.CreditCardAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND custpaycc.CreditCardType = 'Master Card'" &
                                        " AND ISNULL(custpaycc.CreditCardAmount,0) > 0 " &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID ) " &
                                        ",0)" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_VIS", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpaycc.CreditCardAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpaycc.CreditCardAmount,0) > 0" &
                                        " AND custpaycc.CreditCardType = 'Visa'" &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID ) " &
                                        ",0)" &
                                ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_AMX", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpaycc.CreditCardAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpaycc.CreditCardAmount,0) > 0" &
                                        " AND custpaycc.CreditCardType = 'American Express'" &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID ) " &
                                        ",0)" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_DIN", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpaycc.CreditCardAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpaycc.CreditCardAmount,0) > 0" &
                                        " AND custpaycc.CreditCardType = 'Diners'" &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID ) " &
                                        ",0)" &
                                    ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_JCB", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpaycc.CreditCardAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpaycc.CreditCardAmount,0) > 0" &
                                        " AND custpaycc.CreditCardType = 'JCB'" &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID ) " &
                                        ",0)" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_TC_OC", SqlDbType.VarChar)
                    .Expression = "CONVERT(VARCHAR," &
                                        "ISNULL((SELECT COUNT(custpaycc.CreditCardAmount) " &
                                        " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                        " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                        " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                        " INNER JOIN dbo.TransCustomerPaymentCreditCard custpaycc ON (custpaycc.CustomerPaymentID = custpay.ID)  " &
                                        " WHERE inv.InvoiceNumber > 0 AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                        " AND ISNULL(custpaycc.CreditCardAmount,0) > 0" &
                                        " AND custpaycc.CreditCardType = 'Other Card'" &
                                        " AND inv.FOWSTID = devf.FowstID " &
                                        " AND inv.DeviceID = devf.DeviceID ) " &
                                        ",0)" &
                                   ",0)"
                    .Searchable = False
                End With
                With .Add("TSL_MCH", SqlDbType.VarChar)
                    .Expression = "devf.SMMachineNo"
                    .Searchable = False
                End With
                With .Add("TSL_SRL", SqlDbType.VarChar)
                    .Expression = "REPLACE(devf.POSRegistrationNo,'-','')"
                    .Searchable = False
                End With
                With .Add("TSL_ZCNT", SqlDbType.Int)
                    .Expression = "0"
                    .Searchable = False
                End With
                With .Add("TST_TIME", SqlDbType.VarChar)
                    .Expression = "'" & Now.ToString("HHmmss") & "'"
                    .Searchable = False
                End With
                'dtDateTime old code replaced with Now
                With .Add("TSL_DTE", SqlDbType.VarChar)
                    .Expression = "'" & dtDateTime.ToString("MMddyyyy") & "'"
                    .Searchable = False
                End With
            End With


            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf " &
                    " INNER JOIN dbo.LibraryDevice dev ON (devf.DeviceID = dev.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrganizationFOWSTMallInfoSM sm  ON (sm.FOWSTID = devf.FowstID)"

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(vFowstID) &
                " AND dev.IsActive != 0 " &
                " AND ISNULL(devf.SMMachineNo,'') != ''"

            pDT = .Requery

        End With

        Return pDT

    End Function



End Class
