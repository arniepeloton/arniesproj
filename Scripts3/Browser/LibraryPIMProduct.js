define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryPIMProduct.js?crc=' : '')], function () {
        $(function () {
            var pShowCondensedProductInformationInSearchResult;

            Browser.Init('LibraryPIMProduct', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally
                Title: 'PIM', // optional. Defaults to "Search"
                OnLink: function (vDataRow, vItem, e) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')
                    var pDiv;
                    var vItemContent = $('<div style="display:table;">');
                    var pStr;

                    if (OfflineMode
                        && (pShowCondensedProductInformationInSearchResult == null))
                        pShowCondensedProductInformationInSearchResult = Business.SystemDefaults.ShowCondensedProductInformationInSearchResult();
                        
                    if (pShowCondensedProductInformationInSearchResult) {
                        pDiv = $('<div>')
                            .css({
                                display: 'table-cell',
                                verticalAlign: 'middle',
                                width: '80px'
                            })
                            .append($('<img>')
                                .attr('id', 'sr_' + vDataRow.ID)
                                .css({
                                    'max-width': '80px',
                                    'max-height': '80px',
                                    'margin-right': '9px'
                                })
                                .attr('src', '../Content/images/box.png'));
                        vItemContent.append(pDiv);
                    }

                    if (!pShowCondensedProductInformationInSearchResult) {
                        if (CDec2(vDataRow.IsBrandOwner) != 0) {
                            pDiv = $('<div>')
                                .click(function () {
                                    MessageBox('SKUs with the "Official Item Record" icon are SKU records sourced and maintained by the official brand owner (or authorized agent). ' +
                                        'You get automatic updates to changes made by the brand owner such as new and/or modified item descriptions, pictures, SRP, packing sizes, dimensions, bar code numbers, and more. ' +
                                        'We strongly recommend that you use these official records in all of your transactions.',
                                        'Official Item Record');
                                    return false;
                                })
                                .css({
                                    display: 'table-cell',
                                    verticalAlign: 'middle',
                                    width: '80px'
                                })
                                .append($('<img>')
                                    .css({
                                        'max-width': '80px',
                                        'max-height': '80px',
                                        'margin-right': '9px'
                                    })
                                    .attr('src', '../Content/images/OfficialItem.gif')); //edited by janine. add ../ feb 11 2014
                            vItemContent.append(pDiv);
                        };
                    }

                    if (pShowCondensedProductInformationInSearchResult) 
                        pStr = '';
                    else
                        pStr = vDataRow.ProductNo + ' ';
                    pDiv = $('<div>')
                        .css({
                            display: 'table-cell',
                            verticalAlign: 'top'
                        })
                        .append($('<h3>')
                            .css('white-space', 'normal')
                            .text((e.Index + 1) + '.) ' + pStr + vDataRow.ProductName)
                        );

                    if (!pShowCondensedProductInformationInSearchResult) {
                        if (IsNumeric(vDataRow.DefaultPrice))
                            pDiv.append($('<p>')
                                .css('white-space', 'normal')
                                .text('Retail Price: ' + AccountingFormat(vDataRow.DefaultPrice))
                            );
                        pDiv.append($('<p>')
                                .css('white-space', 'normal')
                                .text('Published by: ' + vDataRow.PublishedBy + (CDec2(vDataRow.IsBrandOwner) != 0 ? ' (Brand Owner)' : ''))
                            ).append($('<p>')
                                .css('white-space', 'normal')
                                .text('Uploaded by: ' + vDataRow.UploadedBy)
                            );
                    }
                    vItemContent.append(pDiv);

                    vItem.append(vItemContent);

                } // OnItem: function(vDataRow, vItem) {

                , PageShow: function () {
                    Browser.LibraryPIMProduct.Page.data('CanRefreshStockPage', 1);
                    //                Browser.LibraryStockInformation.ShowRightButton()
                    //                    .click(function (e) {
                    //                        e.preventDefault();
                    //                        ChangePage('/customer');
                    //                    });

                    if (OfflineMode) {
                    } else {
                        Browser.LibraryPIMProduct.ShowSearchMore().unbind("click")
                            .click(function (e) {
                                e.preventDefault();
                                e.stopPropagation();

                                //Set search to public mode
                                Browser.LibraryPIMProduct.InternalInputData.IsSearchMore = 1;
                                Browser.LibraryPIMProduct.doSearch();
                                //                        //Restore to private mode, If want to make it private search after search more
                                //                        Browser.LibraryPIMProduct.InternalInputData.IsSearchMore = 0;
                            });
                    }; // if (!OfflineMode) {

                }
            }); // Browser.Init('LibraryCustomers', {

        }); // document ready
    }); // require offline
});  // define