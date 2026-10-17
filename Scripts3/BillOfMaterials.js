require(['common',
    GetPathFormat('/Scripts3/Browser/LibraryOrganizationEmployee.js'),
    'promise!/Scripts3/model/BomMaker.js?crc=',
    'promise!/Scripts3/controller/BomMakerMake.js?crc='
], function () {

    $(function () {
        var headerHeight;
        var isPaymentPageLoaded = false;
        var isCustomerPageLoaded = false;
        //ADDITIONAL BY JANINE FEB 18
        var isCartEmpty = true;
        var isOnCartPage = true;
        var mBomMakerMake = new App.controller.BomMakerMake();

        proj.BomMakerMode = true;

        sessionStorage.IsOnWidescreen = 1;
        sessionStorage.IsBrowserSearch = 0;

        LoadCartDIV();
        NewCartBOM();

        $('#btnNewCart').live('click', function () {
            NewCartBOM();


            location.reload();
        });
        $('#btnRequestedFor').live('click', function () {

            SearchEmployee();
        });


        $('#btnMake').live('click', function () {
            mBomMakerMake.butSave_Click({
                success: function(vData) {
                    //proj.HotButtonNewCart();
                    //if (vData.id != null)
                    //    mID = vData.id;
                }
            });
        });

        $('#butBOMOptions').live('click', function () {
            $('#popupMoreOptionsHotButtons').popup('open');
        });

        proj.NewCartBOM = NewCartBOM;

        function NewCartBOM() {
            localStorage.NewCartMode = 1;
            sessionStorage.removeItem('CartOrderID');

            //additional by janine
            sessionStorage.removeItem('InvoiceProductIDList');
            proj.ProductID = '';
            sessionStorage.LastQty = 1;
            sessionStorage.ProductName = '';
            sessionStorage.ProductUnitQty = 0;
            sessionStorage.ProductUnitMeasure = '';
            sessionStorage.ProductPrice = 0;

            $('#divCartPanel').show();

            $('#divCartSection').hide();
            $('#divCartEmpty').show();

        }

        function SearchEmployee() {
            Browser.LibraryOrganizationEmployee.Click = function (e) {
                sessionStorage.RequestedFor = e.ID;

                $('#lblRequestedFor').html(HtmlEncode(e.PeopleName));


            }

            Browser.LibraryOrganizationEmployee.Show();
        }

        function NewCart() {
            localStorage.NewCartMode = 1;
            sessionStorage.removeItem('CartOrderID');
            // sessionStorage.removeItem('FowstID'); // commented by Arvin 2016_08_17__16_18_18_2236
            sessionStorage.removeItem('FowstName');

            //additional by janine
            sessionStorage.removeItem('InvoiceProductIDList');
            proj.ProductID = '';
            sessionStorage.LastQty = 1;
            sessionStorage.ProductName = '';
            sessionStorage.ProductUnitQty = 0;
            sessionStorage.ProductUnitMeasure = '';
            sessionStorage.ProductPrice = 0;
        };

        function LoadCartDIV() {
            AjaxSend({
                ExactUrl: '/SalesMenu/Cart?cache=1',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {

                    var wHeight = $(document).height();
                    $('#divCart').append(vData).trigger('create');

                    //$('#divCartDIV').css('height', CDec2(wHeight) - 110 + 'px');

                    LoadHeader();

                    App.Deferred.fulfill('ZXCVBNMASW');
                }
            });

        }
        function LoadHeader() {
            AjaxSend({
                ExactUrl: '/SalesMenu/Header?cache=1',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    $('#divHeader').append(vData).trigger('create');
                    headerHeight = $('#divHeader').height();

                    LoadHotButtons();
                    App.Deferred.fulfill('ZXCVBNMASE');

                    $('#divPOSHeader').hide();
                    $('#divBOMHeader').show();


                    // $('#divCartHeaderPOS').hide(); // temp comment
                    $('#divCartHeaderBOM').show();

                }
            });
        }

        function LoadHotButtons() {
            AjaxSend({
                ExactUrl: '/SalesMenu/HotButtonMode?cache=1',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    $('#divHotButtons').append(vData).trigger('create');
                    var wHeight = $('#divCart').height();
                    //$('#divHotButtons').css('height', CDec2(wHeight) - 110 + 'px');
                    $('.ui-loader').hide();
                    $("#main").css('min-height', CDec2(wHeight) - 110 + 'px');

                    NewCart();

                    //                          BrowseAddresses();
                    App.Deferred.fulfill('ZXCVBNMASQ');
                }
            });


        }

        function hideDivs() {
            $('#divHeader').hide();
            $('#divHotButtons').hide();
        }

        proj.BomMakerSave = function() {
            mBomMakerMake.butSave_Click({
                success: function(vData) {
                    
                }
            });
        };
    }); // $(function() {]]
});                                                                                                                                                               // END require

