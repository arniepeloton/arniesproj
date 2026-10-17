define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Payment/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Payment.controller = ERP.Payment.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Payment.controller', null, function () {

        var mOut = {};
        var mElement;
        var pDataTable;
        var mUnappliedCreditToUse;

        mElement = $(mHtml);
        if (location.pathname.toLowerCase() != '/salesmenu/webpos' && location.pathname.toLowerCase() != '/salesmenu/webposnew') {
            $.mobile.activePage.append(mElement);

            if (proj.IsBootstrapDesign()) {
                $('#popupCustomerPaymentcontent > *').appendTo('#popupCustomerPaymentcontent-bs');
                $('#popupCustomerPayment').removeAttr('id');
                $('#popupCustomerPayment-bs').attr('id', 'popupCustomerPayment');
            }            
        }
        
        mOut.element = mElement;
        mOut.Mode = null;
        mOut.CustomerName = null;
        mOut.CustomerID = null;
        mOut.FromEcommerce = false;
        mOut.FromWebPos = false;
        mOut.BeforeSave = null; // function with Success callback
        mOut.WebPosInvoiceID = null;
        mOut.WebPosNetInvoiceBalance = null;
        mOut.NewTrans = function(){};
        mOut.EmailReceipt = false;
        mOut.ReadOnly = false;

        var curCash = 0, totalPaymentRcv = 0, totalCashRcv = 0, totalCardRcv = 0, totalChequeRcv = 0, totalSrvRcv = 0, totalOPRcv = 0, totalOnCredit = 0, totalDiscount = 0, totalCharges = 0, rewardscardno = '', OnCreditRemarks = '';
        var mCurrentPaymentsAppliedToInvoices = 0;
        var mConsumedUnappliedPayment = 0;
        var mLoadPaymentInvoicesParams;
        var mDontNoPrintHash = false;
        var mNetInvoiceBalanceWithCurrentPayment = 0;

        mOut.LoadPaymentInvoices = function(vParams) {
                    
            var pData;
            var pUrl;

            mLoadPaymentInvoicesParams = vParams;
            //NoPrintHash();
            if (vParams == null)
                vParams = mLoadPaymentInvoicesParams;
            else
                mLoadPaymentInvoicesParams = vParams;
            vParams = $.extend({
                InvoiceID: null,
                CustomerID: mOut.CustomerID,
                Filter: $('[name=cbPaymentInvoiceFilter]').val()
            }, vParams);
            mOut.CustomerID = vParams.CustomerID;
            if (mOut.Mode == 'AP') {
                mElement.find('[name=cbPaymentInvoiceFilter] option[value=cancelled]').show();
                pData = {
                    ReceivingID: vParams.InvoiceID,
                    SupplierID: vParams.CustomerID,
                    Filter: vParams.Filter
                };
                pUrl = '/CreditAndCollection/GetReceivingsWithPayable';
            } else {
                mElement.find('[name=cbPaymentInvoiceFilter] option[value=cancelled]').hide();
                pData = {
                    InvoiceID: vParams.InvoiceID,
                    CustomerID: vParams.CustomerID,
                    Filter: vParams.Filter
                };
                pUrl = '/CreditAndCollection/GetCustomerInvoicesWithCredit';
            }
            if (CStr2(vParams.InvoiceID) == '')
                $('[name=cbPaymentInvoiceFilter]').show();
            else
                $('[name=cbPaymentInvoiceFilter]').hide();
            pData.ArtificialDelay = sessionStorage.ArtificialDelay; // for debugging
            Bs.Loading('show');
            AjaxSend({
                url: pUrl,
                data: pData,
                success: function(vData) {
                    sessionStorage.CustomerOrganizationID = vParams.CustomerID;
                    DisplayPaymentInvoices(vData);
                }
                ,complete: function() {
                    Bs.Loading('hide');
                }
            });

        }; // END function LoadPaymentInvoices
        proj.LoadPaymentInvoices = mOut.LoadPaymentInvoices;

        var mInvoicesAmountPaid = [];
        function SaveInvoicesAmountPaid() {
            // save to memory
            mInvoicesAmountPaid = [];
            var pRows = $('#tableInvoices').find('tbody tr');
            for (var p = 0; p <= pRows.length - 1; p++) {
                var pTR = $(pRows[p]);
                var pDR = pTR.data('data');
                var pAmount = CDec2(pTR.find('[name=AmountPaid]').val());

                if (pAmount > 0)
                    mInvoicesAmountPaid.push({
                        ID: pDR.ID,
                        AmountPaid: pAmount
                    });
            }
        }

        function GetInvoiceAmountPaid(vInvoiceID) {
            // get from memory
            for (var p = 0; p <= mInvoicesAmountPaid.length - 1; p++) {
                var pDR = mInvoicesAmountPaid[p];
                if (pDR.ID == vInvoiceID)
                    return pDR.AmountPaid;
            }
        }

        var mTableInvoicesTr;
        mOut.DisplayPaymentInvoices = DisplayPaymentInvoices;
        function DisplayPaymentInvoices(vData) {
            if (vData.CustomerID != null) {
                mOut.CustomerID = vData.CustomerID;
                sessionStorage.CustomerOrganizationID = vData.CustomerID;
            }
            SaveInvoicesAmountPaid();
            var pTotalGrossInvoiceAmount = 0;
            var pTotalNetAdjustments = 0;
            var pTotalNetInvoiceAmount = 0;
            var pAccumPrevPaymentsPendingApproval = 0;
            var pTotalAccumPrevPayments = 0;
            var pTotalNetInvoiceBalance = 0;

            var pTBody = $('#tableInvoices').find('tbody');
            if (mTableInvoicesTr == null) 
                mTableInvoicesTr = pTBody.find('tr:first').clone();
            pTBody.html('');
            mOut.WebPosNetInvoiceBalance = 0;

            if (CBool(sessionStorage.IsSOALink)) {
                $('#thAPPPendingApproval, #lblCustomerPaymentFootAccumPrevPaymentsPendingApproval').show();
                mTableInvoicesTr.find('[name="AccumPrevPaymentsPendingApproval"]').show();
            }

            for (var p = 0; p <= vData.Results.Rows.length - 1; p++) {
                var pDR = vData.Results.Rows[p];

                pTotalGrossInvoiceAmount += CDec2(pDR.GrossInvoiceAmount);
                pTotalNetAdjustments += CDec2(pDR.AdjustmentAmount);
                pTotalNetInvoiceAmount += CDec2(pDR.NetInvoiceAmount);
                pTotalAccumPrevPayments += CDec2(pDR.TotalPayment);
                pAccumPrevPaymentsPendingApproval += CDec2(pDR.APPPendingApproval);
                pTotalNetInvoiceBalance += CDec2(pDR.NetInvoiceBalance);
                mOut.WebPosNetInvoiceBalance = CDec2(pDR.NetInvoiceBalance);
                var pTR = mTableInvoicesTr.clone();
                pTR.find('[name=InvoiceImage]').empty().append(((CDec2(pDR.NoOfAttachments) + CDec2(pDR.NoOfReceivingInvRefImages)) > 0 ? $('<button data-role="none" class="btn btn-warning btn-sm" title="View Invoice Image">').append('<span class="bi-paperclip"></span>') : ''))
                    .data('data', pDR)
                    .click(function (e) {
                        e.stopPropagation();
                        var data = $(this).data('data');
                        if ((CDec2(data.NoOfAttachments) + CDec2(data.NoOfReceivingInvRefImages)) > 0) {
                            ERP.AccountDetails.controller.GetInvoiceFiles(data.ID);
                        }
                    }).append('<br>')

                if (CDec2(pDR.NoOfNotes) > 0) {
                    pTR.find('[name=InvoiceImage]').append($('<button data-role="none" class="btn btn-info btn-sm" title="View Notes">').append('<span class="bi-journal-text"></span>')
                            .data('data', pDR)
                            .click(function (e) {
                                e.stopPropagation();
                                var data = $(this).data('data');
                                ERP.AccountDetails.controller.GetNotes(data);
                            })
                        )
                }

                pTR.find('[name=InvoiceDate]').text(CStr2(FormatDate(pDR.InvoiceDate, {Format: 'MM/dd/yyyy hh:mm a'})));
                pTR.find('[name=InvoiceDueDate]').text(CStr2(FormatDate(pDR.InvoiceDueDate)));

                var pParams = {
                    IsReadOnly: 1,
                    OrgID: sessionStorage.OrgID
                };
                if (proj.CacMode == 'AP') {
                    pParams.ShowAccountsPayable = 1;
                    pParams.ID = pDR.AccountsPayableID;
                } else {
                    pParams.ShowInvoice = 1;
                    pParams.ID = pDR.ID;
                }

                if (CBool(sessionStorage.IsSOALink)) {
                    pParams.IsSoaLink = 1
                }

                var pUrl = MergeUrlParameters(pParams, location.origin + (proj.IsBootstrapDesign() ? '/erp' : location.pathname));
                var pLnk = $('<a href=#>');
                var pLinkParams = {
                    url: pUrl,
                    AccountsPayableID: pDR.AccountsPayableID,
                    DR: pDR
                };

                if (proj.CacMode == 'AP') {
                    var pLnkText;
                    if (pDR.ApStatus == 'Finalized')
                        pLnkText = 'AP ' + pDR.ApNum;
                    else
                        pLnkText = 'Draft AP';
                    if (CStr2(pDR.InvoiceNumber).trim() != '')
                        pLnkText += ' (' + pDR.InvoiceNumber.trim() + ')';
                    pLnk.text(pLnkText);
                } else
                    pLnk.text(pDR.InvoiceNumber);

                
                (function(vParams) {
                    pLnk.click(function(e) {
                        e.preventDefault();
                        PopupButtonList('', 'Action Menu', [
                            {
                                Text: 'View' + (proj.CacMode == 'AP' ? ' Payable\'s ' : ' ') +'Document Details',
                                Click: function (e) {
                                    if ((proj.CacMode == 'AP')
                                        && (CStr2(vParams.AccountsPayableID) == '')) {
                                        NewMessageBox('The accounts payable document of this stock receiving is not available because this stock receiving is not yet finalized. Please go to the stock receiving module and finalize this stock receiving transaction.');
                                        return false;
                                    }
                                    var win = window.open(vParams.url,
                                        '_blank',
                                        'width=' + screen.availWidth + ',height=' + screen.availHeight);
                                }
                            },
                            {
                                Text: 'View Account Adjustments',
                                data: vParams.DR,
                                Click: function (e) {
                                    var pID;
                                    var pInvoiceNumber;
                                    if (proj.CacMode == 'AP') {
                                        pID = e.AccountsPayableID;
                                        if (CStr2(pID) == '') {
                                            NewMessageBox('The account adjustment documents of this stock receiving is not available because this stock receiving is not yet finalized. Please go to the stock receiving module and finalize this stock receiving transaction.');
                                            return;
                                        }
                                        if (CStr2(e.FullReceivingNum) != '')
                                            pInvoiceNumber = 'SR - ' + e.FullReceivingNum;
                                        else
                                            pInvoiceNumber = CStr2(e.InvoiceNumber);
                                    } else {
                                        pID = e.ID;
                                        pInvoiceNumber = CStr2(e.InvoiceNumber);
                                    }
                                    var pAccess = 'Adjustment (EAS)';
                                    if (CDec2(e.TotalPayment) > 0) 
                                        pAccess = 'Adjustment to Invoices With Payments (EAS)';
                                    PromptUserAccess(pAccess, {
                                        OnClickOKSuccess: function () {                                                                                        
                                            Cac.LoadAdjustments({
                                                ID: e.ID,
                                                InvoiceID: pID,
                                                InvoiceNumber: pInvoiceNumber,
                                                InvoiceDate: e.InvoiceDate,
                                                GrossInvoiceAmount: e.GrossInvoiceAmount,
                                                NetInvoiceAmount: e.NetInvoiceAmount,
                                                NetInvoiceBalance: e.NetInvoiceBalance,
                                                SupplierID: mOut.CustomerID,
                                                AccountsPayableID: e.AccountsPayableID
                                            });
                                            //mElement.hide(); // commented on 2017_11_09__09_06_50_5349
                                            $('#popupAdjustment').modal('show');
                                        },
                                        OnFailed: function () {
                                            console.log('jsCFDTPD3GXC!');
                                        }
                                    });
                                }
                            }
                        ]);
                        
                        return false;
                    });
                })(pLinkParams);                
                pTR.find('[name=InvoiceNum]').empty().append(pLnk);
                pTR.find('[name=GrossInvoiceAmount]').text(AccountingFormat(pDR.GrossInvoiceAmount));
                pTR.find('[name=NetAdjustments]').text(AccountingFormat(pDR.AdjustmentAmount));
                pTR.find('[name=NetInvoiceAmount]').text(AccountingFormat(pDR.NetInvoiceAmount));

                //if (CBool(sessionStorage.IsSOALink)) {
                    pTR.find('[name=AccumPrevPaymentsPendingApproval]').text(AccountingFormat(pDR.APPPendingApproval));
                //}

                pTR.find('[name=AccumPrevPayments]').text(AccountingFormat(pDR.TotalPayment));
                pTR.find('[name=NetInvoiceBalance]').text(AccountingFormat(pDR.NetInvoiceBalance));
                var pAmountPaid = GetInvoiceAmountPaid(pDR.ID);
                pAmountPaid = CDec2(pAmountPaid);
                if (pAmountPaid > 0)
                    pAmountPaid = AccountingFormat(pAmountPaid);
                else
                    pAmountPaid = '';
                pAmountPaid = ''; // dont reload preview amount paid amount 2019_07_11__14_27_23_6886
                pTR.find('[name=AmountPaid]')
                    .val('')
                    .bind('textchange', AmountPaid_TextChange).val(pAmountPaid);
                pTR.find('[name=butPasteNetInvoiceBalance]').click(function() {
                    var pTR = $(this).closest('[name=InvoiceRow]');
                    var pDR = pTR.data('data');
                    pTR.find('[name=AmountPaid]').val(pTR.find('[name=NetInvoiceBalance]').text());
                    AmountPaid_TextChange();
                });
                pTR.data('data', pDR);
                pTR.click(function (e) {
                    var mData = $(this).data('data');

                    if ($(e.target).attr('name') == 'AmountPaid')
                        return;
                    PopupButtonList('', 'Action Menu', [
                        {
                            Text: 'View Notes',
                            data: mData,
                            Click: function (e) {
                                ERP.AccountDetails.controller.GetNotes(e);
                            }
                        }
                    ]);
                })
                pTBody.append(pTR);
            }

            $('#lblCustomerPaymentFootRecordCount').text(AccountingFormat(vData.Results.Rows.length, 0) + ' records');
            $('#lblCustomerPaymentFootGrossInvoiceAmount').text(AccountingFormat(pTotalGrossInvoiceAmount));
            $('#lblCustomerPaymentFootNetAdjustments').text(AccountingFormat(pTotalNetAdjustments));
            $('#lblCustomerPaymentFootNetInvoiceAmount').text(AccountingFormat(pTotalNetInvoiceAmount));
            $('#lblCustomerPaymentFootAccumPrevPayments').text(AccountingFormat(pTotalAccumPrevPayments));
            $('#lblCustomerPaymentFootAccumPrevPaymentsPendingApproval').text(AccountingFormat(pAccumPrevPaymentsPendingApproval));
            $('#lblCustomerPaymentFootNetInvoiceBalance').text(AccountingFormat(pTotalNetInvoiceBalance));
            $('#divUnappliedPayments').text(AccountingFormat(vData.Overpayment));
            if (mOut.WebPosInvoiceID != null) {
                $('#webPosCartTotal').text(AccountingFormat(mOut.WebPosNetInvoiceBalance));
                mCurrentPaymentsAppliedToInvoices = mOut.WebPosNetInvoiceBalance;
                $('#lblCurrentPaymentsAppliedToInvoices, #lblCustomerPaymentFootPayments').text(AccountingFormat(mCurrentPaymentsAppliedToInvoices));
            }
            AmountPaid_TextChange();
        }

        mOut.ClearValues = function() {

            mOut.WebPosInvoiceID = null;
            mInvoicesAmountPaid = [];
            curCash = 0;
            totalPaymentRcv = 0;
            totalCashRcv = 0;
            totalCardRcv = 0;
            totalChequeRcv = 0;
            totalSrvRcv = 0;
            CacPopup.Srv.totalsrv = 0;
            totalOPRcv = 0;
            totalOnCredit = 0;
            totalDiscount = 0;
            totalCharges = 0;
            rewardscardno = '';
            OnCreditRemarks = '';

            CacPopup.ChequeGC.arrCGC = [];            
            CacPopup.Card.arrCredit = [];
            CacPopup.Card.arrDebit = [];
            CacPopup.OtherPayment.Clear();
            CacPopup.Srv.arrSrv = [];
            CacPopup.Discount.Clear();
            mConsumedUnappliedPayment = 0;
            sessionStorage.removeItem('DefaultOtherPaymentID');
            $('#divPopUnappliedCreditAmount input:text').val('');
            $('#rowVATPercent, #rowAmountLessVAT, #rowVATBackPercent').hide();

            $('#txtPaymentManualReceiptNumber').val('');
            $('#lblPaidCash' + 
                ', #lblPaidDCCard' +
                ', #lblPaidCheque' +
                ', #lblPaidSrv' +
                ', #lblPaidOther' +
                ', #lblTotalPaymentRcv' + 
                ', #lblCustomerPaymentFootPayments' +
                ', #lblCurrentPaymentsAppliedToInvoices' + 
                ', #lblCurrentPaymentsNotAppliedToInvoices').text('');

            if (mOut.FromEcommerce) {
                $('#btnPaymentDCCard, #btnPaymentSrv, #divCustomerPaymentAdjustment, #butCustomerPaymentApplyModifiedFifo, #butCustomerPaymentLookupDraft').remove();
                $('[name="butDeactivateCheckingAccount"]').parent().remove();
                $('#txtPaymentManualReceiptNumber').parent().hide();
            }
        };
        
        function AmountPaid_TextChange() {
            var pTxts = $('#tableInvoices [name=AmountPaid]');
            mCurrentPaymentsAppliedToInvoices = 0;
            mNetInvoiceBalanceWithCurrentPayment = 0;
            for (var p = 0; p <= pTxts.length - 1; p++) {
                var pTxt = $(pTxts[p]);
                mCurrentPaymentsAppliedToInvoices += CDec2(pTxt.val());
                var pTR = pTxt.closest('tr');
                var pDR = pTR.data('data');
                if (CDec2(pTxt.val()) > 0)
                    mNetInvoiceBalanceWithCurrentPayment += CDec2(pDR.NetInvoiceBalance);
            }
            $('#lblCurrentPaymentsAppliedToInvoices, #lblCustomerPaymentFootPayments').text(AccountingFormat(mCurrentPaymentsAppliedToInvoices));

            ComputeTotalPaymentRcv();
        } // END function AmountPaid_TextChange() {

        mOut.ComputeTotalPaymentRcv = ComputeTotalPaymentRcv;
        function ComputeTotalPaymentRcv() {
            RecomputeTotal();

            totalPaymentRcv = totalCashRcv + totalChequeRcv + totalCardRcv + totalSrvRcv + totalOPRcv + mConsumedUnappliedPayment;
            $('#lblTotalPaymentRcv').text(totalPaymentRcv == 0 ? '0.00' : AccountingFormat(totalPaymentRcv));

            var pCurrentPaymentsNotAppliedToInvoices = CDec2($('#lblTotalPaymentRcv').html()) - mCurrentPaymentsAppliedToInvoices;
            $('#lblCurrentPaymentsNotAppliedToInvoices').text(AccountingFormat(pCurrentPaymentsNotAppliedToInvoices));

            var amountLacking = CDec2(mOut.WebPosNetInvoiceBalance) - CDec2(totalPaymentRcv);
            $('#lblAmountLacking').text(Math.max(amountLacking, 0) <= 0 ? '0.00' : AccountingFormat(Math.max(amountLacking, 0)));

            $('#lblPaymentChange').text(Math.max(-amountLacking, 0) <= 0 ? '0.00' : AccountingFormat(Math.max(-amountLacking, 0)));
        }
        $('body')
        .on('click', '#btnPaymentCash', function () {
            if (mOut.ReadOnly) {
                NewMessageBox('You only have read-only access to this module.', 'Read-Only Access');
                return;
            }

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.FromEcommerce = mOut.FromEcommerce;
            CacPopup.Cash.init({
                success: function (e) {
                    totalCashRcv = CDec2(e);
                    $('#lblPaidCash').text(totalCashRcv == 0 ? '' : AccountingFormat(e));
                    if (CDec2(e) > 0){
                        $('#btnPaymentCash').addClass('active');
                    }                    
                    ComputeTotalPaymentRcv();
                }
            });
        })
        .on('click', '#butCustomerPaymentApplyFifo', function() {
            var pPaymentConsumed = 0;
            var pPaymentReceived = CDec2($('#lblTotalPaymentRcv').html());

            var pRows = $('#tableInvoices').find('tbody tr');
            for (var p = 0; p <= pRows.length - 1; p++) {
                var pTR = $(pRows[p]);
                var pDR = pTR.data('data');
                var pTxt = pTR.find('[name=AmountPaid]');
                var pAmount;

                if (CDec2(pDR.NetInvoiceBalance) <= pPaymentReceived - pPaymentConsumed)
                    pAmount = CDec2(pDR.NetInvoiceBalance);
                else
                    pAmount = pPaymentReceived - pPaymentConsumed;
                pPaymentConsumed += pAmount;
                pTxt.val(pAmount);
            }
            AmountPaid_TextChange();
        })
        .on('click', '#btnPaymentDCCard', function () {
            if (mOut.ReadOnly) {
                NewMessageBox('You only have read-only access to this module.', 'Read-Only Access');
                return;
            }

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.Card.init({
                success: function (e) {
                    totalCardRcv = CDec2(e);
                    $('#lblPaidDCCard').text(totalCardRcv == 0 ? '' : AccountingFormat(e));
                    if (CDec2(e) > 0) {
                        $('#btnPaymentDCCard').addClass('active');
                    }                    
                    ComputeTotalPaymentRcv();
                }
            })
        })
        .on('click', '#btnPaymentCheque', function () {
            if (mOut.ReadOnly) {
                NewMessageBox('You only have read-only access to this module.', 'Read-Only Access');
                return;
            }

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.FromEcommerce = mOut.FromEcommerce;
            CacPopup.FromWebPos = mOut.FromWebPos;
            CacPopup.ChequeGC.init({
                success: function (e) {
                    totalChequeRcv = CDec2(e);
                    $('#lblPaidCheque').text(totalChequeRcv == 0 ? '' : AccountingFormat(e));
                    if (CDec2(e) > 0) {
                        $('#btnPaymentCheque').addClass('active');
                    }                    
                    ComputeTotalPaymentRcv()
                }
            })
        })
        .on('click', '#btnPaymentSrv', function () {
            if (mOut.ReadOnly) {
                NewMessageBox('You only have read-only access to this module.', 'Read-Only Access');
                return;
            }

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.Srv.init({
                success: function (e) {
                    totalSrvRcv = CDec2(e);
                    $('#lblPaidSrv').text(totalSrvRcv == 0 ? '' : AccountingFormat(e));
                    if (CDec2(e) > 0) {
                        $('#btnPaymentSrv').addClass('active');
                    }                    
                    ComputeTotalPaymentRcv()
                }
            })
        })
        .on('click', '#btnPaymentOther', function () {
            if (mOut.ReadOnly) {
                NewMessageBox('You only have read-only access to this module.', 'Read-Only Access');
                return;
            }

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.OtherPayment.init({
                CurrentPaymentsAppliedToInvoices: mNetInvoiceBalanceWithCurrentPayment,
                AmountAppliedToInvoices: mCurrentPaymentsAppliedToInvoices,
                success: function (e) {
                    totalOPRcv = CDec2(e);
                    $('#lblPaidOther').text(totalOPRcv == 0 ? '' : AccountingFormat(e));
                    if (CDec2(e) > 0) {
                        $('#btnPaymentOther').addClass('active');
                    }                    
                    ComputeTotalPaymentRcv()
                }
            })
        })
        .on('click', '#btnPaymentDiscounts', function () {
            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.Discount.init({
                success: function (e) {
                    totalDiscount = CDec2(e.Amount);
                    $('#lblPaidDiscounts').text(totalDiscount == 0 ? '' : AccountingFormat(e.Amount));
                    if (CDec2(e.Amount) > 0) {
                        $('#btnPaymentDiscounts').addClass('active');
                    } else {
                        $('#btnPaymentDiscounts').removeClass('active');
                    }
                    ComputeTotalPaymentRcv()
                }
            })
        })
        .on('click', '#btnPaymentCharges', function () {
            NewMessageBox('This feature is under construction.');
            return;

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.Charges.init({
                success: function (e) {
                    totalCharges = CDec2(e);
                    $('#lblPaidCharges').text(totalCharges == 0 ? '' : AccountingFormat(e));
                    if (CDec2(e) > 0) {
                        $('#btnPaymentCharges').addClass('active');
                    }
                    ComputeTotalPaymentRcv()
                }
            })
        })
        .on('click', '#btnPaymentRewards', function () {
            NewMessageBox('This feature is under construction.');
            return;

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.RewardsProgram.init({
                success: function (e) {
                    rewardscardno = e;
                    $('#lblRewardsCardNo').text(e == 0 ? '' : e);
                    if (e != '') {
                        $('#btnPaymentRewards').addClass('active');
                    }
                }
            })
        })
        .on('click', '#btnPaymentOnCredit', function () {
            NewMessageBox('This feature is under construction.');
            return;

            if (!HasTheCustomerPlacedAnOrder()) return;

            CacPopup.OnCredit.init({
                success: function (e) {
                    totalOnCredit = e.CreditAmount;
                    OnCreditRemarks = e.CreditRemarks;
                    $('#lblPaidOnCredit').text(totalOnCredit == 0 ? '' : AccountingFormat(e.CreditAmount));
                    if (CDec2(totalOnCredit) > 0) {
                        $('#btnPaymentOnCredit').addClass('active');
                    }
                    ComputeTotalPaymentRcv()
                }
            })
        })
        .on('click', '#trUnappliedPayments', function () {
            //$('#divPopUnappliedCreditAmount').trigger('create').popup().popup('open');
            //$('#divPopUnappliedCreditAmount').popup().trigger('create').show().popup('open');
            if (mOut.ReadOnly) {
                NewMessageBox('You only have read-only access to this module.', 'Read-Only Access');
                return;
            }
            require(['promise!/Scripts/sources/ERP/UnappliedCredits/controller.js?v=' + assetversion], function() {
                ERP.UnappliedCredits.controller.popup({
                    CustomerOrSupplierID: mOut.CustomerID,
                    onOk: function(vData) {
                        mConsumedUnappliedPayment = vData.total;
                        ComputeTotalPaymentRcv();
                        mUnappliedCreditToUse = vData.details;
                    }
                });
            });
        })
                
        .on('click', '#butCustomerPaymentApplyModifiedFifo', function() {
            NewMessageBox('This feature is not yet implemented. jsHR9MY6KEPJ!');
        })
        .on('click', '#butCustomerPaymentFilterInvoices', function() {
            NewMessageBox('This feature is not yet implemented. js9UUAT6CHYB!');
        })
        .on('click', '#butCustomerPaymentRemoveFilter', function() {
            NewMessageBox('This feature is not yet implemented. jsWEAGYUNS4J!');
        })
        .on('click', '#butCustomerPaymentLookupDraft', function() {
            BrowsePopup('TransCustomerPaymentDraft', 'Draft Payments',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        StyleFormat: 'table',
                        DisplayText: [
                            {
                                Label: 'Payment Date', 
                                Column: 'PaymentDate'
                            },
                            {
                                Label: 'Cash Amount', 
                                Column: function(vData) {
                                    return AccountingFormat(vData.CashAmount);
                                }
                            }
                        ],
                        Click: function (e) {
                            LoadDraft(e.ID);
                        }
                    }
                ], { 
                    CustomerID: mOut.CustomerID
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        })
        .on('click', '#butCustomerPaymentLookup', function() {
            mOut.VendorPaymentsHistory();
        })
        .on('click', '#butCustomerPaymentPrint', function() {
            var pBrowser;
            var pInput = {};
            if (mOut.Mode == 'AP') {
                pBrowser = 'TransReceivingPayment';
                pInput.SupplierID = mOut.CustomerID;
            } else {
                pBrowser = 'TransCustomerPayment';
                pInput.CustomerID = mOut.CustomerID;
            }
            BrowsePopup(pBrowser, 'Reprint Previous Payments',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        StyleFormat: 'table',
                        NoInitialSorting: true,
                        DisplayText: [
                            {
                                Label: 'Payment Date', 
                                Column: 'PaymentDate'
                            },
                            {
                                Label: 'Total Payment', 
                                Column: function(vData) {
                                    return AccountingFormat(vData.TotalPayment);
                                }
                            },
                            {
                                Label: 'Cash Amount', 
                                Column: function(vData) {
                                    return AccountingFormat(vData.CashAmount);
                                }
                            },
                            {
                                Label: 'Check Amount', 
                                Column: function(vData) {
                                    return AccountingFormat(vData.CheckPaymentAmount);
                                }
                            },
                            {
                                Label: 'Credit Card Amount', 
                                Column: function(vData) {
                                    return AccountingFormat(vData.CreditCardAmount);
                                }
                            },
                            {
                                Label: 'Debit Card Amount', 
                                Column: function(vData) {
                                    return AccountingFormat(vData.DebitCardAmount);
                                }
                            }
                        ],
                        Click: function (e) {
                            if (mOut.Mode == 'AP') 
                                localStorage.LastPrintedSupplierPaymentID = e.ID;
                            else
                                localStorage.LastPrintedCustomerPaymentID = e.ID;
                            PrintOrDownloadForm2307();
                        }
                    }
                ], pInput
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        })
        .on('click', '#butPaymentSave,[name=butPaymentSave]', function () {
            PreSavePayment();
        })
        .on('change', '[name=cbPaymentInvoiceFilter]', function () {
            mOut.LoadPaymentInvoices({
                Filter: $('[name=cbPaymentInvoiceFilter]').val()
            });
        })
        .on('click', '[name=butPaymentSaveAndPrint]', function () {
            var pInvoiceID = mOut.WebPosInvoiceID;
            if (proj.IsBootstrapDesign()) {
                Bs.Loading('show');
            }
            PreSavePayment({
                Success: function (vData) {
                    if (!CBool(WebPOS.isSaveReceipt))
                        CreatePdf({
                            InvoiceID: pInvoiceID
                        });
                    else {
                        WebPOS.NewTrans();
                        NewMessageBox('SI ' + vData.InvoiceNum + ' was successfully paid.', 'Paid');
                    }
                }
            });
        })
        .on('click', '[name=butPaymentSaveAndEmail]', function () {
            NewMessageBox('This function is not yet implemented. vbVF4QXWQ5WB!');
        }); // END $('#butPaymentSave').click(function() {

        mOut.VendorPaymentsHistory = function() {
            var pBrowser;
            var pInput = {};
            if (mOut.Mode == 'AP') {
                pBrowser = 'TransReceivingPayment';
                pInput.SupplierID = mOut.CustomerID;
            } else {
                pBrowser = 'TransCustomerPayment';
                pInput.CustomerID = mOut.CustomerID;
            }

            var pColumns = [];
            if (CStr2(pInput.SupplierID) == '')
                pColumns.push({
                    Label: 'Vendor',
                    Column: 'SupplierName'
                });
            pColumns = pColumns.concat([
                {
                    Label: 'Payment Date', 
                    Column: 'PaymentDate'
                },
                {
                    Label: 'Total Payment', 
                    Column: function(vData) {
                        return AccountingFormat(vData.TotalPayment);
                    }
                },
                {
                    Label: 'Cash Amount', 
                    Column: function(vData) {
                        return AccountingFormat(vData.CashAmount);
                    }
                },
                {
                    Label: 'Check Amount', 
                    Column: function(vData) {
                        return AccountingFormat(vData.CheckPaymentAmount);
                    }
                },
                {
                    Label: 'Credit Card Amount', 
                    Column: function(vData) {
                        return AccountingFormat(vData.CreditCardAmount);
                    }
                },
                {
                    Label: 'Debit Card Amount', 
                    Column: function(vData) {
                        return AccountingFormat(vData.DebitCardAmount);
                    }
                },
                {
                    Label: 'Print',
                    Render: function(v1, v2, vData) {
                        return '<div name="divPrint"></div>';
                    }, 
                    DontCopyToPlainText: true
                },
                {
                    Label: 'Cancel Payment',
                    Render: function(v1, v2, vData) {
                        return '<div name="divCancelPayment"></div>';
                    }, 
                    DontCopyToPlainText: true
                }
            ]);

            var pTitle;
            if (CStr2(pInput.SupplierID) == '')
                pTitle = 'Vendor Payments History';
            else
                pTitle = 'Previous Payments';
            BrowsePopup(pBrowser, pTitle,
                [
                    {
                        // Search: mTxtCanvass.val(),
                        ModalSize: 'fullwidth',
                        AutoSearch: true,
                        StyleFormat: 'table',
                        NoInitialSorting: true,
                        DisplayText: pColumns,
                        Click: function (e) {
                            if (mOut.Mode == 'AP') 
                                localStorage.LastPrintedSupplierPaymentID = e.ID;
                            else
                                localStorage.LastPrintedCustomerPaymentID = e.ID;
                            PrintOrDownloadForm2307();
                        },
                        rowCallback: function(row, data, dataIndex){
                            var pDiv = $(row).find('[name=divPrint]');
                            if (pDiv.children().length > 0)
                                return;
                            var pLink = $('<a>')
                                .text('Print')
                                .attr('href', '#')
                                .click(function(e) {
                                    e.preventDefault();
                                    if (mOut.Mode == 'AP') 
                                        localStorage.LastPrintedSupplierPaymentID = data.ID;
                                    else
                                        localStorage.LastPrintedCustomerPaymentID = data.ID;
                                    PrintOrDownloadForm2307();
                                    return false;
                                });
                            pDiv.append(pLink);

                            pDiv = $(row).find('[name=divCancelPayment]');
                            if (pDiv.children().length > 0)
                                return;
                            var pLink = $('<a>')
                                .text('Cancel Payment')
                                .attr('href', '#')
                                .click(function(e) {
                                    e.preventDefault();
                                    NewMessageBox('Are you sure you want to cancel this payment?', 'Cancel Payment', [
                                        {
                                            Text: 'Yes',
                                            Click: function() {
                                                if (mOut.Mode == 'AP') 
                                                    NewMessageBox('This function is not yet implemented. ref: js2THCHHU5ZJ!');
                                                else
                                                    CancelPayment(data.ID);
                                            }
                                        },
                                        {
                                            Text: 'No',
                                            Click: function() {
                                                
                                            }
                                        }
                                    ]);
                                    
                                    return false;
                                });
                            pDiv.append(pLink);
                        }
                    }
                ], pInput
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        };

        function HasTheCustomerPlacedAnOrder() {
            if (CBool(sessionStorage.IsWebPos) && $('#tblSideCartItems tbody').length == 0) {
                if (sessionStorage.FowstPOSView == 'Retail View' && $('[name="tableProducts"] tbody > tr').length == 0) {
                    NewMessageBox('Please add items to the cart.')
                    return false;
                } else if((sessionStorage.FowstPOSView == 'Hot Buttons View' || sessionStorage.FowstPOSView == 'Hot Buttons with side cart') && $('#tblSideCartItems tbody > tr').length == 0) {
                    NewMessageBox('Please add items to the cart.')
                    return false;
                }
                
            }

            return true
        }

        function RecomputeTotal() {
            //if (CDec2($('#lblPaidDiscounts').text()) > 0) {
            //    CacPopup.Discount.GetSelectedDiscount($.trim($('[name="selDiscountType"]').val()));
            //    CacPopup.Discount.GetDiscountOnChange();
            //}            

            //setInterval(function () {
                var discamt = AccountingFormat($('#webPosCartTotalDiscount').val());

                $('#lblPaidDiscounts').val(CDec2(discamt) > 0 ? discamt : '');

                var dblTotalSalesPay = 0;
                var dblTotalAmountDue = 0;
                var amtDue = CacPopup.Discount.isSeniorCitizenDiscount ? parseFloat($('#webPosCartSubTotal').text().replace(/,/g, "")) - parseFloat(CDec2($('#webPosCartVATPercent').text().replace(/,/g, ""))) : $('#webPosCartSubTotal').text().replace(/,/g, "");

                dblTotalAmountDue = parseFloat(CDec2(amtDue)) - parseFloat(CDec2($("#webPosCartTotalDiscount").text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayCharges').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayOtherTax').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTip').text().replace(/,/g, "")));
                mOut.WebPosNetInvoiceBalance = CDec2(dblTotalAmountDue);
                $('#webPosCartTotal').text(dblTotalAmountDue == 0 ? '0.00' : AccountingFormat(dblTotalAmountDue));
            //}, 1000);            
        }

        function CancelPayment(vCustomerPaymentID) {

            NewPromptUserAccess('Customer Payments (EAS) Cancellation', {
                OnClickOKSuccess: function () {
                    Bs.Loading('show');
                    AjaxSend({
                        url: 'CreditAndCollection/CancelCustomerPayment',
                        data: {
                            CustomerPaymentID: vCustomerPaymentID
                        },
                        success: function(vData) {
                            if (vData.Result == 'ok')
                                NewMessageBox('Payment was successfully cancelled.', 'Success');
                            else
                                NewMessageBox(vData.Error, 'Failed');
                        },
                        complete: function() {
                            Bs.Loading('hide');
                        }
                    });
                }
            });

        }

        function PrintOrDownloadForm2307() {

            var pPaymentID;
            if (mOut.Mode == 'AP')
                pPaymentID = localStorage.LastPrintedSupplierPaymentID;
            else
                pPaymentID = localStorage.LastPrintedCustomerPaymentID;

            Bs.Loading('show');
            AjaxSend({
                url: 'Erp/HasWitholdingTax',
                data: {
                    Mode: mOut.Mode,
                    PaymentID: pPaymentID
                },
                success: function(vData) {
                    if (vData.Result == 0)
                        Print();
                    else
                        PopupButtonList('', 'Output Options', [
                            {
                                Text: 'Print Payment Voucher',
                                Click: function() {
                                    Print();
                                }
                            }, {
                                Text: 'Download BIR Form 2307',
                                Click: function() {
                                    DownloadForm2307(pPaymentID);
                                }
                            }
                        ]);
                },
                complete: function() {
                    Bs.Loading('hide');
                }
            });

        }

        function DownloadForm2307(vPaymentID) {
            AjaxSend({
                url: 'Erp/CreateForm2307',
                data: {
                    Mode: mOut.Mode,
                    PaymentID: vPaymentID
                },
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.BaseFilename)
                        location = MergeUrlParameters({
                            BaseFilename: vData.BaseFilename
                        }, '/Erp/DownloadForm2307');
                }
            })
        }

        mOut.PreSavePayment = PreSavePayment;
        function PreSavePayment(vParams) {
            if (mOut.ReadOnly) {
                NewMessageBox('You only have read-only access to this module.', 'Read-Only Access');
                return;
            }
            vParams = $.extend({
                Success: function(){}
            }, vParams);

            if (mOut.BeforePay != null)
                mOut.BeforePay();
            if (mOut.BeforeSave == null)
                MainSave();
            else {
                mOut.BeforeSave({
                    Success: function() {
                        MainSave();
                    }
                });
            }

            function MainSave() {
                var pDocTypesKey; // Receivings or Invoices
                var pData = {
                    Cash: CDec2($('#lblPaidCash').html()),
                    ConsumedUnappliedPayment: mConsumedUnappliedPayment,
                    CurrentPaymentReceived: CDec2($('#lblTotalPaymentRcv').html()),
                    Cheques: [],
                    CreditCards: [],
                    DebitCards: [],
                    OtherPayments: [],
                    SrvPayments: [],
                    CustomerID: sessionStorage.CustomerOrganizationID,
                    ManualReceiptNumber: $('#txtPaymentManualReceiptNumber').val().trim(),
                    Mode: mOut.Mode,
                    FromEcommerce: mOut.FromEcommerce,
                    Success: vParams.Success
                };
                    
                if (mOut.Mode == 'AP') {    
                    pDocTypesKey = 'Receivings';
                } else {
                    pDocTypesKey = 'Invoices';
                }
                pData[pDocTypesKey] = [];

                if ((pData.ManualReceiptNumber == '')
                    && (mOut.WebPosInvoiceID == null)
                    && !CBool(mOut.FromEcommerce))

                    NewMessageBox('Are you sure you want save this payment without a manual-encoded receipt number?', 'No manual-encoded receipt number', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                Proceed();
                            }
                        },
                        {
                            Text: 'No',
                            Click: function () {
                                // do nothing
                            }
                        }
                    ], { HidePrevPopup: false });
                else  
                    Proceed();

                function Proceed() {
                    Bs.Loading('show', 'Saving transaction... please wait');
                    var pRows = $('#tableInvoices').find('tbody tr');
                    if (mOut.WebPosInvoiceID == null)
                        for (var p = 0; p <= pRows.length - 1; p++) {
                            var pTR = $(pRows[p]);
                            var pDR = pTR.data('data');
                            var pAmount = CDec2(pTR.find('[name=AmountPaid]').val());

                            if (pAmount > 0)
                                pData[pDocTypesKey].push({
                                    ID: pDR.ID,
                                    AmountPaid: pAmount
                                });
                        }
                    else
                        pData[pDocTypesKey].push({
                            ID: mOut.WebPosInvoiceID,
                            AmountPaid: mOut.WebPosNetInvoiceBalance
                        });
                    

                    for (var p = 0; p <= CacPopup.ChequeGC.arrCGC.length - 1; p++) {
                        var pDR = CacPopup.ChequeGC.arrCGC[p];

                        /*
                        CacPopup.ChequeGC.txtCGCID.val(), // 0
                        CacPopup.ChequeGC.selCGCBank.val(), // 1
                        CacPopup.ChequeGC.selCGCBank.find('option:selected').text(), // 2
                        CacPopup.ChequeGC.txtCGCBranchID.val(), // 3
                        CacPopup.ChequeGC.txtCGCBranch.val(), // 4
                        CacPopup.ChequeGC.txtCGCAccountName.val(), // 5
                        CacPopup.ChequeGC.txtCGCAccountNo.val(), // 6
                        CacPopup.ChequeGC.txtCGCChequeGCNo.val(), // 7
                        CacPopup.ChequeGC.selCGCDateMonth.val(), // 8
                        CacPopup.ChequeGC.selCGCDateDay.val(), // 9
                        CacPopup.ChequeGC.selCGCDateYear.val(), // 10
                        CacPopup.ChequeGC.txtCGCValue.val(), // 11
                        CacPopup.ChequeGC.cgcState // 12
                        */

                        if (pDR[12] == 'Remove')
                            continue;

                        var pItem = {
                            BankID: pDR[1],
                            BankName: pDR[2],
                            BankBranchID: pDR[3],
                            BankBranchName: pDR[4],
                            AccountName: pDR[5],
                            AccountNumber: pDR[6],
                            CheckNumber: pDR[7],
                            CheckDate: DateString(new Date(pDR[10], pDR[8], pDR[9])),
                            CheckAmount: pDR[11],
                            PaymentType: pDR[14],
                        };
                        var pOtherData = pDR[15];
                        if (pOtherData != null) {
                            pItem.BankID2 = pOtherData.BankID2;
                            pItem.BankName2 = pOtherData.BankName2;
                            pItem.BankBranchName2 = pOtherData.BankBranchName2;
                            pItem.AccountName2 = pOtherData.AccountName2;
                            pItem.AccountNumber2 = pOtherData.AccountNumber2;
                        }
                        pData.Cheques.push(pItem);
                    }

                    for (var p = 0; p <= CacPopup.Card.arrCredit.length - 1; p++) {
                        var pDR = CacPopup.Card.arrCredit[p];

                        /*
                        CacPopup.Card.txtCardID.val(), // 0
                        (cardtype == "Debit" ? "" : CacPopup.Card.selCreditCardType.val()), // 1
                        CacPopup.Card.selCardIssuingBank.val(), // 2
                        CacPopup.Card.selCardIssuingBank.find('option:selected').text(), // 3
                        CacPopup.Card.txtCardLname.val(), // 4
                        CacPopup.Card.txtCardFname.val(), // 5
                        CacPopup.Card.txtCardMname.val(), // 6                                    
                        CacPopup.Card.selCardGenSuffix.val(), // 7
                        CacPopup.Card.selCardBankLocation.val(), // 8
                        CacPopup.Card.selCardBankLocation.find('option:selected').text(), // 9
                        CacPopup.Card.txtCardNo.val(), // 10
                        CacPopup.Card.selCardIssuanceDateMonth.val(), // 11
                        CacPopup.Card.selCardIssuanceDateDay.val(), // 12
                        CacPopup.Card.selCardIssuanceDateYear.val(), // 13
                        CacPopup.Card.selCardExpirationDateMonth.val(), // 14
                        CacPopup.Card.selCardExpirationDateDay.val(), // 15
                        CacPopup.Card.selCardExpirationDateYear.val(), // 16
                        CacPopup.Card.txtCardApprovalNo.val(), // 17
                        CacPopup.Card.txtCardInstallmentNo.val(), // 18
                        CacPopup.Card.txtCardAmount.val(), // 19
                        CacPopup.Card.cdState, // 20
                        cardtype // 21
                        */

                        if (pDR[20] == 'Remove')
                            continue;

                        pData.CreditCards.push({
                            CreditCardType: pDR[1],
                            CreditCardBankID: pDR[2],
                            CreditCardBankName: pDR[3],
                            HolderLastName: pDR[4],
                            HolderFirstName: pDR[5],
                            HolderMiddleName: pDR[6],
                            HolderGenerationalSuffix: pDR[7],
                            BankCountryID: pDR[8],
                            CreditCardNumber: pDR[10],
                            CreditCardIssuanceDate: DateString(new Date(pDR[13], pDR[11], pDR[12])),
                            CreditCardExpirationDate: DateString(new Date(pDR[16], pDR[14], pDR[15])),
                            ApprovalNo: pDR[17],
                            NoOfInstallmentPayments: pDR[18],
                            CreditCardAmount: pDR[19]
                        });
                    } // END for (var p = 0; p <= CacPopup.Card.arrCredit.length - 1; p++) {

                    for (var p = 0; p <= CacPopup.Card.arrDebit.length - 1; p++) {
                        var pDR = CacPopup.Card.arrDebit[p];
                        /*
                        CacPopup.Card.txtCardID.val(), // 0
                        (cardtype == "Debit" ? "" : CacPopup.Card.selCreditCardType.val()), // 1
                        CacPopup.Card.selCardIssuingBank.val(), // 2
                        CacPopup.Card.selCardIssuingBank.find('option:selected').text(), // 3
                        CacPopup.Card.txtCardLname.val(), // 4
                        CacPopup.Card.txtCardFname.val(), // 5
                        CacPopup.Card.txtCardMname.val(), // 6                                    
                        CacPopup.Card.selCardGenSuffix.val(), // 7
                        CacPopup.Card.selCardBankLocation.val(), // 8
                        CacPopup.Card.selCardBankLocation.find('option:selected').text(), // 9
                        CacPopup.Card.txtCardNo.val(), // 10
                        CacPopup.Card.selCardIssuanceDateMonth.val(), // 11
                        CacPopup.Card.selCardIssuanceDateDay.val(), // 12
                        CacPopup.Card.selCardIssuanceDateYear.val(), // 13
                        CacPopup.Card.selCardExpirationDateMonth.val(), // 14
                        CacPopup.Card.selCardExpirationDateDay.val(), // 15
                        CacPopup.Card.selCardExpirationDateYear.val(), // 16
                        CacPopup.Card.txtCardApprovalNo.val(), // 17
                        CacPopup.Card.txtCardInstallmentNo.val(), // 18
                        CacPopup.Card.txtCardAmount.val(), // 19
                        CacPopup.Card.cdState, // 20
                        cardtype // 21
                        */

                        if (pDR[20] == 'Remove')
                            continue;

                        pData.DebitCards.push({
                            DebitCardType: pDR[1],
                            DebitCardBankID: pDR[2],
                            DebitCardBankName: pDR[3],
                            BankCountryID: pDR[8],
                            HolderLastName: pDR[4],
                            HolderFirstName: pDR[5],
                            HolderMiddleName: pDR[6],
                            HolderGenerationalSuffix: pDR[7],
                            DebitCardNumber: pDR[10],
                            DebitCardIssuanceDate: DateString(new Date(pDR[13], pDR[11], pDR[12])),
                            DebitCardExpirationDate: DateString(new Date(pDR[16], pDR[14], pDR[15])),
                            ApprovalNo: pDR[17],
                            NoOfInstallmentPayments: pDR[18],
                            DebitCardAmount: pDR[19]
                        });
                    } // END for (var p = 0; p <= CacPopup.Card.arrDebit.length - 1; p++) {

                    for (var p = 0; p <= CacPopup.OtherPayment.arrOP.length - 1; p++) {
                        var pDR = CacPopup.OtherPayment.arrOP[p];

                        /*
                        CacPopup.OtherPayment.txtOPID.val(), // 0
                        CacPopup.OtherPayment.selOPPaymentType.val(), // 1
                        CacPopup.OtherPayment.txtOPIssuingOrg.val(), // 2
                        CacPopup.OtherPayment.txtOPCardHolder.val(), // 3
                        CacPopup.OtherPayment.txtOPCardNo.val(), // 4
                        CacPopup.OtherPayment.selOPIssuanceDateMonth.val(), // 5
                        CacPopup.OtherPayment.selOPIssuanceDateDay.val(), // 6
                        CacPopup.OtherPayment.selOPIssuanceDateYear.val(), // 7
                        CacPopup.OtherPayment.selOPExpirationDateMonth.val(), // 8
                        CacPopup.OtherPayment.selOPExpirationDateDay.val(), // 9
                        CacPopup.OtherPayment.selOPExpirationDateYear.val(), // 10
                        CacPopup.OtherPayment.txtOPApprovalNo.val(), // 11
                        CacPopup.OtherPayment.txtOPAmount.val(), // 12
                        CacPopup.OtherPayment.opState // 13
                        */

                        if ($.isArray(pDR)) {
                            if (pDR[13] == 'Remove')
                                continue;

                            pData.OtherPayments.push({
                                PaymentType: pDR[1],
                                IssuingOrganization: pDR[2],
                                CardHolderName: pDR[3],
                                CardNumber: pDR[4],
                                CardIssuanceDate: DateString(new Date(pDR[7], pDR[5], pDR[6])),
                                CardExpirationDate: DateString(new Date(pDR[10], pDR[8], pDR[9])),
                                ApprovalNo: pDR[11],
                                Amount: pDR[12]
                            });
                        } else 
                            pData.OtherPayments.push(pDR);
                    } // END for (var p = 0; p <= CacPopup.OtherPayment.arrOP.length - 1; p++) {

                    for (var p = 0; p <= CacPopup.Srv.arrSrv.length - 1; p++) {
                        var pDR = CacPopup.Srv.arrSrv[p];

                        if (pDR[6] == 'Remove')
                            continue;

                        /*
                        this.txtSrvID.val(), // 0
                        this.txtSrvTransMemoID.val(), // 1
                        this.txtSrvNo.val(), // 2
                        this.txtSrvPinCode.val(), // 3
                        this.txtMainSrvAmount.val(), // 4
                        this.txtSrvAmount.val(), // 5
                        this.srvState // 6
                        */

                        pData.SrvPayments.push({
                            SrvNumber: pDR[2],
                            Pin: pDR[3],
                            ConsumedAmount: pDR[5]
                        });
                    } // END for (var p = 0; p <= CacPopup.Srv.arrSrv.length - 1; p++) {

                    pData.UnappliedCreditToUse = mUnappliedCreditToUse;
                    var pUrlParameters = GetUrlParameters();
                    if (pUrlParameters.rsd == '1') {
                        var pInvoiceDate = $('#txtRSDDate').val() + ' ' + $('#txtRSDTime').val();
                        pInvoiceDate = ParseDate(pInvoiceDate);
                        if (pInvoiceDate != null) {
                            pInvoiceDate = DateString(pInvoiceDate);
                            pData.RsdDateTime = pInvoiceDate;
                        }
                    }
                    var pResult = SavePayment(pData);
                    if (!CBool(pResult))
                        NewMessageBox(pResult, 'Unable to proceed');
                } // END Proceed

            } // END function MainSave() {
        } // END function PreSavePayment(vParams) {

        function CreatePdf(vParams) {
            vParams = $.extend({
                Template: null,
                InvoiceID: null
            }, vParams);

            var pInvoiceID = vParams.InvoiceID;
            AjaxSend({
                url: '/ERP/CreatePdf',
                data: {
                    InvoiceID: pInvoiceID,
                    PrintHash: localStorage.getItem('PrintHash'),
                    TransType: 'Invoice',
                    Template: 'PosReceipt'
                },
                success: function (vData) {
                    if (vData.Error) {
                        if (vData.Error.indexOf('PrintHash') == -1) {
                            if (vData.Error == 'UUID is required.')
                                NewMessageBox('You cannot print out documents because your login did not go through the Mobiz Controller app. <br /><br /> \n\n' +
                                    'This is by design to enforce document print controls. <br /><br />\n\n' +
                                    'Please close your Mobiz EAS and then click on the Mobiz Controller app to relaunch Mobiz EAS and enable printing of documents. <br /><br />\n\n' +
                                    'Note: Mobiz Controller is not yet available on None-Windows devices. Please contact Mobiz for methods on printing using a none-Windows device.\n\n', '',
                                    [
                                        {
                                            Text: 'Download Mobiz Controller',
                                            Click: function () {
                                                window.open('/Content/PrintApplet.zip', '_blank');
                                            }
                                        },
                                        {
                                            Text: 'Cancel'
                                        }
                                    ]);
                            else
                                NewMessageBox(vData.Error);
                        } else
                            NoPrintHash();
                    } else if (vData.Result == 'ok')
                        NewMessageBox('Your document will be printed by your local print server.', '', null, {
                            Close: function () {
                                mOut.NewTrans();
                            }
                        });

                    if (proj.IsBootstrapDesign()) {
                        Bs.Loading('hide');
                    }
                }
            });
        }

        function LoadDraft(vID) {
            // vID = TransCustomerPaymentDraft.ID
            mOut.ClearValues();
            AjaxSend({
                url: '/CreditAndCollection/GetDraftPayment',
                data: {
                    ID: vID
                },
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error)
                    else
                        DisplayDraft(vData);
                }
            });
        }

        function DisplayDraft(vData) {
            $('#lblPaidCash').text(AccountingFormat(vData.Cash));
            totalCashRcv = vData.Cash;

            var pRows = $('#tableInvoices').find('tbody tr');
            for (var pSourceIndex = 0; pSourceIndex <= vData.Invoices.length - 1; pSourceIndex++) {
                var pSource = vData.Invoices[pSourceIndex];
                for (var p = 0; p <= pRows.length - 1; p++) {
                    var pTR = $(pRows[p]);
                    var pDR = pTR.data('data');
                    if (ReadyGuid(pDR.ID) == pSource.ID) {
                        pTR.find('[name=AmountPaid]').val(pSource.AmountPaid);
                    }
                }
            }

            totalChequeRcv = 0;
            CacPopup.ChequeGC.arrCGC = [];
            for (var pSourceIndex = 0; pSourceIndex <= vData.Cheques.length - 1; pSourceIndex++) {
                var pSource = vData.Cheques[pSourceIndex];
                var pDest = [];
                pDest[1] = pSource.BankID;
                pDest[2] = pSource.BankName;
                pDest[3] = pSource.BankBranchID;
                pDest[4] = pSource.BankBranchName;
                pDest[5] = pSource.AccountName;
                pDest[6] = pSource.AccountNumber;
                pDest[7] = pSource.CheckNumber;
                var pDate = ParseDate(pSource.CheckDate);
                if (pDate != null) {
                    pDest[10] = pDate.getFullYear();
                    pDest[8] = pDate.getMonth();
                    pDest[9] = pDate.getDate();
                }
                pDest[11] = pSource.CheckAmount;
                totalChequeRcv = CDec2(pSource.CheckAmount);
                CacPopup.ChequeGC.arrCGC.push(pDest);
            }

            $('#lblPaidCheque').text(AccountingFormat(totalChequeRcv));
            if (CDec2(totalChequeRcv) > 0) {
                $('#btnPaymentCheque').addClass('active');
            }                    

            AmountPaid_TextChange();
        }

        function Print() {
            if (mOut.WebPosInvoiceID != null)
                return;

            //if (!CBool(sessionStorage.IsSOALink))
            //    if (CStr2(localStorage.getItem('UUID')).length == 0) {
            //        console.log('wont print coz UUID is blank. ref: jsZATYCJJ3U6');
            //        return;
            //    }

            var pPaymentID;
            if (mOut.Mode == 'AP') {
                if (!IsGuid(localStorage.LastPrintedSupplierPaymentID)) {
                    NewMessageBox('Please make a payment transaction first.');
                    return;
                }
                pPaymentID = localStorage.LastPrintedSupplierPaymentID;
            } else {
                if (!IsGuid(localStorage.LastPrintedCustomerPaymentID)) {
                    NewMessageBox('Please make a payment transaction first.');
                    return;
                }
                pPaymentID = localStorage.LastPrintedCustomerPaymentID;
            }

            if (CBool(sessionStorage.IsSOALink))
                GenerateAcknowledgementReceipt(pPaymentID);
            else
                AjaxSend({
                    url: '/ERP/CreatePaymentPdf',
                    data: {
                        CustomerPaymentID: pPaymentID,
                        UUID: localStorage.getItem('UUID'),
                        Mode: mOut.Mode
                    },
                    success: function(vData) {
                        setTimeout(function() {
                            if (vData.Error) {
                                if (vData.Error.indexOf('PrintHash') == -1)
                                    NewMessageBox(vData.Error);
                                else
                                    NoPrintHash();
                            } else if (vData.Result == 'ok')
                                ViewReport(vData.BaseFilename);
                                NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                        }, 700);
                    }
                });

                function ViewReport(vFilename) {
                    var pUrl = MergeUrlParameters({
                        Filename: vFilename
                    }, '/Erp/DownloadPrintablePdf');
                    window.open(pUrl, '_blank', 
                        'width=' + window.innerWidth + 
                        ',height=' + window.innerHeight + 
                        ',scrollbars=auto');
                } // END function ViewReport() {
        }

        function GenerateAcknowledgementReceipt(vPaymentID) {

            AjaxSend({
                url: '/CreditAndCollection/GenerateAcknowledgementReceipt',
                data: {
                    ID: vPaymentID
                },
                success: function(vData) {
                    ViewReport(vData);
                    if (vData) {
                        NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                    }
                }
            }); // END AjaxSend({

            function ViewReport(vFilename) {
                var pUrl = MergeUrlParameters({
                    Filename: vFilename
                }, '/Erp/DownloadPrintablePdf');
                window.open(pUrl, '_blank', 
                    'width=' + window.innerWidth + 
                    ',height=' + window.innerHeight + 
                    ',scrollbars=auto');
            } // END function ViewReport() {
        } // END function GenerateBilling() {

        function NoPrintHash() {
            if (mDontNoPrintHash)
                return;
            mDontNoPrintHash = true;

            if (mOut.FromEcommerce)
                return;
            if (CStr2(localStorage.getItem('UUID')).length > 0)
                return;

            var pMsg = 'Your Mobiz EAS (Enterprise Application System) needs to download the Mobiz Web Print Application (WPA) to securely print sensitive documents. This is a one-time download and installation process. After the initial installation of Mobiz WPA, you should be able to print documents from your Mobiz EAS quickly and easily.\n\n' +
                'Would you like to download and install Mobiz Web Print Application?';
            NewMessageBox(pMsg, '', [
                {
                    Text: 'Yes',
                    Click: function() {
                        window.open('/Content/PrintApplet.zip', '_blank');
                    }
                },
                {
                    Text: 'No'
                }
            ]);

        }

        $('#divPopUnappliedCreditAmount button[name=ok]').click(function() {
            var pAmount = $('#divPopUnappliedCreditAmount input:text').val();
            if (!IsNumeric(pAmount)) {
                NewMessageBox('Please enter a valid amount.', null, null, {
                    Close: function() {
                        $('#divPopUnappliedCreditAmount').popup('open');
                    }
                });
                return;
            }
            pAmount = CDec2(pAmount);
            if (pAmount < 0) {
                NewMessageBox('Please enter an amount that is equal to or greater than zero.', null, null, {
                    Close: function() {
                        $('#divPopUnappliedCreditAmount').popup('open');
                    }
                });
                return;
            }

            mConsumedUnappliedPayment = pAmount;
            ComputeTotalPaymentRcv();
            $('#divPopUnappliedCreditAmount').modal('hide');
        });
             
        $('#divPopUnappliedCreditAmount button[name=close]').click(function() {
            $('#divPopUnappliedCreditAmount').modal('hide');
        });

        function SavePayment(vParams) {
            vParams = $.extend({
                //Invoices: [
                //    {
                //        ID: '',
                //        AmountPaid: 0
                //    }
                //],
                Cash: 0,
                IsPos: mOut.WebPosInvoiceID != null,
                Success: function(){}
            }, vParams);
            var pVerify = VerifySavePayment(vParams);
            if (!CBool(pVerify)) {
                Bs.Loading('hide');
                return pVerify;
            }

            if (mOut.EmailReceipt) {
                require(['promise!/Scripts/sources/ERP/OrderProcessing/emailAddress/controller.js?v=' + assetversion], function() {
                    Bs.Loading('hide');
                    ERP.OrderProcessing.emailAddress.controller.popup({
                        OnOk: function(vEmailAddresses) {
                            vParams.CustomerEmailAddresses = vEmailAddresses;
                            AjaxSendSavePayment();
                        }
                    });
                });    
            } else
                AjaxSendSavePayment();

            function AjaxSendSavePayment(vAllowOverMaxOverPayment) {
                Bs.Loading('show');
                vParams.AllowOverMaxOverPayment = CBool(vAllowOverMaxOverPayment);
                AjaxSend({
                    url: '/CreditAndCollection/SavePayment',
                    data: vParams,
                    success: function(vData) {
                        var pMessage;
                        var pTable;

                        Bs.Loading('hide');
                        if (vData.Result == 'ok') {
                            if (vParams.Mode == 'AP')
                                localStorage.LastPrintedSupplierPaymentID = vData.ID;
                            else
                                localStorage.LastPrintedCustomerPaymentID = vData.ID;
                            Print();

                            if (CBool(sessionStorage.IsSOALink)) {
                                pMessage = 'Thank you for your payment(s). <br><br>' +
                                    'Your payment will be applied against your payables as soon as we receive and confirm them.<br><br>' +
                                    'For now, would you like to download an acknowledgement receipt and detailed list of payment(s) you entered?<br><br>' +
                                    'You can view your payment history on the customer portal. Please get in touch with your company rep to learn more.'
                            } else {
                                pMessage = 'Payment was successfully saved.';
                            }

                            pTable = CreateDistributionMessage(vData.Distribution.Rows);
                            if (pTable != null)
                                pMessage += '<br /><br />' + pTable;

                            //gibalhin nko diri kay once iclose ang messagebox naay mabilin na black bg by arnie
                            if (proj.IsBootstrapDesign()) {
                                $('#popupCustomerPayment').modal('hide');
                            } else {
                                $('#popupCustomerPayment').hide();
                            }

                            if (!CBool(sessionStorage.IsWebPos))
                                NewMessageBox(pMessage, (CBool(sessionStorage.IsWebPos) ? 'Message' : 'Check Amount Distribution'),
                                (CBool(sessionStorage.IsSOALink) ? [
                                    {
                                        Text: 'Yes, I want to download the acknowledgement receipt',
                                        Click: function() {
                                            //download fx
                                        }
                                    }, { Text: 'No, maybe next time' }
                                ] : null), {
                                    MaxWidth: '97541px',
                                    Close: function() {
                                        if (CBool(sessionStorage.IsWebPos)) {
                                            WebPOS.NewTrans();
                                        }
                                    },
                                    ModalSize: (pTable != null ? 'xl' : '')
                                });
                            var pWebPosInvoiceID = mOut.WebPosInvoiceID;
                            mOut.ClearValues();
                            if (pWebPosInvoiceID == null && !CBool(sessionStorage.IsWebPos)) {
                                mOut.LoadPaymentInvoices();
                                if (mLoadPaymentInvoicesParams != null)
                                  ERP.AccountDetails.controller.GetCustomerCreditDetails(mLoadPaymentInvoicesParams.CustomerID, $('#cbInvoiceFilter').val());
                                if (!mOut.FromEcommerce) {
                                    window.Cac.GetCustomersWithCredit();
                                }
                            }
                            vParams.Success(vData);
                        } else if (vData.Error) {
                            if  (vData.Error.indexOf('Overpayment must be less than ') != -1) {
                                NewMessageBox(vData.Error, 'Warning', [
                                    {
                                        Text: 'Continue Saving',
                                        Click: function() {
                                            AjaxSendSavePayment(true);
                                        }
                                    },
                                    {
                                        Text: 'Don\'t Save',
                                        Click: function() {
                                        }
                                    }
                                ]);
                            } else if (vData.Error.indexOf('vbWUWTEMEXWR') != -1)
                                NewMessageBox('The receipt number you have entered already exists in your company records.<br /><br />' +
                                    'Please check the reference number or add a prefix/suffix to differentiate it from the existing receipt reference number then try saving again. Thank you.',
                                    'Duplicated Receipt Number', null, { HidePrevPopup: false });
                            else
                                NewMessageBox(vData.Error, 'Failed to process payment', null, { HidePrevPopup: false });
                        } else
                            NewMessageBox('Something went wrong.', '', null, { HidePrevPopup: false });
                    }
                }); // END AjaxSend({
            }


            return true;

        } // END function SavePayment

        function CreateDistributionMessage(vDist) {
                    
            if (vDist.length == 0)
                return null;

            var pTable = $('<table class="table table-light table-striped table-hover">');
            var pTBody = $('<tbody>');
            var pTr;
            var pTd;

            pTr = $('<tr>');
            pTr.append($('<th>').text('Check No.'));                    // 1
            pTr.append($('<th>').text('Check Date'));                   // 2
            pTr.append($('<th>').text('Check Amount'));                 // 3
            pTr.append($('<th>').text('Invoice No.'));                  // 4
            pTr.append($('<th>').text('Amount paid by this payment'));  // 5
            pTr.append($('<th>').text('Unapplied Payment'));                  // 6
            pTBody.append(pTr);
            for (var p = 0; p <= vDist.length - 1; p++) {
                var pDR = vDist[p];
                pTr = $('<tr>');
                pTr.append($('<td>').text(pDR.CheckNumber));
                pTr.append($('<td>').text($.format.date(ParseDate(pDR.CheckDate), 'M/d/yyyy')));
                pTr.append($('<td>').text(AccountingFormat(pDR.CheckAmount)));
                pTr.append($('<td>').text(pDR.InvoiceNumber));
                pTr.append($('<td>').text(AccountingFormat(pDR.AmountPaid)));
                pTr.append($('<td>').text(AccountingFormat(pDR.Overpayment)));
                pTBody.append(pTr);
            }

            pTable.append(pTBody);
            // return '<table>' + pTable.html() + '</table>';
            return pTable[0].outerHTML;

        } // END CreateDistributionMessage

        function VerifySavePayment(vParams) {
                    
            var pDocTypesKey; // Receivings or Invoices

            vParams = $.extend({
                //, CurrentPaymentReceived: null
            }, vParams);

            var pTotalPayment = CDec2(vParams.CurrentPaymentReceived);
            var pTotalInvoiceAmountPaid = 0;
            var pMessage;

            if (mOut.Mode == 'AP') {    
                pDocTypesKey = 'Receivings';
                //if (vParams[pDocTypesKey].length == 0) 
                //    return 'Payment must have at least 1 receiving.';
            } else {
                pDocTypesKey = 'Invoices';
                // if (vParams[pDocTypesKey].length == 0) 
                //    return 'Customer must have at least 1 invoice.'; 
            }
                        
            if (pTotalPayment <= 0)
                return 'Full payment must be made to complete this transaction.';
                        
            for (var p = 0; p <= vParams[pDocTypesKey].length - 1; p++) {
                var pDR = vParams[pDocTypesKey][p];
                if (CDec2(pDR.AmountPaid) < 0)
                    return 'Amount paid cannot be lower than zero.';
                if (!IsGuid(pDR.ID))
                    return 'Invoice ID must be a GUID';
                pTotalInvoiceAmountPaid += CDec2(pDR.AmountPaid);
            } // END for

            pTotalInvoiceAmountPaid = parseFloat(pTotalInvoiceAmountPaid.toFixed(2));
            pTotalPayment = parseFloat(pTotalPayment.toFixed(2));
            if (pTotalInvoiceAmountPaid > pTotalPayment) {
                pMessage = 'Total amount paid per ' + (mOut.Mode == 'AP' ? 'receiving' : 'invoice') + ' (' + AccountingFormat(pTotalInvoiceAmountPaid) + ') must be equal to or lesser than the total amount ' + (mOut.Mode == 'AP' ? 'paid' : 'received') + ' (' + AccountingFormat(pTotalPayment) + ').';
                return pMessage;
            }

            return true;
        } // END function VerifySavePayment

        mElement.find('#butPaymentsRefresh').click(function() {
            mOut.LoadPaymentInvoices(mLoadPaymentInvoicesParams);
        });

        mElement.find('[name=butInteractionHistory]').click(function () {
            if (mOut.CustomerID == null) {
                NewMessageBox('Please select ' + (mOut.Mode == 'AP' ? 'vendor' : 'customer') + ' first.');
                return;
            }
            Bs.Loading('show');
            require(['promise!/Scripts/sources/OrganizationShort/notes/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.Company.main.controller.Load({
                    module: mOut.Mode,
                    CompanyID: mOut.CustomerID,
                });
            })
        });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});