define([
    //'promise!/Scripts/sources/ERP/AssetsManagement/dashboard/model.js'
    'moment'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Todo/work/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.Todo.work.controller', null, function () {

        var mOut = {}, mElement, todoData = null, TodoID = null, OrgID = null, IsOrg = null, arrAssignedEmp = [];

        mElement = $(mHtml);
        mOut.element = mElement;        

        $('#tabTodoWork').empty().append(mElement);

        var txtTodoOrg = mElement.find('[name="txtTodoOrg"]');
        var txtTodoDueDate = mElement.find('[name="txtTodoDueDate"]');
        var txtTodoDueTime = mElement.find('[name="txtTodoDueTime"]');
        var selTodoCategory = mElement.find('[name="selTodoCategory"]');
        var btnTodoAddEmployee = mElement.find('[name="btnTodoAddEmployee"]');
        var listTodoAssignedEmp = mElement.find('[name="listTodoAssignedEmp"]');
        var txtTodoTitle = mElement.find('[name="txtTodoTitle"]');
        var txtTodoDescription = mElement.find('[name="txtTodoDescription"]');
        var chbTodoMarkAsDone = mElement.find('[name="chbTodoMarkAsDone"]');
        var selTodoUrgency = mElement.find('[name="selTodoUrgency"]');
        var selTodoImportance = mElement.find('[name="selTodoImportance"]');
        var lblTodoNo = mElement.find('[name="lblTodoNo"]');
        var lblTodoStatus = mElement.find('[name="lblTodoStatus"]');
        var lblCreatedBy = mElement.find('[name="lblCreatedBy"]');
        var lblUpdatedBy = mElement.find('[name="lblUpdatedBy"]');
        var btnTodoNew = mElement.find('[name="btnTodoNew"]');
        var btnTodoLookup = mElement.find('[name="btnTodoLookup"]');
        var btnTodoMakeACopy = mElement.find('[name="btnTodoMakeACopy"]');
        var btnTodoSave = mElement.find('[name="btnTodoSave"]');
        var btnTodoMark = mElement.find('[name="btnTodoMark"]');
        var btnTodoDueClear = mElement.find('[name="btnTodoDueClear"]');

        txtTodoOrg.click(function () {
            BrowsePopup('LibraryOrganizationCustomerAndSupplier', 'Search Organization', [
                {
                    DisplayText: [{ Column: 'CompanyName' }],
                    Click: function (e) {
                        OrgID = e.ID;
                        IsOrg = CInt2(e.IsOrganization);
                        txtTodoOrg.val(e.CompanyName);
                    }
                }
            ])
        })

        txtTodoDueDate.click(function () {
            SingleCalendar({
                Click: function (e) {
                    txtTodoDueDate.val(e);
                }
            })
        })

        btnTodoAddEmployee.click(function () {
            BrowsePopup('LibraryOrganizationEmployee', 'Select Employee', [
                {
                    Message: 'Note: You can select multiple employees. Click OK when you\'re done.',
                    DisplayText: [{ Column: 'PeopleName' }],
                    SelectMultiple: true,
                    Click: function (e) {
                        for (var i in e) {
                            e[i].State = 'new';
                            DisplayAssignedEmployee(e[i]);
                        }
                    }
                }
            ])
        })

        btnTodoDueClear.click(function () {
            txtTodoDueDate.val('');
            txtTodoDueTime.val('');
        })

        txtTodoDueTime.click(function () {
            CustomSetTime({
                Click: function (e) {
                    txtTodoDueTime.val(e.time);
                }
            })
        })

        btnTodoNew.click(function () {
            ClearFields();
            WebToast('All fields has been cleared for new To-Do');
        })

        btnTodoLookup.click(function () {
            BrowsePopup('/CRMDashboard/GetTodo', 'List of To-Dos', [
                {
                    Options: {
                        Fields: [
                            $('<tr>')
                                .append($('<td>')
                                    .append('Status: ')
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Undone')
                                            .append($('<input type="radio" name="radTodoBrowse" value="todo" class="form-check-input" checked>')
                                                .change(function () {
                                                    BrowsePopupNow();
                                                })
                                            )
                                        )
                                    )
                                )
                                .append($('<td>')
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append('Done')
                                            .append($('<input type="radio" name="radTodoBrowse" value="done" class="form-check-input">')
                                                .change(function () {
                                                    BrowsePopupNow();
                                                })
                                            )
                                        )
                                    )
                                )
                        ],
                        OnChange: function () {
                            return {
                                Status: $('[name="radTodoBrowse"]:checked').val()
                            }
                        }
                    },
                    DisplayText: [
                        {
                        Label: 'Reference No.', Column: function (e) {
                            return (CStr2(e.TodoNumber) == '' ? '' : 'TD-' + e.TodoNumber);
                            }
                        },
                        { Label: 'Organization', Column: 'ClientName' },
                        { Label: 'Category', Column: 'CategoryName' },
                        {
                            Label: 'Title', Column: function (data) {
                                return '<div style="display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; max-height: 100px; overflow: hidden; text-overflow: ellipsis;">' + data.ToDoTitle + '</div>';
                            }
                        },
                        {
                            Label: 'Description', Column: function (data) {
                                return '<div style="display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; max-height: 100px; overflow: hidden; text-overflow: ellipsis;">' + data.ToDo + '</div>';
                            }
                        },
                        {
                            Label: 'Last Updated', Column: function (e) {
                                return $.format.date(new Date(e.LastUpdated), 'MM-dd-yyyy hh:mm a')
                            }, className: 'text-center'
                        }
                    ],
                    Sorting: false,
                    AutoSearch: true,
                    StyleFormat: 'table',
                    ModalSize: 'fullwidth',
                    Click: function (e) {
                        DisplayTodoData(e);
                    }
                }
            ])
        })

        btnTodoMakeACopy.click(function () {
            if (todoData == null) {
                NewMessageBox('Please select first an existing To-Do item.');
                return
            }

            TodoID = null;

            for (var i in arrAssignedEmp) {
                arrAssignedEmp[i].ID = arrAssignedEmp[i].EmployeeID;
                arrAssignedEmp[i].State = 'new';
            }

            lblTodoNo.text('');
            lblTodoStatus.removeAttr('class').text('Unsaved');
            lblCreatedBy.text('');
            lblUpdatedBy.text('');
        })

        btnTodoSave.click(function () {
            if (txtTodoDueTime.val() != '' && txtTodoDueDate.val() == '') {
                NewMessageBox('Please set Due date');
                return;
            }            

            if (txtTodoTitle.val() == '') {
                NewMessageBox('Please enter To-Do title');
                return;
            }

            if (txtTodoDescription.val().trim() == '') {
                NewMessageBox('Please enter To-Do description');
                return;
            }

            var data = {
                ToDoID: TodoID,
                ClientID: CStr2(OrgID),
                ToDoTitle: txtTodoTitle.val(),
                ToDo: txtTodoDescription.val().trim(),
                ToDoFinish: CInt2(chbTodoMarkAsDone.is(':checked')),
                ToDoDueDate: $.trim(txtTodoDueDate.val() + ' ' + txtTodoDueTime.val()),
                State: (TodoID == null ? 'Add' : 'Edit'),
                ToDoOrder: 0,
                IsOrg: IsOrg == null ? 1 : IsOrg,
                TaggedType: 'General',
                TodoCategoryID: selTodoCategory.val(),
                AssignedEmployee: arrAssignedEmp,
                Urgency: selTodoUrgency.val(),
                Importance: selTodoImportance.val()
            }

            AjaxSend({
                url: '/CRMDashboard/SaveToDo',
                data: { ToDo: [data], DateNow: new Date() },
                success: function (e) {
                    if (e.Results.toLowerCase() == 'saved') {
                        WebToast('New To-Do has been saved', 'top');
                        ClearFields();
                        GetTodoDetails(e.ID);
                    } else {
                        NewMessageBox(e.Results);
                    }
                }
            })
        })

        btnTodoMark.click(function () {
            if (todoData == null) {
                NewMessageBox('Please select first an existing To-Do item.');
                return
            }

            if (lblTodoStatus.text().toLowerCase() == 'unsaved') {
                NewMessageBox('Please save the To-Do item first. Thank you!');
                return;
            }

            var ischeck = chbTodoMarkAsDone.is(':checked')
            NewMessageBox('Are you sure you want to mark as ' + (ischeck ? '"undone"' : '"done"') + ' this To-Do?', 'Confirmation Message', [
                {
                    Text: 'Yes', Click: function () {
                        if (ischeck) {
                            chbTodoMarkAsDone.removeAttr('checked');
                        } else {
                            chbTodoMarkAsDone.prop('checked', 'checked');
                        }

                        btnTodoSave.click();
                    }
                }, { Text: 'No' }
            ])
        })

        function GetTodoDetails(id) {
            Bs.Loading('show');
            AjaxSend({
                url: '/CRMDashboard/GetTodo',
                data: { ToDoID: id, Status: 'todo' },
                success: function (e) {
                    Bs.Loading('hide');
                    DisplayTodoData(e.Results.Rows[0]);
                }
            })
        }

        function DisplayTodoData(e) {
            ClearFields();

            var duetime = e.ToDoDueDate == undefined ? '' : $.format.date(new Date(e.ToDoDueDate), 'hh:mm a');
            todoData = e;
            TodoID = e.ID;
            OrgID = e.ClientID;
            IsOrg = e.IsOrg;
            txtTodoOrg.val(e.ClientName);
            txtTodoDueDate.val(e.ToDoDueDate == undefined ? '' : $.format.date(new Date(e.ToDoDueDate), 'MMM dd, yyyy'));
            txtTodoDueTime.val(duetime);
            txtTodoTitle.val(e.ToDoTitle);
            txtTodoDescription.val(e.ToDo);
            selTodoCategory.val(CStr2(e.TodoCategoryID));
            selTodoUrgency.val(e.Urgency || 'NA');
            selTodoImportance.val(e.Importance || 'NA');
            lblTodoNo.text((CStr2(e.TodoNumber) == '' ? '' : 'TD-' + e.TodoNumber));
            lblTodoStatus.text(CBool(e.ToDoFinish) ? 'Marked as Done' : 'Marked as Undone').removeAttr('class').addClass(CBool(e.ToDoFinish) ? 'text-success' : 'text-danger');
            lblCreatedBy.text('Created by: ' + e.CreatedByName + (e.CreatedOn == undefined ? '' : ' on ' + $.format.date(new Date(e.CreatedOn), 'MM-dd-yyyy hh:mm a')));
            lblUpdatedBy.text('Last Updated by: ' + e.LastUpdatedByName + ' on ' + $.format.date(new Date(e.LastUpdated), 'MM-dd-yyyy hh:mm a'));
            chbTodoMarkAsDone.prop('checked', CBool(e.ToDoFinish));

            if (CBool(e.ToDoFinish)) {
                btnTodoMark.find('span').removeAttr('class').addClass('bi-square');
                btnTodoMark.find('div').text('Mark as Undone');
            } else {
                btnTodoMark.find('span').removeAttr('class').addClass('bi-check-square');
                btnTodoMark.find('div').text('Mark as Done');
            }

            AjaxSend({
                url: 'CRMDashboard/GetTodoAssignedEmployee',
                data: { TodoID: TodoID },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    for (var i in vRows) {
                        vRows[i].State = 'edit';
                        DisplayAssignedEmployee(vRows[i]);
                    }
                }
            })
        }

        function DisplayAssignedEmployee(data) {
            if ($('[empid="' + data.ID + '"]').lenght > 0) {
                return;
            }

            arrAssignedEmp.push(data);

            listTodoAssignedEmp.append($('<li class="list-group-item list-group-item-action">')
                    .data('data', data)
                    .attr('empid', data.ID)
                    .append($('<div class="row align-items-center">')
                        .append($('<div class="col">').text(data.PeopleName))
                        .append($('<div class="col-auto">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
                                .data('data', data)
                                .click(function () {
                                    data.State = 'removed';
                                    $(this).parents().closest('li').remove();
                                })
                            )
                        )
                    )
                )
        }

        function ClearFields() {
            todoData = null;
            TodoID = null;
            OrgID = null;
            arrAssignedEmp = [];
            txtTodoOrg.val('');
            txtTodoDueDate.val('');
            txtTodoDueTime.val('');
            txtTodoTitle.val('');
            txtTodoDescription.val('');
            selTodoCategory.val('');
            selTodoUrgency.val('NA');
            selTodoImportance.val('NA');
            lblTodoNo.text('');
            lblTodoStatus.text('');
            lblCreatedBy.text('');
            lblUpdatedBy.text('');
            chbTodoMarkAsDone.removeAttr('checked');
            listTodoAssignedEmp.empty();
            btnTodoMark.find('span').removeAttr('class').addClass('bi-check-square');
            btnTodoMark.find('div').text('Mark as Done');
        }
                
        mOut.Initialize = function () {
            AjaxSend({
                url: '/Browser/LibraryCRMTodoCategories',
                success: function (e) {
                    var vRows = e.Results.Rows;
                    selTodoCategory.empty();
                    selTodoCategory.append(new Option('Please select', ''));

                    for (var i in vRows) {
                        selTodoCategory.append(new Option(vRows[i].CategoryName, vRows[i].ID));
                    }
                }
            })
        }

        mOut.Initialize();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});