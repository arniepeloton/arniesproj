require(['common'], function () {
    $(document).ready(function () {

        if (CStr2(sessionStorage.ViewID) != '') {
            GetData();
        }

        window.Centers = {
            Add: function(){
                $('#txtCentersName').val('');
                $('#txtCentersDescription').val('');
                $('#selCentersStatus').val('1');//.selectmenu('refresh');
            },
            View: function () {
                if (CStr2(sessionStorage.ViewID) != '') {
                    GetData();
                }
            }
        }

        $('body')
        .on('click', '#btnCancelCenter', function () {
            if (CBool(sessionStorage.IsCenterFromExpenseTracker)) {
                $('#divETMainDiv').pageshow();
                AddBreadCrumb({
                    text: 'Expense Tracking', Click: function () {
                        $('#divETMainDiv').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            } else {
                location.hash = 'LibraryPage';
                LibraryTable.Centers();
            }            
        })
        .on('click', '#btnSaveCenter', function () {
            var txtCentersName = $('#txtCentersName').val();
            var txtCentersDescription = $('#txtCentersDescription').val();
            var selCentersStatus = $('#selCentersStatus').val();

            if ($.trim(txtCentersName) == '') {
                WebToast('Please enter name', 'top');
                return;
            }

            AjaxSend({
                url: 'Centers/Save',
                data: {
                    ID: sessionStorage.ViewID,
                    Name: txtCentersName,
                    Description: txtCentersDescription,
                    IsActive: selCentersStatus,
                    Type: sessionStorage.LibraryName
                },
                success: function (e) {
                    if (e.Results == 'saved') {
                        WebToast('Successfully saved', 'top');
                        if (CBool(sessionStorage.IsCenterFromExpenseTracker)) {
                            $('#divETMainDiv').pageshow();
                            AddBreadCrumb({
                                text: 'Expense Tracking', Click: function () {
                                    $('#divETMainDiv').pageshow();
                                }
                            }, '.ItemsBreadCrumbs')

                            if (sessionStorage.LibraryName == 'Cost Centers'){
                                ERP.ExpenseTracking.main.controller.GetCostCenters();
                            } else {
                                ERP.ExpenseTracking.main.controller.GetRevenueCenters()
                            }
                        } else {
                            location.hash = 'LibraryPage';
                            LibraryTable.Centers();
                        }                        
                    } else {
                        NewMessageBox(e.Results);
                    }
                }
            })
        })

        function GetData() {
            AjaxSend({
                url: 'Centers/GetData',
                data: {
                    ID: sessionStorage.ViewID
                },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    $('#txtCentersName').val(vRows[0].Name);
                    $('#txtCentersDescription').val(vRows[0].Description);
                    $('#selCentersStatus').val(CInt2(vRows[0].IsActive || 1));//.selectmenu('refresh');
                }
            })
        }
    })
})