$(function () {

    var divSupplier = $('#divSupplier');
    var divResult = divSupplier.find('[name=divResult]');
    var mFindSupplier;
    var spanSupplier = divSupplier.find('[name=spanSupplier]');
    var butBrowseSupplier = $('[name=butBrowseSupplier]')
        .click(butBrowseSupplier_Click);
    var butSave = divSupplier.find('[name=butSave]')
        .click(butSave_Click);
    var txtLeadTime = divSupplier.find('[name=txtLeadTime]');
    var txtLotSize = divSupplier.find('[name=txtLotSize]');
    var cbLotSize = divSupplier.find('[name=cbLotSize]');

    var txtMoq = divSupplier.find('[name=txtMoq]');
    var cbMoq = divSupplier.find('[name=cbMoq]');
    var mSupplierID;
    var mValidator;

    (function () {

        var pParsedUrl;

        pParsedUrl = parseUri(location);
        mSupplierID = pParsedUrl.queryKey.supplierid;

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

    })();

    function butBrowseSupplier_Click(e) {

        if (mFindSupplier == null)
            mFindSupplier = new Finder({
                Title: 'Suppliers',
                Url: '/Suppliers/Search',
                Columns: {
                    'SupplierName': {
                        'Label': 'Supplier Name'
                    },
                    'EmailAddress': null,
                    'IsActive': {
                        'Label': 'Active',
                        'MajorDataType': 'Numeric'
                    },
                    'LastUpdated': {
                        'Label': 'Last Updated',
                        'MajorDataType': 'Date',
                        'Format': 'MMMM d, yyyy h:mm a'
                    }
                }, // END Columns
                OnOk: function (vData) {
                    spanSupplier.text(vData.SupplierName);
                    mSupplierID = vData.ID;
                    GetData();
                }
            });
        mFindSupplier.ShowDialog();
    }; // END butBrowseSupplier_Click

    function GetData() {

        AjaxSend({
            url: '/Supplier/GetData',
            data: { SupplierID: mSupplierID },
            success: function (vData) {
                divSupplier.unblock();
                if (HandleSessionExpired(vData) == false) return;
                if (vData.d.Error)
                    divResult.text(vData.d.Error).MakeError();
                else
                    LoadData(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divSupplier.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END GetData

    function LoadData(vData) {

        spanSupplier.text(vData.SupplierName);
        mSupplierID = vData.SupplierID;
        txtLeadTime.val(vData.LeadTime);
        txtLotSize.val(vData.LotSize);
        cbLotSize.val(vData.LotSizeUnit);
        txtMoq.val(vData.MinimumOrderQty);
        cbMoq.val(vData.MinimumOrderUnit);

        $.uniform.update();

    }; // END LoadData

    function butSave_Click() {

        if (mValidator.Validate() == false) return;

        divSupplier.block();
        divResult.hide();
        AjaxSend({
            url: '/Supplier/Save',
            data: {
                SupplierID: mSupplierID,
                txtLeadTime: txtLeadTime.val(),
                txtLotSize: txtLotSize.val(),
                cbLotSize: cbLotSize.val(),
                txtMoq: txtMoq.val(),
                cbMoq: cbMoq.val()
            },
            success: function (vData) {
                divSupplier.unblock();
                if (HandleSessionExpired(vData) == false) return;
                if (vData.d.Error)
                    divResult.text(vData.d.Error).MakeError();
                else
                    divResult.text('Success').MakeHighlight();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divSupplier.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END butSave_Click

});