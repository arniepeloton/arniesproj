define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/uploadPo/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Invoicing.uploadPo.controller = ERP.Invoicing.uploadPo.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Invoicing.uploadPo.controller', null, function () {

        var mOut = {};
        var mElement;
        var popupid = NewID();

        mElement = $('<div>').append($.parseHTML(mHtml));
        mElement.find('.modal:first-child').attr('id', 'popUploadPo' + popupid);
        $('body').append(mElement);
        mOut.element = mElement;

        mOut.popup = function(vParams) {
            mOut.AfterSave = vParams.AfterSave;
            mElement.find('[type="file"]').val('');
            $('#popUploadPo' + popupid).modal('show');
        };

        mElement.find('form').submit(function(e) {
            var p;

            e.preventDefault();
            var formData = new FormData($(this)[0]);
            formData.append('OrgID', sessionStorage.OrgID);

            $.ajax({
                url: '/ERP/ImportPoXml',
                type: 'POST',
                data: formData,
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    try {
                        vData = JSON.parse(vData);
                        if (vData.d)
                            vData = vData.d;
                    } catch (e) {
                        NewMessageBox('An error occured while parsing the server response to JSON: ' + e.message);
                        return;
                    }
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Warning)
                        NewMessageBox(vData.Warning);
                    else if (vData.Result == 'ok') {
                        WebToast('The data was successfully saved.', 'top');
                        $('#popUploadPo' + popupid).modal('hide');
                        mOut.AfterSave(vData.ID);
                    }                        
                    else
                        NewMessageBox('Unknown status. ref jsHCTRGFCAGZ!');
                }
            });
        });

        mOut.Init = function() {
        };

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});