define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('ReadyToInvoice', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Ready to Invoice',
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'dummy?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text('SO#: ' + vDataRow.MIN + '-' + vDataRow.SalesOrderNo)
                    )
                    .append($('<p>')
                        .text('Customer: ' + vDataRow.CompanyName)
                    )
                    .append($('<p>')
                        .text($.format.date(new Date(vDataRow.LastUpdated), 'MMM d, yyyy h:mm a'))
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.ReadyToInvoice.Click))
                            Browser.ReadyToInvoice.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {

    });
});