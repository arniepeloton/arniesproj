$(function () {

    var divLibCurrencies = $('#divLibCurrencies');
    var divTable = divLibCurrencies.find('[name=divTable]');
    var divResult = divLibCurrencies.find('[name=divResult]');
    var butRefresh = divLibCurrencies.find('[name=butRefresh]')
        .click(butRefresh_Click);
    var divUploader = divLibCurrencies.find('[name=divUploader]')
        .pureUploader({
            url: '/LibCurrencies/Upload',
            multiple: false,
            onLoad: FileUploaded
        });

    butRefresh_Click();

    function FileUploaded(vArgs) {

        try {
            vArgs.Data = JSON.parse(vArgs.Data);
        } catch (e) {

        };
        if (HandleSessionExpired(vArgs.Data) == false) return;
        vArgs = vArgs.Data.d;
        if (vArgs.Added != null) {
            LoadGrid(vArgs.Data);
        } else if (vArgs.Error != null) {
            divResult.text(vArgs.Error).MakeError();
        };

    }; // END FileUploaded

    function butRefresh_Click() {

        divLibCurrencies.block();
        AjaxSend({
            url: '/LibCurrencies/Search',
            success: function (vData) {
                divLibCurrencies.unblock();
                if (HandleSessionExpired(vData) == false) return;
                LoadGrid(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divLibCurrencies.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END butRefresh_Click

    function LoadGrid(vData) {

        vData.Columns = {
            Entity: {
                Label: 'Country/Territory/Entity'
            },
            Currency: {},
            AlphaCode: {
                Label: 'Alphabetic Code'
            }
        };
        divTable.ProWebGrid(vData);

    }; // END LoadGrid
});