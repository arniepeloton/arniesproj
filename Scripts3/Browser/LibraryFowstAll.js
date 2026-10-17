define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryFowstAll', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            AutoSearch: true,
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                      .data('data', vDataRow)
                      .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vDataRow.FowstName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text('Corporate Name : ' + vDataRow.FowstOrgName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text('Address : ' + vDataRow.Address)

                        )
                        .click(function (e) {
                            e.preventDefault();

                            if ($.isFunction(Browser.LibraryFowstAll.Click))
                                Browser.LibraryFowstAll.Click($(this).data('data'));
                          
                            return false;
                        })
                    );

            } // OnItem: function(vDataRow, vItem) {

        }); // Browser.Init('Customer', {

    });
});
