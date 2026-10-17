define(['common',
    GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
    GetPathFormat('/Scripts3/Browser/TransLastestOrderedStockByCustomer.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/Browser/TransSalesOrder.js?crc='),
    'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    'business',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc='
    ], function () {

        var mPOSDeliveryAddressID, mPOSBillingAddressID;

        var mInitialized;

        if (window.SelfTest_offline == undefined) {
            window.SelfTest_offline = function () {

                var mMessageBox; // set after saving; get after exporting

                (function Test() {
                    // never init here. this will be called multiple times
                    //            if (location.hostname != '192.168.1.206') return;
                    //            console.log('SalesMenu_offline.Test');
                    //            mobile.Download({
                    //                Source: 'OfflineData/Schema',
                    //                Destination: 'schema_define_your_own_filename_here.zip',
                    //                Success: function() {
                    //                    Android.Toast('download success');
                    //                    //Android.Extract(this.Destination);
                    //                    //var pString = Android.ReadFile(this.Destination + '/sulod_sa_zip.json');
                    //                },
                    //                Error: function() {
                    //                    Android.Toast('download error');
                    //                }
                    //            });
                })();

                this.createTestTable = function () {
                    var pDT;
                    var pSql;

                    //create new table for the test
                    pSql = 'DROP TABLE IF EXISTS private.TestData_InvoiceItems';
                    Sql.ExecuteSql(pSql);
                    pSql = 'CREATE TABLE private.TestData_InvoiceItems (\'ID\', \'InvoiceID\' ,\'ProductID\',\'Quantity\',\'UnitMeasure\',\'UnitQty\',\'PriceLevelID\', \'Price\')';
                    Sql.ExecuteSql(pSql);

                    //create new table in public for the test
                    pSql = 'DROP TABLE IF EXISTS TestData_Results';
                    Sql.ExecuteSql(pSql);
                    pSql = 'CREATE TABLE TestData_Results (\'PeopleID\' ,\'OrganizationID\',\'CompanyName\',\'DeviceID\',\'Result\',\'TestStatus\',\'NoOfOrgSKU\',\'DataSet\')';
                    Sql.ExecuteSql(pSql);

                    return 'true';
                }; // getPIMProduct


                this.getDeviceFOWST = function () {
                    var pDT;
                    var pSql;

                    try {
                        //open private db
                        Sql.OpenDatabase(sessionStorage.TestOrgID + '.public.db');
                        Sql.AttachDatabase(sessionStorage.TestOrgID + '.private.db');

                    }
                    catch (e) {
                        Android.Toast(e.message);
                        Android.AddLog(e.message);

                        return false;
                    }

                    //get 1 result only. temporary
                    pSql = 'SELECT ID,FowstID FROM private.LibraryDeviceFowst WHERE DeviceID = ' + ReadyString(Web.DeviceID());

                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };
                }; // getFOWST

                this.getOrgName = function () {
                    var pOut = {};
                    var pDT;
                    var pSql;


                    pSql = 'SELECT CompanyName FROM LibraryOrganization WHERE ID = ' + ReadyString(sessionStorage.ActualOrgID);

                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };
                }; // getOrgName


                this.getOrganizationCustomer = function () {
                    var pDT;
                    var pSql;


                    pSql = 'SELECT org.ID FROM LibraryOrganization org INNER JOIN private.LibraryOrganizationCustomer cust ON org.ID = cust.OrganizationID INNER JOIN LibraryOrganizationPriceLevel lvl ON cust.OrganizationID = lvl.OrganizationID WHERE lvl.PriceLevelID = "01000012-0005-0000-0000-000000000000" LIMIT 100';

                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };
                }; // getOrganizationCustomer


                this.getTestDetails = function () {
                    var pDT;
                    var pSql;


                    pSql = 'SELECT OrganizationID,DeviceID,PeopleID, DataSet FROM TestData_Results';

                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };
                }; // getOrganizationCustomer


                this.getOrganizationPIM = function () {
                    var pDT;
                    var pSql;

                    //use inner join to show only results with price level and measurement

                    pSql = 'SELECT DISTINCT orgpim.ProductID, meas.Quantity, lbl.Label AS UnitMeasure, lvl.Price FROM LibraryPIMProduct' +
                ' JOIN private.LibraryOrganizationPIM orgpim ON LibraryPIMProduct.ID = orgpim.ProductID' +
                ' INNER JOIN LibraryPIMProductMeasurement meas ON orgpim.ProductID = meas.ProductID' +
                ' INNER JOIN LibraryOrganization pub ON LibraryPIMProduct.PublishedByID = pub.ID' +
                ' INNER JOIN LibraryOrganization upl ON LibraryPIMProduct.UploadedByID = upl.ID' +
                ' INNER JOIN LibraryPIMProductPriceLevel lvl ON LibraryPIMProduct.ID = lvl.ProductID' +
                ' JOIN LibraryPIMMeasure pimM ON pimM.ID = meas.UnitMeasureID' + 
                ' JOIN LibraryCountriesLanguageLabel lbl ON lbl.ID = pimM.MeasureNameID' + 
                ' WHERE LibraryPIMProduct.IsActive = 1 AND lvl.PriceLevelID = "01000012-0005-0000-0000-000000000000" ' +
                ' AND lvl.OrganizationID = ' + ReadyString(sessionStorage.ActualOrgID) + ' LIMIT 100';
                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };
                }; // getPIMProduct

                this.DeleteOrderStockTest = function (vInvoiceID, vProductID, vUnitMeasure) {

                    var pSql;
                    pSql = 'DELETE FROM TestData_InvoiceItems' +
                ' WHERE InvoiceID = ' + ReadyString(vInvoiceID) +
                ' AND ProductID = ' + ReadyString(vProductID);
                    //additional by janine. if blank, ok lang delete all unit measure of that product, if not then delete only specific unit measure
                    if (vUnitMeasure != '') {
                        pSql = pSql + ' AND UnitMeasure = ' + ReadyString(vUnitMeasure);
                    }

                    Sql.ExecuteSql(pSql);

                }; // DeleteOrderStock


                this.UpdatePos = function (vData) {
                    // equivalent to http://Stock/UpdatePos

                    var pProductID;
                    var pCustomerID;
                    var pDtStocks;
                    var pDrPrice;
                    var pInvoiceID;
                    var pSession;

                    if (!Business.TransInvoice.HandleCorrectDate())
                        return;

                    pProductID = vData.StockID;
                    pCustomerID = vData.CustomerID;
                    vData.NewCartMode = CBool(vData.NewCartMode);

                    pDtStocks = Business.LibraryPIMProduct.PriceTableForTEST(pProductID, pCustomerID); //this is the difference with the update pos in stock offline
                    pSession = Android.GetSessionData();
                    if (!vData.NewCartMode) {
                        pInvoiceID = vData.OrderID;
                        if (IsGuid(pInvoiceID)) {
                            if (!Business.TransInvoice.IsValidCart(pInvoiceID)) {
                                pInvoiceID = '';
                            }; // if (!Business.TransInvoice.IsValidCart(vData.OrderID)) {
                        }; // if (IsGuid(vData.OrderID)) { 

                        if (CStr2(pInvoiceID) == '') {
                            pInvoiceID = Business.TransInvoice.GetCartID(pSession.PeopleUserID, pSession.FowstID);
                        };
                    }; // if (vData.NewCartMode) {



                    var pStockItem;
                    var pSalesRep;
                    var pProductSerial;
                    var pIU;
                    var pSql;
                    var pCost;
                    var pOut;
                    var orderedQty = {};
                    var isNewInvoice; //additional by janine sept 9 2014


                    pIU = new InsertUpdate('TransInvoice');
                    pIU.Add('InvoiceNumber', 0);
                    pIU.Add('InvoiceDate', proj.SqliteNow);
                    pIU.Add('FowstID', ReadyString(pSession.FowstID));
                    pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                    pIU.Add('CustomerID', ReadyString(pCustomerID));
                    pIU.Add('Remarks', ReadyString(vData.Remarks)); // uncomment by janine may 28 2014 5pm . to be used in self test. value is TEST ONLY
                    pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                    pIU.Add('IsCancelled', 0);
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('DeliveryAddressID', ReadyString(mPOSDeliveryAddressID));
                    pIU.Add('BillingAddressID', ReadyString(mPOSBillingAddressID));
                    pIU.Add('DateImported', 'null');

                    //additional by janine may 19 2014
                    pIU.Add('CustomerTableNo', ReadyString(vData.CustomerTableNo));
                    pIU.Add('IsTestOnly', vData.IsTestOnly); // to determine if test only. bool. additional june 2
                    //end of additional

                    Sql.BeginTransaction();
                    //                    if (CStr2(pInvoiceID) != '') {
                    //                        Business.TransInvoice.DeleteOrderStock(pInvoiceID, pProductID);
                    //                    };
                    if (CStr2(pInvoiceID) != '') {
                        isNewInvoice = false; // important. do not remove .additional by janine sept 9 see if (!isNewInvoice)
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(pInvoiceID));
                    } else {
                        isNewInvoice = true; // important. do not remove. additional by janine sept 9 see if (!isNewInvoice)
                        pInvoiceID = NewID();
                        pIU.Add('ID', ReadyString(pInvoiceID));
                        pSql = pIU.GetInsert();
                    };

                    try {
                        Sql.ExecuteSql(pSql);

                        for (var pStockIndex = 0; pStockIndex <= vData.Stocks.length - 1; pStockIndex++) {
                            pIU = new InsertUpdate('TransInvoiceItems'); // disconnected checkout
                            pStockItem = vData.Stocks[pStockIndex];
                            if (pStockItem.Qty <= 0) continue; // can be negative?
                            pDrPrice = {};
                            for (var pPriceIndex = 0; pPriceIndex <= pDtStocks.Rows.length - 1; pPriceIndex++) {
                                if (pDtStocks.Rows[pPriceIndex].Quantity == pStockItem.UnitQty
                            && pDtStocks.Rows[pPriceIndex].PriceLevelID == pStockItem.PriceLevelID) {

                                    pDrPrice = pDtStocks.Rows[pPriceIndex];
                                    break;
                                };
                            };

                            //additional by janine sept 5 2014 for sales order taker
                            if (!isNewInvoice) { // if not new invoice, then delete the existing invoice items that match
                                //additional by janine sept 8 for sales order taker

                                orderedQty = Business.TransInvoice.GetOrderedQty(pInvoiceID, pProductID, pDrPrice.UnitMeasure);
                                //end of additional
                                Business.TransInvoice.DeleteOrderStock(pInvoiceID, pProductID, pDrPrice.UnitMeasure);
                            };
                            if (proj.PosMode) {

                                pIU.Add('Quantity', CDec2(pStockItem.Qty));
                       
                                if (orderedQty.length == undefined) {
                                    pIU.Add('OrderedQuantity', CDec2(pStockItem.Qty)); //save ordered qty same as invoiced qty for POS. so ordered = invoiced
                                } else {
                                    if (orderedQty > 0)
                                        pIU.Add('OrderedQuantity', orderedQty.Rows[0].OrderedQuantity);
                                    else
                                        pIU.Add('OrderedQuantity', CDec2(pStockItem.Qty)); //save ordered qty same as invoiced qty for POS. so ordered = invoiced
                                }
                            }
                            else {
                                pIU.Add('Quantity', 0); // need to save it as blank because it's not yet invoiced!
                                pIU.Add('OrderedQuantity', CDec2(pStockItem.Qty));
                            }

                            /////////////////////
                            pIU.Add('ID', ReadyString(NewID()));
                            pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                            pIU.Add('ProductID', ReadyString(pProductID));
                            //pIU.Add('Quantity', CDec2(pStockItem.Qty));

                            pIU.Add('ActualQuantity', CDec2(pStockItem.UnitQty));
                            pIU.Add('UnitMeasure', ReadyString(pDrPrice.UnitMeasure));
                            pIU.Add('UnitPrice', CDec2(pDrPrice.DiscountedPrice));
                            pCost = Business.LibraryPIMProduct.GetLandedCost(pProductID, sessionStorage.OrgID);
                            if (pCost != undefined)
                                pIU.Add('Cost', pCost);
                            else
                                pIU.Add('Cost', 'null');
                            pIU.Add('PriceLevelID', ReadyString(pDrPrice.PriceLevelID));


                            // pIU.Add('Subtotal', CDec2(pStockItem.Qty) * CDec2(pDrPrice.DiscountedPrice));
                            pIU.Add('Subtotal', CDec2(pStockItem.Qty) * CDec2(pStockItem.UnitPrice));
                            pIU.Add('TaxPercent', 12);
                            pIU.Add('TaxSales', pIU.GetItem('Subtotal') / (1 + pIU.GetItem('TaxPercent') / 100));   // todo: round down to nearest 4 decimal
                            pIU.Add('TaxAmount', pIU.GetItem('Subtotal') - pIU.GetItem('TaxSales'));
                            if (CDec2(pStockItem.RequestedUnitPrice) > 0)
                                pIU.Add('RequestedUnitPrice', CDec2(pStockItem.RequestedUnitPrice));
                            pIU.Add('LineNumber', Business.TransInvoice.GetMaxLineNumber(pInvoiceID) + 1);
                            pSql = pIU.GetInsert();
                            Sql.ExecuteSql(pSql);
                        };
                        Business.TransInvoice.UpdateTotalAmount(pInvoiceID);

                        //Sales Rep
                        pSql = "DELETE FROM private.TransInvoiceItemSalesReps WHERE InvoiceID = " + ReadyString(pInvoiceID) + " AND ProductID = " + ReadyString(pProductID);
                        Sql.ExecuteSql(pSql);

                        pIU = new InsertUpdate('private.TransInvoiceItemSalesReps');
                        for (var pSalesRepIndex = 0; pSalesRepIndex <= vData.SalesRep.length - 1; pSalesRepIndex++) {
                            pSalesRep = vData.SalesRep[pSalesRepIndex];

                            pIU.Add('ID', ReadyString(NewID()));
                            pIU.Add('PeopleID', ReadyString(pSalesRep.SalesRepID));
                            pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                            pIU.Add('ProductID', ReadyString(pSalesRep.ProductID));
                            pSql = pIU.GetInsert();
                            Sql.ExecuteSql(pSql);
                        };

                        //Product Serial
                        pSql = "DELETE FROM private.TransInvoiceItemSerials WHERE InvoiceID = " + ReadyString(pInvoiceID) + " AND ProductID = " + ReadyString(pProductID);
                        Sql.ExecuteSql(pSql);

                        pIU = new InsertUpdate('private.TransInvoiceItemSerials');
                        for (var pProductSerialIndex = 0; pProductSerialIndex <= vData.ProductSerial.length - 1; pProductSerialIndex++) {
                            pProductSerial = vData.ProductSerial[pProductSerialIndex];

                            pIU.Add('ID', ReadyString(NewID()));
                            pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                            pIU.Add('ProductID', ReadyString(pProductSerial.ProductID));
                            pIU.Add('SerialNumber', ReadyString(pProductSerial.ProductSerial));
                            pSql = pIU.GetInsert();
                            Sql.ExecuteSql(pSql);
                        };

                        Sql.CommitTransaction();

                        pOut = {};
                        pOut.Result = 'ok';
                        pOut.OrderID = pInvoiceID;

                        if (pDrPrice != null)
                            pOut.DiscountedPrice = pDrPrice.DiscountedPrice;

                        return pOut;
                        //                return {Result: 'ok',
                        //                    OrderID: pInvoiceID 
                        //                }; // OrderID is actually invoice ID
                    } catch (e) {
                        try {
                            Sql.RollbackTransaction();
                        } catch (e) {
                        };
                        MessageBox(e.message);
                    };

                }; // UpdatePos

                this.SaveCustomerDetails = function (tableNo, invID) {

                    var pIU;
                    var pSql;
                    var pResult;

                    pIU = new InsertUpdate('TransInvoice');
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('CustomerTableNo', ReadyString(tableNo));
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(invID));
                    Sql.ExecuteSql(pSql);

                    return true;

                }; // END SaveCustomerDetails


                return this;

            }; // SelfTest
        }; // END if (window.SelfTest_offline == undefined) {
    });