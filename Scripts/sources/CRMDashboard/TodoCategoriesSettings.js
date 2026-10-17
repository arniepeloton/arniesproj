define(['']
    , function () {
        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/CRMDashboard/TodoCategoriesSettings?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    ERP.TodoCategories.main.controller = ERP.TodoCategories.main.controller()
                    mDeferred.resolve();
                }
            });
        })();

        CreateStringObject('ERP.TodoCategories.main.controller', null, function () {

            var mOut = {}, data;
            var mElement, isAddOnly = false, module = '', id = '';

            mElement = $(mHtml);
            mOut.element = mElement;

            $('#tabTodoCategories').append(mElement);

            var txtTodoCategoryName = mElement.find('[name="txtTodoCategoryName"]');

            mOut.Initialize = function (vModule) {
                module = CStr2(vModule);
                mOut.GetTodoCategories();
            }

            mElement.find('[name="btnAddTodoCategory"]').click(function () {
                id = '';
                txtTodoCategoryName.val('');
                $('#popupTodoCategory').modal('show');
            })

            mElement.find('[name="btnSaveTodoCategory"]').click(function () {
                txtTodoCategoryName = mElement.find('[name="txtTodoCategoryName"]').val().trim();

                if (txtTodoCategoryName == '') {
                    WebToast('Please enter category name', 'top');
                    return
                }

                Bs.Loading('show');

                AjaxSend({
                    url: '/CRMDashboard/SaveTodoCategory',
                    data: { CategoryID: id, CategoryName: txtTodoCategoryName, Module: module },
                    success: function (e) {
                        Bs.Loading('hide');

                        if (e.Result == 'saved') {
                            WebToast('Successfully saved', 'top');
                            $('#popupTodoCategory').modal('hide');
                            mOut.GetTodoCategories();
                        } else {
                            NewMessageBox(e.Error, 'Error');
                        }
                    }
                })                
            })

            mOut.GetTodoCategories = function () {
                $('#tblTodoCategories tbody').empty()

                if ($.fn.DataTable.isDataTable('#tblTodoCategories')) {
                    //check if datatable is created 
                    $('#tblTodoCategories').DataTable().clear().destroy();
                }

                var tblTodoCategories = $('#tblTodoCategories').DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "pageLength": 20,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/CRMDashboard/GetTodoCategories?OrgID=" + sessionStorage.OrgID + "&module=" + module,
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum + '.';
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.CategoryName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.LastUpdatedBy + ' on ' + $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MM-dd-yyyy hh:mm a');
                            }
                        },
                        {
                            "data": null,
                            createdCell: function (td, cellData, rowData, row, col) {
                                var btn = $('<button data-role="none" class="btn btn-danger">')
                                            .append('<span class="bi-trash"></span>')
                                            .data('data', rowData)
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                var data = $(this).data('data');

                                            })

                                $(td).html(btn)
                            }
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                    "initComplete": function () {
                        $('#tblTodoCategories').removeClass('dataTable');
                    },
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })

                $('#tblTodoCategories tbody').unbind('click').on('click', 'tr', function () {
                    var data = tblTodoCategories.row(this).data();
                    if (CStr2(data) != '') {
                        id = data.ID;
                        mElement.find('[name="txtTodoCategoryName"]').val(data.CategoryName);
                        $('#popupTodoCategory').modal('show');
                    }
                });
            }

            return mOut;

        }); // END CreateStringObject

        return mDeferred.promise();
    })