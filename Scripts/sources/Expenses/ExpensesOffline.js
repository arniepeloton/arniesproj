define(['common'], function () {
    CreateStringObject('App.model.Expenses', null, function () {
        var pData = {}, pSql, pDT;

        pData.CountUnflushedData = function () {
            pSql = " SELECT SUM(total) as Total FROM ( \n " +
                    " SELECT COUNT(ID) as total FROM TransExpensesCheckInOutDetails WHERE IFNULL(DateImported, '') = '' " +
                    " UNION ALL " +
                    " SELECT COUNT(ID) as total FROM TransExpensesReceiveFunds WHERE IFNULL(DateImported, '') = '' \n " +
                    " UNION ALL " +
                    " SELECT COUNT(ID) as total FROM TransExpensesLogDetails WHERE IFNULL(DateImported, '') = '' \n " +
                    " UNION ALL " +
                    " SELECT COUNT(ID) as total FROM TransNetFundBalance WHERE IFNULL(DateImported, '') = '' \n " +
                    " ) "
            pDT = Sql.Execute(pSql).Rows

            if (pDT.length > 0) {
                return CDec2(pDT[0].Total);
            } else {
                return 0;
            }
        }

        pData.SaveCarryOverFunds = function (e) {
            var pIU = new InsertUpdate('TransNetFundBalance');
            var newID = NewID();
            var prevmonth = new Date(new Date().setMonth(new Date().getMonth() - 1));
            var month = $.format.date(prevmonth, 'MM');
            var year = $.format.date(prevmonth, 'yyyy');

            try {
                pIU.Add("OriginalAmount", ReadyString(e.OrigAmount));
                pIU.Add("AdjustedAmount", ReadyString(e.AdjustedAmount));
                pIU.Add("Remarks", ReadyString(e.Remarks));
                pIU.Add("FowstID", ReadyString(e.FowstID));
                pIU.Add("LastUpdated", proj.SqliteNow);                
                pIU.Add("PeopleID", ReadyString(Web.PeopleUserID()));
                pIU.Add("MonthCovered", ReadyString(month))
                pIU.Add("YearCovered", ReadyString(year));
                pIU.Add("Status", ReadyString('Pending'));
                pIU.Add("IsAdjustment", 1);
                pIU.Add("IsCancelRequest", 0);

                if (CStr2(e.ID) == '') {
                    pIU.Add("ID", ReadyString(newID));
                    pIU.Add("RequestedDate", proj.SqliteNow);
                    pSql = pIU.GetInsert();
                } else {
                    newID = e.ID;
                    pIU.Add("DateImported", 'NULL');
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(e.ID));
                }

                Sql.BeginTransaction();
                Sql.ExecuteSql(pSql);
                Sql.CommitTransaction();

                return newID;
            } catch (e) {
                return e.message;
            }
        }

        pData.CarryOverFunds = function (fowstid) {
            var prevmonth = new Date(new Date().setDate(0));
            pSql = "SELECT NetFundBalance as TotalBalance, MonthCovered, YearCovered \n " +
                    " FROM TransNetFundBalance \n " +
                    " WHERE (MonthCovered = " + $.format.date(prevmonth, 'MM') + " \n " +
                    " AND YearCovered = " + $.format.date(prevmonth, 'yyyy') + ") \n " +
                    " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) +
                    " AND CASE WHEN IFNULL(IsAdjustment, 0) = 1 THEN Status = 'Approved' ELSE IFNULL(IsAdjustment, 0) = 0 END " + " \n" +
                    " AND IFNULL(IsCancelRequest, 0) = 0 " + " \n " +
                    " AND FowstID = " + ReadyString(fowstid) + " \n" +
                    " ORDER BY IsAdjustment DESC, LastUpdated DESC, Status ASC LIMIT 1";

            return Sql.Execute(pSql);
        }

        pData.RequestAdjustFund = function () {
            var prevmonth = new Date(new Date().setMonth(new Date().getMonth() - 1));
            pSql = "SELECT * \n " +
                    " FROM TransNetFundBalance \n " +
                    " WHERE IsAdjustment = 1 " + " \n " +
                    " AND IFNULL(IsCancelRequest, 0) = 0 " + " \n " +
                    " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) +
                    " AND (MonthCovered = " + $.format.date(prevmonth, 'MM') + " \n " +
                    " AND YearCovered = " + $.format.date(prevmonth, 'yyyy') + ") \n " +
                    " AND Status = 'Pending' \n";

            return Sql.Execute(pSql);
        }

        pData.CancelRequestFA = function (id) {
            var pIU = new InsertUpdate('TransNetFundBalance');

            try {
                pIU.Add("IsCancelRequest", 1);
                pIU.Add("DateImported", 'NULL');

                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                Sql.BeginTransaction();
                Sql.ExecuteSql(pSql);
                Sql.CommitTransaction();

                return 'saved';
            } catch(e) {
                return e.message;
            }
        }

        pData.UpdateRAF = function (e) {
            var pIU = new InsertUpdate('TransNetFundBalance');

            try {
                pIU.Add("LastUpdated", ReadyString(e.LastUpdated));
                pIU.Add("NetFundBalance", ReadyString(e.NetFundBalance))
                pIU.Add("RequestUpdatedBy", ReadyString(e.RequestUpdatedBy));
                pIU.Add("Status", ReadyString(e.Status));

                pSql = pIU.GetUpdate('ID = ' + ReadyString(e.ID));

                Sql.BeginTransaction();
                Sql.ExecuteSql(pSql);

                Sql.CommitTransaction();

                return 'saved';
            } catch (e) {
                return e.message;
            }
        }

        pData.GetLastCheckIn = function () {
            pSql = "SELECT * FROM TransExpensesCheckInOutDetails \n" +
                    " Where ifnull(CheckIn, '') <> '' AND ifnull(CheckOut, '') = ''  " +
                    " AND PeopleID = " + ReadyString(Web.PeopleUserID()) +
                    "  ORDER BY LastUpdated DESC LIMIT 1 "

            return Sql.Execute(pSql);
        }

        pData.SaveLogExpenseCache = function (e, isDataFromServer) {
            var pIU = new InsertUpdate('TransLogExpensesCache');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                pIU.Add("ExpenseType", ReadyString(e.ExpenseType));
                pIU.Add("Amount", ReadyString(e.Amount));
                pIU.Add("MonthCovered", ReadyString(e.MonthCovered));
                pIU.Add("YearCovered", ReadyString(e.YearCovered));
                pIU.Add("NoOfRecords", CStr2(e.NoOfRecords) == '' ? 'NULL' : ReadyString(e.NoOfRecords));
                pIU.Add("FowstID", CStr2(e.FowstID) == '' ? 'NULL' : ReadyString(e.FowstID));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("PeopleID", ReadyString(Web.PeopleUserID()));
                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                if (CBool(isDataFromServer) == false) {
                    Sql.BeginTransaction();
                }

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(id));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.CommitTransaction();
                }

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Log Expense Cache: ' + e.message)
                return 'error';
            }
        }

        pData.SaveLogExpense = function (e, isDataFromServer) {
            var pIU = new InsertUpdate('TransExpensesLogDetails');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                pIU.Add("ExpenseType", ReadyString(e.ExpenseType));
                pIU.Add("ExpenseDate", ReadyString(e.ExpenseDate));
                pIU.Add("FromLocation", ReadyString(e.From));
                pIU.Add("ToLocation", ReadyString(e.To));
                pIU.Add("Amount", ReadyString(e.Amount));
                pIU.Add("AssociatedTo", ReadyString(e.AssociatedTo));
                pIU.Add("FowstID", ReadyString(e.FowstID));
                pIU.Add("CostCenter", ReadyString(e.CostCenter));
                pIU.Add("Remarks", ReadyString(e.Remarks));
                pIU.Add("GPSLocation", ReadyString(e.GPSLocation));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("PeopleID", ReadyString(Web.PeopleUserID()));
                pIU.Add("IsFromCheckInOut", CInt2(e.IsFromCheckInOut));
                pIU.Add("IsDelete", e.IsDelete);
                pIU.Add("EncodedBy", (CStr2(e.EncodedBy) == '' ? 'NULL' : ReadyString(e.EncodedBy)));
                pIU.Add("LastUpdatedBy", (CStr2(e.LastUpdatedBy) == '' ? 'NULL' : ReadyString(e.LastUpdatedBy)));                
                pIU.Add("DateImported", (CStr2(e.DateImported) == '' ? 'NULL' : ReadyString($.format.date(new Date(e.DateImported), 'yyyy-MM-dd HH:mm:ss.000'))));
                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                if (CBool(isDataFromServer)) {
                    pIU.Add("DateCreated", (CStr2(e.DateCreated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.DateCreated), 'yyyy-MM-dd HH:mm:ss.000'))));
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.BeginTransaction();
                }

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {    
                    pIU.Add("ID", ReadyString(id));
                    pIU.Add("DateCreated", (CStr2(e.DateCreated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.DateCreated), 'yyyy-MM-dd HH:mm:ss.000'))));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                //pData.UpdateNetFundBalance(e.ExpenseDate);

                if (CBool(isDataFromServer) == false) {
                    Sql.CommitTransaction();
                }

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Log Expense: ' + e.message)
                return 'error';
            }
        }

        pData.ScheduleInfo = function (e) {
            pSql = "SELECT IFNULL(Frequency, 'monthly') as Frequency, IFNULL(StrictlyFollowSched, 1) as StrictlyFollowSched \n" +
                    " FROM LibraryExpensesRouteSchedulesEmployees \n" +
                    " WHERE EmployeeID = " + ReadyString(sessionStorage.PeopleID)
            var pInfo = Sql.ExecuteRow(pSql);

            return pInfo;
        }

        pData.SaveCheckInOut = function (e, isDataFromServer) {
            var pIU = new InsertUpdate('TransExpensesCheckInOutDetails');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                if (e.Type == 'in' || isDataFromServer) {
                    pIU.Add("CheckInLocationName", ReadyString(e.CheckInLocationName));
                    pIU.Add("CheckInGPSLocation", ReadyString(e.CheckInGPSLocation));
                    pIU.Add("CheckIn", e.CheckIn == '' ? 'NULL' : ReadyString(e.CheckIn));
                    pIU.Add("CheckInLogExpenseID", ReadyString(e.CheckInLogExpenseID));
                    pIU.Add("CheckInCustomerOrgID", e.CustomerOrgID == '' ? 'NULL' : ReadyString(e.CustomerOrgID));
                    pIU.Add("CheckInCustomerOrgOutletID", e.CustomerOrgOutletID == '' ? 'NULL' : ReadyString(e.CustomerOrgOutletID));
                }

                if (e.Type == 'out' || isDataFromServer) {
                    pIU.Add("CheckOutLocationName", ReadyString(e.CheckOutLocationName));
                    pIU.Add("CheckOutGPSLocation", ReadyString(e.CheckOutGPSLocation));
                    pIU.Add("CheckOut", e.CheckOut == '' ? 'NULL' : ReadyString(e.CheckOut));
                    pIU.Add("CheckOutLogExpenseID", ReadyString(e.CheckOutLogExpenseID));
                    pIU.Add("CheckOutCustomerOrgID", e.CustomerOrgID == '' ? 'NULL' : ReadyString(e.CustomerOrgID));
                    pIU.Add("CheckOutCustomerOrgOutletID", e.CustomerOrgOutletID == '' ? 'NULL' : ReadyString(e.CustomerOrgOutletID));
                }
                
                pIU.Add("Remarks", ReadyString(e.Remarks));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("DateImported", (CStr2(e.DateImported) == '' ? 'NULL' : ReadyString($.format.date(new Date(e.DateImported), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("PeopleID", ReadyString(Web.PeopleUserID()));
                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                if (CBool(isDataFromServer) == false) {
                    Sql.BeginTransaction();
                }

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(id));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.CommitTransaction();
                }

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Check In/Out: ' + e.message)
                return 'error';
            }
        }

        pData.SaveReceiveFundCache = function (e, isDataFromServer) {
            var pIU = new InsertUpdate('TransReceiveFundsCache');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                pIU.Add("Amount", CDec2(e.Amount));
                pIU.Add("MonthCovered", CDec2(e.MonthCovered));
                pIU.Add("YearCovered", CDec2(e.YearCovered));
                pIU.Add("FowstID", CStr2(e.FowstID) == '' ? 'NULL' : ReadyString(e.FowstID));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("PeopleID", ReadyString(Web.PeopleUserID()));
                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                if (CBool(isDataFromServer) == false) {
                    Sql.BeginTransaction();
                }

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(id));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.CommitTransaction();
                }

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Log Expense Cache: ' + e.message)
                return 'error';
            }
        }

        pData.SaveReceiveFund = function (e, isDataFromServer) {
            var pIU = new InsertUpdate('TransExpensesReceiveFunds');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                pIU.Add("ReceiveDate", ReadyString($.format.date(new Date(e.ReceiveDate), 'yyyy-MM-dd')));
                pIU.Add("ReceiveFrom", (e.ReceiveFrom == null ? 'NULL' : ReadyString(e.ReceiveFrom)));
                pIU.Add("OtherPerson", (e.ReceiveFrom == null ? ReadyString(e.OtherPerson) : 'NULL'));
                pIU.Add("Amount", ReadyString(e.Amount));
                pIU.Add("Comment", ReadyString(e.Comment));
                pIU.Add("FowstID", ReadyString(e.FowstID));
                pIU.Add("IsDelete", (CStr2(e.IsDelete) == '' ? 0 : CInt2(e.IsDelete)));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("PeopleID", ReadyString(Web.PeopleUserID()));
                pIU.Add("EncodedBy", (CStr2(e.EncodedBy) == '' ? 'NULL' : ReadyString(e.EncodedBy)));
                pIU.Add("LastUpdatedBy", (CStr2(e.LastUpdatedBy) == '' ? 'NULL' : ReadyString(e.LastUpdatedBy)));
                pIU.Add("DateImported", (CStr2(e.DateImported) == '' ? 'NULL' : ReadyString($.format.date(new Date(e.DateImported), 'yyyy-MM-dd HH:mm:ss.000'))));
                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                if (CBool(isDataFromServer)) {
                    pIU.Add("DateCreated", (CStr2(e.DateCreated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.DateCreated), 'yyyy-MM-dd HH:mm:ss.000'))));
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.BeginTransaction();
                }

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(id));
                    pIU.Add("DateCreated", (CStr2(e.DateCreated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.DateCreated), 'yyyy-MM-dd HH:mm:ss.000'))));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.CommitTransaction();
                }

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Received Fund: ' + e.message)
                return 'error';
            }
        }

        pData.SaveNetFundBalance = function (e, isDataFromServer) {
            var pIU = new InsertUpdate('TransNetFundBalance');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                pIU.Add("NetFundBalance", ReadyString(e.NetFundBalance));
                pIU.Add("MonthCovered", ReadyString(e.MonthCovered));
                pIU.Add("YearCovered", ReadyString(e.YearCovered));
                pIU.Add("IsAdjustment", (CStr2(e.IsAdjustment) == '' ? 'NULL' : CInt2(e.IsAdjustment)));
                pIU.Add("OriginalAmount", (CStr2(e.OriginalAmount) == '' ? 'NULL' : ReadyString(e.OriginalAmount)));
                pIU.Add("FowstID", ReadyString(e.FowstID));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("PeopleID", ReadyString(Web.PeopleUserID()));
                pIU.Add("EncodedBy", (CStr2(e.EncodedBy) == '' ? 'NULL' : ReadyString(e.EncodedBy)));
                pIU.Add("Status", (CStr2(e.Status) == '' ? 'NULL' : ReadyString(e.Status)));
                pIU.Add("DateImported", (CStr2(e.DateImported) == '' ? 'NULL' : ReadyString($.format.date(new Date(e.DateImported), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("RequestUpdatedBy", (CStr2(e.RequestUpdatedBy) == '' ? 'NULL' : ReadyString(e.RequestUpdatedBy)));
                pIU.Add("AdjustmentType", (CStr2(e.AdjustmentType) == '' ? 'NULL' : ReadyString(e.AdjustmentType)));
                pIU.Add("Remarks", (CStr2(e.Remarks) == '' ? 'NULL' : ReadyString(e.Remarks)));                
                pIU.Add("IsCancelRequest", (CStr2(e.IsCancelRequest) == '' ? 'NULL' : CInt2(e.IsCancelRequest)));
                pIU.Add("FowstID", ReadyString(e.FowstID));
                pIU.Add("EncodedBy", (CStr2(e.EncodedBy) == '' ? 'NULL' : ReadyString(e.EncodedBy)));
                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                if (CBool(isDataFromServer)) {
                    pIU.Add("RequestedDate", (CStr2(e.RequestedDate) == '' ? 'NULL' : ReadyString($.format.date(new Date(e.RequestedDate), 'yyyy-MM-dd HH:mm:ss.000'))));
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.BeginTransaction();
                }

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(id));
                    pIU.Add("RequestedDate", (CStr2(e.RequestedDate) == '' ? 'NULL' : ReadyString($.format.date(new Date(e.RequestedDate), 'yyyy-MM-dd HH:mm:ss.000'))));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                if (CBool(isDataFromServer) == false) {
                    Sql.CommitTransaction();
                }                

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Received Fund: ' + e.message)
                return 'error';
            }
        }

        pData.GetCostCenters = function () {
            pSql = "SELECT * FROM LibraryExpensesCenters \n" +
                    " WHERE Type = 'Cost Centers' ";

            return Sql.Execute(pSql);
        }

        pData.GetLogExpenseHeader = function (fowstid) {
            pSql = "SELECT ExpenseType, SUM(TotalAmount) as TotalAmount FROM (SELECT ExpenseType, SUM(Amount) as TotalAmount from TransExpensesLogDetails \n" +
                    " WHERE strftime('%m %Y', ExpenseDate) = strftime('%m %Y', 'now') \n" +
                    " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    " AND IFNULL(IsDelete, 0) = 0 " + " \n" +
                    " AND FowstID = " + ReadyString(fowstid) + " \n" +
                    " GROUP BY ExpenseType \n" +
                    " --UNION ALL \n" +
                    " --SELECT ExpenseType, SUM(Amount) from TransLogExpensesCache \n" +
                    " --WHERE monthcovered = strftime('%m', 'now') \n" +
                    " --AND yearcovered = strftime('%Y', 'now') \n" +
                    " --AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    " --AND FowstID = " + ReadyString(fowstid) + " \n" +
                    " --GROUP BY ExpenseType \n" +
                    ") as T GROUP BY ExpenseType Order By ExpenseType ASC"

            return Sql.Execute(pSql);
        }

        pData.GetReceiveFundsHeader = function (fowstid) {
            pSql = "SELECT SUM(TotalAmount) as TotalAmount FROM (SELECT SUM(Amount) as TotalAmount from TransExpensesReceiveFunds \n" +
                    " WHERE strftime('%m %Y', ReceiveDate) = strftime('%m %Y', 'now') \n" +
                    " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    " AND IFNULL(IsDelete, 0) = 0 " + " \n" +
                    " AND FowstID = " + ReadyString(fowstid) + " \n" +
                    " --UNION ALL \n" +
                    " --SELECT SUM(Amount) from TransReceiveFundsCache \n" +
                    " --WHERE monthcovered = strftime('%m', 'now') \n" +
                    " --AND yearcovered = strftime('%Y', 'now') \n" +
                    " --AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    " --AND FowstID = " + ReadyString(fowstid) + " \n" +
                    ") as T"

            return Sql.Execute(pSql);
        }

        pData.GetReceiveFundsHeaderDetails = function (fowstid) {
            pSql = "SELECT fund.*, CASE IFNULL(fund.ReceiveFrom, '') WHEN '' THEN fund.OtherPerson ELSE ppol.FirstName || ' ' || ppol.Surname END PeopleName " +
                    " FROM TransExpensesReceiveFunds as fund \n" +
                    " LEFT JOIN LibraryPeople as ppol ON ppol.ID = fund.ReceiveFrom " +
                    " WHERE strftime('%m %Y', fund.ReceiveDate) = strftime('%m %Y', 'now') \n" +
                    " AND fund.PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    " AND IFNULL(fund.IsDelete, 0) = 0 \n" +
                    " AND FowstID = " + ReadyString(fowstid) + " \n" +
                    " ORDER BY ReceiveDate DESC "

            return Sql.Execute(pSql);
        }

        pData.ViewExpenseReportsCacheHeaderDetails = function (data) {
            pSql = "SELECT * FROM TransLogExpensesCache \n" +
                    " WHERE ExpenseType = " + ReadyString(data.ExpenseType) +
                    " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) +
                    " AND FowstID = " + ReadyString(data.FowstID) +
                    " \n AND MonthCovered = strftime('%m', 'NOW')+0" +
                    " \n AND YearCovered = strftime('%Y', 'NOW')+0"

            return Sql.Execute(pSql);
        }

        pData.ViewExpenseReportsHeaderDetails = function (data) {
            var m_DateFrom = new Date();
            var m_DateTo = new Date();

            pSql = "SELECT log.*, org.CompanyName FROM TransExpensesLogDetails as log \n" +
                    " LEFT JOIN LibraryOrganization as org ON org.ID = log.AssociatedTo \n" +
                    " WHERE ExpenseType = " + ReadyString(data.ExpenseType) +
                    " AND log.PeopleID = " + ReadyString(sessionStorage.PeopleID) +
                    " AND FowstID = " + ReadyString(data.FowstID) +
                    " AND IFNULL(log.IsDelete, 0) = 0 \n "

            if (data.Period == 'today') {
                pSql += " AND DATE(ExpenseDate) = DATE('NOW') ";
            } else if (data.Period == 'week') {
                m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());
                pSql += " AND DATE(ExpenseDate) BETWEEN DATE('" + $.format.date(m_DateFrom, 'yyyy-MM-dd') + "') AND DATE('" + $.format.date(m_DateTo, 'yyyy-MM-dd') + "') ";
            } else if (data.Period == 'month') {
                m_DateFrom.setDate(m_DateFrom.getDate() - (m_DateFrom.getDate() - 1));
                pSql += " AND DATE(ExpenseDate) BETWEEN DATE('" + $.format.date(m_DateFrom, 'yyyy-MM-dd') + "') AND DATE('" + $.format.date(m_DateTo, 'yyyy-MM-dd') + "') ";
            }

            pSql += " ORDER BY ExpenseDate " + data.Sort;

            return Sql.Execute(pSql);
        }

        pData.GetCheckinDetails = function (order){
            pSql = "SELECT checks.*, \n" +
                    " logcheckin.Amount as CheckinAmount, logcheckin.FromLocation as CheckinFromLocation, logcheckin.ToLocation as CheckinToLocation, \n" +
                    " logcheckout.Amount as CheckoutAmount, logcheckout.FromLocation as CheckoutFromLocation, logcheckout.ToLocation as CheckoutToLocation \n" +
                    " FROM TransExpensesCheckInOutDetails as checks \n" +
                    " LEFT JOIN TransExpensesLogDetails as logcheckin ON logcheckin.ID = checks.CheckInLogExpenseID \n" +
                    " LEFT JOIN TransExpensesLogDetails as logcheckout ON logcheckout.ID = checks.CheckOutLogExpenseID \n" +
                    " WHERE checks.PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n " +
                    " AND strftime('%m', checks.CheckIn) = strftime('%m', 'NOW') \n " +
                    " AND strftime('%Y', checks.CheckIn) = strftime('%Y', 'NOW') \n " +
                    " ORDER BY checks.CheckIn " + order

            return Sql.Execute(pSql);
        }

        pData.CheckIfMyDataExistInSqlite = function () {
            if (CInt2(Sql.ExecuteScalar('SELECT COUNT(ID) FROM TransExpensesCheckInOutDetails WHERE PeopleID = ' + ReadyString(sessionStorage.PeopleID))) > 0) {
                return true;
            }

            if (CInt2(Sql.ExecuteScalar('SELECT COUNT(ID) FROM TransExpensesLogDetails WHERE PeopleID = ' + ReadyString(sessionStorage.PeopleID))) > 0) {
                return true;
            }

            if (CInt2(Sql.ExecuteScalar('SELECT COUNT(ID) FROM TransExpensesReceiveFunds WHERE PeopleID = ' + ReadyString(sessionStorage.PeopleID))) > 0) {
                return true;
            }

            return false;
        }

        pData.DeleteLogExpense = function (e) {
            try {
                Sql.BeginTransaction();

                var pIU = new InsertUpdate('TransExpensesCheckInOutDetails');

                //ayaw nalang iremove ang nakabind na log expense sa Check in/out since dili totally deletion ang log expense - by arnie on Aug 6, 2020
                //if (CBool(e.IsFromCheckInOut)) {
                //    pSql = " SELECT CheckInLogExpenseID, CheckOutLogExpenseID FROM TransExpensesCheckInOutDetails " +
                //            ' WHERE CheckInLogExpenseID = ' + ReadyString(e.ID) + ' Or CheckOutLogExpenseID = ' + ReadyString(e.ID)
                //    pDT = Sql.Execute(pSql);

                //    pIU.Add("DateImported", 'NULL');

                //    if (CStr2(pDT.Rows[0].CheckInLogExpenseID) != '') {
                //        pIU.Add("CheckInLogExpenseID", 'NULL');
                //        pSql = pIU.GetUpdate('CheckInLogExpenseID = ' + ReadyString(e.ID));
                //    }

                //    if (CStr2(pDT.Rows[0].CheckOutLogExpenseID) != '') {
                //        pIU.Add("CheckOutLogExpenseID", 'NULL');
                //        pSql = pIU.GetUpdate('CheckOutLogExpenseID = ' + ReadyString(e.ID));
                //    }

                //    Sql.ExecuteSql(pSql);
                //}

                pIU = new InsertUpdate('TransExpensesLogDetails');

                pIU.Add("DateImported", 'NULL');
                pIU.Add("IsDelete", 1);
                pIU.Add("LastUpdated", proj.SqliteNow);

                pSql = pIU.GetUpdate('ID = ' + ReadyString(e.ID));
                
                Sql.ExecuteSql(pSql);

                //pData.UpdateNetFundBalance(e.ExpenseDate);

                Sql.CommitTransaction();

                return true;
            } catch(e){
                Android.AddLog(e.message);
                return false;
            }            
        }

        pData.DeleteFund = function (e) {
            try {
                Sql.BeginTransaction();

                pIU = new InsertUpdate('TransExpensesReceiveFunds');

                pIU.Add("DateImported", 'NULL');
                pIU.Add("IsDelete", 0);
                pIU.Add("LastUpdated", proj.SqliteNow);

                pSql = pIU.GetUpdate('ID = ' + ReadyString(e.ID));

                Sql.ExecuteSql(pSql);

                //pData.UpdateNetFundBalance(e.ReceiveDate);

                Sql.CommitTransaction();

                return true;
            } catch (e) {
                Android.AddLog(e.message);
                return false;
            }
        }

        pData.UpdateNetFundBalance = function (date) {
            var month = $.format.date(new Date(date), 'MM');
            var year = $.format.date(new Date(date), 'yyyy');
            var prevmonth = new Date(new Date().setMonth(new Date().getMonth() - 1));

            pSql = " SELECT IFNULL(SUM(Total), 0) as Total FROM ( \n" +
                    "   SELECT IFNULL(SUM(Total), 0) as Total FROM ( \n" +
                    "       SELECT IFNULL(SUM(Amount), 0) as Total \n" +
                    "       FROM TransReceiveFundsCache \n" +
                    "       WHERE MonthCovered = strftime('%m', 'now') \n" +
                    "       AND YearCovered = strftime('%Y', 'now') " +
                    "       AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    "       UNION ALL SELECT IFNULL(SUM(Amount), 0) as Total \n" +
                    "       FROM TransExpensesReceiveFunds \n" +
                    "       WHERE strftime('%m %Y', ReceiveDate) = strftime('%m %Y', 'now') \n" +
                    "       AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    "       AND IsDelete = 'False' \n " +
                    ") UNION ALL \n " +
                        " SELECT Total FROM ( \n" +
                            "SELECT NetFundBalance as Total \n " +
                            " FROM TransNetFundBalance \n " +
                            " WHERE (MonthCovered = " + $.format.date(prevmonth, 'MM') + " \n " +
                            " AND YearCovered = " + $.format.date(prevmonth, 'yyyy') + ") \n " +
                            " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) +
                            " AND CASE WHEN IFNULL(IsAdjustment, 0) = 1 THEN Status = 'Approved' ELSE IFNULL(IsAdjustment, 0) = 0 END " + " \n" +
                            " AND IFNULL(IsCancelRequest, 0) = 0 \n " +
                            " ORDER BY LastUpdated DESC LIMIT 1 \n" +
                        " ) " +
                    ") \n"

            var pDtfund = Sql.Execute(pSql).Rows            

            var fund = CDec2(pDtfund[0].Total)

            if (fund == 0) {
                return false;
            }

            pSql = "SELECT SUM(TotalAmount) as TotalAmount FROM (SELECT SUM(Amount) as TotalAmount from TransExpensesLogDetails \n" +
                    " WHERE strftime('%m %Y', DateCreated) = strftime('%m %Y', 'now') \n" +
                    " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    " AND IFNULL(IsDelete, 0) = 0 " + " \n" +
                    " UNION ALL \n" +
                    " SELECT SUM(Amount) from TransLogExpensesCache \n" +
                    " WHERE monthcovered = strftime('%m', 'now') \n" +
                    " AND yearcovered = strftime('%Y', 'now') \n" +
                    " AND PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    ") as T "

            var expense = CDec2(Sql.Execute(pSql).Rows[0].TotalAmount)
            var begbal = fund - expense;

            var pIU = new InsertUpdate('TransNetFundBalance');

            pIU.Add("PeopleID", ReadyString(sessionStorage.PeopleID));
            pIU.Add("NetFundBalance", ReadyString(begbal));
            pIU.Add("MonthCovered", ReadyString(month))
            pIU.Add("YearCovered", ReadyString(year));
            pIU.Add("LastUpdated", proj.SqliteNow);
            //pIU.Add("DateImported", "NULL");

            if (CDec2(Sql.ExecuteSql(pIU.GetUpdate("MonthCovered = " + ReadyString(month) + " AND YearCovered = " + ReadyString(year) + " AND PeopleID = " + ReadyString(sessionStorage.PeopleID)))) == 0) {
                pIU.Add("ID", ReadyString(NewID()));
                pSql = pIU.GetInsert();
                Sql.ExecuteSql(pSql);
            }
        }

        pData.GetCustomerOutlet = function (id, name) {
            pSql = "SELECT ID, Name \n"  +
                    " FROM LibraryOrganizationFOWST \n" +
                    " WHERE Name LIKE '%" + name + "%'" +
                    " \n AND OrganizationID = " + ReadyString(id) +
                    " \n ORDER BY Name ASC "

            return Sql.Execute(pSql);
        }

        pData.RecentlyVisitedLocation = function (type) {
            if (type == 'in') {
                pSql = "SELECT DISTINCT IFNULL(CheckInCustomerOrgID, '') as OrgID, IFNULL(CheckInCustomerOrgOutletID , '') as OutletID, \n " +
                        " CASE IFNULL(CheckInCustomerOrgID, '') WHEN '' THEN CheckInLocationName ELSE (SELECT CompanyName FROM LibraryOrganization WHERE ID = CheckInCustomerOrgID) || ' - ' || (SELECT Name FROM LibraryOrganizationFOWST WHERE ID = CheckInCustomerOrgOutletID) END as CompanyName, \n" +
                        " CASE IFNULL(CheckInCustomerOrgID, '') WHEN '' THEN 1 ELSE 0 END as IsUserDefinedLocation \n" +
                        " FROM TransExpensesCheckInOutDetails \n" +
                        " WHERE CheckIn IS NOT NULL \n"

                if (!CBool(Business.SystemDefaults.GetSystemDefault('AllowCustomLocationName'))) {
                    pSql += " AND CheckInCustomerOrgID IS NOT NULL"
                }

                pSql += " ORDER BY CompanyName"
            } else {
                pSql = "SELECT DISTINCT IFNULL(CheckOutCustomerOrgID, '') as OrgID, IFNULL(CheckOutCustomerOrgOutletID , '') as OutletID, \n " +
                        " CASE IFNULL(CheckOutCustomerOrgID, '') WHEN '' THEN CheckOutLocationName ELSE (SELECT CompanyName FROM LibraryOrganization WHERE ID = CheckOutCustomerOrgID) || ' - ' || (SELECT Name FROM LibraryOrganizationFOWST WHERE ID = CheckOutCustomerOrgOutletID) END as CompanyName, \n" +
                        " CASE IFNULL(CheckOutCustomerOrgID, '') WHEN '' THEN 1 ELSE 0 END as IsUserDefinedLocation \n" +
                        " FROM TransExpensesCheckInOutDetails \n" +
                        " WHERE CheckOut IS NOT NULL \n"

                if (!CBool(Business.SystemDefaults.GetSystemDefault('AllowCustomLocationName'))) {
                    pSql += " AND CheckOutCustomerOrgID IS NOT NULL"
                }

                pSql += " ORDER BY CompanyName"
            }            

            return Sql.Execute(pSql);
        }

        pData.EmployeeScheduleForToday = function () {
            //By default, MONDAY is the start of the week

            var pInfo = pData.ScheduleInfo();

            pSql = "SELECT IFNULL(cus.CompanyName, cus.PersonFirstName || ' ' || cus.PersonSurname) || ' - ' || fowst.Name as CompanyName, \n" +
                    " CASE WHEN IFNULL(StartTime, '') = '' \n" +
                    "   THEN '' \n" +
                    "   ELSE \n" +
                    "       CASE WHEN \n" +
                    "           CAST(StrFTime('%H', StartTime) as Integer) % 12 = 0 THEN 12 \n" +
                    "           ELSE CAST(StrFTime('%H', StartTime) as Integer) % 12 END \n" +
                    "       || ':' || StrFTime('%M', StartTime) \n" +
                    "       || ' ' || CASE WHEN CAST(StrFTime('%H', StartTime) as Integer) > 12 THEN 'PM' ELSE 'AM' \n" +
                    "       END \n" +
                    "       || ' - ' || \n" +
                    "       CASE WHEN \n" +
                    "           CAST(StrFTime('%H', EndTime) as Integer) % 12 = 0 THEN 12 \n" +
                    "           ELSE CAST(StrFTime('%H', EndTime) as Integer) % 12 END \n" +
                    "           || ':' || StrFTime('%M', EndTime) \n" +
                    "           || ' ' || CASE WHEN CAST(StrFTime('%H', EndTime) as Integer) > 12 THEN 'PM' ELSE 'AM' \n" +
                    "       END \n" +
                    "   END \n" +
                    " as Time, \n" +
                    " CASE WHEN Day = 0 THEN 'Mon' \n " +
                    " WHEN Day = 1 THEN 'Tue' \n " +
                    " WHEN Day = 2 THEN 'Wed' \n " +
                    " WHEN Day = 3 THEN 'Thu' \n " +
                    " WHEN Day = 4 THEN 'Fri' \n " +
                    " WHEN Day = 5 THEN 'Sat' \n " +
                    " WHEN Day = 6 THEN 'Sun' \n " +
                    " WHEN Day IS NULL THEN 'Any Day' \n " +
                    " END as DayName, route.Week, \n " +
                    " route.LocationID as OrgID, route.FowstID as OutletID " +
                    " FROM LibraryExpensesRouteSchedules as route \n" +
                    " INNER JOIN LibraryOrganization as cus ON cus.ID = route.LocationID \n" +
                    " INNER JOIN LibraryOrganizationFowst as fowst ON fowst.ID = route.FowstID \n" +
                    " WHERE EmployeeID = "+ ReadyString(sessionStorage.PeopleID) + " \n"

            if (pInfo.Frequency == 'monthly' && CBool(pInfo.StrictlyFollowSched)) {
                pSql += " AND (Day IS NULL OR Day = " + (new Date().getDay() - 1) + ") \n"
            } else if (pInfo.Frequency == 'weekly' && CBool(pInfo.StrictlyFollowSched)) {
                pSql += " AND (Day IS NULL OR Day = " + (new Date().getDay() - 1) + ") \n"

                //kwaon ang date sa monday
                var date = new Date();
                var day = date.getDay();
                var diff = date.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday
                var mondayOfTheWeek = new Date(date.setDate(diff))

                date = new Date(mondayOfTheWeek);
                var week = getWeekInMonth(date.getFullYear(), date.getMonth(), date.getDate());
                pSql += " AND route.Week = " + week + " \n"
            }

            pSql += " ORDER BY Week, Day, StartTime ASC"

            pInfo.Schedules = Sql.Execute(pSql);

            return pInfo;
        }

        pData.SaveRouteSchedules = function (e) {
            var pIU = new InsertUpdate('LibraryExpensesRouteSchedules');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                pIU.Add("ParentID", ReadyString(e.ParentID));
                pIU.Add("EmployeeID", ReadyString(e.EmployeeID));
                pIU.Add("LocationID", ReadyString(e.LocationID));
                pIU.Add("FowstID", ReadyString(e.FowstID));                
                pIU.Add("Day", (CStr2(e.Day) == '' ? 'NULL' : ReadyString(e.Day)));
                pIU.Add("StartTime", ReadyString(e.StartTime));
                pIU.Add("EndTime", ReadyString(e.EndTime));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("LastUpdatedBy", (CStr2(e.EncodedBy) == '' ? 'NULL' : ReadyString(e.LastUpdatedBy)));
                pIU.Add("Frequency", ReadyString(e.Frequency || 'monthly'));
                pIU.Add("Week", ReadyString(e.Week));
                pIU.Add("Tag", ReadyString(e.Tag));
                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                Sql.BeginTransaction();

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(id));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                Sql.CommitTransaction();

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Route Schedule: ' + e.message)
                return 'error';
            }
        }

        pData.DeleteRouteSchedules = function (ids) {
            pSql = "DELETE FROM LibraryExpensesRouteSchedules WHERE ID NOT IN (" + ids + ") AND EmployeeID = " + ReadyString(sessionStorage.PeopleID);
            Sql.Execute(pSql);
        }

        pData.SaveRouteSchedulesEmployees = function (e) {
            var pIU = new InsertUpdate('LibraryExpensesRouteSchedulesEmployees');
            var id = CStr2(e.ID) == '' ? NewID() : e.ID;

            try {
                pIU.Add("EmployeeID", ReadyString(e.EmployeeID));
                pIU.Add("RouteName", ReadyString(e.RouteName));
                pIU.Add("RouteDescription", (CStr2(e.RouteDescription) == '' ? 'NULL' : ReadyString(e.RouteDescription)));
                pIU.Add("LastUpdated", (CStr2(e.LastUpdated) == '' ? proj.SqliteNow : ReadyString($.format.date(new Date(e.LastUpdated), 'yyyy-MM-dd HH:mm:ss.000'))));
                pIU.Add("LastUpdatedBy", ReadyString(e.LastUpdatedBy));
                pIU.Add("StrictlyFollowSched", ReadyString(e.StrictlyFollowSched));
                pIU.Add("Frequency", ReadyString(e.Frequency));

                pSql = pIU.GetUpdate('ID = ' + ReadyString(id));

                Sql.BeginTransaction();

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(id));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }

                Sql.CommitTransaction();

                return id;
            } catch (e) {
                Android.AddLog('Error while saving Employee Route Schedule: ' + e.message)
                return 'error';
            }
        }

        pData.ValidateCheckIn = function (data, callback) { //dapat "FIXED" ang Work Schedule Type
            var pInfo = pData.ScheduleInfo();

            function Confirm() {
                NewMessageBox('It seems you didn\'t follow your schedule today. Do you want to continue?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            callback();
                        }
                    }, { Text: 'No' }
                ])
            }

            function Validate() {
                pSql = "SELECT ID FROM TransExpensesCheckInOutDetails WHERE PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                            " AND strftime('%m', CheckIn) = strftime('%m', 'NOW') \n " +
                            " AND strftime('%Y', CheckIn) = strftime('%Y', 'NOW') \n " +
                            " AND strftime('%d', CheckIn) = strftime('%d', 'NOW') \n " +
                            " AND CheckInCustomerOrgID IN (SELECT LocationID FROM LibraryExpensesRouteSchedules WHERE EmployeeID = " + ReadyString(sessionStorage.PeopleID) + " AND (Day IS NULL OR Day = " + (new Date().getDay() - 1) + ") " + OptionForWeeklySched() + ") \n" +
                            " AND CheckInCustomerOrgOutletID IN (SELECT FowstID FROM LibraryExpensesRouteSchedules WHERE EmployeeID = " + ReadyString(sessionStorage.PeopleID) + " AND (Day IS NULL OR Day = " + (new Date().getDay() - 1) + ") " + OptionForWeeklySched() + ") \n";

                if (Sql.Execute(pSql).Rows.length == pData.EmployeeScheduleForToday().Schedules.Rows.length) {
                    //if nabisita na ang tanan schedule today, irelax nalang ang validation
                    callback();
                } else {
                    Confirm();
                }
            }

            function OptionForWeeklySched() {
                if (pInfo.Frequency == 'weekly') {
                    //kwaon ang date sa monday
                    var date = new Date();
                    var day = date.getDay();
                    var diff = date.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday
                    var mondayOfTheWeek = new Date(date.setDate(diff))

                    date = new Date(mondayOfTheWeek);
                    var week = getWeekInMonth(date.getFullYear(), date.getMonth(), date.getDate());
                    return " AND route.Week = " + week + " \n";
                } else {
                    return '';
                }
            }

            if (CStr2(data.CustomerOrgID) == '' && CStr2(data.CustomerOrgOutletID) == '') {
                Validate();
            } else {
                pSql = "SELECT ID FROM LibraryExpensesRouteSchedules WHERE LocationID = " + ReadyString(data.CustomerOrgID) + " \n" +
                    " AND FowstID = " + ReadyString(data.CustomerOrgOutletID) + " \n" +
                    " AND EmployeeID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                    " AND (Day IS NULL OR Day = " + (new Date().getDay() - 1) + ") \n"

                if (pInfo.Frequency == 'weekly' && CBool(pInfo.StrictlyFollowSched)) {
                    pSql += OptionForWeeklySched();
                }

                var pRows = Sql.Execute(pSql).Rows

                if (pRows.length > 0) {
                    callback()
                } else {
                    Validate();
                }
            }            
        }

        pData.CheckLastCheckInOut = function () {
            pSql = "SELECT * FROM TransExpensesCheckInOutDetails WHERE (CheckIn IS NOT NULL AND STRFTIME('%Y-%m-%d', CheckIn) = STRFTIME('%Y-%m-%d', 'NOW')) AND CheckOut IS NULL ORDER BY LastUpdated DESC LIMIT 1";
            return Sql.Execute(pSql).Rows.length > 0
        }

        function getWeekInMonth(year, month, day) {

            var weekNum = 0;
            var weekDay = new Date(year, month, 1).getDay();
            weekDay = weekDay === 0 ? 6 : weekDay - 1;
            var monday = 1 + (7 - weekDay); // get the first monday of the month

            while (monday <= day) {
                weekNum++;
                monday += 7;
            }

            return weekNum;
        }

        return pData;
    })
})