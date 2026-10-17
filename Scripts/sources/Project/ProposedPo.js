$(function() {
    var formSearch = $('#formSearch')
        .submit(formSearch_Submit);
    var txtSearch = formSearch.find('[name=txtSearch]')
        .focus();
    var butSearch = formSearch.find('[name=butSearch]');
    var mDataGrid;
    var divResult = $('#divResult');
    var divSearch = $('#divSearch');
    var mData;
    var mDataGrid;
    var divDetails = $('#divDetails');

    // details
    var lblSupplierName = divDetails.find('[name=lblSupplierName]');

    function formSearch_Submit(e) {

        e.preventDefault();
        divSearch.block();
        divResult.hide();
        AjaxSend({
            url: '/drp/ProposedPoSearch',
            data: formSearch.serializeObject(),
            success: function(vData) {
                $('#divData').empty(); // remove its children and text
                divSearch.unblock();
                if (!HandleSessionExpired(vData)) return;
                mData = vData.d;
                if (mData.Error != undefined) {
                    divResult.text(mData.Error).MakeError();
                    divSearch.unblock();
                    return;
                } else if (mData.Text) {
                    divResult.text(mData.Text).MakeHighlight();
                    divSearch.unblock();
                    return;
                };
                mData.Columns = {
                    Num: {
                        Label: 'Proposed PO #',
                        MajorDataType: 'Numeric'
                    },
                    SupplierName: {
                        Label: 'Supplier'
                    },
                    ContactNumber: {
                        Label: 'Supplier Contact No.'
                    },
                    EmailAddress: {
                        Label: 'Supplier Email'
                    },
                    SupplierAddress: {
                        Label: 'SupplierAddress'
                    },
                    DateNeeded: {
                        Label: 'Date Needed',
                        MajorDataType: 'Date',
                        Format: 'M/d/yyyy'
                    },
                    CancellationDate: {
                        Label: 'Cancellation Date',
                        MajorDataType: 'Date',
                        Format: 'M/d/yyyy'
                    },
                    DateNeeded: {
                        Label: 'Date Needed',
                        MajorDataType: 'Date',
                        Format: 'M/d/yyyy'
                    },
                    ShipperName: {
                        Label: 'Shipper'
                    }
                };
                mDataGrid = $('#divData').ProDataGrid(mData);
                mDataGrid.AfterRowActivate(mDataGrid_AfterRowActivate);
            },
            error: function(XMLHttpRequest,textStatus,errorThrown) {
                divSearch.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END formSearch_Submit

    function mDataGrid_AfterRowActivate(e) {

        var pDr;
        var pText;

        pDr = mDataGrid.RowData(e.SelectedRow);
        lblSupplierName.text(pDr.SupplierName);

    }; // END mDataGrid_AfterRowActivate
});