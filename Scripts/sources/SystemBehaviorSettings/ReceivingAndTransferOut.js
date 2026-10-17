require([
    'common',
    '/Scripts/jquery-ui-sortable.js'
], function () {

    var IsAutomaticReceived = 0;
    var IsShowQuantity = 0;
    var IsExternalInReceiveIn = 0;
    var IsThirdPartyCompanyOrOrganization = 0;
    var SRSupplierID = '', SRSupplierOutletID = '';
    var SKURankNo = 0;
    var arrInventoryCountItems = [], arrSKUWithMaxQty = [], dttblMaxQtyRecTrans;

    $(function () {
        GetSystemBehavior();

        window.SystemBehavior = {
            GetSystemBehaviorData: function () {
                GetSystemBehavior();
            }
        }

        $('body')
            .on('click', '#rdReceivedAutomaticallyWithAcknowledgement', function () {
                IsAutomaticReceived = 1;
            })
            .on('click', '#rdManuallyReceivedStocks', function () {
                IsAutomaticReceived = 0;
            })
            .on('click', '#rdHideStocksQuantity', function () {
                IsShowQuantity = 0;
            })
            .on('click', '#rdShowStocksQuantity', function () {
                IsShowQuantity = 1;
            })
            .on('click', '[name="btnSaveReceivingSettings"]', function () {
                SaveReceivingSystemBehavior();
            })
            .on('click', '[name="btnDone"]', function () {
                location.hash = '';
            })
            .on('click', '#rdWithinTheCompanyOrOrganization', function () {
                IsThirdPartyCompanyOrOrganization = 0;
                $('#divInternalRecInDefaultSetting').show();
            })
            .on('click', '#rdThirdPartyCopanyOrOrganization', function () {
                IsThirdPartyCompanyOrOrganization = 1;
                $('#divInternalRecInDefaultSetting').hide();
            })
            .on('click', '#rdInternalOrganization', function () {
                IsExternalInReceiveIn = 0;
                $('#divExternalDefaultSetting').hide();
            })
            .on('click', '#rdExternalOrganization', function () {
                IsExternalInReceiveIn = 1;
                $('#divExternalDefaultSetting').show();
            })
            .on('change', '[name="rdReceiveFromExternal"]', function () {
                SRSupplierID = '';
                SRSupplierOutletID = '';
                $('#btnCancelGeneralSettings, #btnSRThirdPartySupplierOutlet').text('Please select');

                if ($(this).val() == 'set') {
                    $('#divSRThirdParty').show();
                } else {
                    $('#divSRThirdParty').hide();
                }
            })
            .on('click', '#btnSRThirdPartySupplier', function () {
                BrowsePopup('LibraryOrganizationSupplier', 'Select Supplier', [
                    {
                        DisplayText: [{ Column: 'SupplierName' }],
                        Click: function (s) {
                            if (SRSupplierID != s.ID) {
                                SRSupplierID = s.ID;
                                SRSupplierOutletID = '';
                                $('#btnSRThirdPartySupplier').text(s.SupplierName);
                                $('#btnSRThirdPartySupplierOutlet').text('Please select');

                                if (SRSupplierOutletID == '') {
                                    $('#btnSRThirdPartySupplierOutlet').click();
                                }
                            }
                        }
                    }
                ])
            })
            .on('click', '#btnSRThirdPartySupplierOutlet', function () {
                BrowsePopup('OrganizationShort/GetCustomerOrgOutlets', 'Select Outlet', [
                    {
                        DisplayText: [{ Column: 'Name' }],
                        ReturnVariable: 'Results',
                        AutoSearch: true,
                        Click: function (o) {
                            SRSupplierOutletID = o.ID;
                            $('#btnSRThirdPartySupplierOutlet').text(o.Name);
                        }
                    }
                ], { ID: SRSupplierID });
            })
            .on('click', '#btnStockRecTransAddSKU', function () {
                BrowsePopup('LibraryPIMProduct', 'Select Product', [
                    {
                        DisplayText: [{ Label: 'Product No.', Column: 'ProductNo' }, { Label: 'Description', Column: 'ProductName' }],
                        SelectMultiple: true,
                        Click: function (e) {
                            for (var i in e) {
                                DisplayStockRecTransSKU(e[i]);
                            }
                        }
                    }
                ])
            })
            .on('keydown', '#txtSearchStockRecTrans', function (e) {
                var filter = $(this).val().toLowerCase();
                var lis = $('#listStockRecTransSKURanking > li');
                lis.each(function () {
                    var name = $(this).text();
                    if (name.toLowerCase().indexOf(filter) != -1)
                        $(this).css('display', 'block');
                    else
                        $(this).css('display', 'none');
                })
            })
            .on('change', '#chbShowSKUWithMaxQty', function (e) {
                if ($(this).is(':checked')) {
                    DisplayInventoryCountItems(arrSKUWithMaxQty);
                } else {
                    DisplayInventoryCountItems(arrInventoryCountItems);
                }
            })
            .on('click', '#btnSaveMaxQty', function () {
                var str = '';

                for (var i in arrSKUWithMaxQty) {
                    var sku = arrSKUWithMaxQty[i];
                    str += (CInt2(i)+ 1) + '. (' + sku.ProductNo + ') ' + sku.ProductNameOnly + ': <b>' + sku.MaxQtyToBeRcvTfer + '</b><br>';
                }

                NewMessageBox('This is the list of SKUs that you set with maximum quantity. Please review and make changes if necessary. <br><br>' + str, 'Confirmation Dialog', [
                    {
                        Text: 'Confirm and save now', Click: function () {
                            Bs.Loading('show')

                            AjaxSend({
                                url: 'SystemBehaviorSettings/SaveMaxQty',
                                data: { data: arrSKUWithMaxQty },
                                success: function (e) {
                                    if (e.Result == 'saved') {
                                        WebToast('Successfully saved');
                                        GetInventoryCountItems();
                                    } else {
                                        NewMessageBox(e.Result, 'Error')
                                    }
                                }
                            })
                        }
                    }, { Text: 'Cancel' }
                ])
            })
    });

    function DisplayStockRecTransSKU(e) {
        SKURankNo += 1;
        e.OrderNo = SKURankNo;
        $('#listStockRecTransSKURanking').append($('<li class="list-group-item">')
            .data('data', e)
            .css('cursor', 'pointer')
            .append($('<div class="row align-items-center">')
                .append($('<div class="col-auto">').text(SKURankNo + '.'))
                .append($('<div class="col">').html(e.ProductNo + ' <br> ' + e.ProductName))
                .append($('<div class="col-auto">')
                    .append($('<button data-role="none" class="btn btn-danger">')
                        .append('<span class="bi-trash"></span>')
                        .click(function (e) {
                            $(this).parents().closest('li').remove();
                        })
                    )
                )
            )            
        )
    }

    function SaveReceivingSystemBehavior() {
        var rdReceiveFromExternal = $('[name="rdReceiveFromExternal"]:checked').val();

        if (rdReceiveFromExternal == 'set') {
            if (SRSupplierID == '' || SRSupplierOutletID == '') {
                NewMessageBox('Please select Supplier and Outlet');
                return;
            }
        }

        var SKUList = [];
        SKURankNo = 0;
        $('#listStockRecTransSKURanking li').each(function () {
            SKURankNo += 1;
            $(this).data('data').OrderNo = SKURankNo;
            SKUList.push($(this).data('data'));
        })

        var data = {
            //IsAutomaticallyReceivedStocks: IsAutomaticReceived,
            SRDefaultSourceDocType: $.trim($('#txtSRDefaultSourceDocType').val()),
            IsShowQuantities: IsShowQuantity,
            IsThirdPartyCompanyOrOrganization: IsThirdPartyCompanyOrOrganization,
            IsExternalInReceiveIn: IsExternalInReceiveIn,
            IsReceiveInInternalSetToDefault: ($('[name="rdReceiveInInternal"]:checked').val() == 'set' ? true : false),
            SRUseThirdPartyAsSource: (rdReceiveFromExternal == 'set' ? true : false),
            SRThirdPartySupplier: SRSupplierID,
            SRThirdPartySupplierOutlet: SRSupplierOutletID,
            AllowSRBadCount: $('#rdBadCount').is(':checked'),
            RemindUserAboutTheDraftTransactionsStockRecTrans: $('#rdRemindUserAboutTheDraftTransactionsStockRecTransfer').is(':checked'),
            RecTransPackSizeDiscprepancy: $('#rdRecTransPackSizeDiscprepancy').is(':checked'),
            SKUList: SKUList
        };

        AjaxSend({
            url: 'SystemBehaviorSettings/SaveReceivingBehavior',
            data: data,
            success: function (vData) {
                if (vData.Result == "Success") {
                    WebToast('Successfully saved', 'top');
                    history.back();
                }
            }
        });

    }

    function GetSystemBehavior() {
        SRSupplierID = '';
        SRSupplierOutletID = '';
        SKURankNo = 0;
        $('#btnCancelGeneralSettings, #btnSRThirdPartySupplierOutlet').text('Please select');

        AjaxSend({
            url: 'SystemBehaviorSettings/GetReceivingAndTransferOutBehavior',
            success: function (vData) {

                //if (vData.IsAutomaticallyReceivedStocks == 1) {

                //    $('#rdReceivedAutomaticallyWithAcknowledgement').prop('checked', true);//.checkboxradio("refresh");
                //    $('#rdManuallyReceivedStocks').prop('checked', false);//.checkboxradio("refresh");
                    
                //} else {

                //    $('#rdManuallyReceivedStocks').prop('checked', true);//.checkboxradio("refresh");
                //    $('#rdReceivedAutomaticallyWithAcknowledgement').prop('checked', false);//.checkboxradio("refresh");

                //}
                $('#rdBadCount').prop('checked', CBool(vData.AllowSRBadCount));
                $('#rdRemindUserAboutTheDraftTransactionsStockRecTransfer').prop('checked', CBool(vData.RemindUserAboutTheDraftTransactionsStockRecTrans));
                $('#rdRecTransPackSizeDiscprepancy').prop('checked', CBool(vData.RecTransPackSizeDiscprepancy));
                $('#txtSRDefaultSourceDocType').val(CStr2(vData.SRDefaultSourceDocType));
                $('[name="rdReceiveInInternal"][value="' + (CBool(vData.IsReceiveInInternalSetToDefault) ? 'set' : 'no') + '"]').prop('checked', 1);

                if (vData.IsShowQuantities == 1) {
                    
                    $('#rdShowStocksQuantity').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdHideStocksQuantity').prop('checked', false);//.checkboxradio("refresh");
                    IsShowQuantity = 1;

                } else {
                    $('#rdHideStocksQuantity').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdShowStocksQuantity').prop('checked', false);//.checkboxradio("refresh");
                    IsShowQuantity = 0;
                }

                if (vData.IsThirdPartyCompanyOrOrganization == 1) {

                    $('#rdThirdPartyCopanyOrOrganization').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdWithinTheCompanyOrOrganization').prop('checked', false);//.checkboxradio("refresh");
                    IsThirdPartyCompanyOrOrganization = 1;

                } else {
                    $('#rdWithinTheCompanyOrOrganization').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdThirdPartyCopanyOrOrganization').prop('checked', false);//.checkboxradio("refresh");
                    IsThirdPartyCompanyOrOrganization = 0;
                }

                if (vData.IsExternalInReceiveIn == 1) {

                    $('#rdExternalOrganization').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdInternalOrganization').prop('checked', false);//.checkboxradio("refresh");
                    IsExternalInReceiveIn = 1;
                    $('#divExternalDefaultSetting').show();

                    $('[name="rdReceiveFromExternal"][value="' + (CBool(vData.SRUseThirdPartyAsSource) ? 'set' : 'no') + '"]').prop('checked', 1);

                    if (CBool(vData.SRUseThirdPartyAsSource)) {
                        SRSupplierID = vData.SRThirdPartySupplier;
                        SRSupplierOutletID = vData.SRThirdPartySupplierOutlet;
                        $('#divSRThirdParty').show();
                        $('#btnSRThirdPartySupplier').text(vData.SRThirdPartySupplierName);
                        $('#btnSRThirdPartySupplierOutlet').text(vData.SRThirdPartySupplierOutletName);
                    } else {
                        $('#divSRThirdParty').hide();
                    }

                } else {
                    $('#rdInternalOrganization').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdExternalOrganization').prop('checked', false);//.checkboxradio("refresh");
                    IsExternalInReceiveIn = 0;
                    $('#divExternalDefaultSetting').hide();
                }

                //if (vData.OrganizationReceiveFrom == 1) {

                //    $('#rdInternalOrganization').prop('checked', true);//.checkboxradio("refresh");
                //    $('#rdInternalOrganization').prop('checked', false);//.checkboxradio("refresh");
                //    IsExternalInReceiveFrom = 1;

                //} else {
                //    $('#rdInternalOrganization').prop('checked', true);//.checkboxradio("refresh");
                //    $('#rdInternalOrganization').prop('checked', false);//.checkboxradio("refresh");
                //    IsExternalInReceiveFrom = 0;
                //}

                $('#listStockRecTransSKURanking').empty();

                for (var i in vData.SKUList.Rows) {
                    DisplayStockRecTransSKU(vData.SKUList.Rows[i]);
                }

                setTimeout(function () {
                    $('#listStockRecTransSKURanking').sortable({
                        list: '#listStockRecTransSKURanking',
                        delay: 150,
                        start: function (event, ui) {
                            ui.item.parent().css('border', '1px solid #CCC');
                        },
                        stop: function (event, ui) {
                            ui.item.parent().css('border', '1px solid #FFF');
                        },
                        update: function (event, ui) {
                            var id = ui.item.parent().attr('id');
                            var newOrderNo = 1;
                            $('#' + id).find('> li').each(function () {
                                $(this).find('.col-auto').text(newOrderNo + '.');
                                $(this).data('data').OrderNo = newOrderNo;
                                newOrderNo += 1;
                            })
                        }
                    });
                }, 100);

                GetInventoryCountItems();
            }
        });
    }
    
    function GetInventoryCountItems() {
        Bs.Loading('show');
        AjaxSend({
            url: 'Browser/LibraryPIMProduct',
            data: {
                IncludeMaxUnit: true,
                InventoryCountItemOnly: true,
                IsAll: true
            }, success: function (e) {
                Bs.Loading('hide');

                arrSKUWithMaxQty = [];
                arrInventoryCountItems = e.Results.Rows
                DisplayInventoryCountItems(arrInventoryCountItems);
            }
        })
    }

    function DisplayInventoryCountItems(vRows) {
        var ctr = 0;

        if ($.fn.DataTable.isDataTable($('#tblMaxQtyRecTrans'))) {
            //$('#tblMaxQtyRecTrans').empty();
            try {
                dttblMaxQtyRecTrans.destroy();
            } catch (e) {

            }
        }

        dttblMaxQtyRecTrans = $('#tblMaxQtyRecTrans').DataTable({
            pageLength: 50,
            "ordering": false,
            "data": vRows,
            "columns": [
                {
                    "data": function (data) {
                        ctr += 1;
                        return ctr + '.';
                    }, width: '50px'
                },
                { "data": "ProductNo" },
                { "data": "ProductNameOnly" },
                { "data": "MaxUnitMeasure" },
                {
                    data: null,
                    createdCell: function (td, cellData, rowData, row, col) {
                        var inp = $('<input data-role="none" type="number" class="form-control">')
                            .data('data', rowData)
                            .val(CInt2(rowData.MaxQtyToBeRcvTfer) == 0 ? '' : rowData.MaxQtyToBeRcvTfer)
                            .keyup(function (e) {
                                var dis = $(this), data = dis.data('data')
                                PopulateSKUWithMaxQty(data, dis.val())
                            })

                        $(td).html(inp);

                        if (CInt2(rowData.MaxQtyToBeRcvTfer) == 0 && $('#chbShowSKUWithMaxQty').is(':checked')) {
                            $(td).parent().hide();
                        }

                        if (CInt2(rowData.MaxQtyToBeRcvTfer) > 0) PopulateSKUWithMaxQty(rowData, rowData.MaxQtyToBeRcvTfer);
                    }
                },
            ],
            dom: '<"clear"><"table-fixed-container"<"table-fixed"t>><"bottom"ip><"clear">',
            initComplete: function () {
                $('#tblMaxQtyRecTrans').removeClass('dataTable');
            }
        });

        $('#txtSearchSKUMaxQty').unbind('keyup').keyup(function () {
            dttblMaxQtyRecTrans.search($(this).val()).draw();
        })
    }

    function PopulateSKUWithMaxQty(data, qty) {
        var isExist = false, ind;

        $.grep(arrSKUWithMaxQty, function (existingData, index) {
            if (existingData.ID == data.ID) {
                ind = index;
                isExist = true;
                return false;
            }
        })

        if (isExist) {
            arrSKUWithMaxQty[ind].MaxQtyToBeRcvTfer = qty;
        } else {
            data.MaxQtyToBeRcvTfer = qty;
            arrSKUWithMaxQty.push(data);
        }
    }
});