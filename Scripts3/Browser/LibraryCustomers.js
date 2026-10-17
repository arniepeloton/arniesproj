define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function() {

        var mInitialized;

        Browser.Init('LibraryCustomers', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            Title: 'Customers', // optional. Defaults to "Search"
            OnItem: function(vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'Customer?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .css('white-space', 'normal')
                        .text(vDataRow.CompanyName)
                    )
                    .append($('<p>')
                        .text(vDataRow.ContactNumber)
                    ).append($('<p>')
                        .addClass('ui-li-aside')
                        .text(vDataRow.TinNumber)
                    ).click(function(e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryCustomers.Click))
                            Browser.LibraryCustomers.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );

                vItem.append($('<a>')
                    .attr('href', '#')
                    .data('data', vDataRow)
                    .click(function(e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryCustomers.ClickEdit))
                            Browser.LibraryCustomers.ClickEdit($(this).data('data'));
                        //history.back();

                        return false;
                    })
                );

            } // OnItem: function(vDataRow, vItem) {

            , PageShow: function () {

                Browser.LibraryCustomers.ShowRightButton().unbind("click")
                    .click(function (e) {
                        e.preventDefault();
                        e.stopPropagation();


                        var pData = {};
                        pData.IsNew = 0;
                        pData.ID = '';
                        
                        AjaxSend({
                            url: 'OrganizationShort/setCreateNew',
                            data: pData,
                            success: function (vData) {

                                window.IsPageLoaded = false;
                                ChangePage('/OrganizationShort/OrganizationInfo');

                            }
                        });

                    });

            }
        }); // Browser.Init('LibraryCustomers', {

    });
});