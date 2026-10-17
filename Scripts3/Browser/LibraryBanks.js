define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {

        Browser.Init('LibraryBanks', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                    vItem.append($('<a>')
                        .attr('href', 'LibraryBanks?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vDataRow.BankName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Code : " + vDataRow.BankCode)
                        )
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.LibraryBanks.Click))
                                Browser.LibraryBanks.Click($(this).data('data'));
                            history.back();

                            return false;
                        })
                    );
                } // OnItem: function(vDataRow, vItem) {
//                , PageShow: function () {

//                    Browser.LibraryBanks.ShowManualEntry().unbind("click")
//                        .click(function (e) {
//                            e.preventDefault();
//                            e.stopPropagation();

//                            var pData = { IsManualEntry: true };
//                            Browser.LibraryBanks.Click(pData);                                

//                            history.back();

//                            return false;
//                    });

//                }


        }); // Browser.Init('Customer', {
    });
});