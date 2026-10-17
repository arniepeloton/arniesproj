define([], function () {
    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/RemarksMessages/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.RemarksMessages.main.controller = ERP.RemarksMessages.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.RemarksMessages.main.controller', null, function () {

        var mOut = {}, data, id = '';
        var mElement, isAddOnly = false;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('body').append(mElement)
        .on('click', '[name="btnAddRemarks"]', function () {
            var popup = $('[name="popQuickRemarks"]');
            id = '';
            popup.find('textarea, select').val('');
            popup.find('[name="chbIsInternalRemarks"]').prop('checked', $('[name="chbShowInternalRemarks"]').is(':checked'));
            popup.modal('show');
        })
        .on('click', '[name="btnQuickRemarksSave"]', function () {
            var txtRemarksMessage = $('[name="txtRemarksMessage"]').val();
            var selRemarksTag = $('[name="selRemarksTag"]').val();

            if (txtRemarksMessage == '') {
                WebToast('Please enter message', 'top');
                return;
            }

            if (selRemarksTag == '') {
                WebToast('Please select tag', 'top');
                return;
            }
            Bs.Loading('show');
            AjaxSend({
                url: '/ERP/SaveQuickRemarks',
                data: {
                    ID: id,
                    Message: txtRemarksMessage,
                    Tag: selRemarksTag,
                    IsInternalRemarks: $('[name="chbIsInternalRemarks"]').is(':checked')
                }, success: function (e) {
                    Bs.Loading('hide');
                    if (e.Results == 'saved') {
                        WebToast('Successfully saved', 'top');
                        GetQuickRemarks();
                        $('[name="popQuickRemarks"]').modal('hide');
                    } else {
                        NewMessageBox(e.Error, 'Message');
                    }
                }
            })
        })
        .on('change', '[name="selFilterQuickRemarks"]', function () {
            GetQuickRemarks();
        })
        .on('change', '[name="chbShowInternalRemarks"]', function () {
            if ($(this).is(':checked')) {
                $('[name="btnAddRemarks"]').text('Add Internal Remarks');
            } else {
                $('[name="btnAddRemarks"]').text('Add Remarks');
            }

            GetQuickRemarks();
        })

        function GetQuickRemarks() {
            var tbodyRemarksMessages = $('#tbodyRemarksMessages');

            tbodyRemarksMessages.empty();

            if ($.fn.DataTable.isDataTable("#tblQuickRemarks")) {
                //check if datatable is created 
                $('#tblQuickRemarks').DataTable().clear().destroy();
            }

            var tblQuickRemarks = $('#tblQuickRemarks').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                //"order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        Show: $('[name="selFilterQuickRemarks"]').val(),
                        IsInternalRemarks: $('[name="chbShowInternalRemarks"]').is(':checked'),
                        keywords: ''
                    }, '/ERP/GetQuickRemarks'),
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": [
                    {
                        title: '#',
                        "data": function (data, type, full, meta) {
                            return data.RowNum + '.';
                        },
                        "width": "50px"
                    },
                    {
                        title: 'Message',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.Message);
                        }
                    },
                    {
                        title: 'Tag',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.Tag);
                        }, "width": "150px"
                    },
                    {
                        title: 'Updated By',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.UpdatedByName);
                        }, "width": "150px"
                    },
                    {
                        title: 'Last Updated',
                        "data": function (data, type, full, meta) {
                            return $.format.date(CJsonDate(data.LastUpdated), 'MM-dd-yyyy hh:mm a');
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:center');
                        }, "width": "180px"
                    },
                    {
                        title: '',
                        "data": function (data, type, full, meta) {
                            return '<button data-role="none" class="btn btn-danger"><span class="bi bi-trash"></span></button>';
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).data('data', rowData).attr('style', 'text-align:center')
                                .click(function (e) {
                                    e.stopPropagation();
                                    var data = $(this).data('data');
                                    NewMessageBox('Are you sure you want to delete this remarks? <br><br> If Yes, it cannot be undone.', 'Confirmation Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                Bs.Loading('show');

                                                AjaxSend({
                                                    url: '/ERP/DeleteQuickRemarks',
                                                    data: { RemarksID: data.ID },
                                                    success: function (e) {
                                                        Bs.Loading('hide');

                                                        if (e.Results != undefined) {
                                                            WebToast('Selected remarks has been deleted', 'top');
                                                            GetQuickRemarks();
                                                        } else {
                                                            NewMessageBox(e.Error, 'Error');
                                                        }
                                                    }
                                                })
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                        }, "width": "60px"
                    }
                ],
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = $(this).data('data');
                        $('[name="btnAddRemarks"]').click();
                        id = data.ID;
                        $('[name="txtRemarksMessage"]').val(data.Message);
                        $('[name="selRemarksTag"]').val(data.Tag);
                        $('[name="chbIsInternalRemarks"]').prop('checked', CBool(data.IsInternalRemarks));
                    })
                },
                "initComplete": function () {
                    $('#tblQuickRemarks').removeClass('dataTable');
                    $('#tblQuickRemarks thead tr').addClass('align-top text-center').css('font-size', '14px')
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        mOut.show = function (vParams) {
            if (vParams.module == 'disbursement') {
                $('#divRemarksMessage').appendTo('#tabDSRemarksMessages');
            } else if (vParams.module == 'revenue') {
                $('#divRemarksMessage').appendTo('#tabRSRemarksMessages');
            }

            GetQuickRemarks();
        }

        return mOut;

    }); // END CreateStringObject

    return mDeferred.promise();
})