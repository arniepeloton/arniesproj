define(['common', '/Scripts/jquery.dataTables.min.js', '/Scripts/jquery.scrollTo.js'],
function () {
    var mDeferred = $.Deferred();
    require([
        //'/Scripts/jquery.dataTables.buttons.min.js',
        '/Scripts/buttons.html5.min.js',
        '/Content/fancybox/jquery.fancybox.pack.js', 
        '/Scripts/sources/CreditAndCollection/CacPaymentPopup.js?v=' + assetversion,
        '/Scripts/sources/CreditAndCollection/settings/controller.js',
        '/Scripts/sources/SetupChecklist/NewPromptUser.js?crc='
    ],
    function () {
        require(['/Content/fancybox/helpers/jquery.fancybox-buttons.js'],
        function () {
            $(document).ready(function () {
                var mInvoiceID;
                var mInvoiceNumber;
                var mInvoiceDate;
                var mCustomerID;
                var mCustomerName;
                var mTableInvoiceAdjusmentsTr;
                var mLoadAdjustmentsData;
                var mContextMenuAdjustment; // 2017_02_13__10_29_05_5774 replace mContextMenuAdjustmentID with mContextMenuAdjustment.ID
                var mTableReturnedCheck;
                var mTrReturnCheck;
                var mConsumedUnappliedPayment = 0;
                var mGrossInvoiceAmount; // for adjustment dialog
                var mNetInvoiceAmount; // for adjustment dialog
                var mNetInvoiceBalance; // for adjustment dialog

                //var curCash = 0, totalPaymentRcv = 0, totalCashRcv = 0, totalCardRcv = 0, totalChequeRcv = 0, totalSrvRcv = 0, totalOPRcv = 0;
                //var mCurrentPaymentsAppliedToInvoices = 0;
                var invoiceID = '', arrAttachmentTobedeleted = [], arrNewInvoiceAttachments = [];
                var mMode; // AR (default) or AR

                window.Cac = {
                    Show: function (vParams) {
                        vParams = $.extend({
                            Mode: 'AR'
                        }, vParams);
                        mMode = GetUrlParameters().mode || vParams.Mode;
                        proj.CacMode = mMode;
                        Init();
                        $('#divCaCMainPage').pageshow();
                    },
                    InvoiceID: null
                }

                Cac.Show();

                function Init() {
                    if (mMode == 'AP') {
                        $('#lblCacTitle').text('Accounts Payable');
                        $('#divTabCac').find('[data-target=#tabCacDashboard]').text('Dashboard');
                        $('#divTabCac').find('[data-target=#tabCacWork]').text('Accounts Payable');
                        $('#divTabCac').find('[data-target=#tabCacSettings]').text('Settings');

                        $('#tbodyCustomerWithCredit').find('[name=thClientName]').text('Vendor Name');
                        $('#tbodyCustomerWithCredit').find('[name=thAccountsReceivableBalance]').text('Accounts Payable Balance');

                        $('#popupAccountDetails').find('[name=divCustomerName]').text('Vendor Name');
                        $('#butGenerateBilling').hide(); // no need for closest('.ui-btn')
                        $('#tbodyCustomerWithCredit').find('[name=thAssignedSalesTeam]').hide();                        

                        $('#tabCacWork [name=selCustomerBalanceFilter] [value=""]').text('Show vendors with balance');
                        $('#tabCacWork [name=selCustomerBalanceFilter] [value="withoutbalance"]').text('Show vendors without balance');
                        $('#tabCacWork [name=selCustomerBalanceFilter] [value="all"]').text('Show all vendors');
                    } else {
                        $('#lblCacTitle').text('Credit and Collection');
                        $('#divTabCac').find('[data-target=#tabCacDashboard]').text('Dashboard');
                        $('#divTabCac').find('[data-target=#tabCacWork]').text('Credit & Collection');
                        $('#divTabCac').find('[data-target=#tabCacSettings]').text('Settings');

                        $('#tbodyCustomerWithCredit').find('[name=thClientName]').text('Client Name');
                        $('#tbodyCustomerWithCredit').find('[name=thAccountsReceivableBalance]').text('Accounts Receivable Balance');
                    
                        $('#popupAccountDetails').find('[name=divCustomerName]').text('Customer Name');
                        $('#butGenerateBilling').show();
                        $('#tbodyCustomerWithCredit').find('[name=thAssignedSalesTeam]').show();
                    }
                    LoadData();                
                }

                window.ScrollToMiddle = function (scrollableDiv, el) {
                    $(scrollableDiv).scrollTo('#' + el.attr('id'), { offset: -$(scrollableDiv).height() / 2 });
                }

                //Init(); // i-delete na nako ni 2017_11_07__14_47_46_0043

                $('#tabCacSettingsTab').off('click').click(function () {
                    NewPromptUserAccess('Access Credit and Collection Settings*', {
                        OnClickOKSuccess: function () {
                            $('#tabCacSettings').opentabmenu();
                        }
                    });
                });

                $('body')
                //.on('click', '#btnrow', function () {
                //    PageAccountDetails()
                //})
                //.on('click', '.btnrowaccdet', function () {
                //    invoiceID = $(this).attr('id');
                //    AccDetOptions()
                //})
                .on('change', '#fileAttach', function () {
                    //AttachFile($('#fileAttach')[0].files);
                    //$(this).val('');
                    var formdata = new FormData();
                    var url = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        InvoiceID: sessionStorage.CacInvoiceID
                    }, '/CreditAndCollection/UploadInvoiceAttachments')

                    for (var i = 0; i < $('#fileAttach')[0].files.length; i++){
                        formdata.append($('#fileAttach')[0].files[i].name, $('#fileAttach')[0].files[i]);
                    }

                    Bs.Loading('show', 'Please wait while uploading file(s) ...');

                    AjaxSend({
                        url: url,
                        type: 'POST',
                        data: formdata,
                        async: true,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            Bs.Loading('hide');
                            if (vData.Results == 'saved') {
                                WebToast('File(s) was successfully uploaded', 'top');
                                ERP.AccountDetails.controller.Mode = mMode;
                                ERP.AccountDetails.controller.CustomerID = mCustomerID;
                                ERP.AccountDetails.controller.CustomerName = mCustomerName;
                                ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, 'has payable');
                            } else {
                                NewMessageBox('Thers\'s an error uploading attachment(s).', 'Message');
                            }
                        }
                    })
                })
                .on('change', '#chbImageSelectAll', function () {
                    var ischecked = $(this).is(':checked');
                    $('.chbattach').each(function () {
                        $(this).prop('checked', ischecked);
                        ToBeRemoved($(this).data('imgdata'), ischecked);
                    })
                })
                .on('click', '#btnDeleteAttachment', function () {
                    if(arrAttachmentTobedeleted.length == 0){
                        WebToast('Please select at least one image.', 'top');
                        return;
                    }

                    //tag all selected images as deleted
                    for (var i = 0; i < arrAttachmentTobedeleted.length; i++){
                        $('.clsAttachment[imgid="' + arrAttachmentTobedeleted[i].ID + '"]').addClass('deleted');
                    }

                    //get all remaining images
                    $('.clsAttachment:not(.deleted)').each(function () {
                        arrNewInvoiceAttachments.push($(this).data('imgdata'));
                    })
                                        
                    ShowGallery(arrNewInvoiceAttachments)
                    $('#chbImageSelectAll').prop('checked', false).checkboxradio('refresh'); //uncheck select all

                    //this is for testing only
                    AttachFile(arrNewInvoiceAttachments)
                    arrAttachmentTobedeleted = [];
                    arrNewInvoiceAttachments = [];
                })
                .on('click', '.btnAdjustmentRemove', function (e) {
                    e.preventDefault();
                    mContextMenuAdjustment = $(this).closest('tr').data('data');
                    PromptDeleteMemo();
                    return false;
                })
                .on('click', '#btnNewAccAdjustment, #divCustomerPaymentAdjustment', function () {
                    PromptUserAccess('Adjustment (EAS)', {
                        OnClickOKSuccess: function () {   
                            mContextMenuAdjustment = null; // BEGIN 2021_02_10__12_54_25_8823 dont comment this
                            //mInvoiceNumber = null;
                            //mInvoiceDate = null;
                            //mGrossInvoiceAmount = null;
                            //mNetInvoiceAmount = null;
                            //mNetInvoiceBalance = null;
                            LoadAdjustment({
                                New: true
                            });
                            $('#popupAdjustment').modal('hide');
                            $('#popupNewAdjustment').modal('show');
                        },
                        OnFailed: function () {
                            console.log('jsCFDTPD3GXC!');
                        }
                    });
                })
                .on('keydown', '.fieldnumber', function (e) {
                    if (!NumberDotOnly($(this), e, false)) {
                        return;
                    }                    
                })
                .on('blur', '.fieldnumber', function () {
                    //if($.trim($(this).val()) != ""){
                    //    if (CDec2($(this).val()) > 300) {
                    //        WebToast('Amount paid should not exceed the net invoice balance', 'top');
                    //        $(this).val('').focus();
                    //        return
                    //    } else {
                    //        $(this).val(AccountingFormat($(this).val()))
                    //    }
                    //}                    
                })
                .on('click', '#tbodyAccAdjustment > tr', function (e) {
                    e.preventDefault();
                    mContextMenuAdjustment = $(this).data('data');
                    AccountAdjustmentOptions();
                    return false;
                })
                
                .on('click', '#butGenerateBilling', function () {
                    PopupButtonList('', 'Options', [
                        {
                            Text: 'View Billing Statement', Click: function () {
                                GenerateBilling();
                            }
                        }, {
                            Text: 'Generate SOA Link', Click: function () {
                                Bs.Loading('show');
                                GetCustomerContactInfo({
                                    CustomerID: mCustomerID,
                                    CustomerName: mCustomerName,
                                    success: function (contactinfo) {
                                        AjaxSend({
                                            url: 'CreditAndCollection/GenerateSoaLink',
                                            data: { CustomerID: mCustomerID },
                                            success: function (e) {
                                                Bs.Loading('hide');
                                                var vRows = e.Results.Rows;
                                                var from = (CStr2(vRows[0].DateFrom) == '' ? new Date() : vRows[0].DateFrom), to = (CStr2(vRows[0].DateTo) == '' ? new Date(new Date().setDate(new Date().getDate() + 7)) : vRows[0].DateTo);

                                                from = $.format.date(new Date(from), 'MMM dd, yyyy');
                                                to = $.format.date(new Date(to), 'MMM dd, yyyy');

                                                $('#lblSOALinkDateFrom').text(from);
                                                $('#lblSOALinkDateTo').text(to);

                                                var url = e.soalinkdomain + MergeUrlParameters({
                                                    t: vRows[0].Token,
                                                    o: sessionStorage.OrgID,
                                                    f: sessionStorage.FowstID,
                                                    c: mCustomerID,
                                                }, '/CreditAndCollection/OnlineSOA')
                                                var msgsoalink = 'Good day, \n\n' +
                                                                'This is the billing statement for: ' + mCustomerName + '. \n\n' +
                                                                'Please click on the link below to view your billing and open invoices. You may also encode payments and download acknowledgment receipts via this link. ' +
                                                                '\n\n' +
                                                                url +
                                                                '\n\n' +
                                                                'Please do not share this link with unauthorized persons. This link expires in 7 days (from ' + from + ' to ' + to + ') for your security.' +
                                                                '\n\n' +
                                                                'Thank you very much and keep safe.'
                                                $('#txtgeneratedsoalink').val(msgsoalink);
                                                $('#divSoaLinkContactInfo').empty().append(contactinfo);
                                                $('#radSOALinkDomain').prop('checked', 1);
                                                $('#popupSOALink').modal('show');

                                                $('#btnCopySOALink').unbind('click').click(function () {
                                                    CopyToClipboard('#txtgeneratedsoalink');
                                                })

                                                $('[name="radSOALink"]').unbind('change').change(function () {
                                                    var val = $(this).val();

                                                    if (val == 'domain') {
                                                        url = e.soalinkdomain;
                                                    } else if (val == 'in') {
                                                        url = e.soalinkinternalipaddress;

                                                        if (CStr2(url) == '') {
                                                            WebToast('Internal Public IP is blank.', 'top');
                                                            url = e.soalinkdomain;
                                                            $('#radSOALinkDomain').prop('checked', 1);
                                                        }
                                                    } else if (val == 'ex') {
                                                        url = e.soalinkexternalipaddress;

                                                        if (CStr2(url) == '') {
                                                            WebToast('External Public IP is blank.', 'top');
                                                            url = e.soalinkdomain;
                                                            $('#radSOALinkDomain').prop('checked', 1);
                                                        }
                                                    }

                                                    url += MergeUrlParameters({
                                                        t: vRows[0].Token,
                                                        o: sessionStorage.OrgID,
                                                        f: sessionStorage.FowstID,
                                                        c: mCustomerID,
                                                    }, '/CreditAndCollection/OnlineSOA')

                                                    msgsoalink = 'Good day, \n\n' +
                                                                'This is the billing statement for: ' + mCustomerName + '. \n\n' +
                                                                'Please click on the link below to view your billing and open invoices. You may also encode payments and download acknowledgment receipts via this link. ' +
                                                                '\n\n' +
                                                                url +
                                                                '\n\n' +
                                                                'Please do not share this link with unauthorized persons. This link expires in 7 days (from ' + from + ' to ' + to + ') for your security.' +
                                                                '\n\n' +
                                                                'Thank you very much and keep safe.';

                                                    $('#txtgeneratedsoalink').val(msgsoalink);
                                                })
                                            }
                                        })
                                    }
                                })                                
                            }
                        }, {
                            Text: 'Export SOA as Plain Text', Click: function () {
                                function Copy(withnumbering) {
                                    Bs.Loading('show');
                                    AjaxSend({
                                        url: '/CreditAndCollection/GetCustomerCreditDetails',
                                        data: {
                                            Filter: 'has payable',
                                            CustomerID: mCustomerID
                                        },
                                        success: function (vData) {
                                            Bs.Loading('hide');
                                            var txtarea = $('<textarea id="txtCopyTransToPlainText" style="margin-left:-1000px;">'), msg = '';
                                            var pRows = vData.Results.Rows, soatotal = 0;

                                            if ($('body').find('#txtCopyTransToPlainText').length == 0) {
                                                $('body').append(txtarea);
                                            } else {
                                                txtarea = $('#txtCopyTransToPlainText');
                                            }

                                            msg = 'Statement of Account (SOA) as of ' + $.format.date(new Date(), 'MM.dd.yyyy') + ' \n\n';
                                            msg += 'From: ' + sessionStorage.OrgName + ' \n';
                                            msg += 'Customer: ' + mCustomerName + ' \n\n';

                                            msg += 'Good day, \n\n';
                                            msg += 'Kindly see your Open Invoices. For questions, concerns or clarifications, please feel free to get in touch with our Credit and Collection Department. Thank you very much and keep safe. \n\n';

                                            msg += '*** \n\n';

                                            for (var i = 0; i < pRows.length; i++) {
                                                var pd = pRows[i];
                                                msg += (withnumbering ? (i + 1) + '. \n' : '');
                                                msg += 'Inv. #:\t\t\t' + pd.InvoiceNumber + ' \n';
                                                msg += 'Date:\t\t\t' + $.format.date(new Date(pd.InvoiceDate), 'M/dd/yy') + ' \n';
                                                msg += 'Inv. Amount:\t\t' + AccountingFormat(pd.GrossInvoiceAmount) + ' \n';
                                                msg += 'Adjustments:\t\t' + AccountingFormat(pd.AdjustmentAmount) + ' \n';
                                                msg += 'Partial Payments:\t' + AccountingFormat(pd.TotalPayment) + ' \n';
                                                msg += 'Net Inv. Amount:\t' + AccountingFormat(pd.NetInvoiceBalance) + ' \n\n';
                                                soatotal += CDec2(pd.NetInvoiceBalance);
                                            }

                                            msg += '\n------------------------------ \n';
                                            msg += 'Grand Total:\t\t' + AccountingFormat(soatotal) + ' \n';
                                            msg += '------------------------------ \n';

                                            txtarea.val(msg).select();
                                            document.execCommand("copy");
                                            WebToast('Copied to clipboard');
                                        }
                                    });
                                }

                                NewMessageBox('Do you want to include line number in the plain text?', 'Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            Copy(true);
                                        }
                                    }, {
                                        Text: 'No', Click: function () {
                                            Copy(false);
                                        }
                                    }
                                ])
                            }
                        }
                    ])                    
                })
                .on('change', '#cbInvoiceFilter', function() {
                    ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, $('#cbInvoiceFilter').val());
                })
                .on('click', '#tbodyCustomerWithCredit tbody tr td a.WeightedAverageDaysOverdue', function (e) {
                    e.preventDefault();
                    GetCustomersWithCredit({
                        ComputeWeightedAverageDaysOverdue: true
                    });
                    return false;
                })
                .on('click', '#tbodyCustomerWithCredit tbody tr td', function () {

                    //if (pcustomerwithcredit.cell(this).index().columnVisible == 0) {
                    //    return;
                    //}

                    if (mMode == 'AP') {
                        $('#popupCustomerPayment').find('[name=TitlePrefix]').text('Supplier Payment');
                    } else {
                        $('#popupCustomerPayment').find('[name=TitlePrefix]').text('Customer Payment');
                    }
                    mCustomerID = pcustomerwithcredit.row(this).data().ID;
                    mCustomerName = pcustomerwithcredit.row(this).data().CustomerName;
                    $('.lblCustomerName').text(pcustomerwithcredit.row(this).data().CustomerName);

                    // $('#popupAccountDetails').popup().popup('open').show();
                    //BreadCrumbs('.ItemsBreadCrumbs',
                    //    [
                    //        {
                    //            Text: 'Credit And Collection', Click: function () {
                    //                $('#divCaCMainPage').pageshow();
                    //            }
                    //        }, { Text: 'Account Details' }
                    //    ]
                    //);
                    require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion], function() {
                        ERP.AccountDetails.controller.Mode = mMode;
                        ERP.AccountDetails.controller.CustomerID = mCustomerID;
                        ERP.AccountDetails.controller.CustomerName = mCustomerName;
                        $('#popupAccountDetailsContent').insertAfter($('#popupAccountDetails .popup-header'));
                        $('#popupAccountDetails').modal('show');
                        ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, 'has payable');
                    });

                });

                $(".fancybox").fancybox({
                    prevEffect: 'none',
                    nextEffect: 'none',
                    closeBtn: false,
                    helpers: {
                        title: { type: 'inside' },
                        buttons: {}
                    }
                });

                

                function LoadData() {
                    $('#divCaCMainPage').pageshow();
                    GetCustomersWithCredit();
                    //GetPaymentPopups();

                    if (location.pathname.toLocaleLowerCase() == '/dashboardsandreports') { // for mcr dashboard only by arnie may 23,17 430pm
                        GetFowst();
                    }
                }

                function GetFowst() {
                    if(OfflineMode){
                        var data = OfflineBrowser.LibraryFowst.GetData({
                            FilterFowst: 1
                        }).Rows;
                        DisplayFowst(data);
                    } else {
                        AjaxSend({
                            url: 'Browser/LibraryFowst',
                            data: {IsAll: true},
                            success: function (v) {
                                DisplayFowst(v.Results.Rows);
                            }
                        })
                    }

                    function DisplayFowst(vRows) {
                        var selCacFowst = $('#selCacFowst');
                        selCacFowst.empty();
                        for (var i = 0; i < vRows.length; i++){
                            selCacFowst.append(new Option(vRows[i].FowstName, vRows[i].ID));
                        }
                        sessionStorage.FowstID = $('#selCacFowst option:selected').val()

                        selCacFowst.unbind('change').change(function () {
                            sessionStorage.FowstID = $(this).val();
                        })
                    }
                }

                var tbodyCustomerWithCredit = $('#tbodyCustomerWithCredit');
                var pcustomerwithcredit;
                
                $('#tabCacWork [name=selCustomerBalanceFilter]').change(GetCustomersWithCredit);

                function GetCustomersWithCredit(vParams) {
                
                    vParams = $.extend({
                        //ComputeWeightedAverageDaysOverdue: false,
                    }, vParams);
                    //tbodyCustomerWithCredit.empty();
                    var pUrl;
                    var pSearchLabel;

                    if (mMode == 'AP') {
                        pUrl = '/CreditAndCollection/GetSuppliersWithPayable';
                        pSearchLabel = 'Search Vendor';
                    } else {
                        pUrl = 'CreditAndCollection/GetCustomerWithCredit';
                        pSearchLabel = 'Search Customer';
                    }
                    var pData = {
                        Filter: $('#tabCacWork [name=selCustomerBalanceFilter]').val()
                    };
                    if (vParams.ComputeWeightedAverageDaysOverdue) {
                        pData.ComputeWeightedAverageDaysOverdue = true;
                        NewMessageBox('Please wait while the server computes the weighted average days overdue for each customer. This will take several seconds up to a few minutes. You don\'t have to click the OK button. This message will automatically close once the computations are done.', 'Please Wait');
                    }

                    pData.ArtificialDelay = sessionStorage.ArtificialDelay; // for debugging
                    Bs.Loading('show');
                    AjaxSend({
                        url: pUrl,
                        data: pData,
                        success: function (vData) {
                            if (vParams.ComputeWeightedAverageDaysOverdue) 
                                NewMessageBoxClose();

                            var vRows = vData.Results.Rows;

                            if ($.fn.DataTable.isDataTable(tbodyCustomerWithCredit)) {
                                //empty the table first before destroying datatabase para dili magdoble ang header
                                tbodyCustomerWithCredit.empty();
                                pcustomerwithcredit.destroy();
                            }

                            var buttons = [
                                {
                                    text: 'Refresh',
                                    action: function (e, dt, node, config) {
                                        GetCustomersWithCredit()
                                    }
                                }
                            ];
                            if (!IsMobile) {
                                buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Credit and Collection' })
                            }

                            pcustomerwithcredit = tbodyCustomerWithCredit.DataTable({
                                "data": vRows,
                                "columns": [
                                    { "data": "CustomerName" },
                                    {
                                        "data": function (data, type, full, meta) {
                                            var str = '';

                                            if (CStr2(data.Telephone) != '') {
                                                str += 'T: ' + $.trim(data.Telephone) + '<br>'
                                            }

                                            if (CStr2(data.Mobile) != '') {
                                                str += 'M: ' + $.trim(data.Mobile) + '<br>'
                                            }

                                            if (CStr2(data.Email) != '') {
                                                str += 'E: ' + $.trim(data.Email) + '<br>'
                                            }

                                            if (CStr2(data.Website) != '') {
                                                str += 'W: ' + $.trim(data.Website) + '<br>'
                                            }

                                            return str;
                                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                                            $(td).attr('style', 'min-width:230px;').data('data', rowData).click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                Bs.Loading('show');
                                                var data = $(this).data('data');
                                                GetCustomerContactInfo({
                                                    CustomerID: data.ID,
                                                    CustomerName: data.CustomerName,
                                                    success: function (msg) {
                                                        Bs.Loading('hide');
                                                        NewMessageBox(msg, 'Contact Details')
                                                    }
                                                })
                                            })
                                        }
                                    },
                                    {
                                        "data": function (data, type, full, meta) {
                                            return CStr2(data.BusinessIndustry)
                                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                                            $(td).attr('style', 'min-width:200px;');
                                        }
                                    },
                                    {
                                        "data": function (data, type, full, meta) {
                                            return CStr2(data.CreditTerms)
                                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                                            $(td).attr('style', 'min-width:100px;');
                                        }
                                    },
                                    {
                                        "data": function (data, type, full, meta) {
                                            return CStr2(data.CreditLimit)
                                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                                            $(td).attr('style', 'min-width:150px;');
                                        }
                                    },
                                    { "data": "NetInvoiceBalance" },
                                    { "data": "WeightedAverageDaysOverdue" },
                                    { "": "" }
                                ],
                                "columnDefs": [                                    
                                    //{
                                    //    'targets': [1],
                                    //    'searchable': false,
                                    //    'orderable': false,
                                    //    'className': 'dt-body-center',
                                    //    'render': function (data, type, full, meta) {
                                    //        return '';
                                    //    }
                                    //},
                                    //{
                                    //    'targets': [2],
                                    //    'searchable': false,
                                    //    'orderable': false,
                                    //    'className': 'dt-body-center',
                                    //    'render': function (data, type, full, meta) {
                                    //        return '';
                                    //    }
                                    //},
                                    //{
                                    //    'targets': [3],
                                    //    'searchable': false,
                                    //    'orderable': false,
                                    //    'className': 'dt-body-center',
                                    //    'render': function (data, type, full, meta) {
                                    //        return '';
                                    //    }
                                    //},
                                    //{
                                    //    'targets': [4],
                                    //    'searchable': false,
                                    //    'orderable': false,
                                    //    'className': 'dt-body-center',
                                    //    'render': function (data, type, full, meta) {
                                    //        return '';
                                    //    }
                                    //},
                                    {
                                        'targets': [5],
                                        'searchable': true,
                                        'orderable': true,
                                        'className': 'dt-body-right',
                                        'render': function (data, type, full, meta) {
                                            return AccountingFormat(data);
                                        }
                                    },
                                    {
                                        'targets': [6],
                                        'searchable': true,
                                        'orderable': true,
                                        'className': 'dt-body-right',
                                        'render': function (data, type, full, meta) {
                                            if (vData.OutdatedWeightedAverageDaysOverdue)
                                                return '<a class=WeightedAverageDaysOverdue href="">Click here to recompute</a>';
                                            else
                                                return AccountingFormat(data);
                                        }
                                    },
                                    {
                                        'targets': [7],
                                        'searchable': false,
                                        'orderable': false,
                                        'className': 'dt-body-center',
                                        "visible": (mMode == 'AP' ? false : true),
                                        'render': function (data, type, full, meta) {
                                            return '';
                                        }
                                    }
                                ],
                                "language": {
                                    "search": pSearchLabel + " "
                                },
                                sDom: '<"top">rt<"bottom"ip><"clear">',
                                dom: 'Bfrtip',
                                //buttons: buttons,
                                initComplete: function () {
                                    $('#tbodyCustomerWithCredit').removeClass('dataTable');
                                    $('#tbodyCustomerWithCredit').find('thead tr').css({ 'font-size': '12px', 'vertical-align': 'top', 'text-align': 'center' });
                                },
                                "order": []
                            });

                            if(mMode == 'AP'){
                                $('#tbodyCustomerWithCredit').find('[name=thAssignedSalesTeam]').hide();
                            } else {
                                $('#tbodyCustomerWithCredit').find('[name=thAssignedSalesTeam]').show();
                            }

                            $('#txtCacSearch').unbind('keydown focus').keydown(function () {
                                pcustomerwithcredit.search($.trim($(this).val())).draw();
                            }).focus(function () {
                                $(this).select();
                            })

                        //    if(vRows.length > 0){
                        //        for (var i = 0; i < vRows.length; i++){
                        //            tbodyCustomerWithCredit.append($('<tr>')
                        //                    .data('data', vRows[i])
                        //                    .append($('<td>').text(vRows[i].CustomerName))
                        //                    .append($('<td>').text(''))
                        //                    .append($('<td>').text(''))
                        //                    .append($('<td>').text(''))
                        //                    .append($('<td>').text(''))
                        //                    .append($('<td>').css('text-align', 'right').text(AccountingFormat(vRows[i].NetInvoiceBalance)))
                        //                    .append($('<td>').text(''))
                        //                    .click(function () {
                        //                        mCustomerID = $(this).data('data').ID;
                        //                        mCustomerName = $(this).data('data').CustomerName;
                        //                        $('.lblCustomerName').text($(this).data('data').CustomerName);
                        //                        $('#popupAccountDetails').show();
						//// $('#popupAccountDetails').popup().popup('open').show();
                        //                        BreadCrumbs('.ItemsBreadCrumbs',
                        //                            [
                        //                                {
                        //                                    Text: 'Credit And Collection', Click: function () {
                        //                                        $('#divCaCMainPage').pageshow();
                        //                                    }
                        //                                }, { Text: 'Account Details' }
                        //                            ]
                        //                        );
                        //                        GetCustomerCreditDetails(mCustomerID, 'has payable');
                        //                    })
                        //                )
                        //        }

                        //        tbodyCustomerWithCredit.parent().table('refresh');
                        //    } else {
                        //        tbodyCustomerWithCredit.append('<tr><td colspan="7" style="text-align:center;">No Result(s) Found</td></tr>')
                        //    }
                        },
                        complete: function() {
                            Bs.Loading('hide');
                        }
                    })
                } // END function GetCustomersWithCredit() {
                window.Cac.GetCustomersWithCredit = GetCustomersWithCredit;

                window.Cac.GetCustomerContactInfo = GetCustomerContactInfo;
                function GetCustomerContactInfo(params) {
                    AjaxSend({
                        url: '/CreditAndCollection/GetCustomerContactInfo',
                        data: { CustomerID: params.CustomerID },
                        success: function (e) {
                            Bs.Loading('hide');

                            var tel = e.Telephone, mobile = e.Mobile, email = e.Email, website = e.Website;
                            var msg = 'Customer Name: <b>' + params.CustomerName + '</b> <br><br>';

                            msg += '<b>Telephone</b><br><ol>'
                            if (tel.length > 0) {
                                for (var i = 0; i < tel.length; i++) {
                                    msg += '<li>' + (CStr2(tel[i].CountryCode).split('-')[1] == undefined ? "" : "+" + $.trim(CStr2(tel[i].CountryCode).split('-')[1])) + " (" + (CStr2(tel[i].AreaCode).split('-')[1] == undefined ? $.trim(CStr2(tel[i].AreaCode).split('-')[0]) : $.trim(CStr2(tel[i].AreaCode).split('-')[1])) + ") " + $.trim(tel[i].TelephoneNo) + ($.trim(tel[i].ExtensionNo) == "" ? "" : "." + $.trim(tel[i].ExtensionNo)) + '</li>';
                                }
                            } else {
                                msg += '<li>No results found</li>';
                            }

                            msg += '</ol>';

                            msg += '<b>Mobile</b><br><ol>';
                            if (mobile.length > 0) {
                                for (var i = 0; i < mobile.length; i++) {
                                    msg += '<li>' + "+" + $.trim(mobile[i].CountryCode.split('-')[1]) + "" + (mobile[i].MobileNo.length <= 7 ? mobile[i].Prefix : '') + $.trim(mobile[i].MobileNo) + '</li>';
                                }
                            } else {
                                msg += '<li>No results found</li>';
                            }

                            msg += '</ol>';

                            msg += '<b>Email Address</b><br><ol>';
                            if (email.length > 0) {for (var i = 0; i < email.length; i++) {
                                    msg += '<li><a href="mailto: ' + email[i].EmailAdd + '">' + email[i].EmailAdd + '</a></li>';
                                }
                            } else {
                                msg += '<li>No results found</li>';
                            }

                            msg += '</ol>';

                            msg += '<b>Website</b><br><ol>';
                            if (website.length > 0) {
                                for (var i = 0; i < website.length; i++) {
                                    msg += '<li><a href="' + website[i].Website + '" target="_blank">' + website[i].Website + '</a></li>';
                                }
                            } else {
                                msg += '<li>No results found</li>';
                            }

                            msg += '</ol>';

                            params.success(msg);
                        }
                    })
                }

                //function GetCustomerCreditDetails(id, vFilter) {
                //    var pUrl;
                //    var pData = {
                //        Filter: vFilter
                //    };
                //    if (mMode == 'AP') {
                //        pUrl = '/CreditAndCollection/GetSupplierReceivablesDetails';
                //        pData.SupplierID = id;
                //    } else {
                //        pUrl = '/CreditAndCollection/GetCustomerCreditDetails';
                //        pData.CustomerID = id;
                //    }
                //    AjaxSend({
                //        url: pUrl,
                //        data: pData,
                //        success: function (vData) {
                //            var pCreditBegBalance = CDec2(GetStringObject('Data.CreditBegBalance', vData));
                //            var pCreditTerm = CInt2(GetStringObject('Data.CreditTerm', vData));
                //            var pCreditBalanceAsOf = CStr2(GetStringObject('Data.CreditBalanceAsOf', vData));
                //            var pCreditLimit = CDec2(GetStringObject('Data.CreditLimit', vData));
                //            var pAvailableCredit = pCreditLimit - vData.NetInvoiceBalance;
                //            var vRows = vData.Results.Rows;
                //            arrCreditDetails = vRows;
                //            DisplayInvoices({
                //                BeginningBalance: pCreditBegBalance,
                //                Overpayment: vData.Overpayment
                //            });
                //            $('#lblBeginningBalance').text(AccountingFormat(pCreditBegBalance));
                //            $('#lblBeginningBalanceAsOf').text(pCreditBalanceAsOf);
                //            $('#lblUnappliedPayments').text(AccountingFormat(vData.Overpayment));
                //            $('#lblExcessPayments').text(AccountingFormat(vData.ExcessPayments));
                //            $('#lblTotalOfUnappliedCreditAndExcessPayments').text(AccountingFormat(CDec2(vData.Overpayment) + CDec2(vData.ExcessPayments)));

                //            $('[name=spanCustomerCreditLimit]').text(AccountingFormat(pCreditLimit));
                //            $('[name=spanCustomerCreditTerms]').text(AccountingFormat(pCreditTerm, 0));
                //            $('[name=spanCustomerAvailableCredit]').text(AccountingFormat(pAvailableCredit));
                //            $('[name=spanCustomerAgingOfAr]').text(AccountingFormat(vData.AgingOfAr));
                //        }
                //    });
                //}

                function AccountAdjustmentOptions() {
                    var pOptions = [];

                    if (mContextMenuAdjustment.Reason == 'Sales Returns') 
                        pOptions.push({
                            Text: 'Print Memo',
                            Click: function () {
                                CreateSalesReturnPdf();
                            }
                        });
                    else
                        pOptions.push({
                            Text: 'Edit Memo',
                            Click: function () {
                                LoadAdjustment({
                                    New: false   
                                });
                                $('#popupNewAdjustment').modal('show');
                            }
                        });

                    pOptions.push({
                        Text: 'Delete Memo',
                        Click: function() {
                            PromptDeleteMemo();
                        }
                    });
                    


                    PopupButtonList('', 'Options', pOptions);
                }

                function CreateSalesReturnPdf() {
                    Bs.Loading('show');
                    AjaxSend({
                        url: '/SalesReturns/CreateEasPdf',
                        data: {
                            MemoID: mContextMenuAdjustment.MemoID
                        },
                        success: function(vData) {
                            if (vData.Error) {
                                NewMessageBox(vData.Error);
                                return;
                            }
                            ViewReport(vData.BaseFilename);
                            NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                        },
                        complete: function() {
                            Bs.Loading('hide');
                        }
                    });
                }

                function ViewReport(vFilename) {
                    var pUrl = MergeUrlParameters({
                        Filename: vFilename
                    }, '/Erp/DownloadPrintablePdf');
                    window.open(pUrl, '_blank', 
                        'width=' + window.innerWidth + 
                        ',height=' + window.innerHeight + 
                        ',scrollbars=auto');
                } // END function ViewReport() {

                function PromptDeleteMemo() {
                    NewMessageBox('Are you sure you want to delete ' + mContextMenuAdjustment.AdjustmentType + ' #' + mContextMenuAdjustment.AdjustmentRef + '?', 'Delete Memo', [
                        {
                            Text: 'Yes',
                            Click: function() {
                                console.log('code to ajax send delete memo js3SD2KXLWUZ!');
                                AjaxSend({
                                    url: '/CreditAndCollection/DeactivateMemo',
                                    data: {ID: mContextMenuAdjustment.ID},
                                    success: function(vData) {
                                        if (vData.Error)
                                            NewMessageBox(vData.Error);
                                        else if (vData.Result == 'ok')
                                            NewMessageBox('Adjustment was successfully deleted.');
                                        LoadAdjustments();
                                        ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, $('#cbInvoiceFilter').val());
                                        GetCustomersWithCredit();
                                    }
                                });
                            }
                        },
                        {
                            Text: 'No'
                        }
                    ]);
                }

                function AttachFile(file) {
                    var allowedFiles = [];
                    for (var i = 0; i < file.length; i++) {
                        if (ValidateAttachFile(file[i])) {
                            file[i].ID = NewID(); //this is temp only
                            file[i].InvoiceID = invoiceID;
                            file[i].State = 'new';
                            allowedFiles.push(file[i]);
                        }
                    }
                    $('#' + invoiceID).data('attachments', allowedFiles)
                        .find('td:eq(10)').text(file.length + (file.length == 0 ? '' : (file.length == 1 ? ' file' : ' files')));
                }

                function LoadImages(imgid, file) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $('[imglink="' + imgid + '"]').attr('href', e.target.result);
                        $('[imgid="' + imgid + '"]').attr('src', e.target.result).attr('alt', 'Unable to load');
                    }
                    reader.readAsDataURL(file)
                }

                function ValidateAttachFile(file) {
                    if (file.type == "image/jpeg" || file.type == "image/png") {
                        return true;
                    } else {
                        WebToast('Opps!');
                        return false;
                    }
                }

                Cac.ShowGallery = ShowGallery;
                function ShowGallery(dataimages) {
                    var divImageGallery = $('#popupContentInvoiceAttachments');
                    var grid = $('<div class="ui-grid-a">');
                    var ctrRow = 0;

                    divImageGallery.empty();

                    //let's create first the container of images before we load the image
                    for (var i = 0; i < dataimages.length; i++) {
                        var block = '';
                        if (ctrRow == 2) {
                            ctrRow = 0;
                            divImageGallery.append(grid);
                            grid = $('<div class="ui-grid-a">');
                        }

                        if (ctrRow == 0) {
                            block = 'a';
                        } else {
                            block = 'b';
                        }

                        grid.append($('<div class="ui-block-' + block + '" id="block-' + dataimages[i].ID + '">')
                                .css({
                                    padding: '3px',
                                    position: 'relative'
                                })
                                .append($('<a data-role="none">')
                                    .css('position', 'relative')
                                    .attr('imglink', dataimages[i].ID)
                                    .attr('rel', dataimages[i].InvoiceID) // used for fancy box plugin
                                    //.addClass('fancybox')
                                    .append($('<img style="width:100%;">')
                                        .data('imgdata', dataimages[i])
                                        .attr('imgid', dataimages[i].ID)
                                        .attr('alt', 'Loading')
                                        .attr('src', location.origin + '/InvoicesFiles/' + sessionStorage.OrgID + '/' + dataimages[i].InvoiceID + '/' + dataimages[i].FileName)
                                        .addClass('clsAttachment')
                                    )                                    
                                ).append($('<div>')
                                    .addClass('clsdelete')
                                    .css({
                                        position: 'absolute',
                                        top: 0,
                                        right: 0,
                                        height: '50px',
                                        width: '50px'
                                    })
                                    .append($('<button data-role="none" class="btn custom-btn-theme-c btnCircle-xs">')
                                        .data('imgdata', dataimages[i])
                                        .append($('<span class="glyphicon glyphicon-remove">'))
                                        .click(function () {
                                            var data = $(this).data('imgdata');

                                            NewMessageBox('Are you sure you want to delete this file?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        AjaxSend({
                                                            url: 'CreditAndCollection/DeleteInvoiceFiles',
                                                            data: { ID: data.ID },
                                                            success: function (e) {
                                                                ERP.AccountDetails.controller.Mode = mMode;
                                                                ERP.AccountDetails.controller.CustomerID = mCustomerID;
                                                                ERP.AccountDetails.controller.CustomerName = mCustomerName;
                                                                ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, 'has payable');

                                                                $('#block-' + data.ID).empty().text('File was deleted');
                                                            }
                                                        })
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                    //.append($('<label class="checkbox-only" for="chbattach-' + i + '">')
                                    //    .append($('<input type="checkbox" class="chbattach" name="chbattach" id="chbattach-' + i + '" data-iconpos="notext"/>')
                                    //        .checkboxradio()
                                    //        .checkboxradio('refresh')
                                    //        .data('imgdata', dataimages[i])
                                    //        .click(function () {
                                    //            ToBeRemoved($(this).data('imgdata'), $(this).is(':checked'));
                                    //        })
                                    //    )
                                    //)
                                )
                            )

                        if (i == (dataimages.length - 1)) {
                            divImageGallery.append(grid);
                        }

                        ctrRow += 1;
                    }

                    $('.chbattach').trigger('create').checkboxradio('refresh')

                    //lets load the images since we finished creating the image container/tag
                    //for (var i = 0; i < dataimages.length; i++) {
                    //    LoadImages(dataimages[i].ID, dataimages[i])
                    //}

                    $('#popupInvoiceViewAttachments').modal('show');
                }

                function ToBeRemoved(data, ischecked) {
                    if (arrAttachmentTobedeleted.length > 0) {
                        if (ischecked) {
                            arrAttachmentTobedeleted.push(data);
                        } else {
                            arrAttachmentTobedeleted = $.grep(arrAttachmentTobedeleted, function (e) {
                                return e.ID != data.ID;
                            })
                        }
                    } else {
                        arrAttachmentTobedeleted.push(data);
                    }
                }

                function GetPaymentPopups() {
                    AjaxSend({
                        ExactUrl: '/SalesMenu/FMCGPopup?cache=1&OrgID=' + sessionStorage.OrgID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            $('#divPaymentsPopup').empty();
                            $('#divPaymentsPopup').append(vData).trigger('create');
                        }
                    });
                }


                
                Cac.LoadAdjustments = LoadAdjustments;
                function LoadAdjustments(vData) {
                    
                    if (vData == null)
                        vData = mLoadAdjustmentsData;
                    else
                        mLoadAdjustmentsData = vData;
                        
                    vData = $.extend({
                        InvoiceID: null,
                        InvoiceNumber: null
                    }, vData);
                    mInvoiceID = vData.InvoiceID;
                    if (CStr2(mInvoiceID) == '')
                        return;
                    mInvoiceNumber = vData.InvoiceNumber;
                    mInvoiceDate = vData.InvoiceDate;
                    mGrossInvoiceAmount = vData.GrossInvoiceAmount;
                    mNetInvoiceAmount = vData.NetInvoiceAmount;
                    mNetInvoiceBalance = vData.NetInvoiceBalance;

                    var pUrl;
                    var pData = {
                        InvoiceID: vData.InvoiceID,
                        AccountsPayableID: vData.AccountsPayableID
                    };
                    if (proj.CacMode == 'AP') {
                        pUrl = '/Receiving/GetCacheInvoice';
                        //pData.SupplierID = vData.SupplierID;
                        //pData.ReceivingID = vData.ID;
                    } else
                        pUrl = '/SalesMenu/GetCacheInvoice';

                    AjaxSend({
                        url: pUrl,
                        data: pData,
                        success: function(vData) {
                            DisplayInvoice(vData.Invoice);
                            DisplayAdjustments(vData.Adjustments.Rows);
                        }
                    }); // END AjaxSend({

                    function DisplayInvoice(vData) {
                        var pTBody = $('#tableAdjustmentInvoice').find('tbody');
                        var pTR = pTBody.find('tr:first');
                        pTR.find('[name=InvoiceDate]').text(vData.InvoiceDate);
                        pTR.find('[name=InvoiceNumber]').text(vData.InvoiceNumber);
                        pTR.find('[name=GrossInvoiceAmount]').text(AccountingFormat(vData.GrossInvoiceAmount));
                        pTR.find('[name=NetAdjustments]').text(AccountingFormat(vData.AdjustmentAmount));
                        $('#tdNetAdjustmentsToInvoice').text(AccountingFormat(vData.AdjustmentAmount));
                        pTR.find('[name=NetInvoiceAmount]').text(AccountingFormat(vData.NetInvoiceAmount));
                        pTR.find('[name=AccumPrevPayments]').text(AccountingFormat(vData.TotalPayment));
                        pTR.find('[name=NetInvoiceBalance]').text(AccountingFormat(vData.NetInvoiceBalance));
                        pTR.find('[name=InvoiceTerms]').text(AccountingFormat(vData.CreditTerms));
                        pTR.find('[name=InvoiceDueDate]').text(vData.InvoiceDueDate);
                    } // END function DisplayInvoice(vData) {

                    function DisplayAdjustments(vData) {
                        var pTBody = $('#tableInvoiceAdjustments').find('tbody');
                        if (mTableInvoiceAdjusmentsTr == null) 
                            mTableInvoiceAdjusmentsTr = pTBody.find('tr:first').clone();
                        pTBody.html('');
                        for (var p = 0; p <= vData.length - 1; p++) {
                            var pDR = vData[p];
                            var pTR = mTableInvoiceAdjusmentsTr.clone();
                            pTR.find('[name=AdjustmentDate]').text(pDR.AdjustmentDate);
                            pTR.find('[name=AdjustmentRef]').text(pDR.AdjustmentRef);
                            pTR.find('[name=AdjustmentType]').text(pDR.AdjustmentType);
                            pTR.find('[name=Reason]').text(pDR.Reason);
                            pTR.find('[name=Amount]').text(AccountingFormat(pDR.Amount));
                            pTR.find('[name=Remarks]').text(pDR.Remarks);
                            pTR.find('[name=UpdatedBy]').text(pDR.UpdateByName);
                            pTR.data('data', pDR);
                            pTBody.append(pTR);
                        }
                    } // END function DisplayAdjustments(vData) {
                } // END function LoadAdjustments(vInvoiceID) {

                Cac.LoadAdjustment = LoadAdjustment;
                function LoadAdjustment(vParams) {
                    vParams = $.extend({
                        New: false,
                        ID: null
                    }, vParams);
                    
                    $('#butInvoiceToAdjust')
                        .text(mInvoiceNumber)
                        //.button('refresh'); // 2017_04_07__16_44_19_4229 button has been converted to span
                    $('#spanAdjustmentCustomer').text(mCustomerName);
                    $('#spanAdjustmentInvoiceToDate').text(mInvoiceDate);
                    $('#spanAccountAdjustmentGrossInvoiceAmount').text(AccountingFormat(mGrossInvoiceAmount));
                    $('#spanAccountAdjustmentNetInvoiceAmount').text(AccountingFormat(mNetInvoiceAmount));
                    $('#spanAccountAdjustmentNetInvoiceBalance').text(AccountingFormat(mNetInvoiceBalance));
                    LoadReasons();

                    if (vParams.New) {
                        $('#txtAdjustmentAmount').val('');
                        $('#txtAdjustmentRemarks').val('');
                    } else
                        GetAdjustment();

                    $('#divAccountAdjustmentChecks').hide();
                    mTableReturnedCheck.find('tbody').html('');

                    function GetAdjustment() {
                        
                        var pData = {};

                        if (mContextMenuAdjustment)
                            pData.AdjustmentID = mContextMenuAdjustment.ID;
                        AjaxSend({
                            url: '/CreditAndCollection/GetAdjustment',
                            data: pData,
                            success: function(vData) {
                                if (vData.Master.Rows.length == 0) {
                                    NewMessageBox('Record does not exist.');
                                    return;
                                }
                                var pDR = vData.Master.Rows[0];
                                $('#txtAdjustmentRemarks').val(pDR.Remarks);
                                var pCB = $('#cbReasonForAdjustment');
                                pCB.find('option[value=' + pDR.ReasonID + ']')
                                    .prop('selected', true);
                                //pCB.selectmenu('refresh'); // error
                                pCB.change();
                                //if ($('#cbReasonForAdjustment Option:selected').text().indexOf('Returned Checks') != -1) {
                                //    $('#popupNewAdjustment [name=divNewAdjustmentInvoice]').hide();
                                //    $('#divAccountAdjustmentChecks').show();
                                //} else {
                                //    $('#divAccountAdjustmentChecks').hide();
                                //    $('#popupNewAdjustment [name=divNewAdjustmentInvoice]').show();
                                //}
                                var pDT = vData.ReturnedChecks;
                                for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                                    pDR = pDT.Rows[p];
                                    AddReturnedCheckTableRow(pDR);
                                }
                                $('#txtAdjustmentAmount').val(AccountingFormat(vData.Master.Rows[0].Amount, null, null, '', ''));
                            }
                        }); // END AjaxSend({

                    } // END function GetAdjustment() {
                    
                    function LoadReasons() {
                        var pData = {};
                        if (proj.CacMode == 'AP') 
                            pData.AccountType = 'ap';
                        else
                            pData.AccountType = 'ar';

                        AjaxSend({
                            url: '/Browser/LibraryAccountAdjustments',
                            data: pData,
                            success: function(vData) {
                                DisplayReasons(vData.Results.Rows);
                            }
                        });
                    } // END function LoadReasons() {

                    function DisplayReasons(vData) {
                        var pCb = $('#cbReasonForAdjustment');
                        pCb.html('');
                        pCb.append(new Option('Please select'));
                        for (var p = 0; p <= vData.length - 1; p++) {
                            var pDR = vData[p];
                            //var pReason = pDR.Reason + ' - ' + pDR.AdjustmentType + ' - ' + pDR.Effect;
                            var pReason = pDR.Display;
                            var pOption = new Option(pReason, pDR.ID);
                            pCb.append(pOption);
                        }
                    } // END function DisplayReasons() {
                } // END function LoadAdjustment() {

                $('#cbReasonForAdjustment').change(function() {
                    console.log('cbReasonForAdjustment change js2SNVMADUC2!');
                    var pText = $('#cbReasonForAdjustment Option:selected').text();
                    if (pText.indexOf('Returned Checks') != -1) {
                        $('#divAccountAdjustmentChecks').show();
                        $('#popupNewAdjustment [name=divNewAdjustmentInvoice]').hide();
                        $('#txtAdjustmentAmount').attr('readonly', true);
                    } else {
                        $('#divAccountAdjustmentChecks').hide();
                        $('#popupNewAdjustment [name=divNewAdjustmentInvoice]').show();
                        $('#txtAdjustmentAmount').attr('readonly', false);
                    }

                    if (pText.indexOf('Sales Return') != -1) {
                        require(['promise!/Scripts/sources/ERP/SalesReturn/work/controller.js'], function() {
                            if ($('#popupSalesReturn').length == 0)
                                console.log('#popupSalesReturn does not exist. ref: jsDCYMVWAJK8!');
                            $('#popupNewAdjustment').modal('hide');
                            $('#popupSalesReturn').modal('show');                            
                            if (mInvoiceID) {
                                ERP.SalesReturn.work.controller.RepopupAdjustment = true;
                                ERP.SalesReturn.work.controller.LoadTrans({ID: mInvoiceID});
                            }
                            //ERP.SalesReturn.controller();
                        });
                    } 
                });

                $('#divAccountAdjustmentChecks [name=butAdd]').click(function() {
                    BrowsePopup('TransCustomerPaymentChecks', 'Checks',
                        [
                            {
                                StyleFormat: 'table',
                                AutoSearch: false,
                                DisplayText: [
                                    {
                                        Label: 'Bank',
                                        Column: 'BankName'
                                    },
                                    {
                                        Label: 'Check No.',
                                        Column: 'CheckNumber'
                                    },
                                    {
                                        Label: 'Check Amount',
                                        Column: 'CheckAmount'
                                    },
                                    {
                                        Label: 'Check Date',
                                        Column: 'CheckDate'
                                    },
                                    {
                                        Label: 'Invoice No.',
                                        Column: function(vDR) {
                                            var pStr = '';
                                            if (CStr2(vDR.InvoiceMin) != '')
                                                pStr = vDR.InvoiceMin + '-';
                                            pStr += vDR.InvoiceNumber;
                                            vDR.InvoiceNumText = pStr;
                                            return pStr;
                                        }
                                    },
                                    {
                                        Label: 'Invoice Date',
                                        Column: function(vDR) {
                                            vDR.InvoiceDateText = FormatDate(new Date(vDR.InvoiceDate), {Format: 'MMMM d, yyyy h:mm a'});
                                            return vDR.InvoiceDateText;
                                        }
                                    },
                                    {
                                        Label: 'Invoice Amount',
                                        Column: function(vDR) {
                                            vDR.InvoiceAmountText = AccountingFormat(vDR.InvoiceAmount);
                                            return vDR.InvoiceAmountText;
                                        }
                                    },
                                    {
                                        Label: 'Debit Amount',
                                        Column: function(vDR) {
                                            vDR.ChequeAmountPaidText = AccountingFormat(vDR.ChequeAmountPaid);
                                            return vDR.ChequeAmountPaidText;
                                        }
                                    }
                                ],
                                Click: function (e) {
                                    PromptUserAccess('Adjustment (EAS)', {
                                        OnClickOKSuccess: function () {    
                                            GetChequeInvoices(e.ID);
                                            //AddReturnedCheckTableRow(e);
                                            $('#popupNewAdjustment').modal('show');
                                        },
                                        OnFailed: function () {
                                            console.log('jsCFDTPD3GXC!');
                                        }
                                    });
                                    
                                }
                            }
                        ], 
                        { 
                            CustomerID: mCustomerID
                        }
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',
                });

                function GetChequeInvoices(vCustomerPaymentChequeID) {
                    if (!ReturnedCheque_BeforeAdd(vCustomerPaymentChequeID))
                        return;

                    AjaxSend({
                        url: '/CreditAndCollection/GetChequeDistribution',
                        data: {
                            CustomerPaymentChequeID: vCustomerPaymentChequeID
                        },
                        success: function(vData) {
                            if (vData.Error) {
                                NewMessageBox(vData.Error);
                                return;
                            }
                            if (!$.isArray(vData)) {
                                NewMessageBox(vData);
                                return;
                            }
                            for(var pIndex = 0; pIndex <= vData.length - 1; pIndex++) {
                                var pDR = vData[pIndex];
                                pDR.ID = pDR.CustomerPaymentCheckID;
                                
                                pDR.ChequeAmountPaidText = pDR.AmountPaid;
                                pDR.InvoiceAmount = pDR.GrossInvoiceAmount;
                                pDR.ChequeAmountPaid = pDR.AmountPaid;
                                AddReturnedCheckTableRow(pDR);
                            }
                        },
                        error: function(vParams) {
                            NewMessageBox('An error occured while getting cheque invoices. ' + vParams.responseText);
                        }
                    });
                }

                function ReturnedCheque_BeforeAdd(vCustomerPaymentChequeID) {
                    var pTRs = mTableReturnedCheck.find('tbody tr');
                    for (var p = 0; p <= pTRs.length - 1; p++) {
                        var pTR = $(pTRs[p]);
                        var pDR = pTR.data('data');
                        if (ReadyGuid(pDR.ID) == ReadyGuid(vCustomerPaymentChequeID)) {
                            NewMessageBox('That cheque is already on the table.', 'Duplicate Entry');
                            return false;
                        }
                    }

                    return true;
                } // END ReturnedCheque_BeforeAdd

                function AddReturnedCheckTableRow(vDR) {
                    var pTR = mTrReturnCheck.clone();
                    pTR.data('data', vDR);
                    pTR.find('[name=bank]').text(vDR.BankName);
                    pTR.find('[name=checkNumber]').text(vDR.CheckNumber);
                    pTR.find('[name=checkAmount]').text(AccountingFormat(vDR.CheckAmount));
                    pTR.find('[name=checkDate]').text($.format.date(new Date(vDR.CheckDate), 'MMM dd, yyyy'));

                    var pStr = '';
                    if (CStr2(vDR.InvoiceMin) != '')
                        pStr = vDR.InvoiceMin + '-';
                    pStr += vDR.InvoiceNumber;
                    vDR.InvoiceNumText = pStr;
                    pTR.find('[name=invoiceNum]').text(vDR.InvoiceNumText);

                    vDR.InvoiceDateText = FormatDate(new Date(vDR.InvoiceDate), {Format: 'MMMM d, yyyy h:mm a'});
                    pTR.find('[name=invoiceDate]').text(vDR.InvoiceDateText);

                    vDR.InvoiceAmountText = AccountingFormat(vDR.InvoiceAmount);
                    pTR.find('[name=invoiceAmount]').text(vDR.InvoiceAmountText);

                    vDR.ChequeAmountPaidText = AccountingFormat(vDR.ChequeAmountPaid);
                    pTR.find('[name=debitAmount]').text(vDR.ChequeAmountPaidText);
                    mTableReturnedCheck.append(pTR);

                    $('#txtAdjustmentAmount').val(CDec2($('#txtAdjustmentAmount').val()) + CDec2(vDR.ChequeAmountPaid));
                }

                // 2017_04_07__16_44_19_4229 button has been converted to span
                $('#butInvoiceToAdjust').on('click', function() {
                    BrowsePopup('TransInvoices', 'Invoice',
                        [
                            {
                                //AutoSearch: true,
                                ModalSize: 'fullwidth',
                                StyleFormat: 'table',
                                DisplayText: [
                                    {
                                        Label: 'Invoice No.',
                                        Column: function(vDR) {
                                            var pOut = '';
                                            if (CStr2(vDR.FowstWideInvoiceNumber).trim() != '')
                                                pOut = vDR.FOWSTNo + '-' + vDR.FowstWideInvoiceNumber;
                                            else if (CStr2(vDR.MIN).trim() != '') 
                                                pOut = vDR.MIN + '-' + vDR.InvoiceNumber;
                                            else
                                                pOut = vDR.InvoiceNumber;
                                            if (CStr2(vDR.DocumentName).trim() == '')
                                                return '<a href="#">' + 'SI-' + pOut + '</a>';
                                            else
                                                return '<a href="#">' + vDR.DocumentName + '-' + pOut + '</a>';
                                        }, CreatedCell: function (td, cellData, rowData, row, col) {
                                            $(td).data('data', rowData)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    PeekView({
                                                        ID: $(this).data('data').ID,
                                                        DontReplaceCurrentID: vParams.DontReplaceCurrentID,
                                                        IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                                                        TransType: vParams.TransType
                                                    });
                                                })
                                        }
                                    },
                                    {
                                        Label: 'Sales Order No.',
                                        Column: function(vDR) {
                                            return CStr2(vDR.InvoiceSalesOrderNo);
                                        }
                                    },
                                    {
                                        Label: 'Manual Invoice No.',
                                        Column: 'RefInvoiceNum'
                                    },
                                    {
                                        Label: 'Serving Location',
                                        Column: 'FOWSTName'
                                    },
                                    {
                                        Label: 'Order Entry Location',
                                        Column: function(vDR) {
                                            if (CStr2(vDR.EncodingFOWSTName).trim() == '')
                                                return vDR.FOWSTName;
                                            else
                                                return vDR.EncodingFOWSTName;
                                        }
                                    },
                                    {
                                        Label: 'Customer',
                                        Column: 'CompanyName'
                                    },
                                    {
                                        Label: 'Ordered Amount',
                                        Column: function(vDR) {
                                            return AccountingFormat(vDR.OrderedAmount, 4);
                                        }
                                    },
                                    {
                                        Label: 'Served Amount',
                                        Column: function(vDR) {
                                            return AccountingFormat(vDR.ServedAmount, 4);
                                        }
                                    },
                                    {
                                        Label: 'SKU Count',
                                        Column: 'SkuCount'
                                    },
                                    {
                                        Label: 'Date Created',
                                        Column: 'DateCreated'
                                    },
                                    {
                                        Label: 'Last Updated',
                                        Column: 'LastUpdated'
                                    },
                                    {
                                        Label: 'Updated By',
                                        Column: 'EmployeeFullName'
                                    },
                                    {
                                        Label: 'Action',
                                        Column: function () {
                                            return '<div name="divAction" />';
                                        }
                                    }
                                ],
                                Click: function (e) {
                                    console.log(e + 'jsRQPZNQWWES!');
                                    $('#butInvoiceToAdjust')
                                        .text(e.DocumentName + '-' + e.MIN + '-' + e.InvoiceNumber);
                                        //.button('refresh');
                                    mInvoiceID = e.ID;
                                    $('#spanAdjustmentCustomer').text(e.CompanyName);
                                    $('#popupNewAdjustment').modal('show');
                                },
                                rowCallback: function(row, data, dataIndex) {
                                    $(row).find('input').click(function(e) {
                                        e.stopPropagation();
                                    });

                                    var pCell = $(row).find('[name=divAction]');
                                    if (pCell.length == 0)
                                        return;
                                    var pLink = $('<a>')
                                        .text('Preview')
                                        .attr('href', '#')
                                        .click(function(e) {
                                            e.preventDefault();
                                            var win = window.open(location.origin + '/erp' + location.search + '&IsReadOnly=1&IsPreview=1&ID=' + data.ID,
                                                '_blank',
                                                'width=' + screen.availWidth + ',height=' + screen.availHeight);

                                            win.onload = function () {
                                                function LoadRecord() {
                                                    if (vParams.TransType == 'SalesOrder'){
                                                        win.location.href = '/Erp/OrderProcessing' + win.location.search;
                                                    } else if (vParams.TransType == 'Invoice'){
                                                        win.location.href = '/Erp/Invoicing' + win.location.search;
                                                    }
                                                }
                                                LoadRecord();    
                                            }
                        
                                            return false; // para DILI magclose ang popup
                                        });
                                    pCell.append(pLink);
                                },
                            }
                        ], { 
                            TransType: 'Invoice',
                            IsMcr: true,
                            CustomerID: mCustomerID,
                            NonZero: 1,
                            AllowNoItems: false,
                            LimitSearchFields: true
                        }
                    );
                }); // END $('#butInvoiceToAdjust').on('click', function() {

                //$('#btnCloseAdjustment').click(function () {
                //    $('#popupAdjustment').modal('show');
                //})

                $('#btnAddAdjustment').on('click', function() {
                    var pID;
                    var pData;

                    if ($('#cbReasonForAdjustment Option:selected').text().indexOf('Returned Checks') == -1)
                        if (!IsGuid(mInvoiceID)) {
                            WebToast('Please select an invoice to adjust', 'top');
                            PromptUserAccess('Adjustment (EAS)', {
                                OnClickOKSuccess: function () {
                                    $('#popupAdjustment').modal('show');
                                },
                                OnFailed: function () {
                                    console.log('jsCFDTPD3GXC!');
                                }
                            });
                            return;
                        }

                    if (!IsGuid($('#cbReasonForAdjustment Option:selected').val())) {
                        WebToast('Please select a reason', 'top');
                        return;
                    }

                    if (mContextMenuAdjustment != null)
                        pID = mContextMenuAdjustment.ID;

                    pData = {
                        ID: pID,
                        InvoiceID: mInvoiceID,
                        ReasonID: $('#cbReasonForAdjustment Option:selected').val(),
                        Amount: $('#txtAdjustmentAmount').val(),
                        Remarks: $('#txtAdjustmentRemarks').val()
                    };

                    if ($('#cbReasonForAdjustment Option:selected').text().indexOf('Returned Checks') != -1) {
                        pData.ReturnedChecks = [];
                        var pTRs = mTableReturnedCheck.find('tbody tr');
                        for (var p = 0; p <= pTRs.length - 1; p++) {
                            var pTR = $(pTRs[p]);
                            var pDR = pTR.data('data');
                            pData.ReturnedChecks.push(pDR);
                        }
                        if (pData.ReturnedChecks.length == 0) {
                            WebToast('Please add at least 1 returned check', 'top');
                            return;
                        }
                    }
                    SaveAdjustment(pData);
                }); // END $('#btnAddAdjustment').on('click', function() {

                function SaveAdjustment(vParams) {
                    vParams = $.extend({
                        ID: null,
                        InvoiceID: null,
                        ReasonID: null,
                        Amount: null,
                        Remarks: null
                    }, vParams);
                    Bs.Loading('show');
                    AjaxSend({
                        url: '/CreditAndCollection/SaveAdjustment',
                        data: vParams,
                        success: function (vData) {
                            Bs.Loading('hide');
                            if (proj.LoadPaymentInvoices != null)
                                proj.LoadPaymentInvoices({
                                    CustomerID: mCustomerID
                                });

                            if (vData.Error) 
                                NewMessageBox(vData.Error, null, [
                                    {
                                        Text: 'Ok',
                                        Click: function() {
                                            $('#popupAdjustment').modal('show');
                                        }
                                    }
                                ]); // END NewMessageBox
                            else if (vData.Result == 'ok')
                                WebToast('Adjustment was successfully saved.', 'top');
                                $('#popupNewAdjustment').modal('hide');
                                $('#popupAdjustment').modal('show');
                            LoadAdjustments();
                            ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, $('#cbInvoiceFilter').val());
                            GetCustomersWithCredit();
                        }
                    });
                } // END function SaveAdjustment(vParams) {


                function GenerateBilling() {

                    AjaxSend({
                        url: '/CreditAndCollection/CreateBilling',
                        data: {
                            CustomerID: mCustomerID || proj.CustomerPaymentID
                        },
                        success: function(vData) {
                            ViewReport(vData.Filename);
                            if (vData.Filename) {
                                NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                            }
                        }
                    }); // END AjaxSend({

                    function ViewReport(vFilename) {
                        var pUrl = MergeUrlParameters({
                            Filename: vFilename
                        }, '/CreditAndCollection/GetBilling');
                        window.open(pUrl, '_blank', 
                            'width=' + window.innerWidth + 
                            ',height=' + window.innerHeight + 
                            ',scrollbars=auto');
                    } // END function ViewReport() {
                } // END function GenerateBilling() {

                mTableReturnedCheck = $('#divAccountAdjustmentChecks [name=table]');
                mTrReturnCheck = mTableReturnedCheck.find('tbody tr');
                mTrReturnCheck.remove();

                mDeferred.resolve();

            }); // END $(document).ready(function () {

        });
    });
    return mDeferred.promise();
});