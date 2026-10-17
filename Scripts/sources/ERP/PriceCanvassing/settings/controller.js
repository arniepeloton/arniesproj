define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceCanvassing/settings/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.PriceCanvassing.settings.controller', null, function () {

        var mOut = {};

        mElement = $('body');
        mOut.element = mElement;

        $('#tabPriceCanvassSettings').empty().append(mHtml);
        setTimeout(function () {
            AttachEvents();
            GetUserToLinkSKU();
        }, 500)

        mOut.LoadPage = function () {
            $('#tabPriceCanvassSettings').empty().append(mHtml);

            mElement = $('body');

            mElement.find('#tabPriceCanvassSettings *').undelegate();
            AttachEvents();
        }

        //LoadRequiredData();

        function AttachEvents() {
            mOut.element = mElement;
            
            mElement.off('click', '#btnAddUserForLinkingSKU').on('click', '#btnAddUserForLinkingSKU', function () {
                BrowsePopup('LibraryOrganizationEmployee', 'Select User', [
                    {
                        DisplayText: [{ Column: 'PeopleName' }],
                        AutoSearch: true,
                        SelectMultiple: true,
                        Click: function (e) {
                            AjaxSend({
                                url: 'Canvass/SaveUserToLinkSKU',
                                data: { data: e },
                                success: function (e) {
                                    var res = e.Results;

                                    if (res == 'saved') {
                                        WebToast('Successfully saved', 'top');
                                        GetUserToLinkSKU();
                                    } else {
                                        NewMessageBox(res, 'Message');
                                    }
                                }
                            })
                        }
                    }
                ])
            })
        }

        function GetUserToLinkSKU() {
            var tbodyUserForLinkingSKU = $('#tbodyUserForLinkingSKU');

            tbodyUserForLinkingSKU.empty();

            AjaxSend({
                url: 'Canvass/GetUserToLinkSKU',
                success: function (e) {
                    var res = e.Results;

                    if ($.isArray(res) || IsObject(res)) {
                        var vRows = res.Rows;

                        if(vRows.length > 0){
                            for (var i = 0; i < vRows.length; i++) {
                                tbodyUserForLinkingSKU.append($('<tr scope="row">')
                                        .append($('<td style="width:20px;">').text((i + 1) + '.'))
                                        .append($('<td>').text(vRows[i].EmployeeUser))
                                        .append($('<td style="width:150px;">').text(vRows[i].ModifiedBy))
                                        .append($('<td style="width:170px;">').text($.format.date(new Date(vRows[i].LastUpdated), 'MMM dd, yyyy hh:mm a')))
                                        .append($('<td style="width:20px; text-align:center;">').append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                            .data('data', vRows[i])
                                            .append($('<span class="glyphicon glyphicon-remove">'))
                                            .click(function (e) {
                                                var data = $(this).data('data');

                                                NewMessageBox('Are you sure you want to remove user <b>' + data.EmployeeUser + '</b>?', 'Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            AjaxSend({
                                                                url: 'Canvass/DeleteUserToLinkSKU',
                                                                data: { ID: data.ID },
                                                                success: function (e) {
                                                                    if(e.Results == 'success'){
                                                                        WebToast('User has been successfully removed', 'top');
                                                                        GetUserToLinkSKU();
                                                                    } else {
                                                                        NewMessageBox(e.Results, 'Message');
                                                                    }
                                                                }
                                                            })
                                                        }
                                                    }, { Text: 'No' }
                                                ])
                                            })
                                        ))
                                    )
                            }
                        } else {
                            tbodyUserForLinkingSKU.append($('<tr scope="row">')
                                        .append($('<td colspan="5" style="text-align:center;">').text('No result(s) found'))
                                    )
                        }
                    }
                }
            })
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});