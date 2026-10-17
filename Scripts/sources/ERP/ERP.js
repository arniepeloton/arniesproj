require(['common', '/Scripts/sources/crm/qrcodeNew.js',
    '/Scripts/sources/CRMDashboard/customcalendar.js',
    'promise!/Scripts/sources/ERP/ErpRender.js',
    'promise!/Scripts3/NewMessageBox.js?crc=' + assetversion,
    'promise!/Scripts/sources/ERP/OrganizationMerger/controller.js',
    'promise!/Scripts/sources/SetupChecklist/NewPromptUser.js',
    'bootstrap-tooltip'
], function () {

    $('document').ready(function () {
        var mQuoteController;
        var mSalesOrderController;
        var mInvoiceController;
        var mItemRequisition;
        var mStockTransferController;
        var mPurchaseOrderController;
        var mPurchaseOrderPageController;
        var mStockReceivingController;
        var mStockReceivingPageController;
        var mDeliveryTrackingController;
        var mPriceListsController;
        var mOrderFulfillmentWorkLoaded;
        var isLoadPriceCanvass = false;
        var mConsigneeController;
        var mRevenueDashboardDateFrom = new Date();
        var mRevenueDashboardDateTo = new Date();

        // BEGIN debug
        (function () {

            OfflineMode = false;
            $('.OrganizationNameHeader').text(sessionStorage.OrgName);

            if (CStr2(sessionStorage.CurrentUserFullName) == '') {
                sessionStorage.CurrentUserFullName = '';
            }

            $('#txtLoggedUserName').text((sessionStorage.CurrentUserFullName).split(' ')[0]);
            function DoThis() {
                if (window.BrowsePopup == null) {
                    setTimeout(DoThis, 1000); // END setTimeout(function() {
                    return;
                }
                if (sessionStorage.FowstID == undefined) {
                    DisplayFowst({
                        AddFowstsToDeviceIfDeviceHasNoFowst: true
                    });
                } else {
                    $('#lblWorkFowstName').text(sessionStorage.FowstName);
                    if (window.IsMobile)
                        ShowOrderFulfillment();
                }

                AjaxSend({
                    url: '/Alerts/GetDashboardReminders',
                    success: function (e) {
                        var vRows = e.Compliance.Rows, cnt = 0;
                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var data = vRows[i];
                                cnt += CInt2(data.Cnt);
                            }
                            $('#lblCRNotifCount').text(cnt);
                        } else {
                            $('#lblCRNotifCount').text('');
                        }

                        vRows = e.ToDo.Rows;
                        if (CInt2(vRows[0].Cnt) > 0) {
                            $('#lblToDoNotifCount').text(vRows[0].Cnt);
                        } else {
                            $('#lblToDoNotifCount').text('');
                        }
                    }
                })
            }

            DoThis();

            $('body').tooltip({ selector: '[data-toggle="tooltip"]', trigger: 'hover' });

            sessionStorage.removeItem('CrmType');
        })(); // END (function() {
        // END debug

        //temp sa . dapat mauna ug load ang NewMessageBox.js kung asa naka butang ang AddBreadCrumb function. walay choice need i setInterval :'(
        var tempinterval = setInterval(function () {
            if (typeof AddBreadCrumb != 'undefined') {
                clearInterval(tempinterval)
                $('.ItemsBreadCrumbs').empty();
                AddBreadCrumb({
                    text: 'Home', Click: function () {
                        $('#divErpMain').pageshow();
                        location.hash = '#EASHome';
                        sessionStorage.removeItem('SystemAdminFor');
                        sessionStorage.removeItem('CrmType');
                        sessionStorage.IsUploadPIMViaPriceCanvass = false;
                        sessionStorage.IsCenterFromExpenseTracker = false;
                        sessionStorage.ViewID = '';
                    }
                }, '.ItemsBreadCrumbs')
		/*
                // BEGIN for testing only
                require([
                    'promise!/Scripts/sources/ERP/DeliveryTrackingEdit/controller.js'
                ], function () {
                    ERP.DeliveryTrackingEdit.controller.popup({
                        InvoiceIDs: ['eafdd281-526b-4bff-ad84-69952774f70e']
                    });
                });
                // END for testing only
		*/
            }
        }, 100)

        var htmlCreditAndCollection;
        var htmlInventoryHandling;
        var htmlDTR;
        var htmlCrm, htmlProject, htmlKB;
        var htmlGps; var htmlRevenueDashboard; var htmlSystemSettings; var htmlDisbursementDashboard;
        var htmlDisbursementSettings, htmlGatePass;
        var pUrlParams = GetUrlParameters();
        if (CBool(pUrlParams.IsReadOnly)) { // for first load 
            $('#divEasHeader, #divErpMain, #divRevMainMenu').hide();
        } else {
            $('#divEasHeader, #divErpMain, #divRevMainMenu').show();
        }
        $('#lblWorkFowstName').click(function () {
            if ($.isFunction(proj.BeforeChangeFowst)) {
                proj.BeforeChangeFowst({
                    Success: DisplayFowst
                });
            } else
                DisplayFowst();
        })

        $('#btnERPPeopleMenu').click(function () {
            $('.dropdownMenu').toggle('fast');
        })

        $('#btnViewProfile').click(function () {
            location.href = '/People/PeopleInfoSummary?p=eas';
        })

        $('#btnERPLogOut').click(function () {
            SignOut();
        })

        $('#btnErpRevenue').click(function () {
            //location.href = '/ERP/Revenue?OrgID=' + sessionStorage.OrgID;
            //return
            ShowRevenue();
        });

        $('#btnErpGatePass').click(function () {
            //location.href = '/GatePass?OrgID=' + sessionStorage.OrgID;
            require(['promise!/Scripts/sources/ERP/GatePass/main/controller.js?crc=' + assetversion], function () {
                $('#divGatePass nav:eq(0)').remove();
                $('#divGatePass .container-fluid').removeClass('container-fluid');
                $('#divGatePass').pageshow();
                AddBreadCrumb({
                    text: 'Gate Pass Creation and Approval', Click: function () {
                        $('#divGatePass').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            })
        })

        var ismobizhelploaded = false;
        $('#btnMobizHelp').click(function () {
            var popupUserGuide = $('#popupUserGuide'), src;

            if (!ismobizhelploaded) {
                if (location.hostname == 'localhost') {
                    src = 'http://' + location.host + '/KnowledgeBase?OrgID=cfe39df6-8809-4973-9515-810df75aed12';
                } else {
                    src = 'http://appmobiz.com/KnowledgeBase?OrgID=84e972ed-f208-4867-9506-5b8f9572562f';
                }
                var iFrame = $('<iframe class="w-100 h-100">').attr('src', src);
                popupUserGuide.find('.modal-body').append(iFrame);
                ismobizhelploaded = true;
            }

            popupUserGuide.modal('show');
        })

        function ShowRevenue() {
            sessionStorage.EasModule = 'revenue';
            $('#divErpRevenue').pageshow();
            //ReloadRevenueDashboard();
            if (!CBool(GetUrlParameters().IsReadOnly)) {
                $('#divRevMainMenu').pageshow();
                $('#tabRevenueWork').opentabmenu(); //$('#tabRevenueDashboard').opentabmenu();
            }

            function Proceed() {
                if (window.AddBreadCrumb)
                    AddBreadCrumb({
                        text: 'Revenue Modules', Click: function () {
                            sessionStorage.removeItem('CrmType');
                            $('#divErpRevenue, #divRevMainMenu').pageshow();
                        }
                    }, '.ItemsBreadCrumbs');
                else
                    setTimeout(Proceed, 100);
            }
            Proceed();
        }

        $('#butRevenueDashboardDateRange').click(function() {
            DualCalendar({
                Click: function (e) {
                    mRevenueDashboardDateFrom = e.From;
                    mRevenueDashboardDateTo = e.To;
                    $('#butRevenueDashboardDateRange span:first').text('Date Range: ' + FormatDate(mRevenueDashboardDateFrom) + ' to ' + FormatDate(mRevenueDashboardDateTo));
                    ReloadRevenueDashboard();
                }
            }, new Date(), new Date())
        });
        $('#butRevenueDashboardDateRange span:first').text('Date Range: ' + FormatDate(mRevenueDashboardDateFrom) + ' to ' + FormatDate(mRevenueDashboardDateTo));

        $('#btnTabRevenueDashboard').click(function (e) {
            //CallPage('#tabRevenueDashboard', htmlRevenueDashboard, '/Scripts/sources/ERP/Revenue/dashboard/dashboard.html', function (e) {
            //    htmlRevenueDashboard = e;
            //})
            //var grid = [];

            //for (var i = 0; i < 7; i++){
            //    grid.push({
            //        Header: 'Title ' + (i + 1),
            //        Body: AccountingFormat(Math.floor(100000 + Math.random() * 900000)),
            //        Click: function (data) {
            //            console.log(data);
            //        }
            //    })
            //}
            //RenderGrid('#tabRevenueDashboard', grid);
            
            NewPromptUserAccess('Main Revenue Dashboards', {
                OnClickOKSuccess: function () {
                    $('#tabRevenueDashboard').opentabmenu(); // tab button data-target attribute
                    ReloadRevenueDashboard();
                }
            });
            
        })

        var htmlRevenueSettings = '';
        $('#btnTabRevenueSettings').off('click').click(function () {
            NewPromptUserAccess('Main Revenue Settings', {
                OnClickOKSuccess: function () {
                    $('#tabRevenueSettings').opentabmenu();

                    if (CStr2(htmlRevenueSettings) == '') {
                        CallPage('#tabRevenueSettings', htmlRevenueSettings, '/Scripts/sources/ERP/Revenue/settings/view.html', function (e) {
                            htmlRevenueSettings = e;
                        })
                    } else {
                        $('#btnRSRemarksMessages').click();
                    }
                }
            });            
        })

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

        function GetGrossProfitabilityValidation() {
            AjaxSend({
                url: 'ERP/GetGrossProfitabilityValidationCount',
                success: function (e) {
                    if(e.Error != undefined){
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
                    if(e.Error != undefined){
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

        $('#btnReportCustomerAdmin').click(function () {
            $('#popupCustomerAdminApproval').modal('show');
            $('#tabVendorPortal').opentabmenu();
            GetCustomerContactPersonRequestList();
        })

        $('#txtSearchCustomerAdmin').keydown(function (e) {
            if (e.which == 13){
                GetCustomerContactPersonRequestList($(this).val())
            }
        })

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
            NewPromptUserAccess('Allow approval of contact person\'s request to access Online Store?', {
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

        $('#btnErpDisbursement').click(function () {
            //location.href = '/ERP/Disbursement?OrgID=' + sessionStorage.OrgID;
            //return;
            ShowDisbursement();
        });

        function ShowDisbursement() {
            sessionStorage.EasModule = 'disbursement';
            $('#divErpDisbursement, #divDisburseMainMenu').pageshow();
            //$('#tabDisbursementDashboard').opentabmenu(); with access rights na
            AddBreadCrumb({
                text: 'Disbursement', Click: function () {
                    sessionStorage.removeItem('CrmType');
                    $('#divErpDisbursement, #divDisburseMainMenu').pageshow();
                    $('#divdivErpDisbursementAccountsPayable').pagehide();
                }
            }, '.ItemsBreadCrumbs')            

            if (CStr2(htmlDisbursementDashboard) == '') {
                CallPage('#tabDisbursementDashboard', htmlDisbursementDashboard, '/Scripts/sources/ERP/Disbursement/dashboard/view.html', function (e) {
                    htmlDisbursementDashboard = e;
                })
            }

            //if (CStr2(htmlDisbursementSettings) == '') {
            //    CallPage('#tabDisbursementSettings', htmlDisbursementSettings, '/Scripts/sources/ERP/Disbursement/settings/view.html', function (e) {
            //        htmlDisbursementSettings = e;
            //        $('#tabDisbursementSettings').hide();
            //    })
            //}
        }

        $('#btnErpInventoryHandling').click(function () {
            //location.href = '/InventoryCount/InventoryHandling?OrgID=' + sessionStorage.OrgID;
            $('#tabInventoryHandlingDashboard').opentabmenu();
            $('#divInventoryHandling').pageshow();
            AddBreadCrumb({
                text: 'Warehouse & Inventory Management', Click: function () {
                    $('#divInventoryHandling, #divIHMain').pageshow();
                }
            }, '.ItemsBreadCrumbs');
            CallPage('#divInventoryHandling', htmlInventoryHandling, '/Scripts/sources/ERP/InventoryHandling/main.html?v=' + assetversion, function (e) {
                $('#divInventoryHandling nav:eq(0)').remove();
                $('#divInventoryHandling .container-fluid').removeClass('container-fluid');
                htmlInventoryHandling = e;
                if (typeof InventoryHC != 'undefined') {
                    InventoryHC.GetData();
                }
            })
        })

        $('#btnErpDTR').click(function () {
            //location.href = '/ERP/DTR?OrgID=' + sessionStorage.OrgID;
            //return
            AddBreadCrumb({
                text: 'Daily Time Record', Click: function () {
                    $('#divDailyTimeRecord').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            AjaxSend({
                url: '/UpdateServer/GetMobizDTRPortNo',
                success: function (vData) {

                    if (vData.Error || vData.MobizDTRPort == '')
                        sessionStorage.dtrhostname = location.hostname + ':9090'
                    else
                        sessionStorage.dtrhostname = location.hostname + ':' + vData.MobizDTRPort

                    //location.hostname + ':9090'
                    sessionStorage.SystemAdminFor = 'dtr';

                    if(CStr2(htmlDTR) != ''){
                        $('#divDailyTimeRecord').pageshow();
                        $('#tabDTRDashboard').opentabmenu();
                        return;
                    }

                    CallPage('#divDailyTimeRecord', htmlDTR, location.protocol + '//' + sessionStorage.dtrhostname + '/DTRReports/Main', function (e) {
                        htmlDTR = e;
                        $('#divDailyTimeRecord').find('#MainDiv').css('margin-top', '0px'); //naa sa dtr page
                        $('#divDailyTimeRecord').find('#HeaderDesktopVersion, #HeaderMobileVersion, .BreadCrumbs').remove();
                        $('#divDailyTimeRecord').find('#divDTRDashboardTab, .elementsForErp').removeClass('hidetab').show();
                        
                        if (typeof dtr !== 'undefined') {
                            dtr.RefreshLogs();
                        }
                    })
                }
            });
        })

        $('#btnErpProjectManagement').click(function () {
            //location.href = '/ERP/ProjectManagement?OrgID=' + sessionStorage.OrgID;
            //return;
            AddBreadCrumb({
                text: 'Project Management', Click: function () {
                    $('#divProjectManagement').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            CallPage('#divProjectManagement', htmlProject, '/SetupChecklist/BOMProjectExcel', function (e) {
                htmlProject = e;
                $('#divProjects').remove();
                $('#tabProjectMgmtDashboard').opentabmenu();
            })
        })
        
        function AddOnsCRM(module) {            
            //$('#btnCRMTabCrmServiceTicket, .btnSupporTicket').show() // show ticket buttons
            $('.lblCRMModule').text('Support ');
            sessionStorage.CrmType = module;
            sessionStorage.SystemAdminFor = 'crm';
            sessionStorage.IsCrm = true;

            if (module == 'revsalescrm') {
                $('#tabCrmSupportCrm').opentabmenu();
            } else {
                $('#tabCrmDashboard').opentabmenu();
            }

            $('#pageCRMMainDashboard').pageshow();

            if (typeof ShowProspectList !== 'undefined') {
                ShowProspectList();
            }
        }

        $('#btnSIDocScanner').click(function () {
            location.href = '/SIDocScanner?OrgID=' + sessionStorage.OrgID;
        })

        $('#btnRevSalesCrm').click(function () {
            AddBreadCrumb({
                text: 'Sales CRM', Click: function () {
                    $('#divCustomerSupport, #pageCRMMainDashboard').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            sessionStorage.CRMDashboardPage = 'leadgen';

            Z9Q3C9ASDA.CallCRMPage(function (e) {
                AddOnsCRM('revsalescrm')
            })
        })

        $('#btnErpCustomerSupport').click(function () {
            sessionStorage.CRMDashboardPage = 'leadgen';
            sessionStorage.CrmType = 'crm';
            sessionStorage.SystemAdminFor = 'crm';
            sessionStorage.IsCrm = true;
            //location.href = '/ERP/Crm?OrgID=' + sessionStorage.OrgID;
            //return
            AddBreadCrumb({
                text: 'Support CRM', Click: function () {
                    $('#divCustomerSupport, #pageCRMMainDashboard').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            if (CStr2(htmlCrm) != '') {
                $('#divCustomerSupport, #pageCRMMainDashboard').pageshow();
                AddOnsCRM('crm');

                if ($('#tabDTR201File > *').length > 0) {
                    $('#tabDTR201File > #divCRMPopups').prependTo('#divCRMMainPage');
                    $('#tabDTRSupportCrm').appendTo('#divCRMDashboardContent');
                    $('#tabDTRSupportCrm').attr('id', 'tabCrmSupportCrm').hide();
                    $('#pageCRMActions').appendTo('#divCRMPages');
                    $('#pageCRMActions').attr('subpage', 'pageCRMLeadGen');
                    $('[menu-button="#btnCRMPageOpportunity"], #btnCRMPageOpportunity').parent('li').show();
                    $('[menu-button="#btnCRMPageTickets"], #btnCRMPageTickets').parent('li').show();
                }
                return;
            }

            Z9Q3C9ASDA.CallCRMPage(function (e) {                
                AddOnsCRM('crm')
            })
        })

        $('#btnErpAssetsMgmt').click(function () {
            //location.href = '/AssetsManagement?OrgID=' + sessionStorage.OrgID;
            require(['promise!/Scripts/sources/ERP/AssetsManagement/main/controller.js?v=' + assetversion], function () {
                $('#divErpAssetsManagement nav:eq(0)').remove();
                $('#divErpAssetsManagement .container-fluid').removeClass('container-fluid');
                $('#divErpAssetsManagement').pageshow();
                AddBreadCrumb({
                    text: 'Assets Management', Click: function () {
                        $('#divErpAssetsManagement').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            })
        })

        $('#btnErpRentableServices').click(function () {
            //location.href = '/AssetsManagement?OrgID=' + sessionStorage.OrgID;
            require(['promise!/Scripts/sources/ERP/RentableServices/main/controller.js?v=' + assetversion], function () {
                $('#divErpRentableServices').pageshow();
                AddBreadCrumb({
                    text: 'Rentable Services', Click: function () {
                        $('#divErpRentableServices').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            })
        })
        
        $('#butErpProductMaker').click(function () {
            require(['promise!/Scripts/sources/ERP/ProductMaker/main/controller.js?v=' + assetversion], function () {
                $('#divErpProductMaker').pageshow();
                AddBreadCrumb({
                    text: 'Product Maker', Click: function () {
                        $('#divErpProductMaker').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            })
        })

        $('#btnErpFinancialStatement').click(function () {
            NewPromptUserAccess('Access Financial Statement Module*', {
                OnClickOKSuccess: function () {
            require(['promise!/Scripts/sources/ERP/FinancialStatement/main/controller.js?v=' + assetversion], function () {
                $('#divErpFinancialStatement').pageshow();
                AddBreadCrumb({
                    text: 'Financial Statements', Click: function () {
                        $('#divErpFinancialStatement').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            })
                }
            });
        })

        $('#btnErpComplianceReminders').click(function () {
            NewPromptUserAccess('Access Compliance Reminders Module*', {
                Message: 'You don\'t have access to <b>Compliance Reminders module</b>',
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/ComplianceReminders/main/controller.js?v=' + assetversion], function () {
                        $('#divErpComplianceReminders').pageshow();
                        AddBreadCrumb({
                            text: 'Compliance Reminders', Click: function () {
                                $('#divErpComplianceReminders').pageshow();
                            }
                        }, '.ItemsBreadCrumbs')
                    })
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        })

        $('#btnErpGPS').click(function () {
            //location.href = '/Erp/GPSDeviceTracker?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&v=' + assetversion
            //return
            AddBreadCrumb({
                text: 'Fleet Tracking & Management', Click: function () {
                    $('#divGPS').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            CallPage('#divGPS', htmlGps, '/DashboardsAndReports/Device', function (e) {
                htmlGps = e;
                $('#divGPS').find('#divHeaderGPS').remove();
                $('#divGPS').find('div[data-role="page"]').removeAttr('data-role')
                $('#divGPS').find('.elementsForErp').show();
            })
        })

        $('#btnErpSettings').click(function () {
            NewPromptUserAccess('System Settings', {
                OnClickOKSuccess: function () {
                    sessionStorage.SystemAdminFor = 'mcr';
                    sessionStorage.IsCrm = false;
                    //location = '/SetupChecklist/SystemAdministration?OrgID=' + sessionStorage.OrgID + '&v=' + assetversion;
                    //return;
                    AddBreadCrumb({
                        text: 'System Administration', Click: function () {
                            window.location.hash = 'SystemAdministration';
                        }
                    }, '.ItemsBreadCrumbs')

                    CallPage('#divSystemSettings', htmlSystemSettings, '/SetupChecklist/SystemAdministration', function (e) {
                        htmlSystemSettings = e;
                        $('#divSystemSettings nav, #divContent > br').remove();
                        $('#divSystemSettings .container-fluid').removeClass('container-fluid');
                        $('#divSystemAdDashboard').show();
                        $('#divSecond, #UniversalDiv').hide();
                        location.hash = '';
                        sessionStorage.SystemAdminFor = 'mcr';
                        sessionStorage.IsCrm = false;
                    })
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#btnErpContractDesigner').click(function () {
            //location.href = '/ContractDesigner?OrgID=' + sessionStorage.OrgID;
            require(['https://cdn.jsdelivr.net/npm/summernote@0.9.0/dist/summernote-lite.min.js'], function () {
                require(['promise!/Scripts/sources/ERP/ContractDesigner/main/controller.js?v=' + assetversion], function () {
                    $('#divErpContractDesigner nav:eq(0)').remove();
                    $('#divErpContractDesigner .container-fluid').removeClass('container-fluid');
                    $('#divErpContractDesigner').pageshow();
                    AddBreadCrumb({
                        text: 'Contract and Letter Designer', Click: function () {
                            $('#divErpContractDesigner, #divContractDesignerList').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                })
            })
        })

        $('#btnErpKnowledgeBase').click(function () {
            $('#divErpKnowledgeBase, #pageMainDashboard').pageshow();
            AddBreadCrumb({
                text: 'Knowledge Base', Click: function () {
                    $('#divErpKnowledgeBase, #panMainDashboardKB').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            if (CStr2(htmlKB) == '') {
                if ($('#tabDTRKb > *').length == 0) {
                    CallPage('#divErpKnowledgeBase', htmlKB, '/KnowledgeBase/ContentCreator', function (e) {
                        htmlKB = e;
                        $('#divErpKnowledgeBase').find('#divKB').remove();
                        $('#divErpKnowledgeBase').find('div[data-role="page"]').removeAttr('data-role')
                    })
                } else {
                    $('#tabDTRKb > *').appendTo('#divErpKnowledgeBase');
                }
            } if ($('#tabDTRKb > *').length > 0) {
                $('#tabDTRKb > *').appendTo('#divErpKnowledgeBase');
            }
        })

        $('#butErpUPIScanner').click(function () {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/OrderProcessing/product/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                var pProductPopup = new ERP.OrderProcessing.product.controller();
                pProductPopup.UPIpopup();
            })
        })

        //BUTTONS FOR REVENUE
        $('#btnRevMarketing').click(function () {
            AddBreadCrumb({
                text: 'Marketing CRM', Click: function () {
                    $('#divCustomerSupport, #pageMainDashboard').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            sessionStorage.CRMDashboardPage = 'leadgen';

            CallPage('#divCustomerSupport', htmlCrm, '/CRMDashboard/db', function (e) {
                $('#pageMainDashboard').pageshow();
                $('#btnTabCrmServiceTicket, .btnSupporTicket').hide() // hide ticket buttons
                htmlCrm = e;
                $('#tabCrmDashboard').opentabmenu();
                $('.lblCRMModule').text('Marketing ');
                sessionStorage.SystemAdminFor = 'crm';
            })
        })

        $('#btnRevQuotation').click(function () {
            NewPromptUserAccess('Sales Quotation (EAS)', {
                OnClickOKSuccess: function () {
                    //location.href = '/Erp/Quotation?OrgID=' + sessionStorage.OrgID;
                    //return
                    require(['promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion], function () {
                        HideModules();
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

        function HideModules() {
            //if (mQuoteController != null)
            //    mQuoteController.element.hide();
            //if (mSalesOrderController != null)
            //    mSalesOrderController.element.hide();
            //if (mInvoiceController != null)
            //    mInvoiceController.element.hide();
        }
        $('#btnRevOrderProcessing').click(function () {
            PromptUserAccess('Sales Order (EAS)', {
                OnClickOKSuccess: function () {
                    //location.href = '/Erp/OrderProcessing?OrgID=' + sessionStorage.OrgID;
                    //return
                    require(['promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion], function () {
                        HideModules();
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

        function ShowItemRequisition(vParams) {
            vParams = $.extend({
                ID: null,
                ShowProductSuppliersColumn: false
            }, vParams);
            //ShowDisbursement();

            function OpenTabMenu() {
                var p = $('#tabShowItemRequisitionWork');
                if (p.length > 0)
                    p.opentabmenu();
                else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                    //'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
            ], function () {
                HideModules();
                $('#divShowItemRequisition, #divShowItemRequisitionMain').pageshow();
                if (mItemRequisition == null) {
                    require(['promise!/Scripts/sources/ERP/PurchaseOrder/poBySku/controller.js?v=' + assetversion], function() {
                        mItemRequisition = ERP.PurchaseOrder.poBySku.controller();
                        mItemRequisition.ItemRequisitionMode = true;
                        mItemRequisition.Initialize();
                        $('#tabShowItemRequisitionWork').html('').append(mItemRequisition.element);
                        require(['promise!/Scripts/sources/ERP/PurchaseOrder/ItemRequisitionSettings/controller.js?v=' + assetversion], function() {
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
        }

        $('#butErpItemRequisition').click(function () {
            //PromptUserAccess('Sales Invoice (EAS)', {
            //    OnClickOKSuccess: function () {
                    ShowItemRequisition();
            //    },
            //    OnFailed: function () {
            //        console.log('jsCFDTPD3GXC!');
            //    }
            //});
        });

        $('#butErpAutoPrintInvoice').click(function () {
            proj.AutoPrintEnrollment();
        });

        $('#btnRevOrderFulfillment').click(function () {
            ShowOrderFulfillment();
        });
        function ShowOrderFulfillment() {
            //location.href = '/Erp/FulfillmentCenter?OrgID=' + sessionStorage.OrgID;
            //return
            //ShowRevenue();
            $('#divOrderFulfillment, #divOrderFulfillmentMain').pageshow();
            AddBreadCrumb({
                text: 'Fulfillment Center', Click: function () {
                    $('#divOrderFulfillment, #divOrderFulfillmentMain').pageshow();
                }
            }, '.ItemsBreadCrumbs');
            if (mOrderFulfillmentWorkLoaded)
                $('#divOrderFulfillmentWork').show();
            else
                PromptUserAccess('Access Order Fulfillment Module*', {
                    OnClickOKSuccess: function () {
                        AjaxSend({
                            url: '/DashboardsAndReports/SalesOrder?OrgID=' + sessionStorage.OrgID,
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                $('#divOrderFulfillmentWork').empty();
                                $('#divOrderFulfillmentWork').append(vData).trigger('create');
                                mOrderFulfillmentWorkLoaded = true;
                            }
                        });

                        //$('#divMainDashboard, #divSalesAnalytics, #divRequestAndApprovals, #divCacMcr,' +
                        //    '#divOnPrintPOSSalesReport,' +
                        //    '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                        //$('#divSalesOrderProcessing').show();
                        //$('#pageOrderStatus').hide();
                        //$('#pageOrderCentral').show();
                    }
                }); // END PromptUserAccess('Sales Order Processing and Analytics', {
        } // END function ShowOrderFulfillment

        $('#btnQuoteDateNeeded, #btnOPDateNeeded, #btnOPPODateNeeded, #btnOFPODate, #btnOFDateNeeded, #btnInvoicingSODate, ' +
            '#btnInvoicingQuoteDate, #btnInvoicingPODate, #btnInvoicingDateNeeded, ' +
            '#btnPurchasingDateNeeded, #btnJOQuoteDate, #btnJODateNeeded').click(function () {
                // 2017_01_16__12_57_59_9805 removed #btnPurchasingQuoteDate
                var dis = $(this);
                SingleCalendar({
                    Click: function (e) {
                        dis.val(e)
                    }
                })
            })

        $('#btnRevInvoicing').click(function () {
            PromptUserAccess('Sales Invoice (EAS)', {
                OnClickOKSuccess: function () {
                    //location.href = '/Erp/Invoicing?OrgID=' + sessionStorage.OrgID;
                    //return
                    ShowInvoice();
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#btnRevPOS').click(function () {
            require(['/Scripts/sources/ERP/OrderProcessing/work/model.js?v=' + assetversion], function() {
                var pModel = new ERP.OrderProcessing.work.model();
                pModel.HandleWebPosDeviceFowst({
                    Success: function () {
                        sessionStorage.IsWebPos = true;
                        location.href = '/SalesMenu/WebPOSNew?OrgID=' + sessionStorage.OrgID + '&IsMobile=0'
                    },
                    Fail: function(vMsg) {
                        NewMessageBox(vMsg, 'Unauthorized Web POS Device');
                    }
                });
            });
        });

        $('#btnRevPriceBulletin').click(function () {
            require(['promise!/Scripts/sources/ERP/PriceBulletin/main/controller.js?v=' + assetversion], function () {
                AddBreadCrumb({
                    text: 'Price Bulletin', Click: function () {
                        $('#divErpPriceBulletin, #divPriceBulletinMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                $('#divErpPriceBulletin').pageshow();
            });
        });

        $('#butStockTransfer').click(function () {
            PromptUserAccess('Sales Invoice (EAS)', {
                OnClickOKSuccess: function () {
                    ShowStockTransfer();
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });
        /*
        window.Z9Q3C9ASDA.ShowInvoice({
            ID: '71adf5c1-2a26-4671-bca5-1ff06811d3fe',
            ReadOnly: true
        });
        */
        window.Z9Q3C9ASDA = {
            ShowInvoice: ShowInvoice,
            ShowSalesOrder: ShowSalesOrder,
            ShowQuotation: ShowQuotation,
            ShowPurchaseOrder: ShowPurchaseOrder,
            ShowStockReceiving: ShowStockReceiving,
            ShowAccountsPayable: ShowAccountsPayable,
            CallCRMPage: function (callback) {
                if (CStr2(htmlCrm) == '') {
                    CallPage('#divCustomerSupport', htmlCrm, '/CRMDashboard/db', function (e) {
                        htmlCrm = e;
                        if ($.isFunction(callback)) {
                            callback(e);
                        }

                        require(['/Scripts/sources/ERP/OrderProcessing/orderhistory/controller.js?v=' + assetversion])
                    })
                } else {
                    $('#divCustomerSupport').pageshow();

                    if ($.isFunction(callback)) {
                        callback(htmlCrm);
                    }
                }
            }
        };
        function ShowStockTransfer(vParams) {
            //location.href = '/Erp/StockTransfer?OrgID=' + sessionStorage.OrgID;
            //return
            vParams = $.extend({
                ID: null
            }, vParams);
            //ShowRevenue();
            function OpenTabMenu() {
                var p = $('#tabStockTransferWork');
                if (p.length > 0) 
                    p.opentabmenu();
                else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                    'promise!/Scripts/sources/ERP/Receiving/work/controller.js?v=' + assetversion,
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
                ], function () {
                    HideModules();
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
        }
        function ShowInvoice(vParams) {
            vParams = $.extend({
                ID: null,
                ShowProductSuppliersColumn: false
            }, vParams);
            //ShowRevenue();

            function OpenTabMenu() {
                var p = $('#tabInvoicingWork');
                if (p.length > 0)
                    p.opentabmenu();
                else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
            ], function () {
                HideModules();
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
        }
        
        function ShowSalesOrder(vParams) {
            location.href = '/Erp/OrderProcessing' + location.search + '&IsPreview=1';
            return

            vParams = $.extend({
                ID: null
            }, vParams);
            ShowRevenue();

            function OpenTabMenu() {
                var p = $('#tabOPWork');
                if (p.length > 0)
                    p.opentabmenu();
                else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
            ], function () {
                HideModules();
                $('#divOrderProcessing, #divOrderProcessingMain').pageshow();
                if (mInvoiceController == null) {
                    mInvoiceController = new ERP.OrderProcessing.work.controller({
                        TransType: 'SalesOrder'
                    });
                    //require(['promise!/Scripts/sources/ERP/Order/settings/controller.js']);
                } else
                    mInvoiceController.Initialize();
                AddBreadCrumb({
                    text: 'Order Processing', Click: function () {
                        $('#divOrderProcessing, #divOrderProcessingMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                if (vParams.ID)
                    mInvoiceController.LoadTrans({
                        ID: vParams.ID
                    });
                if (vParams.ReadOnly)
                    mInvoiceController.ReadOnly();
            });
        }
        
        $('#butDeliveryTracking').click(ShowDeliveryTracking);
        
        function ShowDeliveryTracking(vParams) {
            //location.href = '/Erp/DeliveryTracking?OrgID=' + sessionStorage.OrgID;
            //return
            vParams = $.extend({
                ID: null
            }, vParams);

            require([
                'promise!/Scripts/sources/ERP/DeliveryTracking/controller.js?v=' + assetversion
            ], function () {
                HideModules();
                if (mDeliveryTrackingController == null) 
                    mDeliveryTrackingController = new ERP.DeliveryTracking.controller();
                AddBreadCrumb({
                    text: 'Delivery Tracking', Click: function () {
                        $('#divDeliveryTracking, #divDeliveryTrackingMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                    $('#divDeliveryTracking, #divDeliveryTrackingMain').pageshow();
                if (vParams.ID)
                    mInvoiceController.LoadTrans({
                        ID: vParams.ID
                    });
                if (vParams.ReadOnly)
                    mInvoiceController.ReadOnly();
            });
        }
        
        function ShowQuotation(vParams) {
            vParams = $.extend({
                ID: null
            }, vParams);
            ShowRevenue();

            function OpenTabMenu() {
                var p = $('#tabQuoteWork');
                if (p.length > 0)
                    p.opentabmenu();
                else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
            ], function () {
                HideModules();
                $('#divQuotation, #divQuotationMain').pageshow();
                if (mQuoteController == null) {
                    mQuoteController = new ERP.OrderProcessing.work.controller({
                        TransType: 'Quote'
                    });
                    //require(['promise!/Scripts/sources/ERP/Order/settings/controller.js']);
                } else
                    mQuoteController.Initialize();
                AddBreadCrumb({
                    text: 'Quotation', Click: function () {
                        $('#divQuotation').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                if (vParams.ID)
                    mQuoteController.LoadTrans({
                        ID: vParams.ID
                    });
                if (vParams.ReadOnly)
                    mQuoteController.ReadOnly();
            });
        }

        function ShowStockReceiving(vParams) {
            vParams = $.extend({
                ID: null
            }, vParams);
            ShowRevenue();

            function OpenTabMenu() {
                var p = $('#tabStockReceivingWork');
                if (p.length > 0)
                    p.opentabmenu();
                else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion,
                    'promise!/Scripts/sources/ERP/Receiving/work/controller.js?v=' + assetversion
            ], function () {
                HideModules();
                $('#divErpDisbursement').show();
                $('#divStockReceiving, #divStockReceivingMain').pageshow();
                if (mStockReceivingPageController == null) {
                    mStockReceivingPageController = new ERP.Receiving.work.controller({
                        TransType: 'StockReceiving'
                    });
                    //require(['promise!/Scripts/sources/ERP/Order/settings/controller.js']);
                } else
                    mStockReceivingPageController.Initialize();
                AddBreadCrumb({
                    text: 'Stock Receiving', Click: function () {
                        $('#divStockReceiving, divStockReceivingMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                if (vParams.ID)
                    mStockReceivingPageController.LoadTrans({
                        ID: vParams.ID
                    });
                if (vParams.ReadOnly)
                    mStockReceivingPageController.ReadOnly();
            });
        }

        var htmlPO = '';
        function ShowPurchaseOrder(vParams) {
            AddBreadCrumb({
                text: 'Purchasing', Click: function () {
                    $('#divPurchasing, #divPurchasingMain').pageshow();
                }
            }, '.ItemsBreadCrumbs');

            if (htmlPO == '') {
                CallPage('#divPurchasing', htmlPO, '/PurchaseOrder/Trans?v=' + assetversion, function (e) {
                    htmlPO = e;
                    $('#divPurchasing').find('nav:eq(0)').remove();
                    $('#divPurchasing').find('#divMain').removeClass('container-fluid');
                    $('#divPurchasing').pageshow();
                })
            } else {
                $('#divPurchasing').pageshow();
            }
            return
            vParams = $.extend({
                ID: null
            }, vParams);
            ShowDisbursement();

            function OpenTabMenu() {
                var p = $('#tabPurchasingWork');
                if (p.length > 0)
                    p.opentabmenu();
                else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
            ], function () {
                HideModules();
                $('#divPurchasing, #divPurchasingMain').pageshow();
                if (mPurchaseOrderPageController == null) {
                    mPurchaseOrderPageController = new ERP.OrderProcessing.work.controller({
                        TransType: 'PurchaseOrder',
                        AccountsPayable: vParams.AccountsPayable
                    });
                    require(['promise!/Scripts/sources/ERP/PurchaseOrder/settings/controller.js?v=' + assetversion]);
                    require(['promise!/Scripts/sources/ERP/PurchaseOrder/poBySku/controller.js?v=' + assetversion], function() {
                        var pInstance = ERP.PurchaseOrder.poBySku.controller();
                        $('#tabPurchasingPoBySku').html('').append(pInstance.element);
                        pInstance.Initialize();
                    });
                } else
                    mPurchaseOrderPageController.Initialize();
                AddBreadCrumb({
                    text: 'Purcahsing', Click: function () {
                        $('#divPurchasing, #divPurchasingMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                if (vParams.ID)
                    mPurchaseOrderPageController.LoadTrans({
                        ID: vParams.ID,
                        IncludeZeroQty: true
                    });
                if (vParams.ReadOnly)
                    mPurchaseOrderPageController.ReadOnly();
            });
        }

        function ShowAccountsPayable(vParams) {
            vParams = $.extend({
                ID: null
            }, vParams);
            ShowDisbursement();

            function OpenTabMenu() {
                var p = $('#tabPurchasingWork');
                if (p.length > 0) {
                    p.opentabmenu();
                    $('body').show();
                } else
                    setTimeout(OpenTabMenu, 1000);
            }
            OpenTabMenu();
            require([
                    'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
            ], function () {
                HideModules();
                $('#divPurchasingTab').hide();
                $('#divPurchasing, #divPurchasingMain').pageshow();
                if (mPurchaseOrderPageController == null) {
                    mPurchaseOrderPageController = new ERP.OrderProcessing.work.controller({
                        TransType: 'AccountsPayable',
                        AccountsPayable: vParams.AccountsPayable
                    });
                    //require(['promise!/Scripts/sources/ERP/Order/settings/controller.js']);
                } else
                    mPurchaseOrderPageController.Initialize();
                AddBreadCrumb({
                    text: 'Accounts Payable', Click: function () {
                        $('#divPurchasing, #divPurchasingMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                if (vParams.ID)
                    mPurchaseOrderPageController.LoadTrans({
                        ID: vParams.ID,
                        IncludeZeroQty: true
                    });
                if (vParams.ReadOnly)
                    mPurchaseOrderPageController.ReadOnly();
            });
        }

        $('#butDisbursmentStockReceiving').click(function () {
            PromptUserAccess('Stock Receiving (EAS)', {
                OnClickOKSuccess: function () {
                    //location.href = '/Erp/StockReceiving?OrgID=' + sessionStorage.OrgID;
                    //return
                    require([
                        'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion,
                        'promise!/Scripts/sources/ERP/Receiving/work/controller.js?v=' + assetversion
                    ], function () {
                        HideModules();
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
        $('#btnRevCreditAndCollection').click(function () {
            PromptUserAccess('Credit and Collection (EAS)', {
                OnClickOKSuccess: function () {
                    GotoCac('AR');
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butRevDeposits').click(function () {
            PromptUserAccess('Deposits (EAS)', {
                OnClickOKSuccess: function () {
                    //location.href = '/Erp/Deposits?OrgID=' + sessionStorage.OrgID;
                    //return
                    $('#divErpDeposits').pageshow();
                    AddBreadCrumb({
                        text: 'Deposits', Click: function () {
                            $('#divErpDeposits').pageshow();
                        }
                    }, '.ItemsBreadCrumbs');
                    require(['promise!/Scripts/sources/ERP/Deposits/main/controller.js?v=' + assetversion], function () {
                        ERP.Deposits.main.controller = ERP.Deposits.main.controller('qwe');
                        $('#divErpDeposits').pageshow();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butDisbursementChequeClearing').click(function () {
            PromptUserAccess('Deposits (EAS)', {
                OnClickOKSuccess: function () {
                    //location.href = '/Erp/Deposits?OrgID=' + sessionStorage.OrgID;
                    //return
                    $('#divErpDeposits').pageshow();
                    AddBreadCrumb({
                        text: 'Cheque Clearing', Click: function () {
                            $('#divErpDeposits').pageshow();
                        }
                    }, '.ItemsBreadCrumbs');
                    require(['promise!/Scripts/sources/ERP/Deposits/main/controller.js?v=' + assetversion], function () {
                        ERP.Deposits.main.controller = ERP.Deposits.main.controller({
                            mode: 'disbursement'
                        });
                        $('#divErpDeposits').pageshow();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butCrossTabRreport').click(function () {
            PromptUserAccess('Cross Tab Report', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Reports/crossTab/controller.js'], function () {
                        ERP.Reports.crossTab.controller.Show();
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#butSalesReportBySalesRepresentativeAndSupplier').click(function () {
            PromptUserAccess('Cross Tab Report', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Reports/crossTab/controller.js'], function () {
                        ERP.Reports.crossTab.controller.Show('SalesReportBySalesRepresentativeAndSupplier');
                    });
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
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
        $('#btnRevenuePriceCanvassing').click(function () {
            NewPromptUserAccess('Access Competitor Price Canvassing Module*', {
                OnClickOKSuccess: function () {
                    sessionStorage.PriceCanvassType = "competitor";
                    //location.href = '/ERP/PriceCanvass?OrgID=' + sessionStorage.OrgID + '&mod=competitor';
                    //return
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

        $('#btnConsignee').click(function () {
            //location.href = '/ERP/Consignee?OrgID=' + sessionStorage.OrgID;
            //return
            require(['promise!/Scripts/sources/ERP/Consignee/main/controller.js'], function () {
                $('#divErpRevenue, #divConsignee').pageshow();
                AddBreadCrumb({
                    text: 'Consignee', Click: function () {
                        $('#divErpRevenue, #divConsignee').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
            });
        })

        //$('#btnConsignee').click(function () {
        //    PromptUserAccess('Sales Order (EAS)', {
        //        OnClickOKSuccess: function () {
        //            require(['promise!/Scripts/sources/ERP/Consignee/main/controller.js?v=' + assetversion], function () {
        //                HideModules();
        //                $('#divOrderProcessing, #divOrderProcessingMain').pageshow();
        //                if (mConsigneeController == null) {
        //                    mConsigneeController = new ERP.OrderProcessing.work.controller({
        //                        TransType: 'SalesOrder'
        //                    });
        //                    require(['promise!/Scripts/sources/ERP/Consignee/dashboard/controller.js?v=' + assetversion]);
        //                } else
        //                    mConsigneeController.Initialize();
        //                AddBreadCrumb({
        //                    text: 'Consignee', Click: function () {
        //                        $('#divOrderProcessing, #divOrderProcessingMain').pageshow();
        //                    }
        //                }, '.ItemsBreadCrumbs');
        //            });
        //        },
        //        OnFailed: function () {
        //            console.log('jsCFDTPD3GXC!');
        //        }
        //    });


        //});

        $('#butErpSerialNumberTracking').click(function() {
            require(['promise!/Scripts/sources/ERP/ActivityTracker/controller.js?v=' + assetversion], function() {
                ERP.ActivityTracker.controller.popup({
                    
                });
            });
        });

        $('#butRevenuePriceLists').click(function () {
            NewPromptUserAccess('Access Competitor Price Canvassing Module*', { // todo: change to proper required access
                OnClickOKSuccess: function () {
                    $('#divPriceLists').pageshow();
                    require(['promise!/Scripts/sources/ERP/PriceLists/controller.js'], function () {
                        HideModules();
                        if (mPriceListsController == null) {
                            mPriceListsController = new ERP.PriceLists.controller();
                        } 

                        AddBreadCrumb({
                            text: 'Create and Download Price Lists', Click: function () {
                                $('#divPriceLists').pageshow();
                            }
                        }, '.ItemsBreadCrumbs');
                    });
                }
            });
        });
        //END REVENUE

        //BUTTONS FOR DISBURSEMENT        
        $('#btnDisbursePriceCanvassing').click(function () {
            NewPromptUserAccess('Access Vendor Price Canvass  Module*', {
                OnClickOKSuccess: function () {
                    sessionStorage.PriceCanvassType = "vendor";
                    //location.href = '/ERP/PriceCanvass?OrgID=' + sessionStorage.OrgID + '&mod=vendor';
                    //return
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
            //location.href = '/ERP/ExpenseTracking?OrgID=' + sessionStorage.OrgID;
            //return
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

        $('#btnDisbursePurchasing').click(function () {
            PromptUserAccess('Purchasing (EAS)', {
                OnClickOKSuccess: function () {
                    //location.href = '/PurchaseOrder/Trans?OrgID=' + sessionStorage.OrgID;
                    //return
                    ShowPurchaseOrder();
                    //require(['promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion], function () {
                    //    HideModules();
                    //    $('#divPurchasing').pageshow();
                    //    if (mPurchaseOrderController == null) {
                    //        mPurchaseOrderController = new ERP.OrderProcessing.work.controller({
                    //            TransType: 'PurchaseOrder'
                    //        });
                    //        require(['promise!/Scripts/sources/ERP/PurchaseOrder/settings/controller.js?v=' + assetversion]);
                    //        require(['promise!/Scripts/sources/ERP/PurchaseOrder/poBySku/controller.js']);
                    //    } else
                    //        mPurchaseOrderController.Initialize();
                    //    AddBreadCrumb({
                    //        text: 'Purchasing', Click: function () {
                    //            $('#divPurchasing, #divPurchasingMain').pageshow();
                    //        }
                    //    }, '.ItemsBreadCrumbs');
                    //});
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
                    GotoCac('AP');
                    //$('#divCreditAndCollection').pageshow();
                    //$('#divDisburseMainMenu').hide();
                    //AddBreadCrumb({
                    //    text: 'Accounts Payable', Click: function () {
                    //        $('#divdivErpDisbursementAccountsPayable').pageshow();
                    //    }
                    //}, '.ItemsBreadCrumbs');

                    //CallPage('#divdivErpDisbursementAccountsPayable', htmlCreditAndCollection, '/CreditAndCollection', function (e) {
                    //    require(['promise!/Scripts/sources/ERP/OrderProcessing/dashboard/controller.js'], function () {
                    //        $('#pageCac').pageshow();
                    //        htmlCreditAndCollection = e;

                    //        function AttemptShow() {
                    //            if (typeof Cac !== 'undefined') {
                    //                Cac.Show({
                    //                    Mode: 'AP'
                    //                });
                    //            } else
                    //                setTimeout(AttemptShow, 100);
                    //        }
                    //        setTimeout(AttemptShow, 100);
                    //    }); // END require(['promise!/Scripts/sources/ERP/OrderProcessing/dashboard/controller.js'], function() {
                    //});
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });
        });

        $('#btnTabDisbursementDashboard').off('click').click(function () {
            NewPromptUserAccess('Main Disbursement Dashboards', {
                Message: 'You are not allowed to access <b>Main Disbursement Dashboards</b>.',
                OnClickOKSuccess: function () {
                    $('#tabDisbursementDashboard').opentabmenu();
                }
            });
        })

        $('#btnTabDisbursementSettings').off('click').click(function () {
            NewPromptUserAccess('Main Disbursement Settings', {
                Message: 'You are not allowed to access <b>Main Disbursement Settings</b>.',
                OnClickOKSuccess: function () {
                    $('#tabDisbursementSettings').opentabmenu();

                    if (CStr2(htmlDisbursementSettings) == '') {
                        CallPage('#tabDisbursementSettings', htmlDisbursementSettings, '/Scripts/sources/ERP/Disbursement/settings/view.html', function (e) {
                            htmlDisbursementSettings = e;
                        })
                    } else {
                        $('[href="#tabDSFundAdjustments"]').click();
                    }
                }
            });            
        })

        $('#butDisbursementScanVendorInvoices').click(function () {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/ScanVendorInvoices/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.ScanVendorInvoices.main.controller.popup({
                    success: function (vData) {
                        
                    }
                });
            })
        });
        //END DISBURSEMENT

        $('.btnOngoing').click(function () {
            NewMessageBox('This feature is ongoing development', 'Message');
        })

        // TEMP only . Dashboards grid must be dynamic
        $('#btnReportCac').click(function () {
            //$('#tabRevenueWork').opentabmenu();
            //$('#btnRevCreditAndCollection').click();
            require(['promise!/Scripts/sources/ERP/CollectionReport/controller.js?v=' + assetversion], function () {
                ERP.CollectionReport.Popup();
            });
        })

        var htmlMcrDashboard;
        $('#btnReportNetSales').click(function () {
            sessionStorage.PrintingOfflineData = false;
            location.href = '/DashboardsAndReports?OrgID=' + sessionStorage.OrgID + '&IsMobile=0&v=' + assetversion
            return
            AddBreadCrumb({
                text: 'Net Sales', Click: function () {
                    $('#divMainDashboard').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            CallPage('#divNetSales', htmlMcrDashboard, '/DashboardsAndReports', function (e) {
                htmlMcrDashboard = e;               

                $('#h5FYX6Q8AA4').removeAttr('data-role');
                $('#divHeaderForDashboard').remove();
                $('#divMainContentForDashboard').removeClass('mainContent');
                $('#divMainDashboard').pageshow();
                $('#btnMCRMoreOptionForEAS').show();

                if (typeof McrDashboard != 'undefined') {
                    McrDashboard.LoadData();
                }

                //PopupUrlPage('Sales Analytics', e, function () {
                    
                //});
            })
        })

        var htmlMcrSopa;
        $('#btnReportSopa').click(function () {
            CallPage('', htmlMcrSopa, '/DashboardsAndReports/SalesOrder', function (e) {
                htmlMcrSopa = e;
                PopupUrlPage('Sales Order and Processing Analytics', htmlMcrSopa, function () {
                    if (typeof Sopa != 'undefined') {
                        Sopa.LoadData();
                    }
                });
            });
        });

        $('#butReportProfit').click(function () {
            require(['promise!/Scripts/sources/ERP/ProfitReport/controller.js?v=' + assetversion], function () {
                ERP.ProfitReport.controller.popup();
            });
        });

        $('#butReportSalesRep').click(function () {
            require(['promise!/Scripts/sources/ERP/SalesRepsReport/controller.js?v=' + assetversion], function () {
                ERP.SalesRepsReport.controller.popup();
            });
        });
        //END dashboards grid event

        $('#btnRevTodo').click(function () {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/Todo/main/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                $('#divErpTodo, #divTodoMain').pageshow();
                AddBreadCrumb({
                    text: 'To-Do', Click: function () {
                        $('#divErpTodo, #divTodoMain').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
            });
        })

        //window.CallPage = gibalhin sa /scripts3/common/common.js para mogana gihapon ni na fx sa new UI na ginaredirect na ang paaccess  by arnie 6-8-2021 2:35 PM

        window.GotoCac = function (mode) {
            var containerCac;
            //$('#divCreditAndCollection').pageshow();

            if (mode == 'AR') {
                //location.href = '/CreditAndCollection?OrgID=' + sessionStorage.OrgID + '&mode=AR'
                //return;
                AddBreadCrumb({
                    text: 'Credit and Collection', Click: function () {
                        $('#divCreditAndCollection').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                containerCac = '#divCreditAndCollection'
            } else {
                //location.href = '/CreditAndCollection?OrgID=' + sessionStorage.OrgID + '&mode=AP'
                //return;
                $('#divDisburseMainMenu').hide();
                AddBreadCrumb({
                    text: 'Accounts Payable', Click: function () {
                        $('#divdivErpDisbursementAccountsPayable').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                containerCac = '#divdivErpDisbursementAccountsPayable';
            }
            
            if (htmlCreditAndCollection != undefined) {
                if(mode == 'AR'){
                    $('#divdivErpDisbursementAccountsPayable > *').appendTo('#divCreditAndCollection');
                    $('#divCreditAndCollection').pageshow();
                    $('#divMainDashboardOP').show();
                } else {
                    $('#divCreditAndCollection > *').appendTo('#divdivErpDisbursementAccountsPayable');
                    $('#divdivErpDisbursementAccountsPayable').pageshow();
                    $('#divMainDashboardOP').hide();
                }                
                $('#tabCacWork').opentabmenu();
                Cac.Show({
                    Mode: mode
                });
            } else {
                CallPage(containerCac, htmlCreditAndCollection, '/CreditAndCollection', function (e) {
                    $('#divCreditAndCollection nav:eq(0), #divdivErpDisbursementAccountsPayable nav:eq(0)').remove();
                    $('#divCreditAndCollection #divMainCac, #divdivErpDisbursementAccountsPayable #divMainCac').removeClass('container-fluid');
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

                        if(mode == 'AR'){
                            $('#divMainDashboardOP').show();
                        } else {
                            $('#divMainDashboardOP').hide();
                        }
                    });
                });
            }
        }

        function DisplayFowst(vParams) {
            vParams = $.extend({
                AddFowstsToDeviceIfDeviceHasNoFowst: false
            }, vParams);
            BrowsePopup('LibraryFowst', 'FOWST',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            sessionStorage.SelectedFOWST = e.ID;
                            sessionStorage.SelectedFOWSTName = e.FowstName;
                            sessionStorage.FowstID = e.ID;
                            sessionStorage.FowstName = e.FowstName;
                            sessionStorage.FowstAddress = e.Address;
                            $('#lblWorkFowstName').text(sessionStorage.FowstName);
                            if (window.IsMobile)
                                ShowOrderFulfillment();
                        }
                    }
                ], {
                    FilterFowst: 1,
                    DontCloseOnOverlayClick: true,
                    AddFowstsToDeviceIfDeviceHasNoFowst: vParams.AddFowstsToDeviceIfDeviceHasNoFowst,
                    Uuid: localStorage.UUID
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function GotoSigninPage() {

            var pParam = {};
            if (IsMobile)
                pParam.IsMobile = 1;
            pParam = $.param(pParam);
            window.location = '/SignIn/erp?' + pParam;

        }; // GotoSigninPage

        function SignOut() {
            AjaxSend({
                url: 'Login3/TestCookies',
                success: function (vData) {

                    AjaxSend({
                        url: "CRMDashboard/setSignOut",
                        success: function (vData) {
                            var pUrl = MergeUrlParameters({
                                mode: 'logout',
                                UUID: localStorage.UUID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/SignIn/erp');
                            window.open(pUrl, "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                        }
                    });

                },
                error: function (vError) {
                    GotoSigninPage();
                }
            })
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

        $('#butReportRevenueSalesQuotes').click(function() {
            ShowBigReport({
                TransType: 'Quote'
            });
        });

        $('#butReportRevenueSalesOrders').click(function() {
            ShowBigReport({
                TransType: 'SalesOrder',
                ExcludeBackorders: true
            });
        });

        $('#butReportRevenueBackOrders').click(function() {
            ShowBigReport({
                TransType: 'SalesOrder',
                BackordersOnly: true
            });
        });
        
        $('#butReportRevenueSalesInvoices').click(function() {
            ShowBigReport({
                TransType: 'Invoice'
            });
        });

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
                        Column: function(vDR) {
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
                        Column: function(vDR) {
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
                        Column: function(vDR) {
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
                        Column: function(vDR) {
                            return AccountingFormat(vDR.Total);
                        }
                    }
                ];
            } else if (vParams.TransType == 'Canvass') {
                pTitle = 'Price Canvass';
                pColumns = [
                    {
                        Label: 'PC No.',
                        Column: function(vDR) {
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
                        Column: function(vDR) {
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
                        Column: function(vDR) {
                            if (CDec2(vDR.QuotationNo) > 0)
                                return 'Finalized';

                            return 'Draft';
                        },
                        className: 'dt-body-center'
                    },
                    {
                        Label: 'Transaction Value',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.Total);
                        },
                        className: 'dt-body-center'
                    }
                ]);
            } else if (vParams.TransType == 'SalesOrder') {
                pColumns = pColumns.concat([
                    {
                        Label: 'Transaction Status', // ok
                        Column: function(vDR) {
                            if (CDec2(vDR.SalesOrderNo) > 0)
                                return 'Finalized';

                            return 'Draft';
                        },
                        className: 'dt-body-center'
                    },
                    {
                        Label: 'Transaction Value',
                        Column: function(vDR) {
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
                        Column: function(vDR) {
                            if (CDec2(vDR.InvoiceNumber) > 0)
                                return 'Finalized';

                            return 'Draft';
                        },
                        className: 'dt-body-center'
                    },
                    {
                        Label: 'Transaction Value',
                        Column: function(vDR) {
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
                            rowCallback: function(row, data, dataIndex) {
                                var pCell = $(row).find('[name=divAction]');
                                if (pCell.length == 0)
                                    return;
                                var pLink = $('<a>')
                                    .text('Preview')
                                    .attr('href', '#')
                                    .click(function(e) {
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
                            LastModifiedFilterChange: function(e) {
                                ShowPopup(e.From, e.To);
                            }
                        }
                    ], $.extend({ 
                        TransType: vParams.TransType,
                        AnyZero: 1,
                        AllowNoItems: true,
                        LastUpdatedDateFrom: FormatDate(vLastModifiedFrom, {Format: 'MMM d, yyyy'}),
                        LastUpdatedDateTo: FormatDate(vLastModifiedTo, {Format: 'MMM d, yyyy'}),
                        BackordersOnly: vParams.BackordersOnly,
                        ExcludeBackorders: vParams.ExcludeBackorders
                        //LastUpdatedDateFrom: '2020/09/01', // for debugging only
                        //LastUpdatedDateTo: '2020/09/30' // for debugging only
                    }, vParams.QueryParams)
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }

            ShowPopup();
        } // END function Find(vParams) {

        $('#butReportRevenueGrossProfitabilityValidation').click(function () {
            NewPromptUserAccess('Gross Profitability Validation', {
                OnClickOKSuccess: function () {
                    require(['/Scripts/sources/ERP/ProfitabilityValidation.js?crc=' + assetversion], function () {
                        ERP.ProfitabilityValidation.show();
                    })
                }
            });
        });

        $('#btnReportReprintRequests').click(function() {
            require(['/Scripts/sources/ERP/UnapprovedReprintRequests/controller.js?crc=' + assetversion], function () {
                ERP.UnapprovedReprintRequests.controller.show();
            })
        });

        $('#butReportPriceUpdateRequests').click(function() {
            require(['/Scripts/sources/ERP/PriceUpdateRequests/controller.js?crc=' + assetversion], function() {
                ERP.PriceUpdateRequests.controller.show();
            });
        });

        function GetCollectionReport() {
            var pData = {
                "EncodingDateRange": $.format.date(mRevenueDashboardDateFrom, 'MMM dd, yyyy') + ' to ' + $.format.date(mRevenueDashboardDateTo, 'MMM dd, yyyy') ,
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
                        error: function(vData) {
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
        if (CBool(pUrlParams.ShowAccountsPayable)) {
            $('body').hide();
            location.href = '/PurchaseOrder/Preview' + location.search; // new UI na ang PO, iredirect nalang diri
            return
            window.Z9Q3C9ASDA.ShowAccountsPayable({
                ID: pUrlParams.ID,
                AccountsPayable: true,
                ReadOnly: pUrlParams.IsReadOnly
            });
        }
        else if (CBool(pUrlParams.ShowInvoice)){
            location.href = '/Erp/Invoicing' + location.search + '&IsPreview=1';
            return
            window.Z9Q3C9ASDA.ShowInvoice({
                ID: pUrlParams.ID,
                ReadOnly: pUrlParams.IsReadOnly
            });
        }            
        else if (CBool(pUrlParams.ShowSalesOrder)) {
            location.href = '/Erp/OrderProcessing' + location.search + '&IsPreview=1';
            return
            window.Z9Q3C9ASDA.ShowSalesOrder({
                ID: pUrlParams.ID,
                ReadOnly: pUrlParams.IsReadOnly
            });
        } else if (CBool(pUrlParams.ShowQuotation)) {
            location.href = '/Erp/Quotation' + location.search + '&IsPreview=1';
        }
    })
})