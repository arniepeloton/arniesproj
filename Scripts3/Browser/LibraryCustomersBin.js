define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryCustomersBin', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            Title: 'Customer\'s Bin',
            AutoSearch: true,
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryCustomersBin?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .text(vDataRow.Name)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryCustomersBin.Click))
                            Browser.LibraryCustomersBin.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
            , PageShow: function () {

                Browser.LibraryCustomersBin.ShowRightButton().unbind("click")
                    .click(function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        window.IsPageLoaded = false;
                        ChangePage('/fowstbin/BinInfo');
                    });

            }
        }); // Browser.Init('Customer', {

    });
});