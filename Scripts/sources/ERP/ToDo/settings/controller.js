define([''],
function () {
    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Todo/settings/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Todo.settings.controller = ERP.Todo.settings.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.Todo.settings.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabTodoSettings').append(mElement).find('.divspinner').remove();

        mOut.Initialize = function () {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/CRMDashboard/TodoCategoriesSettings.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.TodoCategories.main.controller.Initialize();
            })
        }

        mOut.Initialize();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();
})