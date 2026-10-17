define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc='
    ], function () {

        window.SalesReport_offline = function () {

            this.getOrgFOWSTPrinterIP = function () {

                pSql = "SELECT devfowstpntr.ID, " +
                    " fowstpntr.PrinterIPAddress " +
                    " FROM private.LibraryDeviceFowstPrinter devfowstpntr " +
                    " INNER JOIN private.LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID) " +
                    " Where fowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                    " AND devfowstpntr.DeviceID = '" + Web.DeviceID() + "'";

//                pSql = "SELECT devfowstpntr.ID, " +
//                    " devfowstpntr.PrinterIPAddress " +
//                    " FROM private.LibraryDeviceFowstPrinter devfowstpntr " +
//                    " Where devfowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
//                    " AND devfowstpntr.DeviceID = '" + Web.DeviceID() + "'";

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.getReportHeader = function (IsManagerSalesReport) {

                var pSql = ' SELECT subcrbr.OrganizationID as SubscriberOrganizationID ' + '\n' + 
                    ' ,IFNULL((SELECT BusinessRegisteredName' + '\n' +
                    ' FROM LibraryDevice' + '\n' +
                    ' WHERE ID = devfowst.DeviceID), subcr.CompanyName) AS SubscriberOrganizationName \n' +
                    ' ,rsetfowst.FowstID as OrganizationBranchID  ' + '\n' + 
                    ' ,IFNULL((SELECT BusinessName' + '\n' +
                    ' FROM LibraryDevice' + '\n' +
                    ' WHERE ID = devfowst.DeviceID), rsettrdname.BusinessTradeName) AS OrgBusinessTradeName \n' +
                    ' ,subcrbr.[Name] as OrgBranchName' + '\n' + 
                    ' ,IFNULL(NULLIF(TRIM(subcrbr.StreetNumber) || \' \', \' \'), \'\') || IFNULL(NULLIF(TRIM(subcrbr.Street) || \', \',\', \'), \'\') || IFNULL(NULLIF(TRIM(subcrcty.Name) || \', \',\', \'), \'\') || IFNULL(NULLIF(TRIM(subcrctry.Name) || \', \',\', \'), \'\') as OrgCompleteAddress ' + '\n' + 
                    ' ,(SELECT GROUP_CONCAT(IFNULL(ctry.CallingCode,\'\') || \' \' || IFNULL(areacode.CallingCode,\'\') || \' \' || IFNULL(orgfowsttel.ContactNo,\'\') || IFNULL(NULLIF(\'Ext. \' || orgfowsttel.ExtensionNo,\'Ext. \'),\'\'),\', \') ' + '\n' + 
                    '     FROM LibraryOrganizationFOWSTTel orgfowsttel ' + '\n' + 
                    '     JOIN LibraryCountries ctry ON (orgfowsttel.CountryID = ctry.ID) ' + '\n' + 
                    '     JOIN LibraryAreaCodes areacode ON (orgfowsttel.AreaCodeID = areacode.ID) ' + '\n' + 
                    '     WHERE orgfowsttel.FOWSTID = rsetfowst.FowstID ' + '\n' + 
                    '     ORDER BY IFNULL(ctry.CallingCode,\'\') || \' \' || IFNULL(areacode.CallingCode,\'\') || \' \' || IFNULL(orgfowsttel.ContactNo,\'\') || IFNULL(NULLIF(\'Ext. \' || orgfowsttel.ExtensionNo,\'Ext. \'),\'\') ASC) as OrgTelNo ' + '\n' + 
                    " ,IFNULL((SELECT TinDisplay" + '\n' +
                    " FROM LibraryDevice" + '\n' +
                    " WHERE ID = devfowst.DeviceID), subcr.TinNumber || '-00' || IFNULL(substr('000'||subcrbr.TINBranchCode, -3, 3),'')) as TIN " +
                    ' ,IFNULL((SELECT IFNULL(NULLIF(TRIM(Surname) || \', \',\', \'), \'\') || IFNULL(NULLIF(TRIM(FirstName) || \' \',\' \'), \'\') || IFNULL(NULLIF(TRIM(MiddleName) || \' \',\' \'), \'\') ' + '\n' + 
                    ' FROM LibraryPeople WHERE ID = ' + ReadyString(Web.PeopleUserID()) + ' LIMIT 1),\'\') as PrintedBy ' + '\n' + 
                    ',' + proj.SqliteNow + ' as PrintDateTime ' + '\n' + 
                    ',' + IsManagerSalesReport + ' as POSReportType  ' + '\n' + 
                    ' ,devfowst.IsAuthorizedForPOS \n' +
                    " ,devfowst.BIRMachineNo as MIN " +
                    " ,devfowst.POSRegistrationNo as POSPermitNo " +
                    " ,IFNULL(subcrbr.IsVAT, 1) as IsVAT " +
                    ' ,(SELECT UuidDisplay' + '\n' +
                    ' FROM LibraryDevice' + '\n' +
                    ' WHERE ID = devfowst.DeviceID) AS UuidDisplay \n' +
                    ' FROM LibraryOrganizationFOWST subcrbr' + '\n' + 
                    ' JOIN LibraryOrganization subcr ON (subcrbr.OrganizationID = subcr.ID)  ' + '\n' + 
                    ' LEFT JOIN LibraryCities subcrcty ON (subcrbr.FowstCityID = subcrcty.ID)  ' + '\n' + 
                    ' LEFT JOIN LibraryCountries subcrctry ON (subcrbr.FowstCountryID = subcrctry.ID)  ' + '\n' + 
                    ' LEFT JOIN private.LibraryReceiptSettingsFowst rsetfowst ON (rsetfowst.FowstID = subcrbr.ID)  ' + '\n' + 
                    ' LEFT JOIN private.LibraryReceiptSettings rset ON (rsetfowst.ReceiptSettingsID = rset.ID)  ' + '\n' + 
                    ' LEFT JOIN LibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID)  \n' +
                    " LEFT JOIN private.LibraryDeviceFowst devfowst ON (rsetfowst.FowstID = devfowst.FOWSTID AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ") \n" +
                    " LEFT JOIN private.LibraryDevice dev ON (devfowst.DeviceID = dev.ID) \n" +
                    ' Where subcrbr.ID = ' + ReadyString(sessionStorage.FowstID);

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.getCashierSalesTransactionSummary = function (pData) {
                var strCustInvDateRange = '';
                var strCustPayDateRange = '';
                var strMemoDateRange = '';
                var strRangeYearFrom = '';
                var strRangeYearTo = '';
                var strRangeFrom = '';
                var strRangeTo = '';
                var strCurDate = (new Date().getFullYear() + '-' + ((new Date().getMonth() + 1) < 10 ? '0' + (new Date().getMonth() + 1) : (new Date().getMonth() + 1)) + '-' + (new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate()));
                var strYesterdayDate = '';
                var newDate = new Date();
                newDate.setDate(newDate.getDate() - 1);
                strYesterdayDate = (newDate.getFullYear() + '-' + ((newDate.getMonth() + 1) < 10 ? '0' + (newDate.getMonth() + 1) : (newDate.getMonth() + 1)) + '-' + (newDate.getDate() < 10 ? '0' + newDate.getDate() :  newDate.getDate()));

                strRangeYearFrom = new Date().getFullYear();
                strRangeYearTo = new Date().getFullYear();

                if (pData.RangeYear == "This year.") {
                    strRangeYearFrom = new Date().getFullYear();
                }
                else if (pData.RangeYear == "This year and last year.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 1;
                }
                else if (pData.RangeYear == "This year and last 2 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 2;
                }
                else if (pData.RangeYear == "This year and last 3 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 3;
                }
                else if (pData.RangeYear == "This year and last 4 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 4;
                }
                else if (pData.RangeYear == "This year and last 5 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 5;
                }

                if (pData.RangeType == "Today") {
                    strRangeFrom = getFOWSTOperationHR(strCurDate, true);
                    strRangeTo = getFOWSTOperationHR(strCurDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Yesterday") {

                    strRangeFrom = getFOWSTOperationHR(strYesterdayDate, true);
                    strRangeTo = getFOWSTOperationHR(strYesterdayDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Day") {
                    var newDateFrom = new Date(pData.RangeFrom);
                    var newDateTo = new Date(pData.Rangeto);

                    strRangeFrom = getFOWSTOperationHR(((newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear()), true);
                    strRangeTo = getFOWSTOperationHR(((newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear()), false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Week") {

                    strCustInvDateRange = " strftime('%w',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%w',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%w',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Month") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Quarter") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Year") {
                    strCustInvDateRange = " strftime('%Y', inv.InvoiceDate) = " + strRangeYear

                    strCustPayDateRange = " strftime('%Y', custpay.PaymentDate) = " + strRangeYear

                    strMemoDateRange = " strftime('%Y', memo.LastUpdated) = " + strRangeYear
                }

                pSql = "SELECT " +
                " IFNULL((SELECT IFNULL(NULLIF(TRIM(cshr.Surname)  || ', ',', '), '') || " +
                "                IFNULL(NULLIF(TRIM(cshr.FirstName) || ' ',' '), '') || " +
                "                IFNULL(NULLIF(TRIM(cshr.MiddleName) || ' ',' '), '') as CashierName " +
                "         FROM LibraryPeople cshr " +
                "         WHERE cshr.ID = " + ReadyString(Web.PeopleUserID()) + "),'') as CashierName" +
                ",IFNULL((SELECT SUM(invitem.SubTotal) as CurGrossSales " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND invitem.TaxAmount > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurGrossSales " +
                ",IFNULL((SELECT SUM(invitem.DiscountAmount) + SUM(inv.SCDiscount) as CurSalesDiscount " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND invitem.TaxAmount > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurSalesDiscount " +
                ",IFNULL((SELECT SUM(inv.ForegoneVat) as CurVATExempt " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurVATExempt " +
                ",IFNULL((SELECT SUM(srledger.GrossRefundAmount) as CurSalesReturnRefund " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND memo.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurSalesReturnRefund " +
                ",IFNULL((SELECT SUM(srledger.GrossSrvAmount) as CurSalesReturnExchange " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND memo.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurSalesReturnExchange " +
                ",IFNULL((SELECT SUM(invitem.TaxSales) as CurTotalVATSales " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurTotalVATSales " +
                ",IFNULL((SELECT SUM(invitem.TaxAmount) as CurTotalVATAmount " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurTotalVATAmount " +
                ",IFNULL((SELECT SUM(IFNULL(memosr.TotalAmount,0) - IFNULL(memosr.TaxAmount,0)) as CurTotalVATSalesSalesReturn " +
                "         FROM private.TransMemoSalesReturn memosr " +
                "         INNER JOIN private.TransMemo memo ON (memosr.MemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND memo.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurTotalVATSalesSalesReturn " +
                ",IFNULL((SELECT SUM(invitem.SubTotal) as CurNoTAXSales " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE invitem.TaxAmount <= 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CurNoTAXSales " +
                ",0 as CurOthers " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0)) as CashAmount " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CashAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.DebitCardAmount,0)) as DebitCardAmount " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as DebitCardAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.CreditCardAmount,0)) as CreditCardAmount " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID " +
                " AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CreditCardAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.CheckPaymentAmount,0)) as CheckPaymentAmount " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID " +
                " AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as CheckPaymentAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.SRVAmount,0)) as SRVAmount " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID " +
                " AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as SRVAmount " +
                 ",IFNULL((SELECT SUM(IFNULL(custpay.ChargeAmount,0)) as ChargeAmount " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID " +
                " AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as ChargeAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.OtherTypeAmount,0)) as OtherTypeAmount " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID " +
                " AND custpay.UserID = " + ReadyString(Web.PeopleUserID()) + ") " +
                ",0) as OtherTypeAmount " +
                ",0 as GiftCertAmount " +
                ",0 as LoyaltyCardAmount " +
                ",0 as CreditTermsAmount " +
                "," + (pData.RangeType = "Today" ? ReadyString(strCurDate) : ReadyString(strYesterdayDate)) + " as SalesDate " +
                " FROM private.LibraryDeviceFowst devf " +
                " WHERE devf.FowstID = " + ReadyString(sessionStorage.FowstID);

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.getManagerSalesTransactionSummary = function (pData) {
                var strCustInvDateRange = '';
                var strCustPayDateRange = '';
                var strMemoDateRange = '';
                var strRangeYearFrom = '';
                var strRangeYearTo = '';
                var strRangeFrom = '';
                var strRangeTo = '';
                var strCurDate = (new Date().getFullYear() + '-' + ((new Date().getMonth() + 1) < 10 ? '0' + (new Date().getMonth() + 1) : (new Date().getMonth() + 1)) + '-' + (new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate()));
                var strYesterdayDate = '';
                var newDate = new Date();
                newDate.setDate(newDate.getDate() - 1);
                strYesterdayDate = (newDate.getFullYear() + '-' + ((newDate.getMonth() + 1) < 10 ? '0' + (newDate.getMonth() + 1) : (newDate.getMonth() + 1)) + '-' + (newDate.getDate() < 10 ? '0' + newDate.getDate() : newDate.getDate()));

                strRangeYearFrom = new Date().getFullYear();
                strRangeYearTo = new Date().getFullYear();

                if (pData.RangeYear == "This year.") {
                    strRangeYearFrom = new Date().getFullYear();
                }
                else if (pData.RangeYear == "This year and last year.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 1;
                }
                else if (pData.RangeYear == "This year and last 2 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 2;
                }
                else if (pData.RangeYear == "This year and last 3 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 3;
                }
                else if (pData.RangeYear == "This year and last 4 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 4;
                }
                else if (pData.RangeYear == "This year and last 5 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 5;
                }

                if (pData.RangeType == "Today") {
                    strRangeFrom = getFOWSTOperationHR(strCurDate, true);
                    strRangeTo = getFOWSTOperationHR(strCurDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Yesterday") {

                    strRangeFrom = getFOWSTOperationHR(strYesterdayDate, true);
                    strRangeTo = getFOWSTOperationHR(strYesterdayDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Day") {
                    var newDateFrom = new Date(pData.RangeFrom);
                    var newDateTo = new Date(pData.Rangeto);

                    strRangeFrom = getFOWSTOperationHR(((newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear()), true);
                    strRangeTo = getFOWSTOperationHR(((newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear()), false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Week") {

                    strCustInvDateRange = " strftime('%w',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%w',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%w',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Month") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Quarter") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Year") {
                    strCustInvDateRange = " strftime('%Y', inv.InvoiceDate) = " + strRangeYear

                    strCustPayDateRange = " strftime('%Y', custpay.PaymentDate) = " + strRangeYear

                    strMemoDateRange = " strftime('%Y', memo.LastUpdated) = " + strRangeYear
                }

                pSql = "SELECT " +
                "IFNULL((SELECT SUM(invitem.SubTotal) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND invitem.TaxAmount > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CurGrossSales " +
                ",IFNULL((SELECT SUM(invitem.DiscountAmount) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND invitem.TaxAmount > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CurSalesDiscount " +
                ",IFNULL((SELECT SUM(srledger.GrossRefundAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID) " +
                ",0) as CurSalesReturnRefund " +
                ",IFNULL((SELECT SUM(srledger.GrossSrvAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID) " +
                ",0) as CurSalesReturnExchange " +
                ",IFNULL((SELECT SUM(invitem.TaxSales) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CurTotalVATSales " +
                ",IFNULL((SELECT SUM(invitem.TaxAmount) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CurTotalVATAmount " +
                ",IFNULL((SELECT SUM(IFNULL(memosr.TotalAmount,0) - IFNULL(memosr.TaxAmount,0)) " +
                "         FROM private.TransMemoSalesReturn memosr " +
                "         INNER JOIN private.TransMemo memo ON (memosr.MemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID) " +
                ",0) as CurTotalVATSalesSalesReturn " +
                ",IFNULL((SELECT SUM(invitem.SubTotal) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE invitem.TaxAmount <= 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CurNoTAXSales " +
                ",0 as CurOthers " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0)) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CashAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.DebitCardAmount,0)) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as DebitCardAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.CreditCardAmount,0)) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CreditCardAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.CheckPaymentAmount,0)) " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CheckPaymentAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.SRVAmount,0)) " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID) " +
                ",0) as SRVAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.ChargeAmount,0)) " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID) " +
                ",0) as ChargeAmount " +
                ",IFNULL((SELECT SUM(IFNULL(custpay.OtherTypeAmount,0)) " +
                " FROM private.TransCustomerPaymentInvoices custpayinv " +
                " INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                " WHERE " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                " AND inv.FOWSTID = devf.FowstID " +
                " AND inv.DeviceID = devf.DeviceID) " +
                ",0) as OtherTypeAmount " +
                ",0 as GiftCertAmount " +
                ",0 as LoyaltyCardAmount " +
                ",0 as CreditTermsAmount " +
                "," + (pData.RangeType = "Today" ? ReadyString(strCurDate) : ReadyString(strYesterdayDate)) + " as SalesDate " +
                ",IFNULL((SELECT COUNT(inv.InvoiceNumber) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND " + (strCustInvDateRange = !"" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CurTransCountInv " +
                ",IFNULL((SELECT COUNT(invitem.ProductID) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) as CurTransInvItemCount " +
                ",IFNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND srledger.SrvNum > 0) " +
                ",0) as CurTransCountMemoReturnExchange " +
                ",IFNULL((SELECT COUNT(memoret.ProductID) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         INNER JOIN private.TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND srledger.SrvNum > 0) " +
                ",0) as CurSalesReturnExchangeItemCount " +
                ",IFNULL((SELECT DISTINCT COUNT(memo.MemoNumber) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND srledger.RefundNum > 0) " +
                ",0) as CurTransCountMemoRefund " +
                ",IFNULL((SELECT COUNT(memoret.ProductID) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         INNER JOIN private.TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND srledger.RefundNum > 0) " +
                ",0) as CurSalesRefundItemCount " +
                " FROM private.LibraryDeviceFowst devf " +
                " WHERE devf.FowstID = " + ReadyString(sessionStorage.FowstID);

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.getManagerSalesReportPaymentInfoByCashier = function (pData) {
                var strCustInvDateRange = '';
                var strCustPayDateRange = '';
                var strMemoDateRange = '';
                var strRangeYearFrom = '';
                var strRangeYearTo = '';
                var strRangeFrom = '';
                var strRangeTo = '';
                var strCurDate = (new Date().getFullYear() + '-' + ((new Date().getMonth() + 1) < 10 ? '0' + (new Date().getMonth() + 1) : (new Date().getMonth() + 1)) + '-' + (new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate()));
                var strYesterdayDate = '';
                var newDate = new Date();
                newDate.setDate(newDate.getDate() - 1);
                strYesterdayDate = (newDate.getFullYear() + '-' + ((newDate.getMonth() + 1) < 10 ? '0' + (newDate.getMonth() + 1) : (newDate.getMonth() + 1)) + '-' + (newDate.getDate() < 10 ? '0' + newDate.getDate() : newDate.getDate()));

                strRangeYearFrom = new Date().getFullYear();
                strRangeYearTo = new Date().getFullYear();

                if (pData.RangeYear == "This year.") {
                    strRangeYearFrom = new Date().getFullYear();
                }
                else if (pData.RangeYear == "This year and last year.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 1;
                }
                else if (pData.RangeYear == "This year and last 2 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 2;
                }
                else if (pData.RangeYear == "This year and last 3 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 3;
                }
                else if (pData.RangeYear == "This year and last 4 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 4;
                }
                else if (pData.RangeYear == "This year and last 5 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 5;
                }

                if (pData.RangeType == "Today") {
                    strRangeFrom = getFOWSTOperationHR(strCurDate, true);
                    strRangeTo = getFOWSTOperationHR(strCurDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Yesterday") {

                    strRangeFrom = getFOWSTOperationHR(strYesterdayDate, true);
                    strRangeTo = getFOWSTOperationHR(strYesterdayDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Day") {
                    var newDateFrom = new Date(pData.RangeFrom);
                    var newDateTo = new Date(pData.Rangeto);

                    strRangeFrom = getFOWSTOperationHR(((newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear()), true);
                    strRangeTo = getFOWSTOperationHR(((newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear()), false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Week") {

                    strCustInvDateRange = " strftime('%w',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%w',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%w',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Month") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Quarter") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Year") {
                    strCustInvDateRange = " strftime('%Y', inv.InvoiceDate) = " + strRangeYear

                    strCustPayDateRange = " strftime('%Y', custpay.PaymentDate) = " + strRangeYear

                    strMemoDateRange = " strftime('%Y', memo.LastUpdated) = " + strRangeYear
                }

                pSql = "SELECT * FROM (SELECT " +
                "IFNULL(NULLIF(TRIM(cshr.Surname) || ', ',', '), '') || " +
                "IFNULL(NULLIF(TRIM(cshr.FirstName) || ' ',' '), '') || " +
                "IFNULL(NULLIF(TRIM(cshr.MiddleName) || ' ',' '), '')  as CashierName " +
                ",IFNULL((SELECT SUM(invitem.SubTotal - IFNULL(invitem.DiscountAmount,0)) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND invitem.TaxAmount > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = orgemp.PeopleID) " +
                ",0) - " +
                " IFNULL((SELECT SUM(srledger.GrossRefundAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND memo.UserID = orgemp.PeopleID) " +
                ",0) - " +
                " IFNULL((SELECT SUM(srledger.GrossSrvAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND memo.UserID = orgemp.PeopleID) " +
                ",0) as TotalSales " +
                " FROM private.LibraryDeviceFowst devf, LibraryOrganizationEmployee orgemp " +
                " INNER JOIN LibraryPeople cshr ON (orgemp.PeopleID = cshr.ID) " +
                " WHERE devf.FowstID = " + ReadyString(sessionStorage.FowstID) +
                ") fin WHERE TotalSales <> 0 ";

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.getManagerSalesReportPaymentInfoByMachine = function (pData) {
                var strCustInvDateRange = '';
                var strCustPayDateRange = '';
                var strMemoDateRange = '';
                var strRangeYearFrom = '';
                var strRangeYearTo = '';
                var strRangeFrom = '';
                var strRangeTo = '';
                var strCurDate = (new Date().getFullYear() + '-' + ((new Date().getMonth() + 1) < 10 ? '0' + (new Date().getMonth() + 1) : (new Date().getMonth() + 1)) + '-' + (new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate()));
                var strYesterdayDate = '';
                var newDate = new Date();
                newDate.setDate(newDate.getDate() - 1);
                strYesterdayDate = (newDate.getFullYear() + '-' + ((newDate.getMonth() + 1) < 10 ? '0' + (newDate.getMonth() + 1) : (newDate.getMonth() + 1)) + '-' + (newDate.getDate() < 10 ? '0' + newDate.getDate() : newDate.getDate()));

                strRangeYearFrom = new Date().getFullYear();
                strRangeYearTo = new Date().getFullYear();

                if (pData.RangeYear == "This year.") {
                    strRangeYearFrom = new Date().getFullYear();
                }
                else if (pData.RangeYear == "This year and last year.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 1;
                }
                else if (pData.RangeYear == "This year and last 2 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 2;
                }
                else if (pData.RangeYear == "This year and last 3 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 3;
                }
                else if (pData.RangeYear == "This year and last 4 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 4;
                }
                else if (pData.RangeYear == "This year and last 5 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 5;
                }

                if (pData.RangeType == "Today") {
                    strRangeFrom = getFOWSTOperationHR(strCurDate, true);
                    strRangeTo = getFOWSTOperationHR(strCurDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Yesterday") {

                    strRangeFrom = getFOWSTOperationHR(strYesterdayDate, true);
                    strRangeTo = getFOWSTOperationHR(strYesterdayDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Day") {
                    var newDateFrom = new Date(pData.RangeFrom);
                    var newDateTo = new Date(pData.Rangeto);

                    strRangeFrom = getFOWSTOperationHR(((newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear()), true);
                    strRangeTo = getFOWSTOperationHR(((newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear()), false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Week") {

                    strCustInvDateRange = " strftime('%w',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%w',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%w',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Month") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Quarter") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Year") {
                    strCustInvDateRange = " strftime('%Y', inv.InvoiceDate) = " + strRangeYear

                    strCustPayDateRange = " strftime('%Y', custpay.PaymentDate) = " + strRangeYear

                    strMemoDateRange = " strftime('%Y', memo.LastUpdated) = " + strRangeYear
                }

                pSql = "SELECT * FROM (SELECT " +
                "TRIM(CAST(fowst.FOWSTNo as CHAR)) || '-' || TRIM(CAST(dev.DeviceNo as CHAR)) as DeviceName " +
                ",IFNULL((SELECT SUM(invitem.SubTotal - IFNULL(invitem.DiscountAmount,0)) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND invitem.TaxAmount > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID) " +
                ",0) - " +
                " IFNULL((SELECT SUM(srledger.GrossRefundAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID) " +
                ",0) - " +
                " IFNULL((SELECT SUM(srledger.GrossSrvAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID) " +
                ",0) as TotalSales " +
                " FROM private.LibraryDeviceFowst devf " +
                " INNER JOIN private.LibraryDevice dev ON (devf.DeviceID = dev.ID) " +
                " LEFT JOIN LibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) " +
                " WHERE devf.FowstID = " + ReadyString(sessionStorage.FowstID) +
                ") fin WHERE TotalSales <> 0 ";

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.getManagerSalesReportPaymentInfoByCashierMachine = function (pData) {
                var strCustInvDateRange = '';
                var strCustPayDateRange = '';
                var strMemoDateRange = '';
                var strRangeYearFrom = '';
                var strRangeYearTo = '';
                var strRangeFrom = '';
                var strRangeTo = '';
                var strCurDate = (new Date().getFullYear() + '-' + ((new Date().getMonth() + 1) < 10 ? '0' + (new Date().getMonth() + 1) : (new Date().getMonth() + 1)) + '-' + (new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate()));
                var strYesterdayDate = '';
                var newDate = new Date();
                newDate.setDate(newDate.getDate() - 1);
                strYesterdayDate = (newDate.getFullYear() + '-' + ((newDate.getMonth() + 1) < 10 ? '0' + (newDate.getMonth() + 1) : (newDate.getMonth() + 1)) + '-' + (newDate.getDate() < 10 ? '0' + newDate.getDate() : newDate.getDate()));

                strRangeYearFrom = new Date().getFullYear();
                strRangeYearTo = new Date().getFullYear();

                if (pData.RangeYear == "This year.") {
                    strRangeYearFrom = new Date().getFullYear();
                }
                else if (pData.RangeYear == "This year and last year.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 1;
                }
                else if (pData.RangeYear == "This year and last 2 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 2;
                }
                else if (pData.RangeYear == "This year and last 3 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 3;
                }
                else if (pData.RangeYear == "This year and last 4 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 4;
                }
                else if (pData.RangeYear == "This year and last 5 years.") {
                    strRangeYearFrom = (new Date().getFullYear()) - 5;
                }

                if (pData.RangeType == "Today") {
                    strRangeFrom = getFOWSTOperationHR(strCurDate, true);
                    strRangeTo = getFOWSTOperationHR(strCurDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Yesterday") {

                    strRangeFrom = getFOWSTOperationHR(strYesterdayDate, true);
                    strRangeTo = getFOWSTOperationHR(strYesterdayDate, false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Day") {
                    var newDateFrom = new Date(pData.RangeFrom);
                    var newDateTo = new Date(pData.Rangeto);

                    strRangeFrom = getFOWSTOperationHR(((newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear()), true);
                    strRangeTo = getFOWSTOperationHR(((newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear()), false);

                    strCustInvDateRange = " inv.InvoiceDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strCustPayDateRange = " custpay.PaymentDate BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                    strMemoDateRange = " memo.LastUpdated BETWEEN " + ReadyString(strRangeFrom) + " AND " + ReadyString(strRangeTo)
                }
                else if (pData.RangeType == "Week") {

                    strCustInvDateRange = " strftime('%w',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%w',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%w',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Month") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Quarter") {

                    strCustInvDateRange = " strftime('%m',inv.InvoiceDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',inv.InvoiceDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strCustPayDateRange = " strftime('%m',custpay.PaymentDate) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',custpay.PaymentDate) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo

                    strMemoDateRange = " strftime('%m',memo.LastUpdated) BETWEEN " + pData.RangeFrom + " AND " + pData.Rangeto +
                        " AND strftime('%Y',memo.LastUpdated) BETWEEN " + strRangeYearFrom + " AND " + strRangeYearTo
                }
                else if (pData.RangeType == "Year") {
                    strCustInvDateRange = " strftime('%Y', inv.InvoiceDate) = " + strRangeYear

                    strCustPayDateRange = " strftime('%Y', custpay.PaymentDate) = " + strRangeYear

                    strMemoDateRange = " strftime('%Y', memo.LastUpdated) = " + strRangeYear
                }

                pSql = "SELECT * FROM (SELECT " +
                "IFNULL(NULLIF(TRIM(cshr.Surname) || ', ',', '), '') || " +
                "IFNULL(NULLIF(TRIM(cshr.FirstName) || ' ',' '), '') || " +
                "IFNULL(NULLIF(TRIM(cshr.MiddleName) || ' ',' '), '') as CashierName" +
                ",IFNULL((SELECT TRIM(CAST(minfowst.FOWSTNo as CHAR)) || '-' || TRIM(CAST(mindev.DeviceNo as CHAR)) " +
                "        FROM private.LibraryDeviceFowst mindevf " +
                "        INNER JOIN private.LibraryDevice mindev ON (mindevf.DeviceID = mindev.ID) " +
                "        LEFT JOIN LibraryOrganizationFOWST minfowst ON (mindevf.FowstID = minfowst.ID) " +
                "        WHERE mindevf.ID = devf.ID LIMIT 1),'') as DeviceName " +
                ",IFNULL((SELECT SUM(invitem.SubTotal - IFNULL(invitem.DiscountAmount,0)) " +
                "         FROM private.TransCustomerPaymentInvoices custpayinv " +
                "         INNER JOIN private.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                "         INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                "         INNER JOIN private.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                "         WHERE inv.InvoiceNumber > 0 " +
                "         AND invitem.TaxAmount > 0 " +
                "         AND " + (strCustInvDateRange != "" ? "(" + strCustInvDateRange + ")" : " inv.InvoiceDate = " + ReadyString(strCurDate)) +
                "         AND inv.FOWSTID = devf.FowstID " +
                "         AND inv.DeviceID = devf.DeviceID " +
                "         AND custpay.UserID = orgemp.PeopleID) " +
                ",0) - " +
                " IFNULL((SELECT SUM(srledger.GrossRefundAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND memo.UserID = orgemp.PeopleID) " +
                ",0) - " +
                " IFNULL((SELECT SUM(srledger.GrossSrvAmount) " +
                "         FROM private.TransSalesReturnLedger srledger " +
                "         INNER JOIN private.TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                "         WHERE " + (strMemoDateRange != "" ? "(" + strMemoDateRange + ")" : " CAST(memo.LastUpdated AS DATE) = " + ReadyString(strCurDate)) +
                "         AND memo.FOWSTID = devf.FowstID " +
                "         AND memo.DeviceID = devf.DeviceID " +
                "         AND memo.UserID = orgemp.PeopleID) " +
                ",0) as TotalSales " +
                " FROM private.LibraryDeviceFowst devf, LibraryOrganizationEmployee orgemp " +
                " INNER JOIN LibraryPeople cshr ON (orgemp.PeopleID = cshr.ID) " +
                " WHERE devf.FowstID = " + ReadyString(sessionStorage.FowstID) +
                ") fin WHERE TotalSales <> 0 ";

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.getCashierTurnOverAndDeposits = function (strRangeFrom) {
                
                pSql = 'SELECT ' + 
                    'IsTurnOver ' + 
                    ', IFNULL(ppl.FirstName || \' \' || ppl.SurName, IFNULL(DepositedToByName,\'\') || \' of \' || org.CompanyName) DepositedTo ' +
                    ',SUM(ToTalAmount) TotalAmount ' + 
                    'FROM TransCashHandling ' + 
                    'LEFT JOIN LibraryPeople ppl ' + 
                    'ON ppl.ID = DepositedTo ' + 
                    'LEFT JOIN LibraryOrganization org ' + 
                    'ON org.ID = DepositedTo ' + 
                    'where DepositedBy = ' + ReadyString(Web.PeopleUserID()) +
                    ' AND date(DepositDate) between ' + ReadyString(strRangeFrom) + ' AND ' + ReadyString(strRangeFrom) +
                    ' Group By IsTurnOver, ppl.FirstName, ppl.SurName, CompanyName '

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            function getFOWSTOperationHR(dtDate, IsOpening) {
                var dtReturn = null;

                pSql = "SELECT opthr.ID " +
                    " ,opthr.FOWSTID as FOWSTID " +
                    " ,opthr.WeekDay as WeekDay " +
                    " ,strftime('%H:%M:%S',opthr.OpeningTime) as OpeningTime " +
                    " ,strftime('%H:%M:%S',opthr.ClosingTime) as ClosingTime " +
                    " FROM LibraryOrganizationFOWSTOperationHRs opthr " +
                    " WHERE opthr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                    " AND opthr.WeekDay = (strftime('%w','" + dtDate + "') + 1) ";

                var pDT = Sql.Execute(pSql);

                if (pDT.Rows.length > 0) {
                    if (IsOpening) {
                        dtReturn = dtDate + " " + pDT.Rows[0].OpeningTime;
                    } else {
                        dtReturn = dtDate + " " + pDT.Rows[0].ClosingTime;
                    }

                } else if (IsOpening) {
                    dtReturn = dtDate
                } else {
                    var newDate = new Date(dtDate);
                    newDate.setDate(newDate.getDate() + 1);
                    dtReturn = newDate.getFullYear() + '-' + ((newDate.getMonth() + 1) < 10 ? '0' + (newDate.getMonth() + 1) : (newDate.getMonth() + 1)) + '-' + newDate.getDate();
                }

                return dtReturn;

            }

            return this;

        }; // SalesPayment_offline

    });