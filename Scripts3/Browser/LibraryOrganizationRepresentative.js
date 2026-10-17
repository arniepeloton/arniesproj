define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryOrganizationRepresentative.js?crc=' : '')], function () {
        $(function () {

            Browser.Init('LibraryOrganizationRepresentative', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally

                Title: 'Customer Representative',
                AutoSearch: true,
                OnItem: function (vDataRow, vItem) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')

                    vItem.append($('<a>')
                        .attr('href', 'LibraryOrganizationRepresentative?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vDataRow.RepresentativeName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Tel. No. : " + vDataRow.TelNo)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Mobile No. : " + vDataRow.MobileNo)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Email : " + vDataRow.Email)
                        )
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.LibraryOrganizationRepresentative.Click))
                                Browser.LibraryOrganizationRepresentative.Click($(this).data('data'));
                            history.back();

                            return false;
                        })
                    );
                } // OnItem: function(vDataRow, vItem) {
                , PageShow: function () {

                    Browser.LibraryOrganizationRepresentative.ShowRightButton().unbind("click")
                        .click(function (e) {
                            e.preventDefault();
                            e.stopPropagation();
                            ChangePage('/OrganizationShort/OrganizationRepresentativeOld');
                            $('#pageRepList').show()
                            $("div[data-url='LibraryOrganizationRepresentative']").hide()
                        });

                }
            }); // Browser.Init('Customer', {

        });

    }); // require
});