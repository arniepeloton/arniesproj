define(['jquery'], function () {
    if(OfflineMode){
        window.Business = $.extend({}, window.Business);

        var pDeferred = $.Deferred();
        require([
                '/Scripts3/Offline/Business/SystemDefaults.js?crc=',
                '/Scripts3/Offline/Business/LibraryPIMProduct.js?crc=',
                '/Scripts3/Offline/Business/TransInvoice.js?crc=',
                '/Scripts3/Offline/Business/LibraryDevice.js?crc=',
                '/Scripts3/Offline/Business/LibraryDeviceFowst.js?crc=',
                '/Scripts3/Offline/Business/LibraryOrganizationCustomerFowstBinProduct.js?crc=',
                '/Scripts3/Offline/Business/LibraryOrganization.js?crc=',
                '/Scripts3/Offline/Business/LibraryPeople.js?crc=',
                '/Scripts3/Offline/Business/LibraryTableJoined.js?crc=',
                '/Scripts3/Offline/Business/TransBomMaker.js?crc=',
                '/Scripts3/Offline/Business/LibraryItemModifiers.js?crc=',
                '/Scripts3/Offline/Business/TransInventoryLedger.js?crc=',
                '/Scripts3/Offline/Business/LibraryOrganizationFOWST.js?crc=',
                '/Scripts3/Offline/Business/ZRead.js?crc=',
                '/Scripts3/Offline/Business/LibraryFowstBin.js?crc=',
                '/Scripts3/Offline/Business/LibraryTableTableInfo.js?crc=',
                '/Scripts3/Offline/Business/TransSalesReturn.js?crc='
         ], function (vTest) {
             pDeferred.resolve(vTest);
         });

        return pDeferred.promise();
    }
});