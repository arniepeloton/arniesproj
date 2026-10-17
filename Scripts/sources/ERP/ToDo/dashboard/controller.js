define([''
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Todo/dashboard/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.Todo.dashboard.controller', null, function () {

        var mOut = {}, mElement, createdBy = sessionStorage.PeopleID, assignedTo = sessionStorage.PeopleID;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabTodoDashboard').empty().append(mElement);

        var selTodoDashboardCreatedBy = mElement.find('[name="selTodoDashboardCreatedBy"]');
        var selTodoDashboardAssignedTo = mElement.find('[name="selTodoDashboardAssignedTo"]');
        var selTodoDashboardCategory = mElement.find('[name="selTodoDashboardCategory"]');
        var txtTodoDashboardDueDate = mElement.find('[name="txtTodoDashboardDueDate"]');
        var btnTodoDashboardDueDateClear = mElement.find('[name="btnTodoDashboardDueDateClear"]');
        var selTodoDashboardStatus = mElement.find('[name="selTodoDashboardStatus"]');
        var selTodoDashboardUrgency = mElement.find('[name="selTodoDashboardUrgency"]');
        var selTodoDashboardImportance = mElement.find('[name="selTodoDashboardImportance"]');
        var txtSearchTodoDashboard = mElement.find('[name="txtSearchTodoDashboard"]');
        var popupTodoDashoardDetails = mElement.find('[name="popupTodoDashoardDetails"]');
        var txtTodoDashoardRefNo = mElement.find('[name="txtTodoDashoardRefNo"]');
        var txtTodoDashoardCategory = mElement.find('[name="txtTodoDashoardCategory"]');
        var txtTodoDashoardTitle = mElement.find('[name="txtTodoDashoardTitle"]');
        var txtTodoDashoardDescription = mElement.find('[name="txtTodoDashoardDescription"]');
        var txtTodoDashoardOrganization = mElement.find('[name="txtTodoDashoardOrganization"]');
        var txtTodoDashoardDueDate = mElement.find('[name="txtTodoDashoardDueDate"]');
        var txtTodoDashoardUrgency = mElement.find('[name="txtTodoDashoardUrgency"]');
        var txtTodoDashoardImportance = mElement.find('[name="txtTodoDashoardImportance"]');
        var txtTodoDashoardAssignedPersons = mElement.find('[name="txtTodoDashoardAssignedPersons"]');
        var btnTodoDashboardMarkAsUndone = mElement.find('[name="btnTodoDashboardMarkAsUndone"]');
        var btnTodoDashboardMarkAsDone = mElement.find('[name="btnTodoDashboardMarkAsDone"]');

        selTodoDashboardCreatedBy.click(function () {
            BrowseEmployee(true);
        })

        selTodoDashboardAssignedTo.click(function () {
            BrowseEmployee(false);
        })

        selTodoDashboardCategory.change(function () {
            mOut.GetDashboard();
        })

        txtTodoDashboardDueDate.click(function () {
            DualCalendar({
                Format: 'MM-dd-yyyy',
                Click: function (e) {
                    txtTodoDashboardDueDate.val(e.From + ' to ' + e.To);
                    mOut.GetDashboard();
                }
            })
        })

        btnTodoDashboardDueDateClear.click(function () {
            txtTodoDashboardDueDate.val('');
            mOut.GetDashboard();
        })

        selTodoDashboardStatus.change(function () {
            mOut.GetDashboard();
        })

        selTodoDashboardUrgency.change(function () {
            mOut.GetDashboard();
        })

        selTodoDashboardImportance.change(function () {
            mOut.GetDashboard();
        })

        txtSearchTodoDashboard.keydown(function (e) {
            if (e.which == 13) {
                mOut.GetDashboard();
            }            
        })

        function BrowseEmployee(isCreatedBy) {
            BrowsePopup('LibraryOrganizationEmployee', (isCreatedBy ? 'Created By' : 'Assigned To'), [
                {
                    DisplayText: [{ Label: 'Employee Name', Column: 'PeopleName' }],
                    AllowToSelectAll: true,
                    StyleFormat: 'table',
                    AutoSearch: true,
                    Click: function (e) {
                        if ($.isArray(e)) {
                            if (isCreatedBy) {
                                createdBy = 'all';
                                selTodoDashboardCreatedBy.val('All');
                            } else {
                                assignedTo = 'all';
                                selTodoDashboardAssignedTo.val('All');
                            }
                        } else {
                            if (isCreatedBy) {
                                createdBy = e.ID;
                                selTodoDashboardCreatedBy.val(e.PeopleName);
                            } else {
                                assignedTo = e.ID;
                                selTodoDashboardAssignedTo.val(e.PeopleName);
                            }
                        }

                        mOut.GetDashboard();
                    }
                }
            ])
        }

        var tblTodoDashboard;
        mOut.GetDashboard = function (type) {
            var tbl = '[name="tblTodoDashboard"]';
            //$('#tbodyETMakers').empty()

            if ($.fn.DataTable.isDataTable(tbl)) {
                //check if datatable is created 
                $(tbl).DataTable().clear().destroy();
            }

            tblTodoDashboard = $(tbl).DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 10,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        CreatedBy: createdBy,
                        AssignedTo: assignedTo,
                        Category: selTodoDashboardCategory.val(),
                        DueDate: txtTodoDashboardDueDate.val(),
                        Status: selTodoDashboardStatus.val(),
                        Urgency: selTodoDashboardUrgency.val(),
                        Importance: selTodoDashboardImportance.val(),
                        keywords: txtSearchTodoDashboard.val().trim()
                    }, '/CRMDashboard/TodoDashboard'),
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
                            return (CStr2(data.TodoNumber) == '' ? '' : 'TD-' + data.TodoNumber);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<div style="display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; max-height: 100px; overflow: hidden; text-overflow: ellipsis;">' + data.OrgName + '</div>';;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.CategoryName || 'Uncategorized');
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<div style="display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; max-height: 100px; overflow: hidden; text-overflow: ellipsis;">' + data.ToDoTitle + '</div>';
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<div style="display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; max-height: 100px; overflow: hidden; text-overflow: ellipsis;">'+ data.ToDo +'</div>';
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return data.CreatedByName + '<br>on ' + $.format.date(CJsonDate(data.CreatedOn), 'MM-dd-yyyy hh:mm a');
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<div style="display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; max-height: 100px; overflow: hidden; text-overflow: ellipsis;">' + CStr2(data.AssignedPersons) + '</div>';
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CBool(data.ToDoFinish) ? 'Marked as Done' : 'Marked as Undone' + (CStr2(data.ToDoDueDate) == '' ? '' : '<br>Due on ' +  $.format.date(CJsonDate(data.ToDoDueDate), 'MM-dd-yyyy'));
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return (CStr2(data.Urgency || 'NA') == 'NA' ? '' : '- ' + data.Urgency + '<br>') + '  ' + (CStr2(data.Importance || 'NA') == 'NA' ? '' : '- ' + data.Importance);
                        }, className: 'text-center'
                    },
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [4] }],
                "initComplete": function () {
                    $(tbl).removeClass('dataTable');
                },
                "responsive": true,
                "columnDefs": [{ "targets": 4, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                rowCallback: function (tr, data) {
                    $(tr)
                        .data('data', data)
                        .click(function (e) {
                            e.stopPropagation();
                            e.preventDefault();
                            var data = $(this).data('data');

                            function MarkTodo(mark) {
                                popupTodoDashoardDetails.modal('hide');

                                AjaxSend({
                                    url: '/CRMDashboard/MarkTodo',
                                    data: { TodoID: data.ID, Mark: mark },
                                    success: function (e) {
                                        if (e.Results != undefined) {
                                            NewMessageBox('To-Do number <b>' + data.TodoNumber + '</b> has been marked as ' + (CBool(mark) ? 'done' : 'undone') + '. Thank you!');
                                            mOut.GetDashboard();
                                        } else {
                                            NewMessageBox(e.Error, 'Error');
                                        }
                                    }
                                })
                            }

                            popupTodoDashoardDetails.find('input, textarea').val();

                            txtTodoDashoardRefNo.val((CStr2(data.TodoNumber) == '' ? '' : 'TD-' + data.TodoNumber));
                            txtTodoDashoardCategory.val(CStr2(data.CategoryName || 'Uncategorized'));
                            txtTodoDashoardTitle.val(data.ToDoTitle);
                            txtTodoDashoardDescription.val(data.ToDo);
                            txtTodoDashoardOrganization.val(data.OrgName);
                            txtTodoDashoardDueDate.val($.format.date(CJsonDate(data.ToDoDueDate), 'MM-dd-yyyy'));
                            txtTodoDashoardUrgency.val(data.Urgency);
                            txtTodoDashoardImportance.val(data.Importance);
                            txtTodoDashoardAssignedPersons.val(data.AssignedPersons);

                            btnTodoDashboardMarkAsUndone.unbind('click').click(function () {
                                btnTodoDashboardMarkAsUndone.unbind('click');
                                MarkTodo(0);
                            })

                            btnTodoDashboardMarkAsDone.unbind('click').click(function () {
                                btnTodoDashboardMarkAsDone.unbind('click');
                                MarkTodo(1);
                            })
                            
                            popupTodoDashoardDetails.modal('show');
                        })
                }
                //"stateSave": true
            })
        }

        mOut.Initialize = function () {
            selTodoDashboardCreatedBy.val(sessionStorage.CurrentUserFullName);
            selTodoDashboardAssignedTo.val(sessionStorage.CurrentUserFullName);

            AjaxSend({
                url: '/Browser/LibraryCRMTodoCategories',
                success: function (e) {
                    vRows = e.Results.Rows;
                    selTodoDashboardCategory.empty();
                    selTodoDashboardCategory.append(new Option('All', ''));
                    selTodoDashboardCategory.append(new Option('Uncategorized', 'uncategorized'));

                    for (var i in vRows) {
                        selTodoDashboardCategory.append(new Option(vRows[i].CategoryName, vRows[i].ID));
                    }

                    mOut.GetDashboard();
                }
            })
        }

        mOut.Initialize();
        
        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});