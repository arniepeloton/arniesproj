define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {

    $(function () {

        var mInitialized;
        var invoiceID = 0;
        var browserClick = '';

        Browser.Init('TransInvoicesForReprint', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Sales Invoices', // optional. Defaults to "Search"
            AutoSearchIfBlank: true,
            OnItem: function (vDataRow, vItem) {
                vItem.append($('<a>')
                    .attr('href', 'Invoice?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .text('Invoice No.: ' + vDataRow.InvoiceNum)
                    ).append($('<p>')
                        .text('Invoice Date : ' + $.format.date(new Date(vDataRow.InvoiceDate), 'MMM d, yyyy h:mm a'))
                    ).append($('<p>')
                        .text('Customer : ' + vDataRow.CompanyName)
                    ).append($('<p>')
                        .text('FOWST : ' + vDataRow.FOWSTName)
                    ).append($('<p>')
                        .text('Device : ' + vDataRow.DeviceName)
                    ).append($('<p>')
                        .text('Cashier : ' + vDataRow.EmployeeFullName)
                    ).append($('<p>')
                        .text('Total Value: ' + AccountingFormat(vDataRow.Total))
                    ).append($('<p>')
                        .text('SKU Count: ' + vDataRow.SkuCount)
                    ).append($('<p>')
                        .text('Returnable Qty.: ' + AccountingFormat(vDataRow.Returnables, 0))
                    ).click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.TransInvoicesForReprint.Click))
                            Browser.TransInvoicesForReprint.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {

        }); // Browser.Init('xxx', {

    }); // 

    if (OfflineMode) {
        var pDeferred = $.Deferred();
        require(['/Scripts3/Offline/Browser/TransInvoicesForReprint.js?crc='], function () {
            pDeferred.resolve({});
        }); // require
        return pDeferred.promise();
    }; // if (OfflineMode) {

});