define(['common', '/Scripts/jquery.dataTables.min.js', '/Scripts/jquery.scrollTo.js'],
function () {
    var mDeferred = $.Deferred();
    require([
        '/Content/fancybox/jquery.fancybox.pack.js', 
        '/Scripts/sources/CreditAndCollection/CacPaymentPopup.js',
        '/Scripts/sources/CreditAndCollection/settings/controller.js'
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
                        mMode = vParams.Mode;
                        proj.CacMode = mMode;
                        Init();
                        $('#divCaCMainPage').pageshow();
                    },
                    InvoiceID: null
                }

                function Init() {
                    if (mMode == 'AP') {
                        $('#divTabCac').find('[custom-tab-content=#tabCacDashboard]').text('Accounts Payable Dashboard');
                        $('#divTabCac').find('[custom-tab-content=#tabCacWork]').text('Accounts Payable');
                        $('#divTabCac').find('[custom-tab-content=#tabCacSettings]').text('Accounts Payable Settings');

                        $('#tbodyCustomerWithCredit').find('[name=thClientName]').text('Vendor Name');
                        $('#tbodyCustomerWithCredit').find('[name=thAccountsReceivableBalance]').text('Accounts Payable Balance');

                        $('#popupAccountDetails').find('[name=divCustomerName]').text('Vendor Name');
                        $('#butGenerateBilling').hide(); // no need for closest('.ui-btn')
                        $('#tbodyCustomerWithCredit').find('[name=thAssignedSalesTeam]').hide();
                    } else {
                        $('#divTabCac').find('[custom-tab-content=#tabCacDashboard]').text('Credit & Collection Dashboard');
                        $('#divTabCac').find('[custom-tab-content=#tabCacWork]').text('Credit & Collection');
                        $('#divTabCac').find('[custom-tab-content=#tabCacSettings]').text('Credit & Collection Settings');

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

                    NewMessageBox('Please wait while uploading file(s) ...', 'Message');

                    AjaxSend({
                        url: url,
                        type: 'POST',
                        data: formdata,
                        async: true,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            NewMessageBoxClose();
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
                            mContextMenuAdjustment = null;
                            LoadAdjustment({
                                New: true
                            });
                            $('#popupNewAdjustment').popup('open');
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
                
                .on('click', '#butGenerateBilling', function() {
                    GenerateBilling();
                })
                .on('change', '#cbInvoiceFilter', function() {
                    ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, $('#cbInvoiceFilter').val());
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
                    require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js'], function() {
                        ERP.AccountDetails.controller.Mode = mMode;
                        ERP.AccountDetails.controller.CustomerID = mCustomerID;
                        ERP.AccountDetails.controller.CustomerName = mCustomerName;
                        $('#popupAccountDetailsContent').insertAfter($('#popupAccountDetails .popup-header'));
                        $('#popupAccountDetails').show();
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
                    GetPaymentPopups();

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

                function GetCustomersWithCredit() {
                
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

                    AjaxSend({
                        url: pUrl,
                        data: pData,
                        success: function (vData) {
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
                                    { "": "" },
                                    { "": "" },
                                    { "": "" },
                                    { "": "" },
                                    { "data": "NetInvoiceBalance" },
                                    { "": "" }
                                ],
                                "columnDefs": [                                    
                                    {
                                        'targets': [1],
                                        'searchable': false,
                                        'orderable': false,
                                        'className': 'dt-body-center',
                                        'render': function (data, type, full, meta) {
                                            return '';
                                        }
                                    },
                                    {
                                        'targets': [2],
                                        'searchable': false,
                                        'orderable': false,
                                        'className': 'dt-body-center',
                                        'render': function (data, type, full, meta) {
                                            return '';
                                        }
                                    },
                                    {
                                        'targets': [3],
                                        'searchable': false,
                                        'orderable': false,
                                        'className': 'dt-body-center',
                                        'render': function (data, type, full, meta) {
                                            return '';
                                        }
                                    },
                                    {
                                        'targets': [4],
                                        'searchable': false,
                                        'orderable': false,
                                        'className': 'dt-body-center',
                                        'render': function (data, type, full, meta) {
                                            return '';
                                        }
                                    },
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
                                        'searchable': false,
                                        'orderable': false,
                                        'className': 'dt-body-center',
                                        'render': function (data, type, full, meta) {
                                            return '';
                                        }
                                    }
                                ],
                                "language": {
                                    "search": pSearchLabel + " "
                                },
                                sDom: '<"top"Bf>rt<"bottom"ip><"clear">',
                                buttons: buttons,
                            });

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
                        }
                    })
                } // END function GetCustomersWithCredit() {
                window.Cac.GetCustomersWithCredit = GetCustomersWithCredit;

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
                    PopupButtonList('', 'Options', [
                        {
                            Text: 'Edit Memo',
                            Click: function () {
                                LoadAdjustment({
                                    New: false   
                                });
                                $('#popupNewAdjustment').popup('open');
                            }
                        },
                        //{
                        //    Text: 'Print Memo',
                        //    Click: function () {

                        //    }
                        //},
                        {
                            Text: 'Delete Memo',
                            Click: function() {
                                PromptDeleteMemo();
                            }
                        }
                    ]);
                }

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
                                        .attr('src', location.origin + '/InvoicesFiles/' + dataimages[i].InvoiceID + '/' + dataimages[i].FileName)
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

                    $('#popupInvoiceViewAttachments').show();
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
                        .button('refresh'); // 2017_04_07__16_44_19_4229 button has been converted to span
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
                                $('#txtAdjustmentAmount').val(AccountingFormat(pDR.Amount));
                                $('#txtAdjustmentRemarks').val(pDR.Remarks);
                                var pCB = $('#cbReasonForAdjustment');
                                pCB.find('option[value=' + pDR.ReasonID + ']')
                                    .prop('selected', true);
                                //pCB.selectmenu('refresh'); // error
                                if ($('#cbReasonForAdjustment Option:selected').text().indexOf('Returned Checks') != -1) 
                                    $('#divAccountAdjustmentChecks').show();
                                else
                                    $('#divAccountAdjustmentChecks').hide();

                                var pDT = vData.ReturnedChecks;
                                for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                                    pDR = pDT.Rows[p];
                                    AddReturnedCheckTableRow(pDR);
                                }
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
                    if (pText.indexOf('Returned Checks') != -1) 
                        $('#divAccountAdjustmentChecks').show();
                    else
                        $('#divAccountAdjustmentChecks').hide();

                    if (pText.indexOf('Sales Return') != -1) {
                        require(['promise!/Scripts/sources/ERP/SalesReturn/work/controller.js'], function() {
                            if ($('#popupSalesReturn').length == 0)
                                console.log('#popupSalesReturn does not exist. ref: jsDCYMVWAJK8!');
                            $('#popupSalesReturn').show();
                            $('#popupNewAdjustment').popup('close');
                            if (mInvoiceID)
                                ERP.SalesReturn.work.controller.LoadTrans({ID: mInvoiceID});
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
                                    }
                                ],
                                Click: function (e) {
                                    PromptUserAccess('Adjustment (EAS)', {
                                        OnClickOKSuccess: function () {                                                                                        
                                            AddReturnedCheckTableRow(e);
                                            $('#popupNewAdjustment').popup('open');
                                        },
                                        OnFailed: function () {
                                            console.log('jsCFDTPD3GXC!');
                                        }
                                    });
                                    
                                }
                            }
                        ], 
                        { 
                            InvoiceID: mInvoiceID
                        }
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',
                });

                function AddReturnedCheckTableRow(vDR) {
                    var pTR = mTrReturnCheck.clone();
                    pTR.data('data', vDR);
                    pTR.find('[name=bank]').text(vDR.BankName);
                    pTR.find('[name=checkNumber]').text(vDR.CheckNumber);
                    pTR.find('[name=checkAmount]').text(AccountingFormat(vDR.CheckAmount));
                    pTR.find('[name=checkDate]').text($.format.date(new Date(vDR.CheckDate), 'MMM dd, yyyy'));
                    mTableReturnedCheck.append(pTR);

                    $('#txtAdjustmentAmount').val(CDec2($('#txtAdjustmentAmount').val()) + CDec2(vDR.CheckAmount));
                }

                // 2017_04_07__16_44_19_4229 button has been converted to span
                $('#butInvoiceToAdjust').on('click', function() {
                    BrowsePopup('TransInvoiceFromCache', 'Invoice',
                        [
                            {
                                //AutoSearch: true,
                                DisplayText: [{ Column: 'InvoiceNumber' }],
                                Click: function (e) {
                                    console.log(e + 'jsRQPZNQWWES!');
                                    $('#butInvoiceToAdjust')
                                        .text(e.InvoiceNumber)
                                        .button('refresh');
                                    mInvoiceID = e.ID;
                                    $('#spanAdjustmentCustomer').text(e.CompanyName);
                                    $('#popupNewAdjustment').popup('open');
                                }
                            }
                        ], { 
                            IsMcr: true,
                            CustomerID: mCustomerID
                        }
                    );
                }); // END $('#butInvoiceToAdjust').on('click', function() {

                $('#btnAddAdjustment').on('click', function() {
                    var pID;
                    var pData;

                    if (!IsGuid(mInvoiceID)) {
                        NewMessageBox('Please select an invoice to adjust.', null, [
                            {
                                Text: 'Ok',
                                Click: function() {
                                    PromptUserAccess('Adjustment (EAS)', {
                                        OnClickOKSuccess: function () {                                                                                        
                                            $('#popupAdjustment').show();
                                        },
                                        OnFailed: function () {
                                            console.log('jsCFDTPD3GXC!');
                                        }
                                    });
                                }
                            }
                        ]);
                        return;
                    }

                    if (!IsGuid($('#cbReasonForAdjustment Option:selected').val())) {
                        NewMessageBox('Please select a reason.', null, [
                            {
                                Text: 'Ok',
                                Click: function() {
                                    $('#popupNewAdjustment').popup('open');
                                }
                            }
                        ]);
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
                            NewMessageBox('Please add at least 1 returned check.', null, [
                                {
                                    Text: 'Ok',
                                    Click: function() {
                                        $('#popupNewAdjustment').popup('open');
                                    }
                                }
                            ]);
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

                    AjaxSend({
                        url: '/CreditAndCollection/SaveAdjustment',
                        data: vParams,
                        success: function(vData) {
                            if (vData.Error) 
                                NewMessageBox(vData.Error, null, [
                                    {
                                        Text: 'Ok',
                                        Click: function() {
                                            $('#popupNewAdjustment').popup('open');
                                        }
                                    }
                                ]); // END NewMessageBox
                            else if (vData.Result == 'ok')
                                NewMessageBox('Adjustment was successfully saved.');
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
                            CustomerID: mCustomerID
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