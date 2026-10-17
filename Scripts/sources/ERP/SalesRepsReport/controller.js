define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/SalesRepsReport/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.SalesRepsReport.controller = ERP.SalesRepsReport.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.SalesRepsReport.controller', null, function () {

        var mOut = {};
        var mElement;

        var mInvoiceDateFrom = new Date();
        var mInvoiceDateTo = new Date();

        mElement = $(mHtml);
        $.mobile.activePage.append(mElement);

        mOut.popup = function() {
            mElement.modal('show');
        };

        mElement.find('[name=butDateRange]')
            .text(FormatDate(new Date(), {
                    Format: 'MMM dd, yyyy'
                }) + ' to ' +
                FormatDate(new Date(), {
                    Format: 'MMM dd, yyyy'
                }))
            .click(function () {
                DualCalendar({
                    Click: function (e) {
                        mElement.find('[name=butDateRange]').text(FormatDate(e.From, {
                                Format: 'MMM dd, yyyy'
                            }) + ' to ' +
                            FormatDate(e.To, {
                                Format: 'MMM dd, yyyy'
                            }));
                        mInvoiceDateFrom = e.From;
                        mInvoiceDateTo = e.To;
                    }
                }, new Date(), new Date());    
            }
        );

        mElement.find('[name=butView]').click(function() {
            LoadData();
        });

        function LoadData() {
            var pData = {
                InvoiceDateFrom: mInvoiceDateFrom,
                InvoiceDateTo: mInvoiceDateTo,
            };

            AjaxSend({
                url: '/DashboardsAndReports/GetSalesBySalesRep',
                data: pData,
                success: function(vData) {
                    ViewReport(vData.BaseFilename);
                    if (vData.BaseFilename) {
                        NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                    }
                }
            });

            function ViewReport(vFilename) {
                var pUrl = MergeUrlParameters({
                    Filename: vFilename
                }, '/ERP/DownloadPrintablePdf');
                window.open(pUrl, '_blank', 
                    'width=' + window.innerWidth + 
                    ',height=' + window.innerHeight + 
                    ',scrollbars=auto');
            } // END function ViewReport() {
        }
        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});