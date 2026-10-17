require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryOrganizationRepresentative.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {

        $(function () {

            var IsDeliveryInfoProceedToPayments;
            var IsDeliveryInfoPriceQuote;
            var intDeliveryInfoEmailCntr = -1;
            var strDeliveryInfoEmail = "";

            $(document).ready(function () {

                IsDeliveryInfoProceedToPayments = false;
                IsDeliveryInfoPriceQuote = false;

                //                $("#divDeliveryInfoDelMode").hide();
                $("#btnDeliveryInfoProceedPayments").hide();
                $("#btnDeliveryInfoDone").hide();

                var pParams = GetUrlParameters();
                $("#divPickupLocation").show();
                $("#btnDeliveryInfoProceedPayments").show();

                //default Value for POS Pickup
                $('#selDeliveryMode').val('Pick Up').selectmenu('refresh');
                SetDeliveryMode();

                setMonthCollection("selPickUpMonth");
                setYearCollection("selPickUpYear");
                setHoursCollection("selPickUpHour");
                setMinutesCollection("selPickUpMin");

                ShowDeliveryInfo();

                window.pageSourcePage = $.mobile.activePage;

            });

            $('#btnOrderBy').click(function () {

                Browser.LibraryOrganizationRepresentative.InputData.OrganizationID = $("#txtOrgCustomerID").val();
                Browser.LibraryOrganizationRepresentative.Click = function (e) {

                    $('#txtOrderedByID').val(e.ID);
                    $('#btnOrderBy').text(e.RepresentativeName)
                        .button('refresh');

                    $('#txtDeliveryInfoTelNo').val(e.TelNo);
                    $('#txtDeliveryInfoFaxNo').val(e.FaxNo);
                    $('#txtDeliveryInfoMobileNo').val(e.MobileNo);
                    $('#txtDeliveryInfoEmail').val(e.Email);

                };
                Browser.LibraryOrganizationRepresentative.Show();

            });

            $('#btnBillTo').click(function () {

                Browser.LibraryFowst.InputData.OrganizationID = $("#txtOrgCustomerID").val();
                Browser.LibraryFowst.Click = function (e) {

                    $('#txtBillToID').val(e.ID)
                    $('#btnBillTo').text(e.Address)
                    .button('refresh');

                };
                Browser.LibraryFowst.Show();

            });

            $('#btnShipTo').click(function () {

                Browser.LibraryFowst.InputData.OrganizationID = $("#txtOrgCustomerID").val();
                Browser.LibraryFowst.Click = function (e) {

                    $('#txtShipToID').val(e.ID)
                    $('#btnShipTo').text(e.Address)
                    .button('refresh');

                };
                Browser.LibraryFowst.Show();

            });

            $('#btnPickLocation').click(function () {

                AjaxSend({
                    url: "DeliveryInfo/getCurrentSubcriberOrgID",
                    success: function (vData) {
                        if(vData.DeliveryInfoData != ""){
                            Browser.LibraryFowst.InputData.OrganizationID = vData.DeliveryInfoData;
                            Browser.LibraryFowst.Click = function (e) {

                                $('#txtPickUpLocationID').val(e.ID)
                                $('#btnPickLocation').text(e.BranchName)
                                .button('refresh');

                            };
                            Browser.LibraryFowst.Show();
                        }
                    }
                });

            });

            $('#selDeliveryMode').change(function (e) {
                e.preventDefault();

                SetDeliveryMode();

            });

            $('#rdoRush').click(function () {
                SetDeliveryDownTown($(this).is(":checked"));
            });

            $('#rdoNotRush').click(function () {
                SetDeliveryDownTown(!$(this).is(":checked"));

            });

            $("#txtDeliverWithIn").keydown(function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            });

            $('#rdoNormalRoute').click(function () {

                SetDeliveryOutOfTown($(this).is(":checked"));

            });

            $('#rdoBus').click(function () {

                SetDeliveryOutOfTown(!$(this).is(":checked"));

            });

            $('#rdoCustomerIsWaiting').click(function () {

                SetPickUp($(this).is(":checked"));

            });

            $('#rdoDateAndTime').click(function () {

                SetPickUp(!$(this).is(":checked"));

            });

            $('#selPickUpMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selPickUpMonth', 'selPickUpDay', 'selPickUpYear', true);

            });

            $('#selPickUpYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selPickUpMonth', 'selPickUpDay', 'selPickUpYear', true);

            });

            $('#btnDeliveryInfoDone').click(function (e) {
                e.preventDefault();

                var pParams = GetUrlParameters();

                if ($.trim($("#txtOrgCustomerID").val()) == "") {
                    MessageBox("Please select customer account.",
                            "Customer Account",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtOrderedByID").val()) == "") {
                    MessageBox("Please select customer representative.",
                                "Customer Representative",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }

                if ($.trim($("#txtBillToID").val()) == "") {
                    MessageBox("Please select billing address.",
                            "Billing Address",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtShipToID").val()) == "") {
                    MessageBox("Please select delivery address.",
                            "Delivery Address",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                //                if (pParams.SalesOrderID != undefined) {

                if ($.trim($("#selDeliveryMode").val()) == "") {
                    MessageBox("Please select delivery mode.",
                                "Delivery Mode",
                                [
                                    { Text: 'Ok' }
                                ]
                        );
                    return;
                }
                else {
                    if ($.trim($("#selDeliveryMode").val()) == "Deliver (Downtown)") {
                        if ($("#rdoRush").is(":checked")) {
                            if ($.trim($("#txtDeliverWithIn").val()) == "") {

                                MessageBox("Please enter minute(s) or hour(s) value.",
                                        "Hours or Minutes",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );

                                return;
                            }

                            if (!$.isNumeric($("#txtDeliverWithIn").val())) {
                                MessageBox("Please enter valid minute(s) or hour(s) value.",
                                           "Hours or Minutes",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );
                                return;
                            }

                            if ($.trim($("#selHourMinute").val()) == "") {

                                MessageBox("Please select if value is in minutes or hours.",
                                        "Hours or Minutes",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );

                                return;
                            }

                        }
                    }

                    if ($.trim($("#selDeliveryMode").val()) == "Pick Up") {
                        if ($("#rdoDateAndTime").is(":checked")) {

                            if ($.trim($("#selPickUpMonth").val()) == "" ||
                                    $.trim($("#selPickUpDay").val()) == "" ||
                                    $.trim($("#selPickUpYear").val()) == "" ||
                                    $.trim($("#selPickUpHour").val()) == "" ||
                                    $.trim($("#selPickUpMin").val()) == "" ||
                                    $.trim($("#selPickUpAMPM").val()) == "") {

                                MessageBox("Please enter valid pick up date and time.",
                                                "Pick Up Date and Time",
                                                [
                                                    { Text: 'Ok' }
                                                ]
                                    );
                                return;
                            }

                        }
                    }

                }

                //                };

                if ($("#selHoldOrder").val() == '1') {
                    MessageBox('This sales order will be held. Held sales orders will not be processed. Do still want to save this transaction?', 'Hold',
                        [{ Text: 'Yes. Save and hold',
                            Click: function () {
                                SaveDeliveryInfo();
                            }
                        }, { Text: 'No'
                        }]); // MessageBox
                } else
                    SaveDeliveryInfo();

            });

            $('#btnDeliveryInfoBack').click(function (e) {
                e.preventDefault();

                history.back();

            });

            $('#btnDeliveryInfoProceedPayments').click(function (e) {
                e.preventDefault();

                IsDeliveryInfoProceedToPayments = true;
                IsDeliveryInfoPriceQuote = false;
                $('#btnDeliveryInfoDone').click();

            });

            $('#btnDeliveryInfoPriceQoute').click(function (e) {
                e.preventDefault();

                IsDeliveryInfoProceedToPayments = false;
                IsDeliveryInfoPriceQuote = true;
                $('#btnDeliveryInfoDone').click();

            });

            $('#btnDeliveryInfoEmailOK').click(function (e) {
                e.preventDefault();

                strDeliveryInfoEmail = "";
                for (var i = 0; i <= intDeliveryInfoEmailCntr; i++) {

                    if ($("#chkDeliveryInfoEmail-" + i).is(":checked")) {
                        var pEmailData = $("#chkDeliveryInfoEmail-" + i).data('Data');

                        strDeliveryInfoEmail = strDeliveryInfoEmail + (strDeliveryInfoEmail.length > 0 ? ", " : "") + pEmailData.EmailAddress
                    }

                }

                if ($.trim($("#txtDeliveryInfoEmailAddress").val()) == "" && strDeliveryInfoEmail == "") {
                    MessageBox("Please enter email address.",
                           "Email Address",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                } else {
                    if ($.trim($("#txtDeliveryInfoEmailAddress").val()).length > 0) {
                        strDeliveryInfoEmail = strDeliveryInfoEmail + (strDeliveryInfoEmail.length > 0 ? ", " : "") + $.trim($("#txtDeliveryInfoEmailAddress").val())
                    }
                }

                $('#btnDeliveryInfoPriceQoute').click();

            });

            $('#btnDeliveryInfoEmailCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageDeliveryInfo');

            });

            function SetDeliveryMode() {
                var pParams = GetUrlParameters();

                $('#divDeliverDownTown').hide();
                $('#divDeliverOutOfTown').hide();
                $('#divPickup').hide();

//                if (pParams.SalesOrderID != undefined) {
                if ($('#selDeliveryMode').val() == "Deliver (Downtown)") {
                    $('#divDeliverDownTown').show();
                }
                else if ($('#selDeliveryMode').val() == "Deliver (Out Of Town)") {
                    $('#divDeliverOutOfTown').show();
                }
                else if ($('#selDeliveryMode').val() == "Pick Up") {
                    $('#divPickup').show();

                    $('#divPickUpType').hide();
                        
                    $("#rdoCustomerIsWaiting").attr("checked", false).checkboxradio("refresh");
                    $("#rdoDateAndTime").attr("checked", true).checkboxradio("refresh");
                    SetPickUp(false);
                }
//                }

            }

            function SetDeliveryDownTown(IsRush) {

                $('#divRush').hide();
                $('#divNotRush').hide();
                if (IsRush) {
                    $('#divRush').show();
                }
                else {
                    $('#txtDeliverWithIn').val('0');
                    $('#selHourMinute').val('').selectmenu('refresh');
                    $('#divNotRush').show();
                }

            }

            function SetDeliveryOutOfTown(IsNormalRoute) {

                $('#divBus').hide();
                if (!IsNormalRoute) {
                    $('#divBus').show();
                }

            }

            function SetPickUp(IsCustomerWaiting) {

                $('#divCustomerIsWaiting').hide();
                $('#divDateAndTime').hide();
                if (IsCustomerWaiting) {
                    $('#selPickUpMonth').val('').selectmenu('refresh');
                    $('#selPickUpDay').val('').selectmenu('refresh');
                    $('#selPickUpYear').val('').selectmenu('refresh');
                    $('#selPickUpHour').val('').selectmenu('refresh');
                    $('#selPickUpMin').val('').selectmenu('refresh');
                    $('#selPickUpAMPM').val('').selectmenu('refresh');

                    $('#divCustomerIsWaiting').show();
                }
                else {
                    $('#divDateAndTime').show();
                }
            }

            function ShowDeliveryInfo() {
                var pData = {FowstID: sessionStorage.FowstID};
                var pParams = GetUrlParameters();

                pData.InvoiceID = pParams.InvoiceID;

                ClearDeliveryInfoFields();

                AjaxSend({
                    url: "DeliveryInfo/getDeliveryInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.Error) {
                            MessageBox(vData.Error);
                            return;
                        };
                        if (vData.DeliveryInfoData.Rows.length > 0) {

                            $("#txtOrgCustomerID").val(vData.DeliveryInfoData.Rows[0].CustomerID || '');
                            $("#txtOrgCustomer").val(vData.DeliveryInfoData.Rows[0].CustomerName || '');

                            $("#txtOrderedByID").val(vData.DeliveryInfoData.Rows[0].RepresentativeID || '');
                            $("#btnOrderBy").text(vData.DeliveryInfoData.Rows[0].Representative || 'Please Select').button('refresh');

                            $("#txtBillToID").val(vData.DeliveryInfoData.Rows[0].BillingAddressID || '');
                            $("#btnBillTo").text(vData.DeliveryInfoData.Rows[0].BillingAddress || 'Please Select').button('refresh');

                            $("#txtShipToID").val(vData.DeliveryInfoData.Rows[0].DeliveryAddressID || '');
                            $("#btnShipTo").text(vData.DeliveryInfoData.Rows[0].DeliveryAddress || 'Please Select').button('refresh');

                            $("#txtDeliveryInfoTelNo").val(vData.DeliveryInfoData.Rows[0].TelNo || '');
                            $("#txtDeliveryInfoFaxNo").val(vData.DeliveryInfoData.Rows[0].FaxNo || '');
                            $("#txtDeliveryInfoMobileNo").val(vData.DeliveryInfoData.Rows[0].MobileNo || '');
                            $("#txtDeliveryInfoEmail").val(vData.DeliveryInfoData.Rows[0].Email || '');

                            $("#txtPickUpLocationID").val(vData.DeliveryInfoData.Rows[0].PickUpLocationID || '');
                            $("#btnPickLocation").text(vData.DeliveryInfoData.Rows[0].PickUpLocation || 'Please Select').button('refresh');

                            //Set Delivery Method
                            if (vData.DeliveryInfoData.Rows[0].DeliveryMethod == 'Deliver') {
                                if (vData.DeliveryInfoData.Rows[0].IsDownTown == 1) {
                                    $("#selDeliveryMode").val('Deliver (Downtown)').selectmenu('refresh');
                                }
                                else {
                                    $("#selDeliveryMode").val('Deliver (Out Of Town)').selectmenu('refresh');
                                }
                            }
                            else {
                                $("#selDeliveryMode").val('Pick Up').selectmenu('refresh');
                            }
                            SetDeliveryMode();

                            //Set if Rush or Not
                            if (vData.DeliveryInfoData.Rows[0].IsRushDelivery == 1) {
                                $("#rdoRush").attr("checked", true).checkboxradio("refresh");
                                $("#rdoNotRush").attr("checked", false).checkboxradio("refresh");
                                $("#txtDeliverWithIn").val(vData.DeliveryInfoData.Rows[0].DeliverWithin || '');
                                $("#selHourMinute").val('Working Minutes').selectmenu('refresh');
                                SetDeliveryDownTown(true);
                            }
                            else {
                                $("#rdoRush").attr("checked", false).checkboxradio("refresh");
                                $("#rdoNotRush").attr("checked", true).checkboxradio("refresh");
                                SetDeliveryDownTown(false);
                            }

                            //Set Normal Route, Bus or Customer id Waiting
                            if (vData.DeliveryInfoData.Rows[0].DeliveryType == 'Normal Route Delivery') {
                                $("#rdoNormalRoute").attr("checked", true).checkboxradio("refresh");
                                $("#rdoBus").attr("checked", false).checkboxradio("refresh");
                                SetDeliveryOutOfTown(true);
                            }
                            else if (vData.DeliveryInfoData.Rows[0].DeliveryType == 'Bus') {
                                $("#rdoNormalRoute").attr("checked", false).checkboxradio("refresh");
                                $("#rdoBus").attr("checked", true).checkboxradio("refresh");
                                SetDeliveryOutOfTown(false);
                            }
                            else if (vData.DeliveryInfoData.Rows[0].DeliveryType == 'Customer Is Waiting') {
                                $("#rdoCustomerIsWaiting").attr("checked", true).checkboxradio("refresh");
                                $("#rdoDateAndTime").attr("checked", false).checkboxradio("refresh");
                                SetPickUp(true);
                            }
                            else {
                                if (vData.DeliveryInfoData.Rows[0].DeliveryMethod.toLowerCase() == 'pickup') {
                                    $("#rdoCustomerIsWaiting").attr("checked", false).checkboxradio("refresh");
                                    $("#rdoDateAndTime").attr("checked", true).checkboxradio("refresh");

                                    $("#selPickUpMonth").val(vData.DeliveryInfoData.Rows[0].PickupDateMonth || '').selectmenu('refresh');
                                    $("#selPickUpYear").val(vData.DeliveryInfoData.Rows[0].PickupDateYear || '').selectmenu('refresh');
                                    setDayCollection('selPickUpMonth', 'selPickUpDay', 'selPickUpYear', true);
                                    $("#selPickUpDay").val(vData.DeliveryInfoData.Rows[0].PickupDateDay || '').selectmenu('refresh');

                                    $("#selPickUpHour").val(vData.DeliveryInfoData.Rows[0].PickupDateHour || '').selectmenu('refresh');
                                    $("#selPickUpMin").val(vData.DeliveryInfoData.Rows[0].PickupDateMinute || '').selectmenu('refresh');
                                    $("#selPickUpAMPM").val(vData.DeliveryInfoData.Rows[0].PickupDateAMPM || '').selectmenu('refresh');

                                    SetPickUp(false);
                                }
                            }

                            // always not on-hold
                            // $("#selHoldOrder").val(vData.DeliveryInfoData.Rows[0].IsHold || '0').slider("refresh");

                            //Get OutOfTownBus Default
                            AjaxSend({
                                url: "DeliveryInfo/getOutOfTownBusDefault",
                                data: pData,
                                success: function (vData) {

                                    $("#lblOutOfTownBus").text('Maximum service time : ' + vData.OutOfTownBusDefault + ' hour(s)' || 'Maximum service time : 2 hour(s)');

                                }
                            });

                            //Get Pick Up Customer Is Waiting Default
                            AjaxSend({
                                url: "DeliveryInfo/getPickUpCustomerIsWaitingDefault",
                                data: pData,
                                success: function (vData) {

                                    $("#lblPickUpCustomerIsWaiting").text('Maximum service time : ' + vData.PickUpCustomerIsWaitingDefault + ' Minute(s)' || 'Maximum service time : 30 Minute(s)');

                                }
                            });

                            AjaxSend({
                                url: "Browser/LibraryOrganizationRepresentative",
                                data: {Top1: 1},
                                success: function (vData) {
                                    var pDR = vData.Results.Rows[0];
                                    if (pDR == undefined) return;
                                    $('#txtOrderedByID').val(pDR.ID);
                                    $('#btnOrderBy').text(pDR.RepresentativeName)
                                        .button('refresh');

                                    $('#txtDeliveryInfoTelNo').val(pDR.TelNo);
                                    $('#txtDeliveryInfoFaxNo').val(pDR.FaxNo);
                                    $('#txtDeliveryInfoMobileNo').val(pDR.MobileNo);
                                    $('#txtDeliveryInfoEmail').val(pDR.Email);                                    
                                }
                            });

                        }
                    }
                });

            };

            function ClearDeliveryInfoFields() {

                $("#txtOrgCustomerID").val('');
                $("#txtOrgCustomer").val('');

                $("#txtOrderedByID").val('');
                $("#btnOrderBy").text('Please Select').button('refresh');

                $("#txtBillToID").val('');
                $("#btnBillTo").text('Please Select').button('refresh');

                $("#txtShipToID").val('');
                $("#btnShipTo").text('Please Select').button('refresh');

                $("#txtDeliveryInfoTelNo").val('');
                $("#txtDeliveryInfoFaxNo").val('');
                $("#txtDeliveryInfoMobileNo").val('');
                $("#txtDeliveryInfoEmail").val('');

                $("#txtPickUpLocationID").val('');
                $("#btnPickLocation").text('Please Select').button('refresh');

                $("#selDeliveryMode").val('');
                SetDeliveryMode();

                $("#txtDeliverWithIn").val('');
                $("#selHourMinute").val('').selectmenu('refresh');
                $("#rdoNotRush").attr("checked", true).checkboxradio("refresh");
                SetDeliveryDownTown(false);

                $("#rdoNormalRoute").attr("checked", true).checkboxradio("refresh");
                SetDeliveryOutOfTown(true);

                $("#rdoCustomerIsWaiting").attr("checked", true).checkboxradio("refresh");
                SetPickUp(true);

                $("#selPickUpMonth").val('').selectmenu('refresh');
                $("#selPickUpDay").val('').selectmenu('refresh');
                $("#selPickUpYear").val('').selectmenu('refresh');
                $("#selPickUpHour").val('').selectmenu('refresh');
                $("#selPickUpMin").val('').selectmenu('refresh');
                $("#selPickUpAMPM").val('').selectmenu('refresh');

                $("#selHoldOrder").val('0').slider("refresh");

            }

            function SaveDeliveryInfo() {
                var pData = [];
                var pDataEmail = [];
                var pPODate = "";
                var pDeliveryType = "";
                var pDeliverWithin = 0;
                var pPickupDate = "";
                var pParams = GetUrlParameters();

                pDeliverWithin = $("#txtDeliverWithIn").val();
                if ($.trim($("#selHourMinute").val()) == "Working Hours") {
                    pDeliverWithin = $("#txtDeliverWithIn").val() * 60;
                }

                if ($.trim($("#selDeliveryMode").val()) == "Deliver (Out Of Town)") {
                    if ($("#rdoNormalRoute").is(":checked")) {
                        pDeliveryType = "Normal Route Delivery"
                    }
                    else if ($("#rdoBus").is(":checked")) {
                        pDeliveryType = "Bus"
                    }
                }
                else if ($.trim($("#selDeliveryMode").val()) == "Pick Up") {
                    if ($("#rdoCustomerIsWaiting").is(":checked")) {
                        pDeliveryType = "Customer Is Waiting"
                    }
                }

                if ($.trim($("#selPickUpMonth").val()) != "" &&
                    $.trim($("#selPickUpDay").val()) != "" &&
                    $.trim($("#selPickUpYear").val()) != "" &&
                    $.trim($("#selPickUpHour").val()) != "" &&
                    $.trim($("#selPickUpMin").val()) != "" &&
                    $.trim($("#selPickUpAMPM").val()) != "") {

                    pPickupDate = $("#selPickUpMonth").val() + '/' + $("#selPickUpDay").val() + '/' + $("#selPickUpYear").val() + " " + $("#selPickUpHour").val() + ":" + $("#selPickUpMin").val() + ":00 " + $("#selPickUpAMPM").val();
                }

                if (IsDeliveryInfoPriceQuote) {
                    if (strDeliveryInfoEmail == "") {

                        pDataEmail = {
                            CustomerID: $.trim($("#txtOrgCustomerID").val())
                        };

                        AjaxSend({
                            url: "SalesPayments/getCustomerEmails",
                            data: pDataEmail,
                            success: function (vDataEmail) {

                                LoadDeliveryInfoCustomerEmailList(vDataEmail.SalesPaymentData.Rows);
                            }
                        });

                        return;
                    }
                }

                pData = {
                    ID: pParams.InvoiceID,
                    CustomerID: $.trim($("#txtOrgCustomerID").val()),
                    CusRepresentativeID: $.trim($("#txtOrderedByID").val()),
                    BillToID: $.trim($("#txtBillToID").val()),
                    ShipToID: $.trim($("#txtShipToID").val()),
                    EmailAddress: $.trim(strDeliveryInfoEmail),
                    PickUpLocationID: $.trim($("#txtPickUpLocationID").val()),
                    DeliveryMethod: ($.trim($("#selDeliveryMode").val()) == "Deliver (Downtown)" || $.trim($("#selDeliveryMode").val()) == "Deliver (Out Of Town)" ? "Deliver" : "Pickup"),
                    IsDownTown: ($.trim($("#selDeliveryMode").val()) == "Deliver (Downtown)" ? "1" : "0"),
                    IsRushDelivery: ($("#rdoRush").is(":checked") ? "1" : "0"),
                    DeliverWithin: pDeliverWithin,
                    DeliveryType: pDeliveryType,
                    PickupDate: pPickupDate,
                };

                AjaxSend({
                    url: "DeliveryInfo/SaveInvoiceDeliveryInfo",
                    data: pData,
                    success: function (vSaveData) {

                        if (vSaveData.DeliveryInfoData == "InvalidEmail") {
                            MessageBox("Please enter valid email address.",
                                    "Invalid email",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                            return;
                        }
                        else if (vSaveData.DeliveryInfoData == "MinutesOrHoursError") {
                            MessageBox("Please enter valid minutes or hours value.",
                                    "Minutes or Hours",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                    );
                            return;
                        }
                        else if (vSaveData.DeliveryInfoData == "PickUpDateAndTimeError") {
                            MessageBox("Please select valid pick up date and time.",
                                    "Pick Up Date and Time",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                    );
                            return;
                        }

                        localStorage.removeItem('CustomerOrganizationID');

                        if (IsDeliveryInfoProceedToPayments) {
                            window.location = GetPathFormat("SalesPayments") + "?InvoiceID=" + pParams.InvoiceID;
                        }
                        else if (IsDeliveryInfoPriceQuote) {

                            AjaxSend({
                                url: "DeliveryInfo/SaveInvoiceToQuotation",
                                data: pData,
                                success: function (vSaveQuoteData) {

                                    var pEmailQuoteData = [];
                                    pEmailQuoteData = {
                                        QuotationID: vSaveQuoteData.DeliveryInfoData,
                                        EmailAddress: $.trim(strDeliveryInfoEmail)
                                    };

                                    AjaxSend({
                                        url: "DeliveryInfo/SendEmailQuotation",
                                        data: pEmailQuoteData,
                                        success: function (vSendQuoteData) {
                                            MessageBox("Price quote has been sent.",
                                                    "Email Price Quote",
                                                    [
                                                        { Text: 'Ok',
                                                            Click: function () {
                                                                window.location = GetPathFormat("/SalesMenu?mode=pos");
                                                            }
                                                        }
                                                    ]
                                            );

                                        }
                                    });
                                }
                            });

                        }
                        else {
                            window.location = GetPathFormat("SalesMenu");
                        }
                    }
                });


            };

            function LoadDeliveryInfoCustomerEmailList(vRows) {
                var pLV = $('#fldsetDeliveryInfoEmail');
                var pItem;

                intDeliveryInfoEmailCntr = vRows.length - 1;

                pLV.html('');
                for (var i = 0; i <= vRows.length - 1; i++) {

                    pItem = $('<input>')
                                .attr("type", 'checkbox')
                                .attr('id', 'chkDeliveryInfoEmail-' + i)
                                .attr('checked', false)
                                .data('Data', vRows[i])
                            .append($('<label>')
                                .attr('for', 'chkDeliveryInfoEmail-' + i)
                                .text((vRows[i].EmailAddress || 'No Email Address'))
                            );

                    pLV.append(pItem);
                }

                $('#divDeliveryInfoEmail').trigger("create");

                if (vRows.length <= 0) {
                    $("#divDeliveryInfoEmailNone").show();
                } else {
                    $("#divDeliveryInfoEmailNone").hide();
                }

                $.mobile.changePage("#divDeliveryInfoPromptEmail");
                return;

            };


        });
    });