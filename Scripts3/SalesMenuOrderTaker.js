require([
    'common'
], function () {
    $(function () {

        $('body').on('change', '#selFOWSTOrderTaker', function () {
            $('#cbOrderTakerDeliveryAddress').val($(this).val()).selectmenu('refresh');
            $('#cbOrderTakerBillingAddress').val($(this).val()).selectmenu('refresh');
        })
        .on('click', '#btnCancelOrder', function () {
            $.mobile.changePage('#pageMain');
        })
        .on('click', '#btnOrderDetailsSave', function () {
            PopupButtonList('', 'Save Options', [
                {
                    HasImage: true,
                    Text: '<img src="/Content/images/savefinalized.jpg" align="absmiddle" class="ui-li-thumb" /> <b>Save and Finalize Transaction</b> <br /> <div style="text-align:left !important; font-size:14px; font-weight:normal;">Save and send to head office. Transaction can be searched but not edited after saving.</div>',
                    Click: function () {
                        Business.TransInvoice.SaveAndFinalize(sessionStorage.CartOrderID);
                        Form_Submit();
                    }
                },
                {
                    HasImage: true,
                    Text: '<img src="/Content/images/savedraft.jpg" align=" " class="ui-li-thumb" /> <b>Save as Draft Transaction</b> <br /> <div style="text-align:left !important; font-size:14px; font-weight:normal;">Will not be sent to the head office. Transaction can still be searched and edited.</div>',
                    Click: function () {
                        Business.TransInvoice.SaveAsDraft(sessionStorage.CartOrderID);
                        Form_Submit();
                    }
                }
            ], { AutoDismiss: true })
        })
        .on('click', '#btnHomeOrderDetails', function () {
            window.location = '/People/PeopleMenu';
        })
        .on('click', '#btnDeliveryDate', function (e) {
            e.preventDefault();
            $(".OrderDetailsCalendar").calendar({
                format: "mmmm dd, yyyy",
                multiSelect: false,
                otherDays: true,
                getDates: function (data) {
                    $("#txtDateNowOrderDetails").text(data);
                }
            });
            $("#txtDateNowOrderDetails").text($.format.date(new Date(), 'MMM dd, yyyy'));
            $('#panDatesOrderDetails').popup('open');
        })
        .on('click', '#btnCalCancel', function () {
            $('#panDatesOrderDetails').popup('close');
        })
        .on('click', '#btnCalDone', function () {
            $('#panDatesOrderDetails').popup('close');
            if (ParseDate($("#txtDateNowOrderDetails").text()) < RemoveTime(new Date())) {
                NewMessageBox('Delivery Date cannot be earlier than the current date.');
                return;
            }
            $('#btnDeliveryDate').text($("#txtDateNowOrderDetails").text()).button('refresh');

            var selectedDate = new Date($("#txtDateNowOrderDetails").text());
            $('#selDeliveryMonth').val(selectedDate.getMonth() + 1);
            $('#selDeliveryDay').val(selectedDate.getDate());
            $('#selDeliveryYear').val(selectedDate.getFullYear());
        })

        var mPage;
        var cbDeliveryAddress;
        var cbBillingAddress;
        var txtRemarks;
        var cbFOWSTOrderTaker;

        //additional
        var txtDeliveryStreetNo;
        var txtDeliveryAddress;
        var selDeliveryCity;
        var selDeliveryCountry;

        var txtPurchaseOrderNo;
        var selPOMonth;
        var selPODay;
        var selPOYear;
        var txtBuyerName;
        var selDeliveryMonth;
        var selDeliveryDay;
        var selDeliveryYear;
        var isPurchaseOrder = false;
        var selInvoiceTypeCode;

        $(window).on("navigate", function (event, data) {
            if (data.state.direction == 'back') {
                if (GetUrlParameters().mode == 'po') {
                    if ($('#pageOrderTaker').css('display') == 'block') {

                    }
                    $('#pageOrderTaker').hide();
                    $('#divHeader').show();
                    $('#divHotButtons').show();
                }
            }
        });

        window.popSalesMenuOrderTaker = {};

        proj.SalesMenuOrderTakerLoadView = function() {
            // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
            AjaxSend({
                ExactUrl: '/SalesMenu/OrderTaker?cache=1',
                type: 'GET',
                dataType: 'text',
                async: false,
                success: function (vData) {
                    $('#pageOrderTaker').append(vData).trigger('create');
                    //   $('[data-role=page]:first').append(vData).trigger('create');

                    App.Deferred.fulfill('FQ26E5TP76');
                }
            });
        }; // END SalesMenuOrderTakerLoadView

        // popSalesMenuOrderTaker.OnSubmit = function() {};

      

        window.popSalesMenuOrderTaker.Show = function (vDontChangePage) {
            if (!CBool(vDontChangePage)) {
                $.mobile.changePage('#pageOrderTaker');
                $('#pageOrderTaker').show();
            }
            Loaded();
            GetData();
            //                var pElement = mPage;

            //                //show sales order taker page
            //                $('[data-role=footer_no_theme]:first').hide();
            //                $('#mainPageBody').hide();
            //                pElement.show();
            //                pElement.popup('open');
            //                pElement = pElement.closest('.ui-popup-container');
            //                if (pElement.length == 0)
            //                    console.trace('popup container for popupSalesRepresentatives not found');
            //                else
            //                // know issue: pop up dimension will revert to default after browser window resize
            //                    pElement.css({
            //                        left: 0,
            //                        width: '80%',
            //                        margin: 'auto 10%'
            //                    });
            popSalesMenuOrderTaker.LoadCombo();
            $('#spanOrderTakerSupplier').text(sessionStorage.SupplierOrganizationName).button('refresh');
            $('#spanOrderTakerCustomer').text(sessionStorage.CustomerOrganizationName).button('refresh');

            if (proj.POMode) {
                isPurchaseOrder = true;
                $('#divPurchaseOrderCollapsible').trigger('expand');
                $('#lblSalesOrderFOWST').html('Purchasing Branch / FOWST <span style="color: #FF0000;" >*</span>');
                $('#divPurchaseOrder').hide();
                $('#divInvoiceTypeCode').hide();
            }

        }; // END Show

        window.popSalesMenuOrderTaker.Clear = function () {

            if (mPage == undefined)
            // throw 'Loaded not yet called js6WSUQEZJNH!';
                return;

            cbDeliveryAddress.html('');
            cbBillingAddress.html('');
            txtRemarks.val('');

            popSalesMenuOrderTaker.ClearNewAddress();



        }; // END Clear

        //additional
        window.popSalesMenuOrderTaker.ClearNewAddress = function () {

            if (mPage == undefined)
            // throw 'Loaded not yet called js6WSUQEZJNH!';
                return;


            txtDeliveryStreetNo.val('');
            txtDeliveryAddress.val('');
            selDeliveryCity.html('');
            selDeliveryCountry.html('');
            cbFOWSTOrderTaker.val('');
            txtPurchaseOrderNo.val('');
            setMonthCollection("selPOMonth");
            setYearCollection("selPOYear");
            //                var d = new Date();
            //                selPOMonth.val(d.getMonth() + 1);
            //                selPOMonth.selectmenu('refresh');
            //                selPOYear.val(d.getFullYear());
            //                selPOYear.selectmenu('refresh');
            setDayCollection('selPOMonth', 'selPODay', 'selPOYear', true);
            //                selPODay.val(d.getDate());
            //                selPODay.selectmenu('refresh');

            //setMonthCollection("selDeliveryMonth");
            //setYearCollection("selDeliveryYear");
            //setDayCollection('selDeliveryMonth', 'selDeliveryDay', 'selDeliveryYear', true);

            txtBuyerName.val('');


        }; // END Clear

        function GetData() {
            
            var pDT = Business.TransInvoice.GetCart(sessionStorage.CartOrderID);
            if (pDT.Invoices.Rows.length == 0) {
                // put some error message here
                return;
            }
            var pDR = pDT.Invoices.Rows[0];

            txtRemarks.text(pDR.Remarks);
            $('#btnDeliveryDate').text($.format.date(new Date(pDR.DeliveryDate), 'MMM d, yyyy')).button('refresh');
        }

        function Loaded() {

            mPage = $('#pageOrderTaker');
            cbDeliveryAddress = mPage.find('#cbOrderTakerDeliveryAddress');
            cbBillingAddress = mPage.find('#cbOrderTakerBillingAddress');
            txtRemarks = mPage.find('#txtOrderTakerRemarks');


            //additional
            txtDeliveryStreetNo = mPage.find('#txtDeliveryStreetNo');
            txtDeliveryAddress = mPage.find('#txtDeliveryAddress');
            selDeliveryCity = mPage.find('#selDeliveryCity');
            selDeliveryCountry = mPage.find('#selDeliveryCountry');
            cbFOWSTOrderTaker = mPage.find('#selFOWSTOrderTaker');

            txtPurchaseOrderNo = mPage.find('#txtPurchaseOrderNo');
            selPOMonth = mPage.find('#selPOMonth');
            selPODay = mPage.find('#selPODay');
            selPOYear = mPage.find('#selPOYear');
            txtBuyerName = mPage.find('#txtBuyerName');

            selDeliveryMonth = mPage.find('#selDeliveryMonth');
            selDeliveryDay = mPage.find('#selDeliveryDay');
            selDeliveryYear = mPage.find('#selDeliveryYear');
            selInvoiceTypeCode = mPage.find('#selInvoiceTypeCode');
            $(selInvoiceTypeCode.find('option:first')).attr('selected', 1);
            selInvoiceTypeCode.selectmenu('refresh');

            CheckIfDeliveryBillingAddressIsRestricted();

            mPage.find('form').submit(Form_Submit);

            startclock('#clockOrder');

            //$('#divContentOrderDetails').css('margin-top', $('#divHeaderOrderDetails').height() + 15 + 'px');
            $('#btnDeliveryDate').text($.format.date(new Date(), 'MMM dd, yyyy')).button('refresh');
            var currentDate = new Date();
            $('#selDeliveryMonth').val(currentDate.getMonth() + 1);
            $('#selDeliveryDay').val(currentDate.getDate());
            $('#selDeliveryYear').val(currentDate.getFullYear());

            //                mPage.popup();
            //                mPage.show();
        } // END Loaded

        function CheckIfDeliveryBillingAddressIsRestricted() {
            var pSql = 'SELECT * FROM LibraryPurchaseOrderSettings ORDER BY LastUpdated DESC';
            var pDT = Sql.Execute(pSql);

            if (pDT.Rows.length > 0) {
                if (CBool(pDT.Rows[0].IsRestrict)) {
                    cbDeliveryAddress.selectmenu('disable');
                    cbBillingAddress.selectmenu('disable');
                }
                else {
                    cbDeliveryAddress.selectmenu('enable');
                    cbBillingAddress.selectmenu('enable');
                }
            }
            else {
                cbDeliveryAddress.selectmenu('enable');
                cbBillingAddress.selectmenu('enable');
            }
        }

        function Form_Submit(e) {
            //e.preventDefault();
            if (!$.isFunction(popSalesMenuOrderTaker.OnSubmit))
                throw 'popSalesMenuOrderTaker.OnSubmit must be a function jsLDHFQV5WXA!';

            if ($.trim(cbDeliveryAddress.val()) == '') {
                Android.Toast('Delivery Address is required. Please add an address.');
                $('#divCollapse').trigger('expand');

                return;
            }

            if ($.trim(cbBillingAddress.val()) == '') {
                Android.Toast('Billing Address is required. Please add an address.');
                $('#divCollapse').trigger('expand');

                return;
            }

            if ($.trim(cbFOWSTOrderTaker.val()) == '') {
                Android.Toast('FOWST is required. Please select a location where you want the order to be processed.');
                return;
            }

            if ($.trim(selPOMonth.val()) != "" || $.trim(selPODay.val()) != "" || $.trim(selPOYear.val()) != "") {
                if ($.trim(selPOMonth.val()) == "") {
                    Android.Toast('Please enter a valid purchase order date.');
                    return;
                }
                if ($.trim(selPODay.val()) == "") {
                    Android.Toast('Please enter a valid purchase order date.');
                    return;
                }
                if ($.trim(selPOYear.val()) == "") {
                    Android.Toast('Please enter a valid purchase order date.');
                    return;
                }
            }

            if ($.trim(selDeliveryMonth.val()) != "" || $.trim(selDeliveryDay.val()) != "" || $.trim(selDeliveryYear.val()) != "") {
                if ($.trim(selDeliveryMonth.val()) == "") {
                    Android.Toast('Please enter a valid delivery date.');
                    return;
                }
                if ($.trim(selDeliveryDay.val()) == "") {
                    Android.Toast('Please enter a valid delivery date.');
                    return;
                }
                if ($.trim(selDeliveryYear.val()) == "") {
                    Android.Toast('Please enter a valid delivery date.');
                    return;
                }
            }

            if ($.trim(selDeliveryMonth.val()) == "" && $.trim(selDeliveryDay.val()) == "" && $.trim(selDeliveryYear.val()) == "") {

                Android.Toast('Please enter a valid delivery date.');
                return;

            }

            var pInvoiceTypeCode;

            if (selInvoiceTypeCode.prop('selectedIndex') != 0)
                pInvoiceTypeCode = selInvoiceTypeCode.val();

            popSalesMenuOrderTaker.OnSubmit({
                DeliveryDate: $('#btnDeliveryDate').text(), // $.trim(selDeliveryMonth.val()) != '' ? selDeliveryMonth.val() + '/' + selDeliveryDay.val() + '/' + selDeliveryYear.val() : '',
                DeliveryAddressID: cbDeliveryAddress.val(),
                BillingAddressID: cbBillingAddress.val(),
                PurchaseOrderNo: txtPurchaseOrderNo.val(),
                PurchaseOrderDate: $.trim(selPOMonth.val()) != '' ? selPOMonth.val() + '/' + selPODay.val() + '/' + selPOYear.val() : '',
                PurchaserName: txtBuyerName.val(),
                FowstID: cbFOWSTOrderTaker.val(),
                IsPurchaseOrder: isPurchaseOrder,
                InvoiceTypeCode: pInvoiceTypeCode,
                Remarks: txtRemarks.val()
            });
        } // END Form_Submit

        window.popSalesMenuOrderTaker.LoadCombo = function () {
            //additional by janine
            setMonthCollection("selPOMonth");
            setYearCollection("selPOYear");
            setDayCollection('selPOMonth', 'selPODay', 'selPOYear', true);

            //setMonthCollection("selDeliveryMonth");
            //setYearCollection("selDeliveryYear");
            //setDayCollection('selDeliveryMonth', 'selDeliveryDay', 'selDeliveryYear', true);

            //$("#selDeliveryMonth").val(parseInt(new Date().getMonth()) + 1).selectmenu('refresh');
            //$("#selDeliveryYear").val(new Date().getFullYear()).selectmenu('refresh');
            //setDayCollection('selDeliveryMonth', 'selDeliveryDay', 'selDeliveryYear', true);
            //$("#selDeliveryDay").val(new Date().getDate()).selectmenu('refresh');

            //////////////////////
            var pDT;

            cbDeliveryAddress.empty();
            cbBillingAddress.empty();

            cbDeliveryAddress.html('');
            if (proj.SOMode)
                pDT = Business.LibraryOrganization.GetAddresses(sessionStorage.CustomerOrganizationID);
            else
                pDT = Business.LibraryOrganizationFOWST.GetEmpDeviceFOWST();
            Fill(cbDeliveryAddress);
            Fill(cbBillingAddress);


            function Fill(vCb) {
                var pDR;

                vCb.html('');
                for (var pIndex in pDT.Rows) {
                    pDR = pDT.Rows[pIndex];
                    vCb.append(new Option(pDR.OrgCompleteAddress, pDR.ID));
                }
                vCb.selectmenu('refresh', true);
            } // END Fill

        }; // END LoadCombo

        //additional by janine for disable fields sept 29 2014
        window.popSalesMenuOrderTaker.FillFowsts = function (vData) {

            //var vRows = vData.FowstData.Rows;
            var vRows = Business.LibraryOrganizationFOWST.GetData().Rows;

            cbFOWSTOrderTaker.empty();

            cbFOWSTOrderTaker.append(new Option('Please Select FOWST', ''));

            for (var p = 0; p <= vRows.length - 1; p++) {
                cbFOWSTOrderTaker.append(new Option(vRows[p].FowstName, vRows[p].FowstID));

            }


            $('#selFOWSTOrderTaker option[value="' + sessionStorage.FowstID + '"]').prop('selected', true);
            if (proj.SOMode) {
                cbDeliveryAddress.val(sessionStorage.DeliveryAddressID).selectmenu('refresh');
                cbBillingAddress.val(sessionStorage.BillingAddressID).selectmenu('refresh');
            } else {
                cbDeliveryAddress.val(sessionStorage.FowstID).selectmenu('refresh');
                cbBillingAddress.val(sessionStorage.FowstID).selectmenu('refresh');
            }
            cbFOWSTOrderTaker.selectmenu('refresh', true);


        };
        window.popSalesMenuOrderTaker.FillCities = function (vData) {

            var vRows = vData.CityData.Rows;

            selDeliveryCity.empty();

            selDeliveryCity.append(new Option('Please Select City', ''));

            for (var p = 0; p <= vRows.length - 1; p++) {
                selDeliveryCity.append(new Option(vRows[p].City, vRows[p].ID));

            }
            selDeliveryCity.selectmenu('refresh', true);


        };
        window.popSalesMenuOrderTaker.FillCountries = function (vData) {
            var vRows = vData.CountryData.Rows;

            selDeliveryCountry.empty();

            selDeliveryCountry.append(new Option('Please Select Country', ''));

            for (var p = 0; p <= vRows.length - 1; p++) {
                selDeliveryCountry.append(new Option(vRows[p].Country, vRows[p].ID));

            }

            $("#selDeliveryCountry option:contains('Philippines')").attr('selected', 'selected');

            selDeliveryCountry.selectmenu('refresh', true);

        };
        //end of additional
    }); // $(function() {]]
});                                                                        // END require
