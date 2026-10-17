define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryCustomerFowstBin', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            Title: 'FOWST Bin',
            AutoSearch: true,
            OnLink: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.FowstName)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.Address)
                    );

            } // OnItem: function(vDataRow, vItem) {
            , PageShow: function () {

                Browser.LibraryCustomerFowstBin.ShowRightButton().unbind("click")
                    .click(function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        ChangePage('/fowstbin/BinInfo?CustomerBin=1');
                    });

            }
        }); // Browser.Init('Customer', {

    });
});
