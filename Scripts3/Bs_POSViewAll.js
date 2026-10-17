require(['common'], function () {

    $(function () {
        var headerHeight;
        var isPaymentPageLoaded = false;
        var isCustomerPageLoaded = false;
        var isCheckPaymentPageLoaded = false;
        var isDebitCardPaymentPageLoaded = false;
        var isCreditCardPaymentPageLoaded = false;
        var isSRVPaymentPageLoaded = false;
        var isGiftCertPaymentPageLoaded = false;
        var isOtherPaymentTypePageLoaded = false;
        var isSeniorCitizenPWDPageLoaded = false;
        var isSeniorCitizenPWDPageBillingLoaded = false;
        var pParams = GetUrlParameters();
        proj.POMode = (pParams.mode == 'po');

        window.pageHeader = {};
        sessionStorage.IsOnWidescreen = 1;
        sessionStorage.IsBrowserSearch = 0;

        window.onhashchange = function () {

//            if (location.hash == '#payment')
//                sessionStorage.CurrentPage = 'payment';
//            else {
//                if (location.hash != '#LibraryOrganization' && location.hash != '#SeniorCitizenPWD') {
//                    sessionStorage.CurrentPage = 'discount';
//                }
//            }

//            if (location.hash == '' && sessionStorage.IsOnWidescreen == 0 && sessionStorage.IsBrowserSearch == 0) {

//                if (sessionStorage.CurrentPage != 'discount')
//                    window.pageHeader.LoadHotButtons();

//            }

//            if (isPaymentPageLoaded && location.hash == '#payment' && !isInPaymentPage) {
//                sessionStorage.IsBrowserSearch = 0;

//                window.pageHeader.ShowPaymentsPage(false);

//            }

        };
		
		//This function is important because it makes the items from retail cart retainable when
		//being transfered to sidecart.
        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' || window.location.pathname.toLowerCase() == '/tableview/bsnewtable') {
        	//Load the retained items back to its original state
        	if (sessionStorage.CartOrderID && localStorage.getItem("tempData") != undefined ) {
            	proj.CartData = JSON.parse(localStorage.getItem("tempData"));
            	proj.RebateData = JSON.parse(localStorage.getItem("tempData2"));
            	proj.RebateData = localStorage.getItem("tempModifierID");
            	
            	//Always remember GIGO, Garbage In and Garbage Out
				//Remove templates after use
                localStorage.removeItem("tempData");
                localStorage.removeItem("tempData2");
                localStorage.removeItem("tempModifierID");
            }
            LoadCart();
        }

        $(window).resize(function () {
            ResizePanel();
        })

        function ResizePanel() {
            if ($(window).width() >= 992) {
                $('#divPanelLeft').removeClass('col-5').addClass('col-4');
                $('#divPanelRight').removeClass('col-7').addClass('col-8');                
            } else {
                $('#divPanelLeft').removeClass('col-4').addClass('col-5');
                $('#divPanelRight').removeClass('col-8').addClass('col-7');
            }
        }

        ResizePanel();

        function LoadCart() {
            AjaxSend({
                ExactUrl: '/SalesMenu/BsCart?cache=1',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {

                    var wHeight = $(document).height();
                    $('#divCart').append(vData)//.trigger('create');

                    $('#divCartDIV').css('height', '100%');

                    LoadHotButtons();

                    App.Deferred.fulfill('ZXCVBNMASW');
                }
            });

        }

        function LoadHeader() {
            AjaxSend({
                ExactUrl: '/SalesMenu/BsHeader?cache=1',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    $('#divHeader').append(vData)//.trigger('create');
                    headerHeight = $('#divHeader').height();
                    $('#divHotButtons').css('top', headerHeight + 'px');
                    //LoadHotButtons();
                    if (proj.POMode) {
                        $('#imgSupplier').attr('src', '/Content/images/Supplier.png');
                        $('#imgPosMode').attr('src', '/Content/images/Customer.png');
                    }
                    App.Deferred.fulfill('ZXCVBNMASE');

                    if (CStr2(GetUrlParameters().TableNo) != '') {
                        $('#divPOSHeader > .ui-block-a').hide();
                        $('#divPOSHeader').addClass('ui-grid-c').removeClass('ui-grid-d');
                        $('#divPOSHeader > .ui-block-b').addClass('ui-block-a').removeClass('ui-block-b');
                        $('#divPOSHeader > .ui-block-c').addClass('ui-block-b').removeClass('ui-block-c');
                        $('#divPOSHeader > .ui-block-d').addClass('ui-block-c').removeClass('ui-block-d');
                        $('#divPOSHeader > .ui-block-e').addClass('ui-block-d').removeClass('ui-block-e');
                    }                    

                    if (CBool(sessionStorage.FmcgMode)) {
                        $('#divPOSHeaderFmcg').show();
                        $('#divCartTotal').hide();
                        pageHeader.ShowPaymentsPage();
                    } else { // ELSE if (proj.FmcgMode) {
                        $('#divFmcgFooter').hide();
                    } // END if (proj.FmcgMode) {
                } // END success: function (vData) {
            }); // END AjaxSend({
        } // END function LoadHeader() {

        function LoadHotButtons() {
            AjaxSend({
                ExactUrl: '/SalesMenu/BsHotButtonMode?cache=1',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    $('#divHotButtons').append(vData)//.trigger('create
                    $('#divHotButtons').css('top', $('#divHeader').height() + 'px');
                    var wHeight = $('#divCart').height();
                    //$('#divHotButtons').css('height', CDec2(wHeight) - 110 + 'px');
                    $('.ui-loader').hide();
                    //$("#main").css('min-height', '0px');
                    LoadHeader();
                    App.Deferred.fulfill('ZXCVBNMASQ');
                }
            });


        }


        function hideDivs() {
            $('#divPaymentGiftCert').hide();
            $('#divPaymentOtherType').hide();
            $('#divSeniorCitizenPWD').hide();
            $('#divPaymentSRV').hide();
            $('#divPaymentCreditCard').hide();
            $('#divPaymentDebitCard').hide();
            $('#divPaymentCheck').hide();
            //$('#divHeader').hide();
            //$('#divHotButtons').hide();
            $('#divPayment').hide();
            $('#divCustomer').hide();
            $('#divPreviewBill').hide();
            $('#pageOrderTaker').hide();
        }


        window.pageHeader.LoadCustomer = function (callback) {
            sessionStorage.IsOnWidescreen = 0;

            //location.hash = 'customer';
            if (!isCustomerPageLoaded) {
                // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
                AjaxSend({
                    ExactUrl: '/SalesMenu/BsCustomerDetails?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#pageCustomerOptions').empty().append(vData)//.trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divCustomer').css('height', CDec2(wHeight) + 'px');

                        //hideDivs();
                        $('#pageCustomerOptions').pageshow();
                        //$('#divCustomer').css('overflow-y', 'scroll');
                        window.pageCustomerDetails.setCustomerInfo();
                        isCustomerPageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASI');

                        if ($.isFunction(callback)) {
                            callback()
                        }
                    }
                });
            } else {
                //hideDivs();
                $('#pageCustomerOptions').pageshow();
                window.pageCustomerDetails.setCustomerInfo();
            }



        };


        window.pageHeader.LoadHotButtons = function () {
            sessionStorage.IsOnWidescreen = 1;
            location.hash = '';

            //hideDivs();
            $('#pageMain').pageshow();
        };

        var isInPaymentPage = false;

        window.pageHeader.ShowPaymentsPage = function (showDetails) {
            sessionStorage.IsOnWidescreen = 0;
            //location.hash = 'payment';

            if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                proj.SalesMenu.GoToPaymentsPage();
                return;
            }

            if (!isPaymentPageLoaded) {
                AjaxSend({
                    ExactUrl: '/SalesPayments/Payment?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divPayment').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divPayment').css('height', CDec2(wHeight) + 'px');

                        ////hideDivs(); // modified by Arvin; Apr 8, 2015; precache payment page assets after loading POS data. 
                        //$('#divPayment').show(); // modified by Arvin; Apr 8, 2015; precache payment page assets after loading POS data. 
                        $('#divPayment').css('overflow-y', 'scroll');

                        //iload lang ni pag gamiton jud sa user -- arnie 4-20-2018
                        //LoadCheck();
                        //LoadDebitCard();
                        //LoadCreditCard();
                        //LoadGiftCert();
                        //LoadOtherPaymentType();
                        //LoadSRV();
                        //                        LoadSeniorCitizenPWD();


                        isPaymentPageLoaded = true;

                        App.Deferred.fulfill('ZXCVBNMASU');
                        Proceed(true);
                    }

                });

            } else 
                Proceed(false);

            function Proceed(isFirstLoad) {
                //hideDivs();
                if (CBool(isFirstLoad)) { // para walay puti mogawas pag load sa payments page -- arnie 4-20-2018
                    $('#divHeader').show();
                    $('#divHotButtons').show();
                } else {
                    $('#divPayment').show();
                }                

                $('#divPayment').css('overflow-y', 'scroll');

          
                if (showDetails) {
                    if ($.isFunction(proj.ShowPaymentDetails)) {
                        proj.ShowPaymentDetails();
                    }
                }
                isInPaymentPage = true;
            } // END function Proceed() {

        }; // END window.pageHeader.ShowPaymentsPage = function (showDetails) {

        window.pageHeader.ShowOrderTakerPage = function () {
            sessionStorage.IsOnWidescreen = 0;
            location.hash = 'pageOrderTaker';

            //hideDivs();
            $('#pageOrderTaker').show();
            $('#pageOrderTaker').css('overflow-y', 'scroll');
            window.popSalesMenuOrderTaker.Show(true);
        };

        window.pageHeader.ShowCheckPaymentPage = function (callback) {
            LoadCheck(function () {
                sessionStorage.IsOnWidescreen = 0;
                location.hash = 'check';

                //hideDivs();
                $('#divPaymentCheck').show();

                $('#divPaymentCheck').css('overflow-y', 'scroll');

                if ($.isFunction(callback)) {
                    callback();
                }
            })
        };

        window.pageHeader.ShowDebitCardPaymentPage = function (callback) {
            LoadDebitCard(function () {
                sessionStorage.IsOnWidescreen = 0;
                location.hash = 'debit';

                //hideDivs();
                $('#divPaymentDebitCard').show();

                $('#divPaymentDebitCard').css('overflow-y', 'scroll');

                if ($.isFunction(callback)) {
                    callback();
                }
            })
        };

        window.pageHeader.ShowCreditCardPaymentPage = function (callback) {
            LoadCreditCard(function () {
                sessionStorage.IsOnWidescreen = 0;
                location.hash = 'credit';

                //hideDivs();
                $('#divPaymentCreditCard').show();

                $('#divPaymentCreditCard').css('overflow-y', 'scroll');

                if ($.isFunction(callback)) {
                    callback();
                }
            })
        };

        window.pageHeader.ShowSRVPaymentPage = function (callback) {
            LoadSRV(function () {
                sessionStorage.IsOnWidescreen = 0;
                location.hash = 'srv';

                //hideDivs();
                $('#divPaymentSRV').show();

                $('#divPaymentSRV').css('overflow-y', 'scroll');

                if ($.isFunction(callback)) {
                    callback();
                }
            })
        };

        window.pageHeader.ShowGiftCertPaymentPage = function (callback) {
            LoadGiftCert(function () {
                sessionStorage.IsOnWidescreen = 0;
                location.hash = 'giftcert';

                //hideDivs();
                $('#divPaymentGiftCert').show();

                $('#divPaymentGiftCert').css('overflow-y', 'scroll');

                if ($.isFunction(callback)) {
                    callback();
                }
            })
        };

        window.pageHeader.ShowOtherTypePaymentPage = function (callback) {
            LoadOtherPaymentType(function () {
                sessionStorage.IsOnWidescreen = 0;
                location.hash = 'othertype';

                //hideDivs();
                $('#divPaymentOtherType').show();

                $('#divPaymentOtherType').css('overflow-y', 'scroll');

                if ($.isFunction(callback)) {
                    callback();
                }
            })
        };

        window.pageHeader.ShowSeniorCitizenPWDPage = function (callback) {
            LoadSeniorCitizenPWD(function () {
                sessionStorage.IsOnWidescreen = 0;
                location.hash = 'seniorcitizenpwd';

                //hideDivs();
                $('#divSeniorCitizenPWD').show();

                $('#divSeniorCitizenPWD').css('overflow-y', 'scroll');

                if ($.isFunction(callback)) {
                    callback();
                }
            })
        };


        function LoadCheck(callback) {

            if (!isCheckPaymentPageLoaded) {
                Android.Toast('Please wait');
                AjaxSend({
                    ExactUrl: '/SalesPayments/CheckPayment?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divPaymentCheck').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divPaymentCheck').css('height', CDec2(wHeight) + 'px');

                        isCheckPaymentPageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASL');

                        callback();
                    }
                });

            } else {
                //window.pageHeader.ShowCheckPaymentPage();
                callback();
            }

        }

        function LoadDebitCard(callback) {

            if (!isDebitCardPaymentPageLoaded) {
                Android.Toast('Please wait');
                AjaxSend({
                    ExactUrl: '/SalesPayments/DebitCardPayment?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divPaymentDebitCard').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divPaymentDebitCard').css('height', CDec2(wHeight) + 'px');

                        isDebitCardPaymentPageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASK');
                        callback();
                    }
                });
            } else {
                //window.pageHeader.ShowDebitCardPaymentPage();
                callback();
            }
        }

        function LoadCreditCard(callback) {

            if (!isCreditCardPaymentPageLoaded) {
                Android.Toast('Please wait');
                AjaxSend({
                    ExactUrl: '/SalesPayments/CreditCardPayment?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divPaymentCreditCard').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divPaymentCreditCard').css('height', CDec2(wHeight) + 'px');

                        isCreditCardPaymentPageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASJ');
                        callback();
                    }
                });

            } else {
                //window.pageHeader.ShowCreditCardPaymentPage();
                callback();
            }
        }

        function LoadSRV(callback) {
            if (!isSRVPaymentPageLoaded) {
                Android.Toast('Please wait');
                AjaxSend({
                    ExactUrl: '/SalesPayments/SRVPayment?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divPaymentSRV').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divPaymentSRV').css('height', CDec2(wHeight) + 'px');

                        isSRVPaymentPageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASH');
                        callback();
                    }
                });

            } else {
                //window.pageHeader.ShowSRVPaymentPage();
                callback();
            }
        }

        function LoadGiftCert(callback) {
            if (!isGiftCertPaymentPageLoaded) {
                Android.Toast('Please wait');
                AjaxSend({
                    ExactUrl: '/SalesPayments/GiftCertPayment?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divPaymentGiftCert').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divPaymentGiftCert').css('height', CDec2(wHeight) + 'px');

                        isGiftCertPaymentPageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASG');
                        callback();
                    }
                });
            } else {
                //window.pageHeader.ShowGiftCertPaymentPage();
                callback();
            }
        }

        function LoadOtherPaymentType(callback) {
            if (!isOtherPaymentTypePageLoaded) {
                Android.Toast('Please wait');
                AjaxSend({
                    ExactUrl: '/SalesPayments/OtherPaymentType?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divPaymentOtherType').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divPaymentOtherType').css('height', CDec2(wHeight) + 'px');

                        isOtherPaymentTypePageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASF');
                        callback();
                    }
                });
            } else {
                //window.pageHeader.ShowOtherTypePaymentPage();
                callback();
            }
        }

        function LoadSeniorCitizenPWD(callback) {
            if (!isSeniorCitizenPWDPageLoaded) {
                Android.Toast('Please wait');
                AjaxSend({
                    ExactUrl: '/SalesPayments/SeniorCitizenPWD?cache=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#divSeniorCitizenPWD').append(vData).trigger('create');

                        var wHeight = $('#divCart').height();
                        //$('#divSeniorCitizenPWD').css('height', CDec2(wHeight) + 'px');

                        isSeniorCitizenPWDPageLoaded = true;
                        App.Deferred.fulfill('ZXCVBNMASD');
                        callback();
                    }
                });
            } else {
                //window.pageHeader.ShowSeniorCitizenPWDPage();
                callback();
            }
        }

        // window.pageHeader.ShowPaymentsPage(); // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
    }); // $(function() {]]

    // BILLING events. additional by janine dec 16

});                                                                                                                                                             // END require

