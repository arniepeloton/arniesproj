define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryOrganizationSupplier.js?crc=' : '')], function() {    
        $(function () {

            Browser.Init('LibraryOrganizationSupplier', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally
                AutoSearch: true,
                Url: 'LibraryOrganization',
                InputData: {
                    FilterSupplier: true
                },
                OnItem: function (vDataRow, vItem) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')

                    var pCompanyName = vDataRow.CompanyName;
                    if (CStr2(vDataRow.FowstName).trim() != '')
                        pCompanyName = vDataRow.FowstName + ' - ' + pCompanyName;

                    vItem.append($('<a>')
                        .attr('href', 'LibraryOrganizationSupplier?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(pCompanyName)
                        )
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.LibraryOrganizationSupplier.Click))
                                Browser.LibraryOrganizationSupplier.Click($(this).data('data'));
                            history.back();

                            return false;
                        })
                    );
                } // OnItem: function(vDataRow, vItem) {

                //, PageShow: function () {

                //    Browser.LibraryOrganization.Page.data('CanRefreshStockPage', 1);

                //    if (CBool(sessionStorage.IsNewCustomer)) {
                //        var data = [];

                //        data = { IsNew: true };

                //        Browser.LibraryOrganization.ShowRightButton().unbind("click")
                //            .data('data', data)
                //            .click(function (e) {
                //                e.preventDefault();
                                                                
                //                if ($.isFunction(Browser.LibraryOrganization.Click))
                //                    Browser.LibraryOrganization.Click($(this).data('data'));
                //                history.back();

                //                return false;

                //                //e.stopPropagation();
                                
                //                //var pData = {};
                //                //pData.IsNew = 0;
                //                //pData.ID = '';
                        
                //                //AjaxSend({
                //                //    url: 'OrganizationShort/setCreateNew',
                //                //    data: pData,
                //                //    success: function (vData) {

                //                //        window.IsPageLoaded = false;
                //                //        ChangePage('/OrganizationShort/OrganizationInfo');

                //                //    }
                //                //});

                //        });
                //    }
                        

                //    if (!OfflineMode) {
                //        Browser.LibraryOrganization.ShowSearchMore().unbind("click")
                //            .click(function (e) {
                //                e.preventDefault();
                //                e.stopPropagation();

                //                //Set search to public mode
                //                Browser.LibraryOrganization.InternalInputData.FilterCustomers = 0;
                //                Browser.LibraryOrganization.doSearch();
                //                //                        //Restore to private mode, If want to make it private search after search more
                //                //                        Browser.LibraryPIMProduct.InternalInputData.IsSearchMore = 0;
                //            });
                //    }; // if (!OfflineMode) {

                //}
            }); // Browser.Init('Customer', {

        }); // $(function () {
    }); // require
}); // define
