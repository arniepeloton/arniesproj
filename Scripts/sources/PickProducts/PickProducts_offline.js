define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc='
], function () {
    var mInitialized;

    if (window.PickProducts_offline == undefined) {
        window.PickProducts_offline = function () {
            (function () {

                if (mInitialized)
                    return;
                else
                    mInitialized = true;

                Device.CheckServerDate();
          
                if (Web.HandleOrganization() == false)
                    return;

                //                $($('[data-role=content]')[0]).hide();
                //                Business.TransInvoice.HandleCorrectDate({
                //                    Success: function () {
                //                        $($('[data-role=content]')[0]).show();
                //                        console.log('js9LKZ9BADF5! checking for App.controller.MasterFrame.hasBackgroundIframe');
                //                        if (App.controller.MasterFrame.hasBackgroundIframe()) {
                //                            console.log('jsUC6QMJUE8Y! checking for App.controller.MasterFrame.hasBackgroundIframe is true');
                //                            App.controller.MasterFrame.runExporter();
                //                        } else {
                //                            console.log('js4Q94W86QSE! checking for App.controller.MasterFrame.hasBackgroundIframe is false');
                //                            SalesPaymentsExport_offline().Export({
                //                                OnUploaded: function () {
                //                                    //                                    FollowUp();
                //                                },
                //                                OnDone: function (vData) {
                //                                    if (vData.Result == 'ok')
                //                                        Android.AddLog('Done exporting js2CQHQ5B4QA');
                //                                    else if (vData.Result == 'error')
                //                                        Android.Toast(vData.Error);
                //                                    App.Deferred.fulfill('KWXCETE5XN');
                //                                }
                //                            }); // END SalesPaymentsExport_offline().Export({
                //                        }
                //                    } // END Success
                //                }); // END Business.TransInvoice.HandleCorrectDate


            })(); // END (function() {

            this.saveFowstBinProduct = function (binID, productID) {

                var pIU;
                var pSql;
                //check if existing
                pIU = new InsertUpdate('private.LibraryFowstBinProduct');
                pIU.Add('ID', ReadyString(NewID()));
                pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                pIU.Add('BinID', ReadyString(binID));
                pIU.Add('ProductID', ReadyString(productID));
                pIU.Add('IsDefault', 0);
                pIU.Add('MinPerProduct', 0);
                pIU.Add('MaxPerProduct', 0);
                pIU.Add('LastUpdated', proj.SqliteNow);
                pSql = pIU.GetInsert();

                Sql.ExecuteSql(pSql);


                if (CStr2(pBinID) != '') {
                    //Save bin id
                    pSql = "DELETE FROM private.TransInvoiceItemWarehouseWithdrawal WHERE InvoiceID = " + ReadyString(sessionStorage.CartOrderID) +
                            " AND ProductID = " + ReadyString(pProductID);

                    Sql.ExecuteSql(pSql);


                    pIU = new InsertUpdate('TransInvoiceItemWarehouseWithdrawal');
                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                    pIU.Add('ProductID', ReadyString(pProductID));
                    pIU.Add('FowstBinID', ReadyString(pBinID));
                    pIU.Add('PreparedQuantity', CDec2(pStockItem.Qty));
                    pIU.Add('DateImported', 'null');

                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql);
                }

                return 'ok';


            };
            this.getProductBinLocation = function (prodID) {

                pSql = "SELECT DISTINCT transBin.PreparedQuantity, binProd.BinID, bin.Name" +
                    " FROM private.LibraryFowstBinProduct binProd " +
                    " INNER JOIN private.LibraryFowstBin bin ON binProd.BinID = bin.ID" +
                    " LEFT JOIN private.TransInvoiceItemWarehouseWithdrawal transBin ON transBin.BinID = binProd.BinID AND transBin.ProductID = binProd.ProductID" +
                    " WHERE binProd.ProductID = " + ReadyString(prodID) + " AND binProd.FowstID = " + ReadyString(sessionStorage.FowstID) +
                    " AND transBin.InvoiceID = " + ReadyString(sessionStorage.CartOrderID);

                var pDT = Sql.Execute(pSql);
                if (pDT == undefined) return null;

                return pDT;

            };

            this.PosCart = function (vData) {

                var pInvoiceID;
                var pDsCart;
                var pOut;

                pInvoiceID = vData.OrderID; // yes, OrderID is not a typo

                if (IsGuid(pInvoiceID)) {
                    if (!Business.TransInvoice.IsValidCart(pInvoiceID))
                        return { 'text': 'No item' };
                } else {
                    if (CBool(vData.DontFetchIfBlank))
                        return { 'text': 'No item' };
                };

                pDsCart = Business.TransInvoice.GetCart(pInvoiceID);

                if (pDsCart != undefined) {
                    pOut = {};
                    pOut.Master = pDsCart.Invoices.Rows[0];
                    pOut.Units = pDsCart.Stocks;

                    pOut.Stocks = JSON.parse(JSON.stringify(pOut.Units)); // clone JS objects; http://heyjavascript.com/4-creative-ways-to-clone-objects/
                    pOut.Stocks.Rows = GetDistinct(pOut.Stocks.Rows, ['ProductID', 'ProductNo', 'ProductName']);

                    return pOut;
                } else
                    return { 'text': 'No item' };

            }; // PosCart

            this.Import = function (vData) {

                var pPOSDeliveryAddressID;
                var pPOSBillingAddressID;
                var pMessage;

                console.log('Import invoice data from server');
                if (Web.HandleOrganization() == false)
                    return;

                pPOSDeliveryAddressID = Business.SystemDefaults.POSDeliveryAddressID();
                if (!IsGuid(pPOSDeliveryAddressID))
                    if (CStr2(pPOSDeliveryAddressID) == '')
                        pMessage = 'POS Delivery Address ID is blank.';
                    else
                        pMessage = 'POS Delivery Address ID must be a valid GUID.';

                pPOSBillingAddressID = Business.SystemDefaults.POSBillingAddressID();
                if (!IsGuid(pPOSBillingAddressID))
                    if (CStr2(pPOSBillingAddressID) == '')
                        pMessage = 'POS Billing Address ID is blank.';
                    else
                        pMessage = 'POS Billing Address ID must be a valid GUID.';

                if (pMessage != undefined)
                    MessageBox(pMessage + ' Please set in SystemDefaults', null,
                        [{
                            Text: 'Go to People Menu'
                        }],
                        {
                            OnClose: function () {
                                window.location = '/People/PeopleMenu?IsMobile=1';
                            }
                        });

                ImportInvoice(vData.Master);
                ImportItems(vData.Units.Rows);

                function ImportInvoice(vData) {
                    if (!CheckIsInvoiceExist(vData.ID)) {
                        var pIU = new InsertUpdate('TransInvoice');
                        var pSql;

                        pIU.Add('ID', ReadyString(vData.ID));
                        pIU.Add('CustomerID', ReadyString(vData.CustomerID));
                        pIU.Add('FowstID', ReadyString(vData.FowstID));
                        pIU.Add('InvoiceDate', ReadyString(vData.InvoiceDate));
                        pIU.Add('InvoiceNumber', ReadyString(vData.InvoiceNumber));
                        pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));
                        pIU.Add('UserID', ReadyString(vData.UserID));
                        pIU.Add('DeviceID', ReadyString(vData.DeviceID));
                        pIU.Add('DeliveryAddressID', ReadyString(pPOSDeliveryAddressID));
                        pIU.Add('BillingAddressID', ReadyString(pPOSBillingAddressID));
                        if (IsDate(vData.SalesOrderDate))
                            pIU.Add('SalesOrderDate', ReadyString(vData.SalesOrderDate));
                        else
                            pIU.Add('SalesOrderDate', 'null');
                        if (IsNumeric(vData.SalesOrderNo))
                            pIU.Add('SalesOrderNo', CDec2(vData.SalesOrderNo));
                        else
                            pIU.Add('SalesOrderNo', 'null');
                        if (IsGuid(vData.SalesOrderUserID))
                            pIU.Add('SalesOrderUserID', ReadyString(vData.SalesOrderUserID));
                        else
                            pIU.Add('SalesOrderUserID', 'null');
                        pIU.Add('IsCancelled', 0);

                        pSql = 'DELETE FROM TransInvoiceItems WHERE InvoiceID = ' + ReadyString(vData.ID);
                        Sql.ExecuteSql(pSql);

                        //                            pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                        //                            if (Sql.ExecuteSql(pSql) == 0) {
                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql);
                        //                            };
                    }
                };

                function ImportItems(vData) {

                    var pIU;
                    var pSql;
                    var pData;

                    for (var p = 0; p <= vData.length - 1; p++) {
                        pData = vData[p];
                        pIU = new InsertUpdate('TransInvoiceItems');
                        pIU.Add('ID', ReadyString(pData.ID));
                        pIU.Add('ActualQuantity', CDec2(pData.ActualQuantity));
                        pIU.Add('InvoiceID', ReadyString(pData.InvoiceID));
                        pIU.Add('ProductID', ReadyString(pData.ProductID));
                        pIU.Add('OrderedQuantity', CDec2(pData.OrderedQuantity)); //additional by janine sept 10 1pm for sales order taker
                        pIU.Add('Quantity', CDec2(pData.Quantity));
                        pIU.Add('UnitPrice', CDec2(pData.UnitPrice));
                        pIU.Add('UnitMeasure', ReadyString(pData.UnitMeasure));
                        pIU.Add('PriceLevelID', ReadyString(pData.PriceLevelID));
                        pIU.Add('SubTotal', CDec2(pData.SubTotal));
                        pIU.Add('LineNumber', CDec2(pData.LineNumber));
                        pIU.Add('TaxPercent', CDec2(pData.TaxPercent));
                        pIU.Add('TaxAmount', CDec2(pData.TaxAmount));
                        pIU.Add('TaxSales', CDec2(pData.TaxSales));
                        pIU.Add('VATableSale', CDec2(pData.SubTotal));
                        pIU.Add('VATExemptSale', '0');
                        pIU.Add('ZeroRatedSale', '0');
                        pIU.Add('NonVATSale', '0');

                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql);
                    };

                };

            }; // END this.Import = function(vData) {


            this.CheckIsInvoiceExist = function (invID) {
                var result;

                pSql = 'SELECT ID FROM private.TransInvoice WHERE ID = ' + ReadyString(invID);
                pDT = Sql.Execute(pSql);

                if (pDT.Rows.length > 0)
                    result = true;
                else
                    result = false;

                return result;
            };

            this.SaveBasketInfo = function (vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.TransInvoiceItems');

                pIU.Add('BasketName', ReadyString(vData.BasketName));

                pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.CartOrderID));

                Sql.ExecuteSql(pSql);

                return true;

            };

            this.GetBasketInfo = function (invID) {
                var result;

                pSql = 'SELECT BasketID FROM private.TransInvoice WHERE ID = ' + ReadyString(invID);
                pDT = Sql.Execute(pSql);

                return pDT;
            };


            return this;

        }; // SalesPayment_offline
    };
});