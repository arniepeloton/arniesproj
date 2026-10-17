define(['']
    , function () {
        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/Tickets/popuprai/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    ReportAnIssue.popup = ReportAnIssue.popup()
                    mDeferred.resolve();
                }
            });
        })();

        CreateStringObject('ReportAnIssue.popup', null, function () {

            var mOut = {}, data;
            var mElement, isAddOnly = false;

            mElement = $(mHtml);
            mOut.element = mElement;            

            $('body').append(mElement);

            mElement.find('#selRAIStatus').change(function () {
                GetRAI();
            })

            mElement.find('#txtSearchRAI').keydown(function (e) {
                if (e.which == 13) {
                    GetRAI();
                }
            })

            mOut.show = function (element) {
                if (element.data('IDs') != null) {
                    AjaxSend({
                        url: '/Ticket/TagAsNotified',
                        data: { IDs: element.data('IDs') },
                        success: function (e) {
                            //do nothing
                        }
                    })
                }

                GetRAI();
                mElement.find('#popupRAIMain').modal('show');
            }

            function GetRAI() {
                mElement.find('tbody').empty();
                var tbl = mElement.find('table');

                if ($.fn.DataTable.isDataTable(tbl)) {
                    //check if datatable is created 
                    tbl.DataTable().clear().destroy();
                }

                tbl.DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 10,
                    //"order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            Status: $('#selRAIStatus').val(),
                            keywords: $.trim($('#txtSearchRAI').val())
                        }, '/Ticket/GetRAI'),
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            title: '#',
                            "data": function (data, type, full, meta) {
                                return data.RowNum + '.';
                            },
                            "width": "30px"
                        },
                        {
                            title: 'Ticket No.',
                            "data": function (data, type, full, meta) {
                                return data.TicketNo;
                            }
                        },
                        {
                            title: 'Customer Name',
                            "data": function (data, type, full, meta) {
                                return data.CustomerName;
                            }
                        },
                        {
                            title: 'FOWST',
                            "data": function (data, type, full, meta) {
                                return data.FowstName;
                            }
                        },
                        {
                            title: 'Reported By',
                            "data": function (data, type, full, meta) {
                                return data.ReportedByName;
                            }
                        },
                        {
                            title: 'Subject',
                            "data": function (data, type, full, meta) {
                                return data.Subject;
                            }
                        },
                        {
                            title: 'Urgency',
                            "data": function (data, type, full, meta) {
                                return data.Urgency;
                            }
                        },
                        {
                            title: 'Importance',
                            "data": function (data, type, full, meta) {
                                return data.Importance;
                            }
                        },
                        {
                            title: 'Status',
                            "data": function (data, type, full, meta) {
                                return data.Status;
                            }
                        },
                        {
                            title: 'Date Reported',
                            "data": function (data, type, full, meta) {
                                return $.format.date(CJsonDate(data.DateReported), 'MM-dd-yy hh:mm a');
                            }, className: 'text-center'
                        },
                    ],
                    "createdRow": function (row, data, index) {
                        $(row).data('data', data).click(function () {
                            var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                            var btnRAIUpdate = mElement.find('#btnRAIUpdate');                            
                            var lblRAIDescription = mElement.find('#lblRAIDescription');
                            var selRAIDetailsStatus = mElement.find('#selRAIDetailsStatus');
                            var lblStatus = mElement.find('#lblStatus');
                            
                            mElement.find('#lblRAIDate').val($.format.date(CJsonDate(data.DateReported), 'MM-dd-yy hh:mm a'));
                            mElement.find('#lblRAICustomerName').val(data.CustomerName);
                            mElement.find('#lblRAIFowstName').val(data.FowstName);
                            mElement.find('#lblRAIProduct').val(data.Product);
                            mElement.find('#lblRAISubject').val(data.Subject);
                            mElement.find('#lblRAIUrgency').val(data.Urgency);
                            mElement.find('#lblRAIImportance').val(data.Importance);
                            lblRAIDescription.css('height', '0px').val(data.Description);
                            selRAIDetailsStatus.removeAttr('disabled').val(data.Status);
                            lblStatus.text('');
                            btnRAIUpdate.removeAttr('disabled').unbind('click');

                            mElement.find('#popupRAIDetails').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                                lblRAIDescription.css('height', (lblRAIDescription[0].scrollHeight + 2) + 'px');
                            });                            

                            if (data.Status != 'Unresolved') {
                                selRAIDetailsStatus.attr('disabled', 'disabled');
                                btnRAIUpdate.attr('disabled', 'disabled');
                                lblStatus.text('Last updated by ' + data.UpdatedByName + ' on ' + $.format.date(CJsonDate(data.LastUpdated), 'MM-dd-yy hh:mm a'));
                                return;
                            }

                            btnRAIUpdate.click(function () {
                                Bs.Loading('show');
                                AjaxSend({
                                    url: '/Ticket/UpdateRAI',
                                    data: {
                                        TransID: data.ID,
                                        Status: mElement.find('#selRAIDetailsStatus').val(),
                                        UpdatedBy: sessionStorage.PeopleID
                                    }, success: function (e) {
                                        Bs.Loading('hide');
                                        mElement.find('#popupRAIDetails').modal('hide');
                                        if (e.Results != undefined) {
                                            GetRAI();
                                            WebToast('Ticket No. ' + data.TicketNo + ' has been tagged as ' + data.Status);
                                        } else {
                                            NewMessageBox(e.Error, 'Error');
                                        }
                                    }
                                })
                            })
                        })
                    },
                    "initComplete": function () {
                        tbl.removeClass('dataTable');
                        tbl.find('thead tr').addClass('align-top text-center').css('font-size', '14px')
                    },
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                    "responsive": true,
                    //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })

                mOut.CountNewReports();
            }

            mOut.CountNewReports = function () {
                AjaxSend({
                    url: '/Ticket/CountNewReports',
                    success: function (e) {
                        if (CInt2(e.Count) > 0) {
                            $('#lblCountNewReports').text(e.Count);
                            $('#lblCountNewReports').parent().data('IDs', e.IDs);
                        } else {
                            $('#lblCountNewReports').text('');
                            $('#lblCountNewReports').parent().data('IDs', null);
                        }
                    }
                })
            }
            
            return mOut;

        }); // END CreateStringObject

        return mDeferred.promise();
    })