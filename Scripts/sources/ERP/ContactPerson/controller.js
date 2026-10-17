define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ContactPerson/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ContactPerson.controller', null, function () {

        var mOut = {};
        var mElement = $(mHtml);
        var mOrgID;

        $('body').append(mElement);//.trigger('create');
        //mElement.trigger('create'); // does not work properly
        var mTable = mElement.find('[name=mainTable]');

        mOut.Show = function(vParams) {
            vParams = $.extend({
                OrgID: null
            }, vParams);

            mOrgID = vParams.OrgID;
            NewMessageBoxClose();
            //GetData();
            mElement.show();
        };

        var mTxtFirstName = mElement.find('[name=txtFirstName]');
        var mTxtMiddleName = mElement.find('[name=txtMiddleName]');
        var mTxtLastName = mElement.find('[name=txtLastName]');

        mElement.find('[name=butSave]').click(function() {
            var pData = {
                ClientOrgID: mOrgID,
                FirstName: mTxtFirstName.val(),
                MiddleName: mTxtMiddleName.val(),
                LastName: mTxtLastName.val()
            };
            AjaxSend({
                url: 'Erp/SaveContactPerson',
                data: pData,
                success: function(vData) {
                    
                }
            });
            mElement.hide();
        });

        mElement.find('[name=butClose]').click(function() {
            mElement.hide();
        });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});