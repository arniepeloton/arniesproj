$(function () {

    setLevel0("Employees", "../welcome", 1);
    setLevel1("Drp", "../StockSched/", 1);
    setLevel2("DRP Parameters", "../StockSched/", 1);

    var divDrpParameters = $('#divDrpParameters');
    var divCurrency = divDrpParameters.find('[name=divCurrency]');
    var butCurrency = divDrpParameters.find('[name=butCurrency]')
        .click(butCurrency_Click);
    var butSave = divDrpParameters.find('[name=butSave]')
        .click(butSave_Click);
    var divResult = divDrpParameters.find('[name=divResult]');

    var txtPoProcessingLeadTime = divDrpParameters.find('[name=txtPoProcessingLeadTime]'); // PO Processing Lead Time in Days
    var txtVendorLeadTime = divDrpParameters.find('[name=txtVendorLeadTime]'); // Vendor Lead Time in Days
    var txtShippingLeadTime = divDrpParameters.find('[name=txtShippingLeadTime]'); // Add Shipping Lead Time in Days
    var spanTotalLeadTime = divDrpParameters.find('[name=spanTotalLeadTime]');

    var txtFirmOrders = divDrpParameters.find('[name=txtFirmOrders]'); // Make firm orders for planned orders in X weeks
    var txtFrequency = divDrpParameters.find('[name=txtFrequency]'); // Frequency of recomputation of DRP tables (in days)
    var txtForecastHorizon = divDrpParameters.find('[name=txtForecastHorizon]');
    var txtSafetyStockDuration = divDrpParameters.find('[name=txtSafetyStockDuration]');

    var txtLotSize = divDrpParameters.find('[name=txtLotSize]');
    var cbLotSize = divDrpParameters.find('[name=cbLotSize]');

    var txtMoq = divDrpParameters.find('[name=txtMoq]');
    var cbMoq = divDrpParameters.find('[name=cbMoq]');

    var mFindCurrency;
    var mCurrency; // value (AlphaCode)
    var mValidator;

    (function () {

        txtPoProcessingLeadTime
            .add(txtVendorLeadTime)
            .add(txtShippingLeadTime)
            .bind('textchange', DisplayTotalLeadTime);
        InitValidation();
        GetData();

        function InitValidation() {

            mValidator = new Validator;
            mValidator.SummaryDiv = divResult;

            mValidator.AddItem({
                Label: 'PO Processing Lead Time in Days',
                Control: txtPoProcessingLeadTime,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Vendor Lead Time in Days',
                Control: txtVendorLeadTime,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Add Shipping Lead Time in Days',
                Control: txtShippingLeadTime,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Make firm orders for planned orders in X weeks',
                Control: txtFirmOrders,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Frequency of recomputation of DRP tables (in days)',
                Control: txtFrequency,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Forecast horizon',
                Control: txtForecastHorizon,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Safety stock duration',
                Control: txtSafetyStockDuration,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Default lot size quantity',
                Control: txtLotSize,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Default lot size unit',
                Control: cbLotSize,
                ErrorControl: null
            }).AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Minimum order quantity',
                Control: txtMoq,
                ErrorControl: null
            }).AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Minimum order unit',
                Control: cbMoq,
                ErrorControl: null
            }).AddRequiredValidation();

        }; // END InitValidation
    })(); // END ()

    function GetData() {

        AjaxSend({
            url: '/drp/ParametersGet',
            success: function (vData) {
                divDrpParameters.unblock();
                if (HandleSessionExpired(vData) == false) return;
                if (vData.d.Error)
                    divResult.text(vData.d.Error).MakeError();
                else
                    LoadData(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divDrpParameters.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END GetData

    function LoadData(vData) {

        txtPoProcessingLeadTime.val(vData.txtPoProcessingLeadTime);
        txtVendorLeadTime.val(vData.txtVendorLeadTime);
        txtShippingLeadTime.val(vData.txtShippingLeadTime);
        if (vData.Currency != null)
            LoadCurrency(vData.Currency);
        else
            divCurrency.html('');
        txtFirmOrders.val(vData.MakeFirmOrders);
        txtFrequency.val(vData.txtFrequency);
        txtForecastHorizon.val(vData.txtForecastHorizon);
        txtSafetyStockDuration.val(vData.txtSafetyStockDuration);
        txtLotSize.val(vData.txtLotSize);
        cbLotSize.val(vData.cbLotSize);
        txtMoq.val(vData.txtMoq);
        cbMoq.val(vData.cbMoq);
        DisplayTotalLeadTime();

        $.uniform.update();

    }; // END LoadData

    function butCurrency_Click() {

        if (mFindCurrency == null)
            mFindCurrency = new Finder({
                Title: 'Currencies',
                Url: '/LibCurrencies/Search',
                Columns: {
                    Entity: {
                        Label: 'Country/Territory/Entity'
                    },
                    Currency: {},
                    AlphaCode: {
                        Label: 'Alphabetic Code'
                    }
                }, // END Columns
                OnOk: function (vData) {
                    mCurrency = vData.AlphaCode;
                    LoadCurrency(vData);
                }
            });
        mFindCurrency.ShowDialog();

    }; // END butCurrency_Click

    function butSave_Click() {

        if (mValidator.Validate() == false) return;

        divDrpParameters.block();
        divResult.hide();
        AjaxSend({
            url: '/drp/ParametersSave',
            data: {
                txtPoProcessingLeadTime: txtPoProcessingLeadTime.val(),
                txtVendorLeadTime: txtVendorLeadTime.val(),
                txtShippingLeadTime: txtShippingLeadTime.val(),
                Currency: mCurrency,
                MakeFirmOrders: txtFirmOrders.val(),
                txtFrequency: txtFrequency.val(),
                txtForecastHorizon: txtForecastHorizon.val(),
                txtSafetyStockDuration: txtSafetyStockDuration.val(),
                txtLotSize: txtLotSize.val(),
                cbLotSize: cbLotSize.val(),
                txtMoq: txtMoq.val(),
                cbMoq: cbMoq.val()
            },
            success: function (vData) {
                divDrpParameters.unblock();
                if (HandleSessionExpired(vData) == false) return;
                divResult.text('Success').MakeHighlight();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divDrpParameters.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END butSave_Click

    function LoadCurrency(vData) {

        divCurrency.html(
            '<table>' +
                '<tr>' +
                    '<td>' +
                        'Country/Territory/Entity:' +
                    '</td>' +
                    '<td><b>' +
                        HtmlEncode(vData.Entity) +
                    '</b></td>' +
                '</tr>' +
                '<tr>' +
                    '<td>' +
                        'Currency:' +
                    '</td>' +
                    '<td><b>' +
                        HtmlEncode(vData.Currency) +
                    '</b></td>' +
                '<tr>' +
                    '<td>' +
                        'Alphabetic Code:' +
                    '</td>' +
                    '<td><b>' +
                        HtmlEncode(vData.AlphaCode) +
                    '</b></td>' +
                '</tr>' +
            '</table>'
        );

    }; // LoadCurrency

    function DisplayTotalLeadTime() {

        spanTotalLeadTime.text(
            CDec2(txtPoProcessingLeadTime.val()) +
            CDec2(txtVendorLeadTime.val()) +
            CDec2(txtShippingLeadTime.val())
        );

    }; // END DisplayTotalLeadTime
});