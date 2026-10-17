require(['common'], function () {
    $(document).ready(function () {
        var arrSettings = ["FOWST", "Authorized Devices", "Receipt Settings", "Security Roles", "Employees",
                            "Printer Roles", "Hot Button Settings", "Group Settings", "Customer List", "Supplier List",
                            "Sales Team"];

        window.GeneralSettings = {
            Load: function () {
                GetGeneralSettings();
            }
        }

        $('body')
        .on('click', '#btnCancelGeneralSettings', function () {
            window.location.hash = 'SystemAdministration';
        })
        .on('click', '#btnSaveGeneralSettings', function () {
            AjaxSend({
                url: 'SetupChecklist/SaveGeneralSettings',
                data: {
                    Fowst: $('#chbFOWST').is(':checked'),
                    AuthorizedDevices: $('#chbAuthorizedDevices').is(':checked'),
                    ReceiptSettings: $('#chbReceiptSettings').is(':checked'),
                    SecurityRoles: $('#chbSecurityRoles').is(':checked'),
                    Employees: $('#chbEmployees').is(':checked'),
                    PrinterRoles: $('#chbPrinterRoles').is(':checked'),
                    HotButtonSettings: $('#chbHotButtonSettings').is(':checked'),
                    GroupSettings: $('#chbGroupSettings').is(':checked'),
                    CustomerList: $('#chbCustomerList').is(':checked'),
                    SupplierList: $('#chbSupplierList').is(':checked'),
                    SalesTeam: $('#chbSalesTeam').is(':checked'),
                    Enable2FA: $('#chb2FA').is(':checked'),
                    AutoApproveFranchiseeRequests: $('#chbAutoApproveFR').is(':checked'),
                    ShowAnnouncements: $('#chbShowAnnouncement').is(':checked'),
                    RequiredCredentialsTurnoverMonies: $('#chbRequiredCredentialsTurnoverMonies').is(':checked'),
                    EnableVDMFeatures: $('#chbVDMFeatures').is(':checked'),
                    DownloadSKUPics: $('#chbDownloadSKUPics').is(':checked'),
                    ShowMCRModulePOS: $('#chbPOS').is(':checked'),
                    ShowMCRModulePriceChecker: $('#chbPriceChecker').is(':checked'),
                    ShowMCRModulePO: $('#chbPO').is(':checked'),
                    ShowMCRModuleSalesReturn: $('#chbSalesReturn').is(':checked'),
                    ShowMCRModuleStockReceiving: $('#chbStockReceiving').is(':checked'),
                    ShowMCRModuleStockTransfer: $('#chbStockTransfer').is(':checked'),
                    ShowMCRModuleInventoryCount: $('#chbInventoryCount').is(':checked'),
                    ShowMCRModuleRouteExpenseTracking: $('#chbRouteExpenseTracking').is(':checked'),
                    ShowMCRModuleProductMaker: $('#chbProductMaker').is(':checked'),
                    ShowMCRModuleShelfBinLabelling: $('#chbShelfBinLabelling').is(':checked'),
                    ShowMCRModuleRequestApprovals: $('#chbRequestApprovals').is(':checked'),
                    ShowMCRModuleCashHandling: $('#chbCashHandling').is(':checked'),
                    ShowMCRModuleDashboard: $('#chbDashboard').is(':checked'),
                    ShowMCRModuleReportZRead: $('#chbReportZRead').is(':checked'),
                    ShowMCRModuleReconstructSalesData: $('#chbReconstructSalesData').is(':checked'),
                    ShowMCRModuleLazyUpdate: $('#chbLazyUpdate').is(':checked'),
                    ShowMCRModuleRestoreSystem: $('#chbRestoreSystem').is(':checked'),
                    ShowMCRModuleCustomerDisplay: $('#chbCustomerDisplay').is(':checked'),
                    ShowMCRModuleOrderTakerSetup: $('#chbOrderTakerSetup').is(':checked'),
                    ShowMCRModuleSMSSetup: $('#chbSMSSetup').is(':checked'),
                    ShowMCRModuleAdjustZReadDate: $('#chbAdjustZReadDate').is(':checked'),
                    ShowMCRModuleUpdateCacheDB: $('#chbUpdateCacheDB').is(':checked')
                }, success: function (e) {
                    if (e.Results == 'saved') {
                        window.location.hash = 'SystemAdministration';
                        WebToast('Successfully saved');
                    } else {
                        NewMessageBox('There was an error while saving.');
                    }
                }
            })
        })
        .on('change', '#sel2FAFilter', function () {
            EmployeesEmailAdd();
        })

        function GetGeneralSettings() {
            AjaxSend({
                url: 'SetupChecklist/GetGeneralSettings',
                success: function (e) {
                    $('#chbFOWST').prop('checked', CInt2(e.Fowst));
                    $('#chbAuthorizedDevices').prop('checked', CInt2(e.AuthorizedDevices));
                    $('#chbReceiptSettings').prop('checked', CInt2(e.ReceiptSettings));
                    $('#chbSecurityRoles').prop('checked', CInt2(e.SecurityRoles));
                    $('#chbEmployees').prop('checked', CInt2(e.Employees));
                    $('#chbPrinterRoles').prop('checked', CInt2(e.PrinterRoles));
                    $('#chbHotButtonSettings').prop('checked', CInt2(e.HotButtonSettings));
                    $('#chbGroupSettings').prop('checked', CInt2(e.GroupSettings));
                    $('#chbCustomerList').prop('checked', CInt2(e.CustomerList));
                    $('#chbSupplierList').prop('checked', CInt2(e.SupplierList));
                    $('#chbSalesTeam').prop('checked', CInt2(e.SalesTeam));
                    $('#chb2FA').prop('checked', CInt2(e.Enable2FA));
                    $('#chbAutoApproveFR').prop('checked', CInt2(e.AutoApproveFranchiseeRequests));
                    $('#chbShowAnnouncement').prop('checked', CInt2(e.ShowAnnouncements));
                    $('#chbRequiredCredentialsTurnoverMonies').prop('checked', CInt2(e.RequiredCredentialsTurnoverMonies));
                    $('#chbVDMFeatures').prop('checked', CInt2(e.EnableVDMFeatures));
                    $('#chbDownloadSKUPics').prop('checked', CInt2(e.DownloadSKUPics));
                    $('#chbPOS').prop('checked', CInt2(e.ShowMCRModulePOS));
                    $('#chbPriceChecker').prop('checked', CInt2(e.ShowMCRModulePriceChecker));
                    $('#chbPO').prop('checked', CInt2(e.ShowMCRModulePO));
                    $('#chbSalesReturn').prop('checked', CInt2(e.ShowMCRModuleSalesReturn));
                    $('#chbStockReceiving').prop('checked', CInt2(e.ShowMCRModuleStockReceiving));
                    $('#chbStockTransfer').prop('checked', CInt2(e.ShowMCRModuleStockTransfer));
                    $('#chbInventoryCount').prop('checked', CInt2(e.ShowMCRModuleInventoryCount));
                    $('#chbRouteExpenseTracking').prop('checked', CInt2(e.ShowMCRModuleRouteExpenseTracking));
                    $('#chbProductMaker').prop('checked', CInt2(e.ShowMCRModuleProductMaker));
                    $('#chbShelfBinLabelling').prop('checked', CInt2(e.ShowMCRModuleShelfBinLabelling));
                    $('#chbRequestApprovals').prop('checked', CInt2(e.ShowMCRModuleRequestApprovals));
                    $('#chbCashHandling').prop('checked', CInt2(e.ShowMCRModuleCashHandling));
                    $('#chbDashboard').prop('checked', CInt2(e.ShowMCRModuleDashboard));
                    $('#chbReportZRead').prop('checked', CInt2(e.ShowMCRModuleReportZRead));
                    $('#chbReconstructSalesData').prop('checked', CInt2(e.ShowMCRModuleReconstructSalesData));
                    $('#chbLazyUpdate').prop('checked', CInt2(e.ShowMCRModuleLazyUpdate));
                    $('#chbRestoreSystem').prop('checked', CInt2(e.ShowMCRModuleRestoreSystem));
                    $('#chbCustomerDisplay').prop('checked', CInt2(e.ShowMCRModuleCustomerDisplay));
                    $('#chbOrderTakerSetup').prop('checked', CInt2(e.ShowMCRModuleOrderTakerSetup));
                    $('#chbSMSSetup').prop('checked', CInt2(e.ShowMCRModuleSMSSetup));
                    $('#chbAdjustZReadDate').prop('checked', CInt2(e.ShowMCRModuleAdjustZReadDate));
                    $('#chbUpdateCacheDB').prop('checked', CInt2(e.ShowMCRModuleUpdateCacheDB));

                    EmployeesEmailAdd();
                }
            })
        }

        function EmployeesEmailAdd() {
            AjaxSend({
                url: 'SetupChecklist/EmployeesEmailAdd',
                data: { FilterBy: $('#sel2FAFilter option:selected').val() },
                success: function (e) {
                    $('#tbl2FAEmployees tbody').empty();

                    if ($.fn.DataTable.isDataTable("#tbl2FAEmployees")) {
                        //check if datatable is created 
                        $('#tbl2FAEmployees').DataTable().clear().destroy();
                    }

                    var ctr = 0, tbl2FAEmployees = $('#tbl2FAEmployees').DataTable({
                        data: e.Employees,
                        "autoWidth": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return ctr + '.';
                                },
                                "width": "50px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.EmployeeName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return CStr2(data.EmailAdd);
                                }, className: 'text-center'
                            },
                            {
                                data: null,
                                createdCell: function (td, cellData, rowData, row, col) {
                                    var btn = $('<button data-role="none" class="btn btn-secondary">')
                                                .data('data', rowData)
                                                .append('<span class="bi-gear"></span>')
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();

                                                    var data = $(this).data('data'), div2FAEmailAddList = $('#div2FAEmailAddList');

                                                    $('#lbl2FAEmpName').text(data.EmployeeName);
                                                    div2FAEmailAddList.empty();

                                                    for (var i in data.EmailAddress) {
                                                        var pData = data.EmailAddress[i];
                                                        var field = PopulateEmailAddField().data('data', pData);

                                                        field.find('input').val(pData.EmailAddress);
                                                        div2FAEmailAddList.append(field);
                                                    }
                                                    
                                                    div2FAEmailAddList.append(PopulateEmailAddField());
                                                    $('#pop2FAEmailAdd').modal('show');

                                                    $('#btnAdd2FAEmailAdd').unbind('click').click(function () {
                                                        div2FAEmailAddList.append(PopulateEmailAddField());
                                                    })

                                                    $('#btn2FAEmailAddSave').unbind('click').click(function () {
                                                        var arrEA = [], isValid = true;

                                                        $('.txt2FAEA').each(function () {
                                                            var dis = $(this), val = $.trim(dis.val());

                                                            if (val != '') {
                                                                if (!IsEmail(val)) {
                                                                    isValid = false;
                                                                    WebToast('Please enter valid e-mail address', 'top');
                                                                    return isValid;
                                                                } else {
                                                                    var vData = dis.parent().data('data');
                                                                    if (vData == undefined) {
                                                                        arrEA.push({
                                                                            ID: NewID(),
                                                                            EmailAddress: val
                                                                        })
                                                                    } else {
                                                                        vData.EmailAddress = val;
                                                                        arrEA.push(vData);
                                                                    }
                                                                }
                                                            }
                                                        })

                                                        if (!isValid) {
                                                            return;
                                                        }

                                                        AjaxSend({
                                                            url: 'SetupChecklist/SaveEmpEmailAdd',
                                                            data: {
                                                                EmployeeID: data.ID,
                                                                EmailAddress: arrEA
                                                            }, success: function (e) {
                                                                if (e.Results == 'saved') {
                                                                    WebToast('Successfully saved', 'top');
                                                                    EmployeesEmailAdd();
                                                                    $('#pop2FAEmailAdd').modal('hide');
                                                                } else {
                                                                    NewMessageBox('There\'s an error while saving.');
                                                                }
                                                            }
                                                        })
                                                    })
                                                })

                                    $(td).css('text-align', 'right').html(btn);
                                },
                                "width": "50px"
                            }
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                        "initComplete": function () {
                            $('#tbl2FAEmployees').removeClass('dataTable');
                        },
                        //"columnDefs": [{ "targets": 0, "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">',
                        stateSave: true
                    })

                    $('#txtSearch2FA').unbind('keyup').keyup(function () {
                        tbl2FAEmployees.search($(this).val()).draw();
                    });
                }
            })
        }

        function PopulateEmailAddField() {
            var field = $('<div class="input-group mb-3">')
                            .append('<input data-role="none" class="form-control txt2FAEA" placeholder="Enter E-mail Address here" />')
                            .append($('<button data-role="none" class="btn btn-danger" title="Remove">')
                                .append('<span class="bi-trash"></span>')
                                .click(function () {
                                    var dis = $(this).parent(), val = $.trim(dis.find('input').val()), data = dis.data('data');
                                    
                                    if (val != '') {
                                        NewMessageBox('Are you sure you want to remove this email address (<i>' + val + '</i>)?', 'Confirmation Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    if (data != undefined) {
                                                        data.State = 'remove';
                                                        dis.hide();
                                                    } else {
                                                        dis.remove();
                                                    }
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    } else {
                                        dis.remove();
                                    }
                                })
                            )

            return field
        }

        GetGeneralSettings();
    })
})