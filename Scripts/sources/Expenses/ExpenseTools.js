require(['common'], function () {
    $(document).ready(function () {
        GetListOfEmployee();

        $('body')
        .on('click', '#btnRecompute', function () {
            AjaxSend({
                url: 'Expenses/RecomputeNetFundBalance',
                data: { EmployeeID: $('#selExpenseEmployee').val(), Password: $('#txtAdminPassword').val() },
                success: function (e) {
                    $('#divResult').text(JSON.stringify(e, null, 4));
                }
            })
        })
        .on('click', '#btnAssignFowst', function () {
            AjaxSend({
                url: 'Expenses/ApplySelectedFowstToExpenses',
                data: { EmployeeID: $('#selExpenseEmployee').val(), FowstID: $('#selExpenseFowst').val(), Password: $('#txtAdminPassword').val() },
                success: function (e) {
                    $('#divResult').text(JSON.stringify(e, null, 4));
                }
            })
        })

        function GetListOfEmployee() {
            AjaxSend({
                url: 'Browser/LibraryOrganizationEmployee',
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var selExpenseEmployee = $('#selExpenseEmployee');

                    selExpenseEmployee.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            selExpenseEmployee.append(new Option(vRows[i].PeopleName, vRows[i].ID));
                        }
                    }

                    selExpenseEmployee.selectmenu('refresh');

                    GetAllFOWST();
                }
            })
        }

        function GetAllFOWST() {
            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: {
                    IsAll: 1
                },
                success: function (e) {
                    var selExpenseFowst = $('#selExpenseFowst');
                    var rows = e.Results.Rows;

                    selExpenseFowst.empty();

                    if (rows.length > 0) {
                        for (var i = 0; i < rows.length; i++) {
                            selExpenseFowst.append(new Option(rows[i].FowstName, rows[i].ID));
                        }

                        selExpenseFowst.val(sessionStorage.FowstID);
                    }

                    selExpenseFowst.selectmenu('refresh');
                }
            })
        }
    })
})