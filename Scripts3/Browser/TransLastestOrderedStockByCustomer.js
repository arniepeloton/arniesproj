define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function() {

        Browser.Init('TransLastestOrderedStockByCustomer', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Recently Purchased', // optional. Defaults to "Search"
            AutoSearch: true,
            OnItem: function(vDataRow, vItem, e) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'Stock?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<img>')
                        .attr('src', '/Content/images/box.png')
                    ).append($('<h3>')
                        .css('white-space','normal')
                        .text((e.Index + 1) + '.) ' + vDataRow.StockNumber + ' ' + vDataRow.StockDescription)
                    )
                    .append($('<p>')
                        .text('Last purchase date: ' + $.format.date(new Date(vDataRow.TransDate), 'MMM d, yyyy'))
                    ).append($('<p>')
                        .text('Total value: ' + $.format.number(parseFloat(vDataRow.Subtotal),'#,###.00'))
    //                ).append($('<p>')
    //                    .addClass('ui-li-aside')
    //                    .text(vDataRow.TinNumber)
                    ).click(function(e) {
                        e.preventDefault();
    //                    if ($.isFunction(Browser.TransLastestOrderedStockByCustomer.Click))
    //                        Browser.TransLastestOrderedStockByCustomer.Click($(this).data('data'));
                        alert('This function is not yet implemented');
                        //history.back();

                        return false;
                    })
                );
            }, // OnItem: function(vDataRow, vItem) {
            AfterResult: function(vData) {
                Browser.TransLastestOrderedStockByCustomer.Page.find('.divBrowserContentTop').text(vData.DateRange);
            }
        }); // Browser.Init('LibraryCustomers', {

    });
});