require(['common'],
function () {
    $(document).ready(function () {
        //AjaxSend({
        //    ExactUrl: '/Scripts/sources/ERP/InventoryHandling/main.html',
        //    type: 'GET',
        //    dataType: 'text',
        //    success: function (vData) {
        //        $('#divInventoryHandling').html('');
        //        $('#divInventoryHandling').append(vData).trigger('create');
        //    }
        //});

        var htmlInvetoryBalance, htmlInvetoryLedger, htmlInvetoryStockRT, htmlPhysicalInventory, htmlBinAddressTags;
        var tblDormantSkus, arrForDeactivation = [], arrDoNotDeactivate = [], isCheckedAll = 0, recordsTotal = 0;

        LoadData();

        window.InventoryHC = {
            GetData: LoadData
        }

        $('body')
        .on('change', '#selInventoryDashboard', function () {
            var dis = $(this).val();

            if (dis == 'Inventory Balance') {
                CallPage('#tabInventoryBalance', htmlInvetoryBalance, '/InventoryLedger/InventoryBalance', function (e) {
                    htmlInvetoryBalance = e;
                    $('#tabInventoryBalance').trigger('create');
                })
            } else if (dis == 'Inventory Ledger') {
                $('#divForInventoryLedger').empty(); // inv ledger access through inv balance
                sessionStorage.IsFromInventoryBalance = false;
                CallPage('#tabInventoryLedger', htmlInvetoryLedger, '/InventoryLedger', function (e) {
                    htmlInvetoryLedger = e;
                    if ($('#tabInventoryLedger').html().length == 0) {
                        $('#tabInventoryLedger').html(htmlInvetoryLedger);
                    }
                    $('#tabInventoryLedger').trigger('create');
                })
            } else if (dis == 'Stock Receveing/Transfers') {
                CallPage('#tabStockRT', htmlInvetoryStockRT, '/Receiving/ReceivingAndTransfersReport', function (e) {
                    htmlInvetoryStockRT = e;
                    $('#tabStockRT').trigger('create');
                })
            } else if (dis == 'Variance Report') {
                CallPage('#tabPhysicalInventory', htmlPhysicalInventory, '/InventoryCount/Report', function (e) {
                    htmlPhysicalInventory = e;
                    $('#tabPhysicalInventory').trigger('create');
                })
            }
        })
        .on('click', '#btnInventoryHandlingAndControlBinAddressTags, #btnInventoryHandlingAndControl', function () {
            if (CStr2(htmlBinAddressTags) != '') {
                return;
            }

            Bs.Loading('show');

            AjaxSend({
                ExactUrl: '/ShelfAndBinLabeling?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&proj=eas&pid=' + sessionStorage.PeopleID + '&v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    Bs.Loading('hide');
                    vData = $('<div>').append(vData);
                    vData.find('#divIHModules > *').prependTo('#divEASIHModules');
                    vData.find('.modal').prependTo('#divIHMain');
                }
            });

            //CallPage('#tabBinAddressTags', htmlBinAddressTags, '/ShelfAndBinLabeling', function (e) {
            //    htmlBinAddressTags = e;
            //    $('#tabBinAddressTags').find('nav').remove();
            //})
        })
        .on('click', '#btnInventoryHandlingAndControls', function (e) {
            $('#selInventoryDashboard').val('Inventory Balance');
            $('#tabInventoryBalance, #tabInventoryLedger, #tabStockRT, #tabPhysicalInventory').empty();

            AddBreadCrumb({
                text: 'Inventory Handling Reports', Click: function () {
                    $('#divIHReports').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            CallPage('#tabInventoryBalance', htmlInvetoryBalance, '/InventoryLedger/InventoryBalance', function (e) {
                htmlInvetoryBalance = e;
                //$('#tabInventoryBalance').trigger('create');
                $('#divIHReports').pageshow();
            })
        })
        .on('click', '#btnDormantSkusReport', function (e) {
            arrDoNotDeactivate = [];
            arrForDeactivation = [];
            isCheckedAll = 0;
            $('#btnSelectAllDormantSkus').attr('value', 1).text('Select All').removeClass('btn-danger').addClass('btn-light');
            Bs.Loading('show', 'Please wait while getting the dormant SKUs');
            AjaxSend({
                url: 'InventoryForm/UpdateProductDeactivation',
                success: function (e) {
                    if (e.Results == 'done') {
                        Bs.Loading('hide');
                        GetDeactivatingProducts();
                    } else {
                        NewMessageBox(e.Results, 'Error');
                    }
                }
            })
        })
        .on('keydown', '#txtSearchDormantSkus', function (e) {
            if (e.which == 13) {
                tblDormantSkus.search($.trim($(this).val())).draw();
            }
        })
        .on('click', '#btnDeactivateAllDormantSkus', function (e) {
            var dis = $(this);
            if (isCheckedAll == 0 && arrForDeactivation.length == 0) {
                WebToast('Please select at least one dormant SKU to deactivate', 'top');
                return;
            }

            NewMessageBox('Are you sure you want to deactivate <b>'+ (isCheckedAll == 1 ? Math.abs(recordsTotal - arrDoNotDeactivate.length) : arrForDeactivation.length) +'</b> dormant SKUs?', 'Confirmation Message', [
                {
                    Text: 'Yes', Click: function () {
                        dis.attr('disabled', 'disabled');
                        AjaxSend({
                            url: '/InventoryForm/DeactivateAllDormantSkus',
                            data: {
                                IsDeactivateAll: isCheckedAll,
                                DonotDeactivate: "'" + arrDoNotDeactivate.join("','") + "'",
                                Deactivate: "'" + arrForDeactivation.join("','") + "'",
                                AllSkusSelected: "'" + $.merge(arrDoNotDeactivate, arrForDeactivation).join("','") + "'" // whether wala giapil or manually selected, GAMIT ni sya kapag giselect ALL sa user
                            },
                            success: function (e) {
                                dis.removeAttr('disabled');
                                if (e.Results != undefined) {
                                    arrDoNotDeactivate = [];
                                    arrForDeactivation = [];
                                    isCheckedAll = 0;
                                    $('#btnSelectAllDormantSkus').attr('value', 1).text('Select All').removeClass('btn-danger').addClass('btn-light');
                                    WebToast('Selected SKU(s) has been deactivated', 'top');
                                    $('#popupDormantSkus').modal('show');
                                    GetDeactivatingProducts();
                                } else {
                                    NewMessageBox(e.Error, 'Error');
                                }
                            }
                        })
                    }
                }, { Text: 'No' }
            ])
        })
        .on('click', '#btnSelectAllDormantSkus', function () {
            isCheckedAll = CInt2($(this).attr('value'));

            $('[name="chbDormantSku"]').prop('checked', isCheckedAll)//.checkboxradio('refresh');

            if (isCheckedAll == 1) {
                //$('[name="chbDormantSku"]:checked').each(function(){
                //    arrForDeactivation.push($(this).data('data'));
                //})
                $(this).attr('value', 0).text('Unselect All').removeClass('btn-light').addClass('btn-danger');
                WebToast('All ' + recordsTotal + ' dormant SKUS has been selected', 'top');
            } else {
                $(this).attr('value', 1).text('Select All').removeClass('btn-danger').addClass('btn-light');
                arrForDeactivation = [];
                WebToast('All ' + recordsTotal + ' dormant SKUS has been unselected', 'top');
            }
        })
        .on('click', '#btnUndoDormantDeactivation', function () {
            var datenow = $.format.date(new Date(), 'yyyy-MM-dd');
            $('#txtUndoDeactivatedDate').attr('max', datenow).val(datenow);
            $('#popUndoDeactivatedSkus').popup('open');
        })
        .on('click', '#btnReactivateDeactivatedSkus', function () {
            var dis = $(this);
            dis.attr('disabled', 'disabled');
            AjaxSend({
                url: '/InventoryForm/ReactivateDeactivatedSkus',
                data: {
                    Date: $('#txtUndoDeactivatedDate').val()
                }, success: function (e) {
                    dis.removeAttr('disabled');
                    if (e.Results != undefined) {
                        WebToast(e.Results + ' SKU(s) has been reactivated', 'top');
                        GetDeactivatingProducts();
                    } else {
                        NewMessageBox(e.Error, 'Error');
                    }
                }
            })
        })

        //.on('click', '#btnTabInventoryBalance', function () {
        //    //$('#popupViewInventoryLedger').hide();
        //    CallPage('#tabInventoryBalance', htmlInvetoryBalance, '/InventoryLedger/InventoryBalance', function (e) {
        //        htmlInvetoryBalance = e;
        //    })
        //})
        //.on('click', '#btnTabInventoryLedger', function () {
        //    $('#divForInventoryLedger').empty(); // inv ledger access through inv balance
        //    sessionStorage.IsFromInventoryBalance = false;
        //    CallPage('#tabInventoryLedger', htmlInvetoryLedger, '/InventoryLedger', function (e) {
        //        htmlInvetoryLedger = e;
        //        if ($('#tabInventoryLedger').html().length == 0) {
        //            $('#tabInventoryLedger').html(htmlInvetoryLedger);
        //        }
        //    })
        //})
        //.on('click', '#btnTabStockRT', function () {
        //    CallPage('#tabStockRT', htmlInvetoryStockRT, '/Receiving/ReceivingAndTransfersReport', function (e) {
        //        htmlInvetoryStockRT = e;
        //    })
        //})
        //.on('click', '#btnTabPhysicalInventory', function () {
        //    CallPage('#tabPhysicalInventory', htmlPhysicalInventory, '/InventoryCount/Report', function (e) {
        //        htmlPhysicalInventory = e;
        //    })
        //})

        //$('#tabInventoryBalance').opentabmenu();

        function GetDeactivatingProducts() {
            tblDormantSkus = $('#tblDormantSkus');
            tblDormantSkus.find('tbody').empty();


            if ($.fn.DataTable.isDataTable("#tblDormantSkus")) {
                //check if datatable is created 
                $('#tblDormantSkus').DataTable().clear().destroy();
            }

            tblDormantSkus = $('#tblDormantSkus').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: '/InventoryForm/GetDeactivatingProducts?OrgID=' + sessionStorage.OrgID,
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
                            return data.ProductName;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return data.ProductNo;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.Brand);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return $.format.date(new Date(CJsonDate(data.LastTransactionDate)), 'MM-dd-yyyy hh:mm a');
                        },
                        "className": 'txt-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return data.LastTransactionReference;
                        }
                    },
                    {
                        data: null,
                        createdCell: function (td, cellData, rowData, row, col) {
                            var isExist = false;
                            var chb = $('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .append($('<input type="checkbox" class="form-check-input" name="chbDormantSku" data-iconpos="notext" value="' + rowData.ID + '">')
                                                .data('data', rowData)
                                                .change(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    var data = $(this).data('data'), index;
                                                    isExist = false;

                                                    if ($(this).is(':checked')) {
                                                        $.grep(arrForDeactivation, function (e, i) {
                                                            if (e == data.ID) {
                                                                isExist = true;
                                                                index = i;
                                                            }
                                                        })

                                                        if (!isExist) {
                                                            arrForDeactivation.push(data.ID);

                                                            $.grep(arrDoNotDeactivate, function (e, i) {
                                                                if (e == data.ID) {
                                                                    isExist = true;
                                                                    index = i;
                                                                }
                                                            })

                                                            if (isExist) {
                                                                arrDoNotDeactivate.splice(index, 1);
                                                            }
                                                        }
                                                    } else {
                                                        $.grep(arrDoNotDeactivate, function (e, i) {
                                                            if (e == data.ID) {
                                                                isExist = true;
                                                                index = i;
                                                            }
                                                        })

                                                        if (!isExist) {
                                                            arrDoNotDeactivate.push(data.ID);

                                                            $.grep(arrForDeactivation, function (e, i) {
                                                                if (e == data.ID) {
                                                                    isExist = true;
                                                                    index = i;
                                                                }
                                                            })

                                                            if (isExist) {
                                                                arrForDeactivation.splice(index, 1);
                                                            }
                                                        }
                                                    }
                                                })
                                            )
                                        )
                            $(td).data('data', rowData).css('text-align', 'center').html(chb);

                            chb.find('input').prop('checked', isCheckedAll)//.checkboxradio().checkboxradio('refresh');
                        },
                        "width": "100px"
                    }
                ],
                //"createdRow": function (row, data, index) {
                //    $(row).data('data', data).click(function () {
                //        var data = JSON.parse(JSON.stringify(($(this).data('data'))));
                //        console.log(data);
                //    })
                //},
                "initComplete": function (e) {
                    recordsTotal = e.json.recordsTotal;
                    $('#lblCountDormantSkus').text(recordsTotal);
                    $('#lblDormantPeriod').text(e.json.DormantPeriod);
                    //$('#tblDormantSkus').table('refresh');
                    //$('[name="chbDormantSku"]').checkboxradio().checkboxradio('refresh');
                    $('#popupDormantSkus').modal('show');
                },
                "fnDrawCallback": function( oSettings ) {
                    $.grep(arrDoNotDeactivate, function (e, i) {
                        $('[name="chbDormantSku"][value="' + e + '"]').prop('checked', 0)//.checkboxradio().checkboxradio('refresh');
                    })

                    $.grep(arrForDeactivation, function (e, i) {
                        $('[name="chbDormantSku"][value="' + e + '"]').prop('checked', 1)//.checkboxradio().checkboxradio('refresh');
                    })
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        function LoadData() {
            AjaxSend({
                url: 'InventoryForm/CountDormantSkus',
                success: function (e) {
                    if (e.Results.Rows.length > 0) {
                        $('#lblCountDormantSkus').text(e.Results.Rows[0].Counter);
                    }

                    if (sessionStorage.system != 'eas') {
                        AddBreadCrumb({
                            text: 'Main', Click: function () {
                                $('#divIHMain').pageshow();
                            }
                        }, '.ItemsBreadCrumbs')
                    }
                }
            })

            //var tempinterval = setInterval(function () {
            //    if (typeof AddBreadCrumb != 'undefined') {
            //        clearInterval(tempinterval)
            //        $('.ItemsBreadCrumbs').empty();
            //        AddBreadCrumb({
            //            text: 'Main', Click: function () {
            //                $('#divIHMain').pageshow();
            //            }
            //        }, '.ItemsBreadCrumbs')
            //    }
            //}, 100)
        }
    })
})