define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryCustomerFowst', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            Title: 'FOWST',
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
        }); // Browser.Init('Customer', {

    });

    if (OfflineMode) {
        var pDeferred = $.Deferred();
        require(['/Scripts3/Offline/Browser/LibraryCustomerFowst.js?crc='], function() {    
            pDeferred.resolve({});
        }); // require
        return pDeferred.promise();
    }; // if (OfflineMode) {
});
