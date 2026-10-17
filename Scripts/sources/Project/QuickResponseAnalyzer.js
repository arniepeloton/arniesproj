$(function () {

    var divQrAnalyzer = $('#divQrAnalyzer');

    // Step 1: Choose branches
    var divBranches = divQrAnalyzer.find('[name=divBranches]');
    var mGridBranches;

    // Step 2: Choose SKU's
    var divStocks = divQrAnalyzer.find('[name=divStocks]');
    var butStock = divQrAnalyzer.find('[name=butStock]')
        .click(butStock_Click);
    var mFinderStock;
    var mDtStocks;

    // Step 3: Choose date range
    var txtDateFrom = divQrAnalyzer.find('[name=txtDateFrom]')
        .datepicker();
    var txtDateTo = divQrAnalyzer.find('[name=txtDateTo]')
        .datepicker();

    // Step 4: Generate report
    var butGenerate = divQrAnalyzer.find('[name=butGenerate]')
        .click(butGenerate_Click);

    (function Init() {
        if (mBranchData != null && mBranchData.d != null) {
            LoadBranches(mBranchData.d);
        };
    })();

    function LoadBranches(vData) {

        vData.Columns = {
            Include: {
                'Control': 'Checkbox',
                'MajorDataType': 'Numeric'
            },
            Name: {
                Label: 'Name'
            },
            CompanyName: {
                Label: 'Customer Account'
            },
            DCName: {
                Label: 'Distribution Center'
            },
            CpoName: {
                Label: 'Central Processing Office'
            },
            LastUpdated: {
                Label: 'Last Updated',
                MajorDataType: 'Date'
            },
            UpdatedBy: {
                Label: 'Updated By'
            }
        };
        mGridBranches = divBranches.ProDataGrid(vData);

    }; // END LoadBranches

    function butStock_Click() {

        if (mFinderStock) {
        } else {
            mFinderStock = new Finder({
                Title: 'Stock',
                Url: '/StockInformation/Search',
                Columns: {
                    StockNumber: {
                        Label: 'Stock number'
                    },
                    StockDescription: {
                        Label: 'Stock description'
                    }
                },
                OnOk: function (vData) {
                    AppendStockInfo(vData)
                }
            });
        };
        mFinderStock.ShowDialog();

    }; // END butStock_Click

    function AppendStockInfo(vData) {

        if (mDtStocks == null) {
            mDtStocks = {};
            mDtStocks.Columns = {
                StockNumber: {
                    Label: 'Stock Number'
                },
                StockDescription: {
                    Label: 'Stock Description'
                }
            };
            mDtStocks.Rows = [];
        };
        mDtStocks.Rows.push(vData);
        divStocks.ProWebGrid(mDtStocks);

    }; // END AppendStockInfo

    function butGenerate_Click() {

        var pCheckedData;
        var pStocks;

        pCheckedData = mGridBranches.GetCheckedRows('Include', ['NumericId']);
        if (pCheckedData.length == 0) {
            ShowMessage('Please check at least 1 branch first');
            return;
        };
        if (mDtStocks == null) {
            ShowMessage('Please add at least 1 SKU first');
            return;
        };
        if (IsDate(txtDateFrom.val()) == false) {
            ShowMessage('POS/CS Date From must be a valid date');
            return;
        };
        if (IsDate(txtDateTo.val()) == false) {
            ShowMessage('POS/CS Date To must be a valid date');
            return;
        };
        if (new Date(txtDateTo.val()) < new Date(txtDateFrom.val())) {
            ShowMessage('POS/CS Date To must not be earlier than POS/CS Date From');
            return;
        };

        pStocks = [];
        for(var p = 0; p <= mDtStocks.Rows.length - 1; p++) {
            pStocks.push(mDtStocks.Rows[p].ID);
        };

        divQrAnalyzer.block();
        AjaxSend({
            url: '/Reports/QrAnalyzerGenerate',
            data: {
                Branches: pCheckedData,
                Stocks: pStocks,
                DateFrom: new Date(txtDateFrom.val()),
                DateTo: new Date(txtDateTo.val())
            },
            success: function (vData) {
                //if (HandleSessionExpired(vData) == false) return;
                divQrAnalyzer.unblock();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divQrAnalyzer.unblock();
                ShowMessage(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END butGenerate_Click
});