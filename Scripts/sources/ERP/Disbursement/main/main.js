require(['common'], function () {
    $(document).ready(function () {
        var htmlDisbursementDashboard = '', mItemRequisition, isLoadPriceCanvass = false, mStockReceivingController,
            htmlPO = '', htmlCreditAndCollection;

        $('#btnTabDisbursementSettings').off('click').click(function () {
            PromptUserAccess('Access Disbursements Settings*', {
                Message: 'You are not allowed to access <b>Disbursement Module Settings</b>.',
                OnClickOKSuccess: function () {
                    $('#tabDisbursementSettings').opentabmenu();

                    if (CStr2(htmlDisbursementSettings) == '') {
                        CallPage('#tabDisbursementSettings', htmlDisbursementSettings, '/Scripts/sources/ERP/Disbursement/settings/view.html', function (e) {
                            htmlDisbursementSettings = e;
                        })
                    }
                }
            });
        })

        $('#butErpItemRequisition').click(function () {
            var vParams = $.extend({
                ID: null,
                ShowProductSuppliersColumn: false
            }, vParams);


            require([
                    //'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
            ], function () {
                $('#divShowItemRequisition, #divShowItemRequisitionMain').pageshow();
                if (mItemRequisition == null) {
                    require(['promise!/Scripts/sources/ERP/PurchaseOrder/poBySku/controller.js?v=' + assetversion], function () {
                        mItemRequisition = ERP.PurchaseOrder.poBySku.controller();
                        mItemRequisition.ItemRequisitionMode = true;
                        mItemRequisition.Initialize();
                        $('#tabShowItemRequisitionWork').html('').append(mItemRequisition.element);
                        $('#tabShowItemRequisitionWork').opentabmenu();
                        require(['promise!/Scripts/sources/ERP/PurchaseOrder/ItemRequisitionSettings/controller.js?v=' + assetversion], function () {
                            //var pInstance = ERP.PurchaseOrder.ItemRequisitionSettings.controller();
                            //pInstance.Initialize();
                        });
                    });
                    //require(['promise!/Scripts/sources/ERP/ItemRequisition/settings/controller.js']);
                } else {
                    // mItemRequisition.Initialize();
                }
                AddBreadCrumb({
                    text: 'Item Requisition', Click: function () {
                        $('#divShowItemRequisition, #divShowItemRequisitionMain').pageshow();
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

        $('#btnDisbursePriceCanvassing').click(function () {
            PromptUserAccess('Access Vendor Price Canvass  Module*', {
                OnClickOKSuccess: function () {
                    sessionStorage.PriceCanvassType = "vendor";
                    $('#divPriceCanvass, #divPriceCanvassMain, #divPriceCanvassPage').pageshow();
                    require(['promise!/Scripts/sources/ERP/PriceCanvassing/main/controller.js?v=' + assetversion], function () {
                        $('#divRevenuePriceCanvassMain').empty();
                        if (!isLoadPriceCanvass) {
                            $('#divPriceCanvassMain').empty();
                            ERP.PriceCanvassing.main.controller();
                            isLoadPriceCanvass = true;
                        } else {
                            ERP.PriceCanvassing.main.controller().LoadPage();
                        }

                        AddBreadCrumb({
                            text: 'Vendor Price Canvass', Click: function () {
                                $('#divPriceCanvass, #divPriceCanvassMain, #divPriceCanvassPage').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                    });
                }
            });
        });

        $('#butDisbursementExpenseTracking').click(function () {
            require(['promise!/Scripts/sources/ERP/ExpenseTracking/main/controller.js?crc=' + assetversion], function () {
                $('#divErpExpenseTracking, #divETMainDiv').pageshow();
                $('#tabETDashboard').opentabmenu();
                AddBreadCrumb({
                    text: 'Expense Tickets', Click: function () {
                        sessionStorage.ViewID = '';
                        sessionStorage.IsCenterFromExpenseTracker = false;
                        $('#divErpExpenseTracking, #divETMainDiv').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            })
        })

        $('#butDisbursmentStockReceiving').click(function () {
            PromptUserAccess('Stock Receiving (EAS)', {
                OnClickOKSuccess: function () {
                    require([
                        'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion,
                        'promise!/Scripts/sources/ERP/Receiving/work/controller.js?v=' + assetversion
                    ], function () {
                        $('#divStockReceiving, #divStockReceivingMain').pageshow();
                        if (mStockReceivingController == null) {
                            mStockReceivingController = new ERP.Receiving.work.controller({
                                TransType: 'StockReceiving'
                            });
                            //require(['promise!/Scripts/sources/ERP/Invoicing/settings/controller.js']);
                        } else
                            mStockReceivingController.Initialize();
                        AddBreadCrumb({
                            text: 'Stock Receiving', Click: function () {
                                $('#divStockReceiving, divStockReceivingMain').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                        //mStockReceivingController.element.show();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#btnDisbursePurchasing').click(function () {
            PromptUserAccess('Purchasing (EAS)', {
                OnClickOKSuccess: function () {
                    AddBreadCrumb({
                        text: 'Purchasing', Click: function () {
                            $('#divPurchasing, #divPurchasingMain').pageshow();
                        }
                    }, '.ItemsBreadCrumbs');

                    if (htmlPO == '') {
                        CallPage('#divPurchasing', htmlPO, '/PurchaseOrder/Trans?v=' + assetversion, function (e) {
                            htmlPO = e;
                            $('#divPurchasing').find('nav').remove();
                            $('#divPurchasing').find('#divMain').removeClass('container-fluid');
                            $('#divPurchasing').pageshow();
                        })
                    } else {
                        $('#divPurchasing').pageshow();
                    }
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#btnDisburseJO').click(function () {
            $('#divJO').pageshow();
            AddBreadCrumb({
                text: 'JO', Click: function () {
                    $('#divJO, #divJOMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#butDisbursementAccountsPayable').click(function () {
            PromptUserAccess('Accounts Payable (EAS)', {
                OnClickOKSuccess: function () {
                    var mode = 'AP';

                    AddBreadCrumb({
                        text: 'Accounts Payable', Click: function () {
                            $('#divdivErpDisbursementAccountsPayable').pageshow();
                        }
                    }, '.ItemsBreadCrumbs');

                    if (htmlCreditAndCollection != undefined) {
                        $('#divdivErpDisbursementAccountsPayable').pageshow();
                        $('#divMainDashboardOP').show();
                        $('#tabCacWork').opentabmenu();
                        Cac.Show({
                            Mode: mode
                        });
                    } else {
                        CallPage('#divdivErpDisbursementAccountsPayable', htmlCreditAndCollection, '/CreditAndCollection', function (e) {
                            $('#divdivErpDisbursementAccountsPayable nav').remove();
                            $('#divdivErpDisbursementAccountsPayable #divMainCac').removeClass('container-fluid');
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

                                $('#divMainDashboardOP').hide();
                            });
                        });
                    }
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('.btnOngoing').click(function () {
            NewMessageBox('This feature is ongoing development', 'Message');
        })

        function ShowDisbursement() {
            $('#divErpDisbursement, #divDisburseMainMenu').pageshow();

            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    clearInterval(tempinterval)
                    $('.ItemsBreadCrumbs').empty();
                    AddBreadCrumb({
                        text: 'Home', Click: function () {
                            $('#divErpDisbursement, #divDisburseMainMenu').pageshow();
                            $('#divdivErpDisbursementAccountsPayable').pagehide();
                        }
                    }, '.ItemsBreadCrumbs')
                }
            }, 100)            

            if (CStr2(htmlDisbursementDashboard) == '') {
                CallPage('#tabDisbursementDashboard', htmlDisbursementDashboard, '/Scripts/sources/ERP/Disbursement/dashboard/view.html', function (e) {
                    htmlDisbursementDashboard = e;
                })
            }
        }

        ShowDisbursement();
    })
})