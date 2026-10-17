define([''], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ComplianceReminders/dashboard/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ComplianceReminders.dashboard.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabComplianceRemindersDashboard').append(mElement).find('> .divspinner').remove();

        mOut.GetDashboardReminders = function () {
            AjaxSend({
                url: '/Alerts/GetDashboardReminders',
                success: function (e) {
                    var vRows = e.Results.Rows, cnt = 0;
                    var divCRLists = $('#divCRLists');
                    divCRLists.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var data = vRows[i];
                            cnt += CInt2(data.Cnt);
                            divCRLists.append($('<div class="col-xs-12 col-sm-6 col-md-3 mb-3">')
                                    .append($('<div class="dashboardGrid bg-light border p-3 rounded h-100">')
                                        .data('data', data)
                                        .append($('<div class="gHeader">').text(data.ComplianceOrg))
                                        .append($('<div class="gBody">').text(data.Cnt))
                                        .click(function () {
                                            var data = $(this).data('data');

                                            Bs.Loading('show');

                                            AjaxSend({
                                                url: '/Alerts/TagReminders',
                                                data: { RemindersID: data.IDs },
                                                success: function (e) {
                                                    Bs.Loading('hide');
                                                    BrowsePopup('/Alerts/GetComplianceReminders', 'Reminders for today', [
                                                        {
                                                            AutoSearch: true,
                                                            ModalSize: 'lg',
                                                            DisplayText: [{ Label: 'Compliance Organizaton', Column: 'ComplianceOrg' },
                                                            {
                                                                Label: 'Description', Column: function (e) {
                                                                    return CStr2(e.TaxType) + '' + CStr2(e.ComplianceTaskTile);
                                                                }
                                                            }, {
                                                                Label: 'Due Date Description', Column: function (e) {
                                                                    return CStr2(e.Description);
                                                                }
                                                            }],
                                                            ServerSideProcessing: true,
                                                            DontCloseWhenClick: true,
                                                            Click: function (e) {
                                                                var msg = '<b>Compliance Organization:</b> <br>' + (e.ComplianceOrg == 'Others' ? e.OtherComplianceOrg : e.ComplianceOrg) + '<br><br>';

                                                                if (e.ComplianceOrg == 'BIR') {
                                                                    msg += '<b>Tax Type:</b> <br>' + e.TaxType + '<br><br>';
                                                                    msg += '<b>Form Type:</b> <br>' + e.FormType + '<br><br>';
                                                                    msg += '<b>Filing Start Date:</b> <br>' + $.format.date(CJsonDate(e.FilingStartDate), 'MMMM dd, yyyy') + '<br><br>';
                                                                    msg += '<b>Filing Frequency:</b> <br>' + e.Frequency + '<br><br>';
                                                                    msg += '<b>Filing Due Date:</b> <br>' + e.Description + '<br><br>';
                                                                } else {
                                                                    msg += '<b>Title of Compliance Task:</b> <br>' + e.ComplianceTaskTile + '<br><br>';
                                                                    msg += '<b>Description:</b> <br>' + e.Description + '<br><br>';
                                                                    msg += '<b>Frequency:</b> <br>' + e.Frequency + '<br><br>';
                                                                }

                                                                NewMessageBox(msg, 'Details');
                                                            }, InitComplete: function () {
                                                                mOut.GetDashboardReminders();
                                                            }
                                                        }
                                                    ], { ids: data.IDs })
                                                }
                                            })                                            
                                        })
                                    )
                                )
                        }
                        $('#lblCRNotifCount').text(cnt);
                    } else {
                        $('#lblCRNotifCount').text('');
                        divCRLists.append($('<div class="col-12 mb-3">')
                            .append('<div class="text-center text-secondary mt-5"><span class="bi bi-bell-fill" style="font-size:80px;"></span><div class="mt-3">Empty</div></div>')
                        )
                    }
                }
            })
        }

        mOut.GetDashboardReminders();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});