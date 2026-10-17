Imports Amazon.Kinesis
Imports Amazon.KinesisFirehose
Imports Amazon.KinesisFirehose.Model
Imports Newtonsoft.Json
Imports System.IO

Imports Aspose
Imports Aspose.Cells

Imports System.Net

Imports System.Security.Cryptography

Public Class Triggers

    Dim mBusiness As Business
    Private sFtp As Renci.SshNet.SftpClient
    Private myFileStream As System.IO.FileStream
    ReadOnly mProData As ProData

    Sub ConnectToDatabase(dbName As String)
        'mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
        mProData.Execute("USE " & EscapeSqlName(dbName))
    End Sub

    Sub New(Optional ByRef rProData As ProData = Nothing)
        mProData = rProData
    End Sub

    Sub SendAuditTrailReport(OrgID As String)
        Dim dashboards As New DashBoardsAndReports
        Dim pSql As String
        Dim pDTFOWST As DataTable
        Dim m_Day As Integer
        Dim m_ScheduleDay As Integer
#If DEBUG Then
        m_Day = 2
        m_ScheduleDay = 2
#Else
        m_Day = Today.DayOfWeek
        m_ScheduleDay = 6
#End If


        If m_Day = m_ScheduleDay Then
            pSql = "Select ID, Name from MobizLibraryOrganizationFOWST where OrganizationID = " & ReadyString(OrgID)
            '& " AND Name like '%Main%'"

            pDTFOWST = mProData.Execute(pSql)

            For i As Integer = 0 To pDTFOWST.Rows.Count - 1
                Dim pDTRawData As DataTable

                dashboards.FOWSTID = CStr2(pDTFOWST.Rows(i).Item("ID"))
                dashboards.DateFrom = Today.AddDays(-6)
                dashboards.DateTo = Today
                dashboards.OrgID = OrgID
                pDTRawData = dashboards.GetRawData()

                Dim pSourceURL As String = Common.ServerPath & "temp\AuditTrail"
                Dim strFileName As String

                If (pDTRawData.Rows.Count > 0) Then
                    If Not System.IO.Directory.Exists(pSourceURL) Then
                        IO.Directory.CreateDirectory(pSourceURL)
                    End If

                    strFileName = "Audit_Trail_" & CStr2(pDTFOWST.Rows(i).Item("Name")).Replace(" ", "_") & "_" & Today.AddDays(-7).ToString("MMddyyy") & "_" & Today.AddDays(-1).ToString("MMddyyy") & ".xls"

                    If (IO.File.Exists(pSourceURL & "\" & strFileName)) Then
                        IO.File.Delete(pSourceURL & "\" & strFileName)
                    End If

                    TableToExcelFile(pDTRawData, "", "", pSourceURL & "\" & strFileName)

#If DEBUG Then
                    SendMail("mobizcompleteretail@outlook.com",
                     "francis.hernandez@mobiz-apps.com",
                     "Mobiz",
                     Replace(strFileName, ".xls", ""),
                     Replace(strFileName, ".xls", ""),
                     "smtp.live.com",
                     587,
                     "mobizcompleteretail@outlook.com",
                     "m0b!zp30pl30nly!", , ,
                     strFileName,
                     pSourceURL & "\" & strFileName,
                     True)
#Else
                    SendMail("mobizcompleteretail@outlook.com", "francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                    "Mobiz",
                     Replace(strFileName, ".xls", ""),
                     Replace(strFileName, ".xls", ""),
                     "smtp.live.com",
                     587,
                     "mobizcompleteretail@outlook.com",
                     "m0b!zp30pl30nly!", , ,
                     strFileName,
                     pSourceURL & "\" & strFileName,
                     True)

#End If
                End If

            Next
        End If

    End Sub

#Region "Nike SEA Report V1"

    Sub NikeSEAReport(ByVal CompanyDBName As String)
        Dim m_Day As Integer

        'Dim DateFrom As Date = CDate(DateAdd(DateInterval.Day, -8, Today))
        'Dim DateTo As Date = CDate(DateAdd(DateInterval.Day, -2, Today))

        Dim DateFrom As Date = CDate(DateAdd(DateInterval.Day, -7, Today))
        Dim DateTo As Date = CDate(DateAdd(DateInterval.Day, -1, Today))

        Dim clsReportToPrincipal As New ReportToPrincipal
        Dim m_Path As String

        If (CompanyDBName.Contains("Nike")) Then

            m_Day = Today.DayOfWeek

            If m_Day = 0 Then

                m_Path = "InvBal_Week_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                If Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path) Then
                    If GetWeeklyInventoryReport(DateTo) Then

#If DEBUG Then
                        SendMail("mobizcompleteretail@outlook.com",
                                                         "francis.hernandez@mobiz-apps.com",
                                                         "Mobiz",
                                                         Replace(m_Path, ".csv", ""),
                                                         Replace(m_Path, ".csv", ""),
                                                         "smtp.live.com",
                                                         587,
                                                         "mobizcompleteretail@outlook.com",
                                                         "m0b!zp30pl30nly!", , ,
                                                         m_Path,
                                                         Common.ServerPath & "temp\" & m_Path,
                                                         True)
#Else
                            SendMail("mobizcompleteretail@outlook.com",
                                                             "sellthru.sea@nike.com, christinegrace.perez@yahoo.com, VincentEmmanuel.Villanueva@nike.com, francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                                                             "Mobiz",
                                                             Replace(m_Path, ".csv", ""),
                                                             Replace(m_Path, ".csv", ""),
                                                             "smtp.live.com",
                                                             587,
                                                             "mobizcompleteretail@outlook.com",
                                                             "m0b!zp30pl30nly!", , ,
                                                             m_Path,
                                                             Common.ServerPath & "temp\" & m_Path,
                                                             True)
#End If

                    End If
                End If

                m_Path = "POS_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                If Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path) Then
                    If (GetWeeklySalesReport(DateFrom, DateTo)) Then
#If DEBUG Then
                        SendMail("mobizcompleteretail@outlook.com",
                         "francis.hernandez@mobiz-apps.com",
                         "Mobiz",
                         Replace(m_Path, ".csv", ""),
                         Replace(m_Path, ".csv", ""),
                         "smtp.live.com",
                         587,
                         "mobizcompleteretail@outlook.com",
                         "m0b!zp30pl30nly!", , ,
                         m_Path,
                         Common.ServerPath & "temp\" & m_Path,
                         True)
#Else
                            SendMail("mobizcompleteretail@outlook.com",
                             "sellthru.sea@nike.com, christinegrace.perez@yahoo.com, VincentEmmanuel.Villanueva@nike.com, francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                             "Mobiz",
                             Replace(m_Path, ".csv", ""),
                             Replace(m_Path, ".csv", ""),
                             "smtp.live.com",
                             587,
                             "mobizcompleteretail@outlook.com",
                             "m0b!zp30pl30nly!", , ,
                             m_Path,
                             Common.ServerPath & "temp\" & m_Path,
                             True)

#End If

                    End If
                End If
            End If

            Dim m_StartOfDay As Date = Today.AddDays(-4)

            If Today.Day = 5 Then
                m_Path = "InvMvmt_Month_" & Today.AddDays(-6).Year & Format(Today.AddDays(-6).Month, "00") & Format(Today.AddDays(-5).Day, "00") & ".csv"

                'm_Path = GetMonthInventoryReport()
                'sellthru.sea@nike.com, christinegrace.perez@yahoo.com, 

                If (Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path)) Then
                    If GetMonthInventoryReport(m_StartOfDay.AddMonths(-1), Today.AddDays(-5)) Then
#If DEBUG Then
                        SendMail("mobizcompleteretail@outlook.com",
                           "francis.hernandez@mobiz-apps.com",
                           "Mobiz",
                           Replace(m_Path, ".csv", ""),
                           Replace(m_Path, ".csv", ""),
                           "smtp.live.com",
                           587,
                           "mobizcompleteretail@outlook.com",
                           "m0b!zp30pl30nly!", , ,
                           m_Path,
                           Common.ServerPath & "temp\" & m_Path,
                           True)
#Else
                            SendMail("mobizcompleteretail@outlook.com",
                             "sellthru.sea@nike.com, christinegrace.perez@yahoo.com, VincentEmmanuel.Villanueva@nike.com, francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "mobizcompleteretail@outlook.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#End If

                    End If
                End If
            End If

            '            If Today.Day = 1 Then
            '                DateTo = CDate(DateAdd(DateInterval.Day, -1, Today))
            '                m_Path = "InvBal_Asof_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

            '                'm_Path = GetMonthInventoryReport()
            '                'sellthru.sea@nike.com, christinegrace.perez@yahoo.com, 

            '                If (Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path)) Then
            '                    If GetWeeklyInventoryReport(CDate(DateAdd(DateInterval.Day, -1, Today))) Then
            '#If DEBUG Then
            '                        SendMail("customercare@mobiz-apps.com",
            '                           "francis.hernandez@mobiz-apps.com",
            '                           "Mobiz",
            '                           Replace(m_Path, ".csv", ""),
            '                           Replace(m_Path, ".csv", ""),
            '                           "smtp.live.com",
            '                           587,
            '                           "customercare@mobiz-apps.com",
            '                           "MobizCustCare2221", , ,
            '                           m_Path,
            '                           Common.ServerPath & "temp\" & m_Path,
            '                           True)
            '#Else
            '                            SendMail("christinegrace.perez@yahoo.com", "francis.hernandez@mobiz-apps.com", "gmtmanluctao@yahoo.com",
            '                               "Mobiz",
            '                               Replace(m_Path, ".csv", ""),
            '                               Replace(m_Path, ".csv", ""),
            '                               "smtp.live.com",
            '                               587,
            '                               "customercare@mobiz-apps.com",
            '                               "MobizCustCare2221", , ,
            '                               m_Path,
            '                               Common.ServerPath & "temp\" & m_Path,
            '                               True)
            '#End If

            '                    End If
            '                End If
            '            End If

        End If
    End Sub

    Function GetWeeklyInventoryReport(ByVal m_date As Date) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = mProData

            .Distinct = True
            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "t.Account_ID"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "t.Store_ID"
                End With
                With .Add("Inv_Balance_Date", SqlDbType.VarChar)
                    .Expression = "t.Inv_Balance_Date"
                End With
                With .Add("Product_Code", SqlDbType.Date)
                    .Expression = "t.Product_Code"
                End With
                With .Add("Size", SqlDbType.Date)
                    .Expression = "t.Size"
                End With
                With .Add("UPC_Code", SqlDbType.Date)
                    .Expression = "t.UPC_Code"
                End With
                With .Add("Inv_Balance_Qty", SqlDbType.Float)
                    .Expression = "convert(int,SUM(t.balance))"
                End With
            End With

            .SqlFrom = "FROM (SELECT DISTINCT " &
                        "dbo.MobizLibraryPIMProduct.ID, dbo.MobizLibraryPIMProduct.ProductNo AS UPC_Code, dbo.MobizLibraryPIMProduct.ProductName, " &
                        "dbo.MobizLibraryPIMProduct.ProductGroupNo AS Product_Code, dbo.MobizLibraryOrganizationFOWST.AccountID AS Account_ID,  " &
                        "dbo.MobizLibraryOrganizationFOWST.StoreID AS Store_ID, replace(convert(varchar, convert(date," & ReadyString(m_date) & "), 102),'.','') AS Inv_Balance_Date, dbo.MobizLibraryPIMProductVariableFeatures001.C0017 AS Size, " &
                        "bal1.Balance " &
                        "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " &
                        "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.LibraryOrganizationPIM.ProductID LEFT OUTER JOIN " &
                        "dbo.MobizLibraryOrganization INNER JOIN " &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.MobizLibraryOrganization.ID = dbo.MobizLibraryOrganizationFOWST.OrganizationID INNER JOIN " &
                        "dbo.TransInventoryLedger AS t0 INNER JOIN " &
                        "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = t0.ProductID ON dbo.MobizLibraryOrganizationFOWST.ID = t0.FowstID ON  " &
                        "dbo.LibraryOrganizationPIM.ProductID = t0.ProductID " &
                        "outer apply(  " &
                     "select top 1 Balance from TransInventoryLedger as t1 where t1.ProductID = t0.ProductID And t1.FowstID = t0.FowstID  " &
                     "And t1.BinID = t0.BinID order by LastUpdated desc )  bal1 	 " &
                     "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND dbo.LibraryOrganizationPIM.IsDeactivated = 0 " &
                     ") t   "
            .SqlGroup = "group by Account_ID, Store_ID, Inv_Balance_Date, UPC_Code, Size, Product_Code"

            pDT = mProData.Execute(.CreateSql)

            pDT.DefaultView.RowFilter = "Inv_Balance_Qty <> 0"
            'pDT = .Requery

            'Dim m_date As Date = CDate(WebRequest.Instance.Data("To"))

            TableToExcelFile(pDT.DefaultView.ToTable, "", "", Common.ServerPath & "temp\InvBal_Week_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

            'Return "InvBal_Week_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            Return True
        End With
    End Function

    Function GetWeeklySalesReport(ByVal DateFrom As Date, ByVal DateTo As Date) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = mProData

            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID"
                End With
                With .Add("Transaction_ID", SqlDbType.VarChar)
                    .Expression = "tSource.TransName"
                End With
                With .Add("Transaction_Date", SqlDbType.Date)
                    .Expression = "replace(convert(varchar, tSource.TransactionDate, 102),'.','')"
                End With
                With .Add("Product_Code", SqlDbType.Float)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0017,'MISC')"
                End With
                With .Add("UPC_Code", SqlDbType.Int)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("Sold_Qty", SqlDbType.Float)
                    .Expression = "convert(int,dbo.TransInventoryLedger.Qty * -1)"
                End With
                With .Add("Sold_Amt", SqlDbType.VarChar)
                    .Expression = "replace(CONVERT(varchar(12), tSource.VatExcluded, 1),',','')"
                End With
                With .Add("Sold_Amt_Discount", SqlDbType.VarChar)
                    .Expression = "replace(CONVERT(varchar(12), tSource.Discount, 1),',','')"
                End With
                With .Add("Transaction_Type", SqlDbType.VarChar)
                    .Expression = "dbo.TransInventoryLedger.TransactionType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                        "dbo.TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInventoryLedger.FowstID INNER JOIN " & vbCrLf &
                        "dbo.LibraryOrganizationPIM ON dbo.TransInventoryLedger.ProductID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct LEFT JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProductVariableFeatures001 ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductVariableFeatures001.ProductID ON  " & vbCrLf &
                        "dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                        "OUTER APPLY ( " & vbCrLf &
                        "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoiceItems.TaxSales as VatExcluded, TransInvoice.InvoiceDate as TransactionDate, TransInvoiceItems.DiscountSales as Discount  " & vbCrLf &
                        "FROM TransInvoice INNER JOIN TransInvoiceItems on TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
                        "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransInvoiceItems.ProductID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                        "UNION " & vbCrLf &
                        "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, (dbo.TransMemoSalesReturn.TotalAmount - TransMemoSalesReturn.TaxAmount ) * -1 as VatExcluded, TransMemo.MemoDate as TransactionDate, '' as Discount  " & vbCrLf &
                        "FROM TransMemo " & vbCrLf &
                        "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransMemoSalesReturn.ProductID " & vbCrLf &
                        "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                        ") tSource"

            .SqlWhere = "WHERE (TransactionType = 'Sales Returns' OR TransactionType = 'Invoice') AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlOrder = "ORDER BY tSource.TransName"

            'pDT = .Requery
            pDT = mProData.Execute(.CreateSql)

            Dim m_date As Date = DateTo
            TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\POS_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

            'Return "POS_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            Return True
        End With

    End Function

    Function GetMonthInventoryReport(ByVal DateFrom As Date, ByVal DateTo As Date) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = mProData

            .Distinct = True
            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID"
                End With
                With .Add("Transaction_ID", SqlDbType.VarChar)
                    .Expression = "ISNULL(tSource.TransName, TransInventoryLedger.TransactionType)"
                End With
                With .Add("Inv_Mvmt_Date", SqlDbType.Date)
                    .Expression = "replace(convert(date, tSource.TransactionDate),'-','')"
                End With
                With .Add("Product_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProductVariableFeatures001.C0017"
                End With
                With .Add("UPC_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("Inv_Mvmt_Qty", SqlDbType.Float)
                    .Expression = "convert(int,TransInventoryLedger.Qty)"
                End With
                With .Add("TransactionType", SqlDbType.Int)
                    .Expression = "TransInventoryLedger.TransactionType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProduct.ID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInventoryLedger AS TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID ON  " & vbCrLf &
                      "dbo.LibraryOrganizationPIM.ProductID = TransInventoryLedger.ProductID " & vbCrLf &
                        "OUTER APPLY ( " & vbCrLf &
                      "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoice.InvoiceDate As TransactionDate " & vbCrLf &
                         "FROM TransInvoice " & vbCrLf &
                         "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                         "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                         "FROM TransReceiving " & vbCrLf &
                         "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Receiving' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                         "FROM TransReceiving " & vbCrLf &
                         "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'TransferOut' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, TransMemo.MemoDate As TransactionDate " & vbCrLf &
                         "FROM TransMemo " & vbCrLf &
                         "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                         "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT replace(CONVERT(varchar, CONVERT(date,TransInventoryProductCount.InventoryCountDate)),'-','') AS TransName,  TransInventoryProductCount.InventoryCountDate As TransactionDate" & vbCrLf &
                         "FROM TransInventoryProductCount " & vbCrLf &
                         "WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'InventoryCount' " & vbCrLf &
                        ") tSource"

            .SqlWhere = "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND dbo.LibraryOrganizationPIM.IsDeactivated = 0 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlOrder = "ORDER BY Inv_Mvmt_Date"

            pDT = mProData.Execute(.CreateSql)

            Dim m_date As Date = CDate(DateTo)

            TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\InvMvmt_Month_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

            'Return "InvMvmt_Month_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            Return True
        End With

    End Function

#End Region

#Region "Nike SEA Report V2"

    Sub NikeSEAReportVersion2(OrgID As String, ByVal CompanyDBName As String)
        Dim m_Day As Integer

        'Dim DateFrom As Date = CDate(DateAdd(DateInterval.Day, -8, Today))
        'Dim DateTo As Date = CDate(DateAdd(DateInterval.Day, -2, Today))

        Dim DateFrom As Date = CDate(DateAdd(DateInterval.Day, -7, Today))
        Dim DateTo As Date = CDate(DateAdd(DateInterval.Day, -1, Today))

        Dim clsReportToPrincipal As New ReportToPrincipal
        Dim m_Path As String
        Dim AccountID As String

        If (CompanyDBName.Contains("Nike")) Then

            Try

                AccountID = GetAccountID(OrgID)

                m_Day = Today.DayOfWeek

                If m_Day = 0 Then

                    m_Path = "InvtBal_" & AccountID & "_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                    If Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path) Then
                        If GetWeeklyInventoryReportVersion2(AccountID, DateTo) Then

#If DEBUG Then
                            SendMail("mobizcompleteretail@outlook.com",
                                                             "francis.hernandez@mobiz-apps.com",
                                                             "Mobiz",
                                                             Replace(m_Path, ".csv", ""),
                                                             Replace(m_Path, ".csv", ""),
                                                             "smtp.live.com",
                                                             587,
                                                             "mobizcompleteretail@outlook.com",
                                                             "m0b!zp30pl30nly!", , ,
                                                             m_Path,
                                                             Common.ServerPath & "temp\" & m_Path,
                                                             True)
#Else
                            SendMail("mobizcompleteretail@outlook.com",
                                                             "S.SEA.SELLTHRU.UAT@nike.com, VincentEmmanuel.Villanueva@nike.com, christinegrace.perez@yahoo.com, francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                                                             "Mobiz",
                                                             Replace(m_Path, ".csv", ""),
                                                             Replace(m_Path, ".csv", ""),
                                                             "smtp.live.com",
                                                             587,
                                                             "mobizcompleteretail@outlook.com",
                                                             "m0b!zp30pl30nly!", , ,
                                                             m_Path,
                                                             Common.ServerPath & "temp\" & m_Path,
                                                             True)
#End If

                        End If
                    End If

                    m_Path = "Sales_" & AccountID & "_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                    If Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path) Then
                        If (GetWeeklySalesReportVersion2(AccountID, DateFrom, DateTo)) Then
#If DEBUG Then
                            SendMail("mobizcompleteretail@outlook.com",
                             "francis.hernandez@mobiz-apps.com",
                             "Mobiz",
                             Replace(m_Path, ".csv", ""),
                             Replace(m_Path, ".csv", ""),
                             "smtp.live.com",
                             587,
                             "mobizcompleteretail@outlook.com",
                             "m0b!zp30pl30nly!", , ,
                             m_Path,
                             Common.ServerPath & "temp\" & m_Path,
                             True)
#Else
                            SendMail("mobizcompleteretail@outlook.com",
                             "S.SEA.SELLTHRU.UAT@nike.com, christinegrace.perez@yahoo.com, VincentEmmanuel.Villanueva@nike.com, francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                             "Mobiz",
                             Replace(m_Path, ".csv", ""),
                             Replace(m_Path, ".csv", ""),
                             "smtp.live.com",
                             587,
                             "mobizcompleteretail@outlook.com",
                             "m0b!zp30pl30nly!", , ,
                             m_Path,
                             Common.ServerPath & "temp\" & m_Path,
                             True)

#End If

                        End If
                    End If

                    m_Path = "InvtMvmt_" & AccountID & "_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                    If (Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path)) Then

                        If GetMonthInventoryReportVersion2(AccountID, DateFrom, DateTo) Then
#If DEBUG Then
                            SendMail("mobizcompleteretail@outlook.com",
                               "francis.hernandez@mobiz-apps.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "mobizcompleteretail@outlook.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#Else
                            SendMail("mobizcompleteretail@outlook.com",
                             "S.SEA.SELLTHRU.UAT@nike.com, christinegrace.perez@yahoo.com, VincentEmmanuel.Villanueva@nike.com, francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "mobizcompleteretail@outlook.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#End If

                        End If
                    End If
                End If

                'Dim m_StartOfDay As Date = Today.AddDays(-4)

                'If Today.Day = 5 Then

                '    'm_Path = GetMonthInventoryReport()
                '    'sellthru.sea@nike.com, christinegrace.perez@yahoo.com, 

                'End If

                If Today.Day = 1 Then
                    DateTo = CDate(DateAdd(DateInterval.Day, -1, Today))
                    m_Path = "InvBal_Asof_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                    'm_Path = GetMonthInventoryReport()
                    'sellthru.sea@nike.com, christinegrace.perez@yahoo.com, 

                    If (Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path)) Then
                        If GetWeeklyInventoryReport(CDate(DateAdd(DateInterval.Day, -1, Today))) Then
#If DEBUG Then
                            SendMail("mobizcompleteretail@outlook.com",
                               "francis.hernandez@mobiz-apps.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "mobizcompleteretail@outlook.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#Else
                            SendMail("christinegrace.perez@yahoo.com", "francis.hernandez@mobiz-apps.com", "gmtmanluctao@yahoo.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "mobizcompleteretail@outlook.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#End If

                        End If
                    End If
                End If
            Catch ex As Exception
                AppendToDebug(ex.Message, True, ex, True)
            End Try
        End If
    End Sub

    Function GetWeeklyInventoryReportVersion2(AccountID As String, ByVal m_date As Date) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = mProData

            .Distinct = True
            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "t.Account_ID"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "t.Store_ID"
                End With
                With .Add("Inv_Balance_Date", SqlDbType.VarChar)
                    .Expression = "t.Inv_Balance_Date"
                End With
                With .Add("Product_Code", SqlDbType.Date)
                    .Expression = "t.Product_Code"
                End With
                With .Add("Size", SqlDbType.Date)
                    .Expression = "t.Size"
                End With
                With .Add("UPC_Code", SqlDbType.VarChar)
                    .Expression = "t.UPC_Code"
                End With
                With .Add("Inv_Balance_Qty", SqlDbType.Float)
                    .Expression = "convert(int,SUM(t.balance))"
                End With
                With .Add("Acquired_Cost", SqlDbType.Decimal)
                    .Expression = "t.AcquiredCost"
                End With
                With .Add("Adjusted_Cost", SqlDbType.Decimal)
                    .Expression = "t.AdjustedCost"
                End With
                With .Add("Stated_Cost", SqlDbType.Decimal)
                    .Expression = "t.StatedCost"
                End With
                With .Add("Retailer_Net_Cost_of_Inventory_Amt", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), t.AcquiredCost * SUM(t.balance))"
                End With
            End With

            .SqlFrom = "FROM (SELECT DISTINCT " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct.ID, dbo.MobizLibraryPIMProduct.ProductNo AS UPC_Code, dbo.MobizLibraryPIMProduct.ProductName, " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct.ProductGroupNo AS Product_Code, dbo.MobizLibraryOrganizationFOWST.AccountID2 AS Account_ID,  " & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST.StoreID2 AS Store_ID, replace(convert(varchar, convert(date," & ReadyString(m_date) & "), 102),'.','') AS Inv_Balance_Date, " & vbCrLf &
                        "dbo.MobizLibraryPIMProductVariableFeatures001.C0017 AS Size, " & vbCrLf &
                        "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0.00)) AS AcquiredCost, " & vbCrLf &
                        "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0086,0.00)) AS AdjustedCost,  " & vbCrLf &
                        "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0087,0.00)) AS StatedCost, " & vbCrLf &
                        "bal1.Balance " & vbCrLf &
                        "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                        "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.LibraryOrganizationPIM.ProductID LEFT OUTER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganization INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.MobizLibraryOrganization.ID = dbo.MobizLibraryOrganizationFOWST.OrganizationID INNER JOIN " & vbCrLf &
                        "dbo.TransInventoryLedger AS t0 INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = t0.ProductID ON dbo.MobizLibraryOrganizationFOWST.ID = t0.FowstID ON  " & vbCrLf &
                        "dbo.LibraryOrganizationPIM.ProductID = t0.ProductID " & vbCrLf &
                        "outer apply(  " & vbCrLf &
                     "select top 1 Balance from TransInventoryLedger as t1 where t1.ProductID = t0.ProductID And t1.FowstID = t0.FowstID  " & vbCrLf &
                     "And t1.BinID = t0.BinID order by LastUpdated desc )  bal1 	 " & vbCrLf &
                     "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 " & vbCrLf &
                     "AND dbo.LibraryOrganizationPIM.IsDeactivated = 0 " & vbCrLf &
                     ") t   "

            .SqlGroup = "group by Account_ID, Store_ID, Inv_Balance_Date, UPC_Code, Size, Product_Code, AcquiredCost, AdjustedCost, StatedCost"

            pDT = mProData.Execute(.CreateSql)

            pDT.DefaultView.RowFilter = "Inv_Balance_Qty <> 0"
            'pDT = .Requery

            'Dim m_date As Date = CDate(WebRequest.Instance.Data("To"))

            TableToExcelFile(pDT.DefaultView.ToTable, "", "", Common.ServerPath & "temp\InvtBal_" & AccountID & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

            Return True
        End With
    End Function

    Function GetWeeklySalesReportVersion2(AccountID As String, ByVal DateFrom As Date, ByVal DateTo As Date) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = mProData

            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID2"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID2"
                End With
                With .Add("Transaction_ID", SqlDbType.VarChar)
                    .Expression = "tSource.TransName"
                End With
                With .Add("Transaction_Date", SqlDbType.Date)
                    .Expression = "replace(convert(varchar, tSource.TransactionDate, 102),'.','')"
                End With
                With .Add("Product_Code", SqlDbType.Float)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0017,'MISC')"
                End With
                With .Add("UPC_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("Sold_Qty", SqlDbType.Float)
                    .Expression = "convert(int,dbo.TransInventoryLedger.Qty * -1)"
                End With
                With .Add("Sold_Amt", SqlDbType.VarChar)
                    .Expression = "replace(CONVERT(varchar(12), tSource.VatExcluded, 1),',','')"
                End With
                With .Add("Sold_Amt_Discount", SqlDbType.VarChar)
                    .Expression = "replace(CONVERT(varchar(12), tSource.Discount, 1),',','')"
                End With
                With .Add("Acquired_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0.00))"
                End With
                With .Add("Adjusted_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0086,0.00))"
                End With
                With .Add("Stated_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0087,0.00))"
                End With
                With .Add("Net_Cost_of_Goods_Sold_Amt", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0) * convert(decimal(18,2),dbo.TransInventoryLedger.Qty) * -1)"
                End With
                With .Add("Transaction_Type", SqlDbType.VarChar)
                    .Expression = "CASE dbo.TransInventoryLedger.TransactionType WHEN 'Invoice' THEN 'SLC' else 'RTC' END"
                    '.Expression = "dbo.TransInventoryLedger.TransactionType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                        "dbo.TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInventoryLedger.FowstID INNER JOIN " & vbCrLf &
                        "dbo.LibraryOrganizationPIM ON dbo.TransInventoryLedger.ProductID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct LEFT JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProductVariableFeatures001 ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductVariableFeatures001.ProductID ON  " & vbCrLf &
                        "dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                        "OUTER APPLY ( " & vbCrLf &
                        "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoiceItems.TaxSales as VatExcluded, TransInvoice.InvoiceDate as TransactionDate, TransInvoiceItems.DiscountSales as Discount  " & vbCrLf &
                        "FROM TransInvoice INNER JOIN TransInvoiceItems on TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
                        "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransInvoiceItems.ProductID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                        "UNION " & vbCrLf &
                        "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, (dbo.TransMemoSalesReturn.TotalAmount - TransMemoSalesReturn.TaxAmount ) * -1 as VatExcluded, TransMemo.MemoDate as TransactionDate, '' as Discount  " & vbCrLf &
                        "FROM TransMemo " & vbCrLf &
                        "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransMemoSalesReturn.ProductID " & vbCrLf &
                        "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                        ") tSource"

            .SqlWhere = "WHERE (TransactionType = 'Sales Returns' OR TransactionType = 'Invoice') AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlOrder = "ORDER BY tSource.TransName"

            'pDT = .Requery
            pDT = mProData.Execute(.CreateSql)

            Dim m_date As Date = DateTo
            TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\Sales_" & AccountID & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

            'Return "POS_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
            Return True
        End With

    End Function

    Function GetMonthInventoryReportVersion2(AccountID As String, ByVal DateFrom As Date, ByVal DateTo As Date) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = mProData

            .Distinct = True
            With .Fields
                With .Add("Account_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID2"
                End With
                With .Add("Store_ID", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID2"
                End With
                With .Add("Transaction_ID", SqlDbType.VarChar)
                    .Expression = "ISNULL(tSource.TransName, TransInventoryLedger.TransactionType)"
                End With
                With .Add("Inv_Mvmt_Date", SqlDbType.Date)
                    .Expression = "replace(convert(date, tSource.TransactionDate),'-','')"
                End With
                With .Add("Product_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProductVariableFeatures001.C0017"
                End With
                With .Add("UPC_Code", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                End With
                With .Add("Inv_Mvmt_Qty", SqlDbType.Float)
                    .Expression = "convert(int,TransInventoryLedger.Qty)"
                End With
                With .Add("Acquired_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0.00))"
                End With
                With .Add("Adjusted_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0086,0.00))"
                End With
                With .Add("Stated_Cost", SqlDbType.Decimal)
                    .Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0087,0.00))"
                End With
                With .Add("Transaction_Type", SqlDbType.Int)
                    .Expression = "CASE TransInventoryLedger.TransactionType " &
                                "WHEN 'Invoice' THEN 'SLC' " &
                                "WHEN 'Sales Returns' THEN 'RTC' " &
                                "WHEN 'Receiving' THEN 'TIN' " &
                                "WHEN 'TransferOut' THEN 'TOT' " &
                                "WHEN 'InventoryCount' THEN 'ADJ' " &
                                "END"
                    '.Expression = "TransInventoryLedger.TransactionType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProduct.ID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInventoryLedger AS TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID ON  " & vbCrLf &
                      "dbo.LibraryOrganizationPIM.ProductID = TransInventoryLedger.ProductID " & vbCrLf &
                        "OUTER APPLY ( " & vbCrLf &
                      "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoice.InvoiceDate As TransactionDate " & vbCrLf &
                         "FROM TransInvoice " & vbCrLf &
                         "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                         "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                         "FROM TransReceiving " & vbCrLf &
                         "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Receiving' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                         "FROM TransReceiving " & vbCrLf &
                         "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'TransferOut' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, TransMemo.MemoDate As TransactionDate " & vbCrLf &
                         "FROM TransMemo " & vbCrLf &
                         "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                         "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                        "UNION " & vbCrLf &
                         "SELECT replace(CONVERT(varchar, CONVERT(date,TransInventoryProductCount.InventoryCountDate)),'-','') AS TransName,  TransInventoryProductCount.InventoryCountDate As TransactionDate" & vbCrLf &
                         "FROM TransInventoryProductCount " & vbCrLf &
                         "WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                         "AND TransInventoryLedger.TransactionType = 'InventoryCount' " & vbCrLf &
                        ") tSource"

            .SqlWhere = "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND dbo.LibraryOrganizationPIM.IsDeactivated = 0 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlOrder = "ORDER BY Inv_Mvmt_Date"

            pDT = mProData.Execute(.CreateSql)

            Dim m_date As Date = CDate(DateTo)

            TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\InvtMvmt_" & AccountID & "_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

            Return True
        End With

    End Function

    Function GetAccountID(OrganizationID As String) As String

        Dim pSql As String
        pSql = "select top 1 AccountID2 from MobizLibraryOrganizationFOWST where OrganizationID = " & ReadyString(OrganizationID) & " AND IsIncludeSpecialReport = 1"

        Return CStr2(mProData.ExecuteScalar(pSql))

    End Function

#End Region

#Region "Nike ShooperTrak"

    Sub ShopperTrakReport(ByVal CompanyDBName As String)
        Dim pSql As String
        Dim dtSchedule As DataTable
        Dim dtPrevSchedule As DataTable
        Dim fileEntry As String
        Dim m_Day As Integer

        'Dim t As New ShopperTrakController
        Dim SalesDataFilePath As String
        Dim LaborDataFilePath As String

        If (CompanyDBName.Contains("Nike")) Then
            'check if new sched should be applied

            m_Day = Today.DayOfWeek

            If m_Day = 0 Then
                pSql = "Select top 1 * from LibraryOrganizationWorkScheduleEmployee where convert(date,EffectivityDateFrom) = " & ReadyString(Today)

                dtSchedule = mProData.Execute(pSql)

                If dtSchedule.Rows.Count > 0 Then 'new sched found
                    'get last schedule and create csv file

                    Dim pFinder As Finder

                    pFinder = New Finder

                    With pFinder
                        .ProData = mProData

                        .Distinct = True

                        With .Fields
                            With .Add("Account_ID", SqlDbType.UniqueIdentifier)
                                .Searchable = False
                                .Expression = "dbo.MobizLibraryOrganizationFOWST.ShopperTrakAccountID"
                            End With
                            .Add("PeopleNo", SqlDbType.Int)
                            .Add("MondayFrom", SqlDbType.DateTime2)
                            .Add("MondayTo", SqlDbType.DateTime2)
                            .Add("MondayLunchBreakFrom", SqlDbType.DateTime2)
                            .Add("MondayLunchBreakTo", SqlDbType.DateTime2)
                            .Add("MondayDinnerBreakFrom", SqlDbType.DateTime2)
                            .Add("MondayDinnerBreakTo", SqlDbType.DateTime2)
                            .Add("TuesdayFrom", SqlDbType.DateTime2)
                            .Add("TuesdayTo", SqlDbType.DateTime2)
                            .Add("TuesdayLunchBreakFrom", SqlDbType.DateTime2)
                            .Add("TuesdayLunchBreakTo", SqlDbType.DateTime2)
                            .Add("TuesdayDinnerBreakFrom", SqlDbType.DateTime2)
                            .Add("TuesdayDinnerBreakTo", SqlDbType.DateTime2)
                            .Add("WednesdayFrom", SqlDbType.DateTime2)
                            .Add("WednesdayTo", SqlDbType.DateTime2)
                            .Add("WednesdayLunchBreakFrom", SqlDbType.DateTime2)
                            .Add("WednesdayLunchBreakTo", SqlDbType.DateTime2)
                            .Add("WednesdayDinnerBreakFrom", SqlDbType.DateTime2)
                            .Add("WednesdayDinnerBreakTo", SqlDbType.DateTime2)
                            .Add("ThursdayFrom", SqlDbType.DateTime2)
                            .Add("ThursdayTo", SqlDbType.DateTime2)
                            .Add("ThursdayLunchBreakFrom", SqlDbType.DateTime2)
                            .Add("ThursdayLunchBreakTo", SqlDbType.DateTime2)
                            .Add("ThursdayDinnerBreakFrom", SqlDbType.DateTime2)
                            .Add("ThursdayDinnerBreakTo", SqlDbType.DateTime2)
                            .Add("FridayFrom", SqlDbType.DateTime2)
                            .Add("FridayTo", SqlDbType.DateTime2)
                            .Add("FridayLunchBreakFrom", SqlDbType.DateTime2)
                            .Add("FridayLunchBreakTo", SqlDbType.DateTime2)
                            .Add("FridayDinnerBreakFrom", SqlDbType.DateTime2)
                            .Add("FridayDinnerBreakTo", SqlDbType.DateTime2)
                            .Add("SaturdayFrom", SqlDbType.DateTime2)
                            .Add("SaturdayTo", SqlDbType.DateTime2)
                            .Add("SaturdayLunchBreakFrom", SqlDbType.DateTime2)
                            .Add("SaturdayLunchBreakTo", SqlDbType.DateTime2)
                            .Add("SaturdayDinnerBreakFrom", SqlDbType.DateTime2)
                            .Add("SaturdayDinnerBreakTo", SqlDbType.DateTime2)
                            .Add("SundayFrom", SqlDbType.DateTime2)
                            .Add("SundayTo", SqlDbType.DateTime2)
                            .Add("SundayLunchBreakFrom", SqlDbType.DateTime2)
                            .Add("SundayLunchBreakTo", SqlDbType.DateTime2)
                            .Add("SundayDinnerBreakFrom", SqlDbType.DateTime2)
                            .Add("SundayDinnerBreakTo", SqlDbType.DateTime2)
                        End With

                        .SqlFrom = "FROM dbo.LibraryOrganizationWorkSchedule INNER JOIN " & vbCrLf &
                              "dbo.LibraryOrganizationWorkScheduleEmployee ON  " & vbCrLf &
                              "dbo.LibraryOrganizationWorkSchedule.ID = dbo.LibraryOrganizationWorkScheduleEmployee.WorkScheduleID INNER JOIN " & vbCrLf &
                              "dbo.MobizLibraryOrganizationFOWST ON dbo.LibraryOrganizationWorkScheduleEmployee.FowstID = dbo.MobizLibraryOrganizationFOWST.ID INNER JOIN " & vbCrLf &
                              "dbo.LibraryOrganizationEmployee ON dbo.LibraryOrganizationWorkScheduleEmployee.PeopleID = dbo.LibraryOrganizationEmployee.ID"

                        .SqlWhere = "WHERE IsIncludeToShopperTrakReport = 1  AND convert(date,EffectivityDateFrom) = " & ReadyString(DateAdd(DateInterval.Day, -7, Today)) & " AND convert(date,EffectivityDateTo) = " & ReadyString(DateAdd(DateInterval.Day, -1, Today))

                        dtPrevSchedule = mProData.Execute(.CreateSql)
                    End With

                    Dim CurrentDate As Date = CDate(DateAdd(DateInterval.Day, -7, Today))
                    Dim i As Integer
                    Dim sb As StringBuilder = New StringBuilder()
                    Dim m_DayOfWeek As String

                    While DateDiff(DateInterval.Day, CurrentDate, CDate(DateAdd(DateInterval.Day, -1, Today))) >= 0

                        Select Case CInt(DateTime.Parse(CurrentDate).DayOfWeek)
                            Case 0
                                m_DayOfWeek = "Sunday"
                            Case 1
                                m_DayOfWeek = "Monday"
                            Case 2
                                m_DayOfWeek = "Tuesday"
                            Case 3
                                m_DayOfWeek = "Wednesday"
                            Case 4
                                m_DayOfWeek = "Thursday"
                            Case 5
                                m_DayOfWeek = "Friday"
                            Case 6
                                m_DayOfWeek = "Saturday"
                        End Select

                        dtPrevSchedule.DefaultView.RowFilter = m_DayOfWeek & "FROM IS NOT NULL "

                        Dim dt As DataTable = dtPrevSchedule.DefaultView.ToTable
                        Dim j As Integer = 0

                        If dt.Rows.Count > 0 Then
                            For j = 0 To dt.Rows.Count - 1
                                If IsDate(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakFrom")) Then
                                    sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "FROM"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakFrom"), "HHmmss"))
                                    sb.AppendLine()
                                    sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakTo"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "To"), "HHmmss"))
                                End If
                                If IsDate(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakFrom")) Then
                                    sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "FROM"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakFrom"), "HHmmss"))
                                    sb.AppendLine()
                                    sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakTo"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "To"), "HHmmss"))
                                End If
                                sb.AppendLine()
                            Next
                        End If
                        CurrentDate = DateAdd(DateInterval.Day, 1, CurrentDate)
                    End While

                    FileText(Common.ServerPath & "temp\LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt") = sb.ToString

                    LaborDataFilePath = "LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt"

                    'sFtp = New Renci.SshNet.SftpClient("data.shoppertrak.com", "athletic", "waxQ85Whisp")
                    'sFtp.Connect()

                    'fileEntry = Common.ServerPath & "temp\LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -1, Today)), "yyyyMMdd") & ".txt"
                    'sFtp.ChangeDirectory("Labor")

                    'Try
                    '    myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                    '    sFtp.UploadFile(myFileStream, fileEntry, True)
                    '    myFileStream.Close()

                    'Catch ex As Exception
                    '    AppendToDebug(ex.Message)
                    'End Try

#If DEBUG Then
                    sFtp = New Renci.SshNet.SftpClient("192.168.1.80", "Administrator", "secret")
                    sFtp.Connect()
                    fileEntry = Common.ServerPath & "temp\" & LaborDataFilePath
                    sFtp.ChangeDirectory("LABOR")

                    Try
                        myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                        sFtp.UploadFile(myFileStream, LaborDataFilePath, True)
                        myFileStream.Close()

                        SendMail("mobizcompleteretail@outlook.com",
                                "francis.hernandez@mobiz-apps.com",
                                "Mobiz",
                                "LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd"),
                                "ShopperTrak Labor Data File",
                                "smtp.live.com",
                                587,
                                "mobizcompleteretail@outlook.com",
                                "m0b!zp30pl30nly!", , ,
                                "Labor_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt",
                                fileEntry,
                                True)

                    Catch ex As Exception
                        AppendToDebug(ex.Message)
                    End Try


#Else
                    sFtp = New Renci.SshNet.SftpClient("data.shoppertrak.com", "athletic", "waxQ85Whisp")
                        sFtp.Connect()
                        fileEntry = Common.ServerPath & "temp\" & LaborDataFilePath
                        sFtp.ChangeDirectory("LABOR")

                        Try
                            myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                            sFtp.UploadFile(myFileStream, LaborDataFilePath, True)
                            myFileStream.Close()

                            SendMail("mobizcompleteretail@outlook.com",
                                    "francis.hernandez@mobiz-apps.com",
                                    "Mobiz",
                                    "LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd"),
                                    "ShopperTrak Labor Data File",
                                    "smtp.live.com",
                                    587,
                                    "mobizcompleteretail@outlook.com",
                                    "m0b!zp30pl30nly!", , ,
                                    "Labor_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt",
                                    fileEntry,
                                    True)

                        Catch ex As Exception
                            AppendToDebug(ex.Message)
                        End Try
#End If

                    '"christinegrace.perez@yahoo.com",

                End If



                SalesDataFilePath = ShopperTrakSalesDataReport(DateAdd(DateInterval.Day, -7, Today), DateAdd(DateInterval.Day, -1, Today))

                If Not SalesDataFilePath Is Nothing Then
#If DEBUG Then
                    sFtp = New Renci.SshNet.SftpClient("192.168.1.80", "Administrator", "secret")
                    sFtp.Connect()

                    fileEntry = Common.ServerPath & "temp\" & SalesDataFilePath
                    sFtp.ChangeDirectory("HourlySummary")

                    Try
                        myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                        sFtp.UploadFile(myFileStream, SalesDataFilePath, True)
                        myFileStream.Close()

                        SendMail("mobizcompleteretail@outlook.com",
                                "francis.hernandez@mobiz-apps.com",
                                "Mobiz",
                                "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -1, Today)), "yyyyMMdd"),
                                "ShopperTrak Sales Data File",
                                "smtp.live.com",
                                587,
                                "mobizcompleteretail@outlook.com",
                                "m0b!zp30pl30nly!", , ,
                                "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -1, Today)), "yyyyMMdd") & ".txt",
                                fileEntry,
                                True)

                    Catch ex As Exception
                        AppendToDebug(ex.Message)
                    End Try
#Else
                    sFtp = New Renci.SshNet.SftpClient("data.shoppertrak.com", "athletic", "waxQ85Whisp")
                    sFtp.Connect()

                    fileEntry = Common.ServerPath & "temp\" & SalesDataFilePath
                    sFtp.ChangeDirectory("HourlySummary")

                    Try
                        myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                        sFtp.UploadFile(myFileStream, SalesDataFilePath, True)
                        myFileStream.Close()

                    Catch ex As Exception
                        AppendToDebug(ex.Message)
                    End Try

                    SendMail("mobizcompleteretail@outlook.com",
                                                     "francis.hernandez@mobiz-apps.com",
                                                     "Mobiz",
                                                     "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd"),
                                                     "ShopperTrak Labor Data File",
                                                     "smtp.live.com",
                                                     587,
                                                     "mobizcompleteretail@outlook.com",
                                                     "m0b!zp30pl30nly!", , ,
                                                     "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt",
                                                     fileEntry,
                                                     True)
#End If
                End If




            End If
        End If
    End Sub

    <HttpPost()>
    Function ShopperTrakSalesDataReport(ByVal DateFrom As String, ByVal DateTo As String) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1


        'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        If Not System.IO.File.Exists(Common.ServerPath & "temp\SALES_" & Format(CDate(DateTo), "yyyyMMdd") & ".txt") Then

            pFinder = New Finder

            With pFinder
                .ProData = mProData

                With .Fields
                    With .Add("Account_ID", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.ShopperTrakAccountID"
                    End With


                    With .Add("InvoiceDate", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "replace(CONVERT(VARCHAR(10),dbo.TransInvoice.InvoiceDate,21),'-','')"
                    End With

                    With .Add("FullInvoiceDate", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "dbo.TransInvoice.InvoiceDate"
                    End With

                    With .Add("Sales", SqlDbType.VarChar)
                        .Expression = "dbo.TransInvoice.TotalAmount - dbo.TransInvoice.DiscountAmount"
                    End With

                    With .Add("Category", SqlDbType.VarChar)
                        .Expression = "''"
                    End With

                    With .Add("Currency", SqlDbType.VarChar)
                        .Expression = "'PHP'"
                    End With

                End With

                .SqlFrom = "FROM dbo.TransInvoice INNER JOIN " &
                            "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID "

                .SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND " &
                            "CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " &
                            ReadyString(DateTo) & " AND IsIncludeToShopperTrakReport = 1"

                .SqlOrder = "ORDER BY dbo.TransInvoice.InvoiceDate"

                pDT = mProData.Execute(.CreateSql)

                Dim DaysApart As Integer

                DaysApart = DateDiff(DateInterval.Day, CDate(DateFrom), CDate(DateTo))

                Dim CurrentDate As Date = CDate(DateFrom)
                Dim i As Integer
                Dim sb As StringBuilder = New StringBuilder()

                While DateDiff(DateInterval.Day, CurrentDate, CDate(DateTo)) >= 0
                    i = 0
                    While i < 23
                        pDT.DefaultView.RowFilter = "FullInvoiceDate >= '" & Format(CurrentDate, "yyyy-MM-dd " & i & ":00:01") & "' AND FullInvoiceDate <= '" & Format(CurrentDate, "yyyy-MM-dd " & i + 1 & ":00:00") & "'"
                        Dim dt As DataTable = pDT.DefaultView.ToTable
                        Dim j As Integer = 0
                        Dim TotalSales As Double = 0.0

                        If dt.Rows.Count > 0 Then
                            While j < dt.Rows.Count
                                TotalSales = TotalSales + dt.Rows(j).Item("Sales")
                                j += 1
                            End While
                            sb.Append(dt.Rows(0).Item("Account_ID") & "," & dt.Rows(0).Item("InvoiceDate") & "," & Format(i + 1, "##") & "0000," & Format(TotalSales, "#.00") & "," & dt.Rows.Count & ",,PHP")
                            sb.AppendLine()
                        End If
                        i += 1
                    End While

                    CurrentDate = DateAdd(DateInterval.Day, 1, CurrentDate)
                End While

                FileText(Common.ServerPath & "temp\SALES_" & Format(CDate(DateTo), "yyyyMMdd") & ".txt") = sb.ToString

                Return "SALES_" & Format(CDate(DateTo), "yyyyMMdd") & ".txt"
            End With
        End If
    End Function

#End Region

#Region "Nike Onitsuka"
    Sub OnitsukaReport(ByVal CompanyDBName As String)
        Dim m_Day As Integer

        Dim DateFrom As Date = CDate(DateAdd(DateInterval.Day, -1, Today))
        Dim DateTo As Date = CDate(DateAdd(DateInterval.Day, -1, Today))

        Dim clsReportToPrincipal As New ReportToPrincipal
        Dim m_Path As String

            Dim m_StartOfDay As Date = Today.AddDays(-4)

        m_Path = "DAILY SALES UPDATE_" & Today.Year & Format(Today.Month, "00") & Format(Today.Day, "00") & ".xls"

                'm_Path = GetMonthInventoryReport()
                'sellthru.sea@nike.com, christinegrace.perez@yahoo.com, 

                If (Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path)) Then
            If GetOnitsukaSalesReport(DateFrom, DateTo).Length > 0 Then
#If DEBUG Then
                SendMail("mobizcompleteretail@outlook.com",
                   "francis.hernandez@mobiz-apps.com",
                   "Mobiz",
                   Replace(m_Path, ".csv", ""),
                   Replace(m_Path, ".csv", ""),
                   "smtp.live.com",
                   587,
                   "mobizcompleteretail@outlook.com",
                   "m0b!zp30pl30nly!", , ,
                   m_Path,
                   Common.ServerPath & "temp\" & m_Path,
                   True)
#Else
                            SendMail("mobizcompleteretail@outlook.com",
                             "christinegrace.perez@yahoo.com, francis.hernandez@mobiz-apps.com, gmtmanluctao@yahoo.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "mobizcompleteretail@outlook.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#End If

            End If
                End If
    End Sub

    Function GetOnitsukaSalesReport(ByVal DateFrom As Date, ByVal DateTo As Date) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String

        pFinder = New Finder

        With pFinder
            .ProData = mProData

            With .Fields
                With .Add("Transaction_Date", SqlDbType.VarChar)
                    .Expression = "replace(convert(varchar, tSource.TransactionDate, 6),' ','-')"
                End With
                With .Add("Day", SqlDbType.VarChar)
                    .Expression = "DATENAME(dw,tSource.TransactionDate)"
                End With
                With .Add("InvoiceNo", SqlDbType.VarChar)
                    .Expression = "tSource.TransName"
                End With
                With .Add("Gender", SqlDbType.Date)
                    .Expression = "MobizLibraryPIMProductVariableFeatures001.C0043"
                End With
                With .Add("Category", SqlDbType.Float)
                    .Expression = "MobizLibraryPIMProduct.DivisionDescription"
                End With
                With .Add("Item", SqlDbType.VarChar)
                    .Expression = "SUBSTRING(dbo.MobizLibraryPIMProduct.ProductGroupNo,0,charindex('-',dbo.MobizLibraryPIMProduct.ProductGroupNo))"
                End With
                With .Add("Color", SqlDbType.Int)
                    .Expression = "replace(SUBSTRING(dbo.MobizLibraryPIMProduct.ProductGroupNo,charindex('-',dbo.MobizLibraryPIMProduct.ProductGroupNo),len(dbo.MobizLibraryPIMProduct.ProductGroupNo)+1),'-','')"
                End With
                With .Add("ColorDescription", SqlDbType.Float)
                    .Expression = "ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0047,'NA')"
                End With
                With .Add("Size", SqlDbType.VarChar)
                    .Expression = "REPLACE(ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0017,'MISC'),'.5','h')"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProductVariableFeatures001.C0008"
                End With
                With .Add("Qty", SqlDbType.VarChar)
                    .Expression = "convert(int,dbo.TransInventoryLedger.Qty * -1)"
                End With
                With .Add("UnitPrice", SqlDbType.VarChar)
                    '.Expression = "convert(decimal(18,2), ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0085,0))"
                    .Expression = "tSource.UnitPrice"
                End With
                With .Add("Payment", SqlDbType.VarChar)
                    .Expression = "CASE " &
                                "WHEN TransCustomerPayment.CashAmount>0 then 'CASH' " &
                                "WHEN TransCustomerPayment.CreditCardAmount>0 then 'Credit Card' " &
                                "WHEN TransCustomerPayment.DebitCardAmount>0 then 'Debit Card' " &
                                "WHEN TransCustomerPayment.SRVAmount>0 then 'SRV' " &
                                "WHEN TransCustomerPayment.OtherTypeAmount>0 then 'Others' " &
                                "End "
                End With
                With .Add("Amount", SqlDbType.VarChar)
                    .Expression = "convert(decimal(18,2), tSource.UnitPrice * convert(decimal(18,2),dbo.TransInventoryLedger.Qty) * -1) "
                End With
                With .Add("Transaction_Type", SqlDbType.VarChar)
                    .Expression = "CASE dbo.TransInventoryLedger.TransactionType WHEN 'Invoice' THEN 'Sales' else 'Sales Return' END"
                    '.Expression = "dbo.TransInventoryLedger.TransactionType"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                        "dbo.TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInventoryLedger.FowstID INNER JOIN  " & vbCrLf &
                        "dbo.LibraryOrganizationPIM ON dbo.TransInventoryLedger.ProductID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN  " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct LEFT JOIN  " & vbCrLf &
                        "dbo.MobizLibraryPIMProductVariableFeatures001 ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductVariableFeatures001.ProductID ON   " & vbCrLf &
                        "dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                        "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInventoryLedger.TransactionID " & vbCrLf &
                        "inner join TransCustomerPayment on TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID " & vbCrLf &
                        "OUTER APPLY (  " & vbCrLf &
                        "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoiceItems.UnitPrice as UnitPrice, TransInvoice.InvoiceDate as TransactionDate, TransInvoiceItems.DiscountSales as Discount  " & vbCrLf &
                        "FROM TransInvoice INNER JOIN TransInvoiceItems on TransInvoiceItems.InvoiceID = TransInvoice.ID  " & vbCrLf &
                        "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID  " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransInvoiceItems.ProductID  " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Invoice'  " & vbCrLf &
                        "UNION " & vbCrLf &
                        "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, (dbo.TransMemoSalesReturn.TotalAmount) * -1 as UnitPrice, TransMemo.MemoDate as TransactionDate, '' as Discount  " & vbCrLf &
                        "FROM TransMemo  " & vbCrLf &
                        "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID  " & vbCrLf &
                        "JOIN TransSalesReturnLedger  ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.DestinationPaymentID IS NULL  " & vbCrLf &
                        "AND TransInventoryLedger.ProductID = TransMemoSalesReturn.ProductID  " & vbCrLf &
                        "WHERE TransMemo.ID = TransInventoryLedger.TransactionID  " & vbCrLf &
                        "AND TransInventoryLedger.TransactionType = 'Sales Returns'  " & vbCrLf &
                        ") tSource "

            .SqlWhere = "WHERE (TransactionType = 'Sales Returns' OR TransactionType = 'Invoice') AND dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            If CBool2(Common.WebRequest.Data("IsForToraSports")) Then
                .SqlWhere &= " AND dbo.MobizLibraryPIMProduct.ProductNo NOT like '%STI%'"
            Else
                .SqlWhere &= " AND dbo.MobizLibraryPIMProduct.ProductNo like '%STI%'"
            End If

            .SqlOrder = "ORDER BY tSource.TransName"

            'pDT = .Requery
            Try
                pDT = mProData.Execute(.CreateSql)
            Catch ex As Exception
                pDT = Common.WebRequest.ProData.Execute(.CreateSql)
            End Try


            Dim m_date As Date = DateTo
            Dim ReportFileName As String

            If CBool2(Common.WebRequest.Data("IsForToraSports")) Then
                ReportFileName = "DAILY_SALES_UPDATE_Sonak_Sports_Specialists_Inc_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".xls"
            Else
                ReportFileName = "DAILY_SALES_UPDATE_Sports_Town_Inc_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".xls"
            End If
            TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\" & ReportFileName)

            'Return "DAILY_SALES_UPDATE_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".xls"
            Return ReportFileName
        End With

    End Function

    Function GetOnitsukaInventoryReport(ByVal DateFrom As Date, ByVal DateTo As Date) As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim pSql As String

        pSql = "Select " & vbCrLf

        pSql &= "Season" & vbCrLf
        pSql &= ", Gender" & vbCrLf
        pSql &= ", ProductName " & vbCrLf
        pSql &= ", Item_Code" & vbCrLf
        pSql &= ", Color_Code" & vbCrLf
        pSql &= ", FirstRecDate" & vbCrLf
        pSql &= ", LastRecDate" & vbCrLf
        pSql &= ", RecQty" & vbCrLf
        pSql &= ", StockTransferQty" & vbCrLf
        pSql &= ", SalesReturnQty" & vbCrLf
        pSql &= ", Beginning_Inventory" & vbCrLf
        pSql &= ",ISNULL([3.5],'') [3.5] " & vbCrLf
        pSql &= ",ISNULL([4],'') [4]" & vbCrLf
        pSql &= ",ISNULL([4.5],'') [4.5]" & vbCrLf
        pSql &= ",ISNULL([5],'') [5]" & vbCrLf
        pSql &= ",ISNULL([5.5],'') [5.5]" & vbCrLf
        pSql &= ",ISNULL([6],'') [6]" & vbCrLf
        pSql &= ",ISNULL([6.5],'') [6.5]" & vbCrLf
        pSql &= ",ISNULL([7],'') [7]" & vbCrLf
        pSql &= ",ISNULL([7.5],'') [7.5]" & vbCrLf
        pSql &= ",ISNULL([8.0],'') [8.0]" & vbCrLf
        pSql &= ",ISNULL([8.5],'') [8.5]" & vbCrLf
        pSql &= ",ISNULL([9.0],'') [9.0]" & vbCrLf
        pSql &= ",ISNULL([9.5],'') [9.5]" & vbCrLf
        pSql &= ",ISNULL([10.0],'') [10.0]" & vbCrLf
        pSql &= ",ISNULL([10.5],'') [10.5]" & vbCrLf
        pSql &= ",ISNULL([11],'') [11]" & vbCrLf
        pSql &= ",ISNULL([11.5],'') [11.5]" & vbCrLf
        pSql &= ",ISNULL([12],'') [12]" & vbCrLf
        pSql &= ",ISNULL([12.5],'') [12.5]" & vbCrLf
        pSql &= ",ISNULL([13],'') [13]" & vbCrLf
        pSql &= ",ISNULL([14],'') [14]" & vbCrLf
        pSql &= ", [OnHand]" & vbCrLf
        pSql &= ", [Price]" & vbCrLf
        pSql &= ", [Extended_Price]" & vbCrLf
        pSql &= ", [Inv_Qty_Sold]" & vbCrLf
        pSql &= ", [Total_Sales]" & vbCrLf
        pSql &= ", SellThru" & vbCrLf
        pSql &= "from (SELECT DISTINCT TOP 100 PERCENT  " & vbCrLf
        pSql &= "t.ProductName " & vbCrLf
        pSql &= ",t.Season " & vbCrLf
        pSql &= ", t.Gender [Gender]" & vbCrLf
        pSql &= ",SUBSTRING(t.Product_Code,0,charindex('-',t.Product_Code)) Item_Code" & vbCrLf
        pSql &= ",replace(SUBSTRING(t.Product_Code,charindex('-',t.Product_Code),len(t.Product_Code)+1),'-','') Color_Code" & vbCrLf
        pSql &= ", t.Size AS [Size]" & vbCrLf
        pSql &= ", ISNULL(convert(int,SUM(t.balance)),0) AS [OnHand]" & vbCrLf
        pSql &= ", ISNULL(convert(int,SUM(t.balance)),0) AS [Inv_Balance_Qty]" & vbCrLf
        pSql &= ", ISNULL(convert(int,SUM(t.Qty)),0) * -1 AS [Inv_Qty_Sold]" & vbCrLf
        pSql &= ", ISNULL(t.Price,0) Price" & vbCrLf
        pSql &= ", ISNULL(convert(int,SUM(t.balance)),0) * ISNULL(t.Price,0) Extended_Price" & vbCrLf
        pSql &= ", ISNULL(convert(int,SUM(t.Qty)),0) * -1 * ISNULL(t.Price,0) Total_Sales" & vbCrLf
        pSql &= ",FirstRecDate" & vbCrLf
        pSql &= ",LastRecDate" & vbCrLf
        pSql &= ",ISNULL(RecQty,0) AS RecQty" & vbCrLf
        pSql &= ",ISNULL(StockTransferQty,0) AS StockTransferQty" & vbCrLf
        pSql &= ",ISNULL(SalesReturnQty,0) AS SalesReturnQty" & vbCrLf
        pSql &= ",ISNULL(RecQty,0) - ISNULL(StockTransferQty,0) + ISNULL(SalesReturnQty,0) AS Beginning_Inventory" & vbCrLf
        pSql &= ",convert(varchar,(case when (RecQty - StockTransferQty + SalesReturnQty) = 0 then 0 else round(convert(float,(convert(int,SUM(t.Qty)) * -1)  / (RecQty - StockTransferQty + SalesReturnQty) * 100),2) end)) + '%' AS SellThru" & vbCrLf
        pSql &= "FROM (SELECT DISTINCT " & vbCrLf
        pSql &= "dbo.MobizLibraryPIMProduct.ID" & vbCrLf
        pSql &= ", dbo.MobizLibraryPIMProduct.ProductGroupNo AS Product_Code" & vbCrLf
        pSql &= ", dbo.MobizLibraryPIMProductVariableFeatures001.C0017 AS Size" & vbCrLf
        pSql &= ", MobizLibraryPIMProductVariableFeatures001.C0043 AS Gender" & vbCrLf
        pSql &= ", MobizLibraryPIMProductVariableFeatures001.C0008 AS ProductName" & vbCrLf
        pSql &= ", MobizLibraryPIMProductVariableFeatures001.C0033 AS Season" & vbCrLf
        pSql &= ", bal1.Balance" & vbCrLf
        pSql &= ", bal2.Qty" & vbCrLf
        pSql &= ", rp1.Price" & vbCrLf
        pSql &= ", rsAsc.DateReceived AS FirstRecDate" & vbCrLf
        pSql &= ", rsDesc.DateReceived AS LastRecDate" & vbCrLf
        pSql &= ", rsQuantity.QuantityGood AS RecQty" & vbCrLf
        pSql &= ", salesReturnQuantity.QuantityReturned AS SalesReturnQty" & vbCrLf
        pSql &= ", rstQuantity.QuantityGood AS StockTransferQty" & vbCrLf
        pSql &= "FROM " & vbCrLf
        pSql &= "dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.LibraryOrganizationPIM.ProductID LEFT OUTER JOIN " & vbCrLf &
                "dbo.MobizLibraryOrganization INNER JOIN " & vbCrLf &
                "dbo.MobizLibraryOrganizationFOWST ON dbo.MobizLibraryOrganization.ID = dbo.MobizLibraryOrganizationFOWST.OrganizationID INNER JOIN " & vbCrLf &
                "dbo.TransInventoryLedger AS t0 INNER JOIN  " & vbCrLf &
                "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = t0.ProductID ON dbo.MobizLibraryOrganizationFOWST.ID = t0.FowstID ON " & vbCrLf &
                "dbo.LibraryOrganizationPIM.ProductID = t0.ProductID "
        pSql &= "outer apply(  " & vbCrLf &
                "select top 1 Balance from TransInventoryLedger as t1 where t1.ProductID = t0.ProductID And t1.FowstID = t0.FowstID  " & vbCrLf &
                "And t1.BinID = t0.BinID order by LastUpdated desc )  bal1 "
        pSql &= "outer apply( " & vbCrLf &
                "select SUM(Qty) Qty from TransInventoryLedger as t2 where t2.ProductID = t0.ProductID And t2.FowstID = t0.FowstID " & vbCrLf &
                "And t2.BinID = t0.BinID And t2.TransactionType='Invoice')  bal2 "
        pSql &= "outer apply( " & vbCrLf &
                "select top 1 Price Price from MobizLibraryPIMProductPriceLevel pr " &
                "where pr.PriceLevelID = '01000012-0005-0000-0000-000000000000' aND pr.ProductID = t0.ProductID) rp1 "
        pSql &= "outer apply(  " & vbCrLf &
                "select top 1 convert(date,DocumentDate) DateReceived from TransReceiving " & vbCrLf &
                "inner join TransReceivingStock rs on rs.ReceivingID = TransReceiving.ID " & vbCrLf &
                "where rs.ProductID = t0.ProductID AND TransReceiving.IsReceiving=1 " & vbCrLf &
                "order by DocumentDate) rsAsc "
        pSql &= "outer apply( " & vbCrLf &
                "select top 1 convert(date, DocumentDate) DateReceived from TransReceiving " & vbCrLf &
                "inner join TransReceivingStock rs on rs.ReceivingID = TransReceiving.ID " & vbCrLf &
                "where rs.ProductID = t0.ProductID AND TransReceiving.IsReceiving=1" & vbCrLf &
                "order by DocumentDate) rsDesc "
        pSql &= "outer apply(" & vbCrLf &
                "select SUM(QuantityGood) QuantityGood from TransReceiving " & vbCrLf &
                "inner join TransReceivingStock rs on rs.ReceivingID = TransReceiving.ID " & vbCrLf &
                "where rs.ProductID = t0.ProductID AND TransReceiving.IsReceiving=1 " & vbCrLf &
                ") rsQuantity "
        pSql &= "outer apply( " & vbCrLf &
                "select SUM(QuantityGood) QuantityGood from TransReceiving " & vbCrLf &
                "inner join TransReceivingStock rs on rs.ReceivingID = TransReceiving.ID " & vbCrLf &
                "where rs.ProductID = t0.ProductID AND TransReceiving.IsReceiving=0 " & vbCrLf &
                ") rstQuantity "
        pSql &= "outer apply( " & vbCrLf &
                "SELECT convert(int,TransMemoSalesReturn.QuantityReturned) QuantityReturned " & vbCrLf &
                "FROM TransMemoSalesReturn where TransMemoSalesReturn.ProductID  = t0.ProductID  " & vbCrLf &
                ") salesReturnQuantity "
        pSql &= "WHERE dbo.LibraryOrganizationPIM.IsDeactivated = 0 and "

        If CBool2(Common.WebRequest.Data("IsForToraSports")) Then
            pSql &= " dbo.MobizLibraryPIMProduct.ProductNo NOT like '%STI%' AND "
        Else
            pSql &= " dbo.MobizLibraryPIMProduct.ProductNo like '%STI%' AND "
        End If

        pSql &= "dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0) t "
        pSql &= "group by Size, Product_Code, Gender, ProductName, Season, Price, FirstRecDate, LastRecDate, RecQty, StockTransferQty, SalesReturnQty " 'edit this next time
        pSql &= ") Report "
        pSql &= "pivot (max(Inv_Balance_Qty) for Size in ([3.5],[4],[4.5],[5],[5.5],[6],[6.5],[7],[7.5],[8.0],[8.5],[9.0],[9.5],[10.0],[10.5],[11],[11.5],[12],[12.5],[13],[14])) as t"

        'pDT = .Requery
        Try
            pDT = mProData.Execute(pSql)
        Catch ex As Exception
            Try
                pDT = Common.WebRequest.ProData.Execute(pSql)
            Catch ex2 As Exception
                AppendToDebug(ex2.Message)
            End Try
        End Try

        Dim m_date As Date = DateTo

        Dim ReportFileName As String

        If CBool2(Common.WebRequest.Data("IsForToraSports")) Then
            ReportFileName = "Inventory_And_Sales_Report_Sonak_Sports_Specialists_Inc_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".xls"
        Else
            ReportFileName = "Inventory_And_Sales_Sports_Town_Inc_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".xls"
        End If

        TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\" & ReportFileName)

        'Return "Inventory_And_Sales_Report_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".xls"
        Return ReportFileName

    End Function
#End Region

#Region "Potato Corner"

    Private Function IsRunAWSDataSender() As Boolean

        Dim pFilename = Common.ServerPath & "settings\IsRunAWSDataSender.txt"
        Dim pText = CStr2(FileText(pFilename))
        Dim pOut = CBool2(pText)
        FileText(pFilename) = pOut

        Return pOut

    End Function

    Function RunAWSDataSender()

        If IsRunAWSDataSender() = False Then
            Exit Function
        End If

        Dim counter As Integer = 0
        Dim hournow As Integer = 0

        While True

            'Try
            '    Dim firehoseClient As AmazonKinesisFirehoseClient
            '    Dim putRecordRequest As New PutRecordRequest
            '    Dim putRecordRequestBatch As New PutRecordBatchRequest
            '    Dim mData As String
            '    Dim mRecord As Record
            '    Dim mStream As MemoryStream
            '    Dim pSql As String
            '    Dim pDTData As DataTable

            '    pSql = "Select top 1 " & vbCrLf &
            '    "ID TransactionID, " & vbCrLf &
            '    "FowstID StoreID, " & vbCrLf &
            '    "CustomerID, " & vbCrLf &
            '    "InvoiceDate, " & vbCrLf &
            '    "InvoiceNumber, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),GrossInvoiceAmount)) TotalAmount, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),SCDiscount)) SCDiscount, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),CashNetOfChange)) CashPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),GCash)) GCashPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),PayMaya)) PayMayaPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),FoodPanda)) FoodPandaPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),ShopeeEVoucher)) ShopeeEVoucherPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),ShopeePay)) ShopeePayPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),Eatigo)) EatigoPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),DeliRush)) DeliRushPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),PoodTrip)) PoodTripPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),[Share Treats])) ShareTreatsPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),Booky)) BookyPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),Lazada)) LazadaPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),GrabFood)) GrabFoodPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),LocalDelivery)) LocalDeliveryPayment, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),PotatoCornerGC)) PotatoCornerGC, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),Pickaroo)) Pickaroo, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),DebitCardAmount)) DebitCardAmount, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),CreditCardAmount)) CreditCardAmount, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),Riderko)) Riderko, " & vbCrLf &
            '    "convert(varchar,convert(decimal(8,2),HomeCredit)) HomeCredit " & vbCrLf &
            '    "FROM CacheInvoices " & vbCrLf &
            '    "WHERE ISNULL(SentToFirehoseServer,0) = 0 " & vbCrLf &
            '    "order by InvoiceDate desc"

            '    pDTData = mProData.Execute(pSql)

            '    If pDTData.Rows.Count > 0 Then

            '        For i As Integer = 5 To pDTData.Columns.Count - 1
            '            pDTData.Rows(0).Item(i) = Format(CDec2(pDTData.Rows(0).Item(i)), "#,###,###.00")
            '        Next

            '        Dim oByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pDTData).Replace("[", "").Replace("]", ""))

            '        Dim tempstr = Encoding.UTF8.GetString(oByte)

            '        putRecordRequest.DeliveryStreamName = "poc-mobiz-potato-corner-stream"
            '        mStream = New MemoryStream(oByte)

            '        mRecord = New Record()
            '        mRecord.Data = mStream

            '        putRecordRequest.Record = mRecord

            '        firehoseClient = New AmazonKinesisFirehoseClient("AKIA355WGAKJHP2HYKFA", "8CVY3wI8X16WhEA6k4wHMw7NwjIxyI9hbxQZ1xAI", Amazon.RegionEndpoint.APSoutheast1)
            '        firehoseClient.PutRecord(putRecordRequest)

            '        pSql = "Update CacheInvoices set SentToFirehoseServer=1, DateSentToFirehoseServer=GETDATE() WHERE ID = " & ReadyString(CStr2(pDTData.Rows(0).Item("TransactionID")))
            '        mProData.Execute(pSql)

            '        counter += 1

            '        AppendToFile(Common.ServerPath & "temp\logs\FireHost.txt", vbCr & counter & ". " & CStr2(pDTData.Rows(0).Item("TransactionID")) & " || " & CStr2(Date.Now))

            '        'If Hour(Now) < 13 Or Hour(Now) > 19 Then
            '        '    'updating transinvoice items para sa caching kay dili makaya ug 1 run lang

            '        '    pSql = "update TOP (4000) TransInvoiceItems Set ProductNameInvoiceitems = (Select ProductName from MobizLibraryPIMProduct WHERE MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID) WHERE ProductNameInvoiceitems IS NULL"
            '        '    mProData.Execute(pSql)


            '        '    pSql = "update TOP (4000) TransInvoiceItems set Fries = (Select ISNULL(MobizLibraryPIMProductVariableFeatures001.c0093,'NonFries') FROM MobizLibraryPIMProductVariableFeatures001 WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems.ProductID) WHERE Fries IS NULL"
            '        '    mProData.Execute(pSql)

            '        '    pSql = "update TOP (4000) TransInvoiceItems set Size = (Select ISNULL(MobizLibraryPIMProductVariableFeatures001.C0036,'N/A') FROM MobizLibraryPIMProductVariableFeatures001 WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems.ProductID) WHERE Size IS NULL"
            '        '    mProData.Execute(pSql)
            '        'End If
            '    Else
            '        Threading.Thread.Sleep(60000)
            '    End If

            'Catch ex As Exception
            '    AppendToLog(ex.Message)
            '    Exit While
            'End Try

            'sleep for 3 seconds
            Threading.Thread.Sleep(3000)

            If hournow < Hour(Now) Then

                Dim pSql As String
                Dim count As Integer = 0

                count = CInt2(mProData.ExecuteScalar("Select COUNT(*) from TransImports where TransImports.ProcessStart is null"))
                pSql = "Insert into TransUnflushedDataCount(NoOfUnflushedFile) values(" & count & ")"

                mProData.Execute(pSql)

                hournow = Hour(Now)

                'sleep for 1 hour
                'Threading.Thread.Sleep(3600000)
            End If

        End While
    End Function

    Function CreateWeeklyReport(Optional customdatefrom As String = "", Optional customdateto As String = "")
        'Dim datefrom As Date = DateAdd(DateInterval.Day, -8, Date.Now)
        'Dim dateto As Date = DateAdd(DateInterval.Day, -2, Date.Now)
        Dim DateFrom As Date = DateAdd(DateInterval.Day, -7, Date.Now)
        Dim DateTo As Date = DateAdd(DateInterval.Day, -1, Date.Now)
        Dim isCustomizeReport As Boolean = False

        If String.IsNullOrWhiteSpace(customdatefrom) And String.IsNullOrWhiteSpace(customdateto) Then
            DateFrom = DateAdd(DateInterval.Day, -7, Date.Now)
            DateTo = DateAdd(DateInterval.Day, -1, Date.Now)
        Else
            DateFrom = CDate(customdatefrom)
            DateTo = CDate(customdateto)
            isCustomizeReport = True
        End If

        Dim reportCounter As Integer = 0
        Dim IsCompanyOwned As Boolean = True
        Dim EmailTitle As String
        Dim msg As String = ""

        While reportCounter <= 1

            msg = ""

            If reportCounter = 0 Then
                IsCompanyOwned = True
            Else
                IsCompanyOwned = False
            End If


            Dim arrFowst = GetDeviceUtilizationDetails(DateFrom, DateTo, True, IsCompanyOwned)
            Dim arrFowstSummary = GetDeviceUtilizationSummary(DateFrom, DateTo, True, , IsCompanyOwned)
            Dim headerStyles = "padding: 10px; text-align:center; white-space:nowrap; font-weight:bold;"
            Dim theadDeviceUtilUsageDetails = "<thead><tr style='background-color:#CCC'>"
            Dim tbodyDeviceUtilUsageDetails = "<tbody>"
            Dim bodyStyles = "padding: 5px; white-space:nowrap; "
            Dim ctrRow As Integer = 0
            Dim ctrColDate As Integer = 0
            Dim colsDate As String = ""
            Dim pFilename As String

            If IsCompanyOwned Then
                pFilename = "CO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy") & ".xlsx"
                EmailTitle = "CO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy")
            Else
                pFilename = "FO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy") & ".xlsx"
                EmailTitle = "FO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy")
            End If

            theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "' >#</th>"
            theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Region</th>"
            theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>No of Stores</th>"

            GenerateExcelDeviceUtilizationDetails(arrFowst, DateFrom, DateTo, pFilename)

            'theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>" & Format(DateFrom, "MMM dd yyyy") & " Utilization</th>"

            Dim DateFromOld As Date = DateFrom

            While (DateFromOld <= DateTo)
                theadDeviceUtilUsageDetails += "<th style='" + headerStyles + "'>" + DateFromOld.ToString("MMM dd") + "</th>"
                colsDate += "<td style='" + bodyStyles + " text-align:center;'></td>"
                ctrColDate += 1
                DateFromOld = DateAdd("d", 1, DateFromOld)
            End While

            theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Average</th>"
            theadDeviceUtilUsageDetails += "</tr></thead>"

            Dim totalStores As Integer = 0
            Dim totalclosedStores As Integer = 0
            Dim tdfooter As String = ""
            Dim footerperiod As New JsonDictionary
            Dim WeeklyTotal As Double
            For Each store As Object In arrFowstSummary
                ctrRow += 1
                Dim tr As String = "<tr style='background-color:  " & IIf(ctrRow Mod 2 = 0, "#f5f5f5", "#FFFFFF") & "'><td style='" & bodyStyles & " text-align:center();'>" & ctrRow & ".</td>" &
                "<td style='" & bodyStyles & " text-align:center'>" & store("Region") & "</td>" &
                "<td style='" & bodyStyles & " text-align:center'>" & store("TotalStores") & "</td>"

                totalStores += CDec2(store("TotalStores"))
                DateFromOld = DateFrom

                If store("DateUsage").Length > 0 Then

                    WeeklyTotal = 0

                    While (DateFromOld <= DateTo)
                        Dim isUsage As Boolean = False
                        Dim storeCount As Integer = 0
                        Dim dtold = CDate(DateFromOld).ToString("yyyy-MM-dd")
                        Dim closedStores As Integer = GetClosedStores(store("GroupID"), Format(DateFromOld, "MMM dd yyyy"))

                        For Each dit As Object In store("DateUsage")
                            If CDate(dit("Date")).ToString("yyyy-MM-dd") = dtold Then
                                storeCount = CInt2(dit("Count"))
                                WeeklyTotal += storeCount
                                isUsage = True
                                Exit For
                            End If
                        Next

                        If isUsage Then
                            tr &= "<td style='" + bodyStyles + " text-align:center;'>" & storeCount & " (" & Format(CDec2(storeCount) / (CDec2(store("TotalStores") - closedStores)) * 100, "###.##") & "%)" & "</td>"
                        Else
                            tr &= "<td style='" + bodyStyles + " text-align:center;'></td>"
                        End If

                        If footerperiod.ContainsKey(dtold) Then
                            Dim footerperiodcount As String() = Split(footerperiod(dtold), "|")
                            Dim splitstorecount As Integer = CDec2(footerperiodcount(0))
                            Dim splitclosedstorecount As Integer = CDec2(footerperiodcount(1))
                            footerperiod(dtold) = (CDec2(splitstorecount) + CDec2(storeCount)) & "|" & (CDec2(splitclosedstorecount) + closedStores)
                        Else
                            footerperiod.Add(dtold, CDec2(storeCount) & "|" & closedStores)
                        End If

                        DateFromOld = DateAdd("d", 1, DateFromOld)
                    End While

                    If WeeklyTotal > 0 Then
                        tr &= "<td style='" + bodyStyles + " text-align:center;'>" & CInt2(WeeklyTotal / 7) & " (" & Format(CInt2(WeeklyTotal / 7) / (CDec2(store("TotalStores"))) * 100, "###.##") & "%)" & "</td>"
                    Else
                        tr &= "<td style='" + bodyStyles + " text-align:center;'></td>"
                    End If

                    'fromDate = CDate(DateFrom)
                    'tr &= "<td style='" & bodyStyles & " text-align:center;'>" & store("DateUsage").Length & " of " & ctrColDate & " day(s)</td>"
                Else
                    tr &= colsDate
                    tr &= "<td style='" + bodyStyles + " text-align:center;'></td>"
                End If

                '"<td style='" & bodyStyles & " text-align:center'>" & CInt2(store("Usage")) & "</td>" &

                'tr &= "<td style='" & bodyStyles & " text-align:center'>Total</td>"
                tr &= "</tr>"

                tbodyDeviceUtilUsageDetails &= tr
            Next

            tbodyDeviceUtilUsageDetails &= "</tbody>"

            Dim tfooterDeviceUtilUsageDetails As String = "<tfoot>"
            Dim footertotalstorecount As String = 0

            tfooterDeviceUtilUsageDetails &= "<tr>"
            tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;' colspan='2'>TOTAL:</td>"
            tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & totalStores & "</td>"

            For i As Integer = 0 To footerperiod.Count - 1
                Dim footerperiodcount As String() = Split(footerperiod.Values(i), "|")
                Dim splitstorecount As Integer = CDec2(footerperiodcount(0))
                Dim splitclosedstorecount As Integer = CDec2(footerperiodcount(1))

                If CDec2(splitstorecount) = 0 Then
                    tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'></td>"
                Else
                    footertotalstorecount += splitstorecount
                    tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & splitstorecount & " (" & Format(CDec2(splitstorecount) / CDec2(totalStores - splitclosedstorecount) * 100, "###.##") & "%)" & "</td>"
                End If
            Next

            tfooterDeviceUtilUsageDetails &= "<td style='" + bodyStyles + " font-weight: bold; text-align:center;'>" & CInt2(footertotalstorecount / 7) & " (" & Format(CInt2(footertotalstorecount / 7) / (CDec2(totalStores)) * 100, "###.##") & "%)" & "</td>"
            tfooterDeviceUtilUsageDetails &= "</tr>"
            tfooterDeviceUtilUsageDetails &= "</tfoot>"

            Dim strFileName As String = Common.ServerPath & "temp\DeviceUtilization\" & pFilename 'Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy") & ".xlsx"

            msg &= "<table border='0' cellpadding='5' cellspacing='0' style='width:100%'>" & theadDeviceUtilUsageDetails & "" & tbodyDeviceUtilUsageDetails & tfooterDeviceUtilUsageDetails & "</table>"

#If DEBUG Then
            'SendMail("customercare@mobiz-apps.com",
            '       "arniepeloton17@gmail.com", ', francis.hernandez@mobiz-apps.com
            '       "1Mobiz Inc.",
            '           EmailTitle,
            '           msg,
            '           "smtp-mail.outlook.com",
            '            587,
            '            "customercare@mobiz-apps.com",
            '            "MobizCustCare2221", True, , pFilename, strFileName, True)

            '"Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy") & ".xlsx"

#Else
        SendMail("customercare@email.mobiz-apps.com",
                   "mai.salangsang@potatocorner.com, pcit.support@potatocorner.com, lee.bajamunde@potatocorner.com, arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com, freak_222@yahoo.com",
                   "1Mobiz Inc.",
                   EmailTitle,
                   msg,
                   "smtp.pepipost.com",
                   587,
                   "arvinboggs",
                   "p$P.p/Kus7v%3<A@~dN^", , , "Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy") & ".xlsx", strFileName, True)

          'SendMail("customercare@email.mobiz-apps.com",
          '         "arniepeloton17@gmail.com, freak_222@yahoo.com",
          '         "1Mobiz Inc.",
          '         EmailTitle,
          '         msg,
          '         "smtp.pepipost.com",
          '         587,
          '         "arvinboggs",
          '         "p$P.p/Kus7v%3<A@~dN^", , , "Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy") & ".xlsx", strFileName, True)

#End If
            reportCounter += 1
        End While

        If isCustomizeReport Then
            Return "Sent"
        End If

        Return msg
    End Function

    Function RunDeviceMonitoringCheck(OrganizationDatabaseName As String)
        Dim DateFrom As Date = DateAdd(DateInterval.Day, -1, Date.Now)
        Dim DateTo As Date = DateAdd(DateInterval.Day, -1, Date.Now)

        Try
            Dim fromDate As Date = CDate(DateFrom)
            Dim toDate As Date = CDate(DateTo)
            Dim ctrColDate As Integer = 0
            Dim colsDate As String = ""
            Dim headerStyles = "padding: 10px; text-align:center; white-space:nowrap; font-weight:bold;"
            Dim bodyStyles = "padding: 5px; white-space:nowrap; "
            Dim pDTSales As DataTable

            Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(OrganizationDatabaseName))


            Dim dayname As String = CStr2(DateTime.Parse(Date.Now()).DayOfWeek)
            Dim weeklyreport As String = ""

            'If dayname = "Saturday" AndAlso Hour(Now) > 12 Then
            If dayname = "Saturday" AndAlso Hour(Now) > 12 Then
                Dim templastDateWeekly = mProData.ExecuteScalar("Select DefaultValue  from systemdefaults where [description] = 'LastSendOfDeviceMonitoringInfoWeekly'")

                If Not IsNothing(templastDateWeekly) Then
                    DateFrom = templastDateWeekly
                Else
                    DateFrom = DateAdd(DateInterval.Day, -1, Today)
                End If

                'If DateDiff(DateInterval.Day, DateFrom.Date, Today) > 0 Then
                '    weeklyreport = CreateWeeklyReport()

                '    If IsNothing(templastDateWeekly) Then
                '        mProData.Execute("Insert into SystemDefaults(Description, DefaultValue, LastUpdated) values('LastSendOfDeviceMonitoringInfoWeekly',GETDATE(), GETDATE())")
                '    Else
                '        mProData.Execute("Update SystemDefaults set DefaultValue = GETDATE() where [description] = 'LastSendOfDeviceMonitoringInfoWeekly'")
                '    End If
                'End If
            End If


            Dim templastDate = mProData.ExecuteScalar("Select DefaultValue  from systemdefaults where [description] = 'LastSendOfDeviceMonitoringInfo'")

            DateFrom = templastDate

            If DateDiff(DateInterval.Day, DateFrom.Date, Today) > 0 Then

                Dim reportCounter As Integer = 0
                Dim IsCompanyOwned As Boolean

                While reportCounter <= 1

                    Dim theadDeviceUtilUsageDetails = "<thead><tr style='background-color:#CCC'>"
                    Dim tbodyDeviceUtilUsageDetails = "<tbody>"
                    Dim ctrRow As Integer = 0

                    If reportCounter = 0 Then
                        IsCompanyOwned = True
                    Else
                        IsCompanyOwned = False
                    End If

                    If Not IsNothing(templastDate) Then
                        DateFrom = DateAdd(DateInterval.Day, -1, templastDate)
                        DateTo = DateAdd(DateInterval.Day, -1, templastDate)
                    Else
                        DateFrom = DateAdd(DateInterval.Day, -1, Today)
                    End If


                    theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "' >#</th>"
                    theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Region</th>"
                    theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>No of Stores</th>"
                    theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>" & Format(DateFrom, "MMM dd yyyy") & " Utilization</th>"
                    theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Utilization(%)</th>"

                    fromDate = CDate(DateFrom)

                    theadDeviceUtilUsageDetails += "</tr></thead>"

                    Dim arrFowst = GetDeviceUtilizationDetails(DateFrom, DateTo, False, IsCompanyOwned)
                    Dim arrFowstSummary = GetDeviceUtilizationSummary(DateFrom, DateTo, False, , IsCompanyOwned)
                    Dim EmailTitle As String

                    Dim strFileName As String = Common.ServerPath & "temp\DeviceUtilization\" '& Format(DateFrom, "MMM dd yyyy") & ".xlsx"

                    If IsCompanyOwned Then
                        strFileName &= "CO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & ".xlsx"
                        EmailTitle = "CO-Device Utilization " & Format(DateFrom, "MMM dd yyyy")
                    Else
                        strFileName &= "FO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & ".xlsx"
                        EmailTitle = "FO-Device Utilization " & Format(DateFrom, "MMM dd yyyy")
                    End If

                    Dim totalstores As Integer = 0
                    Dim totalusage As Integer = 0
                    Dim totalclosedstores As Integer = 0

                    For Each store As Object In arrFowstSummary
                        ctrRow += 1
                        totalstores += CDec2(store("TotalStores"))
                        totalusage += CDec2(store("Usage"))

                        Dim closedStores As Integer = GetClosedStores(store("GroupID"), Format(DateFrom, "MMM dd yyyy"))

                        totalclosedstores += closedStores

                        Dim tr As String = "<tr style='background-color:  " & IIf(ctrRow Mod 2 = 0, "#f5f5f5", "#FFFFFF") & "'><td style='" & bodyStyles & " text-align:center();'>" & ctrRow & ".</td>" &
                        "<td style='" & bodyStyles & " text-align:center'>" & store("Region") & "</td>" &
                        "<td style='" & bodyStyles & " text-align:center'>" & store("TotalStores") & "</td>" &
                        "<td style='" & bodyStyles & " text-align:center'>" & CInt2(store("Usage")) & "</td>" &
                        "<td style='" & bodyStyles & " text-align:center'>" & Format((CDec2(store("Usage")) / (CDec2(store("TotalStores")) - CDec2(closedStores)) * 100), "##.##") & "%</td>"
                        tr &= "</tr>"

                        tbodyDeviceUtilUsageDetails &= tr
                    Next

                    tbodyDeviceUtilUsageDetails &= "</tbody>"

                    Dim tfooterDeviceUtilUsageDetails As String = "<tfoot>"

                    tfooterDeviceUtilUsageDetails &= "<tr>"
                    tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;' colspan='2'>TOTAL:</td>"
                    tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & totalstores & "</td>"
                    tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & totalusage & "</td>"
                    tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & Format((CDec2(totalusage) / CDec2(totalstores - totalclosedstores) * 100), "##.##") & "%</td>"

                    tfooterDeviceUtilUsageDetails &= "</tr>"
                    tfooterDeviceUtilUsageDetails &= "</tfoot>"

                    Dim msg As String = ""

                    msg &= "<table border='0' cellpadding='5' cellspacing='0' style='width:100%'>" & theadDeviceUtilUsageDetails & "" & tbodyDeviceUtilUsageDetails & tfooterDeviceUtilUsageDetails & "</table>"

                    'Return weeklyreport & msg
#If DEBUG Then
                    SendMail("customercare@mobiz-apps.com",
                               "arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com",
                               "1Mobiz Inc.",
                               EmailTitle,
                               msg,
                               "smtp-mail.outlook.com",
                                587,
                                "customercare@mobiz-apps.com",
                                "MobizCustCare2221", , , "Device Utilization " & Format(DateFrom, "MMM dd yyyy") & ".xlsx", strFileName, True)

#Else
                 'SendMail("customercare@email.mobiz-apps.com",
                 '          "arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com, freak_222@yahoo.com",
                 '          "1Mobiz Inc.",
                 '          EmailTitle,
                 '          msg,
                 '          "smtp-mail.outlook.com",
                 '           587,
                 '           "arvinboggs",
                 '           "p$P.p/Kus7v%3<A@~dN^", , , strFileName & ".xlsx", strFileName, True)

                SendMail("customercare@email.mobiz-apps.com",
                         "mai.salangsang@potatocorner.com, lee.bajamunde@potatocorner.com, cherry.demesa@potatocorner.com, ysai.mastrile@potatocorner.com, irene.amado@potatocorner.com, rolly.adina@potatocorner.com, ric.medalla@potatocorner.com, maryjune.tumpag@potatocorner.com, rina.dimaculangan@potatocorner.com, Chris.bernardo@potatocorner.com, francis.hernandez@mobiz-apps.com, arniepeloton17@gmail.com, pcit.support@potatocorner.com",
                         "1Mobiz Inc.",
                         "Device Utilization " & Format(DateFrom, "MMM dd yyyy"),
                         msg,
                         "smtp.pepipost.com",
                         587,
                         "arvinboggs",
                         "p$P.p/Kus7v%3<A@~dN^", , , strFileName & ".xlsx", strFileName, True)
#End If

                    AppendToLog("Update SystemDefaults")

                    reportCounter += 1
                End While

                If IsNothing(templastDate) Then
                    mProData.Execute("Insert into SystemDefaults(Description, DefaultValue, LastUpdated) values('LastSendOfDeviceMonitoringInfo',GETDATE(), GETDATE())")
                Else
                    mProData.Execute("Update SystemDefaults set DefaultValue = GETDATE() where [description] = 'LastSendOfDeviceMonitoringInfo'")
                End If

            End If
            Return 1
       Catch ex As Exception
            AppendToDebug(ex.Message)
            Return ex.Message
        End Try

    End Function

    Function RunDeviceMonitoringByManager(OrganizationDatabaseName As String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        
        Dim GroupID() As String = {"'50ddc475-2b88-498f-aef2-c5f6379e2ba2'", "'7c3897bd-4501-4211-93cb-0ed2a4749141'", "'0a31a6e7-f2d9-404c-b5cd-0f970c840b2b'", "'70312661-460a-42ad-a85b-0c46e2e727c4'", "'1f41e72c-35cc-4a82-acd9-27760751f499'", "'e9db8b68-b1ea-4f38-9c45-8bbdb2334e78'", "'5c8d6617-1db5-460f-8b61-d5ed693e0f07'", "'78aeb14d-9b95-47a6-82d3-2eb8f87b76bc'",
                                  "'61bab350-115f-4901-ab88-bcbbac7ada73'", "'3a6e4c4a-e89f-485c-aeb4-35c45143a428'", "'b5e9d777-84c6-49e8-b7f9-7b446c7c4915'", "'f9c92bf2-d8c0-4fa7-9812-b7e539c3356c'", "'d3750fec-31ca-40a8-8b9f-77b0dfed1631'", "'db5bd2cc-d4fa-4081-884f-a19d5ec34548'", "'669976DD-E8F5-440A-8D24-4E5AB83D1B7A'"}

        Dim GroupName() As String = {"Luzon-Dev", "Cebu", "CO - GMAN", "CO - GMAS", "CO - Mindanao", "CO - North Luzon", "CO - South Luzon", "CO - Visayas", "FO - GMAN", "FO - GMAS", "FO - Mindanao", "FO - North Luzon", "FO - South Luzon", "FO - Visayas", "Third Party POS"}
        Dim dtStores As New DataTable
        Dim dtManagers As New DataTable
        Dim DateFrom As Date = Date.Now

        Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(OrganizationDatabaseName))

        '#If DEBUG Then
        '        GroupID = "'50ddc475-2b88-498f-aef2-c5f6379e2ba2'"
        '#Else
        '        'GroupID = "'7c3897bd-4501-4211-93cb-0ed2a4749141','0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4','1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc','61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915','f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548'"
        '        ' Cebu, CO - GMAN, CO - GMAS, CO - Mindanao, CO - North Luzon, CO - South Luzon, CO - Visayas, FO - GMAN, FO - GMAS, FO - Mindanao, FO - North Luzon, FO - South Luzon, FO - Visayas

        '        if IsCompanyOwned then
        '            GroupID = "'0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4','1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc'"
        '        else

        '            GroupID = "'7c3897bd-4501-4211-93cb-0ed2a4749141','61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915','f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548'"
        '        end if
        '#End If

        Dim templastDate = mProData.ExecuteScalar("Select DefaultValue  from systemdefaults where [description] = 'LastDeviceMonitoringDataCreation'")

        If Not IsNothing(templastDate) Then
            DateFrom = templastDate
        Else
            DateFrom = DateAdd(DateInterval.Day, -1, Today)
        End If

        If DateDiff(DateInterval.Day, DateFrom.Date, Today) > 0 Then

            For i As Integer = 0 To GroupID.Length - 1

                'Get All 'Store and device are operational.' FOWST by Group

                pSql = "Select DISTINCT MobizLibraryOrganizationFOWST.ID FOWSTID, MobizLibraryOrganizationFOWST.Name FOWSTName " & vbCrLf &
                        " , MobizLibraryOrganizationFOWST.Code FOWSTCode " & vbCrLf &
                        " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        " LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID " & vbCrLf

                pSql &= " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID IN (" & GroupID(i) & ") " & vbCrLf

                pSql &= " AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.') " & vbCrLf


                dtStores = Common.WebRequest.ProData.Execute(pSql)

                For j As Integer = 0 To dtStores.Rows.Count - 1
                    pIU = New InsertUpdate("TransStoreDeviceUtilization")

                    pIU("FOWSTID") = ReadyString(CStr2(dtStores.Rows(j).Item("FOWSTID")))
                    pIU("FOWSTCode") = ReadyString(CStr2(dtStores.Rows(j).Item("FOWSTCode")))
                    pIU("FOWSTName") = ReadyString(CStr2(dtStores.Rows(j).Item("FOWSTName")))
                    pIU("GroupID") = GroupID(i)
                    pIU("GroupName") = ReadyString(GroupName(i))

                    If GroupName(i) = "Third Party POS" Then
                        pIU("OwnershipType") = ReadyString(IIf(CStr2(dtStores.Rows(j).Item("FOWSTCode")).StartsWith("PC") AndAlso Not CStr2(dtStores.Rows(j).Item("FOWSTCode")).StartsWith("PCF"), "Company-Owned", "Franchise"))
                    Else
                        pIU("OwnershipType") = ReadyString(IIf(GroupName(i).Contains("CO"), "Company-Owned", "Franchise"))
                    End If

                    pSql = "Select ISNULL(IsClosed, 0) from MobizLibraryOrganizationFOWSTOperationHRs " & vbCrLf &
                    "WHERE FOWSTID = " & ReadyString(CStr2(dtStores.Rows(j).Item("FOWSTID"))) & vbCrLf &
                    " And Weekday = " & Weekday(Today)

                    Dim dtIsClosed As Boolean = CBool2(Common.WebRequest.ProData.ExecuteScalar(pSql))

                    If dtIsClosed Then
                        pIU("Status") = "'Closed'"
                    End If

                    pSql = "Select DISTINCT MobizLibraryPeople.ID, MobizLibraryPeople.FirstName + ' ' + MobizLibraryPeople.Surname ManagerName, CompanyEmailAddress from LibraryFranchiseGroupHeads " & vbCrLf &
                            "INNER JOIN MobizLibraryPeople on MobizLibraryPeople.ID = LibraryFranchiseGroupHeads.PeopleID " & vbCrLf &
                            "INNER JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = MobizLibraryPeople.ID " & vbCrLf &
                            " INNER JOIN LibraryOrganizationEmployeeFOWST on LibraryOrganizationEmployeeFOWST.PeopleID= MobizLibraryPeople.ID " & vbCrLf &
                            "WHERE FranchiseGroupID = " & GroupID(i) & " AND CompanyEmailAddress IS NOT NULL " & vbCrLf &
                            " AND LibraryOrganizationEmployeeFOWST. FOWSTID = " & ReadyString(CStr2(dtStores.Rows(j).Item("FOWSTID"))) & vbCrLf &
                            " AND CompanyEmailAddress <> '-' " & vbCrLf &
                            "AND LibraryOrganizationEmployee.RoleTagID IN ('1ab7ba3c-6d71-429e-8826-0a929d0c7ea2','aa870dbf-a238-4246-8963-448a1771a9f6') "

                    dtManagers = Common.WebRequest.ProData.Execute(pSql)

                    'save 2 manager email

                    If dtManagers.Rows.Count > 0 Then

                        For k As Integer = 0 To 1
                            If k = 0 Then
                                pIU("ManagerID1") = ReadyString(CStr2(dtManagers.Rows(k).Item("ID")))
                                pIU("Manager1") = ReadyString(CStr2(dtManagers.Rows(k).Item("ManagerName")))
                                pIU("Manager1Email") = ReadyString(CStr2(dtManagers.Rows(k).Item("CompanyEmailAddress")))
                            Else
                                If dtManagers.Rows.Count > 1 Then
                                    pIU("ManagerID2") = ReadyString(CStr2(dtManagers.Rows(k).Item("ID")))
                                    pIU("Manager2") = ReadyString(CStr2(dtManagers.Rows(k).Item("ManagerName")))
                                    pIU("Manager2Email") = ReadyString(CStr2(dtManagers.Rows(k).Item("CompanyEmailAddress")))
                                End If
                            End If
                        Next

                    End If

                    Common.WebRequest.ProData.Execute(pIU.GetInsert())
                Next

            Next

            If IsNothing(templastDate) Then
                mProData.Execute("Insert into SystemDefaults(Description, DefaultValue, LastUpdated) values('LastDeviceMonitoringDataCreation',GETDATE(), GETDATE())")
            Else
                mProData.Execute("Update SystemDefaults set DefaultValue = GETDATE() where [description] = 'LastDeviceMonitoringDataCreation'")
            End If

        End If

        'Send Email

        Return 1

    End Function

    Function EmailAMAndDMforDeviceMonitoring(OrganizationDatabaseName As String)
        Dim DateFrom As Date = DateAdd(DateInterval.Day, -1, Date.Now)
        Dim ctrRow As Integer
        Dim pSql As String
        Dim arrFowstSummary As New DataTable
        Dim arrManagers As New DataTable
        Dim EmailTitle As String

        Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(OrganizationDatabaseName))

        Dim templastDate = mProData.ExecuteScalar("Select DefaultValue  from systemdefaults where [Description] = 'LastSendOfDeviceMonitoringInfoForManagers'")

        If Not IsNothing(templastDate) Then
            DateFrom = templastDate
        End If

        If DateDiff(DateInterval.Day, DateFrom.Date, Today) > 0 Then

            EmailTitle = "Device Utilization " & Format(DateFrom, "MMM dd yyyy")

            pSql = "Select FOWSTCode, FOWSTName, GroupName, OwnershipType, Manager1Email, Manager2Email, [Status], GrossSales from TransStoreDeviceUtilization " & vbCrLf &
                    "WHERE convert(date,[Date]) = convert(date," & ReadyString(CStr2(DateFrom.Date)) & ") " & vbCrLf &
                    "Order by OwnershipType, [Status] "

            arrFowstSummary = Common.WebRequest.ProData.Execute(pSql)

            pSql = "Select distinct Manager1Email Email from TransStoreDeviceUtilization " & vbCrLf &
                    "WHERE convert(date, [date]) = convert(date," & ReadyString(CStr2(DateFrom.Date)) & ") AND Manager1Email <> '' " & vbCrLf &
                    "UNION " & vbCrLf &
                    "Select distinct Manager2Email Email from TransStoreDeviceUtilization " & vbCrLf &
                    "WHERE convert(date, [date]) = convert(date," & ReadyString(CStr2(DateFrom.Date)) & ") AND Manager2Email <> '' "

            arrManagers = Common.WebRequest.ProData.Execute(pSql)

            For i As Integer = 0 To arrManagers.Rows.Count - 1

                Dim ManagersEmail As String = CStr2(arrManagers.Rows(i).Item("Email"))

                Dim headerStyles = "padding: 10px; text-align:center; white-space:nowrap; font-weight:bold;"
                Dim bodyStyles = "padding: 5px; white-space:nowrap; "

                Dim theadDeviceUtilUsageDetails = "<thead><tr style='background-color:#CCC'>"
                Dim tbodyDeviceUtilUsageDetails = "<tbody>"

                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "' >#</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Store Code</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Store Name</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Region</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Date</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Status</th>"

                theadDeviceUtilUsageDetails += "</tr></thead>"

                Dim dtTempManagerAssignedFOWST As DataTable

                arrFowstSummary.DefaultView.RowFilter = " Manager1Email = " & ReadyString(CStr2(arrManagers.Rows(i).Item("Email"))) & " OR Manager2Email = " & ReadyString(CStr2(arrManagers.Rows(i).Item("Email")))

                dtTempManagerAssignedFOWST = arrFowstSummary.DefaultView.ToTable

                ctrRow = 0

                For j As Integer = 0 To dtTempManagerAssignedFOWST.Rows.Count - 1
                    ctrRow += 1

                    Dim tr As String = "<tr style='background-color:  " & IIf(ctrRow Mod 2 = 0, "#f5f5f5", "#FFFFFF") & "'><td style='" & bodyStyles & " text-align:center();'>" & ctrRow & ".</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(dtTempManagerAssignedFOWST.Rows(j).Item("FOWSTCode")) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(dtTempManagerAssignedFOWST.Rows(j).Item("FOWSTName")) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(dtTempManagerAssignedFOWST.Rows(j).Item("GroupName")) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(DateFrom.Date) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(dtTempManagerAssignedFOWST.Rows(j).Item("Status")) & "</td>"

                    tr &= "</tr>"

                    tbodyDeviceUtilUsageDetails &= tr
                Next

                tbodyDeviceUtilUsageDetails &= "</tbody>"

                'Dim tfooterDeviceUtilUsageDetails As String = "<tfoot>"

                'tfooterDeviceUtilUsageDetails &= "<tr>"
                'tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;' colspan='2'>TOTAL:</td>"
                'tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & totalstores & "</td>"
                'tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & totalusage & "</td>"
                'tfooterDeviceUtilUsageDetails &= "<td style='" & bodyStyles & " font-weight: bold; text-align: center;'>" & Format((CDec2(totalusage) / CDec2(totalstores - totalclosedstores) * 100), "##.##") & "%</td>"

                'tfooterDeviceUtilUsageDetails &= "</tr>"
                'tfooterDeviceUtilUsageDetails &= "</tfoot>"

                Dim msg As String = ""

                '& tfooterDeviceUtilUsageDetails 

                msg &= "<table border='0' cellpadding='5' cellspacing='0' style='width:100%'>" & theadDeviceUtilUsageDetails & "" & tbodyDeviceUtilUsageDetails & "</table>"


#If DEBUG Then
                SendMail("customercare@email.mobiz-apps.com",
                                       "arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com",
                                       "1Mobiz Inc.",
                                       EmailTitle,
                                       msg,
                                        "smtp.pepipost.com",
                                        587,
                                        "arvinboggs",
                                        "p$P.p/Kus7v%3<A@~dN^", , , , , True)
#Else
         SendMail("customercare@email.mobiz-apps.com",
                               ManagersEmail & ", Joanna.isidoro@potatocorner.com, arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com, freak_222@yahoo.com",
                               "1Mobiz Inc.",
                               EmailTitle,
                               msg,
                               "smtp.pepipost.com",
                               587,
                               "arvinboggs",
                               "p$P.p/Kus7v%3<A@~dN^", , , , , True)
#End If

                Threading.Thread.Sleep(5000)
            Next

            'Email Sales

            AppendToLog("Create Email Sales")

            EmailTitle = "Store Sales " & Format(DateFrom, "MMM dd yyyy")

            For i As Integer = 0 To arrManagers.Rows.Count - 1

                Dim ManagersEmail As String = CStr2(arrManagers.Rows(i).Item("Email"))

                Dim headerStyles = "padding: 10px; text-align:center; white-space:nowrap; font-weight:bold;"
                Dim bodyStyles = "padding: 5px; white-space:nowrap; "

                Dim theadDeviceUtilUsageDetails = "<thead><tr style='background-color:#CCC'>"
                Dim tbodyDeviceUtilUsageDetails = "<tbody>"

                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "' >#</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Store Code</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Store Name</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Region</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Date</th>"
                theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Gross Sales</th>"

                theadDeviceUtilUsageDetails += "</tr></thead>"

                Dim dtTempManagerAssignedFOWST As DataTable

                arrFowstSummary.DefaultView.RowFilter = " Manager1Email = " & ReadyString(CStr2(arrManagers.Rows(i).Item("Email"))) & " OR Manager2Email = " & ReadyString(CStr2(arrManagers.Rows(i).Item("Email")))

                dtTempManagerAssignedFOWST = arrFowstSummary.DefaultView.ToTable

                ctrRow = 0

                For j As Integer = 0 To dtTempManagerAssignedFOWST.Rows.Count - 1
                    ctrRow += 1

                    Dim tr As String = "<tr style='background-color:  " & IIf(ctrRow Mod 2 = 0, "#f5f5f5", "#FFFFFF") & "'><td style='" & bodyStyles & " text-align:center();'>" & ctrRow & ".</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(dtTempManagerAssignedFOWST.Rows(j).Item("FOWSTCode")) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(dtTempManagerAssignedFOWST.Rows(j).Item("FOWSTName")) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(dtTempManagerAssignedFOWST.Rows(j).Item("GroupName")) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & CStr2(DateFrom.Date) & "</td>" &
                    "<td style='" & bodyStyles & " text-align:center'>" & Format(CDec2(dtTempManagerAssignedFOWST.Rows(j).Item("GrossSales")), "###,###.##") & "</td>"

                    tr &= "</tr>"

                    tbodyDeviceUtilUsageDetails &= tr
                Next

                tbodyDeviceUtilUsageDetails &= "</tbody>"

                Dim msg As String = ""

                msg &= "<table border='0' cellpadding='5' cellspacing='0' style='width:100%'>" & theadDeviceUtilUsageDetails & "" & tbodyDeviceUtilUsageDetails & "</table>"

                AppendToLog("Email Sales")
#If DEBUG Then
                SendMail("customercare@mobiz-apps.com",
                      "arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com",
                      "1Mobiz Inc.",
                       EmailTitle,
                      msg,
                      "smtp-mail.outlook.com",
                       587,
                       "customercare@mobiz-apps.com",
                       "MobizCustCare2221", , , , , True)

                'SendMail("customercare@email.mobiz-apps.com",
                '                       "arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com",
                '                       "1Mobiz Inc.",
                '                       EmailTitle,
                '                       msg,
                '                        "smtp.pepipost.com",
                '                        587,
                '                        "arvinboggs",
                '                        "p$P.p/Kus7v%3<A@~dN^", , , , , True)
#Else
            SendMail("customercare@email.mobiz-apps.com",
                                       "arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com, pcit.support@potatocorner.com",
                                       "1Mobiz Inc.",
                                       EmailTitle,
                                       msg,
                                        "smtp.pepipost.com",
                                        587,
                                        "arvinboggs",
                                        "p$P.p/Kus7v%3<A@~dN^", , , , , True)

#End If

                Threading.Thread.Sleep(5000)
            Next

            If IsNothing(templastDate) Then
                mProData.Execute("Insert into SystemDefaults(Description, DefaultValue, LastUpdated) values('LastSendOfDeviceMonitoringInfoForManagers',GETDATE(), GETDATE())")
            Else
                mProData.Execute("Update SystemDefaults set DefaultValue = GETDATE() where [description] = 'LastSendOfDeviceMonitoringInfoForManagers'")
            End If

        End If

        Return 1

    End Function

    Function GetClosedStores(ByVal groupID As String, ByVal datefrom As String)
        Return Common.WebRequest.ProData.Execute(" SELECT DISTINCT fowst.ID AS FowstID " & vbCrLf &
                        " , fowst.Name AS FowstName " & vbCrLf &
                        " , MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                        " , opera.WeekDay " & vbCrLf &
                        " FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
                        " INNER join MobizLibraryOrganizationFOWST fowst on fowst.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganization org ON org.ID = fowst.OrganizationID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWSTOperationHRs as opera ON opera.FowstID = fowst.ID AND opera.IsClosed = 1 AND opera.WeekDay = (SELECT DATEPART(dw,CONVERT(date, " & ReadyString(datefrom) & ")))" & vbCrLf &
                        " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(groupID) & vbCrLf &
                        " ORDER BY fowst.Name ASC ").Rows.Count
    End Function

    Function GetDeviceUtilizationDetails(DateFrom As Date, DateTo As Date, Optional IsWeeklyReport As Boolean = False, Optional IsCompanyOwned As Boolean = False)
        Dim pSql As String
        Dim pDT As DataTable
        Dim clsSecurityRole As New SecurityRole

#If DEBUG Then
        Dim GroupID As String = "'26a4dcc4-cab8-4467-b7b3-d2ce30e664ef'"
#Else
        Dim GroupID As String = "'7c3897bd-4501-4211-93cb-0ed2a4749141','0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4','1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc','61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915','f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548'"
        ' Cebu, CO - GMAN, CO - GMAS, CO - Mindanao, CO - North Luzon, CO - South Luzon, CO - Visayas, FO - GMAN, FO - GMAS, FO - Mindanao, FO - North Luzon, FO - South Luzon, FO - Visayas


        if IsCompanyOwned then
            GroupID = "'0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4','1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc'"
        else

            GroupID = "'7c3897bd-4501-4211-93cb-0ed2a4749141','61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915','f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548'"
        end if
#End If

        pSql = "Select DISTINCT  MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Code as StoreCode, MobizLibraryOrganizationFOWST.Name as Store, ISNULL(LibraryFranchiseGroup.GroupName,'') Region " & vbCrLf &
                " , CASE WHEN ISNULL(count(distinct convert(date,invoicedate)),0)=1 THEN 'Yes' ELSE '' END Usage " & vbCrLf

        'If IsWeeklyReport Then
        '    pSql &= ", convert(date,InvoiceDate) TransactionDate "
        'End If

        pSql &= " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                " LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID " & vbCrLf &
                " LEFT JOIN CacheInvoices on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID and convert(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf
        pSql &= " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID IN (" & GroupID & ") " & vbCrLf
        pSql &= " AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.') " & vbCrLf

        pSql &= " GROUP BY MobizLibraryOrganizationFOWST.ID,  MobizLibraryOrganizationFOWST.Code, MobizLibraryOrganizationFOWST.Name, ISNULL(LibraryFranchiseGroup.GroupName,'') "

        If IsWeeklyReport Then
            pSql &= ", convert(date,InvoiceDate) "
        End If

        pSql &= " ORDER BY ISNULL(LibraryFranchiseGroup.GroupName,''), Usage, MobizLibraryOrganizationFOWST.Name "

        If IsWeeklyReport Then
            'Dim pFilter1 As DataTable

            'pFilter1 = CreateCrossTab(arrFOWST, {"Store", "Region"}, {New CrossColumn("{TransactionDate}", "Usage", "Usage")})
            'pFilter1.Columns.Remove("TransactionDate")
            'pFilter1.Columns.Remove("Usage")

            'strFileName = "Device Utilization " & Format(DateFrom, "MMM dd yyyy") & "-" & Format(DateTo, "MMM dd yyyy") & ".xlsx"

            Dim arrFOWST As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            For i As Integer = 0 To arrFOWST.Length - 1
                pSql = " SELECT DISTINCT CONVERT(varchar, convert(date,InvoiceDate)) as Date " & vbCrLf &
                     " FROM CacheInvoices " & vbCrLf &
                     " WHERE FowstID = " & ReadyString(arrFOWST(i)("ID").ToString()) & vbCrLf &
                     " AND CONVERT(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                     " ORDER BY CONVERT(varchar, convert(date,InvoiceDate)) " & vbCrLf

                arrFOWST(i).Add("DateUsage", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
            Next

            Return arrFOWST

        Else
            Dim arrFOWST As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Dim pSourceURL As String = Common.ServerPath & "temp\DeviceUtilization"
            Dim strFileName As String = ""

            arrFOWST.Columns.Remove("ID")

            If IsCompanyOwned Then
                strFileName = "CO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & ".xlsx"
            Else
                strFileName = "FO-Device Utilization " & Format(DateFrom, "MMM dd yyyy") & ".xlsx"
            End If

            If Not System.IO.Directory.Exists(pSourceURL) Then
                IO.Directory.CreateDirectory(pSourceURL)
            End If

            If (IO.File.Exists(pSourceURL & "\" & strFileName)) Then
                IO.File.Delete(pSourceURL & "\" & strFileName)
            End If

            If arrFOWST.Rows.Count > 0 Then
                TableToExcelFile(arrFOWST, "", "", pSourceURL & "\" & strFileName)
            End If

            Return arrFOWST

        End If

    End Function

    Function GetDeviceUtilizationSummary(DateFrom As Date, DateTo As Date, Optional IsWeeklyReport As Boolean = False, Optional GroupID As String = "", Optional IsCompanyOwned As Boolean = True)
        Dim pSql As String
        Dim pDT As DataTable
        Dim clsSecurityRole As New SecurityRole

#If DEBUG Then
        If String.IsNullOrWhiteSpace(GroupID) Then
            'WOW Groups
            GroupID = "'26a4dcc4-cab8-4467-b7b3-d2ce30e664ef'" '"'7c3897bd-4501-4211-93cb-0ed2a4749141','0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4','1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc','61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915','f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548'"
            ' Cebu, CO - GMAN, CO - GMAS, CO - Mindanao, CO - North Luzon, CO - South Luzon, CO - Visayas, FO - GMAN, FO - GMAS, FO - Mindanao, FO - North Luzon, FO - South Luzon, FO - Visayas
        End If
#Else
        'GroupID = "'7c3897bd-4501-4211-93cb-0ed2a4749141','0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4','1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc','61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915','f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548'"
        ' Cebu, CO - GMAN, CO - GMAS, CO - Mindanao, CO - North Luzon, CO - South Luzon, CO - Visayas, FO - GMAN, FO - GMAS, FO - Mindanao, FO - North Luzon, FO - South Luzon, FO - Visayas

        if IsCompanyOwned then
            GroupID = "'0a31a6e7-f2d9-404c-b5cd-0f970c840b2b','70312661-460a-42ad-a85b-0c46e2e727c4','1f41e72c-35cc-4a82-acd9-27760751f499','e9db8b68-b1ea-4f38-9c45-8bbdb2334e78','5c8d6617-1db5-460f-8b61-d5ed693e0f07','78aeb14d-9b95-47a6-82d3-2eb8f87b76bc'"
        else

            GroupID = "'7c3897bd-4501-4211-93cb-0ed2a4749141','61bab350-115f-4901-ab88-bcbbac7ada73','3a6e4c4a-e89f-485c-aeb4-35c45143a428','b5e9d777-84c6-49e8-b7f9-7b446c7c4915','f9c92bf2-d8c0-4fa7-9812-b7e539c3356c','d3750fec-31ca-40a8-8b9f-77b0dfed1631','db5bd2cc-d4fa-4081-884f-a19d5ec34548'"
        end if
#End If
        pSql = " UPDATE MobizLibraryOrganizationFOWST SET [Status] = 'Store and device are operational.' WHERE ID IN (SELECT distinct fowstid from CacheInvoices " & vbCrLf &
                     " WHERE CONVERT(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf & ")"

        Common.WebRequest.ProData.Execute(pSql)

        pSql = "Select DISTINCT ISNULL(CONVERT(varchar(max), LibraryFranchiseGroup.ID),'') as GroupID, ISNULL(LibraryFranchiseGroup.GroupName,'') Region " & vbCrLf &
                " , COUNT(distinct FOWSTID) Usage " & vbCrLf &
                "  , COUNT(distinct MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID) TotalStores " & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                " LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID " & vbCrLf &
                " LEFT JOIN CacheInvoices on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID and convert(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf
        pSql &= " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID IN (" & GroupID & ") " & vbCrLf

        pSql &= " AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.') " & vbCrLf

        pSql &= " GROUP BY ISNULL(CONVERT(varchar(max), LibraryFranchiseGroup.ID),''), ISNULL(LibraryFranchiseGroup.GroupName,'') "
        pSql &= " ORDER BY ISNULL(LibraryFranchiseGroup.GroupName,'') "


        If IsWeeklyReport Then

            Dim arrFOWST As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            For i As Integer = 0 To arrFOWST.Length - 1
                pSql = " SELECT DISTINCT CONVERT(varchar, convert(date,InvoiceDate)) as Date, COUNT(distinct fowstid) Count " & vbCrLf &
                     " FROM CacheInvoices " & vbCrLf &
                     " WHERE GroupName = " & ReadyString(arrFOWST(i)("Region").ToString()) & vbCrLf &
                     " AND CONVERT(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                     " GROUP BY convert(date,InvoiceDate) " & vbCrLf &
                     " ORDER BY CONVERT(varchar, convert(date,InvoiceDate)) " & vbCrLf

                arrFOWST(i).Add("DateUsage", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
            Next

            Return arrFOWST

        Else

            Dim arrFOWST As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            Return arrFOWST

            'Dim arrFOWST As DataTable = Common.WebRequest.ProData.Execute(pSql)

            'Dim pSourceURL As String = Common.ServerPath & "temp\DeviceUtilization"
            'Dim strFileName As String = "Device Utilization " & Format(DateFrom, "MMM dd yyyy") & ".xlsx"

            'If Not System.IO.Directory.Exists(pSourceURL) Then
            '    IO.Directory.CreateDirectory(pSourceURL)
            'End If

            'If (IO.File.Exists(pSourceURL & "\" & strFileName)) Then
            '    IO.File.Delete(pSourceURL & "\" & strFileName)
            'End If

            'If arrFOWST.Rows.Count > 0 Then
            '    TableToExcelFile(arrFOWST, "", "", pSourceURL & "\" & strFileName)
            'End If

            'Return arrFOWST

        End If

    End Function


    Function GenerateExcelDeviceUtilizationDetails(arrFowstTemp As Object, dateFrom As Date, DateTo As Date, pFilename As String)
        Try
            Dim fromDate As Date = CDate(dateFrom)
            Dim toDate As Date = CDate(DateTo)
            Dim ctrColDate As Integer = 0
            Dim pWB As Aspose.Cells.Workbook
            Dim pWS As Aspose.Cells.Worksheet

            pWB = New Aspose.Cells.Workbook
            pWS = pWB.Worksheets(0)

            pWS.Cells(0, 0).PutValue("Store Code")
            pWS.Cells(0, 0).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
            pWS.Cells(0, 0).Style.HorizontalAlignment = TextAlignmentType.Center
            pWS.Cells(0, 0).Style.Pattern = BackgroundType.Solid
            pWS.Cells(0, 0).Style.Font.Size = 11
            pWS.Cells(0, 0).Style.Font.IsBold = True

            pWS.Cells(0, 1).PutValue("Store Name")
            pWS.Cells(0, 1).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
            pWS.Cells(0, 1).Style.HorizontalAlignment = TextAlignmentType.Center
            pWS.Cells(0, 1).Style.Pattern = BackgroundType.Solid
            pWS.Cells(0, 1).Style.Font.Size = 11
            pWS.Cells(0, 1).Style.Font.IsBold = True

            pWS.Cells(0, 2).PutValue("Region")
            pWS.Cells(0, 2).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
            pWS.Cells(0, 2).Style.HorizontalAlignment = TextAlignmentType.Center
            pWS.Cells(0, 2).Style.Pattern = BackgroundType.Solid
            pWS.Cells(0, 2).Style.Font.Size = 11
            pWS.Cells(0, 2).Style.Font.IsBold = True

            Dim storeCtr As Integer = 1 'start row 2
            Dim dateCtr As Integer = 3 ' row 1 column D

            While (fromDate <= toDate)
                pWS.Cells(0, dateCtr).PutValue(fromDate.ToString("MMM dd, yyyy"))
                pWS.Cells(0, dateCtr).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
                pWS.Cells(0, dateCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                pWS.Cells(0, dateCtr).Style.Pattern = BackgroundType.Solid
                pWS.Cells(0, dateCtr).Style.Font.Size = 11
                pWS.Cells(0, dateCtr).Style.Font.IsBold = True
                dateCtr += 1
                ctrColDate += 1
                fromDate = DateAdd("d", 1, fromDate)
            End While

            pWS.Cells(0, dateCtr).PutValue("Usage")
            pWS.Cells(0, dateCtr).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
            pWS.Cells(0, dateCtr).Style.HorizontalAlignment = TextAlignmentType.Center
            pWS.Cells(0, dateCtr).Style.Pattern = BackgroundType.Solid
            pWS.Cells(0, dateCtr).Style.Font.Size = 11
            pWS.Cells(0, dateCtr).Style.Font.IsBold = True

            Dim arrFowst = arrFowstTemp

            fromDate = CDate(dateFrom)

            For Each store As Object In arrFowst
                pWS.Cells(storeCtr, 0).PutValue(store("StoreCode"))
                pWS.Cells(storeCtr, 1).PutValue(store("Store"))
                pWS.Cells(storeCtr, 2).PutValue(store("Region"))

                If store("DateUsage").Length > 0 Then
                    dateCtr = 3
                    While (fromDate <= toDate)
                        Dim isUsage As Boolean = False

                        For Each dit As Object In store("DateUsage")
                            If CDate(dit("Date")).ToString("yyyy-MM-dd") = CDate(fromDate).ToString("yyyy-MM-dd") Then
                                isUsage = True
                                Exit For
                            End If
                        Next

                        If isUsage Then
                            pWS.Cells(storeCtr, dateCtr).PutValue("Yes")
                            pWS.Cells(storeCtr, dateCtr).Style.Font.Color = Color.FromArgb(26, 112, 45)
                        Else
                            Dim pSql As String = "SELECT ID FROM MobizLibraryOrganizationFOWSTOperationHRs WHERE FOWSTID = " & ReadyString(store("ID").ToString()) & vbCrLf &
                                                " AND IsClosed = 1 AND WeekDay = (SELECT DATEPART(dw,CONVERT(date, " & ReadyString(CDate(fromDate).ToString("yyyy-MM-dd")) & ")))" & vbCrLf

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                                pWS.Cells(storeCtr, dateCtr).PutValue("Closed")
                                pWS.Cells(storeCtr, dateCtr).Style.Font.Color = Color.FromArgb(224, 47, 65)
                            End If
                        End If

                        pWS.Cells(storeCtr, dateCtr).Style.HorizontalAlignment = TextAlignmentType.Center

                        dateCtr += 1
                        fromDate = DateAdd("d", 1, fromDate)
                    End While

                    fromDate = CDate(dateFrom)
                End If

                pWS.Cells(storeCtr, ctrColDate + 3).PutValue(store("DateUsage").Length & " of " & ctrColDate & " day(s)")
                pWS.Cells(storeCtr, ctrColDate + 3).Style.HorizontalAlignment = TextAlignmentType.Center

                storeCtr += 1
            Next

            pWS.Cells.StandardHeight = 30
            pWS.FreezePanes(1, 1, 1, 0)
            pWS.AutoFitColumns()

            Dim filesource = Common.ServerPath & "temp\\DeviceUtilization\" & pFilename

            pWB.Save(filesource)

            Return 1
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return 0
        End Try
    End Function

    'Old Device Monitoring. Comment date 1/2/2024

    'Function RunDeviceMonitoringCheck(OrganizationDatabaseName As String)

    '    Try
    '        Dim pSql As String
    '        Dim pDTGroup As DataTable
    '        Dim pDTFOWST As DataTable
    '        Dim OrgID As String
    '        Dim lastDate As Date

    '        AppendToLog("USE " & EscapeSqlName(OrganizationDatabaseName))
    '        mProData.Execute("USE " & EscapeSqlName(OrganizationDatabaseName))

    '        AppendToLog("Select OrganizationID from MobizLibrarySubscription where [Database] = " & ReadyString(OrganizationDatabaseName))
    '        OrgID = CStr2(mProData.ExecuteScalar("Select OrganizationID from MobizLibrarySubscription where [Database] = " & ReadyString(OrganizationDatabaseName)))
    '        'get last date since this function is triggered

    '        Dim templastDate = mProData.ExecuteScalar("Select DefaultValue  from systemdefaults where [description] = 'LastSendOfDeviceMonitoringInfo'")

    '        If Not IsNothing(templastDate) Then
    '            lastDate = templastDate
    '        Else
    '            lastDate = DateAdd(DateInterval.Day, -1, Today)
    '        End If

    '        AppendToLog("End of LastSendOfDeviceMonitoringInfo")

    '        AppendToLog(lastDate)
    '        AppendToLog(Today)
    '        AppendToLog(DateDiff(DateInterval.Day, lastDate, Today))

    '        If DateDiff(DateInterval.Day, lastDate, Today) > 0 Then

    '            pSql = "Select * from LibraryFranchiseGroup"

    '            pDTGroup = mProData.Execute(pSql)

    '            For i As Integer = 0 To pDTGroup.Rows.Count - 1

    '                If Trim(CStr2(pDTGroup.Rows(i).Item("GroupName"))) = "Training Group" Then
    '                    AppendToLog("Skip Training Group")
    '                    Continue For
    '                Else

    '                    pSql = "SELECT DISTINCT TOP 100 PERCENT MobizLibraryOrganizationFOWST.Name AS [FOWST] " & vbCrLf &
    '                            ", LibraryDevice.PhoneName AS [PhoneName] " & vbCrLf &
    '                            ", MobizLibraryDeviceFOWSTMonitoring.UUID AS [UUID] " & vbCrLf &
    '                            ", ISNULL(LastDataFlushReceivedInServer,'') AS [LastDataFlushReceivedInServer] " & vbCrLf &
    '                            ", ISNULL(LastDateOfTransactionFlushedFromDevice,'') AS [LastDateOfTransactionFlushedFromDevice] " & vbCrLf &
    '                            ", ISNULL(LastDataSync,'') AS [LastDataSync] " & vbCrLf &
    '                            ", ISNULL(InstalledDatabaseCreationDate,'') AS [InstalledDatabaseCreationDate] " & vbCrLf &
    '                            ", APKVersion AS [APKVersion] " & vbCrLf &
    '                            ", APKBuildDate AS [APKBuildDate] " & vbCrLf &
    '                            ", LibraryFranchiseGroup.GroupName AS [GroupName] " & vbCrLf &
    '                            ", MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName AS [GroupHead] " & vbCrLf &
    '                            " FROM MobizLibraryDeviceFOWSTMonitoring " & vbCrLf &
    '                            "inner join MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = MobizLibraryDeviceFOWSTMonitoring.FOWSTID  " & vbCrLf &
    '                            "inner join LibraryDevice on LibraryDevice.ID = MobizLibraryDeviceFOWSTMonitoring.DeviceID  " & vbCrLf &
    '                            "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID  " & vbCrLf &
    '                            "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID  " & vbCrLf &
    '                            "LEFT JOIN MobizLibraryPeople on MobizLibraryPeople.ID = LibraryFranchiseGroup.FranchisorGroupHead  " & vbCrLf &
    '                            "WHERE LibraryDevice.IsActive = 1 AND MobizLibraryDeviceFOWSTMonitoring.OrgID =  " & ReadyString(OrgID) & vbCrLf &
    '                            " AND (DateDiff(DAY, LastDataFlushReceivedInServer,GETDATE()) > 3 OR DateDiff(DAY, LastDateOfTransactionFlushedFromDevice,GETDATE()) > 3)" & vbCrLf &
    '                            " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN(" & ReadyString(CStr2(pDTGroup.Rows(i).Item("ID"))) & ") " & vbCrLf &
    '                            "   Order By  LastDataFlushReceivedInServer, LastDateOfTransactionFlushedFromDevice, APKVersion "

    '                    AppendToLog(pSql)

    '                    pDTFOWST = mProData.Execute(pSql)

    '                    If pDTFOWST.Rows.Count > 0 Then

    '                        Dim pdtGroupHeads As DataTable
    '                        Dim GroupEmail As String

    '                        AppendToLog("Getting Franchise Group Email")

    '                        pSql = "Select distinct CompanyEmailAddress from LibraryFranchiseGroupHeads WHERE FranchiseGroupID = " & ReadyString(CStr2(pDTGroup.Rows(i).Item("ID"))) & " ORDER BY CompanyEmailAddress DESC "
    '                        pdtGroupHeads = mProData.Execute(pSql)

    '                        If pdtGroupHeads.Rows.Count > 0 Then

    '                            For j As Integer = 0 To pdtGroupHeads.Rows.Count - 1

    '                                AppendToLog(CStr2(pdtGroupHeads.Rows(j).Item("CompanyEmailAddress")))

    '                                If IsEmail(CStr2(pdtGroupHeads.Rows(j).Item("CompanyEmailAddress"))) Then

    '                                    If j = 0 Then
    '                                        GroupEmail = CStr2(pdtGroupHeads.Rows(j).Item("CompanyEmailAddress"))
    '                                    Else
    '                                        GroupEmail &= ", " & CStr2(pdtGroupHeads.Rows(j).Item("CompanyEmailAddress"))
    '                                    End If

    '                                End If

    '                            Next

    '                            AppendToLog(GroupEmail)

    '                            If Not GroupEmail = vbNullString Then
    '                                Dim EmailMessage As String

    '                                EmailMessage = "<div>Good day, " & vbCrLf &
    '                                               "<br><br> " & vbCrLf &
    '                                               "We've found devices under your group that needs attention." & vbCrLf &
    '                                               "<br><br> " & vbCrLf &
    '                                               "Group/Region Name: <span style=""margin-left:20px;"">" & CStr2(pDTGroup.Rows(i).Item("GroupName")) & "</span><br> " & vbCrLf &
    '                                               "Date: <span style=""margin-left:73px;""> " & FormatDateTime(DateAdd(DateInterval.Day, -1, Today), vbLongDate) & "</span> " & vbCrLf &
    '                                               "</div> " & vbCrLf &
    '                                               "<table style=""width:100%;"" border=""0"" cellpadding=""3"" cellspacing=""0""> " & vbCrLf &
    '                                                   "<thead> " & vbCrLf &
    '                                                       "<tr style=""background-color:#3498db; color:#FFF;""> " & vbCrLf &
    '                                                           "<th style=""padding:10px;"">  </th> " & vbCrLf &
    '                                                           "<th style=""padding:10px;"">Store</th> " & vbCrLf &
    '                                                           "<th style=""padding:10px;"">Device Name </th> " & vbCrLf &
    '                                                           "<th style=""padding:10px;"">Device Serial Number </th> " & vbCrLf &
    '                                                           "<th style=""padding:10px;"">Last Data Flush Received In Server</th> " & vbCrLf &
    '                                                           "<th style=""padding:10px;"">Last Date Of Transaction Flushed From Device</th> " & vbCrLf &
    '                                                           "<th style=""padding:10px;"">Last Data Sync</th> " & vbCrLf &
    '                                                       "</tr> " & vbCrLf &
    '                                                   "</thead> " & vbCrLf &
    '                                               "<tbody> "

    '                                AppendToLog(EmailMessage)

    '                                For k As Integer = 0 To pDTFOWST.Rows.Count - 1

    '                                    If CBool2(k Mod 2) Then

    '                                        EmailMessage &= "<tr style=""background-color:#FFF;"">" & vbCrLf &
    '                                            "<td>" & k + 1 & ". </td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("FOWST")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("PhoneName")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("UUID")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("LastDataFlushReceivedInServer")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("LastDateOfTransactionFlushedFromDevice")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("LastDataSync")) & "</td>" & vbCrLf &
    '                                        "</tr>"

    '                                    Else
    '                                        EmailMessage &= "<tr style=""background-color:#EEE;"">" & vbCrLf &
    '                                            "<td>" & k + 1 & ". </td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("FOWST")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("PhoneName")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("UUID")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("LastDataFlushReceivedInServer")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("LastDateOfTransactionFlushedFromDevice")) & "</td>" & vbCrLf &
    '                                            "<td>" & CStr2(pDTFOWST.Rows(k).Item("LastDataSync")) & "</td>" & vbCrLf &
    '                                        "</tr>"

    '                                    End If

    '                                Next

    '                                AppendToLog(EmailMessage)

    '                                Try
    '                                    SendMail("customercare@mobiz-apps.com",
    '                                                GroupEmail & ", francis.hernandez@mobiz-apps.com",
    '                                                "Mobiz",
    '                                                "Device with issues connecting to server as of " & Format(DateAdd(DateInterval.Day, -1, Today), "MM/dd/yy"),
    '                                                EmailMessage,
    '                                                "smtp-mail.outlook.com",
    '                                                587,
    '                                                "customercare@mobiz-apps.com",
    '                                                "MobizCustCare2221", , , , , True)

    '                                    Threading.Thread.Sleep(30000)

    '                                    'SendMail("customercare@email.mobiz-apps.com",
    '                                    '    GroupEmail & ", francis.hernandez@mobiz-apps.com",
    '                                    '    "Mobiz",
    '                                    '    "Device with issues connecting to server as of " & Format(DateAdd(DateInterval.Day, -1, Today), "MM/dd/yy"),
    '                                    '    EmailMessage,
    '                                    '    "smtp.pepipost.com",
    '                                    '    587,
    '                                    '    "arvinboggs",
    '                                    '    "p$P.p/Kus7v%3<A@~dN^", , , , , True)

    '                                Catch ex As Exception
    '                                    AppendToLog(ex.Message)
    '                                End Try
    '                            End If

    '                        End If

    '                    End If
    '                End If
    '            Next

    '            If IsNothing(templastDate) Then
    '                mProData.Execute("Insert into SystemDefaults(Description, DefaultValue, LastUpdated) values('LastSendOfDeviceMonitoringInfo',GETDATE(), GETDATE())")
    '            Else
    '                mProData.Execute("Update SystemDefaults set DefaultValue = GETDATE() where [description] = 'LastSendOfDeviceMonitoringInfo'")
    '            End If
    '        End If

    '        'Insert Device Availability
    '        Dim pDT As DataTable
    '        Dim pIU As InsertUpdate
    '        Dim DeviceID As String
    '        Dim DeviceName As String

    '        pSql = "select * from LibraryDevice WHERE LibraryDevice.Isdummydevice = 0 AND LibraryDevice.IsActive = 1 "


    '        pDT = mProData.Execute(pSql)

    '        For i As Integer = 0 To pDT.Rows.Count - 1

    '            DeviceID = CStr2(pDT.Rows(i).Item("ID"))
    '            DeviceName = CStr2(pDT.Rows(i).Item("PhoneName"))

    '            pSql = "Select ID from TransDeviceAvailability WHERE DeviceID = " & ReadyString(DeviceID) & " AND CONVERT(date,TransDeviceAvailability.TransactionDate) = convert(date, GETDATE())"
    '            Dim rowid As String = CStr2(mProData.ExecuteScalar(pSql))

    '            If rowid.Length = 0 Then

    '                'check if restday
    '                Dim reason As String = ""
    '                Dim dayname As String = CStr2(DateTime.Parse(Date.Now()).DayOfWeek)
    '                Dim daynum As Integer

    '                If dayname = "Sunday" Then
    '                    daynum = 1
    '                ElseIf dayname = "Monday" Then
    '                    daynum = 2
    '                ElseIf dayname = "Tuesday" Then
    '                    daynum = 3
    '                ElseIf dayname = "Wednesday" Then
    '                    daynum = 4
    '                ElseIf dayname = "Thursday" Then
    '                    daynum = 5
    '                ElseIf dayname = "Friday" Then
    '                    daynum = 6
    '                ElseIf dayname = "Saturday" Then
    '                    daynum = 7
    '                End If

    '                pSql = "SELECT FowstID FROM LibraryDeviceFowst WHERE LibraryDeviceFowst.IsActive=1 AND DeviceID = " & ReadyString(DeviceID)
    '                Dim devFowst As DataTable = mProData.Execute(pSql)

    '                If devFowst.Rows.Count > 0 Then

    '                    For Each fowst As Object In devFowst.Rows
    '                        pSql = "SELECT ISNULL(IsClosed, 0) as IsClosed, MobizLibraryOrganizationFOWST.Name as FowstName " & vbCrLf &
    '                                " FROM MobizLibraryOrganizationFOWSTOperationHRs " & vbCrLf &
    '                                " INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFOWSTOperationHRs.FOWSTID " & vbCrLf &
    '                                " WHERE FOWSTID = " & ReadyString(fowst("FowstID").ToString()) & vbCrLf &
    '                                " AND WeekDay = " & daynum
    '                        Dim fowstophr As DataRow = mProData.ExecuteRow(pSql)
    '                        If Not fowstophr Is Nothing Then
    '                            If CBool2(fowstophr("IsClosed")) Then
    '                                reason &= fowstophr("FowstName") & " is closed today."
    '                            End If
    '                        End If
    '                    Next

    '                    pIU = New InsertUpdate("TransDeviceAvailability")

    '                    pIU("DeviceID") = ReadyString(DeviceID)
    '                    pIU("DeviceName") = ReadyString(DeviceName)
    '                    pIU("TransactionDate") = "GETDATE()"
    '                    pIU("IsAvailable") = 0
    '                    pIU("ReasonForInability") = IIf(String.IsNullOrWhiteSpace(reason), "NULL", ReadyString(reason))

    '                    pSql = pIU.GetInsert()

    '                    mProData.Execute(pSql)

    '                End If
    '            Else
    '                pIU = New InsertUpdate("TransDeviceAvailability")
    '                pIU("DeviceName") = ReadyString(DeviceName)
    '                pSql = pIU.GetUpdate(" ID = " & ReadyString(rowid))
    '                mProData.Execute(pSql)
    '            End If
    '        Next
    '        'End With
    '    Catch ex As Exception
    '        AppendToLog(ex.Message)
    '    End Try

    'End Function

    Function OneTimeQuery(OrganizationDatabaseName As String) As Boolean
        Dim Region() As String
        Dim Month As Integer
        Dim RegionName() = {"'CO - GMAN'", "'CO - GMAS'", "'CO - Mindanao'", "'CO - North Luzon'", "'CO - South Luzon'", "'CO - Visayas'"}

        If Not CStr2(Common.WebRequest.QueryString("Region")) = "" Then
            ReDim Region(0)

            Region(0) = "'CO - South Luzon'"

        Else
            ReDim Region(5)

            Region(0) = "'0a31a6e7-f2d9-404c-b5cd-0f970c840b2b'"
            Region(1) = "'70312661-460a-42ad-a85b-0c46e2e727c4'"
            Region(2) = "'1f41e72c-35cc-4a82-acd9-27760751f499'"
            Region(3) = "'e9db8b68-b1ea-4f38-9c45-8bbdb2334e78'"
            Region(4) = "'5c8d6617-1db5-460f-8b61-d5ed693e0f07'"
            Region(5) = "'78aeb14d-9b95-47a6-82d3-2eb8f87b76bc'"

        End If

        If Not CInt2(Common.WebRequest.QueryString("mMonth")) = 0 Then
            Month = CInt2(Common.WebRequest.QueryString("mMonth"))
        Else
            Month = 1
        End If

        Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(OrganizationDatabaseName))

        For mMonth = Month To 12
            For mRegion As Integer = 0 To Region.Length - 1
                Dim pFinder As Finder
                Dim pDT As DataTable
                Dim pFilter1 As DataTable

                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData

                    With .Fields
                        With .Add("StoreName", SqlDbType.UniqueIdentifier)
                            .Expression = "CacheInvoices.FOWSTName"
                        End With

#If DEBUG Then
                        With .Add("TransactionDate", SqlDbType.Bit)
                            .Expression = "convert(date,CacheInvoices.InvoiceDate)"
                        End With
#Else
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "FORMAT(CacheInvoices.InvoiceDate, 'd/MMM')"
                End With
#End If

                        With .Add("GrossSales", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                        End With
                        With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                            .Expression = "ISNULL(" &
                                "(Select SUM(Amount) FROM TransSalesReturnLedger " & vbCrLf &
                                "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoices.FOWSTID and convert(date, TransMemo.LastUpdated) = convert(date,CacheInvoices.InvoiceDate) " & vbCrLf &
                                "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL),'') " 
                        End With
                        
                        With .Add("DiscountAmount", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.SCDiscount),0) + ISNULL(SUM(CacheInvoices.ForegoneVAT),0)"
                        End With
                        With .Add("CashAmount", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.CashNetOfChange),0)"
                        End With
                        
                        With .Add("FoodPanda", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[FoodPanda]),0)"
                        End With
                        With .Add("GCash", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[GCash]),0)"
                        End With
                        With .Add("PayMaya", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[PayMaya]),0)"
                        End With
                        With .Add("GrabFood", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[GrabFood]),0)"
                        End With
                       
                        With .Add("ShopeeEVoucher", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[ShopeeEVoucher]),0)"
                        End With

                        With .Add("LocalDelivery", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[LocalDelivery]),0)"
                        End With

                        With .Add("ShopeePay", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[ShopeePay]),0)"
                        End With

                        With .Add("Eatigo", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[Eatigo]),0)"
                        End With

                        With .Add("DeliRush", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[DeliRush]),0)"
                        End With

                        With .Add("PoodTrip", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[PoodTrip]),0)"
                        End With

                        With .Add("ShareTreats", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[Share Treats]),0)"
                        End With

                        With .Add("Booky", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[Booky]),0)"
                        End With

                        With .Add("Lazada", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[Lazada]),0)"
                        End With

                        With .Add("Pickaroo", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.[Pickaroo]),0)"
                        End With

                        With .Add("SupercardVoucher", SqlDbType.Bit)
                            .Expression = "ISNULL(SUM(CacheInvoices.SupercardVoucher),0)"
                        End With

                    End With

                    .SqlFrom = "FROM CacheInvoices " & vbCrLf

                    If mMonth = 12 Then
                        .SqlWhere &= " WHERE CONVERT(date, CacheInvoices.InvoiceDate) between '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth & "-31') " & vbCrLf
                    Else
                        .SqlWhere &= "WHERE CONVERT(date, CacheInvoices.InvoiceDate) between '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth + 1 & "-01') " & vbCrLf
                    End If

                    .SqlWhere &= "AND CacheInvoices.FranchiseGroupID IN (" & Region(mRegion) & ")" & vbCrLf


                    .SqlGroup = "GROUP BY " & vbCrLf &
                                "CacheInvoices.FOWSTName " & vbCrLf &
                                ",CacheInvoices.FowstID " & vbCrLf &
                                ", CacheInvoices.InvoiceDate "

                    .SqlOrder = " ORDER BY convert(date,CacheInvoices.InvoiceDate) ASC "

                    pDT = .Requery

                    pFilter1 = CreateCrossTab(pDT, {"StoreName"}, {New CrossColumn("GrossSales {TransactionDate}", "GrossSales", "GrossSales"),
                                                                                    New CrossColumn("Refund {TransactionDate}", "SalesReturnAndRefund", "SalesReturnAndRefund"),
                                                                                    New CrossColumn("Discount {TransactionDate}", "DiscountAmount", "DiscountAmount"),
                                                                                    New CrossColumn("Cash {TransactionDate}", "CashAmount", "CashAmount"),
                                                                                    New CrossColumn("GCash {TransactionDate}", "GCash", "GCash"),
                                                                                    New CrossColumn("PayMaya {TransactionDate}", "PayMaya", "PayMaya"),
                                                                                    New CrossColumn("ShopeePay {TransactionDate}", "ShopeePay", "ShopeePay"),
                                                                   New CrossColumn("ShopeeEVoucher {TransactionDate}", "ShopeeEVoucher", "ShopeeEVoucher"),
                                                                   New CrossColumn("GrabFood {TransactionDate}", "GrabFood", "GrabFood"),
                                                                   New CrossColumn("FoodPanda {TransactionDate}", "FoodPanda", "FoodPanda"),
                                                                   New CrossColumn("Eatigo {TransactionDate}", "Eatigo", "Eatigo"),
                                                                   New CrossColumn("DeliRush {TransactionDate}", "DeliRush", "DeliRush"),
                                                                   New CrossColumn("PoodTrip {TransactionDate}", "PoodTrip", "PoodTrip"),
                                                                   New CrossColumn("ShareTreats {TransactionDate}", "ShareTreats", "ShareTreats"),
                                                                   New CrossColumn("Booky {TransactionDate}", "Booky", "Booky"),
                                                                   New CrossColumn("Lazada {TransactionDate}", "Lazada", "Lazada"),
                                                                   New CrossColumn("Pickaroo {TransactionDate}", "Pickaroo", "Pickaroo"),
                                                                   New CrossColumn("LocalDelivery {TransactionDate}", "LocalDelivery", "LocalDelivery")})
                End With

                pFilter1.Columns.Remove("GrossSales")
                pFilter1.Columns.Remove("SalesReturnAndRefund")
                pFilter1.Columns.Remove("DiscountAmount")
                pFilter1.Columns.Remove("CashAmount")
                pFilter1.Columns.Remove("GCash")
                pFilter1.Columns.Remove("PayMaya")
                pFilter1.Columns.Remove("ShopeePay")
                pFilter1.Columns.Remove("ShopeeEVoucher")
                pFilter1.Columns.Remove("GrabFood")
                pFilter1.Columns.Remove("FoodPanda")
                pFilter1.Columns.Remove("Eatigo")
                pFilter1.Columns.Remove("DeliRush")
                pFilter1.Columns.Remove("PoodTrip")
                pFilter1.Columns.Remove("ShareTreats")
                pFilter1.Columns.Remove("Booky")
                pFilter1.Columns.Remove("Lazada")
                pFilter1.Columns.Remove("Pickaroo")
                pFilter1.Columns.Remove("LocalDelivery")


                Dim pSourceURL As String = Common.ServerPath & "temp\OneTimeQuery"
                Dim strFileName As String = RegionName(mRegion) & " - " & mMonth & " - Daily Sales per Report 2024.xls"

                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                TableToExcelFile(pFilter1, "", "", pSourceURL & "\" & strFileName)

            Next
        Next

        '================================================================================================= Daily Sales per SKU Report =================================================

        '        For mMonth = Month To 12
        '            For mRegion As Integer = 0 To Region.Length - 1
        '                Dim pFinder As Finder
        '                Dim pDT As DataTable
        '                Dim SqlWhere As String
        '                Dim strWhereFowst As String
        '                Dim SqlGroupBy As String
        '                Dim SqlOrderBy As String
        '                Dim SqlFrom As String
        '                Dim Fowst As String()
        '                Dim CtrFowst As Integer = 1
        '                Dim pFilter1 As DataTable
        '                Dim pSql As String

        '                pFinder = New Finder

        '                With pFinder

        '                    '.Distinct = True
        '                    .ProData = Common.WebRequest.ProData

        '                    With .Fields

        '#If DEBUG Then
        '                        With .Add("TransactionDate", SqlDbType.Bit)
        '                            .Expression = "convert(date, InvoiceDate)"
        '                        End With
        '#Else
        '                        With .Add("TransactionDate", SqlDbType.Bit)
        '                            .Expression = "FORMAT(InvoiceDate, 'd/MMM')"
        '                        End With
        '#End If

        '                        With .Add("ProductDescription", SqlDbType.VarChar)
        '                            .Expression = "ISNULL(ProductName,'')"
        '                        End With
        '                        With .Add("Qty", SqlDbType.VarChar)
        '                            .Expression = "ISNULL(SUM(Qty),0)"
        '                        End With
        '                        With .Add("UnitMeasure", SqlDbType.VarChar)
        '                            .Expression = "ISNULL(UnitMeasure,'')"
        '                        End With
        '                        With .Add("SubTotal", SqlDbType.VarChar)
        '                            .Expression = "ISNULL(SUM(SubTotal),0) "
        '                        End With
        '                    End With

        '                    .SqlFrom = "FROM CacheDailySalesPerSKUReport " & vbCrLf

        '                    If mMonth = 12 Then
        '                        .SqlWhere &= " WHERE CONVERT(date, InvoiceDate) between '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth & "-31') " & vbCrLf
        '                    Else
        '                        .SqlWhere &= "WHERE CONVERT(date, InvoiceDate) between '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth + 1 & "-01') " & vbCrLf
        '                    End If

        '                    .SqlGroup = "GROUP BY convert(date, InvoiceDate), ISNULL(ProductName,''), ISNULL(UnitMeasure,'') " & vbCrLf


        '                    .SqlWhere &= " AND FranchiseGroupID IN (" & Region(mRegion) & ") " & vbCrLf


        '                    pDT = .Requery

        '                End With


        '                pFilter1 = CreateCrossTab(pDT, {"ProductDescription", "UnitMeasure"}, {New CrossColumn("Qty {TransactionDate}", "Qty", "Qty"),
        '                                                                                        New CrossColumn("Sales {TransactionDate}", "SubTotal", "SubTotal")})

        '                'End With

        '                pFilter1.Columns.Remove("Qty")
        '                pFilter1.Columns.Remove("SubTotal")

        '                Dim pSourceURL As String = Common.ServerPath & "temp\OneTimeQuery"
        '                Dim strFileName As String = RegionName(mRegion) & " - " & mMonth & " - Daily Sales per SKU Report 2024.xls"

        '                If Not System.IO.Directory.Exists(pSourceURL) Then
        '                    IO.Directory.CreateDirectory(pSourceURL)
        '                End If

        '                TableToExcelFile(pFilter1, "", "", pSourceURL & "\" & strFileName)

        '            Next
        '        Next


        '================================================================================================= Yearly Conso SCD =================================================
        'Dim Region() As String
        'Dim Month As Integer
        'Dim pDT As DataTable
        'Dim RegionName() = {"'CO - GMAN'", "'CO - GMAS'", "'CO - Mindanao'", "'CO - North Luzon'", "'CO - South Luzon'", "'CO - Visayas'"}

        'If Not CStr2(Common.WebRequest.QueryString("Region")) = "" Then
        '    ReDim Region(0)

        '    Region(0) = "'CO - South Luzon'"

        'Else
        '    ReDim Region(5)

        '    Region(0) = "'0a31a6e7-f2d9-404c-b5cd-0f970c840b2b'"
        '    Region(1) = "'70312661-460a-42ad-a85b-0c46e2e727c4'"
        '    Region(2) = "'1f41e72c-35cc-4a82-acd9-27760751f499'"
        '    Region(3) = "'e9db8b68-b1ea-4f38-9c45-8bbdb2334e78'"
        '    Region(4) = "'5c8d6617-1db5-460f-8b61-d5ed693e0f07'"
        '    Region(5) = "'78aeb14d-9b95-47a6-82d3-2eb8f87b76bc'"

        'End If

        'If Not CInt2(Common.WebRequest.QueryString("mMonth")) = 0 Then
        '    Month = CInt2(Common.WebRequest.QueryString("mMonth"))
        'Else
        '    Month = 1
        'End If

        'Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(OrganizationDatabaseName))

        'For mMonth = Month To 12
        '    For mRegion As Integer = 0 To Region.Length - 1
        '        Dim pFilter1 As DataTable
        '        Dim pFinder As Finder

        '        pFinder = New Finder

        '        With pFinder

        '            .Distinct = True
        '            .ProData = Common.WebRequest.ProData

        '            With .Fields
        '                With .Add("StoreName", SqlDbType.VarChar)
        '                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
        '                End With
        '                With .Add("InvoiceDate", SqlDbType.VarChar)
        '                    .Expression = "convert(date,dbo.TransInvoice.InvoiceDate)"
        '                End With
        '                With .Add("ProductDescription", SqlDbType.VarChar)
        '                    .Expression = "ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') "
        '                End With
        '                With .Add("Qty", SqlDbType.VarChar)
        '                    .Expression = "COUNT(TransInvoiceItems.ProductID)"
        '                End With
        '            End With

        '            .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
        '                        "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN   " & vbCrLf &
        '                        "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN   " & vbCrLf &
        '                        "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID   " & vbCrLf &
        '                        "INNER JOIN TransInvoiceSeniorCitizenPWDCustomer on TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = TransInvoice.ID   " & vbCrLf &
        '                        "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID  " & vbCrLf &
        '                        "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf
        '            .SqlWhere = " WHERE(dbo.TransInvoice.InvoiceNumber > 0 Or TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
        '                          "AND TransInvoice.SCDiscount>0 " & vbCrLf &
        '                         "AND LibraryFranchiseGroup.ID IN (" & Region(mRegion) & ")" & vbCrLf

        '            If mMonth = 12 Then
        '                .SqlWhere &= " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) between '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth & "-31') " & vbCrLf
        '            Else
        '                .SqlWhere &= "AND CONVERT(date, dbo.TransInvoice.InvoiceDate) between '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth + 1 & "-01') " & vbCrLf
        '            End If

        '            .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.Name, convert(date,dbo.TransInvoice.InvoiceDate), MobizLibraryPIMProduct.ProductGroupNo, MobizLibraryPIMProduct.ProductNo, MobizLibraryPIMProduct.ProductName "

        '            .SqlOrder &= " ORDER BY convert(date,TransInvoice.InvoiceDate) ASC "

        '            pDT = .Requery

        '            pFilter1 = CreateCrossTab(pDT, {"StoreName", "ProductDescription"}, {New CrossColumn("Qty {InvoiceDate}", "Qty", "Qty")})

        '            pFilter1.Columns.Remove("Qty")

        '        End With


        '        'Sort Columns

        '        Dim tempDatatable As New DataTable
        '        Dim mHeaders As New List(Of Date)


        '        For i As Integer = 0 To pFilter1.Columns.Count - 1
        '            If IsDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("Qty", ""))) Then
        '                mHeaders.Add(CDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("Qty", ""))))
        '            End If
        '        Next

        '        mHeaders.Sort()

        '        tempDatatable.Columns.Add("StoreName")
        '        tempDatatable.Columns.Add("ProductDescription")

        '        For i As Integer = 0 To mHeaders.Count - 1
        '            tempDatatable.Columns.Add(mHeaders(i))
        '        Next

        '        For i As Integer = 0 To pFilter1.Rows.Count - 1
        '            Dim dr As DataRow

        '            dr = tempDatatable.NewRow

        '            dr(0) = pFilter1.Rows(i).Item("StoreName")
        '            dr(1) = pFilter1.Rows(i).Item("ProductDescription")

        '            For j As Integer = 2 To tempDatatable.Columns.Count - 1
        '                dr(j) = pFilter1.Rows(i).Item("Qty " & tempDatatable.Columns(j).ColumnName)
        '            Next

        '            tempDatatable.Rows.Add(dr)
        '        Next

        '        Dim pSourceURL As String = Common.ServerPath & "temp\OneTimeQuery"
        '        Dim strFileName As String = RegionName(mRegion) & " - " & mMonth & " - Consolidated SCD Report 2024.xls"

        '        If Not System.IO.Directory.Exists(pSourceURL) Then
        '            IO.Directory.CreateDirectory(pSourceURL)
        '        End If

        '        TableToExcelFile(pFilter1, "", "", pSourceURL & "\" & strFileName)

        '    Next
        'Next

        '================================================================================================= Yearly consumption report per month =================================================
        'Dim pSql As String
        'Dim pDT As DataTable
        'Dim pFilter1 As DataTable
        'Dim k As Integer = 1
        'Dim Region() As String

        'If Not CStr2(Common.WebRequest.QueryString("Region")) = "" Then
        '    ReDim Region(0)

        '    Region(0) = "'CO - South Luzon'"
        '    'Region(0) = "'" & CStr2(Common.WebRequest.QueryString("Region")).Replace("-", " - ") & "'"

        'Else
        '    ReDim Region(5)

        '    Region(0) = "'CO - GMAN'"
        '    Region(1) = "'CO - GMAS'"
        '    Region(2) = "'CO - Mindanao'"
        '    Region(3) = "'CO - North Luzon'"
        '    Region(4) = "'CO - South Luzon'"
        '    Region(5) = "'CO - Visayas'"

        'End If

        'If Not CInt2(Common.WebRequest.QueryString("mMonth")) = 0 Then
        '    k = CInt2(Common.WebRequest.QueryString("mMonth"))
        'Else
        '    k = 12
        'End If

        'Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(OrganizationDatabaseName))

        'For mMonth = k To 12

        '    For j As Integer = 0 To Region.Length - 1

        '        pSql = "SELECT DISTINCT TOP 100 PERCENT ISNULL(Category,'-') AS [Category] " & vbCrLf &
        '                ", FOWSTNAME AS Store " & vbCrLf &
        '                ", ISNULL(ProductName,'-') AS [ProductName] " & vbCrLf &
        '                ", CacheInventoryMovement.ProductID AS [ProductID] " & vbCrLf &
        '                ", ISNULL(UnitMeasure,'-') AS [UnitMeasure] " & vbCrLf &
        '                ", TransactionDate AS [TransactionDate] " & vbCrLf &
        '                ", convert(bigint,ISNULL(CacheInventoryMovement.BeginningBalance, 0)) AS [BeginningBalance] " & vbCrLf &
        '                ", ISNULL(CacheInventoryMovement.Delivery, 0) AS [Delivery] " & vbCrLf &
        '                ", ISNULL(CacheInventoryMovement.TransIn, 0) AS [TransIn] " & vbCrLf &
        '                ", ISNULL(CacheInventoryMovement.TransOut, 0) AS [TransOut] " & vbCrLf &
        '                ", 0 AS [Wastage] " & vbCrLf &
        '                ", convert(bigint,ISNULL(CacheInventoryMovement.EndingBalance, 0)) AS [EndingBalance] " & vbCrLf &
        '                ", ISNULL(CacheInventoryMovement.Usage, 0) * -1 AS [Usage] " & vbCrLf &
        '                ", convert(bigint, ISNULL(CacheInventoryMovement.BeginningBalance, 0) + ISNULL(CacheInventoryMovement.Delivery, 0) +  ISNULL(CacheInventoryMovement.TransIn, 0)  " & vbCrLf &
        '                "- ISNULL(CacheInventoryMovement.TransOut, 0) - ISNULL(CacheInventoryMovement.EndingBalance, 0)  " & vbCrLf &
        '                "+ ISNULL(CacheInventoryMovement.Usage, 0))  AS [Variance] " & vbCrLf &
        '                ", ISNULL(LandedCost, 0) AS [Cost] " & vbCrLf &
        '                ", ISNULL(LandedCost, 0) * ISNULL(CacheInventoryMovement.Usage, 0) * -1 AS [TotalCost] " & vbCrLf &
        '                ", ISNULL(LandedCost, 0) * ISNULL(CacheInventoryMovement.Usage, 0) * -1 / 1.12 AS [SalesNetVAT] " & vbCrLf &
        '                ", convert(bigint, ISNULL(tblInventoryCount.Quantity, 0)) AS [InventoryCount] " & vbCrLf &
        '                ", ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999)  AS [InventoryRankingNo] " & vbCrLf &
        '                ", ISNULL(CacheInventoryMovement.ProductDefect,0)  AS [ProductDefect] " & vbCrLf &
        '                "        FROM CacheInventoryMovement " & vbCrLf &
        '                " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = CacheInventoryMovement.ProductID  " & vbCrLf &
        '                "OUTER APPLY (  " & vbCrLf &
        '                "SELECT DISTINCT TOP 1 Quantity, TransInventoryProductCount.InventoryCountDate  " & vbCrLf &
        '                "FROM TransInventoryProductCountStock INNER JOIN TransInventoryProductCount ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID  " & vbCrLf &
        '                "WHERE TransInventoryProductCount.FowstID = CacheInventoryMovement.FowstID  " & vbCrLf &
        '                "AND TransInventoryProductCount.ProductID = CacheInventoryMovement.ProductID  " & vbCrLf &
        '                "AND CONVERT(date, TransInventoryProductCount.InventoryCountDate) = convert(date,CacheInventoryMovement.TransactionDate)  " & vbCrLf &
        '                "ORDER BY TransInventoryProductCount.InventoryCountDate DESC  " & vbCrLf &
        '                ") tblInventoryCount   WHERE ISNULL(LibraryOrganizationPIM.IsInventoryCountItem,0) = 1   " & vbCrLf

        '        If mMonth = 12 Then
        '            pSql &= " AND convert(date,CacheInventoryMovement.TransactionDate) BETWEEN '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth & "-31') " & vbCrLf
        '        Else
        '            pSql &= " AND convert(date,CacheInventoryMovement.TransactionDate) BETWEEN '2024-" & mMonth & "-01' AND DateAdd(day, -1, '2024-" & mMonth + 1 & "-01') " & vbCrLf
        '        End If

        '        pSql &= " --AND CacheInventoryMovement.TransactionDate BETWEEN '2020-01-01 00:00:00' AND '2024-12-31 00:00:00' " & vbCrLf &
        '                " AND Groupname = " & Region(j) & vbCrLf &
        '                " --AND CacheInventoryMovement.FowstID in (Select ID from MobizLibraryOrganizationFOWST where organizationid = 'd362ffdb-15d1-451f-97dc-c567b94fb410') " & vbCrLf &
        '                "    ORDER BY InventoryRankingNo "

        '        Common.WebRequest.ProData.CommandTimeout = 99999999

        '        pDT = Common.WebRequest.ProData.Execute(pSql)

        '        pDT.Columns.Add("TheoreticalEnding")

        '        For i As Integer = 0 To pDT.Rows.Count - 1
        '            If CDec2(pDT.Rows(i).Item("InventoryCount")) > 0 Then
        '                pDT.Rows(i).Item("TheoreticalEnding") = CDec2(pDT.Rows(i).Item("InventoryCount")) - (CDec2(pDT.Rows(i).Item("Variance")) * -1)
        '            Else
        '                pDT.Rows(i).Item("TheoreticalEnding") = CDec2(pDT.Rows(i).Item("EndingBalance"))
        '            End If
        '        Next

        '        pFilter1 = CreateCrossTab(pDT, {"ProductName", "UnitMeasure", "Cost"}, {New CrossColumn("Beg Bal {TransactionDate}", "BeginningBalance", "BeginningBalance"),
        '                                                                        New CrossColumn("Del {TransactionDate}", "Delivery", "Delivery"),
        '                                                                        New CrossColumn("Trans In {TransactionDate}", "TransIn", "TransIn"),
        '                                                                        New CrossColumn("Trans Out {TransactionDate}", "TransOut", "TransOut"),
        '                                                                        New CrossColumn("Product Defect {TransactionDate}", "ProductDefect", "ProductDefect"),
        '                                                                        New CrossColumn("Usage {TransactionDate}", "Usage", "Usage"),
        '                                                                        New CrossColumn("EndBal {TransactionDate}", "EndingBalance", "EndingBalance"),
        '                                                                        New CrossColumn("Variance {TransactionDate}", "Variance", "Variance"),
        '                                                                        New CrossColumn("Actual Count {TransactionDate}", "InventoryCount", "InventoryCount"),
        '                                                                        New CrossColumn("Theoretical Ending {TransactionDate}", "TheoreticalEnding", "TheoreticalEnding"),
        '                                                                        New CrossColumn("Total Cost {TransactionDate}", "TotalCost", "TotalCost"),
        '                                                                        New CrossColumn("Sales N/ VAT {TransactionDate}", "SalesNetVAT", "SalesNetVAT")})


        '        pFilter1.Columns.Remove("BeginningBalance")
        '        pFilter1.Columns.Remove("Delivery")
        '        pFilter1.Columns.Remove("TransIn")
        '        pFilter1.Columns.Remove("TransOut")
        '        pFilter1.Columns.Remove("Usage")
        '        pFilter1.Columns.Remove("EndingBalance")
        '        pFilter1.Columns.Remove("Variance")
        '        pFilter1.Columns.Remove("InventoryCount")
        '        pFilter1.Columns.Remove("TheoreticalEnding")
        '        pFilter1.Columns.Remove("ProductDefect")
        '        pFilter1.Columns.Remove("TotalCost")
        '        pFilter1.Columns.Remove("SalesNetVAT")

        '        Dim pSourceURL As String = Common.ServerPath & "temp\OneTimeQuery"
        '        Dim strFileName As String = Region(j) & " - " & mMonth & " - Consumption Report 2024.xls"

        '        If Not System.IO.Directory.Exists(pSourceURL) Then
        '            IO.Directory.CreateDirectory(pSourceURL)
        '        End If

        '        TableToExcelFile(pFilter1, "", "", pSourceURL & "\" & strFileName)

        '    Next
        'Next
        '================================================================================================= Yearly consumption report per month =================================================

        Return True
    End Function
