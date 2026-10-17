define(['/Scripts3/Browser/LibraryOrganizationEmployeeFOWST.js?crc=', // dont append crc. else, wont load
    'promise!/Scripts3/Browser/TransBomMaker.js?crc='

    // 2016_08_18__12_28_17_5879
    // SalesPayments_offline may not be needed
    // '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc='

    ], function () {

        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts3/view/BomMakerMake.html',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    mDeferred.resolve();
                }
            });

        })();

        $(function () {

            CreateStringObject('App.controller.BomMakerMake', null, function () {

                var mOut = {};
                var mElement;
                var mData;
                var mComboInited = false;
                var mAssignedTo; // PMT Assigned to:
                var mAssignedToID; // PMT Assigned to:

                mOut.id = null; // the current transaction ID. if null, the next save is considered a new record.
                                // null this if you intend to insert a new record

                mOut.model = new App.model.BomMaker();

                mElement = $(mHtml);
                mOut.element = mElement;

                function CacheHtml(vCallback) {

                    if (mHtml == undefined) {

                    } else
                        vCallback();

                } // END function CacheHtml() {

                mOut.popup = function () {

                    var pElement;

                    if (mElement.parent().length == 0) {
                        $.mobile.activePage.append(mElement);
                        $.mobile.activePage.trigger('create');
                    }

                    mElement.popup('open');
                    pElement = mElement.closest('.ui-popup-container');
                    if (pElement.length == 0)
                        console.trace('popup container for divRequestPrice not found');
                    else
                    // known issue: pop up dimension will revert to default after browser window resize
                        pElement.css({
                            left: 0,
                            width: '80%',
                            margin: 'auto 10%',
                            padding: 15
                        });
                };

                mOut.popupClose = function () {

                    mElement.popup('close');

                };

                mOut.txtAssignedTo = mElement.find('[name=txtAssignedTo]');
                mOut.txtDateNeeded = mElement.find('[name=txtDateNeeded]');

                mOut.txtAssignedTo.closest('form').submit(function (e) {
                    e.preventDefault();
                    Browser.LibraryOrganizationEmployeeFOWST.Click = function (e) {
                        mAssignedTo = e.PeopleName;
                        mOut.txtAssignedTo.val(mAssignedTo);
                        mAssignedToID = e.ID;
                        setTimeout(mOut.popup, 100);
                    }; // END Browser.LibraryOrganizationEmployeeFOWST.Click
                    Browser.LibraryOrganizationEmployeeFOWST.Show(mOut.txtAssignedTo.val());
                });

                mElement.find('[name=butSave]').click(function (e) {
                    e.preventDefault();
                    finalSaveAndPrint();
                    return false;
                });

                mElement.find('[name=butSaveAndPrint]').click(function (e) {
                    e.preventDefault();
                    finalSaveAndPrint(true);
                    return false;
                });

                function finalSaveAndPrint(vPrint) {

                    mOut.save({
                        id: mOut.id,
                        success: function () {
                            NewMessageBox('The transaction was saved successfully in the device.', 'Saved', [
                                {
                                    Text: 'Ok',
                                    Click: function() {
                                        proj.NewCartBOM();
                                        location.reload();
                                    }
                                }
                            ]);

                            if (vPrint) {
                                mOut.model.ticket({
                                    id: mOut.id,
                                    success: function (vData) {
                                        //var pData = vData.replace(/\n/g, '<br />');
                                        //MessageBox(pData, 'Printing...', null, {
                                        //    EncodeHtml: false
                                        //});
                                        var intFAFCTotalLenght = 42;
                                        var strTemp = '';
                                        var strPrint = '';
                                        var pDrContent;
                                        var strPrinterIP;
                                        var pDtPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();

                                        if (CDec2(GetStringObject('Rows.length', pDtPrint)) == 0) {
                                            MessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                                                "Printer",
                                                [
                                                    { Text: 'Ok'
                                                    }
                                                ]
                                            );
                                            return;
                                        }
                                        strPrinterIP = pDtPrint.Rows[0].PrinterIPAddress;

                                        NewPrintBuilder();
                                        var pFormattedNum = $.format.number(CDec2(vData.Header[0].Num), '000000');

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim("Product Maker Ticket #" + pFormattedNum), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 0, 0, 0, 0);

                                        strPrint = '\n';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('Date/Time Issued :', 0, 18), 18, $.trim($.format.date(new Date((vData.Header[0].LastUpdated || '')), proj.DateFormat)), intFAFCTotalLenght, '', 0);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('PMT Issued By    :', 0, 18), 18, $.trim((vData.Header[0].peopleUserFullName || '--')), intFAFCTotalLenght, '', 0);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('PMT Assigned To  :', 0, 18), 18, $.trim((vData.Header[0].peopleAssignedToFullName || '--')), intFAFCTotalLenght, '', 0);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strPrint = '\n';

                                        if (vData.Header[0].DateNeeded != undefined) {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('Date/Time Needed :', 0, 18), 18, $.trim($.format.date(new Date((vData.Header[0].DateNeeded)), proj.DateFormat)), intFAFCTotalLenght, '', 0);
                                            strPrint = strPrint + strTemp;
                                        }
                                        else {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('Date/Time Needed :', 0, 18), 18, $.trim('--'), intFAFCTotalLenght, '', 0);
                                            strPrint = strPrint + strTemp;
                                        }

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        for (var iBody = 0; iBody < vData.Body.length; iBody++) {
                                            strPrint = '\n';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('__________________________________________', 0, 42), 42, '', 0, '', 0);
                                            strPrint = strPrint + strTemp;
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strPrint = '\n';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim("PLEASE MAKE:"), intFAFCTotalLenght, '', 0, '', 0, 1);
                                            strPrint = strPrint + strTemp;
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                            for (var iMake = 0; iMake <= vData.Body[iBody].Rows.length - 1; iMake++) {
                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vData.Body[iBody].Rows[iMake].ParentQuantity + ' ' + vData.Body[iBody].Rows[iMake].ParentUnitMeasure + ' ' + vData.Body[iBody].Rows[iMake].ParentProductName, intFAFCTotalLenght, '', 0, '', 0);
                                                strPrint = strPrint + strTemp;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vData.Body[iBody].Rows[iMake].ParentProductNo, intFAFCTotalLenght, '', 0, '', 0);
                                                strPrint = strPrint + strTemp;

                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('__________________________________________', 0, 42), 42, '', 0, '', 0);
                                                strPrint = strPrint + strTemp;
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim("BILL OF MATERIALS:"), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                strPrint = strPrint + strTemp;
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                strPrint = '\n';

                                                for (var pContentIndex = 0; pContentIndex <= vData.Body[iBody].Rows[iMake].Contents.length - 1; pContentIndex++) {
                                                    pDrContent = vData.Body[iBody].Rows[iMake].Contents[pContentIndex];
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, pDrContent.ContentQuantity + ' ' + pDrContent.ContentUnitMeasure + ' ' + pDrContent.ContentProductName + ' ' + pDrContent.ContentProductNo + '\n', intFAFCTotalLenght, '', 0, '', 0);
                                                    strPrint = strPrint + strTemp;
                                                }
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim("INSTRUCTIONS:"), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                strPrint = strPrint + strTemp;
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vData.Body[iBody].Rows[iMake].BomInstruction, intFAFCTotalLenght, '', 0, '', 0);
                                                strPrint = strPrint + strTemp;
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 1);
                                            }
                                        }

                                        PrintText(strPrinterIP);
                                        ClearCommandBuffer();

                                        NewPrintBuilder();

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim("Materials Request Ticket #" + pFormattedNum), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 0, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim("Maker Ticket (PMT) #" + pFormattedNum), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strPrint = '\n';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('MRT Date/Time :', 0, 19), 19, $.trim($.format.date(new Date((vData.Header[0].LastUpdated || '')), proj.DateFormat)), intFAFCTotalLenght, '', 0);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('MRT Issued By    :', 0, 19), 19, $.trim((vData.Header[0].peopleUserFullName || '--')), intFAFCTotalLenght, '', 0);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('PMT Assigned To  :', 0, 19), 19, $.trim((vData.Header[0].peopleAssignedToFullName || '--')), intFAFCTotalLenght, '', 0);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strPrint = '\n';

                                        if (vData.Header[0].DateNeeded != undefined) {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('Product Needed by :', 0, 19), 19, $.trim($.format.date(new Date((vData.Header[0].DateNeeded)), proj.DateFormat)), intFAFCTotalLenght, '', 0);
                                            strPrint = strPrint + strTemp;
                                        }
                                        else {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('Product Needed by :', 0, 19), 19, $.trim('--'), intFAFCTotalLenght, '', 0);
                                            strPrint = strPrint + strTemp;
                                        }

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        for (var iBody = 0; iBody < vData.Body.length; iBody++) {
                                            strPrint = '\n';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('__________________________________________', 0, 42), 42, '', 0, '', 0);
                                            strPrint = strPrint + strTemp;
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strPrint = '\n';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim("WE WILL MAKE:"), intFAFCTotalLenght, '', 0, '', 0, 1);
                                            strPrint = strPrint + strTemp;
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                            for (var iMake = 0; iMake <= vData.Body[iBody].Rows.length - 1; iMake++) {
                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vData.Body[iBody].Rows[iMake].ParentQuantity + ' ' + vData.Body[iBody].Rows[iMake].ParentUnitMeasure + ' ' + vData.Body[iBody].Rows[iMake].ParentProductName, intFAFCTotalLenght, '', 0, '', 0);
                                                strPrint = strPrint + strTemp;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vData.Body[iBody].Rows[iMake].ParentProductNo, intFAFCTotalLenght, '', 0, '', 0);
                                                strPrint = strPrint + strTemp;

                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetTextAlign('__________________________________________', 0, 42), 42, '', 0, '', 0);
                                                strPrint = strPrint + strTemp;
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                strPrint = '\n';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim("BILL OF MATERIALS:"), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                strPrint = strPrint + strTemp;
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                strPrint = '\n';

                                                for (var pContentIndex = 0; pContentIndex <= vData.Body[iBody].Rows[iMake].Contents.length - 1; pContentIndex++) {
                                                    pDrContent = vData.Body[iBody].Rows[iMake].Contents[pContentIndex];
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, pDrContent.ContentQuantity + ' ' + pDrContent.ContentUnitMeasure + ' ' + pDrContent.ContentProductName + ' ' + pDrContent.ContentProductNo + '\n', intFAFCTotalLenght, '', 0, '', 0);
                                                    strPrint = strPrint + strTemp;
                                                }
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                            }
                                        }

                                        strPrint = '\n';
                                        strTemp = FormatTextToPrint(40, $.trim("MRT Issued By:\n"), 40, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                        strPrint = '\n';
                                        strTemp = FormatTextToPrint(40, (vData.Header[0].peopleUserFullName || '______________') + '\n Logged-in User', 40, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strPrint = '\n';
                                        strTemp = FormatTextToPrint(40, $.trim("MRT Approved By:\n"), 40, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                        strPrint = '\n';
                                        strTemp = FormatTextToPrint(40, '______________ \n User Selected', 40, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                        PrintText(strPrinterIP);
                                        ClearCommandBuffer();
                                    }
                                }); // END mBomMaker.ticket({
                            } // END if (vPrint) {
                        },
                        error: function (vMessage) {
                            MessageBox(vMessage, null, null, {
                                OnClose: mOut.popup
                            });
                        }
                    }); // END mOut.save({

                    return false;
                }

                mOut.save = function (vParams) {
                    if (mOut.txtDateNeeded.val().trim() != '')
                        if (!IsDate(mOut.txtDateNeeded.val())) {
                            vParams.error('Date Needed must be a valid date.');
                            return;
                        }
                    mOut.model.save({
                        id: vParams.id,
                        assignedToID: mAssignedToID,
                        dateNeeded: mOut.txtDateNeeded.val(),
                        success: function (vData) {
                            vParams.success(vData);
                        },
                        error: function (vMessage) {
                            vParams.error(arguments);
                        }
                    }); // END mOut.model.save({
                }; // END mOut.save = function() {

                // this is the "Make" button
                mOut.butSave_Click = function(vParams) {
                    //vParams = {
                    //    id: 'guid', // if null, this transaction will be considered a new record
                    //    success: function(vData) {
                    //        vData = {
                    //            id: 'guid'
                    //        }
                    //    }
                    //}

                    var pProducts;
                    var pItem; // element of proj.CartStock
                    var pTotalQty = 0;

                    if (sessionStorage.FowstID == undefined) {
                        MessageBox('Please select your current FOWST location.',
                            'FOWST',
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    // if (CStr2(sessionStorage.CartOrderID) != '') { // this is the original condition
                    if (true) {
                        if (OfflineMode) {
                            if ((CDec2(GetStringObject('CartStock.length', proj)) == 0)
                                && (CDec2(GetStringObject('CartData.Stocks.Rows.length', proj)) == 0)) {

                                NewMessageBox('Please a put at least 1 item into the cart.');
                                return;
                            }
                            // create pProducts
                            pProducts = [];
                            for (var pIndex in proj.CartStock) {
                                if (!proj.CartStock.hasOwnProperty(pIndex)) 
                                    continue;

                                pItem = proj.CartStock[pIndex];
                                pProducts.push({
                                    productID: pItem.ProductID,
                                    qty: pItem.Quantity,
                                    unitMeasureID: pItem.UnitMeasureID
                                });
                                pTotalQty += CDec2(pItem.Quantity);
                            } // END for (var pIndex in proj.CartStock) {
                            if (pTotalQty > 0) {
                                mOut.model.save({
                                    id: mOut.id,
                                    products: pProducts,
                                    success: function(vData) {
                                        //vData = {
                                        //    status: 'added', // or
                                        //    status: 'updated'
                                        //};
                                        mOut.popup();
                                        mOut.id = vData.id;
                                        sessionStorage.CartOrderID = mOut.id;
                                        vParams.success({
                                            id: vData.id
                                        });
                                    },
                                    error: function(vMessage) {
                                        NewMessageBox(vMessage);
                                    }
                                });
                            } else { // ELSE     if (pTotalQty > 0) {
                                mOut.id = sessionStorage.CartOrderID;
                                mOut.popup();
                                vParams.success({
                                    id: sessionStorage.CartOrderID
                                });
                            } // END if (pTotalQty > 0) {
                        } else {
                            MessageBox('This feature is available in offline mode only.');
                        }
                    } else {
                        MessageBox('Your cart is empty',
                                    'Product Maker',
                                    [
                                        { Text: 'Ok' }
                                    ]
                            );
                        return;
                    }
                };
                
                function SaveFromMemory() {
                    for (var pIndex in proj.CartStock) {
                        if (proj.CartStock.hasOwnProperty(pIndex)) {
                            pData.Stocks = [];
                            pItem = proj.CartStock[pIndex];
                            pData.StockID = pItem.ProductID;
                            pData.ProductID = pItem.ProductID;
                            pStock = {
                                Qty: pItem.Quantity,
                                UnitQty: pItem.ActualQuantity,
                                UnitPrice: pItem.DiscountedPrice,
                                PriceLevelID: defaultPriceLevel,
                                UnitMeasure: pItem.UnitMeasure
                            };
                            pData.Stocks.push(pStock);
                            pData.SalesRep = [];
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
                            pResult = pStock_offline.UpdatePos(pData);
                            sessionStorage.CartOrderID = pResult.OrderID;
                            localStorage.NewCartMode = false;
                            pData.NewCartMode = false;
                            pData.OrderID = pResult.OrderID
                        }
                    }
                } // END SaveFromMemory

                //mOut.divRequestPriceMessage = mElement.find('[name=divRequestPriceMessage]');
                //mOut.formRequestedPrice = mElement.find('[name=formRequestedPrice]');
                //mOut.txtRequestedPrice = mElement.find('[name=txtRequestedPrice]');
                //mOut.txtRequestedPriceDiscountPercent = mElement.find('[name=txtRequestedPriceDiscountPercent]');
                //mOut.cbRequestedPriceBasePrice = mElement.find('[name=cbRequestedPriceBasePrice]');
                //mOut.txtRequestedPriceDiscountAmount = mElement.find('[name=txtRequestedPriceDiscountAmount]');
                //mOut.lnkSecurityDiscountedPrice = mElement.find('[name=lnkSecurityDiscountedPrice]');
                //mOut.lnkSecurityDiscountPercentage = mElement.find('[name=lnkSecurityDiscountPercentage]');
                //mOut.lnkSecurityDiscountAmount = mElement.find('[name=lnkSecurityDiscountAmount]');

                //mOut.txtRequestedPrice.bind('input', function () {
                //    var pThis = mOut.txtRequestedPrice;

                //    if (!IsNumeric(pThis.val()))
                //        return;

                //    mOut.model.setNetAmount(pThis.val());
                //    mOut.txtRequestedPriceDiscountPercent.val(mOut.model.discountPercent.toFixed(2));
                //    mOut.txtRequestedPriceDiscountAmount.val(mOut.model.discountAmount.toFixed(2));
                //});

                //mOut.txtRequestedPriceDiscountPercent.bind('input', function () {
                //    var pThis = $(this);

                //    if (!IsNumeric(pThis.val()))
                //        return;

                //    mOut.model.setDiscountPercent(pThis.val());
                //    mOut.txtRequestedPrice.val(mOut.model.netAmount.toFixed(2));
                //    mOut.txtRequestedPriceDiscountAmount.val(mOut.model.discountAmount.toFixed(2));
                //});

                //mOut.txtRequestedPriceDiscountAmount.bind('input', function () {
                //    var pThis = $(this);

                //    if (!IsNumeric(pThis.val()))
                //        return;

                //    mOut.model.setDiscountAmount(pThis.val());
                //    mOut.txtRequestedPrice.val(mOut.model.netAmount.toFixed(2));
                //    mOut.txtRequestedPriceDiscountPercent.val(mOut.model.discountPercent.toFixed(2));
                //});

                //mOut.unitQuantity = 1;

                //mOut.setData = function (vData) {

                //    mData = vData;
                //    mOut.popCombo();

                //};

                //mOut.popCombo = function () {

                //    var pCbItem;
                //    var pDR;
                //    var pRows = GetStringObject('data.Rows', mData);

                //    if (!$.isArray(pRows))
                //        RaiseError('Rows is required and must be of type array js29KZHQ23U6!');

                //    mOut.cbRequestedPriceBasePrice.html('');
                //    for (var pIndex = 0; pIndex <= pRows.length - 1; pIndex++) {
                //        pDR = pRows[pIndex];
                //        if (mOut.unitQuantity == pDR.Quantity) {
                //            pCbItem = new Option(pDR.PriceLevel + ' ' + $.format.number(CDec2(pDR.DiscountedPrice), '#,##0.00'), ReadyGuid(pDR.PriceLevelID));
                //            $(pCbItem).data('data', pDR);
                //            // pCbItem = new Option(pRows[p1].PriceLevel, pRows[p1].PriceLevelID); 
                //            mOut.cbRequestedPriceBasePrice.append(pCbItem);
                //        };
                //    }

                //    if (!mComboInited) {
                //        mComboInited = true;
                //        mOut.cbRequestedPriceBasePrice.change(cbRequestedPriceBasePrice_change);
                //    }
                //    cbRequestedPriceBasePrice_change.apply(mOut.cbRequestedPriceBasePrice);
                //    mOut.cbRequestedPriceBasePrice.selectmenu();
                //    mOut.cbRequestedPriceBasePrice.selectmenu('refresh', true);

                //}; // END mOut.popCombo = function() {

                //mOut.setPriceLevelID = function (vID) {

                //    mOut.cbRequestedPriceBasePrice.val(ReadyGuid(vID));

                //};

                //mOut.getPriceLevelID = function () {

                //    return mOut.cbRequestedPriceBasePrice.val();

                //};

                //function cbRequestedPriceBasePrice_change() {
                //    var pThis = $(this);
                //    var pSelected = pThis.find(':selected');
                //    var pDR = pSelected.data('data');
                //    mOut.model.setBaseAmount(pDR.DiscountedPrice, 'netAmount');
                //    mOut.txtRequestedPrice.val(mOut.model.netAmount.toFixed(2));
                //    mOut.txtRequestedPriceDiscountPercent.val(mOut.model.discountPercent.toFixed(2));
                //    mOut.txtRequestedPriceDiscountAmount.val(mOut.model.discountAmount.toFixed(2));
                //}

                //mOut.onInvalid = function (vResult) {

                //    MessageBox(vResult.error);

                //};

                //mOut.validate = function () {

                //    var pResult = mOut.model.validate();
                //    if (pResult.error != undefined) {
                //        if ($.isFunction(mOut.onInvalid))
                //            mOut.onInvalid(pResult);
                //    }

                //    return pResult;

                //};

                //mOut.onSubmit = function () { };
                //mElement.find('[name=formRequestedPrice]').submit(function (e) {
                //    e.preventDefault();
                //    var pResult = mOut.validate();
                //    if (pResult.error != undefined) {
                //        return false;
                //    }
                //    return mOut.onSubmit(arguments);
                //});

                $('#lnkRecallProductMaker').live('click', function() {
                    Browser.TransBomMaker.Click = function(e) {
                        sessionStorage.CartOrderID = e.ID;
                        //var p = new App.model.BomMaker();
                        //var pBomCart = p.bomCart({
                        //    id: sessionStorage.CartOrderID
                        //});
                        //proj.CartData = pBomCart;
                        //proj.CartStock = [];
                        proj.DisplayCart();
                    };
                    Browser.TransBomMaker.Show();
                });

                return mOut;

            }); // END CreateStringObject('App.controller.PriceChange', null, function() {

        }); // END $(function () {

        return mDeferred.promise();

    });