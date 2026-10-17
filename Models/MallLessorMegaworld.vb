
Public Class MallLessorMegaworld


    Function getMegaworldTenantInfo(ByVal mProdata As ProData, Optional vFowstID As String = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = mProdata

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "mgw.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "mgw.FOWSTID"
                End With
                With .Add("TenantCode", SqlDbType.VarChar)
                    .Expression = "mgw.TenantCode"
                End With
                With .Add("TenantPOSTerminalNo", SqlDbType.VarChar)
                    .Expression = "mgw.TenantPOSTerminalNo"
                End With
                With .Add("SaleType", SqlDbType.VarChar)
                    .Expression = "mgw.SaleType"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrganizationFOWSTMallInfoMegaworld mgw "

            .SqlWhere = " WHERE mgw.FOWSTID = " & ReadyString(vFowstID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getFOWSTOperationHR(ByVal mProdata As ProData, ByVal dtDate As Date, Optional vFowstID As String = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID

        pFinder = New Finder
        With pFinder
            .ProData = mProdata

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

            .SqlFrom = " FROM MobizLibraryOrganizationFOWSTOperationHRs opthr " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (opthr.FOWSTID = fowst.ID) "

            .SqlWhere = " WHERE opthr.FOWSTID = " & ReadyString(vFowstID) &
                    " AND opthr.WeekDay = DATEPART(weekday,'" & dtDate.ToString("MM/dd/yyyy") & "')"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GenerateLessorSalesReport(ByVal isGenerateNew As Boolean, ByVal mProdata As ProData, ByVal dtDate As DateTime, ByVal tenantinfo As Object, Optional vOrganizationID As String = Nothing,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing) As Object

        Dim strRet As Object
        Dim dtaDSaleTable As DataTable
        Dim dtaHSaleTable As DataTable
        Dim dtaSchedTable As DataTable
        Dim dtDateFrom As DateTime
        Dim dtDateTo As DateTime
        Dim strMegaworldFolder As String
        'Dim strDailyTempFile As String
        'Dim strHourlyTempFile As String

        Dim strDeviceDir As String
        Dim strDailySalesFile As String
        Dim strDailySalesFileText As String
        Dim strHourlySalesFile As String
        Dim strHourlySalesFileText As String
        Dim strDiscountFileText As String
        Dim strDailySalesFileName As String
        Dim strDailySalesFileNameText As String
        Dim strHourlySalesFileName As String
        Dim strHourlySalesFileNameText As String
        Dim strDiscountFileNameText As String
        Dim strSQL As String
        Dim strDeviceIDs As String
        Dim DBFConn As New OleDb.OleDbConnection()
        Dim pFS As IO.FileStream
        Dim pBytes() As Byte

        Try
            If vOrganizationID Is Nothing Then vOrganizationID = WebRequest.Instance.OrgID.Trim()
            If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID.Trim()
            If vDeviceID Is Nothing Then vDeviceID = IIf(WebRequest.Instance.DeviceID = Nothing, "808ecdfd-86a6-4e77-acc3-1aad0955a179", CStr2(WebRequest.Instance.DeviceID).Trim())

            strMegaworldFolder = Common.ServerPath & "Content\MegaworldTenant"
            'strDailyTempFile = strMegaworldFolder & "\XXXMMDD.dbf"
            'strHourlyTempFile = strMegaworldFolder & "\XXXMMDDH.dbf"

            '''Set Directory and File Name
            strDeviceDir = Common.ServerPath & "Content\Megaworld"
            'AppendToDebug("Path: " & strDeviceDir)
            If Not IO.Directory.Exists(strDeviceDir) Then
                IO.Directory.CreateDirectory(strDeviceDir)
            End If

            strDeviceDir = strDeviceDir & "\" & vOrganizationID

            If Not IO.Directory.Exists(strDeviceDir) Then
                IO.Directory.CreateDirectory(strDeviceDir)
            End If

            strDeviceDir = strDeviceDir & "\" & vFowstID

            If Not IO.Directory.Exists(strDeviceDir) Then
                IO.Directory.CreateDirectory(strDeviceDir)
            End If

            strDeviceDir = strDeviceDir & "\" & vDeviceID

            'If IO.Directory.Exists(strDeviceDir) And isGenerateNew Then
            '    IO.Directory.Delete(strDeviceDir, True)
            'End If

            'If Not IO.Directory.Exists(strDeviceDir) Then
            '    AppendToDebug("Folder: " & strDeviceDir)
            '    IO.Directory.CreateDirectory(strDeviceDir)
            'End If

            'strDailySalesFileName = "S" & strTenantCode.Substring(0, 3) & strTenantPOSTerminalNo & "." & dtDate.ToString("Mdd")
            'strDailySalesFile = strDeviceDir & "\" & strDailySalesFileName
            Dim month As String = DateAndTime.Month(dtDate)
            If CInt2(month) > 9 Then
                If CInt2(month) = 10 Then
                    month = "A"
                ElseIf CInt2(month) = 11 Then
                    month = "B"
                ElseIf CInt2(month) = 12 Then
                    month = "C"
                End If
            End If

            Dim tenantlog As DataRow = mProdata.ExecuteRow("SELECT ID, BatchNo FROM TenantMonitoringLogs WHERE FOWSTID = " & ReadyString(vFowstID) & " AND CONVERT(date, SalesDate) = " & ReadyString(dtDate.ToString("MM/dd/yyyy")))
            Dim batchnumber As Integer = CInt2(tenantlog("BatchNo")) + 1
            Dim pIU As New InsertUpdate("TenantMonitoringLogs")
            Dim zreadname As String
            pIU("BatchNo") = batchnumber
            mProdata.Execute(pIU.GetUpdate(" ID = " & ReadyString(tenantlog("ID").ToString())))

            strDailySalesFileNameText = "S" & Left(tenantinfo("TenantCode"), 4) & Format(tenantinfo("TenantPOSTerminalNo"), "00") & batchnumber & "." & month & Format(DateAndTime.Day(dtDate), "00")
            strDailySalesFileText = strDeviceDir & "\" & strDailySalesFileNameText

            strHourlySalesFileNameText = "H" & Left(tenantinfo("TenantCode"), 4) & Format(tenantinfo("TenantPOSTerminalNo"), "00") & batchnumber & "." & month & Format(DateAndTime.Day(dtDate), "00")
            strHourlySalesFileText = strDeviceDir & "\" & strHourlySalesFileNameText

            strDiscountFileNameText = "D" & Left(tenantinfo("TenantCode"), 4) & Format(tenantinfo("TenantPOSTerminalNo"), "00") & batchnumber & "." & month & Format(DateAndTime.Day(dtDate), "00")
            strDiscountFileText = strDeviceDir & "\" & strDiscountFileNameText

            dtaSchedTable = getFOWSTOperationHR(mProdata, dtDate, vFowstID)

            'If (dtaSchedTable.Rows.Count > 0) Then

            ''' Set the Opening and closing DateTime
            dtDateFrom = dtDate.ToString("MM/dd/yyyy") 'Convert.ToDateTime( '& " " & dtaSchedTable.Rows(0).Item("OpeningTime").ToString())
            dtDateTo = dtDate.ToString("MM/dd/yyyy") 'Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy")) '" & dtaSchedTable.Rows(0).Item("ClosingTime").ToString())
            If (dtDateFrom > dtDateTo) Then
                dtDateTo = dtDateTo.AddDays(1)
            End If

            ''' Get the Daily sales for the period
            dtaDSaleTable = getDailySales(mProdata, dtDateFrom, dtDateTo, vFowstID, vDeviceID)

            If (dtaDSaleTable.Rows.Count > 0) Then
                Dim dRowSales = dtaDSaleTable.Rows(0)
                'AppendToLog("Actual Output Data " & vbTab & " 1", "temp\Megaworld", "SNNNNTTB", ".txt")
                Dim folder As String = strDeviceDir
                If Not System.IO.Directory.Exists(folder) Then
                    System.IO.Directory.CreateDirectory(folder)
                End If
                Dim filename As String = folder & "\" & strDailySalesFileNameText '& ".txt"
                Dim description As String = ""
                Dim clsSalesReport As New ReportPOS
                Dim pDtHeader = clsSalesReport.getReportHeader(mProdata, vFowstID, Nothing, vDeviceID)

#Region "Daily Sales"
                Dim netsales As String = (dRowSales("CurGrossSales") - (dRowSales("TotalSCDiscount") + dRowSales("CurTotalDiscount")) - dRowSales("CurSalesReturnRefund") - dRowSales("CurSalesReturnExchange") - dRowSales("CurVATExempt") + dRowSales("CurServiceCharge"))
                Dim prevnetsales As String = (dRowSales("PrevGrossSales") - (dRowSales("PrevTotalSCDiscount") + dRowSales("PrevTotalDiscount")) - dRowSales("PrevSalesReturnRefund") - dRowSales("PrevSalesReturnExchange") - dRowSales("PrevVATExempt") + dRowSales("PrevServiceCharge")) ' + dRowSales("PrevChargeAmount")
                Dim CashRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "cash")))
                Dim OtherPaymentRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "otherpayment")))
                Dim DiscountRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "discount")))
                Dim DebitRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "debit")))
                Dim CreditRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "credit")))
                Dim prevtotalpayments = (CDec2(dRowSales("PrevCashAmount")) + (CDec2(dRowSales("PrevDebitCardAmount")) + CDec2(dRowSales("PrevCreditCardAmount"))) + (CDec2(dRowSales("PrevCheckPaymentAmount")) + CDec2(dRowSales("PrevOthers")))) - (CashRefund + OtherPaymentRefund)

                CashRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "cash")))
                OtherPaymentRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "otherpayment")))
                DiscountRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "discount")))
                DebitRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "debit")))
                CreditRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "credit")))

                Dim totalpayments = (CDec2(dRowSales("CashAmount")) + (CDec2(dRowSales("DebitCardAmount")) + CDec2(dRowSales("CreditCardAmount"))) + (CDec2(dRowSales("CheckPaymentAmount")) + CDec2(dRowSales("CurOthers")))) - (CashRefund + OtherPaymentRefund)

                netsales = Roundoff(netsales)
                prevnetsales = Roundoff(prevnetsales)
                totalpayments = Roundoff(totalpayments)

                'description &= "Actual Output Data" & vbTab & vbTab & "Fields Description" & vbTab & vbTab & vbTab & "Actual Value" & vbCrLf
                description &= "01" & tenantinfo.Item("TenantCode") & vbCrLf
                description &= "02" & tenantinfo.Item("TenantPOSTerminalNo") & vbCrLf
                description &= "03" & Format(dtDate, "MMddyyyy") & vbCrLf
                description &= "04" & Int64.Parse(Regex.Replace(CDec2(prevtotalpayments).ToString("N"), "[^\d]", "")) & vbCrLf
                description &= "05" & Int64.Parse(Regex.Replace(CDec2(CStr2(CDec2(prevtotalpayments) + CDec2(totalpayments))).ToString("N"), "[^\d]", "")) & vbCrLf

                Dim scdiscount = Roundoff(CDec2(dRowSales("TotalSCDiscount").ToString()))
                Dim otherdiscount = Roundoff(CDec2(dRowSales("CurTotalDiscount").ToString()))
                Dim salesrefund = Roundoff(CDec2(dRowSales("CurSalesReturnRefund").ToString()))

                If CDec2(dRowSales("CurGrossSales")) = 0 Then
                    description &= "06000" & vbCrLf
                Else
                    description &= "06" & Int64.Parse(Regex.Replace(CDec2(CDec2(totalpayments) + CDec2(scdiscount - DiscountRefund) + CDec2(otherdiscount) + CDec2(salesrefund)).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("CurNoTAXSales")) = 0 Then
                    description &= "07000" & vbCrLf
                Else
                    description &= "07" & Int64.Parse(Regex.Replace(CDec2(dRowSales("CurNoTAXSales").ToString()).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("TotalSCDiscount")) = 0 Then
                    description &= "08000" & vbCrLf
                Else
                    description &= "08" & Int64.Parse(Regex.Replace(CDec2(scdiscount - DiscountRefund).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("CurTotalDiscount")) = 0 Then
                    description &= "09000" & vbCrLf
                Else
                    description &= "09" & Int64.Parse(Regex.Replace(CDec2(otherdiscount).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("CurSalesReturnRefund")) = 0 Then
                    description &= "10000" & vbCrLf
                Else
                    description &= "10" & Int64.Parse(Regex.Replace(CDec2(salesrefund).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("CurTotalVAT")) = 0 Then
                    description &= "11000" & vbCrLf
                Else
                    description &= "11" & Int64.Parse(Regex.Replace(CDec2(dRowSales("CurTotalVAT").ToString()).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("CurServiceCharge")) = 0 Then
                    description &= "12000" & vbCrLf
                Else
                    description &= "12" & Int64.Parse(Regex.Replace(CDec2(dRowSales("CurServiceCharge").ToString()).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(totalpayments) = 0 Then
                    description &= "13000" & vbCrLf
                Else
                    description &= "13" & Int64.Parse(Regex.Replace(CDec2(totalpayments).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("CashAmount")) = 0 Then
                    description &= "14000" & vbCrLf
                Else
                    description &= "14" & Int64.Parse(Regex.Replace(CDec2(dRowSales("CashAmount") - CashRefund).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("DebitCardAmount")) = 0 And CDec2(dRowSales("CreditCardAmount")) = 0 Then
                    description &= "15000" & vbCrLf
                Else
                    description &= "15" & Int64.Parse(Regex.Replace(CDec2(CStr2(dRowSales("DebitCardAmount") + dRowSales("CreditCardAmount"))).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                If CDec2(dRowSales("CheckPaymentAmount")) = 0 And CDec2(dRowSales("CurOthers")) = 0 Then
                    '
                    description &= "16000" & vbCrLf
                Else
                    description &= "16" & Int64.Parse(Regex.Replace(CDec2(CStr2((dRowSales("CheckPaymentAmount") + dRowSales("CurOthers")) - OtherPaymentRefund)).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                description &= "17000" & vbCrLf
                description &= "18" & CStr2(dRowSales("CurTransCountInv") + dRowSales("CurTransCountMemoReturnExchange") + dRowSales("CurTransCountMemoRefund")) & vbCrLf
                description &= "1901" & vbCrLf
                description &= "20" & dRowSales("CurTransCountInv").ToString() & vbCrLf
                description &= "21" & tenantinfo("SaleType").ToString() & vbCrLf

                If CDec2(netsales) = 0 Then
                    description &= "22000" & vbCrLf
                Else
                    description &= "22" & Int64.Parse(Regex.Replace(CDec2(netsales).ToString("N"), "[^\d]", "")) & vbCrLf
                End If

                pFS = New IO.FileStream(filename, IO.FileMode.Create)
                pBytes = New System.Text.ASCIIEncoding().GetBytes(description)
                pFS.Write(pBytes, 0, pBytes.Length)
                pFS.Close()
#End Region

#Region "Hourly Sales"
                filename = folder & "\" & strHourlySalesFileNameText '& ".txt"
                description = ""
                Dim hourcode(23) As Object
                Dim totalnetsales As Decimal = 0
                Dim totalsalestrans As Decimal = 0
                Dim totalcustomercount As Decimal = 0
                hourcode(0) = New With {Key .From = "1:01", .To = "2:00", .Code = "1"}
                hourcode(1) = New With {Key .From = "2:01", .To = "3:00", .Code = "2"}
                hourcode(2) = New With {Key .From = "3:01", .To = "4:00", .Code = "3"}
                hourcode(3) = New With {Key .From = "4:01", .To = "5:00", .Code = "4"}
                hourcode(4) = New With {Key .From = "5:01", .To = "6:00", .Code = "5"}
                hourcode(5) = New With {Key .From = "6:01", .To = "7:00", .Code = "6"}
                hourcode(6) = New With {Key .From = "7:01", .To = "8:00", .Code = "7"}
                hourcode(7) = New With {Key .From = "8:01", .To = "9:00", .Code = "8"}
                hourcode(8) = New With {Key .From = "9:01", .To = "10:00", .Code = "9"}
                hourcode(9) = New With {Key .From = "10:01", .To = "11:00", .Code = "10"}
                hourcode(10) = New With {Key .From = "11:01", .To = "12:00", .Code = "11"}
                hourcode(11) = New With {Key .From = "12:01", .To = "13:00", .Code = "12"}
                hourcode(12) = New With {Key .From = "13:01", .To = "14:00", .Code = "13"}
                hourcode(13) = New With {Key .From = "14:01", .To = "15:00", .Code = "14"}
                hourcode(14) = New With {Key .From = "15:01", .To = "16:00", .Code = "15"}
                hourcode(15) = New With {Key .From = "16:01", .To = "17:00", .Code = "16"}
                hourcode(16) = New With {Key .From = "17:01", .To = "18:00", .Code = "17"}
                hourcode(17) = New With {Key .From = "18:01", .To = "19:00", .Code = "18"}
                hourcode(18) = New With {Key .From = "19:01", .To = "20:00", .Code = "19"}
                hourcode(19) = New With {Key .From = "20:01", .To = "21:00", .Code = "20"}
                hourcode(20) = New With {Key .From = "21:01", .To = "22:00", .Code = "21"}
                hourcode(21) = New With {Key .From = "22:01", .To = "23:00", .Code = "22"}
                hourcode(22) = New With {Key .From = "23:01", .To = "00:00", .Code = "23"}
                hourcode(23) = New With {Key .From = "00:01", .To = "1:00", .Code = "24"}

                'description &= "Actual Output Data" & vbTab & vbTab & "Fields Description" & vbTab & vbTab & vbTab & vbTab & "Actual Value" & vbCrLf
                description &= "01" & tenantinfo.Item("TenantCode") & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "01. Tenant Code" & vbTab & vbTab & vbTab & vbTab & vbTab & tenantinfo.Item("TenantCode") & vbCrLf
                description &= "02" & tenantinfo.Item("TenantPOSTerminalNo") & vbCrLf  ' & vbTab & vbTab & vbTab & "02. POS Terminal Number" & vbTab & vbTab & vbTab & vbTab & tenantinfo.Item("TenantPOSTerminalNo") & vbCrLf
                description &= "03" & Format(dtDate, "MMddyyyy") & vbCrLf  ' & vbTab & vbTab & vbTab & "03. Date (mmddyyyy)" & vbTab & vbTab & vbTab & vbTab & Format(dtDate, "MM/dd/yyyy") & vbCrLf

                For i As Integer = 0 To 23
                    dtaDSaleTable = getDailyHourlySales(mProdata, dtDate.ToString("MM/dd/yyyy") & " " & hourcode(i).From, dtDateTo.ToString("MM/dd/yyyy") & " " & hourcode(i).To, vFowstID, vDeviceID)

                    If dtaDSaleTable.Rows.Count > 0 Then
                        dRowSales = dtaDSaleTable.Rows(0)
                        If dRowSales("CurGrossSales") > 0 Then
                            netsales = dRowSales("CurGrossSales") - (dRowSales("TotalSCDiscount") + dRowSales("CurTotalDiscount")) - dRowSales("CurSalesReturnRefund") - dRowSales("CurSalesReturnExchange") - dRowSales("CurVATExempt") + dRowSales("CurServiceCharge")
                            totalnetsales += netsales
                            totalsalestrans += dRowSales("CurTransCountInv")
                            totalcustomercount += dRowSales("CurCustomerCount")

                            description &= "04" & hourcode(i).Code & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "04. Hour Code" & vbTab & vbTab & vbTab & vbTab & vbTab & hourcode(i).Code & vbCrLf

                            If netsales = 0 Then
                                description &= "05000" & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "05. Net Sales Amount / Hour" & vbTab & vbTab & vbTab & "0.00" & vbCrLf
                            Else
                                description &= "05" & Int64.Parse(Regex.Replace(CDec2(netsales).ToString("N"), "[^\d]", "")) & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "05. Net Sales Amount / Hour" & vbTab & vbTab & vbTab & CDec2(netsales).ToString("N") & vbCrLf
                            End If

                            description &= "06" & dRowSales("CurTransCountInv").ToString() & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "06. Number of Sales Transaction / Hour" & vbTab & vbTab & dRowSales("CurTransCountInv").ToString() & vbCrLf
                            description &= "07" & dRowSales("CurCustomerCount").ToString() & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "07. Customer Count / Hour" & vbTab & vbTab & vbTab & dRowSales("CurCustomerCount").ToString() & vbCrLf
                        End If
                    End If
                Next

                If totalnetsales = 0 Then
                    description &= "08000" & vbCrLf  ' & vbTab & vbTab & vbTab & "08. Total Net Sales Amount / Day" & vbTab & vbTab & "0.00" & vbCrLf
                Else
                    description &= "08" & Int64.Parse(Regex.Replace(CDec2(totalpayments).ToString("N"), "[^\d]", "")) & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "08. Total Net Sales Amount / Day" & vbTab & vbTab & CDec2(totalnetsales).ToString("N") & vbCrLf
                End If

                description &= "09" & totalsalestrans & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "09. Total Number of Sales Transaction / Day" & vbTab & totalsalestrans & vbCrLf
                description &= "10" & totalcustomercount & vbCrLf  ' & vbTab & vbTab & vbTab & vbTab & "10. Total Customer Count / Day" & vbTab & vbTab & vbTab & totalcustomercount & vbCrLf

                pFS = New IO.FileStream(filename, IO.FileMode.Create)
                pBytes = New System.Text.ASCIIEncoding().GetBytes(description)
                pFS.Write(pBytes, 0, pBytes.Length)
                pFS.Close()
#End Region

#Region "DISCOUNTS"
                filename = folder & "\" & strDiscountFileNameText '& ".txt"
                description = ""
                dtaDSaleTable = getDailyDiscountBreakdown(mProdata, dtDate.ToString("MM/dd/yyyy hh:mm:ss tt"), dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt"), vFowstID, vDeviceID)
                dtaDSaleTable.Columns.Add("Code")
                If dtaDSaleTable.Rows.Count > 0 Then
                    For Each disc As Object In dtaDSaleTable.Rows
                        Dim code As String = ""

                        If disc("DiscountTypeID").ToString().ToLower() = "35f954a0-c67c-499c-af60-f08e64477fdc" Then
                            code = "VIPDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "8e2dca9c-433f-42fc-a644-04476dba056a" Then
                            code = "REGDISC3"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "c4199347-6a00-45c9-be11-8934dd63ecd1" Then
                            code = "REGDISC2"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "764edacb-0131-4033-8e44-7b903368008c" Then
                            code = "REGDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "7804a35a-b531-4380-945e-fc9643e99843" Then
                            code = "EMPDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "055a0a33-dd98-47ec-add3-8b2009f3944a" Then
                            code = "SENDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "34a4032f-a142-4c15-a868-4ac4410434df" Then
                            code = "PWDDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "674ad791-188b-4238-b95c-97b5c2b9d544" Then
                            code = "SPESENDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "c2218c86-0450-4cd3-b019-3edd43e51741" Then
                            code = "GPCDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "21ebf55b-526c-4276-b3b7-0cbca504f425" Then
                            code = "NAACDISC"
                        ElseIf disc("DiscountTypeID").ToString().ToLower() = "e95f0b64-1eea-47b4-bde5-b795234b462d" Then
                            code = "SOLOPARENTDISC"
                        Else
                            code = disc("DiscountType")
                        End If
                        disc("Code") = code

                        If code = "SENDISC" Then
                            disc("DiscountAmount") = CDec2(CDec2(disc("DiscountAmount")) - DiscountRefund)
                        End If

                        description &= code & ", " & disc("DiscountType") & ", " & CDec2(disc("DiscountAmount")).ToString("N") & vbCrLf
                    Next
                End If

                pFS = New IO.FileStream(filename, IO.FileMode.Create)
                pBytes = New System.Text.ASCIIEncoding().GetBytes(description)
                pFS.Write(pBytes, 0, pBytes.Length)
                pFS.Close()
#End Region

#Region "ZRead"
                Dim dtDiscounts As DataTable = dtaDSaleTable
                Dim pDtOtherPayments = getOtherPaymentTypes(mProdata, dtDateFrom, dtDateTo, vFowstID, vDeviceID)
                Dim totalOtherPayments = 0
                dtaDSaleTable = getDailySales(mProdata, dtDateFrom, dtDateTo, vFowstID, vDeviceID) 'getZReadReport(mProdata, dtDate.ToString("MM/dd/yyyy"), dtDateTo.ToString("MM/dd/yyyy"), vFowstID, vDeviceID)

                Dim scdiscvatablesales As Decimal = 0, scdiscvatableamount As Decimal = 0

                If CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) > 0 Then
                    scdiscvatablesales = CDec2(CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) / 1.12)
                    scdiscvatableamount = CDec2(CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) - scdiscvatablesales)
                End If

                CashRefund = Roundoff(CDec2(GetAmount(mProdata, "period", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "cash")))
                OtherPaymentRefund = Roundoff(CDec2(GetAmount(mProdata, "period", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "otherpayment")))
                DiscountRefund = Roundoff(CDec2(GetAmount(mProdata, "period", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "discount")))
                DebitRefund = Roundoff(CDec2(GetAmount(mProdata, "period", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "debit")))
                CreditRefund = Roundoff(CDec2(GetAmount(mProdata, "period", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "credit")))

                dtaDSaleTable.Columns.Add("TotalVATType")
                dtaDSaleTable.Columns.Add("VATableSales")
                dtaDSaleTable.Columns.Add("VATAmount")
                dtaDSaleTable.Columns.Add("NonVAT")
                dtaDSaleTable.Columns.Add("VATExempt")
                dtaDSaleTable.Columns.Add("ZeroRatedVAT")
                dtaDSaleTable.Columns.Add("TotalOtherPayments")
                dtaDSaleTable.Columns.Add("CashRefund")
                dtaDSaleTable.Columns.Add("OtherPaymentRefund")
                dtaDSaleTable.Columns.Add("DiscountRefund")
                dtaDSaleTable.Columns.Add("DebitRefund")
                dtaDSaleTable.Columns.Add("CreditRefund")
                dtaDSaleTable.Rows(0).Item("TotalVATType") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSales")) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSalesSalesReturn"))) + (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVAT")) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmountSalesReturn"))) + CDec2(dtaDSaleTable.Rows(0).Item("CurNoTAXSales")) + CDec2(dtaDSaleTable.Rows(0).Item("CurVATExemptSales")), netsales)
                dtaDSaleTable.Rows(0).Item("VATableSales") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSales")) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSalesSalesReturn")), 0)
                dtaDSaleTable.Rows(0).Item("VATAmount") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVAT")) + scdiscvatableamount) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmountSalesReturn")), 0)
                dtaDSaleTable.Rows(0).Item("NonVAT") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurNoTAXSales")), netsales)
                dtaDSaleTable.Rows(0).Item("VATExempt") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurVATExemptSales")), 0)
                dtaDSaleTable.Rows(0).Item("ZeroRatedVAT") = 0
                dtaDSaleTable.Rows(0).Item("CashRefund") = CashRefund
                dtaDSaleTable.Rows(0).Item("OtherPaymentRefund") = OtherPaymentRefund
                dtaDSaleTable.Rows(0).Item("DiscountRefund") = DiscountRefund
                dtaDSaleTable.Rows(0).Item("DebitRefund") = DebitRefund
                dtaDSaleTable.Rows(0).Item("CreditRefund") = CreditRefund

                For Each op As Object In pDtOtherPayments.Rows
                    totalOtherPayments += CDec2(op("Amount"))
                Next

                dtaDSaleTable.Rows(0).Item("TotalOtherPayments") = CDec2(totalOtherPayments)

                'Dim pReport = New ProReport
                'pReport.SourceFilename = "NewZRead"
                'pReport.AddTable("Data", dtaDSaleTable)
                'pReport.AddTable("Header", pDtHeader)
                'pReport.AddTable("Discounts", dtDiscounts)
                'pReport.AddTable("OtherPayments", pDtOtherPayments)
                'pReport.ExportXmlFile()

                zreadname = folder & "\ZREAD " & dtDateFrom.ToString("MM-dd-yyyy") & ".pdf"
                'pReport.ExportFile(ProReport.eExportFormat.Pdf, zreadname)
#End Region

            End If
            'End If

            strRet = New With {Key .HFile = strHourlySalesFileText,
                                        .DFile = strDiscountFileText,
                                        .SFile = strDailySalesFileText,
                                        .ZRead = zreadname,
                                        .SalesDate = dtDate.ToString("MM/dd/yyyy")}
        Catch ex As Exception
            If (DBFConn.State = ConnectionState.Open) Then
                DBFConn.Close()
            End If
            strRet = ""
            AppendToDebug(ex.StackTrace)
        End Try

        Return strRet

    End Function

    Function Roundoff(ByVal amt As String)
        If amt.ToString().Substring(amt.ToString().IndexOf(".") + 1).Length = 4 Then
            amt = FormatNumber(amt, 3)
        End If

        Return CDec2(amt)
    End Function

    Function GetAmount(ByVal mProdata As ProData, ByVal cover As String, ByVal dtDateFrom As Date,
                       ByVal dtDateTo As Date,
                       ByVal vFowstID As String,
                       ByVal vDeviceID As String,
                       ByVal type As String,
                       Optional vUserID As String = Nothing)
        Dim pSql As String

        pSql = " SELECT ISNULL(SUM(" & vbCrLf

        If type = "discount" Then
            pSql &= " inv.SCDiscount "
        ElseIf type = "debit" Then
            pSql &= " custpay.DebitCardAmount "
        ElseIf type = "credit" Then
            pSql &= " custpay.CreditCardAmount "
        Else
            pSql &= " ledger.Amount " & IIf(type = "cash", "", "")
        End If

        pSql &= "), 0) as Total " & vbCrLf &
                " FROM TransSalesReturnLedger as ledger " & vbCrLf &
                " INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " & vbCrLf &
                " INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " & IIf(type = "cash", " and custpay.CashAmount > 0 ", " ") & vbCrLf

        If type = "otherpayment" Then
            pSql &= " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) "
        End If

        pSql &= " INNER JOIN TransInvoice inv ON (cuspayinv.InvoiceID = inv.ID) " & vbCrLf &
                " WHERE ledger.FowstID = " & ReadyString(vFowstID) & vbCrLf &
                " AND ledger.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
                " AND ISNULL(RefundNum, 0) > 0 " & vbCrLf

        If Not String.IsNullOrWhiteSpace(vUserID) Then
            pSql += " AND memo.UserID = " & ReadyString(vUserID) & vbCrLf
        End If

        If cover = "today" Then
            pSql &= " AND CONVERT(date, ledger.LastUpdated) = " & ReadyString(dtDateFrom) & vbCrLf
        ElseIf cover = "prev" Then
            pSql &= " AND CONVERT(date, ledger.LastUpdated) < " & ReadyString(dtDateFrom) & vbCrLf
        ElseIf cover = "period" Then
            pSql &= " AND CONVERT(date, ledger.LastUpdated) BETWEEN '" & dtDateFrom & "' AND '" & dtDateTo & "'" & vbCrLf
        End If

        Return mProdata.ExecuteScalar(pSql)
    End Function

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

                ' COMMENTED BY ARNIE 3-29-2025
                ' Kung mag sales refund og more than 1 item, bale 1 item lang makita sa TransMemoSalesReturn
                ' TO BE FIXED by Arvin
                'With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                '    .Expression = "ISNULL((SELECT SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)) " &
                '               " FROM TransMemoSalesReturn memosr " &
                '               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                '               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString( dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                '               " AND memo.FOWSTID = devf.FowstID " &
                '               " AND memo.DeviceID = devf.DeviceID) " &
                '               ",0)"
                'End With
                'With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                '    .Expression = "ISNULL((SELECT SUM(memosr.TaxAmount) " &
                '               " FROM TransMemoSalesReturn memosr " &
                '               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                '               " WHERE CONVERT(datetime, memo.LastUpdated) BETWEEN " & ReadyString( dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                '               " AND memo.FOWSTID = devf.FowstID " &
                '               " AND memo.DeviceID = devf.DeviceID) " &
                '               ",0)"
                'End With

                'MAO SA NI ANG WORKAROUND, bale sa TransSalesReturnLedger.Amount lang sa kwaon
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
                    .Expression = "ISNULL((SELECT ISNULL(ZReadCount, 1) FROM TransZReadCache WHERE ISNULL(ZReadCount, 0) > 0 AND FOWSTID = devf.FowstID AND DeviceID = devf.DeviceID AND CONVERT(date, SalesDate) = CONVERT(date, " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & ")), 1)"
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

    Sub dummy()

    End Sub

    Function getDailySalesRobinsons(ByVal mProdata As ProData, ByVal dtDateFrom As DateTime, ByVal dtDateTo As DateTime,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = mProdata

            With .Fields
                With .Add("TransDate", SqlDbType.DateTime)
                    .Expression = "'" & IIf(dtDateFrom = dtDateTo, dtDateFrom.ToString("MM/dd/yyyy"), dtDateFrom.ToString("MM/dd/yyyy") & " - " & dtDateTo.ToString("MM/dd/yyyy")) & "'"
                    .Searchable = False
                End With
                With .Add("BegBal", SqlDbType.Money)
                    .Expression = " ISNULL(devf.BegBalance,0) "
                End With
                With .Add("PrevGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)" & vbCrLf
                    '      " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015

                End With
                With .Add("PrevTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(custpay.DiscountAmount) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID" & vbCrLf &
                                " AND LibraryDiscountSettings.DiscountType NOT IN ('Person With Disability Discount', 'Senior Citizen Discount')" & vbCrLf &
                                " WHERE inv.InvoiceNumber > 0 And custpay.ReceiptType <> 'RF' AND ISNULL(disc.NoOfSeniorCitizens, 0) =  0 " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevTotalSCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.SCDiscount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE CONVERT(date, memo.LastUpdated) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE CONVERT(date, memo.LastUpdated) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevTotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)) " & vbCrLf &
                               " FROM TransInvoiceItems invitem " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) " & vbCrLf &
                               " - " & vbCrLf &
                               "ISNULL((SELECT SUM(sr.TaxAmount) " & vbCrLf &
                               " FROM TransMemoSalesReturn sr " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (sr.MemoID = memo.ID) " & vbCrLf &
                               " WHERE CONVERT(date, memo.LastUpdated) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " & vbCrLf &
                                  " FROM TransSalesReturnLedger srledger " & vbCrLf &
                                  " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                                  " WHERE CONVERT(date, memo.LastUpdated)  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                                  " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                                  " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                                  ",0) +" & vbCrLf &
                                 "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " & vbCrLf &
                                 " FROM TransInvoice inv " & vbCrLf &
                                 " INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " & vbCrLf &
                                 " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                                 " AND CONVERT(date, inv.InvoiceDate)  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                                 " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                                 " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                                  ",0)"
                End With
                With .Add("PrevChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("PrevVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
                               " FROM TransInvoiceItems invitem " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurGrossSalesExcludingSenior", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
                               " FROM TransInvoiceItems invitem " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND ISNULL(inv.SCDiscount, 0) = 0 " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                .Add("CurTotalDiscount", SqlDbType.Money, "ISNULL(tblDiscount.Sum, 0)")
                .Add("CurDiscountCount", SqlDbType.Int, "ISNULL(tblDiscount.Count, 0)")

                .Add("CurTotalScDiscount", SqlDbType.Money, "ISNULL(tblScDiscount.Sum, 0)")
                .Add("CurDiscountScCount", SqlDbType.Int, "ISNULL(tblScDiscount.Count, 0)")

                .Add("CurSalesReturnRefund", SqlDbType.Money, "ISNULL(tblRefund.Sum, 0)")
                .Add("CurSalesReturnRefundCount", SqlDbType.Int, "ISNULL(tblRefund.Count, 0)")

                .Add("CurSalesReturnRefund2", SqlDbType.Money, "ISNULL(tblRefund2.Sum, 0)")
                .Add("CurSalesReturnRefundCount2", SqlDbType.Int, "ISNULL(tblRefund2.Count, 0)")

                .Add("ErrorCorrectAmount", SqlDbType.Money, "ISNULL(tblInvoice.ErrorCorrectAmount, 0)")
                .Add("ErrorCorrectCount", SqlDbType.Int, "ISNULL(tblInvoice.ErrorCorrectCount, 0)")

                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTotalVAT", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)) " &
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)- ISNULL(invitem.DiscountTax,0)) " & vbCrLf &
                               " FROM --TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " --INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " --INNER JOIN " & vbCrLf &
                               " TransInvoice inv --ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 --AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " and (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL) " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurBegInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MIN(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurEndInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MAX(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountMemoReturnExchange", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
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
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
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
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchangeItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
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
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTotalVatablePortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.Vatable) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTransCountMemo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memo.MemoNumber) " & vbCrLf &
                               " FROM TransMemo memo " & vbCrLf &
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurLocalTax", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " & vbCrLf &
                                              " FROM TransInvoice inv " & vbCrLf &
                                              " INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " & vbCrLf &
                                              " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                                              " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                                              " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                                              " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                                              ",0)"
                End With
                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " & vbCrLf &
                                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                                               " WHERE CONVERT(date, memo.LastUpdated)  BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                                               ",0) +" & vbCrLf &
                                              "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " & vbCrLf &
                                              " FROM TransInvoice inv " & vbCrLf &
                                              " INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " & vbCrLf &
                                              " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                                              " AND CONVERT(date, inv.InvoiceDate)  BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                                              " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                                              " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                                               ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.NonVATSale) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurVATExemptSales", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - invitem.DiscountAmount) " &
                    .Expression = "ISNULL((SELECT SUM(inv.VATExempt - inv.SCDiscount) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' ) AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    'AND invitem.TaxAmount = 0 AND ISNULL(invitem.VATableSale,0) = 0 
                End With
                With .Add("CurVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
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
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevCashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevDebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevCreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("PrevCheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
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
                            " WHERE CONVERT(date, inv.InvoiceDate) = " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
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
                            " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
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
                '               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
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
                              " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
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
                              " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
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
                            " WHERE inv.InvoiceNumber > 0  AND CONVERT(date, inv.InvoiceDate) = " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
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
                            " WHERE CONVERT(date, inv.InvoiceDate) < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & vbCrLf &
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
                With .Add("TotalSCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.SCDiscount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
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
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("OnCreditAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OnCreditAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSales", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)) " &
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)- ISNULL(invitem.DiscountSales,0)) " & vbCrLf &
                               " FROM --TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " --INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " --INNER JOIN " & vbCrLf &
                               " TransInvoice inv --ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  --AND custpay.ReceiptType <> 'RF'" & vbCrLf &
                               " and (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL) " & vbCrLf &
                               " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(memosr.TaxAmount) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("ZCount", SqlDbType.Int)
                    .Expression = "devf.ZReadCount"
                End With
                With .Add("ReprintedAmount", SqlDbType.Money)
                    .Expression = "ISNULL(tblReprint.Sum, 0)"
                End With
                With .Add("ReprintedCount", SqlDbType.Int)
                    .Expression = "ISNULL(tblReprint.Count, 0)"
                End With
                With .Add("RobinsonTenantId", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.RobinsonTenantId"
                End With
                With .Add("RobinsonPosTerminalNum", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.RobinsonPosTerminalNum"
                End With
                With .Add("SeniorAndPwdVatExempt", SqlDbType.Money)
                    .Expression = "ISNULL(tblInvoiceWithScAndPwdDiscount.VATExempt, 0)"
                End With
                With .Add("SeniorAndPwdDiscount", SqlDbType.Money)
                    .Expression = "ISNULL(tblInvoiceWithScAndPwdDiscount.SCDiscount, 0)"
                End With
                With .Add("SeniorOnlyVatExempt", SqlDbType.Money)
                    .Expression = "ISNULL(tblInvoiceWithScOnlyDiscount.VATExempt, 0)"
                End With
                With .Add("SeniorOnlyDiscount", SqlDbType.Money)
                    .Expression = "ISNULL(tblInvoiceWithScOnlyDiscount.SCDiscount, 0)"
                End With
                With .Add("PwdOnlyVatExempt", SqlDbType.Money)
                    .Expression = "ISNULL(tblInvoiceWithPwdOnlyDiscount.VATExempt, 0)"
                End With
                With .Add("PwdOnlyDiscount", SqlDbType.Money)
                    .Expression = "ISNULL(tblInvoiceWithPwdOnlyDiscount.SCDiscount, 0)"
                End With
                With .Add("CreditCardPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL(tblCreditCardPayment.Amount, 0)"
                End With
                With .Add("CreditCardPaymentCount", SqlDbType.Money)
                    .Expression = "ISNULL(tblCreditCardPayment.Count, 0)"
                End With
                With .Add("CreditCardPaymentVat", SqlDbType.Money)
                    .Expression = "ISNULL(tblCreditCardPayment.CreditCardVat, 0)"
                End With
            End With

            .SqlFrom = " -- main FROM" & vbCrLf &
                " FROM LibraryDeviceFowst devf " & vbCrLf &
                " INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) " & vbCrLf &
                " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = devf.FowstID " & vbCrLf

            .SqlFrom &= " OUTER APPLY(" & vbCrLf &
                "SELECT SUM(custpay.DiscountAmount) AS Sum " & vbCrLf &
                ",COUNT(DISTINCT custpayinv.InvoiceID) AS Count " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " & vbCrLf &
                " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID" & vbCrLf &
                " AND LibraryDiscountSettings.DiscountType NOT IN ('Person With Disability Discount', 'Senior Citizen Discount')" & vbCrLf &
                " WHERE inv.InvoiceNumber > 0 And custpay.ReceiptType <> 'RF' AND ISNULL(disc.NoOfSeniorCitizens, 0) =  0 " & vbCrLf &
                " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                " AND inv.DeviceID = devf.DeviceID" & vbCrLf &
                ") tblDiscount" & vbCrLf

            .SqlFrom &= " OUTER APPLY(" & vbCrLf &
                "SELECT SUM(custpay.DiscountAmount) AS Sum " & vbCrLf &
                ",COUNT(DISTINCT custpayinv.InvoiceID) AS Count " & vbCrLf &
                " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " & vbCrLf &
                " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID" & vbCrLf &
                " WHERE inv.InvoiceNumber > 0 And custpay.ReceiptType <> 'RF' AND ISNULL(disc.NoOfSeniorCitizens, 0) > 0" & vbCrLf &
                " AND LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount' " & vbCrLf &
                " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                " AND inv.DeviceID = devf.DeviceID" & vbCrLf &
                ") tblScDiscount" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(srledger.GrossRefundAmount) AS Sum" & vbCrLf &
                "   ,COUNT(DISTINCT memo.id) AS Count" & vbCrLf &
                "  	FROM TransSalesReturnLedger srledger" & vbCrLf &
                "  	JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                "   WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                "  	AND memo.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND memo.DeviceID = devf.DeviceID" & vbCrLf &
                " 	AND srledger.GrossRefundAmount > 0" & vbCrLf &
                " ) tblRefund" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(srledger.GrossRefundAmount) AS Sum" & vbCrLf &
                "   ,COUNT(DISTINCT srledger.id) AS Count" & vbCrLf &
                "  	FROM TransSalesReturnLedger srledger" & vbCrLf &
                "  	JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = srledger.SourceMemoID" & vbCrLf &
                "  	JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID" & vbCrLf &
                "   WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                "  	AND TransInvoice.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND TransInvoice.DeviceID = devf.DeviceID" & vbCrLf &
                " 	AND srledger.GrossRefundAmount > 0" & vbCrLf &
                " ) tblRefund2" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoice.TotalAmount) AS Sum" & vbCrLf &
                "   	,COUNT(DISTINCT TransInvoice.ID) AS Count" & vbCrLf &
                "  	FROM TransInvoice" & vbCrLf &
                "  	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                "  	AND TransInvoice.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND TransInvoice.DeviceID = devf.DeviceID" & vbCrLf &
                " 	AND TransInvoice.PrintCount > 1" & vbCrLf &
                " ) tblReprint" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoice.ErrorCorrectAmount) AS ErrorCorrectAmount" & vbCrLf &
                "   ,SUM(TransInvoice.ErrorCorrectCount) AS ErrorCorrectCount" & vbCrLf &
                "  	FROM TransInvoice" & vbCrLf &
                "  	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                "  	AND TransInvoice.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND TransInvoice.DeviceID = devf.DeviceID" & vbCrLf &
                " ) tblInvoice" & vbCrLf

            .SqlFrom &= "  OUTER APPLY (" & vbCrLf &
                "  	SELECT SUM(TransInvoice.VATExempt) AS VATExempt" & vbCrLf &
                "   ,SUM(TransInvoice.SCDiscount) AS SCDiscount" & vbCrLf &
                "   FROM TransInvoice" & vbCrLf &
                "  	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                "   AND TransInvoice.FOWSTID = devf.FowstID" & vbCrLf &
                "   AND TransInvoice.DeviceID = devf.DeviceID" & vbCrLf &
                "   AND TransInvoice.SCDiscount > 0" & vbCrLf &
                "  ) tblInvoiceWithScAndPwdDiscount" & vbCrLf

            .SqlFrom &= "  OUTER APPLY (" & vbCrLf &
                "  	SELECT SUM(TransInvoice.VATExempt) AS VATExempt" & vbCrLf &
                "   ,SUM(TransInvoice.SCDiscount) AS SCDiscount" & vbCrLf &
                "   FROM TransInvoice" & vbCrLf &
                " INNER JOIN TransInvoiceDiscount ON TransInvoiceDiscount.InvoiceID = TransInvoice.ID " & vbCrLf &
                " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID" & vbCrLf &
                "  	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                " AND LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount' " & vbCrLf &
                "   AND TransInvoice.FOWSTID = devf.FowstID" & vbCrLf &
                "   AND TransInvoice.DeviceID = devf.DeviceID" & vbCrLf &
                "   AND TransInvoice.SCDiscount > 0" & vbCrLf &
                "  ) tblInvoiceWithScOnlyDiscount" & vbCrLf

            .SqlFrom &= "  OUTER APPLY (" & vbCrLf &
                "  	SELECT SUM(TransInvoice.VATExempt) AS VATExempt" & vbCrLf &
                "   ,SUM(TransInvoice.SCDiscount) AS SCDiscount" & vbCrLf &
                "   FROM TransInvoice" & vbCrLf &
                " INNER JOIN TransInvoiceDiscount ON TransInvoiceDiscount.InvoiceID = TransInvoice.ID " & vbCrLf &
                " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID" & vbCrLf &
                "  	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                " AND LibraryDiscountSettings.DiscountType = 'Person With Disability Discount' " & vbCrLf &
                "   AND TransInvoice.FOWSTID = devf.FowstID" & vbCrLf &
                "   AND TransInvoice.DeviceID = devf.DeviceID" & vbCrLf &
                "   AND TransInvoice.SCDiscount > 0" & vbCrLf &
                "  ) tblInvoiceWithPwdOnlyDiscount" & vbCrLf

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(custpay.CreditCardAmount) AS Amount" & vbCrLf &
                "   ,COUNT(DISTINCT custpay.id) AS Count" & vbCrLf &
                "   ,SUM(custpay.CreditCardVat) AS CreditCardVat" & vbCrLf &
                " 	FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                " 	JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                " 	JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                "  	WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy")) & vbCrLf &
                "   AND custpay.CreditCardAmount > 0" & vbCrLf &
                " 	AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                "   AND inv.DeviceID = devf.DeviceID" & vbCrLf &
                ") tblCreditCardPayment" & vbCrLf

            .SqlWhere = " WHERE devf.FowstID = " & vbCrLf & ReadyString(vFowstID) & vbCrLf &
                        " AND devf.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
                        " /* vbA7XJ7CTDKZ vb92AQQE34NK */"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDailyHourlySales(ByVal mProdata As ProData, ByVal dtDateFrom As String, ByVal dtDateTo As String,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = mProdata

            With .Fields
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
                               " FROM TransInvoiceItems invitem " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND inv.InvoiceDate BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND inv.InvoiceDate BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE memo.LastUpdated BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE memo.LastUpdated BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)) " & vbCrLf &
                               " FROM TransInvoiceItems invitem " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND inv.InvoiceDate BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) " & vbCrLf &
                               " - " & vbCrLf &
                               "ISNULL((SELECT SUM(sr.TaxAmount) " & vbCrLf &
                               " FROM TransMemoSalesReturn sr " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (sr.MemoID = memo.ID) " & vbCrLf &
                               " WHERE memo.LastUpdated BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " & vbCrLf &
                               " FROM TransInvoice inv " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                               " AND inv.InvoiceDate BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT sum(invitems.GrossSalesNetOfVATExemptions - invitems.DiscountAmount) " & vbCrLf &
                             " FROM TransInvoiceItems invitems " & vbCrLf &
                             " INNER JOIN TransInvoice inv ON (invitems.InvoiceID = inv.ID)  " & vbCrLf &
                             " WHERE (inv.InvoiceNumber > 0) AND invitems.TaxAmount = 0 AND ISNULL(invitems.VATableSale,0) = 0 " & vbCrLf &
                             " AND inv.InvoiceDate  BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                             " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                             " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate  BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " & vbCrLf &
                                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                                               " WHERE memo.LastUpdated  BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                                               ",0) +" & vbCrLf &
                                              "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " & vbCrLf &
                                              " FROM TransInvoice inv " & vbCrLf &
                                              " INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " & vbCrLf &
                                              " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
                                              " AND inv.InvoiceDate  BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                                              " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                                              " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                                               ",0)"
                End With
                With .Add("TotalSCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.SCDiscount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE inv.InvoiceDate BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
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
                               " AND inv.InvoiceDate BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With
            End With

            .SqlFrom = " FROM LibraryDeviceFowst devf " & vbCrLf &
                        " INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(vFowstID) & vbCrLf &
                        " AND devf.DeviceID = " & ReadyString(vDeviceID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDailyDiscountBreakdown(ByVal mProdata As ProData, ByVal dtDateFrom As DateTime, ByVal dtDateTo As DateTime,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing,
        Optional vUserID As String = Nothing) As DataTable
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
                With .Add("ID", SqlDbType.Money)
                    .Expression = " disc.DiscountTypeID " & vbCrLf
                End With
                With .Add("DiscountTypeID", SqlDbType.Money)
                    .Expression = " disc.DiscountTypeID " & vbCrLf
                End With
                With .Add("DiscountType", SqlDbType.Money)
                    .Expression = " ISNULL(libdisc.DiscountType, cusdisc.Name) " & vbCrLf
                End With
                With .Add("DiscountAmount", SqlDbType.Money)
                    .Expression = " SUM(ROUND(custpay.DiscountAmount, 2)) " & vbCrLf
                End With
            End With

            .SqlFrom = " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID  " & vbCrLf &
                               " LEFT JOIN LibraryCustomDiscounts as cusdisc ON cusdisc.ID = disc.DiscountTypeID  " & vbCrLf

            .SqlWhere = " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & vbCrLf & ReadyString(dtDateFrom) & vbCrLf & " AND " & vbCrLf & ReadyString(dtDateTo) & vbCrLf &
                               " AND inv.FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
                               " AND inv.DeviceID = " & ReadyString(vDeviceID) & vbCrLf

            If Not String.IsNullOrWhiteSpace(vUserID) Then
                .SqlWhere += " AND inv.UserID = " & ReadyString(vUserID) & vbCrLf
            End If

            .SqlGroup = " GROUP BY disc.DiscountTypeID, libdisc.DiscountType, cusdisc.Name "
            .SqlOrder = " ORDER BY libdisc.DiscountType ASC "

            .ProData.CommandTimeout = 6000
            pDT = .Requery

        End With

        Return pDT
    End Function

    Function getZReadReport(ByVal mProdata As ProData, ByVal dtDateFrom As DateTime, ByVal dtDateTo As DateTime,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing)

        Dim pSql As String

        pSql = "Select top 1 *, " & vbCrLf &
            " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
            " FROM TransInvoiceItems invitem " & vbCrLf &
            " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
            " WHERE inv.InvoiceNumber > 0 " & vbCrLf &
            " AND inv.InvoiceDate < " & vbCrLf & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
            " AND inv.FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
            " AND inv.DeviceID = " & ReadyString(vDeviceID) & ") " & vbCrLf &
            ",0) as NewPrevGrossSales " & vbCrLf &
            " from TransZReadCache " & vbCrLf &
            " WHERE FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
            " AND convert(date, SalesDate) = " & ReadyString(dtDateFrom) & vbCrLf &
            " AND DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " Order by SalesDate desc"

        Return mProdata.Execute(pSql)
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

    Function GenerateRobinsonReport(ByVal mProdata As ProData, ByVal dtDate As DateTime, Optional vOrganizationID As String = Nothing,
        Optional vFowstID As String = Nothing,
        Optional vDeviceID As String = Nothing)

        Try
            Dim strDeviceDir As String

            strDeviceDir = Common.ServerPath & "Content\Robinson"
            AppendToDebug("Path: " & strDeviceDir)
            If Not IO.Directory.Exists(strDeviceDir) Then
                IO.Directory.CreateDirectory(strDeviceDir)
            End If

            strDeviceDir = strDeviceDir & "\" & vOrganizationID

            If Not IO.Directory.Exists(strDeviceDir) Then
                IO.Directory.CreateDirectory(strDeviceDir)
            End If

            strDeviceDir = strDeviceDir & "\" & vFowstID

            If Not IO.Directory.Exists(strDeviceDir) Then
                IO.Directory.CreateDirectory(strDeviceDir)
            End If

            strDeviceDir = strDeviceDir & "\" & vDeviceID

            Dim folder As String = strDeviceDir
            Dim filename As String
            If Not System.IO.Directory.Exists(folder) Then
                System.IO.Directory.CreateDirectory(folder)
            End If

            Dim dtDateFrom As DateTime
            Dim dtDateTo As DateTime
            Dim tenantlog As DataRow = mProdata.ExecuteRow("SELECT ID, BatchNo FROM TenantMonitoringLogs" & vbCrLf &
                " WHERE FOWSTID = " & ReadyString(vFowstID) & vbCrLf &
                " And CONVERT(date, SalesDate) = " & ReadyString(dtDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
                " AND DeviceID = " & ReadyString(vDeviceID))
            Dim pBatchNum As Integer = CInt2(tenantlog("BatchNo"))
            Dim pFS As IO.FileStream
            Dim pBytes() As Byte
            'Dim pIU As New InsertUpdate("TenantMonitoringLogs")
            'pIU("BatchNo") = batchnumber
            'mProdata.Execute(pIU.GetUpdate(" ID = " & ReadyString(tenantlog("ID").ToString())))

            dtDateFrom = dtDate.ToString("MM/dd/yyyy")
            dtDateTo = dtDate.ToString("MM/dd/yyyy")
            If (dtDateFrom > dtDateTo) Then
                dtDateTo = dtDateTo.AddDays(1)
            End If

            Dim dtaDSaleTable As DataTable = getDailySalesRobinsons(mProdata, dtDateFrom, dtDateTo, vFowstID, vDeviceID)

            If dtaDSaleTable.Rows.Count > 0 Then
                Dim dRowSales = dtaDSaleTable.Rows(0)
                Dim description As String = ""
                Dim netsales As String = Roundoff(dRowSales("CurGrossSales") - (dRowSales("TotalSCDiscount") + dRowSales("CurTotalDiscount")) - dRowSales("CurSalesReturnRefund") - dRowSales("CurSalesReturnExchange") - dRowSales("CurVATExempt") + dRowSales("CurServiceCharge"))
                Dim prevnetsales As String = Roundoff(dRowSales("PrevGrossSales") - (dRowSales("PrevTotalSCDiscount") + dRowSales("PrevTotalDiscount")) - dRowSales("PrevSalesReturnRefund") - dRowSales("PrevSalesReturnExchange") - dRowSales("PrevVATExempt") + dRowSales("PrevServiceCharge")) ' + dRowSales("PrevChargeAmount")
                Dim CashRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "cash")))
                Dim OtherPaymentRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "otherpayment")))
                Dim DiscountRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "discount")))
                Dim DebitRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "debit")))
                Dim CreditRefund = Roundoff(CDec2(GetAmount(mProdata, "prev", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "credit")))
                Dim prevtotalpayments = (CDec2(dRowSales("PrevCashAmount")) + (CDec2(dRowSales("PrevDebitCardAmount")) + CDec2(dRowSales("PrevCreditCardAmount"))) + (CDec2(dRowSales("PrevCheckPaymentAmount")) + CDec2(dRowSales("PrevOthers")))) - (CashRefund + OtherPaymentRefund)

                CashRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "cash")))
                OtherPaymentRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "otherpayment")))
                DiscountRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "discount")))
                DebitRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "debit")))
                CreditRefund = Roundoff(CDec2(GetAmount(mProdata, "today", dtDateFrom, dtDateTo, vFowstID, vDeviceID, "credit")))

                Dim totalpayments = (CDec2(dRowSales("CashAmount")) + (CDec2(dRowSales("DebitCardAmount")) + CDec2(dRowSales("CreditCardAmount"))) + (CDec2(dRowSales("CheckPaymentAmount")) + CDec2(dRowSales("CurOthers")))) - (CashRefund + OtherPaymentRefund)

                netsales = Roundoff(netsales)
                prevnetsales = Roundoff(prevnetsales)
                totalpayments = Roundoff(totalpayments)

                Dim scdiscount = Roundoff(CDec2(dRowSales("TotalSCDiscount").ToString()))
                Dim otherdiscount = Roundoff(CDec2(dRowSales("CurTotalDiscount").ToString()))
                Dim salesrefund = Roundoff(CDec2(dRowSales("CurSalesReturnRefund").ToString()))  ' CurSalesReturnRefund is based on refund date
                Dim pSalesRefund2 = Roundoff(CDec2(dRowSales("CurSalesReturnRefund2").ToString())) ' CurSalesReturnRefund2 is based on invoice date
                Dim pRlc03 = CDec2(dRowSales("CurGrossSalesExcludingSenior")) + CDec2(dRowSales("SeniorAndPwdVatExempt")) - pSalesRefund2
                Dim pRlc11 = CDec2(dRowSales("CurTotalScDiscount"))
                dRowSales("SeniorAndPwdDiscount") = Math.Round(CDec2(dRowSales("SeniorAndPwdDiscount")), 2)
                Dim pRlc24 = CDec2(dRowSales("SeniorAndPwdDiscount")) / 0.2 * 0.8
                Dim pRlc27 = CDec2(dRowSales("PwdOnlyDiscount"))
                Dim pRlc04 = (pRlc03 - pRlc11 - pRlc24 - pRlc27) / 1.12 * 0.12
                Dim EOD As DataRow = mProdata.ExecuteRow(" SELECT " & vbCrLf &
                                    "ISNULL((SELECT TOP 1 EndOfDayCounter FROM TenantMonitoringLogs WHERE CONVERT(date, SalesDate) < " & ReadyString(dtDate.ToString("MM-dd-yyyy")) & " And FOWSTID = " & ReadyString(vFowstID) & "  And DeviceID = " & ReadyString(vDeviceID) & " ORDER BY SalesDate DESC), 0) as PrevEOD," & vbCrLf &
                                    "       (SELECT TOP 1 EndOfDayCounter FROM TenantMonitoringLogs WHERE CONVERT(date, SalesDate) = " & ReadyString(dtDate.ToString("MM-dd-yyyy")) & " And FOWSTID = " & ReadyString(vFowstID) & "  And DeviceID = " & ReadyString(vDeviceID) & ") as CurEOD")

                'description &= "01" & (CStr2(dRowSales("RobinsonTenantId")) & Space(16)).Substring(0, 16) & vbCrLf
                description &= "01" & ("0000000050093432" & Space(16)).Substring(0, 16) & vbCrLf
                'description &= "02" & (CStr2(dRowSales("RobinsonPosTerminalNum")) & Space(16)).Substring(0, 16) & vbCrLf
                description &= "02" & ("0000000000000001" & Space(16)).Substring(0, 16) & vbCrLf
                description &= "03" & Format(pRlc03, "0000000000000.00") & vbCrLf
                description &= "04" & Format(pRlc04, "0000000000000.00") & vbCrLf ' 
                description &= "05" & Format(0, "0000000000000.00") & vbCrLf
                description &= "06" & Format(0, "0000000000000000") & vbCrLf
                description &= "07" & Format(CDec2((0 - DiscountRefund) + otherdiscount), "0000000000000.00") & vbCrLf
                description &= "08" & Format(CDec2(dRowSales("CurDiscountCount")), "0000000000000000") & vbCrLf 'to do done
                description &= "09" & Format(CDec2(salesrefund), "0000000000000.00") & vbCrLf
                description &= "10" & Format(CDec2(dRowSales("CurSalesReturnRefundCount")), "0000000000000000") & vbCrLf 'to do done
                description &= "11" & Format(pRlc11, "0000000000000.00") & vbCrLf
                description &= "12" & Format(CDec2(dRowSales("CurDiscountScCount")), "0000000000000000") & vbCrLf
                description &= "13" & Format(CDec2(dRowSales("CurServiceCharge")), "0000000000000.00") & vbCrLf
                description &= "14" & Format(CDec2(EOD("PrevEOD")), "0000000000000000") & vbCrLf
                description &= "15" & Format(CDec2(prevnetsales), "0000000000000.00") & vbCrLf
                description &= "16" & Format(CDec2(EOD("CurEOD")), "0000000000000000") & vbCrLf
                description &= "17" & Format(CDec2(prevnetsales) + CDec2(netsales), "0000000000000.00") & vbCrLf
                description &= "18" & Format(CDec2(dtDate.ToString("MMddyyyy")), "00000000/00/0000") & vbCrLf
                description &= "19" & Format(CDec2(0), "0000000000000.00") & vbCrLf 'todo
                description &= "20" & Format(CDec2(0), "0000000000000.00") & vbCrLf 'todo
                description &= "21" & Format(0, "0000000000000.00") & vbCrLf
                description &= "22" & Format(CDec2(dRowSales("CreditCardPaymentAmount")), "0000000000000.00") & vbCrLf 'Total Credit Sales
                'description &= "23" & Format(CDec2(dRowSales("CreditCardPaymentVat")), "0000000000000.00") & vbCrLf 'Total Credit TaxNat ' mali daw ni 
                description &= "23" & Format(CDec2(dRowSales("CreditCardPaymentAmount")) / 1.12 * 0.12, "0000000000000.00") & vbCrLf 'Total Credit Sales
                'description &= "24" & Format(CDec2(dRowSales("SeniorOnlyVatExempt")) - CDec2(dRowSales("SeniorOnlyDiscount")), "0000000000000.00") & vbCrLf ' mali daw ni
                description &= "24" & Format(pRlc24, "0000000000000.00") & vbCrLf
                description &= "25" & Format(CDec2(0), "0000000000000.00") & vbCrLf 'todo
                description &= "26" & Format(CDec2(0), "0000000000000.00") & vbCrLf 'todo
                description &= "27" & Format(pRlc27, "0000000000000.00") & vbCrLf 'PWD discount
                description &= "28" & Format(CDec2(0), "0000000000000.00") & vbCrLf 'todo
                description &= "29" & Format(CDec2(dRowSales("ReprintedAmount")), "0000000000000.00") & vbCrLf 'todo
                description &= "30" & Format(CDec2(dRowSales("ReprintedCount")), "0000000000000000") & vbCrLf 'todo

                Dim pSql = "SELECT OrganizationID" & vbCrLf &
                    ",FOWSTMallName" & vbCrLf &
                    ",FtpServer" & vbCrLf &
                    ",FtpUsername" & vbCrLf &
                    ",FtpPassword" & vbCrLf &
                    ",RobinsonTenantId" & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                    " WHERE ID = " & ReadyString(vFowstID)
                Dim pDrFowst = mProdata.ExecuteRow(pSql)
                If pDrFowst Is Nothing Then
                    Throw New ApplicationException("The specified fowst does Not exist. ref: vbA8RRK3U2D7!")
                End If
                If pBatchNum < 1 Then
                    pBatchNum = 1
                Else
                    pBatchNum += 1
                End If
                Dim pDestinationFilename = CStr2(pDrFowst("RobinsonTenantId"))
                If pDestinationFilename.Length < 4 Then
                    pDestinationFilename = "3432" ' default
                Else
                    pDestinationFilename = pDestinationFilename.Substring(pDestinationFilename.Length - 4, 4)
                End If
                pDestinationFilename &= dtDate.ToString("MM") & dtDate.ToString("dd") & ".01" & pBatchNum
                Dim pIU = New InsertUpdate("TenantMonitoringLogs")
                pIU("Filename") = ReadyString(pDestinationFilename)
                pSql = pIU.GetUpdate("ID = " & ReadyString(tenantlog("ID")))
                mProdata.ExecuteSQL(pSql)
                filename = folder & "\" & pDestinationFilename
                pFS = New IO.FileStream(filename, IO.FileMode.Create)
                pBytes = New System.Text.ASCIIEncoding().GetBytes(description)
                pFS.Write(pBytes, 0, pBytes.Length)
                pFS.Close()
                Return pDestinationFilename
            End If
            Return ""
        Catch ex As Exception
            Throw
            AppendToDebug(ex.StackTrace)
            Return ex.Message
        End Try
    End Function
End Class