#End Region

    Dim pJSONFinalTransaction As String = ""

    Function TestPITX() As String

        Dim pSql As String
        Dim pDT As DataTable
        Dim pJsonString As String

        Dim PDTMainTable As New DataTable
        Dim submission_uuid As String = Guid.NewGuid.ToString
        Dim tenant_id As Integer = 95
        Dim terminal_id As Integer = 102
        Dim submission_timestamp As String = Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'")
        Dim transaction_count As Integer = 1
        Dim payload_checksum As String
        Dim pBytes() As Byte


        Dim pResult As String
        Dim pResultDictionary As New JsonDictionary

        Dim pitx As New MallLessorPITX

        pitx.StartSendingTransactionToLessor()

        Return ""

        '===================== test checksum ===================== 
        Dim testSTR As String

        Dim utf8String As String

        testSTR = "{""adjustments"":[{""adjustment_type"":""promo_discount"",""amount"":""0.00""},{""adjustment_type"":""senior_discount"",""amount"":""0.00""},{""adjustment_type"":""pwd_discount"",""amount"":""0.00""},{""adjustment_type"":""vip_card_discount"",""amount"":""0.00""},{""adjustment_type"":""service_charge_distributed_to_employees"",""amount"":""0.00""},{""adjustment_type"":""service_charge_retained_by_management"",""amount"":""0.00""},{""adjustment_type"":""employee_discount"",""amount"":""0.00""}],""customer_code"":""C-B1028"",""gross_sales"":""140.00"",""hardware_id"":""BUI-XTM80213"",""net_sales"":""125.00"",""promo_status"":""WITHOUT_APPROVAL"",""receipt_no"":""000001072840"",""taxes"":[{""amount"":""15.00"",""tax_type"":""VAT""},{""amount"":""125.00"",""tax_type"":""VATABLE_SALES""},{""amount"":""0.00"",""tax_type"":""SC_VAT_EXEMPT_SALES""},{""amount"":""0.00"",""tax_type"":""OTHER_TAX""}],""transaction_id"":""f9c5dd6a-2d71-40b8-903c-df0a02b417ed"",""transaction_timestamp"":""2026-05-14T08:59:28Z""}"

        pBytes = Encoding.UTF8.GetBytes(testSTR)
      
        pBytes = New SHA256Managed().ComputeHash(pBytes)

        payload_checksum = BitConverter.ToString(pBytes).Replace("-", "").ToLower


        '===================== end of test checksum ===================== 

        PDTMainTable.Columns.Add("submission_uuid")
        PDTMainTable.Columns.Add("tenant_id")
        PDTMainTable.Columns.Add("terminal_id")
        PDTMainTable.Columns.Add("submission_timestamp")
        PDTMainTable.Columns.Add("transaction_count")
        PDTMainTable.Columns.Add("payload_checksum")

        'temporary, dapat guide ni sya sa transaction
        PDTMainTable.Columns.Add("transaction_id")

        Dim pDR As DataRow

        pDR = PDTMainTable.NewRow

        pDR(0) = submission_uuid
        pDR(1) = tenant_id
        pDR(2) = terminal_id
        pDR(3) = submission_timestamp
        pDR(4) = transaction_count
        'temporary, dapat guide ni sya sa transaction
        pDR(6) = Guid.NewGuid.ToString

        PDTMainTable.Rows.Add(pDR)


        '------------ Transaction-----------------

        'pSql = "Select top 1 * from CacheInvoices"
        'pDT = mProData.Execute(pSql)

        'pDT.Columns.Add("payload_checksum")

        'pJsonString = CreateJSONPayloadTransaction(pDT)

        ''------------ Transaction-----------------

        '------ Header -----------------

        pJsonString = CreateJSONPayload(PDTMainTable, pJsonString)

        'Return ""

        ''------ header payload checksum -----------------

        '--sending

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Try
            Dim mRequestPresignedUrl As String = "https://stagingtsms.pitx.com.ph/api/v1/transactions/official"
            Dim pWeb = New WebClient()
            Dim pUploadResult As String

            pWeb.Headers.Add("Authorization", "Bearer 438|3ZQBM33wxeJNqK6DoTi613QCe85OwnvhTDqydtrmd76058b6")
            pWeb.Headers.Add("Content-Type", "application/json")

            pUploadResult = pWeb.UploadString(mRequestPresignedUrl, "POST", pJsonString)

            AppendToFile(Common.ServerPath & "temp\logs\PITX.txt", Now.ToString & "-Success \n" & pUploadResult)

        Catch ex As Exception
            AppendToLog(ex.Message)
            AppendToFile(Common.ServerPath & "temp\logs\PITX.txt", Now.ToString & "-Error \n" & ex.Message)
        End Try

    End Function

    Private Function CreateJSONPayload(PDTMainTable As DataTable, pJsonStringTransaction As String) As String
        Dim jFinalString As String
        Dim pJsonHeader As JsonDictionary
        Dim submission_uuid As String
        Dim tenant_id As Integer = 40
        Dim terminal_id As Integer = 55
        Dim submission_timestamp As String
        Dim transaction_count As Integer = 1
        Dim payload_checksum As String

        Dim promo_discount_amount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim senior_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim pwd_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim vip_card_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim service_charge_distributed_to_employees As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim service_charge_retained_by_management As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim employee_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))

        Dim VAT As String = CStr2(CDec2(15).ToString("#,###,##0.00"))
        Dim VATABLE_SALES As String = CStr2(CDec2(125).ToString("#,###,##0.00"))
        Dim SC_VAT_EXEMPT_SALES As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim OTHER_TAX As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))

        Dim transaction_id As String
        Dim hardware_id As String = "BUI-XTM80213"
        Dim receipt_no As String = "000001072840"
        Dim transaction_timestamp As String = "2026-05-14T08:59:28Z" 'Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'")
        Dim gross_sales As String = "140.00"
        Dim net_sales As String = "125.00"
        Dim promo_status As String = "WITHOUT_APPROVAL"
        Dim customer_code As String = "C-B1028"
        Dim payload_transaction_checksum As String = ""
        Dim pJsonForTransactionPayload As JsonDictionary
        Dim pJsonForHeaderPayload As JsonDictionary

        pJsonForTransactionPayload = New JsonDictionary
        pJsonForHeaderPayload = New JsonDictionary

        pJsonHeader = New JsonDictionary

        '=======================Header================================

        submission_uuid = CStr2(PDTMainTable.Rows(0).Item("submission_uuid"))
        tenant_id = CInt2(PDTMainTable.Rows(0).Item("tenant_id"))
        terminal_id = CInt2(PDTMainTable.Rows(0).Item("terminal_id"))
        submission_timestamp = "2026-05-14T08:59:28Z" 'CStr2(CDate(PDTMainTable.Rows(0).Item("submission_timestamp")).ToString("yyyy-MM-ddTHH:mm:ss'Z'"))
        transaction_count = CInt2(PDTMainTable.Rows(0).Item("transaction_count"))
        transaction_id = "f9c5dd6a-2d71-40b8-903c-df0a02b417ed" ' CStr2(PDTMainTable.Rows(0).Item("transaction_id"))

        pJsonHeader.Add("submission_uuid", "" & submission_uuid & "")
        pJsonHeader.Add("tenant_id", tenant_id)
        pJsonHeader.Add("terminal_id", terminal_id)
        pJsonHeader.Add("submission_timestamp", submission_timestamp)
        pJsonHeader.Add("transaction_count", transaction_count)
        'pJsonHeader.Add("payload_checksum", payload_checksum)

        pJsonForHeaderPayload.Add("submission_uuid", "" & submission_uuid & "")
        pJsonForHeaderPayload.Add("tenant_id", tenant_id)
        pJsonForHeaderPayload.Add("terminal_id", terminal_id)
        pJsonForHeaderPayload.Add("submission_timestamp", submission_timestamp)
        pJsonForHeaderPayload.Add("transaction_count", transaction_count)
        'pJsonForHeaderPayload.Add("payload_checksum", payload_checksum)
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

        transaction_timestamp = "2026-05-14T08:59:28Z" 'CStr2(Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'"))

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

        'pJsonHeader.Item("payload_checksum") = payload_checksum

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
                    'AddList("transaction").AddDictionary

                    SortedJSONObjectTransaction = SortJSONObject(jsonTransaction)

                    For Each KeyTrans As String In SortedJSONObjectTransaction.Keys

                        If KeyTrans = "adjustments" OrElse KeyTrans = "taxes" Then

                            'With .AddDictionary(KeyTrans)
                            '    .Add(SortedJSONObjectTransaction(KeyTrans)) 'adjustments
                            'End With

                            .Add(KeyTrans, SortedJSONObjectTransaction(KeyTrans))
                        Else
                            .Add(KeyTrans, SortedJSONObjectTransaction(KeyTrans))
                        End If
                       
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

    Sub PITXSending()

        Dim pSql As String
        Dim pDT As DataTable
        Dim pJsonString As String

        Dim PDTMainTable As New DataTable
        Dim submission_uuid As String = "550e8400-e29b-41d4-a716-446655440000"
        Dim tenant_id As Integer = 95
        Dim terminal_id As Integer = 102
        Dim submission_timestamp As String = Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'")
        Dim transaction_count As Integer = 1
        Dim payload_checksum As String
        Dim pBytes() As Byte
        Dim pDTTransactions As DataTable

        PDTMainTable.Columns.Add("submission_uuid")
        PDTMainTable.Columns.Add("tenant_id")
        PDTMainTable.Columns.Add("terminal_id")
        PDTMainTable.Columns.Add("submission_timestamp")
        PDTMainTable.Columns.Add("transaction_count")
        PDTMainTable.Columns.Add("payload_checksum")


