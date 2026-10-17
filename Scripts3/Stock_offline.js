define(['common',
    GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
    GetPathFormat('/Scripts3/Browser/TransLastestOrderedStockByCustomer.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/Browser/TransSalesOrder.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    'business',
    '/Scripts3/Offline/Web.js?crc='
    ], function () {

        var mInitialized;
        var mPOSDeliveryAddressID, mPOSBillingAddressID;
        var mAllowFreeTypeAPrice;
        var mAllowFreeTypeADiscountAmount;
        var mAllowFreeTypeADiscountPercentage;
        var mAllowToSellBelowCost;
        var mAllowFreeTypeDiscountedPrice;

        window.Stock_offline = function () {

            (function Init() {
                var pMessage;

                if (mInitialized)
                    return;
                else
                    mInitialized = true;

                mPOSDeliveryAddressID = Business.SystemDefaults.POSDeliveryAddressID();
                if (!IsGuid(mPOSDeliveryAddressID))
                    if (CStr2(mPOSDeliveryAddressID) == '')
                        pMessage = 'POS Delivery Address ID is blank.';
                    else
                        pMessage = 'POS Delivery Address ID must be a valid GUID.';

                mPOSBillingAddressID = Business.SystemDefaults.POSBillingAddressID();
                if (!IsGuid(mPOSBillingAddressID))
                    if (CStr2(mPOSBillingAddressID) == '')
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
            })();

            this.GetInfoPos = function (vData) {
                // equivalent to http://Stock/GetInfoPos

                var pInvoiceID = '';
                var pSession;
                var pDT, pDR;
                var pProductID;
                var pOut;
                var pCustomerID;
                var pDtCartStock;
                var pSql;
                var pDefaultPriceID; // DefaultSellingPriceLevel (for POS/SO) or POPriceLevel (for PO)

                //console.time('Stock_offline.GetInfoPos before PriceTable');
                pProductID = vData.ProductID;
                pCustomerID = vData.CustomerID;
                vData.NewCartMode = CBool(vData.NewCartMode);

                if (!vData.NewCartMode) {
                    pInvoiceID = vData.OrderID;
                    if (IsGuid(pInvoiceID)) {
                        if (!Business.TransInvoice.IsValidCart(pInvoiceID)) {
                            pInvoiceID = '';
                        }; // if (!Business.TransInvoice.IsValidCart(vData.OrderID)) {
                    }; // if (IsGuid(vData.OrderID)) { 

                    pSession = Android.GetSessionData();
                    if (CStr2(pInvoiceID) == '') {
                        pInvoiceID = Business.TransInvoice.GetCartID(pSession.PeopleUserID, pSession.FowstID);
                    };

                    if (CStr2(pInvoiceID) != '') {
                        proj.GroupIdOfItemModifiers = vData.ModifierGroupID;
                        pDtCartStock = Business.TransInvoice.GetCartStock(pInvoiceID, pProductID, vData.PosMode, (proj.GroupIdOfItemModifiers));
                    };

                }; // if (vData.NewCartMode) {

                pDT = Business.LibraryPIMProduct.ByIDs(ReadyString(pProductID));
                if (pDT.Rows.length == 0) {
                    return { Error: 'Non-existing Product ID' };
                };
                pDR = pDT.Rows[0];

                pOut = {};
                pOut.ProductNo = pDR.ProductNo;
                pOut.ProductName = pDR.ProductName;
                var pDontPoMode;

                pDontPoMode = (ReadyGuid(sessionStorage.OrgID) == ReadyGuid(vData.SupplierOrgID));
                var pDrFranchiseeData;
                if (pDontPoMode)
                    pDrFranchiseeData = {};
                else
                    pDrFranchiseeData = Business.LibraryOrganizationFOWST.GetFranchiseeData(sessionStorage.FowstID);
                //console.timeEnd('Stock_offline.GetInfoPos before PriceTable');
                //console.time('Stock_offline.GetInfoPos during PriceTable');
                // delete pDrFranchiseeData.FranchiseGroupID; // for debugging only
                pOut.data = Business.LibraryPIMProduct.PriceTable({
                    ID: pProductID,
                    CustomerID: pCustomerID,
                    FranchiseGroupID: pDrFranchiseeData.FranchiseGroupID,
                    DontPoMode: pDontPoMode
                });
                if (!$.isEmptyObject(pDrFranchiseeData)) {
                    if (vData.PosMode == 'po')
                        pDefaultPriceID = pDrFranchiseeData.POPriceLevel;
                    else
                        pDefaultPriceID = pDrFranchiseeData.DefaultSellingPriceLevel;
                }
                    
                if (pOut.data.Rows.length == 0) {
                    // no price for this customer, so add retail price from cash
                    var pDefaultCustomerID = Business.SystemDefaults.CustomerOrganizationID();
                    if (CStr2(pDefaultCustomerID).trim() != '') {
                        pOut.data = Business.LibraryPIMProduct.PriceTable(pProductID, pDefaultCustomerID);
                    }
                }
                //console.timeEnd('Stock_offline.GetInfoPos during PriceTable');
                //console.time('Stock_offline.GetInfoPos after PriceTable');

                var pQtysIndex;
                var pRows;
                var pQtys;
                var pRowItem;

                //console.time('Stock_offline.GetInfoPos unit measures');
                if (!IsGuid(pDefaultPriceID))
                    pDefaultPriceID = proj.POMode ? ReadyGuid(sessionStorage.SupplierPriceLevelID) : Business.SystemDefaults.GetRetailPriceID();
                pOut.UnitMeasures = JSON.parse(JSON.stringify(pOut.data)); // clone JS objects; http://heyjavascript.com/4-creative-ways-to-clone-objects/
                pQtys = pOut.UnitMeasures.Rows;
                pQtys = GetDistinct(pQtys, ['Quantity']);
                pRows = [];

                // sort unit measures
                if (IsGuid(pDefaultPriceID)) {
                    pDefaultPriceID = ReadyGuid(pDefaultPriceID);
                    var pUnitMeasureRows = [];
                    for (var pUnitIndex = 0; pUnitIndex <= pOut.UnitMeasures.Rows.length - 1; pUnitIndex++) {
                        pRowItem = pOut.UnitMeasures.Rows[pUnitIndex];
                        if (ReadyGuid(pRowItem.PriceLevelID) == pDefaultPriceID)
                            pUnitMeasureRows.push(pRowItem);
                    }
                    for (var pUnitIndex = 0; pUnitIndex <= pOut.UnitMeasures.Rows.length - 1; pUnitIndex++) {
                        pRowItem = pOut.UnitMeasures.Rows[pUnitIndex];
                        if (ReadyGuid(pRowItem.PriceLevelID) != pDefaultPriceID)
                            pUnitMeasureRows.push(pRowItem);
                    }
                    pOut.UnitMeasures.Rows = pUnitMeasureRows;
                }

                for (pQtysIndex = 0; pQtysIndex <= pQtys.length - 1; pQtysIndex++) {
                    for (var pUnitIndex = 0; pUnitIndex <= pOut.UnitMeasures.Rows.length - 1; pUnitIndex++) {
                        if (pQtys[pQtysIndex].Quantity == pOut.UnitMeasures.Rows[pUnitIndex].Quantity) {
         
                            // if (pDefaultPriceID == ReadyGuid(pOut.UnitMeasures.Rows[pUnitIndex].PriceLevelID)) {
                            // if (pUnitIndex == 0) {
                        
                                pRowItem = pOut.UnitMeasures.Rows[pUnitIndex];
                                //if (!$.isEmptyObject(pDrFranchiseeData)
                                //    && (IsGuid(pDefaultPriceID))) {

                                //    if (ReadyGuid(pRowItem.PriceLevelID) != ReadyGuid(pDefaultPriceID))
                                //        continue;
                                //}
                                    
                                for (var pUnitIndex2 = 0; pUnitIndex2 <= pOut.UnitMeasures.Rows.length - 1; pUnitIndex2++) {
                                    if (ReadyGuid(pOut.UnitMeasures.Rows[pUnitIndex2].PriceLevelID) == ReadyGuid(Business.LibraryPIMProduct.GetSrpPriceID()))
                                        pRowItem.Srp = pOut.UnitMeasures.Rows[pUnitIndex2].DiscountedPrice;
                                }
                                pRows.push(pRowItem);
                                break;
                            }
                        //};
                    }
                }

                if (pDtCartStock != undefined
                    && pDtCartStock.Rows.length > 0) {

                    for (var pUnitIndex = 0; pUnitIndex <= pRows.length - 1; pUnitIndex++) {
                        for (var pCartIndex = 0; pCartIndex <= pDtCartStock.Rows.length - 1; pCartIndex++) {
                            if (pRows[pUnitIndex].Quantity == pDtCartStock.Rows[pCartIndex].ActualQuantity) {
                                pRows[pUnitIndex].OrderedQty = pDtCartStock.Rows[pCartIndex].Quantity;
                                pRows[pUnitIndex].PriceLevelID = pDtCartStock.Rows[pCartIndex].PriceLevelID;
                                pRows[pUnitIndex].RequestedUnitPrice = pDtCartStock.Rows[pCartIndex].RequestedUnitPrice;
                                pRows[pUnitIndex].UnmodifiedDiscountedPrice = pRows[pUnitIndex].DiscountedPrice;

                                if (IsNumeric(pDtCartStock.Rows[pCartIndex].UnitPrice))
                                    pRows[pUnitIndex].DiscountedPrice = pDtCartStock.Rows[pCartIndex].UnitPrice;
                                else if (IsNumeric(pRows[pUnitIndex].RequestedUnitPrice))
                                    pRows[pUnitIndex].DiscountedPrice = pRows[pUnitIndex].RequestedUnitPrice;
                                break;
                            };
                        };
                    };
                };

                pOut.UnitMeasures.Rows = pRows;
                //console.timeEnd('Stock_offline.GetInfoPos unit measures');

                //console.time('Stock_offline.GetInfoPos sales rep and product serials. js6B3F6VHJ6Y!');
                //Sales Rep
                pSql = "SELECT srep.PeopleID " +
                    ",IFNULL(ppl.Surname, \'\') " +
                    " || \',\' || IFNULL(ppl.FirstName, \'\') " +
                    " || \',\' || IFNULL(ppl.MiddleName, \'\') as PeopleName " +
                    " FROM private.TransInvoiceItemSalesReps srep " +
                    " INNER JOIN LibraryPeople ppl ON (srep.PeopleID = ppl.ID) " +
                    " WHERE srep.InvoiceID = " + ReadyString(pInvoiceID) +
                    " AND srep.ProductID = " + ReadyString(pProductID)

                var pDTSRep = Sql.Execute(pSql);
                pOut.SalesReps = pDTSRep;

                //Product Serial
                pSql = "SELECT prdserial.ID" +
                    ",prdserial.SerialNumber " +
                    " FROM private.TransInvoiceItemSerials prdserial " +
                    " WHERE prdserial.InvoiceID = " + ReadyString(pInvoiceID) +
                    " AND prdserial.ProductID = " + ReadyString(pProductID);

                var pDTPrdSerial = Sql.Execute(pSql);
                pOut.ProductSerial = pDTPrdSerial;
                //console.timeEnd('Stock_offline.GetInfoPos sales rep and product serials. js6B3F6VHJ6Y!');

                //console.time('Stock_offline.GetInfoPos others. jsQVGBAWUKGQ!');

                var pLandedCost = Business.LibraryPIMProduct.GetLandedCost(pProductID, sessionStorage.OrgID);
                //console.timeLog('Stock_offline.GetInfoPos others. jsQVGBAWUKGQ!'); // console.timeLog = "undefined is not a function" sa ami duos
                var pRetailPrice = Business.LibraryPIMProduct.GetRetailPrice(pProductID, sessionStorage.OrgID);
                //console.timeLog('Stock_offline.GetInfoPos others. jsQVGBAWUKGQ!'); // console.timeLog = "undefined is not a function" sa ami duos

                // pOut.HasLandedCost = (pLandedCost != undefined);
                pOut.HasLandedCost = true; // tanggalon muna ang trapping. march 15, 2014
                pOut.HasRetailPrice = (pRetailPrice != undefined);
                pOut.AllowDecimal = Business.LibraryPIMProduct.GetAllowDecimal(pProductID);
                //console.timeLog('Stock_offline.GetInfoPos others. jsQVGBAWUKGQ!'); // console.timeLog = "undefined is not a function" sa ami duos

                //additional by janine feb 21 2015 
                //CHECK IF VAT EXEMPT
                var pSqlVATInfo = "SELECT VATExemptSale, VATableSale" +
                    " FROM private.TransInvoiceItems " +
                    " WHERE InvoiceID = " + ReadyString(pInvoiceID) +
                    " AND ProductID = " + ReadyString(pProductID);

                var pDTPrdVATInfo = Sql.Execute(pSqlVATInfo);
                pOut.VATInfo = pDTPrdVATInfo;
                //console.timeLog('Stock_offline.GetInfoPos others. jsQVGBAWUKGQ!'); // console.timeLog = "undefined is not a function" sa ami duos

                // end of additional

                //console.time('Stock_offline.GetInfoPos GetCapsByFowstID. jsLXXL723ZVM!');
                pDT = Business.LibraryOrganizationCustomerFowstBinProduct.GetCapsByFowstID(sessionStorage.CustomerFowstID, pProductID);
                if (pDT.Rows.length > 0) {
                    pDR = pDT.Rows[0];
                    if (CDec2(pDR.MinPerProduct) > 0) {
                        pOut.MinPerProduct = CDec2(pDR.MinPerProduct);
                    }
                    if (CDec2(pDR.MaxPerProduct) > 0) {
                        pOut.MaxPerProduct = CDec2(pDR.MaxPerProduct);
                    }
                }
                //console.timeEnd('Stock_offline.GetInfoPos GetCapsByFowstID. jsLXXL723ZVM!');

                // commented 2021_02_17__12_34_49_8169
                //pOut.HasModifier = window.Business.LibraryItemModifiers.HasModifier(pProductID);
                //if (pOut.HasModifier)
                //    if (window.DebugMode) {
                //        var pDtRebates = window.Business.LibraryItemModifiers.GetRebates(pProductID);
                //        if (pDtRebates)
                //            console.table(pDtRebates.Rows);
                //    }

                //console.timeEnd('Stock_offline.GetInfoPos others. jsQVGBAWUKGQ!');
                //console.timeEnd('Stock_offline.GetInfoPos after PriceTable');
                return pOut;

            }; // this.GetInfoPos = function() {

            this.UpdatePos = function (vData) {
                /**********************************************************************************
                equivalent to http://Stock/UpdatePos
                sample vData:
                {
                "StockID": "9d952e0d-324c-43ac-bd80-74522d69124b",
                "CustomerID": "cd71a0f7-08a9-4601-876e-5f13c064f873",
                "NewCartMode": "1",
                "FowstID": "abe6d501-3cad-4e79-88ab-f81bbeac9a93",
                "ProductID": "9d952e0d-324c-43ac-bd80-74522d69124b", // never used
                "Stocks": [
                {
                "Qty": "4355",
                "UnitQty": "1",
                "PriceLevelID": "01000012-0005-0000-0000-000000000000"
                // UnitPrice is optional
                }
                ],
                "Remarks": "",
                "IsTestOnly": 0,
                "SalesRep": [],
                "ProductSerial": []
                }
                **********************************************************************************/
                var pID;
                var pProductID;
                var pCustomerID;
                var pDtStocks;
                //var pDrPrice;
                var pInvoiceID;
                var pSession;
                var pVerified;

                //console.time('Bs_Stock.Stock_offline().UpdatePos jsJTBUSAW2KS!');
                if (!Business.TransInvoice.HandleCorrectDate())
                    return;

                pProductID = vData.StockID;
                pCustomerID = vData.CustomerID;
                vData.NewCartMode = CBool(vData.NewCartMode);

                // pDtStocks = Business.LibraryPIMProduct.PriceTable(pProductID, pCustomerID);
                pSession = Android.GetSessionData();
                // sample pSession:
                //{
                //    "PeopleUserID": "c530bdf3-ea03-4dd3-95cd-a384374aa570",
                //    "Username": "dd",
                //    "Password": "dd",
                //    "OrgID": "9d4b1500-13c0-49f9-a8e5-a16c13ec899e",
                //    "FowstID": "abe6d501-3cad-4e79-88ab-f81bbeac9a93"
                //}

                // BEGIN verify vData
                function Verify() {
                    var pStockItem;
                    //var pDrPrice;
                    var pUnitPrice;
                    for (var pStockIndex = 0; pStockIndex <= vData.Stocks.length - 1; pStockIndex++) {
                        pStockItem = vData.Stocks[pStockIndex];
                        if (pStockItem == undefined) continue;
                        if (pStockItem.Qty < 0)
                            pStockItem.Qty = 0; // prevent negative
                        if (!IsGuid(pStockItem.PriceLevelID)) {
                            return {
                                success: false,
                                error: 'Line item ' + pStockIndex + ' does not have PriceLevelID'
                            };
                        }

                        //pDrPrice = {};
                        //for (var pPriceIndex = 0; pPriceIndex <= pDtStocks.Rows.length - 1; pPriceIndex++) {
                        //    if (pDtStocks.Rows[pPriceIndex].Quantity == pStockItem.UnitQty
                        //            && ReadyGuid(pDtStocks.Rows[pPriceIndex].PriceLevelID) == ReadyGuid(pStockItem.PriceLevelID)) {

                        //        pDrPrice = pDtStocks.Rows[pPriceIndex];
                        //        break;
                        //    };
                        //};

                        if (pStockItem.UnitPrice == undefined) {
                            return {
                                success: false,
                                error: 'Line item ' + pStockIndex + ' has an invalid UnitPrice'
                            };
                            //if (!IsNumeric(pDrPrice.DiscountedPrice))
                            //    return {
                            //        success: false,
                            //        error: 'Line item ' + pStockIndex + '\'s PIM has an invalid UnitPrice'
                            //    };
                            //if (CDec2(pDrPrice.DiscountedPrice) < 0)
                            //    return {
                            //        success: false,
                            //        error: 'Line item ' + pStockIndex + '\'s PIM\'s UnitPrice is less than zero'
                            //    };
                            //pUnitPrice = pDrPrice.DiscountedPrice;
                        } else {
                            if (!IsNumeric(pStockItem.UnitPrice))
                                return {
                                    success: false,
                                    error: 'Line item ' + pStockIndex + ' has an invalid UnitPrice'
                                };
                            if (CDec2(pStockItem.UnitPrice) < 0)
                                return {
                                    success: false,
                                    error: 'Line item ' + pStockIndex + '\'s UnitPrice is less than zero'
                                };
                            pUnitPrice = pStockItem.UnitPrice;
                        }

                        //pCost = Business.LibraryPIMProduct.GetLandedCost(pProductID, pSession.OrgID);
                        //if (pCost != undefined) {
                        //    if (CDec2(pUnitPrice) < CDec2(pCost)) {
                        //        if (!this.GetAllowToSellBelowCost())
                        //            return {
                        //                success: false,
                        //                error: 'Line item ' + pStockIndex + ' is selling below cost.'
                        //            };
                        //    }
                        //}
                    }; // END for (var pStockIndex = 0; pStockIndex <= vData.Stocks.length - 1; pStockIndex++) {

                    return {
                        success: true
                    };
                }
                // END verify vData
                pVerified = Verify() || {};
                if (!pVerified.success) {
                    MessageBox(pVerified.error, 'Verified failed on Stock_offline.UpdatePos');
                    return {
                        Error: pVerified.error
                    };
                }

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
                var pTransNum;
                var isNewInvoice; //additional by janine sept 9 2014
                var pDtFowst = Business.LibraryOrganizationFOWST.GetData(sessionStorage.FowstID);

                Sql.BeginTransaction();
                function SaveHeader() {
                    
                    var pInvoiceDate;

                    pIU = new InsertUpdate('TransInvoice');
                    pIU.Add('InvoiceNumber', 0);
                    pInvoiceDate = proj.GetZReadDate();
                    pIU.Add('InvoiceDate', ReadyDate(pInvoiceDate));
                    pIU.Add('FowstID', ReadyString(pSession.FowstID));
                    pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                    pIU.Add('CustomerID', ReadyString(pCustomerID));
                    pIU.Add('Remarks', ReadyString(vData.Remarks)); // uncomment by janine may 28 2014 5pm . to be used in self test. value is TEST ONLY
                    pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                    pIU.Add('IsCancelled', 0);
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('DeliveryAddressID', CStr2(vData.DeliveryAddressID) == '' ? ReadyString(mPOSDeliveryAddressID) : ReadyString(vData.DeliveryAddressID));
                    pIU.Add('BillingAddressID', CStr2(vData.BillingAddressID) == '' ? ReadyString(mPOSBillingAddressID) : ReadyString(vData.BillingAddressID));
                    pIU.Add('DeliveryDate', CStr2(vData.DeliveryDate) == '' ? proj.SqliteNow : ReadyString(vData.DeliveryDate));
                    if (CBool(sessionStorage.getItem('IsOrderTaker')))
                        pIU.Add('DateImported', proj.SqliteNow);
                    else
                        pIU.Add('DateImported', 'null');

                    //additional by janine may 19 2014
                    pIU.Add('CustomerTableNo', ReadyString(vData.CustomerTableNo));
                    pIU.Add('IsTestOnly', CBool(vData.IsTestOnly) ? 1 : 0); // to determine if test only. bool. additional june 2

                    pIU.Add('DeliveryMethod', CStr2(vData.DeliveryMethod) == '' ? 'NULL' : ReadyString(vData.DeliveryMethod));
                    pIU.Add('OrderMethod', ReadyString(vData.OrderMethod));
                    pIU.Add('CustomerDemographicGender', ReadyString(vData.Gender));
                    pIU.Add('CustomerDemographicAge', ReadyString(vData.Age));
                    pIU.Add('CustomerTableNo', ReadyString(vData.CustomerTableNo));
                    pIU.Add('TotalNumberOfCustomer', ReadyString(vData.TotalNumberOfCustomer));
                    if (IsGuid(vData.SupplierID))
                        pIU.Add('SupplierID', ReadyString(vData.SupplierID));
                    else if (IsGuid(sessionStorage.SupplierOrganizationID))
                        pIU.Add('SupplierID', ReadyString(sessionStorage.SupplierOrganizationID));
                    else
                        pIU.Add('SupplierID', 'null');
                    if (vData.IsPurchaseOrder) {
                        pIU.Add('PurchaserName', ReadyString(sessionStorage["CurrentUserFullName"]));
                    
                        if (CStr2(sessionStorage.SupplierFowstID).trim() != '')
                            pIU.Add('SupplierFowstID', ReadyString(sessionStorage.SupplierFowstID));
                        else
                            pIU.Add('SupplierFowstID', 'null');

                        if (CStr2(sessionStorage.SupplierBinID).trim() != '')
                            pIU.Add('SupplierBinID', ReadyString(sessionStorage.SupplierBinID));
                        else
                            pIU.Add('SupplierBinID', 'null');

                        //if (CStr2(sessionStorage.CustomerOrgID).trim() != '')
                        //    pIU.Add('CustomerOrgID', ReadyString(sessionStorage.CustomerOrgID));
                        //else
                        //    pIU.Add('CustomerOrgID', 'null');

                        if (CStr2(sessionStorage.CustomerFowstID).trim() != '')
                            pIU.Add('CustomerFowstID', ReadyString(sessionStorage.CustomerFowstID));
                        else
                            pIU.Add('CustomerFowstID', 'null');

                        if (CStr2(sessionStorage.CustomerBinID).trim() != '')
                            pIU.Add('CustomerBinID', ReadyString(sessionStorage.CustomerBinID));
                        else
                            pIU.Add('CustomerBinID', 'null');
                    }
                    //end of additional

                    if (CStr2(pInvoiceID) != '') {
                        isNewInvoice = false; // important. do not remove .additional by janine sept 9 see if (!isNewInvoice)
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(pInvoiceID));
                    } else {
                        isNewInvoice = true; // important. do not remove. additional by janine sept 9 see if (!isNewInvoice)
                        pInvoiceID = NewID();
                        pIU.Add('ID', ReadyString(pInvoiceID));
                        pIU.Add('IsPrinted', 0);
                        
                        var pIncrementedInt;

                        pTransNum = $.format.date(pInvoiceDate, 'yyMMdd');

                        if (pDtFowst.Rows.length > 0) 
                            pTransNum += pDtFowst.Rows[0].Code;
                        pIncrementedInt = Business.TransInvoice.GetDatedCounterFileContent(sessionStorage.FowstID, Web.DeviceID(), pInvoiceDate) + 1;
                        pTransNum += $.format.number(pIncrementedInt, '000000');
                        Business.TransInvoice.UpdateDatedCounterFile(sessionStorage.FowstID, Web.DeviceID(), pInvoiceDate, pIncrementedInt);
                        pIU.Add('TransNum', ReadyString(pTransNum));
                        pIU.Add('IsNoneVATSale', pDtFowst.Rows[0].IsVat);
                        pSql = pIU.GetInsert();
                    }
                    Sql.ExecuteSql(pSql);
                }
                try {
                    if (!CBool(vData.DontSaveHeader))
                        SaveHeader();
                    var pOldInvoiceAmount = null;
                    if (vData.UpdateErrorCorrectAmount)
                        pOldInvoiceAmount = Business.TransInvoice.GetTotalAmount(pInvoiceID);
                    for (var pStockIndex = 0; pStockIndex <= vData.Stocks.length - 1; pStockIndex++) {
                        pStockItem = vData.Stocks[pStockIndex];
                        if (pStockItem == undefined) continue;
                        if (pStockItem.Qty < 0)
                            pStockItem.Qty = 0; // prevent negative
                        pIU = new InsertUpdate('TransInvoiceItems'); // disconnected checkout
                        var pQuantityPrinted = 0;
                        var orderedqty = 0;
                        //pDrPrice = {};
                        //for (var pPriceIndex = 0; pPriceIndex <= pDtStocks.Rows.length - 1; pPriceIndex++) {
                        //    if (pDtStocks.Rows[pPriceIndex].Quantity == pStockItem.UnitQty
                        //        && ReadyGuid(pDtStocks.Rows[pPriceIndex].PriceLevelID) == ReadyGuid(pStockItem.PriceLevelID)) {

                        //        pDrPrice = pDtStocks.Rows[pPriceIndex];
                        //        break;
                        //    }
                        //}

                        //additional by janine sept 5 2014 for sales order taker
                        if (!isNewInvoice) { // if not new invoice, then delete the existing invoice items that match
                            //additional by janine sept 8 for sales order taker

                            orderedQty = Business.TransInvoice.GetOrderedQty(pInvoiceID, pProductID, pStockItem.UnitMeasure);
                            pQuantityPrinted = Business.TransInvoice.GetQuantityPrintedByProductIDUnitMeasureModifierGroupID(pInvoiceID, pProductID, pStockItem.UnitMeasure, pStockItem.GroupIdOfItemModifiers);
                            //end of additional
                            Business.TransInvoice.DeleteOrderStock(pInvoiceID, 
                                pProductID, 
                                pStockItem.UnitMeasure, 
                                (proj.ModifierGroupID || pStockItem.GroupIdOfItemModifiers)
                                 // 2025_09_18__10_45_32_4255 commented
                                //null,
                                //{
                                //    createVoidEntries: true,
                                //    UnitMeasure: pStockItem.UnitMeasure,
                                //    UnitQty: pStockItem.UnitQty,
                                //    Qty: pStockItem.Qty,
                                //    UnitPrice: pStockItem.UnitPrice
                                //}
                                );
                        }
                        pQuantityPrinted = CDec2(pQuantityPrinted);
                        orderedqty = CDec2(pStockItem.Qty);
                        if (proj.PosMode) {

                            pIU.Add('Quantity', CDec2(pStockItem.Qty));
                            if (orderedQty.length == undefined) {
                                pIU.Add('OrderedQuantity', CDec2(pStockItem.Qty)); //save ordered qty same as invoiced qty for POS. so ordered = invoiced
                            } else {
                                if (orderedQty.Rows.length > 0) {
                                    orderedqty = orderedQty.Rows[0].OrderedQuantity
                                    pIU.Add('OrderedQuantity', orderedQty.Rows[0].OrderedQuantity);
                                } else {
                                    pIU.Add('OrderedQuantity', CDec2(pStockItem.Qty)); //save ordered qty same as invoiced qty for POS. so ordered = invoiced
                                }
                            }
                        }
                        else {
                            pIU.Add('Quantity', 0); // need to save it as 0 because it's not yet invoiced!
                            pIU.Add('OrderedQuantity', CDec2(pStockItem.Qty));
                        }

                        ///////////////////
                        pID = NewID();
                        pIU.Add('ID', ReadyString(pID));
                        pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                        pIU.Add('ProductID', ReadyString(pProductID));
                        //                        pIU.Add('Quantity', CDec2(pStockItem.Qty)); //commented by janine see above block
                        pIU.Add('ActualQuantity', CDec2(pStockItem.UnitQty));
                        //if (pDrPrice.UnitMeasure)
                        //    pIU.Add('UnitMeasure', ReadyString(pDrPrice.UnitMeasure));
                        //else
                        pIU.Add('UnitMeasure', ReadyString(pStockItem.UnitMeasure));
                        // pIU.Add('UnitPrice', CDec2(pDrPrice.DiscountedPrice));
                        //if (pStockItem.UnitPrice == undefined)
                        //    pIU.Add('UnitPrice', CDec2(pDrPrice.DiscountedPrice));
                        //else
                        pIU.Add('UnitPrice', CDec2(pStockItem.UnitPrice));
                        pIU.Add('OriginalUnitPrice', CDec2(pStockItem.UnitPrice));
                        pCost = Business.LibraryPIMProduct.GetLandedCost(pProductID, sessionStorage.OrgID);
                        if (pCost != undefined)
                            pIU.Add('Cost', pCost);
                        else
                            pIU.Add('Cost', 'null');
                        pIU.Add('PriceLevelID', ReadyString(pStockItem.PriceLevelID));
                        pIU.Add('QuantityPrinted', pQuantityPrinted);
                        // pIU.Add('Subtotal', CDec2(pStockItem.Qty) * CDec2(pDrPrice.DiscountedPrice));
                        pIU.Add('Subtotal', (CDec2(pStockItem.Qty)) * CDec2(pIU.GetItem('UnitPrice')) + (CDec2(pStockItem.TotalModifier)));

                        pIU.Add('IsPromoItem', pStockItem.IsPromoItem);
                        pIU.Add('TaxPercent', 12);

                        if (CStr2(vData.IsVATExempt) == '1') {
                            pIU.Add('VATableSale', '0'); //additional by janine feb 19 2015
                            pIU.Add('VATExemptSale', pIU.GetItem('Subtotal'));
                            pIU.Add('NonVATSale', '0');
                            pIU.Add('ZeroRatedSale', '0');
                            pIU.Add('TaxSales', '0');
                            pIU.Add('TaxAmount', '0');
                        } else {
                            pIU.Add('VATableSale', pIU.GetItem('Subtotal')); //additional by janine feb 19 2015
                            pIU.Add('VATExemptSale', '0');
                            pIU.Add('NonVATSale', '0');
                            pIU.Add('ZeroRatedSale', '0');
                            pIU.Add('TaxSales', pIU.GetItem('Subtotal') / (1 + pIU.GetItem('TaxPercent') / 100));   // todo: round down to nearest 4 decimal
                            pIU.Add('TaxAmount', pIU.GetItem('Subtotal') - pIU.GetItem('TaxSales'));
                        }

                        //if (CBool(pDtFowst.Rows[0].IsVat)) {
                            
                        //} else {
                        //    pIU.Add('NonVATSale', pIU.GetItem('Subtotal'));
                        //}                        

                        if (CDec2(pStockItem.RequestedUnitPrice) > 0)
                            pIU.Add('RequestedUnitPrice', CDec2(pStockItem.RequestedUnitPrice));
                        pIU.Add('LineNumber', Business.TransInvoice.GetMaxLineNumber(pInvoiceID) + 1);
                        if (proj.ModifierGroupID || pStockItem.GroupIdOfItemModifiers)
                            pIU.Add('ModifierGroupID', ReadyString(proj.ModifierGroupID || pStockItem.GroupIdOfItemModifiers));


                        pIU.Add('IsOrderSlipPrinted', pStockItem.IsOrderSlipPrinted||0);

                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql);

                        if (proj.PosMode) {
                            Business.TransInvoice.UpdateItemsModifier(pID, orderedqty, pStockItem.ItemModifier);
                        }
                    };
                    pSql = 'DELETE FROM TransInvoiceItems' +
                        ' WHERE IFNULL(Quantity, 0) = 0' +
                        ' AND IFNULL(OrderedQuantity, 0) = 0' + 
                        ' AND IFNULL(QuantityPrinted, 0) = 0';
                    Sql.ExecuteSql(pSql);
                    Business.TransInvoice.UpdateTotalAmount(pInvoiceID);
                    Business.TransInvoice.UpdateItemCount(pInvoiceID);

                    //Sales Rep
                    //// delete sales rep
                    //pSql = "SELECT ID\n" +
                    //    ' FROM TransInvoiceItems\n' +
                    //    ' WHERE InvoiceID = ' + ReadyString(pInvoiceID) +
                    //    ' AND ProductID = ' + ReadyString(pProductID);
                    //if (proj.ModifierGroupID)
                    //    pSql += ' AND ModifierGroupID = ' + ReadyString(proj.ModifierGroupID);
                    //pSql = "DELETE FROM private.TransInvoiceItemSalesReps WHERE InvoiceID = " + ReadyString(pInvoiceID) +
                    //    ' AND InvoiceItemID IN \n' +
                    //    '(' + pSql + ')';
                    //pSql = Sql.ExecuteSql(pSql);

                    if (vData.SalesRep != undefined) {
                        pIU = new InsertUpdate('private.TransInvoiceItemSalesReps');
                        for (var pSalesRepIndex = 0; pSalesRepIndex <= vData.SalesRep.length - 1; pSalesRepIndex++) {
                            pSalesRep = vData.SalesRep[pSalesRepIndex];

                            pIU.Add('ID', ReadyString(NewID()));
                            pIU.Add('PeopleID', ReadyString(pSalesRep.SalesRepID));
                            pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                            pIU.Add('ProductID', ReadyString(pSalesRep.ProductID));
                            pIU.Add('InvoiceItemID', ReadyString(pID));

                            if (pSalesRep.RecState == 'Remove') {
                                pSql = 'DELETE FROM private.TransInvoiceItemSalesReps WHERE PeopleID = ' + ReadyString(pSalesRep.SalesRepID) +
                                     ' AND InvoiceID = ' + ReadyString(pInvoiceID);
                            } else {
                                pSql = pIU.GetInsert();
                            }
                            
                            Sql.ExecuteSql(pSql);
                        };
                    } // END if (vData.SalesRep != undefined) {

                    //Product Serial
                    pSql = "DELETE FROM private.TransInvoiceItemSerials WHERE InvoiceID = " + ReadyString(pInvoiceID) + " AND ProductID = " + ReadyString(pProductID);
                    Sql.ExecuteSql(pSql);

                    if (vData.ProductSerial != undefined) {
                        pIU = new InsertUpdate('private.TransInvoiceItemSerials');
                        for (var pProductSerialIndex = 0; pProductSerialIndex <= vData.ProductSerial.length - 1; pProductSerialIndex++) {

                            if (vData.ProductSerial[pProductSerialIndex].RecState != 'Remove') {
                                pProductSerial = vData.ProductSerial[pProductSerialIndex];

                                pIU.Add('ID', ReadyString(NewID()));
                                pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                                pIU.Add('ProductID', ReadyString(pProductSerial.ProductID));
                                pIU.Add('SerialNumber', ReadyString(pProductSerial.ProductSerial));
                                pSql = pIU.GetInsert();
                                Sql.ExecuteSql(pSql);
                            }
                        };
                    } // END if (vData.ProductSerial != undefined) {
                    //end of additional

                    SaveModifiers(proj.ModifierGroupID || proj.GroupIdOfItemModifiers);

                    if (vData.UpdateErrorCorrectAmount
                        && (GetStringObject('Rows.length', orderedQty) > 0)) { // dont use this syntax: orderedQty?.Rows?.length . kay mag error sa ubang android 

                        var pNewInvoiceAmount = Business.TransInvoice.GetTotalAmount(pInvoiceID);
                        var pErrorCorrectAmount = Math.abs(pOldInvoiceAmount - pNewInvoiceAmount);
                        if (pErrorCorrectAmount > 0) {
                            pIU = new InsertUpdate('TransInvoice');
                            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                            var pNewInvoiceAmount = Business.TransInvoice.GetTotalAmount(pInvoiceID);
                            pIU.Add('ErrorCorrectAmount', 'IFNULL(ErrorCorrectAmount, 0) + ' + pErrorCorrectAmount);
                            pIU.Add('ErrorCorrectCount', 'IFNULL(ErrorCorrectCount, 0) + 1');
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(pInvoiceID));
                            Sql.ExecuteSql(pSql);
                        }
                    }

                    Sql.CommitTransaction();

                    pOut = {};
                    pOut.Result = 'ok';
                    pOut.OrderID = pInvoiceID;

                    if (pStockItem != null)
                        pOut.DiscountedPrice = pStockItem.UnitPrice;
                    pOut.TransNum = pTransNum;
                    return pOut;
                } catch (e) {
                    try {
                        Sql.RollbackTransaction();
                    } catch (e) {
                    };
                    MessageBox(e.message);
                };

                function SaveModifiers(vModifierGroupID) {
                    var pIU;
                    var pSql;

                    if (proj.RebateData == null)
                        return false;

                    pIU = new InsertUpdate('TransInvoiceItemsRebates');
                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('ModifierGroupID', ReadyString(vModifierGroupID));
                    pIU.Add('RebateID', ReadyString(proj.RebateData.ID));
                    pIU.Add('ProductID', ReadyString(proj.RebateData.ProductID));
                    pIU.Add('Amount', CDec2(proj.RebateData.Amount));
                    pIU.Add('UnitMeasure', ReadyString(proj.RebateData.UnitMeasure));
                    pIU.Add('UnitQuantity', CDec2(proj.RebateData.UnitQuantity));
                    pIU.Add('Quantity', CDec2(proj.RebateData.Quantity));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql);

                    return true;
                } // END function SaveModifiers() {
            }; // UpdatePos

            this.GetAllowFreeTypeAPrice = function () {

                var pDT;

                try {
                    if (mAllowFreeTypeAPrice == undefined) {
                        //                        pDT = security_offline().CheckSecurityCurrentUserAcccess({
                        //                            OrgID: Web.OrgID(),
                        //                            UserID: Web.PeopleUserID(),
                        //                            ModuleName: 'Allow to Free Type a Price'
                        //                        });
                        //user will be allowed to free type a price if at least one of the ff is true:
                        mAllowFreeTypeAPrice = GetAllowFreeTypeDiscountedPrice() || GetAllowFreeTypeADiscountPercentage() || GetAllowFreeTypeADiscountAmount();
                    }
                    return mAllowFreeTypeAPrice;
                } catch (e) {
                    Android.Toast('jsUYKR75XHLP! An error occured while GetAllowFreeTypeAPrice: ' + e.message);
                }

            };

            this.GetAllowFreeTypeDiscountedPrice = function () {

                var pDT;

                try {
                    if (mAllowFreeTypeDiscountedPrice == undefined) {
                        pDT = security_offline().CheckSecurityCurrentUserAcccess({
                            OrgID: Web.OrgID(),
                            UserID: Web.PeopleUserID(),
                            ModuleName: 'Allow to Free Type Discounted Price'
                        });
                        mAllowFreeTypeDiscountedPrice = pDT.Rows.length > 0;
                    }
                    return mAllowFreeTypeDiscountedPrice;
                } catch (e) {
                    Android.Toast('jsUYKR75XHL1! An error occured while GetAllowFreeTypeDiscountedPrice: ' + e.message);
                }

            };
            this.GetAllowFreeTypeADiscountPercentage = function () {

                var pDT;

                try {
                    if (mAllowFreeTypeADiscountPercentage == undefined) {
                        pDT = security_offline().CheckSecurityCurrentUserAcccess({
                            OrgID: Web.OrgID(),
                            UserID: Web.PeopleUserID(),
                            ModuleName: 'Allow to Free Type a Discount Percentage'
                        });
                        mAllowFreeTypeADiscountPercentage = pDT.Rows.length > 0;
                    }
                    return mAllowFreeTypeADiscountPercentage;
                } catch (e) {
                    Android.Toast('jsUYKR75XHL2! An error occured while GetAllowFreeTypeADiscountPercentage: ' + e.message);
                }

            };
            this.GetAllowFreeTypeADiscountAmount = function () {

                var pDT;

                try {
                    if (mAllowFreeTypeADiscountAmount == undefined) {
                        pDT = security_offline().CheckSecurityCurrentUserAcccess({
                            OrgID: Web.OrgID(),
                            UserID: Web.PeopleUserID(),
                            ModuleName: 'Allow to Free Type a Discount Amount'
                        });
                        mAllowFreeTypeADiscountAmount = pDT.Rows.length > 0;
                    }
                    return mAllowFreeTypeADiscountAmount;
                } catch (e) {
                    Android.Toast('jsUYKR75XHL3! An error occured while GetAllowFreeTypeADiscountAmount: ' + e.message);
                }

            };
            this.GetAllowToSellBelowCost = function () {

                var pDT;

                try {
                    if (mAllowToSellBelowCost == undefined) {
                        pDT = security_offline().CheckSecurityCurrentUserAcccess({
                            OrgID: Web.OrgID(),
                            UserID: Web.PeopleUserID(),
                            ModuleName: 'Allow to Sell Below Cost'
                        });
                        mAllowToSellBelowCost = pDT.Rows.length > 0;
                    }
                    return mAllowToSellBelowCost;
                } catch (e) {
                    Android.Toast('jsUYKR75XHLP! An error occured while GetAllowToSellBelowCost: ' + e.message);
                }

            };

            //additional by janine sept 8 2014 for sales order taker
            //update the ordered qty to invoice quantity
            this.UpdateOrderedQuantity = function (invoiceid) {

                var pIU;
                var pSql;
                var pResult;
                var pDT;

                try {
                    pSql = 'SELECT ID FROM private.TransInvoiceItems WHERE InvoiceID = ' + ReadyString(invoiceid);
                    pDT = Sql.Execute(pSql);

                    //if in offline
                    if (pDT.Rows.length > 0) {

                        pSql = 'SELECT ID, OrderedQuantity\n' +
                            ' FROM private.TransInvoiceItems\n' + 
                            ' WHERE OrderedQuantity > 0\n' +
                            ' AND Quantity IS NULL\n' + 
                            ' AND InvoiceID = ' + ReadyString(invoiceid);
                        pDT = Sql.Execute(pSql);

                        if (pDT.Rows.length > 0) {
                            for (var i = 0; i <= pDT.Rows.length - 1; i++) {

                                pIU = new InsertUpdate('TransInvoiceItems');
                                pIU.Add('Quantity', pDT.Rows[i].OrderedQuantity);
                                pSql = pIU.GetUpdate('ID = ' + ReadyString(pDT.Rows[i].ID));
                                Sql.ExecuteSql(pSql);

                            }
                        }
                    } else {
                        //if in server
                        var pData = {};
                        pData.InvoiceID = invoiceid;

                        AjaxSend({
                            url: 'SalesMenu/UpdateOrderedQuantity',
                            data: pData,
                            success: function (vData) {

                            },
                            error: function (vError) {
                                MessageBox(vError);
                            }
                        });
                    }


                    return { Result: 'ok' };
                }
                catch (e) {
                    return { Result: e.Message };
                }



            }; // END this.UpdateOrderedQuantity

            // additional
            return this;

        }; // Stock_offline

    });