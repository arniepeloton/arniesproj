require(['common', GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='), 'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc='], function () {

        require([(OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : ''), (OfflineMode ? '/Scripts/sources/SelfTest/SelfTest_offline.js?crc=' : ''), (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''), (OfflineMode ? '/Scripts3/Offline/Business/SystemDefaults.js?crc=' : ''), (OfflineMode ? '/Scripts3/Stock_offline.js?crc=' : ''), (OfflineMode ? '/Scripts/sources/SelfTest/SelfTest_offline.js?crc=' : '')], function () {

            $(function () {
                var pricelevelID = '01000012-0005-0000-0000-000000000000'; //retail only
                var subTotal;
                var totalChange;
                var cashAmount;
                var intSalesPayCheckCntr = 0;
                var intSalesPayDebitCardCntr = 0;
                var intSalesPayCreditCardCntr = 0;
                var intSalesPaySRVCntr = 0;
                var customerID;
                var vVerifyOnly = false;

                var isLast = false;
                var isSingleInvoice = false;

                var flushedCounter = 0;
                var isDone = false;

                var testStartDateTime;
                var flushedCounter = 0;
                var totalamt = 0;
                var isPassed = false;

                var lineItems = 35;
                var noOfTransactions = 0;
                var noOfItems = 0;
                var startDateLocal;
                var endDateLocal;
                var startDateServer;
                var locDuration;
                var orgName;
                var orgSKU;
                var mFlushAttempts;
                var mMaxFlushAttempts = 3;

                proj.PosMode = true; //if true, pos

                PageShow();


                function PageShow() {
                    if (CStr2(sessionStorage.IsTestDone) == 'done')
                        location = '/signin?mode=logout';
                    else {

                        if (sessionStorage.OrgID == undefined || sessionStorage.OrgID == '7c794971-1cd2-4ad4-922f-36a93edde09d') {
                            try {

                                var pSql = 'Select OrganizationID from TestData_Results';
                                var pDt = Sql.Execute(pSql);


                                if (pDt.Rows.length > 0) { //add to checking

                                    sessionStorage.ActualOrgID = pDt.Rows[0].OrganizationID; //this is the real org id

                                } else {
                                    location = '/signin?mode=logout';

                                }
                            } catch (ex) {
                                Android.AddLog(ex.message);
                                Android.Toast(ex.message);
                                // location = '/signin?mode=logout';

                            }
                        } else {
                            sessionStorage.ActualOrgID = sessionStorage.OrgID; //this is the real org id

                        }

                        sessionStorage.TestOrgID = '7c794971-1cd2-4ad4-922f-36a93edde09d'; //this is the test org id

                        Web.SetOrgID(sessionStorage.TestOrgID);
                        sessionStorage.OrgID = sessionStorage.TestOrgID;
                        Android.UpdateSessionData({ OrgID: sessionStorage.OrgID })

                        //get org name
                        var pData = SelfTest_offline().getOrgName();
                        orgName = pData.Data.Rows[0].CompanyName;

                        AddToPeopleOrgTest();
                    }
                };

                $('#btnSignInSelfTest').click(function (e) {
                    e.preventDefault();

                    location = '/signin?mode=logout';
                });

                $('#btnSignInNoData').click(function (e) {
                    e.preventDefault();

                    location = '/signin?mode=logout';
                });

                $('#btnRetry').click(function (e) {
                    e.preventDefault();
                    PageShow();
                });

                function AddToPeopleOrgTest() {

                    var pRetryable = Retryable({
                        method: function() {
                            AjaxSend({
                                url: 'SelfTest/AddToPeopleOrgTest',
                                data: { PeopleID: Web.PeopleUserID() },
                                success: function (vData) {
                                    if (vData.Result == 'ok') {
                                        var vDataFowst = SelfTest_offline().getDeviceFOWST();
                                        //                    var fowstID;
                                        // get fowst only that is registered to the device

                                        if (vDataFowst.Data != undefined) {
                                            sessionStorage.DeviceFowstID = vDataFowst.Data.Rows[0].ID;

                                            sessionStorage.FowstID = vDataFowst.Data.Rows[0].FowstID;

                                            Android.UpdateSessionData({
                                                FowstID: sessionStorage.FowstID
                                            });

                                            AddToDeviceFowstTest(sessionStorage.FowstID);
                                        } else
                                            return;
                                    }

                                    if (vData.Error != undefined) {
                                        Android.Toast('Error in AddToPeopleOrgTest: ' + vData.Error);
                                        Android.AddLog('Error in AddToPeopleOrgTest: ' + vData.Error);
                                        //                                location = '/signin?mode=logout';
                                        return;
                                    }

                                },
                                error: function (vError) {
                                    Android.Toast('Error in AddToPeopleOrgTest: ' + vError);
                                    Android.AddLog('Error in AddToPeopleOrgTest: ' + vError);
                                    pRetryable.retry();
                                    //                            location = '/signin?mode=logout';
                                    //callMsg6('Error in AddToPeopleOrgTest: ' + vError);
                                    //return;
                                }
                            });
                        },
                        surrender: function() {
                            location = '/signin?mode=logout'
                        }
                    });
                    
                }

                function AddToDeviceFowstTest(fowst) {

                    var pRetryable = Retryable({
                        method: function() {
                            AjaxSend({
                                url: 'SelfTest/AddToDeviceFowstTest',
                                data: { UUID: Web.DeviceSerial(), DeviceID: Web.DeviceID(), FowstID: fowst, PeopleID: Web.PeopleUserID() },
                                success: function (vData) {
                                    if (vData.Result == 'ok') {

                                        initializeData();

                                    }

                                    if (vData.Error != undefined) {
                                        Android.Toast('Error in AddToDeviceFowstTest: ' + vData.Error);
                                        Android.AddLog('Error in AddToDeviceFowstTest: ' + vData.Error);
                                        //                                location = '/signin?mode=logout';
                                        return;
                                    }

                                },
                                error: function (vError) {
                                    Android.Toast('Error in AddToDeviceFowstTest: ' + vError);
                                    Android.AddLog('Error in AddToDeviceFowstTest: ' + vError);
                                    //                            location = '/signin?mode=logout';
                                    pRetryable.retry();
                                }
                            });
                        }
                    });
                }

                function initializeData() {

                    $('#msg1').show();

                    if (OfflineMode && Sql.IsExist(sessionStorage.OrgID + '.public') == 1) { //
                        NewCart(); // reset order id to make sure it starts new

                        if (SelfTest_offline().createTestTable() == 'true') {
                            //save in test table so that we can compare
                            var pSql = 'INSERT INTO TestData_Results (PeopleID, OrganizationID, CompanyName, DeviceID, TestStatus) VALUES (' +
                                    ReadyString(Web.PeopleUserID()) + ', ' +
                                    ReadyString(sessionStorage.ActualOrgID) + ', ' + ReadyString(orgName) + ', ' +
                                    ReadyString(Web.DeviceID()) + ', ' + ReadyString('on progress') + ')';

                            Sql.ExecuteSql(pSql);

                            setTimeout(callMsg2, 800);
                        } else {
                            Android.AddLog('Failed to create test table');
                        } //end create test table

                    } else {
                        Android.Toast('Invalid Organization');
                        Android.AddLog('Invalid Organization');
                        return;
                    } //end if offline mode

                }

                function callMsg2() {

                    $('#imgLoading1').hide();
                    $('#imgSuccess1').show();

                    $('#msg2').show();


                    setTimeout(callMsg3, 800);


                }

                function callMsg3() {
                    msg = 'Please wait for a few minutes while the system is testing your data. <br/>This is to make sure that your data will work correctly with the POS and<br/> the reports coming from your data will be reliable and accurate.';

                    $('#popTestMessage').html(msg);
                    $("#dvpopupTest").popup("open");

                    $('#imgLoading2').hide();
                    $('#imgSuccess2').show();

                    $('#msg3').show();


                    sessionStorage.TestOrgID = '7c794971-1cd2-4ad4-922f-36a93edde09d'; //this is the test org id

                    Web.SetOrgID(sessionStorage.TestOrgID);
                    sessionStorage.OrgID = sessionStorage.TestOrgID;
                    Android.UpdateSessionData({ OrgID: sessionStorage.OrgID })


                    setTimeout(generateData, 800);


                }


                function callMsg4() {
                    $("#dvpopupTest").popup("close");

                    $('#imgLoading3').hide();
                    $('#imgSuccess3').show();

                    $('#msg4').show();

                    setTimeout(CompareLocal, 800);


                }

                function callMsg5() {
                    startDateServer = new Date();

                    $('#imgLoading4').hide();
                    $('#imgSuccess4').show();

                    $('#msg5').show();

                    callMsg8();


                }


                function callMsg6(remarks) {
                    $('#imgLoading4').hide();
                    $('#imgSuccess4').show();


                    $('#msg6').show();
                    $('#divFailTest').show();

                    $("#dvpopupWait").popup("open");



                    callResult('fail', remarks, 'complete');

                    return;

                }

                function callMsg8() {

                    $('#msg8').show();

                    checkUnflushed();


                }
                function callMsg9() {

                    $('#imgLoading8').hide();
                    $('#imgSuccess8').show();


                    $('#msg9').show();

                    CompareResults();
                }

                function callMsg10() {

                    $('#imgLoading9').hide();
                    $('#imgSuccess9').show();

                    $('#msg10').show();


                    $('#msg11').hide();
                    $('#divFailTest').hide();
                    $('#divFailTestNoData').hide();


                    DeleteTestServerData(sessionStorage.DeviceFowstID);

                }


                function callMsg12() {
                    $("#dvpopupTest").popup("close");

                    $('#imgLoading3').hide();
                    $('#imgFail3').show();



                    $("#dvpopupWait").popup("open");

                    callResult('fail', 'Incomplete data (fowst, product, or price). Unable to start the test.', 'incomplete');

                    return;
                }


                function callResult(result, remarks, status) {
                    SendTestResult(result, remarks, status);



                }


                function generateData() {
                    startDateLocal = new Date();


                    for (var i = 1; i <= lineItems; i++) {

                        if (i == lineItems) {
                            isLast = true;
                        } else
                            isLast = false;

                        if (i == 2) {
                            //get first transaction saved

                            var pSql = 'SELECT LastUpdated from private.TransInvoice ORDER BY LastUpdated desc';
                            testStartDateTime = Sql.Execute(pSql).Rows[0].LastUpdated;
                            if (IsDate(testStartDateTime)) {
                                testStartDateTime = ParseDate(testStartDateTime);
                                testStartDateTime.setMilliseconds(0);
                            }
                        }

                        if (!getRandomData()) {

                            //end the test
                            callMsg12();
                            break;

                        }


                    } //end for


                }

                var countr = 0;
                function CompareLocal() {

                    var totalamtExpected = 0;

                    var totalamtActual = 0;

                    var pSql2 = 'SELECT DISTINCT itm.InvoiceID FROM private.TestData_InvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.InvoiceNumber > 0';
                    var pData2 = Sql.Execute(pSql2);

                    var result3;
                    var pSql3;

                    var pData3;
                    for (var i = 0; i < pData2.Rows.length; i++) {
                        pSql3 = 'SELECT CAST(SUM(Price * Quantity) as TEXT) as totalAmt FROM private.TestData_InvoiceItems WHERE InvoiceID = ' + ReadyString(pData2.Rows[i].InvoiceID);
                        pData3 = Sql.Execute(pSql3);

                        totalamtExpected = parseFloat(totalamtExpected) + parseFloat(pData3.Rows[0].totalAmt);
                    }



                    var pSql4 = 'SELECT itm.ID FROM private.TestData_InvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.InvoiceNumber > 0';
                    var pData4 = Sql.Execute(pSql4);


                    //                    if (window.InAndroid) {
                    var pSql5 = 'SELECT itm.InvoiceID, itm.ProductID, itm.Quantity, Count(itm.ProductID) as Count ' +
                              'FROM private.TestData_InvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >= ' + ReadyDate(testStartDateTime) + '' +
                    ' GROUP BY itm.ProductID, itm.InvoiceID,itm.Quantity ' +
                              'HAVING Count(*) > 1';
                    var pData5 = Sql.Execute(pSql5);


                    // excluding invoice 0
                    var pSql6 = 'SELECT DISTINCT itm.InvoiceID from private.TransInvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >= ' + ReadyDate(testStartDateTime);
                    var pData6 = Sql.Execute(pSql6);

                    var pSql7;
                    var result7;
                    var pData7;

                    for (var i = 0; i < pData6.Rows.length; i++) {
                        pSql7 = 'SELECT CAST(SUM(UnitPrice * Quantity) as TEXT) as totalAmt from private.TransInvoiceItems WHERE InvoiceID = ' + ReadyString(pData6.Rows[i].InvoiceID);
                        pData7 = Sql.Execute(pSql7);

                        totalamtActual = parseFloat(totalamtActual) + parseFloat(pData7.Rows[0].totalAmt);
                    }

                    var pSql11 = 'SELECT itm.id FROM private.TransInvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >= ' + ReadyDate(testStartDateTime);
                    var pData11 = Sql.Execute(pSql11);

                    var pSql13 = 'SELECT itm.InvoiceID, itm.ProductID, itm.Quantity, itm.ActualQuantity, itm.Unitmeasure, Count(itm.ProductID) as Count ' +
                              'FROM private.TransInvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >=' + ReadyDate(testStartDateTime) +
                    ' GROUP BY itm.ProductID, itm.InvoiceID,itm.Quantity, itm.ActualQuantity, itm.UnitMeasure ' +
                              'HAVING Count(*) > 1';
                    var pData13 = Sql.Execute(pSql13);
                    //                    } else {
                    //                        var pSql5 = 'SELECT itm.InvoiceID, itm.ProductID, itm.Quantity, Count(itm.ProductID) as Count ' +
                    //                              'FROM private.TestData_InvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >= CAST (' + ReadyString(testStartDateTime) + ' AS DATETIME)' +
                    //                    ' GROUP BY itm.ProductID, itm.InvoiceID,itm.Quantity ' +
                    //                              'HAVING Count(*) > 1';
                    //                        var pData5 = Sql.Execute(pSql5);


                    //                        // excluding invoice 0
                    //                        var pSql6 = 'SELECT DISTINCT itm.InvoiceID from private.TransInvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >= CAST (' + ReadyString(testStartDateTime) + ' AS DATETIME)';
                    //                        var pData6 = Sql.Execute(pSql6);

                    //                        var pSql7;
                    //                        var result7;
                    //                        var pData7;

                    //                        for (var i = 0; i < pData6.Rows.length; i++) {
                    //                            pSql7 = 'SELECT CAST(SUM(UnitPrice * Quantity) as TEXT) as totalAmt from private.TransInvoiceItems WHERE InvoiceID = ' + ReadyString(pData6.Rows[i].InvoiceID);
                    //                            pData7 = Sql.Execute(pSql7);

                    //                            totalamtActual = parseFloat(totalamtActual) + parseFloat(pData7.Rows[0].totalAmt);
                    //                        }

                    //                        var pSql11 = 'SELECT itm.id FROM private.TransInvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >= CAST (' + ReadyString(testStartDateTime) + ' AS DATETIME)';
                    //                        var pData11 = Sql.Execute(pSql11);

                    //                        var pSql13 = 'SELECT itm.InvoiceID, itm.ProductID, itm.Quantity, itm.ActualQuantity, itm.Unitmeasure, Count(itm.ProductID) as Count ' +
                    //                              'FROM private.TransInvoiceItems itm INNER JOIN private.TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.LastUpdated >= CAST (' + ReadyString(testStartDateTime) + ' AS DATETIME)' +
                    //                    ' GROUP BY itm.ProductID, itm.InvoiceID,itm.Quantity, itm.ActualQuantity, itm.UnitMeasure ' +
                    //                              'HAVING Count(*) > 1';
                    //                        var pData13 = Sql.Execute(pSql13);

                    //                    }




                    ///////////////////////////////////////////////
                    noOfTransactions = pData2.Rows.length;
                    noOfItems = pData4.Rows.length;
                    totalamt = totalamtExpected;

                    //$('#divResults').show();

                    $('#lblResultsTestExpected').text(AccountingFormat(totalamtExpected));
                    $('#lblResultsCountExpected').text(pData2.Rows.length);
                    $('#lblItemCountExpected').text(pData4.Rows.length);
                    $('#lblDuplicateExpected').text((pData5.Rows.length > 0 ? 'Yes' : 'No'));

                    $('#lblResultsActual').text(AccountingFormat(totalamtActual));
                    $('#lblResultsDiscrepancy').text(AccountingFormat(totalamtExpected - totalamtActual));
                    $('#lblResultsCount').text(pData6.Rows.length);
                    $('#lblItemCount').text(pData11.Rows.length);
                    $('#lblDuplicate').text((pData13.Rows.length > 0 ? 'Yes' : 'No'));


                    countr = countr + 1;

                    var date1 = startDateLocal; // 9:00 AM
                    var date2 = new Date(); // 5:00 PM
                    if (date2 < date1) {
                        date2.setDate(date2.getDate() + 1);
                    }
                    var diff = date2 - date1;
                    // 28800000 milliseconds (8 hours)
                    var msec = diff;
                    var hh = Math.floor(msec / 1000 / 60 / 60);
                    msec -= hh * 1000 * 60 * 60;
                    var mm = Math.floor(msec / 1000 / 60);
                    msec -= mm * 1000 * 60;
                    var ss = Math.floor(msec / 1000);
                    msec -= ss * 1000;

                    // diff = 28800000 => hh = 8, mm = 0, ss = 0, msec = 0
                    //Text formatting
                    var hourtext = '00';
                    if (hh > 0) { hourtext = String(hh); }
                    if (hourtext.length == 1) { hourtext = '0' + hourtext };

                    var mintext = '00';
                    if (mm > 0) { mintext = String(mm); }
                    if (mintext.length == 1) { mintext = '0' + mintext };

                    var sectext = '00';
                    if (ss > 0) { sectext = String(ss); }
                    if (sectext.length == 1) { sectext = '0' + sectext };

                    locDuration = hourtext + ':' + mintext + ':' + sectext;


                    //check if equal
                    if (pData2.Rows.length == pData6.Rows.length) {
                        if (pData4.Rows.length == pData11.Rows.length) {
                            // check within tolerance
                            if (totalamtExpected == totalamtActual || ((totalamtExpected - totalamtActual) < 1 && (totalamtExpected - totalamtActual) > -1)) {

                                callMsg5();
                            }
                            else {
                                if (countr < 3) {
                                    CompareLocal();
                                }
                                else {
                                    callMsg6('Local vs Local sum not tally');
                                    return;
                                }
                            }

                        } else {
                            if (countr < 3) {
                                CompareLocal();
                            }
                            else {
                                callMsg6('Local has duplicate items');
                                return;
                            }
                        }


                    } else {
                        if (countr < 3) {
                            CompareLocal();
                        }
                        else {
                            callMsg6('Local vs Local transaction count not equal');
                            return;
                        }
                    }




                }

                var flushCntr = 0;

                function CompareResults() {

                    flushCntr = flushCntr + 1;
                    try {
                        AjaxSend({
                            url: 'SelfTest/GetTransInvoiceTotal',
                            data: { OrgID: Web.OrgID(), DeviceID: Web.DeviceID(), FowstID: sessionStorage.FowstID, TestDate: testStartDateTime },
                            success: function (vData) {
                                var serverTotal = vData.Data;

                                $('#lblResultsServer').text(AccountingFormat(serverTotal));
                                $('#lblResultsDiscrepancyServer').text(AccountingFormat(totalamt - serverTotal));
                                $('#lblResultsCountServer').text(vData.Count);
                                $('#lblItemCountServer').text(vData.ItemCount);
                                $('#lblDuplicateServer').text(vData.Duplicate);
                                //if discrepancy is less than 1 ok lang
                                if (vData.Duplicate = 'No') {
                                    //                                alert(totalamt + ' ' + serverTotal);
                                    ///extend threshold to 5. for very large values/ qty. price may change thru discount bulk
                                    if (vData.ItemCount == noOfItems) {
                                        if (totalamt == serverTotal || ((totalamt - serverTotal) < 1 && (totalamt - serverTotal) > -1)) {
                                            isPassed = true;
                                            callMsg10();
                                            return;
                                        } else {
                                            if (flushCntr < 3) {
                                                mFlushAttempts = 0;
                                                flushData();
                                            } else {

                                                $('#imgLoading9').hide();
                                                $('#imgSuccess9').show();

                                                $('#msg11').show();
                                                $("#dvpopupWait").popup("open");

                                                callResult('fail', 'Local vs Server sum not tally', 'complete');

                                                Android.AddLog('Local vs Server Not Tally');
                                            }



                                        }

                                    } else {
                                        //                                        alert('dli pantay');
                                        if (flushCntr < 3) {
                                            mFlushAttempts = 0;
                                            flushData();
                                        } else {

                                            $('#imgLoading9').hide();
                                            $('#imgSuccess9').show();

                                            $('#msg11').show();

                                            $("#dvpopupWait").popup("open");

                                            var pStrBSPTY3ZQMN = 'Local vs Server invoice item count not equal. '
                                                + 'vData.ItemCount: ' + vData.ItemCount
                                                + '; noOfItems: ' + noOfItems;
                                            callResult('fail', pStrBSPTY3ZQMN, 'complete');
                                            Android.AddLog(pStrBSPTY3ZQMN);

                                        }

                                    }

                                } else {

                                    if (flushCntr < 3) {
                                        mFlushAttempts = 0;
                                        flushData();
                                    } else {

                                        $('#imgLoading9').hide();
                                        $('#imgSuccess9').show();

                                        $('#msg11').show();

                                        $("#dvpopupWait").popup("open");

                                        callResult('fail', 'Server has duplicates', 'complete');
                                        Android.AddLog('Server has duplicates');

                                    }


                                }



                            },
                            error: function (vError) {
                                //                                alert(vError);
                                if (flushCntr < 3) {
                                    mFlushAttempts = 0;
                                    flushData();
                                } else {

                                    $('#imgLoading9').hide();
                                    $('#imgSuccess9').show();

                                    $('#msg11').show();

                                    $("#dvpopupWait").popup("open");


                                    callResult('fail', 'Error in GetTransInvoiceTotal: ' + vError, 'complete');
                                    Android.Toast('Error in GetTransInvoiceTotal: ' + vError);
                                    Android.AddLog('Error in GetTransInvoiceTotal: ' + vError);
                                    return;
                                }




                            }
                        });

                    } catch (ex) {
                        mFlushAttempts = 0;
                        flushData();

                        Android.Toast(ex.Message);
                        Android.AddLog(ex.Message);

                    }

                }



                var countCheck = 0;

                function checkUnflushed() {

                    var pSql = '';
                    var pSql2 = '';

                    Android.UpdateSessionData({ OrgID: sessionStorage.OrgID });


                    pSql = 'Select * from TransInvoice where DateImported IS NULL AND InvoiceNumber>0';
                    pDt = Sql.Execute(pSql);

                    //additional by janine. check if there are unflushed customer. april 21 2014
                    pSql2 = 'Select * from LibraryOrganization where IsFromDevice = 1 AND FlushToServer = 1';
                    pDt2 = Sql.Execute(pSql2);
                    //end of additional

                    if (countCheck < 6) {
                        if (pDt.Rows.length > 0 || pDt2.Rows.length > 0) { //add to checking
                            if (countCheck == 5)
                                setTimeout(function() {
                                    mFlushAttempts = 0;
                                    flushData()
                                }, 800);
                            else
                                setTimeout(checkUnflushed, 10000);

                        } else {
                            setTimeout(callMsg9, 800);
                        }
                    }

                    countCheck = countCheck + 1;
                }

                function flushData() {
                    var pSql = '';
                    var pSql2 = '';
                    var pStr;

                    // before calling this function from outside, set mFlushAttempts = 0;
                    mFlushAttempts = CDec2(mFlushAttempts);
                    if (mMaxFlushAttempts <= mFlushAttempts) {
                        pStr = 'Oops! We cannot initialize your local database due to a connection issue with the Mobiz Cloud.' + '\n\n' + 
                            'Please make sure that you have a live and stable Internet connection on your device and try signing in again at a later time. ' + '\n\n' + 
                            'If the problem persists, please email Mobiz at customercare@mobiz-apps.com.  Thank you.';
                        pStr = HtmlEncode(pStr);
                        pStr = pStr.replace(/\n/g, '<br />');
                        MessageBox(pStr,
                            null, [
                                {
                                    Text: 'Retry Data Test',
                                    Click: function() {
                                        history.back();
                                        mFlushAttempts = 0;
                                        delete sessionStorage.IsTestDone;
                                        flushData();
                                    }
                                },
                                {
                                    Text: 'Try again later',
                                    Click: function() {
                                        location = '/signin?mode=logout';
                                    }
                                }
                            ],
                            {
                                EncodeHtml: false
                            }
                        );
                        return;
                    }
                    mFlushAttempts++;

                    Android.UpdateSessionData({ OrgID: sessionStorage.OrgID });

                    pSql = 'Select * from TransInvoice where DateImported IS NULL AND InvoiceNumber>0';
                    pDt = Sql.Execute(pSql);

                    //additional by janine. check if there are unflushed customer. april 21 2014
                    pSql2 = 'Select * from LibraryOrganization where IsFromDevice = 1 AND FlushToServer = 1';
                    pDt2 = Sql.Execute(pSql2);
                    //end of additional
                    //                    alert(pDt.Rows.length > 0 || pDt2.Rows.length > 0);
                    if (pDt.Rows.length > 0 || pDt2.Rows.length > 0) { //add to checking
                        SalesPaymentsExport_offline().Export({
                            OnUploaded: function () {
                                //                                    FollowUp();
                            },
                            OnDone: function (vData) {
                                if (vData.Result == 'ok')
                                    setTimeout(flushData, 10000);
                                else if (vData.Result == 'error') {
                                    setTimeout(flushData, 10000);
                                    Android.Toast(vData.Error);
                                    return;
                                }
                            }
                        });

                    } else {

                        setTimeout(callMsg9, 800);
                    }




                } //end function

                function Retryable(vParams) {
                    // vParams
                    //  method: function to retry
                    //  surrender: function to call if user clicks "Try again later"
                    //  maxAttempts = 3
                    //  delay: 10000

                    vParams = $.extend({
                            maxAttempts: 3,
                            delay: 10000,
                            surrender: function() {
                                location = '/signin?mode=logout';
                            }
                        }, vParams);

                    var mOut = {
                        attempts: 0,
                        retry: function() {
                            mOut.attempts++;
                            if (vParams.maxAttempts < mOut.attempts) {
                                pStr = 'Oops! We cannot initialize your local database due to a connection issue with the Mobiz Cloud.' + '\n\n' + 
                                    'Please make sure that you have a live and stable Internet connection on your device and try signing in again at a later time. ' + '\n\n' + 
                                    'If the problem persists, please email Mobiz at customercare@mobiz-apps.com.  Thank you.';
                                pStr = HtmlEncode(pStr);
                                pStr = pStr.replace(/\n/g, '<br />');
                                MessageBox(pStr,
                                    null, [
                                        {
                                            Text: 'Retry Data Test',
                                            Click: function() {
                                                history.back();
                                                mOut.attempts = 0;
                                                mOut.retry();
                                            }
                                        },
                                        {
                                            Text: 'Try again later',
                                            Click: function() {
                                                vParams.surrender();
                                            }
                                        }
                                    ],
                                    {
                                        EncodeHtml: false
                                    }
                                );
                                return;
                            }

                            if (mOut.attempts == 1)
                                vParams.method();
                            else
                                setTimeout(vParams.method, vParams.delay);
                        } // END retry: function() {
                    } // END var mOut = {

                    mOut.retry();

                    return mOut;
                } // END Retryable

                var deleteCnt = 0;
                function DeleteTestServerData(lastInvoice, devFowstID) {
                    deleteCnt = deleteCnt + 1;

                    AjaxSend({
                        url: 'SelfTest/UpdateDeleteTestInvoice',
                        data: { DeviceFowstID: devFowstID, DeviceID: Web.DeviceID(), TestDate: testStartDateTime },
                        success: function (vData) {
                            $("#dvpopupWait").popup("open");

                            callResult('pass', 'none', 'complete');
                        },
                        error: function (vError) {
                            if (deleteCnt < 3) {
                                DeleteTestServerData(lastInvoice, devFowstID);
                            } else {
                                Android.Toast(vError);
                                Android.AddLog(vError);
                                $("#dvpopupWait").popup("open");

                                callResult('pass', 'Failed to delete invoices in server', 'complete');
                            }

                        }
                    });

                }


                function SendTestResult(result, remarks, status) {
                    //save in test table so that we can compare
                    var pSql = 'UPDATE TestData_Results SET Result=' + ReadyString(result) + ', TestStatus = ' +
                            ReadyString('done') + ', DataSet=' + ReadyString(status) + ' WHERE PeopleID = ' + ReadyString(Web.PeopleUserID()) +
                            ' AND OrganizationID = ' + ReadyString(sessionStorage.ActualOrgID);

                    Sql.ExecuteSql(pSql);



                    var pData = {
                        ActualOrgID: sessionStorage.ActualOrgID,
                        DeviceID: Web.DeviceID(),
                        DeviceModel: Device.Name(),
                        DevicePlatform: Device.Platform(),
                        PeopleUserID: Web.PeopleUserID(),
                        Result: result, Remarks: remarks,
                        LocalDuration: locDuration,
                        StartDateServer: startDateServer,
                        TransCount: noOfTransactions,
                        LineCount: noOfItems,
                        OrgName: orgName,
                        OrgSKU: orgSKU
                    };

                    var pRetryable = Retryable({
                        method: function() {
                            AjaxSend({
                                url: 'SelfTest/SendTestResult',
                                data: pData,
                                success: function (vData) {
                                    //                            Android.AddLog('Test results has been sent to server.');
                                    sessionStorage.IsTestDone = 'done';

                                    if (result == 'pass') {

                                        $('#dvpopupWait').popup('close');

                                        $('#divSuccessTest').show();
                                        $('#msg11').hide();
                                        $('#divFailTest').hide();
                                        $('#divFailTestNoData').hide();

                                        Sql.CloseConnection();
                                        //delete the test org if result is ok so that this will not be accidentally used.
                                        Sql.DeleteIfExist(sessionStorage.TestOrgID);
                                        Sql.DeleteIfExist(sessionStorage.TestOrgID + '.private');
                                        Sql.DeleteIfExist(sessionStorage.TestOrgID + '.public');

										//Get data for inventory count
										Android.OfflineBalances();
                                    }
                                    else {

                                        $('#dvpopupWait').popup('close');

                                        if (status == 'complete')
                                            $('#divFailTest').show();
                                        else
                                            $('#divFailTestNoData').show();

                                    }


                                    //remove sessionstorage used
                                    sessionStorage.removeItem('OrgID');
                                    sessionStorage.removeItem('ActualOrgID');
                                    sessionStorage.removeItem('TestOrgID');
                                    sessionStorage.removeItem('DeviceFowstID');


                                },
                                error: function (vError) {
                                    Android.Toast('Failed to send report.');
                                    Android.AddLog(vError);
                                    pRetryable.retry();
                                } // END error
                            }); // END ajaxsend
                        }, // END method: function() {
                        surrender: function() {
                            sessionStorage.IsTestDone = 'done';

                            if (result == 'pass') {
                                $("#dvpopupWait").popup("close");

                                $('#divSuccessTest').show();
                                $('#msg11').hide();
                                $('#divFailTest').hide();
                                $('#divFailTestNoData').hide();

                                Sql.CloseConnection();
                                //delete the test org if result is ok so that this will not be accidentally used.
                                //                                Sql.DeleteIfExist(sessionStorage.TestOrgID);
								Android.OfflineBalances();
                            }
                            else {
                                $("#dvpopupWait").popup("close");

                                if (status == 'complete')
                                    $('#divFailTest').show();
                                else
                                    $('#divFailTestNoData').show();

                            }

                            //remove sessionstorage used
                            sessionStorage.removeItem('OrgID');
                            sessionStorage.removeItem('ActualOrgID');
                            sessionStorage.removeItem('TestOrgID');
                            sessionStorage.removeItem('DeviceFowstID');

                            location = '/signin?mode=logout';
                        } // END surrender: function() {
                    }); // END var pRetryable = Retryable({

                } // END function SendTestResult(result, remarks, status) {



                function getRandomData() {

                    var randNo;

                    //                    var vDataCustomer = SelfTest_offline().getOrganizationCustomer();

                    //                    if (vDataCustomer.Data.Rows.length > 0) {
                    //                        if (vDataCustomer.Data.Rows.length == 1) {
                    //                            customerID = vDataCustomer.Data.Rows[0].ID;
                    //                        } else {
                    //                            if (RandomInteger(0, 1) == 0) {

                    // cash payment
                    customerID = 'cd71a0f7-08a9-4601-876e-5f13c064f873'; // cash payment
                    //                            } else {

                    //                                randNo = RandomInteger(0, vDataCustomer.Data.Rows.length - 1);
                    //                                customerID = vDataCustomer.Data.Rows[randNo].ID;
                    //                            }
                    //                        }
                    //                    } else {
                    //                        Android.AddLog('No customer orgnization.');
                    //                        return false;
                    //                    }



                    var vDataPIM = SelfTest_offline().getOrganizationPIM();
                    var productID;
                    var productUnitQty;
                    var productUnitMeasure;
                    var price;
                    var pDrPrice = {};

                    orgSKU = vDataPIM.Data.Rows.length;
                    var pSql = 'UPDATE TestData_Results SET NoOfOrgSKU =' + ReadyString(orgSKU) + ' WHERE PeopleID = ' + ReadyString(Web.PeopleUserID()) +
                            ' AND OrganizationID = ' + ReadyString(sessionStorage.ActualOrgID);

                    Sql.ExecuteSql(pSql);


                    if (vDataPIM.Data.Rows.length > 0) {
                        if (vDataPIM.Data.Rows.length == 1) {
                            productID = vDataPIM.Data.Rows[0].ProductID;
                            productUnitQty = vDataPIM.Data.Rows[0].Quantity;
                            productUnitMeasure = vDataPIM.Data.Rows[0].UnitMeasure;

                        } else {
                            randNo = RandomInteger(0, vDataPIM.Data.Rows.length - 1);
                            productID = vDataPIM.Data.Rows[randNo].ProductID;
                            productUnitQty = vDataPIM.Data.Rows[randNo].Quantity;
                            productUnitMeasure = vDataPIM.Data.Rows[randNo].UnitMeasure;

                        }


                        if (sessionStorage.CartOrderID != undefined && productID != undefined) {
                            // check if product already exists in invoice. if yes, delete it. then add this new product so there could only be 1 product of same kind in every invoice

                            var pUrl;
                            var pData = {
                                OrderID: sessionStorage.CartOrderID,
                                ProductID: productID,
                                UnitMeasure: productUnitMeasure
                            };

                            //delete in test as well

                            SelfTest_offline().DeleteOrderStockTest(sessionStorage.CartOrderID, productID, productUnitMeasure);
                            var vData = SalesMenu_offline().DeleteStockPos(pData);
                            //                            if (vData.Result == 'ok')
                            //                                alert('naay duplicate deleted');
                        }


                    } else {
                        Android.AddLog('No org PIM found');
                        return false;
                    }

                    //                    Web.SetOrgID(sessionStorage.ActualOrgID);

                    var pDtStocks = Business.LibraryPIMProduct.PriceTableForTEST(productID, customerID);
                    //                    Web.SetOrgID(sessionStorage.TestOrgID);

                    if (pDtStocks != undefined) {
                        if (pDtStocks.Rows.length > 0) {
                            for (var pPriceIndex = 0; pPriceIndex <= pDtStocks.Rows.length - 1; pPriceIndex++) {
                                if (pDtStocks.Rows[pPriceIndex].Quantity == productUnitQty
                                && pDtStocks.Rows[pPriceIndex].PriceLevelID == pricelevelID) {

                                    pDrPrice = pDtStocks.Rows[pPriceIndex];

                                    break;
                                };
                            };

                            UpdatePos(productID, RandomInteger(1, 24), productUnitMeasure, productUnitQty, sessionStorage.FowstID, pDrPrice.DiscountedPrice); //2nd param is quantity
                        }
                        else {
                            Android.AddLog('No product price ' + productID);
                            return false;
                        }

                    }
                    else {
                        Android.AddLog('No product price. Undefined' + productID);
                        return false;
                    }



                    return true;

                } //get random data

                function NewCart() {

                    localStorage.NewCartMode = 1;
                    sessionStorage.removeItem('CartOrderID');
                }

                function UpdatePos(skuID, qty, unitMeasure, unitQty, fowstID, price) {
                    //                    alert(skuID + ' ' + qty + ' ' + unitQty + ' ' + fowstID + ' ' + price);

                    if (parseFloat(qty) >= 0) {
                        var pData = {};
                        pData.StockID = skuID;
                        pData.CustomerID = customerID;
                        pData.NewCartMode = localStorage.NewCartMode;
                        pData.FowstID = fowstID;
                        pData.ProductID = skuID;
                        pData.Stocks = [];
                        pData.SalesRep = [];
                        pData.ProductSerial = [];
                        pData.Remarks = 'TEST ONLY';
                        pData.IsTestOnly = 1; //this means, it's a test! additional june 2 by janine
                        //additional customer demographic oct 23
                        pData.DeliveryMethod = 'PickUp';
                        pData.OrderMethod = 'Walk In';
                        pData.Gender = 'NA';
                        pData.Age = 'Mixed Ages/ NA';
                        //additonal for sm coin VAT EXEMPT
                        pData.IsVATExempt = 0;
                        //end of additional

                        if (sessionStorage.CartOrderID != undefined) {
                            pData.OrderID = sessionStorage.CartOrderID;
                        }

                        pData.Stocks[0] = {
                            Qty: parseFloat(qty),
                            UnitQty: parseFloat(unitQty),
                            UnitPrice: parseFloat(price),
                            PriceLevelID: pricelevelID
                        };

                        if (OfflineMode) {

                            LoadData(
                                SelfTest_offline().UpdatePos(pData), skuID, parseFloat(qty), unitMeasure, parseFloat(unitQty), parseFloat(price)
                            );
                        }

                    } else {
                        // if invalid qty
                        MessageBox('Invalid Quantity');
                        return;
                    }
                }


                function LoadData(vData, skuID, qty, unitMeasure, unitQty, price) {
                    $('#divMsg2').fadeOut();
                    $('#divMsg3').fadeIn();

                    var pMessage;
                    var pDivDetails;
                    var pLnkDetails;

                    if (vData == undefined) return;
                    if (vData.Error != undefined) {
                        if (CDec2(vData.ReloadBrowser) != 0)
                            MessageBox(vData.Error, 'Error', null, {
                                OnClose: function () {
                                    location.reload();
                                }
                            });
                        else
                            MessageBox(vData.Error);
                        return;
                    };
                    if (vData.Result != 'ok') {
                        pMessage = $('<div>').html('The price you have requested is too low. Please do one of the following:<br />'
                                + '<ol>'
                                + '<li>Take the original selling price</li>'
                                + '<li>Increase the requested price</li>'
                                + '<li>Try a price request again after adding some more items to your cart.</li>'
                                + '</ol>'
                                + '• Price requests are approved/ disapproved based on transaction profitability. The more items you put into the cart, the more likely that your price request will be approved.<br />'
                            );
                        pLnkDetails = $('<a>')
                        .attr('href', '#')
                        .text('Details')
                        .click(function (e) {
                            e.preventDefault();
                            pLnkDetails.hide();
                            pDivDetails.show();
                        });
                        pMessage.append(pLnkDetails);
                        pDivDetails = $('<div>').html('<h3>Details</h3>'
                        + '<h5>Decision Factors for Denying Price Request:</h5>'
                        + '<ol>'
                        + '<li><b>Is this line item’s Gross Profit Margin (GPM) within acceptable limits?: No'
                        + '</li>'
                        + '</ol>' + HtmlEncode(vData.Warning)
                    ).hide();
                        pMessage.append(pDivDetails);
                    }; // if (vData.Result != 'ok') {
                    if (vVerifyOnly) {
                        if (vData.Result == 'ok') {
                            MessageBox('Price adjustment request is acceptable.');
                        } else {
                            MessageBox(pMessage, 'Price Request Denied', null, {
                                EncodeHtml: false
                            });
                        };
                    } else {
                        if ((vData.Result == 'ok')
                        || (vData.Warning != undefined)) {

                            function Done() {
                                var pDT;

                                //save in test table so that we can compare
                                pSql = 'SELECT ID FROM private.TestData_InvoiceItems WHERE InvoiceID =' + ReadyString(vData.OrderID) + ' AND ProductID =' + ReadyString(skuID) + ' AND UnitQty =' + CDec2(unitQty);
                                pDT = Sql.Execute(pSql);

                                //if theres an item with same unit quantity, replace it
                                if (pDT.Rows.length > 0) {
                                    pSql = 'UPDATE private.TestData_InvoiceItems SET Quantity = ' + CDec2(qty) + ', UnitMeasure = ' + ReadyString(unitMeasure)  +' , PriceLevelID = ' + ReadyString(pricelevelID) + ', Price =' + CDec2(price) + ' WHERE ID = ' + ReadyString(pDT.Rows[0].ID);
                                    Sql.ExecuteSql(pSql);
                                } else {

                                    pSql = 'INSERT INTO private.TestData_InvoiceItems VALUES (' + ReadyString(NewID()) + ', ' + ReadyString(vData.OrderID) + ', ' + ReadyString(skuID) + ', ' + CDec2(qty) + ', ' + ReadyString(unitMeasure) + ' , ' + CDec2(unitQty) + ' , ' + ReadyString(pricelevelID) + ', ' + CDec2(price) + ')';
                                    Sql.ExecuteSql(pSql);

                                }

                                //set new cart mode. based on random. returns 1 or 0 randomly


                                //if 1, then proceed to payments
                                if (isSingleInvoice) {
                                    if (isLast)
                                        localStorage.NewCartMode = 1;
                                    else
                                        localStorage.NewCartMode = 0;
                                } else {
                                    localStorage.NewCartMode = RandomInteger(0, 1);
                                }

                                if (localStorage.NewCartMode == 1) {

                                    getSalesPaymentsInfo(vData.OrderID);

                                } else {

                                    sessionStorage.CartOrderID = vData.OrderID;

                                    if (isLast)
                                        callMsg4();

                                }

                            }; // function Done() {

                            if (vData.Warning != undefined)
                                MessageBox(pMessage, 'Warning', null, {
                                    OnClose: Done,
                                    EncodeHtml: false
                                });
                            else
                                Done();
                        } else {
                            MessageBox((vData.Error || 'An error occured while updating cart'), 'Failed',
                                [
                                    {
                                        Text: 'Retry',
                                        Click: function () {
                                            if (Verify())
                                                Submit();
                                        }
                                    },
                                    {
                                        Text: 'Close this message'
                                    }
                                ]
                            ); // MessageBox((vData.Error || 'An error
                        }; // if (vData.Result == 'ok') {
                    }; // if (vVerifyOnly) {
                }; // LoadData



                function getSalesPaymentsInfo(invID) {
                    var pData = {};


                    pData.InvoiceID = invID;

                    if (OfflineMode) {

                        subTotal = AccountingFormat(SalesPayment_offline().ShowSalesPaymentsInfo(invID).Rows[0].TotalAmountDue);

                        RefreshSalesPaymentTotal(invID);

                    }
                };

                function RefreshSalesPaymentTotal(invID) {

                    var dblTotalSalesPay = 0;
                    var dblTotalAmountDue = 0;


                    dblTotalSalesPay = parseFloat(subTotal) - parseFloat(subTotal);

                    if (dblTotalSalesPay >= 0) {

                        totalChange = AccountingFormat(dblTotalSalesPay);
                    } else {

                        totalChange = "No Change";
                    }

                    var pData = [];

                    pData = {
                        ID: invID,
                        CustomerID: customerID,
                        CusRepresentativeID: "",
                        BillToID: Business.SystemDefaults.POSBillingAddressID(),
                        ShipToID: Business.SystemDefaults.POSDeliveryAddressID(),
                        EmailAddress: '',
                        PickUpLocationID: sessionStorage.FowstID,
                        DeliveryMethod: "Pickup",
                        IsDownTown: "0",
                        IsRushDelivery: "0",
                        DeliverWithin: 0,
                        DeliveryType: "",
                        PickupDate: ""



                    };


                    if (SalesPayment_offline().SaveDeliveryInfo(pData)) {
                        SaveSalesPayment(invID, false);

                    }



                };



                function SaveSalesPayment(pInvoiceID, blnIsCheckChangeTY) {

                    var pData = [];
                    var pDataDiscount = [];
                    var pDataSeniorCitizenPWD = [];
                    var pDataEmail = [];
                    var pDataCheck = [];
                    var pDataDebitCard = [];
                    var pDataCreditCard = [];
                    var pDataSRV = [];
                    var pDataOtherType = [];
                    var intIndex;

                    var debitCardAmount = '0';
                    var creditCardAmount = '0';
                    var checkPaymentAmount = '0';
                    var SRVAmount = '0';
                    var otherTypeAmount = '0';

                    //                    //Discount
                    //                    if ($("#selDiscountType").val() != "") {
                    //                        intIndex = 0;
                    //                        pDataDiscount[intIndex] = {
                    //                            ID: $("#selDiscountType").val(),
                    //                            DiscountPercent: $("#txtDiscountPercent").val(),
                    //                            DiscountRemarks: $("#txtDiscountRemarks").val()
                    //                        };
                    //                    }

                    //                    //Check
                    //                    intIndex = 0;
                    //                    for (var i = 0; i <= intSalesPayCheckCntr; i++) {

                    //                        if (sessionStorage["SalesPayCheck-" + i] != undefined) {

                    //                            arSalesPayCheck = new Array();
                    //                            arSalesPayCheck = JSON.parse(sessionStorage["SalesPayCheck-" + i]);

                    //                            var dtCheckDate = "";
                    //                            if (arSalesPayCheck[intSalesPayCheckDateMonth] != "" &&
                    //                            arSalesPayCheck[intSalesPayCheckDateDay] != "" &&
                    //                            arSalesPayCheck[intSalesPayCheckDateyear] != "") {

                    //                                dtCheckDate = arSalesPayCheck[intSalesPayCheckDateMonth] + "/" + arSalesPayCheck[intSalesPayCheckDateDay] + "/" + arSalesPayCheck[intSalesPayCheckDateyear];
                    //                            }

                    //                            pDataCheck[intIndex] = {
                    //                                ID: arSalesPayCheck[intSalesPayCheckRecID],
                    //                                BankID: arSalesPayCheck[intSalesPayCheckBankID],
                    //                                BankName: arSalesPayCheck[intSalesPayCheckBank],
                    //                                BankBranchID: arSalesPayCheck[intSalesPayCheckBankBranchID],
                    //                                BankBranchName: arSalesPayCheck[intSalesPayCheckBankBranch],
                    //                                AccountName: arSalesPayCheck[intSalesPayCheckAccountName],
                    //                                AccountNumber: arSalesPayCheck[intSalesPayCheckAccountNo],
                    //                                CheckNumber: arSalesPayCheck[intSalesPayCheckNumber],
                    //                                CheckDate: dtCheckDate,
                    //                                CheckAmount: arSalesPayCheck[intSalesPayCheckAmount],
                    //                                RecState: arSalesPayCheck[intSalesPayCheckState]
                    //                            };

                    //                            intIndex = intIndex + 1;

                    //                        }
                    //                    }

                    //                    //Debit Card
                    //                    intIndex = 0;
                    //                    for (var i = 0; i <= intSalesPayDebitCardCntr; i++) {

                    //                        if (sessionStorage["SalesPayDebitCard-" + i] != undefined) {

                    //                            arSalesPayDebitCard = new Array();
                    //                            arSalesPayDebitCard = JSON.parse(sessionStorage["SalesPayDebitCard-" + i]);

                    //                            var dtDebitCardIssuanceDate = "";
                    //                            if (arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateMonth] != "" &&
                    //                            arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateDay] != "" &&
                    //                            arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateYear] != "") {

                    //                                dtDebitCardIssuanceDate = arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateMonth] + "/" + arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateDay] + "/" + arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateYear];
                    //                            }

                    //                            var dtDebitCardExpirationDate = "";
                    //                            if (arSalesPayDebitCard[intSalesPayDebitCardExpirationDateMonth] != "" &&
                    //                            arSalesPayDebitCard[intSalesPayDebitCardExpirationDateDay] != "" &&
                    //                            arSalesPayDebitCard[intSalesPayDebitCardExpirationDateYear] != "") {

                    //                                dtDebitCardExpirationDate = arSalesPayDebitCard[intSalesPayDebitCardExpirationDateMonth] + "/" + arSalesPayDebitCard[intSalesPayDebitCardExpirationDateDay] + "/" + arSalesPayDebitCard[intSalesPayDebitCardExpirationDateYear];
                    //                            }

                    //                            pDataDebitCard[intIndex] = {
                    //                                ID: arSalesPayDebitCard[intSalesPayDebitCardRecID],
                    //                                DebitCardType: arSalesPayDebitCard[intSalesPayDebitCardType],
                    //                                DebitCardBankID: arSalesPayDebitCard[intSalesPayDebitCardBankID],
                    //                                DebitCardBank: arSalesPayDebitCard[intSalesPayDebitCardBank],
                    //                                HolderLastName: arSalesPayDebitCard[intSalesPayDebitCardHolderLName],
                    //                                HolderFirstName: arSalesPayDebitCard[intSalesPayDebitCardHolderFName],
                    //                                HolderMiddleName: arSalesPayDebitCard[intSalesPayDebitCardHolderMName],
                    //                                HolderGenerationalSuffix: arSalesPayDebitCard[intSalesPayDebitCardHolderGenSuffix],
                    //                                BankCountryID: arSalesPayDebitCard[intSalesPayDebitCardBankCountryID],
                    //                                DebitCardNumber: arSalesPayDebitCard[intSalesPayDebitCardNumber],
                    //                                DebitCardIssuanceDate: dtDebitCardIssuanceDate,
                    //                                DebitCardExpirationDate: dtDebitCardExpirationDate,
                    //                                ApprovalNo: arSalesPayDebitCard[intSalesPayDebitCardApprovalNumber],
                    //                                NoOfInstallmentPayments: arSalesPayDebitCard[intSalesPayDebitCardNoOfInstallment],
                    //                                DebitCardAmount: arSalesPayDebitCard[intSalesPayDebitCardAmount],
                    //                                RecState: arSalesPayDebitCard[intSalesPayDebitCardState]
                    //                            };

                    //                            intIndex = intIndex + 1;

                    //                        }
                    //                    }

                    //                    //Credit Card
                    //                    intIndex = 0;
                    //                    for (var i = 0; i <= intSalesPayCreditCardCntr; i++) {

                    //                        if (sessionStorage["SalesPayCreditCard-" + i] != undefined) {

                    //                            arSalesPayCreditCard = new Array();
                    //                            arSalesPayCreditCard = JSON.parse(sessionStorage["SalesPayCreditCard-" + i]);

                    //                            var dtCreditCardIssuanceDate = "";
                    //                            if (arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateMonth] != "" &&
                    //                            arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateDay] != "" &&
                    //                            arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateYear] != "") {

                    //                                dtCreditCardIssuanceDate = arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateMonth] + "/" + arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateDay] + "/" + arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateYear];
                    //                            }

                    //                            var dtCreditCardExpirationDate = "";
                    //                            if (arSalesPayCreditCard[intSalesPayCreditCardExpirationDateMonth] != "" &&
                    //                            arSalesPayCreditCard[intSalesPayCreditCardExpirationDateDay] != "" &&
                    //                            arSalesPayCreditCard[intSalesPayCreditCardExpirationDateYear] != "") {

                    //                                dtCreditCardExpirationDate = arSalesPayCreditCard[intSalesPayCreditCardExpirationDateMonth] + "/" + arSalesPayCreditCard[intSalesPayCreditCardExpirationDateDay] + "/" + arSalesPayCreditCard[intSalesPayCreditCardExpirationDateYear];
                    //                            }

                    //                            pDataCreditCard[intIndex] = {
                    //                                ID: arSalesPayCreditCard[intSalesPayCreditCardRecID],
                    //                                CreditCardType: arSalesPayCreditCard[intSalesPayCreditCardType],
                    //                                CreditCardBankID: arSalesPayCreditCard[intSalesPayCreditCardBankID],
                    //                                CreditCardBank: arSalesPayCreditCard[intSalesPayCreditCardBank],
                    //                                HolderLastName: arSalesPayCreditCard[intSalesPayCreditCardHolderLName],
                    //                                HolderFirstName: arSalesPayCreditCard[intSalesPayCreditCardHolderFName],
                    //                                HolderMiddleName: arSalesPayCreditCard[intSalesPayCreditCardHolderMName],
                    //                                HolderGenerationalSuffix: arSalesPayCreditCard[intSalesPayCreditCardHolderGenSuffix],
                    //                                BankCountryID: arSalesPayCreditCard[intSalesPayCreditCardBankCountryID],
                    //                                CreditCardNumber: arSalesPayCreditCard[intSalesPayCreditCardNumber],
                    //                                CreditCardIssuanceDate: dtCreditCardIssuanceDate,
                    //                                CreditCardExpirationDate: dtCreditCardExpirationDate,
                    //                                ApprovalNo: arSalesPayCreditCard[intSalesPayCreditCardApprovalNumber],
                    //                                NoOfInstallmentPayments: arSalesPayCreditCard[intSalesPayCreditCardNoOfInstallment],
                    //                                CreditCardAmount: arSalesPayCreditCard[intSalesPayCreditCardAmount],
                    //                                RecState: arSalesPayCreditCard[intSalesPayCreditCardState]
                    //                            };

                    //                            intIndex = intIndex + 1;

                    //                        }
                    //                    }

                    //                    //SRV
                    //                    intIndex = 0;
                    //                    for (var i = 0; i <= intSalesPaySRVCntr; i++) {

                    //                        if (sessionStorage["SalesPaySRV-" + i] != undefined) {

                    //                            arSalesPaySRV = new Array();
                    //                            arSalesPaySRV = JSON.parse(sessionStorage["SalesPaySRV-" + i]);

                    //                            pDataSRV[intIndex] = {
                    //                                ID: arSalesPaySRV[intSalesPaySRVRecID],
                    //                                MemoID: arSalesPaySRV[intSalesPaySRVMemoID],
                    //                                ConsumedAmount: arSalesPaySRV[intSalesPaySRVConsumedAmount],
                    //                                RecState: arSalesPaySRV[intSalesPaySRVState],
                    //                                SrvNum: arSalesPaySRV[intSalesPaySRVNo],
                    //                                Pin: arSalesPaySRV[intSalesPaySRVPinCode]
                    //                            };

                    //                            intIndex = intIndex + 1;

                    //                        }
                    //                    }

                    pData = {
                        EmailAddress: "",
                        CustomerID: customerID,
                        InvoiceID: pInvoiceID,
                        IsForGovReport: "0",
                        CashAmount: subTotal,
                        DebitCardAmount: debitCardAmount,
                        CreditCardAmount: creditCardAmount,
                        CheckPaymentAmount: checkPaymentAmount,
                        SRVAmount: SRVAmount,
                        OtherTypeAmount: otherTypeAmount,
                        TotalAmountDue: subTotal,
                        TotalDiscountAmount: "0",
                        TotalAmountTendered: subTotal,
                        CustomerChangeCash: (totalChange == "No Change" ? "0.00" : (blnIsCheckChangeTY ? "0.00" : totalChange)),
                        NoOfPeople: '',
                        NoOfSeniorCitizens: '',
                        SalesPaymentCheckList: pDataCheck,
                        SalesPaymentDebitCardList: pDataDebitCard,
                        SalesPaymentCreditCardList: pDataCreditCard,
                        SalesPaymentSRVList: pDataSRV,
                        SalesPaymentDiscountList: pDataDiscount,
                        SalesPaymentSeniorCitizenPWDList: pDataSeniorCitizenPWD,
                        SalesPaymentOtherTypeList: pDataOtherType
                    };


                    if (OfflineMode) {
                        //                        //save in test table so that we can compare
                        //                        pSql = 'UPDATE private.TestData_InvoiceItems SET InvoiceID = ' + ReadyString(sessionStorage.CartOrderID) + ' WHERE ID = ' + ReadyString(sessionStorage.ItemID);
                        //                        Sql.ExecuteSql(pSql);

                        SalesPayment_offline().PreConsumeSrv(pDataSRV,
                        function (vData) {
                            // PARAMS:
                            //  CustomerPaymentID: optional.

                            if (vData != undefined) {
                                pData = $.extend(pData, {
                                    CustomerPaymentID: vData.CustomerPaymentID
                                });
                            }

                            if (SalesPayment_offline().SaveSalesPayments(pData)) {

                                NewCart();
                                if (isLast)
                                    callMsg4();

                            } else {
                                Android.AddLog('Invoice has been changed.');
                                Android.Toast('Invoice has been changed.');
                                //                                MessageBox("Invoice has been changed.",
                                //                                        "Save Receipt",
                                //                                        [
                                //                                            { Text: 'Ok'
                                //                                            }
                                //                                        ]
                                //                                );
                            } // END if(SalesPayment_offline().SaveSalesPayments(pData)){
                        } // END function
                    );


                    } else {
                        AjaxSend({
                            url: "SalesPayments/SaveSalesPayments",
                            data: pData,
                            success: function (vData) {
                                sessionStorage.removeItem('CustomerOrganizationID'); //reset to customer org para mahimong cash ang default

                                if (vData.Error != undefined) {
                                    MessageBox(vData.Error);
                                    return;
                                };

                                if (vData.SalesPaymentData == "InsufficientPayment") {
                                    MessageBox("Please pay full amount due.",
                                       "Insufficient Payment",
                                        [
                                            { Text: 'Ok'
                                            }
                                        ]
                                );
                                } else if (vData.SalesPaymentData == "InvalidEmail") {
                                    MessageBox("Please enter valid email address.",
                                       "Invalid email",
                                        [
                                            { Text: 'Ok'
                                            }
                                        ]
                                );
                                } else if (vData.SalesPaymentData == "DoubleEntry") {
                                    PrintReceipt(pInvoiceID);
                                    return;
                                } else {
                                    NewCart();
                                    sessionStorage.removeItem('CustomerOrganizationID'); //reset to customer org para mahimong cash ang default


                                }; // end if

                            }
                        });
                    }
                }


            });

        });

        window.SetPopUpMessage = function (IsComparing, Table) {

            var vTable = '';
            var msg = '';

            if (vTable.indexOf('Error') > 0) {
                msg = vTable;
            }
            else if (IsComparing == 1) {
                msg = 'Checking updates for ' + vTable;
            }
            else if (IsComparing == 2) {
                msg = 'Updating ' + vTable;
            }
            else {
                msg = Table;
            }

            msg = 'Please Wait ... <br> <br>' + msg;

            $("#dvpopup").popup("close");
            $('#popMessage').html(msg);
            $("#dvpopup").popup("open");
        }
    });