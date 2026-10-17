define(['common',
    GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
    GetPathFormat('/Scripts3/Browser/TransLastestOrderedStockByCustomer.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/Browser/TransSalesOrder.js?crc='),
    'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    'promise!/Scripts3/Browser/LibraryCustomerFowst.js?crc=',
    'business',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
    '/Scripts3/Stock_offline.js?crc=',
    '/Scripts3/controller/MasterFrame.js?crc=',
    'promise!/Scripts3/NewMessageBox.js?crc=',
    (OfflineMode ? '/Scripts/sources/MallLessorMegaworld/RobinsonsReport_offline.js' : null)
    // 'promise!/Scripts3/controller/PurchaseOrder.js?crc=' // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
    ], function () {
    
        var mInitialized;
        var mButtonData;
        var mThis;

        if (window.SalesMenu_offline == undefined) {
            window.SalesMenu_offline = function () {

                if (mThis != null)
                    return mThis;
                mThis = this;
                var mMessageBox; // set after saving; get after exporting
                var mZReadMessageBox;
                var defaultPriceLevel = '01000012-0005-0000-0000-000000000000';
                var mPurchaseOrder; // App.controller.PurchaseOrder
                mThis.HasCorrectDate = false;

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

                (function () {

                    if (mInitialized)
                        return;
                    else
                        mInitialized = true;

                    Device.CheckServerDate();
                    if (Web.HandleOrganization() == false)
                        return;

                    $($('[data-role=content]')[0]).hide();
                    Business.TransInvoice.HandleCorrectDate({
                        Success: function () {
                            mThis.HasCorrectDate = true;
                            $($('[data-role=content]')[0]).show();
                            console.log('js9LKZ9BADF5! checking for App.controller.MasterFrame.hasBackgroundIframe');
                            if (App.controller.MasterFrame.hasBackgroundIframe()) {
                                console.log('jsUC6QMJUE8Y! checking for App.controller.MasterFrame.hasBackgroundIframe is true');
                                App.controller.MasterFrame.runExporter();
                            } else {
                                console.log('js4Q94W86QSE! checking for App.controller.MasterFrame.hasBackgroundIframe is false');

                                // may 25, 2015: dont auto export here. auto export when idle for 2 minutes
                                //SalesPaymentsExport_offline().Export({
                                //    OnUploaded: function () {
                                //        //                                    FollowUp();
                                //    },
                                //    OnDone: function (vData) {
                                //        if (vData.Result == 'ok')
                                //            Android.AddLog('Done exporting js2CQHQ5B4QA');
                                //        else if (vData.Result == 'error')
                                //            Android.Toast(vData.Error);
                                //        App.Deferred.fulfill('KWXCETE5XN');
                                //    }
                                //}); // END SalesPaymentsExport_offline().Export({
                            }
                        } // END Success
                    }); // END Business.TransInvoice.HandleCorrectDate

                    proj.SaveCart = function (vParams) {

                        vParams = $.extend({
                            ForceSaveHeader: false,
                            UpdateErrorCorrectAmount: false
                        }, vParams);
                        var pData = {};
                        /*
                        vData = "{
                        "StockID": "cd975aad-eed6-4f04-b18c-928cb933f8b1",
                        "CustomerID": "cd71a0f7-08a9-4601-876e-5f13c064f873",
                        "FowstID": "fe639d1f-cd83-4b3e-8b0a-586731f479cb",
                        "ProductID": "cd975aad-eed6-4f04-b18c-928cb933f8b1",
                        "Stocks": [
                        {
                        "Qty": 1,
                        "UnitQty": 1,
                        "UnitPrice": 150,
                        "PriceLevelID": "01000012-0005-0000-0000-000000000000"
                        }
                        ],
                        "SalesRep": [],
                        "ProductSerial": [],
                        "Remarks": "",
                        "IsTestOnly": 0,
                        "PosMode": "pos",
                        "DeliveryMethod": "Dine In",
                        "OrderMethod": "Walk In",
                        "Gender": "NA",
                        "Age": "Mixed Ages/ NA",
                        "IsVATExempt": 0,
                        "CustomerTableNo": "TBL_1",
                        "OrderID": "ad36711f-870c-431e-a7ed-5d6c791fb4cd"
                        }"
                        */

                        pData.CustomerID = sessionStorage.CustomerOrganizationID;
                        pData.NewCartMode = localStorage.NewCartMode;
                        pData.FowstID = sessionStorage.FowstID;
                        pData.SalesRep = [];
                        pData.ProductSerial = [];
                        pData.Remarks = '';
                        pData.IsTestOnly = 0; //this means, it's not a test! additional june 2 by janine
                        pData.PosMode = proj.PosMode ? "pos" : "so"; //additional by janine sept 5 2014 for sales order taker
                        //additional customer demographic oct 23
                        pData.DeliveryMethod = sessionStorage.DeliveryMethod;
                        pData.OrderMethod = sessionStorage.OrderMethod;
                        pData.Gender = sessionStorage.Gender;
                        pData.Age = sessionStorage.Age;
                        //additonal for sm coin VAT EXEMPT
                        pData.IsVATExempt = 0;
                        pData.SupplierID = sessionStorage.SupplierOrganizationID;
                        pData.IsPurchaseOrder = CBool(proj.POMode);

                        if (CStr2(sessionStorage.CustomerTableNo) == '') {
                            sessionStorage.CustomerTableNo = CStr2(GetUrlParameters().TableNo);
                        }
                        if (CStr2(sessionStorage.CustomerTableNo) != '' || CStr2(sessionStorage.GasPumpNo) != '') {
                            if (CStr2(sessionStorage.GasPumpNo) != '')
                                pData.CustomerTableNo = 'GP_' + sessionStorage.GasPumpNo;

                            if (CStr2(sessionStorage.CustomerTableNo) != '')
                                pData.CustomerTableNo = 'TBL_' + sessionStorage.CustomerTableNo;
                        }

                        if (sessionStorage.CartOrderID != undefined) {
                            pData.OrderID = sessionStorage.CartOrderID;
                        }

                        if (sessionStorage.TotalNumberOfCustomer == undefined) {
                            pData.TotalNumberOfCustomer = 1;
                        } else {
                            pData.TotalNumberOfCustomer = sessionStorage.TotalNumberOfCustomer;
                        }

                        var pItem;
                        var pStock;
                        var pStock_offline = Stock_offline();
                        var pResult;
                        var pSalesRep;
                        var pAllSalesRep;

                        proj.ModifierGroupID = null;
                        var arrmodifierids = JSON.parse(JSON.stringify(proj.ItemsInCart) || "[]");
                        for (var pIndex in proj.CartStock) {
                            if (proj.CartStock.hasOwnProperty(pIndex) == false) 
                                continue;

                            pData.Stocks = [];
                            pItem = proj.CartStock[pIndex];
                            pData.StockID = pItem.ProductID;
                            pData.ProductID = pItem.ProductID;

                            var totalModifier = 0;
                            var mIsPremiumPriceApplied = false;

                            if (pItem.ItemModifier != undefined) {
                                for (var i = 0; i < pItem.ItemModifier.length; i++) {
                                    for (var m = 0; m < pItem.ItemModifier[i].SelectedModifier.length; m++) {
                                        if (pItem.ItemModifier[i].Type == 'Inventory Affecting') {
                                            if (CDec2(pItem.ItemModifier[i].SelectedModifier[m].PremiumPrice > 0) && !mIsPremiumPriceApplied) {
                                                totalModifier += (CDec2(pItem.ItemModifier[i].SelectedModifier[m].OrderQty) * CDec2(pItem.ItemModifier[i].SelectedModifier[m].UnitPrice)) + (CDec2(pItem.Quantity) * CDec2(pItem.ItemModifier[i].SelectedModifier[m].PremiumPrice));
                                                pItem.ItemModifier[i].SelectedModifier[m].PremiumPriceApplied = true;
                                                mIsPremiumPriceApplied = true
                                            }
                                            else {
                                                totalModifier += CDec2(pItem.ItemModifier[i].SelectedModifier[m].OrderQty) * CDec2(pItem.ItemModifier[i].SelectedModifier[m].UnitPrice);
                                                pItem.ItemModifier[i].SelectedModifier[m].PremiumPriceApplied = false;
                                            }                                            
                                        }
                                    }
                                }
                            }

                            pStock = {
                                IsPromoItem: CBool(pItem.IsPromoItem),
                                Qty: pItem.Quantity,
                                UnitQty: pItem.ActualQuantity,
                                UnitPrice: pItem.DiscountedPrice || pItem.UnitPrice,
                                PriceLevelID: defaultPriceLevel,
                                UnitMeasure: pItem.UnitMeasure,
                                IsOrderSlipPrinted: pItem.IsOrderSlipPrinted,
                                ItemModifier: pItem.ItemModifier,
                                GroupIdOfItemModifiers: pItem.GroupIdOfItemModifiers,
                                TotalModifier: totalModifier
                            };
                            //override the existing item details of the same sku and item modifiers
                            //for (var i = 0; i < arrmodifierids.length; i++){
                            //    var pDrItemsInCart = arrmodifierids[i];
                            //    if ((ReadyGuid(pDrItemsInCart.ProductID) == ReadyGuid(pItem.ProductID))
                            //        && (pDrItemsInCart.UnitMeasure == pItem.UnitMeasure)) {

                            //        if ((arrmodifierids[i].SelectedItemModifierID).join(',') == (pItem.SelectedItemModifierID || []).join(',')) {
                            //            pStock.ItemModifier = pItem.ItemModifier;
                            //            pStock.GroupIdOfItemModifiers = arrmodifierids[i].GroupIdOfItemModifiers;
                            //            break;
                            //        }
                            //    }
                            //}
                            pData.Stocks.push(pStock);
                            pData.SalesRep = [];
                            if (window.SalesMenuSalesRepresentatives != null) {
                                pAllSalesRep = window.SalesMenuSalesRepresentatives.GetData();
                                for (var pSalesRepIndex in pAllSalesRep) {
                                    if (pAllSalesRep.hasOwnProperty(pSalesRepIndex)) {
                                        pSalesRep = pAllSalesRep[pSalesRepIndex];
                                        pData.SalesRep.push({
                                            SalesRepID: pSalesRep.ID,
                                            ProductID: pData.ProductID
                                        });
                                    }
                                }
                            }
                            pData.DontSaveHeader = (pIndex != 0);
                            pData.UpdateErrorCorrectAmount = vParams.UpdateErrorCorrectAmount;
                            pResult = pStock_offline.UpdatePos(pData);
                            sessionStorage.CartOrderID = pResult.OrderID;
                            localStorage.NewCartMode = false;
                            pData.NewCartMode = false;
                            pData.OrderID = pResult.OrderID;
                        }
                        if (vParams.ForceSaveHeader
                            && (pResult == null)) {

                            pData.Stocks = [];
                            pResult = pStock_offline.UpdatePos(pData);
                        }
                        proj.CartStock = [];
                        //if (window.InAndroid)
                            //Android.ScriptToWebView({
                            //    Script: 'proj.ReloadTableView()',
                            //    WebViewIndex: 1
                            //});
                    }; // END proj.SaveCart

                    HandleUnflushedReminder();
                })(); // END (function() {

                function HandleUnflushedReminder() {
                    var pEveryXMinutes = Business.SystemDefaults.GetSystemDefault('UnflushedReminderMinutes');
                    pEveryXMinutes = CInt2(pEveryXMinutes);
                    if (pEveryXMinutes <= 0) 
                        return;
                    var pMessage = Business.SystemDefaults.GetSystemDefault('UnflushedReminderMessage');
                    if (CStr2(pMessage).trim() == '')
                        pMessage = 'This is friendly remainder to flush unflushed data.';
                    // get next trigger
                    var pStartingMinute = 17 * 60; // 8:00am
                    var pCurrentDateTime = new Date();
                    var pNoSeconds = new Date(pCurrentDateTime.getFullYear(), pCurrentDateTime.getMonth(), pCurrentDateTime.getDate(), pCurrentDateTime.getHours(), pCurrentDateTime.getMinutes());
                    var pDateOnly = RemoveTime(pNoSeconds);
                    var pCurrentMinute = (pNoSeconds - pDateOnly) / 1000 / 60;
                    pCurrentMinute = Math.floor(pCurrentMinute)
                    if (pCurrentMinute < pStartingMinute)
                        pCurrentMinute += 24 * 60;
                    var pRemainingMinutes = pCurrentMinute - pStartingMinute;
                    pRemainingMinutes %= pEveryXMinutes; // get the remainder
                    pRemainingMinutes = pEveryXMinutes - pRemainingMinutes;
                    if (pRemainingMinutes == 0)
                        pRemainingMinutes = pEveryXMinutes;
                    var pTargetTime = new Date(pNoSeconds.getTime() + pRemainingMinutes * 60 * 1000);
                    console.log('pTargetTime: ' + pTargetTime);
                    var pRemainingMilliseconds = pTargetTime - pCurrentDateTime;
                    setTimeout(function() {
                        if (SalesPaymentsExport_offline().HasUnflushedRecords()) {
                            console.log('alerting ' + new Date());
                            Android.Alert(pMessage, 'Reminder');
                        }
                        HandleUnflushedReminder();
                    }, pRemainingMilliseconds);
                }

                this.GetSelectedFowstDefaultHBTemplate = function () {
                    var pSql;
                    var hbtemplateid;
                    if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                        hbtemplateid = Business.SystemDefaults.GetSystemDefault('ProductMakerDefaultHotButtonTemplateID');
                    }
                    if (!IsGuid(hbtemplateid)) {
                        pSql = "SELECT DefaultHotButtonTemplateID FROM LibraryOrganizationFOWST WHERE ID = " + ReadyString(sessionStorage.FowstID);
                        var pDT = Sql.Execute(pSql);
                        hbtemplateid = CStr2(pDT.Rows[0].DefaultHotButtonTemplateID);
                    }

                    if (hbtemplateid != '') {
                        pSql = 'SELECT pim.ID' + '\n' +
                                ',pim.Name, pim.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname' + '\n' +
                                ' FROM private.LibraryPimMenu pim LEFT JOIN LibraryPeople ppl' + '\n' +
                                ' ON pim.UserID = ppl.ID WHERE pim.IsActive = 1 AND pim.ID = ' + ReadyString(hbtemplateid);
                        return Sql.Execute(pSql);
                    } else {
                        return [];
                    }
                }

                this.GetUser = function () {
                    // returns:
                    //  if FowstID == null, no valid fowst. so, show message then return to people menu
                    //  if FowstCount > 0, show fowst browser
                    var pSql;
                    var pPeopleUserID = Android.GetSessionData().PeopleUserID;
                    var pOut = {};
                    var pFowst;

                    if (pPeopleUserID == undefined) {
                        proj.PromptLogin();
                        return null;
                    };
                    pSql = 'SELECT FirstName' +
                    ' FROM LibraryPeople' +
                    ' WHERE ID = ' + ReadyString(pPeopleUserID);
                    var pDT = Sql.Execute(pSql);
                    if (pDT == undefined) return null;
                    if (pDT.Rows.length == 0) return null;

                    pOut.FirstName = pDT.Rows[0].FirstName;
                    pOut.IsSalesAgent = 1;
                    pOut.FowstID = null;
                    //                pOut.OrgID = GetOrg();
                    pFowst = SalesMenu_offline().GetSoloFowst(sessionStorage.FowstID);
                    if (pFowst != null) {
                        pOut.FowstID = pFowst.ID;
                        pOut.FowstName = pFowst.Name;
                        pOut.IsFOWSTInsideAMall = pFowst.IsFOWSTInsideAMall;
                        pOut.FOWSTMallName = pFowst.FOWSTMallName;
                        pOut.Operates247 = pFowst.Operates247;
                        pOut.FowstCount = pFowst.FowstCount;
                        pOut.DataImportationPort = pFowst.DataImportationPort;
                        pOut.EnableHotButtonsContextualMenu = pFowst.EnableHotButtonsContextualMenu;
                        pOut.Address = pFowst.Address;
                        pOut.TIN = pFowst.Tin;
                        pOut.DefaultPosView = pFowst.DefaultPosView;
                    };

                    Android.UpdateSessionData({
                        OrgID: pOut.OrgID,
                        FowstID: pOut.FowstID
                    });
                    pOut.AllowPriceAdjustment = Business.SystemDefaults.AllowPriceAdjustment();

                    return pOut;

                }; // this.GetUser = function() {

                this.GetSoloFowst = function (vFowstID) {
                    // PARAMS:
                    //  vFowstID: optional. if set, filter by this FowstID.

                    var pOrgID;
                    var pPeopleUserID;
                    var pDT;

                    pOrgID = Web.OrgID();
                    if (pOrgID == undefined)
                        throw ('GetSoloFowst: OrgID must not be null');

                    pPeopleUserID = Web.PeopleUserID();
                    if (pPeopleUserID == undefined)
                        throw ('GetSoloFowst: PeopleUserID must not be null');

                    if (vFowstID == undefined)
                        pDT = GetData();
                    else {
                        pDT = GetData(vFowstID);
                        if ((pDT == undefined)
                        || (pDT.Rows.length == 0))

                            pDT = GetData();
                    }
                    if (pDT == undefined) return null;
                    if (pDT.Rows.length == 0) return null;
                    if (pDT.Rows.length > 1)
                        return { FowstCount: pDT.Rows.length };

                    return {
                        ID: pDT.Rows[0].FowstID,
                        Name: pDT.Rows[0].FowstName,
                        DataImportationPort: pDT.Rows[0].DataImportationPort,
                        EnableHotButtonsContextualMenu: pDT.Rows[0].EnableHotButtonsContextualMenu,
                        Tin: pDT.Rows[0].TINBranchCode,
                        Address: pDT.Rows[0].Address,
                        DefaultPosView: pDT.Rows[0].DefaultPosView,
                        IsFOWSTInsideAMall: pDT.Rows[0].IsFOWSTInsideAMall,
                        FOWSTMallName: pDT.Rows[0].FOWSTMallName,
                        Operates247: pDT.Rows[0].Operates247
                    };

                    function GetData(vFowstID) {

                        var pSql;
                        var pDT;

                        pSql = ' SELECT DISTINCT LibraryDeviceFowst.DeviceID' + '\n' +
                        ' ,LibraryDeviceFowst.FowstID' + '\n' +
                        ' ,LibraryOrganizationFOWST.Name AS FowstName' + '\n' +
                        ' ,LibraryOrganizationFOWST.FOWSTMallName' + '\n' +
                        ' ,LibraryOrganizationFOWST.IsFOWSTInsideAMall' + '\n' +
                        ' ,LibraryOrganizationFOWST.Operates247' + '\n' +
                        ' ,LibraryOrganizationFOWST.DefaultPosView' + '\n' +
                        ' ,LibraryOrganizationFOWST.DataImportationPort' + '\n' +
                        ' ,LibraryOrganizationFOWST.EnableHotButtonsContextualMenu AS EnableHotButtonsContextualMenu' + '\n' +
                        ' ,IFNULL(LibraryOrganizationFOWST.StreetNumber, \'\') || \',\' || IFNULL(LibraryOrganizationFOWST.Street, \'\') || \',\' || IFNULL(cty.Name, \'\')  || \',\' || IFNULL(ctry.Name, \'\') as Address' +
                        ' ,LibraryOrganizationFOWST.TINBranchCode' + '\n' +
                        ' FROM LibraryDevice ' + '\n' +
                        ' JOIN LibraryDeviceFowst ON LibraryDeviceFowst.DeviceID = LibraryDevice.ID' + '\n' +
                        ' AND IFNULL(LibraryDeviceFowst.IsActive, 0) <> 0\n' +
                        ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID' + '\n' +
                        ' JOIN LibraryOrganizationEmployeeFOWST ON LibraryOrganizationEmployeeFOWST.FowstID = LibraryOrganizationFOWST.ID ' + '\n' +
                        ' LEFT JOIN LibraryCities AS cty ON LibraryOrganizationFOWST.FowstCityID = cty.ID ' + '\n' +
                        ' LEFT JOIN LibraryCountries AS ctry ON LibraryOrganizationFOWST.FowstCountryID = ctry.ID ' + '\n' +
                        ' WHERE LibraryDevice.UUID = ' + ReadyString(Web.DeviceSerial()) + '\n' +
                        ' AND LibraryOrganizationFOWST.OrganizationID = ' + ReadyString(pOrgID) + '\n' +
                        ' AND LibraryOrganizationEmployeeFOWST.PeopleID = ' + ReadyString(pPeopleUserID) + '\n';
                        if (IsGuid(vFowstID))
                            pSql += ' AND LibraryDeviceFowst.FowstID = ' + ReadyString(vFowstID);
                        pDT = Sql.Execute(pSql);

                        return pDT;

                    }

                }; // function GetFowstID

                //edited by janine may 14 12nn
                this.GetCustomer = function (customerID) {

                    var pOut;
                    var pCustomerID;

                    if (CStr2(customerID) == '') {
                        pCustomerID = Business.SystemDefaults.CustomerOrganizationID();

                        if (CStr2(pCustomerID) == '') return null;
                        pOut = Business.LibraryOrganization.ByID(pCustomerID);

                    } else {
                        pOut = Business.LibraryOrganization.ByID(customerID);
                    }

                    return pOut;
                }; // GetCustomer


                //edited by janine may 14 12nn
                this.getSupplier = function (fowstid) {
                    var pDT;
                    var pSql;

                    if (CStr2(sessionStorage["OrgID"]) == '')
                        return;
                    else {


                        pSql = "SELECT DISTINCT FranchisorOrgID as ID, (CASE " +
                                    "     WHEN LibraryOrganization.IsOrganization = 1 THEN LibraryOrganization.CompanyName " +
                                    "     WHEN LibraryOrganization.IsOrganization = 0 THEN " +
                                    "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonSurname) || ', ',', '), '') || " +
                                    "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonFirstName) || ' ',' '), '') || " +
                                    "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonMiddleName) || ' ',' '), '') || " +
                                    "         IFNULL(NULLIF(', ' || TRIM(LibraryOrganization.PersonSuffixName),', '), '') " +
                                    " END) as CompanyName, LibraryOrganizationFranchisee.IsPOExclusiveToPrincipal, " +
                                    " LibraryOrganizationFranchisee.POPriceLevel, LibraryPIMPriceLevel.PriceLevel FROM LibraryOrganizationFranchisee INNER JOIN " +
                                    " LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryOrganizationFranchisee.FranchiseeOrgFowstID " +
                                    " INNER JOIN LibraryOrganization ON LibraryOrganization.ID = LibraryOrganizationFranchisee.FranchisorOrgID " +
                                    " INNER JOIN LibraryPIMPriceLevel ON LibraryPIMPriceLevel.ID = LibraryOrganizationFranchisee.POPriceLevel" +
                                    " WHERE LibraryOrganizationFranchisee.IsFranchisorConfirmed = 1 AND " +
                                    " LibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND LibraryOrganizationFranchisee.IsActive= 1 " +
                                    " AND LibraryOrganizationFOWST.OrganizationID = " + ReadyString(sessionStorage["OrgID"]);

                        if (CStr2(fowstid) != '')
                            pSql += " AND LibraryOrganizationFranchisee.FranchiseeOrgFowstID = " + ReadyString(fowstid);


                        pDT = Sql.Execute(pSql);
                    }

                    return pDT;
                }; // GetSupplier

                //additional by janine june 29 10am
                this.getSupplierPriceLevel = function (orgID, fowstid) {
                    var pDT;
                    var pSql;

                    if (CStr2(fowstid) == '' || CStr2(orgID) == '')
                        return;
                    else {


                        pSql = "SELECT DISTINCT LibraryOrganizationFranchisee.POPriceLevel, LibraryPIMPriceLevel.PriceLevel FROM LibraryOrganizationFranchisee " +
                                    " INNER JOIN LibraryPIMPriceLevel ON LibraryPIMPriceLevel.ID = LibraryOrganizationFranchisee.POPriceLevel" +
                                    " WHERE LibraryOrganizationFranchisee.IsFranchisorConfirmed = 1 AND " +
                                    " LibraryOrganizationFranchisee.IsFranchiseeConfirmed= 1 AND LibraryOrganizationFranchisee.IsActive= 1 ";


                        pSql += " AND LibraryOrganizationFranchisee.FranchisorOrgID = " + ReadyString(orgID) + " AND LibraryOrganizationFranchisee.FranchiseeOrgFowstID = " + ReadyString(fowstid) + " ORDER BY LibraryOrganizationFranchisee.Lastupdated DESC LIMIT 1";


                        pDT = Sql.Execute(pSql);
                    }

                    return pDT;
                }; // GetSupplier
                /////////////////////////////
                ///////////ADDITIONAL BY JANINE FOR HOT BUTTONS
                //////////////////////////////
                this.getPimMenu = function (searchKey) {
                    var pOut = {};
                    var pDT;
                    var pSql;
                    if (searchKey == '') {
                        pSql = 'SELECT pim.ID' + '\n' +
                    ',pim.Name, pim.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname' + '\n' +
                    ' FROM private.LibraryPimMenu pim INNER JOIN LibraryPeople ppl' + '\n' +
                    ' ON pim.UserID = ppl.ID WHERE pim.IsActive = 1';
                    } else {
                        pSql = 'SELECT pim.ID' + '\n' +
                    ',pim.Name, pim.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname' + '\n' +
                    ' FROM private.LibraryPimMenu pim INNER JOIN LibraryPeople ppl' + '\n' +
                    ' ON pim.UserID = ppl.ID WHERE pim.IsActive = 1 AND pim.Name LIKE ' + ReadyStringWild(searchKey);
                    }

                    pSql += ' ORDER BY pim.Name';
                    pDT = Sql.Execute(pSql);

                    return pDT;


                }; // getPimMenu

                this.GetCustomerPriceLevel = function (orgId) {
                    var pOut = {};
                    var pDT;
                    var pSql;

                    if (CStr2(orgId) != '') {

                        pSql = 'SELECT org.PriceLevelID, pim.PriceLevel' + '\n' +
                    ' FROM private.LibraryOrganizationPriceLevel org INNER JOIN LibraryPIMPriceLevel pim' + '\n' +
                    ' ON org.PriceLevelID = pim.ID' + '\n' +
                    ' WHERE org.OrganizationID =' + ReadyString(orgId);
                    }

                    pSql += ' ORDER BY pim.PriceLevel';
                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };

                }; // GetCustomerPriceLevel

                this.GetSupplierPriceLevel = function (orgId) {
                    var pOut = {};
                    var pDT;
                    var pSql;

                    if (CStr2(orgId) != '') {

                        pSql = 'SELECT org.PriceLevelID, pim.PriceLevel' + '\n' +
                    ' FROM private.LibraryPIMProductPriceLevel org INNER JOIN LibraryPIMPriceLevel pim' + '\n' +
                    ' ON org.PriceLevelID = pim.ID' + '\n' +
                    ' WHERE org.OrganizationID =' + ReadyString(orgId);
                    }

                    pSql += ' ORDER BY pim.PriceLevel';
                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };

                }; // GetSupplierPriceLevel


                this.GetGroupChildren = function (id, priceLevelID, WithContentsOnly) {
                    var pOut = {};
                    var pDT;
                    var pSql;

                    if (priceLevelID == null)
                        priceLevelID = '01000012-0005-0000-0000-000000000000';


                    if (CStr2(id) != '') {
                        //to make sure that there is a price level. default to retail price

                        pSql = 'SELECT DISTINCT ID as ID, \'0\' as AllowDecimal, ID as ProductID, Name as ProductName' + '\n' +
                      ',Name as Name, \'Group\' as table_name, \'1\' as Quantity, \'0\' as DiscountedPrice, \'\' as UnitMeasure, ID as UnitMeasureID' + '\n';
                      pSql += ' ,0 AS HasModifier';
                      pSql += ' FROM private.LibraryPimGroups' + '\n' +
                      ' WHERE IsActive = 1 AND ParentID= ' + ReadyString(id) + '\n' +
                      ' UNION SELECT sku.ID as ID, orgpim.AllowDecimal as AllowDecimal, sku.ProductID as ProductID, pim.ProductName as ProductName' + '\n' +
                      ',sku.ShortName as Name, \'SKU\' as table_name, meas.Quantity as Quantity,' +
                      ' meas.Quantity * price.Price - (meas.Quantity * price.Price) * (CAST(meas.Discount AS Float) / 100) * (NOT price.DeactivateDiscount) AS DiscountedPrice,' +
                      ' LibraryCountriesLanguageLabel.Label as UnitMeasure, meas.UnitMeasureID as UnitMeasureID' + '\n';

                      pSql += ' ,(SELECT 1' + '\n' + 
                        '     FROM LibraryPIMProductVariableFeatures001' + '\n' + 
                        '     JOIN private.LibraryItemModifierCategories ON LibraryItemModifierCategories.CategoryName = LibraryPIMProductVariableFeatures001.C0020' + '\n' + 
                        '      WHERE LibraryPIMProductVariableFeatures001.ProductID = sku.ProductID' + '\n' + 
                        '      LIMIT 1' + '\n' + 
                        ' ) AS HasModifier';
                      pSql += ' FROM private.LibraryPimGroupPims sku LEFT JOIN LibraryPIMProduct pim ON sku.ProductID = pim.ID' + '\n' +
                        ' LEFT JOIN LibraryPIMProductMeasurement meas ON sku.ProductID = meas.ProductID' + '\n' +
                        ' LEFT JOIN LibraryPIMProductPriceLevel price ON sku.ProductID = price.ProductID' + '\n' +
                        ' LEFT JOIN LibraryPIMMeasure ON meas.UnitMeasureID = LibraryPIMMeasure.ID' + '\n' +
                        ' LEFT JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID' + '\n' +
                        ' LEFT JOIN LibraryOrganizationPIM orgpim ON pim.ID = orgpim.ProductID';
                        if (WithContentsOnly) {
                            pSql += ' JOIN LibraryPIMProductContents ON LibraryPIMProductContents.ParentProductID = pim.ID' +
                                ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(Web.OrgID());
                        }
                        pSql += ' WHERE price.OrganizationID= ' + ReadyString(Web.OrgID()) + ' AND sku.GroupID= ' + ReadyString(id) + ' AND meas.MeasurementLevel = 1 AND sku.IsActive = 1 AND price.PriceLevelID = ' + ReadyString(priceLevelID);
                    }

                    pSql += ' ORDER BY table_name, Name';
                    pDT = Sql.Execute(pSql);

                    return { 'Data': pDT };

                }; // GetGroupChildren

                this.GetHotButtonsData = function (menuID, vID, priceLevelID, WithContentsOnly, vBatchProductOnly) {
                    var pSql;
                    var pDT;
                    var pOut;

                    pOut = {};

                    if (sessionStorage.HotButtonData == null)
                        mButtonData = null;
                    if ((sessionStorage.HotButtonData != null) 
                        && (ReadyGuid(sessionStorage.getItem('HotButtonDataCacheTemplateID')) == ReadyGuid(menuID))
                        && (CBool(sessionStorage.getItem('HotButtonDataCacheWithContentsOnly')) == CBool(WithContentsOnly))
                        && (CBool(sessionStorage.getItem('HotButtonDataCacheBatchProductOnly')) == CBool(vBatchProductOnly))
                        && (ReadyGuid(sessionStorage.getItem('HotButtonDataCacheCustomerID')) == ReadyGuid(sessionStorage.CustomerOrganizationID))
                        ) {

                        mButtonData = JSON.parse(sessionStorage.HotButtonData);
                    } else
                        mButtonData = null;

                    if (mButtonData == null)
                        mButtonData = GetButtons(menuID, vID, priceLevelID, WithContentsOnly, vBatchProductOnly, sessionStorage.CustomerOrganizationID);

                    var availableHB = GetButtonsFromCache(menuID, vID);
                    var visibleHB = { Columns: availableHB.Columns, Rows: [] };

                    for (var i = 0 in availableHB.Rows) {
                        pSql = "SELECT * FROM LibraryPimMenuButtonAssignedStores WHERE HotButtonID = " + ReadyString(availableHB.Rows[i].ID);
                        if (Sql.Execute(pSql).Rows.length == 0) {
                            //if 0 rows meaning visible ang button sa tanan FOWST sa Org
                            visibleHB.Rows.push(availableHB.Rows[i]);
                        } else {
                            //if not, assigned sya sa specific stores
                            pSql += ' AND FowstID = ' + ReadyString(sessionStorage.FowstID);

                            if (Sql.Execute(pSql).Rows.length == 0) {
                                //if 0 rows meaning dili sya visible sa current FOWST and need iremove;
                                continue;
                            } else {
                                visibleHB.Rows.push(availableHB.Rows[i]);
                            }
                        }
                    }

                    pOut.Data = visibleHB;

                    //if (window.DebugMode && (vID == '0cd0cee4-331f-45d1-ab87-897dbde93a25'))
                    //    pOut.Data = {"ElapsedSeconds":4,"ElapsedMilliseconds":4080,"Columns":{"ID":{"Name":"ID"},"ProductID":{"Name":"ProductID"},"ProductName":{"Name":"ProductName"},"Name":{"Name":"Name"},"table_name":{"Name":"table_name"},"Quantity":{"Name":"Quantity"},"DiscountedPrice":{"Name":"DiscountedPrice"},"UnitMeasure":{"Name":"UnitMeasure"},"UnitMeasureID":{"Name":"UnitMeasureID"}},"Rows":[{"ID":"df882a48-7ea9-453a-95dd-c47156ee879b","ProductID":"b7282593-60b6-46ab-b745-b145dfb0abb1","ProductName":"1-Pc Chicken BBQ with Unli Rice","Name":"1-Pc Chicken BBQ with U","table_name":"SKU","Quantity":"1","DiscountedPrice":"80","UnitMeasure":"Order","UnitMeasureID":"a79de8d4-b121-494a-9d29-c095a51f64dc"},{"ID":"df882a48-7ea9-453a-95dd-c47156ee879b","ProductID":"b7282593-60b6-46ab-b745-b145dfb0abb1","ProductName":"1-Pc Chicken BBQ with Unli Rice","Name":"1-Pc Chicken BBQ with U","table_name":"SKU","Quantity":"1","DiscountedPrice":"81","UnitMeasure":"Order","UnitMeasureID":"a79de8d4-b121-494a-9d29-c095a51f64dc"},{"ID":"79fff30e-9d13-485f-baf9-92d48b6d55ae","ProductID":"3fb9a5d9-cfaf-4071-86e2-30fcb302e6c7","ProductName":"Chicken Ball Lugaw","Name":"Chicken Ball Lugaw","table_name":"SKU","Quantity":"1","DiscountedPrice":"67","UnitMeasure":"Order","UnitMeasureID":"a79de8d4-b121-494a-9d29-c095a51f64dc"},{"ID":"7416a643-38bf-4392-a88f-a592319be93b","ProductID":"e6bf853d-c92e-481f-889e-741e402cc6f2","ProductName":"Chicken Burger","Name":"Chicken Burger","table_name":"SKU","Quantity":"1","DiscountedPrice":"135","UnitMeasure":"Order","UnitMeasureID":"a79de8d4-b121-494a-9d29-c095a51f64dc"},{"ID":"c55d2ed1-84ff-4f78-be2b-0dd8ff0dcaf0","ProductID":"c34a21f5-25e1-427d-bf9c-9b84843b2e5a","ProductName":"Chicken Mami","Name":"Chicken Mami","table_name":"SKU","Quantity":"1","DiscountedPrice":"70","UnitMeasure":"Order","UnitMeasureID":"a79de8d4-b121-494a-9d29-c095a51f64dc"},{"ID":"c0bf5344-1138-4c8a-8106-3e201af3344c","ProductID":"a0bb1fa3-645b-45f5-8adc-b5f50843678b","ProductName":"Garlic Chicken","Name":"Garlic Chicken","table_name":"SKU","Quantity":"1","DiscountedPrice":"165","UnitMeasure":"Order","UnitMeasureID":"a79de8d4-b121-494a-9d29-c095a51f64dc"}]};
                    //else
                    //    pOut.Data = GetButtons(menuID, vID, priceLevelID, WithContentsOnly);

                    return pOut;

                    function GetButtons(menuID, vID, priceLevelID, WithContentsOnly, vBatchProductOnly, vCustomerID) {
                        //to make sure that there is a price level. default to retail price

                        if (priceLevelID == null)
                            priceLevelID = ReadyGuid(Business.SystemDefaults.GetSystemDefault("Retail Price ID"));

                        pSql = 'DROP TABLE IF EXISTS _ProductCategories';
                        Sql.ExecuteSql(pSql);

                        pSql = 'CREATE TABLE IF NOT EXISTS private._ProductCategories AS \n' +
                            ' SELECT MAX(LibraryItemModifierCategories.CategoryName) AS CategoryName' + '\n' + 
                            '      ,IFNULL(LibraryItemModifierCategories.ProductID, LibraryPIMProductVariableFeatures001.ProductID) AS ProductID' + '\n' + 
                            '      FROM private.LibraryItemModifierCategories' + '\n' +
                            '      INNER JOIN LibraryItemModifiers ON LibraryItemModifiers.ID = LibraryItemModifierCategories.ItemModifierID \n' +
                            '      LEFT JOIN LibraryPIMProductVariableFeatures001 ON LibraryItemModifierCategories.CategoryName = LibraryPIMProductVariableFeatures001.C0020' + '\n' +
                            '      WHERE LibraryItemModifiers.IsActive = 1 \n' +
                            ' GROUP BY IFNULL(LibraryItemModifierCategories.ProductID, LibraryPIMProductVariableFeatures001.ProductID)';

                        Sql.ExecuteSql(pSql);

                        pSql = 'DROP TABLE IF EXISTS private._HotButtonsPrice';
                        Sql.ExecuteSql(pSql);

                        // old
                        pSql = 'CREATE TABLE IF NOT EXISTS private._HotButtonsPrice AS \n' +
                            '      SELECT MIN(LibraryPIMProductPriceLevel.Price) AS Price' + '\n' + 
                            '      ,MIN(LibraryPIMProductPriceLevel.DeactivateDiscount) AS DeactivateDiscount' + '\n' + 
                            '      ,LibraryPIMProductPriceLevel.ProductID AS ProductID' + '\n' + 
                            '      ,LibraryPIMProductPriceLevel.PriceLevelID AS PriceLevelID' + '\n' + 
                            '      ,LibraryPIMProductPriceLevel.OrganizationID AS OrganizationID' + '\n' + 
                            '      FROM LibraryPIMProductPriceLevel' + '\n' + 
                                ' WHERE LibraryPIMProductPriceLevel.PriceLevelID = ' + ReadyString(priceLevelID) + '\n' +
                                ' AND LibraryPIMProductPriceLevel.OrganizationID= ' + ReadyString(Web.OrgID()) + '\n' + 
                            '      GROUP BY LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                            '      ,LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            '      ,LibraryPIMProductPriceLevel.OrganizationID';

                        // new
                        pSql = ' CREATE TABLE private._HotButtonsPrice AS' + '\n' + 
                            ' with cte as (' + '\n' + 
                            '  SELECT MIN(LibraryPIMProductPriceLevel.Price) AS Price' + '\n' + 
                            '       ,MIN(LibraryPIMProductPriceLevel.DeactivateDiscount) AS DeactivateDiscount' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.ProductID AS ProductID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID AS PriceLevelID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.OrganizationID AS OrganizationID' + '\n' + 
                            ' ,4 AS SortOrder' + '\n' + 
                            '       FROM LibraryPIMProductPriceLevel' + '\n' + 
                            '  WHERE  LibraryPIMProductPriceLevel.PriceLevelID = ' + ReadyString(priceLevelID) + '\n' +
                            ' AND LibraryPIMProductPriceLevel.OrganizationID= ' + ReadyString(Web.OrgID()) + '\n' + 
                            '       GROUP BY LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.OrganizationID' + '\n' + 
                            ' UNION ALL' + '\n' + 
                            '  SELECT MIN(LibraryPIMProductPriceLevel.Price) AS Price' + '\n' + 
                            '       ,MIN(LibraryPIMProductPriceLevel.DeactivateDiscount) AS DeactivateDiscount' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.ProductID AS ProductID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID AS PriceLevelID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.OrganizationID AS OrganizationID' + '\n' + 
                            ' ,3.1 AS SortOrder' + '\n' + 
                            '       FROM LibraryPIMProductPriceLevel' + '\n' + 
                            '  JOIN LibraryOrganizationFranchisee ON LibraryOrganizationFranchisee.DefaultSellingPriceLevel = LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            ' WHERE LibraryPIMProductPriceLevel.OrganizationID= ' + ReadyString(Web.OrgID()) + '\n' + 
                            ' and    LibraryOrganizationFranchisee.FranchiseeOrgFowstID = ' + ReadyString(sessionStorage.FowstID) + '\n' + 
                            ' AND LibraryOrganizationFranchisee.OwnershipType = \'Franchised\'\n' +
                            '       GROUP BY LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.OrganizationID' + '\n' +

                            ' UNION ALL' + '\n' + 
                            '  SELECT MIN(LibraryPIMProductPriceLevel.Price) AS Price' + '\n' + 
                            '       ,MIN(LibraryPIMProductPriceLevel.DeactivateDiscount) AS DeactivateDiscount' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.ProductID AS ProductID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID AS PriceLevelID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.OrganizationID AS OrganizationID' + '\n' + 
                            ' ,3 AS SortOrder' + '\n' + 
                            '       FROM LibraryPIMProductPriceLevel' + '\n' + 
                            '  JOIN LibraryOrganizationFranchisee ON LibraryOrganizationFranchisee.DefaultSellingPriceLevel = LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            ' WHERE LibraryPIMProductPriceLevel.OrganizationID= ' + ReadyString(Web.OrgID()) + '\n' + 
                            ' and    LibraryOrganizationFranchisee.FranchiseeOrgFowstID = ' + ReadyString(sessionStorage.FowstID) + '\n' + 
                            ' AND LibraryOrganizationFranchisee.OwnershipType <> \'Franchised\'\n' +
                            '       GROUP BY LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            '       ,LibraryPIMProductPriceLevel.OrganizationID' + '\n' +

                            ' UNION ALL' + '\n' +
                            '  SELECT LibraryPimUpcomingPrices.Price AS Price' + '\n' +
                            '       ,MIN(LibraryPIMProductPriceLevel.DeactivateDiscount) AS DeactivateDiscount' + '\n' +
                            '       ,LibraryPIMProductPriceLevel.ProductID AS ProductID' + '\n' +
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID AS PriceLevelID' + '\n' +
                            '       ,LibraryPIMProductPriceLevel.OrganizationID AS OrganizationID' + '\n' +
                            ' ,2 AS SortOrder' + '\n' +
                            '       FROM LibraryPIMProductPriceLevel' + '\n' +
                            ' JOIN LibraryPimUpcomingPrices ON LibraryPimUpcomingPrices.ProductID = LibraryPIMProductPriceLevel.ProductID ' +
                            ' WHERE LibraryPIMProductPriceLevel.OrganizationID= ' + ReadyString(Web.OrgID()) + '\n' +
                            ' AND LibraryPIMProductPriceLevel.PriceLevelID = ' + ReadyString(priceLevelID) + '\n' +
                            ' AND date(LibraryPimUpcomingPrices.EffectivityDate) <= ' + proj.GetZReadDateSql() +
                            '       GROUP BY LibraryPIMProductPriceLevel.ProductID' + '\n' +
                            '       ,LibraryPIMProductPriceLevel.PriceLevelID' + '\n' +
                            '       ,LibraryPIMProductPriceLevel.OrganizationID' + '\n'

                            //+
                            //' UNION ALL' + '\n' +
                            //' Select Price as Price, "0" as DeactivateDiscount, ProductID as ProductID, PriceLevelID as PriceLevelID, \'' + sessionStorage.OrgID + '\' as OrganizationID' + '\n ' +
                            //' ,1 AS SortOrder' + '\n' +
                            //' FROM LibraryPimUpcomingPrices WHERE date(EffectivityDate) <= ' +  proj.GetZReadDateSql();
                            
                        if ((CStr2(vCustomerID) != '')
                            && (ReadyGuid(vCustomerID) != ReadyGuid(Business.SystemDefaults.CustomerOrganizationID())))
                            pSql += ' UNION ALL' + '\n' + 
                                '  SELECT MAX(LibraryPIMProductPriceLevel.Price) AS Price' + '\n' + 
                                '   ,MIN(LibraryPIMProductPriceLevel.DeactivateDiscount) AS DeactivateDiscount' + '\n' + 
                                '   ,LibraryPIMProductPriceLevel.ProductID AS ProductID' + '\n' + 
                                '   ,LibraryPIMProductPriceLevel.PriceLevelID AS PriceLevelID' + '\n' + 
                                '   ,LibraryPIMProductPriceLevel.OrganizationID AS OrganizationID' + '\n' + 
                                '   ,1 AS SortOrder' + '\n' + 
                                '   FROM LibraryPIMProductPriceLevel' + '\n' + 
                                '   JOIN private.LibraryOrganizationPriceLevel ON LibraryOrganizationPriceLevel.PriceLevelID = LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                                '   JOIN LibraryPIMPriceLevel ON LibraryPIMPriceLevel.ID = LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                                '   WHERE  LibraryOrganizationPriceLevel.OrganizationID = ' + ReadyString(vCustomerID) + '\n' +
                                '   AND LibraryPIMProductPriceLevel.OrganizationID= ' + ReadyString(Web.OrgID()) + '\n' + 
                                '   GROUP BY LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                                '   ,LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                                '   ,LibraryPIMProductPriceLevel.OrganizationID' + '\n'; 

                        pSql += ' ORDER BY \n' +
                            ' SortOrder' + '\n' + 
                            ' )' + '\n' + 
                            ' , CTEA AS (\n' +
                            ' select CTE.* ' + '\n' + 
                            ' from cte' + '\n' + 
                            ' JOIN (' + '\n' + 
                            '     SELECT ProductID, MIN(SortOrder) AS SortOrder' + '\n' + 
                            '     FROM CTE' + '\n' + 
                            '     GROUP BY ProductID' + '\n' + 
                            ' ) CTE2 ON CTE2.ProductID = cte.ProductID' + '\n' + 
                            ' AND CTE2.SortOrder= cte.SortOrder\n' +
                            ')\n';

                        pSql += ' SELECT CTEA.*' + '\n' + 
                            ' FROM CTEA' + '\n' + 
                            ' JOIN (' + '\n' + 
                            '     SELECT ProductID, MAX(Price) AS Price' + '\n' + 
                            '     FROM CTEA' + '\n' + 
                            '     GROUP BY ProductID' + '\n' + 
                            ' ) CTEA2 ON CTEA2.ProductID = CTEA.ProductID' + '\n' + 
                            ' AND CTEA2.Price = CTEA.Price\n';

                        Sql.ExecuteSql(pSql);

                        pSql = 'SELECT DISTINCT ID as ID, ID as ProductID, Name as ProductName, \'\' as ProductNo' + '\n' +
                            ',Name as Name, \'Group\' as table_name, \'1\' as Quantity, \'0\' as DiscountedPrice, \'\' as UnitMeasure, ID as UnitMeasureID' + '\n' +
                            ',MenuID\n' +
                            ',ParentID\n' +
                            ',0 as HasModifier\n' +
                            ',"" as CategoryName\n' +
                            ',OrderNo\n' +
                            ',DefaultOtherPaymentID\n' +
                            ',"" as FileName \n' +                            
                            ',"" as IsPromoItem \n' +
                            '' + (CBool(sessionStorage.IsVendingMachine) ?
                            '   ,"" as IsUseUOM \n' +
                            '   ,"" as SellingPreference \n' +
                            '   ,"" as RecommendedSKU \n' +
                            '   ,"" as RecommendedSKUName \n' +
                            '   ,"" as IsDoNotIncludeBaseUOM \n' +
                            '   ,"" as PlanogramTempID \n' +
                            '   ,"" as SKUCategory \n' +
                            '   , NULL as VDMMotorNo, NULL as VDMDispenserType, NULL as BoardUUID, NULL as VDMBinNo, NULL as RemainingQty \n'
                            : '') +
                            ' FROM private.LibraryPimGroups' + '\n' +
                            ' WHERE IsActive = 1 \n' + 
                            ' -- remove the following WHERE. fast way to query in memory \n' +
                            ' -- AND ParentID = ' + ReadyString(vID) + '\n' +
                            ' UNION SELECT sku.ID as ID, sku.ProductID as ProductID, LibraryPIMProduct.ProductName as ProductName, LibraryPIMProduct.ProductNo' + '\n' +
                            ',sku.ShortName as Name, \'SKU\' as table_name, LibraryPIMProductMeasurement.Quantity as Quantity, ' +
                            ' LibraryPIMProductMeasurement.Quantity * price.Price - (LibraryPIMProductMeasurement.Quantity * price.Price) * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT price.DeactivateDiscount) AS DiscountedPrice,' +
                            ' LibraryCountriesLanguageLabel.Label as UnitMeasure, LibraryPIMProductMeasurement.UnitMeasureID as UnitMeasureID' + '\n' +
                            ',sku.MenuID\n' +
                            ',sku.GroupID AS ParentID\n' +
                            ',_ProductCategories.ProductID IS NOT NULL AS HasModifier\n' +
                            ',_ProductCategories.CategoryName\n' +
                            ',sku.OrderNo\n' +
                            ',"" as DefaultOtherPaymentID\n' +
                            ',pic.GroupImageID as FileName \n' +
                            ',(SELECT IsPromoItem FROM LibraryOrganizationPIM WHERE ProductID = sku.ProductID) as IsPromoItem \n' +
                            '' + (CBool(sessionStorage.IsVendingMachine) ? 
                            '   ,IFNULL(sku.IsUseUOM, 0) as IsUseUOM\n' +
                            '   ,IFNULL(sku.SellingPreference, 0) as SellingPreference\n' +
                            '   ,IFNULL(sku.RecommendedSKU, "No Recommendation") as RecommendedSKU\n' +
                            '   ,CASE WHEN IFNULL(RecommendedSKU, "No Recommendation") = "No Recommendation" THEN "" ELSE (SELECT ProductName FROM LibraryPimProduct WHERE ID = RecommendedSKU) END as RecommendedSKUName\n' +
                            '   ,IFNULL(sku.IsDoNotIncludeBaseUOM, 0) as IsDoNotIncludeBaseUOM\n' +
                            '   ,IFNULL(sku.PlanogramTempID, "") as PlanogramTempID\n' +
                            '   ,(SELECT C0020 FROM LibraryPIMProductVariableFeatures001 WHERE ProductID = sku.ProductID) as SKUCategory\n' +
                            '   , (SELECT CAST(DispenserMotorNo as INTEGER) + (CASE WHEN DispenserType = "Spring Coil" THEN 10 ELSE 0 END) FROM LibraryVendingMachinePlanogramTemplateList as plan WHERE plan.PlanogramTemplateID = sku.PlanogramTempID AND CAST(TRIM(CASE WHEN INSTR(sku.VDMMotorNo, ", ") > 0 THEN SUBSTR(sku.VDMMotorNo, 1, INSTR(VDMMotorNo, ",") - 1) ELSE sku.VDMMotorNo END) AS INT) = plan.MotorNo AND plan.ProductID = sku.ProductID) as VDMMotorNo ' +
                            '   , (SELECT DispenserType FROM LibraryVendingMachinePlanogramTemplateList as plan WHERE plan.PlanogramTemplateID = sku.PlanogramTempID AND sku.VDMMotorNo = plan.MotorNo AND plan.ProductID = sku.ProductID) as VDMDispenserType ' +
                            '   , (SELECT UUID FROM LibraryVendingMachinePlanogramTemplateList as plan INNER JOIN LibraryVendingMachineBoards as board ON board.ID = plan.BoardID WHERE sku.PlanogramTempID = plan.PlanogramTemplateID AND sku.VDMMotorNo = plan.MotorNo AND plan.ProductID = sku.ProductID) as BoardUUID \n' +
                            '   , sku.VDMBinNo, NULL as RemainingQty \n'
                            : '') +
                            ' FROM private.LibraryPimGroupPims sku LEFT JOIN LibraryPIMProduct ON sku.ProductID = LibraryPIMProduct.ID' + '\n' +
                            ' LEFT JOIN' +
                           
                            // 2017_04_21__10_12_37_8281: commented by Arvin. Reason: hangs on Lenovo business tablet
                            // added by Arvin; Apr 2, 2016
                            // select only the 1st measurement from each product
                            //' (SELECT * FROM LibraryPIMProductMeasurement' + '\n' + 
                            //' WHERE LibraryPIMProductMeasurement.ID IN (' + '\n' + 
                            //' SELECT t.ID' + '\n' + 
                            //' FROM LibraryPIMProductMeasurement t' + '\n' + 
                            //' WHERE t.ProductID = LibraryPIMProductMeasurement.ProductID' + '\n' + 
                            //' ORDER BY t.Quantity, t.MeasurementLevel' + '\n' + 
                            //' LIMIT 1' + '\n' + 
                            //' ))\n' +
                            // END 2017_04_21__10_12_37_8281: commented by Arvin. Reason: hangs on Lenovo business tablet

                            ' LibraryPIMProductMeasurement ON sku.ProductID = LibraryPIMProductMeasurement.ProductID' + '\n' +
                            
                            '  LEFT JOIN _ProductCategories ON sku.ProductID = _ProductCategories.ProductID\n' +
                            '  LEFT JOIN private._HotButtonsPrice price  ON sku.ProductID = price.ProductID\n' +

                            ' LEFT JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID' + '\n' +
                            ' LEFT JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID' +
                            " LEFT JOIN LibraryPIMPictures as pic ON pic.ProductID = sku.ProductID AND pic.IsDefault = 1 AND pic.Size = 'big' \n" 
                            if (WithContentsOnly) {
                                pSql += ' JOIN LibraryPIMProductContents ON LibraryPIMProductContents.ParentProductID = LibraryPIMProduct.ID' +
                                ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(Web.OrgID()) + '\n';
                                
                            }
                            if (vBatchProductOnly)
                                pSql += ' JOIN LibraryPIMProductParent ON LibraryPIMProductParent.ProductID = sku.ProductID \n' +
                                    ' AND LibraryPIMProductParent.OrganizationID = ' + ReadyString(Web.OrgID()) + '\n' +
                                    ' AND IFNULL(LibraryPIMProductParent.Jit, 0) = 0 \n';
                                
                            pSql += ' WHERE sku.IsActive = 1\n' +
                                ' -- AND LibraryPIMProductMeasurement.MeasurementLevel = 1 -- commented by Arvin 2019_12_05__13_44_53_1047 \n' +
                                ' AND LibraryPIMProductMeasurement.Quantity = 1\n' +
                                ' AND sku.MenuID= ' + ReadyString(menuID) + '\n';

                        pSql += ' ORDER BY table_name, OrderNo, Name';

                        pDT = Sql.Execute(pSql);

                        // BEGIN 2017_04_21__10_12_37_8281: code to unique-fy without subquery
                        pDT.Rows = GetUniqueIDProductIDMenuIDParentID(pDT.Rows);

                        function GetUniqueIDProductIDMenuIDParentID(vRows) {
                            var pOut = [];
                            for (var p = 0; p <= vRows.length - 1; p++) {
                                var pRow = vRows[p];
                                var pExisting = GetByIDProductIDMenuIDParentID(pOut, pRow.ID, pRow.ProductID, pRow.MenuID, pRow.ParentID);
                                if (pExisting == null)
                                    pOut.push(pRow);
                            }
                            return pOut;
                        }

                        function GetByIDProductIDMenuIDParentID(vRows, vID, vProductID, vMenuID, vParentID) {
                            for (var p = 0; p <= vRows.length - 1; p++) {
                                var pRow = vRows[p];
                                if ((pRow.ID == vID)
                                    && (pRow.ProductID == vProductID)
                                    && (pRow.MenuID = vMenuID)
                                    && (pRow.ParentID == vParentID))
                                    return pRow;
                            }
                        }
                        // END 2017_04_21__10_12_37_8281: code to unique-fy without subquery

                        try {
                            sessionStorage.HotButtonData = JSON.stringify(pDT);
                        } catch (e) {
                            sessionStorage.removeItem('HotButtonData');
                        }
                        sessionStorage.setItem('HotButtonDataCacheTemplateID', menuID);
                        sessionStorage.setItem('HotButtonDataCacheWithContentsOnly', CInt2(WithContentsOnly));
                        sessionStorage.setItem('HotButtonDataCacheBatchProductOnly', CInt2(vBatchProductOnly));
                        sessionStorage.setItem('HotButtonDataCacheCustomerID', ReadyGuid(vCustomerID));
                        return pDT;
                    }
                }; // GetHotButtonsData

                function GetButtonsFromCache(vMenuID, vID) {
                    
                    var pOut = {
                        Columns: mButtonData,
                        Rows: []
                    };
                    vMenuID = ReadyGuid(vMenuID);
                    vID = ReadyGuid(vID);

                    for (var p = 0; p <= mButtonData.Rows.length - 1; p++) {
                        var pDR = mButtonData.Rows[p];
                        if ((ReadyGuid(pDR.MenuID) == vMenuID)
                            && (ReadyGuid(pDR.ParentID) == vID))
                            pOut.Rows.push(pDR);
                    }

                    return pOut;

                } // END function GetButtonsFromCache(vMenuID, vID) {

                this.SaveCustomerDetails = function (tableNo, invID) {

                    var pIU;
                    var pSql;
                    var pResult;

                    pIU = new InsertUpdate('TransInvoice');
                    //                pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('CustomerTableNo', ReadyString(tableNo));
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(invID));
                    Sql.ExecuteSql(pSql);

                    return true;

                }; // END SaveCustomerDetails


                this.UpdateCustomer = function (invID, custID) {

                    var pIU;
                    var pSql;
                    var pResult;

                    pIU = new InsertUpdate('TransInvoice');
                    //                pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('CustomerID', ReadyString(custID));
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(invID));
                    Sql.ExecuteSql(pSql);

                    return true;

                }; // END SaveCustomerDetails




                this.getCountryListAll = function () {
                    var pOut = {};
                    var pDT;
                    var pSql;

                    pSql = 'SELECT ID, Name as Country' + '\n' +
                      ' FROM LibraryCountries'

                    pSql += ' ORDER BY Country ASC';
                    pDT = Sql.Execute(pSql);

                    return { 'CountryData': pDT };

                }; // getCountryListAll

                this.getCityListPH = function () {
                    var pOut = {};
                    var pDT;
                    var pSql;

                    pSql = 'SELECT ID, Name as City' + '\n' +
                      ' FROM LibraryCities'

                    pSql += ' ORDER BY City ASC';
                    pDT = Sql.Execute(pSql);

                    return { 'CityData': pDT };

                }; // getCountryListAll

                this.getFowsts = function () {
                    var pOut = {};
                    var pDT;
                    var pSql;

                    pSql = 'SELECT ID, FowstTypeID, Name' + '\n' +
                      ' FROM LibraryOrganizationFowst WHERE OrganizationID =' + ReadyString(sessionStorage.OrgID);

                    pSql += ' ORDER BY Name ASC';
                    pDT = Sql.Execute(pSql);

                    return { 'FowstData': pDT };

                }; // getFowsts




                //////////////////////////////
                ////////END OF ADDITIONAL BY JANINE
                //////////////////////////////

                this.PosCart = function (vData) {

                    var pInvoiceID;
                    var pDsCart;
                    var pOut;

                    console.time('SalesMenu_offline PosCart jsS8S9RLVVLF!');
                    pInvoiceID = vData.OrderID; // yes, OrderID is not a typo

                    if (IsGuid(pInvoiceID)) {
                        if (!Business.TransInvoice.IsValidCart(pInvoiceID))
                            return { 'text': 'No item' };
                    } else {
                        if (CBool(vData.DontFetchIfBlank))
                            return { 'text': 'No item' };
                    }

                    pDsCart = Business.TransInvoice.GetCart(pInvoiceID);
                    if (pDsCart != undefined) {
                        pOut = {};
                        pOut.Master = pDsCart.Invoices.Rows[0];
                        pOut.Units = pDsCart.Stocks;
                        pOut.Stocks = JSON.parse(JSON.stringify(pOut.Units)); // clone JS objects; http://heyjavascript.com/4-creative-ways-to-clone-objects/

                        // pOut.Stocks.Rows = GetDistinct(pOut.Stocks.Rows, ['ProductID', 'ProductNo', 'ProductName', 'ModifierGroupID', 'IsOrderSlipPrinted']); //old
                        pOut.Stocks.Rows = GetDistinct(pOut.Stocks.Rows, ['ProductID', 'ProductNo', 'ProductName', 'ModifierGroupID']); // new attempt

                        pOut.Modifiers = pDsCart.Modifiers;
                        //AttachModifers(pDsCart, pOut);
                        proj.CartData = pOut;

                        console.timeEnd('SalesMenu_offline PosCart jsS8S9RLVVLF!');
                        return pOut;
                    } else
                        return { 'text': 'No item' };

                }; // PosCart

                this.DeleteStockPos = function (vData) {

                    var pInvoiceID;
                    var pProductID;
                    var pUnitMeasure; // additional by janine sept 9 2014 
                    var pDT;
                    var pIU;
                    var pSql;
                    var pModifierGroupID;

                    pInvoiceID = vData.OrderID; // yes, OrderID is not a typo
                    pProductID = vData.ProductID;
                    pUnitMeasure = vData.UnitMeasure; // additional by janine sept 9 2014 
                    pModifierGroupID = vData.ModifierGroupID;

                    if (Business.TransInvoice.IsValidCart(pInvoiceID)) {
                        pDT = Business.TransInvoice.GetCartStock(pInvoiceID, pProductID, proj.PosMode, pModifierGroupID);
                        if (pDT.Rows.length > 0) {
                            Sql.BeginTransaction();
                            var pOldInvoiceAmount = Business.TransInvoice.GetTotalAmount(pInvoiceID);
                            for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                                var pDR = pDT.Rows[p];
                                if ((CInt2(pDR.QuantityPrinted) == 0)
                                    || (CDec2(pDR.Quantity) == 0))

                                    Business.TransInvoice.DeleteOrderStock(pInvoiceID, pProductID, pUnitMeasure, pModifierGroupID, null, {
                                    createVoidEntries: true
                                });
                                else {
                                    pIU = new InsertUpdate('TransInvoiceItems');
                                    pIU.Add('OrderedQuantity', 0);
                                    pIU.Add('Quantity', 0);
                                    pSql = 'InvoiceID = ' + ReadyString(pInvoiceID) +
                                        ' AND ProductID = ' + ReadyString(pProductID) +
                                        ' AND IFNULL(TransInvoiceItems.ModifierGroupID, \'\') = ' + ReadyString(pModifierGroupID);
                                    if (CStr2(pUnitMeasure) != '')
                                        pSql += ' AND UnitMeasure = ' + ReadyString(pUnitMeasure);
                                    pSql = pIU.GetUpdate(pSql);
                                    pSql = Sql.ExecuteSql(pSql); // output is used for debugging only
                                }
                            }
                            pIU = new InsertUpdate('TransInvoice');
                            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                            // old code. commented on 2018_01_11__11_01_14_4978
                            // pIU.Add('LastUpdated', ReadyString(FormatDate(proj.GetZReadDate())));
                            pIU.Add('LastUpdated', proj.SqliteNow);
                            var pNewInvoiceAmount = Business.TransInvoice.GetTotalAmount(pInvoiceID);
                            pIU.Add('TotalAmount', pNewInvoiceAmount);
                            var pErrorCorrectAmount = Math.abs(pOldInvoiceAmount - pNewInvoiceAmount);
                            if (pErrorCorrectAmount > 0) {
                                pIU.Add('ErrorCorrectAmount', 'IFNULL(ErrorCorrectAmount, 0) + ' + pErrorCorrectAmount);
                                pIU.Add('ErrorCorrectCount', 'IFNULL(ErrorCorrectCount, 0) + 1');
                            }
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(pInvoiceID));
                            Sql.ExecuteSql(pSql);
                            Business.TransInvoice.UpdateItemCount(pInvoiceID);
                            Sql.CommitTransaction();
                            return { 'Result': 'ok' };
                        } else
                            return { 'Error': 'That item was already removed from your cart' };
                    } else
                        return { 'Error': 'Invalid cart' };

                }; // DeleteStockPos

                this.UpdateCounterFromOnline = function (vParams) {

                    vParams = $.extend({
                        OnSuccess: function(){},
                        OnError: function(){},
                        OnExit: function(){}
                    }, vParams);

                    if (window.IsOfflineMode) {
                        vParams.OnExit();
                        return;
                    }

                    var pOptions = {};
                    if (GetUrlParameters().mode == 'po') 
                        pOptions = {
                            FieldName: 'POSLastPONo',
                            InvoiceFieldName: 'PurchaseOrderNo'
                        };
                    var pCounter = Business.TransInvoice.VerifyCounter(pOptions);
                    var pDeviceID = Web.DeviceID();
                    var pLog;
                    var pData;

                    pData = {
                        FowstID: sessionStorage.FowstID,
                        DeviceID: pDeviceID,
                        DeviceCounter: pCounter,
                        FieldName: pOptions.FieldName,
                        InvoiceFieldName: pOptions.InvoiceFieldName
                    }
                    pLog = 'js2KVE9PZA8U! Getting counter from server for fowst (' + pData.FowstID + ') and device (' + pData.DeviceID + '). ';
                    AjaxSend({
                        url: '/SalesMenu/VerifyCounter',
                        timeout: proj.AjaxTimeout + 5000,
                        dontHandleSessionExpired: true,
                        data: pData,
                        success: function (vData) {

                            var pCounter;
                            var pSkip;
                            var pMessage;

                            try {
                                if (CBool(vData.Deactivated)) {
                                    Business.LibraryDevice.UpdateIsActive(Web.DeviceID(), false);
                                    Business.LibraryDevice.HandleInactive(true);
                                } else {
                                    pSkip = vData.Skip;
                                    if (IsNumeric(pSkip)) {
                                        // pMessage = 'Your invoice numbers will be skipping. Next number will be ' + (vData.DeviceCounter + 1) + '. It should be ' + (vData.Counter + 1) + '. ' + 
                                        pMessage = 'The system has detected a change in the invoice number series. ' +
                                        'Please report this timestamp to your administrator: ' + vData.TimeStamp + '.';
                                        //                                    MessageBox(pMessage);
                                        //                                    Android.AddLog(pMessage);
                                    }
                                    Business.LibraryDevice.UpdateIsActive(Web.DeviceID(), true);
                                    if (IsNumeric(vData.Counter)) {
                                        pLog += 'jsJ3A4DSF9VA! Counter from server = ' + vData.Counter + '.';
                                        if (proj.LogInvoiceNumberCounter)
                                            Android.AddLog(pLog);
                                        Business.TransInvoice.UpdateCounter(vData.Counter, pOptions);
                                        vParams.OnSuccess();
                                    };
                                }
                            } catch (e) {
                                vParams.OnError(e.message);
                                MessageBox(e.message);
                            }
                            vParams.OnExit();

                        },
                        error: function (e) {
                            if (e == null)
                                vParams.OnError();
                            else
                                vParams.OnError(e.responseText);
                            Business.LibraryDevice.HandleInactive();
                            vParams.OnExit();
                        }
                    });

                }; // END UpdateCounterFromOnline

                this.SaveSalesOrder = function (vData) {
                    // PARAMS:
                    //  vData:
                    //      DeliveryAddressID (optional)
                    //      BillingAddressID (optional)
                    //      Remarks (optional)
                    //      AsOrderSlip (optional) : boolean

                    var pIU;
                    var pSql;
                    var pDTInvDetails;
                    var pUpdateOrderSlipNum;
                    var pTransNum;
                    var blnIsPurchaseOrderEdit = false;

                    if (!IsGuid(vData.InvoiceID))
                        throw 'vData.InvoiceID must be GUID';

                    if (vData.IsPurchaseOrder) {
                        //check Purchase Order No
                        pSql = "SELECT inv.ID " +
                            " ,inv.InvoiceNumber " +
                            " ,IFNULL(inv.PurchaseOrderNo,0) as PurchaseOrderNo " +
                            " ,inv.PurchaseOrderDate " +
                            " FROM private.TransInvoice inv " +
                            " Where inv.ID = " + ReadyString(vData.InvoiceID);
                        pDTInvDetails = Sql.Execute(pSql);
                        if (pDTInvDetails.Rows.length > 0) {
                            if (pDTInvDetails.Rows[0].PurchaseOrderNo > 0) {
                                blnIsPurchaseOrderEdit = true;
                            }
                        }
                    } else {
                        //check Sales Order No
                        pSql = "SELECT inv.ID " +
                            " ,inv.InvoiceNumber " +
                            " ,IFNULL(inv.SalesOrderNo,0) as SalesOrderNo " +
                            " ,inv.SalesOrderDate " +
                            " ,inv.OrderSlipNum " +
                            " FROM private.TransInvoice inv " +
                            " Where inv.ID = " + ReadyString(vData.InvoiceID);
                        pDTInvDetails = Sql.Execute(pSql);
                        var blnIsSalesOrderEdit = false;
                        if (pDTInvDetails.Rows.length > 0) {
                            if (pDTInvDetails.Rows[0].SalesOrderNo > 0) {
                                blnIsSalesOrderEdit = true;
                            }
                            if ((vData.AsOrderSlip) 
                                && (CDec2(pDTInvDetails.Rows[0].OrderSlipNum)) == 0) {

                                pUpdateOrderSlipNum = true;
                            }
                        }
                    }

                    //Update Invoice 
                    pIU = new InsertUpdate('private.TransInvoice');

                    if (vData.IsPurchaseOrder) {
                        if (!blnIsPurchaseOrderEdit) {
                            pTransNum = Business.TransInvoice.GetCounter({
                                FieldName: 'POSLastPONo',
                                InvoiceFieldName: 'PurchaseOrderNo'
                            });
                            pIU.Add('PurchaseOrderNo', pTransNum + 1);
                        }
                    } else if (pUpdateOrderSlipNum) {
                        pIU.Add('OrderSlipNum', "(SELECT IFNULL(devfowst.OrderSlipNum, 0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");
                        if (!blnIsSalesOrderEdit)
                        pIU.Add('SalesOrderNo', "(SELECT IFNULL(devfowst.POSLastSONo,0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");
                    } else if (!blnIsSalesOrderEdit) {
                            pIU.Add('SalesOrderNo', "(SELECT IFNULL(devfowst.POSLastSONo,0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");
                    }

                    pIU.Add('SalesOrderDate', proj.SqliteNow);
                    pIU.Add('SalesOrderUserID', ReadyString(Web.PeopleUserID()));
                    pIU.Add('InvoiceDate', proj.SqliteNow);
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                    if (vData.DeliveryAddressID != undefined)
                        pIU.Add('DeliveryAddressID', ReadyString(vData.DeliveryAddressID));
                    if (vData.BillingAddressID != undefined)
                        pIU.Add('BillingAddressID', ReadyString(vData.BillingAddressID));
                    if (vData.Remarks != undefined)
                        pIU.Add('Remarks', ReadyString(vData.Remarks));
                    if (vData.DepositAmount != null)
                        pIU.Add('DepositAmount', vData.DepositAmount);

                    //additional by janine
                    if (vData.IsPurchaseOrder) {
                        pIU.Add('PurchaseOrderDate', proj.SqliteNow);
                        pIU.Add('PurchaserName', ReadyString(sessionStorage["CurrentUserFullName"]));
                    } else {
                        if (CStr2(vData.PurchaseOrderNo) != '')
                            pIU.Add('PurchaseOrderNo', ReadyString(vData.PurchaseOrderNo));
                        if (CStr2(vData.PurchaseOrderDate) != '')
                            pIU.Add('PurchaseOrderDate', ReadyString(vData.PurchaseOrderDate));
                        if (CStr2(vData.PurchaserName) != '')
                            pIU.Add('PurchaserName', ReadyString(vData.PurchaserName));
                    }
                    if (CStr2(vData.Signatory) != '')
                        pIU.Add('SalesOrderSignatory', ReadyString(vData.Signatory));
                    if (CStr2(vData.DeliveryDate) != '')
                        pIU.Add('DeliveryDate', ReadyString(vData.DeliveryDate));
                    //end of additional
                    if (CStr2(vData.InvoiceTypeCode) != '')
                        pIU.Add('InvoiceTypeCode', ReadyString(vData.InvoiceTypeCode));

                    //additional sept 4 2014
                    if (CStr2(vData.FowstID) != '')
                        pIU.Add('FowstID', ReadyString(vData.FowstID));

                    //end of additional
                    if (vData.Remarks != null)
                        pIU.Add('Remarks', ReadyString(vData.Remarks));

                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.InvoiceID));

                    Sql.BeginTransaction();
                    Sql.ExecuteSql(pSql);


                    if (vData.IsPurchaseOrder) {
                        ///Update FOWST POS PO Counter
                        if (!blnIsPurchaseOrderEdit) {
                            pIU = new InsertUpdate('private.LibraryDeviceFowst');
                            pIU.Add('POSLastPONo', pTransNum);
                            pSql = pIU.GetUpdate("FowstID = " + ReadyString(sessionStorage.FowstID) + " AND DeviceID = " + ReadyString(Web.DeviceID()));
                            Sql.ExecuteSql(pSql);

                            SendTransactionViaSMS('PO');
                        }
                    } else {

                        ///Update FOWST POS SO Counter
                        // if (!blnIsSalesOrderEdit && !CBool(vData.AsOrderSlip)) { // old code 2016_08_25__08_57_21_2553
                        if (!blnIsSalesOrderEdit) {
                            pIU = new InsertUpdate('private.LibraryDeviceFowst');
                            pIU.Add('POSLastSONo', "(SELECT IFNULL(devfowst.POSLastSONo,0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");
                            pSql = pIU.GetUpdate("FowstID = " + ReadyString(sessionStorage.FowstID) + " AND DeviceID = " + ReadyString(Web.DeviceID()));
                            Sql.ExecuteSql(pSql);

                            SendTransactionViaSMS('SO');
                        }
                    }

                    var pOut = {};
                    var pOutInvoice;

                    pOutInvoice = Business.TransInvoice.GetByID(vData.InvoiceID);
                    if (pOutInvoice.Rows.length > 0) {
                        pOut.ID = vData.InvoiceID;
                        pOut.SalesOrderNo = pOutInvoice.Rows[0].SalesOrderNo;
                        pOut.InvoiceDate = pOutInvoice.Rows[0].InvoiceDate;
                        pOut.IsDraft = CDec2(pOutInvoice.Rows[0].IsDraft);
                        pOut.OrderSlipNum = CDec2(pOutInvoice.Rows[0].OrderSlipNum);
                    }

                    if (CBool(vData.AsOrderSlip))
                        Business.TransInvoice.UpdateCounter(pOut.OrderSlipNum, {
                            FieldName: 'OrderSlipNum',
                            InvoiceFieldName: 'OrderSlipNum',
                            DontUseFileCounter: true
                        });
                    else if (vData.IsPurchaseOrder && !blnIsPurchaseOrderEdit) 
                        Business.TransInvoice.UpdateCounter(pTransNum, {
                            FieldName: 'POSLastPONo',
                            InvoiceFieldName: 'PurchaseOrderNo'
                        });
                    Sql.CommitTransaction();

                    function SendTransactionViaSMS(TransType) {

                        var pData = {};
                        var mData;
                        var mGateWayMobileNo;
                        var pSettings = Transfers.ReadFile('SMSGateWaySettings.txt');

                        if (pSettings != undefined) {

                            pSettings = JSON.parse(pSettings);

                            if (CBool(pSettings.IsAllowToSendTransactionViaSMS)) {
                                pData.PosMode = proj.PosMode ? "pos" : "so";

                                pData.CustomerID = sessionStorage.CustomerOrganizationID;
                                pData.OrderID = sessionStorage.CartOrderID;
                                pData.FowstID = sessionStorage.FowstID;

                                mData = SalesMenu_offline().PosCart(pData);

                                delete mData.Master.CustomerName
                                delete mData.Master.CustomerFowstName
                                delete mData.Master.DeliveryAddress
                                delete mData.Master.OrgFowst
                                delete mData.Master.OrgName
                                delete mData.Master.SupplierFowstName
                                delete mData.Master.SupplierOrgName
                                delete mData.Stocks
                                delete mData.Modifiers

                                for (i = 0; i <= mData.Units.Rows.length - 1; i++) {
                                    delete mData.Units.Rows[i].ProductName
                                    delete mData.Units.Rows[i].ProductNo
                                }


                                if (pSettings.OverrideIPAddress == '' || pSettings.OverrideIPAddress == undefined) {
                                    mGateWayMobileNo = pSettings.DefaultIPAddress
                                } else {
                                    mGateWayMobileNo = pSettings.OverrideIPAddress
                                }

                                SendTransactionAsSMS(mData, TransType, mGateWayMobileNo);
                            }
                        }
                    }

                    return pOut;

                };

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

                    if (vData.SaveAsNew) {
                        vData.Master.ID = NewID();
                        for (var p = 0; p <= vData.Units.Rows.length - 1; p++) 
                            vData.Units.Rows[p].InvoiceID = vData.Master.ID;
                    }

                    ImportInvoice(vData.Master);
                    ImportItems(vData.Units.Rows);
                    Business.TransInvoice.UpdateTotalAmount(vData.Master.ID);
                    Business.TransInvoice.UpdateItemCount(vData.Master.ID);

                    if (CBool(proj.POMode)) {
                        if (mPurchaseOrder == null)
                            mPurchaseOrder = new App.controller.PurchaseOrder();
                        mPurchaseOrder.SaveSupplierAndCustomer(vData.Master.ID);
                    }

                    function ImportInvoice(vData) {

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
                        if (IsDate(vData.PurchaseOrderDate))
                            pIU.Add('PurchaseOrderDate', ReadyString(vData.PurchaseOrderDate));
                        else
                            pIU.Add('PurchaseOrderDate', 'null');
                        // commented 2017_04_20__10_47_54_7427
                        //if (IsNumeric(vData.PurchaseOrderNo))
                        //    pIU.Add('PurchaseOrderNo', CDec2(vData.PurchaseOrderNo));
                        //else
                        //    pIU.Add('PurchaseOrderNo', 'null');
                        if (CStr2(vData.PurchaserName) != '')
                            pIU.Add('PurchaserName', ReadyString(vData.PurchaserName));
                        else
                            pIU.Add('PurchaserName', 'null');
                        if (CStr2(vData.CustomerTableNo) != '')
                            pIU.Add('CustomerTableNo', ReadyString(vData.CustomerTableNo));
                            
                        pIU.Add('IsCancelled', 0);
                        pIU.Add('DepositAmount', CDec2(vData.DepositAmount));
                        
                        pSql = 'DELETE FROM TransInvoiceItems WHERE InvoiceID = ' + ReadyString(vData.ID);
                        Sql.ExecuteSql(pSql);

                        pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                        if (Sql.ExecuteSql(pSql) == 0) {
                            pSql = pIU.GetInsert();
                            Sql.ExecuteSql(pSql);
                        };

                    };

                    function ImportItems(vData) {

                        var pIU;
                        var pSql;
                        var pData;

                        for (var p = 0; p <= vData.length - 1; p++) {
                            pData = vData[p];
                            pIU = new InsertUpdate('TransInvoiceItems');
                            pData.ID = NewID();
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
                            if ($.isFunction(pData.AfterSave))
                                pData.AfterSave(pData);
                        };

                    };

                }; // END this.Import = function(vData) {

                this.SaveRsd = function (vData) {

                    var pIU;
                    var pSql;
                    var pResult;

                    pIU = new InsertUpdate('TransInvoice');
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('InvoiceDate', ReadyString(vData.DateTime));
                    pIU.Add('ReasonForReconstructingData', ReadyString(vData.ReasonForReconstructingData));
                    pIU.Add('RefNum', ReadyString(vData.RefNum));
                    pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.InvoiceID));
                    pResult = Sql.ExecuteSql(pSql);
                    if (CDec2(pResult) > 0)
                        return { Result: 'ok' };
                    else
                        return { Error: 'No error in SaveRsd but nothing was updated.' };

                }; // END this.SaveRsd

                this.AddMultiToCart = function (vData, vParams) {
                    //vParams = { // optional
                    //    Count: 50  // optional
                    //};

                    // requires:
                    //  sessionStorage.CustomerOrganizationID
                    //  Android.GetSessionData().FowstID

                    // sample vData:
                    // [
                    //      {
                    //          "ID": "796311b7-d87c-4f6e-84f0-ef3c5105d9f0",
                    //          "ProductNo": "6927511315048",
                    //          "ProductName": ", GLUE, Stick #5002 Med, 15g",
                    //      },
                    //      {
                    //          "ID": "813f5eb4-a1d3-4206-b65b-0c22c2b50649"
                    //      },
                    //      {
                    //          "ID": "e024abb3-c639-425c-9378-e5e2de4df91d"
                    //      },
                    //      {
                    //          "ID": "4f8603b5-5b99-4f00-8677-577e52b25482"
                    //      }
                    // ]
                    // note: ProductNo and ProductName are optional. only used during problems (e.g. no unit)

                    var pStock_offline = Stock_offline();
                    var pData = {};
                    var pDR;
                    var pPriceLevelID;
                    var pResult;
                    var pOut = {
                        NoUnit: [], // no unit measure
                        Success: [],
                        Existing: []
                    };
                    var pParams = $.extend({
                        Count: 50
                    }, vParams);
                    var pCount;
                    var pCart;
                    var pBaseUnit;

                    pPriceLevelID = Business.SystemDefaults.GetRetailPriceID();
                    pData.CustomerID = sessionStorage.CustomerOrganizationID;
                    //additional by janine apr 1 2014
                    if (sessionStorage.GasPumpNo != '')
                        pData.CustomerTableNo = 'GP_' + sessionStorage.GasPumpNo;

                    if (sessionStorage.CustomerTableNo != '')
                        pData.CustomerTableNo = 'TBL_' + sessionStorage.CustomerTableNo;

                    //end of additional
                    pData.OrderID = sessionStorage.CartOrderID; // InvoiceID
                    pData.NewCartMode = localStorage.NewCartMode; // if this is true, ignore InvoiceID
                    pData.Remarks = '';

                    if (!CBool(pData.NewCartMode)) {
                        pCart = SalesMenu_offline().PosCart({
                            OrderID: pData.OrderID,
                            DontFetchIfBlank: true
                        });
                    }

                    pCount = Math.min(pParams.Count, vData.length);
                    for (var pRowIndex = 0; pRowIndex <= pCount - 1; pRowIndex++) {
                        pDR = vData[pRowIndex];
                        pBaseUnit = Business.LibraryPIMProduct.BaseUnit(pDR.ID);
                        if ((pBaseUnit == undefined)
                            || (CDec2(pBaseUnit.UnitQty) == 0)) {
                            pOut.NoUnit.push({
                                ID: pDR.ID,
                                ProductNo: pDR.ProductNo,
                                ProductName: pDR.ProductName
                            });
                        } else {
                            if (IsAlreadyInCart(pDR.ID)) {
                                pOut.Existing.push(pDR.ID);
                            } else {
                                var pUnitPrice = Business.LibraryPIMProduct.GetPrice(pDR.ID, '', pPriceLevelID, sessionStorage.OrgID);
                                pData.StockID = pDR.ID;
                                pData.Stocks = [{
                                    Qty: 1,
                                    UnitQty: pBaseUnit.UnitQty,
                                    PriceLevelID: pPriceLevelID,
                                    UnitPrice: pUnitPrice
                                }];
                                pResult = pStock_offline.UpdatePos(pData);
                                pData.OrderID = pResult.OrderID;
                                pData.NewCartMode = false;
                                pOut.Success.push(pDR.ID);
                            }
                        } // END if (Business.LibraryPIMProduct.BaseUnit(pDR.ID) == undefined) {
                    } // END for (var pRowIndex = 0; pRowIndex <= vData.length - 1; pRowIndex++) {
                    if (pResult != undefined) {
                        sessionStorage.IsDraft = 1;
                        pOut.OrderID = pResult.OrderID;
                        sessionStorage.CartOrderID = pResult.OrderID;
                        localStorage.NewCartMode = false;
                    }

                    function IsAlreadyInCart(vProductID) {

                        if (pCart == undefined)
                            return false;

                        if (pCart.Stocks == undefined)
                            return false;

                        if (pCart.Stocks.Rows == undefined)
                            return false;

                        if (pCart.Stocks.Rows.length == 0)
                            return false;

                        for (var p = 0; p <= pCart.Stocks.Rows.length - 1; p++) {
                            if (ReadyGuid(pCart.Stocks.Rows[p].ProductID) == ReadyGuid(vProductID))
                                return true;
                        }

                        return false;

                    } // END IsAlreadyInCart
                    return pOut;

                }; // AddMultiToCart


                this.HandleAddMultiToCart = function (vLvRecentlyBought) {
                    try {
                        if ((vLvRecentlyBought == undefined)
                            || (vLvRecentlyBought.GetData() == undefined)
                            || (vLvRecentlyBought.GetData().length == 0)) {

                            MessageBox('No recently bought items for the selected customer and date range.');
                            return;
                        }

                        MessageBox('You are about to add all ' + vLvRecentlyBought.GetData().length + ' recently bought items to the cart. Do you want to continue?', 'Add to cart',
                            [
                                {
                                    Text: 'Yes, add to cart',
                                    Click: function () {
                                        MessageBox('Adding to cart... please wait');
                                        setTimeout(function () {
                                            var pResult;
                                            var pMessage;

                                            try {
                                                pResult = SalesMenu_offline().AddMultiToCart(vLvRecentlyBought.GetData());
                                                if (pResult == null) {
                                                    return;
                                                }
                                                pMessage = pResult.Success.length + ' items were added to the cart. ';
                                                if (pResult.Existing.length > 0)
                                                    pMessage += pResult.Existing.length + ' items were already in the cart. ';
                                                if (pResult.NoUnit.length > 0)
                                                    pMessage += pResult.NoUnit.length + ' items were not added due to problems (e.g. no unit, no price)';
                                                MessageBox(pMessage, null, null, {
                                                    OnClose: function () {
                                                        proj.ShowCart(1);
                                                    }
                                                });
                                            } catch (e) {
                                                MessageBox('js7WSV9A267K! ' + e.stack);
                                            }
                                        }, 1000);                                        
                                    }
                                },
                                {
                                    Text: 'No, don\'t add to cart'
                                }
                            ]
                        );
                    } catch (e) {
                        MessageBox('jsN3W3CLD37U! ' + e.stack);
                    }
                }; // END this.HandleAddMultiToCart

                this.BrowseCustomerAddress = function (vParams) {
                    /***
                    vParams = {
                    OrgID: 'customer org id',
                    Click: function(e) {    
                    console.log(e.ID);
                    }
                    };
                    ***/

                    Browser.LibraryCustomerFowst.InputData.OrgID = vParams.OrgID;
                    Browser.LibraryCustomerFowst.Click = function (e) {
                        vParams.Click(e);
                    };
                    Browser.LibraryCustomerFowst.Show();
                }; // END this.BrowseCustomerAddress = function(vParams) {

                // commented 2024_12_04__15_15_09_7498
                //var mHandleEnforceZReadInterval;
                //var mHandleEnforceZReadLastCheck;
                //function SetHandleEnforceZReadLoop(vParams) {
                //    if (!CBool(vParams.IsFOWSTInsideAMall))
                //        return;
                //    if (vParams.FOWSTMallName != 'Robinson')
                //        return;
                //    if (!CBool(Transfers.ReadFile('enable rlc.txt')))
                //        return;
                //    if (mHandleEnforceZReadInterval != null)
                //        return;
                //    mHandleEnforceZReadInterval = setInterval(function() {
                //        var pNow = FormatDate(new Date(), {Format: 'yyyy-MM-dd HH:mm'});
                //        var pLastChecked = FormatDate(mHandleEnforceZReadLastCheck, {Format: 'yyyy-MM-dd HH:mm'});
                //        if (pNow == pLastChecked)
                //            return;
                //        mThis.HandleEnforceZRead(vParams);
                //    }, 1000);
                //}

                this.HandleEnforceZRead = function(vParams) {
                    // this function requires current private database
                    //vParams = {
                    //    silent: boolean (optional)
                    //    callback: function() {}
                    //};

                    vParams = $.extend({
                        FOWSTMallName: null,
                        DontKick: false // if true, prevents: 'This store is inside a Robinsons Mall. Cashiering transactions are not accepted from 4:00 AM to 9:00 AM.'
                    }, vParams);
                    //mHandleEnforceZReadLastCheck = new Date();
                    //SetHandleEnforceZReadLoop(vParams);
                    var pCallback = vParams.callback;
                    if ($.isFunction(pCallback) == false)
                        pCallback = function() {};
                    if (!mThis.HasCorrectDate) {
                        pCallback(false);
                        return false;
                    }

                    // 2026-02-07 always zread date na - for BIR demo purposes only
                    //gi uncomment balik para dili sigeg popup sa mga store na wala naga enforce zread - by Arnie 050126
                    if (!Business.SystemDefaults.GetEnforceZRead() && !GetEnforceZReadByFOWST()) {
                        // orginal startclock('#clockHotButtons, #clock', '');
                        startclock('#clockHotButtons, #clock', 'C ');
                        pCallback(true);
                        return true;
                    } 
                    startclock('#clockHotButtons, #clock', 'Z ');

                    var pZReadDate = mThis.GetZReadDay();
                    proj.SetZReadDate(pZReadDate);

                    var pToday = RemoveTime(new Date());
                    var pStr;

                    //if (CBool(vParams.IsFOWSTInsideAMall)
                    //    && (vParams.FOWSTMallName == 'Robinson')
                    //    && CBool(Transfers.ReadFile('enable rlc.txt')))

                    //    HandleRobinson(Proceed);
                    //else
                        Proceed();

                    function Proceed() {
                        var pIsZReadDateInFuture = +pZReadDate > +pToday;
                        if (pIsZReadDateInFuture) {
                            pStr = 'The Z-Read for ' + $.format.date(pToday, 'MMM d, yyyy') + ' is already closed. You may use the POS starting at 12:00 AM on ' + $.format.date(pZReadDate, 'MMM d, yyyy') + '. Thank you!';
                            NewMessageBox(pStr);
                            return;
                        }

                        var pMustIncrement = +pZReadDate < +pToday;

                        if (pMustIncrement
                            && CBool(vParams.IsFOWSTInsideAMall)
                            && (vParams.FOWSTMallName == 'Robinson') 
                            // && CBool(Transfers.ReadFile('enable rlc.txt'))  // commented 2024_12_04__15_15_09_7498
                            && !CBool(vParams.Operates247)) {

                            if (+pToday - +pZReadDate == 24 * 60 * 60 * 1000) // zread date is 1 day behind
                                if (new Date().getHours() < 4)
                                    pMustIncrement = false;
                        } 

                        if (pMustIncrement) {
                            // orignal startclock('#clockHotButtons, #clock', 'Z ');
                            if (CBool(vParams.silent)) {
                                proj.SetZReadDate(pZReadDate);
                                // orignal startclock('#clockHotButtons, #clock', 'Z ');
                                pCallback(true);
                            } else {
                                pStr = '';
                                if (CBool(vParams.IsFOWSTInsideAMall)
                                    && (vParams.FOWSTMallName == 'Robinson')
                                    //&& CBool(Transfers.ReadFile('enable rlc.txt')) // commented 2024_12_04__15_15_09_7498
                                    ) { 

                                    mThis.CheckIfInsideTheMall(mThis.GetZReadDay());
                                    mThis.IncrementZReadDay();
                                    NewMessageBox('Previous’ day’s EOD was not performed', null, null, {
                                        Close: function() {
                                            SalesPaymentsExport_offline().FlushDeviceData({
                                                Success: function(vData) {
                                                    //Android.Toast('All data were successfully sent to the server.');
                                                    //var pData = {
                                                    //    FowstID: sessionStorage.FowstID,
                                                    //    DeviceID: Web.DeviceID(),
                                                    //    SalesDate: $.format.date(pZReadDate, 'yyyy/MM/dd')
                                                    //};
                                                    //Android.Toast('Data was successfully flushed. Sending RLC file to FTP server.');
                                                    //mThis.HandleEnforceZRead(vParams);    
                                                },
                                                Fail: function() {
                                                    //mThis.HandleEnforceZRead(vParams);                                                    
                                                }
                                            }); // END FlushDeviceData
                                            mThis.HandleEnforceZRead(vParams);    
                                        } // END Close: function() {
                                    });
                                } else {
                                    pStr = 'Your device date is ' + $.format.date(pToday, 'MMM d, yyyy') + ', but the Z-Read date is ' + $.format.date(pZReadDate, 'MMM d, yyyy') + '. To ensure your reporting remains accurate, the system will now close the Z-Read date and advance the business date.<br><br>What would you like to do?';
                                    NewMessageBox(pStr, 'Catch Up Daily Closing', 
                                        [
                                            // 2026-02-07 big no no ni sila para sa BIR
                                            {
                                                Text: 'Close ' + $.format.date(pZReadDate, 'MMM d') + ' and advance to ' + $.format.date(new Date(new Date(pZReadDate).setDate(new Date(pZReadDate).getDate() + 1)), 'MMM d'), //'Close the current Z-Read date and move date forward by 1 day',
                                                Click: function () {
                                                    mThis.CheckIfInsideTheMall(mThis.GetZReadDay());
                                                    mThis.IncrementZReadDay();
                                                    mThis.HandleEnforceZRead(vParams);
                                                    return
                                                    NewMessageBox('You will no longer be able to post sales and other transactions to the current Z-Read Date after closing.', '', [
                                                        {
                                                            Text: 'Yes',
                                                            Click: function () {
                                                                mThis.CheckIfInsideTheMall(mThis.GetZReadDay());
                                                                mThis.IncrementZReadDay();
                                                                mThis.HandleEnforceZRead(vParams);                                                    
                                                            }
                                                        },
                                                        {
                                                            Text: 'No',
                                                            Click: function() {
                                                                mThis.HandleEnforceZRead(vParams);
                                                            }
                                                        }
                                                    ]);
                                                }
                                            },
                                            {
                                                Text: 'Use ' + $.format.date(pZReadDate, 'MMM d, yyyy') + ' as the current date',
                                                Click: function() {
                                                    proj.SetZReadDate(pZReadDate);
                                                    // orignal startclock('#clockHotButtons, #clock', 'Z ');
                                                    pCallback(true);
                                                } 
                                            },
                                            {
                                                Text: 'Print Z-Read report for ' + $.format.date(pZReadDate, 'MMM d, yyyy'),
                                                Click: function () {
                                                    window.open(GetPathFormat("/ReportsPOS/ZReport") + "?OrgID=" + sessionStorage["OrgID"] + "&Online=1", "_self");
                                                    return
                                                    //by default ZRead lang sa ang option
                                                    PopupButtonList('Select which report to print', 'POS Sales Reports', [
                                                        {
                                                            Text: 'Cashiers Sales Report',
                                                            Click: function () {                                                    

                                                                var pData = {};
                                                                var isPageReady = false;
                                                                pData.OrgID = sessionStorage["OrgID"];
                                                                pData.POSSalesReportType = 0;

                                                                sessionStorage.POSSalesReportType = 0;

                                                                AjaxSend({
                                                                    url: 'ReportsPOS/setPOSSalesReportType',
                                                                    data: pData,
                                                                    success: function (vData) {
                                                                        require([
                                                                            GetPathFormat('promise!/Scripts/sources/ReportsPOS/SalesReport.js?crc=')
                                                                        ], function () {
                                                                            var pData = {};

                                                                            pData.RangeType = 'Day';
                                                                            pData.RangeFrom = new Date(proj.GetZReadDateSql());
                                                                            pData.RangeTo = new Date(proj.GetZReadDateSql());
                                                                            pData.RangeYear = 'Day';

                                                                            sessionStorage['ReportDate'] = Date(proj.GetZReadDateSql());
                                                                            getSalesReportData(pData, Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()));
                                                                        });
                                                                    }
                                                                });
                                                            }
                                                        },
                                                        {
                                                            Text: 'Managers Sales Report',
                                                            Click: function () {
                                               
                                                                var pData = {};
                                                                var isPageReady = false;
                                                                pData.OrgID = sessionStorage["OrgID"];
                                                                pData.POSSalesReportType = 1;

                                                                sessionStorage.POSSalesReportType = 1;

                                                                AjaxSend({
                                                                    url: 'ReportsPOS/setPOSSalesReportType',
                                                                    data: pData,
                                                                    success: function (vData) {
                                                                        require([
                                                                            GetPathFormat('promise!/Scripts/sources/ReportsPOS/SalesReport.js?crc=')
                                                                        ], function () {
                                                                            var pData = {};

                                                                            pData.RangeType = 'Day';
                                                                            pData.RangeFrom = new Date(proj.GetZReadDateSql());
                                                                            pData.RangeTo = new Date(proj.GetZReadDateSql());
                                                                            pData.RangeYear = 'Day';

                                                                            sessionStorage['ReportDate'] = Date(proj.GetZReadDateSql());
                                                                            getSalesReportData(pData, Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()), Date(proj.GetZReadDateSql()));
                                                                        });
                                                                    }
                                                                })
                                                    
                                                            }
                                                        },
                                                        {
                                                            Text: 'Z-Read',
                                                            Click: function () {
                                                                //    sessionStorage.PrintingOfflineData = true;
                                                                //    sessionStorage.OfflineType = 'zreport';
                                                                //    sessionStorage.removeItem('IsRequestAndApproval');
                                                                //    location = MergeUrlParameters({
                                                                //        OrgID: sessionStorage.OrgID,
                                                                //        IsMobile: CBool(IsMobile) ? 1 : 0
                                                                //    }, '/DashBoardsAndReports');
                                                                //}

                                                                window.open(GetPathFormat("/ReportsPOS/ZReport") + "?OrgID=" + sessionStorage["OrgID"] + "&Online=1", "_self");

                                                                //require([
                                                                //     GetPathFormat('promise!/Scripts/sources/ReportsPOS/ZReport.js?v=' + assetversion)
                                                                //], function () {
                                                                //    sessionStorage.PrintingOfflineData = true; //para sa offline database sya magkuha ug zread data just incase naa pa unflushed data sa device
                                                                //    PrintSalesZReport(false);
                                                                //    sessionStorage.PrintingOfflineData = false; 
                                                                //});
                                                            }
                                                        }
                                                    ])
                                                }
                                            }
                                        ]
                                    , {AutoDismiss: false}); // END NewMessageBox(pStr, 'Closing Date', [
                                }
                            } // END if (CBool(vParams.silent)) {
                        } else { // ELSE pMustIncrement
                            // orignal startclock('#clockHotButtons, #clock', 'C ');
                            if (CBool(vParams.IsFOWSTInsideAMall)
                                && (vParams.FOWSTMallName == 'Robinson')
                                // && CBool(Transfers.ReadFile('enable rlc.txt')) // commented 2024_12_04__15_15_09_7498
                                ) {

                                if (!vParams.DontKick) {
                                    var pHours = new Date().getHours();
                                    if ((pHours >= 4)
                                        && (pHours < 9)) {

                                        NewMessageBox('This store is inside a Robinsons Mall. Cashiering transactions are not accepted from 4:00 AM to 9:00 AM.', '', null, {
                                            AutoDismiss: false,
                                            Close: function() {
                                                location = MergeUrlParameters({
                                                    IsMobile: CBool(IsMobile) ? 1 : ''
                                                }, '/People/PeopleMenu');
                                                pCallback(false);
                                            }
                                        });
                                        return;
                                    } else if (!AllowToPos())
                                        return;
                                } else
                                    pCallback(true);

                                Reports.Robinsons.SendUnsentLogs({
                                    Callback: function() {
                                        setTimeout(function() {
                                            mThis.HandleEnforceZRead(vParams);    
                                        }, 5000);
                                    }
                                });
                            }
                            pCallback(true);
                        } // END pMustIncrement
                    } // END function Proceed()

                    function GetEnforceZReadByFOWST() {
                        var pSql = 'Select EnforceZRead from LibraryOrganizationFOWST WHERE ID = ' + ReadyString(sessionStorage.FowstID);

                        return CBool(Sql.ExecuteScalar(pSql));
                    }
                }; // END HandleEnforceZRead

                mThis.HandleRobinson = HandleRobinson;
                function HandleRobinson(vCallback) {

                    //Reports.Robinsons.SendUnsentLogs({
                    //    Callback: vCallback
                    //});

                    // BEGIN commented: 2024_08_26__06_20_41_2483
                    //var pData = {
                    //    FowstID: sessionStorage.FowstID,
                    //    DeviceID: Web.DeviceID()
                    //};

                    //GetStats();

                    //function GetStats() {
                    //    AjaxSend({
                    //        url: 'MallLessorMegaworld/GetStats',
                    //        data: pData,
                    //        success: function(vData) {
                    //            if (vData.Error) {
                    //                NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor.', 'Server Error', null, {
                    //                    Close: function() {
                    //                        vCallback();
                    //                    }
                    //                });
                    //            } 
                            
                    //            var pRlcDisconnected = CBool(Transfers.ReadFile('RlcDisconnected.txt'));
                    //            if (pRlcDisconnected) {
                    //                NewMessageBox('Trying to send unsent files…successful', '', null, {
                    //                    Close: function() {
                    //                        Transfers.WriteFile('RlcDisconnected.txt', null, '0');
                    //                        Proceed();
                    //                    }
                    //                });
                    //            } else
                    //                Proceed();

                    //            function Proceed() {
                    //                mThis.UpdateRlcReportedStats(vData.Results);

                    //                if (CInt2(GetStringObject('Results.UnreportedCount', vData)) > 0) {
                    //                    Resend();
                    //                } else {
                    //                    var pStats = mThis.GetRlcReportedStats();
                    //                    if ((CInt2(pStats.ReportedCountConfirmed) != CInt2(vData.Results.ReportedCount))
                    //                        || (pStats.ReportedMaxSalesDateConfirmed != vData.Results.ReportedMaxSalesDate)) {

                    //                        NewMessageBox('Sales file successfully sent to RLC server.', 'Success', null, {
                    //                            Close: function() {
                    //                                pStats.ReportedCountConfirmed = vData.Results.ReportedCount;
                    //                                pStats.ReportedMaxSalesDateConfirmed = vData.Results.ReportedMaxSalesDate;
                    //                                mThis.UpdateRlcReportedStats(pStats);
                    //                                vCallback();
                    //                            }
                    //                        });
                    //                    } else
                    //                        vCallback();
                    //                }
                    //            }
                    //        },
                    //        error: function(vError) {
                    //            Transfers.WriteFile('RlcDisconnected.txt', null, '1');
                    //            // just ignore any error here
                    //            //NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor. ' + vError, '', null, {
                    //            //    Close: function() {
                    //            //        vCallback();
                    //            //    }
                    //            //});
                    //            vCallback();
                    //        }
                    //    });
                    //}

                    //function Resend() {
                    //    AjaxSend({
                    //        url: 'MallLessorMegaworld/Resend',
                    //        data: pData,
                    //        success: function(vData) {
                    //            if (vData.Error) {
                    //                NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor.', 'Server Error', null, {
                    //                    Close: function() {
                    //                        vCallback();
                    //                    }
                    //                });
                    //            } else if ((vData.Results == 'ok')
                    //                && (CStr2(vData.Filename) != '')) {

                    //                GetStats();
                    //                NewMessageBox('Sales file successfully sent to RLC server.', 'Success', null, {
                    //                    Close: function() {
                    //                        vCallback();
                    //                    }
                    //                });
                    //            } else
                    //                vCallback();
                    //        },
                    //        error: function(vError) {
                    //            NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor.', '', null, {
                    //                Close: function() {
                    //                    vCallback();
                    //                }
                    //            });
                    //        }
                    //    });
                    //}
                    // END commented: 2024_08_26__06_20_41_2483
                } // END function HandleRobinson(vCallback)

                this.CheckIfInsideTheMall = function (curZReadDate) {
                    var pSql = '';
                    var LastTransactionID = '';
                    var pdtMall = '';

                    pSql = 'Select IsFOWSTInsideAMall, FOWSTMallName from LibraryOrganizationFOWST WHERE  ID=' + ReadyString(sessionStorage.FowstID)

                    pdtMall = Sql.Execute(pSql);

                    if (pdtMall.Rows.length > 0) {
                        if (CBool(pdtMall.Rows[0].IsFOWSTInsideAMall)) {

                            pSql = 'Select ID from private.Transinvoice WHERE InvoiceNumber <> 0 AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' AND FOWSTID=' + ReadyString(sessionStorage.FowstID) + ' AND date(Invoicedate) = ' + ReadyString($.format.date(new Date(curZReadDate), 'yyyy-MM-dd')) + ' ORDER BY InvoiceDate desc limit 1'

                            LastTransactionID = Sql.ExecuteScalar(pSql);

                            var pIU = new InsertUpdate('TenantMonitoringLogs');

                            pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                            pIU.Add('SalesDate', ReadyString($.format.date(new Date(curZReadDate), 'yyyy-MM-dd')));
                            pIU.Add('Mall', ReadyString(CStr2(pdtMall.Rows[0].FOWSTMallName)));
                            pIU.Add('LastTransactionID', (CStr2(LastTransactionID) == '' ? "NULL" : ReadyString(LastTransactionID)));
                            pIU.Add('LastUpdated', proj.SqliteNow);
                            pIU.Add('UserID', ReadyString(sessionStorage.PeopleID));
                            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));

                            var pID = Sql.ExecuteScalar("SELECT ID FROM TenantMonitoringLogs WHERE DeviceID = " + ReadyString(Web.DeviceID()) + " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) + " AND DATE(SalesDate) = " + ReadyString($.format.date(new Date(curZReadDate), 'yyyy-MM-dd')) + " AND Mall = " + ReadyString(CStr2(pdtMall.Rows[0].FOWSTMallName)));
                            if (pID != undefined) {
                                pIU.Add('DateImported', "NULL");
                                pIU.Add('NextPrintedDate', proj.SqliteNow);
                                pIU.Add('NextPrintedBy', ReadyString(sessionStorage.PeopleID));
                                pSql = pIU.GetUpdate(" ID = " + ReadyString(pID));
                            } else {
                                pIU.Add('EndOfDayCounter', '(SELECT IFNULL(MAX(EndOfDayCounter), 0) + 1\n' +
                                    '   FROM TenantMonitoringLogs WHERE FOWSTID = ' + ReadyString(sessionStorage.FowstID) + '\n' +
                                    '   AND DeviceID = ' + ReadyString(Web.DeviceID()) + ')');
                                pIU.Add('ID', ReadyString(NewID()));
                                pIU.Add('FirstPrintedDate', proj.SqliteNow);
                                pIU.Add('FirstPrintedBy', ReadyString(sessionStorage.PeopleID));
                                pSql = pIU.GetInsert();
                            }

                            Sql.Execute(pSql);
                        }
                        return pdtMall.Rows[0];
                    }
                }

                this.GetZReadDay = function() {
                    if (!IsGuid(sessionStorage.OrgID)) {
                        throw 'sessionStorage.OrgID must be a valid GUID before calling GetZReadDay. jsWZ3WVGHB7Y!';
                    }

                    if (!IsGuid(sessionStorage.FowstID)) {
                        throw 'sessionStorage.FowstID must be a valid GUID before calling GetZReadDay. jsFXQVFSTGHL!';
                    }

                    var pData = mThis.LoadZReadDay();
                    try {
                        pData = JSON.parse(pData);
                    } catch (e) {
                        pData = {};
                    }
                    var pDate = GetStringObject(ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID), pData);
                    if (IsDate(pDate)) {
                        
                    } else {
                        // pDate = Android.AppInstallDate();
                        pDate = new Date();
                        var pPath = ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID);
                        CreateStringObject(pPath, pData, pDate);
                        mThis.SaveZReadDay(pData);
                    }
                    pDate = RemoveTime(pDate);

                    return pDate;
                };

                function AllowToPos(vCallback) {
                    var pSql = 'SELECT TenantMonitoringLogs.SalesDate ' + '\n' + 
                        ' ,cachezread.CurGrossSales' + '\n' + 
                        ' ,TenantMonitoringLogs.FileStatus' + '\n' + 
                        ' FROM TenantMonitoringLogs' + '\n' + 
                        ' LEFT JOIN cache.cachezread ON cachezread.SalesDate = TenantMonitoringLogs.SalesDate' + '\n' + 
                        ' AND cachezread.FOWSTID = TenantMonitoringLogs.FOWSTID' + '\n' + 
                        ' AND cachezread.DeviceID = TenantMonitoringLogs.DeviceID' + '\n' + 
                        ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TenantMonitoringLogs.FowstID' + '\n' + 
                        ' AND LibraryOrganizationFOWST.IsFOWSTInsideAMall = 1' + '\n' + 
                        ' AND LibraryOrganizationFOWST.FOWSTMallName = \'Robinson\'' + '\n' + 
                        ' WHERE TenantMonitoringLogs.FOWSTID = ' + ReadyString(sessionStorage.FowstID) + '\n' + 
                        ' AND TenantMonitoringLogs.DeviceID = ' + ReadyString(Web.DeviceID()) + '\n' +
                        ' AND IFNULL(cachezread.CurGrossSales, 0) = 0' + '\n' + 
                        ' AND TenantMonitoringLogs.FileStatus <> \'ftp-send\'' + '\n' + 
                        ' ORDER BY TenantMonitoringLogs.SalesDate';
                    var pDR = Sql.ExecuteRow(pSql);
                    if (pDR == null)
                        return true; // vCallback(true);
                    else {
                        var pMsg = 'Per Robinson\'s guidelines, you cannot use the POS on days when it was not previously used and there is an unsent file. An unsent file has been detected for ';
                        pMsg += $.format.date(new Date(pDR.SalesDate), 'MMMM d, yyyy') +  '. Please send this file before proceeding.';
                        NewMessageBox(pMsg, 'POS Access Denied', null, {
                            Close: function() {
                                location = MergeUrlParameters({
                                    IsMobile: CBool(IsMobile) ? 1 : ''
                                }, '/People/PeopleMenu');
                            }
                        });
                        return false;
                    }
                }

                this.LoadZReadDay = function() {
                    return Transfers.ReadFile('ZReadDay.txt');
                };

                this.IncrementZReadDay = function(vParams) {
                    vParams = $.extend({
                        DontLimit: false,
                        LimitTomorrow: false,
                        TargetDate: null,
                        DontAlert: false
                    }, vParams);
                    var pDate;
                    if (vParams.TargetDate == null)
                        pDate = mThis.GetZReadDay();
                    else
                        pDate = ParseDate(vParams.TargetDate);
                    var pCurrentDate = RemoveTime(new Date());
                    if (pCurrentDate <= pDate) {
                        if (vParams.LimitTomorrow)  {
                            var pDaysDiff = DateDiff(pCurrentDate, pDate);
                            if (pDaysDiff > 0)
                                return;
                        } else if (!CBool(vParams.DontLimit)) {

                            if (!vParams.DontAlert) {
                                var pMsg = 'Cannot increment Z-Read Date because the Z-Read Date (' + FormatDate(pDate) + ') is already equal to or later than the device date (' + FormatDate(pCurrentDate) + '.';
                                alert(pMsg);
                            }
                            return;
                        } 
                    }
                    if (vParams.TargetDate == null)
                        pDate.setDate(pDate.getDate() + 1);
                    pDate = FormatDate(pDate);
                    var pData = mThis.LoadZReadDay();
                    try {
                        pData = JSON.parse(pData);
                    } catch (e) {
                        pData = {};
                    }
                    var pPath = ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID);
                    CreateStringObject(pPath, pData, pDate);
                    mThis.SaveZReadDay(pData);
                };

                this.SaveZReadDay = function(vData) {
                    Transfers.WriteFile('ZReadDay.txt', null, JSON.stringify(vData, null, 4));
                };

                ///

                this.GetRlcDate = function() {
                    if (!IsGuid(sessionStorage.OrgID)) {
                        throw 'sessionStorage.OrgID must be a valid GUID before calling GetRlcDate. js66FK3KXLG2!';
                    }

                    if (!IsGuid(sessionStorage.FowstID)) {
                        throw 'sessionStorage.FowstID must be a valid GUID before calling GetRlcDate. js982QPKVJKG!';
                    }

                    var pData = mThis.LoadRlcDate();
                    try {
                        pData = JSON.parse(pData);
                    } catch (e) {
                        pData = {};
                    }
                    var pDate = GetStringObject(ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID), pData);
                    if (IsDate(pDate)) {
                        pDate = RemoveTime(pDate);
                    } else {
                        // no default date
                    }

                    return pDate;
                };

                this.LoadRlcDate= function() {
                    return Transfers.ReadFile('RlcDate.txt');
                };

                this.SaveRlcDate = function(vData) {
                    Transfers.WriteFile('RlcDate.txt', null, JSON.stringify(vData, null, 4));
                };

                this.GetRlcDate = function() {
                    if (!IsGuid(sessionStorage.OrgID)) {
                        throw 'sessionStorage.OrgID must be a valid GUID before calling GetRlcDate. js66FK3KXLG2!';
                    }

                    if (!IsGuid(sessionStorage.FowstID)) {
                        throw 'sessionStorage.FowstID must be a valid GUID before calling GetRlcDate. js982QPKVJKG!';
                    }

                    var pData = mThis.LoadRlcDate();
                    try {
                        pData = JSON.parse(pData);
                    } catch (e) {
                        pData = {};
                    }
                    var pDate = GetStringObject(ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID), pData);
                    if (IsDate(pDate)) {
                        pDate = RemoveTime(pDate);
                    } else {
                        // no default date
                    }

                    return pDate;
                };

                ///

                this.LoadRlcReportedStats = function(vConfirmed) {
                    return Transfers.ReadFile('RlcReportedStats.txt');
                };

                this.SaveRlcReportedStats = function(vData) {
                    Transfers.WriteFile('RlcReportedStats.txt', null, JSON.stringify(vData, null, 4));
                };

                this.GetRlcReportedStats = function() {
                    if (!IsGuid(sessionStorage.OrgID)) {
                        throw 'sessionStorage.OrgID must be a valid GUID before calling GetRlcReportedStats. jsVJAZDSCX8T!';
                    }

                    if (!IsGuid(sessionStorage.FowstID)) {
                        throw 'sessionStorage.FowstID must be a valid GUID before calling GetRlcReportedStats. js99V3UU6V6W!';
                    }

                    var pDeviceID = Web.DeviceID();
                    if (!IsGuid(pDeviceID)) {
                        throw 'Web.DeviceID() must be a valid GUID before calling GetRlcReportedStats. jsJ6CJ3Y9ELE!';
                    }
                    pDeviceID = ReadyGuid(pDeviceID);

                    var pData = mThis.LoadRlcReportedStats();
                    try {
                        pData = JSON.parse(pData);
                    } catch (e) {
                        pData = {};
                    }
                    var pOut = GetStringObject(ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID) + '.' + pDeviceID, pData);

                    return pOut;
                };

                this.UpdateRlcReportedStats = function(vData) {
                    var pData = mThis.LoadRlcReportedStats();
                    try {
                        pData = JSON.parse(pData);
                    } catch (e) {
                        pData = {};
                    }
                    var pDeviceID = Web.DeviceID();
                    if (!IsGuid(pDeviceID)) {
                        throw 'Web.DeviceID() must be a valid GUID before calling UpdateRlcReportedStats. jsTFEMEHX9F5!';
                    }
                    pDeviceID = ReadyGuid(pDeviceID);
                    var pPath = ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID) + '.' + pDeviceID;
                    var pBranch = GetStringObject(pPath, pData);
                    $.extend(pBranch, vData);
                    CreateStringObject(pPath, pData, pBranch);
                    mThis.SaveRlcReportedStats(pData);
                }   

                this.TestPrice = function(vParams) {
                    /*
                    vParams = {
                        Callback: function() {
                            // call after user closed the messagebox
                        }
                    }
                    */

                    vParams = $.extend({}, vParams);
                    if (!$.isFunction(vParams.Callback))
                        vParams.Callback = function() {};

                    if (sessionStorage.CartOrderID == null) {
                        NewMessageBox('Cart must have at least 1 item', null, 
                            [
                                {
                                    Text: 'Ok',
                                    Click: vParams.Callback
                                }
                            ]);
                        return;
                    }
                    Business.TransInvoice.TestPrice({
                        ID: sessionStorage.CartOrderID,
                        Callback: function(vParams2) {
                            var pMessage = 'Your price request has ';

                            if (vParams2.IsOk)
                                pMessage += '';
                            else
                                pMessage += 'not ';
                            pMessage += 'been approved. ' + vParams2.Message;
                            NewMessageBox(pMessage, null, 
                                [
                                    {
                                        Text: 'Ok',
                                        Click: function() {
                                            if (this.vParams2.IsOk)
                                                this.vParams.Callback();
                                        },
                                        vParams: vParams,
                                        vParams2: vParams2
                                    }
                                ]);
                        }
                    });
                };

                this.PoDefaultSupplierAndCustomer = function() {
                    
                    var pSupplierSetting;
                    var pSupplierOrgID;
                    var pSupplierOrgName;
                    var pSupplierFowstID;
                    var pSupplierFowstName;

                    var pCustomerSetting;
                    var pCustomerOrgID;
                    var pCustomerOrgName;
                    var pCustomerFowstID;
                    var pCustomerFowstName;

                    var pDR;

                    pSupplierSetting = Business.SystemDefaults.PurchaseOrderSupplierSetting();
                    if ((pSupplierSetting == 'locked to')
                        || (pSupplierSetting == 'default to')) {

                        pSupplierOrgID = Business.SystemDefaults.PurchaseOrderSupplierID();
                        if (IsGuid(pSupplierOrgID)) {
                            pDR = Business.LibraryOrganization.ByID(pSupplierOrgID);
                            if (pDR != null)
                                pSupplierOrgName = pDR.CompanyName;
                        }

                        pSupplierFowstID = Business.SystemDefaults.PurchaseOrderSupplierFowstID();
                        if (IsGuid(pSupplierFowstID)) {
                            pDR = Business.LibraryOrganizationFOWST.ByID(pSupplierFowstID);
                            if (pDR != null)
                                pSupplierFowstName = pDR.FowstName;
                        }
                    }

                    pCustomerSetting = Business.SystemDefaults.PurchaseOrderCustomerSetting();
                    if ((pCustomerSetting == 'locked to')
                        || (pCustomerSetting == 'default to')) {

                        pCustomerOrgID = Business.SystemDefaults.PurchaseOrderCustomerID();
                        if (IsGuid(pCustomerOrgID)) {
                            pDR = Business.LibraryOrganization.ByID(pCustomerOrgID);
                            if (pDR != null)
                                pCustomerOrgName = pDR.CompanyName;
                        }

                        pCustomerFowstID = Business.SystemDefaults.PurchaseOrderCustomerFowstID();
                        if (IsGuid(pCustomerFowstID)) {
                            pDR = Business.LibraryOrganizationFOWST.ByID(pCustomerFowstID);
                            if (pDR != null)
                                pCustomerFowstName = pDR.FowstName;
                        }
                    }

                    return {
                        SupplierOrgID: pSupplierOrgID,
                        SupplierOrgName: pSupplierOrgName,
                        SupplierFowstID: pSupplierFowstID,
                        SupplierFowstName: pSupplierFowstName,

                        CustomerOrgID: pCustomerOrgID,
                        CustomerOrgName: pCustomerOrgName,
                        CustomerFowstID: pCustomerFowstID,
                        CustomerFowstName: pCustomerFowstName
                    };
                }; // END PoDefaultSupplierAndCustomer

                this.SaveCustomer = function(vOrgID) {
                    if (!IsGuid(vOrgID))
                        return;
                    if (!IsGuid(sessionStorage.getItem('CartOrderID')))
                        return;
                    var pIU = new InsertUpdate('TransInvoice');
                    pIU.Add('CustomerID', ReadyString(vOrgID));
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    if (!CBool(sessionStorage.getItem('IsOrderTaker')))
                        pIU.Add('DateImported', 'null');
                    var pSql = pIU.GetUpdate('ID = ' + ReadyString(sessionStorage.getItem('CartOrderID')));
                    pSql = Sql.ExecuteSql(pSql); // output is used to debugging only
                };

                this.TransferTable = function(vInvoiceID, vToTableNo) {
                    vToTableNo = CStr2(vToTableNo);
                    if (vToTableNo.indexOf('TBL_') != 0)
                        vToTableNo = 'TBL_' + vToTableNo;
                    var pIU = new InsertUpdate('TransInvoice');
                    pIU.Add('CustomerTableNo', ReadyString(vToTableNo));
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    if (!CBool(sessionStorage.getItem('IsOrderTaker')))
                        pIU.Add('DateImported', 'null');
                    var pSql = pIU.GetUpdate('ID = ' + ReadyString(vInvoiceID));
                    pSql = Sql.ExecuteSql(pSql); // output is used to debugging only
           
                };

                return this;

            }; // SalesMenu_offline
        } else { // END if (window.SalesMenu_offline == undefined) {
            
        } // END  // END if (window.SalesMenu_offline == undefined) {
    });