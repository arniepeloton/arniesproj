define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function() {

        Browser.Init('TransSalesOrder', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Sales Orders', // optional. Defaults to "Search"
            AutoSearch: true,
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'SalesOrder?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .text('SO#: ' + vDataRow.OrderNo)
                    )
                    .append($('<p>')
                        .text($.format.date(new Date(vDataRow.LastUpdated), 'MMM d, yyyy h:mm a'))
//                    ).append($('<p>')
//                        .addClass('ui-li-aside')
//                        .text($.format.date(new Date(vDataRow.LastUpdated), 'MMM d, yyyy h:mm a'))
                    ).append($('<p>')
                        .text('Total Value: ' + AccountingFormat(vDataRow.Total))
                    ).append($('<p>')
                        .text('SKU Count: ' + vDataRow.SkuCount)
                    ).click(function(e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.TransSalesOrder.Click))
                            Browser.TransSalesOrder.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );

//                vItem.append($('<a>')
//                    .attr('href', '#')
//                    .data('data', vDataRow)
//                    .click(function(e) {
//                        e.preventDefault();
//                        if ($.isFunction(Browser.LibraryCustomers.ClickEdit))
//                            Browser.LibraryCustomers.ClickEdit($(this).data('data'));
//                        //history.back();

//                        return false;
//                    })
//                );

            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('xxx', {

    });
});