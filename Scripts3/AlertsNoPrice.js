require(['common'
    ], function() {

    $(function () {

        Refresh();

        $('#butRefresh').click(function() {
            Refresh();
        });

        function Refresh() {

            $('#divMessage')
                .text('Loading...')
                .show();
            AjaxSend({
                url: '/Alerts/NoPriceData',
                success: function(vData) {
                    if (vData.Warning) {
                        $('#divTable').html('');
                        $('#divList').hide();
                        $('#divMessage')
                            .text(vData.Warning)
                            .show();
                        $('#butDelete').closest('.ui-btn').hide();
                    } else {
                        $('#divList').show();
                        $('#divMessage').hide();
                        LoadData(vData);
                        $('#butDelete').closest('.ui-btn').show();
                    };
                }

            });
        }; // function Refresh() {

        function LoadData(vData) {

            var pTable = CreateTable(vData.data, {
                AddAllColumns: false,
                Columns: {
                    CheckBox: {
                        Text: ' '
                    },
                    ProductNo: {
                        Text: 'Product'
                    },
                    LastUpdated: {
                        Text: 'Last Updated',
                        DataType: 'date',
                        Format: 'M/d/yyyy h:mm a'
                    },
                    EmployeeName: {
                        Text: 'Update By'
                    }
                },
                BeforeCell: function(e) {
                    switch (e.Key) {
                        case 'CheckBox':
                            e.Cell.append(
                                $('<label>')
                                    .width('50px')
                                    .append(
                                    $('<input type="checkbox">')
                                        .data('ProductID', e.DataRow.ProductID)
                                )
                            );
                            return false;

                        case 'ProductNo':
                            e.Text = e.DataRow.ProductNo + ' - ' + e.DataRow.ProductName;
                            return true;
                            break;

                        default:
                            return true;
                    }
                },
                BeforeRow: function(e) {
                    return true; // add this row to the table
                }
            });

            $('#divTable')
                .html('')
                .append(pTable);
            pTable.table();
            pTable.trigger('create');            

        }; // function LoadData(vData) {

        $('#butDelete').click(function() {

            var pChecked = $('input:checked');

            if (pChecked.length == 0) {
                MessageBox('Please select the rows that you want to delete');
            } else {
                MessageBox('Are you sure you want to delete the checked rows from "No Price" list? ' +
                    'NOTE: The products will not be deleted from the library.', '', [
                    {
                        Text: 'Yes',
                        Click: function() {
                            Delete(pChecked);
                        }
                    }, {
                        Text: 'No'
                    }
                ]);
            };

        }); // $('#butDelete').click(function() {

        function Delete(vElements) {
            
            var pProductIDs = [];

            vElements.each(function(vIndex, vItem) {
                pProductIDs.push($(vItem).data('ProductID'));
            });

            AjaxSend({
                url: '/Alerts/NoPriceDelete',
                data: {ProductIDs: pProductIDs},
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                    } else {
                        MessageBox('Success', '', [], {
                            OnClose: function() {
                                Refresh();
                            }
                        });
                    };
                }
            }); // AjaxSend({

        }; // function Delete() {

    });
});