#If DEBUG Then
        pSql = "Select top 1 * from CacheInvoices where FOWSTID = 'cfe39df6-8809-4973-9515-810df75aed12' and convert(date,InvoiceDate) > '2026-05-18' and ISNULL(IsSentToPITX,0) = 0 ORDER BY InvoiceDate "
#Else

#End If

        pDTTransactions = mProData.Execute(pSql)

        If PDTMainTable.Rows.Count > 0 Then

            Dim pDR As DataRow

            pDR = PDTMainTable.NewRow

            pDR(0) = submission_uuid
            pDR(1) = tenant_id
            pDR(2) = terminal_id
            pDR(3) = submission_timestamp
            pDR(4) = transaction_count

            PDTMainTable.Rows.Add(pDR)


            '------------ Transaction-----------------

            'pSql = "Select top 1 * from CacheInvoices"
            'pDT = mProData.Execute(pSql)

            'pDT.Columns.Add("payload_checksum")

            'pJsonString = CreateJSONPayloadTransaction(pDT)

            ''------------ Transaction-----------------

            '------ Header -----------------

            pJsonString = CreateJSONPayload(PDTMainTable, pJsonString)

            'Return ""

            ''------ header payload checksum -----------------

            '--sending

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Try
                Dim mRequestPresignedUrl As String = "https://stagingtsms.pitx.com.ph/api/v1/transactions/official"
                Dim pWeb = New WebClient()
                Dim pUploadResult As String

                pWeb.Headers.Add("Authorization", "Bearer 438|3ZQBM33wxeJNqK6DoTi613QCe85OwnvhTDqydtrmd76058b6")
                pWeb.Headers.Add("Content-Type", "application/json")

                pUploadResult = pWeb.UploadString(mRequestPresignedUrl, "POST", pJsonString)

                Debug.WriteLine(pUploadResult)

            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try




        Else
            Threading.Thread.Sleep(3600000) ' 1 hour
        End If

    End Sub

    Private Function CreateJSONPayloadFinal(PDTMainTable As DataTable, pJsonStringTransaction As String) As String
        Dim jFinalString As String
        Dim pJsonHeader As JsonDictionary
        Dim submission_uuid As String
        Dim tenant_id As Integer = 40
        Dim terminal_id As Integer = 55
        Dim submission_timestamp As String
        Dim transaction_count As Integer = 1
        Dim payload_checksum As String

        Dim promo_discount_amount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim senior_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim pwd_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim vip_card_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim service_charge_distributed_to_employees As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim service_charge_retained_by_management As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim employee_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))

        Dim VAT As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim VATABLE_SALES As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim SC_VAT_EXEMPT_SALES As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
        Dim OTHER_TAX As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))

        Dim transaction_id As String = "TXN-2026-999"
        Dim hardware_id As String = "TA04930027"
        Dim receipt_no As String = "R001-0000456"
        Dim transaction_timestamp As String = Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'")
        Dim gross_sales As String = "1499.00"
        Dim net_sales As String = "1499.00"
        Dim promo_status As String = "WITH_APPROVAL"
        Dim customer_code As String = "C-S1011"
        Dim payload_transaction_checksum As String = ""
        Dim pJsonForTransactionPayload As JsonDictionary
        Dim pJsonForHeaderPayload As JsonDictionary

        pJsonForTransactionPayload = New JsonDictionary
        pJsonForHeaderPayload = New JsonDictionary

        pJsonHeader = New JsonDictionary

        '=======================Header================================

        submission_uuid = CStr2(PDTMainTable.Rows(0).Item("submission_uuid"))
        tenant_id = CInt2(PDTMainTable.Rows(0).Item("tenant_id"))
        terminal_id = CInt2(PDTMainTable.Rows(0).Item("terminal_id"))
        submission_timestamp = CStr2(CDate(PDTMainTable.Rows(0).Item("submission_timestamp")).ToString("yyyy-MM-ddTHH:mm:ss'Z'"))
        transaction_count = CInt2(PDTMainTable.Rows(0).Item("transaction_count"))

        pJsonHeader.Add("submission_uuid", "" & submission_uuid & "")
        pJsonHeader.Add("tenant_id", tenant_id)
        pJsonHeader.Add("terminal_id", terminal_id)
        pJsonHeader.Add("submission_timestamp", submission_timestamp)
        pJsonHeader.Add("transaction_count", transaction_count)
        pJsonHeader.Add("payload_checksum", payload_checksum)

        pJsonForHeaderPayload.Add("submission_uuid", "" & submission_uuid & "")
        pJsonForHeaderPayload.Add("tenant_id", tenant_id)
        pJsonForHeaderPayload.Add("terminal_id", terminal_id)
        pJsonForHeaderPayload.Add("submission_timestamp", submission_timestamp)
        pJsonForHeaderPayload.Add("transaction_count", transaction_count)
        pJsonForHeaderPayload.Add("payload_checksum", payload_checksum)
        pJsonForHeaderPayload.Add("transaction", "")

        '=================For Transaction Checksum=================

        'Tranasction checksum first kay dili ma-assign ang transaction checksum sa inner json

        pJsonForTransactionPayload.Add("transaction_id", "" & transaction_id & "")
        pJsonForTransactionPayload.Add("hardware_id", "" & hardware_id & "")
        pJsonForTransactionPayload.Add("receipt_no", "" & receipt_no & "")
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

        payload_transaction_checksum = CreateJSONPayloadTransaction(pJsonForTransactionPayload)

        pJsonForTransactionPayload.Add("payload_checksum", payload_transaction_checksum)
        '=================For Checksum=================

        '=======================Transaction================================

        transaction_timestamp = CStr2(Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'"))

        With pJsonHeader.AddDictionary("transaction")

            .Add("transaction_id", "" & transaction_id & "")
            .Add("hardware_id", "" & hardware_id & "")
            .Add("receipt_no", "" & receipt_no & "")
            .Add("transaction_timestamp", "" & transaction_timestamp & "")
            .Add("gross_sales", "" & gross_sales & "")
            .Add("net_sales", "" & net_sales & "")
            .Add("promo_status", "" & promo_status & "")
            .Add("customer_code", "" & customer_code & "")
            .Add("payload_checksum", "" & payload_transaction_checksum & "")

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

        End With

        '======================= End of Transaction==============================

        '======================= Header Checksum ================================
        payload_checksum = CreateJSONPayloadMain(pJsonForHeaderPayload, pJsonForTransactionPayload)

        pJsonHeader.Item("payload_checksum") = payload_checksum

        '======================= End of Header Checksum =========================

        jFinalString = pJsonHeader.ToString


        Return jFinalString

    End Function
    'Private Function CreateJSONPayloadTransaction(PDTMainTable As DataTable) As String
    '    Dim jFinalString As String

    '    Dim transaction_id As String = "TXN-2026-999"
    '    Dim hardware_id As String = "TA04930027"
    '    Dim receipt_no As String = "R001-0000456"
    '    Dim transaction_timestamp As String = Date.Now.ToString("yyyy-MM-ddTHH:mm:ss'Z'")
    '    Dim gross_sales As String = "1499.00"
    '    Dim net_sales As String = "1499.00"
    '    Dim promo_status As String = "WITH_APPROVAL"
    '    Dim customer_code As String = "C-S1011"
    '    Dim payload_checksum As String
    '    Dim pJson As JsonDictionary

    '    transaction_timestamp = CStr2(CDate(PDTMainTable.Rows(0).Item("InvoiceDate")).ToString("yyyy-MM-ddTHH:mm:ss'Z'"))

    '    pJson = New JsonDictionary

    '    pJson.Add("transaction_id", "" & transaction_id & "")
    '    pJson.Add("hardware_id", "" & hardware_id & "")
    '    pJson.Add("receipt_no", "" & receipt_no & "")
    '    pJson.Add("transaction_timestamp", "" & transaction_timestamp & "")
    '    pJson.Add("gross_sales", "" & gross_sales & "")
    '    pJson.Add("net_sales", "" & net_sales & "")
    '    pJson.Add("promo_status", "" & promo_status & "")
    '    pJson.Add("customer_code", "" & customer_code & "")
    '    pJson.Add("payload_checksum", "")

    '    'pJson.Add("transaction_id", "" & transaction_id & "")
    '    'pJson.Add("hardware_id", "" & hardware_id & "")
    '    'pJson.Add("receipt_no", "" & receipt_no & "")
    '    'pJson.Add("transaction_timestamp", "" & transaction_timestamp & "")
    '    'pJson.Add("gross_sales", "" & gross_sales & "")
    '    'pJson.Add("net_sales", "" & net_sales & "")
    '    'pJson.Add("promo_status", "" & promo_status & "")
    '    'pJson.Add("customer_code", "" & customer_code & "")
    '    'pJson.Add("payload_checksum", "")

    '    '--adjustments
    '    Dim promo_discount_amount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim senior_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim pwd_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim vip_card_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim service_charge_distributed_to_employees As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim service_charge_retained_by_management As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim employee_discount As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))

    '    With pJson.AddList("adjustments")
    '        With .AddDictionary
    '            .Add("adjustment_type", "promo_discount")
    '            .Add("amount", "" & promo_discount_amount & "")
    '        End With
    '        With .AddDictionary
    '            .Add("adjustment_type", "senior_discount")
    '            .Add("amount", "" & senior_discount & "")
    '        End With
    '        With .AddDictionary
    '            .Add("adjustment_type", "pwd_discount")
    '            .Add("amount", "" & pwd_discount & "")
    '        End With
    '        With .AddDictionary
    '            .Add("adjustment_type", "vip_card_discount")
    '            .Add("amount", "" & vip_card_discount & "")
    '        End With
    '        With .AddDictionary
    '            .Add("adjustment_type", "service_charge_distributed_to_employees")
    '            .Add("amount", "" & service_charge_distributed_to_employees & "")
    '        End With
    '        With .AddDictionary
    '            .Add("adjustment_type", "service_charge_retained_by_management")
    '            .Add("amount", "" & service_charge_retained_by_management & "")
    '        End With
    '        With .AddDictionary
    '            .Add("adjustment_type", "employee_discount")
    '            .Add("amount", "" & employee_discount & "")
    '        End With
    '    End With

    '    '--end of adjustments


    '    '--taxes

    '    Dim VAT As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim VATABLE_SALES As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim SC_VAT_EXEMPT_SALES As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))
    '    Dim OTHER_TAX As String = CStr2(CDec2(0.0).ToString("#,###,##0.00"))

    '    With pJson.AddList("taxes")
    '        With .AddDictionary
    '            .Add("tax_type", "VAT")
    '            .Add("amount", "" & VAT & "")
    '        End With
    '        With .AddDictionary
    '            .Add("tax_type", "VATABLE_SALES")
    '            .Add("amount", "" & VATABLE_SALES & "")
    '        End With
    '        With .AddDictionary
    '            .Add("tax_type", "SC_VAT_EXEMPT_SALES")
    '            .Add("amount", "" & SC_VAT_EXEMPT_SALES & "")
    '        End With
    '        With .AddDictionary
    '            .Add("tax_type", "OTHER_TAX")
    '            .Add("amount", "" & OTHER_TAX & "")
    '        End With
    '    End With

    '    Dim SortedJSONObject As String
    '    Dim pJsonStr As String

    '    'If payload_checksum.Length = 0 Then

    '    SortedJSONObject = SortJSONObject(pJson)

    '    'add checksum
    '    Dim pBytes() As Byte

    '    pBytes = New System.Text.ASCIIEncoding().GetBytes(SortedJSONObject)
    '    pBytes = New SHA256Managed().ComputeHash(pBytes)
    '    payload_checksum = BitConverter.ToString(pBytes).Replace("-", "").ToLower

    '    pJson("payload_checksum") = payload_checksum

    '    pJsonStr = pJson.ToString
    '    'End If

    '    Return pJsonStr

    'End Function

End Class
