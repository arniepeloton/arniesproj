define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryStockInformation', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Stock Information', // optional. Defaults to "Search"
            OnItem: function (vDataRow, vItem, e) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', GetPathFormat('Stock') + '?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<img>')
                        .attr('src', 'Content/images/box.png')
                    ).append($('<h3>')
                        .css('white-space', 'normal')
                        .text((e.Index + 1) + '.) ' + vDataRow.StockNumber + ' ' + vDataRow.StockDescription)
                    ).click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryStockInformation.Click)) {
                            Browser.LibraryStockInformation.Click($(this).data('data'));
                            history.back();
                        } else {
                            proj.StockID = vDataRow.ID;  
                            ChangePage('Stock');
                        };
                        return false; 
                    })
                );
            } // OnItem: function(vDataRow, vItem) {

            , PageShow: function () {

//                Browser.LibraryStockInformation.ShowRightButton()
//                    .click(function (e) {
//                        e.preventDefault();
//                        ChangePage('/customer');
//                    });

            }
        }); // Browser.Init('LibraryCustomers', {

    });
});