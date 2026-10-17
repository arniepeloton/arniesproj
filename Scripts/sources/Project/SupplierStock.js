$(function () {

    var divSupplierStock = $('#divSupplierStock');
    var divResult = divSupplierStock.find('[name=divResult]');
    var spanSupplier = divSupplierStock.find('[name=spanSupplier]');
    var spanStockNumber = divSupplierStock.find('[name=spanStockNumber]');
    var spanStockDescription = divSupplierStock.find('[name=spanStockDescription]');
    var txtLeadTime = divSupplierStock.find('[name=txtLeadTime]');
    var txtLotSize = divSupplierStock.find('[name=txtLotSize]');
    var cbLotSize = divSupplierStock.find('[name=cbLotSize]');

    var txtMoq = divSupplierStock.find('[name=txtMoq]');
    var cbMoq = divSupplierStock.find('[name=cbMoq]');

    var butSave = divSupplierStock.find('[name=butSave]')
        .click(butSave_Click);
    var mValidator;
    var mSupplierID, mStockID;

    (function () {

        var pParsedUrl;

        pParsedUrl = parseUri(location);
        mSupplierID = pParsedUrl.queryKey.supplierid;
        mStockID = pParsedUrl.queryKey.stockid;

        InitValidation();
        GetData();

        function InitValidation() {

            mValidator = new Validator;
            mValidator.SummaryDiv = divResult;

            mValidator.AddItem({
                Label: 'Lead time',
                Control: txtLeadTime,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Lot size quantity',
                Control: txtLotSize,
                ErrorControl: null
            }).AddNumericValidation(0)
            .AddRequiredValidation();

            mValidator.AddItem({
                Label: 'Lot size unit',
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

    })();

    function GetData() {

        AjaxSend({
            url: '/SupplierStock/GetData?SupplierID=' + encodeURIComponent(mSupplierID) + '&StockID=' + encodeURIComponent(mStockID),
            success: function (vData) {
                divSupplierStock.unblock();
                if (HandleSessionExpired(vData) == false) return;
                if (vData.d.Error)
                    divResult.text(vData.d.Error).MakeError();
                else
                    LoadData(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divSupplierStock.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END GetData

    function LoadData(vData) {

        var pText, pVal;

        spanSupplier.text(vData.SupplierName);
        spanStockNumber.text(vData.StockNumber);
        spanStockDescription.text(vData.StockDescription);
        txtLeadTime.val(vData.LeadTime);
        txtLotSize.val(vData.LotSize);
        txtMoq.val(vData.MinimumOrderQty);
        cbLotSize.empty();
        cbMoq.empty();
        for (var p = 0; p < vData.Units.Rows.length; p++) {
            pVal = vData.Units.Rows[p].UnitMeasure;
            pText = pVal + ' [' + vData.Units.Rows[p].UnitQty + ']';
            cbLotSize.append($('<option>').text(pText).val(pVal));
            cbMoq.append($('<option>').text(pText).val(pVal));
        };
        cbLotSize.val(vData.LotSizeUnit);
        cbMoq.val(vData.MinimumOrderUnit);
        $.uniform.update();

    }; // END LoadData

    function butSave_Click() {

        if (mValidator.Validate() == false) return;

        divSupplierStock.block();
        divResult.hide();
        AjaxSend({
            url: '/SupplierStock/Save',
            data: {
                SupplierID: mSupplierID,
                StockID: mStockID,
                txtLeadTime: txtLeadTime.val(),
                txtLotSize: txtLotSize.val(),
                cbLotSize: cbLotSize.val(),
                txtMoq: txtMoq.val(),
                cbMoq: cbMoq.val()
            },
            success: function (vData) {
                divSupplierStock.unblock();
                if (HandleSessionExpired(vData) == false) return;
                if (vData.d.Error)
                    divResult.text(vData.d.Error).MakeError();
                else
                    divResult.text('Success').MakeHighlight();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divSupplierStock.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END butSave_Click
});