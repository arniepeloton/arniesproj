define([
    '/Scripts/sources/ERP/PriceCanvassing/work/controller.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml, arrPriceCanvass;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceCanvassing/dashboard/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
        
    })();

    CreateStringObject('ERP.PriceCanvassing.dashboard.controller', null, function () {

        var mOut = {};
        var mElement, canvassDataForEdit, dtPriceCanvass;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabPriceCanvassDashboard').empty().append(mElement).trigger('create');        

        mElement.find('#btnComparePricesEdit').click(function () {
            PriceCanvass.DisplayCanvassDetails(canvassDataForEdit, false);
        })        

        function DisplayComparePrice(data) {
            var theadComparePricesVendor = $('#theadComparePricesVendor');
            var tbodyComparePrices = $('#tbodyComparePrices');
            var noOfVendor = data.Vendor.length;
            var vendorCol = '';
            theadComparePricesVendor.empty().append($('<th>').css('width', '300px').text('Item Description'));
            tbodyComparePrices.empty();

            $('[name="lblCanvassNo"]').text(data.No);
            $('[name="lblCanvassName"]').text(data.Name);
            $('[name="lblCanvassNoOfVendors"]').text(data.Vendor.length);
            $('[name="lblCanvassLastUpdated"]').text(data.LastUpdated);
            $('[name="lblCanvassModifiedBy"]').text(data.ModifiedBy);

            //Display header
            for (var i = 0 ; i < noOfVendor; i++) {
                theadComparePricesVendor.append($('<th>')
                        .css('width', '200px')
                        .css('text-align', 'center')
                        .text((CBool(data.Vendor[i].IsOrganization) ? data.Vendor[i].CompanyName : $.trim(CStr2(data.Vendor[i].FirstName) + ' ' + CStr2(data.Vendor[i].MiddleName) + ' ' + CStr2(data.Vendor[i].Surname))))
                    )
                vendorCol += '<td vendorid="' + data.Vendor[i].ID + '" style="text-align:center;"></td>';
            }

            for (var i = 0; i < data.Item.length; i++) {
                tbodyComparePrices.append($('<tr>').append($('<td>')
                                .text(data.Item[i].Description)
                            )
                            .append(vendorCol)
                        )

                if (data.Item[i].Sku.length > 0) {
                    for (var s = 0; s < data.Item[i].Sku.length; s++) {
                        tbodyComparePrices.append($('<tr>')
                                .attr('skuid', data.Item[i].Sku[s].ID)
                                .append($('<td>')
                                    .css('padding-left', '50px')
                                    .text(data.Item[i].Sku[s].ProductName)
                                )
                                .append(vendorCol)
                            )
                    }
                }
            }

            for (var i = 0 ; i < noOfVendor; i++) {
                for (s = 0; s < data.Vendor[i].SkuPrices.length; s++) {
                    $('[skuid="' + data.Vendor[i].SkuPrices[s].ID + '"]').find('[vendorid="' + data.Vendor[i].ID + '"]')
                        .text((CStr2(data.Vendor[i].SkuPrices[s].VendorPrice) == '' ? '' : AccountingFormat(data.Vendor[i].SkuPrices[s].VendorPrice)))
                }
            }

            $('#popComparePrices').show();
        }        

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});