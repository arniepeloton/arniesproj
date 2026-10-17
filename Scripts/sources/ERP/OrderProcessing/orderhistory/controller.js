define([
    //'/Scripts/sources/ERP/OrderProcessing/dashboard/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var dtCollectionReport;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/orderhistory/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.OrderHistory.main.controller = ERP.OrderHistory.main.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.OrderHistory.main.controller', null, function () {

        var mOut = {};
        var mElement;
        var mCustomerID;
        var mTransID; // TransInvoice.ID;

        mElement = $('<div>').append(mHtml);
        mOut.element = mElement; // mElement does not work!
        //mOut.Model = new ERP.OrderProcessing.main.model();

        mOut.Initialize = function () {
            var parentdiv = $('<div>').append(mHtml);
            var pageCRMOrderHistory = $('#pageCRMOrderHistory'), divErpRevenue = $('#divErpRevenue');
            parentdiv.find('[data-role="content"]').appendTo(parentdiv.find('#popOrderHistory .modal-body'));
            //parentdiv.find('#popCRMOrderHistoryBreakdown .popup-content').appendTo(parentdiv.find('#popCRMOrderHistoryBreakdown-bs .modal-body'));
            //parentdiv.find('#popCRMOrderHistoryBreakdown').remove();
            //parentdiv.find('#popCRMOrderHistoryBreakdown-bs').attr('id', 'popCRMOrderHistoryBreakdown');

            if (sessionStorage.CrmType == 'crm' || sessionStorage.CrmType == 'revsalescrm') {
                if (divErpRevenue.find('#popOrderHistory div[data-role="content"]').length > 0){
                    divErpRevenue.find('#popOrderHistory div[data-role="content"]').appendTo(pageCRMOrderHistory);
                } else {
                    pageCRMOrderHistory.html('').append(parentdiv.find('#popOrderHistory .modal-body [data-role="content"]')).append(parentdiv.find('#popCRMOrderHistoryBreakdown'));
                }

                if ($('#popCRMOrderHistoryBreakdown').length > 0){
                    $('#popCRMOrderHistoryBreakdown').appendTo(pageCRMOrderHistory);
                }

                mOut.element = pageCRMOrderHistory;
            } else {
                if (divErpRevenue.find('#popOrderHistory').length == 0){
                    parentdiv.find('#popOrderHistory .modal-body').empty();
                    divErpRevenue.append(parentdiv.find('#popOrderHistory'));
                }

                if ($('#popCRMOrderHistoryBreakdown').length > 0){
                    $('#popCRMOrderHistoryBreakdown').appendTo(divErpRevenue);
                }

                if (pageCRMOrderHistory.find('div[data-role="content"]').length > 0){
                    pageCRMOrderHistory.find('div[data-role="content"]').appendTo(divErpRevenue.find('#popOrderHistory .modal-body'));
                }
                
                mOut.element = divErpRevenue;
            }
        }; // END mOut.Initialize = function() {

        mOut.Initialize();        

        mOut.popup = function(vParams) {
            mOut.Initialize();
            if (!IsGuid(vParams.CustomerID)){
                NewMessageBox('Select Customer first');
                return
            }
            mTransID = vParams.TransID;
            if (mTransID == null)
                mOut.element.find('[name=butSaveAsOrder]').text('Save Line Items with "New Order Quantities" As A New Sales Order');
            else
                mOut.element.find('[name=butSaveAsOrder]').text('Add Items with New Order Quantities to Cart');

            mCustomerID = vParams.CustomerID;
            $('#selShowOrderHistory').val($('#selInvoiceMadeWith').val())
            mOut.GetOrderHistory(vParams.CustomerID);
            mOut.element.find('.txtActionClientName').text(vParams.CustomerName);
            mOut.Success = vParams.success;

            if (CStr2(sessionStorage.CrmType) == '')
                $('#popOrderHistory').modal('show');
        }

        var dtOrderHistory, dtOrderHistoryBreakdown;
        mOut.GetOrderHistory = function (id) {
            mCustomerID = id;
            var tblDataTableOrderHistory = $('#tblDataTableOrderHistory'), rownum = 0;
            var url = '/CRMDashboard/GetOrderHistory?OrgID=' + sessionStorage.OrgID + '&ID=' + id + '&selectedweek=' + $('#selShowOrderHistory').val();
            mOut.element.find('#btnCRMMarkAsFollowedUp').hide();
            AjaxSend({
                url: url,
                success: function (e) {
                    var vRows = e.OrderHistory.Rows;
                    if (e.IsForFollowUp){
                        mOut.element.find('#btnCRMMarkAsFollowedUp').show();
                    }

                    if ($.fn.DataTable.isDataTable('#tblDataTableOrderHistory')) {
                        $('#tblDataTableOrderHistory').empty();
                        dtOrderHistory.destroy();
                        $('#tblDataTableOrderHistory thead').empty();
                    }

                    dtOrderHistory = tblDataTableOrderHistory.DataTable({
                        paging: false,
                        "pageLength": 20,
                        "bProcessing": false,
                        "ordering": false,
                        "bServerSide": false,
                        "data": vRows,
                        "autoWidth": false,
                        "columns": [
                            {
                                title: '#',
                                "data": function (data, type, full, meta) {
                                    rownum += 1;
                                    return rownum;
                                },
                                "width": "30px"
                            },
                            {
                                title: 'Product No.',
                                "data": function (data, type, full, meta) {
                                    return data.ProductNo;
                                }, width: '150px'
                            },
                            {
                                title: 'Product Name',
                                "data": function (data, type, full, meta) {
                                    return data.ProductDescription;
                                }
                            },
                            {
                                title: 'Ordered Qty.',
                                "data": function (data, type, full, meta) {
                                    return data.OrderQty == undefined ? '' : CInt2(data.OrderQty);
                                }, className: 'txt-right', width: '150px'
                            },
                            {
                                title: 'Invoiced Qty',
                                "data": function (data, type, full, meta) {
                                    return data.InvoiceQty == undefined ? '' : CInt2(data.InvoiceQty);
                                }, className: 'txt-right', width: '150px'
                            },
                            {
                                title: 'New Order Qty',
                                "data": function (data, type, full, meta) {
                                    return '<input class="form-control" name=txtNewOrderQty style="text-align:center" />';
                                }, className: 'txt-center', width: '150px'
                            }
                            //,{
                            //    "data": null,
                            //    "createdCell": function (td, cellData, rowData, row, col) {
                            //        $(td).html('<input data-role="none" type="number" class="form-control" />').click(function (e) {
                            //            e.stopPropagation();
                            //        })
                            //    }
                            //}
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": false }],
                        "sDom": '<"top">rt<"bottom"><"clear">',
                        //"stateSave": true
                        rowCallback: function(row, data, dataIndex){
                            $(row).data('data', data)
                                .addClass('dataRow')
                                .click(function(e) {
                                    if (!$(e.target).parent().is(row))
                                        return;

                                    if (dtOrderHistory.row(this).data() != undefined) {
                                        var data = dtOrderHistory.row(this).data();
                                        GetOrderHistoryBreakdown(id, data)
                                        mOut.element.find('#selShowOrderHistoryBreakdown').unbind('change').change(function () {
                                            GetOrderHistoryBreakdown(id, data);
                                        })
                                    }    
                                });
                        }, initComplete: function(){
                            tblDataTableOrderHistory.removeClass('dataTable');
                        }
                    })
                    
                    mOut.element.find('#txtSearchCRMOrderHistory').unbind('keydown').keydown(function () {
                        dtOrderHistory.search($.trim($(this).val())).draw();
                    })
                    mOut.element.find('#btnCRMMarkAsFollowedUp').unbind('click').click(function () {
                        AjaxSend({
                            url: '/CRMDashboard/MarkTransAsFollowedUp',
                            data: { CustomerID: mCustomerID },
                            success: function (e) {
                                if (e.Results == 'ok')
                                    NewMessageBox('Marked as followed up');
                            }
                        })
                    })

                    mOut.element.find('#selShowOrderHistory').unbind('change').change(function () {
                        mOut.GetOrderHistory(mCustomerID);
                    })

                    mOut.element.find('#btnCRMCopyOHToPlainText').unbind('click').click(function () {
                        var txtarea = $('<textarea id="txtCRMCopyTransToPlainText" style="margin-left:-100000px; position:fixed;">');

                        if (mOut.element.find('[data-role="content"]').find('#txtCRMCopyTransToPlainText').length == 0) {
                            mOut.element.find('[data-role="content"]').append(txtarea);
                        }

                        txtarea = mOut.element.find('#txtCRMCopyTransToPlainText');

                        var msg = 'ORDER HISTORY of ' + mOut.element.find('.txtActionClientName').text();
                        msg += '\n\nITEMS: \n';

                        for (var i = 0; i < vRows.length; i++){
                            msg +=  '\n' + (i + 1) + '.\n';
                            msg += 'Product No.: ' + vRows[i].ProductNo + ' \n';
                            msg += 'Product Name: ' + vRows[i].ProductDescription + ' \n';
                            msg += 'Ordered Qty: ' + CInt2(vRows[i].OrderQty) + ' \n';
                            msg += 'Invoiced Qty: ' + CInt2(vRows[i].InvoiceQty) + ' \n';
                        }

                        txtarea.val(msg).select();
                        document.execCommand("copy");
                        WebToast('Copied to clipboard');

                        AjaxSend({
                            url: '/CRMDashboard/TagTransAsFollowUp',
                            data: { CustomerID: mCustomerID },
                            success: function (e) {

                            }
                        })
                    })
                }
            })
        }

        function GetOrderHistoryBreakdown(id, data) {
            AjaxSend({
                url: '/CRMDashboard/GetOrderHistoryBreakdown?OrgID=' + sessionStorage.OrgID + '&ID=' + id + '&ProductID=' + data.ProductID + '&selectedweek=' + $('#selShowOrderHistoryBreakdown').val(),
                success: function (e) {
                    var tblDataTableOrderHistoryBreakdown = $('#tblDataTableOrderHistoryBreakdown'), rownumbr = 0;
                    vRows = e.Results.Rows;

                    $('#lblCRMProductID').html('<b>' + data.ProductNo + '</b>');
                    $('#lblCRMProductName').html('<b>' + data.ProductDescription + '</b>');

                    if ($.fn.DataTable.isDataTable('#tblDataTableOrderHistoryBreakdown')) {
                        dtOrderHistoryBreakdown.destroy();
                    }

                    dtOrderHistoryBreakdown = tblDataTableOrderHistoryBreakdown.DataTable({
                        "pageLength": 20,
                        "bProcessing": false,
                        "ordering": false,
                        "bServerSide": false,
                        "data": vRows,
                        "autoWidth": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    rownumbr += 1;
                                    return rownumbr;
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return $.format.date(new Date(data.InvoiceDate), 'MM-dd-yyyy');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return CStr2(data.FullTransNumber);
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return CInt2(data.TransType == 'Invoice' ? data.Quantity : data.OrderedQuantity);
                                }, "className": 'txt-right'
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.UnitMeasure;
                                }, "className": 'txt-center'
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return AccountingFormat(data.UnitPrice);
                                }, "className": 'txt-right'
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return AccountingFormat(data.SubTotal);
                                }, "className": 'txt-right'
                            }
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": false }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })

                    dtOrderHistoryBreakdown.on('click', 'tr', function () {
                        if (dtOrderHistoryBreakdown.row(this).data() != undefined) {
                            var data = dtOrderHistoryBreakdown.row(this).data();

                        }
                    });

                    mOut.element.find('#txtSearchCRMOrderHistoryBreakdown').unbind('keydown').keydown(function () {
                        dtOrderHistoryBreakdown.search($.trim($(this).val())).draw();
                    })

                    if (proj.IsBootstrapDesign()){
                        $('#popCRMOrderHistoryBreakdown').modal('show');
                    } else {
                        $('#popCRMOrderHistoryBreakdown').show();
                    }                    
                }
            })
        }

        mOut.element.find('[name=butSaveAsOrder]').click(function() {
            var pJson = {
                ID: mTransID,
                CustomerID: mCustomerID,
                FowstID: sessionStorage.FowstID,
                Products: []
            };
            var pTRs = mOut.element.find('tr.dataRow');
            for (var pTR of pTRs.toArray()) {
                pTR = $(pTR);
                var pDR = pTR.data('data');
                var pTxtNewOrderQty = pTR.find('[name=txtNewOrderQty]');
                var pQty = pTxtNewOrderQty.val().trim();
                if (pQty == '')
                    continue;
                if (!IsNumeric(pQty)) {
                    NewMessageBox('The new order quantity for ' + pDR.ProductDescription + ' is invalid.', 'Invalid Qty');
                    return;
                }
                pQty = CDec2(pQty);
                if (pQty == 0)
                    continue;
                pJson.Products.push({
                    ProductID: pDR.ProductID,
                    Qty: pQty
                });
            }

            if (pJson.Products.length == 0) {
                NewMessageBox('Please enter at least 1 new order qty.', 'No qty');
                return;
            }

            AjaxSend({
                url: '/SalesMenu/CreateSalesOrder',
                data: pJson,
                success: function(vData) {
                    if (vData.Error) {
                        NewMessageBox(vData.Error, 'Failed');
                        return;
                    } else if (IsGuid(vData.Result)) {
                        if (ReadyGuid(vData.Result) == ReadyGuid(mTransID)) {
                            NewMessageBox('The transaction was saved successfully.', 'Saved');
                            mOut.Success();
                        } else {
                            NewMessageBox('The new order was saved successfully.', 'Saved');
                            sessionStorage.removeItem('CrmType');
                            var win = window.open(location.origin + '/Erp/OrderProcessing' + location.search + '&IsReadOnly=0&ID=' + vData.Result,
                                '_blank',
                                'width=' + window.innerWidth + ',height=' + window.innerHeight);

                            var timer = setInterval(function () {
                                if (win.closed) {
                                    sessionStorage.CrmType = 'revsalescrm'
                                }
                            }, 500);
                            //win.onload = function () {
                            //    function LoadRecord() {
                            //        if (win.Z9Q3C9ASDA)
                            //            win.Z9Q3C9ASDA.ShowSalesOrder({
                            //                ID: vData.Result,
                            //                ReadOnly: false
                            //            });
                            //        else
                            //            setTimeout(LoadRecord, 1000);
                            //    }
                            //    LoadRecord();    
                            //}
                        }
                    }
                }
            });
        });        

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});