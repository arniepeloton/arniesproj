Namespace WebProj

    Public Class TriggersController
        Inherits System.Web.Mvc.Controller

        Dim mBusiness As Business
        '
        ' GET: /Triggers

        Function Index() As ActionResult
            StartTrigger()
            AppendToLog("Reports and Subscription has been triggered successfully")
            Return View()
        End Function
        ''' <summary>
        ''' '''''''''''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function PIMBrandOwner() As ActionResult
            CopyProductNoToBrandOwner()
            AppendToLog("CopyProductNoToBrandOwner has been triggered successfully")
            Return View()
        End Function
        'nike
        Function RemoveDuplicate() As ActionResult
            RemoveDuplicateProductNo()
            AppendToLog("RemoveDuplicateProductNo has been triggered successfully")
            Return View()
        End Function
        ''' <summary>
        ''' '''''''''''''''''''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ProductNumber() As ActionResult
            'GenerateProductNumber()
            CopyProductNumberFromBarcode()
            AppendToLog("GenerateProductNumber has been triggered successfully")
            Return View()
        End Function

        Function Duplicate() As ActionResult
            DuplicateProductNoAddSuffix()
            AppendToLog("DuplicateProductNoAddSuffix has been triggered successfully")
            Return View()
        End Function
        ''' <summary>
        ''' ''''''''''''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function StartTrigger()
            Dim dtSubscriptionDatabase As DataTable
            Dim triggers As New Triggers(Common.WebRequest.ProData)

            dtSubscriptionDatabase = GetAllSubscriptionDatabase()

            For i As Integer = 0 To dtSubscriptionDatabase.Rows.Count - 1

                If (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("Nike")) Then

                    triggers.ConnectToDatabase(CStr(dtSubscriptionDatabase.Rows(i).Item("Database")))

                    triggers.NikeSEAReport(dtSubscriptionDatabase.Rows(i).Item("Database"))
                    triggers.NikeSEAReportVersion2(CStr2(dtSubscriptionDatabase.Rows(i).Item("OrganizationID")), dtSubscriptionDatabase.Rows(i).Item("Database"))
                    triggers.ShopperTrakReport(dtSubscriptionDatabase.Rows(i).Item("Database"))

                ElseIf (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("ToraSports")) Then

                    triggers.OnitsukaReport(CStr(dtSubscriptionDatabase.Rows(i).Item("Database")))
                    'ElseIf (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("MobizAficionadoTesting")) Then
                    '    Try
                    '        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
                    '        mBusiness.ProData.Execute("USE " & EscapeSqlName(CStr(dtSubscriptionDatabase.Rows(i).Item("Database"))))
                    '        Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(CStr(dtSubscriptionDatabase.Rows(i).Item("Database"))))
                    '        SendDailyAlerts(CStr2(dtSubscriptionDatabase.Rows(i).Item("OrganizationID")), dtSubscriptionDatabase.Rows(i).Item("Database"))
                    '    Catch ex As Exception
                    '        AppendToLog(ex.Message)
                    '    End Try
                End If
            Next

            '''Trigger for Subscription
            CheckForSubscriber()

        End Function

        Function CheckForSubscriber()

            TimedThread(
                Sub()

                    Try

                        Dim clsSubscription As New SubscriptionController
                        clsSubscription.DownloadNewSubscriber()

                    Catch ex As Exception
                        AppendToLog(ex.Message)
                    Finally
                    End Try

                End Sub
            )
        End Function

        Function GetAllSubscriptionDatabase() As DataTable

            Dim pSql As String
            pSql = "select * from Librarysubscription"

            Return Common.WebRequest.ProData.Execute(pSql)

        End Function

        Function RunDeviceMonitoringCheck() As String

            Dim pSql As String
            Dim triggers As New Triggers
            Dim pProData As ProData

            pProData = New ProData()
            pProData.FailedConnectRetryDuration = 60 * 1000
            pProData.Connect(Common.ConnectionInfo)

            triggers = New Triggers(pProData)

            triggers.ConnectToDatabase("MobizABC")

            Return CStr(triggers.RunDeviceMonitoringCheck("MobizABC"))

        End Function

        Function RunDeviceMonitoringCheckProd() As String
            Dim triggers As New Triggers
            Dim pProData As ProData

            pProData = New ProData()
            pProData.FailedConnectRetryDuration = 60 * 1000
            pProData.Connect(Common.ConnectionInfo)

            triggers = New Triggers(pProData)

            triggers.ConnectToDatabase("MobizCinco_Corporation")

            triggers.RunDeviceMonitoringByManager("MobizCinco_Corporation")

            triggers.EmailAMAndDMforDeviceMonitoring("MobizCinco_Corporation")

            Return CStr(triggers.RunDeviceMonitoringCheck("MobizCinco_Corporation"))

        End Function

        Sub CopyProductNumberFromBarcode()
            Dim p As New InsertUpdate("MobizLibraryPIMProduct")
            Dim pSql As String
            Dim pDT As DataTable

            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE Mobiz")

            Try
                mBusiness.ProData.BeginTransaction()
                pSql = "SELECT DISTINCT MobizLibraryPIMProduct.ID, MobizLibraryPIMProductMeasurementBarcode.Barcode FROM MobizLibraryPIMProductMeasurementBarcode " &
                    " INNER JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID = MobizLibraryPIMProductMeasurement.ID " &
                    " INNER JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = MobizLibraryPIMProductMeasurement.ProductID " &
                    " WHERE ProductNo = '' and Barcode <> '' and MobizLibraryPIMProductMeasurement.MeasurementLevel = 1  order by MobizLibraryPIMProduct.id"
                pDT = mBusiness.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    For Each pDR As DataRow In pDT.Rows
                        p("ProductNo") = ReadyString(pDR("Barcode"))
                        p("LastUpdated") = "getdate()"

                        pSql = p.GetUpdate(" ID = " & ReadyString(pDR("ID")))
                        mBusiness.ProData.ExecuteSQL(pSql)
                    Next

                End If

                mBusiness.ProData.CommitTransaction()
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
            End Try
        End Sub

        Sub DuplicateProductNoAddSuffix()
            Dim p As New InsertUpdate("MobizLibraryPIMProduct")
            Dim pSql As String
            Dim pDT As DataTable
            Dim pSql2 As String
            Dim pDT2 As DataTable
            Dim pSql3 As String
            Dim count As Int16

            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE Mobiz")
            'is not null and ProductNo <> '' " &
            Try
                mBusiness.ProData.BeginTransaction()
                pSql = "select ProductNo from LibraryPIMProduct " &
                    " where productno is not null and productno <> '' and PublishedByID ='9D4B1500-13C0-49F9-A8E5-A16C13EC899E' " &
                    " group by ProductNo having COUNT(ProductNo) > 1"
                pDT = mBusiness.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    For Each pDR As DataRow In pDT.Rows

                        pSql2 = "select ID from LibraryPIMProduct where ProductNo = " & ReadyString(pDR("ProductNo"))
                        pDT2 = mBusiness.ProData.Execute(pSql2)

                        count = 1
                        For Each pDR2 As DataRow In pDT2.Rows

                            p("ProductNo") = ReadyString(pDR("ProductNo").ToString() & "-" & count)
                            p("LastUpdated") = "getdate()"

                            pSql3 = p.GetUpdate(" ID = " & ReadyString(pDR2("ID")))
                            mBusiness.ProData.ExecuteSQL(pSql3)

                            count = count + 1
                        Next
                      
                    Next

                End If

                mBusiness.ProData.CommitTransaction()
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
            End Try
        End Sub

        'Sub GenerateProductNumber()
        '    Dim p As New InsertUpdate("MobizLibraryPIMProduct")
        '    Dim pSql As String
        '    Dim pDT As DataTable
        '    mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
        '    mBusiness.ProData.Execute("USE Mobiz")

        '    Try
        '        mBusiness.ProData.BeginTransaction()
        '        pSql = "SELECT ID FROM MobizLibraryPIMProduct WHERE (ProductNo = '' OR ProductNo IS NULL) AND PublishedByID = '9D4B1500-13C0-49F9-A8E5-A16C13EC899E'"
        '        pDT = mBusiness.ProData.Execute(pSql)

        '        If (pDT.Rows.Count > 0) Then
        '            For Each pDR As DataRow In pDT.Rows
        '                p("ProductNo") = ReadyString(GenerateString(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ") & "" & GenerateString(7, "1234567890"))
        '                p("LastUpdated") = "getdate()"

        '                pSql = p.GetUpdate(" ID = " & ReadyString(pDR("ID")))
        '                mBusiness.ProData.ExecuteSQL(pSql)
        '            Next

        '        End If


        '        mBusiness.ProData.CommitTransaction()
        '    Catch ex As Exception
        '        mBusiness.ProData.RollbackTransaction()
        '    End Try
        'End Sub

        Sub CopyProductNoToBrandOwner()
            Dim p As New InsertUpdate("MobizLibraryPIMProduct")
            Dim p2 As New InsertUpdate("LibraryOrganizationPIM")
            Dim p3 As New InsertUpdate("MobizLibraryPIMProduct")
            Dim pSql As String
            Dim pSql2 As String
            Dim pSql3 As String
            Dim pDT As DataTable


            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE MobizNike")

            Try
                mBusiness.ProData.BeginTransaction()

                pSql = "select distinct pim.ID as nikID, t.ID as athID, t.ProductNo, pim.productgroupno, feat.C0017 " &
                    " from MobizLibraryPIMProduct pim inner join " &
                    " MobizLibraryPIMProductVariableFeatures001 feat on pim.ID = feat.ProductID " &
                    " outer apply (select pim2.id, pim2.ProductNo from MobizLibraryPIMProduct pim2 inner join " &
                    " MobizLibraryPIMProductVariableFeatures001 feat2 on pim2.ID = feat2.ProductID " &
                    " where pim2.productgroupno = pim.productgroupno and feat2.C0017 = feat.C0017 and pim2.PublishedByID = '9DA344F3-DADB-4675-B723-15F0C591DA3C') as t" &
                    " where pim.PublishedByID = '118533EE-83D8-4405-8391-D5566489D888' and pim.ProductNo = '' " &
                    " and t.ProductNo <> '' and pim.ProductGroupNo is not null and feat.C0017 is not null" &
                    " order by t.ProductNo"

                pDT = mBusiness.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    For Each pDR As DataRow In pDT.Rows
                        'copy the product number so that it will no longer be blank
                        p("ProductNo") = ReadyString(pDR("ProductNo"))
                        p("LastUpdated") = "getdate()"

                        pSql = p.GetUpdate(" ID = " & ReadyString(pDR("nikID")))
                        mBusiness.ProData.ExecuteSQL(pSql)

                        'deactivate in private 
                        p2("IsDeactivated") = "1"
                        p2("DeactivationDate") = "getdate()"
                        p2("DeactivationReason") = ReadyString("Duplicate")
                        p2("LastUpdated") = "getdate()"

                        pSql2 = p2.GetUpdate(" ProductID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql2)

                        'deactivate duplicate in public
                        p3("IsActive") = "0"
                        p3("ProductNo") = ReadyString("")
                        p("LastUpdated") = "getdate()"
                        pSql3 = p3.GetUpdate("ID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql3)


                    Next

                End If


                'for size 2xl
                pSql = "select distinct pim.ID as nikID,t.ID as athID,t.ProductNo" &
                " from MobizLibraryPIMProduct pim inner join " &
                " MobizLibraryPIMProductVariableFeatures001 feat on pim.ID = feat.ProductID " &
                " outer apply (select pim2.id, pim2.ProductNo,feat2.C0017 from MobizLibraryPIMProduct pim2 inner join " &
                " MobizLibraryPIMProductVariableFeatures001 feat2 on pim2.ID = feat2.ProductID " &
                " where pim2.productgroupno = pim.productgroupno and feat2.C0017 = '2XL' and pim2.PublishedByID = '9DA344F3-DADB-4675-B723-15F0C591DA3C') as t" &
                " where pim.PublishedByID = '118533EE-83D8-4405-8391-D5566489D888' and pim.ProductNo = '' " &
                " and t.ProductNo <> '' and pim.ProductGroupNo is not null and feat.C0017 = 'XXL'" &
                " order by t.ProductNo"

                pDT = mBusiness.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    For Each pDR As DataRow In pDT.Rows
                        'copy the product number so that it will no longer be blank
                        p("ProductNo") = ReadyString(pDR("ProductNo"))
                        p("LastUpdated") = "getdate()"

                        pSql = p.GetUpdate(" ID = " & ReadyString(pDR("nikID")))
                        mBusiness.ProData.ExecuteSQL(pSql)

                        'deactivate in private 
                        p2("IsDeactivated") = "1"
                        p2("DeactivationDate") = "getdate()"
                        p2("DeactivationReason") = ReadyString("Duplicate")
                        p2("LastUpdated") = "getdate()"

                        pSql2 = p2.GetUpdate(" ProductID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql2)

                        'deactivate duplicate in public
                        p3("IsActive") = "0"
                        p3("ProductNo") = ReadyString("")
                        p("LastUpdated") = "getdate()"
                        pSql3 = p3.GetUpdate("ID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql3)


                    Next

                End If


                mBusiness.ProData.CommitTransaction()

            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
            End Try

        End Sub

        Sub RemoveDuplicateProductNo()
            Dim p As New InsertUpdate("MobizLibraryPIMProduct")
            Dim p2 As New InsertUpdate("LibraryOrganizationPIM")
            Dim p3 As New InsertUpdate("MobizLibraryPIMProduct")
            Dim pSql As String
            Dim pSql2 As String
            Dim pSql3 As String
            Dim pDT As DataTable


            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
            mBusiness.ProData.Execute("USE MobizNike")

            Try
                mBusiness.ProData.BeginTransaction()

                pSql = "select distinct pim.ID as nikID, t.ID as athID, t.ProductNo, pim.productgroupno, feat.C0017 " &
                    " from MobizLibraryPIMProduct pim inner join " &
                    " MobizLibraryPIMProductVariableFeatures001 feat on pim.ID = feat.ProductID " &
                    " outer apply (select pim2.id, pim2.ProductNo from MobizLibraryPIMProduct pim2 inner join " &
                    " MobizLibraryPIMProductVariableFeatures001 feat2 on pim2.ID = feat2.ProductID " &
                    " where pim2.productgroupno = pim.productgroupno and feat2.C0017 = feat.C0017 and pim2.PublishedByID = '9DA344F3-DADB-4675-B723-15F0C591DA3C') as t" &
                    " where pim.PublishedByID = '118533EE-83D8-4405-8391-D5566489D888' and pim.ProductNo <> '' AND pim.ProductNo is not null " &
                    " and t.ProductNo <> '' and pim.ProductGroupNo is not null and feat.C0017 is not null" &
                    " order by t.ProductNo"

                pDT = mBusiness.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    For Each pDR As DataRow In pDT.Rows
                        'deactivate in private 
                        p2("IsDeactivated") = "1"
                        p2("DeactivationDate") = "getdate()"
                        p2("DeactivationReason") = ReadyString("Duplicate")
                        p2("LastUpdated") = "getdate()"

                        pSql2 = p2.GetUpdate(" ProductID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql2)

                        'deactivate duplicate in public
                        p3("IsActive") = "0"
                        p3("ProductNo") = ReadyString("")
                        p("LastUpdated") = "getdate()"
                        pSql3 = p3.GetUpdate("ID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql3)


                    Next

                End If


                'for size 2xl
                pSql = "select distinct pim.ID as nikID,t.ID as athID,t.ProductNo" &
                " from MobizLibraryPIMProduct pim inner join " &
                " MobizLibraryPIMProductVariableFeatures001 feat on pim.ID = feat.ProductID " &
                " outer apply (select pim2.id, pim2.ProductNo,feat2.C0017 from MobizLibraryPIMProduct pim2 inner join " &
                " MobizLibraryPIMProductVariableFeatures001 feat2 on pim2.ID = feat2.ProductID " &
                " where pim2.productgroupno = pim.productgroupno and feat2.C0017 = '2XL' and pim2.PublishedByID = '9DA344F3-DADB-4675-B723-15F0C591DA3C') as t" &
                " where pim.PublishedByID = '118533EE-83D8-4405-8391-D5566489D888' and pim.ProductNo <> '' AND pim.ProductNo is not null " &
                " and t.ProductNo <> '' and pim.ProductGroupNo is not null and feat.C0017 = 'XXL'" &
                " order by t.ProductNo"

                pDT = mBusiness.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then
                    For Each pDR As DataRow In pDT.Rows
                        'deactivate in private 
                        p2("IsDeactivated") = "1"
                        p2("DeactivationDate") = "getdate()"
                        p2("DeactivationReason") = ReadyString("Duplicate")
                        p2("LastUpdated") = "getdate()"

                        pSql2 = p2.GetUpdate(" ProductID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql2)

                        'deactivate duplicate in public
                        p3("IsActive") = "0"
                        p3("ProductNo") = ReadyString("")
                        p("LastUpdated") = "getdate()"
                        pSql3 = p3.GetUpdate("ID = " & ReadyString(pDR("athID")))
                        mBusiness.ProData.ExecuteSQL(pSql3)


                    Next

                End If


                mBusiness.ProData.CommitTransaction()

            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
            End Try

        End Sub

        Sub SendDailyAlerts(OrgID As String, ByVal CompanyDBName As String)
            Dim dtGroup As DataTable
            Dim dtGroupHeads As DataTable
            Dim dtGroupFOWST As DataTable

            Dim dtGroupInventoryLevels As DataTable

            Dim dtFOWSTSales As DataTable
            Dim dtFOWSTSalesReturns As DataTable

            Dim FranchisorCentral As New FranchisorCentral
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            Dim EmailMessage As String
            Dim EmailAddresses As String


            dtGroup = FranchisorCentral.GetAllFranchisorGroup("")

            For i As Integer = 0 To dtGroup.Rows.Count - 1

                clsDashBoardsAndReports.OrgID = OrgID
                dtGroupInventoryLevels = clsDashBoardsAndReports.GroupInventoryLevels(CStr2(dtGroup.Rows(i).Item("ID")))

                FranchisorCentral.ID = CStr2(dtGroup.Rows(i).Item("ID"))

                dtGroupFOWST = FranchisorCentral.GetGroupFranchiseeFOWST()

                dtGroupHeads = FranchisorCentral.GetGroupHeads()

                If dtGroupHeads.Rows.Count > 0 Then
                    EmailAddresses = CStr2(dtGroupHeads.Rows(0).Item("CompanyEmailAddress"))

                    For EmailCount As Integer = 1 To dtGroupHeads.Rows.Count - 1
                        EmailAddresses &= "," & CStr2(dtGroupHeads.Rows(EmailCount).Item("CompanyEmailAddress"))
                    Next

                    ''''''''''''''''''''''''HTML'''''''''''''''''''''''''''''''''''

                    EmailMessage = "<div>Good day, " & vbCrLf &
                    "<br><br> " & vbCrLf &
                    "Here are the results of your group " & vbCrLf &
                    "<br><br> " & vbCrLf &
                    "Group Name: <span style=""margin-left:20px;"">" & CStr2(dtGroup.Rows(i).Item("GroupName")) & "</span><br> " & vbCrLf &
                    "Date: <span style=""margin-left:73px;""> " & FormatDateTime(DateAdd(DateInterval.Day, -1, Today), vbLongDate) & "</span> " & vbCrLf &
                    "</div> " & vbCrLf &
                    "<br> " & vbCrLf &
                    "<table style=""width:100%;"" border=""0"" cellpadding=""3"" cellspacing=""0""> " & vbCrLf &
                        "<thead> " & vbCrLf &
                            "<tr style=""background-color:#3498db; color:#FFF;""> " & vbCrLf &
                                "<th style=""padding:10px;"">FOWST</th> " & vbCrLf &
                                "<th style=""padding:10px;"">Gross Sales</th> " & vbCrLf &
                                "<th style=""padding:10px;"">Sales Return</th> " & vbCrLf &
                                "<th style=""padding:10px;"">Discounts</th> " & vbCrLf &
                                "<th style=""padding:10px;"">Net Sales</th> " & vbCrLf &
                                "<th style=""padding:10px;"">Inventory Value</th> " & vbCrLf &
                                "<th style=""padding:10px;"">Accumulated # Of Unapproved<br> Inventory Counts</th> " & vbCrLf &
                            "</tr> " & vbCrLf &
                        "</thead> " & vbCrLf &
                    "<tbody> "

                    '"<th style=""padding:10px;"">Shortage/Overage of <br> Monies for the Day</th> " & vbCrLf &

                    Dim TotalGrossSales As Double
                    Dim TotalSalesDiscounts As Double
                    Dim TotalSalesReturns As Double
                    Dim TotalNetSales As Double
                    Dim TotalSrpValue As Double
                    Dim TotalNoOfUnapprovedCounts As Integer

                    For j As Integer = 0 To dtGroupFOWST.Rows.Count - 1

                        clsDashBoardsAndReports.FOWSTID = CStr2(dtGroupFOWST.Rows(j).Item("ID"))
                        clsDashBoardsAndReports.DateFrom = DateAdd(DateInterval.Day, -1, Today)
                        clsDashBoardsAndReports.DateTo = DateAdd(DateInterval.Day, -1, Today)
                        clsDashBoardsAndReports.GroupBy = "Day"

                        dtFOWSTSales = clsDashBoardsAndReports.GetPOSSalesReportByTrendOrSalesDistribution()
                        dtFOWSTSalesReturns = clsDashBoardsAndReports.GetSalesReturnsData()

                        dtGroupInventoryLevels.DefaultView.RowFilter = "FOWSTID = " & ReadyString(CStr2(dtGroupFOWST.Rows(j).Item("ID")))

                        Dim GrossSales As Double
                        Dim SalesDiscounts As Double
                        Dim SalesReturns As Double
                        Dim NetSales As Double
                        Dim SrpValue As Double
                        Dim NoOfUnapprovedCounts As Integer

                        If dtFOWSTSales.Rows.Count > 0 Then
                            GrossSales = CDec2(dtFOWSTSales.Rows(0).Item("Total"))
                        Else
                            GrossSales = 0.0
                        End If

                        If dtFOWSTSales.Rows.Count > 0 Then
                            SalesDiscounts = CDec2(dtFOWSTSales.Rows(0).Item("DiscountTotal"))
                        Else
                            SalesDiscounts = 0.0
                        End If

                        If dtFOWSTSalesReturns.Rows.Count > 0 Then
                            SalesReturns = CDec2(dtFOWSTSalesReturns.Rows(0).Item("Total"))
                        Else
                            SalesReturns = 0.0
                        End If

                        NetSales = GrossSales - SalesDiscounts - SalesReturns

                        If dtGroupInventoryLevels.DefaultView.ToTable.Rows.Count > 0 Then
                            SrpValue = CDec2(dtGroupInventoryLevels.DefaultView.ToTable.Rows(0).Item("SrpValue"))
                            NoOfUnapprovedCounts = CInt2(dtGroupInventoryLevels.DefaultView.ToTable.Rows(0).Item("NoOfUnapprovedCounts"))
                        Else
                            SrpValue = 0.0
                            NoOfUnapprovedCounts = 0.0
                        End If

                        If CBool2(j Mod 2) Then

                            EmailMessage &= "<tr style=""background-color:#FFF;"">" & vbCrLf &
                                "<td>" & CStr2(dtGroupFOWST.Rows(j).Item("FowstName")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(GrossSales = 0, "-", Format(GrossSales, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(SalesDiscounts = 0, "-", Format(SalesDiscounts, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(SalesReturns = 0, "-", Format(SalesReturns, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(NetSales = 0, "-", Format(NetSales, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(SrpValue = 0, "-", Format(SrpValue, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(NoOfUnapprovedCounts = 0, "-", Format(NoOfUnapprovedCounts, "#,###")) & "</td>" & vbCrLf &
                            "</tr>"

                        Else
                            EmailMessage &= "<tr style=""background-color:#EEE;"">" & vbCrLf &
                                "<td>" & CStr2(dtGroupFOWST.Rows(j).Item("FowstName")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(GrossSales = 0, "-", Format(GrossSales, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(SalesDiscounts = 0, "-", Format(SalesDiscounts, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(SalesReturns = 0, "-", Format(SalesReturns, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(NetSales = 0, "-", Format(NetSales, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(SrpValue = 0, "-", Format(SrpValue, "#,###.##")) & "</td>" & vbCrLf &
                                "<td style=""text-align:right;"">" & IIf(NoOfUnapprovedCounts = 0, "-", Format(NoOfUnapprovedCounts, "#,###")) & "</td>" & vbCrLf &
                            "</tr>"

                        End If

                        TotalGrossSales += GrossSales
                        TotalSalesDiscounts += SalesDiscounts
                        TotalSalesReturns += SalesReturns
                        TotalNetSales += NetSales
                        TotalSrpValue += SrpValue
                        TotalNoOfUnapprovedCounts += NoOfUnapprovedCounts

                    Next

                    EmailMessage &= "<tr style=""background-color:#EEE;"">" & vbCrLf &
                                        "<td>GROUP TOTALS</td>" & vbCrLf &
                                        "<td style=""text-align:right;"">" & IIf(TotalGrossSales = 0, "-", Format(TotalGrossSales, "#,###.##")) & "</td>" & vbCrLf &
                                        "<td style=""text-align:right;"">" & IIf(TotalSalesDiscounts = 0, "-", Format(TotalSalesDiscounts, "#,###.##")) & "</td>" & vbCrLf &
                                        "<td style=""text-align:right;"">" & IIf(TotalSalesReturns = 0, "-", Format(TotalSalesReturns, "#,###.##")) & "</td>" & vbCrLf &
                                        "<td style=""text-align:right;"">" & IIf(TotalNetSales = 0, "-", Format(TotalNetSales, "#,###.##")) & "</td>" & vbCrLf &
                                        "<td style=""text-align:right;"">" & IIf(TotalSrpValue = 0, "-", Format(TotalSrpValue, "#,###.##")) & "</td>" & vbCrLf &
                                        "<td style=""text-align:right;"">" & IIf(TotalNoOfUnapprovedCounts = 0, "-", Format(TotalNoOfUnapprovedCounts, "#,###.##")) & "</td>" & vbCrLf &
                                    "</tr>"

                    EmailMessage &= "</tbody>" & vbCrLf &
                                 "</table> " & vbCrLf &
                                 "<br> " & vbCrLf &
                                 "<div> " & vbCrLf &
                                     "<label style=""font-weight:bold; font-size:16px;"">Note:</label> <br> " & vbCrLf &
                                     "To view more details of group performance reports, please go to:<br> " & vbCrLf &
                                     "<a href=""http://www.mobiz-apps.com"">www.mobiz-apps.com</a> " & vbCrLf &
                                     "<br> " & vbCrLf &
                                     "After signing in, please go to ""Dashboards and Reports """ & vbCrLf &
                                    "<br><br><br> " & vbCrLf &
                                    "All the best,<br> " & vbCrLf &
                                    "The Mobiz Team " & vbCrLf &
                                    "</div>"

                    ''''''''''''''''''''''END HTML'''''''''''''''''''''''''''''''''

#If DEBUG Then

                    Try
                        SendMail("customercare@mobiz-apps.com",
                        EmailAddresses,
                        "Mobiz",
                        "Daily Sales And Inventory Update for " & Format(DateAdd(DateInterval.Day, -1, Today), "MM/dd/yy"),
                        EmailMessage,
                        "smtp.live.com",
                        587,
                        "customercare@mobiz-apps.com",
                        "MobizCustCare2221", , , , ,
                        True)

                    Catch ex As Exception
                        AppendToDebug(ex.Message)
                    End Try


#Else
                    Try
                        SendMail("customercare@mobiz-apps.com",
                        EmailAddresses,
                        "Mobiz",
                        "Daily Sales And Inventory Update for " & Format(DateAdd(DateInterval.Day, -1, Today), "MM/dd/yy"),
                        EmailMessage,
                        "smtp.live.com",
                        587,
                        "customercare@mobiz-apps.com",
                        "MobizCustCare2221", , , , ,
                        True)

                    Catch ex As Exception
                        AppendToDebug(ex.Message)
                    End Try
#End If
                End If
            Next
        End Sub

        Sub StartSendingAuditTrailReport()
            Dim dtSubscriptionDatabase As DataTable
            Dim triggers As New Triggers(Common.WebRequest.ProData)

            dtSubscriptionDatabase = GetAllSubscriptionDatabase()

            For i As Integer = 0 To dtSubscriptionDatabase.Rows.Count - 1

                If (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("ABC")) Then

                    triggers.ConnectToDatabase(CStr(dtSubscriptionDatabase.Rows(i).Item("Database")))

                    triggers.SendAuditTrailReport(CStr2(dtSubscriptionDatabase.Rows(i).Item("OrganizationID")))
                    Exit For
                End If
            Next

        End Sub

        Function OneTimeQuery() As String
            Dim triggers As New Triggers
            Dim pProData As ProData

            pProData = New ProData()
            pProData.FailedConnectRetryDuration = 60 * 1000
            pProData.Connect(Common.ConnectionInfo)

            triggers = New Triggers(pProData)

#If DEBUG Then
            triggers.ConnectToDatabase("MobizABC")
            Return triggers.OneTimeQuery("MobizABC")
#Else
            triggers.ConnectToDatabase("MobizCinco_Corporation")
            Return triggers.OneTimeQuery("MobizCinco_Corporation")
#End If

        End Function

        Function TagTransactionNo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDTDeviceFowst As DataTable
            Dim pSql As String
            Dim result As String = ""
            Dim maxstore As Integer = Request.Params.Get("Max")
            Dim totalrows As Integer = 0
#If DEBUG Then
            'Common.WebRequest.ProData.Execute("USE MobizABC")
#Else
            'Common.WebRequest.ProData.Execute("USE MobizCinco_Corporation")
#End If
            'Common.WebRequest.ProData.CommandTimeout = 600000
            pSql = " SELECT * FROM (Select LibraryDeviceFOWST.ID, Code, Name, POSLastORNo, DeviceFowstTransactionNumber, DeviceID, FowstID " & vbCrLf &
                    " , (SELECT TOP 1 ID FROM TransInvoice WHERE TransInvoice.FowstID = LibraryDeviceFOWST.FowstID AND TransInvoice.DeviceID = LibraryDeviceFOWST.DeviceID) as InvoiceID " & vbCrLf &
                    " from LibraryDeviceFOWST INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = FowstID " & vbCrLf &
                    " WHERE ISNULL(DeviceFowstTransactionNumber,0) = 0 AND POSLastORNo > 0 " & IIf(ReadyGuid(WebRequest.Instance.OrgID()) = ReadyGuid("d362ffdb-15d1-451f-97dc-c567b94fb410"), " AND Code NOT LIKE 'PCF%' AND Code NOT LIKE 'CEB%' ", "") & vbCrLf &
                    " AND MobizLibraryOrganizationFOWST.IsActive = 1 " & vbCrLf &
                    " ) as t WHERE InvoiceID IS NOT NULL order by POSLastORNo " & vbCrLf

            pDTDeviceFowst = Common.WebRequest.ProData.Execute(pSql)
            result += "Devices without transaction number: " & pDTDeviceFowst.Rows.Count & "<br>"
            result += "Start time: " & Format(Now, "hh:mm tt") & "<br><br>"
            maxstore = IIf(pDTDeviceFowst.Rows.Count > maxstore, maxstore - 1, pDTDeviceFowst.Rows.Count - 1)
            For i As Integer = 0 To maxstore ' pDTDeviceFowst.Rows.Count - 1
                Dim FowstID As String
                Dim DeviceID As String
                Dim TransactionNo As Integer = 1
                Dim pDTTransactions As DataTable

                'check if transaction no is greater than 0

                FowstID = CStr2(pDTDeviceFowst.Rows(i).Item("FowstID"))
                DeviceID = CStr2(pDTDeviceFowst.Rows(i).Item("DeviceID"))

                result += CStr2(pDTDeviceFowst.Rows(i).Item("Code")) & " - " & CStr2(pDTDeviceFowst.Rows(i).Item("Name"))

                pSql = "(Select distinct TransInvoice.ID, TransactionType, TransactionDate, TransInvoice.DeviceID  from TransInventoryLedger " & vbCrLf &
                        " inner join TransInvoice on TransInvoice.ID = TransInventoryLedger.TransactionID  " & vbCrLf &
                        " where TransInvoice.fowstid = " & ReadyString(FowstID) & " and DeviceID = " & ReadyString(DeviceID) & vbCrLf &
                        " ) " & vbCrLf &
                        " UNION " & vbCrLf &
                        " ( " & vbCrLf &
                        " Select distinct TransMemo.ID, TransactionType, TransactionDate, TransMemo.DeviceID  from TransInventoryLedger " & vbCrLf &
                        " inner join TransMemo on TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                        " where TransMemo.fowstid = " & ReadyString(FowstID) & " and DeviceID = " & ReadyString(DeviceID) & vbCrLf &
                        " ) " & vbCrLf &
                        " Order by TransactionDate " & vbCrLf

                pDTTransactions = Common.WebRequest.ProData.Execute(pSql)

                Try

                    'Common.WebRequest.ProData.BeginTransaction()

                    For TransCount As Integer = 0 To pDTTransactions.Rows.Count - 1

                        Dim TransType As String = CStr2(CStr2(pDTTransactions.Rows(TransCount).Item("TransactionType")))

                        If TransType = "Invoice" Then

                            pSql = "Update TransInvoice set DeviceFowstTransactionNumber = " & TransactionNo & " WHERE ID = " & ReadyString(CStr2(CStr2(pDTTransactions.Rows(TransCount).Item("ID"))))
                            Common.WebRequest.ProData.Execute(pSql)

                        Else

                            pSql = "Update TransSalesReturnLedger set DeviceFowstTransactionNumber = " & TransactionNo & " WHERE SourceMemoID = " & ReadyString(CStr2(CStr2(pDTTransactions.Rows(TransCount).Item("ID"))))
                            Common.WebRequest.ProData.Execute(pSql)

                        End If

                        TransactionNo += 1

                    Next
                    result += ": " & (TransactionNo - 1) & "<br>"
                    totalrows += (TransactionNo - 1)

                    pSql = "Update LibraryDeviceFOWST set DeviceFowstTransactionNumber = " & (TransactionNo - 1) & " WHERE ID = " & ReadyString(CStr2(pDTDeviceFowst.Rows(i).Item("ID")))
                    Common.WebRequest.ProData.Execute(pSql)

                    'Common.WebRequest.ProData.CommitTransaction()

                Catch ex As Exception
                    result += "<br>" & ex.Message
                    'Common.WebRequest.ProData.RollbackTransaction()

                End Try
            Next
            result += "<br>End time: " & Format(Now, "hh:mm tt") & ""
            result += "<br>TOTAL ROWS AFFECTED: " & totalrows & "<br>"
            Return result
        End Function

    End Class
End Namespace