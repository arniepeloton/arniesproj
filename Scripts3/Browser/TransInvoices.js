define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {

    $(function () {

        var mInitialized;
        var invoiceID = 0;
        var browserClick = '';

        Browser.Init('TransInvoices', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Sales Invoices', // optional. Defaults to "Search"
            AutoSearchIfBlank: true,
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')
                //                    if (GetUrlParameters().mode == 'pos') {
                //                        vItem.append($('<a>')
                //                            .attr('href', 'Invoice?id=' + vDataRow.ID)
                //                            .data('data', vDataRow)
                //                            .append($('<h3>')
                //                                .text('Invoice No.: ' + vDataRow.InvoiceNum)
                //                            )
                //                            .append($('<p>')
                //                                .text(vDataRow.CompanyName)
                //                            ).append($('<p>')
                //                                .text($.format.date(new Date(vDataRow.InvoiceDate), 'MMM d, yyyy h:mm a'))
                //                            ).append($('<p>')
                //                                .text('Total Value: ' + AccountingFormat(vDataRow.Total))
                //                            ).append($('<p>')
                //                                .text('SKU Count: ' + vDataRow.SkuCount)
                //                            ).click(function (e) {
                //                                e.preventDefault();
                //                                if ($.isFunction(Browser.TransInvoices.Click))
                //                                    if (OfflineMode) {
                //                                        Browser.TransInvoices.Click($(this).data('data'));
                //                                        history.back();
                //                                    } else
                //                                        ShowPreview(vDataRow.ID, $(this).data('data'), $.format.date(new Date(vDataRow.InvoiceDate), 'MMM d, yyyy h:mm a'), vDataRow.CompanyName, AccountingFormat(vDataRow.Total), vDataRow.SkuCount); //edited by Janine Oct 17 8am 
                //                                //                            
                //                                return false;
                //                            })
                //                        );
                //                    }
                //                    else {

                // additonal by janine may 16 11am
                //put condition if table no or gas pump no if GP or TBL
                var refNo;
                var pTransName;
                var pCustomerLabel;
                var pStatusP;
                var pFowstLabel;

                if (CStr2(vDataRow.TableNo) != '') {
                    var prefix = vDataRow.TableNo.split('_');
                    var label;

                    if (prefix[0] == 'GP')
                        refNo = 'Gas Pump No. : ' + prefix[1];
                    else
                        refNo = 'Customer / Table No. : ' + prefix[prefix.length - 1];
                } else {
                    refNo = '';
                }
                //end of additional
                if (vDataRow.PurchaserName) {
                    pTransName = 'Purchase Order No.: ';
                    pTransName += vDataRow.MIN + '-' + (vDataRow.PurchaseOrderNo || '');
                    pCustomerLabel = 'Supplier : ';
                    if (vDataRow.IsDraft == undefined)
                        vDataRow.IsDraft = 1;
                    pStatusP = $('<p>').text('Status : ' + ((CBool(vDataRow.IsDraft)) ? 'Draft' : 'Finalized'));
                    pFowstLabel = 'Customer : ';
                } else {
                    pTransName = 'Sales Order No.: ';
                    pTransName += vDataRow.MIN + '-' + (vDataRow.SalesOrderNo || '');
                    pCustomerLabel = 'Customer : ';
                    pFowstLabel = 'FOWST : ';
                }

                if (vDataRow.SalesOrderNo > 0) {
                    vItem.append($('<a>')
                        .attr('href', 'Invoice?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .text(pTransName)
                        )
                        .append($('<h3>')
                            .text(refNo) //see above additional
                        )
                        .append($('<p>')
                            .text(pCustomerLabel + vDataRow.CompanyName)
                        ).append($('<p>')
                            .text($.format.date(new Date(vDataRow.InvoiceDate), 'MMM d, yyyy h:mm a'))
                        ).append($('<p>')
                            .text(pFowstLabel + vDataRow.FOWSTName)
                        ).append($('<p>')
                            .text('Device : ' + vDataRow.DeviceName)
                        ).append($('<p>')
                            .text('Total Value: ' + AccountingFormat(vDataRow.TotalOrder || vDataRow.TotalOrder)) // TotalOrder = offline; TotalOrder = online
                        ).append($('<p>')
                            .text('SKU Count: ' + vDataRow.SkuCountOrder)
                        ).append($('<p>')
                            .text('Created By: ' + vDataRow.EmployeeFullName)
                        ).append(pStatusP)
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.TransInvoices.Click))
                                Browser.TransInvoices.Click($(this).data('data'));                     
                            return false;
                        })
                    );
                } else if (vDataRow.PurchaserName) {
                    vItem.append($('<a>')
                        .attr('href', 'Invoice?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .text(pTransName)
                        )
                        .append($('<h3>')
                            .text(refNo) //see above additional
                        )
                        .append($('<p>')
                            .text('Customer: ' + vDataRow.CompanyName)
                        ).append($('<p>')
                            .text('Outlet: ' + vDataRow.CustomerFowst)
                        ).append($('<p>')
                            .text('Department: ' + vDataRow.CustomerBin)
                        ).append($('<p>')
                            .text($.format.date(new Date(vDataRow.InvoiceDate), 'MMM d, yyyy h:mm a'))
                        ).append($('<p>')
                            .text('Supplier: ' + (vDataRow.SupplierName || vDataRow.FOWSTName))
                        ).append($('<p>')
                            .text('Device : ' + vDataRow.DeviceName)
                        ).append($('<p>')
                            .text('Total Value: ' + AccountingFormat(vDataRow.TotalOrder || vDataRow.TotalOrder)) // TotalOrder = offline; TotalOrder = online
                        ).append($('<p>')
                            .text('SKU Count: ' + vDataRow.SkuCount)
                        ).append($('<p>')
                            .text('Created By: ' + vDataRow.EmployeeFullName)
                        ).append(pStatusP)
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.TransInvoices.Click))
                                Browser.TransInvoices.Click($(this).data('data'));                          
                            return false;
                        })
                    );
                } else {
                    vItem.append($('<a>')
                        .attr('href', 'Invoice?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .text(pTransName)
                        )
                        .append($('<h3>')
                            .text(refNo) //see above additional
                        )
                        .append($('<p>')
                            .text(pCustomerLabel + vDataRow.CompanyName)
                        ).append($('<p>')
                            .text($.format.date(new Date(vDataRow.InvoiceDate), 'MMM d, yyyy h:mm a'))
                        ).append($('<p>')
                            .text(pFowstLabel + vDataRow.FOWSTName)
                        ).append($('<p>')
                            .text('Device : ' + vDataRow.DeviceName)
                        ).append($('<p>')
                            .text('Total Value: ' + AccountingFormat(vDataRow.TotalOrder || vDataRow.TotalOrder)) // TotalOrder = offline; TotalOrder = online
                        ).append($('<p>')
                            .text('SKU Count: ' + vDataRow.SkuCount)
                        ).append($('<p>')
                            .text('Created By: ' + vDataRow.EmployeeFullName)
                        ).append(pStatusP)
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.TransInvoices.Click))
                                Browser.TransInvoices.Click($(this).data('data'));                          
                            return false;
                        })
                    );
                }
                //                    }
            } // OnItem: function(vDataRow, vItem) {

        }); // Browser.Init('xxx', {

        Browser.Init('TransInvoicesReturnables', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Sales Invoices', // optional. Defaults to "Search"
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'Invoice?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .text('Invoice No.: ' + vDataRow.InvoiceNum)
                    ).append($('<p>')
                        .text('Invoice Date : ' + $.format.date(new Date(vDataRow.InvoiceDate), 'MMM d, yyyy h:mm a'))
                //                    ).append($('<p>')
                //                        .addClass('ui-li-aside')
                //                        .text($.format.date(new Date(vDataRow.LastUpdated), 'MMM d, yyyy h:mm a'))
                    ).append($('<p>')
                        .text('Customer : ' + vDataRow.CompanyName)
                    ).append($('<p>')
                        .text('FOWST : ' + vDataRow.FOWSTName)
                    ).append($('<p>')
                        .text('Device : ' + vDataRow.DeviceName)
                    ).append($('<p>')
                        .text('Total Value: ' + AccountingFormat(vDataRow.Total))
                    //).append($('<p>')
                    //    .text('SKU Count: ' + vDataRow.SkuCount)
                    //).append($('<p>')
                    //    .text('Returnable Qty.: ' + AccountingFormat(vDataRow.Returnables, 0))
                    ).click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.TransInvoicesReturnables.Click))
                            Browser.TransInvoicesReturnables.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('xxx', {


    }); // 


    if (OfflineMode) {
        var pDeferred = $.Deferred();
        require(['/Scripts3/Offline/Browser/TransInvoices.js?crc='], function () {
            pDeferred.resolve({});
        }); // require
        return pDeferred.promise();
    }; // if (OfflineMode) {
});