$(function () {

    var divMain = $('#divMain');
    var divResult = divMain.find('[name=divResult]');
    var butRefresh = divMain.find('[name=butRefresh]')
        .click(butRefresh_Click);
    var divPrices = divMain.find('[name=divPrices]');
    var mGridPrices;
    var divCustomers = divMain.find('[name=divCustomers]');

    (function () { // init

        butRefresh_Click();

    })();

    function butRefresh_Click() {

        divResult.hide();
        divMain.block();
        AjaxSend({
            url: '/Reports/GetPriceLevels',
            success: function (vData) {
                divMain.unblock();
                if (!HandleSessionExpired(vData)) return;
                LoadGridPrices(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divMain.unblock();
                divResult.MakeError(XMLHttpRequest.responseText);
            }
        });

    }; // END butRefresh_Click

    function LoadGridPrices(vData) {

        vData.Columns = {
            PriceLevelName: {
                Label: 'Price Level Name'
            },
            IsActive: {
                Label: 'Is active',
                Control: 'Checkbox'
            }
        };
        mGridPrices = divPrices.ProWebGrid(vData)
            .AfterRowActivate(GridStocks_AfterRowActivate);

    }; // END LoadGridPrices

    function GridStocks_AfterRowActivate(e) {

        divResult.hide();
        divMain.block();
        AjaxSend({
            url: '/Reports/GetPriceLevelCustomers',
            data: { ID: mGridPrices.RowData(e.SelectedRow).ID },
            success: function (vData) {
                divMain.unblock();
                if (!HandleSessionExpired(vData)) return;
                LoadGridCustomers(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divMain.unblock();
                divResult.MakeError(XMLHttpRequest.responseText);
            }
        });

    }; // END GridStocks_AfterRowActivate

    function LoadGridCustomers(vData) {

        divCustomers.ProWebGrid(vData);

    }; // END LoadGridCustomers
});