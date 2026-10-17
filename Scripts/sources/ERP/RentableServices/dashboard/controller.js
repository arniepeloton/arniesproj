define([
    //'promise!/Scripts/sources/ERP/AssetsManagement/dashboard/model.js'
    'moment'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/RentableServices/dashboard/view.html?v' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.RentableServices.dashboard.controller', null, function () {

        var mOut = {}, mElement, ctrChildren = 1, cntChildren = 0;
        var datefrom = $.format.date(new Date(), 'MMM dd, yyyy'), dateTo = datefrom;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabRentableServicesDashboard').empty().append(mElement);

        var txtSearch = mElement.find('[name="txtSearch"]');
        var btnDateReport = mElement.find('[name="btnDateReport"]');
        var btnRefreshList = mElement.find('[name="btnRefreshList"]');
        var tbodyPlayRoomReport = mElement.find('#tbodyPlayRoomReport');

        txtSearch.keydown(function (e) {
            if (e.which == 13) {
                mOut.GetPlayRoomReport();
            }
        })

        btnDateReport.text(datefrom).click(function () {
            PopupPeriod({
                Click: function (e) {
                    datefrom = e.From;
                    dateTo = e.To;

                    if (e.Period == '2 days ago' || e.Period == 'Yesterday' || e.Period == 'Today') {
                        btnDateReport.text(datefrom);
                    } else {
                        btnDateReport.text(datefrom + ' - ' + dateTo);
                    }

                    mOut.GetPlayRoomReport();
                }
            })
        })

        btnRefreshList.click(function () {
            mOut.GetPlayRoomReport();
        })

        mOut.GetPlayRoomReport = function () {
            tbodyPlayRoomReport.empty();
            AjaxSend({
                url: '/PlayRoom/GetReport',
                data: {
                    searchkey: txtSearch.val(),
                    reportDateFrom: datefrom,
                    reportDateTo: dateTo
                }, success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var pData = vRows[i];
                            tbodyPlayRoomReport.append($('<tr>')
                                .data('data', pData)
                                .append($('<td>').text((i + 1) + '.'))
                                .append($('<td>').text(pData.FullName))
                                .append($('<td>').text(pData.RepresentativeName))
                                .append($('<td class="text-center">').text($.format.date(new Date(pData.PlayDate), 'MM-dd-yyyy')))
                                .append($('<td class="text-center">').text($.format.date(new Date('1-1-2022 ' + pData.EnterTime), 'hh:mm a')))
                                .append($('<td class="text-center">').text($.format.date(new Date('1-1-2022 ' + pData.ExitTime), 'hh:mm a')))
                                .append($('<td class="text-center">').text((pData.TotalDuration == undefined ? 'ONGOING' : pData.TotalDuration)))
                                .append($('<td class="text-center">').text((pData.TotalDuration == undefined ? '' : pData.ActualTotalDuration)))
                                .click(function () {
                                    RentableServicesPlayRoomController.GetDetails($(this).data('data'));
                                    RentableServicesPlayRoomController.element.find('#btnPRSave').parent().hide();
                                })
                            )
                        }
                    } else {
                        tbodyPlayRoomReport.append($('<tr>').append('<td colspan="8" class="text-center">No results found</td>'))
                    }
                }
            })
        }

        mOut.Initialize = function () {
            mOut.GetPlayRoomReport();
        }

        //mOut.Initialize();
        
        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});