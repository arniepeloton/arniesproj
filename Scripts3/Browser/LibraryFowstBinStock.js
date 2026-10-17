define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryFowstBinStock', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            Title: 'FOWST Bin Stock',
            AutoSearch: true,
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryFowstBinStock?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .text(vDataRow.Name)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryFowstBinStock.Click))
                            Browser.LibraryFowstBinStock.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
//            , PageShow: function () {

//                Browser.LibraryFowstBinStock.ShowRightButton().unbind("click")
//                    .click(function (e) {
//                        e.preventDefault();
//                        e.stopPropagation();
//                        window.IsPageLoaded = false;
//                        ChangePage('/fowstbin/AssignBinToStock');
//                    });

//            }
        }); // Browser.Init('Customer', {

    });
});