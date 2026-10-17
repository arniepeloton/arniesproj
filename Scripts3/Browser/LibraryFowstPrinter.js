    define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
        $(function () {

            Browser.Init('LibraryFowstPrinter', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally

                Title: 'FOWST Printer',
                AutoSearch: true,
                OnItem: function (vDataRow, vItem) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')

                    vItem.append($('<a>')
                    .attr('href', 'LibraryFowstPrinter?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.PrinterIPAddress)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryFowstPrinter.Click))
                            Browser.LibraryFowstPrinter.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
                } // OnItem: function(vDataRow, vItem) {
            }); // Browser.Init('Customer', {

        });
    });

