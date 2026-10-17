require(['common'], function () {
    $(document).ready(function () {
        var htmlCrm = '', mQuoteController, mSalesOrderController, mStockTransferController,
            mOrderFulfillmentWorkLoaded, mInvoiceController, mDeliveryTrackingController, htmlCreditAndCollection,
            isLoadPriceCanvass = false;
        var mRevenueDashboardDateFrom = new Date();
        var mRevenueDashboardDateTo = new Date();

        $('#butRevenueDashboardDateRange').click(function () {
            DualCalendar({
                Click: function (e) {
                    mRevenueDashboardDateFrom = e.From;
                    mRevenueDashboardDateTo = e.To;
                    $('#butRevenueDashboardDateRange span:first').text('Date Range: ' + FormatDate(mRevenueDashboardDateFrom) + ' to ' + FormatDate(mRevenueDashboardDateTo));
                    ReloadRevenueDashboard();
                }
            }, new Date(), new Date())
        });

        $('#btnRevPOS').click(function () {
            require(['/Scripts/sources/ERP/OrderProcessing/work/model.js?v=' + assetversion], function () {
                var pModel = new ERP.OrderProcessing.work.model();
                pModel.HandleWebPosDeviceFowst({
                    Success: function () {
                        sessionStorage.IsWebPos = true;
                        location.href = '/SalesMenu/WebPOSNew?OrgID=' + sessionStorage.OrgID + '&IsMobile=0'
                    },
                    Fail: function (vMsg) {
                        NewMessageBox(vMsg, 'Unauthorized Web POS Device');
                    }
                });
            });
        });

        $('#btnRevSalesCrm').click(function () {
            AddBreadCrumb({
                text: 'Sales CRM', Click: function () {
                    $('#divCustomerSupport, #pageCRMMainDashboard').pageshow();
                }
            }, '.ItemsBreadCrumbs')
            sessionStorage.CrmType = 'revsalescrm';
            sessionStorage.SystemAdminFor = 'crm';
            sessionStorage.IsCrm = true;
            if (CStr2(htmlCrm) == '') {
                CallPage('#divCustomerSupport', htmlCrm, '/CRMDashboard/db', function (e) {
                    htmlCrm = e;

                    require(['/Scripts/sources/ERP/OrderProcessing/orderhistory/controller.js?v=' + assetversion])
                })
            } else {
                $('#divCustomerSupport, #pageCRMMainDashboard').pageshow();
            }
        })

        $('#btnRevQuotation').click(function () {
            PromptUserAccess('Sales Quotation (EAS)', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion], function () {
                        $('#divQuotation').pageshow();
                        if (mQuoteController == null)
                            mQuoteController = new ERP.OrderProcessing.work.controller({
                                TransType: 'Quote'
                            });
                        else
                            mQuoteController.Initialize();
                        AddBreadCrumb({
                            text: 'Quotation', Click: function () {
                                $('#divQuotation').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#btnRevOrderProcessing').click(function () {
            PromptUserAccess('Sales Order (EAS)', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion], function () {
                        $('#divOrderProcessing, #divOrderProcessingMain').pageshow();
                        if (mSalesOrderController == null) {
                            mSalesOrderController = new ERP.OrderProcessing.work.controller({
                                TransType: 'SalesOrder'
                            });
                            require(['promise!/Scripts/sources/ERP/OrderProcessing/dashboard/controller.js?v=' + assetversion]);
                        } else
                            mSalesOrderController.Initialize();
                        AddBreadCrumb({
                            text: 'Order Processing', Click: function () {
                                $('#divOrderProcessing, #divOrderProcessingMain').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                        //mSalesOrderController.element.show();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butStockTransfer').click(function () {
            PromptUserAccess('Sales Invoice (EAS)', {
                OnClickOKSuccess: function () {
                    var vParams = $.extend({
                        ID: null
                    }, vParams);
                    require([
                    'promise!/Scripts/sources/ERP/Receiving/work/controller.js?v=' + assetversion,
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
                    ], function () {
                        $('#divStockTransfer, #divStockTransferMain').pageshow();
                        if (mStockTransferController == null) {
                            mStockTransferController = new ERP.Receiving.work.controller({
                                TransType: 'StockTransfer'
                            });
                        } else
                            mStockTransferController.Initialize();
                        AddBreadCrumb({
                            text: 'Stock Transfer', Click: function () {
                                $('#divStockTransfer, #divStockTransferMain').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                        if (vParams.ID)
                            mStockTransferController.LoadTrans({
                                ID: vParams.ID
                            });
                        if (vParams.ReadOnly)
                            mStockTransferController.ReadOnly();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#btnRevOrderFulfillment').click(function () {
            $('#divOrderFulfillment, #divOrderFulfillmentMain').pageshow();
            AddBreadCrumb({
                text: 'Fulfillment Center', Click: function () {
                    $('#divOrderFulfillment, #divOrderFulfillmentMain').pageshow();
                }
            }, '.ItemsBreadCrumbs');
            if (mOrderFulfillmentWorkLoaded){
                $('#popupSOPAReport .modal-body > div').appendTo($('#divOrderFulfillmentWork'));
                $('#divOrderFulfillmentWork').show();
            } else
                PromptUserAccess('Access Order Fulfillment Module*', {
                    OnClickOKSuccess: function () {
                        GetSalesOrderPage('fc');
                        mOrderFulfillmentWorkLoaded = true;
                    }
                });
        });

        $('#btnRevInvoicing').click(function () {
            PromptUserAccess('Sales Invoice (EAS)', {
                OnClickOKSuccess: function () {
                    var vParams = $.extend({
                        ID: null,
                        ShowProductSuppliersColumn: false
                    }, vParams);
                    require([
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
                    ], function () {
                        $('#divInvoicing, #divInvoicingMain').pageshow();
                        if (mInvoiceController == null) {
                            mInvoiceController = new ERP.OrderProcessing.work.controller({
                                TransType: 'Invoice',
                                ShowProductSuppliersColumn: vParams.ShowProductSuppliersColumn
                            });
                            require(['promise!/Scripts/sources/ERP/Invoicing/settings/controller.js?v=' + assetversion]);
                        } else
                            mInvoiceController.Initialize();
                        AddBreadCrumb({
                            text: 'Invoicing', Click: function () {
                                $('#divInvoicing, #divInvoicingMain').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                        if (vParams.ID)
                            mInvoiceController.LoadTrans({
                                ID: vParams.ID
                            });
                        if (vParams.ReadOnly)
                            mInvoiceController.ReadOnly();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butDeliveryTracking').click(function () {
            var vParams = $.extend({
                ID: null
            }, vParams);

            require([
                    'promise!/Scripts/sources/ERP/DeliveryTracking/controller.js'
            ], function () {
                $('#divDeliveryTracking, #divDeliveryTrackingMain').pageshow();
                if (mDeliveryTrackingController == null)
                    mDeliveryTrackingController = new ERP.DeliveryTracking.controller();
                AddBreadCrumb({
                    text: 'Delivery Tracking', Click: function () {
                        $('#divDeliveryTracking, #divDeliveryTrackingMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                if (vParams.ID)
                    mInvoiceController.LoadTrans({
                        ID: vParams.ID
                    });
                if (vParams.ReadOnly)
                    mInvoiceController.ReadOnly();
            });
        });

        $('#btnRevCreditAndCollection').click(function () {
            PromptUserAccess('Credit and Collection (EAS)', {
                OnClickOKSuccess: function () {
                    var mode = 'AR';

                    AddBreadCrumb({
                        text: 'Credit and Collection', Click: function () {
                            $('#divCreditAndCollection').pageshow();
                        }
                    }, '.ItemsBreadCrumbs');

                    if (htmlCreditAndCollection != undefined) {
                        $('#divCreditAndCollection').pageshow();
                        $('#divMainDashboardOP').show();
                        $('#tabCacWork').opentabmenu();
                        Cac.Show({
                            Mode: mode
                        });
                    } else {
                        CallPage('#divCreditAndCollection', htmlCreditAndCollection, '/CreditAndCollection', function (e) {
                            $('#divCreditAndCollection nav').remove();
                            $('#divCreditAndCollection #divMainCac').removeClass('container-fluid');
                            require(['promise!/Scripts/sources/ERP/OrderProcessing/dashboard/controller.js?v=' + assetversion], function () {
                                $('#pageCac').pageshow();
                                htmlCreditAndCollection = e;

                                function AttemptShow() {
                                    if (typeof Cac !== 'undefined') {
                                        Cac.Show({
                                            Mode: mode
                                        });
                                    } else
                                        setTimeout(AttemptShow, 100);
                                }
                                setTimeout(AttemptShow, 100);

                                $('#divMainDashboardOP').show();
                            });
                        });
                    }
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butRevDeposits').click(function () {
            PromptUserAccess('Deposits (EAS)', {
                OnClickOKSuccess: function () {
                    $('#divErpDeposits').pageshow();
                    AddBreadCrumb({
                        text: 'Deposits', Click: function () {
                            $('#divErpDeposits').pageshow();
                        }
                    }, '.ItemsBreadCrumbs');
                    require(['promise!/Scripts/sources/ERP/Deposits/main/controller.js'], function () {
                        $('#divErpDeposits').pageshow();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butErpSerialNumberTracking').click(function () {
            require(['promise!/Scripts/sources/ERP/ActivityTracker/controller.js?v=' + assetversion], function () {
                ERP.ActivityTracker.controller.popup({

                });
            });
        });

        $('#btnRevenuePriceCanvassing').click(function () {
            PromptUserAccess('Access Competitor Price Canvassing Module*', {
                OnClickOKSuccess: function () {
                    sessionStorage.PriceCanvassType = "competitor";
                    $('#divRevenuePriceCanvass, #divRevenuePriceCanvassMain, #divPriceCanvassPage').pageshow();
                    require(['promise!/Scripts/sources/ERP/PriceCanvassing/main/controller.js?v=' + assetversion], function () {
                        $('#divPriceCanvassMain').empty();
                        if (!isLoadPriceCanvass) {
                            $('#divRevenuePriceCanvassMain').empty();
                            ERP.PriceCanvassing.main.controller();
                            isLoadPriceCanvass = true;
                        } else {
                            ERP.PriceCanvassing.main.controller().LoadPage();
                        }

                        AddBreadCrumb({
                            text: 'Competitor Price Canvass', Click: function () {
                                $('#divRevenuePriceCanvass, #divRevenuePriceCanvassMain, #divPriceCanvassPage').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                    });
                }
            });
        });

        $('#butErpAutoPrintInvoice').click(function () {
            proj.AutoPrintEnrollment();
        });

        $('#btnConsignee').click(function () {
            require(['promise!/Scripts/sources/ERP/Consignee/main/controller.js'], function () {
                $('#divErpRevenue, #divConsignee').pageshow();
                AddBreadCrumb({
                    text: 'Consignee', Click: function () {
                        $('#divErpRevenue, #divConsignee').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
            });
        })

        $('#butReportRevenueSalesQuotes').click(function () {
            ShowBigReport({
                TransType: 'Quote'
            });
        });

        $('#butReportRevenueSalesOrders').click(function () {
            ShowBigReport({
                TransType: 'SalesOrder',
                ExcludeBackorders: true
            });
        });

        $('#butReportRevenueBackOrders').click(function () {
            ShowBigReport({
                TransType: 'SalesOrder',
                BackordersOnly: true
            });
        });

        $('#butReportRevenueSalesInvoices').click(function () {
            ShowBigReport({
                TransType: 'Invoice'
            });
        });

        $('#butReportRevenueGrossProfitabilityValidation').click(function () {
            require(['/Scripts/sources/ERP/ProfitabilityValidation.js?crc=' + assetversion], function () {
                ERP.ProfitabilityValidation.show();
            })
        });

        $('#btnReportNetSales').click(function () {
            var win = window.open('/DashboardsAndReports?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&v=' + assetversion,
                                        '_blank',
                                        'width=' + screen.availWidth + ',height=' + screen.availHeight);
        })

        $('#btnReportCac').click(function () {
            require(['promise!/Scripts/sources/ERP/CollectionReport/controller.js?v=' + assetversion], function () {
                ERP.CollectionReport.Popup();
            });
        })

        
        $('#btnReportSopa').click(function () {
            GetSalesOrderPage('popup');
            $('#popupSOPAReport').modal('show');
        });

        $('#butReportProfit').click(function () {
            require(['promise!/Scripts/sources/ERP/ProfitReport/controller.js?v=' + assetversion], function () {
                ERP.ProfitReport.controller.popup();
            });
        });

        $('#btnReportCustomerAdmin').click(function () {
            $('#popupCustomerAdminApproval').modal('show');
            $('#tabVendorPortal').opentabmenu();
            GetCustomerContactPersonRequestList();
        })

        $('#txtSearchCustomerAdmin').keydown(function (e) {
            if (e.which == 13) {
                GetCustomerContactPersonRequestList($(this).val())
            }
        })

        $('#btnReportReprintRequests').click(function () {
            require(['/Scripts/sources/ERP/UnapprovedReprintRequests/controller.js?crc=' + assetversion], function () {
                ERP.UnapprovedReprintRequests.controller.show();
            })
        });

        $('#butReportPriceUpdateRequests').click(function() {
            require(['/Scripts/sources/ERP/PriceUpdateRequests/controller.js?crc=' + assetversion], function() {
                ERP.PriceUpdateRequests.controller.show();
            });
        });

        $('#butSalesReportByCustomerAndSupplierProducts').click(function () {
            PromptUserAccess('Cross Tab Report', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Reports/crossTab/controller.js'], function () {
                        ERP.Reports.crossTab.controller.Show('SalesReportByCustomerAndSupplierProducts');
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butSalesReportByProductQuantity').click(function () {
            PromptUserAccess('Cross Tab Report', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Reports/crossTab/controller.js'], function () {
                        ERP.Reports.crossTab.controller.Show('SalesReportByProductMeasurementQuantity');
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butSalesReportByProductQuantityPerMonth').click(function () {
            PromptUserAccess('Cross Tab Report', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Reports/crossTab/controller.js'], function () {
                        ERP.Reports.crossTab.controller.Show('SalesReportByProductQuantityPerMonth');
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butSalesReportByProductBaseQuantity').click(function () {
            PromptUserAccess('Cross Tab Report', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Reports/crossTab/controller.js'], function () {
                        ERP.Reports.crossTab.controller.Show('SalesReportByProductBaseQuantity');
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butInventoryMovementByFOWST').click(function () {
            PromptUserAccess('Cross Tab Report', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Reports/crossTab/controller.js'], function () {
                        ERP.Reports.crossTab.controller.Show('InventoryMovementByFOWST');
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        var htmlMcrSopa = '';
        function GetSalesOrderPage(type) {
            if (htmlMcrSopa == '') {
                AjaxSend({
                    url: '/DashboardsAndReports/SalesOrder?OrgID=' + sessionStorage.OrgID,
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        htmlMcrSopa = vData;
                        if (type == 'fc') {
                            $('#divOrderFulfillmentWork').empty();
                            $('#divOrderFulfillmentWork').append(htmlMcrSopa).trigger('create');
                        } else {
                            $('#popupSOPAReport .modal-body').append(htmlMcrSopa);
                        }

                        mOrderFulfillmentWorkLoaded = true;
                    }
                });
            } else {
                if (type == 'fc') {
                    $('#popupSOPAReport .modal-body > div').appendTo($('#divOrderFulfillmentWork'));
                } else {
                    $('#divOrderFulfillmentWork > div').appendTo($('#popupSOPAReport .modal-body'));
                }
            }
        }

        var tableCustomerContactPersonAdmin;
        function GetCustomerContactPersonRequestList(keywords) {
            var tbodyCustomerContactPersonAdmin = $('#tbodyCustomerContactPersonAdmin');

            tbodyCustomerContactPersonAdmin.empty().append($('<tr>').append($('<td colspan="5" style="text-align:center">').text('Getting data ... please wait')))
            AjaxSend({
                url: 'OrganizationShort/GetCustomerContactPersonRequestList',
                data: { keywords: $.trim(CStr2(keywords)) },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var ctr = 0;

                    tbodyCustomerContactPersonAdmin.empty()

                    if ($.fn.DataTable.isDataTable("#tableCustomerContactPersonAdmin")) {
                        //check if datatable is created 
                        $('#tableCustomerContactPersonAdmin').DataTable().clear().destroy();
                    }

                    tableCustomerContactPersonAdmin = $('#tableCustomerContactPersonAdmin').DataTable({
                        retrieve: true,
                        "bdestroy": true,
                        "pageLength": 20,
                        "data": vRows,
                        "autoWidth": false,
                        "ordering": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return ctr + '.';
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.OrgName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.AdminName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return $.format.date(new Date(data.StatusLastUpdated), 'MM-dd-yyyy hh:mm a');
                                }
                            },
                            {
                                data: null,
                                createdCell: function (td, cellData, rowData, row, col) {
                                    var b = $('<button data-role="none" class="btn btn-success">')
                                        .attr('title', 'Approve')
                                        .append('<span class="bi-hand-thumbs-up-fill"></span>')
                                        .click(function () {
                                            rowData.Site = 'Online Store';
                                            SubmitContactPersonRequest(rowData, 'Approved');
                                        });
                                    $(td).css('width', '50px').html(b);
                                }
                            },
                            {
                                data: null,
                                createdCell: function (td, cellData, rowData, row, col) {
                                    var b = $('<button data-role="none" class="btn btn-danger">')
                                        .attr('title', 'Disapprove')
                                        .append('<span class="bi-hand-thumbs-down-fill"></span>')
                                        .click(function () {
                                            rowData.Site = 'Online Store';
                                            SubmitContactPersonRequest(rowData, 'Disapproved');
                                        });
                                    $(td).css('width', '50px').html(b);
                                }
                            }
                        ],
                        "initComplete": function (row, data) {
                            $('#tableCustomerContactPersonAdmin').removeClass('dataTable');
                        },
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                        "responsive": true,
                        "columnDefs": [{ "orderable": false }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })
                }
            })
        }

        function SubmitContactPersonRequest(data, status) {
            PromptUserAccess('Allow approval of contact person\'s request to access Online Store?', {
                OnClickOKSuccess: function () {
                    NewMessageBox('Are you sure want to <b>' + status.replace('d', '').toLowerCase() + '</b> request of <b>' + data.AdminName + '</b> for access to ' + data.Site + '?', 'Message', [
                        {
                            Text: 'Yes', Click: function (e) {
                                AjaxSend({
                                    url: 'OrganizationShort/ApproveContactPersonRequest',
                                    data: { ID: data.ID, Status: status },
                                    success: function (e) {
                                        if (e.Results == 'saved') {
                                            WebToast('Request of ' + data.AdminName + ' has been ' + status.toLowerCase(), 'top');
                                            GetCustomerContactPersonRequestList();
                                            GetNumberContactPersonForApproval();
                                        } else {
                                            NewMessageBox(e.Results, 'Error');
                                        }
                                    }
                                })
                            }
                        }, { Text: 'No' }
                    ])
                },
                Message: 'You\'re not allowed to approve contact person\'s request to access ' + data.Site
            });
        }

        function ShowBigReport(vParams) {
            var pTitle;
            var pColumns = [];

            if (vParams.TransType == 'Quote') {
                pTitle = 'Quotations';
                pColumns = [
                    {
                        Label: 'Quotation. No.', // ok
                        Column: 'QuotationNo',
                        className: 'dt-body-center'
                    }
                ];
            } else if (vParams.TransType == 'SalesOrder') {
                pTitle = 'Sales Order';
                pColumns = [
                    {
                        Label: 'Sales Order No.', // ok
                        Column: function (vDR) {
                            var pOut = CStr2(vDR.MIN).trim();
                            if (pOut != '')
                                pOut = vDR.MIN + '-';
                            if (vDR.TransType == 'StockTransfer')
                                pOut = 'STR-' + pOut + vDR.StrNum;
                            else
                                pOut = 'SO-' + pOut + vDR.SalesOrderNo;

                            return pOut;
                        },
                        className: 'dt-body-center'
                    }
                ];
            } else if (vParams.TransType == 'Invoice') {
                pTitle = 'Invoice';
                pColumns = [
                    {
                        Label: 'Invoice No.',
                        Column: function (vDR) {
                            var pOut = '';
                            if (CStr2(vDR.FowstWideInvoiceNumber).trim() != '')
                                pOut = vDR.FowstWideInvoiceNumber;
                            else if (CStr2(vDR.MIN).trim() != '')
                                pOut = vDR.MIN + '-' + vDR.InvoiceNumber;
                            else
                                pOut = vDR.InvoiceNumber;
                            if (CStr2(vDR.DocumentName).trim() == '')
                                return 'SI-' + pOut;
                            else
                                return vDR.DocumentName + '-' + pOut;
                        },
                        className: 'dt-body-center'
                    }
                ];
            } else if (vParams.TransType == 'PurchaseOrder') {
                pTitle = 'Purchase Order';
                pColumns = [
                    {
                        Label: 'PO No.',
                        Column: function (vDR) {
                            var pOut = '';
                            if (CStr2(vDR.MIN).trim() != '')
                                pOut = vDR.MIN + '-';
                            if (!IsNumeric(vDR.PurchaseOrderNo))
                                vDR.PurchaseOrderNo = 0;
                            return 'PO-' + pOut + vDR.PurchaseOrderNo;
                        },
                        className: 'dt-body-center'
                    },
                    {
                        Label: 'Vendor',
                        Column: 'CompanyName'
                    },
                    {
                        Label: 'Amount',
                        Column: function (vDR) {
                            return AccountingFormat(vDR.Total);
                        }
                    }
                ];
            } else if (vParams.TransType == 'Canvass') {
                pTitle = 'Price Canvass';
                pColumns = [
                    {
                        Label: 'PC No.',
                        Column: function (vDR) {
                            var pOut = '';
                            if (CStr2(vDR.MIN).trim() != '')
                                pOut = vDR.MIN + '-';
                            if (!IsNumeric(vDR.PurchaseOrderNo))
                                vDR.PurchaseOrderNo = 0;
                            return 'PO-' + pOut + vDR.PurchaseOrderNo;
                        }
                    },
                    {
                        Label: 'Canvass Name',
                        Column: 'CanvassName'
                    },
                    {
                        Label: 'Vendor',
                        Column: 'CompanyName'
                    },
                    {
                        Label: 'Amount',
                        Column: function (vDR) {
                            return AccountingFormat(vDR.Total);
                        }
                    }
                ];

            }

            pColumns = pColumns.concat([
                {
                    Label: 'Date Created', // ok
                    Column: 'DateCreated',
                    className: 'dt-body-center'
                },
                {
                    Label: 'Last Modified', // ok
                    Column: 'LastUpdated',
                    className: 'dt-body-center'
                },
                {
                    Label: 'Customer', // ok
                    Column: 'CompanyName'
                }
            ]);

            if (vParams.TransType == 'Quote') {
                pColumns = pColumns.concat([
                    {
                        Label: 'Transaction Status', // ok
                        Column: function (vDR) {
                            if (CDec2(vDR.QuotationNo) > 0)
                                return 'Finalized';

                            return 'Draft';
                        },
                        className: 'dt-body-center'
                    },
                    {
                        Label: 'Transaction Value',
                        Column: function (vDR) {
                            return AccountingFormat(vDR.Total);
                        },
                        className: 'dt-body-center'
                    }
                ]);
            } else if (vParams.TransType == 'SalesOrder') {
                pColumns = pColumns.concat([
                    {
                        Label: 'Transaction Status', // ok
                        Column: function (vDR) {
                            if (CDec2(vDR.SalesOrderNo) > 0)
                                return 'Finalized';

                            return 'Draft';
                        },
                        className: 'dt-body-center'
                    },
                    {
                        Label: 'Transaction Value',
                        Column: function (vDR) {
                            return AccountingFormat(vDR.OrderedAmount);
                        },
                        className: 'dt-body-center'
                    }
                ]);
            } else if (vParams.TransType == 'Invoice') {
                pTitle = 'Invoice';
                pColumns = pColumns.concat([
                    {
                        Label: 'Transaction Status', // ok
                        Column: function (vDR) {
                            if (CDec2(vDR.InvoiceNumber) > 0)
                                return 'Finalized';

                            return 'Draft';
                        },
                        className: 'dt-body-center'
                    },
                    {
                        Label: 'Transaction Value',
                        Column: function (vDR) {
                            return AccountingFormat(vDR.ServedAmount);
                        },
                        className: 'dt-body-center'
                    }
                ]);
            }

            pColumns = pColumns.concat([
                {
                    Label: 'Modified By',
                    Column: 'EmployeeFullName',
                    className: 'dt-body-center'
                }
            ]);

            if ((vParams.TransType == 'Invoice')
                || (vParams.TransType == 'SalesOrder')) {
                pColumns.push({
                    Label: 'Action',
                    Column: function () {
                        return '<div name="divAction" />';
                    }
                });
            }

            function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {
                if (vLastModifiedFrom == null)
                    vLastModifiedFrom = new Date();
                if (vLastModifiedTo == null)
                    vLastModifiedTo = new Date();

                BrowsePopup('TransInvoices', pTitle,
                    [
                        {
                            StyleFormat: 'table',
                            AutoSearch: true,
                            DisplayText: pColumns,
                            Click: vParams.Click,
                            Validation: vParams.Validation,
                            ShowLastModifiedFilter: true,
                            rowCallback: function (row, data, dataIndex) {
                                var pCell = $(row).find('[name=divAction]');
                                if (pCell.length == 0)
                                    return;
                                var pLink = $('<a>')
                                    .text('Preview')
                                    .attr('href', '#')
                                    .click(function (e) {
                                        e.preventDefault();
                                        var pParams = {
                                            IsReadOnly: 1,
                                            ID: data.ID,
                                            OrgID: sessionStorage.OrgID
                                        };
                                        if (vParams.TransType == 'Invoice')
                                            pParams.ShowInvoice = 1;
                                        else if (vParams.TransType == 'SalesOrder')
                                            pParams.ShowSalesOrder = 1;

                                        var pUrl = MergeUrlParameters(pParams, location.origin + location.pathname);
                                        var win = window.open(pUrl,
                                            '_blank',
                                            'width=' + window.innerWidth + ',height=' + window.innerHeight);
                                        return false; // para DILI magclose ang popup
                                    });
                                pCell.append(pLink);
                            },
                            Sorting: true,
                            LastModifiedFrom: vLastModifiedFrom,
                            LastModifiedTo: vLastModifiedTo,
                            //OrderBy: [1, 'asc']
                            LastModifiedFilterChange: function (e) {
                                ShowPopup(e.From, e.To);
                            }
                        }
                    ], $.extend({
                        TransType: vParams.TransType,
                        AnyZero: 1,
                        AllowNoItems: true,
                        LastUpdatedDateFrom: FormatDate(vLastModifiedFrom, { Format: 'MMM d, yyyy' }),
                        LastUpdatedDateTo: FormatDate(vLastModifiedTo, { Format: 'MMM d, yyyy' }),
                        BackordersOnly: vParams.BackordersOnly,
                        ExcludeBackorders: vParams.ExcludeBackorders
                        //LastUpdatedDateFrom: '2020/09/01', // for debugging only
                        //LastUpdatedDateTo: '2020/09/30' // for debugging only
                    }, vParams.QueryParams)
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            ShowPopup();
        }

        function LoadRequiredData() {
            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    clearInterval(tempinterval)
                    $('.ItemsBreadCrumbs').empty();
                    AddBreadCrumb({
                        text: 'Home', Click: function () {
                            $('#divErpRevenue, #divRevMainMenu').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                }
            }, 100)

            $('#butRevenueDashboardDateRange span:first').text('Date Range: ' + FormatDate(mRevenueDashboardDateFrom) + ' to ' + FormatDate(mRevenueDashboardDateTo));
            ReloadRevenueDashboard();
        }

        function ReloadRevenueDashboard() {
            GetSalesQuotes();
            GetSalesOrders();
            GetBackOrders();
            GetSalesInvoices();
            GetTotalGrossAndDiscounts();
            GetCollectionReport();
            GetSopa();
            GetNumberContactPersonForApproval();
            GetGrossProfitabilityValidation();
        }

        function GetTotalGrossAndDiscounts() {
            $('#lblNetSalesFooter').text($.format.date(new Date(), 'MMM dd, yyyy'));
            AjaxSend({
                url: 'DashboardsAndReports/GetTotalGrossAndDiscounts',
                data: {
                    GroupBy: 'All',
                    Fowst: 'All',
                    From: mRevenueDashboardDateFrom,
                    To: mRevenueDashboardDateTo,
                    IsRawData: 0
                },
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        $('#lblNetSalesBody').text(AccountingFormat(CDec2(vData.Results.Rows[0].GrossSales) - CDec2(vData.Results.Rows[0].TotalDiscounts) - CDec2(vData.Results.Rows[0].TotalReturns)));
                    } else {
                        $('#lblNetSalesBody').text('0');
                    }
                }
            });
        }

        function GetSalesQuotes() {
            AjaxSend({
                url: 'DashboardsAndReports/GetSalesQuotes',
                data: {
                    DateFrom: mRevenueDashboardDateFrom,
                    DateTo: mRevenueDashboardDateTo
                },
                success: function (vData) {
                    $('#lblReportRevenueSalesQuotesBody').empty().append($('<span style="font-size:12px">').text('Finalized SQ Value / Count'))
                        .append('<br>')
                        .append('P' + AccountingFormat(vData.FinalizedAmount) + ' (' + AccountingFormat(vData.FinalizedCount, 0) + ' finalized)');

                    $('#lblReportRevenueSalesQuotesFooter').empty().append($('<span style="font-size:12px">').text('Draft SQ Value / Count'))
                        .append('<br>')
                        .append('P' + AccountingFormat(vData.DraftAmount) + ' (' + AccountingFormat(vData.DraftCount, 0) + ' draft)');
                }
            });
        }

        function GetSalesOrders() {
            AjaxSend({
                url: 'DashboardsAndReports/GetSalesOrders',
                data: {
                    DateFrom: mRevenueDashboardDateFrom,
                    DateTo: mRevenueDashboardDateTo
                },
                success: function (vData) {
                    $('#lblReportRevenueSalesOrdersBody').empty().append($('<span style="font-size:12px">').text('Finalized SO Value / Count'))
                        .append('<br>')
                        .append('P' + AccountingFormat(CDec2(vData.FinalizedAmount) + CDec2(vData.BackorderFinalizedAmount)) + ' (' + AccountingFormat(CInt2(vData.FinalizedCount) + CInt2(vData.BackorderFinalizedCount), 0) + ' finalized)');

                    $('#lblReportRevenueSalesOrdersFooter').empty().append($('<span style="font-size:12px">').text('Draft SO Value / Count'))
                        .append('<br>')
                        .append('P' + AccountingFormat(CDec2(vData.DraftAmount)) + ' (' + AccountingFormat(CInt2(vData.DraftCount), 0) + ' draft)');
                }
            });
        }

        function GetBackOrders() {
            AjaxSend({
                url: 'DashboardsAndReports/GetSalesOrders',
                data: {
                    DateFrom: mRevenueDashboardDateFrom,
                    DateTo: mRevenueDashboardDateTo
                },
                success: function (vData) {
                    $('#lblReportRevenueBackOrdersFooter').empty().append($('<span style="font-size:12px">').text('Draft SO Value / Count'))
                        .append('<br>')
                        .append('P' + AccountingFormat(CDec2(vData.BackorderDraftAmount)) + ' (' + AccountingFormat(CInt2(vData.BackorderDraftCount), 0) + ' draft)');
                }
            });
        }

        function GetSalesInvoices() {
            AjaxSend({
                url: 'DashboardsAndReports/GetSalesInvoices',
                data: {
                    DateFrom: mRevenueDashboardDateFrom,
                    DateTo: mRevenueDashboardDateTo
                },
                success: function (vData) {
                    $('#lblReportRevenueInvoicesBody').empty().append($('<span style="font-size:12px">').text('Finalized SI Value / Count'))
                        .append('<br>')
                        .append('P' + AccountingFormat(vData.FinalizedAmount) + ' (' + AccountingFormat(vData.FinalizedCount, 0) + ' finalized)');

                    $('#lblReportRevenueInvoicesFooter').empty().append($('<span style="font-size:12px">').text('Draft SI Value / Count'))
                        .append('<br>')
                        .append('P' + AccountingFormat(vData.DraftAmount) + ' (' + AccountingFormat(vData.DraftCount, 0) + ' draft)');
                }
            });
        }

        function GetCollectionReport() {
            var pData = {
                "EncodingDateRange": $.format.date(mRevenueDashboardDateFrom, 'MMM dd, yyyy') + ' to ' + $.format.date(mRevenueDashboardDateTo, 'MMM dd, yyyy'),
                "DoNotFilterEncodingDate": false,
                "MaturityDateRange": $.format.date(mRevenueDashboardDateFrom, 'MMM dd, yyyy') + ' to ' + $.format.date(mRevenueDashboardDateTo, 'MMM dd, yyyy'),
                "DoNotFilterMaturityDate": true,
                "DoNotFilterDepositDate": 1,
                "IncludeCash": true,
                "IncludeChecks": true,
                "IncludeCreditCards": true,
                "IncludeDebitCards": true,
                "IncludeSrv": 0,
                "IncludeDeposited": 1,
                "IncludeReturned": 1,
                "IncludeUndeposited": 1
            }
            AjaxSend({
                url: '/Browser/CustomerPaymentReport',
                data: pData,
                success: function (vDataCollection) {
                    var vRowsCollection = vDataCollection.Result.Rows;

                    $('#lblCacBody').empty().append($('<span style="font-size:12px">').text('Today\'s Total Collected'))
                            .append('<br>');

                    if (vRowsCollection.length > 0) {
                        var totalcollection = 0;
                        for (var i = 0; i < vRowsCollection.length; i++) {
                            totalcollection += CDec2(vRowsCollection[i].AmountPaid);
                        }
                        $('#lblCacBody').append(AccountingFormat(totalcollection))
                    } else {
                        $('#lblCacBody').append('0');
                    }

                    $('#lblCacBody').append('<br>');

                    AjaxSend({
                        url: 'CreditAndCollection/GetCustomerWithCredit',
                        success: function (vDataCredit) {
                            var vRowsCredit = vDataCredit.Results.Rows;
                            $('#lblCacBody').append($('<span style="font-size:12px">').text('Total Credit'))
                                .append('<br>');

                            if (vRowsCredit.length > 0) {
                                var totalcredit = 0;
                                for (var i = 0; i < vRowsCredit.length; i++) {
                                    totalcredit += CDec2(vRowsCredit[i].NetInvoiceBalance);
                                }
                                $('#lblCacBody').append(AccountingFormat(totalcredit))
                            } else {
                                $('#lblCacBody').append('0');
                            }
                        },
                        error: function (vData) {
                            console.log(vData);
                        }
                    })
                }
            });
        }

        function GetSopa() {
            var data = {
                SearchString: '',
                OrderDateFrom: $.format.date(mRevenueDashboardDateFrom, 'MM-0' + 1 + '-yy'),
                OrderDateTo: $.format.date(mRevenueDashboardDateTo, 'MM-dd-yy')
            }

            AjaxSend({
                url: 'DashBoardsAndReports/GetSalesOrderReport',
                data: data,
                success: function (vData) {
                    var vRows = vData.SalesOrderReport.Rows;
                    var totalnewsopa = 0;

                    $('#lblSopaBody').empty().append($('<span style="font-size:12px">').text('Total order for the month'))
                            .append('<br>')

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            if (CStr2(vRows[i].Status) != '') {
                                totalnewsopa += 1;
                            }
                        }

                        $('#lblSopaBody').append(CDec2(totalnewsopa));
                    } else {
                        $('#lblSopaBody').append(0)
                    }
                }
            });
        }

        function GetGrossProfitabilityValidation() {
            AjaxSend({
                url: 'ERP/GetGrossProfitabilityValidationCount',
                success: function (e) {
                    if (e.Error != undefined) {
                        NewMessageBox(e.Error, 'Message');
                    } else {
                        $('#lblReportRevenueGrossProfitabilityValidationBody').empty().append($('<span style="font-size:12px">').text('Unapproved Count'))
                        .append('<br>')
                        .append(AccountingFormat(e.Result, 0));
                    }
                }
            })
        }

        function GetNumberContactPersonForApproval() {
            AjaxSend({
                url: 'OrganizationShort/GetNumberContactPersonForApproval',
                success: function (e) {
                    if (e.Error != undefined) {
                        NewMessageBox(e.Error, 'Message');
                    } else {
                        $('#lblCustomerAdminBody').empty()
                            .append($('<span style="font-size:12px">').text('to Online Store'))
                            .append('<br>')
                            .append(e.Customer)
                    }
                }
            })
        }

        LoadRequiredData();
    })
})