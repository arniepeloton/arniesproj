define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {

        Browser.Init('LibraryUNSPSCCommodity', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryUNSPSCCommodity?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Segment : " + vDataRow.SegmentCode + " - " + vDataRow.Segment)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Family : " + vDataRow.FamilyCode + " - " + vDataRow.Family)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Class : " + vDataRow.ClassCode + " - " + vDataRow.Class)
                    )
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text("Commodity: " + vDataRow.CommodityCode + " - " + vDataRow.Commodity)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryUNSPSCCommodity.Click))
                            Browser.LibraryUNSPSCCommodity.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {

    });
});
