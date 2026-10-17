require(['common'], function () {
    $(document).ready(function () {
        var htmlStoreReplenishment, htmlSalesOrderProcessing;
        $('body')
        .on('click', '#btnTabRevStoreReplenishment', function () {
            CallPage('#tabRevStoreReplenishment', htmlStoreReplenishment, '/DashboardsAndReports/Franchise', function (e) {
                htmlStoreReplenishment = e;
            })
        })
        .on('click', '#btnTabRevSaleOrder', function () {
            CallPage('#tabRevSaleOrder', htmlSalesOrderProcessing, '/DashboardsAndReports/SalesOrder', function (e) {
                htmlSalesOrderProcessing = e;
            })
        })

        $('#tabRevStoreReplenishment').opentabmenu();
    })
})