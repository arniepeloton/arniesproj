define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {

        Browser.Init('TransBomMaker', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            AutoSearch: true,
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', '#')
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.Num)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text('Product Maker #: ' + vDataRow.Num)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text('Last Updated: ' + $.format.date(ParseDate(vDataRow.LastUpdated), proj.DateFormat))
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text('Updated by: ' + vDataRow.Updater)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.TransBomMaker.Click))
                            Browser.TransBomMaker.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {
    });

        if (OfflineMode) {
        var pDeferred = $.Deferred();
        require(['/Scripts3/Offline/Browser/TransBomMaker.js?crc='], function() {    
            pDeferred.resolve({});
        }); // require
        return pDeferred.promise();
    }; // if (OfflineMode) {

});