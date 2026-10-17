function Finder(vOptions) {
// vOptions:
//      Title = modal form caption
//      Url = url to controller
//      Columns = ProWebDataGrid.Column
//      OnOk = function(vData)

    var mDivDialog;
    var mDataGrid;      // ProWebGrid
    var mPanelDiv;      // <div> that is inside the dialog. This contains the search text box, button and table. This exludes the OK and CANCEL of the form
    var mSearchDiv;     // <div>
    var mTableDiv;      // <div>
    var mDivMessage;

    ShowDialog();

    this.ShowDialog = ShowDialog;
    function ShowDialog() {
        
        if (mDivDialog == null) {
            mDivDialog = $('<div>')
            mDivDialog.dialog({
                title: vOptions.Title,
                modal: false,
                buttons: {
                    'Ok': Ok_Click,
                    'Cancel': function () {
                        $(this).dialog('close');
                    }
                }, // END buttons
                autoOpen: false,
                minWidth: 500,
                minHeight: 329,
                create: OnCreate,
                beforeClose: function () {
                    //mPanelDiv.hide();
                    if (mDataGrid) mDataGrid.StoreScroll();
                    $(document).append(mTableDiv); // remove table from dialog before closing
                    // read the issue here:
                    // http://old.nabble.com/dialog-slow-opening-closing-to-the-point-of-not-being--usable...%28IE-only-problem,-naturally...%29-td21804548s27240.html
                }, // END beforeClose
                open: function () {
                    // delay to prevent flickering.
                    // otherwise, flickering will happend if the data grid contains large amount of data and user scrolls downward
                    setTimeout(function () {
                        mPanelDiv.show();
                        mPanelDiv.append(mTableDiv);
                        if (mDataGrid) mDataGrid.RestoreScroll();
                    }, 1);
                },
                resizeStop: OnResize
            });  // END mDivDialog.dialog
        }; // END (mDivDialog == null)

        mDivDialog.dialog('open');
    }; // END ShowDialog

    function OnCreate() {

        var formSearch;     // <form>
        var txtSearch;      // <input>
        var butSearch;      // <button>
        var pLabel;         // <span>
        var pDiv;

        mDivMessage = $('<div>');
        pLabel = $('<span>').text('Search:');
        txtSearch = $('<input>')
            .attr('name', 'txtSearch')
            .width(300)
            .addClass('text');
        butSearch = $('<button type="submit" />')
            .css('height', '2px')
            .css('width', '26px');

        pDiv = $('<div>')
            .css('margin', '5px')
            .addClass('SearchBox')
            .append(txtSearch)
            .append(butSearch);
        formSearch = $('<form>');
        formSearch.submit(formSearch_Submit);
        formSearch
            .append(mDivMessage)
            .append(pLabel)
            .append(pDiv);
        mSearchDiv = $('<div>');
        mSearchDiv.append(formSearch);

        mPanelDiv = $('<div>');
        mTableDiv = $('<div>').height(200);
        mPanelDiv
            .append(mSearchDiv); // append the mTableDiv during open
        $(this).append(mPanelDiv);

        InitCss();
    }; // END OnCreate

    function formSearch_Submit(e) {
        
        e.preventDefault();
        mPanelDiv.block();
        mDivMessage.hide();

        AjaxSend({
            url: vOptions.Url,
            data: $(this).serializeObject(),
            success: function (vData) {
                if (HandleSessionExpired(vData) == false) return;
                LoadData(vData);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                mPanelDiv.unblock();
                mDivMessage.text(XMLHttpRequest.responseText).MakeError();
            }
        }); // END AjaxSend

    }; // END formSearch_Submit

    function OnResize() {
        
        mPanelDiv.height(mPanelDiv.parent().height());
        mTableDiv.height(mPanelDiv.height() - mSearchDiv.height());

    }; // END OnResize

    // the Ok button was clicked
    // or a datarow was double-clicked
    function Ok_Click() {

        var pTr;
        var pData;

        if (mDataGrid == null) {
            $('<div>').text('Please select a data first').dialog();
        } else {
            pTr = mDataGrid.SelectedRow();
            if (pTr == null) {
                $('<div>').text('Please select a data first').dialog();
            } else {
                pData = pTr.data('ProWebGridDataRowKey');
                mDivDialog.dialog('close');
                if (vOptions.OnOk)
                    vOptions.OnOk(pData);
            };
        };

    }; // END Ok_Click

    function LoadData(vData) {

        vData.d.Columns = vOptions.Columns;
        mDataGrid = mTableDiv.ProWebGrid({
            Data: vData.d
            //            ,HoverEffect: false
            //            ,Sortable: false
            //            ,FixedHeaders: false
        }).RowDoubleClick(function () {
            Ok_Click();
        });
        mPanelDiv.unblock();

    }; // END LoadData

    this.DataGrid = function () {

        return mDataGrid;

    }; // END DataGrid
}; // END Finder
