$(function(){

    new AmountAndQuantityReport();
//    new WebGridTest();

});

function AmountAndQuantityReport() {

    // controls
    var formMain = $('#formMain')
        .submit(formMain_Submit);
    var divSupplier = $('#divSupplier');
    var butBrowseSupplier = $('#butBrowseSupplier')
        .click(butBrowseSupplier_Click);
    var txtDateFrom = $('#txtDateFrom')
        .datepicker({
            changeMonth: true,
			changeYear: true
        });
    var txtDateTo = $('#txtDateTo')
        .datepicker({
            changeMonth: true,
			changeYear: true
        });
    var hidSupplierId = $('<input type="hidden" name="hidSupplierId">').appendTo(formMain);
    var mFindSupplier;
    var divResult = $('#divResult');

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
                OnOk: function(vData) {
                    divSupplier.text(vData.SupplierName);
                    hidSupplierId.val(vData.ID);
                }
            });
        mFindSupplier.ShowDialog();
    }; // END butBrowseSupplier_Click

    function formMain_Submit(e) {

        var pResult;

        e.preventDefault();
        pResult = Validator.ValidateDate({
            Value1: txtDateFrom.val(),
            Value2: txtDateTo.val(),
            Field1: 'Date From',
            Field2: 'Date To'
        });
        if (pResult != null) {
            ShowMessage(pResult, 'Invalid');
            return;
        };

        if (hidSupplierId.val().length == 0) {
            ShowMessage('Select a supplier first', 'Supplier');
            return;
        };
        $('.divInput').block();
        AjaxSend({
            url: '/Reports/AmountAndQuantityReport',
            data: formMain.serializeObject(),
            success: function (vData) {
                if (vData.d) {
                    if (vData.d.Text) {
                        divResult
                            .html('<a href="/download/' +  vData.d.Text + '">Please click here to download the document again</a>')
                            .MakeHighlight();
                        window.location = '/download/' + vData.d.Text;
                    } else if (vData.d.Error) {
                        divResult
                            .text(vData.d.Error)
                            .MakeError();
                    };
                } else {
                };
                $('.divInput').unblock();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('.divInput').unblock();
//                mParent.unblock();
//                divResult.hide()
//                    .fadeIn(1000)
//                    .ScrollTo()
//                    .text('An error occured while retrieving data. ' + errorThrown + ' Error: ' + XMLHttpRequest.responseText);
            }
        }); // AjaxSend
    }; // END formMain_Submit

}; // END AmountAndQuantityReport

function WebGridTest() {

        AjaxSend({
            url: '/Suppliers/Search',
            data: {
                'txtSearch': ''
            },
            success: function (vData) {
                vData.d.Columns = {
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
                }; // END Columns

                $('#divGridTest').ProWebGrid({
                    Data: vData.d
//                    ,HoverEffect: false
//                    ,Sortable: false
//                    ,FixedHeaders: false
                });

                if (vData.d) {
                    if (vData.d.Text) {
                        divResult
                            .html('<a href="/download/' +  vData.d.Text + '">Please click here to download the document again</a>')
                            .MakeHighlight();
                        window.location = '/download/' + vData.d.Text;
                    } else if (vData.d.Error) {
                        divResult.text(vData.d.Error);
                    };
                } else {
                };
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                var qweqweqwe;
//                mParent.unblock();
//                divResult.hide()
//                    .fadeIn(1000)
//                    .ScrollTo()
//                    .text('An error occured while retrieving data. ' + errorThrown + ' Error: ' + XMLHttpRequest.responseText);
            }
        }); // AjaxSend
    
}; // END WebGridTest