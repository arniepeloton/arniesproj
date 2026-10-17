
Public Class MallLessorAyala


    Function getAyalaTenantInfo(Optional vFowstID As String = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ayl.ID"
                    .Searchable = False
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "ayl.FOWSTID"
                End With
                With .Add("TenantCode", SqlDbType.VarChar)
                    .Expression = "ayl.TenantCode"
                End With
                With .Add("TenantName", SqlDbType.VarChar)
                    .Expression = "ayl.TenantName"
                End With
                With .Add("TenantNameForDBF", SqlDbType.VarChar)
                    .Expression = "ayl.TenantNameForDBF"
                End With
                With .Add("MerchantsLeaseOutNumber", SqlDbType.VarChar)
                    .Expression = "ayl.MerchantsLeaseOutNumber"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWSTMallInfoAyala ayl "

            .SqlWhere = " WHERE ayl.FOWSTID = " & ReadyString(vFowstID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getFOWSTOperationHR(ByVal dtDate As Date, Optional vFowstID As String = Nothing) As DataTable
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

    Function GenerateLessorSalesReport(ByVal dtDate As DateTime, ByVal strTenantCode As String, ByVal strTenantName As String, ByVal strTenantNameForDBF As String, MerchantsLeaseOutNumber As String,
        Optional vOrganizationID As String = Nothing,
        Optional vFowstID As String = Nothing,
        Optional ByRef oError As String = Nothing) As String

        Dim strRet As String = ""
        Dim dtaDSaleTable As DataTable
        Dim dtaHSaleTable As DataTable
        Dim dtaSchedTable As DataTable
        Dim dtDateFrom As DateTime
        Dim dtDateTo As DateTime
        Dim strAyalaFolder As String
        Dim strDailyTempFile As String
        Dim strHourlyTempFile As String

        Dim strDeviceDir As String
        Dim strDailySalesFile As String
        Dim strDailySalesFileText As String
        Dim strHourlySalesFile As String
        Dim strHourlySalesFileText As String
        Dim strDailySalesFileName As String
        Dim strDailySalesFileNameText As String
        Dim strHourlySalesFileName As String
        Dim strHourlySalesFileNameText As String
        Dim strSQL As String
        Dim strDeviceIDs As String
        Dim DBFConn As New OleDb.OleDbConnection()

        Try
            oError = Nothing
            If vOrganizationID Is Nothing Then vOrganizationID = WebRequest.Instance.DeviceID.Trim()

            strAyalaFolder = Common.ServerPath & "Content\AyalaTenant"
            strDailyTempFile = strAyalaFolder & "\XXXMMDD.dbf"
            strHourlyTempFile = strAyalaFolder & "\XXXMMDDH.dbf"

            '''Set Directory and File Name
            strDeviceDir = strAyalaFolder & "\" & vOrganizationID

            If (IO.Directory.Exists(strDeviceDir)) Then
                For Each pFilename As String In IO.Directory.GetFiles(strDeviceDir, "*", IO.SearchOption.AllDirectories)
                    IO.File.SetAttributes(pFilename, IO.FileAttributes.Normal)
                Next pFilename
                IO.Directory.Delete(strDeviceDir, True)
            End If
            IO.Directory.CreateDirectory(strDeviceDir)

            If strTenantCode.Length < 3 Then
                oError = "Tenant code must be at least 3 characters long."
                Return Nothing
            End If
            strDailySalesFileName = strTenantCode.Substring(0, 3) & dtDate.ToString("MMdd") & ".dbf"
            strDailySalesFile = strDeviceDir & "\" & strDailySalesFileName

            strDailySalesFileNameText = MerchantsLeaseOutNumber & dtDate.ToString("MMdd") & ".csv"
            strDailySalesFileText = strDeviceDir & "\" & strDailySalesFileNameText

            dtaSchedTable = getFOWSTOperationHR(dtDate, vFowstID)

            If (dtaSchedTable.Rows.Count > 0) Then

                ''' Set the Opening and closing DateTime
                dtDateFrom = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & dtaSchedTable.Rows(0).Item("OpeningTime").ToString())
                dtDateTo = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & dtaSchedTable.Rows(0).Item("ClosingTime").ToString())
                If (dtDateFrom > dtDateTo) Then
                    dtDateTo = dtDateTo.AddDays(1)
                End If

                ''' Get the Daily sales for the period
                dtaDSaleTable = getDailySales(dtDateFrom, dtDateTo, vFowstID)
                If (dtaDSaleTable.Rows.Count > 0) Then

                    '''Generate Daily Sales
                    If (IO.File.Exists(strDailyTempFile)) Then

                        '''Delete if destination file exist
                        If (IO.File.Exists(strDailySalesFile)) Then
                            IO.File.Delete(strDailySalesFile)
                        End If

                        CreateFolder(strDailySalesFile)
                        IO.File.Copy(strDailyTempFile, strDailySalesFile)
                        IO.File.SetAttributes(strDailySalesFile, IO.FileAttributes.Normal)
                        'IO.File.Create(strDailySalesFileText)
                        'IO.File.SetAttributes(strDailySalesFileText, IO.FileAttributes.Normal)
                        '''After Copy, dump the daily sales to dbf file
                        If (IO.File.Exists(strDailySalesFile)) Then

                            Dim intCntr As Integer
                            Dim DBAdapter As New OleDb.OleDbDataAdapter()

                            DBFConn.ConnectionString = "Provider=VFPOLEDB;" &
                                                    "Data Source=" & strDeviceDir.Replace("\", "\\") & ";Exclusive=Yes;Collating Sequence=machine;"
                            DBFConn.Open()

                            strDeviceIDs = ""
                            intCntr = 0
                            While (intCntr < dtaDSaleTable.Rows.Count)

                                Dim TransDate As Date
                                Dim dblOldGT As Double
                                Dim dblNewGT As Double
                                Dim dblDlySale As Double
                                Dim dblTotDisc As Double
                                Dim dblTotRef As Double
                                Dim dblTotCan As Double
                                Dim dblVAT As Double

                                Dim intBegInv As Integer
                                Dim intEndInv As Integer
                                Dim intBegOR As Integer
                                Dim intEndOR As Integer
                                Dim intTranCnt As Integer

                                Dim dblLocalTax As Double
                                Dim dblServCharge As Double
                                Dim dblNoTaxSale As Double
                                Dim dblRawGross As Double
                                Dim dblDlylocTax As Double
                                Dim dblOthers As Double
                                Dim intTermNum As Integer

                                TransDate = Convert.ToDateTime(dtaDSaleTable.Rows(intCntr).Item("TransDate"))

                                dblDlySale = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurGrossSales")) - Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurTotalDiscount")) - Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurSalesReturnRefund")) - Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurSalesReturnExchange")) - Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurTotalVAT"))

                                dblTotDisc = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurTotalDiscount"))

                                dblTotRef = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurSalesReturnRefund")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurSalesReturnExchange"))

                                dblTotCan = 0

                                dblVAT = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurTotalVAT"))

                                intBegInv = Convert.ToInt32(dtaDSaleTable.Rows(intCntr).Item("CurBegInvoiceNo"))
                                intEndInv = Convert.ToInt32(dtaDSaleTable.Rows(intCntr).Item("CurEndInvoiceNo"))
                                intBegOR = 0
                                intEndOR = 0

                                intTranCnt = Convert.ToInt32(dtaDSaleTable.Rows(intCntr).Item("CurTransCountInv")) '+ Convert.ToInt32(dtaDSaleTable.Rows(intCntr).Item("CurTransCountMemo"))

                                dblLocalTax = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurLocalTax"))
                                dblServCharge = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurServiceCharge"))
                                dblNoTaxSale = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurNoTAXSales"))
                                dblRawGross = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurGrossSales")) - Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurTotalDiscount")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurServiceCharge")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurLocalTax"))
                                dblDlylocTax = dblDlySale - dblLocalTax
                                dblOthers = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurOthers"))
                                intTermNum = CInt2(dtaDSaleTable.Rows(intCntr).Item("CurTermNum"))

                                dblOldGT = Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("BegBal")) +
                                    (Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("PrevGrossSales")) - (Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("PrevTotalDiscount")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("PrevSalesReturnRefund")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("PrevSalesReturnExchange")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("PrevServiceCharge"))))

                                dblNewGT = dblOldGT + dblDlySale + dblVAT

                                If (dblDlySale <> 0) Then

                                    'strSQL = " INSERT INTO " & strDailySalesFileName.Replace(".dbf", "") & " ( " &
                                    '             "Trandate " &
                                    '             ",Oldgt " &
                                    '             ",Newgt " &
                                    '             ",Dlysale " &
                                    '             ",Totdisc " &
                                    '             ",Totref " &
                                    '             ",Totcan " &
                                    '             ",Vat " &
                                    '             ",Tentname " &
                                    '             ",Beginv " &
                                    '             ",Endinv " &
                                    '             ",Begor " &
                                    '             ",Endor " &
                                    '             ",Trancnt " &
                                    '             ",Localtx " &
                                    '             ",Servcharge " &
                                    '             ",Notaxsale " &
                                    '             ",Rawgross " &
                                    '             ",Dlyloctax " &
                                    '             ",Others " &
                                    '             ",Termnum) VALUES ( " &
                                    '            "{" & TransDate.ToString("MM/dd/yyyy") & "}" &
                                    '            "," & dblOldGT.ToString("###0.00") &
                                    '            "," & dblNewGT.ToString("###0.00") &
                                    '            "," & dblDlySale.ToString("###0.00") &
                                    '            "," & dblTotDisc.ToString("###0.00") &
                                    '            "," & dblTotRef.ToString("###0.00") &
                                    '            "," & dblTotCan.ToString("###0.00") &
                                    '            "," & dblVAT.ToString("###0.00") &
                                    '            ",'" & strTenantName & "'" &
                                    '            "," & intBegInv.ToString("###0") &
                                    '            "," & intEndInv.ToString("###0") &
                                    '            "," & intBegOR.ToString("###0") &
                                    '            "," & intEndOR.ToString("###0") &
                                    '            "," & intTranCnt.ToString("###0") &
                                    '            "," & dblLocalTax.ToString("###0.00") &
                                    '            "," & dblServCharge.ToString("###0.00") &
                                    '            "," & dblNoTaxSale.ToString("###0.00") &
                                    '            "," & dblRawGross.ToString("###0.00") &
                                    '            "," & dblDlylocTax.ToString("###0.00") &
                                    '            "," & dblOthers.ToString("###0.00") &
                                    '            "," & intTermNum.ToString("###0") &
                                    '            ") "

                                    strSQL = " INSERT INTO " & strDailySalesFileName.Replace(".dbf", "") & " ( " &
                                                 "TRANDATE " &
                                                 ",OLDGT " &
                                                 ",NEWGT " &
                                                 ",DLYSALE " &
                                                 ",TOTDISC " &
                                                 ",TOTREF " &
                                                 ",TOTCAN " &
                                                 ",VAT " &
                                                 ",TENTNAME " &
                                                 ",BEGINV " &
                                                 ",ENDINV " &
                                                 ",BEGOR " &
                                                 ",ENDOR " &
                                                 ",TRANCNT " &
                                                 ",LOCALTX " &
                                                 ",SERVCHARGE " &
                                                 ",NOTAXSALE " &
                                                 ",RAWGROSS " &
                                                 ",DLYLOCTAX " &
                                                 ",OTHERS " &
                                                 ",TERMNUM) VALUES ( " &
                                                "{" & TransDate.ToString("MM/dd/yyyy") & "}" &
                                                "," & dblOldGT.ToString("###0.00") &
                                                "," & dblNewGT.ToString("###0.00") &
                                                "," & dblDlySale.ToString("###0.00") &
                                                "," & dblTotDisc.ToString("###0.00") &
                                                "," & dblTotRef.ToString("###0.00") &
                                                "," & dblTotCan.ToString("###0.00") &
                                                "," & dblVAT.ToString("###0.00") &
                                                ",'" & strTenantNameForDBF & "'" &
                                                "," & intBegInv.ToString("###0") &
                                                "," & intEndInv.ToString("###0") &
                                                "," & intBegOR.ToString("###0") &
                                                "," & intEndOR.ToString("###0") &
                                                "," & intTranCnt.ToString("###0") &
                                                "," & dblLocalTax.ToString("###0.00") &
                                                "," & dblServCharge.ToString("###0.00") &
                                                "," & dblNoTaxSale.ToString("###0.00") &
                                                "," & dblRawGross.ToString("###0.00") &
                                                "," & dblDlylocTax.ToString("###0.00") &
                                                "," & dblOthers.ToString("###0") &
                                                "," & intTermNum.ToString("###0") &
                                                ") "

                                    With DBAdapter
                                        .SelectCommand = DBFConn.CreateCommand
                                        .SelectCommand.CommandType = CommandType.Text
                                        .SelectCommand.CommandText = strSQL
                                        .SelectCommand.ExecuteNonQuery()
                                    End With

                                    strDeviceIDs = strDeviceIDs & IIf(strDeviceIDs.Trim().Length > 0, ", '" & dtaDSaleTable.Rows(intCntr).Item("DeviceID").ToString() & "'", "'" & dtaDSaleTable.Rows(intCntr).Item("DeviceID").ToString() & "'")
                                End If

                                intCntr = intCntr + 1
                            End While

                            strSQL = "Select * from " & strDailySalesFileName.Replace(".dbf", "")

                            With DBAdapter
                                .SelectCommand = DBFConn.CreateCommand
                                .SelectCommand.CommandType = CommandType.Text
                                .SelectCommand.CommandText = strSQL
                                .SelectCommand.ExecuteNonQuery()
                            End With

                            Dim dtTemp As DataTable
                            Dim dtOriginal As New DataTable

                            'dtOriginal.Load(DBAdapter.SelectCommand.ExecuteReader(), LoadOption.Upsert)

                            dtOriginal.Load(DBAdapter.SelectCommand.ExecuteReader())

                            dtTemp = dtOriginal.Clone

                            dtTemp.Columns("TRANDATE").DataType = GetType(String)

                            For Each row As DataRow In dtOriginal.Rows
                                Dim dr As DataRow

                                dr = dtTemp.NewRow

                                dr(0) = Format(row(0), "MM/dd/yyyy")
                                dr(1) = row(1)
                                dr(2) = row(2)
                                dr(3) = row(3)
                                dr(4) = row(4)
                                dr(5) = row(5)
                                dr(6) = row(6)
                                dr(7) = row(7)
                                dr(8) = strTenantName 'row(8)
                                dr(9) = row(9)
                                dr(10) = row(10)
                                dr(11) = row(11)
                                dr(12) = row(12)
                                dr(13) = row(13)
                                dr(14) = row(14)
                                dr(15) = row(15)
                                dr(16) = row(16)
                                dr(17) = row(17)
                                dr(18) = row(18)
                                dr(19) = CInt(row(19))
                                dr(20) = row(20)

                                dtTemp.Rows.Add(dr)
                            Next

                            dtTemp.Columns.Item("TRANDATE").ColumnName = dtTemp.Columns.Item("TRANDATE").Caption.ToUpper()
                            dtTemp.Columns.Item("OLDGT").ColumnName = dtTemp.Columns.Item("OLDGT").Caption.ToUpper()
                            dtTemp.Columns.Item("NEWGT").ColumnName = dtTemp.Columns.Item("NEWGT").Caption.ToUpper()
                            dtTemp.Columns.Item("DLYSALE").ColumnName = dtTemp.Columns.Item("DLYSALE").Caption.ToUpper()
                            dtTemp.Columns.Item("TOTDISC").ColumnName = dtTemp.Columns.Item("TOTDISC").Caption.ToUpper()
                            dtTemp.Columns.Item("TOTREF").ColumnName = dtTemp.Columns.Item("TOTREF").Caption.ToUpper()
                            dtTemp.Columns.Item("TOTCAN").ColumnName = dtTemp.Columns.Item("TOTCAN").Caption.ToUpper()
                            dtTemp.Columns.Item("VAT").ColumnName = dtTemp.Columns.Item("VAT").Caption.ToUpper()
                            dtTemp.Columns.Item("TENTNAME").ColumnName = dtTemp.Columns.Item("TENTNAME").Caption.ToUpper()
                            dtTemp.Columns.Item("BEGINV").ColumnName = dtTemp.Columns.Item("BEGINV").Caption.ToUpper()
                            dtTemp.Columns.Item("ENDINV").ColumnName = dtTemp.Columns.Item("ENDINV").Caption.ToUpper()
                            dtTemp.Columns.Item("BEGOR").ColumnName = dtTemp.Columns.Item("BEGOR").Caption.ToUpper()
                            dtTemp.Columns.Item("ENDOR").ColumnName = dtTemp.Columns.Item("ENDOR").Caption.ToUpper()
                            dtTemp.Columns.Item("TRANCNT").ColumnName = dtTemp.Columns.Item("TRANCNT").Caption.ToUpper()
                            dtTemp.Columns.Item("LOCALTX").ColumnName = dtTemp.Columns.Item("LOCALTX").Caption.ToUpper()
                            dtTemp.Columns.Item("SERVCHARGE").ColumnName = dtTemp.Columns.Item("SERVCHARGE").Caption.ToUpper()
                            dtTemp.Columns.Item("NOTAXSALE").ColumnName = dtTemp.Columns.Item("NOTAXSALE").Caption.ToUpper()
                            dtTemp.Columns.Item("RAWGROSS").ColumnName = dtTemp.Columns.Item("RAWGROSS").Caption.ToUpper()
                            dtTemp.Columns.Item("DLYLOCTAX").ColumnName = dtTemp.Columns.Item("DLYLOCTAX").Caption.ToUpper()
                            dtTemp.Columns.Item("OTHERS").ColumnName = dtTemp.Columns.Item("OTHERS").Caption.ToUpper()
                            dtTemp.Columns.Item("TERMNUM").ColumnName = dtTemp.Columns.Item("TERMNUM").Caption.ToUpper()

                            TableToExcelFile(dtTemp.DefaultView.ToTable, "", "", strDailySalesFileText, True)
                            IO.File.Move(strDailySalesFileText, Replace(strDailySalesFileText, "csv", "txt"))

                            DBFConn.Close()
                        Else
                            oError &= "strDailySalesFile does not exist. "
                        End If
                    Else
                        oError &= "Daily temp file does not exist. "
                    End If
                Else
                    oError &= "No daily sales. "
                End If

                ''' Get the Hourly sales for the period
                strHourlySalesFileName = strTenantCode.Substring(0, 3) & dtDate.ToString("MMdd") & "H.dbf"
                strHourlySalesFile = strDeviceDir & "\" & strHourlySalesFileName

                strHourlySalesFileNameText = MerchantsLeaseOutNumber & dtDate.ToString("MMdd") & "H.csv"
                strHourlySalesFileText = strDeviceDir & "\" & strHourlySalesFileNameText

                If (Not IO.Directory.Exists(strDeviceDir)) Then
                    IO.Directory.CreateDirectory(strDeviceDir)
                End If

                '''Generate Daily Sales
                If (IO.File.Exists(strHourlyTempFile)) Then

                    '''Delete if destination file exist
                    If (IO.File.Exists(strHourlySalesFile)) Then
                        IO.File.Delete(strHourlySalesFile)
                        'IO.File.Delete(strHourlySalesFileText)
                    End If

                    IO.File.Copy(strHourlyTempFile, strHourlySalesFile)
                    IO.File.SetAttributes(strHourlySalesFile, IO.FileAttributes.Normal)

                    'IO.File.Create(strHourlySalesFileText)
                    'IO.File.SetAttributes(strHourlySalesFileText, IO.FileAttributes.Normal)

                    If (strDeviceIDs.Trim().Length > 0) Then

                        Dim intHRCntr As Integer
                        Dim intHRCntrAll As Integer
                        Dim dtDateHR As DateTime

                        dtDateHR = dtDateFrom
                        intHRCntr = dtDateFrom.Hour
                        intHRCntrAll = intHRCntr

                        Dim intCntr As Integer
                        Dim DBAdapter As New OleDb.OleDbDataAdapter()

                        DBFConn.ConnectionString = "Provider=VFPOLEDB;" &
                                                "Data Source=" & strDeviceDir.Replace("\", "\\") & ";Exclusive=Yes;Collating Sequence=machine;"
                        DBFConn.Open()

                        While (Convert.ToDateTime(dtDateFrom.ToString("MM/dd/yyyy")).AddHours(intHRCntrAll) <= dtDateTo)

                            If (intHRCntr >= 24) Then
                                intHRCntr = 0
                                dtDateHR = dtDateHR.AddDays(1)
                            End If

                            dtaHSaleTable = getDailyHourlySales(dtDateHR, intHRCntr, vFowstID, strDeviceIDs)

                            If (dtaHSaleTable.Rows.Count > 0) Then

                                '''After Copy, dump the daily sales to dbf file
                                If (IO.File.Exists(strHourlySalesFile)) Then

                                    intCntr = 0
                                    While (intCntr < dtaHSaleTable.Rows.Count)

                                        Dim TransDate As Date
                                        Dim dblSale As Double
                                        Dim intTranCnt As Integer
                                        Dim intTermNum As Integer

                                        TransDate = Convert.ToDateTime(dtaHSaleTable.Rows(intCntr).Item("TransDate"))

                                        dblSale = Convert.ToDouble(dtaHSaleTable.Rows(intCntr).Item("GrossSales")) - Convert.ToDouble(dtaHSaleTable.Rows(intCntr).Item("TotalDiscount")) - Convert.ToDouble(dtaHSaleTable.Rows(intCntr).Item("SalesReturnRefund")) - Convert.ToDouble(dtaHSaleTable.Rows(intCntr).Item("SalesReturnExchange")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurServiceCharge")) + Convert.ToDouble(dtaDSaleTable.Rows(intCntr).Item("CurLocalTax"))

                                        intTranCnt = Convert.ToInt32(dtaHSaleTable.Rows(intCntr).Item("TransCountInv")) '+ Convert.ToInt32(dtaHSaleTable.Rows(intCntr).Item("TransCountMemo"))

                                        intTermNum = Convert.ToInt32(dtaHSaleTable.Rows(intCntr).Item("TermNum"))

                                        'strSQL = " INSERT INTO " & strHourlySalesFile.Replace(".dbf", "") & " ( " &
                                        '                "Trandate " &
                                        '                ",[Hour] " &
                                        '                ",Sales " &
                                        '                ",Trancnt " &
                                        '                ",Tentname " &
                                        '                ",Termnum) VALUES ( " &
                                        '            "{" & TransDate.ToString("MM/dd/yyyy") & "}" &
                                        '            "," & ReadyString(intHRCntr.ToString("00") & ":00") &
                                        '            "," & dblSale.ToString("###0.00") &
                                        '            "," & intTranCnt.ToString("###0") &
                                        '            ",'" & strTenantName & "'" &
                                        '            "," & intTermNum.ToString("###0") &
                                        '            ") "

                                        strSQL = " INSERT INTO " & strHourlySalesFile.Replace(".dbf", "") & " ( " &
                                                        "TRANDATE " &
                                                        ",[HOUR] " &
                                                        ",SALES " &
                                                        ",TRANCNT " &
                                                        ",TENTNAME " &
                                                        ",TERMNUM) VALUES ( " &
                                                    "{" & TransDate.ToString("MM/dd/yyyy") & "}" &
                                                    "," & ReadyString(intHRCntr.ToString("00") & ":00") &
                                                    "," & dblSale.ToString("###0.00") &
                                                    "," & intTranCnt.ToString("###0") &
                                                    ",'" & strTenantNameForDBF & "'" &
                                                    "," & intTermNum.ToString("###0") &
                                                    ") "

                                        With DBAdapter
                                            .SelectCommand = DBFConn.CreateCommand
                                            .SelectCommand.CommandType = CommandType.Text
                                            .SelectCommand.CommandText = strSQL
                                            .SelectCommand.ExecuteNonQuery()
                                        End With

                                        intCntr = intCntr + 1
                                    End While



                                End If

                            End If

                            intHRCntr = intHRCntr + 1
                            intHRCntrAll = intHRCntrAll + 1
                        End While

                        Dim DBAdapter2 As New OleDb.OleDbDataAdapter()

                        strSQL = "Select * from " & strHourlySalesFile.Replace(".dbf", "")

                        With DBAdapter2
                            .SelectCommand = DBFConn.CreateCommand
                            .SelectCommand.CommandType = CommandType.Text
                            .SelectCommand.CommandText = strSQL
                            .SelectCommand.ExecuteNonQuery()
                        End With

                        Dim dtTemp As DataTable
                        Dim dtOriginal As New DataTable

                        dtOriginal.Load(DBAdapter2.SelectCommand.ExecuteReader())

                        dtTemp = dtOriginal.Clone

                        dtTemp.Columns("TRANDATE").DataType = GetType(String)

                        For Each row As DataRow In dtOriginal.Rows
                            Dim dr As DataRow

                            dr = dtTemp.NewRow

                            dr(0) = Format(row(0), "MM/dd/yyyy")
                            dr(1) = row(1)
                            dr(2) = row(2)
                            dr(3) = row(3)
                            dr(4) = strTenantName ' row(4)
                            dr(5) = row(5)

                            dtTemp.Rows.Add(dr)
                        Next

                        dtTemp.Columns.Item("TRANDATE").ColumnName = dtTemp.Columns.Item("TRANDATE").Caption.ToUpper()
                        dtTemp.Columns.Item("HOUR").ColumnName = dtTemp.Columns.Item("HOUR").Caption.ToUpper()
                        dtTemp.Columns.Item("SALES").ColumnName = dtTemp.Columns.Item("SALES").Caption.ToUpper()
                        dtTemp.Columns.Item("TRANCNT").ColumnName = dtTemp.Columns.Item("TRANCNT").Caption.ToUpper()
                        dtTemp.Columns.Item("TENTNAME").ColumnName = dtTemp.Columns.Item("TENTNAME").Caption.ToUpper()
                        dtTemp.Columns.Item("TERMNUM").ColumnName = dtTemp.Columns.Item("TERMNUM").Caption.ToUpper()

                        TableToExcelFile(dtTemp.DefaultView.ToTable, "", "", strHourlySalesFileText, True)
                        IO.File.Move(strHourlySalesFileText, Replace(strHourlySalesFileText, "csv", "txt"))

                        DBFConn.Close()
                    Else
                        oError &= "Hours temp file does not exist."
                    End If
                End If
            Else
                oError = "The current FOWST has no operating hours."
            End If

            '''generate zip file
            Dim pZip As ProZip
            Dim pZipFilename As String
            Dim pFileTitle As String

            pFileTitle = vOrganizationID & ".zip"
            pZipFilename = strAyalaFolder & "\" & pFileTitle

            If (IO.File.Exists(pZipFilename)) Then
                IO.File.Delete(pZipFilename)
            End If

            pZip = New ProZip(pZipFilename)
            pZip.AddFolder(strDeviceDir)

            strRet = pFileTitle
        Catch ex As Exception
            If (DBFConn.State = ConnectionState.Open) Then
                DBFConn.Close()
            End If
            strRet = ""
            oError &= ex.Message
            AppendToDebug("vbCK6F23TQ85! " & ex.Message, True, ex, True)
        End Try

        Return strRet

    End Function

    Function getDailySales(ByVal dtDateFrom As DateTime, ByVal dtDateTo As DateTime, _
        Optional vFowstID As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransDate", SqlDbType.DateTime)
                    .Expression = "'" & dtDateFrom.ToString("MM/dd/yyyy") & "'"
                    .Searchable = False
                End With
                With .Add("BegBal", SqlDbType.Money)
                    .Expression = " ISNULL(devf.BegBalance,0) "
                End With
                With .Add("PrevGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM dbo.TransInvoiceItems invitem " &
                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevTotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)) " &
                               " FROM dbo.TransInvoiceItems invitem " &
                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0) " &
                               " - " &
                               "ISNULL((SELECT SUM(sr.TaxAmount) " &
                               " FROM TransMemoSalesReturn sr " &
                               " INNER JOIN TransMemo memo ON (sr.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                  " FROM TransSalesReturnLedger srledger " &
                                  " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                  " WHERE memo.LastUpdated  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                  " AND memo.FOWSTID = devf.FowstID " &
                                  " AND memo.DeviceID = devf.DeviceID) " &
                                  ",0) +" &
                                 "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                 " FROM dbo.TransInvoice inv " &
                                 " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                 " WHERE inv.InvoiceNumber > 0 " &
                                 " AND inv.InvoiceDate  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                 " AND inv.FOWSTID = devf.FowstID " &
                                 " AND inv.DeviceID = devf.DeviceID)" &
                                  ",0)"
                End With
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM dbo.TransInvoiceItems invitem " &
                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)) " &
                               " FROM dbo.TransInvoiceItems invitem " &
                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0) " &
                               " - " &
                               "ISNULL((SELECT SUM(sr.TaxAmount) " &
                               " FROM TransMemoSalesReturn sr " &
                               " INNER JOIN TransMemo memo ON (sr.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"

                End With
                With .Add("CurBegInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MIN(inv.InvoiceNumber) " &
                               " FROM dbo.TransInvoice inv " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurEndInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MAX(inv.InvoiceNumber) " &
                               " FROM dbo.TransInvoice inv " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurTransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " &
                               " FROM dbo.TransInvoice inv " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurTransCountMemo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memo.MemoNumber) " &
                               " FROM TransMemo memo " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurLocalTax", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                              ",0)"
                End With
                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                               " FROM TransSalesReturnLedger srledger " &
                                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                               " WHERE memo.LastUpdated  BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                               " AND memo.FOWSTID = devf.FowstID " &
                                               " AND memo.DeviceID = devf.DeviceID) " &
                                               ",0) +" &
                                              "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                              " FROM dbo.TransInvoice inv " &
                                              " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                              " WHERE inv.InvoiceNumber > 0 " &
                                              " AND inv.InvoiceDate  BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                              " AND inv.FOWSTID = devf.FowstID " &
                                              " AND inv.DeviceID = devf.DeviceID)" &
                                               ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT sum(invitems.GrossSalesNetOfVATExemptions - invitems.DiscountAmount) " &
                             " FROM dbo.TransInvoiceItems invitems " &
                             " INNER JOIN dbo.TransInvoice inv ON (invitems.InvoiceID = inv.ID)  " &
                             " WHERE (inv.InvoiceNumber > 0) AND invitems.TaxAmount = 0 AND ISNULL(invitems.VATableSale,0) = 0 " &
                             " AND inv.InvoiceDate  BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateTo.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                             " AND inv.FOWSTID = devf.FowstID " &
                             " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurOthers", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CurTermNum", SqlDbType.Money)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("DeviceID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf " &
                    " INNER JOIN dbo.LibraryDevice dev ON (devf.DeviceID = dev.ID) "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(vFowstID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDailyHourlySales(ByVal dtDate As Date, ByVal intHR As Integer,
        Optional vFowstID As String = Nothing,
        Optional vDeviceIDs As String = Nothing) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        If vFowstID Is Nothing Then vFowstID = WebRequest.Instance.FowstID
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransDate", SqlDbType.DateTime)
                    .Expression = "'" & dtDate.ToString("MM/dd/yyyy") & "'"
                    .Searchable = False
                End With
                With .Add("TransHour", SqlDbType.VarChar)
                    .Expression = "'" & intHR.ToString("00") & ":00" & "'"
                    .Searchable = False
                End With
                With .Add("GrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM dbo.TransInvoiceItems invitem " &
                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                'With .Add("TotalDiscount", SqlDbType.Money)
                '    .Expression = "0"
                'End With
                With .Add("TotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("SalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("SalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("ServiceCharge", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("TotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0) - ISNULL(invitem.DiscountTax,0)) " &
                               " FROM dbo.TransInvoiceItems invitem " &
                               " INNER JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0) " &
                               " - " &
                               "ISNULL((SELECT SUM(sr.TaxAmount) " &
                               " FROM TransMemoSalesReturn sr " &
                               " INNER JOIN TransMemo memo ON (sr.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("TransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " &
                               " FROM dbo.TransInvoice inv " &
                               " WHERE inv.InvoiceNumber > 0 " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("TransCountMemo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memo.MemoNumber) " &
                               " FROM TransMemo memo " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":00") & " AND " & ReadyString(dtDate.ToString("MM/dd/yyyy") & " " & intHR.ToString("00") & ":59:59") &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("TermNum", SqlDbType.Money)
                    .Expression = "dev.DeviceNo"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf " &
                    " INNER JOIN dbo.LibraryDevice dev ON (devf.DeviceID = dev.ID) "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(vFowstID) &
                IIf(vDeviceIDs.Trim().Length > 0, " AND dev.ID IN (" & vDeviceIDs & ")", "")

            pDT = .Requery

        End With

        Return pDT

    End Function





End Class
