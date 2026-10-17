define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {

    Browser.Init('LibraryOrganizationFOWSTSection', {
        // Url: optional; if omitted, follows vKey
        //  this is the name of the action. Controller URL is prepended internally

        Title: 'Section',
        AutoSearch: true,
        OnLink: function (vDataRow, vItem) {
            // PARAMS:
            //  vDataRow: object containing data. 
            //  vItem: $('<li>')

            vItem.append($('<h3>')
                    .attr("style", "white-space:normal")
                    .text(vDataRow.Code + ' - ' + vDataRow.Name)
                )
                //.append($('<p>')
                //    .attr("style", "white-space:normal")
                //    .text(vDataRow.Address)
                //);

        } // OnItem: function(vDataRow, vItem) {
//        , PageShow: function () {

//            Browser.LibraryFowst.ShowRightButton().unbind("click")
//                .click(function (e) {
//                    e.preventDefault();
//                    e.stopPropagation();

//                    ChangePage('/OrganizationShort/OrganizationAddress');
//                });

//        }
    }); // Browser.Init('Customer', {

    //if (OfflineMode) {
    //    var pDeferred = $.Deferred();
    //    require(['/Scripts3/Offline/Browser/LibraryFowst.js?crc='], function() {    
    //        pDeferred.resolve({});
    //    }); // require
    //    return pDeferred.promise();
    //}; // if (OfflineMode) {
});
