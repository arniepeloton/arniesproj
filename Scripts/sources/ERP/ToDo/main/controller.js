define([
    'promise!/Scripts/sources/ERP/Todo/dashboard/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/Todo/work/controller.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Todo/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Todo.main.controller = ERP.Todo.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.Todo.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpTodo').append(mElement).find('.divspinner').remove();        

        var TodoDashboard = new ERP.Todo.dashboard.controller();
        var TodoWork = new ERP.Todo.work.controller();

        $('#btnTodoDashboard').click(function () {
            TodoDashboard.GetDashboard();
        })

        $('#btnTodoSettings').click(function () {
            require(['promise!/Scripts/sources/ERP/Todo/settings/controller.js?v=' + assetversion], function () {

            })
        })

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});