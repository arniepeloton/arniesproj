define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    require([(OfflineMode ? '/Scripts3/Offline/Browser/TransReceivingAndTransfers.js?crc=' : '')], function () {
        $(function () {

            Browser.Init('TransReceivingAndTransfers', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally

                Title: 'Receipts And Transfers',
                AutoSearch: true,
                OnLink: function (vDataRow, vItem) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')
                    var pElement;

                    vItem.append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text((vDataRow.IsReceiving == 0 ? 'T-Out #' : 'Receiving #') + vDataRow.ReceivingNo)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text((vDataRow.IsReceiving == 0 ? 'From: ' : 'To: ') + (vDataRow.ReceivingPlaceFowst || vDataRow.ReceivingPlaceThirdParty || vDataRow.ReceivingPlace))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text((vDataRow.IsReceiving == 0 ? 'To: ' : 'From: ') + (vDataRow.SourceOfSKU1 || vDataRow.SourceOfSKU2))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text(vDataRow.DateReceived)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text('SKU Count: ' + vDataRow.SKUCount)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text('Transported By: ' + vDataRow.TransportedBy)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .html('Status: ' + (CBool(vDataRow.IsDraft) == true ? '<span style="color:#F00;">Draft</span>' : '<span style="color:#4CAF50">Finalized</span>'))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text('Last Updated: ' + $.format.date(new Date(vDataRow.LastUpdated), 'MMM dd, yyyy hh:mm a'))
                        );
                    if (!$.isEmptyObject(vDataRow.NetSuiteNum))
                        vItem.append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('NetSuite No.: ' + vDataRow.NetSuiteNum)
                            );
                    vItem.click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.TransReceivingAndTransfers.Click))
                            Browser.TransReceivingAndTransfers.Click($(this).data('data'));
                        //history.back();

                        return false;
                    });

                } // OnItem: function(vDataRow, vItem) {
                //        , PageShow: function () {

                //            Browser.LibraryFowst.ShowRightButton().unbind("click")
                //                .click(function (e) {
                //                    e.preventDefault();
                //                    e.stopPropagation();

                //                    ChangePage('/OrganizationShort/OrganizationAddress');
                //                });

                //        }
            }); // Browser.Init('Customer', {

        }); // $(function () {
    }); // require
});
