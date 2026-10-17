Imports Aspose.Cells
Imports Renci.SshNet.Sftp
Imports Stimulsoft.Report
Imports Stimulsoft.Report.Components
Imports System.Data.SqlTypes
Imports System.IO

Public Class ReportPOS

    Function getReportHeader(ByVal mProdata As ProData, ByVal strSubscriberFowstID As String, ByVal strUserID As String, Optional vDeviceID As String = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = mProdata

            With .Fields
                With .Add("SubscriberOrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "subcrbr.OrganizationID"
                    .Searchable = False
                End With
                With .Add("SubscriberOrganizationName", SqlDbType.VarChar)
                    .Expression = "ISNULL((SELECT BusinessRegisteredName FROM LibraryDevice WHERE ID = devfowst.DeviceID), subcr.CompanyName)"
                End With
                With .Add("OrganizationBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetfowst.FowstID"
                    .Searchable = False
                End With
                With .Add("OrgBranchName", SqlDbType.VarChar)
                    .Expression = "subcrbr.[Name]"
                End With
                With .Add("OrgCompleteAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(subcrbr.StreetNumber)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(subcrbr.Street)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(subcrcty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(subcrctry.Name)) + ', ',', '), '')"
                End With
                With .Add("OrgTelNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationFOWSTTelNos(rsetfowst.FowstID)"
                End With
                If Not String.IsNullOrWhiteSpace(strUserID) Then
                    With .Add("PrintedBy", SqlDbType.VarChar)
                        .Expression = "ISNULL((SELECT TOP 1 ISNULL(NULLIF(LTRIM(RTRIM(Surname)) + ', ',', '), '') + " &
                                      "        ISNULL(NULLIF(LTRIM(RTRIM(FirstName)) + ' ',' '), '') +  " &
                                      "        ISNULL(NULLIF(LTRIM(RTRIM(MiddleName)) + ' ',' '), '') " &
                                      " FROM MobizLibraryPeople " &
                                      " WHERE ID = " & ReadyString(strUserID) & ") " &
                                      ",'')"
                    End With
                End If
                With .Add("PrintDateTime", SqlDbType.DateTime)
                    .Expression = "getdate()"
                End With
                With .Add("IsVAT", SqlDbType.VarChar)
                    .Expression = "ISNULL(subcrbr.IsVAT, 1)"
                End With
                If HttpContext.Current IsNot Nothing Then
                    With .Add("POSReportType", SqlDbType.Int)
                        .Expression = WebSession.Instance.POSSalesReportType
                    End With
                End If
                If String.IsNullOrWhiteSpace(vDeviceID) = False Then
                    .Add("IsAuthorizedForPOS", SqlDbType.VarChar, "devfowst.IsAuthorizedForPOS")
                    With .Add("UuidDisplay", SqlDbType.VarChar)
                        .Expression = "ISNULL((SELECT UuidDisplay FROM LibraryDevice WHERE ID = devfowst.DeviceID), '')"
                    End With
                    With .Add("POSRegistrationNo", SqlDbType.VarChar)
                        .Expression = "devfowst.POSRegistrationNo"
                    End With
                    With .Add("POSPermitNo", SqlDbType.VarChar)
                        .Expression = "devfowst.POSRegistrationNo"
                    End With
                    With .Add("PTUDateIssued", SqlDbType.VarChar)
                        .Expression = "ISNULL(CONVERT(varchar(max), devfowst.PTUDateIssued, 107), '')"
                    End With
                    With .Add("MIN", SqlDbType.VarChar)
                        .Expression = "devfowst.BIRMachineNo"
                    End With
                    With .Add("OrgBusinessTradeName", SqlDbType.VarChar)
                        .Expression = "ISNULL((SELECT BusinessName FROM LibraryDevice WHERE ID = devfowst.DeviceID), rsettrdname.BusinessTradeName)"
                    End With
                    With .Add("OrgTIN", SqlDbType.VarChar)
                        .Expression = "ISNULL((SELECT TinDisplay FROM LibraryDevice WHERE ID = devfowst.DeviceID), subcr.TinNumber + '-00' + RIGHT(subcrbr.TINBranchCode, 3))"
                    End With
                    With .Add("ZCount", SqlDbType.VarChar)
                        .Expression = "devfowst.ZReadCount"
                    End With
                    With .Add("MobizAppVersion", SqlDbType.VarChar)
                        .Expression = "ISNULL(dev.MobizAppVersion, 1)"
                    End With
                Else
                    With .Add("OrgBusinessTradeName", SqlDbType.VarChar)
                        .Expression = "rsettrdname.BusinessTradeName"
                    End With
                    With .Add("OrgTIN", SqlDbType.VarChar)
                        .Expression = "subcr.TinNumber + '-00' + RIGHT(subcrbr.TINBranchCode, 3)"
                    End With
                End If
            End With

            .SqlFrom = " FROM dbo.MobizLibraryOrganizationFOWST subcrbr  " & vbCrLf &
                    " INNER JOIN dbo.MobizLibraryOrganization subcr ON (subcrbr.OrganizationID = subcr.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCities subcrcty ON (subcrbr.FowstCityID = subcrcty.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCountries subcrctry ON (subcrbr.FowstCountryID = subcrctry.ID) " & vbCrLf &
                    " LEFT JOIN dbo.LibraryReceiptSettingsFowst rsetfowst ON (rsetfowst.FowstID = subcrbr.ID)" & vbCrLf &
                    " LEFT JOIN dbo.LibraryReceiptSettings rset ON (rsetfowst.ReceiptSettingsID = rset.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID) " & vbCrLf
            If String.IsNullOrWhiteSpace(vDeviceID) = False Then
                .SqlFrom &= " LEFT JOIN LibraryDeviceFowst devfowst ON rsetfowst.FowstID = devfowst.FOWSTID AND devfowst.DeviceID = " + ReadyString(vDeviceID) & vbCrLf &
                    " LEFT JOIN LibraryDevice dev ON devfowst.DeviceID = dev.ID" & vbCrLf
            End If

            .SqlWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE subcrbr.ID IS NULL", " WHERE subcrbr.ID = " + ReadyString(strSubscriberFowstID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getCashierTurnOverAndDeposits(ByVal strCashierID As String,
                                           ByVal strRangeType As String,
                                           ByVal strRangeFrom As String,
                                           ByVal strRangeTo As String,
                                           ByVal strRangeYear As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strCustInvDateRange As String
        Dim strCustPayDateRange As String
        Dim strMemoDateRange As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String


        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        strCustInvDateRange = ""
        strCustPayDateRange = ""
        strMemoDateRange = ""

        If (strRangeType = "Today") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today, True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today, False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Yesterday") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today.AddDays(-1), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Day") Then

            strRangeFrom = getFOWSTOperationHR(Convert.ToDateTime(strRangeFrom), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Convert.ToDateTime(strRangeTo), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Week") Then

            strCustInvDateRange = " DATEPART(week,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(week,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Month") Then

            strCustInvDateRange = " DATEPART(month,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(month,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Quarter") Then

            strCustInvDateRange = " DATEPART(q,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(q,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Year") Then

            strCustInvDateRange = " DATEPART(year,inv.InvoiceDate) = " & strRangeYear

            strCustPayDateRange = " DATEPART(year,custpay.PaymentDate) = " & strRangeYear

            strMemoDateRange = " DATEPART(year,memo.LastUpdated) = " & strRangeYear

        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields

                With .Add("IsTurnOver", SqlDbType.Bit)
                    .Expression = "t.IsTurnOver"
                End With

                With .Add("DepositedTo", SqlDbType.VarChar)
                    .Expression = "t.DepositedTo"
                End With

                'With .Add("DepositedTo", SqlDbType.Bit)
                '    .Expression = "ISNULL(ppl.FirstName + ' ' + ppl.SurName, org.CompanyName)"
                'End With

                With .Add("TotalAmount", SqlDbType.Float)
                    .Expression = "t.TotalAmount"
                End With

            End With

            .SqlFrom = "from TransCashHandling " & vbCrLf &
                        "outer apply( " & vbCrLf &
                            "Select IsTurnOver, TotalAmount, ISNULL(DepositedToByName,'') + ' of ' + CompanyName  DepositedTo  " & vbCrLf &
                            "from MobizlibraryOrganization  " & vbCrLf &
                            "inner join TransCashHandling on MobizlibraryOrganization.ID = TransCashHandling.DepositedTo " & vbCrLf &
                            " WHERE DepositedBy = " & ReadyString(strCashierID) & vbCrLf &
                            " AND convert(date,DepositDate) between " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf &
                            "UNION " & vbCrLf &
                            "Select IsTurnOver, TotalAmount, SurName + ', ' + FirstName DepositedTo  " & vbCrLf &
                            "from MobizlibraryPeople  " & vbCrLf &
                            "inner join TransCashHandling on MobizlibraryPeople.ID = TransCashHandling.DepositedTo " & vbCrLf &
                            " WHERE DepositedBy = " & ReadyString(strCashierID) & vbCrLf &
                            " AND convert(date,DepositDate) between " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf &
                        ")t"

            '.SqlFrom = "FROM TransCashHandling " & vbCrLf &
            '            " LEFT JOIN MobizLibraryPeople ppl " & vbCrLf &
            '            " ON ppl.ID = DepositedTo " & vbCrLf &
            '            " LEFT JOIN MobizLibraryOrganization org " & vbCrLf &
            '            " ON org.ID = DepositedTo "

            '.SqlGroup = "Group By IsTurnOver, t.DepositedTo"

            '.SqlWhere = "WHERE 

            pDT = .Requery

        End With

        Return pDT


    End Function

    Function getCashierSalesTransactionSummary(ByVal strCashierID As String,
                                       ByVal strRangeType As String,
                                       ByVal strRangeFrom As String,
                                       ByVal strRangeTo As String,
                                       ByVal strRangeYear As String,
                                       Optional ByVal strFowstID As String = "",
                                       Optional ByVal strDeviceID As String = "") As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strCustInvDateRange As String
        Dim strCustPayDateRange As String
        Dim strMemoDateRange As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        If String.IsNullOrWhiteSpace(strFowstID) Then
            strFowstID = WebRequest.Instance.FowstID
        End If

        If String.IsNullOrWhiteSpace(strDeviceID) Then
            strDeviceID = Common.WebRequest.Data("DeviceID")
        End If

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        strCustInvDateRange = ""
        strCustPayDateRange = ""
        strMemoDateRange = ""

        If (strRangeType = "Today") Then

            strRangeFrom = Convert.ToDateTime(strRangeFrom).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = Convert.ToDateTime(strRangeTo).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Yesterday") Then

            strRangeFrom = Convert.ToDateTime(strRangeFrom).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = Convert.ToDateTime(strRangeTo).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Day") Then

            strRangeFrom = Convert.ToDateTime(strRangeFrom).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = Convert.ToDateTime(strRangeTo).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Week") Then

            strCustInvDateRange = " DATEPART(week,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(week,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Month") Then

            strCustInvDateRange = " DATEPART(month,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(month,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Quarter") Then

            strCustInvDateRange = " DATEPART(q,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(q,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Year") Then

            strCustInvDateRange = " DATEPART(year,inv.InvoiceDate) = " & strRangeYear

            strCustPayDateRange = " DATEPART(year,custpay.PaymentDate) = " & strRangeYear

            strMemoDateRange = " DATEPART(year,memo.LastUpdated) = " & strRangeYear

        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .ProData.CommandTimeout = 60000
            With .Fields
                With .Add("CashierID", SqlDbType.VarChar)
                    .Expression = ReadyString(strCashierID)
                End With
                With .Add("CashierName", SqlDbType.VarChar)
                    .Expression = "ISNULL((SELECT ISNULL(NULLIF(LTRIM(RTRIM(cshr.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') " &
                                  " FROM dbo.MobizLibraryPeople cshr " &
                                  " WHERE cshr.ID = " & ReadyString(strCashierID) & "),'')"
                End With
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(invitem.GrossSalesNetOfVATExemptions, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                    '         " AND invitem.TaxAmount > 0 " &  commented by janine feb 9 2015, because senior citizen discount is vat exempt
                End With
                With .Add("CurSalesDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(custpay.DiscountAmount, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " &
                               " AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df') -- Senior, PWD " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND disc.NoOfSeniorCitizens IS NULL " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                    '         " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015, because senior citizen discount is vat exempt
                End With
                With .Add("CurSoloParentDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.SoloParentDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND (LibraryDiscountSettings.DiscountType = 'Solo Parent Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurPhNatTeamDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(inv.PhNatTeamDiscount, 2),0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND (LibraryDiscountSettings.DiscountType = 'Philippine National Sports Team Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurSCDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.SCDiscount, 2))" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                    '         " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015, because senior citizen discount is vat exempt
                End With
                With .Add("CurSCDiscountExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.VAT, 2)) + SUM(ROUND(Vatable, 2)) " &
                               " FROM TransInvoice inv " &
                               " WHERE inv.InvoiceNumber > 0 AND inv.SCDiscount > 0" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND inv.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.ForegoneVat, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                    '         " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015, because senior citizen discount is vat exempt
                End With
                With .Add("CurSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(srledger.GrossRefundAmount, 2)) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND memo.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(srledger.GrossSrvAmount, 2)) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND memo.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurTotalVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT ROUND(SUM(ISNULL(invitem.TaxSales,0) - ISNULL(invitem.DiscountSales,0)), 2) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT ROUND(SUM(ISNULL(invitem.TaxAmount,0)- ISNULL(invitem.DiscountTax,0)), 2) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurTotalVatablePortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.Vatable, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurTotalVatAmountPortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(ROUND(inv.VAT, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0 " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT ROUND(SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)), 2) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " INNER JOIN TransInvoice as inv ON inv.ID = memosr.InvoiceID " & vbCrLf &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 " &
                               " AND memo.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(memosr.TaxAmount, 2)) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " INNER JOIN TransInvoice as inv ON inv.ID = memosr.InvoiceID " & vbCrLf &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND ISNULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 " &
                               " AND memo.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ROUND(invitem.NonVATSale, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurVATExemptSales", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - invitem.DiscountAmount) " &
                    .Expression = "ISNULL((SELECT SUM(ROUND(inv.VATExempt, 2)) " &
                           " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                           " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                           " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                           " WHERE (inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' ) " &
                           " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                           " AND inv.FOWSTID = devf.FowstID " &
                           " AND inv.DeviceID = devf.DeviceID " &
                           " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                           ",0)" & vbCrLf &
                           " - ISNULL((SELECT SUM(ROUND((items.UnitPrice * memosr.QuantityReturned) / 1.12, 2)) " & vbCrLf &
                            " FROM TransMemoSalesReturn memosr " & vbCrLf &
                            " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice as inv ON inv.ID = memosr.InvoiceID " & vbCrLf &
                            " INNER JOIN TransInvoiceItems as items ON items.InvoiceID = inv.ID AND items.ProductID = memosr.ProductID " & vbCrLf &
                            " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                            " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                            " AND memo.UserID = " & ReadyString(strCashierID) & vbCrLf &
                            " And memo.DeviceID = devf.DeviceID And (ISNULL(inv.SCDiscount, 0) > 0 Or ISNULL(inv.SoloParentDiscount, 0) > 0)) " & vbCrLf &
                            ",0)" & vbCrLf
                    'AND invitem.TaxAmount = 0 AND ISNULL(invitem.VATableSale,0) = 0 AND 
                End With
                With .Add("CurZeroRatedVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ROUND(invitem.ZeroRatedSale, 2)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem On (invitem.InvoiceID = inv.ID) " &
                               " WHERE " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " And inv.FOWSTID = devf.FowstID " &
                               " And inv.DeviceID = devf.DeviceID " &
                               " And custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With

                With .Add("CurOthers", SqlDbType.Money)
                    .Expression = "0"
                End With

                With .Add("CashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " CONVERT(datetime, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " And inv.FOWSTID = devf.FowstID " &
                               " And inv.DeviceID = devf.DeviceID " &
                               " And custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " CONVERT(datetime, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " And inv.FOWSTID = devf.FowstID " &
                               " And inv.DeviceID = devf.DeviceID " &
                               " And custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " CONVERT(datetime, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " And inv.FOWSTID = devf.FowstID " &
                               " And inv.DeviceID = devf.DeviceID " &
                               " And custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " CONVERT(datetime, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " And inv.FOWSTID = devf.FowstID " &
                               " And inv.DeviceID = devf.DeviceID " &
                               " And custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ISNULL(custpay.SRVAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " CONVERT(datetime, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " And inv.FOWSTID = devf.FowstID " &
                               " And inv.DeviceID = devf.DeviceID " &
                               " And custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("OnCreditAmount", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ISNULL(custpay.OnCreditAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " CONVERT(datetime, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " And inv.FOWSTID = devf.FowstID " &
                               " And inv.DeviceID = devf.DeviceID " &
                               " And custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("GCashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((Select SUM(ISNULL(other.Amount,0)) " &
                             " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay On (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other On (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv On (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'GCash' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("OtherTypeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                                 " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("PotatoCornerGC", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                            " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                            " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                            " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                            " AND inv.DeviceID = devf.DeviceID " & vbCrLf &
                            " AND other.PaymentType = 'PotatoCornerGC' " & vbCrLf &
                            " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                            ",0)"
                End With
                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ROUND(srledger.Fees, 2), 0) + ISNULL(ROUND(srledger.RefundFees, 2), 0)) " & vbCrLf &
                                " FROM TransSalesReturnLedger srledger " & vbCrLf &
                                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                                " WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf &
                                " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                                " AND memo.DeviceID = devf.DeviceID " & vbCrLf &
                                " AND memo.UserID = " & ReadyString(strCashierID) & ") " &
                                ",0)" & vbCrLf
                End With
                With .Add("SMACAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'SMAC' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("EPlusAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'E-Plus' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("VoucherAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = " & ReadyString(strCashierID) & ") " &
                               ",0)"
                End With
                With .Add("CurPotatoCornerGC", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " & vbCrLf &
                            " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                            " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " & vbCrLf &
                            " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                            " WHERE CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf &
                            " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                            " AND inv.DeviceID = devf.DeviceID " & vbCrLf &
                            " AND other.PaymentType = 'PotatoCornerGC' " & vbCrLf &
                            " AND custpay.UserID = " & ReadyString(strCashierID) & vbCrLf &
                            "),0)"
                End With
                With .Add("GiftCertAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("LoyaltyCardAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CreditTermsAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("SalesDate", SqlDbType.Date)
                    .Expression = "'" & IIf(CDate(strRangeFrom).ToString("MM/dd/yyyy") = CDate(strRangeTo).ToString("MM/dd/yyyy"), CDate(strRangeFrom).ToString("MM/dd/yyyy"), CDate(strRangeFrom).ToString("MM/dd/yyyy") & " - " & CDate(strRangeTo).ToString("MM/dd/yyyy")) & "'"
                    '.Expression = IIf(strRangeType = "Today", ReadyString(Date.Today.ToString("MM/dd/yyyy")), ReadyString(Date.Today.AddDays(-1).ToString("MM/dd/yyyy")))
                End With
                With .Add("TransDate", SqlDbType.Date)
                    .Expression = "'" & IIf(CDate(strRangeFrom).ToString("MM/dd/yyyy") = CDate(strRangeTo).ToString("MM/dd/yyyy"), CDate(strRangeFrom).ToString("MM/dd/yyyy"), CDate(strRangeFrom).ToString("MM/dd/yyyy") & " - " & CDate(strRangeTo).ToString("MM/dd/yyyy")) & "'"
                    '.Expression = IIf(strRangeType = "Today", ReadyString(Date.Today.ToString("MM/dd/yyyy")), ReadyString(Date.Today.AddDays(-1).ToString("MM/dd/yyyy")))
                End With
                With .Add("CSRPrintCount", SqlDbType.Date)
                    .Expression = "devf.XReadCount"
                End With
                With .Add("ZeroRatedVAT", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CurVoid", SqlDbType.Money)
                    .Expression = "ISNULL(tblVoid.Sum, 0)"
                End With
                With .Add("CurVoidItemQty", SqlDbType.Int)
                    .Expression = "ISNULL(tblVoid.Count, 0)"
                End With
                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "getdate()"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf "

            .SqlFrom &= " OUTER APPLY (" & vbCrLf &
                " 	SELECT SUM(TransInvoiceItemsVoided.Quantity * TransInvoiceItemsVoided.UnitPrice) AS Sum" & vbCrLf &
                "   	,CONVERT(int, SUM(TransInvoiceItemsVoided.Quantity)) AS Count" & vbCrLf &
                "  	FROM TransInvoiceItemsVoided" & vbCrLf &
                "  	WHERE CONVERT(datetime, TransInvoiceItemsVoided.DateTimeRemoved) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf &
                "  	AND TransInvoiceItemsVoided.FOWSTID = devf.FowstID" & vbCrLf &
                "  	AND TransInvoiceItemsVoided.DeviceID = devf.DeviceID" & vbCrLf &
                "   AND TransInvoiceItemsVoided.CashierID = " & ReadyString(strCashierID) & vbCrLf &
                " ) tblVoid" & vbCrLf

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(strFowstID) & vbCrLf &
                        " AND devf.DeviceID = " & ReadyString(strDeviceID)

            pDT = .Requery

        End With

        Dim megaworld As New MallLessorMegaworld

        Dim CashRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", strRangeFrom, strRangeTo, strFowstID, strDeviceID, "cash", strCashierID)))
        Dim OtherPaymentRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", strRangeFrom, strRangeTo, strFowstID, strDeviceID, "otherpayment", strCashierID)))
        Dim DiscountRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", strRangeFrom, strRangeTo, strFowstID, strDeviceID, "discount", strCashierID)))
        Dim DebitRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", strRangeFrom, strRangeTo, strFowstID, strDeviceID, "debit", strCashierID)))
        Dim CreditRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", strRangeFrom, strRangeTo, strFowstID, strDeviceID, "credit", strCashierID)))

        pDT.Rows(0)("CashAmount") = CDec2(pDT.Rows(0)("CashAmount")) - Format(CashRefund, "0.00")
        pDT.Rows(0)("DebitCardAmount") = CDec2(pDT.Rows(0)("DebitCardAmount")) - Format(DebitRefund, "0.00")
        pDT.Rows(0)("CreditCardAmount") = CDec2(pDT.Rows(0)("CreditCardAmount")) - Format(CreditRefund, "0.00")

        Return pDT

    End Function

    Function getManagerSalesReportPaymentInfoByCashier(ByVal strRangeType As String,
                                          ByVal strRangeFrom As String,
                                          ByVal strRangeTo As String,
                                          ByVal strRangeYear As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtaView As DataView
        Dim pSQL As String
        Dim strCustInvDateRange As String
        Dim strCustPayDateRange As String
        Dim strMemoDateRange As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        strCustInvDateRange = ""
        strCustPayDateRange = ""
        strMemoDateRange = ""

        If (strRangeType = "Today") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today, True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today, False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Yesterday") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today.AddDays(-1), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Day") Then

            strRangeFrom = getFOWSTOperationHR(Convert.ToDateTime(strRangeFrom), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Convert.ToDateTime(strRangeTo), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Week") Then

            strCustInvDateRange = " DATEPART(week,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(week,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Month") Then

            strCustInvDateRange = " DATEPART(month,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(month,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Quarter") Then

            strCustInvDateRange = " DATEPART(q,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(q,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Year") Then

            strCustInvDateRange = " DATEPART(year,inv.InvoiceDate) = " & strRangeYear

            strCustPayDateRange = " DATEPART(year,custpay.PaymentDate) = " & strRangeYear

            strMemoDateRange = " DATEPART(year,memo.LastUpdated) = " & strRangeYear

        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("CashierName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cshr.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') "
                End With
                With .Add("TotalSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - ISNULL(invitem.DiscountAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(inv.SCDiscount)" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND memo.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND memo.UserID = orgemp.PeopleID) " &
                               ",0) + " &
                               "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                 " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                               ",0) "
                    ' " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf, dbo.LibraryOrganizationEmployee orgemp " &
                       " INNER JOIN dbo.MobizLibraryPeople cshr ON (orgemp.PeopleID = cshr.ID) "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(WebRequest.Instance.FowstID)

            pSQL = "SELECT CashierName, " &
                   "     SUM(TotalSales) as TotalSales " &
                   " FROM (" & pFinder.CreateSql() & ") fin " &
                   " WHERE TotalSales <> 0 " &
                   " GROUP BY CashierName"

            pDT = Common.WebRequest.ProData.Execute(pSQL)

            'pDT = .Requery

            'dtaView = New DataView(pDT)
            'dtaView.RowFilter = " TotalSales <> 0"

        End With

        'Return dtaView.ToTable()
        Return pDT

    End Function

    Function getManagerSalesReportPaymentInfoByMachine(ByVal strRangeType As String,
                                          ByVal strRangeFrom As String,
                                          ByVal strRangeTo As String,
                                          ByVal strRangeYear As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtaView As DataView
        Dim strCustInvDateRange As String
        Dim strCustPayDateRange As String
        Dim strMemoDateRange As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        strCustInvDateRange = ""
        strCustPayDateRange = ""
        strMemoDateRange = ""

        If (strRangeType = "Today") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today, True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today, False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf

        ElseIf (strRangeType = "Yesterday") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today.AddDays(-1), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf

        ElseIf (strRangeType = "Day") Then

            strRangeFrom = getFOWSTOperationHR(Convert.ToDateTime(strRangeFrom), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Convert.ToDateTime(strRangeTo), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) & vbCrLf

        ElseIf (strRangeType = "Week") Then

            strCustInvDateRange = " DATEPART(week,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

            strCustPayDateRange = " DATEPART(week,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

            strMemoDateRange = " DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

        ElseIf (strRangeType = "Month") Then

            strCustInvDateRange = " DATEPART(month,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

            strCustPayDateRange = " DATEPART(month,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

            strMemoDateRange = " DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

        ElseIf (strRangeType = "Quarter") Then

            strCustInvDateRange = " DATEPART(q,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

            strCustPayDateRange = " DATEPART(q,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

            strMemoDateRange = " DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo & vbCrLf &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo & vbCrLf

        ElseIf (strRangeType = "Year") Then

            strCustInvDateRange = " DATEPART(year,inv.InvoiceDate) = " & strRangeYear & vbCrLf

            strCustPayDateRange = " DATEPART(year,custpay.PaymentDate) = " & strRangeYear & vbCrLf

            strMemoDateRange = " DATEPART(year,memo.LastUpdated) = " & strRangeYear & vbCrLf

        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = "LTRIM(RTRIM(CAST(fowst.FOWSTNo as CHAR))) +'-'+ LTRIM(RTRIM(CAST(dev.DeviceNo as CHAR)))"
                End With
                With .Add("TotalSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - ISNULL(invitem.DiscountAmount,0)) " & vbCrLf &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) - " & vbCrLf &
                               " ISNULL((SELECT SUM(srledger.GrossRefundAmount) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) - " & vbCrLf &
                               " ISNULL((SELECT SUM(inv.SCDiscount)" & vbCrLf &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) - " & vbCrLf &
                               " ISNULL((SELECT SUM(inv.ForegoneVAT) " & vbCrLf &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) - " & vbCrLf &
                               " ISNULL((SELECT SUM(srledger.GrossSrvAmount) " & vbCrLf &
                               " FROM TransSalesReturnLedger srledger " & vbCrLf &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " & vbCrLf &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
                               " AND memo.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND memo.DeviceID = devf.DeviceID) " & vbCrLf &
                                ",0) + " & vbCrLf &
                               "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " & vbCrLf &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                                " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " & vbCrLf &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0) " & vbCrLf
                    '  " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf " & vbCrLf &
                       " INNER JOIN dbo.LibraryDevice dev ON (devf.DeviceID = dev.ID) " & vbCrLf &
                       " LEFT JOIN dbo.MobizLibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) " & vbCrLf

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(WebRequest.Instance.FowstID) & vbCrLf

            Dim pSql As String
            If Now.Date = #9/28/2015# Then
                pSql = .CreateSql
                AppendToLog("vbML4F3Z6KCK! " & pSql, , "Manager Sales Report Error")
            End If

            pDT = .Requery

            dtaView = New DataView(pDT)
            dtaView.RowFilter = " TotalSales <> 0"

        End With

        Return dtaView.ToTable()

    End Function

    Function getManagerSalesReportPaymentInfoByCashierMachine(ByVal strRangeType As String,
                                          ByVal strRangeFrom As String,
                                          ByVal strRangeTo As String,
                                          ByVal strRangeYear As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtaView As DataView
        Dim strCustInvDateRange As String
        Dim strCustPayDateRange As String
        Dim strMemoDateRange As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        strCustInvDateRange = ""
        strCustPayDateRange = ""
        strMemoDateRange = ""

        If (strRangeType = "Today") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today, True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today, False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Yesterday") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today.AddDays(-1), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Day") Then

            strRangeFrom = getFOWSTOperationHR(Convert.ToDateTime(strRangeFrom), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Convert.ToDateTime(strRangeTo), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " inv.InvoiceDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Week") Then

            strCustInvDateRange = " DATEPART(week,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(week,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Month") Then

            strCustInvDateRange = " DATEPART(month,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(month,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Quarter") Then

            strCustInvDateRange = " DATEPART(q,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(q,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Year") Then

            strCustInvDateRange = " DATEPART(year,inv.InvoiceDate) = " & strRangeYear

            strCustPayDateRange = " DATEPART(year,custpay.PaymentDate) = " & strRangeYear

            strMemoDateRange = " DATEPART(year,memo.LastUpdated) = " & strRangeYear

        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("CashierName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cshr.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') "
                End With
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = " ISNULL((SELECT TOP 1 LTRIM(RTRIM(CAST(minfowst.FOWSTNo as CHAR))) +'-'+ LTRIM(RTRIM(CAST(mindev.DeviceNo as CHAR))) " &
                          " FROM dbo.LibraryDeviceFowst mindevf " &
                          " INNER JOIN dbo.LibraryDevice mindev ON (mindevf.DeviceID = mindev.ID) " &
                          " LEFT JOIN dbo.MobizLibraryOrganizationFOWST minfowst ON (mindevf.FowstID = minfowst.ID) " &
                          " WHERE mindevf.ID = devf.ID),'')"
                End With
                With .Add("TotalSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - ISNULL(invitem.DiscountAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(inv.SCDiscount)" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND memo.UserID = orgemp.PeopleID) " &
                               ",0) - " &
                               " ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND memo.UserID = orgemp.PeopleID) " &
                                  ",0) + " &
                               "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID " &
                               " AND custpay.UserID = orgemp.PeopleID) " &
                                  ",0) "
                    '" AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf, dbo.LibraryOrganizationEmployee orgemp " &
                       " INNER JOIN dbo.MobizLibraryPeople cshr ON (orgemp.PeopleID = cshr.ID) "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(WebRequest.Instance.FowstID)

            pDT = .Requery

            dtaView = New DataView(pDT)
            dtaView.RowFilter = " TotalSales <> 0"

        End With

        Return dtaView.ToTable()

    End Function

    Function getManagerSalesReportOtherPaymentInfo(ByVal strRangeType As String,
                                          ByVal strRangeFrom As String,
                                          ByVal strRangeTo As String,
                                          ByVal strRangeYear As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strCustInvDateRange As String
        Dim strCustPayDateRange As String
        Dim strMemoDateRange As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        strCustInvDateRange = ""
        strCustPayDateRange = ""
        strMemoDateRange = ""

        If (strRangeType = "Today") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today, True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today, False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Yesterday") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Date.Today.AddDays(-1), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Day") Then

            strRangeFrom = getFOWSTOperationHR(Convert.ToDateTime(strRangeFrom), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")
            strRangeTo = getFOWSTOperationHR(Convert.ToDateTime(strRangeTo), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy hh:mm:ss tt")

            strCustInvDateRange = " CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " custpay.PaymentDate BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " memo.LastUpdated BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Week") Then

            strCustInvDateRange = " DATEPART(week,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(week,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Month") Then

            strCustInvDateRange = " DATEPART(month,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(month,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Quarter") Then

            strCustInvDateRange = " DATEPART(q,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(q,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Year") Then

            strCustInvDateRange = " DATEPART(year,inv.InvoiceDate) = " & strRangeYear

            strCustPayDateRange = " DATEPART(year,custpay.PaymentDate) = " & strRangeYear

            strMemoDateRange = " DATEPART(year,memo.LastUpdated) = " & strRangeYear

        End If

        Dim pSql As String

        pSql = "select PaymentType,sum(Amount) TotalAmount from TransCustomerPaymentOther " & vbCrLf &
                "inner join TransCustomerPaymentInvoices custpayinv on custpayinv.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID  " & vbCrLf &
                "INNER JOIN dbo.TransCustomerPayment custpay ON custpayinv.CustomerPaymentID = custpay.ID " & vbCrLf &
                "inner join TransInvoice inv on inv.ID = custpayinv.InvoiceID  " & vbCrLf &
                 " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
                 " AND inv.FOWSTID = " & ReadyString(WebRequest.Instance.FowstID) & vbCrLf &
                 " group by PaymentType"

        'pSql = "select PaymentType,sum(Amount) from TransCustomerPaymentOther " & vbCrLf &
        '        "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID " & vbCrLf &
        '        "inner join TransInvoice on TransInvoice.ID = TransCustomerPaymentInvoices.InvoiceID " & vbCrLf &
        '        " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
        '        " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " inv.InvoiceDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) & vbCrLf &
        '        " AND devf.FowstID = " & ReadyString(WebRequest.Instance.FowstID) & vbCrLf &
        '        " group by PaymentType"

        Return Common.WebRequest.ProData.Execute(pSql)

    End Function

    Function getManagerSalesTransactionSummary(ByVal strRangeType As String,
                                          ByVal strRangeFrom As String,
                                          ByVal strRangeTo As String,
                                          ByVal strRangeYear As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strCustInvDateRange As String
        Dim strCustPayDateRange As String
        Dim strMemoDateRange As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        WebRequest.Instance.Business.TransMemoSalesReturn.FixSalesReturnDeviceIdFowstId()
        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        strCustInvDateRange = ""
        strCustPayDateRange = ""
        strMemoDateRange = ""

        If (strRangeType = "Today") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today, True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy")
            strRangeTo = getFOWSTOperationHR(Date.Today, False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy")

            strCustInvDateRange = " CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " CONVERT(date, custpay.PaymentDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Yesterday") Then

            strRangeFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy")
            strRangeTo = getFOWSTOperationHR(Date.Today.AddDays(-1), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy")

            strCustInvDateRange = " CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " CONVERT(date, custpay.PaymentDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Day") Then

            strRangeFrom = getFOWSTOperationHR(Convert.ToDateTime(strRangeFrom), True, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy")
            strRangeTo = getFOWSTOperationHR(Convert.ToDateTime(strRangeTo), False, WebRequest.Instance.FowstID).ToString("MM/dd/yyyy ")

            strCustInvDateRange = " CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strCustPayDateRange = " CONVERT(date, custpay.PaymentDate) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)
            strMemoDateRange = " CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo)

        ElseIf (strRangeType = "Week") Then

            strCustInvDateRange = " DATEPART(week,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(week,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Month") Then

            strCustInvDateRange = " DATEPART(month,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(month,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Quarter") Then

            strCustInvDateRange = " DATEPART(q,inv.InvoiceDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,inv.InvoiceDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strCustPayDateRange = " DATEPART(q,custpay.PaymentDate) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,custpay.PaymentDate) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

            strMemoDateRange = " DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo

        ElseIf (strRangeType = "Year") Then

            strCustInvDateRange = " DATEPART(year,inv.InvoiceDate) = " & strRangeYear

            strCustPayDateRange = " DATEPART(year,custpay.PaymentDate) = " & strRangeYear

            strMemoDateRange = " DATEPART(year,memo.LastUpdated) = " & strRangeYear

        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '" AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurSalesDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount)" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '     " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurSCDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SCDiscount)" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '         " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015, because senior citizen discount is vat exempt
                End With
                With .Add("CurVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '     " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)-ISNULL(invitem.DiscountSales,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF'  and (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL)" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)-ISNULL(invitem.DiscountTax,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' and (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL)" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVatablePortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.Vatable) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVatAmountPortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.VAT) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(memosr.TaxAmount) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.NonVATSale) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurVATExemptSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(inv.VATExempt) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' ) AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    'AND invitem.TaxAmount = 0 AND ISNULL(invitem.VATableSale,0) = 0 
                End With
                With .Add("CurZeroRatedVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.ZeroRatedSale) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurOthers", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("OnCreditAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OnCreditAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("SMACAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                             " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'SMAC' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("EPlusAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'E-Plus' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("VoucherAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"

                End With
                With .Add("GCashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                             " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE other.PaymentType = 'GCash' AND " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("GiftCertAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("LoyaltyCardAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CreditTermsAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("OtherTypeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE " & IIf(strCustPayDateRange.Trim().Length > 0, strCustPayDateRange, " custpay.PaymentDate = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("SalesDate", SqlDbType.Date)
                    .Expression = IIf(strRangeType = "Today", ReadyString(Date.Today.ToString("MM/dd/yyyy")), ReadyString(Date.Today.AddDays(-1).ToString("MM/dd/yyyy")))
                End With
                With .Add("CurTransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurTransInvItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(invitem.ProductID) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID" &
                               " AND invitem.Quantity > 0)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountMemoReturnExchange", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.SrvNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchangeItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
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
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesRefundItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE " & IIf(strMemoDateRange.Trim().Length > 0, strMemoDateRange, " CAST(memo.LastUpdated AS DATE) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurCustomerCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT SUM(inv.TotalNumberOfCustomer) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND " & IIf(strCustInvDateRange.Trim().Length > 0, strCustInvDateRange, " CONVERT(date, inv.InvoiceDate) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy"))) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(WebRequest.Instance.FowstID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesZReportSummary(ByVal strRangeType As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtDateFrom As Date
        Dim dtDateto As Date

        If (strRangeType = "Today") Then
            dtDateFrom = getFOWSTOperationHR(Date.Today, True, WebRequest.Instance.FowstID)
            dtDateto = getFOWSTOperationHR(Date.Today, False, WebRequest.Instance.FowstID)
        ElseIf (strRangeType = "Yesterday") Then
            dtDateFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, WebRequest.Instance.FowstID)
            dtDateto = getFOWSTOperationHR(Date.Today.AddDays(-1), False, WebRequest.Instance.FowstID)
        End If

        'dtDateFrom = Convert.ToDateTime("07/10/2013 08:00:00")
        'dtDateto = Convert.ToDateTime("07/10/2013 19:00:00")


        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("BegBal", SqlDbType.Money)
                    .Expression = " ISNULL(devf.BegBalance,0) "
                End With
                With .Add("PrevGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    ' " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015

                End With
                With .Add("PrevTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND INULL(inv.SCDiscount, 0) = 0 AND ISNULL(inv.SoloParentDiscount, 0) = 0 " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    ' " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("PrevTotalSCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.SCDiscount,0)) " & vbCrLf &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " WHERE custpay.PaymentDate < " & vbCrLf & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
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
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'" &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '    " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) + SUM(inv.SCDiscount) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '    " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurTotalVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.ForeGoneVAT) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '    " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)- ISNULL(invitem.DiscountSales,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL)" &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)- ISNULL(invitem.DiscountTax,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(memosr.TaxAmount) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurLocalTax", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.NonVATSale) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurVATExemptSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - invitem.DiscountAmount) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' ) AND invitem.TaxAmount = 0 AND ISNULL(invitem.VATableSale,0) = 0 AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurZeroRatedVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.ZeroRatedSale) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurOthers", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("SMACAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE other.PaymentType = 'SMAC' AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("EPlusAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE other.PaymentType = 'E-Plus' AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("VoucherAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                              " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("GiftCertAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("LoyaltyCardAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CreditTermsAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("SalesDate", SqlDbType.Date)
                    .Expression = ReadyString(dtDateFrom.ToString("MM/dd/yyyy"))
                End With
                With .Add("CurBegInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MIN(inv.InvoiceNumber) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurEndInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MAX(inv.InvoiceNumber) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurTransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurTransInvItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(invitem.ProductID) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID" &
                               " AND invitem.Quantity > 0)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountMemoReturnExchange", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.SrvNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchangeItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
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
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesRefundItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTermNum", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("FowstNo", SqlDbType.Int)
                    .Expression = "fowst.FOWSTNo"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryDeviceFowst devf " &
                    " INNER JOIN dbo.LibraryDevice dev ON (devf.DeviceID = dev.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(WebRequest.Instance.FowstID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesZReportSummary(ByVal strRangeType As String, strDate As String, FOWSTID As String, DeviceID As String, Optional mProData As ProData = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtDateFrom As DateTime
        Dim dtDateto As DateTime

        If (strRangeType = "Today") Then
            dtDateFrom = getFOWSTOperationHR(Date.Today, True, FOWSTID)
            dtDateto = getFOWSTOperationHR(Date.Today, False, FOWSTID)
        ElseIf (strRangeType = "Yesterday") Then
            dtDateFrom = getFOWSTOperationHR(Date.Today.AddDays(-1), True, FOWSTID)
            dtDateto = getFOWSTOperationHR(Date.Today.AddDays(-1), False, FOWSTID)
        ElseIf (strRangeType = "By Date") Then
            dtDateFrom = getFOWSTOperationHR(Convert.ToDateTime(strDate), True, FOWSTID, mProData)
            dtDateto = getFOWSTOperationHR(Convert.ToDateTime(strDate), False, FOWSTID, mProData)
        End If

        dtDateFrom = Convert.ToDateTime(dtDateFrom.ToString("MM/dd/yyyy") & " 00:00:00")

        'we put dtDateFrom below because some malls want 24 operating hours in their report
        dtDateto = Convert.ToDateTime(dtDateFrom.ToString("MM/dd/yyyy") & " 23:59:59")

        pFinder = New Finder
        With pFinder

            If mProData Is Nothing Then
                .ProData = Common.WebRequest.ProData
            Else
                .ProData = mProData
            End If

            With .Fields
                With .Add("BegBal", SqlDbType.Money)
                    .Expression = " ISNULL(devf.BegBalance,0) "
                End With
                With .Add("PrevGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '      " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015

                End With
                With .Add("PrevTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '    " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With

                With .Add("PrevSoloParentDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.SoloParentDiscount,0)) " & vbCrLf &
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
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.PhNatTeamDiscount,0)) " & vbCrLf &
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

                With .Add("PrevSCDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SCDiscount)" &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '         " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015, because senior citizen discount is vat exempt
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
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
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
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("PrevChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("PrevSCDiscountExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SCDiscountExempt) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSCDiscountExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SCDiscountExempt) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '        " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount)" &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("TotalSoloParentDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.SoloParentDiscount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Solo Parent Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("TotalPhNatTeamDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(inv.PhNatTeamDiscount,0)) " & vbCrLf &
                               " FROM TransCustomerPaymentInvoices custpayinv " & vbCrLf &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " & vbCrLf &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " & vbCrLf &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0 AND CONVERT(datetime, inv.InvoiceDate) BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) & vbCrLf &
                               " AND (LibraryDiscountSettings.DiscountType = 'Philippine National Sports Team Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND inv.DeviceID = devf.DeviceID) " & vbCrLf &
                               ",0)"
                End With
                With .Add("CurSCDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SCDiscount)" &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '         " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015, because senior citizen discount is vat exempt
                End With
                With .Add("CurVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("RegularDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM TransInvoiceItems invitem " &
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '764EDACB-0131-4033-8E44-7B903368008C' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("EmployeeDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM TransInvoiceItems invitem " &
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '7804A35A-B531-4380-945E-FC9643E99843' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("SeniorCitizenDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM TransInvoiceItems invitem " &
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '055A0A33-DD98-47EC-ADD3-8B2009F3944A' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("PWDDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM TransInvoiceItems invitem " &
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '34A4032F-A142-4C15-A868-4AC4410434DF' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("VIPDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM TransInvoiceItems invitem " &
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = '35F954A0-C67C-499C-AF60-F08E64477FDC' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("GPCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                            " FROM TransInvoiceItems invitem " &
                                            " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                            " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                            " WHERE inv.InvoiceNumber > 0 " &
                                            " AND invdisc.DiscountTypeID = 'C2218C86-0450-4CD3-B019-3EDD43E51741' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("Discount1", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " &
                                               " FROM TransInvoiceItems invitem " &
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " &
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " &
                                               " WHERE inv.InvoiceNumber > 0 " &
                                               " AND invdisc.DiscountTypeID = 'C4199347-6A00-45C9-BE11-8934DD63ECD1' " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    '   " AND invitem.TaxAmount > 0 " & commented by janine feb 9 2015
                End With
                With .Add("CurSalesReturnRefund", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchange", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSales", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)) " &
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)- ISNULL(invitem.DiscountSales,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' and (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL) " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVAT", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)) " &
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)- ISNULL(invitem.DiscountTax,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' and (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL) " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVatablePortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.Vatable) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVatAmountPortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.VAT) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATSalesSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(memosr.TotalAmount,0) - ISNULL(memosr.TaxAmount,0)) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurTotalVATAmountSalesReturn", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(memosr.TaxAmount) " &
                               " FROM TransMemoSalesReturn memosr " &
                               " INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurLocalTax", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurNoTAXSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.NonVATSale) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
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
                               " WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' ) AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                    'AND invitem.TaxAmount = 0 AND ISNULL(invitem.VATableSale,0) = 0 
                End With
                With .Add("CurZeroRatedVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.ZeroRatedSale) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CurOthers", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("OnCreditAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OnCreditAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("OtherTypeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With
                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("SMACAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE other.PaymentType = 'SMAC' AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("EPlusAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE other.PaymentType = 'E-Plus' AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("VoucherAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " &
                              " FROM TransCustomerPaymentInvoices custpayinv " &
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " &
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                              " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND custpay.PaymentDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                              " AND inv.FOWSTID = devf.FowstID " &
                              " AND inv.DeviceID = devf.DeviceID) " &
                              ",0)"
                End With
                With .Add("GiftCertAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("LoyaltyCardAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("CreditTermsAmount", SqlDbType.Money)
                    .Expression = "0"
                End With
                With .Add("SalesDate", SqlDbType.Date)
                    .Expression = ReadyString(dtDateFrom.ToString("MM/dd/yyyy"))
                End With
                With .Add("CurBegInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MIN(inv.InvoiceNumber) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'  " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurEndInvoiceNo", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT MAX(inv.InvoiceNumber) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'  " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurTransCountInv", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'  " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID)" &
                               ",0)"
                End With
                With .Add("CurTransInvItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(invitem.ProductID) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'  " &
                               " AND inv.InvoiceDate BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID" &
                               " AND invitem.Quantity > 0)" & vbCrLf &
                               ",0)"
                End With
                With .Add("CurTransCountMemoReturnExchange", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.SrvNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesReturnExchangeItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
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
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurSalesRefundItemCount", SqlDbType.Int)
                    .Expression = "ISNULL((SELECT COUNT(memoret.ProductID) " &
                               " FROM TransSalesReturnLedger srledger " &
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                               " INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " &
                               " WHERE memo.LastUpdated BETWEEN " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) & " AND " & ReadyString(dtDateto.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                               " AND memo.FOWSTID = devf.FowstID " &
                               " AND memo.DeviceID = devf.DeviceID " &
                               " AND srledger.DestinationPaymentID IS NULL " &
                               " AND srledger.RefundNum > 0) " &
                               ",0)"
                End With
                With .Add("CurTermNum", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("FowstNo", SqlDbType.Int)
                    .Expression = "fowst.FOWSTNo"
                End With
                With .Add("ZCount", SqlDbType.Int)
                    .Expression = "devf.ZReadCount"
                End With

                With .Add("PresentationPrevGrossSales", SqlDbType.Int)
                    .Expression = "0"
                End With
                With .Add("PresentationCurrGrossSales", SqlDbType.Int)
                    .Expression = "0"
                End With

            End With

            .SqlFrom = " /* vbA7XJ7CTDKZ vb3LY6M9GD7S */ FROM LibraryDeviceFowst devf " &
                    " INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) " &
                    " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) "

            .SqlWhere = " WHERE devf.FowstID = " & ReadyString(FOWSTID)

            If Not DeviceID = "" Then
                .SqlFrom &= " AND dev.ID = " & ReadyString(DeviceID)
            End If

            .ProData.CommandTimeout = 10 * 60 * 1000 ' 10 minutes

            pDT = .Requery

        End With

        Return pDT

    End Function

    Public Shared Function GetDateRange(ByVal strRangeType As String, strDate As String) As DateRange

        Dim dtDateFrom As DateTime
        Dim dtDateto As DateTime

        If (strRangeType = "Today") Then
            dtDateFrom = Date.Today
            dtDateto = Date.Today
        ElseIf (strRangeType = "Yesterday") Then
            dtDateFrom = Date.Today.AddDays(-1)
            dtDateto = Date.Today.AddDays(-1)
        ElseIf (strRangeType = "By Date") Then
            dtDateFrom = Convert.ToDateTime(strDate)
            dtDateto = Convert.ToDateTime(strDate)
        End If

        dtDateFrom = Convert.ToDateTime(dtDateFrom.ToString("MM/dd/yyyy") & " 00:00:00")
        dtDateto = Convert.ToDateTime(dtDateto.ToString("MM/dd/yyyy") & " 23:59:59")

        Dim pOut As New DateRange()
        pOut.DateFrom = dtDateFrom
        pOut.DateTo = dtDateto

        Return pOut

    End Function

    Function getSalesZReportSummaryFromCache(ByVal strRangeType As String, strDate As String, FOWSTID As String, DeviceID As String, Optional mProdata As ProData = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtDateFrom As DateTime
        Dim dtDateto As DateTime
        Dim clsSalesReport As New ReportPOS

        Dim pSql As String

        If (strRangeType = "Today") Then
            dtDateFrom = Date.Today
            dtDateto = Date.Today
        ElseIf (strRangeType = "Yesterday") Then
            dtDateFrom = Date.Today.AddDays(-1)
            dtDateto = Date.Today.AddDays(-1)
        ElseIf (strRangeType = "By Date") Then
            dtDateFrom = Convert.ToDateTime(strDate)
            dtDateto = Convert.ToDateTime(strDate)
        End If

        dtDateFrom = Convert.ToDateTime(dtDateFrom.ToString("MM/dd/yyyy") & " 00:00:00")

        dtDateto = Convert.ToDateTime(dtDateto.ToString("MM/dd/yyyy") & " 23:59:59")

        If DeviceID = vbNullString AndAlso Not CStr2(Common.WebRequest.Data("UUID")) = vbNullString Then
            DeviceID = CStr2(Common.WebRequest.ProData.ExecuteScalar("Select top 1 ID from LibraryDevice WHERE UUID = " & ReadyString(CStr2(Common.WebRequest.Data("UUID")))))
        End If

        pSql = "Select top 1 * from TransZReadCache " & vbCrLf &
            " WHERE FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
            " AND convert(date, SalesDate) = " & ReadyString(dtDateFrom) & vbCrLf &
            " AND DeviceID = " & ReadyString(DeviceID) & vbCrLf

        pSql &= " Order by SalesDate desc"

        If DeviceID = vbNullString Then
            pDT = getSalesZReportSummary(strRangeType, strDate, FOWSTID, DeviceID)
        Else

            Try
                If Not CStr2(Common.WebRequest.Data("UUID")) = vbNullString Then

                    Dim pBusiness As New Business(ProData.CreateFromConnectionInfo(
                       vServer:=Common.ConnectionInfo.Server,
                       vDatabase:="MobizWebPOSCache",
                       vUsername:=Common.ConnectionInfo.Username,
                       vPassword:=Common.ConnectionInfo.Password,
                       vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity
                   ))

                    Using mReports = pBusiness.ProData
                        pDT = mReports.Execute(pSql)
                    End Using

                    If pDT.Rows.Count > 0 Then
                        Return pDT
                    End If
                Else
                    pDT = Common.WebRequest.ProData.Execute(pSql)
                End If


            Catch ex As Exception

                AppendToLog(ex.Message)

                Try
                    pDT = mProdata.Execute(pSql)

                Catch ex2 As Exception
                    AppendToLog(ex2.Message)
                End Try

            End Try

            If pDT.Rows.Count = 0 Then 'And DateDiff(DateInterval.Day, dtDateFrom, Today) = 0 

                Dim p As New InsertUpdate("TransZReadCache")
                Dim dtPrevZRead As DataTable

                pSql = "Select top 1 * from TransZReadCache " & vbCrLf &
                        " WHERE FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND convert(date, SalesDate) <= " & ReadyString(dtDateFrom) & vbCrLf &
                        " AND DeviceID = " & ReadyString(DeviceID) & vbCrLf &
                        " Order by SalesDate desc"

                dtPrevZRead = Common.WebRequest.ProData.Execute(pSql)

                '--

                If dtPrevZRead.Rows.Count = 0 Then
                    dtPrevZRead = clsSalesReport.getSalesZReportSummary(strRangeType, strDate, FOWSTID, CStr2(Common.WebRequest.Data("DeviceID")))
                    If dtPrevZRead.Rows.Count > 0 Then
                        p("PresentationPrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
                    Else
                        p("PresentationPrevGrossSales") = 0

                    End If

                Else
                    p("PresentationPrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PresentationPrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
                End If

                p("DeviceID") = ReadyString(DeviceID)
                p("FOWSTID") = ReadyString(FOWSTID)

                p("PrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
                p("PrevTotalDiscount") = CDec2(dtPrevZRead.Rows(0).Item("PrevTotalDiscount")) + CDec2(dtPrevZRead.Rows(0).Item("CurTotalDiscount"))
                p("PrevSalesReturnRefund") = CDec2(dtPrevZRead.Rows(0).Item("PrevSalesReturnRefund")) + CDec2(dtPrevZRead.Rows(0).Item("CurSalesReturnRefund"))
                p("PrevSalesReturnExchange") = CDec2(dtPrevZRead.Rows(0).Item("PrevSalesReturnExchange")) + CDec2(dtPrevZRead.Rows(0).Item("CurSalesReturnExchange"))
                p("PrevTotalVAT") = CDec2(dtPrevZRead.Rows(0).Item("PrevTotalVAT")) + CDec2(dtPrevZRead.Rows(0).Item("CurTotalVAT"))
                p("PrevServiceCharge") = CDec2(dtPrevZRead.Rows(0).Item("PrevServiceCharge")) + CDec2(dtPrevZRead.Rows(0).Item("CurServiceCharge"))
                p("PrevChargeAmount") = CDec2(dtPrevZRead.Rows(0).Item("PrevChargeAmount")) + CDec2(dtPrevZRead.Rows(0).Item("ChargeAmount"))

                p("SalesDate") = ReadyString(dtDateFrom)
                'p("PresentationPrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PresentationPrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
                p("CurGrossSales") = 0
                p("CurTotalDiscount") = 0
                p("CurTotalVATSales") = 0
                p("CurTotalVAT") = 0
                p("CurLocalTax") = 0
                p("CurServiceCharge") = 0
                p("CurNoTAXSales") = 0
                p("CurVATExemptSales") = 0
                p("CurZeroRatedVATSales") = 0
                p("CurOthers") = 0
                p("CashAmount") = 0
                p("DebitCardAmount") = 0
                p("CreditCardAmount") = 0
                p("SMACAmount") = 0
                p("CheckPaymentAmount") = 0
                p("EPlusAmount") = 0
                p("VoucherAmount") = 0
                p("CurBegInvoiceNo") = 0
                p("CurEndInvoiceNo") = 0
                p("CurTransCountInv") = 0
                p("CurTransInvItemCount") = 0
                p("CurTransCountMemoReturnExchange") = 0
                p("CurSalesReturnExchangeItemCount") = 0
                p("CurTransCountMemoRefund") = 0
                p("CurSalesRefundItemCount") = 0
                p("CurTermNum") = CDec2(dtPrevZRead.Rows(0).Item("CurTermNum"))
                p("FowstNo") = CDec2(dtPrevZRead.Rows(0).Item("FowstNo"))

                pSql = p.GetInsert()

                Common.WebRequest.ProData.Execute(pSql)

                pSql = "Select top 1 * from TransZReadCache " & vbCrLf &
                        " WHERE FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND convert(date, SalesDate) = " & ReadyString(dtDateFrom) & vbCrLf &
                        " AND DeviceID = " & ReadyString(DeviceID) & vbCrLf

                pSql &= " Order by SalesDate desc"

                Try
                    pDT = Common.WebRequest.ProData.Execute(pSql)

                Catch ex As Exception

                    AppendToLog(ex.Message)

                    Try
                        pDT = mProdata.Execute(pSql)

                    Catch ex2 As Exception
                        AppendToLog(ex2.Message)
                    End Try

                End Try

                'getSalesZReportSummaryFromCache(strRangeType, strDate, FOWSTID, DeviceID, Nothing)

            End If
        End If

        If pDT.Rows.Count > 0 Then
            Dim zCount As Integer

            pSql = "Select ZReadCount from LibraryDeviceFOWST" & vbCrLf &
                    " where DeviceID = " & ReadyString(CStr2(DeviceID)) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(CStr2(FOWSTID))

            zCount = CInt2(Common.WebRequest.ProData.ExecuteScalar(pSql))
            If pDT.Columns.Contains("ZCount") = False Then
                pDT.Columns.Add("ZCount")
            End If
            pDT.Rows(0).Item("ZCount") = zCount
        End If

        Return pDT
    End Function

    Function getFOWSTOperationHR(ByVal dtDate As Date, ByVal IsOpening As Boolean, FOWSTID As String, Optional mProdata As ProData = Nothing) As DateTime
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtReturn As DateTime

        pFinder = New Finder
        With pFinder

            If mProdata Is Nothing Then
                .ProData = Common.WebRequest.ProData
            Else
                .ProData = mProdata
            End If

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

            .SqlWhere = " WHERE opthr.FOWSTID = " & ReadyString(FOWSTID) &
                    " AND opthr.WeekDay = DATEPART(weekday,'" & dtDate.ToString("MM/dd/yyyy") & "')"

            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then
            If (CDec2(pDT.Rows(0).Item("IsFOWSTInsideAMall")) = 1 And pDT.Rows(0).Item("FOWSTMallName").ToString().Trim().ToUpper() = "SM MALL") Then
                If (IsOpening) Then
                    dtReturn = dtDate.ToString("MM/dd/yyyy") & " 00:00:00"
                Else
                    dtReturn = dtDate.AddDays(1).AddMilliseconds(-1)
                End If
            Else
                If (IsOpening) Then
                    dtReturn = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & pDT.Rows(0).Item("OpeningTime").ToString())
                Else
                    If (Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & pDT.Rows(0).Item("OpeningTime").ToString()) > Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & pDT.Rows(0).Item("ClosingTime").ToString())) Then
                        dtReturn = Convert.ToDateTime(dtDate.AddDays(1).ToString("MM/dd/yyyy") & " " & pDT.Rows(0).Item("ClosingTime").ToString())
                    Else
                        dtReturn = Convert.ToDateTime(dtDate.ToString("MM/dd/yyyy") & " " & pDT.Rows(0).Item("ClosingTime").ToString())
                    End If
                End If
            End If

        ElseIf IsOpening Then
            dtReturn = dtDate
        Else
            dtReturn = dtDate.AddDays(1).AddMilliseconds(-1)
        End If

        Return dtReturn

    End Function

    Function getSalesAdjustmentReportInfo(ByVal strSubscriberFowstID As String,
                                   ByVal strRangeType As String,
                                   ByVal strRangeFrom As String,
                                   ByVal strRangeTo As String,
                                   ByVal strRangeYear As String) As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pSQL As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        pSQL = "SELECT " & strRangeYearFrom & " as YearFrom, " &
                strRangeYearTo & " as YearTo, " &
                IIf(strRangeType = "Today", ReadyString(Date.Today.ToString("MM/dd/yyyy")), ReadyString(Date.Today.AddDays(-1).ToString("MM/dd/yyyy"))) & " as SalesDate "

        pDT = Common.WebRequest.ProData.Execute(pSQL)

        Return pDT

    End Function

    Function getSalesAdjustmentRefundReport(ByVal strSubscriberFowstID As String,
                                      ByVal strRangeType As String,
                                      ByVal strRangeFrom As String,
                                      ByVal strRangeTo As String,
                                      ByVal strRangeYear As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strWhere As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("SRVNo", SqlDbType.Int)
                    .Expression = "sretledger.RefundNum"
                End With
                With .Add("TransDate", SqlDbType.Money)
                    .Expression = "memo.LastUpdated"
                End With
                With .Add("GrossRefundAmount", SqlDbType.Money)
                    .Expression = "sretledger.GrossRefundAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransSalesReturnLedger sretledger " &
                    " INNER JOIN dbo.TransMemo memo ON (sretledger.SourceMemoID = memo.ID) " &
                    " INNER JOIN dbo.TransMemoSalesReturn memosret ON (sretledger.SourceMemoID = memosret.MemoID) "


            strWhere = ""
            If (strRangeType = "Today") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND cast(memo.LastUpdated as date) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy")) &
                    " AND sretledger.GrossRefundAmount > 0"

            ElseIf (strRangeType = "Yesterday") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND cast(memo.LastUpdated as date) = " & ReadyString(Date.Today.AddDays(-1).ToString("MM/dd/yyyy")) &
                    " AND sretledger.GrossRefundAmount > 0"

            ElseIf (strRangeType = "Day") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND cast(memo.LastUpdated as date) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) &
                    " AND sretledger.GrossRefundAmount > 0"

            ElseIf (strRangeType = "Week") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                    " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo &
                    " AND sretledger.GrossRefundAmount > 0"

            ElseIf (strRangeType = "Month") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                    " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo &
                    " AND sretledger.GrossRefundAmount > 0"

            ElseIf (strRangeType = "Quarter") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                    " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo &
                    " AND sretledger.GrossRefundAmount > 0"

            ElseIf (strRangeType = "Year") Then
                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(year,memo.LastUpdated) = " & strRangeYear &
                    " AND sretledger.GrossRefundAmount > 0"

            End If

            .SqlWhere = strWhere


            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesAdjustmentSRVReport(ByVal strSubscriberFowstID As String,
                                  ByVal strRangeType As String,
                                  ByVal strRangeFrom As String,
                                  ByVal strRangeTo As String,
                                  ByVal strRangeYear As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim strWhere As String
        Dim strRangeYearFrom As String
        Dim strRangeYearTo As String

        strRangeYearFrom = Date.Today.Year.ToString()
        strRangeYearTo = Date.Today.Year.ToString()
        If (strRangeYear = "This year.") Then
            strRangeYearFrom = Date.Today.Year.ToString()
        ElseIf (strRangeYear = "This year and last year.") Then
            strRangeYearFrom = Date.Today.AddYears(-1).Year.ToString()
        ElseIf (strRangeYear = "This year and last 2 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-2).Year.ToString()
        ElseIf (strRangeYear = "This year and last 3 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-3).Year.ToString()
        ElseIf (strRangeYear = "This year and last 4 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-4).Year.ToString()
        ElseIf (strRangeYear = "This year and last 5 years.") Then
            strRangeYearFrom = Date.Today.AddYears(-5).Year.ToString()
        End If

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("SRVNo", SqlDbType.Int)
                    .Expression = "sretledger.SrvNum"
                End With
                With .Add("TransDate", SqlDbType.Money)
                    .Expression = "memo.LastUpdated"
                End With
                With .Add("GrossSrvAmount", SqlDbType.Money)
                    .Expression = "sretledger.GrossSrvAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransSalesReturnLedger sretledger " &
                    " INNER JOIN dbo.TransMemo memo ON (sretledger.SourceMemoID = memo.ID) " &
                    " INNER JOIN dbo.TransMemoSalesReturn memosret ON (sretledger.SourceMemoID = memosret.MemoID) "


            strWhere = ""
            If (strRangeType = "Today") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND cast(memo.LastUpdated as date) = " & ReadyString(Date.Today.ToString("MM/dd/yyyy")) &
                    " AND sretledger.GrossSrvAmount > 0"

            ElseIf (strRangeType = "Yesterday") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND cast(memo.LastUpdated as date) = " & ReadyString(Date.Today.AddDays(-1).ToString("MM/dd/yyyy")) &
                    " AND sretledger.GrossSrvAmount > 0"

            ElseIf (strRangeType = "Day") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND cast(memo.LastUpdated as date) BETWEEN " & ReadyString(strRangeFrom) & " AND " & ReadyString(strRangeTo) &
                    " AND sretledger.GrossSrvAmount > 0"

            ElseIf (strRangeType = "Week") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(week,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                    " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo &
                    " AND sretledger.GrossSrvAmount > 0"

            ElseIf (strRangeType = "Month") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(month,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                    " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo &
                    " AND sretledger.GrossSrvAmount > 0"

            ElseIf (strRangeType = "Quarter") Then

                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(q,memo.LastUpdated) BETWEEN " & strRangeFrom & " AND " & strRangeTo &
                    " AND DATEPART(year,memo.LastUpdated) BETWEEN " & strRangeYearFrom & " AND " & strRangeYearTo &
                    " AND sretledger.GrossSrvAmount > 0"

            ElseIf (strRangeType = "Year") Then
                strWhere = IIf(strSubscriberFowstID.Trim().Length = 0, " WHERE sretledger.FowstID IS NULL", " WHERE sretledger.FowstID = " + ReadyString(strSubscriberFowstID)) &
                    " AND DATEPART(year,memo.LastUpdated) = " & strRangeYear &
                    " AND sretledger.GrossSrvAmount > 0"

            End If

            .SqlWhere = strWhere


            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetNewEJournal()
        Dim fowstid As String = Common.WebRequest.Data("FOWSTID")
        Dim deviceid As String = Common.WebRequest.Data("DeviceID")
        Dim datefrom As String = Common.WebRequest.Data("From")
        Dim dateto As String = Common.WebRequest.Data("To")
        Dim excludeInvoice As String = Common.WebRequest.Data("ExcludeInvoice")
        Dim excludeZRead As String = Common.WebRequest.Data("ExcludeZRead")
        Dim excludeXRead As String = Common.WebRequest.Data("ExcludeXRead")
        Dim excludeReturns As String = Common.WebRequest.Data("ExcludeReturns")
        Dim pSql As String
        Dim sb As StringBuilder = New StringBuilder()
        Dim culture As New Globalization.CultureInfo("en-US")
        Dim mFilenameFolder As String
        Dim mFilenameParentFolder As String
        Dim netsales As Decimal = 0
        Dim totalpayments As Decimal = 0

        mFilenameFolder = Common.ServerPath & "Content\EJournal\" & WebRequest.Instance().OrgID
        mFilenameParentFolder = Common.ServerPath & "Content\EJournal\"

        If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
            System.IO.Directory.CreateDirectory(mFilenameParentFolder)
        End If

        If Not System.IO.Directory.Exists(mFilenameFolder) Then
            System.IO.Directory.CreateDirectory(mFilenameFolder)
        End If

        Try

            'jss.Deserialize(Of Object)(zread)("d")("Header")("Rows")(0)("MIN")

            Dim header As Object = getReportHeader(Common.WebRequest.ProData, fowstid, Nothing, deviceid).Rows(0)
            sb.Append("Business Name:" & vbTab & header("OrgBusinessTradeName"))
            sb.AppendLine()
            sb.Append("Company Name:" & vbTab & header("SubscriberOrganizationName"))
            sb.AppendLine()
            sb.Append("Branch:" & vbTab & vbTab & header("OrgBranchName"))
            sb.AppendLine()
            sb.Append("Address:" & vbTab & header("OrgCompleteAddress"))
            sb.AppendLine()
            sb.Append(IIf(CBool2(header("IsVAT")), "VAT ", "Non-VAT ") & "TIN:" & vbTab & header("OrgTIN"))
            sb.AppendLine()
            sb.Append("PTU No.:" & vbTab & header("POSRegistrationNo"))
            sb.AppendLine()
            sb.Append("MIN:" & vbTab & vbTab & header("MIN"))
            sb.AppendLine()
            sb.Append("Serial No.:" & vbTab & header("UuidDisplay"))

            sb.AppendLine()
            sb.AppendLine()

            sb.Append("DATE PERIOD:" & vbTab & CDate(datefrom).ToString("MM-dd-yyyy") & " to " & CDate(dateto).ToString("MM-dd-yyyy"))

            sb.AppendLine()
            sb.AppendLine()

#Region "ZRead"
            If Not CBool2(excludeZRead) Then
                Dim zread = DownloadZRead(Common.WebRequest.Data("DownloadDataOnly"),
                                        Common.WebRequest.Data("FOWSTID"), Common.WebRequest.Data("DeviceID"),
                                        datefrom, dateto,
                                        datefrom, dateto,
                                        Common.WebRequest.Data("ForceNONVAT"), True, False)

                Dim ZHeader As Object = jss.Deserialize(Of Object)(zread)("d")("Header")("Rows")(0)
                Dim ZData As Object = jss.Deserialize(Of Object)(zread)("d")("Data")("Rows")(0)
                Dim ZDiscounts As Object = jss.Deserialize(Of Object)(zread)("d")("Discounts")("Rows")
                Dim ZOtherPayments As Object = jss.Deserialize(Of Object)(zread)("d")("OtherPayments")("Rows")
                netsales = CDec2(ZData("CurGrossSales")) - ((CDec2(ZData("TotalSCDiscount")) - CDec2(ZData("DiscountRefund"))) + CDec2(ZData("TotalSoloParentDiscount")) + CDec2(ZData("CurTotalDiscount"))) - CDec2(ZData("CurSalesReturnRefund")) - CDec2(ZData("CurSalesReturnExchange")) - CDec2(ZData("CurVATExempt")) + (CDec2(ZData("ChargeAmount")) + CDec2(ZData("CurServiceCharge")))
                sb.Append("#########################################")
                sb.AppendLine()
                sb.Append(vbTab & vbTab & "Z-READING")
                sb.AppendLine()
                sb.Append("#########################################")
                sb.AppendLine()
                sb.AppendLine()

                sb.Append("Gross Sales:" & vbTab & EJAccountingFormat(ZData("CurGrossSales")))
                sb.AppendLine()
                sb.Append("Discounts:" & vbTab & EJAccountingFormat(((CDec2(ZData("TotalSCDiscount")) - CDec2(ZData("DiscountRefund"))) + CDec2(ZData("TotalSoloParentDiscount")) + CDec2(ZData("CurTotalDiscount")))))
                sb.AppendLine()

                If ZDiscounts.Length > 0 Then
                    sb.AppendLine()
                    sb.Append("== DISCOUNT BREAKDOWN ==")
                    sb.AppendLine()
                    For Each discount As Object In ZDiscounts
                        sb.Append(" - " & discount("Code") & ": " & EJAccountingFormat(discount("DiscountAmount")))
                        sb.AppendLine()
                    Next
                    sb.Append("========================")
                    sb.AppendLine()
                    sb.AppendLine()
                End If

                sb.Append("VAT-Ex Tax:" & vbTab & EJAccountingFormat(CDec2(ZData("CurVATExempt"))))
                sb.AppendLine()
                sb.AppendLine()

                sb.Append(vbTab & vbTab & "SUMMARY OF ADJUSTMENTS")

                sb.AppendLine()
                sb.AppendLine()

                sb.Append("Refund:" & vbTab & vbTab & EJAccountingFormat(CDec2(ZData("CurSalesReturnRefund"))))
                sb.AppendLine()
                sb.Append("Return (SRV):" & vbTab & EJAccountingFormat(CDec2(ZData("CurSalesReturnExchange"))))
                sb.AppendLine()
                sb.Append("Net of Adj:" & vbTab & EJAccountingFormat(CDec2(ZData("CurSalesReturnRefund")) + CDec2(ZData("CurSalesReturnExchange"))))
                sb.AppendLine()
                sb.Append("-----------------------------------------")
                sb.AppendLine()
                sb.Append("Add Charges:" & vbTab & EJAccountingFormat(CDec2(ZData("ChargeAmount")) + CDec2(ZData("CurServiceCharge"))))
                sb.AppendLine()
                sb.Append("NET SALES:" & vbTab & EJAccountingFormat(netsales))
                sb.AppendLine()
                sb.AppendLine()

                sb.Append(vbTab & vbTab & "INVENTORY OF PAYMENTS")

                sb.AppendLine()
                sb.AppendLine()

                totalpayments += CDec2(ZData("CashAmount")) - CDec2(ZData("CashRefund"))
                sb.Append("Cash:" & vbTab & vbTab & EJAccountingFormat(CDec2(ZData("CashAmount")) - CDec2(ZData("CashRefund"))))
                sb.AppendLine()

                totalpayments += CDec2(ZData("CheckPaymentAmount")) + CDec2(ZData("CurPotatoCornerGC"))
                sb.Append("Check:" & vbTab & vbTab & EJAccountingFormat(CDec2(ZData("CheckPaymentAmount")) + CDec2(ZData("CurPotatoCornerGC"))))
                sb.AppendLine()

                totalpayments += CDec2(ZData("DebitCardAmount")) - CDec2(ZData("DebitRefund"))
                sb.Append("Debit Card:" & vbTab & EJAccountingFormat(CDec2(ZData("DebitCardAmount")) - CDec2(ZData("DebitRefund"))))
                sb.AppendLine()

                totalpayments += CDec2(ZData("CreditCardAmount")) - CDec2(ZData("CreditRefund"))
                sb.Append("Credit Card:" & vbTab & EJAccountingFormat(CDec2(ZData("CreditCardAmount")) - CDec2(ZData("CreditRefund"))))
                sb.AppendLine()

                totalpayments += CDec2(ZData("SRVAmount"))
                sb.Append("SRV:" & vbTab & vbTab & EJAccountingFormat(CDec2(ZData("SRVAmount"))))
                sb.AppendLine()

                totalpayments += CDec2(ZData("OnCreditAmount"))
                sb.Append("On Credit:" & vbTab & EJAccountingFormat(CDec2(ZData("OnCreditAmount"))))
                sb.AppendLine()

                For Each payment As Object In ZOtherPayments
                    totalpayments += CDec2(payment("Amount"))
                    sb.Append(payment("PaymentType") & ":" & IIf(payment("PaymentType").ToString().Count > 6, vbTab, vbTab & vbTab) & EJAccountingFormat(CDec2(payment("Amount"))))
                    sb.AppendLine()
                Next

                sb.Append("-----------------------------------------")
                sb.AppendLine()
                sb.Append("Total Payments:" & vbTab & EJAccountingFormat(CDec2(totalpayments)))
                sb.AppendLine()
                sb.Append("Adjustments (SRV Liability): " & vbTab & "-" & EJAccountingFormat(CDec2(ZData("CurSalesReturnExchange"))))
                sb.AppendLine()
                sb.Append("-----------------------------------------")
                sb.AppendLine()
                sb.Append("CURRENT STANDING:" & vbTab & EJAccountingFormat(CDec2(totalpayments) - CDec2(ZData("CurSalesReturnExchange"))))
                sb.AppendLine()
                sb.AppendLine()

                sb.Append(vbTab & vbTab & "SALES GROUPED BY VAT TYPE")

                sb.AppendLine()
                sb.AppendLine()

                sb.Append("VAT-able Sales:" & vbTab & EJAccountingFormat(CDec2(ZData("VATableSales"))))
                sb.AppendLine()
                sb.Append("VAT Amount:" & vbTab & EJAccountingFormat(CDec2(ZData("VATAmount"))))
                sb.AppendLine()
                sb.Append("VAT-Exempt:" & vbTab & EJAccountingFormat(CDec2(ZData("VATExempt"))))
                sb.AppendLine()
                sb.Append("Non-VAT:" & vbTab & EJAccountingFormat(CDec2(ZData("NonVAT"))))
                sb.AppendLine()
                sb.Append("Zero-rated VAT:" & vbTab & EJAccountingFormat(CDec2(0)))
                sb.AppendLine()
                sb.Append("-----------------------------------------")
                sb.AppendLine()
                sb.Append(vbTab & vbTab & EJAccountingFormat(CDec2(ZData("VATableSales")) + CDec2(ZData("VATAmount")) + CDec2(ZData("VATExempt")) + CDec2(ZData("NonVAT"))))

                sb.AppendLine()
                sb.AppendLine()

                sb.Append(vbTab & vbTab & "OTHER INFORMATION")

                sb.AppendLine()
                sb.AppendLine()

                sb.Append("Total Trans. Count:" & vbTab & (CDec2(ZData("CurTransCountInv")) + CDec2(ZData("CurTransCountMemoReturnExchange")) + CDec2(ZData("CurTransCountMemoRefund"))))
                sb.AppendLine()
                sb.Append("Sales Trans. Count:" & vbTab & CDec2(ZData("CurTransCountInv")))
                sb.AppendLine()
                sb.Append("Sales Item Count:" & vbTab & CDec2(ZData("CurTransInvItemCount")))
                sb.AppendLine()
                sb.Append("SRV Trans. Count:" & vbTab & CDec2(ZData("CurTransCountMemoReturnExchange")))
                sb.AppendLine()
                sb.Append("SRV Item CNT:" & vbTab & vbTab & CDec2(ZData("CurSalesReturnExchangeItemCount")))
                sb.AppendLine()
                sb.Append("Refund Trans. Count:" & vbTab & CDec2(ZData("CurTransCountMemoRefund")))
                sb.AppendLine()
                sb.Append("Refund Item Count:" & vbTab & CDec2(ZData("CurSalesRefundItemCount")))
                sb.AppendLine()
                sb.Append("Customer Count:" & vbTab & vbTab & CDec2(ZData("CurTransCountInv")))
                sb.AppendLine()
                sb.Append("Prev. End SI No.:" & vbTab & CDec2(ZData("PrevEndInvoiceNo")))
                sb.AppendLine()
                sb.Append("Beg. SI No.:" & vbTab & vbTab & CDec2(ZData("CurBegInvoiceNo")))
                sb.AppendLine()
                sb.Append("End SI No.:" & vbTab & vbTab & CDec2(ZData("CurEndInvoiceNo")))
                sb.AppendLine()
                sb.Append("Old Grand Total:" & vbTab & EJAccountingFormat(CDec2(ZData("PrevGrossSales")) - (CDec2(ZData("PrevTotalSCDiscount")) + CDec2(ZData("PrevSoloParentDiscount")) + CDec2(ZData("PrevTotalDiscount"))) - CDec2(ZData("PrevSalesReturnRefund")) - CDec2(ZData("PrevSalesReturnExchange")) - CDec2(ZData("PrevVATExempt")) + (CDec2(ZData("PrevChargeAmount")) + CDec2(ZData("PrevServiceCharge")))))
                sb.AppendLine()
                sb.Append("New Grand Total:" & vbTab & EJAccountingFormat((CDec2(ZData("PrevGrossSales")) - (CDec2(ZData("PrevTotalSCDiscount")) + CDec2(ZData("PrevSoloParentDiscount")) + CDec2(ZData("PrevTotalDiscount"))) - CDec2(ZData("PrevSalesReturnRefund")) - CDec2(ZData("PrevSalesReturnExchange")) - CDec2(ZData("PrevVATExempt")) + (CDec2(ZData("PrevChargeAmount")) + CDec2(ZData("PrevServiceCharge")))) + (CDec2(ZData("CurGrossSales")) - (CDec2(ZData("TotalSCDiscount")) + CDec2(ZData("TotalSoloParentDiscount")) + CDec2(ZData("CurTotalDiscount"))) - CDec2(ZData("CurSalesReturnRefund")) - CDec2(ZData("CurSalesReturnExchange")) - CDec2(ZData("CurVATExempt")) + (CDec2(ZData("ChargeAmount")) + CDec2(ZData("CurServiceCharge"))))))
                sb.AppendLine()
                sb.Append("Z Counter:" & vbTab & vbTab & CDec2(ZData("ZCount")))

                sb.AppendLine()
                sb.AppendLine()
            End If
#End Region

            If CDate(datefrom).TimeOfDay.TotalSeconds = 0 Then
                datefrom = datefrom & " 12:00:00 AM"
            End If

            If CDate(dateto).TimeOfDay.TotalSeconds = 0 Then
                dateto = dateto & " 11:59:59 PM"
            End If

#Region "XData"
            If Not CBool2(excludeXRead) Then
                Dim cashierIDs As DataTable = Common.WebRequest.ProData.Execute("SELECT DISTINCT UserID FROM TransInvoice" & vbCrLf &
                    "   WHERE FowstID = " & ReadyString(fowstid) & vbCrLf &
                    "   AND DeviceID = " & ReadyString(deviceid) & vbCrLf &
                    "   AND InvoiceDate BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf)
                Dim megaworld As New MallLessorMegaworld 'reuse

                sb.Append("#########################################")
                sb.AppendLine()
                sb.Append(vbTab & vbTab & "X-READ PER CASHIER")
                sb.AppendLine()
                sb.Append("#########################################")
                sb.AppendLine()
                sb.AppendLine()

                For Each cashier As DataRow In cashierIDs.Rows
                    Dim cashierid As String = cashier("UserID").ToString()
                    Dim XData As DataRow = getCashierSalesTransactionSummary(cashierid, "Day", datefrom, dateto, "").Rows(0)
                    Dim XOtherPayments As DataTable = New MallLessorMegaworld().getOtherPaymentTypes(Common.WebRequest.ProData, CDate(datefrom), CDate(dateto), fowstid, deviceid, cashierid)
                    netsales = CDec2(XData("CurGrossSales")) - (CDec2(XData("CurSalesDiscount")) + CDec2(XData("CurSCDiscount"))) - CDec2(XData("CurSalesReturnRefund")) - CDec2(XData("CurSalesReturnExchange")) - CDec2(XData("CurVATExempt")) + (CDec2(XData("ChargeAmount")) + CDec2(XData("CurServiceCharge")))
                    'Dim CashRefund = (CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "cash", cashierid)))
                    'Dim OtherPaymentRefund = (CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "otherpayment", cashierid)))
                    'Dim DiscountRefund = (CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "discount", cashierid)))
                    'Dim DebitRefund = (CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "debit", cashierid)))
                    'Dim CreditRefund = (CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "credit", cashierid)))

                    sb.Append("Cashier Name:" & vbTab & XData("CashierName"))
                    sb.AppendLine()
                    sb.AppendLine()

                    sb.Append("Gross Sales:" & vbTab & EJAccountingFormat(XData("CurGrossSales")))
                    sb.AppendLine()
                    sb.Append("Discounts:" & vbTab & EJAccountingFormat((CDec2(XData("CurSalesDiscount")) + CDec2(XData("CurSCDiscount")))))
                    sb.AppendLine()
                    sb.Append("VAT-Ex Tax:" & vbTab & EJAccountingFormat(CDec2(XData("CurVATExempt"))))
                    sb.AppendLine()
                    sb.AppendLine()

                    sb.Append(vbTab & vbTab & "SUMMARY OF ADJUSTMENTS")

                    sb.AppendLine()
                    sb.AppendLine()

                    sb.Append("Refund:" & vbTab & vbTab & EJAccountingFormat(CDec2(XData("CurSalesReturnRefund"))))
                    sb.AppendLine()
                    sb.Append("Return (SRV):" & vbTab & EJAccountingFormat(CDec2(XData("CurSalesReturnExchange"))))
                    sb.AppendLine()
                    sb.Append("Net of Adj:" & vbTab & EJAccountingFormat(CDec2(XData("CurSalesReturnRefund")) + CDec2(XData("CurSalesReturnExchange"))))
                    sb.AppendLine()
                    sb.Append("-----------------------------------------")
                    sb.AppendLine()
                    sb.Append("Add Charges:" & vbTab & EJAccountingFormat(CDec2(XData("ChargeAmount")) + CDec2(XData("CurServiceCharge"))))
                    sb.AppendLine()
                    sb.Append("Net Sales:" & vbTab & EJAccountingFormat(netsales))
                    sb.AppendLine()
                    sb.AppendLine()

                    sb.Append(vbTab & vbTab & "INVENTORY OF PAYMENTS")

                    sb.AppendLine()
                    sb.AppendLine()

                    totalpayments = 0

                    totalpayments += CDec2(XData("CashAmount"))
                    sb.Append("Cash:" & vbTab & vbTab & EJAccountingFormat(CDec2(XData("CashAmount"))))
                    sb.AppendLine()

                    totalpayments += CDec2(XData("CheckPaymentAmount")) + CDec2(XData("CurPotatoCornerGC"))
                    sb.Append("Check:" & vbTab & vbTab & EJAccountingFormat(CDec2(XData("CheckPaymentAmount")) + CDec2(XData("CurPotatoCornerGC"))))
                    sb.AppendLine()

                    totalpayments += CDec2(XData("DebitCardAmount"))
                    sb.Append("Debit Card:" & vbTab & EJAccountingFormat(CDec2(XData("DebitCardAmount"))))
                    sb.AppendLine()

                    totalpayments += CDec2(XData("CreditCardAmount"))
                    sb.Append("Credit Card:" & vbTab & EJAccountingFormat(CDec2(XData("CreditCardAmount"))))
                    sb.AppendLine()

                    totalpayments += CDec2(XData("SRVAmount"))
                    sb.Append("SRV:" & vbTab & vbTab & EJAccountingFormat(CDec2(XData("SRVAmount"))))
                    sb.AppendLine()

                    totalpayments += CDec2(XData("OnCreditAmount"))
                    sb.Append("On Credit:" & vbTab & EJAccountingFormat(CDec2(XData("OnCreditAmount"))))
                    sb.AppendLine()

                    For Each payment As DataRow In XOtherPayments.Rows
                        totalpayments += CDec2(payment("Amount"))
                        sb.Append(payment("PaymentType") & ":" & IIf(payment("PaymentType").ToString().Count > 6, vbTab, vbTab & vbTab) & EJAccountingFormat(CDec2(payment("Amount"))))
                        sb.AppendLine()
                    Next

                    sb.Append("-----------------------------------------")
                    sb.AppendLine()
                    sb.Append("Total Payments:" & vbTab & EJAccountingFormat(CDec2(totalpayments)))
                    sb.AppendLine()
                    sb.Append("Adjustments (SRV Liability): " & vbTab & "-" & EJAccountingFormat(CDec2(XData("CurSalesReturnExchange"))))
                    sb.AppendLine()
                    sb.Append("-----------------------------------------")
                    sb.AppendLine()
                    sb.Append("CURRENT STANDING:" & vbTab & EJAccountingFormat(CDec2(totalpayments) - CDec2(XData("CurSalesReturnExchange"))))

                    sb.AppendLine()
                    sb.AppendLine()

                    sb.Append(vbTab & vbTab & "SALES GROUPED BY VAT TYPE")

                    sb.AppendLine()
                    sb.AppendLine()

                    Dim dblVAT As Decimal = (CDec2(XData("CurTotalVatablePortionOfSCDTransaction")) + CDec2(XData("CurTotalVATSales"))) - CDec2(XData("CurTotalVATSalesSalesReturn"))
                    Dim dblVATAmount As Decimal = (CDec2(XData("CurTotalVatAmountPortionOfSCDTransaction")) + CDec2(XData("CurTotalVATAmount"))) - CDec2(XData("CurTotalVATAmountSalesReturn"))
                    Dim dblNonVAT As Decimal = CDec2(XData("CurNoTAXSales"))
                    Dim dblVATExempt As Decimal = CDec2(XData("CurVATExemptSales"))
                    Dim dblZeroRatedVAT As Decimal = CDec2(XData("CurZeroRatedVATSales"))
                    Dim dblTotalVAT As Decimal = dblVAT + dblVATAmount + dblVATExempt + dblNonVAT + dblZeroRatedVAT

                    sb.Append("VAT-able Sales:" & vbTab & EJAccountingFormat(dblVAT))
                    sb.AppendLine()
                    sb.Append("VAT Amount:" & vbTab & EJAccountingFormat(dblVATAmount))
                    sb.AppendLine()
                    sb.Append("VAT-Exempt:" & vbTab & EJAccountingFormat(dblVATExempt))
                    sb.AppendLine()
                    sb.Append("Non-VAT:" & vbTab & EJAccountingFormat(dblNonVAT))
                    sb.AppendLine()
                    sb.Append("Zero-rated VAT:" & vbTab & EJAccountingFormat(dblZeroRatedVAT))
                    sb.AppendLine()
                    sb.Append("-----------------------------------------")
                    sb.AppendLine()
                    sb.Append(vbTab & vbTab & EJAccountingFormat(dblTotalVAT))
                    sb.AppendLine()
                    sb.AppendLine()
                    sb.Append("-----------------------------------------")
                    sb.AppendLine()
                    sb.AppendLine()
                Next
            End If
#End Region

#Region "RETURN / REFUND"
            If Not CBool2(excludeReturns) Then
                pSql = " SELECT * " & vbCrLf &
                "   , T.OrigQtyInInvoice - T.TotalReturnedQtyForThisInvoice as NetQtyInOrigInvoice" & vbCrLf &
                "   FROM (SELECT " & vbCrLf &
                "       sr.LastUpdated as SRDate, " & vbCrLf &
                "       fowst.Name as FowstName, " & vbCrLf &
                "       ppol.Surname + ', ' + ppol.FirstName TransactedBy, " & vbCrLf &
                "       pim.ProductNo + ' ' + pim.ProductName as ItemDescription, " & vbCrLf &
                "       CASE WHEN sr.GrossSrvAmount = 0 THEN 'Refund' ELSE 'Merchandise Exchange' END SRType, " & vbCrLf &
                "       memo.QuantityReturned as ReturnedQtyForThisTrans, " & vbCrLf &
                "       memo.TotalAmount as TotalValueOfReturnedQty, " & vbCrLf &
                "       (SELECT SUM(QuantityReturned) FROM TransMemoSalesReturn WHERE ProductID = memo.ProductID AND InvoiceID = sr.SourceInvoiceID GROUP BY ProductID) as TotalReturnedQtyForThisInvoice, " & vbCrLf &
                "       (SELECT SUM(Quantity) FROM TransInvoiceItems WHERE InvoiceID = sr.SourceInvoiceID) as OrigQtyInInvoice, " & vbCrLf &
                "       inv.InvoiceNumber, " & vbCrLf &
                "       inv.InvoiceDate, SrvNum, Pin, ISNULL(sr.DeviceFowstTransactionNumber, 0) as TransactionNumber, " & vbCrLf &
                "       MobizLibraryOrganization.CompanyName, ISNULL(MobizLibraryUserDefinedIndustry.Name,'') IndustryClass " & vbCrLf &
                "       FROM TransMemoSalesReturn as memo " & vbCrLf &
                "       INNER JOIN TransSalesReturnLedger as sr ON sr.SourceMemoID = memo.MemoID " & vbCrLf &
                "       INNER JOIN MobizLibraryPIMProduct as pim ON memo.ProductID = pim.ID " & vbCrLf &
                "       INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = sr.FowstID " & vbCrLf &
                "       INNER join MobizLibraryPeople as ppol on ppol.ID = sr.UserID " & vbCrLf &
                "       INNER join TransInvoice as inv on inv.ID = sr.SourceInvoiceID " & vbCrLf &
                "       INNER JOIN MobizLibraryOrganization on MobizLibraryOrganization.ID = inv.CustomerID " & vbCrLf &
                "       LEFT JOIN MobizLibraryUserDefinedIndustry on MobizLibraryUserDefinedIndustry.ID = MobizLibraryOrganization.UserDefinedIndustryID " & vbCrLf &
                "       WHERE sr.LastUpdated BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf

                pSql &= " AND sr.FowstID = " & ReadyString(fowstid) & " AND sr.DeviceID = " & ReadyString(deviceid) & vbCrLf

                pSql &= "       GROUP BY sr.LastUpdated, fowst.Name, ppol.Surname, ppol.FirstName, pim.ProductNo, pim.ProductName, " & vbCrLf &
                    "           SrvNum, Pin, sr.DeviceFowstTransactionNumber, memo.QuantityReturned, memo.TotalAmount, sr.SourceInvoiceID, " & vbCrLf &
                    "           inv.InvoiceNumber, inv.InvoiceDate, sr.GrossSrvAmount, memo.ProductID, MobizLibraryUserDefinedIndustry.Name, MobizLibraryOrganization.CompanyName " & vbCrLf &
                    "   ) as T " & vbCrLf

                pSql &= " ORDER BY SRType, SRDate DESC "

                Dim refunds As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim refundtitle As Boolean = False
                Dim returntitle As Boolean = False

                For Each refund As DataRow In refunds.Rows
                    If refund("SRType") = "Refund" And Not refundtitle Then
                        sb.Append("#########################################")
                        sb.AppendLine()
                        sb.Append(vbTab & vbTab & "REFUNDS")
                        sb.AppendLine()
                        sb.Append("#########################################")
                        sb.AppendLine()
                        sb.AppendLine()

                        refundtitle = True
                    ElseIf refund("SRType") = "Merchandise Exchange" And Not returntitle Then
                        sb.Append("#########################################")
                        sb.AppendLine()
                        sb.Append(vbTab & vbTab & "RETURNS")
                        sb.AppendLine()
                        sb.Append("#########################################")
                        sb.AppendLine()
                        sb.AppendLine()

                        returntitle = True
                    End If

                    sb.Append("Date Refunded:" & vbTab & Format(CDate(refund("SRDate")), "MMM dd, yyyy hh:mm tt"))
                    sb.AppendLine()
                    sb.Append("Transaction #:" & vbTab & Format(refund("TransactionNumber"), "000000000000"))
                    sb.AppendLine()
                    sb.Append("Ref. Invoice #:" & vbTab & Format(refund("InvoiceNumber"), "000000000000"))
                    sb.AppendLine()
                    sb.Append("Ref. Inv. Date:" & vbTab & Format(CDate(refund("InvoiceDate")), "MMM dd, yyyy hh:mm tt"))
                    sb.AppendLine()

                    If refund("SRType") = "Refund" Then
                        sb.Append("Refunded Item:" & vbTab & refund("ItemDescription"))
                        sb.AppendLine()
                        sb.Append("Refunded Qty:" & vbTab & CInt2(refund("ReturnedQtyForThisTrans")))
                        sb.AppendLine()
                        sb.Append("Refunded Amt.:" & vbTab & EJAccountingFormat(refund("TotalValueOfReturnedQty")))
                        sb.AppendLine()
                        sb.AppendLine()
                        sb.Append("-----------------------------------------")
                        sb.AppendLine()
                        sb.AppendLine()
                    ElseIf refund("SRType") = "Merchandise Exchange" Then
                        sb.Append("SRV No.:" & vbTab & CDec2(refund("SrvNum")))
                        sb.AppendLine()
                        sb.Append("Returned Item:" & vbTab & refund("ItemDescription"))
                        sb.AppendLine()
                        sb.Append("Returned Qty:" & vbTab & CInt2(refund("ReturnedQtyForThisTrans")))
                        sb.AppendLine()
                        sb.Append("Returned Amt.:" & vbTab & EJAccountingFormat(refund("TotalValueOfReturnedQty")))
                        sb.AppendLine()
                        sb.AppendLine()
                        sb.Append("-----------------------------------------")
                        sb.AppendLine()
                        sb.AppendLine()
                    End If
                Next
            End If
#End Region

#Region "VOID"
            'DILI NA NI APIL SINCE DILI SYA SALES - by Arnie 32226
            'pSql = " SELECT void.*, pim.ProductNo, pim.ProductName, ISNULL(inv.InvoiceNumber, 0) as InvoiceNumber, ISNULL(inv.InvoiceDate, '') as InvoiceDate " & vbCrLf &
            '        "   FROM TransInvoiceItemsVoided as void " & vbCrLf &
            '        "   INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = void.ProductID " & vbCrLf &
            '        "   LEFT JOIN TransInvoice as inv ON inv.ID = void.InvoiceID " & vbCrLf &
            '        "   WHERE void.FowstID = " & ReadyString(fowstid) & vbCrLf &
            '        "   AND void.DeviceID = " & ReadyString(deviceid) & vbCrLf &
            '        "   AND void.DateTimeRemoved BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf

            'Dim voids As DataTable = Common.WebRequest.ProData.Execute(pSql)

            'sb.AppendLine()
            'sb.Append("#########################################")
            'sb.AppendLine()
            'sb.Append(vbTab & vbTab & "VOID ITEMS")
            'sb.AppendLine()
            'sb.Append("#########################################")
            'sb.AppendLine()
            'sb.AppendLine()

            'If voids.Rows.Count > 0 Then
            '    Dim ctr As Integer = 1
            '    For Each void As DataRow In voids.Rows
            '        sb.Append("Line #:" & vbTab & vbTab & ctr)
            '        sb.AppendLine()
            '        sb.Append("Void #:" & vbTab & vbTab & void("VoidNumber"))
            '        sb.AppendLine()
            '        sb.Append("Ref. Invoice #:" & vbTab & IIf(void("InvoiceNumber") = 0, "", Format(void("InvoiceNumber"), "000000000000")))
            '        sb.AppendLine()
            '        sb.Append("Ref. Inv. Date:" & vbTab & IIf(void("InvoiceNumber") = 0, "", Format(CDate(void("InvoiceDate")), "MMM dd, yyyy hh:mm tt")))
            '        sb.AppendLine()
            '        sb.Append("Product No.:" & vbTab & void("ProductNo"))
            '        sb.AppendLine()
            '        sb.Append("Product Name:" & vbTab & void("ProductName"))
            '        sb.AppendLine()
            '        sb.Append("Quantity:" & vbTab & CInt2(void("Quantity")))
            '        sb.AppendLine()
            '        sb.Append("Price:" & vbTab & vbTab & EJAccountingFormat(void("UnitPrice")))
            '        sb.AppendLine()
            '        sb.Append("Total:" & vbTab & vbTab & EJAccountingFormat(CDec2(void("Quantity")) * CDec2(void("UnitPrice"))))
            '        sb.AppendLine()
            '        sb.AppendLine()
            '        sb.Append("-----------------------------------------")
            '        sb.AppendLine()
            '        sb.AppendLine()

            '        ctr += 1
            '    Next
            'Else
            '    sb.Append("No records found for the period")
            '    sb.AppendLine()
            'End If

#End Region

#Region "INVOICES"
            If Not CBool2(excludeInvoice) Then
                pSql = " IF OBJECT_ID('tempdb..##EJ_TransInvoice') IS NOT NULL DROP TABLE ##EJ_TransInvoice " & vbCrLf &
                    "   SELECT ID, InvoiceDate, InvoiceNumber, ISNULL(DeviceFowstTransactionNumber, 0) as DeviceFowstTransactionNumber, TotalAmount, Vatable, VAT, ISNULL(VATExempt, 0) as VATExempt, ISNULL(ForegoneVAT, 0) as ForegoneVAT, SCDiscount, SCDiscountExempt, GrossSalesToSC, GrossSalesToRegDiner, ISNULL(PhNatTeamDiscount, 0) as PhNatTeamDiscount, ISNULL(SoloParentDiscount, 0) as SoloParentDiscount, Charges.Amount as OtherCharges, " & vbCrLf &
                    "       CASE WHEN SCDiscount > 0 OR PhNatTeamDiscount > 0 OR SoloParentDiscount > 0 THEN " & vbCrLf &
                    "           (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + PersonFullName " & vbCrLf &
                    "               FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                    "               INNER JOIN MobizLibraryOrganization as org ON org.ID = CustomerID " & vbCrLf &
                    "               WHERE InvoiceID = inv.ID " & vbCrLf &
                    "               FOR XML PATH('')" & vbCrLf &
                    "           ), 1, 1,''))" & vbCrLf &
                    "       ELSE (SELECT ISNULL(PersonFullName, CompanyName) FROM MobizLibraryOrganization WHERE ID = inv.CustomerID) END as CustomerName, " & vbCrLf &
                    "       (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + ISNULL(PersonSeniorCitizensIDNo, '') + ISNULL(PersonPWDIDNo, '') + ISNULL(PersonOtherIDNo, '') + ISNULL(PersonPassportNo, '') + ISNULL(PersonDriversLicenseNo, '') + ISNULL(PersonSSSNo, '') + ISNULL(PersonGSISIDNo, '') + ISNULL(PersonPRCIDNo, '') + ISNULL(PersonPostalIDNo, '') + ISNULL(PersonVotersIDNo, '')  " & vbCrLf &
                    "           FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                    "           INNER JOIN MobizLibraryOrganization as org ON org.ID = CustomerID " & vbCrLf &
                    "           WHERE InvoiceID = inv.ID " & vbCrLf &
                    "           FOR XML PATH('')" & vbCrLf &
                    "       ), 1, 1,'')) as CustomerIDNo, " & vbCrLf &
                    "       CASE WHEN ISNULL(SCDiscount, 0) = 0 AND ISNULL(PhNatTeamDiscount, 0) = 0 AND ISNULL(SoloParentDiscount, 0) = 0 THEN (SELECT TOP 1 pay.DiscountAmount FROM TransCustomerPaymentInvoices as payinv INNER JOIN TransCustomerPayment as pay ON pay.ID = payinv.CustomerPaymentID WHERE InvoiceID = inv.ID) ELSE 0 END as OtherDiscount " & vbCrLf &
                    "       , ISNULL(CONVERT(varchar(MAX), discount.DiscountTypeID), '') as DiscountTypeID, ISNULL(discount.DiscountPercentage, '0') as DiscountPercentage, ISNULL(discount.DiscountType, '') as DiscountType " & vbCrLf &
                    "   INTO ##EJ_TransInvoice " & vbCrLf &
                    "   FROM TransInvoice as inv " & vbCrLf &
                    "   --OUTER APPLY( " & vbCrLf &
                    "   --    SELECT CustomerID , PersonFullName, ISNULL(PersonSeniorCitizensIDNo, '') + ISNULL(PersonPWDIDNo, '') + ISNULL(PersonOtherIDNo, '') + ISNULL(PersonPassportNo, '') + ISNULL(PersonDriversLicenseNo, '') + ISNULL(PersonSSSNo, '') + ISNULL(PersonGSISIDNo, '') + ISNULL(PersonPRCIDNo, '') + ISNULL(PersonPostalIDNo, '') + ISNULL(PersonVotersIDNo, '') as CustomerIDNo " & vbCrLf &
                    "   --    FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                    "   --    INNER JOIN MobizLibraryOrganization as org ON org.ID = CustomerID " & vbCrLf &
                    "   --    WHERE InvoiceID = inv.ID " & vbCrLf &
                    "   --) as customer " & vbCrLf &
                    "   OUTER APPLY( " & vbCrLf &
                    "       SELECT ISNULL(SUM(ChargeAmount), 0) as Amount " & vbCrLf &
                    "       FROM TransInvoiceCharges " & vbCrLf &
                    "       WHERE InvoiceID = inv.ID " & vbCrLf &
                    "   ) as Charges " & vbCrLf &
                    "   OUTER APPLY( " & vbCrLf &
                    "       SELECT DiscountTypeID, DiscountPercentage, DiscountType " & vbCrLf &
                    "       FROM LibraryDiscountSettings as disc " & vbCrLf &
                    "       INNER JOIN TransInvoiceDiscount ON DiscountTypeID = disc.ID " & vbCrLf &
                    "       WHERE InvoiceID = inv.ID " & vbCrLf &
                    "   ) as discount " & vbCrLf &
                    "   WHERE FowstID = " & ReadyString(fowstid) & vbCrLf &
                    "   AND DeviceID = " & ReadyString(deviceid) & vbCrLf &
                    "   AND InvoiceDate BETWEEN " & ReadyString(datefrom) & " AND " & ReadyString(dateto) & vbCrLf &
                    "   ORDER BY InvoiceDate DESC " & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                pSql = " IF OBJECT_ID('tempdb..##EJ_TransCustomerPaymentInvoices') IS NOT NULL DROP TABLE ##EJ_TransCustomerPaymentInvoices " & vbCrLf &
                        "   SELECT * INTO ##EJ_TransCustomerPaymentInvoices FROM( " & vbCrLf &
                        "   SELECT payinv.CustomerPaymentID, InvoiceID, ISNULL(CashAmount, 0) + ISNULL(CreditCardAmount, 0) + ISNULL(DebitCardAmount, 0) + ISNULL(SRVAmount, 0) + ISNULL(OnCreditAmount, 0) as Amount, " & vbCrLf &
                        "       CASE WHEN CashAmount > 0 THEN  'Cash' " & vbCrLf &
                        "           WHEN CreditCardAmount > 0 THEN 'Credit Card' " & vbCrLf &
                        "           WHEN DebitCardAmount > 0 THEN 'Debit Card' " & vbCrLf &
                        "           WHEN SRVAmount > 0 THEN 'SRV' " & vbCrLf &
                        "           WHEN OnCreditAmount > 0 THEN 'On Credit' " & vbCrLf &
                        "       END AS PaymentType " & vbCrLf &
                        "   , '' as CardNumber " & vbCrLf &
                        "   , STUFF((SELECT ', ' + ISNULL(CONVERT(varchar(max), SrvNum),0) FROM TransSalesReturnLedger WHERE SourceMemoID = (SELECT TransCustomerPaymentSRV.MemoID " & vbCrLf &
                        "       FROM TransCustomerPaymentSRV " & vbCrLf &
                        "       JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentSRV.CustomerPaymentID " & vbCrLf &
                        "       WHERE TransCustomerPaymentInvoices.InvoiceID = payinv.InvoiceID AND DestinationPaymentID IS NULL) FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') as SRVNo" & vbCrLf &
                        "   FROM TransCustomerPaymentInvoices as payinv " & vbCrLf &
                        "   INNER JOIN TransCustomerPayment as pay ON pay.ID = payinv.CustomerPaymentID " & vbCrLf &
                        "   WHERE InvoiceID IN (SELECT ID FROM ##EJ_TransInvoice) " & vbCrLf &
                        "   UNION ALL " & vbCrLf & vbCrLf &
                        "   SELECT payinv.CustomerPaymentID, InvoiceID, ISNULL(otherpay.Amount, 0) as Amount, " & vbCrLf &
                        "       otherpay.PaymentType AS PaymentType " & vbCrLf &
                        "   , ISNULL(STUFF((SELECT ', ' + ISNULL(CONVERT(varchar(max), CardNumber),0) FROM TransCustomerPaymentOther WHERE ID = otherpay.ID FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), '') as CardNumber " & vbCrLf &
                        "   , '' as SRVNo" & vbCrLf &
                        "   FROM TransCustomerPaymentInvoices as payinv " & vbCrLf &
                        "   INNER JOIN TransCustomerPaymentOther as otherpay ON otherpay.CustomerPaymentID = payinv.CustomerPaymentID " & vbCrLf &
                        "   WHERE InvoiceID IN (SELECT ID FROM ##EJ_TransInvoice) " & vbCrLf &
                        "   ) as payments " & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                pSql = " IF OBJECT_ID('tempdb..##EJ_TransInvoiceItems') IS NOT NULL DROP TABLE ##EJ_TransInvoiceItems " & vbCrLf &
                        "   SELECT TransInvoiceItems.ID, InvoiceID, ProductID, ProductNo, ProductName, CONVERT(int, OrderedQuantity) as OrderedQuantity, UnitPrice, SubTotal, UnitMeasure " & vbCrLf &
                        "   INTO ##EJ_TransInvoiceItems " & vbCrLf &
                        "   FROM TransInvoiceItems " & vbCrLf &
                        "   INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = ProductID " & vbCrLf &
                        "   WHERE InvoiceID IN (SELECT ID FROM ##EJ_TransInvoice) " & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                pSql = " IF OBJECT_ID('tempdb..##EJ_TransInvoiceCharges') IS NOT NULL DROP TABLE ##EJ_TransInvoiceCharges " & vbCrLf &
                    "   SELECT TransInvoiceCharges.InvoiceID, TransInvoiceCharges.ChargeType, ISNULL(ChargeAmount, 0) + ISNULL(TaxAmount, 0) + ISNULL(TipAmount, 0) as Amount " & vbCrLf &
                    "   INTO ##EJ_TransInvoiceCharges " & vbCrLf &
                    "   FROM TransInvoiceCharges " & vbCrLf &
                    "   WHERE InvoiceID IN (SELECT ID FROM ##EJ_TransInvoice) " & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                pSql = " IF OBJECT_ID('tempdb..##EJ_TransInvoiceItemsModifier') IS NOT NULL DROP TABLE ##EJ_TransInvoiceItemsModifier " & vbCrLf &
                        " SELECT InvoiceItemID, pim.ProductName, CONVERT(int, OrderedQty) as OrderedQty, ModifierPrice, SubTotal, modif.PremiumPrice " & vbCrLf &
                        " INTO ##EJ_TransInvoiceItemsModifier " & vbCrLf &
                        " FROM TransInvoiceItemsModifier as modif " & vbCrLf &
                        " INNER JOIN LibraryItemModifierInventoryAffecting as itemmod ON itemmod.ID = modif.ModifierID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = itemmod.ProductID " & vbCrLf &
                        " WHERE InvoiceItemID IN (SELECT ID FROM ##EJ_TransInvoiceItems) " & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                sb.AppendLine()
                sb.Append("#########################################")
                sb.AppendLine()
                sb.Append(vbTab & vbTab & "INVOICES")
                sb.AppendLine()
                sb.Append("#########################################")
                sb.AppendLine()

                Dim invoices As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##EJ_TransInvoice ORDER BY InvoiceDate")

                For Each invoice As DataRow In invoices.Rows
                    sb.AppendLine()
                    sb.Append("Transaction No.:" & vbTab & Format(invoice("DeviceFowstTransactionNumber"), "000000000000"))
                    sb.AppendLine()
                    sb.Append("Sales Invoice No.:" & vbTab & Format(invoice("InvoiceNumber"), "000000000000"))
                    sb.AppendLine()
                    sb.Append("Date:" & vbTab & vbTab & Format(invoice("InvoiceDate"), "MMM dd, yyyy hh:mm tt"))
                    sb.AppendLine()
                    sb.Append("Customer:" & vbTab & invoice("CustomerName"))

                    If CDec2(invoice("SCDiscount")) Or CDec2(invoice("SoloParentDiscount")) > 0 Or CDec2(invoice("PhNatTeamDiscount")) > 0 > 0 Then
                        sb.AppendLine()
                        sb.Append("Cust. ID No.:" & vbTab & CStr2(invoice("CustomerIDNo")))
                    End If

                    Dim grosstotal As Decimal = 0
                    Dim items As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##EJ_TransInvoiceItems WHERE InvoiceID = " & ReadyString(invoice("ID")))

                    sb.AppendLine()

                    For Each item As DataRow In items.Rows
                        grosstotal += item("SubTotal")
                        sb.AppendLine()
                        sb.Append(item("ProductName"))
                        sb.AppendLine()
                        sb.Append(vbTab & item("OrderedQuantity") & " x " & EJAccountingFormat(item("UnitPrice")) & " = " & EJAccountingFormat(item("OrderedQuantity") * item("UnitPrice")))
                        sb.AppendLine()

                        Dim modifiers As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##EJ_TransInvoiceItemsModifier WHERE InvoiceItemID = " & ReadyString(item("ID")))

                        For Each modifier As DataRow In modifiers.Rows
                            grosstotal += modifier("SubTotal")
                            sb.Append("+ " & modifier("ProductName"))
                            sb.AppendLine()
                            sb.Append(vbTab & modifier("OrderedQty") & " x " & EJAccountingFormat(modifier("ModifierPrice")))

                            If CDec2(modifier("PremiumPrice")) > 0 Then
                                sb.Append(" (+" & EJAccountingFormat(modifier("PremiumPrice")) & " Premium Price)")
                            End If

                            sb.Append(" = " & EJAccountingFormat(modifier("SubTotal")))
                            sb.AppendLine()
                        Next
                    Next

                    sb.AppendLine()

                    sb.Append("Gross:" & vbTab & vbTab & EJAccountingFormat(grosstotal))
                    sb.AppendLine()

                    Dim scdiscex As Decimal = 0
                    If CDec2(invoice("SCDiscount")) > 0 Then
                        Dim disclabel = IIf(invoice("DiscountTypeID").ToString().ToLower() = "055a0a33-dd98-47ec-add3-8b2009f3944a", "SC", "PWD")

                        sb.AppendLine()
                        sb.Append("Gross to " & disclabel & ":" & vbTab & EJAccountingFormat(invoice("GrossSalesToSC"))) 'EJAccountingFormat(grosstotal - invoice("SCDiscountExempt")))
                        sb.AppendLine()
                        sb.Append("VAT-Ex for " & disclabel & ":" & vbTab & EJAccountingFormat(invoice("VATExempt")))
                        sb.AppendLine()
                        sb.Append(disclabel & " Disc. Rate:" & vbTab & "20%")
                        sb.AppendLine()
                        sb.Append(disclabel & " Disc. Amt.:" & vbTab & EJAccountingFormat(invoice("SCDiscount")))
                        sb.AppendLine()
                        sb.Append("Net Amt.:" & vbTab & EJAccountingFormat(invoice("VATExempt") - invoice("SCDiscount")))
                        sb.AppendLine()

                        grosstotal = invoice("VATExempt") - invoice("SCDiscount")

                        If CDec2(invoice("GrossSalesToRegDiner")) > 0 Then
                            sb.Append("Gross to Regular Customers:" & vbTab & EJAccountingFormat(invoice("GrossSalesToRegDiner")))
                            grosstotal = grosstotal + CDec2(invoice("GrossSalesToRegDiner"))
                            sb.AppendLine()
                        End If

                        If CDec2(invoice("SCDiscountExempt")) > 0 Then
                            scdiscex = CDec2(invoice("SCDiscountExempt"))
                            grosstotal += scdiscex
                            sb.Append(disclabel & " Disc. Ex.:" & vbTab & EJAccountingFormat(invoice("SCDiscountExempt")))
                            sb.AppendLine()
                        End If

                        sb.AppendLine()
                    End If

                    If CDec2(invoice("SoloParentDiscount")) > 0 Then
                        sb.AppendLine()
                        sb.Append("Less VAT:" & vbTab & EJAccountingFormat(invoice("ForegoneVAT")))
                        sb.AppendLine()
                        sb.Append("VAT-Ex:" & vbTab & vbTab & EJAccountingFormat(invoice("VATExempt")))
                        sb.AppendLine()
                        sb.Append("SOLOPAR Disc. Rate:" & vbTab & "10%")
                        sb.AppendLine()
                        sb.Append("SOLOPAR Disc. Amt.:" & vbTab & EJAccountingFormat(invoice("SoloParentDiscount")))
                        sb.AppendLine()
                        sb.Append("Net Amt.:" & vbTab & EJAccountingFormat(invoice("VATExempt") - invoice("SoloParentDiscount")))
                        sb.AppendLine()
                        sb.AppendLine()

                        grosstotal = invoice("VATExempt") - invoice("SoloParentDiscount")
                    End If

                    If CDec2(invoice("PhNatTeamDiscount")) > 0 Then
                        sb.AppendLine()
                        sb.Append("Less VAT:" & vbTab & EJAccountingFormat(invoice("VAT")))
                        sb.AppendLine()
                        sb.Append("VAT-Ex:" & vbTab & vbTab & EJAccountingFormat(grosstotal - invoice("VAT")))
                        sb.AppendLine()
                        sb.Append("PHNATTEAM Disc. Rate:" & vbTab & "20%")
                        sb.AppendLine()
                        sb.Append("PHNATTEAM Disc. Amt.:" & vbTab & EJAccountingFormat(invoice("PhNatTeamDiscount")))
                        sb.AppendLine()
                        sb.AppendLine()
                        sb.Append("Net Amt.:" & vbTab & EJAccountingFormat((grosstotal - invoice("VAT")) - invoice("PhNatTeamDiscount")))
                        sb.AppendLine()
                        sb.Append("Add VAT:" & vbTab & EJAccountingFormat(invoice("VAT")))
                        sb.AppendLine()
                        sb.AppendLine()

                        grosstotal = grosstotal - invoice("PhNatTeamDiscount")
                    End If

                    If CDec2(invoice("OtherDiscount")) > 0 Then
                        sb.AppendLine()
                        sb.Append(invoice("DiscountType") & ":" & vbTab & CDec2(invoice("DiscountPercentage")) & "%")
                        sb.AppendLine()
                        sb.Append("Discount Amount:" & vbTab & EJAccountingFormat(invoice("OtherDiscount")))
                        sb.AppendLine()

                        grosstotal = grosstotal - invoice("OtherDiscount")
                    End If

                    Dim charges As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##EJ_TransInvoiceCharges WHERE InvoiceID = " & ReadyString(invoice("ID")))

                    For Each charge As DataRow In charges.Rows
                        sb.Append(charge("ChargeType") & ":" & EJAccountingFormat(charge("Amount")))
                        sb.AppendLine()
                    Next

                    Dim amtdue As Decimal = grosstotal + invoice("OtherCharges")
                    sb.Append("Amount Due:" & vbTab & EJAccountingFormat(amtdue))
                    sb.AppendLine()

                    Dim payments As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##EJ_TransCustomerPaymentInvoices WHERE InvoiceID = " & ReadyString(invoice("ID")) & " ORDER BY CASE WHEN PaymentType = 'CASH' THEN 1 END, PaymentType")
                    totalpayments = 0

                    For Each payment As DataRow In payments.Rows
                        If CDec2(payment("Amount")) = 0 Then
                            Continue For
                        End If
                        totalpayments += payment("Amount")
                        sb.Append(payment("PaymentType") & ":" & IIf(payment("PaymentType").ToString().Count > 6, vbTab, vbTab & vbTab) & EJAccountingFormat(payment("Amount")))
                        sb.AppendLine()
                    Next

                    sb.Append("Change:" & vbTab & vbTab & EJAccountingFormat(totalpayments - amtdue))
                    sb.AppendLine()
                    sb.AppendLine()

                    sb.Append("VAT-able Sales:" & vbTab & EJAccountingFormat(IIf(CBool2(header("IsVAT")), invoice("Vatable"), 0)))
                    sb.AppendLine()
                    sb.Append("VAT Amount:" & vbTab & EJAccountingFormat(IIf(CBool2(header("IsVAT")), invoice("VAT"), 0)))
                    sb.AppendLine()
                    sb.Append("Non-VAT Amount:" & vbTab & EJAccountingFormat(IIf(CBool2(header("IsVAT")), 0, amtdue)))
                    sb.AppendLine()
                    sb.Append("VAT-Ex Sales:" & vbTab & EJAccountingFormat(IIf(CBool2(header("IsVAT")), invoice("VATExempt"), 0)))
                    sb.AppendLine()
                    sb.Append("VAT-Ex Tax:" & vbTab & EJAccountingFormat(IIf(CBool2(header("IsVAT")), invoice("ForegoneVAT"), 0)))
                    sb.AppendLine()
                    sb.Append("Zero-rated VAT:" & vbTab & EJAccountingFormat(0))
                    sb.AppendLine()

                    sb.AppendLine()
                    sb.AppendLine()
                    sb.Append("-----------------------------------------")
                    sb.AppendLine()
                    sb.AppendLine()
                Next
            End If
#End Region
            Dim filename As String = Regex.Replace(header("OrgBranchName") & "_eJournal_" & IIf(Format(CDate(datefrom), "MM-dd-yyyy") = Format(CDate(dateto), "MM-dd-yyyy"), Format(CDate(datefrom), "MM-dd-yyyy"), Format(CDate(datefrom), "MM-dd-yyyy") & "_to_" & Format(CDate(dateto), "MM-dd-yyyy")) & ".txt", "\s", "_")
            FileText(mFilenameFolder & "\" & filename) = sb.ToString
            Return filename
        Catch ex As Exception
            AppendToDebug(ex.Message & vbTab & GetStackTrace(, ex))
        End Try
    End Function

    Function EJAccountingFormat(ByVal val As String)
        Return Format(CDec2(val), "#,##0.00")
    End Function

    Function getEJournal(dtZRead As DataTable, dtAuditTrail As DataTable, MIN As String, Optional DateFromSource As String = "") As String

        Dim mFilenameFolder As String
        Dim mFilenameParentFolder As String

        mFilenameFolder = Common.ServerPath & "Content\EJournal\" & WebRequest.Instance().OrgID
        mFilenameParentFolder = Common.ServerPath & "Content\EJournal\"

        If Not System.IO.Directory.Exists(mFilenameParentFolder) Then
            System.IO.Directory.CreateDirectory(mFilenameParentFolder)
        End If

        If Not System.IO.Directory.Exists(mFilenameFolder) Then
            System.IO.Directory.CreateDirectory(mFilenameFolder)
        End If

        Dim sb As StringBuilder = New StringBuilder()
        Dim CurrentInvoiceNo As Integer = 0

        Dim TotalAmount As Double
        Dim TotalDiscount As Double
        Dim TotalQty As Integer
        Dim Cash As Double
        Dim Check As Double
        Dim DebitCard As Double
        Dim CreditCard As Double
        Dim SRV As Double
        Dim Others As Double
        Dim Change As Double
        Dim VatSales As Double
        Dim VatEx As Double
        Dim VatAmount As Double
        Dim InvoiceDate As Date
        Dim Cashier As String
        Dim dtTemp As DataTable
        Dim Customer As String
        Dim DateFrom As Date

        AppendToLog(DateFromSource)

        If CStr2(Common.WebRequest.Data("DateFrom")) = "" Then
            DateFrom = Convert.ToDateTime(DateFromSource)
        Else
            DateFrom = Convert.ToDateTime(Common.WebRequest.Data("DateFrom"))
        End If


        For i As Integer = 0 To dtAuditTrail.Rows.Count - 1

            If i = 0 Then
                CurrentInvoiceNo = CStr2(dtAuditTrail.Rows(i).Item("TransactionNo"))

                dtTemp = ComputeTotals(dtAuditTrail, CurrentInvoiceNo)

                TotalAmount = dtTemp.Rows(0).Item("TotalAmount")
                TotalQty = dtTemp.Rows(0).Item("TotalQty")
                TotalDiscount = dtTemp.Rows(0).Item("TotalDiscount")
                VatSales = dtTemp.Rows(0).Item("VatSales")
                VatEx = dtTemp.Rows(0).Item("VatEx")
                VatAmount = dtTemp.Rows(0).Item("VatAmount")

                Cash = CDec2(dtAuditTrail.Rows(i).Item("CashAmount"))
                Check = CDec2(dtAuditTrail.Rows(i).Item("CheckPaymentAmount"))
                DebitCard = CDec2(dtAuditTrail.Rows(i).Item("DebitCardAmount"))
                CreditCard = CDec2(dtAuditTrail.Rows(i).Item("CreditCardAmount"))
                SRV = CDec2(dtAuditTrail.Rows(i).Item("SRVAmount"))
                Others = CDec2(dtAuditTrail.Rows(i).Item("OtherTypeAmount"))
                Change = CDec2(dtAuditTrail.Rows(i).Item("CustomerChangeCash"))
                Cashier = CStr2(dtAuditTrail.Rows(i).Item("CashierName"))
                InvoiceDate = CStr2(dtAuditTrail.Rows(i).Item("TransactionDate"))
                Customer = CStr2(dtAuditTrail.Rows(i).Item("Customer"))
            End If

            If CInt(dtAuditTrail.Rows(i).Item("Quantity")) > 0 Then

                If CurrentInvoiceNo = CStr2(dtAuditTrail.Rows(i).Item("TransactionNo")) Then

                    'AppentTotalsToTextFile(TotalQty, TotalAmount, TotalDiscount, Cash, Change, VatSales, VatAmount, InvoiceDate, Cashier, CurrentInvoiceNo, False)

                    sb.Append(CStr2(dtAuditTrail.Rows(i).Item("ProductName")) & "  " & CInt(dtAuditTrail.Rows(i).Item("Quantity")) & "   " & Format(dtAuditTrail.Rows(i).Item("UnitPrice"), "#,###.00"))
                    sb.AppendLine()
                    sb.Append("         SubTotal  " & Format(dtAuditTrail.Rows(i).Item("SubTotal"), "#,###.00"))
                    sb.AppendLine()

                Else
                    'Print previous invoice Totals
                    AppentTotalsToTextFile(Customer, TotalQty, TotalAmount, TotalDiscount, Cash, Check, DebitCard, CreditCard, SRV, Others, Change, VatSales, VatAmount, InvoiceDate, Cashier, CurrentInvoiceNo, False, sb)

                    'Clear Totals
                    TotalAmount = 0
                    Cash = 0
                    Check = 0
                    DebitCard = 0
                    CreditCard = 0
                    SRV = 0
                    Others = 0
                    Change = 0
                    VatSales = 0
                    VatEx = 0
                    VatAmount = 0
                    Cashier = ""
                    'InvoiceDate = ""

                    'Get New invoice Totals

                    CurrentInvoiceNo = CStr2(dtAuditTrail.Rows(i).Item("TransactionNo"))

                    dtTemp = ComputeTotals(dtAuditTrail, CurrentInvoiceNo)

                    TotalAmount = dtTemp.Rows(0).Item("TotalAmount")
                    TotalQty = dtTemp.Rows(0).Item("TotalQty")
                    TotalDiscount = dtTemp.Rows(0).Item("TotalDiscount")
                    VatSales = dtTemp.Rows(0).Item("VatSales")
                    VatEx = dtTemp.Rows(0).Item("VatEx")
                    VatAmount = dtTemp.Rows(0).Item("VatAmount")

                    Cash = Format(dtAuditTrail.Rows(i).Item("CashAmount"), "#,###.00")
                    Check = CDec2(dtAuditTrail.Rows(i).Item("CheckPaymentAmount"))
                    DebitCard = CDec2(dtAuditTrail.Rows(i).Item("DebitCardAmount"))
                    CreditCard = CDec2(dtAuditTrail.Rows(i).Item("CreditCardAmount"))
                    SRV = CDec2(dtAuditTrail.Rows(i).Item("SRVAmount"))
                    Others = CDec2(dtAuditTrail.Rows(i).Item("OtherTypeAmount"))
                    Change = Format(dtAuditTrail.Rows(i).Item("CustomerChangeCash"), "#,###.00")

                    Cashier = CStr2(dtAuditTrail.Rows(i).Item("CashierName"))
                    InvoiceDate = CStr2(dtAuditTrail.Rows(i).Item("TransactionDate"))
                    Customer = CStr2(dtAuditTrail.Rows(i).Item("Customer"))

                    sb.Append(CStr2(dtAuditTrail.Rows(i).Item("ProductName")) & "  " & CInt(dtAuditTrail.Rows(i).Item("Quantity")) & "   " & Format(dtAuditTrail.Rows(i).Item("UnitPrice"), "#,###.00"))
                    sb.AppendLine()
                    sb.Append("         SubTotal  " & Format(dtAuditTrail.Rows(i).Item("SubTotal"), "#,###.00"))
                    sb.AppendLine()

                End If
            End If
        Next

        AppentTotalsToTextFile(Customer, TotalQty, TotalAmount, TotalDiscount, Cash, Check, DebitCard, CreditCard, SRV, Others, Change, VatSales, VatAmount, InvoiceDate, Cashier, CurrentInvoiceNo, False, sb)

        '-----------------ZRead------------------

        'Dim dblTotalSales As Double = 0
        'Dim dblTotalSalesDiscount As Double = 0
        'Dim dblAdjRefund As Double = 0
        'Dim dblAdjSRV As Double = 0
        'Dim dblNetSales As Double = 0

        'Dim dblCash = 0
        'Dim dblCheck As Double = 0
        'Dim dblDebitCard As Double = 0
        'Dim dblCreditCard As Double = 0
        'Dim dblSRV As Double = 0
        'Dim dblOtherPaymentType As Double = 0
        'Dim dblCharges As Double = 0
        'Dim dblSMAC As Double = 0
        'Dim dblEPlus As Double = 0
        'Dim dblVoucher As Double = 0
        'Dim dblTotalPayment As Double = 0


        'Dim dblVAT As Double = 0
        'Dim dblVATAmount As Double = 0
        'Dim dblNonVAT As Double = 0
        'Dim dblVATExempt As Double = 0
        'Dim dblZeroRatedVAT As Double = 0
        'Dim dblTotalVAT As Double = 0

        'Dim intTransCount As Double = 0
        'Dim intSalesTransCount As Integer = 0
        'Dim intSalesItemCount As Integer = 0
        'Dim intSRVTransCount As Integer = 0
        'Dim intSRVItemCount As Integer = 0
        'Dim intRefTransCount As Integer = 0
        'Dim intRefItemCount As Integer = 0
        'Dim dblDailySale As Double = 0
        'Dim dblSalesVAT As Double = 0
        'Dim dblOldGT As Double = 0
        'Dim dblNewGT As Double = 0

        'Dim dblRegDiscount As Double = 0
        'Dim dblEmpDiscount As Double = 0
        'Dim dblSCD As Double = 0
        'Dim dblPWD As Double = 0
        'Dim dblVIP As Double = 0
        'Dim dblGPC As Double = 0
        'Dim dblDiscount1 As Double = 0

        'Dim dblServiceCharge As Double = 0
        'Dim dblLocalTax As Double = 0
        'Dim zReadCount As Integer = 0

        'For p As Integer = 0 To dtZRead.Rows.Count - 1

        '    dblTotalSales = dblTotalSales + CDec2(dtZRead.Rows(p).Item("CurGrossSales"))
        '    dblTotalSalesDiscount = dblTotalSalesDiscount + CDec2(dtZRead.Rows(p).Item("CurTotalDiscount"))
        '    dblAdjRefund = dblAdjRefund + CDec2(dtZRead.Rows(p).Item("CurSalesReturnRefund"))
        '    dblAdjSRV = dblAdjSRV + CDec2(dtZRead.Rows(p).Item("CurSalesReturnExchange"))

        '    dblCash = dblCash + CDec2(dtZRead.Rows(p).Item("CashAmount"))
        '    dblCheck = dblCheck + CDec2(dtZRead.Rows(p).Item("CheckPaymentAmount"))
        '    dblDebitCard = dblDebitCard + CDec2(dtZRead.Rows(p).Item("DebitCardAmount"))
        '    dblCreditCard = dblCreditCard + CDec2(dtZRead.Rows(p).Item("CreditCardAmount"))
        '    dblSRV = dblSRV + CDec2(dtZRead.Rows(p).Item("SRVAmount"))
        '    dblOtherPaymentType = dblOtherPaymentType + CDec2(dtZRead.Rows(p).Item("OtherTypeAmount"))
        '    dblCharges = dblCharges + CDec2(dtZRead.Rows(p).Item("ChargeAmount"))
        '    dblSMAC = dblSMAC + CDec2(dtZRead.Rows(p).Item("SMACAmount"))
        '    dblEPlus = dblEPlus + CDec2(dtZRead.Rows(p).Item("EPlusAmount"))
        '    dblVoucher = dblVoucher + CDec2(dtZRead.Rows(p).Item("VoucherAmount"))
        '    'dblServiceCharge = dblServiceCharge + CDec2(dtZRead.Rows(p).Item("ServiceCharge"))
        '    'dblLocalTax = dblLocalTax + CDec2(dtZRead.Rows(p).Item("LocalTax"))


        '    dblVAT = dblVAT + CDec2(dtZRead.Rows(p).Item("CurTotalVATSales")) - CDec2(dtZRead.Rows(p).Item("CurTotalVATSalesSalesReturn"))
        '    dblVATAmount = dblVATAmount + CDec2(dtZRead.Rows(p).Item("CurTotalVAT")) - CDec2(dtZRead.Rows(p).Item("CurTotalVATAmountSalesReturn"))
        '    dblNonVAT = dblNonVAT + CDec2(dtZRead.Rows(p).Item("CurNoTAXSales"))
        '    dblVATExempt = dblVATExempt + CDec2(dtZRead.Rows(p).Item("CurVATExemptSales"))
        '    dblZeroRatedVAT = dblZeroRatedVAT + 0

        '    dblRegDiscount = dblRegDiscount + CDec2(dtZRead.Rows(p).Item("RegularDiscount"))
        '    dblEmpDiscount = dblEmpDiscount + CDec2(dtZRead.Rows(p).Item("EmployeeDiscount"))
        '    dblSCD = dblSCD + CDec2(dtZRead.Rows(p).Item("SeniorCitizenDiscount"))
        '    dblPWD = dblPWD + CDec2(dtZRead.Rows(p).Item("PWDDiscount"))
        '    dblVIP = dblVIP + CDec2(dtZRead.Rows(p).Item("VIPDiscount"))
        '    dblGPC = dblGPC + CDec2(dtZRead.Rows(p).Item("GPCDiscount"))
        '    dblDiscount1 = dblDiscount1 + CDec2(dtZRead.Rows(p).Item("Discount1"))



        '    intTransCount = intTransCount + CDec2(dtZRead.Rows(p).Item("CurTransCountInv")) + CDec2(dtZRead.Rows(p).Item("CurTransCountMemoReturnExchange")) + CDec2(dtZRead.Rows(p).Item("CurTransCountMemoRefund"))
        '    intSalesTransCount = intSalesTransCount + CDec2(dtZRead.Rows(p).Item("CurTransCountInv"))
        '    intSalesItemCount = intSalesItemCount + CDec2(dtZRead.Rows(p).Item("CurTransInvItemCount"))
        '    intSRVTransCount = intSRVTransCount + CDec2(dtZRead.Rows(p).Item("CurTransCountMemoReturnExchange"))
        '    intSRVItemCount = intSRVItemCount + CDec2(dtZRead.Rows(p).Item("CurSalesReturnExchangeItemCount"))
        '    intRefTransCount = intRefTransCount + CDec2(dtZRead.Rows(p).Item("CurTransCountMemoRefund"))
        '    intRefItemCount = intRefItemCount + CDec2(dtZRead.Rows(p).Item("CurSalesRefundItemCount"))

        '    dblDailySale = dblDailySale + CDec2(dtZRead.Rows(p).Item("CurGrossSales")) - (dtZRead.Rows(p).Item("CurTotalDiscount")) - (dtZRead.Rows(p).Item("CurSalesReturnRefund")) - (dtZRead.Rows(p).Item("CurSalesReturnExchange")) - (dtZRead.Rows(p).Item("CurServiceCharge")) - (dtZRead.Rows(p).Item("CurTotalVAT")) + CDec2(dtZRead.Rows(p).Item("ChargeAmount"))
        '    dblSalesVAT = dblSalesVAT + CDec2(dtZRead.Rows(p).Item("CurTotalVAT"))
        '    dblOldGT = dblOldGT + CDec2(dtZRead.Rows(p).Item("BegBal")) + CDec2(dtZRead.Rows(p).Item("PrevGrossSales")) - CDec2(dtZRead.Rows(p).Item("PrevTotalDiscount")) - CDec2(dtZRead.Rows(p).Item("PrevSalesReturnRefund")) - CDec2(dtZRead.Rows(p).Item("PrevSalesReturnExchange")) - CDec2(dtZRead.Rows(p).Item("PrevServiceCharge")) + CDec2(dtZRead.Rows(p).Item("PrevChargeAmount"))

        '    zReadCount += CInt2(dtZRead.Rows(p).Item("ZCount"))
        'Next

        'dblNewGT = dblOldGT + dblDailySale + dblSalesVAT
        'dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges
        'dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType
        'dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT

        'sb.Append("                                Z -REPORT                                          ")
        'sb.AppendLine()
        'sb.Append("                                 " & Format(InvoiceDate, "MM/dd/yyyy") & "                                       ")
        'sb.AppendLine()
        'sb.Append("Gross Sales: " & Format(dblTotalSales, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Sales Discount: " & Format(dblTotalSalesDiscount, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Charges: ")
        'sb.AppendLine()
        'sb.Append("Summary of Adjustments ")
        'sb.AppendLine()
        'sb.Append("Total Sales Refund for the Period " & Format(dblAdjRefund, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Total Sales Returns Subject to Merchadise Exchange (SRV Issued) " & Format(dblAdjSRV, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Net of Adjustments: " & Format(CDec2(dblAdjRefund) + CDec2(dblAdjSRV), "#,###.00"))
        'sb.AppendLine()
        'sb.Append("___________________________________________________________________________________")
        'sb.AppendLine()
        'sb.Append("Net Sales: " & Format(dblNetSales, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("                          Inventory of Payments                                    ")
        'sb.AppendLine()
        'sb.Append("Cash:         " & Format(dblCash, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Check:        " & Format(dblCheck, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Debit Card:   " & Format(dblDebitCard, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Credit Card:  " & Format(dblCreditCard, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("SRV:          " & Format(dblSRV, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Other Tender: " & Format(dblOtherPaymentType, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("___________________________________________________________________________________")
        'sb.AppendLine()

        ''Sales Grouped by VAT TYPE----------------------------
        'Dim dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100
        'Dim dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100
        'Dim dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100
        'Dim dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100
        'Dim dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100
        'Dim dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent

        'sb.Append("                       Sales Grouped by VAT Type                                   ")
        'sb.AppendLine()
        'sb.Append("VAT-able Sales: " & Format(dblVAT, "#,###.00") & "   " & Format(dblTotalVATSalesPercent, "#,###.00") & "%")
        'sb.AppendLine()
        'sb.Append("VAT Amount:     " & Format(dblVATAmount, "#,###.00") & "   " & Format(dblTotalVATAmountPercent, "#,###.00") & "%")
        'sb.AppendLine()
        'sb.Append("Non-VAT:        " & Format(dblNonVAT, "#,###.00") & "   " & Format(dblTotalNonVATPercent, "#,###.00") & "%")
        'sb.AppendLine()
        'sb.Append("VAT-Exempt:     " & Format(dblVATExempt, "#,###.00") & "   " & Format(dblTotalVATExemptPercent, "#,###.00") & "%")
        'sb.AppendLine()
        'sb.Append("Zero-Rated VAT: " & Format(dblZeroRatedVAT, "#,###.00") & "   " & Format(dblTotalZeroRatedVATPercent, "#,###.00") & "%")
        'sb.AppendLine()
        'sb.Append("___________________________________________________________________________________")
        'sb.AppendLine()
        'sb.Append("                           Other Information                                       ")
        'sb.AppendLine()
        'sb.Append("Total Trans. Count:     " & intTransCount)
        'sb.AppendLine()
        'sb.Append("Sales Trans. Count:     " & intSalesTransCount)
        'sb.AppendLine()
        'sb.Append("Sales Line Item Count:  " & intSalesItemCount)
        'sb.AppendLine()
        'sb.Append("SRV Trans. Count:       " & intSRVTransCount)
        'sb.AppendLine()
        'sb.Append("SRV Line Item Count:    " & intSRVItemCount)
        'sb.AppendLine()
        'sb.Append("Refund Trans. Count:    " & intRefTransCount)
        'sb.AppendLine()
        'sb.Append("Refund Line Item Count: " & intRefItemCount)
        'sb.AppendLine()
        'sb.Append("Customer Count:         " & intSalesTransCount)
        'sb.AppendLine()
        'sb.Append("Old Grand Total:        " & Format(dblOldGT, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("New Grand Total:        " & Format(dblNewGT, "#,###.00"))
        'sb.AppendLine()
        'sb.Append("Z-Read Print Count:     " & zReadCount)
        'sb.AppendLine()

        FileText(mFilenameFolder & "\POS_eJournal_" & MIN & "_" & Format(DateFrom, "MM-yyy") & ".jrn") = sb.ToString

        If dtAuditTrail.Rows.Count > 0 Then
            Dim pSql As String

            pSql = "Update TransSalesInvoiceAndRebatesHeader set FileName = " & ReadyString("POS_eJournal_" & Format(InvoiceDate, "MM-dd-yyy") & ".jrn") & vbCrLf &
                                                                " WHERE DeviceID = " & ReadyString(dtAuditTrail.Rows(0).Item("DeviceID")) & vbCrLf &
                                                                " AND FOWSTID = " & ReadyString(dtAuditTrail.Rows(0).Item("FOWSTID")) & vbCrLf &
                                                                " AND Month(TransactionMonthYear) = " & CDate(dtAuditTrail.Rows(0).Item("TransactionDate")).Month & vbCrLf &
                                                                " AND Year(TransactionMonthYear) = " & CDate(dtAuditTrail.Rows(0).Item("TransactionDate")).Year
            Common.WebRequest.ProData.Execute(pSql)
        End If

        Return "POS_eJournal_" & MIN & "_" & Format(DateFrom, "MM-yyy") & ".jrn"
    End Function

    Private Function ComputeTotals(dtAuditTrail As DataTable, CurrentInvoiceNo As Integer) As DataTable
        Dim dtTemp As New DataTable
        Dim dr As DataRow

        dtTemp.Columns.Add("TotalAmount")
        dtTemp.Columns.Add("TotalQty")
        dtTemp.Columns.Add("TotalDiscount")
        dtTemp.Columns.Add("VatSales")
        dtTemp.Columns.Add("VatEx")
        dtTemp.Columns.Add("VatAmount")

        dr = dtTemp.NewRow
        dr(0) = CDec2(dtAuditTrail.Compute("SUM(SubTotal)", "TransactionNo = " & CurrentInvoiceNo))
        dr(1) = dtAuditTrail.Compute("SUM(Quantity)", "TransactionNo = " & CurrentInvoiceNo)
        dr(2) = CDec2(dtAuditTrail.Compute("SUM(DiscountAmount)", "TransactionNo = " & CurrentInvoiceNo))
        dr(3) = CDec2(dtAuditTrail.Compute("SUM(VATableSales)", "TransactionNo = " & CurrentInvoiceNo))
        dr(4) = CDec2(dtAuditTrail.Compute("SUM(SubTotal)", "TransactionNo = " & CurrentInvoiceNo))
        dr(5) = CDec2(dtAuditTrail.Compute("SUM(VATAmount)", "TransactionNo = " & CurrentInvoiceNo))

        dtTemp.Rows.Add(dr)

        Return dtTemp
    End Function

    Function AppentTotalsToTextFile(Customer As String,
                                    TotalQty As Integer,
                                    TotalAmount As Double,
                                    TotalDiscount As Double,
                                    Cash As Double,
                                    Check As Double,
                                    DebitCard As Double,
                                    CreditCard As Double,
                                    SRVAmount As Double,
                                    OtherTypeAmount As Double,
                                    Change As Double,
                                    VatSales As Double,
                                    VatAmount As Double,
                                    InvoiceDate As Date,
                                    Cashier As String,
                                    CurrentInvoiceNo As Integer,
                                    IsTransaction As Boolean,
                                    ByRef sb As StringBuilder)

        Dim TotalAmountRoundedToCeiling As Double = 0

        Change = 0

        If sb.Length = 0 Then
            sb.AppendLine("No sales data was found")
        Else
            sb.AppendLine("      SKU Count " & TotalQty)
            sb.AppendLine("Total         " & Format(TotalAmount, "#,###.00"))
            sb.AppendLine("Total Discount         " & Format(TotalDiscount, "#,###.00"))

            'If Cash > 0 Then
            '    sb.AppendLine("Cash          " & Format(Cash, "#,###.00"))
            'ElseIf Check > 0 Then
            '    sb.AppendLine("Check         " & Format(Check, "#,###.00"))
            'ElseIf DebitCard > 0 Then
            '    sb.AppendLine("Debit Card    " & Format(DebitCard, "#,###.00"))
            'ElseIf CreditCard > 0 Then
            '    sb.AppendLine("Credit Card   " & Format(CreditCard, "#,###.00"))
            'ElseIf SRVAmount > 0 Then
            '    sb.AppendLine("SRV           " & Format(SRVAmount, "#,###.00"))
            'ElseIf OtherTypeAmount > 0 Then
            '    sb.AppendLine("Others        " & Format(OtherTypeAmount, "#,###.00"))
            'End If

            If Cash > 0 Then

                TotalAmountRoundedToCeiling = (Math.Ceiling(TotalAmount / 100) * 100)
                sb.AppendLine("Cash          " & Format(TotalAmountRoundedToCeiling, "#,###.00"))
                Change = TotalAmountRoundedToCeiling - TotalAmount

                'sb.AppendLine("Cash          " & Format(TotalAmount, "#,###.00"))
            ElseIf Check > 0 Then
                sb.AppendLine("Check         " & Format(TotalAmount, "#,###.00"))
            ElseIf DebitCard > 0 Then
                sb.AppendLine("Debit Card    " & Format(TotalAmount, "#,###.00"))
            ElseIf CreditCard > 0 Then
                sb.AppendLine("Credit Card   " & Format(TotalAmount, "#,###.00"))
            ElseIf SRVAmount > 0 Then
                sb.AppendLine("SRV           " & Format(TotalAmount, "#,###.00"))
            ElseIf OtherTypeAmount > 0 Then
                sb.AppendLine("Others        " & Format(TotalAmount, "#,###.00"))
            End If

            sb.AppendLine("Change        " & Format(Change, "#,###.00"))
            sb.AppendLine("VAT Sales     " & Format(TotalAmount, "#,###.00"))
            sb.AppendLine("VAT Rate      12%")
            sb.AppendLine("VATable Sales " & Format(TotalAmount / 1.12, "#,###.00"))
            sb.AppendLine("VAT Amount    " & Format(TotalAmount - (TotalAmount / 1.12), "#,###.00"))
            sb.AppendLine("Invoice Date  " & InvoiceDate)
            sb.AppendLine("Cashier       " & Cashier)
            sb.AppendLine("Customer      " & Customer)

            If TotalAmount > 0 Then
                sb.AppendLine("Invoice No.   " & CurrentInvoiceNo)
            Else
                sb.AppendLine("Transaction No.   " & CurrentInvoiceNo)
            End If


            sb.Append("-------------------------------------")
            sb.AppendLine()
        End If
    End Function
    Function DownloadZRead(ByVal DownloadDataOnly As Boolean,
                          ByVal fowstid As String,
                           ByVal deviceid As String,
                           ByVal datefrom As String,
                           ByVal dateto As String,
                           ByVal origdatefrom As String,
                           ByVal origdateto As String,
                           ByVal ForceNONVAT As String,
                           ByVal GetFromRawData As Boolean,
                           ByVal GenerateZeroSales As Boolean,
                           Optional ExcludePTUDetails As Boolean = False)

        Try
            Dim clsSalesReport As New ReportPOS
            Dim megaworld As New MallLessorMegaworld

            If CBool2(GenerateZeroSales) Then
                datefrom = "2000-01-01"
                dateto = "2000-01-01"
            End If

            Dim pDtHeader = clsSalesReport.getReportHeader(Common.WebRequest.ProData, fowstid, Nothing, deviceid)
            Dim pDtOtherPayments = megaworld.getOtherPaymentTypes(Common.WebRequest.ProData, datefrom, dateto, fowstid, deviceid)
            Dim dtDiscounts As DataTable = megaworld.getDailyDiscountBreakdown(Common.WebRequest.ProData, datefrom, dateto, fowstid, deviceid)

            Dim CashRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "cash")))
            Dim OtherPaymentRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "otherpayment")))
            Dim DiscountRefund = 0 'megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "discount")))
            Dim DebitRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "debit")))
            Dim CreditRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "credit")))

            If fowstid = "228dff67-dea7-4141-9767-f9c39706ea12" Or
                fowstid = "6f039617-d2ab-4922-887e-524f72a50e76" Or
                fowstid = "5bfe1612-eb90-4f39-aaa1-7254011118c8" Or
                fowstid = "f591c52b-6f68-4126-a4dd-98dedbf3c6b0" Then
                'Newport, San Antonio Residence, Venice Grand Canal, Clark City Front Mall
                'gusto ibawas ni Megaworld ang discount sa "Sales Discount" field kung irefund sa zread, no choice
                'during refund after maaccredit sa Megaworld, bale discounted amount na ang marefund, so basig pwede na tanggalon ni puhon after ma accredit usab sa BIR
                'for safety purposes lang ni since nagapangayo og monthly online zread ang Megaworld, need magdeploy og changes sa Azure, and wala pa naaccredit usab sa BIR as of Oct 15, 2025
                DiscountRefund = megaworld.Roundoff(CDec2(megaworld.GetAmount(Common.WebRequest.ProData, "period", datefrom, dateto, fowstid, deviceid, "discount")))
            End If

            dtDiscounts.Columns.Add("Code")
            If dtDiscounts.Rows.Count > 0 Then
                For Each disc As Object In dtDiscounts.Rows
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

                    'If code = "SENDISC" Then
                    '    disc("DiscountAmount") = CDec2(CDec2(disc("DiscountAmount")) - DiscountRefund)
                    'End If
                Next
            End If

            Common.WebRequest.ProData.CommandTimeout = 60000
            Dim totalOtherPayments As Decimal = 0
            Dim dtaDSaleTable As DataTable
            Dim pCacheID = CStr2(WebRequest.Instance.QueryString("cacheid"))
            Dim pCacheFilename As String
            If String.IsNullOrWhiteSpace(pCacheID) = False Then
                pCacheFilename = Path.Combine(Common.ServerPath, "temp\DownloadZReadXml", pCacheID & ".xml")
            End If
            Try
                If String.IsNullOrWhiteSpace(pCacheID) = False Then
                    If IO.File.Exists(pCacheFilename) Then
                        dtaDSaleTable = New DataTable
                        dtaDSaleTable.ReadXml(pCacheFilename)
                    End If
                End If
            Catch ex As Exception
                dtaDSaleTable = Nothing
            End Try
            If dtaDSaleTable Is Nothing Then
                'If GetFromRawData Then
                dtaDSaleTable = megaworld.getDailySales(Common.WebRequest.ProData, datefrom, dateto, fowstid, deviceid) 'getZReadReport(mProdata, dtDate.ToString("MM/dd/yyyy"), dtDateTo.ToString("MM/dd/yyyy"), vFowstID, vDeviceID)
                'Else
                'dtaDSaleTable = GetCacheZReadData(Common.WebRequest.ProData, datefrom, dateto, fowstid, deviceid)
                'End If

                ' START insert E-Sales "new grand total"
                GetGrandTotalFromESales(dtaDSaleTable, dateto, fowstid, deviceid)
                ' END insert E-Sales "new grand total"

                If String.IsNullOrWhiteSpace(pCacheID) = False Then
                    CreateFolder(pCacheFilename)
                    dtaDSaleTable.WriteXml(pCacheFilename, XmlWriteMode.WriteSchema)
                End If
            End If

            Dim dRowSales = dtaDSaleTable.Rows(0)
            Dim netsales As String = megaworld.Roundoff(dRowSales("CurGrossSales") - (dRowSales("TotalSCDiscount") + dRowSales("CurTotalDiscount")) - dRowSales("CurSalesReturnRefund") - dRowSales("CurSalesReturnExchange") - dRowSales("CurVATExempt") + dRowSales("CurServiceCharge"))
            Dim scdiscvatablesales As Decimal = 0, scdiscvatableamount As Decimal = 0

            If CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) > 0 Then
                scdiscvatablesales = CDec2(CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) / 1.12)
                scdiscvatableamount = CDec2(CDec2(dtaDSaleTable.Rows(0).Item("CurSCDiscountExempt")) - scdiscvatablesales)
            End If

            If ForceNONVAT IsNot Nothing And Not CBool2(ForceNONVAT) Then
                pDtHeader.Rows(0)("IsVAT") = CBool2(ForceNONVAT)
            End If

            If CDec2(CashRefund) > 0 Then
                dtaDSaleTable.Rows(0).Item("CashAmount") = CDec2(dtaDSaleTable.Rows(0).Item("CashAmount")) - CDec2(Format(CashRefund, "0.00"))
                CashRefund = 0
            End If

            dtaDSaleTable.Columns.Add("TotalVATType")
            dtaDSaleTable.Columns.Add("VATableSales")
            dtaDSaleTable.Columns.Add("VATAmount")
            dtaDSaleTable.Columns.Add("NonVAT")
            dtaDSaleTable.Columns.Add("VATExempt")
            dtaDSaleTable.Columns.Add("ZeroRatedVAT")
            dtaDSaleTable.Columns.Add("TotalOtherPayments")
            dtaDSaleTable.Columns.Add("CashRefund")
            dtaDSaleTable.Columns.Add("OtherPaymentRefund")
            If dtaDSaleTable.Columns.IndexOf("DiscountRefund") = -1 Then
                dtaDSaleTable.Columns.Add("DiscountRefund")
            End If
            dtaDSaleTable.Columns.Add("DebitRefund")
            dtaDSaleTable.Columns.Add("CreditRefund")
            dtaDSaleTable.Rows(0).Item("TotalVATType") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSales")) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSalesSalesReturn"))) + (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVAT") + scdiscvatableamount) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmountSalesReturn"))) + CDec2(dtaDSaleTable.Rows(0).Item("CurNoTAXSales")) + CDec2(dtaDSaleTable.Rows(0).Item("CurVATExemptSales")), netsales)
            dtaDSaleTable.Rows(0).Item("VATableSales") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSales")) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATSalesSalesReturn")), 0)
            dtaDSaleTable.Rows(0).Item("VATAmount") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), (CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVAT")) + scdiscvatableamount) - CDec2(dtaDSaleTable.Rows(0).Item("CurTotalVATAmountSalesReturn")), 0)
            dtaDSaleTable.Rows(0).Item("NonVAT") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurNoTAXSales")), netsales)
            dtaDSaleTable.Rows(0).Item("VATExempt") = IIf(CBool2(pDtHeader.Rows(0)("IsVAT")), CDec2(dtaDSaleTable.Rows(0).Item("CurVATExemptSales")), 0)
            dtaDSaleTable.Rows(0).Item("ZeroRatedVAT") = 0
            dtaDSaleTable.Rows(0).Item("CashRefund") = Format(CashRefund, "0.00")
            dtaDSaleTable.Rows(0).Item("OtherPaymentRefund") = Format(OtherPaymentRefund, "0.00")
            dtaDSaleTable.Rows(0).Item("DiscountRefund") = Format(DiscountRefund, "0.00")
            dtaDSaleTable.Rows(0).Item("DebitRefund") = Format(DebitRefund, "0.00")
            dtaDSaleTable.Rows(0).Item("CreditRefund") = Format(CreditRefund, "0.00")

            For Each op As Object In pDtOtherPayments.Rows
                totalOtherPayments += CDec2(op("Amount"))
            Next

            dtaDSaleTable.Rows(0).Item("TotalOtherPayments") = CDec2(totalOtherPayments)
            dtaDSaleTable.Rows(0).Item("OtherPaymentAmount") = CDec2(totalOtherPayments)

            If CBool2(GenerateZeroSales) Then
                datefrom = Format(CDate(origdatefrom), "MM/dd/yyyy")
                dateto = Format(CDate(origdateto), "MM/dd/yyyy")
                dtaDSaleTable.Rows(0).Item("TransDate") = IIf(datefrom = dateto, datefrom, datefrom & " - " & dateto)
            End If

            If ExcludePTUDetails Then
                pDtHeader.Rows(0)("POSRegistrationNo") = ""
                pDtHeader.Rows(0)("MIN") = ""
                pDtHeader.Rows(0)("UuidDisplay") = ""
            End If

            If DownloadDataOnly Then
                Dim pOut As New JsonDictionary

                pOut.Add("Data", dtaDSaleTable)
                pOut.Add("Header", pDtHeader)
                pOut.Add("Discounts", dtDiscounts)
                pOut.Add("OtherPayments", pDtOtherPayments)

                Return pOut.Serialize()
            Else
                Dim pReport = New ProReport
                pReport.SourceFilename = "NewZRead"
                pReport.AddTable("Data", dtaDSaleTable)
                pReport.AddTable("Header", pDtHeader)
                pReport.AddTable("Discounts", dtDiscounts)
                pReport.AddTable("OtherPayments", pDtOtherPayments)
                pReport.ExportXmlFile()
                Dim fowstname = Common.WebRequest.ProData.ExecuteScalar("SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(fowstid))
                Dim zreadname = "ZREAD_" & fowstname.Replace(",", "") & "_" & datefrom & " - " & dateto & ".pdf"
                Dim path = Common.ServerPath & "temp\ZRead\"
                CreateFolder(path)
                path &= CreateValidFilename(zreadname, True)
                pReport.ExportFile(ProReport.eExportFormat.Pdf, path)

                Dim file As New System.IO.FileInfo(path)

                Return path
                'Response.Clear()
                'Response.AddHeader("Content-Disposition", "attachment; filename=" & zreadname)
                'Response.AddHeader("Content-Length", file.Length.ToString())
                'Response.ContentType = "application/octet-stream"
                'Response.WriteFile(file.FullName)
                'Response.End()
            End If
        Catch ex As Exception
            AppendToDebug(ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
            If DownloadDataOnly Then
                Dim pOut As New JsonDictionary
                pOut.Add("Error", ex.Message)
                Return pOut.Serialize()
            Else
                Return ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            End If
        End Try
    End Function

    Sub GetGrandTotalFromESales(ByRef rDT As DataTable, vDateTo As Date, vFowstID As String, vDeviceID As String)

        AppendToLog("GetGrandTotalFromESales" & vbCrLf &
            "vDateTo = " & vDateTo.ToString(cGlobal.FormatDateTimeGrid) & vbCrLf &
            "vDeviceID: " & vDeviceID)

        Dim pDrDevice = WebRequest.Instance.Business.LibraryDevice.ById(vDeviceID)
        If pDrDevice Is Nothing Then
            Return
        End If

        Dim pSql = "SELECT TransESalesYearly.*" & vbCrLf &
            " FROM TransESalesYearly" & vbCrLf &
            " JOIN LibraryDeviceFowst ON LibraryDeviceFowst.BIRMachineNo = TransESalesYearly.MachineNumber" & vbCrLf &
            " WHERE LibraryDeviceFowst.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " AND LibraryDeviceFowst.DeviceID = " & ReadyString(vDeviceID) & vbCrLf &
            " AND TransESalesYearly.ZReadYear = " & vDateTo.Year
        Dim pDrESales = WebRequest.Instance.ProData.ExecuteRow(pSql)
        If pDrESales Is Nothing Then
            Return
        End If

        Dim pGrossSales = CDec2(pDrESales("AccumulatedGrossSales" & vDateTo.Month.ToString("00")))
        AppendToLog("GetGrandTotalFromESales" & vbCrLf &
            "pGrossSales = " & pGrossSales)
        If pGrossSales <= 0 Then
            Return
        End If

        AppendToLog("GetGrandTotalFromESales" & vbCrLf &
            "rDT.Rows.Count = " & rDT.Rows.Count)
        For Each pDR As DataRow In rDT.Rows
            ' PresentationCurrGrossSales = pDT.Data.Rows[0].CurGrossSales;
            ' CurSCDiscount = pDT.Data.Rows[0].TotalSCDiscount;
            ' - CDec2(pDR("CurTotalVatAmountPortionOfSCDTransaction")) 
            Dim pDailySale = CDec2(pDR("CurGrossSales")) -
                CDec2(pDR("CurVATExempt")) -
                CDec2(pDR("TotalSCDiscount")) -
                CDec2(pDR("TotalSoloParentDiscount")) -
                CDec2(pDR("CurTotalDiscount")) -
                CDec2(pDR("CurSalesReturnRefund")) -
                CDec2(pDR("CurSalesReturnExchange")) -
                CDec2(pDR("CurServiceCharge")) -
                CDec2(pDR("CurTotalVAT")) +
                CDec2(pDR("ChargeAmount"))

            ' CDec2(pDR("CurTotalVatAmountPortionOfSCDTransaction")) +
            Dim pSalesVat = CDec2(pDR("CurTotalVAT"))

            ' PrevSCDiscount = pDT.Data.Rows[0].PrevTotalSCDiscount;
            ' PresentationPrevGrossSales = pDT.Data.Rows[0].PrevGrossSales;
            Dim pOldGrandTotal = CDec2(pDR("BegBal")) +
                CDec2(pDR("PrevGrossSales")) -
                CDec2(pDR("PrevTotalDiscount")) +
                CDec2(pDR("PrevTotalSCDiscount")) +
                CDec2(pDR("PrevSoloParentDiscount")) -
                CDec2(pDR("PrevSalesReturnRefund")) -
                CDec2(pDR("PrevSalesReturnExchange")) -
                CDec2(pDR("PrevServiceCharge")) -
                CDec2(pDR("PrevVATExempt")) +
                CDec2(pDR("PrevChargeAmount"))

            'Dim pNewGrandTotal = pOldGrandTotal + pDailySale + pSalesVat

            ' PresentationPrevGrossSales = pDT.Data.Rows[0].PrevGrossSales;
            pDR("PrevGrossSales") = pGrossSales - pDailySale - pSalesVat - pOldGrandTotal + CDec2(pDR("PrevGrossSales"))
        Next

    End Sub
End Class
