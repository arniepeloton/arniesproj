define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    // require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryPIMProductPublic.js?crc=' : '')], function() {    
    require([], function() {    
        $(function () {

            Browser.Init('LibraryPIMProductPublic', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally
                Title: 'PIM', // optional. Defaults to "Search"
                OnLink: function (vDataRow, vItem, e) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')
                    var pDiv;
             
                    pDiv = $('<div>')
                        .addClass('ui-block-a')
                        .append($('<img>')
                            .css({
                                'max-width': '80px',
                                'max-height': '80px',
                                'margin-right': '9px'
                            })
                            .attr('src', '../Content/images/box.png'));
                    vItem.append(pDiv);

                    if (CDec2(vDataRow.IsBrandOwner) != 0) {
                        pDiv = $('<div>')
                            .click(function () {
                                MessageBox('SKUs with the "Official Item Record" icon are SKU records sourced and maintained by the official brand owner (or authorized agent). ' +
                                    'You get automatic updates to changes made by the brand owner such as new and/or modified item descriptions, pictures, SRP, packing sizes, dimensions, bar code numbers, and more. ' +
                                    'We strongly recommend that you use these official records in all of your transactions.',
                                    'Official Item Record');
                                return false;
                            })
                            .addClass('ui-block-b')
                            .append($('<img>')
                                .css({
                                    'max-width': '80px',
                                    'max-height': '80px',
                                    'margin-right': '9px'
                                })
                                .attr('src', '../Content/images/OfficialItem.gif')); //edited by janine. add ../ feb 11 2014
                        vItem.append(pDiv);
                    };

                    pDiv = $('<div>')
                        .addClass('ui-block-c')
                        .append($('<h3>')
                            .css('white-space', 'normal')
                            .text((e.Index + 1) + '.) ' + vDataRow.ProductNo + ' ' + vDataRow.ProductName)
                        ).append($('<p>')
                            .css('white-space', 'normal')
                            .text('Published by: ' + vDataRow.PublishedBy + (CDec2(vDataRow.IsBrandOwner) != 0 ? ' (Brand Owner)' : ''))
                        ).append($('<p>')
                            .css('white-space', 'normal')
                            .text('Uploaded by: ' + vDataRow.UploadedBy)
                        );
                    vItem.append(pDiv);

                } // OnItem: function(vDataRow, vItem) {

                , PageShow: function () {
                    Browser.LibraryPIMProductPublic.Page.data('CanRefreshStockPage', 1);
                    //                Browser.LibraryStockInformation.ShowRightButton()
                    //                    .click(function (e) {
                    //                        e.preventDefault();
                    //                        ChangePage('/customer');
                    //                    });

                    if (!OfflineMode) {
                        Browser.LibraryPIMProductPublic.ShowSearchMore().unbind("click")
                            .click(function (e) {
                                e.preventDefault();
                                e.stopPropagation();

                                //Set search to public mode
                                Browser.LibraryPIMProductPublic.InternalInputData.IsSearchMore = 1;
                                Browser.LibraryPIMProductPublic.doSearch();
        //                        //Restore to private mode, If want to make it private search after search more
        //                        Browser.LibraryPIMProduct.InternalInputData.IsSearchMore = 0;
                            });
                    }; // if (!OfflineMode) {

                }
            }); // Browser.Init('LibraryCustomers', {

        }); // document ready
    }); // require offline
}); // define