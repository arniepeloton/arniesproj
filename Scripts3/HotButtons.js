require(['common',
    '/Scripts3/controller/CustomerScreenTransmitter.js?crc='
    ], function () {

        require([(OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''), (OfflineMode ? '/Scripts3/Stock_offline.js?crc=' : ''), 
            (OfflineMode ? '/Scripts3/ItemModifiers.js?crc=' : ''),
            (OfflineMode ? '/Scripts3/model/TableView_Offline.js?crc=' : '')], function () {
            $(function () {

                var divMain = $('#divMain_ProductGroup');
                var txtName = $('#txtName_ProductGroup');
                var divProducts = $('#divProducts_ProductGroup');
                var divGroups = $('#divGroups_ProductGroup');
                var mID;
                var mProducts = [];
                var hotButtonID;

                var isOnMain = true;
                var isGroup1Accessible = false;
                var isGroup2Accessible = false;
                var isGroup3Accessible = false;
                var group1Data;
                var group2Data;
                var group3Data;
                var origCustomerID = sessionStorage.CustomerOrganizationID;
                sessionStorage.LastQty = 1;
                sessionStorage.LastQtyDecimal = '';

                sessionStorage.ProductName = '';
                sessionStorage.ProductUnitQty = 0;
                sessionStorage.ProductUnitMeasure = '';
                sessionStorage.ProductPrice = 0;
                var isEditQty = false;
                var divProductGroup = $('[name=divHotButtonsRow]');
                var defaultPriceLevel = '01000012-0005-0000-0000-000000000000';
                var priceLevelName = 'RETAIL PRICE';
                sessionStorage.ProductPriceLevel = ''; //used in stock.js
                sessionStorage.ProductPriceLevelName = 'RETAIL PRICE'; //used in stock.js
                var menuTemplateList;
                var re = /^\d*\.{0,1}\d*$/;
                var mCurrentItem; // set during button click
                // get during "+" or "-"
                var mTableView = new App.model.TableView();

                $("[data-role=header]").fixedtoolbar({ tapToggle: false });
                //            setTimeout(function () {
                //                //            $.mobile.activePage.bind('pageshow', PageShow);
                //            }, 1000);
                //$.mobile.activePage.bind('pageshow', PageShow);

                //                // for button sounds
                //                $.ionSound({
                //                    sounds: [
                //                        "water_droplet_3"
                //                    ],
                //                    path: "/Content/sounds/"
                //                });
                if (window.HotButtons == null)
                    window.HotButtons = {
                        UpdateCartStock: UpdateCartStock
                    };

                setTimeout(function () {
                    var pPage = $.mobile.activePage;
                    var pHeader = pPage.find("[data-role=header]");
                    var pHeight = pHeader.height();

                    pPage.css('padding-top', pHeight + 'px');

                    pHeader.fixedtoolbar({ tapToggle: false,
                        updatePagePadding: false    // http://jquerymobile.com/test/docs/toolbars/bars-fixed-options.html
                        // http://stackoverflow.com/questions/10545540/jquery-mobile-disable-tap-to-toggle-fixed-header-and-footer
                    });

                }, 100);

                $('body').on('click', '#btnPopupTemp', function () {
                    $('#popupItemModifier').trigger('create').popup().popup('open');
                });

                if (proj.CustomerScreenTransmitter == null) { 
                    proj.CustomerScreenTransmitter = new App.controller.CustomerScreenTransmitter();
                }

                function PageShow() {

                    $.mobile.activePage.data('CanRefreshStockPage', 1);
                    //                    if (ValidPrevPage(arguments)) {
                    if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                        sessionStorage.PosMode = 'Widescreen';
                    else {

                        sessionStorage.PosMode = 'Hot Button Mode';
                    
                        // commented by Arvin 2016_08_22__14_56_57_5578
                        //if ((CStr2(sessionStorage.FowstID) == '')
                        //    || (window.location.pathname.toLowerCase() != '/billofmaterials'))
                        //    BrowseAddressesHotButtons();
                        //else
                        //    SalesMenu_offline().HandleEnforceZRead();
                    }

                    getPimMenu();

                    // commented by Arvin; Aug 27, 2015
                    //if ((window.pageHeader != null)
                    //    && ($.isFunction(window.pageHeader.ShowPaymentsPage))) {

                    //    window.pageHeader.ShowPaymentsPage(false); // modified by Arvin; Apr 8, 2015; precache payment page assets after loading POS data. 
                    //}

                    if (proj.POMode) {
                        $('#divBlockCustomerHotButtons').hide();
                        $('#divBlockSupplierHotButtons').show();

                        $('#lblSupplierNameHotButtons').html(HtmlEncode(sessionStorage.SupplierOrganizationName))
                                        .data('id', sessionStorage.SupplierOrganizationID);

                    } else {
                        $('#divBlockCustomerHotButtons').show();
                        $('#divBlockSupplierHotButtons').hide();
                    }


                    // see /Scripts3/common/common.js
                    startclock('#clockHotButtons');
                    //                    };

                    function ValidPrevPage(args) {
                        try {

                            //return args[1].prevPage.attr('data-role') != 'dialog';
                            return args[1].prevPage.data('CanRefreshStockPage');
                        } catch (e) {

                            return true;
                        };
                    };
                };

                PageShow();
                startclock('#clock');
                /////////////////
                //////////////////
                ///////////////////
                //added by janine
                //Load hot buttons menu
                ///jquery mobile tap event fix double tap issue ghost click
                var lastTapTime;
                function isJQMGhostClick(event) {
                    return false; // by Arvin. why need this function?
                    var currTapTime = new Date().getTime();

                    if (lastTapTime == null || currTapTime > (lastTapTime + 700)) {
                        lastTapTime = currTapTime;
                        return false;
                    }
                    else {
                        return true;
                    }
                }

                function GetDefaultHotButtonDefault(vRows) {

                    if (window.location.pathname.toLowerCase() != '/billofmaterials') 
                        return vRows[0];
                        
                    var pID = Business.SystemDefaults.ProductMakerDefaultHotButtonTemplateID();
                    if (!IsGuid(pID))
                        return vRows[0];
                    pID = ReadyGuid(pID);
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        var pDR = vRows[p];
                        if (ReadyGuid(pDR.ID) == pID)
                            return pDR;
                    }
                    return vRows[0];
                }

                function getPimMenu() {

                    var pData = {};
                    pData.SearchKeyword = $.trim($("#txtHotButtonSearch").val());

                    if (OfflineMode) {
                        var vMenu = SalesMenu_offline().GetSelectedFowstDefaultHBTemplate();

                        if (vMenu.length == 0) {
                            vMenu = SalesMenu_offline().getPimMenu($.trim($("#txtHotButtonSearch").val()));
                        }                        

                        if (vMenu.Rows != undefined) {

                            if (vMenu.Rows.length < 1) {
                                //                                sessionStorage.PosMode = 'Retail Mode';


                                MessageBox('No available menu template for Hot Buttons. Go to System Administration -> System Settings -> Hot Button Settings to create a menu template.', 'Set Hot Buttons', [
                                        { Text: 'Ok',
                                            Click: function () {
                                                window.location = location = MergeUrlParameters({
                                                    mode: proj.POMode ? 'po' : 'pos',
                                                    OrgID: sessionStorage.OrgID,
                                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                                }, '/SalesMenu');
                                            }
                                        }
                                    ]); // MessageBox

                                return;
                            } else {


                                sessionStorage.IsHotButtonVisited = 'true';

                                //                                //if only 1 menu is available, automatically proceed to hot buttons                                
                                var pDefaultTemplate = GetDefaultHotButtonDefault(vMenu.Rows);
                                sessionStorage.HotButtonMenuID = pDefaultTemplate.ID;
                                sessionStorage.MenuTemplateName = pDefaultTemplate.Name;
                                menuTemplateList = vMenu.Rows;

                                proj.ProductID = '';
                                LoadFowstHotButtons();
                                if (CStr2(sessionStorage.CustomerOrganizationID) != '') {
                                    GetCustomerPriceLevel();
                                    SetPOSModeHotButtons();

                                } else {
                                    if (!proj.POMode)
                                        LoadCustomerHotButtons();
                                }
                                if (vMenu.Rows.length == 1) {
                                    $('#divChangeMenuButtons, #divChangeMenuButtons2').hide();

                                } else {

                                    $('#divChangeMenuButtons, #divChangeMenuButtons2').show();
                                }
                            }


                        } else {
                            MessageBox('Error in Load Hot Buttons Menu');
                        }

                    } else {

                        AjaxSend({
                            url: 'ProductGroup/GetPimMenu',
                            data: pData,
                            success: function (vData) {

                                if (vData.PimMenuData.Rows.length < 1) {
                                    //                                    sessionStorage.PosMode = 'Retail Mode';
                                    //                                    $.mobile.changePage('#pageTemplate');

                                    MessageBox('No available Hot Button template. Go to System Administration -> System Settings -> Hot Buttons Settings to create a template.', 'Hot Buttons', [
                                        { Text: 'Ok',
                                            Click: function () {
                                                window.location = location = MergeUrlParameters({
                                                    mode: 'pos',
                                                    OrgID: sessionStorage.OrgID,
                                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                                }, '/SalesMenu');
                                            }
                                        }
                                    ]); // MessageBox

                                    return;
                                } else {

                                    sessionStorage.IsHotButtonVisited = 'true';
                                    //if only 1 menu is available, automatically proceed to hot buttons

                                    sessionStorage.HotButtonMenuID = vData.PimMenuData.Rows[0].ID;
                                    sessionStorage.MenuTemplateName = vData.PimMenuData.Rows[0].Name;
                                    menuTemplateList = vData.PimMenuData.Rows;

                                    proj.ProductID = '';
                                    LoadFowstHotButtons();
                                    if (CStr2(sessionStorage.CustomerOrganizationID) != '') {
                                        GetCustomerPriceLevel();
                                        SetPOSModeHotButtons();

                                    } else {
                                        if (!proj.POMode)
                                            LoadCustomerHotButtons();
                                    }
                                    if (vData.PimMenuData.Rows.length == 1) {
                                        $('#divChangeMenuButtons, #divChangeMenuButtons2').hide();

                                    } else {
                                        $('#divChangeMenuButtons, #divChangeMenuButtons2').show();

                                    }
                                }


                            }
                        });
                    }
                }


                function LoadPimMenu(vRows) {

                    var pLV = $('#lstHotButtonTemplate');
                    var pItem;

                    $("#divHotButtonTemplate").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                    .append($('<a>')
                    .data('id', vRows[p].ID)
                    .data('name', vRows[p].Name)
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();

                            sessionStorage.HotButtonMenuID = $(this).data('id');
                            sessionStorage.MenuTemplateName = $(this).data('name');

                            //                            if (IsMobile)
                            //                                $('#btnHotButtonMain').tap();
                            //                            else
                            //                                $('#btnHotButtonMain').click();

                            //reload/ reset values
                            LoadFowstHotButtons();
                            if (CStr2(sessionStorage.CustomerOrganizationID) != '') {
                                GetCustomerPriceLevel();
                                SetPOSModeHotButtons();

                            } else
                                LoadCustomerHotButtons();
                            proj.ProductID = '';

                            $('#txtOrder').html('');
                            $('#txtQtyUnit').html('');

                            ChangePage('/SalesMenu/HotButtons');

                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Name)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Created By: " + vRows[p].Surname + ', ' + vRows[p].FirstName + ' ' + vRows[p].MiddleName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Last Updated: " + $.format.date(new Date(vRows[p].LastUpdated), 'MMM dd, yyyy'))
                        )
                    );

                        pLV.append(pItem);
                    };

                    try {
                        pLV.listview('refresh');
                    } catch (e) {
                        //                        pLV.listview();
                    }
                };

                $('#btnViewCart').click(function (e) {
                    e.preventDefault();
                    if (CStr2(sessionStorage.CustomerTableNo) != '') {
                        DisplayTableNumberOnButton();

                        $('#selTableNo').val(sessionStorage.CustomerTableNo);
                        $('#lblLabel').html('Customer / Table No.:<br/>');
                    }


                    if (CStr2(sessionStorage.GasPumpNo) != '') {
                        $('#lblCustomerName').text(sessionStorage.GasPumpNo);

                        $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                        $('#lblLabel').html('Gas Pump No.:<br/>');
                    }

                    if (CStr2(sessionStorage.GasPumpNo) != '' && CStr2(sessionStorage.CustomerTableNo) != '') {
                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                        $('#selTableNo').val('');
                        $('#lblLabel').html('Customer:<br/>');

                    }
                    startclock('#clock');
                    SaveCart();
                    //                    sessionStorage.PosMode = 'Retail Mode';
                    //check if the customer is same or not
                    //if yes, then display his cart, if not, reload the cart page tag new cart
                    //origCustomerID == sessionStorage.CustomerOrganizationID && localStorage.NewCartMode != 1
                    if (sessionStorage.CartOrderID != 'undefined' && localStorage.NewCartMode != 1) {
                        proj.ShowCart();
                    } else {
                        var pParams = GetUrlParameters();

                        location = MergeUrlParameters({
                            mode: pParams.mode,
                            OrgID: sessionStorage.OrgID,
                            rsd: pParams.rsd,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/SalesMenu');

                    }

                });

                $('#btnHotButtonsBack').click(function (e) {
                    e.preventDefault();

                    history.back();
                    //                    location.reload();
                });


                GetPOSMode();
                //                GetPaymentMode();

                function DisplayTableNumberOnButton() {
                    var pTableNumber;
                    var pTableName;

                    pTableNumber = sessionStorage.CustomerTableNo;

                    try {
                        pTableName = mTableView.GetTableInfoByInternalName(pTableNumber);
                        if (CStr2(pTableName).trim() != '')
                            pTableNumber = pTableName;    
                        $('#lblLabel, #lblLabelHotButtons').html('Customer / Table No.:<br/>');
                    } catch (e) {
                        debugger;
                    }
                    $('#lblCustomerNameHotButtons').text(pTableNumber);
                }

                function setCustomerInfoHotButtons() {
                    //this will set all customer name to the current selected
                    //added by janine

                    $('#lblCustomerNameRecentHotButtons').text(sessionStorage.CustomerOrganizationName);
                    $('#lblCustomerNameRecent2HotButtons').text(sessionStorage.CustomerOrganizationName);


                    if (CStr2(sessionStorage.CustomerTableNo) != '') {

                        DisplayTableNumberOnButton();

                        try {
                            $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo).selectmenu('refresh', true);

                        } catch (e) {
                            $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo);
                            $('#selTableNoHotButtons').selectmenu();

                        }

                        sessionStorage.GasPumpNo = '';
                        try {
                            $('#selGasPumpNoHotButtons').val('').selectmenu('refresh', true);
                        } catch (e) {

                            $('#selGasPumpNoHotButtons').val('');
                            $('#selGasPumpNoHotButtons').selectmenu();

                        }
                        $('#lblLabelHotButtons').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /> <br> Table No.:');
                    }



                    if (CStr2(sessionStorage.GasPumpNo) != '') {

                        $('#lblCustomerNameHotButtons').text(sessionStorage.GasPumpNo);

                        try {
                            $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo).selectmenu('refresh', true);
                        } catch (e) {
                            $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo);
                            $('#selGasPumpNoHotButtons').selectmenu();

                        }

                        sessionStorage.CustomerTableNo = '';
                        try {
                            $('#selTableNoHotButtons').val('').selectmenu('refresh', true);

                        } catch (e) {
                            $('#selTableNoHotButtons').val('');
                            $('#selTableNoHotButtons').selectmenu();

                        }
                        $('#lblLabelHotButtons').html('Gas Pump No.:<br/>');

                    }

                    if (sessionStorage.GasPumpNo == '' && sessionStorage.CustomerTableNo == '') {

                        $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);

                        try {
                            $('#selTableNoHotButtons').val('').selectmenu('refresh', true);

                        } catch (e) {
                            $('#selTableNoHotButtons').val('');
                            $('#selTableNoHotButtons').selectmenu();

                        }


                        try {
                            $('#selGasPumpNoHotButtons').val('').selectmenu('refresh', true);
                        } catch (e) {

                            $('#selGasPumpNoHotButtons').val('');
                            $('#selGasPumpNoHotButtons').selectmenu();

                        }
                        $('#lblLabelHotButtons').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');

                    }
                } //end of setcustomerinfo

                function SetPOSModeHotButtons() {
                    if (OfflineMode) {
                        if (Business.SystemDefaults.GetSystemDefault("IsShowTableNo") == 1) {
                            $("#divCustomerDetailsRestaurantHotButtons").show();
                            $("#selTableNoHotButtons").empty();
                            $('#selTableNoHotButtons').append(new Option('Please Select', ''));

                            var tableCount = Business.SystemDefaults.GetSystemDefault("NoOfTables");
                            if (CStr2(tableCount) != '') {

                                for (var i = 1; i <= parseInt(tableCount); i++) {
                                    $('#selTableNoHotButtons').append(new Option(i, i));
                                }

                            }



                        } else {
                            $("#divCustomerDetailsRestaurantHotButtons").hide();
                        }

                        if (Business.SystemDefaults.GetSystemDefault("IsShowGasPumpNo") == 1) {
                            $("#divCustomerDetailsGasStationHotButtons").show();
                            $("#selGasPumpNoHotButtons").empty();
                            $('#selGasPumpNoHotButtons').append(new Option('Please Select', ''));

                            var pumpCount = Business.SystemDefaults.GetSystemDefault("NoOfGasPumps");
                            if (CStr2(pumpCount) != '') {

                                for (var i = 1; i <= parseInt(pumpCount); i++) {
                                    $('#selGasPumpNoHotButtons').append(new Option(i, i));
                                }

                            }

                        } else {
                            $("#divCustomerDetailsGasStationHotButtons").hide();
                        }
                        setCustomerInfoHotButtons();
                    } else {
                        AjaxSend({
                            url: 'SalesMenu/GetPOSMode',
                            success: function (vData) {
                                if (vData.IsShowTableNo == 1) {
                                    $("#divCustomerDetailsRestaurantHotButtons").show();
                                    $("#selTableNoHotButtons").empty();
                                    $('#selTableNoHotButtons').append(new Option('Please Select', ''));

                                    if (CStr2(vData.NoOfTables) != '') {

                                        for (var i = 1; i <= vData.NoOfTables; i++) {
                                            $('#selTableNoHotButtons').append(new Option(i, i));
                                        }

                                    }

                                    try {
                                        $('#selTableNoHotButtons').selectmenu('refresh', true);
                                    } catch (e) {

                                        $('#selTableNoHotButtons').selectmenu();

                                    }

                                } else {
                                    $("#divCustomerDetailsRestaurantHotButtons").hide();
                                }

                                if (vData.IsShowGasPumpNo == 1) {
                                    $("#divCustomerDetailsGasStationHotButtons").show();
                                    $("#selGasPumpNoHotButtons").empty();
                                    $('#selGasPumpNoHotButtons').append(new Option('Please Select', ''));

                                    if (CStr2(vData.NoOfTables) != '') {

                                        for (var i = 1; i <= vData.NoOfGasPumps; i++) {
                                            $('#selGasPumpNoHotButtons').append(new Option(i, i));
                                        }

                                    }

                                    try {
                                        $('#selGasPumpNoHotButtons').selectmenu('refresh', true);
                                    } catch (e) {

                                        $('#selGasPumpNoHotButtons').selectmenu();

                                    }

                                } else {
                                    $("#divCustomerDetailsGasStationHotButtons").hide();
                                }

                                setCustomerInfoHotButtons();
                            }
                        });
                    }


                }

                $('#butNewSalesOrderHotButtons').click(function (e) {

                    var pCount = 0;

                    //additional by janine
                    $('#divRecentlyBoughtPanel').hide();
                    $('#divCartPanel').show();

                    if (proj.PosMode) {
                        $('#lblPayNow').html('Pay <br/>Now');
                    } else {
                        $('#lblPayNow').html('Save <br/>Sales Order');
                    }

                    isOnCartPage = true;

                    //                    if (mLvCartHotButtons)
                    //                        pCount = mLvCart.vData.Stocks.Rows.length;
                    //                    alert(sessionStorage.CartOrderID != undefined);
                    if (sessionStorage.CartOrderID != undefined) {
                        MessageBox('Are you sure you want to create a New Cart?' +
                            ' If yes, the contents of your current cart will be placed “On Hold”.  We recommend that you complete your current cart first before creating a new cart.', 'New Cart', [
                            {
                                Text: 'Continue with Current Cart',
                                Click: function () {

                                    ChangePage('/SalesMenu/HotButtons');

                                }
                            }, {
                                Text: 'Create New Cart',
                                Click: function () {
                                    NewCartHotButtons(); //additional by janine
                                    isCartEmpty = true;
                                    $('.ui-dialog').dialog('close');
                                    //////////                if (OfflineMode) {
                                    //////////                    GotoFirstPage(); //Added by Janine Oct 19 3pm
                                    //////////                    NewCart();
                                    //////////                    Search();
                                    //////////                } else
                                    //////////                    window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID); //Added by JAnine 0ct 22. clear contents

                                }
                            }
                        ]);
                    } else {
                        NewCartHotButtons(); //additional by janine
                        isCartEmpty = true;
                        $('#popupMoreOptionsHotButtons').popup('close');

                    }

                }); // $('#butNewSalesOrder').click(function(e) {


                $('#lnkSalesOrderHotButtons').click(function (e) {

                    var pBrowser;
                    //additional by janine
                    $('#divRecentlyBoughtPanel').hide();
                    $('#divCartPanel').show();

                    if (proj.PosMode) {
                        $('#lblPayNow').html('Pay <br/>Now');
                    } else {
                        $('#lblPayNow').html('Save <br/>Sales Order');
                    }

                    isOnCartPage = true;

                    e.preventDefault();
                    if (sessionStorage.CustomerOrganizationID == undefined) {
                        MessageBox('Please select a customer first');
                        return;
                    };
                    ////////////////////                                        if (proj.PosMode) {
                    pBrowser = Browser.TransInvoices;
                    ////////////////////                                        } else {
                    ////////////////////                                            pBrowser = Browser.TransSalesOrder;
                    ////////////////////                                        };

                    pBrowser.InputData.CustomerID = sessionStorage.CustomerOrganizationID;
                    pBrowser.Click = function (e) {
                        sessionStorage.CustomerOrganizationID = e.CustomerID;

                        if (e.UserID != e.CurrentUserID) {
                            MessageBox("Warning : This cart was created by " + e.EmployeeFullName + ". Do you want to continue?",
                                        'Recall Cart',
                                        [
                                            { Text: 'Yes',
                                                Click: function () {

                                                    sessionStorage.CartOrderID = e.ID;

                                                    //Added By Francis to load Customer Info
                                                    sessionStorage.CustomerOrganizationName = e.CompanyName;
                                                    //put condition if table no or gas pump no if GP or TBL\

                                                    if (CStr2(e.TableNo) != '') {
                                                        var prefix = e.TableNo.split('_');

                                                        if (prefix[0] == 'GP')
                                                            sessionStorage.GasPumpNo = prefix[1];
                                                        else
                                                            sessionStorage.CustomerTableNo = prefix[1];
                                                    } else {
                                                        sessionStorage.GasPumpNo = '';
                                                        sessionStorage.CustomerTableNo = '';
                                                    }
                                                    setCustomerInfoHotButtons();
                                                    $('#lnkCustomer').data('id', e.CustomerID);

                                                    proj.ShowCart();
                                                    //                                                    $('#btnViewCart').click();
                                                    //                                                                                                        LoadCart();

                                                    //                                                                                                        GotoFirstPage();

                                                }
                                            }
                                        ,
                                            { Text: 'No',
                                                Click: function () {
                                                    GotoFirstPage();
                                                    return;
                                                }
                                            }
                                        ]
                                    );
                        } else {

                            sessionStorage.CartOrderID = e.ID;

                            //Added By Francis to load Customer Info
                            sessionStorage.CustomerOrganizationName = e.CompanyName;
                            //put condition if table no or gas pump no if GP or TBL
                            if (CStr2(e.TableNo) != '') {
                                var prefix = e.TableNo.split('_');
                                if (prefix[0] == 'GP')
                                    sessionStorage.GasPumpNo = prefix[1];
                                else
                                    sessionStorage.CustomerTableNo = prefix[1];
                            } else {
                                sessionStorage.GasPumpNo = '';
                                sessionStorage.CustomerTableNo = '';
                            }
                            setCustomerInfoHotButtons();

                            $('#lnkCustomer').data('id', e.CustomerID);

                            proj.ShowCart();

                            //                            $('#btnViewCart').click();
                            //                                                        LoadCart();

                            //                                                        GotoFirstPage();

                        }
                    };
                    pBrowser.Show();

                });


                function GetPOSMode() {
                    //                    AjaxSend({
                    //                        url: '/SalesMenu/GetPOSMode',
                    //                        success: function (vData) {
                    if (proj.PosMode) {
                        var vData = [
                        //                                {
                        //                                    ID: 1,
                        //                                    ModeName: 'Expedite Mode',
                        //                                    IsActive: 1,
                        //                                    Description: 'Lets you see which customers have pending orders so that you can help <br/>  expedite their order. This also lets you view which ones have <br/>open tabs and it also lets you close their tabs via cart checkout.'
                        //                                },
                        //                                {
                        //                                    ID: 2,
                        //                                    ModeName: 'Kiosk Mode',
                        //                                    IsActive: 1,
                        //                                    Description: 'Used for customer self-ordering. '
                        //                                },
                                {
                                ID: 3,
                                ModeName: 'Retail Mode',
                                IsActive: 1,
                                Description: 'Makes it easy for retailers to search for SKUs using <br/>keyboard entry or bar code / QR code scanning (best for retailers <br/>with hundreds or thousands of SKUs).'
                            },
                                {
                                    ID: 4,
                                    ModeName: 'Hot Button Mode',
                                    IsActive: 1,
                                    Description: 'Shows predefined buttons sorted by categories (best for fast food <br/>outlets and retailers whose items do not have UPC or bar code labels). <br/>Administrators can customize these buttons online.'
                                }
                                ,
                                {
                                    ID: 5,
                                    ModeName: 'Table View Mode',
                                    IsActive: 1,
                                    Description: 'Lets you view dining patrons as they are seated in tables <br/>(best for fine dining restaurants that require a high-touch service). '
                                },
                                {
                                    ID: 7,
                                    ModeName: 'FMCG Mode (DOS)',
                                    IsActive: 1,
                                    Description: 'Fast Moving Consumer Goods'
                                },
                                {
                                    ID: 8,
                                    ModeName: 'FMCG Mode (Modern)',
                                    IsActive: 1,
                                    Description: 'Fast Moving Consumer Goods'
                                }
                            ];
                    } else {
                        var vData = [
                                {
                                    ID: 6,
                                    ModeName: 'Straight Ordering Mode',
                                    IsActive: 1,
                                    Description: 'Makes it easy for retailers to search for SKUs using <br/>keyboard entry or bar code / QR code scanning (best for retailers <br/>with hundreds or thousands of SKUs).'
                                }
                        //,
                        //{
                        //    ID: 7,
                        //    ModeName: 'Inventory Based Ordering Mode',
                        //    IsActive: 1,
                        //    Description: '<br/> <br/>'
                        //},
                            ];
                    }


                    var pLV = $('#divPOSModeHotButtons');
                    var pItem;

                    pLV.html('');
                    for (intCntr = 0; intCntr < vData.length; intCntr++) {

                        pItem = $('<div style="text-align: center" name="divPOSModeButtonMainHotButtons">')
                        .data('data', vData[intCntr])
                            .click(function (e) {
                                e.preventDefault();

                                //TABLE VIEW MODE
                                if ($(this).data('data').ID == '5') {
                                    if (OfflineMode) {
                                        if (window.location.pathname.toLowerCase() == '/tableview/NewTable') {
                                            Android.Toast('You are already in Table View Mode');
                                            $('#popupPOSMode').popup('close');
                                        } else {
                                            goToTableViewPage();
                                        }
                                    } else {
                                        MessageBox("Table View Mode is only available on the mobile app",
                                                    "Table View Mode",
                                                    [
                                                        { Text: 'Ok' }
                                                    ]
                                            );
                                    }
                                }
                                // if hot button mode
                                if ($(this).data('data').ID == '4') {
                                    //                                    sessionStorage.PosMode = 'Hot Button Mode';
                                    //                                    
                                    if (OfflineMode) {
                                        Android.Toast('You are already in Hot Button Mode');
                                        $('#popupPOSModeHotButtons').popup('close');

                                    } else {
                                        MessageBox("You are already in Hot Button Mode",
                                                    "POS Mode",
                                                    [
                                                        { Text: 'Ok',
                                                            Click: function () {
                                                                ChangePage('/SalesMenu/HotButtons');

                                                            }
                                                        }
                                                    ]
                                        );
                                    }

                                }

                                // if retail mode
                                if ($(this).data('data').ID == '3') {
                                    //                                    sessionStorage.IsManualSelectRetailMode = 'true';

                                    if (CStr2(sessionStorage.CustomerTableNo) != '') {

                                        DisplayTableNumberOnButton();
                                        try {

                                            $('#selTableNo').val(sessionStorage.CustomerTableNo).selectmenu('refresh', true);
                                        } catch (e) {

                                            $('#selTableNo').val(sessionStorage.CustomerTableNo);
                                            $('#selTableNo').selectmenu();

                                        }

                                        $('#lblLabel').html('Customer / Table No.:<br/>');

                                    }

                                    if (CStr2(sessionStorage.GasPumpNo) != '') {

                                        $('#lblCustomerName').text(sessionStorage.GasPumpNo);

                                        try {
                                            $('#selGasPumpNo').val(sessionStorage.GasPumpNo).selectmenu('refresh', true);
                                        } catch (e) {
                                            $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                                            $('#selGasPumpNo').selectmenu();

                                        }

                                        $('#lblLabel').html('Gas Pump No.:<br/>');
                                    }

                                    if (CStr2(sessionStorage.GasPumpNo) != '' && CStr2(sessionStorage.CustomerTableNo) != '') {
                                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                                        $('#lblLabel').html('Customer:<br/>');



                                        try {
                                            $('#selTableNo').val('').selectmenu('refresh', true);
                                        } catch (e) {
                                            $('#selTableNo').val('');
                                            $('#selTableNo').selectmenu();

                                        }

                                        $('#lblLabel').html('Customer:<br/>');

                                    }
                                    $('#btnViewCart').click();
                                }

                                // Same as Retail Mode
                                if ($(this).data('data').ID == '6') {
                                    sessionStorage.PosMode = 'Retail Mode';

                                    $('#lblCurrentPOSModeHotButtons').text($('#btnPOS_6').find('h3:first').text());

                                    sessionStorage.IsInventoryCountMode = false;

                                    $('#popupPOSModeHotButtons').popup('close');
                                }

                                if ($(this).data('data').ID == '7') {
                                    sessionStorage.PosMode = 'Inventory Based Ordering Mode';

                                    sessionStorage.IsInventoryCountMode = true;

                                    $('#lblCurrentPOSModeHotButtons').text($('#btnPOS_7').find('h3:first').text());

                                    $('#popupPOSModeHotButtons').popup('close');

                                }
                                // FMCG DOS
                                if ($(this).data('data').ID == '7') {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/FMCGBasic');
                                }
                                // FMCG Modern
                                if ($(this).data('data').ID == '8') {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/FMCG');
                                }

                            })
                        .append($('<a name="btnPOSModeButtonHotButtons" id="btnPOS_' + vData[intCntr].ID + '" data-role="button" data-theme="e">')
                        .append($('<h3>')
                            .text(vData[intCntr].ModeName)
                            )
                            .append($('<p style="font-weight: normal; margin-top: -15px;">')
                            .html(vData[intCntr].Description)
                            )
                        )
                        pLV.append(pItem);

                        pLV.trigger("create");

                    } //end for

                    if (CBool(proj.POMode)) {
                        $('#lblCurrentPOSModeHotButtons').text('Customer: ' + sessionStorage.FowstName);
                    } else {
                        if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall') {
                            $('#lblCurrentPOSMode').text('Hot Button Mode (Widescreen)');
                        } else {
                            $('#lblCurrentPOSModeHotButtons').text('Hot Button Mode');
                        }

                        if (GetUrlParameters().TableNo != undefined) {
                            $('#lblCurrentPOSMode').text('Table View Mode');
                            $('#lblCurrentPOSModeHotButtons').text('Table View');
                        }
                    }
                    //                        } //end success
                    //                    }); // AjaxSend
                }
                function goToTableViewPage() {
                    sessionStorage.PosMode = 'Table View Mode';
                    proj.ProductID = '';
                    $.mobile.changePage('');
                    location = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0
                    }, '/TableView/NewTable');

                    //                    ChangePage('/SalesMenu/TableView');
                }
                function GetPaymentMode() {
                    var vData = [

                                {
                                    ID: 1,
                                    ModeName: 'Pay As You Order'
                                },
                                {
                                    ID: 2,
                                    ModeName: 'Open Tab'
                                }

                            ];


                    $('#selPaymentMode').empty();

                    for (var p = 0; p <= vData.length - 1; p++) {
                        $('#selPaymentMode').append(new Option(vData[p].ModeName, vData[p].ID, false, false));
                    }
                    $('#selPaymentMode').val(vData[0].ID);
                    $('#selPaymentMode').selectmenu();
                    $('#selPaymentMode').selectmenu('refresh', true);

                }

                function GetCustomerPriceLevel() {

                    if (OfflineMode) {
                        if (!proj.POMode) {
                            var vDataPriceLevel = SalesMenu_offline().GetCustomerPriceLevel(sessionStorage.CustomerOrganizationID);

                            if (vDataPriceLevel.Data != undefined) {
                                $('#selPriceLevel').empty();

                                for (var p = 0; p <= vDataPriceLevel.Data.Rows.length - 1; p++) {
                                    $('#selPriceLevel').append(new Option(vDataPriceLevel.Data.Rows[p].PriceLevel, vDataPriceLevel.Data.Rows[p].PriceLevelID, false, false));
                                }

                                var pDrFranchiseeData = Business.LibraryOrganizationFOWST.GetFranchiseeData(sessionStorage.FowstID);
                                var pDefaultPriceID = pDrFranchiseeData.DefaultSellingPriceLevel;
                                if (IsGuid(pDefaultPriceID))
                                    pDefaultPriceID = ReadyGuid(pDefaultPriceID);
                                else {
                                    pDefaultPriceID = Business.SystemDefaults.GetRetailPriceID();
                                    //if (window.DebugMode)
                                    //    pDefaultPriceID = ReadyGuid(Business.SystemDefaults.GetWholesalePriceID()); // for testing only
                                }
                                if (IsGuid(pDefaultPriceID))
                                    pDefaultPriceID = ReadyGuid(pDefaultPriceID);
                                else
                                    pDefaultPriceID = '01000012-0005-0000-0000-000000000000';
                                $('#selPriceLevel').val(pDefaultPriceID);
                                $('#selPriceLevel').selectmenu();
                                $('#selPriceLevel').selectmenu('refresh', true);
                                proj.ProductID = '';

                                //$('#txtOrder').html(''); // commented by Arvin 2016_08_17__16_28_34_3631
                                $('#txtQtyUnit').html('');


                                GetData(sessionStorage.HotButtonMenuID, pDefaultPriceID);

                            }
                        } else {


                            $('#selPriceLevel').empty();
                            $('#selPriceLevel').append(new Option(sessionStorage.SupplierPriceLevelName, sessionStorage.SupplierPriceLevelID, false, false));
                            $('#selPriceLevel').val(sessionStorage.SupplierPriceLevelID);
                            $('#selPriceLevel').selectmenu();
                            $('#selPriceLevel').selectmenu('refresh', true);

                            proj.ProductID = '';

                            $('#txtOrder').html('');
                            $('#txtQtyUnit').html('');

                            GetData(sessionStorage.HotButtonMenuID);
                        }
                    } else {
                        AjaxSend({
                            url: 'SalesMenu/GetCustomerPriceLevel',
                            data: { CustomerOrganizationID: sessionStorage.CustomerOrganizationID },
                            success: function (vData) {

                                if (vData.Data != undefined) {
                                    $('#selPriceLevel').empty();

                                    for (var p = 0; p <= vData.Data.Rows.length - 1; p++) {
                                        $('#selPriceLevel').append(new Option(vData.Data.Rows[p].PriceLevel, vData.Data.Rows[p].PriceLevelID, false, false));
                                    }
                                    $('#selPriceLevel').val('01000012-0005-0000-0000-000000000000');

                                    $('#selPriceLevel').selectmenu();
                                    $('#selPriceLevel').selectmenu('refresh', true);
                                    proj.ProductID = '';

                                    $('#txtOrder').html('');
                                    $('#txtQtyUnit').html('');


                                    GetData(sessionStorage.HotButtonMenuID);

                                }
                            }
                        });
                    };


                }

                $('#selPriceLevel').change(function () {
                    defaultPriceLevel = $('#selPriceLevel').val();
                    priceLevelName = $("#selPriceLevel option:selected").text();
                    //refresh the buttons to reflect new price 
                    proj.ProductID = '';

                    $('#txtOrder').html('');
                    $('#txtQtyUnit').html('');

                    sessionStorage.ProductPriceLevelName = priceLevelName;
                    GetData(sessionStorage.HotButtonMenuID);

                });



                $('#btnSearchHotButtons').click(function () {

                    SearchHotButtons();
                });

                $('#butBarCodeHotButtons').click(function () {
                    if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {

                        mobile.Scan({
                            Success: function (vData) {

                                $('#txtSearch').val(vData.Value);
                                SearchHotButtons();
                            }
                        });
                    } else {
                        MessageBox('Scan Barcode / QR Code feature is only available on the mobile app',
                                    'Scan Barcode / QR Code',
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    }
                    // old code
                    //                window.plugins.barcodeScanner.scan(function (result) {
                    //                    $('#txtSearch').val(result.text);
                    //                    Search();
                    //                }, function (error) {
                    //                    alert("Scanning failed: " + error);
                    //                });

                });

                function SearchHotButtons() {

                    window.IsBrowserloaded = false;
                    Browser.LibraryPIMProduct.InputData.IsSearchMore = 0;
                    Browser.LibraryPIMProduct.Click = function (e, vOpt) {
                        proj.ProductID = e.ID;
                        vOpt.DontClose = true;

                        window.NewStockBrowser = true;
                        startclock('#clockStock');
                        clearTimeout(proj.RetransmitToSecondScreenTimeoutHandle);
                        ChangePage('Stock');
                    };
                    Browser.LibraryPIMProduct.Show($('#txtSearch').val());

                    $('#txtSearch').val('');

                };

                // TABLE NO ONCHANGE
                $('#selTableNoHotButtons').change(function () {

                    $('#lblCustomerNameHotButtons').text($(this).val());

                    if (CStr2($(this).val()) != '') {
                        sessionStorage.GasPumpNo = '';
                        $('#selGasPumpNoHotButtons').val('').selectmenu('refresh', true);

                        sessionStorage.CustomerTableNo = $(this).val();
                        DisplayTableNumberOnButton();
                        $('#lblLabelHotButtons').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /> <br> Table No.:');
                    } else {
                        sessionStorage.CustomerTableNo = '';
                        $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);
                        $('#lblLabelHotButtons').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');
                    }

                    return;
                });
                $('#selGasPumpNoHotButtons').change(function () {

                    $('#lblCustomerNameHotButtons').text($(this).val());

                    if (CStr2($(this).val()) != '') {
                        sessionStorage.CustomerTableNo = '';
                        $('#selTableNoHotButtons').val('').selectmenu('refresh', true);

                        sessionStorage.GasPumpNo = $(this).val();
                        $('#lblCustomerNameHotButtons').text(sessionStorage.GasPumpNo);
                        $('#lblLabelHotButtons').html('Gas Pump No.:<br/>');
                    } else {
                        sessionStorage.GasPumpNo = '';
                        $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);
                        $('#lblLabelHotButtons').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');
                    }

                    return;
                });


                $('#btnOrder').bind((InAndroid ? 'tap' : 'click'), function (e) {
                    if (isJQMGhostClick(e)) { return; }

                    if (CStr2(proj.ProductID) != '') {
                        var pData = $(this).data('data');
                        proj.ProductID = pData.ProductID;
                        if (CBool(pData.HasModifier)) {
                            NewMessageBox('', 'Options', [
                                {
                                    Text: 'Go to Stock Info',
                                    Size: 3,
                                    Click: function () {
                                        proj.GroupIdOfItemModifiers = pData.GroupIdOfItemModifiers;
                                        sessionStorage.ItemModifier = JSON.stringify(pData.ItemModifier);
                                        GotoStockInfoPage();
                                    }
                                },
                                {
                                    Text: 'Edit Modifier',
                                    Size: 3,
                                    Click: function () {
                                        NewItemModifier({
                                            EditModifier: true,
                                            ProductData: pData,
                                            success: function (e) {
                                                var pItem = UpdateCartStock($.extend({
                                                    ModifierGroupID: proj.ModifierGroupID,
                                                    IsOrderSlipPrinted: 0
                                                }, e), false, e.Quantity, {
                                                    DoAdd: true
                                                });
                                                DisplayStock(pItem);
                                            }
                                        });
                                    }
                                }
                            ], { CloseButton: 'enable', AutoDismiss: true });
                        } else {
                            $("#popupLoad").popup("open");
                            setTimeout(function () {
                                GotoStockInfoPage()
                            }, 1000);
                        }

                        function GotoStockInfoPage() {
                            SaveCart();
                            window.NewStockBrowser = true;
                            startclock('#clockStock');
                            clearTimeout(proj.RetransmitToSecondScreenTimeoutHandle);
                            ChangePage('Stock');
                        }                        
                    } else
                        return;
                });


                $('#btnHomeHotButtons').click(function (e) {
                    e.preventDefault();
                    SaveCart();
                    window.location = '/People/PeopleMenu';

                });


                $('#btnTagAsVIPOrder').click(function (e) {
                    e.preventDefault();
                    MessageBox('This feature is not yet implemented');
                    // TODO

                });
                //////CUSTOMER///////////////////////////

                $('#btnCustomerHotButtons').click(function (e) {
                    e.preventDefault();
                    //new edited by janine oct 24 see CustomerDetails.js in Scripts3
                    pageCustomerDetails.Show();
                    //                    $('#popupCustomerOptionsHotButtons').popup('open');
                    //                    $('#txtCustomerNameHotButtons').focus();

                });



                function LoadCustomerHotButtons() {

                    if (OfflineMode) {
                        DisplayCustomerHotButtons(SalesMenu_offline().GetCustomer(sessionStorage.CustomerOrganizationID));
                        GetCustomerPriceLevel();
                        SetPOSModeHotButtons();
                    } else {
                        AjaxSend({
                            url: 'SalesMenu/GetCustomer',
                            data: { CustomerOrganizationID: sessionStorage.CustomerOrganizationID },
                            success: function (vData) {

                                if ((vData != undefined) && (vData.CompanyName != undefined)) {
                                    DisplayCustomerHotButtons(vData);
                                } else {
                                    BrowseCustomerHotButtons();
                                }; // if (vData.CompanyName != undefined) {
                                GetCustomerPriceLevel();
                                SetPOSModeHotButtons();

                            }
                        });
                    };

                    function DisplayCustomerHotButtons(vData) {

                        if (vData == undefined) return;
                        sessionStorage.CustomerOrganizationID = vData.ID;

                        sessionStorage.CustomerOrganizationName = ((CStr2(vData.CompanyName) != '') ? vData.CompanyName : vData.PersonSurname + ', ' + vData.PersonFirstName + ' ' + vData.PersonMiddleName);

                        $('#lnkCustomerHotButtons').html(HtmlEncode('Search Customer: ' + sessionStorage.CustomerOrganizationName) + '&#x25BC;')
                            .data('id', sessionStorage.CustomerOrganizationID);

                        //TODO PRICE LEVEL
                        //                          $('#lnkPriceLevel').html(HtmlEncode(vData.PriceLevel) + '&#x25BC;')
                        //                            .data('id', vData.PriceLevelID);
                        //   
                        //ADDITIONAL BY JANINE FEB 18
                        //                        setCustomerInfoHotButtons();
                    };

                }; // function DisplayCustomer() {


                //////FOWST
                ////////////////////

                $('#btnFowstHotButtons').click(function (e) {
                    e.preventDefault();
                    BrowseAddressesHotButtons();

                    return false;
                });

                function LoadFowstHotButtons() {

                    //                    sessionStorage.FowstID
                    $('#lnkPosBranchHotButtons').html(HtmlEncode('Fowst: ' + sessionStorage.FowstName) + '&#x25BC;');
                    $('.lblPosBranch').text(sessionStorage.FowstName);
                }


                function BrowseAddressesHotButtons() {

                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.AfterClick = function (e) {
                        sessionStorage.FowstID = e.ID;
                        sessionStorage.EnableHotButtonsContextualMenu = CBool(e.EnableHotButtonsContextualMenu);
                        HandleMallLessor();
                        $('#lnkPosBranchHotButtons').html(HtmlEncode('Fowst: ' + e.FowstName) + '&#x25BC;');
                        $('.lblPosBranch').text(e.FowstName);
                        sessionStorage.FowstName = e.FowstName;
                        LoadFowstHotButtons();
                        PredeterminePrinter();
                        VerifyDevice();

                        if (OfflineMode) {
                            SalesMenu_offline().UpdateCounterFromOnline();
                            SalesMenu_offline().HandleEnforceZRead();
                        }
                    };
                    Browser.LibraryFowst.Show();

                };

                function HandleMallLessor() {

                    if (localStorage.LoginHash != undefined) {
                        AjaxSend({
                            url: 'MallLessor/SetMall',
                            data: {
                                LoginHash: localStorage.LoginHash
                                // ,Mall: $('#cbMall > option:selected').val()
                            },
                            success: function (vData) {
                                if (vData.Error) {
                                    MessageBox(vData.Error);
                                    return;
                                } else if (vData.Result) {
                                    localStorage.removeItem('LoginHash');
                                    MessageBox('Mall Report Setup was successful.', 'Success');
                                };
                            }
                        }); // AjaxSend({
                    }; // if (localStorage.LoginHash != undefined) {
                }; // HandleMallLessor

                function PredeterminePrinter() {
                    if (!IsMobile) return;
                    if (OfflineMode) return; // todo
                    AjaxSend({
                        url: '/Organization/getOrgFOWSTPrinterIP',
                        success: function (vData) {
                            try {
                                if (vData.OrganizationFOWSTData.Rows.length == 0) {
                                    MessageBox('This device has no assigned printer in the selected FOWST');
                                };
                            } catch (e) {
                                MessageBox('Unable to determine "vData.OrganizationFOWSTData.Rows.length"');
                                console.error(e);
                            };
                        }
                    });

                }; // PredeterminePrinter

                function VerifyDevice() {

                    if (OfflineMode) return; // todo
                    AjaxSend({
                        url: '/Login3/VerifyDevice',
                        success: function (vData) {
                            if (vData.Error != undefined) {
                                MessageBox('This device is not assigned to this FOWST',
                                                'FOWST',
                                                [
                                                    { Text: 'Ok',
                                                        Click: function () {
                                                            //redirect to home. kick out of POS
                                                            window.location = '/People/PeopleMenu';
                                                        }
                                                    }
                                                ]
                                    );

                            } else {
                                NewCartHotButtons(); //additional by janine
                                isCartEmpty = true;
                            }

                        }
                    }); // AjaxSend

                }; // VerifyDevice


                ////END OF FOWST////
                ////////////////



                function NewCartHotButtons() {

                    localStorage.NewCartMode = 1;
                    sessionStorage.removeItem('CartOrderID');

                    //additional by janine
                    sessionStorage.removeItem('InvoiceProductIDList');
                    //additional by janine
                    $('#divCartPanel').show();

                    $('#divCartSection').hide();
                    $('#divCartEmpty').show();


                    $('#txtOrder').html('');
                    $('#txtQtyUnit').html('');

                    sessionStorage.removeItem('CustomerOrganizationID'); //reset to customer org para mahimong cash ang default
                    sessionStorage.removeItem('LastQty');
                    LoadCustomerHotButtons();

                    //Inserted by Francis A. to load the Invoice Details from Sales Payment due to Overpayment of Check 
                    //09/18/2013
                    if (sessionStorage.CartOnHoldCheckPayment != undefined) {
                        if (sessionStorage.CartOnHoldCheckPayment == 1 && sessionStorage.CartOnHoldCheckPaymentOrderID) {
                            sessionStorage.CartOrderID = sessionStorage.CartOnHoldCheckPaymentOrderID;
                            proj.ShowCart();
                        }
                    }
                    //End Inserted by Francis A. ------------------------
                    proj.CartStock = [];
                };

                //////FOWST///////////////////////////

                $('#btnPOSModeHotButtons').click(function (e) {
                    e.preventDefault();

                    $('#popupPOSModeHotButtons').popup('open');
                });

                $('#btnPOSMode1').click(function (e) {
                    e.preventDefault();
                    BrowseAddresses();
                    return false;

                    //e.preventDefault();
                    //$('#popupPOSMode').popup('open');
                });

                // remove blanking of button 2017_12_18__14_06_19_7766
                //if (CStr2(sessionStorage.CustomerTableNo) != '')
                //    $('#btnMoreOptionsHotButtons').text('');
                //else
                    $('#btnMoreOptionsHotButtons').click(function (e) {
                        e.preventDefault();

                        $('#popupMoreOptionsHotButtons').popup('open');
                        $('#popupMoreOptions').popup('open');

                    });
                //More Options

                window.HotButtons.ChangeHotButtonsTemplate = function() {
                    var pCondition = window.location.pathname.toLowerCase();
                    pCondition = (pCondition == '/BillOfMaterials'.toLowerCase())
                        || (pCondition == '/SalesMenu/POSViewAll'.toLowerCase());
                    
                    if (pCondition) {
                        var hb = [];

                        for (var i = 0; i < menuTemplateList.length; i++) {
                            hb.push({
                                data: menuTemplateList[i],
                                Text: menuTemplateList[i].Name,
                                Click: function (e) {
                                    sessionStorage.HotButtonMenuID = e.ID;
                                    sessionStorage.MenuTemplateName = e.Name;

                                    //reload/ reset values
                                    LoadFowstHotButtons();
                                    if (CStr2(sessionStorage.CustomerOrganizationID) != '') {
                                        sessionStorage.removeItem('HotButtonData');
                                        GetCustomerPriceLevel();
                                        SetPOSModeHotButtons();

                                    } else
                                        LoadCustomerHotButtons();
                                    proj.ProductID = '';

                                    $('#txtOrder').html('');
                                    $('#txtQtyUnit').html('');
                                }
                            })
                        }

                        PopupButtonList('', 'List of Hot Buttons', hb)
                    } else {
                        LoadPimMenu(menuTemplateList);

                        $.mobile.changePage('#pageTemplate');
                    }                    
                };
                $('#btnChangeMenuButtons, #btnChangeMenuButtons2').click(window.HotButtons.ChangeHotButtonsTemplate);



                ///////////////////



                ////////////////////

                ////////////////
                function getCityNameHotButtons() {
                    if (OfflineMode) {
                        var vCity;
                        vCity = SalesMenu_offline().getCityListPH();
                        DisplayCitiesHotButtons(vCity);
                    } else {
                        AjaxSend({
                            url: 'City/getCityListAll',
                            success: function (vData) {

                                DisplayCitiesHotButtons(vData);

                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });
                    }

                }


                function getCountryNameHotButtons() {
                    if (OfflineMode) {
                        var vCountry;
                        vCountry = SalesMenu_offline().getCountryListAll();
                        DisplayCountriesHotButtons(vCountry);
                    } else {
                        AjaxSend({
                            url: 'Country/getCountryListAll',
                            success: function (vData) {

                                DisplayCountriesHotButtons(vData);

                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });
                    }

                }

                function DisplayCountriesHotButtons(vData) {
                    var vRows = vData.CountryData.Rows;

                    $('#selOrgCountryHotButtons').empty();
                    //                    $('#selPersonCountry').empty();

                    $('#selOrgCountryHotButtons').append(new Option('Please Select Country', ''));
                    //                    $('#selPersonCountry').append(new Option('Please Select Country', ''));

                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgCountryHotButtons').append(new Option(vRows[p].Country, vRows[p].ID, false, false));
                        //                        $('#selPersonCountry').append(new Option(vRows[p].Country, vRows[p].ID, false, false));
                    }
                    $("#selOrgCountryHotButtons option:contains('Philippines')").attr('selected', 'selected');
                    //                    $("#selPersonCountry option:contains('Philippines')").attr('selected', 'selected');

                    try {
                        $('#selOrgCountryHotButtons').selectmenu('refresh', true);
                        //                        $('#selPersonCountry').selectmenu('refresh', true);
                    } catch (e) {

                        $('#selOrgCountryHotButtons').selectmenu();
                        $('#selOrgCountryHotButtons').selectmenu('refresh', true);
                        //                        $('#selPersonCountry').selectmenu();
                        //                        $('#selPersonCountry').selectmenu('refresh', true);

                    }
                }

                function DisplayCitiesHotButtons(vData) {
                    var vRows = vData.CityData.Rows;

                    $('#selOrgCityHotButtons').empty();
                    //                    $('#selPersonCity').empty();

                    $('#selOrgCityHotButtons').append(new Option('Please Select City', ''));
                    //                    $('#selPersonCity').append(new Option('Please Select City', ''));

                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgCityHotButtons').append(new Option(vRows[p].City, vRows[p].ID, false, false));
                        //                        $('#selPersonCity').append(new Option(vRows[p].City, vRows[p].ID, false, false));
                    }
                    //                     $("#selOrgCity option:contains('Philippines')").attr('selected', 'selected');
                    //                     $("#selPersonCity option:contains('Philippines')").attr('selected', 'selected');

                    try {
                        $('#selOrgCityHotButtons').selectmenu('refresh', true);
                        //                        $('#selPersonCity').selectmenu('refresh', true);
                    } catch (e) {

                        $('#selOrgCityHotButtons').selectmenu();
                        $('#selOrgCityHotButtons').selectmenu('refresh', true);
                        //                        $('#selPersonCity').selectmenu();
                        //                        $('#selPersonCity').selectmenu('refresh', true);
                    }
                }


                proj.HotButtonNewCart = ClearGroup;

                function ClearGroup() {

                    divMain.hide();
                    divGroups.hide();
                    divProducts.hide();


                    $('#btnHotButtonMain').attr("data-theme", "green").addClass("ui-btn-up-green");
                    $('#btnHotButtonGroup1').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                    $('#btnHotButtonGroup2').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                    $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                    $('#spanHotButtonMain').css("color", "white");

                    ResetCreateTemplate();
                    ClearBreadcrumbs(1);
                    ClearBreadcrumbs(2);
                    ClearBreadcrumbs(3);

                    isGroup1Accessible = false;
                    isGroup2Accessible = false;
                    isGroup3Accessible = false;
                }; // END ClearGroup

                function GetData(id, vPriceLevelID) {

                    vPriceLevelID = vPriceLevelID || defaultPriceLevel;
                    //                    sessionStorage.PosMode = 'Hot Button Mode';
                    sessionStorage.IsHotButtonVisited = 'true';

                    ClearGroup();

                    var withContents = false;
                    var pBatchProductOnly;
                    if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                        withContents = true;
                        pBatchProductOnly = true;
                    }

                    if (OfflineMode) {
                        LoadButtons($('#divHotButtonsSection').find('[name=divHotButtonsRowNext]'), SalesMenu_offline().GetHotButtonsData(id, mID, vPriceLevelID, withContents, pBatchProductOnly), 0, 1);

                    } else {

                        AjaxSend({
                            url: '/ProductGroup/GetData',
                            data: {
                                ID: mID,
                                MenuID: id,
                                PriceLevelID: defaultPriceLevel,
                                WithContentsOnly: withContents
                            },
                            success: function (vData) {

                                LoadButtons($('#divHotButtonsSection').find('[name=divHotButtonsRowNext]'), vData, 0, 1);

                            }
                        }); // END Ajax
                    }
                }; // END LoadData

                function ClearQtyFields() {
                    $('#txtDecimalQty').focus();
                    //                    var val1 = $('#txtDecimalQty').val('1.0'); //store the value of the element
                    $('#txtDecimalQty').val(''); //clear the value of the element
                    //                    $('#txtDecimalQty').val('1.0'); //set that value back.  

                    $('#spanErrorQty').hide();
                }

                $('#btnSaveQty').click(function (e) {

                    e.preventDefault();
                    var pVal = $.trim($('#txtDecimalQty').val());
                    if (CStr2(pVal).trim().length > 0) {

                        if ((!IsNumeric(pVal))) {
                            $('#txtDecimalQty').focus();
                            $('#spanErrorQty').show();

                        } else {
                            $('#spanErrorQty').hide();
                            $('#popupDecimalInput').popup('close');

                            UpdateCartSKU(qtyInputProductID, qtyInputProductName, qtyInputQuantity, qtyInputUnitMeasure, qtyInputUnitPrice);

                        }

                    } else
                        $('#popupDecimalInput').popup('close');


                });

                var qtyInputProductID;
                var qtyInputProductName;
                var qtyInputQuantity;
                var qtyInputUnitMeasure;
                var qtyInputUnitPrice;
                function LoadButtons(vParentDiv, vData, index, level) {

                    var pDiv;
                    var pDivGroup;
                    var pIndex;

                    pDiv = divProductGroup.clone();

                    $('#spanHotButtonMain').text(sessionStorage.MenuTemplateName);

                    vParentDiv.append(pDiv);

                    if (vData.Data != undefined && index < vData.Data.Rows.length) {
                        function AddButton(vPosition) {
                            // vPosition = 0, 1 or 3

                            pDiv.find('[name=btnHotButton' + vPosition + ']').attr('id', 'btnHotButton_' + vData.Data.Rows[index + vPosition].ID);

                            if (vData.Data.Rows[index + vPosition].table_name == 'Group')
                                pDiv.find('[name=btnHotButtonItem' + vPosition + ']').text(vData.Data.Rows[index + vPosition].Name + ' *');
                            else
                                pDiv.find('[name=btnHotButtonItem' + vPosition + ']').text(vData.Data.Rows[index + vPosition].Name);

                            $('#btnHotButton_' + vData.Data.Rows[index + vPosition].ID).bind((InAndroid ? 'tap' : 'click'), function (e) {
                                if (isJQMGhostClick(e)) { return; }
                                //                                $.ionSound.play("water_droplet_3");
                                e.preventDefault();

                                if (!proj.HandlePosIsEditable())
                                    return false;

                                sessionStorage.IsDraft = 1;
                                var pData = vData.Data.Rows[index + vPosition];
                                mCurrentItem = pData;

                                if (vData.Data.Rows[index + vPosition].table_name == 'SKU') {
                                    //edited by francis h sept 26 2014
                                    proj.ProductID = vData.Data.Rows[index + vPosition].ProductID;
                                    if (CBool(sessionStorage.IsInventoryCountMode)) {
                                        //vOpt.DontClose = true;

                                        window.NewStockBrowser = true;
                                        startclock('#clockStock');
                                        ChangePage('Stock');
                                    } else {
                                        if (vData.Data.Rows[index + vPosition].AllowDecimal == '1') {
                                            $('#hdrQty').text(vData.Data.Rows[index + 1].ProductName);
                                            $('#popupDecimalInput').popup('open');
                                            $('#divQty').find('[name=btnSaveQty]').attr('id', 'btnSaveQty_' + vData.Data.Rows[index + vPosition].ID);

                                            ClearQtyFields();

                                            qtyInputProductID = vData.Data.Rows[index + vPosition].ProductID;
                                            qtyInputProductName = vData.Data.Rows[index + vPosition].ProductName;
                                            qtyInputQuantity = vData.Data.Rows[index + vPosition].Quantity;
                                            qtyInputUnitMeasure = vData.Data.Rows[index + vPosition].UnitMeasure;
                                            qtyInputUnitPrice = vData.Data.Rows[index + vPosition].DiscountedPrice;

                                        } else {
                                            proj.RebateData = null;
                                            proj.ModifierGroupID = null;
                                            // if (Stock_offline().GetInfoPos({ ProductID: vData.Data.Rows[index + vPosition].ProductID, CustomerID: sessionStorage.CustomerOrganizationID }).HasModifier) {
                                            if (!!!proj.POMode && CBool(pData.HasModifier) && location.pathname.toLocaleLowerCase() != '/billofmaterials') {
                                                //$('#popupItemModifier').trigger('create').popup().popup('open');
                                                NewItemModifier({
                                                    AllowSKUQty: true,
                                                    ProductData: pData,
                                                    success: function (e) {
                                                        var pItem = UpdateCartStock($.extend({
                                                            ModifierGroupID: proj.ModifierGroupID,
                                                            IsOrderSlipPrinted: 0,
                                                            GroupIdOfItemModifiers: NewID() // for grouping the productid and selected item modifier, temporary only
                                                        }, e), false, e.Quantity, {
                                                            DoAdd: true
                                                        });
                                                        DisplayStock(pItem);
                                                    }
                                                });
                                                //commented by arnie 4-3-17
                                                //ItemModifier({
                                                //    ProductName: vData.Data.Rows[index + vPosition].Name,
                                                //    ProductID: vData.Data.Rows[index + vPosition].ProductID,
                                                //    OnClickNoModification: function () {
                                                //        // UpdateCartSKU(vData.Data.Rows[index + vPosition].ProductID, vData.Data.Rows[index + vPosition].ProductName, vData.Data.Rows[index + vPosition].Quantity, vData.Data.Rows[index + vPosition].UnitMeasure, vData.Data.Rows[index + vPosition].DiscountedPrice); // old code
                                                //        var pItem = UpdateCartStock(pData);
                                                //        DisplayStock(pItem);
                                                //    },
                                                //    OnClickRebates: function () {
                                                //        ItemModifier_Rebates();
                                                //    },
                                                //    OnClickRebatesDone: function (vData) {
                                                //        var pModifierGroupID;

                                                //        if (vData != null) {
                                                //            proj.RebateData = vData;
                                                //            if (proj.RebateData) {
                                                //                pModifierGroupID = Business.TransInvoice.GetModifierGroupID(sessionStorage.CartOrderID, proj.ProductID, [proj.RebateData]);
                                                //                if (pModifierGroupID == null)
                                                //                    proj.ModifierGroupID = NewID();
                                                //                else
                                                //                    proj.ModifierGroupID = pModifierGroupID;
                                                //            }
                                                //        }
                                                //        mCurrentItem.ModifierGroupID = proj.ModifierGroupID;
                                                //        proj.HotButtonUpdateCallback = function (vData) {
                                                //            UpdateCartStock(mCurrentItem, 0, vData.Quantity);
                                                //        };
                                                //        ShowStockPage();
                                                //    }
                                                //});
                                            } else {
                                                // UpdateCartSKU(vData.Data.Rows[index + vPosition].ProductID, vData.Data.Rows[index + vPosition].ProductName, vData.Data.Rows[index + vPosition].Quantity, vData.Data.Rows[index + vPosition].UnitMeasure, vData.Data.Rows[index + vPosition].DiscountedPrice); // old code
                                                var pItem = UpdateCartStock($.extend({
                                                    ModifierGroupID: proj.ModifierGroupID,
                                                    IsOrderSlipPrinted: 0
                                                }, pData));
                                                DisplayStock(pItem);
                                            }
                                        }
                                    }
                                } else
                                    GetChildren(vData.Data.Rows[index + vPosition].ID, vData.Data.Rows[index + vPosition].Name, level);

                            });
                        } // END AddButton

                        AddButton(0);

                        if (index + 1 < vData.Data.Rows.length)
                            AddButton(1);

                        if (index + 2 < vData.Data.Rows.length)
                            AddButton(2);

                        pDivGroup = $('#divHotButtonsSection').find('[name=divHotButtonsRowNext]');

                        if (index < vData.Data.Rows.length) {
                            LoadButtons(pDivGroup, vData, index + 3, level);
                        }; // END for pIndex

                        pDiv.show();

                    } else {
                        pDiv.hide();
                    } // end if vdata undefined

                    //hide the template pattern
                    $('#divHotButtonsTemplate').hide();


                }; // END LoadBranch

                function ShowStockPage() {
                    clearTimeout(proj.RetransmitToSecondScreenTimeoutHandle);
                    if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                        ChangePage('../Stock');
                    else
                        ChangePage('Stock');
                }

                function GetChildren(id, buttonName, level) {
                    var withContents = false;
                    var pBatchProductOnly = false;
                    if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                        withContents = true;
                        pBatchProductOnly = true;
                    }

                    if (OfflineMode) {
                        ResetCreateTemplate();
                        // var vGroupData = SalesMenu_offline().GetGroupChildren(id, defaultPriceLevel, withContents);
                        var vGroupData = SalesMenu_offline().GetHotButtonsData(sessionStorage.HotButtonMenuID, id, defaultPriceLevel, withContents, pBatchProductOnly);

                        LoadButtons($('#divHotButtonsSection').find('[name=divHotButtonsRowNext]'), vGroupData, 0, level + 1);

                        if (level == 1) {
                            $('#spanHotButtonGroup1').text(buttonName);

                            $('#btnHotButtonMain').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                            $('#btnHotButtonGroup1').attr("data-theme", "green").addClass("ui-btn-up-green");
                            $('#btnHotButtonGroup2').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                            $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                            $('#spanHotButtonMain').css("color", "green");
                            $('#spanHotButtonGroup1').css("color", "white");

                            isGroup1Accessible = true;
                            group1Data = vGroupData;
                        }
                        else if (level == 2) {
                            $('#spanHotButtonGroup2').text(buttonName);

                            $('#btnHotButtonMain').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                            $('#btnHotButtonGroup1').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                            $('#btnHotButtonGroup2').attr("data-theme", "green").addClass("ui-btn-up-green");
                            $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                            $('#spanHotButtonMain').css("color", "green");
                            $('#spanHotButtonGroup1').css("color", "green");
                            $('#spanHotButtonGroup2').css("color", "white");

                            isGroup2Accessible = true;

                            group2Data = vGroupData;
                        }
                        else {
                            $('#spanHotButtonGroup3').text(buttonName);

                            $('#btnHotButtonMain').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                            $('#btnHotButtonGroup1').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                            $('#btnHotButtonGroup2').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                            $('#btnHotButtonGroup3').attr("data-theme", "green").addClass("ui-btn-up-green");

                            $('#spanHotButtonMain').css("color", "green");
                            $('#spanHotButtonGroup1').css("color", "green");
                            $('#spanHotButtonGroup2').css("color", "green");
                            $('#spanHotButtonGroup3').css("color", "white");

                            isGroup3Accessible = true;

                            group3Data = vGroupData;
                        }

                    } else {

                        AjaxSend({
                            url: '/ProductGroup/GetGroupChildren',
                            data: {
                                ID: id,
                                PriceLevelID: defaultPriceLevel,
                                WithContentsOnly: withContents
                            },
                            success: function (vData) {
                                ResetCreateTemplate();

                                LoadButtons($('#divHotButtonsSection').find('[name=divHotButtonsRowNext]'), vData, 0, level + 1);

                                if (level == 1) {
                                    $('#spanHotButtonGroup1').text(buttonName);

                                    $('#btnHotButtonMain').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                                    $('#btnHotButtonGroup1').attr("data-theme", "green").addClass("ui-btn-up-green");
                                    $('#btnHotButtonGroup2').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                                    $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                                    $('#spanHotButtonMain').css("color", "green");
                                    $('#spanHotButtonGroup1').css("color", "white");

                                    isGroup1Accessible = true;
                                    group1Data = vData;
                                }
                                else if (level == 2) {
                                    $('#spanHotButtonGroup2').text(buttonName);

                                    $('#btnHotButtonMain').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                                    $('#btnHotButtonGroup1').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                                    $('#btnHotButtonGroup2').attr("data-theme", "green").addClass("ui-btn-up-green");
                                    $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                                    $('#spanHotButtonMain').css("color", "green");
                                    $('#spanHotButtonGroup1').css("color", "green");
                                    $('#spanHotButtonGroup2').css("color", "white");

                                    isGroup2Accessible = true;

                                    group2Data = vData;
                                }
                                else {
                                    $('#spanHotButtonGroup3').text(buttonName);

                                    $('#btnHotButtonMain').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                                    $('#btnHotButtonGroup1').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                                    $('#btnHotButtonGroup2').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                                    $('#btnHotButtonGroup3').attr("data-theme", "green").addClass("ui-btn-up-green");

                                    $('#spanHotButtonMain').css("color", "green");
                                    $('#spanHotButtonGroup1').css("color", "green");
                                    $('#spanHotButtonGroup2').css("color", "green");
                                    $('#spanHotButtonGroup3').css("color", "white");

                                    isGroup3Accessible = true;

                                    group3Data = vData;
                                }




                            }

                        }); // AjaxSend
                    }
                }


                $('#btnHotButtonMain').bind((InAndroid ? 'tap' : 'click'), function (e) {
                    if (isJQMGhostClick(e)) { return; }
                    //                    $.ionSound.play("water_droplet_3");
                    $('#btnHotButtonMain').attr("data-theme", "green").addClass("ui-btn-up-green");
                    $('#btnHotButtonGroup1').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                    $('#btnHotButtonGroup2').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                    $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                    $('#spanHotButtonMain').css("color", "white");

                    ResetCreateTemplate();
                    ClearBreadcrumbs(1);
                    ClearBreadcrumbs(2);
                    ClearBreadcrumbs(3);

                    isGroup1Accessible = false;
                    isGroup2Accessible = false;
                    isGroup3Accessible = false;

                    GetData(sessionStorage.HotButtonMenuID);

                    return;
                });


                $('#btnHotButtonGroup1').bind((InAndroid ? 'tap' : 'click'), function (e) {
                    if (isJQMGhostClick(e)) { return; }
                    //                    $.ionSound.play("water_droplet_3");
                    if (isGroup1Accessible) {

                        $('#btnHotButtonGroup1').attr("data-theme", "green").removeClass("ui-btn-up-c").addClass("ui-btn-up-green");
                        $('#btnHotButtonGroup2').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");
                        $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").removeClass("ui-btn-hover-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                        $('#spanHotButtonGroup1').css("color", "white");

                        isGroup2Accessible = false;
                        isGroup3Accessible = false;

                        ResetCreateTemplate();
                        ClearBreadcrumbs(2);
                        ClearBreadcrumbs(3);
                        LoadButtons($('#divHotButtonsSection').find('[name=divHotButtonsRowNext]'), group1Data, 0, 2);

                    }

                    return;
                });



                $('#btnHotButtonGroup2').bind((InAndroid ? 'tap' : 'click'), function (e) {
                    if (isJQMGhostClick(e)) { return; }
                    //                    $.ionSound.play("water_droplet_3");
                    if (isGroup2Accessible) {

                        $('#btnHotButtonGroup2').attr("data-theme", "green").removeClass("ui-btn-hover-green").removeClass("ui-btn-up-c").addClass("ui-btn-up-green");
                        $('#btnHotButtonGroup3').removeClass("ui-btn-up-green").attr("data-theme", "c").addClass("ui-btn-up-c");

                        $('#spanHotButtonGroup2').css("color", "white");

                        isGroup3Accessible = false;
                        ClearBreadcrumbs(3);

                        ResetCreateTemplate();

                        LoadButtons($('#divHotButtonsSection').find('[name=divHotButtonsRowNext]'), group2Data, 0, 3);

                    }


                    return;
                });


                $('#btnHotButtonGroup3').bind((InAndroid ? 'tap' : 'click'), function (e) {
                    if (isJQMGhostClick(e)) { return; }
                    //                    $.ionSound.play("water_droplet_3");
                    if (isGroup3Accessible) {

                        ResetCreateTemplate();

                        LoadButtons($('#divHotButtonsSection').find('[name=divHotButtonsRowNext]'), group3Data, 0, 4);

                    }


                    return;
                });

                function ResetCreateTemplate() {
                    $('#divHotButtonsSection').contents().remove();

                    var pLV = $('#divHotButtonsSection');
                    var pItem;

                    pLV.html('');
                    pItem = $('<div name="divHotButtonsRowNext">')
                    pLV.append(pItem);
                    pLV.trigger("create");
                    $('#divHotButtonsTemplate').show();
                }

                function ClearBreadcrumbs(number) {
                    $('#spanHotButtonGroup' + number).text('');

                    //                    $('#btnHotButtonGroup' + number).attr("data-theme", "gray").removeClass("ui-btn-up-green").addClass("ui-btn-up-gray");
                }

                //add event for android to make click / tap faster
                $('#btnAddQTY').bind((InAndroid ? 'tap' : 'click'), function (e) {
                    if (mCurrentItem == null)
                        return;
                    var pItem = UpdateCartStock(mCurrentItem);
                    DisplayStock(pItem);
                });

                $('#btnDeleteQTY').bind((InAndroid ? 'tap' : 'click'), function (e) {
                    if (mCurrentItem == null)
                        return;
                    var pItem = UpdateCartStock(mCurrentItem, true);
                    DisplayStock(pItem);
                });

                var oldArrayLength;

                //Check if existing product
                function UpdateCartSKU(skuID, skuName, unitQty, unitMeasure, unitPrice) {
                    var existingProduct;
                    var existingProductID;
                    var existingProductQty;

                    // reset the cart order id. this happens when you go to stock viewer then put 0 qty only 1 item.
                    if (sessionStorage.CartOrderID == 'undefined')
                        sessionStorage.removeItem('CartOrderID');

                    if (proj.ProductID == skuID) {
                        // therefore the price level has changed
                        if (CStr2(sessionStorage.ProductPriceLevel) != '') {
                            defaultPriceLevel = sessionStorage.ProductPriceLevel;
                            sessionStorage.ProductPriceLevel = '';

                            priceLevelName = sessionStorage.ProductPriceLevelName;
                            //                            sessionStorage.ProductPriceLevelName = '';

                        }

                        if ($.trim($('#txtDecimalQty').val()).length > 0)
                            sessionStorage.LastQtyDecimal = $.trim($('#txtDecimalQty').val());
                        else
                            sessionStorage.LastQty = CDec2(sessionStorage.LastQty) + 1;

                        // UpdatePosHotButtons(skuID, skuName, unitQty, unitMeasure, unitPrice); // this is the old code
                        UpdateCartStock(skuID, proj.ModifierGroupID);
                    } else {
                        //if purchase order mode
                        if (proj.POMode)
                            defaultPriceLevel = sessionStorage.SupplierPriceLevelID;
                        else
                            defaultPriceLevel = $('#selPriceLevel').val();

                        priceLevelName = $("#selPriceLevel option:selected").text();


                        if (sessionStorage.CartOrderID != 'undefined') {

                            if (sessionStorage.InvoiceProductIDList != undefined) {
                                var arrProductIDValues = JSON.parse(sessionStorage.InvoiceProductIDList);
                                oldArrayLength = arrProductIDValues.length;
                                var isHasMatch = false;
                                for (var i = 0; i < arrProductIDValues.length; i++) {
                                    if (arrProductIDValues[i] != null) {
                                        existingProduct = arrProductIDValues[i].split('|');
                                        existingProductID = existingProduct[0];
                                        existingProductQty = existingProduct[1];


                                        if (existingProductID == skuID) {

                                            if (isEditQty) {
                                                if ($.trim($('#txtDecimalQty').val()).length > 0)
                                                    sessionStorage.LastQtyDecimal = $.trim($('#txtDecimalQty').val());
                                                else
                                                    sessionStorage.LastQty = parseFloat(sessionStorage.LastQty) + 1;

                                                UpdatePosHotButtons(skuID, skuName, unitQty, unitMeasure, unitPrice);
                                            } else {

                                                isHasMatch = true;
                                                if ($.trim($('#txtDecimalQty').val()).length > 0)
                                                    sessionStorage.LastQtyDecimal = $.trim($('#txtDecimalQty').val());
                                                else
                                                    sessionStorage.LastQty = parseInt(existingProductQty) + 1;

                                                isEditQty = true;

                                                UpdatePosHotButtons(skuID, skuName, unitQty, unitMeasure, unitPrice);
                                                return;

                                                //                                                //Check if existing product, prompt to overwrite
                                                //                                                MessageBox("The product already exists in the cart. Adding this item again will increase the current quantity by 1. Do you want to continue?",
                                                //                                                    'Add to Cart',
                                                //                                                    [
                                                //                                                        { Text: 'Yes',
                                                //                                                            Click: function () {
                                                //                                                                qty = parseInt(existingProductQty) + 1;

                                                //                                                                isEditQty = true;

                                                //                                                                $('.ui-dialog').dialog('close');
                                                //                                                                UpdatePosHotButtons(skuID, skuName, unitQty, unitMeasure, price);
                                                //                                                                return;
                                                //                                                            }
                                                //                                                        }
                                                //                                                    ,
                                                //                                                        { Text: 'No'
                                                //                                                        }
                                                //                                                    ]
                                                //                                                );


                                            }
                                            break;
                                        }

                                    } //end if not null
                                } //end for

                                if (!isHasMatch) {

                                    if ($.trim($('#txtDecimalQty').val()).length > 0)
                                        sessionStorage.LastQtyDecimal = $.trim($('#txtDecimalQty').val());
                                    else
                                        sessionStorage.LastQty = 1;

                                    UpdatePosHotButtons(skuID, skuName, unitQty, unitMeasure, unitPrice);
                                }

                            } else {
                                if ($.trim($('#txtDecimalQty').val()).length > 0)
                                    sessionStorage.LastQtyDecimal = $.trim($('#txtDecimalQty').val());
                                else
                                    sessionStorage.LastQty = 1;

                                UpdatePosHotButtons(skuID, skuName, unitQty, unitMeasure, unitPrice);
                            } //if else array of products is undefined


                        } else
                            MessageBox('Invalid Order ID', 'Error');
                    }

                }

                function UpdatePosHotButtons(skuID, skuName, unitQty, unitMeasure, unitPrice) {

                    if (parseFloat(sessionStorage.LastQty) >= 0 || $.trim($('#txtDecimalQty').val()).length > 0) {
                        var pData = {};
                        pData.StockID = skuID;
                        pData.CustomerID = sessionStorage.CustomerOrganizationID;
                        pData.NewCartMode = localStorage.NewCartMode;
                        pData.FowstID = sessionStorage.FowstID;
                        pData.ProductID = skuID;
                        pData.Stocks = [];
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

                        sessionStorage.LastQty = 1;
                        var pCartStock;

                        // pDtCartStock = Business.TransInvoice.GetCartStock(sessionStorage.CartOrderID, proj.ProductID, proj.PosMode ? 'pos' : '', proj.ModifierGroupID); // this is the replaced gud
                        pCartStock = GetCartStock(proj.ProductID, proj.ModifierGroupID);
                        sessionStorage.LastQty = parseFloat(pCartStock.Quantity) + 1;

                        if ($.trim($('#txtDecimalQty').val()).length > 0) {
                            //check if it is negative, make it 0
                            if (re.test(parseFloat(sessionStorage.LastQtyDecimal) * 1)) {
                                // +ve numbers (with decimal point like 2.3)
                                pData.Stocks[0] = {
                                    Qty: parseFloat(sessionStorage.LastQtyDecimal),
                                    UnitQty: parseFloat(unitQty),
                                    UnitPrice: parseFloat(unitPrice),
                                    PriceLevelID: defaultPriceLevel
                                };
                            } else {
                                sessionStorage.LastQty = 0;
                                sessionStorage.LastQtyDecimal = '';
                                $('#txtDecimalQty').val('');
                                // -ve numbers (with decimal point like -5.34)
                                pData.Stocks[0] = {
                                    Qty: 0,
                                    UnitQty: parseFloat(unitQty),
                                    UnitPrice: parseFloat(unitPrice),
                                    PriceLevelID: defaultPriceLevel
                                };

                            }

                        } else {
                            sessionStorage.LastQtyDecimal = '';

                            if (sessionStorage.LastQty > 0) {
                                pData.Stocks[0] = {
                                    Qty: parseFloat(sessionStorage.LastQty),
                                    UnitQty: parseFloat(unitQty),
                                    UnitPrice: parseFloat(unitPrice),
                                    PriceLevelID: defaultPriceLevel
                                };
                            }

                        }

                        var isDone = false;
                        var isMatch = false;
                        if (OfflineMode) {

                            isEditQty = false;
                            proj.ProductID = skuID;
                            sessionStorage.ProductName = skuName;
                            sessionStorage.ProductUnitQty = unitQty;
                            sessionStorage.ProductUnitMeasure = unitMeasure;
                            sessionStorage.ProductPrice = unitPrice;

                            var arrProductIDList = new Array();
                            var vDataUpdatePos;
                            if (window.DebugMode)
                                vDataUpdatePos = {
                                    "Result": "ok",
                                    "OrderID": "ad36711f-870c-431e-a7ed-5d6c791fb4cd",
                                    "DiscountedPrice": "150"
                                };
                            else
                                vDataUpdatePos = Stock_offline().UpdatePos(pData);
                            if (vDataUpdatePos != undefined) {

                                if (sessionStorage.InvoiceProductIDList != undefined) {

                                    var arrProductIDValues = JSON.parse(sessionStorage.InvoiceProductIDList);
                                    var pProduct;
                                    var pProductID;
                                    var pProductQty;

                                    if (arrProductIDValues.length > 0) {

                                        for (var i = 0; i < arrProductIDValues.length; i++) {
                                            if (arrProductIDValues[i] != null) {
                                                pProduct = arrProductIDValues[i].split('|');
                                                pProductID = pProduct[0];
                                                pProductQty = pProduct[1];


                                                if (pProductID == skuID) {
                                                    isMatch = true;
                                                    //if same sku, delete the existing, then insert the new qty
                                                    delete arrProductIDValues[i];

                                                    if ($.trim($('#txtDecimalQty').val()).length > 0) {
                                                        arrProductIDList[i] = skuID + '|' + sessionStorage.LastQtyDecimal;
                                                    } else {

                                                        if (parseFloat(sessionStorage.LastQty) > 0)
                                                            arrProductIDList[i] = skuID + '|' + sessionStorage.LastQty;
                                                        else
                                                            proj.ProductID = '';
                                                    }
                                                } else
                                                    arrProductIDList[i] = arrProductIDValues[i];

                                                if (i == arrProductIDValues.length - 1)
                                                    isDone = true;

                                            } //if not null
                                        }
                                    } else {
                                        isDone = true;
                                    }
                                } else {
                                    isDone = true;
                                    if ($.trim($('#txtDecimalQty').val()).length > 0 ? arrProductIDList[0] = skuID + '|' + sessionStorage.LastQtyDecimal : arrProductIDList[0] = skuID + '|' + sessionStorage.LastQty);

                                }


                                if (isDone) {
                                    if (!isMatch) {
                                        //add to the last slot
                                        if (sessionStorage.InvoiceProductIDList != undefined) {
                                            var arrProductIDValues2 = JSON.parse(sessionStorage.InvoiceProductIDList);
                                            if ($.trim($('#txtDecimalQty').val()).length > 0 ? arrProductIDList[arrProductIDValues2.length] = skuID + '|' + sessionStorage.LastQtyDecimal : arrProductIDList[arrProductIDValues2.length] = skuID + '|' + sessionStorage.LastQty);

                                        }

                                    }

                                    sessionStorage.InvoiceProductIDList = JSON.stringify(arrProductIDList);

                                    LoadData(vDataUpdatePos, skuName, unitMeasure); // todo: load from memory. not from sqlite

                                }
                            } else {
                                MessageBox('There was an error while processing your request.');
                            }


                        } else {
                            if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                                var vDataSample = { Result: 'ok', OrderID: 'cfe39df6-8809-4973-9515-810df75aed99', DiscountedPrice: unitPrice };

                                LoadData(vDataSample, skuName, unitMeasure);
                            } else {
                                AjaxSend({
                                    url: 'Stock/UpdatePos',
                                    data: pData,
                                    success: function (vData) {

                                        isEditQty = false;
                                        proj.ProductID = skuID;
                                        sessionStorage.ProductName = skuName;
                                        sessionStorage.ProductUnitQty = unitQty;
                                        sessionStorage.ProductUnitMeasure = unitMeasure;
                                        sessionStorage.ProductPrice = unitPrice;

                                        var arrProductIDList = new Array();

                                        if (vData.Error != undefined) {

                                            MessageBox(vData.Error);

                                            return;

                                        } else {

                                            if (sessionStorage.InvoiceProductIDList != undefined) {

                                                var arrProductIDValues = JSON.parse(sessionStorage.InvoiceProductIDList);
                                                var pProduct;
                                                var pProductID;
                                                var pProductQty;

                                                if (arrProductIDValues.length > 0) {

                                                    for (var i = 0; i < arrProductIDValues.length; i++) {

                                                        if (arrProductIDValues[i] != null) {
                                                            pProduct = arrProductIDValues[i].split('|');
                                                            pProductID = pProduct[0];
                                                            pProductQty = pProduct[1];


                                                            if (pProductID == skuID) {
                                                                isMatch = true;
                                                                //if same sku, delete the existing, then insert the new qty
                                                                delete arrProductIDValues[i];
                                                                if ($.trim($('#txtDecimalQty').val()).length > 0) {
                                                                    arrProductIDList[i] = skuID + '|' + sessionStorage.LastQtyDecimal;
                                                                } else {
                                                                    if (parseFloat(sessionStorage.LastQty) > 0)
                                                                        arrProductIDList[i] = skuID + '|' + sessionStorage.LastQty;
                                                                    else
                                                                        proj.ProductID = '';
                                                                }
                                                            } else
                                                                arrProductIDList[i] = arrProductIDValues[i];

                                                            if (i == arrProductIDValues.length - 1)
                                                                isDone = true;

                                                        } //if not null

                                                    }
                                                } else {
                                                    isDone = true;
                                                }

                                            } else {

                                                isDone = true;
                                                if ($.trim($('#txtDecimalQty').val()).length > 0 ? arrProductIDList[0] = skuID + '|' + sessionStorage.LastQtyDecimal : arrProductIDList[0] = skuID + '|' + sessionStorage.LastQty);

                                            }


                                            if (isDone) {

                                                if (!isMatch) {
                                                    //add to the last slot
                                                    if (sessionStorage.InvoiceProductIDList != undefined) {
                                                        var arrProductIDValues2 = JSON.parse(sessionStorage.InvoiceProductIDList);
                                                        if ($.trim($('#txtDecimalQty').val()).length > 0 ? arrProductIDList[arrProductIDValues2.length] = skuID + '|' + sessionStorage.LastQtyDecimal : arrProductIDList[arrProductIDValues2.length] = skuID + '|' + sessionStorage.LastQty);

                                                    }

                                                }

                                                sessionStorage.InvoiceProductIDList = JSON.stringify(arrProductIDList);

                                                LoadData(vData, skuName, unitMeasure);
                                            } //if isDone

                                        } //if error
                                    } // success
                                }); // AjaxSend
                            }


                        }; // if (OfflineMode) {

                    } else {
                        // if invalid qty

                        sessionStorage.LastQty = 0;
                        sessionStorage.LastQtyDecimal = '';
                    }
                }



                function LoadData(vData, skuName, unitMeasure) {

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

                    if ((vData.Result == 'ok')
                        || (vData.Warning != undefined)) {

                        function Done() {
                            //TODO
                            if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                                //                                proj.ShowCartBOM(); // see billofmaterials.js
                            }
                            else {
                                if ($.isFunction(proj.ShowCart)) {

                                    if (sessionStorage.CartOrderID == undefined || sessionStorage.CartOrderID == 'undefined')
                                        sessionStorage.CartOrderID = vData.OrderID;

                                    localStorage.removeItem('NewCartMode');
                                    sessionStorage.ProductPrice = vData.DiscountedPrice;

                                    if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                        proj.ShowCart(); // back to retail mode cart



                                    if ($.trim($('#txtDecimalQty').val()).length > 0) {
                                        if (parseFloat(sessionStorage.LastQtyDecimal) == 0) {
                                            $('#txtOrder').html('');
                                            $('#txtQtyUnit').html('');
                                        }
                                        else {

                                            $('#txtOrder').html('<label style="font-size:16px;">' + skuName + '</label><br/><br/> <label style="font-size:14px;">' + priceLevelName + ': ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + '<br/>' + 'Item total: <span class="cartListTotal">' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(sessionStorage.LastQtyDecimal)) + '</span></label>');
                                            $('#txtQtyUnit').html('<label style="font-size: 40px;">' + $.format.number(CDec2(sessionStorage.LastQtyDecimal), proj.QuantityFormat) + '</label><br/>' + unitMeasure);
                                        }

                                        $('#txtDecimalQty').val('');


                                    } else {
                                        if (parseFloat(sessionStorage.LastQty) == 0) {
                                            $('#txtOrder').html('');
                                            $('#txtQtyUnit').html('');
                                        }
                                        else {

                                            $('#txtOrder').html('<label style="font-size:16px;">' + skuName + '</label><br/><br/><label style="font-size:14px;">' + priceLevelName + ': ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + '<br/>' + 'Item total: <span class="cartListTotal">' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(sessionStorage.LastQty)) + '</span></label>');
                                            $('#txtQtyUnit').html('<label style="font-size: 40px;">' + sessionStorage.LastQty + '</label><br/>' + unitMeasure);
                                        }
                                    }

                                    var pDtCartStock = Business.TransInvoice.GetCartStock(sessionStorage.CartOrderID, proj.ProductID, proj.PosMode ? 'pos' : '', proj.ModifierGroupID);
                                    if (pDtCartStock.Rows.length > 0) {
                                        var pDrCartStock = pDtCartStock.Rows[0];
                                        $('#txtOrder').html('<label style="font-size:16px;">' + skuName + '</label><br/><br/><label style="font-size:14px;">' + priceLevelName + ': ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + '<br/>' + 'Item total: <span class="cartListTotal">' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(pDrCartStock.Quantity)) + '</span></label>');
                                        $('#txtQtyUnit').html('<label style="font-size: 40px;">' + pDrCartStock.Quantity + '</label><br/>' + unitMeasure);
                                    }
                                } else {
                                    MessageBox('Cart was successfully updated', 'Success');
                                    window.location = GetWindowPathFormat('salesmenu') + '#cart';
                                };
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

                }; // LoadData


                ////////////////

                proj.CartStock = [];
                /*
                proj.CartStock[0] = {
                ProductID: 'guid',
                Quantity: 123,
                ModifierGroupID: 'guid'
                };
                */

                function GetCartStock(vData, vDontCreate, vOptions) {

                    var pItem;
                    var pNewItem;
                    var pCartStockIndex;

                    vOptions = $.extend({
                        GetByGroupIdOfItemModifiers: false
                    }, vOptions);

                    vData.ProductID = ReadyGuid(vData.ProductID);
                    vData.ModifierGroupID = ReadyGuid(vData.ModifierGroupID);
                    if (!$.isArray(vData.ItemModifier))
                        vData.ItemModifier = [];
                    var pDataModifierJson = JSON.stringify(vData.ItemModifier);
                    var pItemModifierJson;

                    for (var pIndex in proj.CartStock) {
                        pItem = proj.CartStock[pIndex];
                        if (vOptions.GetByGroupIdOfItemModifiers) {
                            if (vData.GroupIdOfItemModifiers == pItem.GroupIdOfItemModifiers)
                                return pItem;
                        } else {
                            if (!$.isArray(pItem.ItemModifier))
                                pItem.ItemModifier = [];
                            pItemModifierJson = JSON.stringify(pItem.ItemModifier);
                            if ((ReadyGuid(pItem.ProductID) == vData.ProductID)
                                && (pItemModifierJson == pDataModifierJson)
                                && (CDec2(pItem.IsOrderSlipPrinted) == CDec2(vData.IsOrderSlipPrinted))) {

                                return pItem;
                            }
                        }
                    }

                    pNewItem = {
                        ProductID: vData.ProductID,
                        ProductName: vData.ProductName,
                        DiscountedPrice: (vData.DiscountedPrice || vData.UnitPrice), // added by arnie 91917 pag 1st load sa item sa cart walay price ang item pag iedit ang modifier
                        UnitMeasure: vData.UnitMeasure,
                        UnitMeasureID: vData.UnitMeasureID,
                        CategoryName: vData.CategoryName,
                        Quantity: 0,
                        ActualQuantity: 1,
                        IsOrderSlipPrinted: 0,
                        ItemModifier: (vData.ItemModifier != undefined ? vData.ItemModifier : []),
                        SelectedItemModifierID: (vData.SelectedItemModifierID != undefined ? vData.SelectedItemModifierID : []),
                        HasModifier: CBool(vData.HasModifier),
                        EditModifier: CBool(vData.EditModifier),
                        ModifierGroupID: vData.ModifierGroupID,
                        GroupIdOfItemModifiers: vData.GroupIdOfItemModifiers || vData.ModifierGroupID
                    };

                    if (!CBool(vDontCreate)) // new code 2016_09_16__11_21_07_7924
                        pCartStockIndex = proj.CartStock.push(pNewItem) - 1;

                    if (proj.CartData == null) {
                        // console.error('jsNG78UZC7SJ! proj.CartData is expected');
                        return pNewItem;
                    }

                    for (var pIndex in proj.CartData.Units.Rows) {
                        pItem = proj.CartData.Units.Rows[pIndex];
                        if (!$.isArray(pItem.ItemModifier))
                            pItem.ItemModifier = [];
                        pItemModifierJson = JSON.stringify(pItem.ItemModifier);
                        if (vOptions.GetByGroupIdOfItemModifiers) {
                            if (vData.GroupIdOfItemModifiers == pItem.GroupIdOfItemModifiers) {
                                if (IsNumeric(pCartStockIndex))
                                    proj.CartStock[pCartStockIndex] = pItem;
                                return pItem;
                            }
                        } else if ((ReadyGuid(pItem.ProductID) == vData.ProductID)
                            && (pItemModifierJson == pDataModifierJson)) {

                            if (CDec2(pItem.ActualQuantity) == 0)
                                pItem.ActualQuantity = 1;

                            if (proj.POMode)
                                pNewItem.Quantity += pItem.OrderedQuantity * pItem.ActualQuantity;
                            else
                                pNewItem.Quantity += pItem.Quantity * pItem.ActualQuantity;
                            pNewItem.GroupIdOfItemModifiers = pItem.GroupIdOfItemModifiers;
                            pNewItem.ModifierGroupID = pItem.ModifierGroupID;
                            if (pItem.UnitPrice != null)
                                pNewItem.DiscountedPrice = pItem.UnitPrice;
                        }
                    }

                    return pNewItem;

                } // END GetCartStock

                function UpdateCartStock(vData, vDoMinus, vQty, vOptions) {

                    var pItem;

                    vOptions = $.extend({
                        DoAdd: false,
                        GetByGroupIdOfItemModifiers: false
                    }, vOptions);

                    pItem = GetCartStock(vData, null, {
                        GetByGroupIdOfItemModifiers: vOptions.GetByGroupIdOfItemModifiers
                    });

                    if (IsNumeric(vQty)) {
                        if (vOptions.DoAdd)
                            pItem.Quantity += vQty;
                        else
                            pItem.Quantity = vQty;
                    } else {                        
                        if (CBool(vDoMinus)) {
                            if (CDec2(pItem.Quantity) >= 1)
                                pItem.Quantity--;
                            else
                                pItem.Quantity = 0;
                        } else
                            pItem.Quantity++;
                    }

                    return pItem;

                } // END UpdateCartStock

                //Edit by Edmund Uba, August 26, 2015

                $('#lnkCheckout:not(.bound)').addClass('bound').off('click').click(function () {
                    $("#popupLoad").popup("open"); // to know that the button was clicked
                    Android.AddLog($.format.date(new Date(), 'hh:mm:ss a'))
                    setTimeout(function () {
                        SaveCart();
                        Android.AddLog($.format.date(new Date(), 'hh:mm:ss a'))
                        if (proj.SOMode) {
                            proj.SalesMenu.PaymentsFunction({
                                DontPopupBilling: true
                            });
                            return;
                        }

                        if (sessionStorage.EnableHotButtonsContextualMenu) {
                            startclock('#clock');
                            //SaveCart();
                            //proj.ShowCart();
                            proj.SalesMenu.GoToPaymentsPage();
                            return;
                        } // END if (sessionStorage.EnableHotButtonsContextualMenu) {

                        if ($(window).width() > proj.WideWidth) {
                            if (CStr2(sessionStorage.CustomerTableNo) != '') {
                                DisplayTableNumberOnButton();
                                $('#selTableNo').val(sessionStorage.CustomerTableNo);
                                $('#lblLabel').html('Customer / Table No.:<br/>');
                            }
                            if (CStr2(sessionStorage.GasPumpNo) != '') {
                                $('#lblCustomerName').text(sessionStorage.GasPumpNo);
                                $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                                $('#lblLabel').html('Gas Pump No.:<br/>');
                            }
                            if (CStr2(sessionStorage.GasPumpNo) != '' && CStr2(sessionStorage.CustomerTableNo) != '') {
                                $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                                $('#selTableNo').val('');
                                $('#lblLabel').html('Customer:<br/>');
                            }
                            startclock('#clock');
                            //SaveCart();
                            proj.ShowCart();
                            proj.SalesMenu.GoToPaymentsPage();
                        }
                        $("#popupLoad").popup("close");
                    }, 100)                    
                });
                //End of edit

                proj.HotButtonDisplayStock = function (vData) {
                    DisplayStock(vData);
                }
                function DisplayStock(vData) {
                    var vData = JSON.parse(JSON.stringify(vData));
                    TransmitToSecondScreen();
                    var id;
                    if (CBool(vData.HasModifier) == false) {
                        //id = (vData.ProductID + vData.IsOrderSlipPrinted).toUpperCase();
                        id = (vData.ProductID).toUpperCase();
                    } else {
                        // id = (vData.ProductID + vData.IsOrderSlipPrinted + '_' + vData.GroupIdOfItemModifiers).toUpperCase();
                        id = (vData.ProductID + '_' + vData.GroupIdOfItemModifiers).toUpperCase();
                    }
                    //ForSidecart
                    //this function creates a list of items on the sidecart
                    if ($(window).width() > proj.WideWidth) {
                        $("#pushTemplate").hide();                       
                        var isExist = $("#pushSection").find("#" + id).is("#" + id);

                        if ((isExist && CBool(vData.HasModifier) == false) || (isExist && CBool(vData.HasModifier))) {
                            //If the item is already on the sidecart, update the quantity
                            $("#" + id).prependTo("#pushSection");
                            $("#" + id).find("#txtOrder").attr("id", vData.ProductID + "-Order");
                            //$("#" + vData.ProductID + vData.IsOrderSlipPrinted + "-Order").html('<label style="font-size:16px;">' + vData.ProductName + '</label><br/><br/><label style="font-size:14px;">' + priceLevelName + ': ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + '<br/>' + 'Item total: <span class="cartListTotal">' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity)) + '</span></label>');
                            $("#" + id + "-Order")
                                .html($('<div class="divTable" style="font-size:16px;">')
                                    .append($('<div class="divTableCell" style="border-right: 1px solid #CCC;">')
                                        .text(vData.ProductName + ' @ ' + AccountingFormat(parseFloat(vData.DiscountedPrice || vData.UnitPrice)) + ' each') // || vData.UnitPrice from stock info page
                                    )
                                    .append($('<div class="divTableCell cartListTotal" style="width:60px; text-align:right;">')
                                        .text(AccountingFormat(parseFloat(vData.DiscountedPrice || vData.UnitPrice) * parseFloat(vData.Quantity)))
                                    )
                                ).append($('<div class="divCartItemModifier">'))
                                //.html('<label style="font-size:16px;">' + vData.ProductName + ' @ ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + ' each </label><br/><div id="divCartItemModifier"></div><label>Item total: <span class="cartListTotal"> ' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity)) + '</span></label>');
                            $("#" + id).find("#txtQtyUnit").attr("id", vData.ProductID + "-QtyUnit");
                            $("#" + id + "-QtyUnit").html('<label style="font-size: 40px;">' + $.format.number(CDec2(vData.Quantity), proj.QuantityFormat) + '</label><br/>' + vData.UnitMeasure);

                            //update the data object
                            $("#" + id).find('.btnListDelete').data('data', vData);
                            $("#" + id).find('.btnListItem').data('data', vData);

                            var itemtotal = proj.DisplayStockIteModifier(vData); //salesmenu.js

                            //Update the cost of all total items
                            //Note: An instruction belows remove the 'comma' from the spanCartTotal
                            //      If comma is not remove, it will make an error
                            var cartTotal = 0; //$('#spanCartTotal').text();
                            //cartTotal = cartTotal.replace(/,/g, '');
                            //var initialTotal = AccountingFormat(parseFloat(vData.DiscountedPrice));
                            //initialTotal = initialTotal.replace(/,/g, '');
                            //cartTotal = parseFloat(cartTotal) + parseFloat(initialTotal);
                            $('.cartListTotal').each(function () {
                                cartTotal = cartTotal + CDec2($(this).text());
                            });
                            $('#spanCartTotal').text(AccountingFormat(cartTotal));
                        } else {
                            //If this item is not on the sidecart then create a new list for it
                            $("#pushTemplate").clone().prependTo("#pushSection").attr("id", id).show();
                            //The instructions below makes sure that the item is clickable or interactive
                            // or ready for editing.
                            $("#" + id).find('.btnListDelete').bind((InAndroid ? 'tap' : 'click'), function (e) {
                                e.preventDefault();
                                if (!proj.HandlePosIsEditable())
                                    return false;
                                var pData = $(this).data('data');
                                var pMsg = 'Do you want to remove ' + pData.ProductName + ' from your cart?';
                                var pParent = $(this).parent();
                                NewMessageBox(pMsg, 'Remove from cart', [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            PromptUserAccess('Allow removal of cart items', {
                                                OnClickOKSuccess: function () {
                                                    RemoveItem();
                                                },
                                                OnFailed: function () {
                                                    console.log('jsD23C4N8ETV!');
                                                }
                                            });

                                            function RemoveItem() {
                                                UpdateCartStock(pData, null, 0);
                                                pParent.remove();
                                                //Update the cost of all total items once removed
                                                var cartTotal = 0;
                                                $('.cartListTotal').each(function () {
                                                    cartTotal = cartTotal + CDec2($(this).text());
                                                });
                                                $('#spanCartTotal').text((cartTotal == 0 ? "0.00" : AccountingFormat(cartTotal)));
                                                proj.SaveCart();
                                                TransmitToSecondScreen();
                                                NewMessageBox('The item was successfully removed from the cart. Please do not forget to print a cancellation slip (and order slip if applicable).', '', null, {
                                                    Close: function() {
                                                    }
                                                });
                                            }
                                        }
                                    },
                                    {
                                        Text: 'No'
                                    }
                                ]);
                                return false;
                            }).data('data', vData);
                            $("#" + id).find('.btnListItem').bind((InAndroid ? 'tap' : 'click'), function (e) {
                                e.preventDefault();
                                if (!proj.HandlePosIsEditable())
                                    return false;
                                var pData = $(this).data('data');
                                proj.ProductID = pData.ProductID;
                                if (CBool(pData.HasModifier)) {
                                    NewMessageBox('', 'Options', [
                                        {
                                            Text: 'Go to Stock Info',
                                            Size: 3,
                                            Click: function () {
                                                proj.GroupIdOfItemModifiers = pData.GroupIdOfItemModifiers;
                                                sessionStorage.ItemModifier = JSON.stringify(pData.ItemModifier);
                                                GotoStockInfoPage();
                                            }
                                        },
                                        {
                                            Text: 'Edit Modifier',
                                            Size: 3,
                                            Click: function () {
                                                NewItemModifier({
                                                    EditModifier: true,
                                                    ProductData: pData,
                                                    success: function (e) {
                                                        var pItem;

                                                        pItem = UpdateCartStock($.extend({
                                                            ModifierGroupID: proj.ModifierGroupID,
                                                            IsOrderSlipPrinted: 0
                                                        }, e), false, 0, {
                                                            GetByGroupIdOfItemModifiers: true
                                                        });
                                                        DisplayStock(pItem);

                                                        pItem = UpdateCartStock($.extend(e, {
                                                            // ModifierGroupID: proj.ModifierGroupID, // old code
                                                            GroupIdOfItemModifiers: NewID(),
                                                            IsOrderSlipPrinted: 0
                                                        }), false, e.Quantity, {
                                                            DoAdd: true
                                                        });
                                                        DisplayStock(pItem);
                                                    }
                                                });
                                            }
                                        }
                                    ], { CloseButton: 'enable', AutoDismiss: true });                                    
                                } else {
                                    $("#popupLoad").popup("open");
                                    setTimeout(function () {
                                        GotoStockInfoPage()
                                    }, 1000);
                                }

                                function GotoStockInfoPage() {
                                    if (CStr2(sessionStorage.CustomerTableNo) != '') {
                                        DisplayTableNumberOnButton();

                                        $('#selTableNo').val(sessionStorage.CustomerTableNo);
                                        $('#lblLabel').html('Customer / Table No.:<br/>');
                                    }
                                    if (CStr2(sessionStorage.GasPumpNo) != '') {
                                        $('#lblCustomerName').text(sessionStorage.GasPumpNo);

                                        $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                                        $('#lblLabel').html('Gas Pump No.:<br/>');
                                    }
                                    if (CStr2(sessionStorage.GasPumpNo) != '' && CStr2(sessionStorage.CustomerTableNo) != '') {
                                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                                        $('#selTableNo').val('');
                                        $('#lblLabel').html('Customer:<br/>');
                                    }
                                    startclock('#clock');
                                    clearTimeout(proj.RetransmitToSecondScreenTimeoutHandle);
                                    SaveCart();

                                    proj.CartStock = [];

                                    //Making sure that modifiers are also included
                                    if (CBool(proj.PosMode) || CBool(proj.SOMode)) {
                                        proj.ModifierGroupID = vData.ModifierGroupID;
                                        if (window.OfflineMode) {
                                            if (proj.ModifierGroupID == null || proj.ModifierGroupID == '') {
                                                proj.RebateData = null;
                                            } else {
                                                proj.RebateData = Business.TransInvoice.GetRebates(proj.ModifierGroupID);
                                                proj.RebateData.ID = proj.RebateData.RebateID;
                                            }
                                        }
                                    }
                                    $("#popupLoad").popup("close");
                                    if (CStr2(proj.ProductID + vData.IsOrderSlipPrinted) != '') {
                                        window.NewStockBrowser = true;
                                        startclock('#clockStock');
                                        ChangePage('../Stock');
                                    } else {
                                        return;
                                    }
                                }
                            }).data('data', vData);;
                            $("#" + id).find("#txtOrder").attr("id", id + "-Order");
                            //$("#" + vData.ProductID + vData.IsOrderSlipPrinted + "-Order").html('<label style="font-size:16px;">' + vData.ProductName + '</label><br/><br/><label style="font-size:14px;">' + priceLevelName + ': ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + '<br/> Item total: <span class="cartListTotal"> ' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity)) + '</span></label>');
                            $("#" + id + "-Order")
                                .html($('<div class="divTable" style="font-size:16px;">')
                                    .append($('<div class="divTableCell" style="border-right: 1px solid #CCC;">')
                                        .text(vData.ProductName + ' @ ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + ' each')
                                    )
                                    .append($('<div class="divTableCell cartListTotal" style="width:60px; text-align:right;">')
                                        .text(AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity)))
                                    )
                                ).append($('<div class="divCartItemModifier">'))
                                //.html('<label style="font-size:16px;">' + vData.ProductName + ' @ ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + ' each </label><br/><div id="divCartItemModifier"></div><label>Item total: <span class="cartListTotal"> ' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity)) + '</span></label>');
                            $("#" + id).find("#txtQtyUnit").attr("id", id + "-QtyUnit");
                            $("#" + id + "-QtyUnit").html('<label style="font-size: 40px;">' + $.format.number(CDec2(vData.Quantity), proj.QuantityFormat) + '</label><br/>' + vData.UnitMeasure);
                            
                            var itemtotal = proj.DisplayStockIteModifier(vData); //salesmenu.js
                            
                            //Update the cost of all total items
                            //Note: An instruction belows remove the 'comma' from the spanCartTotal
                            //      If comma is not remove, it will make an error
                            var cartTotal = 0; //$('#spanCartTotal').text();
                            //cartTotal = cartTotal.replace(/,/g, '');
                            //var initialTotal = AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity));
                            //initialTotal = initialTotal.replace(/,/g, '');
                            //cartTotal = parseFloat(cartTotal) + parseFloat(initialTotal);
                            $('.cartListTotal').each(function () {
                                cartTotal = cartTotal + CDec2($(this).text());
                            });
                            $('#spanCartTotal').text(AccountingFormat(cartTotal));
                        }
                        $('*').removeClass('activeSideCartList');
                        $('#' + id).addClass('activeSideCartList').delay(500).queue(function () {
                            $(this).removeClass('activeSideCartList').clearQueue();
                        })
                    } else {
                        $('#btnOrder').data('data', vData);
                        if ((vData.SelectedItemModifierID || []).length > 0) { //(vData.SelectedItemModifierID || []) to avoid error
                            $('#txtOrder').html($('<div class="divTable" style="font-size:16px;">')
                                    .append($('<div class="divTableCell" style="border-right: 1px solid #CCC;">')
                                        .text(vData.ProductName + ' @ ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + ' each')
                                    )
                                    .append($('<div class="divTableCell cartListTotal" style="width:60px; text-align:right;">')
                                        .text(AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity)))
                                    )
                                ).append($('<div class="divCartItemModifier">'))
                            proj.DisplayStockIteModifier(vData, $('#txtOrder'), true); //salesmenu.js
                        } else {
                            $('#txtOrder').html('<label style="font-size:16px;">' + vData.ProductName + '</label><br/><br/><label style="font-size:14px;">' + priceLevelName + ': ' + AccountingFormat(parseFloat(vData.DiscountedPrice)) + '<br/>' + 'Item total: <span class="cartListTotal">' + AccountingFormat(parseFloat(vData.DiscountedPrice) * parseFloat(vData.Quantity)) + '</span></label>');
                        }
                        $('#txtQtyUnit').html('<label style="font-size: 40px;">' + $.format.number(CDec2(vData.Quantity), proj.QuantityFormat) + '</label><br/>' + vData.UnitMeasure);
                    }
                    if (CDec2(vData.Quantity) == 0)
                        $("#" + id).remove();
                } // END DisplayStock                

                function SaveCart() {

                    if (proj.BomMakerMode)
                        proj.BomMakerSave();
                    else {
                        proj.SaveCart();
                        //francis jan 05 2017
                        //save first before updating cart
                        if (!proj.SOMode && (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')) {
                            if (sessionStorage.POSSalesRepIndex != undefined) {
                                for (var i = 0; i <= CInt2(sessionStorage.POSSalesRepIndex) ; i++) {

                                    if (sessionStorage["POSSalesRep-" + i] != undefined) {

                                        var mData = [];

                                        arSalesRep = new Array();
                                        arSalesRep = JSON.parse(sessionStorage["POSSalesRep-" + i]);

                                        mData.push({
                                            ID: arSalesRep[1],
                                            PeopleName: arSalesRep[2],
                                        });
                                        Business.TransInvoice.AssignSalesRepresentatives(sessionStorage.CartOrderID, mData); // CartOrderID = invoiceID                                                                                                               
                                    }
                                }
                            }
                        }
                    }
                        
                    
                } // END SaveCart

                window.HotButtons.SaveCart = SaveCart;

                function TransmitToSecondScreen() {
                    var pTotal = GetTotal();
                    var pData = MergeCartStockData();
                    proj.CustomerScreenTransmitter.AddProduct({
                        // Products: proj.CartStock,
                        Products: pData,
                        // Products: GetStringObject('CartData.Units.Rows', proj),
                        Total: pTotal
                    });

                    function MergeCartStockData() {
                        var pOut = [];
                        var p;
                        var pItem;
                        var pStock;
                        var pData = GetStringObject('CartData.Units.Rows', proj);

                        if (pData != null)
                            for (p = 0; p <= pData.length - 1; p++) {
                                pItem = pData[p];
                                pItem = $.extend({
                                    DiscountedPrice: pItem.UnitPrice
                                }, pItem);
                                pStock = GetCartStock(pItem, true);
                                pStock = $.extend({
                                    UnitPrice: pStock.DiscountedPrice
                                }, pStock);
                                pOut.push(pStock);
                            }

                        if (proj.CartStock != null)
                            for (p = 0; p <= proj.CartStock.length - 1; p++) {
                                pItem = proj.CartStock[p];
                                for (var pOutIndex = 0; pOutIndex <= pOut.length - 1; pOutIndex++) {
                                    var pOutItem = pOut[pOutIndex];
                                    if ((pItem.ProductID == pOutItem.ProductID)
                                        && (pItem.ModifierGroupID == pOutItem.ModifierGroupID)
                                        && (pItem.IsOrderSlipPrinted == pOutItem.IsOrderSlipPrinted)) {

                                        pItem = null;
                                        break; // for (var pOutIndex = 0; pOutIndex <= pOut.length - 1; pOutIndex++) {
                                    }
                                } // END for (var pOutIndex = 0; pOutIndex <= pOut.length - 1; pOutIndex++) {

                                if (pItem != null) {
                                    pItem = $.extend({
                                        UnitPrice: pItem.DiscountedPrice
                                    }, pItem);
                                    pOut.push(pItem);
                                }
                            } // END for (p = 0; p <= proj.CartStock.length - 1; p++) {

                        return pOut;
                    } // END function MergeCartStockData() {
                } // END function TransmitToSecondScreen() {

                proj.HotButtonsUpdateCartStock = UpdateCartStock;
                proj.RetransmitToSecondScreen = function () {
                    clearTimeout(proj.RetransmitToSecondScreenTimeoutHandle);
                    TransmitToSecondScreen();
                    proj.RetransmitToSecondScreenTimeoutHandle = setTimeout(proj.RetransmitToSecondScreen, 2000);
                };
                //proj.RetransmitToSecondScreen();

                function GetTotal() {
                    var pTotal = 0;
                    //if (proj.CartStock = null)
                    //    return null;
                    //for (var p = 0; p <= proj.CartStock.length - 1; p++) {
                    //    var pItem = proj.CartStock[p];
                    //    pTotal += CDec2(pItem.Quantity) * CDec2(pItem.DiscountedPrice);
                    //}
                    return pTotal;
                }
            }); // $(function() {

        }); // require offline 
    });                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   // require require


